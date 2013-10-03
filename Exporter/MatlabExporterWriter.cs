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
 * 10/02/2013      RC          2.20.2           Initial coding
 * 
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
    /// TODO: Update summary.
    /// </summary>
    public class MatlabExporterWriter : IExporterWriter
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

        #endregion

        /// <summary>
        /// Initialize the object.
        /// </summary>
        public MatlabExporterWriter()
        {
            _fileName = "";

            string myDoc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string DEFAULT_FOLDER_PATH = string.Format(@"{0}\RTI", myDoc);

            _filePath = DEFAULT_FOLDER_PATH;
        }

        
        /// <summary>
        /// Open the export to begin the writing process.
        /// The filename will be used to give the sub-filename.
        /// Files will be broken up by ensemble.
        /// </summary>
        /// <param name="filePath">File path to the write to.</param>
        /// <param name="fileName">Sub-Filename to use to for each file.</param>
        public void  Open(string filePath, string fileName)
        {
 	        //Set the file path and name
            _filePath = filePath;
            _fileName = fileName;
        }

        /// <summary>
        /// Each ensemble will have its own file.  So the filename will contain the subsystem and 
        /// ensemble number.
        /// </summary>
        /// <param name="ensemble"></param>
        /// <param name="isMultipleFile"></param>
        public void  Write(DataSet.Ensemble ensemble, bool isMultipleFile = false)
        {
            if (ensemble != null)
            {
                // Create a file name
                string filename = _filePath + _fileName;

                // Get the ensemble number and subsystem config
                if (ensemble.IsEnsembleAvail)
                {
                    string ensNum = ensemble.EnsembleData.EnsembleNumber.ToString("00000");
                    string subsys = String.Format("{0}_{1}", ensemble.EnsembleData.SubsystemConfig.CommandSetupToString(), ensemble.EnsembleData.SubsystemConfig.SubSystem.CodeToString());
                    filename += string.Format("_{0}_{1}", ensNum, subsys);
                }

                // Get the extension
                filename += ".mat";

                // Ony used for testing to force some datasets off
                //ensemble.IsEnsembleAvail = false;
                //ensemble.IsAmplitudeAvail = false;
                //ensemble.IsAncillaryAvail = false;
                //ensemble.IsBeamVelocityAvail = false;
                //ensemble.IsBottomTrackAvail = false;
                //ensemble.IsCorrelationAvail = false;
                //ensemble.IsEarthVelocityAvail = false;
                //ensemble.IsEarthWaterMassAvail = false;
                //ensemble.IsGoodBeamAvail = false;
                //ensemble.IsGoodEarthAvail = false;
                //ensemble.IsInstrumentVelocityAvail = false;
                //ensemble.IsInstrumentWaterMassAvail = false;
                //ensemble.IsNmeaAvail = false;
         
                File.WriteAllBytes(filename, ensemble.EncodeMatlab());
            }
        }

        /// <summary>
        /// Do nothing
        /// </summary>
        public void  Close()
        {
 	        
        }
    }
}
