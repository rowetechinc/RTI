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
 * 
 */

using System;
using System.Collections.Generic;


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

        #endregion

        /// <summary>
        /// Commands handled by the ADCP.
        /// </summary>
        public class AdcpCommands
        {
            /// <summary>
            /// Serial number to know how many 
            /// subsystem exist in the current 
            /// ADCP.
            /// </summary>
            private SerialNumber _serialNumber;

            #region Commands

            //public const char CR = '\r';

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

            #region Diagnostic

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

        }
    }
}