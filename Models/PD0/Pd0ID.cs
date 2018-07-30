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
 * 03/03/2014      RC          2.21.4     Initial coding
 * 03/28/2014      RC          2.21.4     Added unknown data type.  Added GetType().  Added Default IDs.
 * 07/24/2018      RC          3.4.8      Added NmeaData data type.
 * 
 * 
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// PD0 ID.  This includes the type,
    /// MSB ID and LSB ID.
    /// </summary>
    public class Pd0ID
    {
        #region Class and Enum

        /// <summary>
        /// PD0 Data Type.
        /// </summary>
        public enum Pd0Types
        {
            /// <summary>
            /// Header.
            /// </summary>
            Header,

            /// <summary>
            /// Fixed leader.
            /// </summary>
            FixedLeader,

            /// <summary>
            /// Variable Leader.
            /// </summary>
            VariableLeader,

            /// <summary>
            /// Bottom Track.
            /// </summary>
            BottomTrack,

            /// <summary>
            /// Velocity.
            /// </summary>
            Velocity,

            /// <summary>
            /// Correlation Maginitude.
            /// </summary>
            Correlation,

            /// <summary>
            /// Echo Intensity.
            /// </summary>
            EchoIntensity,

            /// <summary>
            /// Percent Good.
            /// </summary>
            PercentGood,

            /// <summary>
            /// NMEA data.
            /// </summary>
            NmeaData,

            /// <summary>
            /// Unknown data type given.
            /// </summary>
            Unknown
        }

        #endregion

        #region ID

        /// <summary>
        /// LSB ID.
        /// </summary>
        public byte ID_LSB { get; set; }

        /// <summary>
        /// MSB ID.
        /// </summary>
        public byte ID_MSB { get; set; }

        /// <summary>
        /// PD0 ID Type.
        /// </summary>
        public Pd0Types Type { get; set; }

        #endregion

        #region Default IDs

        /// <summary>
        /// Bottom Track ID.
        /// </summary>
        public static Pd0ID BottomTrackID = new Pd0ID(Pd0BottomTrack.ID_LSB, Pd0BottomTrack.ID_MSB, Pd0Types.BottomTrack);

        /// <summary>
        /// Correlation ID.
        /// </summary>
        public static Pd0ID CorrelationID = new Pd0ID(Pd0Correlation.ID_LSB, Pd0Correlation.ID_MSB, Pd0Types.Correlation);

        /// <summary>
        /// Echo Intensity ID.
        /// </summary>
        public static Pd0ID EchoIntensityID = new Pd0ID(Pd0EchoIntensity.ID_LSB, Pd0EchoIntensity.ID_MSB, Pd0Types.EchoIntensity);

        /// <summary>
        /// Fixed Leader ID.
        /// </summary>
        public static Pd0ID FixLeaderID = new Pd0ID(Pd0FixedLeader.ID_LSB, Pd0FixedLeader.ID_MSB, Pd0Types.FixedLeader);

        /// <summary>
        /// Header ID.
        /// </summary>
        public static Pd0ID HeaderID = new Pd0ID(Pd0Header.ID_LSB, Pd0Header.ID_MSB, Pd0Types.Header);

        /// <summary>
        /// Percent Good ID.
        /// </summary>
        public static Pd0ID PercentGoodID = new Pd0ID(Pd0PercentGood.ID_LSB, Pd0PercentGood.ID_MSB, Pd0Types.PercentGood);

        /// <summary>
        /// Variable Leader ID.
        /// </summary>
        public static Pd0ID VariableLeaderID = new Pd0ID(Pd0VariableLeader.ID_LSB, Pd0VariableLeader.ID_MSB, Pd0Types.VariableLeader);

        /// <summary>
        /// Velocity ID.
        /// </summary>
        public static Pd0ID VelocityID = new Pd0ID(Pd0Velocity.ID_LSB, Pd0Velocity.ID_MSB, Pd0Types.Velocity);

        /// <summary>
        /// NMEA Data ID.
        /// </summary>
        public static Pd0ID NmeaDataID = new Pd0ID(Pd0NmeaData.ID_LSB, Pd0NmeaData.ID_MSB, Pd0Types.NmeaData);

        #endregion

        /// <summary>
        /// Initialize the ID.
        /// </summary>
        public Pd0ID()
        {

        }

        /// <summary>
        /// Initialize the ID.
        /// </summary>
        /// <param name="lsb">LSB ID.</param>
        /// <param name="msb">MSB ID.</param>
        /// <param name="type">PD0 Data Type.</param>
        public Pd0ID(byte lsb, byte msb, Pd0Types type)
        {
            ID_LSB = lsb;
            ID_MSB = msb;
            Type = type;
        }

        #region Find ID

        /// <summary>
        /// Get the ID based off the LSB and MSB byte given.
        /// This will check if the LSB and MSB match the 
        /// data type ID.
        /// If nothing is found, a blank data type is given.
        /// </summary>
        /// <param name="lsb">Data Type LSB ID.</param>
        /// <param name="msb">Data Type MSB ID.</param>
        /// <returns>The PD0 ID for the LSB and MSB given.</returns>
        public static Pd0ID GetType(byte lsb, byte msb)
        {
            // Bottom Track
            if (lsb == Pd0BottomTrack.ID_LSB && msb == Pd0BottomTrack.ID_MSB)
            {
                return BottomTrackID;
            }

            // Correlation
            if (lsb == Pd0Correlation.ID_LSB && msb == Pd0Correlation.ID_MSB)
            {
                return CorrelationID;
            }

            // Echo Intensity
            if (lsb == Pd0EchoIntensity.ID_LSB && msb == Pd0EchoIntensity.ID_MSB)
            {
                return EchoIntensityID;
            }

            // Fixed Leader
            if (lsb == Pd0FixedLeader.ID_LSB && msb == Pd0FixedLeader.ID_MSB)
            {
                return FixLeaderID;
            }

            // Header
            if (lsb == Pd0Header.ID_LSB && msb == Pd0Header.ID_MSB)
            {
                return HeaderID;
            }

            // Percent Good
            if (lsb == Pd0PercentGood.ID_LSB && msb == Pd0PercentGood.ID_MSB)
            {
                return PercentGoodID;
            }

            // Variable Leader
            if (lsb == Pd0VariableLeader.ID_LSB && msb == Pd0VariableLeader.ID_MSB)
            {
                return VariableLeaderID;
            }

            // Velocity
            if (lsb == Pd0Velocity.ID_LSB && msb == Pd0Velocity.ID_MSB)
            {
                return VelocityID;
            }

            // NMEA Data 
            if (lsb == Pd0NmeaData.ID_LSB && msb == Pd0NmeaData.ID_MSB)
            {
                return NmeaDataID;
            }

            // Unknown type
            return new Pd0ID(0, 0, Pd0Types.Unknown);
        }

        #endregion

    }
}
