/*
 * Copyright 2011, Rowe Technology Inc. 
 * All rights reserved.
 * http://www.rowetechinc.com
 * https://github.com/rowetechinc
 * 
 * Redistribution and use in source and binary forms, with or without modification, are
 * permitted provided that the following conditions are met:
 * 
 *  1. Redistributions of source code must retain the above copyright notice, this list of
 *      conditions and the following disclaimer.
 *      
 *  2. Redistributions in binary form must reproduce the above copyright notice, this list
 *      of conditions and the following disclaimer in the documentation and/or other materials
 *      provided with the distribution.
 *      
 *  THIS SOFTWARE IS PROVIDED BY Rowe Technology Inc. ''AS IS'' AND ANY EXPRESS OR IMPLIED 
 *  WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
 *  FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> OR
 *  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 *  CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 *  SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 *  ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 *  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 *  ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *  
 * The views and conclusions contained in the software and documentation are those of the
 * authors and should not be interpreted as representing official policies, either expressed
 * or implied, of Rowe Technology Inc.
 * 
 * 
 * HISTORY
 * -----------------------------------------------------------------
 * Date            Initials    Version    Comments
 * -----------------------------------------------------------------
 * 09/12/2016      RC          3.3.2      Initial coding
 * 10/21/2016      RC          3.3.2      Fixed bug missing GPS data when connected to computer.
 * 04/27/2017      RC          3.4.2      Check for buffer overflow with _incomingDataTimeout.
 * 10/18/2017      RC          3.4.4      Made the class public.
 * 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI
{
    using RTI.DataSet;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    /// <summary>
    /// Search for the pattery in the byte array.
    /// </summary>
    static class ByteArraySearch
    {

        /// <summary>
        /// Empty array.
        /// </summary>
        static readonly int[] Empty = new int[0];

        /// <summary>
        /// Locate the pattern in the given array.
        /// </summary>
        /// <param name="self">Array to search.</param>
        /// <param name="candidate">Pattern to look for in array.</param>
        /// <returns>List of all the positions where the pattern exists.</returns>
        public static int[] Locate(this byte[] self, byte[] candidate)
        {
            if (IsEmptyLocate(self, candidate))
                return Empty;

            var list = new List<int>();

            for (int i = 0; i < self.Length; i++)
            {
                if (!IsMatch(self, i, candidate))
                    continue;

                list.Add(i);
            }

            return list.Count == 0 ? Empty : list.ToArray();
        }

        /// <summary>
        /// Return flag and position where the pattern matches.
        /// </summary>
        /// <param name="array">Array to search.</param>
        /// <param name="position">Position to search</param>
        /// <param name="candidate">Pattern to look for.</param>
        /// <returns>TRUE if the pattern matches the position.</returns>
        static bool IsMatch(byte[] array, int position, byte[] candidate)
        {
            if (candidate.Length > (array.Length - position))
                return false;

            for (int i = 0; i < candidate.Length; i++)
                if (array[position + i] != candidate[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Check for an empty array.
        /// </summary>
        /// <param name="array">Array to earch</param>
        /// <param name="candidate">Pattern to look for.</param>
        /// <returns>TRUE if empty.</returns>
        static bool IsEmptyLocate(byte[] array, byte[] candidate)
        {
            return array == null
                || candidate == null
                || array.Length == 0
                || candidate.Length == 0
                || candidate.Length > array.Length;
        }
    }

    /// <summary>
    /// Search for pattern in the data to find the beginning of an ensemble.
    /// </summary>
    public class AdcpBinaryCodecNew: ICodec, IDisposable
    {

        #region Variables

        /// <summary>
        /// Setup logger to report errors.
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Size of the NMEA buffer.
        /// </summary>
        private const int NMEA_BUFFER_SIZE = 100;

        /// <summary>
        /// Timeout of a loop.
        /// </summary>
        private const int TIMEOUT_MAX = 20;

        // Header Start Pattern
        private static readonly byte[] pattern = new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80 };

        /// <summary>
        /// A buffer to store the current NMEA data.
        /// </summary>
        private LinkedList<string> _nmeaBuffer;

        /// <summary>
        /// Buffer for the incoming data.
        /// This buffer holds all the individual bytes to be processed.
        /// </summary>
        private byte[] _incomingBuffer;

        /// <summary>
        /// Lock for the incoming buffer.
        /// </summary>
        private readonly object _incomingBufferLock = new object();

        /// <summary>
        /// Lock for the NMEA buffer.
        /// </summary>
        private readonly object _nmeaBufferLock = new object();

        /// <summary>
        /// Thread to decode incoming data.
        /// </summary>
        private Thread _processDataThread;

        /// <summary>
        /// Flag used to stop the thread.
        /// </summary>
        private bool _continue;

        /// <summary>
        /// Event to cause the thread
        /// to go to sleep or wakeup.
        /// </summary>
        private EventWaitHandle _eventWaitData;

        /// <summary>
        /// Flag that we are processing data.
        /// </summary>
        private bool _processingData;

        /// <summary>
        /// This is a timeout to wait for the buffer to increase.
        /// If after 5 tries, the buffer has not increased enough to get
        /// a whole ensemble, we have to assume the end of a file or
        /// transimission and no more data will come, so clear out the buffer.
        /// </summary>
        private int _timeoutBuffer;

        /// <summary>
        /// Buffer timeout.
        /// </summary>
        private const int BUFFER_TIMEOUT = 5;

        /// <summary>
        /// Set a timeout if the incoming data is accumulating and
        /// never finding any ensembles.
        /// </summary>
        private int _incomingDataTimeout;

        /// <summary>
        /// Number of times to take incoming data before a timeout
        /// occurs and the data is cleared.
        /// </summary>
        private const int INCOMING_DATA_TIMEOUT = 50;

        // <summary>
        // TCP Server.
        // </summary>
        //private TcpServer _tcpServer;

        #endregion

        #region Struct 

        /// <summary>
        /// Ensemble Info.
        /// </summary>
        public struct EnsInfo
        {
            /// <summary>
            /// Ensemble size.
            /// </summary>
            public int EnsSize { get; set; }

            /// <summary>
            /// Good Ensemble.
            /// </summary>
            public bool EnsGood { get; set; }

            /// <summary>
            /// Ensemble Byte array.
            /// </summary>
            public byte[] RawEnsemble { get; set; }

            /// <summary>
            /// Ensemble object.
            /// </summary>
            public DataSet.Ensemble Ensemble { get; set; }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// Initialize values.
        /// </summary>
        public AdcpBinaryCodecNew()
        {
            _nmeaBuffer = new LinkedList<string>();

            // Initialize buffer
            _incomingBuffer = new byte[0];
            //_buffer = new BlockingCollection<byte[]>();
            //_processingData = false;
            _timeoutBuffer = 0;
            _incomingDataTimeout = 0;

            //try
            //{
            //    // Create UDP client
            //    _tcpServer = new TcpServer(RTI.Core.Commons.TCP_ENS);
            //}
            //catch(Exception)
            //{
            //    // The server can only be started once
            //    // So if Pulse is opened again, the first connection got it.
            //    _tcpServer = null;
            //}


            // Initialize the thread
            _continue = true;
            _eventWaitData = new EventWaitHandle(false, EventResetMode.AutoReset);
            _processDataThread = new Thread(ProcessDataThread);
            _processDataThread.Name = "ADCP Binary Codec new";
            _processDataThread.Start();

        }

        /// <summary>
        /// Shutdown this object.
        /// </summary>
        public void Dispose()
        {
            StopThread();

            ClearIncomingData();
            _incomingBuffer = null;
            _eventWaitData.Dispose();
        }


        /// <summary>
        /// Take incoming data and add it to the
        /// buffer to be decoded.
        /// 
        /// Then start the thread to start decoding data
        /// </summary>
        /// <param name="data">Data to add to incoming buffer.</param>
        public void AddIncomingData(byte[] data)
        {
            if (data != null)
            {
                //_buffer.Add(data);

                lock (_incomingBuffer)
                {
                    // Reset buffer timeout
                    _timeoutBuffer = 0;

                    // Make a deep copy of the data and replace the old array
                    byte[] temp = new byte[data.Length + _incomingBuffer.Length];
                    Buffer.BlockCopy(_incomingBuffer, 0, temp, 0, _incomingBuffer.Length);
                    Buffer.BlockCopy(data, 0, temp, _incomingBuffer.Length, data.Length);
                    _incomingBuffer = temp;
                }
            }

            if (!_processingData)
            {
                // Wake up the thread to process data
                _eventWaitData.Set();
            }

            // Check timeout
            _incomingDataTimeout++;
            if(_incomingDataTimeout > INCOMING_DATA_TIMEOUT)
            {
                // Reset the value and clear the data
                _incomingDataTimeout = 0;
                ClearIncomingData();
            }
        }

        /// <summary>
        /// Clear the incoming data.
        /// </summary>
        public void ClearIncomingData()
        {
            lock (_incomingBuffer)
            {
                _incomingBuffer = new byte[0];
            }

            lock (_nmeaBufferLock)
            {
                _nmeaBuffer.Clear();
            }
        }

        /// <summary>
        /// Decode the ADCP data received using a seperate
        /// thread.  This thread will also pass data to all
        /// _observers.
        /// </summary>
        private void ProcessDataThread()
        {
            while (_continue)
            {
                try
                {
                    // Block until awoken when data is received
                    // Timeout every 60 seconds to see if shutdown occured
                    _eventWaitData.WaitOne(1000);

                    _processingData = true;

                    // If wakeup was called to kill thread
                    if (!_continue)
                    {
                        return;
                    }

                    // Accumulate the buffer
                    // Then decode the data
                    // Lock it so the buffer is not modified while decoding
                    lock (_incomingBufferLock)
                    {

                        // Continue decoding if there is data in the buffer
                        while (_incomingBuffer.Length > 1000)
                        {
                            // Process all data in the buffer
                            DecodeIncomingData();
                        }
                    }

                    // If wakeup was called to kill thread
                    if (!_continue)
                    {
                        return;
                    }
                }
                catch (ThreadAbortException)
                {
                    // Thread is aborted to stop processing
                    return;
                }
                catch (Exception e)
                {
                    log.Error("Error processing binary codec data.", e);
                    return;
                }

                // Send an event that processing is complete
                if (ProcessDataCompleteEvent != null)
                {
                    ProcessDataCompleteEvent();
                }

                _processingData = false;
            }
        }

        /// <summary>
        /// Stop the thread.
        /// </summary>
        private void StopThread()
        {
            _continue = false;

            try
            {
                // Wake up the thread to stop thread
                _eventWaitData.Set();
                //_eventWaitData.Dispose();

                // Force the thread to stop
                //_processDataThread.Abort();
                _processDataThread.Join(1000);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Store the latest NMEA data to a buffer.
        /// As new ensembles are record, the buffer will
        /// be flushed.
        /// </summary>
        /// <param name="nmeaData">String of NMEA data.  Can be multiple lines.</param>
        public void AddNmeaData(string nmeaData)
        {
            lock (_nmeaBufferLock)
            {
                // Add the data to the buffer
                _nmeaBuffer.AddLast(nmeaData);

                // Ensure the buffer does not get to large
                if (_nmeaBuffer.Count > NMEA_BUFFER_SIZE)
                {
                    _nmeaBuffer.RemoveFirst();
                }
            }
        }

        #region Parse Data

        /// <summary>
        /// Decode the incoming data for ensemble data.
        /// First search for header.  When a header is found,
        /// verify it is a good header.  When the header is
        /// verified, get the payload size.  Determine the
        /// buffer contains the entire ensemble.  If the
        /// entire ensemble is present in the buffer, retreive
        /// the ensemble and place in a seperate byte array
        /// and remove it from the buffer.  Then decode the
        /// ensemble for all the ranges present.
        /// </summary>
        private void DecodeIncomingData()
        {
            // Get a list of all the start positions in the file
            var posList = _incomingBuffer.Locate(pattern);
            while(posList.Length > 0)
            {
                // Find the first position in the array with the given pattern
                int position = posList.First();
                
                // Verify the buffer has enough room to look for the header
                if (_incomingBuffer.Length < position + DataSet.Ensemble.HEADER_START_ENSNUM_PAYLOAD_COUNT)
                {
                    // Break and wait for more data in the buffer
                    break;
                }

                //Debug.WriteLine("Ensemble Pos: " + position);

                // Get Ensembles
                EnsInfo ens = VerifyEnsemble(position);
                if (ens.EnsGood)
                {
                    // Get the checksum values
                    long calculatedChecksum = DataSet.Ensemble.CalculateEnsembleChecksum(ens.RawEnsemble);
                    long ensembleChecksum = RetrieveEnsembleChecksum(ens.RawEnsemble);

                    // Verify the checksum match
                    if (calculatedChecksum == ensembleChecksum)
                    {
                        // Decode the binary data
                        var pak = DecodeAdcpData(ens.RawEnsemble);

                        // Set the values
                        ens.Ensemble = pak.Ensemble;
                        ens.RawEnsemble = pak.RawEnsemble;

                        // Publish the data
                        // Send an event that data was processed
                        // in this format
                        if (ProcessDataEvent != null)
                        {
                            ProcessDataEvent(ens.RawEnsemble, ens.Ensemble);
                        }

                        //if (_tcpServer != null)
                        //{
                        //    // Send the ensemble to UDP port
                        //    _tcpServer.Write(ens.Ensemble.EncodeJSON() + "\n");
                        //}

                        // Reset the incoming data timeout
                        _incomingDataTimeout = 0;

                        //Debug.WriteLine("Ens: " + ens.Ensemble.EnsembleData.EnsembleNumber);
                    }
                }

                // Reset the position list
                posList = _incomingBuffer.Locate(pattern);
            }

            // Clear the buffer
            _incomingBuffer = new byte[0];
        }

        /// <summary>
        /// Remove the ensemble from the beginning of the buffer.
        /// </summary>
        /// <param name="ensSize">Size of the ensemble to remove.</param>
        private void RemoveEnsembleFromBuffer(int ensSize)
        {
            if (_incomingBuffer.Length >= ensSize)
            {
                byte[] newArray = new byte[_incomingBuffer.Length - ensSize];
                Buffer.BlockCopy(_incomingBuffer, ensSize, newArray, 0, newArray.Length);

                _incomingBuffer = newArray;
            }
        }

        /// <summary>
        /// Parse the incoming packet for all the Data Sets.
        /// Add the data to a AdcpDataSet variable and 
        /// return the filled variable when complete.
        /// </summary>
        /// <param name="binaryEnsemble">Byte array containing data from an ADCP.</param>
        /// <returns>Object holding decoded ADCP data.</returns>
        public DataSet.EnsemblePackage DecodeAdcpData(byte[] binaryEnsemble)
        {
            // Keep track where in the packet
            // we are currently decoding
            int packetPointer = DataSet.Ensemble.ENSEMBLE_HEADER_LEN;
            int type = 0;
            int numElements = 0;
            int elementMultiplier = 0;
            int imag = 0;
            int nameLen = 0;
            string name = "";
            int dataSetSize = 0;

            Ensemble ensemble = new Ensemble();

            for (int i = 0; i < DataSet.Ensemble.MAX_NUM_DATA_SETS; i++)
            {
                //Debug.Print("binaryEnsemble: " + binaryEnsemble.Length + " packetPointer: " + packetPointer + "\n");
                type = MathHelper.ByteArrayToInt32(binaryEnsemble, packetPointer + (DataSet.Ensemble.BYTES_IN_INT32 * 0));
                numElements = MathHelper.ByteArrayToInt32(binaryEnsemble, packetPointer + (DataSet.Ensemble.BYTES_IN_INT32 * 1));
                elementMultiplier = MathHelper.ByteArrayToInt32(binaryEnsemble, packetPointer + (DataSet.Ensemble.BYTES_IN_INT32 * 2));
                imag = MathHelper.ByteArrayToInt32(binaryEnsemble, packetPointer + (DataSet.Ensemble.BYTES_IN_INT32 * 3));
                nameLen = MathHelper.ByteArrayToInt32(binaryEnsemble, packetPointer + (DataSet.Ensemble.BYTES_IN_INT32 * 4));
                name = MathHelper.ByteArrayToString(binaryEnsemble, 8, packetPointer + (DataSet.Ensemble.BYTES_IN_FLOAT * 5));

                // Verify the data is good
                if (string.IsNullOrEmpty(name))
                {
                    break;
                }

                //Debug.Print("name: " + name + "\n");
                //Debug.Print("numElements: " + numElements + "\n");
                //Debug.Print("elementMultiplier" + elementMultiplier + "\n");

                // Get the size of this data set
                dataSetSize = BaseDataSet.GetDataSetSize(type, nameLen, numElements, elementMultiplier);

                if (Ensemble.BeamVelocityID.Equals(name, StringComparison.Ordinal))
                {
                    // Create a sub array of just this data set data
                    byte[] velData = MathHelper.SubArray<byte>(binaryEnsemble, packetPointer, dataSetSize);

                    // Add the data
                    ensemble.AddBeamVelocityData(type, numElements, elementMultiplier, imag, nameLen, name, velData);
                    //Debug.WriteLine(adcpData.BeamVelocityData.ToString());

                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }
                else if (Ensemble.InstrumentVelocityID.Equals(name, StringComparison.Ordinal))
                {
                    // Create a sub array of just this data set data
                    byte[] velData = MathHelper.SubArray<byte>(binaryEnsemble, packetPointer, dataSetSize);

                    // Add the data
                    ensemble.AddInstrumentVelocityData(type, numElements, elementMultiplier, imag, nameLen, name, velData);
                    //Debug.WriteLine(adcpData.InstrVelocityData.ToString());

                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }
                else if (Ensemble.EarthVelocityID.Equals(name, StringComparison.Ordinal))
                {
                    // Create a sub array of just this data set data
                    byte[] velData = MathHelper.SubArray<byte>(binaryEnsemble, packetPointer, dataSetSize);

                    // Add the data
                    ensemble.AddEarthVelocityData(type, numElements, elementMultiplier, imag, nameLen, name, velData);
                    //Debug.WriteLine(adcpData.EarthVelocityData.ToString());

                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }
                else if (Ensemble.AmplitudeID.Equals(name, StringComparison.Ordinal))
                {
                    // Create a sub array of just this data set data
                    byte[] ampData = MathHelper.SubArray<byte>(binaryEnsemble, packetPointer, dataSetSize);

                    // Add the data
                    ensemble.AddAmplitudeData(type, numElements, elementMultiplier, imag, nameLen, name, ampData);
                    //Debug.WriteLine(adcpData.AmplitudeData.ToString());

                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }
                else if (Ensemble.CorrelationID.Equals(name, StringComparison.Ordinal))
                {
                    // Create a sub array of just this data set data
                    byte[] corrData = MathHelper.SubArray<byte>(binaryEnsemble, packetPointer, dataSetSize);

                    // Add the data
                    ensemble.AddCorrelationData(type, numElements, elementMultiplier, imag, nameLen, name, corrData);
                    //Debug.WriteLine(adcpData.CorrelationData.ToString());

                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }
                else if (Ensemble.GoodBeamID.Equals(name, StringComparison.Ordinal))
                {
                    // Create a sub array of just this data set data
                    byte[] goodBeamData = MathHelper.SubArray<byte>(binaryEnsemble, packetPointer, dataSetSize);

                    // Add the data
                    ensemble.AddGoodBeamData(type, numElements, elementMultiplier, imag, nameLen, name, goodBeamData);
                    //Debug.WriteLine(adcpData.GoodBeamData.ToString());

                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }
                else if (Ensemble.GoodEarthID.Equals(name, StringComparison.Ordinal))
                {
                    // Create a sub array of just this data set data
                    byte[] goodEarthData = MathHelper.SubArray<byte>(binaryEnsemble, packetPointer, dataSetSize);

                    // Add the data
                    ensemble.AddGoodEarthData(type, numElements, elementMultiplier, imag, nameLen, name, goodEarthData);
                    //Debug.WriteLine(adcpData.GoodEarthData.ToString());

                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }
                else if (Ensemble.EnsembleDataID.Equals(name, StringComparison.Ordinal))
                {
                    // Create a sub array of just this data set data
                    byte[] ensembleData = MathHelper.SubArray<byte>(binaryEnsemble, packetPointer, dataSetSize);

                    // Add the data
                    ensemble.AddEnsembleData(type, numElements, elementMultiplier, imag, nameLen, name, ensembleData);
                    //Debug.WriteLine(adcpData.EnsembleData.ToString());

                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }
                else if (Ensemble.AncillaryID.Equals(name, StringComparison.Ordinal))
                {
                    // Create a sub array of just this data set data
                    byte[] ancillaryData = MathHelper.SubArray<byte>(binaryEnsemble, packetPointer, dataSetSize);

                    // Add the data
                    ensemble.AddAncillaryData(type, numElements, elementMultiplier, imag, nameLen, name, ancillaryData);
                    //Debug.WriteLine(adcpData.AncillaryData.ToString());

                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }
                else if (Ensemble.BottomTrackID.Equals(name, StringComparison.Ordinal))
                {
                    // Create a sub array of just this data set data
                    byte[] bottomTrackData = MathHelper.SubArray<byte>(binaryEnsemble, packetPointer, dataSetSize);

                    // Add the data
                    ensemble.AddBottomTrackData(type, numElements, elementMultiplier, imag, nameLen, name, bottomTrackData);
                    //Debug.WriteLine(adcpData.BottomTrackData.ToString());

                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }
                else if (Ensemble.NmeaID.Equals(name, StringComparison.Ordinal))
                {
                    // List of all data read
                    byte[] nmeaData = new byte[dataSetSize];

                    // Scan through the data set and store all the data
                    for (int x = 0; x < dataSetSize; x++)
                    {
                        nmeaData[x] = binaryEnsemble[packetPointer++];
                    }

                    // Add the data
                    ensemble.AddNmeaData(type, numElements, elementMultiplier, imag, nameLen, name, nmeaData);
                    //Debug.WriteLine(adcpData.NmeaData.ToString());
                }
                else if (Ensemble.ProfileEngineeringID.Equals(name, StringComparison.Ordinal))
                {
                    // Create a sub array of just this data set data
                    byte[] peData = MathHelper.SubArray<byte>(binaryEnsemble, packetPointer, dataSetSize);

                    // Add the data
                    ensemble.AddProfileEngineeringData(type, numElements, elementMultiplier, imag, nameLen, name, peData);
                    //Debug.WriteLine(adcpData.BottomTrackData.ToString());

                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }
                else if (Ensemble.BottomTrackEngineeringID.Equals(name, StringComparison.Ordinal))
                {
                    // Create a sub array of just this data set data
                    byte[] bteData = MathHelper.SubArray<byte>(binaryEnsemble, packetPointer, dataSetSize);

                    // Add the data
                    ensemble.AddBottomTrackEngineeringData(type, numElements, elementMultiplier, imag, nameLen, name, bteData);
                    //Debug.WriteLine(adcpData.BottomTrackData.ToString());

                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }
                else if (Ensemble.SystemSetupID.Equals(name, StringComparison.Ordinal))
                {
                    // Create a sub array of just this data set data
                    byte[] ssData = MathHelper.SubArray<byte>(binaryEnsemble, packetPointer, dataSetSize);

                    // Add the data
                    ensemble.AddSystemSetupData(type, numElements, elementMultiplier, imag, nameLen, name, ssData);
                    //Debug.WriteLine(adcpData.BottomTrackData.ToString());

                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }
                else if (Ensemble.RangeTrackingID.Equals(name, StringComparison.Ordinal))
                {
                    // Create a sub array of just this data set data
                    byte[] rtData = MathHelper.SubArray<byte>(binaryEnsemble, packetPointer, dataSetSize);

                    // Add the data
                    ensemble.AddRangeTrackingData(type, numElements, elementMultiplier, imag, nameLen, name, rtData);
                    //Debug.WriteLine(adcpData.RangeTrackingData.ToString());

                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }
                else if (Ensemble.GageHeightID.Equals(name, StringComparison.Ordinal))
                {
                    // Create a sub array of just this data set data
                    byte[] ghData = MathHelper.SubArray<byte>(binaryEnsemble, packetPointer, dataSetSize);

                    // Add the data
                    ensemble.AddGageHeightData(type, numElements, elementMultiplier, imag, nameLen, name, ghData);
                    //Debug.WriteLine(adcpData.GageHeightData.ToString());

                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }
                else if (Ensemble.Adcp2InfoID.Equals(name, StringComparison.Ordinal))
                {
                    // Create a sub array of just this data set data
                    byte[] adcp2InfoData = MathHelper.SubArray<byte>(binaryEnsemble, packetPointer, dataSetSize);

                    // Add the data
                    ensemble.AddAdcp2InfoData(type, numElements, elementMultiplier, imag, nameLen, name, adcp2InfoData);
                    //Debug.WriteLine(adcpData.GageHeightData.ToString());

                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }
                else
                {
                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }


                //Debug.Print("DataSetSize: " + dataSetSize + "\n");
                //Debug.Print(" packetPointer: " + packetPointer + "\n");
                if (packetPointer + 4 >= binaryEnsemble.Length || packetPointer < 0)
                    break;
            }

            // If NMEA data is available, add it to the dataset
            // Add it to both the binary data and ensemble object
            if (_nmeaBuffer.Count > 0)
            {
                // Create a NMEA dataset
                MergeNmeaDataSet(ref ensemble);

                // Add the NMEA binary data to binary data
                if (ensemble.IsNmeaAvail)
                {
                    MergeNmeaBinary(ref binaryEnsemble, ensemble.NmeaData);
                }
            }

            // Send an event that data was processed
            // in this format
            //if (ProcessDataEvent != null)
            //{
            //    ProcessDataEvent(binaryEnsemble, ensemble);
            //}

            // Set the values to return 
            var pak = new DataSet.EnsemblePackage();
            pak.Ensemble = ensemble;
            pak.RawEnsemble = binaryEnsemble;

            return pak;
        }

        /// <summary>
        /// Add NMEA data to the ensemble.
        /// </summary>
        /// <param name="adcpData">DataSet to add NMEA data.</param>
        private void MergeNmeaDataSet(ref DataSet.Ensemble adcpData)
        {
            // Copy the data from the buffer
            // This will take a current cout of the buffer.
            // Then create a string of the buffer and remove
            // the item from the buffer at the same time.
            StringBuilder nmeaData = new StringBuilder();

            // Copy the buffer so it can be unlocked
            LinkedList<string> bufferCopy;
            lock (_nmeaBufferLock)
            {
                // Copy the buffer then clear it
                bufferCopy = new LinkedList<string>(_nmeaBuffer);
                _nmeaBuffer.Clear();
            }

            // Create a string of all the buffered data
            for (int x = 0; x < bufferCopy.Count; x++)
            {
                nmeaData.Append(bufferCopy.First.Value);

                // Remove the data
                bufferCopy.RemoveFirst();
            }

            // Check if NMEA data already exsit, if it does, combine the data
            if (adcpData.IsNmeaAvail)
            {
                // Merge the NMEA data with the new nmea data
                adcpData.NmeaData.MergeNmeaData(nmeaData.ToString());
            }
            else
            {
                // Add the NMEA data to the dataset
                adcpData.AddNmeaData(nmeaData.ToString());
            }
        }

        /// <summary>
        /// Add NMEA data to the binary data.
        /// </summary>
        /// <param name="ensemble">DataSet to add NMEA data.</param>
        /// <param name="nmea">NMEA dataset to add.</param>
        private void MergeNmeaBinary(ref byte[] ensemble, NmeaDataSet nmea)
        {
            ensemble = Ensemble.AddDataSet(ensemble, nmea.Encode());
        }

        /// <summary>
        /// The ensemble number and size and 1's complement of
        /// the ensemble number and size are stored in the header.
        /// Retrieve these ranges.  Then invert the 1's compliment
        /// and determine if the number match.  If they do not match
        /// then the header is incorrect.  If they do match then
        /// return true.
        /// </summary>
        /// <returns>The value and 1's compliment match for the ensemble number and size.</returns>
        private EnsInfo VerifyEnsemble(int startPos)
        {
            // Find ensemble number and ensemble size
            long EnsNum = 0;
            long NotEnsNum = 1;
            int payloadSize = 0;
            long NotEnsSiz = 1;
            int i = startPos + DataSet.Ensemble.HEADER_START_COUNT;     // 16 byte Header + startPos

            EnsInfo ensInfo = new EnsInfo();
            ensInfo.EnsGood = false;

            // Get Ensemble number and inverse
            EnsNum = _incomingBuffer[i++];
            EnsNum += _incomingBuffer[i++] << 8;
            EnsNum += _incomingBuffer[i++] << 16;
            EnsNum += _incomingBuffer[i++] << 24;

            NotEnsNum = _incomingBuffer[i++];
            NotEnsNum += _incomingBuffer[i++] << 8;
            NotEnsNum += _incomingBuffer[i++] << 16;
            NotEnsNum += _incomingBuffer[i++] << 24;
            NotEnsNum = ~NotEnsNum;

            // Get payload and inverse
            payloadSize = _incomingBuffer[i++];
            payloadSize += _incomingBuffer[i++] << 8;
            payloadSize += _incomingBuffer[i++] << 16;
            payloadSize += _incomingBuffer[i++] << 24;

            NotEnsSiz = _incomingBuffer[i++];
            NotEnsSiz += _incomingBuffer[i++] << 8;
            NotEnsSiz += _incomingBuffer[i++] << 16;
            NotEnsSiz += _incomingBuffer[i++] << 24;
            NotEnsSiz = ~NotEnsSiz;

            // Determine if the inverted 1's compliment matches
            // the actual value
            if (EnsNum == NotEnsNum && payloadSize == NotEnsSiz)
            {
                // Set the ensemble size
                ensInfo.EnsSize = DataSet.Ensemble.CalculateEnsembleSize(payloadSize);

                // Make sure the buffer contains enough data to take the entire ensemble
                if (ensInfo.EnsSize > 0 && _incomingBuffer.Length >= startPos + ensInfo.EnsSize)
                {
                    // Store the current value
                    // Include the checksum size to the ensemble size
                    ensInfo.EnsGood = true;
                    ensInfo.RawEnsemble = new byte[ensInfo.EnsSize];
                    Buffer.BlockCopy(_incomingBuffer, startPos, ensInfo.RawEnsemble, 0, ensInfo.EnsSize);

                    // Remove ensemble from buffer
                    RemoveEnsembleFromBuffer(startPos + ensInfo.EnsSize);
                }
                else if(_timeoutBuffer >= BUFFER_TIMEOUT)
                {
                    // TIMEOUT has occurred, so no new data has arrived and
                    // we do not have enough data in the buffer for the entire ensemble
                    // Clear the buffer
                    _incomingBuffer = new byte[0];
                }
                else
                {
                    _timeoutBuffer++;

                    // Wait for data in the buffer
                    Thread.Sleep(250);
                }
            }
            else
            {
                // Data in header was bad, find next header
                // Assume we are removing the 16 byte header to find the next header
                RemoveEnsembleFromBuffer(startPos + 16);
            }

            return ensInfo;
        }

        /// <summary>
        /// Get the checksum value for the ensemble.  It is the 
        /// last 4 bytes of the ensemble.  The last 2 bytes should
        /// be 0's if you visually inspect the ensemble.
        /// This will find the checksum value and convert the value
        /// to a long.
        /// </summary>
        /// <param name="ensemble">Good ensemble containing a checksum value.</param>
        /// <returns>Checksum value converted from byte array to long.</returns>
        public long RetrieveEnsembleChecksum(byte[] ensemble)
        {
            long checksum = ensemble[ensemble.Length - DataSet.Ensemble.CHECKSUM_SIZE];
            checksum += ensemble[ensemble.Length - 3] << 8;
            checksum += ensemble[ensemble.Length - 2] << 16;
            checksum += ensemble[ensemble.Length - 1] << 24;

            return checksum;
        }

        /// <summary>
        /// Calculate a NMEA string checksum.
        /// </summary>
        /// <param name="sentence">NMEA string to calculate.</param>
        /// <returns>Checksum value as a string.</returns>
        public string CalculateNMEAChecksum(string sentence)
        {
            // Loop through all chars to get a checksum
            int Checksum = 0;
            foreach (char Character in sentence)
            {
                if (Character == '$')
                {
                    // Ignore the dollar sign
                }
                else if (Character == '*')
                {
                    // Stop processing before the asterisk
                    break;
                }
                else
                {
                    // Is this the first value for the checksum?
                    if (Checksum == 0)
                    {
                        // Yes. Set the checksum to the value
                        Checksum = Convert.ToByte(Character);
                    }
                    else
                    {
                        // No. XOR the checksum with this character's value
                        Checksum = Checksum ^ Convert.ToByte(Character);
                    }
                }
            }
            // Return the checksum formatted as a two-character hexadecimal
            return Checksum.ToString("X2");
        }

        #endregion

        #region Events

        /// <summary>
        /// Event To subscribe to.  This gives the paramater
        /// that will be passed when subscribing to the event.
        /// </summary>
        /// <param name="ensemble">Byte array of raw ensemble data.</param>
        /// <param name="adcpData">Ensemble data as an object.</param>
        public delegate void ProcessDataEventHandler(byte[] ensemble, DataSet.Ensemble adcpData);

        /// <summary>
        /// Subscribe to receive event when data has been successfully
        /// processed.  This can be used to tell if data is in this format
        /// and is being processed or is not in this format.
        /// Subscribe to this event.  This will hold all subscribers.
        /// 
        /// To subscribe:
        /// adcpBinaryCodec.ProcessDataEvent += new adcpBinaryCodec.ProcessDataEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// adcpBinaryCodec.ProcessDataEvent -= (method to call)
        /// </summary>
        public event ProcessDataEventHandler ProcessDataEvent;

        /// <summary>
        /// Event To subscribe to.  This gives the paramater
        /// that will be passed when subscribing to the event.
        /// </summary>
        public delegate void ProcessDataCompleteEventHandler();

        /// <summary>
        /// Subscribe to know when the entire file has been processed.
        /// This event will be fired when there is no more data in the 
        /// buffer to decode.
        /// 
        /// To subscribe:
        /// adcpBinaryCodec.ProcessDataCompleteEvent += new adcpBinaryCodec.ProcessDataCompleteEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// adcpBinaryCodec.ProcessDataCompleteEvent -= (method to call)
        /// </summary>
        public event ProcessDataCompleteEventHandler ProcessDataCompleteEvent;

        #endregion
    }

}