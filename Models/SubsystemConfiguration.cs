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
 * 09/18/2012      RC          2.15       Initial coding
 * 10/09/2012      RC          2.15       Changed the COMMAND_SETUP_START.
 * 12/28/2012      RC          2.17       Removed SubsystemConfiguration.Empty and replaced is IsEmpty().
 *                                         Made SubsystemConfiguration take a Subsystem in its constructor.
 *                                         Added DescString().
 * 07/25/2013     RC           2.19.1     Added IndexCodeString().
 * 07/26/2013     RC           2.19.3     Added SubsystemConfigIndex property.  This is to differentiate between CepoIndex and SubsystemConfigIndex.
 * 08/12/2013     RC           2.19.4     Convert to and from JSON.
 * 08/14/2013     RC           2.19.4     Fixed JSON conversions.
 * 08/22/2013     RC           2.19.4     Added StringDesc property.
 * 
 */


using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace RTI
{

    /// <summary>
    /// Identifies which command setup is being used
    /// for the current subsystem.
    /// 
    /// Use this configuration with the Subsystem Type to
    /// separate the ensemble for each setup/subsystem.
    /// </summary>
    [JsonConverter(typeof(SubsystemConfigurationSerializer))]
    public class SubsystemConfiguration
    {
        #region Variables

        /// <summary>
        /// Number of bytes in the SubsystemConfiguration.
        /// </summary>
        public const int NUM_BYTES = 4;

        /// <summary>
        /// Default Configuration value.
        /// </summary>
        public const byte DEFAULT_SETUP = 0x0;

        /// <summary>
        /// Location in the byte array for the Command Setup byte.
        /// </summary>
        private const int COMMAND_SETUP_START = 3;

        #endregion

        #region Properties

        /// <summary>
        /// Subsystem associated with this configuration.
        /// A Subsystem can have mulitple configurations.
        /// </summary>
        public Subsystem SubSystem { get; set; }

        /// <summary>
        /// CEPO index.  This is the location within
        /// the CEPO command that this configuration 
        /// is located.
        /// </summary>
        public byte CepoIndex { get; set; }

        /// <summary>
        /// Index for the SubsystemConfiguration.  Multiple
        /// configurations can be created for a Subsystem.  This
        /// index determines which order the SubsystemConfiguration
        /// is in.
        /// </summary>
        public byte SubsystemConfigIndex { get; set; }

        /// <summary>
        /// Description string as a property.
        /// </summary>
        public string StringDesc { get { return DescString(); } }

        #endregion

        /// <summary>
        /// Default constructor sets the default command setup.
        /// </summary>
        public SubsystemConfiguration()
        {
            SetDefault();
        }

        /// <summary>
        /// Set the Command Setup value.
        /// </summary>
        /// <param name="ss">Subsystem for the configuration.</param>
        /// <param name="cepoIndex">CEPO index.</param>
        /// <param name="ssConfigIndex">SubsystemConfiguration Index.</param>
        [JsonConstructor]
        public SubsystemConfiguration(Subsystem ss, byte cepoIndex, byte ssConfigIndex)
        {
            SubSystem = ss;
            CepoIndex = cepoIndex;
            SubsystemConfigIndex = ssConfigIndex;
        }

        /// <summary>
        /// Receive the Subsystem configuration as a byte 
        /// array and decode the data.
        /// </summary>
        /// <param name="ss">Subsystem for the configuration.</param>
        /// <param name="data">Byte array containing the subsystem configuration data.</param>
        public SubsystemConfiguration(Subsystem ss, byte[] data)
        {
            SubSystem = ss;
            Decode(data);
        }

        /// <summary>
        /// Determine if the configuration is empty.  The
        /// configuration is empty if the default value is
        /// set for the command setup.
        /// </summary>
        /// <returns>TRUE = Empty Configuration.</returns>
        public bool IsEmpty()
        {
            if (SubSystem.IsEmpty() && CepoIndex == DEFAULT_SETUP)
            {
                return true;
            }

            return false;
        }

        #region Methods

        /// <summary>
        /// Convert the Subsystem Configuration into a byte array.
        /// </summary>
        /// <returns>Byte array of the Subsystem Configuration.</returns>
        public byte[] Encode()
        {
            byte[] result = new byte[NUM_BYTES];
            result[0] = (byte)0;
            result[1] = (byte)0;
            result[2] = (byte)0;
            result[COMMAND_SETUP_START] = CepoIndex;

            return result;
        }

        /// <summary>
        /// Decode the byte array for the Subsystem configuration.
        /// 
        /// Set the Command Setup.
        /// </summary>
        /// <param name="data">Byte array to decode.</param>
        private void Decode(byte[] data)
        {
            if (data.Length >= NUM_BYTES)
            {
                CepoIndex = data[COMMAND_SETUP_START];
            }
            else
            {
                SetDefault();
            }
        }

        /// <summary>
        /// Set the default values.
        /// </summary>
        private void SetDefault()
        {
            CepoIndex = DEFAULT_SETUP;
            SubSystem = new Subsystem();
        }

        /// <summary>
        /// Return the string version of the command setup.
        /// This will convert the code from a hex byte value to 
        /// a string character.
        /// </summary>
        /// <returns>Command Setup as a string.</returns>
        public string CommandSetupToString()
        {
            //return Convert.ToString(Convert.ToChar(CommandSetup));
            return Convert.ToString(CepoIndex);
        }

        /// <summary>
        /// Return a description string of this object.  This will
        /// include the configuration number in brackets and the
        /// subsystem description.
        /// 
        /// Ex:
        /// [CEPO Index] Subsystem description
        /// </summary>
        /// <returns>Description string for this object.</returns>
        public string DescString()
        {
            return String.Format("[{0}] {1}", CommandSetupToString(), SubSystem.DescString());
        }

        /// <summary>
        /// Return the CEPO index and the Subsystem code for this configuration.
        /// 
        /// Ex:
        /// [CEPO Index] Code
        /// </summary>
        /// <returns>[CEPO Index] Code</returns>
        public string IndexCodeString()
        {
            return String.Format("[{0}] {1}", CommandSetupToString(), SubSystem.CodeToString());
        }

        #endregion

        #region Overrides

        ///// <summary>
        ///// Return the CommandSetup as a string.
        ///// </summary>
        ///// <returns>CommandSetup as as string.</returns>
        //public override string ToString()
        //{
        //    return CommandSetupToString();
        //}

        /// <summary>
        /// Determine if the 2 SubsystemConfigurations given are the equal.
        /// </summary>
        /// <param name="config1">First SubsystemConfigurations to check.</param>
        /// <param name="config2">SubsystemConfigurations to check against.</param>
        /// <returns>True if there CommandSetup match.</returns>
        public static bool operator ==(SubsystemConfiguration config1, SubsystemConfiguration config2)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(config1, config2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)config1 == null) || ((object)config2 == null))
            {
                return false;
            }

            // Return true if the fields match:
            return (config1.SubSystem == config2.SubSystem && config1.CepoIndex == config2.CepoIndex && config1.SubsystemConfigIndex == config2.SubsystemConfigIndex);
        }

        /// <summary>
        /// Return the opposite of ==.
        /// </summary>
        /// <param name="config1">First SubsystemConfigurations to check.</param>
        /// <param name="config2">SubsystemConfigurations to check against.</param>
        /// <returns>Return the opposite of ==.</returns>
        public static bool operator !=(SubsystemConfiguration config1, SubsystemConfiguration config2)
        {
            return !(config1 == config2);
        }

        /// <summary>
        /// Create a hashcode based off the CommandSetup stored.
        /// </summary>
        /// <returns>Hash the Code.</returns>
        public override int GetHashCode()
        {
            return CepoIndex.GetHashCode();
        }

        /// <summary>
        /// Check if the given object is 
        /// equal to this object.
        /// </summary>
        /// <param name="obj">Object to check.</param>
        /// <returns>If the codes are the same, then they are equal.</returns>
        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            SubsystemConfiguration p = (SubsystemConfiguration)obj;

            return (SubSystem == p.SubSystem && CepoIndex == p.CepoIndex && SubsystemConfigIndex == p.SubsystemConfigIndex);
        }

        #endregion
    }

    #region JSON

    /// <summary>
    /// Convert this object to a JSON object.
    /// Calling this method is twice as fast as calling the default serializer:
    /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.SubsystemConfiguration).
    /// 
    /// 50ms for this method.
    /// 100ms for calling SerializeObject default.
    /// 
    /// Use this method whenever possible to convert to JSON.
    /// 
    /// http://james.newtonking.com/projects/json/help/
    /// http://james.newtonking.com/projects/json/help/index.html?topic=html/ReadingWritingJSON.htm
    /// http://blog.maskalik.com/asp-net/json-net-implement-custom-serialization
    /// </summary>
    public class SubsystemConfigurationSerializer : JsonConverter
    {
        #region JSON Names

        /// <summary>
        /// JSON object name for SubSystem.
        /// </summary>
        public const string JSON_STR_SUBSYSTEM = "SubSystem";


        /// <summary>
        /// JSON object name for CepoIndex.
        /// </summary>
        public const string JSON_STR_CEPO_INDEX = "CepoIndex";

        /// <summary>
        /// JSON object name for SubsystemConfigIndex.
        /// </summary>
        public const string JSON_STR_SUBSYSTEM_CONFIG_INDEX = "SubsystemConfigIndex";

        #endregion

        /// <summary>
        /// Write the JSON string.  This will convert all the properties to a JSON string.
        /// This is done manaully to improve conversion time.  The default serializer will check
        /// each property if it can convert.  This will convert the properties automatically.  This
        /// will double the speed.
        /// 
        /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.SubsystemConfiguration).
        /// 
        /// </summary>
        /// <param name="writer">JSON Writer.</param>
        /// <param name="value">Object to write to JSON.</param>
        /// <param name="serializer">Serializer to convert the object.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Cast the object
            var data = value as SubsystemConfiguration;

            // Start the object
            writer.Formatting = Formatting.None;                    // Make the text not indented, so not as human readable.  This will save disk space
            writer.WriteStartObject();                              // Start the JSON object

            // SubSystem
            writer.WritePropertyName(JSON_STR_SUBSYSTEM);                                           // Subsystem name
            writer.WriteRawValue(Newtonsoft.Json.JsonConvert.SerializeObject(data.SubSystem));      // Subsystem value

            // CEPO Index
            writer.WritePropertyName(JSON_STR_CEPO_INDEX);                                          // CEPO Index name
            writer.WriteValue(data.CepoIndex);                                                      // CEPO Index value

            // Subsystem Config index
            writer.WritePropertyName(JSON_STR_SUBSYSTEM_CONFIG_INDEX);                              // Subsystem Config Index name
            writer.WriteValue(data.SubsystemConfigIndex);                                           // Subsystem Config Index value

            // End the object
            writer.WriteEndObject();
        }

        /// <summary>
        /// Read the JSON object and convert to the object.  This will allow the serializer to
        /// automatically convert the object.  No special instructions need to be done and all
        /// the properties found in the JSON string need to be used.
        /// 
        /// SubsystemConfiguration subsysConfig = Newtonsoft.Json.JsonConvert.DeserializeObject{ensemble.SubsystemConfiguration}(encodedSubsystemConfig)
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

                byte cepoIndex = jsonObject[JSON_STR_CEPO_INDEX].ToObject<byte>(); ;
                byte configIndex = jsonObject[JSON_STR_SUBSYSTEM_CONFIG_INDEX].ToObject<byte>(); ;
                Subsystem ss = jsonObject[JSON_STR_SUBSYSTEM].ToObject<Subsystem>();

                return new SubsystemConfiguration(ss, cepoIndex, configIndex);
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
            return typeof(SubsystemConfiguration).IsAssignableFrom(objectType);
        }
    }

    #endregion
}
