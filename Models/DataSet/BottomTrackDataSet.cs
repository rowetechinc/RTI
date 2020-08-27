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
 * 03/06/2012      RC          2.05       Modified GetAverageRange() to check for bad values.
 * 03/30/2012      RC          2.07       Moved Converters.cs methods to MathHelper.cs.
 * 06/20/2012      RC          2.12       Added IsInstrumentVelocityGood().
 * 06/21/2012      RC          2.12       Add 3 beam solution option in IsInstrumentVelocityGood() and IsEarthVelocityGood().
 * 02/27/2013      RC          2.18       Set FirstPingTime when decoding the PRTI sentences.
 * 02/28/2013      RC          2.18       Added JSON encoding and Decoding.
 * 08/13/2013      RC          2.19.4     Fixed a bug with the GetAverageRange() using ElementMulitplier instead of Range.Length.
 * 08/16/2013      RC          2.19.4     Added IsBeamVelocityGood.  A good way to check for 3 beam solutions or any bad values.
 * 01/06/2014      RC          2.21.3     Added constructor that takes a PRTI03 sentence.
 * 03/25/2014      RC          2.21.4     Added a simpler constructor and added DecodePd0Ensemble().
 * 04/15/2014      RC          2.21.4     In GetVelocityMagnitude() check for 4 beams.
 * 04/03/2015      RC          3.0.3      Fixed bug with Encode().
 * 04/10/2015      RC          3.0.4      Added GetRangeBin().
 * 09/25/2015      RC          3.1.1      Allow BT to handle 3 beams.
 * 11/11/2015      RC          3.2.1      Fixed bug in IsEarthVelocityGood.
 * 02/08/2017      RC          3.4.0      Add AddAdditionalBottomTrackData for PRTI03 sentence.
 * 08/25/2017      RC          3.4.2      Added Ship Velocity.
 * 09/13/2017      RC          3.4.3      Fixed bug if no beams in data.
 * 02/07/2018      RC          3.4.5      Added IsUpwardFacing() to know the direction the ADCP is facing.
 * 11/27/2018      RC          3.4.11     Fixed the sign for PD0 data in DecodePd0Ensemble() Earth Velocity.
 * 03/06/2019      RC          3.4.11     Remove screening the correlation data for PD0.
 * 06/26/2019      RC          3.4.12     Added decoding ShipVelocity in BottomTrackDataSet::Decode().
 *                                        Added setting the Ship Velocity in BottomTrackDataSet::DecodePd0Ensemble().
 *                                        Added ShipVelocity to JSON decoding.
 *                                        Updated GetVelocityMagnitude() and GetVelocityDirection() to handle bad values of 0.0 and ShipVelocity.
 * 07/03/2019      RC          3.4.12     In DecodePd0Ensemble(), do not remap Instrument and Ship velocity data.
 * 03/11/2020      RC          3.4.16     Added Pulse Coherent Values.
 * 08/26/2020      RC          3.4.20     Check for NaN when looking for bad velocities.
 * 
 */

using System;
using System.Data;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;

namespace RTI
{
    namespace DataSet
    {
        /// <summary>
        /// Data set containing all the Bottom Track data.
        /// </summary>
        [JsonConverter(typeof(BottomTrackDataSetSerializer))]
        public class BottomTrackDataSet : BaseDataSet
        {
            #region Variables

            /// <summary>
            /// Number of elements in this data set.
            /// </summary>
            public const int NUM_DATA_ELEMENTS = 54;

            #region Status

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

            #endregion

            #endregion

            #region Properties

            /// <summary>
            /// First Bottom Track Ping Time in seconds.
            /// </summary>
            public float FirstPingTime { get; set; }

            /// <summary>
            /// Round version of First Bottom Track Ping time in seconds.
            /// </summary>
            [JsonIgnore]
            public string FirstPingTimeRounded { get { return FirstPingTime.ToString("0.00"); } }

            /// <summary>
            /// Last Bottom Track Ping Time in seconds.
            /// </summary>
            public float LastPingTime { get; set; }

            /// <summary>
            /// Round version of Last Ping Time in seconds.
            /// </summary>
            [JsonIgnore]
            public string LastPingRounded { get { return LastPingTime.ToString("0.00"); } }

            /// <summary>
            /// Heading in degrees.
            /// </summary>
            public float Heading { get; set; }

            /// <summary>
            /// Round version of Heading in degrees.
            /// </summary>
            [JsonIgnore]
            public string HeadingRounded { get { return Heading.ToString("0.000"); } }

            /// <summary>
            /// Pitch in degrees.
            /// </summary>
            public float Pitch { get; set; }

            /// <summary>
            /// Round version of Pitch in degrees. 
            /// </summary>
            [JsonIgnore]
            public string PitchRounded { get { return Pitch.ToString("0.000"); } }

            /// <summary>
            /// Roll in degrees.
            /// </summary>
            public float Roll { get; set; }

            /// <summary>
            /// Round version of Roll in degrees. 
            /// </summary>
            [JsonIgnore]
            public string RollRounded { get { return Roll.ToString("0.000"); } }

            /// <summary>
            /// Water Temperature in degrees.  Used in Speed of Sound.
            /// </summary>
            public float WaterTemp { get; set; }

            /// <summary>
            /// Round version of Water Temperature in degrees.
            /// </summary>
            [JsonIgnore]
            public string WaterTempRounded { get { return WaterTemp.ToString("0.000"); } }

            /// <summary>
            /// System (board) temperature in degrees.
            /// </summary>
            public float SystemTemp { get; set; }

            /// <summary>
            /// Round version of System Temperature in degrees.
            /// </summary>
            [JsonIgnore]
            public string SystemTempRounded { get { return SystemTemp.ToString("0.000"); } }

            /// <summary>
            /// Salinity in parts per thousand (ppt).
            /// Used in Speed of Sound.
            /// </summary>
            public float Salinity { get; set; }

            /// <summary>
            /// Round version of Salinity in parts per thousand.
            /// </summary>
            [JsonIgnore]
            public string SalinityRounded { get { return Salinity.ToString("0.000"); } }

            /// <summary>
            /// Pressure in Pascal.
            /// </summary>
            public float Pressure { get; set; }

            /// <summary>
            /// Round version of Pressure in pascal.
            /// </summary>
            [JsonIgnore]
            public string PressureRounded { get { return Pressure.ToString("0.000"); } }

            /// <summary>
            /// Transducer Depth in meters.
            /// Used in Speed of Sound.
            /// </summary>
            public float TransducerDepth { get; set; }

            /// <summary>
            /// Round version of Tranducer Depth in meters.
            /// </summary>
            [JsonIgnore]
            public string TransducerDepthRounded { get { return TransducerDepth.ToString("0.000"); } }

            /// <summary>
            /// Speed of Sound in meters/sec.
            /// </summary>
            public float SpeedOfSound { get; set; }

            /// <summary>
            /// Round version of Speed Of Sound in meter/sec.
            /// </summary>
            [JsonIgnore]
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
            [JsonIgnore]
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
            /// Bottom Track Ship Velocity in meters/second.  Only populated if data is retransformed.
            /// </summary>
            public float[] ShipVelocity { get; set; }

            /// <summary>
            /// Bottom Track Pulse Coherent, Short Lag, First Echo Signal to Noise in dB.
            /// </summary>
            public float[] PulseCohr_Snr { get; set; }

            /// <summary>
            /// Bottom Track Pulse Coherent, Short Lag, First Echo Amplitude in dB.
            /// </summary>
            public float[] PulseCohr_Amp { get; set; }

            /// <summary>
            /// Bottom Track Pulse Coherent, Short Lag, First Echo Velocity in m/s.
            /// </summary>
            public float[] PulseCohr_Vel { get; set; }

            /// <summary>
            /// Bottom Track Pulse Coherent, Short Lag, First Echo Noise in m/s.
            /// </summary>
            public float[] PulseCohr_Noise { get; set; }

            /// <summary>
            /// Bottom Track Pulse Coherent, Short Lag, First Echo Correlation in percent.
            /// </summary>
            public float[] PulseCohr_Corr { get; set; }

            #endregion

            /// <summary>
            /// Create a Bottom Track data set.  This will create an empty dataset.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numElements">Number of Bin</param>
            /// <param name="elementMultiplier">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public BottomTrackDataSet(int valueType, int numElements, int elementMultiplier, int imag, int nameLength, string name) :
                base(valueType, numElements, elementMultiplier, imag, nameLength, name)
            {
                // Initialize arrays
                Init(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM);
            }

            /// <summary>
            /// Create a Bottom Track data set.  This will create an empty dataset.
            /// </summary>
            /// <param name="numBeams">Number of beams.  Default uses DEFAULT_NUM_BEAMS_BEAM.</param>
            public BottomTrackDataSet(int numBeams = DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM) :
                base(DataSet.Ensemble.DATATYPE_FLOAT,                   // Type of data stored (Float or Int)
                        NUM_DATA_ELEMENTS,                              // Number of data elements
                        1,                                              // Multiplier
                        DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                        DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                        DataSet.Ensemble.BottomTrackID)                 // Dataset ID
            {
                // Initialize arrays
                Init(numBeams);
            }

            /// <summary>
            /// Create a Bottom Track data set.  Includes all the information
            /// about the current Bottom Track data.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numElements">Number of Bin</param>
            /// <param name="elementMultiplier">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="ancData">Byte array containing Bottom Track data</param>
            public BottomTrackDataSet(int valueType, int numElements, int elementMultiplier, int imag, int nameLength, string name, byte[] ancData) :
                base(valueType, numElements, elementMultiplier, imag, nameLength, name)
            {
                // Decode the information
                Decode(ancData);
            }

            /// <summary>
            /// Create a Bottom Track data set.  Includes all the information
            /// about the current Bottom Track data.
            /// </summary>
            /// <param name="sentence"></param>
            public BottomTrackDataSet(Prti01Sentence sentence) :
                base(DataSet.Ensemble.DATATYPE_FLOAT, NUM_DATA_ELEMENTS, DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.BottomTrackID)
            {
                // Initialize arrays
                Init(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM);

                // Decode the information
                DecodeBottomTrackData(sentence);
            }

            /// <summary>
            /// Create a Bottom Track data set.  Includes all the information
            /// about the current Bottom Track data.
            /// </summary>
            /// <param name="sentence">NMEA sentence.</param>
            public BottomTrackDataSet(Prti02Sentence sentence) :
                base(DataSet.Ensemble.DATATYPE_FLOAT, NUM_DATA_ELEMENTS, DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.BottomTrackID)
            {
                // Initialize arrays
                Init(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM);

                // Decode the information
                DecodeBottomTrackData(sentence);
            }

            /// <summary>
            /// Create a Bottom Track data set.  Includes all the information
            /// about the current Bottom Track data.
            /// </summary>
            /// <param name="sentence">NMEA sentence.</param>
            public BottomTrackDataSet(Prti03Sentence sentence) :
                base(DataSet.Ensemble.DATATYPE_FLOAT, NUM_DATA_ELEMENTS, DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.BottomTrackID)
            {
                // Initialize arrays
                Init(DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM);

                // Decode the information
                DecodeBottomTrackData(sentence);
            }

            /// <summary>
            /// Create an Bottom Track data set.  Intended for JSON  deserialize.  This method
            /// is called when Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.BottomTrackDataSet}(json) is
            /// called.
            /// 
            /// DeserializeObject is slightly faster then passing the string to the constructor.
            /// 97ms for this method.
            /// 181ms for JSON string constructor.
            /// 
            /// Alternative to decoding manually is to use the command:
            /// DataSet.BottomTrackDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.BottomTrackDataSet}(json); 
            /// 
            /// To use this method for JSON you must have all the parameters match all the properties in this object.
            /// 
            /// </summary>
            /// <param name="ValueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="NumElements">Number of Elements in the dataset.</param>
            /// <param name="ElementsMultiplier">Items per Elements.</param>
            /// <param name="Imag"></param>
            /// <param name="NameLength">Length of name</param>
            /// <param name="Name">Name of data type</param>
            /// <param name="FirstPingTime">Time of first ping in seconds</param>
            /// <param name="LastPingTime">Time of last ping in seconds.</param>
            /// <param name="Heading">Heading in degrees.</param>
            /// <param name="Pitch">Pitch in degrees.</param>
            /// <param name="Roll">Roll in degrees.</param>
            /// <param name="WaterTemp">Water Temperature in degrees farenheit.</param>
            /// <param name="SystemTemp">System Temperature in degrees farenheit.</param>
            /// <param name="Salinity">Salinity of the water in PPM.</param>
            /// <param name="Pressure">Pressure read by the pressure sensor in Pascals.</param>
            /// <param name="TransducerDepth">Depth of the transducer in meters.</param>
            /// <param name="SpeedOfSound">Speed of Sound measured in m/s.</param>
            /// <param name="Status">Status of the Adcp.</param>
            /// <param name="NumBeams">Number of beams.</param>
            /// <param name="ActualPingCount">Actual Ping Count.</param>
            /// <param name="Range">Range array.</param>
            /// <param name="SNR">Signal To Noise Ratio array.</param>
            /// <param name="Amplitude">Amplitude array.</param>
            /// <param name="Correlation">Correlation array.</param>
            /// <param name="BeamVelocity">Beam Velocity array.</param>
            /// <param name="BeamGood">Good Beam Velocity array.</param>
            /// <param name="InstrumentVelocity">Instrument Velocity array.</param>
            /// <param name="InstrumentGood">Good Instrument Velocity array.</param>
            /// <param name="EarthVelocity">Earth Velocity array.</param>
            /// <param name="EarthGood">Good Earth Velocity array.</param>
            [JsonConstructor]
            public BottomTrackDataSet(int ValueType, int NumElements, int ElementsMultiplier, int Imag, int NameLength, string Name,
                        float FirstPingTime, float LastPingTime,
                        float Heading, float Pitch, float Roll, float WaterTemp, float SystemTemp,
                        float Salinity, float Pressure, float TransducerDepth, float SpeedOfSound,
                        Status Status, float NumBeams, float ActualPingCount,
                        float[] Range, float[] SNR, float[] Amplitude, float[] Correlation, 
                        float[] BeamVelocity, float[] BeamGood, float[] InstrumentVelocity,
                        float[] InstrumentGood, float[] EarthVelocity, float[] EarthGood) :
                base(ValueType, NumElements, ElementsMultiplier, Imag, NameLength, Name)
            {
                // Initialize the values
                this.FirstPingTime = FirstPingTime;
                this.LastPingTime = LastPingTime;
                this.Heading = Heading;
                this.Pitch = Pitch;
                this.Roll = Roll;
                this.WaterTemp = WaterTemp;
                this.SystemTemp = SystemTemp;
                this.Salinity = Salinity;
                this.Pressure = Pressure;
                this.TransducerDepth = TransducerDepth;
                this.SpeedOfSound = SpeedOfSound;
                this.Status = Status;
                this.NumBeams = NumBeams;
                this.ActualPingCount = ActualPingCount;

                this.Range = Range;
                this.SNR = SNR;
                this.Amplitude = Amplitude;
                this.Correlation = Correlation;
                this.BeamVelocity = BeamVelocity;
                this.BeamGood = BeamGood;
                this.InstrumentVelocity = InstrumentVelocity;
                this.InstrumentGood = InstrumentGood;
                this.EarthVelocity = EarthVelocity;
                this.EarthGood = EarthGood;
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
            /// This will add additional Bottom Track data from the
            /// Prti03Sentence.
            /// </summary>
            /// <param name="sentence">Additional data to add to the dataset.</param>
            public void AddAdditionalBottomTrackData(Prti03Sentence sentence)
            {
                // Decode the information
                DecodeBottomTrackData(sentence);
            }

            /// <summary>
            /// This will add additional Bottom Track data from the
            /// Prti30Sentence.
            /// </summary>
            /// <param name="sentence">Additional data to add to the dataset.</param>
            public void AddAdditionalBottomTrackData(Prti30Sentence sentence)
            {
                Heading = sentence.Heading;
                Pitch = sentence.Pitch;
                Roll = sentence.Roll;
            }

            /// <summary>
            /// This will add additional Bottom Track data from the
            /// Prti31Sentence.
            /// </summary>
            /// <param name="sentence">Additional data to add to the dataset.</param>
            public void AddAdditionalBottomTrackData(Prti31Sentence sentence)
            {
                Heading = sentence.Heading;
                Pitch = sentence.Pitch;
                Roll = sentence.Roll;
            }

            /// <summary>
            /// Initialize all the arrays for the Bottom Track dataset.
            /// This will create the arrays based off the number of beams
            /// given.
            /// </summary>
            /// <param name="numBeams">Number of beams.</param>
            public void Init(int numBeams)
            {
                FirstPingTime = 0.0f;
                LastPingTime = 0.0f;

                Heading = 0.0f;
                Pitch = 0.0f;
                Roll = 0.0f;
                WaterTemp = 0.0f;
                SystemTemp = 0.0f;
                Salinity = 0.0f;
                Pressure = 0.0f;
                TransducerDepth = 0.0f;
                SpeedOfSound = 0.0f;

                Status = new Status(0);
                NumBeams = numBeams;
                ActualPingCount = 0.0f;

                CreateArrays(numBeams);
            }

            /// <summary>
            /// Create all the arrays with the number of beams.
            /// </summary>
            /// <param name="numBeams">Number of beams.</param>
            private void CreateArrays(int numBeams)
            {
                Range = new float[numBeams];
                SNR = new float[numBeams];
                Amplitude = new float[numBeams];
                Correlation = new float[numBeams];
                BeamVelocity = new float[numBeams];
                BeamGood = new float[numBeams];
                InstrumentVelocity = new float[numBeams];
                InstrumentGood = new float[numBeams];
                EarthVelocity = new float[numBeams];
                EarthGood = new float[numBeams];
                ShipVelocity = new float[numBeams];
                PulseCohr_Snr = new float[numBeams];
                PulseCohr_Amp = new float[numBeams];
                PulseCohr_Vel = new float[numBeams];
                PulseCohr_Noise = new float[numBeams];
                PulseCohr_Corr = new float[numBeams];
            }

            /// <summary>
            /// Get all the information about the Bottom Track data.
            /// </summary>
            /// <param name="data">Byte array containing the Bottom Track data type.</param>
            private void Decode(byte[] data)
            {
                FirstPingTime = MathHelper.ByteArrayToFloat(data, GenerateIndex(0));
                LastPingTime = MathHelper.ByteArrayToFloat(data, GenerateIndex(1));

                Heading = MathHelper.ByteArrayToFloat(data, GenerateIndex(2));
                Pitch = MathHelper.ByteArrayToFloat(data, GenerateIndex(3));
                Roll = MathHelper.ByteArrayToFloat(data, GenerateIndex(4));
                WaterTemp = MathHelper.ByteArrayToFloat(data, GenerateIndex(5));
                SystemTemp = MathHelper.ByteArrayToFloat(data, GenerateIndex(6));
                Salinity = MathHelper.ByteArrayToFloat(data, GenerateIndex(7));
                Pressure = MathHelper.ByteArrayToFloat(data, GenerateIndex(8));
                TransducerDepth = MathHelper.ByteArrayToFloat(data, GenerateIndex(9));
                SpeedOfSound = MathHelper.ByteArrayToFloat(data, GenerateIndex(10));

                Status = new Status((int)MathHelper.ByteArrayToFloat(data, GenerateIndex(11)));
                NumBeams = MathHelper.ByteArrayToFloat(data, GenerateIndex(12));
                ActualPingCount = MathHelper.ByteArrayToFloat(data, GenerateIndex(13));

                // Init the arrays
                CreateArrays((int)NumBeams);

                // Initialize the index
                int index = 14;
                for (int beam = 0; beam < NumBeams; beam++)
                {
                    Range[beam] = MathHelper.ByteArrayToFloat(data, GenerateIndex(index++));
                }

                for (int beam = 0; beam < NumBeams; beam++)
                {
                    SNR[beam] = MathHelper.ByteArrayToFloat(data, GenerateIndex(index++));
                }

                for (int beam = 0; beam < NumBeams; beam++)
                {
                    Amplitude[beam] = MathHelper.ByteArrayToFloat(data, GenerateIndex(index++));
                }

                for (int beam = 0; beam < NumBeams; beam++)
                {
                    Correlation[beam] = MathHelper.ByteArrayToFloat(data, GenerateIndex(index++));
                }

                for (int beam = 0; beam < NumBeams; beam++)
                {
                    BeamVelocity[beam] = MathHelper.ByteArrayToFloat(data, GenerateIndex(index++));
                }

                for (int beam = 0; beam < NumBeams; beam++)
                {
                    BeamGood[beam] = MathHelper.ByteArrayToFloat(data, GenerateIndex(index++));
                }

                for (int beam = 0; beam < NumBeams; beam++)
                {
                    InstrumentVelocity[beam] = MathHelper.ByteArrayToFloat(data, GenerateIndex(index++));
                }

                for (int beam = 0; beam < NumBeams; beam++)
                {
                    InstrumentGood[beam] = MathHelper.ByteArrayToFloat(data, GenerateIndex(index++));
                }

                for (int beam = 0; beam < NumBeams; beam++)
                {
                    EarthVelocity[beam] = MathHelper.ByteArrayToFloat(data, GenerateIndex(index++));
                }

                for (int beam = 0; beam < NumBeams; beam++)
                {
                    EarthGood[beam] = MathHelper.ByteArrayToFloat(data, GenerateIndex(index++));
                }

                // Check for additional data
                if (data.Length >= 55)
                {
                    for (int beam = 0; beam < NumBeams; beam++)
                    {
                        PulseCohr_Snr[beam] = MathHelper.ByteArrayToFloat(data, GenerateIndex(index++));
                    }

                    for (int beam = 0; beam < NumBeams; beam++)
                    {
                        PulseCohr_Amp[beam] = MathHelper.ByteArrayToFloat(data, GenerateIndex(index++));
                    }

                    for (int beam = 0; beam < NumBeams; beam++)
                    {
                        PulseCohr_Vel[beam] = MathHelper.ByteArrayToFloat(data, GenerateIndex(index++));
                    }

                    for (int beam = 0; beam < NumBeams; beam++)
                    {
                        PulseCohr_Noise[beam] = MathHelper.ByteArrayToFloat(data, GenerateIndex(index++));
                    }

                    for (int beam = 0; beam < NumBeams; beam++)
                    {
                        PulseCohr_Corr[beam] = MathHelper.ByteArrayToFloat(data, GenerateIndex(index++));
                    }
                }


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
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(FirstPingTime), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(LastPingTime), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Heading), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Pitch), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Roll), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(WaterTemp), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(SystemTemp), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Salinity), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Pressure), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(TransducerDepth), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(SpeedOfSound), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Status.Value), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(NumBeams), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                System.Buffer.BlockCopy(MathHelper.FloatToByteArray(ActualPingCount), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);

                for (int beam = 0; beam < NumBeams; beam++)
                {
                    System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Range[beam]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                }

                for (int beam = 0; beam < NumBeams; beam++)
                {
                    System.Buffer.BlockCopy(MathHelper.FloatToByteArray(SNR[beam]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                }

                for (int beam = 0; beam < NumBeams; beam++)
                {
                    System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Amplitude[beam]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                }

                for (int beam = 0; beam < NumBeams; beam++)
                {
                    System.Buffer.BlockCopy(MathHelper.FloatToByteArray(Correlation[beam]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                }

                for (int beam = 0; beam < NumBeams; beam++)
                {
                    System.Buffer.BlockCopy(MathHelper.FloatToByteArray(BeamVelocity[beam]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                }

                for (int beam = 0; beam < NumBeams; beam++)
                {
                    System.Buffer.BlockCopy(MathHelper.FloatToByteArray(BeamGood[beam]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                }

                for (int beam = 0; beam < NumBeams; beam++)
                {
                    System.Buffer.BlockCopy(MathHelper.FloatToByteArray(InstrumentVelocity[beam]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                }

                for (int beam = 0; beam < NumBeams; beam++)
                {
                    System.Buffer.BlockCopy(MathHelper.FloatToByteArray(InstrumentGood[beam]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                }

                for (int beam = 0; beam < NumBeams; beam++)
                {
                    System.Buffer.BlockCopy(MathHelper.FloatToByteArray(EarthVelocity[beam]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                }

                for (int beam = 0; beam < NumBeams; beam++)
                {
                    System.Buffer.BlockCopy(MathHelper.FloatToByteArray(EarthGood[beam]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                }

                for (int beam = 0; beam < NumBeams; beam++)
                {
                    System.Buffer.BlockCopy(MathHelper.FloatToByteArray(PulseCohr_Snr[beam]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                }

                for (int beam = 0; beam < NumBeams; beam++)
                {
                    System.Buffer.BlockCopy(MathHelper.FloatToByteArray(PulseCohr_Amp[beam]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                }

                for (int beam = 0; beam < NumBeams; beam++)
                {
                    System.Buffer.BlockCopy(MathHelper.FloatToByteArray(PulseCohr_Vel[beam]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                }

                for (int beam = 0; beam < NumBeams; beam++)
                {
                    System.Buffer.BlockCopy(MathHelper.FloatToByteArray(PulseCohr_Noise[beam]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                }

                for (int beam = 0; beam < NumBeams; beam++)
                {
                    System.Buffer.BlockCopy(MathHelper.FloatToByteArray(PulseCohr_Corr[beam]), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_FLOAT);
                }

                // Generate header for the dataset
                byte[] header = this.GenerateHeader(NUM_DATA_ELEMENTS);

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
            /// off the data, move to the correct location.
            /// </summary>
            /// <param name="index">Index of the order of the data.</param>
            /// <returns>kIndex for the given data.  Start of the data</returns>
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
                    ShipVelocity[0] = DataSet.Ensemble.BAD_VELOCITY;
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
                    ShipVelocity[1] = DataSet.Ensemble.BAD_VELOCITY;
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
                    ShipVelocity[2] = DataSet.Ensemble.BAD_VELOCITY;
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

                // First ping time
                FirstPingTime = Convert.ToInt32(sentence.StartTime / 100);      // Sentence stores the time in hundredths of a second
            }

            /// <summary>
            /// Prti03Sentence contains bottom track velocity in Instrument form. It also
            /// contains the Error velocity, depth and status.
            /// </summary>
            /// <param name="sentence">DVL sentence containing data.</param>
            private void DecodeBottomTrackData(Prti03Sentence sentence)
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
                    ShipVelocity[0] = DataSet.Ensemble.BAD_VELOCITY;
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
                    ShipVelocity[1] = DataSet.Ensemble.BAD_VELOCITY;
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
                    ShipVelocity[2] = DataSet.Ensemble.BAD_VELOCITY;
                }

                // Set the instrument velocity Q in m/s
                // If the velocity is bad, set bad velocity
                if (sentence.BottomTrackVelQ.Value != DotSpatial.Positioning.Speed.BadDVL.Value)
                {
                    InstrumentVelocity[3] = Convert.ToSingle(sentence.BottomTrackVelQ.ToMetersPerSecond().Value);
                }
                else
                {
                    // Set Bad velocity
                    InstrumentVelocity[3] = DataSet.Ensemble.BAD_VELOCITY;
                    EarthVelocity[3] = DataSet.Ensemble.BAD_VELOCITY;
                    ShipVelocity[3] = DataSet.Ensemble.BAD_VELOCITY;
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

                // First ping time
                FirstPingTime = Convert.ToInt32(sentence.StartTime / 100);      // Sentence stores the time in hundredths of a second
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
                    ShipVelocity[0] = DataSet.Ensemble.BAD_VELOCITY;
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
                    ShipVelocity[1] = DataSet.Ensemble.BAD_VELOCITY;
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
                    ShipVelocity[2] = DataSet.Ensemble.BAD_VELOCITY;
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

                // First ping time
                FirstPingTime = Convert.ToInt32(sentence.StartTime / 100);      // Sentence stores the time in hundredths of a second
            }

            /// <summary>
            /// Return whether any of the Beam Velocity values are bad.
            /// If one is bad, they are all bad.
            /// 
            /// Also a good way to check if 3 beam solution was attempted.  If one value is bad, but Earth
            /// and Instrument data exist, then a 3 beam solution was done.
            /// </summary>
            /// <returns>TRUE = All values good / False = One or more of the values are bad.</returns>
            public bool IsBeamVelocityGood()
            {
                // Ensure at least a 4 beam system
                if (NumBeams >= DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM)
                {
                    // Check for RTB Bad Velocity
                    if (BeamVelocity[DataSet.Ensemble.BEAM_EAST_INDEX] == DataSet.Ensemble.BAD_VELOCITY ||
                        BeamVelocity[DataSet.Ensemble.BEAM_NORTH_INDEX] == DataSet.Ensemble.BAD_VELOCITY ||
                        BeamVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX] == DataSet.Ensemble.BAD_VELOCITY ||
                        BeamVelocity[DataSet.Ensemble.BEAM_Q_INDEX] == DataSet.Ensemble.BAD_VELOCITY)
                    {

                        return false;
                    }

                    // Check for PD0 Bad Velocity
                    if (BeamVelocity[DataSet.Ensemble.BEAM_EAST_INDEX] == PD0.BAD_VELOCITY ||
                        BeamVelocity[DataSet.Ensemble.BEAM_NORTH_INDEX] == PD0.BAD_VELOCITY ||
                        BeamVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX] == PD0.BAD_VELOCITY ||
                        BeamVelocity[DataSet.Ensemble.BEAM_Q_INDEX] == PD0.BAD_VELOCITY)
                    {

                        return false;
                    }

                    // Check for 0.0 Velocity
                    if (BeamVelocity[DataSet.Ensemble.BEAM_EAST_INDEX] == 0.0f &&
                        BeamVelocity[DataSet.Ensemble.BEAM_NORTH_INDEX] == 0.0f &&
                        BeamVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX] == 0.0f &&
                        BeamVelocity[DataSet.Ensemble.BEAM_Q_INDEX] == 0.0f)
                    {

                        return false;
                    }

                    // Check for NaN
                    if (Double.IsNaN(BeamVelocity[DataSet.Ensemble.BEAM_EAST_INDEX]) ||
                        Double.IsNaN(BeamVelocity[DataSet.Ensemble.BEAM_NORTH_INDEX]) ||
                        Double.IsNaN(BeamVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX]))
                    {

                        return false;
                    }
                }
                // Vertical beam
                else
                {
                    // Check for RTB Bad Velocity
                    if (NumBeams >= 1 && BeamVelocity[DataSet.Ensemble.BEAM_EAST_INDEX] == DataSet.Ensemble.BAD_VELOCITY)
                    {
                        return false;
                    }

                    // Check for PD0 Bad Velocity
                    if (NumBeams >= 1 && BeamVelocity[DataSet.Ensemble.BEAM_EAST_INDEX] == PD0.BAD_VELOCITY)
                    {
                        return false;
                    }

                    // Check for 0.0 Velocity
                    if (NumBeams >= 1 && BeamVelocity[DataSet.Ensemble.BEAM_EAST_INDEX] == 0.0f)
                    {
                        return false;
                    }

                    // Check for 0.0 Velocity
                    if (NumBeams >= 1 && Double.IsNaN(BeamVelocity[DataSet.Ensemble.BEAM_EAST_INDEX]))
                    {
                        return false;
                    }
                }


                return true;
            }

            /// <summary>
            /// Return whether any of the Earth Velocity values are bad.
            /// If one is bad, they are all bad.
            /// If we do not allow 3 Beam solution and Q is bad, then all is bad.
            /// </summary>
            /// <param name="allow3BeamSolution">Allow a 3 Beam solution. Default = true</param>
            /// <returns>TRUE = All values good / False = One or more of the values are bad.</returns>
            public bool IsEarthVelocityGood(bool allow3BeamSolution = true)
            {
                // Ensure at least a 4 beam system
                if (NumBeams >= DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM)
                {
                    // If the Q is bad and we do not allow 3 Beam solution, then all is bad.
                    if (EarthVelocity[DataSet.Ensemble.BEAM_Q_INDEX] == DataSet.Ensemble.BAD_VELOCITY && !allow3BeamSolution)
                    {
                        return false;
                    }

                    // Check for RTB Bad Velocity
                    if (EarthVelocity[DataSet.Ensemble.BEAM_EAST_INDEX] == DataSet.Ensemble.BAD_VELOCITY ||
                        EarthVelocity[DataSet.Ensemble.BEAM_NORTH_INDEX] == DataSet.Ensemble.BAD_VELOCITY ||
                        EarthVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX] == DataSet.Ensemble.BAD_VELOCITY)
                    {

                        return false;
                    }

                    // Check for PD0 Bad Velocity
                    if (EarthVelocity[DataSet.Ensemble.BEAM_EAST_INDEX] == PD0.BAD_VELOCITY ||
                        EarthVelocity[DataSet.Ensemble.BEAM_NORTH_INDEX] == PD0.BAD_VELOCITY ||
                        EarthVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX] == PD0.BAD_VELOCITY)
                    {

                        return false;
                    }

                    // Check for 0.0 Velocity
                    if (EarthVelocity[DataSet.Ensemble.BEAM_EAST_INDEX] == 0.0f &&
                        EarthVelocity[DataSet.Ensemble.BEAM_NORTH_INDEX] == 0.0f &&
                        EarthVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX] == 0.0f)
                    {

                        return false;
                    }

                    // Check for NaN
                    if (Double.IsNaN(EarthVelocity[DataSet.Ensemble.BEAM_EAST_INDEX]) ||
                        Double.IsNaN(EarthVelocity[DataSet.Ensemble.BEAM_NORTH_INDEX]) ||
                        Double.IsNaN(EarthVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX]))
                    {

                        return false;
                    }
                }
                // If a vertical beam
                else
                {
                    // Check for RTB Bad Velocity
                    if (NumBeams >= 1 && EarthVelocity[DataSet.Ensemble.BEAM_EAST_INDEX] == DataSet.Ensemble.BAD_VELOCITY)
                    {
                        return false;
                    }

                    // Check for PD0 Bad Velocity
                    if (NumBeams >= 1 && EarthVelocity[DataSet.Ensemble.BEAM_EAST_INDEX] == PD0.BAD_VELOCITY)
                    {
                        return false;
                    }

                    // Check for 0.0 Velocity
                    if (NumBeams >= 1 && EarthVelocity[DataSet.Ensemble.BEAM_EAST_INDEX] == 0.0f)
                    {
                        return false;
                    }

                    // Check for NaN
                    if (NumBeams >= 1 && Double.IsNaN(EarthVelocity[DataSet.Ensemble.BEAM_EAST_INDEX]))
                    {
                        return false;
                    }

                }

                return true;
            }

            /// <summary>
            /// Return whether any of the Instrument Velocity values are bad.
            /// If one is bad, they are all bad.
            /// If we do not allow 3 Beam solution and Q is bad, then all is bad.
            /// </summary>
            /// <param name="allow3BeamSolution">Allow a 3 Beam solution. Default = true</param>
            /// <returns>TRUE = All values good / False = One or more of the values are bad.</returns>
            public bool IsInstrumentVelocityGood(bool allow3BeamSolution = true)
            {
                // Ensure at least a 4 beam system
                if (NumBeams >= DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM)
                {
                    // If the Q is bad and we do not allow 3 Beam solution, then all is bad.
                    if (InstrumentVelocity[DataSet.Ensemble.BEAM_Q_INDEX] == DataSet.Ensemble.BAD_VELOCITY && !allow3BeamSolution)
                    {
                        return false;
                    }

                    // Check for RTB Bad Velocity
                    if (InstrumentVelocity[DataSet.Ensemble.BEAM_X_INDEX] == DataSet.Ensemble.BAD_VELOCITY ||
                        InstrumentVelocity[DataSet.Ensemble.BEAM_Y_INDEX] == DataSet.Ensemble.BAD_VELOCITY ||
                        InstrumentVelocity[DataSet.Ensemble.BEAM_Z_INDEX] == DataSet.Ensemble.BAD_VELOCITY)
                    {
                        return false;
                    }

                    // Check for PD0 Bad Velocity
                    if (InstrumentVelocity[DataSet.Ensemble.BEAM_X_INDEX] == PD0.BAD_VELOCITY ||
                        InstrumentVelocity[DataSet.Ensemble.BEAM_Y_INDEX] == PD0.BAD_VELOCITY ||
                        InstrumentVelocity[DataSet.Ensemble.BEAM_Z_INDEX] == PD0.BAD_VELOCITY)
                    {
                        return false;
                    }

                    // Check for 0.0 Velocity
                    if (InstrumentVelocity[DataSet.Ensemble.BEAM_X_INDEX] == 0.0f &&
                        InstrumentVelocity[DataSet.Ensemble.BEAM_Y_INDEX] == 0.0f &&
                        InstrumentVelocity[DataSet.Ensemble.BEAM_Z_INDEX] == 0.0f)
                    {

                        return false;
                    }

                    // Check for NaN
                    if (Double.IsNaN(InstrumentVelocity[DataSet.Ensemble.BEAM_X_INDEX]) ||
                        Double.IsNaN(InstrumentVelocity[DataSet.Ensemble.BEAM_Y_INDEX]) ||
                        Double.IsNaN(InstrumentVelocity[DataSet.Ensemble.BEAM_Z_INDEX]))
                    {

                        return false;
                    }
                }
                else
                {
                    // Vertical beam only for RTB Bad Velocity
                    if (NumBeams >= 1 && InstrumentVelocity[DataSet.Ensemble.BEAM_X_INDEX] == DataSet.Ensemble.BAD_VELOCITY)
                    {
                        return false;
                    }

                    // Vertical beam only for PD0 Bad Velocity
                    if (NumBeams >= 1 && InstrumentVelocity[DataSet.Ensemble.BEAM_X_INDEX] == PD0.BAD_VELOCITY)
                    {
                        return false;
                    }

                    // Vertical beam only for 0.0 Velocity
                    if (NumBeams >= 1 && InstrumentVelocity[DataSet.Ensemble.BEAM_X_INDEX] == 0.0f)
                    {
                        return false;
                    }

                    // Vertical beam only for NaN
                    if (NumBeams >= 1 && Double.IsNaN(InstrumentVelocity[DataSet.Ensemble.BEAM_X_INDEX]))
                    {
                        return false;
                    }
                }

                return true;
            }

            /// <summary>
            /// Return whether any of the Ship Velocity values are bad.
            /// If one is bad, they are all bad.
            /// If we do not allow 3 Beam solution and Q is bad, then all is bad.
            /// </summary>
            /// <param name="allow3BeamSolution">Allow a 3 Beam solution. Default = true</param>
            /// <returns>TRUE = All values good / False = One or more of the values are bad.</returns>
            public bool IsShipVelocityGood(bool allow3BeamSolution = true)
            {
                // Ensure at least a 4 beam system
                if (NumBeams >= DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM)
                {
                    // If the Q is bad and we do not allow 3 Beam solution, then all is bad.
                    if (ShipVelocity[DataSet.Ensemble.BEAM_Q_INDEX] == DataSet.Ensemble.BAD_VELOCITY && !allow3BeamSolution)
                    {
                        return false;
                    }

                    // Check for RTB Bad Velocity
                    if (ShipVelocity[DataSet.Ensemble.BEAM_X_INDEX] == DataSet.Ensemble.BAD_VELOCITY ||
                        ShipVelocity[DataSet.Ensemble.BEAM_Y_INDEX] == DataSet.Ensemble.BAD_VELOCITY ||
                        ShipVelocity[DataSet.Ensemble.BEAM_Z_INDEX] == DataSet.Ensemble.BAD_VELOCITY)
                    {
                        return false;
                    }

                    // Check for PD0 Bad Velocity
                    if (ShipVelocity[DataSet.Ensemble.BEAM_X_INDEX] == PD0.BAD_VELOCITY ||
                        ShipVelocity[DataSet.Ensemble.BEAM_Y_INDEX] == PD0.BAD_VELOCITY ||
                        ShipVelocity[DataSet.Ensemble.BEAM_Z_INDEX] == PD0.BAD_VELOCITY)
                    {
                        return false;
                    }

                    // Check for 0.0 Velocity
                    if (ShipVelocity[DataSet.Ensemble.BEAM_X_INDEX] == 0.0f &&
                        ShipVelocity[DataSet.Ensemble.BEAM_Y_INDEX] == 0.0f &&
                        ShipVelocity[DataSet.Ensemble.BEAM_Z_INDEX] == 0.0f)
                    {

                        return false;
                    }

                    // Check for NaN
                    if (Double.IsNaN(ShipVelocity[DataSet.Ensemble.BEAM_X_INDEX]) ||
                        Double.IsNaN(ShipVelocity[DataSet.Ensemble.BEAM_Y_INDEX]) ||
                        Double.IsNaN(ShipVelocity[DataSet.Ensemble.BEAM_Z_INDEX]))
                    {

                        return false;
                    }
                }
                else
                {
                    // Vertical beam only for RTB Bad Velocity
                    if (NumBeams >= 1 && ShipVelocity[DataSet.Ensemble.BEAM_X_INDEX] == DataSet.Ensemble.BAD_VELOCITY)
                    {
                        return false;
                    }

                    // Vertical beam only for PD0 Bad Velocity
                    if (NumBeams >= 1 && ShipVelocity[DataSet.Ensemble.BEAM_X_INDEX] == PD0.BAD_VELOCITY)
                    {
                        return false;
                    }

                    // Vertical beam only for 0.0 Velocity
                    if (NumBeams >= 1 && ShipVelocity[DataSet.Ensemble.BEAM_X_INDEX] == 0.0f)
                    {
                        return false;
                    }

                    // Vertical beam only for NaN
                    if (NumBeams >= 1 && Double.IsNaN(ShipVelocity[DataSet.Ensemble.BEAM_X_INDEX]))
                    {
                        return false;
                    }
                }

                return true;
            }

            /// <summary>
            /// This will calculate the average range value.
            /// If a value is bad, it will not include the value.
            /// </summary>
            /// <returns>Average range to the bottom.</returns>
            public float GetAverageRange()
            {
                int count = 0;
                float result = 0.0f;
                for (int beam = 0; beam < Range.Length; beam++)
                {
                    if (Range[beam] > DataSet.Ensemble.BAD_RANGE)
                    {
                        result += Range[beam];
                        count++;
                    }
                }

                // If at least 2 values are bad
                // Return a bad range
                if (count < 2)
                {
                    return DataSet.Ensemble.BAD_RANGE;
                }

                return result / count;
            }

            /// <summary>
            /// Get the bottom Track Bin.  This is based off the bin size
            /// and the range measured in the bottom track.
            /// Return a negative number if it is not good.
            /// </summary>
            /// <param name="binSize">Bins size.</param>
            /// <param name="blank">Blank distance.</param>
            /// <returns>Bottom Track bin.</returns>
            public int GetRangeBin(float binSize, float blank)
            {
                int bin = -1;

                // Get the bottom track depth
                double depth = GetAverageRange();

                // If no depth found, return 0
                if(depth == 0)
                {
                    return 0;
                }

                // Range = BtDepth * cos(BeamAngle)
                // Max Bin = (Range / binLength) - 1Bin
                //double range = GetAverageRange() * Math.Cos(beamAngle);

                //// Check for an error
                //if (binSize != 0)
                //{
                //    bin = (int)Math.Round((range / binSize) - 1.0);
                //}

                // Remove the blanking distance
                depth -= blank;

                // Ensure a depth is given
                if (depth > 0.0)
                {
                    double binDepth = depth / binSize;
                    bin = (int)Math.Round(binDepth);
                }

                return bin;
            }

            /// <summary>
            /// Calculate the magnitude of the velocity.  Use Earth form East, North and Vertical velocity to 
            /// calculate the value.
            /// </summary>
            /// <returns>Magnitude of Velocity.  If any velocities were bad, return 0.</returns>
            public double GetVelocityMagnitude()
            {
                // Ensure the velocities are good
                // They could be set to bad velocity or 0.0.  So check for both for bad values.
                if (EarthVelocity.Length >= 3 &&
                    ((EarthVelocity[0] != Ensemble.BAD_VELOCITY) && (EarthVelocity[1] != Ensemble.BAD_VELOCITY) && (EarthVelocity[2] != Ensemble.BAD_VELOCITY)) &&
                    ((EarthVelocity[0] != 0.0) && (EarthVelocity[1] != 0.0) && (EarthVelocity[2] != 0.0)))
                {
                    return Math.Sqrt((EarthVelocity[0] * EarthVelocity[0]) + (EarthVelocity[1] * EarthVelocity[1]) + (EarthVelocity[2] * EarthVelocity[2]));
                }
                else if (InstrumentVelocity.Length >= 3 &&
                    ((InstrumentVelocity[0] != Ensemble.BAD_VELOCITY) && (InstrumentVelocity[1] != Ensemble.BAD_VELOCITY) && (InstrumentVelocity[2] != Ensemble.BAD_VELOCITY)) &&
                    ((InstrumentVelocity[0] != 0.0) && (InstrumentVelocity[1] != 0.0) && (InstrumentVelocity[2] != 0.0)))
                {
                    return Math.Sqrt((InstrumentVelocity[0] * InstrumentVelocity[0]) + (InstrumentVelocity[1] * InstrumentVelocity[1]) + (InstrumentVelocity[2] * InstrumentVelocity[2]));
                }
                else if (ShipVelocity.Length >= 3 &&
                    ((ShipVelocity[0] != Ensemble.BAD_VELOCITY) && (ShipVelocity[1] != Ensemble.BAD_VELOCITY) && (ShipVelocity[2] != Ensemble.BAD_VELOCITY)) &&
                    ((ShipVelocity[0] != 0.0) && (ShipVelocity[1] != 0.0) && (ShipVelocity[2] != 0.0)))
                {
                    return Math.Sqrt((ShipVelocity[0] * ShipVelocity[0]) + (ShipVelocity[1] * ShipVelocity[1]) + (ShipVelocity[2] * ShipVelocity[2]));
                }
                else
                {
                    return 0.0;
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
                // They could be set to bad velocity or 0.0.  So check for both for bad values.
                if (EarthVelocity.Length >= 3 &&
                    ((EarthVelocity[0] != Ensemble.BAD_VELOCITY) && (EarthVelocity[1] != Ensemble.BAD_VELOCITY) && (EarthVelocity[2] != Ensemble.BAD_VELOCITY)) &&
                    ((EarthVelocity[0] != 0.0) && (EarthVelocity[1] != 0.0) && (EarthVelocity[2] != 0.0)))
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
                // Ensure the velocities are good
                else if (InstrumentVelocity.Length >= 3 &&
                    ((InstrumentVelocity[0] != Ensemble.BAD_VELOCITY) && (InstrumentVelocity[1] != Ensemble.BAD_VELOCITY) && (InstrumentVelocity[2] != Ensemble.BAD_VELOCITY)) &&
                    ((InstrumentVelocity[0] != 0.0) && (InstrumentVelocity[1] != 0.0) && (InstrumentVelocity[2] != 0.0)))
                {
                    if (isYNorth)
                    {
                        return (Math.Atan2(InstrumentVelocity[1], InstrumentVelocity[0])) * (180 / Math.PI);
                    }
                    else
                    {
                        return (Math.Atan2(InstrumentVelocity[0], InstrumentVelocity[1])) * (180 / Math.PI);
                    }
                }
                // Ensure the velocities are good
                else if (ShipVelocity.Length >= 3 &&
                    ((ShipVelocity[0] != Ensemble.BAD_VELOCITY) && (ShipVelocity[1] != Ensemble.BAD_VELOCITY) && (ShipVelocity[2] != Ensemble.BAD_VELOCITY)) &&
                    ((ShipVelocity[0] != 0.0) && (ShipVelocity[1] != 0.0) && (ShipVelocity[2] != 0.0)))
                {
                    if (isYNorth)
                    {
                        return (Math.Atan2(ShipVelocity[1], ShipVelocity[0])) * (180 / Math.PI);
                    }
                    else
                    {
                        return (Math.Atan2(ShipVelocity[0], ShipVelocity[1])) * (180 / Math.PI);
                    }
                }
                else
                {
                    return -1;
                }
            }

            /// <summary>
            /// Check if the ADCP is upward facing or downward facing.
            /// 
            /// The ADCP is Upward facing if the Roll is around 0 degrees.
            /// The ADCP is Downward facing if the Roll is around 180 degrees.
            /// </summary>
            /// <returns></returns>
            public bool IsUpwardFacing()
            {
                float roll = Math.Abs(Roll);

                if (roll > 0 && roll < 30)
                {
                    return true;
                }
                else
                {
                    return false;
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
                s += "ShipVelocity Vel ";
                for (i = 0; i < NumBeams; i++)
                    s += ShipVelocity[i].ToString("0.000") + " ";
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

            #region PD0 Ensemble

            /// <summary>
            /// Convert the PD0 Bottom Track data type to the RTI Bottom Track data set.
            /// </summary>
            /// <param name="bt">PD0 Bottom Track.</param>
            /// <param name="xform">Coordinate Transform.</param>
            /// <param name="vl">Variable Leader.</param>
            public void DecodePd0Ensemble(Pd0BottomTrack bt, PD0.CoordinateTransforms xform, Pd0VariableLeader vl)
            {


                // Get the time to seconds
                TimeSpan ts = new TimeSpan((int)vl.RtcY2kDay, (int)vl.RtcY2kHour, (int)vl.RtcY2kMinute, (int)vl.RtcY2kSecond);

                FirstPingTime = (float)ts.TotalSeconds;
                LastPingTime = FirstPingTime;
                Heading = vl.Heading;
                Pitch = vl.Pitch;
                Roll = vl.Roll;
                WaterTemp = vl.Temperature;
                SystemTemp = 0.0f;
                Salinity = vl.Salinity;
                Pressure = vl.Pressure / 0.0001f;
                TransducerDepth = vl.DepthOfTransducer / 10.0f;
                SpeedOfSound = 0.0f;
                Status = new Status(0);
                NumBeams = DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM;
                ActualPingCount = bt.BtPingsPerEnsemble;

                #region Range

                if (bt.NumBeams >= 4)
                {
                    Range[0] = bt.GetRangeBeam3() / 100.0f;
                    Range[1] = bt.GetRangeBeam2() / 100.0f;
                    Range[2] = bt.GetRangeBeam0() / 100.0f;
                    Range[3] = bt.GetRangeBeam1() / 100.0f;
                }
                else
                {
                    Range[0] = bt.GetRangeBeam0() / 100.0f;
                    Range[1] = bt.GetRangeBeam1() / 100.0f;
                    Range[2] = bt.GetRangeBeam2() / 100.0f;
                    Range[3] = bt.GetRangeBeam3() / 100.0f;
                }

                #endregion

                #region SNR

                if (bt.NumBeams >= 4)
                {
                    // SNR
                    // PD0 Beam 2, RTI Beam 0
                    if (bt.BtAmplitudeBeam2 < PD0.BAD_AMPLITUDE)
                    {
                        SNR[0] = bt.BtAmplitudeBeam2 / 2.0f;
                    }
                    else
                    {
                        SNR[0] = DataSet.Ensemble.BAD_VELOCITY;
                    }

                    // PD0 Beam 3, RTI Beam 1
                    if (bt.BtAmplitudeBeam3 < PD0.BAD_AMPLITUDE)
                    {
                        SNR[1] = bt.BtAmplitudeBeam3 / 2.0f;
                    }
                    else
                    {
                        SNR[1] = DataSet.Ensemble.BAD_VELOCITY;
                    }

                    // PD0 Beam 1, RTI Beam 2
                    if (bt.BtAmplitudeBeam1 < PD0.BAD_AMPLITUDE)
                    {
                        SNR[2] = bt.BtAmplitudeBeam1 / 2.0f;
                    }
                    else
                    {
                        SNR[2] = DataSet.Ensemble.BAD_VELOCITY;
                    }

                    // PD0 Beam 0, RTI Beam 3
                    if (bt.BtAmplitudeBeam0 < PD0.BAD_AMPLITUDE)
                    {
                        SNR[3] = bt.BtAmplitudeBeam0 / 2.0f;
                    }
                    else
                    {
                        SNR[3] = DataSet.Ensemble.BAD_VELOCITY;
                    }
                }
                else
                {
                    // SNR
                    if (bt.BtAmplitudeBeam2 < PD0.BAD_AMPLITUDE)
                    {
                        SNR[0] = bt.BtAmplitudeBeam0 / 2.0f;
                    }
                    else
                    {
                        SNR[0] = DataSet.Ensemble.BAD_VELOCITY;
                    }

                    if (bt.BtAmplitudeBeam1 < PD0.BAD_AMPLITUDE)
                    {
                        SNR[1] = bt.BtAmplitudeBeam1 / 2.0f;
                    }
                    else
                    {
                        SNR[1] = DataSet.Ensemble.BAD_VELOCITY;
                    }

                    if (bt.BtAmplitudeBeam2 < PD0.BAD_AMPLITUDE)
                    {
                        SNR[2] = bt.BtAmplitudeBeam2 / 2.0f;
                    }
                    else
                    {
                        SNR[2] = DataSet.Ensemble.BAD_VELOCITY;
                    }

                    // PD0 Beam 0, RTI Beam 3
                    if (bt.BtAmplitudeBeam3 < PD0.BAD_AMPLITUDE)
                    {
                        SNR[3] = bt.BtAmplitudeBeam3 / 2.0f;
                    }
                    else
                    {
                        SNR[3] = DataSet.Ensemble.BAD_VELOCITY;
                    }
                }

                #endregion

                #region Amplitude

                if (bt.NumBeams >= 4)
                {
                    // Amplitude
                    // PD0 Beam 2, RTI Beam 0
                    if (bt.BtRssiBeam2 < PD0.BAD_AMPLITUDE)
                    {
                        Amplitude[0] = bt.BtRssiBeam2 / 2.0f;
                    }
                    else
                    {
                        Amplitude[0] = DataSet.Ensemble.BAD_VELOCITY;
                    }

                    // PD0 Beam 3, RTI Beam 1
                    if (bt.BtRssiBeam3 < PD0.BAD_AMPLITUDE)
                    {
                        Amplitude[1] = bt.BtRssiBeam3 / 2.0f;
                    }
                    else
                    {
                        Amplitude[1] = DataSet.Ensemble.BAD_VELOCITY;
                    }

                    // PD0 Beam 1, RTI Beam 2
                    if (bt.BtRssiBeam1 < PD0.BAD_AMPLITUDE)
                    {
                        Amplitude[2] = bt.BtRssiBeam1 / 2.0f;
                    }
                    else
                    {
                        Amplitude[2] = DataSet.Ensemble.BAD_VELOCITY;
                    }

                    // PD0 Beam 0, RTI Beam 3
                    if (bt.BtRssiBeam0 < PD0.BAD_AMPLITUDE)
                    {
                        Amplitude[3] = bt.BtRssiBeam0 / 2.0f;
                    }
                    else
                    {
                        Amplitude[3] = DataSet.Ensemble.BAD_VELOCITY;
                    }
                }
                else
                {
                    // Amplitude
                    if (bt.BtRssiBeam0 < PD0.BAD_AMPLITUDE)
                    {
                        Amplitude[0] = bt.BtRssiBeam0 / 2.0f;
                    }
                    else
                    {
                        Amplitude[0] = DataSet.Ensemble.BAD_VELOCITY;
                    }

                    if (bt.BtRssiBeam1 < PD0.BAD_AMPLITUDE)
                    {
                        Amplitude[1] = bt.BtRssiBeam1 / 2.0f;
                    }
                    else
                    {
                        Amplitude[1] = DataSet.Ensemble.BAD_VELOCITY;
                    }

                    if (bt.BtRssiBeam2 < PD0.BAD_AMPLITUDE)
                    {
                        Amplitude[2] = bt.BtRssiBeam2 / 2.0f;
                    }
                    else
                    {
                        Amplitude[2] = DataSet.Ensemble.BAD_VELOCITY;
                    }

                    if (bt.BtRssiBeam3 < PD0.BAD_AMPLITUDE)
                    {
                        Amplitude[3] = bt.BtRssiBeam3 / 2.0f;
                    }
                    else
                    {
                        Amplitude[3] = DataSet.Ensemble.BAD_VELOCITY;
                    }
                }

                #endregion

                #region Correlation

                if (bt.NumBeams >= 4)
                {
                    // Correlation
                    // PD0 Beam 2, RTI Beam 0
                    Correlation[0] = bt.BtCorrelationMagnitudeBeam2 / 255.0f;

                    // PD0 Beam 3, RTI Beam 1
                    Correlation[1] = bt.BtCorrelationMagnitudeBeam3 / 255.0f;

                    // PD0 Beam 1, RTI Beam 2
                    Correlation[2] = bt.BtCorrelationMagnitudeBeam1 / 255.0f;

                    // PD0 Beam 0, RTI Beam 3
                    Correlation[3] = bt.BtCorrelationMagnitudeBeam0 / 255.0f;
                }
                else
                {
                    // Correlation
                    Correlation[0] = bt.BtCorrelationMagnitudeBeam0 / 255.0f;
                    Correlation[1] = bt.BtCorrelationMagnitudeBeam1 / 255.0f;
                    Correlation[2] = bt.BtCorrelationMagnitudeBeam2 / 255.0f;
                    Correlation[3] = bt.BtCorrelationMagnitudeBeam3 / 255.0f;
                }

                #endregion

                #region Velocity


                // Velocity
                switch(xform)
                {
                    // Beam Coordinate Transform
                    case PD0.CoordinateTransforms.Coord_Beam:

                        if (bt.NumBeams >= 4)
                        {
                            // PD0 Beam 2, RTI Beam 0
                            if (bt.BtVelocityBeam2 != PD0.BAD_VELOCITY)
                            {
                                BeamVelocity[0] = bt.BtVelocityBeam2 / 1000.0f;
                            }
                            else
                            {
                                BeamVelocity[0] = DataSet.Ensemble.BAD_VELOCITY;
                            }

                            // PD0 Beam 3, RTI Beam 1
                            if (bt.BtVelocityBeam3 != PD0.BAD_VELOCITY)
                            {
                                BeamVelocity[1] = bt.BtVelocityBeam3 / 1000.0f;
                            }
                            else
                            {
                                BeamVelocity[1] = DataSet.Ensemble.BAD_VELOCITY;
                            }

                            // PD0 Beam 1, RTI Beam 2
                            if (bt.BtVelocityBeam1 != PD0.BAD_VELOCITY)
                            {
                                BeamVelocity[2] = bt.BtVelocityBeam1 / 1000.0f;
                            }
                            else
                            {
                                BeamVelocity[2] = DataSet.Ensemble.BAD_VELOCITY;
                            }

                            // PD0 Beam 0, RTI Beam 3
                            if (bt.BtVelocityBeam0 != PD0.BAD_VELOCITY)
                            {
                                BeamVelocity[3] = bt.BtVelocityBeam0 / 1000.0f;
                            }
                            else
                            {
                                BeamVelocity[3] = DataSet.Ensemble.BAD_VELOCITY;
                            }
                        }
                        else
                        {
                            // Do Not Remap
                            if (bt.BtVelocityBeam0 != PD0.BAD_VELOCITY)
                            {
                                BeamVelocity[0] = bt.BtVelocityBeam0 / 1000.0f;
                            }
                            else
                            {
                                BeamVelocity[0] = DataSet.Ensemble.BAD_VELOCITY;
                            }

                            if (bt.BtVelocityBeam1 != PD0.BAD_VELOCITY)
                            {
                                BeamVelocity[1] = bt.BtVelocityBeam1 / 1000.0f;
                            }
                            else
                            {
                                BeamVelocity[1] = DataSet.Ensemble.BAD_VELOCITY;
                            }

                            if (bt.BtVelocityBeam2 != PD0.BAD_VELOCITY)
                            {
                                BeamVelocity[2] = bt.BtVelocityBeam2 / 1000.0f;
                            }
                            else
                            {
                                BeamVelocity[2] = DataSet.Ensemble.BAD_VELOCITY;
                            }

                            if (bt.BtVelocityBeam3 != PD0.BAD_VELOCITY)
                            {
                                BeamVelocity[3] = bt.BtVelocityBeam3 / 1000.0f;
                            }
                            else
                            {
                                BeamVelocity[3] = DataSet.Ensemble.BAD_VELOCITY;
                            }
                        }

                        break;

                    // Instrument Coordinate Transform
                    case PD0.CoordinateTransforms.Coord_Instrument:

                        if (bt.NumBeams >= 4)
                        {
                            // PD0 Beam 1, RTI Beam 0
                            if (bt.BtVelocityBeam1 != PD0.BAD_VELOCITY)
                            {
                                InstrumentVelocity[0] = bt.BtVelocityBeam1 / 1000.0f;
                            }
                            else
                            {
                                InstrumentVelocity[0] = DataSet.Ensemble.BAD_VELOCITY;
                            }

                            // PD0 Beam 0, RTI Beam 1
                            if (bt.BtVelocityBeam0 != PD0.BAD_VELOCITY)
                            {
                                InstrumentVelocity[1] = bt.BtVelocityBeam0 / 1000.0f;
                            }
                            else
                            {
                                InstrumentVelocity[1] = DataSet.Ensemble.BAD_VELOCITY;
                            }

                            // PD0 Beam -2, RTI Beam 2
                            if (bt.BtVelocityBeam2 != PD0.BAD_VELOCITY)
                            {
                                InstrumentVelocity[2] = (bt.BtVelocityBeam2 * -1) / 1000.0f;
                            }
                            else
                            {
                                InstrumentVelocity[2] = DataSet.Ensemble.BAD_VELOCITY;
                            }

                            // PD0 Beam 3, RTI Beam 3
                            if (bt.BtVelocityBeam3 != PD0.BAD_VELOCITY)
                            {
                                InstrumentVelocity[3] = bt.BtVelocityBeam3 / 1000.0f;
                            }
                            else
                            {
                                InstrumentVelocity[3] = DataSet.Ensemble.BAD_VELOCITY;
                            }
                        }
                        else
                        {
                            // DO NOT REMAP
                            if (bt.BtVelocityBeam0 != PD0.BAD_VELOCITY)
                            {
                                InstrumentVelocity[0] = bt.BtVelocityBeam0 / 1000.0f;
                            }
                            else
                            {
                                InstrumentVelocity[0] = DataSet.Ensemble.BAD_VELOCITY;
                            }

                            if (bt.BtVelocityBeam1 != PD0.BAD_VELOCITY)
                            {
                                InstrumentVelocity[1] = bt.BtVelocityBeam1 / 1000.0f;
                            }
                            else
                            {
                                InstrumentVelocity[1] = DataSet.Ensemble.BAD_VELOCITY;
                            }

                            if (bt.BtVelocityBeam2 != PD0.BAD_VELOCITY)
                            {
                                InstrumentVelocity[2] = bt.BtVelocityBeam2 / 1000.0f;
                            }
                            else
                            {
                                InstrumentVelocity[2] = DataSet.Ensemble.BAD_VELOCITY;
                            }

                            if (bt.BtVelocityBeam3 != PD0.BAD_VELOCITY)
                            {
                                InstrumentVelocity[3] = bt.BtVelocityBeam3 / 1000.0f;
                            }
                            else
                            {
                                InstrumentVelocity[3] = DataSet.Ensemble.BAD_VELOCITY;
                            }
                        }
                        break;

                    // Earth Coordinate Transform
                    case PD0.CoordinateTransforms.Coord_Earth:

                        // Beam 0
                        if(bt.BtVelocityBeam0 != PD0.BAD_VELOCITY)
                        {
                            EarthVelocity[0] = -1.0f * (bt.BtVelocityBeam0 / 1000.0f);
                        }
                        else
                        {
                            EarthVelocity[0] = DataSet.Ensemble.BAD_VELOCITY;
                        }

                        // Beam 1
                        if(bt.BtVelocityBeam1 != PD0.BAD_VELOCITY)
                        {
                            EarthVelocity[1] = -1.0f * (bt.BtVelocityBeam1 / 1000.0f);
                        }
                        else
                        {
                            EarthVelocity[1] = DataSet.Ensemble.BAD_VELOCITY;
                        }

                        // Beam 2
                        if(bt.BtVelocityBeam2 != PD0.BAD_VELOCITY)
                        {
                            EarthVelocity[2] = -1.0f * (bt.BtVelocityBeam2 / 1000.0f);
                        }
                        else
                        {
                            EarthVelocity[2] = DataSet.Ensemble.BAD_VELOCITY;
                        }

                        // Beam 3
                        if(bt.BtVelocityBeam3 != PD0.BAD_VELOCITY)
                        {
                            EarthVelocity[3] = -1.0f * (bt.BtVelocityBeam3 / 1000.0f);
                        }
                        else
                        {
                            EarthVelocity[3] = DataSet.Ensemble.BAD_VELOCITY;
                        }
                        break;

                    case PD0.CoordinateTransforms.Coord_Ship:

                        //if (bt.NumBeams >= 4)
                        //{
                        //    // PD0 Beam 1, RTI Beam 0
                        //    if (bt.BtVelocityBeam1 != PD0.BAD_VELOCITY)
                        //    {
                        //        ShipVelocity[0] = bt.BtVelocityBeam1 / 1000.0f;
                        //    }
                        //    else
                        //    {
                        //        ShipVelocity[0] = DataSet.Ensemble.BAD_VELOCITY;
                        //    }

                        //    // PD0 Beam 0, RTI Beam 1
                        //    if (bt.BtVelocityBeam0 != PD0.BAD_VELOCITY)
                        //    {
                        //        ShipVelocity[1] = bt.BtVelocityBeam0 / 1000.0f;
                        //    }
                        //    else
                        //    {
                        //        ShipVelocity[1] = DataSet.Ensemble.BAD_VELOCITY;
                        //    }

                        //    // PD0 Beam -2, RTI Beam 2
                        //    if (bt.BtVelocityBeam2 != PD0.BAD_VELOCITY)
                        //    {
                        //        ShipVelocity[2] = (bt.BtVelocityBeam2 * -1) / 1000.0f;
                        //    }
                        //    else
                        //    {
                        //        ShipVelocity[2] = DataSet.Ensemble.BAD_VELOCITY;
                        //    }

                        //    // PD0 Beam 3, RTI Beam 3
                        //    if (bt.BtVelocityBeam3 != PD0.BAD_VELOCITY)
                        //    {
                        //        ShipVelocity[3] = bt.BtVelocityBeam3 / 1000.0f;
                        //    }
                        //    else
                        //    {
                        //        ShipVelocity[3] = DataSet.Ensemble.BAD_VELOCITY;
                        //    }
                        //}
                        if (bt.NumBeams >= 4)
                        {
                            // PD0 Beam 0, RTI Beam 0
                            if (bt.BtVelocityBeam0 != PD0.BAD_VELOCITY)
                            {
                                ShipVelocity[0] = bt.BtVelocityBeam0 / 1000.0f;
                            }
                            else
                            {
                                ShipVelocity[0] = DataSet.Ensemble.BAD_VELOCITY;
                            }

                            // PD0 Beam 1, RTI Beam 1
                            if (bt.BtVelocityBeam1 != PD0.BAD_VELOCITY)
                            {
                                ShipVelocity[1] = bt.BtVelocityBeam1 / 1000.0f;
                            }
                            else
                            {
                                ShipVelocity[1] = DataSet.Ensemble.BAD_VELOCITY;
                            }

                            // PD0 Beam 2, RTI Beam 2
                            if (bt.BtVelocityBeam2 != PD0.BAD_VELOCITY)
                            {
                                ShipVelocity[2] = (bt.BtVelocityBeam2) / 1000.0f;
                            }
                            else
                            {
                                ShipVelocity[2] = DataSet.Ensemble.BAD_VELOCITY;
                            }

                            // PD0 Beam 3, RTI Beam 3
                            if (bt.BtVelocityBeam3 != PD0.BAD_VELOCITY)
                            {
                                ShipVelocity[3] = bt.BtVelocityBeam3 / 1000.0f;
                            }
                            else
                            {
                                ShipVelocity[3] = DataSet.Ensemble.BAD_VELOCITY;
                            }
                        }
                        else
                        {
                            // DO NOT REMAP
                            if (bt.BtVelocityBeam0 != PD0.BAD_VELOCITY)
                            {
                                ShipVelocity[0] = bt.BtVelocityBeam0 / 1000.0f;
                            }
                            else
                            {
                                ShipVelocity[0] = DataSet.Ensemble.BAD_VELOCITY;
                            }

                            if (bt.BtVelocityBeam1 != PD0.BAD_VELOCITY)
                            {
                                ShipVelocity[1] = bt.BtVelocityBeam1 / 1000.0f;
                            }
                            else
                            {
                                ShipVelocity[1] = DataSet.Ensemble.BAD_VELOCITY;
                            }

                            if (bt.BtVelocityBeam2 != PD0.BAD_VELOCITY)
                            {
                                ShipVelocity[2] = bt.BtVelocityBeam2 / 1000.0f;
                            }
                            else
                            {
                                ShipVelocity[2] = DataSet.Ensemble.BAD_VELOCITY;
                            }

                            if (bt.BtVelocityBeam3 != PD0.BAD_VELOCITY)
                            {
                                ShipVelocity[3] = bt.BtVelocityBeam3 / 1000.0f;
                            }
                            else
                            {
                                ShipVelocity[3] = DataSet.Ensemble.BAD_VELOCITY;
                            }
                        }
                        break;
                }

                #endregion

                #region Good Earth

                if (bt.NumBeams >= 4)
                {
                    // PD0 Beam 2, RTI Beam 0
                    if (bt.BtPercentGoodBeam2 != PD0.BAD_PERCENT_GOOD)
                    {
                        EarthGood[0] = (bt.BtPercentGoodBeam2 / 100.0f) * bt.BtPingsPerEnsemble;
                    }
                    else
                    {
                        EarthGood[0] = DataSet.Ensemble.BAD_VELOCITY;
                    }

                    // PD0 Beam 3, RTI Beam 1
                    if (bt.BtPercentGoodBeam3 != PD0.BAD_PERCENT_GOOD)
                    {
                        EarthGood[1] = (bt.BtPercentGoodBeam3 / 100.0f) * bt.BtPingsPerEnsemble;
                    }
                    else
                    {
                        EarthGood[1] = DataSet.Ensemble.BAD_VELOCITY;
                    }

                    // PD0 Beam 1, RTI Beam 2
                    if (bt.BtPercentGoodBeam1 != PD0.BAD_PERCENT_GOOD)
                    {
                        EarthGood[2] = (bt.BtPercentGoodBeam1 / 100.0f) * bt.BtPingsPerEnsemble;
                    }
                    else
                    {
                        EarthGood[2] = DataSet.Ensemble.BAD_VELOCITY;
                    }

                    // PD0 Beam 0, RTI Beam 3
                    if (bt.BtPercentGoodBeam0 != PD0.BAD_PERCENT_GOOD)
                    {
                        EarthGood[3] = (bt.BtPercentGoodBeam0 / 100.0f) * bt.BtPingsPerEnsemble;
                    }
                    else
                    {
                        EarthGood[3] = DataSet.Ensemble.BAD_VELOCITY;
                    }
                }
                else
                {
                    // DO NOT REMAP
                    if (bt.BtPercentGoodBeam0 != PD0.BAD_PERCENT_GOOD)
                    {
                        EarthGood[0] = (bt.BtPercentGoodBeam0 / 100.0f) * bt.BtPingsPerEnsemble;
                    }
                    else
                    {
                        EarthGood[0] = DataSet.Ensemble.BAD_VELOCITY;
                    }

                    if (bt.BtPercentGoodBeam1 != PD0.BAD_PERCENT_GOOD)
                    {
                        EarthGood[1] = (bt.BtPercentGoodBeam1 / 100.0f) * bt.BtPingsPerEnsemble;
                    }
                    else
                    {
                        EarthGood[1] = DataSet.Ensemble.BAD_VELOCITY;
                    }

                    if (bt.BtPercentGoodBeam2 != PD0.BAD_PERCENT_GOOD)
                    {
                        EarthGood[2] = (bt.BtPercentGoodBeam2 / 100.0f) * bt.BtPingsPerEnsemble;
                    }
                    else
                    {
                        EarthGood[2] = DataSet.Ensemble.BAD_VELOCITY;
                    }

                    if (bt.BtPercentGoodBeam3 != PD0.BAD_PERCENT_GOOD)
                    {
                        EarthGood[3] = (bt.BtPercentGoodBeam3 / 100.0f) * bt.BtPingsPerEnsemble;
                    }
                    else
                    {
                        EarthGood[3] = DataSet.Ensemble.BAD_VELOCITY;
                    }
                }

                #endregion

            }

            #endregion
        }

        /// <summary>
        /// Convert this object to a JSON object.
        /// Calling this method is twice as fast as calling the default serializer:
        /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.BottomTrackData).
        /// 
        /// 50ms for this method.
        /// 100ms for calling SerializeObject default.
        /// 
        /// Use this method whenever possible to convert to JSON.
        /// 
        /// http://james.newtonking.com/projects/json/help/
        /// http://james.newtonking.com/projects/json/help/index.html?topic=html/ReadingWritingJSON.htm
        /// http://blog.maskalik.com/asp-net/json-net-implement-custom-serialization
        /// </summary>
        public class BottomTrackDataSetSerializer : JsonConverter
        {
            /// <summary>
            /// Write the JSON string.  This will convert all the properties to a JSON string.
            /// This is done manaully to improve conversion time.  The default serializer will check
            /// each property if it can convert.  This will convert the properties automatically.  This
            /// will double the speed.
            /// 
            /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.BottomTrackData).
            /// 
            /// </summary>
            /// <param name="writer">JSON Writer.</param>
            /// <param name="value">Object to write to JSON.</param>
            /// <param name="serializer">Serializer to convert the object.</param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // Cast the object
                var data = value as BottomTrackDataSet;

                // Start the object
                writer.Formatting = Formatting.None;            // Make the text not indented, so not as human readable.  This will save disk space
                writer.WriteStartObject();                      // Start the JSON object

                // Write the base values
                writer.WriteRaw(data.ToJsonBaseStub());
                writer.WriteRaw(",");

                // FirstPingTime
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_FIRSTPINGTIME);
                writer.WriteValue(data.FirstPingTime);

                // LastPingTime
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_LASTPINGTIME);
                writer.WriteValue(data.LastPingTime);

                // Heading
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_HEADING);
                writer.WriteValue(data.Heading);

                // Pitch
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_PITCH);
                writer.WriteValue(data.Pitch);

                // Roll
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_ROLL);
                writer.WriteValue(data.Roll);

                // WaterTemp
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_WATERTEMP);
                writer.WriteValue(data.WaterTemp);

                // SystemTemp
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SYSTEMP);
                writer.WriteValue(data.SystemTemp);

                // Salinity
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SALINITY);
                writer.WriteValue(data.Salinity);

                // Pressure
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_PRESSURE);
                writer.WriteValue(data.Pressure);

                // TransducerDepth
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_TRANSDUCERDEPTH);
                writer.WriteValue(data.TransducerDepth);

                // SpeedOfSound
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SPEEDOFSOUND);
                writer.WriteValue(data.SpeedOfSound);

                // Status Value
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BT_STATUS);
                writer.WriteValue(data.Status.Value);

                // NumBeams
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BT_NUMBEAMS);
                writer.WriteValue(data.NumBeams);

                // ActualPingCount
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BT_ACTUALPINGCOUNT);
                writer.WriteValue(data.ActualPingCount);

                // Range
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BT_RANGE);
                writer.WriteStartArray();
                for (int beam = 0; beam < data.NumBeams; beam++)
                {
                    writer.WriteValue(data.Range[beam]);
                }
                writer.WriteEndArray();

                // SNR
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BT_SNR);
                writer.WriteStartArray();
                for (int beam = 0; beam < data.NumBeams; beam++)
                {
                    writer.WriteValue(data.SNR[beam]);
                }
                writer.WriteEndArray();

                // Amplitude
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BT_AMPLITUDE);
                writer.WriteStartArray();
                for (int beam = 0; beam < data.NumBeams; beam++)
                {
                    writer.WriteValue(data.Amplitude[beam]);
                }
                writer.WriteEndArray();

                // Correlation
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BT_CORRELATION);
                writer.WriteStartArray();
                for (int beam = 0; beam < data.NumBeams; beam++)
                {
                    writer.WriteValue(data.Correlation[beam]);
                }
                writer.WriteEndArray();

                // Beam Velocity
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BT_BEAMVELOCITY);
                writer.WriteStartArray();
                for (int beam = 0; beam < data.NumBeams; beam++)
                {
                    writer.WriteValue(data.BeamVelocity[beam]);
                }
                writer.WriteEndArray();

                // Good Beam Velocity
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BT_BEAMGOOD);
                writer.WriteStartArray();
                for (int beam = 0; beam < data.NumBeams; beam++)
                {
                    writer.WriteValue(data.BeamGood[beam]);
                }
                writer.WriteEndArray();

                // Instrument Velocity
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BT_INSTRUMENTVELOCITY);
                writer.WriteStartArray();
                for (int beam = 0; beam < data.NumBeams; beam++)
                {
                    writer.WriteValue(data.InstrumentVelocity[beam]);
                }
                writer.WriteEndArray();

                // Good Instrument Velocity
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BT_INSTRUMENTGOOD);
                writer.WriteStartArray();
                for (int beam = 0; beam < data.NumBeams; beam++)
                {
                    writer.WriteValue(data.InstrumentGood[beam]);
                }
                writer.WriteEndArray();

                // Earth Velocity
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BT_EARTHVELOCITY);
                writer.WriteStartArray();
                for (int beam = 0; beam < data.NumBeams; beam++)
                {
                    writer.WriteValue(data.EarthVelocity[beam]);
                }
                writer.WriteEndArray();

                // Good Earth Velocity
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BT_EARTHGOOD);
                writer.WriteStartArray();
                for (int beam = 0; beam < data.NumBeams; beam++)
                {
                    writer.WriteValue(data.EarthGood[beam]);
                }
                writer.WriteEndArray();

                // Ship Velocity
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BT_SHIPVELOCITY);
                writer.WriteStartArray();
                for (int beam = 0; beam < data.NumBeams; beam++)
                {
                    writer.WriteValue(data.ShipVelocity[beam]);
                }
                writer.WriteEndArray();

                // Pulse Coherent SNR
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BT_PC_SNR);
                writer.WriteStartArray();
                for (int beam = 0; beam < data.NumBeams; beam++)
                {
                    writer.WriteValue(data.PulseCohr_Snr[beam]);
                }
                writer.WriteEndArray();

                // Pulse Coherent Amplitude
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BT_PC_AMP);
                writer.WriteStartArray();
                for (int beam = 0; beam < data.NumBeams; beam++)
                {
                    writer.WriteValue(data.PulseCohr_Amp[beam]);
                }
                writer.WriteEndArray();

                // Pulse Coherent Velocity
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BT_PC_VEL);
                writer.WriteStartArray();
                for (int beam = 0; beam < data.NumBeams; beam++)
                {
                    writer.WriteValue(data.PulseCohr_Vel[beam]);
                }
                writer.WriteEndArray();

                // Pulse Coherent Noise
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BT_PC_NOISE);
                writer.WriteStartArray();
                for (int beam = 0; beam < data.NumBeams; beam++)
                {
                    writer.WriteValue(data.PulseCohr_Noise[beam]);
                }
                writer.WriteEndArray();

                // Pulse Coherent Correlation
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_BT_PC_CORR);
                writer.WriteStartArray();
                for (int beam = 0; beam < data.NumBeams; beam++)
                {
                    writer.WriteValue(data.PulseCohr_Corr[beam]);
                }
                writer.WriteEndArray();

                // End the object
                writer.WriteEndObject();
            }

            /// <summary>
            /// Read the JSON object and convert to the object.  This will allow the serializer to
            /// automatically convert the object.  No special instructions need to be done and all
            /// the properties found in the JSON string need to be used.
            /// 
            /// DataSet.BottomTrackDataSet decodedEns = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.BottomTrackDataSet}(encodedEns)
            /// 
            /// </summary>
            /// <param name="reader">NOT USED. JSON reader.</param>
            /// <param name="objectType">NOT USED> Type of object.</param>
            /// <param name="existingValue">NOT USED.</param>
            /// <param name="serializer">Serialize the object.</param>
            /// <returns>Serialized object.</returns>
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType != JsonToken.Null)
                {
                    // Load the object
                    JObject jsonObject = JObject.Load(reader);

                    // Decode the data
                    int NumElements = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_NUMELEMENTS];
                    int ElementsMultiplier = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_ELEMENTSMULTIPLIER];

                    // Create the object
                    var data = new BottomTrackDataSet(DataSet.Ensemble.DATATYPE_FLOAT, NumElements, ElementsMultiplier, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.BottomTrackID);

                    // FirstPingTime
                    data.FirstPingTime = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_FIRSTPINGTIME];

                    // LastPingTime
                    data.LastPingTime = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_LASTPINGTIME];

                    // Heading
                    data.Heading = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_HEADING];

                    // Pitch
                    data.Pitch = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_PITCH];

                    // Roll
                    data.Roll = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_ROLL];

                    // WaterTemp
                    data.WaterTemp = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_WATERTEMP];

                    // SystemTemp
                    data.SystemTemp = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SYSTEMP];

                    // Salinity
                    data.Salinity = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SALINITY];

                    // Pressure
                    data.Pressure = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_PRESSURE];

                    // TransducerDepth
                    data.TransducerDepth = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_TRANSDUCERDEPTH];

                    // SpeedOfSound
                    data.SpeedOfSound = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SPEEDOFSOUND];

                    // Status Value
                    data.Status = new Status((int)jsonObject[DataSet.BaseDataSet.JSON_STR_BT_STATUS]);

                    // NumBeams
                    data.NumBeams = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_BT_NUMBEAMS];

                    // ActualPingCount
                    data.ActualPingCount = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_BT_ACTUALPINGCOUNT];

                    // Range
                    JArray jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_BT_RANGE];
                    data.Range = new float[jArray.Count];
                    for (int x = 0; x < jArray.Count; x++)
                    {
                        // Add all the values to the array
                        data.Range[x] = (float)jArray[x];
                    }

                    // SNR
                    jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_BT_SNR];
                    data.SNR = new float[jArray.Count];
                    for (int x = 0; x < jArray.Count; x++)
                    {
                        // Add all the values to the array
                        data.SNR[x] = (float)jArray[x];
                    }

                    // Amplitude
                    jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_BT_AMPLITUDE];
                    data.Amplitude = new float[jArray.Count];
                    for (int x = 0; x < jArray.Count; x++)
                    {
                        // Add all the values to the array
                        data.Amplitude[x] = (float)jArray[x];
                    }

                    // Correlation
                    jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_BT_CORRELATION];
                    data.Correlation = new float[jArray.Count];
                    for (int x = 0; x < jArray.Count; x++)
                    {
                        // Add all the values to the array
                        data.Correlation[x] = (float)jArray[x];
                    }

                    // BeamVelocity
                    jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_BT_BEAMVELOCITY];
                    data.BeamVelocity = new float[jArray.Count];
                    for (int x = 0; x < jArray.Count; x++)
                    {
                        // Add all the values to the array
                        data.BeamVelocity[x] = (float)jArray[x];
                    }

                    // BeamGood
                    jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_BT_BEAMGOOD];
                    data.BeamGood = new float[jArray.Count];
                    for (int x = 0; x < jArray.Count; x++)
                    {
                        // Add all the values to the array
                        data.BeamGood[x] = (float)jArray[x];
                    }

                    // InstrumentVelocity
                    jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_BT_INSTRUMENTVELOCITY];
                    data.InstrumentVelocity = new float[jArray.Count];
                    for (int x = 0; x < jArray.Count; x++)
                    {
                        // Add all the values to the array
                        data.InstrumentVelocity[x] = (float)jArray[x];
                    }

                    // InstrumentGood
                    jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_BT_INSTRUMENTGOOD];
                    data.InstrumentGood = new float[jArray.Count];
                    for (int x = 0; x < jArray.Count; x++)
                    {
                        // Add all the values to the array
                        data.InstrumentGood[x] = (float)jArray[x];
                    }

                    // EarthVelocity
                    jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_BT_EARTHVELOCITY];
                    data.EarthVelocity = new float[jArray.Count];
                    for (int x = 0; x < jArray.Count; x++)
                    {
                        // Add all the values to the array
                        data.EarthVelocity[x] = (float)jArray[x];
                    }

                    // EarthGood
                    jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_BT_EARTHGOOD];
                    data.EarthGood = new float[jArray.Count];
                    for (int x = 0; x < jArray.Count; x++)
                    {
                        // Add all the values to the array
                        data.EarthGood[x] = (float)jArray[x];
                    }

                    // ShipVelocity
                    jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_BT_SHIPVELOCITY];
                    data.ShipVelocity = new float[jArray.Count];
                    for (int x = 0; x < jArray.Count; x++)
                    {
                        // Add all the values to the array
                        data.ShipVelocity[x] = (float)jArray[x];
                    }

                    // Pulse Coherent SNR
                    if (jsonObject.ContainsKey(DataSet.BaseDataSet.JSON_STR_BT_PC_SNR))
                    {
                        jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_BT_PC_SNR];
                        data.PulseCohr_Snr = new float[jArray.Count];
                        for (int x = 0; x < jArray.Count; x++)
                        {
                            // Add all the values to the array
                            data.PulseCohr_Snr[x] = (float)jArray[x];
                        }
                    }

                    // Pulse Coherent Amplitude
                    if (jsonObject.ContainsKey(DataSet.BaseDataSet.JSON_STR_BT_PC_AMP))
                    {
                        jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_BT_PC_AMP];
                        data.PulseCohr_Amp = new float[jArray.Count];
                        for (int x = 0; x < jArray.Count; x++)
                        {
                            // Add all the values to the array
                            data.PulseCohr_Amp[x] = (float)jArray[x];
                        }
                    }

                    // Pulse Coherent Velocity
                    if (jsonObject.ContainsKey(DataSet.BaseDataSet.JSON_STR_BT_PC_VEL))
                    {
                        jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_BT_PC_VEL];
                        data.PulseCohr_Vel = new float[jArray.Count];
                        for (int x = 0; x < jArray.Count; x++)
                        {
                            // Add all the values to the array
                            data.PulseCohr_Vel[x] = (float)jArray[x];
                        }
                    }

                    // Pulse Coherent Noise
                    if (jsonObject.ContainsKey(DataSet.BaseDataSet.JSON_STR_BT_PC_NOISE))
                    {
                        jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_BT_PC_NOISE];
                        data.PulseCohr_Noise = new float[jArray.Count];
                        for (int x = 0; x < jArray.Count; x++)
                        {
                            // Add all the values to the array
                            data.PulseCohr_Noise[x] = (float)jArray[x];
                        }
                    }

                    // Pulse Coherent Correlation
                    if (jsonObject.ContainsKey(DataSet.BaseDataSet.JSON_STR_BT_PC_CORR))
                    {
                        jArray = (JArray)jsonObject[DataSet.BaseDataSet.JSON_STR_BT_PC_CORR];
                        data.PulseCohr_Corr = new float[jArray.Count];
                        for (int x = 0; x < jArray.Count; x++)
                        {
                            // Add all the values to the array
                            data.PulseCohr_Corr[x] = (float)jArray[x];
                        }
                    }

                    if (jArray != null)
                    {
                        return data;
                    }
                }

                return null;
            }

            /// <summary>
            /// Check if the given object is the correct type.
            /// </summary>
            /// <param name="objectType">Object to convert.</param>
            /// <returns>TRUE = object given is the correct type.</returns>
            public override bool CanConvert(Type objectType)
            {
                return typeof(BottomTrackDataSet).IsAssignableFrom(objectType);
            }
        }
    }
}