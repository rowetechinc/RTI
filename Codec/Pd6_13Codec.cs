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
 * 
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
            // Clear the values
            Clear();
        }

        /// <summary>
        /// Shutdown the codec.
        /// </summary>
        public void Dispose()
        {
            ClearIncomingData();
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

                // Start to process the data
                ProcessData();
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
            _prevEns = new DataSet.Ensemble();
            _count = 0;
        }

        #endregion

        #region Process data

        /// <summary>
        /// Find the first :. Then find the next start
        /// of a sentence.  Remove the string of a complete
        /// sentence.
        /// </summary>
        private void ProcessData()
        {
            // Find a sentence in the buffer
            string sent = FindSentence();

            // If one is found, process the data
            if (sent.Length > 0)
            {
                // Process sentence
                ProcessSentence(sent);
            }

            // Check if we should continue processing data
            if (_buffer.Length > PD6_MIN && _buffer.Contains(":"))
            {
                ProcessData();
            }
        }

        /// <summary>
        /// Find a sentence in 
        /// the buffer.  The sentence will start with a colon.
        /// If one is found return it as a string.
        /// </summary>
        /// <returns>Sentence found in buffer.</returns>
        private string FindSentence()
        {

            string sent = "";
            lock (_bufferLock)
            {
                try
                {
                    // Remove everything to first ':'
                    int begin = _buffer.IndexOf(":");
                    if (begin > 0)
                    {
                        _buffer = _buffer.Remove(0, begin);
                    }

                    // Find the next start sentence
                    int nextSentLoc = 0;
                    if (begin >= 0 && _buffer.Length > begin + 1 )
                    {
                        nextSentLoc = _buffer.IndexOf(":", begin + 1);
                    }

                    // If another sentence is found, process the current sentence
                    if (nextSentLoc > 0)
                    {
                        sent = _buffer.Substring(0, nextSentLoc);           // Get the sentence
                        _buffer = _buffer.Remove(0, sent.Length);           // Remove it from the buffer

                        // Remove any trailing new lines
                        sent = sent.TrimEnd(REMOVE_END);
                    }

                    // Ensure buffer does not overflow
                    if (_buffer.Length > MAX_BUFFER_SIZE)
                    {
                        _buffer = _buffer.Substring(_buffer.Length - MAX_BUFFER_SIZE, MAX_BUFFER_SIZE);
                    }

                    // Sentence not found so remove the first char
                    if (string.IsNullOrEmpty(sent) && _buffer.Length > 0)
                    {
                        _buffer = _buffer.Remove(0);
                    }
                }
                catch (Exception e)
                {
                    log.Error("Error finding a sentence", e);
                }
            }

            return sent;
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

            // Create the DVL dataset if it does not exist
            if (!_prevEns.IsDvlDataAvail)
            {
                _prevEns.IsDvlDataAvail = true;
                _prevEns.DvlData = new DataSet.DvlDataSet();
            }

            // Create the Ensemble dataset if it does not exist
            if(!_prevEns.IsEnsembleAvail)
            {
                _prevEns.IsEnsembleAvail = true;
                _prevEns.EnsembleData = new DataSet.EnsembleDataSet();
            }

            // Add Ensemble DataSet
            _prevEns.EnsembleData.EnsembleNumber = _count++;
            //_prevEns.EnsembleData.SetTime();
            _prevEns.EnsembleData.SysSerialNumber = SerialNumber.DVL;

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

        }

        #endregion

        #region Set Data

        /// <summary>
        /// Set the data sets for the SA messages.
        /// </summary>
        /// <param name="sentence">Message with the SA data.</param>
        private void SetSA(string sentence)
        {
            _prevSA = new SA(sentence);

            // Add DVL dataset
            if (_prevEns.DvlData == null)
            {
                _prevEns.DvlData = new DataSet.DvlDataSet();
                _prevEns.IsDvlDataAvail = true;
            }
            _prevEns.DvlData.Heading = _prevSA.Heading;
            _prevEns.DvlData.Pitch = _prevSA.Pitch;
            _prevEns.DvlData.Roll = _prevSA.Roll;

            // Add Ancillary DataSet
            if (!_prevEns.IsAncillaryAvail)
            {
                _prevEns.AncillaryData = new DataSet.AncillaryDataSet();
                _prevEns.IsAncillaryAvail = true;
            }
            _prevEns.AncillaryData.Heading = _prevSA.Heading;
            _prevEns.AncillaryData.Pitch = _prevSA.Pitch;
            _prevEns.AncillaryData.Roll = _prevSA.Roll;

            // Add Bottom Track DataSet
            if (!_prevEns.IsBottomTrackAvail)
            {
                _prevEns.BottomTrackData = new DataSet.BottomTrackDataSet();
                _prevEns.IsBottomTrackAvail = true;
            }
            _prevEns.BottomTrackData.Heading = _prevSA.Heading;
            _prevEns.BottomTrackData.Pitch = _prevSA.Pitch;
            _prevEns.BottomTrackData.Roll = _prevSA.Roll;
        }

        /// <summary>
        /// Set the data sets for the TS message.
        /// </summary>
        /// <param name="sentence">Message with the TS data.</param>
        private void SetTS(string sentence)
        {
            _prevTS = new TS(sentence);

            // Add DVL dataset
            if (_prevEns.DvlData == null)
            {
                _prevEns.DvlData = new DataSet.DvlDataSet();
                _prevEns.IsDvlDataAvail = true;
            }
            _prevEns.DvlData.DateAndTime = _prevTS.DateAndTime;
            _prevEns.DvlData.Salinity = _prevTS.Salinity;
            _prevEns.DvlData.Temperature = _prevTS.Temperature;
            _prevEns.DvlData.DepthOfTransducer = _prevTS.DepthOfTransducer;
            _prevEns.DvlData.SpeedOfSound = _prevTS.SpeedOfSound;
            _prevEns.DvlData.BIT = _prevTS.BIT;

            // Add Ancillary DataSet
            if (!_prevEns.IsAncillaryAvail)
            {
                _prevEns.AncillaryData = new DataSet.AncillaryDataSet();
                _prevEns.IsAncillaryAvail = true;
            }
            _prevEns.AncillaryData.Salinity = _prevTS.Salinity;
            _prevEns.AncillaryData.WaterTemp = _prevTS.Temperature;
            _prevEns.AncillaryData.TransducerDepth = _prevTS.DepthOfTransducer;
            _prevEns.AncillaryData.SpeedOfSound = _prevTS.SpeedOfSound;

            // Add Bottom Track DataSet
            if (!_prevEns.IsBottomTrackAvail)
            {
                _prevEns.BottomTrackData = new DataSet.BottomTrackDataSet();
                _prevEns.IsBottomTrackAvail = true;
            }
            _prevEns.BottomTrackData.Salinity = _prevTS.Salinity;
            _prevEns.BottomTrackData.WaterTemp = _prevTS.Temperature;
            _prevEns.BottomTrackData.TransducerDepth = _prevTS.DepthOfTransducer;
            _prevEns.BottomTrackData.SpeedOfSound = _prevTS.SpeedOfSound;

            // Add Ensemble DataSet
            if (!_prevEns.IsEnsembleAvail)
            {
                _prevEns.EnsembleData = new DataSet.EnsembleDataSet();
                _prevEns.IsEnsembleAvail = true;
            }
            _prevEns.EnsembleData.SetTime(_prevTS.DateAndTime);

        }

        /// <summary>
        /// Set the data sets for the RA message.
        /// </summary>
        /// <param name="sentence">Message with the RA data.</param>
        private void SetRA(string sentence)
        {
            _prevRA = new RA(sentence);

            // Add DVL dataset
            if (_prevEns.DvlData == null)
            {
                _prevEns.DvlData = new DataSet.DvlDataSet();
                _prevEns.IsDvlDataAvail = true;
            }
            _prevEns.DvlData.Pressure = _prevRA.Pressure;
            _prevEns.DvlData.RangeBeam0 = _prevRA.RangeToBottomB0;
            _prevEns.DvlData.RangeBeam1 = _prevRA.RangeToBottomB1;
            _prevEns.DvlData.RangeBeam2 = _prevRA.RangeToBottomB2;
            _prevEns.DvlData.RangeBeam3 = _prevRA.RangeToBottomB3;

            // Add Ancillary DataSet
            if (!_prevEns.IsAncillaryAvail)
            {
                _prevEns.AncillaryData = new DataSet.AncillaryDataSet();
                _prevEns.IsAncillaryAvail = true;
            }
            _prevEns.AncillaryData.Pressure = _prevRA.Pressure / 0.0001f;

            // Add Bottom Track DataSet
            if (!_prevEns.IsBottomTrackAvail)
            {
                _prevEns.BottomTrackData = new DataSet.BottomTrackDataSet();
                _prevEns.IsBottomTrackAvail = true;
            }
            _prevEns.BottomTrackData.Pressure = _prevRA.Pressure / 0.0001f;
            _prevEns.BottomTrackData.Range[0] = _prevRA.RangeToBottomB0;
            _prevEns.BottomTrackData.Range[1] = _prevRA.RangeToBottomB1;
            _prevEns.BottomTrackData.Range[2] = _prevRA.RangeToBottomB2;
            _prevEns.BottomTrackData.Range[3] = _prevRA.RangeToBottomB3;
        }

        /// <summary>
        /// Set the data sets for the WI message.
        /// </summary>
        /// <param name="sentence">Message with the WI data.</param>
        private void SetWI(string sentence)
        {
            _prevWI = new WI(sentence);

            // Add DVL dataset
            if (_prevEns.DvlData == null)
            {
                _prevEns.DvlData = new DataSet.DvlDataSet();
                _prevEns.IsDvlDataAvail = true;
            }
            _prevEns.DvlData.WmXVelocity = _prevWI.X * 0.001f;
            _prevEns.DvlData.WmYVelocity = _prevWI.Y * 0.001f;
            _prevEns.DvlData.WmZVelocity = _prevWI.Z * 0.001f;
            _prevEns.DvlData.WmErrorVelocity = _prevWI.Q * 0.001f;
            _prevEns.DvlData.WmInstrumentIsGoodVelocity = _prevWI.IsGood;

            // Add Instrument Water Mass DataSet
            if (!_prevEns.IsInstrumentWaterMassAvail)
            {
                _prevEns.InstrumentWaterMassData = new DataSet.InstrumentWaterMassDataSet();
                _prevEns.IsInstrumentWaterMassAvail = true;
            }
            _prevEns.InstrumentWaterMassData.VelocityX = _prevWI.X * 0.001f;
            _prevEns.InstrumentWaterMassData.VelocityY = _prevWI.Y * 0.001f;
            _prevEns.InstrumentWaterMassData.VelocityZ = _prevWI.Z * 0.001f;
            _prevEns.InstrumentWaterMassData.VelocityQ = _prevWI.Q * 0.001f;
        }

        /// <summary>
        /// Set the data sets for the WS message.
        /// </summary>
        /// <param name="sentence">Message with the WS data.</param>
        private void SetWS(string sentence)
        {
            _prevWS = new WS(sentence);

            // Add DVL dataset
            if (_prevEns.DvlData == null)
            {
                _prevEns.DvlData = new DataSet.DvlDataSet();
                _prevEns.IsDvlDataAvail = true;
            }
            _prevEns.DvlData.WmTransverseVelocity = _prevWS.T * 0.001f;
            _prevEns.DvlData.WmLongitudinalVelocity = _prevWS.L * 0.001f;
            _prevEns.DvlData.WmNormalVelocity = _prevWS.N * 0.001f;
            _prevEns.DvlData.WmShipIsGoodVelocity = _prevWS.IsGood;
        }

        /// <summary>
        /// Set the data sets for the WE message.
        /// </summary>
        /// <param name="sentence">Message with the WE data.</param>
        private void SetWE(string sentence)
        {
            _prevWE = new WE(sentence);

            // Add DVL dataset
            if (_prevEns.DvlData == null)
            {
                _prevEns.DvlData = new DataSet.DvlDataSet();
                _prevEns.IsDvlDataAvail = true;
            }
            _prevEns.DvlData.WmEastVelocity = _prevWE.E * 0.001f;
            _prevEns.DvlData.WmNorthVelocity = _prevWE.N * 0.001f;
            _prevEns.DvlData.WmUpwardVelocity = _prevWE.U * 0.001f;
            _prevEns.DvlData.WmEarthIsGoodVelocity = _prevWE.IsGood;

            // Add Earth Water Mass DataSet
            if (!_prevEns.IsEarthWaterMassAvail)
            {
                _prevEns.EarthWaterMassData = new DataSet.EarthWaterMassDataSet();
                _prevEns.IsEarthWaterMassAvail = true;
            }
            _prevEns.EarthWaterMassData.VelocityEast = _prevWE.E * 0.001f;
            _prevEns.EarthWaterMassData.VelocityNorth = _prevWE.N * 0.001f;
            _prevEns.EarthWaterMassData.VelocityVertical = _prevWE.U * 0.001f;
        }

        /// <summary>
        /// Set the data sets for the WD message.
        /// </summary>
        /// <param name="sentence">Message with the WD data.</param>
        private void SetWD(string sentence)
        {
            _prevWD = new WD(sentence);

            // Add DVL dataset
            if (_prevEns.DvlData == null)
            {
                _prevEns.DvlData = new DataSet.DvlDataSet();
                _prevEns.IsDvlDataAvail = true;
            }
            _prevEns.DvlData.WmEastDistance = _prevWD.E;
            _prevEns.DvlData.WmNorthDistance = _prevWD.N;
            _prevEns.DvlData.WmUpwardDistance = _prevWD.U;
            _prevEns.DvlData.WmEarthRangeToWaterMassCenter = _prevWD.RangeToWmCenter;
            _prevEns.DvlData.WmEarthTimeLastGoodVel = _prevWD.Time;

            // Add Instrument Water Mass DataSet
            if (!_prevEns.IsInstrumentWaterMassAvail)
            {
                _prevEns.InstrumentWaterMassData = new DataSet.InstrumentWaterMassDataSet();
                _prevEns.IsInstrumentWaterMassAvail = true;
            }
            _prevEns.InstrumentWaterMassData.WaterMassDepthLayer = _prevWD.RangeToWmCenter;

            // Add Earth Water Mass DataSet
            if (!_prevEns.IsEarthWaterMassAvail)
            {
                _prevEns.EarthWaterMassData = new DataSet.EarthWaterMassDataSet();
                _prevEns.IsEarthWaterMassAvail = true;
            }
            _prevEns.EarthWaterMassData.WaterMassDepthLayer = _prevWD.RangeToWmCenter;
        }

        /// <summary>
        /// Set the data sets for the BI message.
        /// </summary>
        /// <param name="sentence">Message with the BI data.</param>
        private void SetBI(string sentence)
        {
            _prevBI = new BI(sentence);

            // Add DVL dataset
            if (_prevEns.DvlData == null)
            {
                _prevEns.DvlData = new DataSet.DvlDataSet();
                _prevEns.IsDvlDataAvail = true;
            }
            _prevEns.DvlData.BtXVelocity = _prevBI.X * 0.001f;
            _prevEns.DvlData.BtYVelocity = _prevBI.Y * 0.001f;
            _prevEns.DvlData.BtZVelocity = _prevBI.Z * 0.001f;
            _prevEns.DvlData.BtErrorVelocity = _prevBI.Q * 0.001f;
            _prevEns.DvlData.BtInstrumentIsGoodVelocity = _prevBI.IsGood;

            // Add Bottom Track DataSet
            if (!_prevEns.IsBottomTrackAvail)
            {
                _prevEns.BottomTrackData = new DataSet.BottomTrackDataSet();
                _prevEns.IsBottomTrackAvail = true;
            }
            _prevEns.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_X_INDEX] = _prevBI.X * 0.001f;
            _prevEns.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_Y_INDEX] = _prevBI.Y * 0.001f;
            _prevEns.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_Z_INDEX] = _prevBI.Z * 0.001f;
            _prevEns.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_Q_INDEX] = _prevBI.Q * 0.001f;
        }

        /// <summary>
        /// Set the data sets for the BS message.
        /// </summary>
        /// <param name="sentence">Message with the BS data.</param>
        private void SetBS(string sentence)
        {
            _prevBS = new BS(sentence);

            // Add DVL dataset
            if (_prevEns.DvlData == null)
            {
                _prevEns.DvlData = new DataSet.DvlDataSet();
                _prevEns.IsDvlDataAvail = true;
            }
            _prevEns.DvlData.BtTransverseVelocity = _prevBS.T * 0.001f;
            _prevEns.DvlData.BtLongitudinalVelocity = _prevBS.L * 0.001f;
            _prevEns.DvlData.BtNormalVelocity = _prevBS.N * 0.001f;
            _prevEns.DvlData.BtShipIsGoodVelocity = _prevBS.IsGood;
        }

        /// <summary>
        /// Set the data sets for the BE message.
        /// </summary>
        /// <param name="sentence">Message with the BE data.</param>
        private void SetBE(string sentence)
        {
            _prevBE = new BE(sentence);

            // Add DVL dataset
            if (_prevEns.DvlData == null)
            {
                _prevEns.DvlData = new DataSet.DvlDataSet();
                _prevEns.IsDvlDataAvail = true;
            }
            _prevEns.DvlData.BtEastVelocity = _prevBE.E * 0.001f;
            _prevEns.DvlData.BtNorthVelocity = _prevBE.N * 0.001f;
            _prevEns.DvlData.BtUpwardVelocity = _prevBE.U * 0.001f;
            _prevEns.DvlData.BtEarthIsGoodVelocity = _prevBE.IsGood;

            // Add Bottom Track DataSet
            if (!_prevEns.IsBottomTrackAvail)
            {
                _prevEns.BottomTrackData = new DataSet.BottomTrackDataSet();
                _prevEns.IsBottomTrackAvail = true;
            }
            _prevEns.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_EAST_INDEX] = _prevBE.E * 0.001f;
            _prevEns.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_NORTH_INDEX] = _prevBE.N * 0.001f;
            _prevEns.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX] = _prevBE.U * 0.001f;
        }

        /// <summary>
        /// Set the data sets for the BD message.
        /// </summary>
        /// <param name="sentence">Message with the BD data.</param>
        private void SetBD(string sentence)
        {
            _prevBD = new BD(sentence);

            // Add DVL dataset
            if (_prevEns.DvlData == null)
            {
                _prevEns.DvlData = new DataSet.DvlDataSet();
                _prevEns.IsDvlDataAvail = true;
            }
            _prevEns.DvlData.BtEastDistance = _prevBD.E;
            _prevEns.DvlData.BtNorthDistance = _prevBD.N;
            _prevEns.DvlData.BtUpwardDistance = _prevBD.U;
            _prevEns.DvlData.BtRangeToBottom = _prevBD.RangeToBottom;
            _prevEns.DvlData.BtEarthTimeLastGoodVel = _prevBD.Time;
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
            if (ProcessDataEvent != null)
            {
                ProcessDataEvent(data.ToByteArray(), _prevEns);
            }
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

        #endregion

    }
}
