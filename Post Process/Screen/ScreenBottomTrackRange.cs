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
 * 01/16/2013      RC          2.17       Fixed bug in how the screening for bad ranges was done.
 * 04/26/2013      RC          2.19       Changed namespace from Screen to ScreenData.
 * 
 */

using RTI.DataSet;
using System;
namespace RTI
{
    namespace ScreenData
    {
        /// <summary>
        /// This class will screen the Bottom Track Range.
        /// It will first check for bad values.  If the values
        /// are not bad, it will screen using population standard deviation
        /// to look for anomalies in the data.
        /// </summary>
        public class ScreenBottomTrackRange
        {

            #region Result

            /// <summary>
            /// This will include all the results that
            /// were used to come to a conclusion whether
            /// the data should be screened out.
            /// </summary>
            public class ScreenBottomTrackRangeResult
            {
                /// <summary>
                /// Standard Deviation calculated for the range.
                /// </summary>
                public double StdDev { get; set; }

                /// <summary>
                /// Average Calculated for the range.
                /// </summary>
                public double Avg { get; set; }

                /// <summary>
                /// Minimum range allowed based off standard deviation and average calculation.
                /// minRange = avg - stdev;
                /// </summary>
                public double MinRange { get; set; }

                /// <summary>
                /// Minimum range allowed based off standard deviation and average calculation.
                /// maxRange = avg + stdev;
                /// </summary>
                public double MaxRange { get; set; }

                /// <summary>
                /// Flag if the Beam 0 was set bad based off screening parameters.
                /// </summary>
                public bool Beam0Bad { get; set; }

                /// <summary>
                /// Flag if the Beam 1 was set bad based off screening parameters.
                /// </summary>
                public bool Beam1Bad { get; set; }

                /// <summary>
                /// Flag if the Beam 2 was set bad based off screening parameters.
                /// </summary>
                public bool Beam2Bad { get; set; }

                /// <summary>
                /// Flag if the Beam 3 was set bad based off screening parameters.
                /// </summary>
                public bool Beam3Bad { get; set; }

                /// <summary>
                /// Initial value for the Beam 0 range.
                /// </summary>
                public double Beam0Range { get; set; }

                /// <summary>
                /// Initial value for the Beam 1 range.
                /// </summary>
                public double Beam1Range { get; set; }

                /// <summary>
                /// Initial value for the Beam 2 range.
                /// </summary>
                public double Beam2Range { get; set; }

                /// <summary>
                /// Initial value for the Beam 3 range.
                /// </summary>
                public double Beam3Range { get; set; }

                /// <summary>
                /// Create object with values initialized.
                /// </summary>
                public ScreenBottomTrackRangeResult()
                {
                    StdDev = 0.0;
                    Avg = 0.0;
                    MinRange = 0.0;
                    MaxRange = 0.0;
                    Beam0Bad = false;
                    Beam1Bad = false;
                    Beam2Bad = false;
                    Beam3Bad = false;
                    Beam0Range = 0.0;
                    Beam1Range = 0.0;
                    Beam2Range = 0.0;
                    Beam3Range = 0.0;
                }
            }

            #endregion

            #region Variables

            /// <summary>
            /// Maximum Standard Deviation for Bottom Track Range.
            /// </summary>
            private const int MAX_BT_RANGE_STDEV = 20;

            /// <summary>
            /// Default multiplier value.
            /// </summary>
            public const double DEFAULT_MULTIPLIER = 2.0;

            #endregion

            /// <summary>
            /// Screen the data using population standard deviation.
            /// This will calculate the standard deviation for all the good
            /// range values.  If any anomalies in the data are found, the value
            /// will be set to a bad range.
            /// Ensemble passed as a reference, so the data is saved to the object.
            /// </summary>
            /// <param name="ensemble">Ensemble to screen.</param>
            /// <param name="multiplier">Multiplier to the standard deviation.</param>
            /// <returns>Result of the screening calculations.</returns>
            public static ScreenBottomTrackRangeResult Screen(ref DataSet.Ensemble ensemble, double multiplier = DEFAULT_MULTIPLIER)
            {
                ScreenBottomTrackRangeResult result = new ScreenBottomTrackRangeResult();


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
                    else
                    {
                        stdev *= multiplier;
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
                    if ((curRangeB0 == DataSet.Ensemble.BAD_RANGE) || (curRangeB0 > maxRange || curRangeB0 < minRange))
                    {
                        ensemble.BottomTrackData.Range[DataSet.Ensemble.BEAM_0_INDEX] = DataSet.Ensemble.BAD_RANGE;
                        result.Beam0Bad = true;
                    }

                    if ((curRangeB1 == DataSet.Ensemble.BAD_RANGE) || (curRangeB1 > maxRange || curRangeB1 < minRange))
                    {
                        ensemble.BottomTrackData.Range[DataSet.Ensemble.BEAM_1_INDEX] = DataSet.Ensemble.BAD_RANGE;
                        result.Beam1Bad = true;
                    }

                    if ((curRangeB2 == DataSet.Ensemble.BAD_RANGE) || (curRangeB2 > maxRange || curRangeB2 < minRange))
                    {
                        ensemble.BottomTrackData.Range[DataSet.Ensemble.BEAM_2_INDEX] = DataSet.Ensemble.BAD_RANGE;
                        result.Beam2Bad = true;
                    }

                    if ((curRangeB3 == DataSet.Ensemble.BAD_RANGE) || (curRangeB3 > maxRange || curRangeB3 < minRange))
                    {
                        ensemble.BottomTrackData.Range[DataSet.Ensemble.BEAM_3_INDEX] = DataSet.Ensemble.BAD_RANGE;
                        result.Beam3Bad = true;
                    }

                    // Set the result values
                    result.Avg = avg;
                    result.StdDev = stdev;
                    result.MinRange = minRange;
                    result.MaxRange = maxRange;
                    result.Beam0Range = curRangeB0;
                    result.Beam1Range = curRangeB1;
                    result.Beam2Range = curRangeB2;
                    result.Beam3Range = curRangeB3;
                }

                return result;
            }

        }

    }

}