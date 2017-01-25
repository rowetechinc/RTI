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
 * 05/17/2013      RC          2.19       Initial coding
 * 07/12/2013      RC          2.19.1     Added AddEntry() and ClearLog().
 * 07/30/2013      RC          2.19.3     Fixed bug setting the date and time to string.  Minutes and Month were swapped.
 * 
 */

using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RTI.DataSet;

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using log4net;

    #region MaintenceEntry

    /// <summary>
    /// Maitence entries in the maitence file
    /// on the ADCP.
    /// </summary>
    [JsonConverter(typeof(MaintenceEntrySerializer))]
    public class MaintenceEntry
    {
        /// <summary>
        /// IDs to identify the 
        /// type of entry.
        /// </summary>
        public enum EntryId
        {
            /// <summary>
            ///  Factory Compass calibration.
            /// This is done at RTI
            /// before shipping the unit.
            /// </summary>
            FactoryCompassCal,

            /// <summary>
            /// User Compass Calibration.
            /// </summary>
            UserCompassCal,

            /// <summary>
            /// System Test.
            /// </summary>
            SystemTest,

            /// <summary>
            /// Zero the pressure sensor.
            /// </summary>
            ZeroPressure,

            /// <summary>
            /// Change the orings.
            /// </summary>
            ChangeOrings,

        }

        #region Properties

        /// <summary>
        /// Time of the entry.
        /// </summary>
        public DateTime EntryTime { get; set; }

        /// <summary>
        /// ID for the entry.
        /// </summary>
        public EntryId ID { get; set; }

        /// <summary>
        /// Results of the entry.  Store any
        /// Results from the entry.
        /// </summary>
        public string Results { get; set; }

        /// <summary>
        /// Notes about the entry.
        /// </summary>
        public string Notes { get; set; }

        #endregion

        /// <summary>
        /// Set the values.
        /// </summary>
        /// <param name="id">Entry ID.</param>
        /// <param name="results">Results of the entry.</param>
        /// <param name="notes">Notes for the entry.</param>
        public MaintenceEntry(EntryId id, string results, string notes)
        {
            // Set the ID
            ID = id;

            // Set the time
            EntryTime = DateTime.Now;

            // Set the Results
            Results = results;

            // Set the Notes
            Notes = notes;
        }

        /// <summary>
        /// Set the values.
        /// This is used with the JSON decoding and allows the
        /// DateTime to be set.
        /// </summary>
        /// <param name="ID">Entry ID.</param>
        /// <param name="EntryTime">Time and Date for the entry.</param>
        /// <param name="Results">Results of the entry.</param>
        /// <param name="Notes">Notes for the entry.</param>
        [JsonConstructor]
        public MaintenceEntry(EntryId ID, DateTime EntryTime, string Results, string Notes)
        {
            // Set the ID
            this.ID = ID;

            // Set the time
            this.EntryTime = EntryTime;

            // Set the Results
            this.Results = Results;

            // Set the Notes
            this.Notes = Notes;
        }

        /// <summary>
        /// Convert the ID to a string.
        /// </summary>
        /// <param name="id">ID to convert.</param>
        /// <returns>String for the ID.</returns>
        public static string IdToString(EntryId id)
        {
            switch (id)
            {
                case EntryId.ChangeOrings:
                    return "Change O-Rings";
                case EntryId.FactoryCompassCal:
                    return "Factory Compass Calibration";
                case EntryId.SystemTest:
                    return "System Test";
                case EntryId.UserCompassCal:
                    return "User Compass Calibration";
                case EntryId.ZeroPressure:
                    return "Zero Pressure Sensor";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Create a string to represent this object.
        /// It will display all the values as a single line string.
        /// </summary>
        /// <returns>String of this object.</returns>
        public override string ToString()
        {
            return IdToString(ID) + ", " +
                EntryTime.ToString("yyyyMMdd, HHmmss") + ", " +
                Results + ", " +
                Notes;

        }

    }

    #endregion

    #region JSON Converter

    /// <summary>
    /// Convert the MaintenceEntry to an
    /// JSON object so it can be written to the
    /// maintence file.
    /// </summary>
    public class MaintenceEntrySerializer : JsonConverter
    {
        #region JSON Property Names

        /// <summary>
        /// EntryTime property.
        /// </summary>
        public const string JSON_STR_ENTRYTIME = "EntryTime";

        /// <summary>
        /// ID property.
        /// </summary>
        public const string JSON_STR_ID= "ID";

        /// <summary>
        /// Results property.
        /// </summary>
        public const string JSON_STR_RESULTS = "Results";

        /// <summary>
        /// Notes property.
        /// </summary>
        public const string JSON_STR_NOTES = "Notes";

        #endregion

        /// <summary>
        /// Write the JSON string.  This will convert all the properties to a JSON string.
        /// This is done manaully to improve conversion time.  The default serializer will check
        /// each property if it can convert.  This will convert the properties automatically.  This
        /// will double the speed.
        /// 
        /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble).
        /// 
        /// </summary>
        /// <param name="writer">JSON Writer.</param>
        /// <param name="value">Object to write to JSON.</param>
        /// <param name="serializer">Serializer to convert the object.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Cast the object
            var me = value as MaintenceEntry;

            // Start the object
            writer.Formatting = Formatting.None;            // Make the text not indented, so not as human readable.  This will save disk space
            writer.WriteStartObject();                      // Start the JSON object

            // EntryTime
            writer.WritePropertyName(JSON_STR_ENTRYTIME);
            writer.WriteValue(me.EntryTime);

            // ID
            writer.WritePropertyName(JSON_STR_ID);
            writer.WriteValue(me.ID);

            // Results
            writer.WritePropertyName(JSON_STR_RESULTS);
            writer.WriteValue(me.Results);

            // Notes
            writer.WritePropertyName(JSON_STR_NOTES);
            writer.WriteValue(me.Notes);

            // End the object
            writer.WriteEndObject();
        }

        /// <summary>
        /// Read the JSON object and convert to the object.  This will allow the serializer to
        /// automatically convert the object.  No special instructions need to be done and all
        /// the properties found in the JSON string need to be used.
        /// 
        /// Newtonsoft.Json.JsonConvert.DeserializeObject[DataSet.Ensemble](ensembleJsonStr).
        /// 
        /// </summary>
        /// <param name="reader">NOT USED. JSON reader.</param>
        /// <param name="objectType">NOT USED> Type of object.</param>
        /// <param name="existingValue">NOT USED.</param>
        /// <param name="serializer">Serialize the object.</param>
        /// <returns>Serialized object.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.Null)
            {
                // Load the object
                JObject jsonObject = JObject.Load(reader);

                // EntryTime
                var time = (DateTime)jsonObject[JSON_STR_ENTRYTIME];

                // ID
                var id = (int)jsonObject[JSON_STR_ID];

                // Results
                var results = (string)jsonObject[JSON_STR_RESULTS];

                // Notes
                var notes = (string)jsonObject[JSON_STR_NOTES];

                // Return the new object
                return new MaintenceEntry((MaintenceEntry.EntryId)id, time, results, notes);

            }

            return null;
        }

        /// <summary>
        /// Check if the given object is the correct type.
        /// </summary>
        /// <param name="objectType">Object to convert.</param>
        /// <returns>TRUE = object given is the correct type.</returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(MaintenceEntry).IsAssignableFrom(objectType);
        }
    }

    #endregion

    /// <summary>
    /// Create a maintence log for the ADCP.  THis
    /// will store all the maintence information on a
    /// file in the ADCP.  The data will be written in 
    /// JSON.  
    /// </summary>
    public class MaintenceLog
    {

        #region Variables

        /// <summary>
        ///  Setup logger
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Filename for the maintence file on the ADCP.
        /// </summary>
        public const string MAINT_FILE_NAME = "MAINT.TXT";

        /// <summary>
        /// Flag when Download is complete.
        /// </summary>
        private bool _isDownloadComplete;

        #endregion

        /// <summary>
        /// Initialize the object.
        /// </summary>
        public MaintenceLog()
        {
            
        }

        /// <summary>
        /// Get a list of all the entries from the
        /// maintence log.  This will connect to the ADCP.
        /// Download the file.  Read whats in the file 
        /// and store it to the list. 
        /// 
        /// If the serial port conneciton given is not
        /// connected, it will not attempt to get the file.
        /// If the serial port is in any mode but ADCP, it will
        /// not attempt to get the file.
        /// 
        /// If you want to download the file and store it to a specific
        /// location, set the dir.  If you do not want to store the file,
        /// leave it null.
        /// </summary>
        /// <param name="adcp">Adcp serial port.</param>
        /// <param name="dir">Directory to store the log.  If NULL is given, the file will be stored to the temp path.</param>
        /// <returns>A list of all the MaitenceEntry in the file.</returns>
        public List<MaintenceEntry> GetMaintenceLog(AdcpSerialPort adcp, string dir = null)
        {
            var list = new List<MaintenceEntry>();

            // If no directory was given, write the file
            // to a temp location
            if (dir == null)
            {
                dir = System.IO.Path.GetTempPath();
            }

            if (adcp.Mode == AdcpSerialPort.AdcpSerialModes.ADCP && adcp.IsAvailable())
            {
                // Get the log file
                string maintLog = DownloadLog(dir, adcp);

                // Decode the log file of all its content
                list = DecodeLog(maintLog);
                
                // The list will be null if the file is empty
                // Then create an empty list so NULL will not be returned
                if (list == null)
                {
                    list = new List<MaintenceEntry>();
                }

                // If the list was empty, then the log downloaded
                // was empty or bad.  Delete the file
                if (list.Count == 0)
                {
                    try
                    {
                        File.Delete(dir + @"\" + MAINT_FILE_NAME);
                    }
                    catch (Exception e)
                    {
                        log.Error(string.Format("Error deleting maintence file {0}", dir + @"\" + MAINT_FILE_NAME), e);
                    }
                }
            }


            return list;
        }

        #region Get Log

        /// <summary>
        /// Download the log file and return 
        /// the Results of the file.
        /// 
        /// It is assumed the connection is already made to the ADCP
        /// serial port.
        /// </summary>
        /// <param name="dir">Director to store the log file.</param>
        /// <param name="adcp">ADCP serial connection.</param>
        /// <returns>Content of the file.</returns>
        private string DownloadLog(string dir, AdcpSerialPort adcp)
        {
            // Result
            string result = "";

            // Directory to download the file
            _isDownloadComplete = false;
            bool resultDL = adcp.XModemDownload(dir, MAINT_FILE_NAME, false, true);
            adcp.DownloadCompleteEvent += delegate(string fileName, bool goodDownload)
            {
                // Set the flag when download is complete
                _isDownloadComplete = true;
            };

            // Wait for the download to be completed
            int timeout = 100;
            while (!_isDownloadComplete)
            {
                // Sleep to wait
                Thread.Sleep(AdcpSerialPort.WAIT_STATE * 2);

                // Give a timeout so it does not get stuck
                timeout--;
                if (timeout < 0)
                {
                    break;
                }
            }

            // Check if the download could be completed
            if (resultDL)
            {
                string filePath = dir + @"\" + MAINT_FILE_NAME;
                // If the file exist read in all the data
                if (File.Exists(filePath))
                {
                    try
                    {
                        result = File.ReadAllText(filePath);
                    }
                    catch (Exception e)
                    {
                        log.Error(string.Format("Error reading maintence file {0}", filePath), e);
                    }

                    // Remove the empty data in the file
                    // The file has a complete buffer written to it
                    // The buffer will contain empty data.  This
                    // will find the end of the JSON array.
                    // Add 1 to include the end of the array character
                    result = result.Substring(0, result.IndexOf(']') + 1);

                    try
                    {
                        // Write the cleaned up data to the file
                        File.WriteAllText(filePath, result);
                    }
                    catch (Exception e)
                    {
                        log.Error(string.Format("Error writing maintence file {0}", filePath), e);
                    }
                }
            }

            // Wait for the state to change
            System.Threading.Thread.Sleep(SerialConnection.WAIT_STATE);

            return result;
        }

        #endregion

        #region Decode Log

        /// <summary>
        /// Decode the Maintence log data.  It is assumed the
        /// log is a list of MaintenceEntries.  This will
        /// deserialize the data.
        /// An exception will be thrown if the file was not properly
        /// formatted or the file is empty.  The
        /// </summary>
        /// <param name="log">JSON data from the Maintence file.</param>
        /// <returns>List of all the MaintenceEntries.</returns>
        private List<MaintenceEntry> DecodeLog(string log)
        {
            try
            {
                // Check if the file is empty
                // The ADCP will return File Empty
                if (log.Contains("Empty File"))
                {
                    return new List<MaintenceEntry>();
                }

                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<MaintenceEntry>>(log);
            }
            catch (Exception)
            {
                return new List<MaintenceEntry>();
            }
        }

        #endregion

        #region Write To Log

        /// <summary>
        /// Write the list of maintence Entries to the
        /// Maintence log.  This will convert the list
        /// to a JSON object.  It will then create a temp
        /// file and write the JSON data
        /// to the file.  It will then upload the data to the
        /// ADCP.
        /// 
        /// Append will allow the maintence file to be complete
        /// overwritten.  If Append = false, only the data in list
        /// will be written to the maintence log and anything
        /// previously in the log will be lost.
        /// 
        /// </summary>
        /// <param name="adcp">Connection to the ADCP.</param>
        /// <param name="list">List to write to the ADCP.</param>
        /// <param name="append">TRUE = Add to end of file.  False = Overwrite file.</param>
        public void WriteLog(AdcpSerialPort adcp, List<MaintenceEntry> list, bool append = true)
        {
            // Create a list
            var maintList = new List<MaintenceEntry>();

            // If we are appending, add the data already in the
            // log
            // If we are not appending, the entire log file will
            // be overwritten with the new list
            if (append)
            {
                // Get the current log and append to it
                // Get the log file
                string maintLog = DownloadLog(System.IO.Path.GetTempPath(), adcp);

                // Decode the log file of all its content
                // and add it to the list
                maintList.AddRange(DecodeLog(maintLog));
            }

            // Add the new list to the one from the file
            maintList.AddRange(list);

            // Generate a temp file
            // Then write the list a JSON object to the file
            string fileName = System.IO.Path.GetTempPath() + @"\" + MAINT_FILE_NAME;
            using (FileStream fs = File.Open(fileName, FileMode.OpenOrCreate))
            using (StreamWriter sw = new StreamWriter(fs))
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
              jw.Formatting = Formatting.Indented;
              JsonSerializer serializer = new JsonSerializer();

              serializer.Serialize(jw, maintList);
            }

            // Upload the log to the ADCP
            adcp.XModemUpload(fileName);
        }

        /// <summary>
        /// Add the entry to the maintence log.  This will
        /// create a list of entries and write it to the log.
        /// </summary>
        /// <param name="adcp">Adcp to write the entry to.</param>
        /// <param name="entry">Entry to write.</param>
        public void AddEntry(AdcpSerialPort adcp, MaintenceEntry entry)
        {
            // Create a list
            var maintList = new List<MaintenceEntry>();
            maintList.Add(entry);

            // Append the entry
            WriteLog(adcp, maintList, true);
        }

        #endregion

        #region Clear Log

        /// <summary>
        /// Clear the maintence log of all information.
        /// This will only clear the log of user information
        /// but will still maintain the factory information.
        /// </summary>
        /// <param name="adcp">Adcp serial port connection.</param>
        public void ClearLog(AdcpSerialPort adcp)
        {
            // Get the current log and store to temp file
            string maintLog = DownloadLog(System.IO.Path.GetTempPath(), adcp);

            // Decode the log file of all its content
            var maintList = DecodeLog(maintLog);

            for (int x = 0; x < maintList.Count; x++ )
            {
                if (maintList[x].ID != MaintenceEntry.EntryId.FactoryCompassCal)
                {
                    maintList.Remove(maintList[x]);
                }
            }

            // Write back the new list to the ADCP
            // Setting false will overwrite the file
            WriteLog(adcp, maintList, false);
        }

        #endregion

    }
}
