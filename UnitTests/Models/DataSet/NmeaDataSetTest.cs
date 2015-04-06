/*
 * Copyright © 2011 
 * Rowe Technology Inc.
 * All rights reserved.
 * http://www.rowetechinc.com
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification is NOT permitted.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
 * COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
 * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
 * POSSIBILITY OF SUCH DAMAGE.
 * 
 * HISTORY
 * -----------------------------------------------------------------
 * Date            Initials    Version    Comments
 * -----------------------------------------------------------------
 * 12/20/2011      RC          1.10       Initial Coding
 * 02/28/2013      RC          2.18       Added Test for JSON.
 * 
 */

namespace RTI
{
    using System;
    using NUnit.Framework;
    using System.Diagnostics;

    /// <summary>
    /// Unit test of the Ancillary DataSet object.
    /// </summary>
    [TestFixture]
    public class NmeaDataSetTest
    {
        /// <summary>
        /// Test the constructor that takes a string of
        /// NMEA data.  Many NMEA sentences.
        /// </summary>
        [Test]
        public void TestConstructorNmea()
        {
            string nmeaData = "$GPGSV,3,3,11,30,22,145,,31,68,063,,32,62,336,,,,,*40$HEHDT,,T*01$HEROT,,V*12$GPGGA,,,,,,0,,,,M,,M,,*66$GPVTG,,T,,M,,N,,K,N*2C$GPZDA,025735.78,20,12,2011,00,00*6C$GPGSV,3,1,11,01,49,269,,11,36,239,,14,23,056,,16,04,164,*70$GPGSV,3,2,11,20,37,315,,22,16,118,,23,18,269,,25,07,046,*71$GPGSV,3,3,11,30,22,145,,31,68,063,,32,62,336,,,,,*40$HEHDT,,T*01$HEROT,,V*12$GPGGA,,,,,,0,,,,M,,M,,*66$GPVTG,,T,,M,,N,,K,N*2C$GPZDA,025736.78,20,12,2011,00,00*6F$GPGSV,3,1,11,01,49,269,,11,36,239,,14,23,056,,16,04,164,*70$GPGSV,3,2,11,20,37,315,,22,16,118,,23,18,269,,25,07,046,*71$GPGSV,3,3,11,30,22,145,,31,68,063,,32,62,336,,,,,*40$HEHDT,,T*01$HEROT,,V*12$GPGGA,,,,,,0,,,,M,,M,,*66$GPVTG,,T,,M,,N,,K,N*2C$GPZDA,025737.78,20,12,2011,00,00*6E$GPGSV,3,1,11,01,49,269,,11,36,239,,14,23,056,,16,04,164,*70$GPGSV,3,2,11,20,37,315,,22,16,118,,23,18,269,,25,07,046,*71$GPGSV,3,3,11,30,22,145,,31,68,063,,32,62,336,,,,,*40$HEHDT,,T*01$HEROT,,V*12$GPGGA,,,,,,0,,,,M,,M,,*66";

            DataSet.Ensemble adcpData = new DataSet.Ensemble();
            adcpData.AddNmeaData(nmeaData);

            Assert.IsNotNull(adcpData, "Adcp Data was not properly created.");
            Assert.IsTrue(adcpData.IsNmeaAvail, "Nmea Dataset not created.");
            Assert.IsTrue(adcpData.NmeaData.IsGpggaAvail(), "GGA message not parsed correctly.");
            Assert.IsTrue(adcpData.NmeaData.IsGpvtgAvail(), "VTG message not parsed correctly.");

            Assert.AreEqual(DotSpatial.Positioning.Position.Invalid, adcpData.NmeaData.GPGGA.Position, "Position not invalid");
            Assert.AreEqual(DotSpatial.Positioning.Latitude.Invalid, adcpData.NmeaData.GPGGA.Position.Latitude, "Latitude not invalid");
            Assert.AreEqual(DotSpatial.Positioning.Longitude.Invalid, adcpData.NmeaData.GPGGA.Position.Longitude, "Longitude not invalid.");
            Assert.AreEqual(DotSpatial.Positioning.Distance.Invalid, adcpData.NmeaData.GPGGA.Altitude, "Altitude not invalid");
        }

        /// <summary>
        /// Test the constructor that takes a string of
        /// NMEA data. Incomplete NMEA sentence.
        /// </summary>
        [Test]
        public void TestConstructorIncompleteString()
        {
            string nmeaData = "$GPGSV,3,3,11,30";

            DataSet.Ensemble adcpData = new DataSet.Ensemble();
            adcpData.AddNmeaData(nmeaData);

            Assert.IsNotNull(adcpData, "Adcp Data was not properly created.");
            Assert.IsTrue(adcpData.IsNmeaAvail, "Nmea Dataset not created.");
            Assert.IsFalse(adcpData.NmeaData.IsGpggaAvail(), "GGA message not parsed correctly.");
            Assert.IsFalse(adcpData.NmeaData.IsGpvtgAvail(), "VTG message not parsed correctly.");
        }

        /// <summary>
        /// Test the constructor that takes a string of
        /// NMEA data.  Just a GGA message.
        /// </summary>
        [Test]
        public void TestConstructorCompleteString()
        {
            string nmeaData = "$GPGGA,,,,,,0,,,,M,,M,,*66";

            DataSet.Ensemble adcpData = new DataSet.Ensemble();
            adcpData.AddNmeaData(nmeaData);

            Assert.IsNotNull(adcpData, "Adcp Data was not properly created.");
            Assert.IsTrue(adcpData.IsNmeaAvail, "Nmea Dataset not created.");
            Assert.IsTrue(adcpData.NmeaData.IsGpggaAvail(), "GGA message not parsed correctly.");
            Assert.IsFalse(adcpData.NmeaData.IsGpvtgAvail(), "VTG message not parsed correctly.");
        }

        /// <summary>
        /// Test the constructor that takes a string of
        /// NMEA data. Just a VTG message.
        /// </summary>
        [Test]
        public void TestConstructorCompleteString1()
        {
            string nmeaData = "$GPVTG,,T,,M,,N,,K,N*2C";

            DataSet.Ensemble adcpData = new DataSet.Ensemble();
            adcpData.AddNmeaData(nmeaData);

            Assert.IsNotNull(adcpData, "Adcp Data was not properly created.");
            Assert.IsTrue(adcpData.IsNmeaAvail, "Nmea Dataset not created.");
            Assert.IsFalse(adcpData.NmeaData.IsGpggaAvail(), "GGA message not parsed correctly.");
            Assert.IsTrue(adcpData.NmeaData.IsGpvtgAvail(), "VTG message not parsed correctly.");
        }

        /// <summary>
        /// Test the constructor that takes a string of
        /// NMEA data.  Just a GGA message, but with a bad checksum.
        /// </summary>
        [Test]
        public void TestConstructorBadChecksum()
        {
            string nmeaData = "$GPGGA,,,,,,0,,,,M,,M,,*63";

            DataSet.Ensemble adcpData = new DataSet.Ensemble();
            adcpData.AddNmeaData(nmeaData);

            Assert.IsNotNull(adcpData, "Adcp Data was not properly created.");
            Assert.IsTrue(adcpData.IsNmeaAvail, "Nmea Dataset not created.");
            Assert.IsFalse(adcpData.NmeaData.IsGpggaAvail(), "GGA message not parsed correctly.");
            Assert.IsFalse(adcpData.NmeaData.IsGpvtgAvail(), "VTG message not parsed correctly.");
        }

        /// <summary>
        /// Test the constructor that takes a string of
        /// NMEA data.  This will have a missing byte in the checksum for GGA.
        /// </summary>
        [Test]
        public void TestConstructorIncompleteChecksum()
        {
            string nmeaData = "$GPGGA,,,,,,0,,,,M,,M,,*6";

            DataSet.Ensemble adcpData = new DataSet.Ensemble();
            adcpData.AddNmeaData(nmeaData);

            Assert.IsNotNull(adcpData, "Adcp Data was not properly created.");
            Assert.IsTrue(adcpData.IsNmeaAvail, "Nmea Dataset not created.");
            Assert.IsFalse(adcpData.NmeaData.IsGpggaAvail(), "GGA message not parsed correctly.");
            Assert.IsFalse(adcpData.NmeaData.IsGpvtgAvail(), "VTG message not parsed correctly.");
        }

        /// <summary>
        /// Test the constructor that takes a string of
        /// NMEA data. This will have a missing byte in the checksum for GGA.
        /// It will then combine with another NMEA sentence.
        /// </summary>
        [Test]
        public void TestConstructorIncompleteChecksum1()
        {
            string nmeaData = "$GPGGA,,,,,,0,,,,M,,M,,*6$GPVTG,,T,,M,,N,,K,N*2C";

            DataSet.Ensemble adcpData = new DataSet.Ensemble();
            adcpData.AddNmeaData(nmeaData);

            Assert.IsNotNull(adcpData, "Adcp Data was not properly created.");
            Assert.IsTrue(adcpData.IsNmeaAvail, "Nmea Dataset not created.");
            Assert.IsFalse(adcpData.NmeaData.IsGpggaAvail(), "GGA message not parsed correctly.");
            Assert.IsTrue(adcpData.NmeaData.IsGpvtgAvail(), "VTG message not parsed correctly.");
        }

        /// <summary>
        /// Test the constructor that takes a string of
        /// NMEA data.  All messages that could be added.
        /// </summary>
        [Test]
        public void TestConstructorFull()
        {
            //string gga = "$GPGGA,155339.00,3245.44007,N,11719.83271,W,2,09,0.9,-1.1,M,-33.3,M,5.0,0138*50";
            //string vtg = "$GPVTG,277.26,T,265.15,M,2.62,N,4.86,K,D*29";
            //string hehdt = "$HEHDT,274.67,T*1F";
            //string herot = "$HEROT,-32.6,A*31";
            //string zda = "$GPZDA,155339.00,08,12,2011,00,00*67";
            //string gsv = "$GPGSV,3,1,09,02,75,182,50,04,56,053,51,05,08,167,42,09,50,241,48*75";
            string nmeaData = "$HEHDT,274.67,T*1F$HEROT,-32.6,A*31$GPGGA,155339.00,3245.44007,N,11719.83271,W,2,09,0.9,-1.1,M,-33.3,M,5.0,0138*50$GPVTG,277.26,T,265.15,M,2.62,N,4.86,K,D*29$GPZDA,155339.00,08,12,2011,00,00*67$GPGSV,3,1,09,02,75,182,50,04,56,053,51,05,08,167,42,09,50,241,48*75$GPGSV,3,2,09,10,24,111,46,12,45,322,47,17,17,063,45,25,15,313,44*71$GPGSV,3,3,09,28,05,121,36,,,,,,,,,,,,*48";

            DataSet.Ensemble adcpData = new DataSet.Ensemble();
            adcpData.AddNmeaData(nmeaData);

            Assert.IsNotNull(adcpData, "Adcp Data was not properly created.");
            Assert.IsTrue(adcpData.IsNmeaAvail, "Nmea Dataset not created.");
            Assert.IsTrue(adcpData.NmeaData.IsGpggaAvail(), "GGA message not parsed correctly.");
            Assert.IsTrue(adcpData.NmeaData.IsGpvtgAvail(), "VTG message not parsed correctly.");
            Assert.IsTrue(adcpData.NmeaData.IsGpgsvAvail(), "GSV message not parsed correctly.");
            Assert.IsFalse(adcpData.NmeaData.IsGpgllAvail(), "GLL message should not have been found.");
            Assert.IsFalse(adcpData.NmeaData.IsGpgsaAvail(), "GSA message should not have been found.");
            Assert.IsFalse(adcpData.NmeaData.IsGprmcAvail(), "RMC message should not have been found.");
            Assert.IsFalse(adcpData.NmeaData.IsPgrmfAvail(), "PGRMF message should not have been found.");

            Assert.AreEqual(new DotSpatial.Positioning.Latitude("32 45.44007").DecimalDegrees, adcpData.NmeaData.GPGGA.Position.Latitude.DecimalDegrees, 0.0001, "Latitude is not correct");
            Assert.AreEqual(new DotSpatial.Positioning.Latitude("-117 19.83271").DecimalDegrees, adcpData.NmeaData.GPGGA.Position.Longitude.DecimalDegrees, 0.0001, "Longitude is not correct");
            Assert.AreEqual(DotSpatial.Positioning.FixQuality.DifferentialGpsFix, adcpData.NmeaData.GPGGA.FixQuality, "Fix Quality is not correct");
            Assert.AreEqual(9, adcpData.NmeaData.GPGGA.FixedSatelliteCount, "Number of fixed satellites is incorrect.");
            Assert.AreEqual(new DotSpatial.Positioning.Distance(-1.1, DotSpatial.Positioning.DistanceUnit.Meters).Value, adcpData.NmeaData.GPGGA.Altitude.Value, 0.00001, "Altitude is not correct");

            Assert.AreEqual(new DotSpatial.Positioning.Azimuth(277.26), adcpData.NmeaData.GPVTG.Bearing, "True Track Made Good Bearing not correct.");
            Assert.AreEqual(new DotSpatial.Positioning.Speed(2.62, DotSpatial.Positioning.SpeedUnit.Knots), adcpData.NmeaData.GPVTG.Speed, "Speed is not correct.");
        }

        #region JSON

        /// <summary>
        /// Test encoding and decoding to JSON.
        /// </summary>
        [Test]
        public void TestJson()
        {
            // Generate an Ensemble
            DataSet.Ensemble ensemble = EnsembleHelper.GenerateEnsemble(30);

            // Modify the data
            string nmeaData = "$HEHDT,274.67,T*1F$HEROT,-32.6,A*31$GPGGA,155339.00,3245.44007,N,11719.83271,W,2,09,0.9,-1.1,M,-33.3,M,5.0,0138*50$GPVTG,277.26,T,265.15,M,2.62,N,4.86,K,D*29$GPZDA,155339.00,08,12,2011,00,00*67$GPGSV,3,1,09,02,75,182,50,04,56,053,51,05,08,167,42,09,50,241,48*75$GPGSV,3,2,09,10,24,111,46,12,45,322,47,17,17,063,45,25,15,313,44*71$GPGSV,3,3,09,28,05,121,36,,,,,,,,,,,,*48";
            ensemble.AddNmeaData(nmeaData);
            Assert.IsNotNull(ensemble, "Adcp Data was not properly created.");
            Assert.IsTrue(ensemble.IsNmeaAvail, "Nmea Dataset not created.");
            Assert.IsTrue(ensemble.NmeaData.IsGpggaAvail(), "GGA message not parsed correctly.");
            Assert.IsTrue(ensemble.NmeaData.IsGpvtgAvail(), "VTG message not parsed correctly.");
            Assert.IsTrue(ensemble.NmeaData.IsGpgsvAvail(), "GSV message not parsed correctly.");
            Assert.IsFalse(ensemble.NmeaData.IsGpgllAvail(), "GLL message should not have been found.");
            Assert.IsFalse(ensemble.NmeaData.IsGpgsaAvail(), "GSA message should not have been found.");
            Assert.IsFalse(ensemble.NmeaData.IsGprmcAvail(), "RMC message should not have been found.");
            Assert.IsFalse(ensemble.NmeaData.IsPgrmfAvail(), "PGRMF message should not have been found.");
            Assert.AreEqual(new DotSpatial.Positioning.Latitude("32 45.44007").DecimalDegrees, ensemble.NmeaData.GPGGA.Position.Latitude.DecimalDegrees, 0.0001, "Latitude is not correct");
            Assert.AreEqual(new DotSpatial.Positioning.Latitude("-117 19.83271").DecimalDegrees, ensemble.NmeaData.GPGGA.Position.Longitude.DecimalDegrees, 0.0001, "Longitude is not correct");
            Assert.AreEqual(DotSpatial.Positioning.FixQuality.DifferentialGpsFix, ensemble.NmeaData.GPGGA.FixQuality, "Fix Quality is not correct");
            Assert.AreEqual(9, ensemble.NmeaData.GPGGA.FixedSatelliteCount, "Number of fixed satellites is incorrect.");
            Assert.AreEqual(new DotSpatial.Positioning.Distance(-1.1, DotSpatial.Positioning.DistanceUnit.Meters).Value, ensemble.NmeaData.GPGGA.Altitude.Value, 0.00001, "Altitude is not correct");
            Assert.AreEqual(new DotSpatial.Positioning.Azimuth(277.26), ensemble.NmeaData.GPVTG.Bearing, "True Track Made Good Bearing not correct.");
            Assert.AreEqual(new DotSpatial.Positioning.Speed(2.62, DotSpatial.Positioning.SpeedUnit.Knots), ensemble.NmeaData.GPVTG.Speed, "Speed is not correct.");

            string encoded = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.NmeaData);                                      // Serialize object to JSON
            DataSet.NmeaDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.NmeaDataSet>(encoded);            // Deserialize the JSON

            // Verify the values are the same
            Assert.IsTrue(decoded.IsGpggaAvail(), "GGA message not parsed correctly.");
            Assert.IsTrue(decoded.IsGpvtgAvail(), "VTG message not parsed correctly.");
            Assert.IsTrue(decoded.IsGpgsvAvail(), "GSV message not parsed correctly.");
            Assert.IsFalse(decoded.IsGpgllAvail(), "GLL message should not have been found.");
            Assert.IsFalse(decoded.IsGpgsaAvail(), "GSA message should not have been found.");
            Assert.IsFalse(decoded.IsGprmcAvail(), "RMC message should not have been found.");
            Assert.IsFalse(decoded.IsPgrmfAvail(), "PGRMF message should not have been found.");
            Assert.AreEqual(new DotSpatial.Positioning.Latitude("32 45.44007").DecimalDegrees, decoded.GPGGA.Position.Latitude.DecimalDegrees, 0.0001, "Latitude is not correct");
            Assert.AreEqual(new DotSpatial.Positioning.Latitude("-117 19.83271").DecimalDegrees, decoded.GPGGA.Position.Longitude.DecimalDegrees, 0.0001, "Longitude is not correct");
            Assert.AreEqual(DotSpatial.Positioning.FixQuality.DifferentialGpsFix, decoded.GPGGA.FixQuality, "Fix Quality is not correct");
            Assert.AreEqual(9, decoded.GPGGA.FixedSatelliteCount, "Number of fixed satellites is incorrect.");
            Assert.AreEqual(new DotSpatial.Positioning.Distance(-1.1, DotSpatial.Positioning.DistanceUnit.Meters).Value, decoded.GPGGA.Altitude.Value, 0.00001, "Altitude is not correct");
            Assert.AreEqual(new DotSpatial.Positioning.Azimuth(277.26), decoded.GPVTG.Bearing, "True Track Made Good Bearing not correct.");
            Assert.AreEqual(new DotSpatial.Positioning.Speed(2.62, DotSpatial.Positioning.SpeedUnit.Knots), decoded.GPVTG.Speed, "Speed is not correct.");
        }

        /// <summary>
        /// Testing the timing for the JSON conversions.
        /// Put breakstatements on all the time results.
        /// Then run the code and check the results.
        /// </summary>
        [Test]
        public void TestTiming()
        {

            // Generate an Ensemble
            DataSet.Ensemble ensemble = EnsembleHelper.GenerateEnsemble(30);
            string nmeaData = "$HEHDT,274.67,T*1F$HEROT,-32.6,A*31$GPGGA,155339.00,3245.44007,N,11719.83271,W,2,09,0.9,-1.1,M,-33.3,M,5.0,0138*50$GPVTG,277.26,T,265.15,M,2.62,N,4.86,K,D*29$GPZDA,155339.00,08,12,2011,00,00*67$GPGSV,3,1,09,02,75,182,50,04,56,053,51,05,08,167,42,09,50,241,48*75$GPGSV,3,2,09,10,24,111,46,12,45,322,47,17,17,063,45,25,15,313,44*71$GPGSV,3,3,09,28,05,121,36,,,,,,,,,,,,*48";
            ensemble.AddNmeaData(nmeaData);

            Stopwatch watch = new Stopwatch();

            // Test Serialize()
            watch = new Stopwatch();
            watch.Start();
            for (int x = 0; x < 1000; x++)
            {
                string encoded = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.NmeaData);
            }
            watch.Stop();
            long resultSerialize = watch.ElapsedMilliseconds;

            // Test Deserialize()
            string encodedd = Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.NmeaData);
            watch = new Stopwatch();
            watch.Start();
            for (int x = 0; x < 1000; x++)
            {
                DataSet.NmeaDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet.NmeaDataSet>(encodedd);
            }
            watch.Stop();
            long resultDeserialize = watch.ElapsedMilliseconds;

            Debug.WriteLine(String.Format("Serialize:{0}  Deserialize:{1}", resultSerialize, resultDeserialize));

            Debug.WriteLine("Complete");

        }

        #endregion
    }

}