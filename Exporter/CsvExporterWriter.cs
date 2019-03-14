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
 * 08/26/2013      RC          2.20.2     Initial coding
 * 05/01/2014      RC          2.21.4     Select which datasets to export to the CSV file.
 * 08/13/2014      RC          3.0.0      Fixed the spacing for missing datasets.
 * 10/31/2014      RC          3.0.2      Added Range Tracking dataset.
 * 02/13/2015      RC          3.0.2      Added NMEA dataset.
 * 03/18/2015      RC          3.0.3      Added GageHeight dataset.
 * 07/27/2015      RC          3.0.5      Make the number format for decimal place US so decimals are not commas.
 * 10/27/2015      RC          3.2.1      Fixed bug in EncodeCSV() if bin sizes were different between configurations.
 * 10/28/2015      RC          3.2.1      Fixed missing Range Tracking Header.  Fixed Correlation header.  Fixed Bottom Track extra ,.  Made it handle any number of beams in BT.
 * 09/28/2016      RC          3.3.2      Added export of Velocity Vectors in CSV.
 * 03/13/2019      RC          3.4.11     Fixed bug with exporting CSV data with a 3 beam system.
 * 
 */


using System.IO;
using RTI.DataSet;
using System.Text;
using System.Globalization;
using System.Diagnostics;
namespace RTI
{
    /// <summary>
    /// Export the ensemble data to CSV format.
    /// </summary>
    public class CsvExporterWriter: IExporterWriter
    {
        #region Variables

        /// <summary>
        /// Write to the file using this writer.
        /// </summary>
        private StreamWriter _writer;

        /// <summary>
        /// Export options.
        /// </summary>
        private ExportOptions _options;

        #endregion

        /// <summary>
        /// Initializes a new instance of the CsvExporterWriter class.
        /// </summary>
        public CsvExporterWriter()
        {
            _options = new ExportOptions();
        }

        /// <summary>
        /// Open the file given and add the CSV header.
        /// </summary>
        /// <param name="filePath">File path to export the file to.</param>
        /// <param name="fileName">File Name to create.</param>
        /// <param name="options">Export options.</param>
        public void Open(string filePath, string fileName, ExportOptions options)
        {
            // Create the file name with file path
            string file = filePath + fileName;

            // Options
            _options = options;

            // If the writer exist, make sure it is closed
            if (_writer != null)
            {
                _writer.Close();
            }

            _writer = new StreamWriter(file, false);

            // Write the header
            _writer.WriteLine(GetHeader(options));
        }

        /// <summary>
        /// Write the Ensemble number, Ensemble date and time, the velocity data, the pressure, heading pitch and roll
        /// to the file.
        /// </summary>
        /// <param name="ensemble">Ensemble to get the data.</param>
        /// <param name="isMultipleFiles">Set if each ensemble should be a seperate file.  Default to FALSE.</param>
        public void Write(Ensemble ensemble, bool isMultipleFiles = false)
        {
            // Encode the ensemble to a string and write it to the file
            _writer.WriteLine(EncodeCSV(ensemble, _options));
        }

        /// <summary>
        /// Close the file.
        /// </summary>
        public ExportOptions Close()
        {
            if (_writer != null)
            {
                _writer.Close();
                _writer.Dispose();
                _writer = null;
            }

            return _options;
        }

        #region Header
        
        /// <summary>
        /// Get the Header for the CSV file.
        /// </summary>
        /// <param name="options">Export options.</param>
        /// <returns>CSV Header.</returns>
        public static string GetHeader(ExportOptions options)
        {
            StringBuilder sb = new StringBuilder();

            // Ensemble DataSet
            sb.Append(CreateEnsembleHeader());
            sb.Append(",");

            // Ancillary DataSet
            sb.Append(CreateAncillaryHeader());
            sb.Append(",");

            // Beam Velocity
            if (options.IsBeamVelocityDataSetOn)
            {
                sb.Append(CreateBeamVelocityHeader(options));
                sb.Append(",");
            }

            // Instrument Velocity
            if (options.IsInstrumentVelocityDataSetOn)
            {
                sb.Append(CreateInstrumentVelocityHeader(options));
                sb.Append(",");
            }

            // Earth Velocity
            if (options.IsEarthVelocityDataSetOn)
            {
                sb.Append(CreateEarthVelocityHeader(options));
                sb.Append(",");
            }

            // Velocity Vector
            if (options.IsVelocityVectorDataSetOn)
            {
                sb.Append(CreateVelocityVectorHeader(options));
                sb.Append(",");
            }

            // Amplitude DataSet
            if (options.IsAmplitudeDataSetOn)
            {
                sb.Append(CreateAmplitudeHeader(options));
                sb.Append(",");
            }

            // Correlation DataSet
            if (options.IsCorrelationDataSetOn)
            {
                sb.Append(CreateCorrelationHeader(options));
                sb.Append(",");
            }

            // Earth Water Mass DataSet
            if (options.IsEarthWaterMassDataSetOn)
            {
                sb.Append(CreateEarthWaterMassHeader());
                sb.Append(",");
            }

            // Instrument Water Mass DataSet
            if (options.IsInstrumentWaterMassDataSetOn)
            {
                sb.Append(CreateInstrumentWaterMassHeader());
                sb.Append(",");
            }

            // Good Beam DataSet
            if (options.IsGoodBeamDataSetOn)
            {
                sb.Append(CreateGoodBeamHeader(options));
                sb.Append(",");
            }

            // Good Earth DataSet
            if (options.IsGoodEarthDataSetOn)
            {
                sb.Append(CreateGoodEarthHeader(options));
                sb.Append(",");
            }

            // Bottom Track DataSet
            if (options.IsBottomTrackDataSetOn)
            {
                sb.Append(CreateBottomTrackHeader());
                sb.Append(",");
            }

            // Range Tracking DataSet
            if (options.IsRangeTrackingDataSetOn)
            {
                sb.Append(CreateRangeTrackingHeader());
                sb.Append(",");
            }

            // Gage Height DataSet
            if (options.IsGageHeightDataSetOn)
            {
                sb.Append(CreateGageHeightHeader());
                sb.Append(",");
            }

            // Bottom Track Engineering DataSet
            if (options.IsBottomTrackEngineeringDataSetOn)
            {
                sb.Append(CreateBottomTrackEngineeringHeader());
                sb.Append(",");
            }

            // Profile Engineering DataSet
            if (options.IsProfileEngineeringDataSetOn)
            {
                sb.Append(CreateProfileEngineeringHeader());
                sb.Append(",");
            }

            // System Setup DataSet
            if (options.IsSystemSetupDataSetOn)
            {
                sb.Append(CreateSystemSetupHeader());
                sb.Append(",");
            }

            // Range Tracking DataSet
            if (options.IsRangeTrackingDataSetOn)
            {
                sb.Append(CreateRangeTrackingHeader());
                sb.Append(",");
            }

            // NMEA DataSet
            if (options.IsNmeaDataSetOn)
            {
                sb.Append(CreateNmeaHeader());
                sb.Append(",");
            }

            return sb.ToString();
        }

        #endregion

        #region Encode Ensemble

        /// <summary>
        /// Encode the given ensemble in a CSV format.  This will be
        /// based off the options given.
        /// </summary>
        /// <param name="ensemble">Ensemble to encode to CSV.</param>
        /// <param name="options">Options for encoding the CSV file.</param>
        /// <returns>A string of the ensemble in CSV format.</returns>
        public static string EncodeCSV(DataSet.Ensemble ensemble, ExportOptions options)
        {
            if (ensemble != null)
            {
                StringBuilder sb = new StringBuilder();

                // Ensemble Data
                sb.Append(WriteEnsembleData(ensemble));
                sb.Append(",");

                // Ancillary Data
                sb.Append(WriteAncillaryData(ensemble));
                sb.Append(",");

                // Check values
                #region Check Values
                
                // Beam Velocity
                if (ensemble.IsBeamVelocityAvail)
                {
                    if (options.BeamMinBin >= ensemble.BeamVelocityData.BeamVelocityData.GetLength(0))
                    {
                        options.BeamMinBin = 0;
                        options.GoodBeamMinBin = 0;
                    }
                    if (options.BeamMaxBin >= ensemble.BeamVelocityData.BeamVelocityData.GetLength(0))
                    {
                        options.BeamMaxBin = ensemble.BeamVelocityData.BeamVelocityData.GetLength(0) - 1;
                        options.GoodBeamMaxBin = ensemble.BeamVelocityData.BeamVelocityData.GetLength(0) - 1;
                    }
                }

                // Instrument Velocity
                if (ensemble.IsInstrumentVelocityAvail)
                {
                    if (options.InstrumentMinBin >= ensemble.InstrumentVelocityData.InstrumentVelocityData.GetLength(0))
                    {
                        options.InstrumentMinBin = 0;
                    }
                    if (options.InstrumentMaxBin >= ensemble.InstrumentVelocityData.InstrumentVelocityData.GetLength(0))
                    {
                        options.InstrumentMaxBin = ensemble.InstrumentVelocityData.InstrumentVelocityData.GetLength(0) - 1;
                    }
                }

                // Earth Velocity
                if (ensemble.IsEarthVelocityAvail)
                {
                    if (options.EarthMinBin >= ensemble.EarthVelocityData.EarthVelocityData.GetLength(0))
                    {
                        options.EarthMinBin = 0;
                        options.GoodEarthMinBin = 0;
                    }
                    if (options.EarthMaxBin >= ensemble.EarthVelocityData.EarthVelocityData.GetLength(0))
                    {
                        options.EarthMaxBin = ensemble.EarthVelocityData.EarthVelocityData.GetLength(0) - 1;
                        options.GoodEarthMaxBin = ensemble.EarthVelocityData.EarthVelocityData.GetLength(0) - 1;
                    }
                }

                // Amplitude
                if (ensemble.IsAmplitudeAvail)
                {
                    if (options.AmplitudeMinBin >= ensemble.AmplitudeData.AmplitudeData.GetLength(0))
                    {
                        options.AmplitudeMinBin = 0;
                    }
                    if (options.AmplitudeMaxBin >= ensemble.AmplitudeData.AmplitudeData.GetLength(0))
                    {
                        options.AmplitudeMaxBin = ensemble.AmplitudeData.AmplitudeData.GetLength(0) - 1;
                    }
                }

                // Correlation
                if (ensemble.IsCorrelationAvail)
                {
                    if (options.CorrelationMinBin >= ensemble.CorrelationData.CorrelationData.GetLength(0))
                    {
                        options.CorrelationMinBin = 0;
                    }
                    if (options.CorrelationMaxBin >= ensemble.CorrelationData.CorrelationData.GetLength(0))
                    {
                        options.CorrelationMaxBin = ensemble.CorrelationData.CorrelationData.GetLength(0) - 1;
                    }
                }


                #endregion

                // Beam Velocity
                if (options.IsBeamVelocityDataSetOn)
                {
                    sb.Append(WriteBeamVelocityData(ensemble, options));
                    sb.Append(",");
                }

                // Instrument Velocity
                if (options.IsInstrumentVelocityDataSetOn)
                {
                    sb.Append(WriteInstrumentVelocityData(ensemble, options));
                    sb.Append(",");
                }

                // Earth Velocity
                if (options.IsEarthVelocityDataSetOn)
                {
                    sb.Append(WriteEarthVelocityData(ensemble, options));
                    sb.Append(",");
                }

                // Velocity Vector
                if (options.IsVelocityVectorDataSetOn)
                {
                    sb.Append(WriteVelocityVectorData(ensemble, options));
                    sb.Append(",");
                }

                // Amplitude Data
                if (options.IsAmplitudeDataSetOn)
                {
                    sb.Append(WriteAmplitudeData(ensemble, options));
                    sb.Append(",");
                }

                // Correlation Data
                if (options.IsCorrelationDataSetOn)
                {
                    sb.Append(WriteCorrelationData(ensemble, options));
                    sb.Append(",");
                }

                // Earth Water Mass Data
                if (options.IsEarthWaterMassDataSetOn)
                {
                    sb.Append(WriteEarthWaterMassData(ensemble));
                    sb.Append(",");
                }

                // Instrument Water Mass Data
                if (options.IsInstrumentWaterMassDataSetOn)
                {
                    sb.Append(WriteInstrumentWaterMassData(ensemble));
                    sb.Append(",");
                }

                // Good Beam Data
                if (options.IsGoodBeamDataSetOn)
                {
                    sb.Append(WriteGoodBeamData(ensemble, options));
                    sb.Append(",");
                }

                // Good Earth Data
                if (options.IsGoodEarthDataSetOn)
                {
                    sb.Append(WriteGoodEarthData(ensemble, options));
                    sb.Append(",");
                }

                // Bottom Track Data
                if (options.IsBottomTrackDataSetOn)
                {
                    sb.Append(WriteBottomTrackData(ensemble));
                    sb.Append(",");
                }

                // Range Tracking Data
                if (options.IsRangeTrackingDataSetOn)
                {
                    sb.Append(WriteRangeTrackingData(ensemble));
                    sb.Append(",");
                }

                // Gage Height Data
                if (options.IsGageHeightDataSetOn)
                {
                    sb.Append(WriteGageHeightData(ensemble));
                    sb.Append(",");
                }

                // Bottom Track Engineering Data
                if (options.IsBottomTrackEngineeringDataSetOn)
                {
                    sb.Append(WriteBottomTrackEngineeringData(ensemble));
                    sb.Append(",");
                }

                // Profile Engineering Data
                if (options.IsProfileEngineeringDataSetOn)
                {
                    sb.Append(WriteProfileEngineeringData(ensemble));
                    sb.Append(",");
                }

                // System Setup Data
                if (options.IsSystemSetupDataSetOn)
                {
                    sb.Append(WriteSystemSetupData(ensemble));
                    sb.Append(",");
                }

                // Range Tracking Data
                if (options.IsRangeTrackingDataSetOn)
                {
                    sb.Append(WriteRangeTrackingData(ensemble));
                    sb.Append(",");
                }

                // NMEA Data
                if (options.IsNmeaDataSetOn)
                {
                    sb.Append(WriteNmeaData(ensemble));
                    sb.Append(",");
                }

                return sb.ToString();
            }

            return string.Empty;
        }

        #endregion

        #region Ensemble DataSet

        /// <summary>
        /// Create the Ensemble header based off the options selected.
        /// </summary>
        /// <returns>Ensemble Header.</returns>
        public static string CreateEnsembleHeader()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("EnsembleNumber,");
            sb.Append("DateTime,");
            sb.Append("NumBins,");
            sb.Append("NumBeams,");
            sb.Append("DesiredPingCount,");
            sb.Append("ActualPingCount,");
            sb.Append("SerialNumber,");
            sb.Append("Firmware,");
            sb.Append("SubsystemCode,");
            sb.Append("SubsystemIndex,");
            sb.Append("Status");

            return sb.ToString();
        }

        /// <summary>
        /// Output the Ensemble dataset to CSV format.
        /// </summary>
        /// <param name="ensemble">Data.</param>
        /// <returns>Ensemble DataSet data in CSV format.</returns>
        public static string WriteEnsembleData(Ensemble ensemble)
        {
            if (ensemble.IsEnsembleAvail)
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(ensemble.EnsembleData.EnsembleNumber);
                sb.Append(",");
                sb.Append(ensemble.EnsembleData.EnsDateTime.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.EnsembleData.NumBins);
                sb.Append(",");
                sb.Append(ensemble.EnsembleData.NumBeams);
                sb.Append(",");
                sb.Append(ensemble.EnsembleData.DesiredPingCount);
                sb.Append(",");
                sb.Append(ensemble.EnsembleData.ActualPingCount);
                sb.Append(",");
                sb.Append(ensemble.EnsembleData.SysSerialNumber.ToString());
                sb.Append(",");
                sb.Append(ensemble.EnsembleData.SysFirmware.ToString());
                sb.Append(",");
                sb.Append(ensemble.EnsembleData.SubsystemConfig.SubSystem.CodeToString());
                sb.Append(",");
                sb.Append(ensemble.EnsembleData.SubsystemConfig.SubSystem.Index);
                sb.Append(",");
                sb.Append(ensemble.EnsembleData.Status.ToString().Replace(",", ";"));        // Replace the , with ; so it does not mess up the seperation);

                return sb.ToString();
            }

            return ",,,,,,,,,,";
        }

        #endregion

        #region Ancillary DataSet

        /// <summary>
        /// Create the Ancillary header based off the options selected.
        /// </summary>
        /// <returns>Ancillary Header</returns>
        public static string CreateAncillaryHeader()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("FirstBinRange,");
            sb.Append("BinSize,");
            sb.Append("FirstPingTime,");
            sb.Append("LastPingTime,");
            sb.Append("Heading,");
            sb.Append("Pitch,");
            sb.Append("Roll,");
            sb.Append("WaterTemp,");
            sb.Append("SystemTemp,");
            sb.Append("Salinity,");
            sb.Append("Pressure,");
            sb.Append("TransducerDepth,");
            sb.Append("SpeedOfSound");

            return sb.ToString();
        }

        /// <summary>
        /// Output the Ancillary dataset to CSV format.
        /// </summary>
        /// <param name="ensemble">Data.</param>
        /// <returns>Ancillary DataSet data in CSV format.</returns>
        public static string WriteAncillaryData(Ensemble ensemble)
        {
            if (ensemble.IsAncillaryAvail)
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(ensemble.AncillaryData.FirstBinRange.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.AncillaryData.BinSize.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.AncillaryData.FirstPingTime.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.AncillaryData.LastPingTime.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.AncillaryData.Heading.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.AncillaryData.Pitch.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.AncillaryData.Roll.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.AncillaryData.WaterTemp.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.AncillaryData.SystemTemp.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.AncillaryData.Salinity.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.AncillaryData.Pressure.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.AncillaryData.TransducerDepth.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.AncillaryData.SpeedOfSound.ToString(new CultureInfo("en-US")));

                return sb.ToString();
            }

            return ",,,,,,,,,,,,";
        }

        #endregion

        #region Beam Velocity DataSet

        /// <summary>
        /// Create the Beam Velocity header based off the options selected.
        /// </summary>
        /// <param name="options">Export Options.</param>
        /// <returns>Beam Velocity Header</returns>
        public static string CreateBeamVelocityHeader(ExportOptions options)
        {
            StringBuilder sb = new StringBuilder();

            for (int bin = options.BeamMinBin; bin < options.BeamMaxBin + 1; bin++)
            {
                for (int beam = 0; beam < DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM; beam++)
                {
                    sb.Append(string.Format("BeamVel{0}_{1},", bin, beam));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Output the Beam Velocity dataset to CSV format.
        /// </summary>
        /// <param name="ensemble">Data.</param>
        /// <param name="options">Export Options.</param>
        /// <returns>Beam Velocity DataSet data in CSV format.</returns>
        public static string WriteBeamVelocityData(Ensemble ensemble, ExportOptions options)
        {
            StringBuilder sb = new StringBuilder();

            if (ensemble.IsBeamVelocityAvail)
            {
                //// Check values
                //int minBin = options.BeamMinBin;
                //if (minBin >= ensemble.BeamVelocityData.BeamVelocityData.GetLength(0))
                //{
                //    minBin = 0;
                //}
                //int maxBin = options.BeamMaxBin;
                //if (maxBin >= ensemble.BeamVelocityData.BeamVelocityData.GetLength(0))
                //{
                //    maxBin = ensemble.BeamVelocityData.BeamVelocityData.GetLength(0) - 1;
                //}

                for (int bin = options.BeamMinBin; bin < options.BeamMaxBin + 1; bin++)
                {
                    for (int beam = 0; beam < ensemble.BeamVelocityData.BeamVelocityData.GetLength(1); beam++)
                    {
                        sb.Append(string.Format("{0},", ensemble.BeamVelocityData.BeamVelocityData[bin, beam].ToString(new CultureInfo("en-US"))));
                    }

                    // 3 Beam System
                    if(ensemble.BeamVelocityData.BeamVelocityData.GetLength(1) <= 3)
                    {
                        sb.Append(",");                                                 // Handle the missing beam
                    }

                    // If a vertical beam
                    if (ensemble.BeamVelocityData.BeamVelocityData.GetLength(1) <= 1)
                    {
                        sb.Append(",,,");                                               // Handle the missing 3 beams
                    }
                }
            }
            else
            {
                // Put blank data
                for (int bin = options.BeamMinBin; bin < options.BeamMaxBin + 1; bin++)
                {
                    sb.Append(",,,,");
                }
            }

            return sb.ToString();
        }




        #endregion

        #region Instrument Velocity DataSet

        /// <summary>
        /// Create the Instrument Velocity header based off the options selected.
        /// </summary>
        /// <param name="options">Export Options.</param>
        /// <returns>Instrument Velocity Header</returns>
        public static string CreateInstrumentVelocityHeader(ExportOptions options)
        {
            StringBuilder sb = new StringBuilder();

            for (int bin = options.InstrumentMinBin; bin < options.InstrumentMaxBin + 1; bin++)
            {
                for (int beam = 0; beam < DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM; beam++)
                {
                    sb.Append(string.Format("InstrVel{0}_{1},", bin, beam));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Output the Instrument Velocity dataset to CSV format.
        /// </summary>
        /// <param name="ensemble">Data.</param>
        /// <param name="options">Export Options.</param>
        /// <returns>Instrument Velocity DataSet data in CSV format.</returns>
        public static string WriteInstrumentVelocityData(Ensemble ensemble, ExportOptions options)
        {
            StringBuilder sb = new StringBuilder();

            if (ensemble.IsInstrumentVelocityAvail)
            {
                for (int bin = options.InstrumentMinBin; bin < options.InstrumentMaxBin + 1; bin++)
                {
                    for (int beam = 0; beam < ensemble.InstrumentVelocityData.InstrumentVelocityData.GetLength(1); beam++)
                    {
                        sb.Append(string.Format("{0},", ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, beam].ToString(new CultureInfo("en-US"))));
                    }

                    // 3 Beam System
                    if (ensemble.BeamVelocityData.BeamVelocityData.GetLength(1) <= 3)
                    {
                        sb.Append(",");                                                 // Handle the missing beam
                    }

                    // If a vertical beam
                    if (ensemble.InstrumentVelocityData.InstrumentVelocityData.GetLength(1) <= 1)
                    {
                        sb.Append(",,,");
                    }
                }
            }
            else
            {
                // Put blank data
                for (int bin = options.InstrumentMinBin; bin < options.InstrumentMaxBin + 1; bin++)
                {
                    sb.Append(",,,,");
                }
            }

            return sb.ToString();
        }

        #endregion

        #region Earth Velocity DataSet

        /// <summary>
        /// Create the Earth Velocity header based off the options selected.
        /// </summary>
        /// <param name="options">Export Options.</param>
        /// <returns>Earth Velocity Header</returns>
        public static string CreateEarthVelocityHeader(ExportOptions options)
        {
            StringBuilder sb = new StringBuilder();

            for (int bin = options.EarthMinBin; bin < options.EarthMaxBin + 1; bin++)
            {
                for (int beam = 0; beam < DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM; beam++)
                {
                    sb.Append(string.Format("EarthVel{0}_{1},", bin, beam));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Output the Earth Velocity dataset to CSV format.
        /// </summary>
        /// <param name="ensemble">Data.</param>
        /// <param name="options">Export Options.</param>
        /// <returns>Earth Velocity DataSet data in CSV format.</returns>
        public static string WriteEarthVelocityData(Ensemble ensemble, ExportOptions options)
        {
            StringBuilder sb = new StringBuilder();

            if (ensemble.IsEarthVelocityAvail)
            {
                for (int bin = options.EarthMinBin; bin < options.EarthMaxBin + 1; bin++)
                {
                    for (int beam = 0; beam < ensemble.EarthVelocityData.EarthVelocityData.GetLength(1); beam++)
                    {
                        sb.Append(string.Format("{0},", ensemble.EarthVelocityData.EarthVelocityData[bin, beam].ToString(new CultureInfo("en-US"))));
                    }

                    // 3 Beam System
                    if (ensemble.BeamVelocityData.BeamVelocityData.GetLength(1) <= 3)
                    {
                        sb.Append(",");                                                 // Handle the missing beam
                    }

                    // If a vertical beam
                    if (ensemble.EarthVelocityData.EarthVelocityData.GetLength(1) <= 1)
                    {
                        sb.Append(",,,");
                    }
                }
            }
            else
            {
                // Put blank data
                for (int bin = options.EarthMinBin; bin < options.EarthMaxBin + 1; bin++)
                {
                    sb.Append(",,,,");
                }
            }

            return sb.ToString();
        }




        #endregion

        #region Velocity Vector DataSet

        /// <summary>
        /// Create the Velocity Vector header based off the options selected.
        /// </summary>
        /// <param name="options">Export Options.</param>
        /// <returns>Velocity Velocity Header</returns>
        public static string CreateVelocityVectorHeader(ExportOptions options)
        {
            StringBuilder sb = new StringBuilder();

            for (int bin = options.VelVectorMinBin; bin < options.VelVectorMaxBin + 1; bin++)
            {
                sb.Append(string.Format("VelVector{0}_{1},", bin, "Mag"));
                sb.Append(string.Format("VelVector{0}_{1},", bin, "Dir XNorth"));
                sb.Append(string.Format("VelVector{0}_{1},", bin, "Dir YNorth"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Output the Velocity Vector dataset to CSV format.
        /// </summary>
        /// <param name="ensemble">Data.</param>
        /// <param name="options">Export Options.</param>
        /// <returns>Velocity Vector DataSet data in CSV format.</returns>
        public static string WriteVelocityVectorData(Ensemble ensemble, ExportOptions options)
        {
            StringBuilder sb = new StringBuilder();

            if (ensemble.IsEarthVelocityAvail && ensemble.EarthVelocityData.IsVelocityVectorAvail)
            {
                for (int bin = options.VelVectorMinBin; bin < options.VelVectorMaxBin + 1; bin++)
                {
                        sb.Append(string.Format("{0},", ensemble.EarthVelocityData.VelocityVectors[bin].Magnitude.ToString(new CultureInfo("en-US"))));
                        sb.Append(string.Format("{0},", ensemble.EarthVelocityData.VelocityVectors[bin].DirectionXNorth.ToString(new CultureInfo("en-US"))));
                        sb.Append(string.Format("{0},", ensemble.EarthVelocityData.VelocityVectors[bin].DirectionYNorth.ToString(new CultureInfo("en-US"))));
                }
            }
            else
            {
                // Put blank data
                for (int bin = options.VelVectorMinBin; bin < options.VelVectorMaxBin + 1; bin++)
                {
                    sb.Append(",,,");
                }
            }

            return sb.ToString();
        }




        #endregion

        #region Correlation DataSet

        /// <summary>
        /// Create the Correlation header based off the options selected.
        /// </summary>
        /// <param name="options">Export Options.</param>
        /// <returns>Correlation Header</returns>
        public static string CreateCorrelationHeader(ExportOptions options)
        {
            StringBuilder sb = new StringBuilder();

            for (int bin = options.CorrelationMinBin; bin < options.CorrelationMaxBin + 1; bin++)
            {
                for (int beam = 0; beam < DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM; beam++)
                {
                    sb.Append(string.Format("Corr{0}_{1},", bin, beam));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Output the Correlation dataset to CSV format.
        /// </summary>
        /// <param name="ensemble">Data.</param>
        /// <param name="options">Export Options.</param>
        /// <returns>Correlation DataSet data in CSV format.</returns>
        public static string WriteCorrelationData(Ensemble ensemble, ExportOptions options)
        {
            StringBuilder sb = new StringBuilder();

            if (ensemble.IsCorrelationAvail)
            {
                for (int bin = options.CorrelationMinBin; bin < options.CorrelationMaxBin + 1; bin++)
                {
                    for (int beam = 0; beam < ensemble.CorrelationData.CorrelationData.GetLength(1); beam++)
                    {
                        sb.Append(string.Format("{0},", ensemble.CorrelationData.CorrelationData[bin, beam].ToString(new CultureInfo("en-US"))));
                    }

                    // 3 Beam System
                    if (ensemble.CorrelationData.CorrelationData.GetLength(1) <= 3)
                    {
                        sb.Append(",");
                    }

                    // If a vertical beam
                    if (ensemble.CorrelationData.CorrelationData.GetLength(1) <= 1)
                    {
                        sb.Append(",,,");
                    }
                }
            }
            else
            {
                // Put blank data
                for (int bin = options.CorrelationMinBin; bin < options.CorrelationMaxBin + 1; bin++)
                {
                    sb.Append(",,,,");
                }
            }

            return sb.ToString();
        }

        #endregion

        #region Amplitude DataSet

        /// <summary>
        /// Create the Amplitude header based off the options selected.
        /// </summary>
        /// <param name="options">Export Options.</param>
        /// <returns>Amplitude Header</returns>
        public static string CreateAmplitudeHeader(ExportOptions options)
        {
            StringBuilder sb = new StringBuilder();

            for (int bin = options.AmplitudeMinBin; bin < options.AmplitudeMaxBin + 1; bin++)
            {
                for (int beam = 0; beam < DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM; beam++)
                {
                    sb.Append(string.Format("Amp{0}_{1},", bin, beam));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Output the Amplitude dataset to CSV format.
        /// </summary>
        /// <param name="ensemble">Data.</param>
        /// <param name="options">Export Options.</param>
        /// <returns>Amplitude DataSet data in CSV format.</returns>
        public static string WriteAmplitudeData(Ensemble ensemble, ExportOptions options)
        {
            StringBuilder sb = new StringBuilder();

            if (ensemble.IsAmplitudeAvail)
            {
                for (int bin = options.AmplitudeMinBin; bin < options.AmplitudeMaxBin + 1; bin++)
                {
                    for (int beam = 0; beam < ensemble.AmplitudeData.AmplitudeData.GetLength(1); beam++)
                    {
                        sb.Append(string.Format("{0},", ensemble.AmplitudeData.AmplitudeData[bin, beam].ToString(new CultureInfo("en-US"))));
                    }

                    // 3 Beam System
                    if (ensemble.AmplitudeData.AmplitudeData.GetLength(1) <= 3)
                    {
                        sb.Append(",");
                    }

                    // If a vertical beam
                    if (ensemble.AmplitudeData.AmplitudeData.GetLength(1) <= 1)
                    {
                        sb.Append(",,,");
                    }
                }
            }
            else
            {
                // Put blank data
                for (int bin = options.AmplitudeMinBin; bin < options.AmplitudeMaxBin + 1; bin++)
                {
                    sb.Append(",,,,");
                }
            }

            return sb.ToString();
        }

        #endregion

        #region Earth Water Mass DataSet

        /// <summary>
        /// Create the Earth Water Mass header based off the options selected.
        /// </summary>
        /// <returns>Earth Water Mass Header</returns>
        public static string CreateEarthWaterMassHeader()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("EarthWmLayer,");
            sb.Append("EarthWmVelEast,");
            sb.Append("EarthWmVelNorth,");
            sb.Append("EarthWmVelVertical,");

            return sb.ToString();
        }

        /// <summary>
        /// Output the Earth Water Mass dataset to CSV format.
        /// </summary>
        /// <param name="ensemble">Data.</param>
        /// <returns>Earth Water Mass DataSet data in CSV format.</returns>
        public static string WriteEarthWaterMassData(Ensemble ensemble)
        {
            StringBuilder sb = new StringBuilder();

            if (ensemble.IsEarthWaterMassAvail)
            {
                sb.Append(ensemble.EarthWaterMassData.WaterMassDepthLayer.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.EarthWaterMassData.VelocityEast.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.EarthWaterMassData.VelocityNorth.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.EarthWaterMassData.VelocityVertical.ToString(new CultureInfo("en-US")));
            }
            else
            {
                // Put blank data
                sb.Append(",,,,");
            }

            return sb.ToString();
        }

        #endregion

        #region Instrument Water Mass DataSet

        /// <summary>
        /// Create the Instrument Water Mass header based off the options selected.
        /// </summary>
        /// <returns>Instrument Water Mass Header</returns>
        public static string CreateInstrumentWaterMassHeader()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("InstrWmLayer,");
            sb.Append("InstrWmVelX,");
            sb.Append("InstrWmVelY,");
            sb.Append("InstrWmVelZ,");

            return sb.ToString();
        }

        /// <summary>
        /// Output the Instrument Water Mass dataset to CSV format.
        /// </summary>
        /// <param name="ensemble">Data.</param>
        /// <returns>Instrument Water Mass DataSet data in CSV format.</returns>
        public static string WriteInstrumentWaterMassData(Ensemble ensemble)
        {
            StringBuilder sb = new StringBuilder();

            if (ensemble.IsInstrumentWaterMassAvail)
            {
                sb.Append(ensemble.InstrumentWaterMassData.WaterMassDepthLayer.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.InstrumentWaterMassData.VelocityX.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.InstrumentWaterMassData.VelocityY.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.InstrumentWaterMassData.VelocityZ.ToString(new CultureInfo("en-US")));
            }
            else
            {
                // Put blank data
                sb.Append(",,,,");
            }

            return sb.ToString();
        }

        #endregion

        #region Good Beam DataSet

        /// <summary>
        /// Create the Good Beam header based off the options selected.
        /// </summary>
        /// <param name="options">Export Options.</param>
        /// <returns>Good Beam Header</returns>
        public static string CreateGoodBeamHeader(ExportOptions options)
        {
            StringBuilder sb = new StringBuilder();

            for (int bin = options.GoodBeamMinBin; bin < options.GoodBeamMaxBin + 1; bin++)
            {
                for (int beam = 0; beam < DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM; beam++)
                {
                    sb.Append(string.Format("GoodBeam{0}_{1},", bin, beam));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Output the Good Beam dataset to CSV format.
        /// </summary>
        /// <param name="ensemble">Data.</param>
        /// <param name="options">Export Options.</param>
        /// <returns>Good Beam DataSet data in CSV format.</returns>
        public static string WriteGoodBeamData(Ensemble ensemble, ExportOptions options)
        {
            StringBuilder sb = new StringBuilder();

            if (ensemble.IsGoodBeamAvail)
            {
                for (int bin = options.GoodBeamMinBin; bin < options.GoodBeamMaxBin + 1; bin++)
                {
                    for (int beam = 0; beam < ensemble.GoodBeamData.GoodBeamData.GetLength(1); beam++)
                    {
                        sb.Append(string.Format("{0},", ensemble.GoodBeamData.GoodBeamData[bin, beam]));
                    }

                    // 3 Beam System
                    if (ensemble.GoodBeamData.GoodBeamData.GetLength(1) <= 3)
                    {
                        sb.Append(",");
                    }

                    // If a vertical beam
                    if (ensemble.GoodBeamData.GoodBeamData.GetLength(1) <= 1)
                    {
                        sb.Append(",,,");
                    }
                }
            }
            else
            {
                // Put blank data
                for (int bin = options.GoodBeamMinBin; bin < options.GoodBeamMaxBin + 1; bin++)
                {
                    sb.Append(",,,,");
                }
            }

            return sb.ToString();
        }

        #endregion

        #region Good Earth DataSet

        /// <summary>
        /// Create the Good Earth header based off the options selected.
        /// </summary>
        /// <param name="options">Export Options.</param>
        /// <returns>Good Earth Header</returns>
        public static string CreateGoodEarthHeader(ExportOptions options)
        {
            StringBuilder sb = new StringBuilder();

            for (int bin = options.GoodEarthMinBin; bin < options.GoodEarthMaxBin + 1; bin++)
            {
                for (int beam = 0; beam < DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM; beam++)
                {
                    sb.Append(string.Format("GoodEarth{0}_{1},", bin, beam));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Output the Good Earth dataset to CSV format.
        /// </summary>
        /// <param name="ensemble">Data.</param>
        /// <param name="options">Export Options.</param>
        /// <returns>Good Earth DataSet data in CSV format.</returns>
        public static string WriteGoodEarthData(Ensemble ensemble, ExportOptions options)
        {
            StringBuilder sb = new StringBuilder();

            if (ensemble.IsGoodEarthAvail)
            {
                for (int bin = options.GoodEarthMinBin; bin < options.GoodEarthMaxBin + 1; bin++)
                {
                    for (int beam = 0; beam < ensemble.GoodEarthData.GoodEarthData.GetLength(1); beam++)
                    {
                        sb.Append(string.Format("{0},", ensemble.GoodEarthData.GoodEarthData[bin, beam]));
                    }

                    // 3 Beam System
                    if (ensemble.GoodEarthData.GoodEarthData.GetLength(1) <= 3)
                    {
                        sb.Append(",");
                    }

                    // If a vertical beam
                    if (ensemble.GoodEarthData.GoodEarthData.GetLength(1) <= 1)
                    {
                        sb.Append(",,,");
                    }
                }
            }
            else
            {
                // Put blank data
                for (int bin = options.GoodEarthMinBin; bin < options.GoodEarthMaxBin + 1; bin++)
                {
                    sb.Append(",,,,");
                }
            }

            return sb.ToString();
        }




        #endregion

        #region Bottom Track DataSet

        /// <summary>
        /// Create the Bottom Track header based off the options selected.
        /// </summary>
        /// <returns>Bottom Track Header.</returns>
        public static string CreateBottomTrackHeader()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("FirstPingTime,");
            sb.Append("LastPingTime,");
            sb.Append("Heading,");
            sb.Append("Pitch,");
            sb.Append("Roll,");
            sb.Append("WaterTemp,");
            sb.Append("SystemTemp,");
            sb.Append("Salinity,");
            sb.Append("Pressure,");
            sb.Append("TransducerDepth,");
            sb.Append("SpeedOfSound,");
            sb.Append("Status,");
            sb.Append("NumBeams,");
            sb.Append("ActualPingCount,");

            sb.Append("Range[0],");
            sb.Append("Range[1],");
            sb.Append("Range[2],");
            sb.Append("Range[3],");
            sb.Append("SNR[0],");
            sb.Append("SNR[1],");
            sb.Append("SNR[2],");
            sb.Append("SNR[3],");
            sb.Append("Amplitude[0],");
            sb.Append("Amplitude[1],");
            sb.Append("Amplitude[2],");
            sb.Append("Amplitude[3],");
            sb.Append("Correlation[0],");
            sb.Append("Correlation[1],");
            sb.Append("Correlation[2],");
            sb.Append("Correlation[3],");
            sb.Append("BeamVelocity[0],");
            sb.Append("BeamVelocity[1],");
            sb.Append("BeamVelocity[2],");
            sb.Append("BeamVelocity[3],");
            sb.Append("BeamGood[0],");
            sb.Append("BeamGood[1],");
            sb.Append("BeamGood[2],");
            sb.Append("BeamGood[3],");
            sb.Append("InstrumentVelocity[0],");
            sb.Append("InstrumentVelocity[1],");
            sb.Append("InstrumentVelocity[2],");
            sb.Append("InstrumentVelocity[3],");
            sb.Append("InstrumentGood[0],");
            sb.Append("InstrumentGood[1],");
            sb.Append("InstrumentGood[2],");
            sb.Append("InstrumentGood[3],");
            sb.Append("EarthVelocity[0],");
            sb.Append("EarthVelocity[1],");
            sb.Append("EarthVelocity[2],");
            sb.Append("EarthVelocity[3],");
            sb.Append("EarthGood[0],");
            sb.Append("EarthGood[1],");
            sb.Append("EarthGood[2],");
            sb.Append("EarthGood[3]");

            return sb.ToString();
        }

        /// <summary>
        /// Output the Bottom Track dataset to CSV format.
        /// </summary>
        /// <param name="ensemble">Data.</param>
        /// <returns>Bottom Track DataSet data in CSV format.</returns>
        public static string WriteBottomTrackData(Ensemble ensemble)
        {
            StringBuilder sb = new StringBuilder();

            if (ensemble.IsBottomTrackAvail)
            {
                sb.Append(ensemble.BottomTrackData.FirstPingTime.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.LastPingTime.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.Heading.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.Pitch.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.Roll.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.WaterTemp.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.SystemTemp.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.Salinity.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.Pressure.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.TransducerDepth.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.SpeedOfSound.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.Status.ToString().Replace(",", ";"));        // Replace the , with ; so it does not mess up the seperation
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.NumBeams.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.ActualPingCount.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                #region Range Tracking
                if (ensemble.BottomTrackData.Range.Length >= 1)
                {
                    sb.Append(ensemble.BottomTrackData.Range[0].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.Range.Length >= 2)
                {
                    sb.Append(ensemble.BottomTrackData.Range[1].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.Range.Length >= 3)
                {
                    sb.Append(ensemble.BottomTrackData.Range[2].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.Range.Length >= 4)
                {
                    sb.Append(ensemble.BottomTrackData.Range[3].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                #endregion
                #region SNR
                if (ensemble.BottomTrackData.SNR.Length >= 1)
                {
                    sb.Append(ensemble.BottomTrackData.SNR[0].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.SNR.Length >= 2)
                {
                    sb.Append(ensemble.BottomTrackData.SNR[1].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.SNR.Length >= 3)
                {
                    sb.Append(ensemble.BottomTrackData.SNR[2].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.SNR.Length >= 4)
                {
                    sb.Append(ensemble.BottomTrackData.SNR[3].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                #endregion
                #region Amplitude
                if (ensemble.BottomTrackData.Amplitude.Length >= 1)
                {
                    sb.Append(ensemble.BottomTrackData.Amplitude[0].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.Amplitude.Length >= 2)
                {
                    sb.Append(ensemble.BottomTrackData.Amplitude[1].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.Amplitude.Length >= 3)
                {
                    sb.Append(ensemble.BottomTrackData.Amplitude[2].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.Amplitude.Length >= 4)
                {
                    sb.Append(ensemble.BottomTrackData.Amplitude[3].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                #endregion
                #region Correlation
                if (ensemble.BottomTrackData.Correlation.Length >= 1)
                {
                    sb.Append(ensemble.BottomTrackData.Correlation[0].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.Correlation.Length >= 2)
                {
                    sb.Append(ensemble.BottomTrackData.Correlation[1].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.Correlation.Length >= 3)
                {
                    sb.Append(ensemble.BottomTrackData.Correlation[2].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.Correlation.Length >= 4)
                {
                    sb.Append(ensemble.BottomTrackData.Correlation[3].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                #endregion
                #region Beam Velocity
                if (ensemble.BottomTrackData.BeamVelocity.Length >= 1)
                {
                    sb.Append(ensemble.BottomTrackData.BeamVelocity[0].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.BeamVelocity.Length >= 2)
                {
                    sb.Append(ensemble.BottomTrackData.BeamVelocity[1].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.BeamVelocity.Length >= 3)
                {
                    sb.Append(ensemble.BottomTrackData.BeamVelocity[2].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.BeamVelocity.Length >= 4)
                {
                    sb.Append(ensemble.BottomTrackData.BeamVelocity[3].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                #endregion
                #region BeamGood
                if (ensemble.BottomTrackData.BeamGood.Length >= 1)
                {
                    sb.Append(ensemble.BottomTrackData.BeamGood[0].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.BeamGood.Length >= 2)
                {
                    sb.Append(ensemble.BottomTrackData.BeamGood[1].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.BeamGood.Length >= 3)
                {
                    sb.Append(ensemble.BottomTrackData.BeamGood[2].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.BeamGood.Length >= 4)
                {
                    sb.Append(ensemble.BottomTrackData.BeamGood[3].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                #endregion
                #region Instrument Velocity
                if (ensemble.BottomTrackData.InstrumentVelocity.Length >= 1)
                {
                    sb.Append(ensemble.BottomTrackData.InstrumentVelocity[0].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.InstrumentVelocity.Length >= 2)
                {
                    sb.Append(ensemble.BottomTrackData.InstrumentVelocity[1].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.InstrumentVelocity.Length >= 3)
                {
                    sb.Append(ensemble.BottomTrackData.InstrumentVelocity[2].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.InstrumentVelocity.Length >= 4)
                {
                    sb.Append(ensemble.BottomTrackData.InstrumentVelocity[3].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                #endregion
                #region Instrument Good
                if (ensemble.BottomTrackData.InstrumentGood.Length >= 1)
                {
                    sb.Append(ensemble.BottomTrackData.InstrumentGood[0].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.InstrumentGood.Length >= 2)
                {
                    sb.Append(ensemble.BottomTrackData.InstrumentGood[1].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.InstrumentGood.Length >= 3)
                {
                    sb.Append(ensemble.BottomTrackData.InstrumentGood[2].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.InstrumentGood.Length >= 4)
                {
                    sb.Append(ensemble.BottomTrackData.InstrumentGood[3].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                #endregion
                #region Earth Velocity
                if (ensemble.BottomTrackData.EarthVelocity.Length >= 1)
                {
                    sb.Append(ensemble.BottomTrackData.EarthVelocity[0].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.EarthVelocity.Length >= 2)
                {
                    sb.Append(ensemble.BottomTrackData.EarthVelocity[1].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.EarthVelocity.Length >= 3)
                {
                    sb.Append(ensemble.BottomTrackData.EarthVelocity[2].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.EarthVelocity.Length >= 4)
                {
                    sb.Append(ensemble.BottomTrackData.EarthVelocity[3].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                #endregion
                #region Earth Good
                if (ensemble.BottomTrackData.EarthGood.Length >= 1)
                {
                    sb.Append(ensemble.BottomTrackData.EarthGood[0].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.EarthGood.Length >= 2)
                {
                    sb.Append(ensemble.BottomTrackData.EarthGood[1].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.EarthGood.Length >= 3)
                {
                    sb.Append(ensemble.BottomTrackData.EarthGood[2].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.BottomTrackData.EarthGood.Length >= 4)
                {
                    sb.Append(ensemble.BottomTrackData.EarthGood[3].ToString(new CultureInfo("en-US")));
                    //sb.Append(",");
                }
                else
                {
                    //sb.Append(",");
                }
                #endregion

            }
            else
            {
                sb.Append(",,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,");
            }

            return sb.ToString();
        }

        #endregion

        #region Bottom Track Engineering DataSet

        /// <summary>
        /// Create the Bottom Track Engineering header based off the options selected.
        /// </summary>
        /// <returns>Bottom Track Engineering Header.</returns>
        public static string CreateBottomTrackEngineeringHeader()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("SamplesPerSecond,");
            sb.Append("SystemFreqHz,");
            sb.Append("LagSamples,");
            sb.Append("CPCE,");
            sb.Append("NCE,");
            sb.Append("RepeatN,");
            sb.Append("AmbHz0,");
            sb.Append("AmbHz1,");
            sb.Append("AmbHz2,");
            sb.Append("AmbHz3,");
            sb.Append("AmbVel0,");
            sb.Append("AmbVel1,");
            sb.Append("AmbVel2,");
            sb.Append("AmbVel3,");
            sb.Append("AmbAmp0,");
            sb.Append("AmbAmp1,");
            sb.Append("AmbAmp2,");
            sb.Append("AmbAmp3,");
            sb.Append("AmpCor0,");
            sb.Append("AmpCor1,");
            sb.Append("AmpCor2,");
            sb.Append("AmpCor3,");
            sb.Append("AmbSNR0,");
            sb.Append("AmbSNR1,");
            sb.Append("AmbSNR2,");
            sb.Append("AmbSNR3,");
            sb.Append("LagUsed0,");
            sb.Append("LagUsed1,");
            sb.Append("LagUsed2,");
            sb.Append("LagUsed3");

            return sb.ToString();
        }

        /// <summary>
        /// Output the Bottom Track Engineering dataset to CSV format.
        /// </summary>
        /// <param name="ensemble">Data.</param>
        /// <returns>Bottom Track Engineering DataSet data in CSV format.</returns>
        public static string WriteBottomTrackEngineeringData(Ensemble ensemble)
        {
            StringBuilder sb = new StringBuilder();

            if (ensemble.IsBottomTrackEngineeringAvail)
            {
                sb.Append(ensemble.BottomTrackEngineeringData.SamplesPerSecond.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.SystemFreqHz.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.LagSamples.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.CPCE.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.NCE.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.RepeatN.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbHz[0].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbHz[1].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbHz[2].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbHz[3].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbVel[0].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbVel[1].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbVel[2].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbVel[3].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbAmp[0].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbAmp[1].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbAmp[2].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbAmp[3].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbCor[0].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbCor[1].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbCor[2].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbCor[3].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbSNR[0].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbSNR[1].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbSNR[2].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbSNR[3].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.LagUsed[0].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.LagUsed[1].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.LagUsed[2].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.LagUsed[3].ToString(new CultureInfo("en-US")));

            }
            else
            {
                sb.Append(",,,,,,,,,,,,,,,,,,,,,,,,,,,,,");
            }

            return sb.ToString();
        }

        #endregion

        #region Profile Engineering DataSet

        /// <summary>
        /// Create the Profile Engineering header based off the options selected.
        /// </summary>
        /// <returns>Profile Engineering Header.</returns>
        public static string CreateProfileEngineeringHeader()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("SamplesPerSecond,");
            sb.Append("SystemFreqHz,");
            sb.Append("LagSamples,");
            sb.Append("CPCE,");
            sb.Append("NCE,");
            sb.Append("RepeatN,");
            sb.Append("PrePingGap,");
            sb.Append("PrePingNCE,");
            sb.Append("PrePingRepeatN,");
            sb.Append("PrePingLagSamples,");
            sb.Append("TRHighGain,");
            sb.Append("PrePingVel0,");
            sb.Append("PrePingVel1,");
            sb.Append("PrePingVel2,");
            sb.Append("PrePingVel3,");
            sb.Append("PrePingCor0,");
            sb.Append("PrePingCor1,");
            sb.Append("PrePingCor2,");
            sb.Append("PrePingCor3,");
            sb.Append("PrePingAmp0,");
            sb.Append("PrePingAmp1,");
            sb.Append("PrePingAmp2,");
            sb.Append("PrePingAmp3");

            return sb.ToString();
        }

        /// <summary>
        /// Output the Profile Engineering dataset to CSV format.
        /// </summary>
        /// <param name="ensemble">Data.</param>
        /// <returns>Profile Engineering DataSet data in CSV format.</returns>
        public static string WriteProfileEngineeringData(Ensemble ensemble)
        {
            StringBuilder sb = new StringBuilder();

            if (ensemble.IsProfileEngineeringAvail)
            {
                sb.Append(ensemble.ProfileEngineeringData.SamplesPerSecond.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.SystemFreqHz.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.LagSamples.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.CPCE.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.NCE.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.RepeatN.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.PrePingGap.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.PrePingNCE.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.PrePingRepeatN.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.PrePingLagSamples.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.TRHighGain.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                if (ensemble.ProfileEngineeringData.PrePingVel.Length > 0)
                    sb.Append(ensemble.ProfileEngineeringData.PrePingVel[0].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                if (ensemble.ProfileEngineeringData.PrePingVel.Length > 1)
                    sb.Append(ensemble.ProfileEngineeringData.PrePingVel[1].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                if (ensemble.ProfileEngineeringData.PrePingVel.Length > 2)
                    sb.Append(ensemble.ProfileEngineeringData.PrePingVel[2].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                if (ensemble.ProfileEngineeringData.PrePingVel.Length > 3)
                    sb.Append(ensemble.ProfileEngineeringData.PrePingVel[3].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                if (ensemble.ProfileEngineeringData.PrePingCor.Length > 0)
                    sb.Append(ensemble.ProfileEngineeringData.PrePingCor[0].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                if (ensemble.ProfileEngineeringData.PrePingCor.Length > 1)
                    sb.Append(ensemble.ProfileEngineeringData.PrePingCor[1].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                if (ensemble.ProfileEngineeringData.PrePingCor.Length > 2)
                    sb.Append(ensemble.ProfileEngineeringData.PrePingCor[2].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                if (ensemble.ProfileEngineeringData.PrePingCor.Length > 3)
                    sb.Append(ensemble.ProfileEngineeringData.PrePingCor[3].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                if (ensemble.ProfileEngineeringData.PrePingAmp.Length > 0)
                    sb.Append(ensemble.ProfileEngineeringData.PrePingAmp[0].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                if (ensemble.ProfileEngineeringData.PrePingAmp.Length > 1)
                    sb.Append(ensemble.ProfileEngineeringData.PrePingAmp[1].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                if (ensemble.ProfileEngineeringData.PrePingAmp.Length > 2)
                    sb.Append(ensemble.ProfileEngineeringData.PrePingAmp[2].ToString(new CultureInfo("en-US")));
                sb.Append(",");
                if (ensemble.ProfileEngineeringData.PrePingAmp.Length > 3)
                    sb.Append(ensemble.ProfileEngineeringData.PrePingAmp[3].ToString(new CultureInfo("en-US")));
            }
            else
            {
                sb.Append(",,,,,,,,,,,,,,,,,,,,,,");
            }

            return sb.ToString();
        }

        #endregion

        #region System Setup DataSet

        /// <summary>
        /// Create the System Setup header based off the options selected.
        /// </summary>
        /// <returns>System Setup Header.</returns>
        public static string CreateSystemSetupHeader()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("BtSamplesPerSecond,");
            sb.Append("BtSystemFreqHz,");
            sb.Append("BtCPCE,");
            sb.Append("BtNCE,");
            sb.Append("BtRepeatN,");
            sb.Append("WpSamplesPerSecond,");
            sb.Append("WpSystemFreqHz,");
            sb.Append("WpCPCE,");
            sb.Append("WpNCE,");
            sb.Append("WpRepeatN,");
            sb.Append("WpLagSamples,");
            sb.Append("Voltage");

            return sb.ToString();
        }

        /// <summary>
        /// Output the System Setup dataset to CSV format.
        /// </summary>
        /// <param name="ensemble">Data.</param>
        /// <returns>System Setup DataSet data in CSV format.</returns>
        public static string WriteSystemSetupData(Ensemble ensemble)
        {
            StringBuilder sb = new StringBuilder();

            if (ensemble.IsSystemSetupAvail)
            {
                sb.Append(ensemble.SystemSetupData.BtSamplesPerSecond.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.SystemSetupData.BtSystemFreqHz.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.SystemSetupData.BtCPCE.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.SystemSetupData.BtNCE.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.SystemSetupData.BtRepeatN.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.SystemSetupData.WpSamplesPerSecond.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.SystemSetupData.WpSystemFreqHz.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.SystemSetupData.WpCPCE.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.SystemSetupData.WpNCE.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.SystemSetupData.WpRepeatN.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.SystemSetupData.WpLagSamples.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.SystemSetupData.Voltage.ToString(new CultureInfo("en-US")));
            }
            else
            {
                sb.Append(",,,,,,,,,,,");
            }

            return sb.ToString();
        }

        #endregion

        #region Range Tracking DataSet

        /// <summary>
        /// Create the Range Tracking header based off the options selected.
        /// </summary>
        /// <returns>Range Tracking Header.</returns>
        public static string CreateRangeTrackingHeader()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Snr0,");
            sb.Append("Snr1,");
            sb.Append("Snr2,");
            sb.Append("Snr3,");
            sb.Append("Range0,");
            sb.Append("Range1,");
            sb.Append("Range2,");
            sb.Append("Range3,");
            sb.Append("Pings0,");
            sb.Append("Pings1,");
            sb.Append("Pings2,");
            sb.Append("Pings3");

            return sb.ToString();
        }

        /// <summary>
        /// Output the Range Tracking dataset to CSV format.
        /// </summary>
        /// <param name="ensemble">Data.</param>
        /// <returns>Range Tracking DataSet data in CSV format.</returns>
        public static string WriteRangeTrackingData(Ensemble ensemble)
        {
            StringBuilder sb = new StringBuilder();

            if (ensemble.IsRangeTrackingAvail)
            {
                if (ensemble.RangeTrackingData.SNR.Length >= 1)
                {
                    sb.Append(ensemble.RangeTrackingData.SNR[0].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.RangeTrackingData.SNR.Length >= 2)
                {
                    sb.Append(ensemble.RangeTrackingData.SNR[1].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.RangeTrackingData.SNR.Length >= 3)
                {
                    sb.Append(ensemble.RangeTrackingData.SNR[2].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.RangeTrackingData.SNR.Length >= 4)
                {
                    sb.Append(ensemble.RangeTrackingData.SNR[3].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }

                if (ensemble.RangeTrackingData.Range.Length >= 1)
                { 
                    sb.Append(ensemble.RangeTrackingData.Range[0].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.RangeTrackingData.Range.Length >= 2)
                { 
                    sb.Append(ensemble.RangeTrackingData.Range[1].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.RangeTrackingData.Range.Length >= 3)
                {
                    sb.Append(ensemble.RangeTrackingData.Range[2].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.RangeTrackingData.Range.Length >= 4)
                { 
                    sb.Append(ensemble.RangeTrackingData.Range[3].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }

                if (ensemble.RangeTrackingData.Pings.Length >= 1)
                { 
                    sb.Append(ensemble.RangeTrackingData.Pings[0].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.RangeTrackingData.Pings.Length >= 2)
                { 
                    sb.Append(ensemble.RangeTrackingData.Pings[1].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.RangeTrackingData.Pings.Length >= 3)
                { 
                    sb.Append(ensemble.RangeTrackingData.Pings[2].ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
                if (ensemble.RangeTrackingData.Pings.Length >= 4)
                { 
                    sb.Append(ensemble.RangeTrackingData.Pings[3].ToString(new CultureInfo("en-US")));
                    //sb.Append(",");
                }
                else
                {
                    //sb.Append(",");
                }
            }
            else
            {
                sb.Append(",,,,,,,,,,,");
            }

            return sb.ToString();
        }

        #endregion

        #region Gage Height DataSet

        /// <summary>
        /// Create the Gage Height header based off the options selected.
        /// </summary>
        /// <returns>Gage Height Header.</returns>
        public static string CreateGageHeightHeader()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Status,");
            sb.Append("AvgRange,");
            sb.Append("StdDev,");
            sb.Append("AvgSN,");
            sb.Append("N,");
            sb.Append("Salinity,");
            sb.Append("Pressure,");
            sb.Append("Depth,");
            sb.Append("WaterTemp,");
            sb.Append("SystemTemp,");
            sb.Append("SoS,");
            sb.Append("Heading,");
            sb.Append("Pitch,");
            sb.Append("Roll");

            return sb.ToString();
        }

        /// <summary>
        /// Output the Gage Height dataset to CSV format.
        /// </summary>
        /// <param name="ensemble">Data.</param>
        /// <returns>Gage Height DataSet data in CSV format.</returns>
        public static string WriteGageHeightData(Ensemble ensemble)
        {
            StringBuilder sb = new StringBuilder();

            if (ensemble.IsGageHeightAvail)
            {
                sb.Append(ensemble.GageHeightData.Status);
                sb.Append(",");
                sb.Append(ensemble.GageHeightData.AvgRange.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.GageHeightData.StdDev.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.GageHeightData.AvgSN.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.GageHeightData.N.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.GageHeightData.Salinity.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.GageHeightData.Pressure.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.GageHeightData.Depth.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.GageHeightData.WaterTemp.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.GageHeightData.SystemTemp.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.GageHeightData.SoS.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.GageHeightData.Heading.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.GageHeightData.Pitch.ToString(new CultureInfo("en-US")));
                sb.Append(",");
                sb.Append(ensemble.GageHeightData.Roll.ToString(new CultureInfo("en-US")));
            }
            else
            {
                sb.Append(",,,,,,,,,,,,,");
            }

            return sb.ToString();
        }

        #endregion

        #region NMEA DataSet

        /// <summary>
        /// Create the NMEA header based off the options selected.
        /// </summary>
        /// <returns>NMEA Header.</returns>
        public static string CreateNmeaHeader()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Latitude,");
            sb.Append("Longitude,");
            sb.Append("Altitude,");
            sb.Append("UtcTime,");
            sb.Append("Heading");

            return sb.ToString();
        }

        /// <summary>
        /// Output the NMEA dataset to CSV format.
        /// </summary>
        /// <param name="ensemble">Data.</param>
        /// <returns>NMEA DataSet data in CSV format.</returns>
        public static string WriteNmeaData(Ensemble ensemble)
        {
            StringBuilder sb = new StringBuilder();

            if (ensemble.IsNmeaAvail)
            {
                if (ensemble.NmeaData.IsGpggaAvail())
                {
                    sb.Append(ensemble.NmeaData.GPGGA.Position.Latitude.DecimalDegrees.ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                    sb.Append(ensemble.NmeaData.GPGGA.Position.Longitude.DecimalDegrees.ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                    sb.Append(ensemble.NmeaData.GPGGA.Altitude.ToMeters().ToString("g", new CultureInfo("en-US")));
                    sb.Append(",");
                    sb.Append(ensemble.NmeaData.GPGGA.UtcTime);
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",,,,");
                }
                if(ensemble.NmeaData.IsGphdtAvail())
                {
                    sb.Append(ensemble.NmeaData.GPHDT.Heading.DecimalDegrees.ToString(new CultureInfo("en-US")));
                    sb.Append(",");
                }
                else
                {
                    sb.Append(",");
                }
            }
            else
            {
                sb.Append(",,,,,");
            }

            return sb.ToString();
        }

        #endregion

    }
}