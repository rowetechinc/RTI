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
 * 01/20/2012      RC          1.14       Initial coding
 * 01/26/2012      RC          1.14       Changed constructor to take a byte instead of string for subsystem.
 * 07/20/2012      Rc          2.12       Added FirmwareVersionList() to get a list of all possible firmware major, minor and revision values.
 * 10/09/2012      RC          2.15       Changed SubsystemIndex to SubsystemCode.
 * 12/28/2012      RC          2.17       Added GetSubsystem() to get the subsystem for this firmware.
 * 01/04/2013      RC          2.17       Added equal, == and != to the object.
 *                                         Fixed ToString() for the subsystem code.
 * 01/09/2013      RC          2.17       Made SubsystemCode private and you now must get the code using the function GetSubsystemCode().
 *                                         In GetSubsystemCode() and GetSubsystem() converted the code to a byte correctly.
 * 01/17/2013      RC          2.17       Added DEBUG_MAJOR_VER for a debug firmware flag.
 * 02/21/2013      RC          2.18       In GetSubsystemCode() and GetSubsystem(), check if the serial number given is a DVL serial number.  If so, handle differently.
 * 02/26/2013      RC          2.18       Maded SubsystemCode public so it can be decoded for JSON.
 * 02/19/2014      RC          2.21.3     In ToString(), display the SubSystemCode as a char.
 *
 */

using System;
using System.Collections.Generic;
namespace RTI
{
    /// <summary>
    /// A class to describe the system firmware version.
    /// This will include the hardware sub-system, the 
    /// major and minor firmware version and the 
    /// firmware revision.
    /// </summary>
    public class Firmware
    {

        #region Variables

        /// <summary>
        /// Number of bytes in the Firmware.
        /// </summary>
        public const int NUM_BYTES = 4;

        /// <summary>
        /// Location of the Hardware sub-system.
        /// </summary>
        public const int SUBSYSTEM_START = 3;

        /// <summary>
        /// Location of the Major Firmware version.
        /// </summary>
        public const int MAJOR_START = 2;

        /// <summary>
        /// Location of the Minor Firmware version.
        /// </summary>
        public const int MINOR_START = 1;

        /// <summary>
        /// Location of the Firmware revsion.
        /// </summary>
        public const int REVISION_START = 0;

        /// <summary>
        /// If the firmware is a debug version, 
        /// the Major version number would be set to 99.
        /// </summary>
        public const int DEBUG_MAJOR_VER = 99;

        #endregion

        #region Properties

        /// <summary>
        /// Major Firmware version.
        /// This is an unsigned int between 0 and 255.
        /// </summary>
        public UInt16 FirmwareMajor { get; set; }

        /// <summary>
        /// Minor Firmware version.
        /// This is an unsigned int between 0 and 255.
        /// </summary>
        public UInt16 FirmwareMinor { get; set; }

        /// <summary>
        /// Firmware revision.
        /// This is an unsigned int between 0 and 255.
        /// </summary>
        public UInt16 FirmwareRevision { get; set; }

        /// <summary>
        /// The code should be the hex value for the character in the serial number 
        /// for the subsystem.  
        /// You can convert the character to the hex byte 
        /// with the following code:
        /// (byte)System.Convert.ToUInt32(code[0]);
        /// or
        /// decimal subsysCode = Convert.ToChar(cepo.Substring(x, 1));
        /// (byte)subsysCode;
        /// 
        /// Because this value can vary in its meaning based off firmware versions,
        /// i have made it private.  You now must use GetSubsystemCode() and give a 
        /// serial number to get the true SubsystemCode.  Before firmware version 0.2.13,
        /// the value given in the ensemble was the SubsystemIndex within the serial number.
        /// Afterwards it changed to the actual SubsystemCode.
        /// </summary>
        public byte SubsystemCode { get; set; } 

        #endregion


        /// <summary>
        /// Set the values to nothing.
        /// </summary>
        public Firmware()
        {
            FirmwareMajor = 0;
            FirmwareMinor = 0;
            FirmwareRevision = 0;
            SubsystemCode = Subsystem.EMPTY_CODE;
        }

        /// <summary>
        /// Set the Firmware based off 
        /// the byte array given.
        /// </summary>
        /// <param name="firmware">Firmware data.</param>
        public Firmware(byte[] firmware)
        {
            Decode(firmware);
        }

        /// <summary>
        /// Constructor that takes all the values.
        /// </summary>
        /// <param name="subSystem">Sub-system of ensemble.</param>
        /// <param name="major">Firmware major version.</param>
        /// <param name="minor">Firmware minor version.</param>
        /// <param name="revision">Firmware revision.</param>
        public Firmware(byte subSystem, UInt16 major, UInt16 minor, UInt16 revision)
        {
            FirmwareMajor = major;
            FirmwareMinor = minor;
            FirmwareRevision = revision;
            SubsystemCode = subSystem;
        }

        /// <summary>
        /// Decode the firmware into its 
        /// parts.  There will be 4 bytes.
        /// Most significant[3]: Hardware Sub-system code.
        /// [2]: Major Firmware Version.
        /// [1]: Minor Firmware Version.
        /// Least significant[0]: Firmware Revision
        /// </summary>
        /// <param name="firmware">Firmware data.</param>
        private void Decode(byte[] firmware)
        {
            FirmwareMajor = Convert.ToUInt16(firmware[MAJOR_START]);
            FirmwareMinor = Convert.ToUInt16(firmware[MINOR_START]);
            FirmwareRevision = Convert.ToUInt16(firmware[REVISION_START]);
            SubsystemCode = firmware[SUBSYSTEM_START];
        }

        /// <summary>
        /// Convert the values back to a byte array.
        /// </summary>
        /// <returns>Byte array of the firmware.</returns>
        public byte[] Encode()
        {
            byte[] result = new byte[NUM_BYTES];
            result[MAJOR_START] = (byte)FirmwareMajor;
            result[MINOR_START] = (byte)FirmwareMinor;
            result[REVISION_START] = (byte)FirmwareRevision;
            result[SUBSYSTEM_START] = (byte)SubsystemCode;

            return result;
        }

        /// <summary>
        /// Return a subsystem for this firmware version.
        /// This will use the SubsystemCode to create a
        /// subsystem to return.
        /// 
        /// FOR BACKWARDS COMPATITBILITY
        /// Old subsystems in the ensemble were set by the Subsystem Index in Firmware.
        /// This means the that a subsystem code of 0 could be passed because
        /// the index was 0 to designate the first subsystem index.  Firmware revision 0.2.13 changed
        /// SubsystemIndex to SubsystemCode.  This will check which Firmware version this ensemble is
        /// and convert to the new type using SubsystemCode.
        /// 
        /// If the firmwawre is a debug firmware, the Major number will be set to 99.  So also 99.2.13 or less uses the old form.
        /// </summary>
        /// <param name="serial">Serial number to get the subsystem code if the firmware version is less than 0.2.13.</param>
        /// <returns>Subsystem for this firmware.</returns>
        public Subsystem GetSubsystem(SerialNumber serial)
        {
            // The DVL serial number is a special serial number
            // used to designate the ensemble as a DVL ensemble
            // Give the DVL subsystem code as the DVL
            if (serial == SerialNumber.DVL)
            {
                // Get the Subsystem code from the serialnumber, there should only be 1 subsystem
                string code = serial.SubSystems.Substring(0, 1);

                SubsystemCode = Subsystem.ConvertSubsystemCode(code[0]);

                // Create a subsystem with the code
                return new Subsystem(SubsystemCode);
            }

            if ((FirmwareMajor <= 0 || FirmwareMajor == DEBUG_MAJOR_VER) && FirmwareMinor <= 2 && FirmwareRevision <= 13)
            {
                // Set the correct subsystem based off the serial number
                // Get the index for the subsystem
                byte index = SubsystemCode;

                // Ensure the index is not out of range of the subsystem string
                if (serial.SubSystems.Length > index)
                {
                    // Get the Subsystem code from the serialnumber based off the index found
                    string code = serial.SubSystems.Substring(index, 1);

                    // Create a subsystem with the code and index
                    return new Subsystem(Subsystem.ConvertSubsystemCode(code[0]), index);
                }
            }

            return new Subsystem(SubsystemCode);
        }

        /// <summary>
        /// Get the Subsystem code.  The subsystem code is gotten in 2
        /// different ways depending on the firmware version.
        /// 
        /// Firmare Version less than or equal to 0.2.13
        /// The code stored is the index within the serial number.
        /// Get the serial numbers subsystems and use the index
        /// to get the code.
        /// 
        /// If the firmwawre is a debug firmware, the Major number will be set to 99.  So also 99.2.13 or less uses the old form.
        /// 
        /// Firmware Version greater than 0.2.13
        /// The code stored is the code.
        /// </summary>
        /// <param name="serial">Serial number used if the firmware version is less than 0.2.13</param>
        /// <returns>Subsystem code.</returns>
        public byte GetSubsystemCode(SerialNumber serial)
        {
            // The DVL serial number is a special case
            // No Subsystem or Subsystem configuration is given
            // for a DVL message.  This will be a generic response
            // to a DVL message
            if (serial == SerialNumber.DVL)
            {
                // Get the Subsystem code from the serialnumber, there should only be 1 subsystem
                string code = serial.SubSystems.Substring(0, 1);

                SubsystemCode = Subsystem.ConvertSubsystemCode(code[0]);

                return SubsystemCode;
            }

            // If the firmware version is less than 0.2.13, 
            // then the code store is actually the index and we must use
            // the serial number to get the code
            // A DVL serial number is a special case where nothing should change
            if ((FirmwareMajor <= 0 || FirmwareMajor == DEBUG_MAJOR_VER) && FirmwareMinor <= 2 && FirmwareRevision <= 13 && !serial.IsEmpty())
            {
                // Set the correct subsystem based off the serial number
                // Get the index for the subsystem
                byte index = SubsystemCode;

                // Ensure the index is not out of range of the subsystem string
                if (serial.SubSystems.Length > index)
                {
                    // Get the Subsystem code from the serialnumber based off the index found
                    string code = serial.SubSystems.Substring(index, 1);

                    return Subsystem.ConvertSubsystemCode(code[0]);
                }
            }

            // Based off the firmware version, the code stored is the correct code
            return SubsystemCode;
        }

        /// <summary>
        /// Print out the firmware version and Subsystem.
        /// Format: Major.Minor.Revision - SubsystemCode.
        /// </summary>
        /// <returns>String of the version number.</returns>
        public override string ToString()
        {
            return string.Format("{0}.{1}.{2} - {3}", FirmwareMajor, FirmwareMinor, FirmwareRevision, Convert.ToChar(SubsystemCode));
        }



        /// <summary>
        /// Create a list of all the possible major, minor and revision values.
        /// </summary>
        /// <returns>List of all possible values.</returns>
        public static List<ushort> FirmwareVersionList()
        {
            List<ushort> list = new List<ushort>();

            // Populate the list
            for (ushort x = 0; x < 255; x++) 
            { 
                list.Add(x); 
            }

            return list;
        }

        #region Overrides

        /// <summary>
        /// Hashcode for the object.
        /// This will return the hashcode for the
        /// this object's string.
        /// </summary>
        /// <returns>Hashcode for the object.</returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Determine if the given object is equal to this
        /// object.  This will check if the firmware version match.
        /// </summary>
        /// <param name="obj">Object to compare with this object.</param>
        /// <returns>TRUE = Firmware Versions matched.</returns>
        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            Firmware p = (Firmware)obj;

            return (FirmwareMajor == p.FirmwareMajor) &&
                    (FirmwareMinor == p.FirmwareMinor) &&
                    (FirmwareRevision == p.FirmwareRevision) &&
                    (SubsystemCode == p.SubsystemCode);
        }

        /// <summary>
        /// Determine if the two Firmware given are the equal.
        /// </summary>
        /// <param name="fw1">First Firmware to check.</param>
        /// <param name="fw2">Firmware to check against.</param>
        /// <returns>True if there strings match.</returns>
        public static bool operator ==(Firmware fw1, Firmware fw2)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(fw1, fw2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)fw1 == null) || ((object)fw2 == null))
            {
                return false;
            }

            // Return true if the fields match:
            return (fw1.FirmwareMajor == fw2.FirmwareMajor) &&
                    (fw1.FirmwareMinor == fw2.FirmwareMinor) &&
                    (fw1.FirmwareRevision == fw2.FirmwareRevision) &&
                    (fw1.SubsystemCode == fw2.SubsystemCode);
        }

        /// <summary>
        /// Return the opposite of ==.
        /// </summary>
        /// <param name="fw1">First Firmware to check.</param>
        /// <param name="fw2">Firmware to check against.</param>
        /// <returns>Return the opposite of ==.</returns>
        public static bool operator !=(Firmware fw1, Firmware fw2)
        {
            return !(fw1 == fw2);
        }

        #endregion
    }
}

