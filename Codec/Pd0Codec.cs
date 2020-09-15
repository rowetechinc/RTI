﻿/*
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
 * 03/27/2014      RC          2.21.4     Initial coding
 * 09/17/2014      RC          3.0.1      Abort and Join the processing thread on StopThread.
 * 10/28/2014      RC          3.0.2      Fixed bug to check if the buffer is empty.
 * 01/16/2015      RC          3.0.2      Lock the buffer in VerifyHeaderStart().
 * 06/18/2015      RC          3.0.5      Ensure the codec is shutdown properly.
 * 07/09/2015      RC          3.0.5      Mode the codec a thread.
 * 08/13/2015      RC          3.0.5      Added complete event.
 * 04/27/2017      RC          3.4.2      Check for buffer overflow with _incomingDataTimeout.
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Parse the PD0 codec.
    /// </summary>
    public class Pd0Codec : ICodec, IDisposable
    {
        #region Variable

        /// <summary>
        /// Setup logger to report errors.
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Timeout of a loop.
        /// </summary>
        private const int TIMEOUT_MAX = 20;

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

        /// <summary>
        /// Decode the PD0 binary data.
        /// </summary>
        public Pd0Codec()
        {
            // Initialize buffer
            //_incomingDataQueue = new Queue<byte[]>();
            _incomingDataBuffer = new List<Byte>();
            _incomingDataTimeout = 0;

            // Initialize the thread
            _continue = true;
            _eventWaitData = new EventWaitHandle(false, EventResetMode.AutoReset);
            _processDataThread = new Thread(ProcessDataThread);
            _processDataThread.Name = "PD0 Codec";
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
            if (!_eventWaitData.SafeWaitHandle.IsClosed)
            {
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
            // Lock the buffer while clearing
            lock (_bufferLock)
            {
                // Clear the buffer
                _incomingDataBuffer.Clear();
            }
        }

        /// <summary>
        /// Shutdown this object.
        /// </summary>
        public void Dispose()
        {
            StopThread();
        }

        /// <summary>
        /// Stop the thread.
        /// </summary>
        private void StopThread()
        {
            _continue = false;

            // Wake up the thread to stop thread
            if (!_eventWaitData.SafeWaitHandle.IsClosed)
            {
                _eventWaitData.Set();
            }

            // Force the thread to stop
            //_processDataThread.Abort();
            _processDataThread.Join(1000);
        }

        #region Process Data

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
                    _eventWaitData.WaitOne(10000);

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
                catch (ThreadAbortException)
                {
                    // Thread is aborted to stop processing
                    return;
                }
                catch (Exception e)
                {
                    log.Error("Error processing PD0 codec data.", e);
                    return;
                }

                // Send an event that processing is complete
                if (ProcessDataCompleteEvent != null)
                {
                    ProcessDataCompleteEvent();
                }

            }
        }

        #endregion

        #region Decode Data

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
            // It will contain 2 0x7F at the start
            SearchForHeaderStart();

            // Verify the incoming data can at least fit the header
            if (_incomingDataBuffer.Count > Pd0Header.HEADER_MIN_BYTE)
            {
                // If found 2 bytes of 0x7F
                // Continue forward
                // Find ensemble number and ensemble size
                if (VerifyHeaderStart())
                {
                    // Get Ensemble size
                    int currentEnsembleSize = GetEnsembleSize();

                    // Ensure the entire ensemble is present
                    // before proceeding
                    if (_incomingDataBuffer.Count >= currentEnsembleSize && currentEnsembleSize > 0)
                    {
                        // Create an array to hold the ensemble
                        byte[] ensemble = new byte[currentEnsembleSize];

                        // Lock the buffer, to ensure the same data is copied
                        lock (_bufferLock)
                        {
                            // Check one more time just in case the buffer was modified
                            if (_incomingDataBuffer.Count >= currentEnsembleSize)
                            {
                                // Copy the ensemble to a byte array
                                _incomingDataBuffer.CopyTo(0, ensemble, 0, currentEnsembleSize);

                                // Remove ensemble from buffer
                                _incomingDataBuffer.RemoveRange(0, currentEnsembleSize);
                            }
                        }

                        // Get the checksum values
                        ushort calculatedChecksum = PD0.CalculateChecksum(ensemble, currentEnsembleSize - PD0.CHECKSUM_NUM_BYTE);
                        ushort ensembleChecksum = RetrieveEnsembleChecksum(ensemble);

                        // Verify the checksum match
                        if (calculatedChecksum == ensembleChecksum)
                        {
                            // Decode the binary data
                            DecodePd0Data(ensemble);

                            // Pass event that a good ensemble was found
                            if (GoodEnsembleEvent != null)
                            {
                                GoodEnsembleEvent();
                            }
                        }
                        else
                        {
                            // Pass event that a good ensemble was found
                            if (BadEnsembleEvent != null)
                            {
                                BadEnsembleEvent();
                            }
                        }
                    }
                }
                else
                {
                    // Lock the buffer while removing
                    lock (_bufferLock)
                    {
                        if (_incomingDataBuffer.Count > 0)
                        {
                            // Remove the first element to continue searching
                            _incomingDataBuffer.RemoveAt(0);
                        }
                    }
                }

            }

        }

        /// <summary>
        /// Convert the given binary data into an ensemble.
        /// Then publish the data to all subscribers.
        /// </summary>
        /// <param name="binaryEnsemble">Binary PD0 data.</param>
        public void DecodePd0Data(byte[] binaryEnsemble)
        {
            // Create the PD0 and RTI ensemble
            PD0 pd0Ensemble = new PD0(binaryEnsemble);

            // Send an event that data was processed
            // in this format
            if (ProcessDataEvent != null)
            {
                ProcessDataEvent(binaryEnsemble, pd0Ensemble);
            }

            // Reset the timeout
            _incomingDataTimeout = 0;
        }

        /// <summary>
        /// Search for the beginning of the header.
        /// The header will contain 7F 7F
        /// at the beginning.  Try to find the 
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
                    // Check if shutting down
                    if(!_continue)
                    {
                        return;
                    }

                    // Find a start
                    if (_incomingDataBuffer[0] == 0x7F && _incomingDataBuffer[1] == 0x7F)
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
        /// Verify the PD0 header was found.
        /// This will look for the two 0x7F.
        /// </summary>
        /// <returns>TRUE = Header found.</returns>
        private bool VerifyHeaderStart()
        {
            byte buff0 = 0x0;
            byte buff1 = 0x0;

            // Lock the buffer and get the values
            lock (_bufferLock)
            {
                if (_incomingDataBuffer.Count >= 2)
                {
                    buff0 = _incomingDataBuffer[0];
                    buff1 = _incomingDataBuffer[1];
                }
            }

            // Find a start
            if (buff0 == 0x7F && buff1 == 0x7F)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Return the number of bytes in the ensemble.
        /// This is found in the header of the ensemble in bytes 3 and 4.
        /// This will assume the Header ID has already been found.
        /// Add 2 to the end to account for the checksum at the end.
        /// </summary>
        /// <returns>The ensemble size found in the header.</returns>
        private int GetEnsembleSize()
        {
            if (_incomingDataBuffer.Count >= 4)
            {
                return MathHelper.LsbMsbInt(_incomingDataBuffer[2], _incomingDataBuffer[3]) + PD0.CHECKSUM_NUM_BYTE;
            }
            return 0;
        }

        /// <summary>
        /// Return the checksum from the ensemble.  This will
        /// get the last two bytes and convert them to a ushort.
        /// </summary>
        /// <param name="ensemble">Ensemble to get the checksum.</param>
        /// <returns>Checksum value from the ensemble.</returns>
        private ushort RetrieveEnsembleChecksum(byte[] ensemble)
        {
            return MathHelper.LsbMsbUShort(ensemble[ensemble.Length - 2], ensemble[ensemble.Length - 1]);
        }

        #endregion

        #region Events

        /// <summary>
        /// Event To subscribe to.  This gives the paramater
        /// that will be passed when subscribing to the event.
        /// </summary>
        /// <param name="ensemble">Byte array of raw ensemble data.</param>
        /// <param name="pd0Ensemble">PD0 ensemble data as an object.</param>
        public delegate void ProcessDataEventHandler(byte[] ensemble, PD0 pd0Ensemble);

        /// <summary>
        /// Subscribe to receive event when data has been successfully
        /// processed.  This can be used to tell if data is in this format
        /// and is being processed or is not in this format.
        /// Subscribe to this event.  This will hold all subscribers.
        /// 
        /// To subscribe:
        /// pd0Codec.ProcessDataEvent += new pd0Codec.ProcessDataEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// pd0Codec.ProcessDataEvent -= (method to call)
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

        /// <summary>
        /// Event To subscribe to.  This gives the paramater
        /// that will be passed when subscribing to the event.
        /// </summary>
        public delegate void GoodEnsembleEventHandler();

        /// <summary>
        /// Subscribe to know when a good ensemble has been found.
        /// 
        /// To subscribe:
        /// codec.GoodEnsembleEvent += new codec.GoodEnsembleEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// codec.GoodEnsembleEvent -= (method to call)
        /// </summary>
        public event GoodEnsembleEventHandler GoodEnsembleEvent;

        /// <summary>
        /// Event To subscribe to.  This gives the paramater
        /// that will be passed when subscribing to the event.
        /// </summary>
        public delegate void BadEnsembleEventHandler();

        /// <summary>
        /// Subscribe to know when a bad ensemble has been found
        /// 
        /// To subscribe:
        /// codec.BadEnsembleEvent += new codec.BadEnsembleEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// codec.BadEnsembleEvent -= (method to call)
        /// </summary>
        public event BadEnsembleEventHandler BadEnsembleEvent;

        #endregion

    }
}
