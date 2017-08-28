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
 * 07/23/2012      RC          2.19.1     Initial coding
 * 07/25/2013      RC          2.19.2     Added Instrument transform.
 *                                         Updated how Pitch and Roll are used per SM.
 * 05/07/2014      RC          2.21.4     Added Correlation and SNR threshold.
 * 01/06/2016      RC          3.3.0      Added GPS heading and Heading offset.
 * 05/11/2016      RC          3.3.2      Fixed bug for Q value.
 * 08/25/2017      RC          3.4.2      Added Water Mass trasnformation.  Added Ship trasnformation to Bottom Track.
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Transform the data.  This is used to convert BEAM velocity data to 
    /// Earth velocity data.
    /// </summary>
    public class Transform
    {
        #region Profile Transform

        /// <summary>
        /// Convert the Beam velocity data to Instrument Velocity and Earth velocity data.  If the dataset does not exist in the ensemble,
        /// it will be created and then filled with the data.  If the ensemble already has the Instrument/Earth dataset, the
        /// data will be overwritten with new data.
        /// 
        /// If the isUseGpsHeading is set to true it will use the GPS heading if it exist.  If there is no GPS data, or no GPS heading,
        /// fall back to the ancillary heading data.
        /// 
        /// Add the heading offset to the heading value before using it to retransform the data.
        /// 
        /// Correlation Thresholds Defaults
        /// BB = 0.25f
        /// NB = 0.80f
        /// </summary>
        /// <param name="ensemble">Ensemble to add the Earth velocity data.</param>
        /// <param name="corrThresh">Correlation Threshold.</param>
        /// <param name="isUseGpsHeading">Use the GPS heading instead of the Anciallary data.</param>
        /// <param name="headingOffset">Heading offset to add to the heading.</param>
        /// <param name="shipXdcrOffset">The calcuate ship coordinate transform, i need to know the angle offset between Beam 0 and the front of the boat.</param>
        public static void ProfileTransform(ref DataSet.Ensemble ensemble, float corrThresh = 0.25f, bool isUseGpsHeading = false, float headingOffset = 0.0f, float shipXdcrOffset = 0.0f)
        {
            // Create the array to hold the earth data
            int numBins = 0;
            int numBeams = 0;
            if(ensemble.IsEnsembleAvail)
            {
                numBins = ensemble.EnsembleData.NumBins;
                numBeams = ensemble.EnsembleData.NumBeams;
            }

            // Create the Instrument dataset if it does not exist
            if (!ensemble.IsInstrumentVelocityAvail)
            {
                EnsembleHelper.AddInstrumentVelocity(ref ensemble, numBins, numBeams);
            }
            else
            {
                // Recreate the array
                ensemble.InstrumentVelocityData.InstrumentVelocityData = new float[numBins, numBeams];
            }

            // Create the Earth dataset if it does not exist
            if (!ensemble.IsEarthVelocityAvail)
            {
                EnsembleHelper.AddEarthVelocity(ref ensemble, numBins, numBeams);
            }
            else
            {
                // Recreate the array
                ensemble.EarthVelocityData.EarthVelocityData = new float[numBins, numBeams];
            }

            // Create the Earth Good dataset if it does not exist
            if (!ensemble.IsGoodEarthAvail)
            {
                EnsembleHelper.AddGoodEarth(ref ensemble, numBins, numBeams);
            }
            else
            {
                // Recreate the array
                ensemble.GoodEarthData.GoodEarthData = new int[numBins, numBeams];
            }

            // If there is no ensemble data, we cannot get heading, pitch and roll
            // If there is no beam data, we have nothing to transform
            // If there is no correlation data, we cannot verify the beam data is good
            if(!ensemble.IsEnsembleAvail || !ensemble.IsBeamVelocityAvail || !ensemble.IsCorrelationAvail)
            {
                return;
            }

            // Set the Heading pitch and roll
            float Heading = 0.0f;
            float Pitch = 0.0f;
            float Roll = 0.0f;
            if(isUseGpsHeading)
            {
                if(ensemble.IsNmeaAvail)
                {
                    // GPHDT
                    if (ensemble.NmeaData.IsGphdtAvail())
                    {
                        Heading = (float)ensemble.NmeaData.GPHDT.Heading.DecimalDegrees;
                        
                        // If NaN, use backup of Ancillary heading
                        if (float.IsNaN(Heading))
                        {
                            Heading = ensemble.AncillaryData.Heading;
                        }
                    }
                    // GPRMC
                    else if (ensemble.NmeaData.IsGprmcAvail())
                    {
                        Heading = (float)ensemble.NmeaData.GPRMC.Bearing.DecimalDegrees;

                        // If NaN, use backup of Ancillary heading
                        if (float.IsNaN(Heading))
                        {
                            Heading = ensemble.AncillaryData.Heading;
                        }
                    }
                    // Ancillary
                    else if(ensemble.IsAncillaryAvail)
                    {
                        Heading = ensemble.AncillaryData.Heading;
                    }
                    // Bottom Track
                    else if(ensemble.IsBottomTrackAvail)
                    {
                        Heading = ensemble.BottomTrackData.Heading;
                    }
                }
            }
            else if(ensemble.IsAncillaryAvail)
            {
                Heading = ensemble.AncillaryData.Heading;
            }
            else if (ensemble.IsBottomTrackAvail)
            {
                Heading = ensemble.BottomTrackData.Heading;
            }

            // Set pitch and roll
            if (ensemble.IsAncillaryAvail)
            {
                Pitch = ensemble.AncillaryData.Pitch;
                Roll = ensemble.AncillaryData.Roll;
            }
            else if (ensemble.IsBottomTrackAvail)
            {
                Pitch = ensemble.BottomTrackData.Pitch;
                Roll = ensemble.BottomTrackData.Roll;
            }

            // Add the heading offset
            Heading += headingOffset;

            // Set the new Heading value back into the ensemble
            // Do not set the Bottom Track heading here, because there is a 
            // seperate Bottom Track Transform which will then apply the heading offset again if replaced.
            if(ensemble.IsAncillaryAvail)
            {
                ensemble.AncillaryData.Heading = Heading;
            }

            // Subsystem selection
            switch(ensemble.EnsembleData.SubsystemConfig.SubSystem.Code)
            {
                default:
                    break;
                case Subsystem.SUB_1_2MHZ_4BEAM_20DEG_PISTON_OPPOSITE_FACING_c:
                case Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_OPPOSITE_FACING_d:
                case Subsystem.SUB_300KHZ_4BEAM_20DEG_PISTON_OPPOSITE_FACING_e:
                    if (Roll < 0)
                    {
                        Roll = 180.0f + Roll;
                    }
                    else
                    {
                        Roll = Roll - 180.0f;
                    }
                    break;
            }

            // Calculate new Pitch and Roll for Beam0 offset from compass bin mapping
            //
            // A = new angle +45 degrees for second piston system
            // P = Pitch
            // R = Roll
            //
            // P' = PcosA - RsinA
            // R' = PsinA + RcosA
            float P, R;

            float Bm0CosHeading = 1.0f;      // For a XDCR that is forward facing.  If it is 45 degrees offset, you would do COS of 45 degrees.  This if found based off the subsystem type  
            float Bm0SinHeading = 0.0f;      // For a XDCR that is forward facing.  If it is 45 degrees offset, you would do SIN of 45 degrees.  This if found based off the subsystem type 

            // Subsystems that are off 45 degrees
            switch(ensemble.EnsembleData.SubsystemConfig.SubSystem.Code)
            {
                case RTI.Subsystem.SUB_2MHZ_4BEAM_20DEG_PISTON_45OFFSET_5:
                case RTI.Subsystem.SUB_1_2MHZ_4BEAM_20DEG_PISTON_45OFFSET_6:
                case RTI.Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_45OFFSET_7:
                case RTI.Subsystem.SUB_300KHZ_4BEAM_20DEG_PISTON_45OFFSET_8:
                    Bm0CosHeading = (float)Math.Cos(45.0);
                    Bm0SinHeading = (float)Math.Sin(45.0);
                    break;
                default:
                    break;
            }

            if (Roll >= 90.0f || Roll <= -90.0f) // Down facing case
            {
                float R1;
                float B1;
                if (Roll > 90.0f)
                {
                    B1 = -180.0f;
                }
                else
                {
                    B1 = +180.0f;
                }

                R1 = Roll + B1;

                P = Pitch * Bm0CosHeading - R1 * Bm0SinHeading;
                R = Pitch * Bm0SinHeading + R1 * Bm0CosHeading - B1;
            }
            else // Up Facing case
            {
                P = Pitch * Bm0CosHeading + Roll * Bm0SinHeading;
                R = -Pitch * Bm0SinHeading + Roll * Bm0CosHeading;
            }

            double SP = Math.Sin(Math.PI * P / 180.0);
            double CP = Math.Cos(Math.PI * P / 180.0);
            double SR = Math.Sin(Math.PI * R / 180.0);
            double CR = Math.Cos(Math.PI * R / 180.0);
            double SH = Math.Sin(Math.PI * Heading / 180.0);                                                    // Earth Coordinate Transform Compass Heading
            double CH = Math.Cos(Math.PI * Heading / 180.0);                                                    // Earth Coordinate Transform Compass Heading
            double SSH = Math.Sin(Math.PI * shipXdcrOffset / 180.0);                                            // Ship Coordinate Transform Ship Alignment Offset
            double SCH = Math.Cos(Math.PI * shipXdcrOffset / 180.0);                                            // Ship Coordinate Transform Ship Alignment Offset

            float GOOD_CORR = corrThresh;                                                                       // Good Correlation threshold

            // Rotate to ENU
            // Check to see if a three beam solution is needed
            // Go through each bin
            for(int bin = 0; bin < numBins; bin++)
            {
                int goodBeam = 0;                                                                               // Count the number of good beams
                int badBeam = 0;                                                                                // Keep track of bad beam
                float[] tempVel = new float[numBeams];                                                          // Array to temperary hold velocity values
                for (int beam = 0; beam < numBeams; beam++)
                {
                    ensemble.GoodEarthData.GoodEarthData[bin, beam] = 0;                                        // Initialize the Good Earth data
                    if (ensemble.CorrelationData.CorrelationData[bin, beam] >= GOOD_CORR &&                     // Check Correlation is above threshold
                        ensemble.BeamVelocityData.BeamVelocityData[bin,beam] != DataSet.Ensemble.BAD_VELOCITY)  // Check that the velocity is good (could be forced marked bad)
                    {
                        goodBeam++;                                                                             // Set count of good beams
                        tempVel[beam] = ensemble.BeamVelocityData.BeamVelocityData[bin, beam];                  // Store the good velocity value
                    }
                    else
                    {
                        badBeam = beam;                                                                         // Store the last bad beam
                    }
                }

                // Check how many beams were good
                // If there were at least 3 good beams, we can do a 3 beam solution
                if (goodBeam >= 3)
                {
                    // Check if we need to do a 3 beam solution
                    if (goodBeam == 3)
                    {
                        // Q = (tempVel[0] + tempVel[1] - tempVel[2] - tempVel[3];
                        // Set Q = 0 then solve for missing beam
                        switch(badBeam)
                        {
                            case 0:
                                tempVel[badBeam] = -tempVel[1] + tempVel[2] + tempVel[3];
                                break;
                            case 1:
                                tempVel[badBeam] = -tempVel[0] + tempVel[2] + tempVel[3];
                                break;
                            case 2:
                                tempVel[badBeam] = tempVel[0] + tempVel[1] - tempVel[3];
                                break;
                            case 3:
                                tempVel[badBeam] = tempVel[0] + tempVel[1] - tempVel[2];
                                break;
                            default:
                                break;
                        }
                    }

                    // Values used to calculate nominal beam angle
                    float beamAngle = 20.0f;                                        // This value should be found based off subsystem type
                    float[,] M = new float[4, 4];
                    float s = (float)Math.Sin(beamAngle / 180.0 * Math.PI);         // SIN of the beam angle
                    float c = (float)Math.Cos(beamAngle / 180.0 * Math.PI);         // COS of the beam angle
                    //X
                    M[0, 0] = -1 / (2 * s);
                    M[0, 1] = 1 / (2 * s);
                    M[0, 2] = 0;
                    M[0, 3] = 0;
                    //Y
                    M[1, 0] = 0;
                    M[1, 1] = 0;
                    M[1, 2] = -1 / (2 * s);
                    M[1, 3] = 1 / (2 * s);
                    //Z
                    M[2, 0] = -1 / (4 * c);
                    M[2, 1] = -1 / (4 * c);
                    M[2, 2] = -1 / (4 * c);
                    M[2, 3] = -1 / (4 * c);
                    //Q
                    M[3, 0] = (float)0.25;
                    M[3, 1] = (float)0.25;
                    M[3, 2] = (float)-0.25;
                    M[3, 3] = (float)-0.25;

                    float X = (tempVel[0] * M[0, 0]                                                 // Profile does not invert the value like BT
                              + tempVel[1] * M[0, 1]
                              + tempVel[2] * M[0, 2]
                              + tempVel[3] * M[0, 3]);

                    float Y = (tempVel[0] * M[1, 0]                                                 // Profile does not invert the value like BT
                              + tempVel[1] * M[1, 1]
                              + tempVel[2] * M[1, 2]
                              + tempVel[3] * M[1, 3]);

                    // Rotate Axis to align beam 0 with compass
                    float X1 = X;
                    float Y1 = Y;
                    X = X1 * Bm0CosHeading - Y1 * Bm0SinHeading;
                    Y = X1 * Bm0SinHeading + Y1 * Bm0CosHeading;

                    // If doppler array the vertical angle uses current speed of sound
                    float Z = (tempVel[0] * M[2, 0]                                                 // Profile does not invert the value like BT
                              + tempVel[1] * M[2, 1]
                              + tempVel[2] * M[2, 2]
                              + tempVel[3] * M[2, 3]);

                    // Instrument Velocity Transform
                    // X
                    ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_X_INDEX] = X;

                    // Y
                    ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_Y_INDEX] = Y;

                    // Z
                    ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_Z_INDEX] = Z;


                    // Ship Velocity Transform
                    // Transverse 
                    ensemble.ShipVelocityData.ShipVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] = (float)(
                                                                        X * (SSH * CP)
                                                                      - Y * (SCH * CR + SSH * SR * SP)
                                                                      + Z * (SCH * SR - SSH * CR * SP));

                    // Longitudinal 
                    ensemble.ShipVelocityData.ShipVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] = (float)(
                                                                        X * (SCH * CP)
                                                                      + Y * (SSH * CR - SCH * SR * SP)
                                                                      - Z * (SSH * SR + SCH * SP * CR));

                    // Normal
                    ensemble.ShipVelocityData.ShipVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = (float)(
                                                                        X * (SP)
                                                                      + Y * (SR * CP)
                                                                      + Z * (CP * CR));


                    // Earth Velocity Transform
                    // East
                    ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] = (float)(
                                                                        X * (SH * CP)
                                                                      - Y * (CH * CR + SH * SR * SP)
                                                                      + Z * (CH * SR - SH * CR * SP));

                    // North
                    ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] = (float)(
                                                                        X * (CH * CP)
                                                                      + Y * (SH * CR - CH * SR * SP)
                                                                      - Z * (SH * SR + CH * SP * CR));

                    // Up
                    ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = (float)(
                                                                        X * (SP)
                                                                      + Y * (SR * CP)
                                                                      + Z * (CP * CR));

                    // Set the Good Earth data
                    ensemble.GoodEarthData.GoodEarthData[bin, DataSet.Ensemble.BEAM_EAST_INDEX] = 1;
                    ensemble.GoodEarthData.GoodEarthData[bin, DataSet.Ensemble.BEAM_NORTH_INDEX] = 1;
                    ensemble.GoodEarthData.GoodEarthData[bin, DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 1;

                    // Check if there was a 3 beam solution
                    if (goodBeam == 3)
                    {
                        // 3 Beam solution so set values to 0 for Q
                        ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_Q_INDEX] = 0;
                        ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_Q_INDEX] = 0.0f;
                        ensemble.GoodEarthData.GoodEarthData[bin, DataSet.Ensemble.BEAM_Q_INDEX] = 0;
                    }
                    else
                    {
                        float Q = (tempVel[0] * M[3, 0]                                                 // Profile does not invert the value like BT
                                 + tempVel[1] * M[3, 1]
                                 + tempVel[2] * M[3, 2]
                                 + tempVel[3] * M[3, 3]);
                        
                        // Q
                        ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, DataSet.Ensemble.BEAM_Q_INDEX] = Q;

                        // Q
                        ensemble.EarthVelocityData.EarthVelocityData[bin, DataSet.Ensemble.BEAM_Q_INDEX] = Q;
                        ensemble.GoodEarthData.GoodEarthData[bin, DataSet.Ensemble.BEAM_Q_INDEX] = 1;
                    }
                }
                else
                {
                    // Set all the Instrument and Earth and Good Earth to 0
                    for(int x = 0; x < numBeams; x++)
                    {
                        ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, x] = 0.0f;
                        ensemble.EarthVelocityData.EarthVelocityData[bin, x] = 0.0f;
                        ensemble.GoodEarthData.GoodEarthData[bin, x] = 0;
                    }
                }
            }

            // Create the new velocity vectors based off the new data
            DataSet.VelocityVectorHelper.CreateVelocityVector(ref ensemble);

            return;
        }

        #endregion

        #region Bottom Track Transform

        /// <summary>
        /// Convert the BottomTrack Beam velocity data to Bottom Track Instrument and Earth velocity data.  If the dataset does not exist in the ensemble,
        /// it will be created and then filled with the data.  If the ensemble already has the Instrument/Earth dataset, the
        /// data will be overwritten with new data.
        /// 
        /// If the isUseGpsHeading is set to true it will use the GPS heading if it exist.  If there is no GPS data, or no GPS heading,
        /// fall back to the ancillary heading data.
        /// 
        /// Add the heading offset to the heading value before using it to retransform the data.
        /// 
        /// Correlation Thresholds Default
        /// BB = 0.90f
        /// 
        /// SNR Thresold Default
        /// BB = 10.0f
        /// </summary>
        /// <param name="ensemble">Ensemble to add the Earth velocity data.</param>
        /// <param name="corrThresh">Correlation Threshold.</param>
        /// <param name="snrThresh">SNR Threshold.</param>
        /// <param name="isUseGpsHeading">Use the GPS heading instead of the Anciallary data.</param>
        /// <param name="headingOffset">Heading offset to add to the heading.</param>
        /// <param name="shipXdcrOffset">The calcuate ship coordinate transform, i need to know the angle offset between Beam 0 and the front of the boat.</param>
        public static void BottomTrackTransform(ref DataSet.Ensemble ensemble, float corrThresh = 0.90f, float snrThresh = 10.0f, bool isUseGpsHeading = false, float headingOffset = 0.0f, float shipXdcrOffset = 0.0f)
        {
            // If there is no ensemble data, we cannot get the subsystem
            // If there is no Bottom Track data, we have nothing to transform
            if (!ensemble.IsEnsembleAvail || !ensemble.IsBottomTrackAvail)
            {
                return;
            }

            // Create the array to hold the earth data
            int numBeams = (int)ensemble.BottomTrackData.NumBeams;

            // Recreate the BT Instrument array
            ensemble.BottomTrackData.InstrumentVelocity = new float[numBeams];

            // Recreate the BT Earth array
            ensemble.BottomTrackData.EarthVelocity = new float[numBeams];

            // Recreate the Bottom Track Earth Good dataset if it does not exist
            ensemble.BottomTrackData.EarthGood = new float[numBeams];

            //// Set the Heading pitch and roll
            //float Heading = ensemble.BottomTrackData.Heading;
            float Pitch = ensemble.BottomTrackData.Pitch;
            float Roll = ensemble.BottomTrackData.Roll;

            // Set the Heading pitch and roll
            float Heading = 0.0f;
            if (isUseGpsHeading)
            {
                if (ensemble.IsNmeaAvail)
                {
                    // GPHDT
                    if (ensemble.NmeaData.IsGphdtAvail())
                    {
                        Heading = (float)ensemble.NmeaData.GPHDT.Heading.DecimalDegrees;

                        // If NaN, use backup of Bottom Track heading
                        if (float.IsNaN(Heading))
                        {
                            Heading = ensemble.BottomTrackData.Heading;
                        }

                    }
                    // GPRMC
                    else if (ensemble.NmeaData.IsGprmcAvail())
                    {
                        Heading = (float)ensemble.NmeaData.GPRMC.Bearing.DecimalDegrees;

                        // If NaN, use backup of Bottom Track heading
                        if(float.IsNaN(Heading))
                        {
                            Heading = ensemble.BottomTrackData.Heading;
                        }
                    }
                    // Bottom Track
                    else
                    {
                        Heading = ensemble.BottomTrackData.Heading;
                    }
                }
            }
            else
            {
                Heading = ensemble.BottomTrackData.Heading;
            }

            // Add the heading offset
            Heading += headingOffset;

            // Set the heading back to the ensemble
            // Do not set the Ancillary heading here, because there is a 
            // seperate Water Profile Transform which will then apply the heading offset again if replaced.
            ensemble.BottomTrackData.Heading = Heading;

            // Subsystem selection
            switch (ensemble.EnsembleData.SubsystemConfig.SubSystem.Code)
            {
                default:
                    break;
                case Subsystem.SUB_1_2MHZ_4BEAM_20DEG_PISTON_OPPOSITE_FACING_c:
                case Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_OPPOSITE_FACING_d:
                case Subsystem.SUB_300KHZ_4BEAM_20DEG_PISTON_OPPOSITE_FACING_e:
                    if (Roll < 0)
                    {
                        Roll = 180.0f + Roll;
                    }
                    else
                    {
                        Roll = Roll - 180.0f;
                    }
                    break;
            }

            // Calculate new Pitch and Roll for Beam0 offset from compass bin mapping
            //
            // A = new angle +45 degrees for second piston system
            // P = Pitch
            // R = Roll
            //
            // P' = PcosA - RsinA
            // R' = PsinA + RcosA
            float P, R;

            float Bm0CosHeading = 1.0f;      // For a XDCR that is forward facing.  If it is 45 degrees offset, you would do COS of 45 degrees.  This if found based off the subsystem type  
            float Bm0SinHeading = 0.0f;      // For a XDCR that is forward facing.  If it is 45 degrees offset, you would do SIN of 45 degrees.  This if found based off the subsystem type 

            if (Roll >= 90.0f || Roll <= -90.0f) // Down facing case
            {
                float R1;
                float B1;
                if (Roll > 90.0f)
                {
                    B1 = -180.0f;
                }
                else
                {
                    B1 = +180.0f;
                }

                R1 = Roll + B1;

                P = Pitch * Bm0CosHeading - R1 * Bm0SinHeading;
                R = Pitch * Bm0SinHeading + R1 * Bm0CosHeading - B1;
            }
            else // Up Facing case
            {
                P = Pitch * Bm0CosHeading + Roll * Bm0SinHeading;
                R = -Pitch * Bm0SinHeading + Roll * Bm0CosHeading;
            }

            double SP = Math.Sin(Math.PI * P / 180.0);
            double CP = Math.Cos(Math.PI * P / 180.0);
            double SR = Math.Sin(Math.PI * R / 180.0);
            double CR = Math.Cos(Math.PI * R / 180.0);
            double SH = Math.Sin(Math.PI * Heading / 180.0);                // Earth Coordinate Compass Heading
            double CH = Math.Cos(Math.PI * Heading / 180.0);                // Earth Coordinate Compass Heading
            double SSH = Math.Sin(Math.PI * shipXdcrOffset / 180.0);        // Ship Coordinate Offset
            double SCH = Math.Cos(Math.PI * shipXdcrOffset / 180.0);        // Ship Coordinate Offset

            // Rotate to ENU
            // Check to see if a three beam solution is needed
            int goodBeam = 0;                                                                               // Count the number of good beams
            int badBeam = 0;                                                                                // Keep track of bad beam
            float GOOD_CORR = corrThresh;                                                                   // Good Correlation threshold
            float GOOD_SNR = snrThresh;                                                                     // Good SNR threshold
            float[] tempVel = new float[numBeams];                                                          // Array to temperary hold velocity values
            for (int beam = 0; beam < numBeams; beam++)
            {
                ensemble.BottomTrackData.EarthGood[beam] = 0;                                               // Initialize the Good Earth data
                if (ensemble.BottomTrackData.Correlation[beam] >= GOOD_CORR &&                              // Check Correlation is above threshold
                    ensemble.BottomTrackData.SNR[beam] >= GOOD_SNR &&                                       // Check SNR is above threshold
                    ensemble.BottomTrackData.BeamVelocity[beam] != DataSet.Ensemble.BAD_VELOCITY)           // Check that the velocity is good (could be forced marked bad)
                {
                    goodBeam++;                                                                             // Set count of good beams
                    tempVel[beam] = ensemble.BottomTrackData.BeamVelocity[beam];                            // Store the good velocity value
                }
                else
                {
                    badBeam = beam;                                                                         // Store the last bad beam
                }
            }

            // Check how many beams were good
            // If there were at least 3 good beams, we can do a 3 beam solution
            if (goodBeam >= 3)
            {
                // Check if we need to do a 3 beam solution
                if (goodBeam == 3)
                {
                    // Q = (tempVel[0] + tempVel[1] - tempVel[2] - tempVel[3];
                    // Set Q = 0 then solve for missing beam
                    switch (badBeam)
                    {
                        case 0:
                            tempVel[badBeam] = -tempVel[1] + tempVel[2] + tempVel[3];
                            break;
                        case 1:
                            tempVel[badBeam] = -tempVel[0] + tempVel[2] + tempVel[3];
                            break;
                        case 2:
                            tempVel[badBeam] = tempVel[0] + tempVel[1] - tempVel[3];
                            break;
                        case 3:
                            tempVel[badBeam] = tempVel[0] + tempVel[1] - tempVel[2];
                            break;
                        default:
                            break;
                    }
                }

                // Values used to calculate nominal beam angle
                float beamAngle = 20.0f;                                        // This value should be found based off subsystem type
                float[,] M = new float[4, 4];
                float s = (float)Math.Sin(beamAngle / 180.0 * Math.PI);         // SIN of the beam angle
                float c = (float)Math.Cos(beamAngle / 180.0 * Math.PI);         // COS of the beam angle
                //X
                M[0, 0] = -1 / (2 * s);
                M[0, 1] = 1 / (2 * s);
                M[0, 2] = 0;
                M[0, 3] = 0;
                //Y
                M[1, 0] = 0;
                M[1, 1] = 0;
                M[1, 2] = -1 / (2 * s);
                M[1, 3] = 1 / (2 * s);
                //Z
                M[2, 0] = -1 / (4 * c);
                M[2, 1] = -1 / (4 * c);
                M[2, 2] = -1 / (4 * c);
                M[2, 3] = -1 / (4 * c);
                //Q
                M[3, 0] = (float)0.25;
                M[3, 1] = (float)0.25;
                M[3, 2] = (float)-0.25;
                M[3, 3] = (float)-0.25;

                float X = -(tempVel[0] * M[0, 0]
                            + tempVel[1] * M[0, 1]
                            + tempVel[2] * M[0, 2]
                            + tempVel[3] * M[0, 3]);

                float Y = -(tempVel[0] * M[1, 0]
                            + tempVel[1] * M[1, 1]
                            + tempVel[2] * M[1, 2]
                            + tempVel[3] * M[1, 3]);

                // Rotate Axis to align beam 0 with compass
                float X1 = X;
                float Y1 = Y;
                X = X1 * Bm0CosHeading - Y1 * Bm0SinHeading;
                Y = X1 * Bm0SinHeading + Y1 * Bm0CosHeading;

                // If doppler array the vertical angle uses current speed of sound
                float Z = -(tempVel[0] * M[2, 0]
                            + tempVel[1] * M[2, 1]
                            + tempVel[2] * M[2, 2]
                            + tempVel[3] * M[2, 3]);

                // Instrument Coordintate Transform
                // X
                ensemble.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_X_INDEX] = X;

                // Y
                ensemble.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_Y_INDEX] = Y;

                // Z
                ensemble.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_Z_INDEX] = Z;


                // Ship Coordinate Transform
                float[] shipVelocity = new float[4];
                // Transverse (+ = Port Stbd ship movement rel. to water mass)
                ensemble.BottomTrackData.ShipVelocity[0] = (float)(
                                                                    X * (SSH * CP)
                                                                    - Y * (SCH * CR + SSH * SR * SP)
                                                                    + Z * (SCH * SR - SSH * CR * SP));

                // Longitudinal (+ = Aft Fwd ship movement rel. to water mass)
                ensemble.BottomTrackData.ShipVelocity[1] = (float)(
                                                                    X * (SCH * CP)
                                                                    + Y * (SSH * CR - SCH * SR * SP)
                                                                    - Z * (SSH * SR + SCH * SP * CR));

                // Normal (+ = ship movement away from water mass)
                ensemble.BottomTrackData.ShipVelocity[2] = (float)(
                                                                    X * (SP)
                                                                    + Y * (SR * CP)
                                                                    + Z * (CP * CR));

                // Earth Coordinate Transform
                // East
                ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_EAST_INDEX] = (float)(
                                                                    X * (SH * CP)
                                                                    - Y * (CH * CR + SH * SR * SP)
                                                                    + Z * (CH * SR - SH * CR * SP));

                // North
                ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_NORTH_INDEX] = (float)(
                                                                    X * (CH * CP)
                                                                    + Y * (SH * CR - CH * SR * SP)
                                                                    - Z * (SH * SR + CH * SP * CR));

                // Up
                ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX] = (float)(
                                                                    X * (SP)
                                                                    + Y * (SR * CP)
                                                                    + Z * (CP * CR));


                // Set the Good Earth data
                ensemble.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_EAST_INDEX] = 1;
                ensemble.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_NORTH_INDEX] = 1;
                ensemble.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_VERTICAL_INDEX] = 1;

                // Check if there was a 3 beam solution
                if (goodBeam == 3)
                {
                    // 3 Beam solution so set values to 0 for Q
                    ensemble.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_Q_INDEX] = 0.0f;
                    ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_Q_INDEX] = 0.0f;
                    ensemble.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_Q_INDEX] = 0;
                }
                else
                {
                    // Error value
                    float Q = -(tempVel[0] * M[3, 0]
                              + tempVel[1] * M[3, 1]
                              + tempVel[2] * M[3, 2]
                              + tempVel[3] * M[3, 3]);

                    ensemble.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_Q_INDEX] = Q;
                    ensemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_Q_INDEX] = Q;
                    ensemble.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_Q_INDEX] = 1;
                }
            }
            else
            {
                // Set all the Instrument, Earth and Good Earth to 0
                for (int x = 0; x < numBeams; x++)
                {
                    ensemble.BottomTrackData.InstrumentVelocity[x] = 0.0f;
                    ensemble.BottomTrackData.EarthVelocity[x] = 0.0f;
                    ensemble.BottomTrackData.EarthGood[x] = 0;
                }
            }

            // Create the new velocity vectors based off the new data
            //DataSet.VelocityVectorHelper.CreateVelocityVector(ref ensemble);

            return;
        }

        #endregion

        #region Water Mass Transform

        /// <summary>
        /// Convert the Water Mass Instrument Data to Earth and Ship coordinate transform.
        /// 
        /// If the isUseGpsHeading is set to true it will use the GPS heading if it exist.  If there is no GPS data, or no GPS heading,
        /// fall back to the ancillary heading data.
        /// 
        /// Add the heading offset to the heading value before using it to retransform the data.
        /// 
        /// Correlation Thresholds Default
        /// BB = 0.90f
        /// 
        /// SNR Thresold Default
        /// BB = 10.0f
        /// </summary>
        /// <param name="ensemble">Ensemble to add the Earth velocity data.</param>
        /// <param name="corrThresh">Correlation Threshold.</param>
        /// <param name="snrThresh">SNR Threshold.</param>
        /// <param name="isUseGpsHeading">Flag if using GPS heading or compass heading.</param>
        /// <param name="headingOffset">Heading offset to add to the heading.</param>
        /// <param name="shipXdcrOffset">The calcuate ship coordinate transform, i need to know the angle offset between Beam 0 and the front of the boat.</param>
        public static void WaterMassTransform(ref DataSet.Ensemble ensemble, float corrThresh = 0.90f, float snrThresh = 10.0f, bool isUseGpsHeading = false, float headingOffset = 0.0f, float shipXdcrOffset = 0.0f)
        {
            if (ensemble.IsInstrumentWaterMassAvail)
            {
                // If the data is bad, we cannot transform the data
                if (ensemble.InstrumentWaterMassData.VelocityX == DataSet.Ensemble.BAD_VELOCITY || 
                    ensemble.InstrumentWaterMassData.VelocityY == DataSet.Ensemble.BAD_VELOCITY || 
                    ensemble.InstrumentWaterMassData.VelocityZ== DataSet.Ensemble.BAD_VELOCITY)
                {
                    return;
                }
            }
            else
            {
                // We need Water Mass Instrument to get the X, Y, Z values, since Beam does not exist
                return;
            }

            float pitch = 0.0f;
            float roll = 0.0f;
            float compassHeading = 0.0f;
            if(ensemble.IsAncillaryAvail)
            {
                pitch = ensemble.AncillaryData.Pitch;
                roll = ensemble.AncillaryData.Roll;
                compassHeading = ensemble.AncillaryData.Heading;
            }

            // Replace the heading value GPS HDT if flag set
            if(isUseGpsHeading)
            {
                if(ensemble.IsNmeaAvail && ensemble.NmeaData.IsGphdtAvail())
                {
                    compassHeading = (float)ensemble.NmeaData.GPHDT.Heading.DecimalDegrees;
                }
            }

            // Set the pitch and roll
            // From a compass or gyro
            float Pitch = pitch;
            float Roll = roll;
            float Heading = compassHeading;
            float ShipXdcrOffset = shipXdcrOffset;

            // Add the heading offset
            Heading += headingOffset;

            //// Subsystem selection
            //switch (ensemble.EnsembleData.SubsystemConfig.SubSystem.Code)
            //{
            //    default:
            //        break;
            //    case Subsystem.SUB_1_2MHZ_4BEAM_20DEG_PISTON_OPPOSITE_FACING_c:
            //    case Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_OPPOSITE_FACING_d:
            //    case Subsystem.SUB_300KHZ_4BEAM_20DEG_PISTON_OPPOSITE_FACING_e:
            //        if (Roll < 0)
            //        {
            //            Roll = 180.0f + Roll;
            //        }
            //        else
            //        {
            //            Roll = Roll - 180.0f;
            //        }
            //        break;
            //}

            // Calculate new Pitch and Roll for Beam0 offset from compass bin mapping
            //
            // A = new angle +45 degrees for second piston system
            // P = Pitch
            // R = Roll
            //
            // P' = PcosA - RsinA
            // R' = PsinA + RcosA
            float P, R;

            float Bm0CosHeading = 1.0f;      // For a XDCR that is forward facing.  If it is 45 degrees offset, you would do COS of 45 degrees.  This if found based off the subsystem type  
            float Bm0SinHeading = 0.0f;      // For a XDCR that is forward facing.  If it is 45 degrees offset, you would do SIN of 45 degrees.  This if found based off the subsystem type 

            if (Roll >= 90.0f || Roll <= -90.0f) // Down facing case
            {
                float R1;
                float B1;
                if (Roll > 90.0f)
                {
                    B1 = -180.0f;
                }
                else
                {
                    B1 = +180.0f;
                }

                R1 = Roll + B1;

                P = Pitch * Bm0CosHeading - R1 * Bm0SinHeading;
                R = Pitch * Bm0SinHeading + R1 * Bm0CosHeading - B1;
            }
            else // Up Facing case
            {
                P = Pitch * Bm0CosHeading + Roll * Bm0SinHeading;
                R = -Pitch * Bm0SinHeading + Roll * Bm0CosHeading;
            }

            double SP = Math.Sin(Math.PI * P / 180.0);
            double CP = Math.Cos(Math.PI * P / 180.0);
            double SR = Math.Sin(Math.PI * R / 180.0);
            double CR = Math.Cos(Math.PI * R / 180.0);
            double SH = Math.Sin(Math.PI * Heading / 180.0);                // Earth Coordinate Compass Heading
            double CH = Math.Cos(Math.PI * Heading / 180.0);                // Earth Coordinate Compass Heading
            double SSH = Math.Sin(Math.PI * shipXdcrOffset / 180.0);        // Ship Coordinate Offset
            double SCH = Math.Cos(Math.PI * shipXdcrOffset / 180.0);        // Ship Coordinate Offset

            // Check how many beams were good
            // If there were at least 3 good beams, we can do a 3 beam solution

            // Values used to calculate nominal beam angle
            float beamAngle = 20.0f;                                        // This value should be found based off subsystem type
            float[,] M = new float[4, 4];
            float s = (float)Math.Sin(beamAngle / 180.0 * Math.PI);         // SIN of the beam angle
            float c = (float)Math.Cos(beamAngle / 180.0 * Math.PI);         // COS of the beam angle
            //X
            M[0, 0] = -1 / (2 * s);
            M[0, 1] = 1 / (2 * s);
            M[0, 2] = 0;
            M[0, 3] = 0;
            //Y
            M[1, 0] = 0;
            M[1, 1] = 0;
            M[1, 2] = -1 / (2 * s);
            M[1, 3] = 1 / (2 * s);
            //Z
            M[2, 0] = -1 / (4 * c);
            M[2, 1] = -1 / (4 * c);
            M[2, 2] = -1 / (4 * c);
            M[2, 3] = -1 / (4 * c);
            //Q
            M[3, 0] = (float)0.25;
            M[3, 1] = (float)0.25;
            M[3, 2] = (float)-0.25;
            M[3, 3] = (float)-0.25;

            // DO I NEED TO MAKE THIS NEGATIVE???
            float X = ensemble.InstrumentWaterMassData.VelocityX;
            float Y = ensemble.InstrumentWaterMassData.VelocityY;

            // Rotate Axis to align beam 0 with compass
            float X1 = X;
            float Y1 = Y;
            X = X1 * Bm0CosHeading - Y1 * Bm0SinHeading;
            Y = X1 * Bm0SinHeading + Y1 * Bm0CosHeading;

            // If doppler array the vertical angle uses current speed of sound
            float Z = ensemble.InstrumentWaterMassData.VelocityZ;

            // Ship Coordinate Transform
            float[] shipVelocity = new float[3];
            // Transverse (+ = Port Stbd ship movement rel. to water mass)
            shipVelocity[0] = (float)(
                                        X * (SSH * CP)
                                        - Y * (SCH * CR + SSH * SR * SP)
                                        + Z * (SCH * SR - SSH * CR * SP));

            // Longitudinal (+ = Aft Fwd ship movement rel. to water mass)
            shipVelocity[1] = (float)(
                                        X * (SCH * CP)
                                        + Y * (SSH * CR - SCH * SR * SP)
                                        - Z * (SSH * SR + SCH * SP * CR));

            // Normal (+ = ship movement away from water mass)
            shipVelocity[2] = (float)(
                                        X * (SP)
                                        + Y * (SR * CP)
                                        + Z * (CP * CR));

            // Add Ship Water Mass if it is not already in the ensemble
            if(!ensemble.IsShipWaterMassAvail)
            {
                EnsembleHelper.AddWaterMassShip(ref ensemble);
            }
            ensemble.ShipWaterMassData.VelocityTransverse = shipVelocity[0];
            ensemble.ShipWaterMassData.VelocityLongitudinal = shipVelocity[1];
            ensemble.ShipWaterMassData.VelocityNormal = shipVelocity[2];


            // Earth Coordinate Transform
            float[] earthVelocity = new float[4];
            // East
            earthVelocity[DataSet.Ensemble.BEAM_EAST_INDEX] = (float)(
                                                                X * (SH * CP)
                                                                - Y * (CH * CR + SH * SR * SP)
                                                                + Z * (CH * SR - SH * CR * SP));

            // North
            earthVelocity[DataSet.Ensemble.BEAM_NORTH_INDEX] = (float)(
                                                                X * (CH * CP)
                                                                + Y * (SH * CR - CH * SR * SP)
                                                                - Z * (SH * SR + CH * SP * CR));

            // Up
            earthVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX] = (float)(
                                                                X * (SP)
                                                                + Y * (SR * CP)
                                                                + Z * (CP * CR));
            if(!ensemble.IsEarthWaterMassAvail)
            {
                EnsembleHelper.AddWaterMassEarth(ref ensemble);
            }
            ensemble.EarthWaterMassData.VelocityEast = earthVelocity[DataSet.Ensemble.BEAM_EAST_INDEX];
            ensemble.EarthWaterMassData.VelocityNorth = earthVelocity[DataSet.Ensemble.BEAM_NORTH_INDEX];
            ensemble.EarthWaterMassData.VelocityVertical = earthVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX];


            return;
        }

        #endregion

        #region Water Mass Transform

        /// <summary>
        /// Results from the transformation of the Water Mass data.
        /// </summary>
        public class WaterMassTransformResults
        {
            /// <summary>
            /// Ship velocity results
            /// </summary>
            public float[] ShipVelocity { get; set; }

            /// <summary>
            /// Earth Velocity results.
            /// </summary>
            public float[] EarthVelocity { get; set; }

            /// <summary>
            /// Initialize the values.
            /// </summary>
            public WaterMassTransformResults()
            {
                this.ShipVelocity = new float[4];
                this.EarthVelocity = new float[4];

                for(int x = 0; x < 4; x++)
                {
                    this.ShipVelocity[x] = DataSet.Ensemble.BAD_VELOCITY;
                    this.EarthVelocity[x] = DataSet.Ensemble.BAD_VELOCITY;
                }

            }
        }

        /// <summary>
        /// Convert the Water Mass Instrument Data to Earth and Ship coordinate transform.
        /// 
        /// If the isUseGpsHeading is set to true it will use the GPS heading if it exist.  If there is no GPS data, or no GPS heading,
        /// fall back to the ancillary heading data.
        /// 
        /// Add the heading offset to the heading value before using it to retransform the data.
        /// 
        /// Correlation Thresholds Default
        /// BB = 0.90f
        /// 
        /// SNR Thresold Default
        /// BB = 10.0f
        /// </summary>
        /// <param name="compassHeading">Compass heading.</param>
        /// <param name="corrThresh">Correlation Threshold.</param>
        /// <param name="snrThresh">SNR Threshold.</param>
        /// <param name="headingOffset">Heading offset to add to the heading.</param>
        /// <param name="shipXdcrOffset">The calcuate ship coordinate transform, i need to know the angle offset between Beam 0 and the front of the boat.</param>
        /// <param name="wmVel_X">Water Mass Insturment X Velocity in m/s.</param>
        /// <param name="wmVel_Y">Water Mass Insturment Y Velocity in m/s.</param>
        /// <param name="wmVel_Z">Water Mass Insturment Z Velocity in m/s.</param>
        /// <param name="pitch">Pitch value in degrees.</param>
        /// <param name="roll">Roll valuein degrees.</param>
        public static WaterMassTransformResults WaterMassTransform(float compassHeading, float shipXdcrOffset, float pitch, float roll, float wmVel_X, float wmVel_Y, float wmVel_Z, float corrThresh = 0.90f, float snrThresh = 10.0f, float headingOffset = 0.0f)
        {
            // If the data is bad, we cannot transform the data
            if (wmVel_X == DataSet.Ensemble.BAD_VELOCITY || wmVel_Y == DataSet.Ensemble.BAD_VELOCITY || wmVel_Z == DataSet.Ensemble.BAD_VELOCITY)
            {
                return new WaterMassTransformResults();
            }

            // Set the pitch and roll
            // From a compass or gyro
            float Pitch = pitch;
            float Roll = roll;
            float Heading = compassHeading;
            float ShipXdcrOffset = shipXdcrOffset;
            WaterMassTransformResults results = new WaterMassTransformResults();

            // Add the heading offset
            Heading += headingOffset;

            //// Subsystem selection
            //switch (ensemble.EnsembleData.SubsystemConfig.SubSystem.Code)
            //{
            //    default:
            //        break;
            //    case Subsystem.SUB_1_2MHZ_4BEAM_20DEG_PISTON_OPPOSITE_FACING_c:
            //    case Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_OPPOSITE_FACING_d:
            //    case Subsystem.SUB_300KHZ_4BEAM_20DEG_PISTON_OPPOSITE_FACING_e:
            //        if (Roll < 0)
            //        {
            //            Roll = 180.0f + Roll;
            //        }
            //        else
            //        {
            //            Roll = Roll - 180.0f;
            //        }
            //        break;
            //}

            // Calculate new Pitch and Roll for Beam0 offset from compass bin mapping
            //
            // A = new angle +45 degrees for second piston system
            // P = Pitch
            // R = Roll
            //
            // P' = PcosA - RsinA
            // R' = PsinA + RcosA
            float P, R;

            float Bm0CosHeading = 1.0f;      // For a XDCR that is forward facing.  If it is 45 degrees offset, you would do COS of 45 degrees.  This if found based off the subsystem type  
            float Bm0SinHeading = 0.0f;      // For a XDCR that is forward facing.  If it is 45 degrees offset, you would do SIN of 45 degrees.  This if found based off the subsystem type 

            if (Roll >= 90.0f || Roll <= -90.0f) // Down facing case
            {
                float R1;
                float B1;
                if (Roll > 90.0f)
                {
                    B1 = -180.0f;
                }
                else
                {
                    B1 = +180.0f;
                }

                R1 = Roll + B1;

                P = Pitch * Bm0CosHeading - R1 * Bm0SinHeading;
                R = Pitch * Bm0SinHeading + R1 * Bm0CosHeading - B1;
            }
            else // Up Facing case
            {
                P = Pitch * Bm0CosHeading + Roll * Bm0SinHeading;
                R = -Pitch * Bm0SinHeading + Roll * Bm0CosHeading;
            }

            double SP = Math.Sin(Math.PI * P / 180.0);
            double CP = Math.Cos(Math.PI * P / 180.0);
            double SR = Math.Sin(Math.PI * R / 180.0);
            double CR = Math.Cos(Math.PI * R / 180.0);
            double SH = Math.Sin(Math.PI * Heading / 180.0);                // Earth Coordinate Compass Heading
            double CH = Math.Cos(Math.PI * Heading / 180.0);                // Earth Coordinate Compass Heading
            double SSH = Math.Sin(Math.PI * shipXdcrOffset / 180.0);        // Ship Coordinate Offset
            double SCH = Math.Cos(Math.PI * shipXdcrOffset / 180.0);        // Ship Coordinate Offset

            // Check how many beams were good
            // If there were at least 3 good beams, we can do a 3 beam solution

            // Values used to calculate nominal beam angle
            float beamAngle = 20.0f;                                        // This value should be found based off subsystem type
            float[,] M = new float[4, 4];
            float s = (float)Math.Sin(beamAngle / 180.0 * Math.PI);         // SIN of the beam angle
            float c = (float)Math.Cos(beamAngle / 180.0 * Math.PI);         // COS of the beam angle
            //X
            M[0, 0] = -1 / (2 * s);
            M[0, 1] = 1 / (2 * s);
            M[0, 2] = 0;
            M[0, 3] = 0;
            //Y
            M[1, 0] = 0;
            M[1, 1] = 0;
            M[1, 2] = -1 / (2 * s);
            M[1, 3] = 1 / (2 * s);
            //Z
            M[2, 0] = -1 / (4 * c);
            M[2, 1] = -1 / (4 * c);
            M[2, 2] = -1 / (4 * c);
            M[2, 3] = -1 / (4 * c);
            //Q
            M[3, 0] = (float)0.25;
            M[3, 1] = (float)0.25;
            M[3, 2] = (float)-0.25;
            M[3, 3] = (float)-0.25;

            // DO I NEED TO MAKE THIS NEGATIVE???
            float X = wmVel_X;
            float Y = wmVel_Y;

            // Rotate Axis to align beam 0 with compass
            float X1 = X;
            float Y1 = Y;
            X = X1 * Bm0CosHeading - Y1 * Bm0SinHeading;
            Y = X1 * Bm0SinHeading + Y1 * Bm0CosHeading;

            // If doppler array the vertical angle uses current speed of sound
            float Z = wmVel_Z;


            // Ship Coordinate Transform
            // Transverse (+ = Port Stbd ship movement rel. to water mass)
            results.ShipVelocity[0] = (float)(
                                        X * (SSH * CP)
                                        - Y * (SCH * CR + SSH * SR * SP)
                                        + Z * (SCH * SR - SSH * CR * SP));

            // Longitudinal (+ = Aft Fwd ship movement rel. to water mass)
            results.ShipVelocity[1] = (float)(
                                        X * (SCH * CP)
                                        + Y * (SSH * CR - SCH * SR * SP)
                                        - Z * (SSH * SR + SCH * SP * CR));

            // Normal (+ = ship movement away from water mass)
            results.ShipVelocity[2] = (float)(
                                        X * (SP)
                                        + Y * (SR * CP)
                                        + Z * (CP * CR));

            // There is no beam data to calculate the error
            results.ShipVelocity[3] = 0.0f;

            // Earth Coordinate Transform
            // East
            results.EarthVelocity[DataSet.Ensemble.BEAM_EAST_INDEX] = (float)(
                                                                X * (SH * CP)
                                                                - Y * (CH * CR + SH * SR * SP)
                                                                + Z * (CH * SR - SH * CR * SP));

            // North
            results.EarthVelocity[DataSet.Ensemble.BEAM_NORTH_INDEX] = (float)(
                                                                X * (CH * CP)
                                                                + Y * (SH * CR - CH * SR * SP)
                                                                - Z * (SH * SR + CH * SP * CR));

            // Up
            results.EarthVelocity[DataSet.Ensemble.BEAM_VERTICAL_INDEX] = (float)(
                                                                X * (SP)
                                                                + Y * (SR * CP)
                                                                + Z * (CP * CR));
            // There is no beam data to calculate the error
            results.EarthVelocity[3] = 0.0f;

            return results;
        }

        #endregion


    }
}
