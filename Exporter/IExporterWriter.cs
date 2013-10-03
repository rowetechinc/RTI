// -----------------------------------------------------------------------
// <copyright file="IExporterWriter.cs" company="HP">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

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
        void Open(string filePath, string fileName);

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
        void Close();


    }
}
