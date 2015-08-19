/*
 * Copyright © 2011 
 * Rowe Technology Inc.
 * All rights reserved.
 * http://www.rowetechinc.com
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification is NOT permitted.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
 * COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
 * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
 * POSSIBILITY OF SUCH DAMAGE.
 * 
 * HISTORY
 * -----------------------------------------------------------------
 * Date            Initials    Version    Comments
 * -----------------------------------------------------------------
 * 09/01/2011      RC                     Initial coding
 * 10/18/2011      RC                     Add Shutdown method  
 * 11/07/2011      RC                     Removed thread.  Added sleep in loop to prevent overflow.
 *                                        Fixed warnings in StyleCop.
 * 11/29/2011      RC                     Turn on recording then turn off to record imported data.
 * 12/29/2011      RC          1.11       Adding log and removing RecorderManager.
 * 01/10/2012      RC          1.12       Set IsImporting in RecorderManager when importing data.
 * 01/12/2012      RC          1.12       Call RecorderManager.ParseImportData() instead of ParseAdcpData() to prevent ping data and import data corrupting codec buffer.
 * 02/07/2012      RC          2.00       Removed the PropertyChanged event.
 * 06/21/2012      RC          2.12       Fixed bug where the final buffer of data read in was not processed.
 * 08/23/2012      RC          2.13       In Open(), check if the file is in use, if it is, give the user a message box.  Return a bool if the file could be opened.
 *                                         In ImportFiles(), if the file cannot be opened, do not continue processing that file.
 * 07/01/2013      RC          2.19       Removed the RecorderManager calls.
 * 01/16/2015      RC          3.0.2      Use the FilePlayback to read in the ensemble data.
 * 
 */

namespace RTI
{
    using System;
    using System.IO;
    using System.Threading;
    using log4net;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    

    /// <summary>
    /// This class will read in data from a binary file and pass it to
    /// the codec to parse the data.
    /// </summary>
    public class AdcpImportBinaryFile : IDisposable
    {
        #region Variables

        /// <summary>
        /// Setup logger to report errors.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///  Used to prevent importing from overflowing buffer (50 microseconds is conservative value.  Around 25 should be lowest)
        /// </summary>
        private const int IMPORT_SPEED_MICROSECONDS = 25;  //25

        /// <summary>
        /// Buffer size to read from a file
        /// </summary>
        private const int BUFFER_SIZE = 1028 * 5; //4096 1048576

        #endregion

        /// <summary>
        /// Initializes a new instance of the AdcpImportBinaryFile class.
        /// Initialize variables
        /// </summary>
        public AdcpImportBinaryFile()
        {

        }

        /// <summary>
        /// Import all the files given in the array.
        /// This will open the file.  Read in all the data
        /// and pass the data to the codec.  It will then
        /// close the file and move to the next file.
        /// </summary>
        /// <param name="filenames">Array of file names.</param>
        /// <returns>List of all the ensembles found in the file.</returns>
        public List<FilePlayback.EnsembleData> ImportFiles(string[] filenames)
        {
            try
            {
                // Read in all the information from the file
                using (FilePlayback playback = new FilePlayback())
                {
                    Task.Run(() => playback.FindEnsembles(filenames));

                    // Send an event when complete
                    PublishCompleteEvent();

                    // Return all the ensemble data found
                    return playback.GetEnsembleDataList();
                }
            }
            catch (Exception e)
            {
                log.Error(string.Format("Error Importing the files: {0}", filenames), e);
                return new List<FilePlayback.EnsembleData>();
            }
        }

        /// <summary>
        /// Stop the thread and close the file.
        /// </summary>
        public void Dispose()
        {

        }

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