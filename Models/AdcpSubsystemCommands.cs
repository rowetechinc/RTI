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
 *
 */

using System;
using System.Collections.Generic;
using System.Text;
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
            public const bool DEFAULT_CBTBB = false;

            /// <summary>
            /// Default Water Track Broadband.
            /// </summary>
            public const bool DEFAULT_CWTBB = false;

            /// <summary>
            /// Default Water Profile Broadband.
            /// </summary>
            public const bool DEFAULT_CWPBB = true;

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
            #endregion

            #endregion

            #region Min Max Values

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
            /// sets the number of pings in the ensemble.
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
            /// Water Profile Time between Pings
            /// Sets the time between profile pings
            /// 
            /// Command: CWPTBP n.nn \r
            /// Scale: seconds
            /// Range: 0.00 to 86400.0 
            /// </summary>
            public float CWPTBP { get; set; }

            #endregion // Water Profile

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

            /// <summary>
            /// Settings are associated with this subsystem.
            /// </summary>
            public Subsystem SubSystem { get; set; }

            #endregion

            /// <summary>
            /// Set the default values and the subsystem.
            /// </summary>
            /// <param name="ss">Subsystem associated with these options.</param>
            public AdcpSubsystemCommands(Subsystem ss)
            {
                // Set the subsystem
                SubSystem = ss;

                // Set default values
                SetDefaults();
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
                // Water Profile defaults
                CWPON = DEFAULT_CWPON;
                CWPBB = DEFAULT_CWPBB;
                CWPAI = new TimeValue();
                
                // Bottom Track defaults
                CBTON = DEFAULT_CBTON;
                CBTBB = DEFAULT_CBTBB;
                
                // Water Track defaults
                CWTON = DEFAULT_CWTON;
                CWTBB = DEFAULT_CWTBB;
                
                // Set the default frequency values
                SetFrequencyDefaults();
            }

            /// <summary>
            /// Set the default value based off the
            /// frequency.
            /// </summary>
            public void SetFrequencyDefaults()
            {

                switch(SubSystem.Code)
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
                    case Subsystem.SUB_300KHZ_1BEAM_0DEG_ARRAY_V:
                    case Subsystem.SUB_300KHZ_4BEAM_15DEG_ARRAY_P:
                    case Subsystem.SUB_300KHZ_4BEAM_20DEG_PISTON_4:
                    case Subsystem.SUB_300KHZ_4BEAM_20DEG_PISTON_45OFFSET_8:
                    case Subsystem.SUB_300KHZ_4BEAM_30DEG_ARRAY_J:
                    case Subsystem.SUB_300KHZ_VERT_PISTON_C:
                        Set300Defaults();
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
                    default:
                        break;
                }
            }

            /// <summary>
            /// Set Default values for a 38 kHz
            /// system.
            /// </summary>
            private void Set38Defaults()
            {
                CWPBL = DEFAULT_38_CWPBL;
                CWPBS = DEFAULT_38_CWPBS;
                CWPX = DEFAULT_38_CWPX;
                CWPBN = DEFAULT_38_CWPBN;
                CWPP = DEFAULT_38_CWPP;
                CWPTBP = DEFAULT_38_CWPTBP;
                
                CBTBL = DEFAULT_38_CBTBL;
                CBTMX = DEFAULT_38_CBTMX;
                CBTTBP = DEFAULT_38_CBTTBP;
                
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
                CWPBL = DEFAULT_75_CWPBL;
                CWPBS = DEFAULT_75_CWPBS;
                CWPX = DEFAULT_75_CWPX;
                CWPBN = DEFAULT_75_CWPBN;
                CWPP = DEFAULT_75_CWPP;
                CWPTBP = DEFAULT_75_CWPTBP;

                CBTBL = DEFAULT_75_CBTBL;
                CBTMX = DEFAULT_75_CBTMX;
                CBTTBP = DEFAULT_75_CBTTBP;

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
                CWPBL = DEFAULT_150_CWPBL;
                CWPBS = DEFAULT_150_CWPBS;
                CWPX = DEFAULT_150_CWPX;
                CWPBN = DEFAULT_150_CWPBN;
                CWPP = DEFAULT_150_CWPP;
                CWPTBP = DEFAULT_150_CWPTBP;
                
                CBTBL = DEFAULT_150_CBTBL;
                CBTMX = DEFAULT_150_CBTMX;
                CBTTBP = DEFAULT_150_CBTTBP;
                
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
                CWPBL = DEFAULT_300_CWPBL;
                CWPBS = DEFAULT_300_CWPBS;
                CWPX = DEFAULT_300_CWPX;
                CWPBN = DEFAULT_300_CWPBN;
                CWPP = DEFAULT_300_CWPP;
                CWPTBP = DEFAULT_300_CWPTBP;
                
                CBTBL = DEFAULT_300_CBTBL;
                CBTMX = DEFAULT_300_CBTMX;
                CBTTBP = DEFAULT_300_CBTTBP;
                
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
                CWPBL = DEFAULT_600_CWPBL;
                CWPBS = DEFAULT_600_CWPBS;
                CWPX = DEFAULT_600_CWPX;
                CWPBN = DEFAULT_600_CWPBN;
                CWPP = DEFAULT_600_CWPP;
                CWPTBP = DEFAULT_600_CWPTBP;
                
                CBTBL = DEFAULT_600_CBTBL;
                CBTMX = DEFAULT_600_CBTMX;
                CBTTBP = DEFAULT_600_CBTTBP;
                
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
                CWPBL = DEFAULT_1200_CWPBL;
                CWPBS = DEFAULT_1200_CWPBS;
                CWPX = DEFAULT_1200_CWPX;
                CWPBN = DEFAULT_1200_CWPBN;
                CWPP = DEFAULT_1200_CWPP;
                CWPTBP = DEFAULT_1200_CWPTBP;

                CBTBL = DEFAULT_1200_CBTBL;
                CBTMX = DEFAULT_1200_CBTMX;
                CBTTBP = DEFAULT_1200_CBTTBP;

                CWTBL = DEFAULT_1200_CWTBL;
                CWTBS = DEFAULT_1200_CWTBS;
                CWTTBP = DEFAULT_1200_CWTTBP;
            }

            #endregion

            /// <summary>
            /// Create a list of all the commands and there value.
            /// Add all the commands to the list and there value.
            /// </summary>
            /// <returns>List of all the commands and there value.</returns>
            public List<string> GetCommandList()
            {
                List<string> list = new List<string>();
                list.Add(String.Format("{0}[{1}] {2}", CMD_CWPON, SubSystem.Index.ToString(), CWPON_ToString()));        // CWPON
                list.Add(String.Format("{0}[{1}] {2}", CMD_CWPBB, SubSystem.Index.ToString(), CWPBB_ToString()));        // CWBB
                list.Add(String.Format("{0}[{1}] {2}", CMD_CWPBL, SubSystem.Index.ToString(), CWPBL));                   // CWPBL
                list.Add(String.Format("{0}[{1}] {2}", CMD_CWPBS, SubSystem.Index.ToString(), CWPBS));                   // CWPBS
                list.Add(String.Format("{0}[{1}] {2}", CMD_CWPX, SubSystem.Index.ToString(), CWPX));                     // CWPX
                list.Add(String.Format("{0}[{1}] {2}", CMD_CWPBN, SubSystem.Index.ToString(), CWPBN));                   // CWPBN

                if (!IsEnableCWPAI())
                {
                    list.Add(String.Format("{0}[{1}] {2}", CMD_CWPP, SubSystem.Index.ToString(), CWPP));                 // CWPP
                }
                else
                {
                    list.Add(String.Format("{0}[{1}] {2}", CMD_CWPAI, SubSystem.Index.ToString(), CWPAI.ToString()));    // CWPAI
                }
                list.Add(String.Format("{0}[{1}] {2}", CMD_CWPTBP, SubSystem.Index.ToString(), CWPTBP));                 // CWPTBP

                list.Add(String.Format("{0}[{1}] {2}", CMD_CBTON, SubSystem.Index.ToString(), CBTON_ToString()));        // CBTON
                list.Add(String.Format("{0}[{1}] {2}", CMD_CBTBB, SubSystem.Index.ToString(), CBTBB_ToString()));        // CBTBB
                list.Add(String.Format("{0}[{1}] {2}", CMD_CBTBL, SubSystem.Index.ToString(), CBTBL));                   // CBTBL
                list.Add(String.Format("{0}[{1}] {2}", CMD_CBTMX, SubSystem.Index.ToString(), CBTMX));                   // CBTMX
                list.Add(String.Format("{0}[{1}] {2}", CMD_CBTTBP, SubSystem.Index.ToString(), CBTTBP));                 // CBTTBP

                list.Add(String.Format("{0}[{1}] {2}", CMD_CWTON, SubSystem.Index.ToString(), CWTON_ToString()));        // CWTON
                list.Add(String.Format("{0}[{1}] {2}", CMD_CWTBB, SubSystem.Index.ToString(), CWTBB_ToString()));        // CWTBB
                list.Add(String.Format("{0}[{1}] {2}", CMD_CWTBL, SubSystem.Index.ToString(), CWTBL));                   // CWTBL
                list.Add(String.Format("{0}[{1}] {2}", CMD_CWTBS, SubSystem.Index.ToString(), CWTBS));                   // CWTBS
                list.Add(String.Format("{0}[{1}] {2}", CMD_CWTTBP, SubSystem.Index.ToString(), CWTTBP));                 // CWTTBP

                return list;
            }

            /// <summary>
            /// String of all the commands and there value.
            /// </summary>
            /// <returns>String of all the commands and there value.</returns>
            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(String.Format("{0}[{1}] {2}\n", CMD_CWPON, SubSystem.Index.ToString(), CWPON_ToString()));        // CWPON
                builder.Append(String.Format("{0}[{1}] {2}\n", CMD_CWPBB, SubSystem.Index.ToString(), CWPBB_ToString()));        // CWBB
                builder.Append(String.Format("{0}[{1}] {2}\n", CMD_CWPBL, SubSystem.Index.ToString(), CWPBL));                   // CWPBL
                builder.Append(String.Format("{0}[{1}] {2}\n", CMD_CWPBS, SubSystem.Index.ToString(), CWPBS));                   // CWPBS
                builder.Append(String.Format("{0}[{1}] {2}\n", CMD_CWPX, SubSystem.Index.ToString(), CWPX));                     // CWPX
                builder.Append(String.Format("{0}[{1}] {2}\n", CMD_CWPBN, SubSystem.Index.ToString(), CWPBN));                   // CWPBN
                builder.Append(String.Format("{0}[{1}] {2}\n", CMD_CWPP, SubSystem.Index.ToString(), CWPP));                     // CWPP
                builder.Append(String.Format("{0}[{1}] {2}\n", CMD_CWPAI, SubSystem.Index.ToString(), CWPAI.ToString()));        // CWPAI
                builder.Append(String.Format("{0}[{1}] {2}\n", CMD_CWPTBP, SubSystem.Index.ToString(), CWPTBP));                 // CWPTBP

                builder.Append(String.Format("{0}[{1}] {2}\n", CMD_CBTON, SubSystem.Index.ToString(), CBTON_ToString()));        // CBTON
                builder.Append(String.Format("{0}[{1}] {2}\n", CMD_CBTBB, SubSystem.Index.ToString(), CBTBB_ToString()));        // CBTBB
                builder.Append(String.Format("{0}[{1}] {2}\n", CMD_CBTBL, SubSystem.Index.ToString(), CBTBL));                   // CBTBL
                builder.Append(String.Format("{0}[{1}] {2}\n", CMD_CBTMX, SubSystem.Index.ToString(), CBTMX));                   // CBTMX
                builder.Append(String.Format("{0}[{1}] {2}\n", CMD_CBTTBP, SubSystem.Index.ToString(), CBTTBP));                 // CBTTBP

                builder.Append(String.Format("{0}[{1}] {2}\n", CMD_CWTON, SubSystem.Index.ToString(), CWTON_ToString()));        // CWTON
                builder.Append(String.Format("{0}[{1}] {2}\n", CMD_CWTBB, SubSystem.Index.ToString(), CWTBB_ToString()));        // CWTBB
                builder.Append(String.Format("{0}[{1}] {2}\n", CMD_CWTBL, SubSystem.Index.ToString(), CWTBL));                   // CWTBL
                builder.Append(String.Format("{0}[{1}] {2}\n", CMD_CWTBS, SubSystem.Index.ToString(), CWTBS));                   // CWTBS
                builder.Append(String.Format("{0}[{1}] {2}\n", CMD_CWTTBP, SubSystem.Index.ToString(), CWTTBP));                 // CWTTBP

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

            #endregion

        }
    }
}