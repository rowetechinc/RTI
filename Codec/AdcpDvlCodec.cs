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
 * HISTORY
 * -----------------------------------------------------------------
 * Date            Initials    Version    Comments
 * -----------------------------------------------------------------
 * 11/28/2011      RC                     Initial coding
 * 11/29/2011      RC                     Remove buffer.
 * 12/08/2011      RC          1.09       Added Water Mass dataset parsing.
 * 12/13/2011      RC          1.09       Rewrite how to parse and record data.
 * 12/14/2011      RC          1.09       Added ProcessDataEvent event.
 *                                         Clear _buffer in ClearIncomingData.
 * 12/15/2011      RC          1.09       Created FindSentence() to lock buffer.
 * 12/16/2011      RC          1.10       Add a lock around NmeaBuffer.
 * 12/20/2011      RC          1.10       Continue to ProcessData if contains $ and *.
 * 12/29/2011      RC          1.11       Removed RecorderManager and added Events.
 * 01/03/2012      RC          1.11       Set a buffer size limit and check for limit when finding a sentence.
 *                                         Added _prevBuffer to prevent StackOverflowException.
 * 06/28/2013      RC          2.19       Replaced Shutdown() with IDisposable.
 * 08/02/2013      RC          2.19.4     Have ProcessDataEventHandler match AdcpBinaryCodec and give both binary and ensemble.  Removed BinaryRecordEvent.
 * 02/06/2014      RC          2.21.3     Added ability to decode PRIT03 sentence.
 * 02/13/2014      RC          2.21.3     Pass all the DVL data to as binary data to be recorded in SendData().
 * 07/24/2014      RC          2.23.0     Fixed a bug if AddIncomingData() received a null byte array.
 * 07/24/2014      RC          2.23.0     When adding PRTI30 and PRTI31 messages, replace the serialnumber subsystem with the correct value.
 * 07/31/2014      RC          2.23.0     Removed trimming the sentence in FindSentence().
 * 06/17/2015      RC          3.0.5      Removed the recusive call in ProcessData().
 * 07/09/2015      RC          3.0.5      Made the codec its own thread.
 * 08/13/2015      RC          3.0.5      Added complete event.
 * 04/27/2017      RC          3.4.2      Check for buffer overflow with _incomingDataTimeout.
 * 
 */


using DotSpatial.Positioning;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using log4net;
using System.Threading;
namespace RTI
{
    /// <summary>
    /// Decode the data received from the ADCP in
    /// DVL mode.  This data is formated like NMEA data.
    /// </summary>
    public class AdcpDvlCodec: ICodec, IDisposable
    {

        #region Variables
 
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
        /// Lock the buffer of data.
        /// </summary>
        private readonly object _nmeaBufferLock = new object();

        /// <summary>
        /// Buffer to hold incoming data.
        /// </summary>
        private string _buffer;

        /// <summary>
        /// Temporary variable to store a 
        /// PRTI01 Sentence so it can be
        /// combined with a PRTI02 Sentence.
        /// </summary>
        private Prti01Sentence _prti01;

        /// <summary>
        /// Temporary variable to store a 
        /// PRTI30 Sentence so it can be
        /// combined with a PRTI02 Sentence, PRTI01 and PRTI31.
        /// </summary>
        private Prti30Sentence _prti30;

        /// <summary>
        /// Temporary variable to store a 
        /// PRTI31 Sentence so it can be
        /// combined with a PRTI02 Sentence, PRTI01 and PRTI30.
        /// </summary>
        private Prti31Sentence _prti31;

        /// <summary>
        /// A buffer to store the current NMEA data.
        /// </summary>
        private LinkedList<string> _nmeaBuffer;

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

        #endregion

        #region Properties

        /// <summary>
        /// Use the Bottom Track Heading, Pitch and Roll Value.
        /// If set to false, it will use the Water Track Heading, Pitch
        /// and Roll value.
        /// </summary>
        public bool IsUseBtHpr { get; set; }

        #endregion

        /// <summary>
        /// Constructor
        /// Initialize any values.
        /// </summary>
        public AdcpDvlCodec()
        {
            // Initialize values
            //_adcpData = null;
            _prti01 = null;

            // Default to use BT Heading, Pitch and Roll
            IsUseBtHpr = true;

            _nmeaBuffer = new LinkedList<string>();

            // Initialize the thread
            _continue = true;
            _eventWaitData = new EventWaitHandle(false, EventResetMode.AutoReset);
            _processDataThread = new Thread(ProcessDataThread);
            _processDataThread.Name = "DVL Codec";
            _processDataThread.Start();
        }

        /// <summary>
        /// Shutdown this object.
        /// </summary>
        public void Dispose()
        {
            StopThread();

            // Send any remaining data
            if (_prti01 != null)
            {
                SendData(_prti01);
            }

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

            lock (_nmeaBufferLock)
            {
                // Clear the NMEA buffer.
                _nmeaBuffer.Clear();
            }
        }

        #endregion

        #region Send Data

        /// <summary>
        /// If there is data that has not been sent,
        /// send the data and set to null.
        /// </summary>
        /// <param name="adcpData">Data to be recorded.</param>
        /// <param name="adcpMsg">ADCP message as a byte array.</param>
        private void SendData(DataSet.Ensemble adcpData, byte[] adcpMsg)
        {
            // Send any remaining data
            if (adcpData != null)
            {
                // Byte array of the binary data
                // Add the ADCP message
                List<byte> byteList = new List<byte>(adcpMsg);

                // Get all the GPS NMEA strings
                // Add them to the ensemble
                // and add it to the list
                byte[] nmeaBA = GetNmeaData(ref adcpData);
                byteList.AddRange(nmeaBA);

                // Send an event that data was processed
                // in this format
                if (ProcessDataEvent != null)
                {
                    ProcessDataEvent(byteList.ToArray(), adcpData);
                }
            }
        }

        /// <summary>
        /// Create the data set using PRTI01.
        /// Then send the dataset.
        /// </summary>
        /// <param name="sentence">PRTI01 Sentence.</param>
        private void SendData(Prti01Sentence sentence)
        {
            // Create the dataset
            DataSet.Ensemble adcpData = CreateDataSet(sentence);

            // Send the data set
            SendData(adcpData, sentence.ToByteArray());
        }

        /// <summary>
        /// Create the data set using PRTI02.
        /// Then send the dataset.
        /// </summary>
        /// <param name="sentence">PRTI02 Sentence.</param>
        private void SendData(Prti02Sentence sentence)
        {
            // Create the dataset
            DataSet.Ensemble adcpData = CreateDataSet(sentence);

            // Send the data set
            SendData(adcpData, sentence.ToByteArray());
        }

        /// <summary>
        /// Create the data set using PRTI03.
        /// Then send the dataset.
        /// </summary>
        /// <param name="sentence">PRTI03 Sentence.</param>
        private void SendData(Prti03Sentence sentence)
        {
            // Create the dataset
            DataSet.Ensemble adcpData = CreateDataSet(sentence);

            // Send the data set
            SendData(adcpData, sentence.ToByteArray());
        }

        /// <summary>
        /// Create the data set using PRTI01.  Then add PRTI02 to the
        /// dataset.  Then send the dataset.
        /// </summary>
        /// <param name="prti01">PRTI01 Sentence.</param>
        /// <param name="prti02">PRTI02 Sentence.</param>
        /// <param name="prti30">PRTI30 Sentence.</param>
        /// <param name="prti31">PRTI31 Sentence.</param>
        private void SendData(Prti01Sentence prti01, Prti02Sentence prti02, Prti30Sentence prti30 = null, Prti31Sentence prti31 = null)
        {
            // Create the dataset
            DataSet.Ensemble adcpData = CreateDataSet(prti01);
            adcpData.AddAdditionalBottomTrackData(prti02);

            if (IsUseBtHpr)
            {
                if (prti30 != null)
                {
                    adcpData.AddAdditionalAncillaryData(prti30);
                    adcpData.AddAdditionalBottomTrackData(prti30);

                    // Setup the serial number
                    if (adcpData.IsEnsembleAvail)
                    {
                        // Remove the temp serial number subsystem
                        adcpData.EnsembleData.SysSerialNumber.RemoveSubsystem(SerialNumber.DVL_Subsystem);

                        // Add the actual subsystem
                        adcpData.EnsembleData.SysSerialNumber.AddSubsystem(prti30.SubsystemConfig.SubSystem);
                    }
                }
            }
            else
            {
                if (prti31 != null)
                {
                    adcpData.AddAdditionalAncillaryData(prti31);
                    adcpData.AddAdditionalBottomTrackData(prti31);

                    // Setup the serial number
                    if (adcpData.IsEnsembleAvail)
                    {
                        // Remove the temp serial number subsystem
                        adcpData.EnsembleData.SysSerialNumber.RemoveSubsystem(SerialNumber.DVL_Subsystem);

                        // Add the actual subsystem
                        adcpData.EnsembleData.SysSerialNumber.AddSubsystem(prti30.SubsystemConfig.SubSystem);
                    }
                }
            }

            // Combine all the messages and convert to a byte array
            string adcpMesg = "";
            if (prti01 != null)
            {
                adcpMesg += prti01.ToString();
            }
            if (prti02 != null)
            {
                adcpMesg += prti02.ToString();
            }
            if (prti30 != null)
            {
                adcpMesg += prti30.ToString();
            }
            if (prti31 != null)
            {
                adcpMesg += prti31.ToString();
            }
            byte[] adcpMesgBA = Encoding.ASCII.GetBytes(adcpMesg);

            // Send the data set
            SendData(adcpData, adcpMesgBA);
        }

        #endregion

        #region Process data

        /// <summary>
        /// Find the first $. Then find the first start
        /// of a checksum.  Remove the string of a complete
        /// NMEA sentence and check if its a valid sentence.
        /// If it is valid, process the sentence.
        /// </summary>
        private void ProcessDataThread( )
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

                    // Ensure the data can contain NMEA sentences
                    while (_buffer.Length > 0 && _buffer.Contains("$") && _buffer.Contains("*"))
                    {

                        // Find a sentence in the buffer
                        string nmea = FindSentence();

                        // If one is found, process the data
                        if (nmea.Length > 0)
                        {
                            // Create a NMEA sentence and verify its valid
                            NmeaSentence sentence = new NmeaSentence(nmea);
                            if (sentence.IsValid)
                            {
                                // Process sentence
                                ProcessSentence(sentence);
                            }
                        }

                        // If wakeup was called to kill thread
                        if (!_continue)
                        {
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
        /// Find a NMEA style sentence in 
        /// the buffer.  If one is found return
        /// it as a string.
        /// </summary>
        /// <returns>Sentence found in buffer.</returns>
        private string FindSentence()
        {
            string nmea = "";
            lock (_bufferLock)
            {
                // Remove everything to first $
                int begin = _buffer.IndexOf("$");
                if (begin > 0)
                {
                    _buffer = _buffer.Remove(0, begin);
                }
                else if(begin < 0)
                {
                    // No NMEA messages in the buffer
                    return nmea;
                }

                // Check if a checksum exist in the data
                // If so, being parsing
                int checksumLoc = _buffer.IndexOf("*");

                // Find the start of the checksum
                // Add NMEA_CHECKSUM_SIZE to include the * and checksum value
                if (checksumLoc >= 0 && _buffer.Length >= checksumLoc + NMEA_CHECKSUM_SIZE)
                {
                    // Check if the checksum is good
                    if (!_buffer.Substring(checksumLoc, NMEA_CHECKSUM_SIZE).Contains("$"))
                    {
                        // Get the NMEA string and remove it from the buffer.
                        nmea = _buffer.Substring(0, checksumLoc + NMEA_CHECKSUM_SIZE);
                        _buffer = _buffer.Remove(0, nmea.Length);

                        // Remove any trailing new lines
                        //nmea = nmea.TrimEnd(REMOVE_END);
                    }
                    else
                    {
                        // Bad Nmea string, $ within checksum
                        // Remove the bad string
                        _buffer = _buffer.Remove(0, checksumLoc);
                    }
                }
                else
                {
                    // Ensure buffer does not overflow
                    if (_buffer.Length > MAX_BUFFER_SIZE)
                    {
                        lock (_bufferLock)
                        {
                            _buffer = _buffer.Substring(_buffer.Length - MAX_BUFFER_SIZE, MAX_BUFFER_SIZE);
                        }
                    }
                }
            }

            return nmea;
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
        /// Process the valid NMEA sentence.  Check which 
        /// type of DVL message it is and create the data set.
        /// Then send the data set to the CurrentDataSetManager.
        /// </summary>
        /// <param name="sentence">Sentence to process.</param>
        private void ProcessSentence(NmeaSentence sentence)
        {
            // Check for PRTI01
            if (sentence.CommandWord.EndsWith(RTI.Prti01Sentence.CMD_WORD_PRTI01, StringComparison.Ordinal))
            {
                // Check if the previous PRTI01 was used
                // If it was not used, send the data
                if (_prti01 != null)
                {
                    SendData(_prti01);
                    _prti01 = null;
                }

                // Store the sentence to be combined with PRTI02
                _prti01 = new Prti01Sentence(sentence.Sentence, sentence.CommandWord, sentence.Words, sentence.ExistingChecksum);

            }
            // Check for PRTI02
            else if (sentence.CommandWord.EndsWith(RTI.Prti02Sentence.CMD_WORD_PRTI02, StringComparison.Ordinal))
            {
                Prti02Sentence prti02 = new Prti02Sentence(sentence.Sentence, sentence.CommandWord, sentence.Words, sentence.ExistingChecksum);

                // Check if the PRTI01 and PRTI02 match
                // If they match, combine and send
                // If they do not match, send PRTI02
                if (_prti01 != null && _prti01.SampleNumber == prti02.SampleNumber)
                {
                    SendData(_prti01, prti02, _prti30, _prti31);
                    _prti01 = null;
                }
                else
                {
                    SendData(prti02);
                }
            }
            // Check for PRTI03
            else if (sentence.CommandWord.EndsWith(RTI.Prti03Sentence.CMD_WORD_PRTI03, StringComparison.Ordinal))
            {
                // Store the sentence to be combined with PRTI02
                Prti03Sentence prti03 = new Prti03Sentence(sentence.Sentence, sentence.CommandWord, sentence.Words, sentence.ExistingChecksum);
                SendData(prti03);
            }
            // Check for PRTI30
            else if (sentence.CommandWord.EndsWith(RTI.Prti30Sentence.CMD_WORD_PRTI30, StringComparison.Ordinal))
            {
                // Store the sentence to be combined with PRTI01 and PRTI02
                _prti30 = new Prti30Sentence(sentence.Sentence, sentence.CommandWord, sentence.Words, sentence.ExistingChecksum);
            }
            // Check for PRTI31
            else if (sentence.CommandWord.EndsWith(RTI.Prti31Sentence.CMD_WORD_PRTI31, StringComparison.Ordinal))
            {
                // Store the sentence to be combined with PRTI01 and PRTI02
                _prti31 = new Prti31Sentence(sentence.Sentence, sentence.CommandWord, sentence.Words, sentence.ExistingChecksum);
            }
            else
            {
                // If the data was nether PRTI01 or PRTI02, then it must be GPS NMEA data and add it 
                // to the NMEA buffer to be processed with a complete dataset
                AddNmeaData(sentence.Sentence + NMEA_END);
            }

            // Reset the incoming data timeout
            _incomingDataTimeout = 0;
        }

        #endregion

        #region Create DataSets

        /// <summary>
        /// Create a dataset.  Set the bottom track instrument velocity and water mass velocity.
        /// </summary>
        /// <param name="sentence">Sentence containing DVL data.</param>
        /// <returns>Dataset with values set.</returns>
        private DataSet.Ensemble CreateDataSet(Prti01Sentence sentence)
        {
            DataSet.Ensemble adcpData = new DataSet.Ensemble();

            // Add the Ensemble number to EnsembleDataSet
            adcpData.AddEnsembleData(sentence);

            // Add the Temp to AncillaryDataSet
            adcpData.AddAncillaryData(sentence);

            // Add Bottom Track data
            adcpData.AddBottomTrackData(sentence);

            // Add Water Mass data
            adcpData.AddInstrumentWaterMassData(sentence);

            return adcpData;
        }

        /// <summary>
        /// Create a dataset.  Set the bottom track Earth velocity and water mass velocity.
        /// </summary>
        /// <param name="sentence">Sentence containing DVL data.</param>
        /// <returns>Dataset with values set.</returns>
        private DataSet.Ensemble CreateDataSet(Prti02Sentence sentence)
        {
            DataSet.Ensemble adcpData = new DataSet.Ensemble();

            // Add the Ensemble number to EnsembleDataSet
            adcpData.AddEnsembleData(sentence);

            // Add the Temp to AncillaryDataSet
            adcpData.AddAncillaryData(sentence);

            // Add Bottom Track data
            adcpData.AddBottomTrackData(sentence);

            // Add Water Mass data
            adcpData.AddEarthWaterMassData(sentence);

            return adcpData;
        }

        /// <summary>
        /// Create a dataset.  Set the bottom track instrument velocity and water mass velocity.
        /// </summary>
        /// <param name="sentence">Sentence containing DVL data.</param>
        /// <returns>Dataset with values set.</returns>
        private DataSet.Ensemble CreateDataSet(Prti03Sentence sentence)
        {
            DataSet.Ensemble adcpData = new DataSet.Ensemble();

            // Add the Ensemble number to EnsembleDataSet
            adcpData.AddEnsembleData(sentence);

            // Add the Temp to AncillaryDataSet
            adcpData.AddAncillaryData(sentence);

            // Add Bottom Track data
            adcpData.AddBottomTrackData(sentence);

            // Add Water Mass data
            adcpData.AddInstrumentWaterMassData(sentence);

            return adcpData;
        }

        #endregion

        #region Nmea Buffer

        /// <summary>
        /// Get the latest data from the NMEA buffer
        /// and create a string.  
        /// Then record the NMEA data and add it to
        /// the ensemble.
        /// </summary>
        /// <param name="adcpData">Ensemble data.</param>
        /// <returns>Binary data.</returns>
        private byte[] GetNmeaData(ref DataSet.Ensemble adcpData)
        {
            // Byte array of the binary data
            List<byte> byteList = new List<byte>();
            
            string nmeaData = GetNmeaBuffer();
            if (nmeaData.Length > 0)
            {
                // Add the NMEA data to the dataset
                adcpData.AddNmeaData(nmeaData.ToString());

                // Create byte array for all the NMEA data accumulated
                // Record the NMEA data to the file
                // Get the NMEA data from the dataset to ensure valid NMEA messages
                for (int x = 0; x < adcpData.NmeaData.NmeaStrings.Count; x++)
                {
                    string nmeaStr = adcpData.NmeaData.NmeaStrings[x] + NMEA_END;
                    byte[] nmeaBA = System.Text.Encoding.ASCII.GetBytes(nmeaStr);
                    byteList.AddRange(nmeaBA);
                }
            }

            // Return the binary data
            return byteList.ToArray();
        }

        /// <summary>
        /// Take all the data from the NMEA buffer
        /// and add it to a string.  Return the string.
        /// If nothing is in the string, return an
        /// empty string.
        /// </summary>
        /// <returns>String of data from NMEA buffer.</returns>
        private string GetNmeaBuffer()
        {
            lock (_nmeaBufferLock)
            {
                if (_nmeaBuffer.Count > 0)
                {
                    // Copy the data from the buffer
                    // This will take a current cout of the buffer.
                    // Then create a string of the buffer and remove
                    // the item from the buffer at the same time.
                    StringBuilder nmeaData = new StringBuilder();
                    for (int x = 0; x < _nmeaBuffer.Count; x++)
                    {
                        nmeaData.Append(_nmeaBuffer.First.Value);

                        // Remove the data
                        _nmeaBuffer.RemoveFirst();
                    }

                    return nmeaData.ToString();
                }
            }

            return "";
        }

        #endregion

        #region Events

        /// <summary>
        /// Event To subscribe to.  This gives the paramater
        /// that will be passed when subscribing to the event.
        /// </summary>
        /// <param name="binaryData">Binary data.</param>
        /// <param name="adcpData">DataSet to record and process.</param>
        public delegate void ProcessDataEventHandler(byte[] binaryData, DataSet.Ensemble adcpData);

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
