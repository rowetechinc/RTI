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
 * 09/01/2011      RC                     Initial coding
 * 10/04/2011      RC                     Added BinEntry to store in lists.
 * 10/24/2011      RC                     Changed from receiving a DataTable to a DataRow
 *                                         for Ensemble and Ancillary data.
 * 10/25/2011      RC                     Added Add methods that take no data so all data can be added in 1 loop.
 * 11/01/2011      RC                     Added Beam xAxis value.
 * 11/28/2011      RC                     Added Bottom Track taking Prti01Sentence.
 *                                         Added Ensemble Data taking Prti01Sentence.
 *                                         Added Ancillary Data taking Prti01Sentence.
 * 12/08/2011      RC          1.09       Adding Water Mass datasets.
 *                                         Added Orientation to all velocity data sets with a default value of down.
 *                                         Added AddNmeaData(string).
 * 12/09/2011      RC          1.09       Added Header length and other public constants about a dataset.
 *                                         Added static methods to calculate checksum and size of ensemble.
 * 12/12/2011      RC          1.09       Added BAD_VELOCITY and EMPTY_VELOCITY.
 * 12/27/2011      RC          1.10       Added index for Earth and Instrument beams.
 * 12/30/2011      RC          1.11       Renamed to Ensemble.
 * 01/13/2012      RC          1.12       Merged Ensemble table and Bottom Track table in database.
 * 01/18/2012      RC          1.14       Added Encode() to create byte array of the ensemble.
 *                                         Changed name of MAX_HEADER_COUNT to HEADER_START_COUNT.
 *                                         Changed name of DATASET_HEADER_LEN to ENSEMBLE_HEADER_LEN.
 *                                         Added EnsembleNumber property.
 * 02/13/2012      RC          2.03       Made the object serializable to allow for deep copies.
 * 02/14/2012      RC          2.03       Added BAD_RANGE.
 * 03/30/2012      RC          2.07       Moved Converters.cs methods to MathHelper.cs.
 * 04/10/2012      RC          2.08       Changed BYTES_IN_INT to BYTES_IN_INT8 and BYTES_IN_INT32.
 * 06/14/2012      RC          2.11       Added variable MAX_NUM_BINS.
 * 10/05/2012      RC          2.15       Added more description for BEAM_Q_INDEX.
 * 01/04/2013      RC          2.17       Added AddEnsembleData() that takes no data.
 * 02/13/2013      RC          2.18       Added static methods XXXBeamName() to convert the beam number to a string of the beam description for the coordinate transform.
 * 02/20/2013      RC          2.18       Added AddNmeaData() that takes no data.
 * 02/22/2013      RC          2.18       Removed the private set from all the properties so the object can be convert to a from JSON.
 * 02/25/2013      RC          2.18       Removed Orientation from all the datasets.  Replaced with SubsystemConfiguration.
 * 03/12/2013      RC          2.18       Improved the Ensemble.Clone() by using JSON to clone.       
 * 10/02/2013      RC          2.20.2     Added EncodeMatlab() to get just the ensemble as Matlab datasets with the RTI header or checksum.
 *       
 * 
 */

using System.Data;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace RTI
{
    namespace DataSet
    {
        /// <summary>
        /// Contains all the datasets within
        /// an ensemble.  An ensemble holds
        /// one instance of data from the 
        /// ADCP.  Datasets are added to the
        /// ensemble.
        /// </summary>
        [JsonConverter(typeof(EnsembleSerializer))]
        public class Ensemble
        {
            #region Variables

            #region Data Set IDs Variables

            /// <summary>
            /// Beam Velocity ID for binary format.
            /// </summary>
            public const string BeamVelocityID = "E000001\0";

            /// <summary>
            /// Instrument Velocity ID for binary format.
            /// </summary>
            public const string InstrumentVelocityID = "E000002\0";

            /// <summary>
            /// Earth Velocity ID for binary format.
            /// </summary>
            public const string EarthVelocityID = "E000003\0";

            /// <summary>
            /// Amplitude ID for binary format.
            /// </summary>
            public const string AmplitudeID = "E000004\0";

            /// <summary>
            /// Correlation ID for binary format.
            /// </summary>
            public const string CorrelationID = "E000005\0";

            /// <summary>
            /// Good Beam ID for binary format.
            /// </summary>
            public const string GoodBeamID = "E000006\0";

            /// <summary>
            /// Good Transformed Beam ID for binary format.
            /// </summary>
            public const string GoodEarthID = "E000007\0";

            /// <summary>
            /// Ensemble Data ID for binary format.
            /// </summary>
            public const string EnsembleDataID = "E000008\0";

            /// <summary>
            /// Ancillary ID for binary format.
            /// </summary>
            public const string AncillaryID = "E000009\0";

            /// <summary>
            /// Bottom Track ID for binary format.
            /// </summary>
            public const string BottomTrackID = "E000010\0";

            /// <summary>
            /// NMEA ID for binary format.
            /// </summary>
            public const string NmeaID = "E000011\0";

            /// <summary>
            /// PRTI02 ID for DVL mode format.
            /// Earth Velocity data and Water Mass.
            /// </summary>
            public const string WaterMassEarthID = "PRTI02"; 
            /// <summary>
            /// PRTI01 ID for DVL mode format.
            /// Insturment Velocity and Water Mass
            /// </summary>
            public const string WaterMassInstrumentID = "PRTI01";

            #endregion

            #region Data Type Sizes Variables

            /// <summary>
            /// Maximum number of data sets to look for.
            /// </summary>
            public const int MAX_NUM_DATA_SETS = 20;

            /// <summary>
            /// Number of bytes in the ensemble Header.
            /// </summary>
            public const int ENSEMBLE_HEADER_LEN = 32;
            
            /// <summary>
            /// Number of bytes per byte.
            /// </summary>
            public const int BYTES_IN_BYTES = 1;

            /// <summary>
            /// Number of bytes per float.
            /// </summary>
            public const int BYTES_IN_FLOAT = 4;

            /// <summary>
            /// Number of bytes per a 32bit integer.
            /// </summary>
            public const int BYTES_IN_INT32 = 4;

            /// <summary>
            /// Number of bytes in an 8 byte integer.
            /// </summary>
            public const int BYTES_IN_INT8 = 1;

            /// <summary>
            /// Default value for base data type FLOAT (ValueType).
            /// </summary>
            public const int DATATYPE_FLOAT = 10;

            /// <summary>
            /// Default value for base data type INTEGER (ValueType).
            /// </summary>
            public const int DATATYPE_INT = 20;

            /// <summary>
            /// Default value for base data type BYTE (ValueType).
            /// </summary>
            public const int DATATYPE_BYTE = 50;
            
            /// <summary>
            /// Default value for number of beams for datasets with beam data.
            /// </summary>
            public const int DEFAULT_NUM_BEAMS_BEAM = 4;

            /// <summary>
            /// Default value for number of beams for datasets without beam data (Ensemble, Ancillary, Nmea ...).
            /// </summary>
            public const int DEFAULT_NUM_BEAMS_NONBEAM = 1;

            /// <summary>
            /// Default value for IMG in base data.
            /// </summary>
            public const int DEFAULT_IMAG = 0;

            /// <summary>
            /// Default value for Name length in base data.
            /// </summary>
            public const int DEFAULT_NAME_LENGTH = 8;

            /// <summary>
            /// Number of bytes for checksum.
            /// </summary>
            public const int CHECKSUM_SIZE = 4;

            /// <summary>
            /// Number of 0x80 for header.
            /// </summary>
            public const int HEADER_START_COUNT = 16;

            /// <summary>
            /// Each payload contains a header with DataType, 
            /// Bins or Elements, Beams or 1, Image, ID (Name) 
            /// Length and ID (name).
            /// </summary>
            public const int PAYLOAD_HEADER_LEN = 28;

            #endregion

            #region Beam Index Variables

            /// <summary>
            /// Index of beams in Beam transformation.
            /// Beam 0.
            /// </summary>
            public const int BEAM_0_INDEX = 0;

            /// <summary>
            /// Index of beams in Beam transformation.
            /// Beam 1.
            /// </summary>
            public const int BEAM_1_INDEX = 1;

            /// <summary>
            /// Index of beams in Beam transformation.
            /// Beam 2.
            /// </summary>
            public const int BEAM_2_INDEX = 2;

            /// <summary>
            /// Index of beams in Beam transformation.
            /// Beam 3.
            /// </summary>
            public const int BEAM_3_INDEX = 3;

            /// <summary>
            /// Index of beams in Earth transformation.
            /// Beam East.
            /// </summary>
            public const int BEAM_EAST_INDEX = BEAM_0_INDEX;

            /// <summary>
            /// Index of beams in Earth transformation.
            /// Beam North.
            /// </summary>
            public const int BEAM_NORTH_INDEX = BEAM_1_INDEX;

            /// <summary>
            /// Index of beams in Earth transformation.
            /// Beam Vertical.
            /// </summary>
            public const int BEAM_VERTICAL_INDEX = BEAM_2_INDEX;

            /// <summary>
            /// Index of beams in Earth transformation.
            /// Beam Q.
            /// 
            /// Q value is Earth is the Error velocity.
            /// This is calculated by taking the vertical velocity
            /// of Beams 0-1 and Beams 2-3.  Beams 0-1 are opposites sides of each other and
            /// the same for 2-3.  Subtract the vertical velocity between Beams 0-1 and Beams 2-3.  This 
            /// value if there is no error should be 0.  If there is any error, then the result is the error
            /// value set for Q.
            /// </summary>
            public const int BEAM_Q_INDEX = BEAM_3_INDEX;

            /// <summary>
            /// Index of beams in Instrument transformation.
            /// Beam X.
            /// </summary>
            public const int BEAM_X_INDEX = BEAM_0_INDEX;

            /// <summary>
            /// Index of beams in Instrument transformation.
            /// Beam Y.
            /// </summary>
            public const int BEAM_Y_INDEX = BEAM_1_INDEX;

            /// <summary>
            /// Index of beams in Instrument transformation.
            /// Beam Z.
            /// </summary>
            public const int BEAM_Z_INDEX = BEAM_2_INDEX;

            #endregion

            #region Validation Variables

            /// <summary>
            /// Maximum number of bins in an ensemble.
            /// Based off command: CWPBN.
            /// </summary>
            public const int MAX_NUM_BINS = 200;

            #endregion

            #region Bad Bottom Track Range

            /// <summary>
            /// Bad Range value used with
            /// Bottom Track Range.
            /// </summary>
            public const float BAD_RANGE = 0.0f;

            #endregion

            #region Bad Velocity Variables

            /// <summary>
            /// Bad Velocity Value.
            /// </summary>
            public const float BAD_VELOCITY = 88.888F;

            /// <summary>
            /// Empty Velocity value.
            /// </summary>
            public const float EMPTY_VELOCITY = -0.0F;

            /// <summary>
            /// Place holder for bad velocity.
            /// </summary>
            public const string BAD_VELOCITY_PLACEHOLDER = "-";

            #endregion

            #region JSON Strings

            #region Available

            /// <summary>
            /// String for IsBeamVelocityAvail.
            /// </summary>
            public const string JSON_STR_ISBEAMVELOCITYAVAIL = "IsBeamVelocityAvail";

            /// <summary>
            /// String for IsInstrumentVelocityAvail.
            /// </summary>
            public const string JSON_STR_ISINSTRUMENTVELOCITYAVAIL = "IsInstrumentVelocityAvail";

            /// <summary>
            /// String for IsEarthVelocityAvail.
            /// </summary>
            public const string JSON_STR_ISEARTHVELOCITYAVAIL = "IsEarthVelocityAvail";

            /// <summary>
            /// String for IsAmplitudeAvail.
            /// </summary>
            public const string JSON_STR_ISAMPLITUDEAVAIL = "IsAmplitudeAvail";

            /// <summary>
            /// String for IsCorrelationAvail.
            /// </summary>
            public const string JSON_STR_ISCORRELATIONAVAIL = "IsCorrelationAvail";

            /// <summary>
            /// String for IsGoodBeamAvail.
            /// </summary>
            public const string JSON_STR_ISGOODBEAMAVAIL = "IsGoodBeamAvail";

            /// <summary>
            /// String for IsGoodEarthAvail.
            /// </summary>
            public const string JSON_STR_ISGOODEARTHAVAIL = "IsGoodEarthAvail";

            /// <summary>
            /// String for IsEnsembleAvail.
            /// </summary>
            public const string JSON_STR_ISENSEMBLEAVAIL = "IsEnsembleAvail";

            /// <summary>
            /// String for IsAncillaryAvail.
            /// </summary>
            public const string JSON_STR_ISANCILLARYAVAIL = "IsAncillaryAvail";

            /// <summary>
            /// String for IsBottomTrackAvail.
            /// </summary>
            public const string JSON_STR_ISBOTTOMTRACKAVAIL = "IsBottomTrackAvail";

            /// <summary>
            /// String for IsEarthWaterMassAvail.
            /// </summary>
            public const string JSON_STR_ISEARTHWATERMASSAVAIL = "IsEarthWaterMassAvail";

            /// <summary>
            /// String for IsInstrumentWaterMassAvail.
            /// </summary>
            public const string JSON_STR_ISINSTRUMENTWATERMASSAVAIL = "IsInstrumentWaterMassAvail";

            /// <summary>
            /// String for IsNmeaAvail.
            /// </summary>
            public const string JSON_STR_ISNMEAAVAIL = "IsNmeaAvail";

            #endregion

            #region DataSets

            /// <summary>
            /// String for BeamVelocityData.
            /// </summary>
            public const string JSON_STR_BEAMVELOCITYDATA = "BeamVelocityData";

            /// <summary>
            /// String for InstrumentVelocityData.
            /// </summary>
            public const string JSON_STR_INSTRUMENTVELOCITYDATA = "InstrumentVelocityData";

            /// <summary>
            /// String for EarthVelocityData.
            /// </summary>
            public const string JSON_STR_EARTHVELOCITYDATA = "EarthVelocityData";

            /// <summary>
            /// String for AmplitudeData.
            /// </summary>
            public const string JSON_STR_AMPLITUDEDATA = "AmplitudeData";

            /// <summary>
            /// String for CorrelationData.
            /// </summary>
            public const string JSON_STR_CORRELATIONDATA = "CorrelationData";

            /// <summary>
            /// String for GoodBeamData.
            /// </summary>
            public const string JSON_STR_GOODBEAMDATA = "GoodBeamData";

            /// <summary>
            /// String for GoodEarthData.
            /// </summary>
            public const string JSON_STR_GOODEARTHDATA = "GoodEarthData";

            /// <summary>
            /// String for EnsembleData.
            /// </summary>
            public const string JSON_STR_ENSEMBLEDATA = "EnsembleData";

            /// <summary>
            /// String for AncillaryData.
            /// </summary>
            public const string JSON_STR_ANCILLARYDATA = "AncillaryData";

            /// <summary>
            /// String for BottomTrackData.
            /// </summary>
            public const string JSON_STR_BOTTOMTRACKDATA = "BottomTrackData";

            /// <summary>
            /// String for EarthWaterMassData.
            /// </summary>
            public const string JSON_STR_EARTHWATERMASSDATA = "EarthWaterMassData";

            /// <summary>
            /// String for InstrumentWaterMassData.
            /// </summary>
            public const string JSON_STR_INSTRUMENTWATERMASSDATA = "InstrumentWaterMassData";

            /// <summary>
            /// String for NmeaData.
            /// </summary>
            public const string JSON_STR_NMEADATA = "NmeaData";

            #endregion

            #endregion

            #endregion

            #region Properties

            #region Data Set Available Properties

            /// <summary>
            /// Set if the Beam velocity data set is available for this data set
            /// </summary>
            public bool IsBeamVelocityAvail { get; set; }

            /// <summary>
            /// Set if the InstrumentVelocity velocity data set is available for this data set
            /// </summary>
            public bool IsInstrumentVelocityAvail { get; set; }

            /// <summary>
            /// Set if the EarthVelocity velocity data set is available for this data set
            /// </summary>
            public bool IsEarthVelocityAvail { get; set; }

            /// <summary>
            /// Set if the Amplitude data set is available for this data set
            /// </summary>
            public bool IsAmplitudeAvail { get; set; }

            /// <summary>
            /// Set if the Correlation data set is available for this data set
            /// </summary>
            public bool IsCorrelationAvail { get; set; }

            /// <summary>
            /// Set if the Good Beam data set is available for this data set
            /// </summary>
            public bool IsGoodBeamAvail { get; set; }

            /// <summary>
            /// Set if the Good EarthVelocity data set is available for this data set
            /// </summary>
            public bool IsGoodEarthAvail { get; set; }

            /// <summary>
            /// Set if the Ensemble data set is available for this data set
            /// </summary>
            public bool IsEnsembleAvail { get; set; }

            /// <summary>
            /// Set if the Ancillary data set is available for this data set
            /// </summary>
            public bool IsAncillaryAvail { get; set; }

            /// <summary>
            /// Set if the Bottom Track data set is available for this data set
            /// </summary>
            public bool IsBottomTrackAvail { get; set; }

            /// <summary>
            /// Set if the Earth Water Mass data set is available for this data set
            /// </summary>
            public bool IsEarthWaterMassAvail { get; set; }

            /// <summary>
            /// Set if the Insturment Water Mass data set is available for this data set
            /// </summary>
            public bool IsInstrumentWaterMassAvail { get; set; }

            /// <summary>
            /// Set if the NMEA data set is available for this data set
            /// </summary>
            public bool IsNmeaAvail { get; set; }

            #endregion

            #region Data Sets Properties

            /// <summary>
            /// Beam Velocity Data set for this data set.
            /// </summary>
            public BeamVelocityDataSet BeamVelocityData { get; set; }

            /// <summary>
            /// Instrument Velocity Data set for this data set.
            /// </summary>
            public InstrumentVelocityDataSet InstrumentVelocityData { get; set; }

            /// <summary>
            /// Earth Velocity Data set for this data set.
            /// </summary>
            public EarthVelocityDataSet EarthVelocityData { get; set; }

            /// <summary>
            /// Amplitude Data set for this data set.
            /// </summary>
            public AmplitudeDataSet AmplitudeData { get; set; }

            /// <summary>
            /// Correlation Data set for this data set.
            /// </summary>
            public CorrelationDataSet CorrelationData { get; set; }

            /// <summary>
            /// Good Beam Data set for this data set.
            /// </summary>
            public GoodBeamDataSet GoodBeamData { get; set; }

            /// <summary>
            /// Good Velocity Data set for this data set.
            /// </summary>
            public GoodEarthDataSet GoodEarthData { get; set; }

            /// <summary>
            /// Ensemble Data set for this data set.
            /// </summary>
            public EnsembleDataSet EnsembleData { get; set; }

            /// <summary>
            /// Ancillary Data set for this data set.
            /// </summary>
            public AncillaryDataSet AncillaryData { get; set; }

            /// <summary>
            /// Bottom Track Data set for this data set.
            /// </summary>
            public BottomTrackDataSet BottomTrackData { get; set; }

            /// <summary>
            /// Water Mass Earth Velocity Data set for this data set.
            /// </summary>
            public EarthWaterMassDataSet EarthWaterMassData { get; set; }

            /// <summary>
            /// Water Mass Instrument Velocity Data set for this data set.
            /// </summary>
            public InstrumentWaterMassDataSet InstrumentWaterMassData { get; set; }

            /// <summary>
            /// NMEA Data set for this data set.
            /// </summary>
            public NmeaDataSet NmeaData { get; set; }

            #endregion

            #endregion

            /// <summary>
            /// Constructor
            /// 
            /// Initialize all ranges.
            /// </summary>
            public Ensemble()
            {
                // Set ensemble number
                //EnsembleNumber = ensNum;

                // Initialize all ranges
                IsBeamVelocityAvail = false;
                IsInstrumentVelocityAvail = false;
                IsEarthVelocityAvail = false;
                IsAmplitudeAvail = false;
                IsCorrelationAvail = false;
                IsGoodBeamAvail = false;
                IsGoodEarthAvail = false;
                IsEnsembleAvail = false;
                IsAncillaryAvail = false;
                IsBottomTrackAvail = false;
                IsEarthWaterMassAvail = false;
                IsInstrumentWaterMassAvail = false;
                IsNmeaAvail = false;
            }

            /// <summary>
            /// Create an Ensemble data set.  Intended for JSON  deserialize.  This method
            /// is called when Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.EnsembleDataSet}(json) is
            /// called.
            /// 
            /// DeserializeObject is slightly faster then passing the string to the constructor.
            /// 162ms for this method.
            /// 181ms for JSON string constructor.
            /// 
            /// Alternative to decoding manually is to use the command:
            /// DataSet.EnsembleDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.EnsembleDataSet}(json); 
            /// 
            /// To use this method for JSON you must have all the parameters match all the properties in this object.
            /// 
            /// </summary>
            /// <param name="IsBeamVelocityAvail">Flag if Beam Velocity DataSet Is Available.</param>
            /// <param name="IsInstrumentVelocityAvail">Flag if Instrument Velocity DataSet Is Available.</param>
            /// <param name="IsEarthVelocityAvail">Flag if Earth Velocity DataSet Is Available.</param>
            /// <param name="IsAmplitudeAvail">Flag if Amplitude DataSet Is Available.</param>
            /// <param name="IsCorrelationAvail">Flag if Correlation DataSet Is Available.</param>
            /// <param name="IsGoodBeamAvail">Flag if Good Beam DataSet Is Available.</param>
            /// <param name="IsGoodEarthAvail">Flag if Good Earth DataSet Is Available.</param>
            /// <param name="IsEnsembleAvail">Flag if Ensemble DataSet Is Available.</param>
            /// <param name="IsAncillaryAvail">Flag if Ancillary DataSet Is Available.</param>
            /// <param name="IsBottomTrackAvail">Flag if Bottom Track DataSet Is Available.</param>
            /// <param name="IsEarthWaterMassAvail">Flag if Earth Water Mass Velocity DataSet Is Available.</param>
            /// <param name="IsInstrumentWaterMassAvail">Flag if Instrument Water Mass Velocity DataSet Is Available.</param>
            /// <param name="IsNmeaAvail">Flag if Nmea DataSet Is Available.</param>
            /// <param name="BeamVelocityData">Beam Velocity DataSet.</param>
            /// <param name="InstrumentVelocityData">Instrument Velocity DataSet.</param>
            /// <param name="EarthVelocityData">Earth Velocity DataSet.</param>
            /// <param name="AmplitudeData">Amplitude DataSet.</param>
            /// <param name="CorrelationData">Correlation DataSet.</param>
            /// <param name="GoodBeamData">Good Beam DataSet.</param>
            /// <param name="GoodEarthData">Good Earth DataSet.</param>
            /// <param name="EnsembleData">Ensemble DataSet.</param>
            /// <param name="AncillaryData">Ancillary DataSet.</param>
            /// <param name="BottomTrackData">Bottom Track DataSet.</param>
            /// <param name="EarthWaterMassData">Earth Water Mass Velocity DataSet.</param>
            /// <param name="InstrumentWaterMassData">Instrument Water Mass Velocity DataSet.</param>
            /// <param name="NmeaData">Nmea DataSet.</param>
            [JsonConstructor]
            public Ensemble(bool IsBeamVelocityAvail, bool IsInstrumentVelocityAvail, bool IsEarthVelocityAvail, bool IsAmplitudeAvail, bool IsCorrelationAvail,
                            bool IsGoodBeamAvail, bool IsGoodEarthAvail, bool IsEnsembleAvail, bool IsAncillaryAvail, bool IsBottomTrackAvail,
                            bool IsEarthWaterMassAvail, bool IsInstrumentWaterMassAvail, bool IsNmeaAvail,
                            BeamVelocityDataSet BeamVelocityData, InstrumentVelocityDataSet InstrumentVelocityData, EarthVelocityDataSet EarthVelocityData,
                            AmplitudeDataSet AmplitudeData, CorrelationDataSet CorrelationData, GoodBeamDataSet GoodBeamData, GoodEarthDataSet GoodEarthData,
                            EnsembleDataSet EnsembleData, AncillaryDataSet AncillaryData, BottomTrackDataSet BottomTrackData, EarthWaterMassDataSet EarthWaterMassData,
                            InstrumentWaterMassDataSet InstrumentWaterMassData, NmeaDataSet NmeaData)
            {
                // Initialize all ranges
                this.IsBeamVelocityAvail = IsBeamVelocityAvail;
                this.IsInstrumentVelocityAvail = IsInstrumentVelocityAvail;
                this.IsEarthVelocityAvail = IsEarthVelocityAvail;
                this.IsAmplitudeAvail = IsAmplitudeAvail;
                this.IsCorrelationAvail = IsCorrelationAvail;
                this.IsGoodBeamAvail = IsGoodBeamAvail;
                this.IsGoodEarthAvail = IsGoodEarthAvail;
                this.IsEnsembleAvail = IsEnsembleAvail;
                this.IsAncillaryAvail = IsAncillaryAvail;
                this.IsBottomTrackAvail = IsBottomTrackAvail;
                this.IsEarthWaterMassAvail = IsEarthWaterMassAvail;
                this.IsInstrumentWaterMassAvail = IsInstrumentWaterMassAvail;
                this.IsNmeaAvail = IsNmeaAvail;

                this.BeamVelocityData = BeamVelocityData;
                this.InstrumentVelocityData = InstrumentVelocityData;
                this.EarthVelocityData = EarthVelocityData;
                this.AmplitudeData = AmplitudeData;
                this.CorrelationData = CorrelationData;
                this.GoodBeamData = GoodBeamData;
                this.GoodEarthData = GoodEarthData;
                this.EnsembleData = EnsembleData;
                this.AncillaryData = AncillaryData;
                this.BottomTrackData = BottomTrackData;
                this.EarthWaterMassData = EarthWaterMassData;
                this.InstrumentWaterMassData = InstrumentWaterMassData;
                this.NmeaData = NmeaData;
            }

            #region Beam Velocity Data Set

            /// <summary>
            /// Add the Beam Velocity data set to the data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public void AddBeamVelocityData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name)
            {
                IsBeamVelocityAvail = true;
                BeamVelocityData = new BeamVelocityDataSet(valueType, numBins, numBeams, imag, nameLength, name);
            }

            /// <summary>
            /// Add the Beam Velocity data set to the data.
            /// This will add the Beam velocity data and decode the byte array
            /// for all the Beam velocity data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="velocityData">Byte array containing Beam velocity data</param>
            public void AddBeamVelocityData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] velocityData)
            {
                IsBeamVelocityAvail = true;
                BeamVelocityData = new BeamVelocityDataSet(valueType, numBins, numBeams, imag, nameLength, name, velocityData);
            }

            #endregion

            #region Instrument Velocity Data Set

            /// <summary>
            /// Add the Instrument Velocity data set to the data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public void AddInstrumentVelocityData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name)
            {
                IsInstrumentVelocityAvail = true;
                InstrumentVelocityData = new InstrumentVelocityDataSet(valueType, numBins, numBeams, imag, nameLength, name);
            }

            /// <summary>
            /// Add the Instrument Velocity data set to the data.
            /// This will add the Instrument Velocity data and decode the byte array
            /// for all the Instrument Velocity data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="velocityData">Byte array containing InstrumentVelocity velocity data</param>
            public void AddInstrumentVelocityData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] velocityData)
            {
                IsInstrumentVelocityAvail = true;
                InstrumentVelocityData = new InstrumentVelocityDataSet(valueType, numBins, numBeams, imag, nameLength, name, velocityData);
            }

            #endregion

            #region Earth Velocity Data Set

            /// <summary>
            /// Add the Earth Velocity data set to the data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public void AddEarthVelocityData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name)
            {
                IsEarthVelocityAvail = true;
                EarthVelocityData = new EarthVelocityDataSet(valueType, numBins, numBeams, imag, nameLength, name);
            }

            /// <summary>
            /// Add the Earth Velocity data set to the data.
            /// This will add the Earth Velocity data and decode the byte array
            /// for all the Earth Velocity data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="velocityData">Byte array containing EarthVelocity velocity data</param>
            public void AddEarthVelocityData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] velocityData)
            {
                IsEarthVelocityAvail = true;
                EarthVelocityData = new EarthVelocityDataSet(valueType, numBins, numBeams, imag, nameLength, name, velocityData);
            }

            #endregion

            #region Amplitude Data Set

            /// <summary>
            /// Add the Amplitude data set to the data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public void AddAmplitudeData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name)
            {
                IsAmplitudeAvail = true;
                AmplitudeData = new AmplitudeDataSet(valueType, numBins, numBeams, imag, nameLength, name);
            }

            /// <summary>
            /// Add the Amplitude data set to the data.
            /// This will add the Amplitude data and decode the byte array
            /// for all the Amplitude data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="amplitudeData">Byte array containing Amplitude data</param>
            public void AddAmplitudeData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] amplitudeData)
            {
                IsAmplitudeAvail = true;
                AmplitudeData = new AmplitudeDataSet(valueType, numBins, numBeams, imag, nameLength, name, amplitudeData);
            }

            #endregion

            #region Correlation Data Set

            /// <summary>
            /// Add the Correlation data set to the data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public void AddCorrelationData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name)
            {
                IsCorrelationAvail = true;
                CorrelationData = new CorrelationDataSet(valueType, numBins, numBeams, imag, nameLength, name);
            }

            /// <summary>
            /// Add the Correlation data set to the data.
            /// This will add the Correlation data and decode the byte array
            /// for all the Correlation data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="correlationData">Byte array containing Correlation data</param>
            public void AddCorrelationData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] correlationData)
            {
                IsCorrelationAvail = true;
                CorrelationData = new CorrelationDataSet(valueType, numBins, numBeams, imag, nameLength, name, correlationData);
            }

            #endregion

            #region Good Beam Data Set

            /// <summary>
            /// Add the Good Beam data set to the data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public void AddGoodBeamData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name)
            {
                IsGoodBeamAvail = true;
                GoodBeamData = new GoodBeamDataSet(valueType, numBins, numBeams, imag, nameLength, name);
            }

            /// <summary>
            /// Add the Good Beam data set to the data.
            /// This will add the Good Beam data and decode the byte array
            /// for all the Good Beam data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="goodBeamData">Byte array containing Good Beam data</param>
            public void AddGoodBeamData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] goodBeamData)
            {
                IsGoodBeamAvail = true;
                GoodBeamData = new GoodBeamDataSet(valueType, numBins, numBeams, imag, nameLength, name, goodBeamData);
            }

            #endregion

            #region Good Earth Data Set

            /// <summary>
            /// Add the Good Earth data set to the data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public void AddGoodEarthData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name)
            {
                IsGoodEarthAvail = true;
                GoodEarthData = new GoodEarthDataSet(valueType, numBins, numBeams, imag, nameLength, name);
            }

            /// <summary>
            /// Add the Good Earth data set to the data.
            /// This will add the Good Earth data and decode the byte array
            /// for all the Good Earth data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="goodEarthData">Byte array containing Good Earth data</param>
            public void AddGoodEarthData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] goodEarthData)
            {
                IsGoodEarthAvail = true;
                GoodEarthData = new GoodEarthDataSet(valueType, numBins, numBeams, imag, nameLength, name, goodEarthData);
            }

            #endregion

            #region Ensemble Data Set

            /// <summary>
            /// Add the Ensemble data set to the data.
            /// This will add the Ensemble data and take no data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public void AddEnsembleData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name)
            {
                IsEnsembleAvail = true;
                EnsembleData = new EnsembleDataSet(valueType, numBins, numBeams, imag, nameLength, name);
            }

            /// <summary>
            /// Add the Ensemble data set to the data.
            /// This will add the Ensemble data and decode the byte array
            /// for all the Ensemble data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="ensembleData">Byte array containing Ensemble data</param>
            public void AddEnsembleData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] ensembleData)
            {
                IsEnsembleAvail = true;
                EnsembleData = new EnsembleDataSet(valueType, numBins, numBeams, imag, nameLength, name, ensembleData);
            }

            /// <summary>
            /// Add the Ensemble number and time based off the 
            /// Prti01Sentence given.
            /// </summary>
            /// <param name="sent">Sentence containing data.</param>
            public void AddEnsembleData(Prti01Sentence sent)
            {
                IsEnsembleAvail = true;
                EnsembleData = new EnsembleDataSet(sent);
            }

            /// <summary>
            /// Add the Ensemble number and time based off the 
            /// Prti02Sentence given.
            /// </summary>
            /// <param name="sent">Sentence containing data.</param>
            public void AddEnsembleData(Prti02Sentence sent)
            {
                IsEnsembleAvail = true;
                EnsembleData = new EnsembleDataSet(sent);
            }

            #endregion

            #region Ancillary Data Set

            /// <summary>
            /// Add the Ancillary data set to the data.
            /// This will add the Ancillary data and decode the byte array
            /// for all the Ancillary data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public void AddAncillaryData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name)
            {
                IsAncillaryAvail = true;
                AncillaryData = new AncillaryDataSet(valueType, numBins, numBeams, imag, nameLength, name);
            }

            /// <summary>
            /// Add the Ancillary data set to the data.
            /// This will add the Ancillary data and decode the byte array
            /// for all the Ancillary data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="ancillaryData">Byte array containing Ancillary data</param>
            public void AddAncillaryData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] ancillaryData)
            {
                IsAncillaryAvail = true;
                AncillaryData = new AncillaryDataSet(valueType, numBins, numBeams, imag, nameLength, name, ancillaryData);
            }

            /// <summary>
            /// Add the temperature based off the 
            /// Prti01Sentence given.
            /// </summary>
            /// <param name="sent">Sentence containing data.</param>
            public void AddAncillaryData(Prti01Sentence sent)
            {
                IsAncillaryAvail = true;
                AncillaryData = new AncillaryDataSet(sent);
            }

            /// <summary>
            /// Add the temperature based off the 
            /// Prti02Sentence given.
            /// </summary>
            /// <param name="sent">Sentence containing data.</param>
            public void AddAncillaryData(Prti02Sentence sent)
            {
                IsAncillaryAvail = true;
                AncillaryData = new AncillaryDataSet(sent);
            }

            #endregion

            #region Bottom Track Data Set

            /// <summary>
            /// Add the Bottom Track data set to the data.
            /// This will add the Bottom Track data with NO data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public void AddBottomTrackData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name)
            {
                IsBottomTrackAvail = true;
                BottomTrackData = new BottomTrackDataSet(valueType, numBins, numBeams, imag, nameLength, name);
            }

            /// <summary>
            /// Add the Bottom Track data set to the data.
            /// This will add the Bottom Track data and decode the byte array
            /// for all the Bottom Track data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="bottomTrackData">Byte array containing Bottom Track data</param>
            public void AddBottomTrackData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] bottomTrackData)
            {
                IsBottomTrackAvail = true;
                BottomTrackData = new BottomTrackDataSet(valueType, numBins, numBeams, imag, nameLength, name, bottomTrackData);
            }

            /// <summary>
            /// Add the Bottom Track data set to the data.
            /// This will add the Bottom Track data and decode the Prti01Sentence
            /// for all the Bottom Track data;
            /// </summary>
            /// <param name="sentence">DVL message containing Bottom Track data.</param>
            public void AddBottomTrackData(Prti01Sentence sentence)
            {
                IsBottomTrackAvail = true;
                BottomTrackData = new BottomTrackDataSet(sentence);
            }

            /// <summary>
            /// Add the Bottom Track data set to the data.
            /// This will add the Bottom Track data and decode the Prti02Sentence
            /// for all the Bottom Track data;
            /// </summary>
            /// <param name="sentence">DVL message containing Bottom Track data.</param>
            public void AddBottomTrackData(Prti02Sentence sentence)
            {
                IsBottomTrackAvail = true;
                BottomTrackData = new BottomTrackDataSet(sentence);
            }

            /// <summary>
            /// Take existing Bottom Track data and add additional 
            /// Bottom Track data from the Prti02Sentence.
            /// </summary>
            /// <param name="sentence">Sentence containing additional Bottom Track data.</param>
            public void AddAdditionalBottomTrackData(Prti02Sentence sentence)
            {
                if (IsBottomTrackAvail)
                {
                    BottomTrackData.AddAdditionalBottomTrackData(sentence);
                }
            }

            #endregion

            #region Earth Water Mass Data Set

            /// <summary>
            /// Add the Earth Water Mass Velocity data to the dataset.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="east">East Velocity.</param>
            /// <param name="north">North Velocity.</param>
            /// <param name="vertical">Vertical Velocity.</param>
            /// <param name="depthLayer">Water Mass Depth Layer.</param>
            public void AddEarthWaterMassData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, float east, float north, float vertical, float depthLayer)
            {
                IsEarthWaterMassAvail = true;
                EarthWaterMassData = new EarthWaterMassDataSet(valueType, numBins, numBeams, imag, nameLength, name, east, north, vertical, depthLayer);
            }

            /// <summary>
            /// Add the Earth Water Mass Velocity data based off the 
            /// Prti02Sentence given.
            /// </summary>
            /// <param name="sent">Sentence containing data.</param>
            public void AddEarthWaterMassData(Prti02Sentence sent)
            {
                IsEarthWaterMassAvail = true;
                EarthWaterMassData = new EarthWaterMassDataSet(sent);
            }

            #endregion

            #region Instrument Water Mass Data Set

            /// <summary>
            /// Add the Instrument Water Mass Velocity data to the dataset.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="x">X Velocity.</param>
            /// <param name="y">Y Velocity.</param>
            /// <param name="z">Z Velocity.</param>
            /// <param name="depthLayer">Water Mass Depth Layer.</param>
            public void AddInstrumentWaterMassData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, float x, float y, float z, float depthLayer)
            {
                IsInstrumentWaterMassAvail = true;
                InstrumentWaterMassData = new InstrumentWaterMassDataSet(valueType, numBins, numBeams, imag, nameLength, name, x, y, z, depthLayer);
            }

            /// <summary>
            /// Add the Instrument Water Mass Velocity data based off the 
            /// Prti02Sentence given.
            /// </summary>
            /// <param name="sent">Sentence containing data.</param>
            public void AddInstrumentWaterMassData(Prti01Sentence sent)
            {
                IsInstrumentWaterMassAvail = true;
                InstrumentWaterMassData = new InstrumentWaterMassDataSet(sent);
            }

            #endregion

            #region NMEA Data Set

            /// <summary>
            /// Add the NMEA data set to the data.
            /// This will add an empty NMEA data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public void AddNmeaData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name)
            {
                IsNmeaAvail = true;
                NmeaData = new NmeaDataSet(valueType, numBins, numBeams, imag, nameLength, name);
            }

            /// <summary>
            /// Add the NMEA data set to the data.
            /// This will add the NMEA data and decode the byte array
            /// for all the NMEA data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="nmeaData">Byte array containing NMEA data</param>
            public void AddNmeaData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] nmeaData)
            {
                IsNmeaAvail = true;
                NmeaData = new NmeaDataSet(valueType, numBins, numBeams, imag, nameLength, name, nmeaData);
            }

            /// <summary>
            /// Add the NMEA data set to the data.
            /// This will add the NMEA data and decode the string
            /// for all the NMEA data.
            /// </summary>
            /// <param name="nmeaData">String containing NMEA data</param>
            public void AddNmeaData(string nmeaData)
            {
                IsNmeaAvail = true;
                NmeaData = new NmeaDataSet(nmeaData);
            }

            #endregion

            #region Clone

            /// <summary>
            /// Make a deep copy of this object.
            /// This will clone the object and
            /// return a new object.
            /// This will add all the datasets that exist
            /// in this ensemble to a new ensemble.
            /// </summary>
            /// <returns>A new object identical to this object.  Deep Copy.</returns>
            public Ensemble Clone()
            {
                //Ensemble ensemble = new Ensemble();

                //if (IsBeamVelocityAvail)
                //{
                //    ensemble.AddBeamVelocityData(BeamVelocityData.ValueType, BeamVelocityData.NumElements, BeamVelocityData.ElementsMultiplier, BeamVelocityData.Imag, BeamVelocityData.NameLength, BeamVelocityData.Name, BeamVelocityData.Encode());
                //}

                //if (IsInstrumentVelocityAvail)
                //{
                //    ensemble.AddInstrumentVelocityData(InstrumentVelocityData.ValueType, InstrumentVelocityData.NumElements, InstrumentVelocityData.ElementsMultiplier, InstrumentVelocityData.Imag, InstrumentVelocityData.NameLength, InstrumentVelocityData.Name, InstrumentVelocityData.Encode());
                //}

                //if (IsEarthVelocityAvail)
                //{
                //    ensemble.AddEarthVelocityData(EarthVelocityData.ValueType, EarthVelocityData.NumElements, EarthVelocityData.ElementsMultiplier, EarthVelocityData.Imag, EarthVelocityData.NameLength, EarthVelocityData.Name, EarthVelocityData.Encode());
                //}

                //if (IsAmplitudeAvail)
                //{
                //    ensemble.AddAmplitudeData(AmplitudeData.ValueType, AmplitudeData.NumElements, AmplitudeData.ElementsMultiplier, AmplitudeData.Imag, AmplitudeData.NameLength, AmplitudeData.Name, AmplitudeData.Encode());
                //}

                //if (IsCorrelationAvail)
                //{
                //    ensemble.AddCorrelationData(CorrelationData.ValueType, CorrelationData.NumElements, CorrelationData.ElementsMultiplier, CorrelationData.Imag, CorrelationData.NameLength, CorrelationData.Name, CorrelationData.Encode());
                //}

                //if (IsGoodBeamAvail)
                //{
                //    ensemble.AddGoodBeamData(GoodBeamData.ValueType, GoodBeamData.NumElements, GoodBeamData.ElementsMultiplier, GoodBeamData.Imag, GoodBeamData.NameLength, GoodBeamData.Name, GoodBeamData.Encode());
                //}

                //if (IsGoodEarthAvail)
                //{
                //    ensemble.AddGoodEarthData(GoodEarthData.ValueType, GoodEarthData.NumElements, GoodEarthData.ElementsMultiplier, GoodEarthData.Imag, GoodEarthData.NameLength, GoodEarthData.Name, GoodEarthData.Encode());
                //}

                //if (IsEnsembleAvail && EnsembleData != null)
                //{
                //    ensemble.AddEnsembleData(EnsembleData.ValueType, EnsembleData.NumElements, EnsembleData.ElementsMultiplier, EnsembleData.Imag, EnsembleData.NameLength, EnsembleData.Name, EnsembleData.Encode());
                //}

                //if (IsAncillaryAvail)
                //{
                //    ensemble.AddAncillaryData(AncillaryData.ValueType, AncillaryData.NumElements, AncillaryData.ElementsMultiplier, AncillaryData.Imag, AncillaryData.NameLength, AncillaryData.Name, AncillaryData.Encode());
                //}

                //if (IsBottomTrackAvail)
                //{
                //    ensemble.AddBottomTrackData(BottomTrackData.ValueType, BottomTrackData.NumElements, BottomTrackData.ElementsMultiplier, BottomTrackData.Imag, BottomTrackData.NameLength, BottomTrackData.Name, BottomTrackData.Encode());
                //}

                //if (IsEarthWaterMassAvail)
                //{
                //    ensemble.AddEarthWaterMassData(EarthWaterMassData.ValueType, EarthWaterMassData.NumElements, EarthWaterMassData.ElementsMultiplier, EarthWaterMassData.Imag, EarthWaterMassData.NameLength, EarthWaterMassData.Name, EarthWaterMassData.VelocityEast, EarthWaterMassData.VelocityNorth, EarthWaterMassData.VelocityVertical, EarthWaterMassData.WaterMassDepthLayer);
                //}

                //if (IsInstrumentWaterMassAvail)
                //{
                //    ensemble.AddInstrumentWaterMassData(InstrumentWaterMassData.ValueType, InstrumentWaterMassData.NumElements, InstrumentWaterMassData.ElementsMultiplier, InstrumentWaterMassData.Imag, InstrumentWaterMassData.NameLength, InstrumentWaterMassData.Name, InstrumentWaterMassData.VelocityX, InstrumentWaterMassData.VelocityY, InstrumentWaterMassData.VelocityZ, InstrumentWaterMassData.WaterMassDepthLayer);
                //}

                //if(IsNmeaAvail)
                //{
                //    ensemble.AddNmeaData(NmeaData.ValueType, NmeaData.NumElements, NmeaData.ElementsMultiplier, NmeaData.Imag, NmeaData.NameLength, NmeaData.Name, NmeaData.Encode());
                //}

                //return ensemble;

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(this);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.Ensemble>(json);
            }
            #endregion

            #region Strings

            /// <summary>
            /// String representation of a the entire ensemble.
            /// If the dataset is available, add it to the string.
            /// </summary>
            /// <returns>String representation of the entire ensemble.</returns>
            public override string ToString()
            {
                string s = "";
                // Check if the dataset is available.
                // If it is, gets its string representation
                if (IsEnsembleAvail)
                {
                    s += EnsembleData.ToString();
                }
                if (IsAncillaryAvail)
                {
                    s += AncillaryData.ToString();
                }
                if (IsBeamVelocityAvail)
                {
                    s += BeamVelocityData.ToString();
                }
                if (IsInstrumentVelocityAvail)
                {
                    s += InstrumentVelocityData.ToString();
                }
                if (IsEarthVelocityAvail)
                {
                    s += EarthVelocityData.ToString();
                }
                if (IsAmplitudeAvail)
                {
                    s += AmplitudeData.ToString();
                }
                if (IsCorrelationAvail)
                {
                    s += CorrelationData.ToString();
                }
                if (IsGoodBeamAvail)
                {
                    s += GoodBeamData.ToString();
                }
                if (IsGoodEarthAvail)
                {
                    s += GoodEarthData.ToString();
                }
                if (IsBottomTrackAvail)
                {
                    s += BottomTrackData.ToString();
                }
                if (IsEarthWaterMassAvail)
                {
                    s += EarthWaterMassData.ToString();
                }
                if (IsInstrumentWaterMassAvail)
                {
                    s += InstrumentWaterMassData.ToString();
                }

                return s;
            }

            /// <summary>
            /// Based off the given beam number, give
            /// the name of the beam in Beam Coordinate transform.
            /// Beam 0, Beam 1, Beam 2 and Beam 3.
            /// </summary>
            /// <param name="beam">Beam number.</param>
            /// <returns>Name for the beam number in Beam Coordinate Transform.</returns>
            public static string BeamBeamName(int beam)
            {
                return string.Format("Beam {0}", beam);
            }

            /// <summary>
            /// Based off the given beam number, give
            /// the name of the beam in Earth Coordinate transform.
            /// East, North, Vertical or Q.
            /// </summary>
            /// <param name="beam">Beam number.</param>
            /// <returns>Name for the beam number in Earth Coordinate Transform.</returns>
            public static string EarthBeamName(int beam)
            {
                switch (beam)
                {
                    case DataSet.Ensemble.BEAM_EAST_INDEX:
                        return "East";
                    case DataSet.Ensemble.BEAM_NORTH_INDEX:
                        return "North";
                    case DataSet.Ensemble.BEAM_VERTICAL_INDEX:
                        return "Vertical";
                    case DataSet.Ensemble.BEAM_Q_INDEX:
                        return "Q";
                    default:
                        return "";
                }
            }

            /// <summary>
            /// Based off the given beam number, give
            /// the name of the beam in Instrument Coordinate transform.
            /// X, Y, Z, and Q.
            /// </summary>
            /// <param name="beam">Beam number.</param>
            /// <returns>Name for the beam number in Instrument Coordinate Transform.</returns>
            public static string InstrumentBeamName(int beam)
            {
                switch (beam)
                {
                    case DataSet.Ensemble.BEAM_X_INDEX:
                        return "X";
                    case DataSet.Ensemble.BEAM_Y_INDEX:
                        return "Y";
                    case DataSet.Ensemble.BEAM_Z_INDEX:
                        return "Z";
                    case DataSet.Ensemble.BEAM_Q_INDEX:
                        return "Q";
                    default:
                        return "";
                }
            }

            #endregion

            #region Checksum and Sizes

            /// <summary>
            /// Get the playload size from the ensemble.
            /// Then add in the header size and the checksum
            /// size to get the overall size of the ensemble.
            /// </summary>
            /// <param name="payloadSize">Size of the payload find in the ensemble header.</param>
            /// <returns>Total size of the ensemble including header, payload and checksum.</returns>
            public static int CalculateEnsembleSize(int payloadSize)
            {
                return payloadSize + DataSet.Ensemble.ENSEMBLE_HEADER_LEN + DataSet.Ensemble.CHECKSUM_SIZE;
            }

            /// <summary>
            /// Calculate the checksum for the given ensemble.
            /// This will use CRC-16 to calculate the checksum.
            /// Give all bytes in the Ensemble including the checksum.
            /// </summary>
            /// <param name="ensemble">Byte array of the data.</param>
            /// <returns>Checksum value for the given byte[].</returns>
            public static long CalculateEnsembleChecksum(byte[] ensemble)
            {
                ushort crc = 0;

                // Do not include the checksum to calculate the checksum
                for (int i = DataSet.Ensemble.ENSEMBLE_HEADER_LEN; i < ensemble.Length - DataSet.Ensemble.CHECKSUM_SIZE; i++)
                {
                    crc = (ushort)((byte)(crc >> 8) | (crc << 8));
                    crc ^= ensemble[i];
                    crc ^= (byte)((crc & 0xff) >> 4);
                    crc ^= (ushort)((crc << 8) << 4);
                    crc ^= (ushort)(((crc & 0xff) << 4) << 1);
                }

                return crc;
            }

            #endregion

            #region Add Byte Array Data

            /// <summary>
            /// Add a dataset to the end of an ensemble byte array.
            /// This will take the byte array of an ensemble and add
            /// another data type to the end.  It will then recalculate
            /// the payload size and checksum.
            /// </summary>
            /// <param name="ensemble">Ensemble data.</param>
            /// <param name="dataset">Dataset to add.</param>
            /// <returns>Ensemble with dataset added.</returns>
            public static byte[] AddDataSet(byte[] ensemble, byte[] dataset)
            {
                // Get the current payload size
                int payloadSize = 0;

                // Add 8 to header to take into account the ensemble number and 1's compliment
                int i = DataSet.Ensemble.HEADER_START_COUNT + 8;
                payloadSize = ensemble[i++];
                payloadSize += ensemble[i++] << 8;
                payloadSize += ensemble[i++] << 16;
                payloadSize += ensemble[i++] << 24;

                // Add the new dataset to the payload size
                payloadSize += dataset.Length;

                // Generate the 1's compliment of the payload size
                int payloadSizeNot = ~payloadSize;

                // Create a new array for the new dataset including new dataset
                byte[] result = new byte[ensemble.Length + dataset.Length];

                // Add the ensemble to the result
                // Subtract 4 to not include the checksum
                System.Buffer.BlockCopy(ensemble, 0, result, 0, ensemble.Length - BYTES_IN_INT32);

                // Replace the new payload and 1's compliment
                byte[] payloadSizeByte = MathHelper.Int32ToByteArray(payloadSize);
                byte[] payloadSizeNotByte = MathHelper.Int32ToByteArray(payloadSizeNot);
                System.Buffer.BlockCopy(payloadSizeByte, 0, result, (HEADER_START_COUNT + BYTES_IN_INT32 + BYTES_IN_INT32), BYTES_IN_INT32);
                System.Buffer.BlockCopy(payloadSizeNotByte, 0, result, (HEADER_START_COUNT + BYTES_IN_INT32 + BYTES_IN_INT32 + BYTES_IN_INT32), BYTES_IN_INT32);

                // Add the new dataset to the ensemble overlapping on checksum
                // Subtract 4 to not include the checksum
                System.Buffer.BlockCopy(dataset, 0, result, ensemble.Length - BYTES_IN_INT32, dataset.Length);

                // Create new checksum 
                long checksum = Ensemble.CalculateEnsembleChecksum(result);
                
                // Add checksum to the end of the datatype
                byte[] checksumByte = MathHelper.Int32ToByteArray((int)checksum);
                System.Buffer.BlockCopy(checksumByte, 0, result, result.Length - BYTES_IN_INT32, BYTES_IN_INT32);

                return result;
            }

            #endregion

            #region Encode

            /// <summary>
            /// Create a byte array of the ensemble.
            /// This will include the header, payload and
            /// checksum.  The payload will contain all the
            /// data.  The definition of the ensemble can be
            /// found in the RDI ADCP User Guide.
            /// </summary>
            /// <returns>Byte array of the ensemble.</returns>
            public byte[] Encode( )
            {
                // Get all the datasets as a byte array
                byte[] payload = GetAllDataSets();

                // Create a new array for the new dataset including new dataset
                byte[] result = new byte[ENSEMBLE_HEADER_LEN + payload.Length + CHECKSUM_SIZE];

                // Add the Header to the result array
                int ensembleNumber = GetEnsembleNumber();
                byte[] header = GenerateStubHeader(ensembleNumber);
                System.Buffer.BlockCopy(header, 0, result, 0, ENSEMBLE_HEADER_LEN);

                // Add the payload to the ensemble
                System.Buffer.BlockCopy(payload, 0, result, ENSEMBLE_HEADER_LEN, payload.Length);

                // Set the payload size and 1's compliment
                byte[] payloadSizeByte = MathHelper.Int32ToByteArray(payload.Length);
                byte[] payloadSizeNotByte = MathHelper.Int32ToByteArray(~payload.Length);
                System.Buffer.BlockCopy(payloadSizeByte, 0, result, (HEADER_START_COUNT + BYTES_IN_INT32 + BYTES_IN_INT32), BYTES_IN_INT32);
                System.Buffer.BlockCopy(payloadSizeNotByte, 0, result, (HEADER_START_COUNT + BYTES_IN_INT32 + BYTES_IN_INT32 + BYTES_IN_INT32), BYTES_IN_INT32);

                // Create new checksum 
                long checksum = Ensemble.CalculateEnsembleChecksum(result);

                // Add checksum to the end of the datatype
                byte[] checksumByte = MathHelper.Int32ToByteArray((int)checksum);
                System.Buffer.BlockCopy(checksumByte, 0, result, result.Length - BYTES_IN_INT32, BYTES_IN_INT32);

                return result;
            }

            /// <summary>
            /// This will give only the datasets in Matlab format.  To specify only 1 or more datasets, set all the IsXXXAvail
            /// methods to TRUE or FALSE.  Then call this method.
            /// This will not include the RTI header and checksum to the data.
            /// </summary>
            /// <returns>A byte array of all the datasets in the ensemble in Matlab format.</returns>
            public byte[] EncodeMatlab()
            {
                return GetAllDataSets();
            }

            /// <summary>
            /// Get the ensemble number from
            /// the Ensemble DataSet.  If the
            /// ensemble number is not available,
            /// use the ensemble number 0.
            /// </summary>
            /// <returns>Ensemble number from Ensemble DataSet or 0 if no ensemble number.</returns>
            private int GetEnsembleNumber()
            {
                if (IsEnsembleAvail)
                {
                    return EnsembleData.EnsembleNumber;
                }

                return 0;
            }

            /// <summary>
            /// Generate a stub header for the ensemble.
            /// This will include the 16 0x80 and the 
            /// ensemble number and its 1's compliment.  The
            /// Payload and its 1's compliment is left blank.
            /// </summary>
            /// <param name="ensembleNum">Ensemble number.</param>
            /// <returns>Byte array for the ensemble header.</returns>
            private byte[] GenerateStubHeader(int ensembleNum)
            {
                // Create an array that will contain:
                // 0x80 - 16 bytes
                // Ensemble Number - 4 bytes
                // ~Ensemble Number - 4 bytes
                // Payload size - 4 bytes
                // ~Payload size - 4 bytes
                // We will leave the payload and ~payload blank
                byte[] header = new byte[ENSEMBLE_HEADER_LEN];
                
                // Add 0x80
                for (int x = 0; x < HEADER_START_COUNT; x++)
                {
                    header[x] = 0x80;
                }

                // Add ensemble number
                byte[] ensNum = MathHelper.Int32ToByteArray(ensembleNum);
                System.Buffer.BlockCopy(ensNum, 0, header, HEADER_START_COUNT, BYTES_IN_INT32);

                byte[] notEnsNum = MathHelper.Int32ToByteArray(~ensembleNum);
                System.Buffer.BlockCopy(notEnsNum, 0, header, HEADER_START_COUNT + BYTES_IN_INT32, BYTES_IN_INT32);

                return header;
            }

            /// <summary>
            /// Get a list of all the datasets as byte arrays.
            /// Then combine them into 1 byte array and return the
            /// result.
            /// </summary>
            /// <returns>Byte array of all the datasets.</returns>
            private byte[] GetAllDataSets()
            {
                // Create a list of all the dataset byte arrays
                // Calculate the size
                int size = 0;
                List<byte[]> datasetList = new List<byte[]>();

                // Beam Velocity DataSet
                if (IsBeamVelocityAvail)
                {
                    byte[] beamDataSet = BeamVelocityData.Encode();
                    datasetList.Add(beamDataSet);
                    size += beamDataSet.Length;
                }

                // Earth Velocity DataSet
                if (IsEarthVelocityAvail)
                {
                    byte[] earthDataSet = EarthVelocityData.Encode();
                    datasetList.Add(earthDataSet);
                    size += earthDataSet.Length;
                }

                // Instrument Velocity DataSet
                if (IsInstrumentVelocityAvail)
                {
                    byte[] instrDataSet = InstrumentVelocityData.Encode();
                    datasetList.Add(instrDataSet);
                    size += instrDataSet.Length;
                }

                // Amplitude DataSet
                if (IsAmplitudeAvail)
                {
                    byte[] ampDataSet = AmplitudeData.Encode();
                    datasetList.Add(ampDataSet);
                    size += ampDataSet.Length;
                }

                // Correlation DataSet
                if (IsCorrelationAvail)
                {
                    byte[] corrDataSet = CorrelationData.Encode();
                    datasetList.Add(corrDataSet);
                    size += corrDataSet.Length;
                }

                // Good Beam Dataset
                if (IsGoodBeamAvail)
                {
                    byte[] goodBeamDataSet = GoodBeamData.Encode();
                    datasetList.Add(goodBeamDataSet);
                    size += goodBeamDataSet.Length;
                }

                // Good Earth DataSet
                if (IsGoodEarthAvail)
                {
                    byte[] goodEarthDataSet = GoodEarthData.Encode();
                    datasetList.Add(goodEarthDataSet);
                    size += goodEarthDataSet.Length;
                }

                // Ensemble DataSet
                if (IsEnsembleAvail)
                {
                    byte[] ensembleDataSet = EnsembleData.Encode();
                    datasetList.Add(ensembleDataSet);
                    size += ensembleDataSet.Length; 
                }

                // Ancillary DataSet
                if (IsAncillaryAvail)
                {
                    byte[] ancillaryDataSet = AncillaryData.Encode();
                    datasetList.Add(ancillaryDataSet);
                    size += ancillaryDataSet.Length; 
                }

                // Bottom Track DataSet
                if (IsBottomTrackAvail)
                {
                    byte[] btDataSet = BottomTrackData.Encode();
                    datasetList.Add(btDataSet);
                    size += btDataSet.Length; 
                }

                // NMEA dataset
                if (IsNmeaAvail)
                {
                    byte[] nmeaDataSet = NmeaData.Encode();
                    datasetList.Add(nmeaDataSet);
                    size += nmeaDataSet.Length;
                }

                return CombineDataSets(size, datasetList);
            }

            /// <summary>
            /// Combine the datasets into one byte array.
            /// This will go through the list combining 
            /// each dataset byte array into one large byte array.
            /// This will be the payload for an ensemble.
            /// </summary>
            /// <param name="size">Size of all the byte arrays combined.</param>
            /// <param name="datasetList">List of all the byte arrays for the ensemble.</param>
            /// <returns>Byte array of all the ensembles combined.</returns>
            private byte[] CombineDataSets(int size, List<byte[]> datasetList)
            {
                // If datasets exist, combine them and return
                // the result
                if (size > 0)
                {
                    // Create an array to hold all the datasets
                    byte[] result = new byte[size];

                    // Go throught the list combining the datasets
                    int index = 0;
                    for (int x = 0; x < datasetList.Count; x++)
                    {
                        System.Buffer.BlockCopy(datasetList[x], BEAM_0_INDEX, result, index, datasetList[x].Length);
                        index += datasetList[x].Length;
                    }

                    return result;
                }

                // If bad, return an empty byte array
                return new byte[1];
            }

            #endregion
        }


        /// <summary>
        /// Convert this object to a JSON object.
        /// Calling this method is twice as fast as calling the default serializer:
        /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble).
        /// 
        /// 420ms for this method.
        /// 900ms for calling SerializeObject default.
        /// 
        /// 1000 Ensembles
        /// Serialize: 440ms
        /// Deserialize: 1260 ms
        /// 
        /// Use this method whenever possible to convert to JSON.
        /// 
        /// http://james.newtonking.com/projects/json/help/
        /// http://james.newtonking.com/projects/json/help/index.html?topic=html/ReadingWritingJSON.htm
        /// http://blog.maskalik.com/asp-net/json-net-implement-custom-serialization
        /// </summary>
        public class EnsembleSerializer : JsonConverter
        {
            /// <summary>
            /// Write the JSON string.  This will convert all the properties to a JSON string.
            /// This is done manaully to improve conversion time.  The default serializer will check
            /// each property if it can convert.  This will convert the properties automatically.  This
            /// will double the speed.
            /// 
            /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble).
            /// 
            /// </summary>
            /// <param name="writer">JSON Writer.</param>
            /// <param name="value">Object to write to JSON.</param>
            /// <param name="serializer">Serializer to convert the object.</param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // Cast the object
                var ensemble = value as Ensemble;

                // Start the object
                writer.Formatting = Formatting.None;            // Make the text not indented, so not as human readable.  This will save disk space
                writer.WriteStartObject();                      // Start the JSON object

                #region Available

                // IsBeamVelocityAvail
                writer.WritePropertyName(DataSet.Ensemble.JSON_STR_ISBEAMVELOCITYAVAIL);
                writer.WriteValue(ensemble.IsBeamVelocityAvail);

                // IsInstrumentVelocityAvail
                writer.WritePropertyName(DataSet.Ensemble.JSON_STR_ISINSTRUMENTVELOCITYAVAIL);
                writer.WriteValue(ensemble.IsInstrumentVelocityAvail);

                // IsEarthVelocityAvail
                writer.WritePropertyName(DataSet.Ensemble.JSON_STR_ISEARTHVELOCITYAVAIL);
                writer.WriteValue(ensemble.IsEarthVelocityAvail);

                // IsAmplitudeAvail
                writer.WritePropertyName(DataSet.Ensemble.JSON_STR_ISAMPLITUDEAVAIL);
                writer.WriteValue(ensemble.IsAmplitudeAvail);

                // IsCorrelationAvail
                writer.WritePropertyName(DataSet.Ensemble.JSON_STR_ISCORRELATIONAVAIL);
                writer.WriteValue(ensemble.IsCorrelationAvail);

                // IsGoodBeamAvail
                writer.WritePropertyName(DataSet.Ensemble.JSON_STR_ISGOODBEAMAVAIL);
                writer.WriteValue(ensemble.IsGoodBeamAvail);

                // IsGoodEarthAvail
                writer.WritePropertyName(DataSet.Ensemble.JSON_STR_ISGOODEARTHAVAIL);
                writer.WriteValue(ensemble.IsGoodEarthAvail);

                // IsEnsembleAvail
                writer.WritePropertyName(DataSet.Ensemble.JSON_STR_ISENSEMBLEAVAIL);
                writer.WriteValue(ensemble.IsEnsembleAvail);

                // IsAncillaryAvail
                writer.WritePropertyName(DataSet.Ensemble.JSON_STR_ISANCILLARYAVAIL);
                writer.WriteValue(ensemble.IsAncillaryAvail);

                // IsBottomTrackAvail
                writer.WritePropertyName(DataSet.Ensemble.JSON_STR_ISBOTTOMTRACKAVAIL);
                writer.WriteValue(ensemble.IsBottomTrackAvail);

                // IsEarthWaterMassAvail
                writer.WritePropertyName(DataSet.Ensemble.JSON_STR_ISEARTHWATERMASSAVAIL);
                writer.WriteValue(ensemble.IsEarthWaterMassAvail);

                // IsInstrumentWaterMassAvail
                writer.WritePropertyName(DataSet.Ensemble.JSON_STR_ISINSTRUMENTWATERMASSAVAIL);
                writer.WriteValue(ensemble.IsInstrumentWaterMassAvail);

                // IsNmeaAvail
                writer.WritePropertyName(DataSet.Ensemble.JSON_STR_ISNMEAAVAIL);
                writer.WriteValue(ensemble.IsNmeaAvail);

                #endregion

                #region DataSet

                // BeamVelocityData
                writer.WritePropertyName(DataSet.Ensemble.JSON_STR_BEAMVELOCITYDATA);
                if (ensemble.IsBeamVelocityAvail)
                {
                    writer.WriteRawValue(Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.BeamVelocityData));
                }
                else
                {
                    writer.WriteNull();
                }

                // InstrumentVelocityData
                writer.WritePropertyName(DataSet.Ensemble.JSON_STR_INSTRUMENTVELOCITYDATA);
                if (ensemble.IsInstrumentVelocityAvail)
                {
                    writer.WriteRawValue(Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.InstrumentVelocityData));
                }
                else
                {
                    writer.WriteNull();
                }

                // EarthVelocityData
                writer.WritePropertyName(DataSet.Ensemble.JSON_STR_EARTHVELOCITYDATA);
                if (ensemble.IsEarthVelocityAvail)
                {
                    writer.WriteRawValue(Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.EarthVelocityData));
                }
                else
                {
                    writer.WriteNull();
                }

                // AmplitudeData
                writer.WritePropertyName(DataSet.Ensemble.JSON_STR_AMPLITUDEDATA);
                if (ensemble.IsAmplitudeAvail)
                {
                    writer.WriteRawValue(Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.AmplitudeData));
                }
                else
                {
                    writer.WriteNull();
                }

                // CorrelationData
                writer.WritePropertyName(DataSet.Ensemble.JSON_STR_CORRELATIONDATA);
                if (ensemble.IsCorrelationAvail)
                {
                    writer.WriteRawValue(Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.CorrelationData));
                }
                else
                {
                    writer.WriteNull();
                }

                // GoodBeamData
                writer.WritePropertyName(DataSet.Ensemble.JSON_STR_GOODBEAMDATA);
                if (ensemble.IsGoodBeamAvail)
                {
                    writer.WriteRawValue(Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.GoodBeamData));
                }
                else
                {
                    writer.WriteNull();
                }

                // GoodEarthData
                writer.WritePropertyName(DataSet.Ensemble.JSON_STR_GOODEARTHDATA);
                if (ensemble.IsGoodEarthAvail)
                {
                    writer.WriteRawValue(Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.GoodEarthData));
                }
                else
                {
                    writer.WriteNull();
                }

                // EnsembleData
                writer.WritePropertyName(DataSet.Ensemble.JSON_STR_ENSEMBLEDATA);
                if (ensemble.IsEnsembleAvail)
                {
                    writer.WriteRawValue(Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.EnsembleData));
                }
                else
                {
                    writer.WriteNull();
                }

                // AncillaryData
                writer.WritePropertyName(DataSet.Ensemble.JSON_STR_ANCILLARYDATA);
                if (ensemble.IsAncillaryAvail)
                {
                    writer.WriteRawValue(Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.AncillaryData));
                }
                else
                {
                    writer.WriteNull();
                }

                // BottomTrackData
                writer.WritePropertyName(DataSet.Ensemble.JSON_STR_BOTTOMTRACKDATA);
                if (ensemble.IsBottomTrackAvail)
                {
                    writer.WriteRawValue(Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.BottomTrackData));
                }
                else
                {
                    writer.WriteNull();
                }

                // EarthWaterMassData
                writer.WritePropertyName(DataSet.Ensemble.JSON_STR_EARTHWATERMASSDATA);
                if (ensemble.IsEarthWaterMassAvail)
                {
                    writer.WriteRawValue(Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.EarthWaterMassData));
                }
                else
                {
                    writer.WriteNull();
                }

                // InstrumentWaterMassData
                writer.WritePropertyName(DataSet.Ensemble.JSON_STR_INSTRUMENTWATERMASSDATA);
                if (ensemble.IsInstrumentWaterMassAvail)
                {
                    writer.WriteRawValue(Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.InstrumentWaterMassData));
                }
                else
                {
                    writer.WriteNull();
                }

                // NmeaData
                writer.WritePropertyName(DataSet.Ensemble.JSON_STR_NMEADATA);
                if (ensemble.IsNmeaAvail)
                {
                    writer.WriteRawValue(Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.NmeaData));
                }
                else
                {
                    writer.WriteNull();
                }

                #endregion

                // End the object
                writer.WriteEndObject();
            }

            /// <summary>
            /// Read the JSON object and convert to the object.  This will allow the serializer to
            /// automatically convert the object.  No special instructions need to be done and all
            /// the properties found in the JSON string need to be used.
            /// 
            /// Newtonsoft.Json.JsonConvert.DeserializeObject[DataSet.Ensemble](ensembleJsonStr).
            /// 
            /// </summary>
            /// <param name="reader">NOT USED. JSON reader.</param>
            /// <param name="objectType">NOT USED> Type of object.</param>
            /// <param name="existingValue">NOT USED.</param>
            /// <param name="serializer">Serialize the object.</param>
            /// <returns>Serialized object.</returns>
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var ensemble = new Ensemble();
                serializer.Populate(reader, ensemble);
                return ensemble;
            }

            /// <summary>
            /// Check if the given object is the correct type.
            /// </summary>
            /// <param name="objectType">Object to convert.</param>
            /// <returns>TRUE = object given is the correct type.</returns>
            public override bool CanConvert(Type objectType)
            {
                return typeof(Ensemble).IsAssignableFrom(objectType);
            }
        }

    }
}