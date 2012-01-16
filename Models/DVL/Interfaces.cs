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
 * 12/29/2011      RC          1.11       Initial coding. Moved from DotSpatial.
 * 
 */

using DotSpatial.Positioning;
namespace RTI
{
    /// <summary>
    /// Represents an NMEA sentence which contains
    /// </summary>
    /// <remarks></remarks>
    public interface IBtVelocityXSentence
    {
        /// <summary>
        /// Bottom Track Velocity X in mm/s.
        /// </summary>
        Speed BottomTrackVelX { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains
    /// </summary>
    /// <remarks></remarks>
    public interface IBtVelocityYSentence
    {
        /// <summary>
        /// Bottom Track Velocity Y in mm/s.
        /// </summary>
        Speed BottomTrackVelY { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains
    /// </summary>
    /// <remarks></remarks>
    public interface IBtVelocityZSentence
    {
        /// <summary>
        /// Bottom Track Velocity Z in mm/s.
        /// </summary>
        Speed BottomTrackVelZ { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains
    /// </summary>
    /// <remarks></remarks>
    public interface IBtVelocityEastSentence
    {
        /// <summary>
        /// Bottom Track Velocity East in mm/s.
        /// </summary>
        Speed BottomTrackVelEast { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains
    /// </summary>
    /// <remarks></remarks>
    public interface IBtVelocityNorthSentence
    {
        /// <summary>
        /// Bottom Track Velocity North in mm/s.
        /// </summary>
        Speed BottomTrackVelNorth { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains
    /// </summary>
    /// <remarks></remarks>
    public interface IBtVelocityUpSentence
    {
        /// <summary>
        /// Bottom Track Velocity Up in mm/s.
        /// </summary>
        Speed BottomTrackVelUp { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains
    /// </summary>
    /// <remarks></remarks>
    public interface IBtDepthSentence
    {
        /// <summary>
        /// Bottom Track Depth in mm.
        /// </summary>
        Distance BottomTrackDepth { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains
    /// </summary>
    /// <remarks></remarks>
    public interface IWmVelocityXSentence
    {
        /// <summary>
        /// Water Mass Velocity X in mm/s.
        /// </summary>
        Speed WaterMassVelX { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains
    /// </summary>
    /// <remarks></remarks>
    public interface IWmVelocityYSentence
    {
        /// <summary>
        /// Water Mass Velocity Y in mm/s.
        /// </summary>
        Speed WaterMassVelY { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains
    /// </summary>
    /// <remarks></remarks>
    public interface IWmVelocityZSentence
    {
        /// <summary>
        /// Water Mass Velocity Z in mm/s.
        /// </summary>
        Speed WaterMassVelZ { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains
    /// </summary>
    /// <remarks></remarks>
    public interface IWmVelocityEastSentence
    {
        /// <summary>
        /// Water Mass Velocity East in mm/s.
        /// </summary>
        Speed WaterMassVelEast { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains
    /// </summary>
    /// <remarks></remarks>
    public interface IWmVelocityNorthSentence
    {
        /// <summary>
        /// Water Mass Velocity North in mm/s.
        /// </summary>
        Speed WaterMassVelNorth { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains
    /// </summary>
    /// <remarks></remarks>
    public interface IWmVelocityUpSentence
    {
        /// <summary>
        /// Water Mass Velocity Up in mm/s.
        /// </summary>
        Speed WaterMassVelUp { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains
    /// </summary>
    /// <remarks></remarks>
    public interface IWmDepthSentence
    {
        /// <summary>
        /// Water Mass Depth  mm.
        /// </summary>
        Distance WaterMassDepth { get; }
    }

    /// <summary>
    /// Represents an NMEA sentence which contains
    /// </summary>
    /// <remarks></remarks>
    public interface IStatusSentence
    {
        /// <summary>
        /// Water Mass Depth  mm.
        /// </summary>
        RTI.Status SystemStatus { get; }
    }
}