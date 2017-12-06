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
 * 09/23/2014      RC          3.0.2      Initial coding
 * 10/27/2014      RC          3.0.2      Verified the _currentEnsembleSize was at least greater than the header size in DecodeIncomingData().
 * 10/28/2014      RC          3.0.2      Fixed bug to check if the buffer is empty.
 * 12/04/2014      RC          3.0.2      Added header lock.
 * 06/12/2015      RC          3.0.5      Added WaitOne(60000) to wakeup and check if we should close.
 * 06/23/2015      RC          3.0.5      Removed the thread.
 * 07/09/2015      RC          3.0.5      Mode the codec a thread.
 * 07/17/2015      RC          3.0.5      Set the number of beams and the ensemble number.
 * 07/20/2015      RC          3.0.5      Fixed setting the beam numbers.
 * 08/13/2015      RC          3.0.5      Added _headerStartLock in ClearIncomingData().
 * 08/13/2015      RC          3.0.5      Added complete event.
 * 04/27/2017      RC          3.4.2      Check for buffer overflow with _incomingDataTimeout.
 * 09/13/2017      RC          3.4.3      Fixed a bug where the thread lock holds in SearchForHeaderStart().
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
using System.Collections.Concurrent;

namespace RTI
{
    /// <summary>
    /// Decode TRDI Binary ADCP PD4 and PD5 files.
    /// This will parse the data into an 
    /// Ensemble object.
    /// </summary>
    public class PD4_5Codec : ICodec, IDisposable
    {

        #region Variables 

        /// <summary>
        /// Setup logger to report errors.
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// A minimum of 47 bytes is required for the PD4 data type.
        /// </summary>
        private const int MIN_ENS_SIZE = PD5_ENS_SIZE;

        /// <summary>
        /// Number of bytes PD4 has including the checksum.
        /// </summary>
        private const int PD4_ENS_SIZE = 47;

        /// <summary>
        /// Number of bytes PD5 has including the checksum.
        /// </summary>
        private const int PD5_ENS_SIZE = 88;


        /// <summary>
        /// Number of bytes for the header.
        /// 0x7D and either 0x00 or 0x01.
        /// </summary>
        private const int ID_SIZE = 2;

        /// <summary>
        /// Number of bytes for the header.
        /// This includes the ID 0x7D and either 0x00 or 0x01 and
        /// the number of bytes in the ensemble.
        /// </summary>
        private const int HEADER_SIZE = 4;

        /// <summary>
        /// Number of bytes in the checksum.
        /// </summary>
        private const int CHECKSUM_SIZE = 2;

        /// <summary>
        /// PD4 and PD5 ID.
        /// </summary>
        private const byte ID = 0x7D;

        /// <summary>
        /// Data structure ID for PD4.
        /// </summary>
        private const byte ID_PD4 = 0x00;

        /// <summary>
        /// Data structure ID for PD5.
        /// </summary>
        private const byte ID_PD5 = 0x01;

        /// <summary>
        /// Timeout of a loop.
        /// </summary>
        private const int TIMEOUT_MAX = 20;

        /// <summary>
        /// Buffer for the incoming data.
        /// This buffer holds all the individual bytes to be processed.
        /// </summary>
        private BlockingCollection<Byte> _incomingDataBuffer;

        /// <summary>
        /// List containing the beginning of the ensemble.
        /// This will contain the header start, ensemble number and payload.
        /// </summary>
        private List<byte> _headerStart;

        /// <summary>
        /// Lock for the header start list.
        /// </summary>
        private object _headerStartLock = new object();

        /// <summary>
        /// Current size of the ensemble being processed.
        /// This is calculated while decoding the current
        /// ensemble.
        /// </summary>
        private int _currentEnsembleSize;

        /// <summary>
        /// Previous time in seconds.
        /// </summary>
        private float _prevTime;

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
        /// Ensemble index.
        /// </summary>
        private int _ensembleIndex;

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

        #endregion

        #region Struct

        /// <summary>
        /// System configuration.
        /// </summary>
        private struct SystemConfig
        {
            /// <summary>
            /// Velocity transform.
            /// </summary>
            public Core.Commons.Transforms VelTransform;

            /// <summary>
            /// Is Tilt information used in calculation.
            /// </summary>
            public bool IsTiltUsed;

            /// <summary>
            /// 3 Beam solutions not computed.
            /// </summary>
            public bool Is3BeamSolution;

            /// <summary>
            /// System frequency.
            /// </summary>
            public Subsystem Frequency;
        }

        #endregion

        /// <summary>
        /// Constructor
        /// Initialize values.
        /// </summary>
        public PD4_5Codec()
        {
            //_nmeaBuffer = new LinkedList<string>();

            // Initialize buffer
            _incomingDataBuffer = new BlockingCollection<Byte>();
            _headerStart = new List<byte>();

            // Initialize the ensemble size to at least the HDRLEN
            _currentEnsembleSize = MIN_ENS_SIZE;

            _prevTime = 0.0f;
            _ensembleIndex = 0;

            // Initialize the thread
            _continue = true;
            _eventWaitData = new EventWaitHandle(false, EventResetMode.AutoReset);
            _processDataThread = new Thread(ProcessDataThread);
            _processDataThread.Name = "PD4 PD5 Codec";
            _processDataThread.Start();
        }

        /// <summary>
        /// Shutdown this object.
        /// </summary>
        public void Dispose()
        {
            StopThread();

            //ClearIncomingData();
            _incomingDataBuffer.Dispose();
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
                // Add all the data to the buffer
                for (int x = 0; x < data.Length; x++)
                {
                    _incomingDataBuffer.Add(data[x]);
                }
            }

            // Wake up the thread to process data
            _eventWaitData.Set();

            // Check timeout
            _incomingDataTimeout++;
            if (_incomingDataTimeout > INCOMING_DATA_TIMEOUT)
            {
                // Reset the value and clear the data
                _incomingDataTimeout = 0;
                ClearIncomingData();
            }
        }

        /// <summary>
        /// Clear the incoming buffer of all data.
        /// </summary>
        public void ClearIncomingData()
        {
            // Clear the buffer
            _incomingDataBuffer = new BlockingCollection<Byte>();
            

            //lock (_headerStartLock)
            //{
                //Debug.WriteLine("PD4_5Codec: ClearIncomingData() Lock Open");
                _headerStart.Clear();
            //}

            //Debug.WriteLine("PD4_5Codec: ClearIncomingData() Lock Closed");

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
                try
                {
                    // Block until awoken when data is received
                    // Timeout every 60 seconds to see if shutdown occured
                    _eventWaitData.WaitOne(1000);

                    // If wakeup was called to kill thread
                    if (!_continue)
                    {
                        return;
                    }

                    // Process all data in the buffer
                    // If the buffer cannot be processed
                    while (_incomingDataBuffer.Count > MIN_ENS_SIZE)
                    {
                        // Decode the data sent to the codec
                        DecodeIncomingData();

                        // If wakeup was called to kill thread
                        if (!_continue)
                        {
                            Thread.Sleep(1000);
                            return;
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    // Thread is aborted to stop processing
                    return;
                }
                catch (Exception e)
                {
                    log.Error("Error processing PD4/PD5 codec data.", e);
                    return;
                }

                // Send an event that processing is complete
                if (ProcessDataCompleteEvent != null)
                {
                    ProcessDataCompleteEvent();
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

            // Force the thread to stop
            //_processDataThread.Abort();
            _processDataThread.Join(1000);
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
            // It will contain 0x7D and either 0x00 or 0x01
            SearchForHeaderStart();

            // Verify the incoming data can at least fit an ensemble
            // Subtract the header size because it is already stored in the _headerStart
            if (_incomingDataBuffer.Count >= MIN_ENS_SIZE - HEADER_SIZE)
            {
                // Get the size
                if (GetEnsembleSize())
                {
                    // Ensure the entire ensemble is present
                    // before preceeding
                    if (_currentEnsembleSize >= HEADER_SIZE && _incomingDataBuffer.Count + _headerStart.Count >= _currentEnsembleSize)
                    {
                        // Create an array to hold the ensemble
                        byte[] ensemble = new byte[_currentEnsembleSize];

                        lock (_headerStartLock)
                        {
                            //Debug.WriteLine("PD4_5Codec: DecodeIncomingData() Lock Open");
                            // Copy the header start to the ensemble
                            Buffer.BlockCopy(_headerStart.ToArray(), 0, ensemble, 0, HEADER_SIZE);
                            _headerStart.Clear();
                        }
                        //Debug.WriteLine("PD4_5Codec: DecodeIncomingData() Lock Closed");

                        // Copy the remainder of the ensemble 
                        for (int x = HEADER_SIZE; x < _currentEnsembleSize; x++)
                        {
                            ensemble[x] = _incomingDataBuffer.Take();
                        }

                        // Get the checksum values
                        long calculatedChecksum = PD0.CalculateChecksum(ensemble, _currentEnsembleSize - 2);
                        long ensembleChecksum = RetrieveEnsembleChecksum(ensemble);

                        // Verify the checksum match
                        if (calculatedChecksum == ensembleChecksum)
                        {
                            // Decode the binary data
                            DecodeAdcpData(ensemble);
                        }
                        //else
                        //{
                        //    Debug.WriteLine(string.Format("Checksums do not match Cal: {0}  Actual:{1}", calculatedChecksum, ensembleChecksum));
                        //}
                    }

                }
                // Checksum or Ensemble failed
                else
                {
                    lock (_headerStartLock)
                    {
                        //Debug.WriteLine("PD4_5Codec: DecodingIncomingData() Else Lock Open");
                        // Remove the first element to continue searching
                        if (_headerStart.Count > 0)
                        {
                            _headerStart.RemoveAt(0);
                        }
                    }
                    //Debug.WriteLine("PD4_5Codec: DecodeIncomingData() Else Lock Closed");
                }
            }
        }

        /// <summary>
        /// Parse the incoming packet for all the Data Sets.
        /// Add the data to a AdcpDataSet variable and 
        /// return the filled variable when complete.
        /// </summary>
        /// <param name="binaryEnsemble">Byte array containing data from an ADCP.</param>
        /// <returns>Object holding decoded ADCP data.</returns>
        private Ensemble DecodeAdcpData(byte[] binaryEnsemble)
        {
            Ensemble ensemble = new Ensemble();

            #region DataSets

            if(!ensemble.IsEnsembleAvail)
            {
                ensemble.EnsembleData = new EnsembleDataSet();
                ensemble.IsEnsembleAvail = true;
            }

            if (!ensemble.IsAncillaryAvail)
            {
                ensemble.AncillaryData = new AncillaryDataSet();
                ensemble.IsAncillaryAvail = true;
            }

            // Add Bottom Track data
            if (!ensemble.IsBottomTrackAvail)
            {
                ensemble.BottomTrackData = new BottomTrackDataSet();
                ensemble.IsBottomTrackAvail = true;
            }

            // Add DVL dataset
            if (!ensemble.IsDvlDataAvail)
            {
                ensemble.DvlData = new DvlDataSet();
                ensemble.IsDvlDataAvail = true;
            }

            #endregion

            #region System Config

            SystemConfig config = GetSystemConfig(binaryEnsemble[4]);

            // Set the serial number
            ensemble.EnsembleData.SysSerialNumber = SerialNumber.DVL;
            ensemble.EnsembleData.SysSerialNumber.RemoveSubsystem(SerialNumber.DVL_Subsystem);
            ensemble.EnsembleData.SysSerialNumber.AddSubsystem(config.Frequency);

            // ensemble number
            ensemble.EnsembleData.EnsembleNumber = _ensembleIndex++;

            #endregion

            #region Speed of Sound

            // Speed of Sound
            ensemble.AncillaryData.SpeedOfSound = MathHelper.LsbMsbUShort(binaryEnsemble[41], binaryEnsemble[42]);
            ensemble.BottomTrackData.SpeedOfSound = MathHelper.LsbMsbUShort(binaryEnsemble[41], binaryEnsemble[42]);
            ensemble.DvlData.SpeedOfSound = MathHelper.LsbMsbUShort(binaryEnsemble[41], binaryEnsemble[42]);

            // Temperature
            ensemble.AncillaryData.WaterTemp = MathHelper.LsbMsbUShort(binaryEnsemble[43], binaryEnsemble[44]) * 0.01f;
            ensemble.BottomTrackData.WaterTemp = MathHelper.LsbMsbUShort(binaryEnsemble[43], binaryEnsemble[44]) * 0.01f;
            ensemble.DvlData.WaterTemp = MathHelper.LsbMsbUShort(binaryEnsemble[43], binaryEnsemble[44]) * 0.01f;

            #endregion

            #region Ensemble Time

            // Ensemble time
            int hour = binaryEnsemble[35];
            int min = binaryEnsemble[36];
            int sec = binaryEnsemble[37];
            int hun = binaryEnsemble[38];
            ensemble.EnsembleData.Hour = hour;
            ensemble.EnsembleData.Minute = min;
            ensemble.EnsembleData.Second = sec;
            ensemble.EnsembleData.HSec = hun;

            ensemble.DvlData.Hour = hour;
            ensemble.DvlData.Minute = min;
            ensemble.DvlData.Second = sec;
            ensemble.DvlData.HSec = hun;

            // Time of first ping
            float currTime = (hour * 3600.0f) + (min * 60.0f) + sec + (hun * 0.01f);
            ensemble.AncillaryData.FirstPingTime = currTime - _prevTime;
            ensemble.AncillaryData.LastPingTime = currTime - _prevTime;
            ensemble.BottomTrackData.FirstPingTime = currTime - _prevTime;
            ensemble.BottomTrackData.LastPingTime = currTime - _prevTime;

            // Set the prev time if it has not been set
            if (_prevTime == 0)
            {
                _prevTime = currTime;
            }

            #endregion

            #region Bottom Track Velcocity

            // Reset the number of beams values
            // We will count the number of beams below
            ensemble.EnsembleData.NumBeams = 0;       // Number of beams
            ensemble.BottomTrackData.NumBeams = 0;    // Number of beams

            // X
            if (MathHelper.LsbMsbShort(binaryEnsemble[5], binaryEnsemble[6]) == PD0.BAD_VELOCITY)
            {
                ensemble.BottomTrackData.BeamVelocity[Ensemble.BEAM_0_INDEX] = Ensemble.BAD_VELOCITY;
                ensemble.BottomTrackData.InstrumentVelocity[Ensemble.BEAM_X_INDEX] = Ensemble.BAD_VELOCITY;
                ensemble.BottomTrackData.EarthVelocity[Ensemble.BEAM_EAST_INDEX] = Ensemble.BAD_VELOCITY;
                ensemble.BottomTrackData.BeamGood[Ensemble.BEAM_0_INDEX] = 0.0f;
                ensemble.BottomTrackData.EarthGood[Ensemble.BEAM_EAST_INDEX] = 0.0f;
                ensemble.BottomTrackData.InstrumentGood[Ensemble.BEAM_X_INDEX] = 0.0f;
                
                ensemble.DvlData.BtEastVelocity = Ensemble.BAD_VELOCITY;
                ensemble.DvlData.BtXVelocity = Ensemble.BAD_VELOCITY;
                ensemble.DvlData.BtTransverseVelocity = Ensemble.BAD_VELOCITY;
                ensemble.DvlData.BtEarthIsGoodVelocity = false;
                ensemble.DvlData.BtInstrumentIsGoodVelocity = false;
                ensemble.DvlData.BtShipIsGoodVelocity = false;
            }
            else
            {
                // Beam velocity
                if (config.VelTransform == Core.Commons.Transforms.BEAM)
                {
                    ensemble.BottomTrackData.BeamVelocity[Ensemble.BEAM_X_INDEX] = MathHelper.LsbMsbShort(binaryEnsemble[5], binaryEnsemble[6]) * 0.001f;        // mm/s to m/s
                    ensemble.BottomTrackData.BeamGood[Ensemble.BEAM_X_INDEX] = 1.0f;
                }

                // Earth velocity
                if (config.VelTransform == Core.Commons.Transforms.EARTH)
                {
                    ensemble.BottomTrackData.EarthVelocity[Ensemble.BEAM_EAST_INDEX] = MathHelper.LsbMsbShort(binaryEnsemble[5], binaryEnsemble[6]) * 0.001f;        // mm/s to m/s
                    ensemble.BottomTrackData.EarthGood[Ensemble.BEAM_EAST_INDEX] = 1.0f;

                    ensemble.DvlData.BtEastVelocity = MathHelper.LsbMsbShort(binaryEnsemble[5], binaryEnsemble[6]) * 0.001f;        // mm/s to m/s
                    ensemble.DvlData.BtEarthIsGoodVelocity = true;

                    ensemble.EnsembleData.NumBeams++;       // Number of beams
                    ensemble.BottomTrackData.NumBeams++;    // Number of beams
                }

                // Instrument velocity
                if (config.VelTransform == Core.Commons.Transforms.INSTRUMENT)
                {
                    ensemble.BottomTrackData.InstrumentVelocity[Ensemble.BEAM_X_INDEX] = MathHelper.LsbMsbShort(binaryEnsemble[5], binaryEnsemble[6]) * 0.001f;        // mm/s to m/s
                    ensemble.BottomTrackData.InstrumentGood[Ensemble.BEAM_X_INDEX] = 1.0f;

                    ensemble.DvlData.BtXVelocity = MathHelper.LsbMsbShort(binaryEnsemble[5], binaryEnsemble[6]) * 0.001f;        // mm/s to m/s
                    ensemble.DvlData.BtInstrumentIsGoodVelocity = true;

                    ensemble.EnsembleData.NumBeams++;       // Number of beams
                    ensemble.BottomTrackData.NumBeams++;    // Number of beams
                }

                // Ship velocity
                if (config.VelTransform == Core.Commons.Transforms.SHIP)
                {
                    ensemble.DvlData.BtTransverseVelocity = MathHelper.LsbMsbShort(binaryEnsemble[5], binaryEnsemble[6]) * 0.001f;        // mm/s to m/s
                    ensemble.DvlData.BtShipIsGoodVelocity = true;

                    ensemble.EnsembleData.NumBeams++;       // Number of beams
                    ensemble.BottomTrackData.NumBeams++;    // Number of beams
                }
            }

            // Y
            if (MathHelper.LsbMsbShort(binaryEnsemble[7], binaryEnsemble[8]) == PD0.BAD_VELOCITY)
            {
                ensemble.BottomTrackData.BeamVelocity[Ensemble.BEAM_1_INDEX] = Ensemble.BAD_VELOCITY;
                ensemble.BottomTrackData.InstrumentVelocity[Ensemble.BEAM_Y_INDEX] = Ensemble.BAD_VELOCITY;
                ensemble.BottomTrackData.EarthVelocity[Ensemble.BEAM_NORTH_INDEX] = Ensemble.BAD_VELOCITY;
                ensemble.BottomTrackData.BeamGood[Ensemble.BEAM_1_INDEX] = 0.0f;
                ensemble.BottomTrackData.EarthGood[Ensemble.BEAM_NORTH_INDEX] = 0.0f;
                ensemble.BottomTrackData.InstrumentGood[Ensemble.BEAM_Y_INDEX] = 0.0f;

                ensemble.DvlData.BtNorthVelocity = Ensemble.BAD_VELOCITY;
                ensemble.DvlData.BtYVelocity = Ensemble.BAD_VELOCITY;
                ensemble.DvlData.BtLongitudinalVelocity = Ensemble.BAD_VELOCITY;
            }
            else
            {
                // Beam velocity
                if (config.VelTransform == Core.Commons.Transforms.BEAM)
                {
                    ensemble.BottomTrackData.BeamVelocity[Ensemble.BEAM_1_INDEX] = MathHelper.LsbMsbShort(binaryEnsemble[7], binaryEnsemble[8]) * 0.001f;        // mm/s to m/s
                    ensemble.BottomTrackData.BeamGood[Ensemble.BEAM_1_INDEX] = 1.0f;

                    ensemble.EnsembleData.NumBeams++;       // Number of beams
                    ensemble.BottomTrackData.NumBeams++;    // Number of beams
                }

                // Earth velocity
                if (config.VelTransform == Core.Commons.Transforms.EARTH)
                {
                    ensemble.BottomTrackData.EarthVelocity[Ensemble.BEAM_NORTH_INDEX] = MathHelper.LsbMsbShort(binaryEnsemble[7], binaryEnsemble[8]) * 0.001f;        // mm/s to m/s
                    ensemble.BottomTrackData.EarthGood[Ensemble.BEAM_NORTH_INDEX] = 1.0f;

                    ensemble.DvlData.BtNorthVelocity = MathHelper.LsbMsbShort(binaryEnsemble[7], binaryEnsemble[8]) * 0.001f;        // mm/s to m/s

                    ensemble.EnsembleData.NumBeams++;       // Number of beams
                    ensemble.BottomTrackData.NumBeams++;    // Number of beams
                }

                // Instrument velocity
                if (config.VelTransform == Core.Commons.Transforms.INSTRUMENT)
                {
                    ensemble.BottomTrackData.InstrumentVelocity[Ensemble.BEAM_Y_INDEX] = MathHelper.LsbMsbShort(binaryEnsemble[7], binaryEnsemble[8]) * 0.001f;        // mm/s to m/s
                    ensemble.BottomTrackData.InstrumentGood[Ensemble.BEAM_Y_INDEX] = 1.0f;

                    ensemble.DvlData.BtYVelocity = MathHelper.LsbMsbShort(binaryEnsemble[7], binaryEnsemble[8]) * 0.001f;        // mm/s to m/s

                    ensemble.EnsembleData.NumBeams++;       // Number of beams
                    ensemble.BottomTrackData.NumBeams++;    // Number of beams
                }

                // Ship velocity
                if (config.VelTransform == Core.Commons.Transforms.SHIP)
                {
                    ensemble.DvlData.BtLongitudinalVelocity = MathHelper.LsbMsbShort(binaryEnsemble[7], binaryEnsemble[8]) * 0.001f;        // mm/s to m/s

                    ensemble.EnsembleData.NumBeams++;       // Number of beams
                    ensemble.BottomTrackData.NumBeams++;    // Number of beams
                }
            }

            // Z
            if (MathHelper.LsbMsbShort(binaryEnsemble[9], binaryEnsemble[10]) == PD0.BAD_VELOCITY)
            {
                ensemble.BottomTrackData.BeamVelocity[Ensemble.BEAM_2_INDEX] = Ensemble.BAD_VELOCITY;
                ensemble.BottomTrackData.InstrumentVelocity[Ensemble.BEAM_Z_INDEX] = Ensemble.BAD_VELOCITY;
                ensemble.BottomTrackData.EarthVelocity[Ensemble.BEAM_VERTICAL_INDEX] = Ensemble.BAD_VELOCITY;
                ensemble.BottomTrackData.BeamGood[Ensemble.BEAM_2_INDEX] = 0.0f;
                ensemble.BottomTrackData.EarthGood[Ensemble.BEAM_VERTICAL_INDEX] = 0.0f;
                ensemble.BottomTrackData.InstrumentGood[Ensemble.BEAM_Z_INDEX] = 0.0f;

                ensemble.DvlData.BtUpwardVelocity = Ensemble.BAD_VELOCITY;
                ensemble.DvlData.BtZVelocity = Ensemble.BAD_VELOCITY;
                ensemble.DvlData.BtNormalVelocity = Ensemble.BAD_VELOCITY;
            }
            else
            {
                // Beam velocity
                if (config.VelTransform == Core.Commons.Transforms.BEAM)
                {
                    ensemble.BottomTrackData.BeamVelocity[Ensemble.BEAM_2_INDEX] = MathHelper.LsbMsbShort(binaryEnsemble[9], binaryEnsemble[10]) * 0.001f;        // mm/s to m/s
                    ensemble.BottomTrackData.BeamGood[Ensemble.BEAM_2_INDEX] = 1.0f;

                    ensemble.EnsembleData.NumBeams++;       // Number of beams
                    ensemble.BottomTrackData.NumBeams++;    // Number of beams
                }

                // Earth velocity
                if (config.VelTransform == Core.Commons.Transforms.EARTH)
                {
                    ensemble.BottomTrackData.EarthVelocity[Ensemble.BEAM_VERTICAL_INDEX] = MathHelper.LsbMsbShort(binaryEnsemble[9], binaryEnsemble[10]) * 0.001f;        // mm/s to m/s
                    ensemble.BottomTrackData.EarthGood[Ensemble.BEAM_VERTICAL_INDEX] = 1.0f;

                    ensemble.DvlData.BtUpwardVelocity = MathHelper.LsbMsbShort(binaryEnsemble[9], binaryEnsemble[10]) * 0.001f;        // mm/s to m/s

                    ensemble.EnsembleData.NumBeams++;       // Number of beams
                    ensemble.BottomTrackData.NumBeams++;    // Number of beams
                }

                // Instrument velocity
                if (config.VelTransform == Core.Commons.Transforms.INSTRUMENT)
                {
                    ensemble.BottomTrackData.InstrumentVelocity[Ensemble.BEAM_Z_INDEX] = MathHelper.LsbMsbShort(binaryEnsemble[9], binaryEnsemble[10]) * 0.001f;        // mm/s to m/s
                    ensemble.BottomTrackData.InstrumentGood[Ensemble.BEAM_Z_INDEX] = 1.0f;

                    ensemble.DvlData.BtZVelocity = MathHelper.LsbMsbShort(binaryEnsemble[9], binaryEnsemble[10]) * 0.001f;        // mm/s to m/s

                    ensemble.EnsembleData.NumBeams++;       // Number of beams
                    ensemble.BottomTrackData.NumBeams++;    // Number of beams
                }

                // Ship velocity
                if (config.VelTransform == Core.Commons.Transforms.SHIP)
                {
                    ensemble.DvlData.BtNormalVelocity = MathHelper.LsbMsbShort(binaryEnsemble[9], binaryEnsemble[10]) * 0.001f;        // mm/s to m/s

                    ensemble.EnsembleData.NumBeams++;       // Number of beams
                    ensemble.BottomTrackData.NumBeams++;    // Number of beams
                }
            }

            // Q
            if (MathHelper.LsbMsbShort(binaryEnsemble[11], binaryEnsemble[12]) == PD0.BAD_VELOCITY)
            {
                ensemble.BottomTrackData.BeamVelocity[Ensemble.BEAM_3_INDEX] = Ensemble.BAD_VELOCITY;
                ensemble.BottomTrackData.InstrumentVelocity[Ensemble.BEAM_Q_INDEX] = Ensemble.BAD_VELOCITY;
                ensemble.BottomTrackData.EarthVelocity[Ensemble.BEAM_Q_INDEX] = Ensemble.BAD_VELOCITY;
                ensemble.BottomTrackData.BeamGood[Ensemble.BEAM_Q_INDEX] = 0.0f;
                ensemble.BottomTrackData.EarthGood[Ensemble.BEAM_Q_INDEX] = 0.0f;
                ensemble.BottomTrackData.InstrumentGood[Ensemble.BEAM_Q_INDEX] = 0.0f;

                ensemble.DvlData.BtErrorVelocity = Ensemble.BAD_VELOCITY;
                ensemble.DvlData.BtShipErrorVelocity = Ensemble.BAD_VELOCITY;
            }
            else
            {
                // Beam velocity
                if (config.VelTransform == Core.Commons.Transforms.BEAM)
                {
                    ensemble.BottomTrackData.BeamVelocity[Ensemble.BEAM_3_INDEX] = MathHelper.LsbMsbShort(binaryEnsemble[11], binaryEnsemble[12]) * 0.001f;        // mm/s to m/s
                    ensemble.BottomTrackData.BeamGood[Ensemble.BEAM_3_INDEX] = 1.0f;

                    ensemble.EnsembleData.NumBeams++;       // Number of beams
                    ensemble.BottomTrackData.NumBeams++;    // Number of beams
                }

                // Earth velocity
                if (config.VelTransform == Core.Commons.Transforms.EARTH)
                {
                    ensemble.BottomTrackData.EarthVelocity[Ensemble.BEAM_Q_INDEX] = MathHelper.LsbMsbShort(binaryEnsemble[11], binaryEnsemble[12]) * 0.001f;        // mm/s to m/s
                    ensemble.BottomTrackData.EarthGood[Ensemble.BEAM_Q_INDEX] = 1.0f;

                    ensemble.DvlData.BtErrorVelocity = MathHelper.LsbMsbShort(binaryEnsemble[11], binaryEnsemble[12]) * 0.001f;        // mm/s to m/s

                    ensemble.EnsembleData.NumBeams++;       // Number of beams
                    ensemble.BottomTrackData.NumBeams++;    // Number of beams
                }

                // Instrument velocity
                if (config.VelTransform == Core.Commons.Transforms.INSTRUMENT)
                {
                    ensemble.BottomTrackData.InstrumentVelocity[Ensemble.BEAM_Q_INDEX] = MathHelper.LsbMsbShort(binaryEnsemble[11], binaryEnsemble[12]) * 0.001f;        // mm/s to m/s
                    ensemble.BottomTrackData.InstrumentGood[Ensemble.BEAM_Q_INDEX] = 1.0f;

                    ensemble.DvlData.BtErrorVelocity = MathHelper.LsbMsbShort(binaryEnsemble[11], binaryEnsemble[12]) * 0.001f;        // mm/s to m/s

                    ensemble.EnsembleData.NumBeams++;       // Number of beams
                    ensemble.BottomTrackData.NumBeams++;    // Number of beams
                }

                // Ship velocity
                if (config.VelTransform == Core.Commons.Transforms.SHIP)
                {
                    //ensemble.BottomTrackData.InstrumentVelocity[Ensemble.BEAM_Q_INDEX] = MathHelper.LsbMsbShort(binaryEnsemble[11], binaryEnsemble[12]) * 0.001f;        // mm/s to m/s
                    //ensemble.BottomTrackData.InstrumentGood[Ensemble.BEAM_Q_INDEX] = 1.0f;

                    ensemble.DvlData.BtShipErrorVelocity = MathHelper.LsbMsbShort(binaryEnsemble[11], binaryEnsemble[12]) * 0.001f;        // mm/s to m/s

                    ensemble.EnsembleData.NumBeams++;       // Number of beams
                    ensemble.BottomTrackData.NumBeams++;    // Number of beams
                }
            }

            // Range
            ensemble.BottomTrackData.Range[Ensemble.BEAM_0_INDEX] = MathHelper.LsbMsbUShort(binaryEnsemble[13], binaryEnsemble[14]) * 0.01f;        // cm to m
            ensemble.BottomTrackData.Range[Ensemble.BEAM_1_INDEX] = MathHelper.LsbMsbUShort(binaryEnsemble[15], binaryEnsemble[16]) * 0.01f;        // cm to m
            ensemble.BottomTrackData.Range[Ensemble.BEAM_2_INDEX] = MathHelper.LsbMsbUShort(binaryEnsemble[17], binaryEnsemble[18]) * 0.01f;        // cm to m
            ensemble.BottomTrackData.Range[Ensemble.BEAM_3_INDEX] = MathHelper.LsbMsbUShort(binaryEnsemble[19], binaryEnsemble[20]) * 0.01f;        // cm to m

            ensemble.DvlData.RangeBeam0 = MathHelper.LsbMsbUShort(binaryEnsemble[13], binaryEnsemble[14]) * 0.01f;        // cm to m
            ensemble.DvlData.RangeBeam1 = MathHelper.LsbMsbUShort(binaryEnsemble[15], binaryEnsemble[16]) * 0.01f;        // cm to m
            ensemble.DvlData.RangeBeam2 = MathHelper.LsbMsbUShort(binaryEnsemble[17], binaryEnsemble[18]) * 0.01f;        // cm to m
            ensemble.DvlData.RangeBeam3 = MathHelper.LsbMsbUShort(binaryEnsemble[19], binaryEnsemble[20]) * 0.01f;        // cm to m

            #endregion

            #region Reference Layer Data

            #region Earth

            // Add the proper DataSet
            if(config.VelTransform == Core.Commons.Transforms.EARTH)
            {
                // Add Earth Water Mass
                if (!ensemble.IsEarthWaterMassAvail)
                {
                    ensemble.EarthWaterMassData = new EarthWaterMassDataSet();
                    ensemble.IsEarthWaterMassAvail = true;
                }

                // East
                if (MathHelper.LsbMsbShort(binaryEnsemble[22], binaryEnsemble[23]) == PD0.BAD_VELOCITY)
                {
                    ensemble.EarthWaterMassData.VelocityEast = Ensemble.BAD_VELOCITY;
                    ensemble.DvlData.WmEastVelocity = Ensemble.BAD_VELOCITY;
                }
                else
                {
                    ensemble.EarthWaterMassData.VelocityEast = MathHelper.LsbMsbShort(binaryEnsemble[22], binaryEnsemble[23]) * 0.001f;         // mm/s to m/s
                    ensemble.DvlData.WmEastVelocity = MathHelper.LsbMsbShort(binaryEnsemble[22], binaryEnsemble[23]) * 0.001f;                  // mm/s to m/s
                    //ensemble.EnsembleData.NumBeams++;   // Number of beams
                }

                // North
                if (MathHelper.LsbMsbShort(binaryEnsemble[24], binaryEnsemble[25]) == PD0.BAD_VELOCITY)
                {
                    ensemble.EarthWaterMassData.VelocityNorth = Ensemble.BAD_VELOCITY;
                    ensemble.DvlData.WmNorthVelocity = Ensemble.BAD_VELOCITY;
                }
                else
                {
                    ensemble.EarthWaterMassData.VelocityNorth = MathHelper.LsbMsbShort(binaryEnsemble[24], binaryEnsemble[25]) * 0.001f;        // mm/s to m/s
                    ensemble.DvlData.WmNorthVelocity = MathHelper.LsbMsbShort(binaryEnsemble[24], binaryEnsemble[25]) * 0.001f;                 // mm/s to m/s
                    //ensemble.EnsembleData.NumBeams++;   // Number of beams
                }

                // Vertical
                if (MathHelper.LsbMsbShort(binaryEnsemble[26], binaryEnsemble[27]) == PD0.BAD_VELOCITY)
                {
                    ensemble.EarthWaterMassData.VelocityVertical = Ensemble.BAD_VELOCITY;
                    ensemble.DvlData.WmUpwardVelocity = Ensemble.BAD_VELOCITY;
                }
                else
                {
                    ensemble.EarthWaterMassData.VelocityVertical = MathHelper.LsbMsbShort(binaryEnsemble[26], binaryEnsemble[27]) * 0.001f;     // mm/s to m/s
                    ensemble.DvlData.WmUpwardVelocity = MathHelper.LsbMsbShort(binaryEnsemble[26], binaryEnsemble[27]) * 0.001f;                // mm/s to m/s
                    //ensemble.EnsembleData.NumBeams++;   // Number of beams
                }

                // Depth layer
                ensemble.EarthWaterMassData.WaterMassDepthLayer = MathHelper.LsbMsbShort(binaryEnsemble[30], binaryEnsemble[31]) * 0.1f;        // dm to m
                ensemble.DvlData.RefLayerMin = (int)(MathHelper.LsbMsbUShort(binaryEnsemble[30], binaryEnsemble[31]) * 0.1f);                   // dm to m
                ensemble.DvlData.RefLayerMax = (int)(MathHelper.LsbMsbUShort(binaryEnsemble[32], binaryEnsemble[33]) * 0.1f);                   // dm to m

            }

            #endregion

            #region Instrument

            if (config.VelTransform == Core.Commons.Transforms.INSTRUMENT)
            {
                // Add Instrument Water Mass
                if (!ensemble.IsInstrumentWaterMassAvail)
                {
                    ensemble.InstrumentWaterMassData = new InstrumentWaterMassDataSet();
                    ensemble.IsInstrumentWaterMassAvail = true;
                }

                // X
                if (MathHelper.LsbMsbShort(binaryEnsemble[22], binaryEnsemble[23]) == PD0.BAD_VELOCITY)
                {
                    ensemble.InstrumentWaterMassData.VelocityX = Ensemble.BAD_VELOCITY;
                    ensemble.DvlData.WmXVelocity = Ensemble.BAD_VELOCITY;
                }
                else
                {
                    ensemble.InstrumentWaterMassData.VelocityX = MathHelper.LsbMsbShort(binaryEnsemble[22], binaryEnsemble[23]) * 0.001f;               // mm/s to m/s
                    ensemble.DvlData.WmXVelocity = MathHelper.LsbMsbShort(binaryEnsemble[22], binaryEnsemble[23]) * 0.001f;                             // mm/s to m/s
                    //ensemble.EnsembleData.NumBeams++;   // Number of beams
                }

                // Y
                if (MathHelper.LsbMsbShort(binaryEnsemble[24], binaryEnsemble[25]) == PD0.BAD_VELOCITY)
                {
                    ensemble.InstrumentWaterMassData.VelocityY = Ensemble.BAD_VELOCITY;
                    ensemble.DvlData.WmYVelocity = Ensemble.BAD_VELOCITY;
                }
                else
                {
                    ensemble.InstrumentWaterMassData.VelocityY = MathHelper.LsbMsbShort(binaryEnsemble[24], binaryEnsemble[25]) * 0.001f;               // mm/s to m/s
                    ensemble.DvlData.WmYVelocity = MathHelper.LsbMsbShort(binaryEnsemble[24], binaryEnsemble[25]) * 0.001f;                             // mm/s to m/s
                    //ensemble.EnsembleData.NumBeams++;   // Number of beams
                }

                // Z
                if (MathHelper.LsbMsbShort(binaryEnsemble[26], binaryEnsemble[27]) == PD0.BAD_VELOCITY)
                {
                    ensemble.InstrumentWaterMassData.VelocityZ = Ensemble.BAD_VELOCITY;
                    ensemble.DvlData.WmZVelocity = Ensemble.BAD_VELOCITY;
                }
                else
                {
                    ensemble.InstrumentWaterMassData.VelocityZ = MathHelper.LsbMsbShort(binaryEnsemble[26], binaryEnsemble[27]) * 0.001f;               // mm/s to m/s
                    ensemble.DvlData.WmZVelocity = MathHelper.LsbMsbShort(binaryEnsemble[26], binaryEnsemble[27]) * 0.001f;                             // mm/s to m/s
                    //ensemble.EnsembleData.NumBeams++;   // Number of beams
                }

                // Error
                if (MathHelper.LsbMsbShort(binaryEnsemble[28], binaryEnsemble[29]) == PD0.BAD_VELOCITY)
                {
                    ensemble.InstrumentWaterMassData.VelocityQ = Ensemble.BAD_VELOCITY;
                }
                else
                {
                    ensemble.InstrumentWaterMassData.VelocityQ = MathHelper.LsbMsbShort(binaryEnsemble[28], binaryEnsemble[29]) * 0.001f;               // mm/s to m/s
                    //ensemble.EnsembleData.NumBeams++;   // Number of beams
                }

                // Depth layer
                ensemble.InstrumentWaterMassData.WaterMassDepthLayer = MathHelper.LsbMsbShort(binaryEnsemble[30], binaryEnsemble[31]) * 0.1f;           // dm to m
                ensemble.DvlData.RefLayerMin = (int)(MathHelper.LsbMsbUShort(binaryEnsemble[30], binaryEnsemble[31]) * 0.1f);                           // dm to m
                ensemble.DvlData.RefLayerMax = (int)(MathHelper.LsbMsbUShort(binaryEnsemble[32], binaryEnsemble[33]) * 0.1f);                           // dm to m

            }

            #endregion

            #endregion

            #region PD5

            // If PD5
            // PD5 will have more bytes than PD4
            if(binaryEnsemble.Length >= PD5_ENS_SIZE)
            {
                // Decode PD5 values
                
                // Salinity
                ensemble.AncillaryData.Salinity = binaryEnsemble[45];
                ensemble.BottomTrackData.Salinity = binaryEnsemble[45];
                ensemble.DvlData.Salinity = binaryEnsemble[45];

                // Tranducer Depth
                ensemble.AncillaryData.TransducerDepth = MathHelper.LsbMsbUShort(binaryEnsemble[46], binaryEnsemble[47]) * 0.1f;
                ensemble.BottomTrackData.TransducerDepth = MathHelper.LsbMsbUShort(binaryEnsemble[46], binaryEnsemble[47]) * 0.1f;
                ensemble.DvlData.TransducerDepth = MathHelper.LsbMsbUShort(binaryEnsemble[46], binaryEnsemble[47]) * 0.1f;

                // Heading
                ensemble.AncillaryData.Heading = MathHelper.LsbMsbUShort(binaryEnsemble[52], binaryEnsemble[53]) * 0.01f;
                ensemble.BottomTrackData.Heading = MathHelper.LsbMsbUShort(binaryEnsemble[52], binaryEnsemble[53]) * 0.01f;
                ensemble.DvlData.Heading = MathHelper.LsbMsbUShort(binaryEnsemble[52], binaryEnsemble[53]) * 0.01f;

                // Pitch
                ensemble.AncillaryData.Pitch = MathHelper.LsbMsbShort(binaryEnsemble[48], binaryEnsemble[49]) * 0.01f;
                ensemble.BottomTrackData.Pitch = MathHelper.LsbMsbShort(binaryEnsemble[48], binaryEnsemble[49]) * 0.01f;
                ensemble.DvlData.Pitch = MathHelper.LsbMsbShort(binaryEnsemble[48], binaryEnsemble[49]) * 0.01f;

                // Roll
                ensemble.AncillaryData.Roll = MathHelper.LsbMsbShort(binaryEnsemble[50], binaryEnsemble[51]) * 0.01f;
                ensemble.BottomTrackData.Roll = MathHelper.LsbMsbShort(binaryEnsemble[50], binaryEnsemble[51]) * 0.01f;
                ensemble.DvlData.Roll = MathHelper.LsbMsbShort(binaryEnsemble[50], binaryEnsemble[51]) * 0.01f;

                // DMG
                ensemble.DvlData.DmgEast = MathHelper.ByteArrayToInt32(binaryEnsemble, 54) * 0.1f;                                      // dm to m
                ensemble.DvlData.DmgNorth = MathHelper.ByteArrayToInt32(binaryEnsemble, 58) * 0.1f;                                     // dm to m
                ensemble.DvlData.DmgUpward = MathHelper.ByteArrayToInt32(binaryEnsemble, 62) * 0.1f;                                    // dm to m
                ensemble.DvlData.DmgError = MathHelper.ByteArrayToInt32(binaryEnsemble, 66) * 0.1f;                                     // dm to m

                // DMG Ref Layer
                ensemble.DvlData.DmgRefEast = MathHelper.ByteArrayToInt32(binaryEnsemble, 70) * 0.1f;                                   // dm to m
                ensemble.DvlData.DmgRefNorth = MathHelper.ByteArrayToInt32(binaryEnsemble, 74) * 0.1f;                                  // dm to m
                ensemble.DvlData.DmgRefUpward = MathHelper.ByteArrayToInt32(binaryEnsemble, 78) * 0.1f;                                 // dm to m
                ensemble.DvlData.DmgRefError = MathHelper.ByteArrayToInt32(binaryEnsemble, 82) * 0.1f;                                  // dm to m

            }

            #endregion

            // Send an event that data was processed
            // in this format
            if (ProcessDataEvent != null)
            {
                ProcessDataEvent(binaryEnsemble, ensemble);
            }

            // Reset incoming data timeout
            _incomingDataTimeout = 0;

            return ensemble;
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
            //lock (_headerStartLock)
            //{
                //Debug.WriteLine("PD4_5Codec: SearchHeaderStart() Lock Open");
                if(!_continue)
                {
                    return;
                }

                lock (_headerStartLock)
                {
                    // Clear the header
                    _headerStart.Clear();
                }

                // Find the beginning of an ensemble
                // Also make sure we are still running with _continue
                while (_incomingDataBuffer.Count > ID_SIZE && _continue)
                {
                    // Populate the buffer if its empty
                    if (_headerStart.Count < ID_SIZE)
                    {
                        lock (_headerStartLock)
                        {
                            // Get the header start from the buffer
                            for (int x = 0; x < ID_SIZE; x++)
                            {
                                // Store the value to the header start array
                                _headerStart.Add(_incomingDataBuffer.Take());
                            }
                        }
                    }

                    // Find a start
                    if (_headerStart.Count >= ID_SIZE && (_headerStart[0] == ID && (_headerStart[1] == ID_PD4 || _headerStart[1] == ID_PD5)) && _continue)
                    {
                        lock (_headerStartLock)
                        {
                            // Get the next 2 bytes for the ensemble size
                            _headerStart.Add(_incomingDataBuffer.Take());
                            _headerStart.Add(_incomingDataBuffer.Take());
                        }

                        break;
                    }
                    // Remove the first byte until you find the start
                    else
                    {
                        if (_headerStart.Count > 0 && _continue)
                        {
                            lock (_headerStartLock)
                            {
                                _headerStart.RemoveAt(0);
                                _headerStart.Add(_incomingDataBuffer.Take());
                            }
                        }
                    }
                }
            //}
            //Debug.WriteLine("PD4_5Codec: SearchHeaderStart() Lock Closed");
        }

        /// <summary>
        /// If the header was found, the next 2 bytes should be the ensemble size.
        /// Get the ensemble size and set it to the variable.  Add 2 to the size to 
        /// include the checksum.
        /// </summary>
        /// <returns>If the size can be found, return true.</returns>
        private bool GetEnsembleSize()
        {
            lock (_headerStartLock)
            {
                //Debug.WriteLine("PD4_5Codec: GetEnsembleSize() Lock Open");
                if (_headerStart.Count >= HEADER_SIZE)
                {
                    byte[] ensSize = new byte[2];
                    ensSize[0] = _headerStart[2];
                    ensSize[1] = _headerStart[3];
                    _currentEnsembleSize = MathHelper.ByteArrayToUInt16(ensSize, 0) + CHECKSUM_SIZE;
                    //Debug.WriteLine("PD4_5Codec: GetEnsembleSize() Internal Lock Closed");
                    return true;
                }
            }
            //Debug.WriteLine("PD4_5Codec: GetEnsembleSize() Lock Closed");
            return false;
        }

        /// <summary>
        /// Get the checksum value for the ensemble.  It is the 
        /// last 2 bytes of the ensemble.  The last 2 bytes should
        /// be 0's if you visually inspect the ensemble.
        /// This will find the checksum value and convert the value
        /// to a long.
        /// </summary>
        /// <param name="ensemble">Good ensemble containing a checksum value.</param>
        /// <returns>Checksum value converted from byte array to long.</returns>
        private long RetrieveEnsembleChecksum(byte[] ensemble)
        {
           return MathHelper.LsbMsbUShort(ensemble[ensemble.Length - 2], ensemble[ensemble.Length - 1]);
        }

        #region System Config

        /// <summary>
        /// Get the System configuration.
        /// </summary>
        /// <param name="value">Value to get the configuration.</param>
        /// <returns>The system configuration.</returns>
        private SystemConfig GetSystemConfig(byte value)
        {
            SystemConfig sysCfg = new SystemConfig();

            sysCfg.VelTransform = GetTransform(value);
            sysCfg.IsTiltUsed = GetTilt(value);
            sysCfg.Is3BeamSolution = Get3BeamSolution(value);
            sysCfg.Frequency = GetFreq(value);

            return sysCfg;
        }

        /// <summary>
        /// Get the velocity transformation.
        /// </summary>
        /// <param name="val">Value to get the transform.</param>
        /// <returns>Velocity Transform.</returns>
        private Core.Commons.Transforms GetTransform(byte val)
        {
            // Beam
            if(!MathHelper.IsBitSet(val, 7) && !MathHelper.IsBitSet(val, 6))
            {
                return Core.Commons.Transforms.BEAM;
            }

            // Instrument
            if (!MathHelper.IsBitSet(val, 7) && MathHelper.IsBitSet(val, 6))
            {
                return Core.Commons.Transforms.INSTRUMENT;
            }

            // Ship
            if (MathHelper.IsBitSet(val, 7) && !MathHelper.IsBitSet(val, 6))
            {
                return Core.Commons.Transforms.SHIP;
            }

            // Earth
            if (MathHelper.IsBitSet(val, 7) && MathHelper.IsBitSet(val, 6))
            {
                return Core.Commons.Transforms.EARTH;
            }

            return Core.Commons.Transforms.BEAM;
        }

        /// <summary>
        /// Get if tilt was used
        /// in the calculations.
        /// </summary>
        /// <param name="val">Value to check.</param>
        /// <returns>TRUE = Use tilts for calculation.</returns>
        private bool GetTilt(byte val)
        {
            if(MathHelper.IsBitSet(val, 5))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get if 3 beam solution was used.
        /// </summary>
        /// <param name="val">Value to check.</param>
        /// <returns>TRUE = 3 Beam solution.</returns>
        private bool Get3BeamSolution(byte val)
        {
            if (MathHelper.IsBitSet(val, 4))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get the system frequency.
        /// </summary>
        /// <param name="val">Value to get the system frequency.</param>
        /// <returns>System frequency.</returns>
        private Subsystem GetFreq(byte val)
        {
            // 300 kHz
            if (!MathHelper.IsBitSet(val, 2) && MathHelper.IsBitSet(val, 1) && !MathHelper.IsBitSet(val, 0))
            {
                return new Subsystem(Subsystem.SUB_300KHZ_4BEAM_20DEG_PISTON_4);
            }

            // 600 kHz
            if (!MathHelper.IsBitSet(val, 2) && MathHelper.IsBitSet(val, 1) && MathHelper.IsBitSet(val, 0))
            {
                return new Subsystem(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3);
            }

            // 1200 kHz
            if (MathHelper.IsBitSet(val, 2) && !MathHelper.IsBitSet(val, 1) && !MathHelper.IsBitSet(val, 0))
            {
                return new Subsystem(Subsystem.SUB_1_2MHZ_4BEAM_20DEG_PISTON_2);
            }

            return new Subsystem(Subsystem.SUB_1_2MHZ_4BEAM_20DEG_PISTON_2);
        }

        #endregion

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
        /// codec.ProcessDataEvent += new codec.ProcessDataEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// codec.ProcessDataEvent -= (method to call)
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
        /// codec.ProcessDataCompleteEvent += new codec.ProcessDataCompleteEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// codec.ProcessDataCompleteEvent -= (method to call)
        /// </summary>
        public event ProcessDataCompleteEventHandler ProcessDataCompleteEvent;

        #endregion
    }

}