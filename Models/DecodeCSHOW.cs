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
 * 09/22/2012      RC          2.15       Initial coding
 * 09/26/2012      RC          2.15       Added WP commands.
 * 09/27/2012      RC          2.15       Fixed bug in Decoding Indexed values.  It did not handle spaces correctly.
 * 09/28/2012      RC          2.15       Added all remaining commands.
 * 10/04/2012      RC          2.15       Added CBI command.
 * 09/17/2013      RC          2.20.0     Updated DecodeCERECORD() with the new CERECORD command.
 * 
 */
namespace RTI
{
    using System;
    using System.Text.RegularExpressions;
    using System.Collections.Generic;
    using RTI.Commands;

    /// <summary>
    /// Decode the CSHOW command.  This will give all
    /// the settings currently set on an ADCP.
    /// </summary>
    public class DecodeCSHOW
    {

        /// <summary>
        /// Initialize the values.
        /// </summary>
        public DecodeCSHOW()
        {
            // Initialize the values

        }

        /// <summary>
        /// Decode the command and return all the settings found.
        /// Send the command CSHOW and give the string result.
        /// </summary>
        /// <param name="buffer">Command output from the ADCP.</param>
        /// <param name="serial">Serial number used to determine the system type.</param>
        /// <returns>All the settings found from CSHOW.</returns>
        public AdcpConfiguration Decode(string buffer, SerialNumber serial)
        {
            AdcpConfiguration config = new AdcpConfiguration();
            config.SerialNumber = serial;

            // Decode each line in CSHOW
            string[] results = buffer.Split(new char[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
            for (int x = 0; x < results.Length; x++)
            {

                // Mode
                if (results[x].Contains("Mode"))
                {
                    DecodeMode(results[x], ref config);
                }

                // CEI
                if (results[x].Contains(AdcpCommands.CMD_CEI))
                {
                    DecodeCEI(results[x], ref config);
                }

                // CETFP
                if (results[x].Contains(AdcpCommands.CMD_CETFP))
                {
                    DecodeCETFP(results[x], ref config);
                }

                // CERECORD
                if (results[x].Contains(AdcpCommands.CMD_CERECORD))
                {
                    DecodeCERECORD(results[x], ref config);
                }

                // CEOUTPUT
                if (results[x].Contains(AdcpCommands.CMD_CEOUTPUT))
                {
                    DecodeCEOUTPUT(results[x], ref config);
                }

                // CEPO
                // MUST BE SET BEFORE ALL THE SUBSYSTEM COMMANDS ARE DECODED
                if (results[x].Contains(AdcpCommands.CMD_CEPO))
                {               
                    DecodeCEPO(results[x], serial, ref config);
                }

                // CWPON
                if (results[x].Contains(AdcpSubsystemCommands.CMD_CWPON))
                {
                    DecodeCWPON(results[x], ref config);
                }

                // CWPBB
                if (results[x].Contains(AdcpSubsystemCommands.CMD_CWPBB))
                {
                    DecodeCWPBB(results[x], ref config);
                }

                // CWPAP
                if (results[x].Contains(AdcpSubsystemCommands.CMD_CWPAP))
                {
                    DecodeCWPAP(results[x], ref config);
                }

                // CWPST
                if (results[x].Contains(AdcpSubsystemCommands.CMD_CWPST))
                {
                    DecodeCWPST(results[x], ref config);
                }

                // CWPBL
                if (results[x].Contains(AdcpSubsystemCommands.CMD_CWPBL))
                {
                    DecodeCWPBL(results[x], ref config);
                }

                // CWPBS
                if (results[x].Contains(AdcpSubsystemCommands.CMD_CWPBS))
                {
                    DecodeCWPBS(results[x], ref config);
                }

                // CWPX
                if (results[x].Contains(AdcpSubsystemCommands.CMD_CWPX))
                {
                    DecodeCWPX(results[x], ref config);
                }

                // CWPBN
                if (results[x].Contains(AdcpSubsystemCommands.CMD_CWPBN))
                {
                    DecodeCWPBN(results[x], ref config);
                }

                // CWPP
                if (results[x].Contains(AdcpSubsystemCommands.CMD_CWPP))
                {
                    DecodeCWPP(results[x], ref config);
                }

                // CWPBP
                if (results[x].Contains(AdcpSubsystemCommands.CMD_CWPBP))
                {
                    DecodeCWPBP(results[x], ref config);
                }

                // CWPTBP
                if (results[x].Contains(AdcpSubsystemCommands.CMD_CWPTBP))
                {
                    DecodeCWPTBP(results[x], ref config);
                }

                // CWPAI
                if (results[x].Contains(AdcpSubsystemCommands.CMD_CWPAI))
                {
                    DecodeCWPAI(results[x], ref config);
                }

                // CBTON
                if (results[x].Contains(AdcpSubsystemCommands.CMD_CBTON))
                {
                    DecodeCBTON(results[x], ref config);
                }

                // CBTBB
                if (results[x].Contains(AdcpSubsystemCommands.CMD_CBTBB))
                {
                    DecodeCBTBB(results[x], ref config);
                }

                // CBTST
                if (results[x].Contains(AdcpSubsystemCommands.CMD_CBTST))
                {
                    DecodeCBTST(results[x], ref config);
                }

                // CBTBL
                if (results[x].Contains(AdcpSubsystemCommands.CMD_CBTBL))
                {
                    DecodeCBTBL(results[x], ref config);
                }

                // CBTMX
                if (results[x].Contains(AdcpSubsystemCommands.CMD_CBTMX))
                {
                    DecodeCBTMX(results[x], ref config);
                }

                // CBTTBP
                if (results[x].Contains(AdcpSubsystemCommands.CMD_CBTTBP))
                {
                    DecodeCBTTBP(results[x], ref config);
                }

                // CBTT
                if (results[x].Contains(AdcpSubsystemCommands.CMD_CBTT))
                {
                    DecodeCBTT(results[x], ref config);
                }

                // CWTON
                if (results[x].Contains(AdcpSubsystemCommands.CMD_CWTON))
                {
                    DecodeCWTON(results[x], ref config);
                }

                // CWTBB
                if (results[x].Contains(AdcpSubsystemCommands.CMD_CWTBB))
                {
                    DecodeCWTBB(results[x], ref config);
                }

                // CWTBL
                if (results[x].Contains(AdcpSubsystemCommands.CMD_CWTBL))
                {
                    DecodeCWTBL(results[x], ref config);
                }

                // CWTBS
                if (results[x].Contains(AdcpSubsystemCommands.CMD_CWTBS))
                {
                    DecodeCWTBS(results[x], ref config);
                }

                // CWTTBP
                if (results[x].Contains(AdcpSubsystemCommands.CMD_CWTTBP))
                {
                    DecodeCWTTBP(results[x], ref config);
                }

                // CWS
                // Ensure its CWS and not CWSS
                if (results[x].Contains(AdcpCommands.CMD_CWS) && !results[x].Contains(AdcpCommands.CMD_CWSS))
                {
                    DecodeCWS(results[x], ref config);
                }

                // CWT
                if (results[x].Contains(AdcpCommands.CMD_CWT))
                {
                    DecodeCWT(results[x], ref config);
                }

                // CVSF
                if (results[x].Contains(AdcpCommands.CMD_CVSF))
                {
                    DecodeCVSF(results[x], ref config);
                }

                // CHS
                if (results[x].Contains(AdcpCommands.CMD_CHS))
                {
                    DecodeCHS(results[x], ref config);
                }

                // CHO
                if (results[x].Contains(AdcpCommands.CMD_CHO))
                {
                    DecodeCHO(results[x], ref config);
                }

                // CWSS
                if (results[x].Contains(AdcpCommands.CMD_CWSS))
                {
                    DecodeCWSS(results[x], ref config);
                }

                // CTD
                if (results[x].Contains(AdcpCommands.CMD_CTD))
                {
                    DecodeCTD(results[x], ref config);
                }

                // C232B
                if (results[x].Contains(AdcpCommands.CMD_C232B))
                {
                    DecodeC232B(results[x], ref config);
                }

                // C485B
                if (results[x].Contains(AdcpCommands.CMD_C485B))
                {
                    DecodeC485B(results[x], ref config);
                }

                // C422B
                if (results[x].Contains(AdcpCommands.CMD_C422B))
                {
                    DecodeC422B(results[x], ref config);
                }

                // CBI
                if (results[x].Contains(AdcpSubsystemCommands.CMD_CBI))
                {
                    DecodeCBI(results[x], ref config);
                }
            }

            return config;
        }

        /// <summary>
        /// Decode the CEPO command.  This contains the number
        /// of configurations the system will contain and which
        /// order these configurations will ping.
        /// 
        /// CEPO is a special command for AdcpConfiguration.  It sets
        /// the number of SubsystemConfigurations to create.  It must be
        /// the first command to be decoded to ensure that SubsystemConfigurations
        /// are created so they can be updated with the command values later.
        /// </summary>
        /// <param name="buffer">Line for the command.</param>
        /// <param name="serial">Serial Number to determine the system type.</param>
        /// <param name="config">AdcpConfiguration to set the value.</param>
        /// <returns>CEPO string.</returns>
        private string  DecodeCEPO(string buffer, SerialNumber serial, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            // Check that the buffer contains 2 items and the first item is CEPO command
            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpCommands.CMD_CEPO))
            {
                // Set the CEPO to the AdcpConfiguration
                // CEPO is a speical command to set to AdcpConfiguration
                config.SetCepo(result[1], serial);

                return result[1];
            }

            // Command given was not correct
            return "";
        }

        /// <summary>
        /// Decode the Mode line.  This will state whether the ADCP is set to 
        /// DVL or Profile mode.
        /// </summary>
        /// <param name="buffer">Line for the command.</param>
        /// <param name="config">AdcpConfiguration to set the value.</param>
        private void DecodeMode(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (result.Length > 1 && result[0].Contains("Mode"))
            {
                // Profile mode
                if (result[1].ToLower().Contains("profile"))
                {
                    config.Commands.Mode = Commands.AdcpCommands.AdcpMode.PROFILE;
                }

                // DVL mode
                if (result[1].ToLower().Contains("dvl"))
                {
                    config.Commands.Mode = Commands.AdcpCommands.AdcpMode.DVL;
                }
            }
        }

        /// <summary>
        /// Decode the CEI line.  This is the Ensemble interval.
        /// 
        /// Ex:
        /// CEI 00:00:00.25
        /// </summary>
        /// <param name="buffer">Line for the command.</param>
        /// <param name="config">AdcpConfiguration to set the value.</param>
        private void DecodeCEI(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpCommands.CMD_CEI))
            {
                // Decode the TimeValue and set to CEI
                TimeValue time = DecodeTimeValue(result[1]);
                config.Commands.CEI = time;
            }
        }

        /// <summary>
        /// Decode the CETFP line.  Time and date of first ping.
        /// 
        /// Ex:
        /// CETFP 2012/09/18,14:00:00.00
        /// </summary>
        /// <param name="buffer">Line for the command.</param>
        /// <param name="config">AdcpConfiguration to set the value.</param>
        private void DecodeCETFP(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpCommands.CMD_CETFP))
            {
                // Split the values by the colon, then try to parse the values
                string[] values = result[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (values.Length >= 2)
                {
                    bool yearGood = false;
                    bool monthGood = false;
                    bool dayGood = false;
                    bool hrGood = false;
                    bool minGood = false;
                    bool secGood = false;
                    bool hunSecGood = false;

                    UInt16 year = 0;
                    UInt16 month = 0;
                    UInt16 day = 0;
                    UInt16 hr = 0;
                    UInt16 min = 0;
                    UInt16 sec = 0;
                    UInt16 hunSec = 0;

                    #region Convert Date

                    // Convert the date
                    string[] dates = values[0].Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                    if (dates.Length >= 3)
                    {
                        yearGood = UInt16.TryParse(dates[0], out year);
                        monthGood = UInt16.TryParse(dates[1], out month);
                        dayGood = UInt16.TryParse(dates[2], out day);
                    }

                    #endregion

                    #region Convert Time

                    // Split the values by the colon, then try to parse the values
                    string[] times = values[1].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                    if (times.Length >= 3)
                    {
                        hrGood = UInt16.TryParse(times[0], out hr);
                        minGood = UInt16.TryParse(times[1], out min);

                        // Split the seconds into seconds and hundredth of second
                        if (times[2].Contains("."))
                        {
                            string[] secValues = times[2].Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                            if (secValues.Length >= 2)
                            {
                                secGood = UInt16.TryParse(secValues[0], out sec);
                                hunSecGood = UInt16.TryParse(secValues[1], out hunSec);
                            }
                        }
                    }

                    #endregion

                    // If all the values are good, set the values to config
                    if ( year + month + day + hr + min + sec > 0  && yearGood && monthGood && dayGood && hrGood && minGood && secGood && hunSecGood)
                    {
                        //config.Commands.CETFP_Year = year;
                        //config.Commands.CETFP_Month = month;
                        //config.Commands.CETFP_Day = day;
                        //config.Commands.CETFP_Hour = hr;
                        //config.Commands.CETFP_Minute = min;
                        //config.Commands.CETFP_Second = sec;
                        //config.Commands.CETFP_HunSec = hunSec;

                        config.Commands.CETFP = new DateTime(year, month, day, hr, min, sec, (int)Math.Round(hunSec * MathHelper.HUNSEC_TO_MILLISEC));
                    }


                }
            }
        }

        /// <summary>
        /// Decode the CERECORD line.  Flag to turn on or off internal recording.
        /// 
        /// Ex:
        /// CERECORD 0,0
        /// </summary>
        /// <param name="buffer">Line for the command.</param>
        /// <param name="config">AdcpConfiguration to set the value.</param>
        private void DecodeCERECORD(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpCommands.CMD_CERECORD))
            {
                // Split the values by the colon, then try to parse the values
                string[] values = result[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (values.Length >= 2)
                {
                    // CERECORD Ensemble Ping
                    int crecordValue = 0;
                    if (int.TryParse(values[0], out crecordValue))
                    {
                        if (crecordValue == 1)
                        {
                            config.Commands.CERECORD_EnsemblePing = true;
                        }
                        else
                        {
                            config.Commands.CERECORD_EnsemblePing = false;
                        }
                    }

                    // CRECORD Single Ping
                    int crecordSinglePingValue = 0;
                    if (int.TryParse(values[1], out crecordSinglePingValue))
                    {
                        if (crecordSinglePingValue == 1)
                        {
                            config.Commands.CERECORD_SinglePing = true;
                        }
                        else
                        {
                            config.Commands.CERECORD_SinglePing = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Decode the CEOUTPUT line.  Flag to turn on or off ensemble output to serial port.
        /// 
        /// Ex:
        /// CEOUTPUT 0
        /// </summary>
        /// <param name="buffer">Line for the command.</param>
        /// <param name="config">AdcpConfiguration to set the value.</param>
        private void DecodeCEOUTPUT(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpCommands.CMD_CEOUTPUT))
            {
                UInt16 flag = 0;
                if (UInt16.TryParse(result[1], out flag))
                {
                    switch(flag)
                    {
                        case (ushort)AdcpCommands.AdcpOutputMode.Disable:
                            config.Commands.CEOUTPUT = AdcpCommands.AdcpOutputMode.Disable;
                            break;
                        case (ushort)AdcpCommands.AdcpOutputMode.Binary:
                            config.Commands.CEOUTPUT = AdcpCommands.AdcpOutputMode.Binary;
                            break;
                        case (ushort)AdcpCommands.AdcpOutputMode.ASCII:
                            config.Commands.CEOUTPUT = AdcpCommands.AdcpOutputMode.ASCII;
                            break;
                        default:
                            config.Commands.CEOUTPUT = AdcpCommands.DEFAULT_CEOUTPUT;
                            break;

                    }
                }
            }
        }

        /// <summary>
        /// Decode CWS command.  
        /// It consist of 1 values.
        /// 
        /// If any of the steps fail, then the default will will remain in config.
        /// 
        /// Ex:
        /// CWS 0.00
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCWS(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpCommands.CMD_CWS))
            {
                float value = 0;
                if (float.TryParse(result[1], out value))
                {
                    config.Commands.CWS = value;
                }
            }
        }

        /// <summary>
        /// Decode CWT command.  
        /// It consist of 1 values.
        /// 
        /// If any of the steps fail, then the default will will remain in config.
        /// 
        /// Ex:
        /// CWT 15.00
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCWT(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpCommands.CMD_CWT))
            {
                float value = 0;
                if (float.TryParse(result[1], out value))
                {
                    config.Commands.CWT = value;
                }
            }
        }

        /// <summary>
        /// Decode CTD command.  
        /// It consist of 1 values.
        /// 
        /// If any of the steps fail, then the default will will remain in config.
        /// 
        /// Ex:
        /// CTD 0.00
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCTD(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpCommands.CMD_CTD))
            {
                float value = 0;
                if (float.TryParse(result[1], out value))
                {
                    config.Commands.CTD = value;
                }
            }
        }

        /// <summary>
        /// Decode CWSS command.  
        /// It consist of 1 values.
        /// 
        /// If any of the steps fail, then the default will will remain in config.
        /// 
        /// Ex:
        /// CWSS 0.00
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCWSS(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpCommands.CMD_CWSS))
            {
                float value = 0;
                if (float.TryParse(result[1], out value))
                {
                    config.Commands.CWSS = value;
                }
            }
        }

        /// <summary>
        /// Decode CHO command.  
        /// It consist of 1 values.
        /// 
        /// If any of the steps fail, then the default will will remain in config.
        /// 
        /// Ex:
        /// CHO 0.00
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCHO(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpCommands.CMD_CHO))
            {
                float value = 0;
                if (float.TryParse(result[1], out value))
                {
                    config.Commands.CHO = value;
                }
            }
        }

        /// <summary>
        /// Decode CHS command.  
        /// It consist of 1 values.
        /// 
        /// If any of the steps fail, then the default will will remain in config.
        /// 
        /// Ex:
        /// CHS 1
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCHS(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpCommands.CMD_CHS))
            {
                int value = 0;
                if (int.TryParse(result[1], out value))
                {
                    // Check if internal
                    if (value == (int)HeadingSrc.INTERNAL)
                    {
                        config.Commands.CHS = HeadingSrc.INTERNAL;
                    }

                    // Check if serial
                    if (value == (int)HeadingSrc.SERIAL)
                    {
                        config.Commands.CHS = HeadingSrc.SERIAL;
                    }
                }
            }
        }

        /// <summary>
        /// Decode CVSF command.  
        /// It consist of 1 values.
        /// 
        /// If any of the steps fail, then the default will will remain in config.
        /// 
        /// Ex:
        /// CVSF 1.00
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCVSF(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpCommands.CMD_CVSF))
            {
                float value = 0;
                if (float.TryParse(result[1], out value))
                {
                    config.Commands.CVSF = value;
                }
            }
        }

        /// <summary>
        /// Decode C232B command.  
        /// It consist of 1 values.
        /// 
        /// If any of the steps fail, then the default will will remain in config.
        /// 
        /// Ex:
        /// C232B 115200
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeC232B(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpCommands.CMD_C232B))
            {
                int value = 0;
                if (int.TryParse(result[1], out value))
                {
                    switch(value)
                    {
                        case (int)Baudrate.BAUD_4800:
                            config.Commands.C232B = Baudrate.BAUD_4800;
                            break;
                        case (int)Baudrate.BAUD_2400:
                            config.Commands.C232B = Baudrate.BAUD_2400;
                            break;
                        case (int)Baudrate.BAUD_9600:
                            config.Commands.C232B = Baudrate.BAUD_9600;
                            break;
                        case (int)Baudrate.BAUD_38400:
                            config.Commands.C232B = Baudrate.BAUD_38400;
                            break;
                        case (int)Baudrate.BAUD_19200:
                            config.Commands.C232B = Baudrate.BAUD_19200;
                            break;
                        case (int)Baudrate.BAUD_115200:
                            config.Commands.C232B = Baudrate.BAUD_115200;
                            break;
                        case (int)Baudrate.BAUD_230400:
                            config.Commands.C232B = Baudrate.BAUD_230400;
                            break;
                        case (int)Baudrate.BAUD_460800:
                            config.Commands.C232B = Baudrate.BAUD_460800;
                            break;
                        case (int)Baudrate.BAUD_921600:
                            config.Commands.C232B = Baudrate.BAUD_921600;
                            break;
                        default:
                            config.Commands.C232B = AdcpCommands.DEFAULT_C232B;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Decode C485B command.  
        /// It consist of 1 values.
        /// 
        /// If any of the steps fail, then the default will will remain in config.
        /// 
        /// Ex:
        /// C485B 115200
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeC485B(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpCommands.CMD_C485B))
            {
                int value = 0;
                if (int.TryParse(result[1], out value))
                {
                    switch (value)
                    {
                        case (int)Baudrate.BAUD_4800:
                            config.Commands.C485B = Baudrate.BAUD_4800;
                            break;
                        case (int)Baudrate.BAUD_2400:
                            config.Commands.C485B = Baudrate.BAUD_2400;
                            break;
                        case (int)Baudrate.BAUD_9600:
                            config.Commands.C485B = Baudrate.BAUD_9600;
                            break;
                        case (int)Baudrate.BAUD_38400:
                            config.Commands.C485B = Baudrate.BAUD_38400;
                            break;
                        case (int)Baudrate.BAUD_19200:
                            config.Commands.C485B = Baudrate.BAUD_19200;
                            break;
                        case (int)Baudrate.BAUD_115200:
                            config.Commands.C485B = Baudrate.BAUD_115200;
                            break;
                        case (int)Baudrate.BAUD_230400:
                            config.Commands.C485B = Baudrate.BAUD_230400;
                            break;
                        case (int)Baudrate.BAUD_460800:
                            config.Commands.C485B = Baudrate.BAUD_460800;
                            break;
                        case (int)Baudrate.BAUD_921600:
                            config.Commands.C485B = Baudrate.BAUD_921600;
                            break;
                        default:
                            config.Commands.C485B = AdcpCommands.DEFAULT_C485B;
                            break;
                    }
                }
            }
        }


        /// <summary>
        /// Decode C422B command.  
        /// It consist of 1 values.
        /// 
        /// If any of the steps fail, then the default will will remain in config.
        /// 
        /// Ex:
        /// C422B 115200
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeC422B(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpCommands.CMD_C422B))
            {
                int value = 0;
                if (int.TryParse(result[1], out value))
                {
                    switch (value)
                    {
                        case (int)Baudrate.BAUD_4800:
                            config.Commands.C422B = Baudrate.BAUD_4800;
                            break;
                        case (int)Baudrate.BAUD_2400:
                            config.Commands.C422B = Baudrate.BAUD_2400;
                            break;
                        case (int)Baudrate.BAUD_9600:
                            config.Commands.C422B = Baudrate.BAUD_9600;
                            break;
                        case (int)Baudrate.BAUD_38400:
                            config.Commands.C422B = Baudrate.BAUD_38400;
                            break;
                        case (int)Baudrate.BAUD_19200:
                            config.Commands.C422B = Baudrate.BAUD_19200;
                            break;
                        case (int)Baudrate.BAUD_115200:
                            config.Commands.C422B = Baudrate.BAUD_115200;
                            break;
                        case (int)Baudrate.BAUD_230400:
                            config.Commands.C422B = Baudrate.BAUD_230400;
                            break;
                        case (int)Baudrate.BAUD_460800:
                            config.Commands.C422B = Baudrate.BAUD_460800;
                            break;
                        case (int)Baudrate.BAUD_921600:
                            config.Commands.C422B = Baudrate.BAUD_921600;
                            break;
                        default:
                            config.Commands.C422B = AdcpCommands.DEFAULT_C422B;
                            break;
                    }
                }
            }
        }

        #region WP

        /// <summary>
        /// Find the AdcpSubsystemConfig in the dictionary of
        /// config with the same CEPO index. Then set the CWPON value.
        /// 
        /// If any of the steps fail, then the default will will remain in config.
        /// 
        /// Ex:
        /// CWPON[0] 1 [1] 0 [2] 1 [3] 1 [4] 0 [5] 1 [6] 1
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCWPON(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Check that the buffer contains 2 items and the first item is CWPON command
            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpSubsystemCommands.CMD_CWPON))
            {
                // Decode the values for each index
                Dictionary<int, int> values = DecodeIndexedIntValues(buffer);

                // Set the value to the SubsysteConfiguration
                foreach (AdcpSubsystemConfig ssConfig in config.SubsystemConfigDict.Values)
                {
                    // Check if the index exist in the dictionary of AdcpSubsystemConfig
                    if (values.ContainsKey(ssConfig.SubsystemConfig.CepoIndex))
                    {
                        // Convert the int to a bool
                        bool cwponValue = RTI.Commands.AdcpSubsystemCommands.DEFAULT_CWPON;
                        if (values[ssConfig.SubsystemConfig.CepoIndex] == 1)
                        {
                            cwponValue = true;
                        }
                        else
                        {
                            cwponValue = false;
                        }

                        // Set the value
                        ssConfig.Commands.CWPON = cwponValue;
                    }

                }
            }
        }

        /// <summary>
        /// Decode CWPBB command.  
        /// It consist of 2 values for each configuration.  A type and lag length.
        /// 
        /// If any of the steps fail, then the default will will remain in config.
        /// 
        /// Ex:
        /// CWPBB[0] 1,0.042 [1] 1,0.042 [2] 1,0.084 [3] 1,0.042 [4] 1,0.084 [5] 1,0.084 [6] 1,0.042
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCWPBB(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Check that the buffer contains 2 items and the first item is CWPBB command
            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpSubsystemCommands.CMD_CWPBB))
            {
                // Decode the values for each index
                Dictionary<int, string> values = DecodeIndexedMultiValues(buffer);

                // Set the value to the SubsysteConfiguration
                foreach (AdcpSubsystemConfig ssConfig in config.SubsystemConfigDict.Values)
                {
                    // Check if the index exist in the dictionary of AdcpSubsystemConfig
                    if (values.ContainsKey(ssConfig.SubsystemConfig.CepoIndex))
                    {
                        float[] cwpBBValues = DecodeGroupedFloatValues(values[ssConfig.SubsystemConfig.CepoIndex]);

                        // Ensure there are at least CWPBB_NUM_PARAM values
                        if (cwpBBValues.Length >= AdcpSubsystemCommands.CWPBB_NUM_PARAM)
                        {
                            // Set Lag Length
                            ssConfig.Commands.CWPBB_LagLength = cwpBBValues[1];
                            
                            // Set the Transmit Pulse Type
                            switch((int)cwpBBValues[0])
                            {
                                case (int)Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND:
                                    ssConfig.Commands.CWPBB_TransmitPulseType = Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND;
                                    break;
                                case (int)Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.NARROWBAND:
                                    ssConfig.Commands.CWPBB_TransmitPulseType = Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.NARROWBAND;
                                    break;
                                case (int)Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND_PULSE_TO_PULSE:
                                    ssConfig.Commands.CWPBB_TransmitPulseType = Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND_PULSE_TO_PULSE;
                                    break;
                                case (int)Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.NONCODED_PULSE_TO_PULSE:
                                    ssConfig.Commands.CWPBB_TransmitPulseType = Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.NONCODED_PULSE_TO_PULSE;
                                    break;
                                default:
                                    ssConfig.Commands.CWPBB_TransmitPulseType = Commands.AdcpSubsystemCommands.DEFAULT_CWPBB_TRANSMITPULSETYPE;
                                    break;
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Decode CWPAP command.
        /// It consist of 5 values for each configuration.  1 int and 4 floats.
        /// 
        /// Ex:
        /// CWPAP[0] 0,0.00,0.00,0.00,0.00 [1] 15,0.50,0.20,0.10,0.10 [2] 0,0.00,0.00,0.00,0.00 
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCWPAP(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Check that the buffer contains 2 items and the first item is CWPAP command
            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpSubsystemCommands.CMD_CWPAP))
            {
                // Decode the values for each index
                Dictionary<int, string> values = DecodeIndexedMultiValues(buffer);

                // Set the value to the SubsysteConfiguration
                foreach (AdcpSubsystemConfig ssConfig in config.SubsystemConfigDict.Values)
                {
                    // Check if the index exist in the dictionary of AdcpSubsystemConfig
                    if (values.ContainsKey(ssConfig.SubsystemConfig.CepoIndex))
                    {
                        float[] cwpapValues = DecodeGroupedFloatValues(values[ssConfig.SubsystemConfig.CepoIndex]);

                        // Ensure there are at least CWPAP_NUM_PARAM values
                        if (cwpapValues.Length >= AdcpSubsystemCommands.CWPAP_NUM_PARAM)
                        {
                            // Number of Pings
                            ssConfig.Commands.CWPAP_NumPingsAvg = (UInt16)cwpapValues[0];

                            // Lag
                            ssConfig.Commands.CWPAP_Lag = cwpapValues[1];

                            // Blank
                            ssConfig.Commands.CWPAP_Blank = cwpapValues[2];

                            // Bin Size
                            ssConfig.Commands.CWPAP_BinSize = cwpapValues[3];

                            // Time Between Pings
                            ssConfig.Commands.CWPAP_TimeBetweenPing = cwpapValues[4];
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Decode CWPST command.
        /// It consist of 3 values for each configuration.  3 floats.
        /// 
        /// Ex:
        /// CWPST[0] 0.400,1.000,1.000 [1] 0.400,1.000,1.000 [2] 0.400,1.000,1.000
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCWPST(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Check that the buffer contains 2 items and the first item is CWPST command
            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpSubsystemCommands.CMD_CWPST))
            {
                // Decode the values for each index
                Dictionary<int, string> values = DecodeIndexedMultiValues(buffer);

                // Set the value to the SubsysteConfiguration
                foreach (AdcpSubsystemConfig ssConfig in config.SubsystemConfigDict.Values)
                {
                    // Check if the index exist in the dictionary of AdcpSubsystemConfig
                    if (values.ContainsKey(ssConfig.SubsystemConfig.CepoIndex))
                    {
                        float[] cwpstValues = DecodeGroupedFloatValues(values[ssConfig.SubsystemConfig.CepoIndex]);

                        // Ensure there are at least CWPST_NUM_PARAM values
                        if (cwpstValues.Length >= AdcpSubsystemCommands.CWPST_NUM_PARAM)
                        {
                            // Correlation Threshold
                            ssConfig.Commands.CWPST_CorrelationThresh = cwpstValues[0];
                            // Q Velocity Threshold
                            ssConfig.Commands.CWPST_QVelocityThresh = cwpstValues[1];
                            // V Velocity Threshold
                            ssConfig.Commands.CWPST_VVelocityThresh = cwpstValues[2];
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Decode CWPBL command.
        /// It consist of 1 values for each configuration.  1 float.
        /// 
        /// Ex:
        /// CWPBL[0] 0.20 [1] 0.20 [2] 0.20 
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCWPBL(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Check that the buffer contains 2 items and the first item is CWPBL command
            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpSubsystemCommands.CMD_CWPBL))
            {
                // Decode the values for each index
                Dictionary<int, float> values = DecodeIndexedFloatValues(buffer);

                // Set the value to the SubsysteConfiguration
                foreach (AdcpSubsystemConfig ssConfig in config.SubsystemConfigDict.Values)
                {
                    // Check if the index exist in the dictionary of AdcpSubsystemConfig
                    if (values.ContainsKey(ssConfig.SubsystemConfig.CepoIndex))
                    {
                        // Blank
                        ssConfig.Commands.CWPBL = values[ssConfig.SubsystemConfig.CepoIndex];
                    }
                }
            }
        }

        /// <summary>
        /// Decode CWPBS command.
        /// It consist of 1 values for each configuration.  1 float.
        /// 
        /// Ex:
        /// CWPBS[0] 0.05 [1] 0.10 [2] 0.50 
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCWPBS(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Check that the buffer contains 2 items and the first item is CWPBS command
            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpSubsystemCommands.CMD_CWPBS))
            {
                // Decode the values for each index
                Dictionary<int, float> values = DecodeIndexedFloatValues(buffer);

                // Set the value to the SubsysteConfiguration
                foreach (AdcpSubsystemConfig ssConfig in config.SubsystemConfigDict.Values)
                {
                    // Check if the index exist in the dictionary of AdcpSubsystemConfig
                    if (values.ContainsKey(ssConfig.SubsystemConfig.CepoIndex))
                    {
                        // Bin Size
                        ssConfig.Commands.CWPBS = values[ssConfig.SubsystemConfig.CepoIndex];
                    }
                }
            }
        }

        /// <summary>
        /// Decode CWPX command.
        /// It consist of 1 values for each configuration.  1 float.
        /// 
        /// Ex:
        /// CWPX[0] 0.00 [1] 0.00 [2] 0.00
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCWPX(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Check that the buffer contains 2 items and the first item is CWPX command
            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpSubsystemCommands.CMD_CWPX))
            {
                // Decode the values for each index
                Dictionary<int, float> values = DecodeIndexedFloatValues(buffer);

                // Set the value to the SubsysteConfiguration
                foreach (AdcpSubsystemConfig ssConfig in config.SubsystemConfigDict.Values)
                {
                    // Check if the index exist in the dictionary of AdcpSubsystemConfig
                    if (values.ContainsKey(ssConfig.SubsystemConfig.CepoIndex))
                    {
                        // CWPX
                        ssConfig.Commands.CWPX = values[ssConfig.SubsystemConfig.CepoIndex];
                    }
                }
            }
        }

        /// <summary>
        /// Decode CWPBN command.
        /// It consist of 1 values for each configuration.  1 int.
        /// 
        /// Ex:
        /// CWPBN[0] 6 [1] 40 [2] 40 
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCWPBN(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Check that the buffer contains 2 items and the first item is CWPBN command
            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpSubsystemCommands.CMD_CWPBN))
            {
                // Decode the values for each index
                Dictionary<int, int> values = DecodeIndexedIntValues(buffer);

                // Set the value to the SubsysteConfiguration
                foreach (AdcpSubsystemConfig ssConfig in config.SubsystemConfigDict.Values)
                {
                    // Check if the index exist in the dictionary of AdcpSubsystemConfig
                    if (values.ContainsKey(ssConfig.SubsystemConfig.CepoIndex))
                    {
                        // CWPX
                        ssConfig.Commands.CWPBN = (ushort)values[ssConfig.SubsystemConfig.CepoIndex];
                    }
                }
            }
        }

        /// <summary>
        /// Decode CWPP command.
        /// It consist of 1 values for each configuration.  1 int.
        /// 
        /// Ex:
        /// CWPP[0] 1 [1] 1 [2] 10 
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCWPP(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Check that the buffer contains 2 items and the first item is CWPP command
            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpSubsystemCommands.CMD_CWPP))
            {
                // Decode the values for each index
                Dictionary<int, int> values = DecodeIndexedIntValues(buffer);

                // Set the value to the SubsysteConfiguration
                foreach (AdcpSubsystemConfig ssConfig in config.SubsystemConfigDict.Values)
                {
                    // Check if the index exist in the dictionary of AdcpSubsystemConfig
                    if (values.ContainsKey(ssConfig.SubsystemConfig.CepoIndex))
                    {
                        // CWPP
                        ssConfig.Commands.CWPP = (ushort)values[ssConfig.SubsystemConfig.CepoIndex];
                    }
                }
            }
        }

        /// <summary>
        /// Decode CWPBP command.  
        /// It consist of 2 values for each configuration.  Num Pings to Average and Time between pings.
        /// 
        /// If any of the steps fail, then the default will will remain in config.
        /// 
        /// Ex:
        /// CWPBP[0] 1,0.04 [1] 0,0.00 [2] 0,0.00 
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCWPBP(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Check that the buffer contains 2 items and the first item is CWPBP command
            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpSubsystemCommands.CMD_CWPBP))
            {
                // Decode the values for each index
                Dictionary<int, string> values = DecodeIndexedMultiValues(buffer);

                // Set the value to the SubsysteConfiguration
                foreach (AdcpSubsystemConfig ssConfig in config.SubsystemConfigDict.Values)
                {
                    // Check if the index exist in the dictionary of AdcpSubsystemConfig
                    if (values.ContainsKey(ssConfig.SubsystemConfig.CepoIndex))
                    {
                        float[] cwpBBValues = DecodeGroupedFloatValues(values[ssConfig.SubsystemConfig.CepoIndex]);

                        // Ensure there are at least CWPBP_NUM_PARAM values
                        if (cwpBBValues.Length >= AdcpSubsystemCommands.CWPBP_NUM_PARAM)
                        {
                            // Set Num Pings Avg
                            ssConfig.Commands.CWPBP_NumPingsAvg = (UInt16)cwpBBValues[0];

                            // Time Between Pings
                            ssConfig.Commands.CWPBP_TimeBetweenBasePings = cwpBBValues[1];

                        }
                    }

                }
            }
        }

        /// <summary>
        /// Decode CWPTBP command.
        /// It consist of 1 values for each configuration.  1 float.
        /// 
        /// Ex:
        /// CWPTBP[0] 0.25 [1] 0.25 [2] 0.25 
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCWPTBP(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Check that the buffer contains 2 items and the first item is CWPTBP command
            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpSubsystemCommands.CMD_CWPTBP))
            {
                // Decode the values for each index
                Dictionary<int, float> values = DecodeIndexedFloatValues(buffer);

                // Set the value to the SubsysteConfiguration
                foreach (AdcpSubsystemConfig ssConfig in config.SubsystemConfigDict.Values)
                {
                    // Check if the index exist in the dictionary of AdcpSubsystemConfig
                    if (values.ContainsKey(ssConfig.SubsystemConfig.CepoIndex))
                    {
                        // Blank
                        ssConfig.Commands.CWPTBP = values[ssConfig.SubsystemConfig.CepoIndex];
                    }
                }
            }
        }

        /// <summary>
        /// Decode the CEI line.  This is the Ensemble interval.
        /// 
        /// Ex:
        /// CWPAI 00:00:00.25
        /// </summary>
        /// <param name="buffer">Line for the command.</param>
        /// <param name="config">AdcpConfiguration to set the value.</param>
        private void DecodeCWPAI(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpSubsystemCommands.CMD_CWPAI))
            {
                // Decode the values for each index
                Dictionary<int, string> values = DecodeIndexedMultiValues(buffer);

                // Set the value to the SubsysteConfiguration
                foreach (AdcpSubsystemConfig ssConfig in config.SubsystemConfigDict.Values)
                {
                    // Check if the index exist in the dictionary of AdcpSubsystemConfig
                    if (values.ContainsKey(ssConfig.SubsystemConfig.CepoIndex))
                    {
                        // Decode the TimeValue and set it to CWPAI
                        TimeValue time = DecodeTimeValue(values[ssConfig.SubsystemConfig.CepoIndex]);
                        ssConfig.Commands.CWPAI = time;
                    }
                }
            }
        }

        #endregion

        #region Burst Interval

        /// <summary>
        /// Decode CBI command.  
        /// It consist of 2 values for each configuration.  Interval of Burst and Number of Ensembles per Burst.
        /// 
        /// If any of the steps fail, then the default will will remain in config.
        /// 
        /// Ex:
        /// CBI[0] 00:00:00.25,100 [1] 00:00:00.00,0 [2] 00:00:00.00,0 
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCBI(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Check that the buffer contains 2 items and the first item is CWPBP command
            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpSubsystemCommands.CMD_CBI))
            {
                // Decode the values for each index
                Dictionary<int, string> values = DecodeIndexedMultiValues(buffer);

                // Set the value to the SubsysteConfiguration
                foreach (AdcpSubsystemConfig ssConfig in config.SubsystemConfigDict.Values)
                {
                    // Check if the index exist in the dictionary of AdcpSubsystemConfig
                    if (values.ContainsKey(ssConfig.SubsystemConfig.CepoIndex))
                    {
                        string[] cbiValues = (values[ssConfig.SubsystemConfig.CepoIndex]).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        // Ensure there are at least CWPBP_NUM_PARAM values
                        if (cbiValues.Length >= AdcpSubsystemCommands.CBI_NUM_PARAM)
                        {
                            // Decode the TimeValue and set it to CBI_BurstInterval
                            TimeValue time = DecodeTimeValue(cbiValues[0]);
                            ssConfig.Commands.CBI_BurstInterval = time;

                            // Try to convert the number of ensembles and set it to CBI_NumEnsembles
                            UInt16 cbi_NumEnsembles = 0;
                            if (UInt16.TryParse(cbiValues[1], out cbi_NumEnsembles))
                            {
                                ssConfig.Commands.CBI_NumEnsembles = cbi_NumEnsembles;
                            }

                        }
                    }

                }
            }
        }

        #endregion

        #region BT

        /// <summary>
        /// Decode CBTON command.
        /// It consist of 1 values for each configuration.  1 int that will be converted to a boolean.
        /// 
        /// Ex:
        /// CBTON[0] 0 [1] 0 [2] 0 
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCBTON(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Check that the buffer contains 2 items and the first item is CBTON command
            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpSubsystemCommands.CMD_CBTON))
            {
                // Decode the values for each index
                Dictionary<int, int> values = DecodeIndexedIntValues(buffer);

                // Set the value to the SubsysteConfiguration
                foreach (AdcpSubsystemConfig ssConfig in config.SubsystemConfigDict.Values)
                {
                    // Check if the index exist in the dictionary of AdcpSubsystemConfig
                    if (values.ContainsKey(ssConfig.SubsystemConfig.CepoIndex))
                    {
                        // Convert the int to a bool
                        bool cbtonValue = RTI.Commands.AdcpSubsystemCommands.DEFAULT_CBTON;
                        if (values[ssConfig.SubsystemConfig.CepoIndex] == 1)
                        {
                            cbtonValue = true;
                        }
                        else
                        {
                            cbtonValue = false;
                        }

                        // Set the value
                        ssConfig.Commands.CBTON = cbtonValue;
                    }

                }
            }
        }

        /// <summary>
        /// Decode CBTBB command.  
        /// It consist of 3 values for each configuration.  1 int and 2 floats.
        /// 
        /// If any of the steps fail, then the default will will remain in config.
        /// 
        /// Ex:
        /// CBTBB[0] 0, 0.000, 50.00 [1] 0, 0.000, 0.00 [2] 0, 0.000, 0.00 
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCBTBB(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Check that the buffer contains 2 items and the first item is CWPBB command
            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpSubsystemCommands.CMD_CBTBB))
            {
                // Decode the values for each index
                Dictionary<int, string> values = DecodeIndexedMultiValues(buffer);

                // Set the value to the SubsysteConfiguration
                foreach (AdcpSubsystemConfig ssConfig in config.SubsystemConfigDict.Values)
                {
                    // Check if the index exist in the dictionary of AdcpSubsystemConfig
                    if (values.ContainsKey(ssConfig.SubsystemConfig.CepoIndex))
                    {
                        float[] cbtbbValues = DecodeGroupedFloatValues(values[ssConfig.SubsystemConfig.CepoIndex]);

                        // Ensure there are at least CWPBB_NUM_PARAM values
                        if (cbtbbValues.Length >= AdcpSubsystemCommands.CBTBB_NUM_PARAM)
                        {
                            // Set the Transmit Pulse Type
                            switch ((int)cbtbbValues[0])
                            {
                                case (int)Commands.AdcpSubsystemCommands.eCBTBB_Mode.NARROWBAND_LONG_RANGE:                                                         // 0
                                    ssConfig.Commands.CBTBB_Mode = Commands.AdcpSubsystemCommands.eCBTBB_Mode.NARROWBAND_LONG_RANGE;
                                    break;
                                case (int)Commands.AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_CODED:                                                               // 1
                                    ssConfig.Commands.CBTBB_Mode = Commands.AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_CODED;
                                    break;
                                case (int)Commands.AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_NON_CODED:                                                           // 2
                                    ssConfig.Commands.CBTBB_Mode = Commands.AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_NON_CODED;
                                    break;
                                case (int)Commands.AdcpSubsystemCommands.eCBTBB_Mode.NA_3:                                                                          // 3
                                    ssConfig.Commands.CBTBB_Mode = Commands.AdcpSubsystemCommands.eCBTBB_Mode.NA_3;
                                    break;
                                case (int)Commands.AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_NON_CODED_P2P:                                                       // 4
                                    ssConfig.Commands.CBTBB_Mode = Commands.AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_NON_CODED_P2P;
                                    break;
                                case (int)Commands.AdcpSubsystemCommands.eCBTBB_Mode.NA_5:                                                                          // 5
                                    ssConfig.Commands.CBTBB_Mode = Commands.AdcpSubsystemCommands.eCBTBB_Mode.NA_5;
                                    break;
                                case (int)Commands.AdcpSubsystemCommands.eCBTBB_Mode.NA_6:                                                                          // 6
                                    ssConfig.Commands.CBTBB_Mode = Commands.AdcpSubsystemCommands.eCBTBB_Mode.NA_6;
                                    break;
                                case (int)Commands.AdcpSubsystemCommands.eCBTBB_Mode.AUTO_SWITCH_NARROWBAND_BB_NONCODED_BB_NONCODED_P2P:                            // 7
                                    ssConfig.Commands.CBTBB_Mode = Commands.AdcpSubsystemCommands.eCBTBB_Mode.AUTO_SWITCH_NARROWBAND_BB_NONCODED_BB_NONCODED_P2P;
                                    break;
                                default:
                                    ssConfig.Commands.CBTBB_Mode = Commands.AdcpSubsystemCommands.eCBTBB_Mode.NARROWBAND_LONG_RANGE;
                                    break;
                            }

                            // Set Pulse to Pulse Lag
                            ssConfig.Commands.CBTBB_PulseToPulseLag = cbtbbValues[1];

                            // Set Long Range Depth
                            ssConfig.Commands.CBTBB_LongRangeDepth = cbtbbValues[2];
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Decode CBTST command.
        /// It consist of 3 values for each configuration.  3 floats.
        /// 
        /// Ex:
        /// CBTST[0] 0.900,1.000,1.000 [1] 0.000,0.000,0.000 [2] 0.000,0.000,0.000 
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCBTST(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Check that the buffer contains 2 items and the first item is CWPST command
            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpSubsystemCommands.CMD_CBTST))
            {
                // Decode the values for each index
                Dictionary<int, string> values = DecodeIndexedMultiValues(buffer);

                // Set the value to the SubsysteConfiguration
                foreach (AdcpSubsystemConfig ssConfig in config.SubsystemConfigDict.Values)
                {
                    // Check if the index exist in the dictionary of AdcpSubsystemConfig
                    if (values.ContainsKey(ssConfig.SubsystemConfig.CepoIndex))
                    {
                        float[] cbtstValues = DecodeGroupedFloatValues(values[ssConfig.SubsystemConfig.CepoIndex]);

                        // Ensure there are at least CWPST_NUM_PARAM values
                        if (cbtstValues.Length >= AdcpSubsystemCommands.CWPST_NUM_PARAM)
                        {
                            // Correlation Threshold
                            ssConfig.Commands.CBTST_CorrelationThresh = cbtstValues[0];
                            // Q Velocity Threshold
                            ssConfig.Commands.CBTST_QVelocityThresh = cbtstValues[1];
                            // V Velocity Threshold
                            ssConfig.Commands.CBTST_VVelocityThresh = cbtstValues[2];
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Decode CBTBL command.
        /// It consist of 1 values for each configuration.  1 float.
        /// 
        /// Ex:
        /// CBTBL[0] 0.20 [1] 0.20 [2] 0.20 
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCBTBL(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Check that the buffer contains 2 items and the first item is CBTBL command
            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpSubsystemCommands.CMD_CBTBL))
            {
                // Decode the values for each index
                Dictionary<int, float> values = DecodeIndexedFloatValues(buffer);

                // Set the value to the SubsysteConfiguration
                foreach (AdcpSubsystemConfig ssConfig in config.SubsystemConfigDict.Values)
                {
                    // Check if the index exist in the dictionary of AdcpSubsystemConfig
                    if (values.ContainsKey(ssConfig.SubsystemConfig.CepoIndex))
                    {
                        // Blank
                        ssConfig.Commands.CBTBL = values[ssConfig.SubsystemConfig.CepoIndex];
                    }
                }
            }
        }

        /// <summary>
        /// Decode CBTMX command.
        /// It consist of 1 values for each configuration.  1 float.
        /// 
        /// Ex:
        /// CBTMX[0] 0.20 [1] 0.20 [2] 0.20 
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCBTMX(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Check that the buffer contains 2 items and the first item is CBTMX command
            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpSubsystemCommands.CMD_CBTMX))
            {
                // Decode the values for each index
                Dictionary<int, float> values = DecodeIndexedFloatValues(buffer);

                // Set the value to the SubsysteConfiguration
                foreach (AdcpSubsystemConfig ssConfig in config.SubsystemConfigDict.Values)
                {
                    // Check if the index exist in the dictionary of AdcpSubsystemConfig
                    if (values.ContainsKey(ssConfig.SubsystemConfig.CepoIndex))
                    {
                        // Blank
                        ssConfig.Commands.CBTMX = values[ssConfig.SubsystemConfig.CepoIndex];
                    }
                }
            }
        }

        /// <summary>
        /// Decode CBTTBP command.
        /// It consist of 1 values for each configuration.  1 float.
        /// 
        /// Ex:
        /// CBTTBP[0] 0.20 [1] 0.20 [2] 0.20 
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCBTTBP(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Check that the buffer contains 2 items and the first item is CBTTBP command
            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpSubsystemCommands.CMD_CBTTBP))
            {
                // Decode the values for each index
                Dictionary<int, float> values = DecodeIndexedFloatValues(buffer);

                // Set the value to the SubsysteConfiguration
                foreach (AdcpSubsystemConfig ssConfig in config.SubsystemConfigDict.Values)
                {
                    // Check if the index exist in the dictionary of AdcpSubsystemConfig
                    if (values.ContainsKey(ssConfig.SubsystemConfig.CepoIndex))
                    {
                        // Blank
                        ssConfig.Commands.CBTTBP = values[ssConfig.SubsystemConfig.CepoIndex];
                    }
                }
            }
        }

        /// <summary>
        /// Decode CBTT command.
        /// It consist of 4 values for each configuration.  4 floats.
        /// 
        /// Ex:
        /// CBTT[0] 15.0,50.0,5.0,4.0 [1] 0.0,0.0,0.0,0.0 [2] 0.0,0.0,0.0,0.0 
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCBTT(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Check that the buffer contains 2 items and the first item is CBTT command
            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpSubsystemCommands.CMD_CBTT))
            {
                // Decode the values for each index
                Dictionary<int, string> values = DecodeIndexedMultiValues(buffer);

                // Set the value to the SubsysteConfiguration
                foreach (AdcpSubsystemConfig ssConfig in config.SubsystemConfigDict.Values)
                {
                    // Check if the index exist in the dictionary of AdcpSubsystemConfig
                    if (values.ContainsKey(ssConfig.SubsystemConfig.CepoIndex))
                    {
                        float[] cbttValues = DecodeGroupedFloatValues(values[ssConfig.SubsystemConfig.CepoIndex]);

                        // Ensure there are at least CWPBB_NUM_PARAM values
                        if (cbttValues.Length >= AdcpSubsystemCommands.CBTT_NUM_PARAM)
                        {
                            // SNR Shallow
                            ssConfig.Commands.CBTT_SNRShallowDetectionThresh = cbttValues[0];

                            // Depth switch SNR
                            ssConfig.Commands.CBTT_DepthSNR = cbttValues[1];

                            // SNR Deep
                            ssConfig.Commands.CBTT_SNRDeepDetectionThresh = cbttValues[2];

                            // Depth Gain switch
                            ssConfig.Commands.CBTT_DepthGain = cbttValues[3];
                        }
                    }
                }
            }
        }

        #endregion

        #region WT
 
        /// <summary>
        /// Decode CWTON command.
        /// It consist of 1 values for each configuration.  1 int that will be converted to a boolean.
        /// 
        /// Ex:
        /// CWTON[0] 0 [1] 0 [2] 0 
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCWTON(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Check that the buffer contains 2 items and the first item is CWTON command
            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpSubsystemCommands.CMD_CWTON))
            {
                // Decode the values for each index
                Dictionary<int, int> values = DecodeIndexedIntValues(buffer);

                // Set the value to the SubsysteConfiguration
                foreach (AdcpSubsystemConfig ssConfig in config.SubsystemConfigDict.Values)
                {
                    // Check if the index exist in the dictionary of AdcpSubsystemConfig
                    if (values.ContainsKey(ssConfig.SubsystemConfig.CepoIndex))
                    {
                        // Convert the int to a bool
                        bool cwtonValue = RTI.Commands.AdcpSubsystemCommands.DEFAULT_CWTON;
                        if (values[ssConfig.SubsystemConfig.CepoIndex] == 1)
                        {
                            cwtonValue = true;
                        }
                        else
                        {
                            cwtonValue = false;
                        }

                        // Set the value
                        ssConfig.Commands.CWTON = cwtonValue;
                    }

                }
            }
        }

        /// <summary>
        /// Decode CWTBB command.
        /// It consist of 1 values for each configuration.  1 int that will be converted to a boolean.
        /// 
        /// Ex:
        /// CWTBB[0] 0 [1] 0 [2] 0 
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCWTBB(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Check that the buffer contains 2 items and the first item is CWTBB command
            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpSubsystemCommands.CMD_CWTBB))
            {
                // Decode the values for each index
                Dictionary<int, int> values = DecodeIndexedIntValues(buffer);

                // Set the value to the SubsysteConfiguration
                foreach (AdcpSubsystemConfig ssConfig in config.SubsystemConfigDict.Values)
                {
                    // Check if the index exist in the dictionary of AdcpSubsystemConfig
                    if (values.ContainsKey(ssConfig.SubsystemConfig.CepoIndex))
                    {
                        // Convert the int to a bool
                        bool cwtbbValue = RTI.Commands.AdcpSubsystemCommands.DEFAULT_CWTBB;
                        if (values[ssConfig.SubsystemConfig.CepoIndex] == 1)
                        {
                            cwtbbValue = true;
                        }
                        else
                        {
                            cwtbbValue = false;
                        }

                        // Set the value
                        ssConfig.Commands.CWTBB = cwtbbValue;
                    }

                }
            }
        }

        /// <summary>
        /// Decode CWTBL command.
        /// It consist of 1 values for each configuration.  1 float.
        /// 
        /// Ex:
        /// CWTBL[0] 2.00 [1] 0.00 [2] 0.00 
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCWTBL(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Check that the buffer contains 2 items and the first item is CWTBL command
            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpSubsystemCommands.CMD_CWTBL))
            {
                // Decode the values for each index
                Dictionary<int, float> values = DecodeIndexedFloatValues(buffer);

                // Set the value to the SubsysteConfiguration
                foreach (AdcpSubsystemConfig ssConfig in config.SubsystemConfigDict.Values)
                {
                    // Check if the index exist in the dictionary of AdcpSubsystemConfig
                    if (values.ContainsKey(ssConfig.SubsystemConfig.CepoIndex))
                    {
                        // Blank
                        ssConfig.Commands.CWTBL = values[ssConfig.SubsystemConfig.CepoIndex];
                    }
                }
            }
        }

        /// <summary>
        /// Decode CWTBS command.
        /// It consist of 1 values for each configuration.  1 float.
        /// 
        /// Ex:
        /// CWTBS[0] 2.00 [1] 0.00 [2] 0.00 
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCWTBS(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Check that the buffer contains 2 items and the first item is CWTBS command
            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpSubsystemCommands.CMD_CWTBS))
            {
                // Decode the values for each index
                Dictionary<int, float> values = DecodeIndexedFloatValues(buffer);

                // Set the value to the SubsysteConfiguration
                foreach (AdcpSubsystemConfig ssConfig in config.SubsystemConfigDict.Values)
                {
                    // Check if the index exist in the dictionary of AdcpSubsystemConfig
                    if (values.ContainsKey(ssConfig.SubsystemConfig.CepoIndex))
                    {
                        // Blank
                        ssConfig.Commands.CWTBS = values[ssConfig.SubsystemConfig.CepoIndex];
                    }
                }
            }
        }

        /// <summary>
        /// Decode CWTTBP command.
        /// It consist of 1 values for each configuration.  1 float.
        /// 
        /// Ex:
        /// CWTTBP[0] 0.25 [1] 0.00 [2] 0.00 
        /// </summary>
        /// <param name="buffer">Command Values.</param>
        /// <param name="config">Configuration to store the results.</param>
        private void DecodeCWTTBP(string buffer, ref AdcpConfiguration config)
        {
            string[] result = buffer.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Check that the buffer contains 2 items and the first item is CWTTBP command
            if (result.Length > 1 && result[0].Contains(RTI.Commands.AdcpSubsystemCommands.CMD_CWTTBP))
            {
                // Decode the values for each index
                Dictionary<int, float> values = DecodeIndexedFloatValues(buffer);

                // Set the value to the SubsysteConfiguration
                foreach (AdcpSubsystemConfig ssConfig in config.SubsystemConfigDict.Values)
                {
                    // Check if the index exist in the dictionary of AdcpSubsystemConfig
                    if (values.ContainsKey(ssConfig.SubsystemConfig.CepoIndex))
                    {
                        // Blank
                        ssConfig.Commands.CWTTBP = values[ssConfig.SubsystemConfig.CepoIndex];
                    }
                }
            }
        }

        #endregion

        #region Decode Indexed Values

        /// <summary>
        /// Separate all the indexed configurations and the values.  This will
        /// create a dictionary with the CEPO index as the key and the float
        /// value as the value.
        /// 
        /// Separate all the values by [, this will leave X] N.
        /// Separate the index and the by ], this will leave X and N.
        /// 
        /// ABCD[X] N [X] N [X] N
        /// ABCD is the command, it may or maynot have a space between the command and the first index bracket
        /// X are the Index within CEPO
        /// N are the values for the index as int.
        /// </summary>
        /// <param name="buffer">Line to convert into indexed values.</param>
        /// <returns>Dictionary with the index values.</returns>
        private Dictionary<int, int> DecodeIndexedIntValues(string buffer)
        {
            Dictionary<int, int> results = new Dictionary<int, int>();
            
            // Split it by [ to find all configurations
            string[] split = buffer.Split(new char[] { '[' }, StringSplitOptions.RemoveEmptyEntries);

            // Go through each configuration and decode the index and value
            foreach (string indexSplit in split)
            {
                // Split it by ] to seperate the index from the configuration values
                string[] indexValue = indexSplit.Split(new char[] { ']' }, StringSplitOptions.RemoveEmptyEntries);

                if (indexValue.Length >= 2)
                {
                    int cepoIndex = 0;
                    bool cepoIndexGood = int.TryParse(indexValue[0], out cepoIndex);

                    int intValue = 0;
                    bool valueGood = int.TryParse(indexValue[1], out intValue);

                    // If the Index and the value is good, add it to the list
                    if (cepoIndexGood && valueGood)
                    {
                        // Add the value to the results array
                        results.Add(cepoIndex, intValue);
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Separate all the indexed configurations and the values.  This will
        /// create a dictionary with the CEPO index as the key and the float
        /// value as the value.
        /// 
        /// Separate all the values by [, this will leave X] N.NN
        /// Separate the index and the by ], this will leave X and N.NN.
        /// 
        /// ABCD[X] N.NN [X] N.NN [X] N.NN
        /// ABCD is the command, it may or maynot have a space between the command and the first index bracket
        /// X are the Index within CEPO
        /// N are the values for the index as double.
        /// </summary>
        /// <param name="buffer">Line to convert into indexed values.</param>
        /// <returns>Dictionary with the index values.</returns>
        private Dictionary<int, float> DecodeIndexedFloatValues(string buffer)
        {
            Dictionary<int, float> results = new Dictionary<int, float>();

            // Split it by [ to find all configurations
            string[] split = buffer.Split(new char[] { '[' }, StringSplitOptions.RemoveEmptyEntries);

            // Go through each configuration and decode the index and value
            foreach (string indexSplit in split)
            {
                // Split it by ] to seperate the index from the configuration values
                string[] indexValue = indexSplit.Split(new char[] { ']' }, StringSplitOptions.RemoveEmptyEntries);

                if (indexValue.Length >= 2)
                {
                    int cepoIndex = 0;
                    bool cepoIndexGood = int.TryParse(indexValue[0], out cepoIndex);

                    float floatValue = 0;
                    bool valueGood = float.TryParse(indexValue[1], out floatValue);

                    // If the Index and the value is good, add it to the list
                    if (cepoIndexGood && valueGood)
                    {
                        // Add the value to the results array
                        results.Add(cepoIndex, floatValue);
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Separate all the indexed configurations and the values.  This will
        /// create a dictionary with the CEPO index as the key and the string of all the
        /// values as the value.
        /// 
        /// Separate all the values by [, this will leave X] N.NN,M.MM.
        /// Separate the index and the by ], this will leave X and N.NN,M.MM.
        /// 
        /// ABCD[X] N.NN,M.MM [X] N.NN,M.MM [X] N.NN,M.MM
        /// ABCD is the command, it may or maynot have a space between the command and the first index bracket
        /// X are the Index within CEPO
        /// N,M are the values for the index as multiple types of values.
        /// </summary>
        /// <param name="buffer">Line to convert into indexed values.</param>
        /// <returns>Dictionary with the index values.</returns>
        private Dictionary<int, string> DecodeIndexedMultiValues(string buffer)
        {
            Dictionary<int, string> results = new Dictionary<int, string>();

            // Split it by [ to find all configurations
            string[] split = buffer.Split(new char[] { '[' }, StringSplitOptions.RemoveEmptyEntries);

            // Go through each configuration and decode the index and values
            foreach (string indexSplit in split)
            {
                // Split it by ] to seperate the index from the configuration values
                string[] indexValue = indexSplit.Split(new char[] { ']' }, StringSplitOptions.RemoveEmptyEntries);

                if (indexValue.Length >= 2)
                {
                    int cepoIndex = 0;
                    bool cepoIndexGood = int.TryParse(indexValue[0], out cepoIndex);

                    // If the Index is good, add it to the list
                    if (cepoIndexGood)
                    {
                        // Add the value to the results array
                        results.Add(cepoIndex, indexValue[1]);
                    }
                }
            }

            return results;
        }

        #endregion

        #region Decode Grouped Values

        /// <summary>
        /// Decode the buffer of all the values separated by a comma.
        /// This will convert each value in to a float and store to an array.
        /// </summary>
        /// <param name="buffer">Buffer of the values to decode.</param>
        /// <returns>Array of all the values found.</returns>
        private float[] DecodeGroupedFloatValues(string buffer)
        {
            // Split the values by a comma
            string[] values = buffer.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // Convert the results to doubles and add the value to a list
            // Then return the list as an array
            List<float> results = new List<float>();
            foreach (string s in values)
            {
                float d = 0.0f;
                if (float.TryParse(s, out d))
                {
                    results.Add(d);
                }
            }

            return results.ToArray();
        }

        #endregion

        #region Decode TimeValue

        /// <summary>
        /// Decode the given string into a time value.
        /// If the value can not be decode, then a default TimeValue will be returned.
        /// Ex:
        /// 10:59:33.22
        /// </summary>
        /// <param name="buffer">String containing the time value.</param>
        /// <returns>TimeValue from the given string.</returns>
        private TimeValue DecodeTimeValue(string buffer)
        {
            // Split the values by the colon, then try to parse the values
            string[] values = buffer.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            if (values.Length >= 3)
            {
                bool hrGood = false;
                bool minGood = false;
                bool secGood = false;
                bool hunSecGood = false;

                ushort hr = 0;
                ushort min = 0;
                ushort sec = 0;
                ushort hunSec = 0;

                hrGood = ushort.TryParse(values[0], out hr);
                minGood = ushort.TryParse(values[1], out min);

                // Split the seconds into seconds and hundredth of second
                if (values[2].Contains("."))
                {
                    string[] secValues = values[2].Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                    if (secValues.Length >= 2)
                    {
                        secGood = ushort.TryParse(secValues[0], out sec);
                        hunSecGood = ushort.TryParse(secValues[1], out hunSec);
                    }
                }

                // If all the values are good, set the values to config
                if (hrGood && minGood && secGood && hunSecGood)
                {
                    return new Commands.TimeValue(hr, min, sec, hunSec);
                }
            }

            return new TimeValue();
        }

        #endregion
    }
}
