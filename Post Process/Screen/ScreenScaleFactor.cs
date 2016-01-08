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
 * 12/23/2015      RC          3.3.0       Initial coding
 * 
 */


using RTI.DataSet;
using System;
namespace RTI
{
    namespace ScreenData
    {
        /// <summary>
        /// Apply a scale factor to all the velocity and bottom track data.  If the speed of sound value is incorrect,
        /// the data needs to be scaled.  
        /// First the correct speed of sound needs to be calculated.
        /// Then create a scale factor.
        /// 
        /// Speed of Sound for less than 1000m
        /// SoS = (1449.20 + 4.60000 * T - 0.055 * T * T + 0.00029 * T * T * T + (1.34 - 0.01 * T) * (S - 35.0) + 0.01600 * D)
        /// T = Temperature
        /// D = Depth of Transducer
        /// S = Salinity
        /// 
        /// Scale Factor = Actual / Measured
        /// The Actual value is the recalculated Speed of Sound value based off the new Temperature, Tranducer Depth and/or Salinity.
        /// The Measured value is the original Speed of Sound value that was used by the ADCP.
        /// 
        /// 
        /// </summary>
        public class ScreenScaleFactor
        {
            /// <summary>
            /// Apply the scale factor to the Beam, Earth and Instrument Velocity.
            /// </summary>
            /// <param name="ensemble">Ensemble to screen.</param>
            /// <param name="scaleFactor">Scale factor to apply.</param>
            /// <returns>TRUE if screening occured.</returns>
            public static bool Screen(ref DataSet.Ensemble ensemble, float scaleFactor)
            {
                bool result = false;

                // If any of the velocity data is available,
                // then screening can occur
                result = ScreenBeamVelocity(ref ensemble, scaleFactor);
                result = ScreenEarthVelocity(ref ensemble, scaleFactor);
                result = ScreenInstrumentVelocity(ref ensemble, scaleFactor);
                result = ScreenBottomTrack(ref ensemble, scaleFactor);

                return result;
            }

            /// <summary>
            /// Apply the scale factor to the Beam velocity data.  
            /// 
            /// This will check for any bad velocity.
            /// </summary>
            /// <param name="ensemble">Ensemble to screen.</param>
            /// <param name="scaleFactor">Scale factor.</param>
            /// <returns>TRUE if screening occured.</returns>
            public static bool ScreenBeamVelocity(ref DataSet.Ensemble ensemble, float scaleFactor)
            {
                // Screen the data if it exist
                if (ensemble.IsBeamVelocityAvail)
                {
                    // Go through each bin, checking the data
                    for (int bin = 0; bin < ensemble.BeamVelocityData.NumElements; bin++)
                    {
                        for(int beam = 0; beam < ensemble.BeamVelocityData.BeamVelocityData.GetLength(1); beam++)
                        {
                            if(ensemble.BeamVelocityData.BeamVelocityData[bin, beam] != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                ensemble.BeamVelocityData.BeamVelocityData[bin, beam] = ensemble.BeamVelocityData.BeamVelocityData[bin, beam] * scaleFactor;
                            }
                        }
                    }

                    return true;
                }
                
                return false;
            }

            /// <summary>
            /// Apply the scale factor to the Instrument velocity data.  
            /// 
            /// This will check for any bad velocity.
            /// </summary>
            /// <param name="ensemble">Ensemble to screen.</param>
            /// <param name="scaleFactor">Scale factor.</param>
            /// <returns>TRUE if screening occured.</returns>
            public static bool ScreenInstrumentVelocity(ref DataSet.Ensemble ensemble, float scaleFactor)
            {
                // Screen the data if it exist
                if (ensemble.IsInstrumentVelocityAvail)
                {
                    // Go through each bin, checking the data
                    for (int bin = 0; bin < ensemble.InstrumentVelocityData.NumElements; bin++)
                    {
                        for (int beam = 0; beam < ensemble.InstrumentVelocityData.InstrumentVelocityData.GetLength(1); beam++)
                        {
                            if (ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, beam] != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, beam] = ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, beam] * scaleFactor;
                            }
                        }
                    }

                    return true;
                }

                return false;
            }

            /// <summary>
            /// Apply the scale factor to the Earth velocity data.  
            /// 
            /// This will check for any bad velocity.
            /// </summary>
            /// <param name="ensemble">Ensemble to screen.</param>
            /// <param name="scaleFactor">Scale factor.</param>
            /// <returns>TRUE if screening occured.</returns>
            public static bool ScreenEarthVelocity(ref DataSet.Ensemble ensemble, float scaleFactor)
            {
                // Screen the data if it exist
                if (ensemble.IsEarthVelocityAvail)
                {
                    // Go through each bin, checking the data
                    for (int bin = 0; bin < ensemble.EarthVelocityData.NumElements; bin++)
                    {
                        for (int beam = 0; beam < ensemble.EarthVelocityData.EarthVelocityData.GetLength(1); beam++)
                        {
                            if (ensemble.EarthVelocityData.EarthVelocityData[bin, beam] != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                ensemble.EarthVelocityData.EarthVelocityData[bin, beam] = ensemble.EarthVelocityData.EarthVelocityData[bin, beam] * scaleFactor;
                            }
                        }
                    }

                    return true;
                }

                return false;
            }

            /// <summary>
            /// Apply the scale factor to the Earth velocity data.  
            /// 
            /// This will check for any bad velocity.
            /// </summary>
            /// <param name="ensemble">Ensemble to screen.</param>
            /// <param name="scaleFactor">Scale factor.</param>
            /// <returns>TRUE if screening occured.</returns>
            public static bool ScreenBottomTrack(ref DataSet.Ensemble ensemble, float scaleFactor)
            {
                // Screen the data if it exist
                if (ensemble.IsBottomTrackAvail)
                {
                    // Beam Data
                    for (int beam = 0; beam < ensemble.BottomTrackData.BeamVelocity.GetLength(0); beam++)
                    {
                        if (ensemble.BottomTrackData.BeamVelocity[beam] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            ensemble.BottomTrackData.BeamVelocity[beam] = ensemble.BottomTrackData.BeamVelocity[beam] * scaleFactor;
                        }
                    }

                    // Earth Data
                    for (int beam = 0; beam < ensemble.BottomTrackData.EarthVelocity.GetLength(0); beam++)
                    {
                        if (ensemble.BottomTrackData.EarthVelocity[beam] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            ensemble.BottomTrackData.EarthVelocity[beam] = ensemble.BottomTrackData.EarthVelocity[beam] * scaleFactor;
                        }
                    }

                    // Range Data
                    for (int beam = 0; beam < ensemble.BottomTrackData.Range.GetLength(0); beam++)
                    {
                        if (ensemble.BottomTrackData.Range[beam] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            ensemble.BottomTrackData.Range[beam] = ensemble.BottomTrackData.Range[beam] * scaleFactor;
                        }
                    }
                    

                    return true;
                }

                return false;
            }


        }
    }

}