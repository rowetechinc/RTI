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
 * 02/06/2014      RC          2.21.3     Initial coding.
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
    /// Data in Instrument transform.  This sentences differs from 
    /// PRTI01 because it includes the Q (error) velocity.
    /// </summary>
    public sealed class Prti03Sentence : NmeaSentence, IStartTimeSentence, ISampleNumberSentence, ITemperatureSentence, 
                                                        IBtVelocityXSentence, IBtVelocityYSentence, IBtVelocityZSentence, IBtVelocityQSentence,
                                                        IBtDepthSentence, 
                                                        IWmVelocityXSentence, IWmVelocityYSentence, IWmVelocityZSentence, IWmVelocityQSentence,
                                                        IWmDepthSentence, IStatusSentence, ISubsystemConfigurationSentence 
    {
        /// <summary>
        /// Command word for PRTI03.
        /// </summary>
        public const string CMD_WORD_PRTI03 = "PRTI03";

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
        /// Bottom Track X velocity component mm/s.
        /// (Instrument Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        private Speed _btVelocityX;
        /// <summary>
        /// Bottom Track X velocity component mm/s.
        /// (Instrument Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        public Speed BottomTrackVelX
        {
            get { return _btVelocityX; }
        }

        /// <summary>
        /// Bottom Track Y velocity component mm/s.
        /// (Instrument Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        private Speed _btVelocityY;
        /// <summary>
        /// Bottom Track Y velocity component mm/s.
        /// (Instrument Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        public Speed BottomTrackVelY
        {
            get { return _btVelocityY; }
        }

        /// <summary>
        /// Bottom Track Z velocity component mm/s.
        /// (Instrument Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        private Speed _btVelocityZ;
        /// <summary>
        /// Bottom Track Z velocity component mm/s.
        /// (Instrument Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        public Speed BottomTrackVelZ
        {
            get { return _btVelocityZ; }
        }

        /// <summary>
        /// Bottom Track Q velocity component mm/s.
        /// (Instrument Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        private Speed _btVelocityQ;
        /// <summary>
        /// Bottom Track Q velocity component mm/s.
        /// (Instrument Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        public Speed BottomTrackVelQ
        {
            get { return _btVelocityQ; }
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
        /// Water Mass X velocity component mm/s.
        /// (Instrument Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        private Speed _wmVelocityX;
        /// <summary>
        /// Water Mass X velocity component mm/s.
        /// (Instrument Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        public Speed WaterMassVelX
        {
            get { return _wmVelocityX; }
        }

        /// <summary>
        /// Water Mass Y velocity component mm/s.
        /// (Instrument Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        private Speed _wmVelocityY;
        /// <summary>
        /// Water Mass Y velocity component mm/s.
        /// (Instrument Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        public Speed WaterMassVelY
        {
            get { return _wmVelocityY; }
        }

        /// <summary>
        /// Water Mass Z velocity component mm/s.
        /// (Instrument Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        private Speed _wmVelocityZ;
        /// <summary>
        /// Water Mass Z velocity component mm/s.
        /// (Instrument Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        public Speed WaterMassVelZ
        {
            get { return _wmVelocityZ; }
        }

        /// <summary>
        /// Water Mass Q velocity component mm/s.
        /// (Instrument Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        private Speed _wmVelocityQ;
        /// <summary>
        /// Water Mass Q velocity component mm/s.
        /// (Instrument Coordinate system)
        /// (-99999 indicates no valid velocity)
        /// </summary>
        public Speed WaterMassVelQ
        {
            get { return _wmVelocityQ; }
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
        /// Creates a new Prti01Sentence
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <remarks></remarks>
        public Prti03Sentence(string sentence)
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
        internal Prti03Sentence(string sentence, string commandWord, string[] words, string validChecksum)
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
        /// <param name="btVelQ">Bottom Track Velocity Q.</param>
        /// <param name="depth">Depth.</param>
        /// <param name="wmVelX">Water Mass Velocity X.</param>
        /// <param name="wmVelY">Water Mass Velocity Y.</param>
        /// <param name="wmVelZ">Water Mass Velocity Z.</param>
        /// <param name="wmVelQ">Water Mass Velocity Q.</param>
        /// <param name="wmDepth">Water Mass Depth.</param>
        /// <param name="status">Status.</param>
        /// <param name="subsystem">Subsystem.</param>
        /// <param name="cepoIndex">CEPO Index.</param>
        public Prti03Sentence(string time, string sampleNum, string temp,
                                string btVelX, string btVelY, string btVelZ, string btVelQ,
                                string depth,
                                string wmVelX, string wmVelY, string wmVelZ, string wmVelQ,
                                string wmDepth, string status, string subsystem, string cepoIndex)
        {
            // Use a string builder to create the sentence text
            StringBuilder builder = new StringBuilder(128);

            #region Append the command word

            // Append the command word
            builder.Append("$");
            builder.Append(CMD_WORD_PRTI03);

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
            builder.Append(',');
            builder.Append(btVelQ);

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
            builder.Append(',');
            builder.Append(wmVelQ);

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

            #region Bottom Track Q Velocity

            // Do we have enough data to process the Bottom Track Z Velocity?
            if (wordCount >= 7 && words[6].Length != 0)
            {
                _btVelocityQ = new Speed(
                                        // Parse the numeric portion
                                        int.Parse(words[6], NmeaCultureInfo),
                                        // Use mm/s
                                        SpeedUnit.MillimetersPerSecond);
            }
            else
            {
                _btVelocityQ = Speed.Empty;
            }

            #endregion

            #region Depth

            // Do we have enough data to process the Depth?
            if (wordCount >= 8 && words[7].Length != 0)
            {
                _depth = new Distance(int.Parse(words[7], NmeaCultureInfo), DistanceUnit.Millimeters);
            }
            else
            {
                // Bottom not detected
                _depth = Distance.BottomNotDetectedDVL;
            }

            #endregion

            #region Water Mass X Velocity

            // Do we have enough data to process the Water Mass X Velocity?
            if (wordCount >= 9 && words[8].Length != 0)
            {
                _wmVelocityX = new Speed(
                                        // Parse the numeric portion
                                        int.Parse(words[8], NmeaCultureInfo),
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
            if (wordCount >= 10 && words[9].Length != 0)
            {
                _wmVelocityY = new Speed(
                                        // Parse the numeric portion
                                        int.Parse(words[9], NmeaCultureInfo),
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
            if (wordCount >= 11 && words[10].Length != 0)
            {
                _wmVelocityZ = new Speed(
                                        // Parse the numeric portion
                                        int.Parse(words[10], NmeaCultureInfo),
                                        // Use mm/s
                                        SpeedUnit.MillimetersPerSecond);
            }
            else
            {
                _wmVelocityZ = Speed.Empty;
            }

            #endregion

            #region Water Mass Q Velocity

            // Do we have enough data to process the Water Mass Q Velocity?
            if (wordCount >= 12 && words[11].Length != 0)
            {
                _wmVelocityZ = new Speed(
                                        // Parse the numeric portion
                                        int.Parse(words[11], NmeaCultureInfo),
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
            if (wordCount >= 13 && words[12].Length != 0)
            {
                _wmDepth = new Distance(int.Parse(words[12], NmeaCultureInfo), DistanceUnit.Millimeters);
            }
            else
            {
                // Water Mass depth not found
                _wmDepth = Distance.Empty;
            }

            #endregion

            #region Status

            // Do we have enough data to process the Status?
            if (wordCount >= 14 && words[13].Length != 0)
            {
                // Convert the hex string to an int
                int status = Convert.ToInt32(words[13], 16);

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
            if (wordCount >= 16 && words[14].Length != 0 && words[15].Length != 0)
            {
                byte ssConfig = 0;
                byte.TryParse(words[15], out ssConfig);         // Subsystem configuration index in CEPO

                _SubsystemConfig = new SubsystemConfiguration(new Subsystem(words[14], 0), ssConfig, ssConfig);
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
            builder.Append(CMD_WORD_PRTI03);

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

            builder.Append(_btVelocityX.ToMillimetersPerSecond().Value);
            builder.Append(',');
            builder.Append(_btVelocityY.ToMillimetersPerSecond().Value);
            builder.Append(',');
            builder.Append(_btVelocityZ.ToMillimetersPerSecond().Value);
            builder.Append(',');
            builder.Append(_btVelocityQ.ToMillimetersPerSecond().Value);

            #endregion

            // Append a comma
            builder.Append(',');

            #region Append Depth below transducer

            builder.Append(_depth.ToMillimeters().Value);

            #endregion

            // Append a comma
            builder.Append(',');

            #region Append Water Mass Velocity

            builder.Append(_wmVelocityX.ToMillimetersPerSecond().Value);
            builder.Append(',');
            builder.Append(_wmVelocityY.ToMillimetersPerSecond().Value);
            builder.Append(',');
            builder.Append(_wmVelocityZ.ToMillimetersPerSecond().Value);
            builder.Append(',');
            builder.Append(_wmVelocityQ.ToMillimetersPerSecond().Value);

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