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
 * 12/28/2011      RC          1.10       Initial coding.
 * 02/24/2012      RC          2.03       Create VelocityVectorHelper with a method to create velocity vectors here.
 * 03/16/2012      RC          2.06       Set the magnitude to absolute value.  No negatives.
 * 06/07/2012      RC          2.11       Added GenerateVelocityVectors() and GenerateAmplitudeVectors().
 * 03/07/2013      RC          2.18       In CreateVelocityVector(), create the VelocityVector before setting the values.
 * 03/22/2013      RC          2.19       Fixed bug in GenerateAmplitudeVectors() where the VelociytVector was not created.      
 * 07/29/2014      RC          2.23.0     Added Instrument Velocity Vectors.  Made specific Earth and Instrument functions.
 * 04/16/2015      RC          3.0.4      Check how many beams in GenerateInstrumentVectors().
 * 09/05/2017      RC          3.4.3      Added GenerateShipVectors().
 * 
 */


using System;
namespace RTI
{
    namespace DataSet
    {
        /// <summary>
        /// A struct to hold the velocity vectors
        /// for an ensemble.
        /// </summary>
        public struct EnsembleVelocityVectors
        {
            /// <summary>
            /// Unique ID for the ensemble.
            /// </summary>
            public DataSet.UniqueID Id;

            /// <summary>
            /// Array of vectors representing
            /// the velocities for each bin.
            /// </summary>
            public VelocityVector[] Vectors;
        }

        /// <summary>
        /// Represent the velocity data for each
        /// bin as a vector.  This will store the
        /// velocity data as magnitude and direction.
        /// Direction is given with X and Y North.
        /// </summary>
        public class VelocityVector
        {
            /// <summary>
            /// Magnitude for the bin with 
            /// Bottom Track velocity removed.
            /// </summary>
            public double Magnitude { get; set; }

            /// <summary>
            /// 
            /// USE THIS VALUE FOR ADCP WATER DIRECTION
            /// 
            /// Direction of the bin velocity
            /// with Bottom Track velocity removed.
            /// The positive X direction is North.
            /// </summary>
            public double DirectionXNorth { get; set; }

            /// <summary>
            /// Direction of the bin velocity
            /// with Bottom Track velocity removed.
            /// The positive Y direction is North.
            /// </summary>
            public double DirectionYNorth { get; set; }
        }

        /// <summary>
        /// Helper class to create velocity vectors
        /// for an ensemble.
        /// </summary>
        public class VelocityVectorHelper
        {
            /// <summary>
            /// Create a VelocityVector array based off the ensemble given.  Earth velocity
            /// data needs to be available for a result to be returned.  This will remove the
            /// ship speed either using Bottom Track data or GPS speed.  The Bottom Track data
            /// is passed in to ensure the previous good Bottom Track data is used if current
            /// Bottom Track data is bad.  GPS speed will be used if Bottom Track data is bad.
            /// If both Bottom Track and GPS are bad, nothing will be used.  Direction is given
            /// with Y both North and East.
            /// </summary>
            /// <param name="ensemble">Ensemble to remove ship speed and create VelocityVector array.</param>
            /// <returns>VelocityVector created.</returns>
            public static bool CreateVelocityVector(ref DataSet.Ensemble ensemble)
            {
                // Generate the Earth Velocity Vectors
                GenerateEarthVectors(ref ensemble);

                // Generate the Instrument Velocity Vectors
                GenerateInstrumentVectors(ref ensemble);

                // Generate the Instrument Velocity Vectors
                GenerateShipVectors(ref ensemble);

                return true;
            }

            /// <summary>
            /// Create an struct to hold bin vectors for the velocity data
            /// based off the ensemble given.  This will remove the
            /// Bottom Track velocity from the velocity data then
            /// create  a vector representing the
            /// water velocity.
            /// Removing Ship Speed:
            /// BT Good, GPS Good or Bad = BT velocities
            /// BT Bad, GPS Good = GPS velocity
            /// BT Bad, GPS Bad = Previous Good BT
            /// BT Bad, Gps Bad, Previous BT Bad = None
            /// 
            /// </summary>
            /// <param name="adcpData">Ensemble to generate vector values.</param>
            /// <returns>Vectors for each bin in ensemble.</returns>
            public static DataSet.EnsembleVelocityVectors GenerateEarthVelocityVectors(DataSet.Ensemble adcpData)
            {
                // Create the velocity vector data
                DataSet.VelocityVectorHelper.CreateVelocityVector(ref adcpData);

                // Create struct to hold the data
                DataSet.EnsembleVelocityVectors ensVec = new DataSet.EnsembleVelocityVectors();
                ensVec.Id = adcpData.EnsembleData.UniqueId;

                if (adcpData.IsEarthVelocityAvail && adcpData.EarthVelocityData.IsVelocityVectorAvail)
                {
                    ensVec.Vectors = adcpData.EarthVelocityData.VelocityVectors;
                }

                return ensVec;
            }

            /// <summary>
            /// Create an struct to hold bin vectors for the velocity data
            /// based off the ensemble given.  This will remove the
            /// Bottom Track velocity from the velocity data then
            /// create  a vector representing the
            /// water velocity.
            /// Removing Ship Speed:
            /// BT Good, GPS Good or Bad = BT velocities
            /// BT Bad, GPS Good = GPS velocity
            /// BT Bad, GPS Bad = Previous Good BT
            /// BT Bad, Gps Bad, Previous BT Bad = None
            /// 
            /// </summary>
            /// <param name="adcpData">Ensemble to generate vector values.</param>
            /// <returns>Vectors for each bin in ensemble.</returns>
            public static DataSet.EnsembleVelocityVectors GenerateInstrumentVelocityVectors(DataSet.Ensemble adcpData)
            {
                // Create the velocity vector data
                DataSet.VelocityVectorHelper.CreateVelocityVector(ref adcpData);

                // Create struct to hold the data
                DataSet.EnsembleVelocityVectors ensVec = new DataSet.EnsembleVelocityVectors();
                ensVec.Id = adcpData.EnsembleData.UniqueId;

                if (adcpData.IsInstrumentVelocityAvail && adcpData.InstrumentVelocityData.IsVelocityVectorAvail)
                {
                    ensVec.Vectors = adcpData.InstrumentVelocityData.VelocityVectors;
                }

                return ensVec;
            }

            /// <summary>
            /// Create an EnsembleVelocityVector based off the ensemble.
            /// If the Velocity Vectors do not exist, create the vectors.
            /// </summary>
            /// <param name="adcpData">Ensemble data.</param>
            /// <returns>EnsembleVelocityVector with the velocity vector data.</returns>
            public static DataSet.EnsembleVelocityVectors GetEarthVelocityVectors(DataSet.Ensemble adcpData)
            {
                // Create struct to hold the data
                DataSet.EnsembleVelocityVectors ensVec = new DataSet.EnsembleVelocityVectors();
                ensVec.Id = adcpData.EnsembleData.UniqueId;

                // Check, if velocity vectors exist
                if (adcpData.IsEarthVelocityAvail && adcpData.EarthVelocityData.IsVelocityVectorAvail)
                {
                    ensVec.Vectors = adcpData.EarthVelocityData.VelocityVectors;
                }
                else if (adcpData.IsEarthVelocityAvail)
                {
                    // Create the velocity vector data
                    DataSet.VelocityVectorHelper.CreateVelocityVector(ref adcpData);

                    ensVec.Vectors = adcpData.EarthVelocityData.VelocityVectors;
                }

                return ensVec;
            }

            /// <summary>
            /// Create an EnsembleVelocityVector based off the ensemble.
            /// If the Velocity Vectors do not exist, create the vectors.
            /// </summary>
            /// <param name="adcpData">Ensemble data.</param>
            /// <returns>EnsembleVelocityVector with the velocity vector data.</returns>
            public static DataSet.EnsembleVelocityVectors GetInstrumentVelocityVectors(DataSet.Ensemble adcpData)
            {
                // Create struct to hold the data
                DataSet.EnsembleVelocityVectors ensVec = new DataSet.EnsembleVelocityVectors();
                ensVec.Id = adcpData.EnsembleData.UniqueId;

                // Check, if velocity vectors exist
                if (adcpData.IsInstrumentVelocityAvail && adcpData.InstrumentVelocityData.IsVelocityVectorAvail)
                {
                    ensVec.Vectors = adcpData.InstrumentVelocityData.VelocityVectors;
                }
                else if (adcpData.IsInstrumentVelocityAvail)
                {
                    // Create the velocity vector data
                    DataSet.VelocityVectorHelper.CreateVelocityVector(ref adcpData);

                    ensVec.Vectors = adcpData.InstrumentVelocityData.VelocityVectors;
                }

                return ensVec;
            }

            /// <summary>
            /// Create an EnsembleVelocityVector based off the ensemble.
            /// If the Velocity Vectors do not exist, create the vectors.
            /// </summary>
            /// <param name="adcpData">Ensemble data.</param>
            /// <returns>EnsembleVelocityVector with the velocity vector data.</returns>
            public static DataSet.EnsembleVelocityVectors GetShipVelocityVectors(DataSet.Ensemble adcpData)
            {
                // Create struct to hold the data
                DataSet.EnsembleVelocityVectors ensVec = new DataSet.EnsembleVelocityVectors();
                ensVec.Id = adcpData.EnsembleData.UniqueId;

                // Check, if velocity vectors exist
                if (adcpData.IsShipVelocityAvail && adcpData.ShipVelocityData.IsVelocityVectorAvail)
                {
                    ensVec.Vectors = adcpData.ShipVelocityData.VelocityVectors;
                }
                else if (adcpData.IsShipVelocityAvail)
                {
                    // Create the velocity vector data
                    DataSet.VelocityVectorHelper.CreateVelocityVector(ref adcpData);

                    ensVec.Vectors = adcpData.ShipVelocityData.VelocityVectors;
                }

                return ensVec;
            }

            /// <summary>
            /// Generate the Earth Velocity vector and store it to the
            /// give ensemble.
            /// </summary>
            /// <param name="ensemble">Ensemble to create the Earth velocity vectors.</param>
            public static void GenerateEarthVectors(ref DataSet.Ensemble ensemble)
            {
                if (ensemble != null && ensemble.IsEarthVelocityAvail)
                {
                    // Create array to store all the vectors
                    ensemble.EarthVelocityData.IsVelocityVectorAvail = true;
                    ensemble.EarthVelocityData.VelocityVectors = new VelocityVector[ensemble.EarthVelocityData.NumElements];

                    // Create a vector for each bin
                    for (int bin = 0; bin < ensemble.EarthVelocityData.NumElements; bin++)
                    {
                        // Create the object
                        ensemble.EarthVelocityData.VelocityVectors[bin] = new VelocityVector();

                        // Get the velocity values
                        float east = ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX];
                        float north = ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX];
                        float vertical = ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX];

                        // If any of the velocities are bad, then set bad velocities for all the velocities and move to the next bin
                        if (east == DataSet.Ensemble.BAD_VELOCITY ||
                            north == DataSet.Ensemble.BAD_VELOCITY ||
                            vertical == DataSet.Ensemble.BAD_VELOCITY)
                        {
                            ensemble.EarthVelocityData.VelocityVectors[bin].Magnitude = DataSet.Ensemble.BAD_VELOCITY;
                            ensemble.EarthVelocityData.VelocityVectors[bin].DirectionXNorth = DataSet.Ensemble.BAD_VELOCITY;
                            ensemble.EarthVelocityData.VelocityVectors[bin].DirectionYNorth = DataSet.Ensemble.BAD_VELOCITY;
                        }
                        else
                        {

                            // Calculate the magnitude of the velocity
                            //double mag = MathHelper.CalculateMagnitude(ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX],
                            //                                           ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX],
                            //                                           ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX]);
                            double mag = MathHelper.CalculateMagnitude(ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX],
                                                                        ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX],
                                                                        0);      // do not use Vertical

                            // Set the values
                            ensemble.EarthVelocityData.VelocityVectors[bin].Magnitude = Math.Abs(mag);
                            ensemble.EarthVelocityData.VelocityVectors[bin].DirectionXNorth = MathHelper.CalculateDirection(east, north);       // USE THIS VALUE FOR ADCP
                            ensemble.EarthVelocityData.VelocityVectors[bin].DirectionYNorth = MathHelper.CalculateDirection(north, east);
                        }
                    }
                }
            }

            /// <summary>
            /// Generate the Instrument Velocity vector and store it to the
            /// give ensemble.
            /// </summary>
            /// <param name="ensemble">Ensemble to create the Instrument velocity vectors.</param>
            public static void GenerateInstrumentVectors(ref DataSet.Ensemble ensemble)
            {
                if (ensemble != null && ensemble.IsInstrumentVelocityAvail)
                {
                    // Create array to store all the vectors
                    ensemble.InstrumentVelocityData.IsVelocityVectorAvail = true;
                    ensemble.InstrumentVelocityData.VelocityVectors = new VelocityVector[ensemble.InstrumentVelocityData.NumElements];

                    // Create a vector for each bin
                    for (int bin = 0; bin < ensemble.InstrumentVelocityData.NumElements; bin++)
                    {
                        // Create the object
                        ensemble.InstrumentVelocityData.VelocityVectors[bin] = new VelocityVector();

                        float east = DataSet.Ensemble.BAD_VELOCITY;
                        float north = DataSet.Ensemble.BAD_VELOCITY;
                        float vertical = DataSet.Ensemble.BAD_VELOCITY;

                        // Get the velocity values
                        if (ensemble.EnsembleData.NumBeams > 0)
                        {
                            east = ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX];
                        }

                        if (ensemble.EnsembleData.NumBeams > 1)
                        {
                            north = ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX];
                        }

                        if (ensemble.EnsembleData.NumBeams > 2)
                        {
                            vertical = ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX];
                        }

                        // If any of the velocities are bad, then set bad velocities for all the velocities and move to the next bin
                        if (east == DataSet.Ensemble.BAD_VELOCITY ||
                            north == DataSet.Ensemble.BAD_VELOCITY)
                        {
                            ensemble.InstrumentVelocityData.VelocityVectors[bin].Magnitude = DataSet.Ensemble.BAD_VELOCITY;
                            ensemble.InstrumentVelocityData.VelocityVectors[bin].DirectionXNorth = DataSet.Ensemble.BAD_VELOCITY;
                            ensemble.InstrumentVelocityData.VelocityVectors[bin].DirectionYNorth = DataSet.Ensemble.BAD_VELOCITY;
                        }
                        else
                        {

                            // Calculate the magnitude of the velocity
                            //double mag = MathHelper.CalculateMagnitude(ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX],
                            //                                           ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX],
                            //                                           ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX]);
                            double mag = MathHelper.CalculateMagnitude(ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX],
                                                                        ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX],
                                                                        0);      // do not use Vertical

                            // Set the values
                            ensemble.InstrumentVelocityData.VelocityVectors[bin].Magnitude = Math.Abs(mag);
                            ensemble.InstrumentVelocityData.VelocityVectors[bin].DirectionXNorth = MathHelper.CalculateDirection(east, north);
                            ensemble.InstrumentVelocityData.VelocityVectors[bin].DirectionYNorth = MathHelper.CalculateDirection(north, east);
                        }
                    }
                }
            }

            /// <summary>
            /// Generate the Ship Velocity vector and store it to the
            /// give ensemble.
            /// </summary>
            /// <param name="ensemble">Ensemble to create the Ship velocity vectors.</param>
            public static void GenerateShipVectors(ref DataSet.Ensemble ensemble)
            {
                if (ensemble != null && ensemble.IsShipVelocityAvail)
                {
                    // Create array to store all the vectors
                    ensemble.ShipVelocityData.IsVelocityVectorAvail = true;
                    ensemble.ShipVelocityData.VelocityVectors = new VelocityVector[ensemble.InstrumentVelocityData.NumElements];

                    // Create a vector for each bin
                    for (int bin = 0; bin < ensemble.ShipVelocityData.NumElements; bin++)
                    {
                        // Create the object
                        ensemble.ShipVelocityData.VelocityVectors[bin] = new VelocityVector();

                        float east = DataSet.Ensemble.BAD_VELOCITY;
                        float north = DataSet.Ensemble.BAD_VELOCITY;
                        float vertical = DataSet.Ensemble.BAD_VELOCITY;

                        // Get the velocity values
                        if (ensemble.EnsembleData.NumBeams > 0)
                        {
                            east = ensemble.ShipVelocityData.ShipVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX];
                        }

                        if (ensemble.EnsembleData.NumBeams > 1)
                        {
                            north = ensemble.ShipVelocityData.ShipVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX];
                        }

                        if (ensemble.EnsembleData.NumBeams > 2)
                        {
                            vertical = ensemble.ShipVelocityData.ShipVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX];
                        }

                        // If any of the velocities are bad, then set bad velocities for all the velocities and move to the next bin
                        if (east == DataSet.Ensemble.BAD_VELOCITY ||
                            north == DataSet.Ensemble.BAD_VELOCITY)
                        {
                            ensemble.ShipVelocityData.VelocityVectors[bin].Magnitude = DataSet.Ensemble.BAD_VELOCITY;
                            ensemble.ShipVelocityData.VelocityVectors[bin].DirectionXNorth = DataSet.Ensemble.BAD_VELOCITY;
                            ensemble.ShipVelocityData.VelocityVectors[bin].DirectionYNorth = DataSet.Ensemble.BAD_VELOCITY;
                        }
                        else
                        {

                            // Calculate the magnitude of the velocity
                            //double mag = MathHelper.CalculateMagnitude(ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX],
                            //                                           ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX],
                            //                                           ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX]);
                            double mag = MathHelper.CalculateMagnitude(ensemble.ShipVelocityData.ShipVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX],
                                                                        ensemble.ShipVelocityData.ShipVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX],
                                                                        0);      // do not use Vertical

                            // Set the values
                            ensemble.ShipVelocityData.VelocityVectors[bin].Magnitude = Math.Abs(mag);
                            ensemble.ShipVelocityData.VelocityVectors[bin].DirectionXNorth = MathHelper.CalculateDirection(east, north);
                            ensemble.ShipVelocityData.VelocityVectors[bin].DirectionYNorth = MathHelper.CalculateDirection(north, east);
                        }
                    }
                }
            }
            /// <summary>
            /// Create an struct to hold bin vectors for the amplitude data.
            /// Average the amplitude value for each bin and store as the magnitude value.
            /// </summary>
            /// <param name="ensemble">Ensemble to generate vector values.</param>
            /// <returns>Vectors for each bin in ensemble.</returns>
            public static DataSet.EnsembleVelocityVectors GenerateAmplitudeVectors(DataSet.Ensemble ensemble)
            {
                RTI.DataSet.VelocityVector[] vv = null;

                if (ensemble.IsAmplitudeAvail)
                {
                    // Create Velocity Vector with averaged amplitude data
                    vv = new RTI.DataSet.VelocityVector[ensemble.AmplitudeData.NumElements];

                    // Create a vector for each bin
                    // Take the average of the amplitude for the bin value
                    for (int bin = 0; bin < ensemble.EarthVelocityData.NumElements; bin++)
                    {
                        // Get the average for each bin
                        float avg = 0;
                        int count = 0;
                        if (ensemble.AmplitudeData.AmplitudeData[bin, DataSet.Ensemble.BEAM_0_INDEX] != DataSet.Ensemble.BAD_VELOCITY) { avg += ensemble.AmplitudeData.AmplitudeData[bin, DataSet.Ensemble.BEAM_0_INDEX]; count++; }
                        if (ensemble.AmplitudeData.AmplitudeData[bin, DataSet.Ensemble.BEAM_1_INDEX] != DataSet.Ensemble.BAD_VELOCITY) { avg += ensemble.AmplitudeData.AmplitudeData[bin, DataSet.Ensemble.BEAM_1_INDEX]; count++; }
                        if (ensemble.AmplitudeData.AmplitudeData[bin, DataSet.Ensemble.BEAM_2_INDEX] != DataSet.Ensemble.BAD_VELOCITY) { avg += ensemble.AmplitudeData.AmplitudeData[bin, DataSet.Ensemble.BEAM_2_INDEX]; count++; }
                        if (ensemble.AmplitudeData.AmplitudeData[bin, DataSet.Ensemble.BEAM_3_INDEX] != DataSet.Ensemble.BAD_VELOCITY) { avg += ensemble.AmplitudeData.AmplitudeData[bin, DataSet.Ensemble.BEAM_3_INDEX]; count++; }

                        // Ensure values were found
                        if (count > 0)
                        {
                            avg /= count;
                        }

                        vv[bin] = new VelocityVector();
                        vv[bin].Magnitude = avg;
                        vv[bin].DirectionXNorth = 0;
                        vv[bin].DirectionYNorth = 0;
                    }

                }


                // Create struct to hold the data
                DataSet.EnsembleVelocityVectors ensVec = new DataSet.EnsembleVelocityVectors();
                ensVec.Id = ensemble.EnsembleData.UniqueId;

                if (vv != null)
                {
                    ensVec.Vectors = vv;
                }
                else
                {
                    // Put an BAD velocity entry
                    vv = new DataSet.VelocityVector[1];
                    vv[0].Magnitude = DataSet.Ensemble.BAD_VELOCITY;
                    vv[0].DirectionXNorth = DataSet.Ensemble.BAD_VELOCITY;
                    vv[0].DirectionYNorth = DataSet.Ensemble.BAD_VELOCITY;

                    ensVec.Vectors = vv;
                }

                return ensVec;
            }
        }
    }

}