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
 * 11/29/2011      RC          Initial coding
 * 
 */

using DotSpatial.Positioning;
using System.Text;
namespace RTI
{

    /// <summary>
    /// A NMEA styte message received from the ADCP in 
    /// DVL Mode.  Data consist of Bottom Track and Water Track data.
    /// Data is in Earth Transform.
    /// </summary>
    public sealed class Prti02Sentence : NmeaSentence, IStartTimeSentence, ISampleNumberSentence, ITemperatureSentence,
                                                        IBtVelocityEastSentence, IBtVelocityNorthSentence, IBtVelocityUpSentence,
                                                        IBtDepthSentence,
                                                        IWmVelocityEastSentence, IWmVelocityNorthSentence, IWmVelocityUpSentence,
                                                        IWmDepthSentence, IStatusSentence
    {
        /// <summary>
        /// Command word for PRTI02.
        /// </summary>
        public const string CMD_WORD_PRTI02 = "PRTI02";

        #region Properties

        /// <summary>
        /// Start time of this sample in hundreds of seconds since
        /// power up or user reset.
        /// </summary>
        private int _startTime;

        /// <summary>
        /// Sample Number.
        /// </summary>
        private int _sampleNumber;

        /// <summary>
        /// Temperature in hundreds of degrees Celsius.
        /// (Measured from the transducer)
        /// </summary>
        private int _temperature;

        /// <summary>
        /// Bottom Track East velocity component mm/s.
        /// (Earth Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        private Speed _btVelocityEast;

        /// <summary>
        /// Bottom Track North velocity component mm/s.
        /// (Earth Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        private Speed _btVelocityNorth;


        /// <summary>
        /// Bottom Track Up velocity component mm/s.
        /// (Earth Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        private Speed _btVelocityUp;

        /// <summary>
        /// Depth below transducer in mm.  
        /// (Range to the bottom in front of the transducer, 
        /// 0 = no detection)
        /// </summary>
        private Distance _depth;

        /// <summary>
        /// Water Mass East velocity component mm/s.
        /// (Earth Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        private Speed _wmVelocityEast;

        /// <summary>
        /// Water Mass North velocity component mm/s.
        /// (Earth Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        private Speed _wmVelocityNorth;


        /// <summary>
        /// Water Mass Up velocity component mm/s.
        /// (Earth Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        private Speed _wmVelocityUp;

        /// <summary>
        /// Depth of water mass measurement in mm.
        /// (Position of the bin in front of the transducer)
        /// </summary>
        private Distance _wmDepth;

        /// <summary>
        /// Built in test and status bits in hexadecimal.
        /// (0000 = OK)
        /// </summary>
        private Status _status;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new Prti02Sentence
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <remarks></remarks>
        public Prti02Sentence(string sentence)
            : base(sentence)
        {
            // If the sentence is valid, parse the data
            if (IsValid)
            {
                OnSentenceChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Prti02Sentence"/> class.
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <param name="commandWord">The command word.</param>
        /// <param name="words">The words.</param>
        /// <param name="validChecksum">The valid checksum.</param>
        /// <remarks></remarks>
        internal Prti02Sentence(string sentence, string commandWord, string[] words, string validChecksum)
            : base(sentence, commandWord, words, validChecksum)
        { }

        /// <summary>
        /// Constructor.  Set all the values and add to a string.
        /// </summary>
        /// <param name="time">Start time.</param>
        /// <param name="sampleNum">Sample Number.</param>
        /// <param name="temp">Temperature.</param>
        /// <param name="btVelEast">Bottom Track Velocity East.</param>
        /// <param name="btVelNorth">Bottom Track Velocity North.</param>
        /// <param name="btVelUp">Bottom Track Velocity Up.</param>
        /// <param name="depth">Depth.</param>
        /// <param name="wmVelEast">Water Mass Velocity East.</param>
        /// <param name="wmVelNorth">Water Mass Velocity North.</param>
        /// <param name="wmVelUp">Water Mass Velocity Up.</param>
        /// <param name="wmDepth">Water Mass Depth.</param>
        /// <param name="status">Status.</param>
        public Prti02Sentence(string time, string sampleNum, string temp,
                                string btVelEast, string btVelNorth, string btVelUp,
                                string depth,
                                string wmVelEast, string wmVelNorth, string wmVelUp,
                                string wmDepth, string status)
        {
            // Use a string builder to create the sentence text
            StringBuilder builder = new StringBuilder(128);

            #region Append the command word

            // Append the command word
            builder.Append("$");
            builder.Append(CMD_WORD_PRTI02);

            #endregion Append the command word

            // Append a comma
            builder.Append(',');

            #region Append Sample time

            builder.Append(time);

            #endregion

            // Append a comma
            builder.Append(',');

            #region Append Sample Number

            builder.Append(sampleNum);

            #endregion

            // Append a comma
            builder.Append(',');

            #region Append Temperature

            builder.Append(temp);

            #endregion

            // Append a comma
            builder.Append(',');

            #region Append Bottom Track Velocity

            builder.Append(btVelEast);
            builder.Append(',');
            builder.Append(btVelNorth);
            builder.Append(',');
            builder.Append(btVelUp);

            #endregion

            // Append a comma
            builder.Append(',');

            #region Append Depth below transducer

            builder.Append(depth);

            #endregion

            // Append a comma
            builder.Append(',');

            #region Append Water Mass Velocity

            builder.Append(wmVelEast);
            builder.Append(',');
            builder.Append(wmVelNorth);
            builder.Append(',');
            builder.Append(wmVelUp);

            #endregion

            // Append a comma
            builder.Append(',');

            #region Append Depth of water mass measurement

            builder.Append(wmDepth);

            #endregion

            // Append a comma
            builder.Append(',');

            #region Append Status

            builder.Append(status);

            #endregion

            // Set this object's sentence
            SetSentence(builder.ToString());

            // Finally, append the checksum
            AppendChecksum();

            // Set all variables
            OnSentenceChanged();
        }

        #endregion


        /// <summary>
        /// Called when [sentence changed].
        /// </summary>
        /// <remarks></remarks>
        protected override void OnSentenceChanged()
        {
            // Parse the basic sentence information
            base.OnSentenceChanged();

            // Cache the words
            string[] words = Words;
            int wordCount = words.Length;

            #region Start Time

            // Do we have enough data to process the Start time?
            if (wordCount >= 1 && words[0].Length != 0)
            {
                _startTime = int.Parse(words[0], NmeaCultureInfo);
            }
            else
            {
                _startTime = 0;
            }

            #endregion

            #region Sample Number

            // Do we have enough data to process the Sample number?
            if (wordCount >= 2 && words[1].Length != 0)
            {
                _sampleNumber = int.Parse(words[1], NmeaCultureInfo);
            }
            else
            {
                _sampleNumber = 0;
            }

            #endregion

            #region Temperature

            // Do we have enough data to process the Temperature?
            if (wordCount >= 3 && words[2].Length != 0)
            {
                _temperature = int.Parse(words[2], NmeaCultureInfo);
            }
            else
            {
                _temperature = 0;
            }

            #endregion

            #region Bottom Track East Velocity

            // Do we have enough data to process the Bottom Track East Velocity?
            if (wordCount >= 4 && words[3].Length != 0)
            {
                _btVelocityEast = new Speed(
                                        // Parse the numeric portion
                                        int.Parse(words[3], NmeaCultureInfo),
                                        // Use mm/s
                                        SpeedUnit.MillimetersPerSecond);
            }
            else
            {
                _btVelocityEast = Speed.Empty;
            }

            #endregion

            #region Bottom Track North Velocity

            // Do we have enough data to process the Bottom Track North Velocity?
            if (wordCount >= 5 && words[4].Length != 0)
            {
                _btVelocityNorth = new Speed(
                                        // Parse the numeric portion
                                        int.Parse(words[4], NmeaCultureInfo),
                                        // Use mm/s
                                        SpeedUnit.MillimetersPerSecond);
            }
            else
            {
                _btVelocityNorth = Speed.Empty;
            }

            #endregion

            #region Bottom Track Up Velocity

            // Do we have enough data to process the Bottom Track Up Velocity?
            if (wordCount >= 6 && words[5].Length != 0)
            {
                _btVelocityUp = new Speed(
                                        // Parse the numeric portion
                                        int.Parse(words[5], NmeaCultureInfo),
                                        // Use mm/s
                                        SpeedUnit.MillimetersPerSecond);
            }
            else
            {
                _btVelocityUp = Speed.Empty;
            }

            #endregion

            #region Depth

            // Do we have enough data to process the Depth?
            if (wordCount >= 7 && words[6].Length != 0)
            {
                _depth = new Distance(int.Parse(words[6], NmeaCultureInfo), DistanceUnit.Millimeters);
            }
            else
            {
                // Bottom not detected
                _depth = Distance.BottomNotDetectedDVL;
            }

            #endregion

            #region Water Mass East Velocity

            // Do we have enough data to process the Water Mass East Velocity?
            if (wordCount >= 8 && words[7].Length != 0)
            {
                _wmVelocityEast = new Speed(
                                        // Parse the numeric portion
                                        int.Parse(words[7], NmeaCultureInfo),
                                        // Use mm/s
                                        SpeedUnit.MillimetersPerSecond);
            }
            else
            {
                _wmVelocityEast = Speed.Empty;
            }

            #endregion

            #region Water Mass North Velocity

            // Do we have enough data to process the Water Mass North Velocity?
            if (wordCount >= 9 && words[8].Length != 0)
            {
                _wmVelocityNorth = new Speed(
                                        // Parse the numeric portion
                                        int.Parse(words[8], NmeaCultureInfo),
                                        // Use mm/s
                                        SpeedUnit.MillimetersPerSecond);
            }
            else
            {
                _wmVelocityNorth = Speed.Empty;
            }

            #endregion

            #region Water Mass Up Velocity

            // Do we have enough data to process the Water Mass Up Velocity?
            if (wordCount >= 10 && words[9].Length != 0)
            {
                _wmVelocityUp = new Speed(
                                        // Parse the numeric portion
                                        int.Parse(words[9], NmeaCultureInfo),
                                        // Use mm/s
                                        SpeedUnit.MillimetersPerSecond);
            }
            else
            {
                _wmVelocityUp = Speed.Empty;
            }

            #endregion

            #region Water Mass Depth

            // Do we have enough data to process the Depth?
            if (wordCount >= 11 && words[10].Length != 0)
            {
                _wmDepth = new Distance(int.Parse(words[10], NmeaCultureInfo), DistanceUnit.Millimeters);
            }
            else
            {
                // Water Mass depth not found
                _wmDepth = Distance.Empty;
            }

            #endregion

            #region Status

            // Do we have enough data to process the Status?
            if (wordCount >= 12 && words[11].Length != 0)
            {
                _status = new Status(int.Parse(words[11]));
            }
            else
            {
                // Status not found
                _status = new Status(0);
            }

            #endregion

        }

        /// <summary>
        /// Return the Start time.
        /// </summary>
        public int StartTime
        {
            get { return _startTime; }
        }

        /// <summary>
        /// Get the Sample number.
        /// </summary>
        public int SampleNumber
        {
            get { return _sampleNumber; }
        }

        /// <summary>
        /// Get the temperature in Celcius.
        /// </summary>
        public int Temperature
        {
            get { return _temperature; }
        }

        /// <summary>
        /// Get the Bottom Track Velocity East as mm/s.
        /// </summary>
        public Speed BottomTrackVelEast
        {
            get { return _btVelocityEast; }
        }

        /// <summary>
        /// Get the Bottom Track Velocity North as mm/s.
        /// </summary>
        public Speed BottomTrackVelNorth
        {
            get { return _btVelocityNorth; }
        }

        /// <summary>
        /// Get the Bottom Track Velocity Up as mm/s.
        /// </summary>
        public Speed BottomTrackVelUp
        {
            get { return _btVelocityUp; }
        }

        /// <summary>
        /// Get the Bottom Track Depth in mm.
        /// </summary>
        public Distance BottomTrackDepth
        {
            get { return _depth; }
        }

        /// <summary>
        /// Get the Water Mass Velocity East in mm/s.
        /// </summary>
        public Speed WaterMassVelEast
        {
            get { return _wmVelocityEast; }
        }

        /// <summary>
        /// Get the Water Mass Velocity North in mm/s.
        /// </summary>
        public Speed WaterMassVelNorth
        {
            get { return _wmVelocityNorth; }
        }

        /// <summary>
        /// Get the Water Mass Velocity Up in mm/s.
        /// </summary>
        public Speed WaterMassVelUp
        {
            get { return _wmVelocityUp; }
        }

        /// <summary>
        /// Get the Water Mass Depth in mm.
        /// </summary>
        public Distance WaterMassDepth
        {
            get { return _wmDepth; }
        }

        /// <summary>
        /// Get the status of the system.
        /// </summary>
        public Status SystemStatus
        {
            get { return _status; }
        }
    }

}