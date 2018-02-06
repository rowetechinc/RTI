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
 * 03/12/2014      RC          2.21.4     Initial coding
 * 04/16/2014      RC          2.21.4     Fixed code to handle vertical beams.
 * 
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// PD0 Velocity Data Type.
    /// </summary>
    public class Pd0Velocity : Pd0DataType
    {
        #region Variable

        /// <summary>
        /// LSB for the ID for the PD0 Velocity data type.
        /// </summary>
        public const byte ID_LSB = 0x00;

        /// <summary>
        /// MSB for the ID for the PD0 Velocity data type.
        /// </summary>
        public const byte ID_MSB = 0x01;

        /// <summary>
        /// Number of bytes in a depth cell.
        /// 4 Beams per depth cell.
        /// 2 Bytes per beam.
        /// </summary>
        public const int BYTES_PER_DEPTHCELL = 8;

        /// <summary>
        /// Number of bytes for the header.
        /// The LSB and MSB.
        /// </summary>
        public const int BYTES_PER_HEADER = 2;

        #endregion

        #region Properties

        /// <summary>
        /// Velocity values.
        /// Velocities are stored as mm/s.
        /// 
        /// The Workhorse packs velocity data for each depth cell of each beam into a
        /// two-byte, two’s-complement integer [-32768, 32767] with the LSB sent
        /// first. The Workhorse scales velocity data in millimeters per second (mm/s).
        /// A value of –32768 (8000h) indicates bad velocity values.
        /// 
        /// All velocities are relative based on a stationary instrument. To obtain absolute
        /// velocities, algebraically remove the velocity of the instrument. For example,
        /// 
        /// RELATIVE WATER CURRENT VELOCITY:     EAST 650 mm/s
        /// INSTRUMENT VELOCITY            : (-) EAST 600 mm/s
        /// -----------------------------------------------------
        /// ABSOLUTE WATER VELOCITY        :     EAST 50 mm/s
        /// 
        /// The setting of the EX-command (Coordinate Transformation) determines
        /// how the Workhorse references the velocity data as shown below.
        /// 
        /// EX-CMD       COORD SYS     VEL 1       VEL 2       VEL 3       VEL
        /// -------------------------------------------------------------------------
        /// xxx00xxx     BEAM          TO BEAM 1   TO BEAM 2   TO BEAM 3   TO BEAM 4
        /// xxx01xxx     INST          Bm1-Bm2     Bm4-Bm3     TO XDUCER   ERR VEL
        /// xxx10xxx     SHIP          PRT-STBD    AFT-FWD     TO SURFACE  ERR VEL
        /// xxx11xxx     EARTH         TO EAST     TO NORTH    TO SURFACE  ERR VEL
        /// 
        /// POSITIVE VALUES INDICATE WATER MOVEMENT
        /// </summary>
        public short[,] Velocities { get; set; }

        #endregion

        /// <summary>
        /// Initialize the data type.
        /// </summary>
        public Pd0Velocity()
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.Velocity)
        {

        }

        /// <summary>
        /// Initialize the data type.
        /// </summary>
        /// <param name="data">Binary data to decode.</param>
        public Pd0Velocity(byte[] data)
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.Velocity)
        {
            Decode(data);
        }

        /// <summary>
        /// Initialize the data type.
        /// </summary>
        /// <param name="data">Binary data to decode.</param>
        /// <param name="offset">Offset in the binary data.</param>
        public Pd0Velocity(byte[] data, ushort offset)
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.Velocity)
        {
            this.Offset = offset;
            Decode(data);
        }

        /// <summary>
        /// Initialize the data type.
        /// </summary>
        /// <param name="numDepthCells">Number of depth cells.</param>
        public Pd0Velocity(int numDepthCells)
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.Velocity)
        {
            // Create the array to hold all the depth cells
            Velocities = new short[numDepthCells, 4];
        }

        /// <summary>
        /// Initialize the data type.
        /// </summary>
        /// <param name="beamVelocity">RTI Beam Velocity.</param>
        public Pd0Velocity(DataSet.BeamVelocityDataSet beamVelocity)
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.Velocity)
        {
            // Decode Beam Velocity
            DecodeRtiEnsemble(beamVelocity);
        }

        /// <summary>
        /// Initialize the data type.
        /// </summary>
        /// <param name="instrumentVelocity">RTI Instrument Velocity.</param>
        public Pd0Velocity(DataSet.InstrumentVelocityDataSet instrumentVelocity)
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.Velocity)
        {
            // Decode Beam Velocity
            DecodeRtiEnsemble(instrumentVelocity);
        }

        /// <summary>
        /// Initialize the data type.
        /// </summary>
        /// <param name="earthVelocity">RTI Earth Velocity.</param>
        public Pd0Velocity(DataSet.EarthVelocityDataSet earthVelocity)
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.Velocity)
        {
            // Decode Beam Velocity
            DecodeRtiEnsemble(earthVelocity);
        }

        /// <summary>
        /// Encode the data type to binary PD0 format.
        /// </summary>
        /// <returns>Binary PD0 format.</returns>
        public override byte[] Encode()
        {
            // Start with the first 2 bytes for the header
            // Then determine how many depth cells exist
            int numBytes = BYTES_PER_HEADER;
            numBytes += (Velocities.GetLength(0) * BYTES_PER_DEPTHCELL);

            byte[] data = new byte[numBytes];

            // Header
            data[0] = ID_LSB;
            data[1] = ID_MSB;

            // Set the start location
            int loc = 2;

            for (int x = 0; x < Velocities.GetLength(0); x++)
            {
                // Beam 0
                byte b0Lsb, b0Msb;
                MathHelper.LsbMsbShort(Velocities[x, 0], out b0Lsb, out b0Msb);
                
                // Beam 1
                byte b1Lsb, b1Msb;
                MathHelper.LsbMsbShort(Velocities[x, 1], out b1Lsb, out b1Msb);

                // Beam 2
                byte b2Lsb, b2Msb;
                MathHelper.LsbMsbShort(Velocities[x, 2], out b2Lsb, out b2Msb);

                // Beam 3
                byte b3Lsb, b3Msb;
                MathHelper.LsbMsbShort(Velocities[x, 3], out b3Lsb, out b3Msb);

                // Add the data to the array
                data[loc++] = b0Lsb;
                data[loc++] = b0Msb;
                data[loc++] = b1Lsb;
                data[loc++] = b1Msb;
                data[loc++] = b2Lsb;
                data[loc++] = b2Msb;
                data[loc++] = b3Lsb;
                data[loc++] = b3Msb;
            }

            return data;
        }

        /// <summary>
        /// Decode the given binary PD0 data in the object.
        /// </summary>
        /// <param name="data">Binary PD0 data.</param>
        public override void Decode(byte[] data)
        {
            // Remove the first 2 bytes for the header
            // Divide by 8, because there are 8 bytes per depth cell
            // 2 bytes per beam in a depth cell.
            // 4 beams per depth cells
            int numDepthCells = (int)Math.Round(((double)(data.Length - BYTES_PER_HEADER) / BYTES_PER_DEPTHCELL));

            // Create the array to hold all the depth cells
            Velocities = new short[numDepthCells, 4];

            // Start after the header
            for (int x = 2; x < data.Length; x += BYTES_PER_DEPTHCELL)
            {
                int depthCell = (int)Math.Round((double)(x / BYTES_PER_DEPTHCELL));

                Velocities[depthCell, 0] = MathHelper.LsbMsbShort(data[x + 0], data[x + 1]);
                Velocities[depthCell, 1] = MathHelper.LsbMsbShort(data[x + 2], data[x + 3]);
                Velocities[depthCell, 2] = MathHelper.LsbMsbShort(data[x + 4], data[x + 5]);
                Velocities[depthCell, 3] = MathHelper.LsbMsbShort(data[x + 6], data[x + 7]);
            }
        }

        /// <summary>
        /// Get the velocity from the array based off the depth cell and
        /// beam given.
        /// </summary>
        /// <param name="depthCell">Depth cell.</param>
        /// <param name="beam">Beam.</param>
        /// <returns>Velocity.</returns>
        public short GetVelocity(int depthCell, int beam)
        {
            return Velocities[depthCell, beam];
        }

        /// <summary>
        /// Get the number of depth cells.
        /// </summary>
        /// <returns>Number of depth cells.</returns>
        public int GetNumDepthCells()
        {
            return Velocities.GetLength(0);
        }

        /// <summary>
        /// Get the number of bytes in the data type.
        /// This is based off the number of depth cells and
        /// bytes per depth cells.
        /// </summary>
        /// <returns>Number of bytes for the data type.</returns>
        public override int GetDataTypeSize()
        {
            // Start with the first 2 bytes for the header
            // Then determine how many depth cells exist
            int numBytes = BYTES_PER_HEADER;
            numBytes += (Velocities.GetLength(0) * BYTES_PER_DEPTHCELL);

            return numBytes;
        }

        /// <summary>
        /// Get the size of a Velocity Data type based
        /// off the number of depth cells.
        /// </summary>
        /// <param name="numDepthCells">Number of depth cells.</param>
        /// <returns>Number of byte in a Velocity Data Type.</returns>
        public static int GetVelocitySize(int numDepthCells)
        {
            return 2 + (4 * 2 * numDepthCells);
        }

        #region RTI Ensemble

        /// <summary>
        /// Convert the RTI Beam Velocity data set to the PD0 Velocity data type.
        /// </summary>
        /// <param name="vel">RTI Beam Velocity data set.</param>
        public void DecodeRtiEnsemble(DataSet.BeamVelocityDataSet vel)
        {
            if (vel.BeamVelocityData != null)
            {
                Velocities = new short[vel.BeamVelocityData.GetLength(0), PD0.NUM_BEAMS];

                for (int bin = 0; bin < vel.BeamVelocityData.GetLength(0); bin++)
                {
                    // 4 Beam System
                    if (vel.BeamVelocityData.GetLength(1) >= PD0.NUM_BEAMS)
                    {
                        for (int beam = 0; beam < vel.BeamVelocityData.GetLength(1); beam++)
                        {
                            // beam order 3,2,0,1; XYZ order 1,0,-2,3, ENU order 0,1,2,3
                            int newBeam = 0;
                            switch (beam)
                            {
                                case 0:
                                    newBeam = 3;
                                    break;
                                case 1:
                                    newBeam = 2;
                                    break;
                                case 2:
                                    newBeam = 0;
                                    break;
                                case 3:
                                    newBeam = 1;
                                    break;
                                default:
                                    break;
                            }

                            // Check for bad velocity
                            if (vel.BeamVelocityData[bin, beam] != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                Velocities[bin, beam] = (short)(Math.Round(vel.BeamVelocityData[bin, newBeam] * 1000.0));   // mm/s to m/s 
                            }
                            else
                            {
                                // Bad velocity
                                Velocities[bin, beam] = PD0.BAD_VELOCITY;
                            }
                        }
                    }
                    // Vertical Beam
                    else if (vel.BeamVelocityData.GetLength(1) == 1)
                    {
                        // Check for bad velocity
                        if (vel.BeamVelocityData[bin, 0] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            Velocities[bin, 0] = (short)(Math.Round(vel.BeamVelocityData[bin, 0] * 1000.0));   // mm/s to m/s 
                        }
                        else
                        {
                            // Bad velocity
                            Velocities[bin, 0] = PD0.BAD_VELOCITY;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Convert the RTI Earth Velocity data set to the PD0 Velocity data type.
        /// </summary>
        /// <param name="vel">RTI Earth Velocity data set.</param>
        public void DecodeRtiEnsemble(DataSet.EarthVelocityDataSet vel)
        {
            if (vel.EarthVelocityData != null)
            {
                Velocities = new short[vel.EarthVelocityData.GetLength(0), PD0.NUM_BEAMS];

                for (int bin = 0; bin < vel.EarthVelocityData.GetLength(0); bin++)
                {
                    // 4 Beam System
                    if (vel.EarthVelocityData.GetLength(1) >= PD0.NUM_BEAMS)
                    {
                        for (int beam = 0; beam < vel.EarthVelocityData.GetLength(1); beam++)
                        {
                            // beam order 3,2,0,1; XYZ order 1,0,-2,3, ENU order 0,1,2,3
                            // Check for bad velocity
                            if (vel.EarthVelocityData[bin, beam] != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                Velocities[bin, beam] = (short)(Math.Round(vel.EarthVelocityData[bin, beam] * 1000.0));   // mm/s to m/s 
                            }
                            else
                            {
                                // Bad velocity
                                Velocities[bin, beam] = PD0.BAD_VELOCITY;
                            }
                        }
                    }
                    // Vertical Beam
                    else if (vel.EarthVelocityData.GetLength(1) == 1)
                    {
                        // Check for bad velocity
                        if (vel.EarthVelocityData[bin, 0] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            Velocities[bin, 0] = (short)(Math.Round(vel.EarthVelocityData[bin, 0] * 1000.0));   // mm/s to m/s 
                        }
                        else
                        {
                            // Bad velocity
                            Velocities[bin, 0] = PD0.BAD_VELOCITY;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Convert the RTI Instrument Velocity data set to the PD0 Velocity data type.
        /// </summary>
        /// <param name="vel">RTI Instrument Velocity data set.</param>
        public void DecodeRtiEnsemble(DataSet.InstrumentVelocityDataSet vel)
        {
            if (vel.InstrumentVelocityData != null)
            {
                Velocities = new short[vel.InstrumentVelocityData.GetLength(0), PD0.NUM_BEAMS];

                for (int bin = 0; bin < vel.InstrumentVelocityData.GetLength(0); bin++)
                {
                    // 4 Beam System
                    if (vel.InstrumentVelocityData.GetLength(1) >= PD0.NUM_BEAMS)
                    {
                        for (int beam = 0; beam < vel.InstrumentVelocityData.GetLength(1); beam++)
                        {
                            // beam order 3,2,0,1; XYZ order 1,0,-2,3, ENU order 0,1,2,3
                            int sign = 1;
                            int newBeam = 0;
                            switch (beam)
                            {
                                case 0:
                                    newBeam = 1;
                                    sign = 1;
                                    break;
                                case 1:
                                    newBeam = 0;
                                    sign = 1;
                                    break;
                                case 2:
                                    newBeam = 2;
                                    sign = -1;
                                    break;
                                case 3:
                                    newBeam = 3;
                                    sign = 1;
                                    break;
                                default:
                                    break;
                            }

                            // Check for bad velocity
                            if (vel.InstrumentVelocityData[bin, beam] != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                Velocities[bin, beam] = (short)(Math.Round(vel.InstrumentVelocityData[bin, newBeam] * 1000.0 * sign));   // mm/s
                            }
                            else
                            {
                                // Bad velocity
                                Velocities[bin, beam] = PD0.BAD_VELOCITY;
                            }
                        }
                    }
                    // Vertical Beam
                    else if (vel.InstrumentVelocityData.GetLength(1) == 1)
                    {
                        // Check for bad velocity
                        if (vel.InstrumentVelocityData[bin, 0] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            Velocities[bin, 0] = (short)(Math.Round(vel.InstrumentVelocityData[bin, 0] * 1000.0));   // mm/s
                        }
                        else
                        {
                            // Bad velocity
                            Velocities[bin, 0] = PD0.BAD_VELOCITY;
                        }
                    }
                }
            }
        }

        #endregion
    }
}
