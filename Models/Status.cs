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
 * 11/22/2011      RC                     Initial coding
 * 11/28/2011      RC                     Changed ToString() to display text.
 * 01/31/2012      RC                     Added new errors from User Guide Rev F.
 * 09/18/2012      RC          2.15       Updated errors from User Guide Rev H.
 * 
 */


namespace RTI
{
    /// <summary>
    /// Status of the Ensemble Data
    /// ---------------------------
    /// Final value is a logical OR of each status bit.
    /// 
    /// Good Status                             = 0x0000
    /// 
    /// Water Track 3 Beam Solution (DVL only)  = 0x0001
    ///    Indicates the Water Track velocity output contains
    ///    a 3 beam solution
    ///    
    /// Bottom Track 3 Beam Solution            = 0x0002
    ///    Indicates the Bottom Track velocity output contains
    ///    a 3 beam solution
    ///    
    /// Bottom Track Hold (not searching yet)   = 0x0004
    ///    Indicates Bottom Track did not detect the bottom but
    ///    is still using the last good detection as an estimate
    ///    of the bottom location
    ///    
    /// Bottom Track Searching                  = 0x0008
    ///    Indicates Bottom Track is actively changing the ping
    ///    settings to attempt bottom detection
    ///    
    /// Receiver Timeout                        = 0x8000
    ///    The receiver hardware did not respond to the ping request
    /// 
    /// Receiver Data Error                     = 0x4000
    ///    The receiver did not output the expected amount of data
    /// 
    /// Temperature Error                       = 0x2000
    ///    The temperature sensor ADC did not respond or the temperature
    ///    value was out of range (-30 to 70 C).
    /// 
    /// Real Time Clock (RTC) Error             = 0x1000
    ///    The RTC did not respond or the time data value contains
    ///    illegal values i.e. month = 13
    ///    
    /// Non Volatile Data Error                 = 0x0800
    ///    Non volatile memory storage checksum failed
    ///    
    /// Power Down Failure                      = 0x0400
    ///    System power did not shut off between ensembles
    ///    
    /// 
    /// </summary>
    public class Status
    {
        #region Variables

        /// <summary>
        /// Water Track 3 Beam solution STATUS value (DVL only).
        /// </summary>
        public const int BT_WT_3BEAM_SOLUTION = 0x0001;

        /// <summary>
        /// Bottom Track 3 Beam solution STATUS value.
        /// </summary>
        public const int BT_BT_3BEAM_SOLUTION = 0x0002;

        /// <summary>
        /// Bottom Track Hold STATUS value.
        /// </summary>
        public const int BT_HOLD = 0x0004;

        /// <summary>
        /// Bottom Track Searching STATUS value.
        /// </summary>
        public const int BT_SEARCHING = 0x0008;

        /// <summary>
        /// Hardware timeout STATUS value.
        /// </summary>
        public const int HDWR_TIMEOUT = 0x8000;

        /// <summary>
        /// Data error.
        /// </summary>
        public const int DATAERROR = 0x400;

        /// <summary>
        /// Temperature error.
        /// </summary>
        public const int TEMPERATURE_ERROR = 0x2000;

        /// <summary>
        /// Real Time Clock error.
        /// </summary>
        public const int RTC_ERROR = 0x1000;

        /// <summary>
        /// Non Volatile Data Error.
        /// </summary>
        public const int NONVOLATILE_DATA_ERROR = 0x0800;

        /// <summary>
        /// Power Down Failure error.
        /// </summary>
        public const int POWER_DOWN_ERROR = 0x0400;

        #endregion

        #region Properties

        /// <summary>
        /// Status value stored
        /// </summary>
        public int Value { get; set; }

        #endregion

        /// <summary>
        /// Initialize the status value
        /// </summary>
        /// <param name="status">Status value.</param>
        public Status(int status)
        {
            Value = status;
        }

        #region Methods

        /// <summary>
        /// Check whether status bit set for
        /// Water Track 3 beam solution.
        /// </summary>
        /// <returns>TRUE = Water Track 3 Beam Solution.</returns>
        public bool IsWaterTrack3BeamSolution()
        {
            return (Value & BT_WT_3BEAM_SOLUTION) > 0;
        }

        /// <summary>
        /// Check whether status bit set for
        /// Bottom Track 3 Beam solution.
        /// </summary>
        /// <returns>TRUE = Bottom Track 3 Beam Solution.</returns>
        public bool IsBottomTrack3BeamSolution()
        {
            return (Value & BT_BT_3BEAM_SOLUTION) > 0;
        }

        /// <summary>
        /// Check whether status bit set for
        /// Bottom Track Hold.
        /// </summary>
        /// <returns>TRUE = Bottom Track Hold.</returns>
        public bool IsBottomTrackHold()
        {
           return (Value & BT_HOLD) > 0;
        }

        /// <summary>
        /// Check whether status bit set for 
        /// Bottom Track Searching.
        /// </summary>
        /// <returns>TRUE = Bottom Track Searching.</returns>
        public bool IsBottomTrackSearching()
        {
            return (Value & BT_SEARCHING) > 0;
        }

        /// <summary>
        /// Check whether status bit set for
        /// Hardware Timeout.
        /// </summary>
        /// <returns>TRUE = Hardware Timeout.</returns>
        public bool IsReceiverTimeout()
        {
            return (Value & HDWR_TIMEOUT) > 0;
        }

        /// <summary>
        /// Check whether status bit set for
        /// Data Error.
        /// </summary>
        /// <returns>TRUE = Data Error.</returns>
        public bool IsReceiverDataError()
        {
            return (Value & DATAERROR) > 0;
        }

        /// <summary>
        /// Check whether status bit set for
        /// Temperature Error.
        /// </summary>
        /// <returns>TRUE = Temperature Error.</returns>
        public bool IsTemperatureError()
        {
            return (Value & TEMPERATURE_ERROR) > 0;
        }

        /// <summary>
        /// Check whether status bit set for
        /// Real Time Clock Error.
        /// </summary>
        /// <returns>TRUE = Real time clock error.</returns>
        public bool IsRealTimeClockError()
        {
            return (Value & RTC_ERROR) > 0;
        }

        /// <summary>
        /// Check whether status bit set for
        /// Non Volatile Data Error.
        /// </summary>
        /// <returns>TRUE = Non Volatile Data Error.</returns>
        public bool IsNonVolatileDataError()
        {
            return (Value & NONVOLATILE_DATA_ERROR) > 0;
        }

        /// <summary>
        /// Check whether status bit set for
        /// Power Down Failure error.
        /// </summary>
        /// <returns>TRUE = Power Down failure.</returns>
        public bool IsPowerDownError()
        {
            return (Value & POWER_DOWN_ERROR) > 0;
        }

        /// <summary>
        /// Return a string representing the
        /// status.  This could be multiple statuses
        /// values in the string.
        /// </summary>
        /// <returns>Status value as a string.</returns>
        public override string ToString()
        {
            // No errors, return good
            if (!IsWaterTrack3BeamSolution() && 
                !IsBottomTrack3BeamSolution() && 
                !IsBottomTrackHold() && 
                !IsBottomTrackSearching() && 
                !IsReceiverTimeout() && 
                !IsReceiverDataError() && 
                !IsTemperatureError() && 
                !IsRealTimeClockError() &&
                !IsNonVolatileDataError() &&
                !IsPowerDownError()
                )
            {
                return "Good";
            }

            string result = "";
            // Check for all warnings.
            if (IsWaterTrack3BeamSolution())
            {
                result += "WT 3 Beam Solution ";
            }
            if (IsBottomTrack3BeamSolution())
            {
                result += "BT 3 Beam Solution ";
            }
            if (IsBottomTrackHold())
            {
                result += "Hold ";
            }
            if (IsBottomTrackSearching())
            {
                result += "Searching ";
            }
            if (IsReceiverTimeout())
            {
                result += "Receiver Timeout ";
            }
            if (IsReceiverDataError())
            {
                result += "Receiver Data Error ";
            }
            if (IsTemperatureError())
            {
                result += "Temperature Error ";
            }
            if (IsRealTimeClockError())
            {
                result += "RTC Error ";
            }
            if (IsNonVolatileDataError())
            {
                result += "Non volatile Storage Error ";
            }
            if (IsPowerDownError())
            {
                result += "Power Down Error ";
            }

            //return Value.ToString();
            return result;
        }

        #endregion
    }

}