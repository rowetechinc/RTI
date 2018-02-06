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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI
{
    namespace Average
    {
        /// <summary>
        /// Average bins together to create a larger bin.
        /// </summary>
        public class AverageBinsTogether
        {

            /// <summary>
            /// Average the ensembles bins together.
            /// Bin size will set the size of a bin.  We will then find
            /// how many bins need to be grouped together and averaged.
            /// This is basically equavialent to setting the bin size.
            /// But if the bin size incorrectly, this will merge bins together
            /// to create a greater bin.  
            /// 
            /// If the bin size given, is less than or equal to the bin size,
            /// nothing will be given.  This will average all the amplitude, correlation
            /// and velocity data together.
            /// </summary>
            /// <param name="ensemble">Ensemble to average.</param>
            /// <param name="binSize">Size of the new bin.</param>
            /// <returns>TRUE = averaging could be done.</returns>
            public static bool Average(ref DataSet.Ensemble ensemble, float binSize)
            {
                // Beam Velocity
                AverageBeamVelocity(ref ensemble, binSize);

                // Instrument Velocity
                AverageInstrumentVelocity(ref ensemble, binSize);

                // Earth Velocity
                AverageEarthVelocity(ref ensemble, binSize);

                // Amplitude
                AverageAmplitude(ref ensemble, binSize);

                // Correlation
                AverageCorrelation(ref ensemble, binSize);

                // Good Beam Ping
                AverageGoodBeamPing(ref ensemble, binSize);

                // Good Earth Ping
                AverageGoodEarthPing(ref ensemble, binSize);

                // Set Bin Size
                SetBinSize(ref ensemble, binSize);

                return true;
            }

            /// <summary>
            /// Average the beam velocity bins together.  This will average bins together to create a larger bin.
            /// 
            /// If the bin size given is equal or smaller than the current bin size, then nothing will be done and false will
            /// be returned.
            /// </summary>
            /// <param name="ensemble"></param>
            /// <param name="binSize"></param>
            /// <param name="setEnsData">Set the ensemble data values.</param>
            /// <returns>TRUE = average was done.</returns>
            public static bool AverageBeamVelocity(ref DataSet.Ensemble ensemble, float binSize, bool setEnsData = false)
            {
                if(ensemble.IsEnsembleAvail && ensemble.IsAncillaryAvail && ensemble.IsBeamVelocityAvail)
                {
                    // Get the current bin size
                    int avgNumBins = NumBins(ensemble.AncillaryData.BinSize, binSize);
                    if(avgNumBins <= 1)
                    {
                        // Verify more that 1 bin needs to be averaged together
                        // If 1 or less bins are averaged together, then nothing will happen
                        return false;
                    }

                    // Get number of beams
                    int numBeams = ensemble.BeamVelocityData.BeamVelocityData.GetLength(1);

                    int counter = 0;
                    float[] accumData = new float[numBeams];
                    int[] accumCounter = new int[numBeams];
                    List<float[]> avgData = new List<float[]>();
                    for(int bin = 0; bin < ensemble.BeamVelocityData.BeamVelocityData.GetLength(0); bin++)
                    {
                        // Accumulate the data
                        for (int beam = 0; beam < numBeams; beam++ )
                        {
                            // Check for bad velocity when averaging
                            if (ensemble.BeamVelocityData.BeamVelocityData[bin, beam] != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                accumData[beam] += ensemble.BeamVelocityData.BeamVelocityData[bin, beam];
                                accumCounter[beam]++;
                            }
                        }

                        // Increment the counter
                        counter++;

                        if(counter == avgNumBins)
                        {
                            // Average the accumulated data
                            float[] avgVel = new float[accumData.Length];
                            for (int x = 0; x < accumData.Length; x++ )
                            {
                                if (accumCounter[x] > 0)
                                {
                                    avgVel[x] = accumData[x] / accumCounter[x];
                                }
                                else
                                {
                                    avgVel[x] = DataSet.Ensemble.BAD_VELOCITY;
                                }
                            }

                            // Store the new bin of data
                            avgData.Add(avgVel);

                            // Clear the data
                            accumData = new float[ensemble.EnsembleData.NumBeams];
                            accumCounter = new int[ensemble.EnsembleData.NumBeams];
                            counter = 0;
                        }
                    }

                    // Create the new data
                    int newNumBins = avgData.Count;
                    float[,] newBinData = new float[newNumBins, numBeams];
                    for(int bin = 0; bin < newNumBins; bin++)
                    {
                        // Store the new data
                        for(int beam = 0; beam < numBeams; beam++)
                        {
                            newBinData[bin, beam] = avgData[bin][beam];
                        }
                    }

                    // Set the new data
                    ensemble.BeamVelocityData.BeamVelocityData = newBinData;
                    ensemble.BeamVelocityData.NumElements = newNumBins;

                    // Set the new bin size and number of bins
                    if (setEnsData)
                    {
                        // Set the new bin size and number of bins
                        ensemble.EnsembleData.NumBins = newNumBins;
                        ensemble.AncillaryData.BinSize = binSize;
                    }

                }
                else
                {
                    return false;
                }

                return true;
            }

            /// <summary>
            /// Average the instrument velocity bins together.  This will average bins together to create a larger bin.
            /// 
            /// If the bin size given is equal or smaller than the current bin size, then nothing will be done and false will
            /// be returned.
            /// </summary>
            /// <param name="ensemble"></param>
            /// <param name="binSize"></param>
            /// <param name="setEnsData">Set the ensemble data values.</param>
            /// <returns>TRUE = average was done.</returns>
            public static bool AverageInstrumentVelocity(ref DataSet.Ensemble ensemble, float binSize, bool setEnsData = false)
            {
                if (ensemble.IsEnsembleAvail && ensemble.IsAncillaryAvail && ensemble.IsInstrumentVelocityAvail)
                {
                    // Get the current bin size
                    int avgNumBins = NumBins(ensemble.AncillaryData.BinSize, binSize);
                    if (avgNumBins <= 1)
                    {
                        // Verify more that 1 bin needs to be averaged together
                        // If 1 or less bins are averaged together, then nothing will happen
                        return false;
                    }

                    // Get number of beams
                    int numBeams = ensemble.InstrumentVelocityData.InstrumentVelocityData.GetLength(1);

                    int counter = 0;
                    float[] accumData = new float[numBeams];
                    int[] accumCounter = new int[numBeams];
                    List<float[]> avgData = new List<float[]>();
                    for (int bin = 0; bin < ensemble.InstrumentVelocityData.InstrumentVelocityData.GetLength(0); bin++)
                    {
                        // Accumulate the data
                        for (int beam = 0; beam < numBeams; beam++)
                        {
                            // Check for bad velocity when averaging
                            if (ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, beam] != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                accumData[beam] += ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, beam];
                                accumCounter[beam]++;
                            }
                        }

                        // Increment the counter
                        counter++;

                        if (counter == avgNumBins)
                        {
                            // Average the accumulated data
                            float[] avgVel = new float[accumData.Length];
                            for (int x = 0; x < accumData.Length; x++)
                            {
                                if (accumCounter[x] > 0)
                                {
                                    avgVel[x] = accumData[x] / accumCounter[x];
                                }
                                else
                                {
                                    avgVel[x] = DataSet.Ensemble.BAD_VELOCITY;
                                }
                            }

                            // Store the new bin of data
                            avgData.Add(avgVel);

                            // Clear the data
                            accumData = new float[ensemble.EnsembleData.NumBeams];
                            accumCounter = new int[ensemble.EnsembleData.NumBeams];
                            counter = 0;
                        }
                    }

                    // Create the new data
                    int newNumBins = avgData.Count;
                    float[,] newBinData = new float[newNumBins, numBeams];
                    for (int bin = 0; bin < newNumBins; bin++)
                    {
                        // Store the new data
                        for (int beam = 0; beam < numBeams; beam++)
                        {
                            newBinData[bin, beam] = avgData[bin][beam];
                        }
                    }

                    // Set the new data
                    ensemble.InstrumentVelocityData.InstrumentVelocityData = newBinData;
                    ensemble.InstrumentVelocityData.NumElements = newNumBins;

                    // Set the new bin size and number of bins
                    if (setEnsData)
                    {
                        ensemble.EnsembleData.NumBins = newNumBins;
                        ensemble.AncillaryData.BinSize = binSize;
                    }

                }
                else
                {
                    return false;
                }

                return true;
            }

            /// <summary>
            /// Average the earth velocity bins together.  This will average bins together to create a larger bin.
            /// 
            /// If the bin size given is equal or smaller than the current bin size, then nothing will be done and false will
            /// be returned.
            /// </summary>
            /// <param name="ensemble"></param>
            /// <param name="binSize"></param>
            /// <param name="setEnsData">Set the ensemble data values.</param>
            /// <returns>TRUE = average was done.</returns>
            public static bool AverageEarthVelocity(ref DataSet.Ensemble ensemble, float binSize, bool setEnsData = false)
            {
                if (ensemble.IsEnsembleAvail && ensemble.IsAncillaryAvail && ensemble.IsEarthVelocityAvail)
                {
                    // Get the current bin size
                    int avgNumBins = NumBins(ensemble.AncillaryData.BinSize, binSize);
                    if (avgNumBins <= 1)
                    {
                        // Verify more that 1 bin needs to be averaged together
                        // If 1 or less bins are averaged together, then nothing will happen
                        return false;
                    }

                    // Get number of beams
                    int numBeams = ensemble.EarthVelocityData.EarthVelocityData.GetLength(1);

                    int counter = 0;
                    float[] accumData = new float[numBeams];
                    int[] accumCounter = new int[numBeams];
                    List<float[]> avgData = new List<float[]>();
                    for (int bin = 0; bin < ensemble.EarthVelocityData.EarthVelocityData.GetLength(0); bin++)
                    {
                        // Accumulate the data
                        for (int beam = 0; beam < numBeams; beam++)
                        {
                            // Check for bad velocity when averaging
                            if (ensemble.EarthVelocityData.EarthVelocityData[bin, beam] != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                accumData[beam] += ensemble.EarthVelocityData.EarthVelocityData[bin, beam];
                                accumCounter[beam]++;
                            }
                        }

                        // Increment the counter
                        counter++;

                        if (counter == avgNumBins)
                        {
                            // Average the accumulated data
                            float[] avgVel = new float[accumData.Length];
                            for (int x = 0; x < accumData.Length; x++)
                            {
                                if (accumCounter[x] > 0)
                                {
                                    avgVel[x] = accumData[x] / accumCounter[x];
                                }
                                else
                                {
                                    avgVel[x] = DataSet.Ensemble.BAD_VELOCITY;
                                }
                            }

                            // Store the new bin of data
                            avgData.Add(avgVel);

                            // Clear the data
                            accumData = new float[ensemble.EnsembleData.NumBeams];
                            accumCounter = new int[ensemble.EnsembleData.NumBeams];
                            counter = 0;
                        }
                    }

                    // Create the new data
                    int newNumBins = avgData.Count;
                    float[,] newBinData = new float[newNumBins, numBeams];
                    for (int bin = 0; bin < newNumBins; bin++)
                    {
                        // Store the new data
                        for (int beam = 0; beam < numBeams; beam++)
                        {
                            newBinData[bin, beam] = avgData[bin][beam];
                        }
                    }

                    // Set the new data
                    ensemble.EarthVelocityData.EarthVelocityData = newBinData;
                    ensemble.EarthVelocityData.NumElements = newNumBins;

                    // Set the new bin size and number of bins
                    if (setEnsData)
                    {
                        ensemble.EnsembleData.NumBins = newNumBins;
                        ensemble.AncillaryData.BinSize = binSize;
                    }

                }
                else
                {
                    return false;
                }

                return true;
            }

            /// <summary>
            /// Average the amplitude bins together.  This will average bins together to create a larger bin.
            /// 
            /// If the bin size given is equal or smaller than the current bin size, then nothing will be done and false will
            /// be returned.
            /// </summary>
            /// <param name="ensemble"></param>
            /// <param name="binSize"></param>
            /// <param name="setEnsData">Set the ensemble data values.</param>
            /// <returns>TRUE = average was done.</returns>
            public static bool AverageAmplitude(ref DataSet.Ensemble ensemble, float binSize, bool setEnsData = false)
            {
                if (ensemble.IsEnsembleAvail && ensemble.IsAncillaryAvail && ensemble.IsAmplitudeAvail)
                {
                    // Get the current bin size
                    int avgNumBins = NumBins(ensemble.AncillaryData.BinSize, binSize);
                    if (avgNumBins <= 1)
                    {
                        // Verify more that 1 bin needs to be averaged together
                        // If 1 or less bins are averaged together, then nothing will happen
                        return false;
                    }

                    // Get number of beams
                    int numBeams = ensemble.AmplitudeData.AmplitudeData.GetLength(1);

                    int counter = 0;
                    float[] accumData = new float[numBeams];
                    int[] accumCounter = new int[numBeams];
                    List<float[]> avgData = new List<float[]>();
                    for (int bin = 0; bin < ensemble.AmplitudeData.AmplitudeData.GetLength(0); bin++)
                    {
                        // Accumulate the data
                        for (int beam = 0; beam < numBeams; beam++)
                        {
                            accumData[beam] += ensemble.AmplitudeData.AmplitudeData[bin, beam];
                            accumCounter[beam]++;
                        }

                        // Increment the counter
                        counter++;

                        if (counter == avgNumBins)
                        {
                            // Average the accumulated data
                            float[] avgVel = new float[accumData.Length];
                            for (int x = 0; x < accumData.Length; x++)
                            {
                                if (accumCounter[x] > 0)
                                {
                                    avgVel[x] = accumData[x] / accumCounter[x];
                                }
                                else
                                {
                                    avgVel[x] = 0;
                                }

                            }

                            // Store the new bin of data
                            avgData.Add(avgVel);

                            // Clear the data
                            accumData = new float[ensemble.EnsembleData.NumBeams];
                            accumCounter = new int[ensemble.EnsembleData.NumBeams];
                            counter = 0;
                        }
                    }

                    // Create the new data
                    int newNumBins = avgData.Count;
                    float[,] newBinData = new float[newNumBins, numBeams];
                    for (int bin = 0; bin < newNumBins; bin++)
                    {
                        // Store the new data
                        for (int beam = 0; beam < numBeams; beam++)
                        {
                            newBinData[bin, beam] = avgData[bin][beam];
                        }
                    }

                    // Set the new data
                    ensemble.AmplitudeData.AmplitudeData = newBinData;
                    ensemble.AmplitudeData.NumElements = newNumBins;

                    // Set the new bin size and number of bins
                    if (setEnsData)
                    {
                        ensemble.EnsembleData.NumBins = newNumBins;
                        ensemble.AncillaryData.BinSize = binSize;
                    }

                }
                else
                {
                    return false;
                }

                return true;
            }

            /// <summary>
            /// Average the correlation bins together.  This will average bins together to create a larger bin.
            /// 
            /// If the bin size given is equal or smaller than the current bin size, then nothing will be done and false will
            /// be returned.
            /// </summary>
            /// <param name="ensemble"></param>
            /// <param name="binSize"></param>
            /// <param name="setEnsData">Set the ensemble data values.</param>
            /// <returns>TRUE = average was done.</returns>
            public static bool AverageCorrelation(ref DataSet.Ensemble ensemble, float binSize, bool setEnsData = false)
            {
                if (ensemble.IsEnsembleAvail && ensemble.IsAncillaryAvail && ensemble.IsCorrelationAvail)
                {
                    // Get the current bin size
                    int avgNumBins = NumBins(ensemble.AncillaryData.BinSize, binSize);
                    if (avgNumBins <= 1)
                    {
                        // Verify more that 1 bin needs to be averaged together
                        // If 1 or less bins are averaged together, then nothing will happen
                        return false;
                    }

                    // Get number of beams
                    int numBeams = ensemble.CorrelationData.CorrelationData.GetLength(1);

                    int counter = 0;
                    float[] accumData = new float[numBeams];
                    int[] accumCounter = new int[numBeams];
                    List<float[]> avgData = new List<float[]>();
                    for (int bin = 0; bin < ensemble.CorrelationData.CorrelationData.GetLength(0); bin++)
                    {
                        // Accumulate the data
                        for (int beam = 0; beam < numBeams; beam++)
                        {
                            accumData[beam] += ensemble.CorrelationData.CorrelationData[bin, beam];
                            accumCounter[beam]++;
                        }

                        // Increment the counter
                        counter++;

                        if (counter == avgNumBins)
                        {
                            // Average the accumulated data
                            float[] avgVel = new float[accumData.Length];
                            for (int x = 0; x < accumData.Length; x++)
                            {
                                if (accumCounter[x] > 0)
                                {
                                    avgVel[x] = accumData[x] / accumCounter[x];
                                }
                                else
                                {
                                    avgVel[x] = 0;
                                }
                            }

                            // Store the new bin of data
                            avgData.Add(avgVel);

                            // Clear the data
                            accumData = new float[ensemble.EnsembleData.NumBeams];
                            accumCounter = new int[ensemble.EnsembleData.NumBeams];
                            counter = 0;
                        }
                    }

                    // Create the new data
                    int newNumBins = avgData.Count;
                    float[,] newBinData = new float[newNumBins, numBeams];
                    for (int bin = 0; bin < newNumBins; bin++)
                    {
                        // Store the new data
                        for (int beam = 0; beam < numBeams; beam++)
                        {
                            newBinData[bin, beam] = avgData[bin][beam];
                        }
                    }

                    // Set the new data
                    ensemble.CorrelationData.CorrelationData = newBinData;
                    ensemble.CorrelationData.NumElements = newNumBins;

                    // Set the new bin size and number of bins
                    if (setEnsData)
                    {
                        ensemble.EnsembleData.NumBins = newNumBins;
                        ensemble.AncillaryData.BinSize = binSize;
                    }

                }
                else
                {
                    return false;
                }

                return true;
            }

            /// <summary>
            /// Average the good beam pings bins together.  This will average bins together to create a larger bin.
            /// 
            /// If the bin size given is equal or smaller than the current bin size, then nothing will be done and false will
            /// be returned.
            /// </summary>
            /// <param name="ensemble"></param>
            /// <param name="binSize"></param>
            /// <param name="setEnsData">Set the ensemble data values.</param>
            /// <returns>TRUE = average was done.</returns>
            public static bool AverageGoodBeamPing(ref DataSet.Ensemble ensemble, float binSize, bool setEnsData = false)
            {
                if (ensemble.IsEnsembleAvail && ensemble.IsAncillaryAvail && ensemble.IsGoodBeamAvail)
                {
                    // Get the current bin size
                    int avgNumBins = NumBins(ensemble.AncillaryData.BinSize, binSize);
                    if (avgNumBins <= 1)
                    {
                        // Verify more that 1 bin needs to be averaged together
                        // If 1 or less bins are averaged together, then nothing will happen
                        return false;
                    }

                    // Get number of beams
                    int numBeams = ensemble.GoodBeamData.GoodBeamData.GetLength(1);

                    int counter = 0;
                    int[] accumData = new int[numBeams];
                    int[] accumCounter = new int[numBeams];
                    List<int[]> avgData = new List<int[]>();
                    for (int bin = 0; bin < ensemble.GoodBeamData.GoodBeamData.GetLength(0); bin++)
                    {
                        // Accumulate the data
                        for (int beam = 0; beam < numBeams; beam++)
                        {
                            accumData[beam] += ensemble.GoodBeamData.GoodBeamData[bin, beam];
                            accumCounter[beam]++;
                        }

                        // Increment the counter
                        counter++;

                        if (counter == avgNumBins)
                        {
                            // Average the accumulated data
                            int[] avgVel = new int[accumData.Length];
                            for (int x = 0; x < accumData.Length; x++)
                            {
                                if (accumCounter[x] > 0)
                                {
                                    avgVel[x] = accumData[x] / accumCounter[x];
                                }
                                else
                                {
                                    avgVel[x] = 0;
                                }
                            }

                            // Store the new bin of data
                            avgData.Add(avgVel);

                            // Clear the data
                            accumData = new int[ensemble.EnsembleData.NumBeams];
                            accumCounter = new int[ensemble.EnsembleData.NumBeams];
                            counter = 0;
                        }
                    }

                    // Create the new data
                    int newNumBins = avgData.Count;
                    int[,] newBinData = new int[newNumBins, numBeams];
                    for (int bin = 0; bin < newNumBins; bin++)
                    {
                        // Store the new data
                        for (int beam = 0; beam < numBeams; beam++)
                        {
                            newBinData[bin, beam] = avgData[bin][beam];
                        }
                    }

                    // Set the new data
                    ensemble.GoodBeamData.GoodBeamData = newBinData;
                    ensemble.GoodBeamData.NumElements = newNumBins;

                    // Set the new bin size and number of bins
                    if (setEnsData)
                    {
                        ensemble.EnsembleData.NumBins = newNumBins;
                        ensemble.AncillaryData.BinSize = binSize;
                    }

                }
                else
                {
                    return false;
                }

                return true;
            }

            /// <summary>
            /// Average the good earth pings bins together.  This will average bins together to create a larger bin.
            /// 
            /// If the bin size given is equal or smaller than the current bin size, then nothing will be done and false will
            /// be returned.
            /// </summary>
            /// <param name="ensemble"></param>
            /// <param name="binSize"></param>
            /// <param name="setEnsData">Set the ensemble data values.</param>
            /// <returns>TRUE = average was done.</returns>
            public static bool AverageGoodEarthPing(ref DataSet.Ensemble ensemble, float binSize, bool setEnsData = false)
            {
                if (ensemble.IsEnsembleAvail && ensemble.IsAncillaryAvail && ensemble.IsGoodEarthAvail)
                {
                    // Get the current bin size
                    int avgNumBins = NumBins(ensemble.AncillaryData.BinSize, binSize);
                    if (avgNumBins <= 1)
                    {
                        // Verify more that 1 bin needs to be averaged together
                        // If 1 or less bins are averaged together, then nothing will happen
                        return false;
                    }

                    // Get number of beams
                    int numBeams = ensemble.GoodEarthData.GoodEarthData.GetLength(1);

                    int counter = 0;
                    int[] accumData = new int[numBeams];
                    int[] accumCounter = new int[numBeams];
                    List<int[]> avgData = new List<int[]>();
                    for (int bin = 0; bin < ensemble.GoodEarthData.GoodEarthData.GetLength(0); bin++)
                    {
                        // Accumulate the data
                        for (int beam = 0; beam < numBeams; beam++)
                        {
                            accumData[beam] += ensemble.GoodEarthData.GoodEarthData[bin, beam];
                            accumCounter[beam]++;
                        }

                        // Increment the counter
                        counter++;

                        if (counter == avgNumBins)
                        {
                            // Average the accumulated data
                            int[] avgVel = new int[accumData.Length];
                            for (int x = 0; x < accumData.Length; x++)
                            {
                                if(accumCounter[x] > 0)
                                { 
                                    avgVel[x] = accumData[x] / accumCounter[x];
                                }
                                else
                                {
                                    avgVel[x] = 0;
                                }
                            }

                            // Store the new bin of data
                            avgData.Add(avgVel);

                            // Clear the data
                            accumData = new int[ensemble.EnsembleData.NumBeams];
                            accumCounter = new int[ensemble.EnsembleData.NumBeams];
                            counter = 0;
                        }
                    }

                    // Create the new data
                    int newNumBins = avgData.Count;
                    int[,] newBinData = new int[newNumBins, numBeams];
                    for (int bin = 0; bin < newNumBins; bin++)
                    {
                        // Store the new data
                        for (int beam = 0; beam < numBeams; beam++)
                        {
                            newBinData[bin, beam] = avgData[bin][beam];
                        }
                    }

                    // Set the new data
                    ensemble.GoodEarthData.GoodEarthData = newBinData;
                    ensemble.GoodEarthData.NumElements = newNumBins;

                    // Set the new bin size and number of bins
                    if (setEnsData)
                    {
                        ensemble.EnsembleData.NumBins = newNumBins;
                        ensemble.AncillaryData.BinSize = binSize;
                    }

                }
                else
                {
                    return false;
                }

                return true;
            }

            /// <summary>
            /// Determine the number of bins that need to be averaged together
            /// to get the new bin size.  This will round the number.
            /// 
            /// </summary>
            /// <param name="origBinSize">Original bin size.</param>
            /// <param name="newBinSize">New bin size.</param>
            /// <returns>Number of bins that need to be averaged together.</returns>
            private static int NumBins(float origBinSize, float newBinSize)
            {
                return (int)Math.Round(newBinSize / origBinSize);
            }

            /// <summary>
            /// Set the bin size to the ensemble.  This will set the ancillary and ensemble data.
            /// </summary>
            /// <param name="ensemble">Ensemble to set.</param>
            /// <param name="newBinSize">New Bin Size.</param>
            public static void SetBinSize(ref DataSet.Ensemble ensemble, float newBinSize)
            {
                if (ensemble.IsEnsembleAvail && ensemble.IsAncillaryAvail)
                {
                    // Get the current bin size
                    int avgNumBins = NumBins(ensemble.AncillaryData.BinSize, newBinSize);

                    if (avgNumBins > 0)
                    {
                        // Set the new bin size and number of bins
                        ensemble.EnsembleData.NumBins = ensemble.EnsembleData.NumBins / avgNumBins;
                        ensemble.AncillaryData.BinSize = newBinSize;
                    }
                }
            }

        }

    }
}

