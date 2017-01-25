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
 * 02/24/2014      RC          2.21.4     Initial coding
 * 03/27/2014      RC          2.21.4     Added constructor that data binary data.
 * 10/29/2015      RC          3.2.1      Added WaterTracking in DecodeRtiEnsemble().
 * 
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
    /// Teledyne RD Instruments PD0 format.
    /// Format based off Workhorse Command and Output Data Format March 2005.
    /// </summary>
    public class PD0
    {
        #region Enums

        /// <summary>
        /// Coordinate Transform types.
        /// </summary>
        public enum CoordinateTransforms
        {
            /// <summary>
            /// Beam Coordinate Transform.
            /// </summary>
            Coord_Beam,

            /// <summary>
            /// Instrument Coordinate Transform.
            /// </summary>
            Coord_Instrument,

            /// <summary>
            /// Ship Coordinate Transform.
            /// </summary>
            Coord_Ship,

            /// <summary>
            /// Earth Coordinate Transform.
            /// </summary>
            Coord_Earth
        }

        #endregion

        #region Variables

        /// <summary>
        /// Bad Velocity.
        /// </summary>
        public const short BAD_VELOCITY = -32768;

        /// <summary>
        /// BAD Amplitude.
        /// </summary>
        public const byte BAD_AMPLITUDE = 255;

        /// <summary>
        /// BAD Correlation.
        /// </summary>
        public const byte BAD_CORRELATION = 255;

        /// <summary>
        /// BAD Percent Good.
        /// </summary>
        public const byte BAD_PERCENT_GOOD = 255;

        /// <summary>
        /// Number of bytes for the checksum.
        /// </summary>
        public const int CHECKSUM_NUM_BYTE = 2;

        /// <summary>
        /// Number of beams.
        /// </summary>
        public const int NUM_BEAMS = 4;

        #endregion

        #region Properties

        #region Data Types

        /// <summary>
        /// PD0 Header.
        /// </summary>
        public Pd0Header Header { get; set; }

        /// <summary>
        /// PD0 Fixed Leader.
        /// </summary>
        public Pd0FixedLeader FixedLeader
        {
            get
            {
                return Header.GetFixedLeader();
            }
        }

        /// <summary>
        /// PD0 Variable Leader.
        /// </summary>
        public Pd0VariableLeader VariableLeader 
        { 
            get
            {
                return Header.GetVariableLeader();
            }
        }

        /// <summary>
        /// PD0 Velocity.
        /// </summary>
        public Pd0Velocity Velocity
        {
            get
            {
                return Header.GetVelocity();
            }
        }


        /// <summary>
        /// PD0 Correlation.
        /// </summary>
        public Pd0Correlation Correlation
        {
            get
            {
                return Header.GetCorrelation();
            }
        }

        /// <summary>
        /// PD0 Echo Intensity.
        /// </summary>
        public Pd0EchoIntensity EchoIntensity
        {
            get
            {
                return Header.GetEchoIntensity();
            }
        }

        /// <summary>
        /// PD0 Percent Good.
        /// </summary>
        public Pd0PercentGood PercentGood
        {
            get
            {
                return Header.GetPercentGood();
            }
        }

        /// <summary>
        /// PD0 Bottom Track.
        /// </summary>
        public Pd0BottomTrack BottomTrack
        {
            get
            {
                return Header.GetBottomTrack();
            }
        }

        #endregion

        #region Data Type Exist

        /// <summary>
        /// Check if the Bottom Track Data Type exist.
        /// </summary>
        public bool IsBottomTrackExist
        {
            get
            {
                if(Header.DataTypes.ContainsKey(Pd0ID.Pd0Types.BottomTrack) && BottomTrack != null)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Check if the Correlation Data Type exist.
        /// </summary>
        public bool IsCorrelationExist
        {
            get
            {
                if (Header.DataTypes.ContainsKey(Pd0ID.Pd0Types.Correlation) && Correlation != null)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Check if the Echo Intensity Data Type exist.
        /// </summary>
        public bool IsEchoIntensityExist
        {
            get
            {
                if (Header.DataTypes.ContainsKey(Pd0ID.Pd0Types.EchoIntensity) && EchoIntensity != null)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Check if the Percent Good Data Type exist.
        /// </summary>
        public bool IsPercentGoodExist
        {
            get
            {
                if (Header.DataTypes.ContainsKey(Pd0ID.Pd0Types.PercentGood) && PercentGood != null)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Check if the Velocity Data Type exist.
        /// </summary>
        public bool IsVelocityExist
        {
            get
            {
                if (Header.DataTypes.ContainsKey(Pd0ID.Pd0Types.Velocity) && Velocity != null)
                {
                    return true;
                }

                return false;
            }
        }

        #endregion

        /// <summary>
        /// Reserved LSB.
        /// </summary>
        public byte ReservedLsb { get; set; }

        /// <summary>
        /// Reserved MSB.
        /// </summary>
        public byte ReservedMsb { get; set; }

        /// <summary>
        /// Checksum LSB.
        /// </summary>
        public byte ChecksumLsb { get; set; }

        /// <summary>
        /// Checksum MSB.
        /// </summary>
        public byte ChecksumMsb { get; set; }

        #endregion

        /// <summary>
        /// Initialize the object.
        /// </summary>
        public PD0()
        {
            // Create the header
            // All PD0 Ensembles have a header
            Header = new Pd0Header();

            // All PD0 ensembles have a Fixed Leader
            AddDataType(new Pd0FixedLeader());

            // All PD0 ensembles have a Variable Leader
            AddDataType(new Pd0VariableLeader());
        }

        /// <summary>
        /// Create a PD0 ensemble with the given binary data.
        /// </summary>
        /// <param name="data">Data to decode into a PD0 ensemble.</param>
        public PD0(byte[] data)
        {
            // Create the header
            // All PD0 Ensembles have a header
            Header = new Pd0Header();

            // All PD0 ensembles have a Fixed Leader
            AddDataType(new Pd0FixedLeader());

            // All PD0 ensembles have a Variable Leader
            AddDataType(new Pd0VariableLeader());

            Decode(data);
        }

        /// <summary>
        /// Initialize the object.
        /// </summary>
        /// <param name="ensemble">RTI ensemble.</param>
        /// <param name="xform">Coordinate transform.</param>
        public PD0(DataSet.Ensemble ensemble, CoordinateTransforms xform)
        {
            // Create the header
            // All PD0 Ensembles have a header
            Header = new Pd0Header();

            // All PD0 ensembles have a Fixed Leader
            AddDataType(new Pd0FixedLeader());

            // All PD0 ensembles have a Variable Leader
            AddDataType(new Pd0VariableLeader());

            // Set the ensemble based off the RTI ensemble given
            DecodeRtiEnsemble(ensemble, xform);
        }

        /// <summary>
        /// Added the data type to the ensemble.
        /// </summary>
        /// <param name="dataType">Data type.</param>
        public void AddDataType(Pd0DataType dataType)
        {
            Header.AddDataType(dataType);
        }


        /// <summary>
        /// Encode the data type to binary PD0 format.
        /// </summary>
        /// <returns>Binary PD0 format.</returns>
        public byte[] Encode()
        {
            int size = Header.NumberOfBytes + 2;                    // Add 2 bytes for the checksum
            byte[] data = new byte[size];

            // Header
            byte[] header = Header.Encode();
            Buffer.BlockCopy(header, 0, data, 0, header.Length);

            // Add all the datatypes
            foreach (var dt in Header.DataTypes)
            {
                // Set each data type
                byte[] encode = dt.Value.Encode();
                Buffer.BlockCopy(encode, 0, data, dt.Value.Offset, encode.Length);
            }

            data[size - 4] = 0x0f;                                  // Spare LSB
            data[size - 3] = 0x0f;                                  // Spare MSB

            ushort checksum = CalculateChecksum(data, size-2);      // Calculate Checksum
            byte csLsb, csMsb;
            MathHelper.LsbMsbUShort(checksum, out csLsb, out csMsb);
            data[size - 2] = csLsb;
            data[size - 1] = csMsb;

            return data;
        }

        /// <summary>
        /// Decode the binary data.  Determine which data types exist and 
        /// add them to the ensemble.
        /// </summary>
        /// <param name="data">PD0 binary data to decode.</param>
        public void Decode(byte[] data)
        {
            // Check for the minimum number of bytes
            if (data.Length > Header.NumberOfBytes)
            {
                if(data[0] == Header.ID.ID_MSB && data[1] == Header.ID.ID_MSB)
                {
                    // Get the number of data types to determine the size of the header
                    byte numDataTypes = data[5];
                    ushort ensSize = (ushort)(MathHelper.LsbMsbUShort(data[2], data[3]) + 2);

                    // Determine of the size of the header
                    //int headerSize = Pd0Header.GetHeaderSize(numDataTypes);

                    //byte[] header = new byte[headerSize];
                    //Buffer.BlockCopy(data, 0, header, 0, headerSize);

                    // Add the header data
                    //Header.Decode(header);

                    byte[] ens = new byte[ensSize];
                    Buffer.BlockCopy(data, 0, ens, 0, ensSize);

                    Header.Decode(ens);
                }
            }
        }

        #region Checksum

        /// <summary>
        /// Calculate the checksum for the ensemble.
        /// This will be all the bytes except the last 2 bytes 
        /// in the ensemble which are the checksum value.
        /// </summary>
        /// <param name="data">Ensemble data.</param>
        /// <param name="length">Lenght of data to calculate the checksum.</param>
        /// <returns>Checksum value.</returns>
        public static ushort CalculateChecksum(byte[] data, int length)
        {
            ushort crc = 0;

            for (int x = 0; x < length; x++)
            {
                crc += data[x];
            }

            return crc;
        }

        #endregion

        #region RTI Ensemble

        /// <summary>
        /// Decode the RTI Ensemble to a PD0 ensemble.
        /// </summary>
        /// <param name="ensemble">RTI Ensemble.</param>
        /// <param name="xform">Coordinate Transform to use.</param>
        public void DecodeRtiEnsemble(DataSet.Ensemble ensemble, CoordinateTransforms xform)
        {
            // Add Fixed Leader and Variable Leader
            FixedLeader.DecodeRtiEnsemble(ensemble.EnsembleData, ensemble.AncillaryData, ensemble.SystemSetupData, xform);
            VariableLeader.DecodeRtiEnsemble(ensemble.EnsembleData, ensemble.AncillaryData);

            // Correlation
            if (ensemble.IsCorrelationAvail)
            {
                float numCodeRepeats = 1.0f;
                if (ensemble.IsSystemSetupAvail)
                {
                    numCodeRepeats = ensemble.SystemSetupData.WpRepeatN;
                }

                this.AddDataType(new Pd0Correlation(ensemble.CorrelationData, numCodeRepeats));
            }

            // Amplitude
            if (ensemble.IsAmplitudeAvail)
            {
                this.AddDataType(new Pd0EchoIntensity(ensemble.AmplitudeData));
            }

            // Bottom Track
            if (ensemble.IsBottomTrackAvail)
            {
                this.AddDataType(new Pd0BottomTrack(ensemble.BottomTrackData, xform));

                // Add Water Track data
                switch (xform)
                {
                    case RTI.PD0.CoordinateTransforms.Coord_Earth:
                        ensemble.EarthWaterMassData = new DataSet.EarthWaterMassDataSet();
                        ensemble.IsEarthWaterMassAvail = true;
                        ensemble.EarthWaterMassData.WaterMassDepthLayer = ((this.BottomTrack.BtRefLayerNear + this.BottomTrack.BtRefLayerFar) / 2.0f) / 10.0f;  // Divide by 10 to convert DM to M

                        // Set velocities and check for bad velocities
                        if (this.BottomTrack.BtRefLayerVelocityBeam0 == PD0.BAD_VELOCITY)
                        {
                            ensemble.EarthWaterMassData.VelocityEast = BAD_VELOCITY;
                        }
                        else
                        {
                            ensemble.EarthWaterMassData.VelocityEast = this.BottomTrack.BtRefLayerVelocityBeam0;
                        }

                        if (this.BottomTrack.BtRefLayerVelocityBeam1 == PD0.BAD_VELOCITY)
                        {
                            ensemble.EarthWaterMassData.VelocityNorth = BAD_VELOCITY;
                        }
                        else
                        {
                            ensemble.EarthWaterMassData.VelocityNorth = this.BottomTrack.BtRefLayerVelocityBeam1;
                        }

                        if (this.BottomTrack.BtRefLayerVelocityBeam2 == PD0.BAD_VELOCITY)
                        {
                            ensemble.EarthWaterMassData.VelocityVertical = BAD_VELOCITY;
                        }
                        else
                        {
                            ensemble.EarthWaterMassData.VelocityVertical = this.BottomTrack.BtRefLayerVelocityBeam2;
                        }
                        break;
                    case RTI.PD0.CoordinateTransforms.Coord_Instrument:
                    case RTI.PD0.CoordinateTransforms.Coord_Ship:
                    case RTI.PD0.CoordinateTransforms.Coord_Beam:
                        ensemble.InstrumentWaterMassData = new DataSet.InstrumentWaterMassDataSet();
                        ensemble.IsInstrumentWaterMassAvail = true;
                        ensemble.InstrumentWaterMassData.WaterMassDepthLayer = ((this.BottomTrack.BtRefLayerNear + this.BottomTrack.BtRefLayerFar) / 2.0f) / 10.0f;  // Divide by 10 to convert DM to M

                        // Set velocities and check for bad velocities
                        if (this.BottomTrack.BtRefLayerVelocityBeam0 == PD0.BAD_VELOCITY)
                        {
                            ensemble.InstrumentWaterMassData.VelocityX = BAD_VELOCITY;
                        }
                        else
                        {
                            ensemble.InstrumentWaterMassData.VelocityX = this.BottomTrack.BtRefLayerVelocityBeam0;
                        }

                        if (this.BottomTrack.BtRefLayerVelocityBeam1 == PD0.BAD_VELOCITY)
                        {
                            ensemble.InstrumentWaterMassData.VelocityY = BAD_VELOCITY;
                        }
                        else
                        {
                            ensemble.InstrumentWaterMassData.VelocityY = this.BottomTrack.BtRefLayerVelocityBeam1;
                        }

                        if (this.BottomTrack.BtRefLayerVelocityBeam2 == PD0.BAD_VELOCITY)
                        {
                            ensemble.InstrumentWaterMassData.VelocityZ = BAD_VELOCITY;
                        }
                        else
                        {
                            ensemble.InstrumentWaterMassData.VelocityZ = this.BottomTrack.BtRefLayerVelocityBeam2;
                        }

                        if (this.BottomTrack.BtRefLayerVelocityBeam3 == PD0.BAD_VELOCITY)
                        {
                            ensemble.InstrumentWaterMassData.VelocityQ = BAD_VELOCITY;
                        }
                        else
                        {
                            ensemble.InstrumentWaterMassData.VelocityQ = this.BottomTrack.BtRefLayerVelocityBeam3;
                        }
                        break;
                    default:
                        break;
                }
            }

            // Velocity
            switch(xform)
            {
                case CoordinateTransforms.Coord_Beam:
                    if (ensemble.IsBeamVelocityAvail)
                    {
                        this.AddDataType(new Pd0Velocity(ensemble.BeamVelocityData));

                        // Percent Good
                        if (ensemble.IsGoodBeamAvail)
                        {
                            this.AddDataType(new Pd0PercentGood(ensemble.GoodBeamData, ensemble.EnsembleData.ActualPingCount));
                        }
                    }
                    break;
                case CoordinateTransforms.Coord_Earth:
                    if (ensemble.IsEarthVelocityAvail)
                    {
                        this.AddDataType(new Pd0Velocity(ensemble.EarthVelocityData));

                        // Percent Good
                        if (ensemble.IsGoodEarthAvail)
                        {
                            this.AddDataType(new Pd0PercentGood(ensemble.GoodEarthData, ensemble.EnsembleData.ActualPingCount));
                        }
                    }
                    break;
                case CoordinateTransforms.Coord_Instrument:
                    if (ensemble.IsInstrumentVelocityAvail)
                    {
                        this.AddDataType(new Pd0Velocity(ensemble.InstrumentVelocityData));

                        // Percent Good
                        if (ensemble.IsGoodBeamAvail)
                        {
                            this.AddDataType(new Pd0PercentGood(ensemble.GoodBeamData, ensemble.EnsembleData.ActualPingCount));
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}
