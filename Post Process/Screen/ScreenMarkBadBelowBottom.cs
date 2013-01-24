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
 * 03/05/2012      RC          2.05       Initial coding
 * 12/24/2012      RC          2.17       Moved SetVelocitiesBad() to EnsembleHelper.
 * 
 */


using RTI.DataSet;
using System;
namespace RTI
{
    namespace Screen
    {
        /// <summary>
        /// Screen the velocity data for any data below the Bottom.  
        /// The bottom is given through bottom track data.  If there
        /// is any data that is below the bottom, based of bin location,
        /// then mark those velocities bad.
        /// </summary>
        public class ScreenMarkBadBelowBottom
        {
            /// <summary>
            /// Screen the data for any velocities below the bottom.
            /// This will check if Bottom Track data exist.  If it does not,
            /// then we do not know the bottom.  It will then check that a
            /// bottom track range is given.  If all the range values are bad,
            /// then we do not know the bottom.  If they are good, take the average
            /// of the range.  Then determine which bin is located at and below the bottom.
            /// Then mark all the velocities at and below the bottom bad.
            /// </summary>
            /// <param name="ensemble">Ensemble to screen.</param>
            /// <returns>True = Screen could be done.</returns>
            public static bool Screen(ref DataSet.Ensemble ensemble)
            {
                // Ensure bottom track data exist
                if (ensemble.IsBottomTrackAvail &&          // Needed for Bottom Depth 
                    ensemble.IsAncillaryAvail &&            // Needed for Blank and Bin Size
                    ensemble.IsEnsembleAvail                // Needed for number of bins
                    )
                {
                    // Get the bottom
                    double bottom = GetAverageRange(ensemble);
                    
                    // Ensure we found a bottom
                    if (bottom == DataSet.Ensemble.BAD_RANGE)
                    {
                        return false;
                    }

                    // Get the bottom bin
                    int bottomBin = GetBottomBin(ensemble, bottom);

                    // Check if the bottom bin is at or beyond
                    // the number of bins
                    if (bottomBin < 0 || bottomBin >= ensemble.EnsembleData.NumBins)
                    {
                        return true;
                    }

                    // Set all the velocities bad
                    // for the bins below the bottom.
                    // This will also set the Good Pings bad
                    for (int bin = bottomBin; bin < ensemble.EnsembleData.NumBins; bin++)
                    {
                        EnsembleHelper.SetVelocitiesBad(ref ensemble, bin);
                    }


                        return true;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// This method is similar to BottomTrackDataSet.GetAverageRange() except
            /// it will not use the value if there is not at least 2 good values.  Anything
            /// less than 2 good values will not be used.  This is to ensure the 1 bad value is
            /// not used.  If we get 1 good and 1 bad, it is better than nothing.
            /// </summary>
            /// <param name="ensemble">Ensemble to get the range.</param>
            /// <returns>Average of the range values.</returns>
            private static double GetAverageRange(DataSet.Ensemble ensemble)
            {
                int count = 0;
                double result = 0;
                for (int beam = 0; beam < ensemble.BottomTrackData.ElementsMultiplier; beam++)
                {
                    if (ensemble.BottomTrackData.Range[beam] > DataSet.Ensemble.BAD_RANGE)
                    {
                        result += ensemble.BottomTrackData.Range[beam];
                        count++;
                    }
                }

                // If all values are bad
                // Return a bad range
                if (count < 2)
                {
                    return DataSet.Ensemble.BAD_RANGE;
                }

                return result / count;
            }

            /// <summary>
            /// Get the bin number for the bottom.  This will
            /// determine which bin is the bottom.  To find the bottom
            /// bin, determine how many bins can fit to reach the bottom.
            /// You must also take into account the blank.  A blank could
            /// be larger then a bin size, so include the blank in the calculation
            /// of finding the bottom bin.
            /// 
            ///             |     |
            ///             \     /
            ///              -----
            ///              
            ///              Blank
            ///              
            ///              -----
            ///               Bin
            ///              -----
            ///               Bin
            ///              -----
            ///               ...
            ///              -----
            ///               Bin
            ///               
            ///        --------------------
            ///              Bottom 
            /// </summary>
            /// <param name="ensemble">Ensemble to screen.</param>
            /// <param name="bottom">Bottom Depth.</param>
            /// <returns>Bin of the bottom.</returns>
            private static int GetBottomBin(DataSet.Ensemble ensemble, double bottom)
            {
                double binSize = ensemble.AncillaryData.BinSize;
                double blank = ensemble.AncillaryData.FirstBinRange;

                // Determine the how many bins 
                // get us to the bottom by dividing
                // by the bin size.  If the blank
                // is larger then a bin size, we have
                // to take into account the blank as 
                // 1 or more bins.
                if (blank < binSize)
                {
                    return (int)(bottom / binSize);
                }
                else
                {
                    // If the blank is bigger than the bin size
                    // Determine how many bins fit in the blank and
                    // subtract that from the end result
                    int binsInBlank = (int)((blank / binSize) + 0.5);
                    return (int)(bottom / (binSize)) - binsInBlank;
                }
            }

            ///// <summary>
            ///// Set all velocities and Good Ping data bad
            ///// for the given bin.  
            ///// </summary>
            ///// <param name="ensemble">Ensemble to modify.</param>
            ///// <param name="bin">Bin to modify.</param>
            //private static void SetVelocitiesBad(ref DataSet.Ensemble ensemble, int bin)
            //{
            //    // Beam Velocities
            //    if (ensemble.IsBeamVelocityAvail)
            //    {
            //        ensemble.BeamVelocityData.BeamVelocityData[bin, DataSet.Ensemble.BEAM_0_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
            //        ensemble.BeamVelocityData.BeamVelocityData[bin, DataSet.Ensemble.BEAM_1_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
            //        ensemble.BeamVelocityData.BeamVelocityData[bin, DataSet.Ensemble.BEAM_2_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
            //        ensemble.BeamVelocityData.BeamVelocityData[bin, DataSet.Ensemble.BEAM_3_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
            //    }
            //    if (ensemble.IsGoodBeamAvail)
            //    {
            //        ensemble.GoodBeamData.GoodBeamData[bin, DataSet.Ensemble.BEAM_0_INDEX] = 0;
            //        ensemble.GoodBeamData.GoodBeamData[bin, DataSet.Ensemble.BEAM_1_INDEX] = 0;
            //        ensemble.GoodBeamData.GoodBeamData[bin, DataSet.Ensemble.BEAM_2_INDEX] = 0;
            //        ensemble.GoodBeamData.GoodBeamData[bin, DataSet.Ensemble.BEAM_3_INDEX] = 0;
            //    }

            //    // Earth Velocities
            //    if (ensemble.IsEarthVelocityAvail)
            //    {
            //        ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
            //        ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
            //        ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
            //        ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_Q_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
            //    }
            //    if (ensemble.IsGoodEarthAvail)
            //    {
            //        ensemble.GoodEarthData.GoodEarthData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] = 0;
            //        ensemble.GoodEarthData.GoodEarthData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] = 0;
            //        ensemble.GoodEarthData.GoodEarthData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0;
            //        ensemble.GoodEarthData.GoodEarthData[bin, DataSet.Ensemble.BEAM_Q_INDEX] = 0;
            //    }

            //    // Earth Velocities
            //    if (ensemble.IsInstrVelocityAvail)
            //    {
            //        ensemble.InstrVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_X_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
            //        ensemble.InstrVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_Y_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
            //        ensemble.InstrVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_Z_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
            //        ensemble.InstrVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_Q_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
            //    }
            //}


        }

    }

}