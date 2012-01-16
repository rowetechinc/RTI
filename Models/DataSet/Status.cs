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
 * Date            Initials    Comments
 * -----------------------------------------------------------------
 * 11/22/2011      RC          Initial coding
 * 11/28/2011      RC          Changed ToString() to display text.
 * 
 */


namespace RTI
{
    /// <summary>
    /// Status of the ensemble.
    /// Final value is a logical OR of each status bit.
    /// Good Status = 0x0000
    /// Hardware Timeout = 08000
    ///   The hardware did not respond to the ping request
    /// 
    /// Status of the Bottom Track.
    /// Final ranges logical OR of each bit below.
    /// Good Status = 0x0000
    /// Water Track 3 Beam Solution (DVL only)  = 0x0001
    ///    3 or 4 beams have a valid signal
    /// Bottom Track 3 Beam Solution            = 0x0002
    ///    3 or 4 beams located the bottom
    /// Bottom Track Hold (not searching yet)   = 0x0004
    ///    Holding the search to last known Depth
    /// Bottom Track Searching                  = 0x0008
    ///    Actively searching for the bottom
    /// Hardware Timeout                        = 0x8000
    ///    The hardware did not respond to the ping request
    /// </summary>
    public class Status
    {
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
        /// Status value stored
        /// </summary>
        public int Value { get; set; }
        
        /// <summary>
        /// Initialize the status value
        /// </summary>
        /// <param name="status">Status value.</param>
        public Status(int status)
        {
            Value = status;
        }

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
        public bool IsHardwareTimeout()
        {
            return (Value & HDWR_TIMEOUT) > 0;
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
            if (!IsWaterTrack3BeamSolution() && !IsBottomTrack3BeamSolution() && !IsBottomTrackHold() && !IsBottomTrackSearching() && !IsHardwareTimeout())
            {
                return "Good";
            }

            string result = "";
            // Check for all warnings.
            if (IsWaterTrack3BeamSolution())
            {
                result += "WT 3 Beam Solution";
            }
            if (IsBottomTrack3BeamSolution())
            {
                result += "BT 3 Beam Solution";
            }
            if (IsBottomTrackHold())
            {
                result += "Hold";
            }
            if (IsBottomTrackSearching())
            {
                result += "Searching";
            }
            if (IsHardwareTimeout())
            {
                result += "Hardware Timeout";
            }

            //return Value.ToString();
            return result;
        }
    }

}