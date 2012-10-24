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
 * 
 */


using System;


namespace RTI
{

    /// <summary>
    /// Identifies which command setup is being used
    /// for the current subsystem.
    /// 
    /// Use this configuration with the Subsystem Type to
    /// separate the ensemble for each setup/subsystem.
    /// </summary>
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
        /// Number that identifies which command setup is being
        /// used for the current subsystem.
        /// </summary>
        public byte CommandSetup { get; set; }


        #endregion

        /// <summary>
        /// Represents an empty or blank subsystem configuration.
        /// </summary>
        public static readonly SubsystemConfiguration Empty = new SubsystemConfiguration();

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
        /// <param name="cmdSetup">Set the command setup.</param>
        public SubsystemConfiguration(byte cmdSetup)
        {
            CommandSetup = cmdSetup;
        }

        /// <summary>
        /// Receive the Subsystem configuration as a byte 
        /// array and decode the data.
        /// </summary>
        /// <param name="data">Byte array containing the subsystem configuration data.</param>
        public SubsystemConfiguration(byte[] data)
        {
            Decode(data);
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
            result[COMMAND_SETUP_START] = CommandSetup;

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
                CommandSetup = data[COMMAND_SETUP_START];
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
            CommandSetup = DEFAULT_SETUP;
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
            return Convert.ToString(CommandSetup);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Return the CommandSetup as a string.
        /// </summary>
        /// <returns>CommandSetup as as string.</returns>
        public override string ToString()
        {
            return CommandSetupToString();
        }

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
            return (config1.CommandSetup == config2.CommandSetup);
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
            return CommandSetup.GetHashCode();
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

            return (CommandSetup == p.CommandSetup);
        }

        #endregion
    }
}
