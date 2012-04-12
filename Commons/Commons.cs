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
 * 12/29/2011      RC          1.11       Initial coding
 * 12/30/2011      RC          1.11       Added Namespace Core to sepreate between Pulse and this.
 * 01/16/2012      RC          1.13       Changed version number.
 * 01/16/2012      RC          1.14       Changed version number.
 * 02/10/2012      RC          2.02       Changed version number.
 * 02/13/2012      RC          2.03       Changed version number.
 * 02/14/2012      RC          2.03       Added file extensions.
 * 02/22/2012      RC          2.03       Added an enum for the different transform types.
 * 02/24/2012      RC          2.04       Changed version number.
 * 03/05/2012      RC          2.05       Changed version number.
 * 03/06/2012      RC          2.06       Changed version number.
 * 03/20/2012      RC          2.07       Changed version number.
 * 04/02/2012      RC          2.08       Changed version number.
 * 04/10/2012      RC          2.08       Changed the recorded file extenstion to match the ADCP.
 * 
 * 
 */
using System;
using System.IO;

namespace RTI
{
    namespace Core
    {
        /// <summary>
        /// Common values used in this project.
        /// This include version numbers.
        /// </summary>
        public class Commons
        {
            #region Version Number

            /// <summary>
            /// Application Major version number.
            /// </summary>
            public const int RTI_VERSION_MAJOR = 2;

            /// <summary>
            /// Application Minor version number.
            /// </summary>
            public const int RTI_VERSION_MINOR = 08;

            /// <summary>
            /// Used to denote Beta or Alpha builds.  Or any
            /// special branches of the application.
            /// </summary>
            public const string RTI_VERSION_ADDITIONAL = " Beta";

            #endregion

            #region File Extensions

            /// <summary>
            /// File extension for the ADCP binary data.
            /// This file will contain a single ensemble
            /// per entry.
            /// </summary>
            public const string SINGLE_ENSEMBLE_FILE_EXT = ".ENS";

            /// <summary>
            /// File extension for the Averaged ADCP binary data.
            /// This file will contain X averaged ensembles per
            /// entry.
            /// </summary>
            public const string AVG_ENSEMBLE_FILE_EXT = ".ENA";

            #endregion

            #region Enums

            /// <summary>
            /// Options to display velocity data in different
            /// transform.
            /// </summary>
            public enum Transforms
            {
                /// <summary>
                /// Beam velocity data.
                /// </summary>
                BEAM,

                /// <summary>
                /// Instrument velocity data.
                /// </summary>
                INSTRUMENT,

                /// <summary>
                /// Earth velocity data.
                /// </summary>
                EARTH
            }

            /// <summary>
            /// Options for which standard of measure
            /// to display.
            /// </summary>
            public enum MeasurementStandards
            {
                /// <summary>
                /// Metric measurement system.
                /// </summary>
                METRIC,

                /// <summary>
                /// Imperial measurement system.
                /// </summary>
                IMPERIAL
            }

            #endregion
        }
    }
}