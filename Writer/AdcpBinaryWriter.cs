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
 * 09/01/2011      RC                     Initial coding
 * 10/04/2011      RC                     Adding IsRecording prop.  Used to determine
 *                                         if creating new file or not.
 * 10/11/2011      RC                     Buffer data before writing
 * 10/18/2011      RC                     Properly shutdown object
 * 11/07/20111     RC                     If there is data in the buffer, write it to file before closing.
 *                                         Close binary reader when closing.
 * 11/14/2011      RC                     Change foreach loop in WriteBuffer() to for statement.  Shutdown complained about buffer.
 * 11/30/2011      RC                     Make file open and close occur only in one spot.
 *                                         Remove Close() and add Flush().
 *                                         Change files when new project is selected.
 * 12/12/2011      RC          1.09       Use max file to determine the size of the file.
 * 12/15/2011      RC          1.10       Fix bug where if mission not selected, name cannot be created.
 * 12/29/2011      RC          1.11       Adding log and removing RecorderManager.
 *                                         Verify there is a project to write to before trying to buffer incoming data.
 * 02/14/2012      RC          2.03       Moved file extension to Commns.cs.
 * 02/17/2012      RC          2.03       Set the file size to 0 when getting a new file name.
 * 01/24/2013      RC          2.18       Made FileSize a public property to monitor the recording.
 * 06/17/2013      RC          2.19       Set a default MaxFileSize.  Add a constructor that takes a project.
 *                                         Added serialnumber to the file name.
 * 06/28/2013      RC          2.19       Replaced Shutdown() with IDisposable.
 * 07/26/2013      RC          2.19.3     Added EnsembleWriteEvent event to send when an ensemble has been written to the file.
 * 08/19/2013      RC          2.19.4     Moved the PublishEnsembleWrite() to AddIncomingData() so that it will display updates everytime it receives data.
 * 10/10/2013      RC          2.21.0     Made MaxFileSize a public property.
 * 10/14/2013      RC          2.21.0     Changed contructor to take individual name, folder and serial number to a project will not be needed.
 * 01/31/2014      RC          2.21.3     Added filetype to know what type of binary file is being created.  This will set the file name and extension.
 * 04/09/2014      RC          2.21.4     Changed _writeBuffer from a list to ConcurrentQueue.
 * 04/16/2015      RC          3.0.4      Improved processing large files and fast data.
 *       
 * 
 */

using System.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using log4net;
using System.Collections.Concurrent;

namespace RTI
{
    /// <summary>
    /// Take the ADCP binary data and
    /// write it to file.  This is used
    /// to write the binary data received
    /// from the ADCP serial port and
    /// write it to a file.
    /// </summary>
    public class AdcpBinaryWriter: IDisposable
    {
        #region Enum and Classes

        /// <summary>
        /// File type that will be written.
        /// This will determine the file name and
        /// the file extension.
        /// </summary>
        public enum FileType
        {
            /// <summary>
            /// Writing ensembles.
            /// </summary>
            Ensemble,

            /// <summary>
            /// GPS 1 data.
            /// </summary>
            GPS1,

            /// <summary>
            /// GPS 2 data.
            /// </summary>
            GPS2,

            /// <summary>
            /// NMEA 1 data.
            /// </summary>
            NMEA1,

            /// <summary>
            /// NMEA 2 data.
            /// </summary>
            NMEA2,

            /// <summary>
            /// Short Term Averaged data.
            /// </summary>
            STA,

            /// <summary>
            /// Long Term Averaged data.
            /// </summary>
            LTA
        }

        #endregion

        #region Variables

        /// <summary>
        /// Setup logger to report errors.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Default size of a binary file.
        /// 16MB
        /// </summary>
        public const long DEFAULT_BINARY_FILE_SIZE = 1048576 * 16;

        /// <summary>
        /// A buffer will be used to write
        /// large amounts of data to the file
        /// instead of writing small chunks.
        /// </summary>
        private const long MAX_BUFFER_SIZE = 1048576 * 1;   // 1 MB
    
        /// <summary>
        /// Buffer to store large amounts of data.
        /// This will be a list of byte arrays.
        /// As data is added, the array will be
        /// added to the list.  When it needs to
        /// be written to file, all the arrays
        /// will be written to the file.
        /// </summary>
        private ConcurrentQueue<byte[]> _writeBuffer;

        /// <summary>
        /// Index for the write buffer.
        /// </summary>
        private long _writeBufferIndex;

        /// <summary>
        /// File name.
        /// </summary>
        private string _fileName;

        /// <summary>
        /// Index to use to seperate file names.
        /// </summary>
        private int _fileNameIndex = 0;

        /// <summary>
        /// File type for this file.
        /// </summary>
        private FileType _fileType;

        /// <summary>
        /// Writer to write binary data to
        /// a file.
        /// </summary>
        BinaryWriter _binWriter = null;

        /// <summary>
        /// A lock for the file when writing.
        /// </summary>
        public object _FileLock = new object();

        /// <summary>
        /// Flag if we are processing the data.
        /// </summary>
        public bool _isProcessingFile;

        #endregion

        #region Properties

        /// <summary>
        /// Name to use for each file.
        /// </summary>
        private string _ProjectName;
        /// <summary>
        /// Name to use for each file.
        /// </summary>
        public string ProjectName
        {
            get { return _ProjectName; }
            set
            {
                _ProjectName = value;
            }
        }

        /// <summary>
        /// Folder path for the data.
        /// </summary>
        private string _ProjectFolderPath;
        /// <summary>
        /// Folder path for the data.
        /// </summary>
        public string ProjectFolderPath
        {
            get { return _ProjectFolderPath; }
            set
            {
                _ProjectFolderPath = value;
            }
        }

        /// <summary>
        /// Serial number for this project.
        /// </summary>
        private string _SerialNumber;
        /// <summary>
        /// Serial number for this project.
        /// </summary>
        public string SerialNumber
        {
            get { return _SerialNumber; }
            set
            {
                _SerialNumber = value;
            }
        }

        /// <summary>
        /// Current size of the file to ensure
        /// it does not exceed a max value.  This can be 
        /// used to monitor the current file size.  
        /// This will calculate what is currently in the buffer
        /// and what has already been written to the file.
        /// </summary>
        private long _fileSize = 0;
        /// <summary>
        /// Current size of the file to ensure
        /// it does not exceed a max value.  This can be 
        /// used to monitor the current file size.  
        /// This will calculate what is currently in the buffer
        /// and what has already been written to the file.
        /// </summary>
        public long FileSize
        {
            get { return _fileSize + _writeBufferIndex; }
        }

        /// <summary>
        /// Maximum file size in bytes.
        /// When the file reaches the maximum size,
        /// a new file will be created and the current
        /// name indexed by 1.
        /// </summary>
        private long _MaxFileSize;
        /// <summary>
        /// Maximum file size in bytes.
        /// When the file reaches the maximum size,
        /// a new file will be created and the current
        /// name indexed by 1.
        /// </summary>
        public long MaxFileSize 
        {
            get { return _MaxFileSize; }
            set
            {
                _MaxFileSize = value;
            }
        }

        #endregion

        /// <summary>
        /// Constructor 
        /// 
        /// Set the maximum file size.
        /// </summary>
        /// <param name="projectName">Project name.</param>
        /// <param name="projectFolderPath">Project folder path.</param>
        /// <param name="serialNum">Project serial number.</param>
        /// <param name="maxFileSize">Maximum file size for the binary file.</param>
        /// <param name="fileType">File type to determine filename and extension.  Default is Ensemble.</param>
        public AdcpBinaryWriter(string projectName, string projectFolderPath, string serialNum, long maxFileSize = DEFAULT_BINARY_FILE_SIZE, FileType fileType = FileType.Ensemble)
        {
            ProjectName = projectName;
            ProjectFolderPath = projectFolderPath;
            SerialNumber = serialNum;
            MaxFileSize = maxFileSize;
            _fileType = fileType;

            _isProcessingFile = false;

            // Write buffer
            _writeBuffer = new ConcurrentQueue<byte[]>();
            _writeBufferIndex = 0;
        }

        /// <summary>
        /// Constructor that takes a project
        /// 
        /// Set the maximum file size.
        /// </summary>
        /// <param name="project">Project.</param>
        /// <param name="maxFileSize">Maximum file size for the binary file.</param>
        /// <param name="fileType">File type to determine filename and extension.  Default is Ensemble.</param>
        public AdcpBinaryWriter(Project project, long maxFileSize = DEFAULT_BINARY_FILE_SIZE, FileType fileType = FileType.Ensemble)
        {
            ProjectName = project.ProjectName;
            ProjectFolderPath = project.ProjectFolderPath;
            SerialNumber = project.SerialNumber.SerialNumberString;
            MaxFileSize = maxFileSize;
            _fileType = fileType;

            _isProcessingFile = false;

            // Write buffer
            _writeBuffer = new ConcurrentQueue<byte[]>();
            _writeBufferIndex = 0;

            // Set the project.
            //SelectedProject = selectedProject;
        }

        /// <summary>
        /// On shutdown close the file.
        /// This will write any remaining
        /// data to the file and close the
        /// file.
        /// </summary>
        public void Dispose()
        {
            Flush();
        }

        /// <summary>
        /// Add the incoming data to the buffer.  If
        /// the data cannot fit in the buffer, write
        /// the current buffer to the file then
        /// write the new data to the buffer.
        /// </summary>
        /// <param name="data">Data to write to the file.</param>
        public void AddIncomingData(byte[] data)
        {
            // Verify there is a project to write to
            //if (_selectedProject != null)
            //{
                // Check if the file can fit in the buffer
                // If not, write the current buffer to the file
                if (data.Length + _writeBufferIndex >= MAX_BUFFER_SIZE)
                {
                    if (!_isProcessingFile)
                    {
                        WriteBuffer();
                    }
                }

                // Write the data to the buffer
                _writeBuffer.Enqueue(data);

                // Keep track of the number of bytes in the list
                // The list stores byte arrays, this is to keep track of the total number of bytes.
                _writeBufferIndex += data.Length;

                // Publish the number of bytes written
                PublishEnsembleWrite(data.Length);
            //}
        }

        /// <summary>
        /// Reset the file name.  If the project changes
        /// or the max file is met, a new file name will need
        /// to be generated.
        /// </summary>
        public void ResetFileName()
        {
            //if (_selectedProject != null)
            //{
                // Get a new file name
                _fileName = GetNewFileName();
            //}
        }

        #region Open 

        /// <summary>
        /// Open or create the binary file.  Create a file name
        /// based off the project name given. 
        /// </summary>
        private void Open()
        {
            try
            {
                // Ensure a filename has been set
                if (string.IsNullOrEmpty(_fileName))
                {
                    ResetFileName();
                }

                // Verify the system is recording in Live mode
                // And a project has been selected
                //if (_selectedProject != null)
                //{
                    _binWriter = new BinaryWriter(File.Open(_fileName, FileMode.Append, FileAccess.Write));

                    // Set the file size
                    FileInfo finfo = new FileInfo(_fileName);
                    _fileSize = finfo.Length;

                //}
            }
            catch (Exception e)
            {
                log.Error("Error trying to open binary file for project: " + ProjectName, e);
            }
        }

        /// <summary>
        /// If the Selected project changes,
        /// then the current binary write needs
        /// to be reset, close then reopen
        /// the binary writer with the new settings.
        /// </summary>
        private void Reopen()
        {
            // Write the remaining data in the buffer to the file
            if (!_isProcessingFile)
            {
                WriteBuffer();
            }

            // Reset the file name xAxis
            _fileNameIndex = 0;

            // Open the file with new settings
            //Open();
        }

        #endregion

        #region Write


        /// <summary>
        /// Write all the data stored in the list.
        /// Then clear the list and reset the buffer.
        /// </summary>
        private void WriteBuffer()
        {
            try
            {
                // Lock so only one write operation can occur
                lock (_FileLock)
                {
                    _isProcessingFile = true;

                    // Open the writer
                    Open();

                    // Make a copy of the buffer just in case new data is being added to the
                    // buffer while we are writing
                    //List<byte[]> buffer = new List<byte[]>(_writeBuffer);

                    // Clear the buffer
                    //_writeBuffer.Clear();
                    //_writeBufferIndex = 0;

                    // Go through the list writing the data
                    //for (int x = 0; x < buffer.Count; x++ )
                    //{
                    // Write the data to the file
                    //    Write(buffer[x]);
                    //}
                    while (!_writeBuffer.IsEmpty)
                    {
                        byte[] data = null;
                        _writeBuffer.TryDequeue(out data);
                        if (data != null)
                        {
                            Write(data);
                        }
                    }

                    // Clear the buffer index
                    _writeBufferIndex = 0;


                    // Flush the data to the file
                    if (_binWriter != null)
                    {
                        _binWriter.Flush();

                        // Close the writer
                        _binWriter.Close();
                    }

                    _isProcessingFile = false;
                }
            }
            catch (Exception e)
            {
                _isProcessingFile = false;
                log.Error(string.Format("Error writing data to file. {0}", _fileName), e);
            }
        }


        /// <summary>
        /// Write the incoming data to the file.  If the file 
        /// reaches the maximum file size, close the file and
        /// create a new file with a new index.
        /// </summary>
        /// <param name="data">Data to write to the file.</param>
        private void Write(byte[] data)
        {
            // Check if the file will exceed the max size
            if (data.Length + _fileSize > MaxFileSize)
            {
                // Flush the current file
                Flush();

                // Generate new file name
                ResetFileName();
            }

            try
            {
                // Write the data to the file
                _binWriter.Write(data);
            }
            catch (Exception e) 
            {
                log.Error(string.Format("Error writing data. {0}", _fileName), e);
            }

            // Keep track of the file size
            _fileSize += data.Length;

            // Publish that the ensembles have been written to the file
            //PublishEnsembleWrite(_fileSize);
        }

        /// <summary>
        /// Write any remaining data in 
        /// the buffer to the file.  Then 
        /// close the file stream.  Flush
        /// to ensure the buffer is cleared.
        /// </summary>
        public void Flush()
        {
            // If there is any data in the buffer, write it to the file before closing
            // the file.
            if (this._writeBuffer.Count > 0)
            {
                if (!_isProcessingFile)
                {
                    WriteBuffer();
                }
            }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Create a file name using the project name and
        /// file xAxis.  If the file name is taken, generate
        /// a new file name until a file name is found that
        /// is not being used.  Return this new file name.
        /// </summary>
        /// <returns>Unique file name.</returns>
        private string GetNewFileName()
        {
            // Generate a file name
            string fileName = GenerateFileName(++_fileNameIndex);

            // Check if file exist
            while (File.Exists(fileName))
            {
                fileName = GenerateFileName(++_fileNameIndex);
            }

            // Reset file size
            _fileSize = 0;

            return fileName;
        }

        /// <summary>
        /// Generate a full path file name based off
        /// the selected project and the index given.
        /// </summary>
        /// <param name="index">PlaybackIndex to include in file name.</param>
        /// <returns>New file name with project name and index.</returns>
        private string GenerateFileName(int index)
        {
            switch (_fileType)
            {
                case FileType.GPS1:
                    return ProjectFolderPath + @"\" + SerialNumber + "_" + ProjectName + "_" + (index) + "_GPS1" + Core.Commons.NMEA_FILE_EXT;
                case FileType.GPS2:
                    return ProjectFolderPath + @"\" + SerialNumber + "_" + ProjectName + "_" + (index) + "_GPS2" + Core.Commons.NMEA_FILE_EXT;
                case FileType.NMEA1:
                    return ProjectFolderPath + @"\" + SerialNumber + "_" + ProjectName + "_" + (index) + "_NMEA1" + Core.Commons.NMEA_FILE_EXT;
                case FileType.NMEA2:
                    return ProjectFolderPath + @"\" + SerialNumber + "_" + ProjectName + "_" + (index) + "_NMEA2" + Core.Commons.NMEA_FILE_EXT;
                case FileType.STA:
                    return ProjectFolderPath + @"\" + SerialNumber + "_" + ProjectName + "_" + (index) + "_STA" + Core.Commons.AVG_STA_ENSEMBLE_FILE_EXT;
                case FileType.LTA:
                    return ProjectFolderPath + @"\" + SerialNumber + "_" + ProjectName + "_" + (index) + "_LTA" + Core.Commons.AVG_LTA_ENSEMBLE_FILE_EXT;
                case FileType.Ensemble:
                default:
                    return ProjectFolderPath + @"\" + SerialNumber + "_" + ProjectName + "_" + (index) + Core.Commons.SINGLE_ENSEMBLE_FILE_EXT;
            }
        }

        #endregion

        #region Events

        #region Ensemble Write Event

        /// <summary>
        /// Event To subscribe to. 
        /// </summary>
        /// <param name="count">Number of ensembles in the database.</param>
        public delegate void EnsembleWriteEventHandler(long count);

        /// <summary>
        /// Subscribe to this event.  This will hold all subscribers.
        /// 
        /// To subscribe:
        /// writer.EnsembleWriteEvent += new writer.EnsembleWriteEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// writer.EnsembleWriteEvent -= (method to call)
        /// </summary>
        public event EnsembleWriteEventHandler EnsembleWriteEvent;

        /// <summary>
        /// Verify there is a subscriber before calling the
        /// subscribers with the new event.
        /// </summary>
        private void PublishEnsembleWrite(long count)
        {
            if (EnsembleWriteEvent != null)
            {
                EnsembleWriteEvent(count);
            }
        }

        #endregion

        #endregion
    }
}