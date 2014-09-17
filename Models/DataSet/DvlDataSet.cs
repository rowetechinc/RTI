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
 * 
 * HISTORY
 * -----------------------------------------------------------------
 * Date            Initials    Version    Comments
 * -----------------------------------------------------------------
 * 06/18/2014      RC          2.22.1     Initial coding
 * 
 * 
 */

using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;
using System.Text;


namespace RTI
{
    namespace DataSet
    {
        /// <summary>
        /// Data set containing all the Amplitude data.
        /// </summary>
        [JsonConverter(typeof(DvlDataSetSerializer))]
        public class DvlDataSet : BaseDataSet
        {
            #region Variables

            /// <summary>
            /// Number of elements in the data set.
            /// </summary>
            private const int NUM_ELEMENTS = 50;

            #endregion

            #region Properties

            #region Subsystem and Sample Number

            /// <summary>
            /// Subsystem configuration the data belong to.
            /// </summary>
            public SubsystemConfiguration SubsystemConfig { get; set; }

            /// <summary>
            /// Sample number.
            /// </summary>
            public int SampleNumber { get; set; }

            #endregion

            #region HPR and SoS

            /// <summary>
            /// System altitude.
            /// Pitch in degrees.
            /// </summary>
            public float Pitch { get; set; }

            /// <summary>
            /// System altitude.
            /// Roll in degrees.
            /// </summary>
            public float Roll { get; set; }

            /// <summary>
            /// System altitude.
            /// Heading in degrees.
            /// </summary>
            public float Heading { get; set; }

            /// <summary>
            /// Year for ensemble.
            /// </summary>
            public int Year { get; set; }

            /// <summary>
            /// Month for ensemble.
            /// </summary>
            public int Month { get; set; }

            /// <summary>
            /// Day for ensemble.
            /// </summary>
            public int Day { get; set; }

            /// <summary>
            /// Hour for ensemble.
            /// </summary>
            public int Hour { get; set; }

            /// <summary>
            /// Minute for ensemble.
            /// </summary>
            public int Minute { get; set; }

            /// <summary>
            /// Seconds for ensemble.
            /// </summary>
            public int Second { get; set; }

            /// <summary>
            /// Hundreth of a Second for ensemble.
            /// </summary>
            public int HSec { get; set; }

            /// <summary>
            /// Date and time of the ensemble.
            /// If the Year, Month or Day are not set, then this will
            /// return the current date and time.
            /// </summary>
            [JsonIgnore]
            public DateTime DateAndTime { get; set; }

            /// <summary>
            /// Salinity in parts per thousand (PPT).
            /// </summary>
            public float Salinity { get; set; }

            /// <summary>
            /// Temperature in C.
            /// </summary>
            public float Temperature { get; set; }

            /// <summary>
            /// Depth of the transducer in meters.
            /// </summary>
            public float DepthOfTransducer { get; set; }

            /// <summary>
            /// Speed of Sound in meters per second (m/s).
            /// </summary>
            public float SpeedOfSound { get; set; }

            /// <summary>
            /// Built in Test Result.
            /// </summary>
            public int BIT { get; set; }

            #endregion

            #region Reference Layer

            /// <summary>
            /// Reference layer minimum bin.
            /// </summary>
            public int RefLayerMin { get; set; }

            /// <summary>
            /// Reference layer maximum bin.
            /// </summary>
            public int RefLayerMax { get; set; }

            #endregion

            #region Water Mass Intrument Velocity

            /// <summary>
            /// Water Mass X Velocity in mm/s.
            /// Bm0 Bm1 XDCR movement relative to water mass.
            /// </summary>
            public float WmXVelocity { get; set; }

            /// <summary>
            /// Water Mass Y Velocity in mm/s.
            /// Bm2 Bm3 XDCR movement relative to water mass.
            /// </summary>
            public float WmYVelocity { get; set; }

            /// <summary>
            /// Water Mass Z Velocity in mm/s.
            /// Transducer movement away from water mass.
            /// </summary>
            public float WmZVelocity { get; set; }

            /// <summary>
            /// Water Mass Error Velocity in mm/s.
            /// </summary>
            public float WmErrorVelocity { get; set; }

            /// <summary>
            /// Water Mass Instrument Is Velocity good.
            /// </summary>
            public bool WmInstrumentIsGoodVelocity { get; set; }

            #endregion

            #region Water Mass Ship Velocity

            /// <summary>
            /// Water Mass Transverse (Y) Velocity in mm/s.
            /// Prt Stbd Ship movement relative to water mass.
            /// </summary>
            public float WmTransverseVelocity { get; set; }

            /// <summary>
            /// Water Mass Longitudinal (X) Velocity in mm/s.
            /// Aft Fwd Ship movement relative to water mass.
            /// </summary>
            public float WmLongitudinalVelocity { get; set; }

            /// <summary>
            /// Water Mass Normal (Z) Velocity in mm/s.
            /// Ship movement away from water mass.
            /// </summary>
            public float WmNormalVelocity { get; set; }

            /// <summary>
            /// Water Mass Ship Is Velocity good.
            /// </summary>
            public bool WmShipIsGoodVelocity { get; set; }

            #endregion

            #region Water Mass Earth Velocity

            /// <summary>
            /// Water Mass East (u-axis) Velocity in mm/s.
            /// </summary>
            public float WmEastVelocity { get; set; }

            /// <summary>
            /// Water Mass North (v-axis) Velocity in mm/s.
            /// </summary>
            public float WmNorthVelocity { get; set; }

            /// <summary>
            /// Water Mass Upward (w-axis) Velocity in mm/s.
            /// </summary>
            public float WmUpwardVelocity { get; set; }

            /// <summary>
            /// Water Mass Earth Is Velocity good.
            /// </summary>
            public bool WmEarthIsGoodVelocity { get; set; }

            #endregion

            #region Water Mass Earth Distance

            /// <summary>
            /// Water Mass East (u-axis) distance data in meters.
            /// </summary>
            public float WmEastDistance { get; set; }

            /// <summary>
            /// Water Mass North (v-axis) distance data in meters.
            /// </summary>
            public float WmNorthDistance { get; set; }

            /// <summary>
            /// Water Mass Upward (w-axis) distance data in meters.
            /// </summary>
            public float WmUpwardDistance { get; set; }

            /// <summary>
            /// Range to Water Mass center in meters. 
            /// </summary>
            public float WmEarthRangeToWaterMassCenter { get; set; }

            /// <summary>
            /// Water Mass Earth Time since last good velocity estimate
            /// in seconds.
            /// </summary>
            public float WmEarthTimeLastGoodVel { get; set; }

            #endregion

            #region Bottom Track Intrument Velocity

            /// <summary>
            /// Bottom Track X Velocity in mm/s.
            /// Bm0 Bm1 XDCR movement relative to water mass.
            /// </summary>
            public float BtXVelocity { get; set; }

            /// <summary>
            /// Bottom Track Y Velocity in mm/s.
            /// Bm2 Bm3 XDCR movement relative to water mass.
            /// </summary>
            public float BtYVelocity { get; set; }

            /// <summary>
            /// Bottom Track Z Velocity in mm/s.
            /// Transducer movement away from water mass.
            /// </summary>
            public float BtZVelocity { get; set; }

            /// <summary>
            /// Bottom Track Error Velocity in mm/s.
            /// </summary>
            public float BtErrorVelocity { get; set; }

            /// <summary>
            /// Bottom Track Instrument Is Velocity good.
            /// </summary>
            public bool BtInstrumentIsGoodVelocity { get; set; }

            #endregion

            #region Bottom Track Ship Velocity

            /// <summary>
            /// Bottom Track Transverse (Y) Velocity in mm/s.
            /// Prt Stbd Ship movement relative to water mass.
            /// </summary>
            public float BtTransverseVelocity { get; set; }

            /// <summary>
            /// Bottom Track Longitudinal (X) Velocity in mm/s.
            /// Aft Fwd Ship movement relative to water mass.
            /// </summary>
            public float BtLongitudinalVelocity { get; set; }

            /// <summary>
            /// Bottom Track Normal (Z) Velocity in mm/s.
            /// Ship movement away from water mass.
            /// </summary>
            public float BtNormalVelocity { get; set; }

            /// <summary>
            /// Bottom Track Ship Is Velocity good.
            /// </summary>
            public bool BtShipIsGoodVelocity { get; set; }

            #endregion

            #region Bottom Track Earth Velocity

            /// <summary>
            /// Bottom Track East (u-axis) Velocity in mm/s.
            /// </summary>
            public float BtEastVelocity { get; set; }

            /// <summary>
            /// Bottom Track North (v-axis) Velocity in mm/s.
            /// </summary>
            public float BtNorthVelocity { get; set; }

            /// <summary>
            /// Bottom Track Upward (w-axis) Velocity in mm/s.
            /// </summary>
            public float BtUpwardVelocity { get; set; }

            /// <summary>
            /// Bottom Track Earth Is Velocity good.
            /// </summary>
            public bool BtEarthIsGoodVelocity { get; set; }

            #endregion

            #region Bottom Track Earth Distance

            /// <summary>
            /// Bottom Track East (u-axis) distance data in meters.
            /// </summary>
            public float BtEastDistance { get; set; }

            /// <summary>
            /// Bottom Track North (v-axis) distance data in meters.
            /// </summary>
            public float BtNorthDistance { get; set; }

            /// <summary>
            /// Bottom Track Upward (w-axis) distance data in meters.
            /// </summary>
            public float BtUpwardDistance { get; set; }

            /// <summary>
            /// Bottom Track range to bottom in meters. 
            /// </summary>
            public float BtRangeToBottom { get; set; }

            /// <summary>
            /// Bottom Track Earth Time since last good velocity estimate
            /// in seconds.
            /// </summary>
            public float BtEarthTimeLastGoodVel { get; set; }

            #endregion

            #region Pressure and Range

            /// <summary>
            /// Pressure in kPa.
            /// </summary>
            public float Pressure { get; set; }

            /// <summary>
            /// Range to the bottom in deci-meter Beam 0.
            /// </summary>
            public float RangeBeam0 { get; set; }

            /// <summary>
            /// Range to the bottom in deci-meter Beam 1.
            /// </summary>
            public float RangeBeam1 { get; set; }

            /// <summary>
            /// Range to the bottom in deci-meter Beam 2.
            /// </summary>
            public float RangeBeam2 { get; set; }

            /// <summary>
            /// Range to the bottom in deci-meter Beam 3.
            /// </summary>
            public float RangeBeam3 { get; set; }

            /// <summary>
            /// Average range to bottom in deci-meter.
            /// </summary>
            public float AverageRange { get; set; }

            #endregion

            #endregion

            /// <summary>
            /// Create the DVL data set.
            /// </summary>
            public DvlDataSet() :
                        base(DataSet.Ensemble.DATATYPE_FLOAT,           // Type of data stored (Float or Int)
                        NUM_ELEMENTS,                                   // Number of bins
                        1,                                              // Number of beams
                        DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                        DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                        DataSet.Ensemble.DvlID)                         // Dataset ID
            {
                // Initialize
                Init();
            }


            /// <summary>
            /// Create an DVL data set.  Intended for JSON  deserialize.  This method
            /// is called when Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.DvlDataSet}(json) is
            /// called.
            /// 
            /// DeserializeObject is slightly faster then passing the string to the constructor.
            /// 97ms for this method.
            /// 181ms for JSON string constructor.
            /// 
            /// Alternative to decoding manually is to use the command:
            /// DataSet.BottomTrackDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.DvlDataSet}(json); 
            /// 
            /// To use this method for JSON you must have all the parameters match all the properties in this object.
            /// 
            /// </summary>
            /// <param name="SubsystemConfig">Subsystem Configuration.</param>
            /// <param name="SampleNumber">Sample number.</param>
            /// <param name="Heading">Heading in degrees.</param>
            /// <param name="Pitch">Pitch in degrees.</param>
            /// <param name="Roll">Roll in degrees.</param>
            /// <param name="Year">Year of the ensemble.</param>
            /// <param name="Month">Month of the ensemble.</param>
            /// <param name="Day">Day of the ensemble.</param>
            /// <param name="Hour">Hour of the ensemble.</param>
            /// <param name="Minute">Minute of the ensemble.</param>
            /// <param name="Second">Second of the ensemble.</param>
            /// <param name="HSec">Hundredth of second of the ensemble.</param>
            /// <param name="Salinity">Salinity of water in PPT.</param>
            /// <param name="Temperature">Tempeature in C.</param>
            /// <param name="DepthOfTransducer">Depth of Transducers.</param>
            /// <param name="SpeedOfSound">Speed of Sound measured in m/s.</param>
            /// <param name="BIT">BIT Result.</param>
            /// <param name="RefLayerMin">Reference Layer minimum.</param>
            /// <param name="RefLayerMax">Reference Layer maximum.</param>
            /// <param name="WmXVelocity">Water Mass X Velocity.</param>
            /// <param name="WmYVelocity">Water Mass Y Velocity.</param>
            /// <param name="WmZVelocity">Water Mass Z Velocity.</param>
            /// <param name="WmErrorVelocity">Water Mass Error Velocity.</param>
            /// <param name="WmInstrumentIsGoodVelocity">Water Mass Instrument Is Good Velocity.</param>
            /// <param name="WmTransverseVelocity">Water Mass Transverse Velocity.</param>
            /// <param name="WmLongitudinalVelocity">Water Mass Longitudinal Velocity.</param>
            /// <param name="WmNormalVelocity">Water Mass Normal Velocity.</param>
            /// <param name="WmShipIsGoodVelocity">Water Mass Ship Is Good Velocity.</param>
            /// <param name="WmEastVelocity">Water Mass East Velocity.</param>
            /// <param name="WmNorthVelocity">Water Mass North Velocity.</param>
            /// <param name="WmUpwardVelocity">Water Mass Upward Velocity.</param>
            /// <param name="WmEarthIsGoodVelocity">Water Mass Earth Is Good Velocity.</param>
            /// <param name="WmEastDistance">Water Mass East Distance.</param>
            /// <param name="WmNorthDistance">Water Mass North Distance.</param>
            /// <param name="WmUpwardDistance">Water Mass Upward Distance.</param>
            /// <param name="WmEarthRangeToWaterMassCenter">Water Mass Earth Range to Water Mass Center.</param>
            /// <param name="WmEarthTimeLastGoodVel">Water Mass Earth Time Last Good Velocity.</param>
            /// <param name="BtXVelocity">Bottom Track X Velocity.</param>
            /// <param name="BtYVelocity">Bottom Track Y Velocity.</param>
            /// <param name="BtZVelocity">Bottom Track Z Velocity.</param>
            /// <param name="BtErrorVelocity">Bottom Track Error Velocity.</param>
            /// <param name="BtInstrumentIsGoodVelocity">Bottom Track Instrument Is Good Velocity.</param>
            /// <param name="BtTransverseVelocity">Bottom Track Transverse Velocity.</param>
            /// <param name="BtLongitudinalVelocity">Bottom Track Longitudinal Velocity.</param>
            /// <param name="BtNormalVelocity">Bottom Track Normal Velocity.</param>
            /// <param name="BtShipIsGoodVelocity">Bottom Track Ship Is Good Velocity.</param>
            /// <param name="BtEastVelocity">Bottom Track East Velocity.</param>
            /// <param name="BtNorthVelocity">Bottom Track North Velocity.</param>
            /// <param name="BtUpwardVelocity">Bottom Track Upward Velocity.</param>
            /// <param name="BtEarthIsGoodVelocity">Bottom Track Earth Is Good Velocity.</param>
            /// <param name="BtEastDistance">Bottom Track East Distance.</param>
            /// <param name="BtNorthDistance">Bottom Track North Distance.</param>
            /// <param name="BtUpwardDistance">Bottom Track Upward Distance.</param>
            /// <param name="BtEarthRangeToWaterMassCenter">Bottom Track Earth Range to Water Mass Center.</param>
            /// <param name="BtEarthTimeLastGoodVel">Bottom Track Earth Time Last Good Velocity.</param>
            /// <param name="Pressure">Pressure in kPa.</param>
            /// <param name="RangeBeam0">Range of Beam 0.</param>
            /// <param name="RangeBeam1">Range of Beam 1.</param>
            /// <param name="RangeBeam2">Range of Beam 2.</param>
            /// <param name="RangeBeam3">Range of Beam 3.</param>
            /// <param name="AverageRange">Average Range.</param>
            [JsonConstructor]
            public DvlDataSet( SubsystemConfiguration SubsystemConfig, int SampleNumber,
                        float Heading, float Pitch, float Roll,
                        int Year, int Month, int Day, int Hour, int Minute, int Second, int HSec,
                        float Salinity, float Temperature, float DepthOfTransducer, float SpeedOfSound, int BIT,
                        int RefLayerMin, int RefLayerMax,
                        float WmXVelocity, float WmYVelocity, float WmZVelocity, float WmErrorVelocity, bool WmInstrumentIsGoodVelocity,
                        float WmTransverseVelocity, float WmLongitudinalVelocity, float WmNormalVelocity, bool WmShipIsGoodVelocity,
                        float WmEastVelocity, float WmNorthVelocity, float WmUpwardVelocity, bool WmEarthIsGoodVelocity,
                        float WmEastDistance, float WmNorthDistance, float WmUpwardDistance, float WmEarthRangeToWaterMassCenter, float WmEarthTimeLastGoodVel,
                        float BtXVelocity, float BtYVelocity, float BtZVelocity, float BtErrorVelocity, bool BtInstrumentIsGoodVelocity,
                        float BtTransverseVelocity, float BtLongitudinalVelocity, float BtNormalVelocity, bool BtShipIsGoodVelocity,
                        float BtEastVelocity, float BtNorthVelocity, float BtUpwardVelocity, bool BtEarthIsGoodVelocity,
                        float BtEastDistance, float BtNorthDistance, float BtUpwardDistance, float BtEarthRangeToWaterMassCenter, float BtEarthTimeLastGoodVel,
                        float Pressure, float RangeBeam0, float RangeBeam1, float RangeBeam2, float RangeBeam3, float AverageRange) :
                base(DataSet.Ensemble.DATATYPE_FLOAT,                   // Type of data stored (Float or Int)
                        NUM_ELEMENTS,                                   // Number of bins
                        1,                                              // Number of beams
                        DataSet.Ensemble.DEFAULT_IMAG,                  // Default Image
                        DataSet.Ensemble.DEFAULT_NAME_LENGTH,           // Default Image length
                        DataSet.Ensemble.DvlID)                         // Dataset ID
            {
                this.SubsystemConfig = SubsystemConfig;
                this.SampleNumber = SampleNumber;

                this.Pitch = Pitch;
                this.Roll = Roll;
                this.Heading = Heading;
                this.DateAndTime = DateAndTime;
                this.Salinity = Salinity;
                this.Temperature = Temperature;
                this.DepthOfTransducer = DepthOfTransducer;
                this.SpeedOfSound = SpeedOfSound;
                this.BIT = BIT;

                // Set time
                this.Year = Year;
                this.Month = Month;
                this.Day = Day;
                this.Hour = Hour;
                this.Minute = Minute;
                this.Second = Second;
                this.HSec = HSec;
                ValidateDateTime(Year, Month, Day, Hour, Minute, Second, HSec / 10);

                this.RefLayerMin = RefLayerMin;
                this.RefLayerMax = RefLayerMax;

                this.WmXVelocity = WmXVelocity;
                this.WmYVelocity = WmYVelocity;
                this.WmZVelocity = WmZVelocity;
                this.WmErrorVelocity = WmErrorVelocity;
                this.WmInstrumentIsGoodVelocity = WmInstrumentIsGoodVelocity;

                this.WmTransverseVelocity = WmTransverseVelocity;
                this.WmLongitudinalVelocity = WmLongitudinalVelocity;
                this.WmNormalVelocity = WmNormalVelocity;
                this.WmShipIsGoodVelocity = WmShipIsGoodVelocity;

                this.WmEastVelocity = WmEastVelocity;
                this.WmNorthVelocity = WmNorthVelocity;
                this.WmUpwardVelocity = WmUpwardVelocity;
                this.WmEarthIsGoodVelocity = WmEarthIsGoodVelocity;

                this.WmEastDistance = WmEastDistance;
                this.WmNorthDistance = WmNorthDistance;
                this.WmUpwardDistance = WmUpwardDistance;
                this.WmEarthRangeToWaterMassCenter = WmEarthRangeToWaterMassCenter;
                this.WmEarthTimeLastGoodVel = WmEarthTimeLastGoodVel;

                this.BtXVelocity = BtXVelocity;
                this.BtYVelocity = BtYVelocity;
                this.BtZVelocity = BtZVelocity;
                this.BtErrorVelocity = BtErrorVelocity;
                this.BtInstrumentIsGoodVelocity = BtInstrumentIsGoodVelocity;

                this.BtTransverseVelocity = BtTransverseVelocity;
                this.BtLongitudinalVelocity = BtLongitudinalVelocity;
                this.BtNormalVelocity = BtNormalVelocity;
                this.BtShipIsGoodVelocity = BtShipIsGoodVelocity;

                this.BtEastVelocity = BtEastVelocity;
                this.BtNorthVelocity = BtNorthVelocity;
                this.BtUpwardVelocity = BtUpwardVelocity;
                this.BtEarthIsGoodVelocity = BtEarthIsGoodVelocity;

                this.BtEastDistance = BtEastDistance;
                this.BtNorthDistance = BtNorthDistance;
                this.BtUpwardDistance = BtUpwardDistance;
                this.BtRangeToBottom = BtEarthRangeToWaterMassCenter;
                this.BtEarthTimeLastGoodVel = BtEarthTimeLastGoodVel;

                this.Pressure = Pressure;
                this.RangeBeam0 = RangeBeam0;
                this.RangeBeam1 = RangeBeam1;
                this.RangeBeam2 = RangeBeam2;
                this.RangeBeam3 = RangeBeam3;
                this.AverageRange = AverageRange;
            }

            #region Init

            /// <summary>
            /// Initialize the values.
            /// </summary>
            private void Init()
            {
                // Create Subsystem Configuration based off Firmware and Serialnumber
                SubsystemConfig = new SubsystemConfiguration(new Firmware().GetSubsystem(SerialNumber.DVL), 0, 0);
                SampleNumber = 0;

                Pitch = 0.0f;
                Roll = 0.0f;
                Heading = 0.0f;
                DateAndTime = DateTime.Now;
                Year = DateAndTime.Year;
                Month = DateAndTime.Month;
                Day = DateAndTime.Day;
                Hour = DateAndTime.Hour;
                Minute = DateAndTime.Minute;
                Second = DateAndTime.Second;
                HSec = DateAndTime.Millisecond * 10;
                Salinity = 0.0f;
                Temperature = 0.0f;
                DepthOfTransducer = 0.0f;
                SpeedOfSound = 0.0f;
                BIT = 0;

                RefLayerMin = 0;
                RefLayerMax = 0;

                WmXVelocity = 0.0f;
                WmYVelocity = 0.0f;
                WmZVelocity = 0.0f;
                WmErrorVelocity = 0.0f;
                WmInstrumentIsGoodVelocity = false;

                WmTransverseVelocity = 0.0f;
                WmLongitudinalVelocity = 0.0f;
                WmNormalVelocity = 0.0f;
                WmShipIsGoodVelocity = false;

                WmEastVelocity = 0.0f;
                WmNorthVelocity = 0.0f;
                WmUpwardVelocity = 0.0f;
                WmEarthIsGoodVelocity = false;

                WmEastDistance = 0.0f;
                WmNorthDistance = 0.0f;
                WmUpwardDistance = 0.0f;
                WmEarthRangeToWaterMassCenter = 0.0f;
                WmEarthTimeLastGoodVel = 0.0f;

                BtXVelocity = 0.0f;
                BtYVelocity = 0.0f;
                BtZVelocity = 0.0f;
                BtErrorVelocity = 0.0f;
                BtInstrumentIsGoodVelocity = false;

                BtTransverseVelocity = 0.0f;
                BtLongitudinalVelocity = 0.0f;
                BtNormalVelocity = 0.0f;
                BtShipIsGoodVelocity = false;

                BtEastVelocity = 0.0f;
                BtNorthVelocity = 0.0f;
                BtUpwardVelocity = 0.0f;
                BtEarthIsGoodVelocity = false;

                BtEastDistance = 0.0f;
                BtNorthDistance = 0.0f;
                BtUpwardDistance = 0.0f;
                BtRangeToBottom = 0.0f;
                BtEarthTimeLastGoodVel = 0.0f;

                Pressure = 0.0f;
                RangeBeam0 = 0.0f;
                RangeBeam1 = 0.0f;
                RangeBeam2 = 0.0f;
                RangeBeam3 = 0.0f;
                AverageRange = 0.0f;
            }

            /// <summary>
            /// Try to set the date and time.  If any values are out of range, an exception
            /// will be thrown and the date and time DateTime.MinValue will be set.
            /// </summary>
            /// <param name="year">Year</param>
            /// <param name="month">Month</param>
            /// <param name="day">Day</param>
            /// <param name="hour">Hour</param>
            /// <param name="min">Minute</param>
            /// <param name="second">Second</param>
            /// <param name="millisec">Milliseconds</param>
            private void ValidateDateTime(int year, int month, int day, int hour, int min, int second, int millisec)
            {
                try
                {
                    DateAndTime = new DateTime(year, month, day, hour, min, second, millisec);
                }
                // Paramater out of range
                catch (ArgumentOutOfRangeException)
                {
                    DateAndTime = new DateTime();
                    DateAndTime = DateTime.MinValue;
                    //Debug.WriteLine(string.Format("Y{0} M{1} D{2} : H{3} M{4} S{5} MS{6}", year, month, day, hour, min, second, millisec));
                }
                // The specified parameters evaluate to less than DateTime.MinValue or more than DateTime.MaxValue
                catch (ArgumentException)
                {
                    DateAndTime = new DateTime();
                    DateAndTime = DateTime.MinValue;
                }
            }

            #endregion

            #region ToString

            /// <summary>
            /// Convert the object to a string.
            /// </summary>
            /// <returns>String of the object.</returns>
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(string.Format("SubsystemConfig = {0}", SubsystemConfig.DescString()));
                sb.AppendLine(string.Format("Sample Number = {0}", SampleNumber));

                sb.AppendLine();
                sb.AppendLine(string.Format("Heading = {0}", Heading));
                sb.AppendLine(string.Format("Pitch = {0}", Pitch));
                sb.AppendLine(string.Format("Roll = {0}", Roll));
                sb.AppendLine(string.Format("DateAndTime = {0}", DateAndTime.ToString()));
                sb.AppendLine(string.Format("Salinity = {0}", Salinity));
                sb.AppendLine(string.Format("Temperature = {0}", Temperature));
                sb.AppendLine(string.Format("DepthOfTransducer = {0}",  DepthOfTransducer));
                sb.AppendLine(string.Format("SpeedOfSound = {0}", SpeedOfSound));
                sb.AppendLine(string.Format("BIT = {0}", BIT));

                sb.AppendLine();
                sb.AppendLine(string.Format("RefLayerMin = {0}", RefLayerMin));
                sb.AppendLine(string.Format("RefLayerMax = {0}", RefLayerMax));

                sb.AppendLine();
                sb.AppendLine(string.Format("WmXVelocity = {0}", WmXVelocity));
                sb.AppendLine(string.Format("WmYVelocity = {0}", WmYVelocity));
                sb.AppendLine(string.Format("WmZVelocity = {0}", WmZVelocity));
                sb.AppendLine(string.Format("WmErrorVelocity = {0}", WmErrorVelocity));
                sb.AppendLine(string.Format("WmInstrumentIsGoodVelocity = {0}", WmInstrumentIsGoodVelocity));

                sb.AppendLine();
                sb.AppendLine(string.Format("WmTransverseVelocity = {0}", WmTransverseVelocity));
                sb.AppendLine(string.Format("WmLongitudinalVelocity = {0}", WmLongitudinalVelocity));
                sb.AppendLine(string.Format("WmNormalVelocity = {0}", WmNormalVelocity));
                sb.AppendLine(string.Format("WmShipIsGoodVelocity = {0}", WmShipIsGoodVelocity));

                sb.AppendLine();
                sb.AppendLine(string.Format("WmEastVelocity = {0}", WmEastVelocity));
                sb.AppendLine(string.Format("WmNorthVelocity = {0}", WmNorthVelocity));
                sb.AppendLine(string.Format("WmUpwardVelocity = {0}", WmUpwardVelocity));
                sb.AppendLine(string.Format("WmEarthIsGoodVelocity = {0}", WmEarthIsGoodVelocity));

                sb.AppendLine();
                sb.AppendLine(string.Format("WmEastDistance = {0}", WmEastDistance));
                sb.AppendLine(string.Format("WmNorthDistance = {0}", WmNorthDistance));
                sb.AppendLine(string.Format("WmUpwardDistance = {0}", WmUpwardDistance));
                sb.AppendLine(string.Format("WmEarthRangeToWaterMassCenter = {0}", WmEarthRangeToWaterMassCenter));
                sb.AppendLine(string.Format("WmEarthTimeLastGoodVel = {0}", WmEarthTimeLastGoodVel));

                sb.AppendLine();
                sb.AppendLine(string.Format("BtXVelocity = {0}", BtXVelocity));
                sb.AppendLine(string.Format("BtYVelocity = {0}", BtYVelocity));
                sb.AppendLine(string.Format("BtZVelocity = {0}", BtZVelocity));
                sb.AppendLine(string.Format("BtErrorVelocity = {0}", BtErrorVelocity));
                sb.AppendLine(string.Format("BtInstrumentIsGoodVelocity = {0}", BtInstrumentIsGoodVelocity));

                sb.AppendLine();
                sb.AppendLine(string.Format("BtTransverseVelocity = {0}", BtTransverseVelocity));
                sb.AppendLine(string.Format("BtLongitudinalVelocity = {0}", BtLongitudinalVelocity));
                sb.AppendLine(string.Format("BtNormalVelocity = {0}", BtNormalVelocity));
                sb.AppendLine(string.Format("BtShipIsGoodVelocity = {0}", BtShipIsGoodVelocity));

                sb.AppendLine();
                sb.AppendLine(string.Format("BtEastVelocity = {0}", BtEastVelocity));
                sb.AppendLine(string.Format("BtNorthVelocity = {0}", BtNorthVelocity));
                sb.AppendLine(string.Format("BtUpwardVelocity = {0}", BtUpwardVelocity));
                sb.AppendLine(string.Format("BtEarthIsGoodVelocity = {0}", BtEarthIsGoodVelocity));

                sb.AppendLine();
                sb.AppendLine(string.Format("BtEastDistance = {0}", BtEastDistance));
                sb.AppendLine(string.Format("BtNorthDistance = {0}", BtNorthDistance));
                sb.AppendLine(string.Format("BtUpwardDistance = {0}", BtUpwardDistance));
                sb.AppendLine(string.Format("BtEarthRangeToWaterMassCenter = {0}", BtRangeToBottom));
                sb.AppendLine(string.Format("BtEarthTimeLastGoodVel = {0}", BtEarthTimeLastGoodVel));

                sb.AppendLine();
                sb.AppendLine(string.Format("Pressure = {0}", Pressure));
                sb.AppendLine(string.Format("RangeBeam0 = {0}", RangeBeam0));
                sb.AppendLine(string.Format("RangeBeam1 = {0}", RangeBeam1));
                sb.AppendLine(string.Format("RangeBeam2 = {0}", RangeBeam2));
                sb.AppendLine(string.Format("RangeBeam3 = {0}", RangeBeam3));
                sb.AppendLine(string.Format("AverageRange = {0}", AverageRange));

                return sb.ToString();
            }

            #endregion
        }

        /// <summary>
        /// Convert this object to a JSON object.
        /// Calling this method is twice as fast as calling the default serializer:
        /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.DvlData).
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
        public class DvlDataSetSerializer : JsonConverter
        {
            /// <summary>
            /// Write the JSON string.  This will convert all the properties to a JSON string.
            /// This is done manaully to improve conversion time.  The default serializer will check
            /// each property if it can convert.  This will convert the properties automatically.  This
            /// will double the speed.
            /// 
            /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.DvlData).
            /// 
            /// </summary>
            /// <param name="writer">JSON Writer.</param>
            /// <param name="value">Object to write to JSON.</param>
            /// <param name="serializer">Serializer to convert the object.</param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // Cast the object
                var data = value as DvlDataSet;

                // Start the object
                writer.Formatting = Formatting.None;            // Make the text not indented, so not as human readable.  This will save disk space
                writer.WriteStartObject();                      // Start the JSON object

                // Write the base values
                writer.WriteRaw(data.ToJsonBaseStub());
                writer.WriteRaw(",");

                #region System

                // Subsystem Index
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SUBSYSTEM_INDEX);
                writer.WriteValue(data.SubsystemConfig.SubSystem.Index);

                // Subsystem Code
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SUBSYSTEM_CODE);
                writer.WriteValue(data.SubsystemConfig.SubSystem.Code);

                // CEPO Index
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_CEPOINDEX);
                writer.WriteValue(data.SubsystemConfig.CepoIndex);

                // SubsystemConfiguration Index
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SSCONFIG_INDEX);
                writer.WriteValue(data.SubsystemConfig.SubsystemConfigIndex);

                // Sample Number
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_SAMPLENUMBER);
                writer.WriteValue(data.SampleNumber);

                // Pitch
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_PITCH);
                writer.WriteValue(data.Pitch);

                // Roll
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_ROLL);
                writer.WriteValue(data.Roll);

                // Heading
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_HEADING);
                writer.WriteValue(data.Heading);

                // Year
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_YEAR);
                writer.WriteValue(data.Year);

                // Month
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_MONTH);
                writer.WriteValue(data.Month);

                // Day
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DAY);
                writer.WriteValue(data.Day);

                // Hour
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_HOUR);
                writer.WriteValue(data.Hour);

                // Minute
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_MINUTE);
                writer.WriteValue(data.Minute);

                // Second
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SECOND);
                writer.WriteValue(data.Second);

                // HSec
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_HSEC);
                writer.WriteValue(data.HSec);

                // Salinity
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SALINITY);
                writer.WriteValue(data.Salinity);

                // Temperature
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_TEMPERATURE);
                writer.WriteValue(data.Temperature);

                // Depth of Transducer
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_TRANSDUCERDEPTH);
                writer.WriteValue(data.DepthOfTransducer);

                // Speed Of Sound
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SPEEDOFSOUND);
                writer.WriteValue(data.SpeedOfSound);

                // BIT
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_BIT);
                writer.WriteValue(data.BIT);

                // Ref Layer Min
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_REF_LAYER_MIN);
                writer.WriteValue(data.RefLayerMin);

                // Ref Layer Max
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_REF_LAYER_MAX);
                writer.WriteValue(data.RefLayerMax);

                #endregion

                #region Water Mass Instrument Vel

                // WM X Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_WM_X_VEL);
                writer.WriteValue(data.WmXVelocity);

                // WM Y Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_WM_Y_VEL);
                writer.WriteValue(data.WmYVelocity);

                // WM Z Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_WM_Z_VEL);
                writer.WriteValue(data.WmZVelocity);

                // WM Error Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_WM_ERROR_VEL);
                writer.WriteValue(data.WmErrorVelocity);

                // WM Instrument Is Good Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_WM_INSTRUMENT_IS_GOOD_VEL);
                writer.WriteValue(data.WmInstrumentIsGoodVelocity);

                #endregion

                #region Water Mass Ship Vel

                // WM Transverse Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_WM_TRANSVERSE_VEL);
                writer.WriteValue(data.WmTransverseVelocity);

                // WM Longitudinal Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_WM_LONGITUDINAL_VEL);
                writer.WriteValue(data.WmLongitudinalVelocity);

                // WM Normal Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_WM_NORMAL_VEL);
                writer.WriteValue(data.WmNormalVelocity);

                // WM Ship Is Good Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_WM_SHIP_IS_GOOD_VEL);
                writer.WriteValue(data.WmShipIsGoodVelocity);

                #endregion

                #region Water Mass Earth Vel

                // WM East Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_WM_EAST_VEL);
                writer.WriteValue(data.WmEastVelocity);

                // WM North Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_WM_NORTH_VEL);
                writer.WriteValue(data.WmNorthVelocity);

                // WM Upward Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_WM_UPWARD_VEL);
                writer.WriteValue(data.WmUpwardVelocity);

                // WM Earth Is Good Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_WM_EARTH_IS_GOOD_VEL);
                writer.WriteValue(data.WmEarthIsGoodVelocity);

                #endregion

                #region Water Mass Earth Distance

                // WM East Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_WM_EAST_DMG);
                writer.WriteValue(data.WmEastDistance);

                // WM North Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_WM_NORTH_DMG);
                writer.WriteValue(data.WmNorthDistance);

                // WM Upward Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_WM_UPWARD_DMG);
                writer.WriteValue(data.WmUpwardDistance);

                // WM Earth Range to WM Center
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_WM_EARTH_RANGE_TO_WM_CENTER);
                writer.WriteValue(data.WmEarthRangeToWaterMassCenter);

                // WM Earth Is Good Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_WM_EARTH_TIME_LAST_GOOD_VEL);
                writer.WriteValue(data.WmEarthTimeLastGoodVel);

                #endregion

                #region Bottom Track Instrument Vel

                // Bottom Track X Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_BT_X_VEL);
                writer.WriteValue(data.BtXVelocity);

                // Bottom Track Y Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_BT_Y_VEL);
                writer.WriteValue(data.BtYVelocity);

                // Bottom Track Z Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_BT_Z_VEL);
                writer.WriteValue(data.BtZVelocity);

                // Bottom Track Error Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_BT_ERROR_VEL);
                writer.WriteValue(data.BtErrorVelocity);

                // Bottom Track Instrument Is Good Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_BT_INSTRUMENT_IS_GOOD_VEL);
                writer.WriteValue(data.BtInstrumentIsGoodVelocity);

                #endregion

                #region Bottom Track Ship Vel

                // Bottom Track Transverse Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_BT_TRANSVERSE_VEL);
                writer.WriteValue(data.BtTransverseVelocity);

                // Bottom Track Longitudinal Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_BT_LONGITUDINAL_VEL);
                writer.WriteValue(data.BtLongitudinalVelocity);

                // Bottom Track Normal Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_BT_NORMAL_VEL);
                writer.WriteValue(data.BtNormalVelocity);

                // Bottom Track Ship Is Good Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_BT_SHIP_IS_GOOD_VEL);
                writer.WriteValue(data.BtShipIsGoodVelocity);

                #endregion

                #region Bottom Track Earth Vel

                // Bottom Track East Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_BT_EAST_VEL);
                writer.WriteValue(data.BtEastVelocity);

                // Bottom Track North Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_BT_NORTH_VEL);
                writer.WriteValue(data.BtNorthVelocity);

                // Bottom Track Upward Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_BT_UPWARD_VEL);
                writer.WriteValue(data.BtUpwardVelocity);

                // Bottom Track Earth Is Good Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_BT_EARTH_IS_GOOD_VEL);
                writer.WriteValue(data.BtEarthIsGoodVelocity);

                #endregion

                #region Bottom Track Earth Distance

                // Bottom Track East Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_BT_EAST_DMG);
                writer.WriteValue(data.BtEastDistance);

                // Bottom Track North Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_BT_NORTH_DMG);
                writer.WriteValue(data.BtNorthDistance);

                // Bottom Track Upward Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_BT_UPWARD_DMG);
                writer.WriteValue(data.BtUpwardDistance);

                // Bottom Track Earth Range to WM Center
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_BT_EARTH_RANGE_TO_WM_CENTER);
                writer.WriteValue(data.BtRangeToBottom);

                // Bottom Track Earth Is Good Vel
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_BT_EARTH_TIME_LAST_GOOD_VEL);
                writer.WriteValue(data.BtEarthTimeLastGoodVel);

                #endregion

                #region Pressure and Range

                // Pressure
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_PRESSURE);
                writer.WriteValue(data.Pressure);

                // Range 0
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_RANGE_B0);
                writer.WriteValue(data.RangeBeam0);

                // Range 1
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_RANGE_B1);
                writer.WriteValue(data.RangeBeam1);

                // Range 2
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_RANGE_B2);
                writer.WriteValue(data.RangeBeam2);

                // Range 3
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_RANGE_B3);
                writer.WriteValue(data.RangeBeam3);

                // Average Range 
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DVL_AVG_RANGE);
                writer.WriteValue(data.AverageRange);

                #endregion

                // End the object
                writer.WriteEndObject();
            }

            /// <summary>
            /// Read the JSON object and convert to the object.  This will allow the serializer to
            /// automatically convert the object.  No special instructions need to be done and all
            /// the properties found in the JSON string need to be used.
            /// 
            /// DataSet.DvlDataSet decodedEns = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.DvlDataSet}(encodedEns)
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

                    // SubsystemConfig
                    ushort index = (ushort)jsonObject[DataSet.BaseDataSet.JSON_STR_SUBSYSTEM_INDEX];
                    byte ssCode = (byte)jsonObject[DataSet.BaseDataSet.JSON_STR_SUBSYSTEM_CODE].ToObject<byte>();
                    byte cepoIndex = (byte)jsonObject[DataSet.BaseDataSet.JSON_STR_CEPOINDEX].ToObject<byte>();
                    byte configNum = (byte)jsonObject[DataSet.BaseDataSet.JSON_STR_SSCONFIG_INDEX].ToObject<byte>();
                    SubsystemConfiguration SubsystemConfig = new SubsystemConfiguration(new Subsystem(ssCode, index), cepoIndex, configNum);

                    #region System

                    // Sample Number
                    int SampleNumber = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_SAMPLENUMBER];

                    // Heading
                    float Heading = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_HEADING];

                    // Pitch
                    float Pitch = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_PITCH];

                    // Roll
                    float Roll = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_ROLL];

                    // Year
                    int Year = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_YEAR];

                    // Month
                    int Month = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_MONTH];

                    // Day
                    int Day = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_DAY];

                    // Hour
                    int Hour = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_HOUR];

                    // Minute
                    int Minute = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_MINUTE];

                    // Second
                    int Second = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_SECOND];

                    // HSec
                    int HSec = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_HSEC];

                    // Salinity
                    float Salinity = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SALINITY];

                    // Salinity
                    float Temperature = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_TEMPERATURE];

                    // TransducerDepth
                    float TransducerDepth = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_TRANSDUCERDEPTH];

                    // SpeedOfSound
                    float SpeedOfSound = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_SPEEDOFSOUND];

                    // BIT
                    int BIT = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_BIT];

                    // Ref Layer Min
                    int RefLayerMin = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_REF_LAYER_MIN];

                    // Ref Layer Max
                    int RefLayerMax = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_REF_LAYER_MAX];

                    #endregion

                    #region Water Mass Instrument Velocity

                    // WM X Vel 
                    float WmXVel = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_WM_X_VEL];

                    // WM Y Vel 
                    float WmYVel = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_WM_Y_VEL];

                    // WM Z Vel 
                    float WmZVel = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_WM_Z_VEL];

                    // WM Error Vel 
                    float WmErrorVel = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_WM_ERROR_VEL];

                    // WM Instrumment Is Good Vel 
                    bool WmInstrumentIsGoodVel = (bool)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_WM_INSTRUMENT_IS_GOOD_VEL];

                    #endregion

                    #region Water Mass Ship Velocity

                    // WM Transverse Vel 
                    float WmTransverseVel = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_WM_TRANSVERSE_VEL];

                    // WM Longitudinal Vel 
                    float WmLongitudinalVel = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_WM_LONGITUDINAL_VEL];

                    // WM Normal Vel 
                    float WmNormalVel = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_WM_NORMAL_VEL];

                    // WM Ship Is Good Vel 
                    bool WmShipIsGoodVel = (bool)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_WM_SHIP_IS_GOOD_VEL];

                    #endregion

                    #region Water Mass Earth Velocity

                    // WM East Vel 
                    float WmEastVel = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_WM_EAST_VEL];

                    // WM North Vel 
                    float WmNorthVel = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_WM_NORTH_VEL];

                    // WM Upward Vel 
                    float WmUpwardVel = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_WM_UPWARD_VEL];

                    // WM Earth Is Good Vel 
                    bool WmEarthIsGoodVel = (bool)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_WM_EARTH_IS_GOOD_VEL];

                    #endregion

                    #region Water Mass Earth Distance

                    // WM East Vel 
                    float WmEastDist = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_WM_EAST_DMG];

                    // WM North Vel 
                    float WmNorthDist = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_WM_NORTH_DMG];

                    // WM Upward Vel 
                    float WmUpwardDist = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_WM_UPWARD_DMG];

                    // WM Earth Range to WM center
                    float WmEarthRangeToWaterMassCenter = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_WM_EARTH_RANGE_TO_WM_CENTER];

                    // WM Earth Range to WM center
                    float WmEarthTimeLastGoodVel = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_WM_EARTH_TIME_LAST_GOOD_VEL];

                    #endregion

                    #region Bottom Track Instrument Velocity

                    // Bottom Track X Vel 
                    float BtXVel = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_BT_X_VEL];

                    // Bottom Track Y Vel 
                    float BtYVel = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_BT_Y_VEL];

                    // Bottom Track Z Vel 
                    float BtZVel = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_BT_Z_VEL];

                    // Bottom Track Error Vel 
                    float BtErrorVel = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_BT_ERROR_VEL];

                    // Bottom Track Instrumment Is Good Vel 
                    bool BtInstrumentIsGoodVel = (bool)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_BT_INSTRUMENT_IS_GOOD_VEL];

                    #endregion

                    #region Bottom Track Ship Velocity

                    // Bottom Track Transverse Vel 
                    float BtTransverseVel = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_BT_TRANSVERSE_VEL];

                    // Bottom Track Longitudinal Vel 
                    float BtLongitudinalVel = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_BT_LONGITUDINAL_VEL];

                    // Bottom Track Normal Vel 
                    float BtNormalVel = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_BT_NORMAL_VEL];

                    // Bottom Track Ship Is Good Vel 
                    bool BtShipIsGoodVel = (bool)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_BT_SHIP_IS_GOOD_VEL];

                    #endregion

                    #region Bottom Track Earth Velocity

                    // Bottom Track East Vel 
                    float BtEastVel = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_BT_EAST_VEL];

                    // Bottom Track North Vel 
                    float BtNorthVel = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_BT_NORTH_VEL];

                    // Bottom Track Upward Vel 
                    float BtUpwardVel = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_BT_UPWARD_VEL];

                    // Bottom Track Earth Is Good Vel 
                    bool BtEarthIsGoodVel = (bool)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_BT_EARTH_IS_GOOD_VEL];

                    #endregion

                    #region Bottom Track Earth Distance

                    // Bottom Track East Distance 
                    float BtEastDist = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_BT_EAST_DMG];

                    // Bottom Track North Distance 
                    float BtNorthDist = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_BT_NORTH_DMG];

                    // Bottom Track Upward Distance 
                    float BtUpwardDist = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_BT_UPWARD_DMG];

                    // Bottom Track Earth Range to WM center
                    float BtEarthRangeToWaterMassCenter = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_BT_EARTH_RANGE_TO_WM_CENTER];

                    // Bottom Track Earth Range to WM center
                    float BtEarthTimeLastGoodVel = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_BT_EARTH_TIME_LAST_GOOD_VEL];

                    #endregion

                    #region Pressure and Range

                    // Pressure
                    float Pressure = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_BT_EAST_DMG];

                    // Range Beam 0
                    float RangeB0 = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_RANGE_B0];

                    // Range Beam 1 
                    float RangeB1 = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_RANGE_B1];

                    // Range Beam 2
                    float RangeB2 = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_RANGE_B2];

                    // Range Beam 3
                    float RangeB3 = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_RANGE_B3];

                    // Average Range
                    float AvgRange = (float)jsonObject[DataSet.BaseDataSet.JSON_STR_DVL_AVG_RANGE];

                    #endregion

                    // Create the object
                    var data = new DvlDataSet(SubsystemConfig, SampleNumber, Heading, Pitch, Roll,
                                                Year, Month, Day, Hour, Minute, Second, HSec,
                                                Salinity, Temperature, TransducerDepth, SpeedOfSound, BIT,
                                                RefLayerMin, RefLayerMax,
                                                WmXVel, WmYVel, WmZVel, WmErrorVel, WmInstrumentIsGoodVel,
                                                WmTransverseVel, WmLongitudinalVel, WmNormalVel, WmShipIsGoodVel,
                                                WmEastVel, WmNorthVel, WmUpwardVel, WmEarthIsGoodVel,
                                                WmEastDist, WmNorthDist, WmUpwardDist, WmEarthRangeToWaterMassCenter, WmEarthTimeLastGoodVel,
                                                BtXVel, BtYVel, BtZVel, BtErrorVel, BtInstrumentIsGoodVel,
                                                BtTransverseVel, BtLongitudinalVel, BtNormalVel, WmShipIsGoodVel,
                                                BtEastVel, BtNorthVel, BtUpwardVel, BtEarthIsGoodVel,
                                                BtEastDist, BtNorthDist, BtUpwardDist, BtEarthRangeToWaterMassCenter, BtEarthTimeLastGoodVel,
                                                Pressure, RangeB0, RangeB1, RangeB2, RangeB3, AvgRange);

                    return data;
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
                return typeof(AmplitudeDataSet).IsAssignableFrom(objectType);
            }
        }

    }
}
