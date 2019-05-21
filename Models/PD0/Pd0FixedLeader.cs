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
 * 02/25/2014      RC          2.21.4     Initial coding
 * 07/16/2014      RC          2.23.0     Check if the values are given in DecodeRtiEnsemble().
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
    using System.Collections;

    /// <summary>
    /// Fixed Leader for the PD0 Ensemble.  This
    /// data format include information about the ensemble and
    /// the ADCP.
    /// </summary>
    public class Pd0FixedLeader : Pd0DataType
    {
        #region Class and Enum

        /// <summary>
        /// System frequency types.
        /// </summary>
        public enum SystemFrequency
        {
            /// <summary>
            /// 75 kHz.
            /// </summary>
            Freq_75kHz,

            /// <summary>
            /// 150 kHz.
            /// </summary>
            Freq_150kHz,

            /// <summary>
            /// 300 kHz.
            /// </summary>
            Freq_300kHz,

            /// <summary>
            /// 600 kHz.
            /// </summary>
            Freq_600kHz,

            /// <summary>
            /// 1200 kHz.
            /// </summary>
            Freq_1200kHz,

            /// <summary>
            /// 2400 kHz.
            /// </summary>
            Freq_2400kHz
        }

        /// <summary>
        /// Beam angle types.
        /// </summary>
        public enum BeamAngles
        {
            /// <summary>
            /// 15 Degrees Beam Angle.
            /// </summary>
            BeamAngle_15_Degree,

            /// <summary>
            /// 20 Degrees Beam Angle.
            /// </summary>
            BeamAngle_20_Degree,

            /// <summary>
            /// 30 Degrees Beam Angle.
            /// </summary>
            BeamAngle_30_Degree,

            /// <summary>
            /// Other Beam Angle.
            /// </summary>
            BeamAngle_Other
        }

        /// <summary>
        /// Beam configuration types.
        /// </summary>
        public enum BeamConfigs
        {
            /// <summary>
            /// 4-Beam Janus Config.
            /// </summary>
            BeamConfig_4_Beam_Janus,

            /// <summary>
            /// 5-Beam Janus Config Demod.
            /// </summary>
            BeamConfig_5_Beam_Janus_Demod,

            /// <summary>
            /// 5-Beam Janus Config 2 Demod.
            /// </summary>
            BeamConfig_5_Beam_Janus_2_Demod
        }

        #endregion

        #region Variables

        /// <summary>
        /// Number of bytes required for the Data Type.
        /// </summary>
        public const int DATATYPE_SIZE = 59;

        /// <summary>
        /// LSB for the ID for the PD0 Fixed Leader data type.
        /// </summary>
        public const byte ID_LSB = 0x00;

        /// <summary>
        /// MSB for the ID for the PD0 Fixed Leader data type.
        /// </summary>
        public const byte ID_MSB = 0x00;

        #region Minimum and Maximum Value

        /// <summary>
        /// Minimum Number of depth cells. (Depth Cells)
        /// </summary>
        public const byte NUM_DEPTH_CELL_MIN = 1;

        /// <summary>
        /// Maximum number of depth cells. (Depth Cells)
        /// </summary>
        public const byte NUM_DEPTH_CELL_MAX = 128;

        /// <summary>
        /// Minimum Pings per Ensembles. (Pings)
        /// </summary>
        public const int PINGS_PER_ENSEMBLE_MIN = 0;

        /// <summary>
        /// Maximum Pings per Ensembles.  (Pings)
        /// </summary>
        public const int PINGS_PER_ENSEMBLE_MAX = 16384;

        /// <summary>
        /// Minimum Depth Cell Length. (Centimeter)
        /// </summary>
        public const int DEPTH_CELL_LENGTH_MIN = 1;

        /// <summary>
        /// Maximum Depth Cell Length (Centimeter)
        /// 210 feet.
        /// </summary>
        public const int DEPTH_CELL_LENGTH_MAX = 6400;

        /// <summary>
        /// Minimum Blank After Transmit. (Centimeter)
        /// </summary>
        public const int BLANK_MIN = 0;

        /// <summary>
        /// Maximum Blank After Transmit. (Centimeter)
        /// </summary>
        public const int BLANK_MAX = 9999;

        /// <summary>
        /// Minimum Low Correlation Threshold. (counts)
        /// </summary>
        public const byte LOW_CORR_THRESH_MIN = 0;

        /// <summary>
        /// Maximum Low Correlation Threshold. (counts)
        /// </summary>
        public const byte LOW_CORR_THRESH_MAX = 255;

        /// <summary>
        /// Minimum Number of Code Repeats.  (counts)
        /// </summary>
        public const byte NUM_CODE_REPEATS_MIN = 0;

        /// <summary>
        /// Maximum Number of Code Repeats. (counts)
        /// </summary>
        public const byte NUM_CODE_REPEATS_MAX = 255;

        /// <summary>
        /// Minimum Percent Good Minimum. (percent)
        /// </summary>
        public const byte PERCENT_GOOD_MINIMUM_MIN = 1;

        /// <summary>
        /// Maximum Percent Good Minimum. (percent)
        /// </summary>
        public const byte PERCENT_GOOD_MINIMUM_MAX = 100;

        /// <summary>
        /// Minimum Error Velocity Threshold. (mm/s)
        /// </summary>
        public const int ERR_VEL_THRESHOLD_MIN = 0;

        /// <summary>
        /// Maximum Error Velocity Threshold. (mm/s)
        /// </summary>
        public const int ERR_VEL_THRESHOLD_MAX = 5000;

        /// <summary>
        /// Minimum Heading Alignment. (degree)
        /// </summary>
        public const double HEADING_ALIGNMENT_MIN = -179.99;

        /// <summary>
        /// Maximum Heading Alignment. (degree)
        /// </summary>
        public const double HEADING_ALIGNMENT_MAX = 180.00;

        /// <summary>
        /// Minimum Heading Bias.  (degree)
        /// </summary>
        public const double HEADING_BIAS_MIN = -179.99;

        /// <summary>
        /// Maximum Heading Bias. (degree)
        /// </summary>
        public const double HEADING_BIAS_MAX = 180.00;

        /// <summary>
        /// Minimum Bin 1 Distance.  (centimeter)
        /// </summary>
        public const int BIN_1_DISTANCE_MIN = 0;

        /// <summary>
        /// Maximum Bin 1 Distance. (centimeter)
        /// 2150 feet
        /// </summary>
        public const int BIN_1_DISTANCE_MAX = 65535;

        /// <summary>
        /// Minimum Transmit Pulse Length. (centimeter)
        /// </summary>
        public const int XMIT_PULSE_LENGTH_MIN = 0;

        /// <summary>
        /// Maximum Transmit Pulse Length. (centimeter)
        /// </summary>
        public const int XMIT_PULSE_LENGTH_MAX = 65535;

        /// <summary>
        /// Minimum Reference Layer Average Depth Cell.  (depth cell)
        /// </summary>
        public const byte REF_LAYER_AVG_DEPTH_CELL_MIN = 1;

        /// <summary>
        /// Maximum Reference Layer Average Depth Cell.  (depth cell)
        /// </summary>
        public const byte REF_LAYER_AVG_DEPTH_CELL_MAX = 128;

        /// <summary>
        /// Minimum False Target Threshold.  (counts)
        /// </summary>
        public const byte FALSE_TARGET_THRESH_MIN = 0;

        /// <summary>
        /// Maximum False Target Threshold.  (counts)
        /// </summary>
        public const byte FALSE_TARGET_THRESH_MAX = 255;

        /// <summary>
        /// Minimum Spare.
        /// </summary>
        public const byte SPARE_MIN = 0;

        /// <summary>
        /// Maximum Spare.
        /// </summary>
        public const byte SPARE_MAX = 5;

        /// <summary>
        /// Minimum Transmit Lag Distance. (centimeter)
        /// </summary>
        public const int TRANSMIT_LAG_DISTANCE_MIN = 0;

        /// <summary>
        /// Maximum Transmit Lag Distance.  (centimeter)
        /// </summary>
        public const int TRANSMIT_LAG_DISTANCE_MAX = 65535;

        /// <summary>
        /// Minimum System Bandwidth.  
        /// </summary>
        public const byte SYSTEM_BANDWIDTH_MIN = 0;

        /// <summary>
        /// Maximum System Bandwidth.
        /// </summary>
        public const byte SYSTEM_BANDWIDTH_MAX = 1;

        /// <summary>
        /// Minimum System Power.
        /// </summary>
        public const byte SYSTEM_POWER_MIN = 0;


        /// <summary>
        /// Maximum System Power.
        /// </summary>
        public const byte SYSTEM_POWER_MAX = 255;

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// CPU Firmware Version.
        /// 
        /// Contains the version number of the CPU Firmware.
        /// </summary>
        public byte CpuFirmwareVersion { get; set; }

        /// <summary>
        /// CPU Firmware Revision.
        /// 
        /// Contains the Revision number of the CPU Firmware.
        /// </summary>
        public byte CpuFirmwareRevision { get; set; }

        /// <summary>
        /// System Configuration.
        /// 
        /// This field defines the Workhorse
        /// hardware configuration. Convert this field (2 bytes, LSB first) to binary and interpret as follows
        /// 
        /// LSB
        /// BITS 7 6 5 4 3 2 1 0
        ///      - - - - - 0 0 0 75-kHz SYSTEM
        ///      - - - - - 0 0 1 150-kHz SYSTEM
        ///      - - - - - 0 1 0 300-kHz SYSTEM
        ///      - - - - - 0 1 1 600-kHz SYSTEM
        ///      - - - - - 1 0 0 1200-kHz SYSTEM
        ///      - - - - - 1 0 1 2400-kHz SYSTEM
        ///      - - - - 0 - - - CONCAVE BEAM PAT.
        ///      - - - - 1 - - - CONVEX BEAM PAT.
        ///      - - 0 0 - - - - SENSOR CONFIG #1
        ///      - - 0 1 - - - - SENSOR CONFIG #2
        ///      - - 1 0 - - - - SENSOR CONFIG #3
        ///      - 0 - - - - - - XDCR HD NOT ATT.
        ///      - 1 - - - - - - XDCR HD ATTACHED
        ///      0 - - - - - - - DOWN FACING BEAM
        ///      1 - - - - - - - UP-FACING BEAM
        /// MSB
        /// BITS 7 6 5 4 3 2 1 0
        ///      - - - - - - 0 0 15E BEAM ANGLE
        ///      - - - - - - 0 1 20E BEAM ANGLE
        ///      - - - - - - 1 0 30E BEAM ANGLE
        ///      - - - - - - 1 1 OTHER BEAM ANGLE
        ///      0 1 0 0 - - - - 4-BEAM JANUS CONFIG
        ///      0 1 0 1 - - - - 5-BM JANUS CFIG DEMOD)
        ///      1 1 1 1 - - - - 5-BM JANUS CFIG.(2 DEMD) 
        /// 
        /// Example: Hex 5249 (i.e., hex 49 followed by hex 52) identifies a 150-kHz system, convex beam pattern, down-facing, 30E beam angle, 5 beams (3 demods). 
        /// 
        /// </summary>
        public short SystemConfiguration { get; set; }

        /// <summary>
        /// Real or simulation flag.
        /// This field is set by default as real data (0). 
        /// </summary>
        public bool RealSimFlag { get; set; }

        /// <summary>
        /// Lag length.
        /// Lag Length. The lag is the time period between sound pulses.
        /// This is varied, and therefore of interest in, at a minimum, for the
        /// WM5, WM8 and WM11 and BM7 commands. 
        /// </summary>
        public byte LagLength { get; set; }

        /// <summary>
        /// Number of beams.
        /// Contains the number of beams used to calculate velocity data
        /// (not physical beams). The Workhorse needs only three beams
        /// to calculate water-current velocities. The fourth beam provides
        /// an error velocity that determines data validity. If only three
        /// beams are available, the Workhorse does not make this validity
        /// check.  Table 37, page 143 (Percent-Good Data Format) has
        /// more information
        /// </summary>
        public byte NumberOfBeams { get; set; }

        /// <summary>
        /// Number of depth cells.
        /// Contains the number of depth cells over which the Workhorse
        /// collects data (WN-command).
        /// 
        /// Scaling: LSD = 1 depth cell; Range = 1 to 128 depth cells
        /// </summary>
        public byte NumberOfCells { get; set; }

        /// <summary>
        /// Pings per Ensemble.
        /// Contains the number of pings averaged together during a data
        /// ensemble (WP-command). If WP = 0, the Workhorse does not
        /// collect the WD water-profile data. Note: The Workhorse 
        /// automatically extends the ensemble interval (TE) 
        /// if the product of WP and time per ping (TP) 
        /// is greater than TE.
        /// 
        /// Scaling: LSD = 1 ping; Range = 0 to 16,384 pings 
        /// </summary>
        public ushort PingsPerEnsemble { get; set; }

        /// <summary>
        /// Depth Cell Length.
        /// 
        /// Contains the length of one depth cell (WS-command). 
        /// </summary>
        public ushort DepthCellLength { get; set; }

        /// <summary>
        /// Blank After Transmit.
        /// 
        /// Contains the blanking distance used by the Workhorse to allow
        /// the transmit circuits time to
        /// recover before the receive cycle
        /// begins (WF-command).
        /// 
        /// Scaling: LSD = 1 centimeter; Range = 0 to 9999 cm (328 feet)
        /// </summary>
        public ushort BlankAfterTransmit { get; set; }

        /// <summary>
        /// Profiling Mode.
        /// 
        /// Contains the Signal Processing Mode. This field will always be
        /// set to 1. 
        /// </summary>
        public byte ProfilingMode { get; set; }

        /// <summary>
        /// Low Correlation Threshold.
        /// Contains the minimum threshold of correlation that water-profile
        /// data can have to be considered good data (WC-command).
        /// 
        /// Scaling: LSD = 1 count; Range = 0 to 255 counts 
        /// </summary>
        public byte LowCorrThresh { get; set; }

        /// <summary>
        /// Number of Code Repeats.
        /// Contains the number of code repetitions in the transmit pulse.
        /// 
        /// Scaling: LSD = 1 count; Range = 0 to 255 counts 
        /// </summary>
        public byte NumCodeRepeats { get; set; }

        /// <summary>
        /// Percent Good Minimum.
        /// Contains the minimum percentage
        /// of water-profiling pings in an
        /// ensemble that must be considered good to output velocity data.
        /// 
        /// Scaling: LSD = 1 percent; Range = 1 to 100 percent 
        /// </summary>
        public byte PercentGoodMinimum { get; set; }

        /// <summary>
        /// Error Velocity Maximum.
        /// This field, initially set by the WE-command, contains the actual
        /// threshold value used to flag water-current data as good or bad.
        /// If the error velocity value exceeds
        /// this threshold, the Workhorse
        /// flags all four beams of the affected bin as bad.
        /// 
        /// Scaling: LSD = 1 mm/s; Range = 0 to 5000 mm/s 
        /// </summary>
        public ushort ErrorVelMaximum { get; set; }

        /// <summary>
        /// Time between Pings Minutes.
        /// These fields, set by the TP-command, contain the amount of
        /// time between ping groups in the ensemble. NOTE: The Workhorse automatically 
        /// extends the ensemble interval (set by TE) if (WP x TP (greater than) TE). 
        /// </summary>
        public byte TimeBetweenPingMinutes { get; set; }

        /// <summary>
        /// Time Between Pings Seconds.
        /// These fields, set by the TP-command, contain the amount of
        /// time between ping groups in the ensemble. NOTE: The Workhorse automatically 
        /// extends the ensemble interval (set by TE) if (WP x TP (greater than) TE). 
        /// </summary>
        public byte TimeBetweenPingSeconds { get; set; }

        /// <summary>
        /// Time Between Pings Hundredths of a Second.
        /// These fields, set by the TP-command, contain the amount of
        /// time between ping groups in the ensemble. NOTE: The Workhorse automatically 
        /// extends the ensemble interval (set by TE) if (WP x TP (greater than) TE). 
        /// </summary>
        public byte TimeBetweenPingHundredths { get; set; }

        /// <summary>
        /// Coordinate Transform.
        /// Contains the coordinate transformation processing parameters
        /// (EX-command). These firmware switches indicate how the
        /// Workhorse collected data.
        /// xxx00xxx = NO TRANSFORMATION (BEAM COORDINATES)
        /// xxx01xxx = INSTRUMENT COORDINATES
        /// xxx10xxx = SHIP COORDINATES
        /// xxx11xxx = EARTH COORDINATES
        /// xxxxx1xx = TILTS (PITCH AND ROLL) USED IN SHIP OR EARTH TRANSFORMATION
        /// xxxxxx1x = 3-BEAM SOLUTION USED IF ONE BEAM IS BELOW THE CORRELATION THRESHOLD SET BY THE WC-COMMAND
        /// xxxxxxx1 = BIN MAPPING USED
        /// </summary>
        public byte CoordinateTransform { get; set; }

        /// <summary>
        /// Heading Alignment.
        /// Contains a correction factor for physical heading misalignment (EA-command).
        /// 
        /// Scaling: LSD = 0.01 degree; Range = -179.99 to 180.00 degrees 
        /// </summary>
        public float HeadingAlignment { get; set; }

        /// <summary>
        /// Heading Bias.
        /// Contains a correction factor for electrical/magnetic heading bias (EB-command).
        /// 
        /// Scaling: LSD = 0.01 degree; Range = -179.99 to 180.00 degrees 
        /// </summary>
        public float HeadingBias { get; set; }

        /// <summary>
        /// Heading source.
        /// Contains the selected source of environmental sensor data
        /// (EZ-command). These firmware switches indicate the following.
        /// FIELD DESCRIPTION
        /// x1xxxxxx = CALCULATES EC (SPEED OF SOUND) FROM ED, ES, AND ET
        /// xx1xxxxx = USES ED FROM DEPTH SENSOR
        /// xxx1xxxx = USES EH FROM TRANSDUCER HEADING SENSOR
        /// xxxx1xxx = USES EP FROM TRANSDUCER PITCH SENSOR
        /// xxxxx1xx = USES ER FROM TRANSDUCER ROLL SENSOR
        /// xxxxxx1x = USES ES (SALINITY) FROM CONDUCTIVITY SENSOR
        /// xxxxxxx1 = USES ET FROM TRANSDUCER TEMPERATURE SENSOR
        /// 
        /// NOTE: If the field = 0, or if the sensor is not available, the
        /// Workhorse uses the manual command setting. If the field = 1,
        /// the Workhorse uses the reading from the internal sensor or an
        /// external synchro sensor (only applicable to heading, roll, and
        /// pitch). Although you can enter a “2” in the EZ-command string,
        /// the Workhorse only displays a 0 (manual) or 1 (int/ext sensor). 
        /// </summary>
        public byte SensorSource { get; set; }

        /// <summary>
        /// Sensors Available.
        /// This field reflects which sensors are available. The bit pattern
        /// is the same as listed for the EZ-command (above). 
        /// </summary>
        public byte SensorsAvailable { get; set; }

        /// <summary>
        /// Bin 1 Distance.
        /// This field contains the distance
        /// to the middle of the first depth
        /// cell (bin). This distance is a function of depth cell length (WS),
        /// the profiling mode (WM), the blank
        /// after transmit distance (WF),
        /// and speed of sound.
        /// 
        /// Scaling: LSD = 1 centimeter; Range = 0 to 65535 cm (2150 feet) 
        /// </summary>
        public ushort Bin1Distance { get; set; }

        /// <summary>
        /// Transmit Pulse Length.
        /// This field, set by the WT-command, contains the length of the
        /// transmit pulse. When the Workhorse receives a [BREAK]
        /// signal, it sets the transmit pulse length as close as possible to
        /// the depth cell length (WS-command). This means the Workhorse uses a WT command
        /// of zero. However, the WT field contains the actual length
        /// of the transmit pulse used.
        /// 
        /// Scaling: LSD = 1 centimeter; Range = 0 to 65535 cm (2150 feet) 
        /// </summary>
        public ushort XmitPulseLength { get; set; }

        /// <summary>
        /// Reference Layer Averaging Start Cell.
        /// Contains the starting depth cell (LSB, byte 37) and the ending
        /// depth cell (MSB, byte 38) used for water reference layer averaging (WL-command).
        /// 
        /// Scaling: LSD = 1 depth cell; Range = 1 to 128 depth cells 
        /// </summary>
        public byte ReferenceLayerAverageStartCell { get; set; }

        /// <summary>
        /// Reference Layer Averaging End Cell.
        /// Contains the starting depth cell (LSB, byte 37) and the ending
        /// depth cell (MSB, byte 38) used for water reference layer averaging (WL-command).
        /// 
        /// Scaling: LSD = 1 depth cell; Range = 1 to 128 depth cells 
        /// </summary>
        public byte ReferenceLayerAverageEndCell { get; set; }

        /// <summary>
        /// False Target Threshold.
        /// Contains the threshold value used to reject data received from
        /// a false target, usually fish (WA-command).
        /// 
        /// Scaling: LSD = 1 count; Range = 0 to 255 counts (255 disables) 
        /// </summary>
        public byte FalseTargetThresh { get; set; }

        /// <summary>
        /// Spare.  Bit 40
        /// Contains the CX-command setting. Range = 0 to 5 
        /// </summary>
        public byte Spare_40 { get; set; }

        /// <summary>
        /// Transmit Lag Distance.
        /// This field, determined mainly by the setting of the WM-command, contains the distance
        /// between pulse repetitions.
        /// 
        /// Scaling: LSD = 1 centimeter; Range = 0 to 65535 centimeters 
        /// </summary>
        public ushort TransmitLagDistance { get; set; }

        /// <summary>
        /// CPU Board Serial Number.
        /// Contains the serial number of the CPU board
        /// </summary>
        public string CpuBoardSerialNumber { get; set; }

        /// <summary>
        /// System Bandwidth.
        /// Contains the WB-command setting. Range = 0 to 1
        /// </summary>
        public ushort SystemBandwidth { get; set; }

        /// <summary>
        /// System Power.
        /// Contains the CQ-command setting for WorkHorse Monitor/Sentinel/Long Ranger ADCPs. Range 0 to 255.
        /// This byte is Spare for Navigator ADCP/DVLS
        /// </summary>
        public byte SystemPower { get; set; }

        /// <summary>
        /// Base Frequency Index.
        /// Base frequency index (Navigators only). This byte is Spare for
        /// other WorkHorse ADCPs. 
        /// </summary>
        public byte BaseFrequencyIndex { get; set; }

        /// <summary>
        /// Instrument serial number.
        /// Instrument serial number
        /// (REMUS only). This byte is Spare for
        /// other WorkHorse ADCPs. 
        /// </summary>
        public string InstrumentSerialNumber { get; set; }

        /// <summary>
        /// Beam Angle.
        /// Beam angle (H-ADCP only). This byte is Spare for other
        /// WorkHorse ADCPs. 
        /// </summary>
        public byte BeamAngle { get; set; }

        #endregion

        /// <summary>
        /// Initialize the Fixed Leader.
        /// </summary>
        public Pd0FixedLeader()
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.FixedLeader)
        {
            Initialize();
        }

        /// <summary>
        /// Initialize the Fixed Leader.
        /// </summary>
        /// <param name="data">Binary data.</param>
        public Pd0FixedLeader(byte[] data)
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.FixedLeader)
        {
            Initialize();
            Decode(data);
        }

        /// <summary>
        /// Initialize the Fixed Leader.
        /// </summary>
        /// <param name="data">Binary data.</param>
        /// <param name="offset">Offset within the binary ensemble.</param>
        public Pd0FixedLeader(byte[] data, ushort offset)
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.FixedLeader)
        {
            this.Offset = offset;
            Initialize();
            Decode(data);
        }

        /// <summary>
        /// Initialize the data type.
        /// Take the RTI data sets to 
        /// initialize the object.
        /// </summary>
        /// <param name="ens">RTI Ensemble aata set.</param>
        /// <param name="anc">RTI Ancillary data set.</param>
        /// <param name="sysSetup">SystemSetup data set.</param>
        /// <param name="xform">Coordinate transform used for this ensemble.</param>
        public Pd0FixedLeader(DataSet.EnsembleDataSet ens, DataSet.AncillaryDataSet anc, DataSet.SystemSetupDataSet sysSetup, PD0.CoordinateTransforms xform)
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.FixedLeader)
        {
            Initialize();
            DecodeRtiEnsemble(ens, anc, sysSetup, xform);
        }

        #region Initialize

        /// <summary>
        /// Initialize the values.
        /// </summary>
        public void Initialize()
        {
            CpuFirmwareVersion = 0;
            CpuFirmwareRevision = 0;
            SystemConfiguration = 0;
            RealSimFlag = true;
            LagLength = 0;
            NumberOfBeams = 4;
            NumberOfCells = NUM_DEPTH_CELL_MIN;
            PingsPerEnsemble = PINGS_PER_ENSEMBLE_MIN;
            DepthCellLength = DEPTH_CELL_LENGTH_MIN;
            BlankAfterTransmit = BLANK_MIN;
            ProfilingMode = 1;
            LowCorrThresh = LOW_CORR_THRESH_MIN;
            NumCodeRepeats = NUM_CODE_REPEATS_MIN;
            PercentGoodMinimum = PERCENT_GOOD_MINIMUM_MIN;
            ErrorVelMaximum = ERR_VEL_THRESHOLD_MIN;
            TimeBetweenPingMinutes = 0;
            TimeBetweenPingSeconds = 0;
            TimeBetweenPingHundredths = 0;
            CoordinateTransform = 0;
            HeadingAlignment = 0.0f;
            HeadingBias = 0.0f;
            SensorSource = 0;
            SensorsAvailable = 0;
            Bin1Distance = BIN_1_DISTANCE_MIN;
            XmitPulseLength = XMIT_PULSE_LENGTH_MIN;
            ReferenceLayerAverageStartCell = REF_LAYER_AVG_DEPTH_CELL_MIN;
            ReferenceLayerAverageEndCell = REF_LAYER_AVG_DEPTH_CELL_MIN + 1;
            FalseTargetThresh = FALSE_TARGET_THRESH_MIN;
            Spare_40 = SPARE_MIN;
            TransmitLagDistance = TRANSMIT_LAG_DISTANCE_MIN;
            CpuBoardSerialNumber = "";
            SystemBandwidth = SYSTEM_BANDWIDTH_MIN;
            SystemPower = SYSTEM_POWER_MIN;
            BaseFrequencyIndex = 0;
            InstrumentSerialNumber = "";
            BeamAngle = 0;
        }

        #endregion

        #region Encode

        /// <summary>
        /// Encode the data type to a byte array.
        /// </summary>
        /// <returns>Byte array of the data type.</returns>
        public override byte[] Encode()
        {
            // System Configuration
            byte sysConfigLsb;
            byte sysConfigMsb;
            MathHelper.LsbMsb(SystemConfiguration, out sysConfigLsb, out sysConfigMsb);

            // Pings Per Ensemble
            byte pingPerEnsembleLsb;
            byte pingPerEnsembleMsb;
            MathHelper.LsbMsb(PingsPerEnsemble, out pingPerEnsembleLsb, out pingPerEnsembleMsb);

            // Depth Cell Length
            byte depthCellLengthLsb;
            byte depthCellLengthMsb;
            MathHelper.LsbMsb(DepthCellLength, out depthCellLengthLsb, out depthCellLengthMsb);

            // Blank After Transmit
            byte blankAfterTransmitLsb;
            byte blankAfterTransmitMsb;
            MathHelper.LsbMsb(BlankAfterTransmit, out blankAfterTransmitLsb, out blankAfterTransmitMsb);

            // Error Velocity Maximum
            byte errorVelocityMaxLsb;
            byte errorVelocityMaxMsb;
            MathHelper.LsbMsb(ErrorVelMaximum, out errorVelocityMaxLsb, out errorVelocityMaxMsb);

            // Heading Alignment
            byte headingAlignmentLsb;
            byte headingAlignmentMsb;
            short headingAlignmentVal = (short)Math.Round(HeadingAlignment / 0.01);                                     // Value stored with LSD = 0.01
            MathHelper.LsbMsbShort(headingAlignmentVal, out headingAlignmentLsb, out headingAlignmentMsb);

            // Heading Bias
            byte headingBiasLsb;
            byte headingBiasMsb;
            short headingBiasVal = (short)Math.Round(HeadingBias / 0.01);                                               // Value stored with LSD = 0.01
            MathHelper.LsbMsbShort(headingBiasVal, out headingBiasLsb, out headingBiasMsb);

            // Bin 1 Distance
            byte bin1DistanceLsb;
            byte bin1DistanceMsb;
            MathHelper.LsbMsb(Bin1Distance, out bin1DistanceLsb, out bin1DistanceMsb);

            // Xmit Pulse Length
            byte xmitPulseLengthLsb;
            byte xmitPulseLengthMsb;
            MathHelper.LsbMsb(XmitPulseLength, out xmitPulseLengthLsb, out xmitPulseLengthMsb);

            // Transmit Lag Distance
            byte transmitLagDistanceLsb;
            byte transmitLagDistanceMsb;
            MathHelper.LsbMsb(TransmitLagDistance, out transmitLagDistanceLsb, out transmitLagDistanceMsb);

            // CPU Board serial number
            byte[] cpuBrdSerial = new byte[8];
            cpuBrdSerial = Encoding.ASCII.GetBytes(CpuBoardSerialNumber);
            if (cpuBrdSerial.Length < 8)
            {
                cpuBrdSerial = new byte[8];
            }

            // System Bandwidth
            byte systemBandwidthLsb;
            byte systemBandwidthMsb;
            MathHelper.LsbMsb(SystemBandwidth, out systemBandwidthLsb, out systemBandwidthMsb);

            // CPU Board serial number
            byte[] instrumentSerial = new byte[4];
            instrumentSerial = Encoding.ASCII.GetBytes(InstrumentSerialNumber);
            if (instrumentSerial.Length < 4)
            {
                instrumentSerial = new byte[4];
            }

            // Create the array
            byte[] data = new byte[DATATYPE_SIZE];

            data[0] = ID_LSB;                                   // Fixed Leader ID LSB 0x00
            data[1] = ID_MSB;                                   // Fixed Leader ID MSB 0x00
            data[2] = CpuFirmwareVersion;                       // CPU Firmware Version
            data[3] = CpuFirmwareRevision;                      // CPU Firmware Revision
            data[4] = sysConfigLsb;                             // System Configuration LSB
            data[5] = sysConfigMsb;                             // System Configuration MSB
            data[6] = Convert.ToByte(RealSimFlag);              // Real Sim Flag
            data[7] = LagLength;                                // Lag Length
            data[8] = NumberOfBeams;                            // Number of Beams
            data[9] = NumberOfCells;                            // Number of Cells
            data[10] = pingPerEnsembleLsb;                       // Pings Per Ensemble LSB
            data[11] = pingPerEnsembleMsb;                       // Pings Per Ensemble MSB
            data[12] = depthCellLengthLsb;                       // Depth Cell Length LSB
            data[13] = depthCellLengthMsb;                       // Depth Cell Length MSB
            data[14] = blankAfterTransmitLsb;                    // Blank After Transmit LSB
            data[15] = blankAfterTransmitMsb;                    // Blank After Transmit MSB
            data[16] = ProfilingMode;                            // Profile Mode
            data[17] = LowCorrThresh;                            // Low Correlation Threshold
            data[18] = NumCodeRepeats;                           // Number of Code Repeats
            data[19] = PercentGoodMinimum;                       // Percent Good Minimum
            data[20] = errorVelocityMaxLsb;                      // Error Velocity Maximum LSB
            data[21] = errorVelocityMaxMsb;                      // Error Velocity Maximum MSB
            data[22] = TimeBetweenPingMinutes;                   // Time Between Ping Minutes
            data[23] = TimeBetweenPingSeconds;                   // Time Between Ping Seconds
            data[24] = TimeBetweenPingHundredths;                // Time Between Ping Hundredth of Second
            data[25] = CoordinateTransform;                      // Coordinate Transform
            data[26] = headingAlignmentLsb;                      // Heading Alginment LSB
            data[27] = headingAlignmentMsb;                      // Heading Alignment MSB
            data[28] = headingBiasLsb;                           // Heading Bias LSB
            data[29] = headingBiasMsb;                           // Heading Bias MSB
            data[30] = SensorSource;                             // Sensor Source
            data[31] = SensorsAvailable;                         // Sensor Available
            data[32] = bin1DistanceLsb;                          // Bin 1 Distance LSB
            data[33] = bin1DistanceMsb;                          // Bin 1 Distance MSB
            data[34] = xmitPulseLengthLsb;                       // Xmit Pulse Length LSB
            data[35] = xmitPulseLengthMsb;                       // Xmit Pulse Length MSB
            data[36] = ReferenceLayerAverageStartCell;           // Reference Layer Average Start Cell LSB
            data[37] = ReferenceLayerAverageEndCell;             // Reference Layer Average End Cell MSB
            data[38] = FalseTargetThresh;                        // False Target Threshold
            data[39] = Spare_40;                                 // Spare
            data[40] = transmitLagDistanceLsb;                   // Transmit Lag Distance LSB
            data[41] = transmitLagDistanceMsb;                   // Transmit Lag Distance MSB
            data[42] = cpuBrdSerial[0];                          // CPU Board Serial Number LSB
            data[43] = cpuBrdSerial[1];                          // CPU Board Serial Number
            data[44] = cpuBrdSerial[2];                          // CPU Board Serial Number
            data[45] = cpuBrdSerial[3];                          // CPU Board Serial Number
            data[46] = cpuBrdSerial[4];                          // CPU Board Serial Number
            data[47] = cpuBrdSerial[5];                          // CPU Board Serial Number
            data[48] = cpuBrdSerial[6];                          // CPU Board Serial Number
            data[49] = cpuBrdSerial[7];                          // CPU Board Serial Number MSB
            data[50] = systemBandwidthLsb;                       // System Bandwidth LSB
            data[51] = systemBandwidthMsb;                       // System Bandwidth MSB
            data[52] = SystemPower;                              // System Power
            data[53] = BaseFrequencyIndex;                       // Base Frequency Index
            data[54] = instrumentSerial[0];                      // Insturment Serial Number LSB
            data[55] = instrumentSerial[1];                      // Insturment Serial Number
            data[56] = instrumentSerial[2];                      // Insturment Serial Number
            data[57] = instrumentSerial[3];                      // Insturment Serial Number MSB
            data[58] = BeamAngle;                                // Beam Angle

            // Convert to a byte array
            //return (byte[])data.ToArray(typeof(byte));
            return data;
        }

        #endregion

        #region Decode
        
        /// <summary>
        /// Decode the binary data to the Fixed Leader.
        /// </summary>
        /// <param name="data">PD0 Binary Fixed Leader data type.</param>
        public void Decode(byte[] data)
        {
            CpuFirmwareVersion = data[2];                                                   // CPU Firmware Version
            CpuFirmwareRevision = data[3];                                                  // CPU Firmware Revision
            SystemConfiguration = MathHelper.LsbMsbShort(data[4], data[5]);                 // System Configuration
            RealSimFlag = Convert.ToBoolean(data[6]);                                       // Real Sim Flag
            LagLength = data[7];                                                            // Lag Length
            NumberOfBeams = data[8];                                                        // Number of Beams
            NumberOfCells = data[9];                                                        // Number of Cells
            PingsPerEnsemble = MathHelper.LsbMsbUShort(data[10], data[11]);                 // Pings Per Ensemble
            DepthCellLength = MathHelper.LsbMsbUShort(data[12], data[13]);                  // Depth Cell Length
            BlankAfterTransmit = MathHelper.LsbMsbUShort(data[14], data[15]);               // Blank After Transmit
            ProfilingMode = data[16];                                                       // Profiling Mode
            LowCorrThresh = data[17];                                                       // Low Correlation Threshold
            NumCodeRepeats = data[18];                                                      // Number of Code Repeats
            PercentGoodMinimum = data[19];                                                  // Percent Good Minimum
            ErrorVelMaximum = MathHelper.LsbMsbUShort(data[20], data[21]);                  // Error Velocity Maximum
            TimeBetweenPingMinutes = data[22];                                              // Time Between Ping Minutes
            TimeBetweenPingSeconds = data[23];                                              // Time Between Ping Seconds
            TimeBetweenPingHundredths = data[24];                                           // Time Between Ping Hundredth of Second
            CoordinateTransform = data[25];                                                 // Coordinate Transform
            HeadingAlignment = MathHelper.LsbMsbShort(data[26], data[27]) * 0.01f;          // Heading Alignment    LSD = 0.01
            HeadingBias = MathHelper.LsbMsbShort(data[28], data[29]) * 0.01f;               // Heading Bias         LSB = 0.01
            SensorSource = data[30];                                                        // Sensor Source
            SensorsAvailable = data[31];                                                    // Sensor Available
            Bin1Distance = MathHelper.LsbMsbUShort(data[32], data[33]);                     // Bin 1 Distance
            XmitPulseLength = MathHelper.LsbMsbUShort(data[34], data[35]);                  // Xmit Pulse Length
            ReferenceLayerAverageStartCell = data[36];                                      // Reference Layer Average Start Cell LSB
            ReferenceLayerAverageEndCell = data[37];                                        // Reference Layer Average End Cell MSB
            FalseTargetThresh = data[38];                                                   // False Target Threshold
            Spare_40 = data[39];                                                            // Spare 
            TransmitLagDistance = MathHelper.LsbMsbUShort(data[40], data[41]);              // Transmit Lag Distance
            //CpuBoardSerialNumber = System.Text.Encoding.Default.GetString(data, 42, 8);     // Cpu Board Serial Number, 8 bytes long
            CpuBoardSerialNumber = "010000000000000000000000" + 
                                   Convert.ToString(data[42]) +
                                   Convert.ToString(data[43]) +
                                   Convert.ToString(data[44]) +
                                   Convert.ToString(data[45]) +
                                   Convert.ToString(data[46]) +
                                   Convert.ToString(data[47]) +
                                   Convert.ToString(data[48]) +
                                   Convert.ToString(data[49]);

            SystemBandwidth = MathHelper.LsbMsbUShort(data[50], data[51]);                  // System Bandwidth
            SystemPower = data[52];                                                         // System Power
            BaseFrequencyIndex = data[53];                                                  // Base Frequency
            InstrumentSerialNumber = System.Text.Encoding.Default.GetString(data, 54, 4);   // Instrument Serial Number, 4 bytes long
            BeamAngle = data[58];                                                           // Beam Angle
        }

        #endregion

        #region System Configuration

        #region System Frequency

        /// <summary>
        /// Get the System Frequency.
        /// </summary>
        /// <returns>System Frequency.</returns>
        public SystemFrequency GetSystemFrequency()
        {
            SystemFrequency sysFreq = SystemFrequency.Freq_300kHz;

            bool bit0 = MathHelper.IsBitSet(SystemConfiguration, 0);
            bool bit1 = MathHelper.IsBitSet(SystemConfiguration, 1);
            bool bit2 = MathHelper.IsBitSet(SystemConfiguration, 2);

            // Value   0 0 0
            // Index   2 1 0
            if (!bit2 && !bit1 && !bit0)
            {
                return SystemFrequency.Freq_75kHz;
            }

            // Value   0 0 1
            // Index   2 1 0
            if (!bit2 && !bit1 && bit0)
            {
                return SystemFrequency.Freq_150kHz;
            }

            // Value   0 1 0
            // Index   2 1 0
            if (!bit2 && bit1 && !bit0)
            {
                return SystemFrequency.Freq_300kHz;
            }

            // Value   0 1 1
            // Index   2 1 0
            if (!bit2 && bit1 && bit0)
            {
                return SystemFrequency.Freq_600kHz;
            }

            // Value   1 0 0
            // Index   2 1 0
            if (bit2 && !bit1 && !bit0)
            {
                return SystemFrequency.Freq_1200kHz;
            }

            // Value   1 0 1
            // Index   2 1 0
            if (bit2 && !bit1 && bit0)
            {
                return SystemFrequency.Freq_2400kHz;
            }

            return sysFreq;
        }

        /// <summary>
        /// Set the system frequency based off the value given.
        /// </summary>
        /// <param name="sysFreq">System frequency to set.</param>
        public void SetSystemFrequency(SystemFrequency sysFreq)
        {
            switch(sysFreq)
            {
                case SystemFrequency.Freq_75kHz:
                    // 000
                    SystemConfiguration = MathHelper.ZeroBitShort(SystemConfiguration, 0);
                    SystemConfiguration = MathHelper.ZeroBitShort(SystemConfiguration, 1);
                    SystemConfiguration = MathHelper.ZeroBitShort(SystemConfiguration, 2);
                    break;
                case SystemFrequency.Freq_150kHz:
                    // 001
                    SystemConfiguration = MathHelper.SetBitShort(SystemConfiguration, 0);
                    SystemConfiguration = MathHelper.ZeroBitShort(SystemConfiguration, 1);
                    SystemConfiguration = MathHelper.ZeroBitShort(SystemConfiguration, 2);
                    break;
                case SystemFrequency.Freq_300kHz:
                    // 010
                    SystemConfiguration = MathHelper.ZeroBitShort(SystemConfiguration, 0);
                    SystemConfiguration = MathHelper.SetBitShort(SystemConfiguration, 1);
                    SystemConfiguration = MathHelper.ZeroBitShort(SystemConfiguration, 2);
                    break;
                case SystemFrequency.Freq_600kHz:
                    // 011
                    SystemConfiguration = MathHelper.SetBitShort(SystemConfiguration, 0);
                    SystemConfiguration = MathHelper.SetBitShort(SystemConfiguration, 1);
                    SystemConfiguration = MathHelper.ZeroBitShort(SystemConfiguration, 2);
                    break;
                case SystemFrequency.Freq_1200kHz:
                    // 100
                    SystemConfiguration = MathHelper.ZeroBitShort(SystemConfiguration, 0);
                    SystemConfiguration = MathHelper.ZeroBitShort(SystemConfiguration, 1);
                    SystemConfiguration = MathHelper.SetBitShort(SystemConfiguration, 2);
                    break;
                case SystemFrequency.Freq_2400kHz:
                    // 101
                    SystemConfiguration = MathHelper.SetBitShort(SystemConfiguration, 0);
                    SystemConfiguration = MathHelper.ZeroBitShort(SystemConfiguration, 1);
                    SystemConfiguration = MathHelper.SetBitShort(SystemConfiguration, 2);
                    break;
            }
        }

        #endregion

        #region Concave/Convex

        /// <summary>
        /// Set the configuration to Concave.
        /// Bit Position 3 is the Concave/Convex value.
        /// </summary>
        public void SetConcave()
        {
            SystemConfiguration = MathHelper.ZeroBitShort(SystemConfiguration, 3);
        }

        /// <summary>
        /// Set the configuration to Convex.
        /// Bit Position 3 is the Concave/Convex value.
        /// </summary>
        public void SetConvex()
        {
            SystemConfiguration = MathHelper.SetBitShort(SystemConfiguration, 3);
        }

        /// <summary>
        /// Check if the configuartion is Convex.
        /// Bit Position 3 is the Concave/Convex value.
        /// </summary>
        /// <returns>TRUE = Convex / False = Concave.</returns>
        public bool IsConvex()
        {
            return MathHelper.IsBitSet(SystemConfiguration, 3);
        }

        /// <summary>
        /// Check if the configuration is Concave.
        /// </summary>
        /// <returns>TRUE = Concave / False = Convex.</returns>
        public bool IsConcave()
        {
            return !IsConvex();
        }

        #endregion

        #region Sensor Config

        /// <summary>
        /// Check if the Sensor Configuration 1 is set.
        /// </summary>
        /// <returns>TRUE = Sensor Config 1 is set.</returns>
        public bool IsSensorConfig1()
        {
            bool bit4 = MathHelper.IsBitSet(SystemConfiguration, 4);
            bool bit5 = MathHelper.IsBitSet(SystemConfiguration, 5);


            // Value   0 0
            // Index   5 4
            if(!bit4 && !bit5)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if the Sensor Configuration 2 is set.
        /// </summary>
        /// <returns>TRUE = Sensor Config 2 is set.</returns>
        public bool IsSensorConfig2()
        {
            bool bit4 = MathHelper.IsBitSet(SystemConfiguration, 4);
            bool bit5 = MathHelper.IsBitSet(SystemConfiguration, 5);

            // Value   0 1
            // Index   5 4
            if (bit4 && !bit5)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if the Sensor Configuration 3 is set.
        /// </summary>
        /// <returns>TRUE = Sensor Config 3 is set.</returns>
        public bool IsSensorConfig3()
        {
            bool bit4 = MathHelper.IsBitSet(SystemConfiguration, 4);
            bool bit5 = MathHelper.IsBitSet(SystemConfiguration, 5);

            // Value   1 0
            // Index   5 4
            if (!bit4 && bit5)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Set the Bits for Sensor Config 1.
        /// 00
        /// </summary>
        public void SetSensorConfig1()
        {
            //BitArray ba = new BitArray(SystemConfiguration);

            //// 00
            //ba.Set(4, false);
            //ba.Set(5, false);

            //// Copy the value back to the bit array
            //int[] array = new int[1];
            //ba.CopyTo(array, 0);
            //array[0] = SystemConfiguration;

            // Value   0 0
            // Index   5 4
            SystemConfiguration = MathHelper.ZeroBitShort(SystemConfiguration, 4);
            SystemConfiguration = MathHelper.ZeroBitShort(SystemConfiguration, 5);
        }

        /// <summary>
        /// Set the bits for Sensor Config 2.
        /// 01
        /// </summary>
        public void SetSensorConfig2()
        {
            // Value   0 1
            // Index   5 4
            SystemConfiguration = MathHelper.SetBitShort(SystemConfiguration, 4);
            SystemConfiguration = MathHelper.ZeroBitShort(SystemConfiguration, 5);
        }

        /// <summary>
        /// Set the bits for the Sensor Config 3.
        /// 10
        /// </summary>
        public void SetSensorConfig3()
        {
            // Value   1 0
            // Index   5 4
            SystemConfiguration = MathHelper.ZeroBitShort(SystemConfiguration, 4);
            SystemConfiguration = MathHelper.SetBitShort(SystemConfiguration, 5);
        }

        #endregion

        #region Attached XDCR Head

        /// <summary>
        /// Set the configuration to Head Attached.
        /// Bit Position 6 is the Head Attached value.
        /// </summary>
        public void SetHeadAttached()
        {
            SystemConfiguration = MathHelper.SetBitShort(SystemConfiguration, 6);
        }

        /// <summary>
        /// Set the configuration to Head Not Attached.
        /// Bit Position 6 is the Head Attached value.
        /// </summary>
        public void SetHeadNotAttached()
        {
            SystemConfiguration = MathHelper.ZeroBitShort(SystemConfiguration, 6);
        }

        /// <summary>
        /// Check if the configuartion is Head Attached.
        /// Bit Position 6 is the Head Attached value.
        /// </summary>
        /// <returns>TRUE = Head Attached / False = Head Not Attached.</returns>
        public bool IsHeadAttached()
        {
            return MathHelper.IsBitSet(SystemConfiguration, 6);
        }

        /// <summary>
        /// Check if the configuration is Head Not Attached.
        /// </summary>
        /// <returns>TRUE = Head Not Attached / False = Head Attached.</returns>
        public bool IsHeadNotAttached()
        {
            return !IsHeadAttached();
        }

        #endregion

        #region Beam Facing

        /// <summary>
        /// Set the configuration to Beams Upwards.
        /// Bit Position 7 is the Beams Facing value.
        /// </summary>
        public void SetBeamsUpward()
        {
            SystemConfiguration = MathHelper.SetBitShort(SystemConfiguration, 7);
        }

        /// <summary>
        /// Set the configuration to Beams Downwards.
        /// Bit Position 7 is the Beams Facing value.
        /// </summary>
        public void SetBeamsDownward()
        {
            SystemConfiguration = MathHelper.ZeroBitShort(SystemConfiguration, 7);
        }

        /// <summary>
        /// Check if the configuartion is Beams Upwards.
        /// Bit Position 7 is the Beams Facing value.
        /// </summary>
        /// <returns>TRUE = Beams Upwards / False = Beams Downwards.</returns>
        public bool IsBeamsUpwards()
        {
            return MathHelper.IsBitSet(SystemConfiguration, 7);
        }

        /// <summary>
        /// Check if the configuration is Beams Downwards.
        /// </summary>
        /// <returns>TRUE = Beams Downwards / False = Beams Upwards.</returns>
        public bool IsBeamsDownwards()
        {
            return !IsBeamsUpwards();
        }

        #endregion

        #region Beam Angle

        /// <summary>
        /// Check if is 15 Degree Beam Angle.
        /// </summary>
        /// <returns>TRUE = 15 Degree Beam Angle is set.</returns>
        public bool Is15DegreeBeamAngle()
        {
            bool bit8 = MathHelper.IsBitSet(SystemConfiguration, 8);
            bool bit9 = MathHelper.IsBitSet(SystemConfiguration, 9);

            // Value   0 0
            // Index   9 8
            if (!bit9 && !bit8)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if is 20 Degree Beam Angle.
        /// </summary>
        /// <returns>TRUE = 20 Degree Beam Angle is set.</returns>
        public bool Is20DegreeBeamAngle()
        {
            bool bit8 = MathHelper.IsBitSet(SystemConfiguration, 8);
            bool bit9 = MathHelper.IsBitSet(SystemConfiguration, 9);

            // Value   0 1
            // Index   9 8
            if (!bit9 && bit8)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if is 30 Degree Beam Angle.
        /// </summary>
        /// <returns>TRUE = 30 Degree Beam Angle is set.</returns>
        public bool Is30DegreeBeamAngle()
        {
            bool bit8 = MathHelper.IsBitSet(SystemConfiguration, 8);
            bool bit9 = MathHelper.IsBitSet(SystemConfiguration, 9);

            // Value   1 0
            // Index   9 8
            if (bit9 && !bit8)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if is Other Degree Beam Angle.
        /// </summary>
        /// <returns>TRUE = Other Degree Beam Angle is set.</returns>
        public bool IsOtherDegreeBeamAngle()
        {
            bool bit8 = MathHelper.IsBitSet(SystemConfiguration, 8);
            bool bit9 = MathHelper.IsBitSet(SystemConfiguration, 9);

            // Value   1 1
            // Index   9 8
            if (bit9 && bit8)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Set the Bits for 15 Beam Angle.
        /// 00
        /// </summary>
        public void Set15DegreeBeamAngle()
        {
            // Value   0 0
            // Index   9 8
            SystemConfiguration = MathHelper.ZeroBitShort(SystemConfiguration, 8);
            SystemConfiguration = MathHelper.ZeroBitShort(SystemConfiguration, 9);
        }

        /// <summary>
        /// Set the bits for 20 Beam Angle.
        /// 01
        /// </summary>
        public void Set20DegreeBeamAngle()
        {
            // Value   0 1
            // Index   9 8
            SystemConfiguration = MathHelper.SetBitShort(SystemConfiguration, 8);
            SystemConfiguration = MathHelper.ZeroBitShort(SystemConfiguration, 9);
        }

        /// <summary>
        /// Set the bits for 30 Beam Angle.
        /// 10
        /// </summary>
        public void Set30DegreeBeamAngle()
        {
            // Value   1 0
            // Index   9 8
            SystemConfiguration = MathHelper.ZeroBitShort(SystemConfiguration, 8);
            SystemConfiguration = MathHelper.SetBitShort(SystemConfiguration, 9);
        }

        /// <summary>
        /// Set the bits for Other Beam Angle.
        /// 11
        /// </summary>
        public void SetOtherDegreeBeamAngle()
        {
            // Value   1 1
            // Index   9 8
            SystemConfiguration = MathHelper.SetBitShort(SystemConfiguration, 8);
            SystemConfiguration = MathHelper.SetBitShort(SystemConfiguration, 9);
        }

        #endregion

        #region Beam Configuration

        /// <summary>
        /// Get the Beam Configuration.
        /// </summary>
        /// <returns>Beam Configuration.</returns>
        public BeamConfigs GetBeamConfiguration()
        {
            BeamConfigs beamConfig = BeamConfigs.BeamConfig_4_Beam_Janus;

            bool bit12 = MathHelper.IsBitSet(SystemConfiguration, 12);
            bool bit13 = MathHelper.IsBitSet(SystemConfiguration, 13);
            bool bit14 = MathHelper.IsBitSet(SystemConfiguration, 14);
            bool bit15 = MathHelper.IsBitSet(SystemConfiguration, 15);

            // Value    0   1   0   0
            // Index   15  14  13  12
            if (!bit15 && bit14 && !bit13 && !bit12)
            {
                return BeamConfigs.BeamConfig_4_Beam_Janus;
            }

            // Value    0   1   0   1
            // Index   15  14  13  12
            if (!bit15 && bit14 && !bit13 && bit12)
            {
                return BeamConfigs.BeamConfig_5_Beam_Janus_Demod;
            }

            // Value    1   1   1   1
            // Index   15  14  13  12
            if (bit15 && bit14 && bit13 && bit12)
            {
                return BeamConfigs.BeamConfig_5_Beam_Janus_2_Demod;
            }

            return beamConfig;
        }

        /// <summary>
        /// Set the Beam Configuration based off the value given.
        /// </summary>
        /// <param name="beamConfig">Beam Configuration to set.</param>
        public void SetBeamConfiguration(BeamConfigs beamConfig)
        {
            switch (beamConfig)
            {
                case BeamConfigs.BeamConfig_4_Beam_Janus:
                    // 0100
                    SystemConfiguration = MathHelper.ZeroBitShort(SystemConfiguration, 12);
                    SystemConfiguration = MathHelper.ZeroBitShort(SystemConfiguration, 13);
                    SystemConfiguration = MathHelper.SetBitShort(SystemConfiguration, 14);
                    SystemConfiguration = MathHelper.ZeroBitShort(SystemConfiguration, 15);
                    break;
                case BeamConfigs.BeamConfig_5_Beam_Janus_Demod:
                    // 0101
                    SystemConfiguration = MathHelper.SetBitShort(SystemConfiguration, 12);
                    SystemConfiguration = MathHelper.ZeroBitShort(SystemConfiguration, 13);
                    SystemConfiguration = MathHelper.SetBitShort(SystemConfiguration, 14);
                    SystemConfiguration = MathHelper.ZeroBitShort(SystemConfiguration, 15);
                    break;
                case BeamConfigs.BeamConfig_5_Beam_Janus_2_Demod:
                    // 1111
                    SystemConfiguration = MathHelper.SetBitShort(SystemConfiguration, 12);
                    SystemConfiguration = MathHelper.SetBitShort(SystemConfiguration, 13);
                    SystemConfiguration = MathHelper.SetBitShort(SystemConfiguration, 14);
                    SystemConfiguration = MathHelper.SetBitShort(SystemConfiguration, 15);
                    break;
            }
        }

        #endregion

        #endregion

        #region Coordinate Transform

        #region Transform

        /// <summary>
        /// Get the Coordinate Transform.
        /// </summary>
        /// <returns>Coordinate Transform.</returns>
        public PD0.CoordinateTransforms GetCoordinateTransform()
        {
            bool bit3 = MathHelper.IsBitSet(CoordinateTransform, 3);
            bool bit4 = MathHelper.IsBitSet(CoordinateTransform, 4);

            // Value   0  0
            // Index   4  3
            if (!bit4 && !bit3)
            {
                return PD0.CoordinateTransforms.Coord_Beam;
            }

            // Value   0  1
            // Index   4  3
            if (!bit4 && bit3)
            {
                return PD0.CoordinateTransforms.Coord_Instrument;
            }

            // Value   1  0
            // Index   4  3
            if (bit4 && !bit3)
            {
                return PD0.CoordinateTransforms.Coord_Ship;
            }

            // Value   1  1
            // Index   4  3
            if (bit4 && bit3)
            {
                return PD0.CoordinateTransforms.Coord_Earth;
            }

            return PD0.CoordinateTransforms.Coord_Beam;
        }

        /// <summary>
        /// Set the Coordinate Transform based off the value given.
        /// </summary>
        /// <param name="xform">Coordinate Transform to set.</param>
        public void SetCoordinateTransform(PD0.CoordinateTransforms xform)
        {
            switch (xform)
            {
                case PD0.CoordinateTransforms.Coord_Beam:
                    // 00
                    CoordinateTransform = MathHelper.ZeroBitByte(CoordinateTransform, 3);
                    CoordinateTransform = MathHelper.ZeroBitByte(CoordinateTransform, 4);
                    break;
                case PD0.CoordinateTransforms.Coord_Instrument:
                    // 01
                    CoordinateTransform = MathHelper.SetBitByte(CoordinateTransform, 3);
                    CoordinateTransform = MathHelper.ZeroBitByte(CoordinateTransform, 4);
                    break;
                case PD0.CoordinateTransforms.Coord_Ship:
                    // 10
                    CoordinateTransform = MathHelper.ZeroBitByte(CoordinateTransform, 3);
                    CoordinateTransform = MathHelper.SetBitByte(CoordinateTransform, 4);
                    break;
                case PD0.CoordinateTransforms.Coord_Earth:
                    // 11
                    CoordinateTransform = MathHelper.SetBitByte(CoordinateTransform, 3);
                    CoordinateTransform = MathHelper.SetBitByte(CoordinateTransform, 4);
                    break;
            }
        }

        #endregion

        #region Tilts Used in Transform

        /// <summary>
        /// Set the configuration to use Tilts in Transform.
        /// Bit Position 2 is the Tilt value.
        /// </summary>
        public void SetTiltsUsedInTransform()
        {
            CoordinateTransform = MathHelper.SetBitByte(CoordinateTransform, 2);
        }

        /// <summary>
        /// Set the configuration to not use Tilts in Transform.
        /// Bit Position 2 is the Tilt value.
        /// </summary>
        public void SetTiltsNotUsedInTransform()
        {
            CoordinateTransform = MathHelper.ZeroBitByte(CoordinateTransform, 2);
        }

        /// <summary>
        /// Check if the configuartion if Tilts Used in Transform.
        /// Bit Position 2 is the Tilt value.
        /// </summary>
        /// <returns>TRUE = Tilts Used in Transform / False = Tilts Not Used in Transform.</returns>
        public bool IsTiltsUsedInTransform()
        {
            return MathHelper.IsBitSet(CoordinateTransform, 2);
        }

        /// <summary>
        /// Check if the configuration if Tilts is Not Used in Transform.
        /// </summary>
        /// <returns>TRUE = Tilts Not Used in Transform / False = Tilts Used in Transform.</returns>
        public bool IsTiltsNotUsedInTransform()
        {
            return !IsTiltsUsedInTransform();
        }

        #endregion

        #region 3-Beam Solution

        /// <summary>
        /// Set the configuration to use 3-Beam Solution.
        /// Bit Position 1 is the 3-Beam Solution.
        /// </summary>
        public void Set3BeamSolution()
        {
            CoordinateTransform = MathHelper.SetBitByte(CoordinateTransform, 1);
        }

        /// <summary>
        /// Set the configuration to not use 3-Beam Solution.
        /// Bit Position 1 is the 3-Beam Solution.
        /// </summary>
        public void SetNo3BeamSolution()
        {
            CoordinateTransform = MathHelper.ZeroBitByte(CoordinateTransform, 1);
        }

        /// <summary>
        /// Check if the configuartion to use 3-Beam solution.
        /// Bit Position 1 is the 3-Beam Solution.
        /// </summary>
        /// <returns>TRUE = 3-Beam Solution. / False = No 3 Beam Solution.</returns>
        public bool Is3BeamSolution()
        {
            return MathHelper.IsBitSet(CoordinateTransform, 1);
        }

        /// <summary>
        /// Check if the configuartion to not use 3-Beam solution.
        /// </summary>
        /// <returns>TRUE = No 3 Beam Solution. / False = 3-Beam Solution.</returns>
        public bool IsNo3BeamSolution()
        {
            return !Is3BeamSolution();
        }

        #endregion

        #region Binmapping

        /// <summary>
        /// Set the configuration to use Binmapping.
        /// Bit Position 0 is the Binmapping.
        /// </summary>
        public void SetBinmappingUsed()
        {
            CoordinateTransform = MathHelper.SetBitByte(CoordinateTransform, 0);
        }

        /// <summary>
        /// Set the configuration to use Binmapping.
        /// Bit Position 0 is the Binmapping.
        /// </summary>
        public void SetNoBinmappingUsed()
        {
            CoordinateTransform = MathHelper.ZeroBitByte(CoordinateTransform, 0);
        }

        /// <summary>
        /// Check if the configuartion to use Binmapping.
        /// Bit Position 0 is the Binmapping.
        /// </summary>
        /// <returns>TRUE = Binmapping. / False = No Binmapping.</returns>
        public bool IsBinmappingUsed()
        {
            return MathHelper.IsBitSet(CoordinateTransform, 0);
        }

        /// <summary>
        /// Check if the configuartion to not use Binmapping.
        /// </summary>
        /// <returns>TRUE = No Binmapping. / False = Binmapping.</returns>
        public bool IsNoBinmappingUsed()
        {
            return !IsBinmappingUsed();
        }

        #endregion

        #endregion

        #region Sensor Source

        #region Transducer Temperature Sensor

        /// <summary>
        /// Set to use the tranducer's temperature sensor.
        /// Bit Position 0 is the tranducer temperature sensor.
        /// </summary>
        public void SetUseXdcrTempSensor()
        {
            SensorSource = MathHelper.SetBitByte(SensorSource, 0);
        }

        /// <summary>
        /// Set to not use the tranducer's temperature sensor.
        /// Bit Position 0 is the tranducer temperature sensor.
        /// </summary>
        public void SetNoUseXdcrTempSensor()
        {
            SensorSource = MathHelper.ZeroBitByte(SensorSource, 0);
        }

        /// <summary>
        /// Check if using the tranducer's temperature sensor..
        /// Bit Position 0 is the tranducer temperature sensor.
        /// </summary>
        /// <returns>TRUE = Using Tranducer Temperature Sensor. / False = Not Using Tranducer Temperature Sensor.</returns>
        public bool IsUseXdcrTempSensor()
        {
            return MathHelper.IsBitSet(SensorSource, 0);
        }

        /// <summary>
        /// Check if not using the tranducer's temperature sensor..
        /// </summary>
        /// <returns>TRUE = Not Using Tranducer Temperature Sensor. / False = Using Tranducer Temperature Sensor.</returns>
        public bool IsNoUseXdcrTempSensor()
        {
            return !IsUseXdcrTempSensor();
        }

        #endregion

        #region Conductivity Sensor

        /// <summary>
        /// Set to use a Conductivity sensor.
        /// Bit Position 1 is the Conductivity sensor.
        /// </summary>
        public void SetUseConductivitySensor()
        {
            SensorSource = MathHelper.SetBitByte(SensorSource, 1);
        }

        /// <summary>
        /// Set to not use a Conductivity sensor.
        /// Bit Position 1 is the Conductivity sensor.
        /// </summary>
        public void SetNoUseConductivitySensor()
        {
            SensorSource = MathHelper.ZeroBitByte(SensorSource, 1);
        }

        /// <summary>
        /// Check if using the Conductivity sensor.
        /// Bit Position 1 is the Conductivity sensor.
        /// </summary>
        /// <returns>TRUE = Using Conductivity Sensor. / False = Not Using Conductivity Sensor.</returns>
        public bool IsUseConductivitySensor()
        {
            return MathHelper.IsBitSet(SensorSource, 1);
        }

        /// <summary>
        /// Check if not using the Conductivity sensor.
        /// </summary>
        /// <returns>TRUE = Not Using Conductivity Sensor. / False = Using Conductivity Sensor.</returns>
        public bool IsNoUseConductivitySensor()
        {
            return !IsUseConductivitySensor();
        }

        #endregion

        #region Roll Sensor

        /// <summary>
        /// Set to use a XDCR Roll sensor.
        /// Bit Position 2 is the XDCR Roll sensor.
        /// </summary>
        public void SetUseXdcrRollSensor()
        {
            SensorSource = MathHelper.SetBitByte(SensorSource, 2);
        }

        /// <summary>
        /// Set to not use a XDCR Roll sensor.
        /// Bit Position 2 is the XDCR Roll sensor.
        /// </summary>
        public void SetNoUseXdcrRollSensor()
        {
            SensorSource = MathHelper.ZeroBitByte(SensorSource, 2);
        }

        /// <summary>
        /// Check if using the XDCR Roll sensor.
        /// Bit Position 2 is the XDCR Roll sensor.
        /// </summary>
        /// <returns>TRUE = Using XDCR Roll Sensor. / False = Not Using XDCR Roll Sensor.</returns>
        public bool IsUseXdcrRollSensor()
        {
            return MathHelper.IsBitSet(SensorSource, 2);
        }

        /// <summary>
        /// Check if not using the XDCR Roll sensor.
        /// </summary>
        /// <returns>TRUE = Not Using XDCR Roll Sensor. / False = Using XDCR Roll Sensor.</returns>
        public bool IsNoUseXdcrRollSensor()
        {
            return !IsUseXdcrRollSensor();
        }

        #endregion

        #region Pitch Sensor

        /// <summary>
        /// Set to use a XDCR Pitch sensor.
        /// Bit Position 3 is the XDCR Pitch sensor.
        /// </summary>
        public void SetUseXdcrPitchSensor()
        {
            SensorSource = MathHelper.SetBitByte(SensorSource, 3);
        }

        /// <summary>
        /// Set to not use a XDCR Pitch sensor.
        /// Bit Position 3 is the XDCR Pitch sensor.
        /// </summary>
        public void SetNoUseXdcrPitchSensor()
        {
            SensorSource = MathHelper.ZeroBitByte(SensorSource, 3);
        }

        /// <summary>
        /// Check if using the XDCR Pitch sensor.
        /// Bit Position 3 is the XDCR Pitch sensor.
        /// </summary>
        /// <returns>TRUE = Using XDCR Pitch Sensor. / False = Not Using XDCR Pitch Sensor.</returns>
        public bool IsUseXdcrPitchSensor()
        {
            return MathHelper.IsBitSet(SensorSource, 3);
        }

        /// <summary>
        /// Check if not using the XDCR Pitch sensor.
        /// </summary>
        /// <returns>TRUE = Not Using XDCR Pitch Sensor. / False = Using XDCR Pitch Sensor.</returns>
        public bool IsNoUseXdcrPitchSensor()
        {
            return !IsUseXdcrPitchSensor();
        }

        #endregion

        #region Heading Sensor

        /// <summary>
        /// Set to use a XDCR Heading sensor.
        /// Bit Position 4 is the XDCR Heading sensor.
        /// </summary>
        public void SetUseXdcrHeadingSensor()
        {
            SensorSource = MathHelper.SetBitByte(SensorSource, 4);
        }

        /// <summary>
        /// Set to not use a XDCR Heading sensor.
        /// Bit Position 4 is the XDCR Heading sensor.
        /// </summary>
        public void SetNoUseXdcrHeadingSensor()
        {
            SensorSource = MathHelper.ZeroBitByte(SensorSource, 4);
        }

        /// <summary>
        /// Check if using the XDCR Heading sensor.
        /// Bit Position 4 is the XDCR Heading sensor.
        /// </summary>
        /// <returns>TRUE = Using XDCR Heading Sensor. / False = Not Using XDCR Heading Sensor.</returns>
        public bool IsUseXdcrHeadingSensor()
        {
            return MathHelper.IsBitSet(SensorSource, 4);
        }

        /// <summary>
        /// Check if not using the XDCR Heading sensor.
        /// </summary>
        /// <returns>TRUE = Not Using XDCR Heading Sensor. / False = Using XDCR Heading Sensor.</returns>
        public bool IsNoUseXdcrHeadingSensor()
        {
            return !IsUseXdcrHeadingSensor();
        }

        #endregion

        #region Depth Sensor

        /// <summary>
        /// Set to use a XDCR Depth sensor.
        /// Bit Position 5 is the XDCR Depth sensor.
        /// </summary>
        public void SetUseXdcrDepthSensor()
        {
            SensorSource = MathHelper.SetBitByte(SensorSource, 5);
        }

        /// <summary>
        /// Set to not use a XDCR Depth sensor.
        /// Bit Position 5 is the XDCR Depth sensor.
        /// </summary>
        public void SetNoUseXdcrDepthSensor()
        {
            SensorSource = MathHelper.ZeroBitByte(SensorSource, 5);
        }

        /// <summary>
        /// Check if using the XDCR Depth sensor.
        /// Bit Position 5 is the XDCR Depth sensor.
        /// </summary>
        /// <returns>TRUE = Using XDCR Depth Sensor. / False = Not Using XDCR Depth Sensor.</returns>
        public bool IsUseXdcrDepthSensor()
        {
            return MathHelper.IsBitSet(SensorSource, 5);
        }

        /// <summary>
        /// Check if not using the XDCR Depth sensor.
        /// </summary>
        /// <returns>TRUE = Not Using XDCR Depth Sensor. / False = Using XDCR Depth Sensor.</returns>
        public bool IsNoUseXdcrDepthSensor()
        {
            return !IsUseXdcrDepthSensor();
        }

        #endregion

        #region Speed of Sound Sensor

        /// <summary>
        /// Set to use a Speed of Sound sensor.
        /// Bit Position 6 is the Speed of Sound sensor.
        /// </summary>
        public void SetUseSpeedOfSoundSensor()
        {
            SensorSource = MathHelper.SetBitByte(SensorSource, 6);
        }

        /// <summary>
        /// Set to not use a Speed of Sound sensor.
        /// Bit Position 6 is the Speed of Sound sensor.
        /// </summary>
        public void SetNoUseSpeedOfSoundSensor()
        {
            SensorSource = MathHelper.ZeroBitByte(SensorSource, 6);
        }

        /// <summary>
        /// Check if using the Speed of Sound sensor.
        /// Bit Position 6 is the Speed of Sound sensor.
        /// </summary>
        /// <returns>TRUE = Using Speed of Sound Sensor. / False = Not Using Speed of Sound Sensor.</returns>
        public bool IsUseSpeedOfSoundSensor()
        {
            return MathHelper.IsBitSet(SensorSource, 6);
        }

        /// <summary>
        /// Check if not using the Speed of Sound sensor.
        /// </summary>
        /// <returns>TRUE = Not Using Speed of Sound Sensor. / False = Using Speed of Sound Sensor.</returns>
        public bool IsNoUseSpeedOfSoundSensor()
        {
            return !IsUseSpeedOfSoundSensor();
        }

        #endregion

        #endregion

        #region Sensor Available

        #region Transducer Temperature Sensor

        /// <summary>
        /// Set if the tranducer's temperature sensor available.
        /// Bit Position 0 is the tranducer temperature sensor.
        /// </summary>
        public void SetXdcrTempSensorAvailable()
        {
            SensorsAvailable = MathHelper.SetBitByte(SensorsAvailable, 0);
        }

        /// <summary>
        /// Set if the tranducer's temperature sensor is not available.
        /// Bit Position 0 is the tranducer temperature sensor.
        /// </summary>
        public void SetNoXdcrTempSensorAvailable()
        {
            SensorsAvailable = MathHelper.ZeroBitByte(SensorsAvailable, 0);
        }

        /// <summary>
        /// Check if the tranducer's temperature sensor available.
        /// Bit Position 0 is the tranducer temperature sensor.
        /// </summary>
        /// <returns>TRUE = Tranducer Temperature Sensor Available. / False = Tranducer Temperature Sensor Not Available.</returns>
        public bool IsXdcrTempSensorAvailable()
        {
            return MathHelper.IsBitSet(SensorsAvailable, 0);
        }

        /// <summary>
        /// Check if tranducer's temperature sensor is not available.
        /// </summary>
        /// <returns>TRUE = Tranducer Temperature Sensor Not Available. / False = Tranducer Temperature Sensor Available.</returns>
        public bool IsNoXdcrTempSensorAvailable()
        {
            return !IsXdcrTempSensorAvailable();
        }

        #endregion

        #region Conductivity Sensor

        /// <summary>
        /// Set that Conductivity sensor is available.
        /// Bit Position 1 is the Conductivity sensor.
        /// </summary>
        public void SetConductivitySensorAvailable()
        {
            SensorsAvailable = MathHelper.SetBitByte(SensorsAvailable, 1);
        }

        /// <summary>
        /// Set that Conductivity sensor is not available.
        /// Bit Position 1 is the Conductivity sensor.
        /// </summary>
        public void SetNoConductivitySensorAvailable()
        {
            SensorsAvailable = MathHelper.ZeroBitByte(SensorsAvailable, 1);
        }

        /// <summary>
        /// Check if the Conductivity sensor is available.
        /// Bit Position 1 is the Conductivity sensor.
        /// </summary>
        /// <returns>TRUE = Conductivity Sensor Available. / False = Conductivity Sensor is not Available.</returns>
        public bool IsConductivitySensorAvailable()
        {
            return MathHelper.IsBitSet(SensorsAvailable, 1);
        }

        /// <summary>
        /// Check if the Conductivity sensor is not available.
        /// </summary>
        /// <returns>TRUE = Conductivity Sensor is not Available. / False = Conductivity Sensor Available.</returns>
        public bool IsNoConductivitySensorAvailable()
        {
            return !IsConductivitySensorAvailable();
        }

        #endregion

        #region Roll Sensor

        /// <summary>
        /// Set that XDCR Roll sensor available.
        /// Bit Position 2 is the XDCR Roll sensor.
        /// </summary>
        public void SetXdcrRollSensorAvailable()
        {
            SensorsAvailable = MathHelper.SetBitByte(SensorsAvailable, 2);
        }

        /// <summary>
        /// Set that XDCR Roll sensor is not available.
        /// Bit Position 2 is the XDCR Roll sensor.
        /// </summary>
        public void SetNoXdcrRollSensorAvailable()
        {
            SensorsAvailable = MathHelper.ZeroBitByte(SensorsAvailable, 2);
        }

        /// <summary>
        /// Check that XDCR Roll sensor is available.
        /// Bit Position 2 is the XDCR Roll sensor.
        /// </summary>
        /// <returns>TRUE = XDCR Roll Sensor Available. / False = XDCR Roll Sensor is Not Available.</returns>
        public bool IsXdcrRollSensorAvailable()
        {
            return MathHelper.IsBitSet(SensorsAvailable, 2);
        }

        /// <summary>
        /// Check that XDCR Roll sensor is not available.
        /// </summary>
        /// <returns>TRUE = XDCR Roll Sensor is Not Available. / False = XDCR Roll Sensor Available.</returns>
        public bool IsNoXdcrRollSensorAvailable()
        {
            return !IsXdcrRollSensorAvailable();
        }

        #endregion

        #region Pitch Sensor

        /// <summary>
        /// Set that XDCR Pitch sensor is available.
        /// Bit Position 3 is the XDCR Pitch sensor.
        /// </summary>
        public void SetXdcrPitchSensorAvailable()
        {
            SensorsAvailable = MathHelper.SetBitByte(SensorsAvailable, 3);
        }

        /// <summary>
        /// Set that XDCR Pitch sensor is not available.
        /// Bit Position 3 is the XDCR Pitch sensor.
        /// </summary>
        public void SetNoXdcrPitchSensorAvailable()
        {
            SensorsAvailable = MathHelper.ZeroBitByte(SensorsAvailable, 3);
        }

        /// <summary>
        /// Check if the XDCR Pitch sensor is available.
        /// Bit Position 3 is the XDCR Pitch sensor.
        /// </summary>
        /// <returns>TRUE = XDCR Pitch Sensor is available. / False = XDCR Pitch Sensor is not available.</returns>
        public bool IsXdcrPitchSensorAvailable()
        {
            return MathHelper.IsBitSet(SensorsAvailable, 3);
        }

        /// <summary>
        /// Check if XDCR Pitch sensor is not available.
        /// </summary>
        /// <returns>TRUE = XDCR Pitch Sensor is not available. / False = XDCR Pitch Sensor is available.</returns>
        public bool IsNoXdcrPitchSensorAvailable()
        {
            return !IsXdcrPitchSensorAvailable();
        }

        #endregion

        #region Heading Sensor

        /// <summary>
        /// Set that XDCR Heading sensor is available.
        /// Bit Position 4 is the XDCR Heading sensor.
        /// </summary>
        public void SetXdcrHeadingSensorAvailable()
        {
            SensorsAvailable = MathHelper.SetBitByte(SensorsAvailable, 4);
        }

        /// <summary>
        /// Set that XDCR Heading sensor is not available.
        /// Bit Position 4 is the XDCR Heading sensor.
        /// </summary>
        public void SetNoXdcrHeadingSensorAvailable()
        {
            SensorsAvailable = MathHelper.ZeroBitByte(SensorsAvailable, 4);
        }

        /// <summary>
        /// Check if  XDCR Heading sensor is available.
        /// Bit Position 4 is the XDCR Heading sensor.
        /// </summary>
        /// <returns>TRUE = XDCR Heading Sensor available. / False = XDCR Heading Sensor is not available.</returns>
        public bool IsXdcrHeadingSensorAvailable()
        {
            return MathHelper.IsBitSet(SensorsAvailable, 4);
        }

        /// <summary>
        /// Check if XDCR Heading sensor is not available.
        /// </summary>
        /// <returns>TRUE = XDCR Heading Sensor is not available. / False = XDCR Heading Sensor available..</returns>
        public bool IsNoXdcrHeadingSensorAvailable()
        {
            return !IsXdcrHeadingSensorAvailable();
        }

        #endregion

        #region Depth Sensor

        /// <summary>
        /// Set that XDCR Depth sensor is available.
        /// Bit Position 5 is the XDCR Depth sensor.
        /// </summary>
        public void SetXdcrDepthSensorAvailable()
        {
            SensorsAvailable = MathHelper.SetBitByte(SensorsAvailable, 5);
        }

        /// <summary>
        /// Set that XDCR Depth sensor is not available.
        /// Bit Position 5 is the XDCR Depth sensor.
        /// </summary>
        public void SetNoXdcrDepthSensorAvailable()
        {
            SensorsAvailable = MathHelper.ZeroBitByte(SensorsAvailable, 5);
        }

        /// <summary>
        /// Check if XDCR Depth sensor is available.
        /// Bit Position 5 is the XDCR Depth sensor.
        /// </summary>
        /// <returns>TRUE = XDCR Depth Sensor available. / False = XDCR Depth Sensor is not available.</returns>
        public bool IsXdcrDepthSensorAvailable()
        {
            return MathHelper.IsBitSet(SensorsAvailable, 5);
        }

        /// <summary>
        /// Check if XDCR Depth sensor is not available.
        /// </summary>
        /// <returns>TRUE = XDCR Depth Sensor is not available. / False = XDCR Depth Sensor available.</returns>
        public bool IsNoXdcrDepthSensorAvailable()
        {
            return !IsXdcrDepthSensorAvailable();
        }

        #endregion

        #region Speed of Sound Sensor

        /// <summary>
        /// Set that Speed of Sound sensor is available.
        /// Bit Position 6 is the Speed of Sound sensor.
        /// </summary>
        public void SetSpeedOfSoundSensorAvailable()
        {
            SensorsAvailable = MathHelper.SetBitByte(SensorsAvailable, 6);
        }

        /// <summary>
        /// Set that Speed of Sound sensor is not available.
        /// Bit Position 6 is the Speed of Sound sensor.
        /// </summary>
        public void SetNoSpeedOfSoundSensorAvailable()
        {
            SensorsAvailable = MathHelper.ZeroBitByte(SensorsAvailable, 6);
        }

        /// <summary>
        /// Check if Speed of Sound sensor is available.
        /// Bit Position 6 is the Speed of Sound sensor.
        /// </summary>
        /// <returns>TRUE = Speed of Sound Sensor available. / False = Speed of Sound Sensor is not available.</returns>
        public bool IsSpeedOfSoundSensorAvailable()
        {
            return MathHelper.IsBitSet(SensorsAvailable, 6);
        }

        /// <summary>
        /// Check if Speed of Sound sensor is not available.
        /// </summary>
        /// <returns>TRUE = Speed of Sound Sensor is not available. / False = Speed of Sound Sensor available.</returns>
        public bool IsNoSpeedOfSoundSensorAvailable()
        {
            return !IsSpeedOfSoundSensorAvailable();
        }

        #endregion

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

        #region RTI Ensemble

        /// <summary>
        /// Convert the RTI Ancillary and Ensemble data set to the PD0 Fixed Leader data type.
        /// </summary>
        /// <param name="ens">RTI Ensemble data set.</param>
        /// <param name="anc">RTI Ancillary data set.</param>
        /// <param name="sysSetup">SystemSetup data set.</param>
        /// <param name="xform">Coordinate Transform.</param>
        public void DecodeRtiEnsemble(DataSet.EnsembleDataSet ens, DataSet.AncillaryDataSet anc, DataSet.SystemSetupDataSet sysSetup, PD0.CoordinateTransforms xform)
        {
            // Ensure the values were given, or create default values
            if (ens == null)
            {
                ens = new DataSet.EnsembleDataSet();
            }
            if (anc == null)
            {
                anc = new DataSet.AncillaryDataSet();
            }
            if (sysSetup == null)
            {
                sysSetup = new DataSet.SystemSetupDataSet();
            }

            CpuFirmwareVersion = (byte)ens.SysFirmware.FirmwareMinor;                           // Firmware Major
            CpuFirmwareRevision = (byte)ens.SysFirmware.FirmwareRevision;                          // Firmware Minor

            switch(ens.SubsystemConfig.SubSystem.GetSystemFrequency())
            {
                case Subsystem.SystemFrequency.Freq_75kHz:
                    SetSystemFrequency(SystemFrequency.Freq_75kHz);                             // 75 kHz
                    Set30DegreeBeamAngle();                                                     // 30 Degree Beam Angle
                    SetBeamConfiguration(BeamConfigs.BeamConfig_4_Beam_Janus);                  // 4 Beam Janus
                    SetConvex();                                                                // Set Convex
                    SetSensorConfig1();                                                         // Set Sensor Config 1
                    SetHeadAttached();                                                          // Set Head Attached
                    break;
                case Subsystem.SystemFrequency.Freq_150kHz:
                    SetSystemFrequency(SystemFrequency.Freq_150kHz);                            // 150 kHz
                    Set30DegreeBeamAngle();                                                     // 20 Degree Beam Angle
                    SetBeamConfiguration(BeamConfigs.BeamConfig_4_Beam_Janus);                  // 4 Beam Janus
                    SetConvex();                                                                // Set Convex
                    SetSensorConfig1();                                                         // Set Sensor Config 1
                    SetHeadAttached();                                                          // Set Head Attached
                    break;
                case Subsystem.SystemFrequency.Freq_300kHz:
                    SetSystemFrequency(SystemFrequency.Freq_300kHz);                            // 300 kHz
                    Set20DegreeBeamAngle();                                                     // 20 Degree Beam Angle
                    SetBeamConfiguration(BeamConfigs.BeamConfig_4_Beam_Janus);                  // 4 Beam Janus
                    SetConvex();                                                                // Set Convex
                    SetSensorConfig1();                                                         // Set Sensor Config 1
                    SetHeadAttached();                                                          // Set Head Attached
                    break;
                case Subsystem.SystemFrequency.Freq_600kHz:
                    SetSystemFrequency(SystemFrequency.Freq_600kHz);                            // 600 kHz
                    Set20DegreeBeamAngle();                                                     // 20 Degree Beam Angle
                    SetBeamConfiguration(BeamConfigs.BeamConfig_4_Beam_Janus);                  // 4 Beam Janus
                    SetConvex();                                                                // Set Convex
                    SetSensorConfig1();                                                         // Set Sensor Config 1
                    SetHeadAttached();                                                          // Set Head Attached
                    break;
                case Subsystem.SystemFrequency.Freq_1200kHz:
                    SetSystemFrequency(SystemFrequency.Freq_1200kHz);                           // 1200 kHz
                    Set20DegreeBeamAngle();                                                     // 20 Degree Beam Angle
                    SetBeamConfiguration(BeamConfigs.BeamConfig_4_Beam_Janus);                  // 4 Beam Janus
                    SetConvex();                                                                // Set Convex
                    SetSensorConfig1();                                                         // Set Sensor Config 1
                    SetHeadAttached();                                                          // Set Head Attached
                    break;
                case Subsystem.SystemFrequency.Freq_2000kHz:
                    SetSystemFrequency(SystemFrequency.Freq_2400kHz);                           // 2400 kHz
                    Set20DegreeBeamAngle();                                                     // 20 Degree Beam Angle
                    SetBeamConfiguration(BeamConfigs.BeamConfig_4_Beam_Janus);                  // 4 Beam Janus
                    SetConvex();                                                                // Set Convex
                    SetSensorConfig1();                                                         // Set Sensor Config 1
                    SetHeadAttached();                                                          // Set Head Attached
                    break;
                case Subsystem.SystemFrequency.Freq_38kHz:
                default:
                    break;
            }

            if (anc.Roll < 90 && anc.Roll > -90)
            {
                SetBeamsUpward();                                                               // Set Beams Upward
            }
            else
            {
                SetBeamsDownward();                                                             // Set Beams Downward
            }

            RealSimFlag = false;
            LagLength = 0;                                                                      // Lag Length
            NumberOfBeams = (byte)ens.NumBeams;                                                 // Number of Beams
            NumberOfCells = (byte)ens.NumBins;                                                  // Number of bins
            PingsPerEnsemble = (ushort)ens.ActualPingCount;                                     // Pings per ensemble
            DepthCellLength = (ushort)Math.Round(anc.BinSize * 100);                            // Depth Cell length
            BlankAfterTransmit = 0;                                                             // Blank - CWPBL - Do not have enough info
            ProfilingMode = 0;                                                                  // Signal Processing Mode - CWPBB - Do not have enough info
            LowCorrThresh = 0;                                                                  // Low Correlation Threshold - CWPCT - Do not have enough info
            NumCodeRepeats = (byte)sysSetup.WpRepeatN;                                          // Number of Code Repeats
            PercentGoodMinimum = 0;                                                             // Percent Good Minimum - Do not have enough info
            ErrorVelMaximum = 0;                                                                // Error Velocity Maximum - Do not have enough info
            TimeBetweenPingMinutes = 0;                                                         // Time Between Pings Minutes - CWPTBP - Do not have enough info
            TimeBetweenPingSeconds = 0;                                                         // Time Between Pings Seconds - CWPTBP - Do not have enough info
            TimeBetweenPingHundredths = 0;                                                      // Time Between Pings Hundredths -CWPTBP - Do not have enough info

            SetCoordinateTransform(xform);                                                      // Set Coordinate Transform

            HeadingAlignment = 0;                                                               // Heading alignment
            HeadingBias = 0;                                                                    // Heading Bias
            SensorSource = 0x5d;                                                                // Sensor Source
            SensorsAvailable = 0x5d;                                                            // Sensors Available
            Bin1Distance = (ushort)Math.Round(anc.FirstBinRange * 100);                         // Bin 1 distance
            XmitPulseLength = 0;                                                                // Transmit Pulse Length - Do not have enough info
            ReferenceLayerAverageStartCell = 0;                                                 // Reference Layer Average Start Cell - Do not have enough info
            ReferenceLayerAverageEndCell = 0;                                                   // Reference Layer Average End Cell - Do not have enough info
            FalseTargetThresh = 0;                                                              // False Target Threshold
            Spare_40 = 0xFE;                                                                    // Spare
            TransmitLagDistance = 0;                                                            // Lag Distance - CWPLL - Do not have enough info
            CpuBoardSerialNumber = ens.SysSerialNumber.ToString();                              // CPU Board Serial Number
            SystemBandwidth = 12;                                                               // System Bandwidth
            SystemPower = 0;                                                                    // System Power
            BaseFrequencyIndex = 0;                                                             // Base Frequency Index
            BeamAngle = 0;                                                                      // Beam Angle
        }

        #endregion
    }
}
