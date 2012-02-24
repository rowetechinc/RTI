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
 * Date            Initials    Comments
 * -----------------------------------------------------------------
 * 09/30/2011      RC                     Initial coding
 * 10/04/2011      RC                     Added decoding a DataTable
 * 10/05/2011      RC                     Round ranges to display
 * 11/01/2011      RC                     Added method to return average Range.
 * 11/02/2011      RC                     Added method to return Speed Magnitude.
 *                                         Removed binding list for data needed for datagrids.
 * 11/03/2011      RC                     Added methods to get Magnitude, direction and XY points.
 * 11/07/2011      RC                     Check for format exception when decoding bottom track data.
 * 11/28/2011      RC                     Decode Prti01Sentence for bottom track data.
 * 11/29/2011      RC                     Added setting water temp for Prti01Sentence.
 *                                         Decode Prti02Sentence for Bottom Track data.
 *                                         Add Prti02Sentence data to existing data using AddAdditionalBottomTrackData().
 * 12/15/2011      RC          1.09       Return 0 instead of -1 for bad velocities in GetVelocityMagnitude() 
 * 01/04/2012      RC          1.11       Added a method to check if the Earth Velocity is good.
 * 01/13/2012      RC          1.12       Merged Ensemble table and Bottom Track table in database.
 * 01/18/2012      RC          1.14       Added Encode() to convert to byte array.
 *                                         Changed Beams to NumBeams.
 *                                         Removed "private set".
 * 02/23/2012      RC          2.03       Changed Status to a Status object.
 * 
 */

using System;
using System.Data;

namespace RTI
{
    namespace DataSet
    {
        /// <summary>
        /// Data set containing all the Ancillary data.
        /// </summary>
        public class BottomTrackDataSet : BaseDataSet
        {
            /// <summary>
            /// Number of elements in this data set.
            /// </summary>
            public const int NUM_DATA_ELEMENTS = 54;

            /// <summary>
            /// Water Track 3 Beam solution STATUS value (DVL only).
            /// </summary>
            public const int BT_WT_3BEAM_SOLUTION = 0x0001;

            /// <summary>
            /// Bottom Track 3 Beam solution STATUS value.
            /// </summary>
            public const int BT_BT_3BEAM_SOLUTION = 0x0002;

            /// <summary>
            /// Bottom Track Hold STATUS value.
            /// </summary>
            public const int BT_HOLD = 0x0004;

            /// <summary>
            /// Bottom Track Searching STATUS value.
            /// </summary>
            public const int BT_SEARCHING = 0x0008;

            /// <summary>
            /// Bottom Track Hardware timeout STATUS value.
            /// </summary>
            public const int BT_HDWR_TIMEOUT = 0x8000;

            /// <summary>
            /// First Bottom Track Ping Time in seconds.
            /// </summary>
            public float FirstPingTime { get; set; }

            /// <summary>
            /// Round version of First Bottom Track Ping time in seconds.
            /// </summary>
            public string FirstPingTimeRounded { get { return FirstPingTime.ToString("0.00"); } }

            /// <summary>
            /// Last Bottom Track Ping Time in seconds.
            /// </summary>
            public float LastPingTime { get; set; }

            /// <summary>
            /// Round version of Last Ping Time in seconds.
            /// </summary>
            public string LastPingRounded { get { return LastPingTime.ToString("0.00"); } }

            /// <summary>
            /// Heading in degrees.
            /// </summary>
            public float Heading { get; set; }

            /// <summary>
            /// Round version of Heading in degrees.
            /// </summary>
            public string HeadingRounded { get { return Heading.ToString("0.000"); } }

            /// <summary>
            /// Pitch in degrees.
            /// </summary>
            public float Pitch { get; set; }

            /// <summary>
            /// Round version of Pitch in degrees. 
            /// </summary>
            public string PitchRounded { get { return Pitch.ToString("0.000"); } }

            /// <summary>
            /// Roll in degrees.
            /// </summary>
            public float Roll { get; set; }

            /// <summary>
            /// Round version of Roll in degrees. 
            /// </summary>
            public string RollRounded { get { return Roll.ToString("0.000"); } }

            /// <summary>
            /// Water Temperature in degrees.  Used in Speed of Sound.
            /// </summary>
            public float WaterTemp { get; set; }

            /// <summary>
            /// Round version of Water Temperature in degrees.
            /// </summary>
            public string WaterTempRounded { get { return WaterTemp.ToString("0.000"); } }

            /// <summary>
            /// System (board) temperature in degrees.
            /// </summary>
            public float SystemTemp { get; set; }

            /// <summary>
            /// Round version of System Temperature in degrees.
            /// </summary>
            public string SystemTempRounded { get { return SystemTemp.ToString("0.000"); } }

            /// <summary>
            /// Salinity in parts per thousand (ppt).
            /// Used in Speed of Sound.
            /// </summary>
            public float Salinity { get; set; }

            /// <summary>
            /// Round version of Salinity in parts per thousand.
            /// </summary>
            public string SalinityRounded { get { return Salinity.ToString("0.000"); } }

            /// <summary>
            /// Pressure in Pascal.
            /// </summary>
            public float Pressure { get; set; }

            /// <summary>
            /// Round version of Pressure in pascal.
            /// </summary>
            public string PressureRounded { get { return Pressure.ToString("0.000"); } }

            /// <summary>
            /// Transducer Depth in meters.
            /// Used in Speed of Sound.
            /// </summary>
            public float TransducerDepth { get; set; }

            /// <summary>
            /// Round version of Tranducer Depth in meters.
            /// </summary>
            public string TransducerDepthRounded { get { return TransducerDepth.ToString("0.000"); } }

            /// <summary>
            /// Speed of Sound in meters/sec.
            /// </summary>
            public float SpeedOfSound { get; set; }

            /// <summary>
            /// Round version of Speed Of Sound in meter/sec.
            /// </summary>
            public string SpeedOfSoundRounded { get { return SpeedOfSound.ToString("0.000"); } }

            /// <summary>
            /// Status of the Bottom Track.
            /// </summary>
            public Status Status { get; set; }

            /// <summary>
            /// Number of beams.
            /// </summary>
            public float NumBeams { get; set; }

            /// <summary>
            /// Number of pings in the sample.
            /// </summary>
            public float ActualPingCount { get; set; }

            /// <summary>
            /// Round version of actual number of pings in samples.
            /// </summary>
            public double ActualPingCountRounded { get { return Math.Round(ActualPingCount, 3); } }

            /// <summary>
            /// Bottom Track Range ranges for each beam in meters.
            /// </summary>
            public float[] Range { get; set; }

            /// <summary>
            /// Bottom Track Signal to Noise ratio in counts.
            /// </summary>
            public float[] SNR { get; set; }

            /// <summary>
            /// Bottom Track Amplitude ranges in counts.
            /// </summary>
            public float[] Amplitude { get; set; }

            /// <summary>
            /// Bottom Track Correlation.  0.5 = 50%
            /// </summary>
            public float[] Correlation { get; set; }

            /// <summary>
            /// Bottom Track velocity in Beam form in meters/second.
            /// </summary>
            public float[] BeamVelocity { get; set; }

            /// <summary>
            /// Number of pings averaged in Beam form.
            /// </summary>
            public float[] BeamGood { get; set; }

            /// <summary>
            /// Bottom Track velocity in InstrumentVelocity form in meters/second.
            /// </summary>
            public float[] InstrumentVelocity { get; set; }

            /// <summary>
            /// Number of pings averaged together in InstrumentVelocity form.
            /// </summary>
            public float[] InstrumentGood { get; set; }

            /// <summary>
            /// Bottom Track velocity in EarthVelocity form in meters/second.
            /// </summary>
            public float[] EarthVelocity { get; set; }

            /// <summary>
            /// Number of pings averaged together in EarthVelocity form.
            /// </summary>
            public float[] EarthGood { get; set; }


            /// <summary>
            /// Create a Bottom Track data set.  Includes all the information
            /// about the current Bottom Track data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="ancData">Byte array containing Bottom Track data</param>
            public BottomTrackDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] ancData) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize arrays
                Range = new float[4];
                SNR = new float[4];
                Amplitude = new float[4];
                Correlation = new float[4];
                BeamVelocity = new float[4];
                BeamGood = new float[4];
                InstrumentVelocity = new float[4];
                InstrumentGood = new float[4];
                EarthVelocity = new float[4];
                EarthGood = new float[4];

                // Decode the information
                Decode(ancData);
            }

            /// <summary>
            /// Create a Bottom Track data set.  Includes all the information
            /// about the current Bottom Track data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="btData">DataRow containing Bottom Track data</param>
            public BottomTrackDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, DataRow btData) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize arrays
                Range = new float[4];
                SNR = new float[4];
                Amplitude = new float[4];
                Correlation = new float[4];
                BeamVelocity = new float[4];
                BeamGood = new float[4];
                InstrumentVelocity = new float[4];
                InstrumentGood = new float[4];
                EarthVelocity = new float[4];
                EarthGood = new float[4];

                // Decode the information
                Decode(btData);
            }

            /// <summary>
            /// Create a Bottom Track data set.  Includes all the information
            /// about the current Bottom Track data.
            /// </summary>
            /// <param name="sentence"></param>
            public BottomTrackDataSet(Prti01Sentence sentence) :
                base(DataSet.Ensemble.DATATYPE_FLOAT, NUM_DATA_ELEMENTS, DataSet.Ensemble.DEFAULT_NUM_BEAMS_NONBEAM, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.BottomTrackID)
            {
                // Initialize arrays
                Range = new float[4];
                SNR = new float[4];
                Amplitude = new float[4];
                Correlation = new float[4];
                BeamVelocity = new float[4];
                BeamGood = new float[4];
                InstrumentVelocity = new float[4];
                InstrumentGood = new float[4];
                EarthVelocity = new float[4];
                EarthGood = new float[4];

                // Decode the information
                DecodeBottomTrackData(sentence);
            }

            /// <summary>
            /// Create a Bottom Track data set.  Includes all the information
            /// about the current Bottom Track data.
            /// </summary>
            /// <param name="sentence"></param>
            public BottomTrackDataSet(Prti02Sentence sentence) :
                base(DataSet.Ensemble.DATATYPE_FLOAT, NUM_DATA_ELEMENTS, DataSet.Ensemble.DEFAULT_NUM_BEAMS_NONBEAM, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.BottomTrackID)
            {
                // Initialize arrays
                Range = new float[4];
                SNR = new float[4];
                Amplitude = new float[4];
                Correlation = new float[4];
                BeamVelocity = new float[4];
                BeamGood = new float[4];
                InstrumentVelocity = new float[4];
                InstrumentGood = new float[4];
                EarthVelocity = new float[4];
                EarthGood = new float[4];

                // Decode the information
                DecodeBottomTrackData(sentence);
            }

            /// <summary>
            /// This will add additional Bottom Track data from the
            /// Prti02Sentence.
            /// </summary>
            /// <param name="sentence">Additional data to add to the dataset.</param>
            public void AddAdditionalBottomTrackData(Prti02Sentence sentence)
            {
                // Decode the information
                DecodeBottomTrackData(sentence);
            }

            /// <summary>
            /// Get all the information about the Bottom Track data.
            /// </summary>
            /// <param name="data">Byte array containing the Bottom Track data type.</param>
            private void Decode(byte[] data)
            {
                FirstPingTime = Converters.ByteArrayToFloat(data, GenerateIndex(0));
                LastPingTime = Converters.ByteArrayToFloat(data, GenerateIndex(1));

                Heading = Converters.ByteArrayToFloat(data, GenerateIndex(2));
                Pitch = Converters.ByteArrayToFloat(data, GenerateIndex(3));
                Roll = Converters.ByteArrayToFloat(data, GenerateIndex(4));
                WaterTemp = Converters.ByteArrayToFloat(data, GenerateIndex(5));
                SystemTemp = Converters.ByteArrayToFloat(data, GenerateIndex(6));
                Salinity = Converters.ByteArrayToFloat(data, GenerateIndex(7));
                Pressure = Converters.ByteArrayToFloat(data, GenerateIndex(8));
                TransducerDepth = Converters.ByteArrayToFloat(data, GenerateIndex(9));
                SpeedOfSound = Converters.ByteArrayToFloat(data, GenerateIndex(10));

                Status = new Status((int)Converters.ByteArrayToFloat(data, GenerateIndex(11)));
                NumBeams = Converters.ByteArrayToFloat(data, GenerateIndex(12));
                ActualPingCount = Converters.ByteArrayToFloat(data, GenerateIndex(13));

                Range[0] = Converters.ByteArrayToFloat(data, GenerateIndex(14));
                Range[1] = Converters.ByteArrayToFloat(data, GenerateIndex(15));
                Range[2] = Converters.ByteArrayToFloat(data, GenerateIndex(16));
                Range[3] = Converters.ByteArrayToFloat(data, GenerateIndex(17));

                SNR[0] = Converters.ByteArrayToFloat(data, GenerateIndex(18));
                SNR[1] = Converters.ByteArrayToFloat(data, GenerateIndex(19));
                SNR[2] = Converters.ByteArrayToFloat(data, GenerateIndex(20));
                SNR[3] = Converters.ByteArrayToFloat(data, GenerateIndex(21));

                Amplitude[0] = Converters.ByteArrayToFloat(data, GenerateIndex(22));
                Amplitude[1] = Converters.ByteArrayToFloat(data, GenerateIndex(23));
                Amplitude[2] = Converters.ByteArrayToFloat(data, GenerateIndex(24));
                Amplitude[3] = Converters.ByteArrayToFloat(data, GenerateIndex(25));

                Correlation[0] = Converters.ByteArrayToFloat(data, GenerateIndex(26));
                Correlation[1] = Converters.ByteArrayToFloat(data, GenerateIndex(27));
                Correlation[2] = Converters.ByteArrayToFloat(data, GenerateIndex(28));
                Correlation[3] = Converters.ByteArrayToFloat(data, GenerateIndex(29));

                BeamVelocity[0] = Converters.ByteArrayToFloat(data, GenerateIndex(30));
                BeamVelocity[1] = Converters.ByteArrayToFloat(data, GenerateIndex(31));
                BeamVelocity[2] = Converters.ByteArrayToFloat(data, GenerateIndex(32));
                BeamVelocity[3] = Converters.ByteArrayToFloat(data, GenerateIndex(33));

                BeamGood[0] = Converters.ByteArrayToFloat(data, GenerateIndex(34));
                BeamGood[1] = Converters.ByteArrayToFloat(data, GenerateIndex(35));
                BeamGood[2] = Converters.ByteArrayToFloat(data, GenerateIndex(36));
                BeamGood[3] = Converters.ByteArrayToFloat(data, GenerateIndex(37));

                InstrumentVelocity[0] = Converters.ByteArrayToFloat(data, GenerateIndex(38));
                InstrumentVelocity[1] = Converters.ByteArrayToFloat(data, GenerateIndex(39));
                InstrumentVelocity[2] = Converters.ByteArrayToFloat(data, GenerateIndex(40));
                InstrumentVelocity[3] = Converters.ByteArrayToFloat(data, GenerateIndex(41));

                InstrumentGood[0] = Converters.ByteArrayToFloat(data, GenerateIndex(42));
                InstrumentGood[1] = Converters.ByteArrayToFloat(data, GenerateIndex(43));
                InstrumentGood[2] = Converters.ByteArrayToFloat(data, GenerateIndex(44));
                InstrumentGood[3] = Converters.ByteArrayToFloat(data, GenerateIndex(45));

                EarthVelocity[0] = Converters.ByteArrayToFloat(data, GenerateIndex(46));
                EarthVelocity[1] = Converters.ByteArrayToFloat(data, GenerateIndex(47));
                EarthVelocity[2] = Converters.ByteArrayToFloat(data, GenerateIndex(48));
                EarthVelocity[3] = Converters.ByteArrayToFloat(data, GenerateIndex(49));

                EarthGood[0] = Converters.ByteArrayToFloat(data, GenerateIndex(50));
                EarthGood[1] = Converters.ByteArrayToFloat(data, GenerateIndex(51));
                EarthGood[2] = Converters.ByteArrayToFloat(data, GenerateIndex(52));
                EarthGood[3] = Converters.ByteArrayToFloat(data, GenerateIndex(53));

                //CreateBindingList();
            }

            /// <summary>
            /// Generate a byte array representing the
            /// dataset.  The byte array is in the binary format.
            /// The format can be found in the RTI ADCP User Guide.
            /// It contains a header and payload.  This byte array 
            /// will be combined with the other dataset byte arrays
            /// to form an ensemble.
            /// </summary>
            /// <returns>Byte array of the ensemble.</returns>
            public byte[] Encode()
            {
                int index = 0;

                // Get the length
                byte[] payload = new byte[NUM_DATA_ELEMENTS * Ensemble.BYTES_IN_FLOAT];
                System.Buffer.BlockCopy(Converters.FloatToByteArray(FirstPingTime), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(LastPingTime), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(Converters.FloatToByteArray(Heading), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(Pitch), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(Roll), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(WaterTemp), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(SystemTemp), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(Salinity), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(Pressure), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(TransducerDepth), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(SpeedOfSound), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(Converters.FloatToByteArray(Status.Value), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(NumBeams), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(ActualPingCount), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(Converters.FloatToByteArray(Range[0]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(Range[1]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(Range[2]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(Range[3]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(Converters.FloatToByteArray(SNR[0]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(SNR[1]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(SNR[2]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(SNR[3]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(Converters.FloatToByteArray(Amplitude[0]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(Amplitude[1]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(Amplitude[2]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(Amplitude[3]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(Converters.FloatToByteArray(Correlation[0]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(Correlation[1]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(Correlation[2]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(Correlation[3]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(Converters.FloatToByteArray(BeamVelocity[0]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(BeamVelocity[1]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(BeamVelocity[2]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(BeamVelocity[3]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(Converters.FloatToByteArray(BeamGood[0]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(BeamGood[1]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(BeamGood[2]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(BeamGood[3]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(Converters.FloatToByteArray(InstrumentVelocity[0]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(InstrumentVelocity[1]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(InstrumentVelocity[2]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(InstrumentVelocity[3]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(Converters.FloatToByteArray(InstrumentGood[0]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(InstrumentGood[1]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(InstrumentGood[2]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(InstrumentGood[3]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(Converters.FloatToByteArray(EarthVelocity[0]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(EarthVelocity[1]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(EarthVelocity[2]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(EarthVelocity[3]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(Converters.FloatToByteArray(EarthGood[0]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(EarthGood[1]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(EarthGood[2]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(Converters.FloatToByteArray(EarthGood[3]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                // Generate header for the dataset
                byte[] header = this.GenerateHeader(NumElements);

                // Create the array to hold the dataset
                byte[] result = new byte[payload.Length + header.Length];

                // Copy the header to the array
                System.Buffer.BlockCopy(header, 0, result, 0, header.Length);

                // Copy the Nmea data to the array
                System.Buffer.BlockCopy(payload, 0, result, header.Length, payload.Length);

                return result;
            }

            /// <summary>
            /// Move pass the Base data of the Bottom Track.  Then based
            /// off the xAxis, move to the correct location.
            /// </summary>
            /// <param name="index">PlaybackIndex of the order of the data.</param>
            /// <returns>PlaybackIndex for the given xAxis.  Start of the data</returns>
            private int GenerateIndex(int index)
            {
                return GetBaseDataSize(NameLength) + (index * Ensemble.BYTES_IN_FLOAT);
            }

            /// <summary>
            /// Generate a index within a payload byte array.
            /// </summary>
            /// <param name="index">Element number within the payload.</param>
            /// <returns>Start location within the payload.</returns>
            private int GeneratePayloadIndex(int index)
            {
                return index * Ensemble.BYTES_IN_FLOAT;
            }

            /// <summary>
            /// Get all the information about the Bottom Track data.
            /// Data is passed in as a DataRow from a database.  The
            /// data is then extracted and added to the properties.
            /// </summary>
            /// <param name="dataRow">DataRow containing the Bottom Track data type.</param>
            private void Decode(DataRow dataRow)
            {
                try
                {
                    // Set the values based off the data given
                    FirstPingTime = Convert.ToSingle(dataRow[DbCommon.COL_BT_FIRST_PING_TIME].ToString());
                    LastPingTime = Convert.ToSingle(dataRow[DbCommon.COL_BT_LAST_PING_TIME].ToString());

                    Heading = Convert.ToSingle(dataRow[DbCommon.COL_BT_HEADING].ToString());
                    Pitch = Convert.ToSingle(dataRow[DbCommon.COL_BT_PITCH].ToString());
                    Roll = Convert.ToSingle(dataRow[DbCommon.COL_BT_ROLL].ToString());
                    WaterTemp = Convert.ToSingle(dataRow[DbCommon.COL_BT_TEMP_WATER].ToString());
                    SystemTemp = Convert.ToSingle(dataRow[DbCommon.COL_BT_TEMP_SYS].ToString());
                    Salinity = Convert.ToSingle(dataRow[DbCommon.COL_BT_SALINITY].ToString());
                    Pressure = Convert.ToSingle(dataRow[DbCommon.COL_BT_PRESSURE].ToString());
                    TransducerDepth = Convert.ToSingle(dataRow[DbCommon.COL_ENS_XDCR_DEPTH].ToString());
                    SpeedOfSound = Convert.ToSingle(dataRow[DbCommon.COL_BT_SOS].ToString());

                    Status = new Status(Convert.ToInt32(dataRow[DbCommon.COL_BT_STATUS].ToString()));
                    NumBeams = Convert.ToSingle(dataRow[DbCommon.COL_ENS_NUM_BEAM].ToString());
                    ActualPingCount = Convert.ToSingle(dataRow[DbCommon.COL_BT_ACTUAL_PING_COUNT].ToString());

                    Range[0] = Convert.ToSingle(dataRow[DbCommon.COL_BT_RANGE_B0].ToString());
                    Range[1] = Convert.ToSingle(dataRow[DbCommon.COL_BT_RANGE_B1].ToString());
                    Range[2] = Convert.ToSingle(dataRow[DbCommon.COL_BT_RANGE_B2].ToString());
                    Range[3] = Convert.ToSingle(dataRow[DbCommon.COL_BT_RANGE_B3].ToString());

                    SNR[0] = Convert.ToSingle(dataRow[DbCommon.COL_BT_SNR_B0].ToString());
                    SNR[1] = Convert.ToSingle(dataRow[DbCommon.COL_BT_SNR_B1].ToString());
                    SNR[2] = Convert.ToSingle(dataRow[DbCommon.COL_BT_SNR_B2].ToString());
                    SNR[3] = Convert.ToSingle(dataRow[DbCommon.COL_BT_SNR_B3].ToString());

                    Amplitude[0] = Convert.ToSingle(dataRow[DbCommon.COL_BT_AMP_B0].ToString());
                    Amplitude[1] = Convert.ToSingle(dataRow[DbCommon.COL_BT_AMP_B1].ToString());
                    Amplitude[2] = Convert.ToSingle(dataRow[DbCommon.COL_BT_AMP_B2].ToString());
                    Amplitude[3] = Convert.ToSingle(dataRow[DbCommon.COL_BT_AMP_B3].ToString());

                    Correlation[0] = Convert.ToSingle(dataRow[DbCommon.COL_BT_CORR_B0].ToString());
                    Correlation[1] = Convert.ToSingle(dataRow[DbCommon.COL_BT_CORR_B1].ToString());
                    Correlation[2] = Convert.ToSingle(dataRow[DbCommon.COL_BT_CORR_B2].ToString());
                    Correlation[3] = Convert.ToSingle(dataRow[DbCommon.COL_BT_CORR_B3].ToString());

                    BeamVelocity[0] = Convert.ToSingle(dataRow[DbCommon.COL_BT_BEAM_VEL_B0].ToString());
                    BeamVelocity[1] = Convert.ToSingle(dataRow[DbCommon.COL_BT_BEAM_VEL_B1].ToString());
                    BeamVelocity[2] = Convert.ToSingle(dataRow[DbCommon.COL_BT_BEAM_VEL_B2].ToString());
                    BeamVelocity[3] = Convert.ToSingle(dataRow[DbCommon.COL_BT_BEAM_VEL_B3].ToString());

                    BeamGood[0] = Convert.ToSingle(dataRow[DbCommon.COL_BT_BEAM_GOOD_B0].ToString());
                    BeamGood[1] = Convert.ToSingle(dataRow[DbCommon.COL_BT_BEAM_GOOD_B1].ToString());
                    BeamGood[2] = Convert.ToSingle(dataRow[DbCommon.COL_BT_BEAM_GOOD_B2].ToString());
                    BeamGood[3] = Convert.ToSingle(dataRow[DbCommon.COL_BT_BEAM_GOOD_B3].ToString());

                    InstrumentVelocity[0] = Convert.ToSingle(dataRow[DbCommon.COL_BT_INSTR_VEL_B0].ToString());
                    InstrumentVelocity[1] = Convert.ToSingle(dataRow[DbCommon.COL_BT_INSTR_VEL_B1].ToString());
                    InstrumentVelocity[2] = Convert.ToSingle(dataRow[DbCommon.COL_BT_INSTR_VEL_B2].ToString());
                    InstrumentVelocity[3] = Convert.ToSingle(dataRow[DbCommon.COL_BT_INSTR_VEL_B3].ToString());

                    InstrumentGood[0] = Convert.ToSingle(dataRow[DbCommon.COL_BT_INSTR_GOOD_B0].ToString());
                    InstrumentGood[1] = Convert.ToSingle(dataRow[DbCommon.COL_BT_INSTR_GOOD_B1].ToString());
                    InstrumentGood[2] = Convert.ToSingle(dataRow[DbCommon.COL_BT_INSTR_GOOD_B2].ToString());
                    InstrumentGood[3] = Convert.ToSingle(dataRow[DbCommon.COL_BT_INSTR_GOOD_B3].ToString());

                    EarthVelocity[0] = Convert.ToSingle(dataRow[DbCommon.COL_BT_EARTH_VEL_B0].ToString());
                    EarthVelocity[1] = Convert.ToSingle(dataRow[DbCommon.COL_BT_EARTH_VEL_B1].ToString());
                    EarthVelocity[2] = Convert.ToSingle(dataRow[DbCommon.COL_BT_EARTH_VEL_B2].ToString());
                    EarthVelocity[3] = Convert.ToSingle(dataRow[DbCommon.COL_BT_EARTH_VEL_B3].ToString());

                    EarthGood[0] = Convert.ToSingle(dataRow[DbCommon.COL_BT_EARTH_GOOD_B0].ToString());
                    EarthGood[1] = Convert.ToSingle(dataRow[DbCommon.COL_BT_EARTH_GOOD_B1].ToString());
                    EarthGood[2] = Convert.ToSingle(dataRow[DbCommon.COL_BT_EARTH_GOOD_B2].ToString());
                    EarthGood[3] = Convert.ToSingle(dataRow[DbCommon.COL_BT_EARTH_GOOD_B3].ToString());
                }
                catch (FormatException)
                {
                    //RecorderManager.Instance.ReportError("Error Decoding Bottom Track data.", e);
                }
            }

            /// <summary>
            /// Prti01Sentence contains bottom track velocity in Instrument form. It also
            /// contains the depth and status.
            /// </summary>
            /// <param name="sentence">DVL sentence containing data.</param>
            private void DecodeBottomTrackData(Prti01Sentence sentence)
            {
                // Set the instrument velocity X in m/s
                // If the velocity is bad, set bad velocity
                if (sentence.BottomTrackVelX.Value != DotSpatial.Positioning.Speed.BadDVL.Value)
                {
                    InstrumentVelocity[0] = Convert.ToSingle(sentence.BottomTrackVelX.ToMetersPerSecond().Value);
                }
                else
                {
                    // Set Bad velocity
                    InstrumentVelocity[0] = DataSet.Ensemble.BAD_VELOCITY;
                    EarthVelocity[0] = DataSet.Ensemble.BAD_VELOCITY;
                }

                // Set the instrument velocity Y in m/s
                // If the velocity is bad, set bad velocity
                if (sentence.BottomTrackVelY.Value != DotSpatial.Positioning.Speed.BadDVL.Value)
                {
                    InstrumentVelocity[1] = Convert.ToSingle(sentence.BottomTrackVelY.ToMetersPerSecond().Value);
                }
                else
                {
                    // Set Bad velocity
                    InstrumentVelocity[1] = DataSet.Ensemble.BAD_VELOCITY;
                    EarthVelocity[1] = DataSet.Ensemble.BAD_VELOCITY;
                }

                // Set the instrument velocity Z in m/s
                // If the velocity is bad, set bad velocity
                if (sentence.BottomTrackVelZ.Value != DotSpatial.Positioning.Speed.BadDVL.Value)
                {
                    InstrumentVelocity[2] = Convert.ToSingle(sentence.BottomTrackVelZ.ToMetersPerSecond().Value);
                }
                else
                {
                    // Set Bad velocity
                    InstrumentVelocity[2] = DataSet.Ensemble.BAD_VELOCITY;
                    EarthVelocity[2] = DataSet.Ensemble.BAD_VELOCITY;
                }

                // Set all 4 values to the same range, then average range will also report the correct value
                Range[0] = Convert.ToSingle(sentence.BottomTrackDepth.ToMeters().Value);
                Range[1] = Convert.ToSingle(sentence.BottomTrackDepth.ToMeters().Value);
                Range[2] = Convert.ToSingle(sentence.BottomTrackDepth.ToMeters().Value);
                Range[3] = Convert.ToSingle(sentence.BottomTrackDepth.ToMeters().Value);

                // Set the status value
                Status = new Status(sentence.SystemStatus.Value);

                // Set the water temp
                WaterTemp = Convert.ToSingle(sentence.Temperature / 100.0);   // Sentence stores temp at 1/100 degree Celcius.

            }

            /// <summary>
            /// Prti02Sentence contains bottom track velocity in Earth form. It also
            /// contains the depth and status.
            /// </summary>
            /// <param name="sentence">DVL sentence containing data.</param>
            private void DecodeBottomTrackData(Prti02Sentence sentence)
            {
                // Set the instrument velocity X in m/s
                // If the velocity is bad, set bad velocity
                if (sentence.BottomTrackVelEast.Value != DotSpatial.Positioning.Speed.BadDVL.Value)
                {
                    EarthVelocity[0] = Convert.ToSingle(sentence.BottomTrackVelEast.ToMetersPerSecond().Value);
                }
                else
                {
                    // Set Bad velocity
                    EarthVelocity[0] = DataSet.Ensemble.BAD_VELOCITY;
                    InstrumentVelocity[0] = DataSet.Ensemble.BAD_VELOCITY;
                }

                // Set the instrument velocity Y in m/s
                // If the velocity is bad, set bad velocity
                if (sentence.BottomTrackVelNorth.Value != DotSpatial.Positioning.Speed.BadDVL.Value)
                {
                    EarthVelocity[1] = Convert.ToSingle(sentence.BottomTrackVelNorth.ToMetersPerSecond().Value);
                }
                else
                {
                    // Set Bad velocity
                    EarthVelocity[1] = DataSet.Ensemble.BAD_VELOCITY;
                    InstrumentVelocity[1] = DataSet.Ensemble.BAD_VELOCITY;
                }

                // Set the instrument velocity Z in m/s
                // If the velocity is bad, set bad velocity
                if (sentence.BottomTrackVelUp.Value != DotSpatial.Positioning.Speed.BadDVL.Value)
                {
                    EarthVelocity[2] = Convert.ToSingle(sentence.BottomTrackVelUp.ToMetersPerSecond().Value);
                }
                else
                {
                    // Set Bad velocity
                    EarthVelocity[2] = DataSet.Ensemble.BAD_VELOCITY;
                    InstrumentVelocity[2] = DataSet.Ensemble.BAD_VELOCITY;
                }

                // Set all 4 values to the same range, then average range will also report the correct value
                Range[0] = Convert.ToSingle(sentence.BottomTrackDepth.ToMeters().Value);
                Range[1] = Convert.ToSingle(sentence.BottomTrackDepth.ToMeters().Value);
                Range[2] = Convert.ToSingle(sentence.BottomTrackDepth.ToMeters().Value);
                Range[3] = Convert.ToSingle(sentence.BottomTrackDepth.ToMeters().Value);

                // Set the status value
                Status = new Status(sentence.SystemStatus.Value);

                // Set the water temp
                WaterTemp = Convert.ToSingle(sentence.Temperature / 100.0);   // Sentence stores temp at 1/100 degree Celcius.

            }

            /// <summary>
            /// Return whether any of the Earth Velocity values are bad.
            /// If one is bad, they are all bad.
            /// </summary>
            /// <returns>TRUE = All values good / False = One or more of the values are bad.</returns>
            public bool IsEarthVelocityGood()
            {
                if (EarthVelocity[DataSet.Ensemble.BEAM_EAST_INDEX] == DataSet.Ensemble.BAD_VELOCITY ||
                    EarthVelocity[DataSet.Ensemble.BEAM_NORTH_INDEX] == DataSet.Ensemble.BAD_VELOCITY ||
                    EarthVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX] == DataSet.Ensemble.BAD_VELOCITY ||
                    EarthVelocity[DataSet.Ensemble.BEAM_Q_INDEX] == DataSet.Ensemble.BAD_VELOCITY)
                {
                    return false;
                }

                return true;
            }

            /// <summary>
            /// This will calculate the average range value.
            /// If a value is bad, it will not include the value.
            /// </summary>
            /// <returns></returns>
            public double GetAverageRange()
            {
                int count = 0;
                double result = 0;
                for (int beam = 0; beam < ElementsMultiplier; beam++)
                {
                    if (Range[beam] > 0)
                    {
                        result += Range[beam];
                        count++;
                    }
                }

                return result / count;
            }

            /// <summary>
            /// Calculate the magnitude of the velocity.  Use Earth form East, North and Vertical velocity to 
            /// calculate the value.
            /// </summary>
            /// <returns>Magnitude of Velocity.  If any velocities were bad, return 0.</returns>
            public double GetVelocityMagnitude()
            {
                // Ensure the velocities are good
                if ((EarthVelocity[0] != Ensemble.BAD_VELOCITY) &&
                    (EarthVelocity[1] != Ensemble.BAD_VELOCITY) &&
                    (EarthVelocity[2] != Ensemble.BAD_VELOCITY))
                {
                    return Math.Sqrt((EarthVelocity[0] * EarthVelocity[0]) + (EarthVelocity[1] * EarthVelocity[1]) + (EarthVelocity[2] * EarthVelocity[2]));
                }
                else
                {
                    return 0;
                }
            }

            /// <summary>
            /// Get the direction for the velocity.  Use the parameter
            /// to determine if the Y axis is North or East.  
            /// </summary>
            /// <param name="isYNorth">Set flag if you want Y axis to be North or East.</param>
            /// <returns>Direction of the velocity in degrees.  If any velocities were bad, return -1.</returns>
            public double GetVelocityDirection(bool isYNorth)
            {
                // Ensure the velocities are good
                if ((EarthVelocity[0] != Ensemble.BAD_VELOCITY) &&
                    (EarthVelocity[1] != Ensemble.BAD_VELOCITY) &&
                    (EarthVelocity[2] != Ensemble.BAD_VELOCITY))
                {
                    if (isYNorth)
                    {
                        return (Math.Atan2(EarthVelocity[1], EarthVelocity[0])) * (180 / Math.PI);
                    }
                    else
                    {
                        return (Math.Atan2(EarthVelocity[0], EarthVelocity[1])) * (180 / Math.PI);
                    }
                }
                else
                {
                    return -1;
                }
            }

            /// <summary>
            /// Override the ToString to return all the Bottom Track data as a string.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                int i;
                string s = "";

                s += "Heading " + Heading.ToString("0.00") + "\t\tWtemp    " + WaterTemp.ToString("0.00")  + "\tPings    " + ActualPingCount.ToString("0") + "\n";
                s += "Pitch   " + Pitch.ToString("0.00")   + "\t\tBtemp    " + SystemTemp.ToString("0.00") + "\tBeams    " + NumBeams.ToString("0") + "\n";
                s += "Roll    " + Roll.ToString("0.00")    + "\t\tSalinity " + Salinity.ToString("0.00")   + "\tPressure " + Pressure.ToString("0.00") + "\n";
                s += "Status  " + Status.ToString()     + "\t\t\tSOS    " + SpeedOfSound.ToString("0.00")    + "\n";
                s += "1st Ping  (s)" + FirstPingTime.ToString("0.000") + "\n";
                s += "Last Ping (s)" + LastPingTime.ToString("0.000") + "\n";
                s += "Beam  Vel ";
                for (i = 0; i < NumBeams; i++)
                    s += BeamVelocity[i].ToString("0.000") + " ";
                s += "Bm Good  ";
                for (i = 0; i < NumBeams; i++)
                    s += BeamGood[i].ToString("0") + " ";
                s += "\n";
                s += "Instr Vel ";
                for (i = 0; i < NumBeams; i++)
                    s += InstrumentVelocity[i].ToString("0.000") + " ";
                s += "In Good  ";
                for (i = 0; i < NumBeams; i++)
                    s += InstrumentGood[i].ToString("0") + " ";
                s += "\n";
                s += "EarthVelocity Vel ";
                for (i = 0; i < NumBeams; i++)
                    s += EarthVelocity[i].ToString("0.000") + " ";
                s += "Ea Good  ";
                for (i = 0; i < NumBeams; i++)
                    s += EarthGood[i].ToString("0") + " ";
                s += "\n";
                s += "SNR\t\t\t";
                for (i = 0; i < NumBeams; i++)
                    s += SNR[i].ToString("0") + "\t";
                s += "\n";
                s += "Amplitude\t";
                for (i = 0; i < NumBeams; i++)
                    s += Amplitude[i].ToString("0") + "\t";
                s += "\n";
                s += "Range\t\t";
                for (i = 0; i < NumBeams; i++)
                    s += Range[i].ToString("0.000") + "\t";
                s += "\n";

                return s;
            }
        }
    }
}