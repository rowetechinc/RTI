using RTI.DataSet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI
{
    /// <summary>
    /// When a file is downloaded from the ADCP, it will contain multiple subsystems.
    /// This will create a file for each subsystem.  It will still have a maximum file
    /// size, so the files do not get to large.
    /// </summary>
    public class BreakupFiles
    {
        /// <summary>
        /// Codec to decode the file
        /// </summary>
        private AdcpCodec _adcpCodec;

        /// <summary>
        /// Dictionary containing the subsystem configuration and a string
        /// for the current file for the subsystem.
        /// </summary>
        public Dictionary<SubsystemConfiguration, BinaryWriter> _ssConfigList;


        /// <summary>
        /// Initialize the values.
        /// </summary>
        public BreakupFiles()
        {
            // Create the dictionary
            _ssConfigList = new Dictionary<SubsystemConfiguration, BinaryWriter>();

            // Codecs
            _adcpCodec = new AdcpCodec();
            _adcpCodec.ProcessDataEvent += new AdcpCodec.ProcessDataEventHandler(_adcpCodec_ProcessDataEvent);
            _adcpCodec.ProcessDataCompleteEvent += new AdcpCodec.ProcessDataCompleteEventHandler(_adcpCodec_ProcessDataCompleteEvent);
        }

        /// <summary>
        /// Dispose the object.
        /// </summary>
        public void Dispose()
        {
            if (_adcpCodec != null)
            {
                _adcpCodec.ProcessDataCompleteEvent -= _adcpCodec_ProcessDataCompleteEvent;
                _adcpCodec.ProcessDataEvent -= _adcpCodec_ProcessDataEvent;
                _adcpCodec.Dispose();
            }
        }

        /// <summary>
        /// Burst are seperate by: 
        /// ********** BURST  END **********
        /// </summary>
        /// <param name="files"></param>
        /// <param name="outputDir"></param>
        public void FindBurstMarkers(string[] files, string outputDir)
        {
            foreach(var file in files)
            {

            }
        }


        /// <summary>
        /// Find similar subsystems.
        /// </summary>
        /// <param name="files"></param>
        /// <param name="outputDir"></param>
        public void FindSimilarSubsystems(string[] files, string outputDir)
        {
            foreach (var file in files)
            {

            }
        }

        /// <summary>
        /// Find ensemble start.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        protected List<int> FindEnsembleStarts(string file)
        {
            var ensStartList = new List<int>();

            using (var fileStream = new BinaryReader(File.Open(file, FileMode.Open, FileAccess.Read)))
            {
                var queue = new List<byte>();

                // Read in the data from file
                byte[] buffer = new byte[1];
                int count = 0;
                int index = 0;
                while ((count = fileStream.Read(buffer, 0, buffer.Length)) > 0)     // Get 1 bytes
                {
                    // Add the data to the queue
                    queue.Add(buffer[0]);

                    if (queue.Count >= 16)
                    {
                        if (queue[0] == 0x80 &&
                            queue[1] == 0x80 &&
                            queue[2] == 0x80 &&
                            queue[3] == 0x80 &&
                            queue[4] == 0x80 &&
                            queue[5] == 0x80 &&
                            queue[6] == 0x80 &&
                            queue[7] == 0x80 &&
                            queue[8] == 0x80 &&
                            queue[9] == 0x80 &&
                            queue[10] == 0x80 &&
                            queue[11] == 0x80 &&
                            queue[12] == 0x80 &&
                            queue[13] == 0x80 &&
                            queue[14] == 0x80 &&
                            queue[15] == 0x80)
                        {
                            // Add the start location to the list
                            // Subtract 15 to get back to the beginning of the queue
                            ensStartList.Add(index - 15);
                        }

                        // Remove the first item from the queue
                        queue.RemoveAt(0);
                    }

                    index++;
                }

                fileStream.Close();
                fileStream.Dispose();
            }


            return ensStartList;
        }

        /// <summary>
        ///  Check if the subsystem configuration exist in the dictionary.  If it
        ///  does not exist, add an entry in the dictionary.
        /// </summary>
        /// <param name="ssConfig">Subsystem configuration.</param>
        /// <param name="outputDir">Output Directory</param>
        private void CheckSubsystem(SubsystemConfiguration ssConfig, string outputDir)
        {
            if (!_ssConfigList.ContainsKey(ssConfig))
            {
                // Create a file path
                string filePath = Path.Combine(outputDir, ssConfig.DescString());

                if (!File.Exists(filePath))
                {
                    // Create a file stream and writer
                    var fs = new FileStream(filePath, FileMode.Create);
                    BinaryWriter writer = new BinaryWriter(fs);

                    // Create the entry in the dictionary
                    _ssConfigList.Add(ssConfig, writer);
                }
            }
        }


        #region EventHandlers

        private void _adcpCodec_ProcessDataCompleteEvent()
        {
            throw new NotImplementedException();
        }

        private void _adcpCodec_ProcessDataEvent(byte[] binaryEnsemble, Ensemble ensemble, AdcpCodec.CodecEnum dataFormat)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
