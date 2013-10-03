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
 * 11/21/2011      RC                     Initial coding
 * 02/07/2012      RC          2.00       Updated to latest AdcpCommand and AdcpSubsystemCommand.
 * 07/11/2012      RC          2.12       Updated Decode BREAK to pass a firmware object instead of a string.
 * 07/19/2012      RC          2.12       Added test for decoding ENGI2CSHOW and STIME.
 * 07/26/2012      RC          2.13       Added test for decoding DSDIR.
 * 09/10/2012      RC          2.15       Updated the the test for the BB commands for WP and BT.
 * 09/25/2012      RC          2.15       Added CEOUTPUT command to test.
 * 09/26/2012      RC          2.15       Added TimeValue test.
 * 09/28/2012      RC          2.15       Added test for all the commands.
 * 10/01/2012      RC          2.15       Removed serial number from the AdcpCommand constructor.
 * 10/11/2012      RC          2.15       Updated test with new minimum values for CWS, CWSS and CTD.
 * 11/21/2012      RC          2.16       Added Command Strings for every command.
 * 09/11/2013      RC          2.19.5     Updated test to 2.19.5
 * 
 * 
 */

namespace RTI
{
    using System;
    using NUnit.Framework;
    using RTI.Commands;
using System.Text;
    using System.Globalization;

    /// <summary>
    /// Create test to test the AdcpCommand object.
    /// </summary>
    [TestFixture]
    public class AdcpCommandsTest
    {

        /// <summary>
        /// Test the constructor.
        /// </summary>
        [Test]
        public void TestConstructor()
        {
            AdcpCommands commands = new AdcpCommands();

            Commands.TimeValue tv = new TimeValue();
            Commands.TimeValue tv1 = new Commands.TimeValue(0, 0, 1, 0); 

            // Verify default values
            Assert.AreEqual(Commands.AdcpCommands.DEFAULT_MODE, commands.Mode);

            // Ensemble defaults
            Assert.AreEqual(tv1, commands.CEI);
            Assert.AreEqual(tv1.Hour, commands.CEI_Hour);
            Assert.AreEqual(tv1.Minute, commands.CEI_Minute);
            Assert.AreEqual(tv1.Second, commands.CEI_Second);
            Assert.AreEqual(tv1.HunSec, commands.CEI_HunSec);

            Assert.AreEqual(false, commands.CERECORD_EnsemblePing, "CERECORD is incorrect.");
            Assert.AreEqual(AdcpCommands.AdcpOutputMode.Binary, commands.CEOUTPUT, "CEOUTPUT is incorrect.");

            // Environmental defaults
            Assert.AreEqual(Commands.AdcpCommands.DEFAULT_CWS, commands.CWS);
            Assert.AreEqual(Commands.AdcpCommands.DEFAULT_CWT, commands.CWT);
            Assert.AreEqual(Commands.AdcpCommands.DEFAULT_CTD, commands.CTD);
            Assert.AreEqual(Commands.AdcpCommands.DEFAULT_CWSS, commands.CWSS);
            Assert.AreEqual(Commands.AdcpCommands.DEFAULT_CHO, commands.CHO);
            Assert.AreEqual(Commands.AdcpCommands.DEFAULT_CHS, commands.CHS);
            Assert.AreEqual(Commands.AdcpCommands.DEFAULT_CVSF, commands.CVSF);

            // Serial Comm defaults
            Assert.AreEqual(Commands.AdcpCommands.DEFAULT_C232B, commands.C232B);
            Assert.AreEqual(Commands.AdcpCommands.DEFAULT_C485B, commands.C485B);
            Assert.AreEqual(Commands.AdcpCommands.DEFAULT_C422B, commands.C422B);

            // Get the first subsystem (Only subsystem)
            SerialNumber serialNum = new SerialNumber("01400000000000000000000000000015");
            Subsystem ss = serialNum.SubSystemsDict[0];
            AdcpSubsystemCommands asc = new AdcpSubsystemCommands(new SubsystemConfiguration(ss, 0, 0));

            // Water Profile Defaults
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_CWPON, asc.CWPON);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_CWPBB_TRANSMITPULSETYPE, asc.CWPBB_TransmitPulseType);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CWPBB_LAGLENGTH, asc.CWPBB_LagLength);
            //Assert.AreEqual(tv, asc.CWPAI);
            Assert.AreEqual(tv.Hour, asc.CWPAI.Hour);
            Assert.AreEqual(tv.Minute, asc.CWPAI.Minute);
            Assert.AreEqual(tv.Second, asc.CWPAI.Second);
            Assert.AreEqual(tv.HunSec, asc.CWPAI.HunSec);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CWPBL, asc.CWPBL);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CWPBS, asc.CWPBS);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CWPX, asc.CWPX);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CWPBN, asc.CWPBN);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CWPP, asc.CWPP);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CWPTBP, asc.CWPTBP);

            // Bottom Track defaults
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_CBTON, asc.CBTON);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_CBTBB_MODE, asc.CBTBB_Mode);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_CBTBB_PULSETOPULSE_LAG, asc.CBTBB_PulseToPulseLag);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CBTBB_LONGRANGEDEPTH, asc.CBTBB_LongRangeDepth);

            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CBTBL, asc.CBTBL);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CBTMX, asc.CBTMX);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CBTTBP, asc.CBTTBP);

            // Water Track defaults
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_CWTON, asc.CWTON);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_CWTBB, asc.CWTBB);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CWTBL, asc.CWTBL);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CWTBS, asc.CWTBS);
            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CWTTBP, asc.CWTTBP);
        }

        #region TimeValue

        /// <summary>
        /// Test creating a TimeValue
        /// </summary>
        [Test]
        public void TestTimeValue()
        {
            TimeValue time = new TimeValue(1, 22, 3, 4);

            Assert.AreEqual(new TimeValue(1, 22, 3, 4), time, "TimeValue is incorrect.");
        }

        /// <summary>
        /// Test creating a TimeValue.
        /// Test having 60 minutes, see it increment hours.
        /// </summary>
        [Test]
        public void TestTimeValue_CornerMinute()
        {
            TimeValue time = new TimeValue(1, 60, 3, 4);

            Assert.AreEqual(new TimeValue(2, 0, 3, 4), time, "TimeValue is incorrect.");
        }

        /// <summary>
        /// Test creating a TimeValue.
        /// Test having exceed 60 minutes, see it increment hours.
        /// </summary>
        [Test]
        public void TestTimeValue_CornerMinute1()
        {
            TimeValue time = new TimeValue(1, 63, 3, 4);

            Assert.AreEqual(new TimeValue(2, 3, 3, 4), time, "TimeValue is incorrect.");
        }

        /// <summary>
        /// Test creating a TimeValue.
        /// Test having 60 seconds, see it increment minutes.
        /// </summary>
        [Test]
        public void TestTimeValue_CornerSecond()
        {
            TimeValue time = new TimeValue(1, 4, 60, 4);

            Assert.AreEqual(new TimeValue(1, 5, 0, 4), time, "TimeValue is incorrect.");
        }

        /// <summary>
        /// Test creating a TimeValue.
        /// Test having exceed 60 seconds, see it increment minutes.
        /// </summary>
        [Test]
        public void TestTimeValue_CornerSeconds1()
        {
            TimeValue time = new TimeValue(1, 3, 66, 4);

            Assert.AreEqual(new TimeValue(1, 4, 6, 4), time, "TimeValue is incorrect.");
        }

        /// <summary>
        /// Test creating a TimeValue.
        /// Test having 100 HunSec, see it increment seconds.
        /// </summary>
        [Test]
        public void TestTimeValue_CornerHunSecond()
        {
            TimeValue time = new TimeValue(1, 4, 10, 100);

            Assert.AreEqual(new TimeValue(1, 4, 11, 0), time, "TimeValue is incorrect.");
        }

        /// <summary>
        /// Test creating a TimeValue.
        /// Test having exceed 100 HunSec, see it increment seconds.
        /// </summary>
        [Test]
        public void TestTimeValue_CornerHunSeconds1()
        {
            TimeValue time = new TimeValue(1, 3, 6, 144);

            Assert.AreEqual(new TimeValue(1, 3, 7, 44), time, "TimeValue is incorrect.");
        }

        /// <summary>
        /// Test creating a TimeValue.
        /// Cause all the values to overflow.
        /// </summary>
        [Test]
        public void TestTimeValue_CornerOver()
        {
            TimeValue time = new TimeValue(1, 340, 66, 144);

            Assert.AreEqual(new TimeValue(6, 41, 7, 44), time, "TimeValue is incorrect.");
        }

        /// <summary>
        /// Test creating a TimeValue.
        /// Cause all the values to overflow.
        /// </summary>
        [Test]
        public void TestTimeValue_CornerOverSetValues()
        {
            TimeValue time = new TimeValue();
            time.Hour = 1;
            time.Minute = 340;
            time.Second = 66;
            time.HunSec = 144;

            Assert.AreEqual(new TimeValue(6, 41, 7, 44), time, "TimeValue is incorrect.");
        }

        /// <summary>
        /// Test creating a TimeValue.
        /// Cause all the values to overflow.  Change the order.
        /// Order matters and this will not give the value you expect.
        /// </summary>
        [Test]
        public void TestTimeValue_CornerOverSetValuesOrder()
        {
            TimeValue time = new TimeValue();
            time.HunSec = 144;
            time.Second = 66;
            time.Minute = 340;
            time.Hour = 1;

            Assert.AreNotEqual(new TimeValue(6, 41, 7, 44), time, "TimeValue is incorrect.");
            Assert.AreEqual(new TimeValue(1, 40, 6, 44), time, "TimeValue is incorrect.");
        }

        /// <summary>
        /// Test the equivalence sign.
        /// </summary>
        [Test]
        public void TestTimeValue_Equal()
        {
            TimeValue time1 = new TimeValue(1, 2, 3, 4);
            TimeValue time2 = new TimeValue(1, 2, 3, 4);
            TimeValue time3 = new TimeValue(4, 2, 2, 1);

            Assert.AreEqual(true, time1 == time2, "TimeValue == is incorrect.");
            Assert.AreEqual(true, time1 != time3, "TimeValue != is incorrect.");
            Assert.AreEqual(true, time1.Equals(time2), "TimeValue Equals is incorrect.");
            Assert.AreEqual(false, time1.Equals(time3), "TimeValue Equals false is incorrect.");
        }

        /// <summary>
        /// Test the ToString().
        /// </summary>
        [Test]
        public void TestTimeValue_ToString()
        {
            TimeValue time1 = new TimeValue(1, 2, 3, 4);

            Assert.AreEqual("01:02:03.04", time1.ToString(), "TimeValue ToString() is incorrect.");
        }

        /// <summary>
        /// Test the ToSeconds().
        /// </summary>
        [Test]
        public void TestTimeValue_ToSeconds()
        {
            TimeValue time1 = new TimeValue(1, 2, 3, 4);

            Assert.AreEqual(3723, time1.ToSeconds(), "TimeValue ToSeconds() is incorrect.");
        }

        /// <summary>
        /// Test the ToSecondsRoundUp().
        /// </summary>
        [Test]
        public void TestTimeValue_ToSecondsRoundUp()
        {
            TimeValue time1 = new TimeValue(1, 2, 3, 70);

            Assert.AreEqual(3724, time1.ToSeconds(), "TimeValue ToSeconds() is incorrect.");
        }

        /// <summary>
        /// Test the ToSecondsRoundUpCorner().
        /// </summary>
        [Test]
        public void TestTimeValue_ToSecondsRoundUpCorner()
        {
            TimeValue time1 = new TimeValue(1, 2, 3, 50);

            Assert.AreEqual(3724, time1.ToSeconds(), "TimeValue ToSeconds() is incorrect.");
        }

        /// <summary>
        /// Test the ToSecondsRoundUpCorner1().
        /// </summary>
        [Test]
        public void TestTimeValue_ToSecondsRoundUpCorner1()
        {
            TimeValue time1 = new TimeValue(1, 2, 3, 51);

            Assert.AreEqual(3724, time1.ToSeconds(), "TimeValue ToSeconds() is incorrect.");
        }

        #endregion

        #region Mode

        /// <summary>
        /// Test setting the Mode command.
        /// </summary>
        [Test]
        public void TestMode()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.Mode = AdcpCommands.AdcpMode.PROFILE;

            Assert.AreEqual(AdcpCommands.AdcpMode.PROFILE, cmd.Mode, "Mode is incorrect.");
            Assert.AreEqual("CPROFILE", cmd.Mode_ToString(), "Mode String is incorrect.");
            Assert.AreEqual(AdcpCommands.CMD_CPROFILE, cmd.Mode_ToString(), "Mode String is incorrect.");
        }

        /// <summary>
        /// Test setting the Mode command.
        /// </summary>
        [Test]
        public void TestMode1()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.Mode = AdcpCommands.AdcpMode.DVL;

            Assert.AreEqual(AdcpCommands.AdcpMode.DVL, cmd.Mode, "Mode is incorrect.");
            Assert.AreEqual("CDVL", cmd.Mode_ToString(), "Mode String is incorrect.");
            Assert.AreEqual(AdcpCommands.CMD_CDVL, cmd.Mode_ToString(), "Mode String is incorrect.");
        }

        #endregion

        #region CEI

        /// <summary>
        /// Test setting the CEI command.
        /// </summary>
        [Test]
        public void TestCEI()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CEI = new TimeValue(22,33,44,55);

            Assert.AreEqual(new TimeValue(22, 33, 44, 55), cmd.CEI, "CEI is incorrect.");
        }

        #endregion

        #region CEPO

        /// <summary>
        /// Test setting the CEPO command.
        /// </summary>
        [Test]
        public void TestCEPO()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CEPO = "333";

            Assert.AreEqual("333", cmd.CEPO, "CEPO is incorrect.");
        }

        /// <summary>
        /// Test setting the CEPO command.
        /// </summary>
        [Test]
        public void TestCEPODefault()
        {
            AdcpCommands cmd = new AdcpCommands();

            Assert.AreEqual(AdcpCommands.DEFAULT_CEPO, cmd.CEPO, "CEPO is incorrect.");
        }

        #endregion

        #region CETFP_Year

        /// <summary>
        /// Test setting the CETFP_Year command.
        /// </summary>
        [Test]
        public void TestCETFP_Year()
        {
            AdcpCommands cmd = new AdcpCommands();

            //cmd.CETFP.Year = 2015;
            cmd.CETFP = new DateTime(2015, 1, 1, 1, 1, 1);

            Assert.AreEqual(2015, cmd.CETFP.Year, "CETFP_Year is incorrect.");
        }

        ///// <summary>
        ///// Give a bad CETFP_Year.
        ///// Range is 2000 to 2099.
        ///// </summary>
        //[Test]
        //public void TestCETFP_Year_BadMin()
        //{
        //    AdcpCommands cmd = new AdcpCommands();

        //    //cmd.CETFP.Year = 15;
        //    cmd.CETFP = new DateTime(15, 1, 1, 1, 1, 1);

        //    Assert.AreEqual(AdcpCommands.DEFAULT_CETFP_YEAR, cmd.CETFP.Year, "CETFP_Year is incorrect.");
        //}

        ///// <summary>
        ///// Give a bad CETFP_Year.
        ///// Range is 2000 to 2099.
        ///// </summary>
        //[Test]
        //public void TestCETFP_Year_BadMax()
        //{
        //    AdcpCommands cmd = new AdcpCommands();

        //    //cmd.CETFP_Year = 2100;
        //    cmd.CETFP = new DateTime(2100, 1, 1, 1, 1, 1);

        //    Assert.AreEqual(AdcpCommands.DEFAULT_CETFP_YEAR, cmd.CETFP.Year, "CETFP_Year is incorrect.");
        //}

        ///// <summary>
        ///// Test corner case Min for CETFP_Year.
        ///// Range is 2000 to 2099.
        ///// </summary>
        //[Test]
        //public void TestCETFP_Year_CornerMin()
        //{
        //    AdcpCommands cmd = new AdcpCommands();

        //    //cmd.CETFP_Year = AdcpCommands.MIN_YEAR;
        //    cmd.CETFP = new DateTime(AdcpCommands.MIN_YEAR, 1, 1, 1, 1, 1);

        //    Assert.AreEqual(AdcpCommands.MIN_YEAR, cmd.CETFP.Year, "CETFP_Year is incorrect.");
        //}

        ///// <summary>
        ///// Test corner case Max for CETFP_Year.
        ///// Range is 2000 to 2099.
        ///// </summary>
        //[Test]
        //public void TestCETFP_Year_CornerMax()
        //{
        //    AdcpCommands cmd = new AdcpCommands();

        //    cmd.CETFP_Year = AdcpCommands.MAX_YEAR;

        //    Assert.AreEqual(AdcpCommands.MAX_YEAR, cmd.CETFP_Year, "CETFP_Year is incorrect.");
        //}

        #endregion

        #region CETFP_Month

        /// <summary>
        /// Test setting the CETFP_Month command.
        /// </summary>
        [Test]
        public void TestCETFP_Month()
        {
            AdcpCommands cmd = new AdcpCommands();

            //cmd.CETFP_Month = 5;
            //cmd.CETFP.Year = 2015;
            cmd.CETFP = new DateTime(2015, 5, 1, 1, 1, 1);

            Assert.AreEqual(5, cmd.CETFP.Month, "CETFP_Month is incorrect.");
        }

        ///// <summary>
        ///// Give a bad CETFP_Month.
        ///// Range is 1 to 12.
        ///// </summary>
        //[Test]
        //public void TestCETFP_Month_BadMin()
        //{
        //    AdcpCommands cmd = new AdcpCommands();

        //    cmd.CETFP_Month = 0;

        //    Assert.AreEqual(DateTime.Now.Month, cmd.CETFP_Month, "CETFP_Month is incorrect.");
        //}

        ///// <summary>
        ///// Give a bad CETFP_Month.
        ///// Range is 1 to 12.
        ///// </summary>
        //[Test]
        //public void TestCETFP_Month_BadMax()
        //{
        //    AdcpCommands cmd = new AdcpCommands();

        //    cmd.CETFP_Month = 2100;

        //    Assert.AreEqual(DateTime.Now.Month, cmd.CETFP_Month, "CETFP_Month is incorrect.");
        //}

        ///// <summary>
        ///// Test corner case Min for CETFP_Month.
        ///// Range is 1 to 12.
        ///// </summary>
        //[Test]
        //public void TestCETFP_Month_CornerMin()
        //{
        //    AdcpCommands cmd = new AdcpCommands();

        //    cmd.CETFP_Month = AdcpCommands.MIN_MONTH;

        //    Assert.AreEqual(AdcpCommands.MIN_MONTH, cmd.CETFP_Month, "CETFP_Month is incorrect.");
        //}

        ///// <summary>
        ///// Test corner case Max for CETFP_Month.
        ///// Range is 1 to 12.
        ///// </summary>
        //[Test]
        //public void TestCETFP_Month_CornerMax()
        //{
        //    AdcpCommands cmd = new AdcpCommands();

        //    cmd.CETFP_Month = AdcpCommands.MAX_MONTH;

        //    Assert.AreEqual(AdcpCommands.MAX_MONTH, cmd.CETFP_Month, "CETFP_Month is incorrect.");
        //}

        #endregion

        #region CETFP_Day

        /// <summary>
        /// Test setting the CETFP_Day command.
        /// </summary>
        [Test]
        public void TestCETFP_Day()
        {
            AdcpCommands cmd = new AdcpCommands();

            //cmd.CETFP_Day = 5;
            //cmd.CETFP.Year = 2015;
            cmd.CETFP = new DateTime(2015, 1, 5, 1, 1, 1);

            Assert.AreEqual(5, cmd.CETFP.Day, "CETFP_Day is incorrect.");
        }

        ///// <summary>
        ///// Give a bad CETFP_Day.
        ///// Range is 1 to 31.
        ///// </summary>
        //[Test]
        //public void TestCETFP_Day_BadMin()
        //{
        //    AdcpCommands cmd = new AdcpCommands();

        //    cmd.CETFP_Day = 0;

        //    Assert.AreEqual(DateTime.Now.Day, cmd.CETFP_Day, "CETFP_Day is incorrect.");
        //}

        ///// <summary>
        ///// Give a bad CETFP_Day.
        ///// Range is 1 to 31.
        ///// </summary>
        //[Test]
        //public void TestCETFP_Day_BadMax()
        //{
        //    AdcpCommands cmd = new AdcpCommands();

        //    cmd.CETFP_Day = 2100;

        //    Assert.AreEqual(DateTime.Now.Day, cmd.CETFP_Day, "CETFP_Day is incorrect.");
        //}

        ///// <summary>
        ///// Test corner case Min for CETFP_Day.
        ///// Range is 1 to 31.
        ///// </summary>
        //[Test]
        //public void TestCETFP_Day_CornerMin()
        //{
        //    AdcpCommands cmd = new AdcpCommands();

        //    cmd.CETFP_Day = AdcpCommands.MIN_DAY;

        //    Assert.AreEqual(AdcpCommands.MIN_DAY, cmd.CETFP_Day, "CETFP_Day is incorrect.");
        //}

        ///// <summary>
        ///// Test corner case Max for CETFP_Day.
        ///// Range is 1 to 31.
        ///// </summary>
        //[Test]
        //public void TestCETFP_Day_CornerMax()
        //{
        //    AdcpCommands cmd = new AdcpCommands();

        //    cmd.CETFP_Day = AdcpCommands.MAX_DAY;

        //    Assert.AreEqual(AdcpCommands.MAX_DAY, cmd.CETFP_Day, "CETFP_Day is incorrect.");
        //}

        #endregion

        #region CETFP_Hour

        /// <summary>
        /// Test setting the CETFP_Hour command.
        /// </summary>
        [Test]
        public void TestCETFP_Hour()
        {
            AdcpCommands cmd = new AdcpCommands();

            //cmd.CETFP_Hour = 5;
            //cmd.CETFP.Year = 2015;
            cmd.CETFP = new DateTime(2015, 1, 1, 5, 1, 1);

            Assert.AreEqual(5, cmd.CETFP.Hour, "CETFP_Hour is incorrect.");
        }

        ///// <summary>
        ///// Give a bad CETFP_Hour.
        ///// Range is 0 to 23.
        ///// </summary>
        //[Test]
        //public void TestCETFP_Hour_BadMax()
        //{
        //    AdcpCommands cmd = new AdcpCommands();

        //    cmd.CETFP_Hour = 2100;

        //    Assert.AreEqual(DateTime.Now.Hour, cmd.CETFP_Hour, "CETFP_Hour is incorrect.");
        //}

        ///// <summary>
        ///// Test corner case Min for CETFP_Hour.
        ///// Range is 0 to 23.
        ///// </summary>
        //[Test]
        //public void TestCETFP_Hour_CornerMin()
        //{
        //    AdcpCommands cmd = new AdcpCommands();

        //    cmd.CETFP_Hour = AdcpCommands.MIN_HOUR;

        //    Assert.AreEqual(AdcpCommands.MIN_HOUR, cmd.CETFP_Hour, "CETFP_Hour is incorrect.");
        //}

        ///// <summary>
        ///// Test corner case Max for CETFP_Hour.
        ///// Range is 0 to 23.
        ///// </summary>
        //[Test]
        //public void TestCETFP_Hour_CornerMax()
        //{
        //    AdcpCommands cmd = new AdcpCommands();

        //    cmd.CETFP_Hour = AdcpCommands.MAX_HOUR;

        //    Assert.AreEqual(AdcpCommands.MAX_HOUR, cmd.CETFP_Hour, "CETFP_Hour is incorrect.");
        //}

        #endregion

        #region CETFP_Minute

        /// <summary>
        /// Test setting the CETFP_Minute command.
        /// </summary>
        [Test]
        public void TestCETFP_Minute()
        {
            AdcpCommands cmd = new AdcpCommands();

            //cmd.CETFP_Minute = 5;
            cmd.CETFP = new DateTime(2015, 1, 1, 1, 5, 1);

            Assert.AreEqual(5, cmd.CETFP.Minute, "CETFP_Minute is incorrect.");
        }

        ///// <summary>
        ///// Give a bad CETFP_Minute.
        ///// Range is 0 to 59.
        ///// </summary>
        //[Test]
        //public void TestCETFP_Minute_BadMax()
        //{
        //    AdcpCommands cmd = new AdcpCommands();

        //    cmd.CETFP_Minute = 2100;

        //    Assert.AreEqual(DateTime.Now.Minute, cmd.CETFP_Minute, "CETFP_Minute is incorrect.");
        //}

        ///// <summary>
        ///// Test corner case Min for CETFP_Minute.
        ///// Range is 0 to 59.
        ///// </summary>
        //[Test]
        //public void TestCETFP_Minute_CornerMin()
        //{
        //    AdcpCommands cmd = new AdcpCommands();

        //    cmd.CETFP_Minute = AdcpCommands.MIN_MINSEC;

        //    Assert.AreEqual(AdcpCommands.MIN_MINSEC, cmd.CETFP_Minute, "CETFP_Minute is incorrect.");
        //}

        ///// <summary>
        ///// Test corner case Max for CETFP_Minute.
        ///// Range is 0 to 59.
        ///// </summary>
        //[Test]
        //public void TestCETFP_Minute_CornerMax()
        //{
        //    AdcpCommands cmd = new AdcpCommands();

        //    cmd.CETFP_Minute = AdcpCommands.MAX_MINSEC;

        //    Assert.AreEqual(AdcpCommands.MAX_MINSEC, cmd.CETFP_Minute, "CETFP_Minute is incorrect.");
        //}

        #endregion

        #region CETFP_Second

        /// <summary>
        /// Test setting the CETFP_Second command.
        /// </summary>
        [Test]
        public void TestCETFP_Second()
        {
            AdcpCommands cmd = new AdcpCommands();

            //cmd.CETFP_Second = 5;
            cmd.CETFP = new DateTime(2015, 1, 1, 1, 1, 5);

            Assert.AreEqual(5, cmd.CETFP.Second, "CETFP_Second is incorrect.");
        }

        ///// <summary>
        ///// Give a bad CETFP_Second.
        ///// Range is 0 to 59.
        ///// </summary>
        //[Test]
        //public void TestCETFP_Second_BadMax()
        //{
        //    AdcpCommands cmd = new AdcpCommands();

        //    cmd.CETFP_Second = 2100;

        //    Assert.AreEqual(DateTime.Now.Second, cmd.CETFP_Second, "CETFP_Second is incorrect.");
        //}

        ///// <summary>
        ///// Test corner case Min for CETFP_Second.
        ///// Range is 0 to 59.
        ///// </summary>
        //[Test]
        //public void TestCETFP_Second_CornerMin()
        //{
        //    AdcpCommands cmd = new AdcpCommands();

        //    cmd.CETFP_Second = AdcpCommands.MIN_MINSEC;

        //    Assert.AreEqual(AdcpCommands.MIN_MINSEC, cmd.CETFP_Second, "CETFP_Second is incorrect.");
        //}

        ///// <summary>
        ///// Test corner case Max for CETFP_Second.
        ///// Range is 0 to 59.
        ///// </summary>
        //[Test]
        //public void TestCETFP_Second_CornerMax()
        //{
        //    AdcpCommands cmd = new AdcpCommands();

        //    cmd.CETFP_Second = AdcpCommands.MAX_MINSEC;

        //    Assert.AreEqual(AdcpCommands.MAX_MINSEC, cmd.CETFP_Second, "CETFP_Second is incorrect.");
        //}

        #endregion

        //#region CETFP_HunSec

        ///// <summary>
        ///// Test setting the CETFP_HunSec command.
        ///// </summary>
        //[Test]
        //public void TestCETFP_HunSec()
        //{
        //    AdcpCommands cmd = new AdcpCommands();

        //    cmd.CETFP_HunSec = 5;

        //    Assert.AreEqual(5, cmd.CETFP_HunSec, "CETFP_HunSec is incorrect.");
        //}

        ///// <summary>
        ///// Give a bad CETFP_HunSec.
        ///// Range is 0 to 99.
        ///// </summary>
        //[Test]
        //public void TestCETFP_HunSec_BadMax()
        //{
        //    AdcpCommands cmd = new AdcpCommands();

        //    cmd.CETFP_Second = 2100;

        //    Assert.AreNotEqual(2100, cmd.CETFP_HunSec, "CETFP_HunSec is incorrect.");
        //}

        ///// <summary>
        ///// Test corner case Min for CETFP_HunSec.
        ///// Range is 0 to 99.
        ///// </summary>
        //[Test]
        //public void TestCETFP_HunSec_CornerMin()
        //{
        //    AdcpCommands cmd = new AdcpCommands();

        //    cmd.CETFP_HunSec = AdcpCommands.MIN_HUNSEC;

        //    Assert.AreEqual(AdcpCommands.MIN_HUNSEC, cmd.CETFP_HunSec, "CETFP_HunSec is incorrect.");
        //}

        ///// <summary>
        ///// Test corner case Max for CETFP_HunSec.
        ///// Range is 0 to 99.
        ///// </summary>
        //[Test]
        //public void TestCETFP_HunSec_CornerMax()
        //{
        //    AdcpCommands cmd = new AdcpCommands();

        //    cmd.CETFP_HunSec = AdcpCommands.MAX_HUNSEC;

        //    Assert.AreEqual(AdcpCommands.MAX_HUNSEC, cmd.CETFP_HunSec, "CETFP_HunSec is incorrect.");
        //}

        //#endregion

        #region CERECORD

        /// <summary>
        /// Test setting the CERECORD command.
        /// </summary>
        [Test]
        public void TestCERECORD()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CERECORD_EnsemblePing = true;

            Assert.AreEqual(true, cmd.CERECORD_EnsemblePing, "CERECORD is incorrect.");
        }

        /// <summary>
        /// Test setting the CERECORD command.
        /// </summary>
        [Test]
        public void TestCERECORD1()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CERECORD_EnsemblePing = false;

            Assert.AreEqual(false, cmd.CERECORD_EnsemblePing, "CERECORD is incorrect.");
        }

        #endregion

        #region CEOUTPUT

        /// <summary>
        /// Test setting the CEOUTPUT command.
        /// </summary>
        [Test]
        public void TestCEOUTPUT()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CEOUTPUT = AdcpCommands.AdcpOutputMode.Binary;

            Assert.AreEqual(AdcpCommands.AdcpOutputMode.Binary, cmd.CEOUTPUT, "CEOUTPUT is incorrect.");
        }

        /// <summary>
        /// Give a bad CEOUTPUT.
        /// Range is 0 to 2.
        /// </summary>
        [Test]
        public void TestCEOUTPUT_BadMax()
        {
            AdcpCommands cmd = new AdcpCommands();

            Assert.AreEqual(AdcpCommands.DEFAULT_CEOUTPUT, cmd.CEOUTPUT, "CEOUTPUT is incorrect.");
        }

        /// <summary>
        /// Test corner case Min for CCEOUTPUTHO.
        /// Range is 0 to 2.
        /// </summary>
        [Test]
        public void TestCEOUTPUT_CornerMin()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CEOUTPUT = AdcpCommands.AdcpOutputMode.Disable;

            Assert.AreEqual(AdcpCommands.AdcpOutputMode.Disable, cmd.CEOUTPUT, "CEOUTPUT is incorrect.");
        }

        /// <summary>
        /// Test corner case Max for CEOUTPUT.
        /// Range is 0 to 2.
        /// </summary>
        [Test]
        public void TestCEOUTPUT_CornerMax()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CEOUTPUT = AdcpCommands.AdcpOutputMode.ASCII;

            Assert.AreEqual(AdcpCommands.AdcpOutputMode.ASCII, cmd.CEOUTPUT, "CEOUTPUT is incorrect.");
        }

        #endregion

        #region CWS

        /// <summary>
        /// Test setting the CWS command.
        /// </summary>
        [Test]
        public void TestCWS()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CWS = 15.004f;

            Assert.AreEqual(15.004f, cmd.CWS, 0.0001, "CWS is incorrect.");
        }

        /// <summary>
        /// Test setting the CWS command.
        /// </summary>
        [Test]
        public void TestCWS_Bad()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CWS = -15.004f;

            Assert.AreEqual(AdcpCommands.DEFAULT_CWS, cmd.CWS, 0.0001, "CWS is incorrect.");
        }

        /// <summary>
        /// Test setting the CWS command.
        /// </summary>
        [Test]
        public void TestCWS_Min()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CWS = AdcpCommands.MIN_CWS;

            Assert.AreEqual(AdcpCommands.MIN_CWS, cmd.CWS, 0.0001, "CWS is incorrect.");
        }

        #endregion

        #region CWT

        /// <summary>
        /// Test setting the CWT command.
        /// </summary>
        [Test]
        public void TestCWT()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CWT = 15.006f;

            Assert.AreEqual(15.006f, cmd.CWT, 0.0001, "CWT is incorrect.");
        }

        #endregion

        #region CTD

        /// <summary>
        /// Test setting the CWT command.
        /// </summary>
        [Test]
        public void TestCTD()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CTD = 15.008f;

            Assert.AreEqual(15.008f, cmd.CTD, 0.0001, "CTD is incorrect.");
        }

        /// <summary>
        /// Test setting the CTD command.
        /// </summary>
        [Test]
        public void TestCTD_Bad()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CTD = -15.004f;

            Assert.AreEqual(AdcpCommands.DEFAULT_CTD, cmd.CTD, 0.0001, "CTD is incorrect.");
        }

        /// <summary>
        /// Test setting the CTD command.
        /// </summary>
        [Test]
        public void TestCTD_Min()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CTD = AdcpCommands.MIN_CTD;

            Assert.AreEqual(AdcpCommands.MIN_CTD, cmd.CTD, 0.0001, "CTD is incorrect.");
        }

        #endregion

        #region CWSS

        /// <summary>
        /// Test setting the CWSS command.
        /// </summary>
        [Test]
        public void TestCWSS()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CWSS = 15.009f;

            Assert.AreEqual(15.009f, cmd.CWSS, 0.0001, "CWSS is incorrect.");
        }

        /// <summary>
        /// Test setting the CWSS command.
        /// </summary>
        [Test]
        public void TestCWSS_Bad()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CWSS = -15.004f;

            Assert.AreEqual(AdcpCommands.DEFAULT_CWSS, cmd.CWSS, 0.0001, "CWSS is incorrect.");
        }

        /// <summary>
        /// Test setting the CWSS command.
        /// </summary>
        [Test]
        public void TestCWSS_Min()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CWSS = AdcpCommands.MIN_CWSS;

            Assert.AreEqual(AdcpCommands.MIN_CWSS, cmd.CWSS, 0.0001, "CWSS is incorrect.");
        }

        #endregion

        #region CHO

        /// <summary>
        /// Test setting the CHO command.
        /// </summary>
        [Test]
        public void TestCHO()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CHO = 15.004f;

            Assert.AreEqual(15.004f, cmd.CHO, 0.0001, "CHO is incorrect.");
        }

        /// <summary>
        /// Give a bad CHO.
        /// Range is -180 to 180.
        /// </summary>
        [Test]
        public void TestCHO_BadMin()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CHO = -256.33f;

            Assert.AreEqual(AdcpCommands.DEFAULT_CHO, cmd.CHO, 0.0001, "CHO is incorrect.");
        }

        /// <summary>
        /// Give a bad CHO.
        /// Range is -180 to 180.
        /// </summary>
        [Test]
        public void TestCHO_BadMax()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CHO = 86500.004f;

            Assert.AreEqual(AdcpCommands.DEFAULT_CHO, cmd.CHO, 0.0001, "CHO is incorrect.");
        }

        /// <summary>
        /// Test corner case Min for CHO.
        /// Range is -180 to 180.
        /// </summary>
        [Test]
        public void TestCHO_CornerMin()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CHO = AdcpCommands.MIN_CHO;

            Assert.AreEqual(AdcpCommands.MIN_CHO, cmd.CHO, "CHO is incorrect.");
        }

        /// <summary>
        /// Test corner case Max for CHO.
        /// Range is -180 to 180.
        /// </summary>
        [Test]
        public void TestCHO_CornerMax()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CHO = AdcpCommands.MAX_CHO;

            Assert.AreEqual(AdcpCommands.MAX_CHO, cmd.CHO, "CHO is incorrect.");
        }

        #endregion

        #region CHS

        /// <summary>
        /// Test setting the CHS command.
        /// </summary>
        [Test]
        public void TestCHS()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CHS = HeadingSrc.INTERNAL;

            Assert.AreEqual(HeadingSrc.INTERNAL, cmd.CHS, "CHS is incorrect.");
        }

        /// <summary>
        /// Test setting the CHS command.
        /// </summary>
        [Test]
        public void TestCHS1()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CHS = HeadingSrc.SERIAL;

            Assert.AreEqual(HeadingSrc.SERIAL, cmd.CHS, "CHS is incorrect.");
        }

        #endregion

        #region CVSF

        /// <summary>
        /// Test setting the CVSF command.
        /// </summary>
        [Test]
        public void TestCVSF()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CVSF = 15.011f;

            Assert.AreEqual(15.011f, cmd.CVSF, 0.0001, "CVSF is incorrect.");
        }

        #endregion

        #region C232B

        /// <summary>
        /// Test setting the C232B command.
        /// </summary>
        [Test]
        public void TestC232B_2400()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C232B = Baudrate.BAUD_2400;

            Assert.AreEqual(Baudrate.BAUD_2400, cmd.C232B, "C232B BAUD_2400 is incorrect.");
        }

        /// <summary>
        /// Test setting the C232B command.
        /// </summary>
        [Test]
        public void TestC232B_4800()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C232B = Baudrate.BAUD_4800;

            Assert.AreEqual(Baudrate.BAUD_4800, cmd.C232B, "C232B BAUD_4800 is incorrect.");
        }

        /// <summary>
        /// Test setting the C232B command.
        /// </summary>
        [Test]
        public void TestC232B_9600()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C232B = Baudrate.BAUD_9600;

            Assert.AreEqual(Baudrate.BAUD_9600, cmd.C232B, "C232B BAUD_9600 is incorrect.");
        }

        /// <summary>
        /// Test setting the C232B command.
        /// </summary>
        [Test]
        public void TestC232B_19200()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C232B = Baudrate.BAUD_19200;

            Assert.AreEqual(Baudrate.BAUD_19200, cmd.C232B, "C232B BAUD_19200 is incorrect.");
        }

        /// <summary>
        /// Test setting the C232B command.
        /// </summary>
        [Test]
        public void TestC232B_38400()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C232B = Baudrate.BAUD_38400;

            Assert.AreEqual(Baudrate.BAUD_38400, cmd.C232B, "C232B BAUD_38400 is incorrect.");
        }

        /// <summary>
        /// Test setting the C232B command.
        /// </summary>
        [Test]
        public void TestC232B_115200()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C232B = Baudrate.BAUD_115200;

            Assert.AreEqual(Baudrate.BAUD_115200, cmd.C232B, "C232B BAUD_115200 is incorrect.");
        }

        /// <summary>
        /// Test setting the C232B command.
        /// </summary>
        [Test]
        public void TestC232B_230400()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C232B = Baudrate.BAUD_230400;

            Assert.AreEqual(Baudrate.BAUD_230400, cmd.C232B, "C232B BAUD_230400 is incorrect.");
        }

        /// <summary>
        /// Test setting the C232B command.
        /// </summary>
        [Test]
        public void TestC232B_460800()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C232B = Baudrate.BAUD_460800;

            Assert.AreEqual(Baudrate.BAUD_460800, cmd.C232B, "C232B BAUD_460800 is incorrect.");
        }

        /// <summary>
        /// Test setting the C232B command.
        /// </summary>
        [Test]
        public void TestC232B_921600()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C232B = Baudrate.BAUD_921600;

            Assert.AreEqual(Baudrate.BAUD_921600, cmd.C232B, "C232B BAUD_921600 is incorrect.");
        }

        #endregion

        #region C485B

        /// <summary>
        /// Test setting the C485B command.
        /// </summary>
        [Test]
        public void TestC485B_2400()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C485B = Baudrate.BAUD_2400;

            Assert.AreEqual(Baudrate.BAUD_2400, cmd.C485B, "C485B BAUD_2400 is incorrect.");
        }

        /// <summary>
        /// Test setting the C485B command.
        /// </summary>
        [Test]
        public void TestC485B_4800()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C485B = Baudrate.BAUD_4800;

            Assert.AreEqual(Baudrate.BAUD_4800, cmd.C485B, "C485B BAUD_4800 is incorrect.");
        }

        /// <summary>
        /// Test setting the C485B command.
        /// </summary>
        [Test]
        public void TestC485B_9600()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C485B = Baudrate.BAUD_9600;

            Assert.AreEqual(Baudrate.BAUD_9600, cmd.C485B, "C485B BAUD_9600 is incorrect.");
        }

        /// <summary>
        /// Test setting the C485B command.
        /// </summary>
        [Test]
        public void TestC485B_19200()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C485B = Baudrate.BAUD_19200;

            Assert.AreEqual(Baudrate.BAUD_19200, cmd.C485B, "C485B BAUD_19200 is incorrect.");
        }

        /// <summary>
        /// Test setting the C485B command.
        /// </summary>
        [Test]
        public void TestC485B_38400()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C485B = Baudrate.BAUD_38400;

            Assert.AreEqual(Baudrate.BAUD_38400, cmd.C485B, "C485B BAUD_38400 is incorrect.");
        }

        /// <summary>
        /// Test setting the C485B command.
        /// </summary>
        [Test]
        public void TestC485B_115200()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C485B = Baudrate.BAUD_115200;

            Assert.AreEqual(Baudrate.BAUD_115200, cmd.C485B, "C485B BAUD_115200 is incorrect.");
        }

        /// <summary>
        /// Test setting the C485B command.
        /// </summary>
        [Test]
        public void TestC485B_230400()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C485B = Baudrate.BAUD_230400;

            Assert.AreEqual(Baudrate.BAUD_230400, cmd.C485B, "C485B BAUD_230400 is incorrect.");
        }

        /// <summary>
        /// Test setting the C485B command.
        /// </summary>
        [Test]
        public void TestC485B_460800()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C485B = Baudrate.BAUD_460800;

            Assert.AreEqual(Baudrate.BAUD_460800, cmd.C485B, "C485B BAUD_460800 is incorrect.");
        }

        /// <summary>
        /// Test setting the C485B command.
        /// </summary>
        [Test]
        public void TestC485B_921600()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C485B = Baudrate.BAUD_921600;

            Assert.AreEqual(Baudrate.BAUD_921600, cmd.C485B, "C485B BAUD_921600 is incorrect.");
        }

        #endregion

        #region C422B

        /// <summary>
        /// Test setting the C422B command.
        /// </summary>
        [Test]
        public void TestC422B_2400()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C422B = Baudrate.BAUD_2400;

            Assert.AreEqual(Baudrate.BAUD_2400, cmd.C422B, "C422B BAUD_2400 is incorrect.");
        }

        /// <summary>
        /// Test setting the C422B command.
        /// </summary>
        [Test]
        public void TestC422B_4800()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C422B = Baudrate.BAUD_4800;

            Assert.AreEqual(Baudrate.BAUD_4800, cmd.C422B, "C422B BAUD_4800 is incorrect.");
        }

        /// <summary>
        /// Test setting the C485B command.
        /// </summary>
        [Test]
        public void TestC422B_9600()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C422B = Baudrate.BAUD_9600;

            Assert.AreEqual(Baudrate.BAUD_9600, cmd.C422B, "C422B BAUD_9600 is incorrect.");
        }

        /// <summary>
        /// Test setting the C422B command.
        /// </summary>
        [Test]
        public void TestC422B_19200()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C422B = Baudrate.BAUD_19200;

            Assert.AreEqual(Baudrate.BAUD_19200, cmd.C422B, "C422B BAUD_19200 is incorrect.");
        }

        /// <summary>
        /// Test setting the C422B command.
        /// </summary>
        [Test]
        public void TestC422B_38400()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C422B = Baudrate.BAUD_38400;

            Assert.AreEqual(Baudrate.BAUD_38400, cmd.C422B, "C422B BAUD_38400 is incorrect.");
        }

        /// <summary>
        /// Test setting the C422B command.
        /// </summary>
        [Test]
        public void TestC422B_115200()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C422B = Baudrate.BAUD_115200;

            Assert.AreEqual(Baudrate.BAUD_115200, cmd.C422B, "C422B BAUD_115200 is incorrect.");
        }

        /// <summary>
        /// Test setting the C422B command.
        /// </summary>
        [Test]
        public void TestCC422B_230400()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C422B = Baudrate.BAUD_230400;

            Assert.AreEqual(Baudrate.BAUD_230400, cmd.C422B, "C422B BAUD_230400 is incorrect.");
        }

        /// <summary>
        /// Test setting the C422B command.
        /// </summary>
        [Test]
        public void TestC422B_460800()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C422B = Baudrate.BAUD_460800;

            Assert.AreEqual(Baudrate.BAUD_460800, cmd.C422B, "C422B BAUD_460800 is incorrect.");
        }

        /// <summary>
        /// Test setting the C422B command.
        /// </summary>
        [Test]
        public void TestC422B_921600()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C422B = Baudrate.BAUD_921600;

            Assert.AreEqual(Baudrate.BAUD_921600, cmd.C422B, "C422B BAUD_921600 is incorrect.");
        }

        #endregion

        #region Decode

        /// <summary>
        /// Test the DecodeBREAK() function.
        /// </summary>
        [Test]
        public void TestDecodeBreak()
        {
            string brk = "Copyright (c) 2009-2012 Rowe Technologies Inc. All rights reserved. \r\nDP300 DP1200 \r\nSN: 01460000000000000000000000000001 \r\nFW: 00.02.05 Apr 17 2012 05:40:11";

            BreakStmt brkStmt = RTI.Commands.AdcpCommands.DecodeBREAK(brk);

            Assert.AreEqual(1, brkStmt.SerialNum.SystemSerialNumber, "Serial Number is incorrect");
            Assert.AreEqual("01460000000000000000000000000001", brkStmt.SerialNum.ToString(), "Serial Number String is Incorrect");
            Assert.AreEqual(2, brkStmt.SerialNum.SubSystemsDict.Count, "Subsystem count is incorrect");
            Assert.AreEqual(0, brkStmt.FirmwareVersion.FirmwareMajor, string.Format("Firmware Major is incorrect", 0, brkStmt.FirmwareVersion.FirmwareMajor));
            Assert.AreEqual(2, brkStmt.FirmwareVersion.FirmwareMinor, string.Format("Firmware Minor is incorrect", 2, brkStmt.FirmwareVersion.FirmwareMinor));
            Assert.AreEqual(5, brkStmt.FirmwareVersion.FirmwareRevision, string.Format("Firmware Revision is incorrect", 5, brkStmt.FirmwareVersion.FirmwareRevision));
            Assert.AreEqual("DP300 DP1200", brkStmt.Hardware, "Hardware is Incorrect");
        }

        /// <summary>
        /// Test the Decode ENGPNI command.
        /// </summary>
        [Test]
        public void TestDecodeEngPni()
        {
            string buffer = "H=0.00, P=0.00, R=0.00";
            HPR result = RTI.Commands.AdcpCommands.DecodeEngPniResult(buffer);

            Assert.AreEqual(0, result.Heading, "Heading is incorrect");
            Assert.AreEqual(0, result.Pitch, "Pitch is incorrect");
            Assert.AreEqual(0, result.Roll, "Roll is incorrect");
        }

        /// <summary>
        /// Test the Decode ENGPNI command with data.
        /// </summary>
        [Test]
        public void TestDecodeEngPniData()
        {
            string buffer = "H=179.235, P=56.23, R=-2.04";
            HPR result = RTI.Commands.AdcpCommands.DecodeEngPniResult(buffer);

            Assert.AreEqual(179.235, result.Heading, "Heading is incorrect");
            Assert.AreEqual(56.23, result.Pitch, "Pitch is incorrect");
            Assert.AreEqual(-2.04, result.Roll, "Roll is incorrect");
        }

        /// <summary>
        /// Test the command that decodes the ENGI2CSHOW output.
        /// This will give the registers values for the RTC and Rcvr.
        /// It will also give the board revision and serial number.
        /// </summary>
        [Test]
        public void TestDecodeENGI2CSHOW()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("I2C Device,Address, Data\n");
            builder.Append("RVCR,      00, --\n");
            builder.Append("RVCR,      01, 01 07 00 00 04 0F 40 A0 7F 00 00 20 20 00 40 00 60 00 50 03 03 00 10 00 00 27 27\n");
            builder.Append("RVCR,      02, --\n");
            builder.Append("RVCR,      03, --\n");
            builder.Append("CAT9534,      20, --\n");
            builder.Append("CAT9534,      21, --\n");
            builder.Append("CAT9534,      22, C9\n");
            builder.Append("CAT9534,      23, --\n");
            builder.Append("CAT9534,      24, --\n");
            builder.Append("CAT9534,      25, --\n");
            builder.Append("CAT9534,      26, --\n");
            builder.Append("CAT9534,      27, --\n");
            builder.Append("AD7991-0,      28, 08 24 17 C8 20 00 30 01 \n");
            builder.Append("AD7991-1,      29, 00 00 10 00 20 00 30 00 \n");
            builder.Append("24AA32AF,     50, 50007  REV:XD1  SER#010\n");
            builder.Append("24AA32AF,     51, 50012  REV:XD1  SER#010\n");
            builder.Append("24AA32AF,     52, 50009  REV:XD1  SER#010\n");
            builder.Append("24AA32AF,     53, 50018  REV:XD1  SER#010   50022  REV:XA  SER#0XX\n");
            builder.Append("24AA32AF,     54, 50016  REV:XC1  SER#013\n");
            builder.Append("24AA32AF,     55, --\n");
            builder.Append("24AA32AF,     56, --\n");
            builder.Append("24AA32AF,     57, --\n");
            builder.Append("DS3231,      68, 14 57 08 02 19 07 12 00 00 00 00 00 00 00 00 00 00 2F C0 \n");

            I2cMemDevs devs = RTI.Commands.AdcpCommands.DecodeENGI2CSHOW(builder.ToString());

            #region RTC Registers
            Assert.AreEqual(1, devs.RtcRegs.Count, "RTC register count is incorrect");
            Assert.AreEqual(19, devs.RtcRegs[0].RegValues.Count, "RTC register number of elements is incorrect");
            Assert.AreEqual(104, devs.RtcRegs[0].ID, string.Format("Register ID for RTC is incorrect Found: {0}", devs.RtcRegs[0].ID));        // 0x68 =  104
            Assert.AreEqual(Convert.ToInt32("68", 16), devs.RtcRegs[0].ID, "Register ID for RTC is incorrect");        // 0x68 =  104
            Assert.AreEqual(Convert.ToInt32("14", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[0], 16), "Register Value 0 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("57", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[1], 16), "Register Value 1 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("08", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[2], 16), "Register Value 2 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("02", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[3], 16), "Register Value 3 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("19", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[4], 16), "Register Value 4 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("07", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[5], 16), "Register Value 5 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("12", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[6], 16), "Register Value 6 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[7], 16), "Register Value 7 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[8], 16), "Register Value 8 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[9], 16), "Register Value 9 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[10], 16), "Register Value 10 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[11], 16), "Register Value 11 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[12], 16), "Register Value 12 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[13], 16), "Register Value 13 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[14], 16), "Register Value 14 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[15], 16), "Register Value 15 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[16], 16), "Register Value 16 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("2F", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[17], 16), "Register Value 17 for RTC is incorrect");
            Assert.AreEqual(Convert.ToInt32("C0", 16), Convert.ToInt32(devs.RtcRegs[0].RegValues[18], 16), "Register Value 18 for RTC is incorrect");
            #endregion

            #region Rcvr Register
            Assert.AreEqual(1, devs.RvcrRegs.Count, "Rcvr register count is incorrect");
            Assert.AreEqual(27, devs.RvcrRegs[0].RegValues.Count, "Rcvr register number of elements is incorrect");
            Assert.AreEqual(1, devs.RvcrRegs[0].ID, "Register ID for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("01", 16), devs.RvcrRegs[0].ID, "Register ID for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("01", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[0], 16), "Register Value 0 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("07", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[1], 16), "Register Value 1 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[2], 16), "Register Value 2 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[3], 16), "Register Value 3 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("04", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[4], 16), "Register Value 4 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("0F", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[5], 16), "Register Value 5 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("40", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[6], 16), "Register Value 6 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("A0", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[7], 16), "Register Value 7 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("7F", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[8], 16), "Register Value 8 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[9], 16), "Register Value 9 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[10], 16), "Register Value 10 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("20", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[11], 16), "Register Value 11 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("20", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[12], 16), "Register Value 12 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[13], 16), "Register Value 13 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("40", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[14], 16), "Register Value 14 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[15], 16), "Register Value 15 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("60", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[16], 16), "Register Value 16 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[17], 16), "Register Value 17 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("50", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[18], 16), "Register Value 18 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("03", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[19], 16), "Register Value 19 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("03", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[20], 16), "Register Value 20 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[21], 16), "Register Value 21 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("10", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[22], 16), "Register Value 22 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[23], 16), "Register Value 23 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("00", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[24], 16), "Register Value 24 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("27", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[25], 16), "Register Value 25 for Rcvr is incorrect");
            Assert.AreEqual(Convert.ToInt32("27", 16), Convert.ToInt32(devs.RvcrRegs[0].RegValues[26], 16), "Register Value 26 for Rcvr is incorrect");
            #endregion

            #region Board Revs and Serial

            Assert.AreEqual(I2cBoardId.IO, devs.IoBoard.ID, "I/O Board ID is incorrect.");
            Assert.AreEqual(50007, (long)devs.IoBoard.ID, "I/O Board ID value is incorrect.");
            Assert.AreEqual(10, devs.IoBoard.SerialNum, "I/O Board serial number is incorrect.");
            Assert.AreEqual("XD1", devs.IoBoard.Revision, "I/O Board rev is incorrect.");

            Assert.AreEqual(I2cBoardId.LOW_PWR_REG, devs.LowPwrRegBoard.ID, "Low Pwr Reg Board ID is incorrect.");
            Assert.AreEqual(50012, (long)devs.LowPwrRegBoard.ID, "Low Pwr Reg Board ID value is incorrect.");
            Assert.AreEqual(10, devs.LowPwrRegBoard.SerialNum, "Low Pwr Reg Board serial number is incorrect.");
            Assert.AreEqual("XD1", devs.LowPwrRegBoard.Revision, "Low Pwr Reg Board rev is incorrect.");

            Assert.AreEqual(I2cBoardId.XMITTER, devs.XmitterBoard.ID, "Xmitter Board ID is incorrect.");
            Assert.AreEqual(50009, (long)devs.XmitterBoard.ID, "Xmitter Board ID value is incorrect.");
            Assert.AreEqual(10, devs.XmitterBoard.SerialNum, "Xmitter Board serial number is incorrect.");
            Assert.AreEqual("XD1", devs.XmitterBoard.Revision, "Xmitter Board rev is incorrect.");

            Assert.AreEqual(I2cBoardId.VIRTUAL_GND, devs.VirtualGndBoard.ID, "Virtual Gnd Board ID is incorrect.");
            Assert.AreEqual(50018, (long)devs.VirtualGndBoard.ID, "Virtual Gnd Board ID value is incorrect.");
            Assert.AreEqual(10, devs.VirtualGndBoard.SerialNum, "Virtual Gnd Board serial number is incorrect.");
            Assert.AreEqual("XD1", devs.VirtualGndBoard.Revision, "Virtual Gnd Board rev is incorrect.");

            Assert.AreEqual(I2cBoardId.RCVR, devs.RcvrBoard.ID, "Rcvr Board ID is incorrect.");
            Assert.AreEqual(50022, (long)devs.RcvrBoard.ID, "Rcvr Board ID value is incorrect.");
            Assert.AreEqual(0, devs.RcvrBoard.SerialNum, "Rcvr Board serial number is incorrect.");
            Assert.AreEqual("XA", devs.RcvrBoard.Revision, "Rcvr Board rev is incorrect.");

            Assert.AreEqual(I2cBoardId.BACKPLANE, devs.BackPlaneBoard.ID, "Backplane Board ID is incorrect.");
            Assert.AreEqual(50016, (long)devs.BackPlaneBoard.ID, "Backplane Board ID value is incorrect.");
            Assert.AreEqual(13, devs.BackPlaneBoard.SerialNum, "Backplane Board serial number is incorrect.");
            Assert.AreEqual("XC1", devs.BackPlaneBoard.Revision, "Backplane Board rev is incorrect.");

            #endregion

        }

        /// <summary>
        /// Test setting a bad serial number for the I2C board.
        /// </summary>
        [Test]
        public void TestI2cBoardSerialBad()
        {
            I2cBoard brd = new I2cBoard();
            brd.SetSerial("0xxx");

            Assert.AreEqual(0, brd.SerialNum, "Serial number is incorrect.");
            Assert.AreEqual("", brd.Revision, "Revision is incorrect.");
            Assert.AreEqual(I2cBoardId.UNKNOWN, brd.ID, "Board ID is incorrect.");
        }

        /// <summary>
        /// Test setting a serial number for the I2C board.
        /// </summary>
        [Test]
        public void TestI2cBoardSerial()
        {
            I2cBoard brd = new I2cBoard();
            brd.SetSerial("011");

            Assert.AreEqual(11, brd.SerialNum);
            Assert.AreEqual("", brd.Revision, "Revision is incorrect.");
            Assert.AreEqual(I2cBoardId.UNKNOWN, brd.ID, "Board ID is incorrect.");
        }

        /// <summary>
        /// Test the command that decodes the STIME output.
        /// 
        /// Ex:
        /// 2012/07/19 10:07:41
        /// </summary>
        [Test]
        public void TestDecodeSTIME()
        {
            string buffer = "2012/07/19 10:07:41";
            DateTime dt = RTI.Commands.AdcpCommands.DecodeSTIME(buffer);

            Assert.AreEqual(2012, dt.Year, "Year is incorrect.");
            Assert.AreEqual(07, dt.Month, "Month is incorrect.");
            Assert.AreEqual(19, dt.Day, "Day is incorrect.");
            Assert.AreEqual(10, dt.Hour, "Hour is incorrect.");
            Assert.AreEqual(7, dt.Minute, "Minute is incorrect.");
            Assert.AreEqual(41, dt.Second, "Second is incorrect.");
        }

        /// <summary>
        /// Test decoding DSDIR.
        /// </summary>
        [Test]
        public void TEstDecodeDSDIR()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("DSDIR\n");
            builder.Append("Total Space:                       3781.500 MB\n");
            builder.Append("BOOT.BIN     2012/07/10 06:41:00      0.023\n");
            builder.Append("RTISYS.BIN   2012/07/24 09:56:18      0.184\n");
            builder.Append("HELP.TXT     2012/07/10 07:10:12      0.003\n");
            builder.Append("BBCODE.BIN   2012/07/10 06:41:00      0.017\n");
            builder.Append("ENGHELP.TXT  2012/07/17 09:49:04      0.002\n");
            builder.Append("SYSCONF.BIN  2012/07/24 10:56:08      0.003\n");
            builder.Append("A0000001.ENS 2012/04/02 16:53:11      1.004\n");
            builder.Append("A0000002.ENS 2012/04/02 17:53:11      1.004\n");
            builder.Append("A0000003.ENS 2012/04/02 18:53:11      1.004\n");
            builder.Append("A0000004.ENS 2012/04/02 19:53:11      1.004\n");
            builder.Append("A0000005.ENS 2012/04/02 20:53:11      1.004\n");
            builder.Append("Used Space:                           5.252 MB\n");
            builder.Append(" \n");
            builder.Append("DSDIR\n");

            RTI.Commands.AdcpDirListing listing = RTI.Commands.AdcpCommands.DecodeDSDIR(builder.ToString());

            Assert.AreEqual(3781.500, listing.TotalSpace, 0.001, "Total Space is incorrect.");
            Assert.AreEqual(5.252, listing.UsedSpace, 0.001, "Used Space is incorrect.");
            Assert.AreEqual(5, listing.DirListing.Count, "Number of files is incorrect.");
            Assert.AreEqual(1.004, listing.DirListing[0].FileSize, "File size of file 1 is incorrect.");
            Assert.AreEqual("A0000001.ENS", listing.DirListing[0].FileName, "File name of file 1 is incorrect.");
            Assert.AreEqual(1.004, listing.DirListing[4].FileSize, "File size of file 5 is incorrect.");
            Assert.AreEqual("A0000005.ENS", listing.DirListing[4].FileName, "File name of file 5 is incorrect.");
        }

        #endregion

        #region Equal

        /// <summary>
        /// Test Equals sign.
        /// </summary>
        [Test]
        public void TestEqual_New()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.Mode = AdcpCommands.AdcpMode.DVL;
            cmd.CEI = new TimeValue(1, 2, 3, 4);
            cmd.CEPO = "33";
            //cmd.CETFP_Year = 2022;
            //cmd.CETFP_Month = 4;
            //cmd.CETFP_Day = 20;
            //cmd.CETFP_Hour = 3;
            //cmd.CETFP_Minute = 2;
            //cmd.CETFP_Second = 32;
            //cmd.CETFP_HunSec = 83;
            cmd.CETFP = new DateTime(2022, 4, 20, 3, 2, 32, 83);
            cmd.CERECORD_EnsemblePing = false;
            cmd.CEOUTPUT = AdcpCommands.AdcpOutputMode.Binary;
            cmd.CWS = 23.234f;
            cmd.CWT = 934.123f;
            cmd.CTD = 945.23f;
            cmd.CWSS = 111.345f;
            cmd.CHO = 83.23f;
            cmd.CHS = HeadingSrc.INTERNAL;
            cmd.CVSF = 234.2345f;
            cmd.C232B = Baudrate.BAUD_19200;
            cmd.C485B = Baudrate.BAUD_460800;
            cmd.C422B = Baudrate.BAUD_38400;

            AdcpCommands cmd1 = cmd;

            #region Mode
            Assert.AreEqual(AdcpCommands.AdcpMode.DVL, cmd.Mode, "Mode is incorrect.");
            Assert.AreEqual(AdcpCommands.AdcpMode.DVL, cmd1.Mode, "Mode 1 is incorrect.");
            Assert.AreEqual(cmd.Mode, cmd1.Mode, "Mode equal is incorrect.");
            #endregion

            #region CEI
            Assert.AreEqual(new TimeValue(1, 2, 3, 4), cmd.CEI, "CEI is incorrect.");
            Assert.AreEqual(new TimeValue(1, 2, 3, 4), cmd1.CEI, "CEI 1 is incorrect.");
            Assert.AreEqual(cmd.CEI, cmd1.CEI, "CEI equal is incorrect.");
            #endregion

            #region CEPO
            Assert.AreEqual("33", cmd.CEPO, "CEPO is incorrect.");
            Assert.AreEqual("33", cmd1.CEPO, "CEPO 1 is incorrect.");
            Assert.AreEqual(cmd.CEPO, cmd1.CEPO, "CEPO equal is incorrect.");
            #endregion

            #region CETFP_Year
            Assert.AreEqual(2022, cmd.CETFP.Year, "CETFP_Year is incorrect.");
            Assert.AreEqual(2022, cmd1.CETFP.Year, "CETFP_Year 1 is incorrect.");
            Assert.AreEqual(cmd.CETFP.Year, cmd1.CETFP.Year, "CETFP_Year equal is incorrect.");
            #endregion

            #region CETFP_Month
            Assert.AreEqual(4, cmd.CETFP.Month, "CETFP_Month is incorrect.");
            Assert.AreEqual(4, cmd1.CETFP.Month, "CETFP_Month 1 is incorrect.");
            Assert.AreEqual(cmd.CETFP.Month, cmd1.CETFP.Month, "CETFP_Month equal is incorrect.");
            #endregion

            #region CETFP_Day
            Assert.AreEqual(20, cmd.CETFP.Day, "CETFP_Day is incorrect.");
            Assert.AreEqual(20, cmd1.CETFP.Day, "CETFP_Day 1 is incorrect.");
            Assert.AreEqual(cmd.CETFP.Day, cmd1.CETFP.Day, "CETFP_Day equal is incorrect.");
            #endregion

            #region CETFP_Hour
            Assert.AreEqual(3, cmd.CETFP.Hour, "CETFP_Hour is incorrect.");
            Assert.AreEqual(3, cmd1.CETFP.Hour, "CETFP_Hour 1 is incorrect.");
            Assert.AreEqual(cmd.CETFP.Hour, cmd1.CETFP.Hour, "CETFP_Hour equal is incorrect.");
            #endregion

            #region CETFP_Minute
            Assert.AreEqual(2, cmd.CETFP.Minute, "CETFP_Minute is incorrect.");
            Assert.AreEqual(2, cmd1.CETFP.Minute, "CETFP_Minute 1 is incorrect.");
            Assert.AreEqual(cmd.CETFP.Minute, cmd1.CETFP.Minute, "CETFP_Minute equal is incorrect.");
            #endregion

            #region CETFP_Second
            Assert.AreEqual(32, cmd.CETFP.Second, "CETFP_Second is incorrect.");
            Assert.AreEqual(32, cmd1.CETFP.Second, "CETFP_Second 1 is incorrect.");
            Assert.AreEqual(cmd.CETFP.Second, cmd1.CETFP.Second, "CETFP_Second equal is incorrect.");
            #endregion

            #region CETFP_HunSec
            //Assert.AreEqual(83, cmd.CETFP_HunSec, "CETFP_HunSec is incorrect.");
            //Assert.AreEqual(83, cmd1.CETFP_HunSec, "CETFP_HunSec 1 is incorrect.");
            //Assert.AreEqual(cmd.CETFP_HunSec, cmd1.CETFP_HunSec, "CETFP_HunSec equal is incorrect.");
            #endregion

            #region CERECORD
            Assert.AreEqual(false, cmd.CERECORD_EnsemblePing, "CERECORD is incorrect.");
            Assert.AreEqual(false, cmd1.CERECORD_EnsemblePing, "CERECORD 1 is incorrect.");
            Assert.AreEqual(cmd.CERECORD_EnsemblePing, cmd1.CERECORD_EnsemblePing, "CERECORD equal is incorrect.");
            #endregion

            #region CEOUTPUT
            Assert.AreEqual(AdcpCommands.AdcpOutputMode.Binary, cmd.CEOUTPUT, "CEOUTPUT is incorrect.");
            Assert.AreEqual(AdcpCommands.AdcpOutputMode.Binary, cmd1.CEOUTPUT, "CEOUTPUT 1 is incorrect.");
            Assert.AreEqual(cmd.CEOUTPUT, cmd1.CEOUTPUT, "CEOUTPUT equal is incorrect.");
            #endregion

            #region CWS
            Assert.AreEqual(23.234f, cmd.CWS, "CWS is incorrect.");
            Assert.AreEqual(23.234f, cmd1.CWS, "CWS 1 is incorrect.");
            Assert.AreEqual(cmd.CWS, cmd1.CWS, "CWS equal is incorrect.");
            #endregion

            #region CWT
            Assert.AreEqual(934.123f, cmd.CWT, "CWT is incorrect.");
            Assert.AreEqual(934.123f, cmd1.CWT, "CWT 1 is incorrect.");
            Assert.AreEqual(cmd.CWT, cmd1.CWT, "CWT equal is incorrect.");
            #endregion

            #region CTD
            Assert.AreEqual(945.23f, cmd.CTD, "CTD is incorrect.");
            Assert.AreEqual(945.23f, cmd1.CTD, "CTD 1 is incorrect.");
            Assert.AreEqual(cmd.CTD, cmd1.CTD, "CTD equal is incorrect.");
            #endregion

            #region CWSS
            Assert.AreEqual(111.345f, cmd.CWSS, "CWSS is incorrect.");
            Assert.AreEqual(111.345f, cmd1.CWSS, "CWSS 1 is incorrect.");
            Assert.AreEqual(cmd.CWSS, cmd1.CWSS, "CWSS equal is incorrect.");
            #endregion

            #region CHO
            Assert.AreEqual(83.23f, cmd.CHO, "CHO is incorrect.");
            Assert.AreEqual(83.23f, cmd1.CHO, "CHO 1 is incorrect.");
            Assert.AreEqual(cmd.CHO, cmd1.CHO, "CHO equal is incorrect.");
            #endregion

            #region CHS
            Assert.AreEqual(HeadingSrc.INTERNAL, cmd.CHS, "CHS is incorrect.");
            Assert.AreEqual(HeadingSrc.INTERNAL, cmd1.CHS, "CHS 1 is incorrect.");
            Assert.AreEqual(cmd.CHS, cmd1.CHS, "CHS equal is incorrect.");
            #endregion

            #region CVSF
            Assert.AreEqual(234.2345f, cmd.CVSF, "CVSF is incorrect.");
            Assert.AreEqual(234.2345f, cmd1.CVSF, "CVSF 1 is incorrect.");
            Assert.AreEqual(cmd.CVSF, cmd1.CVSF, "CVSF equal is incorrect.");
            #endregion

            #region C232B
            Assert.AreEqual(Baudrate.BAUD_19200, cmd.C232B, "C232B is incorrect.");
            Assert.AreEqual(Baudrate.BAUD_19200, cmd1.C232B, "C232B 1 is incorrect.");
            Assert.AreEqual(cmd.C232B, cmd1.C232B, "C232B equal is incorrect.");
            #endregion

            #region C485B
            Assert.AreEqual(Baudrate.BAUD_460800, cmd.C485B, "C485B is incorrect.");
            Assert.AreEqual(Baudrate.BAUD_460800, cmd1.C485B, "C485B 1 is incorrect.");
            Assert.AreEqual(cmd.C485B, cmd1.C485B, "C485B equal is incorrect.");
            #endregion

            #region C485B
            Assert.AreEqual(Baudrate.BAUD_38400, cmd.C422B, "C422B is incorrect.");
            Assert.AreEqual(Baudrate.BAUD_38400, cmd1.C422B, "C422B 1 is incorrect.");
            Assert.AreEqual(cmd.C422B, cmd1.C422B, "C422B equal is incorrect.");
            #endregion
        }

        /// <summary>
        /// Test Equals sign.
        /// </summary>
        [Test]
        public void TestEqual_Copy()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.Mode = AdcpCommands.AdcpMode.DVL;
            cmd.CEI = new TimeValue(1, 2, 3, 4);
            cmd.CEPO = "33";
            //cmd.CETFP_Year = 2022;
            //cmd.CETFP_Month = 4;
            //cmd.CETFP_Day = 20;
            //cmd.CETFP_Hour = 3;
            //cmd.CETFP_Minute = 2;
            //cmd.CETFP_Second = 32;
            //cmd.CETFP_HunSec = 83;
            cmd.CETFP = new DateTime(2022, 4, 20, 3, 2, 32, 83);
            cmd.CERECORD_EnsemblePing = false;
            cmd.CEOUTPUT = AdcpCommands.AdcpOutputMode.Binary;
            cmd.CWS = 23.234f;
            cmd.CWT = 934.123f;
            cmd.CTD = 945.23f;
            cmd.CWSS = 111.345f;
            cmd.CHO = 83.23f;
            cmd.CHS = HeadingSrc.INTERNAL;
            cmd.CVSF = 234.2345f;
            cmd.C232B = Baudrate.BAUD_19200;
            cmd.C485B = Baudrate.BAUD_460800;
            cmd.C422B = Baudrate.BAUD_38400;

            AdcpCommands cmd1 = new AdcpCommands();
            cmd1 = cmd;

            #region Mode
            Assert.AreEqual(AdcpCommands.AdcpMode.DVL, cmd.Mode, "Mode is incorrect.");
            Assert.AreEqual(AdcpCommands.AdcpMode.DVL, cmd1.Mode, "Mode 1 is incorrect.");
            Assert.AreEqual(cmd.Mode, cmd1.Mode, "Mode equal is incorrect.");
            #endregion

            #region CEI
            Assert.AreEqual(new TimeValue(1, 2, 3, 4), cmd.CEI, "CEI is incorrect.");
            Assert.AreEqual(new TimeValue(1, 2, 3, 4), cmd1.CEI, "CEI 1 is incorrect.");
            Assert.AreEqual(cmd.CEI, cmd1.CEI, "CEI equal is incorrect.");
            #endregion

            #region CEPO
            Assert.AreEqual("33", cmd.CEPO, "CEPO is incorrect.");
            Assert.AreEqual("33", cmd1.CEPO, "CEPO 1 is incorrect.");
            Assert.AreEqual(cmd.CEPO, cmd1.CEPO, "CEPO equal is incorrect.");
            #endregion

            #region CETFP_Year
            Assert.AreEqual(2022, cmd.CETFP.Year, "CETFP_Year is incorrect.");
            Assert.AreEqual(2022, cmd1.CETFP.Year, "CETFP_Year 1 is incorrect.");
            Assert.AreEqual(cmd.CETFP.Year, cmd1.CETFP.Year, "CETFP_Year equal is incorrect.");
            #endregion

            #region CETFP_Month
            Assert.AreEqual(4, cmd.CETFP.Month, "CETFP_Month is incorrect.");
            Assert.AreEqual(4, cmd1.CETFP.Month, "CETFP_Month 1 is incorrect.");
            Assert.AreEqual(cmd.CETFP.Month, cmd1.CETFP.Month, "CETFP_Month equal is incorrect.");
            #endregion

            #region CETFP_Day
            Assert.AreEqual(20, cmd.CETFP.Day, "CETFP_Day is incorrect.");
            Assert.AreEqual(20, cmd1.CETFP.Day, "CETFP_Day 1 is incorrect.");
            Assert.AreEqual(cmd.CETFP.Day, cmd1.CETFP.Day, "CETFP_Day equal is incorrect.");
            #endregion

            #region CETFP_Hour
            Assert.AreEqual(3, cmd.CETFP.Hour, "CETFP_Hour is incorrect.");
            Assert.AreEqual(3, cmd1.CETFP.Hour, "CETFP_Hour 1 is incorrect.");
            Assert.AreEqual(cmd.CETFP.Hour, cmd1.CETFP.Hour, "CETFP_Hour equal is incorrect.");
            #endregion

            #region CETFP_Minute
            Assert.AreEqual(2, cmd.CETFP.Minute, "CETFP_Minute is incorrect.");
            Assert.AreEqual(2, cmd1.CETFP.Minute, "CETFP_Minute 1 is incorrect.");
            Assert.AreEqual(cmd.CETFP.Minute, cmd1.CETFP.Minute, "CETFP_Minute equal is incorrect.");
            #endregion

            #region CETFP_Second
            Assert.AreEqual(32, cmd.CETFP.Second, "CETFP_Second is incorrect.");
            Assert.AreEqual(32, cmd1.CETFP.Second, "CETFP_Second 1 is incorrect.");
            Assert.AreEqual(cmd.CETFP.Second, cmd1.CETFP.Second, "CETFP_Second equal is incorrect.");
            #endregion

            #region CETFP_HunSec
            //Assert.AreEqual(83, cmd.CETFP_HunSec, "CETFP_HunSec is incorrect.");
            //Assert.AreEqual(83, cmd1.CETFP_HunSec, "CETFP_HunSec 1 is incorrect.");
            //Assert.AreEqual(cmd.CETFP_HunSec, cmd1.CETFP_HunSec, "CETFP_HunSec equal is incorrect.");
            #endregion

            #region CERECORD
            Assert.AreEqual(false, cmd.CERECORD_EnsemblePing, "CERECORD is incorrect.");
            Assert.AreEqual(false, cmd1.CERECORD_EnsemblePing, "CERECORD 1 is incorrect.");
            Assert.AreEqual(cmd.CERECORD_EnsemblePing, cmd1.CERECORD_EnsemblePing, "CERECORD equal is incorrect.");
            #endregion

            #region CEOUTPUT
            Assert.AreEqual(AdcpCommands.AdcpOutputMode.Binary, cmd.CEOUTPUT, "CEOUTPUT is incorrect.");
            Assert.AreEqual(AdcpCommands.AdcpOutputMode.Binary, cmd1.CEOUTPUT, "CEOUTPUT 1 is incorrect.");
            Assert.AreEqual(cmd.CEOUTPUT, cmd1.CEOUTPUT, "CEOUTPUT equal is incorrect.");
            #endregion

            #region CWS
            Assert.AreEqual(23.234f, cmd.CWS, "CWS is incorrect.");
            Assert.AreEqual(23.234f, cmd1.CWS, "CWS 1 is incorrect.");
            Assert.AreEqual(cmd.CWS, cmd1.CWS, "CWS equal is incorrect.");
            #endregion

            #region CWT
            Assert.AreEqual(934.123f, cmd.CWT, "CWT is incorrect.");
            Assert.AreEqual(934.123f, cmd1.CWT, "CWT 1 is incorrect.");
            Assert.AreEqual(cmd.CWT, cmd1.CWT, "CWT equal is incorrect.");
            #endregion

            #region CTD
            Assert.AreEqual(945.23f, cmd.CTD, "CTD is incorrect.");
            Assert.AreEqual(945.23f, cmd1.CTD, "CTD 1 is incorrect.");
            Assert.AreEqual(cmd.CTD, cmd1.CTD, "CTD equal is incorrect.");
            #endregion

            #region CWSS
            Assert.AreEqual(111.345f, cmd.CWSS, "CWSS is incorrect.");
            Assert.AreEqual(111.345f, cmd1.CWSS, "CWSS 1 is incorrect.");
            Assert.AreEqual(cmd.CWSS, cmd1.CWSS, "CWSS equal is incorrect.");
            #endregion

            #region CHO
            Assert.AreEqual(83.23f, cmd.CHO, "CHO is incorrect.");
            Assert.AreEqual(83.23f, cmd1.CHO, "CHO 1 is incorrect.");
            Assert.AreEqual(cmd.CHO, cmd1.CHO, "CHO equal is incorrect.");
            #endregion

            #region CHS
            Assert.AreEqual(HeadingSrc.INTERNAL, cmd.CHS, "CHS is incorrect.");
            Assert.AreEqual(HeadingSrc.INTERNAL, cmd1.CHS, "CHS 1 is incorrect.");
            Assert.AreEqual(cmd.CHS, cmd1.CHS, "CHS equal is incorrect.");
            #endregion

            #region CVSF
            Assert.AreEqual(234.2345f, cmd.CVSF, "CVSF is incorrect.");
            Assert.AreEqual(234.2345f, cmd1.CVSF, "CVSF 1 is incorrect.");
            Assert.AreEqual(cmd.CVSF, cmd1.CVSF, "CVSF equal is incorrect.");
            #endregion

            #region C232B
            Assert.AreEqual(Baudrate.BAUD_19200, cmd.C232B, "C232B is incorrect.");
            Assert.AreEqual(Baudrate.BAUD_19200, cmd1.C232B, "C232B 1 is incorrect.");
            Assert.AreEqual(cmd.C232B, cmd1.C232B, "C232B equal is incorrect.");
            #endregion

            #region C485B
            Assert.AreEqual(Baudrate.BAUD_460800, cmd.C485B, "C485B is incorrect.");
            Assert.AreEqual(Baudrate.BAUD_460800, cmd1.C485B, "C485B 1 is incorrect.");
            Assert.AreEqual(cmd.C485B, cmd1.C485B, "C485B equal is incorrect.");
            #endregion

            #region C485B
            Assert.AreEqual(Baudrate.BAUD_38400, cmd.C422B, "C422B is incorrect.");
            Assert.AreEqual(Baudrate.BAUD_38400, cmd1.C422B, "C422B 1 is incorrect.");
            Assert.AreEqual(cmd.C422B, cmd1.C422B, "C422B equal is incorrect.");
            #endregion
        }

        #endregion

        #region Mode Command String

        /// <summary>
        /// Test getting the Mode command string.
        /// </summary>
        [Test]
        public void TestModeCmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.Mode = AdcpCommands.AdcpMode.PROFILE;

            Assert.AreEqual(AdcpCommands.AdcpMode.PROFILE, cmd.Mode, "Mode is incorrect.");
            Assert.AreEqual("CPROFILE", cmd.Mode_ToString(), "Mode String is incorrect.");
            Assert.AreEqual(AdcpCommands.CMD_CPROFILE, cmd.Mode_ToString(), "Mode String is incorrect.");
            Assert.AreEqual(AdcpCommands.CMD_CPROFILE, cmd.Mode_CmdStr(), "Mode Command String is incorrect.");
        }

        /// <summary>
        /// Test setting the Mode command.
        /// </summary>
        [Test]
        public void TestModeCmdStr1()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.Mode = AdcpCommands.AdcpMode.DVL;

            Assert.AreEqual(AdcpCommands.AdcpMode.DVL, cmd.Mode, "Mode is incorrect.");
            Assert.AreEqual("CDVL", cmd.Mode_ToString(), "Mode String is incorrect.");
            Assert.AreEqual(AdcpCommands.CMD_CDVL, cmd.Mode_ToString(), "Mode String is incorrect.");
            Assert.AreEqual(AdcpCommands.CMD_CDVL, cmd.Mode_CmdStr(), "Mode Command String is incorrect.");
        }

        #endregion

        #region Time Command String

        /// <summary>
        /// Test getting the Time command string.
        /// </summary>
        [Test]
        public void TestLocalTimeCmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            string dateTime = DateTime.Now.ToString("yyyy/MM/dd,HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US"));

            Assert.AreEqual(AdcpCommands.CMD_STIME + " " + dateTime, cmd.LocalTime_CmdStr(), "Time Command String is incorrect.");
        }

        /// <summary>
        /// Test getting the Time command string.
        /// </summary>
        [Test]
        public void TestUtcTimeCmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            string dateTime = DateTime.Now.ToUniversalTime().ToString("yyyy/MM/dd,HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US"));

            Assert.AreEqual(AdcpCommands.CMD_STIME + " " + dateTime, cmd.GmtTime_CmdStr(), "Time Command String is incorrect.");
        }

        #endregion

        #region CEI Command String

        /// <summary>
        /// Test getting the CEI command string.
        /// </summary>
        [Test]
        public void TestCEICmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CEI = new TimeValue(1, 2, 3, 4);

            Assert.AreEqual("CEI 01:02:03.04", cmd.CEI_CmdStr(), "CEI Command String is incorrect.");
        }

        #endregion

        #region CETFP Command String

        /// <summary>
        /// Test getting the CETFP command string.
        /// </summary>
        [Test]
        public void TestCETFPCmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            //cmd.CETFP_Year = 2012;
            //cmd.CETFP_Month = 11;
            //cmd.CETFP_Day = 21;
            //cmd.CETFP_Hour = 12;
            //cmd.CETFP_Minute = 03;
            //cmd.CETFP_Second = 15;
            //cmd.CETFP_HunSec = 67;
            cmd.CETFP = new DateTime(2012, 11, 21, 12, 03, 15, (int)(67 * MathHelper.HUNSEC_TO_MILLISEC));

            Assert.AreEqual("CETFP 2012/11/21,12:03:15.67", cmd.CETFP_CmdStr(), "CETFP Command String is incorrect.");
        }

        #endregion

        #region CERECORD Command String

        /// <summary>
        /// Test getting the CERECORD command string.
        /// </summary>
        [Test]
        public void TestCERECORDCmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CERECORD_EnsemblePing = false;
            cmd.CERECORD_SinglePing = false;

            Assert.AreEqual("CERECORD 0,0", cmd.CERECORD_CmdStr(), "CERECORD Command String is incorrect.");
        }

        /// <summary>
        /// Test getting the CERECORD command string.
        /// </summary>
        [Test]
        public void TestCERECORDCmdStr1()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CERECORD_EnsemblePing = true;
            cmd.CERECORD_SinglePing = true;

            Assert.AreEqual("CERECORD 1,1", cmd.CERECORD_CmdStr(), "CERECORD Command String is incorrect.");
        }

        #endregion

        #region CEOUTPUT Command String

        /// <summary>
        /// Test getting the CEOUTPUT command string.
        /// </summary>
        [Test]
        public void TestCEOUTPUTCmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CEOUTPUT = AdcpCommands.AdcpOutputMode.Disable;

            Assert.AreEqual("CEOUTPUT 0", cmd.CEOUTPUT_CmdStr(), "CEOUTPUT Command String is incorrect.");
        }

        /// <summary>
        /// Test getting the CEOUTPUT command string.
        /// </summary>
        [Test]
        public void TestCEOUTPUTCmdStr1()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CEOUTPUT = AdcpCommands.AdcpOutputMode.Binary;

            Assert.AreEqual("CEOUTPUT 1", cmd.CEOUTPUT_CmdStr(), "CEOUTPUT Command String is incorrect.");
        }

        /// <summary>
        /// Test getting the CEOUTPUT command string.
        /// </summary>
        [Test]
        public void TestCEOUTPUTCmdStr2()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CEOUTPUT = AdcpCommands.AdcpOutputMode.ASCII;

            Assert.AreEqual("CEOUTPUT 2", cmd.CEOUTPUT_CmdStr(), "CEOUTPUT Command String is incorrect.");
        }

        /// <summary>
        /// Test getting the CEOUTPUT command string.
        /// </summary>
        [Test]
        public void TestCEOUTPUTCmdStr3()
        {
            AdcpCommands cmd = new AdcpCommands();

            Assert.AreEqual("CEOUTPUT 1", cmd.CEOUTPUT_CmdStr(), "CEOUTPUT Command String is incorrect.");
        }

        #endregion

        #region CEPO Command String

        /// <summary>
        /// Test getting the CEPO command string.
        /// </summary>
        [Test]
        public void TestCEPOCmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CEPO = "2323";

            Assert.AreEqual("CEPO 2323", cmd.CEPO_CmdStr(), "CEPO Command String is incorrect.");
        }

        #endregion

        #region CWS Command String

        /// <summary>
        /// Test getting the CWS command string.
        /// </summary>
        [Test]
        public void TestCWSCmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CWS = 0.02456f;

            Assert.AreEqual("CWS 0.02456", cmd.CWS_CmdStr(), "CWS Command String is incorrect.");
        }

        #endregion

        #region CWT Command String

        /// <summary>
        /// Test getting the CWT command string.
        /// </summary>
        [Test]
        public void TestCWTCmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CWT = 0.024564f;

            Assert.AreEqual("CWT 0.024564", cmd.CWT_CmdStr(), "CWT Command String is incorrect.");
        }

        #endregion

        #region CTD Command String

        /// <summary>
        /// Test getting the CTD command string.
        /// </summary>
        [Test]
        public void TestCTDCmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CTD = 0.0245645f;

            Assert.AreEqual("CTD 0.0245645", cmd.CTD_CmdStr(), "CTD Command String is incorrect.");
        }

        #endregion

        #region CWSS Command String

        /// <summary>
        /// Test getting the CTD command string.
        /// </summary>
        [Test]
        public void TestCWSSCmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CWSS = 0.02456456f;

            Assert.AreEqual("CWSS 0.02456456", cmd.CWSS_CmdStr(), "CWSS Command String is incorrect.");
        }

        #endregion

        #region CHS Command String

        /// <summary>
        /// Test getting the CHS command string.
        /// </summary>
        [Test]
        public void TestCHSCmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CHS = HeadingSrc.INTERNAL;

            Assert.AreEqual("CHS 1", cmd.CHS_CmdStr(), "CHS Command String is incorrect.");
        }

        /// <summary>
        /// Test getting the CHS command string.
        /// </summary>
        [Test]
        public void TestCHSCmdStr1()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CHS = HeadingSrc.SERIAL;

            Assert.AreEqual("CHS 2", cmd.CHS_CmdStr(), "CHS Command String is incorrect.");
        }

        #endregion

        #region CHO Command String

        /// <summary>
        /// Test getting the CHO command string.
        /// </summary>
        [Test]
        public void TestCHOCmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CHO = -126.254f;

            Assert.AreEqual("CHO -126.254", cmd.CHO_CmdStr(), "CHO Command String is incorrect.");
        }

        /// <summary>
        /// Test getting the CHO command string.
        /// </summary>
        [Test]
        public void TestCHOCmdStr1()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CHO = 126.254f;

            Assert.AreEqual("CHO 126.254", cmd.CHO_CmdStr(), "CHO Command String is incorrect.");
        }

        #endregion

        #region CVSF Command String

        /// <summary>
        /// Test getting the CVSF command string.
        /// </summary>
        [Test]
        public void TestCVSFCmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.CVSF = -126.254f;

            Assert.AreEqual("CVSF -126.254", cmd.CVSF_CmdStr(), "CVSF Command String is incorrect.");
        }

        #endregion

        #region C232B Command String

        /// <summary>
        /// Test setting the C232B command.
        /// </summary>
        [Test]
        public void TestC232B_2400_CmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C232B = Baudrate.BAUD_2400;

            Assert.AreEqual("C232B 2400", cmd.C232B_CmdStr(), "C232B Command String BAUD_2400 is incorrect.");
        }

        /// <summary>
        /// Test setting the C232B command.
        /// </summary>
        [Test]
        public void TestC232B_4800_CmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C232B = Baudrate.BAUD_4800;

            Assert.AreEqual("C232B 4800", cmd.C232B_CmdStr(), "C232B Command String BAUD_4800 is incorrect.");
        }

        /// <summary>
        /// Test setting the C232B command.
        /// </summary>
        [Test]
        public void TestC232B_9600_CmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C232B = Baudrate.BAUD_9600;

            Assert.AreEqual("C232B 9600", cmd.C232B_CmdStr(), "C232B Command String BAUD_9600 is incorrect.");
        }

        /// <summary>
        /// Test setting the C232B command.
        /// </summary>
        [Test]
        public void TestC232B_19200_CmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C232B = Baudrate.BAUD_19200;

            Assert.AreEqual("C232B 19200", cmd.C232B_CmdStr(), "C232B Command String BAUD_19200 is incorrect.");
        }

        /// <summary>
        /// Test setting the C232B command.
        /// </summary>
        [Test]
        public void TestC232B_38400_CmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C232B = Baudrate.BAUD_38400;

            Assert.AreEqual("C232B 38400", cmd.C232B_CmdStr(), "C232B Command String BAUD_38400 is incorrect.");
        }

        /// <summary>
        /// Test setting the C232B command.
        /// </summary>
        [Test]
        public void TestC232B_115200_CmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C232B = Baudrate.BAUD_115200;

            Assert.AreEqual("C232B 115200", cmd.C232B_CmdStr(), "C232B Command String BAUD_115200 is incorrect.");
        }

        /// <summary>
        /// Test setting the C232B command.
        /// </summary>
        [Test]
        public void TestC232B_230400_CmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C232B = Baudrate.BAUD_230400;

            Assert.AreEqual("C232B 230400", cmd.C232B_CmdStr(), "C232B Command String BAUD_230400 is incorrect.");
        }

        /// <summary>
        /// Test setting the C232B command.
        /// </summary>
        [Test]
        public void TestC232B_460800_CmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C232B = Baudrate.BAUD_460800;

            Assert.AreEqual("C232B 460800", cmd.C232B_CmdStr(), "C232B Command String BAUD_460800 is incorrect.");
        }

        /// <summary>
        /// Test setting the C232B command.
        /// </summary>
        [Test]
        public void TestC232B_921600_CmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C232B = Baudrate.BAUD_921600;

            Assert.AreEqual("C232B 921600", cmd.C232B_CmdStr(), "C232B Command String BAUD_921600 is incorrect.");
        }

        #endregion

        #region C485B Command String

        /// <summary>
        /// Test setting the C485B command.
        /// </summary>
        [Test]
        public void TestC485BB_2400_CmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C485B = Baudrate.BAUD_2400;

            Assert.AreEqual("C485B 2400", cmd.C485B_CmdStr(), "C485B Command String BAUD_2400 is incorrect.");
        }

        /// <summary>
        /// Test setting the C485B command.
        /// </summary>
        [Test]
        public void TestC485B_4800_CmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C485B = Baudrate.BAUD_4800;

            Assert.AreEqual("C485B 4800", cmd.C485B_CmdStr(), "C485B Command String BAUD_4800 is incorrect.");
        }

        /// <summary>
        /// Test setting the C485B command.
        /// </summary>
        [Test]
        public void TestC485B_9600_CmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C485B = Baudrate.BAUD_9600;

            Assert.AreEqual("C485B 9600", cmd.C485B_CmdStr(), "C485B Command String BAUD_9600 is incorrect.");
        }

        /// <summary>
        /// Test setting the C485B command.
        /// </summary>
        [Test]
        public void TestC485B_19200_CmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C485B = Baudrate.BAUD_19200;

            Assert.AreEqual("C485B 19200", cmd.C485B_CmdStr(), "C485B Command String BAUD_19200 is incorrect.");
        }

        /// <summary>
        /// Test setting the C485B command.
        /// </summary>
        [Test]
        public void TestC485B_38400_CmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C485B = Baudrate.BAUD_38400;

            Assert.AreEqual("C485B 38400", cmd.C485B_CmdStr(), "C485B Command String BAUD_38400 is incorrect.");
        }

        /// <summary>
        /// Test setting the C485B command.
        /// </summary>
        [Test]
        public void TestC485B_115200_CmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C485B = Baudrate.BAUD_115200;

            Assert.AreEqual("C485B 115200", cmd.C485B_CmdStr(), "C485B Command String BAUD_115200 is incorrect.");
        }

        /// <summary>
        /// Test setting the C485B command.
        /// </summary>
        [Test]
        public void TestC485B_230400_CmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C485B = Baudrate.BAUD_230400;

            Assert.AreEqual("C485B 230400", cmd.C485B_CmdStr(), "C485B Command String BAUD_230400 is incorrect.");
        }

        /// <summary>
        /// Test setting the C485B command.
        /// </summary>
        [Test]
        public void TestC485B_460800_CmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C485B = Baudrate.BAUD_460800;

            Assert.AreEqual("C485B 460800", cmd.C485B_CmdStr(), "C485B Command String BAUD_460800 is incorrect.");
        }

        /// <summary>
        /// Test setting the C485B command.
        /// </summary>
        [Test]
        public void TestC485B_921600_CmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C485B = Baudrate.BAUD_921600;

            Assert.AreEqual("C485B 921600", cmd.C485B_CmdStr(), "C485B Command String BAUD_921600 is incorrect.");
        }

        #endregion

        #region C422B Command String

        /// <summary>
        /// Test setting the C422B command.
        /// </summary>
        [Test]
        public void TestC422B_2400_CmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C422B = Baudrate.BAUD_2400;

            Assert.AreEqual("C422B 2400", cmd.C422B_CmdStr(), "C422B Command String BAUD_2400 is incorrect.");
        }

        /// <summary>
        /// Test setting the C422B command.
        /// </summary>
        [Test]
        public void TestC422B_4800_CmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C422B = Baudrate.BAUD_4800;

            Assert.AreEqual("C422B 4800", cmd.C422B_CmdStr(), "C422B Command String BAUD_4800 is incorrect.");
        }

        /// <summary>
        /// Test setting the C422B command.
        /// </summary>
        [Test]
        public void TestC422B_9600_CmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C422B = Baudrate.BAUD_9600;

            Assert.AreEqual("C422B 9600", cmd.C422B_CmdStr(), "C422B Command String BAUD_9600 is incorrect.");
        }

        /// <summary>
        /// Test setting the C422B command.
        /// </summary>
        [Test]
        public void TestC422B_19200_CmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C422B = Baudrate.BAUD_19200;

            Assert.AreEqual("C422B 19200", cmd.C422B_CmdStr(), "C422B Command String BAUD_19200 is incorrect.");
        }

        /// <summary>
        /// Test setting the C422B command.
        /// </summary>
        [Test]
        public void TestC422B_38400_CmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C422B = Baudrate.BAUD_38400;

            Assert.AreEqual("C422B 38400", cmd.C422B_CmdStr(), "C422B Command String BAUD_38400 is incorrect.");
        }

        /// <summary>
        /// Test setting the C422B command.
        /// </summary>
        [Test]
        public void TestC422B_115200_CmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C422B = Baudrate.BAUD_115200;

            Assert.AreEqual("C422B 115200", cmd.C422B_CmdStr(), "C422B Command String BAUD_115200 is incorrect.");
        }

        /// <summary>
        /// Test setting the C422B command.
        /// </summary>
        [Test]
        public void TestC422B_230400_CmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C422B = Baudrate.BAUD_230400;

            Assert.AreEqual("C422B 230400", cmd.C422B_CmdStr(), "C422B Command String BAUD_230400 is incorrect.");
        }

        /// <summary>
        /// Test setting the C422B command.
        /// </summary>
        [Test]
        public void TestC422B_460800_CmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C422B = Baudrate.BAUD_460800;

            Assert.AreEqual("C422B 460800", cmd.C422B_CmdStr(), "C422B Command String BAUD_460800 is incorrect.");
        }

        /// <summary>
        /// Test setting the C422B command.
        /// </summary>
        [Test]
        public void TestC422B_921600_CmdStr()
        {
            AdcpCommands cmd = new AdcpCommands();

            cmd.C422B = Baudrate.BAUD_921600;

            Assert.AreEqual("C422B 921600", cmd.C422B_CmdStr(), "C422B Command String BAUD_921600 is incorrect.");
        }

        #endregion
    }
}
