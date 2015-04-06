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
// | Rico  (Rico Castelo)     | 01/31/2014 | Put all parsing in a try block or used TryParse.
// ********************************************************************************************************
using System;
using System.Text;

namespace DotSpatial.Positioning
{
    /// <summary>
    /// $GPGLL
    /// Geographic Position, Latitude / Longitude and time.
    /// http://aprs.gids.nl/nmea/#gll
    /// eg1. $GPGLL, 3751.65, S, 14507.36, E*77
    /// eg2. $GPGLL, 4916.45, N, 12311.12, W, 225444, A
    /// 4916.46, N    Latitude 49 deg. 16.45 min. North
    /// 12311.12, W   Longitude 123 deg. 11.12 min. West
    /// 225444       Fix taken at 22:54:44 UTC
    /// A            Data valid
    /// eg3. $GPGLL, 5133.81, N, 00042.25, W*75
    /// 1    2     3    4 5
    /// 1    5133.81   Current latitude
    /// 2    N         North/South
    /// 3    00042.25  Current longitude
    /// 4    W         East/West
    /// 5    *75       checksum
    /// $--GLL, lll.ll, a, yyyyy.yy, a, hhmmss.ss, A
    /// llll.ll = Latitude of location
    /// a = N or S
    /// yyyyy.yy = Longitude of location
    /// a = E or W
    /// hhmmss.ss = UTC of location
    /// A = status: A = valid data
    /// </summary>
    /// <remarks></remarks>
    public sealed class GpgllSentence : NmeaSentence, IUtcTimeSentence, IPositionSentence, IFixStatusSentence
    {
        /// <summary>
        ///
        /// </summary>
        private TimeSpan _utcTime;
        /// <summary>
        ///
        /// </summary>
        private Position _position;
        /// <summary>
        ///
        /// </summary>
        private FixStatus _fixStatus;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GpgllSentence"/> class.
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <param name="commandWord">The command word.</param>
        /// <param name="words">The words.</param>
        /// <param name="validChecksum">The valid checksum.</param>
        /// <remarks></remarks>
        internal GpgllSentence(string sentence, string commandWord, string[] words, string validChecksum)
            : base(sentence, commandWord, words, validChecksum)
        { }

        /// <summary>
        /// Creates a GpgllSentence from the specified string
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <remarks></remarks>
        public GpgllSentence(string sentence)
            : base(sentence)
        {
            // If the sentence is valid, parse the data
            if (IsValid)
            {
                OnSentenceChanged();
            }
        }

        /// <summary>
        /// Creates a GpgllSentence from the specified parameters
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="utcTime">The UTC time.</param>
        /// <param name="fixStatus">The fix status.</param>
        /// <remarks></remarks>
        public GpgllSentence(Position position, TimeSpan utcTime, FixStatus fixStatus)
        {
            // Build a sentence
            StringBuilder builder = new StringBuilder(128);

            #region Append the command word

            // Append the command word
            builder.Append("$GPGLL");

            #endregion Append the command word

            // Append a comma
            builder.Append(',');

            #region Append the position

            // Append latitude in the format HHMM.MMMM.
            builder.Append(position.Latitude.ToString("HHMM.MMMM, I,", NmeaCultureInfo));
            // Append Longitude in the format HHHMM.MMMM.
            builder.Append(position.Longitude.ToString("HHHMM.MMMM, I,", NmeaCultureInfo));

            #endregion Append the position

            #region Append the UTC time

            builder.Append(utcTime.Hours.ToString("0#", NmeaCultureInfo));
            builder.Append(utcTime.Minutes.ToString("0#", NmeaCultureInfo));
            builder.Append(utcTime.Seconds.ToString("0#", NmeaCultureInfo));
            builder.Append(".");
            builder.Append(utcTime.Milliseconds.ToString("00#", NmeaCultureInfo));

            #endregion Append the UTC time

            // Append a comma
            builder.Append(",");

            #region Append the fix status

            switch (fixStatus)
            {
                case FixStatus.Fix:
                    builder.Append("A");
                    break;
                default:
                    builder.Append("V");  // V = INvalid
                    break;
            }

            #endregion Append the fix status

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

            // Do we have enough words to make a lat/long location?
            if (wordCount >= 4 && words[0].Length != 0 && words[1].Length != 0 && words[2].Length != 0 && words[3].Length != 0)
            {
                #region Latitude

                string latitudeWord = words[0];
                int latitudeHours = 0;
                double latitudeDecimalMinutes = 0.0;

                int.TryParse(latitudeWord.Substring(0, 2), out latitudeHours);
                double.TryParse(latitudeWord.Substring(2), out latitudeDecimalMinutes);

                LatitudeHemisphere latitudeHemisphere =
                    words[1].Equals("N", StringComparison.OrdinalIgnoreCase) ? LatitudeHemisphere.North : LatitudeHemisphere.South;

                #endregion Latitude

                #region Longitude

                string longitudeWord = words[2];
                int longitudeHours = 0;
                double longitudeDecimalMinutes = 0.0;

                int.TryParse(longitudeWord.Substring(0, 3), out longitudeHours);
                double.TryParse(longitudeWord.Substring(3), out longitudeDecimalMinutes);

                LongitudeHemisphere longitudeHemisphere =
                    words[3].Equals("E", StringComparison.OrdinalIgnoreCase) ? LongitudeHemisphere.East : LongitudeHemisphere.West;

                #endregion Longitude

                #region Position

                _position = new Position(
                                new Latitude(latitudeHours, latitudeDecimalMinutes, latitudeHemisphere),
                                new Longitude(longitudeHours, longitudeDecimalMinutes, longitudeHemisphere));

                #endregion Position
            }

            // Do we have enough data to process the UTC time?
            if (wordCount >= 5 && words[4].Length != 0)
            {
                #region UTC Time

                string utcTimeWord = words[4];
                int utcHours = 0;
                int utcMinutes = 0;
                int utcSeconds = 0;
                int utcMilliseconds = 0;

                int.TryParse(utcTimeWord.Substring(0, 2), out utcHours);
                int.TryParse(utcTimeWord.Substring(2, 2), out utcMinutes);
                int.TryParse(utcTimeWord.Substring(4, 2), out utcSeconds);

                if (utcTimeWord.Length > 6)
                {
                    try
                    {
                        utcMilliseconds = Convert.ToInt32(float.Parse(utcTimeWord.Substring(6), NmeaCultureInfo) * 1000, NmeaCultureInfo);
                    }
                    catch (Exception) { }
                }

                // Build a TimeSpan for this value
                _utcTime = new TimeSpan(0, utcHours, utcMinutes, utcSeconds, utcMilliseconds);

                #endregion UTC Time
            }
            else
            {
                // The UTC time is invalid
                _utcTime = TimeSpan.MinValue;
            }

            // Do we have enough data to get the fix status?
            if (wordCount >= 6 && words[5].Length != 0)
            {
                #region Fix Status

                // An "A" means a valid fix
                _fixStatus = words[5].Equals("A", StringComparison.OrdinalIgnoreCase) ? FixStatus.Fix : FixStatus.NoFix;

                #endregion Fix Status
            }
            else
            {
                // The fix status is invalid
                _fixStatus = FixStatus.Unknown;
            }
        }

        #endregion Overrides

        #region IUtcDateTimeSentence Members

        /// <summary>
        /// Gets the time in UTC from the IUtcTimeSentence
        /// </summary>
        /// <remarks></remarks>
        public TimeSpan UtcTime
        {
            get { return _utcTime; }
        }

        #endregion IUtcDateTimeSentence Members

        #region IPositionSentence Members

        /// <summary>
        /// Represents an NMEA sentence which contains a position.
        /// </summary>
        /// <remarks></remarks>
        public Position Position
        {
            get { return _position; }
        }

        #endregion IPositionSentence Members

        #region IFixStatusSentence Members

        /// <summary>
        /// The Fix Status
        /// </summary>
        /// <remarks></remarks>
        public FixStatus FixStatus
        {
            get { return _fixStatus; }
        }

        #endregion IFixStatusSentence Members
    }
}