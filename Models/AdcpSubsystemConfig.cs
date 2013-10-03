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
 * 09/21/2012      RC          2.15       Initial coding
 * 10/08/2012      RC          2.15       Set the Command's CEPO index when CEPO index is set.
 * 10/12/2012      RC          2.15       Made ToString() only use Subsystem and SubystemConfiguration.
 *                                         Added static GetString() to generate a string for a AdcpSubsystemConfig based off a Subsystem and SubsystemConfiguration given.
 * 11/30/2012      RC          2.17       Changed ToString() and GetString() to [Config] SSDesc.
 *                                         Added a note about the subsystem code being EMPTY.
 * 12/28/2012      RC          2.17       Moved AdcpSubsystemConfig.Subsystem into AdcpSubsystemConfig.SubsystemConfig.Subsystem.
 * 05/30/2013      RC          2.19       Take SubsystemConfiguration in the constructor and remove CepoIndex.  CepoIndex is in SubsystemConfiguration.  Added Display.
 * 
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using RTI.Commands;
    using Newtonsoft.Json;

    /// <summary> 
    /// A Configuration is a setup on the ADCP.  An ADCP can have multiple configurations.
    /// Each configuration will have different command settings.  A configuration is based 
    /// off a subsystem type.  The CEPO command determine the ping order of each configuration.
    /// This object will store the subsytem type and configuration number (CEPO index).  It will also store all
    /// the command settings for this configuration.
    /// </summary>
    public class AdcpSubsystemConfig
    {
        #region Properties

        /// <summary>
        /// SubsystemConfiguration for the ADCP.
        /// </summary>
        public SubsystemConfiguration SubsystemConfig { get; set; }

        /// <summary>
        /// Subsystem commands associated with this configuration.
        /// </summary>
        public AdcpSubsystemCommands Commands { get; set; }

        /// <summary>
        /// Interface a user can setup to give predictions
        /// on the ADCP usage.
        /// </summary>
        [JsonIgnore]
        public IPredictor Predictor { get; set; }

        /// <summary>
        /// Used to display in a list.
        /// Set the DisplayMember of the combobox.
        /// http://stackoverflow.com/questions/3664956/c-sharp-combobox-overridden-tostring
        /// </summary>
        public string Display { get { return SubsystemConfig.SubSystem.CodedDescString(); } }

        #endregion

        /// <summary>
        /// Set the default values for the configuration.
        /// </summary>
        public AdcpSubsystemConfig()
        {
            SetDefault();
        }

        /// <summary>
        /// Set the Subsystem, SubsystemConfiguration and index for the object.
        /// This will also create SubsystemCommands with default values.
        /// </summary>
        /// <param name="ssConfig">Subsystem Configuration.</param>
        public AdcpSubsystemConfig(SubsystemConfiguration ssConfig)
        {
            SubsystemConfig = ssConfig;
            Commands = new AdcpSubsystemCommands(ssConfig);            // Create commands before settings CepoIndex
        }

        /// <summary>
        /// Set the default values.  At the very least, the index
        /// should be set correctly, or it will be reordered when the
        /// CEPO command is constructed.
        /// This will also create SubsystemCommands with default values.
        /// </summary>
        public void SetDefault()
        {
            //Subsystem = new Subsystem();
            SubsystemConfig = new SubsystemConfiguration();
            Commands = new AdcpSubsystemCommands(SubsystemConfig);            // Create commands before settings CepoIndex
        }

        /// <summary>
        /// Generate the string for a AdcpSubsystemConfig. Typically the
        /// ToString() is used as a key for the AdcpSubsystemConfig object.
        /// This will generate a string for the key so that an AdcpSubsystemConfig
        /// can be found in a dictionary.
        /// 
        /// [SSCONFIG] SSDesc
        /// 
        /// NOTE
        /// If the subsystem is an old revision of firmware,
        /// the subsystem code will be an index
        /// instead of the code.  Use the index and the
        /// serial number to determine the string. 
        /// 
        /// </summary>
        /// <param name="ssConfig">SubsystemConfiguration for the AdcpSubsystemConfig.</param>
        /// <returns>String for the AdcpSubsystemConfig if it used the given Subsystem and SubsystemConfiguration.</returns>
        public static string GetString(SubsystemConfiguration ssConfig)
        {
            // NOTE
            // If the subsystem is an old revision of firmware,
            // the subsystem code will be an index
            // instead of the code.  Use the index and the
            // serial number to determine the string

            return ssConfig.DescString();
        }

        #region Overrides

        /// <summary>
        /// Get a string for this object.  It will contain:
        /// [SSCONFIG] SSDesc
        /// 
        /// NOTE
        /// If the subsystem is an old revision of firmware,
        /// the subsystem code will be an index
        /// instead of the code.  Use the index and the
        /// serial number to determine the string. 
        /// 
        /// </summary>
        /// <returns>String of this object.</returns>
        public override string ToString()
        {
            // NOTE
            // If the subsystem is an old revision of firmware,
            // the subsystem code will be an index
            // instead of the code.  Use the index and the
            // serial number to determine the string

            return SubsystemConfig.DescString();
        }

        /// <summary>
        /// Hashcode for the object.
        /// This will return the hashcode for the
        /// serial number string.
        /// </summary>
        /// <returns>Hashcode for the object.</returns>
        public override int GetHashCode()
        {
            return SubsystemConfig.SubSystem.GetHashCode() + SubsystemConfig.GetHashCode() + SubsystemConfig.CepoIndex;
        }

        /// <summary>
        /// Determine if the given object is equal to this
        /// object.  This will check the Subsystem, SubsystemConfiguration and Index 
        /// are all the same.
        /// </summary>
        /// <param name="obj">Object to compare with this object.</param>
        /// <returns>TRUE = AdcpSubsystemConfig are the same.</returns>
        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            AdcpSubsystemConfig p = (AdcpSubsystemConfig)obj;

            return (SubsystemConfig == p.SubsystemConfig);
        }

        /// <summary>
        /// Determine if the two AdcpSubsystemConfigs given are equal.
        /// </summary>
        /// <param name="asConfig1">First AdcpSubsystemConfig to check.</param>
        /// <param name="asConfig2">AdcpSubsystemConfigs to check against.</param>
        /// <returns>True if there properties match.</returns>
        public static bool operator ==(AdcpSubsystemConfig asConfig1, AdcpSubsystemConfig asConfig2)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(asConfig1, asConfig2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)asConfig1 == null) || ((object)asConfig2 == null))
            {
                return false;
            }

            // Return true if the fields match:
            return (asConfig1.SubsystemConfig == asConfig2.SubsystemConfig);
        }

        /// <summary>
        /// Determine if the two AdcpSubsystemConfigs given are not equal.
        /// Opposite of ==.
        /// </summary>
        /// <param name="asConfig1">First AdcpSubsystemConfig to check.</param>
        /// <param name="asConfig2">AdcpSubsystemConfigs to check against.</param>
        /// <returns>True if there properties do not match.</returns>
        public static bool operator !=(AdcpSubsystemConfig asConfig1, AdcpSubsystemConfig asConfig2)
        {
            return !(asConfig1 == asConfig2);
        }

        #endregion
    }
}
