// ********************************************************************************************************
// Product Name: DotSpatial.Positioning.dll
// Description:  A library for managing GPS connections.
// ********************************************************************************************************
// The contents of this file are subject to the MIT License (MIT)
// you may not use this file except in compliance with the License. You may obtain a copy of the License at
// http://dotspatial.codeplex.com/license
//
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF
// ANY KIND, either expressed or implied. See the License for the specific language governing rights and
// limitations under the License.
//
// The Original Code is from http://gps3.codeplex.com/ version 3.0
//
// The Initial Developer of this original code is Jon Pearson. Submitted Oct. 21, 2010 by Ben Tombs (tidyup)
//
// Contributor(s): (Open source contributors should list themselves and their modifications here).
// -------------------------------------------------------------------------------------------------------
// |    Developer             |    Date    |                             Comments
// |--------------------------|------------|--------------------------------------------------------------
// | Rico  (Rico Castelo)      | 02/11/2014 | Initial coding.
// ********************************************************************************************************
using System;
using System.Text;

namespace DotSpatial.Positioning
{
    /// <summary>
    /// $GPHDT
    /// Heading, True. 
    /// http://aprs.gids.nl/nmea/#gll
    /// eg1. $GPHDT,123.456,T*00
    /// 
    /// $--HDT,X.X,T*CC [CR] [LF]
    /// X.X = Current heading, in degrees
    /// T = Indicates true heading
    /// </summary>
    /// <remarks></remarks>
    public sealed class GphdtSentence : NmeaSentence, IHeadingAzSentence
    {
        /// <summary>
        ///
        /// </summary>
        private Azimuth _heading;


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GphdtSentence"/> class.
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <param name="commandWord">The command word.</param>
        /// <param name="words">The words.</param>
        /// <param name="validChecksum">The valid checksum.</param>
        /// <remarks></remarks>
        internal GphdtSentence(string sentence, string commandWord, string[] words, string validChecksum)
            : base(sentence, commandWord, words, validChecksum)
        { }

        /// <summary>
        /// Creates a GphdtSentence from the specified string
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <remarks></remarks>
        public GphdtSentence(string sentence)
            : base(sentence)
        {
            // If the sentence is valid, parse the data
            if (IsValid)
            {
                OnSentenceChanged();
            }
        }

        /// <summary>
        /// Creates a GphdtSentence from the specified parameters
        /// </summary>
        /// <param name="heading">The heading.</param>
        /// <remarks></remarks>
        public GphdtSentence(Azimuth heading)
        {
            // Build a sentence
            StringBuilder builder = new StringBuilder(128);

            #region Append the command word

            // Append the command word
            builder.Append("$GPHDT");

            #endregion Append the command word

            // Append a comma
            builder.Append(',');

            #region Append the Heading

            builder.Append(heading.DecimalDegrees);

            #endregion Append the Heading

            // Append a comma
            builder.Append(',');

            #region Append the TRUE

            builder.Append("T");

            #endregion Append the TRUE

            // Set this object's sentence
            SetSentence(builder.ToString());

            // Finally, append the checksum
            AppendChecksum();
        }

        #endregion Constructors

        #region Overrides

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

            // Do we have enough words to make a heading?
            if (wordCount >= 1 && words[0].Length != 0)
            {
                #region Heading

                double heading = 0.0;
                double.TryParse(words[0], out heading);
                _heading = new Azimuth(heading);

                #endregion
            }
        }

        #endregion Overrides

        #region IHeadingAzSentence Members

        /// <summary>
        /// Gets the time in UTC from the IBearingSentence
        /// </summary>
        /// <remarks></remarks>
        public Azimuth Heading
        {
            get { return _heading; }
        }

        #endregion IHeadingAzSentence Members

    }
}