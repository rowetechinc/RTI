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
 * 12/24/2012      RC          2.17       Initial coding
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
        /// Screen the Earth Vertical Velocity against a threshold.  If the value is
        /// greater than the threshold then the bin is considered bad.  The value
        /// can be + or - larger.  So the absolute value will be taken of the value.
        /// 
        /// The Vertical Velocity (V) is assumed to be 0 for a good value.  This will screen
        /// for values to far from 0 in the positive or negative.  The thresold will be checked
        /// against the absolute value of the Q value.  If the value is greater than or equal to
        /// the threshold then the value is bad.
        /// </summary>
        public class ScreenVerticalVelocityThreshold
        {

            /// <summary>
            /// Screen the ensemble with the given threshold.
            /// </summary>
            /// <param name="ensemble">Ensemble to screen.</param>
            /// <param name="threshold">Threshold to check against.</param>
            /// <returns>TRUE =  screening could be done.</returns>
            public static bool Screen(ref DataSet.Ensemble ensemble, double threshold)
            {
                // If the Earth Velocity does not exist,
                // then we cannot screen the data
                if (!ensemble.IsEarthVelocityAvail)
                {
                    return false;
                }

                // Go through each bin in the Earth Velocity Data
                for (int bin = 0; bin < ensemble.EarthVelocityData.NumElements; bin++)
                {
                    if (Math.Abs(ensemble.EarthVelocityData.EarthVelocityData[bin, Ensemble.BEAM_VERTICAL_INDEX]) >= threshold)
                    {
                        //// Mark all the values bad
                        //ensemble.EarthVelocityData.EarthVelocityData[bin, Ensemble.BEAM_EAST_INDEX] = Ensemble.BAD_VELOCITY;
                        //ensemble.EarthVelocityData.EarthVelocityData[bin, Ensemble.BEAM_NORTH_INDEX] = Ensemble.BAD_VELOCITY;
                        //ensemble.EarthVelocityData.EarthVelocityData[bin, Ensemble.BEAM_VERTICAL_INDEX] = Ensemble.BAD_VELOCITY;
                        //ensemble.EarthVelocityData.EarthVelocityData[bin, Ensemble.BEAM_Q_INDEX] = Ensemble.BAD_VELOCITY;

                        // Set all values bad
                        EnsembleHelper.SetVelocitiesBad(ref ensemble, bin);
                    }
                }


                return true;
            }
        }
    }
}
