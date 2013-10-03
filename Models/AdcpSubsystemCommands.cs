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
 * 02/01/2012      RC          1.14       Initial coding
 * 02/07/2012      RC          2.00       Added a method SetDefaults().
 *                                         Removed the CWPAI extra properties.
 *                                         Added method IsEnableCWPAI().
 * 02/10/2012      RC          2.02       Changed default values to match Adcp User Guide Rev F and CDEFAULT in firmware v0.2.04.
 * 08/29/2012      RC          2.14       Added an default constructor.  This constructor is used when default values are not needed.
 *                                         Check in SetFrequencyDefaults() if Subsystem is not null before setting the frequency default settings.
 * 08/29/2012      RC          2.15       Changed how defaults are set to allow outside users to set the defaults with and without a subsystem set.
 * 09/06/2012      RC          2.15       Update the commands to the ADCP manual Revision G.
 * 09/10/2012      RC          2.15       Make 300Khz the default frequency if one is not set.
 *                                         Bug in CBTMX command firmware.  Commented out so the user cannot send the setting for now.
 * 09/26/2012      RC          2.15       Validate the values when settings.
 *                                         Added CWPAP command.
 * 10/03/2012      RC          2.15       Added CEPO Index property.  Give the CEPO index in the constructor.
 * 10/04/2012      RC          2.15       Added CBI command.
 * 10/10/2012      RC          2.15       When creating the command list, ensure the string is set to United States English format.  This is to prevent commas from being used for decimal points.
 * 10/16/2012      RC          2.15       Validate the min and max values for the commands.
 * 10/17/2012      RC          2.15       Added Min/Max values for the CWPAP commands.
 * 11/20/2012      RC          2.16       Created CmdStr to output the command string for reach command with the command, CEPO index and the values.
 * 12/20/2012      RC          2.17       Updated comments to ADCP User Guide Rev H.
 *                                         Added 2 new WP broadband pulse types based off ADCP User Guide Rev H.
 * 12/27/2012      RC          2.17       Replaced Subsystem.Empty with Subsystem.IsEmpty().
 * 05/30/2013      RC          2.19       Replaced Subsystem and CepoIndex with SubsystemConfig which contains this information.
 * 06/11/2013      RC          2.19       Added CBI_BurstPairFlag.
 * 06/14/2013      RC          2.19       Added GetDeploymentCommandList().
 * 09/17/2013      RC          2.20.0     Updated CWPBB_TransmitPulseType options to user guide rev N.
 *                                         Removed the CBI command from the GetDeploymentCommandList().
 *
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
namespace RTI
{
    namespace Commands
    {

        /// <summary>
        /// Each subsystem will be a seperate transducer.  Each
        /// transducer will have its own settings.  This will be
        /// the settings for one transducer.
        /// </summary>
        public class AdcpSubsystemCommands
        {
            #region Commands

            #region Water Profile

            /// <summary>
            /// Water Profile Command: On/Off
            /// Paramater: N = 0 to 1
            /// 0=Disable 1=Enable. 
            /// Enables or disables water profile pings.
            /// </summary>
            public const string CMD_CWPON = "CWPON";

            /// <summary>
            /// Water Profile Command: Water Profile BroadBand.
            /// Parameter: N = 0 to 1
            /// 0=Disable 1=Enable. 
            /// Enables or disables Water Profile boardband coded pulse transmission
            /// and lag.
            /// </summary>
            public const string CMD_CWPBB = "CWPBB";

            /// <summary>
            /// Water Ambiguity Ping.
            /// Used when CWPBB = 5.
            /// Pulse to Pulse ping and processing is used for the
            /// ambiguity resolver therefore (Blank + Bin Size) less than Lag
            /// Parameters:
            /// 1. 0 to 100 sets the number of pings that will be averaged together.
            /// 2. Lag (meters) sets the length of the lag.
            /// 3. Blank (meters) sets the starting position of the bin.
            /// 4. Bin Size (meters).
            /// 5. Time Between Ambiguity Pings (seconds).
            /// </summary>
            public const string CMD_CWPAP = "CWPAP";

            /// <summary>
            /// Water Profile Command: Water Profile Water Base Pings.
            /// Parameter: n, t.t
            /// n = 0 to 100.  Sets the number of pings that will be averaged together during each CWPP ping.
            /// t.t = Time in seconds between the base pings.
            /// </summary>
            public const string CMD_CWPBP = "CWPBP";

            /// <summary>
            /// Water Profile Screening Thresholds.
            /// Parameter: Correlation Threshold, Q Velocity Threshold, V Velocity Threshold.
            /// Screen the water profile data.
            /// </summary>
            public const string CMD_CWPST = "CWPST";

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
            /// Burst Inteval Comand
            /// Sets the Burst Interval values.
            /// Parameter: HH:MM:SS.hh,n
            /// HH:MM:SS.hh = Interval for the CBI to go off.
            /// n = Number of ensembles to collect in a burst.
            /// </summary>
            public const string CMD_CBI = "CBI";

            #endregion

            #region Bottom Track

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
            /// Bottom Track Command: Bottom Track Screening Thresholds.
            /// Parameters: Correlation Threshold, Q Velocity Threshold, V Velocity Threshold.
            /// </summary>
            public const string CMD_CBTST = "CBTST";

            /// <summary>
            /// Bottom Track Command: Bottom Track Thresholds.
            /// Parameters: Shallow Detection Threshold, Depth Switch SNR, SNR deep detection threshold, Depth Switch Gain
            /// </summary>
            public const string CMD_CBTT = "CBTT";

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

            #endregion

            #region Water Track

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

            #endregion

            #endregion

            #region Default Values

            /// <summary>
            /// Default CEPO index.  If there is only 1 subsystem and 1 configuration,
            /// then the CEPO index would be 0.
            /// </summary>
            public const int DEFAULT_CEPO_INDEX = 0;

            /// <summary>
            /// Base Lag Length.  This is used for the 1200kHz system.
            /// To get the Lag Length for other frequencies, multiple by
            /// the scale factor for each frequency (RTI.Core.Commons.FREQ_DIV_1200/600/300/150...) 
            /// vs the 1200kHz base frequency.
            /// </summary>
            public const float DEFAULT_LAG_LENGTH_BASE = 0.042f;

            /// <summary>
            /// Base Water Base Ping time in seconds.  This is used for the 1200kHz
            /// system.  To get the Base Ping time for other frequencies, multiple
            /// by the scale factor for each frequency (RTI.Core.Commons.FREQ_DIV_1200/600/300/150...) 
            /// vs the 1200kHz base frequency.
            /// </summary>
            public const float DEFAULT_WATER_BASE_PING_TIME_BASE = 0.02f;

            /// <summary>
            /// Base Depth (m) at which the Bottom Track switches from using
            /// the shallow to the deep SNR for the 1200kHz system.
            /// To get the depth for other frequencies, multiple
            /// by the scale factor for each frequency (RTI.Core.Commons.FREQ_DIV_1200/600/300/150...) 
            /// vs the 1200kHz base frequency.
            /// </summary>
            public const float DEFAULT_DEPTH_SWITCH_SNR_BASE = 25.0f;

            /// <summary>
            /// Base Depth (m) at which the Bottom Track switches from low 
            /// to high Gain receive for the 1200kHz system.
            /// To get the depth for other frequencies, multiple
            /// by the scale factor for each frequency (RTI.Core.Commons.FREQ_DIV_1200/600/300/150...) 
            /// vs the 1200kHz base frequency.
            /// </summary>
            public const float DEFAULT_DEPTH_SWITCH_GAIN_BASE = 2.0f;

            /// <summary>
            /// Default number of ensembles for CBI command.
            /// Also the value to disable the command.
            /// </summary>
            public const UInt16 DEFAULT_CBI_NUM_ENS = 0;

            /// <summary>
            /// Default value for CBI_BurstPairFlag.
            /// </summary>
            public const bool DEFAULT_CBI_BURST_PAIR_FLAG = false;

            #region Water Profile

            /// <summary>
            /// Default Water Profile On.
            /// </summary>
            public const bool DEFAULT_CWPON = true;

            /// <summary>
            /// Default Water Profile Broadband Transmit Pulse Type.
            /// </summary>
            public const eCWPBB_TransmitPulseType DEFAULT_CWPBB_TRANSMITPULSETYPE = eCWPBB_TransmitPulseType.BROADBAND;

            /// <summary>
            /// Default Water Profile Screening Threshold Correlation Threshold value.
            /// </summary>
            public const float DEFAULT_CWPST_CORR_THRESH = 0.400f;

            /// <summary>
            /// Default Water Profile Screening Threshold Q Velocity Threshold value.
            /// </summary>
            public const float DEFAULT_CWPST_QVEL_THRESH = 1.000f;

            /// <summary>
            /// Default Water Profile Screening Threshold V Velocity Threshold value.
            /// </summary>
            public const float DEFAULT_CWPST_VVEL_THRESH = 1.000f;

            /// <summary>
            /// Default Water Profile Water Base Pings Number of Pings averaged together.
            /// </summary>
            public const int DEFAULT_CWPBP_NUM_PING_AVG = 1;

            #endregion

            #region Bottom Track

            /// <summary>
            /// Default Bottom Track On.
            /// </summary>
            public const bool DEFAULT_CBTON = true;

            /// <summary>
            /// Default Bottom Track Broadband Transmit Mode.
            /// </summary>
            public const eCBTBB_Mode DEFAULT_CBTBB_MODE = eCBTBB_Mode.NARROWBAND_LONG_RANGE;

            /// <summary>
            /// Default Bottom Track Broadband Pulse To Pulse Lag in meters.
            /// </summary>
            public const float DEFAULT_CBTBB_PULSETOPULSE_LAG = 0.00f;

            /// <summary>
            /// Default Bottom Track Correlation Threshold.
            /// </summary>
            public const float DEFAULT_CBTST_CORR_THRESH = 0.900f;

            /// <summary>
            /// Default Bottom Track Q Velocity Threshold in meters per second.
            /// </summary>
            public const float DEFAULT_CBTST_QVEL_THRESHOLD = 1.000f;

            /// <summary>
            /// Default Bottom Track V Velocity Threshold in meters per second.
            /// </summary>
            public const float DEFAULT_CBTST_VVEL_THRESHOLD = 1.000f;

            /// <summary>
            /// Default Bottom Track Thresholds SNR (dB) shallow detection threshold.
            /// </summary>
            public const float DEFAULT_CBTT_SNR_SHALLOW_DET_THRESHOLD = 15.0f;

            /// <summary>
            /// Default Bottom Track Thresholds SNR (dB) deep detection threshold.
            /// </summary>
            public const float DEFAULT_CBTT_SNR_DEEP_DET_THRESHOLD = 5;

            #endregion

            #region Water Track

            /// <summary>
            /// Default Water On.
            /// </summary>
            public const bool DEFAULT_CWTON = false;

            /// <summary>
            /// Default Water Track Broadband.
            /// </summary>
            public const bool DEFAULT_CWTBB = false;

            #endregion

            #region Default 38 kHz

            /// <summary>
            /// Default 40khz Bottom Track Time between pings (2 seconds).
            /// </summary>
            public const float DEFAULT_38_CBTTBP = 2.0f;

            /// <summary>
            /// Default 40khz Water Track Time between pings (4 seconds).
            /// </summary>
            public const float DEFAULT_38_CWTTBP = 4.0f;

            /// <summary>
            /// Default 40khz Water Profile Time between pings (4 seconds).
            /// </summary>
            public const float DEFAULT_38_CWPTBP = 4.0f;

            /// <summary>
            /// Default 40khz Bottom Track Blank (2 meters).
            /// </summary>
            public const float DEFAULT_38_CBTBL = 2.0f;

            /// <summary>
            /// Default 40khz Bottom Track Maximum Depth (2000 meters).
            /// </summary>
            public const float DEFAULT_38_CBTMX = 2000.0f;

            /// <summary>
            /// Default 40khz Water Track Blank (32 meters).
            /// </summary>
            public const float DEFAULT_38_CWTBL = 32.0f;

            /// <summary>
            /// Default 40khz Water Track Bin size (32 meters).
            /// </summary>
            public const float DEFAULT_38_CWTBS = 32.0f;

            /// <summary>
            /// Default 40khz Water Profile Blank (32 meters).
            /// </summary>
            public const float DEFAULT_38_CWPBL = 32.0f;

            /// <summary>
            /// Default 40khz Water Profile Bin size (32 meters).
            /// </summary>
            public const float DEFAULT_38_CWPBS = 32.0f;

            /// <summary>
            /// Default 40khz Water Profile Transmit (0.00 = Same as cell size).
            /// </summary>
            public const float DEFAULT_38_CWPX = 0.00f;

            /// <summary>
            /// Default 40khz Water Profile Number of bins (30 bins).
            /// </summary>
            public const int DEFAULT_38_CWPBN = 30;

            /// <summary>
            /// Default 40khz Water Profile Number of pings (1 pings averaged per ensemble).
            /// </summary>
            public const int DEFAULT_38_CWPP = 1;

            /// <summary>
            /// Default 20kHz Water Profile Broadband Lag Length.  Lag Length is based off the lag length base
            /// and the frequency divsor.
            /// </summary>
            public const float DEFAULT_38_CWPBB_LAGLENGTH = DEFAULT_LAG_LENGTH_BASE * RTI.Core.Commons.FREQ_DIV_38;

            /// <summary>
            /// Default 38kHz Water Profile Water Base Ping Time.  Water Base Ping time is based off the
            /// Water Base Ping base value and the frequency divisor.
            /// </summary>
            public const float DEFAULT_38_CWPBP_WATER_BASE_PING_TIME = DEFAULT_WATER_BASE_PING_TIME_BASE * RTI.Core.Commons.FREQ_DIV_38;

            /// <summary>
            /// Default 38kHz Water Profile Bottom Track Broadband Long Range Depth.
            /// </summary>
            public const float DEFAULT_38_CBTBB_LONGRANGEDEPTH = 1000.0f;

            /// <summary>
            /// Default 38kHz Bottom Track Thresholds Depth (m) at which the bottom Track switches from using the
            /// shallow to the deep SNR.
            /// </summary>
            public const float DEFAULT_38_CBTT_DEPTH_SNR = DEFAULT_DEPTH_SWITCH_SNR_BASE * RTI.Core.Commons.FREQ_DIV_38;

            /// <summary>
            /// Default 38kHz Bottom Track Threshold Depth (m) at which the Bottom Track switches from low to high
            /// gain receive.
            /// </summary>
            public const float DEFAULT_38_CBTT_DEPTH_GAIN = DEFAULT_DEPTH_SWITCH_GAIN_BASE * RTI.Core.Commons.FREQ_DIV_38;

            /// <summary>
            /// Default 38kHz Water Profile Water Ambiguity Ping Number of pings that will be averaged together.
            /// </summary>
            public const int DEFAULT_38_CWPAP_NUMOFPINGSAVG = 0;

            /// <summary>
            /// Default 38kHz Water Profile Water Ambiguity Ping Lag (meters).
            /// </summary>
            public const float DEFAULT_38_CWPAP_LAG = 0.00f;

            /// <summary>
            /// Default 38kHz Water Profile Water Ambiguity Ping Blank (meters).
            /// </summary>
            public const float DEFAULT_38_CWPAP_BLANK = 0.00f;

            /// <summary>
            /// Default 38kHz Water Profile Water Ambiguity Ping Bin Size (meters).
            /// </summary>
            public const float DEFAULT_38_CWPAP_BINSIZE = 0.00f;

            /// <summary>
            /// Default 38kHz Water Profile Water Ambiguity Ping Time Between Pings (seconds).
            /// </summary>
            public const float DEFAULT_38_CWPAP_TIME_BETWEEN_PINGS = 0.00f;

            #endregion

            #region Default 75 kHz
            /// <summary>
            /// Default 80khz Bottom Track Time between pings (1 seconds).
            /// </summary>
            public const float DEFAULT_75_CBTTBP = 1.0f;

            /// <summary>
            /// Default 80khz Water Track Time between pings (2 seconds).
            /// </summary>
            public const float DEFAULT_75_CWTTBP = 2.0f;

            /// <summary>
            /// Default 80khz Water Profile Time between pings (2 seconds).
            /// </summary>
            public const float DEFAULT_75_CWPTBP = 2.0f;

            /// <summary>
            /// Default 80khz Bottom Track Blank (1 meters).
            /// </summary>
            public const float DEFAULT_75_CBTBL = 1.0f;

            /// <summary>
            /// Default 80khz Bottom Track Maximum Depth (1000 meters).
            /// </summary>
            public const float DEFAULT_75_CBTMX = 1000.0f;

            /// <summary>
            /// Default 80khz Water Track Blank (16 meters).
            /// </summary>
            public const float DEFAULT_75_CWTBL = 16.0f;

            /// <summary>
            /// Default 80khz Water Track Bin size (16 meters).
            /// </summary>
            public const float DEFAULT_75_CWTBS = 16.0f;

            /// <summary>
            /// Default 80khz Water Profile Blank (16 meters).
            /// </summary>
            public const float DEFAULT_75_CWPBL = 16.0f;

            /// <summary>
            /// Default 80khz Water Profile Bin size (16 meters).
            /// </summary>
            public const float DEFAULT_75_CWPBS = 16.0f;

            /// <summary>
            /// Default 80khz Water Profile Transmit (0.00 = Same as cell size).
            /// </summary>
            public const float DEFAULT_75_CWPX = 0.00f;

            /// <summary>
            /// Default 80khz Water Profile Number of bins (30 bins).
            /// </summary>
            public const int DEFAULT_75_CWPBN = 30;

            /// <summary>
            /// Default 80khz Water Profile Number of pings (1 pings averaged per ensemble).
            /// </summary>
            public const int DEFAULT_75_CWPP = 1;

            /// <summary>
            /// Default 80kHz Water Profile Broadband Lag Length.  Lag Length is based off the lag length base
            /// and the frequency divsor.
            /// </summary>
            public const float DEFAULT_75_CWPBB_LAGLENGTH = DEFAULT_LAG_LENGTH_BASE * RTI.Core.Commons.FREQ_DIV_75;

            /// <summary>
            /// Default 80kHz Water Profile Water Base Ping Time.  Water Base Ping time is based off the
            /// Water Base Ping base value and the frequency divisor.
            /// </summary>
            public const float DEFAULT_75_CWPBP_WATER_BASE_PING_TIME = DEFAULT_WATER_BASE_PING_TIME_BASE * RTI.Core.Commons.FREQ_DIV_75;

            /// <summary>
            /// Default 80kHz Water Profile Bottom Track Broadband Long Range Depth.
            /// </summary>
            public const float DEFAULT_75_CBTBB_LONGRANGEDEPTH = 600.0f;

            /// <summary>
            /// Default 80kHz Bottom Track Thresholds Depth (m) at which the bottom Track switches from using the
            /// shallow to the deep SNR.
            /// </summary>
            public const float DEFAULT_75_CBTT_DEPTH_SNR = DEFAULT_DEPTH_SWITCH_SNR_BASE * RTI.Core.Commons.FREQ_DIV_75;

            /// <summary>
            /// Default 80kHz Bottom Track Threshold Depth (m) at which the Bottom Track switches from low to high
            /// gain receive.
            /// </summary>
            public const float DEFAULT_75_CBTT_DEPTH_GAIN = DEFAULT_DEPTH_SWITCH_GAIN_BASE * RTI.Core.Commons.FREQ_DIV_75;

            /// <summary>
            /// Default 80kHz Water Profile Water Ambiguity Ping Number of pings that will be averaged together.
            /// </summary>
            public const int DEFAULT_75_CWPAP_NUMOFPINGSAVG = 0;

            /// <summary>
            /// Default 80kHz Water Profile Water Ambiguity Ping Lag (meters).
            /// </summary>
            public const float DEFAULT_75_CWPAP_LAG = 0.00f;

            /// <summary>
            /// Default 80kHz Water Profile Water Ambiguity Ping Blank (meters).
            /// </summary>
            public const float DEFAULT_75_CWPAP_BLANK = 0.00f;

            /// <summary>
            /// Default 80kHz Water Profile Water Ambiguity Ping Bin Size (meters).
            /// </summary>
            public const float DEFAULT_75_CWPAP_BINSIZE = 0.00f;

            /// <summary>
            /// Default 80kHz Water Profile Water Ambiguity Ping Time Between Pings (seconds).
            /// </summary>
            public const float DEFAULT_75_CWPAP_TIME_BETWEEN_PINGS = 0.00f;

            #endregion

            #region Default 150 kHz
            /// <summary>
            /// Default 160khz Bottom Track Time between pings (0.5 seconds).
            /// </summary>
            public const float DEFAULT_150_CBTTBP = 0.5f;

            /// <summary>
            /// Default 160khz Water Track Time between pings (1 seconds).
            /// </summary>
            public const float DEFAULT_150_CWTTBP = 1.0f;

            /// <summary>
            /// Default 160khz Water Profile Time between pings (1 seconds).
            /// </summary>
            public const float DEFAULT_150_CWPTBP = 1.0f;

            /// <summary>
            /// Default 160khz Bottom Track Blank (0.5 meters).
            /// </summary>
            public const float DEFAULT_150_CBTBL = 0.5f;

            /// <summary>
            /// Default 160khz Bottom Track Maximum Depth (500 meters).
            /// </summary>
            public const float DEFAULT_150_CBTMX = 500.0f;

            /// <summary>
            /// Default 160khz Water Track Blank (8 meters).
            /// </summary>
            public const float DEFAULT_150_CWTBL = 8.0f;

            /// <summary>
            /// Default 160khz Water Track Bin size (8 meters).
            /// </summary>
            public const float DEFAULT_150_CWTBS = 8.0f;

            /// <summary>
            /// Default 160khz Water Profile Blank (8 meters).
            /// </summary>
            public const float DEFAULT_150_CWPBL = 8.0f;

            /// <summary>
            /// Default 160khz Water Profile Bin size (8 meters).
            /// </summary>
            public const float DEFAULT_150_CWPBS = 8.0f;

            /// <summary>
            /// Default 160khz Water Profile Transmit (0.00 = Same as cell size).
            /// </summary>
            public const float DEFAULT_150_CWPX = 0.00f;

            /// <summary>
            /// Default 160khz Water Profile Number of bins (30 bins).
            /// </summary>
            public const int DEFAULT_150_CWPBN = 30;

            /// <summary>
            /// Default 160khz Water Profile Number of pings (2 pings averaged per ensemble).
            /// </summary>
            public const int DEFAULT_150_CWPP = 1;

            /// <summary>
            /// Default 160kHz Water Profile Broadband Lag Length.  Lag Length is based off the lag length base
            /// and the frequency divsor.
            /// </summary>
            public const float DEFAULT_150_CWPBB_LAGLENGTH = DEFAULT_LAG_LENGTH_BASE * RTI.Core.Commons.FREQ_DIV_150;

            /// <summary>
            /// Default 160kHz Water Profile Water Base Ping Time.  Water Base Ping time is based off the
            /// Water Base Ping base value and the frequency divisor.
            /// </summary>
            public const float DEFAULT_150_CWPBP_WATER_BASE_PING_TIME = DEFAULT_WATER_BASE_PING_TIME_BASE * RTI.Core.Commons.FREQ_DIV_150;

            /// <summary>
            /// Default 160kHz Water Profile Bottom Track Broadband Long Range Depth.
            /// </summary>
            public const float DEFAULT_150_CBTBB_LONGRANGEDEPTH = 300.0f;

            /// <summary>
            /// Default 160kHz Bottom Track Thresholds Depth (m) at which the bottom Track switches from using the
            /// shallow to the deep SNR.
            /// </summary>
            public const float DEFAULT_150_CBTT_DEPTH_SNR = DEFAULT_DEPTH_SWITCH_SNR_BASE * RTI.Core.Commons.FREQ_DIV_150;

            /// <summary>
            /// Default 160kHz Bottom Track Threshold Depth (m) at which the Bottom Track switches from low to high
            /// gain receive.
            /// </summary>
            public const float DEFAULT_150_CBTT_DEPTH_GAIN = DEFAULT_DEPTH_SWITCH_GAIN_BASE * RTI.Core.Commons.FREQ_DIV_150;

            /// <summary>
            /// Default 160kHz Water Profile Water Ambiguity Ping Number of pings that will be averaged together.
            /// </summary>
            public const int DEFAULT_150_CWPAP_NUMOFPINGSAVG = 0;

            /// <summary>
            /// Default 160kHz Water Profile Water Ambiguity Ping Lag (meters).
            /// </summary>
            public const float DEFAULT_150_CWPAP_LAG = 0.00f;

            /// <summary>
            /// Default 160kHz Water Profile Water Ambiguity Ping Blank (meters).
            /// </summary>
            public const float DEFAULT_150_CWPAP_BLANK = 0.00f;

            /// <summary>
            /// Default 160kHz Water Profile Water Ambiguity Ping Bin Size (meters).
            /// </summary>
            public const float DEFAULT_150_CWPAP_BINSIZE = 0.00f;

            /// <summary>
            /// Default 160kHz Water Profile Water Ambiguity Ping Time Between Pings (seconds).
            /// </summary>
            public const float DEFAULT_150_CWPAP_TIME_BETWEEN_PINGS = 0.00f;

            #endregion

            #region Default 300 kHz
            /// <summary>
            /// Default 320khz Bottom Track Time between pings (0.25 seconds).
            /// </summary>
            public const float DEFAULT_300_CBTTBP = 0.25f;

            /// <summary>
            /// Default 320khz Water Track Time between pings (0.5 seconds).
            /// </summary>
            public const float DEFAULT_300_CWTTBP = 0.5f;

            /// <summary>
            /// Default 320khz Water Profile Time between pings (0.5 seconds).
            /// </summary>
            public const float DEFAULT_300_CWPTBP = 0.5f;

            /// <summary>
            /// Default 320khz Bottom Track Blank (0.25 meters).
            /// </summary>
            public const float DEFAULT_300_CBTBL = 0.25f;

            /// <summary>
            /// Default 320khz Bottom Track Maximum Depth (250 meters).
            /// </summary>
            public const float DEFAULT_300_CBTMX = 250.0f;

            /// <summary>
            /// Default 320khz Water Track Blank (4 meters).
            /// </summary>
            public const float DEFAULT_300_CWTBL = 4.0f;

            /// <summary>
            /// Default 320khz Water Track Bin size (4 meters).
            /// </summary>
            public const float DEFAULT_300_CWTBS = 4.0f;

            /// <summary>
            /// Default 320khz Water Profile Blank (4 meters).
            /// </summary>
            public const float DEFAULT_300_CWPBL = 4.0f;

            /// <summary>
            /// Default 320khz Water Profile Bin size (4 meters).
            /// </summary>
            public const float DEFAULT_300_CWPBS = 4.0f;

            /// <summary>
            /// Default 320khz Water Profile Transmit (0.00 = Same as cell size).
            /// </summary>
            public const float DEFAULT_300_CWPX = 0.00f;

            /// <summary>
            /// Default 320khz Water Profile Number of bins (30 bins).
            /// </summary>
            public const int DEFAULT_300_CWPBN = 30;

            /// <summary>
            /// Default 320khz Water Profile Number of pings (1 pings averaged per ensemble).
            /// </summary>
            public const int DEFAULT_300_CWPP = 1;

            /// <summary>
            /// Default 320kHz Water Profile Broadband Lag Length.  Lag Length is based off the lag length base
            /// and the frequency divsor.
            /// </summary>
            public const float DEFAULT_300_CWPBB_LAGLENGTH = DEFAULT_LAG_LENGTH_BASE * RTI.Core.Commons.FREQ_DIV_300;

            /// <summary>
            /// Default 320kHz Water Profile Water Base Ping Time.  Water Base Ping time is based off the
            /// Water Base Ping base value and the frequency divisor.
            /// </summary>
            public const float DEFAULT_300_CWPBP_WATER_BASE_PING_TIME = DEFAULT_WATER_BASE_PING_TIME_BASE * RTI.Core.Commons.FREQ_DIV_300;

            /// <summary>
            /// Default 320kHz Water Profile Bottom Track Broadband Long Range Depth.
            /// </summary>
            public const float DEFAULT_300_CBTBB_LONGRANGEDEPTH = 150.0f;

            /// <summary>
            /// Default 320kHz Bottom Track Thresholds Depth (m) at which the bottom Track switches from using the
            /// shallow to the deep SNR.
            /// </summary>
            public const float DEFAULT_300_CBTT_DEPTH_SNR = DEFAULT_DEPTH_SWITCH_SNR_BASE * RTI.Core.Commons.FREQ_DIV_300;

            /// <summary>
            /// Default 320kHz Bottom Track Threshold Depth (m) at which the Bottom Track switches from low to high
            /// gain receive.
            /// </summary>
            public const float DEFAULT_300_CBTT_DEPTH_GAIN = DEFAULT_DEPTH_SWITCH_GAIN_BASE * RTI.Core.Commons.FREQ_DIV_300;

            /// <summary>
            /// Default 320kHz Water Profile Water Ambiguity Ping Number of pings that will be averaged together.
            /// </summary>
            public const int DEFAULT_300_CWPAP_NUMOFPINGSAVG = 0;

            /// <summary>
            /// Default 320kHz Water Profile Water Ambiguity Ping Lag (meters).
            /// </summary>
            public const float DEFAULT_300_CWPAP_LAG = 0.00f;

            /// <summary>
            /// Default 320kHz Water Profile Water Ambiguity Ping Blank (meters).
            /// </summary>
            public const float DEFAULT_300_CWPAP_BLANK = 0.00f;

            /// <summary>
            /// Default 320kHz Water Profile Water Ambiguity Ping Bin Size (meters).
            /// </summary>
            public const float DEFAULT_300_CWPAP_BINSIZE = 0.00f;

            /// <summary>
            /// Default 320kHz Water Profile Water Ambiguity Ping Time Between Pings (seconds).
            /// </summary>
            public const float DEFAULT_300_CWPAP_TIME_BETWEEN_PINGS = 0.00f;

            #endregion

            #region Default 600 kHz
            /// <summary>
            /// Default 640khz Bottom Track Time between pings (0.125 seconds).
            /// </summary>
            public const float DEFAULT_600_CBTTBP = 0.125f;

            /// <summary>
            /// Default 640khz Water Track Time between pings (0.25 seconds).
            /// </summary>
            public const float DEFAULT_600_CWTTBP = 0.25f;

            /// <summary>
            /// Default 640khz Water Profile Time between pings (0.25 seconds).
            /// </summary>
            public const float DEFAULT_600_CWPTBP = 0.25f;

            /// <summary>
            /// Default 640khz Bottom Track Blank (0.125 meters).
            /// </summary>
            public const float DEFAULT_600_CBTBL = 0.125f;

            /// <summary>
            /// Default 640khz Bottom Track Maximum Depth (125 meters).
            /// </summary>
            public const float DEFAULT_600_CBTMX = 125.0f;

            /// <summary>
            /// Default 640khz Water Track Blank (2 meters).
            /// </summary>
            public const float DEFAULT_600_CWTBL = 2.0f;

            /// <summary>
            /// Default 640khz Water Track Bin size (2 meters).
            /// </summary>
            public const float DEFAULT_600_CWTBS = 2.0f;

            /// <summary>
            /// Default 640khz Water Profile Blank (2 meters).
            /// </summary>
            public const float DEFAULT_600_CWPBL = 2.0f;

            /// <summary>
            /// Default 640khz Water Profile Bin size (2 meters).
            /// </summary>
            public const float DEFAULT_600_CWPBS = 2.0f;

            /// <summary>
            /// Default 640khz Water Profile Transmit (0.00 = Same as cell size).
            /// </summary>
            public const float DEFAULT_600_CWPX = 0.00f;

            /// <summary>
            /// Default 640khz Water Profile Number of bins (30 bins).
            /// </summary>
            public const int DEFAULT_600_CWPBN = 30;

            /// <summary>
            /// Default 640khz Water Profile Number of pings (1 pings averaged per ensemble).
            /// </summary>
            public const int DEFAULT_600_CWPP = 1;

            /// <summary>
            /// Default 640kHz Water Profile Broadband Lag Length.  Lag Length is based off the lag length base
            /// and the frequency divsor.
            /// </summary>
            public const float DEFAULT_600_CWPBB_LAGLENGTH = DEFAULT_LAG_LENGTH_BASE * RTI.Core.Commons.FREQ_DIV_600;

            /// <summary>
            /// Default 640kHz Water Profile Water Base Ping Time.  Water Base Ping time is based off the
            /// Water Base Ping base value and the frequency divisor.
            /// </summary>
            public const float DEFAULT_600_CWPBP_WATER_BASE_PING_TIME = DEFAULT_WATER_BASE_PING_TIME_BASE * RTI.Core.Commons.FREQ_DIV_600;

            /// <summary>
            /// Default 640kHz Water Profile Bottom Track Broadband Long Range Depth.
            /// </summary>
            public const float DEFAULT_600_CBTBB_LONGRANGEDEPTH = 50.0f;

            /// <summary>
            /// Default 640kHz Bottom Track Thresholds Depth (m) at which the bottom Track switches from using the
            /// shallow to the deep SNR.
            /// </summary>
            public const float DEFAULT_600_CBTT_DEPTH_SNR = DEFAULT_DEPTH_SWITCH_SNR_BASE * RTI.Core.Commons.FREQ_DIV_600;

            /// <summary>
            /// Default 640kHz Bottom Track Threshold Depth (m) at which the Bottom Track switches from low to high
            /// gain receive.
            /// </summary>
            public const float DEFAULT_600_CBTT_DEPTH_GAIN = DEFAULT_DEPTH_SWITCH_GAIN_BASE * RTI.Core.Commons.FREQ_DIV_600;

            /// <summary>
            /// Default 640kHz Water Profile Water Ambiguity Ping Number of pings that will be averaged together.
            /// </summary>
            public const int DEFAULT_600_CWPAP_NUMOFPINGSAVG = 0;

            /// <summary>
            /// Default 640kHz Water Profile Water Ambiguity Ping Lag (meters).
            /// </summary>
            public const float DEFAULT_600_CWPAP_LAG = 0.00f;

            /// <summary>
            /// Default 640kHz Water Profile Water Ambiguity Ping Blank (meters).
            /// </summary>
            public const float DEFAULT_600_CWPAP_BLANK = 0.00f;

            /// <summary>
            /// Default 640kHz Water Profile Water Ambiguity Ping Bin Size (meters).
            /// </summary>
            public const float DEFAULT_600_CWPAP_BINSIZE = 0.00f;

            /// <summary>
            /// Default 640kHz Water Profile Water Ambiguity Ping Time Between Pings (seconds).
            /// </summary>
            public const float DEFAULT_600_CWPAP_TIME_BETWEEN_PINGS = 0.00f;

            #endregion

            #region Default 1.2 MHz
            /// <summary>
            /// Default 1.2 MHz Bottom Track Time between pings (0.05 seconds).
            /// </summary>
            public const float DEFAULT_1200_CBTTBP = 0.05f;

            /// <summary>
            /// Default 1.2 MHz Water Track Time between pings (0.1 seconds).
            /// </summary>
            public const float DEFAULT_1200_CWTTBP = 0.1f;

            /// <summary>
            /// Default 1.2 MHz Water Profile Time between pings (0.1 seconds).
            /// </summary>
            public const float DEFAULT_1200_CWPTBP = 0.1f;

            /// <summary>
            /// Default 1.2 MHz Bottom Track Blank (0.1 meters).
            /// </summary>
            public const float DEFAULT_1200_CBTBL = 0.1f;

            /// <summary>
            /// Default 1.2 MHz Bottom Track Maximum Depth (50 meters).
            /// </summary>
            public const float DEFAULT_1200_CBTMX = 50.0f;

            /// <summary>
            /// Default 1.2 MHz Water Track Blank (2 meters).
            /// </summary>
            public const float DEFAULT_1200_CWTBL = 2.0f;

            /// <summary>
            /// Default 1.2 MHz Water Track Bin size (2 meters).
            /// </summary>
            public const float DEFAULT_1200_CWTBS = 2.0f;

            /// <summary>
            /// Default 1.2 MHz Water Profile Blank (1 meters).
            /// </summary>
            public const float DEFAULT_1200_CWPBL = 1.0f;

            /// <summary>
            /// Default 1.2 MHz Water Profile Bin size (1 meters).
            /// </summary>
            public const float DEFAULT_1200_CWPBS = 1.0f;

            /// <summary>
            /// Default 1.2 MHz Water Profile Transmit (0.00 = Same as cell size).
            /// </summary>
            public const float DEFAULT_1200_CWPX = 0.00f;

            /// <summary>
            /// Default 1.2 MHz Water Profile Number of bins (20 bins).
            /// </summary>
            public const int DEFAULT_1200_CWPBN = 20;

            /// <summary>
            /// Default 1.2 MHz Water Profile Number of pings (1 pings averaged per ensemble).
            /// </summary>
            public const int DEFAULT_1200_CWPP = 1;

            /// <summary>
            /// Default 1.2 MHz Water Profile Broadband Lag Length.  Lag Length is based off the lag length base
            /// and the frequency divisor.
            /// </summary>
            public const float DEFAULT_1200_CWPBB_LAGLENGTH = DEFAULT_LAG_LENGTH_BASE * RTI.Core.Commons.FREQ_DIV_1200;

            /// <summary>
            /// Default 1.2 MHz Water Profile Water Base Ping Time.  Water Base Ping time is based off the
            /// Water Base Ping base value and the frequency divisor.
            /// </summary>
            public const float DEFAULT_1200_CWPBP_WATER_BASE_PING_TIME = DEFAULT_WATER_BASE_PING_TIME_BASE * RTI.Core.Commons.FREQ_DIV_1200;

            /// <summary>
            /// Default 1.2 MHz Water Profile Bottom Track Broadband Long Range Depth.
            /// </summary>
            public const float DEFAULT_1200_CBTBB_LONGRANGEDEPTH = 30.0f;

            /// <summary>
            /// Default 1.2 MHz Bottom Track Thresholds Depth (m) at which the bottom Track switches from using the
            /// shallow to the deep SNR.
            /// </summary>
            public const float DEFAULT_1200_CBTT_DEPTH_SNR = DEFAULT_DEPTH_SWITCH_SNR_BASE * RTI.Core.Commons.FREQ_DIV_1200;

            /// <summary>
            /// Default 1.2 MHz Bottom Track Threshold Depth (m) at which the Bottom Track switches from low to high
            /// gain receive.
            /// </summary>
            public const float DEFAULT_1200_CBTT_DEPTH_GAIN = DEFAULT_DEPTH_SWITCH_GAIN_BASE * RTI.Core.Commons.FREQ_DIV_1200;

            /// <summary>
            /// Default 1.2 MHz Water Profile Water Ambiguity Ping Number of pings that will be averaged together.
            /// </summary>
            public const int DEFAULT_1200_CWPAP_NUMOFPINGSAVG = 10;

            /// <summary>
            /// Default 1.2 MHz Water Profile Water Ambiguity Ping Lag (meters).
            /// </summary>
            public const float DEFAULT_1200_CWPAP_LAG = 0.15f;

            /// <summary>
            /// Default 1.2 MHz Water Profile Water Ambiguity Ping Blank (meters).
            /// </summary>
            public const float DEFAULT_1200_CWPAP_BLANK = 0.06f;

            /// <summary>
            /// Default 1.2 MHz Water Profile Water Ambiguity Ping Bin Size (meters).
            /// </summary>
            public const float DEFAULT_1200_CWPAP_BINSIZE = 0.04f;

            /// <summary>
            /// Default 1.2 MHz Water Profile Water Ambiguity Ping Time Between Pings (seconds).
            /// </summary>
            public const float DEFAULT_1200_CWPAP_TIME_BETWEEN_PINGS = 0.01f;

            #endregion

            #endregion

            #region Min Max Values

            /// <summary>
            /// Minimum CWPBB Lag Length.
            /// </summary>
            public const float MIN_CWPBB_LAGLENGTH = 0.0f;

            /// <summary>
            /// Maximum CWPBB Lag Length.
            /// So far the 38kHz system has the greatest lag length.
            /// </summary>
            public const float MAX_CWPBB_LAGLENGTH = DEFAULT_38_CWPBB_LAGLENGTH;

            /// <summary>
            /// Minimum number of Pings averaged together in CWPAP.
            /// </summary>
            public const UInt16 MIN_CWPAP_NUMPINGS = 0;

            /// <summary>
            /// Maximum number of Pings averaged together in CWPAP.
            /// </summary>
            public const UInt16 MAX_CWPAP_NUMPINGS = 100;

            /// <summary>
            /// Minimum Lag in meters in CWPAP.
            /// </summary>
            public const float MIN_CWPAP_LAG = 0.0f;

            /// <summary>
            /// Maximum Lag in meters in CWPAP.
            /// Use the same max as CWPBB lag.
            /// </summary>
            public const float MAX_CWPAP_LAG = MAX_CWPBB_LAGLENGTH;

            /// <summary>
            /// Minimum Blank in meters in CWPAP.
            /// </summary>
            public const float MIN_CWPAP_BLANK = 0.0f;

            /// <summary>
            /// Maximum Blank in meters in CWPAP.
            /// Use the same max as CWPBL.
            /// </summary>
            public const float MAX_CWPAP_BLANK = MAX_CWPBL;

            /// <summary>
            /// Minimum Bin Size in meters in CWPAP.
            /// </summary>
            public const float MIN_CWPAP_BINSIZE = 0.0f;

            /// <summary>
            /// Maximum Bin Size in meters in CWPAP.
            /// Use the same max as CWPBS.
            /// </summary>
            public const float MAX_CWPAP_BINSIZE = MAX_CWPBS;

            /// <summary>
            /// Minimum time between pings in seconds in CWPAP.
            /// </summary>
            public const float MIN_CWPAP_TIME_BETWEEN_PING = 0.0f;

            /// <summary>
            /// Maximum time between pings in seconds in CWPAP.
            /// Use the max for CWPTBP.
            /// </summary>
            public const float MAX_CWPAP_TIME_BETWEEN_PING = MAX_CWPTBP;

            /// <summary>
            /// Mimimum CWPST Correlation Threshold value.
            /// </summary>
            public const float MIN_CWPST_CORR_THRESH = 0.0f;

            /// <summary>
            /// Maximum CWPST Correlation Threshold value.
            /// </summary>
            public const float MAX_CWPST_CORR_THRESH = 1.0f;

            /// <summary>
            /// Minimum CWPST Q Velocity Threshold value.
            /// </summary>
            public const float MIN_CWPST_Q_VELOCITY_THRESH = 0.0f;

            /// <summary>
            /// Minimum CWPST V Velocity Threshold value.
            /// </summary>
            public const float MIN_CWPST_V_VELOCITY_THRESH = 0.0f;

            /// <summary>
            /// Water Base Pings Number of pings to average togheter minimum.
            /// </summary>
            public const UInt16 MIN_CWPBP_NUM_PING = 0;

            /// <summary>
            /// Water Base Ping Minumim number of time between pings.
            /// </summary>
            public const float MIN_CWPBP_TIME_BETWEEN_PINGS = 0.0f;

            /// <summary>
            /// Water Base Ping Maximum number of time between pings.
            /// Use the same max as CWPTBP.
            /// </summary>
            public const float MAX_CWPBP_TIME_BETWEEN_PINGS = MAX_CWPTBP;

            /// <summary>
            /// Water Base Pings Number of pings to average togheter minimum.
            /// </summary>
            public const UInt16 MAX_CWPBP_NUM_PING = 100;

            /// <summary>
            /// Mimimum CWPST Correlation Threshold value.
            /// </summary>
            public const float MIN_CBTST_CORR_THRESH = 0.0f;

            /// <summary>
            /// Maximum CWPST Correlation Threshold value.
            /// </summary>
            public const float MAX_CBTST_CORR_THRESH = 1.0f;

            /// <summary>
            /// Minimum Q Velocity Threshold in CBTST.
            /// </summary>
            public const float MIN_CBTST_QVEL_THRESH = 0.0f;

            /// <summary>
            /// Minimum Q Velocity Threshold in CBTST.
            /// </summary>
            public const float MIN_CBTST_VVEL_THRESH = 0.0f;

            /// <summary>
            /// Minimum SNR Shallow Detection Threshold for CBTT in dB.
            /// </summary>
            public const float MIN_CBTT_SNR_SHALLOW_THRESH = 0.0f;

            /// <summary>
            /// Minimum depth for SNR switch in meters.
            /// </summary>
            public const float MIN_CBTT_DEPTH_SNR = 0.0f;

            /// <summary>
            /// Minimum SNR Deep Detection Threshold for CBTT in dB.
            /// </summary>
            public const float MIN_CBTT_SNR_DEEP_THRESH = 0.0f;

            /// <summary>
            /// Minimum depth for Gain switch in meters.
            /// </summary>
            public const float MIN_CBTT_DEPTH_GAIN = 0.0f;

            /// <summary>
            /// Minimum BT Time Between Ping.
            /// </summary>
            public const float MIN_CBTTBP = 0.0f;

            /// <summary>
            /// Maximum BT Time Between Ping.
            /// </summary>
            public const float MAX_CBTTBP = 86400.0f;

            /// <summary>
            /// Minimum Water Track Time Between Ping.
            /// </summary>
            public const float MIN_CWTTBP = 0.0f;

            /// <summary>
            /// Maximum Water Track Time Between Ping.
            /// </summary>
            public const float MAX_CWTTBP = 86400.0f;

            /// <summary>
            /// Minimum Water Profile Time Between Ping.
            /// </summary>
            public const float MIN_CWPTBP = 0.0f;

            /// <summary>
            /// Maximum Water Profile Time Between Ping.
            /// </summary>
            public const float MAX_CWPTBP = 86400.0f;

            /// <summary>
            /// Mininimum number of ensembles for CBI command.
            /// Also value to disable the command.
            /// </summary>
            public const UInt16 MIN_CBI_NUM_ENS = 0;

            /// <summary>
            /// Minimum BT Blank.
            /// </summary>
            public const float MIN_CBTBL = 0.0f;

            /// <summary>
            /// Maximum BT Blank.
            /// </summary>
            public const float MAX_CBTBL = 10.0f;

            /// <summary>
            /// Minimum WT Blank.
            /// </summary>
            public const float MIN_CWTBL = 0.0f;

            /// <summary>
            /// Maximum WT Blank.
            /// </summary>
            public const float MAX_CWTBL = 100.0f;

            /// <summary>
            /// Minimum WP Blank.
            /// </summary>
            public const float MIN_CWPBL = 0.0f;

            /// <summary>
            /// Maximum WP Blank.
            /// </summary>
            public const float MAX_CWPBL = 100.0f;

            /// <summary>
            /// Minimum Pulse To Pulse Lag in CBTBB.
            /// </summary>
            public const float MIN_CBTBB_PULSETOPULSE_LAG = 0.0f;

            /// <summary>
            /// Minimum Long Range Depth in CBTBB.
            /// </summary>
            public const float MIN_CBTBB_LONGRANGEDEPTH = 0.0f;

            /// <summary>
            /// Minimum BT maximum Depth.
            /// </summary>
            public const float MIN_CBTMX = 5.0f;

            /// <summary>
            /// Maximum BT maximum Depth.
            /// </summary>
            public const float MAX_CBTMX = 10000f;

            /// <summary>
            /// Minimum Water Track Bin size.
            /// </summary>
            public const float MIN_CWTBS = 0.05f;

            /// <summary>
            /// Maximum Water Track Bin size.
            /// </summary>
            public const float MAX_CWTBS = 64.0f;

            /// <summary>
            /// Minimum Water Profile Bin size.
            /// </summary>
            public const float MIN_CWPBS = 0.01f;

            /// <summary>
            /// Maximum Water Profile Bin size.
            /// </summary>
            public const float MAX_CWPBS = 100.0f;

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
            public const float MIN_CWPX = 0.0f;

            /// <summary>
            /// Maximum Water Profile Transmit.
            /// </summary>
            public const float MAX_CWPX = 100.0f;

            #endregion

            #region Number of Parameters Per Command

            /// <summary>
            /// NUmber of parameters for the CWPBB command.
            /// </summary>
            public const int CWPBB_NUM_PARAM = 2;

            /// <summary>
            /// Number of parameters for the CWPAP command.
            /// </summary>
            public const int CWPAP_NUM_PARAM = 5;

            /// <summary>
            /// Number of parameters for the CWPST command.
            /// </summary>
            public const int CWPST_NUM_PARAM = 3;

            /// <summary>
            /// Number of parameters for the CWPBL command.
            /// </summary>
            public const int CWPBP_NUM_PARAM = 2;

            /// <summary>
            /// Number of parameters for the CBTBB command.
            /// </summary>
            public const int CBTBB_NUM_PARAM = 3;

            /// <summary>
            /// Number of parameters for the CBTT command.
            /// </summary>
            public const int CBTT_NUM_PARAM = 4;

            /// <summary>
            /// Number of parameters for the CBI command.
            /// </summary>
            public const int CBI_NUM_PARAM = 2;

            #endregion

            #region Properties

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

            #region Transmit Pulse Type

            /// <summary>
            /// Water Profile Broadband Pulse Types.
            /// </summary>
            public enum eCWPBB_TransmitPulseType
            {
                /// <summary>
                /// (0) Non-Coded Narrowband.
                /// Provides long range profiles at the expense of variance.
                /// Not recommended for use with bin size less than the default
                /// bin size.
                /// </summary>
                NARROWBAND = 0, 

                /// <summary>
                /// (1) Coded Broadband.
                /// Typically 15% less range than narrow band but has greatly reduced
                /// variance (depending on lag length).
                /// Used in conjunction with CWPBP for small bins.
                /// </summary>
                BROADBAND = 1,

                /// <summary>
                /// (2) Non-Coded Pulse-To-Pulse.
                /// Narrowband and provides ultra low variance for small bin sizes.
                /// Non-coded has slightly higher variance than the coded
                /// transmit without the annoying autocorrelation side peaks.
                /// </summary>
                NONCODED_PULSE_TO_PULSE = 2,

                /// <summary>
                /// (3) Broadband Pulse-To-Pulse. (no ambuguity resolver).
                /// Provides ultra low variance for small bin sizes.  Coded 
                /// has slightly lower variance than the non-coded transmit.
                /// </summary>
                BROADBAND_PULSE_TO_PULSE  = 3,

                /// <summary>
                /// (4) Non Coded Broadband Pulse-To-Pulse. (no ambuguity resolver).
                /// Narrowband and provides ultra low variance for small bin sizes.  Coded 
                /// has slightly lower variance than the non-coded transmit.
                /// </summary>
                NONCODED_BROADBAND_PULSE_TO_PULSE = 4,

                /// <summary>
                /// Broadband with ambuguity resolver ping.
                /// Used in conjunction with CWPBP.
                /// </summary>
                BROADBAND_AMBIGUITY_RESOLVER = 5,

                /// <summary>
                /// Broadband pulse to pulse with ambiguity resolver ping.
                /// Used in conjunction with CWPAP.
                /// </summary>
                BROADBAND_P2P_AMBIGUITY_RESOLVER = 6
            }

            /// <summary>
            /// String for Transmit Pulse Type Narrowband.
            /// </summary>
            public const string TRANSMIT_PULSE_TYPE_NARROWBAND = "NarrowBand";

            /// <summary>
            /// String for Transmit Pulse Type Broadband.
            /// </summary>
            public const string TRANSMIT_PULSE_TYPE_BROADBAND = "BroadBand";

            /// <summary>
            /// String for Transmit Pulse Type Pulse to Pulse Non-Coded.
            /// </summary>
            public const string TRANSMIT_PULSE_TYPE_NONCODED_PTP = "Non-Coded Pulse to Pulse";

            /// <summary>
            /// String for Transmit Pulse Type Broadband Pulse to Pulse.
            /// </summary>
            public const string TRANSMIT_PULSE_TYPE_BB_PTP = "Broadband Pulse to Pulse";

            /// <summary>
            /// String for Transmit Pulse Type Non-Coded Broadband Pulse to Pulse.
            /// </summary>
            public const string TRANSMIT_PULSE_TYPE_NONCODED_BB_PTP = "Non-Coded Broadband Pulse to Pulse";

            /// <summary>
            /// String for Transmit Pulse Type Broadband with ambiguity resolver ping.
            /// </summary>
            public const string TRANSMIT_PULSE_TYPE_BB_AMBIGUITY_RESOLVER = "Broadband with Ambiguity Resolver Ping";

            /// <summary>
            /// String for Transmit Pulse Type Broadband pulse to pulse with ambiguity resolver ping.
            /// </summary>
            public const string TRANSMIT_PULSE_TYPE_BB_PTP_AMBIGUITY_RESOLVER = "Broadband Pulse to Pulse with Ambiguity Resolver Ping";

            /// <summary>
            /// Create a list for all the Transmit Pulse types.
            /// </summary>
            /// <returns>A list of all the Transmit Pulse types.</returns>
            public static List<string> GetCwpbbTransmitPulseTypeList()
            {
                List<string> list = new List<string>();
                list.Add(TRANSMIT_PULSE_TYPE_NARROWBAND);
                list.Add(TRANSMIT_PULSE_TYPE_BROADBAND);
                list.Add(TRANSMIT_PULSE_TYPE_NONCODED_PTP);
                list.Add(TRANSMIT_PULSE_TYPE_BB_PTP);
                list.Add(TRANSMIT_PULSE_TYPE_NONCODED_BB_PTP);
                list.Add(TRANSMIT_PULSE_TYPE_BB_AMBIGUITY_RESOLVER);
                list.Add(TRANSMIT_PULSE_TYPE_BB_PTP_AMBIGUITY_RESOLVER);

                return list;
            }

            /// <summary>
            /// Set the Water Profile Broadband transmit pulse type based off
            /// the string given.  The string should have been found from the list
            /// created with GetCwpbbTransmitPulseTypeList().  The default value is
            /// set by DEFAULT_CWPBB_TRANSMITPULSETYPE.
            /// </summary>
            /// <param name="type">String for the Transmit Pulse type.</param>
            public void SetCwpbbTransmitPulseType(string type)
            {
                switch(type)
                {
                    case TRANSMIT_PULSE_TYPE_NARROWBAND:
                        CWPBB_TransmitPulseType = eCWPBB_TransmitPulseType.NARROWBAND;
                        break;
                    case TRANSMIT_PULSE_TYPE_BROADBAND:
                        CWPBB_TransmitPulseType = eCWPBB_TransmitPulseType.BROADBAND;
                        break;
                    case TRANSMIT_PULSE_TYPE_NONCODED_PTP:
                        CWPBB_TransmitPulseType = eCWPBB_TransmitPulseType.NONCODED_PULSE_TO_PULSE;
                        break;
                    case TRANSMIT_PULSE_TYPE_NONCODED_BB_PTP:
                        CWPBB_TransmitPulseType = eCWPBB_TransmitPulseType.NONCODED_BROADBAND_PULSE_TO_PULSE;
                        break;
                    case TRANSMIT_PULSE_TYPE_BB_PTP:
                        CWPBB_TransmitPulseType = eCWPBB_TransmitPulseType.BROADBAND_PULSE_TO_PULSE;
                        break;
                    case TRANSMIT_PULSE_TYPE_BB_AMBIGUITY_RESOLVER:
                        CWPBB_TransmitPulseType = eCWPBB_TransmitPulseType.BROADBAND_AMBIGUITY_RESOLVER;
                        break;
                    case TRANSMIT_PULSE_TYPE_BB_PTP_AMBIGUITY_RESOLVER:
                        CWPBB_TransmitPulseType = eCWPBB_TransmitPulseType.BROADBAND_P2P_AMBIGUITY_RESOLVER;
                        break;
                    default:
                        CWPBB_TransmitPulseType = DEFAULT_CWPBB_TRANSMITPULSETYPE;
                        break;
                }
            }

            /// <summary>
            /// Get the Transmit Pulse Type as a string based off the
            /// current selection.
            /// </summary>
            /// <returns>String for the Transmit Pulse Type.</returns>
            public string GetCwpbbTransmitPulseType()
            {
                switch(CWPBB_TransmitPulseType)
                {
                    case eCWPBB_TransmitPulseType.NARROWBAND:
                        return TRANSMIT_PULSE_TYPE_NARROWBAND;
                    case eCWPBB_TransmitPulseType.BROADBAND:
                        return TRANSMIT_PULSE_TYPE_BROADBAND;
                    case eCWPBB_TransmitPulseType.NONCODED_PULSE_TO_PULSE:
                        return TRANSMIT_PULSE_TYPE_NONCODED_PTP;
                    case eCWPBB_TransmitPulseType.BROADBAND_PULSE_TO_PULSE:
                        return TRANSMIT_PULSE_TYPE_BB_PTP;
                    case eCWPBB_TransmitPulseType.NONCODED_BROADBAND_PULSE_TO_PULSE:
                        return TRANSMIT_PULSE_TYPE_NONCODED_BB_PTP;
                    case eCWPBB_TransmitPulseType.BROADBAND_AMBIGUITY_RESOLVER:
                        return TRANSMIT_PULSE_TYPE_BB_AMBIGUITY_RESOLVER;
                    case eCWPBB_TransmitPulseType.BROADBAND_P2P_AMBIGUITY_RESOLVER:
                        return TRANSMIT_PULSE_TYPE_BB_PTP_AMBIGUITY_RESOLVER;
                    default:
                        return TRANSMIT_PULSE_TYPE_BROADBAND;
                }
            }

            /// <summary>
            /// Based off the type given as a string, return the correct enum value.
            /// The default value is Broadband.
            /// </summary>
            /// <param name="type">Water Profile Transmit Pulse type as a string.</param>
            /// <returns>Enum value for string given.</returns>
            public static eCWPBB_TransmitPulseType GetTransmitPulseType(string type)
            {
                switch (type)
                {
                    case TRANSMIT_PULSE_TYPE_NARROWBAND:
                        return eCWPBB_TransmitPulseType.NARROWBAND;
                    case TRANSMIT_PULSE_TYPE_BROADBAND:
                        return eCWPBB_TransmitPulseType.BROADBAND;
                    case TRANSMIT_PULSE_TYPE_NONCODED_PTP:
                        return eCWPBB_TransmitPulseType.NONCODED_PULSE_TO_PULSE;
                    case TRANSMIT_PULSE_TYPE_BB_PTP:
                        return eCWPBB_TransmitPulseType.BROADBAND_PULSE_TO_PULSE;
                    case TRANSMIT_PULSE_TYPE_NONCODED_BB_PTP:
                        return eCWPBB_TransmitPulseType.NONCODED_BROADBAND_PULSE_TO_PULSE;
                    case TRANSMIT_PULSE_TYPE_BB_AMBIGUITY_RESOLVER:
                        return eCWPBB_TransmitPulseType.BROADBAND_AMBIGUITY_RESOLVER;
                    case TRANSMIT_PULSE_TYPE_BB_PTP_AMBIGUITY_RESOLVER:
                        return eCWPBB_TransmitPulseType.BROADBAND_P2P_AMBIGUITY_RESOLVER;
                    default:
                        return DEFAULT_CWPBB_TRANSMITPULSETYPE;
                }
            }

            #endregion

            /// <summary>
            /// Water Profile BroadBand (1).
            /// Water Profile Broadband.  Enables or disables water profile coded pulse transmissions and lag.
            /// 
            /// Transmit Pulse Type for the Water Profile.
            /// 
            /// Command CWPBB n, 2 \r
            /// Scale: enum Cwpbb_TransmitPulseType
            /// Range: enum Cwpbb_TransmitPulseType
            /// </summary>
            public eCWPBB_TransmitPulseType CWPBB_TransmitPulseType { get; set; }

            /// <summary>
            /// Water Profile BroadBand (2).
            /// Water Profile Broadband.  Enables or disables water profile coded pulse transmissions and lag.
            /// 
            /// Lag length in vertical meters.
            /// Not used with NarrowBand.  A longer lag will
            /// have lower variance and a lower ambiguity velocity.
            /// 
            /// Command: CWPBB 1, n.nnn \r
            /// Scale: meters
            /// Range: 0.042 to 1 (1200Khz) 
            ///        0.042 * N to 1 * N (N = Frequency Divsor AdcpPredictor.FREQ_DIV_1200, AdcpPredictor.FREQ_DIV_600, AdcpPredictor.FREQ_DIV_300, AdcpPredictor.FREQ_DIV_150)
            /// </summary>
            private float _cWPBB_LagLength;
            /// <summary>
            /// Water Profile BroadBand (2).
            /// Water Profile Broadband.  Enables or disables water profile coded pulse transmissions and lag.
            /// 
            /// Lag length in vertical meters.
            /// Not used with NarrowBand.  A longer lag will
            /// have lower variance and a lower ambiguity velocity.
            /// 
            /// Command: CWPBB 1, n.nnn \r
            /// Scale: meters
            /// Range: 0.042 to 1 (1200Khz) 
            ///        0.042 * N to 1 * N (N = Frequency Divsor AdcpPredictor.FREQ_DIV_1200, AdcpPredictor.FREQ_DIV_600, AdcpPredictor.FREQ_DIV_300, AdcpPredictor.FREQ_DIV_150)
            /// </summary>
            public float CWPBB_LagLength 
            {
                get { return _cWPBB_LagLength; } 
                set
                {
                    // Validate the command
                    if (Validator.ValidateMinMax(value, MIN_CWPBB_LAGLENGTH, MAX_CWPBB_LAGLENGTH))
                    {
                        _cWPBB_LagLength = value;
                    }
                }
            }

            /// <summary>
            /// Water Ambiguity Ping.
            /// Used when CWPBB = 5.
            /// Pulse to pulse ping and processing is used for the 
            /// ambiguity resolver therefore  (Blank + BinSize) > Lag.
            /// 
            /// 0 to 100 sets the number of pings that will be averaged together.
            /// 
            /// Command: CWPAP n X X X X \r
            /// Scale: Number of pings.
            /// Range 0 to 100.
            /// </summary>
            private UInt16 _cWPAP_NumPingsAvg; 
            /// <summary>
            /// Water Ambiguity Ping.
            /// Used when CWPBB = 5.
            /// Pulse to pulse ping and processing is used for the 
            /// ambiguity resolver therefore  (Blank + BinSize) > Lag.
            /// 
            /// 0 to 100 sets the number of pings that will be averaged together.
            /// 
            /// Command: CWPAP n X X X X \r
            /// Scale: Number of pings.
            /// Range 0 to 100.
            /// </summary>
            public UInt16 CWPAP_NumPingsAvg 
            {
                get { return _cWPAP_NumPingsAvg; }
                
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMinMax(value, MIN_CWPAP_NUMPINGS, MAX_CWPAP_NUMPINGS))
                    {
                        _cWPAP_NumPingsAvg = value;
                    }
                }
            }

            /// <summary>
            /// Water Ambiguity Ping.
            /// Used when CWPBB = 5.
            /// Pulse to pulse ping and processing is used for the 
            /// ambiguity resolver therefore  (Blank + BinSize) > Lag.
            /// 
            /// Lag (meters) sets the length of the lag.
            /// 
            /// Command: CWPAP X n.nnn X X X \r
            /// Scale: meters
            /// Range: 
            /// </summary>
            private float _cWPAP_Lag;
            /// <summary>
            /// Water Ambiguity Ping.
            /// Used when CWPBB = 5.
            /// Pulse to pulse ping and processing is used for the 
            /// ambiguity resolver therefore  (Blank + BinSize) > Lag.
            /// 
            /// Lag (meters) sets the length of the lag.
            /// 
            /// Command: CWPAP X n.nnn X X X \r
            /// Scale: meters
            /// Range: 0 to n.nnn
            /// </summary>
            public float CWPAP_Lag 
            {
                get { return _cWPAP_Lag; } 
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMinMax(value, MIN_CWPAP_LAG, MAX_CWPAP_LAG))
                    {
                        _cWPAP_Lag = value;
                    }
                }
            }

            /// <summary>
            /// Water Ambiguity Ping.
            /// Used when CWPBB = 5.
            /// Pulse to pulse ping and processing is used for the 
            /// ambiguity resolver therefore  (Blank + BinSize) > Lag.
            /// 
            /// Blank (meters) sets the starting position of the bin.
            /// 
            /// Command: CWPAP X X n.nnn X X \r
            /// Scale: meters
            /// Range: 0 to n.nnn
            /// </summary>
            private float _cWPAP_Blank;
            /// <summary>
            /// Water Ambiguity Ping.
            /// Used when CWPBB = 5.
            /// Pulse to pulse ping and processing is used for the 
            /// ambiguity resolver therefore  (Blank + BinSize) > Lag.
            /// 
            /// Blank (meters) sets the starting position of the bin.
            /// 
            /// Command: CWPAP X X n.nnn X X \r
            /// Scale: meters
            /// Range: 0 to n.nnn
            /// </summary>
            public float CWPAP_Blank
            {
                get { return _cWPAP_Blank; }
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMinMax(value, MIN_CWPAP_BLANK, MAX_CWPAP_BLANK))
                    {
                        _cWPAP_Blank = value;
                    }
                }
            }

            /// <summary>
            /// Water Ambiguity Ping.
            /// Used when CWPBB = 5.
            /// Pulse to pulse ping and processing is used for the 
            /// ambiguity resolver therefore  (Blank + BinSize) > Lag.
            /// 
            /// Bin Size (meters).
            /// 
            /// Command: CWPAP X X X n.nnn X \r
            /// Scale: meters
            /// Range: 0 to n.nnn
            /// </summary>
            private float _cWPAP_BinSize;
            /// <summary>
            /// Water Ambiguity Ping.
            /// Pulse to pulse ping and processing is used for the 
            /// ambiguity resolver therefore  (Blank + BinSize) > Lag.
            /// Used when CWPBB = 5.
            /// 
            /// Bin Size (meters).
            /// 
            /// Command: CWPAP X X X n.nnn X \r
            /// Scale: meters
            /// Range: 0 to n.nnn
            /// </summary>
            public float CWPAP_BinSize
            {
                get { return _cWPAP_BinSize; }
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMinMax(value, MIN_CWPAP_BINSIZE, MAX_CWPAP_BINSIZE))
                    {
                        _cWPAP_BinSize = value;
                    }
                }
            }

            /// <summary>
            /// Water Ambiguity Ping.
            /// Used when CWPBB = 5.
            /// Pulse to pulse ping and processing is used for the 
            /// ambiguity resolver therefore  (Blank + BinSize) > Lag.
            /// 
            /// Time Between Ambiguity Pings (seconds).
            /// 
            /// Command: CWPAP X X X X n.nnn \r
            /// Scale: Seconds
            /// Range: 0 to n.nnn
            /// </summary>
            private float _cWPAP_TimeBetweenPing;
            /// <summary>
            /// Water Ambiguity Ping.
            /// Used when CWPBB = 5.
            /// Pulse to pulse ping and processing is used for the 
            /// ambiguity resolver therefore  (Blank + BinSize) > Lag.
            /// 
            /// Time Between Ambiguity Pings (seconds).
            /// 
            /// Command: CWPAP X X X X n.nnn \r
            /// Scale: Seconds
            /// Range: 0 to n.nnn
            /// </summary>
            public float CWPAP_TimeBetweenPing
            {
                get { return _cWPAP_TimeBetweenPing; }
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMinMax(value, MIN_CWPAP_TIME_BETWEEN_PING, MAX_CWPAP_TIME_BETWEEN_PING))
                    {
                        _cWPAP_TimeBetweenPing = value;
                    }
                }
            }

            /// <summary>
            /// Water Profile Screening Thresholds (1)
            /// Water Profile Correlation Threshold.
            /// 
            /// Used for screening profile beams.  A beam with a correlation value
            /// less than the threshold will be flagged bad and not included in the bin average.
            /// Nominal beam correlation values are dependent on the pulse codin, the number of
            /// repeated codes, and whether not the pulse-to-pulse processing is being used.  For Example:
            ///  i.  The pulse-to-pulse nominal correlation is 1.00.  A correlation value of 0.50 occurs
            ///      when the signal is equal to the noise (SNR = 1 or 0 dB).
            ///  ii. Broadband correlation is dependent on the number of repeated code sequences in the transmission.
            ///      If 5 repeats are transmitted the nominal correlation will be 4/5 or 0.80.  A correlation value of 0.4,
            ///      in this case, indicates a signal to noise ratio of 1.
            /// 
            /// Command CWPST n.nn, 2, 3 \r
            /// Scale: 
            /// Range: 0.00 to 1.00
            /// 
            /// </summary>
            private float _cWPST_CorrelationThresh;
            /// <summary>
            /// Water Profile Screening Thresholds (1)
            /// Water Profile Correlation Threshold.
            /// 
            /// Used for screening profile beams.  A beam with a correlation value
            /// less than the threshold will be flagged bad and not included in the bin average.
            /// Nominal beam correlation values are dependent on the pulse codin, the number of
            /// repeated codes, and whether not the pulse-to-pulse processing is being used.  For Example:
            ///  i.  The pulse-to-pulse nominal correlation is 1.00.  A correlation value of 0.50 occurs
            ///      when the signal is equal to the noise (SNR = 1 or 0 dB).
            ///  ii. Broadband correlation is dependent on the number of repeated code sequences in the transmission.
            ///      If 5 repeats are transmitted the nominal correlation will be 4/5 or 0.80.  A correlation value of 0.4,
            ///      in this case, indicates a signal to noise ratio of 1.
            /// 
            /// Command CWPST n.nn, 2, 3 \r
            /// Scale: 
            /// Range: 0.00 to 1.00
            /// 
            /// </summary>
            public float CWPST_CorrelationThresh 
            {
                get { return _cWPST_CorrelationThresh; }
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMinMax(value, MIN_CWPST_CORR_THRESH, MAX_CWPST_CORR_THRESH))
                    {
                        _cWPST_CorrelationThresh = value;
                    }
                }
            }

            /// <summary>
            /// Water Profile Thresholds (2)
            /// Water Profile Q Velocity Threshold.
            /// 
            /// Used for screening transformed profile bins.  A bin with a,
            /// absolute Q velocity that is higher than the Q threshold will be 
            /// flagged as bad.  Beam coordinate velocity data is not affected.
            /// 
            /// Command: CWPST 1, n.nnn, 3 \r
            /// Scale: Meters per second.
            /// Range: 
            /// </summary>
            private float _cWPST_QVelocityThresh;
            /// <summary>
            /// Water Profile Thresholds (2)
            /// Water Profile Q Velocity Threshold.
            /// 
            /// Used for screening transformed profile bins.  A bin with a,
            /// absolute Q velocity that is higher than the Q threshold will be 
            /// flagged as bad.  Beam coordinate velocity data is not affected.
            /// 
            /// Command: CWPST 1, n.nnn, 3 \r
            /// Scale: Meters per second.
            /// Range: 0 to n.nnn
            /// </summary>
            public float CWPST_QVelocityThresh 
            {
                get { return _cWPST_QVelocityThresh; } 
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMin(value, MIN_CWPST_Q_VELOCITY_THRESH))
                    {
                        _cWPST_QVelocityThresh = value;
                    }
                }
            }

            /// <summary>
            /// Water Profile Thresholds (3)
            /// Water Profile V Velocity Threshold.
            /// 
            /// Used for screening transformed profile bins.  A bin with a,
            /// absolute Vertical Velocity that is higher than the V threshold will
            /// be flagged as bad.  Beam coordinate velocity data is not affected.
            /// 
            /// Command CWPST 1, 2, n.nnn \r
            /// Scale: Meters per second.
            /// Range: 0 to n.nnn
            /// </summary>
            private float _cWPST_VVelocityThresh;
            /// <summary>
            /// Water Profile Thresholds (3)
            /// Water Profile V Velocity Threshold.
            /// 
            /// Used for screening transformed profile bins.  A bin with a,
            /// absolute Vertical Velocity that is higher than the V threshold will
            /// be flagged as bad.  Beam coordinate velocity data is not affected.
            /// 
            /// Command CWPST 1, 2, n.nnn \r
            /// Scale: Meters per second.
            /// Range: 0 to n.nnn
            /// </summary>
            public float CWPST_VVelocityThresh 
            {
                get { return _cWPST_VVelocityThresh; } 
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMin(value, MIN_CWPST_V_VELOCITY_THRESH))
                    {
                        _cWPST_VVelocityThresh = value;
                    }
                }
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
            private float _cWPBL;
            /// <summary>
            /// Water Profile Blank. 
            /// Sets the vertical range from the face of the 
            /// transducer to the first sample of the first Bin.
            /// 
            /// Command: CWPBL n.nn \r
            /// Scale: meters
            /// Range: 0.00 to 100.00
            /// </summary>
            public float CWPBL 
            {
                get { return _cWPBL; }     
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMinMax(value, MIN_CWPBL, MAX_CWPBL))
                    {
                        _cWPBL = value;
                    }
                }
            }

            /// <summary>
            /// Water Profile Bin Size 
            /// n.nn sets the vertical Bin size.
            /// 
            /// Command CWPBS n.nnn \r
            /// Scale: meters
            /// Range: 0.01 to n.nnn
            /// </summary>
            private float _cWPBS;
            /// <summary>
            /// Water Profile Bin Size 
            /// n.nn sets the vertical Bin size.
            /// 
            /// Command CWPBS n.nnn \r
            /// Scale: meters
            /// Range: 0.01 to n.nnn
            /// </summary>
            public float CWPBS 
            {
                get { return _cWPBS; } 
                set
                {
                    // Verify the value is within range
                    if (Validator.ValidateMinMax(value, MIN_CWPBS, MAX_CWPBS))
                    {
                        _cWPBS = value;
                    }
                }
            }

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
            private float _cWPX;
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
            public float CWPX 
            {
                get { return _cWPX; } 
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMinMax(value, MIN_CWPX, MAX_CWPX))
                    {
                        _cWPX = value;
                    }
                }
            }

            /// <summary>
            /// Water Profile Number of Bins
            /// Sets the number of bins that will be processed and output.
            /// 
            /// Command: CWPBN N \r
            /// Scale: Number of bins
            /// Range: 0 to 200 
            /// </summary>
            private UInt16 _cWPBN;
            /// <summary>
            /// Water Profile Number of Bins
            /// Sets the number of bins that will be processed and output.
            /// 
            /// Command: CWPBN N \r
            /// Scale: Number of bins
            /// Range: 0 to 200 
            /// </summary>
            public UInt16 CWPBN 
            {
                get { return _cWPBN; } 
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMinMax(value, MIN_CWPBN, MAX_CWPBN))
                    {
                        _cWPBN = value;
                    }
                }
            }

            /// <summary>
            /// Water Profile Number of Pings
            /// Sets the number of pings that will 
            /// be averaged together during the ensemble.
            /// 
            /// If CWPAI is set equal to 00:00:00.00
            /// sets the number of pings in the ensemble.
            /// 
            /// Command: CWPP N \r
            /// Scale: Number of Pings
            /// Range: 0 to 10,000 
            /// </summary>
            private UInt16 _cWPP;
            /// <summary>
            /// Water Profile Number of Pings
            /// Sets the number of pings that will 
            /// be averaged together during the ensemble.
            /// 
            /// If CWPAI is set equal to 00:00:00.00
            /// sets the number of pings in the ensemble.
            /// 
            /// Command: CWPP N \r
            /// Scale: Number of Pings
            /// Range: 0 to 10,000 
            /// </summary>
            public UInt16 CWPP 
            {
                get { return _cWPP; } 
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMinMax(value, MIN_CWPP, MAX_CWPP))
                    {
                        _cWPP = value;
                    }
                }
            }

            /// <summary>
            /// Water Base Pings (1)
            /// Used when CWPBB = 0 or 1.
            /// This command should be used when small, non pulse to pulse,
            /// bin sizes are required for profiling.  The Base Pings are averaged
            /// together as a complex number which allows for better correlation screening and
            /// averaging.
            /// 
            /// Pings a value of 2 to 100. Sets the number of pings that will be averaged
            /// together during each CWPP ping.  A value of 0 to 1 disables Base Ping averaging.
            /// 
            /// Command: CWPBP n, 2 \r
            /// Scale: Number of Pings.
            /// Range: 0 to 100
            /// </summary>
            private UInt16 _cWPBP_NumPingsAvg;
            /// <summary>
            /// Water Base Pings (1)
            /// Used when CWPBB = 0 or 1.
            /// This command should be used when small, non pulse to pulse,
            /// bin sizes are required for profiling.  The Base Pings are averaged
            /// together as a complex number which allows for better correlation screening and
            /// averaging.
            /// 
            /// Pings a value of 2 to 100. Sets the number of pings that will be averaged
            /// together during each CWPP ping.  A value of 0 to 1 disables Base Ping averaging.
            /// 
            /// Command: CWPBP n, 2 \r
            /// Scale: Number of Pings.
            /// Range: 0 to 100
            /// </summary>
            public UInt16 CWPBP_NumPingsAvg 
            {
                get { return _cWPBP_NumPingsAvg; } 
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMinMax(value, MIN_CWPBP_NUM_PING, MAX_CWPBP_NUM_PING))
                    {
                        _cWPBP_NumPingsAvg = value;
                    }
                }
            }

            /// <summary>
            /// Water Base Pings (2)
            /// Used when CWPBB = 0 or 1.
            /// This command should be used when small, non pulse to pulse,
            /// bin sizes are required for profiling.  The Base Pings are averaged
            /// together as a complex number which allows for better correlation screening and
            /// averaging. 
            /// 
            /// Time in seconds between the base pings (seconds).  Normally it is a small number i.e.
            /// 0.01 for the 1200 kHz system.
            /// 
            /// Command: CWPBP 1, t.t \r
            /// Scale: Time in Seconds.
            /// Range: 0 to t.t
            /// </summary>
            private float _cWPBP_TimeBetweenBasePings;
            /// <summary>
            /// Water Base Pings (2)
            /// Used when CWPBB = 0 or 1.
            /// This command should be used when small, non pulse to pulse,
            /// bin sizes are required for profiling.  The Base Pings are averaged
            /// together as a complex number which allows for better correlation screening and
            /// averaging. 
            /// 
            /// Time in seconds between the base pings (seconds).  Normally it is a small number i.e.
            /// 0.01 for the 1200 kHz system.
            /// 
            /// Command: CWPBP 1, t.t \r
            /// Scale: Time in Seconds.
            /// Range: 0 to t.t
            /// </summary>
            public float CWPBP_TimeBetweenBasePings 
            {
                get { return _cWPBP_TimeBetweenBasePings; }
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMinMax(value, MIN_CWPBP_TIME_BETWEEN_PINGS, MAX_CWPBP_TIME_BETWEEN_PINGS))
                    {
                        _cWPBP_TimeBetweenBasePings = value;
                    }
                }
            }

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
            /// Water Profile Time between Pings
            /// Sets the time between profile pings
            /// 
            /// Command: CWPTBP n.nn \r
            /// Scale: seconds
            /// Range: 0.00 to 86400.0 
            /// </summary>
            private float _cWPTBP;
            /// <summary>
            /// Water Profile Time between Pings
            /// Sets the time between profile pings
            /// 
            /// Command: CWPTBP n.nn \r
            /// Scale: seconds
            /// Range: 0.00 to 86400.0 
            /// </summary>
            public float CWPTBP 
            {
                get { return _cWPTBP; } 
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMinMax(value, MIN_CWPTBP, MAX_CWPTBP))
                    {
                        _cWPTBP = value;
                    }
                }
            }

            #endregion // Water Profile

            #region Burst Interval Properties

            /// <summary>
            /// Burst Interval.
            /// Sets the time interval between a series of ensembles.
            /// 
            /// Used when a precise short time interval is required between
            /// ensembles followed by a period of sleep.
            /// 
            /// Note:  Burst sampling typically requires precise timing intervals. 
            /// If serial data output is enabled during burst sampling the user 
            /// needs to ensure that each data transfer completes before the 
            /// next ensemble is output. If the length of the data transfer 
            /// exceeds the time between samples the data will bottleneck and 
            /// the desired sample rate will not be maintained. Self-contained 
            /// ADCP users can disable the serial data output by sending the 
            /// command CEOUTPUT 0 [CR]. Of course you need to enable the internal 
            /// data recording CERECORD 1 [CR]. During burst sampling, internal 
            /// data recording only occurs at the end of the burst or when the 
            /// STOP[CR] command is received by the ADCP. Another option, when 
            /// real-time data is required, is to increase the data output rate 
            /// by using the C485B 921600[CR] or C422B 921600[CR] command. Be 
            /// aware that the RS232 connection may not reliably support the 
            /// higher data rates.
            /// 
            /// Command: CBI HH:MM:SS.hh, n \r
            /// Scale: TimeValue
            /// Range: TimeValue
            /// </summary>
            public TimeValue CBI_BurstInterval { get; set; }

            /// <summary>
            /// Burst Inteval.
            /// Sets the Number o ensembles that are output during each burst.  
            /// The time between each ensemble is controlled by the CEI command.
            /// 0 Disables the command.
            /// 
            /// Used when a prices short time interval is required between
            /// ensembles followed by a period of sleep.
            /// 
            /// Command: CBI HH:MM:SS.hh, n \r
            /// Scale: Number of ensembles
            /// Range: 0 - N
            /// </summary>
            private UInt16 _cBI_NumEnsembles;
            /// <summary>
            /// Burst Inteval.
            /// Sets the Number o ensembles that are output during each burst.  
            /// The time between each ensemble is controlled by the CEI command.
            /// 0 Disables the command.
            /// 
            /// Used when a prices short time interval is required between
            /// ensembles followed by a period of sleep.
            /// 
            /// Command: CBI HH:MM:SS.hh, n \r
            /// Scale: Number of ensembles
            /// Range: 0 - N
            /// </summary>
            public UInt16 CBI_NumEnsembles
            {
                get { return _cBI_NumEnsembles; }
                set
                {
                    if (Validator.ValidateMin(value, MIN_CBI_NUM_ENS))
                    {
                        _cBI_NumEnsembles = value;
                    }
                }
            }

            /// <summary>
            /// Set the Burst Pair flag.  If Burst Pair is set to TRUE,
            /// the next subsystem will be interleaved (alternating pings)
            /// with the current subsystem during the burst.  The CBI commands
            /// for the pair should be set to the same value(s).
            /// </summary>
            private bool _cBI_BurstPairFlag;
            /// <summary>
            /// Set the Burst Pair flag.  If Burst Pair is set to TRUE,
            /// the next subsystem will be interleaved (alternating pings)
            /// with the current subsystem during the burst.  The CBI commands
            /// for the pair should be set to the same value(s).
            /// </summary>
            public bool CBI_BurstPairFlag
            {
                get { return _cBI_BurstPairFlag; }
                set
                {
                    _cBI_BurstPairFlag = value;
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

            #region Bottom Track Broadband Mode

            /// <summary>
            /// Bottom Track Broadband Mode.
            /// </summary>
            public enum eCBTBB_Mode
            {
                /// <summary>
                /// (0) Narrowband Long Range
                /// </summary>
                NARROWBAND_LONG_RANGE = 0,

                /// <summary>
                /// (1) Coded Broadband Transmit.
                /// </summary>
                BROADBAND_CODED = 1,

                /// <summary>
                /// (2) Broadband Non-coded Transmit.
                /// </summary>
                BROADBAND_NON_CODED = 2,

                /// <summary>
                /// (3) NA.
                /// </summary>
                NA_3 = 3,

                /// <summary>
                /// (4) Broadband Non-coded Pulse to Pulse
                /// </summary>
                BROADBAND_NON_CODED_P2P = 4,

                /// <summary>
                /// (5) NA.
                /// </summary>
                NA_5 = 5,

                /// <summary>
                /// (6) NA.
                /// </summary>
                NA_6 = 6,

                /// <summary>
                /// (7) Auto switch between Narrowband, Broadband Non-Coded and Broadband Non-Coded Pulse to Pulse.
                /// </summary>
                AUTO_SWITCH_NARROWBAND_BB_NONCODED_BB_NONCODED_P2P= 7
            }

            /// <summary>
            /// String for Narrowband mode.
            /// </summary>
            public const string BT_BB_MODE_NARROWBAND = "Narrowband";

            /// <summary>
            /// String for Broadband Coded mode.
            /// </summary>
            public const string BT_BB_MODE_BROADBAND_CODED = "Broadband Coded";

            /// <summary>
            /// String for Broadband Non-Coded mode.
            /// </summary>
            public const string BT_BB_MODE_BROADBAND_NONCODED = "Broadband Non-coded";

            /// <summary>
            /// String for Broadband Non-Coded Pulse to Pulse mode.
            /// </summary>
            public const string BT_BB_MODE_BROADBAND_NONCODED_P2P = "Broadband Non-coded Pulse to Pulse";

            /// <summary>
            /// String for NarrowBand, Broadband Non-coded, and Broadband Non-coded Pulse to Pulse.
            /// </summary>
            public const string BT_BB_MODE_NB_BBNONCODED_BBNONCODEDP2P = "NarrowBand | Broadband Non-coded | Broadband Non-coded Pulse to Pulse";

            /// <summary>
            /// String for NA 3.
            /// </summary>
            public const string BT_BB_MODE_NA_3 = "NA_3";

            /// <summary>
            /// String for NA 5.
            /// </summary>
            public const string BT_BB_MODE_NA_5 = "NA_5";

            /// <summary>
            /// String for NA 6.
            /// </summary>
            public const string BT_BB_MODE_NA_6 = "NA_6";

            /// <summary>
            /// Create a list for all the Transmit Pulse types.
            /// </summary>
            /// <returns>A list of all the Transmit Pulse types.</returns>
            public static List<string> GetCBTBB_ModeList()
            {
                List<string> list = new List<string>();
                list.Add(BT_BB_MODE_NARROWBAND);
                list.Add(BT_BB_MODE_BROADBAND_CODED);
                list.Add(BT_BB_MODE_BROADBAND_NONCODED);
                list.Add(BT_BB_MODE_BROADBAND_NONCODED_P2P);
                list.Add(BT_BB_MODE_NB_BBNONCODED_BBNONCODEDP2P);

                return list;
            }

            /// <summary>
            /// Set the Bottom Track Broadband mode based off
            /// the string given.  The string should have been found from the list
            /// created with GetCBTBB_ModeList().  The default value is
            /// set by DEFAULT_CBTBB_MODE.
            /// </summary>
            /// <param name="type">String for the Bottom Track Broadband mode.</param>
            public void SetCBTBB_Mode(string type)
            {
                switch (type)
                {
                    case BT_BB_MODE_NARROWBAND:
                        CBTBB_Mode = eCBTBB_Mode.NARROWBAND_LONG_RANGE;
                        break;
                    case BT_BB_MODE_BROADBAND_CODED:
                        CBTBB_Mode = eCBTBB_Mode.BROADBAND_CODED;
                        break;
                    case BT_BB_MODE_BROADBAND_NONCODED:
                        CBTBB_Mode = eCBTBB_Mode.BROADBAND_NON_CODED;
                        break;
                    case BT_BB_MODE_BROADBAND_NONCODED_P2P:
                        CBTBB_Mode = eCBTBB_Mode.BROADBAND_NON_CODED_P2P;
                        break;
                    case BT_BB_MODE_NB_BBNONCODED_BBNONCODEDP2P:
                        CBTBB_Mode = eCBTBB_Mode.AUTO_SWITCH_NARROWBAND_BB_NONCODED_BB_NONCODED_P2P;
                        break;
                    case BT_BB_MODE_NA_3:
                        CBTBB_Mode = eCBTBB_Mode.NA_3;
                        break;
                    case BT_BB_MODE_NA_5:
                        CBTBB_Mode = eCBTBB_Mode.NA_5;
                        break;
                    case BT_BB_MODE_NA_6:
                        CBTBB_Mode = eCBTBB_Mode.NA_6;
                        break;
                    default:
                        CBTBB_Mode = DEFAULT_CBTBB_MODE;
                        break;
                }
            }

            /// <summary>
            /// Get the Bottom Track Broadband Mode as a string based off the
            /// current selection.
            /// </summary>
            /// <returns>String for the Bottom Track Broadband mode.</returns>
            public string GetCBTBB_Mode()
            {
                switch (CBTBB_Mode)
                {
                    case eCBTBB_Mode.NARROWBAND_LONG_RANGE:
                        return BT_BB_MODE_NARROWBAND;
                    case eCBTBB_Mode.BROADBAND_CODED:
                        return BT_BB_MODE_BROADBAND_CODED;
                    case eCBTBB_Mode.BROADBAND_NON_CODED:
                        return BT_BB_MODE_BROADBAND_NONCODED;
                    case eCBTBB_Mode.BROADBAND_NON_CODED_P2P:
                        return BT_BB_MODE_BROADBAND_NONCODED_P2P;
                    case eCBTBB_Mode.AUTO_SWITCH_NARROWBAND_BB_NONCODED_BB_NONCODED_P2P:
                        return BT_BB_MODE_NB_BBNONCODED_BBNONCODEDP2P;
                    case eCBTBB_Mode.NA_3:
                        return BT_BB_MODE_NA_3;
                    case eCBTBB_Mode.NA_5:
                        return BT_BB_MODE_NA_5;
                    case eCBTBB_Mode.NA_6:
                        return BT_BB_MODE_NA_6;
                    default:
                        return BT_BB_MODE_NARROWBAND;
                }
            }

            #endregion

            /// <summary>
            /// Bottom Track BroadBand Control (1).
            /// Bottom Track BroadBand mode.
            /// 
            /// Command: CWPBB n, 2, 3 \r
            /// Scale: enum eCBTBB_Mode
            /// Range: enum eCBTBB_Mode
            /// </summary>
            public eCBTBB_Mode CBTBB_Mode { get; set; }

            /// <summary>
            /// Bottom Track Broadband Control (2)
            /// Pulse to Pulse Lag in meters.
            /// 
            /// Lag length in vertical meters.  When enabled bottom track will
            /// use pulse-to-pulse transmit and processing at depths less than 1/2 the
            /// lag length.  Allows for near bottom ultra low variance velocity measurements.
            /// 
            /// Command: CBTBB 1, n.nnn, 3 \r
            /// Scale: meters
            /// Range: 
            /// </summary>
            private float _cBTBB_PulseToPulseLag;
            /// <summary>
            /// Bottom Track Broadband Control (2)
            /// Pulse to Pulse Lag in meters.
            /// 
            /// Lag length in vertical meters.  When enabled bottom track will
            /// use pulse-to-pulse transmit and processing at depths less than 1/2 the
            /// lag length.  Allows for near bottom ultra low variance velocity measurements.
            /// 
            /// Command: CBTBB 1, n.nnn, 3 \r
            /// Scale: meters
            /// Range: 0 to n.nnn
            /// </summary>
            public float CBTBB_PulseToPulseLag
            {
                get { return _cBTBB_PulseToPulseLag; }
                set
                {
                    // Validate the command is within range
                    if (Validator.ValidateMin(value, MIN_CBTBB_PULSETOPULSE_LAG))
                    {
                        _cBTBB_PulseToPulseLag = value;
                    }
                }
            }

            /// <summary>
            /// Bottom Track Broadband Control (3)
            /// Long Range Depth in meters.
            /// 
            /// The range in meters beyond which the bottom track will switch 
            /// to narrow band processing.
            /// 
            /// Command: CBTBB 1, 2, n.nnn \r
            /// Scale: meters
            /// Range: 0 to n.nnn
            /// </summary>
            private float _cBTBB_LongRangeDepth;
            /// <summary>
            /// Bottom Track Broadband Control (3)
            /// Long Range Depth in meters.
            /// 
            /// The range in meters beyond which the bottom track will switch 
            /// to narrow band processing.
            /// 
            /// Command: CBTBB 1, 2, n.nnn \r
            /// Scale: meters
            /// Range: 0 to n.nnn
            /// </summary>
            public float CBTBB_LongRangeDepth
            {
                get { return _cBTBB_LongRangeDepth; }
                set
                {
                    // Validate the command is within range
                    if (Validator.ValidateMin(value, MIN_CBTBB_LONGRANGEDEPTH))
                    {
                        _cBTBB_LongRangeDepth = value;
                    }
                }
            }


            /// <summary>
            /// Bottom Track Screening Thresholds (1)
            /// Bottom Track Correlation Threshold.
            /// 
            /// USed for screening beam data.  A beam with a correlation value less than
            /// the threshold will be flagged band and not included in the average.  Nominal
            /// correlation for bottom track is 1.
            /// 
            /// Command: CBTST n.nnn, 2, 3 \r
            /// Scale: 0.00 to 1.00
            /// Range: 0.00 to 1.00
            /// </summary>
            private float _cBTST_CorrelationThresh; 
            /// <summary>
            /// Bottom Track Screening Thresholds (1)
            /// Bottom Track Correlation Threshold.
            /// 
            /// USed for screening beam data.  A beam with a correlation value less than
            /// the threshold will be flagged band and not included in the average.  Nominal
            /// correlation for bottom track is 1.
            /// 
            /// Command: CBTST n.nnn, 2, 3 \r
            /// Scale: 0.00 to 1.00
            /// Range: 0.00 to 1.00
            /// </summary>
            public float CBTST_CorrelationThresh 
            {
                get { return _cBTST_CorrelationThresh; }
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMinMax(value, MIN_CBTST_CORR_THRESH, MAX_CBTST_CORR_THRESH))
                    {
                        _cBTST_CorrelationThresh = value;
                    }
                }
            }

            /// <summary>
            /// Bottom Track Screening Thresholds (2)
            /// Bottom Track Q Velocity Threshold.
            /// 
            /// Used for screening transformed bottom track velocities.  An
            /// absolute Q velocity that is higher than the Q threshold will be
            /// flagged as bad.  Beam coordinate velocity data is not affected.
            /// 
            /// Command: CBTST 1, n.nnn, 3 \r
            /// Scale: Meters per Second.
            /// Range: 0 to n.nnn 
            /// </summary>
            private float _cBTST_QVelocityThresh;
            /// <summary>
            /// Bottom Track Screening Thresholds (2)
            /// Bottom Track Q Velocity Threshold.
            /// 
            /// Used for screening transformed bottom track velocities.  An
            /// absolute Q velocity that is higher than the Q threshold will be
            /// flagged as bad.  Beam coordinate velocity data is not affected.
            /// 
            /// Command: CBTST 1, n.nnn, 3 \r
            /// Scale: Meters per Second.
            /// Range: 0 to n.nnn
            /// </summary>
            public float CBTST_QVelocityThresh
            {
                get { return _cBTST_QVelocityThresh; }
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMin(value, MIN_CBTST_QVEL_THRESH))
                    {
                        _cBTST_QVelocityThresh = value;
                    }
                }
            }

            /// <summary>
            /// Bottom Track Screening Threshold (3)
            /// Bottom Track V Velocity Threshold.
            /// 
            /// Used for screening transformed bottom track velocities.  
            /// An absolute Vertical velocity that is higher than the V threshold
            /// will be flagged as bad.  Beam coordinate velocity data is not affected.
            /// 
            /// Command: CBTST 1, 2, n.nnn \r
            /// Scale: Meters per Second.
            /// Range: 0 to n.nnn
            /// </summary>
            private float _cBTST_VVelocityThresh;
            /// <summary>
            /// Bottom Track Screening Threshold (3)
            /// Bottom Track V Velocity Threshold.
            /// 
            /// Used for screening transformed bottom track velocities.  
            /// An absolute Vertical velocity that is higher than the V threshold
            /// will be flagged as bad.  Beam coordinate velocity data is not affected.
            /// 
            /// Command: CBTST 1, 2, n.nnn \r
            /// Scale: Meters per Second.
            /// Range: 0 to n.nnn
            /// </summary>
            public float CBTST_VVelocityThresh
            {
                get { return _cBTST_VVelocityThresh; }
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMin(value, MIN_CBTST_VVEL_THRESH))
                    {
                        _cBTST_VVelocityThresh = value;
                    }
                }
            }

            /// <summary>
            /// Bottom Track Thresholds (1)
            /// Bottom Track SNR(dB) shallow detection threshold.
            /// 
            /// Command: CBTT n.nnn, 2, 3, 4 \r
            /// Scale: dB
            /// Range: 0 to n.nnn
            /// </summary>
            private float _cBTT_SNRShallowDetectionThresh;
            /// <summary>
            /// Bottom Track Thresholds (1)
            /// Bottom Track SNR(dB) shallow detection threshold.
            /// 
            /// Command: CBTT n.nnn, 2, 3, 4 \r
            /// Scale: dB
            /// Range: 0 to n.nnn
            /// </summary>
            public float CBTT_SNRShallowDetectionThresh
            {
                get { return _cBTT_SNRShallowDetectionThresh; }
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMin(value, MIN_CBTT_SNR_SHALLOW_THRESH))
                    {
                        _cBTT_SNRShallowDetectionThresh = value;
                    }
                }
            }

            /// <summary>
            /// Bottom Track Thresholds (2)
            /// Bottom Track Depth(m) at which the bottom track switches from
            /// using the shallow to the deep SNR.
            /// 
            /// Command: CBTT 1, n.nnn, 3, 4 \r
            /// Scale: Meters
            /// Range: 0 to n.nnn
            /// </summary>
            private float _cBTT_DepthSNR;
            /// <summary>
            /// Bottom Track Thresholds (2)
            /// Bottom Track Depth(m) at which the bottom track switches from
            /// using the shallow to the deep SNR.
            /// 
            /// Command: CBTT 1, n.nnn, 3, 4 \r
            /// Scale: Meters
            /// Range: 0 to n.nnn
            /// </summary>
            public float CBTT_DepthSNR
            {
                get { return _cBTT_DepthSNR; }
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMin(value, MIN_CBTT_DEPTH_SNR))
                    {
                        _cBTT_DepthSNR = value;
                    }
                }
            }

            /// <summary>
            /// Bottom Track Thresholds (3)
            /// SNR(dB) deep detection threshold.
            /// 
            /// Command: CBTT 1, 2, n.nnn, 4 \r
            /// Scale: dB
            /// Range: 0 to n.nnn 
            /// </summary>
            private float _cBTT_SNRDeepDetectionThresh;
            /// <summary>
            /// Bottom Track Thresholds (3)
            /// SNR(dB) deep detection threshold.
            /// 
            /// Command: CBTT 1, 2, n.nnn, 4 \r
            /// Scale: dB
            /// Range: 0 to n.nnn
            /// </summary>
            public float CBTT_SNRDeepDetectionThresh
            {
                get { return _cBTT_SNRDeepDetectionThresh; }
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMin(value, MIN_CBTT_SNR_DEEP_THRESH))
                    {
                        _cBTT_SNRDeepDetectionThresh = value;
                    }
                }
            }

            /// <summary>
            /// Bottom Track Thresholds (4)
            /// Bottom Track Depth(m) at which the bottom track swithes from low to high
            /// gain receive.
            /// 
            /// Command: CBTT 1, 2, 3, n.nnn, \r
            /// Scale: Meters
            /// Range: 0 to n.nnn
            /// </summary>
            private float _cBTT_DepthGain;
            /// <summary>
            /// Bottom Track Thresholds (4)
            /// Bottom Track Depth(m) at which the bottom track swithes from low to high
            /// gain receive.
            /// 
            /// Command: CBTT 1, 2, 3, n.nnn, \r
            /// Scale: Meters
            /// Range: 0 to n.nnn
            /// </summary>
            public float CBTT_DepthGain
            {
                get { return _cBTT_DepthGain; }
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMin(value, MIN_CBTT_DEPTH_GAIN))
                    {
                        _cBTT_DepthGain = value;
                    }
                }
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
            private float _cBTBL;
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
            public float CBTBL 
            {
                get { return _cBTBL; }
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMinMax(value, MIN_CBTBL, MAX_CBTBL))
                    {
                        _cBTBL = value;
                    }
                }
            }

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
            private float _cBTMX;
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
            public float CBTMX
            {
                get { return _cBTMX; }
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMinMax(value, MIN_CBTMX, MAX_CBTMX))
                    {
                        _cBTMX = value;
                    }
                }
            }

            /// <summary>
            /// Bottom Track Time Between Pings 
            /// Sets the time between bottom pings.
            /// 
            /// Command: CBTTBP n.nn[cr]
            /// Scale: seconds
            /// Range: 0.00 to 86400.0
            /// </summary>
            private float _cBTTBP;
            /// <summary>
            /// Bottom Track Time Between Pings 
            /// Sets the time between bottom pings.
            /// 
            /// Command: CBTTBP n.nn[cr]
            /// Scale: seconds
            /// Range: 0.00 to 86400.0
            /// </summary>
            public float CBTTBP
            {
                get { return _cBTTBP; }
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMinMax(value, MIN_CBTTBP, MAX_CBTTBP))
                    {
                        _cBTTBP = value;
                    }
                }
            }

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
            /// Water Track Blank (meters)
            /// Sets the vertical range from 
            /// the face of the transducer to 
            /// the first sample of the Bin.
            /// 
            /// Command: CWTBL n.nn[cr]
            /// Scale: meters
            /// Range: 0.00 to 100.0 
            /// </summary>
            private float _cWTBL;
            /// <summary>
            /// Water Track Blank (meters)
            /// Sets the vertical range from 
            /// the face of the transducer to 
            /// the first sample of the Bin.
            /// 
            /// Command: CWTBL n.nn[cr]
            /// Scale: meters
            /// Range: 0.00 to 100.0 
            /// </summary>
            public float CWTBL
            {
                get { return _cWTBL; }
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMinMax(value, MIN_CWTBL, MAX_CWTBL))
                    {
                        _cWTBL = value;
                    }
                }
            }

            /// <summary>
            /// Water Track Bin Size 
            /// Sets the vertical Bin size.
            /// 
            /// Command: CWTBS n.nn[cr]
            /// Scale: meters
            /// Range: 0.05 to 64.0 
            /// </summary>
            private float _cWTBS;
            /// <summary>
            /// Water Track Bin Size 
            /// Sets the vertical Bin size.
            /// 
            /// Command: CWTBS n.nn[cr]
            /// Scale: meters
            /// Range: 0.05 to 64.0 
            /// </summary>
            public float CWTBS
            {
                get { return _cWTBS; }
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMinMax(value, MIN_CWTBS, MAX_CWTBS))
                    {
                        _cWTBS = value;
                    }
                }
            }

            /// <summary>
            /// Water Track Time Between Pings 
            /// Sets the time between bottom pings.
            /// 
            /// Command: CWTTBP n.nn 
            /// Scale: seconds
            /// Range: 0.00 to 86400.0 
            /// </summary>
            private float _cWTTBP;
            /// <summary>
            /// Water Track Time Between Pings 
            /// Sets the time between bottom pings.
            /// 
            /// Command: CWTTBP n.nn 
            /// Scale: seconds
            /// Range: 0.00 to 86400.0 
            /// </summary>
            public float CWTTBP
            {
                get { return _cWTTBP; }
                set
                {
                    // Verify the value is within the range
                    if (Validator.ValidateMinMax(value, MIN_CWTTBP, MAX_CWTTBP))
                    {
                        _cWTTBP = value;
                    }
                }
            }

            #endregion

            /// <summary>
            /// Settings are associated with this subsystem.
            /// </summary>
            public SubsystemConfiguration SubsystemConfig { get; set; }

            #endregion

            /// <summary>
            /// Set the default values for the given subsystem.
            /// A default value of 0 is used for CEPO index.  If there is only
            /// 1 subsystem, then 0 should always work.
            /// </summary>
            /// <param name="ssConfig">Subsystem Configuration associated with these options.</param>
            public AdcpSubsystemCommands(SubsystemConfiguration ssConfig)
            {
                // Set the subsystem
                SubsystemConfig = ssConfig;

                // Set default values
                SetDefaults();
            }

            /// <summary>
            /// Use this constructor if all the settings are going to be
            /// set by the user.
            /// 
            /// If this constructor is used, then none of the 
            /// frequency dependent commands will be set to anything
            /// specific.  The subsystem must be set and 
            /// SetFrequencyDefaults() must be called for the
            /// frequency dependent commands to be set to default values.
            /// </summary>
            public AdcpSubsystemCommands()
            {
                // Set empty subsystem.
                SubsystemConfig = new SubsystemConfiguration();

                // Set Default values
                SetDefaultOptions();
            }

            #region Methods

            #region Set Defaults

            /// <summary>
            /// Set the default values.
            /// This will set the defaults
            /// plus the frequency default values.
            /// </summary>
            public void SetDefaults()
            {
                SetDefaultOptions();
                SetFrequencyDefaults();
            }

            /// <summary>
            /// Set the default values for the non-
            /// frequency dependent values.
            /// </summary>
            private void SetDefaultOptions()
            {
                // Water Profile defaults
                CWPON = DEFAULT_CWPON;
                CWPBB_TransmitPulseType = DEFAULT_CWPBB_TRANSMITPULSETYPE;
                CWPAI = new TimeValue();
                CWPST_CorrelationThresh = DEFAULT_CWPST_CORR_THRESH;
                CWPST_QVelocityThresh = DEFAULT_CWPST_QVEL_THRESH;
                CWPST_VVelocityThresh = DEFAULT_CWPST_VVEL_THRESH;
                CWPBP_NumPingsAvg = DEFAULT_CWPBP_NUM_PING_AVG;

                CBI_BurstInterval = new TimeValue();
                CBI_NumEnsembles = DEFAULT_CBI_NUM_ENS;
                CBI_BurstPairFlag = DEFAULT_CBI_BURST_PAIR_FLAG;

                // Bottom Track defaults
                CBTON = DEFAULT_CBTON;
                CBTBB_Mode = DEFAULT_CBTBB_MODE;
                CBTBB_PulseToPulseLag = DEFAULT_CBTBB_PULSETOPULSE_LAG;
                CBTST_CorrelationThresh = DEFAULT_CBTST_CORR_THRESH;
                CBTST_QVelocityThresh = DEFAULT_CBTST_QVEL_THRESHOLD;
                CBTST_VVelocityThresh = DEFAULT_CBTST_VVEL_THRESHOLD;
                CBTT_SNRShallowDetectionThresh = DEFAULT_CBTT_SNR_SHALLOW_DET_THRESHOLD;
                CBTT_SNRDeepDetectionThresh = DEFAULT_CBTT_SNR_DEEP_DET_THRESHOLD;

                // Water Track defaults
                CWTON = DEFAULT_CWTON;
                CWTBB = DEFAULT_CWTBB;
            }

            /// <summary>
            /// Set the default value based off the
            /// frequency.
            /// </summary>
            public void SetFrequencyDefaults()
            {
                if (SubsystemConfig.SubSystem != null)
                {
                    switch (SubsystemConfig.SubSystem.Code)
                    {
                        case Subsystem.SUB_38KHZ_VERT_PISTON_F:
                        case Subsystem.SUB_38KHZ_1BEAM_0DEG_ARRAY_Y:
                        case Subsystem.SUB_38KHZ_4BEAM_15DEG_ARRAY_S:
                        case Subsystem.SUB_38KHZ_4BEAM_30DEG_ARRAY_M:
                            Set38Defaults();
                            break;
                        case Subsystem.SUB_75KHZ_1BEAM_0DEG_ARRAY_X:
                        case Subsystem.SUB_75KHZ_4BEAM_15DEG_ARRAY_R:
                        case Subsystem.SUB_75KHZ_4BEAM_30DEG_ARRAY_L:
                        case Subsystem.SUB_75KHZ_VERT_PISTON_E:
                            Set75Defaults();
                            break;
                        case Subsystem.SUB_150KHZ_1BEAM_0DEG_ARRAY_W:
                        case Subsystem.SUB_150KHZ_4BEAM_15DEG_ARRAY_Q:
                        case Subsystem.SUB_150KHZ_4BEAM_30DEG_ARRAY_K:
                        case Subsystem.SUB_150KHZ_VERT_PISTON_D:
                            Set150Defaults();
                            break;
                        case Subsystem.SUB_600KHZ_1BEAM_0DEG_ARRAY_U:
                        case Subsystem.SUB_600KHZ_4BEAM_15DEG_ARRAY_O:
                        case Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3:
                        case Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_45OFFSET_7:
                        case Subsystem.SUB_600KHZ_4BEAM_30DEG_ARRAY_I:
                        case Subsystem.SUB_600KHZ_VERT_PISTON_B:
                            Set600Defaults();
                            break;
                        case Subsystem.SUB_1_2MHZ_4BEAM_20DEG_PISTON_2:
                        case Subsystem.SUB_1_2MHZ_4BEAM_20DEG_PISTON_45OFFSET_6:
                        case Subsystem.SUB_1_2MHZ_VERT_PISTON_A:
                            Set1200Defaults();
                            break;
                        case Subsystem.SUB_300KHZ_1BEAM_0DEG_ARRAY_V:
                        case Subsystem.SUB_300KHZ_4BEAM_15DEG_ARRAY_P:
                        case Subsystem.SUB_300KHZ_4BEAM_20DEG_PISTON_4:
                        case Subsystem.SUB_300KHZ_4BEAM_20DEG_PISTON_45OFFSET_8:
                        case Subsystem.SUB_300KHZ_4BEAM_30DEG_ARRAY_J:
                        case Subsystem.SUB_300KHZ_VERT_PISTON_C:
                        default:
                            Set300Defaults();
                            break;
                    }
                }
            }

            /// <summary>
            /// Set Default values for a 38 kHz
            /// system.
            /// </summary>
            private void Set38Defaults()
            {
                CWPBB_LagLength = DEFAULT_38_CWPBB_LAGLENGTH;
                CWPBL = DEFAULT_38_CWPBL;
                CWPBS = DEFAULT_38_CWPBS;
                CWPX = DEFAULT_38_CWPX;
                CWPBN = DEFAULT_38_CWPBN;
                CWPP = DEFAULT_38_CWPP;
                CWPTBP = DEFAULT_38_CWPTBP;
                CWPBP_TimeBetweenBasePings = DEFAULT_38_CWPBP_WATER_BASE_PING_TIME;
                CWPAP_NumPingsAvg = DEFAULT_38_CWPAP_NUMOFPINGSAVG;
                CWPAP_Lag = DEFAULT_38_CWPAP_LAG;
                CWPAP_Blank = DEFAULT_38_CWPAP_BLANK;
                CWPAP_BinSize = DEFAULT_38_CWPAP_BINSIZE;
                CWPAP_TimeBetweenPing = DEFAULT_38_CWPAP_TIME_BETWEEN_PINGS;

                CBTBB_LongRangeDepth = DEFAULT_38_CBTBB_LONGRANGEDEPTH;
                CBTBL = DEFAULT_38_CBTBL;
                CBTMX = DEFAULT_38_CBTMX;
                CBTTBP = DEFAULT_38_CBTTBP;
                CBTT_DepthSNR = DEFAULT_38_CBTT_DEPTH_SNR;
                CBTT_DepthGain = DEFAULT_38_CBTT_DEPTH_GAIN;
                
                CWTBL = DEFAULT_38_CWTBL;
                CWTBS = DEFAULT_38_CWTBS;
                CWTTBP = DEFAULT_38_CWTTBP;
            }

            /// <summary>
            /// Set Default values for a 75 kHz
            /// system.
            /// </summary>
            private void Set75Defaults()
            {
                CWPBB_LagLength = DEFAULT_75_CWPBB_LAGLENGTH;
                CWPBL = DEFAULT_75_CWPBL;
                CWPBS = DEFAULT_75_CWPBS;
                CWPX = DEFAULT_75_CWPX;
                CWPBN = DEFAULT_75_CWPBN;
                CWPP = DEFAULT_75_CWPP;
                CWPTBP = DEFAULT_75_CWPTBP;
                CWPBP_TimeBetweenBasePings = DEFAULT_75_CWPBP_WATER_BASE_PING_TIME;
                CWPAP_NumPingsAvg = DEFAULT_75_CWPAP_NUMOFPINGSAVG;
                CWPAP_Lag = DEFAULT_75_CWPAP_LAG;
                CWPAP_Blank = DEFAULT_75_CWPAP_BLANK;
                CWPAP_BinSize = DEFAULT_75_CWPAP_BINSIZE;
                CWPAP_TimeBetweenPing = DEFAULT_75_CWPAP_TIME_BETWEEN_PINGS;

                CBTBB_LongRangeDepth = DEFAULT_75_CBTBB_LONGRANGEDEPTH;
                CBTBL = DEFAULT_75_CBTBL;
                CBTMX = DEFAULT_75_CBTMX;
                CBTTBP = DEFAULT_75_CBTTBP;
                CBTT_DepthSNR = DEFAULT_75_CBTT_DEPTH_SNR;
                CBTT_DepthGain = DEFAULT_75_CBTT_DEPTH_GAIN;

                CWTBL = DEFAULT_75_CWTBL;
                CWTBS = DEFAULT_75_CWTBS;
                CWTTBP = DEFAULT_75_CWTTBP;
            }

            /// <summary>
            /// Set Default values for a 150 kHz
            /// system.
            /// </summary>
            private void Set150Defaults()
            {
                CWPBB_LagLength = DEFAULT_150_CWPBB_LAGLENGTH;
                CWPBL = DEFAULT_150_CWPBL;
                CWPBS = DEFAULT_150_CWPBS;
                CWPX = DEFAULT_150_CWPX;
                CWPBN = DEFAULT_150_CWPBN;
                CWPP = DEFAULT_150_CWPP;
                CWPTBP = DEFAULT_150_CWPTBP;
                CWPBP_TimeBetweenBasePings = DEFAULT_150_CWPBP_WATER_BASE_PING_TIME;
                CWPAP_NumPingsAvg = DEFAULT_150_CWPAP_NUMOFPINGSAVG;
                CWPAP_Lag = DEFAULT_150_CWPAP_LAG;
                CWPAP_Blank = DEFAULT_150_CWPAP_BLANK;
                CWPAP_BinSize = DEFAULT_150_CWPAP_BINSIZE;
                CWPAP_TimeBetweenPing = DEFAULT_150_CWPAP_TIME_BETWEEN_PINGS;

                CBTBB_LongRangeDepth = DEFAULT_150_CBTBB_LONGRANGEDEPTH;
                CBTBL = DEFAULT_150_CBTBL;
                CBTMX = DEFAULT_150_CBTMX;
                CBTTBP = DEFAULT_150_CBTTBP;
                CBTT_DepthSNR = DEFAULT_150_CBTT_DEPTH_SNR;
                CBTT_DepthGain = DEFAULT_150_CBTT_DEPTH_GAIN;
                
                CWTBL = DEFAULT_150_CWTBL;
                CWTBS = DEFAULT_150_CWTBS;
                CWTTBP = DEFAULT_150_CWTTBP;
            }

            /// <summary>
            /// Set Default values for a 300 kHz
            /// system.
            /// </summary>
            private void Set300Defaults()
            {
                CWPBB_LagLength = DEFAULT_300_CWPBB_LAGLENGTH;
                CWPBL = DEFAULT_300_CWPBL;
                CWPBS = DEFAULT_300_CWPBS;
                CWPX = DEFAULT_300_CWPX;
                CWPBN = DEFAULT_300_CWPBN;
                CWPP = DEFAULT_300_CWPP;
                CWPTBP = DEFAULT_300_CWPTBP;
                CWPBP_TimeBetweenBasePings = DEFAULT_300_CWPBP_WATER_BASE_PING_TIME;
                CWPAP_NumPingsAvg = DEFAULT_300_CWPAP_NUMOFPINGSAVG;
                CWPAP_Lag = DEFAULT_300_CWPAP_LAG;
                CWPAP_Blank = DEFAULT_300_CWPAP_BLANK;
                CWPAP_BinSize = DEFAULT_300_CWPAP_BINSIZE;
                CWPAP_TimeBetweenPing = DEFAULT_300_CWPAP_TIME_BETWEEN_PINGS;

                CBTBB_LongRangeDepth = DEFAULT_300_CBTBB_LONGRANGEDEPTH;
                CBTBL = DEFAULT_300_CBTBL;
                CBTMX = DEFAULT_300_CBTMX;
                CBTTBP = DEFAULT_300_CBTTBP;
                CBTT_DepthSNR = DEFAULT_300_CBTT_DEPTH_SNR;
                CBTT_DepthGain = DEFAULT_300_CBTT_DEPTH_GAIN;
                
                CWTBL = DEFAULT_300_CWTBL;
                CWTBS = DEFAULT_300_CWTBS;
                CWTTBP = DEFAULT_300_CWTTBP;
            }

            /// <summary>
            /// Set Default values for a 600 kHz
            /// system.
            /// </summary>
            private void Set600Defaults()
            {
                CWPBB_LagLength = DEFAULT_600_CWPBB_LAGLENGTH;
                CWPBL = DEFAULT_600_CWPBL;
                CWPBS = DEFAULT_600_CWPBS;
                CWPX = DEFAULT_600_CWPX;
                CWPBN = DEFAULT_600_CWPBN;
                CWPP = DEFAULT_600_CWPP;
                CWPTBP = DEFAULT_600_CWPTBP;
                CWPBP_TimeBetweenBasePings = DEFAULT_600_CWPBP_WATER_BASE_PING_TIME;
                CWPAP_NumPingsAvg = DEFAULT_600_CWPAP_NUMOFPINGSAVG;
                CWPAP_Lag = DEFAULT_600_CWPAP_LAG;
                CWPAP_Blank = DEFAULT_600_CWPAP_BLANK;
                CWPAP_BinSize = DEFAULT_600_CWPAP_BINSIZE;
                CWPAP_TimeBetweenPing = DEFAULT_600_CWPAP_TIME_BETWEEN_PINGS;

                CBTBB_LongRangeDepth = DEFAULT_600_CBTBB_LONGRANGEDEPTH;
                CBTBL = DEFAULT_600_CBTBL;
                CBTMX = DEFAULT_600_CBTMX;
                CBTTBP = DEFAULT_600_CBTTBP;
                CBTT_DepthSNR = DEFAULT_600_CBTT_DEPTH_SNR;
                CBTT_DepthGain = DEFAULT_600_CBTT_DEPTH_GAIN;
                
                CWTBL = DEFAULT_600_CWTBL;
                CWTBS = DEFAULT_600_CWTBS;
                CWTTBP = DEFAULT_600_CWTTBP;
            }

            /// <summary>
            /// Set Default values for a 1.2 MHz
            /// system.
            /// </summary>
            private void Set1200Defaults()
            {
                CWPBB_LagLength = DEFAULT_1200_CWPBB_LAGLENGTH;
                CWPBL = DEFAULT_1200_CWPBL;
                CWPBS = DEFAULT_1200_CWPBS;
                CWPX = DEFAULT_1200_CWPX;
                CWPBN = DEFAULT_1200_CWPBN;
                CWPP = DEFAULT_1200_CWPP;
                CWPTBP = DEFAULT_1200_CWPTBP;
                CWPBP_TimeBetweenBasePings = DEFAULT_1200_CWPBP_WATER_BASE_PING_TIME;
                CWPAP_NumPingsAvg = DEFAULT_1200_CWPAP_NUMOFPINGSAVG;
                CWPAP_Lag = DEFAULT_1200_CWPAP_LAG;
                CWPAP_Blank = DEFAULT_1200_CWPAP_BLANK;
                CWPAP_BinSize = DEFAULT_1200_CWPAP_BINSIZE;
                CWPAP_TimeBetweenPing = DEFAULT_1200_CWPAP_TIME_BETWEEN_PINGS;

                CBTBB_LongRangeDepth = DEFAULT_1200_CBTBB_LONGRANGEDEPTH;
                CBTBL = DEFAULT_1200_CBTBL;
                CBTMX = DEFAULT_1200_CBTMX;
                CBTTBP = DEFAULT_1200_CBTTBP;
                CBTT_DepthSNR = DEFAULT_1200_CBTT_DEPTH_SNR;
                CBTT_DepthGain = DEFAULT_1200_CBTT_DEPTH_GAIN;

                CWTBL = DEFAULT_1200_CWTBL;
                CWTBS = DEFAULT_1200_CWTBS;
                CWTTBP = DEFAULT_1200_CWTTBP;
            }

            #endregion

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
                list.Add(CWPON_CmdStr());              // CWPON
                list.Add(CWPBB_CmdStr());              // CWBB
                list.Add(CWPAP_CmdStr());              // CWPAP
                list.Add(CWPBP_CmdStr());              // CWPBP
                list.Add(CWPST_CmdStr());              // CWPST
                list.Add(CWPBL_CmdStr());              // CWPBL
                list.Add(CWPBS_CmdStr());              // CWPBS
                list.Add(CWPX_CmdStr());               // CWPX
                list.Add(CWPBN_CmdStr());              // CWPBN
                if (!IsEnableCWPAI())
                {
                    list.Add(CWPP_CmdStr());           // CWPP
                }
                else
                {
                    list.Add(CWPAI_CmdStr());          // CWPAI
                }
                list.Add(CWPTBP_CmdStr());             // CWPTBP

                list.Add(CBI_CmdStr());                // CBI

                list.Add(CBTON_CmdStr());              // CBTON
                list.Add(CBTBB_CmdStr());              // CBTBB
                list.Add(CBTST_CmdStr());              // CBTST
                list.Add(CBTBL_CmdStr());              // CBTBL
                //list.Add(CBTMX_CmdStr());            // CBTMX          // REMOVE BECAUSE A BUG IN FIRMWARE AS OF 2.11
                list.Add(CBTTBP_CmdStr());             // CBTTBP
                list.Add(CBTT_CmdStr());               // CBTT

                list.Add(CWTON_CmdStr());              // CWTON
                list.Add(CWTBB_CmdStr());              // CWTBB
                list.Add(CWTBL_CmdStr());              // CWTBL
                list.Add(CWTBS_CmdStr());              // CWTBS
                list.Add(CWTTBP_CmdStr());             // CWTTBP

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

                //list.Add(CBI_CmdStr());                // CBI
                list.Add(CWPBB_CmdStr());              // CWBB
                list.Add(CWPBL_CmdStr());              // CWPBL
                list.Add(CWPBS_CmdStr());              // CWPBS
                list.Add(CWPBN_CmdStr());              // CWPBN
                list.Add(CWPP_CmdStr());               // CWPP
                list.Add(CWPTBP_CmdStr());             // CWPTBP

                return list;
            }

            /// <summary>
            /// String of all the commands and there value.
            /// 
            /// Put all the values in United States English format.
            /// Other formats can use a comma instead of a decimal point for
            /// decimal numbers.
            /// </summary>
            /// <returns>String of all the commands and there value.</returns>
            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine(CWPON_CmdStr());         // CWPON
                builder.AppendLine(CWPBB_CmdStr());         // CWPBB
                builder.AppendLine(CWPAP_CmdStr());         // CWPAP
                builder.AppendLine(CWPBP_CmdStr());         // CWPBP
                builder.AppendLine(CWPST_CmdStr());         // CWPST
                builder.AppendLine(CWPBL_CmdStr());         // CWPBL
                builder.AppendLine(CWPBS_CmdStr());         // CWPBS
                builder.AppendLine(CWPX_CmdStr());          // CWPX
                builder.AppendLine(CWPBN_CmdStr());         // CWPBN
                builder.AppendLine(CWPP_CmdStr());          // CWPP
                builder.AppendLine(CWPAI_CmdStr());         // CWPAI
                builder.AppendLine(CWPTBP_CmdStr());        // CWPTBP

                builder.AppendLine(CBI_CmdStr());           // CBI

                builder.AppendLine(CBTON_CmdStr());         // CBTON
                builder.AppendLine(CBTBB_CmdStr());         // CBTBB
                builder.AppendLine(CBTST_CmdStr());         // CBTST
                builder.AppendLine(CBTBL_CmdStr());         // CBTBL
                //builder.AppendLine(CBTMX_CmdStr());       // CBTMX          // REMOVE BECAUSE A BUG IN FIRMWARE AS OF 2.11
                builder.AppendLine(CBTTBP_CmdStr());        // CBTTBP
                builder.AppendLine(CBTT_CmdStr());          // CBTT

                builder.AppendLine(CWTON_CmdStr());         // CWTON
                builder.AppendLine(CWTBB_CmdStr());         // CWTBB
                builder.AppendLine(CWTBL_CmdStr());         // CWTBL
                builder.AppendLine(CWTBS_CmdStr());         // CWTBS
                builder.AppendLine(CWTTBP_CmdStr());        // CWTTBP

                return builder.ToString();
            }

            /// <summary>
            /// If any of the CWPAI values are set, 
            /// return true, otherwise return false.
            /// This is checking to see whether the
            /// user wants to use CWPAI or CWPP.
            /// </summary>
            /// <returns>TRUE if any values set for CWPAI.</returns>
            public bool IsEnableCWPAI()
            {
                if (CWPAI.Hour > 0 ||
                    CWPAI.Minute > 0 || 
                    CWPAI.Second > 0 || 
                    CWPAI.HunSec > 0 )
                {
                    return true;
                }

                return false;
            }

            #region Command Strings

            #region CWPON

            /// <summary>
            /// Command String for the CWPBB command.
            /// 
            /// The CEPO index determines which configuration owns
            /// this command.  
            /// 
            /// The parameters are converted to english format
            /// so that numbers with a decimal point do not use a comma.
            /// </summary>
            /// <param name="cepoIndex">CEPO index.</param>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWPON_CmdStr(int cepoIndex)
            {
                return String.Format("{0}[{1}] {2}", CMD_CWPON, cepoIndex, CWPON_ToString());
            }

            /// <summary>
            /// Return the CWPON command string using the
            /// CEPO index for this object.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWPON_CmdStr()
            {
                return CWPON_CmdStr(SubsystemConfig.CepoIndex);
            }

            #endregion

            #region CWPBB

            /// <summary>
            /// Command String for the CWPBB command.
            /// 
            /// The CEPO index determines which configuration owns
            /// this command.  
            /// 
            /// The parameters are converted to english format
            /// so that numbers with a decimal point do not use a comma.
            /// </summary>
            /// <param name="cepoIndex">CEPO index.</param>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWPBB_CmdStr(int cepoIndex)
            {
                return String.Format("{0}[{1}] {2},{3}", CMD_CWPBB, cepoIndex, ((int)CWPBB_TransmitPulseType).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                                                                                    CWPBB_LagLength.ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            /// <summary>
            /// Return the CWPBB command string using the
            /// CEPO index for this object.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWPBB_CmdStr()
            {
                return CWPBB_CmdStr(SubsystemConfig.CepoIndex);
            }

            #endregion

            #region CWPAP
            
            /// <summary>
            /// Command String for the CWPAP command.
            /// 
            /// The CEPO index determines which configuration owns
            /// this command.  
            /// 
            /// The parameters are converted to english format
            /// so that numbers with a decimal point do not use a comma.
            /// </summary>
            /// <param name="cepoIndex">CEPO index.</param>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWPAP_CmdStr(int cepoIndex)
            {
                return String.Format("{0}[{1}] {2},{3},{4},{5},{6}", CMD_CWPAP, cepoIndex, CWPAP_NumPingsAvg.ToString(CultureInfo.CreateSpecificCulture("en-US")),
                                                                                                            CWPAP_Lag.ToString(CultureInfo.CreateSpecificCulture("en-US")),
                                                                                                            CWPAP_Blank.ToString(CultureInfo.CreateSpecificCulture("en-US")),
                                                                                                            CWPAP_BinSize.ToString(CultureInfo.CreateSpecificCulture("en-US")),
                                                                                                            CWPAP_TimeBetweenPing.ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            
            /// <summary>
            /// Return the CWPAP command string using the
            /// CEPO index for this object.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWPAP_CmdStr()
            {
                return CWPAP_CmdStr(SubsystemConfig.CepoIndex);
            }

            #endregion

            #region CWPBP

            /// <summary>
            /// Command String for the CWPBP command.
            /// 
            /// The CEPO index determines which configuration owns
            /// this command.  
            /// 
            /// The parameters are converted to english format
            /// so that numbers with a decimal point do not use a comma.
            /// </summary>
            /// <param name="cepoIndex">CEPO index.</param>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWPBP_CmdStr(int cepoIndex)
            {
                return String.Format("{0}[{1}] {2},{3}", CMD_CWPBP, cepoIndex, CWPBP_NumPingsAvg.ToString(CultureInfo.CreateSpecificCulture("en-US")),
                                                                                    CWPBP_TimeBetweenBasePings.ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }


            /// <summary>
            /// Return the CWPBP command string using the
            /// CEPO index for this object.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWPBP_CmdStr()
            {
                return CWPBP_CmdStr(SubsystemConfig.CepoIndex);
            }

            #endregion

            #region CWPST

            /// <summary>
            /// Command String for the CWPST command.
            /// 
            /// The CEPO index determines which configuration owns
            /// this command.  
            /// 
            /// The parameters are converted to english format
            /// so that numbers with a decimal point do not use a comma.
            /// </summary>
            /// <param name="cepoIndex">CEPO index.</param>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWPST_CmdStr(int cepoIndex)
            {
                return String.Format("{0}[{1}] {2},{3},{4}", CMD_CWPST, cepoIndex, CWPST_CorrelationThresh.ToString(CultureInfo.CreateSpecificCulture("en-US")),
                                                                                        CWPST_QVelocityThresh.ToString(CultureInfo.CreateSpecificCulture("en-US")),
                                                                                        CWPST_VVelocityThresh.ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }


            /// <summary>
            /// Return the CWPST command string using the
            /// CEPO index for this object.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWPST_CmdStr()
            {
                return CWPST_CmdStr(SubsystemConfig.CepoIndex);
            }

            #endregion

            #region CWPBL

            /// <summary>
            /// Command String for the CWPBL command.
            /// 
            /// The CEPO index determines which configuration owns
            /// this command.  
            /// 
            /// The parameters are converted to english format
            /// so that numbers with a decimal point do not use a comma.
            /// </summary>
            /// <param name="cepoIndex">CEPO index.</param>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWPBL_CmdStr(int cepoIndex)
            {
                return String.Format("{0}[{1}] {2}", CMD_CWPBL, cepoIndex, CWPBL.ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            /// <summary>
            /// Return the CWPBL command string using the
            /// CEPO index for this object.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWPBL_CmdStr()
            {
                return CWPBL_CmdStr(SubsystemConfig.CepoIndex);
            }

            #endregion

            #region CWPBS

            /// <summary>
            /// Command String for the CWPBS command.
            /// 
            /// The CEPO index determines which configuration owns
            /// this command.  
            /// 
            /// The parameters are converted to english format
            /// so that numbers with a decimal point do not use a comma.
            /// </summary>
            /// <param name="cepoIndex">CEPO index.</param>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWPBS_CmdStr(int cepoIndex)
            {
                return String.Format("{0}[{1}] {2}", CMD_CWPBS, cepoIndex, CWPBS.ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            /// <summary>
            /// Return the CWPBS command string using the
            /// CEPO index for this object.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWPBS_CmdStr()
            {
                return CWPBS_CmdStr(SubsystemConfig.CepoIndex);
            }

            #endregion

            #region CWPX

            /// <summary>
            /// Command String for the CWPX command.
            /// 
            /// The CEPO index determines which configuration owns
            /// this command.  
            /// 
            /// The parameters are converted to english format
            /// so that numbers with a decimal point do not use a comma.
            /// </summary>
            /// <param name="cepoIndex">CEPO index.</param>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWPX_CmdStr(int cepoIndex)
            {
                return String.Format("{0}[{1}] {2}", CMD_CWPX, cepoIndex, CWPX.ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            /// <summary>
            /// Return the CWPX command string using the
            /// CEPO index for this object.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWPX_CmdStr()
            {
                return CWPX_CmdStr(SubsystemConfig.CepoIndex);
            }

            #endregion

            #region CWPBN

            /// <summary>
            /// Command String for the CWPBN command.
            /// 
            /// The CEPO index determines which configuration owns
            /// this command.  
            /// 
            /// The parameters are converted to english format
            /// so that numbers with a decimal point do not use a comma.
            /// </summary>
            /// <param name="cepoIndex">CEPO index.</param>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWPBN_CmdStr(int cepoIndex)
            {
                return String.Format("{0}[{1}] {2}", CMD_CWPBN, cepoIndex, CWPBN.ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            /// <summary>
            /// Return the CWPBN command string using the
            /// CEPO index for this object.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWPBN_CmdStr()
            {
                return CWPBN_CmdStr(SubsystemConfig.CepoIndex);
            }

            #endregion

            #region CWPP

            /// <summary>
            /// Command String for the CWPP command.
            /// 
            /// The CEPO index determines which configuration owns
            /// this command.  
            /// 
            /// The parameters are converted to english format
            /// so that numbers with a decimal point do not use a comma.
            /// </summary>
            /// <param name="cepoIndex">CEPO index.</param>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWPP_CmdStr(int cepoIndex)
            {
                return String.Format("{0}[{1}] {2}", CMD_CWPP, cepoIndex, CWPP.ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            /// <summary>
            /// Return the CWPP command string using the
            /// CEPO index for this object.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWPP_CmdStr()
            {
                return CWPP_CmdStr(SubsystemConfig.CepoIndex);
            }

            #endregion

            #region CWPAI

            /// <summary>
            /// Command String for the CWPAI command.
            /// 
            /// The CEPO index determines which configuration owns
            /// this command.  
            /// 
            /// The parameters are converted to english format
            /// so that numbers with a decimal point do not use a comma.
            /// </summary>
            /// <param name="cepoIndex">CEPO index.</param>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWPAI_CmdStr(int cepoIndex)
            {
                return String.Format("{0}[{1}] {2}", CMD_CWPAI, cepoIndex, CWPAI.ToString());
            }

            /// <summary>
            /// Return the CWPAI command string using the
            /// CEPO index for this object.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWPAI_CmdStr()
            {
                return CWPAI_CmdStr(SubsystemConfig.CepoIndex);
            }

            #endregion

            #region CWPTBP

            /// <summary>
            /// Command String for the CWPTBP command.
            /// 
            /// The CEPO index determines which configuration owns
            /// this command.  
            /// 
            /// The parameters are converted to english format
            /// so that numbers with a decimal point do not use a comma.
            /// </summary>
            /// <param name="cepoIndex">CEPO index.</param>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWPTBP_CmdStr(int cepoIndex)
            {
                return String.Format("{0}[{1}] {2}", CMD_CWPTBP, cepoIndex, CWPTBP.ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            /// <summary>
            /// Return the CWPTBP command string using the
            /// CEPO index for this object.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWPTBP_CmdStr()
            {
                return CWPTBP_CmdStr(SubsystemConfig.CepoIndex);
            }

            #endregion

            #region CBI

            /// <summary>
            /// Command String for the CBI command.
            /// 
            /// The CEPO index determines which configuration owns
            /// this command.  
            /// 
            /// The parameters are converted to english format
            /// so that numbers with a decimal point do not use a comma.
            /// </summary>
            /// <param name="cepoIndex">CEPO index.</param>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CBI_CmdStr(int cepoIndex)
            {
                // Determine the pair flag value
                string pairFlag = "0";
                if (CBI_BurstPairFlag)
                {
                    pairFlag = "1";
                }

                return string.Format("{0}[{1}] {2},{3},{4}", CMD_CBI, cepoIndex, CBI_BurstInterval.ToString(),
                                                                                CBI_NumEnsembles.ToString(CultureInfo.CreateSpecificCulture("en-US")),
                                                                                pairFlag);
            }

            /// <summary>
            /// Return the CBI command string using the
            /// CEPO index for this object.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CBI_CmdStr()
            {
                return CBI_CmdStr(SubsystemConfig.CepoIndex);
            }

            #endregion

            #region CBTON

            /// <summary>
            /// Command String for the CBTON command.
            /// 
            /// The CEPO index determines which configuration owns
            /// this command.  
            /// 
            /// The parameters are converted to english format
            /// so that numbers with a decimal point do not use a comma.
            /// </summary>
            /// <param name="cepoIndex">CEPO index.</param>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CBTON_CmdStr(int cepoIndex)
            {
                return String.Format("{0}[{1}] {2}", CMD_CBTON, cepoIndex, CBTON_ToString());
            }

            /// <summary>
            /// Return the CBTON command string using the
            /// CEPO index for this object.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CBTON_CmdStr()
            {
                return CBTON_CmdStr(SubsystemConfig.CepoIndex);
            }

            #endregion

            #region CBTBB

            /// <summary>
            /// Command String for the CBTBB command.
            /// 
            /// The CEPO index determines which configuration owns
            /// this command.  
            /// 
            /// The parameters are converted to english format
            /// so that numbers with a decimal point do not use a comma.
            /// </summary>
            /// <param name="cepoIndex">CEPO index.</param>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CBTBB_CmdStr(int cepoIndex)
            {
                return String.Format("{0}[{1}] {2},{3},{4}", CMD_CBTBB, cepoIndex, ((int)CBTBB_Mode).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                                                                                        CBTBB_PulseToPulseLag.ToString(CultureInfo.CreateSpecificCulture("en-US")),
                                                                                        CBTBB_LongRangeDepth.ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            /// <summary>
            /// Return the CBTBB command string using the
            /// CEPO index for this object.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CBTBB_CmdStr()
            {
                return CBTBB_CmdStr(SubsystemConfig.CepoIndex);
            }

            #endregion

            #region CBTST

            /// <summary>
            /// Command String for the CBTST command.
            /// 
            /// The CEPO index determines which configuration owns
            /// this command.  
            /// 
            /// The parameters are converted to english format
            /// so that numbers with a decimal point do not use a comma.
            /// </summary>
            /// <param name="cepoIndex">CEPO index.</param>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CBTST_CmdStr(int cepoIndex)
            {
                return String.Format("{0}[{1}] {2},{3},{4}", CMD_CBTST, cepoIndex, CBTST_CorrelationThresh.ToString(CultureInfo.CreateSpecificCulture("en-US")),
                                                                                        CBTST_QVelocityThresh.ToString(CultureInfo.CreateSpecificCulture("en-US")),
                                                                                        CBTST_VVelocityThresh.ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            /// <summary>
            /// Return the CBTST command string using the
            /// CEPO index for this object.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CBTST_CmdStr()
            {
                return CBTST_CmdStr(SubsystemConfig.CepoIndex);
            }

            #endregion

            #region CBTBL

            /// <summary>
            /// Command String for the CBTBL command.
            /// 
            /// The CEPO index determines which configuration owns
            /// this command.  
            /// 
            /// The parameters are converted to english format
            /// so that numbers with a decimal point do not use a comma.
            /// </summary>
            /// <param name="cepoIndex">CEPO index.</param>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CBTBL_CmdStr(int cepoIndex)
            {
                return String.Format("{0}[{1}] {2}", CMD_CBTBL, cepoIndex, CBTBL.ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            /// <summary>
            /// Return the CBTBL command string using the
            /// CEPO index for this object.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CBTBL_CmdStr()
            {
                return CBTBL_CmdStr(SubsystemConfig.CepoIndex);
            }

            #endregion

            #region CBTMX

            /// <summary>
            /// Command String for the CBTMX command.
            /// 
            /// The CEPO index determines which configuration owns
            /// this command.  
            /// 
            /// The parameters are converted to english format
            /// so that numbers with a decimal point do not use a comma.
            /// </summary>
            /// <param name="cepoIndex">CEPO index.</param>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CBTMX_CmdStr(int cepoIndex)
            {
                return String.Format("{0}[{1}] {2}", CMD_CBTMX, cepoIndex, CBTMX.ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            /// <summary>
            /// Return the CBTMX command string using the
            /// CEPO index for this object.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CBTMX_CmdStr()
            {
                return CBTMX_CmdStr(SubsystemConfig.CepoIndex);
            }

            #endregion

            #region CBTTBP

            /// <summary>
            /// Command String for the CBTTBP command.
            /// 
            /// The CEPO index determines which configuration owns
            /// this command.  
            /// 
            /// The parameters are converted to english format
            /// so that numbers with a decimal point do not use a comma.
            /// </summary>
            /// <param name="cepoIndex">CEPO index.</param>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CBTTBP_CmdStr(int cepoIndex)
            {
                return String.Format("{0}[{1}] {2}", CMD_CBTTBP, cepoIndex, CBTTBP.ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            /// <summary>
            /// Return the CBTTBP command string using the
            /// CEPO index for this object.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CBTTBP_CmdStr()
            {
                return CBTTBP_CmdStr(SubsystemConfig.CepoIndex);
            }

            #endregion

            #region CBTT

            /// <summary>
            /// Command String for the CBTT command.
            /// 
            /// The CEPO index determines which configuration owns
            /// this command.  
            /// 
            /// The parameters are converted to english format
            /// so that numbers with a decimal point do not use a comma.
            /// </summary>
            /// <param name="cepoIndex">CEPO index.</param>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CBTT_CmdStr(int cepoIndex)
            {
                return String.Format("{0}[{1}] {2},{3},{4},{5}", CMD_CBTT, cepoIndex, CBTT_SNRShallowDetectionThresh.ToString(CultureInfo.CreateSpecificCulture("en-US")),
                                                                                            CBTT_DepthSNR.ToString(CultureInfo.CreateSpecificCulture("en-US")),
                                                                                            CBTT_SNRDeepDetectionThresh.ToString(CultureInfo.CreateSpecificCulture("en-US")),
                                                                                            CBTT_DepthGain.ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            /// <summary>
            /// Return the CBTT command string using the
            /// CEPO index for this object.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CBTT_CmdStr()
            {
                return CBTT_CmdStr(SubsystemConfig.CepoIndex);
            }

            #endregion

            #region CWTON

            /// <summary>
            /// Command String for the CWTON command.
            /// 
            /// The CEPO index determines which configuration owns
            /// this command.  
            /// 
            /// The parameters are converted to english format
            /// so that numbers with a decimal point do not use a comma.
            /// </summary>
            /// <param name="cepoIndex">CEPO index.</param>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWTON_CmdStr(int cepoIndex)
            {
                return String.Format("{0}[{1}] {2}", CMD_CWTON, cepoIndex, CWTON_ToString());
            }

            /// <summary>
            /// Return the CWTON command string using the
            /// CEPO index for this object.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWTON_CmdStr()
            {
                return CWTON_CmdStr(SubsystemConfig.CepoIndex);
            }

            #endregion

            #region CWTBB

            /// <summary>
            /// Command String for the CWTBB command.
            /// 
            /// The CEPO index determines which configuration owns
            /// this command.  
            /// 
            /// The parameters are converted to english format
            /// so that numbers with a decimal point do not use a comma.
            /// </summary>
            /// <param name="cepoIndex">CEPO index.</param>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWTBB_CmdStr(int cepoIndex)
            {
                return String.Format("{0}[{1}] {2}", CMD_CWTBB, cepoIndex, CWTBB_ToString());
            }

            /// <summary>
            /// Return the CWTBB command string using the
            /// CEPO index for this object.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWTBB_CmdStr()
            {
                return CWTBB_CmdStr(SubsystemConfig.CepoIndex);
            }

            #endregion

            #region CWTBL

            /// <summary>
            /// Command String for the CWTBL command.
            /// 
            /// The CEPO index determines which configuration owns
            /// this command.  
            /// 
            /// The parameters are converted to english format
            /// so that numbers with a decimal point do not use a comma.
            /// </summary>
            /// <param name="cepoIndex">CEPO index.</param>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWTBL_CmdStr(int cepoIndex)
            {
                return String.Format("{0}[{1}] {2}", CMD_CWTBL, cepoIndex, CWTBL.ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            /// <summary>
            /// Return the CWTBL command string using the
            /// CEPO index for this object.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWTBL_CmdStr()
            {
                return CWTBL_CmdStr(SubsystemConfig.CepoIndex);
            }

            #endregion

            #region CWTBS

            /// <summary>
            /// Command String for the CWTBS command.
            /// 
            /// The CEPO index determines which configuration owns
            /// this command.  
            /// 
            /// The parameters are converted to english format
            /// so that numbers with a decimal point do not use a comma.
            /// </summary>
            /// <param name="cepoIndex">CEPO index.</param>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWTBS_CmdStr(int cepoIndex)
            {
                return String.Format("{0}[{1}] {2}", CMD_CWTBS, cepoIndex, CWTBS.ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            /// <summary>
            /// Return the CWTBS command string using the
            /// CEPO index for this object.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWTBS_CmdStr()
            {
                return CWTBS_CmdStr(SubsystemConfig.CepoIndex);
            }

            #endregion

            #region CWTTBP

            /// <summary>
            /// Command String for the CWTTBP command.
            /// 
            /// The CEPO index determines which configuration owns
            /// this command.  
            /// 
            /// The parameters are converted to english format
            /// so that numbers with a decimal point do not use a comma.
            /// </summary>
            /// <param name="cepoIndex">CEPO index.</param>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWTTBP_CmdStr(int cepoIndex)
            {
                return String.Format("{0}[{1}] {2}", CMD_CWTTBP, cepoIndex, CWTTBP.ToString(CultureInfo.CreateSpecificCulture("en-US")));
            }

            /// <summary>
            /// Return the CWTTBP command string using the
            /// CEPO index for this object.
            /// </summary>
            /// <returns>Command to send to the ADCP with the parameters.</returns>
            public string CWTTBP_CmdStr()
            {
                return CWTTBP_CmdStr(SubsystemConfig.CepoIndex);
            }

            #endregion

            #endregion

            #endregion

        }
    }
}