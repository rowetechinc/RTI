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
 * 03/14/2013      RC          2.19       Initial coding
 * 06/28/2013      RC          2.19       Replaced Shutdown() with IDisposable.
 * 11/21/2013      RC          2.21.0     Added send and receive methods to match the ADCP serial ports commands.
 * 03/17/2014      RC          2.21.4     Sped up the download process.
 * 04/16/2015      RC          3.0.4      Added a timer to allow pinging through ethernet.
 * 10/06/2016      RC          3.3.2      Improved download speed by removing all the buffers.  Now downloading about 16mb a minute.
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Net.NetworkInformation;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.Net.Sockets;
    using System.Timers;

    /// <summary>
    /// Send commands and receive data from the ADCP using
    /// an ethernet connection.
    /// 
    /// The Ethernet pinging is done different from serial communication.  You must poll the ADCP for data.  A buffer within the ADCP is filled, when the ADCP is pinged through the ethernet, it will dump whatever is in the buffer. 
    /// The ADCP is not using UDP and is not a TCP/IP server.  You are really just pinging the ADCP and within the ping is a buffer with the command.  The ping will cause the ADCP to send a ping response and within the ping response is a buffer which contains the ADCP data.  The ping response is a "Internet Control Message Protocol (ICMP)" echo.
    /// 
    /// Any command sent through the ethernet must start with "RTIy".
    /// The command must end with a carriage return (\r).
    /// Example if you want to send a BREAK.
    /// 
    /// RTIyBREAK\r
    /// 
    /// To poll the ADCP for data you send a blank command.
    /// RTIy\r
    /// 
    /// If you are writing your code in C#, you can find examples of this in the file:
    /// https://github.com/rowetechinc/RTI/blob/master/Communications/AdcpEthernet.cs
    /// 
    /// CSHOW will give the IP address of the ADCP.  
    /// 
    /// The IP Port does not need to be used for this setup.
    /// 
    /// </summary>
    public class AdcpEthernet: IDisposable
    {
        #region Variables

        /// <summary>
        ///  Setup logger
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Number of retries when downloading the data.
        /// </summary>
        private const int MAX_DOWNLOAD_RETRY = 100;

        /// <summary>
        /// Time to wait to see if a command
        /// was sent.  If a timeout occurs
        /// it means a command did not get
        /// a response from the ADCP for sending
        /// a command.  Value in milliseconds.
        /// </summary>
        private const int TIMEOUT = 2000; 

        /// <summary>
        /// Number to indicate a bad packet was received.
        /// </summary>
        private const int BAD_PACKET = -123;

        /// <summary>
        /// Default period to wait before checking for data to read.
        /// 
        /// 4Hz
        /// </summary>
        protected const int DEFAULT_SERIAL_INTERVAL = 250;

        /// <summary>
        /// A wait state used to wait between sending a command and
        /// expecting a result back in the buffer.  Since the data is
        /// not read continuously from the serial port, use must wait a
        /// period of time between sending a command and expecting a result.
        /// </summary>
        public const int WAIT_STATE = DEFAULT_SERIAL_INTERVAL;    // Time to wait between sending a message and getting a response

        /// <summary>
        /// Binarywriter to write the download data
        /// to a file.
        /// </summary>
        private BinaryWriter _downloadDataBinWriter;

        /// <summary>
        /// Name of the current file being downloaded.
        /// </summary>
        private string _downloadFileName;

        /// <summary>
        /// Flag to cancel the download process.
        /// </summary>
        private bool _cancelDownload;

        /// <summary>
        /// Queue the data up to be written to the file
        /// async.
        /// </summary>
        private ConcurrentQueue<byte[]> _downloadWriterQueue;

        /// <summary>
        /// Flag to know if the writer is currently writing data
        /// async.
        /// </summary>
        private bool _isWritingDownloadedData;

        /// <summary>
        /// If timing based off time, enable this timer.
        /// </summary>
        private System.Timers.Timer _pingTimer;

        #endregion

        #region Properties

        /// <summary>
        /// Options for the ethernet connection.
        /// Port does not need to be set for this setup.
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
                    _receiveBufferString = _receiveBufferString.Remove(0, _receiveBufferString.Length - 6000);
                }
            }
        }

        #endregion

        /// <summary>
        /// Create an object to connect to the ADCP
        /// using the ethernet port.
        /// </summary>
        /// <param name="options">Ethernet options.</param>
        public AdcpEthernet(AdcpEthernetOptions options)
        {
            Options = options;
            _downloadFileName = "";
            _cancelDownload = false;
            _isWritingDownloadedData = false;
            _downloadWriterQueue = new ConcurrentQueue<byte[]>();

            // Create the timer to ping
            SetTimer(options.TimerInterval);
        }

        /// <summary>
        /// Shutdown the object.
        /// </summary>
        public void Dispose()
        {
            // Close the binary writer if it is still open
            CloseBinaryWriter();

            // Stop the timer
            StopTimer();
        }

        #region Pinging

        /// <summary>
        /// Send the commands to the ADCP to start pinging.
        /// It then checks that the command was sent properly.  If
        /// the command was not accepted, FALSE will be returned.
        /// </summary>
        /// <returns>TRUE = Command sent. FALSE = Command not accepted.</returns>
        public bool StartPinging()
        {
            byte[] replyBuffer = null;

            // Check the result to make the command was accepted
            if (SendData(Commands.AdcpCommands.CMD_START_PINGING, ref replyBuffer) == BAD_PACKET)
            {
                return false;
            }

            return true;
        }

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

            // Enable the timer to start calling the  ping command to get a ensemble response
            _pingTimer.Enabled = true;

            // Return if either failed
            return timeResult & pingResult;
        }

        /// <summary>
        /// Send the commands to the ADCP to stop pinging.
        /// It then checks that the command was sent properly.  If
        /// the command was not accepted, FALSE will be returned.
        /// </summary>
        /// <returns>TRUE = Command sent. FALSE = Command not accepted.</returns>
        public bool StopPinging()
        {
            byte[] replyBuffer = null;

            // Stop the ping timer
            _pingTimer.Enabled = false;

            // Check the result to make the command was accepted
            if (SendData(Commands.AdcpCommands.CMD_STOP_PINGING, ref replyBuffer) == BAD_PACKET)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region BinaryWriter

        /// <summary>
        /// Create a binarywriter to write the
        /// data to the incoming data to the file.
        /// </summary>
        /// <param name="filePath">File path to create binary writer.</param>
        /// <returns>BinaryWriter</returns>
        private BinaryWriter CreateBinaryWriter(string filePath)
        {
            // Close any previous binary writers
            CloseBinaryWriter();

            // If the file exist, create a new file
            // If it does not, create the file
            FileStream fs;
            if (File.Exists(filePath))
            {
                fs = new FileStream(filePath, FileMode.Create);
            }
            else
            {
                fs = new FileStream(filePath, FileMode.CreateNew);
            }

            // Create the writer for data.
            return new BinaryWriter(fs);
        }

        /// <summary>
        /// Close the binary writer.  This will close, dispose and
        /// set the writer to null.
        /// </summary>
        private void CloseBinaryWriter()
        {
            if (_downloadDataBinWriter != null)
            {
                // Wait for all the writing to complete before closing and disposing of the writer
                while (_isWritingDownloadedData) { };

                _downloadDataBinWriter.Flush();
                _downloadDataBinWriter.Close();
                _downloadDataBinWriter.Dispose();
                _downloadDataBinWriter = null;
            }
        }

        #endregion

        #region IsOpen

        /// <summary>
        /// Test to see if the ADCP is connected on the ethernet port.
        /// Send a BREAK to the ADCP.  If we get no response,
        /// then the ADCP is not connected.
        /// </summary>
        /// <returns>True = Ethernet Port Open / False = Ethernet Port is NOT open.</returns>
        public bool IsOpen()
        {
            // Create any array, this buffer will be resized by response
            byte[] buffer = new byte[100];

            // Send a soft BREAK
            // If no response comes back, then the ADCP is not connected
            if (SendData(Commands.AdcpCommands.CMD_BREAK, ref buffer, true) > 0)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Send/Receive Data

        /// <summary>
        /// Send a soft break to the ADCP.
        /// </summary>
        public void SendBreak()
        {
            // Create any array, this buffer will be resized by response
            //byte[] buffer = new byte[100];
            
            // Send a soft break
            SendDataGetReply(Commands.AdcpCommands.CMD_BREAK, false);
        }

        /// <summary>
        /// Send the given buffer of data to the ADCP.
        /// 
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
            // Convert the byte array to a string
            string data = MathHelper.ByteArrayToString(buffer, count, offset);

            // This buffer will get recreated in the response
            byte[] reply = new byte[100];

            // Send the data to ethernet port
            SendData(data, ref reply);

            Thread.Sleep(WAIT_STATE);

            // Read the buffer for a reply
            ReadData(ref reply);
        }

        /// <summary>
        /// Send a command to the serial port.
        /// This will add the carrage return to
        /// the end of the string.
        /// </summary>
        /// <param name="data">Data to write.</param>
        public void SendData(string data)
        {
            // Send data and wait for the reply
            SendDataWaitReply(data);
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
            // This buffer will get recreated in the response
            byte[] reply = new byte[100];

            // Send the data to ethernet port
            // Publish the results by setting flag to true
            SendData(data, ref reply, true, timeout);

            Thread.Sleep(WAIT_STATE);

            // Read the buffer for a reply
            if (ReadData(ref reply) > 0)
            {
                return true;
            }

            return false;
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
            // This buffer will get recreated in the response
            byte[] reply = new byte[100];

            // Send the data to ethernet port
            // Publish the results by setting flag to true
            SendData(data, ref reply, true);

            Thread.Sleep(WAIT_STATE);

            // Read the buffer for a reply
            ReadData(ref reply);

            return Encoding.ASCII.GetString(reply);
        }

        /// <summary>
        /// Send data to the ADCP through the ethernet.  
        /// 
        /// This will check for a possible connection to the ADCP using the ethernet.  If a connection 
        /// can be made, it will send the command to the ADCP.  It will then wait for
        /// a reply.  If the response is received, the payload is removed from the
        /// reply and published to all subscribers.  It will then return the number of bytes
        /// read back in the reply.
        /// 
        /// The reply is the not response from the command but the reply that the command was received.
        /// You will need to still send a ReadData() to get the response of the command.
        /// </summary>
        /// <param name="cmd">Command to send to the ADCP.</param>
        /// <param name="replyBuffer">Reply Buffer.  This is the data from the reponse.</param>
        /// <param name="showResults">Flag if the results should be published.</param>
        /// <param name="timeout">Timeout value to wait in milliseconds.</param>
        /// <param name="showStatus">Show the status of the ping response.</param>
        /// <returns>Number of bytes read back from the ADCP.</returns>
        public int SendData(string cmd, ref byte[] replyBuffer, bool showResults = false, int timeout = 2000, bool showStatus = false)
        {
            int i;
            int result = 0;

            using (Ping pingSender = new Ping())
            {
                // Use the default TTL value which is 128,
                // but change the fragmentation behavior.
                PingOptions options = new PingOptions();
                options.DontFragment = true;

                // Create a buffer of bytes to be transmitted.
                byte[] buffer = new byte[1024];

                // Send the RTIy command to the ADCP through ethernet
                // Then any command after RTIy will be replied with the next read
                // The firmware ignores the Y but the command has to be 4 characters long
                string data = "RTIy";    

                // Add the command to the buffer
                if (!string.IsNullOrEmpty(cmd))
                {
                    data += (cmd + '\r');
                }

                // Set the command to the buffer
                byte[] tbuff = Encoding.ASCII.GetBytes(data);
                for (i = 0; i < tbuff.Length; i++)
                {
                    buffer[i] = tbuff[i];
                }

                try
                {
                    // Send the command to the address and get a response back
                    // This will wait for the timeout to get a response
                    PingReply reply = pingSender.Send(Options.IpAddr, timeout, buffer, options);

                    // Verify the reply
                    if (reply.Status == IPStatus.Success)
                    {
                        if (showStatus)
                        {
                            // Form the reply response
                            ReceiveBufferString += string.Format("\r\n");
                            ReceiveBufferString += string.Format("* Status:         {0}\n", reply.Status.ToString());
                            ReceiveBufferString += string.Format("* Address:        {0}\n", reply.Address.ToString());
                            ReceiveBufferString += string.Format("* RoundTrip time: {0}\n", reply.RoundtripTime.ToString());
                            ReceiveBufferString += string.Format("* Time to live:   {0}\n", reply.Options.Ttl.ToString());
                            ReceiveBufferString += string.Format("* Don't fragment: {0}\n", reply.Options.DontFragment.ToString());
                            ReceiveBufferString += string.Format("* Buffer size:    {0}\n", reply.Buffer.Length.ToString());
                            ReceiveBufferString += string.Format("\r\n");
                        }

                        // Check if the IP for the reply matched the one sent
                        if (reply.Buffer[0] == Convert.ToByte(Options.IpAddrA) &&
                            reply.Buffer[1] == Convert.ToByte(Options.IpAddrB) &&
                            reply.Buffer[2] == Convert.ToByte(Options.IpAddrC) &&
                            reply.Buffer[3] == Convert.ToByte(Options.IpAddrD))
                        {
                            // Get the number of bytes in the reply
                            int bytes = GetReplyBytes(reply);

                            // Set the number of bytes in the reply
                            result = bytes;

                            // Start where the buffer data is
                            // and copy the buffered ADCP data from
                            // the response
                            int j = 0;
                            replyBuffer = new byte[bytes];
                            for (i = 6; i < bytes + 6; i++, j++)
                            {
                                // Store the reply in the buffer
                                replyBuffer[j] = reply.Buffer[i];
                            }

                            // Display the results
                            if (showResults && result > 0)
                            {
                                // Display the reply
                                string strBuffer = Encoding.ASCII.GetString(replyBuffer);
                                ReceiveBufferString += strBuffer;

                                // Publish the data
                                PublishEthernetData(strBuffer);
                            }

                            // Publish the raw data
                            // Put this after ReceiveBufferString is set to ensure
                            // that the update is published for the ReceiveBufferString also
                            PublishRawEthernetData(replyBuffer);
                        }
                        else
                        {
                            // The reply IP did not match the IP for the data sent
                            ReceiveBufferString += "...No Reply...";
                            PublishEthernetData(ReceiveBufferString);
                        }
                    }
                    else
                    {
                        // Return a negative number that no reply was recevied
                        // and an error occured.
                        // A RESEND should be sent to try and get the packet again
                        result = BAD_PACKET;

                        // The reply IP did not match the IP for the data sent
                        ReceiveBufferString += "...No Connection... " + reply.Status.ToString();
                        PublishEthernetData(ReceiveBufferString);
                    }
                }
                catch(Exception e)
                {
                    ReceiveBufferString += ("...EMAC error...") + e.ToString();
                    PublishEthernetData(ReceiveBufferString);
                }
            }

            //pingSender.Dispose();
            //tmrReadCommPort.Enabled = true;
            return result;
        }

        /// <summary>
        /// This will read data in from the ethernet buffer on the ADCP.
        /// By sending no command, the ADCP will send back whatever is in the
        /// ethernet buffer.
        /// 
        /// This will continue to read until nothing is left to read.  It will then
        /// return all the data read.
        /// 
        /// </summary>
        /// <param name="replyBuffer">Reply Buffer.  This is the data from the reponse.</param>
        /// <param name="showResults">Display the results of the read to the read buffer.</param>
        /// <param name="timeout">Timeout period in milliseconds.</param>
        /// <returns>Return the number of bytes read.</returns>
        public int ReadData(ref byte[] replyBuffer, bool showResults = false, int timeout = 2000)
        {
            int count = SendData("", ref replyBuffer, true, timeout);            // Send blank command to read the data

            return count;
        }

        /// <summary>
        /// Get the number of bytes in the reply buffer.
        /// </summary>
        /// <param name="reply">Reply from sending data.</param>
        /// <returns>Number of bytes in the reply buffer.</returns>
        private int GetReplyBytes(PingReply reply)
        {
            int bytes = 0;

            if (reply != null && reply.Buffer != null)
            {
                // Get the number of bytes in the response
                bytes = reply.Buffer[4];
                bytes += reply.Buffer[5] << 8;

                // Convert if in megabytes
                int mbytes = reply.Buffer.Length - 6;

                if (bytes > mbytes)
                {
                    bytes = mbytes;
                }

            }

            return bytes;
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
                    bool sendResult = false;
                    int currResult = SendData(cmd, ref reply, true);
                    if (currResult < 0)
                    {
                        // Try again if failed first time
                        if (SendData(cmd, ref reply, true) > 0)
                        {
                            sendResult = true;
                        }
                    }
                    else
                    {
                        sendResult = true;
                    }

                    // Keep track if any were false
                    // If any were false, this will stay false
                    result &= sendResult;
                }

                // Write the line out
                //Debug.WriteLine(cmd);
            }

            return result;
        }

        #endregion

        #region Download

        /// <summary>
        /// This will ask to resend the previous packet.  read data in from the ethernet buffer on the ADCP.
        /// By sending no command, the ADCP will send back whatever is in the
        /// ethernet buffer.
        /// </summary>
        /// <param name="replyBuffer">Reply Buffer.  This is the data from the reponse.</param>
        /// <param name="showResults">Display the results of the read to the read buffer.</param>
        /// <param name="timeout">Timeout period in milliseconds.</param>
        /// <returns>Return the number of bytes read.</returns>
        public int ResendData(ref byte[] replyBuffer, bool showResults = false, int timeout = 2000)
        {

            return SendData("resend", ref replyBuffer, showResults, timeout);
        }

        /// <summary>
        /// This will clear the buffer of all data.  The buffer will then contain nothing so
        /// if an attempt is made to download, there will be no data to download.
        /// </summary>
        /// <param name="replyBuffer">Reply Buffer.  This is the data from the reponse.</param>
        /// <param name="showResults">Display the results of the read to the read buffer.</param>
        /// <param name="timeout">Timeout period in milliseconds.</param>
        /// <returns>Return the number of bytes read.</returns>
        public int ClearData(ref byte[] replyBuffer, bool showResults = false, int timeout = 2000)
        {

            return SendData("clear", ref replyBuffer, showResults, timeout);
        }

        /// <summary>
        /// This will restart downloading the current file.  This will cause the ADCP to reset the buffer
        /// index to the beginning of the file.
        /// </summary>
        /// <param name="replyBuffer">Reply Buffer.  This is the data from the reponse.</param>
        /// <param name="showResults">Display the results of the read to the read buffer.</param>
        /// <param name="timeout">Timeout period in milliseconds.</param>
        /// <returns>Return the number of bytes read.</returns>
        public int RestartData(ref byte[] replyBuffer, bool showResults = false, int timeout = 2000)
        {

            return SendData("restart", ref replyBuffer, showResults, timeout);
        }

        /// <summary>
        /// Download the the given file name and save the file to the given
        /// directory.  If parseData is set true, then also add the data to the
        /// project.
        /// 
        /// Send a message to the ethernet port to clear the current buffer.
        /// Send a message to the ethernet port to download the given file.  This will
        /// cause the file to be loaded into RAM.  This will take some time for the ADCP
        /// to accomplish.  So wait for the ADCP by checking if data is available to download.
        /// If the data is being parsed, we do not need to write the data here, it will be
        /// written by the parser.  Then download all the packets from the ADCP for the
        /// given file.  Once complete, close the writer and publish complete.
        /// 
        /// Download is about 16mb/minute.
        /// 
        /// </summary>
        /// <param name="dirName">Directory to the store the data.</param>
        /// <param name="fileName">File to download from the ADCP.</param>
        /// <param name="parseData">TRUE = Parse and add the data to the project.</param>
        /// <returns>TRUE = File was downloaded.</returns>
        public bool DownloadData(string dirName, string fileName, bool parseData = false)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                // Initialize the values
                _cancelDownload = false;
                _downloadWriterQueue = new ConcurrentQueue<byte[]>();
                _isWritingDownloadedData = false;

                // Initialize the ADCP buffer by clearing it
                byte[] clearBuffer = null;
                ClearData(ref clearBuffer);

                // Set the file name being downloaded
                _downloadFileName = fileName;

                // If the data is being parsed,
                // the raw data will also be written to a file
                if (!parseData)
                {
                    // Create the BinaryWriter
                    string filePath = dirName + "\\" + fileName;
                    _downloadDataBinWriter = CreateBinaryWriter(filePath);
                }

                // Send command to download data
                string cmd = Commands.AdcpCommands.CMD_DSXD + fileName;
                byte[] replyBuffer = null;
                SendData(cmd, ref replyBuffer);

                // Pause to allow the buffer on the ADCP to be filled with the file
                while (DownloadPacket(250) < 0)
                {
                    System.Threading.Thread.Sleep(1000);
                }

                //Stopwatch sw = new Stopwatch();
                //sw.Start();

                // Download all the packets for the file
                long totalBytes = DownloadPackets();

                //sw.Stop();

                // MB to bytes
                //double mb = totalBytes / MathHelper.MB_TO_BYTES;
                //double time = mb / sw.Elapsed.TotalSeconds;

                //Debug.WriteLine(string.Format("Total Seconds: {0}; Totals Bytes: {1}; MB/s: {2}", sw.Elapsed.TotalSeconds, totalBytes, time));

                // Close the writer
                CloseBinaryWriter();

                // Publish the download is complete for this file
                PublishDownloadCompleteEvent(true);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Cancel the download process.  This
        /// will set the flag to cancel the download.
        /// If any loops are running for the download process,
        /// they will be stopped.
        /// </summary>
        public void CancelDownload()
        {
            _cancelDownload = true;
        }

        /// <summary>
        /// Download all the packets for a file.  THis will contine
        /// to download packets until MAX_DOWNLOAD_RETRY has been met.
        /// You will hit MAX_DOWNLOAD_RETRY when the file has no more 
        /// packets to download or if you lose connection.
        /// 
        /// Download is about 7mb/minute.
        /// 
        /// </summary>
        /// <returns>Number of bytes read for the file.</returns>
        private long DownloadPackets()
        {
            int bytesRead = 0;
            long totalBytes = 0;
            //int packets = 1;

            // Set the maximum number of retries per
            // file when downloading.
            int retry = MAX_DOWNLOAD_RETRY;

            // Download a packet
            // Continue to download until all the data
            // has been is downloaded.  This will stop 
            // downloading when the MAX_RETRY is meant.
            // This will happen either because no data
            // is available or a connection was lost.
            bytesRead = DownloadPacket();
            do
            {
                // If no bytes were read, we will retry
                // once we have run out of retries, 
                // we will bail out.
                if (bytesRead <= 0)
                {
                    retry--;
                }

                // If the download is canceled
                // return the current byte count
                if (_cancelDownload)
                {
                    return totalBytes;
                }

                // Accumulate the number of bytes read
                totalBytes += bytesRead;

                // Let the ADCP get the next packet ready
                //System.Threading.Thread.Sleep(1);

                // Get the first packet to determine
                // if we can get data.  If this does 
                // not return any data, we have no
                // connection or the file has no data.
                // Continue downloading data until all
                // the packets have been downloaded.
                bytesRead = DownloadPacket();

                //packets++;

                // Publish number of bytes received
                PublishDownloadProgressEvent((int)totalBytes);
            }
            while (retry > 0);

            //Debug.WriteLine(string.Format("Packets: {0} Bytes: {1}", packets, totalBytes));

            return totalBytes;
        }

        /// <summary>
        /// Send the command to download a packet.  Use the replyBuffer
        /// to get the buffer from the downloaded packet.  If the writer is null,
        /// do no write the data.
        /// </summary>
        /// <param name="timeout">Timeout to wait for the packet in milliseconds.</param>
        /// <returns>Number of bytes read.</returns>
        private int DownloadPacket(int timeout = 2000)
        {
            // Read a packet from the ADCP
            byte[] replyBuffer = null;
            int bytes = ReadData(ref replyBuffer, false, timeout);

            // Retry getting the packet if the packet
            // was not received properly.
            int retry = 20;
            while (bytes == BAD_PACKET || retry-- < 0)
            {
                bytes = ResendData(ref replyBuffer, false, timeout);
            }

            // If a packet was read, and a writer exist,
            // write the packet to a file.
            //if (bytes > 0 && writer != null)
            if (bytes > 0)
            {
                // Write the data to the file
                //writer.Write(replyBuffer, 0, bytes);
                //WriteData(replyBuffer, 0, bytes);
                _downloadDataBinWriter.Write(replyBuffer, 0, bytes);
            }

            // Return the number of bytes read
            return bytes;
        }

        /// <summary>
        /// Download the list of file from the ADCP.  A command
        /// is sent to give the listing of all the files on the ADCP.
        /// Continue reading in the data until "Use Space:" has been 
        /// received.  This is the end of the listing.
        /// </summary>
        /// <returns>String of the directory listing from the ADCP.</returns>
        private string DownloadDirectoryListing()
        {
            // If the ADCP is pinging, make it stop
            StopPinging();

            // The D command will cancel any pending downloads
            // Send it twice to first ignore the last packet sent, then
            // stop the download process
            byte[] replyBuffer = null;
            SendData(string.Format("{0}", RTI.Commands.AdcpCommands.CMD_DS_CANCEL), ref replyBuffer);
            SendData(string.Format("{0}", RTI.Commands.AdcpCommands.CMD_DS_CANCEL), ref replyBuffer);

            // Send the command to clear the download buffer
            ClearData(ref replyBuffer);

            // Send the command to get the directory listing
            SendData(string.Format("{0}", RTI.Commands.AdcpCommands.CMD_DSDIR), ref replyBuffer);               // Get the diretory listing

            string buffer = "";
            if (replyBuffer != null)
            {
                int retry = 100;

                // Convert the reply buffer to a string
                buffer = Encoding.ASCII.GetString(replyBuffer);

                // Check if we have received all the listing
                // If not, continue to read in the data
                // Or continue until retry is maxed out
                while (buffer.IndexOf("Used Space:") <= 0 || retry-- > 0)
                {
                    ReadData(ref replyBuffer);

                    buffer += Encoding.ASCII.GetString(replyBuffer);
                }
            }

            return buffer;
        }

        /// <summary>
        /// Send the command to display the Directory listing.
        /// Then parse the buffer containing the directory listing
        /// and return a list of all the file.s
        /// </summary>
        /// <returns>List of the directory files.</returns>
        public RTI.Commands.AdcpDirListing GetDirectoryListing()
        {
            // Download the directory listing to _dirListingString
            string dirListingString = DownloadDirectoryListing();

            // Filter the list of directories
            RTI.Commands.AdcpDirListing dirListing = RTI.Commands.AdcpCommands.DecodeDSDIR(dirListingString);

            // Return the content of the buffer
            return dirListing;
        }

        /// <summary>
        /// Buffer the data so it can written to the file asnyc.
        /// </summary>
        /// <param name="buffer">Buffer of data.</param>
        /// <param name="index">Start location in the buffer.</param>
        /// <param name="length">Length of the data in the buffer.</param>
        public void WriteData(byte[] buffer, int index, int length)
        {
            // Copy the buffer
            byte[] qBuffer = new byte[length];
            Buffer.BlockCopy(buffer, index, qBuffer, 0, length);

            // Add the data to the queue
            _downloadWriterQueue.Enqueue(qBuffer);
            
            // Write the data if it has not started
            if (!_isWritingDownloadedData)
            {
                //BackgroundWorker worker = new BackgroundWorker();
                //worker.DoWork += delegate(object s, DoWorkEventArgs args)
                //{
                    WriteDownloadData();
                //};
                //worker.RunWorkerAsync();
            }

        }

        /// <summary>
        /// Write the queued data to the file.
        /// </summary>
        private void WriteDownloadData()
        {
            try
            {
                while (_downloadWriterQueue.Count > 0)
                {
                    _isWritingDownloadedData = true;

                    byte[] buffer = null;
                    _downloadWriterQueue.TryDequeue(out buffer);

                    if (buffer != null && _downloadDataBinWriter != null)
                    {
                        _downloadDataBinWriter.Write(buffer, 0, buffer.Length);
                    }
                }
            }
            catch(Exception e)
            {
                log.Error("Error writing download data.", e);
                _isWritingDownloadedData = false;
            }

            _isWritingDownloadedData = false;
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
            System.Threading.Thread.Sleep(RTI.SerialConnection.WAIT_STATE * 20);

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
            System.Threading.Thread.Sleep(RTI.SerialConnection.WAIT_STATE * 20);

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

        #region Timer

        /// <summary>
        /// Set the timer to get the next ping.
        /// By default it will check every second for a ping from the ADCP.
        /// 
        /// <param name="timer">Time between waiting for the next ping in milliseconds.</param>
        /// </summary>
        private void SetTimer(double timer = 1000)
        {
            if (_pingTimer == null)
            {
                _pingTimer = new System.Timers.Timer(timer);
                _pingTimer.Elapsed += new ElapsedEventHandler(_avgTimer_Elapsed);
            }
        }

        /// <summary>
        /// Stop the timer.  This will dispose of the timer.
        /// </summary>
        private void StopTimer()
        {
            if (_pingTimer != null)
            {
                _pingTimer.Elapsed -= _avgTimer_Elapsed;
                _pingTimer.Dispose();
            }
        }

        /// <summary>
        /// If the timer is enabled and it goes off, this method will be called.
        /// Send no command to get the next ensemble from the ADCP.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Not used.</param>
        void _avgTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Send no command to get the next ping

            byte[] buffer = null;
            int count = ReadData(ref buffer);
            while (count > 0)
            {
                count = ReadData(ref buffer);
            }
        }

        #endregion

        #region Events

        #region Publish Data

        /// <summary>
        /// Event To subscribe to.  This gives the paramater
        /// that will be passed when subscribing to the event.
        /// </summary>
        /// <param data="ensemble">Byte array of data received from the port.</param>
        public delegate void ReceiveRawEthernetDataEventHandler(byte[] data);

        /// <summary>
        /// Subscribe to receive event when data has been successfully
        /// processed.  This can be used to tell if data is in this format
        /// and is being processed or is not in this format.
        /// Subscribe to this event.  This will hold all subscribers.
        /// 
        /// To subscribe:
        /// _adcpEthernet.ReceiveRawEthernetDataEvent += new AdcpEthernet.ReceiveRawSerialDataEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// _adcpEthernet.ReceiveRawEthernetDataEvent -= (method to call)
        /// </summary>
        public event ReceiveRawEthernetDataEventHandler ReceiveRawEthernetDataEvent;

        /// <summary>
        /// Send the data to all the subscribers.
        /// </summary>
        /// <param name="data">Data to send.</param>
        private void PublishRawEthernetData(byte[] data)
        {
            // If there are subscribers, send the data
            if (ReceiveRawEthernetDataEvent != null)
            {
                ReceiveRawEthernetDataEvent(data);
            }
        }

        /// <summary>
        /// Method to call when subscribing to an event.
        /// </summary>
        /// <param name="data">Data received from the serial port.</param>
        public delegate void ReceiveEthernetDataEventHandler(string data);

        /// <summary>
        /// Event to subscribe to.
        /// To subscribe:
        /// _adcpEthernet.ReceiveEthernetData += new AdcpEthernet.ReceiveEthernetDataEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// _adcpEthernet.ReceiveEthernetData -= (method to call)
        /// </summary>
        public event ReceiveEthernetDataEventHandler ReceiveEthernetDataEvent;

        /// <summary>
        /// Send the data to all the subscribers.
        /// </summary>
        /// <param name="data">Data to send.</param>
        private void PublishEthernetData(string data)
        {
            // If there are subscribers, send the data
            if (ReceiveEthernetDataEvent != null)
            {
                ReceiveEthernetDataEvent(data);
            }
        }

        #endregion

        #region Download Events

        #region File Size Event

        /// <summary>
        /// Event to get the file size for the given
        /// file name.  The size will be in bytes.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileSize">Size of the file in bytes.</param>
        public delegate void DownloadFileSizeEventHandler(string fileName, long fileSize);

        /// <summary>
        /// Subscribe to receive the event for the size of the file in bytes.
        /// 
        /// To subscribe:
        /// _adcpSerialPort.FileSizeEvent += new serialConnection.FileSizeEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// _adcpSerialPort.FileSizeEvent -= (method to call)
        /// </summary>
        public event DownloadFileSizeEventHandler DownloadFileSizeEvent;

        /// <summary>
        /// Publish the event for the size of the file
        /// that is currently being downloaded.
        /// </summary>
        /// <param name="fileSize">File Size in bytes.</param>
        private void PublishFileSizeEvent(long fileSize)
        {
            if (DownloadFileSizeEvent != null)
            {
                DownloadFileSizeEvent(_downloadFileName, fileSize);
            }
        }

        #endregion

        #region Download Progress Event

        /// <summary>
        /// Event to receive the progress of the download process.
        /// This will give the number of bytes currently written
        /// for the current file being downloaded.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="bytesWritten">Current bytes written.</param>
        public delegate void DownloadProgressEventHandler(string fileName, long bytesWritten);

        /// <summary>
        /// Subscribe to receive the event for the project of the download.  This will get the
        /// current number of bytes written for the current file being downloaded.
        /// 
        /// To subscribe:
        /// _adcpSerialPort.DownloadFileSizeEvent += new serialConnection.DownloadFileSizeEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// _adcpSerialPort.DownloadFileSizeEvent -= (method to call)
        /// </summary>
        public event DownloadProgressEventHandler DownloadProgressEvent;

        /// <summary>
        /// Publish the event of the current progress
        /// of downloading the file.  This will give
        /// the number of bytes currently downloaded.
        /// </summary>
        /// <param name="bytesWritten">Number of bytes written.</param>
        private void PublishDownloadProgressEvent(int bytesWritten)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += delegate(object s, DoWorkEventArgs args)
            {
                if (DownloadProgressEvent != null)
                {
                    DownloadProgressEvent(_downloadFileName, bytesWritten);
                }
            };
            worker.RunWorkerAsync();
        }

        #endregion

        #region Download Complete Event

        /// <summary>
        /// Event to receive when the download is complete
        /// for the given file name.
        /// The parameter goodDownload is used to tell the user
        /// if the download was completed successfully or the download
        /// was aborted.
        /// </summary>
        /// <param name="fileName">Name of the file completed the download.</param>
        /// <param name="goodDownload">Flag if the download was completed successfully.</param>
        public delegate void DownloadCompleteEventHandler(string fileName, bool goodDownload);

        /// <summary>
        /// Subscribe to receive the event when the file has been completely downloaded.
        /// 
        /// To subscribe:
        /// _adcpSerialPort.DownloadCompleteEvent += new serialConnection.DownloadCompleteEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// _adcpSerialPort.DownloadCompleteEvent -= (method to call)
        /// </summary>
        public event DownloadCompleteEventHandler DownloadCompleteEvent;

        /// <summary>
        /// Publish the event that the current file
        /// has completed being downloaded.
        /// If the download is completed because of an error, set the
        /// parameter to false.  This will instructor the user, there was an issue
        /// when downloading.
        /// </summary>
        /// <param name="goodDownload">Set flag if the download was good or bad.</param>
        private void PublishDownloadCompleteEvent(bool goodDownload)
        {
            if (DownloadCompleteEvent != null)
            {
                DownloadCompleteEvent(_downloadFileName, goodDownload);
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
