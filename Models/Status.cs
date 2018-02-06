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
 * 12/20/2012      RC          2.17       Added the new errors from ADCP User Guide Rev H.
 * 01/04/2013      RC          2.17       Added equal, == and != to the object.
 *                                         Fixed bug with ERR_RCVR_DATA code.  Was 0x400 should have been 0x4000.
 * 12/27/2013      RC          2.21.1     Give the hex value error code with the error string.
 *                                         Updated the error codes.
 * 07/24/2014      RC          2.23.0     Added a comma to the status errors.
 * 05/14/2015      RC          3.0.4      Added more status options.
 * 04/28/2016      RC          3.3.2      Changed 0x0001 error to BT_Long_Lag.
 * 
 */


namespace RTI
{
    /// <summary>
    /// Status of the Ensemble Data
    /// ---------------------------
    /// Final value is a logical OR of each status bit.
    /// 
    /// Good Status                                         = 0x0000
    /// 
    /// Water Track 3 Beam Solution (NMEA output only)      = 0x0001
    ///    Indicates the Water Track velocity output contains
    ///    a 3 beam solution
    ///    
    /// Bottom track long lag processing is in use          = 0x0001
    ///    
    /// Bottom Track 3 Beam Solution (NMEA output only)     = 0x0002
    ///    Indicates the Bottom Track velocity output contains
    ///    a 3 beam solution
    ///    
    /// Bottom Track Hold (not searching yet)               = 0x0004
    ///    Indicates Bottom Track did not detect the bottom but
    ///    is still using the last good detection as an estimate
    ///    of the bottom location
    ///    
    /// Bottom Track Searching                              = 0x0008
    ///    Indicates Bottom Track is actively changing the ping
    ///    settings to attempt bottom detection
    ///    
    /// Bottom track coast                                  = 0x0020
    ///    Indicates the bottom track output filter is in use but no 
    ///    new data is available for output.
    /// 
    /// Bottom Track Proof                                  = 0x0040
    ///    Indicates Bottom Track is waiting for the next
    ///    valid Bottom Track ping before allowing
    ///    velocity to be output.
    ///    
    /// Bottom track low gain                               = 0x0080
    ///    Indicates bottom track has reduced the 
    ///    receiver gain below the selected switch range.
    ///    
    /// Heading Sensor Error                                = 0x0100
    ///    Heading Sensor Error.
    ///    
    /// Pressure Sensor Error                                = 0x0200
    ///    Pressure Ensor Error.
    /// 
    /// Power Down Failure                                  = 0x0400
    ///    System power did not shut off between ensembles
    /// 
    /// Non Volatile Data Error                             = 0x0800
    ///    Non volatile memory storage checksum failed
    /// 
    /// Real Time Clock (RTC) Error                         = 0x1000
    ///    The RTC did not respond or the time data 
    ///    value contains illegal values i.e. month = 13
    ///    
    /// Temperature Sensor Error                            = 0x2000
    ///    The temperature sensor ADC did not respond 
    ///    or the temperature value was out of 
    ///    range (-30 to 70 C).
    ///    
    /// Receiver Data Error                                 = 0x4000
    ///    The receiver did not output the expected 
    ///    amount of data
    /// 
    /// Receiver Timeout                                    = 0x8000
    ///    The receiver hardware did not respond to 
    ///    the ping request
    /// 
    /// Low Voltage                                         = 0xFFFF
    ///    The voltage was to low to preform a ping.
    /// 
    /// </summary>
    public class Status
    {
        #region Variables

        #region Error Codes

        /// <summary>
        /// A good value for status.
        /// </summary>
        public const int GOOD = 0x0000;

        /// <summary>
        /// Bottom track long lag processing is in use.
        /// </summary>
        public const int BT_LONG_LAG = 0x0001;

        /// <summary>
        /// Bottom Track 3 Beam solution STATUS value.
        /// Indicates the bottom track velocity output contains
        /// a 3 beam solution.
        /// </summary>
        public const int BT_BT_3BEAM_SOLUTION = 0x0002;

        /// <summary>
        /// Bottom Track Hold STATUS value.
        /// Indicates bottom track did not detect the bottom
        /// but is still using the last good detection as an 
        /// estimate of the bottom location.
        /// </summary>
        public const int BT_HOLD = 0x0004;

        /// <summary>
        /// Bottom Track Searching STATUS value.
        /// Indicates bottom track is actively changing the
        /// ping settings to attempt bottom detection.
        /// </summary>
        public const int BT_SEARCHING = 0x0008;

        /// <summary>
        /// 0x0010 Bottom Track long range narrow band processing is being used. 
        /// Indicates bottom track is using the narrow band processing for long range bottom detection.
        /// </summary>
        public const int BT_LR = 0x0010;

        /// <summary>
        /// 0x0020 Bottom track coast.
        /// Indicates the bottom track output filter is in use but no new data is available for output.
        /// </summary>
        public const int BT_COAST = 0x0020;

        /// <summary>
        /// 0x0040 Bottom track proof.
        /// Indicates bottom track is waiting for the next valid bottom track ping before allowing velocity data to be output.
        /// </summary>
        public const int BT_PROOF = 0x0040;

        /// <summary>
        /// Over temperature STATUS value.
        /// </summary>
        public const int OVERTEMP = 0x0020;

        /// <summary>
        /// 0x0080 Bottom track low gain.
        /// Indicates bottom track has reduced the receiver gain below the selected switch range.
        /// </summary>
        public const int BT_LOWGAIN = 0x0080;

        /// <summary>
        /// Heading sensor error.
        /// </summary>
        public const int ERR_HEADING_SENSOR = 0x0100;

        /// <summary>
        /// Pressure sensor error.
        /// </summary>
        public const int ERR_PRESSURE_SENSOR = 0x0200;

        /// <summary>
        /// Power Down Failure error.
        /// System power did not shut off between ensembles.
        /// </summary>
        public const int ERR_POWER_DOWN_FAILURE = 0x0400;

        /// <summary>
        /// Non Volatile Data Error.
        /// Non volatile memory storage checksum failed.
        /// </summary>
        public const int ERR_NONVOLATILE_DATA = 0x0800;

        /// <summary>
        /// Real Time Clock error.
        /// The RTC did not respond on the time data value
        /// contained illegal values i.e. month = 13.
        /// </summary>
        public const int ERR_RTC = 0x1000;

        /// <summary>
        /// Temperature Sensor error.
        /// The temperature sensor ADC did not respond or the
        /// temperature value was out of range (-30 to 70 C).
        /// </summary>
        public const int ERR_TEMPERATURE = 0x2000;

        /// <summary>
        /// Reciever data error.
        /// The receiver did not output the expected amount
        /// of data.
        /// </summary>
        public const int ERR_RCVR_DATA = 0x4000;

        /// <summary>
        /// Receiver timeout.
        /// The receiver hardware did not respond to the ping request.
        /// </summary>
        public const int ERR_RCVR_TIMEOUT = 0x8000;

        /// <summary>
        /// Low Voltage.
        /// The voltage has fallen below the threshold.
        /// </summary>
        public const int ERR_LOW_VOLTAGE = 0xFFFF;

        #endregion

        #region Strings

        /// <summary>
        /// Good string.
        /// </summary>
        public static readonly string STR_GOOD = "Good";

        /// <summary>
        /// Water Track 3 Beam Solution string.
        /// </summary>
        public static readonly string STR_WT_3_BEAM_SOLUTION = "WT 3 Beam Solution";

        /// <summary>
        /// Bottom Track Long Lag string.
        /// </summary>
        public static readonly string STR_BT_LONG_LAG = "BT Long Lag";

        /// <summary>
        /// Bottom Track 3 Beam Solution string.
        /// </summary>
        public static readonly string STR_BT_3_BEAM_SOLUTION = "BT 3 Beam Solution";

        /// <summary>
        /// Bottom Track Hold string.
        /// </summary>
        public static readonly string STR_BT_HOLD = "Hold";

        /// <summary>
        /// Bottom Track Searching string.
        /// </summary>
        public static readonly string STR_BT_SEARCHING = "Searching";

        /// <summary>
        /// Bottom Track LR string.
        /// </summary>
        public static readonly string STR_BT_LR = "BT LR";

        /// <summary>
        /// Over Temperature string.
        /// </summary>
        public static readonly string STR_OVERTEMP = "Over Temperature";

        /// <summary>
        /// Bottom Track Proof string.
        /// </summary>
        public static readonly string STR_BT_PROOF = "Proof";

        /// <summary>
        /// Bottom Track Coast string.
        /// </summary>
        public static readonly string STR_BT_COAST = "Coast";

        /// <summary>
        /// Bottom Track Low Gain string.
        /// </summary>
        public static readonly string STR_BT_LOWGAIN = "BT Low Gain";

        /// <summary>
        /// Heading Sensor Error string.
        /// </summary>
        public static readonly string STR_HDG_SENSOR_ERR = "Heading Sensor Error";

        /// <summary>
        /// Pressure Sensor Error string.
        /// </summary>
        public static readonly string STR_PRESSURE_SENSOR_ERR = "Pressure Sensor Error";

        /// <summary>
        /// Power Down Error string.
        /// </summary>
        public static readonly string STR_PWR_DOWN_ERR = "Power Down Error";

        /// <summary>
        /// Non-Volatile Storage Error string.
        /// </summary>
        public static readonly string STR_NONVOLATILE_STORAGE_ERR = "Non-volatile Storage Error";

        /// <summary>
        /// Real Time Clock Error string.
        /// </summary>
        public static readonly string STR_RTC_ERR = "RTC Error";

        /// <summary>
        /// Temperature Error string.
        /// </summary>
        public static readonly string STR_TEMP_ERR = "Temperature Error";

        /// <summary>
        /// Receiver Data Error string.
        /// </summary>
        public static readonly string STR_RCVR_DATA_ERR = "Receiver Data Error";

        /// <summary>
        /// Receiver Timeout Error string.
        /// </summary>
        public static readonly string STR_RCVR_TIMEOUT_ERR = "Receiver Timeout";

        /// <summary>
        /// Low Voltage Error string.
        /// </summary>
        public static readonly string STR_LOW_VOLTAGE_ERR = "Low Voltage";

        #endregion

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
        /// Bottom Track Long Lag.
        /// </summary>
        /// <returns>TRUE = Bottom Track Long Lag.</returns>
        public bool IsBottomTrackLongLag()
        {
            return (Value & BT_LONG_LAG) > 0;
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
        /// Bottom Track LR.
        /// </summary>
        /// <returns>TRUE = Bottom Track LR.</returns>
        public bool IsBottomTrackLR()
        {
            return (Value & BT_LR) > 0;
        }

        /// <summary>
        /// Check whether status bit set for 
        /// Over temperature.
        /// </summary>
        /// <returns>TRUE = Over Temperature.</returns>
        public bool IsOverTemperature()
        {
            return (Value & OVERTEMP) > 0;
        }

        /// <summary>
        /// Check whether status bit set for 
        /// Bottom Track Proof.
        /// </summary>
        /// <returns>TRUE = Bottom Track Proof.</returns>
        public bool IsBottomTrackProof()
        {
            return (Value & BT_PROOF) > 0;
        }

        /// <summary>
        /// Check whether status bit set for 
        /// Bottom Track Coast.
        /// </summary>
        /// <returns>TRUE = Bottom Track Coast.</returns>
        public bool IsBottomTrackCoast()
        {
            return (Value & BT_COAST) > 0;
        }

        /// <summary>
        /// Check whether status bit set for 
        /// Bottom Track Low Gain.
        /// </summary>
        /// <returns>TRUE = Bottom Track Low Gain.</returns>
        public bool IsBottomTrackLowGain()
        {
            return (Value & BT_LOWGAIN) > 0;
        }

        /// <summary>
        /// Check whether status bit set for 
        /// Heading Sensor Error.
        /// </summary>
        /// <returns>TRUE = Heading Sensor Error.</returns>
        public bool IsHeadingSensorError()
        {
            return (Value & ERR_HEADING_SENSOR) > 0;
        }

        /// <summary>
        /// Check whether status bit set for 
        /// Pressure Sensor Error.
        /// </summary>
        /// <returns>TRUE = Pressure Sensor Error.</returns>
        public bool IsPressureSensorError()
        {
            return (Value & ERR_PRESSURE_SENSOR) > 0;
        }

        /// <summary>
        /// Check whether status bit set for 
        /// Power Dowm Failure.
        /// </summary>
        /// <returns>TRUE = Power Down Failure.</returns>
        public bool IsPowerDownFailure()
        {
            return (Value & ERR_POWER_DOWN_FAILURE) > 0;
        }

        /// <summary>
        /// Check whether status bit set for
        /// Non Volatile Data Error.
        /// </summary>
        /// <returns>TRUE = Non Volatile Data Error.</returns>
        public bool IsNonVolatileDataError()
        {
            return (Value & ERR_NONVOLATILE_DATA) > 0;
        }

        /// <summary>
        /// Check whether status bit set for
        /// Real Time Clock Error.
        /// </summary>
        /// <returns>TRUE = Real time clock error.</returns>
        public bool IsRealTimeClockError()
        {
            return (Value & ERR_RTC) > 0;
        }

        /// <summary>
        /// Check whether status bit set for
        /// Temperature Error.
        /// </summary>
        /// <returns>TRUE = Temperature Error.</returns>
        public bool IsTemperatureError()
        {
            return (Value & ERR_TEMPERATURE) > 0;
        }

        /// <summary>
        /// Check whether status bit set for
        /// Receiver Data Error.
        /// </summary>
        /// <returns>TRUE = Receiver Data Error.</returns>
        public bool IsReceiverDataError()
        {
            return (Value & ERR_RCVR_DATA) > 0;
        }

        /// <summary>
        /// Check whether status bit set for
        /// Hardware Timeout.
        /// </summary>
        /// <returns>TRUE = Hardware Timeout.</returns>
        public bool IsReceiverTimeout()
        {
            return (Value & ERR_RCVR_TIMEOUT) > 0;
        }

        /// <summary>
        /// Check whether status bit set for
        /// Low Voltage.
        /// </summary>
        /// <returns>TRUE = Low Voltage.</returns>
        public bool IsLowVoltage()
        {
            return Value == ERR_LOW_VOLTAGE;
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
            if (Value == GOOD)
            {
                return STR_GOOD;
            }
            // This error message is 0xFFFF and would include all the
            // other bits, so seperate this
            else if (IsLowVoltage())
            {
                string result = "0x" + Value.ToString("X4") + " ";
                return result + STR_LOW_VOLTAGE_ERR;
            }
            else
            {
                string result = "0x" + Value.ToString("X4") + ", ";

                // Check for all warnings.
                if (IsBottomTrackLongLag())
                {
                    result += STR_BT_LONG_LAG + ", ";
                }
                if (IsBottomTrack3BeamSolution())
                {
                    result += STR_BT_3_BEAM_SOLUTION + ", ";
                }
                if (IsBottomTrackHold())
                {
                    result += STR_BT_HOLD + ", ";
                }
                if (IsBottomTrackSearching())
                {
                    result += STR_BT_SEARCHING + ", ";
                }
                if (IsBottomTrackLR())
                {
                    result += STR_BT_LR + ", ";
                }
                if (IsOverTemperature())
                {
                    result += STR_OVERTEMP + ", ";
                }
                if (IsBottomTrackProof())
                {
                    result += STR_BT_PROOF + ", ";
                }
                if (IsBottomTrackCoast())
                {
                    result += STR_BT_COAST + ", ";
                }
                if (IsBottomTrackLowGain())
                {
                    result += STR_BT_LOWGAIN + ", ";
                }
                if (IsHeadingSensorError())
                {
                    result += STR_HDG_SENSOR_ERR + ", ";
                }
                if (IsPressureSensorError())
                {
                    result += STR_PRESSURE_SENSOR_ERR + ", ";
                }
                if (IsPowerDownFailure())
                {
                    result += STR_PWR_DOWN_ERR + ", ";
                }
                if (IsNonVolatileDataError())
                {
                    result += STR_NONVOLATILE_STORAGE_ERR + ", ";
                }
                if (IsRealTimeClockError())
                {
                    result += STR_RTC_ERR + ", ";
                }
                if (IsTemperatureError())
                {
                    result += STR_TEMP_ERR + ", ";
                }
                if (IsReceiverDataError())
                {
                    result += STR_RCVR_DATA_ERR + ", ";
                }
                if (IsReceiverTimeout())
                {
                    result += STR_RCVR_TIMEOUT_ERR + ", ";
                }

                // Remove the last comma and space
                result = result.Remove(result.Length - 2);

                //return Value.ToString();
                return result;
            }
        }

        /// <summary>
        /// Hashcode for the object.
        /// This will return the hashcode for the
        /// this object's string.
        /// </summary>
        /// <returns>Hashcode for the object.</returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Determine if the given object is equal to this
        /// object.  This will check if the Status Value match.
        /// </summary>
        /// <param name="obj">Object to compare with this object.</param>
        /// <returns>TRUE = Status Value matched.</returns>
        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            Status p = (Status)obj;

            return Value == p.Value;
        }

        /// <summary>
        /// Determine if the two Status Value given are the equal.
        /// </summary>
        /// <param name="stat1">First Status to check.</param>
        /// <param name="stat2">Status to check against.</param>
        /// <returns>True if there strings match.</returns>
        public static bool operator ==(Status stat1, Status stat2)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(stat1, stat2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)stat1 == null) || ((object)stat2 == null))
            {
                return false;
            }

            // Return true if the fields match:
            return stat1.Value == stat2.Value;
        }

        /// <summary>
        /// Return the opposite of ==.
        /// </summary>
        /// <param name="stat1">First Status to check.</param>
        /// <param name="stat2">Status to check against.</param>
        /// <returns>Return the opposite of ==.</returns>
        public static bool operator !=(Status stat1, Status stat2)
        {
            return !(stat1 == stat2);
        }

        #endregion
    }

}