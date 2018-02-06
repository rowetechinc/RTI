using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI
{
    /// <summary>
    /// Convert the ensemble data to VmDas ASCII output.
    /// </summary>
    public class VmDasAsciiCodec
    {
        #region Enum and Class

        /// <summary>
        /// Bin selection values.
        /// </summary>
        public class BinSelection
        {
            /// <summary>
            /// Minimum Bin Selection.
            /// </summary>
            public int MinBin { get; set; }

            /// <summary>
            /// Maximum Bin Selection.
            /// </summary>
            public int MaxBin { get; set; }

            /// <summary>
            /// Number of bins selected.
            /// </summary>
            public int NumBinsSelected { get; set; }

            /// <summary>
            /// Inititialize the values.
            /// </summary>
            public BinSelection()
            {
                MinBin = 0;
                MaxBin = 3;
                NumBinsSelected = 4;
            }

            /// <summary>
            /// Take the min and max selection.
            /// </summary>
            /// <param name="min">Minimum bin.</param>
            /// <param name="max">Maximum bin.</param>
            public BinSelection(int min, int max)
            {
                MinBin = min;
                MaxBin = max;
                NumBinsSelected = MaxBin - MinBin;
            }
        }

        /// <summary>
        /// VmDas ASCII output and Bin selection.
        /// </summary>
        public struct VmDasAsciiOutput
        {
            /// <summary>
            /// ASCII Output.
            /// </summary>
            public string Ascii { get; set; }

            /// <summary>
            /// Bin selections.
            /// </summary>
            public BinSelection BinSelected { get; set; }
        }

        #endregion

        #region Variables

        /// <summary>
        /// Previous Bottom Track East velocity.
        /// Used to remove the ship speed.
        /// </summary>
        private float _prevBtEastVel = DataSet.Ensemble.BAD_VELOCITY;

        /// <summary>
        /// Previous Bottom Track East velocity.
        /// Used to remove the ship speed.
        /// </summary>
        private float _prevBtNorthVel = DataSet.Ensemble.BAD_VELOCITY;

        /// <summary>
        /// Previous Bottom Track East velocity.
        /// Used to remove the ship speed.
        /// </summary>
        private float _prevBtVertVel = DataSet.Ensemble.BAD_VELOCITY;

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public VmDasAsciiCodec()
        {

        }


        /// <summary>
        /// Encode the data into the ASCII output.
        /// </summary>
        /// <param name="ens">Ensemble data.</param>
        /// <param name="minBin">Minimum bin selected.</param>
        /// <param name="maxBin">Maximum bin selected.</param>
        /// <returns>The bin selection and ASCII output.</returns>
        public VmDasAsciiOutput Encode(DataSet.Ensemble ens, int minBin, int maxBin)
        {
            // Get the bin selection
            BinSelection bs = GetBinSelection(ens, minBin, maxBin);

            VmDasAsciiOutput output = new VmDasAsciiOutput();
            output.BinSelected = bs;
            output.Ascii = EncodeData(ens, bs);

            return output;
        }

        /// <summary>
        /// Get the Bin Selection.  This will verify the selections are correct.
        /// </summary>
        /// <param name="ens">Ensemble to get the number of bins.</param>
        /// <param name="minBin">Mininum Bin selected.</param>
        /// <param name="maxBin">Maximum bin selected.</param>
        /// <returns>Bin Selections.</returns>
        private BinSelection GetBinSelection(DataSet.Ensemble ens, int minBin, int maxBin)
        {
            BinSelection bs = new BinSelection(minBin, maxBin);

            // Verify min and max bin is possible
            if (minBin > maxBin)
            {
                minBin = maxBin - 1;
                if (minBin < 0)
                {
                    minBin = 1;
                    maxBin = 2;
                }
            }

            if (ens.IsEnsembleAvail)
            {
                if (maxBin > ens.EnsembleData.NumBins)
                {
                    maxBin = ens.EnsembleData.NumBins;
                }
            }

            bs.MinBin = minBin;
            bs.MaxBin = maxBin;
            bs.NumBinsSelected =(maxBin - minBin) + 1;

            return bs;
        }

        /// <summary>
        /// Encode the data into the VmDas format.
        /// </summary>
        /// <param name="ens"></param>
        /// <param name="bs">Bin selection.</param>
        private string EncodeData(DataSet.Ensemble ens, BinSelection bs)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('\u0002');                                        // STX
            sb.Append(" 0");                                            // 0 Header
            sb.Append("\t" + ens.EnsembleData.EnsembleNumber);          // Ensemble number
            sb.Append(" " + (ens.EnsembleData.Year-2000));              // Year
            sb.Append(" " + ens.EnsembleData.Month);                    // Month
            sb.Append(" " + ens.EnsembleData.Day);                      // Day
            sb.Append(" " + ens.EnsembleData.Hour);                     // Hour
            sb.Append(" " + ens.EnsembleData.Minute);                   // Minute
            sb.Append(" " + ens.EnsembleData.Second);                   // Second
            sb.Append("\r\n");
            sb.Append(EncodeEnuData(ens, bs));                          // 1 Header (ENU)
            sb.Append(EncodeCorrData(ens, bs));                         // 2 Header (Corr)
            sb.Append(EncodeAmpData(ens, bs));                          // 3 Header (Amp)
            sb.Append(EncodePgData(ens, bs));                           // 4 Header (PG)
            sb.Append(EncodeStatusData(ens, bs));                       // 5 Header (Status)
            sb.Append(EncodeLeaderData(ens));                           // 6 Header (Leader)
            sb.Append(EncodeBtData(ens));                               // 7 Header (BT)
            sb.Append(EncodeNavData(ens));                              // 8 Header (NAV)

            sb.Append('\u0003');                                        // ETX

            return sb.ToString();
        }

        /// <summary>
        /// Decode the Earth velocity data.
        /// </summary>
        /// <param name="ens">Ensemble data.</param>
        /// <param name="bs">Bin Selection</param>
        /// <returns>Strings for the Earth Velocity data.</returns>
        private string EncodeEnuData(DataSet.Ensemble ens, BinSelection bs)
        {
            if (ens.IsEarthVelocityAvail)
            {
                if (ens.IsBottomTrackAvail)
                {
                    // Remove the ship speed
                    RTI.ScreenData.RemoveShipSpeed.RemoveVelocity(ref ens, _prevBtEastVel, _prevBtNorthVel, _prevBtVertVel, true, true);

                    // Store previous bottom track values
                    if(ens.BottomTrackData.IsEarthVelocityGood())
                    {
                        _prevBtEastVel = ens.BottomTrackData.EarthVelocity[0];
                        _prevBtNorthVel = ens.BottomTrackData.EarthVelocity[1];
                        _prevBtVertVel = ens.BottomTrackData.EarthVelocity[2];
                    }
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("1");                                         // 1 Header
                sb.Append("\t" + bs.NumBinsSelected.ToString());        // Number of bins selected
                sb.Append(" " + ens.EnsembleData.NumBeams.ToString());  // Number of beams
                sb.Append(" " + bs.MinBin.ToString());                  // Minimum Bin
                sb.Append(" " + bs.MaxBin.ToString());                  // Maximum bin
                sb.Append(" 0 0 0 0");                                  // Reference Layers 
                sb.Append("\r\n");
                for (int bin = bs.MinBin-1; bin < bs.NumBinsSelected; bin++)
                {
                    // Check for bad values and convert from m/s to mm/s
                    sb.Append("\t");
                    
                    // East
                    if (ens.EnsembleData.NumBeams >= 1)
                    {
                        long east = -32768;
                        if (ens.EarthVelocityData.EarthVelocityData[bin, 0] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            east = (long)Math.Round(ens.EarthVelocityData.EarthVelocityData[bin, 0]) * 1000;
                        }

                        sb.Append(" " + east);      // East
                    }

                    // North
                    if (ens.EnsembleData.NumBeams >= 2)
                    {
                        long north = -32768;
                        if (ens.EarthVelocityData.EarthVelocityData[bin, 1] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            north = (long)Math.Round(ens.EarthVelocityData.EarthVelocityData[bin, 1]) * 1000;
                        }

                        sb.Append(" " + north);     // North
                    }

                    // Vertical
                    if (ens.EnsembleData.NumBeams >= 3)
                    {
                        long vert = -32768;
                        if (ens.EarthVelocityData.EarthVelocityData[bin, 2] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            vert = (long)Math.Round(ens.EarthVelocityData.EarthVelocityData[bin, 2]) * 1000;
                        }

                        sb.Append(" " + vert);      // Vertical
                    }

                    // Error
                    if (ens.EnsembleData.NumBeams >= 4)
                    {
                        long error = -32768;
                        if (ens.EarthVelocityData.EarthVelocityData[bin, 3] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            error = (long)Math.Round(ens.EarthVelocityData.EarthVelocityData[bin, 3]) * 1000;
                        }

                        sb.Append(" " + error);     // Error
                    }
                    sb.Append("\r\n");
                }
                return sb.ToString();
            }

            return "";
        }

        /// <summary>
        /// Encode the Correlation data.
        /// </summary>
        /// <param name="ens">Ensemble.</param>
        /// <param name="bs">Bin Selection</param>
        /// <returns>String for Correlation data.</returns>
        private string EncodeCorrData(DataSet.Ensemble ens, BinSelection bs)
        {
            if (ens.IsCorrelationAvail)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("2");                                         // 2 Header
                sb.Append("\t" + bs.NumBinsSelected.ToString());        // Number of bins selected
                sb.Append(" " + ens.EnsembleData.NumBeams.ToString());  // Number of beams
                sb.Append(" " + bs.MinBin.ToString());                  // Minimum Bin
                sb.Append(" " + bs.MaxBin.ToString());                  // Maximum bin
                sb.Append("\r\n");
                for (int bin = bs.MinBin-1; bin < bs.NumBinsSelected; bin++)
                {
                    sb.Append("\t");
                    for (int beam = 0; beam < ens.EnsembleData.NumBeams; beam++)
                    {
                        sb.Append(" " + (ens.CorrelationData.CorrelationData[bin, beam] * 100).ToString("0"));       // Corr
                    }
                    sb.Append("\r\n");
                }
                return sb.ToString();
            }

            return "";
        }

        /// <summary>
        /// Encode the Amplitude data.
        /// </summary>
        /// <param name="ens">Ensemble.</param>
        /// <param name="bs">Bin Selection.</param>
        /// <returns>String for Amplitude data.</returns>
        private string EncodeAmpData(DataSet.Ensemble ens, BinSelection bs)
        {
            if (ens.IsAmplitudeAvail)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("3");                                         // 3 Header
                sb.Append("\t" + bs.NumBinsSelected.ToString());        // Number of bins selected
                sb.Append(" " + ens.EnsembleData.NumBeams.ToString());  // Number of beams
                sb.Append(" " + bs.MinBin.ToString());                  // Minimum Bin
                sb.Append(" " + bs.MaxBin.ToString());                  // Maximum bin
                sb.Append("\r\n");
                for (int bin = bs.MinBin-1; bin < bs.NumBinsSelected; bin++)
                {
                    sb.Append("\t");
                    for (int beam = 0; beam < ens.EnsembleData.NumBeams; beam++)
                    {
                        sb.Append(" " + (ens.AmplitudeData.AmplitudeData[bin, beam]).ToString("0"));       // Amp
                    }
                    sb.Append("\r\n");
                }
                return sb.ToString();
            }

            return "";
        }

        /// <summary>
        /// Encode the Percent Good data.
        /// </summary>
        /// <param name="ens">Ensemble.</param>
        /// <param name="bs">Bin Selection.</param>
        /// <returns>String for Amplitude data.</returns>
        private string EncodePgData(DataSet.Ensemble ens, BinSelection bs)
        {
            if (ens.IsEnsembleAvail)
            {
                int pingCount = ens.EnsembleData.ActualPingCount;

                StringBuilder sb = new StringBuilder();
                sb.Append("4");                                         // 4 Header
                sb.Append("\t" + bs.NumBinsSelected.ToString());        // Number of bins selected
                sb.Append(" " + ens.EnsembleData.NumBeams.ToString());  // Number of beams
                sb.Append(" " + bs.MinBin.ToString());                  // Minimum Bin
                sb.Append(" " + bs.MaxBin.ToString());                  // Maximum bin
                sb.Append("\r\n");
                for (int bin = bs.MinBin-1; bin < bs.NumBinsSelected; bin++)
                {
                    sb.Append("\t");
                    for (int beam = 0; beam < ens.EnsembleData.NumBeams; beam++)
                    {
                        sb.Append(" " + ((ens.GoodEarthData.GoodEarthData[bin, beam] / pingCount) * 100).ToString("0"));       // PG
                    }
                    sb.Append("\r\n");
                }
                return sb.ToString();
            }

            return "";
        }

        /// <summary>
        /// Encode the STA and LTA Status data.
        /// </summary>
        /// <param name="ens">Ensemble.</param>
        /// <param name="bs">Bin Selection.</param>
        /// <returns>String for STA and LTA Status data.</returns>
        private string EncodeStatusData(DataSet.Ensemble ens, BinSelection bs)
        {
            if (ens.IsEnsembleAvail)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("5");                                         // 5 Header
                sb.Append("\t" + bs.NumBinsSelected.ToString());        // Number of bins selected
                sb.Append(" " + ens.EnsembleData.NumBeams.ToString());  // Number of beams
                sb.Append(" " + bs.MinBin.ToString());                  // Minimum Bin
                sb.Append(" " + bs.MaxBin.ToString());                  // Maximum bin
                sb.Append("\r\n");
                for (int bin = bs.MinBin-1; bin < bs.NumBinsSelected; bin++)
                {
                    sb.Append("\t");
                    for (int beam = 0; beam < ens.EnsembleData.NumBeams; beam++ )
                    {
                        sb.Append(" " + -32768);       // Status
                    }
                    sb.Append("\r\n");
                }
                return sb.ToString();
            }

            return "";
        }

        /// <summary>
        /// Encode the Leader data.
        /// </summary>
        /// <param name="ens">Ensemble.</param>
        /// <returns>String for Leader data.</returns>
        private string EncodeLeaderData(DataSet.Ensemble ens)
        {
            if (ens.IsEnsembleAvail && ens.IsAncillaryAvail)
            {
                double tbp = ens.AncillaryData.LastPingTime - ens.AncillaryData.FirstPingTime;
                int tbp_minute = (int)Math.Round(tbp / 60);
                int tbp_sec = (int)Math.Truncate(tbp - (tbp_minute * 60));
                int tbp_hun = (int)Math.Round((tbp - tbp_sec) * 100);
                int pingCount = ens.EnsembleData.ActualPingCount;                           // Ping Count
                int numBins = ens.EnsembleData.NumBins;                                     // Number bins
                int binSize = (int)Math.Round(ens.AncillaryData.BinSize * 100);             // Bin Size (cm)
                int blank = (int)Math.Round(ens.AncillaryData.FirstBinRange * 100);         // Blank (cm)
                int waterMode = 11;                                                         // WM11 High Resolution (Not Used)
                int status = ens.EnsembleData.Status.Value;                                 // Status Value
                int sensorSource = 0;                                                       // Sensor Source (EZ)
                int availSensor = 0;                                                        // Available Sensors (PS)
                int corrThresh = 25;                                                        // Correlation Threshold (WC)
                int errThresh = 0;                                                          // Error Threshould (WE)
                int pgMin = 0;                                                              // Percent Good Minimum (WG)
                int avgPitch = 0;
                int avgRoll = 0;
                int avgHdg = 0;
                int temp = (int)Math.Round(ens.AncillaryData.WaterTemp * 100);
                int stdHdg = 0;
                int stdPitch = 0;
                int stdRoll = 0;
                int salinity = (int)Math.Round(ens.AncillaryData.Salinity);
                int sos = (int)Math.Round(ens.AncillaryData.SpeedOfSound);


                StringBuilder sb = new StringBuilder();
                sb.Append("6");                                         // 6 Header
                sb.Append("\t" + tbp_minute.ToString());                // Time Between Pings Minutes
                sb.Append(" " + tbp_sec.ToString());                    // Time Between Pings Seconds
                sb.Append(" " + tbp_hun.ToString());                    // Time Between Pings Hundredth Seconds
                sb.Append(" " + pingCount.ToString());                  // Ping Counts
                sb.Append(" " + numBins.ToString());                    // Number of bins
                sb.Append(" " + binSize.ToString());                    // Bin Size in cm
                sb.Append(" " + blank.ToString());                      // Blank in cm
                sb.Append(" " + waterMode.ToString());                  // Water Mode
                sb.Append(" " + status.ToString());                     // Status Built-In Test
                sb.Append(" " + sensorSource.ToString());               // Sensor Source
                sb.Append(" " + availSensor.ToString());                // Available Sensors
                sb.Append(" " + corrThresh.ToString());                 // Correlation Threshold
                sb.Append(" " + errThresh.ToString());                  // Error Threshold
                sb.Append(" " + pgMin.ToString());                      // Percent Good Minimum
                sb.Append(" " + avgPitch.ToString());                   // Avg Pitch
                sb.Append(" " + avgRoll.ToString());                    // Avg Roll
                sb.Append(" " + avgHdg.ToString());                     // Avg Heading
                sb.Append(" " + temp.ToString());                       // Temperature
                sb.Append(" " + stdHdg.ToString());                     // STD Heading
                sb.Append(" " + stdPitch.ToString());                   // STD Pitch
                sb.Append(" " + stdRoll.ToString());                    // STD Roll
                sb.Append(" " + salinity.ToString());                   // Salinity
                sb.Append(" " + sos.ToString());                        // Speed of Sound
                sb.Append("\r\n");
                return sb.ToString();
            }

            return "";
        }

        /// <summary>
        /// Decode the Bottom Track data.
        /// </summary>
        /// <param name="ens">Ensemble data.</param>
        /// <returns>Strings for the Bottom Track data.</returns>
        private string EncodeBtData(DataSet.Ensemble ens)
        {
            if (ens.IsBottomTrackAvail)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("7");                                         // 7 Header
                sb.Append("\t2");                                       // Number of rows
                sb.Append(" " + ens.EnsembleData.NumBeams.ToString());  // Number of beams
                sb.Append("\r\n");

                // -------------------------------------------------
                // Velocity
                // Check for bad values and convert from m/s to mm/s
                sb.Append("\t");

                // East
                if (ens.EnsembleData.NumBeams >= 1)
                {
                    long east = -32768;
                    if (ens.BottomTrackData.EarthVelocity[0] != DataSet.Ensemble.BAD_VELOCITY)
                    {
                        east = (long)Math.Round(ens.BottomTrackData.EarthVelocity[0]) * 1000;
                    }

                    sb.Append(" " + east);      // East
                }

                // North
                if (ens.EnsembleData.NumBeams >= 2)
                {
                    long north = -32768;
                    if (ens.BottomTrackData.EarthVelocity[1] != DataSet.Ensemble.BAD_VELOCITY)
                    {
                        north = (long)Math.Round(ens.BottomTrackData.EarthVelocity[1]) * 1000;
                    }

                    sb.Append(" " + north);     // North
                }

                // Vertical
                if (ens.EnsembleData.NumBeams >= 3)
                {
                    long vert = -32768;
                    if (ens.BottomTrackData.EarthVelocity[2] != DataSet.Ensemble.BAD_VELOCITY)
                    {
                        vert = (long)Math.Round(ens.BottomTrackData.EarthVelocity[2]) * 1000;
                    }

                    sb.Append(" " + vert);      // Vertical
                }

                // Error
                if (ens.EnsembleData.NumBeams >= 4)
                {
                    long error = -32768;
                    if (ens.BottomTrackData.EarthVelocity[3] != DataSet.Ensemble.BAD_VELOCITY)
                    {
                        error = (long)Math.Round(ens.BottomTrackData.EarthVelocity[3]) * 1000;
                    }

                    sb.Append(" " + error);     // Error
                }
                sb.Append("\r\n");

                // -------------------------------------------------
                // Range
                // Check for bad values and convert from m/s to mm/s
                sb.Append("\t");

                // Range Beam 0
                if (ens.EnsembleData.NumBeams >= 1)
                {
                    long east = -32768;
                    if (ens.BottomTrackData.Range[0] != DataSet.Ensemble.BAD_RANGE)
                    {
                        east = (long)Math.Round(ens.BottomTrackData.Range[0]);
                    }

                    sb.Append(" " + east);      // East
                }

                // Range Beam 1
                if (ens.EnsembleData.NumBeams >= 2)
                {
                    long north = -32768;
                    if (ens.BottomTrackData.Range[1] != DataSet.Ensemble.BAD_RANGE)
                    {
                        north = (long)Math.Round(ens.BottomTrackData.Range[1]);
                    }

                    sb.Append(" " + north);     // North
                }

                // Range Beam 2
                if (ens.EnsembleData.NumBeams >= 3)
                {
                    long vert = -32768;
                    if (ens.BottomTrackData.Range[2] != DataSet.Ensemble.BAD_RANGE)
                    {
                        vert = (long)Math.Round(ens.BottomTrackData.Range[2]);
                    }

                    sb.Append(" " + vert);      // Vertical
                }

                // Range Beam 3
                if (ens.EnsembleData.NumBeams >= 4)
                {
                    long error = -32768;
                    if (ens.BottomTrackData.Range[3] != DataSet.Ensemble.BAD_RANGE)
                    {
                        error = (long)Math.Round(ens.BottomTrackData.Range[3]);
                    }

                    sb.Append(" " + error);     // Error
                }
                sb.Append("\r\n");

                
                return sb.ToString();
            }

            return "";
        }

        /// <summary>
        /// Encode the Nav data.
        /// </summary>
        /// <param name="ens">Ensemble.</param>
        /// <returns>String for Nav data.</returns>
        private string EncodeNavData(DataSet.Ensemble ens)
        {
            if (ens.IsNmeaAvail)
            {
                double lat_sec = 0;
                double lon_sec = 0;
                double speed = 0;
                double course = 0;

                if (ens.NmeaData.IsGpggaAvail())
                {
                    // Latitude
                    DotSpatial.Positioning.Latitude lat = ens.NmeaData.GPGGA.Position.Latitude;
                    if (!lat.IsInvalid)
                    {
                        lat_sec = (int)Math.Truncate(lat.DecimalDegrees) * 60;              // Degrees converted to minutes
                        lat_sec += (int)Math.Truncate((double)lat.Minutes);                 // Minutes convert to Minutes
                        lat_sec += lat.Seconds / 60.0;                                      // Seconds convert to minutes
                        lat_sec *= 60;                                                      // Convert to seconds
                        lat_sec *= 1000;                                                    // Convert to thousands of second
                        if (lat.Hemisphere == DotSpatial.Positioning.LatitudeHemisphere.South)
                        {
                            lat_sec *= -1;                                                  // Make negative
                        }
                    }

                    // Longitude
                    DotSpatial.Positioning.Longitude lon = ens.NmeaData.GPGGA.Position.Longitude;
                    if (!lon.IsInvalid)
                    {
                        lon_sec = (int)Math.Truncate(lon.DecimalDegrees) * 60;       // Degrees converted to minutes
                        lon_sec += (int)Math.Truncate((double)lon.Minutes);                 // Minutes convert to Minutes
                        lon_sec += lon.Seconds / 60.0;                                      // Seconds convert to minutes
                        lon_sec *= 60;                                                      // Convert to seconds
                        lon_sec *= 1000;                                                    // Convert to thousands of second
                        if (lon.Hemisphere == DotSpatial.Positioning.LongitudeHemisphere.West)
                        {
                            lon_sec *= -1;                                                  // Make negative
                        }
                    }
                }

                if (ens.NmeaData.IsGpvtgAvail())
                {
                    // Velocity
                    speed = ens.NmeaData.GPVTG.Speed.ToMetersPerSecond().Value * 1000; // Make mm/s
                }

                if (ens.NmeaData.IsGphdtAvail())
                {
                    // Course
                    course = ens.NmeaData.GPHDT.Heading.DecimalDegrees * 100;                   // Make hundreth of degree
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("8");                                                 // 8 Header
                sb.Append("\t" + ((int)Math.Round(lat_sec)).ToString());        // Latitude in thousandths of a second
                sb.Append(" " + ((int)Math.Round(lon_sec)).ToString());         // Longitude in thousandths of a second
                sb.Append(" " + ((int)Math.Round(speed)).ToString());           // Speed in mm/s
                sb.Append(" " + ((int)Math.Round(course)).ToString());           // Course in hundreth of second
                sb.Append(" 0 0");
                sb.Append("\r\n");
                return sb.ToString();
            }

            return "";
        }
    }
}
