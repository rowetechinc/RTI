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
    /// Percent Good Data Type.
    /// </summary>
    public class Pd0PercentGood : Pd0DataType
    {
        #region Variable

        /// <summary>
        /// LSB for the ID for the PD0 Percent Good data type.
        /// </summary>
        public const byte ID_LSB = 0x00;

        /// <summary>
        /// MSB for the ID for the PD0 Percent Good data type.
        /// </summary>
        public const byte ID_MSB = 0x04;

        /// <summary>
        /// Number of bytes in a depth cell.
        /// 4 Beams per depth cell.
        /// 1 Bytes per beam.
        /// </summary>
        public const int BYTES_PER_DEPTHCELL = 4;

        /// <summary>
        /// Number of bytes for the header.
        /// The LSB and MSB.
        /// </summary>
        public const int BYTES_PER_HEADER = 2;

        #endregion

        #region Properties

        /// <summary>
        /// The percent-good data field is a data-quality indicator that reports the percentage
        /// (0 to 100) of good data collected for each depth cell of the velocity
        /// profile. The setting of the EX-command (Coordinate Transformation) determines
        /// how the Workhorse references percent-good data as shown below.
        /// 
        /// 
        /// EX-Command     Coord. Sys    Velocity 1     Velocity 2           Velocity 3           Velocity 4
        ///                                            Percentage Of Good Pings For:
        ///                              Beam 1         BEAM 2               BEAM 3               BEAM 4
        /// xxx00xxx       Beam                              Percentage Of:
        /// xxx01xxx       Inst     3-Beam Trans-       Transformations      More than one        4-Beam
        /// xxx10xxx       Ship     formations(note1)    Rejected(note2)     beam bad in bin      Transformation
        /// xxx11xxx       Earth
        ///
        /// 1. Because profile data did not exceed correlation threshold (WC).
        /// 2. Because the error velocity threshold (WE) was exceeded.
        /// 
        /// At the start of the velocity profile, the backscatter echo strength is typically
        /// high on all four beams. Under this condition, the Workhorse uses all four
        /// beams to calculate the orthogonal and error velocities. As the echo returns
        /// from far away depth cells, echo intensity decreases. At some point, the
        /// echo will be weak enough on any given beam to cause the Workhorse to
        /// reject some of its depth cell data. This causes the Workhorse to calculate
        /// velocities with three beams instead of four beams. When the Workhorse
        /// does 3-beam solutions, it stops calculating the error velocity because it
        /// needs four beams to do this. At some further depth cell, the Workhorse rejects
        /// all cell data because of the weak echo. As an example, let us assume
        /// depth cell 60 has returned the following percent-good data.
        /// 
        /// FIELD #1 = 50, FIELD #2 = 5, FIELD #3 = 0, FIELD #4 = 45
        /// 
        /// If the EX-command was set to collect velocities in BEAM coordinates, the
        /// example values show the percentage of pings having good solutions in cell
        /// 60 for each beam based on the Low Correlation Threshold (WC-command).
        /// Here, beam 1=50%, beam 2=5%, beam 3=0%, and beam 4=45%. These are
        /// not typical nor desired percentages. Typically, you would want all four
        /// beams to be about equal and greater than 25%.
        /// 
        /// On the other hand, if velocities were collected in INSTRUMENT, SHIP, or
        /// EARTH coordinates, the example values show:
        /// 
        /// FIELD 1 – Percentage of good 3-beam solutions – Shows percentage of
        /// successful velocity calculations (50%) using 3-beam solutions because the
        /// correlation threshold (WC) was not exceeded.
        /// 
        /// FIELD 2 – Percentage of transformations rejected – Shows percent of error
        /// velocity (5%) that was less than the WE-command setting. WE has a default
        /// of 5000 mm/s. This large WE setting effectively prevents the Workhorse
        /// from rejecting data based on error velocity.
        /// 
        /// FIELD 3 – Percentage of more than one beam bad in bin – 0% of the velocity
        /// data were rejected because not enough beams had good data.
        /// 
        /// FIELD 4 – Percentage of good 4-beam solutions – 45% of the velocity data
        /// collected during the ensemble for depth cell 60 were calculated using four
        /// beams.
        /// </summary>
        public byte[,] PercentGood { get; set; }

        #endregion

        /// <summary>
        /// Initialize the object.
        /// </summary>
        public Pd0PercentGood()
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.PercentGood)
        {

        }

        /// <summary>
        /// Initialize the object.
        /// </summary>
        /// <param name="data">Decode the data.</param>
        public Pd0PercentGood(byte[] data)
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.PercentGood)
        {
            Decode(data);
        }

        /// <summary>
        /// Initialize the object.
        /// </summary>
        /// <param name="data">Decode the data.</param>
        /// <param name="offset">Offset in the binary data.</param>
        public Pd0PercentGood(byte[] data, ushort offset)
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.PercentGood)
        {
            this.Offset = offset;
            Decode(data);
        }

        /// <summary>
        /// Initialize the object.
        /// </summary>
        /// <param name="numDepthCells">Number of depth cells.</param>
        public Pd0PercentGood(int numDepthCells)
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.PercentGood)
        {
            PercentGood = new byte[numDepthCells, 4];
        }

        /// <summary>
        /// Initialize the object.
        /// </summary>
        /// <param name="goodEarth">RTI Good Earth data set.</param>
        /// <param name="pingsPerEnsemble">Pings per Ensemble.</param>
        public Pd0PercentGood(DataSet.GoodEarthDataSet goodEarth, int pingsPerEnsemble)
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.PercentGood)
        {
            DecodeRtiEnsemble(goodEarth, pingsPerEnsemble);
        }

        /// <summary>
        /// Initialize the object.
        /// </summary>
        /// <param name="goodBeam">RTI Good Beam data set.</param>
        /// <param name="pingsPerEnsemble">Pings per Ensemble.</param>
        public Pd0PercentGood(DataSet.GoodBeamDataSet goodBeam, int pingsPerEnsemble)
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.PercentGood)
        {
            DecodeRtiEnsemble(goodBeam, pingsPerEnsemble);
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
            numBytes += (PercentGood.GetLength(0) * BYTES_PER_DEPTHCELL);

            byte[] data = new byte[numBytes];

            // Header
            data[0] = ID_LSB;
            data[1] = ID_MSB;

            // Set the start location
            int loc = 2;

            for (int x = 0; x < PercentGood.GetLength(0); x++)
            {
                // Add the data to the array
                data[loc++] = PercentGood[x, 0];
                data[loc++] = PercentGood[x, 1];
                data[loc++] = PercentGood[x, 2];
                data[loc++] = PercentGood[x, 3];
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
            PercentGood = new byte[numDepthCells, PD0.NUM_BEAMS];

            // Start after the header
            for (int x = 2; x < data.Length; x += BYTES_PER_DEPTHCELL)
            {
                int depthCell = (int)Math.Round((double)(x / BYTES_PER_DEPTHCELL));

                PercentGood[depthCell, 0] = data[x + 0];
                PercentGood[depthCell, 1] = data[x + 1];
                PercentGood[depthCell, 2] = data[x + 2];
                PercentGood[depthCell, 3] = data[x + 3];
            }
        }

        /// <summary>
        /// Get the Percent Good from the array based off the depth cell and
        /// beam given.
        /// </summary>
        /// <param name="depthCell">Depth cell.</param>
        /// <param name="beam">Beam.</param>
        /// <returns>Percent Good.</returns>
        public byte GetPercentGood(int depthCell, int beam)
        {
            return PercentGood[depthCell, beam];
        }

        /// <summary>
        /// Get the number of depth cells.
        /// </summary>
        /// <returns>Number of depth cells.</returns>
        public int GetNumDepthCells()
        {
            return PercentGood.GetLength(0);
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
            numBytes += (PercentGood.GetLength(0) * BYTES_PER_DEPTHCELL);

            return numBytes;
        }

        /// <summary>
        /// Get the size of a Percent Good Data type based
        /// off the number of depth cells.
        /// </summary>
        /// <param name="numDepthCells">Number of depth cells.</param>
        /// <returns>Number of byte in a Percent Good Data Type.</returns>
        public static int GetPercentGoodSize(int numDepthCells)
        {
            return 2 + (4 * numDepthCells);
        }

        #region RTI Ensemble

        /// <summary>
        /// Convert the RTI Good Earth data set to the PD0 Percent Good data type.
        /// </summary>
        /// <param name="goodEarth">RTI Good Earth data set.</param>
        /// <param name="pingsPerEnsemble">Pings per Ensemble.</param>
        public void DecodeRtiEnsemble(DataSet.GoodEarthDataSet goodEarth, int pingsPerEnsemble)
        {
            if (goodEarth.GoodEarthData != null)
            {
                PercentGood = new byte[goodEarth.GoodEarthData.GetLength(0), PD0.NUM_BEAMS];

                for (int bin = 0; bin < goodEarth.GoodEarthData.GetLength(0); bin++)
                {
                    // 4 Beam System
                    if (goodEarth.GoodEarthData.GetLength(1) >= PD0.NUM_BEAMS)
                    {
                        for (int beam = 0; beam < goodEarth.GoodEarthData.GetLength(1); beam++)
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

                            PercentGood[bin, beam] = (byte)(Math.Round((goodEarth.GoodEarthData[bin, newBeam] * 100.0) / pingsPerEnsemble));
                        }
                    }
                    // Vertical Beam
                    else if (goodEarth.GoodEarthData.GetLength(1) == 1)
                    {
                        PercentGood[bin, 0] = (byte)(Math.Round((goodEarth.GoodEarthData[bin, 0] * 100.0) / pingsPerEnsemble));
                    }
                }
            }
        }

        /// <summary>
        /// Convert the RTI Good Beam data set to the PD0 Percent Good data type.
        /// </summary>
        /// <param name="goodBeam">RTI Good Beam data set.</param>
        /// <param name="pingsPerEnsemble">Pings per Ensemble.</param>
        public void DecodeRtiEnsemble(DataSet.GoodBeamDataSet goodBeam, int pingsPerEnsemble)
        {
            if (goodBeam.GoodBeamData != null)
            {
                PercentGood = new byte[goodBeam.GoodBeamData.GetLength(0), PD0.NUM_BEAMS];

                for (int bin = 0; bin < goodBeam.GoodBeamData.GetLength(0); bin++)
                {
                    // 4 Beam System
                    if (goodBeam.GoodBeamData.GetLength(1) >= PD0.NUM_BEAMS)
                    {
                        for (int beam = 0; beam < goodBeam.GoodBeamData.GetLength(1); beam++)
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

                            PercentGood[bin, beam] = (byte)(Math.Round((goodBeam.GoodBeamData[bin, newBeam] * 100.0) / pingsPerEnsemble));
                        }
                    }
                    // Vertical Beam
                    if (goodBeam.GoodBeamData.GetLength(1) == 1)
                    {
                        PercentGood[bin, 0] = (byte)(Math.Round((goodBeam.GoodBeamData[bin, 0] * 100.0) / pingsPerEnsemble));
                    }
                }
            }
        }

        #endregion

    }
}
