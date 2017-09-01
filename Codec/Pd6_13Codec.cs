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
 * 06/16/2014      RC          2.22.1     Initial coding
 * 06/23/2014      RC          2.22.1     Fixed setting the time for the ensemble.
 * 07/01/2014      RC          2.23.0     Fixed bug in SetBS() and FindSentence().
 * 09/16/2014      RC          3.0.1      Fixed bug in FindSentence() looking for the next start location.
 *                                         Fixed bug in SetBE() setting the earth bottom track velocities.
 * 10/02/2014      RC          3.0.2      Fixed bug in FindSentence() when bad data is given.
 * 10/27/2014      RC          3.0.2      Fixed bug in FindSentence() removing bad characters.
 * 07/09/2015      RC          3.0.5      Mode the codec a thread.
 * 07/20/2015      RC          3.0.5      Fixed codec to also do playback and use the NMEA data.
 * 08/11/2015      RC          3.0.5      Verfiy the sentences are correct before processing them.
 * 10/07/2015      RC          3.2.0      Set the BT range values in SetBD().
 * 04/27/2017      RC          3.4.2      Check for buffer overflow with _incomingDataTimeout.
 * 
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using log4net;

    /// <summary>
    /// Parse the PD6 and PD13 data.
    /// </summary>
    public class Pd6_13Codec : ICodec, IDisposable
    {
        #region Variable

        /// <summary>
        /// The header has at least 4 bytes.
        /// </summary>
        private const int PD6_MIN = 6;

        /// <summary>
        /// Logger for logging error messages.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Set a maximum buffer size to prevent
        /// a large buffer.
        /// </summary>
        private const int MAX_BUFFER_SIZE = 1000;

        /// <summary>
        /// All NMEA sentences should end with this.
        /// </summary>
        public const string NMEA_END = "\r\n";

        /// <summary>
        /// Trim these value from the end of a 
        /// sentence.
        /// </summary>
        private char[] REMOVE_END = { '\r', '\n' };

        /// <summary>
        /// Size the checksum including
        /// the *.  The checksum contains
        /// 2 bytes.
        /// </summary>
        public const int NMEA_CHECKSUM_SIZE = 3;

        /// <summary>
        /// Size of the NMEA buffer.
        /// </summary>
        private const int NMEA_BUFFER_SIZE = 100;

        /// <summary>
        /// Lock the buffer of data.
        /// </summary>
        private readonly object _bufferLock = new object();

        /// <summary>
        /// Buffer to hold incoming data.
        /// </summary>
        private string _buffer;

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
        /// Set a timeout if the incoming data is accumulating and
        /// never finding any ensembles.
        /// </summary>
        private int _incomingDataTimeout;

        /// <summary>
        /// Number of times to take incoming data before a timeout
        /// occurs and the data is cleared.
        /// </summary>
        private const int INCOMING_DATA_TIMEOUT = 50;

        #region Previous Values

        /// <summary>
        /// Last SA data recieved.
        /// </summary>
        private SA _prevSA;

        /// <summary>
        /// Last TA data received.
        /// </summary>
        private TS _prevTS;

        /// <summary>
        /// Last RA data received.
        /// </summary>
        private RA _prevRA;

        /// <summary>
        /// Last WI data received.
        /// </summary>
        private WI _prevWI;

        /// <summary>
        /// Last WS data received.
        /// </summary>
        private WS _prevWS;

        /// <summary>
        /// Last WE data received.
        /// </summary>
        private WE _prevWE;

        /// <summary>
        /// Last WD data received.
        /// </summary>
        private WD _prevWD;

        /// <summary>
        /// Last BI data received.
        /// </summary>
        private BI _prevBI;

        /// <summary>
        /// Last BS data received.
        /// </summary>
        private BS _prevBS;

        /// <summary>
        /// Last BE data received.
        /// </summary>
        private BE _prevBE;

        /// <summary>
        /// Last BD data received.
        /// </summary>
        private BD _prevBD;

        /// <summary>
        /// An ensemble to hold all previous sentence.
        /// </summary>
        private DataSet.Ensemble _prevEns;

        /// <summary>
        /// Flag if the previous sentence was a NMEA sentence.
        /// This is used to keep track of new ensembles.
        /// </summary>
        private bool _prevNMEA = false;

        /// <summary>
        /// Previous Date and time.  This is used to
        /// keep track of the ping time.
        /// </summary>
        private DateTime _prevDateAndTime;

        /// <summary>
        /// Data count.
        /// </summary>
        private int _count;

        #endregion

        #endregion

        /// <summary>
        /// Decode the PD6 and PD13 ASCII data.
        /// </summary>
        public Pd6_13Codec()
        {
            _prevEns = new DataSet.Ensemble();
            _incomingDataTimeout = 0;

            // Clear the values
            Clear();

            // Initialize the thread
            _continue = true;
            _eventWaitData = new EventWaitHandle(false, EventResetMode.AutoReset);
            _processDataThread = new Thread(ProcessDataThread);
            _processDataThread.Name = "PD6 PD13 Codec";
            _processDataThread.Start();
        }

        /// <summary>
        /// Shutdown the codec.
        /// </summary>
        public void Dispose()
        {
            StopThread();

            ClearIncomingData();

            _eventWaitData.Dispose();
        }

        #region Incomming Data

        /// <summary>
        /// Take incoming data and add it to the
        /// buffer to be decoded.
        /// </summary>
        /// <param name="data">Data to add to incoming buffer.</param>
        public void AddIncomingData(byte[] data)
        {
            if (data != null)
            {
                // Convert to string 
                string rcvData = System.Text.ASCIIEncoding.ASCII.GetString(data);

                // Add the data to the buffer
                lock (_bufferLock)
                {
                    _buffer += rcvData;
                }

                // Wake up the thread to process data
                _eventWaitData.Set();
            }

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
            // Clear buffer
            lock (_bufferLock)
            {
                _buffer = "";
            }

            // Reinitialize the values
            Clear();
        }

        /// <summary>
        /// Reinitialize the values.
        /// </summary>
        private void Clear()
        {
            // Initialize all the sentences
            _prevSA = null;
            _prevTS = null;
            _prevRA = null;
            _prevWI = null;
            _prevWS = null;
            _prevWE = null;
            _prevWD = null;
            _prevBI = null;
            _prevBS = null;
            _prevBE = null;
            _prevBD = null;
            //_prevEns = new DataSet.Ensemble();
            _count = 0;
        }

        #endregion

        #region Process data

        /// <summary>
        /// Find the first :. Then find the next start
        /// of a sentence.  Remove the string of a complete
        /// sentence.
        /// </summary>
        private void ProcessDataThread()
        {
            while (_continue)
            {
                try
                {
                    // Block until awoken when data is received
                    // Timeout every 60 seconds to see if shutdown occured
                    _eventWaitData.WaitOne();

                    // If wakeup was called to kill thread
                    if (!_continue)
                    {
                        return;
                    }


                    string bufferCopy;
                    lock (_bufferLock)
                    {
                        // Copy the buffer and clear it
                        bufferCopy = _buffer;
                        _buffer = "";
                    }
                    if (bufferCopy.Length > 0)
                    {
                        using (var reader = new System.IO.StringReader(bufferCopy))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                ProcessSentence(line);
                            }
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
                    log.Error("Error processing binary codec data.", e);
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
            //_eventWaitData.Dispose();

            try
            {
                // Force the thread to stop
                //_processDataThread.Abort();
                _processDataThread.Join(1000);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Process the sentence.  Check which 
        /// type of setntence it is and create the data set.
        /// Then publish the data.
        /// </summary>
        /// <param name="sentence">Sentence to process.</param>
        private void ProcessSentence(string sentence)
        {
            // Flags to know which sentence was found
            bool isSA = false;
            bool isTS = false;
            bool isRA = false;
            bool isWI = false;
            bool isWS = false;
            bool isWE = false;
            bool isWD = false;
            bool isBI = false;
            bool isBS = false;
            bool isBE = false;
            bool isBD = false;
            bool isNMEA = false;

            try
            {
                // If the previous was a NMEA and the new sentence is not
                // NMEA data, then a new ensemble will need to be created.
                if (sentence.Contains("$"))
                {
                    isNMEA = true;
                }
                // previous was NMEA and current one is not NMEA
                if (_prevNMEA && !isNMEA)
                {
                    _prevEns = new DataSet.Ensemble();
                }
                // Reset the values
                _prevNMEA = isNMEA;

                // Create the DVL dataset if it does not exist
                if (!_prevEns.IsDvlDataAvail)
                {
                    _prevEns.IsDvlDataAvail = true;
                    _prevEns.DvlData = new DataSet.DvlDataSet();
                }

                // Create the Ensemble dataset if it does not exist
                if (!_prevEns.IsEnsembleAvail)
                {
                    _prevEns.IsEnsembleAvail = true;
                    _prevEns.EnsembleData = new DataSet.EnsembleDataSet();
                }

                // Add Ensemble DataSet
                //_prevEns.EnsembleData.EnsembleNumber = _count++;
                //_prevEns.EnsembleData.SetTime();
                _prevEns.EnsembleData.SysSerialNumber = SerialNumber.DVL;
                _prevEns.EnsembleData.NumBeams = 4;

                // SA
                if (sentence.Contains(SA.ID))
                {
                    // Set SA
                    SetSA(sentence);

                    // Set flag that this data type was found
                    isSA = true;
                }

                // TS
                if (sentence.Contains(TS.ID))
                {
                    // Set TS
                    SetTS(sentence);

                    // Set flag that this data type was found
                    isTS = true;
                }

                // RA
                if (sentence.Contains(RA.ID))
                {
                    // Set RA
                    SetRA(sentence);

                    // Set flag that this data type was found
                    isRA = true;
                }

                // WI
                if (sentence.Contains(WI.ID))
                {
                    // Set WI
                    SetWI(sentence);

                    // Set flag that this data type was found
                    isWI = true;
                }

                // WS
                if (sentence.Contains(WS.ID))
                {
                    // Set WS
                    SetWS(sentence);

                    // Set flag that this data type was found
                    isWS = true;
                }

                // WE
                if (sentence.Contains(WE.ID))
                {
                    // Set WE
                    SetWE(sentence);

                    // Set flag that this data type was found
                    isWE = true;
                }

                // WD
                if (sentence.Contains(WD.ID))
                {
                    // Set WD
                    SetWD(sentence);

                    // Set flag that this data type was found
                    isWD = true;
                }

                // BI
                if (sentence.Contains(BI.ID))
                {
                    // Set BI
                    SetBI(sentence);

                    // Set flag that this data type was found
                    isBI = true;
                }

                // BS
                if (sentence.Contains(BS.ID))
                {
                    // Set BS
                    SetBS(sentence);

                    // Set flag that this data type was found
                    isBS = true;
                }

                // BE
                if (sentence.Contains(BE.ID))
                {
                    // Set BE
                    SetBE(sentence);

                    // Set flag that this data type was found
                    isBE = true;
                }

                // BD
                if (sentence.Contains(BD.ID))
                {
                    // Set BD
                    SetBD(sentence);

                    // Set flag that this data type was found
                    isBD = true;
                }

                // NMEA
                if(sentence.Contains('$'))
                {
                    // Set NMEA
                    SetNmea(sentence);

                    // Set flag that this data type was found
                    isNMEA = true;
                }

                // Send data to subscriber
                // SA
                if (isSA)
                {
                    SendData(_prevSA);
                }
                // TS
                if (isTS)
                {
                    SendData(_prevTS);
                }
                // RA
                if (isRA)
                {
                    SendData(_prevRA);
                }
                // WI
                if (isWI)
                {
                    SendData(_prevWI);
                }
                // WS
                if (isWS)
                {
                    SendData(_prevWS);
                }
                // WE
                if (isWE)
                {
                    SendData(_prevWE);
                }
                // WD
                if (isWD)
                {
                    SendData(_prevWD);
                }
                // BI
                if (isBI)
                {
                    SendData(_prevBI);
                }
                // BS
                if (isBS)
                {
                    SendData(_prevBS);
                }
                // BE
                if (isBE)
                {
                    SendData(_prevBE);
                }
                // BD
                if (isBD)
                {
                    SendData(_prevBD);
                }
                // NMEA
                if(isNMEA)
                {
                    //SendData(_prevNMEA);
                }
            }
            catch(Exception e)
            {
                log.Error("Error processing data.", e);
            }

        }

        #endregion

        #region Set Data

        /// <summary>
        /// Set the data sets for the SA messages.
        /// </summary>
        /// <param name="sentence">Message with the SA data.</param>
        private void SetSA(string sentence)
        {
            // Verify byte count
            if(sentence.Count(x => x == ',') != SA.NUM_ELEM-1)
            {
                return;
            }

            var sa = new SA(sentence);

            if(sa != null)
            {
                // Add DVL dataset
                if (_prevEns.DvlData == null)
                {
                    _prevEns.DvlData = new DataSet.DvlDataSet();
                    _prevEns.IsDvlDataAvail = true;
                }
                _prevEns.DvlData.Heading = sa.Heading;
                _prevEns.DvlData.Pitch = sa.Pitch;
                _prevEns.DvlData.Roll = sa.Roll;

                // Add Ancillary DataSet
                if (!_prevEns.IsAncillaryAvail)
                {
                    _prevEns.AncillaryData = new DataSet.AncillaryDataSet();
                    _prevEns.IsAncillaryAvail = true;
                }
                _prevEns.AncillaryData.Heading = sa.Heading;
                _prevEns.AncillaryData.Pitch = sa.Pitch;
                _prevEns.AncillaryData.Roll = sa.Roll;

                // Add Bottom Track DataSet
                if (!_prevEns.IsBottomTrackAvail)
                {
                    _prevEns.BottomTrackData = new DataSet.BottomTrackDataSet();
                    _prevEns.IsBottomTrackAvail = true;
                }
                _prevEns.BottomTrackData.Heading = sa.Heading;
                _prevEns.BottomTrackData.Pitch = sa.Pitch;
                _prevEns.BottomTrackData.Roll = sa.Roll;
            }

            // Set the value
            _prevSA = sa;
        }

        /// <summary>
        /// Set the data sets for the TS message.
        /// </summary>
        /// <param name="sentence">Message with the TS data.</param>
        private void SetTS(string sentence)
        {
            // Verify byte count
            if (sentence.Count(x => x == ',') != TS.NUM_ELEM-1)
            {
                return;
            }

            var ts = new TS(sentence);

            if (ts != null)
            {
                // Add DVL dataset
                if (_prevEns.DvlData == null)
                {
                    _prevEns.DvlData = new DataSet.DvlDataSet();
                    _prevEns.IsDvlDataAvail = true;
                }
                // Get the first and last date and time to determine the ping time
                var diffTime = ts.DateAndTime - _prevDateAndTime;
                float pingTime = (float)diffTime.TotalSeconds;

                _prevEns.DvlData.DateAndTime = ts.DateAndTime;
                _prevDateAndTime = ts.DateAndTime;
                _prevEns.DvlData.Salinity = ts.Salinity;
                _prevEns.DvlData.WaterTemp = ts.Temperature;
                _prevEns.DvlData.TransducerDepth = ts.DepthOfTransducer;
                _prevEns.DvlData.SpeedOfSound = ts.SpeedOfSound;
                _prevEns.DvlData.BIT = ts.BIT;
                _prevEns.DvlData.LeakDetection = ts.LeakDetection;

                // Add Ancillary DataSet
                if (!_prevEns.IsAncillaryAvail)
                {
                    _prevEns.AncillaryData = new DataSet.AncillaryDataSet();
                    _prevEns.IsAncillaryAvail = true;
                }
                _prevEns.AncillaryData.Salinity = ts.Salinity;
                _prevEns.AncillaryData.WaterTemp = ts.Temperature;
                _prevEns.AncillaryData.TransducerDepth = ts.DepthOfTransducer;
                _prevEns.AncillaryData.SpeedOfSound = ts.SpeedOfSound;
                _prevEns.AncillaryData.FirstPingTime += pingTime;                           // Accumulate the time ping
                _prevEns.AncillaryData.LastPingTime += pingTime;                            // Accumulate the time ping

                // Add Bottom Track DataSet
                if (!_prevEns.IsBottomTrackAvail)
                {
                    _prevEns.BottomTrackData = new DataSet.BottomTrackDataSet();
                    _prevEns.IsBottomTrackAvail = true;
                }
                _prevEns.BottomTrackData.Salinity = ts.Salinity;
                _prevEns.BottomTrackData.WaterTemp = ts.Temperature;
                _prevEns.BottomTrackData.TransducerDepth = ts.DepthOfTransducer;
                _prevEns.BottomTrackData.SpeedOfSound = ts.SpeedOfSound;
                _prevEns.BottomTrackData.FirstPingTime += pingTime;                           // Accumulate the time ping
                _prevEns.BottomTrackData.LastPingTime += pingTime;                            // Accumulate the time ping

                // Add Ensemble DataSet
                if (!_prevEns.IsEnsembleAvail)
                {
                    _prevEns.EnsembleData = new DataSet.EnsembleDataSet();
                    _prevEns.IsEnsembleAvail = true;
                }
                _prevEns.EnsembleData.SetTime(ts.DateAndTime);
                _prevEns.EnsembleData.EnsembleNumber = _count++;
            }

            _prevTS = ts;
        }

        /// <summary>
        /// Set the data sets for the RA message.
        /// </summary>
        /// <param name="sentence">Message with the RA data.</param>
        private void SetRA(string sentence)
        {
            // Verify byte count
            if (sentence.Count(x => x == ',') != RA.NUM_ELEM - 1)
            {
                return;
            }

            var ra = new RA(sentence);

            if (ra != null)
            {
                // Add DVL dataset
                if (_prevEns.DvlData == null)
                {
                    _prevEns.DvlData = new DataSet.DvlDataSet();
                    _prevEns.IsDvlDataAvail = true;
                }
                _prevEns.DvlData.Pressure = ra.Pressure;
                _prevEns.DvlData.RangeBeam0 = ra.RangeToBottomB0;
                _prevEns.DvlData.RangeBeam1 = ra.RangeToBottomB1;
                _prevEns.DvlData.RangeBeam2 = ra.RangeToBottomB2;
                _prevEns.DvlData.RangeBeam3 = ra.RangeToBottomB3;

                // Add Ancillary DataSet
                if (!_prevEns.IsAncillaryAvail)
                {
                    _prevEns.AncillaryData = new DataSet.AncillaryDataSet();
                    _prevEns.IsAncillaryAvail = true;
                }
                _prevEns.AncillaryData.Pressure = ra.Pressure / 0.0001f;

                // Add Bottom Track DataSet
                if (!_prevEns.IsBottomTrackAvail)
                {
                    _prevEns.BottomTrackData = new DataSet.BottomTrackDataSet();
                    _prevEns.IsBottomTrackAvail = true;
                }
                _prevEns.BottomTrackData.Pressure = ra.Pressure / 0.0001f;
                _prevEns.BottomTrackData.Range[0] = ra.RangeToBottomB0;
                _prevEns.BottomTrackData.Range[1] = ra.RangeToBottomB1;
                _prevEns.BottomTrackData.Range[2] = ra.RangeToBottomB2;
                _prevEns.BottomTrackData.Range[3] = ra.RangeToBottomB3;
            }

            _prevRA = ra;
        }

        /// <summary>
        /// Set the data sets for the WI message.
        /// </summary>
        /// <param name="sentence">Message with the WI data.</param>
        private void SetWI(string sentence)
        {
            // Verify byte count
            if (sentence.Count(x => x == ',') != WI.NUM_ELEM - 1)
            {
                return;
            }

            var wi = new WI(sentence);

            if (wi != null)
            {
                // Add DVL dataset
                if (_prevEns.DvlData == null)
                {
                    _prevEns.DvlData = new DataSet.DvlDataSet();
                    _prevEns.IsDvlDataAvail = true;
                }

                // Check for bad velocity
                // WmXVelocity
                if (wi.X == DataSet.Ensemble.BAD_VELOCITY || wi.X == PD0.BAD_VELOCITY)
                {
                    _prevEns.DvlData.WmXVelocity = DataSet.Ensemble.BAD_VELOCITY;
                }
                else
                {
                    _prevEns.DvlData.WmXVelocity = wi.X * 0.001f;
                }

                // WmYVelocity
                if (wi.Y == DataSet.Ensemble.BAD_VELOCITY || wi.Y == PD0.BAD_VELOCITY)
                {
                    _prevEns.DvlData.WmYVelocity = DataSet.Ensemble.BAD_VELOCITY;
                }
                else
                {
                    _prevEns.DvlData.WmYVelocity = wi.Y * 0.001f;
                }

                // WmZVelocity
                if (wi.Z == DataSet.Ensemble.BAD_VELOCITY || wi.Z == PD0.BAD_VELOCITY)
                {
                    _prevEns.DvlData.WmZVelocity = DataSet.Ensemble.BAD_VELOCITY;
                }
                else
                {
                    _prevEns.DvlData.WmZVelocity = wi.Z * 0.001f;
                }

                // WmErrorVelocity
                if (wi.Q == DataSet.Ensemble.BAD_VELOCITY || wi.Q == PD0.BAD_VELOCITY)
                {
                    _prevEns.DvlData.WmErrorVelocity = DataSet.Ensemble.BAD_VELOCITY;
                }
                else
                {
                    _prevEns.DvlData.WmErrorVelocity = wi.Q * 0.001f;
                }

                _prevEns.DvlData.WmInstrumentIsGoodVelocity = wi.IsGood;

                // Add Instrument Water Mass DataSet
                if (!_prevEns.IsInstrumentWaterMassAvail)
                {
                    _prevEns.InstrumentWaterMassData = new DataSet.InstrumentWaterMassDataSet();
                    _prevEns.IsInstrumentWaterMassAvail = true;
                }
                _prevEns.InstrumentWaterMassData.VelocityX = _prevEns.DvlData.WmXVelocity;
                _prevEns.InstrumentWaterMassData.VelocityY = _prevEns.DvlData.WmYVelocity;
                _prevEns.InstrumentWaterMassData.VelocityZ = _prevEns.DvlData.WmZVelocity;
                _prevEns.InstrumentWaterMassData.VelocityQ = _prevEns.DvlData.WmErrorVelocity;
            }

            _prevWI = wi;
        }

        /// <summary>
        /// Set the data sets for the WS message.
        /// </summary>
        /// <param name="sentence">Message with the WS data.</param>
        private void SetWS(string sentence)
        {
            // Verify byte count
            if (sentence.Count(x => x == ',') != WS.NUM_ELEM - 1)
            {
                return;
            }

            var ws = new WS(sentence);

            if (ws != null)
            {
                // Add DVL dataset
                if (_prevEns.DvlData == null)
                {
                    _prevEns.DvlData = new DataSet.DvlDataSet();
                    _prevEns.IsDvlDataAvail = true;
                }

                // Check for bad velocity
                // WmTransverseVelocity
                if (ws.T == DataSet.Ensemble.BAD_VELOCITY || ws.T == PD0.BAD_VELOCITY)
                {
                    _prevEns.DvlData.WmTransverseVelocity = DataSet.Ensemble.BAD_VELOCITY;
                }
                else
                {
                    _prevEns.DvlData.WmTransverseVelocity = ws.T * 0.001f;
                }

                // WmLongitudinalVelocity
                if (ws.L == DataSet.Ensemble.BAD_VELOCITY || ws.L == PD0.BAD_VELOCITY)
                {
                    _prevEns.DvlData.WmLongitudinalVelocity = DataSet.Ensemble.BAD_VELOCITY;
                }
                else
                {
                    _prevEns.DvlData.WmLongitudinalVelocity = ws.L * 0.001f;
                }

                // WmNormalVelocity
                if (ws.N == DataSet.Ensemble.BAD_VELOCITY || ws.N == PD0.BAD_VELOCITY)
                {
                    _prevEns.DvlData.WmNormalVelocity = DataSet.Ensemble.BAD_VELOCITY;
                }
                else
                {
                    _prevEns.DvlData.WmNormalVelocity = ws.N * 0.001f;
                }

                _prevEns.DvlData.WmShipIsGoodVelocity = ws.IsGood;

                // Add Ship Water Mass DataSet
                if (!_prevEns.IsShipWaterMassAvail)
                {
                    _prevEns.ShipWaterMassData = new DataSet.ShipWaterMassDataSet();
                    _prevEns.IsShipWaterMassAvail = true;
                }
                _prevEns.ShipWaterMassData.VelocityTransverse = _prevEns.DvlData.WmTransverseVelocity;
                _prevEns.ShipWaterMassData.VelocityLongitudinal = _prevEns.DvlData.WmLongitudinalVelocity;
                _prevEns.ShipWaterMassData.VelocityNormal = _prevEns.DvlData.WmNormalVelocity;
            }

            _prevWS = ws;
        }

        /// <summary>
        /// Set the data sets for the WE message.
        /// </summary>
        /// <param name="sentence">Message with the WE data.</param>
        private void SetWE(string sentence)
        {
            // Verify byte count
            if (sentence.Count(x => x == ',') != WE.NUM_ELEM - 1)
            {
                return;
            }

            var we = new WE(sentence);

            if (we != null)
            {
                // Add DVL dataset
                if (_prevEns.DvlData == null)
                {
                    _prevEns.DvlData = new DataSet.DvlDataSet();
                    _prevEns.IsDvlDataAvail = true;
                }

                // Check for bad velocity
                // WmEastVelocity
                if (we.E == DataSet.Ensemble.BAD_VELOCITY || we.E == PD0.BAD_VELOCITY)
                {
                    _prevEns.DvlData.WmEastVelocity = DataSet.Ensemble.BAD_VELOCITY;
                }
                else
                {
                    _prevEns.DvlData.WmEastVelocity = we.E * 0.001f;
                }

                // WmNorthVelocity
                if (we.N == DataSet.Ensemble.BAD_VELOCITY || we.N == PD0.BAD_VELOCITY)
                {
                    _prevEns.DvlData.WmNorthVelocity = DataSet.Ensemble.BAD_VELOCITY;
                }
                else
                {
                    _prevEns.DvlData.WmNorthVelocity = we.N * 0.001f;
                }

                // WmUpwardVelocity
                if (we.U == DataSet.Ensemble.BAD_VELOCITY || we.U == PD0.BAD_VELOCITY)
                {
                    _prevEns.DvlData.WmUpwardVelocity = DataSet.Ensemble.BAD_VELOCITY;
                }
                else
                {
                    _prevEns.DvlData.WmUpwardVelocity = we.U * 0.001f;
                }

                _prevEns.DvlData.WmEarthIsGoodVelocity = we.IsGood;

                // Add Earth Water Mass DataSet
                if (!_prevEns.IsEarthWaterMassAvail)
                {
                    _prevEns.EarthWaterMassData = new DataSet.EarthWaterMassDataSet();
                    _prevEns.IsEarthWaterMassAvail = true;
                }
                _prevEns.EarthWaterMassData.VelocityEast = _prevEns.DvlData.WmEastVelocity;
                _prevEns.EarthWaterMassData.VelocityNorth = _prevEns.DvlData.WmNorthVelocity;
                _prevEns.EarthWaterMassData.VelocityVertical = _prevEns.DvlData.WmUpwardVelocity;
            }

            _prevWE = we;
        }

        /// <summary>
        /// Set the data sets for the WD message.
        /// </summary>
        /// <param name="sentence">Message with the WD data.</param>
        private void SetWD(string sentence)
        {
            // Verify byte count
            if (sentence.Count(x => x == ',') != WD.NUM_ELEM - 1)
            {
                return;
            }

            var wd = new WD(sentence);

            if (wd != null)
            {
                // Add DVL dataset
                if (_prevEns.DvlData == null)
                {
                    _prevEns.DvlData = new DataSet.DvlDataSet();
                    _prevEns.IsDvlDataAvail = true;
                }
                _prevEns.DvlData.WmEastDistance = wd.E;
                _prevEns.DvlData.WmNorthDistance = wd.N;
                _prevEns.DvlData.WmUpwardDistance = wd.U;
                _prevEns.DvlData.WmEarthRangeToWaterMassCenter = wd.RangeToWmCenter;
                _prevEns.DvlData.WmEarthTimeLastGoodVel = wd.Time;

                // Add Instrument Water Mass DataSet
                if (!_prevEns.IsInstrumentWaterMassAvail)
                {
                    _prevEns.InstrumentWaterMassData = new DataSet.InstrumentWaterMassDataSet();
                    _prevEns.IsInstrumentWaterMassAvail = true;
                }
                _prevEns.InstrumentWaterMassData.WaterMassDepthLayer = wd.RangeToWmCenter;

                // Add Earth Water Mass DataSet
                if (!_prevEns.IsEarthWaterMassAvail)
                {
                    _prevEns.EarthWaterMassData = new DataSet.EarthWaterMassDataSet();
                    _prevEns.IsEarthWaterMassAvail = true;
                }
                _prevEns.EarthWaterMassData.WaterMassDepthLayer = wd.RangeToWmCenter;
            }

            _prevWD = wd;
        }

        /// <summary>
        /// Set the data sets for the BI message.
        /// </summary>
        /// <param name="sentence">Message with the BI data.</param>
        private void SetBI(string sentence)
        {
            // Verify byte count
            if (sentence.Count(x => x == ',') != BI.NUM_ELEM - 1)
            {
                return;
            }

            var bi = new BI(sentence);

            if (bi != null)
            {
                // Add DVL dataset
                if (_prevEns.DvlData == null)
                {
                    _prevEns.DvlData = new DataSet.DvlDataSet();
                    _prevEns.IsDvlDataAvail = true;
                }

                // Check for bad velocity
                // X
                if (bi.X == DataSet.Ensemble.BAD_VELOCITY || bi.X == PD0.BAD_VELOCITY)
                {
                    _prevEns.DvlData.BtXVelocity = DataSet.Ensemble.BAD_VELOCITY;
                }
                else
                {
                    _prevEns.DvlData.BtXVelocity = bi.X * 0.001f;
                }

                // Y
                if (bi.Y == DataSet.Ensemble.BAD_VELOCITY || bi.Y == PD0.BAD_VELOCITY)
                {
                    _prevEns.DvlData.BtYVelocity = DataSet.Ensemble.BAD_VELOCITY;
                }
                else
                {
                    _prevEns.DvlData.BtYVelocity = bi.Y * 0.001f;
                }

                // Z
                if (bi.Z == DataSet.Ensemble.BAD_VELOCITY || bi.Z == PD0.BAD_VELOCITY)
                {
                    _prevEns.DvlData.BtZVelocity = DataSet.Ensemble.BAD_VELOCITY;
                }
                else
                {
                    _prevEns.DvlData.BtZVelocity = bi.Z * 0.001f;
                }

                // Q
                if (bi.Q == DataSet.Ensemble.BAD_VELOCITY || bi.Q == PD0.BAD_VELOCITY)
                {
                    _prevEns.DvlData.BtErrorVelocity = DataSet.Ensemble.BAD_VELOCITY;
                }
                else
                {
                    _prevEns.DvlData.BtErrorVelocity = bi.Q * 0.001f;
                }
                
                // Set good flag
                _prevEns.DvlData.BtInstrumentIsGoodVelocity = bi.IsGood;

                // Add Bottom Track DataSet
                if (!_prevEns.IsBottomTrackAvail)
                {
                    _prevEns.BottomTrackData = new DataSet.BottomTrackDataSet();
                    _prevEns.IsBottomTrackAvail = true;
                }
                _prevEns.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_X_INDEX] = _prevEns.DvlData.BtXVelocity;
                _prevEns.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_Y_INDEX] = _prevEns.DvlData.BtYVelocity;
                _prevEns.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_Z_INDEX] = _prevEns.DvlData.BtZVelocity;
                _prevEns.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_Q_INDEX] = _prevEns.DvlData.BtErrorVelocity;
            }

            _prevBI = bi;
        }

        /// <summary>
        /// Set the data sets for the BS message.
        /// </summary>
        /// <param name="sentence">Message with the BS data.</param>
        private void SetBS(string sentence)
        {
            // Verify byte count
            if (sentence.Count(x => x == ',') != BS.NUM_ELEM - 1)
            {
                return;
            }

            var bs = new BS(sentence);

            if (bs != null)
            {
                // Add DVL dataset
                if (_prevEns.DvlData == null)
                {
                    _prevEns.DvlData = new DataSet.DvlDataSet();
                    _prevEns.IsDvlDataAvail = true;
                }

                // Check for bad velocity
                // BtTransverseVelocity
                if (bs.T == DataSet.Ensemble.BAD_VELOCITY || bs.T == PD0.BAD_VELOCITY)
                {
                    _prevEns.DvlData.BtTransverseVelocity = DataSet.Ensemble.BAD_VELOCITY;
                }
                else
                {
                    _prevEns.DvlData.BtTransverseVelocity = bs.T * 0.001f;
                }

                // BtLongitudinalVelocity
                if (bs.L == DataSet.Ensemble.BAD_VELOCITY || bs.L == PD0.BAD_VELOCITY)
                {
                    _prevEns.DvlData.BtLongitudinalVelocity = DataSet.Ensemble.BAD_VELOCITY;
                }
                else
                {
                    _prevEns.DvlData.BtLongitudinalVelocity = bs.L * 0.001f;
                }

                // BtNormalVelocity
                if (bs.N == DataSet.Ensemble.BAD_VELOCITY || bs.N == PD0.BAD_VELOCITY)
                {
                    _prevEns.DvlData.BtNormalVelocity = DataSet.Ensemble.BAD_VELOCITY;
                }
                else
                {
                    _prevEns.DvlData.BtNormalVelocity = bs.N * 0.001f;
                }

                _prevEns.DvlData.BtShipIsGoodVelocity = bs.IsGood;
            }

            _prevBS = bs;
        }

        /// <summary>
        /// Set the data sets for the BE message.
        /// </summary>
        /// <param name="sentence">Message with the BE data.</param>
        private void SetBE(string sentence)
        {
            // Verify byte count
            if (sentence.Count(x => x == ',') != BE.NUM_ELEM - 1)
            {
                return;
            }

            var be = new BE(sentence);

            if (be != null)
            {
                // Add DVL dataset
                if (_prevEns.DvlData == null)
                {
                    _prevEns.DvlData = new DataSet.DvlDataSet();
                    _prevEns.IsDvlDataAvail = true;
                }

                // Check for bad velocity
                // BtEastVelocity
                if (be.E == DataSet.Ensemble.BAD_VELOCITY || be.E == PD0.BAD_VELOCITY)
                {
                    _prevEns.DvlData.BtEastVelocity = DataSet.Ensemble.BAD_VELOCITY;
                }
                else
                {
                    _prevEns.DvlData.BtEastVelocity = be.E * 0.001f;
                }

                // BtNorthVelocity
                if (be.N == DataSet.Ensemble.BAD_VELOCITY || be.N == PD0.BAD_VELOCITY)
                {
                    _prevEns.DvlData.BtNorthVelocity = DataSet.Ensemble.BAD_VELOCITY;
                }
                else
                {
                    _prevEns.DvlData.BtNorthVelocity = be.N * 0.001f;
                }

                // BtUpwardVelocity
                if (be.U == DataSet.Ensemble.BAD_VELOCITY || be.U == PD0.BAD_VELOCITY)
                {
                    _prevEns.DvlData.BtUpwardVelocity = DataSet.Ensemble.BAD_VELOCITY;
                }
                else
                {
                    _prevEns.DvlData.BtUpwardVelocity = be.U * 0.001f;
                }


                _prevEns.DvlData.BtEarthIsGoodVelocity = be.IsGood;

                // Add Bottom Track DataSet
                if (!_prevEns.IsBottomTrackAvail)
                {
                    _prevEns.BottomTrackData = new DataSet.BottomTrackDataSet();
                    _prevEns.IsBottomTrackAvail = true;
                }
                _prevEns.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_EAST_INDEX] = _prevEns.DvlData.BtEastVelocity;
                _prevEns.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_NORTH_INDEX] = _prevEns.DvlData.BtNorthVelocity;
                _prevEns.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX] = _prevEns.DvlData.BtUpwardVelocity;
            }

            _prevBE = be;
        }

        /// <summary>
        /// Set the data sets for the BD message.
        /// </summary>
        /// <param name="sentence">Message with the BD data.</param>
        private void SetBD(string sentence)
        {
            // Verify byte count
            if (sentence.Count(x => x == ',') != BD.NUM_ELEM - 1)
            {
                return;
            }

            var bd = new BD(sentence);

            if (bd != null)
            {
                // Add DVL dataset
                if (_prevEns.DvlData == null)
                {
                    _prevEns.DvlData = new DataSet.DvlDataSet();
                    _prevEns.IsDvlDataAvail = true;
                }
                _prevEns.DvlData.BtEastDistance = bd.E;
                _prevEns.DvlData.BtNorthDistance = bd.N;
                _prevEns.DvlData.BtUpwardDistance = bd.U;
                _prevEns.DvlData.BtRangeToBottom = bd.RangeToBottom;
                _prevEns.DvlData.BtEarthTimeLastGoodVel = bd.Time;

                                // Add DVL dataset
                if (_prevEns.DvlData == null)
                {
                    _prevEns.DvlData = new DataSet.DvlDataSet();
                    _prevEns.IsDvlDataAvail = true;
                }
                _prevEns.DvlData.RangeBeam0 = bd.RangeToBottom;
                _prevEns.DvlData.RangeBeam1 = bd.RangeToBottom;
                _prevEns.DvlData.RangeBeam2 = bd.RangeToBottom;
                _prevEns.DvlData.RangeBeam3 = bd.RangeToBottom;


                // Add Bottom Track DataSet
                if (!_prevEns.IsBottomTrackAvail)
                {
                    _prevEns.BottomTrackData = new DataSet.BottomTrackDataSet();
                    _prevEns.IsBottomTrackAvail = true;
                }
                _prevEns.BottomTrackData.Range[0] = bd.RangeToBottom;
                _prevEns.BottomTrackData.Range[1] = bd.RangeToBottom;
                _prevEns.BottomTrackData.Range[2] = bd.RangeToBottom;
                _prevEns.BottomTrackData.Range[3] = bd.RangeToBottom;
            }

            _prevBD = bd;
        }

        /// <summary>
        /// Add the NMEA data to the dataset.
        /// </summary>
        /// <param name="sentence">NMEA data to add to the dataset.</param>
        private void SetNmea(string sentence)
        {
            if (_prevEns.NmeaData == null)
            {
                _prevEns.NmeaData = new DataSet.NmeaDataSet(sentence);
                _prevEns.IsNmeaAvail = true;
            }
            else
            {
                // Add NMEA data
                // Max of 15 NMEA messages per ensemble
                // This will remove out the old data
                _prevEns.NmeaData.MergeNmeaData(sentence, 15);
            }
        }

        #endregion

        #region Send Data

        /// <summary>
        /// Send the data to all the subscribers.
        /// </summary>
        /// <param name="data">Data to send to the subscriber.</param>
        private void SendData(IDvlData data)
        {
            // Send an event that data was processed
            // in this format
            if (data != null && ProcessDataEvent != null)
            {
                ProcessDataEvent(data.ToByteArray(), _prevEns);
            }

            // Reset the incoming data timeout
            _incomingDataTimeout = 0;
        }

        #endregion

        #region Events

        /// <summary>
        /// Event To subscribe to.  This gives the paramater
        /// that will be passed when subscribing to the event.
        /// </summary>
        /// <param name="byteArray">DVL data as a byte array.</param>
        /// <param name="EnsembleData">DataSet to record and process.</param>
        public delegate void ProcessDataEventHandler(byte[] byteArray, DataSet.Ensemble EnsembleData);

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
