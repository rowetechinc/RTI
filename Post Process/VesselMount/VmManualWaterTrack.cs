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
            public int SelectedBin {get; private set;}

            #endregion

            /// <summary>
            /// Initialize the object.
            /// </summary>
            public VmManualWaterTrack()
            {
                SelectedBin = -1;
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
            /// <param name="selectedBin">Selected bin.</param>
            public void Calculate(ref DataSet.Ensemble ens, int selectedBin)
            {
                // Check if the selected bin has changed
                // If it has changed, clear the data accumulated
                if(SelectedBin > 0 && SelectedBin != selectedBin)
                {
                    // Set the bin
                    SelectedBin = selectedBin;

                    // Clear the accumulated data
                    ClearInsturment();
                    ClearEarth();
                    ClearShip();
                    
                }
                else if(SelectedBin < 0)
                {
                    // The selected bin was never set, so set it now
                    SelectedBin = selectedBin;
                }

                // Check if the bin exist
                if(ens.IsEnsembleAvail)
                {
                    if(selectedBin < 0 || selectedBin >= ens.EnsembleData.NumBins)
                    {
                        return;
                    }
                }

                // Calculate the depth layer
                float depthLayer = selectedBin;
                if(ens.IsAncillaryAvail)
                {
                    depthLayer = ens.AncillaryData.FirstBinRange + (selectedBin * ens.AncillaryData.BinSize);
                }

                // Check if the data is good
                // We need Instrument data and Earth data
                // If Instrument is bad, then Earth is bad
                if(ens.IsInstrumentVelocityAvail)
                {
                    if(ens.InstrumentVelocityData.InstrumentVelocityData[selectedBin, 0] == DataSet.Ensemble.BAD_VELOCITY ||
                       ens.InstrumentVelocityData.InstrumentVelocityData[selectedBin, 1] == DataSet.Ensemble.BAD_VELOCITY ||
                       ens.InstrumentVelocityData.InstrumentVelocityData[selectedBin, 2] == DataSet.Ensemble.BAD_VELOCITY ||
                       ens.InstrumentVelocityData.InstrumentVelocityData[selectedBin, 3] == DataSet.Ensemble.BAD_VELOCITY)
                    {
                        return;
                    }

                    // INSTRUMENT DATA
                    // Average the accumulated data
                    float[] avgInstr = AverageInstrument();

                    // Calculate the Water Track Instrument
                    // Subtract the latest data from average
                    float[] wtInstr = CalcWtInstrument(avgInstr,
                                                    ens.InstrumentVelocityData.InstrumentVelocityData[selectedBin, 0],
                                                    ens.InstrumentVelocityData.InstrumentVelocityData[selectedBin, 0],
                                                    ens.InstrumentVelocityData.InstrumentVelocityData[selectedBin, 0]);

                    // Store the results to the Water Track Data Sets
                    StoreInstrumentWt(ref ens, wtInstr, depthLayer);

                    // Accumulate the Instrument data for next average
                    AccumulateInstrument(ens.InstrumentVelocityData.InstrumentVelocityData[selectedBin, 0],
                                         ens.InstrumentVelocityData.InstrumentVelocityData[selectedBin, 1],
                                         ens.InstrumentVelocityData.InstrumentVelocityData[selectedBin, 2]);
                }
             

                // EARTH DATA
                if(ens.IsEarthVelocityAvail)
                {
                    // Average the accumulated data
                    float[] avgEarth = AverageEarth();

                    // Calculate the Water Track Earth
                    // Subtract the latest data from average
                    float[] wtEarth = CalcWtEarth(avgEarth,
                                ens.EarthVelocityData.EarthVelocityData[selectedBin, 0],
                                ens.EarthVelocityData.EarthVelocityData[selectedBin, 0],
                                ens.EarthVelocityData.EarthVelocityData[selectedBin, 0]);

                    // Store the results to the Water Track Data Sets
                    StoreEarthWt(ref ens, wtEarth, depthLayer);

                    // Accumulate the Earth data for next average
                    AccumulateEarth(ens.EarthVelocityData.EarthVelocityData[selectedBin, 0],
                                    ens.EarthVelocityData.EarthVelocityData[selectedBin, 1],
                                    ens.EarthVelocityData.EarthVelocityData[selectedBin, 2]);
                }

                // SHIP DATA
                if (ens.IsShipVelocityAvail)
                {
                    // Average the accumulated data
                    float[] avgShip = AverageShip();

                    // Calculate the Water Track Ship
                    // Subtract the latest data from average
                    float[] wtShip = CalcWtShip(avgShip,
                                ens.ShipVelocityData.ShipVelocityData[selectedBin, 0],
                                ens.ShipVelocityData.ShipVelocityData[selectedBin, 0],
                                ens.ShipVelocityData.ShipVelocityData[selectedBin, 0]);

                    // Store the results to the Water Track Data Sets
                    StoreShipWt(ref ens, wtShip, depthLayer);

                    // Accumulate the Ship data for next average
                    AccumulateShip(ens.ShipVelocityData.ShipVelocityData[selectedBin, 0],
                                    ens.ShipVelocityData.ShipVelocityData[selectedBin, 1],
                                    ens.ShipVelocityData.ShipVelocityData[selectedBin, 2]);
                }

            }

            #region Accumulate

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
                if(_prevInstrX.Count > AVG_COUNT)
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
            /// <param name="east">Traverse Velocity for selected bin.</param>
            /// <param name="north">Longitudinal Velocity for selected bin.</param>
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
                    result[0] = accumZ / countZ;
                }
                else
                {
                    result[0] = 0.0f;
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
            /// <param name="x">Lastest Earth Velocity East for the selected bin.</param>
            /// <param name="y">Latest Earth Velocity North for the selected bin.</param>
            /// <param name="z">Latest Earth Velocity Up for the selected bin.</param>
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

                ens.EarthWaterMassData.VelocityEast = earth[0];
                ens.EarthWaterMassData.VelocityNorth = earth[1];
                ens.EarthWaterMassData.VelocityVertical = earth[2];
                ens.EarthWaterMassData.WaterMassDepthLayer = depthLayer;
            }

            /// <summary>
            /// Store the Instrument Water Track (Water Mass) to the ensemble.
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

                ens.InstrumentWaterMassData.VelocityX = instrument[0];
                ens.InstrumentWaterMassData.VelocityY = instrument[1];
                ens.InstrumentWaterMassData.VelocityZ = instrument[2];
                ens.InstrumentWaterMassData.WaterMassDepthLayer = depthLayer;
            }

            /// <summary>
            /// Store the Ship Water Track (Water Mass) to the ensemble.
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

                ens.ShipWaterMassData.VelocityTransverse = ship[0];
                ens.ShipWaterMassData.VelocityLongitudinal = ship[1];
                ens.ShipWaterMassData.VelocityNormal = ship[2];
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
