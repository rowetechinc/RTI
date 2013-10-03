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
 * 04/16/2012      RC          2.09       Added Upload and modified Download.
 *                                         Changed name of FileSizeEvent to DownloadFileSizeEvent.
 *                                         Added Upload events.
 *                                         Added WAIT_STATE after the upload command is sent.
 * 04/26/2012      RC          2.10       Changed GetDirectoryListing() to return a list of files.
 *                                         Make GetDirectoryListing() wait for all the lines before parsing the data.
 *                                         XModemDownload return true if file was downloaded.
 *                                         Fix cancel download to actually cancel download all the time.
 * 05/03/2012      RC          2.11       Changed TIMEOUT value.
 * 05/22/2012      RC          2.11       Added ability to cancel uploading data.
 * 06/26/2012      RC          2.12       Created SendCommands() to send a list of commands.
 * 07/03/2012      RC          2.12       Created SysTestFirmwareFiles() to test for missing firmware files.
 * 07/06/2012      RC          2.12       Check if the command given is null in SendCompassCommand().
 *                                         Created SysTestCompass() to test is compass is giving data.
 * 07/17/2012      RC          2.12       Added SysTestI2cMemoryDevices() to test boards and registers in the system.
 *                                         Added SetSystemTime() to set the system time.  Used in StartPinging().
 * 07/18/2012      RC          2.12       Added SysTestSingleWaterProfilePing() and SysTestSingleBottomTrackPing() to test the Status of the system.
 *                                         Changed SysTestFirmwareVersion() to check if the firmware version is less then given version.
 *                                         Changed SysTestI2cMemoryDevices() to check for bad serial and revision if not testing for a specific serial and revision.
 * 07/30/2012      RC          2.13       Reduced the timeout in DownloadDirectoryListing().
 * 08/24/2012      RC          2.13       Added test to check for maint.txt and EngHelp.txt in SysTestFirmwareFiles().
 * 08/31/2012      RC          2.15       Added high speed xmodem downloading.
 * 09/04/2012      RC          2.15       In SysTestFirmwareFiles(), made it check for all capitals for the file MAINT.txt and ENGHELP.txt.
 * 10/18/2012      RC          2.15       When canceling or stopping a download, send the D command to stop the current download.
 * 10/19/2012      RC          2.15       Remove the Download watchdog.
 * 12/11/2012      RC          2.17       In SetSystemDateTime(), wait about 5 seconds after setting the ADCP time.
 * 01/23/2013      RC          2.17       Added the Reboot() command.
 * 04/30/2013      RC          2.19       Added ability to scan for serial ports with an ADCP connected.
 * 05/03/2013      RC          2.19       Fixed BUG in ScanSerialConnection() where the serial connection was not shutdown after scanning and cause the read thread to remain alive.
 * 05/06/2013      RC          2.19       Added the class AdcpSerialOptions.  Make the ScanSerialConnection() return a list of AdcpSerialOptions.
 * 05/17/2013      RC          2.19       Added ToString().
 *                                         Changed SetSystemDateTime() to SetLocalSystemDateTime().  Added SetUtcSystemDateTime().
 * 06/28/2013      RC          2.19       Replaced Shutdown() with IDisposable.
 * 07/31/2013      RC          2.19.3     Add flag properties to know what mode the ADCP connection is in.
 * 08/14/2013      RC          2.19.4     Added a wait state in TestSerialBaudConnection() to allow the BREAK response get received.
 * 08/28/2013      RC          2.19.5     In TestSerialBaudConnection(), limited the baud rates to test.
 * 09/17/2013      RC          2.20.0     Added another StartPinging() that does not set the time also.
 * 09/25/2013      RC          2.20.1     In TestSerialBaudConnection(), check if the serial port is open and disconnect if it is to prevent exception.  Change the timeout for the response.
 * 09/30/2013      RC          2.20.1     Fixed bug in GetAdcpConfiguration() where the serial options were not set and lost by the constructor.
 * 
 */

using System.IO;
using System.Threading;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Globalization;
using RTI.Commands;
using log4net;
using System.Timers;

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
        #region Enums and Classes

        #region AdcpSerialModes

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
            DOWNLOAD,

            /// <summary>
            /// Send a file to the ADCP.
            /// </summary>
            UPLOAD
        }

        #endregion

        #region AdcpSerialOptions

        /// <summary>
        /// This class of options will include the
        /// serial options and any additional 
        /// options that are particular to an ADCP.
        /// This includes serial numbers, ...
        /// </summary>
        public class AdcpSerialOptions
        {

            #region Properties

            /// <summary>
            /// Serial options for the ADCP.
            /// </summary>
            public SerialOptions SerialOptions { get; set; }

            /// <summary>
            /// Serial number for the ADCP.
            /// </summary>
            public SerialNumber SerialNumber { get; set; }

            /// <summary>
            /// String of all the hardware.
            /// </summary>
            public string Hardware { get; set; }

            /// <summary>
            /// Firmware version.
            /// </summary>
            public Firmware Firmware { get; set; }

            #endregion

            /// <summary>
            /// Initialize all the values.
            /// </summary>
            public AdcpSerialOptions()
            {
                SerialOptions = new SerialOptions();
                SerialNumber = new SerialNumber();
                Hardware = "";
                Firmware = new Firmware();
            }

            #region Override

            /// <summary>
            /// Display the object to a string.
            /// </summary>
            /// <returns>String of the object.</returns>
            public override string ToString()
            {
                string result = SerialNumber.ToString() + ";";
                result += " " + SerialOptions.ToString() + ";";
                result += " FW: " + Firmware.ToString() + ";";
                result += " HW: " + Hardware;

                return result;
            }

            /// <summary>
            /// Hashcode for the object.
            /// This will return the hashcode for the
            /// this object's string.
            /// </summary>
            /// <returns>Hashcode for the object.</returns>
            public override int GetHashCode()
            {
                return ToString().GetHashCode();
            }

            /// <summary>
            /// Determine if the given object is equal to this
            /// object.  This will check if the Status Value match.
            /// </summary>
            /// <param name="obj">Object to compare with this object.</param>
            /// <returns>TRUE = Status Value matched.</returns>
            public override bool Equals(object obj)
            {
                //Check for null and compare run-time types.
                if (obj == null || GetType() != obj.GetType()) return false;

                AdcpSerialOptions p = (AdcpSerialOptions)obj;

                return SerialOptions == p.SerialOptions &&
                        SerialNumber == p.SerialNumber &&
                        Hardware == p.Hardware &&
                        Firmware == p.Firmware;
            }

            /// <summary>
            /// Determine if the two AdcpConnectionListItemViewModel Value given are the equal.
            /// </summary>
            /// <param name="option1">First AdcpSerialOptions to check.</param>
            /// <param name="option2">AdcpSerialOptions to check against.</param>
            /// <returns>True if there options match.</returns>
            public static bool operator ==(AdcpSerialOptions option1, AdcpSerialOptions option2)
            {
                // If both are null, or both are same instance, return true.
                if (System.Object.ReferenceEquals(option1, option2))
                {
                    return true;
                }

                // If one is null, but not both, return false.
                if (((object)option1 == null) || ((object)option2 == null))
                {
                    return false;
                }

                // Return true if the fields match:
                return option1.SerialOptions == option2.SerialOptions &&
                        option1.SerialNumber == option2.SerialNumber &&
                        option1.Hardware == option2.Hardware &&
                        option1.Firmware == option2.Firmware;
            }

            /// <summary>
            /// Return the opposite of ==.
            /// </summary>
            /// <param name="option1">First AdcpConnectionListItemViewModel to check.</param>
            /// <param name="option2">AdcpConnectionListItemViewModel to check against.</param>
            /// <returns>Return the opposite of ==.</returns>
            public static bool operator !=(AdcpSerialOptions option1, AdcpSerialOptions option2)
            {
                return !(option1 == option2);
            }

            #endregion
        }

        #endregion

        #endregion

        #region Variables

        // Setup logger
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Time to wait to see if a command
        /// was sent.  If a timeout occurs
        /// it means a command did not get
        /// a response from the ADCP for sending
        /// a command.  Value in milliseconds.
        /// </summary>
        private const int TIMEOUT = 2000; 

        #region XModem Variables

        /// <summary>
        /// Buffer size for standard xmodem download.
        /// </summary>
        private const int DEFAULT_DL_BUFF_SIZE = 1024;

        /// <summary>
        /// Buffer size for the High speed xmodem download.
        /// </summary>
        private const int DEFAULT_HSDL_BUFF_SIZE = 16384;

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
        private bool _cancelWaitForDownload;

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

        /// <summary>
        /// Flag if the download should be done in high speed mode.
        /// This is a flag now because it is still be experimented if
        /// it works correctly.
        /// </summary>
        private bool _isHighSpeedDownload;

        #endregion

        /// <summary>
        /// A string to hold the directory listing
        /// when trying to view all the files.
        /// </summary>
        private string _dirListingString;

        #endregion

        #region Properties

        /// <summary>
        /// Mode the serial port is in.
        /// Either ADCP or COMPASS.
        /// </summary>
        public AdcpSerialModes Mode { get; set; }

        /// <summary>
        /// Flag if the ADCP is in ADCP mode.
        /// This is the standard mode.
        /// </summary>
        public bool IsAdcpMode
        {
            get { return Mode == AdcpSerialModes.ADCP; }
        }

        /// <summary>
        /// Flag if the ADCP is in compass mode.
        /// </summary>
        public bool IsCompassMode
        {
            get { return Mode == AdcpSerialModes.COMPASS; }
        }

        /// <summary>
        /// Flag if the ADCP is in Download mode.
        /// </summary>
        public bool IsDownloadMode
        {
            get { return Mode == AdcpSerialModes.DOWNLOAD; }
        }

        /// <summary>
        /// Flag if the ADCP is in Upload mode.
        /// </summary>
        public bool IsUploadMode
        {
            get { return Mode == AdcpSerialModes.UPLOAD; }
        }

        #endregion

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
        /// Shutdown the object.
        /// </summary>
        protected override void SubDispose()
        {
            // Close any previous binary writers
            CloseBinaryWriter();
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
                    else if (Mode == AdcpSerialModes.UPLOAD)
                    {
                        // Do Nothing
                        Debug.WriteLine("Upload data received.");
                    }
                }
                catch (Exception e)
                {
                    // Do nothing on exception
                    Debug.WriteLine("Receive Serial Data Exception: {0}", e.ToString());
                }
            }
        }

        #region Reboot

        /// <summary>
        /// Reboot the ADCP.  Give the command SLEEP, then a BREAK
        /// then STOP.
        /// </summary>
        public void Reboot()
        {
            if (IsAvailable())
            {
                // Put the ADCP to Sleep
                SendData("SLEEP");

                // Wait for it to sleep
                Thread.Sleep(AdcpSerialPort.WAIT_STATE * 2);
                
                // Send the BREAK and STOP using StopPinging
                StopPinging();
            }
        }

        #endregion

        #region XModem Download



        /// <summary>
        /// Clear the input buffer of any commands.
        /// This will write a new line to the serial
        /// port.  Any commands previous typed or partionally
        /// typed will then be cleared away.
        /// </summary>
        public void ClearInputBuffer()
        {
            //clear the input bufffer
            if (IsAvailable())
            {
                try
                {
                    SendData("");
                }
                catch (System.Exception ex)
                {
                    SetReceiveBuffer(String.Format("Exception: {0}", ex.GetType().ToString()));
                    log.Error("Error Clearing Serial Port Input Buffer", ex);
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
        private bool SendCommandDSX(string fileName)
        {
            if (fileName.Length <= 12)
            {
                if (IsAvailable())
                {
                    // Determine which command to use, standard or High speed download.
                    string command = RTI.Commands.AdcpCommands.CMD_DSXT;
                    if (_isHighSpeedDownload)
                    {
                        command = RTI.Commands.AdcpCommands.CMD_DSXD;
                    }


                    //return SendDataWaitReply(string.Format("DSXT {0} '\r'", fileName), 5000);
                    SendData(string.Format("{0}{1}", command, fileName));
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
        /// correct.  The sequence number is
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
            // Set the buffer size based off in high speed mode or not
            int bufferSize = DEFAULT_DL_BUFF_SIZE;
            if (_isHighSpeedDownload)
            {
                bufferSize = DEFAULT_HSDL_BUFF_SIZE;
            }

            // Buffer to hold the data that will be written
            // to the file
            fileBuffer = new byte[bufferSize];

            // Bad buffer given, to small
            if (buff.Length < bufferSize)
            {
                return false;
            }

            long crc = 0;
            long pktcrc = 0;
            int j = 3;
            for (int i = 0; i < bufferSize; i++)
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
            }

            if (ReceiveBufferString.Contains("READY"))
            {
                return true;
            }

            return false;
        }

        #region Download

        #region Download Events

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
            if (DownloadFileSizeEvent != null)
            {
                DownloadFileSizeEvent(_fileName, _fileSize);
            }
        }

        #endregion

        #region Complete/Cancel Download


        /// <summary>
        /// Stop the download process.  If the download process is complete
        /// or needs to be stopped, then return the ADCP to ADCP mode
        /// and starting reading at a normal interval rate again.
        /// </summary>
        /// <param name="goodDownload">Set flag that the download was good or bad when complete.</param>
        private void CompleteDownload(bool goodDownload)
        {
            // The D command will cancel any pending downloads
            // Send it twice to first ignore the last packet sent, then
            // stop the download process
            SendData(string.Format("{0}", RTI.Commands.AdcpCommands.CMD_DS_CANCEL));
            SendData(string.Format("{0}", RTI.Commands.AdcpCommands.CMD_DS_CANCEL));

            // Publish that download is complete for the file
            PublishDownloadCompleteEvent(goodDownload);

            // Stop the download watchdog
            //StopDownloadWatchDog();

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

            // Stop the download watchdog
            //StopDownloadWatchDog();

            // Close the file stream
            CloseBinaryWriter();

            // The D command will cancel any pending downloads
            // Send it twice to first ignore the last packet sent, then
            // stop the download process
            SendData(string.Format("{0}", RTI.Commands.AdcpCommands.CMD_DS_CANCEL));
            SendData(string.Format("{0}", RTI.Commands.AdcpCommands.CMD_DS_CANCEL));

            // Set the mode of the ADCP
            // TODO: Should check if it was previously something else
            Mode = AdcpSerialModes.ADCP;

            // Set the interval back to default
            SetTimerInterval();
        }

        #endregion

        /// <summary>
        /// Download the file from the serial device using XMODE-CRC.
        /// Set parseData to determine what to do with the downloaded data.
        /// 
        /// If parseData is TRUE:
        /// The data will be published to the event: ReceiveAdcpSerialDataEvent.
        /// If parseData is FALSE:
        /// The data will be written to the file: dirName\fileName.
        /// </summary>
        /// <param name="dirName">Directory Name to store the files.</param>
        /// <param name="fileName">Filename to download.</param>
        /// <param name="parseData">Flag to parse and record data to the database. Default = FALSE.</param>
        /// <param name="isHighSpeed">Flag if we should download in high speed mode or not.  Default = TRUE.</param>
        /// <returns>TRUE = file successful download / FALSE = File not downloaded.</returns>
        public bool XModemDownload(string dirName, string fileName, bool parseData = false, bool isHighSpeed = true)
        {
            // Set the mode of the ADCP
            Mode = AdcpSerialModes.DOWNLOAD;
            _parseDownloadedData = parseData;
            _isHighSpeedDownload = isHighSpeed;

            // Clear the input buffer of any previous commands
            // and clear the Receive Buffer
            ClearInputBuffer();
            ReceiveBufferString = "";

            // Increase the time to check the serial port
            SetTimerInterval(DOWNLOAD_SERIAL_INTERVAL);

            _seqNum = 1;
            _byteswritten = 0;
            _fileName = fileName;
            _downloadDataBuffer = new List<byte>();

            bool C_ACK = false;
            _cancelWaitForDownload = false;
            _cancelDownload = false;
            bool xmodemCancel = false;
            byte[] buff = new byte[4];
            //byte[] bBuff = new byte[140000];
            //byte[] Dbuff = new byte[1024 + 5];

            long elapsedTicks;
            DateTime currentDate = DateTime.Now;
            long lastTicks = currentDate.Ticks;
            int retry = XMODEM_RETRY_LIMIT;


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
                        // It will choose if we are downloading in high speed mode or not
                        if (!SendCommandDSX(fileName))
                        {
                            // Command could not be sent
                            // so do not continue
                            return false;
                        }

                        elapsedTicks = 0;
                        currentDate = DateTime.Now;
                        lastTicks = currentDate.Ticks;
                        xmodemCancel = false;

                        // The response from the command is:
                        // "File Size = 0000000000"
                        // "READY"
                        // Wait for READY then begin
                        // If we never receive a READY,
                        // Complete the download for this file
                        long filesize = 0;
                        bool receivedREADY = CheckForREADY(out filesize);
                        while (!xmodemCancel && !receivedREADY && elapsedTicks < 100000000)
                        {
                            receivedREADY = CheckForREADY(out filesize);
                            currentDate = DateTime.Now;
                            elapsedTicks = currentDate.Ticks - lastTicks;
                        }

                        // READY was received from the ADCP
                        if (receivedREADY)
                        {
                            // Publish that a file size was found
                            PublishFileSizeEvent();

                            _downloadDataBuffer.Clear();

                            // Start the watchdog to monitor for a hanging download
                            //StartDownloadWatchDog();

                            xmodemCancel = false;

                            // Set the timeout to wait for data to come in
                            // 10000000 = 1 second
                            long TO = 10000000;
                            if (_isHighSpeedDownload)
                            {
                                switch (_serialOptions.BaudRate)
                                {
                                    default:
                                        TO = 100000000;
                                        break;
                                    case 921600:
                                        TO = 20000000;
                                        break;
                                    case 115200:
                                        TO = 40000000;
                                        break;
                                }
                            }

                            // _startTransfer is set set true when the incoming data is parsed
                            // and an STX packet is valid in ParseDownloadData()
                            // _cancelDownload is set true if the user tries to cancel the download
                            // When the serial port receives data, it will call the event handler.
                            // The event handler will then process the packets for file data in ParseDownloadData().
                            while (!_cancelWaitForDownload && !_cancelDownload)
                            {
                                try
                                {
                                    currentDate = DateTime.Now;
                                    elapsedTicks = currentDate.Ticks - lastTicks;

                                    // Wait for data
                                    if (elapsedTicks > TO)
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
                                            buff[0] = (byte)'C';
                                            SendData(buff, 0, 1);
                                        }
                                        // If not going to use XMODEM-CRC, then send
                                        // a NAK to start the file transfer
                                        else
                                        {
                                            buff[0] = NAK;
                                            SendData(buff, 0, 1);
                                        }

                                        // Monitor number of retries
                                        // Did not receive a C
                                        // So stop trying to download
                                        retry--;
                                        if (retry < 1)
                                        {
                                            xmodemCancel = true;
                                        }
                                    }
                                }
                                catch(Exception e) 
                                {
                                    log.Error("Error downloading a file from the ADCP.", e);
                                }

                                // Canceled download
                                if (xmodemCancel)
                                {
                                    // Stop trying to download the file
                                    _cancelWaitForDownload = true;

                                    // Close the file stream
                                    //CloseBinaryWriter();
                                    CancelDownload();

                                    // File could not be downloaded
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            // The file could not be started
                            // So complete the download
                            CompleteDownload(false);

                            // Close the file stream
                            CloseBinaryWriter();

                            return false;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                //OK = false;
                //Debug.WriteLine("Exception: {0}", e.ToString());
                log.Error("Error trying to download a file.", e);
                // Close the file stream
                CloseBinaryWriter();

                return false;
            }

            return true;
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

            // Set the buffer size based off in high speed mode or not
            int bufferSize = DEFAULT_DL_BUFF_SIZE;
            if (_isHighSpeedDownload)
            {
                bufferSize = DEFAULT_HSDL_BUFF_SIZE;
            }

            // Add the data to a buffer.
            // This is to wait until a complete packet has arrived
            _downloadDataBuffer.AddRange(data);

            // If there is not enough data in the buffer
            // wait for more data
            while (_downloadDataBuffer.Count >= bufferSize + 5)
            {

                int retry = XMODEM_RETRY_LIMIT;
                int slowdown = 0;

                // Find the type of packet
                switch (_downloadDataBuffer[0])
                {
                    default:
                        SetReceiveBuffer("I");
                        _downloadDataBuffer.RemoveAt(0);
                        break;
                    case CAN:
                        break;
                    case SOH:
                        break;
                    case STX:
                        // Stop the loop from requesting the
                        // start of data
                        _cancelWaitForDownload = true;

                        byte[] Fbuff;
                        if (VerifyPacketSTX(_downloadDataBuffer.ToArray(), _seqNum, out Fbuff))
                        {
                            // Should be nothing else in the buffer
                            // the buffer should have only contained the packet
                            _downloadDataBuffer.Clear();

                            // Set how many bytes have been written
                            _byteswritten += Fbuff.Length;

                            // Show in the serial port console progress.
                            //Debug.Write(".");

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
                                catch (Exception e) 
                                {
                                    log.Error("Error trying to write serial port download data to file.", e);
                                }
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
                            // Packet in the buffer was bad
                            // clear the buffer and request another packet
                            _downloadDataBuffer.Clear();

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

        #endregion

        #region XModem Upload

        private const int BYTES_IN_XMODEM_PACKET = 1024 + 5;

        private bool XmodemGotC;
        private byte XmodemPacketNum;
        byte[] bBuff = new byte[140000];
        byte[] XmodemBuff = new byte[BYTES_IN_XMODEM_PACKET];
        private const int XmodemPackageSize = 1024;

        private int XmodemState = 0;

        private const int XmodemInit = 0;
        private const int XmodemXsent = 1;
        private const int XmodemXrcvd = 2;
        private const int XmodemEOTsent = 3;

        private bool XmodemCancel = false;
        private bool XmodemFirstPacketSent = false;
        private bool XMODEM = false;

        /// <summary>
        /// Cancel uploading the data.  This will
        /// stop the while loop in the upload process
        /// if it is running.
        /// </summary>
        public void CancelUpload()
        {
            XmodemCancel = true;
        }

        /// <summary>
        /// Publish the event for the size of the file
        /// that is currently being uploaded.
        /// <param name="fileName">File Name.</param>
        /// <param name="fileSize">File Size.</param>
        /// </summary>
        private void PublishUploadFileSizeEvent(string fileName, long fileSize)
        {
            if (UploadFileSizeEvent != null)
            {
                UploadFileSizeEvent(fileName, fileSize);
            }
        }

        /// <summary>
        /// Publish the event that the current file
        /// has completed being uploaded.
        /// If the upload is completed because of an error, set the
        /// parameter to false.  This will instructor the user, there was an issue
        /// when uploading.
        /// </summary>
        /// <param name="fileName">File Name.</param>
        /// <param name="goodDownload">Set flag if the download was good or bad.</param>
        private void PublishUploadCompleteEvent(string fileName, bool goodDownload)
        {
            if (UploadCompleteEvent != null)
            {
                UploadCompleteEvent(fileName, goodDownload);
            }
        }

        /// <summary>
        /// Publish the event of the current progress
        /// of uploading the file.  This will give
        /// the number of bytes currently uploaded.
        /// </summary>
        /// <param name="fileName">File Name.</param>
        /// <param name="bytesWritten">Bytes written.</param>
        private void PublishUploadProgressEvent(string fileName, long bytesWritten)
        {
            if (UploadProgressEvent != null)
            {
                UploadProgressEvent(fileName, bytesWritten);
            }
        }

        /// <summary>
        /// Send an XModem packet.  This will be a 1024 byte packet.
        /// It will include a header with the STX command, the packet number
        /// and inverse and a checksum.
        /// </summary>
        /// <param name="Data">Packet to send through serial port.</param>
        private void SendXmodemPacket(byte[] Data)
        {
            int i, j, k;
            int sum;

            if (XmodemGotC)
            {
                long crc;
                long D;

                //bytes = 1024 + 5;
                XmodemBuff[0] = STX;
                XmodemBuff[1] = XmodemPacketNum;
                XmodemBuff[2] = (byte)(255 - (int)XmodemPacketNum);

                crc = 0;
                j = 3;
                for (i = 0; i < 1024; i++)
                {
                    XmodemBuff[j] = Data[i];
                    crc = crc ^ (Data[i] << 8);
                    for (k = 0; k < 8; ++k)
                    {
                        long it = crc & 0x8000;
                        if (it == 0x8000)
                            crc = crc << 1 ^ 0x1021;
                        else
                            crc = crc << 1;
                    }
                    j++;
                }
                crc = (crc & 0xFFFF);
                D = 0xFF & (crc >> 8);
                XmodemBuff[j] = (byte)D;
                D = 0xFF & crc;
                XmodemBuff[j + 1] = (byte)D;
            }
            else
            {
                XmodemBuff[0] = SOH;
                XmodemBuff[1] = XmodemPacketNum;
                XmodemBuff[2] = (byte)(255 - (int)XmodemPacketNum);
                sum = 0;
                j = 3;
                for (i = 0; i < 128; i++)
                {
                    XmodemBuff[j] = Data[i];
                    sum = sum + XmodemBuff[j];
                    j++;
                }
                XmodemBuff[j] = (byte)sum;
            }

            try
            {
                SendData(XmodemBuff, 0, BYTES_IN_XMODEM_PACKET);
            }
            catch(Exception e)
            {
                log.Error("Error trying to send a serial port download packet.", e);
            }
        }

        /// <summary>
        /// Upload the given file to the ADCP.
        /// This will take the file path and upload the
        /// file to the ADCP.  It will use the XMODEM-CRC
        /// protocol to send the data.  The data will be 
        /// sent in 1024 packets, with the correct header
        /// and checksum.  Based off the response from the ADCP,
        /// it will continue to sending data to ADCP until the
        /// file has been completely sent.
        /// 
        /// If the file does not exist, an exception will be thrown.
        /// </summary>
        /// <param name="path">File path to the file to send to the ADCP.</param>
        public void XModemUpload(string path)
        {
            int i;
            int count;
            int nBytesRead = 0;
            long bytesWritten = 0;
            Stream stream = null;
            byte[] XmodemData = new byte[2000];
            string exceptionmessage;
            int retry = 0;

            // File does not exist
            if (!File.Exists(path))
            {
                log.Error("Serial port upload file does not exist.");
                throw new IOException("File does not exist");
            }

            // Set all the settings to start an upload
            StartUpload();

            //if (openFileDialog1.ShowDialog() == DialogResult.OK)
            //{
                try
                {
                    if ((stream = File.OpenRead(path)) != null)
                    {
                        // Get the file size and publish
                        FileInfo fileInfo = new FileInfo(path);
                        long fileSize = fileInfo.Length;
                        PublishUploadFileSizeEvent(path, fileSize);

                        // Read in the first packet of data from the file
                        // Determine which is smaller, the packet size or filesize.
                        nBytesRead = stream.Read(XmodemData, 0, (int)Math.Min(XmodemPackageSize, fileSize));

                        if (nBytesRead > 0)
                        {
                            XmodemState = 0;
                            XmodemGotC = false;
                            XmodemCancel = false;
                            XmodemFirstPacketSent = false;
                            XmodemPacketNum = 1;
                            XMODEM = true;

                            // Create command to upload to the ADCP.
                            string message1 = string.Format("{0}{1} \r", RTI.Commands.AdcpCommands.CMD_DSXR, Path.GetFileName(path));
                            //DSXR
                            try
                            {
                                // Send the command to the ADCP to start uploading a file
                                // ****
                                // BUG, SHOULD WAIT FOR ECHO TO ENSURE NO NAME WITH A C IS PARSED LATER
                                // ****
                                SendData(message1);
                                //SendDataWaitReply(message1);  // Need to call a read.  Move StartUpload() to here


                                XmodemState = XmodemXsent;

                                while (XMODEM)
                                {
                                    if (XmodemCancel)
                                    {
                                        XMODEM = false;
                                        CancelXmodem();
                                    }
                                    else
                                    {
                                        try
                                        {
                                            // Get the command from the ADCP
                                            count = ReadData(bBuff, 0, 1);
                                            if (count > 0)
                                            {
                                                bBuff[1] = 0;

                                                string message = System.Text.ASCIIEncoding.ASCII.GetString(bBuff);

                                                if (XmodemState == XmodemXsent)
                                                {
                                                    switch (bBuff[0])
                                                    {
                                                        default:
                                                            break;
                                                        case ACK:
                                                            XmodemState = XmodemXrcvd;
                                                            break;
                                                        case NAK:
                                                            XMODEM = false;
                                                            CancelXmodem();
                                                            break;
                                                    }
                                                }
                                                else
                                                {
                                                    // Determine which command was sent from the ADCP
                                                    switch (bBuff[0])
                                                    {
                                                        default:
                                                            exceptionmessage = "ADCP not ready to upload.";
                                                            break;
                                                        // Request download
                                                        case (byte)'C':
                                                            XmodemGotC = true;
                                                            SendXmodemPacket(XmodemData);
                                                            bytesWritten += XmodemData.Length;
                                                            XmodemFirstPacketSent = true;

                                                            break;
                                                        // Received packet
                                                        case ACK:
                                                            // Reset retry
                                                            retry = 0;

                                                            // End of Transmmision 
                                                            if (XmodemState == XmodemEOTsent)
                                                            {
                                                                // Complete the upload process
                                                                CompleteUpload();
                                                            }
                                                            else
                                                            {
                                                                // If the first packet was received and accepted
                                                                // continue to send packets
                                                                if (XmodemFirstPacketSent)
                                                                {
                                                                    // Read the next packet of data from the file
                                                                    try
                                                                    {
                                                                        nBytesRead = stream.Read(XmodemData, 0, XmodemPackageSize);
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        nBytesRead = 0;
                                                                        exceptionmessage = String.Format("caughtH: {0}", ex.GetType().ToString());
                                                                        SetReceiveBuffer(exceptionmessage);
                                                                        log.Error("Error reading serial port upload file.", ex);
                                                                    }

                                                                    // If there are bytes ready to be sent
                                                                    // Continue sending the files
                                                                    if (nBytesRead > 0)
                                                                    {
                                                                        // Fill in the empty bytes to make a complete packet
                                                                        if (nBytesRead < XmodemPackageSize)
                                                                        {
                                                                            for (i = nBytesRead; i < XmodemPackageSize; i++)
                                                                                XmodemData[i] = 0xFF;
                                                                        }

                                                                        // Send the packet to the serial port
                                                                        XmodemPacketNum++;
                                                                        SendXmodemPacket(XmodemData);

                                                                        // Publish the number of bytes written currently
                                                                        bytesWritten += nBytesRead;
                                                                        PublishUploadProgressEvent(path, bytesWritten);
                                                                    }
                                                                    // Entire file sent
                                                                    else
                                                                    {
                                                                        // Set state that end of transmission is sent
                                                                        XmodemState = XmodemEOTsent;
                                                                        if (IsAvailable())
                                                                        {
                                                                            // Send End of Transmission
                                                                            try
                                                                            {
                                                                                byte[] buff = new byte[2];
                                                                                buff[0] = EOT;
                                                                                SendData(buff, 0, 1);
                                                                            }
                                                                            catch (Exception ex)
                                                                            {
                                                                                nBytesRead = 0;
                                                                                exceptionmessage = String.Format("caughtI: {0}", ex.GetType().ToString());
                                                                                SetReceiveBuffer(exceptionmessage);
                                                                                log.Error("Error trying to send data to the serial port while uploading data.", ex);
                                                                            }
                                                                        }

                                                                        // Send event that upload is complete
                                                                        PublishUploadCompleteEvent(path, true);

                                                                        XMODEM = false;
                                                                    }
                                                                }
                                                                // A first packet has not been sent
                                                                // Try resending the first packet
                                                                else
                                                                {
                                                                    SendXmodemPacket(XmodemData);
                                                                    XmodemFirstPacketSent = true;
                                                                }
                                                            }
                                                            break;
                                                        // Did not receive packet, resend packet
                                                        case NAK:
                                                            SendXmodemPacket(XmodemData);
                                                            XmodemFirstPacketSent = true;

                                                            // Limit the number of times this should retry to send a command
                                                            if (retry++ > XMODEM_RETRY_LIMIT)
                                                            {
                                                                CompleteUpload();
                                                            }
                                                            break;
                                                        // Cancel Upload
                                                        case CAN:
                                                            message = "Client requests CANCEL";
                                                            //WriteMessageTxtSerial(message, false);
                                                            CompleteUpload();
                                                            break;
                                                    }
                                                    SetReceiveBuffer(message);
                                                }
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            }

                            catch (Exception ex)
                            {
                                XmodemState = XmodemInit;
                                exceptionmessage = String.Format("caughtJ: {0}", ex.GetType().ToString());
                                SetReceiveBuffer(exceptionmessage);
                                log.Error("Error uploading a file through the serial port. __", ex);
                            }

                            stream.Close();
                        }
                        CompleteUpload();
                    }
                }
                catch (Exception ex)
                {
                    nBytesRead = 0;
                    exceptionmessage = String.Format("caughtK: {0}", ex.GetType().ToString());
                    SetReceiveBuffer(exceptionmessage);
                    log.Error("Error uploading a file through the serial port.", ex);
                }
            //}
        }

        /// <summary>
        /// Send a cancel message to the serial port
        /// if a cancel is needed.  This will send the CAN
        /// message to the serial port.  It will send it multple
        /// times to ensure it is received.
        /// </summary>
        private void CancelXmodem()
        {
            //XmodemCancelled = true;
            if (IsAvailable())
            {
                try
                {
                    byte[] buff = new byte[4];
                    buff[0] = CAN;
                    buff[1] = CAN;
                    buff[2] = CAN;
                    SendData(buff, 0, 3);
                }
                catch (Exception ex)
                {
                    SetReceiveBuffer(String.Format("caughtL: {0}", ex.GetType().ToString()));
                    log.Error("Error trying to cancel a serial port upload.", ex);
                }
            }

            // Reset the ADCP settings
            CompleteUpload();
        }

        /// <summary>
        /// Start the upload process.  This will set all the
        /// settings in needed to do an upload.  This includes
        /// pausing the read thread and setting the mode to Upload.
        /// </summary>
        private void StartUpload()
        {
            // Pause the read thread
            // This will make the read thread not read data from the serial port
            PauseReadThread(true);

            // Set the mode of the ADCP
            Mode = AdcpSerialModes.UPLOAD;

            // Set the interval to a large number
            // because we will not be using the read thread
            SetTimerInterval(2000);
        }

        /// <summary>
        /// Reset all the settings back to standard ADCP mode.
        /// This includes resuming the read thread and setting
        /// the mode back to ADCP.
        /// </summary>
        private void CompleteUpload()
        {
            // Stop the upload loop
            XMODEM = false;

            // This will make the read thread resume reading data from the serial port
            PauseReadThread(false);

            // Set the mode of the ADCP
            Mode = AdcpSerialModes.ADCP;

            // Set the interval back to default
            SetTimerInterval();
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

            // If the command was not good, try it again
            if (!startResult)
            {
                startResult = SendDataWaitReply(RTI.Commands.AdcpCommands.CMD_DIAGCPT, TIMEOUT);
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

            // Stop ADCP from compass mode
            SendDataWaitReply(RTI.Commands.AdcpCommands.CMD_DIAGCPT_DISCONNECT, TIMEOUT);

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
            if (Mode == AdcpSerialModes.COMPASS  && command != null)
            {
                SendData(command, 0, command.Length);
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

        #region Send Commands To Serial Port

        /// <summary>
        /// Send a list of commands.
        /// </summary>
        /// <param name="commands">List of commands.</param>
        /// <returns>TRUE = All commands were sent successfully.</returns>
        public bool SendCommands(List<string> commands)
        {
            bool result = true;

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
                    bool currResult = SendDataWaitReply(cmd, TIMEOUT);
                    if (!currResult)
                    {
                        // Try again if failed first time
                        currResult = SendDataWaitReply(cmd, TIMEOUT);
                    }

                    // Keep track if any were false
                    // If any were false, this will stay false
                    result &= currResult;
                }

                // Write the line out
                Debug.WriteLine(cmd);
            }

            return result;
        }

        #endregion

        #region System Test

        #region Communication

        /// <summary>
        /// Test if we have communication with the ADCP.  This will send a break
        /// to the ADCP.  If nothing is received from the ADCP, then we do not 
        /// have communication and send a fail result.
        /// </summary>
        /// <returns>Result from the test.</returns>
        public SystemTestResult SysTestAdcpCommunication()
        {
            SystemTestResult result = new SystemTestResult();

            // Clear buffer
            ReceiveBufferString = "";

            // Send a Break
            SendBreak();

            // Wait for an output
            Thread.Sleep(RTI.AdcpSerialPort.WAIT_STATE);

            // Get the buffer output
            string buffer = ReceiveBufferString;

            if (buffer.Length <= 0)
            {
                result.TestResult = false;
                result.ErrorListStrings.Add("Communication with ADCP failed.");
                result.ErrorCodes.Add(SystemTestErrorCodes.ADCP_NO_COMM);
            }
            else
            {
                RTI.Commands.BreakStmt breakStmt = RTI.Commands.AdcpCommands.DecodeBREAK(ReceiveBufferString);
                
                // Pass the results
                result.Results = breakStmt;
            }

            return result;
        }

        #endregion

        #region Firmware

        /// <summary>
        /// Verify the Firmware version matches.  This will send a BREAK to the ADCP and 
        /// decode the BREAK statement for the firmware version.  
        /// </summary>
        /// <param name="isTestVersion">Flag if we should test the version agaisnt the one given.</param>
        /// <param name="major">Major version.</param>
        /// <param name="minor">Minor version.</param>
        /// <param name="revision">Revision version.</param>
        /// <returns>Firmware test result.</returns>
        public SystemTestResult SysTestFirmwareVersion(bool isTestVersion, ushort major = 0, ushort minor = 0, ushort revision = 0)
        {
            SystemTestResult result = new SystemTestResult();

            // Clear buffer
            ReceiveBufferString = "";

            // Send a Break
            SendBreak();

            // Wait for an output
            Thread.Sleep(RTI.AdcpSerialPort.WAIT_STATE);

            // Get the buffer output
            string buffer = ReceiveBufferString;

            if (buffer.Length <= 0)
            {
                result.TestResult = false;
                result.ErrorListStrings.Add("Communication with ADCP failed.");
                result.ErrorCodes.Add(SystemTestErrorCodes.ADCP_NO_COMM);
            }
            else
            {
                // Decode a break statement for version 
                RTI.Commands.BreakStmt breakStmt = RTI.Commands.AdcpCommands.DecodeBREAK(ReceiveBufferString);

                // Check if we should check the versions for specific values
                if (isTestVersion)
                {
                    // Check each value with the one given
                    if (breakStmt.FirmwareVersion.FirmwareMajor < major)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("Firmware Major version does not match.  Expected: {0}  Found: {1}", major, breakStmt.FirmwareVersion.FirmwareMajor));
                        result.ErrorCodes.Add(SystemTestErrorCodes.ADCP_FIRMWARE_MAJOR);
                    }
                    if (breakStmt.FirmwareVersion.FirmwareMinor < minor)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("Firmware Minor version does not match.  Expected: {0}  Found: {1}", minor, breakStmt.FirmwareVersion.FirmwareMinor));
                        result.ErrorCodes.Add(SystemTestErrorCodes.ADCP_FIRMWARE_MINOR);
                    }
                    if (breakStmt.FirmwareVersion.FirmwareRevision < revision)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("Firmware Revision version does not match.  Expected: {0}  Found: {1}", revision, breakStmt.FirmwareVersion.FirmwareRevision));
                        result.ErrorCodes.Add(SystemTestErrorCodes.ADCP_FIRMWARE_REVISION);
                    }
                }
                else
                {
                    // Check if the major, minor and revision are all 0
                    // If all are 0, then the version is not set
                    if (breakStmt.FirmwareVersion.FirmwareMajor == 0 && breakStmt.FirmwareVersion.FirmwareMinor == 0 && breakStmt.FirmwareVersion.FirmwareRevision == 0)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("Firmware version not set."));
                        result.ErrorCodes.Add(SystemTestErrorCodes.ADCP_FIRMWARE_VERSION);
                    }
                }

                // Pass the results
                result.Results = breakStmt;
            }

            return result;
        }

        /// <summary>
        /// Test if the system firmware files exist. This check the
        /// directory listing for all the files. If the firmware files
        /// do not exist, the result will report which files are missing.
        /// </summary>
        /// <returns>Result of the test.  File missing if any missing.</returns>
        public SystemTestResult SysTestFirmwareFiles()
        {
            SystemTestResult result = new SystemTestResult();

            // Download the directory listing to _dirListingString
            DownloadDirectoryListing();

            // Parse the directory listing string for all file info
            // Each line should contain a piece of file info
            string[] lines = _dirListingString.Split('\n');

            // Ensure data exist
            if (lines.Length <= 0)
            {
                result.ErrorListStrings.Add("Firmware files missing.");
                result.ErrorCodes.Add(SystemTestErrorCodes.FIRMWARE_FILES_MISSING);
                result.TestResult = false;
            }
            else
            {
                // Data exist, verify the correct files exist
                bool helpExist = false;
                bool engHelpExist = false;
                bool maintExist = false;
                bool rtisysExist = false;
                bool bootExist = false;
                bool sysconfExist = false;
                bool bbcodeExist = false;

                for(int x = 0; x < lines.Length; x++)
                {
                    if (lines[x].Contains("HELP")) { helpExist = true; }            // HELP.TXT
                    if (lines[x].Contains("ENGHELP")) { engHelpExist = true; }      // ENGHELP.TXT
                    if (lines[x].Contains("MAINT")) { maintExist = true; }          // MAINT.TXT
                    if (lines[x].Contains("RTISYS")) { rtisysExist = true; }        // RTISYS.BIN
                    if (lines[x].Contains("BOOT")) { bootExist = true; }            // BOOT.BIN
                    if (lines[x].Contains("SYSCONF")) { sysconfExist = true; }      // SYSCONF.BIN
                    if (lines[x].Contains("BBCODE")) { bbcodeExist = true; }        // BBCODE.BIN
                }

                // HELP.TXT missing
                if (!helpExist)
                {
                    result.ErrorListStrings.Add("HELP.TXT missing.");
                    result.ErrorCodes.Add(SystemTestErrorCodes.FIRMWARE_HELP_MISSING);
                    result.TestResult = false;
                }

                // EngHelp.TXT missing
                if (!engHelpExist)
                {
                    result.ErrorListStrings.Add("ENGHELP.TXT missing.");
                    result.ErrorCodes.Add(SystemTestErrorCodes.FIRMWARE_ENGHELP_MISSING);
                    result.TestResult = false;
                }

                // maint.TXT missing
                if (!maintExist)
                {
                    result.ErrorListStrings.Add("MAINT.TXT missing.");
                    result.ErrorCodes.Add(SystemTestErrorCodes.FIRMWARE_MAINT_MISSING);
                    result.TestResult = false;
                }

                // RTISYS.BIN missing
                if (!rtisysExist)
                {
                    result.ErrorListStrings.Add("RTISYS.BIN missing.");
                    result.ErrorCodes.Add(SystemTestErrorCodes.FIRMWARE_RTISYS_MISSING);
                    result.TestResult = false;
                }

                // BOOT.BIN missing
                if (!bootExist)
                {
                    result.ErrorListStrings.Add("BOOT.BIN missing.");
                    result.ErrorCodes.Add(SystemTestErrorCodes.FIRMWARE_BOOT_MISSING);
                    result.TestResult = false;
                }

                // SYSCONF.BIN missing
                if (!sysconfExist)
                {
                    result.ErrorListStrings.Add("SYSCONF.BIN missing.");
                    result.ErrorCodes.Add(SystemTestErrorCodes.FIRMWARE_SYSCONF_MISSING);
                    result.TestResult = false;
                }

                // BBCODE.BIN missing
                if (!bbcodeExist)
                {
                    result.ErrorListStrings.Add("BBCODE.BIN missing.");
                    result.ErrorCodes.Add(SystemTestErrorCodes.FIRMWARE_BBCODE_MISSING);
                    result.TestResult = false;
                }
            }

            // Clear the string
            _dirListingString = "";

            return result;
        }

        #endregion

        #region Compass

        /// <summary>
        /// Decode the ENGPNI command.  Verify that heading, pitch and roll are not 0.
        /// If all the values are 0, then the compass is not outputing data.
        /// </summary>
        /// <returns>Return the results of the test.</returns>
        public SystemTestResult SysTestCompass()
        {
            SystemTestResult result = new SystemTestResult();

            // Clear buffer
            ReceiveBufferString = "";

            // Send the command ENGPNI
            SendData(RTI.Commands.AdcpCommands.CMD_ENGPNI);

            // Wait for an output
            Thread.Sleep(RTI.AdcpSerialPort.WAIT_STATE);

            // Decode the result
            RTI.Commands.HPR hpr = RTI.Commands.AdcpCommands.DecodeEngPniResult(ReceiveBufferString);

            // Check if all the values are 0
            if (hpr.Heading == 0 && hpr.Pitch == 0 && hpr.Roll == 0)
            {
                result.TestResult = false;
                result.ErrorListStrings.Add("Compass not outputing data.");
                result.ErrorCodes.Add(SystemTestErrorCodes.COMPASS_NO_OUTPUT);
            }

            return result;
        }

        #endregion

        #region I2C Boards

        /// <summary>
        /// Verify all the registers are present and the
        /// board serial numbers and revisions are correct.
        /// </summary>
        /// <param name="testSerialRev">Flag if we should test the serial and revision of the board.</param>
        /// <param name="memDevsExpected">The expected results.</param>
        /// <returns>Result of the test.</returns>
        public SystemTestResult SysTestI2cMemoryDevices(bool testSerialRev, RTI.Commands.I2cMemDevs memDevsExpected)
        {
            SystemTestResult result = new SystemTestResult();

            // Clear buffer
            ReceiveBufferString = "";

            // Send the command ENGI2CSHOW
            SendData("ENGI2CSHOW");

            // Wait for an output
            Thread.Sleep(RTI.AdcpSerialPort.WAIT_STATE);

            // Get the buffer output
            string buffer = ReceiveBufferString;

            if (buffer.Length <= 0)
            {
                result.TestResult = false;
                result.ErrorListStrings.Add("Communication with ADCP failed.");
                result.ErrorCodes.Add(SystemTestErrorCodes.ADCP_NO_COMM);
            }
            else
            {
                RTI.Commands.I2cMemDevs memDevsResult = RTI.Commands.AdcpCommands.DecodeENGI2CSHOW(ReceiveBufferString);

                // Set the results so the user can see the serial number and revision actually seen
                result.Results = memDevsResult;

                // Check if we are testing the revision and serial number
                // Because the user cannot know the correct serial and rev,
                // this test cannot always pass if the software is not updated with the latest revisions
                // If we are not testing for the correct serial and revision, then verify that anything is set,
                // If the revision or serial number is 0, then the serial or revision is not set.
                if (testSerialRev)
                {
                    // Backplane serial
                    if (memDevsExpected.BackPlaneBoard.SerialNum != memDevsResult.BackPlaneBoard.SerialNum)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("Backplane board serial number does not match system serial number.  Expected: {0}  Found: {1}", memDevsExpected.BackPlaneBoard.SerialNum, memDevsResult.BackPlaneBoard.SerialNum));
                        result.ErrorCodes.Add(SystemTestErrorCodes.INCORRECT_SERIAL_BACKPLANE_BRD);
                    }

                    // Backplane Rev
                    if (memDevsExpected.BackPlaneBoard.Revision.CompareTo(memDevsResult.BackPlaneBoard.Revision) != 0)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("Backplane board revision does not match.  Expected: {0}  Found: {1}", memDevsExpected.BackPlaneBoard.Revision, memDevsResult.BackPlaneBoard.Revision));
                        result.ErrorCodes.Add(SystemTestErrorCodes.INCORRECT_REV_BACKPLANE_BRD);
                    }

                    // I/O serial
                    if (memDevsExpected.IoBoard.SerialNum != memDevsResult.IoBoard.SerialNum)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("I/O board serial number does not match system serial number.  Expected: {0}  Found: {1}", memDevsExpected.IoBoard.SerialNum, memDevsResult.IoBoard.SerialNum));
                        result.ErrorCodes.Add(SystemTestErrorCodes.INCORRECT_SERIAL_IO_BRD);
                    }

                    // I/0 Rev
                    if (memDevsExpected.IoBoard.Revision.CompareTo(memDevsResult.IoBoard.Revision) != 0)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("I/0 board revision does not match.  Expected: {0}  Found: {1}", memDevsExpected.IoBoard.Revision, memDevsResult.IoBoard.Revision));
                        result.ErrorCodes.Add(SystemTestErrorCodes.INCORRECT_REV_IO_BRD);
                    }

                    // RVCR serial
                    if (memDevsExpected.RcvrBoard.SerialNum != memDevsResult.RcvrBoard.SerialNum)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("Receiver board serial number does not match system serial number.  Expected: {0}  Found: {1}", memDevsExpected.RcvrBoard.SerialNum, memDevsResult.RcvrBoard.SerialNum));
                        result.ErrorCodes.Add(SystemTestErrorCodes.INCORRECT_SERIAL_RCVR_BRD);
                    }

                    // RVCR Rev
                    if (memDevsExpected.RcvrBoard.Revision.CompareTo(memDevsResult.RcvrBoard.Revision) != 0)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("Receiver board revision does not match.  Expected: {0}  Found: {1}", memDevsExpected.RcvrBoard.Revision, memDevsResult.RcvrBoard.Revision));
                        result.ErrorCodes.Add(SystemTestErrorCodes.INCORRECT_REV_RCVR_BRD);
                    }

                    // Low Power Reg serial
                    if (memDevsExpected.LowPwrRegBoard.SerialNum != memDevsResult.LowPwrRegBoard.SerialNum)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("Low Power Regulator board serial number does not match system serial number.  Expected: {0}  Found: {1}", memDevsExpected.LowPwrRegBoard.SerialNum, memDevsResult.LowPwrRegBoard.SerialNum));
                        result.ErrorCodes.Add(SystemTestErrorCodes.INCORRECT_SERIAL_LOW_PWR_REG_BRD);
                    }

                    // Low Power Reg Rev
                    if (memDevsExpected.LowPwrRegBoard.Revision.CompareTo(memDevsResult.LowPwrRegBoard.Revision) != 0)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("Low Power Regulator board revision does not match.  Expected: {0}  Found: {1}", memDevsExpected.LowPwrRegBoard.Revision, memDevsResult.LowPwrRegBoard.Revision));
                        result.ErrorCodes.Add(SystemTestErrorCodes.INCORRECT_REV_LOW_PWR_REG_BRD);
                    }

                    // Virtual Ground serial
                    if (memDevsExpected.VirtualGndBoard.SerialNum != memDevsResult.VirtualGndBoard.SerialNum)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("Virtual Ground board serial number does not match system serial number.  Expected: {0}  Found: {1}", memDevsExpected.VirtualGndBoard.SerialNum, memDevsResult.VirtualGndBoard.SerialNum));
                        result.ErrorCodes.Add(SystemTestErrorCodes.INCORRECT_SERIAL_VIRTUAL_GND_BRD);
                    }

                    // Virtual Ground Rev
                    if (memDevsExpected.VirtualGndBoard.Revision.CompareTo(memDevsResult.VirtualGndBoard.Revision) != 0)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("Virtual Ground board revision does not match.  Expected: {0}  Found: {1}", memDevsExpected.VirtualGndBoard.Revision, memDevsResult.VirtualGndBoard.Revision));
                        result.ErrorCodes.Add(SystemTestErrorCodes.INCORRECT_REV_VIRTUAL_GND_BRD);
                    }

                    // Xmitter serial
                    if (memDevsExpected.XmitterBoard.SerialNum != memDevsResult.XmitterBoard.SerialNum)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("Xmitter board serial number does not match system serial number.  Expected: {0}  Found: {1}", memDevsExpected.XmitterBoard.SerialNum, memDevsResult.XmitterBoard.SerialNum));
                        result.ErrorCodes.Add(SystemTestErrorCodes.INCORRECT_SERIAL_XMITTER_BRD);
                    }

                    // Xmitter Rev
                    if (memDevsExpected.XmitterBoard.Revision.CompareTo(memDevsResult.XmitterBoard.Revision) != 0)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("Xmitter board revision does not match.  Expected: {0}  Found: {1}", memDevsExpected.XmitterBoard.Revision, memDevsResult.XmitterBoard.Revision));
                        result.ErrorCodes.Add(SystemTestErrorCodes.INCORRECT_REV_XMITTER_BRD);
                    }
                }
                else
                {
                    // Backplane serial
                    if (memDevsResult.BackPlaneBoard.SerialNum == 0)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("Backplane board serial number is not set."));
                        result.ErrorCodes.Add(SystemTestErrorCodes.INCORRECT_SERIAL_BACKPLANE_BRD);
                    }

                    // Backplane Rev
                    if (memDevsResult.BackPlaneBoard.Revision.CompareTo("0") == 0)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("Backplane board revision is not set."));
                        result.ErrorCodes.Add(SystemTestErrorCodes.INCORRECT_REV_BACKPLANE_BRD);
                    }

                    // I/O serial
                    if (memDevsResult.IoBoard.SerialNum == 0)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("I/O board serial number is not set."));
                        result.ErrorCodes.Add(SystemTestErrorCodes.INCORRECT_SERIAL_IO_BRD);
                    }

                    // I/0 Rev
                    if (memDevsResult.IoBoard.Revision.CompareTo("0") == 0)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("I/0 board revision is not set."));
                        result.ErrorCodes.Add(SystemTestErrorCodes.INCORRECT_REV_IO_BRD);
                    }

                    // RVCR serial
                    if (memDevsResult.RcvrBoard.SerialNum == 0)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("Receiver board serial number is not set."));
                        result.ErrorCodes.Add(SystemTestErrorCodes.INCORRECT_SERIAL_RCVR_BRD);
                    }

                    // RVCR Rev
                    if (memDevsResult.RcvrBoard.Revision.CompareTo("0") == 0)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("Receiver board revision is not set."));
                        result.ErrorCodes.Add(SystemTestErrorCodes.INCORRECT_REV_RCVR_BRD);
                    }

                    // Low Power Reg serial
                    if (memDevsResult.LowPwrRegBoard.SerialNum == 0)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("Low Power Regulator board serial number is not set."));
                        result.ErrorCodes.Add(SystemTestErrorCodes.INCORRECT_SERIAL_LOW_PWR_REG_BRD);
                    }

                    // Low Power Reg Rev
                    if (memDevsResult.LowPwrRegBoard.Revision.CompareTo("0") == 0)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("Low Power Regulator board revision is not set."));
                        result.ErrorCodes.Add(SystemTestErrorCodes.INCORRECT_REV_LOW_PWR_REG_BRD);
                    }

                    // Virtual Ground serial
                    if (memDevsResult.VirtualGndBoard.SerialNum == 0)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("Virtual Ground board serial number is not set."));
                        result.ErrorCodes.Add(SystemTestErrorCodes.INCORRECT_SERIAL_VIRTUAL_GND_BRD);
                    }

                    // Virtual Ground Rev
                    if (memDevsResult.VirtualGndBoard.Revision.CompareTo("0") == 0)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("Virtual Ground board revision is not set."));
                        result.ErrorCodes.Add(SystemTestErrorCodes.INCORRECT_REV_VIRTUAL_GND_BRD);
                    }

                    // Xmitter serial
                    if (memDevsResult.XmitterBoard.SerialNum == 0)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("Xmitter board serial number is not set."));
                        result.ErrorCodes.Add(SystemTestErrorCodes.INCORRECT_SERIAL_XMITTER_BRD);
                    }

                    // Xmitter Rev
                    if (memDevsResult.XmitterBoard.Revision.CompareTo("0") == 0)
                    {
                        result.TestResult = false;
                        result.ErrorListStrings.Add(string.Format("Xmitter board revision is not set."));
                        result.ErrorCodes.Add(SystemTestErrorCodes.INCORRECT_REV_XMITTER_BRD);
                    }
                }
            }

            return result;
        }

        #endregion

        #region RTC Time

        /// <summary>
        /// Test the RTC.  This will set the time to the RTC.  It will
        /// then check the time agaisnt the system time.  The times should be
        /// within a second of each other.  So if ADCP time is before 
        /// </summary>
        /// <returns>Result of the test.</returns>
        public SystemTestResult SysTestRtcLocalTime()
        {
            SystemTestResult result = new SystemTestResult();

            // Set the system time
            if (SetLocalSystemDateTime())
            {
                //// Now get the time and verify it is within 1 second of the previous time
                //// Clear buffer
                //ReceiveBufferString = "";

                //// Send STIME to get the time
                //SendData(string.Format("{0}", RTI.Commands.AdcpCommands.CMD_STIME));

                //// Wait for an output
                //Thread.Sleep(RTI.AdcpSerialPort.WAIT_STATE * 2);


                //DateTime adcpDt = RTI.Commands.AdcpCommands.DecodeSTIME(ReceiveBufferString);           // Decode the date and time from the ADCP
                DateTime adcpDt = GetAdcpDateTime();                                                    // Decode the date and time from the ADCP
                DateTime currDt = DateTime.Now;                                                         // Get the current date and time

                // Set the Results as the ADCP Date and Time
                result.Results = adcpDt;

                // Verify the Date and Time are within a second of each other
                // This could fail on the rare occassion that the hour changes in the second between getting the two times
                if (adcpDt > currDt ||
                    adcpDt.Year != currDt.Year ||
                    adcpDt.Month != currDt.Month ||
                    adcpDt.Day != currDt.Day ||
                    adcpDt.Hour != currDt.Hour ||
                    adcpDt.Minute != currDt.Minute ||
                    adcpDt.Second > currDt.Second)
                {
                    result.TestResult = false;
                    result.ErrorListStrings.Add(string.Format("RTC time does not match current time.  Expected: {0}  Found {1}", currDt.ToString(), adcpDt.ToString()));
                    result.ErrorCodes.Add(SystemTestErrorCodes.INCORRECT_RTC_TIME);
                }
            }
            else
            {
                // Could not send a command to the ADCP
                result.TestResult = false;
                result.ErrorListStrings.Add("Communication with ADCP failed.");
                result.ErrorCodes.Add(SystemTestErrorCodes.ADCP_NO_COMM);
            }

            return result;
        }

        /// <summary>
        /// Test the RTC.  This will set the time to the RTC.  It will
        /// then check the time agaisnt the system time.  The times should be
        /// within a second of each other.  So if ADCP time is before 
        /// </summary>
        /// <returns>Result of the test.</returns>
        public SystemTestResult SysTestRtcUtcTime()
        {
            SystemTestResult result = new SystemTestResult();

            // Set the system time
            if (SetUtcSystemDateTime())
            {
                //// Now get the time and verify it is within 1 second of the previous time
                //// Clear buffer
                //ReceiveBufferString = "";

                //// Send STIME to get the time
                //SendData(string.Format("{0}", RTI.Commands.AdcpCommands.CMD_STIME));

                //// Wait for an output
                //Thread.Sleep(RTI.AdcpSerialPort.WAIT_STATE * 2);

                //DateTime currDt = DateTime.Now.ToUniversalTime();                                       // Get the current date and time
                //DateTime adcpDt = RTI.Commands.AdcpCommands.DecodeSTIME(ReceiveBufferString);           // Decode the date and time from the ADCP

                DateTime adcpDt = GetAdcpDateTime();                                                    // Decode the date and time from the ADCP
                DateTime currDt = DateTime.Now.ToUniversalTime();                                       // Get the current date and time

                // Set the Results as the ADCP Date and Time
                result.Results = adcpDt;

                // Verify the Date and Time are within a second of each other
                // This could fail on the rare occassion that the hour changes in the second between getting the two times
                if (adcpDt > currDt ||
                    adcpDt.Year != currDt.Year ||
                    adcpDt.Month != currDt.Month ||
                    adcpDt.Day != currDt.Day ||
                    adcpDt.Hour != currDt.Hour ||
                    adcpDt.Minute != currDt.Minute ||
                    adcpDt.Second > currDt.Second)
                {
                    result.TestResult = false;
                    result.ErrorListStrings.Add(string.Format("RTC time does not match current time.  Expected: {0}  Found {1}", currDt.ToString(), adcpDt.ToString()));
                    result.ErrorCodes.Add(SystemTestErrorCodes.INCORRECT_RTC_TIME);
                }
            }
            else
            {
                // Could not send a command to the ADCP
                result.TestResult = false;
                result.ErrorListStrings.Add("Communication with ADCP failed.");
                result.ErrorCodes.Add(SystemTestErrorCodes.ADCP_NO_COMM);
            }

            return result;
        }

        #endregion

        #region Status

        /// <summary>
        /// Configure the ADCP to send a single Water Profile ping.  This will turn off Water Track and Bottom Track.
        /// Then Ping the system.  Then reset the default values.  There are no results for this test.  You must subscribe
        /// to receive the ensemble and check the status.
        /// </summary>
        /// <returns>No results are set.</returns>
        public SystemTestResult SysTestSingleWaterProfilePing()
        {
            SystemTestResult result = new SystemTestResult();

            // Now get the time and verify it is within 1 second of the previous time
            // Clear buffer
            ReceiveBufferString = "";

            // Set defaults, turn off bt and wt and ping
            SendData(string.Format("{0}", RTI.Commands.AdcpCommands.CMD_CDEFAULT));             // Set defaults
            SendData(string.Format("{0} 0", RTI.Commands.AdcpSubsystemCommands.CMD_CBTON));     // Turn off Bottom Track
            SendData(string.Format("{0} 0", RTI.Commands.AdcpSubsystemCommands.CMD_CWTON));     // Turn off Water Track
            SendData(string.Format("{0} 1", RTI.Commands.AdcpSubsystemCommands.CMD_CWPON));     // Turn On Water Profile
            SendData(string.Format("{0}", RTI.Commands.AdcpCommands.CMD_PING));                 // Get a ping

            // Wait for an output
            Thread.Sleep(RTI.AdcpSerialPort.WAIT_STATE);

            // Send CDEFAULT to put back in default mode
            SendData(string.Format("{0}", RTI.Commands.AdcpCommands.CMD_CDEFAULT));

            return result;
        }


        /// <summary>
        /// Configure the ADCP to send a single Bottom Track ping.  This will turn off Water Track and Water Profile.
        /// Then Ping the system.  Then reset the default values.  There are no results for this test.  You must subscribe
        /// to receive the ensemble and check the status.
        /// </summary>
        /// <returns>No results are set.</returns>
        public SystemTestResult SysTestSingleBottomTrackPing()
        {
            SystemTestResult result = new SystemTestResult();

            // Now get the time and verify it is within 1 second of the previous time
            // Clear buffer
            ReceiveBufferString = "";

            // Set defaults, turn off bt and wt and ping
            SendData(string.Format("{0}", RTI.Commands.AdcpCommands.CMD_CDEFAULT));             // Set defaults
            SendData(string.Format("{0} 0", RTI.Commands.AdcpSubsystemCommands.CMD_CWPON));     // Turn off Water Profile
            SendData(string.Format("{0} 0", RTI.Commands.AdcpSubsystemCommands.CMD_CWTON));     // Turn off Water Track
            SendData(string.Format("{0} 1", RTI.Commands.AdcpSubsystemCommands.CMD_CBTON));     // Turn On Bottom Track
            SendData(string.Format("{0}", RTI.Commands.AdcpCommands.CMD_PING));                 // Get a ping

            // Wait for an output
            Thread.Sleep(RTI.AdcpSerialPort.WAIT_STATE);

            // Send CDEFAULT to put back in default mode
            SendData(string.Format("{0}", RTI.Commands.AdcpCommands.CMD_CDEFAULT));

            return result;
        }

        #endregion

        #endregion

        #region Scan for ADCP Serial Port

        /// <summary>
        /// Scan for all the ADCPs connected to the serial ports.
        /// This will check each serial port and all the baud
        /// rates to find all the ADCPs connected to this computer.
        /// It will then return a list of all the serial port connections.
        /// </summary>
        /// <returns>List of all the serial ports connections with an ADCP.</returns>
        public List<AdcpSerialOptions> ScanSerialConnection()
        {
            // Get all the available COM ports
            List<string> ports = SerialOptions.PortOptions;

            var result = new List<AdcpSerialOptions>();

            foreach (var port in ports)
            {
                //if (TestSerialPortConnection(port))
                //{
                    AdcpSerialOptions options = TestSerialBaudConnection(port);
                    // Found a port now test the baud rate
                    if (options != null)
                    {
                        result.Add(options);
                    }
                //}
            }

            // Shutdown the object
            // This will stop the read thread
            // and the serial port
            // The user will still need to connect 
            // to the serial port again.
            Dispose();

            return result;
        }

        /// <summary>
        /// Send a BREAK to the port.
        /// If there is no response, then it is
        /// probably not an ADCP.  Even if the baud rate is 
        /// wrong, the BREAK should send something back.
        /// </summary>
        /// <param name="port">Serial port to test.</param>
        /// <returns>TRUE = A response was give for the port.</returns>
        protected bool TestSerialPortConnection(string port)
        {
            try
            {
                // Shutdown the serial port if it is currently open
                if (IsOpen())
                {
                    Disconnect();
                }

                // Create a serial connection
                _serialOptions = new SerialOptions();
                _serialOptions.Port = port;
                Connect();

                if (IsOpen())
                {
                    // Clear the buffer
                    ReceiveBufferString = "";

                    // Send a break to the Port and see if there was a response
                    string response = SendDataGetReply("", true, 500);
                    // Send a BREAK and wait 0.5 seconds
                    if (!string.IsNullOrEmpty(response))
                    {
                        // Shutdown the connection
                        Disconnect();

                        // Something responded
                        return true;
                    }

                    // Shutdown the connection
                    Disconnect();
                }
            }
            catch (Exception e)
            {
                log.Error("Error Searching for an ADCP", e);

                Disconnect();
                return false;
            }

            // Never got a response
            Disconnect();
            return false;
        }

        /// <summary>
        /// Test each baud rate until a good baud rate is found for the 
        /// port given.  
        /// 
        /// A good baud rate is one that when the BREAK statement
        /// is decode, it has valid data.  If the BREAK statement is
        /// garabage, it cannot be decoded.
        /// 
        /// A quick way to decode the BREAK statement is to just check
        /// for a phrase in the buffer.
        /// </summary>
        /// <param name="port">Serial port to test.</param>
        /// <returns>Options to connect to the serial port.</returns>
        public AdcpSerialOptions TestSerialBaudConnection(string port)
        {
            // Get a list of all the baud rates
            //List<int> bauds = SerialOptions.BaudRateOptions;
            List<int> bauds = new List<int>();
            bauds.Add(115200);
            bauds.Add(921600);
            //bauds.Add(460800);
            //bauds.Add(230400);
            //bauds.Add(115200);
            //bauds.Add(38400);
            bauds.Add(19200);
            //bauds.Add(9600);

            //bauds.Insert(0, SerialOptions.DEFAULT_BAUD);                // Add this to the front of the list so the default is tried first to speed up the process

            // Ensure the the serial port is not open now
            if (IsOpen())
            {
                Disconnect();
            }

            // Test all the baud rates until one is found
            foreach (var baud in bauds)
            {
                // Create a serial connection
                _serialOptions = new SerialOptions();
                _serialOptions.Port = port;
                _serialOptions.BaudRate = baud;
                Connect();

                // Verify the connection was created
                if (IsOpen())
                {
                    // Clear the buffer
                    ReceiveBufferString = "";

                    // Send a break to the Port and see if there was a response
                    bool response = SendDataWaitReply(Commands.AdcpCommands.CMD_BREAK, 500);              // True will send a break with no command
                    
                    Thread.Sleep(AdcpSerialPort.WAIT_STATE);                                              // Wait to get the BREAK response, the initial response will be the echo back of BREAK 
                    
                    if (response)
                    {
                        if (!string.IsNullOrEmpty(ReceiveBufferString))
                        {
                            // Decode the data to see if the response is not garabage
                            if (ReceiveBufferString.Contains(" Rowe Technologies Inc."))
                            {

                                // Decode the Break response
                                Commands.BreakStmt breakStmt = Commands.AdcpCommands.DecodeBREAK(ReceiveBufferString);

                                // Close the connection
                                Disconnect();

                                // Return the options used to find the ADCP
                                return new AdcpSerialOptions() { SerialOptions = _serialOptions, SerialNumber = breakStmt.SerialNum, Firmware = breakStmt.FirmwareVersion, Hardware = breakStmt.Hardware };
                            }
                            else
                            {
                                // Nothing found for this baud rate so shutdown the connection
                                Disconnect();
                            }

                        }
                    }
                    else
                    {
                        // Nothing found for this baud rate so shutdown the connection
                        Disconnect();
                    }
                }
            }

            return null;
        }

        #endregion

        #region AdcpConfiguration

        /// <summary>
        /// Get the ADCP Configuration.  This
        /// will decode the BREAK statement
        /// and also CSHOW.
        /// </summary>
        /// <returns></returns>
        public AdcpConfiguration GetAdcpConfiguration()
        {
            // Stop pinging just in case
            StopPinging();

            // Send a BREAK to get the serial number
            //string breakResult = _adcpSerialPort.SendDataGetReply("", true, RTI.AdcpSerialPort.WAIT_STATE * 3);
            SendBreak();
            // Wait for an output
            Thread.Sleep(RTI.AdcpSerialPort.WAIT_STATE);

            // Get the buffer output
            string breakResult = ReceiveBufferString;
            BreakStmt breakStmt = AdcpCommands.DecodeBREAK(breakResult);

            // Wait for an output
            Thread.Sleep(RTI.AdcpSerialPort.WAIT_STATE);

            // Send a command for CSHOW to the ADCP
            // Decode the CSHOW command
            string result = SendDataGetReply(RTI.Commands.AdcpCommands.CMD_CSHOW);

            // Decode CSHOW for all the settings
            AdcpConfiguration adcpConfig = AdcpCommands.DecodeCSHOW(result, breakStmt.SerialNum);

            AdcpSerialOptions adcpSerialOptions = new AdcpSerialOptions();
            adcpSerialOptions.Firmware = breakStmt.FirmwareVersion;
            adcpSerialOptions.Hardware = breakStmt.Hardware;
            adcpSerialOptions.SerialNumber = breakStmt.SerialNum;
            adcpSerialOptions.SerialOptions = SerialOptions;

            // Set the ADCP serial options to the configuraiton
            adcpConfig.SerialOptions = adcpSerialOptions;

            return adcpConfig;
        }

        #endregion

        #region Directory Listings

        /// <summary>
        /// Download the list of file from the ADCP.  A command
        /// is sent to give the listing of all the files on the ADCP.
        /// Store the results to _dirListingString.  This is done
        /// by subscribing to the serial output.  Then send the command.
        /// Receive all the lines from the serial output until it finds
        /// the end of the command.  Then unsubscribe to receive
        /// serial output.
        /// </summary>
        private void DownloadDirectoryListing()
        {
            int TIMEOUT = 20;

            // Clear the buffer
            ReceiveBufferString = "";
            _dirListingString = "";

            // Subscribe to receive serial string data
            this.ReceiveSerialData += new ReceiveSerialDataEventHandler(GetDirListing_ReceiveSerialData);

            if (IsAvailable())
            {
                // Send the command to get the directory listing
                SendData(string.Format("{0}", RTI.Commands.AdcpCommands.CMD_DSDIR));

                // Added an extra wait to Wait for response
                Thread.Sleep(WAIT_STATE);
            }

            // Wait for the complete list
            // "Used Space: ..." is the last line
            // Check ReceiveBufferString and not _dirListingString because it is a smaller string
            while (ReceiveBufferString.IndexOf("Used Space:") <= 0)
            {
                Thread.Sleep(WAIT_STATE * 2);

                // Stop looking after timeout has occured
                if (TIMEOUT <= 0)
                {
                    break;
                }
                TIMEOUT--;
            }

            // Unsubscribe from the event
            this.ReceiveSerialData -= GetDirListing_ReceiveSerialData;
        }

        /// <summary>
        /// Send the command to display the Directory listing.
        /// Then return the results from the serial display.
        /// </summary>
        /// <returns>List of the directory files.</returns>
        public RTI.Commands.AdcpDirListing GetDirectoryListing()
        {
            // Download the directory listing to _dirListingString
            DownloadDirectoryListing();

            // Filter the list of directories
            RTI.Commands.AdcpDirListing dirListing = RTI.Commands.AdcpCommands.DecodeDSDIR(_dirListingString);

            // Clear the string
            _dirListingString = "";

            // Return the content of the buffer
            return dirListing;
        }

        /// <summary>
        /// Event handler for serial data as a string.
        /// This will populate the directory string with incoming data.
        /// </summary>
        /// <param name="data">Incoming data from serial port as a string.</param>
        private void GetDirListing_ReceiveSerialData(string data)
        {
            // Store the directory listing
            _dirListingString += data;
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

        #region Upload Events

        #region Upload File Size Event

        /// <summary>
        /// Event to get the file size for the given
        /// file name.  The size will be in bytes.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileSize">Size of the file in bytes.</param>
        public delegate void UploadFileSizeEventHandler(string fileName, long fileSize);

        /// <summary>
        /// Subscribe to receive the event for the size of the file in bytes.
        /// 
        /// To subscribe:
        /// _adcpSerialPort.UploadFileSizeEvent += new serialConnection.UploadFileSizeEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// _adcpSerialPort.UploadFileSizeEvent -= (method to call)
        /// </summary>
        public event UploadFileSizeEventHandler UploadFileSizeEvent;

        #endregion

        #region Upload Progress Event

        /// <summary>
        /// Event to receive the progress of the upload process.
        /// This will give the number of bytes currently written
        /// for the current file being uploaded.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="bytesWritten">Current bytes written.</param>
        public delegate void UploadProgressEventHandler(string fileName, long bytesWritten);

        /// <summary>
        /// Subscribe to receive the event for the project of the upload.  This will get the
        /// current number of bytes written for the current file being uploaded.
        /// 
        /// To subscribe:
        /// _adcpSerialPort.UploadProgressEvent += new serialConnection.UploadProgressEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// _adcpSerialPort.UploadProgressEvent -= (method to call)
        /// </summary>
        public event UploadProgressEventHandler UploadProgressEvent;

        #endregion

        #region Upload Complete Event

        /// <summary>
        /// Event to receive when the upload is complete
        /// for the given file name.
        /// The parameter goodUpload is used to tell the user
        /// if the upload was completed successfully or the upload
        /// was aborted.
        /// </summary>
        /// <param name="fileName">Name of the file completed the upload.</param>
        /// <param name="goodUpload">Flag if the upload was completed successfully.</param>
        public delegate void UploadCompleteEventHandler(string fileName, bool goodUpload);

        /// <summary>
        /// Subscribe to receive the event when the file has been completely upload.
        /// 
        /// To subscribe:
        /// _adcpSerialPort.UploadCompleteEvent += new serialConnection.UploadCompleteEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// _adcpSerialPort.UploadCompleteEvent -= (method to call)
        /// </summary>
        public event UploadCompleteEventHandler UploadCompleteEvent;

        #endregion

        #endregion

        #endregion

        #region Override

        /// <summary>
        /// Display the object to a string.
        /// </summary>
        /// <returns>String of the object.</returns>
        public override string ToString()
        {
            string mode = "";

            switch (Mode)
            {
                case AdcpSerialModes.COMPASS:
                    mode = "Compass Mode: ";
                    break;
                case AdcpSerialModes.DOWNLOAD:
                    mode = "Download Mode: ";
                    break;
                case AdcpSerialModes.UPLOAD:
                    mode = "Upload Mode: ";
                    break;
                case AdcpSerialModes.ADCP:
                default:
                    mode = "ADCP Mode: ";
                    break;
            }

            return mode + SerialOptions.ToString();
        }

        #endregion

    }
}