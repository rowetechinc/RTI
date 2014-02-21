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
 * 02/12/2014      RC          2.21.3     Initial coding
 * 
 */

namespace RTI
{
    namespace VesselMount
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Text;

        /// <summary>
        /// Set the heading values based off the options.
        /// </summary>
        public class VmTiltSource
        {

            /// <summary>
            /// Change the heading source if it is not ADCP.
            /// It is assumed that the current heading stored in the
            /// ensemble is from the ADCP.  If the options are set to anything
            /// other than ADCP, the heading will be changed.  If the source
            /// does not exist, the value will not change.
            /// </summary>
            /// <param name="ensemble">Ensemble to change the value.</param>
            /// <param name="options">Options to know how the change the value.</param>
            public static void TiltSource(ref DataSet.Ensemble ensemble, VesselMountOptions options)
            {
                //switch(options.TiltSource)
                //{
                //    //case VesselMountOptions.SRC_STR_FIXED_HEADING:
                //    //    HeadingSourceFixed(ref ensemble, options.FixedHeading);
                //    //    break;
                //    case VesselMountOptions.SRC_STR_GPS1:
                //        HeadingSourceGps1(ref ensemble);
                //        break;
                //    case VesselMountOptions.SRC_STR_GPS2:
                //        HeadingSourceGps2(ref ensemble);
                //        break;
                //    case VesselMountOptions.SRC_STR_NMEA1:
                //        HeadingSourceNmea1(ref ensemble);
                //        break;
                //    case VesselMountOptions.SRC_STR_NMEA2:
                //        HeadingSourceNmea2(ref ensemble);
                //        break;
                //    case VesselMountOptions.SRC_STR_ADCP:
                //    default:
                //        break;
                //}

                // Check for a fixed pitch
                if (options.IsPitchFixed)
                {
                    SetPitch(ref ensemble, options.PitchFixed);
                }

                // Check for a fixed roll
                if (options.IsRollFixed)
                {
                    SetRoll(ref ensemble, options.RollFixed);
                }
            }

            /// <summary>
            /// Set the Pitch for the Bottom Track and Ancillary dataset.
            /// </summary>
            /// <param name="ensemble">Ensemble to set the value.</param>
            /// <param name="pitch">Pitch to use.</param>
            private static void SetPitch(ref DataSet.Ensemble ensemble, float pitch)
            {
                // Set the ancillary pitch
                if (ensemble.IsAncillaryAvail)
                {
                    ensemble.AncillaryData.Pitch = pitch;
                }

                // Set the bottom track pitch
                if (ensemble.IsBottomTrackAvail)
                {
                    ensemble.BottomTrackData.Pitch = pitch;
                }
            }

            /// <summary>
            /// Set the Roll for the Bottom Track and Ancillary dataset.
            /// </summary>
            /// <param name="ensemble">Ensemble to set the value.</param>
            /// <param name="roll">Roll to use.</param>
            private static void SetRoll(ref DataSet.Ensemble ensemble, float roll)
            {
                // Set the ancillary roll
                if (ensemble.IsAncillaryAvail)
                {
                    ensemble.AncillaryData.Roll = roll;
                }

                // Set the bottom track roll
                if (ensemble.IsBottomTrackAvail)
                {
                    ensemble.BottomTrackData.Roll = roll;
                }
            }

            ///// <summary>
            ///// Set the heading based off the GPS 1 data.  This will get the GPS 1 data and
            ///// get the heading value from GPHDT or GPRMC sentences.  It will set the heading
            ///// to this value.
            ///// </summary>
            ///// <param name="ensemble">Ensemble to set the value.</param>
            //private static void TiltSourceGps1(ref DataSet.Ensemble ensemble)
            //{
            //    double heading = 0.0;

            //    // Check if GPS 1 data exist
            //    if (ensemble.IsGps1DataAvail)
            //    {
            //        // Create a NMEA dataset to decode the data
            //        DataSet.NmeaDataSet gps1 = new DataSet.NmeaDataSet(ensemble.Gps1Data);

            //        // GPHDT
            //        if (gps1.IsGphdtAvail())
            //        {
            //            heading = gps1.GPHDT.Heading.DecimalDegrees;
            //        }
            //        // GPRMC
            //        else if (gps1.IsGprmcAvail())
            //        {
            //            heading = gps1.GPRMC.Bearing.DecimalDegrees;
            //        }
            //    }

            //    // Set the heading
            //    SetHeading(ref ensemble, (float)heading);

            //}

            ///// <summary>
            ///// Set the heading based off the GPS 2 data.  This will get the GPS 2 data and
            ///// get the heading value from GPHDT or GPRMC sentences.  It will set the heading
            ///// to this value.
            ///// </summary>
            ///// <param name="ensemble">Ensemble to set the value.</param>
            //private static void TiltSourceGps2(ref DataSet.Ensemble ensemble)
            //{
            //    double heading = 0.0;

            //    // Check if GPS 2 data exist
            //    if (ensemble.IsGps2DataAvail)
            //    {
            //        // Create a NMEA dataset to decode the data
            //        DataSet.NmeaDataSet gps2 = new DataSet.NmeaDataSet(ensemble.Gps2Data);

            //        // GPHDT
            //        if (gps2.IsGphdtAvail())
            //        {
            //            heading = gps2.GPHDT.Heading.DecimalDegrees;
            //        }
            //        // GPRMC
            //        else if (gps2.IsGprmcAvail())
            //        {
            //            heading = gps2.GPRMC.Bearing.DecimalDegrees;
            //        }
            //    }

            //    // Set the heading
            //    SetHeading(ref ensemble, (float)heading);
            //}

            ///// <summary>
            ///// Set the heading based off the NMEA 1 data.  This will get the NMEA 1 data and
            ///// get the heading value from GPHDT or GPRMC sentences.  It will set the heading
            ///// to this value.
            ///// </summary>
            ///// <param name="ensemble">Ensemble to set the value.</param>
            //private static void TiltSourceNmea1(ref DataSet.Ensemble ensemble)
            //{
            //    double heading = 0.0;

            //    // Check if NMEA 1 data exist
            //    if (ensemble.IsNmea1DataAvail)
            //    {
            //        // Create a NMEA dataset to decode the data
            //        DataSet.NmeaDataSet nmea1 = new DataSet.NmeaDataSet(ensemble.Nmea1Data);

            //        // GPHDT
            //        if (nmea1.IsGphdtAvail())
            //        {
            //            heading = nmea1.GPHDT.Heading.DecimalDegrees;
            //        }
            //        // GPRMC
            //        else if (nmea1.IsGprmcAvail())
            //        {
            //            heading = nmea1.GPRMC.Bearing.DecimalDegrees;
            //        }
            //    }

            //    // Set the heading
            //    SetHeading(ref ensemble, (float)heading);
            //}

            ///// <summary>
            ///// Set the heading based off the NMEA 2 data.  This will get the NMEA 2 data and
            ///// get the heading value from GPHDT or GPRMC sentences.  It will set the heading
            ///// to this value.
            ///// </summary>
            ///// <param name="ensemble">Ensemble to set the value.</param>
            //private static void TiltSourceNmea2(ref DataSet.Ensemble ensemble)
            //{
            //    double heading = 0.0;

            //    // Check if NMEA 2 data exist
            //    if (ensemble.IsNmea2DataAvail)
            //    {
            //        // Create a NMEA dataset to decode the data
            //        DataSet.NmeaDataSet nmea2 = new DataSet.NmeaDataSet(ensemble.Nmea2Data);

            //        // GPHDT
            //        if (nmea2.IsGphdtAvail())
            //        {
            //            heading = nmea2.GPHDT.Heading.DecimalDegrees;
            //        }
            //        // GPRMC
            //        else if (nmea2.IsGprmcAvail())
            //        {
            //            heading = nmea2.GPRMC.Bearing.DecimalDegrees;
            //        }
            //    }

            //    // Set the heading
            //    SetHeading(ref ensemble, (float)heading);
            //}

        }

    }
}
