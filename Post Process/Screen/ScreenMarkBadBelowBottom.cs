﻿/*
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
 * 04/26/2013      RC          2.19       Changed namespace from Screen to ScreenData.
 * 08/13/2013      RC          2.19.4     Get the AverageRange from BottomTrackDataSet.
 * 01/31/2018      RC          3.4.5      Added Previous BT Range.
 * 10/10/2019      RC          3.4.14     Moved GetBottomBin to Ensemble.
 * 
 */


using RTI.DataSet;
using System;
namespace RTI
{
    namespace ScreenData
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
            /// <param name="prevBottom">Previous Good Bottom.</param>
            /// <returns>True = Screen could be done.</returns>
            public static bool Screen(ref DataSet.Ensemble ensemble, double prevBottom = DataSet.Ensemble.BAD_RANGE)
            {
                if (ensemble != null)
                {
                    // Ensure bottom track data exist
                    if (ensemble.IsBottomTrackAvail &&          // Needed for Bottom Depth 
                        ensemble.IsAncillaryAvail &&            // Needed for Blank and Bin Size
                        ensemble.IsEnsembleAvail                // Needed for number of bins
                        )
                    {
                        // Get the bottom
                        double bottom = ensemble.BottomTrackData.GetAverageRange();

                        // Ensure we found a bottom
                        if (bottom == DataSet.Ensemble.BAD_RANGE && prevBottom == DataSet.Ensemble.BAD_RANGE)
                        {
                            return false;
                        }
                        else if(bottom == DataSet.Ensemble.BAD_RANGE && prevBottom != DataSet.Ensemble.BAD_RANGE)
                        {
                            // PrevBottom is good, so use it
                            bottom = prevBottom;
                        }

                        // Get the bottom bin
                        int bottomBin = Ensemble.GetBottomBin(ensemble, bottom);

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
                return false;
            }
        }

    }

}