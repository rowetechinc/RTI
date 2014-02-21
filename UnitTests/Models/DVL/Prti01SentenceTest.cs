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
 * Date            Initials    Comments
 * -----------------------------------------------------------------
 * 11/23/2011      RC          Initial coding
 * 
 */

namespace RTI
{
    using System;
    using NUnit.Framework;
    using DotSpatial.Positioning;
    using System.Globalization;

    /// <summary>
    /// Unit test of the Prti01Setence object.
    /// </summary>
    [TestFixture]
    public class Prti01SetenceTest
    {
        /// <summary>
        /// Test the constructor taking a string as a
        /// NMEA string.
        /// </summary>
        [Test]
        public void TestConstructorSentence()
        {
            string nmea = "$PRTI01,379550,1,1468,-99999,-99999,-99999,0,,,,,0004*09";

            Prti01Sentence nm = new Prti01Sentence(nmea);

            Assert.AreEqual(true, nm.IsValid);
            Assert.AreEqual("$PRTI01", nm.CommandWord);
            Assert.AreEqual("09", nm.ExistingChecksum);
            Assert.AreEqual("09", nm.CorrectChecksum);
            Assert.AreEqual(12, nm.Words.Length);
            Assert.AreEqual(nmea, nm.Sentence);

            Assert.AreEqual("379550", nm.Words[0]);
            Assert.AreEqual("1", nm.Words[1]);
            Assert.AreEqual("1468", nm.Words[2]);
            Assert.AreEqual("-99999", nm.Words[3]);
            Assert.AreEqual("-99999", nm.Words[4]);
            Assert.AreEqual("-99999", nm.Words[5]);
            Assert.AreEqual("0", nm.Words[6]);
            Assert.AreEqual("", nm.Words[7]);
            Assert.AreEqual("", nm.Words[8]);
            Assert.AreEqual("", nm.Words[9]);
            Assert.AreEqual("", nm.Words[10]);
            Assert.AreEqual("0004", nm.Words[11]);

            Assert.AreEqual(379550, nm.StartTime);
            Assert.AreEqual(1, nm.SampleNumber);
            Assert.AreEqual(1468, nm.Temperature);
            Assert.AreEqual(Speed.BadDVL, nm.BottomTrackVelX);
            Assert.AreEqual(Speed.BadDVL, nm.BottomTrackVelY);
            Assert.AreEqual(Speed.BadDVL, nm.BottomTrackVelZ);
            Assert.AreEqual(new Distance(0, DistanceUnit.Millimeters), nm.BottomTrackDepth);
            Assert.AreEqual(Speed.Empty, nm.WaterMassVelX);
            Assert.AreEqual(Speed.Empty, nm.WaterMassVelY);
            Assert.AreEqual(Speed.Empty, nm.WaterMassVelZ);

            Assert.AreEqual(false, nm.SystemStatus.IsBottomTrack3BeamSolution());
            Assert.AreEqual(true, nm.SystemStatus.IsBottomTrackHold());
            Assert.AreEqual(false, nm.SystemStatus.IsReceiverTimeout());
            Assert.AreEqual(false, nm.SystemStatus.IsWaterTrack3BeamSolution());
            Assert.AreEqual(false, nm.SystemStatus.IsBottomTrackSearching());
        }

                /// <summary>
        /// Test the constructor taking all the values
        /// as strings.
        /// </summary>
        [Test]
        public void TestConstructorArg()
        {
            string nmea = "$PRTI01,379550,1,1468,-99999,-99999,-99999,0,,,,,0004,3,0*0A";
            //NmeaSentence sent = new NmeaSentence(nmea);

            Prti01Sentence nm = new Prti01Sentence("379550", "1", "1468",
                                                        Speed.BadDVL.Value.ToString(), Speed.BadDVL.Value.ToString(), Speed.BadDVL.Value.ToString(),
                                                        new Distance(0, DistanceUnit.Millimeters).Value.ToString(),
                                                        "", "", "",
                                                        "",
                                                        "0004", "3", "0");

            Assert.AreEqual(true, nm.IsValid);
            Assert.AreEqual(nmea, nm.Sentence);
            Assert.AreEqual("$PRTI01", nm.CommandWord);
            Assert.AreEqual("0A", nm.CorrectChecksum);
            Assert.AreEqual("0A", nm.ExistingChecksum);
            Assert.AreEqual(14, nm.Words.Length);

            Assert.AreEqual("379550", nm.Words[0]);
            Assert.AreEqual("1", nm.Words[1]);
            Assert.AreEqual("1468", nm.Words[2]);
            Assert.AreEqual("-99999", nm.Words[3]);
            Assert.AreEqual("-99999", nm.Words[4]);
            Assert.AreEqual("-99999", nm.Words[5]);
            Assert.AreEqual("0", nm.Words[6]);
            Assert.AreEqual("", nm.Words[7]);
            Assert.AreEqual("", nm.Words[8]);
            Assert.AreEqual("", nm.Words[9]);
            Assert.AreEqual("", nm.Words[10]);
            Assert.AreEqual("0004", nm.Words[11]);
            Assert.AreEqual("3", nm.Words[12]);
            Assert.AreEqual("0", nm.Words[13]);

            Assert.AreEqual(379550, nm.StartTime);
            Assert.AreEqual(1, nm.SampleNumber);
            Assert.AreEqual(1468, nm.Temperature);
            Assert.AreEqual(Speed.BadDVL, nm.BottomTrackVelX);
            Assert.AreEqual(Speed.BadDVL, nm.BottomTrackVelY);
            Assert.AreEqual(Speed.BadDVL, nm.BottomTrackVelZ);
            Assert.AreEqual(new Distance(0, DistanceUnit.Millimeters), nm.BottomTrackDepth);
            Assert.AreEqual(Speed.Empty, nm.WaterMassVelX);
            Assert.AreEqual(Speed.Empty, nm.WaterMassVelY);
            Assert.AreEqual(Speed.Empty, nm.WaterMassVelZ);

            Assert.AreEqual(false, nm.SystemStatus.IsBottomTrack3BeamSolution());
            Assert.AreEqual(true, nm.SystemStatus.IsBottomTrackHold());
            Assert.AreEqual(false, nm.SystemStatus.IsReceiverTimeout());
            Assert.AreEqual(false, nm.SystemStatus.IsWaterTrack3BeamSolution());
            Assert.AreEqual(false, nm.SystemStatus.IsBottomTrackSearching());
        }


    }
}