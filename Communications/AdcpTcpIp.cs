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
 * 07/23/2013      RC          2.23.0       Initial coding
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
    using System.Net.Sockets;

    /// <summary>
    /// Receive and send data to an ADCP through a TCP-IP port.
    /// </summary>
    public class AdcpTcpIp : IDisposable
    {
        #region Variables

        /// <summary>
        ///  Setup logger
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Default period to wait before checking for data to read.
        /// 
        /// 4Hz
        /// </summary>
        protected const int DEFAULT_SERIAL_INTERVAL = 250;

        /// <summary>
        /// Time to wait to see if a command
        /// was sent.  If a timeout occurs
        /// it means a command did not get
        /// a response from the ADCP for sending
        /// a command.  Value in milliseconds.
        /// </summary>
        private const int TIMEOUT = 2000;

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
        /// TCP Client.  To send and receive data through the serial port.
        /// </summary>
        public TcpClient _tcpClient;

        /// <summary>
        /// TCP network stream used to read and write to the TCP client.
        /// </summary>
        public NetworkStream _tcpStream;

        /// <summary>
        /// Set flag if the TCP connection is connected.
        /// </summary>
        public bool _isTcpConnected;

        /// <summary>
        /// Flag to stop the read thread.
        /// </summary>
        public bool _stopTcpReadThread;

        /// <summary>
        /// Used to make the thread sleep between reads.
        /// </summary>
        private EventWaitHandle _threadWait;

        /// <summary>
        /// Thread to read the data from the TCP port.
        /// </summary>
        private Thread _readTcpThread;

        /// <summary>
        /// Time period to wait between reads of the TCP 
        /// port in the ReadThread.
        /// </summary>
        private int _threadInterval;

        /// <summary>
        /// String used to validate the correct message
        /// was echoed back when sending data and waiting
        /// for a response.
        /// </summary>
        private string _validateMsg;

        /// <summary>
        /// Wait for ACK when sending a command to the serial port.
        /// </summary>
        private EventWaitHandle _eventWaitResponse;

        /// <summary>
        /// This is used to pause the processing of the
        /// read thread.  This will cause no processing
        /// to be done in the read thread if set true.
        /// </summary>
        private bool _pauseReadThread;

        #endregion

        #region Properties

        /// <summary>
        /// Options for the ethernet connection.
        /// </summary>
        public AdcpEthernetOptions Options { get; set; }

        /// <summary>
        /// Buffer to hold all the data received from the ethernet port.
        /// </summary>
        private string _receiveBufferString;
        /// <summary>
        /// Buffer to hold all the data received from the ethernet port.
        /// </summary>
        public string ReceiveBufferString
        {
            get { return _receiveBufferString; }
            set
            {
                _receiveBufferString = value;

                // Limit the size of the data in the buffer
                if (_receiveBufferString.Length > 16000)
                {
                    _receiveBufferString = _receiveBufferString.Remove(10000, 6000);
                }
            }
        }

        #endregion

        /// <summary>
        /// Initialize the values.
        /// </summary>
        /// <param name="options">Ethernet options.</param>
        public AdcpTcpIp(AdcpEthernetOptions options)
        {
            // Intialize the values
            Options = options;

            _pauseReadThread = false;

            _threadWait = new EventWaitHandle(false, EventResetMode.AutoReset);
            _threadInterval = DEFAULT_SERIAL_INTERVAL;

            _stopTcpReadThread = false;
            _readTcpThread = new Thread(new ParameterizedThreadStart(ReadThreadMethod));
            _readTcpThread.Start();
        }

        /// <summary>
        /// Shutdown the object.
        /// </summary>
        public void Dispose()
        {
            _stopTcpReadThread = true;
        }

        #region IsOpen

        /// <summary>
        /// Test to see if the ADCP is connected on the TCP-IP port.
        /// </summary>
        /// <returns>True = TCP-IP Port Open / False = TCP-IP Port is NOT open.</returns>
        public bool IsOpen()
        {
            if (_isTcpConnected)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Send Receive Data

        /// <summary>
        /// Connect the TCP client with the given
        /// server and port.
        /// </summary>
        /// <param name="server">TCP server ip.</param>
        /// <param name="port">TCP server port.</param>
        public void ConnectTCP(string server, uint port)
        {
            try
            {
                _tcpClient = new TcpClient(server, (int)port);
                _tcpStream = _tcpClient.GetStream();
                _isTcpConnected = true;

                // Send a status message
                ReceiveBufferString = string.Format("TCP Connected to: {0}:{1} ", server, port);
                PublishRawTcpData(null);
            }
              catch (ArgumentNullException e) 
            {
                log.Error("ArgumentNullException TCP Connecting.", e);
                _isTcpConnected = false;

                ReceiveBufferString = string.Format("TCP Connection to: {0}:{1} could not be made.", server, port);
                PublishRawTcpData(null);
            } 
            catch (SocketException e) 
            {
                log.Error("SocketException TCP Connecting.", e);
                _isTcpConnected = false;

                ReceiveBufferString = string.Format("TCP Socket Error. Connection to: {0}:{1} could not be made.", server, port);
                PublishRawTcpData(null);
            }
            catch(Exception e)
            {
                log.Error("Exception TCP Connecting.", e);
                _isTcpConnected = false;

                ReceiveBufferString = string.Format("TCP Error. Connection to: {0}:{1} could not be made.", server, port);
                PublishRawTcpData(null);
            }
        }

        /// <summary>
        /// Disconnect the TCP port.
        /// </summary>
        public void DisconnectTCP()
        {
            try
            {
                _isTcpConnected = false;

                if (_tcpStream != null)
                {
                    _tcpStream.Close();
                }

                if (_tcpClient != null)
                {
                    // If connected, send a message that we are disconnecting
                    if (_tcpClient.Connected)
                    {
                        // Send a status message
                        ReceiveBufferString = string.Format("TCP Disconnected");
                        PublishRawTcpData(null);

                        _tcpClient.Close();
                    }
                }
            }
              catch (ArgumentNullException e) 
            {
                log.Error("ArgumentNullException TCP Disconnecting.", e);
            } 
            catch (SocketException e) 
            {
                log.Error("SocketException TCP Disconnecting.", e);
            }
            catch(Exception e)
            {
                log.Error("Exception TCP Disconnecting.", e);
            }
        }

        /// <summary>
        /// Read the data from the TCP server.  The size
        /// of the byte array is the number of bytes that
        /// were read in.  The data will be contained within
        /// the byte array.  
        /// 
        /// To optimize the reads, the user should change the
        /// buffer size parameter to the number of bytes read in.
        /// If the max buffer size is always used, increase the
        /// buffer size until a good number is found.
        /// 
        /// If the user would like to convert the data to a string
        /// use the following command:
        /// System.Text.Encoding.ASCII.GetString(result, 0, numBytes);
        /// </summary>
        /// <param name="bufferSize">Buffer size, default is 1024.</param>
        /// <returns>The bytes read from the TCP server.</returns>
        public byte[] ReadData(int bufferSize = 1024)
        {
            byte[] result = null;

            try
            {
                if (_tcpStream != null && _tcpStream.CanRead)
                {
                    // Create a buffer to read in the data
                    byte[] data = new byte[bufferSize];

                    // Read a batch of the TcpServer response bytes.
                    Int32 bytes = _tcpStream.Read(data, 0, data.Length);

                    // Return the data 
                    result = new byte[bytes];
                    Buffer.BlockCopy(data, 0, result, 0, bytes);
                }
            }
            catch (ArgumentNullException e)
            {
                log.Error("ArgumentNullException TCP Read.", e);
                return result;
            }
            catch (SocketException e)
            {
                log.Error("SocketException TCP Read.", e);
                return result;
            }
            catch (Exception e)
            {
                log.Error("Exception TCP Read.", e);
                return result;
            }

            return result;
        }

        /// <summary>
        /// Write the data to the TCP server.
        /// </summary>
        /// <param name="data">Data to write.</param>
        /// <param name="offset">Offset to start the write.</param>
        /// <param name="size">Number of bytes to write from the given data.</param>
        public void SendData(byte[] data, int offset, int size)
        {
            try
            {
                // Check that the stream is available
                if (_tcpStream != null && _tcpStream.CanWrite)
                {
                    _tcpStream.Write(data, offset, size);
                }
            }
            catch (ArgumentNullException e)
            {
                log.Error("ArgumentNullException TCP Write.", e);
            }
            catch (SocketException e)
            {
                log.Error("SocketException TCP Write.", e);
            }
            catch (Exception e)
            {
                log.Error("Exception TCP Write.", e);
            }
        }

        /// <summary>
        /// Send the given string to through the
        /// TCP connection.
        /// </summary>
        /// <param name="data">Data to send.</param>
        /// <returns>The number of bytes written.</returns>
        public void SendData(string data)
        {
            try
            {
                // Check that the stream is available
                if (_tcpStream != null && _tcpStream.CanWrite)
                {
                    // Format the command
                    data += '\r';

                    //byte[] buffer = MathHelper.GetBytes(data);
                    byte[] buffer = MathHelper.GetBytesUtf8(data);
                    _tcpStream.Write(buffer, 0, buffer.Length);
                }
            }
            catch (Exception e)
            {
                log.Error(string.Format("Error sending data to TCP port. Sent data: {0} length: {1}", data, data.Length), e);
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
            this.ReceiveTcpDataEvent += new AdcpTcpIp.ReceiveTcpDataEventHandler(WaitForAck);

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
                    if (!_validateMsg.Contains(" Rowe Technologies Inc."))
                    {
                        log.Warn(string.Format("Error sending BREAK to TCP port. Sent data: {0} length: {1}  Received Data: {2} length: {3}", data, data.Length, _validateMsg, _validateMsg.Length));
                        result = false;
                    }
                }
                else
                {
                    // Compare sent vs what was echoed back
                    //if (data.ToLower().CompareTo(_validateMsg.Substring(0, data.Length).ToLower()) != 0)
                    if (!_validateMsg.ToLower().Contains(data.ToLower()))
                    {
                        log.Warn(string.Format("Error sending data to TCP port. Sent data: {0} length: {1}  Received Data: {2} length: {3}", data, data.Length, _validateMsg, _validateMsg.Length));
                        result = false;
                    }
                }
            }
            // string lengths did not match
            else
            {
                log.Warn(string.Format("Error sending data to TCP port. Sent data: {0} length: {1}  Received Data: {2} length: {3}", data, data.Length, _validateMsg, _validateMsg.Length));
                result = false;
            }

            // ACK occurs when command echoed back with ACK at the end
            // Unsubscribe from receiving an event
            this.ReceiveTcpDataEvent -= WaitForAck;

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

        /// <summary>
        /// Send a BREAK to the TCP port.
        /// </summary>
        public void SendBreak()
        {
            // Send a soft break
            SendData(Commands.AdcpCommands.CMD_BREAK);
        }

        /// <summary>
        /// Send a list of commands.
        /// </summary>
        /// <param name="commands">List of commands.</param>
        /// <returns>TRUE = All commands were sent successfully.</returns>
        public bool SendCommands(List<string> commands)
        {
            bool result = true;
            byte[] reply = new byte[100];

            foreach (string cmd in commands)
            {
                if (cmd.CompareTo(RTI.Commands.AdcpCommands.CMD_START_PINGING) == 0)
                {
                    // Start pinging
                    result &= StartPinging();
                }
                else if (cmd.CompareTo(RTI.Commands.AdcpCommands.CMD_STOP_PINGING) == 0)
                {
                    // Stop pinging
                    result &= StopPinging();
                }
                else if (cmd.CompareTo(RTI.Commands.AdcpCommands.CMD_BREAK) == 0)
                {
                    // Send a break
                    SendBreak();
                }
                else
                {
                    // Send the command within the buffer
                    // Try sending the command.  If it fails try one more time
                    bool currResult = SendDataWaitReply(cmd);
                    if (!currResult)
                    {
                        // Try again if failed first time
                        currResult = SendDataWaitReply(cmd);
                    }

                    // Keep track if any were false
                    // If any were false, this will stay false
                    result &= currResult;
                }

                // Write the line out
                //Debug.WriteLine(cmd);
            }

            return result;
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
            while (!_stopTcpReadThread)
            {
                try
                {
                    // Ensure the serial port is open
                    // Not in a break state
                    // And not sending data
                    // And not pasued
                    //if (IsAvailable() && !_isSendingData && !_pauseReadThread)
                    if (IsOpen() && !_pauseReadThread)
                    {
                        // Verify data was read from the serial port
                        if (_tcpStream.DataAvailable)
                        {
                            // Read in some data from the TCP port
                            byte[] buffer = ReadData();

                            // Pass the data to all subscribers
                            PublishRawTcpData(buffer);

                            // Set the receive buffer string
                            string strBuffer = System.Text.Encoding.ASCII.GetString(buffer, 0, buffer.Length);
                            ReceiveBufferString = strBuffer + ReceiveBufferString;

                            // Pass the data to a function that can be overloaded
                            // so the data can be handled differently
                            PublishTcpData(strBuffer);
                        }
                    }
                }
                catch (System.UnauthorizedAccessException ex)
                {
                    // If the port is already in use, do not
                    // start the thread
                    log.Warn("TCP Port Error Reading: " + Options.IpAddr + ":" + Options.Port, ex);

                    if (_tcpClient != null)
                    {
                        DisconnectTCP();
                    }
                }
                catch (Exception ex)
                {
                    log.Warn("Error Reading in TCP Port: " + Options.IpAddr + ":" + Options.Port, ex);

                    if (_tcpClient != null)
                    {
                        DisconnectTCP();
                    }
                }
                finally
                {
                    _threadWait.WaitOne(_threadInterval);
                }
            }
        }

        #endregion

        #region Ping Serial Commands

        /// <summary>
        /// Send the commands to the ADCP to start pinging.
        /// To start pinging, send the command START.  This
        /// will also set the date and time to the system
        /// based off the computers current date and time.
        /// If any command cannot be sent, set the flag and
        /// return that a command could not be set.
        /// </summary>
        /// <param name="UseLocal">Use the local time to set the ADCP time.  FALSE will set the GMT time.</param>
        /// <returns>TRUE = All commands sent. FALSE = 1 or more commands could not be sent.</returns>
        public bool StartPinging(bool UseLocal = true)
        {
            bool timeResult = false;
            bool pingResult = false;

            // Try to send the command to set the time, if it fails try again
            // The user can choose to set local or UTC time
            if (UseLocal)
            {
                // Local
                timeResult = SetLocalSystemDateTime();
            }
            else
            {
                // UTC time
                timeResult = SetUtcSystemDateTime();
            }

            // Try to send the command, if it fails try again
            pingResult = SendDataWaitReply(RTI.Commands.AdcpCommands.CMD_START_PINGING, TIMEOUT);
            if (!pingResult)
            {
                pingResult = SendDataWaitReply(RTI.Commands.AdcpCommands.CMD_START_PINGING, TIMEOUT);
            }

            // Return if either failed
            return timeResult & pingResult;
        }

        /// <summary>
        /// Send the commands to the ADCP to start pinging.
        /// To start pinging, send the command START.
        /// If any command cannot be sent, set the flag and
        /// return that a command could not be set.
        /// </summary>
        /// <returns>TRUE = All commands sent. FALSE = 1 or more commands could not be sent.</returns>
        public bool StartPinging()
        {
            bool pingResult = false;

            // Try to send the command, if it fails try again
            pingResult = SendDataWaitReply(RTI.Commands.AdcpCommands.CMD_START_PINGING, TIMEOUT);
            if (!pingResult)
            {
                pingResult = SendDataWaitReply(RTI.Commands.AdcpCommands.CMD_START_PINGING, TIMEOUT);
            }

            // Return if either failed
            return pingResult;
        }

        /// <summary>
        /// Send the commands to the ADCP to stop pinging.
        /// To stop pinging, send a BREAK then the STOP command.
        /// The command will wait for an echo back off the command.
        /// If thhe command is not echoed back, then the command was
        /// not receive or the command was not correct.
        /// If any command cannot be sent, set the flag and
        /// return that a command could not be sent.
        /// </summary>
        /// <returns>TRUE = All commands sent. FALSE = 1 or more commands could not be sent.</returns>
        public bool StopPinging()
        {
            bool stopResult = false;

            // Send a break
            SendBreak();

            // Try to send the command, if it fails try again
            stopResult = SendDataWaitReply(RTI.Commands.AdcpCommands.CMD_STOP_PINGING, TIMEOUT);
            if (!stopResult)
            {
                stopResult = SendDataWaitReply(RTI.Commands.AdcpCommands.CMD_STOP_PINGING, TIMEOUT);
            }

            // Return if either failed
            return stopResult;
        }

        #endregion

        #region STIME


        /// <summary>
        /// Set the system time to the ADCP.  This will use the time
        /// currently set on the computer.  This includes
        /// the time and date.
        /// </summary>
        /// <returns>TRUE = DateTime set.</returns>
        public bool SetLocalSystemDateTime()
        {
            bool timeResult = false;

            // Try to send the command, if it fails try again
            timeResult = SendDataWaitReply(RTI.Commands.AdcpCommands.GetLocalSystemTimeCommand(), TIMEOUT);
            if (!timeResult)
            {
                timeResult = SendDataWaitReply(RTI.Commands.AdcpCommands.GetLocalSystemTimeCommand(), TIMEOUT);
            }

            // Allow some time for the ADCP to set the time
            // Wait about 5 seconds
            System.Threading.Thread.Sleep(WAIT_STATE * 20);

            return timeResult;
        }

        /// <summary>
        /// Set the UTC time to the ADCP.  This will use the time
        /// currently set on the computer, then convert it to UTC time.  This includes
        /// the time and date.
        /// </summary>
        /// <returns>TRUE = DateTime set.</returns>
        public bool SetUtcSystemDateTime()
        {
            bool timeResult = false;

            // Try to send the command, if it fails try again
            timeResult = SendDataWaitReply(RTI.Commands.AdcpCommands.GetGmtSystemTimeCommand(), TIMEOUT);
            if (!timeResult)
            {
                timeResult = SendDataWaitReply(RTI.Commands.AdcpCommands.GetGmtSystemTimeCommand(), TIMEOUT);
            }

            // Allow some time for the ADCP to set the time
            // Wait about 5 seconds
            System.Threading.Thread.Sleep(WAIT_STATE * 20);

            return timeResult;
        }

        /// <summary>
        /// Get the Date and Time from the ADCP.
        /// It will be unknown if this time is UTC
        /// or Local.  So DateTime.Kind will be unspecified.
        /// </summary>
        /// <returns>DateTime from the ADCP.</returns>
        public DateTime GetAdcpDateTime()
        {
            // Now get the time and verify it is within 1 second of the previous time
            // Clear buffer
            ReceiveBufferString = "";

            // Send STIME to get the time
            SendData(string.Format("{0}", RTI.Commands.AdcpCommands.CMD_STIME));

            // Wait for an output
            Thread.Sleep(RTI.AdcpSerialPort.WAIT_STATE * 2);
            // Get the current date and time
            DateTime adcpDt = RTI.Commands.AdcpCommands.DecodeSTIME(ReceiveBufferString);           // Decode the date and time from the ADCP

            return adcpDt;
        }

        #endregion

        #region Event

        #region Publish Data

        /// <summary>
        /// Event To subscribe to.  This gives the paramater
        /// that will be passed when subscribing to the event.
        /// </summary>
        /// <param data="ensemble">Byte array of data received from the port.</param>
        public delegate void ReceiveRawTcpDataEventHandler(byte[] data);

        /// <summary>
        /// Subscribe to receive event when data has been successfully
        /// processed.  This can be used to tell if data is in this format
        /// and is being processed or is not in this format.
        /// Subscribe to this event.  This will hold all subscribers.
        /// 
        /// To subscribe:
        /// _adcpTcp.ReceiveRawTcpDataEvent += new AdcpTcp.ReceiveRawTcpDataEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// _adcpEthernet.ReceiveRawTcpDataEvent -= (method to call)
        /// </summary>
        public event ReceiveRawTcpDataEventHandler ReceiveRawTcpDataEvent;

        /// <summary>
        /// Send the data to all the subscribers.
        /// </summary>
        /// <param name="data">Data to send.</param>
        private void PublishRawTcpData(byte[] data)
        {
            // If there are subscribers, send the data
            if (ReceiveRawTcpDataEvent != null)
            {
                ReceiveRawTcpDataEvent(data);
            }
        }

        /// <summary>
        /// Method to call when subscribing to an event.
        /// </summary>
        /// <param name="data">Data received from the serial port.</param>
        public delegate void ReceiveTcpDataEventHandler(string data);

        /// <summary>
        /// Event to subscribe to.
        /// To subscribe:
        /// _adcpTcp.ReceiveTcpDataEvent += new AdcpTcp.ReceiveTcpDataEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// _adcpTcp.ReceiveTcpDataEvent -= (method to call)
        /// </summary>
        public event ReceiveTcpDataEventHandler ReceiveTcpDataEvent;

        /// <summary>
        /// Send the data to all the subscribers.
        /// </summary>
        /// <param name="data">Data to send.</param>
        private void PublishTcpData(string data)
        {
            // If there are subscribers, send the data
            if (ReceiveTcpDataEvent != null)
            {
                ReceiveTcpDataEvent(data);
            }
        }

        #endregion

        #endregion
    }
}
