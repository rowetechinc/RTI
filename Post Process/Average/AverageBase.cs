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
 * 01/04/2013      RC          2.17       Initial coding
 * 01/07/2013      RC          2.17       Added duplicate functions calls here.
 * 04/16/2015      RC          3.0.4      Set the average velocity to bad if nothing is accumulated.
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
        /// Base class for averaging.  This will give the 
        /// mininum number of method calls required for averaging.
        /// </summary>
        public abstract class AverageBase
        {

            #region Structs and Enums

            /// <summary>
            /// Struct to hold the accumulated ata.
            /// </summary>
            protected struct AccumulatedData
            {
                /// <summary>
                /// Array for the accumulated values.
                /// </summary>
                public float[,] AvgAccum { get; set; }

                /// <summary>
                /// Array for the number of values accumulated.
                /// </summary>
                public int[,] AvgCount { get; set; }
            }

            #endregion

            #region Variables

            /// <summary>
            /// Default number of samples if a bad number
            /// of samples is given.
            /// </summary>
            protected const int DEFAULT_NUM_SAMPLES = 2;

            /// <summary>
            /// Default scale.
            /// </summary>
            protected const float DEFAULT_SCALE = 1.0f;

            /// <summary>
            /// Accumulate the data.
            /// </summary>
            protected List<float[,]> _accumData;

            #endregion

            #region Properties

            /// <summary>
            /// Number of samples to accumulate before
            /// averaging the data.  The value cannot be less than or equal to 0.
            /// If a bad value is given, the default value will be used.
            /// </summary>
            protected int _numSamples;
            /// <summary>
            /// Number of samples to accumulate before
            /// averaging the data.  The value cannot be less than or equal to 0.
            /// If a bad value is given, the default value will be used.
            /// </summary>
            public int NumSamples 
            {
                get { return _numSamples; } 
                set
                {
                    if (value <= 0)
                    {
                        _numSamples = DEFAULT_NUM_SAMPLES;
                    }
                    else
                    {
                        _numSamples = value;
                    }
                }
            }

            /// <summary>
            /// Set wheter the average will be a running average or an
            /// average with X number of samples have been received.
            /// 
            /// A running average will continously average the data until it is cleared.
            /// 
            /// </summary>
            public bool IsRunningAverage { get; set; }

            /// <summary>
            /// Set wheter the sample averaging average will be a running average or an
            /// average with X number of samples have been received.
            /// 
            /// A running average will average together NumSamples together.
            /// As a new ensemble is added, the last one is removed and the 
            /// average is taken.  This will publish an averaged ensemble
            /// for every ensemble received.
            /// 
            /// A non-running (block) average will wait for NumSamples to be received.
            /// It will then take the average and clear the list of ensembles.
            /// It will then wait again for NumSamples.  This will publish an
            /// averaged ensemble after NumSamples have been received.
            /// </summary>
            public bool IsSampleRunningAverage { get; set; }

            /// <summary>
            /// Get or set the scale for the average value.  This value should be
            /// mulitplied to the averaged value.
            /// </summary>
            public float Scale { get; set; }

            #endregion

            /// <summary>
            /// Constructor will not take any arguments.
            /// </summary>
            public AverageBase()
            {
                // Initialize values
                Scale = DEFAULT_SCALE;
                NumSamples = DEFAULT_NUM_SAMPLES;

                _accumData = new List<float[,]>();
            }

            /// <summary>
            /// Add and ensemble to be averaged.
            /// </summary>
            /// <param name="ensemble">Add an ensemble to be averaged.</param>
            public abstract void AddEnsemble(DataSet.Ensemble ensemble);

            /// <summary>
            /// Set the averaged value to the given ensemble.  This will take
            /// the given ensemble and replace the data within it with the average
            /// data.  If it is not a RunningAverage, the accumulator should then
            /// clear.
            /// </summary>
            /// <param name="ensemble">Ensemble to set the average.</param>
            /// <param name="scale">Scale value to multiply to the averaged value.</param>
            public abstract void SetAverage(ref DataSet.Ensemble ensemble, float scale);

            /// <summary>
            /// Get the averaged value.  This will take the accumulated values and generate
            /// an averaged value.  It will then multiply the scale value to the averaged
            /// value.  The average value is then stored to the array.
            /// If no data has been accumulated, NULL will be returned.
            /// </summary>
            /// <param name="scale">Scale value to multiply to the averaged value.</param>
            /// <returns>An array of averaged value and a scale value multiplied in.</returns>
            public virtual float[,] GetAverage(float scale = 1.0f)
            {
                // Accumulate the data
                AccumulatedData data = AccumulateData();

                // Then average the data
                return AverageData(data, scale);
            }

            #region Methods

            /// <summary>
            /// Check if we have accumulated enough data.  If we have
            /// determine how we should remove the data.
            /// 
            /// If this is a running average, only remove the
            /// first node from the list.  The remaining values
            /// in the accumulator will be used for the next
            /// ensemble received.
            /// 
            /// If this is not a running average, clear the entire
            /// list and wait for _numSamples to be accumulated
            /// before taking the next average.
            /// </summary>
            public virtual void RemoveFirstEnsemble()
            {
                // Check if the number of samples has been met in the list
                if (_accumData.Count >= 1)
                {
                    // Remove the first data in the list
                    _accumData.RemoveAt(0);
                }
            }

            /// <summary>
            /// Clear the accumulator.
            /// </summary>
            public virtual void ClearAllEnsembles()
            {
                // Clear the list
                _accumData.Clear();
            }

            /// <summary>
            /// This will accumulate all the data.  THis will go through each
            /// accumulated data and accumulate the values.  It will check for
            /// bad values before accumulating.  If the value is bad, it will
            /// not be included in the accumulated data.  This will keep track
            /// of the accumulated data and the number of data points for each bin.
            /// The results will then be stored to a struct and returned.
            /// </summary>
            /// <returns>Struct containing the accumulated data.</returns>
            protected AccumulatedData AccumulateData()
            {
                AccumulatedData result = new AccumulatedData();

                if (_accumData.Count > 0)
                {
                    // Get the number of bins and beams
                    // These values should be the same for all the accumulated data
                    float[,] firstData = _accumData.First();
                    int numBins = firstData.GetLength(0);
                    int numBeams = firstData.GetLength(1);

                    // Create arrays to accumulate the data
                    float[,] avgAccum = new float[numBins, numBeams];
                    int[,] avgCount = new int[numBins, numBeams];

                    // Accumulate the data for each accumulate array
                    for (int x = 0; x < _accumData.Count; x++)
                    {
                        // Get the data from the list
                        float[,] data = _accumData[x];

                        // Accumulate the values
                        for (int bin = 0; bin < data.GetLength(0); bin++)
                        {
                            // Beam 0
                            if (numBeams > 0)
                            {
                                float b0 = data[bin, DataSet.Ensemble.BEAM_0_INDEX];
                                if (b0 != DataSet.Ensemble.BAD_VELOCITY)
                                {
                                    avgAccum[bin, DataSet.Ensemble.BEAM_0_INDEX] += b0;
                                    avgCount[bin, DataSet.Ensemble.BEAM_0_INDEX]++;
                                }
                            }

                            // Beam 1
                            if (numBeams > 1)
                            {
                                float b1 = data[bin, DataSet.Ensemble.BEAM_1_INDEX];
                                if (b1 != DataSet.Ensemble.BAD_VELOCITY)
                                {
                                    avgAccum[bin, DataSet.Ensemble.BEAM_1_INDEX] += b1;
                                    avgCount[bin, DataSet.Ensemble.BEAM_1_INDEX]++;
                                }
                            }

                            // Beam 2
                            if (numBeams > 2)
                            {
                                float b2 = data[bin, DataSet.Ensemble.BEAM_2_INDEX];
                                if (b2 != DataSet.Ensemble.BAD_VELOCITY)
                                {
                                    avgAccum[bin, DataSet.Ensemble.BEAM_2_INDEX] += b2;
                                    avgCount[bin, DataSet.Ensemble.BEAM_2_INDEX]++;
                                }
                            }

                            // Beam 3
                            if (numBeams > 3)
                            {
                                float b3 = data[bin, DataSet.Ensemble.BEAM_3_INDEX];
                                if (b3 != DataSet.Ensemble.BAD_VELOCITY)
                                {
                                    avgAccum[bin, DataSet.Ensemble.BEAM_3_INDEX] += b3;
                                    avgCount[bin, DataSet.Ensemble.BEAM_3_INDEX]++;
                                }
                            }
                        }
                    }

                    // Set the accumulated data
                    result.AvgAccum = avgAccum;
                    result.AvgCount = avgCount;
                }

                return result;
            }

            /// <summary>
            /// Average all the given accumulated data.  This will then multiple a
            /// scale value to the averaged data.
            /// </summary>
            /// <param name="accumData">Accumulated values.</param>
            /// <param name="scale">Scale value to multiply to averaged value.</param>
            /// <returns>Array with the averaged and scaled value.</returns>
            protected float[,] AverageData(AccumulatedData accumData, float scale)
            {
                // Create an array, if no data is accumulated, null will be retruned
                float[,] result = null;

                if (accumData.AvgAccum != null && accumData.AvgCount != null)
                {
                    // Create the results array based off the accumulated values
                    result = new float[accumData.AvgAccum.GetLength(0), accumData.AvgAccum.GetLength(1)];

                    // Set Min and Max bin
                    int minBin = 0;
                    int maxBin = accumData.AvgAccum.GetLength(0);
                    int numBeams = accumData.AvgCount.GetLength(1);

                    // Average all the accumulated data
                    for (int bin = minBin; bin < maxBin; bin++)
                    {
                        // Check if it has at least 1 beams
                        if (numBeams > 0)
                        {
                            // Calculate the average values
                            float B0 = 0.0f;
                            if (accumData.AvgCount[bin, DataSet.Ensemble.BEAM_0_INDEX] > 0)
                            {
                                B0 = accumData.AvgAccum[bin, DataSet.Ensemble.BEAM_0_INDEX] / accumData.AvgCount[bin, DataSet.Ensemble.BEAM_0_INDEX];       // Beam 0 Average
                                // Set results
                                B0 *= scale;
                                result[bin, DataSet.Ensemble.BEAM_0_INDEX] = B0;
                            }
                            else
                            {
                                result[bin, DataSet.Ensemble.BEAM_0_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
                            }
                        }

                        // Check if it has at least 2 beams
                        if (numBeams > 1)
                        {
                            float B1 = 0.0f;
                            if (accumData.AvgCount[bin, DataSet.Ensemble.BEAM_1_INDEX] > 0)
                            {
                                B1 = accumData.AvgAccum[bin, DataSet.Ensemble.BEAM_1_INDEX] / accumData.AvgCount[bin, DataSet.Ensemble.BEAM_1_INDEX];       // Beam 1 Average

                                // Set Results
                                B1 *= scale;
                                result[bin, DataSet.Ensemble.BEAM_1_INDEX] = B1;
                            }
                            else
                            {
                                result[bin, DataSet.Ensemble.BEAM_1_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
                            }
                        }

                        // Check if it has a least 3 beams
                        if (numBeams > 2)
                        {
                            float B2 = 0.0f;
                            if (accumData.AvgCount[bin, DataSet.Ensemble.BEAM_2_INDEX] > 0)
                            {
                                B2 = accumData.AvgAccum[bin, DataSet.Ensemble.BEAM_2_INDEX] / accumData.AvgCount[bin, DataSet.Ensemble.BEAM_2_INDEX];       // Beam 2 Average
                                
                                // Set Results
                                B2 *= scale;
                                result[bin, DataSet.Ensemble.BEAM_2_INDEX] = B2;
                            }
                            else
                            {
                                result[bin, DataSet.Ensemble.BEAM_2_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
                            }
                        }

                        // Check if it has a least 4 beams
                        if (numBeams > 3)
                        {
                            float B3 = 0.0f;
                            if (accumData.AvgCount[bin, DataSet.Ensemble.BEAM_3_INDEX] > 0)
                            {
                                B3 = accumData.AvgAccum[bin, DataSet.Ensemble.BEAM_3_INDEX] / accumData.AvgCount[bin, DataSet.Ensemble.BEAM_3_INDEX];       // Beam 3 Average

                                // Set Result
                                B3 *= scale;
                                result[bin, DataSet.Ensemble.BEAM_3_INDEX] = B3;
                            }
                            else
                            {
                                result[bin, DataSet.Ensemble.BEAM_3_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
                            }
                        }
                    }
                }

                return result;
            }

            #endregion
        }
    }
}
