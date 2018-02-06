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
 * 06/25/2013      RC          2.23.0     Initial coding
 * 04/15/2015      RC          3.0.4      Average Velocity Vectors.
 * 
 */

namespace RTI
{

    namespace Average
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Text;

        /// <summary>
        /// Average the Earth Velocity data.  This will take the Earth Velocity data
        /// and continuously average the data.  
        /// </summary>
        public class AverageEarthVelocity : AverageBase
        {

            #region Variable

            /// <summary>
            /// Accumulate the Earth velocity vector data.
            /// </summary>
            protected List<DataSet.VelocityVector[]> _accumVV;

            #endregion

            /// <summary>
            /// Initialize the values.
            /// </summary>
            public AverageEarthVelocity() :
                base()
            {
                _accumVV = new List<DataSet.VelocityVector[]>();
            }

            /// <summary>
            /// Add the ensemble data to the accumulator.  This will accumulate all the
            /// Earth Velocity data into a list.  If it is a running average, it will remove
            /// the first item in the list as needed.
            /// </summary>
            /// <param name="ensemble">Ensemble to accumulate.</param>
            public override void AddEnsemble(DataSet.Ensemble ensemble)
            {
                // Accumulate the data
                if (ensemble.IsEarthVelocityAvail)
                {
                    _accumData.Add(ensemble.EarthVelocityData.EarthVelocityData);

                    // Accumulate the velocity vector
                    if(ensemble.EarthVelocityData.IsVelocityVectorAvail)
                    {
                        _accumVV.Add(ensemble.EarthVelocityData.VelocityVectors);
                    }
                }
            }

            /// <summary>
            /// Set the average Earth Velocity data to the Earth Velocity data set array.
            /// This will replace the array with an averaged array for the accumulated data.
            /// If this is not a running average, it will clear the accumulator.
            /// </summary>
            /// <param name="ensemble">Set the average data to this ensemble.</param>
            /// <param name="scale">Scale value to multiply to the averaged value.</param>
            public override void SetAverage(ref DataSet.Ensemble ensemble, float scale)
            {
                if (ensemble.IsEarthVelocityAvail)
                {
                    ensemble.EarthVelocityData.EarthVelocityData = GetAverage(scale);

                    // Average the velocity vector
                    if (ensemble.EarthVelocityData.IsVelocityVectorAvail)
                    {
                        AverageVelocityVector(ref ensemble);
                    }
                }
            }

            /// <summary>
            /// Clear the base and also the velocity vector 
            /// list.
            /// </summary>
            public override void ClearAllEnsembles()
            {
                base.ClearAllEnsembles();

                _accumVV.Clear();
            }

            /// <summary>
            /// Average the velocity vector data.
            /// </summary>
            /// <param name="ensemble">Ensemble to set the average to.</param>
            private void AverageVelocityVector(ref DataSet.Ensemble ensemble)
            {
                int count = ensemble.EarthVelocityData.VelocityVectors.Length;

                // Accumulate all the bin data
                var accumMag = new double[count];
                var magCount = new int[count];
                var accumDirXNorth = new double[count];
                var dirCountXNorth = new int[count];
                var accumDirYNorth = new double[count];
                var dirCountYNorth = new int[count];
                foreach(var vv in _accumVV)
                {
                    for(int bin = 0; bin < vv.Length; bin++)
                    {
                        if(vv.Length <= count)
                        {
                            if(vv[bin].Magnitude != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                accumMag[bin] += vv[bin].Magnitude;
                                magCount[bin]++;
                            }

                            if(vv[bin].DirectionXNorth != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                accumDirXNorth[bin] += vv[bin].DirectionXNorth;
                                dirCountXNorth[bin]++;
                            }

                            if(vv[bin].DirectionYNorth != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                accumDirYNorth[bin] += vv[bin].DirectionYNorth;
                                dirCountYNorth[bin]++;
                            }
                        }
                    }
                }

                // Average the accumulated data
                for(int x = 0; x < count; x++)
                {
                    if (magCount[x] == 0)
                    {
                        ensemble.EarthVelocityData.VelocityVectors[x].Magnitude = DataSet.Ensemble.BAD_VELOCITY;
                    }
                    else
                    {
                        ensemble.EarthVelocityData.VelocityVectors[x].Magnitude = accumMag[x] / magCount[x];
                    }

                    if (dirCountXNorth[x] == 0)
                    {
                        ensemble.EarthVelocityData.VelocityVectors[x].DirectionXNorth = DataSet.Ensemble.BAD_VELOCITY;
                    }
                    else
                    {
                        ensemble.EarthVelocityData.VelocityVectors[x].DirectionXNorth = accumDirXNorth[x] / dirCountXNorth[x];
                    }

                    if (dirCountYNorth[x] == 0)
                    {
                        ensemble.EarthVelocityData.VelocityVectors[x].DirectionYNorth = DataSet.Ensemble.BAD_VELOCITY;
                    }
                    else
                    {
                        ensemble.EarthVelocityData.VelocityVectors[x].DirectionYNorth = accumDirYNorth[x] / dirCountYNorth[x];
                    }
                }
            }

        }
    }
}
