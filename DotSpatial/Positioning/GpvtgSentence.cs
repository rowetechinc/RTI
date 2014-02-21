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
// | Tidyup  (Ben Tombs)      | 10/21/2010 | Original copy submitted from modified GPS.Net 3.0
// | Shade1974 (Ted Dunsford) | 10/22/2010 | Added file headers reviewed formatting with resharper.
// | Rico  (Rico Castelo)     | 12/08/2011 | Added OnSentenceChanged() to constructor that takes a NmeaSentence.
// | Rico  (Rico Castelo)     | 12/09/2011 | Fixed bug where wrong indexes were used for speed values.
// | Rico  (Rico Castelo)     | 01/31/2014 | Put all parsing in a try block or used TryParse.
// ********************************************************************************************************
using System;
namespace DotSpatial.Positioning
{
    /// <summary>
    /// Track made good and ground speed sentence
    /// </summary>
    /// <remarks></remarks>
    public sealed class GpvtgSentence : NmeaSentence, IBearingSentence, ISpeedSentence, IMagneticVariationSentence
    {
        /// <summary>
        ///
        /// </summary>
        private Azimuth _bearing;
        /// <summary>
        ///
        /// </summary>
        private Longitude _magneticVariation;
        /// <summary>
        ///
        /// </summary>
        private Speed _speed;

        #region Constructors

        /// <summary>
        /// Creates a track made good and ground speed sentence instance from the specified sentence
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <remarks></remarks>
        public GpvtgSentence(string sentence)
            : base(sentence)
        {
            // If the sentence is valid, parse the data
            if (IsValid)
            {
                OnSentenceChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpvtgSentence"/> class.
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <param name="commandWord">The command word.</param>
        /// <param name="words">The words.</param>
        /// <param name="validChecksum">The valid checksum.</param>
        /// <remarks></remarks>
        internal GpvtgSentence(string sentence, string commandWord, string[] words, string validChecksum)
            : base(sentence, commandWord, words, validChecksum)
        { }

        #endregion Constructors

        #region Overrides

        /// <summary>
        /// Overrides OnSentanceChanged for the GPVTGSentence
        /// </summary>
        /// <remarks></remarks>
        protected override void OnSentenceChanged()
        {
            // First, process the basic info for the sentence
            base.OnSentenceChanged();

            // Cache the sentence words
            string[] words = Words;
            int wordCount = words.Length;

            /*
             * $GPVTG

                Track Made Good and Ground Speed.

                eg1. $GPVTG, 360.0, T, 348.7, M, 000.0, N, 000.0, K*43
                eg2. $GPVTG, 054.7, T, 034.4, M, 005.5, N, 010.2, K

                           054.7, T      True track made good
                           034.4, M      Magnetic track made good
                           005.5, N      Ground speed, knots
                           010.2, K      Ground speed, Kilometers per hour

                eg3. $GPVTG, t, T, ,, s.ss, N, s.ss, K*hh
                1    = Track made good
                2    = Fixed text 'T' indicates that track made good is relative to true north
                3    = not used
                4    = not used
                5    = Speed over ground in knots
                6    = Fixed text 'N' indicates that speed over ground in in knots
                7    = Speed over ground in kilometers/hour
                8    = Fixed text 'K' indicates that speed over ground is in kilometers/hour

             *
             *
             */

            #region Bearing

            if (wordCount >= 1 && words[0].Length != 0)
            {
                try
                {
                    _bearing = Azimuth.Parse(words[0], NmeaCultureInfo);
                }
                catch (Exception)
                {
                    _bearing = Azimuth.Invalid;
                }
            }
            else
                _bearing = Azimuth.Invalid;

            #endregion Bearing

            #region Magnetic Variation

            if (wordCount >= 3 && words[2].Length != 0)
            {
                try
                {
                    _magneticVariation = new Longitude(double.Parse(words[2], NmeaCultureInfo) - _bearing.DecimalDegrees);
                }
                catch (Exception)
                {
                    _magneticVariation = Longitude.Invalid;
                }
            }
            else
                _magneticVariation = Longitude.Invalid;

            #endregion Magnetic Variation

            #region Speed

            /* Speed is reported as both knots and KM/H.  We can parse either of the values.
             * First, try to parse knots.  If that fails, parse KM/h.
             */

            if (wordCount > 5 && words[4].Length != 0)
            {
                try
                {
                    _speed = new Speed(
                        // Parse the numeric portion
                        double.Parse(words[4], NmeaCultureInfo),
                        // Use knots
                        SpeedUnit.Knots);
                }
                catch (Exception)
                {
                    _speed = Speed.Invalid;
                }
            }
            else if (wordCount > 7 && words[6].Length != 0)
            {
                try
                {
                    _speed = new Speed(
                        // Parse the numeric portion
                        double.Parse(words[6], NmeaCultureInfo),
                        // Use knots
                        SpeedUnit.KilometersPerHour);
                }
                catch (Exception)
                {
                    _speed = Speed.Invalid;
                }
            }
            else
            {
                // Invalid speed
                _speed = Speed.Invalid;
            }

            #endregion Speed
        }

        #endregion Overrides

        #region IBearingSentence Members

        /// <summary>
        /// the Bearing
        /// </summary>
        /// <remarks></remarks>
        public Azimuth Bearing
        {
            get { return _bearing; }
        }

        #endregion IBearingSentence Members

        #region ISpeedSentence Members

        /// <summary>
        /// The Speed
        /// </summary>
        /// <remarks></remarks>
        public Speed Speed
        {
            get { return _speed; }
        }

        #endregion ISpeedSentence Members

        #region IMagneticVariationSentence Members

        /// <summary>
        /// The Magnetic Variation
        /// </summary>
        /// <remarks></remarks>
        public Longitude MagneticVariation
        {
            get { return _magneticVariation; }
        }

        #endregion IMagneticVariationSentence Members
    }
}