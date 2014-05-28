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
 * 04/12/2012      RC          2.09       Changed version number.
 * 04/23/2012      RC          2.10       Changed version number.  Added version number for DotSpatial.
 * 04/30/2012      RC          2.11       Changed version number.
 * 06/18/2012      RC          2.12       Changed version number.
 * 07/24/2012      RC          2.13       Changed version number.
 * 08/28/2012      RC          2.14       Changed version number.
 * 08/29/2012      RC          2.15       Changed version number.
 * 09/06/2012      RC          2.15       Added System Frequency information.
 * 09/11/2012      RC          2.15       Made version number get retrieved from AssemblyInfo.cs.
 * 01/16/2013      RC          2.17       Added list for Transform types and Measurement standard types.
 * 01/24/2014      RC          2.21.3     Added NMEA_FILE_EXT to file extensions.
 * 
 */
using System;
using System.IO;
using System.ComponentModel;

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
            /// Pulse version number.
            /// Version number is set in AssembleInfo.cs.
            /// </summary>
            public static string VERSION
            {
                get
                {
                    return System.Reflection.Assembly.LoadFrom("RTI.dll").GetName().Version.ToString();
                }
            }

            /// <summary>
            /// Used to denote Beta or Alpha builds.  Or any
            /// special branches of the application.
            /// </summary>
            public const string RTI_VERSION_ADDITIONAL = " Beta";

            /// <summary>
            /// DotSpatial Major version number. 
            /// </summary>
            public const int RTI_DOTSPATIAL_VERSION_MAJOR = 1;

            /// <summary>
            /// DotSpatial Minor version number.
            /// </summary>
            public const int RTI_DOTSPATIAL_VERSION_MINOR = 0;

            /// <summary>
            /// DotSpatial Revision.
            /// </summary>
            public const int RTI_DOTSPAITAL_VERSION_REVESION = 845;

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

            /// <summary>
            /// NMEA file extension.
            /// </summary>
            public const string NMEA_FILE_EXT = ".NMEA";

            #endregion

            #region Frequencies

            /// <summary>
            /// Used to calculate the system frequency.
            /// Divide this value by 1 - 4, to get the freqencyes of 1200kHz to 150KHz.
            /// FREQ_DIV/1 = 1200kHz.
            /// FREQ_DIV/2 = 600kHz.
            /// FREQ_DIV/4 = 300kHz.
            /// FREQ_DIV/8 = 150kHz.
            /// </summary>
            //public const float FREQ_BASE = 1245125.0f;
            public const float FREQ_BASE = 1152000.0f;

            /// <summary>
            /// Divsor to use with FREQ_DIV to get 1200khz frequency.
            /// </summary>
            public const float FREQ_DIV_1200 = 1.0f;

            /// <summary>
            /// Divsor to use with FREQ_DIV to get 600khz frequency.
            /// </summary>
            public const float FREQ_DIV_600 = 2.0f;

            /// <summary>
            /// Divsor to use with FREQ_DIV to get 300khz frequency.
            /// </summary>
            public const float FREQ_DIV_300 = 4.0f;

            /// <summary>
            /// Divsor to use with FREQ_DIV to get 150khz frequency.
            /// </summary>
            public const float FREQ_DIV_150 = 8.0f;

            /// <summary>
            /// Divsor to use with FREQ_DIV to get 75khz frequency.
            /// </summary>
            public const float FREQ_DIV_75 = 16.0f;

            /// <summary>
            /// Divsor to use with FREQ_DIV to get 38khz frequency.
            /// </summary>
            public const float FREQ_DIV_38 = 32.0f;

            #endregion

            #region Ceramic Diameter

            /// <summary>
            /// Ceramic Diameter for 300kHz 3".
            /// 3" = 0.0762m.
            /// </summary>
            public const float CERAMIC_DIA_300_3 = 0.0762f;

            /// <summary>
            /// Ceramic Diameter for 300kHz 5".
            /// 5" = 0.127m.
            /// </summary>
            public const float CERAMIC_DIA_300_5 = 0.127f;

            /// <summary>
            /// Ceramic Diameter for 600kHz 2".
            /// 2" = 0.0508m.
            /// </summary>
            public const float CERAMIC_DIA_600_2 = 0.0508f;

            /// <summary>
            /// Ceramic Diameter for 600kHz 3".
            /// 3" = 0.0762m.
            /// </summary>
            public const float CERAMIC_DIA_600_3 = 0.0762f;

            /// <summary>
            /// Ceramic Diameter for 1200kHz 1.3".
            /// 1.3" = 0.03302m.
            /// </summary>
            public const float CERAMIC_DIA_1200_1_3 = 0.03302f;

            /// <summary>
            /// Ceramic Diameter for 1200kHz 2".
            /// 2" = 0.0508m.
            /// </summary>
            public const float CERAMIC_DIA_1200_2 = 0.0508f;

            /// <summary>
            /// Ceramic Diameter for 150kHz 6".
            /// 6" = 0.152m.
            /// </summary>
            public const float CERAMIC_DIA_150_6 = 0.152f;

            /// <summary>
            /// Ceramic Diameter for 75kHz 10".
            /// 10" = 0.254m.
            /// </summary>
            public const float CERAMIC_DIA_75_10 = 0.254f;

            /// <summary>
            /// Ceramic Diameter for 38kHz 24".
            /// 24" = 0.610m.
            /// </summary>
            public const float CERAMIC_DIA_38_24 = 0.610f;

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
            /// Get the binding list for all the coordinate transforms.
            /// <returns>Return a list of all the Transform types.</returns>
            /// </summary>
            public static BindingList<Core.Commons.Transforms> GetTransformList()
            {
                BindingList<Core.Commons.Transforms> TransformList = new BindingList<Core.Commons.Transforms>();
                TransformList.Add(Core.Commons.Transforms.BEAM);
                TransformList.Add(Core.Commons.Transforms.EARTH);
                TransformList.Add(Core.Commons.Transforms.INSTRUMENT);
                return TransformList;
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

            /// <summary>
            /// Binding list of all the measurement standards.
            /// <returns>List of all the measurement standards.</returns>
            /// </summary>
            public static BindingList<Core.Commons.MeasurementStandards> GetMeasurementStandardList()
            {
                BindingList<Core.Commons.MeasurementStandards> MeasurementStandardList = new BindingList<Core.Commons.MeasurementStandards>();
                MeasurementStandardList.Add(Core.Commons.MeasurementStandards.METRIC);
                MeasurementStandardList.Add(Core.Commons.MeasurementStandards.IMPERIAL);
                return MeasurementStandardList;
            }

            #endregion
        }
    }
}