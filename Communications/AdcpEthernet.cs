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

    /// <summary>
    /// Send commands and receive data from the ADCP using
    /// an ethernet connection.
    /// </summary>
    public class AdcpEthernet: IDisposable
    {
        #region Variables

        /// <summary>
        /// Number of retries when downloading the data.
        /// </summary>
        private const int MAX_DOWNLOAD_RETRY = 100;

        /// <summary>
        /// Number to indicate a bad packet was received.
        /// </summary>
        private const int BAD_PACKET = -123;

        /// <summary>
        /// Options for the ethernet connection.
        /// </summary>
        private AdcpEthernetOptions _options;

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

        #endregion

        #region Properties

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
            _options = options;
            _downloadFileName = "";
            _cancelDownload = false;
        }

        /// <summary>
        /// Shutdown the object.
        /// </summary>
        public void Dispose()
        {
            // Close the binary writer if it is still open
            CloseBinaryWriter();
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
        /// Send the commands to the ADCP to stop pinging.
        /// It then checks that the command was sent properly.  If
        /// the command was not accepted, FALSE will be returned.
        /// </summary>
        /// <returns>TRUE = Command sent. FALSE = Command not accepted.</returns>
        public bool StopPinging()
        {
            byte[] replyBuffer = null;

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
                _downloadDataBinWriter.Flush();
                _downloadDataBinWriter.Close();
                _downloadDataBinWriter.Dispose();
                _downloadDataBinWriter = null;
            }
        }

        #endregion

        #region Send/Receive Data

        /// <summary>
        /// Send data to the ADCP through the ethernet.  
        /// 
        /// This will check for a possible connection to the ADCP using the ethernet.  If a connection 
        /// can be made, it will send the command to the ADCP.  It will then wait for
        /// a reply.  If the response is received, the payload is removed from the
        /// reply and published to all subscribers.  It will then return the number of bytes
        /// read back in the reply.
        /// </summary>
        /// <param name="cmd">Command to send to the ADCP.</param>
        /// <param name="replyBuffer">Reply Buffer.  This is the data from the reponse.</param>
        /// <param name="showResults">Flag if the results should be published.</param>
        /// <param name="timeout">Timeout value to wait in milliseconds.</param>
        /// <returns>Number of bytes read back from the ADCP.</returns>
        public int SendData(string cmd, ref byte[] replyBuffer, bool showResults = false, int timeout = 2000)
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
                    PingReply reply = pingSender.Send(_options.IpAddr, timeout, buffer, options);

                    // Verify the reply
                    if (reply.Status == IPStatus.Success)
                    {
                        if (showResults)
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
                        if (reply.Buffer[0] == Convert.ToByte(_options.IpAddrA) &&
                            reply.Buffer[1] == Convert.ToByte(_options.IpAddrB) &&
                            reply.Buffer[2] == Convert.ToByte(_options.IpAddrC) &&
                            reply.Buffer[3] == Convert.ToByte(_options.IpAddrD))
                        {
                            // Get the number of bytes in the reply
                            int bytes = GetReplyBytes(reply);

                            // Set the number of bytes in the reply
                            result = bytes;

                            int j = 0;
                            replyBuffer = new byte[bytes];
                            for (i = 6; i < bytes + 6; i++, j++)
                            {
                                // Store the reply in the buffer
                                replyBuffer[j] = reply.Buffer[i];
                            }

                            // Publish the raw data
                            PublishRawEthernetData(replyBuffer);

                            // Display the results
                            if (showResults && result > 0)
                            {
                                // Set the last byte to 0 to end the string
                                //replyBuffer[j-1] = 0;

                                // Display the reply
                                string strBuffer = Encoding.ASCII.GetString(replyBuffer);
                                ReceiveBufferString += strBuffer;

                                // Publish the data
                                PublishEthernetData(strBuffer);
                            }
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
                        ReceiveBufferString += "...No Connection...";
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
        /// </summary>
        /// <param name="replyBuffer">Reply Buffer.  This is the data from the reponse.</param>
        /// <param name="showResults">Display the results of the read to the read buffer.</param>
        /// <param name="timeout">Timeout period in milliseconds.</param>
        /// <returns>Return the number of bytes read.</returns>
        public int ReadData(ref byte[] replyBuffer, bool showResults = false, int timeout = 2000)
        {
            return SendData("", ref replyBuffer, showResults, timeout);
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
        /// Download is about 7mb/minute.
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

                // Initialize the ADCP buffer by clearing it
                byte[] clearBuffer = null;
                ClearData(ref clearBuffer);

                // Set the file name being downloaded
                _downloadFileName = fileName;

                // Send command to download data
                string cmd = Commands.AdcpCommands.CMD_DSXD + fileName;
                byte[] replyBuffer = null;
                SendData(cmd, ref replyBuffer);

                // Pause to allow the buffer on the ADCP to be filled with the file
                while (DownloadPacket(null, 250) < 0)
                {
                    System.Threading.Thread.Sleep(1000);
                }

                // If the data is being parsed,
                // the raw data will also be written to a file
                BinaryWriter writer = null;
                if (!parseData)
                {
                    // Create the BinaryWriter
                    string filePath = dirName + "\\" + fileName;
                    writer = CreateBinaryWriter(filePath);
                }
                
                // Download all the packets for the file
                long totalBytes = DownloadPackets(writer);

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
        /// <param name="writer">Write to write the data to the file.</param>
        /// <returns>Number of bytes read for the file.</returns>
        private long DownloadPackets(BinaryWriter writer)
        {
            int bytesRead = 0;
            long totalBytes = 0;

            // Set the maximum number of retries per
            // file when downloading.
            int retry = MAX_DOWNLOAD_RETRY;

            // Download a packet
            // Continue to download until all the data
            // has been is downloaded.  This will stop 
            // downloading when the MAX_RETRY is meant.
            // This will happen either because no data
            // is available or a connection was lost.
            bytesRead = DownloadPacket(writer);
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
                System.Threading.Thread.Sleep(1);

                // Get the first packet to determine
                // if we can get data.  If this does 
                // not return any data, we have no
                // connection or the file has no data.
                // Continue downloading data until all
                // the packets have been downloaded.
                bytesRead = DownloadPacket(writer);

                // Publish number of bytes received
                PublishDownloadProgressEvent((int)totalBytes);
            }
            while (retry > 0);

            return totalBytes;
        }

        /// <summary>
        /// Send the command to download a packet.  Use the replyBuffer
        /// to get the buffer from the downloaded packet.  If the writer is null,
        /// do no write the data.
        /// </summary>
        /// <param name="writer">Writer to write the data to file.  If the writer is null, do not write the data.</param>
        /// <param name="timeout">Timeout to wait for the packet in milliseconds.</param>
        /// <returns>Number of bytes read.</returns>
        private int DownloadPacket(BinaryWriter writer, int timeout = 2000)
        {
            // Read a packet from the ADCP
            byte[] replyBuffer = null;
            int bytes = ReadData(ref replyBuffer, false, timeout);

            // Retry getting the packet if the packet
            // was not received properly.
            while (bytes == BAD_PACKET)
            {
                bytes = ResendData(ref replyBuffer, false, timeout);
            }

            // If a packet was read, and a writer exist,
            // write the packet to a file.
            if (bytes > 0 && writer != null)
            {
                // Write the data to the file
                writer.Write(replyBuffer, 0, bytes);
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
            // Send the command to get the directory listing
            byte[] replyBuffer = null;
            //SendData(string.Format("{0}", RTI.Commands.AdcpCommands.CMD_DS_CANCEL), ref replyBuffer);           // Cancel any previous downloads
            //SendData(string.Format("{0}", RTI.Commands.AdcpCommands.CMD_DS_CANCEL), ref replyBuffer);           // Cancel any previous downloads
            SendData(string.Format("{0}", RTI.Commands.AdcpCommands.CMD_DSDIR), ref replyBuffer);               // Get the diretory listing


            string buffer = "";
            if (replyBuffer != null)
            {
                // Convert the reply buffer to a string
                buffer = Encoding.ASCII.GetString(replyBuffer);

                // Check if we have received all the listing
                // If not, continue to read in the data
                while (buffer.IndexOf("Used Space:") <= 0)
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
            if (DownloadProgressEvent != null)
            {
                DownloadProgressEvent(_downloadFileName, bytesWritten);
            }
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
