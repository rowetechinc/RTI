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
 * 10/04/2011      RC                     Removed Codec and put in RecorderManager.
 * 12/29/2011      RC          1.11       Added event for ADCP serial data.
 * 02/10/2012      RC          2.02       Added common commands to the ADCP.
 * 02/29/2012      RC          2.04       Added a try/catch block in ReceiveDataHandler() to catch any issues so the serial port will not disconnect on issues.
 * 03/23/2012      RC          2.07       Added a 100 ms delay after sending the command to set in Compass mode.  This fixes starting calibration.
 * 04/03/2012      RC          2.08       Added XMODEM-CRC download from the ADCP.
 * 04/05/2012      RC          2.08       Check if serial is open and the break state when sending a command.
 * 04/06/2012      RC          2.08       Changed all Thread.Sleep to use WAIT_STATE.
 *                                         Added CancelDownload() to reset settings if download is canceled.
 * 04/11/2012      RC          2.08       Made DownloadCompleteEvent pass a flag if the download was good when complete.
 * 
 */

using System.IO;
using System.Threading;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace RTI
{
    /// <summary>
    /// Extending the SerialConnection class, open a serial connection
    /// to the ADCP.  Then receive the data by overriding the ReceiveDataHandler
    /// method.  Pass the binary data from the serial port to the codec to be parsed
    /// and to the RecorderManager to be recorded to file.
    /// </summary>
    public class AdcpSerialPort : SerialConnection
    {
        #region Variables

        /// <summary>
        /// Time to wait to see if a command
        /// was sent.  If a timeout occurs
        /// it means a command did not get
        /// a response from the ADCP for sending
        /// a command.  Value in milliseconds.
        /// </summary>
        private const int TIMEOUT = 750; 

        /// <summary>
        /// The ADCP can be put into Compass mode
        /// to send and receive data to the internal
        /// compass.  When in Compass mode, the ADCP
        /// data does not have to decoded, but instead
        /// Compass data has to be decoded.
        /// </summary>
        public enum AdcpSerialModes
        {
            /// <summary>
            /// Data received in ADCP mode.
            /// </summary>
            ADCP,

            /// <summary>
            /// Data received in Compass mode.
            /// </summary>
            COMPASS,

            /// <summary>
            /// Data received is saved to a file.
            /// </summary>
            DOWNLOAD
        }

        #region XModem Variables

        // xmodem control characters
        /// <summary>
        /// Start of sending a 128 byte packet.
        /// </summary>
        private const byte SOH = 0x01;

        /// <summary>
        /// Start of sending a 1024 byte packet.
        /// </summary>
        private const byte STX = 0x02;

        /// <summary>
        /// End of transmission.  When reading or
        /// writing is complete.
        /// </summary>
        private const byte EOT = 0x04;

        /// <summary>
        /// Acknowledgement that packet was received
        /// correctly.
        /// </summary>
        private const byte ACK = 0x06;

        /// <summary>
        /// Device Control 1.
        /// </summary>
        private const byte DC1 = 0x11;

        /// <summary>
        /// Command sent when packet did not pass
        /// the checksum and needs to be resent.
        /// </summary>
        private const byte NAK = 0x15;

        /// <summary>
        /// Cancel transmission.
        /// </summary>
        private const byte CAN = 0x18;

        /// <summary>
        /// Control Z.
        /// </summary>
        private const byte CTRLZ = 0x1A;

        /// <summary>
        /// Carrage return.
        /// </summary>
        private const byte CR = 0x0D;

        /// <summary>
        /// Timeout delay for the XModem.  In seconds.
        /// </summary>
        private const int XMODEM_TIMEOUT_DELAY = 1;

        /// <summary>
        /// Number of retries to get a packet
        /// before giving up.
        /// </summary>
        private const int XMODEM_RETRY_LIMIT = 32;//16

        /// <summary>
        /// Which packet number we are currently receiving.
        /// </summary>
        private int _seqNum;

        /// <summary>
        /// Number of bytes that have been written to the
        /// file so far.
        /// </summary>
        private long _byteswritten;

        /// <summary>
        /// Name of the file currently downloading.
        /// </summary>
        private string _fileName;

        /// <summary>
        /// Size of the file currently being downloaded.
        /// </summary>
        private long _fileSize;

        /// <summary>
        /// Flag to parse the downloaded data.
        /// TRUE = Add the data to the database.
        /// FALSE = Just download the data to file.
        /// </summary>
        private bool _parseDownloadedData;

        /// <summary>
        /// Flag to stop sending start messages
        /// to the serial port.  If turned off, the packets are
        /// either being received or a connection could not be
        /// made and the transfer has been aborted.
        /// </summary>
        private bool _startTransfer;

        /// <summary>
        /// This flag is used to stop requesting for the download process
        /// to begin.  To request, 'C' are sent to the ADCP until a response
        /// is given.  This will stop the requesting.
        /// </summary>
        private bool _cancelDownload;

        /// <summary>
        /// Binarywriter to write the download data
        /// to a file.
        /// </summary>
        private BinaryWriter _downloadDataBinWriter;

        /// <summary>
        /// Buffer to hold the incoming data
        /// from the download.
        /// </summary>
        private List<byte> _downloadDataBuffer;

        #endregion

        #endregion

        /// <summary>
        /// Mode the serial port is in.
        /// Either ADCP or COMPASS.
        /// </summary>
        public AdcpSerialModes Mode { get; set; }


        /// <summary>
        /// Constructor
        /// 
        /// </summary>
        /// <param name="adcpSerialOptions"></param>
        public AdcpSerialPort(SerialOptions adcpSerialOptions) :
            base(adcpSerialOptions)
        {
            // Set the default mode to ADCP mode
            Mode = AdcpSerialModes.ADCP;
        }

        /// <summary>
        /// Receive data from the serial port and pass the data to all
        /// who need the data.
        /// </summary>
        /// <param name="rcvData">Data received from the Serial port on seperate thread from UI.</param>
        protected override void ReceiveDataHandler(byte[] rcvData)
        {
            if (rcvData.Length > 0)
            {
                try
                {
                    // Add data to the display
                    // If in compass mode and the compass is outputing a lot of data
                    // The speed the data is coming in is to quick to display
                    SetReceiveBuffer(System.Text.ASCIIEncoding.ASCII.GetString(rcvData));

                    if (Mode == AdcpSerialModes.ADCP)
                    {
                        // Parse the ADCP data
                        if (this.ReceiveAdcpSerialDataEvent != null)
                        {
                            this.ReceiveAdcpSerialDataEvent(rcvData);
                        }
                    }
                    else if (Mode == AdcpSerialModes.COMPASS)
                    {
                        // Parse the Compass data
                        if (this.ReceiveCompassSerialDataEvent != null)
                        {
                            this.ReceiveCompassSerialDataEvent(rcvData);
                        }
                    }
                    else if (Mode == AdcpSerialModes.DOWNLOAD)
                    {
                        ParseDownloadData(rcvData);
                    }
                }
                catch (Exception e)
                {
                    // Do nothing on exception
                    Debug.WriteLine("Receive Serial Data Exception: {0}", e.ToString());
                }
            }
        }

        #region XModem Download

        /// <summary>
        /// Send the command to display the Directory listing.
        /// Then return the results from the serial display.
        /// </summary>
        /// <returns>String of the directory listing.</returns>
        public string GetDirectoryListing()
        {
            // Clear the buffer
            ReceiveBufferString = "";

            if (IsAvailable())
            {
                // Send the command to get the directory listing
                SendData(string.Format("DSDIR \r"));

                // Wait for response
                Thread.Sleep(WAIT_STATE * 2);
            }

            // Return the content of the buffer
            return ReceiveBufferString;
        }

        /// <summary>
        /// Clear the input buffer of any commands.
        /// This will write a new line to the serial
        /// plort.  Any commands previous typed or partionally
        /// typed will then be cleared away.
        /// </summary>
        public void ClearInputBuffer()
        {
            //clear the input bufffer
            if (IsAvailable())
            {
                try
                {
                    SendData("\r");
                    //_serialPort.DiscardInBuffer();
                    //_serialPort.DiscardOutBuffer();
                    System.Threading.Thread.Sleep(WAIT_STATE);

                }
                catch (System.Exception ex)
                {
                    SetReceiveBuffer(String.Format("Exception: {0}", ex.GetType().ToString()));
                }
            }
        }

        /// <summary>
        /// Create the directory where the download file
        /// will be stored.
        /// Return true if the folder was created or already
        /// existed.
        /// </summary>
        /// <param name="dirName">Directory name.</param>
        /// <returns>True = Folder was created or already existed.</returns>
        private bool CreateDirectory(string dirName)
        {
            DirectoryInfo di = new DirectoryInfo(dirName);
            try
            {
                // Determine whether the directory exists.
                if (!di.Exists)
                {
                    // Try to create the directory.
                    di.Create();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

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
                _downloadDataBinWriter.Close();
                _downloadDataBinWriter.Dispose();
                _downloadDataBinWriter = null;
            }
        }

        /// <summary>
        /// If the packet was received and the checksum
        /// was verified, send an ACK to the serial port
        /// so the sender can send the next packet.
        /// This will send the ACK to the serial port.
        /// </summary>
        /// <returns>TRUE = Command sent / FALSE = Command could not be sent.</returns>
        private bool SendACK()
        {
            // Check that the serial port is working and accepting commands
            if (IsAvailable())
            {
                // Write ACK back to serial port
                byte[] buff = new byte[1];
                buff[0] = ACK;
                SendData(buff, 0, 1);

                return true;
            }

            return false;
        }

        /// <summary>
        /// If a packet's checksum fails, send a NAK back to the
        /// serial port to tell it to resend the packet.  This 
        /// will flush the buffer, then send the NAK to the serial 
        /// port.
        /// </summary>
        /// <returns>TRUE = Command sent / FALSE = Command could not be sent.</returns>
        private bool SendNAK()
        {
            // Check that the serial port is working and accepting commands
            if (IsAvailable())
            {
                // Send a NAK to the serial port to
                // tell the serial port to resend the packet.
                SetReceiveBuffer("NAK");
                byte[] buff = new byte[1];
                buff[0] = NAK;
                SendData(buff, 0, 1);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Data Storage XMODEM transmit.  This command is used
        /// to transfer a file, via the serial communication link, from
        /// the SD card contained within the RTI system to an external
        /// device.  File names are limited to a maximum of 8 characters
        /// before the extension.
        /// </summary>
        /// <param name="fileName">Filename to download.</param>
        /// <returns>TRUE = Command sent.  / FALSE = Command could not be sent.</returns>
        private bool SendCommandDSXT(string fileName)
        {
            if (fileName.Length <= 12)
            {
                if (IsAvailable())
                {
                    //return SendDataWaitReply(string.Format("DSXT {0} '\r'", fileName), 5000);
                    SendData(string.Format("DSXT {0} '\r'", fileName));
                    return true;
                }

                return false;
            }
            else
            {
                SetReceiveBuffer("Filename is too large.");
                return false;
            }
        }

        /// <summary>
        /// Verify the STX packet is correct.  This will
        /// check if the sequence number and the checksum are
        /// correct.  The sequence number will the sequence number
        /// in the second byte and 255-seqnum in the third byte.  The
        /// first byte will the STX identifier.  The checksum will be
        /// the last 2 bytes.  This will calculate the checksum.  It will then
        /// return if the packet was good and give the buffer to write to the
        /// file.
        /// </summary>
        /// <param name="buff">Buffer containing the STX packet.</param>
        /// <param name="seqnum">Current sequence number.</param>
        /// <param name="fileBuffer">Buffer created to write to the file.</param>
        /// <returns>TRUE = Good packet.</returns>
        private bool VerifyPacketSTX(byte[] buff, int seqnum, out byte[] fileBuffer)
        {
            // Buffer to hold the data that will be written
            // to the file
            fileBuffer = new byte[1024];

            long crc = 0;
            long pktcrc = 0;
            int j = 3;
            for (int i = 0; i < 1024; i++)
            {
                fileBuffer[i] = buff[j];
                crc = crc ^ (fileBuffer[i] << 8);
                for (int k = 0; k < 8; ++k)
                {
                    long it = crc & 0x8000;
                    if (it == 0x8000)
                        crc = (crc << 1) ^ 0x1021;
                    else
                        crc = crc << 1;
                }
                crc &= 0xFFFF;
                j++;
            }
            pktcrc = (0xFF & buff[j]) << 8;
            pktcrc += 0xFF & buff[j + 1];

            // Verify packet
            if (pktcrc == crc && seqnum == buff[1] && seqnum == (0xFF & (255 - buff[2])))
            {
                return true;
            }

            return false;

        }

        /// <summary>
        /// Check if the phase "READY" was received
        /// by the serial port.  If it was received by 
        /// the serial port, return TRUE.
        /// </summary>
        /// <returns>TRUE = "READY" received by the serial port.</returns>
        private bool CheckForREADY(out long fileSize)
        {
            fileSize = 0;

            if (ReceiveBufferString.Contains("File Size ="))
            {
                // Try to get the file size
                int index = ReceiveBufferString.IndexOf("File Size =");
                string strFileSize = ReceiveBufferString.Substring(index + 12, 10);
                long.TryParse(strFileSize, out fileSize);
                
                // Set the file size
                _fileSize = fileSize;

                Debug.WriteLine("AdcpSerialPort::CheckForReady String: {0}  Value: {1}", strFileSize, fileSize);
            }

            if (ReceiveBufferString.Contains("READY"))
            {
                return true;
            }

            return false;
        }

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
                DownloadCompleteEvent(_fileName, goodDownload);
            }
        }

        /// <summary>
        /// Publish the event of the current progress
        /// of downloading the file.  This will give
        /// the number of bytes currently downloaded.
        /// </summary>
        private void PublishDownloadProgressEvent()
        {
            if (DownloadProgressEvent != null)
            {
                DownloadProgressEvent(_fileName, _byteswritten);
            }
        }

        /// <summary>
        /// Publish the event for the size of the file
        /// that is currently being downloaded.
        /// </summary>
        private void PublishFileSizeEvent()
        {
            if (FileSizeEvent != null)
            {
                FileSizeEvent(_fileName, _fileSize);
            }
        }

        /// <summary>
        /// Stop the download process.  If the download process is complete
        /// or needs to be stopped, then return the ADCP to ADCP mode
        /// and starting reading at a normal interval rate again.
        /// </summary>
        /// <param name="goodDownload">Set flag that the download was good or bad when complete.</param>
        private void CompleteDownload(bool goodDownload)
        {
            // Publish that download is complete for the file
            PublishDownloadCompleteEvent(goodDownload);

            // Set the mode of the ADCP
            // TODO: Should check if it was previously something else
            Mode = AdcpSerialModes.ADCP;

            // Set the interval back to default
            SetTimerInterval();
        }

        /// <summary>
        /// Stop the download process.
        /// This will close the binary writer and
        /// set the serial port back to ADCP mode.
        /// </summary>
        public void CancelDownload()
        {
            // Stop the trying to ask for a download
            _cancelDownload = true;

            // Close the file stream
            CloseBinaryWriter();

            // The DSDIR command will cancel any pending downloads
            SendData(string.Format("D \r"));

            // Set the mode of the ADCP
            // TODO: Should check if it was previously something else
            Mode = AdcpSerialModes.ADCP;

            // Set the interval back to default
            SetTimerInterval();
        }

        /// <summary>
        /// Download the file from the serial device using XMODE-CRC.
        /// </summary>
        /// <param name="dirName">Directory Name to store the files.</param>
        /// <param name="fileName">Filename to download.</param>
        /// <param name="parseData">Flag to parse and record data to the database.</param>
        public void XModemDownload(string dirName, string fileName, bool parseData)
        {
            // Set the mode of the ADCP
            Mode = AdcpSerialModes.DOWNLOAD;
            _parseDownloadedData = parseData;

            // Increase the time to check the serial port
            SetTimerInterval(DOWNLOAD_SERIAL_INTERVAL);

            _seqNum = 2;
            _byteswritten = 0;
            _fileName = fileName;
            _downloadDataBuffer = new List<byte>();

            bool C_ACK = false;
            _startTransfer = false;
            _cancelDownload = false;
            bool XmodemCancel = false;
            byte[] buff = new byte[4];
            //byte[] bBuff = new byte[140000];
            //byte[] Dbuff = new byte[1024 + 5];

            long elapsedTicks;
            DateTime currentDate = DateTime.Now;
            long lastTicks = currentDate.Ticks;
            int retry = XMODEM_RETRY_LIMIT;


            // Clear the input buffer of any previous commands
            // and clear the Receive Buffer
            ClearInputBuffer();
            ReceiveBufferString = "";

            // Create the file path
            string path = dirName + "\\" + fileName;

            try
            {
                // If we are not parsing the data, then 
                // we will record the data here.  If we are
                // parsing the data, then the parser will also
                // record the data.
                if (!_parseDownloadedData)
                {
                    // Create a binarywriter to write the data to the file
                    _downloadDataBinWriter = CreateBinaryWriter(path);
                }

                // Ensure the serial port is open and accepting commands
                if (IsAvailable())
                {
                    {
                        // Send the command to download the file
                        // from the instrument
                        if (!SendCommandDSXT(fileName))
                        {
                            // Command could not be sent
                            // so do not continue
                            return;
                        }

                        elapsedTicks = 0;
                        currentDate = DateTime.Now;
                        lastTicks = currentDate.Ticks;
                        XmodemCancel = false;

                        // The response from the command is:
                        // "File Size = 0000000000"
                        // "READY"
                        // Wait for READY then begin
                        // If we never receive a READY,
                        // Complete the download for this file
                        long filesize = 0;
                        bool receivedREADY = CheckForREADY(out filesize);
                        while (!XmodemCancel && !receivedREADY && elapsedTicks < 100000000)
                        {
                            receivedREADY = CheckForREADY(out filesize);
                            currentDate = DateTime.Now;
                            elapsedTicks = currentDate.Ticks - lastTicks;
                        }

                        if (receivedREADY)
                        {
                            // Publish that a file size was found
                            PublishFileSizeEvent();

                            XmodemCancel = false;
                            while (!_startTransfer && !_cancelDownload)
                            {
                                try
                                {
                                    currentDate = DateTime.Now;
                                    elapsedTicks = currentDate.Ticks - lastTicks;

                                    // Wait 1 second
                                    if (elapsedTicks > 10000000)// 1 second
                                    {
                                        lastTicks = currentDate.Ticks;

                                        // Send a Capital C character instead of a NAK to start
                                        // the transfer using XMODEM-CRC (16-Bit CRC instead of 8-Bit CRC)
                                        // If the sender responds by sending a packet, it is assumed the
                                        // sender knows XMODEM-CRC.  If no packet is sent, then assumed
                                        // the received does not know and fall back to traditional XMODEM.
                                        // Try at least 4 times to send C just in case one of the C was sent
                                        // bad.
                                        if (!C_ACK)
                                        {
                                            SetReceiveBuffer("C");
                                            buff[0] = (byte)'C';
                                            SendData(buff, 0, 1);
                                        }
                                        // If not going to use XMODEM-CRC, then send
                                        // a NAK to start the file transfer
                                        else
                                        {
                                            SetReceiveBuffer("N");
                                            buff[0] = NAK;
                                            SendData(buff, 0, 1);
                                        }

                                        retry--;
                                        if (retry < 1)
                                            XmodemCancel = true;
                                    }
                                }
                                catch { }
                                if (XmodemCancel)
                                {
                                    //CancelXmodem();
                                    _startTransfer = true;
                                }
                            }
                        }
                        else
                        {
                            // The file could not be started
                            // So complete the download
                            CompleteDownload(false);
                        }
                    }
                }
            }

            catch(Exception e)
            {
                //OK = false;
                Debug.WriteLine("Exception: {0}", e.ToString());
                // Close the file stream
                CloseBinaryWriter();
            }
        }
        
        /// <summary>
        /// Parse the packet received from the serial port.
        /// The system is in Download mode, so the serial port
        /// is sending packets of data in XMODEM format.
        /// This will determine what type of packet is received
        /// and handle the packet.
        /// </summary>
        /// <param name="data">Data received from the serial port.</param>
        private void ParseDownloadData(byte[] data)
        {
            // Check for end of transfer
            // 20 is the length for the message
            // "<EOT> Transfer Complete"
            if (data.Length == 20 && data[0] == EOT)
            {
                // Close the file stream
                CloseBinaryWriter();

                // Stop the Download process
                CompleteDownload(true);

                return;
            }

            // Add the data to a buffer.
            // This is to wait until a complete packet has arrived
            _downloadDataBuffer.AddRange(data);

            // If there is not enough data in the buffer
            // wait for more data
            if (_downloadDataBuffer.Count < 1024 + 5)
            {
                return;
            }

            int retry = XMODEM_RETRY_LIMIT;
            int slowdown = 0;

            // If there is enough data, set it to an array
            // and process the data
            byte[] bBuff = _downloadDataBuffer.ToArray();
            _downloadDataBuffer.Clear();

            if (bBuff.Length > 0)
            {
                // Find the type of packet
                switch (bBuff[0])
                {
                    default:
                        SetReceiveBuffer("I");
                        break;
                    case CAN:
                        break;
                    case SOH:
                        break;
                    case STX:
                        // Stop the loop from requesting the
                        // start of data
                        _startTransfer = true;

                        // If all bytes were read
                        // Verify the packet
                        if (bBuff.Length == 1024 + 5)//got one!
                        {
                            // Verify the packet and get a buffer to write 
                            // to the file
                            byte[] Fbuff;
                            if (VerifyPacketSTX(bBuff, _seqNum, out Fbuff))
                            {
                                // Set how many bytes have been written
                                _byteswritten += Fbuff.Length;

                                // Add data to codec to parse and
                                // add to the database
                                if (_parseDownloadedData)
                                {
                                    // Parse and write the data to the database and file
                                    this.ReceiveAdcpSerialDataEvent(Fbuff);
                                }
                                else
                                {
                                    try
                                    {
                                        // Write the data to the file only
                                        _downloadDataBinWriter.Write(Fbuff);
                                    }
                                    catch (Exception) { }
                                }

                                // Write ACK back to serial port
                                // So the next packet will be sent
                                if (!SendACK())
                                {
                                    // If command not sent properly
                                    // try again.  If still not sent
                                    // properly, there may be an issue
                                    // with the serial port and stop 
                                    // trying to download
                                    if (!SendACK())
                                    {
                                        break;
                                    }
                                }

                                // Publish the event of the current
                                // progress of the file download
                                PublishDownloadProgressEvent();

                                // Used to slow down the reading
                                // It may be to fast for the serial
                                // port to handle
                                if (slowdown > 0)
                                {
                                    System.Threading.Thread.Sleep(slowdown);
                                }

                                // Reset retry
                                retry = XMODEM_RETRY_LIMIT;

                                // Set the sequence number
                                _seqNum++;
                                if (_seqNum > 255)
                                    _seqNum = 0;
                            }
                            else
                            {
                                // Send NAK that data was not received properly
                                if (!SendNAK())
                                {
                                    // If command not sent properly
                                    // try again.  If still not sent
                                    // properly, there may be an issue
                                    // with the serial port and stop 
                                    // trying to download
                                    if (!SendNAK())
                                    {
                                        break;
                                    }
                                }

                                System.Threading.Thread.Sleep(slowdown);

                                // Maybe we are not waiting long enough
                                // for the complete packet to arrive
                                slowdown += 10;
                                if (slowdown > 150)
                                    slowdown = 150;

                                // Limit the number of times to get the 
                                // packet if it fails
                                retry--;
                                if (retry < 1)
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // Send NAK that data was not received properly
                            if (!SendNAK())
                            {
                                // If command not sent properly
                                // try again.  If still not sent
                                // properly, there may be an issue
                                // with the serial port and stop 
                                // trying to download
                                if (!SendNAK())
                                {
                                    break;
                                }
                            }

                            // Limit the number of times to get the 
                            // packet if it fails
                            retry--;
                            if (retry < 1)
                            {
                                break;
                            }
                        }
                        break;
                    case EOT:
                        // Close the file stream
                        CloseBinaryWriter();

                        // Set the mode of the ADCP
                        // TODO: Should check if it was previously something else
                        Mode = AdcpSerialModes.ADCP;

                        break;
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
        /// <returns>TRUE = All commands sent. FALSE = 1 or more commands could not be sent.</returns>
        public bool StartPinging()
        {
            bool timeResult = false;
            bool pingResult = false;

            // Try to send the command, if it fails try again
            timeResult = SendDataWaitReply(RTI.Commands.AdcpCommands.GetTimeCommand(), TIMEOUT);
            if (!timeResult)
            {
                timeResult = SendDataWaitReply(RTI.Commands.AdcpCommands.GetTimeCommand(), TIMEOUT);
            }

            // Allow some time for the ADCP to set the time
            System.Threading.Thread.Sleep(WAIT_STATE * 2);

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

        #region Compass Serial Commands

        /// <summary>
        /// Start Compass mode.  This make the ADCP output
        /// compass data instead of ADCP data.
        /// </summary>
        /// <returns>TRUE if command sent properly.</returns>
        public bool StartCompassMode()
        {
            bool startResult = false;

            // Try to send the command, if it fails try again
            startResult = SendDataWaitReply(RTI.Commands.AdcpCommands.CMD_DIAGCPT, TIMEOUT);
            
            // Delay for 485 response
            //Thread.Sleep(WAIT_STATE);

            // If the command was not good, try it again
            if (!startResult)
            {
                startResult = SendDataWaitReply(RTI.Commands.AdcpCommands.CMD_DIAGCPT, TIMEOUT);

                // Delay for 485 response
                //Thread.Sleep(WAIT_STATE);
            }

            // If was capable of putting 
            // into compass mode, change the mode
            if (startResult)
            {
                // Set the serial port to COMPASS mode to decode compass data
                Mode = AdcpSerialPort.AdcpSerialModes.COMPASS;
            }

            // Return if either failed
            return startResult;
        }

        /// <summary>
        /// Stop the ADCP from being in Compass mode.
        /// This will set the compass to interval mode, 
        /// then set the ADCP back to ADCP mode.
        /// </summary>
        public void StopCompassMode()
        {
            // Turn on compass interval outputing
            byte[] startIntervalCmd = PniPrimeCompassBinaryCodec.StartIntervalModeCommand();
            SendData(startIntervalCmd, 0, startIntervalCmd.Length);

            // Delay for 485 response
            //Thread.Sleep(100);

            // Stop ADCP from compass mode
            SendData(RTI.Commands.AdcpCommands.CMD_DIAGCPT_DISCONNECT);

            // Delay for 485 response
            Thread.Sleep(WAIT_STATE);

            // Set the serial port to ADCP mode to decode ADCP data
            Mode = AdcpSerialPort.AdcpSerialModes.ADCP;
        }

        /// <summary>
        /// Start compass calibration.  Set flag whether to do 
        /// magnitude and acceleration calibration or just magnitude
        /// calibration.
        /// </summary>
        /// <param name="IsMagAndAccelCalibration">Flag if calibrate both Mag and Accel.</param>
        /// <returns>TRUE = Compass Cal started.</returns>
        public bool StartCompassCal(bool IsMagAndAccelCalibration)
        {
            // If not in compass mode, then put in compass mode
            if (Mode != AdcpSerialPort.AdcpSerialModes.COMPASS)
            {
                if (!StartCompassMode())
                {
                    return false;
                }
            }

            // Stop compass from outputing data
            byte[] stopIntervalCmd = PniPrimeCompassBinaryCodec.StopIntervalModeCommand();
            SendData(stopIntervalCmd, 0, stopIntervalCmd.Length);

            //Thread.Sleep(100);                  // Delay for 485 response

            // Start compass in calibration mode
            byte[] startCalCmd;
            if (IsMagAndAccelCalibration)
            {
                startCalCmd = PniPrimeCompassBinaryCodec.StartCalibrationCommand(PniPrimeCompassBinaryCodec.CalMode.CAL_ACCEL_AND_MAG);
            }
            else
            {
                startCalCmd = PniPrimeCompassBinaryCodec.StartCalibrationCommand(PniPrimeCompassBinaryCodec.CalMode.CAL_MAG_ONLY);
            }
            SendData(startCalCmd, 0, startCalCmd.Length);

            return true;
        }

        /// <summary>
        /// Stop a calibration process.  This is used
        /// if the user would like to restart a compass calibration.
        /// </summary>
        public void StopCompassCal()
        {
            // Stop Calibration mode
            byte[] stopCalCmd = PniPrimeCompassBinaryCodec.StopCalibrationCommand();
            SendData(stopCalCmd, 0, stopCalCmd.Length);

            // Delay for 485 response
            Thread.Sleep(WAIT_STATE);
        }

        /// <summary>
        /// Save the compass calibration to the compass.
        /// The ADCP must be in compass mode to accomplish this.
        /// </summary>
        public void SaveCompassCal()
        {
            if (Mode == AdcpSerialModes.COMPASS)
            {
                // Stop Calibration mode
                byte[] SaveCalDataCmd = PniPrimeCompassBinaryCodec.SaveCompassCalCommand();
                SendData(SaveCalDataCmd, 0, SaveCalDataCmd.Length);
                Thread.Sleep(WAIT_STATE);
            }
        }

        /// <summary>
        /// Send the given command to the ADCP.  This will then be 
        /// sent to the compass.  This will only be sent to the
        /// compass, if the ADCP is in compass mode.
        /// </summary>
        /// <returns>TRUE = Command sent.  / False = Command NOT sent.  Most likely not in Compass mode.</returns>
        public bool SendCompassCommand(byte[] command)
        {
            if (Mode == AdcpSerialModes.COMPASS)
            {
                SendData(command, 0, command.Length);
                Thread.Sleep(WAIT_STATE);
                return true;
            }

            return false;
        }

        #endregion

        #region Configuration Serial Commands

        /// <summary>
        /// Send the command to set the default values to the ADCP.
        /// </summary>
        /// <returns>TRUE = All commands sent. FALSE = 1 or more commands could not be sent.</returns>
        public bool SetDefaults()
        {
            bool defaultResult = false;

            // Send the command within the buffer
            // Try sending the command.  If it fails try one more time
            defaultResult = SendDataWaitReply(RTI.Commands.AdcpCommands.CMD_CDEFAULT, TIMEOUT);
            if (!defaultResult)
            {
                // Try again if failed first time
                defaultResult = SendDataWaitReply(RTI.Commands.AdcpCommands.CMD_CDEFAULT, TIMEOUT);
            }

            return defaultResult;
        }

        /// <summary>
        /// Save the configuration to the SD card.
        /// </summary>
        /// <returns>TRUE = All commands sent. FALSE = 1 or more commands could not be sent.</returns>
        public bool SaveConfigurations()
        {

            bool saveResult = false;

            // Send the command within the buffer
            // Try sending the command.  If it fails try one more time
            saveResult = SendDataWaitReply(RTI.Commands.AdcpCommands.CMD_CSAVE, TIMEOUT);
            if (!saveResult)
            {
                // Try again if failed first time
                saveResult = SendDataWaitReply(RTI.Commands.AdcpCommands.CMD_CSAVE, TIMEOUT);
            }

            return saveResult;
        }

        #endregion

        #region Events

        #region ADCP Data Event

        /// <summary>
        /// Event To subscribe to.  This gives the paramater
        /// that will be passed when subscribing to the event.
        /// </summary>
        /// <param data="ensemble">Byte array of data received from the port.</param>
        public delegate void ReceiveAdcpSerialDataEventHandler(byte[] data);

        /// <summary>
        /// Subscribe to receive event when data has been successfully
        /// processed.  This can be used to tell if data is in this format
        /// and is being processed or is not in this format.
        /// Subscribe to this event.  This will hold all subscribers.
        /// 
        /// To subscribe:
        /// _adcpSerialPort.ReceiveAdcpSerialDataEvent += new serialConnection.ReceiveAdcpSerialDataEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// _adcpSerialPort.ReceiveAdcpSerialDataEvent -= (method to call)
        /// </summary>
        public event ReceiveAdcpSerialDataEventHandler ReceiveAdcpSerialDataEvent;

        #endregion

        #region Compass Data Event

        /// <summary>
        /// Event To subscribe to.  This gives the paramater
        /// that will be passed when subscribing to the event.
        /// </summary>
        /// <param data="ensemble">Byte array of data received from the port.</param>
        public delegate void ReceiveCompassSerialDataEventHandler(byte[] data);

        /// <summary>
        /// Subscribe to receive event when data has been successfully
        /// processed.  This can be used to tell if data is in this format
        /// and is being processed or is not in this format.
        /// Subscribe to this event.  This will hold all subscribers.
        /// 
        /// To subscribe:
        /// _adcpSerialPort.ReceiveCompassSerialDataEvent += new serialConnection.ReceiveCompassSerialDataEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// _adcpSerialPort.ReceiveCompassSerialDataEvent -= (method to call)
        /// </summary>
        public event ReceiveCompassSerialDataEventHandler ReceiveCompassSerialDataEvent;

        #endregion

        #region Download Events

        #region File Size Event

        /// <summary>
        /// Event to get the file size for the given
        /// file name.  The size will be in bytes.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileSize">Size of the file in bytes.</param>
        public delegate void FileSizeEventHandler(string fileName, long fileSize);

        /// <summary>
        /// Subscribe to receive the event for the size of the file in bytes.
        /// 
        /// To subscribe:
        /// _adcpSerialPort.FileSizeEvent += new serialConnection.FileSizeEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// _adcpSerialPort.FileSizeEvent -= (method to call)
        /// </summary>
        public event FileSizeEventHandler FileSizeEvent;

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

        #endregion

        #endregion

        #endregion

    }
}