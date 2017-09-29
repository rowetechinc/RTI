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
 * 09/29/2017      RC          3.4.4      Added original data format.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using DotSpatial.Positioning;

namespace RTI
{
    /// <summary>
    /// Decode a DVL ensemble file.
    /// </summary>
    public class AdcpDvlCodecReadFile
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
        public AdcpDvlCodecReadFile()
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
                    // Get the start location of the sentences
                    var sentList = FindSentence(file);

                    // Decode the sentences found
                    return FindCompleteEnsembles(sentList);
                }
                catch (Exception e)
                {
                    log.Error("Error Finding ensembles in file.", e);
                }
            }

            return new List<DataSet.EnsemblePackage>();
        }

        /// <summary>
        /// Look for all the sentences in the file.  This will look for any $ in the file.
        /// If a $ is find, it will look for the next one and create a sentences.
        /// </summary>
        /// <param name="file">File to find the NMEA sentence.</param>
        /// <returns>List of all the NMEA sentence.</returns>
        protected List<NmeaSentence> FindSentence(string file)
        {
            char[] buffer = new char[1];
            //List<int> list = new List<int>();
            List<NmeaSentence> list = new List<NmeaSentence>();
            List<char> sentence = new List<char>();

            using (var fileStream = new StreamReader(file))
            {
                while (fileStream.Read(buffer, 0, buffer.Length) > 0)     // Get 1 bytes
                {
                    if(buffer[0].Equals('$'))
                    {
                        // If the sentance has data, process it 
                        var ens = ProcessSentence(sentence.ToArray());
                        if(ens != null)
                        {
                            list.Add(ens);
                        }

                        // Clear the current sentence and start over
                        sentence.Clear();
                    }

                    //list.Add(index);
                    sentence.Add(buffer[0]);
                }

                fileStream.Close();
                fileStream.Dispose();
            }

            return list;
        }

        /// <summary>
        /// Process the sentence to verify the are good sentences.  If the sentence is not good,
        /// a null will be returned.
        /// </summary>
        /// <param name="data">String that may contain a NMEA sentence.</param>
        /// <returns>NMEA sentence if valid.</returns>
        protected NmeaSentence ProcessSentence(char[] data)
        {
            // Convert the char array to string and see if it is valid sentence
            var line = new string(data);
            var sent = new NmeaSentence(line);
            if (sent.IsValid)
            {
                return sent;
            }

            return null;
        }

        /// <summary>
        /// Find the PRTI01 message.  This is the beginning of a group.  Whenever the next
        /// PRTI01 message, pass all the groupped messages together and pass it as an ensemble.
        /// </summary>
        /// <param name="nmea">List of all the NMEA sentences.</param>
        /// <returns>List of all the ensembles in the file.</returns>
        protected List<DataSet.EnsemblePackage> FindCompleteEnsembles(List<NmeaSentence> nmea)
        {
            var list = new List<DataSet.EnsemblePackage>();
            var buffer = new List<NmeaSentence>();

            foreach (var sent in nmea)
            {
                // Check for PRTI01
                if (sent.CommandWord.EndsWith(RTI.Prti01Sentence.CMD_WORD_PRTI01, StringComparison.Ordinal))
                {
                    // Pass the buffer to create an ensemble
                    // and add it to the list
                    var ens = CreateEnsemble(buffer);
                    if (ens.RawEnsemble != null)
                    {
                        list.Add(ens);
                    }
                    buffer.Clear();
                }
                // Add the sentence to the buffer
                buffer.Add(sent);
            }

            return list;
        }

        /// <summary>
        /// Create an ensemble package based off the buffer given.
        /// This will combine all the sentences into an ensemble.
        /// It will convert the ensemble to byte array and create
        /// an Ensemble package.
        /// </summary>
        /// <param name="buffer">Buffer of NMEA sentences.</param>
        /// <returns>Ensemble package containing ensemble data.</returns>
        protected DataSet.EnsemblePackage CreateEnsemble(List<NmeaSentence> buffer)
        {
            DataSet.EnsemblePackage package = new DataSet.EnsemblePackage();

            // If no data, move on
            if(buffer.Count == 0)
            {
                package.Ensemble = null;
                package.RawEnsemble = null;
                return package;
            }

            // Convert the sentences to an ensemble.
            try
            {
                var ens = DecodeSentences(buffer);

                if (ens != null)
                {
                    package.Ensemble = ens;
                    package.RawEnsemble = ens.Encode();
                    package.OrigDataFormat = AdcpCodec.CodecEnum.DVL;
                }

            }
            catch(Exception e)
            {
                StringBuilder sb = new StringBuilder();
                foreach(var line in buffer)
                {
                    sb.Append(line);
                }
                log.Error("Error Decoding DVL ensemble.  " + sb.ToString(), e);
            }

            return package;
        }

        /// <summary>
        /// Decode the buffer.  This will create all the datasets based off
        /// the sentences.
        /// </summary>
        /// <param name="buffer">Buffer to decode.</param>
        private DataSet.Ensemble DecodeSentences(List<NmeaSentence> buffer)
        {
            Prti01Sentence prti01 = null;
            Prti02Sentence prti02 = null;
            Prti03Sentence prti03 = null;
            Prti30Sentence prti30 = null;
            Prti31Sentence prti31 = null;
            var nmeaBuffer = new List<string>();

            foreach (var sentence in buffer)
            {
                // Check for PRTI01
                if (sentence.CommandWord.EndsWith(RTI.Prti01Sentence.CMD_WORD_PRTI01, StringComparison.Ordinal))
                {
                    // Store the sentence to be combined with PRTI01
                    prti01 = new Prti01Sentence(sentence.Sentence, sentence.CommandWord, sentence.Words, sentence.ExistingChecksum);
                }
                // Check for PRTI02
                else if (sentence.CommandWord.EndsWith(RTI.Prti02Sentence.CMD_WORD_PRTI02, StringComparison.Ordinal))
                {
                    prti02 = new Prti02Sentence(sentence.Sentence, sentence.CommandWord, sentence.Words, sentence.ExistingChecksum);
                }
                // Check for PRTI03
                else if (sentence.CommandWord.EndsWith(RTI.Prti03Sentence.CMD_WORD_PRTI03, StringComparison.Ordinal))
                {
                    // Store the sentence to be combined with PRTI02
                    prti03 = new Prti03Sentence(sentence.Sentence, sentence.CommandWord, sentence.Words, sentence.ExistingChecksum);
                }
                // Check for PRTI30
                else if (sentence.CommandWord.EndsWith(RTI.Prti30Sentence.CMD_WORD_PRTI30, StringComparison.Ordinal))
                {
                    // Store the sentence to be combined with PRTI01 and PRTI02
                    prti30 = new Prti30Sentence(sentence.Sentence, sentence.CommandWord, sentence.Words, sentence.ExistingChecksum);
                }
                // Check for PRTI31
                else if (sentence.CommandWord.EndsWith(RTI.Prti31Sentence.CMD_WORD_PRTI31, StringComparison.Ordinal))
                {
                    // Store the sentence to be combined with PRTI01 and PRTI02
                    prti31 = new Prti31Sentence(sentence.Sentence, sentence.CommandWord, sentence.Words, sentence.ExistingChecksum);
                }
                else
                {
                    // If the data was nether PRTI01 or PRTI02, then it must be GPS NMEA data and add it 
                    // to the NMEA buffer to be processed with a complete dataset
                    nmeaBuffer.Add(sentence.Sentence + "\r\n");
                }
            }

            return CreateEnsemble(prti01, prti02, prti03, prti30, prti31, nmeaBuffer);
        }

        /// <summary>
        /// Create the data set using PRTI01.  Then add PRTI02 to the
        /// dataset.  Then send the dataset.
        /// </summary>
        /// <param name="prti01">PRTI01 Sentence.</param>
        /// <param name="prti02">PRTI02 Sentence.</param>
        /// <param name="prti03">PRTI03 Sentence.</param>
        /// <param name="prti30">PRTI30 Sentence.</param>
        /// <param name="prti31">PRTI31 Sentence.</param>
        /// <param name="nmeaBuffer">NMEA buffer.</param>
        /// <returns>Created ensemble.</returns>
        private DataSet.Ensemble CreateEnsemble(Prti01Sentence prti01, Prti02Sentence prti02, Prti03Sentence prti03 = null, Prti30Sentence prti30 = null, Prti31Sentence prti31 = null, List<string> nmeaBuffer = null)
        {
            // Create the dataset
            DataSet.Ensemble ensemble = CreateDataSet(prti01);

            if (prti02 != null)
            {
                ensemble.AddAdditionalBottomTrackData(prti02);
            }

            if(prti03 != null)
            {
                ensemble.AddAdditionalBottomTrackData(prti03);
            }

            if (prti30 != null)
            {
                ensemble.AddAdditionalAncillaryData(prti30);
                ensemble.AddAdditionalBottomTrackData(prti30);

                // Setup the serial number
                if (ensemble.IsEnsembleAvail)
                {
                    // Remove the temp serial number subsystem
                    ensemble.EnsembleData.SysSerialNumber.RemoveSubsystem(SerialNumber.DVL_Subsystem);

                    // Add the actual subsystem
                    ensemble.EnsembleData.SysSerialNumber.AddSubsystem(prti30.SubsystemConfig.SubSystem);
                }
            }

            if (prti31 != null)
            {
                ensemble.AddAdditionalAncillaryData(prti31);
                ensemble.AddAdditionalBottomTrackData(prti31);

                // Setup the serial number
                if (ensemble.IsEnsembleAvail)
                {
                    // Remove the temp serial number subsystem
                    ensemble.EnsembleData.SysSerialNumber.RemoveSubsystem(SerialNumber.DVL_Subsystem);

                    // Add the actual subsystem
                    ensemble.EnsembleData.SysSerialNumber.AddSubsystem(prti30.SubsystemConfig.SubSystem);
                }
            }

            if(nmeaBuffer != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach(var nmea in nmeaBuffer)
                {
                    sb.Append(nmea);
                }

                ensemble.AddNmeaData(sb.ToString());
            }

            return ensemble;
        }

        /// <summary>
        /// Create a dataset.  Set the bottom track instrument velocity and water mass velocity.
        /// </summary>
        /// <param name="sentence">Sentence containing DVL data.</param>
        /// <returns>Dataset with values set.</returns>
        private DataSet.Ensemble CreateDataSet(Prti01Sentence sentence)
        {
            DataSet.Ensemble adcpData = new DataSet.Ensemble();

            // Add the Ensemble number to EnsembleDataSet
            adcpData.AddEnsembleData(sentence);

            // Add the Temp to AncillaryDataSet
            adcpData.AddAncillaryData(sentence);

            // Add Bottom Track data
            adcpData.AddBottomTrackData(sentence);

            // Add Water Mass data
            adcpData.AddInstrumentWaterMassData(sentence);

            return adcpData;
        }

    }
}
