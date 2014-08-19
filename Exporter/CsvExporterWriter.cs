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
 * 
 * 
 */


using System.IO;
using RTI.DataSet;
using System.Text;
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

            // Amplitude DataSet
            if (options.IsAmplitudeDataSetOn)
            {
                sb.Append(CreateAmplitudeHeader(options));
                sb.Append(",");
            }

            // Correlation DataSet
            if (options.IsCorrelationDataSetOn)
            {
                sb.Append(CreateAmplitudeHeader(options));
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
                sb.Append(ensemble.EnsembleData.EnsDateTime.ToString());
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

                sb.Append(ensemble.AncillaryData.FirstBinRange);
                sb.Append(",");
                sb.Append(ensemble.AncillaryData.BinSize);
                sb.Append(",");
                sb.Append(ensemble.AncillaryData.FirstPingTime);
                sb.Append(",");
                sb.Append(ensemble.AncillaryData.LastPingTime);
                sb.Append(",");
                sb.Append(ensemble.AncillaryData.Heading);
                sb.Append(",");
                sb.Append(ensemble.AncillaryData.Pitch);
                sb.Append(",");
                sb.Append(ensemble.AncillaryData.Roll);
                sb.Append(",");
                sb.Append(ensemble.AncillaryData.WaterTemp);
                sb.Append(",");
                sb.Append(ensemble.AncillaryData.SystemTemp);
                sb.Append(",");
                sb.Append(ensemble.AncillaryData.Salinity);
                sb.Append(",");
                sb.Append(ensemble.AncillaryData.Pressure);
                sb.Append(",");
                sb.Append(ensemble.AncillaryData.TransducerDepth);
                sb.Append(",");
                sb.Append(ensemble.AncillaryData.SpeedOfSound);

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
                for (int bin = options.BeamMinBin; bin < options.BeamMaxBin + 1; bin++)
                {
                    for (int beam = 0; beam < ensemble.BeamVelocityData.BeamVelocityData.GetLength(1); beam++)
                    {
                        sb.Append(string.Format("{0},", ensemble.BeamVelocityData.BeamVelocityData[bin, beam]));
                    }

                    // If a vertical beam
                    if (ensemble.BeamVelocityData.BeamVelocityData.GetLength(1) <= 1)
                    {
                        sb.Append(",,,");
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
                        sb.Append(string.Format("{0},", ensemble.InstrumentVelocityData.InstrumentVelocityData[bin, beam]));
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
                        sb.Append(string.Format("{0},", ensemble.EarthVelocityData.EarthVelocityData[bin, beam]));
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
                        sb.Append(string.Format("{0},", ensemble.CorrelationData.CorrelationData[bin, beam]));
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
                        sb.Append(string.Format("{0},", ensemble.AmplitudeData.AmplitudeData[bin, beam]));
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
                sb.Append(ensemble.EarthWaterMassData.WaterMassDepthLayer);
                sb.Append(",");
                sb.Append(ensemble.EarthWaterMassData.VelocityEast);
                sb.Append(",");
                sb.Append(ensemble.EarthWaterMassData.VelocityNorth);
                sb.Append(",");
                sb.Append(ensemble.EarthWaterMassData.VelocityVertical);
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
                sb.Append(ensemble.InstrumentWaterMassData.WaterMassDepthLayer);
                sb.Append(",");
                sb.Append(ensemble.InstrumentWaterMassData.VelocityX);
                sb.Append(",");
                sb.Append(ensemble.InstrumentWaterMassData.VelocityY);
                sb.Append(",");
                sb.Append(ensemble.InstrumentWaterMassData.VelocityZ);
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
                sb.Append(ensemble.BottomTrackData.FirstPingTime);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.LastPingTime);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.Heading);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.Pitch);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.Roll);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.WaterTemp);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.SystemTemp);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.Salinity);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.Pressure);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.TransducerDepth);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.SpeedOfSound);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.Status.ToString().Replace(",", ";"));        // Replace the , with ; so it does not mess up the seperation
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.NumBeams);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.ActualPingCount);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.Range[0]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.Range[1]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.Range[2]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.Range[3]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.SNR[0]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.SNR[1]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.SNR[2]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.SNR[3]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.Amplitude[0]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.Amplitude[1]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.Amplitude[2]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.Amplitude[3]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.Correlation[0]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.Correlation[1]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.Correlation[2]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.Correlation[3]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.BeamVelocity[0]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.BeamVelocity[1]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.BeamVelocity[2]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.BeamVelocity[3]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.BeamGood[0]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.BeamGood[1]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.BeamGood[2]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.BeamGood[3]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.InstrumentVelocity[0]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.InstrumentVelocity[1]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.InstrumentVelocity[2]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.InstrumentVelocity[3]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.InstrumentGood[0]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.InstrumentGood[1]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.InstrumentGood[2]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.InstrumentGood[3]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.EarthVelocity[0]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.EarthVelocity[1]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.EarthVelocity[2]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.EarthVelocity[3]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.EarthGood[0]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.EarthGood[1]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.EarthGood[2]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackData.EarthGood[3]);

            }
            else
            {
                sb.Append(",,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,");
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
                sb.Append(ensemble.BottomTrackEngineeringData.SamplesPerSecond);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.SystemFreqHz);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.LagSamples);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.CPCE);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.NCE);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.RepeatN);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbHz[0]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbHz[1]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbHz[2]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbHz[3]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbVel[0]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbVel[1]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbVel[2]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbVel[3]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbAmp[0]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbAmp[1]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbAmp[2]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbAmp[3]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbCor[0]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbCor[1]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbCor[2]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbCor[3]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbSNR[0]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbSNR[1]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbSNR[2]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.AmbSNR[3]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.LagUsed[0]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.LagUsed[1]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.LagUsed[2]);
                sb.Append(",");
                sb.Append(ensemble.BottomTrackEngineeringData.LagUsed[3]);

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
                sb.Append(ensemble.ProfileEngineeringData.SamplesPerSecond);
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.SystemFreqHz);
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.LagSamples);
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.CPCE);
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.NCE);
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.RepeatN);
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.PrePingGap);
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.PrePingNCE);
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.PrePingRepeatN);
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.PrePingLagSamples);
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.TRHighGain);
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.PrePingVel[0]);
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.PrePingVel[1]);
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.PrePingVel[2]);
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.PrePingVel[3]);
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.PrePingCor[0]);
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.PrePingCor[1]);
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.PrePingCor[2]);
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.PrePingCor[3]);
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.PrePingAmp[0]);
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.PrePingAmp[1]);
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.PrePingAmp[2]);
                sb.Append(",");
                sb.Append(ensemble.ProfileEngineeringData.PrePingAmp[3]);

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
                sb.Append(ensemble.SystemSetupData.BtSamplesPerSecond);
                sb.Append(",");
                sb.Append(ensemble.SystemSetupData.BtSystemFreqHz);
                sb.Append(",");
                sb.Append(ensemble.SystemSetupData.BtCPCE);
                sb.Append(",");
                sb.Append(ensemble.SystemSetupData.BtNCE);
                sb.Append(",");
                sb.Append(ensemble.SystemSetupData.BtRepeatN);
                sb.Append(",");
                sb.Append(ensemble.SystemSetupData.WpSamplesPerSecond);
                sb.Append(",");
                sb.Append(ensemble.SystemSetupData.WpSystemFreqHz);
                sb.Append(",");
                sb.Append(ensemble.SystemSetupData.WpCPCE);
                sb.Append(",");
                sb.Append(ensemble.SystemSetupData.WpNCE);
                sb.Append(",");
                sb.Append(ensemble.SystemSetupData.WpRepeatN);
                sb.Append(",");
                sb.Append(ensemble.SystemSetupData.WpLagSamples);
                sb.Append(",");
                sb.Append(ensemble.SystemSetupData.Voltage);
            }
            else
            {
                sb.Append(",,,,,,,,,,,");
            }

            return sb.ToString();
        }

        #endregion

    }
}