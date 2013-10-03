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
 * 04/06/2012      RC          2.08       Added WAIT_STATE.
 *                                         Made the serial port private to protect against sending and receiving data incorrectly.
 *                                         Added IsOpen(), IsBreakState() and IsAvailable() to check status of serial port.
 *                                         Use a thread to read the data instead of an event handler.
 *                                         Added a shutdown method that must be called when destorying the object to stop the thread.
 * 04/12/2012      RC          2.09       Added ReadData() to manually read. 
 *                                         Added PauseReadThread() to pause reading data from the serial port.
 * 04/26/2012      RC          2.10       Added a default value to SendDataAndWaitReply().
 * 04/27/2012      RC          2.10       Added a sleep in the SendData() commands.
 * 07/09/2012      RC          2.12       Added locks to read and write methods for multithreading.
 * 08/21/2012      RC          2.12       Used IsAvailable() instead of checking each time for isOpen and !breakState when using the serial port.
 *                                         Made Connect() and Reconnect return a bool if a connection could be made.
 * 08/31/2012      RC          2.12       Changed the serial port ReadBufferSize to handle faster download speeds.
 * 09/21/2012      RC          2.15       Added SendDataGetReply() to send data and get the response back.
 * 10/18/2012      RC          2.15       In SendDataWaitReply(), when checking if the reponse matches the command, make the command and response both lower case so they will match in case.
 * 10/25/2012      RC          2.16       Added a check for IO exception in Connect(), to see if the port exist.
 * 04/26/2013      RC          2.19       Fixed IsAvailable() if the port is not open, it would get a null for BreakState.
 *                                         Set the initial port and baud rate given in the constuctor to the serial port.
 * 05/03/2013      RC          2.19       Fixed connecting multiple times to a serial port.  Threads were left running when multiple connections were attempted.
 * 05/15/2013      RC          2.19       Make a public property for SerialPortOptions to allow others to see the settings.
 * 05/17/2013      RC          2.19       Added ToString().
 * 05/23/2013      RC          2.19       Check for empty or null string in SendDataWaitReply().
 *                                         MAX_DISPLAY_BUFFER increased to ensure the entire HELP message can be displayed.
 * 06/28/2013      RC          2.19       Replaced Shutdown() with IDisposable.
 * 08/09/2013      RC          2.19.4     Added a soft BREAK in SendBreak() for connections that can not handle hard BREAKs.
 * 09/25/2013      RC          2.20.1     In SendDataWaitReply() check the response if it is a BREAK and handle differently.
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
    public abstract class SerialConnection: IDisposable
    {
        // Setup logger
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Variables

        /// <summary>
        /// Default period to wait before checking for data to read.
        /// 
        /// 4Hz
        /// </summary>
        protected const int DEFAULT_SERIAL_INTERVAL = 250;

        /// <summary>
        /// Period to wait when downloading data.  This value is smaller
        /// to ensure a quick download.
        /// </summary>
        protected const int DOWNLOAD_SERIAL_INTERVAL = 25;

        /// <summary>
        /// A wait state used to wait between sending a command and
        /// expecting a result back in the buffer.  Since the data is
        /// not read continuously from the serial port, use must wait a
        /// period of time between sending a command and expecting a result.
        /// </summary>
        public const int WAIT_STATE = DEFAULT_SERIAL_INTERVAL;    // Time to wait between sending a message and getting a response

        /// <summary>
        /// ACK for receiving messages.
        /// </summary>
        private byte ACK = 6;

        /// <summary>
        /// Max to set to observable collection.
        /// </summary>
        private int MAX_DISPLAY_BUFFER = 5000;

        /// <summary>
        /// Serial port connection.
        /// </summary>
        private SerialPort _serialPort;

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

        /// <summary>
        /// Used to make the thread sleep between reads.
        /// </summary>
        private EventWaitHandle _threadWait;

        /// <summary>
        /// Thread to read the data from the serial port.
        /// </summary>
        private Thread _readThread;

        /// <summary>
        /// Time period to wait between reads of the serial 
        /// port in the ReadThread.
        /// </summary>
        private int _threadInterval;

        /// <summary>
        /// Flag used to stop the ReadThread.
        /// </summary>
        private volatile bool _stopThread;

        /// <summary>
        /// This is used to pause the processing of the
        /// read thread.  This will cause no processing
        /// to be done in the read thread if set true.
        /// </summary>
        private bool _pauseReadThread;

        /// <summary>
        /// Flag used to determine if we are sending data.
        /// This is to prevent sending and reading data
        /// at the same time.  Priority is given to sending
        /// data.
        /// </summary>
        private bool _isSendingData;

        /// <summary>
        /// Lock for reading from the serial port.
        /// </summary>
        private object _readLock = new object();

        /// <summary>
        /// Lock for writing to the serial port.
        /// </summary>
        private object _writeLock = new object();

        #endregion

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

        #region Properties

        /// <summary>
        /// Allow others to see the serial port options.
        /// </summary>
        public SerialOptions SerialOptions { get { return _serialOptions; } }

        /// <summary>
        /// Store the receive buffer as a string.
        /// This will keep in tact all the new lines.
        /// 
        /// Ensure the buffer size does not get to large.
        /// </summary>
        protected string _receiveBufferString;
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
            try
            {
                _serialPort = new SerialPort(serialOptions.Port, serialOptions.BaudRate);                         // Serial port
            }
            catch (Exception)
            {
                // If the port is an old connection or no port is given and
                // that connection cannot be made, then it will throw an
                // Argument exception.  This will then set the variable with no port.
                _serialPort = new SerialPort();
            }

            // Initialize the thread
            _threadWait = new EventWaitHandle(false, EventResetMode.AutoReset);
            _threadInterval = DEFAULT_SERIAL_INTERVAL;
            _stopThread = false;
            _pauseReadThread = false;
            _isSendingData = false;
            _readThread = new Thread(new ParameterizedThreadStart(ReadThreadMethod));
            _readThread.Start();

            _validateMsg = "";
        }

        /// <summary>
        ///  Shutdown the object.
        ///  This must be called when destorying the object to 
        ///  ensure the ReadThread is stopped.
        /// </summary>
        public void Dispose()
        {
            // Set the flag to stop the Read thread
            _stopThread = true;

            // Disconnect the serial port
            Disconnect();

            // Call any subsystem shutdown methods
            // Any objects that extend this object
            // must implement this method to ensure
            // proper shutdown.
            SubDispose();
        }

        /// <summary>
        /// Virtual method to extend the shutdown method.
        /// </summary>
        protected virtual void SubDispose()
        {
            // Put shutdown process in here for any objects that
            // extend this object.
        }

        #region Methods
        /// <summary>
        /// Disconnect the current connection,
        /// then connect the serial port.
        /// </summary>
        /// <returns>TRUE = Connection could be made.  / FALSE = Conneciton could not be made.</returns>
        public bool Reconnect()
        {
            log.Debug("Reconnect Serial port: " + _serialOptions.Port);

            // Disconnect the serial port
            Disconnect();

            // Reconnect the serial port
            return Connect();
        }

        /// <summary>
        /// Connect to the serial port.
        /// This will set the serial port based off the serial options.
        /// It will then try to open the serial port.  If it cannot be opened,
        /// it will get an exception and return false.  The error will be logged.
        /// </summary>
        /// <returns>TRUE = Connection could be made.  / FALSE = Conneciton could not be made.</returns>
        public bool Connect()
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
                _serialPort.ReadBufferSize = 16 * 65536;
                ReceiveBufferString = "";                           // Clear the buffer

                //Set the read/write timeouts
                _serialPort.ReadTimeout = 50;
                _serialPort.WriteTimeout = 500;

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
                        Disconnect();
                        return false;
                    }
                    catch (System.ArgumentOutOfRangeException ex_range)
                    {
                        // Not sure what is causing this exception yet
                        log.Error("Error COMM Port: " + _serialOptions.Port, ex_range);
                        Disconnect();
                        return false;
                    }
                    catch (System.IO.IOException io_ex)
                    {
                        // The Serial port does not exist
                        log.Warn(string.Format("Error COM Port: {0} does not exist.", _serialOptions.Port), io_ex);
                        Disconnect();
                        return false;
                    }
                    catch (Exception ex)
                    {
                        log.Error("Error Opening in COMM Port: " + _serialOptions.Port, ex);
                        Disconnect();
                        return false;
                    }
                }

                // If the serial port is already open, return true
                return true;
            }

            // Serial port could not be opened
            // Either a bad string or a string not given for the port
            Disconnect();
            return false;
        }

        /// <summary>
        /// Disconnect serial port.
        /// </summary>
        public void Disconnect()
        {
            // Close and dispose the serial port
            if (_serialPort != null)
            {
                // Close the port
                _serialPort.Close();

                // Dispose of the port
                _serialPort.Dispose();
            }
        }

        /// <summary>
        /// Check if the serial port is in a break state.
        /// </summary>
        /// <returns>True = Break / False = No Break.</returns>
        public bool IsBreakState()
        {
            if (_serialPort != null)
            {
                return _serialPort.BreakState;
            }

            return false;
        }

        /// <summary>
        /// Check if the serial port is open.
        /// </summary>
        /// <returns>True = Serial Port Open / False = Serial Port is NOT open.</returns>
        public bool IsOpen()
        {
            if (_serialPort != null)
            {
                return _serialPort.IsOpen;
            }

            return false;
        }

        /// <summary>
        /// Check if the serial port is available.
        /// The serial port is available if it is open 
        /// and if it is not in a break state.
        /// 
        /// Make it check each one sequentially so that
        /// if any fail above, the next will not be checked.
        /// If the port is not open, it can not check for a 
        /// break state.
        /// 
        /// </summary>
        /// <returns>TRUE = Serial Port available to send or receive data.</returns>
        public bool IsAvailable()
        {
            // Check if the serial port is null
            if (_serialPort == null)
            {
                return false;
            }

            // Check if the serial port is open
            if (!_serialPort.IsOpen)
            {
                return false;
            }

            // Check if the serial port is in a break state
            if (_serialPort.BreakState)
            {
                return false;
            }

        return true;
        }

        /// <summary>
        /// This is use to pause the read thread.
        /// To pause the read thread, set this true.  To unpause read thread,
        /// set this to false.
        /// 
        /// The read thread will still be running, but it will not read from the serial port
        /// and process data.  It will just go back to sleep.  You can also set the timer interval
        /// higher to reduce the cpu usage of the read thread during this time period.
        /// </summary>
        /// <param name="pause">TRUE = Pause read thread / False = Resume read thread.</param>
        public void PauseReadThread(bool pause = false)
        {
            _pauseReadThread = pause;
        }

        /// <summary>
        /// Send a break on the serial port.
        /// This will take at least 100ms to complete.
        /// The serial port turns on the break state, waits
        /// 100ms then turns off the break state.
        /// </summary>
        public void SendBreak()
        {
            if (IsAvailable())
            {
                // Clear the buffer
                _receiveBufferString = string.Empty;

                // Send a break to the serial port
                _serialPort.BreakState = true;

                // Wait for the state to change
                // and leave on a bit of time
                System.Threading.Thread.Sleep(WAIT_STATE);

                // Change state back
                _serialPort.BreakState = false;

                // Wait for state to change back
                System.Threading.Thread.Sleep(WAIT_STATE);

                // Check if something is in the buffer
                if (string.IsNullOrEmpty(_receiveBufferString))
                {
                    // Send a Text BREAK just incase the instrument cannot take a break
                    // This is the case if using wireless serial communication or
                    // ethernet
                    SendDataWaitReply(Commands.AdcpCommands.CMD_BREAK, 2000);
                }
            }
        }

        /// <summary>
        /// Reset the interval time.  If no value is given,
        /// the default value will be used.  This is the time
        /// in milliseconds to check the serial port for data.
        /// </summary>
        /// <param name="interval">Interval time in milliseconds to check the serial port for data.</param>
        protected void SetTimerInterval(int interval = DEFAULT_SERIAL_INTERVAL)
        {
            //_timer.Interval = interval;
            _threadInterval = interval;
        }

        /// <summary>
        /// Read thread used to read the data from the serial port.
        /// This will sleep until a timeout period is met.  Then wake up
        /// and check if there is data to read.  If there is data to read,
        /// it will read the data and pass it to the handlers.  It will not read
        /// data if the serial port is not open, the serial port is in a break state
        /// or if the serial port is sending a command.
        /// 
        /// To stop the thread, set _stopThread to true.  _stopThread is volatile so it
        /// can be set in a multithreaded environment.
        /// </summary>
        /// <param name="data">Not Used.</param>
        private void ReadThreadMethod(object data)
        {
            while (!_stopThread)
            {
                try
                {
                    // Ensure the serial port is open
                    // Not in a break state
                    // And not sending data
                    // And not pasued
                    if (IsAvailable() && !_isSendingData && !_pauseReadThread)
                    {
                        // Block until data is available
                        int bytesAvail = _serialPort.BytesToRead;

                        // Verify data was read from the serial port
                        if (bytesAvail > 0)
                        {
                            // Create a buffer to hold the data
                            byte[] buffer = new byte[bytesAvail];

                            //Debug.WriteLine("Reading data from Serial Port");
                            int bytesRead = _serialPort.Read(buffer, 0, buffer.Length);

                            // Determine if we need to continue to read because not all the bytes were reads
                            if (bytesRead != bytesAvail)
                            {
                                Debug.WriteLine(string.Format("Bytes Read {0},  Bytes Available {1]", bytesRead, bytesAvail));
                                log.Warn(string.Format("Bytes Read {0},  Bytes Available {1]", bytesRead, bytesAvail));
                            }

                            // Pass the data to all subscribers
                            if (this.ReceiveRawSerialDataEvent != null)
                            {
                                this.ReceiveRawSerialDataEvent(buffer);
                            }

                            // Pass the data to a function that can be overloaded
                            // so the data can be handled differently
                            ReceiveDataHandler(buffer);
                        }
                    }
                }
                catch (System.UnauthorizedAccessException ex)
                {
                    // If the port is already in use, do not
                    // start the thread
                    log.Warn("COMM Port Error Reading: " + _serialOptions.Port, ex);

                    if (_serialPort != null)
                    {
                        Disconnect();
                    }
                }
                catch (Exception ex)
                {
                    log.Warn("Error Reading in COMM Port: " + _serialOptions.Port, ex);

                    if (_serialPort != null)
                    {
                        Disconnect();
                    }
                }
                finally
                {
                    _threadWait.WaitOne(_threadInterval);
                }
            }
        }

        /// <summary>
        /// Clear the Input and output buffer.
        /// </summary>
        protected void ClearSerialBuffers()
        {
            if (IsAvailable())
            {
                _serialPort.DiscardInBuffer();
                _serialPort.DiscardOutBuffer();
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
        /// Manually read from the serial port.
        /// You should set PauseReadThread() to true when reading manually
        /// to ensure the read thread does not also read data.
        /// </summary>
        /// <param name="buffer">Buffer to read to.</param>
        /// <param name="offset">Location in buffer to read to.</param>
        /// <param name="count">Number of bytes to read.</param>
        /// <returns>Number of bytes read.</returns>
        public int ReadData(byte[] buffer, int offset, int count)
        {
            int result = 0;
            
            // Lock to prevent multiple threads from reading at the same time
            lock (_readLock)
            {
                if (IsAvailable() && buffer != null)
                {
                    // Read the data from the serial port
                    result = _serialPort.Read(buffer, offset, count);
                }
            }

            // Return number of bytes read
            return result;
        }

        /// <summary>
        /// Send a command to the serial port.
        /// This will add the carrage return to
        /// the end of the string.
        /// </summary>
        /// <param name="data">Data to write.</param>
        public void SendData(string data)
        {
            // Lock to prevent multiple threads from writing at the same time
            lock (_writeLock)
            {
                if (IsAvailable() && !String.IsNullOrEmpty(data))
                {
                    // Format the command
                    data += '\r';

                    _isSendingData = true;
                    _serialPort.Write(data);
                    ReceiveBufferString = data + ReceiveBufferString;

                    // Wait for 485 response and read thread to read response
                    Thread.Sleep(WAIT_STATE);

                    _isSendingData = false;
                }
            }
        }

        /// <summary>
        /// Send the given buffer of data to the serial port.
        /// The serial port must be open and not in a break state.
        /// This will write the data in the buffer at the given
        /// offset with the given length to the serial port.
        /// 
        /// This will not add the carrage return to the
        /// end of the data.
        /// </summary>
        /// <param name="buffer">Buffer of data to write.</param>
        /// <param name="offset">Location in the buffer to write.</param>
        /// <param name="count">Number of bytes to write.</param>
        public void SendData(byte[] buffer, int offset, int count)
        {
            // Lock to prevent multiple threads from writing at the same time
            lock (_writeLock)
            {
                if (IsAvailable() && buffer != null)
                {
                    _isSendingData = true;
                    _serialPort.Write(buffer, offset, count);
                    ReceiveBufferString = System.Text.ASCIIEncoding.ASCII.GetString(buffer) + ReceiveBufferString;

                    // Wait for 485 response and read thread to read response
                    Thread.Sleep(WAIT_STATE);

                    _isSendingData = false;
                }
            }
        }

        /// <summary>
        /// Send data through the serial port and wait for a response
        /// back from the serial port.  If the response was the same
        /// as the message sent, then return true.
        /// 
        /// </summary>
        /// <param name="data">Data to send through the serial port.</param>
        /// <param name="timeout">Timeout to wait in milliseconds.  Default = 1 sec.</param>
        /// <return>Flag if was successful sending the command.</return>
        public bool SendDataWaitReply(string data, int timeout = 1000)
        {
            // If no data was given
            // return true that no data could be sent.
            // I am not return false, because it would 
            // try to retry with empty or null again.
            if (string.IsNullOrEmpty(data))
            {
                return true;
            }

            // Assume good result
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
                // Special case for BREAK response.  It will give the banner as the response
                if (data == Commands.AdcpCommands.CMD_BREAK)
                {
                    if(!_validateMsg.Contains(" Rowe Technologies Inc."))
                    {
                        log.Warn(string.Format("Error sending BREAK to serial port. Sent data: {0} length: {1}  Received Data: {2} length: {3}", data, data.Length, _validateMsg, _validateMsg.Length));
                        result = false;
                    }
                }
                else
                {
                    // Compare sent vs what was echoed back
                    //if (data.ToLower().CompareTo(_validateMsg.Substring(0, data.Length).ToLower()) != 0)
                    if (!_validateMsg.ToLower().Contains(data.ToLower()))
                    {
                        log.Warn(string.Format("Error sending data to serial port. Sent data: {0} length: {1}  Received Data: {2} length: {3}", data, data.Length, _validateMsg, _validateMsg.Length));
                        result = false;
                    }
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
        /// Send data to the serial port.  Then wait for a response.  Get the
        /// response back from the serial port from the data sent.  The
        /// wait time is based on how long the data sent needs to be processed 
        /// on the other end before it sends a response back.  The waitTime is in
        /// milliseconds.  The default value for waitTime is RTI.AdcpSerialPort.WAIT_STATE.
        /// 
        /// sendBreak is used to send a BREAK before the command is sent.  This is useful to 
        /// wake the system up before sending the command or to get the BREAK result.
        /// </summary>
        /// <param name="data">Data to send to the serial port.</param>
        /// <param name="sendBreak">Flag to send a BREAK or not.</param>
        /// <param name="waitTime">Time to wait for a response in milliseconds.  DEFAULT=RTI.AdcpSerialPort.WAIT_STATE</param>
        /// <returns>Data sent back from the serial port after the data was sent.</returns>
        public string SendDataGetReply(string data, bool sendBreak = false, int waitTime = RTI.AdcpSerialPort.WAIT_STATE)
        {
            // Clear buffer
            ReceiveBufferString = "";

            // Send a BREAK if set
            if (sendBreak)
            {
                SendBreak();

                // Wait for an output
                Thread.Sleep(waitTime);
            }

            // If data is given, send it to the serial port
            if (!string.IsNullOrEmpty(data))
            {
                // Send the data
                SendData(data);
            }

            // Wait for an output
            Thread.Sleep(waitTime);

            // Return the buffer output
            return ReceiveBufferString;
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

        #region Send Serial Data

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

        #endregion

        #region Overrides

        /// <summary>
        /// Display the object to a string.
        /// </summary>
        /// <returns>String of the object.</returns>
        public override string ToString()
        {
            string result = SerialOptions.ToString();

            return result;
        }

        #endregion
    }
}