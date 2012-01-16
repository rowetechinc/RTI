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
 *       
 * 
 */

using System.Data;

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
        public class Ensemble
        {
            /// <summary>
            /// Beam Velocity ID for binary format.
            /// </summary>
            public const string VelocityID = "E000001\0";

            /// <summary>
            /// Instrument Velocity ID for binary format.
            /// </summary>
            public const string InstrumentID = "E000002\0";

            /// <summary>
            /// Earth Velocity ID for binary format.
            /// </summary>
            public const string EarthID = "E000003\0";

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
            public const string NMEAID = "E000011\0";

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

            /// <summary>
            /// Maximum number of data sets to look for.
            /// </summary>
            public const int MAX_NUM_DATA_SETS = 20;

            /// <summary>
            /// Number of bytes in the Data Set Header.
            /// </summary>
            public const int DATASET_HEADER_LEN = 32;
            

            /// <summary>
            /// Number of bytes per byte.
            /// </summary>
            public const int BYTES_IN_BYTES = 1;

            /// <summary>
            /// Number of bytes per float.
            /// </summary>
            public const int BYTES_IN_FLOAT = 4;

            /// <summary>
            /// Number of bytes per integer.
            /// </summary>
            public const int BYTES_IN_INT = 4;

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
            public const int MAX_HEADER_COUNT = 16;

            /// <summary>
            /// Each payload contains a header with DataType, 
            /// Bins or Elements, Beams or 1, Image, ID (Name) 
            /// Length and ID (name).
            /// </summary>
            public const int PAYLOAD_HEADER_LEN = 28;

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
            
            /// <summary>
            /// Set if the Beam velocity data set is available for this data set
            /// </summary>
            public bool IsBeamVelocityAvail { get; private set; }

            /// <summary>
            /// Set if the InstrumentVelocity velocity data set is available for this data set
            /// </summary>
            public bool IsInstrVelocityAvail { get; private set; }

            /// <summary>
            /// Set if the EarthVelocity velocity data set is available for this data set
            /// </summary>
            public bool IsEarthVelocityAvail { get; private set; }

            /// <summary>
            /// Set if the Amplitude data set is available for this data set
            /// </summary>
            public bool IsAmplitudeAvail { get; private set; }

            /// <summary>
            /// Set if the Correlation data set is available for this data set
            /// </summary>
            public bool IsCorrelationAvail { get; private set; }

            /// <summary>
            /// Set if the Good Beam data set is available for this data set
            /// </summary>
            public bool IsGoodBeamAvail { get; private set; }

            /// <summary>
            /// Set if the Good EarthVelocity data set is available for this data set
            /// </summary>
            public bool IsGoodEarthAvail { get; private set; }

            /// <summary>
            /// Set if the Ensemble data set is available for this data set
            /// </summary>
            public bool IsEnsembleAvail { get; private set; }

            /// <summary>
            /// Set if the Ancillary data set is available for this data set
            /// </summary>
            public bool IsAncillaryAvail { get; private set; }

            /// <summary>
            /// Set if the Bottom Track data set is available for this data set
            /// </summary>
            public bool IsBottomTrackAvail { get; private set; }

            /// <summary>
            /// Set if the Earth Water Mass data set is available for this data set
            /// </summary>
            public bool IsEarthWaterMassAvail { get; private set; }

            /// <summary>
            /// Set if the Insturment Water Mass data set is available for this data set
            /// </summary>
            public bool IsInstrumentWaterMassAvail { get; private set; }

            /// <summary>
            /// Set if the NMEA data set is available for this data set
            /// </summary>
            public bool IsNmeaAvail { get; private set; }

            /// <summary>
            /// Beam BeamVelocity Data set for this data set.
            /// </summary>
            public BeamVelocityDataSet BeamVelocityData { get; private set; }

            /// <summary>
            /// InstrumentVelocity BeamVelocity Data set for this data set.
            /// </summary>
            public InstrVelocityDataSet InstrVelocityData { get; private set; }

            /// <summary>
            /// EarthVelocity BeamVelocity Data set for this data set.
            /// </summary>
            public EarthVelocityDataSet EarthVelocityData { get; private set; }

            /// <summary>
            /// Amplitude Data set for this data set.
            /// </summary>
            public AmplitudeDataSet AmplitudeData { get; private set; }

            /// <summary>
            /// Correlation Data set for this data set.
            /// </summary>
            public CorrelationDataSet CorrelationData { get; private set; }

            /// <summary>
            /// Good Beam Data set for this data set.
            /// </summary>
            public GoodBeamDataSet GoodBeamData { get; private set; }

            /// <summary>
            /// Good EarthVelocity Data set for this data set.
            /// </summary>
            public GoodEarthDataSet GoodEarthData { get; private set; }

            /// <summary>
            /// Ensemble Data set for this data set.
            /// </summary>
            public EnsembleDataSet EnsembleData { get; private set; }

            /// <summary>
            /// Ancillary Data set for this data set.
            /// </summary>
            public AncillaryDataSet AncillaryData { get; private set; }

            /// <summary>
            /// Bottom Track Data set for this data set.
            /// </summary>
            public BottomTrackDataSet BottomTrackData { get; private set; }

            /// <summary>
            /// Water Mass Earth Velocity Data set for this data set.
            /// </summary>
            public EarthWaterMassDataSet EarthWaterMassData { get; private set; }

            /// <summary>
            /// Water Mass Instrument Velocity Data set for this data set.
            /// </summary>
            public InstrumentWaterMassDataSet InstrumentWaterMassData { get; private set; }

            /// <summary>
            /// NMEA Data set for this data set.
            /// </summary>
            public NmeaDataSet NmeaData { get; private set; }

            /// <summary>
            /// Constructor
            /// 
            /// Initialize all ranges.
            /// </summary>
            public Ensemble()
            {
                // Initialize all ranges
                IsBeamVelocityAvail = false;
                IsInstrVelocityAvail = false;
                IsEarthVelocityAvail = false;
                IsAmplitudeAvail = false;
                IsCorrelationAvail = false;
                IsGoodBeamAvail = false;
                IsGoodEarthAvail = false;
                IsEnsembleAvail = false;
                IsAncillaryAvail = false;
                IsBottomTrackAvail = false;
                IsNmeaAvail = false;
            }

            /// <summary>
            /// Add the Beam Velocity data set to the data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="orientation">Orientation of the beam. Default down.</param>
            public void AddBeamVelocityData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, DataSet.BaseDataSet.BeamOrientation orientation = DataSet.BaseDataSet.BeamOrientation.DOWN)
            {
                IsBeamVelocityAvail = true;
                BeamVelocityData = new BeamVelocityDataSet(valueType, numBins, numBeams, imag, nameLength, name, orientation);
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
            /// <param name="orientation">Orientation of the beam. Default down.</param>
            public void AddBeamVelocityData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] velocityData, DataSet.BaseDataSet.BeamOrientation orientation = DataSet.BaseDataSet.BeamOrientation.DOWN)
            {
                IsBeamVelocityAvail = true;
                BeamVelocityData = new BeamVelocityDataSet(valueType, numBins, numBeams, imag, nameLength, name, velocityData, orientation);
            }

            /// <summary>
            /// Add the Beam Velocity data set to the data.
            /// This will add the Beam velocity data and decode the DataTable
            /// for all the Beam velocity data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="velocityData">DataTable containing Beam velocity data</param>
            /// <param name="orientation">Orientation of the beam. Default down.</param>
            public void AddBeamVelocityData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, DataTable velocityData, DataSet.BaseDataSet.BeamOrientation orientation = DataSet.BaseDataSet.BeamOrientation.DOWN)
            {
                IsBeamVelocityAvail = true;
                BeamVelocityData = new BeamVelocityDataSet(valueType, numBins, numBeams, imag, nameLength, name, velocityData, orientation);
            }

            /// <summary>
            /// Add the Instrument Velocity data set to the data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="orientation">Orientation of the beam. Default down.</param>
            public void AddInstrVelocityData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, DataSet.BaseDataSet.BeamOrientation orientation = DataSet.BaseDataSet.BeamOrientation.DOWN)
            {
                IsInstrVelocityAvail = true;
                InstrVelocityData = new InstrVelocityDataSet(valueType, numBins, numBeams, imag, nameLength, name, orientation);
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
            /// <param name="orientation">Orientation of the beam. Default down.</param>
            public void AddInstrVelocityData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] velocityData, DataSet.BaseDataSet.BeamOrientation orientation = DataSet.BaseDataSet.BeamOrientation.DOWN)
            {
                IsInstrVelocityAvail = true;
                InstrVelocityData = new InstrVelocityDataSet(valueType, numBins, numBeams, imag, nameLength, name, velocityData, orientation);
            }

            /// <summary>
            /// Add the Instrument Velocity data set to the data.
            /// This will add the Instrument Velocity data and decode the DataTable
            /// for all the Instrument Velocity data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="velocityData">DataTable containing InstrumentVelocity velocity data</param>
            /// <param name="orientation">Orientation of the beam. Default down.</param>
            public void AddInstrVelocityData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, DataTable velocityData, DataSet.BaseDataSet.BeamOrientation orientation = DataSet.BaseDataSet.BeamOrientation.DOWN)
            {
                IsInstrVelocityAvail = true;
                InstrVelocityData = new InstrVelocityDataSet(valueType, numBins, numBeams, imag, nameLength, name, velocityData, orientation);
            }

            /// <summary>
            /// Add the Earth Velocity data set to the data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="orientation">Orientation of the beam. Default down.</param>
            public void AddEarthVelocityData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, DataSet.BaseDataSet.BeamOrientation orientation = DataSet.BaseDataSet.BeamOrientation.DOWN)
            {
                IsEarthVelocityAvail = true;
                EarthVelocityData = new EarthVelocityDataSet(valueType, numBins, numBeams, imag, nameLength, name, orientation);
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
            /// <param name="orientation">Orientation of the beam. Default down.</param>
            public void AddEarthVelocityData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] velocityData, DataSet.BaseDataSet.BeamOrientation orientation = DataSet.BaseDataSet.BeamOrientation.DOWN)
            {
                IsEarthVelocityAvail = true;
                EarthVelocityData = new EarthVelocityDataSet(valueType, numBins, numBeams, imag, nameLength, name, velocityData, orientation);
            }

            /// <summary>
            /// Add the Earth Velocity data set to the data.
            /// This will add the Earth Velocity data and decode the DataTable
            /// for all the Earth Velocity data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="velocityData">DataTable containing EarthVelocity velocity data</param>
            /// <param name="orientation">Orientation of the beam. Default down.</param>
            public void AddEarthVelocityData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, DataTable velocityData, DataSet.BaseDataSet.BeamOrientation orientation = DataSet.BaseDataSet.BeamOrientation.DOWN)
            {
                IsEarthVelocityAvail = true;
                EarthVelocityData = new EarthVelocityDataSet(valueType, numBins, numBeams, imag, nameLength, name, velocityData, orientation);
            }

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

            /// <summary>
            /// Add the Amplitude data set to the data.
            /// This will add the Amplitude data and decode the DataTable
            /// for all the Amplitude data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="amplitudeData">DataTable containing Amplitude data</param>
            public void AddAmplitudeData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, DataTable amplitudeData)
            {
                IsAmplitudeAvail = true;
                AmplitudeData = new AmplitudeDataSet(valueType, numBins, numBeams, imag, nameLength, name, amplitudeData);
            }

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

            /// <summary>
            /// Add the Correlation data set to the data.
            /// This will add the Correlation data and decode the DataTable
            /// for all the Correlation data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="correlationData">DataTable containing Correlation data</param>
            public void AddCorrelationData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, DataTable correlationData)
            {
                IsCorrelationAvail = true;
                CorrelationData = new CorrelationDataSet(valueType, numBins, numBeams, imag, nameLength, name, correlationData);
            }

            /// <summary>
            /// Add the Good Beam data set to the data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="orientation">Orientation of the beam. Default down.</param>
            public void AddGoodBeamData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, DataSet.BaseDataSet.BeamOrientation orientation = DataSet.BaseDataSet.BeamOrientation.DOWN)
            {
                IsGoodBeamAvail = true;
                GoodBeamData = new GoodBeamDataSet(valueType, numBins, numBeams, imag, nameLength, name, orientation);
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
            /// <param name="orientation">Orientation of the beam. Default down.</param>
            public void AddGoodBeamData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] goodBeamData, DataSet.BaseDataSet.BeamOrientation orientation = DataSet.BaseDataSet.BeamOrientation.DOWN)
            {
                IsGoodBeamAvail = true;
                GoodBeamData = new GoodBeamDataSet(valueType, numBins, numBeams, imag, nameLength, name, goodBeamData, orientation);
            }

            /// <summary>
            /// Add the Good Beam data set to the data.
            /// This will add the Good Beam data and decode the DataTable
            /// for all the Good Beam data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="goodBeamData">DataTable containing Good Beam data</param>
            /// <param name="orientation">Orientation of the beam. Default down.</param>
            public void AddGoodBeamData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, DataTable goodBeamData, DataSet.BaseDataSet.BeamOrientation orientation = DataSet.BaseDataSet.BeamOrientation.DOWN)
            {
                IsGoodBeamAvail = true;
                GoodBeamData = new GoodBeamDataSet(valueType, numBins, numBeams, imag, nameLength, name, goodBeamData, orientation);
            }

            /// <summary>
            /// Add the Good Earth data set to the data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="orientation">Orientation of the beam. Default down.</param>
            public void AddGoodEarthData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, DataSet.BaseDataSet.BeamOrientation orientation = DataSet.BaseDataSet.BeamOrientation.DOWN)
            {
                IsGoodEarthAvail = true;
                GoodEarthData = new GoodEarthDataSet(valueType, numBins, numBeams, imag, nameLength, name,orientation);
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
            /// <param name="orientation">Orientation of the beam. Default down.</param>
            public void AddGoodEarthData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] goodEarthData, DataSet.BaseDataSet.BeamOrientation orientation = DataSet.BaseDataSet.BeamOrientation.DOWN)
            {
                IsGoodEarthAvail = true;
                GoodEarthData = new GoodEarthDataSet(valueType, numBins, numBeams, imag, nameLength, name, goodEarthData, orientation);
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
            /// <param name="goodEarthData">DataTable containing Good EarthVelocity data</param>
            /// <param name="orientation">Orientation of the beam. Default down.</param>
            public void AddGoodEarthData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, DataTable goodEarthData, DataSet.BaseDataSet.BeamOrientation orientation = DataSet.BaseDataSet.BeamOrientation.DOWN)
            {
                IsGoodEarthAvail = true;
                GoodEarthData = new GoodEarthDataSet(valueType, numBins, numBeams, imag, nameLength, name, goodEarthData, orientation);
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
            /// Add the Ensemble data set to the data.
            /// This will add the Ensemble data and decode the DataTable
            /// for all the Ensemble data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="ensembleData">DataTable containing Ensemble data</param>
            public void AddEnsembleData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, DataRow ensembleData)
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
            /// Add the Ancillary data set to the data.
            /// This will add the Ancillary data and decode the DataTable
            /// for all the Ancillary data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="ancillaryData">DataTable containing Ancillary data</param>
            public void AddAncillaryData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, DataRow ancillaryData)
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
            /// This will add the Bottom Track data and decode the DataTable
            /// for all the Bottom Track data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="bottomTrackData">DataTable containing Bottom Track data</param>
            public void AddBottomTrackData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, DataRow bottomTrackData)
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
            /// <param name="orientation">Orientation of the beam. Default down.</param>
            public void AddEarthWaterMassData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, float east, float north, float vertical, float depthLayer, DataSet.BaseDataSet.BeamOrientation orientation = DataSet.BaseDataSet.BeamOrientation.DOWN)
            {
                IsEarthWaterMassAvail = true;
                EarthWaterMassData = new EarthWaterMassDataSet(valueType, numBins, numBeams, imag, nameLength, name, east, north, vertical, depthLayer, orientation);
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
            /// <param name="orientation">Orientation of the beam. Default down.</param>
            public void AddInstrumentWaterMassData(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, float x, float y, float z, float depthLayer, DataSet.BaseDataSet.BeamOrientation orientation = DataSet.BaseDataSet.BeamOrientation.DOWN)
            {
                IsInstrumentWaterMassAvail = true;
                InstrumentWaterMassData = new InstrumentWaterMassDataSet(valueType, numBins, numBeams, imag, nameLength, name, x, y, z, depthLayer, orientation);
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
                if (IsInstrVelocityAvail)
                {
                    s += InstrVelocityData.ToString();
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
            /// Get the playload size from the ensemble.
            /// Then add in the header size and the checksum
            /// size to get the overall size of the ensemble.
            /// </summary>
            /// <param name="payloadSize">Size of the payload find in the ensemble header.</param>
            /// <returns>Total size of the ensemble including header, payload and checksum.</returns>
            public static int CalculateEnsembleSize(int payloadSize)
            {
                return payloadSize + DataSet.Ensemble.DATASET_HEADER_LEN + DataSet.Ensemble.CHECKSUM_SIZE;
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
                for (int i = DataSet.Ensemble.DATASET_HEADER_LEN; i < ensemble.Length - DataSet.Ensemble.CHECKSUM_SIZE; i++)
                {
                    crc = (ushort)((byte)(crc >> 8) | (crc << 8));
                    crc ^= ensemble[i];
                    crc ^= (byte)((crc & 0xff) >> 4);
                    crc ^= (ushort)((crc << 8) << 4);
                    crc ^= (ushort)(((crc & 0xff) << 4) << 1);
                }

                return crc;
            }

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
                int i = DataSet.Ensemble.MAX_HEADER_COUNT + 8;
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
                System.Buffer.BlockCopy(ensemble, 0, result, 0, ensemble.Length - 4);

                // Replace the new payload and 1's compliment
                byte[] payloadSizeByte = Converters.IntToByteArray(payloadSize);
                byte[] payloadSizeNotByte = Converters.IntToByteArray(payloadSizeNot);
                System.Buffer.BlockCopy(payloadSizeByte, 0, result, DataSet.Ensemble.MAX_HEADER_COUNT + 8, 4);
                System.Buffer.BlockCopy(payloadSizeNotByte, 0, result, DataSet.Ensemble.MAX_HEADER_COUNT + 8 + 4, 4);

                // Add the new dataset to the ensemble overlapping on checksum
                // Subtract 4 to not include the checksum
                System.Buffer.BlockCopy(dataset, 0, result, ensemble.Length - 4, dataset.Length);

                // Create new checksum 
                long checksum = Ensemble.CalculateEnsembleChecksum(result);
                
                // Add checksum to the end of the datatype
                byte[] checksumByte = Converters.IntToByteArray((int)checksum);
                System.Buffer.BlockCopy(checksumByte, 0, result, result.Length - 4, 4);

                return result;
            }
        }
    }
}