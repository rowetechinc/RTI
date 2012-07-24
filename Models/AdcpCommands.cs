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
 * 10/06/2011      RC                     Initial coding
 * 10/11/2011      RC                     Adding ToString for send to ADCP
 *                                         Removed IDataErrorInfo
 * 11/17/2011      RC                     Added GetCommandList().
 * 11/18/2011      RC                     Removed CR from end of GetTimeCommand().
 * 11/21/2011      RC                     Added ==, !=, HashCode and Equal to TimeValue for unit test. 
 * 12/05/2011      RC                     Added min/max Hundredth of a second.
 * 01/31/2012      RC          1.14       Added new commands for User Guide Rev F.
 * 02/01/2012      RC          1.14       Moved some Enums and class outside this class but in the namespace.
 *                                         Removed WP, BT and WT to a AdcpSubsystemCommands.
 *                                         Take serial number in constructor to know subsystems.
 * 02/07/2012      RC          2.00       Made all the TimeValue variables use only the time value variables. (CEI)
 *                                         Created TimeValue.Empty.
 *                                         Added method SetDefaults().
 *                                         Removed Frequency.
 *                                         Added additional baudrates.
 * 02/10/2012      RC          2.02       Added GetDeploymentCommandList().
 *                                         Removed Time of First Ping from GetCommandList();
 *                                         Changed default values to match Adcp User Guide Rev F and CDEFAULT in firmware v0.2.04.
 * 04/13/2012      RC          2.09       Added CMD_DS_CANCEL.
 * 05/01/2012      RC          2.11       Added functions to decode the ENGPNI and BREAK.
 * 06/26/2012      RC          2.12       Added CMD_BREAK to have a command for the command list in AdcpSerialPort::SendCommands().
 * 07/10/2012      RC          2.12       Decode ENGI2CSHOW.
 *                                         Added classes for board serial number and revisions and register information for ENGI2CSHOW decoding.
 *                                         Made decode BREAK statement save the firmware as an object.
 * 07/17/2012      RC          2.12       Added DecodeSTIME() to decode the command STIME result.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;


namespace RTI
{
    namespace Commands
    {
        #region Classes and Enums

        /// <summary>
        /// Enum to handle commands that pass a 0 or 1 for enable for disable.
        /// </summary>
        public enum EnableDisable
        {
            /// <summary>
            /// Disable.
            /// </summary>
            DISABLE = 0,

            /// <summary>
            /// Enable.
            /// </summary>
            ENABLE = 1
        };

        /// <summary>
        /// Baud rate options for C232B, C422B and C485B.
        /// </summary>
        public enum Baudrate
        {
            /// <summary>
            /// 2400 Baudrate.
            /// </summary>
            BAUD_2400 = 2400,

            /// <summary>
            /// 4800 Baudrate.
            /// </summary>
            BAUD_4800 = 4800,

            /// <summary>
            /// 9600 Baudrate.
            /// </summary>
            BAUD_9600 = 9600,

            /// <summary>
            /// 19200 Baudrate.
            /// </summary>
            BAUD_19200 = 19200,

            /// <summary>
            /// 38400 Baudrate.
            /// </summary>
            BAUD_38400 = 38400,

            /// <summary>
            /// 115200 Baudrate.
            /// </summary>
            BAUD_115200 = 115200,

            /// <summary>
            /// 921600 Baudrate.
            /// </summary>
            BAUD_921600 = 921600,

            /// <summary>
            /// 460800 Baudrate.
            /// </summary>
            BAUD_460800 = 460800,

            /// <summary>
            /// 230400 Baudrate.
            /// </summary>
            BAUD_230400 = 230400
        };

        /// <summary>
        /// Heading source options.
        /// </summary>
        public enum HeadingSrc
        {
            /// <summary>
            /// Internal Heading source.
            /// Internal (PNI) compass.
            /// </summary>
            INTERNAL = 1,

            /// <summary>
            /// External Heading source.
            /// GPS HDT strings via serial port.
            /// </summary>
            SERIAL = 2
        };

        /// <summary>
        /// TimeValue used to send command CWPAI.
        /// </summary>
        public class TimeValue
        {
            /// <summary>
            /// Hours.
            /// </summary>
            public UInt16 Hour { get; set; }

            /// <summary>
            /// Minutes.
            /// </summary>
            public UInt16 Minute { get; set; }

            /// <summary>
            /// Seconds.
            /// </summary>
            public UInt16 Second { get; set; }

            /// <summary>
            /// Hundredths of a second.
            /// </summary>
            public UInt16 HunSec { get; set; }

            /// <summary>
            /// Default value is all zeros.
            /// </summary>
            public TimeValue()
            {
                Hour = 0;
                Minute = 0;
                Second = 0;
                HunSec = 0;
            }

            /// <summary>
            /// Set the time.
            /// </summary>
            /// <param name="hour">Hour value</param>
            /// <param name="minute">Minute value</param>
            /// <param name="second">Second value</param>
            /// <param name="hunSec">Hundreth of second value</param>
            public TimeValue(UInt16 hour, UInt16 minute, UInt16 second, UInt16 hunSec)
            {
                Hour = hour;
                Minute = minute;
                Second = second;
                HunSec = hunSec;
            }

            /// <summary>
            /// Return the string version of this class.
            /// </summary>
            /// <returns>String version of this class.  HH:MM:SS.hh</returns>
            public override string ToString()
            {
                return Hour.ToString("00") + ":" + Minute.ToString("00") + ":" + Second.ToString("00") + "." + HunSec.ToString("00");
            }

            /// <summary>
            /// Create an equal operator to check if the
            /// given is equal to each other.
            /// </summary>
            /// <param name="a">Timevalue to compare.</param>
            /// <param name="b">Timevalue to compare.</param>
            /// <returns>TRUE if hour, minute, second and hunsec equal.</returns>
            public static bool operator ==(TimeValue a, TimeValue b)
            {
                // If both are null, or both are same instance, return true.
                if (System.Object.ReferenceEquals(a, b))
                {
                    return true;
                }

                // If one is null, but not both, return false.
                if (((object)a == null) || ((object)b == null))
                {
                    return false;
                }

                if (a.Hour == b.Hour &&
                    a.Minute == b.Minute &&
                    a.Second == b.Second &&
                    a.HunSec == b.HunSec)
                {
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Check if the 2 time values are not equal.
            /// </summary>
            /// <param name="a">First time value to compare.</param>
            /// <param name="b">Second time value to compare.</param>
            /// <returns>TRUE if not equal.</returns>
            public static bool operator !=(TimeValue a, TimeValue b)
            {
                return !(a == b);
            }

            /// <summary>
            /// Create a hashcode for the object.
            /// </summary>
            /// <returns>Hashcode for the object.</returns>
            public override int GetHashCode()
            {
                return this.Hour ^ this.Minute ^ this.Second ^ this.HunSec;
            }

            /// <summary>
            /// Check if the object is equal to the given object.
            /// </summary>
            /// <param name="obj">Object to compare.</param>
            /// <returns>TRUE if they are equal.</returns>
            public override bool Equals(object obj)
            {
                // If parameter cannot be cast to ThreeDPoint return false:
                TimeValue p = obj as TimeValue;
                if ((object)p == null)
                {
                    return false;
                }

                if (this.Hour == p.Hour &&
                    this.Minute == p.Minute &&
                    this.Second == p.Second &&
                    this.HunSec == p.HunSec)
                {
                    return true;
                }

                return false;
            }
        }

        #region Decode Structs

        #region Heading, Pitch Roll

        /// <summary>
        /// Heading, pitch and roll.
        /// </summary>
        public struct HPR
        {
            /// <summary>
            /// Heading in degrees.
            /// </summary>
            public double Heading { get; set; }

            /// <summary>
            /// Pitch in degrees.
            /// </summary>
            public double Pitch { get; set; }

            /// <summary>
            /// Roll in degrees.
            /// </summary>
            public double Roll { get; set; }
        }

        #endregion

        #region BREAK

        /// <summary>
        /// Struct to hold the Break statement
        /// values.
        /// </summary>
        public struct BreakStmt
        {
            /// <summary>
            /// Serial number.
            /// </summary>
            public RTI.SerialNumber SerialNum { get; set; }

            /// <summary>
            /// Firmware version.
            /// </summary>
            public Firmware FirmwareVersion { get; set; }

            /// <summary>
            /// Hardware string.
            /// </summary>
            public string Hardware { get; set; }
        }

        #endregion

        #region I2C Memory Devices

        /// <summary>
        /// I2C Board Id.
        /// </summary>
        public enum I2cBoardId
        {
            /// <summary>
            /// I/O Board.
            /// </summary>
            IO = 50007,

            /// <summary>
            /// Low Power Regulator Board.
            /// </summary>
            LOW_PWR_REG = 50012,

            /// <summary>
            /// Transmitter Board.
            /// </summary>
            XMITTER = 50009,

            /// <summary>
            /// Virtual Ground Board.
            /// </summary>
            VIRTUAL_GND = 50018,

            /// <summary>
            /// Receiver Board.
            /// </summary>
            RCVR = 50022,

            /// <summary>
            /// Backplane Board.
            /// </summary>
            BACKPLANE = 50016,

            /// <summary>
            /// Unknown Board ID.
            /// </summary>
            UNKNOWN = 0
        }

        /// <summary>
        /// Board ID, Revision and Serial number as displayed
        /// from ENGI2CSHOW.
        /// </summary>
        public class I2cBoard
        {
            /// <summary>
            /// Board ID.
            /// </summary>
            public I2cBoardId ID { get; set; }

            /// <summary>
            /// Board Revision.
            /// </summary>
            public string Revision { get; set; }

            /// <summary>
            /// Board Serial Number.
            /// </summary>
            public int SerialNum { get; set; }

            /// <summary>
            /// Constructor.
            /// </summary>
            public I2cBoard()
            {
                ID = I2cBoardId.UNKNOWN;
                Revision = "";
                SerialNum = 0;
            }

            /// <summary>
            /// Parse the serial number.  If it fails,
            /// the serial number will be 0 and false will be
            /// returned.
            /// </summary>
            /// <param name="serial">Serial number to try and set.</param>
            /// <returns>TRUE = serial number set.</returns>
            public bool SetSerial(string serial)
            {
                int sn = 0;
                if(int.TryParse(serial, out sn))
                {
                
                    SerialNum = sn;
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Output the ID and all the register values.
            /// </summary>
            /// <returns>String of this object.</returns>
            public override string ToString()
            {
                string id = ID.ToString();
                return string.Format("{0}  Rev: {1}  Serial: {2}\n", id.PadRight(15), Revision.PadRight(5), SerialNum);
            }
        }

        /// <summary>
        /// The register values for a receiver.  This includes
        /// the ID given from ENGI2CSHOW.
        /// </summary>
        public class I2cRegister
        {
            /// <summary>
            /// ID for the receiver register.
            /// </summary>
            public int ID { get; set; }

            /// <summary>
            /// Register values.  This will be the list of 
            /// values in the register.  It is a list just in case
            /// in the future the number of registers changes.
            /// </summary>
            public List<string> RegValues { get; set; }

            /// <summary>
            /// Constructor.
            /// 
            /// Initialize the list.
            /// </summary>
            public I2cRegister()
            {
                // Initialize the values
                ID = 0;
                RegValues = new List<string>();
            }

            /// <summary>
            /// Output the ID and all the register values.
            /// </summary>
            /// <returns>String of this object.</returns>
            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                string id = ID.ToString();
                builder.AppendFormat("ID: {0}  Regs: ", id.PadRight(5));
                foreach(string str in RegValues)
                {
                    builder.AppendFormat("{0} ", str);
                }

                builder.Append("\n");
                return builder.ToString();
            }
        }

        /// <summary>
        /// All information retrieved from ENGI2CSHOW.  This includes all the board's
        /// revision and serial number and the RTC and receiver registers.
        /// </summary>
        public class I2cMemDevs
        {
            /// <summary>
            /// List of Receiver registers.
            /// Receiver currently contains 27 registers.
            /// </summary>
            public List<I2cRegister> RvcrRegs;

            /// <summary>
            /// List of RTC registers.
            /// RTC currently contains 19 registers.
            /// </summary>
            public List<I2cRegister> RtcRegs { get; set; }

            /// <summary>
            /// I/O board information.
            /// </summary>
            public I2cBoard IoBoard { get; set; }

            /// <summary>
            /// Low Power Regulator board information.
            /// </summary>
            public I2cBoard LowPwrRegBoard { get; set; }

            /// <summary>
            /// Transmitter board information.
            /// </summary>
            public I2cBoard XmitterBoard { get; set; }

            /// <summary>
            /// Virtual Ground board information.
            /// </summary>
            public I2cBoard VirtualGndBoard { get; set; }

            /// <summary>
            /// Receiver board information.
            /// </summary>
            public I2cBoard RcvrBoard { get; set; }

            /// <summary>
            /// Backplane board information.
            /// </summary>
            public I2cBoard BackPlaneBoard { get; set; }

            /// <summary>
            /// Constructor.
            /// 
            /// Initialize lists.
            /// </summary>
            public I2cMemDevs()
            {
                // Initialize lits
                RvcrRegs = new List<I2cRegister>();
                RtcRegs = new List<I2cRegister>();
                IoBoard = new I2cBoard();
                LowPwrRegBoard = new I2cBoard();
                XmitterBoard = new I2cBoard();
                VirtualGndBoard = new I2cBoard();
                RcvrBoard = new I2cBoard();
                BackPlaneBoard = new I2cBoard();
            }

            /// <summary>
            /// Output the object as a string.  Display
            /// all the strings of the objects.
            /// </summary>
            /// <returns>This object as a string.</returns>
            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("Receiver Registers\n");
                builder.Append("------------------------\n");
                foreach (I2cRegister rcvrReg in RvcrRegs)
                {
                    builder.Append(rcvrReg.ToString());
                }

                builder.Append("\nRTC Registers\n");
                builder.Append("------------------------\n");
                foreach (I2cRegister rtcReg in RtcRegs)
                {
                    builder.Append(rtcReg.ToString());
                }

                builder.Append("\nBoards\n");
                builder.Append("------------------------\n");
                builder.AppendFormat("{0}", IoBoard.ToString());
                builder.AppendFormat("{0}", LowPwrRegBoard.ToString());
                builder.AppendFormat("{0}", XmitterBoard.ToString());
                builder.AppendFormat("{0}", VirtualGndBoard.ToString());
                builder.AppendFormat("{0}", RcvrBoard.ToString());
                builder.AppendFormat("{0}", BackPlaneBoard.ToString());

                return builder.ToString();
            }
        }

        #endregion

        #endregion

        #endregion

        /// <summary>
        /// Commands handled by the ADCP.
        /// </summary>
        public class AdcpCommands
        {
            #region Varialbles

            /// <summary>
            /// Serial number to know how many 
            /// subsystem exist in the current 
            /// ADCP.
            /// </summary>
            private SerialNumber _serialNumber;

            #endregion

            #region Commands

            //public const char CR = '\r';

            /// <summary>
            /// Used for commands list to give a command for a BREAK.
            /// </summary>
            public const string CMD_BREAK = "BREAK";

            #region Pinging

            /// <summary>
            /// Command: Start pinging.
            /// ADCP starts pinging.
            /// </summary>
            public const string CMD_START_PINGING = "START";
            
            /// <summary>
            /// Command: Stop pinging.
            /// ADCP Stops pinging.  This is the only
            /// command the ADCP will accept if it is pinging.
            /// </summary>
            public const string CMD_STOP_PINGING = "STOP";

            /// <summary>
            /// Command Help.
            /// Return a list of all the available commands.
            /// </summary>
            public const string CMD_HELP = "Help";

            #endregion

            #region Modes

            /// <summary>
            /// Command: DVL Mode
            /// Parameter: 
            /// Set the ADCP to DVL mode.   The system, when
            /// pinging is started will output NMEA formatted 
            /// data.  Bottom Track and Water Tracking cells
            /// are supported in this mode.
            /// </summary>
            public const string CMD_CDVL = "CDVL";

            /// <summary>
            /// Command: Profile Mode.
            /// Parameter: 
            /// The system, when pinging is started will output
            /// binary MATLAB formatted data.  Bottom Track and
            /// multi-cell water profiling supported in this mode.
            /// </summary>
            public const string CMD_CPROFILE = "CPROFILE";

            #endregion

            #region Ensemble

            /// <summary>
            /// Ensemble Command: Ensemble Interval
            /// Parameter: HH:MM:SS.hh 
            /// Sets the time interval that system will output the 
            /// averaged profile/bottom track data. 
            /// </summary>
            public const string CMD_CEI = "CEI";

            /// <summary>
            /// Ensemble Command: Ensemble Time of First Ping
            /// Parameter: YYYY/MM/DD,HH:mm:SS.hh
            /// Sets the time that the system will awaken and start pinging. 
            /// </summary>
            public const string CMD_CETFP = "CETFP";

            /// <summary>
            /// Ensemble Command: Ensemble Recording
            /// Parameter: 0=disable, 1=enable
            /// When recording is enabled the ADCP searches for the next available 
            /// file number to record to. The ensemble file name starts with the 
            /// letter “A” followed by a 7 digit number and ending with the 
            /// extension “.ens”. For example: The first ensemble file will 
            /// be named “A0000001.ens”. During deployment as each ensemble 
            /// is completed the data is appended to the current file. The 7 
            /// digit number following the “A” is incremented each time the 
            /// system is (re)started or when the file size exceeds 16777216 
            /// bytes. 
            /// </summary>
            public const string CMD_CERECORD = "CERECORD";

            /// <summary>
            /// Ensemble Command: Ensemble Ping Order
            /// Parameter: cccccccccccccccc
            /// Sets the order in which the various subsystems will be pinged. 
            /// Note: a space and at least one subsystem code must follow 
            /// the CEPO command or the system will reject the command.
            /// </summary>
            public const string CMD_CEPO = "CEPO";

            #endregion

            #region Environmental

            /// <summary>
            /// Environmental Command: Salinity
            /// Parameter: n.nn (ppt) 
            /// Used in the water Speed of Sound calculation
            /// </summary>
            public const string CMD_CWS = "CWS";

            /// <summary>
            /// Environmental Command: Water Temperature 
            /// Parameter: n.nnn (degrees)  
            /// Used in the water Speed of Sound calculation if the 
            /// temperature sensor is not available
            /// </summary>
            public const string CMD_CWT = "CWT";

            /// <summary>
            /// Environmental Command: Tranducer Depth
            /// Parameter: n.nnn (meters)
            /// Used in the water Speed of Sound calculation
            /// </summary>
            public const string CMD_CTD = "CTD";

            /// <summary>
            /// Environmental Command: Speed of Sound
            /// Parameter: n.nnn (meters per second)  
            /// Not used
            /// </summary>
            public const string CMD_CWSS = "CWSS";

            /// <summary>
            /// Environmental Command: Heading Offset
            /// Parameter: n.nnn (+-180 deg) 
            /// Added to the compass output prior to heading being used within the system
            /// </summary>
            public const string CMD_CHO = "CHO";

            /// <summary>
            /// Environmental Command: Heading Source
            /// Parameter: N = 0 to 2
            /// n=1 Internal (PNI) compass, 
            /// n=2 GPS HDT string via either serial port
            /// Select the heading source for ENU transformations. 
            /// </summary>
            public const string CMD_CHS = "CHS";

            #endregion

            #region COMM Port

            /// <summary>
            /// Serial Communication Command: RS-232 Baudrate
            /// Parameter: n = 2400, 4800, 9600, 19200, 38400, 115200
            /// The ADCP contains a RS-232 port.  Set its baudrate.
            /// </summary>
            public const string CMD_C232B = "C232B";

            /// <summary>
            /// Serial Communication Command: RS-485 Baudrate
            /// Parameter: n = 2400, 4800, 9600, 19200, 38400, 115200
            /// The ADCP contains a RS-485 port.  Set its baudrate.
            /// </summary>
            public const string CMD_C485B = "C485B";

            /// <summary>
            /// Serial Communication Command: RS-422 Baudrate
            /// Parameter: n = 2400, 4800, 9600, 19200, 38400, 115200
            /// The ADCP contains a RS-422 port.  Set its baudrate.
            /// </summary>
            public const string CMD_C422B = "C422B";

            #endregion

            #region System Config

            /// <summary>
            /// System Config Command: Load Config File
            /// Parameter: 
            /// Loads the file "SYSCONF.BIN", contained on the SD card, into the system configuration.
            /// </summary>
            public const string CMD_CLOAD = "CLOAD";

            /// <summary>
            /// System Config Command: Save Config File
            /// Parameter: 
            /// Saves a copy of the system configuration to the file "SYSCONF.BIN" on the SD card.
            /// </summary>
            public const string CMD_CSAVE = "CSAVE";

            /// <summary>
            /// System Config Command: Display System Configuration
            /// Parameter: 
            /// Cause the system to display/output the system configuration
            /// </summary>
            public const string CMD_CSHOW = "CSHOW";

            /// <summary>
            /// System Config Command: Load Factory Defaults
            /// Parameters: 
            /// Restores the system configuration to factory defaults.
            /// </summary>
            public const string CMD_CDEFAULT = "CDEFAULT";

            #endregion

            #region Firmware

            /// <summary>
            /// Firmware Command: Show Firmware Version
            /// Parameter: 
            /// Show firmware version.
            /// </summary>
            public const string CMD_FMSHOW = "FMSHOW";

            /// <summary>
            /// Firmware Command: Copy Boot Firmware
            /// Parameter:
            /// Copy the boot firmware "rtiboota.Bin" from the secure digital storage to internal flash.
            /// </summary>
            public const string CMD_FMCopyB = "FMCopyB";

            /// <summary>
            /// Firmware Command: Copy System Firmware
            /// Parameter:
            /// Copy the system firmware "rtisys.Bin" from the secure digital storage to internal flash
            /// </summary>
            public const string CMD_FMCopyS = "FMCopyS";

            //public const string CMD_START = "START";            /// Start pinging continously.  Once started the system will only respond to the STOP command
            //public const string CMD_STOP = "STOP";              /// Stop pinging

            #endregion

            #region Deployment

            /// <summary>
            /// Deployment Command: Power Down System
            /// Parameter:
            /// Power down the system to sleep mode.
            /// </summary>
            public const string CMD_SLEEP = "SLEEP";

            /// <summary>
            /// Deployment Command: Set or Display System Time
            /// Parameter: "yyyy/MM/dd,HH:mm:ss"
            /// Set or Display system time.
            /// </summary>
            public const string CMD_STIME = "STIME";

            #endregion

            #region Data Storage

            /// <summary>
            /// Data Storage Command: Receive a File.
            /// Parameter: filename.abc
            /// Data Storage XMODEM Receive.  This command is used 
            /// to transfer a file, via the serial communication 
            /// link, from an external device to the SD card contained 
            /// within the RTI system.  File names are limited to a 
            /// maximum of 8 characters before the extension
            /// </summary>
            public const string CMD_DSXR = "DSXR";

            /// <summary>
            /// Data Storage Command: Transmit a File.
            /// Parameter: filename.abc
            /// Data Storage XMODEM Transmit.  This command is used 
            /// to transfer a file, via the serial communication 
            /// link, from an external device to the SD card contained 
            /// within the RTI system.  File names are limited to a 
            /// maximum of 8 characters before the extension
            /// </summary>
            public const string CMD_DSXT = "DSXT";

            /// <summary>
            /// Data Storage Command: Format SD Card
            /// Paramater: 
            /// Data Storage format. Completely erase the SD card.
            /// </summary>
            public const string CMD_DSFORMAT = "DSFORMAT";

            /// <summary>
            /// Data Storage Command: Show Directory
            /// Parameter:
            /// Data Storage Directory.  Show a directory of stored files.
            /// </summary>
            public const string CMD_DSDIR = "DSDIR";

            /// <summary>
            /// Data Storage Command: Show SD Card usage
            /// Parameter:
            /// Data Storage Show.  Show SD card usage
            /// </summary>
            public const string CMD_DSSHOW = "DSSHOW";

            /// <summary>
            /// Send this command to cancel a download progress.
            /// </summary>
            public const string CMD_DS_CANCEL = "D";

            #endregion

            #region Compass

            /// <summary>
            /// Diagnostic Command: Compass Pass Thru
            /// Parameter:
            /// Diagnostic Compass Pass Thru.  Allows an external device 
            /// to connect directly to the internal compass via a serial 
            /// communication link.  To disconnect the pass-thru the external 
            /// device must send 16 consecutive X's (XXXXXXXXXXXXXXXX) to 
            /// the system.  The pass thru allows other manufacturer software 
            /// to access and calibrate the compass.
            /// </summary>
            public const string CMD_DIAGCPT = "DIAGCPT";

            /// <summary>
            /// Diagnostic Command: Disconnect Compass Pass Thru
            /// Parameter:
            /// To disconnect the pass-thru the external device must send 
            /// 16 consecutive X's (XXXXXXXXXXXXXXXX) to the system.
            /// </summary>
            public const string CMD_DIAGCPT_DISCONNECT = "XXXXXXXXXXXXXXXX";

            #endregion

            #region Engineering Commands

            /// <summary>
            /// Engineering Command: Ping Once
            /// Parameter: 
            /// Ping Once.
            /// </summary>
            public const string CMD_PING = "PING";

            /// <summary>
            /// Engineering Command: Ping Once no transmite output.
            /// Parameter: nnnn (Raw data samples)
            /// Ping once no transmit output.  
            /// </summary>
            public const string CMD_PINGR = "PINGR";

            /// <summary>
            /// Engineering Command: Ping once output Max RSSI
            /// Parameter: nnnn (Raw data samples)
            /// Ping once output Max RSSI. 
            /// </summary>
            public const string CMD_PINGM = "PINGM";

            /// <summary>
            /// Engineering Command: Beam Set Select
            /// Parameter:
            /// Beam Set Select.
            /// </summary>
            public const string CMD_ENGBS = "ENGBS";

            /// <summary>
            /// Engineering Command: Gain Select
            /// Parameter: N = 0 to 1
            /// 0=Low 
            /// 1=high
            /// Gain Select.
            /// </summary>
            public const string CMD_ENGGS = "ENGGS";

            /// <summary>
            /// Engineering Command: Ping with Fixed Transmit
            /// Parameter: nnn (carrier cycles)
            /// Start pinging with fixed transmit.
            /// </summary>
            public const string CMD_ENGSTART = "ENGSTART";

            /// <summary>
            /// Engineering Command: Display System Noise and Temp
            /// Parameter: 
            /// Display System Noise and Temperature.
            /// </summary>
            public const string CMD_ENGSAMP = "ENGSAMP";

            /// <summary>
            /// Engineering Command: Ethernet Link
            /// Parameter: N = 0 to 1 
            /// 0=disable
            /// 1=enable MAC
            /// Establish Ethernet link. 
            /// </summary>
            public const string CMD_ENGMACON = "ENGMACON";

            /// <summary>
            /// Engineering Command: MAC Address
            /// Paramter: xx:xx:xx:xx:xx:xx
            /// Set the system MAC address.  
            /// </summary>
            public const string CMD_ENGMAC = "ENGMAC";

            /// <summary>
            /// Engineering Command: Toggle Don't Stop LO after pinging
            /// Paramter:
            /// Toggle Don't Stop LO after pinging.
            /// </summary>
            public const string CMD_ENGDS = "ENGDS";

            /// <summary>
            /// Engineering Command: FPGA clock divisor
            /// Parameter: n (divisor)
            /// Set FPGA clock divisor to n.
            /// </summary>
            public const string CMD_ENGSDIV = "ENGSDIV";

            /// <summary>
            /// Engineering Command: Toggle Force BT Deep
            /// Parameter: 
            /// Toggle force BT DEEP.
            /// </summary>
            public const string CMD_ENGBTDEEP = "ENGBTDEEP";

            /// <summary>
            /// Engineering Command: Show Heading, Pitch and Roll
            /// Parameter:
            /// Show HPR ranges.
            /// </summary>
            public const string CMD_ENGPNI = "ENGPNI";

            /// <summary>
            /// Engineering Command: Change IP address
            /// Parameter: nnn.nnn.nnn.nnn
            /// Change the IP address.  
            /// </summary>
            public const string CMD_IP = "IP";

            #endregion

            #endregion

            #region Default Values

            /// <summary>
            /// Default RS-232 Baudrate.
            /// </summary>
            public const Baudrate DEFAULT_C232B = Baudrate.BAUD_115200;

            /// <summary>
            /// Default RS-485 Baudrate.
            /// </summary>
            public const Baudrate DEFAULT_C485B = Baudrate.BAUD_115200;

            /// <summary>
            /// Default RS-422 Baudrate.
            /// </summary>
            public const Baudrate DEFAULT_C422B = Baudrate.BAUD_230400;

            /// <summary>
            /// Default ADCP mode.
            /// </summary>
            public const AdcpMode DEFAULT_MODE = AdcpMode.PROFILE;

            /// <summary>
            /// Default MAC enable.
            /// </summary>
            public const EnableDisable DEFAULT_ENGMACON = EnableDisable.DISABLE;

            /// <summary>
            /// Default Salinity.
            /// </summary>
            public const float DEFAULT_CWS = 0.0f;

            /// <summary>
            /// Default Temperature.
            /// </summary>
            public const float DEFAULT_CWT = 15.0f;

            /// <summary>
            /// Default Transducer Depth.
            /// </summary>
            public const float DEFAULT_CTD = 0.0f;

            /// <summary>
            /// Default Speed of Sound.
            /// </summary>
            public const float DEFAULT_CWSS = 1500.0f;

            /// <summary>
            /// Default Heading Offset.
            /// </summary>
            public const float DEFAULT_CHO = 0.0f;

            /// <summary>
            /// Default Heading source.
            /// </summary>
            public const HeadingSrc DEFAULT_CHS = HeadingSrc.INTERNAL;

            /// <summary>
            /// Default Ensemble interval.
            /// 1 second.
            /// </summary>
            public TimeValue DEFAULT_CEI = new TimeValue(0, 0, 1, 0);

            /// <summary>
            /// Default Year to start pinging.
            /// 2012
            /// </summary>
            public const UInt16 DEFAULT_CETFP_YEAR = 12;

            /// <summary>
            /// Default Month to start pinging.
            /// Janurary.
            /// </summary>
            public const UInt16 DEFAULT_CETFP_MONTH = 1;

            /// <summary>
            /// Default Day to start pinging.
            /// 1
            /// </summary>
            public const UInt16 DEFAULT_CETFP_DAY = 1;

            /// <summary>
            /// Default Hour to start pinging.
            /// </summary>
            public const UInt16 DEFAULT_CETFP_HOUR = 1;

            /// <summary>
            /// Default Minute to start pinging.
            /// </summary>
            public const UInt16 DEFAULT_CETFP_MINUTE = 0;

            /// <summary>
            /// Default Second to start pinging.
            /// </summary>
            public const UInt16 DEFAULT_CETFP_SECOND = 0;

            /// <summary>
            /// Default Hundredth of Second to start pinging.
            /// </summary>
            public const UInt16 DEFAULT_CETFP_HUNSEC = 0;

            /// <summary>
            /// Default whethere to record on the system.
            /// </summary>
            public const bool DEFAULT_CERECORD = false;

            /// <summary>
            /// Default subsystem order to ping.
            /// At least have a value.  So choose 0.
            /// </summary>
            public const string DEFAULT_CEPO = "0";

            #endregion

            #region Min/Max Values
            
            /// <summary>
            /// Minimum heading offset.
            /// </summary>
            public const double MIN_CHO = -180.0;

            /// <summary>
            /// Maximum heading offset.
            /// </summary>
            public const double MAX_CHO = 180.0;
            
            /// <summary>
            /// Minimum IP value.
            /// </summary>
            public const int MIN_IP = 0;

            /// <summary>
            /// Maximum IP value.
            /// </summary>
            public const int MAX_IP = 255;

            /// <summary>
            /// Minimum value for the year.
            /// This will be 2000.
            /// </summary>
            public const int MIN_YEAR = 2000;

            /// <summary>
            /// Maximum value for the year.
            /// This will be 2099.
            /// </summary>
            public const int MAX_YEAR = 2099;

            /// <summary>
            /// Minimum Month.
            /// </summary>
            public const int MIN_MONTH = 1;

            /// <summary>
            /// Maximum month.
            /// </summary>
            public const int MAX_MONTH = 12;

            /// <summary>
            /// Minimum Day.
            /// </summary>
            public const int MIN_DAY = 1;

            /// <summary>
            /// Maximum day.
            /// </summary>
            public const int MAX_DAY = 31;

            /// <summary>
            /// Minimum Hours.
            /// </summary>                           
            public const int MIN_HOUR = 0;

            /// <summary>
            /// Maximum hours.
            /// </summary>
            public const int MAX_HOUR = 23;

            /// <summary>
            /// Minimum Minutes/Seconds.
            /// </summary>
            public const int MIN_MINSEC = 0;

            /// <summary>
            /// Maximum Minutes/Seconds.
            /// </summary>
            public const int MAX_MINSEC = 59;
                                            
            /// <summary>
            /// Minimum Hundredth of a second.
            /// </summary>
            public const int MIN_HUNSEC = 0;

            /// <summary>
            /// Maximum Hundredth of a second.
            /// </summary>
            public const int MAX_HUNSEC = 99;

            #endregion

            #region Enums and Lists
    
            /// <summary>
            /// Enum to handle the possible modes the ADCP can be setup for.
            /// </summary>
            public enum AdcpMode
            {
                /// <summary>
                /// DVL Mode.
                /// </summary>
                DVL,

                /// <summary>
                /// Profile mode.
                /// </summary>
                PROFILE
            };



            #endregion

            #region Properties

            /// <summary>
            /// Mode to put the ADCP in.
            /// 
            /// DVL
            /// The system, when started will output NMEA formatted data.  
            /// Bottom track and single water trackign cell are suppported in this mode.
            /// 
            /// Profile
            /// The system, when started will output binary formatted data.  
            /// Bottom track and multi cell water profiling supported in this mode.
            /// 
            /// Command: CDVL \r
            ///          CPROFILE \r
            /// Range: CDVL or CPROFILE
            /// </summary>
            public AdcpMode Mode { get; set; }

            /// <summary>
            /// Return a string of the mode the system
            /// is in.  
            /// </summary>
            /// <returns>String of the command for the mode.</returns>
            public string Mode_ToString()
            {
                if (Mode == AdcpMode.DVL)
                {
                    return "CDVL";
                }

                if (Mode == AdcpMode.PROFILE)
                {
                    return "CPROFILE";
                }

                return "";
            }

            #region Ensemble Properties

            #region CEI
 
            /// <summary>
            /// Ensemble interval 
            /// Sets the time interval that 
            /// system will output the averaged 
            /// profile/bottom track data. 
            /// 
            /// Command: CEI HH:MM:SS.hh\r
            /// Scale: HH:MM:SS.hh
            /// Range: 0 to 86400 seconds
            /// </summary>
            public TimeValue CEI { get; set; }

            /// <summary>
            /// Return a string representation of CEI.
            /// </summary>
            /// <returns>String representation of CEI</returns>
            public string CEI_ToString()
            {
                return CEI_Hour.ToString("00") + ":" + CEI_Minute.ToString("00") + ":" + CEI_Second.ToString("00") + "." + CEI_HunSec.ToString("00");
            }

            /// <summary>
            /// Hours for CEI.
            /// Limit to 2 digits.
            /// 
            /// When setting the value, if the
            /// value is valid, set it to the
            /// TimeValue for CEI.
            /// 
            /// Scale: Hours
            /// Range: 0 to 23
            /// </summary>
            public UInt16 CEI_Hour
            {
                get { return CEI.Hour; }
                set
                {
                    CEI.Hour = value;
                }
            }

            /// <summary>
            /// Minutes in CEI
            /// Limit to 2 digits.
            /// 
            /// When setting the value, if the
            /// value is valid, set it to the
            /// TimeValue for CEI.
            /// 
            /// Scale: Minutes
            /// Range: 0 to 59
            /// </summary>
            public UInt16 CEI_Minute
            {
                get { return CEI.Minute; }
                set
                {
                    CEI.Minute = value;
                }
            }

            /// <summary>
            /// Seconds in CEI
            /// Limit to 2 digits.
            /// 
            /// When setting the value, if the
            /// value is valid, set it to the
            /// TimeValue for CEI.
            /// 
            /// Scale: Seconds
            /// Range: 0 to 59
            /// </summary>
            public UInt16 CEI_Second
            {
                get { return CEI.Second; }
                set
                {
                    CEI.Second = value;
                }
            }

            /// <summary>
            /// Hundredth of seconds in CEI.
            /// Limit to 2 digits.
            /// 
            /// When setting the value, if the
            /// value is valid, set it to the
            /// TimeValue for CEI.
            /// 
            /// Scale: Hundredth of Second
            /// Range: 0 to 59.
            /// </summary>
            public UInt16 CEI_HunSec
            {
                get { return CEI.HunSec; }
                set
                {
                    CEI.HunSec = value;
                }
            }

            #endregion

            #region CETFP

            /// <summary>
            /// Return a string representation of CETFP.
            /// </summary>
            /// <returns>String representation of CETFP</returns>
            public string CETFP_ToString()
            {
                return CETFP_Year.ToString("0000") + "/" + CETFP_Month.ToString("00") + "/" + CETFP_Day.ToString("00") + "," + CETFP_Hour.ToString("00") + ":" + CETFP_Minute.ToString("00") + ":" + CETFP_Second.ToString("00") + "." + CETFP_HunSec.ToString("00");
            }

            /// <summary>
            /// Years in CETFP.
            /// This starts at 2000.
            /// Limit to 2 digits.
            /// 
            /// Scale: Years in 2000.
            /// Range: 0 to 99.
            /// </summary>
            public UInt16 CETFP_Year { get; set; }

            /// <summary>
            /// Month in CETFP.
            /// Limit to 2 digits.
            /// 
            /// Scale: Months
            /// Range: 1 to 12.
            /// </summary>
            public UInt16 CETFP_Month { get; set; }

            /// <summary>
            /// Day in CETFP.
            /// Limit to 2 digits.
            /// 
            /// Scale: Days
            /// Range: 1 to 31.
            /// </summary>
            public UInt16 CETFP_Day { get; set; }

            /// <summary>
            /// Hours in CETFP.
            /// Limit to 2 digits.
            /// 
            /// Scale: Hours
            /// Range: 0 to 23.
            /// </summary>
            public UInt16 CETFP_Hour { get; set; }

            /// <summary>
            /// Minutes in CETFP.
            /// Limit to 2 digits.
            /// 
            /// Scale: Minutes
            /// Range: 0 to 59.
            /// </summary>
            public UInt16 CETFP_Minute { get; set; }

            /// <summary>
            /// Seconds in CETFP.
            /// Limit to 2 digits.
            /// 
            /// Scale: Second
            /// Range: 0 to 59.
            /// </summary>
            public UInt16 CETFP_Second { get; set; }

            /// <summary>
            /// Hundredth of seconds in CETFP.
            /// Limit to 2 digits.
            /// 
            /// Scale: Hundredth of Second
            /// Range: 0 to 59.
            /// </summary>
            public UInt16 CETFP_HunSec {get; set;}

            #endregion

            /// <summary>
            /// Ensemble Recording
            /// When recording is enabled the ADCP searches for the next 
            /// available file number to record to. The ensemble file 
            /// name starts with the letter “A” followed by a 7 digit 
            /// number and ending with the extension “.ens”. For example: 
            /// The first ensemble file will be named “A0000001.ens”. 
            /// During deployment as each ensemble is completed the data 
            /// is appended to the current file. The 7 digit number 
            /// following the “A” is incremented each time the 
            /// system is (re)started or when the file size exceeds 
            /// 16777216 bytes.
            /// 
            /// Command: CERECORD n[cr]
            /// Scale: 0=disable, 1=enable.
            /// Range:
            /// </summary>
            public bool CERECORD { get; set; }

            /// <summary>
            /// Return a string of whether
            /// CERECORD is enabled or disabled.
            /// </summary>
            /// <returns>0 = Disabled / 1 = Enabled.</returns>
            public string CERECORD_ToString()
            {
                if (CERECORD)
                {
                    return "1";
                }

                return "0";
            }

            /// <summary>
            /// Ensemble Ping Order
            /// Sets the order in which the various subsystems will be pinged. 
            /// Note: a space and at least one subsystem code must follow the 
            /// CEPO command or the system will reject the command.
            /// 
            /// Command: CEPO cccccccccccccccc[cr]
            /// Scale: Subsystem
            /// Range: (at least 1 subsystem)
            /// </summary>
            public string CEPO { get; set; }

            #endregion

            #region Environmental Properties

            /// <summary>
            /// Water Salinity
            /// Used in the water Speed of Sound calculation.
            /// 
            /// Command: CWS n.nn[cr]
            /// Scale: Parts per Thousand (ppt)
            /// Range: 
            /// </summary>
            public float CWS { get; set; }

            /// <summary>
            /// Water Temperature
            /// Used in the water Speed of Sound calculation if the temperature sensor is not available.
            /// 
            /// Command: CWT n.nn[cr]
            /// Scale: Degrees
            /// Range: 
            /// </summary>
            public float CWT { get; set; }

            /// <summary>
            /// Transducer Depth
            /// Used in the water Speed of Sound calculation.
            /// 
            /// Command: CTD n.nn[cr]
            /// Scale: meters
            /// Range: 
            /// </summary>
            public float CTD { get; set; }

            /// <summary>
            /// Water Speed of Sound 
            /// NOT USED
            /// 
            /// Command CWSS n.nn[cr]
            /// Scale: meters per second (m/s)
            /// Range: 
            /// </summary>
            public float CWSS { get; set; }

            /// <summary>
            /// Heading offset
            /// Added to the compass output 
            /// prior to heading being used within the system.
            /// 
            /// Command: CHO n.nn[cr]
            /// Scale: degrees
            /// Range: -180.0 to +180.0
            /// </summary>
            public float CHO { get; set; }

            /// <summary>
            /// Heading Source
            /// Select the heading source for ENU transformations. 
            ///
            /// Command: CHS N[cr]
            /// Scale: N
            /// Range 1 or 2, 
            /// N=1 Internal (PNI) compass
            /// N=2 GPS HDT string via either serial port
            /// </summary>
            public HeadingSrc CHS { get; set; }

            /// <summary>
            /// Return a string of the
            /// heading source.
            /// </summary>
            /// <returns>1 = Internal / 2 = GPS via serial port.</returns>
            public string CHS_ToString()
            {
                if (CHS == HeadingSrc.SERIAL)
                {
                    return "2";
                }

                return "1";
            }


            #endregion

            #region Serial Comminication Properties

            /// <summary>
            /// RS232 Baud rate
            /// 
            /// Command: C232B N[cr]
            /// Scale: N
            /// Range: 2400, 4800, 9600, 19200, 38400, 115200
            /// </summary>
            public Baudrate C232B { get; set; }

            /// <summary>
            /// RS485 Baud rate
            /// 
            /// Command: C485B N[cr]
            /// Scale: N
            /// Range: 2400, 4800, 9600, 19200, 38400, 115200
            /// </summary>
            public Baudrate C485B { get; set; }

            /// <summary>
            /// RS422 Baud rate
            /// 
            /// Command: C422B N[cr]
            /// Scale: N
            /// Range: 2400, 4800, 9600, 19200, 38400, 115200
            /// </summary>
            public Baudrate C422B { get; set; }

            #endregion

            #endregion // Properties

            /// <summary>
            /// Constructor
            /// 
            /// Initialize the ranges to there default value.
            /// </summary>
            public AdcpCommands(SerialNumber serialNum)
            {                
                // Set the serial number
                _serialNumber = serialNum;

                // Set Default Values
                SetDefaults();
            }

            #region Methods

            /// <summary>
            /// Set the default values.
            /// </summary>
            public void SetDefaults()
            {
                // Set default ranges
                Mode = DEFAULT_MODE;

                // Ensemble defaults
                CEI = DEFAULT_CEI;
                CEI_Hour = CEI.Hour;
                CEI_Minute = CEI.Minute;
                CEI_Second = CEI.Second;
                CEI_HunSec = CEI.HunSec;

                // Time of First ping default
                // Use the current time
                DateTime timeNow = DateTime.Now;
                CETFP_Year = (UInt16)(timeNow.Year);
                CETFP_Month = (UInt16)timeNow.Month;
                CETFP_Day = (UInt16)timeNow.Day;
                CETFP_Hour = (UInt16)timeNow.Hour;
                CETFP_Minute = (UInt16)timeNow.Minute;
                CETFP_Second = (UInt16)timeNow.Second;
                CETFP_HunSec = DEFAULT_CETFP_HUNSEC;

                CERECORD = DEFAULT_CERECORD;

                // Only display the Actual codes and not the additional 0's
                string subsysStr = "";
                for (int x = 0; x < _serialNumber.SubSystems.Length; x++)
                {

                    string serialNumSubSystemsSubstring = _serialNumber.SubSystems.Substring(x, 1);
                    if (serialNumSubSystemsSubstring != "0")
                    {
                        subsysStr += serialNumSubSystemsSubstring;
                    }
                }
                CEPO = subsysStr;

                // Environmental defaults
                CWS = DEFAULT_CWS;
                CWT = DEFAULT_CWT;
                CTD = DEFAULT_CTD;
                CWSS = DEFAULT_CWSS;
                CHO = DEFAULT_CHO;
                CHS = DEFAULT_CHS;

                // Serial Comm defaults
                C232B = DEFAULT_C232B;
                C485B = DEFAULT_C485B;
                C422B = DEFAULT_C422B;
            }

            /// <summary>
            /// Return the command to set
            /// the current time to the device.
            /// STIME.
            /// </summary>
            /// <returns></returns>
            public static string GetTimeCommand()
            {
                return String.Format("{0} {1}", CMD_STIME, DateTime.Now.ToString("yyyy/MM/dd,HH:mm:ss"));
            }

            /// <summary>
            /// Create a list of all the commands and there value.
            /// Add all the commands to the list and there value.
            /// </summary>
            /// <returns>List of all the commands and there value.</returns>
            public List<string> GetCommandList()
            {
                List<string> list = new List<string>();
                list.Add(String.Format("{0}", Mode_ToString()));                        // Mode
                list.Add(string.Format("{0}", GetTimeCommand()));                       // Time
                list.Add(String.Format("{0} {1}", CMD_CEI, CEI_ToString()));            // CEI
                //list.Add(String.Format("{0} {1}", CMD_CETFP, CETFP_ToString()));        // CETFP
                list.Add(String.Format("{0} {1}", CMD_CERECORD, CERECORD_ToString()));  // CERECORD
                list.Add(String.Format("{0} {1}", CMD_CEPO, CEPO));                     // CEPO

                list.Add(String.Format("{0} {1}", CMD_CWS, CWS));                       // CWS
                list.Add(String.Format("{0} {1}", CMD_CWT, CWT));                       // CWT
                list.Add(String.Format("{0} {1}", CMD_CTD, CTD));                       // CTD
                list.Add(String.Format("{0} {1}", CMD_CWSS, CWSS));                     // CWSS
                list.Add(String.Format("{0} {1}", CMD_CHS, CHS_ToString()));            // CHS
                list.Add(String.Format("{0} {1}", CMD_CHO, CHO));                       // CHO

                list.Add(String.Format("{0} {1}", CMD_C232B, ((int)C232B).ToString())); // C232B
                list.Add(String.Format("{0} {1}", CMD_C485B, ((int)C485B).ToString())); // C485B
                list.Add(String.Format("{0} {1}", CMD_C422B, ((int)C422B).ToString())); // C485B

                return list;
            }

            /// <summary>
            /// Create a list of all the deployment commands and there value.
            /// Add all the commands to the list and there value.
            /// </summary>
            /// <returns>List of all the commands and there value.</returns>
            public List<string> GetDeploymentCommandList()
            {
                List<string> list = new List<string>();

                list.Add(String.Format("{0} {1}", CMD_CETFP, CETFP_ToString()));        // CETFP

                return list;
            }


            /// <summary>
            /// This will be a string representation
            /// of the object.  It will also be 
            /// used to pass to the ADCP for all the 
            /// commands.
            /// </summary>
            /// <returns>All the commands as a string.</returns>
            public override string ToString()
            {
                string s = "";

                s += Mode_ToString() + "\n";
                s += GetTimeCommand() + "\n";
                s += CMD_CEI + " " + CEI_ToString() + "\n";

                s += CMD_CWS + " " + CWS + "\n";
                s += CMD_CWT + " " + CWT + "\n";
                s += CMD_CTD + " " + CTD + "\n";
                s += CMD_CWSS + " " + CWSS + "\n";
                s += CMD_CHS + " " + CHS_ToString() + "\n";
                s += CMD_CHO + " " + CHO + "\n";

                s += CMD_C232B + " " + ((int)C232B).ToString() + "\n";
                s += CMD_C485B + " " + ((int)C485B).ToString() + "\n"; 

                // Add the subsystem commands

                return s;
            }

            #endregion

            #region Decode

            #region ENGPNI

            /// <summary>
            /// Decode the results to ENGPNI.
            /// This will give the heading, pitch and roll
            /// read from the ADCP.
            /// 
            /// Ex:
            /// H=  0.00, P=  0.00, R=  0.00
            /// </summary>
            /// <param name="result">Result string from the ENGPNI command.</param>
            public static HPR DecodeEngPniResult(string result)
            {
                // Look for the ENGPNI result
                double heading = 0.0;
                double pitch = 0.0;
                double roll = 0.0;

                // Look for the line with the results
                char[] delimiters = new char[] { '\r', '\n' };
                string[] lines = result.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                for (int x = 0; x < lines.Length; x++)
                {
                    // Check if is the line to decode
                    if (lines[x].Contains("H="))
                    {
                        // Decode the line
                        delimiters = new char[] { ',' };
                        string[] elem = lines[x].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                        for (int y = 0; y < elem.Length; y++)
                        {
                            // Trim the white space
                            // Then go just past the = to get the value
                            // The first character determines which value is given
                            string value = elem[y].Trim();
                            char firstChar = value[0];
                            int elemStart = value.IndexOf('=') + 1;
                            string subElem = value.Substring(elemStart, value.Length - elemStart);

                            // Parse each value
                            switch (firstChar)
                            {
                                case 'H':
                                    double.TryParse(subElem, out heading);
                                    break;
                                case 'P':
                                    double.TryParse(subElem, out pitch);
                                    break;
                                case 'R':
                                    double.TryParse(subElem, out roll);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                return new HPR() { Heading = heading, Pitch = pitch, Roll = roll };
            }

            #endregion

            #region BREAK

            /// <summary>
            /// Decode the break statement of all its data.
            /// 
            /// Ex:
            /// Copyright (c) 2009-2012 Rowe Technologies Inc. All rights reserved.
            /// DP300 DP1200 
            /// SN: 01460000000000000000000000000000
            /// FW: 00.02.05 Apr 17 2012 05:40:11
            /// </summary>
            /// <param name="buffer">Buffer containing the break statement.</param>
            /// <returns>Return the values decoded from the buffer given.</returns>
            public static BreakStmt DecodeBREAK(string buffer)
            {
                string serial = "";
                string fw = "";
                string hw = "";
                Firmware firmware = new Firmware();

                // Break up the lines
                char[] delimiters = new char[] { '\r', '\n' };
                string[] lines = buffer.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                // Decode each line of data
                for (int x = 0; x < lines.Length; x++)
                {
                    // Change delimiter to a space
                    delimiters = new char[] { ' ' };
                    string[] elem = lines[x].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                    // Firmware
                    if (lines[x].Contains("FW:"))
                    {
                        if (elem.Length >= 2)
                        {
                            fw = elem[1].Trim();

                            // Decode the firmware version
                            char[] fwDelimiters = new char[] { '.' };
                            string[] fwElem = fw.Split(fwDelimiters, StringSplitOptions.RemoveEmptyEntries);
                            if (fwElem.Length == 3)
                            {
                                ushort major = 0;
                                ushort minor = 0;
                                ushort revision = 0;
                                if (ushort.TryParse(fwElem[0], out major))
                                {
                                    firmware.FirmwareMajor = major;
                                }
                                if (ushort.TryParse(fwElem[1], out minor))
                                {
                                    firmware.FirmwareMinor = minor;
                                }
                                if (ushort.TryParse(fwElem[2], out revision))
                                {
                                    firmware.FirmwareRevision = revision;
                                }
                            }

                        }
                    }

                    // Serial number
                    if (lines[x].Contains("SN:"))
                    {
                        if (elem.Length >= 2)
                        {
                            serial = elem[1];
                        }
                    }

                    // Hardware
                    if (lines.Length > 2)
                    {
                        hw = lines[1];
                    }

                }

                // Return the break statement values
                return new BreakStmt(){ SerialNum = new SerialNumber(serial), FirmwareVersion = firmware, Hardware = hw.Trim() };
            }

            #endregion

            #region ENGI2CSHOW

            /// <summary>
            /// Decode the command ENGI2CSHOW to get
            /// the serial numbers for each board in the board stack.
            /// </summary>
            /// <param name="buffer"></param>
            public static I2cMemDevs DecodeENGI2CSHOW(string buffer)
            {
                I2cMemDevs devs = new I2cMemDevs();

                // Look for the line with the results
                char[] delimiters = new char[] { '\r', '\n' };
                string[] lines = buffer.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                for (int x = 0; x < lines.Length; x++)
                {
                    // Skip lines with --
                    // They do not contain any data
                    if (!lines[x].Contains("--"))
                    {
                        // RCVR registers
                        if (lines[x].Contains("RVCR"))
                        {
                            // Decode line and add to the list of Receiver registers
                            devs.RvcrRegs.Add(DecodeRegisterLine(lines[x]));
                        }

                        // Boards
                        if (lines[x].Contains("24AA32AF"))
                        {
                            // Decode the line
                            List<I2cBoard> boards = DecodeBoardLine(lines[x]);
                            foreach (I2cBoard board in boards)
                            {
                                // Set the board based off the ID
                                if (board.ID == I2cBoardId.BACKPLANE) { devs.BackPlaneBoard = board; }
                                if (board.ID == I2cBoardId.IO) { devs.IoBoard = board; }
                                if (board.ID == I2cBoardId.LOW_PWR_REG) { devs.LowPwrRegBoard = board; }
                                if (board.ID == I2cBoardId.RCVR) { devs.RcvrBoard = board; }
                                if (board.ID == I2cBoardId.VIRTUAL_GND) { devs.VirtualGndBoard = board; }
                                if (board.ID == I2cBoardId.XMITTER) { devs.XmitterBoard = board; }
                            }
                        }

                        // RTC registers
                        if (lines[x].Contains("DS3231"))
                        {
                            // Decode line and add to the list of RTC registers
                            devs.RtcRegs.Add(DecodeRegisterLine(lines[x]));
                        }
                    }
                }

                return devs;
            }
            /// <summary>
            /// Decode the line of all the register information.  The number of registers will vary 
            /// between hardware types.  Add all the register values and ID to the struct.
            /// 
            /// Ex: 
            /// 
            /// RVCR,      01, 01 07 00 00 04 0F 40 A0 7F 00 00 20 20 00 40 00 60 00 50 03 03 00 10 00 00 27 27
            /// DS3231,    68, 28 46 11 02 10 07 12 00 00 00 00 00 00 00 00 00 00 30 00
            /// </summary>
            /// <param name="line">Line containing the register information.</param>
            /// <returns>Register information.</returns>
            private static I2cRegister DecodeRegisterLine(string line)
            {
                I2cRegister result = new I2cRegister();

                // Split by comma
                char[] delimiters = new char[] { ',' };
                string[] values = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                // Ensure we have enough data
                if (values.Length >= 3)
                {
                    // Set the ID
                    try
                    {
                        int id = Convert.ToInt32(values[1].Trim(), 16);
                        result.ID = id;
                    }
                    catch(Exception)
                    {
                        result.ID = 0;
                    }

                    // Split by space for registers
                    delimiters = new char[] { ' ' };
                    string[] regs = values[2].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                    result.RegValues = new List<string>(regs);
                }

                return result;
            }

            /// <summary>
            /// Decode the line of all the board information.  It will contain the
            /// board ID, revision and serial number.  Some lines may contain more then
            /// one board's information on the same line.
            /// 
            /// Ex: 
            /// 24AA32AF,     50, 50007  REV:XD1  SER#010
            /// 24AA32AF,     53, 50018  REV:XD1  SER#010   50022  REV:XA  SER#0XX
            /// </summary>
            /// <param name="line">Line containing the board information.</param>
            /// <returns>Board information.</returns>
            private static List<I2cBoard> DecodeBoardLine(string line)
            {
                List<I2cBoard> boards = new List<I2cBoard>();

                // Split by comma
                char[] delimiters = new char[] { ',' };
                string[] values = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                // Ensure we have enough data
                if (values.Length >= 3)
                {
                    delimiters = new char[] { ' ' };
                    string[] brds = values[2].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                    // Verify there is enough to decode a board
                    if (brds.Length >= 3)
                    {
                        // Decode the board
                        I2cBoard i2cBrd = DecodeBoardValues(brds[0].Trim(), brds[1].Trim(), brds[2].Trim());
                        if (i2cBrd != null)
                        {
                            boards.Add(i2cBrd);
                        }

                        // If more than 1 board was on a line
                        // Decode the second board
                        if (brds.Length >= 6)
                        {
                            i2cBrd = DecodeBoardValues(brds[3], brds[4], brds[5]);
                            if (i2cBrd != null)
                            {
                                boards.Add(i2cBrd);
                            }
                        }
                    }

                }



                return boards;
            }

            /// <summary>
            /// Try to decode the board values.
            /// 
            /// If there is an error parsing the board ID,
            /// the ID will be left as Unknown.
            /// 
            /// Ex:
            /// id = 50007  
            /// rev = REV:XD1  
            /// serial = SER#010
            /// </summary>
            /// <param name="id">ID string.</param>
            /// <param name="rev">Revision string.</param>
            /// <param name="serial">Serial string.</param>
            /// <returns>Information about the board.</returns>
            private static I2cBoard DecodeBoardValues(string id, string rev, string serial)
            {
                I2cBoard board = new I2cBoard();

                // Try to parse the Board ID
                // If there is an error return NULL
                int boardId = 0;
                if (int.TryParse(id, out boardId))
                {
                    board.ID = (I2cBoardId)boardId;
                }

                // Parse the Revision
                // Split by semicolon (:)
                char[] delimiters = new char[] { ':' };
                string[] revision = rev.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                if (revision.Length >= 2)
                {
                    board.Revision = revision[1];
                }

                // Parse the Serial Number
                // Split by pound sign (#)
                delimiters = new char[] { '#' };
                string[] serialNum = serial.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                if (serialNum.Length >= 2)
                {
                    board.SetSerial(serialNum[1]);
                }

                return board;
            }



            #endregion

            #region STIME

            /// <summary>
            /// Decode the Date Time line.  This will look for the date and time line.
            /// It will then parse the line using DateTime.Parse().
            /// 
            /// Ex:
            /// 
            /// </summary>
            /// <param name="buffer">Buffer from the serial port.</param>
            /// <returns>DateTime parsed from the buffer.</returns>
            public static DateTime DecodeSTIME(string buffer)
            {
                // Look for the line with the results
                char[] delimiters = new char[] { '\r', '\n' };
                string[] lines = buffer.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                // Create a blank date and time
                DateTime dt = new DateTime();

                // Look for the line with data
                foreach (string line in lines)
                { 
                    // Check for the line that does not contain STIME
                    // This is the date time line
                    if(!line.Contains(RTI.Commands.AdcpCommands.CMD_STIME))
                    {
                        // Create a new date and time by parsing the date time line
                        dt = DateTime.Parse(line);
                    }
                }

                return dt;
            }

            #endregion

            #endregion

        }
    }
}