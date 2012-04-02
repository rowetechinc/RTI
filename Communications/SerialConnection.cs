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
 * 09/01/2011      RC                     Initial coding
 * 10/13/2011      RC                     Removed SendBuffer
 *                                         Added an event for receive data
 *                                         Removed NotificationObject
 * 10/14/2011      RC                     Added SendData that takes byte array.
 * 10/17/2011      RC                     Fixed issue with thread stopping and serial port
 *                                         continue to read and cause crash. http://stackoverflow.com/questions/1319003/safe-handle-has-been-closed-with-serialport-and-a-thread-in-c-sharp
 * 10/19/2011      RC                     Added ManualResetEvent to fix bug like website above. 
 * 10/20/2011      RC                     Check for a port before connecting.
 * 11/15/2011      RC                     On exceptions report to error log.
 * 11/17/2011      RC                     Change event handler for received data.
 * 11/18/2011      RC                     Added SendDataWaitReply() and WaitForACK().  Fix RS-485 issue waiting for resonse.
 * 12/29/2011      RC          1.11       Adding log and removing RecorderManager.
 * 02/06/2012      RC          2.00       Improve the performance of the ReceiveBufferString when limiting the size.
 * 02/10/2012      RC          2.02       Made serial port and serial options PROTECTED so class that extend can use it.
 *                                         Added BREAK command.
 * 03/23/2012      RC          2.07       Added a wait in the Break command to ensure the state has time to change.
 * 03/30/2012      RC          2.07       Check for a break state when sending data.
 * 
 */


using System;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;
using log4net;

namespace RTI
{
    /// <summary>
    /// This class will create a serial port and allow data to
    /// be sent and received to and from the serial port.  
    /// 
    /// An abstract method must be implemented to receive
    /// data from the serial port.  
    /// 
    /// To send data to the serial port, the sendBuffer must
    /// be filled and then the SendDataCommand must be given.
    /// </summary>
    public abstract class SerialConnection
    {
        // Setup logger
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Private variables for this class
        #region Variables

        /// <summary>
        /// ACK for receiving messages.
        /// </summary>
        private byte ACK = 6;

        /// <summary>
        /// Max to set to observable collection.
        /// </summary>
        private int MAX_DISPLAY_BUFFER = 3000;

        /// <summary>
        /// Serial port connection.
        /// </summary>
        protected SerialPort _serialPort;

        /// <summary>
        /// Serial port options.
        /// </summary>
        protected SerialOptions _serialOptions;
        
        /// <summary>
        /// Used to determine if the applicaiton is shutting down
        /// to properly stop the serial port.
        /// </summary>
        protected ManualResetEvent threadStop = new ManualResetEvent(false);

        /// <summary>
        /// Wait for ACK when sending a command to the serial port.
        /// </summary>
        private EventWaitHandle _eventWaitResponse;

        /// <summary>
        /// String used to validate the correct message
        /// was echoed back when sending data and waiting
        /// for a response.
        /// </summary>
        private string _validateMsg;

        #endregion

        // Abstract methods for this class
        #region Abstract
        /// <summary>
        /// Abstract method that must be implemented by all classes
        /// that extended this class.  
        /// 
        /// This method will pass the incoming data.  The data
        /// can then be handled anyway by the class that extends
        /// this class.
        /// 
        /// This will pass a s of string from the serial port.  The
        /// data will not be passed to this method until and end-of-s
        /// is read by the serial port.
        /// 
        /// THIS METHOD MUST RETURN IMMEDIATELY.
        /// </summary>
        /// <param name="buffer">Data received from the serial port.</param>
        //protected abstract void ReceiveDataHandler(string rcvData);
        protected abstract void ReceiveDataHandler(byte[] buffer);

        #endregion

        // Properties for the class
        #region Properties

        /// <summary>
        /// Store the receive buffer as a string.
        /// This will keep in tact all the new lines.
        /// 
        /// Ensure the buffer size does not get to large.
        /// </summary>
        private string _receiveBufferString;
        /// <summary>
        /// Store the receive buffer as a string.
        /// This will keep in tact all the new lines.
        /// 
        /// Ensure the buffer size does not get to large.
        /// </summary>
        public string ReceiveBufferString
        {
            get { return _receiveBufferString; }
            set
            {
                _receiveBufferString = value;

                // Then clear some of the buffer
                if (_receiveBufferString.Length > MAX_DISPLAY_BUFFER)
                {
                    _receiveBufferString = _receiveBufferString.Substring(0, MAX_DISPLAY_BUFFER);
                }
            }
        }

        #endregion

        /// <summary>
        /// Initialize values.
        /// </summary>
        /// <param name="serialOptions"></param>
        public SerialConnection(SerialOptions serialOptions)
        {
            // Initialize ranges
            _serialOptions = serialOptions;                         // Serial port options
            _serialPort = new SerialPort();                         // Serial port

            _validateMsg = "";
        }

        // Method calls for the class
        #region Methods
        /// <summary>
        /// Disconnect the current connection,
        /// then connect the serial port.
        /// </summary>
        public void Reconnect()
        {
            Disconnect();
            Connect();
            Debug.WriteLine("Reconnect Serial port: " + _serialOptions.Port);
        }

        /// <summary>
        /// Connect to the serial port.
        /// </summary>
        public void Connect()
        {
            // Make sure serial port is set
            if (!string.IsNullOrEmpty(_serialOptions.Port))
            {
                _serialPort = new SerialPort(_serialOptions.Port);
                _serialPort.BaudRate = _serialOptions.BaudRate;     // Set baud rate
                _serialPort.DataBits = _serialOptions.DataBits;     // Set Data bits
                _serialPort.Parity = _serialOptions.Parity;         // Set Parity
                _serialPort.StopBits = _serialOptions.StopBits;     // Set stop bits
                _serialPort.Handshake = Handshake.None;             // No handshake (Flow Control None)
                ReceiveBufferString = "";                           // Clear the buffer

                // Event handler to recieve data
                _serialPort.DataReceived += new SerialDataReceivedEventHandler(ReadEventHandler);

                if (!_serialPort.IsOpen)
                {
                    // Open the serial port and remove old data
                    try
                    {
                        _serialPort.Open();

                        // Discard any old data in buffer
                        _serialPort.DiscardInBuffer();
                        Debug.WriteLine(_serialOptions.ToString());
                    }
                    catch (System.UnauthorizedAccessException ex)
                    {
                        // Port is already in use
                        log.Warn("COMM Port already in use: " + _serialOptions.Port, ex);
                        //RecorderManager.Instance.ReportError("COMM Port already in use: " + _serialOptions.Port, ex);
                        //MessageBox.Show(string.Format("Port {0} already in use.", _serialOptions.Port), "Communication Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (System.ArgumentOutOfRangeException ex_range)
                    {
                        // Not sure what is causing this exception yet
                        log.Error("Error COMM Port: " + _serialOptions.Port, ex_range);
                        //RecorderManager.Instance.ReportError("Error COMM Port: " + _serialOptions.Port, ex_range);
                    }
                    catch (Exception ex)
                    {
                        log.Error("Error Opening in COMM Port: " + _serialOptions.Port, ex);
                        //RecorderManager.Instance.ReportError("Error Opening in COMM Port: " + _serialOptions.Port, ex);
                    }
                }
            }
        }

        /// <summary>
        /// Disconnect serial port.
        /// </summary>
        public void Disconnect()
        {
            _serialPort.Close();
            _serialPort.Dispose();
        }

        /// <summary>
        /// Send a break on the serial port.
        /// This will take at least 100ms to complete.
        /// The serial port turns on the break state, waits
        /// 100ms then turns off the break state.
        /// </summary>
        public void SendBreak()
        {
            if (_serialPort.IsOpen)
            {
                // Send a break to the serial port
                _serialPort.BreakState = true;

                // Wait for the state to change
                // and leave on a bit of time
                System.Threading.Thread.Sleep(100);

                // Change state back
                _serialPort.BreakState = false;

                // Wait for state to change back
                System.Threading.Thread.Sleep(100);
            }
        }

        /// <summary>
        /// This is an event handler for the serial port
        /// to receive data.  When data becomes available,
        /// the event handler will be called.  It will then 
        /// pass data to the ReceiveEventHandler which is 
        /// abstract.  
        /// 
        /// ReceiveDataHandler is an abstract method that
        /// all classes that extend this class must implement.
        /// The incoming data will be passed to this method.
        /// This method must return immediately after receiving
        /// the data.
        /// </summary>
        protected void ReadEventHandler(object s, SerialDataReceivedEventArgs e)
        {
            // Open the serial port and remove old data
            try
            {
                byte[] buffer = new byte[_serialPort.BytesToRead];
                _serialPort.Read(buffer, 0, buffer.Length);

                // Pass the data to all subscribers
                if (this.ReceiveRawSerialDataEvent != null)
                {
                    this.ReceiveRawSerialDataEvent(buffer);
                }

                // Pass the data to a function that can be overloaded
                // so the data can be handled differently
                ReceiveDataHandler(buffer);

                // WaitOne(0) tests whether the event was set and returns TRUE
                //   if it was set and FALSE otherwise.
                // The 0 tells the manual reset event to only check if it was set
                //   and return immediately, otherwise if the number is greater than
                //   0 it will wait for that many milliseconds for the event to be set
                //   and only then return - effectively blocking your thread for that
                //   period of time
                if (threadStop.WaitOne(0))
                {
                    Disconnect();
                }
            }
            catch (System.UnauthorizedAccessException ex)
            {
                // If the port is already in use, do not
                // start the thread
                Debug.WriteLine("COMM Port Error Reading: " + _serialOptions.Port, ex);

                if (_serialPort != null)
                {
                    Disconnect();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error Reading in COMM Port: " + _serialOptions.Port, ex);

                if (_serialPort != null)
                {
                    Disconnect();
                }
            }
        }

        /// <summary>
        /// Add the new data to the recieve buffer.  
        /// Call all subscribers that new data arrived.
        /// </summary>
        /// <param name="data">String to add to the collection.</param>
        protected void SetReceiveBuffer(string data)
        {
            //Debug.Write(data);
            // Add data to the buffer
            ReceiveBufferString = data + ReceiveBufferString;

            // Set the string received so if trying to
            // validate a message sent, this can check the response.
            _validateMsg = data;

            // Notify any subscribers that new data has arrived
            if (ReceiveSerialData != null)
            {
                // Call the Event
                ReceiveSerialData(data);
            }
        }

        /// <summary>
        /// Send a command to the serial port.
        /// This will add the carrage return to
        /// the end of the string.
        /// </summary>
        /// <param name="data">Data to write.</param>
        public void SendData(string data)
        {
            if (_serialPort.IsOpen && !String.IsNullOrEmpty(data) && !_serialPort.BreakState)
            {
                // Format the command
                data += '\r';

                _serialPort.Write(data);
            }
        }

        /// <summary>
        /// Send a byte array to the serial port.
        /// This will not add the carrage return to the
        /// end of the data.
        /// </summary>
        /// <param name="data">Data to write.</param>
        /// <param name="offset">Where to start in array.</param>
        /// <param name="count">Number of bytes to write.</param>
        public void SendData(byte[] data, int offset, int count)
        {
            if (_serialPort.IsOpen && (data != null) && !_serialPort.BreakState)
            {
                _serialPort.Write(data, offset, count);
            }
        }

        /// <summary>
        /// Send data through the serial port and wait for a response
        /// back from the serial port.  If the response was the same
        /// as the message sent, then return true.
        /// </summary>
        /// <param name="data">Data to send through the serial port.</param>
        /// <param name="timeout">Timeout to wait in milliseconds.</param>
        /// <return>Flag if was successful sending the command.</return>
        public bool SendDataWaitReply(string data, int timeout)
        {
            bool result = true;

            // Reset the validate message
            _validateMsg = "";

            // Setup the wait and initialize it off and make it manually reset
            _eventWaitResponse = new EventWaitHandle(false, EventResetMode.ManualReset);

            // Subscribe to receive serial data
            // When the data is received, this method will wake up to complete
            this.ReceiveSerialData += new SerialConnection.ReceiveSerialDataEventHandler(WaitForAck);

            // Send command
            this.SendData(data);
           
            // Wait for ACK
            _eventWaitResponse.WaitOne(timeout);

            // Verify message was received and handled properly by
            // checking what was echoed back.
            if (_validateMsg.Length >= data.Length)
            {
                // Compare sent vs what was echoed back
                if (data.CompareTo(_validateMsg.Substring(0, data.Length)) != 0)
                {
                    log.Warn(string.Format("Error sending data to serial port. Sent data: {0} length: {1}  Received Data: {2} length: {3}", data, data.Length, _validateMsg, _validateMsg.Length));
                    result = false;
                }
            }
            // string lengths did not match
            else
            {
                log.Warn(string.Format("Error sending data to serial port. Sent data: {0} length: {1}  Received Data: {2} length: {3}", data, data.Length, _validateMsg, _validateMsg.Length));
                result = false;
            }

            // ACK occurs when command echoed back with ACK at the end
            // Unsubscribe from receiving an event
            this.ReceiveSerialData -= WaitForAck;

            return result;
        }

        /// <summary>
        /// Use this method to recieve a message from the serial port
        /// after sending a message to the serial port.
        /// We are looking for the response to our command sent which should
        /// be a echo of the command with an ACK.
        /// </summary>
        /// <param name="data"></param>
        private void WaitForAck(string data)
        {
            // Convert to byte array
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(data);

            // Look for ACK
            for (int x = 0; x < bytes.Length; x++)
            {
                if (bytes[x] == ACK)
                {
                    _eventWaitResponse.Set();
                }
            }

                
        }


        #endregion


        #region Events

        /// <summary>
        /// Event To subscribe to.  This gives the paramater
        /// that will be passed when subscribing to the event.
        /// </summary>
        /// <param data="ensemble">Byte array of data received from the port.</param>
        public delegate void ReceiveRawSerialDataEventHandler(byte[] data);

        /// <summary>
        /// Subscribe to receive event when data has been successfully
        /// processed.  This can be used to tell if data is in this format
        /// and is being processed or is not in this format.
        /// Subscribe to this event.  This will hold all subscribers.
        /// 
        /// To subscribe:
        /// serialConnection.ReceiveRawSerialDataEvent += new serialConnection.ReceiveRawSerialDataEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// serialConnection.ReceiveRawSerialDataEvent -= (method to call)
        /// </summary>
        public event ReceiveRawSerialDataEventHandler ReceiveRawSerialDataEvent;

        /// <summary>
        /// Method to call when subscribing to an event.
        /// </summary>
        /// <param name="data">Data received from the serial port.</param>
        public delegate void ReceiveSerialDataEventHandler(string data);

        /// <summary>
        /// Event to subscribe to.
        /// To subscribe:
        /// _serialConnection.ReceiveSerialData += new SerialConnection.ReceiveSerialDataEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// _serialConnection.ReceiveSerialData -= (method to call)
        /// </summary>
        public event ReceiveSerialDataEventHandler ReceiveSerialData;

        #endregion
    }
}