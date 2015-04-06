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

    /// <summary>
    /// Interface for exporters.  This will
    /// open, close and write the data.
    /// </summary>
    public interface IExporterWriter
    {
        /// <summary>
        /// Open the file to begin writing.
        /// This will get the file path and the file name.  
        /// If there are multiple files, the file name given will be the base.  No file extension is needed if multiple
        /// files will be created.
        /// </summary>
        /// <param name="filePath">Folder to write the files to.</param>
        /// <param name="fileName">Filename to write the file.</param>
        /// <param name="options">Export options.</param>
        void Open(string filePath, string fileName, ExportOptions options);

        /// <summary>
        /// Ensemble to write to the file.
        /// This will write the ensemble as the exported format to the file.
        /// If isMultipleFile is set to true, then each file will contain 1
        /// ensemble only.
        /// </summary>
        /// <param name="ensemble">Ensemble to write to a file in the new format.</param>
        /// <param name="isMultipleFile">Flag to know if the ensemble should have its own file.</param>
        void Write(DataSet.Ensemble ensemble, bool isMultipleFile = false);

        /// <summary>
        /// Close the file or files created for the export process.
        /// </summary>
        /// <returns>Return the options after completing the export process.</returns>
        ExportOptions Close();


    }
}
