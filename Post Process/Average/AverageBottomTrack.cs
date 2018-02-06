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
 * 01/07/2013      RC          2.17       Initial coding
 * 07/01/2014      RC          2.23.0     Fixed a bug in SetTimeAndCount().
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
        /// Average the Bottom Track data.  This will take the Bottom Track data
        /// and continuously average the data.  
        /// </summary>
        public class AverageBottomTrack : AverageBase
        {

            #region Structs

            /// <summary>
            /// Struct to hold the accumulated Bottom Track data.
            /// </summary>
            protected struct AccumulatedBtData
            {
                /// <summary>
                /// Array for the accumulated values.
                /// </summary>
                public float[] AvgAccum { get; set; }

                /// <summary>
                /// Array for the number of values accumulated.
                /// </summary>
                public int[] AvgCount { get; set; }
            }

            #endregion

            #region Variables

            /// <summary>
            /// Range accumulator.
            /// </summary>
            private List<float[]> _rangeAccum;

            /// <summary>
            /// SNR accumulator.
            /// </summary>
            private List<float[]> _sNRAccum;

            /// <summary>
            /// Amplitude accumulator.
            /// </summary>
            private List<float[]> _amplitudeAccum;

            /// <summary>
            /// Corrlation accumulator.
            /// </summary>
            private List<float[]> _correlationAccum;

            /// <summary>
            /// Beam Velocity accumulator.
            /// </summary>
            private List<float[]> _beamVelocityAccum;

            /// <summary>
            /// Instrument Velocity accumulator.
            /// </summary>
            private List<float[]> _instrumentVelocityAccum;

            /// <summary>
            /// Earth Velocity accumulator.
            /// </summary>
            private List<float[]> _earthVelocityAccum;

            /// <summary>
            /// Ensemble first and last time accumulator.
            /// </summary>
            private List<float> _timeAccum;

            #endregion

            /// <summary>
            /// Initialize the values.
            /// </summary>
            public AverageBottomTrack() :
                base()
            {
                _rangeAccum = new List<float[]>();
                _sNRAccum = new List<float[]>();
                _amplitudeAccum = new List<float[]>();
                _correlationAccum = new List<float[]>();
                _beamVelocityAccum = new List<float[]>();
                _instrumentVelocityAccum = new List<float[]>();
                _earthVelocityAccum = new List<float[]>();
                _timeAccum = new List<float>();
            }

            /// <summary>
            /// Add the ensemble data to the accumulator.  This will accumulate all the
            /// Bottom Track data into a list.  If it is a running average, it will remove
            /// the first item in the list as needed.
            /// </summary>
            /// <param name="ensemble">Ensemble to accumulate.</param>
            public override void AddEnsemble(DataSet.Ensemble ensemble)
            {
                // Accumulate the data
                if (ensemble.IsBottomTrackAvail)
                {
                    _rangeAccum.Add(ensemble.BottomTrackData.Range);                                // Range
                    _sNRAccum.Add(ensemble.BottomTrackData.SNR);                                    // SNR
                    _amplitudeAccum.Add(ensemble.BottomTrackData.Amplitude);                        // Amplitude
                    _correlationAccum.Add(ensemble.BottomTrackData.Correlation);                    // Correlation
                    _beamVelocityAccum.Add(ensemble.BottomTrackData.BeamVelocity);                  // Beam Velocity
                    _instrumentVelocityAccum.Add(ensemble.BottomTrackData.InstrumentVelocity);      // Instrument Velocity
                    _earthVelocityAccum.Add(ensemble.BottomTrackData.EarthVelocity);                // Earth Velocity
                    _timeAccum.Add(ensemble.BottomTrackData.LastPingTime);                          // Time
                }
            }

            /// <summary>
            /// Set the average Bottom Track data to the Bottom Track data set array.
            /// This will replace the array with an averaged array for the accumulated data.
            /// If this is not a running average, it will clear the accumulator.
            /// </summary>
            /// <param name="ensemble">Set the average data to this ensemble.</param>
            /// <param name="rangeScale">Range Scale value to multiply to the averaged value.</param>
            /// <param name="snrScale">SNR Scale value to multiply to the averaged value.</param>
            /// <param name="ampScale">Amplitude Scale value to multiply to the averaged value.</param>
            /// <param name="corrScale">Correlation Scale value to multiply to the averaged value.</param>
            /// <param name="beamVelScale">Beam Velocity Scale value to multiply to the averaged value.</param>
            /// <param name="instrVelScale">Instrument Velocity Scale value to multiply to the averaged value.</param>
            /// <param name="earthVelScale">Earth Velocity Scale value to multiply to the averaged value.</param>
            public void SetAverage(ref DataSet.Ensemble ensemble, float rangeScale, float snrScale, float ampScale, float corrScale, float beamVelScale, float instrVelScale, float earthVelScale)
            {
                if (ensemble.IsBottomTrackAvail)
                {
                    ensemble.BottomTrackData.Range = AverageRange(rangeScale);                                      // Average Range
                    ensemble.BottomTrackData.SNR = AverageSnr(snrScale);                                            // Average SNR
                    ensemble.BottomTrackData.Amplitude = AverageAmplitude(ampScale);                                // Average Amplitude
                    ensemble.BottomTrackData.Correlation = AverageCorrelation(corrScale);                           // Average Correlation
                    ensemble.BottomTrackData.BeamVelocity = AverageBeamVelocity(beamVelScale);                      // Average Beam Velocity
                    ensemble.BottomTrackData.InstrumentVelocity = AverageInstrumentVelocity(instrVelScale);         // Average Instrument Velocity
                    ensemble.BottomTrackData.EarthVelocity = AverageEarthVelocity(earthVelScale);                   // Average Earth Velocity

                    // Set the first and last time
                    SetTimeAndCount(ref ensemble);
                }
            }

            /// <summary>
            /// Set the average Bottom Track data to the Bottom Track data set array.
            /// This will replace the array with an averaged array for the accumulated data.
            /// If this is not a running average, it will clear the accumulator.
            /// </summary>
            /// <param name="ensemble">Set the average data to this ensemble.</param>
            /// <param name="scale">Scale value to multiply to the averaged value.</param>
            public override void SetAverage(ref DataSet.Ensemble ensemble, float scale = DEFAULT_SCALE)
            {
                SetAverage(ref ensemble, scale, scale, scale, scale, scale, scale, scale);
            }

            /// <summary>
            /// Clear the ensembles or remove the last entry if it is a running average.
            /// </summary>
            public override void RemoveFirstEnsemble()
            {
                // Remove the first data in the accumulator
                if (_rangeAccum.Count >= _numSamples)
                {
                    _rangeAccum.RemoveAt(0);
                }

                if (_sNRAccum.Count >= _numSamples)
                {
                    _sNRAccum.RemoveAt(0);
                }

                if (_amplitudeAccum.Count >= _numSamples)
                {
                    _amplitudeAccum.RemoveAt(0);
                }

                if (_correlationAccum.Count >= _numSamples)
                {
                    _correlationAccum.RemoveAt(0);
                }

                if (_beamVelocityAccum.Count >= _numSamples)
                {
                    _beamVelocityAccum.RemoveAt(0);
                }

                if (_instrumentVelocityAccum.Count >= _numSamples)
                {
                    _instrumentVelocityAccum.RemoveAt(0);
                }

                if (_earthVelocityAccum.Count >= _numSamples)
                {
                    _earthVelocityAccum.RemoveAt(0);
                }

                if (_timeAccum.Count >= _numSamples)
                {
                    _timeAccum.RemoveAt(0);
                }
            }

            /// <summary>
            /// Clear the accumulators.
            /// </summary>
            public override void ClearAllEnsembles()
            {
                // Clear the Accumulators
                _rangeAccum.Clear();
                _sNRAccum.Clear();
                _amplitudeAccum.Clear();
                _correlationAccum.Clear();
                _beamVelocityAccum.Clear();
                _instrumentVelocityAccum.Clear();
                _earthVelocityAccum.Clear();
                _timeAccum.Clear();
            }

            #region Average

            #region Base

            /// <summary>
            /// Accumulate the data.
            /// </summary>
            /// <returns>The accumulated data.</returns>
            private AccumulatedBtData Accumulate(List<float[]> input)
            {
                AccumulatedBtData result = new AccumulatedBtData();

                if (input.Count > 0)
                {
                    // Get the number of bins and beams
                    // These values should be the same for all the accumulated data
                    float[] firstData = input.First();
                    int numBeams = firstData.GetLength(0);

                    // Create arrays to accumulate the data
                    float[] avgAccum = new float[numBeams];
                    int[] avgCount = new int[numBeams];

                    // Accumulate the data for each accumulate array
                    for (int x = 0; x < input.Count; x++)
                    {
                        // Get the data from the list
                        float[] data = input[x];

                        // Accumulate the values
                        for (int bin = 0; bin < data.GetLength(0); bin++)
                        {
                            // Beam 0
                            if (numBeams > 0)
                            {
                                float b0 = data[DataSet.Ensemble.BEAM_0_INDEX];
                                if (b0 != DataSet.Ensemble.BAD_VELOCITY)
                                {
                                    avgAccum[DataSet.Ensemble.BEAM_0_INDEX] += b0;
                                    avgCount[DataSet.Ensemble.BEAM_0_INDEX]++;
                                }
                            }

                            // Beam 1
                            if (numBeams > 1)
                            {
                                float b1 = data[DataSet.Ensemble.BEAM_1_INDEX];
                                if (b1 != DataSet.Ensemble.BAD_VELOCITY)
                                {
                                    avgAccum[DataSet.Ensemble.BEAM_1_INDEX] += b1;
                                    avgCount[DataSet.Ensemble.BEAM_1_INDEX]++;
                                }
                            }

                            // Beam 2
                            if (numBeams > 2)
                            {
                                float b2 = data[DataSet.Ensemble.BEAM_2_INDEX];
                                if (b2 != DataSet.Ensemble.BAD_VELOCITY)
                                {
                                    avgAccum[DataSet.Ensemble.BEAM_2_INDEX] += b2;
                                    avgCount[DataSet.Ensemble.BEAM_2_INDEX]++;
                                }
                            }

                            // Beam 3
                            if (numBeams > 3)
                            {
                                float b3 = data[DataSet.Ensemble.BEAM_3_INDEX];
                                if (b3 != DataSet.Ensemble.BAD_VELOCITY)
                                {
                                    avgAccum[DataSet.Ensemble.BEAM_3_INDEX] += b3;
                                    avgCount[DataSet.Ensemble.BEAM_3_INDEX]++;
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
            /// <returns>Array with the averaged and scaled value.</returns>
            /// <param name="scale">Scale value to multiply to the averaged value.</param>
            protected float[] AverageData(AccumulatedBtData accumData, float scale = 1.0f)
            {
                // Create an array, if no data is accumulated, null will be retruned
                float[] result = null;

                if (accumData.AvgAccum != null && accumData.AvgCount != null)
                {
                    // Create the results array based off the accumulated values
                    result = new float[accumData.AvgAccum.GetLength(0)];

                    int numBeams = accumData.AvgCount.GetLength(0);

                    // Check if it has at least 1 beams
                    if (numBeams > 0)
                    {
                        // Calculate the average values
                        float B0 = 0.0f;
                        if (accumData.AvgCount[DataSet.Ensemble.BEAM_0_INDEX] > 0)
                        {
                            B0 = accumData.AvgAccum[DataSet.Ensemble.BEAM_0_INDEX] / accumData.AvgCount[DataSet.Ensemble.BEAM_0_INDEX];       // Beam 0 Average
                        }

                        // Set results
                        B0 *= scale;
                        result[DataSet.Ensemble.BEAM_0_INDEX] = B0;
                    }

                    // Check if it has at least 2 beams
                    if (numBeams > 1)
                    {
                        float B1 = 0.0f;
                        if (accumData.AvgCount[DataSet.Ensemble.BEAM_1_INDEX] > 0)
                        {
                            B1 = accumData.AvgAccum[DataSet.Ensemble.BEAM_1_INDEX] / accumData.AvgCount[DataSet.Ensemble.BEAM_1_INDEX];       // Beam 1 Average
                        }

                        // Set Results
                        B1 *= scale;
                        result[DataSet.Ensemble.BEAM_1_INDEX] = B1;
                    }

                    // Check if it has a least 3 beams
                    if (numBeams > 2)
                    {
                        float B2 = 0.0f;
                        if (accumData.AvgCount[DataSet.Ensemble.BEAM_2_INDEX] > 0)
                        {
                            B2 = accumData.AvgAccum[DataSet.Ensemble.BEAM_2_INDEX] / accumData.AvgCount[DataSet.Ensemble.BEAM_2_INDEX];       // Beam 2 Average
                        }

                        // Set Results
                        B2 *= scale;
                        result[DataSet.Ensemble.BEAM_2_INDEX] = B2;
                    }

                    // Check if it has a least 4 beams
                    if (numBeams > 3)
                    {
                        float B3 = 0.0f;
                        if (accumData.AvgCount[DataSet.Ensemble.BEAM_3_INDEX] > 0)
                        {
                            B3 = accumData.AvgAccum[DataSet.Ensemble.BEAM_3_INDEX] / accumData.AvgCount[DataSet.Ensemble.BEAM_3_INDEX];       // Beam 3 Average
                        }

                        // Set Result
                        B3 *= scale;
                        result[DataSet.Ensemble.BEAM_3_INDEX] = B3;
                    }
                }

                return result;
            }

            #endregion

            #region Range

            /// <summary>
            /// Average the Range data.  
            /// </summary>
            /// <param name="scale">Scale value to multiply to the averaged data.</param>
            /// <returns>An average or the range data.</returns>
            private float[] AverageRange(float scale)
            {
                // Accumulate the data
                AccumulatedBtData data = Accumulate(_rangeAccum);

                // Then average the data
                return AverageData(data, scale);
            }

            #endregion

            #region SNR

            /// <summary>
            /// Average the SNR data.  
            /// </summary>
            /// <param name="scale">Scale value to multiply to the averaged data.</param>
            /// <returns>Average the SNR data.</returns>
            private float[] AverageSnr(float scale)
            {
                // Accumulate the data
                AccumulatedBtData data = Accumulate(_sNRAccum);

                // Then average the data
                return AverageData(data, scale);
            }

            #endregion

            #region Amplitude

            /// <summary>
            /// Average the Amplitude data.  
            /// </summary>
            /// <param name="scale">Scale value to multiply to the averaged data.</param>
            /// <returns>Average the Amplitude data.</returns>
            private float[] AverageAmplitude(float scale)
            {
                // Accumulate the data
                AccumulatedBtData data = Accumulate(_amplitudeAccum);

                // Then average the data
                return AverageData(data, scale);
            }

            #endregion

            #region Correlation

            /// <summary>
            /// Average the Correlation data.  
            /// </summary>
            /// <param name="scale">Scale value to multiply to the averaged data.</param>
            /// <returns>Average the Correlation data.</returns>
            private float[] AverageCorrelation(float scale)
            {
                // Accumulate the data
                AccumulatedBtData data = Accumulate(_correlationAccum);

                // Then average the data
                return AverageData(data, scale);
            }

            #endregion

            #region Beam Velocity

            /// <summary>
            /// Average the Beam Velocity data.  
            /// </summary>
            /// <param name="scale">Scale value to multiply to the averaged data.</param>
            /// <returns>Average the Beam Velocity data.</returns>
            private float[] AverageBeamVelocity(float scale)
            {
                // Accumulate the data
                AccumulatedBtData data = Accumulate(_beamVelocityAccum);

                // Then average the data
                return AverageData(data, scale);
            }

            #endregion

            #region Instrument Velocity

            /// <summary>
            /// Average the Instrument Velocity data.  
            /// </summary>
            /// <param name="scale">Scale value to multiply to the averaged data.</param>
            /// <returns>Average the Instrument Velocity data.</returns>
            private float[] AverageInstrumentVelocity(float scale)
            {
                // Accumulate the data
                AccumulatedBtData data = Accumulate(_instrumentVelocityAccum);

                // Then average the data
                return AverageData(data, scale);
            }

            #endregion

            #region Earth Velocity

            /// <summary>
            /// Average the Earth Velocity data.  
            /// </summary>
            /// <param name="scale">Scale value to multiply to the averaged data.</param>
            /// <returns>Average the Earth Velocity data.</returns>
            private float[] AverageEarthVelocity(float scale)
            {
                // Accumulate the data
                AccumulatedBtData data = Accumulate(_earthVelocityAccum);

                // Then average the data
                return AverageData(data, scale);
            }

            #endregion

            #endregion

            #region Set Time and count

            /// <summary>
            /// Set the time and the number of ensembles averaged together.
            /// </summary>
            /// <param name="ensemble">Ensemble to set the values.</param>
            private void SetTimeAndCount(ref DataSet.Ensemble ensemble)
            {
                if (_timeAccum.Count > 0)
                {
                    ensemble.BottomTrackData.ActualPingCount = _timeAccum.Count;        // Number in the average
                    ensemble.BottomTrackData.FirstPingTime = _timeAccum.First();        // First Ping Time
                    ensemble.BottomTrackData.LastPingTime = _timeAccum.Last();          // Last Ping Time
                }
            }

            #endregion

        }
    }
}
