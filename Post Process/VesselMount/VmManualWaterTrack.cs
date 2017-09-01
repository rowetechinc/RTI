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
 * 08/30/2017      RC          3.4.2      Initial coding.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI
{
    namespace VesselMount
    {
        /// <summary>
        /// Water Tracking is not available in Water Profile mode.  This will create a manual Water Track.
        /// The user will select a bin.  The bin will then be used to Water Track.
        /// 
        /// This will store the previous velocity in bin.  It will then see the difference between the previous
        /// ensemble's velocity and its current velocity.  The difference velocity in velocity is considered the boat speed.
        /// </summary>
        public class VmManualWaterTrack
        {
            #region Variable

            /// <summary>
            /// Number of values to average.
            /// </summary>
            private const int AVG_COUNT = 5;

            /// <summary>
            /// Instrument X Velocity.
            /// </summary>
            private List<float> _prevInstrX;

            /// <summary>
            /// Instrument Y Velocity.
            /// </summary>
            private List<float> _prevInstrY;

            /// <summary>
            /// Instrument Z Velocity.
            /// </summary>
            private List<float> _prevInstrZ;

            /// <summary>
            /// Earth East Velocity.
            /// </summary>
            private List<float> _prevEarthEast;

            /// <summary>
            /// Earth North Velocity.
            /// </summary>
            private List<float> _prevEarthNorth;

            /// <summary>
            /// Earth Up Velocity.
            /// </summary>
            private List<float> _prevEarthUp;

            /// <summary>
            /// Ship Traverse Velocity.
            /// </summary>
            private List<float> _prevShipTraverse;

            /// <summary>
            /// Ship Longitudinal Velocity.
            /// </summary>
            private List<float> _prevShipLongitudinal;

            /// <summary>
            /// Ship Normal Velocity.
            /// </summary>
            private List<float> _prevShipNormal;


            #endregion

            #region Properties

            /// <summary>
            /// Selected bin to Water Track.
            /// </summary>
            public int SelectedBinMin {get; private set;}

            /// <summary>
            /// Selected bin to Water Track.
            /// </summary>
            public int SelectedBinMax { get; private set; }

            #endregion

            /// <summary>
            /// Initialize the object.
            /// </summary>
            public VmManualWaterTrack()
            {
                SelectedBinMin = -1;
                SelectedBinMax = -1;
                _prevInstrX = new List<float>();
                _prevInstrY = new List<float>();
                _prevInstrZ = new List<float>();
                _prevEarthEast = new List<float>();
                _prevEarthNorth = new List<float>();
                _prevEarthUp = new List<float>();
                _prevShipTraverse = new List<float>();
                _prevShipLongitudinal = new List<float>();
                _prevShipNormal = new List<float>();
            }

            /// <summary>
            /// Calculate the Water Track.  This will use the selected bin to calculate Water Track.
            /// It will take an average of this bin.  It will subtract the current value from the average.
            /// The difference will be returned.  This is the speed increased or decreased by the boat.
            /// </summary>
            /// <param name="ens">Ensemble to add Water Track.</param>
            /// <param name="selectedBinMin">Minimum selected bin.</param>
            /// <param name="selectedBinMax">Maximum selected bin.</param>
            public void Calculate(ref DataSet.Ensemble ens, int selectedBinMin, int selectedBinMax)
            {
                // Verify we have enough data to do this calculation
                if(!ens.IsEnsembleAvail || !ens.IsAncillaryAvail)
                {
                    return;
                }

                // Verify Bin selections
                if (!VerifyBinSelection(ens.EnsembleData.NumBins, selectedBinMin, selectedBinMax))
                {
                    return;
                }

                // Calculate the depth layer
                float depthLayer = selectedBinMin;
                if(ens.IsAncillaryAvail)
                {
                    float depthLayerMin = ens.AncillaryData.FirstBinRange + (selectedBinMin * ens.AncillaryData.BinSize);
                    float depthLayerMax = ens.AncillaryData.FirstBinRange + (selectedBinMax * ens.AncillaryData.BinSize);
                    
                    // Set the depth layer to the mid point of the min and max
                    depthLayer = depthLayerMin + ((depthLayerMax - depthLayerMin) / 2.0f);
                }

                // INSTRUMENT DATA
                if(ens.IsInstrumentVelocityAvail)
                {
                    // Accumulate the Instrument data from the selected bins
                    float[] accumEnsInstr = AccumulateEnsemble(ens.InstrumentVelocityData.InstrumentVelocityData, SelectedBinMin, SelectedBinMax);

                    // Accumulate the Instrument data for running average of previous ensembles
                    AccumulateInstrument(accumEnsInstr[0], accumEnsInstr[1], accumEnsInstr[2]);

                    // Average the accumulated data
                    float[] avgInstr = AverageInstrument();

                    // Store the results to the Water Track Data Sets
                    StoreInstrumentWt(ref ens, avgInstr, depthLayer);
                }
             

                // EARTH DATA
                if(ens.IsEarthVelocityAvail)
                {
                    // Accumulate the Earth data from the selected bins
                    float[] accumEnsEarth = AccumulateEnsemble(ens.EarthVelocityData.EarthVelocityData, SelectedBinMin, SelectedBinMax);

                    // Accumulate the Earth data for running average of previous ensembles
                    AccumulateEarth(accumEnsEarth[0], accumEnsEarth[1], accumEnsEarth[2]);

                    // Average the accumulated data
                    float[] avgEarth = AverageEarth();

                    // Store the results to the Water Track Data Sets
                    StoreEarthWt(ref ens, avgEarth, depthLayer);
                }

                // SHIP DATA
                if (ens.IsShipVelocityAvail)
                {
                    // Accumulate the Earth data from the selected bins
                    float[] accumEnsShip = AccumulateEnsemble(ens.ShipVelocityData.ShipVelocityData, SelectedBinMin, SelectedBinMax);

                    // Accumulate the Ship data for running average of previous ensembles
                    AccumulateShip(accumEnsShip[0], accumEnsShip[1], accumEnsShip[2]);

                    // Average the accumulated data
                    float[] avgShip = AverageShip();

                    // Store the results to the Water Track Data Sets
                    StoreShipWt(ref ens, avgShip, depthLayer);
                }

            }

            #region Bin

            /// <summary>
            /// Verify the bins given are correct.  Verify they can be used.
            /// </summary>
            /// <param name="numBins">Number of bins in the ensemble.</param>
            /// <param name="minBin">Minimum Bin.</param>
            /// <param name="maxBin">Maximum Bin.</param>
            /// <returns></returns>
            private bool VerifyBinSelection(int numBins, int minBin, int maxBin)
            {
                // Check if the bin exist
                if (minBin < 0 ||                           // Min Bin is less than 0
                    maxBin < 0 ||                           // Max Bin is less than 0
                    minBin >= numBins ||                    // Min Bin is greater than number of bins
                    maxBin >= numBins ||                    // Max Bin is greater than number of bins 
                    minBin > maxBin)                       // Min Bin is greater than Max Bin
                {
                    return false;
                }

                // Check if the selected bin has changed
                // If it has changed, clear the data accumulated
                if (SelectedBinMin > 0 && SelectedBinMin != minBin)
                {
                    // Set the bin
                    SelectedBinMin = minBin;

                    // Clear the accumulated data
                    ClearInsturment();
                    ClearEarth();
                    ClearShip();

                }
                else if (SelectedBinMin < 0)
                {
                    // The selected bin was never set, so set it now
                    SelectedBinMin = minBin;
                }

                // Check if the selected bin has changed
                // If it has changed, clear the data accumulated
                if (SelectedBinMax > 0 && SelectedBinMax != maxBin)
                {
                    // Set the bin
                    SelectedBinMax = maxBin;

                    // Clear the accumulated data
                    ClearInsturment();
                    ClearEarth();
                    ClearShip();

                }
                else if (SelectedBinMax < 0)
                {
                    // The selected bin was never set, so set it now
                    SelectedBinMax = maxBin;
                }

                return true;
            }

            #endregion

            #region Accumulate

            /// <summary>
            /// Accumulate all the data from the ensemble for the selected bins.  
            /// This will be used to get an average of the selected bins.
            /// </summary>
            /// <param name="data">Velocity Data from the ensemble.</param>
            /// <param name="minBin">Minimum bin selected.</param>
            /// <param name="maxBin">Maximum bin selected.</param>
            /// <returns>Average of selected bin.</returns>
            private float[] AccumulateEnsemble(float[,] data, int minBin, int maxBin)
            {
                // Get the values
                float x, y, z;
                float ensAccumX = 0.0f;
                float ensAccumY = 0.0f;
                float ensAccumZ = 0.0f;
                int ensCountX = 0;
                int ensCountY = 0;
                int ensCountZ = 0;

                // Create an array to store the bin
                // Initialize to bad velocity
                float[] result = new float[3];
                result[0] = DataSet.Ensemble.BAD_VELOCITY;
                result[1] = DataSet.Ensemble.BAD_VELOCITY;
                result[2] = DataSet.Ensemble.BAD_VELOCITY;
 
                // If it is only 1 bin selected
                if (maxBin - minBin == 0)
                {
                    // Give the velocity for this bin
                    result[0] = data[minBin, 0];
                    result[1] = data[minBin, 1];
                    result[2] = data[minBin, 2];
                }
                else
                {
                    // Go through all the selected bins and accumulate the data for an average
                    for (int bin = minBin; minBin <= maxBin; minBin++)
                    {
                        // Get the data
                        x = data[bin, 0];
                        y = data[bin, 1];
                        z = data[bin, 2];

                        // If any of the data is bad, do not accumulate it
                        if (x == DataSet.Ensemble.BAD_VELOCITY ||
                            y == DataSet.Ensemble.BAD_VELOCITY ||
                            z == DataSet.Ensemble.BAD_VELOCITY)
                        {
                            continue;
                        }

                        // Accumulate for the average
                        ensAccumX += x;
                        ensCountX++;

                        // Accumulate for the average
                        ensAccumY += y;
                        ensCountY++;

                        // Accumulate for the average
                        ensAccumZ += z;
                        ensCountZ++;
                    }

                    if (ensCountX > 0)
                    {
                        // Store the average of the ensemble for the selected bins
                        result[0] = ensAccumX / ensCountX;
                        result[1] = ensAccumY / ensCountY;
                        result[2] = ensAccumZ / ensCountZ;
                    }
                }
                return result;
            }

            /// <summary>
            /// Accumulate the Instrument data.
            /// </summary>
            /// <param name="x">X Velocity for selected bin.</param>
            /// <param name="y">Y Velocity for selected bin.</param>
            /// <param name="z">Z Velocity for selected bin.</param>
            private void AccumulateInstrument(float x, float y, float z)
            {
                // If any of the data is bad, do not accumulate it
                if (x == DataSet.Ensemble.BAD_VELOCITY ||
                    y == DataSet.Ensemble.BAD_VELOCITY ||
                    z == DataSet.Ensemble.BAD_VELOCITY)
                {
                    return;
                }

                // Limit the list size to AVG_COUNT
                if (_prevInstrX.Count > AVG_COUNT)
                {
                    // Remove the first then add the new value
                    _prevInstrX.RemoveAt(0);
                    _prevInstrX.Add(x);
                }
                else
                {
                    _prevInstrX.Add(x);
                }

                // Limit the list size to AVG_COUNT
                if (_prevInstrY.Count > AVG_COUNT)
                {
                    // Remove the first then add the new value
                    _prevInstrY.RemoveAt(0);
                    _prevInstrY.Add(y);
                }
                else
                {
                    _prevInstrY.Add(y);
                }

                // Limit the list size to AVG_COUNT
                if (_prevInstrZ.Count > AVG_COUNT)
                {
                    // Remove the first then add the new value
                    _prevInstrZ.RemoveAt(0);
                    _prevInstrZ.Add(z);
                }
                else
                {
                    _prevInstrZ.Add(z);
                }
            }

            /// <summary>
            /// Accumulate the East data.
            /// </summary>
            /// <param name="east">East Velocity for selected bin.</param>
            /// <param name="north">North Velocity for selected bin.</param>
            /// <param name="up">Up Velocity for selected bin.</param>
            private void AccumulateEarth(float east, float north, float up)
            {
                // If any of the data is bad, do not accumulate it
                if (east == DataSet.Ensemble.BAD_VELOCITY ||
                    north == DataSet.Ensemble.BAD_VELOCITY ||
                    up == DataSet.Ensemble.BAD_VELOCITY)
                {
                    return;
                }

                // Limit the list size to AVG_COUNT
                if (_prevEarthEast.Count > AVG_COUNT)
                {
                    // Remove the first then add the new value
                    _prevEarthEast.RemoveAt(0);
                    _prevEarthEast.Add(east);
                }
                else
                {
                    _prevEarthEast.Add(east);
                }

                // Limit the list size to AVG_COUNT
                if (_prevEarthNorth.Count > AVG_COUNT)
                {
                    // Remove the first then add the new value
                    _prevEarthNorth.RemoveAt(0);
                    _prevEarthNorth.Add(north);
                }
                else
                {
                    _prevEarthNorth.Add(north);
                }

                // Limit the list size to AVG_COUNT
                if (_prevEarthUp.Count > AVG_COUNT)
                {
                    // Remove the first then add the new value
                    _prevEarthUp.RemoveAt(0);
                    _prevEarthUp.Add(up);
                }
                else
                {
                    _prevEarthUp.Add(up);
                }
            }

            /// <summary>
            /// Accumulate the Ship data.
            /// </summary>
            /// <param name="traverse">Traverse Velocity for selected bin.</param>
            /// <param name="longitudinal">Longitudinal Velocity for selected bin.</param>
            /// <param name="normal">Normal Velocity for selected bin.</param>
            private void AccumulateShip(float traverse, float longitudinal, float normal)
            {
                // If any of the data is bad, do not accumulate it
                if (traverse == DataSet.Ensemble.BAD_VELOCITY ||
                    longitudinal == DataSet.Ensemble.BAD_VELOCITY ||
                    normal == DataSet.Ensemble.BAD_VELOCITY)
                {
                    return;
                }

                // Limit the list size to AVG_COUNT
                if (_prevShipTraverse.Count > AVG_COUNT)
                {
                    // Remove the first then add the new value
                    _prevShipTraverse.RemoveAt(0);
                    _prevShipTraverse.Add(traverse);
                }
                else
                {
                    _prevShipTraverse.Add(traverse);
                }

                // Limit the list size to AVG_COUNT
                if (_prevShipLongitudinal.Count > AVG_COUNT)
                {
                    // Remove the first then add the new value
                    _prevShipLongitudinal.RemoveAt(0);
                    _prevShipLongitudinal.Add(longitudinal);
                }
                else
                {
                    _prevShipLongitudinal.Add(longitudinal);
                }

                // Limit the list size to AVG_COUNT
                if (_prevShipNormal.Count > AVG_COUNT)
                {
                    // Remove the first then add the new value
                    _prevShipNormal.RemoveAt(0);
                    _prevShipNormal.Add(normal);
                }
                else
                {
                    _prevShipNormal.Add(normal);
                }
            }

            #endregion

            #region Average

            /// <summary>
            /// Average the accumulated Instrument data.
            /// </summary>
            /// <returns>Array containing average of Insturment Velocities selected bin.</returns>
            private float[] AverageInstrument()
            {
                // Result Array
                // result[0] = X
                // result[1] = Y
                // result[2] = Z
                float[] result = new float[3];

                // Average X
                int countX = 0;
                float accumX = 0.0f;
                foreach(float x in _prevInstrX)
                {
                    countX++;
                    accumX += x;
                }
                if (countX > 0)
                {
                    result[0] = accumX / countX;
                }
                else
                {
                    result[0] = 0.0f;
                }

                // Average Y
                int countY = 0;
                float accumY = 0.0f;
                foreach (float y in _prevInstrY)
                {
                    countY++;
                    accumY += y;
                }
                if (countY > 0)
                {
                    result[1] = accumY / countY;
                }
                else
                {
                    result[1] = 0.0f;
                }

                // Average Z
                int countZ = 0;
                float accumZ = 0.0f;
                foreach (float z in _prevInstrZ)
                {
                    countZ++;
                    accumZ += z;
                }
                if (countZ > 0)
                {
                    result[2] = accumZ / countZ;
                }
                else
                {
                    result[2] = 0.0f;
                }


                return result;
            }

            /// <summary>
            /// Average the accumulated Earth data.
            /// </summary>
            /// <returns>Array containing average of Earth Velocities for selected bin.</returns>
            private float[] AverageEarth()
            {
                // Result Array
                // result[0] = East
                // result[1] = North
                // result[2] = Up
                float[] result = new float[3];

                // Average East
                int countEast = 0;
                float accumEast = 0.0f;
                foreach (float east in _prevEarthEast)
                {
                    countEast++;
                    accumEast += east;
                }
                if (countEast > 0)
                {
                    result[0] = accumEast / countEast;
                }
                else
                {
                    result[0] = 0.0f;
                }

                // Average North
                int countNorth = 0;
                float accumNorth = 0.0f;
                foreach (float north in _prevEarthNorth)
                {
                    countNorth++;
                    accumNorth += north;
                }
                if (countNorth > 0)
                {
                    result[1] = accumNorth / countNorth;
                }
                else
                {
                    result[1] = 0.0f;
                }

                // Average Up
                int countUp = 0;
                float accumUp = 0.0f;
                foreach (float up in _prevEarthUp)
                {
                    countUp++;
                    accumUp += up;
                }
                if (countUp > 0)
                {
                    result[2] = accumUp / countUp;
                }
                else
                {
                    result[2] = 0.0f;
                }

                return result;
            }

            /// <summary>
            /// Average the accumulated Ship data.
            /// </summary>
            /// <returns>Array containing average of Ship Velocities for selected bin.</returns>
            private float[] AverageShip()
            {
                // Result Array
                // result[0] = Traverse
                // result[1] = Longitudinal
                // result[2] = Normal
                float[] result = new float[3];

                // Average Traverse
                int countTraverse = 0;
                float accumTraverse = 0.0f;
                foreach (float traverse in _prevShipTraverse)
                {
                    countTraverse++;
                    accumTraverse += traverse;
                }
                if (accumTraverse > 0)
                {
                    result[0] = accumTraverse / countTraverse;
                }
                else
                {
                    result[0] = 0.0f;
                }

                // Average Longitudinal
                int countLongitudinal = 0;
                float accumLongitudinal = 0.0f;
                foreach (float north in _prevShipLongitudinal)
                {
                    countLongitudinal++;
                    accumLongitudinal += north;
                }
                if (countLongitudinal > 0)
                {
                    result[1] = accumLongitudinal / countLongitudinal;
                }
                else
                {
                    result[1] = 0.0f;
                }

                // Average Normal
                int countNormal = 0;
                float accumNormal = 0.0f;
                foreach (float up in _prevShipNormal)
                {
                    countNormal++;
                    accumNormal += up;
                }
                if (countNormal > 0)
                {
                    result[2] = accumNormal / countNormal;
                }
                else
                {
                    result[2] = 0.0f;
                }

                return result;
            }

            #endregion

            #region Calculate Water Track 

            /// <summary>
            /// Calculate the Water Track value based off the average of the bin and the latest value.
            /// Subtract the latest value with the average.  The difference is the difference in speed
            /// that the boat has acheived.
            /// </summary>
            /// <param name="avg">Average of the Instrument velocities for the selected bin.</param>
            /// <param name="x">Lastest Instrument Velocity X for the selected bin.</param>
            /// <param name="y">Latest Instrument Velocity Y for the selected bin.</param>
            /// <param name="z">Latest Instrument Velocity Z for the selected bin.</param>
            /// <returns>Array containing the speed for the selected bin.</returns>
            private float[] CalcWtInstrument(float[] avg, float x, float y, float z)
            {
                float[] result = new float[3];

                // Subtract current value from avreage
                result[0] = avg[0] - x;
                result[1] = avg[1] - y;
                result[2] = avg[2] - z;

                return result;
            }

            /// <summary>
            /// Calculate the Water Track value based off the average of the bin and the latest value.
            /// Subtract the latest value with the average.  The difference is the difference in speed
            /// that the boat has acheived.
            /// </summary>
            /// <param name="avg">Average of the Earth velocities for the selected bin.</param>
            /// <param name="east">Lastest Earth Velocity East for the selected bin.</param>
            /// <param name="north">Latest Earth Velocity North for the selected bin.</param>
            /// <param name="up">Latest Earth Velocity Up for the selected bin.</param>
            /// <returns>Array containing the speed for the selected bin.</returns>
            private float[] CalcWtEarth(float[] avg, float east, float north, float up)
            {
                float[] result = new float[3];

                // Subtract current value from avreage
                result[0] = avg[0] - east;
                result[1] = avg[1] - north;
                result[2] = avg[2] - up;

                return result;
            }

            /// <summary>
            /// Calculate the Water Track value based off the average of the bin and the latest value.
            /// Subtract the latest value with the average.  The difference is the difference in speed
            /// that the boat has acheived.
            /// </summary>
            /// <param name="avg">Average of the Ship velocities for the selected bin.</param>
            /// <param name="traverse">Lastest Ship Velocity Traverse for the selected bin.</param>
            /// <param name="longitudinal">Latest Ship Velocity Longitudinal for the selected bin.</param>
            /// <param name="normal">Latest Ship Velocity Normal for the selected bin.</param>
            /// <returns>Array containing the speed for the selected bin.</returns>
            private float[] CalcWtShip(float[] avg, float traverse, float longitudinal, float normal)
            {
                float[] result = new float[3];

                // Subtract current value from avreage
                result[0] = avg[0] - traverse;
                result[1] = avg[1] - longitudinal;
                result[2] = avg[2] - normal;

                return result;
            }

            #endregion

            #region Store Water Track

            /// <summary>
            /// Store the Earth Water Track (Water Mass) to the ensemble.
            /// Mulitply the final result to -1 to match the sign with Bottom Track.
            /// </summary>
            /// <param name="ens">Ensemble to add the data.</param>
            /// <param name="earth">Earth Water Track data to add to the ensemble.</param>
            /// <param name="depthLayer">Depth that Water Track was collected.</param>
            private void StoreEarthWt(ref DataSet.Ensemble ens, float[] earth, float depthLayer)
            {
                if(!ens.IsEarthWaterMassAvail)
                {
                    // Add the dataset to the ensemble
                    EnsembleHelper.AddWaterMassEarth(ref ens);
                }

                ens.EarthWaterMassData.VelocityEast = (-1) * earth[0];                  // Invert the sign to match Bottom Track
                ens.EarthWaterMassData.VelocityNorth = (-1) * earth[1];
                ens.EarthWaterMassData.VelocityVertical = (-1) * earth[2];
                ens.EarthWaterMassData.WaterMassDepthLayer = depthLayer;
            }

            /// <summary>
            /// Store the Instrument Water Track (Water Mass) to the ensemble.
            /// Mulitply the final result to -1 to match the sign with Bottom Track.
            /// </summary>
            /// <param name="ens">Ensemble to add the data.</param>
            /// <param name="instrument">Instrument Water Track data to add to the ensemble.</param>
            /// <param name="depthLayer">Depth that Water Track was collected.</param>
            private void StoreInstrumentWt(ref DataSet.Ensemble ens, float[] instrument, float depthLayer)
            {
                if (!ens.IsInstrumentWaterMassAvail)
                {
                    // Add the dataset to the ensemble
                    EnsembleHelper.AddWaterMassInstrument(ref ens);
                }

                ens.InstrumentWaterMassData.VelocityX = (-1) * instrument[0];           // Invert the sign to match Bottom Track
                ens.InstrumentWaterMassData.VelocityY = (-1) * instrument[1];
                ens.InstrumentWaterMassData.VelocityZ = (-1) * instrument[2];
                ens.InstrumentWaterMassData.WaterMassDepthLayer = depthLayer;
            }

            /// <summary>
            /// Store the Ship Water Track (Water Mass) to the ensemble.
            /// Mulitply the final result to -1 to match the sign with Bottom Track.
            /// </summary>
            /// <param name="ens">Ensemble to add the data.</param>
            /// <param name="ship">Ship Water Track data to add to the ensemble.</param>
            /// <param name="depthLayer">Depth that Water Track was collected.</param>
            private void StoreShipWt(ref DataSet.Ensemble ens, float[] ship, float depthLayer)
            {
                if (!ens.IsShipWaterMassAvail)
                {
                    // Add the dataset to the ensemble
                    EnsembleHelper.AddWaterMassShip(ref ens);
                }

                ens.ShipWaterMassData.VelocityTransverse = (-1) * ship[0];              // Invert the sign to match Bottom Track
                ens.ShipWaterMassData.VelocityLongitudinal = (-1) * ship[1];
                ens.ShipWaterMassData.VelocityNormal = (-1) * ship[2];
                ens.InstrumentWaterMassData.WaterMassDepthLayer = depthLayer;
            }

            #endregion

            #region Clear Accumulated

            /// <summary>
            /// Clear the Instrument accumulated data.
            /// </summary>
            private void ClearInsturment()
            {
                _prevInstrX.Clear();
                _prevInstrY.Clear();
                _prevInstrZ.Clear();
            }

            /// <summary>
            /// Clear the Earth accumulated data.
            /// </summary>
            private void ClearEarth()
            {
                _prevEarthEast.Clear();
                _prevEarthNorth.Clear();
                _prevEarthUp.Clear();
            }

            /// <summary>
            /// Clear the Ship accumulated data.
            /// </summary>
            private void ClearShip()
            {
                _prevShipTraverse.Clear();
                _prevShipLongitudinal.Clear();
                _prevShipNormal.Clear();
            }

            #endregion
        }
    }
}
