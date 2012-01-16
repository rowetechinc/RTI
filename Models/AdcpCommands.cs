/*
 * Copyright © 2011 
 * Rowe Technology Inc.
 * All rights reserved.
 * http://www.rowetechinc.com
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification is NOT permitted.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
 * COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
 * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
 * POSSIBILITY OF SUCH DAMAGE.
 * 
 * HISTORY
 * -----------------------------------------------------------------
 * Date            Initials    Comments
 * -----------------------------------------------------------------
 * 10/06/2011      RC          Initial coding
 * 10/11/2011      RC          Adding ToString for send to ADCP
 *                              Removed IDataErrorInfo
 * 11/17/2011      RC          Added GetCommandList().
 * 11/18/2011      RC          Removed CR from end of GetTimeCommand().
 * 11/21/2011      RC          Added ==, !=, HashCode and Equal to TimeValue for unit test. 
 * 12/05/2011      RC          Added min/max Hundredth of a second.
 * 
 */

using System;
using System.Collections.Generic;


namespace RTI
{
    namespace Commands
    {
        /// <summary>
        /// Commands handled by the ADCP.
        /// </summary>
        public class AdcpCommands
        {
            #region Commands

            //public const char CR = '\r';

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

            /// <summary>
            /// Water Profile Command: On/Off
            /// Paramater: N = 0 to 1
            /// 0=Disable 1=Enable. 
            /// Enables or disables water profile pings.
            /// </summary>
            public const string CMD_CWPON = "CWPON";

            /// <summary>
            /// Water Profile Command: Broadband On/Off
            /// Parameter: N = 0 to 1
            /// 0=Disable 1=Enable. 
            /// Enables or disables Water Profile boardband processing. 
            /// Narrowband processing is used when broadband disabled
            /// </summary>
            public const string CMD_CWPBB = "CWPBB";

            /// <summary>
            /// Water Profile Command: Blank
            /// Parameter: n.nn = 0.00 to 100 (meters)  
            /// Sets the vertical range from the face of the transducer 
            /// to the first sample of the first Bin.
            /// </summary>
            public const string CMD_CWPBL = "CWPBL"; 

            /// <summary>
            /// Water Profile Command: Bin Size
            /// Parameter: n.nn = 0.01 to 100 (meters)  
            /// n.nn sets the vertical Bin size.
            /// </summary>
            public const string CMD_CWPBS = "CWPBS";
 
            /// <summary>
            /// Water Profile Command: Trasmit
            /// Parameter: n.nn = 0.00 to 100 (meters)
            /// n.nn sets the vertical transmit size.  
            /// A value of 0.00 will cause the system to 
            /// set transmit to the same length as the cell size.
            /// </summary>
            public const string CMD_CWPX = "CWPX";

            /// <summary>
            /// Water Profile Command: Number of bins
            /// Parameter: N = 1 to 200.  
            /// Sets the number of bins that will be processed and output.
            /// </summary>
            public const string CMD_CWPBN = "CWPBN";

            /// <summary>
            /// Water Profile Command: Number of Pings
            /// Parameter: N = 0 to 10,000
            /// Sets the number of pings that will be averaged together during the ensemble.
            /// If CWPAI is set equal to 00:00:00.00.
            /// </summary>
            public const string CMD_CWPP = "CWPP";

            /// <summary>
            /// Water Profile Command: Averaging Interval
            /// Parameter: HH:MM:SS.hh 
            /// Sets the interval over which the profile pings are 
            /// evenly spread.  To determine the number of pings in 
            /// the ensemble CWPAI is divided by CWPTBP.  If CWPAI 
            /// is set equal to 00:00:00.00 sets the number of pings 
            /// in the ensemble.
            /// </summary>
            public const string CMD_CWPAI = "CWPAI";

            /// <summary>
            /// Water Profile Command: Time between Pings
            /// Parameter: n.nn = 0.00 to 86400 (seconds) 
            /// Sets the time between profile pings.
            /// </summary>
            public const string CMD_CWPTBP = "CWPTBP";

            /// <summary>
            /// Ensemble Command: Ensemble Interval
            /// Parameter: HH:MM:SS.hh 
            /// Sets the time interval that system will output the 
            /// averaged profile/bottom track data. 
            /// </summary>
            public const string CMD_CEI = "CEI"; 

            /// <summary>
            /// Bottom Track Command: On/Off
            /// Parameter: N = 0 to 1
            /// 0=Disable 1=Enable.  
            /// Enables or disables bottom track pings. 
            /// </summary>
            public const string CMD_CBTON = "CBTON";

            /// <summary>
            /// Bottom Track Command: Broadband On/Off
            /// Parameter: N = 0 to 1
            /// 0=Disable 1=Enable.  
            /// Enables or disables bottom track boardband processing.  
            /// Narrowband processing is used when boardband is disabled
            /// </summary>
            public const string CMD_CBTBB = "CBTBB";

            /// <summary>
            /// Bottom Track Command: Blank
            /// Parameter: n.nn = 0 to 10 (meters)  
            /// Sets the vertical distance from the face of the transducer 
            /// at which the bottom dectection algorithm begins search for the bottom
            /// </summary>
            public const string CMD_CBTBL = "CBTBL";

            /// <summary>
            /// Bottom Track Command: Max Depth
            /// Parameter: n.nn = 5 to 10,000 (meters)  
            /// Sets the maximum range over which the bottom track algorithm 
            /// will search for the bottom.  A large value will slow acquistion time
            /// </summary>
            public const string CMD_CBTMX = "CBTMX";

            /// <summary>
            /// Bottom Track Command: Time Between Pings
            /// Parameter: n.nn = 0.00 to 86400 (seconds) 
            /// Sets the time between bottom pings
            /// </summary>
            public const string CMD_CBTTBP = "CBTTBP";

            /// <summary>
            /// Water Track Command: On/Off
            /// Parameter: N = 0 to 1
            /// 0=Disable 1=Enable.  
            /// Enable or diable water track
            /// </summary>
            public const string CMD_CWTON = "CWTON";

            /// <summary>
            /// Water Track Command: Broadband On/Off
            /// Parameter: N = 0 to 1
            /// 0=Disable 1=Enable.  
            /// Enable or diable water track broadband processing
            /// </summary>
            public const string CMD_CWTBB = "CWTBB";

            /// <summary>
            /// Water Track Command: Blank
            /// Paramter: n.nn = 0.00 to 100 (meters)  
            /// Sets the vertical range from the face of the 
            /// transducer to the first sample of the Bin.
            /// </summary>
            public const string CMD_CWTBL = "CWTBL";

            /// <summary>
            /// Water Track Command: Bin Size
            /// Parameter: n.nn = 0.05 to 64 (meters) 
            /// Sets the vertical Bin size.
            /// </summary>
            public const string CMD_CWTBS = "CWTBS";

            /// <summary>
            /// Water Track Command: Time Between Pings
            /// Parameter: N = 0.00 to 86400 (seconds)
            /// Sets the time between bottom pings.
            /// </summary>
            public const string CMD_CWTTBP = "CWTTBP"; 

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
            /// Default ADCP mode.
            /// </summary>
            public const AdcpMode DEFAULT_MODE = AdcpMode.PROFILE;

            /// <summary>
            /// Default ADCP frequency.
            /// </summary>
            public const Frequencies DEFAULT_FREQ = Frequencies.Freq_320Khz;

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
            /// Default Bottom Track On.
            /// </summary>
            public const bool DEFAULT_CBTON = true;

            /// <summary>
            /// Default Water On.
            /// </summary>
            public const bool DEFAULT_CWTON = false;

            /// <summary>
            /// Default Water Profile On.
            /// </summary>
            public const bool DEFAULT_CWPON = true;

            /// <summary>
            /// Default Bottom Track Broadband.
            /// </summary>
            public const bool DEFAULT_CBTBB = true;

            /// <summary>
            /// Default Water Track Broadband.
            /// </summary>
            public const bool DEFAULT_CWTBB = true;

            /// <summary>
            /// Default Water Profile Broadband.
            /// </summary>
            public const bool DEFAULT_CWPBB = true;

            /// <summary>
            /// Default flag if CWPAI enabled.
            /// </summary>
            public const bool DEFAULT_ENABLE_CWPAI = false;

            /// <summary>
            /// Default Averaging Interval.
            /// 0 seconds.
            /// </summary>
            public TimeValue DEFAULT_CWPAI = new TimeValue();

            /// <summary>
            /// Default Ensemble interval.
            /// 1 second.
            /// </summary>
            public TimeValue DEFAULT_CEI = new TimeValue(0, 0, 1, 0);

            #region Default 40khz
            
            /// <summary>
            /// Default 40khz Bottom Track Time between pings (2 seconds).
            /// </summary>
            public const float DEFAULT_40_CBTTBP = 2.0f;

            /// <summary>
            /// Default 40khz Water Track Time between pings (4 seconds).
            /// </summary>
            public const float DEFAULT_40_CWTTBP = 4.0f;

            /// <summary>
            /// Default 40khz Water Profile Time between pings (4 seconds).
            /// </summary>
            public const float DEFAULT_40_CWPTBP = 4.0f;

            /// <summary>
            /// Default 40khz Bottom Track Blank (2 meters).
            /// </summary>
            public const float DEFAULT_40_CBTBL = 2.0f;

            /// <summary>
            /// Default 40khz Bottom Track Maximum Depth (2000 meters).
            /// </summary>
            public const float DEFAULT_40_CBTMX = 2000.0f;

            /// <summary>
            /// Default 40khz Water Track Blank (32 meters).
            /// </summary>
            public const float DEFAULT_40_CWTBL = 32.0f;

            /// <summary>
            /// Default 40khz Water Track Bin size (32 meters).
            /// </summary>
            public const float DEFAULT_40_CWTBS = 32.0f;

            /// <summary>
            /// Default 40khz Water Profile Blank (32 meters).
            /// </summary>
            public const float DEFAULT_40_CWPBL = 32.0f;

            /// <summary>
            /// Default 40khz Water Profile Bin size (32 meters).
            /// </summary>
            public const float DEFAULT_40_CWPBS = 32.0f;

            /// <summary>
            /// Default 40khz Water Profile Transmit (0.00 = Same as cell size).
            /// </summary>
            public const float DEFAULT_40_CWPX = 0.00f;

            /// <summary>
            /// Default 40khz Water Profile Number of bins (30 bins).
            /// </summary>
            public const int DEFAULT_40_CWPBN = 30;

            /// <summary>
            /// Default 40khz Water Profile Number of pings (2 pings averaged per ensemble).
            /// </summary>
            public const int DEFAULT_40_CWPP = 2;
            #endregion

            #region Default 80khz
            /// <summary>
            /// Default 80khz Bottom Track Time between pings (1 seconds).
            /// </summary>
            public const float DEFAULT_80_CBTTBP = 1.0f;

            /// <summary>
            /// Default 80khz Water Track Time between pings (2 seconds).
            /// </summary>
            public const float DEFAULT_80_CWTTBP = 2.0f;

            /// <summary>
            /// Default 80khz Water Profile Time between pings (2 seconds).
            /// </summary>
            public const float DEFAULT_80_CWPTBP = 2.0f;

            /// <summary>
            /// Default 80khz Bottom Track Blank (1 meters).
            /// </summary>
            public const float DEFAULT_80_CBTBL = 1.0f;

            /// <summary>
            /// Default 80khz Bottom Track Maximum Depth (1000 meters).
            /// </summary>
            public const float DEFAULT_80_CBTMX = 1000.0f;

            /// <summary>
            /// Default 80khz Water Track Blank (16 meters).
            /// </summary>
            public const float DEFAULT_80_CWTBL = 16.0f;

            /// <summary>
            /// Default 80khz Water Track Bin size (16 meters).
            /// </summary>
            public const float DEFAULT_80_CWTBS = 16.0f;

            /// <summary>
            /// Default 80khz Water Profile Blank (16 meters).
            /// </summary>
            public const float DEFAULT_80_CWPBL = 16.0f;
            
            /// <summary>
            /// Default 80khz Water Profile Bin size (16 meters).
            /// </summary>
            public const float DEFAULT_80_CWPBS = 16.0f;

            /// <summary>
            /// Default 80khz Water Profile Transmit (0.00 = Same as cell size).
            /// </summary>
            public const float DEFAULT_80_CWPX = 0.00f;

            /// <summary>
            /// Default 80khz Water Profile Number of bins (30 bins).
            /// </summary>
            public const int DEFAULT_80_CWPBN = 30;

            /// <summary>
            /// Default 80khz Water Profile Number of pings (2 pings averaged per ensemble).
            /// </summary>
            public const int DEFAULT_80_CWPP = 2;
            #endregion

            #region Default 160khz
            /// <summary>
            /// Default 160khz Bottom Track Time between pings (0.5 seconds).
            /// </summary>
            public const float DEFAULT_160_CBTTBP = 0.5f;

            /// <summary>
            /// Default 160khz Water Track Time between pings (1 seconds).
            /// </summary>
            public const float DEFAULT_160_CWTTBP = 1.0f;

            /// <summary>
            /// Default 160khz Water Profile Time between pings (1 seconds).
            /// </summary>
            public const float DEFAULT_160_CWPTBP = 1.0f;

            /// <summary>
            /// Default 160khz Bottom Track Blank (0.5 meters).
            /// </summary>
            public const float DEFAULT_160_CBTBL = 0.5f;

            /// <summary>
            /// Default 160khz Bottom Track Maximum Depth (500 meters).
            /// </summary>
            public const float DEFAULT_160_CBTMX = 500.0f;

            /// <summary>
            /// Default 160khz Water Track Blank (8 meters).
            /// </summary>
            public const float DEFAULT_160_CWTBL = 8.0f;

            /// <summary>
            /// Default 160khz Water Track Bin size (8 meters).
            /// </summary>
            public const float DEFAULT_160_CWTBS = 8.0f;

            /// <summary>
            /// Default 160khz Water Profile Blank (8 meters).
            /// </summary>
            public const float DEFAULT_160_CWPBL = 8.0f;

            /// <summary>
            /// Default 160khz Water Profile Bin size (8 meters).
            /// </summary>
            public const float DEFAULT_160_CWPBS = 8.0f;

            /// <summary>
            /// Default 160khz Water Profile Transmit (0.00 = Same as cell size).
            /// </summary>
            public const float DEFAULT_160_CWPX = 0.00f;

            /// <summary>
            /// Default 160khz Water Profile Number of bins (30 bins).
            /// </summary>
            public const int DEFAULT_160_CWPBN = 30;

            /// <summary>
            /// Default 160khz Water Profile Number of pings (2 pings averaged per ensemble).
            /// </summary>
            public const int DEFAULT_160_CWPP = 2;
            #endregion

            #region Default 320khz
            /// <summary>
            /// Default 320khz Bottom Track Time between pings (0.25 seconds).
            /// </summary>
            public const float DEFAULT_320_CBTTBP = 0.25f;

            /// <summary>
            /// Default 320khz Water Track Time between pings (0.5 seconds).
            /// </summary>
            public const float DEFAULT_320_CWTTBP = 0.5f;

            /// <summary>
            /// Default 320khz Water Profile Time between pings (0.5 seconds).
            /// </summary>
            public const float DEFAULT_320_CWPTBP = 0.5f;

            /// <summary>
            /// Default 320khz Bottom Track Blank (0.25 meters).
            /// </summary>
            public const float DEFAULT_320_CBTBL = 0.25f;

            /// <summary>
            /// Default 320khz Bottom Track Maximum Depth (250 meters).
            /// </summary>
            public const float DEFAULT_320_CBTMX = 250.0f;

            /// <summary>
            /// Default 320khz Water Track Blank (4 meters).
            /// </summary>
            public const float DEFAULT_320_CWTBL = 4.0f;

            /// <summary>
            /// Default 320khz Water Track Bin size (4 meters).
            /// </summary>
            public const float DEFAULT_320_CWTBS = 4.0f;

            /// <summary>
            /// Default 320khz Water Profile Blank (4 meters).
            /// </summary>
            public const float DEFAULT_320_CWPBL = 4.0f;

            /// <summary>
            /// Default 320khz Water Profile Bin size (4 meters).
            /// </summary>
            public const float DEFAULT_320_CWPBS = 4.0f;

            /// <summary>
            /// Default 320khz Water Profile Transmit (0.00 = Same as cell size).
            /// </summary>
            public const float DEFAULT_320_CWPX = 0.00f;

            /// <summary>
            /// Default 320khz Water Profile Number of bins (30 bins).
            /// </summary>
            public const int DEFAULT_320_CWPBN = 30;

            /// <summary>
            /// Default 320khz Water Profile Number of pings (2 pings averaged per ensemble).
            /// </summary>
            public const int DEFAULT_320_CWPP = 2;
            #endregion

            #region Default 640khz
            /// <summary>
            /// Default 640khz Bottom Track Time between pings (0.125 seconds).
            /// </summary>
            public const float DEFAULT_640_CBTTBP = 0.125f;

            /// <summary>
            /// Default 640khz Water Track Time between pings (0.25 seconds).
            /// </summary>
            public const float DEFAULT_640_CWTTBP = 0.25f;

            /// <summary>
            /// Default 640khz Water Profile Time between pings (0.25 seconds).
            /// </summary>
            public const float DEFAULT_640_CWPTBP = 0.25f;

            /// <summary>
            /// Default 640khz Bottom Track Blank (0.125 meters).
            /// </summary>
            public const float DEFAULT_640_CBTBL = 0.125f;

            /// <summary>
            /// Default 640khz Bottom Track Maximum Depth (150 meters).
            /// </summary>
            public const float DEFAULT_640_CBTMX = 150.0f;

            /// <summary>
            /// Default 640khz Water Track Blank (2 meters).
            /// </summary>
            public const float DEFAULT_640_CWTBL = 2.0f;

            /// <summary>
            /// Default 640khz Water Track Bin size (2 meters).
            /// </summary>
            public const float DEFAULT_640_CWTBS = 2.0f;

            /// <summary>
            /// Default 640khz Water Profile Blank (2 meters).
            /// </summary>
            public const float DEFAULT_640_CWPBL = 2.0f;

            /// <summary>
            /// Default 640khz Water Profile Bin size (2 meters).
            /// </summary>
            public const float DEFAULT_640_CWPBS = 2.0f;

            /// <summary>
            /// Default 640khz Water Profile Transmit (0.00 = Same as cell size).
            /// </summary>
            public const float DEFAULT_640_CWPX = 0.00f;

            /// <summary>
            /// Default 640khz Water Profile Number of bins (30 bins).
            /// </summary>
            public const int DEFAULT_640_CWPBN = 30;

            /// <summary>
            /// Default 640khz Water Profile Number of pings (2 pings averaged per ensemble).
            /// </summary>
            public const int DEFAULT_640_CWPP = 2;
            #endregion

            #endregion

            #region Min/Max Values

            /// <summary>
            /// Minimum BT Time Between Ping.
            /// </summary>
            public const double MIN_CBTTBP = 0.0;

            /// <summary>
            /// Maximum BT Time Between Ping.
            /// </summary>
            public const double MAX_CBTTBP = 86400.0;
            
            /// <summary>
            /// Minimum Water Track Time Between Ping.
            /// </summary>
            public const double MIN_CWTTBP = 0.0;

            /// <summary>
            /// Maximum Water Track Time Between Ping.
            /// </summary>
            public const double MAX_CWTTBP = 86400.0;
            
            /// <summary>
            /// Minimum Water Profile Time Between Ping.
            /// </summary>
            public const double MIN_CWPTBP = 0.0;

            /// <summary>
            /// Maximum Water Profile Time Between Ping.
            /// </summary>
            public const double MAX_CWPTBP = 86400.0;
            
            /// <summary>
            /// Minimum BT Blank.
            /// </summary>
            public const double MIN_CBTBL = 0.0;

            /// <summary>
            /// Maximum BT Blank.
            /// </summary>
            public const double MAX_CBTBL = 10.0;
            
            /// <summary>
            /// Minimum WT Blank.
            /// </summary>
            public const double MIN_CWTBL = 0.0;

            /// <summary>
            /// Maximum WT Blank.
            /// </summary>
            public const double MAX_CWTBL = 100.0;
            
            /// <summary>
            /// Minimum WP Blank.
            /// </summary>
            public const double MIN_CWPBL = 0.0;

            /// <summary>
            /// Maximum WP Blank.
            /// </summary>
            public const double MAX_CWPBL = 100.0;
            
            /// <summary>
            /// Minimum BT maximum Depth.
            /// </summary>
            public const double MIN_CBTMX = 5.0;

            /// <summary>
            /// Maximum BT maximum Depth.
            /// </summary>
            public const double MAX_CBTMX = 10000;
            
            /// <summary>
            /// Minimum Water Track Bin size.
            /// </summary>
            public const double MIN_CWTBS = 0.05;

            /// <summary>
            /// Maximum Water Track Bin size.
            /// </summary>
            public const double MAX_CWTBS = 64.0;
            
            /// <summary>
            /// Minimum Water Profile Bin size.
            /// </summary>
            public const double MIN_CWPBS = 0.01;

            /// <summary>
            /// Maximum Water Profile Bin size.
            /// </summary>
            public const double MAX_CWPBS = 100.0;
            
            /// <summary>
            /// Minimum Water Profile Number of bins.
            /// </summary>
            public const int MIN_CWPBN = 0;

            /// <summary>
            /// Maximum Water Profile Number of bins.
            /// </summary>
            public const int MAX_CWPBN = 200;
            
            /// <summary>
            /// Minimum Water Profile Number of pings averaged per ensemble.
            /// </summary>
            public const int MIN_CWPP = 0;

            /// <summary>
            /// Maximum Water Profile Number of pings averaged per ensemble.
            /// </summary>
            public const int MAX_CWPP = 10000;
            
            /// <summary>
            /// Minimum Water Profile Transmit (Use cell size).
            /// </summary>
            public const double MIN_CWPX = 0.0;

            /// <summary>
            /// Maximum Water Profile Transmit.
            /// </summary>
            public const double MAX_CWPX = 100.0;
            
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

            /// <summary>
            /// Possible frequencies for a transducer.
            /// </summary>
            public enum Frequencies
            {
                /// <summary>
                /// 40KHz ADCP.
                /// </summary>
                Freq_40Khz = 40,

                /// <summary>
                /// 80Khz ADCP.
                /// </summary>
                Freq_80Khz = 80,

                /// <summary>
                /// 160Khz ADCP.
                /// </summary>
                Freq_160Khz = 160,

                /// <summary>
                /// 320Khz ADCP.
                /// </summary>
                Freq_320Khz = 320,

                /// <summary>
                /// 640Khz ADCP.
                /// </summary>
                Freq_640Khz = 640,

                /// <summary>
                /// Other ADCP.
                /// </summary>
                Freq_Other = 0
            };

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
            /// Baud rate options for C232B and C485B.
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
                BAUD_115200 = 115200
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

                    if(a.Hour == b.Hour &&
                        a.Minute == b.Minute &&
                        a.Second == b.Second &&
                        a.HunSec == b.HunSec )
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

            /// <summary>
            /// Frequency of the system.  This will be used
            /// to determine the default ranges for the system.
            /// </summary>
            public Frequencies Frequency { get; set; }

            #region Water Profile Properties

            /// <summary>
            /// Water Profile Pings On. 
            /// Enables or disables water profile pings.
            /// 
            /// Command: CWPON N \r
            /// Range: N = 0 or 1
            /// 0=Disable 
            /// 1=Enable. 
            /// </summary>
            public bool CWPON { get; set; }

            /// <summary>
            /// Return a string of whether
            /// the it is enabled or disabled.
            /// </summary>
            /// <returns>0 = Disabled / 1 = Enabled.</returns>
            public string CWPON_ToString()
            {
                if (CWPON)
                {
                    return "1";
                }

                return "0";
            }

            /// <summary>
            /// Water Profile Broad Band. 
            /// Enables or disables Water Profile boardband processing. 
            /// Narrowband processing is used when broadband disabled
            /// 
            /// Command: CWPBB N \r
            /// Range: N = 0 or 1
            /// 0=Disable 1=Enable. 
            /// </summary>
            public bool CWPBB { get; set; }

            /// <summary>
            /// Return a string of whether
            /// the it is enabled or disabled.
            /// </summary>
            /// <returns>0 = Disabled / 1 = Enabled.</returns>
            public string CWPBB_ToString()
            {
                if (CWPBB)
                {
                    return "1";
                }

                return "0";
            }

            /// <summary>
            /// Water Profile Blank. 
            /// Sets the vertical range from the face of the 
            /// transducer to the first sample of the first Bin.
            /// 
            /// Command: CWPBL n.nn \r
            /// Scale: meters
            /// Range: 0.00 to 100.00
            /// </summary>
            public float CWPBL { get; set; }

            /// <summary>
            /// Water Profile Bin Size 
            /// n.nn sets the vertical Bin size.
            /// 
            /// Command CWPBS n.nnn \r
            /// Scale: meters
            /// Range: 0.01 to 64.00
            /// </summary>
            public float CWPBS { get; set; }

            /// <summary>
            /// Water Profile Tramist 
            /// n.nn sets the vertical transmit size.  
            /// 
            /// A value of 0.00 will cause the system to 
            /// set transmit to the same length as the cell size.
            /// 
            /// Command: CWPX n.nn \r
            /// Scale: meters
            /// Range: 0.00 to 100.0 
            /// </summary>
            public float CWPX { get; set; }

            /// <summary>
            /// Water Profile Number of Bins
            /// Sets the number of bins that will be processed and output.
            /// 
            /// Command: CWPBN N \r
            /// Scale: Number of bins
            /// Range: 1 to 200 
            /// </summary>
            public UInt16 CWPBN { get; set; }

            /// <summary>
            /// Water Profile Number of Pings
            /// Sets the number of pings that will 
            /// be averaged together during the ensemble.
            /// 
            /// If CWPAI is set equal to 00:00:00.00
            /// 
            /// Command: CWPP N \r
            /// Scale: Number of Pings
            /// Range: 0 to 10,000 
            /// </summary>
            public UInt16 CWPP { get; set; }


            /// <summary>
            /// Water Profile Averaging Interval
            /// Sets the interval over which the 
            /// profile pings are evenly spread.  
            /// To determine the number of pings 
            /// in the ensemble CWPAI is divided 
            /// by CWPTBP.  
            /// 
            /// If CWPAI is set equal to 00:00:00.00 
            /// sets the number of pings in the ensemble.
            /// 
            /// Total time cannot exceed 86400 seconds(1 day).
            /// 
            /// There is no SET.  The SET is done using
            /// the CWPAI_Hour and ...
            /// 
            /// Command: CWPAI HH:MM:SS.hh \r
            /// Scale: HH:MM:SS.hh
            /// Range: 0 to 86400 seconds
            /// </summary>
            public TimeValue CWPAI { get; set; }

            /// <summary>
            /// Return a string representation of CWPAI.
            /// </summary>
            /// <returns>String representation of CWPAI</returns>
            public string CWPAI_ToString()
            {
                return CWPAI_Hour.ToString("00") + ":" + CWPAI_Minute.ToString("00") + ":" + CWPAI_Second.ToString("00") + "." + CWPAI_HunSec.ToString("00");
            }

            /// <summary>
            /// Hours for CWPAI.
            /// Limit to 2 digits.
            /// 
            /// When setting the value, if the
            /// value is valid, set it to the
            /// TimeValue for CWPAI.
            /// 
            /// Scale: Hours
            /// Range: 0 to 23
            /// </summary>
            private UInt16 _cwpai_hour;

            /// <summary>
            /// Hours for CWPAI.
            /// Limit to 2 digits.
            /// 
            /// When setting the value, if the
            /// value is valid, set it to the
            /// TimeValue for CWPAI.
            /// 
            /// Scale: Hours
            /// Range: 0 to 23
            /// </summary>
            public UInt16 CWPAI_Hour 
            {
                get { return _cwpai_hour; }
                set
                {
                    _cwpai_hour = value;
                    CWPAI.Hour = value;
                }
            }

            /// <summary>
            /// Minutes in CWPAI
            /// Limit to 2 digits.
            /// 
            /// When setting the value, if the
            /// value is valid, set it to the
            /// TimeValue for CWPAI.
            /// 
            /// Scale: Minutes
            /// Range: 0 to 59
            /// </summary>
            private UInt16 _cwpai_minute;
            /// <summary>
            /// Minutes in CWPAI
            /// Limit to 2 digits.
            /// 
            /// When setting the value, if the
            /// value is valid, set it to the
            /// TimeValue for CWPAI.
            /// 
            /// Scale: Minutes
            /// Range: 0 to 59
            /// </summary>
            public UInt16 CWPAI_Minute
            {
                get { return _cwpai_minute; }
                set
                {
                    _cwpai_minute = value;
                    CWPAI.Minute = value;
                }
            }
            
            /// <summary>
            /// Seconds in CWPAI
            /// Limit to 2 digits.
            /// 
            /// When setting the value, if the
            /// value is valid, set it to the
            /// TimeValue for CWPAI.
            /// 
            /// Scale: Seconds
            /// Range: 0 to 59
            /// </summary>
            private UInt16 _cwpai_second;
            /// <summary>
            /// Seconds in CWPAI
            /// Limit to 2 digits.
            /// 
            /// When setting the value, if the
            /// value is valid, set it to the
            /// TimeValue for CWPAI.
            /// 
            /// Scale: Seconds
            /// Range: 0 to 59
            /// </summary>
            public UInt16 CWPAI_Second
            {
                get { return _cwpai_second; }
                set
                {
                    _cwpai_second = value;
                    CWPAI.Second = value;
                }
            }

            /// <summary>
            /// Hundredth of seconds in CWPAI.
            /// Limit to 2 digits.
            /// 
            /// When setting the value, if the
            /// value is valid, set it to the
            /// TimeValue for CWPAI.
            /// 
            /// Scale: Hundredth of Second
            /// Range: 0 to 59.
            /// </summary>
            private UInt16 _cwpai_hunSec;
            /// <summary>
            /// Hundredth of seconds in CWPAI.
            /// Limit to 2 digits.
            /// 
            /// When setting the value, if the
            /// value is valid, set it to the
            /// TimeValue for CWPAI.
            /// 
            /// Scale: Hundredth of Second
            /// Range: 0 to 59.
            /// </summary>
            public UInt16 CWPAI_HunSec
            {
                get { return _cwpai_hunSec; }
                set
                {
                    _cwpai_hunSec = value;
                    CWPAI.HunSec = value;
                }
            }

            /// <summary>
            /// Water Profile Time between Pings
            /// Sets the time between profile pings
            /// 
            /// Command: CWPTBP n.nn \r
            /// Scale: seconds
            /// Range: 0.00 to 86400.0 
            /// </summary>
            public float CWPTBP { get; set; }

            #endregion // Water Profile

            #region Ensemble Properties

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
            public UInt16 _cei_hour;
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
                get { return _cei_hour; }
                set
                {
                    _cei_hour = value;
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
            private UInt16 _cei_minute;
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
                get { return _cei_minute; }
                set
                {
                    _cei_minute = value;
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
            private UInt16 _cei_second;
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
                get { return _cei_second; }
                set
                {
                    _cei_second = value;
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
            private UInt16 _cei_hunSec;
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
                get { return _cei_hunSec; }
                set
                {
                    _cei_hunSec = value;
                    CEI.HunSec = value;
                }
            }
            
            #endregion

            #region Bottom Track Properties

            /// <summary>
            /// Bottom Track ON
            /// Enables or disables bottom track pings. 
            /// 
            /// Command: CBTON N[cr] 
            /// Range: N = 0 or 1
            /// 0=Disable 1=Enable. 
            /// </summary>
            public bool CBTON { get; set; }

            /// <summary>
            /// Return a string of whether
            /// the it is enabled or disabled.
            /// </summary>
            /// <returns>0 = Disabled / 1 = Enabled.</returns>
            public string CBTON_ToString()
            {
                if (CBTON)
                {
                    return "1";
                }

                return "0";
            }

            /// <summary>
            /// Bottom Track Broadband
            /// Enables or disables bottom track 
            /// boardband processing.  Narrowband 
            /// processing is used when boardband is disabled.
            /// 
            /// Command: CBTBB N[cr] 
            /// Range: N = 0 or 1
            /// 0=Disable 1=Enable. 
            /// </summary>
            public bool CBTBB { get; set; }

            /// <summary>
            /// Return a string of whether
            /// the it is enabled or disabled.
            /// </summary>
            /// <returns>0 = Disabled / 1 = Enabled.</returns>
            public string CBTBB_ToString()
            {
                if (CBTBB)
                {
                    return "1";
                }

                return "0";
            }

            /// <summary>
            /// Bottom Track Blank 
            /// Sets the vertical distance 
            /// from the face of the transducer 
            /// at which the bottom dectection 
            /// algorithm begins search for the bottom.
            /// 
            /// Command: CBTBL n.nn[cr]
            /// Scale: meters
            /// Range: 0.0 to 10.0
            /// </summary>
            public float CBTBL { get; set; }

            /// <summary>
            /// Bottom Track Max Depth
            /// Sets the maximum range over 
            /// which the bottom track 
            /// algorithm will search for the 
            /// bottom.  
            /// 
            /// A large value will slow acquistion time.
            /// 
            /// Command: CBTMX n.nn[cr]
            /// Scale: Meters
            /// Range: 5.0 to 10,000.0 
            /// </summary>
            public float CBTMX { get; set; }

            /// <summary>
            /// Bottom Track Time Between Pings 
            /// Sets the time between bottom pings.
            /// 
            /// Command: CBTTBP n.nn[cr]
            /// Scale: seconds
            /// Range: 0.00 to 86400.0
            /// </summary>
            public float CBTTBP { get; set; }
            
            #endregion

            #region Water Track Properties

            /// <summary>
            /// Water Track ON
            /// Enable or disable water track
            /// 
            /// Command: CWTON N[cr] 
            /// Range: N = 0 or 1
            /// 0=Disable 1=Enable. 
            /// </summary>
            public bool CWTON { get; set; }

            /// <summary>
            /// Return a string of whether
            /// the it is enabled or disabled.
            /// </summary>
            /// <returns>0 = Disabled / 1 = Enabled.</returns>
            public string CWTON_ToString()
            {
                if (CWTON)
                {
                    return "1";
                }

                return "0";
            }

            /// <summary>
            /// Water Track Boardband
            /// Enable or diable water track broadband processing
            /// 
            /// Command: CWTON N[cr] 
            /// Range: N = 0 or 1
            /// 0=Disable 1=Enable. 
            /// </summary>
            public bool CWTBB { get; set; }

            /// <summary>
            /// Return a string of whether
            /// the it is enabled or disabled.
            /// </summary>
            /// <returns>0 = Disabled / 1 = Enabled.</returns>
            public string CWTBB_ToString()
            {
                if (CWTBB)
                {
                    return "1";
                }

                return "0";
            }

            /// <summary>
            /// Water Track Blank
            /// Sets the vertical range from 
            /// the face of the transducer to 
            /// the first sample of the Bin.
            /// 
            /// Command: CWTBL n.nn[cr]
            /// Scale: meters
            /// Range: 0.00 to 100.0 
            /// </summary>
            public float CWTBL { get; set; }

            /// <summary>
            /// Water Track Bin Size 
            /// Sets the vertical Bin size.
            /// 
            /// Command: CWTBS n.nn[cr]
            /// Scale: meters
            /// Range: 0.05 to 64.0 
            /// </summary>
            public float CWTBS { get; set; }

            /// <summary>
            /// Water Track Time Between Pings 
            /// Sets the time between bottom pings.
            /// 
            /// Command: CWTTBP n.nn 
            /// Scale: seconds
            /// Range: 0.00 to 86400.0 
            /// </summary>
            public float CWTTBP { get; set; }

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

            #endregion

            #endregion // Properties

            /// <summary>
            /// Constructor
            /// 
            /// Initialize the ranges to there default value.
            /// </summary>
            public AdcpCommands()
            {
                // Initialize ranges
                
                // Set default ranges
                Mode = DEFAULT_MODE;
                Frequency = DEFAULT_FREQ;

                // Water Profile defaults
                CWPON = DEFAULT_CWPON;
                CWPBB = DEFAULT_CWPBB;
                CWPAI = DEFAULT_CWPAI;
                CWPAI_Hour = CWPAI.Hour;
                CWPAI_Minute = CWPAI.Minute;
                CWPAI_Second = CWPAI.Second;
                CWPAI_HunSec = CWPAI.HunSec;

                // Ensemble defaults
                CEI = DEFAULT_CEI;
                CEI_Hour = CEI.Hour;
                CEI_Minute = CEI.Minute;
                CEI_Second = CEI.Second;
                CEI_HunSec = CEI.HunSec;

                // Bottom Track defaults
                CBTON = DEFAULT_CBTON;
                CBTBB = DEFAULT_CBTBB;
                
                // Water Track defaults
                CWTON = DEFAULT_CWTON;
                CWTBB = DEFAULT_CWTBB;

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

                // Set the default ranges that are
                // based off frequency
                SetFrequencyDefaults();
            }

            #region Methods

            /// <summary>
            /// Return the command to set
            /// the current time to the device.
            /// STIME.
            /// </summary>
            /// <returns></returns>
            public string GetTimeCommand()
            {
                return String.Format("{0} {1}", CMD_STIME, DateTime.Now.ToString("yyyy/MM/dd,HH:mm:ss"));
            }

            /// <summary>
            /// Set the default value based off the
            /// frequency.
            /// </summary>
            public void SetFrequencyDefaults()
            {
                if (Frequency == Frequencies.Freq_40Khz)
                {
                    CWPBL = DEFAULT_40_CWPBL;
                    CWPBS = DEFAULT_40_CWPBS;
                    CWPX = DEFAULT_40_CWPX;
                    CWPBN = DEFAULT_40_CWPBN;
                    CWPP = DEFAULT_40_CWPP;
                    CWPTBP = DEFAULT_40_CWPTBP;

                    CBTBL = DEFAULT_40_CBTBL;
                    CBTMX = DEFAULT_40_CBTMX;
                    CBTTBP = DEFAULT_40_CBTTBP;

                    CWTBL = DEFAULT_40_CWTBL;
                    CWTBS = DEFAULT_40_CWTBS;
                    CWTTBP = DEFAULT_40_CWTTBP;
                    
                }
                else if (Frequency == Frequencies.Freq_80Khz)
                {
                    CWPBL = DEFAULT_80_CWPBL;
                    CWPBS = DEFAULT_80_CWPBS;
                    CWPX = DEFAULT_80_CWPX;
                    CWPBN = DEFAULT_80_CWPBN;
                    CWPP = DEFAULT_80_CWPP;
                    CWPTBP = DEFAULT_80_CWPTBP;

                    CBTBL = DEFAULT_80_CBTBL;
                    CBTMX = DEFAULT_80_CBTMX;
                    CBTTBP = DEFAULT_80_CBTTBP;

                    CWTBL = DEFAULT_80_CWTBL;
                    CWTBS = DEFAULT_80_CWTBS;
                    CWTTBP = DEFAULT_80_CWTTBP;
                }
                else if (Frequency == Frequencies.Freq_160Khz)
                {
                    CWPBL = DEFAULT_160_CWPBL;
                    CWPBS = DEFAULT_160_CWPBS;
                    CWPX = DEFAULT_160_CWPX;
                    CWPBN = DEFAULT_160_CWPBN;
                    CWPP = DEFAULT_160_CWPP;
                    CWPTBP = DEFAULT_160_CWPTBP;

                    CBTBL = DEFAULT_160_CBTBL;
                    CBTMX = DEFAULT_160_CBTMX;
                    CBTTBP = DEFAULT_160_CBTTBP;

                    CWTBL = DEFAULT_160_CWTBL;
                    CWTBS = DEFAULT_160_CWTBS;
                    CWTTBP = DEFAULT_160_CWTTBP;
                }
                else if (Frequency == Frequencies.Freq_320Khz)
                {
                    CWPBL = DEFAULT_320_CWPBL;
                    CWPBS = DEFAULT_320_CWPBS;
                    CWPX = DEFAULT_320_CWPX;
                    CWPBN = DEFAULT_320_CWPBN;
                    CWPP = DEFAULT_320_CWPP;
                    CWPTBP = DEFAULT_320_CWPTBP;

                    CBTBL = DEFAULT_320_CBTBL;
                    CBTMX = DEFAULT_320_CBTMX;
                    CBTTBP = DEFAULT_320_CBTTBP;

                    CWTBL = DEFAULT_320_CWTBL;
                    CWTBS = DEFAULT_320_CWTBS;
                    CWTTBP = DEFAULT_320_CWTTBP;
                }
                else if (Frequency == Frequencies.Freq_640Khz)
                {
                    CWPBL = DEFAULT_640_CWPBL;
                    CWPBS = DEFAULT_640_CWPBS;
                    CWPX = DEFAULT_640_CWPX;
                    CWPBN = DEFAULT_640_CWPBN;
                    CWPP = DEFAULT_640_CWPP;
                    CWPTBP = DEFAULT_640_CWPTBP;

                    CBTBL = DEFAULT_640_CBTBL;
                    CBTMX = DEFAULT_640_CBTMX;
                    CBTTBP = DEFAULT_640_CBTTBP;

                    CWTBL = DEFAULT_640_CWTBL;
                    CWTBS = DEFAULT_640_CWTBS;
                    CWTTBP = DEFAULT_640_CWTTBP;
                }
                else if (Frequency == Frequencies.Freq_Other)
                {

                }
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
                list.Add(string.Format("{0}", GetTimeCommand()));                                                     // Time
                list.Add(String.Format("{0} {1}", CMD_CEI, CEI_ToString()));            // CEI

                list.Add(String.Format("{0} {1}", CMD_CWPON, CWPON_ToString()));        // CWPON
                list.Add(String.Format("{0} {1}", CMD_CWPBB, CWPBB_ToString()));        // CWBB
                list.Add(String.Format("{0} {1}", CMD_CWPBL, CWPBL));                   // CWPBL
                list.Add(String.Format("{0} {1}", CMD_CWPBS, CWPBS));                   // CWPBS
                list.Add(String.Format("{0} {1}", CMD_CWPX, CWPX));                     // CWPX
                list.Add(String.Format("{0} {1}", CMD_CWPBN, CWPBN));                   // CWPBN
                list.Add(String.Format("{0} {1}", CMD_CWPP, CWPP));                     // CWPP
                list.Add(String.Format("{0} {1}", CMD_CWPAI, CWPAI_ToString()));        // CWPAI
                list.Add(String.Format("{0} {1}", CMD_CWPTBP, CWPTBP));                 // CWPTBP

                list.Add(String.Format("{0} {1}", CMD_CBTON, CBTON_ToString()));        // CBTON
                list.Add(String.Format("{0} {1}", CMD_CBTBB, CBTBB_ToString()));        // CBTBB
                list.Add(String.Format("{0} {1}", CMD_CBTBL, CBTBL));                   // CBTBL
                list.Add(String.Format("{0} {1}", CMD_CBTMX, CBTMX));                   // CBTMX
                list.Add(String.Format("{0} {1}", CMD_CBTTBP, CBTTBP));                 // CBTTBP

                list.Add(String.Format("{0} {1}", CMD_CWTON, CWTON_ToString()));        // CWTON
                list.Add(String.Format("{0} {1}", CMD_CWTBB, CWTBB_ToString()));        // CWTBB
                list.Add(String.Format("{0} {1}", CMD_CWTBL, CWTBL));                   // CWTBL
                list.Add(String.Format("{0} {1}", CMD_CWTBS, CWTBS));                   // CWTBS
                list.Add(String.Format("{0} {1}", CMD_CWTTBP, CWTTBP));                 // CWTTBP

                list.Add(String.Format("{0} {1}", CMD_CWS, CWS));                       // CWS
                list.Add(String.Format("{0} {1}", CMD_CWT, CWT));                       // CWT
                list.Add(String.Format("{0} {1}", CMD_CTD, CTD));                       // CTD
                list.Add(String.Format("{0} {1}", CMD_CWSS, CWSS));                     // CWSS
                list.Add(String.Format("{0} {1}", CMD_CHS, CHS_ToString()));            // CHS
                list.Add(String.Format("{0} {1}", CMD_CHO, CHO));                       // CHO

                list.Add(String.Format("{0} {1}", CMD_C232B, ((int)C232B).ToString())); // C232B
                list.Add(String.Format("{0} {1}", CMD_C485B, ((int)C485B).ToString())); // C485B

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

                s += Mode_ToString();
                s += GetTimeCommand();
                s += CMD_CEI + " " + CEI_ToString();

                s += CMD_CWPON + " " + CWPON_ToString();
                s += CMD_CWPBB + " " + CWPBB_ToString();
                s += CMD_CWPBL + " " + CWPBL;
                s += CMD_CWPBS + " " + CWPBS;
                s += CMD_CWPX + " " + CWPX;
                s += CMD_CWPBN + " " + CWPBN;
                s += CMD_CWPP + " " + CWPP;
                s += CMD_CWPAI + " " + CWPAI_ToString();
                s += CMD_CWPTBP + " " + CWPTBP;

                s += CMD_CBTON + " " + CBTON_ToString();
                s += CMD_CBTBB + " " + CBTBB_ToString();
                s += CMD_CBTBL + " " + CBTBL;
                s += CMD_CBTMX + " " + CBTMX;
                s += CMD_CBTTBP + " " + CBTTBP;

                s += CMD_CWTON + " " + CWTON_ToString();
                s += CMD_CWTBB + " " + CWTBB_ToString();
                s += CMD_CWTBL + " " + CWTBL;
                s += CMD_CWTBS + " " + CWTBS;
                s += CMD_CWTTBP + " " + CWTTBP;

                s += CMD_CWS + " " + CWS;
                s += CMD_CWT + " " + CWT;
                s += CMD_CTD + " " + CTD;
                s += CMD_CWSS + " " + CWSS;
                s += CMD_CHS + " " + CHS_ToString();
                s += CMD_CHO + " " + CHO;

                s += CMD_C232B + " " + ((int)C232B).ToString();
                s += CMD_C485B + " " + ((int)C485B).ToString(); 

                return s;
            }

            #endregion

        }
    }
}