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
 * 07/03/2012      RC          2.12       Initial coding
 * 07/10/2012      RC          2.12       Added error codes and a list to hold them all.
 * 07/17/2012      RC          2.12       Add Results to hold an object to pass with results.
 *                                         Added SystemTestErrorCodes::INCORRECT_RTC_TIME.
 * 
 */

using System.Collections.Generic;

namespace RTI
{
    #region Error Codes

    /// <summary>
    /// Error codes for System Test.
    /// </summary>
    public enum SystemTestErrorCodes
    {
        /// <summary>
        /// No connection to the ADCP.
        /// </summary>
        ADCP_NO_COMM,

        /// <summary>
        /// No connection to the ADCP on the RS-232 port.
        /// </summary>
        ADCP_NO_COMM_232,

        /// <summary>
        /// No connection to the ADCP on the RS-485 port.
        /// </summary>
        ADCP_NO_COMM_485,

        /// <summary>
        /// Firmware version not set.
        /// </summary>
        ADCP_FIRMWARE_VERSION,

        /// <summary>
        /// Incorrect Firmware major number.
        /// </summary>
        ADCP_FIRMWARE_MAJOR,

        /// <summary>
        /// Incorrect Firmware minor number.
        /// </summary>
        ADCP_FIRMWARE_MINOR,

        /// <summary>
        /// Incorrect Firmware revision.
        /// </summary>
        ADCP_FIRMWARE_REVISION,

        /// <summary>
        /// No firmware files found.
        /// </summary>
        FIRMWARE_FILES_MISSING,

        /// <summary>
        /// Firmware file Help.txt is missing.
        /// </summary>
        FIRMWARE_HELP_MISSING,

        /// <summary>
        /// Firmware file RTISYS.bin is missing.
        /// </summary>
        FIRMWARE_RTISYS_MISSING,

        /// <summary>
        /// Firmware file BOOT.bin is missing.
        /// </summary>
        FIRMWARE_BOOT_MISSING,

        /// <summary>
        /// Firmware file SYSCONF.bin is missing.
        /// </summary>
        FIRMWARE_SYSCONF_MISSING,

        /// <summary>
        /// Firmware file BBCODE.bin is missing.
        /// </summary>
        FIRMWARE_BBCODE_MISSING,

        /// <summary>
        /// The compass is not outputing data.
        /// Test when ENGPNI command.  Returned
        /// 0 for heading, pitch and roll.
        /// </summary>
        COMPASS_NO_OUTPUT,

        /// <summary>
        /// Incorrect serial number for the IO board.
        /// </summary>
        INCORRECT_SERIAL_IO_BRD,

        /// <summary>
        /// Incorrect serial number for the transmitter board.
        /// </summary>
        INCORRECT_SERIAL_XMITTER_BRD,

        /// <summary>
        /// Incorrect serial number for the Virtual ground board.
        /// </summary>
        INCORRECT_SERIAL_VIRTUAL_GND_BRD,

        /// <summary>
        /// Incorrect serial number for the Low Power Regulator board.
        /// </summary>
        INCORRECT_SERIAL_LOW_PWR_REG_BRD,

        /// <summary>
        /// Incorrect serial number for the Receiver board.
        /// </summary>
        INCORRECT_SERIAL_RCVR_BRD,

        /// <summary>
        /// Incorrect serial number for the Backplane board.
        /// </summary>
        INCORRECT_SERIAL_BACKPLANE_BRD,

        /// <summary>
        /// Incorrect revision for the IO board.
        /// </summary>
        INCORRECT_REV_IO_BRD,

        /// <summary>
        /// Incorrect revision for the transmitter board.
        /// </summary>
        INCORRECT_REV_XMITTER_BRD,

        /// <summary>
        /// Incorrect revision for the Virtual ground board.
        /// </summary>
        INCORRECT_REV_VIRTUAL_GND_BRD,

        /// <summary>
        /// Incorrect revision for the Low Power Regulator board.
        /// </summary>
        INCORRECT_REV_LOW_PWR_REG_BRD,

        /// <summary>
        /// Incorrect revision for the Receiver board.
        /// </summary>
        INCORRECT_REV_RCVR_BRD,

        /// <summary>
        /// Incorrect revision for the Backplane board.
        /// </summary>
        INCORRECT_REV_BACKPLANE_BRD,

        /// <summary>
        /// When the time was set, the RTC time did not match the actual time.
        /// </summary>
        INCORRECT_RTC_TIME,

        /// <summary>
        /// Error in the ensemble status.  This can be a hardware failure or a pinging error. 
        /// </summary>
        ENS_STATUS_ERROR,

        /// <summary>
        /// Error in the bottom track status.  This can be a hardware failure or a pinging error. 
        /// </summary>
        BT_STATUS_ERROR,
    }

    #endregion

    /// <summary>
    /// Object to hold the test result and list of errors.
    /// </summary>
    public class SystemTestResult
    {
        /// <summary>
        /// List of error messages.
        /// </summary>
        public List<string> ErrorListStrings { get; set; }

        /// <summary>
        /// List of all the error codes.
        /// </summary>
        public List<SystemTestErrorCodes> ErrorCodes { get; set; }

        /// <summary>
        /// Test Result.  True = Pass, False = Fail.
        /// </summary>
        public bool TestResult { get; set; }

        /// <summary>
        /// This object must be cast to the correct type.
        /// This object will hold any results needed by the 
        /// user after the test is complete.  This object is
        /// generic to hold any type of result.  The result could
        /// be a struct of results or a single result.
        /// 
        /// To cast the object do 
        /// if(Results is MyObject) { MyObject obj = (MyObject)Results; }
        /// </summary>
        public object Results { get; set; }

        /// <summary>
        /// Initialize the values.
        /// Default test passes.
        /// </summary>
        public SystemTestResult()
        {
            TestResult = true;
            ErrorListStrings = new List<string>();
            ErrorCodes = new List<SystemTestErrorCodes>();
            Results = null;
        }
    }
}
