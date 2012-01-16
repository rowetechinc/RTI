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
 * 11/22/2011      RC          Initial coding
 * 
 */

using DotSpatial.Positioning;
using System.Text;
namespace RTI
{
    /// <summary>
    /// A NMEA styte message received from the ADCP in 
    /// DVL Mode.  Data consist of Bottom Track and Water Track data.
    /// Data in Instrument transform.
    /// </summary>
    public sealed class Prti01Sentence : NmeaSentence, IStartTimeSentence, ISampleNumberSentence, ITemperatureSentence, 
                                                        IBtVelocityXSentence, IBtVelocityYSentence, IBtVelocityZSentence,
                                                        IBtDepthSentence, 
                                                        IWmVelocityXSentence, IWmVelocityYSentence, IWmVelocityZSentence,
                                                        IWmDepthSentence, IStatusSentence
    {
        /// <summary>
        /// Command word for PRTI01.
        /// </summary>
        public const string CMD_WORD_PRTI01 = "PRTI01";

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
        /// Bottom Track X velocity component mm/s.
        /// (Instrument Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        private Speed _btVelocityX;

        /// <summary>
        /// Bottom Track Y velocity component mm/s.
        /// (Instrument Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        private Speed _btVelocityY;


        /// <summary>
        /// Bottom Track Z velocity component mm/s.
        /// (Instrument Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        private Speed _btVelocityZ;

        /// <summary>
        /// Depth below transducer in mm.  
        /// (Range to the bottom in front of the transducer, 
        /// 0 = no detection)
        /// </summary>
        private Distance _depth;

        /// <summary>
        /// Water Mass X velocity component mm/s.
        /// (Instrument Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        private Speed _wmVelocityX;

        /// <summary>
        /// Water Mass Y velocity component mm/s.
        /// (Instrument Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        private Speed _wmVelocityY;


        /// <summary>
        /// Water Mass Z velocity component mm/s.
        /// (Instrument Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        private Speed _wmVelocityZ;

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
        /// Creates a new Prti01Sentence
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <remarks></remarks>
        public Prti01Sentence(string sentence)
            : base(sentence)
        {
            // If the sentence is valid, parse the data
            if (IsValid)
            {
                OnSentenceChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Prti01Sentence"/> class.
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <param name="commandWord">The command word.</param>
        /// <param name="words">The words.</param>
        /// <param name="validChecksum">The valid checksum.</param>
        /// <remarks></remarks>
        internal Prti01Sentence(string sentence, string commandWord, string[] words, string validChecksum)
            : base(sentence, commandWord, words, validChecksum)
        { }

        /// <summary>
        /// Constructor.  Set all the values and add to a string.
        /// </summary>
        /// <param name="time">Start time.</param>
        /// <param name="sampleNum">Sample Number.</param>
        /// <param name="temp">Temperature.</param>
        /// <param name="btVelX">Bottom Track Velocity X.</param>
        /// <param name="btVelY">Bottom Track Velocity Y.</param>
        /// <param name="btVelZ">Bottom Track Velocity Z.</param>
        /// <param name="depth">Depth.</param>
        /// <param name="wmVelX">Water Mass Velocity X.</param>
        /// <param name="wmVelY">Water Mass Velocity Y.</param>
        /// <param name="wmVelZ">Water Mass Velocity Z.</param>
        /// <param name="wmDepth">Water Mass Depth.</param>
        /// <param name="status">Status.</param>
        public Prti01Sentence(string time, string sampleNum, string temp,
                                string btVelX, string btVelY, string btVelZ,
                                string depth,
                                string wmVelX, string wmVelY, string wmVelZ,
                                string wmDepth, string status)
        {
            // Use a string builder to create the sentence text
            StringBuilder builder = new StringBuilder(128);

            #region Append the command word

            // Append the command word
            builder.Append("$");
            builder.Append(CMD_WORD_PRTI01);

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

            builder.Append(btVelX);
            builder.Append(',');
            builder.Append(btVelY);
            builder.Append(',');
            builder.Append(btVelZ);

            #endregion

            // Append a comma
            builder.Append(',');

            #region Append Depth below transducer

            builder.Append(depth);

            #endregion

            // Append a comma
            builder.Append(',');

            #region Append Water Mass Velocity

            builder.Append(wmVelX);
            builder.Append(',');
            builder.Append(wmVelY);
            builder.Append(',');
            builder.Append(wmVelZ);

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
                _sampleNumber =  int.Parse(words[1], NmeaCultureInfo);
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

            #region Bottom Track X Velocity

            // Do we have enough data to process the Bottom Track X Velocity?
            if (wordCount >= 4 && words[3].Length != 0)
            {
                _btVelocityX = new Speed(
                                        // Parse the numeric portion
                                        int.Parse(words[3], NmeaCultureInfo),
                                        // Use mm/s
                                        SpeedUnit.MillimetersPerSecond);
            }
            else
            {
                _btVelocityX = Speed.Empty;
            }

            #endregion

            #region Bottom Track Y Velocity

            // Do we have enough data to process the Bottom Track Y Velocity?
            if (wordCount >= 5 && words[4].Length != 0)
            {
                _btVelocityY = new Speed(
                                        // Parse the numeric portion
                                        int.Parse(words[4], NmeaCultureInfo),
                                        // Use mm/s
                                        SpeedUnit.MillimetersPerSecond);
            }
            else
            {
                _btVelocityY = Speed.Empty;
            }

            #endregion

            #region Bottom Track Z Velocity

            // Do we have enough data to process the Bottom Track Z Velocity?
            if (wordCount >= 6 && words[5].Length != 0)
            {
                _btVelocityZ = new Speed(
                                        // Parse the numeric portion
                                        int.Parse(words[5], NmeaCultureInfo),
                                        // Use mm/s
                                        SpeedUnit.MillimetersPerSecond);
            }
            else
            {
                _btVelocityZ = Speed.Empty;
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

            #region Water Mass X Velocity

            // Do we have enough data to process the Water Mass X Velocity?
            if (wordCount >= 8 && words[7].Length != 0)
            {
                _wmVelocityX = new Speed(
                                        // Parse the numeric portion
                                        int.Parse(words[7], NmeaCultureInfo),
                                        // Use mm/s
                                        SpeedUnit.MillimetersPerSecond);
            }
            else
            {
                _wmVelocityX = Speed.Empty;
            }

            #endregion

            #region Water Mass Y Velocity

            // Do we have enough data to process the Water Mass Y Velocity?
            if (wordCount >= 9 && words[8].Length != 0)
            {
                _wmVelocityY = new Speed(
                                        // Parse the numeric portion
                                        int.Parse(words[8], NmeaCultureInfo),
                                        // Use mm/s
                                        SpeedUnit.MillimetersPerSecond);
            }
            else
            {
                _wmVelocityY = Speed.Empty;
            }

            #endregion

            #region Water Mass Z Velocity

            // Do we have enough data to process the Water Mass Z Velocity?
            if (wordCount >= 10 && words[9].Length != 0)
            {
                _wmVelocityZ = new Speed(
                                        // Parse the numeric portion
                                        int.Parse(words[9], NmeaCultureInfo),
                                        // Use mm/s
                                        SpeedUnit.MillimetersPerSecond);
            }
            else
            {
                _wmVelocityZ = Speed.Empty;
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
        /// Get the Bottom Track Velocity X as mm/s.
        /// </summary>
        public Speed BottomTrackVelX
        {
            get { return _btVelocityX; }
        }

        /// <summary>
        /// Get the Bottom Track Velocity Y as mm/s.
        /// </summary>
        public Speed BottomTrackVelY
        {
            get { return _btVelocityY; }
        }

        /// <summary>
        /// Get the Bottom Track Velocity Z as mm/s.
        /// </summary>
        public Speed BottomTrackVelZ
        {
            get { return _btVelocityZ; }
        }

        /// <summary>
        /// Get the Bottom Track Depth in mm.
        /// </summary>
        public Distance BottomTrackDepth
        {
            get { return _depth; }
        }

        /// <summary>
        /// Get the Water Mass Velocity X in mm/s.
        /// </summary>
        public Speed WaterMassVelX
        {
            get { return _wmVelocityX; }
        }

        /// <summary>
        /// Get the Water Mass Velocity Y in mm/s.
        /// </summary>
        public Speed WaterMassVelY
        {
            get { return _wmVelocityY; }
        }

        /// <summary>
        /// Get the Water Mass Velocity Z in mm/s.
        /// </summary>
        public Speed WaterMassVelZ
        {
            get { return _wmVelocityZ; }
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