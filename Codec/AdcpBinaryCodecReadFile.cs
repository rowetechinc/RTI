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
 * 02/08/2017      RC          3.4.0      Initial coding
 * 02/23/2017      RC          3.4.1      Changed to ReadBytes to read in from the file to improve performance.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace RTI
{
    /// <summary>
    /// Decode a binary ensemble file.
    /// </summary>
    public class AdcpBinaryCodecReadFile
    {

        #region Variables

        /// <summary>
        ///  Setup logger
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        /// <summary>
        /// Initialize the object.
        /// </summary>
        public AdcpBinaryCodecReadFile()
        {

        }

        /// <summary>
        /// Get a list of all the ensembles in the file.
        /// This will look for the start locations of all the ensembles.
        /// Then it will get the entire ensemble from the file.
        /// </summary>
        /// <param name="file">File to get the ensemble.</param>
        /// <returns>The list of ensembles.</returns>
        public List<DataSet.EnsemblePackage> GetEnsembles(string file)
        {
            if (File.Exists(file))
            {
                try
                {
                    // Get the list of all the ensemble start locations
                    var ensStart = FindEnsembleStarts(file);

                    // Decode the ensembles found
                    return FindCompleteEnsembles(ensStart, file);
                }
                catch(Exception e)
                {
                    log.Error("Error Finding ensembles in file.", e);  
                }
            }

            return new List<DataSet.EnsemblePackage>();
        }

        /// <summary>
        /// Look for the start of an ensemble.  This will look for the ID of a header
        /// in the ensemble.  If found, set the start location.
        /// </summary>
        /// <param name="file">File to find the start locations</param>
        /// <returns>List of all the start locations.</returns>
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
                            ensStartList.Add(index-15);     
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
        /// Find the entire ensemble in the file.  This will look for the start location.
        /// Decode the payload size and checksum.  If they are good, then generate an 
        /// ensemble from the data.  Add the data to the list and return it.
        /// </summary>
        /// <param name="ensStart">List of all the ensembles.</param>
        /// <param name="file">File to look for the ensembles.</param>
        /// <returns>List of all the ensembles in the file.</returns>
        protected List<DataSet.EnsemblePackage> FindCompleteEnsembles(List<int> ensStart, string file)
        {
            var list = new List<DataSet.EnsemblePackage>(ensStart.Count);

            using (var fileStream = new BinaryReader(File.Open(file, FileMode.Open, FileAccess.Read)))
            {
                // Go through each start location
                foreach (var start in ensStart)
                {
                    try
                    {
                        // Move the start location and read in the header
                        fileStream.BaseStream.Seek(start, SeekOrigin.Begin);
                        byte[] buffer = fileStream.ReadBytes(DataSet.Ensemble.ENSEMBLE_HEADER_LEN);
                        if (buffer.Length >= DataSet.Ensemble.ENSEMBLE_HEADER_LEN)
                        {
                            // Get the payload size
                            int payloadSize = buffer[24];
                            payloadSize += buffer[25] << 8;
                            payloadSize += buffer[26] << 16;
                            payloadSize += buffer[27] << 24;

                            // Get the payload inverse
                            int notPayloadSize = buffer[28];
                            notPayloadSize += buffer[29] << 8;
                            notPayloadSize += buffer[30] << 16;
                            notPayloadSize += buffer[31] << 24;
                            notPayloadSize = ~notPayloadSize;

                            // Check the payload value is correct based off the inverse
                            if( payloadSize == notPayloadSize)
                            {
                                // Get the ensemble size
                                int ensSize = DataSet.Ensemble.CalculateEnsembleSize(payloadSize);

                                // Sanity check
                                // Check if it is too small or too large
                                if (ensSize > DataSet.Ensemble.ENSEMBLE_HEADER_LEN && ensSize < MathHelper.MB_TO_BYTES)
                                {
                                    // Get the entire ensemble
                                    fileStream.BaseStream.Seek(start, SeekOrigin.Begin);
                                    byte[] rawEns = fileStream.ReadBytes(ensSize);

                                    // Check the checksum
                                    long calculatedChecksum = DataSet.Ensemble.CalculateEnsembleChecksum(rawEns);
                                    long ensembleChecksum = DataSet.Ensemble.RetrieveEnsembleChecksum(rawEns);
                                    if (calculatedChecksum == ensembleChecksum)
                                    {
                                        // Decode the ensemble and add it to the list
                                        var ens = DataSet.Ensemble.DecodeRawAdcpData(rawEns);

                                        // Package the data
                                        var ensPak = new DataSet.EnsemblePackage();
                                        ensPak.Ensemble = ens;
                                        ensPak.RawEnsemble = rawEns;
                                        list.Add(ensPak);
                                        //Debug.WriteLine("Ens: " + ens.EnsembleData.EnsembleNumber + " Count: " + count + " " + System.DateTime.Now);
                                    }
                                }
                                else
                                {
                                    Debug.WriteLine("AdcpBinaryCodecReadFile::Ensemble Size to large or small:" + start + ":" + ensSize);
                                }
                            }
                            else
                            {
                                Debug.WriteLine("AdcpBinaryCodecReadFile::Playload and Inv do not match:" + start + " : " + payloadSize + ":" + notPayloadSize);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error("Error looking for an ensemble. Loc: " + start, e);
                    }
                }
            }

            return list;
        }

    }
}
