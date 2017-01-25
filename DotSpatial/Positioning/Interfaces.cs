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
// | Rico (Rico Castelo)      | 11/22/2011 | Added additional interfaces for Prti01Sentence and Prti02Sentence.
// ********************************************************************************************************
using System;
using System.Collections.Generic;

namespace DotSpatial.Positioning
{
    /// <summary>
    /// Represents an NMEA sentence which contains latitude and longitude values.
    /// </summary>
    /// <remarks></remarks>
    public interface IPositionSentence
    {
        /// <summary>
        /// Represents an NMEA sentence which contains a position.
        /// </summary>
        /// <remarks></remarks>
        Position Position { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains date and time in UTC.
    /// </summary>
    /// <remarks></remarks>
    public interface IUtcDateTimeSentence
    {
        /// <summary>
        /// Represents an NMEA sentence which contains date and time in UTC.
        /// </summary>
        /// <remarks></remarks>
        DateTime UtcDateTime { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains time in UTC.
    /// </summary>
    /// <remarks></remarks>
    public interface IUtcTimeSentence
    {
        /// <summary>
        /// Gets the time in UTC from the IUtcTimeSentence
        /// </summary>
        /// <remarks></remarks>
        TimeSpan UtcTime { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains differential GPS information.
    /// </summary>
    /// <remarks></remarks>
    public interface IDifferentialGpsSentence
    {
        /// <summary>
        /// Gets the Differential Gps Station ID
        /// </summary>
        /// <remarks></remarks>
        int DifferentialGpsStationID { get; }

        /// <summary>
        /// Gets the age of the Differential Gps
        /// </summary>
        /// <remarks></remarks>
        TimeSpan DifferentialGpsAge { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which describes the current fix.
    /// </summary>
    /// <remarks></remarks>
    public interface IFixModeSentence
    {
        /// <summary>
        /// Gets the fix mode
        /// </summary>
        /// <remarks></remarks>
        FixMode FixMode { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains the method used to acquire a fix.
    /// </summary>
    /// <remarks></remarks>
    public interface IFixMethodSentence
    {
        /// <summary>
        /// The Fix Method
        /// </summary>
        /// <remarks></remarks>
        FixMethod FixMethod { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains GPS satellite information.
    /// </summary>
    /// <remarks></remarks>
    public interface ISatelliteCollectionSentence
    {
        /// <summary>
        /// The Satellites
        /// </summary>
        /// <remarks></remarks>
        IList<Satellite> Satellites { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which describes how the fix is being obtained.
    /// </summary>
    /// <remarks></remarks>
    public interface IFixQualitySentence
    {
        /// <summary>
        /// The Fix Quality
        /// </summary>
        /// <remarks></remarks>
        FixQuality FixQuality { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which returns the number of GPS satellites involved in the current fix.
    /// </summary>
    /// <remarks></remarks>
    public interface IFixedSatelliteCountSentence
    {
        /// <summary>
        /// The Fixed Satellite Count
        /// </summary>
        /// <remarks></remarks>
        int FixedSatelliteCount { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains a list of fixed GPS satellites.
    /// </summary>
    /// <remarks></remarks>
    public interface IFixedSatellitesSentence
    {
        /// <summary>
        /// the list of FixedSatellites
        /// </summary>
        /// <remarks></remarks>
        IList<Satellite> FixedSatellites { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains the direction of travel.
    /// </summary>
    /// <remarks></remarks>
    public interface IBearingSentence
    {
        /// <summary>
        /// the Bearing
        /// </summary>
        /// <remarks></remarks>
        Azimuth Bearing { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains the direction of travel.
    /// </summary>
    /// <remarks></remarks>
    public interface IHeadingAzSentence
    {
        /// <summary>
        /// the Heading
        /// </summary>
        /// <remarks></remarks>
        Azimuth Heading { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains whether a fix is currently acquired.
    /// </summary>
    /// <remarks></remarks>
    public interface IFixStatusSentence
    {
        /// <summary>
        /// The Fix Status
        /// </summary>
        /// <remarks></remarks>
        FixStatus FixStatus { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains
    /// </summary>
    /// <remarks></remarks>
    public interface ISpeedSentence
    {
        /// <summary>
        /// The Speed
        /// </summary>
        /// <remarks></remarks>
        Speed Speed { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains
    /// </summary>
    /// <remarks></remarks>
    public interface IMagneticVariationSentence
    {
        /// <summary>
        /// The Magnetic Variation
        /// </summary>
        /// <remarks></remarks>
        Longitude MagneticVariation { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains
    /// </summary>
    /// <remarks></remarks>
    public interface IAltitudeSentence
    {
        /// <summary>
        /// The Altitude
        /// </summary>
        /// <remarks></remarks>
        Distance Altitude { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains
    /// </summary>
    /// <remarks></remarks>
    public interface IAltitudeAboveEllipsoidSentence
    {
        /// <summary>
        /// The Altitude Above Ellipsoid
        /// </summary>
        /// <remarks></remarks>
        Distance AltitudeAboveEllipsoid { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains
    /// </summary>
    /// <remarks></remarks>
    public interface IGeoidalSeparationSentence
    {
        /// <summary>
        /// The Geoidal Separation
        /// </summary>
        /// <remarks></remarks>
        Distance GeoidalSeparation { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains
    /// </summary>
    /// <remarks></remarks>
    public interface IHorizontalDilutionOfPrecisionSentence
    {
        /// <summary>
        /// The Horizontal Dilution of Precision
        /// </summary>
        /// <remarks></remarks>
        DilutionOfPrecision HorizontalDilutionOfPrecision { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains
    /// </summary>
    /// <remarks></remarks>
    public interface IVerticalDilutionOfPrecisionSentence
    {
        /// <summary>
        /// The Vertical Dilution of Precision
        /// </summary>
        /// <remarks></remarks>
        DilutionOfPrecision VerticalDilutionOfPrecision { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains
    /// </summary>
    /// <remarks></remarks>
    public interface IPositionDilutionOfPrecisionSentence
    {
        /// <summary>
        /// The Position Dilution of Precision (PDOP)
        /// </summary>
        /// <remarks></remarks>
        DilutionOfPrecision PositionDilutionOfPrecision { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains
    /// </summary>
    /// <remarks></remarks>
    public interface IStartTimeSentence
    {
        /// <summary>
        /// Start time.
        /// </summary>
        int StartTime { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains
    /// </summary>
    /// <remarks></remarks>
    public interface ISampleNumberSentence
    {
        /// <summary>
        /// Sample Number.
        /// </summary>
        int SampleNumber { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains
    /// </summary>
    /// <remarks></remarks>
    public interface ITemperatureSentence
    {
        /// <summary>
        /// Temperature in Celsius.
        /// </summary>
        int Temperature { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains Heading.
    /// </summary>
    /// <remarks></remarks>
    public interface IHeadingSentence
    {
        /// <summary>
        /// Heading in degrees.
        /// </summary>
        float Heading { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains Pitch.
    /// </summary>
    /// <remarks></remarks>
    public interface IPitchSentence
    {
        /// <summary>
        /// Pitch in degrees.
        /// </summary>
        float Pitch { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains Roll.
    /// </summary>
    /// <remarks></remarks>
    public interface IRollSentence
    {
        /// <summary>
        /// Roll in degrees.
        /// </summary>
        float Roll { get; }
    }
}