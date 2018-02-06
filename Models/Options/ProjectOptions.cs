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
 * 09/01/2011      RC          2.21.0     Initial coding
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
    using Newtonsoft.Json;

    /// <summary>
    /// All the options for a project.  This includes the project name
    /// and project folder paths.
    /// </summary>
    public class ProjectOptions
    {

        #region Properties

        /// <summary>
        /// Name of the project.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Directory the project is located.
        /// </summary>
        public string ProjectDir { get; set; }

        /// <summary>
        /// Folder path to the project.
        /// This will include the directory and
        /// the directory created with the project name.
        /// ProjectDir/ProjectName
        /// It will not end with a '/'.
        /// </summary>
        [JsonIgnore]
        public string ProjectFolderPath { get; set; }

        ///// <summary>
        ///// A flag to know if the backup writer is enabled.
        ///// If the backup writer is enabled, a Backup Folder path
        ///// must be given.
        ///// </summary>
        //public bool IsBackupWriterEnabled { get; set; }

        /// <summary>
        /// Folder path to the backup project.
        /// This will include the directory and
        /// the directory created with the project name.
        /// ProjectDir/ProjectName
        /// It will not end with a '/'.
        /// </summary>
        public string BackupProjectFolderPath { get; set; }

        /// <summary>
        /// Maximum size of a file in bytes.
        /// </summary>
        public long MaxFileSize { get; set; }

        #endregion

        /// <summary>
        /// Create the project options with no backup writer.
        /// </summary>
        /// <param name="name">Project name.</param>
        /// <param name="folderPath">Project folder path.</param>
        public ProjectOptions(string name, string folderPath)
        {
            // Setup the folder paths
            SetFolders(name, folderPath);

            // Set a default value for the max binary file size
            MaxFileSize = AdcpBinaryWriter.DEFAULT_BINARY_FILE_SIZE;
        }

        /// <summary>
        /// Create the project options with a backup writer.
        /// </summary>
        /// <param name="ProjectName">Project name.</param>
        /// <param name="ProjectDir">Project folder path.</param>
        /// <param name="BackupProjectFolderPath">Backup writer folder path.</param>
        /// <param name="MaxFileSize">Max File size in bytes.</param>
        [JsonConstructor]
        public ProjectOptions(string ProjectName, string ProjectDir, string BackupProjectFolderPath, long MaxFileSize = AdcpBinaryWriter.DEFAULT_BINARY_FILE_SIZE)
        {
            // Setup the folder paths
            SetFolders(ProjectName, ProjectDir);

            this.BackupProjectFolderPath = BackupProjectFolderPath;

            this.MaxFileSize = MaxFileSize;

            //// Check if the backup needs to be setup
            //if (IsBackupWriterEnabled && !string.IsNullOrEmpty(BackupProjectFolderPath))
            //{
            //    this.IsBackupWriterEnabled = IsBackupWriterEnabled;
            //    this.BackupProjectFolderPath = BackupProjectFolderPath;
            //}
            //else
            //{
            //    this.IsBackupWriterEnabled = false;
            //}
        }

        #region Folders

        /// <summary>
        /// Set the project name,
        /// project folder and project
        /// folder path.
        /// </summary>
        /// <param name="name">Name of the project.</param>
        /// <param name="dir">Project folder.</param>
        private void SetFolders(string name, string dir)
        {
            ProjectName = name;
            ProjectDir = dir;
            ProjectFolderPath = dir + @"\" + name;
        }

        #endregion

    }
}
