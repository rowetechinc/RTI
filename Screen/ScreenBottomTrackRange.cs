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
 * 02/14/2012      RC          2.03       Initial coding
 * 
 */

using RTI.DataSet;
using System;
namespace RTI
{
    namespace Screen
    {
        /// <summary>
        /// This class will screen the Bottom Track Range.
        /// It will first check for bad values.  If the values
        /// are not bad, it will screen using population standard deviation
        /// to look for anomalies in the data.
        /// </summary>
        public class ScreenBottomTrackRange
        {

            #region Variables

            /// <summary>
            /// Maximum Standard Deviation for Bottom Track Range.
            /// </summary>
            private const int MAX_BT_RANGE_STDEV = 20;

            #endregion

            /// <summary>
            /// Screen the data using population standard deviation.
            /// This will calculate the standard deviation for all the good
            /// range values.  If any anomalies in the data are found, the value
            /// will be set to a bad range.
            /// Ensemble passed as a reference, so the data is saved to the object.
            /// </summary>
            /// <param name="ensemble">Ensemble to screen.</param>
            /// <returns>True = Data screened.</returns>
            public static bool Screen(ref DataSet.Ensemble ensemble)
            {
                if (ensemble.IsBottomTrackAvail)
                {
                    // Set range values
                    double curRangeB0 = ensemble.BottomTrackData.Range[DataSet.Ensemble.BEAM_0_INDEX];
                    double curRangeB1 = ensemble.BottomTrackData.Range[DataSet.Ensemble.BEAM_1_INDEX];
                    double curRangeB2 = ensemble.BottomTrackData.Range[DataSet.Ensemble.BEAM_2_INDEX];
                    double curRangeB3 = ensemble.BottomTrackData.Range[DataSet.Ensemble.BEAM_3_INDEX];

                    double avg = 0;
                    double stdev = MathHelper.StandardDev(curRangeB0, curRangeB1, curRangeB2, curRangeB3, out avg);
                    if (stdev > MAX_BT_RANGE_STDEV)
                    {
                        // If the standard dev is to large, reduce it to prevent outliers
                        stdev /= 2;
                    }
                    double minRange = avg - stdev;
                    if (minRange < 0)
                    {
                        minRange = 0;
                    }

                    double maxRange = avg + stdev;

                    // Filter data
                    // If the value is not within 1 standard deviation
                    // If found, then set the range to BAD_RANGE
                    // If already set to BAD_RANGE, leave the value
                    if ((curRangeB0 != DataSet.Ensemble.BAD_RANGE) && (curRangeB0 > maxRange || curRangeB0 < minRange))
                    {
                        ensemble.BottomTrackData.Range[DataSet.Ensemble.BEAM_0_INDEX] = DataSet.Ensemble.BAD_RANGE;
                    }

                    if ((curRangeB1 != DataSet.Ensemble.BAD_RANGE) && (curRangeB1 > maxRange || curRangeB1 < minRange))
                    {
                        ensemble.BottomTrackData.Range[DataSet.Ensemble.BEAM_1_INDEX] = DataSet.Ensemble.BAD_RANGE;
                    }

                    if ((curRangeB2 != DataSet.Ensemble.BAD_RANGE) && (curRangeB2 > maxRange || curRangeB2 < minRange))
                    {
                        ensemble.BottomTrackData.Range[DataSet.Ensemble.BEAM_2_INDEX] = DataSet.Ensemble.BAD_RANGE;
                    }

                    if ((curRangeB3 != DataSet.Ensemble.BAD_RANGE) && (curRangeB3 > maxRange || curRangeB3 < minRange))
                    {
                        ensemble.BottomTrackData.Range[DataSet.Ensemble.BEAM_3_INDEX] = DataSet.Ensemble.BAD_RANGE;
                    }

                    return true;
                }

                return false;
            }

        }

    }

}