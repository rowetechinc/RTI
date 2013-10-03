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
 * 09/22/2012      RC          2.15       Initial coding
 * 10/01/2012      RC          2.15       Added DeploymentOptions to AdcpConfiguration.
 *                                         Added ValidateCEPO() to validate a given CEPO.
 * 10/02/2012      RC          2.15       Added methods AdcpSubsystemConfigExist() and GetAdcpSubsystemConfig().
 *                                         Added method AddConfiguration().
 * 10/03/2012      RC          2.15       Fixed bug in SetCepo() when setting the serial number, the CEPO was being reset.
 * 10/08/2012      RC          2.15       Added RemoveConfiguration().
 * 10/12/2012      RC          2.15       Improved performance in GetAdcpSubsystemConfig() and AdcpSubsystemConfigExist().
 * 12/28/2012      RC          2.17       Made SubsystemConfiguration take a Subsystem in its constructor.
 *                                         Moved AdcpSubsystemConfig.Subsystem into AdcpSubsystemConfig.SubsystemConfig.Subsystem.
 *                                         AdcpSubsystemConfigExist(), RemoveAdcpSubsystemConfig() and GetAdcpSubsystemConfig()  take only 1 argument.
 * 01/02/2013      RC          2.17       Set a default CEPO when a serial number is give in the constructor.
 * 01/14/2013      RC          2.17       In DecodeCepo() fixed converting the subsystem code from a char to a decimal using MathHelper.
 * 05/09/2013      RC          2.19       Removed the redundant CEPO command and only use the CEPO in the Commands property.
 * 07/26/2013      RC          2.19.3     In SubsystemConfiguration constructor, i give the CEPO index now.
 * 09/25/2013      RC          2.20.1     Added SerialOptions to keep track of the serial connection for this project.
 * 09/30/2013      RC          2.20.2     Fixed bug in AddConfig() when setting the Configuration Index and the CEPO index.
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using RTI.Commands;
    using System.Collections;


    /// <summary>
    /// Keep track of the configuration of the ADCP.  This 
    /// will keep track of how many subsystems and SubsystemConfigurations
    /// exist on the ADCP.  
    /// Each ADCP can have N number of Subsystems and M number of SubsystemConfigurations for each Subsystem.
    /// At the very least, an ADCP will have 1 Subsystem with 1 SubsystemConfiguration.
    /// This will keep a list of all the combinations the system has.
    /// 
    /// The configuration can be determine by CEPO (Ensemble Ping Order command).
    /// </summary>
    public class AdcpConfiguration
    {

        #region Properties

        /// <summary>
        /// Dictionary of all the available AdcpSubsystemConfig.  You check if a
        /// AdcpSubsystemConfig exist, by checking the keys.  If you need the actual
        /// AdcpSubsystemConfig, you can get it by the value.  The key will be
        /// AdcpSubsystemConfig.ToString().
        /// </summary>
        public Dictionary<string, AdcpSubsystemConfig> SubsystemConfigDict { get; set; }

        /// <summary>
        /// Serial Number for the ADCP.
        /// </summary>
        private SerialNumber _serialNumber;
        /// <summary>
        /// Serial Number for the ADCP.
        /// </summary>
        public SerialNumber SerialNumber
        {
            get { return _serialNumber; } 
            set
            {
                if (value != _serialNumber)
                {
                    _serialNumber = value;

                    // Clear CEPO
                    Commands.CEPO = AdcpCommands.DEFAULT_CEPO;

                    // Clear the dictionary
                    SubsystemConfigDict.Clear();
                }
            }
        }

        /// <summary>
        /// ADCP commands.
        /// </summary>
        public AdcpCommands Commands { get; set; }

        /// <summary>
        /// Deployment options.
        /// </summary>
        public DeploymentOptions DeploymentOptions { get; set; }

        /// <summary>
        /// Serial connection options for the ADCP.
        /// </summary>
        public AdcpSerialPort.AdcpSerialOptions SerialOptions { get; set; }

        #endregion

        /// <summary>
        /// Initialize values.
        /// </summary>
        public AdcpConfiguration()
        {
            // Initialize values
            SubsystemConfigDict = new Dictionary<string, AdcpSubsystemConfig>();
            Commands = new AdcpCommands();
            _serialNumber = new SerialNumber();
            DeploymentOptions = new DeploymentOptions();
            SerialOptions = new AdcpSerialPort.AdcpSerialOptions();
        }

        /// <summary>
        /// Initialize values.
        /// </summary>
        public AdcpConfiguration(SerialNumber serial)
        {
            // Initialize values
            SubsystemConfigDict = new Dictionary<string, AdcpSubsystemConfig>();
            Commands = new AdcpCommands();
            _serialNumber = serial; 
            SetCepo(_serialNumber.SubsystemsString(), _serialNumber);       // Must go after Commands is created
            DeploymentOptions = new DeploymentOptions();
            SerialOptions = new AdcpSerialPort.AdcpSerialOptions();
        }

        #region Methods

        #region Configuration

        /// <summary>
        /// Take the CEPO command and decode the command
        /// for the ADCP configuration.
        /// </summary>
        /// <param name="cepo">CEPO command value.</param>
        /// <param name="serial">Serial number to determine the system type.</param>
        /// <returns>Returns a dictionary with all the AdcpSubsystemConfigs created.</returns>
        public Dictionary<string, AdcpSubsystemConfig> SetCepo(string cepo, SerialNumber serial)
        {
            // Verify the CEPO given is valid
            if (ValidateCEPO(cepo, serial))
            {
                // Set CEPO and serial
                //CEPO = cepo;
                Commands.CEPO = cepo;
                _serialNumber = serial;         // Set the private property for serial number or CEPO will be reset

                // Decode CEPO command
                DecodeCepo(cepo, serial);
            }

            // Return the dictionary
            return SubsystemConfigDict;
        }

        /// <summary>
        /// Determine if the AdcpSubsystemConfig exist in the dictionary.  This will generate
        /// a key based off the Subsystem and SubsystemConfiguration given.  It will then
        /// check if the key exist in the dictionary.
        /// </summary>
        /// <param name="ssConfig">SubsystemConfiguration.</param>
        /// <returns>TRUE = Subsystem and SubsystemConfiguration key found.  /  FALSE = No AdcpSubsystemConfig key.</returns>
        public bool AdcpSubsystemConfigExist(SubsystemConfiguration ssConfig)
        {
            // Check for null
            if (ssConfig == null)
            {
                return false;
            }

            //return SubsystemConfigDict.ContainsKey(AdcpSubsystemConfig.GetString(ssConfig));
            return SubsystemConfigDict.ContainsKey(ssConfig.DescString());
        }

        /// <summary>
        /// Remove the AdcpSubsystemConfig from the dictionary.  If it exist in the
        /// dictionary, the AdcpSubsystemConfigs in the dictionary have to reordered.
        /// If it did not exist in the dictionary, do nothing and return false.
        /// 
        /// Create a temporary list of all the configurations in the order
        /// of the CEPO command.  Then add the configurations back to the dictonary
        /// and create a new CEPO command.
        /// </summary>
        /// <param name="config">AdcpSubsystemConfig to remove.</param>
        /// <returns>TRUE = Config removed / FALSE = Config did not exist in the dictonary.  Nothing done.</returns>
        public bool RemoveAdcpSubsystemConfig(AdcpSubsystemConfig config)
        {
            if (config != null)
            {
                // Remove the AdcpSubsystemConfig from the dict
                // If it is removed, all the remaining configurations needed
                // to be renumbered
                if (SubsystemConfigDict.Remove(config.ToString()))
                {
                    // Remove all the configurations from the dictionary and put in an sorted list by CEPO index
                    SortedList<int, AdcpSubsystemConfig> tempList = new SortedList<int, AdcpSubsystemConfig>();
                    //List<AdcpSubsystemConfig> tempList = new List<AdcpSubsystemConfig>();
                    foreach (AdcpSubsystemConfig asConfig in SubsystemConfigDict.Values)
                    {
                        tempList.Add(asConfig.SubsystemConfig.CepoIndex, asConfig);
                        //tempList.Add(asConfig);
                    }

                    // Clear the dictionary and the CEPO command
                    //CEPO = "";
                    Commands.CEPO = AdcpCommands.DEFAULT_CEPO;
                    SubsystemConfigDict.Clear();

                    // Redo the CEPO command
                    // and add the configuration back to the dictionary
                    for (int x = 0; x < tempList.Count; x++)
                    {
                        // Redo the cepo value
                        Commands.CEPO += Convert.ToChar(tempList.Values[x].SubsystemConfig.SubSystem.Code);

                        // Add config to the dictionary
                        AddConfig(tempList.Values[x]);
                    }

                    return true;
                }
            }

            // Config was not in the list so the configuration was not reordered
            return false;
        }

        /// <summary>
        /// Get the AdcpSubsystemConfig from the dictionary if it exist.  If it does not
        /// exist in the dictionary, null will be returned.
        /// </summary>
        /// <param name="ssConfig">SubsystemConfiguration.</param>
        /// <returns>If the AdcpSubystemConfig is found, it will return the AdcpSubsystemConfig.  If it is not found, it will return null.</returns>
        public AdcpSubsystemConfig GetAdcpSubsystemConfig(SubsystemConfiguration ssConfig)
        {
            // Check for null
            if (ssConfig == null)
            {
                return null;
            }

            // Generate the key for the Subsystem and SubsystemConfiguration
            string key = AdcpSubsystemConfig.GetString(ssConfig);

            // If the key exist, return the object
            if (SubsystemConfigDict.ContainsKey(key))
            {
                return SubsystemConfigDict[key];
            }

            // The key did not exist so return null
            return null;

        }

        /// <summary>
        /// Add a new configuration.  This will take a Subsystem as a parameter.
        /// It will then create a new configuration for the given Subsystem.
        /// It will update the CEPO command and it will add the new Configuration
        /// to the dictionary.
        /// </summary>
        /// <param name="ss">Subsystem for the configuration.</param>
        /// <param name="asConfig">Return the AdcpSubsystemConfig created.</param>
        /// <returns>TRUE = Configuration Added. / FALSE = Configuration could not be added.</returns>
        public bool AddConfiguration(Subsystem ss, out AdcpSubsystemConfig asConfig)
        {
            // Initialize the AdcpSubsystemConfig to null
            asConfig = null;

            // Generate a new CEPO
            //string cepo = CEPO + Convert.ToChar(ss.Code);
            string cepo = Commands.CEPO + Convert.ToChar(ss.Code);

            // Validate the new CEPO
            // If it pass, then add the new configuration to the dictionary
            if (ValidateCEPO(cepo, SerialNumber))
            {
                // Set the CEPO
                //CEPO = cepo;
                Commands.CEPO = cepo;

                // Get the CEPO index
                // The index will be the last character in the CEPO command
                // Subtract 1 because it is 0 based
                int cepoIndex = Commands.CEPO.Length - 1;

                // Add the configuration to the dictionary
                // Set the AdcpSubsystemConfig to give to the user
                asConfig = AddConfig(ss, cepoIndex);

                return true;
            }

            return false;
        }

        #endregion

        #region Decode

        /// <summary>
        /// Validate the CEPO value.  This will look at each value in the CEPO and
        /// verify a Subsystem code exist in the serial number.  If any value in CEPO
        /// is not found as a Subsystem in the serial number, the method will return FALSE.
        /// </summary>
        /// <param name="cepo">CEPO command.</param>
        /// <param name="serial">Adcp Serial number.</param>
        /// <returns>TRUE = CEPO is valid / FALSE =  CEPO given is invalid.</returns>
        public static bool ValidateCEPO(string cepo, SerialNumber serial)
        {
            // Verify a string was given
            if (string.IsNullOrEmpty(cepo))
            {
                return false;
            }
            // Assume good
            bool result = true;


            // Check if the given Subsystems in the CEPO string exist
            // in the serial number
            Dictionary<byte, Subsystem> dict = serial.SubSystemsDict;
            for (int cepoIndex = 0; cepoIndex < cepo.Length; cepoIndex++)
            {
                bool test = false;

                foreach(Subsystem ss in dict.Values)
                {
                    // If the CEPO value matched a subsystem code
                    // Then it was a valid value
                    if (cepo[cepoIndex] == ss.Code)
                    {
                        test = true;
                    }
                }

                // If CEPO value never matched any Subsystem code
                // Then it was a bad value and return false
                if (test == false)
                {
                    return false;
                }
            }


            return result;
        }

        /// <summary>
        /// Decode the CEPO command and populate the dictionary.
        /// 
        /// Ex:
        /// CEPO 222
        /// 1 Subsystem
        /// 3 SubsystemConfigurations for Subsystem 2
        /// 
        /// CEPO 232
        /// 2 Subsystems
        /// 2 SubsystemConfigurations for Subsystem 2 and
        /// 1 SubsystemConfiguration for Subsystem 3
        /// 
        /// </summary>
        /// <param name="cepo">CEPO command to decode.</param>
        /// <param name="serial">Serial number to determine the system type.</param>
        /// <returns>Dictionary of all subsystem configurations found.</returns>
        private Dictionary<string, AdcpSubsystemConfig> DecodeCepo(string cepo, SerialNumber serial)
        {
            // Clear the current dictionary
            SubsystemConfigDict.Clear();

            // Add each configuration in the command
            for (int x = 0; x < cepo.Length; x++)
            {
                AddConfig(Subsystem.ConvertSubsystemCode(cepo, x), x, serial);
            }
            
            // Return the populated dictionary
            return SubsystemConfigDict;
        }

        /// <summary>
        /// Get the CEPO configuration character and index within the CEPO command.
        /// The configuration character is the Subsystem code for the configuration.  It
        /// represents the system type.  The Index represents where in teh CEPO command
        /// the configuration was located.  This determines the ping order of the configurations.
        /// It also make the configuration unique for a SubsystemConfiguration for a Subsystem.
        /// Get the Subsystem using the serial number and subsystem code.
        /// 
        /// SubsystemConfiguaration CommandSetup: Index of the Configuration within Subsystem.  (Based off counting configurations for a subsystem)
        /// index: Location in CEPO for the Subsystem configuration.
        /// </summary>
        /// <param name="ssCode">Subsystem Code from the CEPO command.</param>
        /// <param name="cepoIndex">Location in the CEPO command of the Subsystem Code.</param>
        /// <param name="serial">Serial number for the ADCP.</param>
        /// <returns>Return the AdcpSubsystemConfig created or null if one could not be created.</returns>
        private AdcpSubsystemConfig AddConfig(byte ssCode, int cepoIndex, SerialNumber serial)
        {
            AdcpSubsystemConfig asConfig = null;

            // Get the Subsystem index from the serial number
            // If it cannot be found in the serial number, then it is
            // a bad Subsystem and we can not use the command.
            Subsystem ss = serial.GetSubsystem(ssCode);
            if (!ss.IsEmpty())
            {

                // Determine how many of the given subsystem have been added to the dictionary
                // We need to generate SubsystemConfiguration index value.
                // SubsystemConfiguration index is based off the number of SubsystemConfigurations already
                // in the dictionary before this configuration is added.
                ushort ssCount = 0;
                foreach (AdcpSubsystemConfig configuration in SubsystemConfigDict.Values)
                {
                    // If the subsystems are the same, then increment the value
                    if (configuration.SubsystemConfig.SubSystem.Code == ssCode)
                    {
                        ssCount++;
                    }
                }

                // Create all the subsystem configurations and add to the dictionary
                SubsystemConfiguration ssConfig = new SubsystemConfiguration(ss, (byte)cepoIndex, (byte)ssCount);   // SubsystemConfiguration with the CEPO index and Index of the SubsystemConfiguration
                asConfig = new AdcpSubsystemConfig(ssConfig);                                                       // AdcpSubsystemConfig with the Subsystem, SubsystemConfig and CEPO index
                SubsystemConfigDict.Add(asConfig.ToString(), asConfig);                                             // Add to the dictionary the configuration with the string as the key
            }

            return asConfig;
        }

        /// <summary>
        /// Add a given AdcpSubsystemConfig to the dictionary.  This will determine
        /// how many of the given subsystem type have already been added to the dictionary
        /// and generate a new index value.  It will then set the index value to the SubsysteConfiguration.
        /// It will then add the AdcpSubsystemConfig to the dictionary.
        /// </summary>
        /// <param name="asConfig">AdcpSubsystemConfig to add to the dictionary.</param>
        private void AddConfig(AdcpSubsystemConfig asConfig)
        {
            // Determine how many of the given subsystem have been added to the dictionary
            // We need to generate SubsystemConfiguration index value.
            // SubsystemConfiguration index is based off the number of SubsystemConfigurations already
            // in the dictionary before this configuration is added.
            ushort ssCount = 0;
            foreach (AdcpSubsystemConfig configuration in SubsystemConfigDict.Values)
            {
                // If the subsystems are the same, then increment the value
                if (configuration.SubsystemConfig.SubSystem.Code == asConfig.SubsystemConfig.SubSystem.Code)
                {
                    ssCount++;
                }
            }

            // Set the new Configuration index
            asConfig.SubsystemConfig.SubsystemConfigIndex = (byte)ssCount;

            // Set the new CEPO index
            // This is the last index for the CEPO command
            asConfig.SubsystemConfig.CepoIndex = Convert.ToByte(SubsystemConfigDict.Values.Count);

            // Add it to the dictionary
            SubsystemConfigDict.Add(asConfig.ToString(), asConfig);
        }

        /// <summary>
        /// Add a configuration to the dictionary.  This will create an AdcpSubsystemConfig based
        /// off the Subsystem and CEPO index given.  It will then add it to the dictionary.  It will
        /// then return the created AdcpSubsystemConfig.  If the AdcpSubsystemConfig could not be
        /// created, null will be returned.
        /// </summary>
        /// <param name="ss">Subsystem to add a configuration.</param>
        /// <param name="cepoIndex">CEPO index.</param>
        /// <returns>AdcpSubsystemConfig created with the given settings or null if one could not be created.</returns>
        private AdcpSubsystemConfig AddConfig(Subsystem ss, int cepoIndex)
        {
            AdcpSubsystemConfig asConfig = null;

            // If a bad Subsystem is given, we cannot use it
            if (!ss.IsEmpty())
            {
                // Determine how many of the given subsystem have been added to the dictionary
                // We need to generate SubsystemConfiguration index value.
                // SubsystemConfiguration index is based off the number of SubsystemConfigurations already
                // in the dictionary before this configuration is added.
                ushort ssCount = 0;
                foreach (AdcpSubsystemConfig configuration in SubsystemConfigDict.Values)
                {
                    // If the subsystems are the same, then increment the value
                    if (configuration.SubsystemConfig.SubSystem.Code == ss.Code)
                    {
                        ssCount++;
                    }
                }

                // Create all the subsystem configurations and add to the dictionary
                SubsystemConfiguration ssConfig = new SubsystemConfiguration(ss, (byte)cepoIndex, (byte)ssCount);    // SubsystemConfiguration with the Index of the SubsystemConfiguration
                asConfig = new AdcpSubsystemConfig(ssConfig);                        // AdcpSubsystemConfig with the Subsystem, SubsystemConfig and CEPO index
                if (!SubsystemConfigDict.ContainsKey(asConfig.ToString()))
                {
                    SubsystemConfigDict.Add(asConfig.ToString(), asConfig);
                }
            }

            return asConfig;
        }

        #endregion

        #endregion

    }
}
