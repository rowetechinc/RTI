using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI
{
    /// <summary>
    /// Because the RTI ADCP can store data in multiple formats,
    /// this importer will try all the formats to decode the data.
    /// </summary>
    class AdcpImporter
    {
        #region Variables

        AdcpImportBinaryFile importer = new AdcpImportBinaryFile();

        #endregion

        public AdcpImporter()
        {

        }

    }
}
