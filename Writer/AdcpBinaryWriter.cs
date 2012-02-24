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
 *       
 * 
 */

using System.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using log4net;

namespace RTI
{
    /// <summary>
    /// Take the ADCP binary data and
    /// write it to file.  This is used
    /// to write the binary data received
    /// from the ADCP serial port and
    /// write it to a file.
    /// </summary>
    public class AdcpBinaryWriter
    {
        /// <summary>
        /// Setup logger to report errors.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// A buffer will be used to write
        /// large amounts of data to the file
        /// instead of writing small chunks.
        /// </summary>
        private const long MAX_BUFFER_SIZE = 1048576 * 1;   // 1 MB

        /// <summary>
        /// Maximum file size in bytes.
        /// When the file reaches the maximum size,
        /// a new file will be created and the current
        /// name indexed by 1.
        /// </summary>
        private long _maxFileSize = 0;
    
        /// <summary>
        /// Buffer to store large amounts of data.
        /// This will be a list of byte arrays.
        /// As data is added, the array will be
        /// added to the list.  When it needs to
        /// be written to file, all the arrays
        /// will be written to the file.
        /// </summary>
        private List<byte[]> _writeBuffer;

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
        /// Current size of the file to ensure
        /// it does not exceed a max value.
        /// </summary>
        private long _fileSize = 0;

        /// <summary>
        /// Writer to write binary data to
        /// a file.
        /// </summary>
        BinaryWriter _binWriter = null;


        #region Properties

        /// <summary>
        /// Set or get the Selected Project.
        /// When setting the project, it will
        /// reopen the binary writer after the
        /// new selected project is set.
        /// </summary>
        private Project _selectedProject;
        /// <summary>
        /// Set or get the Selected Project.
        /// When setting the project, it will
        /// reopen the binary writer after the
        /// new selected project is set.
        /// </summary>
        public  Project SelectedProject
        {
            private get { return _selectedProject; }
            set
            {
                _selectedProject = value;

                // Clear the remaining data to the file
                Flush();

                // Reset the file index
                _fileNameIndex = 0;

                if (_selectedProject != null)
                {
                    // Get a new file name
                    _fileName = GetNewFileName();
                }
            }
        }

        #endregion

        /// <summary>
        /// Constructor 
        /// 
        /// Set the maximum file size.
        /// </summary>
        /// <param name="maxFileSize">Maximum file size for the binary file.</param>
        public AdcpBinaryWriter(long maxFileSize)
        {
            _maxFileSize = maxFileSize;

            // Write buffer
            _writeBuffer = new List<byte[]>();
            _writeBufferIndex = 0;
        }

        /// <summary>
        /// On shutdown close the file.
        /// This will write any remaining
        /// data to the file and close the
        /// file.
        /// </summary>
        public void Shutdown()
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
            if (_selectedProject != null)
            {
                // Check if the file can fit in the buffer
                // If not, write the current buffer to the file
                if (data.Length + _writeBufferIndex >= MAX_BUFFER_SIZE)
                {
                    WriteBuffer();
                }

                // Write the data to the buffer
                _writeBuffer.Add(data);

                // Keep track of the number of bytes in the list
                // The list stores byte arrays, this is to keep track of the total number of bytes.
                _writeBufferIndex += data.Length;
            }
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
                // Verify the system is recording in Live mode
                // And a project has been selected
                if (_selectedProject != null)
                {
                    _binWriter = new BinaryWriter(File.Open(_fileName, FileMode.Append, FileAccess.Write));

                    // Set the file size
                    FileInfo finfo = new FileInfo(_fileName);
                    _fileSize = finfo.Length;

                }
            }
            catch (Exception e)
            {
                log.Error("Error trying to open binary file for project: " + _selectedProject.ProjectName, e);
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
            WriteBuffer();

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
            // Open the writer
            Open();

            // Make a copy of the buffer just in case new data is being added to the
            // buffer while we are writing
            List<byte[]> buffer = new List<byte[]>(_writeBuffer);

            // Clear the buffer
            _writeBuffer.Clear();
            _writeBufferIndex = 0;

            // Go through the list writing the data
            for (int x = 0; x < buffer.Count; x++ )
            {
                // Write the data to the file
                Write(buffer[x]);
            }

            // Flush the data to the file
            _binWriter.Flush();

            // Close the writer
            _binWriter.Close();
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
            if (data.Length + _fileSize > _maxFileSize)
            {
                // Flush the current file
                Flush();

                // Generate new file name
                _fileName = GetNewFileName();
            }

            // Write the data to the file
            _binWriter.Write(data);

            // Keep track of the file size
            _fileSize += data.Length;
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
                WriteBuffer();
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
            return _selectedProject.ProjectFolderPath + @"\" + _selectedProject.ProjectName + "_" + (index) + Core.Commons.SINGLE_ENSEMBLE_FILE_EXT;
        }

        #endregion
    }
}