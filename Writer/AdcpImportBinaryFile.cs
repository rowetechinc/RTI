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
 * 
 */

namespace RTI
{
    using System;
    using System.IO;
    using System.Threading;
    using log4net;
    using System.Diagnostics;
    

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

        /// <summary>
        /// Read the binary file.
        /// </summary>
        private BinaryReader _binReader;

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
        public void ImportFiles(string[] filenames)
        {
            // Turn on record mode to record data to database file
            // Also set that importing data
            //RecorderManager.Instance.IsRecording = true;
            //RecorderManager.Instance.IsImporting = true;

            try
            {
                // Process each file
                foreach (string file in filenames)
                {
                    // Try to open the file
                    if (this.Open(file))
                    {
                        int pos = 0;
                        long fileLength = this._binReader.BaseStream.Length;

                        // Read in all the data from the file
                        // Read in only a small buffer amount
                        // at a time, so the codec can process
                        // the data in parallel
                        while (pos <= fileLength)
                        {
                            // Read a small amount from the buffer
                            byte[] input = this._binReader.ReadBytes(BUFFER_SIZE);

                            // Add the data from the file to the codec to be processed
                            PublishBinaryData(input);

                            // Advance the position in the file
                            pos += input.Length;


                            // If nothing is read, then reading is complete
                            if (input.Length == 0 && pos >= fileLength)
                            {
                                break;
                            }

                            // Pause the thread so that it
                            // does not read in data to quickly
                            // and overflow the buffer for parsing
                            // the data.
                            // 50 microseconds is a conservative value
                            Thread.Sleep(IMPORT_SPEED_MICROSECONDS);
                        }

                        // Close the file when complete
                        this.CloseFile();
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(string.Format("Error Importing the files: {0}", filenames), e);

            }

            // Turn off record mode
            // Now the user must decide when to record
            //RecorderManager.Instance.IsRecording = false;
            //RecorderManager.Instance.IsImporting = false;

            // Send an event when complete
            PublishCompleteEvent();
        }

        /// <summary>
        /// Stop the read thread.  Then close the
        /// stream to the file.
        /// </summary>
        public void Close()
        {
            this.CloseFile();
        }

        /// <summary>
        /// Stop the thread and close the file.
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Restart reading the file.  If the file
        /// needs to be changed, call this method.
        /// This will also clear the buffer in the
        /// codec to ensure only new data is displayed.
        /// </summary>
        /// <param name="filenames">List of file names.</param>
        public void Restart(string[] filenames)
        {
            // Close the current file
            this.CloseFile();

            // Clear the buffer
            //RecorderManager.Instance.ClearAdcpBuffer();

            // Start to read the new files
            this.ImportFiles(filenames);
        }

        #region Events

        #region Binary Data Event

        /// <summary>
        /// Event To subscribe to.  This gives the paramater
        /// that will be passed when subscribing to the event.
        /// </summary>
        /// <param data="data">Imported binary data.</param>
        public delegate void ReceiveBinaryDataEventHandler(byte[] data);

        /// <summary>
        /// Subscribe to receive the latest binary data imported from the file.
        /// 
        /// To subscribe:
        /// importer.ReceiveBinaryDataEvent += new importer.ReceiveBinaryDataEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// importer.ReceiveBinaryDataEvent -= (method to call)
        /// </summary>
        public event ReceiveBinaryDataEventHandler ReceiveBinaryDataEvent;

        /// <summary>
        /// Publish the latest binary data from the imported file.
        /// </summary>
        /// <param name="data">Data to publish.</param>
        private void PublishBinaryData(byte[] data)
        {
            if (ReceiveBinaryDataEvent != null)
            {
                ReceiveBinaryDataEvent(data);
            }
        }

        #endregion

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

        #region Private Methods

        /// <summary>
        /// Open the file.  Verify the file can be opened
        /// by wrapping around it around a try/catch.  If 
        /// the file can be opened, create a stream to
        /// the file and start the thread to begin reading
        /// the file.
        /// </summary>
        /// <param name="filename">File name.</param>
        /// <returns>TRUE = File is open.  / FALSE = File could not be opened.</returns>
        private bool Open(string filename)
        {
            try
            {
                // Open the file and get the file length
                this._binReader = new BinaryReader(File.Open(filename, FileMode.Open));
            }
            catch (System.IO.IOException)
            {
                log.Error(string.Format("Import Error: File {0} is in use.", filename));
                //MessageBox.Show(string.Format("File {0} is in use.", filename), "Import Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            catch (Exception e)
            {
                log.Error("Error Opening file for playback: " + filename, e);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Close the binary reader.
        /// This will close the file.
        /// </summary>
        private void CloseFile()
        {
            // Stop the binary reader
            if (this._binReader != null)
            {
                this._binReader.Close();
                this._binReader.Dispose();
            }
        }

        #endregion
    }
}