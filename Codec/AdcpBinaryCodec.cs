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
 * 09/14/2011      RC                     Initial coding
 * 10/14/2011      RC                     Replaced 4 with CHECKSUM_SIZE in some calculations
 *                                         Removed stopping thread when clearing buffer
 * 10/18/2011      RC                     Properly shutdown object
 * 10/19/2011      RC                     Replaced with CurrentDataSetManager.DistributeCurrentDataSet() with RecordDataset().
 * 10/24/2011      RC                     Added a way to make a cache stored in a dictionary.
 * 11/30/2011      RC                     Added ICodec.
 * 12/09/2011      RC          1.09       Changed parsing Nmea dataset.
 *                                         Made a public class.
 * 12/12/2011      RC          1.09       Write parsed data to file and database.
 *                                         Queue data coming in and add to byte array in thread.
 * 12/14/2011      RC          1.09       Added ProcessDataEvent event.
 * 12/15/2011      RC          1.09       Lock the buffer in all locations where accessed.
 * 12/15/2011      RC          1.10       Got dataset size using method in BaseDataSet.
 * 12/16/2011      RC          1.10       Check if data is null when adding to queue.
 * 12/19/2011      RC          1.10       Event passes data to record to all subscribers.
 * 12/20/2011      RC          1.10       Added Nmea Buffer Lock.
 * 02/07/2012      RC          2.00       Changed to a while loop to process the queue in the thread.
 * 02/23/2012      RC          2.07       Add the incoming data to the buffer not using a for loop.
 * 03/30/2012      RC          2.07       Moved Converters.cs methods to MathHelper.cs.
 * 06/21/2012      RC          2.12       Remove the queue and put the data in the buffer when received.
 * 03/20/2013      RC          2.19       If VerifyEnsembleNumberAndSize() fails in DecodingIncomingData(), i remove a byte from the buffer and try again.
 * 06/28/2013      RC          2.19       Replaced Shutdown() with IDisposable.
 * 09/12/2013      RC          2.20.0     Added buffer lock to AddIncomingData().
 * 
 */

using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;

using RTI.DataSet;
using System.Threading;
using System.Collections;
using System.Text;

namespace RTI
{
    /// <summary>
    /// Decode RTI Binary ADCP files.
    /// This will parse the data into an 
    /// Ensemble object.
    /// </summary>
    public class AdcpBinaryCodec : ICodec, IDisposable
    {

        #region Variables 

        /// <summary>
        /// Size of the NMEA buffer.
        /// </summary>
        private const int NMEA_BUFFER_SIZE = 100;

        /// <summary>
        /// Timeout of a loop.
        /// </summary>
        private const int TIMEOUT_MAX = 20;

        /// <summary>
        /// A buffer to store the current NMEA data.
        /// </summary>
        private LinkedList<string> _nmeaBuffer;

        /// <summary>
        /// Buffer for the incoming data.
        /// This buffer holds all the individual bytes to be processed.
        /// </summary>
        private List<Byte> _incomingDataBuffer;

        /// <summary>
        /// Lock for the buffer.
        /// </summary>
        private readonly object _bufferLock = new object();

        /// <summary>
        /// Lock for the NMEA buffer.
        /// </summary>
        private readonly object _nmeaBufferLock = new object();

        /// <summary>
        /// Current size of the ensemble being processed.
        /// This is calculated while decoding the current
        /// ensemble.
        /// </summary>
        private int _currentEnsembleSize;

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

        #endregion

        /// <summary>
        /// Constructor
        /// Initialize values.
        /// </summary>
        public AdcpBinaryCodec()
        {
            _nmeaBuffer = new LinkedList<string>();

            // Initialize buffer
            //_incomingDataQueue = new Queue<byte[]>();
            _incomingDataBuffer = new List<Byte>();

            // Initialize the ensemble size to at least the HDRLEN
            _currentEnsembleSize = DataSet.Ensemble.ENSEMBLE_HEADER_LEN;

            // Initialize the thread
            _continue = true;
            _eventWaitData = new EventWaitHandle(false, EventResetMode.AutoReset);
            _processDataThread = new Thread(ProcessDataThread);
            _processDataThread.Start();
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
            if (data != null && data.Length > 0)
            {
                // Lock the buffer while clearing
                lock (_bufferLock)
                {
                    _incomingDataBuffer.AddRange(data);
                }
            }

            // Wake up the thread to process data
            _eventWaitData.Set();
        }

        /// <summary>
        /// Store the latest NMEA data to a buffer.
        /// As new Datasets are record, the buffer will
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

        /// <summary>
        /// Clear the incoming buffer of all data.
        /// </summary>
        public void ClearIncomingData()
        {
            // Lock the buffer while clearing
            lock (_bufferLock)
            {
                // Clear the buffer
                _incomingDataBuffer.Clear();
            }

            lock (_nmeaBufferLock)
            {
                _nmeaBuffer.Clear();
            }
        }

        /// <summary>
        /// Shutdown this object.
        /// </summary>
        public void Dispose()
        {
            StopThread();
        }

        #region Parse Data

        /// <summary>
        /// Decode the ADCP data received using a seperate
        /// thread.  This thread will also pass data to all
        /// _observers.
        /// </summary>
        private void ProcessDataThread()
        {
            while (_continue)
            {
                // Block until awoken when data is received
                _eventWaitData.WaitOne();

                // If wakeup was called to kill thread
                if (!_continue)
                {
                    return;
                }

                // Process all data in the buffer
                // If the buffer cannot be processed, the timeout will break out
                // of the loop
                int timeout = 0;
                while (_incomingDataBuffer.Count > DataSet.Ensemble.ENSEMBLE_HEADER_LEN && timeout <= TIMEOUT_MAX)
                {
                    // Decode the data sent to the codec
                    DecodeIncomingData();

                    timeout++;
                }
            }
        }

        /// <summary>
        /// Stop the thread.
        /// </summary>
        private void StopThread()
        {
            _continue = false;

            // Wake up the thread to stop thread
            _eventWaitData.Set();
        }

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
            // Find the beginning of an ensemble
            // It will contain 16 0x80 at the start
            SearchForHeaderStart();

            // Verify the incoming data can at least fit the header
            if (_incomingDataBuffer.Count > DataSet.Ensemble.ENSEMBLE_HEADER_LEN)
            {
                // If found 16 bytes of 0x80
                // Continue forward
                // Find ensemble number and ensemble size
                if (VerifyHeaderStart())
                {
                    // Verify the ensemble number and size
                    // and 1's compliment match
                    if (VerifyEnsembleNumberAndSize())
                    {
                        // Ensure the entire ensemble is present
                        // before preceeding
                        if (_incomingDataBuffer.Count >= _currentEnsembleSize)
                        {
                            // Create an array to hold the ensemble
                            byte[] ensemble = new byte[_currentEnsembleSize];

                            // Lock the buffer, to ensure the same data is copied
                            lock (_bufferLock)
                            {
                                // Check one more time just in case the buffer was modified
                                if (_incomingDataBuffer.Count >= _currentEnsembleSize)
                                {
                                    // Copy the ensemble to a byte array
                                    _incomingDataBuffer.CopyTo(0, ensemble, 0, _currentEnsembleSize);

                                    // Remove ensemble from buffer
                                    _incomingDataBuffer.RemoveRange(0, _currentEnsembleSize);
                                }
                            }

                            // Get the checksum values
                            long calculatedChecksum = DataSet.Ensemble.CalculateEnsembleChecksum(ensemble);
                            long ensembleChecksum = RetrieveEnsembleChecksum(ensemble);

                            // Verify the checksum match
                            if (calculatedChecksum == ensembleChecksum)
                            {
                                // Decode the binary data
                                DecodeAdcpData(ensemble);
                            }
                            else
                            {
                                //Debug.WriteLine(string.Format("Checksums do not match Cal: {0}  Actual:{1}", calculatedChecksum, ensembleChecksum));
                            }
                        }

                    }
                    // Checksum or Ensemble failed
                    else
                    {
                        // Lock the buffer while removing
                        lock (_bufferLock)
                        {
                            // Remove the first element to continue searching
                            _incomingDataBuffer.RemoveAt(0);
                        }
                    }
                }
                // Not a good header start
                else
                {
                    // Lock the buffer while removing
                    lock (_bufferLock)
                    {
                        // Remove the first element to continue searching
                        _incomingDataBuffer.RemoveAt(0);
                    }
                }
            }
        }

        /// <summary>
        /// Parse the incoming packet for all the Data Sets.
        /// Add the data to a AdcpDataSet variable and 
        /// return the filled variable when complete.
        /// </summary>
        /// <param name="ensemble">Byte array containing data from an ADCP.</param>
        /// <returns>Object holding decoded ADCP data.</returns>
        private Ensemble DecodeAdcpData(byte[] ensemble)
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

            Ensemble adcpData = new Ensemble();

            for (int i = 0; i < DataSet.Ensemble.MAX_NUM_DATA_SETS; i++)
            {
                type = MathHelper.ByteArrayToInt32(ensemble, packetPointer + (DataSet.Ensemble.BYTES_IN_INT32 * 0));
                numElements = MathHelper.ByteArrayToInt32(ensemble, packetPointer + (DataSet.Ensemble.BYTES_IN_INT32 * 1));
                elementMultiplier = MathHelper.ByteArrayToInt32(ensemble, packetPointer + (DataSet.Ensemble.BYTES_IN_INT32 * 2));
                imag = MathHelper.ByteArrayToInt32(ensemble, packetPointer + (DataSet.Ensemble.BYTES_IN_INT32 * 3));
                nameLen = MathHelper.ByteArrayToInt32(ensemble, packetPointer + (DataSet.Ensemble.BYTES_IN_INT32 * 4));
                name = MathHelper.ByteArrayToString(ensemble, 8, packetPointer + (DataSet.Ensemble.BYTES_IN_FLOAT * 5));

                // Get the size of this data set
                dataSetSize = BaseDataSet.GetDataSetSize(type, nameLen, numElements, elementMultiplier);

                if (Ensemble.BeamVelocityID.Equals(name, StringComparison.Ordinal))
                {
                    // Create a sub array of just this data set data
                    byte[] velData = MathHelper.SubArray<byte>(ensemble, packetPointer, dataSetSize);

                    // Add the data
                    adcpData.AddBeamVelocityData(type, numElements, elementMultiplier, imag, nameLen, name, velData);
                    //Debug.WriteLine(adcpData.BeamVelocityData.ToString());

                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }
                else if (Ensemble.InstrumentVelocityID.Equals(name, StringComparison.Ordinal))
                {
                    // Create a sub array of just this data set data
                    byte[] velData = MathHelper.SubArray<byte>(ensemble, packetPointer, dataSetSize);

                    // Add the data
                    adcpData.AddInstrumentVelocityData(type, numElements, elementMultiplier, imag, nameLen, name, velData);
                    //Debug.WriteLine(adcpData.InstrVelocityData.ToString());

                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }
                else if (Ensemble.EarthVelocityID.Equals(name, StringComparison.Ordinal))
                {
                    // Create a sub array of just this data set data
                    byte[] velData = MathHelper.SubArray<byte>(ensemble, packetPointer, dataSetSize);

                    // Add the data
                    adcpData.AddEarthVelocityData(type, numElements, elementMultiplier, imag, nameLen, name, velData);
                    //Debug.WriteLine(adcpData.EarthVelocityData.ToString());

                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }
                else if (Ensemble.AmplitudeID.Equals(name, StringComparison.Ordinal))
                {
                    // Create a sub array of just this data set data
                    byte[] ampData = MathHelper.SubArray<byte>(ensemble, packetPointer, dataSetSize);

                    // Add the data
                    adcpData.AddAmplitudeData(type, numElements, elementMultiplier, imag, nameLen, name, ampData);
                    //Debug.WriteLine(adcpData.AmplitudeData.ToString());

                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }
                else if (Ensemble.CorrelationID.Equals(name, StringComparison.Ordinal))
                {
                    // Create a sub array of just this data set data
                    byte[] corrData = MathHelper.SubArray<byte>(ensemble, packetPointer, dataSetSize);

                    // Add the data
                    adcpData.AddCorrelationData(type, numElements, elementMultiplier, imag, nameLen, name, corrData);
                    //Debug.WriteLine(adcpData.CorrelationData.ToString());

                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }
                else if (Ensemble.GoodBeamID.Equals(name, StringComparison.Ordinal))
                {
                    // Create a sub array of just this data set data
                    byte[] goodBeamData = MathHelper.SubArray<byte>(ensemble, packetPointer, dataSetSize);

                    // Add the data
                    adcpData.AddGoodBeamData(type, numElements, elementMultiplier, imag, nameLen, name, goodBeamData);
                    //Debug.WriteLine(adcpData.GoodBeamData.ToString());

                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }
                else if (Ensemble.GoodEarthID.Equals(name, StringComparison.Ordinal))
                {
                    // Create a sub array of just this data set data
                    byte[] goodEarthData = MathHelper.SubArray<byte>(ensemble, packetPointer, dataSetSize);

                    // Add the data
                    adcpData.AddGoodEarthData(type, numElements, elementMultiplier, imag, nameLen, name, goodEarthData);
                    //Debug.WriteLine(adcpData.GoodEarthData.ToString());

                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }
                else if (Ensemble.EnsembleDataID.Equals(name, StringComparison.Ordinal))
                {
                    // Create a sub array of just this data set data
                    byte[] ensembleData = MathHelper.SubArray<byte>(ensemble, packetPointer, dataSetSize);

                    // Add the data
                    adcpData.AddEnsembleData(type, numElements, elementMultiplier, imag, nameLen, name, ensembleData);
                    //Debug.WriteLine(adcpData.EnsembleData.ToString());

                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }
                else if (Ensemble.AncillaryID.Equals(name, StringComparison.Ordinal))
                {
                    // Create a sub array of just this data set data
                    byte[] ancillaryData = MathHelper.SubArray<byte>(ensemble, packetPointer, dataSetSize);

                    // Add the data
                    adcpData.AddAncillaryData(type, numElements, elementMultiplier, imag, nameLen, name, ancillaryData);
                    //Debug.WriteLine(adcpData.AncillaryData.ToString());

                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }
                else if (Ensemble.BottomTrackID.Equals(name, StringComparison.Ordinal))
                {
                    // Create a sub array of just this data set data
                    byte[] bottomTrackData = MathHelper.SubArray<byte>(ensemble, packetPointer, dataSetSize);

                    // Add the data
                    adcpData.AddBottomTrackData(type, numElements, elementMultiplier, imag, nameLen, name, bottomTrackData);
                    //Debug.WriteLine(adcpData.BottomTrackData.ToString());

                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }
                else if (Ensemble.NmeaID.Equals(name, StringComparison.Ordinal))
                {
                    // List of all data read
                    byte[] nmeaData = new byte[dataSetSize]; 
                    
                    // Scan through the data set and store all the data
                    for(int x = 0; x < dataSetSize; x++)
                    {
                        nmeaData[x] = ensemble[packetPointer++];
                    }

                    // Add the data
                    adcpData.AddNmeaData(type, numElements, elementMultiplier, imag, nameLen, name, nmeaData);
                    //Debug.WriteLine(adcpData.NmeaData.ToString());
                }
                else
                {
                    // Advance the packet pointer
                    packetPointer += dataSetSize;
                }

                if (packetPointer+4 >= ensemble.Length)
                    break;
            }

            // If NMEA data is available, add it to the dataset
            // Add it to both the binary data and ensemble object
            if (_nmeaBuffer.Count > 0)
            {
                // Create a NMEA dataset
                MergeNmeaDataSet(ref adcpData);

                // Add the NMEA binary data to binary data
                MergeNmeaBinary(ref ensemble, adcpData.NmeaData);
            }
            
            // Send an event that data was processed
            // in this format
            if (ProcessDataEvent != null)
            {
                ProcessDataEvent(ensemble, adcpData);
            }

            return adcpData;
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

            lock (_nmeaBufferLock)
            {
                for (int x = 0; x < _nmeaBuffer.Count; x++)
                {
                    nmeaData.Append(_nmeaBuffer.First.Value);

                    // Remove the data
                    _nmeaBuffer.RemoveFirst();
                }
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
        /// Search for the beginning of the header.
        /// The header will contain MAX_HEADER_COUNT of
        /// 0x80 at the beginning.  Try to find the 
        /// beginning before processing.  If it is not
        /// the beginning, remove the value from the
        /// list until the beginning is found.
        /// </summary>
        private void SearchForHeaderStart()
        {
            lock (_bufferLock)
            {
                // Find the beginning of an ensemble
                // It will contain 16 0x80 at the start
                while (_incomingDataBuffer.Count > 2)
                {
                    // Find a start
                    if (_incomingDataBuffer[0] == 0x80 && _incomingDataBuffer[1] == 0x80)
                    {
                        break;
                    }
                    // Remove the first byte until you find the start
                    else
                    {
                        // Lock the buffer while removing
                        _incomingDataBuffer.RemoveAt(0);
                    }
                }
            }
        }

        /// <summary>
        /// Verify there are MAX_HEADER_COUNT number of 0x80
        /// together at the start of the Header.  If there
        /// are not MAX_HEADER_COUNT amount, then this is
        /// not the start of the Header.
        /// </summary>
        /// <returns>True = MAX_HEADER_COUNT of 0x80</returns>
        private bool VerifyHeaderStart()
        {
            int countHdrStart = 0;

            lock (_bufferLock)
            {
                // Verify 16 bytes of 0x80
                for (int x = 0; x < DataSet.Ensemble.HEADER_START_COUNT; x++)
                {
                    if (_incomingDataBuffer[x] == 0x80)
                    {
                        countHdrStart++;
                    }
                }
            }

            if (countHdrStart == DataSet.Ensemble.HEADER_START_COUNT)
            {
                return true;
            }

            return false;
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
        private bool VerifyEnsembleNumberAndSize()
        {
            // Find ensemble number and ensemble size
            long EnsNum = 0;
            long NotEnsNum = 1;
            int payloadSize = 0;
            long NotEnsSiz = 1;
            int i = DataSet.Ensemble.HEADER_START_COUNT;

            lock (_bufferLock)
            {
                EnsNum = _incomingDataBuffer[i++];
                EnsNum += _incomingDataBuffer[i++] << 8;
                EnsNum += _incomingDataBuffer[i++] << 16;
                EnsNum += _incomingDataBuffer[i++] << 24;

                NotEnsNum = _incomingDataBuffer[i++];
                NotEnsNum += _incomingDataBuffer[i++] << 8;
                NotEnsNum += _incomingDataBuffer[i++] << 16;
                NotEnsNum += _incomingDataBuffer[i++] << 24;
                NotEnsNum = ~NotEnsNum;

                payloadSize = _incomingDataBuffer[i++];
                payloadSize += _incomingDataBuffer[i++] << 8;
                payloadSize += _incomingDataBuffer[i++] << 16;
                payloadSize += _incomingDataBuffer[i++] << 24;

                NotEnsSiz = _incomingDataBuffer[i++];
                NotEnsSiz += _incomingDataBuffer[i++] << 8;
                NotEnsSiz += _incomingDataBuffer[i++] << 16;
                NotEnsSiz += _incomingDataBuffer[i++] << 24;
                NotEnsSiz = ~NotEnsSiz;
            }

            // Determine if the inverted 1's compliment matches
            // the actual value
            if (EnsNum == NotEnsNum && payloadSize == NotEnsSiz)
            {
                // Store the current value
                // Include the checksum size to the ensemble size
                _currentEnsembleSize = DataSet.Ensemble.CalculateEnsembleSize(payloadSize);
                return true;
            }

            return false;
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
        private long RetrieveEnsembleChecksum(byte[] ensemble)
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

        #endregion
    }

}