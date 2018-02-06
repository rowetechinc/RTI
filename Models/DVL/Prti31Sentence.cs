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
 * 02/07/2014      RC          2.21.3     Initial coding.
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
    /// DVL Mode.  Data consist of Heading, Pitch and Roll of
    /// the Water Mass ping.
    /// </summary>
    public sealed class Prti31Sentence : NmeaSentence, IHeadingSentence, IPitchSentence, IRollSentence, ISubsystemConfigurationSentence
    {
        /// <summary>
        /// Command word for PRTI31.
        /// </summary>
        public const string CMD_WORD_PRTI31 = "PRTI31";

        #region Properties

        /// <summary>
        /// Heading in degrees.
        /// </summary>
        private float _Heading;
        /// <summary>
        /// Heading in degrees.
        /// </summary>
        public float Heading
        {
            get { return _Heading; }
        }

        /// <summary>
        /// Pitch in degrees.
        /// </summary>
        private float _Pitch;
        /// <summary>
        /// Pitch in degrees.
        /// </summary>
        public float Pitch
        {
            get { return _Pitch; }
        }

        /// <summary>
        /// Roll in degrees.
        /// </summary>
        private float _Roll;
        /// <summary>
        /// Roll in degrees.
        /// </summary>
        public float Roll
        {
            get { return _Roll; }
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
        /// Creates a new Prti30Sentence
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <remarks></remarks>
        public Prti31Sentence(string sentence)
            : base(sentence)
        {
            // If the sentence is valid, parse the data
            if (IsValid)
            {
                OnSentenceChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Prti30Sentence"/> class.
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <param name="commandWord">The command word.</param>
        /// <param name="words">The words.</param>
        /// <param name="validChecksum">The valid checksum.</param>
        /// <remarks></remarks>
        internal Prti31Sentence(string sentence, string commandWord, string[] words, string validChecksum)
            : base(sentence, commandWord, words, validChecksum)
        { }

        /// <summary>
        /// Constructor.  Set all the values and add to a string.
        /// </summary>
        /// <param name="heading">Heading in degrees.</param>
        /// <param name="pitch">Pitch in degrees.</param>
        /// <param name="roll">Roll in degrees.</param>
        /// <param name="subsystem">Subsystem.</param>
        /// <param name="cepoIndex">CEPO Index.</param>
        public Prti31Sentence(string heading, string pitch, string roll, string subsystem, string cepoIndex)
        {
            // Use a string builder to create the sentence text
            StringBuilder builder = new StringBuilder(128);

            #region Append the command word

            // Append the command word
            builder.Append("$");
            builder.Append(CMD_WORD_PRTI31);

            #endregion Append the command word

            // Append a comma
            builder.Append(',');

            #region Append Heading

            builder.Append(heading);

            #endregion

            // Append a comma
            builder.Append(',');

            #region Append Pitch

            builder.Append(pitch);

            #endregion

            // Append a comma
            builder.Append(',');

            #region Append Roll

            builder.Append(roll);

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

            #region Heading

            // Do we have enough data to process the Heading?
            if (wordCount >= 1 && words[0].Length != 0)
            {
                _Heading = float.Parse(words[0], NmeaCultureInfo);
            }
            else
            {
                _Heading = 0.0f;
            }

            #endregion

            #region Pitch

            // Do we have enough data to process the Pitch?
            if (wordCount >= 2 && words[1].Length != 0)
            {
                _Pitch =  float.Parse(words[1], NmeaCultureInfo);
            }
            else
            {
                _Pitch = 0.0f;
            }

            #endregion

            #region Roll

            // Do we have enough data to process the Roll?
            if (wordCount >= 3 && words[2].Length != 0)
            {
                _Roll = float.Parse(words[2], NmeaCultureInfo);
            }
            else
            {
                _Roll = 0.0f;
            }

            #endregion

            #region Subsystem Configuration

            // Do we have enough data to process the Subsystem Configuration?
            if (wordCount >= 5 && words[3].Length != 0 && words[4].Length != 0)
            {
                byte ssConfig = 0;
                byte.TryParse(words[4], out ssConfig);         // Subsystem configuration index in CEPO

                _SubsystemConfig = new SubsystemConfiguration(new Subsystem(words[3], 0), ssConfig, ssConfig);
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
            builder.Append(CMD_WORD_PRTI31);

            #endregion Append the command word

            // Append a comma
            builder.Append(',');

            #region Append Heading

            builder.Append(_Heading);

            #endregion

            // Append a comma
            builder.Append(',');

            #region Append Pitch

            builder.Append(_Pitch);

            #endregion

            // Append a comma
            builder.Append(',');

            #region Append Roll

            builder.Append(_Roll);

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