/*
 * Copyright © 2013 
 * Rowe Technology Inc.
 * All rights reserved.
 * http://www.rowetechinc.com
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification is NOT permitted.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
 * COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
 * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
 * POSSIBILITY OF SUCH DAMAGE.
 * 
 * HISTORY
 * -----------------------------------------------------------------
 * Date            Initials    Version    Comments
 * -----------------------------------------------------------------
 * 02/10/2014      RC          3.2.3      Initial coding
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
    using Newtonsoft.Json;

    /// <summary>
    /// All the options for a Vessel Mount system.
    /// This includes which heading and tilt sources 
    /// to use.
    /// </summary>
    public class VesselMountOptions
    {
        #region Variables

        #region Defaults

        /// <summary>
        /// ADCP string.
        /// </summary>
        public const string SRC_STR_ADCP = "ADCP";

        /// <summary>
        /// GPS 1 string.
        /// </summary>
        public const string SRC_STR_GPS1 = "GPS 1";

        /// <summary>
        /// GPS 2 string.
        /// </summary>
        public const string SRC_STR_GPS2 = "GPS 2";

        /// <summary>
        /// NMEA 1 string.
        /// </summary>
        public const string SRC_STR_NMEA1 = "NMEA 1";

        /// <summary>
        /// NMEA 2 string.
        /// </summary>
        public const string SRC_STR_NMEA2 = "NMEA 2";

        /// <summary>
        /// Fixed Heading string.
        /// </summary>
        public const string SRC_STR_FIXED_HEADING = "Fixed Heading";

        #endregion

        #endregion

        #region Properties

        #region Heading

        /// <summary>
        /// Heading Source.  This will be where the heading value will taken to
        /// set in the ensemble.
        /// </summary>
        public string HeadingSource { get; set; }

        /// <summary>
        /// List of all the Heading Sources.
        /// </summary>
        [JsonIgnore]
        public List<string> HeadingSourceList { get; set; }

        /// <summary>
        /// Magnetic Heading offset in degrees.
        /// </summary>
        public float HeadingOffsetMag { get; set; }

        /// <summary>
        /// Alignment Heading offset in degrees.
        /// </summary>
        public float HeadingOffsetAlignment { get; set; }

        /// <summary>
        /// If the user wants to keep the heading fixed, this value
        /// will be used for the heading.  In degrees.
        /// </summary>
        public float FixedHeading { get; set; }

        #endregion

        #region Tilt

        /// <summary>
        /// Tilt Source.
        /// </summary>
        public string TiltSource { get; set; }

        /// <summary>
        /// List of all the Tilt Sources.
        /// </summary>
        [JsonIgnore]
        public List<string> TiltSourceList { get; set; }

        /// <summary>
        /// Pitch Offset in degrees.
        /// </summary>
        public float PitchOffset { get; set; }

        /// <summary>
        /// Flag to fix the pitch only.
        /// </summary>
        public bool IsPitchFixed { get; set; }

        /// <summary>
        /// Fixed Pitch value in degrees.
        /// </summary>
        public float PitchFixed { get; set; }

        /// <summary>
        /// Roll offset in degrees.
        /// </summary>
        public float RollOffset { get; set; }

        /// <summary>
        /// Flag to fix the roll only.
        /// </summary>
        public bool IsRollFixed { get; set; }

        /// <summary>
        /// Fixed Roll value in degrees.
        /// </summary>
        public float RollFixed { get; set; }

        #endregion

        #endregion

        /// <summary>
        /// Initialize the values.
        /// </summary>
        public VesselMountOptions()
        {
            // Initialize the values
            Init();
        }

        #region Init

        /// <summary>
        /// Initialize the values.
        /// </summary>
        private void Init()
        {
            // Init lists
            InitLists();

            // Init values
            // Heading Source
            HeadingSource = SRC_STR_ADCP;
            HeadingOffsetMag = 0.0f;
            HeadingOffsetAlignment = 0.0f;
            FixedHeading = 0.0f;

            // Tilt Source
            TiltSource = SRC_STR_ADCP;
            PitchOffset = 0.0f;
            RollOffset = 0.0f;
            IsPitchFixed = false;
            IsRollFixed = false;
            PitchFixed = 0.0f;
            RollFixed = 0.0f;
        }

        /// <summary>
        /// Initialize the lists.
        /// </summary>
        private void InitLists()
        {
            // Heading sources
            HeadingSourceList = new List<string>();
            HeadingSourceList.Add(SRC_STR_ADCP);
            HeadingSourceList.Add(SRC_STR_GPS1);
            HeadingSourceList.Add(SRC_STR_GPS2);
            HeadingSourceList.Add(SRC_STR_NMEA1);
            HeadingSourceList.Add(SRC_STR_NMEA2);
            HeadingSourceList.Add(SRC_STR_FIXED_HEADING);

            // Tilt sources
            TiltSourceList = new List<string>();
            TiltSourceList.Add(SRC_STR_ADCP);
            TiltSourceList.Add(SRC_STR_GPS1);
            TiltSourceList.Add(SRC_STR_GPS2);
            TiltSourceList.Add(SRC_STR_NMEA1);
            TiltSourceList.Add(SRC_STR_NMEA2);
        }

        #endregion
    }
}
