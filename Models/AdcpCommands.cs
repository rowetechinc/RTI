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
 * 07/26/2012      RC          2.13       Created AdcpDirListing and AdcpEnsFileInfo to capture the file information on the ADCP.
 *                                         Added DecodeDSDIR().
 * 07/27/2012      RC          2.13       Added ToSeconds() to TimeValue object.
 * 07/31/2012      RC          2.13       Added CommandPayload object to pass commands as an object in events. 
 *                                         Changed DEFAULT_CWSS (Speed of Sound) to 1490 m/s.
 * 08/21/2012      RC          2.13       Changed parsing the DateTime in DecodeSTIME() to TryParse.  If bad value, then it will use the default time.
 * 08/30/2012      RC          2.15       Fixed bug with DEFAULT_CEI.  Reference could change the value.  Removed DEFAULT_CEI and added DEFAULT_CEI_HOUR/MINUTE/SECOND/HUNSEC.
 * 08/31/2012      RC          2.15       Added CMD_DSXD for high speed xmodem download.
 * 09/06/2012      RC          2.15       Update the commands to the ADCP manual Revision G.
 * 09/07/2012      RC          2.15       Removed EnableDisable enum and changed DEFAULT_ENGMACON to a bool.
 *                                         Added the command CETFP to GetCommandList().  This sets the time of first ping.
 * 09/10/2012      RC          2.15       Added DEFAULT_SALINITY_VALUE_SALT and DEFAULT_SALINITY_VALUE_FRESH.
 * 09/11/2012      RC          2.15       Changed the DecodeBoardValues() to handle board IDs with and without a frequency.
 * 09/24/2012      RC          2.15       Added DecodeCSHOW() to decode the CSHOW command.
 * 09/25/2012      RC          2.15       Added CEOUTPUT command.
 * 09/26/2012      RC          2.15       Fixed TimeValue to rollover values when exceed there minutes, seconds and HunSec range.
 * 09/28/2012      RC          2.15       Added the command CVSF.  Check ranges for values when setting the value if ranges exist.
 * 10/01/2012      RC          2.15       Removed requiring the serial number for the constructor.  It was only needed for CEPO, but that was incorrectly created with the serial number.
 * 10/10/2012      RC          2.15       When creating the command list, ensure the string is set to United States English format.  This is to prevent commas from being used for decimal points.
 * 10/11/2012      RC          2.15       Added Minimum values for CWS, CTD and CWSS command.
 * 11/21/2012      RC          2.16       Changed CEOUTPUT from a ushort to AdcpCommands.AdcpOutputMode.
 *                                         Added command strings for each command.
 * 12/20/2012      RC          2.17       Updated comments to ADCP User Guide Rev H.
 * 12/27/2012      RC          2.17       Replaced Subsystem.Empty with Subsystem.IsEmpty().
 * 01/02/2013      RC          2.17       Added list functions for all the commands that have a list of options. 
 * 01/22/2013      RC          2.17       Made CEPO command the first command in the list because CEPO will change all values to defaults.
 * 05/17/2013      RC          2.19       Added GetUtcSystemTimeCommand() to set the time to UTC time and changed GetSystemTimeCommand() to GetLocalSystemTimeCommand().
 *                                          Changed the name of Time_CmdStr() to LocalTime_CmdStr().  Added UtcTime_CmdStr().
 * 05/25/2013      RC          2.19       Check for empty buffer in DecodeBREAK().
 * 05/30/2013      RC          2.19       Added TimeValue constructor that takes seconds.  Added TimeValue.ToStringD() to give the time i seconds as a double.
 * 06/11/2013      RC          2.19       Added SPOS command.
 * 06/12/2013      RC          2.19       Added DecodeENGCONF().
 * 08/23/2013      RC          2.19.4     Added DEFAULT_SALINITY_VALUE_ESTUARY.
 * 09/17/2013      RC          2.20.0     Removed giving the STIME in GetDeploymentCommandList().
 *                                         Added CERECORD command to GetDeploymentCommandList().
 *                                         Updated CERECORD to include SinglePing parameter.
 * 09/18/2013      RC          2.20.1     Added DEFAULT_SAN_DIEGO_DECLINATION for CHO command.
 *                                         Added CHO command to GetDeploymentCommandList().
 * 09/23/2013      RC          2.20.1     Added DecodeSPOS().
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.ComponentModel;
using DotSpatial.Positioning;
using Newtonsoft.Json;


namespace RTI
{
    namespace Commands
    {
        #region Classes and Enums

        #region Baudrate

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

        #endregion

        #region HeadingSrc

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

        #endregion

        #region TimeValue

        /// <summary>
        /// TimeValue used to send command CWPAI.
        /// When setting the values individually, order matters.
        /// Set the highest values first and must up to lower values.
        /// </summary>
        public class TimeValue
        {
            private UInt16 _hour;
            /// <summary>
            /// Hours.
            /// </summary>
            public UInt16 Hour 
            {
                get { return _hour; }
                set
                {
                    _hour = value;
                }
            }

            /// <summary>
            /// Minutes.
            /// </summary>
            private UInt16 _minute;
            /// <summary>
            /// Minutes.
            /// </summary>
            public UInt16 Minute 
            {
                get { return _minute; } 
                set
                {
                    // Verify the value does not exceed 60 minutes
                    if (value >= 60)
                    {
                        Hour += (UInt16)(value / 60);
                        _minute = (UInt16)(value - ((value / 60) * 60));
                    }
                    else
                    {
                        _minute = value;
                    }
                }
            }

            /// <summary>
            /// Seconds.
            /// </summary>
            private UInt16 _second;
            /// <summary>
            /// Seconds.
            /// </summary>
            public UInt16 Second
            {
                get { return _second; }
                set
                {
                    // Verify the value does not exceed 60 seconds
                    if (value >= 60)
                    {
                        Minute += (UInt16)(value / 60);
                        _second = (UInt16)(value - ((value / 60) * 60));
                    }
                    else
                    {
                        _second = value;
                    }
                }
            }

            /// <summary>
            /// Hundredths of a second.
            /// </summary>
            private UInt16 _hunSec;
            /// <summary>
            /// Hundredths of a second.
            /// </summary>
            public UInt16 HunSec
            {
                get { return _hunSec; }
                set
                {
                    // Verify the value does not exceed 100 hun second.
                    if (value >= 100)
                    {
                        Second += (UInt16)(value / 100);
                        _hunSec = (UInt16)(value - ((value / 100) * 100));
                    }
                    else
                    {
                        _hunSec = value;
                    }
                }
            }

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
            /// Set the time value based off the seconds given.
            /// This will convert the seconds to Hours, Minutes, Seconds 
            /// and HunSec.
            /// </summary>
            /// <param name="seconds">Time as seconds.</param>
            public TimeValue(float seconds)
            {
                TimeSpan ts = TimeSpan.FromSeconds(seconds);
                Hour = Convert.ToUInt16(ts.Hours);
                Minute = Convert.ToUInt16(ts.Minutes);
                Second = Convert.ToUInt16(ts.Seconds);
                HunSec = (ushort)Math.Round(ts.Milliseconds * 0.1);
            }

            /// <summary>
            /// Return the string version of this class.
            /// </summary>
            /// <returns>String version of this class.  HH:MM:SS.hh</returns>
            public override string ToString()
            {
                return Hour.ToString("00", CultureInfo.CreateSpecificCulture("en-US")) + ":" + Minute.ToString("00", CultureInfo.CreateSpecificCulture("en-US")) + ":" + Second.ToString("00", CultureInfo.CreateSpecificCulture("en-US")) + "." + HunSec.ToString("00", CultureInfo.CreateSpecificCulture("en-US"));
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

            /// <summary>
            /// Return the number of seconds in the time.
            /// </summary>
            /// <returns>Number of seconds in the time.</returns>
            public int ToSeconds()
            {
                return (int)Math.Round((3600 * Hour) + (60 * Minute) + Second + (HunSec / 100.0));
            }

            /// <summary>
            /// Return the number of seconds in the time as a double.
            /// </summary>
            /// <returns>Number of seconds in the time as a double.</returns>
            public double ToSecondsD()
            {
                return (3600 * Hour) + (60 * Minute) + Second + (HunSec / 100.0);
            }
        }

        #endregion

        #region Decode Structs and Classes

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

        #region DSDIR

        #region AdcpEnsFileInfo

        /// <summary>
        /// Description of an ensemble file
        /// store on the ADCP.  This will include
        /// the file name, date of modification and
        /// file size.
        /// 
        /// A0000001.ENS 2012/04/02 16:53:11      1.004
        /// </summary>
        public class AdcpEnsFileInfo
        {
            #region Properties

            /// <summary>
            /// Name of the file.
            /// </summary>
            public string FileName { get; set; }

            /// <summary>
            /// Date and time the file was last modified.
            /// </summary>
            public DateTime ModificationDateTime { get; set; }

            /// <summary>
            /// Size of the file in megabytes.
            /// </summary>
            public double FileSize { get; set; }

            #endregion

            /// <summary>
            /// Create an empty file info.
            /// </summary>
            public AdcpEnsFileInfo()
            {
                // Default values
                FileName = string.Empty;
                ModificationDateTime = DateTime.Now;
                FileSize = 0.0;
            }

            /// <summary>
            /// Take a single string and parse the
            /// file info data.
            /// 
            /// Example:
            /// A0000001.ENS 2012/04/02 16:53:11      1.004
            /// </summary>
            /// <param name="fileListing">String containing the file info.</param>
            public AdcpEnsFileInfo(string fileListing)
            {
                // Parse the string of all it elements
                char[] delimiters = { ' ' };
                string[] fileInfo = fileListing.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                // Ensure info was found
                if (fileInfo.Length >= 4)
                {
                    FileName = fileInfo[0];
                    SetDateTime(fileInfo[1] + " " + fileInfo[2]);
                    SetFileSize(fileInfo[3]);
                }
                else
                {
                    // Default values
                    FileName = string.Empty;
                    ModificationDateTime = DateTime.Now;
                    FileSize = 0.0;
                }

            }

            /// <summary>
            /// Parse the Date and time.  This will
            /// take the date and time string and
            /// convert it to a DateTime object.  If it
            /// cannot be converted, it will use the 
            /// current time.
            /// 
            /// 2012/04/02 16:53:11
            /// </summary>
            /// <param name="dateTime">String containing the date and time.</param>
            private void SetDateTime(string dateTime)
            {
                // Try to parse the date and time
                // If the date and time cannot be parsed
                // use the current date and time
                DateTime dateTimeValue;
                if (DateTime.TryParseExact(dateTime, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeValue))
                {
                    ModificationDateTime = dateTimeValue;
                }
                else
                {
                    ModificationDateTime = DateTime.Now;
                }

            }

            /// <summary>
            /// Try to parse the string into the file size.
            /// If the parse is not successful, set the size
            /// to 0.0.
            /// </summary>
            /// <param name="fileSize">File Size as a string in Megabytes.</param>
            private void SetFileSize(string fileSize)
            {
                double size = 0.0;
                double.TryParse(fileSize, out size);

                // Set the size after parsing the string.
                FileSize = size;
            }
        }

        #endregion

        #region AdcpDirListing

        /// <summary>
        /// Get the listing of all the files on the ADCP
        /// and free and used space.
        /// </summary>
        public class AdcpDirListing
        {
            /// <summary>
            /// Free memory space on the ADCP.
            /// </summary>
            public float TotalSpace { get; set; }

            /// <summary>
            /// Used memory space on the ADCP.
            /// </summary>
            public float UsedSpace { get; set; }

            /// <summary>
            /// List of all the ensemble files
            /// on the system.
            /// </summary>
            public List<AdcpEnsFileInfo> DirListing { get; set; }

            /// <summary>
            /// Initialize the list and sizes.
            /// </summary>
            public AdcpDirListing()
            {
                DirListing = new List<AdcpEnsFileInfo>();
                TotalSpace = 0;
                UsedSpace = 0;
            }
        }

        #endregion

        #endregion

        #region ENGCONF

        /// <summary>
        /// Class to hold all the devices
        /// that are enabled in the ADCP.
        /// 
        /// Fram     0
        /// RTC      0
        /// KELLER30 0
        /// cd       0
        /// EMAC     0
        /// A11 RCVR 1
        /// SLEEP    0
        /// 
        /// </summary>
        public class EngConf
        {

            #region Properties

            /// <summary>
            /// FRAM.
            /// </summary>
            public bool IsFram { get; set; }

            /// <summary>
            /// Real Time Clock.
            /// </summary>
            public bool IsRtc { get; set; }

            /// <summary>
            /// Keller 30 Pressure Sensor.
            /// </summary>
            public bool IsPressureSensor { get; set; }

            /// <summary>
            /// cd
            /// </summary>
            public bool IsCd { get; set; }

            /// <summary>
            /// Ethernet.
            /// </summary>
            public bool IsEmac { get; set; }

            /// <summary>
            /// A11 Receiver
            /// Old or new receiver.
            /// </summary>
            public bool IsA11Rcvr { get; set; }

            /// <summary>
            /// Sleep.
            /// </summary>
            public bool IsSleep { get; set; }

            #endregion

            /// <summary>
            /// Initialize the values.
            /// </summary>
            public EngConf()
            {
                // Initialize the values
                IsFram = false;
                IsRtc = false;
                IsPressureSensor = false;
                IsCd = false;
                IsEmac = false;
                IsA11Rcvr = false;
                IsSleep = false;
            }
        }

        #endregion

        #endregion

        #region Adcp Command Payload

        /// <summary>
        /// This object is used to send a command around.  This will store 
        /// the command and its parameter.  This can be used for events where
        /// a subscriber is interested in knowning if a command was sent.
        /// 
        /// When receiving the command, the user will have to determine the 
        /// type of the parameter.  This can be done using the following example.
        /// 
        /// if(Parameter is boolean) ....
        /// </summary>
        public class CommandPayload
        {
            #region Properties

            /// <summary>
            /// String of the command.  This is used to determine
            /// which command this payload is.
            /// </summary>
            public string Command { get; set; }

            /// <summary>
            /// The value associated with this command.  If this
            /// command takes a parameter, this value will be set
            /// with its value.  If the command does not use a parameter,
            /// the parameter will be null.
            /// </summary>
            public object Parameter { get; set; }

            /// <summary>
            /// Subsystem for the command.  Each subsystem can send
            /// the same command.  This will differenciate which subsystem
            /// sent the command.
            /// </summary>
            public Subsystem SubSystem { get; set; }

            #endregion

            /// <summary>
            /// Initialize the object with empty values.
            /// </summary>
            public CommandPayload()
            {
                Command = "";
                Parameter = null;
                SubSystem = new Subsystem();
            }

            /// <summary>
            /// Initialize the object with the given values.
            /// </summary>
            /// <param name="ss">Subsystem the command came from.</param>
            /// <param name="cmd">Command to set this object to.</param>
            /// <param name="param">Parameter for the object.</param>
            public CommandPayload(Subsystem ss, string cmd, object param)
            {
                SubSystem = ss;
                Command = cmd;
                Parameter = param;
            }

            /// <summary>
            /// Initialize the object with a command that does not take
            /// a parameter.  Parameter will be set to null.
            /// </summary>
            /// <param name="ss">Subsystem the command came from.</param>
            /// <param name="cmd">Command to set to the payload.</param>
            public CommandPayload(Subsystem ss, string cmd)
            {
                SubSystem = ss;
                Command = cmd;
                Parameter = null;
            }
        }

        #endregion

        #region Clock

        /// <summary>
        /// The time zone options for the user to use
        /// for the ADCP time.
        /// </summary>
        public enum AdcpTimeZone
        {
            /// <summary>
            /// Local time zone of the user.
            /// </summary>
            LOCAL,

            /// <summary>
            /// Greenwich Mean Time zone.
            /// </summary>
            GMT
        }

        #endregion

        #region SPOS

        /// <summary>
        /// Class to hold all the SPOS values.
        /// </summary>
        public class SPOS
        {
            #region Properties

            /// <summary>
            /// Latitude position of the ADCP.
            /// </summary>
            public Latitude SPOS_Latitude { get; set; }

            /// <summary>
            /// Longitude position of the ADCP.
            /// </summary>
            public Longitude SPOS_Longitude { get; set; }

            /// <summary>
            /// Water depth in meters.
            /// </summary>
            public float SPOS_WaterDepth { get; set; }

            /// <summary>
            /// Pressure Sensor height in meters.
            /// </summary>
            public float SPOS_PsensHeight { get; set; }

            #endregion

            /// <summary>
            /// Initialize the values.
            /// </summary>
            public SPOS()
            {
                SPOS_Latitude = new Latitude();
                SPOS_Longitude = new Longitude();
                SPOS_WaterDepth = AdcpCommands.DEFAULT_WATER_DEPTH;
                SPOS_PsensHeight = AdcpCommands.DEFAULT_PSEN_HEIGHT;
            }
        }


        #endregion

        #endregion

        /// <summary>
        /// Commands handled by the ADCP.
        /// </summary>
        public class AdcpCommands
        {
            #region Varialbles

            #region Salinity

            /// <summary>
            /// Fresh water salinity value for CWS.
            /// </summary>
            public const float DEFAULT_SALINITY_VALUE_FRESH = 0.0f;

            /// <summary>
            /// Salt water salinity value for CWS.
            /// </summary>
            public const float DEFAULT_SALINITY_VALUE_SALT = 35.0f;

            /// <summary>
            /// Estuary water salinity value for CWS.
            /// </summary>
            public const float DEFAULT_SALINITY_VALUE_ESTUARY = 15.0f;

            #endregion

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
            /// Ensemble output type.
            /// 
            /// n = 0
            /// Disable serial output.  Saves battery energy when recording
            /// data to the SD card during a self-contained deployment by reducing
            /// extra on time on the system due to data transfer.
            /// 
            /// n= 1
            /// Enables the standard binary output data protocol to be sent out
            /// the serial port.
            /// 
            /// n=2
            /// Enables an ASCII text serial output that is dumb terminal compatible.
            /// </summary>
            public const string CMD_CEOUTPUT = "CEOUTPUT";

            /// <summary>
            /// Ensemble Command: Ensemble Ping Order
            /// Parameter: cccccccccccccccc
            /// Sets the order in which the various subsystems will be pinged. 
            /// Note: a space and at least one subsystem code must follow 
            /// the CEPO command or the system will reject the command.
            /// </summary>
            public const string CMD_CEPO = "CEPO";

            /// <summary>
            /// Ensemble Command: ADCP Position
            /// Parameter: aa.aaaaaaaaa, bb.bbbbbbbbb, cc.cc, dd.dd 
            /// Set the ADCP position.
            /// aa.aaaaaaaaa = Latitude in decimal degrees.
            /// bb.bbbbbbbbb = Longitude in decimal degrees.
            /// cc.cc = Water Depth in meters.
            /// dd.dd = Pressure Sensor Height in meters.
            /// </summary>
            public const string CMD_SPOS = "SPOS";

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

            /// <summary>
            /// Zero Pressure Sensor.  Sets the current pressure reading to the zero point
            /// (if pressure sensor is installed).
            /// </summary>
            public const string CMD_CPZ = "CPZ";

            /// <summary>
            /// ?
            /// </summary>
            public const string CMD_CVSF = "CVSF";

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
            /// Data Storage Command: Transmit a File (High Speed Mode).
            /// Parameter: filename.abc
            /// High Speed Download.
            /// Data Storage XMODEM Transmit.  This command is used 
            /// to transfer a file, via the serial communication 
            /// link, from an external device to the SD card contained 
            /// within the RTI system.  File names are limited to a 
            /// maximum of 8 characters before the extension
            /// </summary>
            public const string CMD_DSXD = "DSXD";

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
            /// Engineering Command: Get Hardware configuration of the ADCP.
            /// Parameter:
            /// Hardware configuration of the ADCP.
            /// </summary>
            public const string CMD_ENGCONF = "ENGCONF";

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
            public const Baudrate DEFAULT_C422B = Baudrate.BAUD_115200;

            /// <summary>
            /// Default ADCP mode.
            /// </summary>
            public const AdcpMode DEFAULT_MODE = AdcpMode.PROFILE;

            /// <summary>
            /// Default MAC enable.
            /// </summary>
            public const bool DEFAULT_ENGMACON = false;

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
            public const float DEFAULT_CWSS = 1490.0f;

            /// <summary>
            /// Default Heading Offset.
            /// </summary>
            public const float DEFAULT_CHO = 0.0f;

            /// <summary>
            /// Declination for San Diego in degrees.  Used for CHO command 
            /// in San Diego.
            /// </summary>
            public const float DEFAULT_SAN_DIEGO_DECLINATION = 12.05f;

            /// <summary>
            /// Default Heading source.
            /// </summary>
            public const HeadingSrc DEFAULT_CHS = HeadingSrc.INTERNAL;

            /// <summary>
            /// Default Ensemble Interval Hour.
            /// </summary>
            public const ushort DEFAULT_CEI_HOUR = 0;

            /// <summary>
            /// Default Ensemble Interval Minute.
            /// </summary>
            public const ushort DEFAULT_CEI_MINUTE = 0;

            /// <summary>
            /// Default Ensemble Interval Second.
            /// </summary>
            public const ushort DEFAULT_CEI_SECOND = 1;

            /// <summary>
            /// Default Ensemble Interval Hundredth of second.
            /// </summary>
            public const ushort DEFAULT_CEI_HUNSEC = 0;

            /// <summary>
            /// Default Year to start pinging.
            /// 2012
            /// </summary>
            public const UInt16 DEFAULT_CETFP_YEAR = 2013;

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
            /// Default whether to record on the system.
            /// </summary>
            public const bool DEFAULT_CERECORD = false;

            /// <summary>
            /// Default whether to record single ping on the system.
            /// </summary>
            public const bool DEFAULT_CERECORD_SINGLEPING = false;

            /// <summary>
            /// Default whether to output data to serial port.
            /// </summary>
            public const AdcpOutputMode DEFAULT_CEOUTPUT = AdcpCommands.AdcpOutputMode.Binary;

            /// <summary>
            /// Default CEPO is blank.  When the serial number
            /// is known and if CEPO is blank, CEPO should at
            /// least be set with a configuration for each
            /// subsystem that exist.
            /// </summary>
            public const string DEFAULT_CEPO = "";

            /// <summary>
            /// Default CVSF value.
            /// </summary>
            public const float DEFAULT_CVSF = 1.000f;

            /// <summary>
            /// Default latitude value.
            /// </summary>
            [JsonIgnore]
            public Latitude DEFAULT_LAT = new Latitude();

            /// <summary>
            /// Default longitude value.
            /// </summary>
            [JsonIgnore]
            public Longitude DEFAULT_LONG = new Longitude();

            /// <summary>
            /// Default water depth in meters.
            /// </summary>
            public const float DEFAULT_WATER_DEPTH = 0.0f;

            /// <summary>
            /// Default pressure sensor height in meters.
            /// </summary>
            public const float DEFAULT_PSEN_HEIGHT = 0.0f;

            #endregion

            #region Min/Max Values
            
            /// <summary>
            /// Minimum heading offset.
            /// </summary>
            public const float MIN_CHO = -180.0f;

            /// <summary>
            /// Maximum heading offset.
            /// </summary>
            public const float MAX_CHO = 180.0f;

            /// <summary>
            /// Minimum Depth of Transducer.
            /// </summary>
            public const float MIN_CTD = 0.0f;

            /// <summary>
            /// Minimum Salinity.
            /// </summary>
            public const float MIN_CWS = 0.0f;

            /// <summary>
            /// Minimum Speed of Sound.
            /// </summary>
            public const float MIN_CWSS = 0.0f;
            
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

            /// <summary>
            /// Minimum value for CEOUTPUT.
            /// </summary>
            public const UInt16 MIN_CEOUTPUT = 0;

            /// <summary>
            /// Maximum value for CEOUTPUT.
            /// </summary>
            public const UInt16 MAX_CEOUTPUT = 2;

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

            /// <summary>
            /// Output mode to the serial port. 
            /// </summary>
            public enum AdcpOutputMode
            {
                /// <summary>
                /// Disable output mode to the serial port.
                /// </summary>
                Disable = 0,

                /// <summary>
                /// Binary output to the serial port.
                /// </summary>
                Binary = 1,

                /// <summary>
                /// ASCII output to the serial port.
                /// </summary>
                ASCII = 2
            }

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
                return CEI_Hour.ToString("00", CultureInfo.CreateSpecificCulture("en-US")) + ":" + CEI_Minute.ToString("00", CultureInfo.CreateSpecificCulture("en-US")) + ":" + CEI_Second.ToString("00", CultureInfo.CreateSpecificCulture("en-US")) + "." + CEI_HunSec.ToString("00", CultureInfo.CreateSpecificCulture("en-US"));
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
            /// Ensemble Time of First Ping
            /// Sets the time that the system will awaken and start
            /// pinging.
            /// 
            /// All digits including the space following CETFP and
            /// the seperator must be part of the command or the system 
            /// will reject the command.
            /// 
            /// This is the time and date of the first ping.
            /// If there is a deployment, this would be the time
            /// the pinging would start.  
            /// 
            /// Command: CETFP
            /// Scale: YYYY/MM/DD,HH:mm:SS.hh
            /// Range: Date and Time
            /// </summary>
            public DateTime CETFP { get; set; }

            /// <summary>
            /// Return a string representation of CETFP.
            /// </summary>
            /// <returns>String representation of CETFP</returns>
            public string CETFP_ToString()
            {
                return CETFP.Year.ToString("0000", CultureInfo.CreateSpecificCulture("en-US")) + "/" + CETFP.Month.ToString("00", CultureInfo.CreateSpecificCulture("en-US")) + "/" + CETFP.Day.ToString("00", CultureInfo.CreateSpecificCulture("en-US")) + "," + CETFP.Hour.ToString("00", CultureInfo.CreateSpecificCulture("en-US")) + ":" + CETFP.Minute.ToString("00", CultureInfo.CreateSpecificCulture("en-US")) + ":" + CETFP.Second.ToString("00", CultureInfo.CreateSpecificCulture("en-US")) + "." + CETFP_HunSec.ToString("00", CultureInfo.CreateSpecificCulture("en-US"));
            }

            /// <summary>
            /// Hundredth of seconds in CETFP.
            /// Limit to 2 digits.
            /// 
            /// Scale: Hundredth of Second
            /// Range: 0 to 99.
            /// </summary>
            public UInt16 CETFP_HunSec
            {
                get { return (UInt16)Math.Round(CETFP.Millisecond / MathHelper.HUNSEC_TO_MILLISEC); }
            }

            #endregion

            #region CERECORD

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
            /// 16777216 bytes (16 Mbytes).
            /// 
            /// Note: Internal data recording during burst sampling only
            /// occurs at the end of the burst.
            /// 
            /// Command: CERECORD n,x[cr]
            /// Scale: 0=disable, 1=enable.
            /// Range:
            /// </summary>
            public bool CERECORD_EnsemblePing { get; set; }

            /// <summary>
            /// Return a string of whether
            /// CERECORD is enabled or disabled.
            /// </summary>
            /// <returns>0 = Disabled / 1 = Enabled.</returns>
            public string CERECORD_EnsemblePing_ToString()
            {
                if (CERECORD_EnsemblePing)
                {
                    return "1";
                }

                return "0";
            }

            /// <summary>
            /// Ensemble Recording
            /// When single recording is enabled and the ADCP is started
            /// (START [CR]) the firmware searches for the next available
            /// file number to record to on the SD card.  The single ping file name
            /// starts with the letter "S" followed by a 7 digit number and the ending with the
            /// extension ".ens".  For example the first single ping file will be named
            /// "S00000001.ens".  During deployment as each ping is completed the 
            /// data is appended to the current file.  The 7 digit number following the
            /// "S" is incremented each time the system is (re)started or when the file size
            /// exceeds 16MBytes.  Each ping, whether Bottom Track or Water Profile, is 
            /// considered to be a single ping.
            /// 
            /// Note: No error/threshold screening or coordinate transformation is 
            /// preformed on the data contained in a single ping file.
            /// 
            /// Command: CERECORD x,n[cr]
            /// Scale: 0=disable, 1=enable.
            /// Range:
            /// </summary>
            public bool CERECORD_SinglePing { get; set; }

            /// <summary>
            /// Return a string of whether
            /// CERECORD_SinglePing is enabled or disabled.
            /// </summary>
            /// <returns>0 = Disabled / 1 = Enabled.</returns>
            public string CERECORD_SinglePing_ToString()
            {
                if (CERECORD_SinglePing)
                {
                    return "1";
                }

                return "0";
            }

            #endregion

            #region CEOUTPUT

            /// <summary>
            /// Ensemble output type.
            /// 
            /// n = 0
            /// Disable serial output.  Saves battery energy when recording
            /// data to the SD card during a self-contained deployment by reducing
            /// extra on time on the system due to data transfer.
            /// 
            /// n= 1
            /// Enables the standard binary output data protocol to be sent out
            /// the serial port.
            /// 
            /// n=2
            /// Enables an ASCII text serial output that is dumb terminal compatible.
            /// 
            /// Command: CEOUTPUT n[cr]
            /// Scale: 0=disable, 1=enable Binary, 2=enable ASCII
            /// Range:
            /// </summary>
            private AdcpOutputMode _cEOUTPUT;
            /// <summary>
            /// Ensemble output type.
            /// 
            /// n = 0
            /// Disable serial output.  Saves battery energy when recording
            /// data to the SD card during a self-contained deployment by reducing
            /// extra on time on the system due to data transfer.
            /// 
            /// n= 1
            /// Enables the standard binary output data protocol to be sent out
            /// the serial port.
            /// 
            /// n=2
            /// Enables an ASCII text serial output that is dumb terminal compatible.
            /// 
            /// Command: CEOUTPUT n[cr]
            /// Scale: 0=disable, 1=enable Binary, 2=enable ASCII
            /// Range:
            /// </summary>
            public AdcpOutputMode CEOUTPUT
            {
                get { return _cEOUTPUT; }
                set
                {
                    // Verify the value is within range
                    _cEOUTPUT = value;
                }
            }

            #endregion

            #region CEPO

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

            #region SPOS

            /// <summary>
            /// Latitude position of the ADCP.
            /// </summary>
            public Latitude SPOS_Latitude { get; set; }

            /// <summary>
            /// Longitude position of the ADCP.
            /// </summary>
            public Longitude SPOS_Longitude { get; set; }

            /// <summary>
            /// Water depth in meters.
            /// </summary>
            public float SPOS_WaterDepth { get; set; }

            /// <summary>
            /// Pressure Sensor height in meters.
            /// </summary>
            public float SPOS_PsensHeight { get; set; }

            #endregion

            #endregion

            #region Environmental Properties

            /// <summary>
            /// Water Salinity
            /// Used in the water Speed of Sound calculation.
            /// 
            /// Command: CWS n.nn[cr]
            /// Scale: Parts per Thousand (ppt)
            /// Range: 0 to N
            /// </summary>
            private float _cWS;
            /// <summary>
            /// Water Salinity
            /// Used in the water Speed of Sound calculation.
            /// 
            /// Command: CWS n.nn[cr]
            /// Scale: Parts per Thousand (ppt)
            /// Range: 0 to N
            /// </summary>
            public float CWS 
            {
                get { return _cWS; } 
                set
                {
                    // Verify the value is within range
                    if (value >= MIN_CWS)
                    {
                        _cWS = value;
                    }
                }
            }

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
            /// Range: 0 to N
            /// </summary>
            private float _cTD;
            /// <summary>
            /// Transducer Depth
            /// Used in the water Speed of Sound calculation.
            /// 
            /// Command: CTD n.nn[cr]
            /// Scale: meters
            /// Range: 0 to N
            /// </summary>
            public float CTD 
            {
                get { return _cTD; } 
                set
                {
                    // Verify the value is within range
                    if (value >= MIN_CTD)
                    {
                        _cTD = value;
                    }
                }
            }

            /// <summary>
            /// Water Speed of Sound 
            /// NOT USED
            /// 
            /// Command CWSS n.nn[cr]
            /// Scale: meters per second (m/s)
            /// Range: 0 to N
            /// </summary>
            private float _cWSS;
            /// <summary>
            /// Water Speed of Sound 
            /// NOT USED
            /// 
            /// Command CWSS n.nn[cr]
            /// Scale: meters per second (m/s)
            /// Range: 0 to N
            /// </summary>
            public float CWSS 
            {
                get { return _cWSS; } 
                set
                {
                    // Verify the value is within range
                    if (value >= MIN_CWSS)
                    {
                        _cWSS = value;
                    }
                }
            }

            /// <summary>
            /// Heading offset
            /// Added to the compass output 
            /// prior to heading being used within the system.
            /// 
            /// Command: CHO n.nn[cr]
            /// Scale: degrees
            /// Range: -180.0 to +180.0
            /// </summary>
            private float _cHO;
            /// <summary>
            /// Heading offset
            /// Added to the compass output 
            /// prior to heading being used within the system.
            /// 
            /// Command: CHO n.nn[cr]
            /// Scale: degrees
            /// Range: -180.0 to +180.0
            /// </summary>
            public float CHO
            {
                get { return _cHO; }
                set
                {
                    // Verify the value is within range
                    if (value >= MIN_CHO && value <= MAX_CHO)
                    {
                        _cHO = value;
                    }
                }
            }

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

            /// <summary>
            /// Velocity Scale Factor.
            /// This scale factor is applied to all velocity
            /// measurement data.  New Velocity = CVSF * Velocity.
            /// 
            /// Command: CVSF n.nn[cr]
            /// Scale: 
            /// Range:
            /// </summary>
            public float CVSF { get; set; }


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
            public AdcpCommands()
            {
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
                CEI = new TimeValue(DEFAULT_CEI_HOUR, DEFAULT_CEI_MINUTE, DEFAULT_CEI_SECOND, DEFAULT_CEI_HUNSEC);
                //CEI_Hour = DEFAULT_CEI_HOUR;
                //CEI_Minute = DEFAULT_CEI_MINUTE;
                //CEI_Second = DEFAULT_CEI_SECOND;
                //CEI_HunSec = DEFAULT_CEI_HUNSEC;

                // Time of First ping default
                // Use the current time
                CETFP = DateTime.Now;
                //DateTime timeNow = DateTime.Now;
                //CETFP_Year = (UInt16)(timeNow.Year);
                //CETFP_Month = (UInt16)timeNow.Month;
                //CETFP_Day = (UInt16)timeNow.Day;
                //CETFP_Hour = (UInt16)timeNow.Hour;
                //CETFP_Minute = (UInt16)timeNow.Minute;
                //CETFP_Second = (UInt16)timeNow.Second;
                //CETFP_HunSec = DEFAULT_CETFP_HUNSEC;

                CERECORD_EnsemblePing = DEFAULT_CERECORD;
                CERECORD_SinglePing = DEFAULT_CERECORD_SINGLEPING;
                CEOUTPUT = DEFAULT_CEOUTPUT;
                CEPO = DEFAULT_CEPO;

                SPOS_Latitude = DEFAULT_LAT;
                SPOS_Longitude = DEFAULT_LONG;
                SPOS_WaterDepth = DEFAULT_WATER_DEPTH;
                SPOS_PsensHeight = DEFAULT_PSEN_HEIGHT;

                // Environmental defaults
                CWS = DEFAULT_CWS;
                CWT = DEFAULT_CWT;
                CTD = DEFAULT_CTD;
                CWSS = DEFAULT_CWSS;
                CHO = DEFAULT_CHO;
                CHS = DEFAULT_CHS;
                CVSF = DEFAULT_CVSF;

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
            /// <returns>The command with the time as a string properly formated for the ADCP time command.</returns>
            public static string GetLocalSystemTimeCommand()
            {
                return String.Format("{0} {1}", CMD_STIME, DateTime.Now.ToString("yyyy/MM/dd,HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US")));
            }

            /// <summary>
            /// Return the command to set
            /// the current time as UTC time to the device.
            /// STIME.
            /// </summary>
            /// <returns>The command with the time as a string properly formated for the ADCP time command.</returns>
            public static string GetGmtSystemTimeCommand()
            {
                return String.Format("{0} {1}", CMD_STIME, DateTime.Now.ToUniversalTime().ToString("yyyy/MM/dd,HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US")));
            }

            /// <summary>
            /// Create a list of all the commands and there value.
            /// Add all the commands to the list and there value.
            /// 
            /// Put all the values in United States English format.
            /// Other formats can use a comma instead of a decimal point for
            /// decimal numbers.
            /// </summary>
            /// <returns>List of all the commands and there value.</returns>
            public List<string> GetCommandList(AdcpTimeZone tz = AdcpTimeZone.LOCAL)
            {
                List<string> list = new List<string>();
                list.Add(CEPO_CmdStr());                                // CEPO     THIS WILL SET DEFAULTS SO IT MUST BE SET FIRST
                list.Add(Mode_CmdStr());                                // Mode
                list.Add(CEI_CmdStr());                                 // CEI
                list.Add(CETFP_CmdStr());                               // CETFP
                list.Add(CERECORD_CmdStr());                            // CERECORD  CultureInfo.CreateSpecificCulture("en-US")   Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
                list.Add(CEOUTPUT_CmdStr());                            // CEOUTPUT
                list.Add(SPOS_CmdStr());                                // SPOS

                list.Add(CWS_CmdStr());                                 // CWS
                list.Add(CWT_CmdStr());                                 // CWT
                list.Add(CTD_CmdStr());                                 // CTD
                list.Add(CWSS_CmdStr());                                // CWSS
                list.Add(CHS_CmdStr());                                 // CHS
                list.Add(CHO_CmdStr());                                 // CHO
                list.Add(CVSF_CmdStr());                                // CVSF

                list.Add(C232B_CmdStr());                               // C232B
                list.Add(C485B_CmdStr());                               // C485B
                list.Add(C422B_CmdStr());                               // C422B

                if (tz == AdcpTimeZone.LOCAL)
                {
                    list.Add(LocalTime_CmdStr());                       // Local Time         SET THE TIME LAST, IT TAKES THE LONGEST TO SET
                }
                else
                {
                    list.Add(GmtTime_CmdStr());                         // GMT Time           SET THE TIME LAST, IT TAKES THE LONGEST TO SET
                }

                return list;
            }

            /// <summary>
            /// A shorten list of commands that are neccessary for a deployment.
            /// Create a list of all the deployment commands and there value.
            /// Add all the commands to the list and there value.
            /// </summary>
            /// <param name="tz">Timezone to use.</param>
            /// <returns>List of all the commands and there value.</returns>
            public List<string> GetDeploymentCommandList(AdcpTimeZone tz = AdcpTimeZone.LOCAL)
            {
                List<string> list = new List<string>();

                list.Add(CEPO_CmdStr());                            // CEPO
                list.Add(CEOUTPUT_CmdStr());                        // CEOUTPUT
                list.Add(SPOS_CmdStr());                            // SPOS
                list.Add(CWS_CmdStr());                             // CWS
                list.Add(CWT_CmdStr());                             // CWT
                list.Add(CHO_CmdStr());                             // CHO
                list.Add(CEI_CmdStr());                             // CEI
                list.Add(CETFP_CmdStr());                           // CETFP
                list.Add(CERECORD_CmdStr());                        // CERECORD

                if (tz == AdcpTimeZone.LOCAL)
                {
                    list.Add(LocalTime_CmdStr());                   // Local Time         SET THE TIME LAST, IT TAKES THE LONGEST TO SET
                }
                else
                {
                    list.Add(GmtTime_CmdStr());                     // GMT Time           SET THE TIME LAST, IT TAKES THE LONGEST TO SET
                }

                return list;
            }


            /// <summary>
            /// This will be a string representation
            /// of the object.  It will also be 
            /// used to pass to the ADCP for all the 
            /// commands.
            /// 
            /// Put all the values in United States English format.
            /// Other formats can use a comma instead of a decimal point for
            /// decimal numbers.
            /// </summary>
            /// <returns>All the commands as a string.</returns>
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(Mode_ToString());                     // Mode
                sb.AppendLine(LocalTime_CmdStr());                  // DateTime
                sb.AppendLine(CEI_CmdStr());                        // CEI
                sb.AppendLine(CETFP_CmdStr());                      // CETFP
                sb.AppendLine(CERECORD_CmdStr());                   // CERECORD
                sb.AppendLine(CEOUTPUT_CmdStr());                   // CEOUTPUT
                sb.AppendLine(CEPO_CmdStr());                       // CEPO
                sb.AppendLine(SPOS_CmdStr());                       // SPOS

                sb.AppendLine(CWS_CmdStr());                        // CWS
                sb.AppendLine(CWT_CmdStr());                        // CWT
                sb.AppendLine(CTD_CmdStr());                        // CTD
                sb.AppendLine(CWSS_CmdStr());                       // CWSS
                sb.AppendLine(CHS_CmdStr());                        // CHS
                sb.AppendLine(CHO_CmdStr());                        // CHO
                sb.AppendLine(CVSF_CmdStr());                       // CVSF
                
                sb.AppendLine(C232B_CmdStr());                      // C232B
                sb.AppendLine(C485B_CmdStr());                      // C485B
                sb.AppendLine(C422B_CmdStr());                      // C422B

                // Add the subsystem commands

                return sb.ToString();
            }

            /// <summary>
            /// This will be a string representation
            /// of the object.  It will also be 
            /// used to pass to the ADCP for all the 
            /// commands.
            /// 
            /// Put all the values in United States English format.
            /// Other formats can use a comma instead of a decimal point for
            /// decimal numbers.
            /// </summary>
            /// <returns>All the commands as a string.</returns>
            public string ToString(AdcpTimeZone tz = AdcpTimeZone.LOCAL)
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(Mode_ToString());                     // Mode

                if (tz == AdcpTimeZone.LOCAL)
                {
                    sb.AppendLine(LocalTime_CmdStr());             // Local DateTime
                }
                else
                {
                    sb.AppendLine(GmtTime_CmdStr());               // Gmt DateTime
                }
                
                sb.AppendLine(CEI_CmdStr());                        // CEI
                sb.AppendLine(CETFP_CmdStr());                      // CETFP
                sb.AppendLine(CERECORD_CmdStr());                   // CERECORD
                sb.AppendLine(CEOUTPUT_CmdStr());                   // CEOUTPUT
                sb.AppendLine(CEPO_CmdStr());                       // CEPO
                sb.AppendLine(SPOS_CmdStr());                       // SPOS

                sb.AppendLine(CWS_CmdStr());                        // CWS
                sb.AppendLine(CWT_CmdStr());                        // CWT
                sb.AppendLine(CTD_CmdStr());                        // CTD
                sb.AppendLine(CWSS_CmdStr());                       // CWSS
                sb.AppendLine(CHS_CmdStr());                        // CHS
                sb.AppendLine(CHO_CmdStr());                        // CHO
                sb.AppendLine(CVSF_CmdStr());                       // CVSF

                sb.AppendLine(C232B_CmdStr());                      // C232B
                sb.AppendLine(C485B_CmdStr());                      // C485B
                sb.AppendLine(C422B_CmdStr());                      // C422B

                // Add the subsystem commands

                return sb.ToString();
            }

            #region Lists

            /// <summary>
            /// Get a list of all the mode types.
            /// </summary>
            /// <returns>List of all the modes.</returns>
            public static BindingList<AdcpCommands.AdcpMode> GetModeList()
            {
                BindingList<AdcpCommands.AdcpMode>  ModeList = new BindingList<AdcpCommands.AdcpMode>();
                ModeList.Add(AdcpCommands.AdcpMode.PROFILE);
                ModeList.Add(AdcpCommands.AdcpMode.DVL);

                return ModeList;
            }

            /// <summary>
            /// Get a list for all the years.
            /// </summary>
            /// <returns>List of all the years.</returns>
            public static BindingList<UInt16> GetYearList()
            {
                BindingList<UInt16> YearList = new BindingList<UInt16>();
                for (UInt16 x = AdcpCommands.MIN_YEAR; x <= AdcpCommands.MAX_YEAR; x++)
                {
                    YearList.Add(x);
                }

                return YearList;
            }

            /// <summary>
            /// Get a list for all the hours.
            /// </summary>
            /// <returns>List of all the hours.</returns>
            public static BindingList<UInt16> GetHourList()
            {
                BindingList<UInt16> HourList = new BindingList<UInt16>();
                for (UInt16 x = AdcpCommands.MIN_HOUR; x <= AdcpCommands.MAX_HOUR; x++)
                {
                    HourList.Add(x);
                }

                return HourList;
            }

            /// <summary>
            /// Get a list for all the Minutes and Seconds.
            /// </summary>
            /// <returns>List for all the Minutes and Seconds.</returns>
            public static BindingList<UInt16> GetMinSecList()
            {
                BindingList<UInt16>  MinSecsList = new BindingList<UInt16>();
                for (UInt16 x = AdcpCommands.MIN_MINSEC; x <= AdcpCommands.MAX_MINSEC; x++)
                {
                    MinSecsList.Add(x);
                }

                return MinSecsList;
            }

            /// <summary>
            /// Get a list for all the Hundredth of a second.
            /// </summary>
            public static BindingList<UInt16> GetHunSecList()
            {
                BindingList<UInt16> HunSecList = new BindingList<UInt16>();
                for (UInt16 x = AdcpCommands.MIN_HUNSEC; x <= AdcpCommands.MAX_HUNSEC; x++)
                {
                    HunSecList.Add(x);
                }

                return HunSecList;
            }

            /// <summary>
            /// Get a list for all the Months.
            /// </summary>
            /// <returns>List of all the months.</returns>
            public static BindingList<UInt16> GetMonthList()
            {
                BindingList<UInt16> MonthList = new BindingList<UInt16>();
                for (UInt16 x = AdcpCommands.MIN_MONTH; x <= AdcpCommands.MAX_MONTH; x++)
                {
                    MonthList.Add(x);
                }

                return MonthList;
            }

            /// <summary>
            /// Get a list for all the days.
            /// </summary>
            /// <returns>List for all the days.</returns>
            public static BindingList<UInt16> GetDayList()
            {
                BindingList<UInt16> DayList = new BindingList<UInt16>();
                for (UInt16 x = AdcpCommands.MIN_DAY; x <= AdcpCommands.MAX_DAY; x++)
                {
                    DayList.Add(x);
                }

                return DayList;
            }

            /// <summary>
            /// Get a list for all the heading sources.
            /// </summary>
            /// <returns>Get a list for all the heading sources.</returns>
            public static BindingList<Commands.HeadingSrc> GetHeadingSourceList()
            {
                BindingList<Commands.HeadingSrc> HeadingSourceList = new BindingList<Commands.HeadingSrc>();
                HeadingSourceList.Add(Commands.HeadingSrc.INTERNAL);
                HeadingSourceList.Add(Commands.HeadingSrc.SERIAL);

                return HeadingSourceList;
            }

            /// <summary>
            /// Get a list for all the Output.
            /// </summary>
            /// <returns>Get a list for all the Output.</returns>
            public static BindingList<AdcpOutputMode> GetOutputList()
            {
                BindingList<AdcpOutputMode> OutputList = new BindingList<AdcpOutputMode>();
                OutputList.Add(AdcpCommands.AdcpOutputMode.Disable);
                OutputList.Add(AdcpCommands.AdcpOutputMode.Binary);
                OutputList.Add(AdcpCommands.AdcpOutputMode.ASCII);

                return OutputList;
            }

            #endregion

            #region Command Strings

            #region Mode

            /// <summary>
            /// Return the Mode command string.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string Mode_CmdStr()
            {
                return String.Format("{0}", Mode_ToString());
            }

            #endregion

            #region Time

            /// <summary>
            /// Return the Local System Time command string.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string LocalTime_CmdStr()
            {
                return string.Format("{0}", GetLocalSystemTimeCommand());
            }

            /// <summary>
            /// Return the GMT System Time command string.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string GmtTime_CmdStr()
            {
                return string.Format("{0}", GetGmtSystemTimeCommand());
            }

            #endregion

            #region CEI

            /// <summary>
            /// Return the CEI command string.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CEI_CmdStr()
            {
                return String.Format("{0} {1}", CMD_CEI, CEI_ToString());
            }

            #endregion

            #region CETFP

            /// <summary>
            /// Return the CETFP command string.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CETFP_CmdStr()
            {
                return String.Format("{0} {1}", CMD_CETFP, CETFP_ToString());
            }

            #endregion

            #region CERECORD

            /// <summary>
            /// Return the CERECORD command string.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CERECORD_CmdStr()
            {
                return String.Format("{0} {1},{2}", CMD_CERECORD, CERECORD_EnsemblePing_ToString(), CERECORD_SinglePing_ToString());
            }

            #endregion

            #region CEOUTPUT

            /// <summary>
            /// Return the CEOUTPUT command string.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CEOUTPUT_CmdStr()
            {
                return string.Format("{0} {1}", CMD_CEOUTPUT, ((int)CEOUTPUT).ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            #endregion

            #region CEPO

            /// <summary>
            /// Return the CEPO command string.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CEPO_CmdStr()
            {
                return String.Format("{0} {1}", CMD_CEPO, CEPO.ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            #endregion

            #region SPOS

            /// <summary>
            /// Return the SPOS command string.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string SPOS_CmdStr()
            {
                return string.Format("{0} {1}, {2}, {3}, {4}", CMD_SPOS, 
                                                                SPOS_Latitude.DecimalDegrees.ToString("0.0000000000000"),
                                                                SPOS_Longitude.DecimalDegrees.ToString("0.0000000000000"),
                                                                SPOS_WaterDepth.ToString("0.000"),
                                                                SPOS_PsensHeight.ToString("0.000"));
            }

            #endregion

            #region CWS

            /// <summary>
            /// Return the CWS command string.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWS_CmdStr()
            {
                return String.Format("{0} {1}", CMD_CWS, CWS.ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            #endregion

            #region CWT

            /// <summary>
            /// Return the CWT command string.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWT_CmdStr()
            {
                return String.Format("{0} {1}", CMD_CWT, CWT.ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            #endregion

            #region CTD

            /// <summary>
            /// Return the CTD command string.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CTD_CmdStr()
            {
                return String.Format("{0} {1}", CMD_CTD, CTD.ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            #endregion

            #region CWSS

            /// <summary>
            /// Return the CWSS command string.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWSS_CmdStr()
            {
                return String.Format("{0} {1}", CMD_CWSS, CWSS.ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            #endregion

            #region CHS

            /// <summary>
            /// Return the CHS command string.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CHS_CmdStr()
            {
                return String.Format("{0} {1}", CMD_CHS, CHS_ToString());
            }

            #endregion

            #region CHO

            /// <summary>
            /// Return the CHO command string.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CHO_CmdStr()
            {
                return String.Format("{0} {1}", CMD_CHO, CHO.ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            #endregion

            #region CVSF

            /// <summary>
            /// Return the CVSF command string.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CVSF_CmdStr()
            {
                return String.Format("{0} {1}", CMD_CVSF, CVSF.ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            #endregion

            #region C232B

            /// <summary>
            /// Return the C232B command string.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string C232B_CmdStr()
            {
                return String.Format("{0} {1}", CMD_C232B, ((int)C232B).ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            #endregion

            #region C485B

            /// <summary>
            /// Return the C485B command string.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string C485B_CmdStr()
            {
                return String.Format("{0} {1}", CMD_C485B, ((int)C485B).ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            #endregion

            #region C422B

            /// <summary>
            /// Return the C422B command string.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string C422B_CmdStr()
            {
                return String.Format("{0} {1}", CMD_C422B, ((int)C422B).ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            #endregion

            #endregion

            #endregion

            #region Decode Commands

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

                // Check if the buffer given was empty
                if (string.IsNullOrEmpty(buffer))
                {
                    return new BreakStmt();
                }

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
                }

                // Hardware
                if (lines.Length > 2)
                {
                    hw = lines[1];
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
            /// id = 50007 or 50007-06
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

                // Do nothing with the board frequency yet
                string boardFreq = "";

                // Try to parse the Board ID
                // If there is an error return NULL
                int boardId = 0;

                // Newer boards will have a board ID and frequency
                // This will convert the id if it contains the frequency also
                if (id.Contains("-"))
                {
                    char[] delimiter = new char[]{'-'};
                    string[] boardIdFreq = id.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    if (boardIdFreq.Length >= 2)
                    {
                        id = boardIdFreq[0];                // Board ID
                        boardFreq = boardIdFreq[1];         // Board Freq
                    }
                }

                // Convert the board ID to int
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
                        DateTime.TryParse(line, out dt);
                    }
                }

                return dt;
            }

            #endregion

            #region DSDIR

            /// <summary>
            /// Parse the buffer of all the lines containing
            /// a file.  This will be the complete line with the
            /// file name, file size and date.
            /// 
            /// Ex:
            /// 
            /// DSDIR
            /// Total Space:                       3781.500 MB
            /// BOOT.BIN     2012/07/10 06:41:00      0.023
            /// RTISYS.BIN   2012/07/24 09:56:18      0.184
            /// HELP.TXT     2012/07/10 07:10:12      0.003
            /// BBCODE.BIN   2012/07/10 06:41:00      0.017
            /// ENGHELP.TXT  2012/07/17 09:49:04      0.002
            /// SYSCONF.BIN  2012/07/24 10:56:08      0.003
            /// A0000001.ENS 2012/04/02 16:53:11      1.004
            /// A0000002.ENS 2012/04/02 17:53:11      1.004
            /// A0000003.ENS 2012/04/02 18:53:11      1.004
            /// A0000004.ENS 2012/04/02 19:53:11      1.004
            /// A0000005.ENS 2012/04/02 20:53:11      1.004
            /// Used Space:                           5.252 MB
            /// 
            /// DSDIR
            /// 
            /// </summary>
            /// <param name="buffer">Buffer containing all the file info.</param>
            /// <returns>List of all the ENS and RAW files in the file list.</returns>
            public static AdcpDirListing DecodeDSDIR(string buffer)
            {
                AdcpDirListing listing = new AdcpDirListing();

                // Parse the directory listing string for all file info
                // Each line should contain a piece of file info
                string[] lines = buffer.Split('\n');

                // Create a list of all the ENS files
                for (int x = 0; x < lines.Length; x++)
                {
                    // Only add the ENS files to the list
                    // Ignore the txt and bin files
                    if (lines[x].Contains("ENS") || lines[x].Contains("RAW"))
                    {
                        listing.DirListing.Add(new AdcpEnsFileInfo(lines[x]));
                    }

                    // Total Space
                    if (lines[x].Contains("Total Space"))
                    {
                        // Parse the string of all it elements
                        // Total Space:                       3781.500 MB
                        char[] delimiters = { ':' };
                        string[] sizeInfo = lines[x].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                        // If it has 2 elements, the second element should be the size
                        if (sizeInfo.Length >= 2)
                        {
                            // Remove the MB from the end of the value
                            string sizeStr = sizeInfo[1].Trim();
                            char[] delimiters1 = { ' ' };
                            string[] sizeStrElem = sizeStr.Split(delimiters1, StringSplitOptions.RemoveEmptyEntries);

                            if (sizeStrElem.Length >= 1)
                            {
                                float size = 0.0f;
                                if (float.TryParse(sizeStrElem[0].Trim(), out size))
                                {
                                    listing.TotalSpace = size;
                                }
                            }
                        }
                    }

                    // Used space
                    if (lines[x].Contains("Used Space"))
                    {
                        // Parse the string of all it elements
                        // Used Space:                           5.252 MB
                        char[] delimiters = { ':' };
                        string[] sizeInfo = lines[x].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                        // If it has 2 elements, the second element should be the size
                        if (sizeInfo.Length >= 2)
                        {
                            // Remove the MB from the end of the value
                            string sizeStr = sizeInfo[1].Trim();
                            char[] delimiters1 = { ' ' };
                            string[] sizeStrElem = sizeStr.Split(delimiters1, StringSplitOptions.RemoveEmptyEntries);

                            if (sizeStrElem.Length >= 1)
                            {
                                float size = 0.0f;
                                if (float.TryParse(sizeStrElem[0].Trim(), out size))
                                {
                                    listing.UsedSpace = size;
                                }
                            }
                        }
                    }
                }

                return listing;
            }

            #endregion

            #region CSHOW

            /// <summary>
            /// Use the DecodeCSHOW decoder to decode the CSHOW command
            /// result.  It will return an AdcpConfiguration with all the
            /// Adcp configurations.
            /// </summary>
            /// <param name="buffer">Result string from CSHOW command.</param>
            /// <param name="serial">Serial number for the system.</param>
            /// <returns>AdcpConfiguration containing all the configurations found from CSHOW.</returns>
            public static AdcpConfiguration DecodeCSHOW(string buffer, SerialNumber serial)
            {
                DecodeCSHOW decoder = new DecodeCSHOW();
                return decoder.Decode(buffer, serial);
            }

            #endregion

            #region ENGCONF

            /// <summary>
            /// Decode the ENGCONF command.  This will
            /// tell you what devices are enabled.
            /// 
            /// Ex:
            /// ENGCONF
            /// Fram     0
            /// RTC      0
            /// KELLER30 0
            /// cd       0
            /// EMAC     0
            /// A11 RCVR 1
            /// SLEEP    0
            /// engconf
            /// </summary>
            /// <param name="buffer">Buffer to decode.</param>
            /// <returns>Results of the buffer decoding.</returns>
            public static EngConf DecodeENGCONF(string buffer)
            {
                // Initialize the value
                EngConf engConf = new EngConf();

                // Get each line of the buffer
                string[] lines = buffer.Split('\n');

                foreach (var line in lines)
                {
                    // Parse the string of all it elements
                    // KELLER30 0
                    char[] delimiters = { ' ' };
                    string[] hdwrInfo = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                    // Ensure it contains a type and value
                    if (hdwrInfo.Length > 1)
                    {
                        int value = 0;
                        // Parse the last value to get the value
                        int.TryParse(hdwrInfo[hdwrInfo.Length-1], out value);
                        bool result = false;
                        if (value >= 1)
                        {
                            result = true;
                        }

                        // Look for each value in the buffer
                        switch (hdwrInfo[0])
                        {
                            case "Fram":
                                engConf.IsFram = result;
                                break;
                            case "RTC":
                                engConf.IsRtc = result;
                                break;
                            case "KELLER30":
                                engConf.IsPressureSensor = result;
                                break;
                            case "cd":
                                engConf.IsCd = result;
                                break;
                            case "EMAC":
                                engConf.IsEmac = result;
                                break;
                            case "A11":
                                engConf.IsA11Rcvr = result;
                                break;
                            case "SLEEP":
                                engConf.IsSleep = result;
                                break;
                            default:
                                break;
                        }
                    }
                }

                return engConf;
            }

            #endregion

            #region SPOS

            /// <summary>
            /// Decode the SPOS command.
            /// 
            /// Ex:
            /// Lat =     2.00000000000000, Lon =     2.00000000000000, Depth =   200.000, P height =     0.000
            /// </summary>
            /// <param name="buffer">Buffer to decode.</param>
            /// <returns>Results of the buffer decoding.</returns>
            public static SPOS DecodeSPOS(string buffer)
            {
                SPOS spos = new SPOS();

                // Seperate each value
                string[] values = buffer.Split(',');

                // Go through each value
                foreach (var val in values)
                {
                    // Find all the values
                    string[] sposVal = val.Split('=');
                    if (sposVal.Length >= 2)
                    {
                        // Latitude
                        if (sposVal[0].Contains("Lat"))
                        {
                            double lat = 0.0;
                            if(double.TryParse(sposVal[1], out lat))
                            {
                                spos.SPOS_Latitude = new Latitude(lat);
                            }
                        }

                        // Longitude
                        if (sposVal[0].Contains("Lon"))
                        {
                            double lon = 0.0;
                            if (double.TryParse(sposVal[1], out lon))
                            {
                                spos.SPOS_Longitude = new Longitude(lon);
                            }
                        }

                        // Water Depth
                        if (sposVal[0].Contains("Depth"))
                        {
                            float depth = 0.0f;
                            if (float.TryParse(sposVal[1], out depth))
                            {
                                spos.SPOS_WaterDepth = depth;
                            }
                        }

                        // Pressure Sensor Height
                        if (sposVal[0].Contains("height"))
                        {
                            float height = 0.0f;
                            if (float.TryParse(sposVal[1], out height))
                            {
                                spos.SPOS_PsensHeight = height;
                            }
                        }

                    }

                }

                return spos;
            }

            #endregion

            #endregion

        }
    }
}