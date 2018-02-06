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
 * 11/29/2011      RC                     Initial coding.
 * 01/19/2012      RC          1.14       Added Encode().
 * 02/21/2013      RC          2.18       Added comments.
 * 01/31/2014      RC          2.21.3     Fixed parsing the status value from hex to int in OnSentenceChanged().
 * 02/06/2014      RC          2.21.3     Added Subsystem Configuration.
 * 02/18/2014      RC          2.21.3     Set the SubsystemConfig number as the same as the CEPO index in OnSentenceChanged(). 
 * 02/21/2014      RC          2.21.3     Fixed constructor to take subsystem CEPO index.
 * 
 */

using DotSpatial.Positioning;
using System.Text;
using System;
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
                                                        IWmDepthSentence, IStatusSentence, ISubsystemConfigurationSentence
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
        /// Start time of this sample in hundreds of seconds since
        /// power up or user reset.
        /// </summary>
        public int StartTime
        {
            get { return _startTime; }
        }

        /// <summary>
        /// Sample Number.
        /// </summary>
        private int _sampleNumber;
        /// <summary>
        /// Get the Sample number.
        /// </summary>
        public int SampleNumber
        {
            get { return _sampleNumber; }
        }

        /// <summary>
        /// Temperature in hundreds of degrees Celsius.
        /// (Measured from the transducer)
        /// </summary>
        private int _temperature;
        /// <summary>
        /// Temperature in hundreds of degrees Celsius.
        /// (Measured from the transducer)
        /// </summary>
        public int Temperature
        {
            get { return _temperature; }
        }

        /// <summary>
        /// Bottom Track East velocity component mm/s.
        /// (Earth Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        private Speed _btVelocityEast;
        /// <summary>
        /// Bottom Track East velocity component mm/s.
        /// (Earth Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        public Speed BottomTrackVelEast
        {
            get { return _btVelocityEast; }
        }

        /// <summary>
        /// Bottom Track North velocity component mm/s.
        /// (Earth Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        private Speed _btVelocityNorth;
        /// <summary>
        /// Bottom Track North velocity component mm/s.
        /// (Earth Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        public Speed BottomTrackVelNorth
        {
            get { return _btVelocityNorth; }
        }

        /// <summary>
        /// Bottom Track Up velocity component mm/s.
        /// (Earth Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        private Speed _btVelocityUp;
        /// <summary>
        /// Bottom Track Up velocity component mm/s.
        /// (Earth Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        public Speed BottomTrackVelUp
        {
            get { return _btVelocityUp; }
        }

        /// <summary>
        /// Depth below transducer in mm.  
        /// (Range to the bottom in front of the transducer, 
        /// 0 = no detection)
        /// </summary>
        private Distance _depth;
        /// <summary>
        /// Depth below transducer in mm.  
        /// (Range to the bottom in front of the transducer, 
        /// 0 = no detection)
        /// </summary>
        public Distance BottomTrackDepth
        {
            get { return _depth; }
        }

        /// <summary>
        /// Water Mass East velocity component mm/s.
        /// (Earth Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        private Speed _wmVelocityEast;
        /// <summary>
        /// Water Mass East velocity component mm/s.
        /// (Earth Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        public Speed WaterMassVelEast
        {
            get { return _wmVelocityEast; }
        }

        /// <summary>
        /// Water Mass North velocity component mm/s.
        /// (Earth Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        private Speed _wmVelocityNorth;
        /// <summary>
        /// Water Mass North velocity component mm/s.
        /// (Earth Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        public Speed WaterMassVelNorth
        {
            get { return _wmVelocityNorth; }
        }

        /// <summary>
        /// Water Mass Up velocity component mm/s.
        /// (Earth Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        private Speed _wmVelocityUp;
        /// <summary>
        /// Water Mass Up velocity component mm/s.
        /// (Earth Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        public Speed WaterMassVelUp
        {
            get { return _wmVelocityUp; }
        }

        /// <summary>
        /// Depth of water mass measurement in mm.
        /// (Position of the bin in front of the transducer)
        /// </summary>
        private Distance _wmDepth;
        /// <summary>
        /// Depth of water mass measurement in mm.
        /// (Position of the bin in front of the transducer)
        /// </summary>
        public Distance WaterMassDepth
        {
            get { return _wmDepth; }
        }

        /// <summary>
        /// Built in test and status bits in hexadecimal.
        /// (0000 = OK)
        /// </summary>
        private Status _status;
        /// <summary>
        /// Built in test and status bits in hexadecimal.
        /// (0000 = OK)
        /// </summary>
        public Status SystemStatus
        {
            get { return _status; }
        }

        /// <summary>
        /// Subsystem configuration the data belong to.
        /// </summary>
        private SubsystemConfiguration _SubsystemConfig;
        /// <summary>
        /// Subsystem configuration the data belong to.
        /// </summary>
        public SubsystemConfiguration SubsystemConfig
        {
            get { return _SubsystemConfig; }
        }

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
        /// <param name="subsystem">Subsystem.</param>
        /// <param name="cepoIndex">CEPO Index.</param>
        public Prti02Sentence(string time, string sampleNum, string temp,
                                string btVelEast, string btVelNorth, string btVelUp,
                                string depth,
                                string wmVelEast, string wmVelNorth, string wmVelUp,
                                string wmDepth, string status, string subsystem, string cepoIndex)
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

            // Append a comma
            builder.Append(',');

            #region Append Subsystem

            builder.Append(subsystem);

            #endregion

            // Append a comma
            builder.Append(',');

            #region Append CEPO Index

            builder.Append(cepoIndex);

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
                // Convert the hex string to an int
                int status = Convert.ToInt32(words[11], 16);

                _status = new Status(status);
            }
            else
            {
                // Status not found
                _status = new Status(0);
            }

            #endregion

            #region Subsystem Configuration

            // Do we have enough data to process the Subsystem Configuration?
            if (wordCount >= 14 && words[12].Length != 0 && words[13].Length != 0)
            {
                byte ssConfig = 0;
                byte.TryParse(words[13], out ssConfig);         // Subsystem configuration index in CEPO

                _SubsystemConfig = new SubsystemConfiguration(new Subsystem(words[12], 0), ssConfig, ssConfig);
            }
            else
            {
                // Subsystem configuration not found
                _SubsystemConfig = new SubsystemConfiguration();
            }

            #endregion

        }

        /// <summary>
        /// Return a NMEA sentence with all the values set.
        /// The format for the NMEA sentence is given in the
        /// RTI ADCP User Guide.
        /// </summary>
        /// <returns>NMEA sentence of this object.</returns>
        public string Encode()
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

            builder.Append(_startTime);

            #endregion

            // Append a comma
            builder.Append(',');

            #region Append Sample Number

            builder.Append(_sampleNumber);

            #endregion

            // Append a comma
            builder.Append(',');

            #region Append Temperature

            builder.Append(_temperature);

            #endregion

            // Append a comma
            builder.Append(',');

            #region Append Bottom Track Velocity

            builder.Append(_btVelocityEast.ToMillimetersPerSecond().Value);
            builder.Append(',');
            builder.Append(_btVelocityNorth.ToMillimetersPerSecond().Value);
            builder.Append(',');
            builder.Append(_btVelocityUp.ToMillimetersPerSecond().Value);

            #endregion

            // Append a comma
            builder.Append(',');

            #region Append Depth below transducer

            builder.Append(_depth.ToMillimeters().Value);

            #endregion

            // Append a comma
            builder.Append(',');

            #region Append Water Mass Velocity

            builder.Append(_wmVelocityEast.ToMillimetersPerSecond().Value);
            builder.Append(',');
            builder.Append(_wmVelocityNorth.ToMillimetersPerSecond().Value);
            builder.Append(',');
            builder.Append(_wmVelocityUp.ToMillimetersPerSecond().Value);

            #endregion

            // Append a comma
            builder.Append(',');

            #region Append Depth of water mass measurement

            builder.Append(_wmDepth.ToMillimeters().Value);

            #endregion

            // Append a comma
            builder.Append(',');

            #region Append Status

            builder.Append(_status.Value);

            #endregion

            // Append a comma
            builder.Append(',');

            #region Append Subsystem Configuration

            builder.Append(_SubsystemConfig.SubSystem.Code.ToString());
            builder.Append(',');
            builder.Append(_SubsystemConfig.CepoIndex.ToString());

            #endregion

            // Set this object's sentence
            // This will also set the checksum
            SetSentence(builder.ToString());

            // Finally, append the checksum
            AppendChecksum();

            return Sentence;
        }
    }

}