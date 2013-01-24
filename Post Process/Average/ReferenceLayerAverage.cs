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
 * 01/17/2012      RC          1.14       Initial coding
 * 02/16/2012      RC          2.03       Added Min and Max layer.
 *                                         Added Event for averaged ensemble.
 *                                         Average the data using running or every X samples.
 * 02/29/2012      RC          2.04       Added try/catch in AverageEnsembles() to prevent exception when jumping around in ensembles.
 *                                         Update FirstPingTime with first ensemble in average when averaging.
 * 03/06/2012      RC          2.05       Fixed bug in AverageEnsemblesAccum() where i check for a bad node.
 * 
 */


using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Diagnostics;
namespace RTI
{

    namespace Average
    {
        /// <summary>
        /// Reference Layer Averaging.
        /// Choose a reference layer.  A reference layer is a layer 
        /// in the ensemble made up of 1 or more bins where you believe 
        /// the value measured can be trusted.  There should not be any 
        /// known issues within the layer.  Issues can include wind variation or
        /// depth changing or ...
        /// 
        /// We will then subtract out the reference layer speed from every bin
        /// in the ensemble.  
        /// (Ens1 - RefLayer1)
        /// 
        /// We then accumulate ensembles with the reference layer subtracted out and
        /// the reference layer speed.
        /// (Ens1 - RefLayer1) --> AccumEns
        /// RefLayer1          --> AccumRefLayer
        /// 
        /// After X number of ensembles have been accummulated, we take an average of
        /// the ensembles.  To calculate the average we will take the average of the 
        /// accumulated reference layer speed and add it to the average of the ensembles.
        /// 
        /// To calculate the average ensemble, we will average the speed of each bin together.
        /// (AccumRefLayer/X) + (AccumEns/X)
        /// 
        /// Reference layer averaging will not only average ensembles together it will also
        /// fill in data where it is missing between ensembles.  If one ensemble only got good
        /// data from bins 1 through 12 and the second ensemble got good data from bins 1 through
        /// 23, the average of the two ensembles would fill in some data for the missing data in 
        /// ensemble one bin's 13 - 23.  With more samples, the data would look cleaner for the
        /// missing data.
        /// </summary>
        public class ReferenceLayerAverage : AverageBase
        {
            #region Variables

            /// <summary>
            /// Number of elements in the reference layer average.
            /// This is North, East and Vertical velocity.  The
            /// error velocity is left alone.
            /// </summary>
            private const int NUM_ELEMENTS = 3;

            /// <summary>
            /// List to hold the accumulating ensembles to average
            /// together.  These ensembles will have the reference
            /// layer average removed already.
            /// </summary>
            private LinkedList<DataSet.Ensemble> _accumEns;

            /// <summary>
            /// List to hold the accumulating reference layer averages.
            /// </summary>
            private LinkedList<float[]> _accumRefLayerAvg;

            /// <summary>
            /// Minimum reference layer bin.
            /// The bin starts with bin 0.
            /// </summary>
            private int _minRefLayerBin;

            /// <summary>
            /// Maximum reference layer bin.
            /// </summary>
            private int _maxRefLayerBin;

            #endregion

            /// <summary>
            /// Constructor:
            /// Average the ensembles using reference layer.
            /// Give the number of samples before averaging.
            /// Set the reference layer later.
            /// </summary>
            /// <param name="numSamples">Number of samples to average.</param>
            /// <param name="minRefLayerBin">Reference layer minimum bin.</param>
            /// <param name="maxRefLayerBin">Reference layer maximum bin.</param>
            /// <param name="runningAvg">Flag if a running average.</param>
            public ReferenceLayerAverage(int numSamples, int minRefLayerBin, int maxRefLayerBin, bool runningAvg)
            {
                // Initialize values
                // Validate number of samples
                _numSamples = numSamples;

                SetMinMaxReferenceLayer(minRefLayerBin, maxRefLayerBin);
                IsRunningAverage = runningAvg;

                // Initialize the list
                _accumEns = new LinkedList<DataSet.Ensemble>();
                _accumRefLayerAvg = new LinkedList<float[]>();
            }

            /// <summary>
            /// Add an ensemble to the accumulator.
            /// Calculate the reference layer and add it
            /// to the accumulator.
            /// </summary>
            /// <param name="ensemble">Ensemble to add to average.</param>
            public override void AddEnsemble(DataSet.Ensemble ensemble)
            {
                // Create worker to do the work
                //BackgroundWorker worker = new BackgroundWorker();
                //worker.DoWork += delegate(object s, DoWorkEventArgs args)
                //{
                    // Ensure data exist to take an average
                    if (ensemble.IsEarthVelocityAvail && ensemble.IsEnsembleAvail)
                    {
                        // Clone the ensemble
                        // Modification will be made to the ensemble,
                        // so the ensemble is cloned.
                        DataSet.Ensemble cloneEnsemble = ensemble.Clone();

                        // Get the reference layer average
                        float[] refLayerAvg = GetReferenceLayerAverage(cloneEnsemble);

                        // Remove the reference layer average from all the bins
                        RemoveRefLayerAvgFromEnsemble(refLayerAvg, cloneEnsemble);

                        // Add the ensemble to the accum
                        _accumEns.AddLast(cloneEnsemble);
                        _accumRefLayerAvg.AddLast(refLayerAvg);

                        // Check the accum size to see if time to average data
                        if (_accumEns.Count > _numSamples)
                        {
                            // UNCOMMENT THIS IF YOU ARE NOT USING THE AVERAGER MANAGER
                            // THIS WILL AUTOMATICALLY PUBLISH THE AVERAGED ENSEMBLE WHEN
                            // THE NUMBER OF SAMPLES HAS BEEN MET
                            // Average the ensembles in the accumulator
                            //AverageEnsembles();

                            // If running average, remove the first ensemble
                            // If not running average, remove all the ensembles
                            RemoveEnsemble();
                        }
                    }
                //};
                //worker.RunWorkerAsync();
            }

            /// <summary>
            /// Clear the accumulators.
            /// If the averaging needs to be restarted,
            /// then clear the accumulators of data.
            /// </summary>
            public override void Clear()
            {
                if (_accumEns != null && _accumRefLayerAvg != null)
                {
                    // Clear the accumulators
                    _accumEns.Clear();
                    _accumRefLayerAvg.Clear();
                }
            }

            /// <summary>
            /// This will validate and set the minimum and maximum
            /// reference layer.  Because we do not know what the number
            /// of bins in the ensemble will be, the max may be set larger
            /// then an ensemble size.  The max must be check against the
            /// ensemble when using the max.  Use Math.Min(ensemble.EnsembleData.NumBins, _maxRefLayerBin);
            /// when using the max reference layer.
            /// 
            /// This will always assume the minRefLayer is correct and make
            /// corrections to the maxRefLayer based off the minRefLayer.
            /// 
            /// The values can be equal to each other.  If they are equal,
            /// then only one bin will be used as the reference layer.
            /// 
            /// Once the reference layers are set, the accumulators are cleared.
            /// By setting new reference layers, the previous reference layers will
            /// not work.  The previous reference layers were used to remove data
            /// from the accumulated ensembles.  So everything must start over.
            /// </summary>
            /// <param name="minRefLayer">Minimum Reference layer.</param>
            /// <param name="maxRefLayer">Maximum Reference layer.</param>
            public void SetMinMaxReferenceLayer(int minRefLayer, int maxRefLayer)
            {
                // Check for less than 0
                if (minRefLayer < 0)
                {
                    minRefLayer = 0;
                }
                if (maxRefLayer < 0)
                {
                    maxRefLayer = 0;
                }

                // Verify min is less than max
                if (minRefLayer > maxRefLayer)
                {
                    maxRefLayer = minRefLayer + 1;                 // Make the value 1 greater then minRefLayer
                }

                // Set the value
                _minRefLayerBin = minRefLayer;
                _maxRefLayerBin = maxRefLayer;

                // Clear everything, because a new reference layer is used
                Clear();
            }

            /// <summary>
            /// Average all the accumulated data.  Then set the averaged data
            /// to the ensemble.  The given ensemble should be the last ensemble accumulated,
            /// so the last time and date for the accumulated is given in the ensemble.
            /// </summary>
            /// <param name="ensemble">Ensemble to add the averaged data to.</param>
            public override void SetAverage(ref DataSet.Ensemble ensemble)
            {
                float[,] avgData = GetAverage();

                // If the ensemble is not null
                // and an average could be taken
                // and the ensemble contains earth velocity dataset
                // Set the average
                if(ensemble != null && avgData != null && ensemble.IsEarthVelocityAvail )
                {
                    ensemble.EarthVelocityData.EarthVelocityData = avgData;
                }

                // If running average, remove the first ensemble
                // If not running average, remove all the ensembles
                RemoveEnsemble();
            }

            /// <summary>
            /// Calculate the average data and return the averaged 
            /// data.  The average data is reference layer Earth Velocity data.
            /// The data is in an array [bin, beam].  If an average could not
            /// be calculated, NULL will be returned.
            /// </summary>
            /// <returns>Reference Layer Averaged Earth Velocity data or NULL.</returns>
            public override float[,] GetAverage()
            {
                try
                {
                    // Ensure data has been accumulated
                    if (_accumEns.Count > 0)
                    {
                        // Get the current last node
                        // This will take the ensembles between the first and
                        // last node in the list.  The last node is determine now 
                        // at the start of this method, so if more ensembles are
                        // added to the list, it will not cause issues.
                        LinkedListNode<DataSet.Ensemble> lastEnsembleNode = _accumEns.Last;
                        LinkedListNode<float[]> lastRefLayerNode = _accumRefLayerAvg.Last;

                        // Clone the last ensemble, this will get us the lastest settings
                        // We will then set the time and date to the ensemble for the time
                        // the data was averaged.  Set the number of ensembles averaged.
                        //DataSet.Ensemble avgEnsemble = CreateAverageEnsemble(lastEnsembleNode.Value);
                        DataSet.Ensemble avgEnsemble = lastEnsembleNode.Value.Clone();

                        // Average the ensembles in the accumulator
                        AverageEnsemblesAccum(ref avgEnsemble, lastEnsembleNode);

                        // Average the reference layers velocities in the accumulator
                        float[] avgRefLayer = AverageRefLayerAccum(lastRefLayerNode);

                        // Add in the reference layer average
                        AddRefLayerAvgToEnsemble(ref avgEnsemble, avgRefLayer);

                        return avgEnsemble.EarthVelocityData.EarthVelocityData;
                    }
                }
                catch (NullReferenceException)
                {
                    // This exception occurs when clearing the list while averaging
                    // or make a large jump in ensembles while averaging
                }
                catch (Exception)
                {

                }

                return null;
            }

            #region Private Methods

            /// <summary>
            /// Take the average for the East, North and Vertical values of
            /// the bins set in the min and max reference layer.  
            /// 
            /// The max reference layer may be out of the range of the ensemble
            /// so use the smaller of the two values, the maxRefLayer or number of
            /// bins.
            /// 
            /// Return an array of the averaged [East, North, Vertical] values.
            /// 
            /// If the velocity is bad, do not include it in the average.
            /// </summary>
            /// <param name="ensemble">Ensemble to get the reference layer.</param>
            /// <returns>Reference layer average.</returns>
            private float[] GetReferenceLayerAverage(DataSet.Ensemble ensemble)
            {
                float[] refLayerAvg = new float[NUM_ELEMENTS];

                // Subtract 1 from NumBins because the bin starts at 0
                int maxRefLayer = Math.Min(ensemble.EnsembleData.NumBins - 1, _maxRefLayerBin);
                int minRefLayer = _minRefLayerBin;
                
                int samplesEast = 0;
                int samplesNorth = 0;
                int samplesVert = 0;

                // Accumulate the reference layer values
                for (int bin = minRefLayer; bin <= maxRefLayer; bin++)
                {
                    // Ensure the velocity is good
                    // East
                    if (ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                    {
                        refLayerAvg[DataSet.Ensemble.BEAM_EAST_INDEX] += ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX];
                        samplesEast++;
                    }

                    // North
                    if (ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                    {
                        refLayerAvg[DataSet.Ensemble.BEAM_NORTH_INDEX] += ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX];
                        samplesNorth++;
                    }

                    // Vertical
                    if (ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                    {
                        refLayerAvg[DataSet.Ensemble.BEAM_VERTICAL_INDEX] += ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX];
                        samplesVert++;
                    }
                }

                // Take the average of the accumulated values
                // The sample size is zero, the average will be 0
                // East
                if (samplesEast > 0)
                {
                    refLayerAvg[DataSet.Ensemble.BEAM_EAST_INDEX] /= samplesEast;
                }
                else
                {
                    refLayerAvg[DataSet.Ensemble.BEAM_EAST_INDEX] = 0;
                }

                // North
                if (samplesNorth > 0)
                {
                    refLayerAvg[DataSet.Ensemble.BEAM_NORTH_INDEX] /= samplesNorth;
                }
                else
                {
                    refLayerAvg[DataSet.Ensemble.BEAM_NORTH_INDEX] = 0;
                }

                // Vertical
                if (samplesVert > 0)
                {
                    refLayerAvg[DataSet.Ensemble.BEAM_VERTICAL_INDEX] /= samplesVert;
                }
                else
                {
                    refLayerAvg[DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0;
                }

                return refLayerAvg;
            }

            /// <summary>
            /// Remove the reference layer average value from each bin
            /// in the ensemble.  This will take each value and subtract
            /// the reference layer value.
            /// [BIN1,EAST] - [RefLayAvg,EAST]
            /// [BIN1,NORTH] - [RefLayAvg,NORTH]
            /// [BIN1,VERT] - [RefLayAvg,VERT]
            /// [BIN2,EAST] - [RefLayAvg,EAST]
            /// ...
            /// 
            /// If the velocity is bad, leave it bad
            /// </summary>
            /// <param name="refLayerAvg">Reference layer average.</param>
            /// <param name="ensemble">Ensemble containing the data.</param>
            private void RemoveRefLayerAvgFromEnsemble(float[] refLayerAvg, DataSet.Ensemble ensemble)
            {
                for(int bin = 0; bin < ensemble.EnsembleData.NumBins; bin++)
                {
                    // If the velocity is bad, leave it bad
                    if (ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                    {
                        ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] -= refLayerAvg[DataSet.Ensemble.BEAM_EAST_INDEX];
                    }

                    if (ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                    {
                        ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] -= refLayerAvg[DataSet.Ensemble.BEAM_NORTH_INDEX];
                    }

                    if (ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                    {
                        ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] -= refLayerAvg[DataSet.Ensemble.BEAM_VERTICAL_INDEX];
                    }
                }
            }

            /// <summary>
            /// Take each ensemble and reference layer average from the accumulator
            /// and average there values.
            /// Average all the East, North and Vertical velocities in the reference layer averages
            /// and in the ensembles from the accumulator.
            /// 
            /// (Ens1[B1,EAST] + Ens2[B1,EAST] + ... + EnsN[B1,EAST]) / N      = AvgEns[B1, EAST]
            /// (Ens1[B1,NORTH] + Ens2[B1,NORTH] + ... + EnsN[B1,NORTH]) / N   = AvgEns[B1, NORTH]
            /// (Ens1[B1,VERT] + Ens2[B1,VERT] + ... + EnsN[B1,VERT]) / N      = AvgEns[B1, VERT]
            /// (Ens1[B2,EAST] + Ens2[B2,EAST] + ... + EnsN[B2,EAST]) / N      = AvgEns[B2, EAST]
            /// ...
            /// 
            /// (Ref1[EAST] + Ref2[EAST] + ... + RefN[EAST]) / N               = AvgRef[EAST]
            /// (Ref1[EAST] + Ref2[EAST] + ... + RefN[EAST]) / N               = AvgRef[NORTH]
            /// (Ref1[EAST] + Ref2[EAST] + ... + RefN[EAST]) / N               = AvgRef[VERT]
            /// 
            /// Then add the averaged ensemble East value to the Reference layer averaged East value.
            /// AvgEns[B1, EAST] + AvgRef[EAST]     = AvgEns[B1, EAST]
            /// AvgEns[B1, NORTH] + AvgRef[NORTH]     = AvgEns[B1, NORTH]
            /// AvgEns[B1, VERT] + AvgRef[VERT]     = AvgEns[B1, VERT]
            /// AvgEns[B2, EAST] + AvgRef[EAST]     = AvgEns[B2, EAST]
            /// ...
            /// 
            /// </summary>
            private void AverageEnsembles()
            {
                try
                {
                    // Get the current last node
                    // This will take the ensembles between the first and
                    // last node in the list.  The last node is determine now 
                    // at the start of this method, so if more ensembles are
                    // added to the list, it will not cause issues.
                    LinkedListNode<DataSet.Ensemble> lastEnsembleNode = _accumEns.Last;
                    LinkedListNode<float[]> lastRefLayerNode = _accumRefLayerAvg.Last;

                    // Clone the last ensemble, this will get us the lastest settings
                    // We will then set the time and date to the ensemble for the time
                    // the data was averaged.  Set the number of ensembles averaged.
                    //DataSet.Ensemble avgEnsemble = CreateAverageEnsemble(lastEnsembleNode.Value);
                    DataSet.Ensemble avgEnsemble = lastEnsembleNode.Value.Clone();

                    // Average the ensembles in the accumulator
                    AverageEnsemblesAccum(ref avgEnsemble, lastEnsembleNode);

                    // Average the reference layers velocities in the accumulator
                    float[] avgRefLayer = AverageRefLayerAccum(lastRefLayerNode);

                    // Add in the reference layer average
                    AddRefLayerAvgToEnsemble(ref avgEnsemble, avgRefLayer);

                    // Publish the average ensemble to all subscribers
                    PublishAveragedEnsemble(avgEnsemble);
                }
                catch (NullReferenceException)
                {
                    // This exception occurs when clearing the list while averaging
                    // or make a large jump in ensembles while averaging
                }
                catch (Exception)
                {

                }
            }

            /// <summary>
            /// Add the reference layer average to the earth velocity value.
            /// 
            /// If the velocity is bad, leave it bad.
            /// </summary>
            /// <param name="ensemble">Ensemble containing average values.</param>
            /// <param name="avgRefLayer">Reference Layer average.</param>
            private void AddRefLayerAvgToEnsemble(ref DataSet.Ensemble ensemble, float[] avgRefLayer)
            {
                // Add all the reference layer averages to the ensemble Earth Velocity
                for (int bin = 0; bin < ensemble.EnsembleData.NumBins; bin++)
                {
                    // If the velocity is bad, leave it
                    // East
                    if (ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                    {
                        ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] += avgRefLayer[DataSet.Ensemble.BEAM_EAST_INDEX];
                    }

                    // North
                    if (ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                    {
                        ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] += avgRefLayer[DataSet.Ensemble.BEAM_NORTH_INDEX];
                    }

                    // Vertical
                    if (ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                    {
                        ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] += avgRefLayer[DataSet.Ensemble.BEAM_VERTICAL_INDEX];
                    }
                }
            }

            /// <summary>
            /// Average together all the reference layer averages in the accumulator.
            /// 
            /// The reference layer averages should not contain any BAD_VELOCITY.  They were
            /// filtered out when calculating the reference layer average.
            /// </summary>
            /// <param name="lastRefLayerNode">Last node in the accumulator to average.</param>
            /// <returns>Array containing the averaged reference layer velocities.</returns>
            private float[] AverageRefLayerAccum(LinkedListNode<float[]> lastRefLayerNode)
            {
                // Reference layer average
                float[] refLayerAvg = new float[NUM_ELEMENTS];

                // Set the last values to the array
                refLayerAvg[DataSet.Ensemble.BEAM_EAST_INDEX] = lastRefLayerNode.Value[DataSet.Ensemble.BEAM_EAST_INDEX];
                refLayerAvg[DataSet.Ensemble.BEAM_NORTH_INDEX] = lastRefLayerNode.Value[DataSet.Ensemble.BEAM_NORTH_INDEX];
                refLayerAvg[DataSet.Ensemble.BEAM_VERTICAL_INDEX] = lastRefLayerNode.Value[DataSet.Ensemble.BEAM_VERTICAL_INDEX];

                // Average the ensemble
                LinkedListNode<float[]> nodeRefLayer = _accumRefLayerAvg.First;
                do
                {
                    float[] refLayerAvgAccum = nodeRefLayer.Value;

                    refLayerAvg[DataSet.Ensemble.BEAM_EAST_INDEX] += refLayerAvgAccum[DataSet.Ensemble.BEAM_EAST_INDEX];
                    refLayerAvg[DataSet.Ensemble.BEAM_NORTH_INDEX] += refLayerAvgAccum[DataSet.Ensemble.BEAM_NORTH_INDEX];
                    refLayerAvg[DataSet.Ensemble.BEAM_VERTICAL_INDEX] += refLayerAvgAccum[DataSet.Ensemble.BEAM_VERTICAL_INDEX];


                    // Move to the next reference layer average in the list
                    nodeRefLayer = nodeRefLayer.Next;

                } while (nodeRefLayer != lastRefLayerNode);

                refLayerAvg[DataSet.Ensemble.BEAM_EAST_INDEX] /= _numSamples;
                refLayerAvg[DataSet.Ensemble.BEAM_NORTH_INDEX] /= _numSamples;
                refLayerAvg[DataSet.Ensemble.BEAM_VERTICAL_INDEX] /= _numSamples;

                return refLayerAvg;
            }

            /// <summary>
            /// Average all the ensembles in the accumulator.  All the ensembles
            /// have the reference layer average velocity removed.  Average
            /// all the values in the Earth Velocity Data Set.
            /// </summary>
            /// <param name="avgEnsemble">Average Ensemble.</param>
            /// <param name="lastEnsembleNode">Last node to average.</param>
            private void AverageEnsemblesAccum(ref DataSet.Ensemble avgEnsemble, LinkedListNode<DataSet.Ensemble> lastEnsembleNode)
            {
                int[,] samples = new int[avgEnsemble.EnsembleData.NumBins, NUM_ELEMENTS]; 

                // Set the last ensemble data to the average ensemble
                // Only use the value if its a good velocity
                for (int bin = 0; bin < avgEnsemble.EnsembleData.NumBins; bin++)
                {
                    // Check if the value is good
                    // If it is not good, set it to 0
                    // East
                    if(avgEnsemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                    {
                        samples[bin, DataSet.Ensemble.BEAM_EAST_INDEX]++;
                    }
                    else
                    {
                        // Set the value to 0 to ensure it will not screw up the average
                        avgEnsemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] = 0;
                    }
                        
                    // North
                    if(avgEnsemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                    {
                        samples[bin, DataSet.Ensemble.BEAM_NORTH_INDEX]++;
                    }
                    else
                    {
                        // Set the value to 0 to ensure it will not screw up the average
                        avgEnsemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] = 0;
                    }

                    // Vertical
                    if(avgEnsemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                    {
                        samples[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX]++;
                    }
                    else
                    {
                        // Set the value to 0 to ensure it will not screw up the average
                        avgEnsemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 0;
                    }
                }


                // Average the ensemble
                LinkedListNode<DataSet.Ensemble> nodeEns = _accumEns.First;
                do
                {
                    // Bad node
                    // List changed and the node are no longer valid
                    // Caused when clearing the list or large jumps
                    if (nodeEns == null)
                    {
                        break;
                    }

                    DataSet.Ensemble ensemble = nodeEns.Value;

                    // Add the bin data to the avgEnsemble
                    // avgEnsemble already contains the last ensembles data
                    for (int bin = 0; bin < ensemble.EnsembleData.NumBins; bin++)
                    {
                        // Check if the velocity is bad, if its bad, do not include it in the average
                        // East
                        if (ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            avgEnsemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] += ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX];
                            samples[bin, DataSet.Ensemble.BEAM_EAST_INDEX]++;
                        }

                        // North
                        if (ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            avgEnsemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] += ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX];
                            samples[bin, DataSet.Ensemble.BEAM_NORTH_INDEX]++;
                        }

                        // Vertical
                        if (ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            avgEnsemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] += ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX];
                            samples[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX]++;
                        }
                    }

                    // Move to the next ensemble in the list
                    nodeEns = nodeEns.Next;

                } while (nodeEns != lastEnsembleNode.Next);
                

                // Average the values
                // If no samples were taken,
                // the velocity was bad, so set it bad
                for (int bin = 0; bin < avgEnsemble.EnsembleData.NumBins; bin++)
                {
                    // East Average
                    if (samples[bin, DataSet.Ensemble.BEAM_EAST_INDEX] > 0)
                    {
                        avgEnsemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] /= samples[bin, DataSet.Ensemble.BEAM_EAST_INDEX];
                    }
                    else
                    {
                        avgEnsemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
                    }

                    // North Average
                    if (samples[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] > 0)
                    {
                        avgEnsemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] /= samples[bin, DataSet.Ensemble.BEAM_NORTH_INDEX];
                    }
                    else
                    {
                        avgEnsemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
                    }

                    // Vertical Average
                    if (samples[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] > 0)
                    {
                        avgEnsemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] /= samples[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX];
                    }
                    else
                    {
                        avgEnsemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = DataSet.Ensemble.BAD_VELOCITY;
                    }
                }

            }

            ///// <summary>
            ///// Clone the last ensemble in the accumulator.
            ///// This will give the latest data. Then update the
            ///// cloned ensemble with the average information.
            ///// </summary>
            ///// <returns>Cloned ensemble with averaged information set.</returns>
            //private DataSet.Ensemble CreateAverageEnsemble(DataSet.Ensemble ensemble)
            //{
            //    DataSet.Ensemble avgEnsemble = ensemble.Clone();

            //    if (avgEnsemble.IsEnsembleAvail)
            //    {
            //        // Update the ensemble with the number of samples
            //        avgEnsemble.EnsembleData.UpdateAverageEnsemble(_numSamples);
            //    }

            //    if (avgEnsemble.IsAncillaryAvail)
            //    {
            //        // Update the first ping time with the first ensemble in the accumulators first ping time
            //        avgEnsemble.AncillaryData.FirstPingTime = _accumEns.First.Value.AncillaryData.FirstPingTime;
            //    }

            //    return avgEnsemble;
            //}

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
            protected override void RemoveEnsemble()
            {
                // Check if the number of samples has been met in the list
                if (_accumEns.Count >= NumSamples)
                {
                    // If running average, remove the first ensemble
                    // If not running average, remove all the ensembles
                    if (IsRunningAverage)
                    {
                        _accumEns.RemoveFirst();
                        _accumRefLayerAvg.RemoveFirst();
                    }
                    else
                    {
                        _accumEns.Clear();
                        _accumRefLayerAvg.Clear();
                    }
                }
            }

            #endregion

            #region Event

            #region Averaged Ensemble

            /// <summary>
            /// Event To subscribe to.  This gives the paramater
            /// that will be passed when subscribing to the event.
            /// </summary>
            /// <param name="ensemble">Data to pass to subscriber.</param>
            public delegate void AveragedEnsembleEventHandler(DataSet.Ensemble ensemble);

            /// <summary>
            /// Subscribe to this event.  This will hold all subscribers.
            /// 
            /// To subscribe:
            /// refLayerAvger.AveragedEnsemble += new refLayerAvger.AveragedEnsembleEventHandler(method to call);
            /// 
            /// To Unsubscribe:
            /// refLayerAvger.AveragedEnsemble -= (method to call)
            /// </summary>
            public event AveragedEnsembleEventHandler AveragedEnsemble;

            /// <summary>
            /// Publish the averaged ensemble to all subscribers.
            /// This will verify there are subscribers, if there are
            /// subscribers, it will pass the ensemble to all subscribers.
            /// </summary>
            /// <param name="ensemble"></param>
            private void PublishAveragedEnsemble(DataSet.Ensemble ensemble)
            {
                if (AveragedEnsemble != null)
                {
                    AveragedEnsemble(ensemble);
                }
            }

            #endregion

            #endregion
        }

    }

}
