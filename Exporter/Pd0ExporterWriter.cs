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
 * 04/02/2014      RC          2.21.4     Initial coding
 * 05/01/2014      RC          3.2.4      Select which datasets to export to the PD0 file.
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
    using System.IO;

    /// <summary>
    /// Export the data to PD0 format.
    /// </summary>
    public class Pd0ExporterWriter : IExporterWriter
    {
        #region Variables

        /// <summary>
        /// Sub-file name to be used for each
        /// ensemble file.
        /// </summary>
        private string _fileName;

        /// <summary>
        /// Folder path.
        /// </summary>
        private string _filePath;

        /// <summary>
        /// Export options.
        /// </summary>
        private ExportOptions _options;

        /// <summary>
        /// Write to the file using this writer.
        /// </summary>
        private BinaryWriter _writer;

        /// <summary>
        /// Current size of the file.
        /// </summary>
        private int _fileSize;

        #endregion

        /// <summary>
        /// Initialize the object.
        /// </summary>
        public Pd0ExporterWriter()
        {

        }

        /// <summary>
        /// Open the export to begin the writing process.
        /// The filename will be used to give the sub-filename.
        /// Files will be broken up by ensemble.
        /// </summary>
        /// <param name="filePath">File path to the write to.</param>
        /// <param name="fileName">Sub-Filename to use to for each file.</param>
        /// <param name="options">Export options.</param>
        public void Open(string filePath, string fileName, ExportOptions options)
        {
            //Set the file path and name
            _filePath = filePath;
            _fileName = fileName;
            _options = options;
            _fileSize = 0;

            int timeout = 0;
            string origFilePath = _filePath;
            while (File.Exists(_filePath + _fileName))
            {
                _filePath = origFilePath + @"\" + timeout.ToString() + "_";

                timeout++;

                if(timeout > 10)
                {
                    break;
                }
            }

            _writer = new BinaryWriter(File.Open(_filePath + _fileName, FileMode.Append, FileAccess.Write));
        }

        /// <summary>
        /// Each ensemble will have its own file.  So the filename will contain the subsystem and 
        /// ensemble number.
        /// </summary>
        /// <param name="ensemble">Ensemble to encode to PD0.</param>
        /// <param name="isMultipleFile">Set flag if you want individual files per ensemble or combine it all into one file.</param>
        public void Write(DataSet.Ensemble ensemble, bool isMultipleFile = false)
        {
            if (ensemble == null)
            {
                return;
            }

            // Check Amplitude 
            if (!_options.IsAmplitudeDataSetOn) { ensemble.IsAmplitudeAvail = false; }
            else
            {
                // Verify the dataset exist
                if (ensemble.IsAmplitudeAvail)
                {
                    // Verify a change in the bins
                    if (_options.AmplitudeMinBin != 0 || _options.AmplitudeMaxBin != 0)
                    {
                        // Verify the max bin
                        if (_options.AmplitudeMaxBin > ensemble.EnsembleData.NumBins - 1)
                        {
                            _options.AmplitudeMaxBin = (byte)(ensemble.EnsembleData.NumBins - 1);
                        }

                        // If min or max bin has been set, then get the new number of bins
                        int numBins = (_options.AmplitudeMaxBin - _options.AmplitudeMinBin) + 1;

                        // Truncate the data
                        float[,] tempAmp = ensemble.AmplitudeData.AmplitudeData;
                        ensemble.AmplitudeData.AmplitudeData = new float[numBins, tempAmp.GetLength(1)];
                        Buffer.BlockCopy(tempAmp, _options.AmplitudeMinBin, ensemble.AmplitudeData.AmplitudeData, 0, numBins);
                    }
                }
                else
                {
                    // Set that data set was not used
                    _options.IsAmplitudeDataSetOn = false;
                }
            }

            // Check Beam Velocity
            if (!_options.IsBeamVelocityDataSetOn) { ensemble.IsBeamVelocityAvail = false; }
            else
            {
                // Verify the dataset exist
                if (ensemble.IsBeamVelocityAvail)
                {
                    // Verify a change in the bins
                    if (_options.BeamMinBin != 0 || _options.BeamMaxBin != 0)
                    {
                        // Verify the max bin
                        if (_options.BeamMaxBin > ensemble.EnsembleData.NumBins - 1)
                        {
                            _options.BeamMaxBin = (byte)(ensemble.EnsembleData.NumBins - 1);
                        }

                        // If min or max bin has been set, then get the new number of bins
                        int numBins = (_options.BeamMaxBin - _options.BeamMinBin) + 1;

                        // Truncate the data
                        float[,] tempBeam = ensemble.BeamVelocityData.BeamVelocityData;
                        ensemble.BeamVelocityData.BeamVelocityData = new float[numBins, tempBeam.GetLength(1)];
                        Buffer.BlockCopy(tempBeam, _options.BeamMinBin, ensemble.BeamVelocityData.BeamVelocityData, 0, numBins);
                    }
                }
                else
                {
                    // Set that data set was not used
                    _options.IsBeamVelocityDataSetOn = false;
                }
            }

            // Check Earth Velocity 
            if (!_options.IsEarthVelocityDataSetOn) { ensemble.IsEarthVelocityAvail = false; }
            else
            {
                // Verify the dataset exist
                if (ensemble.IsEarthVelocityAvail)
                {
                    // Verify a change in the bins
                    if (_options.EarthMinBin != 0 || _options.EarthMaxBin != 0)
                    {
                        // Verify the max bin
                        if (_options.EarthMaxBin > ensemble.EnsembleData.NumBins - 1)
                        {
                            _options.EarthMaxBin = (byte)(ensemble.EnsembleData.NumBins - 1);
                        }

                        // If min or max bin has been set, then get the new number of bins
                        int numBins = (_options.EarthMaxBin - _options.EarthMinBin) + 1;

                        // Truncate the data
                        float[,] tempEarth = ensemble.EarthVelocityData.EarthVelocityData;
                        ensemble.EarthVelocityData.EarthVelocityData = new float[numBins, tempEarth.GetLength(1)];
                        Buffer.BlockCopy(tempEarth, _options.EarthMinBin, ensemble.EarthVelocityData.EarthVelocityData, 0, numBins);
                    }
                }
                else
                {
                    // Set that data set was not used
                    _options.IsEarthVelocityDataSetOn = false;
                }
            }

            // Check Instrument Velocity 
            if (!_options.IsInstrumentVelocityDataSetOn) { ensemble.IsInstrumentVelocityAvail = false; }
            else
            {
                // Verify the dataset exist
                if (ensemble.IsInstrumentVelocityAvail)
                {
                    // Verify a change in the bins
                    if (_options.InstrumentMinBin != 0 || _options.InstrumentMaxBin != 0)
                    {
                        // Verify the max bin
                        if (_options.InstrumentMaxBin > ensemble.EnsembleData.NumBins - 1)
                        {
                            _options.InstrumentMaxBin = (byte)(ensemble.EnsembleData.NumBins - 1);
                            
                        }

                        // If min or max bin has been set, then get the new number of bins
                        int numBins = (_options.InstrumentMaxBin - _options.InstrumentMinBin) + 1;

                        // Truncate the data
                        float[,] tempInstr = ensemble.InstrumentVelocityData.InstrumentVelocityData;
                        ensemble.InstrumentVelocityData.InstrumentVelocityData = new float[numBins, tempInstr.GetLength(1)];
                        Buffer.BlockCopy(tempInstr, _options.InstrumentMinBin, ensemble.InstrumentVelocityData.InstrumentVelocityData, 0, numBins);
                    }
                }
                else
                {
                    // Set that data set was not used
                    _options.IsInstrumentVelocityDataSetOn = false;
                }
            }

            // Check Correlation 
            if (!_options.IsCorrelationDataSetOn) { ensemble.IsCorrelationAvail = false; }
            else
            {
                // Verify the dataset exist
                if (ensemble.IsCorrelationAvail)
                {
                    // Verify a change in the bins
                    if (_options.CorrelationMinBin != 0 || _options.CorrelationMaxBin != 0)
                    {
                        // Verify the max bin
                        if (_options.CorrelationMaxBin > ensemble.EnsembleData.NumBins - 1)
                        {
                            _options.CorrelationMaxBin = (byte)(ensemble.EnsembleData.NumBins - 1);
                        }

                        // If min or max bin has been set, then get the new number of bins
                        int numBins = (_options.CorrelationMaxBin - _options.CorrelationMinBin) + 1;

                        // Truncate the data
                        float[,] tempCorr = ensemble.CorrelationData.CorrelationData;
                        ensemble.CorrelationData.CorrelationData = new float[numBins, tempCorr.GetLength(1)];
                        Buffer.BlockCopy(tempCorr, _options.CorrelationMinBin, ensemble.CorrelationData.CorrelationData, 0, numBins);
                    }
                }
                else
                {
                    // Set that data set was not used
                    _options.IsCorrelationDataSetOn = false;
                }
            }

            // Check Good Beam 
            if (!_options.IsGoodBeamDataSetOn) { ensemble.IsGoodBeamAvail = false; }
            else
            {
                // Verify the dataset exist
                if (ensemble.IsGoodBeamAvail)
                {
                    // Verify a change in the bins
                    if (_options.GoodBeamMinBin != 0 || _options.GoodBeamMaxBin != 0)
                    {
                        // Verify the max bin
                        if (_options.GoodBeamMaxBin > ensemble.EnsembleData.NumBins - 1)
                        {
                            _options.GoodBeamMaxBin = (byte)(ensemble.EnsembleData.NumBins - 1);
                        }

                        // If min or max bin has been set, then get the new number of bins
                        int numBins = (_options.GoodBeamMaxBin - _options.GoodBeamMinBin) + 1;

                        // Truncate the data
                        int[,] tempGB = ensemble.GoodBeamData.GoodBeamData;
                        ensemble.GoodBeamData.GoodBeamData = new int[numBins, tempGB.GetLength(1)];
                        Buffer.BlockCopy(tempGB, _options.GoodBeamMinBin, ensemble.GoodBeamData.GoodBeamData, 0, numBins);
                    }
                }
                else
                {
                    // Set that data set was not used
                    _options.IsGoodBeamDataSetOn = false;
                }
            }

            // Check Good Earth 
            if (!_options.IsGoodEarthDataSetOn) { ensemble.IsGoodEarthAvail = false; }
            else
            {
                // Verify the dataset exist
                if (ensemble.IsGoodEarthAvail)
                {
                    // Verify a change in the bins
                    if (_options.GoodEarthMinBin != 0 || _options.GoodEarthMaxBin != 0)
                    {
                        // Verify the max bin
                        if (_options.GoodEarthMaxBin > ensemble.EnsembleData.NumBins - 1)
                        {
                            _options.GoodEarthMaxBin = (byte)(ensemble.EnsembleData.NumBins - 1);
                        }

                        // If min or max bin has been set, then get the new number of bins
                        int numBins = (_options.GoodEarthMaxBin - _options.GoodEarthMinBin) + 1;

                        // Truncate the data
                        int[,] tempGB = ensemble.GoodEarthData.GoodEarthData;
                        ensemble.GoodEarthData.GoodEarthData = new int[numBins, tempGB.GetLength(1)];
                        Buffer.BlockCopy(tempGB, _options.GoodEarthMinBin, ensemble.GoodEarthData.GoodEarthData, 0, numBins);
                    }
                }
                else
                {
                    // Set that data set was not used
                    _options.IsGoodEarthDataSetOn = false;
                }
            }

            // Check Bottom Track 
            if (!_options.IsBottomTrackDataSetOn) { ensemble.IsBottomTrackAvail = false; }
            else
            {
                if (!ensemble.IsBottomTrackAvail)
                {
                    // Set that data set was not used
                    _options.IsBottomTrackDataSetOn = false;
                }
            }

            // Check Earth Water Mass 
            if (!_options.IsEarthWaterMassDataSetOn) { ensemble.IsEarthWaterMassAvail = false; }
            else
            {
                if (!ensemble.IsEarthWaterMassAvail)
                {
                    // Set that data set was not used
                    _options.IsEarthWaterMassDataSetOn = false;
                }
            }

            // Check Instrument Water Mass 
            if (!_options.IsInstrumentWaterMassDataSetOn) { ensemble.IsInstrumentWaterMassAvail = false; }
            else
            {
                if (!ensemble.IsInstrumentWaterMassAvail)
                {
                    // Set that data set was not used
                    _options.IsInstrumentWaterMassDataSetOn = false;
                }
            }

            // Check Nmea Data 
            if (!_options.IsNmeaDataSetOn) { ensemble.IsNmeaAvail = false; }
            else
            {
                if (!ensemble.IsNmeaAvail)
                {
                    // Set that data set was not used
                    _options.IsNmeaDataSetOn = false;
                }
            }

            // Check Profile Engineering 
            if (!_options.IsProfileEngineeringDataSetOn) { ensemble.IsProfileEngineeringAvail = false; }
            else
            {
                if (!ensemble.IsProfileEngineeringAvail)
                {
                    // Set that data set was not used
                    _options.IsProfileEngineeringDataSetOn = false;
                }
            }

            // Check System Setup 
            if (!_options.IsSystemSetupDataSetOn) { ensemble.IsSystemSetupAvail = false; }
            else
            {
                if (!ensemble.IsSystemSetupAvail)
                {
                    // Set that data set was not used
                    _options.IsSystemSetupDataSetOn = false;
                }
            }

            // Check ADCP GPS 
            if (!_options.IsAdcpGpsDataSetOn) { ensemble.IsAdcpGpsDataAvail = false; }
            else
            {
                if (!ensemble.IsAdcpGpsDataAvail)
                {
                    // Set that data set was not used
                    _options.IsAdcpGpsDataSetOn = false;
                }
            }

            // Check GPS 1 
            if (!_options.IsGps1DataSetOn) { ensemble.IsGps1DataAvail = false; }
            else
            {
                if (!ensemble.IsGps1DataAvail)
                {
                    // Set that data set was not used
                    _options.IsGps1DataSetOn = false;
                }
            }

            // Check GPS 2 
            if (!_options.IsGps2DataSetOn) { ensemble.IsGps2DataAvail = false; }
            else
            {
                if (!ensemble.IsGps2DataAvail)
                {
                    // Set that data set was not used
                    _options.IsGps2DataSetOn = false;
                }
            }

            // Check NMEA 1 
            if (!_options.IsNmea1DataSetOn) { ensemble.IsNmea1DataAvail = false; }
            else
            {
                if (!ensemble.IsNmea1DataAvail)
                {
                    // Set that data set was not used
                    _options.IsNmea1DataSetOn = false;
                }
            }

            // Check NMEA 2 
            if (!_options.IsNmea2DataSetOn) { ensemble.IsNmea2DataAvail = false; }
            else
            {
                if (!ensemble.IsNmea2DataAvail)
                {
                    // Set that data set was not used
                    _options.IsNmea2DataSetOn = false;
                }
            }

            // Write the data to the file
            _writer.Write(ensemble.EncodePd0Ensemble(_options.CoordinateTransform));
        }

        /// <summary>
        /// Close the writer when completed.
        /// </summary>
        /// <returns>Return the options.</returns>
        public ExportOptions Close()
        {
            // If the writer exist, make sure it is closed
            if (_writer != null)
            {
                _writer.Close();
                _writer.Dispose();
            }

            return _options;
        }

        #region Write

        /// <summary>
        /// Write the incoming data to the file.  If the file 
        /// reaches the maximum file size, close the file and
        /// create a new file with a new index.
        /// </summary>
        /// <param name="data">Data to write to the file.</param>
        private void Write(byte[] data)
        {
            // Check if the file will exceed the max size
            if (data.Length + _fileSize > _options.MaxFileSize)
            {
                //// Flush the current file
                //Flush();

                //// Generate new file name
                //ResetFileName();
            }

            try
            {
                // Write the data to the file
                _writer.Write(data);
            }
            catch (Exception) { }

            // Keep track of the file size
            _fileSize += data.Length;

            // Publish that the ensembles have been written to the file
            //PublishEnsembleWrite(_fileSize);
        }

        #endregion
    }
}
