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
 * 03/03/2014      RC          2.21.4     Initial coding
 * 04/16/2014      RC          2.21.4     Fixed code to handle vertical beams.
 * 08/14/2018      RC          3.4.9      Inverted the sign of the Bottom Track Velocity when converting from RTI to PD0.
 * 03/06/2019      RC          3.4.11     Remove screening the correlation data for PD0.
 * 06/26/2019      RC          3.4.12     Added ShipVelocity and Fixed Instrument Velocity in DecodeRtiEnsemble.
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
    /// PD0 Bottom Track.
    /// </summary>
    public class Pd0BottomTrack : Pd0DataType
    {

        #region Variable

        /// <summary>
        /// Number of bytes required for the Data Type.
        /// </summary>
        public const int DATATYPE_SIZE = 85;

        /// <summary>
        /// LSB for the ID for the PD0 Bottom Track data type.
        /// </summary>
        public const byte ID_LSB = 0x00;

        /// <summary>
        /// MSB for the ID for the PD0 Bottom Track data type.
        /// </summary>
        public const byte ID_MSB = 0x06;

        #endregion

        #region Properties
        
        /// <summary>
        /// Bottom Track Pings per Ensemble.
        /// 
        /// Stores the number of bottom-track pings to average together
        /// in each ensemble (BP-command). If BP = 0, the ADCP does
        /// not collect bottom-track data. The ADCP automatically extends
        /// the ensemble interval (TE) if BP x TP > TE.
        /// 
        /// Scaling: LSD = 1 ping; Range = 0 to 999 pings
        /// </summary>
        public ushort BtPingsPerEnsemble { get; set; }

        /// <summary>
        /// Bottom Track Delay Before Reacquire.
        /// 
        /// Stores the number of ADCP ensembles to wait after losing
        /// the bottom before trying to reacquire it (BD-command).
        /// 
        /// Scaling: LSD = 1 ensemble; Range = 0 to 999 ensembles
        /// </summary>
        public ushort BtDelayBeforeReacquire { get; set; }

        /// <summary>
        /// Bottom Track Correlation Magnitude Minimum.
        /// 
        /// Stores the minimum correlation magnitude value (BCcommand).
        /// 
        /// Scaling: LSD = 1 count; Range = 0 to 255 counts
        /// </summary>
        public byte BtCorrMagMin { get; set; }

        /// <summary>
        /// Bottom Track Evaluation Amplitude Minimum.
        /// 
        /// Stores the minimum evaluation amplitude value (BAcommand).
        /// 
        /// Scaling: LSD = 1 count; Range = 1 to 255 counts
        /// </summary>
        public byte BtEvalAmpMin { get; set; }

        /// <summary>
        /// Bottom Track Percent Good Minimum.
        /// 
        /// Stores the minimum percentage of bottom-track pings in an
        /// ensemble that must be good to output velocity data (BGcommand).
        /// </summary>
        public byte BtPercentGoodMin { get; set; }

        /// <summary>
        /// Bottom Track Mode.
        /// 
        /// Stores the bottom-tracking mode (BM-command).
        /// </summary>
        public byte BtMode { get; set; }

        /// <summary>
        /// Bottom Track Error Velocity Maximum.
        /// 
        /// Stores the error velocity maximum value (BE-command).
        /// 
        /// Scaling: LSD = 1 mm/s; Range = 0 to 5000 mm/s (0 = did not screen data)
        /// </summary>
        public ushort BtErrVelMax { get; set; }

        /// <summary>
        /// Reserved.
        /// </summary>
        public int Reserved13_16 { get; set; }

        #region Range

        /// <summary>
        /// Bottom Track Range Beam 0 LSB.
        /// </summary>
        public ushort BtRangeLsbBeam0 { get; set; }

        /// <summary>
        /// Bottom Track Range Beam 1 LSB.
        /// </summary>
        public ushort BtRangeLsbBeam1 { get; set; }

        /// <summary>
        /// Bottom Track Range Beam 2 LSB.
        /// </summary>
        public ushort BtRangeLsbBeam2 { get; set; }

        /// <summary>
        /// Bottom Track Range Beam 3 LSB.
        /// </summary>
        public ushort BtRangeLsbBeam3 { get; set; }

        /// <summary>
        /// Bottom Track Range Beam 0 MSB.
        /// </summary>
        public byte BtRangeMsbBeam0 { get; set; }

        /// <summary>
        /// Bottom Track Range Beam 1 MSB.
        /// </summary>
        public byte BtRangeMsbBeam1 { get; set; }

        /// <summary>
        /// Bottom Track Range Beam 2 MSB.
        /// </summary>
        public byte BtRangeMsbBeam2 { get; set; }

        /// <summary>
        /// Bottom Track Range Beam 3 MSB.
        /// </summary>
        public byte BtRangeMsbBeam3 { get; set; }

        #endregion

        #region Velocity

        /// <summary>
        /// Bottom Track Velocity Beam 0.
        /// 
        /// The meaning of the velocity depends on the EX (coordinate
        /// system) command setting. The four velocities are as follows:
        /// a) Beam Coordinates: Beam 1, Beam 2, Beam 3, Beam 4
        /// b) Instrument Coordinates: 1->2, 4->3, toward face, error
        /// c) Ship Coordinates: Starboard, Fwd, Upward, Error
        /// d) Earth Coordinates: East, North, Upward, Error
        /// </summary>
        public short BtVelocityBeam0 { get; set; }

        /// <summary>
        /// Bottom Track Velocity Beam 1.
        /// 
        /// The meaning of the velocity depends on the EX (coordinate
        /// system) command setting. The four velocities are as follows:
        /// a) Beam Coordinates: Beam 1, Beam 2, Beam 3, Beam 4
        /// b) Instrument Coordinates: 1->2, 4->3, toward face, error
        /// c) Ship Coordinates: Starboard, Fwd, Upward, Error
        /// d) Earth Coordinates: East, North, Upward, Error
        /// </summary>
        public short BtVelocityBeam1 { get; set; }

        /// <summary>
        /// Bottom Track Velocity Beam 2.
        /// 
        /// The meaning of the velocity depends on the EX (coordinate
        /// system) command setting. The four velocities are as follows:
        /// a) Beam Coordinates: Beam 1, Beam 2, Beam 3, Beam 4
        /// b) Instrument Coordinates: 1->2, 4->3, toward face, error
        /// c) Ship Coordinates: Starboard, Fwd, Upward, Error
        /// d) Earth Coordinates: East, North, Upward, Error
        /// </summary>
        public short BtVelocityBeam2 { get; set; }

        /// <summary>
        /// Bottom Track Velocity Beam 3.
        /// 
        /// The meaning of the velocity depends on the EX (coordinate
        /// system) command setting. The four velocities are as follows:
        /// a) Beam Coordinates: Beam 1, Beam 2, Beam 3, Beam 4
        /// b) Instrument Coordinates: 1->2, 4->3, toward face, error
        /// c) Ship Coordinates: Starboard, Fwd, Upward, Error
        /// d) Earth Coordinates: East, North, Upward, Error
        /// </summary>
        public short BtVelocityBeam3 { get; set; }

        #endregion

        #region Correlation

        /// <summary>
        /// Bottom Track Correlation Magnitude Beam 0.
        /// 
        /// Contains the correlation magnitude in relation to the sea bottom
        /// (or surface) as determined by each beam. Bottom-track
        /// correlation magnitudes have the same format and scale factor
        /// as water-profiling magnitudes (Table 5).
        /// </summary>
        public byte BtCorrelationMagnitudeBeam0 { get; set; }

        /// <summary>
        /// Bottom Track Correlation Magnitude Beam 1.
        /// 
        /// Contains the correlation magnitude in relation to the sea bottom
        /// (or surface) as determined by each beam. Bottom-track
        /// correlation magnitudes have the same format and scale factor
        /// as water-profiling magnitudes (Table 5).
        /// </summary>
        public byte BtCorrelationMagnitudeBeam1 { get; set; }

        /// <summary>
        /// Bottom Track Correlation Magnitude Beam 2.
        /// 
        /// Contains the correlation magnitude in relation to the sea bottom
        /// (or surface) as determined by each beam. Bottom-track
        /// correlation magnitudes have the same format and scale factor
        /// as water-profiling magnitudes (Table 5).
        /// </summary>
        public byte BtCorrelationMagnitudeBeam2 { get; set; }

        /// <summary>
        /// Bottom Track Correlation Magnitude Beam 3.
        /// 
        /// Contains the correlation magnitude in relation to the sea bottom
        /// (or surface) as determined by each beam. Bottom-track
        /// correlation magnitudes have the same format and scale factor
        /// as water-profiling magnitudes (Table 5).
        /// </summary>
        public byte BtCorrelationMagnitudeBeam3 { get; set; }

        #endregion

        #region Amplitude

        /// <summary>
        /// Bottom Track Amplitude Beam 0.
        /// 
        /// Contains the evaluation amplitude of the matching filter used
        /// in determining the strength of the bottom echo.
        /// 
        /// Scaling: LSD = 1 count; Range = 0 to 255 counts
        /// </summary>
        public byte BtAmplitudeBeam0 { get; set; }

        /// <summary>
        /// Bottom Track Amplitude Beam 1.
        /// 
        /// Contains the evaluation amplitude of the matching filter used
        /// in determining the strength of the bottom echo.
        /// 
        /// Scaling: LSD = 1 count; Range = 0 to 255 counts
        /// </summary>
        public byte BtAmplitudeBeam1 { get; set; }

        /// <summary>
        /// Bottom Track Amplitude Beam 2.
        /// 
        /// Contains the evaluation amplitude of the matching filter used
        /// in determining the strength of the bottom echo.
        /// 
        /// Scaling: LSD = 1 count; Range = 0 to 255 counts
        /// </summary>
        public byte BtAmplitudeBeam2 { get; set; }

        /// <summary>
        /// Bottom Track Amplitude Beam 3.
        /// 
        /// Contains the evaluation amplitude of the matching filter used
        /// in determining the strength of the bottom echo.
        /// 
        /// Scaling: LSD = 1 count; Range = 0 to 255 counts
        /// </summary>
        public byte BtAmplitudeBeam3 { get; set; }

        #endregion

        #region Percent Good

        /// <summary>
        /// Bottom Track Percent Good Beam 0.
        /// 
        /// Contains bottom-track percent-good data for each beam,
        /// which indicate the reliability of bottom-track data. It is the
        /// percentage of bottom-track pings that have passed the
        /// ADCP’s bottom-track validity algorithm during an ensemble.
        /// 
        /// Scaling: LSD = 1 percent; Range = 0 to 100 percent
        /// </summary>
        public byte BtPercentGoodBeam0 { get; set; }

        /// <summary>
        /// Bottom Track Percent Good Beam 1.
        /// 
        /// Contains bottom-track percent-good data for each beam,
        /// which indicate the reliability of bottom-track data. It is the
        /// percentage of bottom-track pings that have passed the
        /// ADCP’s bottom-track validity algorithm during an ensemble.
        /// 
        /// Scaling: LSD = 1 percent; Range = 0 to 100 percent
        /// </summary>
        public byte BtPercentGoodBeam1 { get; set; }

        /// <summary>
        /// Bottom Track Percent Good Beam 2.
        /// 
        /// Contains bottom-track percent-good data for each beam,
        /// which indicate the reliability of bottom-track data. It is the
        /// percentage of bottom-track pings that have passed the
        /// ADCP’s bottom-track validity algorithm during an ensemble.
        /// 
        /// Scaling: LSD = 1 percent; Range = 0 to 100 percent
        /// </summary>
        public byte BtPercentGoodBeam2 { get; set; }

        /// <summary>
        /// Bottom Track Percent Good Beam 3.
        /// 
        /// Contains bottom-track percent-good data for each beam,
        /// which indicate the reliability of bottom-track data. It is the
        /// percentage of bottom-track pings that have passed the
        /// ADCP’s bottom-track validity algorithm during an ensemble.
        /// 
        /// Scaling: LSD = 1 percent; Range = 0 to 100 percent
        /// </summary>
        public byte BtPercentGoodBeam3 { get; set; }

        #endregion

        #region Reference Layers

        /// <summary>
        /// Bottom Track Reference Layer Mininum.
        /// 
        /// Stores the minimum layer size, the near boundary, and the far
        /// boundary of the BT water-reference layer (BL-command).
        /// 
        /// Scaling (minimum layer size): LSD = 1 dm; Range = 0-999dm
        /// 
        /// Scaling (near/far boundaries): LSD = 1 dm; Range = 0-9999dm
        /// </summary>
        public ushort BtRefLayerMin { get; set; }

        /// <summary>
        /// Bottom Track Reference Layer Near.
        /// 
        /// Stores the minimum layer size, the near boundary, and the far
        /// boundary of the BT water-reference layer (BL-command).
        /// 
        /// Scaling (minimum layer size): LSD = 1 dm; Range = 0-999dm
        /// 
        /// Scaling (near/far boundaries): LSD = 1 dm; Range = 0-9999dm
        /// </summary>
        public ushort BtRefLayerNear { get; set; }

        /// <summary>
        /// Bottom Track Reference Layer Far.
        /// 
        /// Stores the minimum layer size, the near boundary, and the far
        /// boundary of the BT water-reference layer (BL-command).
        /// 
        /// Scaling (minimum layer size): LSD = 1 dm; Range = 0-999dm
        /// 
        /// Scaling (near/far boundaries): LSD = 1 dm; Range = 0-9999dm
        /// </summary>
        public ushort BtRefLayerFar { get; set; }

        #endregion

        #region Reference Layers Velocity

        /// <summary>
        /// Bottom Track Reference Layer Velocity Beam 0.
        /// 
        /// Contains velocity data for the water reference layer for each 
        /// beam. Reference layer velocities have the same format and
        /// scale factor as water-profiling velocities (Table 34, page 139).
        /// The BL-command explains the water reference layer.
        /// </summary>
        public short BtRefLayerVelocityBeam0 { get; set; }

        /// <summary>
        /// Bottom Track Reference Layer Velocity Beam 1.
        /// 
        /// Contains velocity data for the water reference layer for each 
        /// beam. Reference layer velocities have the same format and
        /// scale factor as water-profiling velocities (Table 34, page 139).
        /// The BL-command explains the water reference layer.
        /// </summary>
        public short BtRefLayerVelocityBeam1 { get; set; }

        /// <summary>
        /// Bottom Track Reference Layer Velocity Beam 2.
        /// 
        /// Contains velocity data for the water reference layer for each 
        /// beam. Reference layer velocities have the same format and
        /// scale factor as water-profiling velocities (Table 34, page 139).
        /// The BL-command explains the water reference layer.
        /// </summary>
        public short BtRefLayerVelocityBeam2 { get; set; }

        /// <summary>
        /// Bottom Track Reference Layer Velocity Beam 3.
        /// 
        /// Contains velocity data for the water reference layer for each 
        /// beam. Reference layer velocities have the same format and
        /// scale factor as water-profiling velocities (Table 34, page 139).
        /// The BL-command explains the water reference layer.
        /// </summary>
        public short BtRefLayerVelocityBeam3 { get; set; }
        
        #endregion

        #region Reference Layers Correlation

        /// <summary>
        /// Bottom Track Reference Layer Correlation Beam 0.
        /// 
        /// Contains correlation magnitude data for the water reference
        /// layer for each beam. Reference layer correlation magnitudes
        /// have the same format and scale factor as water-profiling
        /// magnitudes (Table 5).
        /// </summary>
        public byte BtRefLayerCorrBeam0 { get; set; }

        /// <summary>
        /// Bottom Track Reference Layer Correlation Beam 1.
        /// 
        /// Contains correlation magnitude data for the water reference
        /// layer for each beam. Reference layer correlation magnitudes
        /// have the same format and scale factor as water-profiling
        /// magnitudes (Table 5).
        /// </summary>
        public byte BtRefLayerCorrBeam1 { get; set; }

        /// <summary>
        /// Bottom Track Reference Layer Correlation Beam 2.
        /// 
        /// Contains correlation magnitude data for the water reference
        /// layer for each beam. Reference layer correlation magnitudes
        /// have the same format and scale factor as water-profiling
        /// magnitudes (Table 5).
        /// </summary>
        public byte BtRefLayerCorrBeam2 { get; set; }

        /// <summary>
        /// Bottom Track Reference Layer Correlation Beam 3.
        /// 
        /// Contains correlation magnitude data for the water reference
        /// layer for each beam. Reference layer correlation magnitudes
        /// have the same format and scale factor as water-profiling
        /// magnitudes (Table 5).
        /// </summary>
        public byte BtRefLayerCorrBeam3 { get; set; }

        #endregion

        #region Reference Layer Echo Intensity

        /// <summary>
        /// Bottom Track Reference Layer Echo Intensity Beam 0.
        /// 
        /// Contains echo intensity data for the reference layer for each
        /// beam. Reference layer intensities have the same format and
        /// scale factor as water-profiling intensities.
        /// </summary>
        public byte BtRefLayerEchoIntensityBeam0 { get; set; }

        /// <summary>
        /// Bottom Track Reference Layer Echo Intensity Beam 1.
        /// 
        /// Contains echo intensity data for the reference layer for each
        /// beam. Reference layer intensities have the same format and
        /// scale factor as water-profiling intensities.
        /// </summary>
        public byte BtRefLayerEchoIntensityBeam1 { get; set; }

        /// <summary>
        /// Bottom Track Reference Layer Echo Intensity Beam 2.
        /// 
        /// Contains echo intensity data for the reference layer for each
        /// beam. Reference layer intensities have the same format and
        /// scale factor as water-profiling intensities.
        /// </summary>
        public byte BtRefLayerEchoIntensityBeam2 { get; set; }

        /// <summary>
        /// Bottom Track Reference Layer Echo Intensity Beam 3.
        /// 
        /// Contains echo intensity data for the reference layer for each
        /// beam. Reference layer intensities have the same format and
        /// scale factor as water-profiling intensities.
        /// </summary>
        public byte BtRefLayerEchoIntensityBeam3 { get; set; }

        #endregion

        #region Reference Layer Percent Good

        /// <summary>
        /// Bottom Track Reference Layer Percent Good Beam 0.
        /// 
        /// Contains percent-good data for the water reference layer for
        /// each beam. They indicate the reliability of reference layer
        /// data. It is the percentage of bottom-track pings that have
        /// passed a reference layer validity algorithm during an ensemble.
        /// 
        /// Scaling: LSD = 1 percent; Range = 0 to 100 percent
        /// </summary>
        public byte BtRefLayerPercentGoodBeam0 { get; set; }

        /// <summary>
        /// Bottom Track Reference Layer Percent Good Beam 1.
        /// 
        /// Contains percent-good data for the water reference layer for
        /// each beam. They indicate the reliability of reference layer
        /// data. It is the percentage of bottom-track pings that have
        /// passed a reference layer validity algorithm during an ensemble.
        /// 
        /// Scaling: LSD = 1 percent; Range = 0 to 100 percent
        /// </summary>
        public byte BtRefLayerPercentGoodBeam1 { get; set; }

        /// <summary>
        /// Bottom Track Reference Layer Percent Good Beam 2.
        /// 
        /// Contains percent-good data for the water reference layer for
        /// each beam. They indicate the reliability of reference layer
        /// data. It is the percentage of bottom-track pings that have
        /// passed a reference layer validity algorithm during an ensemble.
        /// 
        /// Scaling: LSD = 1 percent; Range = 0 to 100 percent
        /// </summary>
        public byte BtRefLayerPercentGoodBeam2 { get; set; }

        /// <summary>
        /// Bottom Track Reference Layer Percent Good Beam 3.
        /// 
        /// Contains percent-good data for the water reference layer for
        /// each beam. They indicate the reliability of reference layer
        /// data. It is the percentage of bottom-track pings that have
        /// passed a reference layer validity algorithm during an ensemble.
        /// 
        /// Scaling: LSD = 1 percent; Range = 0 to 100 percent
        /// </summary>
        public byte BtRefLayerPercentGoodBeam3 { get; set; }

        #endregion

        /// <summary>
        /// Bottom Track Max Depth.
        /// 
        /// Stores the maximum tracking depth value (BX-command).
        /// 
        /// Scaling: LSD = 1 decimeter; Range = 80 to 9999 decimeters
        /// </summary>
        public ushort BtMaxDepth { get; set; }

        #region RSSI

        /// <summary>
        /// Bottom Track RSSI Amplitude Beam 0.
        /// 
        /// Contains the Receiver Signal Strength Indicator (RSSI) value
        /// in the center of the bottom echo as determined by each
        /// beam.
        /// 
        /// Scaling: LSD ≈ 0.45 dB per count; Range = 0 to 255 counts
        /// </summary>
        public float BtRssiBeam0 { get; set; }

        /// <summary>
        /// Bottom Track RSSI Amplitude Beam 1.
        /// 
        /// Contains the Receiver Signal Strength Indicator (RSSI) value
        /// in the center of the bottom echo as determined by each
        /// beam.
        /// 
        /// Scaling: LSD ≈ 0.45 dB per count; Range = 0 to 255 counts
        /// </summary>
        public float BtRssiBeam1 { get; set; }


        /// <summary>
        /// Bottom Track RSSI Amplitude Beam 2.
        /// 
        /// Contains the Receiver Signal Strength Indicator (RSSI) value
        /// in the center of the bottom echo as determined by each
        /// beam.
        /// 
        /// Scaling: LSD ≈ 0.45 dB per count; Range = 0 to 255 counts
        /// </summary>
        public float BtRssiBeam2 { get; set; }

        /// <summary>
        /// Bottom Track RSSI Amplitude Beam 3.
        /// 
        /// Contains the Receiver Signal Strength Indicator (RSSI) value
        /// in the center of the bottom echo as determined by each
        /// beam.
        /// 
        /// Scaling: LSD ≈ 0.45 dB per count; Range = 0 to 255 counts
        /// </summary>
        public float BtRssiBeam3 { get; set; }

        #endregion

        /// <summary>
        /// Gain.
        /// 
        /// Contains the Gain level for shallow water. See WJ-command.
        /// </summary>
        public byte BtGain { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public int Reserved82_85 { get; set; }



        #endregion

        /// <summary>
        /// Initialize the data type.
        /// </summary>
        public Pd0BottomTrack()
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.BottomTrack)
        {
            Initialize();

            NumDepthCells = 0;
            NumBeams = 4;
        }

        /// <summary>
        /// Initialize and decode the given data.
        /// </summary>
        /// <param name="data">PD0 Binary data.</param>
        /// <param name="numBeams">Number of beams.</param>
        public Pd0BottomTrack(byte[] data, int numBeams = 4)
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.BottomTrack)
        {
            // Initialize and decode the data
            Initialize();

            NumDepthCells = 0;
            NumBeams = numBeams;

            Decode(data, numBeams);
        }

        /// <summary>
        /// Initialize and decode the given data.
        /// </summary>
        /// <param name="data">PD0 Binary data.</param>
        /// <param name="offset">Offset in the binary data.</param>
        /// <param name="numBeams">Number of beams.</param>
        public Pd0BottomTrack(byte[] data, ushort offset, int numBeams = 4)
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.BottomTrack)
        {
            // Initialize and decode the data
            this.Offset = offset;
            Initialize();

            NumDepthCells = 0;
            NumBeams = numBeams;

            Decode(data, numBeams);
        }

        /// <summary>
        /// Initialize and decode the given data.
        /// </summary>
        /// <param name="bt">RTI Bottom Track.</param>
        /// <param name="xform">PD0 Coordinate Transform.</param>
        public Pd0BottomTrack(DataSet.BottomTrackDataSet bt, PD0.CoordinateTransforms xform)
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.BottomTrack)
        {
            // Initialize and decode the data
            Initialize();

            NumDepthCells = 0;
            NumBeams = (int)bt.NumBeams;

            DecodeRtiEnsemble(bt, xform);
        }

        #region Initialize

        /// <summary>
        /// Initialize the object.
        /// </summary>
        public void Initialize()
        {
            BtPingsPerEnsemble = 0;
            BtDelayBeforeReacquire = 0;
            BtCorrMagMin = 0;
            BtEvalAmpMin = 0;
            BtPercentGoodMin = 0;
            BtMode = 0;
            BtErrVelMax = 0;
            Reserved13_16 = 0;
            BtRangeLsbBeam0 = 0;
            BtRangeLsbBeam1 = 0;
            BtRangeLsbBeam2 = 0;
            BtRangeLsbBeam3 = 0;
            BtRangeMsbBeam0 = 0;
            BtRangeMsbBeam1 = 0;
            BtRangeMsbBeam2 = 0;
            BtRangeMsbBeam3 = 0;
            BtVelocityBeam0 = 0;
            BtVelocityBeam1 = 0;
            BtVelocityBeam2 = 0;
            BtVelocityBeam3 = 0;
            BtCorrelationMagnitudeBeam0 = 0;
            BtCorrelationMagnitudeBeam1 = 0;
            BtCorrelationMagnitudeBeam2 = 0;
            BtCorrelationMagnitudeBeam3 = 0;
            BtAmplitudeBeam0 = 0;
            BtAmplitudeBeam1 = 0;
            BtAmplitudeBeam2 = 0;
            BtAmplitudeBeam3 = 0;
            BtPercentGoodBeam0 = 0;
            BtPercentGoodBeam1 = 0;
            BtPercentGoodBeam2 = 0;
            BtPercentGoodBeam3 = 0;
            BtRefLayerMin = 0;
            BtRefLayerNear = 0;
            BtRefLayerFar = 0;
            BtRefLayerVelocityBeam0 = 0;
            BtRefLayerVelocityBeam1 = 0;
            BtRefLayerVelocityBeam2 = 0;
            BtRefLayerVelocityBeam3 = 0;
            BtRefLayerCorrBeam0 = 0;
            BtRefLayerCorrBeam1 = 0;
            BtRefLayerCorrBeam2 = 0;
            BtRefLayerCorrBeam3 = 0;
            BtRefLayerEchoIntensityBeam0 = 0;
            BtRefLayerEchoIntensityBeam1 = 0;
            BtRefLayerEchoIntensityBeam2 = 0;
            BtRefLayerEchoIntensityBeam3 = 0;
            BtRefLayerPercentGoodBeam0 = 0;
            BtRefLayerPercentGoodBeam1 = 0;
            BtRefLayerPercentGoodBeam2 = 0;
            BtRefLayerPercentGoodBeam3 = 0;
            BtMaxDepth = 80;
            BtRssiBeam0 = 0;
            BtRssiBeam1 = 0;
            BtRssiBeam2 = 0;
            BtRssiBeam3 = 0;
            BtGain = 0;
            Reserved82_85 = 0;
        }

        #endregion

        #region Encode

        /// <summary>
        /// Encode the data to binary PD0.
        /// </summary>
        /// <returns>Binary format of the data.</returns>
        public override byte[] Encode()
        {
            // Pings Per Ensemble
            byte ppeLsb, ppeMsb;
            MathHelper.LsbMsbUShort(BtPingsPerEnsemble, out ppeLsb, out ppeMsb);

            // Delay Before Reacquire
            byte dbrLsb, dbrMsb;
            MathHelper.LsbMsbUShort(BtDelayBeforeReacquire, out dbrLsb, out dbrMsb);

            // Error Velocity Maximum
            byte evmLsb, evmMsb;
            MathHelper.LsbMsbUShort(BtErrVelMax, out evmLsb, out evmMsb);

            // Reserved 13-16
            byte[] reserved13_16BA = BitConverter.GetBytes(Reserved13_16);
            if (reserved13_16BA.Length < 4)
            {
                reserved13_16BA = new byte[4];
            }

            #region Range

            // Beam Range Beam 0 LSB
            byte rB0LsbLsb, rb0LsbMsb;
            MathHelper.LsbMsbUShort(BtRangeLsbBeam0, out rB0LsbLsb, out rb0LsbMsb);

            // Beam Range Beam 1 LSB
            byte rB1LsbLsb, rb1LsbMsb;
            MathHelper.LsbMsbUShort(BtRangeLsbBeam1, out rB1LsbLsb, out rb1LsbMsb);

            // Beam Range Beam 2 LSB
            byte rB2LsbLsb, rb2LsbMsb;
            MathHelper.LsbMsbUShort(BtRangeLsbBeam2, out rB2LsbLsb, out rb2LsbMsb);

            // Beam Range Beam 3 LSB
            byte rB3LsbLsb, rb3LsbMsb;
            MathHelper.LsbMsbUShort(BtRangeLsbBeam3, out rB3LsbLsb, out rb3LsbMsb);

            #endregion

            #region Velocity

            // Velocity Beam 0
            byte velB0Lsb, velB0Msb;
            MathHelper.LsbMsbShort(BtVelocityBeam0, out velB0Lsb, out velB0Msb);

            // Velocity Beam 1
            byte velB1Lsb, velB1Msb;
            MathHelper.LsbMsbShort(BtVelocityBeam1, out velB1Lsb, out velB1Msb);

            // Velocity Beam 2
            byte velB2Lsb, velB2Msb;
            MathHelper.LsbMsbShort(BtVelocityBeam2, out velB2Lsb, out velB2Msb);

            // Velocity Beam 3
            byte velB3Lsb, velB3Msb;
            MathHelper.LsbMsbShort(BtVelocityBeam3, out velB3Lsb, out velB3Msb);

            #endregion

            #region Reference Layer

            // Reference Layer Minimum
            byte rlmLsb, rlmMsb;
            MathHelper.LsbMsbUShort(BtRefLayerMin, out rlmLsb, out rlmMsb);

            // Reference Layer Near
            byte rlnLsb, rlnMsb;
            MathHelper.LsbMsbUShort(BtRefLayerNear, out rlnLsb, out rlnMsb);

            // Reference Layer Far
            byte rlfLsb, rlfMsb;
            MathHelper.LsbMsbUShort(BtRefLayerFar, out rlfLsb, out rlfMsb);

            #endregion

            #region Reference Layer Velocity

            // Reference Layer Velocity Beam 0
            byte rlvB0Lsb, rlvB0Msb;
            MathHelper.LsbMsbShort(BtRefLayerVelocityBeam0, out rlvB0Lsb, out rlvB0Msb);

            // Reference Layer Velocity Beam 1
            byte rlvB1Lsb, rlvB1Msb;
            MathHelper.LsbMsbShort(BtRefLayerVelocityBeam1, out rlvB1Lsb, out rlvB1Msb);

            // Reference Layer Velocity Beam 2
            byte rlvB2Lsb, rlvB2Msb;
            MathHelper.LsbMsbShort(BtRefLayerVelocityBeam2, out rlvB2Lsb, out rlvB2Msb);

            // Reference Layer Velocity Beam 3
            byte rlvB3Lsb, rlvB3Msb;
            MathHelper.LsbMsbShort(BtRefLayerVelocityBeam3, out rlvB3Lsb, out rlvB3Msb);

            #endregion

            // Max Depth
            byte maxDepthLsb, maxDepthMsb;
            MathHelper.LsbMsbUShort(BtMaxDepth, out maxDepthLsb, out maxDepthMsb);

            // Reserved 82-85
            byte[] reserved82_85BA = BitConverter.GetBytes(Reserved82_85);
            if (reserved82_85BA.Length < 4)
            {
                reserved82_85BA = new byte[4];
            }

            byte[] data = new byte[DATATYPE_SIZE];

            data[0] = ID_LSB;                                   // Fixed Leader ID LSB 0x00
            data[1] = ID_MSB;                                   // Fixed Leader ID MSB 0x60
            data[2] = ppeLsb;                                   // Pings Per Ensemble LSB
            data[3] = ppeMsb;                                   // Pings Per Ensemble MSB
            data[4] = dbrLsb;                                   // Delay Before Reacquire LSB
            data[5] = dbrMsb;                                   // Delay Before Reacquire MSB
            data[6] = BtCorrMagMin;                             // Correlation Maginitude Minimum
            data[7] = BtEvalAmpMin;                             // Evaluation Amplitude Minimum
            data[8] = BtPercentGoodMin;                         // Percent Good Minimum
            data[9] = BtMode;                                   // Bottom Track Mode
            data[10] = evmLsb;                                  // Error Velocity Maximum LSB
            data[11] = evmMsb;                                  // Error Velocity Maximum MSB
            data[12] = reserved13_16BA[0];                      // Reserved 13-16 LSB
            data[13] = reserved13_16BA[1];                      // Reserved 13-16 
            data[14] = reserved13_16BA[2];                      // Reserved 13-16
            data[15] = reserved13_16BA[3];                      // Reserved 13-16 MSB
            data[16] = rB0LsbLsb;                                // Beam Range Beam 0 LSB LSB
            data[17] = rb0LsbMsb;                                // Beam Range Beam 0 LSB MSB
            data[18] = rB1LsbLsb;                                // Beam Range Beam 1 LSB LSB
            data[19] = rb1LsbMsb;                                // Beam Range Beam 1 LSB MSB
            data[20] = rB2LsbLsb;                                // Beam Range Beam 2 LSB LSB
            data[21] = rb2LsbMsb;                                // Beam Range Beam 2 LSB MSB
            data[22] = rB3LsbLsb;                                // Beam Range Beam 3 LSB LSB
            data[23] = rb3LsbMsb;                                // Beam Range Beam 3 LSB MSB
            data[24] = velB0Lsb;                                // Velocity Beam 0 LSB
            data[25] = velB0Msb;                                // Velocity Beam 0 MSB
            data[26] = velB1Lsb;                                // Velocity Beam 1 LSB
            data[27] = velB1Msb;                                // Velocity Beam 1 MSB
            data[28] = velB2Lsb;                                // Velocity Beam 2 LSB
            data[29] = velB2Msb;                                // Velocity Beam 2 MSB
            data[30] = velB3Lsb;                                // Velocity Beam 3 LSB
            data[31] = velB3Msb;                                // Velocity Beam 3 MSB
            data[32] = BtCorrelationMagnitudeBeam0;             // Correlation Beam 0
            data[33] = BtCorrelationMagnitudeBeam1;             // Correlation Beam 1
            data[34] = BtCorrelationMagnitudeBeam2;             // Correlation Beam 2
            data[35] = BtCorrelationMagnitudeBeam3;             // Correlation Beam 3
            data[36] = BtAmplitudeBeam0;                        // Amplitude Beam 0
            data[37] = BtAmplitudeBeam1;                        // Amplitude Beam 1
            data[38] = BtAmplitudeBeam2;                        // Amplitude Beam 2
            data[39] = BtAmplitudeBeam3;                        // Amplitude Beam 3
            data[40] = BtPercentGoodBeam0;                      // Percent Good Beam 0
            data[41] = BtPercentGoodBeam1;                      // Percent Good Beam 1
            data[42] = BtPercentGoodBeam2;                      // Percent Good Beam 2
            data[43] = BtPercentGoodBeam3;                      // Percent Good Beam 3
            data[44] = rlmLsb;                                  // Reference Layer Min LSB
            data[45] = rlmMsb;                                  // Reference Layer Min MSB
            data[46] = rlnLsb;                                  // Reference Layer Near LSB
            data[47] = rlnMsb;                                  // Reference Layer Near MSB
            data[48] = rlfLsb;                                  // Reference Layer Far LSB
            data[49] = rlfMsb;                                  // Reference Layer Far MSB
            data[50] = rlvB0Lsb;                                // Reference Layer Velocity Beam 0 LSB
            data[51] = rlvB0Msb;                                // Reference Layer Velocity Beam 0 MSB
            data[52] = rlvB1Lsb;                                // Reference Layer Velocity Beam 1 LSB
            data[53] = rlvB1Msb;                                // Reference Layer Velocity Beam 1 MSB
            data[54] = rlvB2Lsb;                                // Reference Layer Velocity Beam 2 LSB
            data[55] = rlvB2Msb;                                // Reference Layer Velocity Beam 2 MSB
            data[56] = rlvB3Lsb;                                // Reference Layer Velocity Beam 3 LSB
            data[57] = rlvB3Msb;                                // Reference Layer Velocity Beam 3 MSB
            data[58] = BtRefLayerCorrBeam0;                     // Reference Layer Correlation Beam 0
            data[59] = BtRefLayerCorrBeam1;                     // Reference Layer Correlation Beam 1
            data[60] = BtRefLayerCorrBeam2;                     // Reference Layer Correlation Beam 2
            data[61] = BtRefLayerCorrBeam3;                     // Reference Layer Correlation Beam 3
            data[62] = BtRefLayerEchoIntensityBeam0;            // Reference Layer Echo Intensity Beam 0
            data[63] = BtRefLayerEchoIntensityBeam1;            // Reference Layer Echo Intensity Beam 1
            data[64] = BtRefLayerEchoIntensityBeam2;            // Reference Layer Echo Intensity Beam 2
            data[65] = BtRefLayerEchoIntensityBeam3;            // Reference Layer Echo Intensity Beam 3
            data[66] = BtRefLayerPercentGoodBeam0;              // Reference Layer Percent Good Beam 0
            data[67] = BtRefLayerPercentGoodBeam1;              // Reference Layer Percent Good Beam 1
            data[68] = BtRefLayerPercentGoodBeam2;              // Reference Layer Percent Good Beam 2
            data[69] = BtRefLayerPercentGoodBeam3;              // Reference Layer Percent Good Beam 3
            data[70] = maxDepthLsb;                             // Max Depth LSB
            data[71] = maxDepthMsb;                             // Max Depth MSB
            data[72] = (byte)Math.Round(BtRssiBeam0 / 0.45f);   // RSSI Beam 0
            data[73] = (byte)Math.Round(BtRssiBeam1 / 0.45f);   // RSSI Beam 1
            data[74] = (byte)Math.Round(BtRssiBeam2 / 0.45f);   // RSSI Beam 2
            data[75] = (byte)Math.Round(BtRssiBeam3 / 0.45f);   // RSSI Beam 3
            data[76] = BtGain;                                    // Gain
            data[77] = BtRangeMsbBeam0;                         // Beam Range Beam 0 MSB
            data[78] = BtRangeMsbBeam1;                         // Beam Range Beam 1 MSB
            data[79] = BtRangeMsbBeam2;                         // Beam Range Beam 2 MSB
            data[80] = BtRangeMsbBeam3;                         // Beam Range Beam 3 MSB
            data[81] = reserved82_85BA[0];                      // Reserved 13-16 LSB
            data[82] = reserved82_85BA[1];                      // Reserved 13-16 
            data[83] = reserved82_85BA[2];                      // Reserved 13-16
            data[84] = reserved82_85BA[3];                      // Reserved 13-16 MSB

            return data;
        }

        #endregion

        #region Decode

        /// <summary>
        /// Decode the binary data to the a Bottom Track.
        /// </summary>
        /// <param name="data">Data to decode.</param>
        /// <param name="numBeams">Number of beams.</param>
        public void Decode(byte[] data, int numBeams=4)
        {
            BtPingsPerEnsemble = MathHelper.LsbMsbUShort(data[2], data[3]);                 // Pings per Ensemble [2,3]
            BtDelayBeforeReacquire = MathHelper.LsbMsbUShort(data[4], data[5]);             // Delay Before Reacquire [4,5]
            BtCorrMagMin = data[6];                                                         // Corrlation Maginitude Minimum [6]
            BtEvalAmpMin = data[7];                                                         // Evaluation Amplitude Minimum [7]
            BtPercentGoodMin = data[8];                                                     // Percent Good Minimum [8]
            BtMode = data[9];                                                               // Bottom Track Mode [9]
            BtErrVelMax = MathHelper.LsbMsbUShort(data[10], data[11]);                      // Error Velocity Maximum [10,11]
            Reserved13_16 = BitConverter.ToInt32(data, 12);                                 // Reserved [12,15]

            int index = 16;
            for (int beam = 0; beam < NumBeams; beam++)
            {
                if (beam == 0)
                {
                    BtRangeLsbBeam0 = MathHelper.LsbMsbUShort(data[index], data[index + 1]);                  // Range LSB Beam 0 [16,17]
                }
                if (beam == 1)
                {
                    BtRangeLsbBeam1 = MathHelper.LsbMsbUShort(data[index], data[index + 1]);                  // Range LSB Beam 1 [18,19]
                }
                if (beam == 2)
                {
                    BtRangeLsbBeam2 = MathHelper.LsbMsbUShort(data[index], data[index + 1]);                  // Range LSB Beam 2 [20,21]
                }
                if (beam == 3)
                {
                    BtRangeLsbBeam3 = MathHelper.LsbMsbUShort(data[index], data[index + 1]);                  // Range LSB Beam 3 [22,23]
                }

                index += 2;
            }

            for (int beam = 0; beam < NumBeams; beam++)
            {
                if (beam == 0)
                {
                    BtVelocityBeam0 = MathHelper.LsbMsbShort(data[index], data[index + 1]);                   // Velocity Beam 0 [24,25]
                }
                if (beam == 1)
                {
                    BtVelocityBeam1 = MathHelper.LsbMsbShort(data[index], data[index + 1]);                   // Velocity Beam 1 [26,27]
                }
                if (beam == 2)
                {
                    BtVelocityBeam2 = MathHelper.LsbMsbShort(data[index], data[index + 1]);                   // Velocity Beam 2 [28,29]
                }
                if (beam == 3)
                {
                    BtVelocityBeam3 = MathHelper.LsbMsbShort(data[index], data[index + 1]);                   // Velocity Beam 3 [30,31]
                }

                index += 2;
            }

            for (int beam = 0; beam < NumBeams; beam++)
            {
                if (beam == 0)
                {
                    BtCorrelationMagnitudeBeam0 = data[index++];                                         // Correlation Magnitude Beam 0 [32]
                }
                if (beam == 1)
                {
                    BtCorrelationMagnitudeBeam1 = data[index++];                                         // Correlation Magnitude Beam 1 [33]
                }
                if (beam == 2)
                {
                    BtCorrelationMagnitudeBeam2 = data[index++];                                         // Correlation Magnitude Beam 2 [34]
                }
                if (beam == 3)
                {
                    BtCorrelationMagnitudeBeam3 = data[index++];                                         // Correlation Magnitude Beam 3 [35]
                }
            }

            for (int beam = 0; beam < NumBeams; beam++)
            {
                if (beam == 0)
                {
                    BtAmplitudeBeam0 = data[index++];                                                    // Amplitude Beam 0 [36]
                }
                if (beam == 1)
                {
                    BtAmplitudeBeam1 = data[index++];                                                    // Amplitude Beam 1 [37]
                }
                if (beam == 2)
                {
                    BtAmplitudeBeam2 = data[index++];                                                    // Amplitude Beam 2 [38]
                }
                if (beam == 3)
                {
                    BtAmplitudeBeam3 = data[index++];                                                    // Amplitude Beam 3 [39]
                }
            }

            for (int beam = 0; beam < NumBeams; beam++)
            {
                if (beam == 0)
                {
                    BtPercentGoodBeam0 = data[index++];                                                  // Percent Good Beam 0 [40]
                }
                if (beam == 1)
                {
                    BtPercentGoodBeam1 = data[index++];                                                  // Percent Good Beam 1 [41]
                }
                if (beam == 2)
                {
                    BtPercentGoodBeam2 = data[index++];                                                  // Percent Good Beam 2 [42]
                }
                if (beam == 3)
                {
                    BtPercentGoodBeam3 = data[index++];                                                  // Percent Good Beam 3 [43]
                }
            }

            BtRefLayerMin = MathHelper.LsbMsbUShort(data[index], data[index + 1]);                    // Reference Layer Minimum [44,45]
            index += 2;

            BtRefLayerNear = MathHelper.LsbMsbUShort(data[index], data[index + 1]);                   // Reference Layer Near [46,47]
            index += 2;

            BtRefLayerFar = MathHelper.LsbMsbUShort(data[index], data[index + 1]);                    // Reference Layer Far [48,49]
            index += 2;

            for (int beam = 0; beam < NumBeams; beam++)
            {
                if (beam == 0)
                {
                    BtRefLayerVelocityBeam0 = MathHelper.LsbMsbShort(data[index], data[index + 1]);           // Reference Layer Velocity Beam 0     [50,51]
                }
                if (beam == 1)
                {
                    BtRefLayerVelocityBeam1 = MathHelper.LsbMsbShort(data[index], data[index + 1]);           // Reference Layer Velocity Beam 1     [52,53]
                }
                if (beam == 2)
                {
                    BtRefLayerVelocityBeam2 = MathHelper.LsbMsbShort(data[index], data[index + 1]);           // Reference Layer Velocity Beam 2    [54,55] 
                }
                if (beam == 3)
                {
                    BtRefLayerVelocityBeam3 = MathHelper.LsbMsbShort(data[index], data[index + 1]);           // Reference Layer Velocity Beam 3     [56,57]
                }

                index += 2;
            }

            for (int beam = 0; beam < NumBeams; beam++)
            {
                if (beam == 0)
                {
                    BtRefLayerCorrBeam0 = data[index++];                                                 // Reference Layer Correlation Beam 0 [58]
                }
                if (beam == 1)
                {
                    BtRefLayerCorrBeam1 = data[index++];                                                 // Reference Layer Correlation Beam 1 [59]
                }
                if (beam == 2)
                {
                    BtRefLayerCorrBeam2 = data[index++];                                                 // Reference Layer Correlation Beam 2 [60]
                }
                if (beam == 3)
                {
                    BtRefLayerCorrBeam3 = data[index++];                                                 // Reference Layer Correlation Beam 3 [61]
                }
            }

            for (int beam = 0; beam < NumBeams; beam++)
            {
                if (beam == 0)
                {
                    BtRefLayerEchoIntensityBeam0 = data[index++];                                        // Reference Layer Echo Intensity Beam 0 [62]
                }
                if (beam == 1)
                {
                    BtRefLayerEchoIntensityBeam1 = data[index++];                                        // Reference Layer Echo Intensity Beam 1 [63]
                }
                if (beam == 2)
                {
                    BtRefLayerEchoIntensityBeam2 = data[index++];                                        // Reference Layer Echo Intensity Beam 2 [64]
                }
                if (beam == 3)
                {
                    BtRefLayerEchoIntensityBeam3 = data[index++];                                        // Reference Layer Echo Intensity Beam 3 [65]
                }
            }

            for (int beam = 0; beam < NumBeams; beam++)
            {
                if (beam == 0)
                {
                    BtRefLayerPercentGoodBeam0 = data[index++];                                          // Reference Layer Percent Good Beam 0 [66]
                }
                if (beam == 1)
                {
                    BtRefLayerPercentGoodBeam1 = data[index++];                                          // Reference Layer Percent Good Beam 1 [67]
                }
                if (beam == 2)
                {
                    BtRefLayerPercentGoodBeam2 = data[index++];                                          // Reference Layer Percent Good Beam 2 [68]
                }
                if (beam == 3)
                {
                    BtRefLayerPercentGoodBeam3 = data[index++];                                          // Reference Layer Percent Good Beam 3 [69]
                }
            }


            BtMaxDepth = MathHelper.LsbMsbUShort(data[index], data[index + 1]);                       // Max Depth [70,71]
            index += 2;

            for (int beam = 0; beam < NumBeams; beam++)
            {
                if (beam == 0)
                {
                    BtRssiBeam0 = data[index++] * 0.45f;                                                 // RSSI Beam 0 [72]
                }
                if (beam == 1)
                {
                    BtRssiBeam1 = data[index++] * 0.45f;                                                 // RSSI Beam 1 [73]
                }
                if (beam == 2)
                {
                    BtRssiBeam2 = data[index++] * 0.45f;                                                 // RSSI Beam 2 [74]
                }
                if (beam == 3)
                {
                    BtRssiBeam3 = data[index++] * 0.45f;                                                 // RSSI Beam 3 [75]
                }
            }


            BtGain = data[index++];                                                                // Gain [76]

            for (int beam = 0; beam < NumBeams; beam++)
            {
                if (beam == 0)
                {
                    BtRangeMsbBeam0 = data[index++];                                                     // Range MSB Beam 0 [77]
                }
                if (beam == 1)
                {
                    BtRangeMsbBeam1 = data[index++];                                                     // Range MSB Beam 1 [78]
                }
                if (beam == 2)
                {
                    BtRangeMsbBeam2 = data[index++];                                                     // Range MSB Beam 2 [79]
                }
                if (beam == 3)
                {
                    BtRangeMsbBeam3 = data[index++];                                                     // Range MSB Beam 3 [80]
                }
            }

            Reserved82_85 = BitConverter.ToInt32(data, index);                                 // Reserved [81,85]
        }

        #endregion

        #region Range

        /// <summary>
        /// Get the Range for Beam 0.
        /// </summary>
        /// <returns>Beam 0 range in cm.</returns>
        public int GetRangeBeam0()
        {
            int range = BtRangeMsbBeam0 * ushort.MaxValue;
            return range + BtRangeLsbBeam0;
        }

        /// <summary>
        /// Get the Range for Beam 1.
        /// </summary>
        /// <returns>Beam 1 range in cm.</returns>
        public int GetRangeBeam1()
        {
            int range = BtRangeMsbBeam1 * ushort.MaxValue;
            return range + BtRangeLsbBeam1;
        }

        /// <summary>
        /// Get the Range for Beam 2.
        /// </summary>
        /// <returns>Beam 2 range in cm.</returns>
        public int GetRangeBeam2()
        {
            int range = BtRangeMsbBeam2 * ushort.MaxValue;
            return range + BtRangeLsbBeam2;
        }

        /// <summary>
        /// Get the Range for Beam 3.
        /// </summary>
        /// <returns>Beam 3 range in cm.</returns>
        public int GetRangeBeam3()
        {
            int range = BtRangeMsbBeam3 * ushort.MaxValue;
            return range + BtRangeLsbBeam3;
        }

        #endregion

        #region Data Type Size

        /// <summary>
        /// Get the number of bytes in the data type.
        /// </summary>
        /// <returns>Number of bytes for the data type.</returns>
        public override int GetDataTypeSize()
        {
            return DATATYPE_SIZE;
        }

        #endregion

        /// <summary>
        /// Convert the RTI Bottom Track data set to the PD0 Bottom Tarack data type.
        /// 
        /// Earth and Instrument Velocity will always have 4 beams worth of data.  No matter if it is a 1, 3 or 4 beam systems.
        /// The transform will mark the missing values bad.
        /// 
        /// </summary>
        /// <param name="bt">RTI Bottom Track data set.</param>
        /// <param name="xform">Coordinate Transform.</param>
        public void DecodeRtiEnsemble(DataSet.BottomTrackDataSet bt, PD0.CoordinateTransforms xform)
        {
            BtPingsPerEnsemble = (ushort)bt.ActualPingCount;                                        // Pings per ensemble
            BtDelayBeforeReacquire = 0;                                                             // Delay Before Re-Acquire
            BtCorrMagMin = 0;                                                                       // Low Correlation Threshold - CWPCT - Do not have enough info
            BtEvalAmpMin = 0;                                                                       // Evaluation Amplitude Min - SNR - Do not have enough info
            BtPercentGoodMin = 0;                                                                   // Percent Good Minimum - Do not have enough info
            BtMode = 0;                                                                             // Bottom Track Mode - CBTBB - Do not have enough info
            BtErrVelMax = 0;                                                                        // Error Velocity Maximum - CBTQT - Do not have enough info
            Reserved13_16 = 0;                                                                      // Reserved

            #region Range

            // 4 Beam system
            if (bt.NumBeams >= 3)
            {
                int beam = 0;
                for (beam = 0; beam < NumBeams; beam++)
                {
                    ushort lsb = 0;
                    byte msb = 0;
                    switch (beam)
                    {
                        case 0:
                            msb = (byte)Math.Floor((bt.Range[beam] * 100.0f) / ushort.MaxValue);
                            lsb = (ushort)Math.Round((bt.Range[beam] * 100.0f) - (msb * ushort.MaxValue));
                            BtRangeLsbBeam3 = lsb;
                            BtRangeMsbBeam3 = msb;
                            break;
                        case 1:
                            msb = (byte)Math.Floor((bt.Range[beam] * 100.0f) / ushort.MaxValue);
                            lsb = (ushort)Math.Round((bt.Range[beam] * 100.0f) - (msb * ushort.MaxValue));
                            BtRangeLsbBeam2 = lsb;
                            BtRangeMsbBeam2 = msb;
                            break;
                        case 2:
                            msb = (byte)Math.Floor((bt.Range[beam] * 100.0f) / ushort.MaxValue);
                            lsb = (ushort)Math.Round((bt.Range[beam] * 100.0f) - (msb * ushort.MaxValue));
                            BtRangeLsbBeam0 = lsb;
                            BtRangeMsbBeam0 = msb;
                            break;
                        case 3:
                            msb = (byte)Math.Floor((bt.Range[beam] * 100.0f) / ushort.MaxValue);
                            lsb = (ushort)Math.Round((bt.Range[beam] * 100.0f) - (msb * ushort.MaxValue));
                            BtRangeLsbBeam1 = lsb;
                            BtRangeMsbBeam1 = msb;
                            break;
                        default:
                            break;
                    }
                }
            }
            // Vertical Beam
            else if (bt.NumBeams == 1)
            {
                ushort lsb = 0;
                byte msb = 0;
                msb = (byte)Math.Floor((bt.Range[0] * 100.0f) / ushort.MaxValue);
                lsb = (ushort)Math.Round((bt.Range[0] * 100.0f) - (msb * ushort.MaxValue));
                BtRangeLsbBeam0 = lsb;
                BtRangeMsbBeam0 = msb;
            }

            #endregion

            #region Velocity

            switch(xform)
            {
                // Beam Coordinate Transform
                case PD0.CoordinateTransforms.Coord_Beam:
                    // 4 Beam System
                    if (bt.NumBeams >= 4)
                    {
                        // Beam Beam 3
                        if (bt.BeamVelocity[0] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            BtVelocityBeam3 = (short)(Math.Round(bt.BeamVelocity[0] * 1000.0f * -1.0f));   // mm/s to m/s 
                        }
                        else
                        {
                            // Bad velocity
                            BtVelocityBeam3 = PD0.BAD_VELOCITY;
                        }

                        // Beam Beam 2
                        if (bt.BeamVelocity[1] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            BtVelocityBeam2 = (short)(Math.Round(bt.BeamVelocity[1] * 1000.0f * -1.0f));   // mm/s to m/s 
                        }
                        else
                        {
                            // Bad velocity
                            BtVelocityBeam2 = PD0.BAD_VELOCITY;
                        }

                        // Beam Beam 0
                        if (bt.BeamVelocity[2] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            BtVelocityBeam0 = (short)(Math.Round(bt.BeamVelocity[2] * 1000.0f * -1.0f));   // mm/s to m/s 
                        }
                        else
                        {
                            // Bad velocity
                            BtVelocityBeam0 = PD0.BAD_VELOCITY;
                        }

                        // Beam Beam 1
                        if (bt.BeamVelocity[3] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            BtVelocityBeam1 = (short)(Math.Round(bt.BeamVelocity[3] * 1000.0f * -1.0f));   // mm/s to m/s 
                        }
                        else
                        {
                            // Bad velocity
                            BtVelocityBeam1 = PD0.BAD_VELOCITY;
                        }
                    }
                    // 3 Beam system
                    else if (bt.NumBeams == 3)
                    {
                        for(int beam = 0; beam < NumBeams; beam++)
                        {
                            if (beam == 0)
                            {
                                // Beam Beam 0
                                if (bt.BeamVelocity[beam] != DataSet.Ensemble.BAD_VELOCITY)
                                {
                                    BtVelocityBeam0 = (short)(Math.Round(bt.BeamVelocity[beam] * 1000.0f * -1.0f));   // mm/s to m/s 
                                }
                                else
                                {
                                    // Bad velocity
                                    BtVelocityBeam0 = PD0.BAD_VELOCITY;
                                }
                            }
                            if (beam == 1)
                            {
                                // Beam Beam 1
                                if (bt.BeamVelocity[beam] != DataSet.Ensemble.BAD_VELOCITY)
                                {
                                    BtVelocityBeam1 = (short)(Math.Round(bt.BeamVelocity[beam] * 1000.0f * -1.0f));   // mm/s to m/s 
                                }
                                else
                                {
                                    // Bad velocity
                                    BtVelocityBeam1 = PD0.BAD_VELOCITY;
                                }
                            }
                            if (beam == 2)
                            {
                                // Beam Beam 2
                                if (bt.BeamVelocity[beam] != DataSet.Ensemble.BAD_VELOCITY)
                                {
                                    BtVelocityBeam2 = (short)(Math.Round(bt.BeamVelocity[beam] * 1000.0f * -1.0f));   // mm/s to m/s 
                                }
                                else
                                {
                                    // Bad velocity
                                    BtVelocityBeam2 = PD0.BAD_VELOCITY;
                                }
                            }
                        }
                    }
                    // Vertical Beam
                    else if (bt.NumBeams == 1)
                    {
                        // Beam Beam 3
                        if (bt.BeamVelocity[0] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            BtVelocityBeam0 = (short)(Math.Round(bt.BeamVelocity[0] * 1000.0f * -1.0f));   // mm/s to m/s 
                        }
                        else
                        {
                            // Bad velocity
                            BtVelocityBeam0 = PD0.BAD_VELOCITY;
                        }
                    }
                    break;

                // Earth Coordinate Transform
                case PD0.CoordinateTransforms.Coord_Earth:
                    // 4 Beam System
                    if (bt.NumBeams >= 4)
                    {
                        if (bt.NumBeams > 0)
                        {
                            // Earth Beam 0
                            if (bt.EarthVelocity[0] != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                BtVelocityBeam0 = (short)(Math.Round(bt.EarthVelocity[0] * 1000.0f * -1.0f));   // mm/s to m/s 
                            }
                            else
                            {
                                // Bad velocity
                                BtVelocityBeam0 = PD0.BAD_VELOCITY;
                            }
                        }

                        if (bt.NumBeams > 1)
                        {
                            // Earth Beam 1
                            if (bt.EarthVelocity[1] != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                BtVelocityBeam1 = (short)(Math.Round(bt.EarthVelocity[1] * 1000.0f * -1.0f));   // mm/s to m/s 
                            }
                            else
                            {
                                // Bad velocity
                                BtVelocityBeam1 = PD0.BAD_VELOCITY;
                            }
                        }

                        if (bt.NumBeams > 2)
                        {
                            // Earth Beam 2
                            if (bt.EarthVelocity[2] != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                BtVelocityBeam2 = (short)(Math.Round(bt.EarthVelocity[2] * 1000.0f * -1.0f));   // mm/s to m/s 
                            }
                            else
                            {
                                // Bad velocity
                                BtVelocityBeam2 = PD0.BAD_VELOCITY;
                            }
                        }

                        if (bt.NumBeams > 3)
                        {
                            // Earth Beam 3
                            if (bt.EarthVelocity[3] != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                BtVelocityBeam3 = (short)(Math.Round(bt.EarthVelocity[3] * 1000.0f * -1.0f));   // mm/s to m/s 
                            }
                            else
                            {
                                // Bad velocity
                                BtVelocityBeam3 = PD0.BAD_VELOCITY;
                            }
                        }
                    }
                    // 3 Beam System
                    else if (bt.NumBeams >= 3)
                    {
                        if (bt.NumBeams > 0)
                        {
                            // Earth Beam 0
                            if (bt.EarthVelocity[0] != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                BtVelocityBeam0 = (short)(Math.Round(bt.EarthVelocity[0] * 1000.0f * -1.0f));   // mm/s to m/s 
                            }
                            else
                            {
                                // Bad velocity
                                BtVelocityBeam0 = PD0.BAD_VELOCITY;
                            }
                        }

                        if (bt.NumBeams > 1)
                        {
                            // Earth Beam 1
                            if (bt.EarthVelocity[1] != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                BtVelocityBeam1 = (short)(Math.Round(bt.EarthVelocity[1] * 1000.0f * -1.0f));   // mm/s to m/s 
                            }
                            else
                            {
                                // Bad velocity
                                BtVelocityBeam1 = PD0.BAD_VELOCITY;
                            }
                        }

                        if (bt.NumBeams > 2)
                        {
                            // Earth Beam 2
                            if (bt.EarthVelocity[2] != DataSet.Ensemble.BAD_VELOCITY)
                            {
                                BtVelocityBeam2 = (short)(Math.Round(bt.EarthVelocity[2] * 1000.0f * -1.0f));   // mm/s to m/s 
                            }
                            else
                            {
                                // Bad velocity
                                BtVelocityBeam2 = PD0.BAD_VELOCITY;
                            }
                        }

                    }
                    // Vertical Beam
                    else if (bt.NumBeams == 1)
                    {
                        // Earth Beam 0
                        if (bt.EarthVelocity[0] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            BtVelocityBeam0 = (short)(Math.Round(bt.EarthVelocity[0] * 1000.0f * -1.0f));   // mm/s to m/s 
                        }
                        else
                        {
                            // Bad velocity
                            BtVelocityBeam0 = PD0.BAD_VELOCITY;
                        }
                    }
                    break;

                // Instrument Coordinate Transform
                case PD0.CoordinateTransforms.Coord_Instrument:
                    // 4 Beam System
                    if (bt.NumBeams >= 4)
                    {
                        // Instrument Beam 1
                        if (bt.InstrumentVelocity[0] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            BtVelocityBeam1 = (short)(Math.Round(bt.InstrumentVelocity[0] * 1000.0f * -1.0f));   // mm/s to m/s 
                        }
                        else
                        {
                            // Bad velocity
                            BtVelocityBeam1 = PD0.BAD_VELOCITY;
                        }

                        // Instrument Beam 0
                        if (bt.InstrumentVelocity[1] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            BtVelocityBeam0 = (short)(Math.Round(bt.InstrumentVelocity[1] * 1000.0f * -1.0f));   // mm/s to m/s 
                        }
                        else
                        {
                            // Bad velocity
                            BtVelocityBeam0 = PD0.BAD_VELOCITY;
                        }

                        // Instrument Beam -2
                        if (bt.InstrumentVelocity[2] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            BtVelocityBeam2 = (short)(Math.Round(bt.InstrumentVelocity[2] * 1000.0f * 1.0f));   // mm/s to m/s 
                        }
                        else
                        {
                            // Bad velocity
                            BtVelocityBeam2 = PD0.BAD_VELOCITY;
                        }

                        // Instrument Beam 3
                        if (bt.InstrumentVelocity[3] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            BtVelocityBeam3 = (short)(Math.Round(bt.InstrumentVelocity[3] * 1000.0f * -1.0f));   // mm/s to m/s 
                        }
                        else
                        {
                            // Bad velocity
                            BtVelocityBeam3 = PD0.BAD_VELOCITY;
                        }
                    }
                    // 3 Beam System
                    else if (bt.NumBeams >= 3)
                    {
                        
                        // Instrument Beam 0
                        if (bt.InstrumentVelocity[0] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            BtVelocityBeam0 = (short)(Math.Round(bt.InstrumentVelocity[0] * 1000.0f * -1.0f));   // mm/s to m/s 
                        }
                        else
                        {
                            // Bad velocity
                            BtVelocityBeam0 = PD0.BAD_VELOCITY;
                        }

                        // Instrument Beam 1
                        if (bt.InstrumentVelocity[1] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            BtVelocityBeam1 = (short)(Math.Round(bt.InstrumentVelocity[1] * 1000.0f * -1.0f));   // mm/s to m/s 
                        }
                        else
                        {
                            // Bad velocity
                            BtVelocityBeam1 = PD0.BAD_VELOCITY;
                        }

                        // Instrument Beam -2
                        if (bt.InstrumentVelocity[2] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            BtVelocityBeam2 = (short)(Math.Round(bt.InstrumentVelocity[2] * 1000.0f * 1.0f));   // mm/s to m/s 
                        }
                        else
                        {
                            // Bad velocity
                            BtVelocityBeam2 = PD0.BAD_VELOCITY;
                        }
                    }
                    // Vertical Beam
                    else if (bt.NumBeams == 1)
                    {
                        // Instrument Beam 1
                        if (bt.InstrumentVelocity[0] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            BtVelocityBeam0 = (short)(Math.Round(bt.InstrumentVelocity[0] * 1000.0f * -1.0f));   // mm/s to m/s 
                        }
                        else
                        {
                            // Bad velocity
                            BtVelocityBeam0 = PD0.BAD_VELOCITY;
                        }
                    }
                    break;

                // Instrument Coordinate Transform
                case PD0.CoordinateTransforms.Coord_Ship:
                    // 4 Beam System
                    if (bt.NumBeams >= 4)
                    {
                        // Ship Beam 1
                        if (bt.ShipVelocity[0] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            BtVelocityBeam1 = (short)(Math.Round(bt.ShipVelocity[0] * 1000.0f * -1.0f));   // mm/s to m/s 
                        }
                        else
                        {
                            // Bad velocity
                            BtVelocityBeam1 = PD0.BAD_VELOCITY;
                        }

                        // Ship Beam 0
                        if (bt.ShipVelocity[1] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            BtVelocityBeam0 = (short)(Math.Round(bt.ShipVelocity[1] * 1000.0f * -1.0f));   // mm/s to m/s 
                        }
                        else
                        {
                            // Bad velocity
                            BtVelocityBeam0 = PD0.BAD_VELOCITY;
                        }

                        // Ship Beam -2
                        if (bt.ShipVelocity[2] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            BtVelocityBeam2 = (short)(Math.Round(bt.ShipVelocity[2] * 1000.0f * 1.0f));   // mm/s to m/s 
                        }
                        else
                        {
                            // Bad velocity
                            BtVelocityBeam2 = PD0.BAD_VELOCITY;
                        }

                        // Ship Beam 3
                        if (bt.ShipVelocity[3] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            BtVelocityBeam3 = (short)(Math.Round(bt.ShipVelocity[3] * 1000.0f * -1.0f));   // mm/s to m/s 
                        }
                        else
                        {
                            // Bad velocity
                            BtVelocityBeam3 = PD0.BAD_VELOCITY;
                        }
                    }
                    // 3 Beam System
                    else if (bt.NumBeams >= 3)
                    {

                        // Ship Beam 0
                        if (bt.ShipVelocity[0] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            BtVelocityBeam0 = (short)(Math.Round(bt.ShipVelocity[0] * 1000.0f * -1.0f));   // mm/s to m/s 
                        }
                        else
                        {
                            // Bad velocity
                            BtVelocityBeam0 = PD0.BAD_VELOCITY;
                        }

                        // Ship Beam 1
                        if (bt.ShipVelocity[1] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            BtVelocityBeam1 = (short)(Math.Round(bt.ShipVelocity[1] * 1000.0f * -1.0f));   // mm/s to m/s 
                        }
                        else
                        {
                            // Bad velocity
                            BtVelocityBeam1 = PD0.BAD_VELOCITY;
                        }

                        // Ship Beam -2
                        if (bt.ShipVelocity[2] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            BtVelocityBeam2 = (short)(Math.Round(bt.ShipVelocity[2] * 1000.0f * 1.0f));   // mm/s to m/s 
                        }
                        else
                        {
                            // Bad velocity
                            BtVelocityBeam2 = PD0.BAD_VELOCITY;
                        }
                    }
                    // Vertical Beam
                    else if (bt.NumBeams == 1)
                    {
                        // Ship Beam 1
                        if (bt.ShipVelocity[0] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            BtVelocityBeam0 = (short)(Math.Round(bt.ShipVelocity[0] * 1000.0f * -1.0f));   // mm/s to m/s 
                        }
                        else
                        {
                            // Bad velocity
                            BtVelocityBeam0 = PD0.BAD_VELOCITY;
                        }
                    }
                    break;

                default:
                    break;
            }

            #endregion

            #region Correlation

            // 4 Beam System
            if (bt.NumBeams >= 4)
            {
                // beam order 3,2,0,1

                BtCorrelationMagnitudeBeam0 = (byte)(Math.Round(bt.Correlation[3] * 255.0f));
                BtCorrelationMagnitudeBeam1 = (byte)(Math.Round(bt.Correlation[2] * 255.0f));
                BtCorrelationMagnitudeBeam2 = (byte)(Math.Round(bt.Correlation[0] * 255.0f));
                BtCorrelationMagnitudeBeam3 = (byte)(Math.Round(bt.Correlation[1] * 255.0f));
            }
            else if (bt.NumBeams >= 3)
            {
                // beam order 3,2,0,1

                BtCorrelationMagnitudeBeam0 = (byte)(Math.Round(bt.Correlation[0] * 255.0f));
                BtCorrelationMagnitudeBeam1 = (byte)(Math.Round(bt.Correlation[1] * 255.0f));
                BtCorrelationMagnitudeBeam2 = (byte)(Math.Round(bt.Correlation[2] * 255.0f));
            }
            // Vertical Beam
            else if (bt.NumBeams == 1)
            {
                BtCorrelationMagnitudeBeam0 = (byte)(Math.Round(bt.Correlation[0] * 255.0f));
            }

            #endregion

            #region Evaluation Amplitude

            // 4 Beam System
            if (bt.NumBeams >= 4)
            {
                // beam order 3,2,0,1

                BtAmplitudeBeam0 = (byte)(Math.Round(bt.SNR[3] * 2.0f)); //0.5 counts per dB
                if (BtAmplitudeBeam0 > PD0.BAD_AMPLITUDE)
                {
                    BtAmplitudeBeam0 = PD0.BAD_AMPLITUDE;
                }

                BtAmplitudeBeam1 = (byte)(Math.Round(bt.SNR[2] * 2.0f)); //0.5 counts per dB
                if (BtAmplitudeBeam1 > PD0.BAD_AMPLITUDE)
                {
                    BtAmplitudeBeam1 = PD0.BAD_AMPLITUDE;
                }

                BtAmplitudeBeam2 = (byte)(Math.Round(bt.SNR[0] * 2.0f)); //0.5 counts per dB
                if (BtAmplitudeBeam2 > PD0.BAD_AMPLITUDE)
                {
                    BtAmplitudeBeam2 = PD0.BAD_AMPLITUDE;
                }

                BtAmplitudeBeam3 = (byte)(Math.Round(bt.SNR[1] * 2.0f)); //0.5 counts per dB
                if (BtAmplitudeBeam3 > PD0.BAD_AMPLITUDE)
                {
                    BtAmplitudeBeam3 = PD0.BAD_AMPLITUDE;
                }
            }
            // 3 Beam System
            else if (bt.NumBeams >= 3)
            {
                BtAmplitudeBeam0 = (byte)(Math.Round(bt.SNR[0] * 2.0f)); //0.5 counts per dB
                if (BtAmplitudeBeam0 > PD0.BAD_AMPLITUDE)
                {
                    BtAmplitudeBeam0 = PD0.BAD_AMPLITUDE;
                }

                BtAmplitudeBeam1 = (byte)(Math.Round(bt.SNR[1] * 2.0f)); //0.5 counts per dB
                if (BtAmplitudeBeam1 > PD0.BAD_AMPLITUDE)
                {
                    BtAmplitudeBeam1 = PD0.BAD_AMPLITUDE;
                }

                BtAmplitudeBeam2 = (byte)(Math.Round(bt.SNR[2] * 2.0f)); //0.5 counts per dB
                if (BtAmplitudeBeam2 > PD0.BAD_AMPLITUDE)
                {
                    BtAmplitudeBeam2 = PD0.BAD_AMPLITUDE;
                }
            }
            // Vertical Beam
            else if (bt.NumBeams == 1)
            {
                BtAmplitudeBeam0 = (byte)(Math.Round(bt.SNR[0] * 2.0f)); //0.5 counts per dB
                if (BtAmplitudeBeam0 > PD0.BAD_AMPLITUDE)
                {
                    BtAmplitudeBeam0 = PD0.BAD_AMPLITUDE;
                }
            }

            #endregion

            #region Percent Good

            // 4 Beam System
            if (bt.NumBeams >= 4)
            {
                // beam order 3,2,0,1

                BtPercentGoodBeam0 = (byte)(Math.Round((bt.EarthGood[3] * 100.0f) / BtPingsPerEnsemble));
                if (BtPercentGoodBeam0 > PD0.BAD_PERCENT_GOOD)
                {
                    BtPercentGoodBeam0 = PD0.BAD_PERCENT_GOOD;
                }

                BtPercentGoodBeam1 = (byte)(Math.Round((bt.EarthGood[2] * 100.0f) / BtPingsPerEnsemble));
                if (BtPercentGoodBeam1 > PD0.BAD_PERCENT_GOOD)
                {
                    BtPercentGoodBeam1 = PD0.BAD_PERCENT_GOOD;
                }

                BtPercentGoodBeam2 = (byte)(Math.Round((bt.EarthGood[0] * 100.0f) / BtPingsPerEnsemble));
                if (BtPercentGoodBeam2 > PD0.BAD_PERCENT_GOOD)
                {
                    BtPercentGoodBeam2 = PD0.BAD_PERCENT_GOOD;
                }

                BtPercentGoodBeam3 = (byte)(Math.Round((bt.EarthGood[1] * 100.0f) / BtPingsPerEnsemble));
                if (BtPercentGoodBeam3 > PD0.BAD_PERCENT_GOOD)
                {
                    BtPercentGoodBeam3 = PD0.BAD_PERCENT_GOOD;
                }
            }
            // 4 Beam System
            else if (bt.NumBeams >= 3)
            {
                BtPercentGoodBeam0 = (byte)(Math.Round((bt.EarthGood[0] * 100.0f) / BtPingsPerEnsemble));
                if (BtPercentGoodBeam0 > PD0.BAD_PERCENT_GOOD)
                {
                    BtPercentGoodBeam0 = PD0.BAD_PERCENT_GOOD;
                }

                BtPercentGoodBeam1 = (byte)(Math.Round((bt.EarthGood[1] * 100.0f) / BtPingsPerEnsemble));
                if (BtPercentGoodBeam1 > PD0.BAD_PERCENT_GOOD)
                {
                    BtPercentGoodBeam1 = PD0.BAD_PERCENT_GOOD;
                }

                BtPercentGoodBeam2 = (byte)(Math.Round((bt.EarthGood[2] * 100.0f) / BtPingsPerEnsemble));
                if (BtPercentGoodBeam2 > PD0.BAD_PERCENT_GOOD)
                {
                    BtPercentGoodBeam2 = PD0.BAD_PERCENT_GOOD;
                }
            }
            // Vertical Beam
            else if (bt.NumBeams == 1)
            {
                BtPercentGoodBeam0 = (byte)(Math.Round((bt.EarthGood[0] * 100.0f) / BtPingsPerEnsemble));
                if (BtPercentGoodBeam0 > PD0.BAD_PERCENT_GOOD)
                {
                    BtPercentGoodBeam0 = PD0.BAD_PERCENT_GOOD;
                }
            }

            #endregion

            #region Reference Layer

            BtRefLayerFar = 0;
            BtRefLayerMin = 0;
            BtRefLayerNear = 0;
            BtRefLayerVelocityBeam0 = 0;
            BtRefLayerVelocityBeam1 = 0;
            BtRefLayerVelocityBeam2 = 0;
            BtRefLayerVelocityBeam3 = 0;
            BtRefLayerCorrBeam0 = 0;
            BtRefLayerCorrBeam1 = 0;
            BtRefLayerCorrBeam2 = 0;
            BtRefLayerCorrBeam3 = 0;
            BtRefLayerEchoIntensityBeam0 = 0;
            BtRefLayerEchoIntensityBeam1 = 0;
            BtRefLayerEchoIntensityBeam2 = 0;
            BtRefLayerEchoIntensityBeam3 = 0;
            BtRefLayerPercentGoodBeam0 = 0;
            BtRefLayerPercentGoodBeam1 = 0;
            BtRefLayerPercentGoodBeam2 = 0;
            BtRefLayerPercentGoodBeam3 = 0;

            #endregion

            BtMaxDepth = 0;                                         // Max Depth - CBTMX - Do not have enough info

            #region RSSI

            // 4 Beam System
            if (bt.NumBeams >= 4)
            {
                // beam order 3,2,0,1

                BtRssiBeam0 = bt.Amplitude[3] * 2.0f; //0.5 counts per dB
                if (BtRssiBeam0 > PD0.BAD_AMPLITUDE)
                {
                    BtRssiBeam0 = PD0.BAD_AMPLITUDE;
                }

                BtRssiBeam1 = bt.Amplitude[2] * 2.0f; //0.5 counts per dB
                if (BtRssiBeam1 > PD0.BAD_AMPLITUDE)
                {
                    BtRssiBeam1 = PD0.BAD_AMPLITUDE;
                }

                BtRssiBeam2 = bt.Amplitude[0] * 2.0f; //0.5 counts per dB
                if (BtRssiBeam2 > PD0.BAD_AMPLITUDE)
                {
                    BtRssiBeam2 = PD0.BAD_AMPLITUDE;
                }

                BtRssiBeam3 = bt.Amplitude[1] * 2.0f; //0.5 counts per dB
                if (BtRssiBeam3 > PD0.BAD_AMPLITUDE)
                {
                    BtRssiBeam3 = PD0.BAD_AMPLITUDE;
                }
            }
            else if (bt.NumBeams >= 3)
            {
                BtRssiBeam0 = bt.Amplitude[0] * 2.0f; //0.5 counts per dB
                if (BtRssiBeam0 > PD0.BAD_AMPLITUDE)
                {
                    BtRssiBeam0 = PD0.BAD_AMPLITUDE;
                }

                BtRssiBeam1 = bt.Amplitude[1] * 2.0f; //0.5 counts per dB
                if (BtRssiBeam1 > PD0.BAD_AMPLITUDE)
                {
                    BtRssiBeam1 = PD0.BAD_AMPLITUDE;
                }

                BtRssiBeam2 = bt.Amplitude[2] * 2.0f; //0.5 counts per dB
                if (BtRssiBeam2 > PD0.BAD_AMPLITUDE)
                {
                    BtRssiBeam2 = PD0.BAD_AMPLITUDE;
                }
            }
            // Vertical Beam
            else if (bt.NumBeams == 1)
            {
                BtRssiBeam0 = bt.Amplitude[0] * 2.0f; //0.5 counts per dB
                if (BtRssiBeam0 > PD0.BAD_AMPLITUDE)
                {
                    BtRssiBeam0 = PD0.BAD_AMPLITUDE;
                }
            }

            #endregion

            BtGain = 0x01;                                          // Gain
            Reserved82_85 = 0;                                      // Reserved
        }
    }
}
