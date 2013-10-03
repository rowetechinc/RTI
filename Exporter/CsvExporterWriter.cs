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
 * 08/26/2013      RC          2.20.2          Initial coding
 * 
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

        #endregion

        /// <summary>
        /// Initializes a new instance of the CsvExporterWriter class.
        /// </summary>
        public CsvExporterWriter()
        {

        }

        /// <summary>
        /// Open the file given and add the CSV header.
        /// </summary>
        /// <param name="filePath">File path to export the file to.</param>
        /// <param name="fileName">File Name to create.</param>
        public void Open(string filePath, string fileName)
        {
            // Create the file name with file path
            string file = filePath + fileName;

            // If the writer exist, make sure it is closed
            if (_writer != null)
            {
                _writer.Close();
            }

            _writer = new StreamWriter(file, false);

            StringBuilder sb = new StringBuilder();
            sb.Append("Ensemble Number,");
            sb.Append("Date,");
            sb.Append("Hours,");
            sb.Append("Minutes,");
            sb.Append("Seconds,");
            sb.Append("HunSeconds,");
            sb.Append("TotalSeconds,");
            sb.Append("Pressure,");
            sb.Append("Heading,");
            sb.Append("Pitch,");
            sb.Append("Roll,");
            sb.Append("Instr Bin0 Beam0,");
            sb.Append("Instr Bin0 Beam1,");
            sb.Append("Instr Bin0 Beam2,");
            sb.Append("Instr Bin0 Beam3,");

            sb.Append("Instr Bin1 Beam0,");
            sb.Append("Instr Bin1 Beam1,");
            sb.Append("Instr Bin1 Beam2,");
            sb.Append("Instr Bin1 Beam3,");

            sb.Append("Instr Bin2 Beam0,");
            sb.Append("Instr Bin2 Beam1,");
            sb.Append("Instr Bin2 Beam2,");
            sb.Append("Instr Bin2 Beam3,");

            sb.Append("Earth Bin0 Beam0,");
            sb.Append("Earth Bin0 Beam1,");
            sb.Append("Earth Bin0 Beam2,");
            sb.Append("Earth Bin0 Beam3,");

            sb.Append("Earth Bin1 Beam0,");
            sb.Append("Earth Bin1 Beam1,");
            sb.Append("Earth Bin1 Beam2,");
            sb.Append("Earth Bin1 Beam3,");

            sb.Append("Earth Bin2 Beam0,");
            sb.Append("Earth Bin2 Beam1,");
            sb.Append("Earth Bin2 Beam2,");
            sb.Append("Earth Bin2 Beam3,");



            _writer.WriteLine(sb.ToString());
        }

        /// <summary>
        /// Write the Ensemble number, Ensemble date and time, the velocity data, the pressure, heading pitch and roll
        /// to the file.
        /// </summary>
        /// <param name="ensemble">Ensemble to get the data.</param>
        /// <param name="isMultipleFiles">Set if each ensemble should be a seperate file.  Default to FALSE.</param>
        public void Write(Ensemble ensemble, bool isMultipleFiles = false)
        {
            if (ensemble != null)
            {
                StringBuilder sb = new StringBuilder();

                if (ensemble.IsEnsembleAvail)
                {
                    sb.Append(string.Format("{0},", ensemble.EnsembleData.EnsembleNumber));
                    sb.Append(string.Format("{0}/{1}/{2},", ensemble.EnsembleData.Year.ToString("0000"), ensemble.EnsembleData.Month.ToString("00"), ensemble.EnsembleData.Day.ToString("00")));
                    sb.Append(string.Format("{0},", ensemble.EnsembleData.Hour.ToString("00")));
                    sb.Append(string.Format("{0},", ensemble.EnsembleData.Minute.ToString("00")));
                    sb.Append(string.Format("{0},", ensemble.EnsembleData.Second.ToString("00")));
                    sb.Append(string.Format("{0},", ensemble.EnsembleData.HSec.ToString("00")));
                    double totalSeconds = (ensemble.EnsembleData.Hour * 3600.0) + (ensemble.EnsembleData.Minute * 60.0) + (ensemble.EnsembleData.Second) + (ensemble.EnsembleData.HSec / 100.0);
                    sb.Append(string.Format("{0},", totalSeconds.ToString("0.000")));
                }
                else
                {
                    sb.Append(",,,,,,,");
                }

                if (ensemble.IsAncillaryAvail)
                {
                    sb.Append(string.Format("{0},", ensemble.AncillaryData.Pressure));
                    sb.Append(string.Format("{0},", ensemble.AncillaryData.Heading));
                    sb.Append(string.Format("{0},", ensemble.AncillaryData.Pitch));
                    sb.Append(string.Format("{0},", ensemble.AncillaryData.Roll));
                }
                else
                {
                    sb.Append(",,,,");
                }

                if (ensemble.IsInstrumentVelocityAvail)
                {
                    sb.Append(string.Format("{0},", ensemble.InstrumentVelocityData.InstrumentVelocityData[0, 0]));
                    sb.Append(string.Format("{0},", ensemble.InstrumentVelocityData.InstrumentVelocityData[0, 1]));
                    sb.Append(string.Format("{0},", ensemble.InstrumentVelocityData.InstrumentVelocityData[0, 2]));
                    sb.Append(string.Format("{0},", ensemble.InstrumentVelocityData.InstrumentVelocityData[0, 3]));

                    sb.Append(string.Format("{0},", ensemble.InstrumentVelocityData.InstrumentVelocityData[1, 0]));
                    sb.Append(string.Format("{0},", ensemble.InstrumentVelocityData.InstrumentVelocityData[1, 1]));
                    sb.Append(string.Format("{0},", ensemble.InstrumentVelocityData.InstrumentVelocityData[1, 2]));
                    sb.Append(string.Format("{0},", ensemble.InstrumentVelocityData.InstrumentVelocityData[1, 3]));

                    sb.Append(string.Format("{0},", ensemble.InstrumentVelocityData.InstrumentVelocityData[2, 0]));
                    sb.Append(string.Format("{0},", ensemble.InstrumentVelocityData.InstrumentVelocityData[2, 1]));
                    sb.Append(string.Format("{0},", ensemble.InstrumentVelocityData.InstrumentVelocityData[2, 2]));
                    sb.Append(string.Format("{0},", ensemble.InstrumentVelocityData.InstrumentVelocityData[2, 3]));
                }
                else
                {
                    sb.Append(",,,,,,,,,,,,");
                }

                if (ensemble.IsEarthVelocityAvail)
                {
                    sb.Append(string.Format("{0},", ensemble.EarthVelocityData.EarthVelocityData[0, 0]));
                    sb.Append(string.Format("{0},", ensemble.EarthVelocityData.EarthVelocityData[0, 1]));
                    sb.Append(string.Format("{0},", ensemble.EarthVelocityData.EarthVelocityData[0, 2]));
                    sb.Append(string.Format("{0},", ensemble.EarthVelocityData.EarthVelocityData[0, 3]));

                    sb.Append(string.Format("{0},", ensemble.EarthVelocityData.EarthVelocityData[1, 0]));
                    sb.Append(string.Format("{0},", ensemble.EarthVelocityData.EarthVelocityData[1, 1]));
                    sb.Append(string.Format("{0},", ensemble.EarthVelocityData.EarthVelocityData[1, 2]));
                    sb.Append(string.Format("{0},", ensemble.EarthVelocityData.EarthVelocityData[1, 3]));

                    sb.Append(string.Format("{0},", ensemble.EarthVelocityData.EarthVelocityData[2, 0]));
                    sb.Append(string.Format("{0},", ensemble.EarthVelocityData.EarthVelocityData[2, 1]));
                    sb.Append(string.Format("{0},", ensemble.EarthVelocityData.EarthVelocityData[2, 2]));
                    sb.Append(string.Format("{0},", ensemble.EarthVelocityData.EarthVelocityData[2, 3]));
                }
                else
                {
                    sb.Append(",,,,,,,,,,,,");
                }

                _writer.WriteLine(sb.ToString());
            }
        }

        /// <summary>
        /// Close the file.
        /// </summary>
        public void Close()
        {
            _writer.Close();
            _writer.Dispose();
        }
    }
}