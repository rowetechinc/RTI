using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI
{
    /// <summary>
    /// Write the data to the files based off the selections
    /// </summary>
    public class ExportWriter
    {
        #region Variables

        /// <summary>
        /// Setup logger to report errors.
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// CSV exporter.
        /// </summary>
        private CsvExporterWriter csv;

        /// <summary>
        /// Matlab exporter.
        /// </summary>
        private MatlabExporterWriter matlab;

        /// <summary>
        /// Matlab Matrix exporter.
        /// </summary>
        private MatlabMatrixExporterWriter matlabMatrix;

        /// <summary>
        /// PD0 exporter.
        /// </summary>
        private Pd0ExporterWriter pd0;

        /// <summary>
        /// Ensemble exporter.
        /// </summary>
        private EnsExporterWriter ensEx;

        #endregion


        /// <summary>
        /// Create the exporters to write the data.
        /// </summary>
        public ExportWriter()
        {
            csv = null;
            matlab = null;
            matlabMatrix = null;
            pd0 = null;
            ensEx = null;
        }

        /// <summary>
        /// Open the exporters based off the selection.
        /// </summary>
        /// <param name="folderPath">Folder path of the exported files.</param>
        /// <param name="filename">File name of the exported file.</param>
        /// <param name="options">Options for exporting.</param>
        /// <param name="isCsvSelected">CSV Exporter selected.</param>
        /// <param name="isMatlabSelected">MATLAB exporter selected.</param>
        /// <param name="isMatlabMatrixSelected">MATLAB matrix selected.</param>
        /// <param name="isPd0Selected">PD0 exporter selected.</param>
        /// <param name="isEnsSelected">ENS exporter selected.</param>
        public void Open(string folderPath, string filename, ExportOptions options, bool isCsvSelected, bool isMatlabSelected, bool isMatlabMatrixSelected, bool isPd0Selected, bool isEnsSelected)
        {            
            // Open all the exporters that are selected
            // The filename is name without the extension
            csv = new CsvExporterWriter();
            matlab = new MatlabExporterWriter();
            matlabMatrix = new MatlabMatrixExporterWriter();
            pd0 = new Pd0ExporterWriter();
            ensEx = new EnsExporterWriter();

            try
            {
                if (isCsvSelected)
                {
                    csv.Open(folderPath, filename + ".csv", options);
                }
                if (isMatlabSelected)
                {
                    matlab.Open(folderPath, filename, options);
                }
                if (isMatlabMatrixSelected)
                {
                    matlabMatrix.Open(folderPath, filename, options);
                }
                if (isPd0Selected)
                {
                    pd0.Open(folderPath, filename + "_pd0" + ".pd0", options);
                }
                if (isEnsSelected)
                {
                    ensEx.Open(folderPath, filename + "_rtb" + ".ens", options);
                }
            }
            catch(Exception ex)
            {
                log.Error("Error opening ensemble file.", ex);
            }
        }

        /// <summary>
        /// Write the ensemble to the selected writers.
        /// </summary>
        /// <param name="isCsvSelected">CSV Exporter selected.</param>
        /// <param name="isMatlabSelected">MATLAB exporter selected.</param>
        /// <param name="isMatlabMatrixSelected">MATLAB matrix selected.</param>
        /// <param name="isPd0Selected">PD0 exporter selected.</param>
        /// <param name="isEnsSelected">ENS exporter selected.</param>
        /// <param name="ens">Ensemble data.</param>
        public void Write(DataSet.Ensemble ens, bool isCsvSelected, bool isMatlabSelected, bool isMatlabMatrixSelected, bool isPd0Selected, bool isEnsSelected)
        {
            if (isCsvSelected)
            {
                csv.Write(ens);
            }
            if (isMatlabSelected)
            {
                matlab.Write(ens);
            }
            if (isMatlabMatrixSelected)
            {
                matlabMatrix.Write(ens);
            }
            if (isPd0Selected)
            {
                pd0.Write(ens);
            }
            if (isEnsSelected)
            {
                ensEx.Write(ens);
            }
        }

        /// <summary>
        /// Close all the exporters.
        /// </summary>
        /// <param name="isCsvSelected">CSV Exporter selected.</param>
        /// <param name="isMatlabSelected">MATLAB exporter selected.</param>
        /// <param name="isMatlabMatrixSelected">MATLAB matrix selected.</param>
        /// <param name="isPd0Selected">PD0 exporter selected.</param>
        /// <param name="isEnsSelected">ENS exporter selected.</param>
        public void Close(bool isCsvSelected, bool isMatlabSelected, bool isMatlabMatrixSelected, bool isPd0Selected, bool isEnsSelected)
        {
            // Close the files
            if (isCsvSelected)
            {
                csv.Close();
            }
            if (isMatlabSelected)
            {
                matlab.Close();
            }
            if (isMatlabMatrixSelected)
            {
                matlabMatrix.Close();
            }
            if (isPd0Selected)
            {
                pd0.Close();
            }
            if (isEnsSelected)
            {
                ensEx.Close();
            }
        }

    }
}
