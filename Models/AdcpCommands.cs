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
 * 11/15/2013      RC          2.21.0     Added TimeZone property to know how to set STIME.
 * 06/10/2014      RC          2.22.0     Fixed finding the Hardware in DecodeBREAK().
 * 09/18/2014      RC          3.0.2      Added CTRIG and GetDvlCommandList().
 * 09/22/2014      RC          3.0.2      Added C232OUT, C422OUT, C485OUT and CWSSC.
 * 09/23/2014      RC          3.0.2      Changed CERECORD_EnsemblePing from a bool to a AdcpRecordOptions.
 * 09/30/2014      RC          3.0.2      Added DecodeCommandSet() to decode a command set.
 * 05/28/2015      RC          3.0.5      Added DiagSamp.
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
            /// Set the time value based off the DateTime given.
            /// This will convert the DateTime into a TimeValue.
            /// The Year, Month and Day will be ignored.
            /// </summary>
            /// <param name="dt">DateTime to use.</param>
            public TimeValue(DateTime dt)
            {
                Hour = Convert.ToUInt16(dt.Hour);
                Minute = Convert.ToUInt16(dt.Minute);
                Second = Convert.ToUInt16(dt.Second);
                HunSec = (ushort)Math.Round(dt.Millisecond * 0.1);
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

            /// <summary>
            /// Return a Date and Time based off the current values.
            /// The Date will be the current date..
            /// </summary>
            /// <returns></returns>
            public DateTime ToDateTime()
            {
                return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Hour, Minute, Second);
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
            /// Directory listing as a string.
            /// </summary>
            public string StrResult { get; set; }

            /// <summary>
            /// Initialize the list and sizes.
            /// </summary>
            public AdcpDirListing()
            {
                DirListing = new List<AdcpEnsFileInfo>();
                TotalSpace = 0;
                UsedSpace = 0;
                StrResult = "";
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

        #region DIAGSAMP

        /// <summary>
        /// The DIAGSAMP results.
        /// 
        /// DP600
        /// AutoGain:
        ///           bm0      bm1      bm2      bm3
        /// amp     39.05,   36.00,   40.52,   38.04
        /// Hz    1984.41,  192.05,  831.30, 1180.32
        /// cor      0.07,    0.01,    0.06,    0.02
        /// Heading    240.90 deg
        /// Pitch       -1.05 deg
        /// Roll       179.71 deg
        /// Water       22.91 deg
        /// BackPlane   24.83 deg
        /// Battery     19.62 V
        /// Boost+      22.69 V
        /// Boost-     -21.95 V
        /// </summary>
        public class DiagSamp
        {
            #region Properties

            /// <summary>
            /// System type.
            /// </summary>
            public string SystemType { get; set; }

            /// <summary>
            /// Amplitude data.
            /// </summary>
            public double[] Amp { get; set; }

            /// <summary>
            /// Hz data.
            /// </summary>
            public double[] Hz { get; set; }

            /// <summary>
            /// Correlation data.
            /// </summary>
            public double[] Cor { get; set; }

            /// <summary>
            /// Heading value in degrees.
            /// </summary>
            public double Heading { get; set; }

            /// <summary>
            /// Pitch value in degrees.
            /// </summary>
            public double Pitch { get; set; }

            /// <summary>
            /// Roll value in degrees.
            /// </summary>
            public double Roll { get; set; }

            /// <summary>
            /// Water temperature in degrees C.
            /// </summary>
            public double WaterTemp { get; set; }

            /// <summary>
            /// Backplane temperature in degrees C.
            /// </summary>
            public double BackPlaneTemp { get; set; }

            /// <summary>
            /// Battery voltage.
            /// </summary>
            public double Battery { get; set; }

            /// <summary>
            /// Boost Positive voltage.
            /// </summary>
            public double BoostPos { get; set; }

            /// <summary>
            /// Boost Negative voltage.
            /// </summary>
            public double BoostNeg { get; set; }

            /// <summary>
            /// String of the DiagSamp.
            /// </summary>
            public string DiagSampStr { get; set; }

            #endregion

            /// <summary>
            /// Initialize with the given number of beams.
            /// </summary>
            /// <param name="beams">Number of the beams.</param>
            public DiagSamp(int beams)
            {
                Init(beams);
            }

            /// <summary>
            /// Initialize with a default number of beams of 4.
            /// </summary>
            public DiagSamp()
            {
                Init(4);
            }

            /// <summary>
            /// Initialize the values.
            /// </summary>
            /// <param name="beams">Number of beams.</param>
            public void Init(int beams)
            {
                SystemType = "";
                Amp = new double[beams];
                Cor = new double[beams];
                Hz = new double[beams];
                Heading = 0.0;
                Pitch = 0.0;
                Roll = 0.0;
                WaterTemp = 0.0;
                BackPlaneTemp = 0.0;
                Battery = 0.0;
                BoostPos = 0.0;
                BoostNeg = 0.0;
            }
        }

        #endregion

        #region DiagPressure

        /// <summary>
        /// Pressure sensor properties.
        /// </summary>
        public class DiagPressure
        {
            #region Properties

            /// <summary>
            /// Flag if the pressure sensor is installed.
            /// </summary>
            public bool IsPressureSensorInstalled { get; set; }

            /// <summary>
            /// Pressure sensor rating.
            /// </summary>
            public string Rating { get; set; }

            /// <summary>
            /// Pressure sensor diagnostic string.
            /// </summary>
            public string DiagPressureStr { get; set; }

            #endregion

            /// <summary>
            /// Initialize the values.
            /// </summary>
            public DiagPressure()
            {
                Init();
            }

            /// <summary>
            /// Initialize the values.
            /// </summary>
            public void Init()
            {
                IsPressureSensorInstalled = false;
                Rating = "";
                DiagPressureStr = "";
            }
        }


        #endregion

        #endregion

        /// <summary>
        /// Commands handled by the ADCP.
        /// </summary>
        public class AdcpCommands
        {
            #region Variables

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

            /// <summary>
            /// Ensemble Command: External Trigger.
            /// Parameter: n
            /// Selects which state the external hardware tigger
            /// needs to be before pinging.  There are 2 types of trigger logice available Edge and
            /// Level.  Edge requires the trigger line to change state before the ping occurs.  For
            /// reliable edge detection the minimum width of a pulse should be >= 50 usec.
            /// Level just needs the trigger line to be either high or low.  There is a 1.4msec
            /// delay before the ping occurs after detection of the trigger.
            /// n = 1 High level
            /// n = 2 Low level
            /// n = 3 Low to high
            /// n = 4 High to low
            /// n = 0 Default disabled.
            /// </summary>
            public const string CMD_CTRIG = "CTRIG";

            #endregion

            #region UDP

            /// <summary>
            /// Flag to set CEMAC or not.  
            /// Temporarily enables Ethernet communication.  This command is
            /// typically sent via one of the serial ports.  The Ethernet port
            /// is disabled after power down or sleep.  To permanently enable the port
            /// a special factory configuration command is required.  Wehn the Ethernet
            /// port is permanently enabled the system requires an additional 2 seconds 
            /// after power up to begin accepting commands.
            /// 
            /// One of two responses occurs on the serial port after the command is accepted by the ADCP.
            /// 
            /// CEMAC+
            /// MAC 02:ff:fe:fd:fc:fb
            /// IP 192.168.1.130
            /// Link OK
            /// 
            /// OR
            /// 
            /// CEMAC+
            /// MAC 02:ff:fe:fd:fc:fb
            /// IP 192.168.1.130
            /// No Link
            /// </summary>
            public const string CMD_CEMAC = "CEMAC";

            /// <summary>
            /// Engineering Command: Change IP address
            /// Parameter: nnn.nnn.nnn.nnn
            /// Change the IP address.  
            /// </summary>
            public const string CMD_IP = "IP";

            /// <summary>
            /// Ensemble Command: CUDPOUT
            /// Parameter: n
            /// UDP output type. This will turn off and the type of data to output to the UDP port.
            /// n = 0 Default disabled.
            /// n = 1   RTI Binary output
            /// n = 2   RTI ASCII output
            /// n = 106 PD6 Binary output
            /// n = 113 PD13 ASCII output
            /// </summary>
            public const string CMD_CUDPOUT = "CUDPOUT";

            /// <summary>
            /// Ensemble Command: UDPPORT
            /// Parameter: n
            /// UDP port number.  The IP is set with the IP command.  This will be the port number for the UDP
            /// port to output the data on the UDP port.
            /// n = UDP Port number.
            /// </summary>
            public const string CMD_UDPPORT = "UDPPORT";

            #endregion

            #region Environmental

            /// <summary>
            /// Environmental Command: Water Speed of Sound Control.
            /// Parameter: n.nn (ppt) 
            /// Used to determine which sensors values should be used
            /// in the water Speed of Sound calculation.
            /// </summary>
            public const string CMD_CWSSC = "CWSSC";

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

            /// <summary>
            /// Serial Communication Command: RS-232 Output
            /// Parameter: n = 0 = off, 1 = on.
            /// The ADCP contains a RS-232 port.  Turn on or off the serial port.
            /// </summary>
            public const string CMD_C232OUT = "C232OUT";

            /// <summary>
            /// Serial Communication Command: RS-485 Output
            /// Parameter: n = 0 = off, 1 = on.
            /// The ADCP contains a RS-485 port.  Turn on or off the serial port.
            /// </summary>
            public const string CMD_C485OUT = "C485OUT";

            /// <summary>
            /// Serial Communication Command: RS-422 Output
            /// Parameter: n = 0 = off, 1 = on.
            /// The ADCP contains a RS-422 port.  Turn on or off the serial port.
            /// </summary>
            public const string CMD_C422OUT = "C422OUT";

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
            /// Engineering Command: Get the serial port used.
            /// Parameter:
            /// Serial port information.
            /// </summary>
            public const string CMD_ENGPORT = "ENGPORT";

            #endregion

            #region Diagnostic Commands

            /// <summary>
            /// Diagnostic Command: Read Pressure sensor
            /// Parameter: 
            /// Shows the Keller30 pressure sensor info. The first part of the response 
            /// shows the serial number and the version of firmware. The next portion 
            /// of the response contains the pressure sensor “Coefficients”. The third 
            /// line of which indicates the maximum pressure the sensor can measure. The 
            /// last portion of the response contains the pressure sensor “Configuration”. 
            /// These values can be programmed using Keller software when the pressure 
            /// sensor is disconnected from the ADCP.
            /// 
            /// DIAGPRESSURE+
            /// 
            /// PS SN: 157896
            /// Address: 250
            /// Hardware Ver: 5.20
            /// Firmware Ver: 5.50
            /// Buffer Size: 10
            /// Status: 1
            /// 
            /// Keller30 Read Coefficients:
            /// 64, offset   -1.01360321, bar,  0
            /// 80, min       0.20000000, bar,  0
            /// 81, max       3.00000000, bar,  0
            /// 84, min     -10.00000000, C,  0
            /// 85, max      80.00000000, C,  0
            /// 
            /// Keller30 Read Configuration:
            /// Nr, Conf, err
            ///  0,   02,   0
            ///  1,   10,   0
            ///  2,   00,   0
            ///  3,   02,   0
            ///  4,   05,   0
            ///  5,   00,   0
            ///  6,   02,   0
            ///  7,   35,   0
            ///  8,   13,   0
            ///  9,   00,   0
            /// 10,   00,   0
            /// 11,   35,   0
            /// 12,   00,   0
            /// 13,   FF,   0
            /// </summary>
            public const string CMD_DIAGPRESSURE = "DIAGPRESSURE";

            /// <summary>
            /// Diagnostic Command: Rub Beam Test
            /// Parameter: 
            /// Shows the results of the beam continuity 
            /// test where the user rubs each beam in 
            /// sequence to determine whether the transducer 
            /// cup is functional. The test collects statistics 
            /// for 10 samples then prompts the user to rub 
            /// the selected beam. PASS will be displayed when 
            /// the test detects the correct amplitude change 
            /// associated with rubbing a transducer. Up to 4 beams 
            /// will be tested per test. For systems with additional 
            /// beams use the CEPO command to select additional sub systems.
            /// 
            /// DIAGRUB+
            /// DP600 Rub Test:
            ///  10  9  8  7  6  5  4  3  2  1 Vigorously Rub Beam 0 ............
            /// Beam 0 PASS 58 - 41 >= 3
            ///  10  9  8  7  6  5  4  3  2  1 Vigorously Rub Beam 1 ..
            /// Beam 1 PASS 46 - 43 >= 3
            ///  10  9  8  7  6  5  4  3  2  1 Vigorously Rub Beam 2 ...
            /// Beam 2 PASS 64 - 42 >= 3
            ///  10  9  8  7  6  5  4  3  2  1 Vigorously Rub Beam 3 ..
            /// Beam 3 PASS 62 - 43 >= 3
            /// DP600 Rub Test Complete
            /// 
            /// </summary>
            public const string CMD_DIAGRUB = "DIAGRUB";

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
            /// Default RS-232 output.
            /// </summary>
            public const AdcpOutputMode DEFAULT_C232OUT = AdcpOutputMode.Disable;

            /// <summary>
            /// Default RS-422 output.
            /// </summary>
            public const AdcpOutputMode DEFAULT_C422OUT = AdcpOutputMode.Disable;

            /// <summary>
            /// Default RS-485 output.
            /// </summary>
            public const AdcpOutputMode DEFAULT_C485OUT = AdcpOutputMode.Disable;

            /// <summary>
            /// Default ADCP mode.
            /// </summary>
            public const AdcpMode DEFAULT_MODE = AdcpMode.PROFILE;

            /// <summary>
            /// Default MAC enable.
            /// </summary>
            public const bool DEFAULT_ENGMACON = false;

            /// <summary>
            /// Default Water Temperture source.
            /// Default use temperature sensor.
            /// </summary>
            public const UInt16 DEFAULT_CWSSC_WATERTEMPSRC = 1;

            /// <summary>
            /// Default Transducer Depth source.
            /// Default use depth from sensor.
            /// </summary>
            public const UInt16 DEFAULT_CWSSC_XDCRDEPTHSRC = 1;

            /// <summary>
            /// Default Salinity source.
            /// Default use salinity command.
            /// </summary>
            public const UInt16 DEFAULT_CWSSC_SALINITYSRC = 0;

            /// <summary>
            /// Default Speed of Sound source.
            /// Default use internal calculation.
            /// </summary>
            public const UInt16 DEFAULT_CWSSC_SOSSRC = 2;

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
            public const AdcpRecordOptions DEFAULT_CERECORD = AdcpRecordOptions.Disable;

            /// <summary>
            /// Default whether to record single ping on the system.
            /// </summary>
            public const bool DEFAULT_CERECORD_SINGLEPING = false;

            /// <summary>
            /// Default whether to output data to serial port.
            /// </summary>
            public const AdcpOutputMode DEFAULT_CEOUTPUT = AdcpCommands.AdcpOutputMode.Binary;

            /// <summary>
            /// Default PD0 coordinate transform.
            /// </summary>
            public const RTI.Core.Commons.Transforms DEFAULT_PD0_TRANSFORM = RTI.Core.Commons.Transforms.EARTH;

            /// <summary>
            /// Default Ocean Server Profile bin.
            /// </summary>
            public const UInt16 DEFAULT_OSPROFILEBIN = 0;

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

            /// <summary>
            /// Default CTRIG value.
            /// </summary>
            public const AdcpExternalTrigger DEFAULT_CTRIG = AdcpExternalTrigger.Disabled;

            /// <summary>
            /// By default the ethernet port is off.
            /// </summary>
            public const bool DEFAULT_CEMAC = false;

            /// <summary>
            /// Default CUDPOUT.
            /// </summary>
            public const UdpOutputMode DEFAULT_CUDPOUT = UdpOutputMode.Disable;

            /// <summary>
            /// Default UDP port.
            /// </summary>
            public const uint DEFAULT_UDPPORT = 257;

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

            /// <summary>
            /// Minimum value for CWSSC.
            /// </summary>
            public const UInt16 MIN_CWSSC = 0;

            /// <summary>
            /// Maximum value for CWSSC.
            /// </summary>
            public const UInt16 MAX_CWSSC = 2;

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
            /// Ensemble output modes for the ADCP. 
            /// These values are used with CEOUTPUT.
            /// </summary>
            public enum AdcpOutputMode
            {
                /// <summary>
                /// Disable serial output mode to the serial port.
                /// Saves battery energy when recording data
                /// to the SD card during a self-contained deployment 
                /// by reducing extra on time of the system due to data transfer.
                /// </summary>
                Disable = 0,

                /// <summary>
                /// Enables the standard binary output data protocol to be sent out
                /// the serial port when the system is in "profile" mode.  If the system
                /// is in "DVL" mode the $PRTI01 $PRTI02 $PRTI30 $PRTI31 data strings are
                /// output.
                /// </summary>
                Binary = 1,

                /// <summary>
                /// Enables an ASCII text serial outpu tthat is dumb terminal 
                /// compatible to be sent out the serial port when the system is in "profile"
                /// mode.  If the system is in "DVL" mode the $PRTI01 $PRTI02 $PRTI32 $PRTI33 
                /// data strings are output.
                /// </summary>
                ASCII = 2,

                /// <summary>
                /// Disables all output except for a NMEA status string.  Allows the user to verify
                /// that the instrument is operating normally while recording data to the internal
                /// recorder.  Saves power and can improve ping timing.
                /// </summary>
                NMEA_Status = 3,

                /// <summary>
                /// Eanbles the special Ocean Server NMEA DVL data output.  When CEOUTPUT 4 is
                /// selected and a second parameter can be sent to select the navigation bin.
                /// CEOUTPUT 4, b
                /// b = The profile bin that will be used in the $DVLNAV string.
                /// </summary>
                OceanServer = 4,

                /// <summary>
                /// If the system is in "DVL" mode the $PRTI03 data string is output.
                /// </summary>
                DVL_PRTI03 = 5,

                /// <summary>
                /// PD0 binary output.  When CEOUTPUT 100 is selected a second paramater 
                /// must be sent to select the velocity coordinate system.
                /// CEOUTPUT 100, c
                /// c = Coordinate Transform
                ///    0 = Beam Coordinate Transform
                ///    1 = Instrument Coordinate Transform
                ///    2 = Earth Coordinate Transform
                ///    3 = Ship Coordinate Transform
                /// </summary>
                PD0 = 100,

                /// <summary>
                /// PD13 ASCII output.
                /// </summary>
                PD13 = 113,

                /// <summary>
                /// PD3 binary output.
                /// </summary>
                PD3 = 103,

                /// <summary>
                /// PD4 binary output.
                /// </summary>
                PD4 = 104,

                /// <summary>
                /// PD5 binary output.
                /// </summary>
                PD5 = 105,

                /// <summary>
                /// PD6 binary output.
                /// </summary>
                PD6 = 106
            }

            /// <summary>
            /// Recording options for the ADCP.
            /// Turn on or off recording to the
            /// internal SD memory card.
            /// </summary>
            public enum AdcpRecordOptions
            {
                /// <summary>
                /// Disable recording to the internal
                /// SD memory card.
                /// </summary>
                Disable = 0,

                /// <summary>
                /// Enable recording to the internal
                /// SD memory card.
                /// </summary>
                Enable = 1,

                /// <summary>
                /// Enable recording to the internal
                /// SD memory card Bottom Track
                /// engineering data.
                /// </summary>
                BT_Eng = 2

            }

            /// <summary>
            /// Sensor sources to calculate the 
            /// Speed of Sound.  The sources
            /// can come from a command which
            /// will have the hard coded value,
            /// a sensor within the ADCP or an
            /// internal calculation with the ADCP.
            /// </summary>
            public enum SpeedOfSoundSources
            {
                /// <summary>
                /// Hard coded value from the command.
                /// </summary>
                Command = 0,

                /// <summary>
                /// Values from the sensor.
                /// </summary>
                Sensor = 1,

                /// <summary>
                /// Internal caluclation within the ADCP.
                /// </summary>
                InternalCalc = 2
            }

            /// <summary>
            /// ADCP External Trigger options.
            /// </summary>
            public enum AdcpExternalTrigger
            {
                /// <summary>
                /// Disable.
                /// </summary>
                Disabled = 0,

                /// <summary>
                /// High Level.
                /// </summary>
                HighLevel = 1,

                /// <summary>
                /// Low Level.
                /// </summary>
                LowLevel = 2,

                /// <summary>
                /// Low to High.
                /// </summary>
                LowToHigh = 3,

                /// <summary>
                /// High to Low.
                /// </summary>
                HighToLow = 4,
            }

            /// <summary>
            /// UDP Output mode.
            /// </summary>
            public enum UdpOutputMode
            {
                /// <summary>
                /// Disable serial output mode to the serial port.
                /// Saves battery energy when recording data
                /// to the SD card during a self-contained deployment 
                /// by reducing extra on time of the system due to data transfer.
                /// </summary>
                Disable = 0,

                /// <summary>
                /// Enables the standard binary output data protocol to be sent out
                /// the serial port when the system is in "profile" mode.  If the system
                /// is in "DVL" mode the $PRTI01 $PRTI02 $PRTI30 $PRTI31 data strings are
                /// output.
                /// </summary>
                Binary = 1,

                /// <summary>
                /// Enables an ASCII text serial outpu tthat is dumb terminal 
                /// compatible to be sent out the serial port when the system is in "profile"
                /// mode.  If the system is in "DVL" mode the $PRTI01 $PRTI02 $PRTI32 $PRTI33 
                /// data strings are output.
                /// </summary>
                ASCII = 2,

                /// <summary>
                /// PD13 ASCII output.
                /// </summary>
                PD13 = 113,

                /// <summary>
                /// PD6 binary output.
                /// </summary>
                PD6 = 106
            }

            #endregion

            #region Properties

            #region Mode

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

            #endregion

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
            /// Display the CEI time.
            /// </summary>
            /// <returns></returns>
            public string CEI_DescStr()
            {
                float milliseconds = (float)CEI.ToSecondsD() * 1000.0f;

                // Get the whole value that is disvisable by 1000 for the seconds
                // The remainder is the milliseconds left over
                int seconds = (int)(milliseconds / 1000.0f);
                int remainder = (int)(milliseconds % 1000.0f);

                TimeSpan ts = new TimeSpan(0, 0, 0, seconds, remainder);
                return MathHelper.TimeSpanPrettyFormat(ts) + " between each ensemble.";
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
            /// Scale: 0=disable, 1=enable, 2=BT Eng data.
            /// Range:
            /// </summary>
            public AdcpRecordOptions CERECORD_EnsemblePing { get; set; }

            /// <summary>
            /// Return a string of whether
            /// CERECORD is enabled or disabled.
            /// </summary>
            /// <returns>0 = Disabled / 1 = Enabled.</returns>
            public string CERECORD_EnsemblePing_ToString()
            {
                switch(CERECORD_EnsemblePing)
                {
                    case AdcpRecordOptions.Enable:
                        return "1";
                    case AdcpRecordOptions.BT_Eng:
                        return "2";
                    case AdcpRecordOptions.Disable:
                    default:
                        return "0";
                }
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
            /// CEOUTPUT n[CR] Ensemble output type.
            /// 1.  n = 0   disables serial output. Saves battery energy when recording data to the SD card during a self-contained deployment by reducing extra on time of the system due to data transfer.
            /// 2.  n = 1   enables the standard binary output data protocol to be sent out the serial port when the system is in “profile” mode. If the system is in “DVL” mode the $PRTI01 $PRTI02 $PRTI30 $PRTI31 data strings are output.
            /// 3.  n = 2   enables an ASCII text serial output that is dumb terminal compatible to be sent out the serial port when the system is in “profile” mode. If the system is in “DVL” mode the $PRTI01 $PRTI02 $PRTI32 $PRTI33 data strings are output.
            /// 4.  n = 3   disables all output except for a NMEA status string. Allows the user to verify that the instrument is operating normally while recording data to the internal recorder. Saves power and can improve ping timing.
            /// 5.  n = 4   enable the special Ocean Server NMEA DVL data output. When CEOUTPUT 4 is selected a second parameter can be sent to select the navigation bin. CEOUTPUT 4, b[CR] where b is the profile bin that will be used in the $DVLNAV string.
            /// 6.  n = 5   If the system is in “DVL” mode the $PRTI03 data string is output.
            /// 7.  n = 100 selects PD0 binary output. When CEOUTPUT 100 is selected a second parameter can be sent to select the velocity coordinate system. CEOUTPUT 100, c[CR] where c = 0 is beam coordinates, c = 1 is instrument (XYZ), c = 2 is Earth (ENU), and c = 3 is Ship (SFM).
            /// 8.  n = 113 selects PD13 ASCII output.
            /// 9.  n = 103 selects PD3 binary output. When CEOUTPUT 103 is selected a second parameter can be sent to select the velocity coordinate system. See n = 100.
            /// 10. n = 104 selects PD4 binary output. When CEOUTPUT 103 is selected a second parameter can be sent to select the velocity coordinate system. See n = 100.
            /// 11. n = 105 selects PD5 binary output. When CEOUTPUT 103 is selected a second parameter can be sent to select the velocity coordinate system. See n = 100.
            /// 12. n = 106 selects PD6 binary output.
            /// Important Note: PD output formats are industry standard formats for DVLs and are typically a string of data output that is a subset of the total data available. Below is a summary of the data available in PD specific formats:
            /// PD0  - binary output format that includes a header, fixed and variable leader, bottom track, and water profile information. The fixed and variable leader is a recording of time, DVL setup, orientation, heading, pitch, roll, temperature, pressure and self test diagnostic results. The user can select data fields to be output. In the case with RTI instruments users can select the coordinates for the data to be represented.
            /// PD4  - is a binary output format that presents bottom track speed over bottom, speed through water and range to bottom information only.
            /// PD5  – is a superset of PD4 and includes additional information such as, salinity, depth, pitch, roll, heading, and distance made good.
            /// PD6  – is a text-based output format that groups separate sentences containing system attitude data, timing and scaling and speed through water relative to the instrument, vehicle, and earth coordinates. Each data sentence contains a unique starting delimiter and comma delimited fields.
            /// PD13 - is a text output format, like PD6 with the addition of information about range to bottom and raw pressure sensor data.
            /// 
            /// Command: CEOUTPUT n[cr]
            /// Scale: 
            /// Range:
            /// </summary>
            private AdcpOutputMode _cEOUTPUT;
            /// <summary>
            /// CEOUTPUT n[CR] Ensemble output type.
            /// 1.  n = 0   disables serial output. Saves battery energy when recording data to the SD card during a self-contained deployment by reducing extra on time of the system due to data transfer.
            /// 2.  n = 1   enables the standard binary output data protocol to be sent out the serial port when the system is in “profile” mode. If the system is in “DVL” mode the $PRTI01 $PRTI02 $PRTI30 $PRTI31 data strings are output.
            /// 3.  n = 2   enables an ASCII text serial output that is dumb terminal compatible to be sent out the serial port when the system is in “profile” mode. If the system is in “DVL” mode the $PRTI01 $PRTI02 $PRTI32 $PRTI33 data strings are output.
            /// 4.  n = 3   disables all output except for a NMEA status string. Allows the user to verify that the instrument is operating normally while recording data to the internal recorder. Saves power and can improve ping timing.
            /// 5.  n = 4   enable the special Ocean Server NMEA DVL data output. When CEOUTPUT 4 is selected a second parameter can be sent to select the navigation bin. CEOUTPUT 4, b[CR] where b is the profile bin that will be used in the $DVLNAV string.
            /// 6.  n = 5   If the system is in “DVL” mode the $PRTI03 data string is output.
            /// 7.  n = 100 selects PD0 binary output. When CEOUTPUT 100 is selected a second parameter can be sent to select the velocity coordinate system. CEOUTPUT 100, c[CR] where c = 0 is beam coordinates, c = 1 is instrument (XYZ), c = 2 is Earth (ENU), and c = 3 is Ship (SFM).
            /// 8.  n = 113 selects PD13 ASCII output.
            /// 9.  n = 103 selects PD3 binary output. When CEOUTPUT 103 is selected a second parameter can be sent to select the velocity coordinate system. See n = 100.
            /// 10. n = 104 selects PD4 binary output. When CEOUTPUT 103 is selected a second parameter can be sent to select the velocity coordinate system. See n = 100.
            /// 11. n = 105 selects PD5 binary output. When CEOUTPUT 103 is selected a second parameter can be sent to select the velocity coordinate system. See n = 100.
            /// 12. n = 106 selects PD6 binary output.
            /// Important Note: PD output formats are industry standard formats for DVLs and are typically a string of data output that is a subset of the total data available. Below is a summary of the data available in PD specific formats:
            /// PD0  - binary output format that includes a header, fixed and variable leader, bottom track, and water profile information. The fixed and variable leader is a recording of time, DVL setup, orientation, heading, pitch, roll, temperature, pressure and self test diagnostic results. The user can select data fields to be output. In the case with RTI instruments users can select the coordinates for the data to be represented.
            /// PD4  - is a binary output format that presents bottom track speed over bottom, speed through water and range to bottom information only.
            /// PD5  – is a superset of PD4 and includes additional information such as, salinity, depth, pitch, roll, heading, and distance made good.
            /// PD6  – is a text-based output format that groups separate sentences containing system attitude data, timing and scaling and speed through water relative to the instrument, vehicle, and earth coordinates. Each data sentence contains a unique starting delimiter and comma delimited fields.
            /// PD13 - is a text output format, like PD6 with the addition of information about range to bottom and raw pressure sensor data.
            /// 
            /// Command: CEOUTPUT n[cr]
            /// Scale: 
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

            /// <summary>
            /// Ocean Server Profile bin if CEOUTPUT is set to 4.
            /// </summary>
            public UInt16 CEOUTPUT_OceanServerProfileBin { get; set; }

            /// <summary>
            /// PD0, PD3, PD4 or PD5 Coordinate Transform if CEOUTPUT.
            /// </summary>
            public RTI.Core.Commons.Transforms CEOUTPUT_PdCoordinateTransform { get; set; }

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

            #region CWSSC

            /// <summary>
            /// Water Speed of Sound Control Water Temperature Source.
            /// 
            /// Water Speed of Sound Control.
            /// 0 = Command
            /// 1 = Sensor
            /// 2 = Internal Calculation
            /// 
            /// Command: CWSSC a,b,c,d[cr]
            /// a = Water Temperature Source
            /// b = Transducer Depth Source
            /// c = Salinity Source
            /// d = Speed of Sound Source
            /// Scale: 
            /// Range:  0 = Command
            ///         1 = Sensor
            ///         2 = Internal Calculation
            /// </summary>
            private UInt16 _cWSSC_WaterTempSrc;
            /// <summary>
            /// Water Speed of Sound Control Water Temperature Source.
            /// 
            /// Water Speed of Sound Control.
            /// 0 = Command
            /// 1 = Sensor
            /// 2 = Internal Calculation
            /// 
            /// Command: CWSSC a,b,c,d[cr]
            /// a = Water Temperature Source
            /// b = Transducer Depth Source
            /// c = Salinity Source
            /// d = Speed of Sound Source
            /// Scale: 
            /// Range:  0 = Command
            ///         1 = Sensor
            ///         2 = Internal Calculation
            /// </summary>
            public UInt16 CWSSC_WaterTempSrc
            {
                get { return _cWSSC_WaterTempSrc; }
                set
                {
                    // Verify the value is within range
                    if (value >= MIN_CWSSC && value <= MAX_CWSSC)
                    {
                        _cWSSC_WaterTempSrc = value;
                    }
                }
            }

            /// <summary>
            /// Water Speed of Sound Control Transducer Depth Source.
            /// 
            /// Water Speed of Sound Control.
            /// 0 = Command
            /// 1 = Sensor
            /// 2 = Internal Calculation
            /// 
            /// Command: CWSSC a,b,c,d[cr]
            /// a = Water Temperature Source
            /// b = Transducer Depth Source
            /// c = Salinity Source
            /// d = Speed of Sound Source
            /// Scale: 
            /// Range:  0 = Command
            ///         1 = Sensor
            ///         2 = Internal Calculation
            /// </summary>
            private UInt16 _cWSSC_TransducerDepthSrc;
            /// <summary>
            /// Water Speed of Sound Control Transducer Depth Source.
            /// 
            /// Water Speed of Sound Control.
            /// 0 = Command
            /// 1 = Sensor
            /// 2 = Internal Calculation
            /// 
            /// Command: CWSSC a,b,c,d[cr]
            /// a = Water Temperature Source
            /// b = Transducer Depth Source
            /// c = Salinity Source
            /// d = Speed of Sound Source
            /// Scale: 
            /// Range:  0 = Command
            ///         1 = Sensor
            ///         2 = Internal Calculation
            /// </summary>
            public UInt16 CWSSC_TransducerDepthSrc
            {
                get { return _cWSSC_TransducerDepthSrc; }
                set
                {
                    // Verify the value is within range
                    if (value >= MIN_CWSSC && value <= MAX_CWSSC)
                    {
                        _cWSSC_TransducerDepthSrc = value;
                    }
                }
            }

            /// <summary>
            /// Water Speed of Sound Control Salinity Source.
            /// 
            /// Water Speed of Sound Control.
            /// 0 = Command
            /// 1 = Sensor
            /// 2 = Internal Calculation
            /// 
            /// Command: CWSSC a,b,c,d[cr]
            /// a = Water Temperature Source
            /// b = Transducer Depth Source
            /// c = Salinity Source
            /// d = Speed of Sound Source
            /// Scale: 
            /// Range:  0 = Command
            ///         1 = Sensor
            ///         2 = Internal Calculation
            /// </summary>
            private UInt16 _cWSSC_SalinitySrc;
            /// <summary>
            /// Water Speed of Sound Control Salinity Source.
            /// 
            /// Water Speed of Sound Control.
            /// 0 = Command
            /// 1 = Sensor
            /// 2 = Internal Calculation
            /// 
            /// Command: CWSSC a,b,c,d[cr]
            /// a = Water Temperature Source
            /// b = Transducer Depth Source
            /// c = Salinity Source
            /// d = Speed of Sound Source
            /// Scale: 
            /// Range:  0 = Command
            ///         1 = Sensor
            ///         2 = Internal Calculation
            /// </summary>
            public UInt16 CWSSC_SalinitySrc
            {
                get { return _cWSSC_SalinitySrc; }
                set
                {
                    // Verify the value is within range
                    if (value >= MIN_CWSSC && value <= MAX_CWSSC)
                    {
                        _cWSSC_SalinitySrc = value;
                    }
                }
            }

            /// <summary>
            /// Water Speed of Sound Control Speed of Sound Source.
            /// 
            /// Water Speed of Sound Control.
            /// 0 = Command
            /// 1 = Sensor
            /// 2 = Internal Calculation
            /// 
            /// Command: CWSSC a,b,c,d[cr]
            /// a = Water Temperature Source
            /// b = Transducer Depth Source
            /// c = Salinity Source
            /// d = Speed of Sound Source
            /// Scale: 
            /// Range:  0 = Command
            ///         1 = Sensor
            ///         2 = Internal Calculation
            /// </summary>
            private UInt16 _cWSSC_SpeedOfSoundSrc;
            /// <summary>
            /// Water Speed of Sound Control Speed of Sound Source.
            /// 
            /// Water Speed of Sound Control.
            /// 0 = Command
            /// 1 = Sensor
            /// 2 = Internal Calculation
            /// 
            /// Command: CWSSC a,b,c,d[cr]
            /// a = Water Temperature Source
            /// b = Transducer Depth Source
            /// c = Salinity Source
            /// d = Speed of Sound Source
            /// Scale: 
            /// Range:  0 = Command
            ///         1 = Sensor
            ///         2 = Internal Calculation
            /// </summary>
            public UInt16 CWSSC_SpeedOfSoundSrc
            {
                get { return _cWSSC_SpeedOfSoundSrc; }
                set
                {
                    // Verify the value is within range
                    if (value >= MIN_CWSSC && value <= MAX_CWSSC)
                    {
                        _cWSSC_SpeedOfSoundSrc = value;
                    }
                }
            }

            #endregion

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

            #region STIME

            /// <summary>
            /// Set the time zone for the STIME command.
            /// This will usually be the local time or GMT time.
            /// </summary>
            public AdcpTimeZone TimeZone { get; set; }

            #endregion

            #region CTRIG

            /// <summary>
            /// Selects which state the external hardware tigger
            /// needs to be before pinging.  There are 2 types of trigger logice available Edge and
            /// Level.  Edge requires the trigger line to change state before the ping occurs.  For
            /// reliable edge detection the minimum width of a pulse should be >= 50 usec.
            /// Level just needs the trigger line to be either high or low.  There is a 1.4msec
            /// delay before the ping occurs after detection of the trigger.
            /// </summary>
            public AdcpExternalTrigger CTRIG { get; set; }

            /// <summary>
            /// Return a string of the
            /// external trigger.
            /// </summary>
            /// <returns>Value for CTRIG.</returns>
            public string CTRIG_ToString()
            {
                switch(CTRIG)
                {
                    case AdcpExternalTrigger.HighLevel:
                        return "1";
                    case AdcpExternalTrigger.LowLevel:
                        return "2";
                    case AdcpExternalTrigger.LowToHigh:
                        return "3";
                    case AdcpExternalTrigger.HighToLow:
                        return "4";
                    case AdcpExternalTrigger.Disabled:
                    default:
                        return "0";
                }
            }

            #endregion

            #region CEMAC

            /// <summary>
            /// Flag to set CEMAC on or off.  
            /// 
            /// Temporarily enables Ethernet communication.  This command is
            /// typically sent via one of the serial ports.  The Ethernet port
            /// is disabled after power down or sleep.  To permanently enable the port
            /// a special factory configuration command is required.  Wehn the Ethernet
            /// port is permanently enabled the system requires an additional 2 seconds 
            /// after power up to begin accepting commands.
            /// 
            /// One of two responses occurs on the serial port after the command is accepted by the ADCP.
            /// 
            /// CEMAC+
            /// MAC 02:ff:fe:fd:fc:fb
            /// IP 192.168.1.130
            /// Link OK
            /// 
            /// OR
            /// 
            /// CEMAC+
            /// MAC 02:ff:fe:fd:fc:fb
            /// IP 192.168.1.130
            /// No Link
            /// </summary>
            public bool CEMAC { get; set; }

            #endregion

            #region IP

            /// <summary>
            /// ADCP IP address and Port.
            /// </summary>
            public AdcpEthernetOptions IP { get; set; }

            #endregion

            #region UDP

            /// <summary>
            /// UDP output mode.
            /// </summary>
            public UdpOutputMode CUDPOUT { get; set; }

            /// <summary>
            /// UDP Port.
            /// </summary>
            public uint UDPPORT 
            { 
                get 
                { 
                    return IP.Port; 
                } 
                set
                {
                    IP.Port = value;
                }
            
            }

            #endregion

            #region Serial Output

            #region 232

            /// <summary>
            /// Set the output format for RS-232 serial output.
            /// </summary>
            public AdcpOutputMode C232OUT { get; set; }

            /// <summary>
            /// Ocean Server Profile bin if C232OUT is set to a PD.
            /// </summary>
            public UInt16 C232OUT_OceanServerProfileBin { get; set; }

            /// <summary>
            /// PD0, PD3, PD4 or PD5 Coordinate Transform if C232OUT.
            /// </summary>
            public RTI.Core.Commons.Transforms C232OUT_PdCoordinateTransform { get; set; }

            #endregion

            #region 485

            /// <summary>
            /// Set the output format for RS-485 serial output.
            /// </summary>
            public AdcpOutputMode C485OUT { get; set; }

            /// <summary>
            /// Ocean Server Profile bin if C485OUT is set to a PD.
            /// </summary>
            public UInt16 C485OUT_OceanServerProfileBin { get; set; }

            /// <summary>
            /// PD0, PD3, PD4 or PD5 Coordinate Transform if C485OUT.
            /// </summary>
            public RTI.Core.Commons.Transforms C485OUT_PdCoordinateTransform { get; set; }

            #endregion

            #region 422

            /// <summary>
            /// Set the output format for RS-422 serial output.
            /// </summary>
            public AdcpOutputMode C422OUT { get; set; }

            /// <summary>
            /// Ocean Server Profile bin if C422OUT is set to a PD.
            /// </summary>
            public UInt16 C422OUT_OceanServerProfileBin { get; set; }

            /// <summary>
            /// PD0, PD3, PD4 or PD5 Coordinate Transform if C422OUT.
            /// </summary>
            public RTI.Core.Commons.Transforms C422OUT_PdCoordinateTransform { get; set; }

            #endregion

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

            #region Default

            /// <summary>
            /// Set the default values.
            /// </summary>
            public void SetDefaults()
            {
                // Set default ranges
                Mode = DEFAULT_MODE;

                // Ensemble defaults
                CEI = new TimeValue(DEFAULT_CEI_HOUR, DEFAULT_CEI_MINUTE, DEFAULT_CEI_SECOND, DEFAULT_CEI_HUNSEC);

                // Time of First ping default
                // Use the current time
                CETFP = DateTime.Now;

                CERECORD_EnsemblePing = DEFAULT_CERECORD;
                CERECORD_SinglePing = DEFAULT_CERECORD_SINGLEPING;
                
                CEOUTPUT = DEFAULT_CEOUTPUT;
                CEOUTPUT_PdCoordinateTransform = DEFAULT_PD0_TRANSFORM;
                CEOUTPUT_OceanServerProfileBin = DEFAULT_OSPROFILEBIN;

                CEPO = DEFAULT_CEPO;

                SPOS_Latitude = DEFAULT_LAT;
                SPOS_Longitude = DEFAULT_LONG;
                SPOS_WaterDepth = DEFAULT_WATER_DEPTH;
                SPOS_PsensHeight = DEFAULT_PSEN_HEIGHT;

                // Environmental defaults
                CWSSC_WaterTempSrc = DEFAULT_CWSSC_WATERTEMPSRC;
                CWSSC_TransducerDepthSrc = DEFAULT_CWSSC_XDCRDEPTHSRC;
                CWSSC_SalinitySrc = DEFAULT_CWSSC_SALINITYSRC;
                CWSSC_SpeedOfSoundSrc = DEFAULT_CWSSC_SOSSRC;
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

                C232OUT = DEFAULT_C232OUT;
                C232OUT_PdCoordinateTransform = DEFAULT_PD0_TRANSFORM;
                C232OUT_OceanServerProfileBin = DEFAULT_OSPROFILEBIN;

                C422OUT = DEFAULT_C422OUT;
                C422OUT_PdCoordinateTransform = DEFAULT_PD0_TRANSFORM;
                C422OUT_OceanServerProfileBin = DEFAULT_OSPROFILEBIN;
                
                C485OUT = DEFAULT_C485OUT;
                C485OUT_PdCoordinateTransform = DEFAULT_PD0_TRANSFORM;
                C485OUT_OceanServerProfileBin = DEFAULT_OSPROFILEBIN;

                // STIME
                TimeZone = AdcpTimeZone.LOCAL;

                // CTRIG
                CTRIG = DEFAULT_CTRIG;

                // Default IP address and port
                IP = new AdcpEthernetOptions();

                // CEMAC 
                CEMAC = DEFAULT_CEMAC;

                // UDP
                CUDPOUT = DEFAULT_CUDPOUT;
                UDPPORT = DEFAULT_UDPPORT;
            }

            #endregion

            #region Time

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

            #endregion

            #region Command List

            /// <summary>
            /// Create a list of all the commands and there value.
            /// Add all the commands to the list and there value.
            /// 
            /// Put all the values in United States English format.
            /// Other formats can use a comma instead of a decimal point for
            /// decimal numbers.
            /// </summary>
            /// <returns>List of all the commands and there value.</returns>
            public List<string> GetCommandList()
            {
                List<string> list = new List<string>();
                list.Add(CEPO_CmdStr());                                // CEPO     THIS WILL SET DEFAULTS SO IT MUST BE SET FIRST
                list.Add(Mode_CmdStr());                                // Mode
                list.Add(CEI_CmdStr());                                 // CEI
                list.Add(CETFP_CmdStr());                               // CETFP
                list.Add(CERECORD_CmdStr());                            // CERECORD  CultureInfo.CreateSpecificCulture("en-US")   Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
                list.Add(CEOUTPUT_CmdStr());                            // CEOUTPUT
                list.Add(SPOS_CmdStr());                                // SPOS

                list.Add(CWSSC_CmdStr());                               // CWSSC
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

                list.Add(C232OUT_CmdStr());                            // C232OUT
                list.Add(C485OUT_CmdStr());                            // C485OUT
                list.Add(C422OUT_CmdStr());                            // C422OUT

                list.Add(CTRIG_CmdStr());                               // CTRIG

                list.Add(IP_CmdStr());                                  // IP

                // UDP
                if (CUDPOUT != UdpOutputMode.Disable)
                {
                    list.Add(CUDPOUT_CmdStr());                         // CUDPOUT
                    list.Add(UDPPORT_CmdStr());                         // UDPPORT
                }

                if (TimeZone == AdcpTimeZone.LOCAL)
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
            /// <returns>List of all the commands and there value.</returns>
            public List<string> GetDeploymentCommandList()
            {
                List<string> list = new List<string>();

                list.Add(CEPO_CmdStr());                            // CEPO
                list.Add(CEOUTPUT_CmdStr());                        // CEOUTPUT
                //list.Add(SPOS_CmdStr());                            // SPOS
                list.Add(CWS_CmdStr());                             // CWS
                list.Add(CWT_CmdStr());                             // CWT
                list.Add(CHO_CmdStr());                             // CHO
                list.Add(CEI_CmdStr());                             // CEI
                list.Add(CETFP_CmdStr());                           // CETFP
                list.Add(CERECORD_CmdStr());                        // CERECORD
                list.Add(CTRIG_CmdStr());                           // CTRIG

                // UDP
                if (CUDPOUT != UdpOutputMode.Disable)
                {
                    list.Add(CUDPOUT_CmdStr());                     // CUDPOUT
                    list.Add(UDPPORT_CmdStr());                     // UDPPORT
                }

                if (TimeZone == AdcpTimeZone.LOCAL)
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
            /// A shorten list of commands that are neccessary for a DVL deployment.
            /// Create a list of all the DVL deployment commands and there value.
            /// </summary>
            /// <returns>List of all the commands and there value for a DVL deployment.</returns>
            public List<string> GetDvlCommandList()
            {
                List<string> list = new List<string>();

                list.Add(CEPO_CmdStr());                            // CEPO
                //list.Add(Mode_CmdStr());                            // Mode
                list.Add(CMD_CDVL);                                 // Force DVL mode
                list.Add(CEOUTPUT_CmdStr());                        // CEOUTPUT
                list.Add(CEI_CmdStr());                             // CEI
                list.Add(CERECORD_CmdStr());                        // CERECORD
                list.Add(C232OUT_CmdStr());                         // C232OUT
                list.Add(C485OUT_CmdStr());                         // C485OUT
                list.Add(C422OUT_CmdStr());                         // C422OUT
                list.Add(C232B_CmdStr());                           // C232B
                list.Add(C485B_CmdStr());                           // C485B
                list.Add(C422B_CmdStr());                           // C422B
                list.Add(CTRIG_CmdStr());                           // CTRIG

                // UDP
                if (CUDPOUT != UdpOutputMode.Disable)
                {
                    // Turn on Ethernet
                    if (!CEMAC)
                    {
                        list.Add(CEMAC_CmdStr());                   // Enable CEMAC
                        list.Add(IP_CmdStr());                      // IP 
                    }


                    list.Add(CUDPOUT_CmdStr());                     // CUDPOUT
                    list.Add(UDPPORT_CmdStr());                     // UDPPORT
                }

                return list;
            }

            /// <summary>
            /// A shorten list of commands that are neccessary for a Waves deployment.
            /// Create a list of all the Waves deployment commands and there value.
            /// </summary>
            /// <returns>List of all the commands and there value for a Waves deployment.</returns>
            public List<string> GetWavesCommandList()
            {
                List<string> list = new List<string>();
                list.Add(CEPO_CmdStr());                                // CEPO     THIS WILL SET DEFAULTS SO IT MUST BE SET FIRST
                list.Add(CMD_CPROFILE);                                 // Force Profile mode
                list.Add(CEI_CmdStr());                                 // CEI
                list.Add(CETFP_CmdStr());                               // CETFP
                list.Add(CERECORD_CmdStr());                            // CERECORD  CultureInfo.CreateSpecificCulture("en-US")   Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
                list.Add(CEOUTPUT_CmdStr());                            // CEOUTPUT
                list.Add(SPOS_CmdStr());                                // SPOS

                //list.Add(CWSSC_CmdStr());                               // CWSSC
                list.Add(CWS_CmdStr());                                 // CWS
                list.Add(CWT_CmdStr());                                 // CWT
                //list.Add(CTD_CmdStr());                                 // CTD
                //list.Add(CWSS_CmdStr());                                // CWSS
                //list.Add(CHS_CmdStr());                                 // CHS
                //list.Add(CHO_CmdStr());                                 // CHO
                //list.Add(CVSF_CmdStr());                                // CVSF

                //list.Add(C232B_CmdStr());                               // C232B
                //list.Add(C485B_CmdStr());                               // C485B
                //list.Add(C422B_CmdStr());                               // C422B

                //list.Add(C232OUT_CmdStr());                            // C232OUT
                //list.Add(C485OUT_CmdStr());                            // C485OUT
                //list.Add(C422OUT_CmdStr());                            // C422OUT

                //list.Add(CTRIG_CmdStr());                               // CTRIG

                //list.Add(IP_CmdStr());                                  // IP

                //// UDP
                //if (CUDPOUT != UdpOutputMode.Disable)
                //{
                //    list.Add(CUDPOUT_CmdStr());                         // CUDPOUT
                //    list.Add(UDPPORT_CmdStr());                         // UDPPORT
                //}

                //if (TimeZone == AdcpTimeZone.LOCAL)
                //{
                //    list.Add(LocalTime_CmdStr());                       // Local Time         SET THE TIME LAST, IT TAKES THE LONGEST TO SET
                //}
                //else
                //{
                //    list.Add(GmtTime_CmdStr());                         // GMT Time           SET THE TIME LAST, IT TAKES THE LONGEST TO SET
                //}

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

                if (TimeZone == AdcpTimeZone.LOCAL)
                {
                    sb.AppendLine(LocalTime_CmdStr());             // Local DateTime
                }
                else
                {
                    sb.AppendLine(GmtTime_CmdStr());               // Gmt DateTime
                }

                sb.AppendLine(Mode_ToString());                     // Mode
                sb.AppendLine(CEI_CmdStr());                        // CEI
                sb.AppendLine(CETFP_CmdStr());                      // CETFP
                sb.AppendLine(CERECORD_CmdStr());                   // CERECORD
                sb.AppendLine(CEOUTPUT_CmdStr());                   // CEOUTPUT
                sb.AppendLine(CEPO_CmdStr());                       // CEPO
                sb.AppendLine(SPOS_CmdStr());                       // SPOS

                sb.AppendLine(CWSSC_CmdStr());                      // CWSSC
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

                sb.AppendLine(C232OUT_CmdStr());                    // C232OUT
                sb.AppendLine(C485OUT_CmdStr());                    // C485OUT
                sb.AppendLine(C422OUT_CmdStr());                    // C422OUT

                sb.AppendLine(CTRIG_CmdStr());                      // CTRIG

                sb.AppendLine(IP_CmdStr());                         // IP

                sb.AppendLine(CUDPOUT_CmdStr());                    // CUDPOUT
                sb.AppendLine(UDPPORT_CmdStr());                    // UDPPORT

                return sb.ToString();
            }

            #endregion

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
                OutputList.Add(AdcpCommands.AdcpOutputMode.NMEA_Status);
                OutputList.Add(AdcpCommands.AdcpOutputMode.OceanServer);
                OutputList.Add(AdcpCommands.AdcpOutputMode.DVL_PRTI03);
                OutputList.Add(AdcpCommands.AdcpOutputMode.PD0);
                OutputList.Add(AdcpCommands.AdcpOutputMode.PD13);
                OutputList.Add(AdcpCommands.AdcpOutputMode.PD4);
                OutputList.Add(AdcpCommands.AdcpOutputMode.PD5);
                OutputList.Add(AdcpCommands.AdcpOutputMode.PD6);

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
                switch(CEOUTPUT)
                {
                    case AdcpOutputMode.PD0:
                    case AdcpOutputMode.PD3:
                    case AdcpOutputMode.PD4:
                    case AdcpOutputMode.PD5:
                        return string.Format("{0} {1},{2}", CMD_CEOUTPUT, ((int)CEOUTPUT).ToString(CultureInfo.CreateSpecificCulture("en-US")), ((int)CEOUTPUT_PdCoordinateTransform).ToString(CultureInfo.CreateSpecificCulture("en-US")));
                    case AdcpOutputMode.OceanServer:
                        return string.Format("{0} {1},{2}", CMD_CEOUTPUT, ((int)CEOUTPUT).ToString(CultureInfo.CreateSpecificCulture("en-US")), ((int)CEOUTPUT_OceanServerProfileBin).ToString(CultureInfo.CreateSpecificCulture("en-US")));
                    default:
                        return string.Format("{0} {1}", CMD_CEOUTPUT, ((int)CEOUTPUT).ToString(CultureInfo.CreateSpecificCulture("en-US")));
                }
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
            /// Return the CWSSC command string.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWSSC_CmdStr()
            {
                return String.Format("{0} {1},{2},{3},{4}", CMD_CWSSC,
                                                            CWSSC_WaterTempSrc.ToString(CultureInfo.CreateSpecificCulture("en-US")),
                                                            CWSSC_TransducerDepthSrc.ToString(CultureInfo.CreateSpecificCulture("en-US")),
                                                            CWSSC_SalinitySrc.ToString(CultureInfo.CreateSpecificCulture("en-US")),
                                                            CWSSC_SpeedOfSoundSrc.ToString(CultureInfo.CreateSpecificCulture("en-US")));
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

            #region C232OUT

            /// <summary>
            /// Return the C232OUT command string.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string C232OUT_CmdStr()
            {
                switch (C232OUT)
                {
                    case AdcpOutputMode.PD0:
                    case AdcpOutputMode.PD3:
                    case AdcpOutputMode.PD4:
                    case AdcpOutputMode.PD5:
                        return string.Format("{0} {1},{2}", CMD_C232OUT, ((int)C232OUT).ToString(CultureInfo.CreateSpecificCulture("en-US")), ((int)C232OUT_PdCoordinateTransform).ToString(CultureInfo.CreateSpecificCulture("en-US")));
                    case AdcpOutputMode.OceanServer:
                        return string.Format("{0} {1},{2}", CMD_C232OUT, ((int)C232OUT).ToString(CultureInfo.CreateSpecificCulture("en-US")), ((int)C232OUT_OceanServerProfileBin).ToString(CultureInfo.CreateSpecificCulture("en-US")));
                    default:
                        return string.Format("{0} {1}", CMD_C232OUT, ((int)C232OUT).ToString(CultureInfo.CreateSpecificCulture("en-US")));
                }
            }

            #endregion

            #region C485OUT

            /// <summary>
            /// Return the C485OUT command string.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string C485OUT_CmdStr()
            {
                switch (C485OUT)
                {
                    case AdcpOutputMode.PD0:
                    case AdcpOutputMode.PD3:
                    case AdcpOutputMode.PD4:
                    case AdcpOutputMode.PD5:
                        return string.Format("{0} {1},{2}", CMD_C485OUT, ((int)C485OUT).ToString(CultureInfo.CreateSpecificCulture("en-US")), ((int)C485OUT_PdCoordinateTransform).ToString(CultureInfo.CreateSpecificCulture("en-US")));
                    case AdcpOutputMode.OceanServer:
                        return string.Format("{0} {1},{2}", CMD_C485OUT, ((int)C485OUT).ToString(CultureInfo.CreateSpecificCulture("en-US")), ((int)C485OUT_OceanServerProfileBin).ToString(CultureInfo.CreateSpecificCulture("en-US")));
                    default:
                        return string.Format("{0} {1}", CMD_C485OUT, ((int)C485OUT).ToString(CultureInfo.CreateSpecificCulture("en-US")));
                }
            }

            #endregion

            #region C422OUT

            /// <summary>
            /// Return the C422OUT command string.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string C422OUT_CmdStr()
            {
                switch (C422OUT)
                {
                    case AdcpOutputMode.PD0:
                    case AdcpOutputMode.PD3:
                    case AdcpOutputMode.PD4:
                    case AdcpOutputMode.PD5:
                        return string.Format("{0} {1},{2}", CMD_C422OUT, ((int)C422OUT).ToString(CultureInfo.CreateSpecificCulture("en-US")), ((int)C422OUT_PdCoordinateTransform).ToString(CultureInfo.CreateSpecificCulture("en-US")));
                    case AdcpOutputMode.OceanServer:
                        return string.Format("{0} {1},{2}", CMD_C422OUT, ((int)C422OUT).ToString(CultureInfo.CreateSpecificCulture("en-US")), ((int)C422OUT_OceanServerProfileBin).ToString(CultureInfo.CreateSpecificCulture("en-US")));
                    default:
                        return string.Format("{0} {1}", CMD_C422OUT, ((int)C422OUT).ToString(CultureInfo.CreateSpecificCulture("en-US")));
                }
            }

            #endregion

            #region CTRIG

            /// <summary>
            /// return the CTRIG command string. 
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CTRIG_CmdStr()
            {
                return String.Format("{0} {1}", CMD_CTRIG, CTRIG_ToString());
            }

            #endregion

            #region Ethernet

            /// <summary>
            /// Return the CEMAC command string.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CEMAC_CmdStr()
            {
                return string.Format("{0}", CMD_CEMAC);
            }

            /// <summary>
            /// Return the IP command string.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string IP_CmdStr()
            {
                return string.Format("{0} {1}", CMD_IP, IP.IpAddr);
            }

            /// <summary>
            /// Return the CUDPOUT command string.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CUDPOUT_CmdStr()
            {
                return string.Format("{0} {1}", CMD_CUDPOUT, ((int)CUDPOUT).ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            /// <summary>
            /// Return the UDPPORT command string.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string UDPPORT_CmdStr()
            {
                return string.Format("{0} {1}", CMD_UDPPORT, ((int)UDPPORT).ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            #endregion

            #endregion

            #region Command Desc

            #region CEOUTPUT

            /// <summary>
            /// The CEOUTPUT description string.
            /// </summary>
            /// <returns>Description of the command.</returns>
            public static string GetCeoutputDesc()
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("CEOUTPUT n<CR>");
                sb.AppendLine("CEOUTPUT 4, b<CR> ");
                sb.AppendLine("CEOUTPUT 100, c<CR> ");

                sb.AppendLine(BaseOutputDesc());

                return sb.ToString();
            }

            /// <summary>
            /// Base ensemble Output type description
            /// to be shared with all the output commands.
            /// </summary>
            /// <returns></returns>
            public static string BaseOutputDesc()
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("Ensemble output type.");
                sb.AppendLine();
                sb.AppendLine("n = 0     Disable:      Disables serial output. Saves battery energy when recording data to the SD card during a self-contained deployment by reducing extra on time ");
                sb.AppendLine("                             of the system due to data transfer.");
                sb.AppendLine("n = 1     Binary:       Enables the standard binary output data protocol to be sent out the serial port when the system is in “profile” mode. If the system is in “DVL” ");
                sb.AppendLine("                             mode the $PRTI01 $PRTI02 $PRTI30 $PRTI31 data strings are output.");
                sb.AppendLine("n = 2     ASCII:        Enables an ASCII text serial output that is dumb terminal compatible to be sent out the serial port when the system is in “profile” mode. If the ");
                sb.AppendLine("                             system is in “DVL” mode the $PRTI01 $PRTI02 $PRTI32 $PRTI33 data strings are output.");
                sb.AppendLine("n = 3     NMEA_Status:  Disables all output except for a NMEA status string. Allows the user to verify that the instrument is operating normally while recording data to the ");
                sb.AppendLine("                             internal recorder. Saves power and can improve ping timing.");
                sb.AppendLine("n = 4     OceanServer:  Enable the special Ocean Server NMEA DVL data output. When CEOUTPUT 4 is selected a second parameter can be sent to select the navigation bin. ");
                sb.AppendLine("  b = profile bin that will be used in the $DVLNAV string.");
                sb.AppendLine("n = 5     DVL_PRTI03:   If the system is in “DVL” mode the $PRTI03 data string is output.");
                sb.AppendLine("n = 100   PD0:          Selects PD0 binary output. When CEOUTPUT 100 is selected a second parameter can be sent to select the velocity coordinate system. ");
                sb.AppendLine("  c = 0 is beam coordinates");
                sb.AppendLine("  c = 1 is instrument (XYZ) ");
                sb.AppendLine("  c = 2 is Earth (ENU) ");
                sb.AppendLine("  c = 3 is Ship (SFM).");
                sb.AppendLine("n = 113   PD13:        Selects PD13 ASCII output.");
                sb.AppendLine("n = 103   PD3:         Selects PD3 binary output. When CEOUTPUT 103 is selected a second parameter can be sent to select the velocity coordinate system. See n = 100.");
                sb.AppendLine("n = 104   PD4:         Selects PD4 binary output. When CEOUTPUT 104 is selected a second parameter can be sent to select the velocity coordinate system. See n = 100.");
                sb.AppendLine("n = 105   PD5:         Selects PD5 binary output. When CEOUTPUT 105 is selected a second parameter can be sent to select the velocity coordinate system. See n = 100.");
                sb.AppendLine("n = 106   PD6:         Selects PD6 binary output.");
                sb.AppendLine();
                sb.AppendLine("Important Note: PD output formats are industry standard formats for DVLs and are typically a string of data output that is a subset of the total data available. Below ");
                sb.AppendLine("is a summary of the data available in PD specific formats:");
                sb.AppendLine("PD0  - binary output format that includes a header, fixed and variable leader, bottom track, and water profile information. The fixed and variable leader is a recording ");
                sb.AppendLine("       of time, DVL setup, orientation, heading, pitch, roll, temperature, pressure and self test diagnostic results. The user can select data fields to be output. In the ");
                sb.AppendLine("       case with RTI instruments users can select the coordinates for the data to be represented.");
                sb.AppendLine("PD4  - is a binary output format that presents bottom track speed over bottom, speed through water and range to bottom information only.");
                sb.AppendLine("PD5  – is a superset of PD4 and includes additional information such as, salinity, depth, pitch, roll, heading, and distance made good.");
                sb.AppendLine("PD6  – is a text-based output format that groups separate sentences containing system attitude data, timing and scaling and speed through water relative to the instrument, ");
                sb.AppendLine("       vehicle, and earth coordinates. Each data sentence contains a unique starting delimiter and comma delimited fields.");
                sb.AppendLine("PD13 - is a text output format, like PD6 with the addition of information about range to bottom and raw pressure sensor data.");

                return sb.ToString();
            }

            #endregion

            #region CERECORD

            /// <summary>
            /// CERECORD description.
            /// </summary>
            /// <returns>Description string.</returns>
            public static string GetCerecordDesc()
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("CERECORD n,m<CR>");
                sb.AppendLine("n = Ensemble Recording");
                sb.AppendLine("  0 = disable");
                sb.AppendLine("  1 = enable. ");
                sb.AppendLine("m = SinglePing Recording ");
                sb.AppendLine("  0 = disable");
                sb.AppendLine("  1 = enable.");
                sb.AppendLine();
                sb.AppendLine("When ensemble recording is enabled and the ADCP is started (START<CR>) the firmware searches for the next available file number to record to on the SD card. ");
                sb.AppendLine("   The ensemble file name starts with the letter “A” followed by a 7 digit number and ending with the extension “.ens”. For example: The first ensemble file will be ");
                sb.AppendLine("   named “A0000001.ens”. During deployment as each ensemble is completed the data is appended to the current file. The 7 digit number following the “A” is incremented ");
                sb.AppendLine("   each time the system is (re)started or when the file size exceeds 16Mbytes bytes.");
                sb.AppendLine("Note: Internal ensemble data recording during burst sampling only occurs at the end of the burst.");
                sb.AppendLine();
                sb.AppendLine("When single recording is enabled and the ADCP is started (START<CR>) the firmware searches for the next available file number to record to on the SD card. The single ");
                sb.AppendLine("   ping file name starts with the letter “S” followed by a 7 digit number and ending with the extension “.ens”. For example: The first single ping file will be named ");
                sb.AppendLine("   “S0000001.ens”. During deployment as each ping is completed the data is appended to the current file. The 7 digit number following the “S” is incremented each time ");
                sb.AppendLine("   the system is (re)started or when the file size exceeds 16Mbytes bytes. Each ping, whether bottom track or profile, is considered to be a single ping.");
                sb.AppendLine("Note: No error/ threshold screening or coordinate transformation is performed on the data contained in a single ping file.");

                return sb.ToString();
            }

            #endregion

            #region CEI

            /// <summary>
            /// CEI description string.
            /// </summary>
            /// <returns>Description string.</returns>
            public static string GetCeiDesc()
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("CEI HH:MM:SS.hh<CR>");
                sb.AppendLine("Ensemble Interval. Sets the time interval that system will output the averaged profile/bottom track data.");
                sb.AppendLine();
                sb.AppendLine("HH = Hour");
                sb.AppendLine("MM = Minute");
                sb.AppendLine("SS = Second");
                sb.AppendLine("hh = Hundredth of a second");
                sb.AppendLine("");
                sb.AppendLine("Note: all digits including the space following CEI and the separators must be part of the command or the system will reject it.");

                return sb.ToString();
            }

            #endregion

            #region CTRIG

            /// <summary>
            /// CTRIG description string.
            /// </summary>
            /// <returns>Description string.</returns>
            public static string GetCtrigDesc()
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("CTRIG n<CR> ");
                sb.AppendLine("External Trigger. Selects which state the external hardware trigger needs to be before pinging. There are 2 types of trigger logic ");
                sb.AppendLine("available Edge and Level. Edge requires the trigger line to change state before the ping occurs. For reliable edge detection the minimum ");
                sb.AppendLine("width of a pulse should be >= 50 usec. Level just needs the trigger line to be either high or low. There is a 1.4 msec delay before the ");
                sb.AppendLine("ping occurs after detection of the trigger.");
                sb.AppendLine();
                sb.AppendLine("n = 1 High level");
                sb.AppendLine("n = 2 Low level");
                sb.AppendLine("n = 3 Low to high");
                sb.AppendLine("n = 4 High to low");
                sb.AppendLine("n = 0 default disabled.");
                sb.AppendLine();
                sb.AppendLine("NOTE: The Low Power Regulator Board J7 Pins 2 and 3 on must be jumpered to enable the input trigger. The input trigger line has minimal protection ");
                sb.AppendLine("so you need to be careful to not exceed +5.3 and -0.3 Vdc. The threshold for logic high is 3.34 Vdc.");

                return sb.ToString();
            }

            #endregion

            #region C232B, C422B and C485B

            /// <summary>
            /// The C232B, C422B and C485B description string.
            /// </summary>
            /// <returns>Description of the command.</returns>
            public static string GetBaudDesc()
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("C232B n<CR> RS232 bps.");
                sb.AppendLine("C485B n<CR> RS485 bps.");
                sb.AppendLine("C422B n<CR> RS422 bps.");
                sb.AppendLine("Serial Baud rates.");
                sb.AppendLine("n = 2400, 4800, 9600, 19200, 38400, 57600, 115200, 230400, 460800, 921600.");

                return sb.ToString();
            }

            #endregion

            #region C232OUT, C422OUT, C485OUT

            /// <summary>
            /// The C232OUT description string.
            /// </summary>
            /// <returns>Description of the command.</returns>
            public static string GetC232outDesc()
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("C232OUT n<CR>");
                sb.AppendLine("C232OUT n, b<CR> ");
                sb.AppendLine("C232OUT n, c<CR> ");

                sb.AppendLine(BaseOutputDesc());

                return sb.ToString();
            }

            /// <summary>
            /// The C422OUT description string.
            /// </summary>
            /// <returns>Description of the command.</returns>
            public static string GetC422outDesc()
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("C422OUT n<CR>");
                sb.AppendLine("C422OUT n, b<CR> ");
                sb.AppendLine("C422OUT n, c<CR> ");

                sb.AppendLine(BaseOutputDesc());

                return sb.ToString();
            }

            /// <summary>
            /// The C4885OUT description string.
            /// </summary>
            /// <returns>Description of the command.</returns>
            public static string GetC485outDesc()
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("C485OUT n<CR>");
                sb.AppendLine("C485OUT n, b<CR> ");
                sb.AppendLine("C485OUT n, c<CR> ");

                sb.AppendLine(BaseOutputDesc());

                return sb.ToString();
            }

            #endregion

            #region UDPPORT

            /// <summary>
            /// UDPPORT description string.
            /// </summary>
            /// <returns>Description string.</returns>
            public static string GetUdpportDesc()
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("UDPPORT n<CR> ");
                sb.AppendLine("Sets the port number to which the system will send UDP data. ");
                sb.AppendLine("The default port is 257.");

                return sb.ToString();
            }

            #endregion

            #region CUDPOUT

            /// <summary>
            /// CUDPOUT description string.
            /// </summary>
            /// <returns>Description string.</returns>
            public static string GetCudpoutDesc()
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("CUDPOUT n<CR>");
                sb.AppendLine("Ensemble output type on UDP port.");
                sb.AppendLine();
                sb.AppendLine("n = 0     Disable:      Disables serial output. Saves battery energy when recording data to the SD card during a self-contained deployment by reducing extra on time ");
                sb.AppendLine("                             of the system due to data transfer.");
                sb.AppendLine("n = 1     Binary:       Enables the standard binary output data protocol to be sent out the serial port when the system is in “profile” mode. If the system is in “DVL” ");
                sb.AppendLine("                             mode the $PRTI01 $PRTI02 $PRTI30 $PRTI31 data strings are output.");
                sb.AppendLine("n = 2     ASCII:        Enables an ASCII text serial output that is dumb terminal compatible to be sent out the serial port when the system is in “profile” mode. If the ");
                sb.AppendLine("                             system is in “DVL” mode the $PRTI01 $PRTI02 $PRTI32 $PRTI33 data strings are output.");
                sb.AppendLine("n = 113   PD13:         Selects PD13 ASCII output.");
                sb.AppendLine("n = 106   PD6:          Selects PD6 binary output.");
                sb.AppendLine();
                sb.AppendLine("Important Note: PD output formats are industry standard formats for DVLs and are typically a string of data output that is a subset of the total data available. Below ");
                sb.AppendLine("is a summary of the data available in PD specific formats:");
                sb.AppendLine("PD6  – is a text-based output format that groups separate sentences containing system attitude data, timing and scaling and speed through water relative to the instrument, ");
                sb.AppendLine("       vehicle, and earth coordinates. Each data sentence contains a unique starting delimiter and comma delimited fields.");
                sb.AppendLine("PD13 - is a text output format, like PD6 with the addition of information about range to bottom and raw pressure sensor data.");

                return sb.ToString();
            }

            #endregion

            #region CEMAC

            /// <summary>
            /// CEMAC description string.
            /// </summary>
            /// <returns>Description string.</returns>
            public static string GetCemacDesc()
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("CEMAC<CR> ");
                sb.AppendLine("Temporarily enables Ethernet communication. ");
                sb.AppendLine("This command is typically sent via one of the serial ports. The Ethernet port is disabled after power down or sleep. To permanently ");
                sb.AppendLine("enable the port a special factory configuration command is required. When the Ethernet port is permanently enabled the system requires ");
                sb.AppendLine("an additional 2 seconds after power up to begin accepting commands.");
                sb.AppendLine();
                sb.AppendLine("One of two responses occurs on the serial port after the command is accepted by the ADCP.");
                sb.AppendLine();
                sb.AppendLine("CEMAC+");
                sb.AppendLine("MAC 02:ff:fe:fd:fc:fb");
                sb.AppendLine("IP192.168.1.130");
                sb.AppendLine("Link OK");
                sb.AppendLine("Speed 1, FullDuplex 1");
                sb.AppendLine();
                sb.AppendLine("OR");
                sb.AppendLine();
                sb.AppendLine("CEMAC+");
                sb.AppendLine("MAC 02:ff:fe:fd:fc:fb");
                sb.AppendLine("IP 192.168.1.130");
                sb.AppendLine("No Link");

                return sb.ToString();
            }

            #endregion

            #region IP

            /// <summary>
            /// IP description string.
            /// </summary>
            /// <returns>Description string.</returns>
            public static string GetIpDesc()
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("IP n.n.n.n<CR>"); 
                sb.AppendLine("Sets a new Ethernet address in the ADCP. The ADCP response to the IP command will be the same as CEMAC. When sending IP<CR> ");
                sb.AppendLine("with no address the system will return the current MAC and IP address.");
                sb.AppendLine();
                sb.AppendLine("IP+");
                sb.AppendLine("MAC 02:ff:fe:fd:fc:fb");
                sb.AppendLine("IP 192.168.1.130");

                return sb.ToString();
            }

            #endregion

            #region SPOS

            /// <summary>
            /// SPOS description string.
            /// </summary>
            /// <returns>Description string.</returns>
            public static string GetSposDesc()
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("SPOS Lat,Lon,Depth,PsenHt<CR>");
                sb.AppendLine("Set system geo position and water depth");
                sb.AppendLine("");
                sb.AppendLine("Lat = Latitude in Degrees.");
                sb.AppendLine("Lon = Longitude in Degrees.");
                sb.AppendLine("Depth = Depth of the transducer in meters.");
                sb.AppendLine("PsenHt = Pressure Sensor height in meters.");


                return sb.ToString();
            }

            #endregion

            #region CWT

            /// <summary>
            /// CWT description string.
            /// </summary>
            /// <returns>Description string.</returns>
            public static string GetCwtDesc()
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("CWT n.nn<CR>");
                sb.AppendLine("Water Temperature (degrees).");
                sb.AppendLine("");
                sb.AppendLine("n.nn = Water Temperature in degrees farenheit.");
                sb.AppendLine("");
                sb.AppendLine("Used in the water speed of sound calculation if the temperature sensor is not available.");

                return sb.ToString();
            }

            #endregion

            #region CWS

            /// <summary>
            /// CWS description string.
            /// </summary>
            /// <returns>Description string.</returns>
            public static string GetCwsDesc()
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("CWS n.nn<CR>");
                sb.AppendLine("Water Salinity (ppt). ");
                sb.AppendLine("");
                sb.AppendLine("n.nn = Salinity in parts per thousand (ppt).");
                sb.AppendLine("");
                sb.AppendLine("Used in the water speed of sound calculation if the temperature sensor is not available.");

                return sb.ToString();
            }

            #endregion

            #region CETFP

            /// <summary>
            /// CETFP description string.
            /// </summary>
            /// <returns>Description string.</returns>
            public static string GetCetfpDesc()
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("CETFP YYYY/MM/DD,HH:mm:SS.hh<CR> ");
                sb.AppendLine("Ensemble Time of First Ping.");
                sb.AppendLine("Sets the time that the system will awaken and start pinging.");
                sb.AppendLine("");
                sb.AppendLine("YY = Year");
                sb.AppendLine("MM = Month");
                sb.AppendLine("DD = Day");
                sb.AppendLine("HH = Hour");
                sb.AppendLine("mm = Minute");
                sb.AppendLine("SS = Second");
                sb.AppendLine("hh = Hundredth of a second");
                sb.AppendLine("");
                sb.AppendLine("Note: all digits including the space following CETFP and the ");
                sb.AppendLine("separators must be part of the command or the system will reject the command.");

                return sb.ToString();
            }

            #endregion

            #region DIAGPRESSURE

            /// <summary>
            /// DIAGPRESSURE description string.
            /// </summary>
            /// <returns>Description string.</returns>
            public static string GetDiagPressureDesc()
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("DIAGPRESSURE<CR> ");
                sb.AppendLine("Rub Beam Test");
                sb.AppendLine("Test the functionality of pressure sensor.");
                sb.AppendLine("");
                sb.AppendLine("Shows the Keller30 pressure sensor info. The first part of the response ");
                sb.AppendLine("shows the serial number and the version of firmware. The next portion ");
                sb.AppendLine("of the response contains the pressure sensor “Coefficients”. The third ");
                sb.AppendLine("line of which indicates the maximum pressure the sensor can measure. The ");
                sb.AppendLine("last portion of the response contains the pressure sensor “Configuration”. ");
                sb.AppendLine("These values can be programmed using Keller software when the pressure ");
                sb.AppendLine("sensor is disconnected from the ADCP.");

                return sb.ToString();
            }

            #endregion

            #region DIAGRUB

            /// <summary>
            /// DIAGRUB description string.
            /// </summary>
            /// <returns>Description string.</returns>
            public static string GetDiagRubDesc()
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("DIAGRUB<CR> ");
                sb.AppendLine("Rub Beam Test");
                sb.AppendLine("Test the functionality of each beam.");
                sb.AppendLine("");
                sb.AppendLine("Shows the results of the beam continuity");
                sb.AppendLine("test where the user rubs each beam in ");
                sb.AppendLine("sequence to determine whether the transducer ");
                sb.AppendLine("cup is functional. The test collects statistics ");
                sb.AppendLine("for 10 samples then prompts the user to rub ");
                sb.AppendLine("the selected beam. PASS will be displayed when ");
                sb.AppendLine("the test detects the correct amplitude change ");
                sb.AppendLine("associated with rubbing a transducer. Up to 4 beams ");
                sb.AppendLine("will be tested per test. For systems with additional ");
                sb.AppendLine("beams use the CEPO command to select additional sub systems.");

                return sb.ToString();
            }

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

                    // Hardware
                    if (lines[x].Contains("DP") || lines[x].Contains("SC"))
                    {
                        hw = lines[x];
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

                // Store the string result
                listing.StrResult = buffer;

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

            #region ENGPORT

            /// <summary>
            /// Decode the ENGPORT command.  This will give
            /// the serial port used.
            /// 
            /// ENGPORT
            /// 
            /// ENGPORT
            /// 
            /// </summary>
            /// <param name="buffer">Buffer containing the command response.</param>
            /// <returns>ENGPORT response.</returns>
            public static string DecodeENGPORT(string buffer)
            {
                // Seperate each value
                string[] values = buffer.Split('\r');

                if(values.Length >= 3)
                {
                    return values[1].Trim();
                }

                return buffer;
            }

            #endregion

            #region DIAGSAMP

            /// <summary>
            /// DP600
            /// AutoGain:
            ///           bm0      bm1      bm2      bm3
            /// amp     39.05,   36.00,   40.52,   38.04
            /// Hz    1984.41,  192.05,  831.30, 1180.32
            /// cor      0.07,    0.01,    0.06,    0.02
            /// Heading    240.90 deg
            /// Pitch       -1.05 deg
            /// Roll       179.71 deg
            /// Water       22.91 deg
            /// BackPlane   24.83 deg
            /// Battery     19.62 V
            /// Boost+      22.69 V
            /// Boost-     -21.95 V
            /// </summary>
            /// <param name="buffer">Decode the buffer.</param>
            public static DiagSamp DecodeDiagSamp(string buffer)
            {
                // Initialize a sample
                DiagSamp samp = new DiagSamp();

                if(string.IsNullOrEmpty(buffer))
                {
                    return samp;
                }

                // Set the buffer
                samp.DiagSampStr = buffer;

                // Break up the lines
                char[] newLines = new char[] { '\r', '\n' };
                char[] spaces = new char[] { ' ' };
                char[] spacesComma = new char[] { ' ', ',' };
                string[] lines = buffer.Split(newLines, StringSplitOptions.RemoveEmptyEntries);

                // Decode each line of data
                for (int x = 0; x < lines.Length; x++)
                {
                    // System type
                    if(lines[x].Contains("DP") || lines[x].Contains("SC"))
                    {
                        samp.SystemType = lines[x].Trim();
                    }

                    // Amp
                    if(lines[x].Contains("amp"))
                    {
                        var amp = lines[x].Split(spacesComma, StringSplitOptions.RemoveEmptyEntries);
                        if(amp.Length >= 2)
                        {
                            for(int y = 1; y < amp.Length; y++)
                            {
                                double.TryParse(amp[y], out samp.Amp[y - 1]);
                            }
                        }
                    }

                    // Hz
                    if (lines[x].Contains("Hz"))
                    {
                        var hz = lines[x].Split(spacesComma, StringSplitOptions.RemoveEmptyEntries);
                        if (hz.Length >= 2)
                        {
                            for (int y = 1; y < hz.Length; y++)
                            {
                                double.TryParse(hz[y], out samp.Hz[y - 1]);
                            }
                        }
                    }

                    // Cor
                    if (lines[x].Contains("cor"))
                    {
                        var cor = lines[x].Split(spacesComma, StringSplitOptions.RemoveEmptyEntries);
                        if (cor.Length >= 2)
                        {
                            for (int y = 1; y < cor.Length; y++)
                            {
                                double.TryParse(cor[y], out samp.Cor[y - 1]);
                            }
                        }
                    }

                    // Heading
                    if(lines[x].Contains("Heading"))
                    {
                        var val = lines[x].Split(spaces, StringSplitOptions.RemoveEmptyEntries);
                        if(val.Length >= 3)
                        {
                            double result = 0.0;
                            if(double.TryParse(val[1], out result))
                            {
                                samp.Heading = result;
                            }
                        }
                    }

                    // Pitch
                    if (lines[x].Contains("Pitch"))
                    {
                        var val = lines[x].Split(spaces, StringSplitOptions.RemoveEmptyEntries);
                        if (val.Length >= 3)
                        {
                            double result = 0.0;
                            if (double.TryParse(val[1], out result))
                            {
                                samp.Pitch = result;
                            }
                        }
                    }

                    // Roll
                    if (lines[x].Contains("Roll"))
                    {
                        var val = lines[x].Split(spaces, StringSplitOptions.RemoveEmptyEntries);
                        if (val.Length >= 3)
                        {
                            double result = 0.0;
                            if (double.TryParse(val[1], out result))
                            {
                                samp.Roll = result;
                            }
                        }
                    }

                    // Water
                    if (lines[x].Contains("Water"))
                    {
                        var val = lines[x].Split(spaces, StringSplitOptions.RemoveEmptyEntries);
                        if (val.Length >= 3)
                        {
                            double result = 0.0;
                            if (double.TryParse(val[1], out result))
                            {
                                samp.WaterTemp = result;
                            }
                        }
                    }

                    // BackPlane
                    if (lines[x].Contains("BackPlane"))
                    {
                        var val = lines[x].Split(spaces, StringSplitOptions.RemoveEmptyEntries);
                        if (val.Length >= 3)
                        {
                            double result = 0.0;
                            if (double.TryParse(val[1], out result))
                            {
                                samp.BackPlaneTemp = result;
                            }
                        }
                    }

                    // Battery
                    if (lines[x].Contains("Battery"))
                    {
                        var val = lines[x].Split(spaces, StringSplitOptions.RemoveEmptyEntries);
                        if (val.Length >= 3)
                        {
                            double result = 0.0;
                            if (double.TryParse(val[1], out result))
                            {
                                samp.Battery = result;
                            }
                        }
                    }

                    // Boost+
                    if (lines[x].Contains("Boost+"))
                    {
                        var val = lines[x].Split(spaces, StringSplitOptions.RemoveEmptyEntries);
                        if (val.Length >= 3)
                        {
                            double result = 0.0;
                            if (double.TryParse(val[1], out result))
                            {
                                samp.BoostPos = result;
                            }
                        }
                    }

                    // Boost-
                    if (lines[x].Contains("Boost-"))
                    {
                        var val = lines[x].Split(spaces, StringSplitOptions.RemoveEmptyEntries);
                        if (val.Length >= 3)
                        {
                            double result = 0.0;
                            if (double.TryParse(val[1], out result))
                            {
                                samp.BoostNeg = result;
                            }
                        }
                    }
                }

                return samp;
            }

            #endregion

            #region DIAGPRESSURE

            /// <summary>
            /// Check the pressure sensor
            /// 
            /// No Pressure Sensor
            /// ===================
            /// 
            /// 
            /// 
            /// Pressure Sensor Installed
            /// 
            /// 
            /// PS SN: 368389
            /// Address: 250
            /// Hardware Ver: 5.20
            /// Firmware Ver: 12.28
            /// Buffer Size: 13
            /// Status: 1
            /// Keller30 Read Coefficients:
            /// 64, offset    0.01416564, bar,  0
            /// 80, min       0.00000000, bar,  0
            /// 81, max      30.00000000, bar,  0
            /// 84, min             -nan, C,  0
            /// 85, max             -nan, C,  0
            /// Keller30 Read Configuration:
            /// Nr, Conf, err
            ///  0,   02,   0
            ///  1,   10,   0
            ///  2,   00,   0
            ///  3,   01,   0
            ///  4,   01,   0
            ///  5,   00,   0
            ///  6,   02,   0
            ///  7,   35,   0
            ///  8,   00,   0
            ///  9,   00,   0
            /// 10,   00,   0
            /// 11,   35,   0
            /// 12,   00,   0
            /// 13,   01,   0
            /// diagpressure
            /// </summary>
            /// <param name="buffer">Buffer of the message.</param>
            /// <returns>Return the pressure sensor results.</returns>
            public static DiagPressure DecodeDiagPressure(string buffer)
            {
                // Initialize a sample
                DiagPressure pressureSensor = new DiagPressure();

                if(string.IsNullOrEmpty(buffer))
                {
                    pressureSensor.IsPressureSensorInstalled = false;
                    return pressureSensor;
                }

                // Set the buffer
                pressureSensor.DiagPressureStr = buffer;
                pressureSensor.IsPressureSensorInstalled = false;

                // Break up the lines
                char[] newLines = new char[] { '\r', '\n' };
                char[] spaces = new char[] { ' ' };
                char[] spacesComma = new char[] { ' ', ',' };
                string[] lines = buffer.Split(newLines, StringSplitOptions.RemoveEmptyEntries);

                // Decode each line of data
                for (int x = 0; x < lines.Length; x++)
                {
                    if (lines[x].Contains("max") && lines[x].Contains("bar"))
                    {
                        var vals = lines[x].Split(spacesComma, StringSplitOptions.RemoveEmptyEntries);
                        if (vals.Length >= 4)
                        {
                            pressureSensor.Rating = vals[2];
                            pressureSensor.IsPressureSensorInstalled = true;
                        }
                    }
                }

                return pressureSensor;
            }


            #endregion

            #region Command Set

            /// <summary>
            /// Decoding a command set is similar to decoding CSHOW except
            /// in the command set, each line is a command for a subsystem where
            /// in CSHOW each line contain multiple subsystems for 1 command line.
            /// 
            /// This will combine all the commands to make the command set similar
            /// to the CSHOW output.
            /// 
            /// </summary>
            /// <param name="buffer">Buffer of the command set.</param>
            /// <param name="serial">Serial number.</param>
            /// <returns>ADCP configuration.</returns>
            public static AdcpConfiguration DecodeCommandSet(string buffer, SerialNumber serial)
            {
                Dictionary<string, string> commands = new Dictionary<string, string>();

                // Convert the commandset in to CSHOW
                string[] results = buffer.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                for (int x = 0; x < results.Length; x++)
                {
                    // Get the command for each line
                    string[] cmd = results[x].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    // Get the cmd
                    if(cmd.Length > 1)
                    {
                        // If the command is CEPO
                        // Get the subsystem code to add to the serial number
                        if(cmd[0] == CMD_CEPO)
                        {
                            var cepoList = cmd[1].ToCharArray();
                            for(int cepoIndex = 0; cepoIndex < cepoList.Length; cepoIndex++ )
                            {
                                var subsys = new Subsystem((byte)cepoList[cepoIndex]);
                                serial.AddSubsystem(subsys);
                            }
                        }

                        // Command all the parameters
                        string param = "";
                        for (int y = 1; y < cmd.Length; y++ )
                        {
                            param += cmd[y];
                        }

                        // Append to the end if the key exist
                        if (commands.ContainsKey(cmd[0]))
                        {
                            commands[cmd[0]] += param;
                        }
                        else
                        {
                            commands.Add(cmd[0], param);
                        }
                    }
                    else
                    {
                        // No command, just add it
                        commands.Add(results[x], "");
                    }
                }

                // Combine all the values into the same format as CSHOW
                StringBuilder sb = new StringBuilder();
                
                foreach(var val in commands.Keys)
                {
                    sb.AppendLine(string.Format("{0} {1}", val, commands[val]));
                }

                DecodeCSHOW decoder = new DecodeCSHOW();
                return decoder.Decode(sb.ToString(), serial);
            }

            #endregion

            #endregion

        }
    }
}