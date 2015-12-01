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
 * 07/01/2013      RC          2.19       Initial coding
 * 07/02/2013      RC          2.19       Added ability to handle multiple files.
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using log4net;

    /// <summary>
    /// Codec built to import GPS data supplied by customer.
    /// This file will have the ensemble number and then
    /// the GPS data that goes with the ensemble.
    /// 
    /// Ex:
    /// #000001
    /// $GPGGA,072130.00,3035.40868085,N,12208.67288887,E,2,09,1.0,8.308,M,9.559,M,7.0,0137*78
    /// $GPVTG,0,T,0,M,0,N,0,K,0*N
    /// 
    /// $GPHDT,236.229,T*3B
    /// $HEROT,0.0,A*00
    /// $VXVY,-1.012,-0.695
    /// #000002
    /// $GPGGA,072131.00,3035.40863539,N,12208.67232697,E,2,09,1.0,8.233,M,9.559,M,7.0,0137*77
    /// $GPVTG,0,T,0,M,0,N,0,K,0*N
    ///
    /// $GPHDT,235.706,T*30
    /// $HEROT,0.0,A*00
    /// $VXVY,-0.954,-0.516
    /// #000003
    /// $GPGGA,072132.00,3035.40856441,N,12208.67176324,E,2,09,1.0,8.242,M,9.559,M,3.0,0137*70
    /// $GPVTG,0,T,0,M,0,N,0,K,0*N
    /// 
    /// $GPHDT,235.384,T*3E
    /// $HEROT,0.0,A*00
    /// $VXVY,-0.833,-0.662
    /// 
    /// </summary>
    public class ImportGpsText
    {
        #region Variables

        /// <summary>
        /// Setup logger to report errors.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        /// <summary>
        /// Initialize.
        /// </summary>
        public ImportGpsText()
        {

        }

        /// <summary>
        /// Decode multiple files.
        /// </summary>
        /// <param name="project">Project to add the decoded data to.</param>
        /// <param name="files">Files to decode.</param>
        public void DecodeFiles(Project project, string[] files)
        {
            try
            {
                // Go through each file given
                foreach (var filePath in files)
                {
                    // Read in the file
                    string text = System.IO.File.ReadAllText(filePath);

                    // Split all the data into groups that start with #
                    // This will be the begining of the ensemble line
                    // It will also remove the character
                    string[] ensGroup = text.Split('#');

                    // Decode each group and add it to the
                    // to the database
                    foreach (string grp in ensGroup)
                    {
                        DecodeGroup(grp, project);
                    }
                }

                // Publish the process is complete
                PublishCompleteEvent();
            }
            catch (Exception e)
            {
                log.Error(string.Format("Error decoding file {0}.", files), e);
            }
        }

        /// <summary>
        /// Decode the file with all the GPS data.
        /// </summary>
        /// <param name="project">Project to store the GPS data.</param>
        /// <param name="filePath">File path of the file to decode.</param>
        public void DecodeFile(Project project, string filePath)
        {
            try
            {
                // Read in the file
                string text = System.IO.File.ReadAllText(filePath);

                // Split all the data into groups that start with #
                // This will be the begining of the ensemble
                string[] ensGroup = text.Split('#');

                // Decode each group and add it to the
                // to the database
                foreach (string grp in ensGroup)
                {
                    DecodeGroup(grp, project);
                }

                // Publish the process is complete
                PublishCompleteEvent();
            }
            catch (Exception e)
            {
                log.Error(string.Format("Error decoding file {0}.", filePath), e);
            }
        }

        #region Decode File

        /// <summary>
        /// Decode the group into a line with the ensemble number
        /// and the rest is the GPS data.  Create a NmeaDataSet which
        /// will decode the GPS data and create the dataset.
        /// </summary>
        /// <param name="group">GPS data per ensemble.</param>
        /// <param name="project">Project to add the data to.</param>
        private void DecodeGroup(string group, Project project)
        {
            // Verify the first line contains the ensemble number
            string[] lines = group.Split('\n');

            // Verify data was found
            if (lines.Length > 0)
            {
                // Verify first line
                //if(lines[0].Contains('#'))
                //{
                    // Get the ensemble number
                    int ensNum = GetEnsNum(lines[0]);
                    
                    // Ensembles will always be greater than 0
                    if (ensNum > 0)
                    {
                        string gpsData = "";
                        
                        // Get the GPS data
                        // Start aver the first line
                        for (int x = 1; x < lines.Length; x++)
                        {
                            gpsData += lines[x];
                        }

                        // Create the dataset
                        DataSet.NmeaDataSet nds = new DataSet.NmeaDataSet(gpsData);

                        // Add the dataset to the ensemble in the project
                        project.UpdateNmeaDataSet(nds, ensNum);
                    }
                //}
            }
        }

        /// <summary>
        /// Get the ensemble number from the string.
        /// This will remove the # then try to parse the
        /// string into an int.
        /// If an error occurs, a 0 will be returned.
        /// </summary>
        /// <param name="line">Line to get the ensemble number.</param>
        /// <returns>Ensemble number.  </returns>
        private int GetEnsNum(string line)
        {
            int ensNum = 0;

            // Remove the #
            //string newEnsNum = line.Replace("#", "");

            // Try to parse the number
            int.TryParse(line, out ensNum);

            return ensNum;
        }

        #endregion

        #region Events

        #region Complete Event

        /// <summary>
        /// Event To subscribe to.  This gives the paramater
        /// that will be passed when subscribing to the event.
        /// </summary>
        public delegate void CompleteEventHandler();

        /// <summary>
        /// Subscribe to receive event when import is complete.
        /// 
        /// To subscribe:
        /// importer.CompleteEvent += new importer.CompleteEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// importer.CompleteEvent -= (method to call)
        /// </summary>
        public event CompleteEventHandler CompleteEvent;

        /// <summary>
        /// Publish the when import is complete.
        /// </summary>
        private void PublishCompleteEvent()
        {
            if (CompleteEvent != null)
            {
                CompleteEvent();
            }
        }

        #endregion

        #endregion
    }
}
