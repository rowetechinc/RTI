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
 * 01/19/2012      RC          1.14       Initial coding
 * 01/27/2012      RC          1.14       Added Validate method.
 * 01/30/2012      RC          1.14       Changed the list to a dictioanry to keep track of index of subsystem.
 *                                         Added a method to get the subsystem from the dictionary.
 * 01/31/2012      RC          1.14       Changed Subystem size and spares size.
 * 02/07/2012      RC          2.00       Added method SubsystemsString().
 * 02/14/2012      RC          2.03       Check if serial numbers are equal and hashcode.
 * 02/29/2012      RC          2.04       Set any empty serial number string to all 0's (32 digits).
 *                                         Made a special serial number for a DVL message.
 * 05/01/2012      RC          2.11       Made serial number string public.
 * 07/11/2012      RC          2.12       Fix bug if the serial number string given is bad.
 * 09/21/2012      RC          2.15       Added GetSubsystem(byte) to get the Subsystem in the list based off the Subsystem code.
 * 10/01/2012      RC          2.15       Needed to remove all private Set so that the object can be Serialized and Deserialized to JSON.
 * 10/09/2012      RC          2.15       Changed the SubSystemsDict to have the key as the Code and not the index.
 * 11/16/2012      RC          2.16       Allow the public strings to be changed and update all the values.
 *                                         Added AddSubsystem() to add a subsystem properly to the serial number.
 *                                         Added RemoveSubsystem() to remove a subsystem properly from the serial number.
 * 11/19/2012      RC          2.16       Added BASE_ELEC_TYPE_ADCP1.
 * 12/03/2012      RC          2.17       Replaced SerialNumber.Empty with IsEmpty().
 * 12/27/2012      RC          2.17       Replaced Subsystem.Empty with Subsystem.IsEmpty().
 * 01/23/2013      RC          2.17       Changed IsEmpty() to only check if a Subsystem is set for the serial number.
 * 05/06/2013      RC          2.19       Added SerialNumberDescString and GetSerialNumberDescString().
 *
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace RTI
{
    /// <summary>
    /// Decode the serial number into a 
    /// hardware type, sub-systems type and
    /// serial number.
    /// </summary>
    public class SerialNumber
    {
        #region Variables

        /// <summary>
        /// String for an empty serial number.
        /// </summary>
        public const string EMPTY_SERIAL_NUM_STRING = "00000000000000000000000000000000";

        /// <summary>
        /// Serial number for an empty serial number.
        /// </summary>
        public const int EMPTY_SERIAL_NUM = 0;

        /// <summary>
        /// Maximum size for the serial number.
        /// </summary>
        [JsonIgnore]
        public int MAX_SERIAL_NUMBER = 999999;

        #region Sizes

        /// <summary>
        /// Number of bytes in a system serial number.
        /// </summary>
        public const int NUM_BYTES = 32;

        /// <summary>
        /// Number of bytes to describe the
        /// Base Electronic Hardware architecture.
        /// </summary>
        public const int BASE_HDWR_NUM_BYTES = 2;

        /// <summary>
        /// Number of bytes to describe the 
        /// possible sub-system.
        /// </summary>
        public const int SUBSYSTEM_NUM_BYTES = 15;

        /// <summary>
        /// Number of bytes in the spare.
        /// </summary>
        public const int SPARE_NUM_BYTES = 9;

        /// <summary>
        /// Number of bytes in the serial number.
        /// </summary>
        public const int SERIAL_NUM_BYTES = 6;

        #endregion

        #region Start Locations

        /// <summary>
        /// Start in the byte array for the serial number.
        /// </summary>
        public const int SERIAL_START = 26;

        /// <summary>
        /// Start in the byte array for the Spare.
        /// </summary>
        public const int SPARE_START = 17;

        /// <summary>
        /// Start in the byte array for the sub-system.
        /// </summary>
        public const int SUBSYSTEM_START = 2;

        /// <summary>
        /// Start in the byte array for the Base Electronic
        /// hardware architecture.
        /// </summary>
        public const int BASE_HDWR_START = 0;

        #endregion

        #region Base Electronic Types

        /// <summary>
        /// Base Electronic type for ADCP1.
        /// </summary>
        public const string BASE_ELEC_TYPE_ADCP1 = "01";

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// The serial number for the system.
        /// </summary>
        private UInt32 _systemSerialNumber;
        /// <summary>
        /// The serial number for the system.
        /// </summary>
        public UInt32 SystemSerialNumber 
        {
            get { return _systemSerialNumber; } 
            set
            {
                // Verify the given value is valid
                if(value >= 0 && value <= MAX_SERIAL_NUMBER )
                {
                    _systemSerialNumber = value;
                }
            }
        }

        /// <summary>
        /// Spare in the serial number.
        /// </summary>
        private string _spare;
        /// <summary>
        /// Spare in the serial number.
        /// </summary>
        public string Spare 
        {
            get { return _spare; } 
            set
            {
                // Verify the value given is valid
                if (value.Length <= SPARE_NUM_BYTES)
                {
                    _spare = value;
                }
            }
        }

        /// <summary>
        /// The Base Electronics hardware architecture.
        /// </summary>
        private string _baseHardware;
        /// <summary>
        /// The Base Electronics hardware architecture.
        /// </summary>
        public string BaseHardware 
        {
            get { return _baseHardware; } 
            set
            {
                // Verify the value given is valid
                if (value.Length <= BASE_HDWR_NUM_BYTES)
                {
                    _baseHardware = value;
                }
            }
        }

        /// <summary>
        /// The Subsystems set for the ADCP.
        /// The ADCP can have multiple frequencies.  Each
        /// frequency would be a subsystem.
        /// </summary>
        private string _subsytems;
        /// <summary>
        /// The Subsystems set for the ADCP.
        /// The ADCP can have multiple frequencies.  Each
        /// frequency would be a subsystem.
        /// </summary>
        public string SubSystems 
        {
            get { return _subsytems; } 
            set
            {
                // Verify the value given is valid
                if (value.Length <= SUBSYSTEM_NUM_BYTES)
                {
                    // Set the subsystem string
                    _subsytems = value;

                    // Decode the Subsystem string
                    // To create the SubsystemsDict
                    DecodeSubsystem(value);
                }
            }
        }

        /// <summary>
        /// Dictionary of all subsystems.  This Dictionary is based off
        /// the subsystems given in the serial number.
        /// </summary>
        private Dictionary<byte, Subsystem> _subsystemsDict;
        /// <summary>
        /// Dictionary of all subsystems.  This Dictionary is based off
        /// the subsystems given in the serial number.
        /// </summary>
        public Dictionary<byte, Subsystem> SubSystemsDict 
        {
            get { return _subsystemsDict; } 
            set
            {
                _subsystemsDict = value;

                // Update the Subsystem string
                UpdateSubsystemString();
            }
        }

        //private string _serialNumberString;
        /// <summary>
        /// Complete serial number stored as a string.
        /// </summary>
        public string SerialNumberString
        {
            get
            {
                return GetSerialNumberString();
            }
            set
            {
                //_serialNumberString = value;
                SetSerialNumberString(value);
            }
        }

        /// <summary>
        /// A descriptive string for the Serial number.
        /// This will include a description of all the subsystems
        /// and the serial number.
        /// </summary>
        public string SerialNumberDescString
        {
            get { return GetSerialNumberDescString(); }
        }

        #endregion

        ///// <summary>
        ///// Static object to represent and empty 
        ///// serial number.
        ///// All SETs are set private to ensure this
        ///// value does not change.
        ///// </summary>
        ////public static readonly SerialNumber Empty = new SerialNumber();

        /// <summary>
        /// Create a special serial number for a DVL message.
        /// The subsystem will use the spare H.  The serial number
        /// will be 999999.
        /// </summary>
        public static readonly SerialNumber DVL = new SerialNumber("01H00000000000000000000000999999");

        /// <summary>
        /// This constructor creates a blank serial number with nothing set.
        /// </summary>
        public SerialNumber()
        {
            SystemSerialNumber = EMPTY_SERIAL_NUM;
            Spare = "";
            BaseHardware = "";
            SubSystems = "";
            SubSystemsDict = new Dictionary<byte, Subsystem>();
        }

        /// <summary>
        /// Take a byte array and turn it into
        /// the information about the system including
        /// the electronics, sub-systems and serial number.
        /// </summary>
        /// <param name="serialNum">Serial number as an ascii byte array.</param>
        public SerialNumber(byte[] serialNum)
        {
            // Decode the byte array
            Decode(serialNum);
        }

        /// <summary>
        /// Take the string and turn it into the
        /// information about the system including
        /// the electronics, sub-systems and serial number.
        /// </summary>
        /// <param name="SerialNumberString">String of the serial number.</param>
        [JsonConstructor]
        public SerialNumber(string SerialNumberString)
        {
            // Set the serial number based off a string given
            SetSerialNumberString(SerialNumberString);
        }

        /// <summary>
        /// Add a subsystem to the serial number.
        /// This will add the subsystem to the dictionary 
        /// and update the Subsystem string.
        /// </summary>
        /// <param name="ss">Subsystem to add.</param>
        public void AddSubsystem(Subsystem ss)
        {
            // Ensure we have not exceeded the maximum allowed subsystems
            if (SubSystemsDict.Count < SUBSYSTEM_NUM_BYTES)
            {
                // Add the subsystem to the dictionary
                _subsystemsDict.Add(Convert.ToByte(_subsystemsDict.Count), ss);

                // Update the Subsystem string
                UpdateSubsystemString();
            }
        }

        /// <summary>
        /// Remove the subsystem from the dictionary of subsystems.
        /// This will also update the subsystem string.
        /// </summary>
        /// <param name="ss">Subsystem to remove.</param>
        public void RemoveSubsystem(Subsystem ss)
        {
            List<Subsystem> list = new List<Subsystem>();

            // Get a list of all the subsystems
            // But do not include the subsystem given
            foreach (Subsystem subsys in SubSystemsDict.Values)
            {
                if (ss != subsys)
                {
                    list.Add(subsys);
                }
            }

            // Create a new dictionary
            _subsystemsDict = new Dictionary<byte, Subsystem>();

            // Add back all the subsystems
            foreach (Subsystem subsys in list)
            {
                // Update the new index for the subsystem 
                subsys.Index = (ushort)_subsystemsDict.Count;

                // Add the new subsystem
                AddSubsystem(subsys);
            }

            // Update the Subsystem string
            UpdateSubsystemString();
        }

        /// <summary>
        /// Determine if the serial number is correctly set.
        /// This will determine if the Subsystem.  If it is empty, then
        /// the serial number is empty.
        /// </summary>
        /// <returns>TRUE = The serial number is empty.</returns>
        public bool IsEmpty()
        {
            if (SubSystemsDict.Count == 0)
            {
                return true;
            }

            return false;
        }


        #region Decode

        /// <summary>
        /// Get the Subsystem based off the code given.
        /// If the Subsystem cannot be found, return an
        /// empty Subsystem.
        /// </summary>
        /// <param name="code">Code to look for.</param>
        /// <returns>Subsystem if found, if not found, return empty Subsystem.</returns>
        public Subsystem GetSubsystem(byte code)
        {
            foreach (Subsystem ss in SubSystemsDict.Values)
            {
                if (ss.Code == code)
                {
                    return ss;
                }
            }

            return new Subsystem();
        }

        /// <summary>
        /// Decode the serial number.
        /// </summary>
        /// <param name="serialNum"></param>
        private void Decode(byte[] serialNum)
        {
            // Decode the serial number
            Decode(System.Text.Encoding.ASCII.GetString(serialNum));
        }

        /// <summary>
        /// Decode the serial number.  The string
        /// will contain 32 digits.   The digits
        /// represent the serial number and 
        /// system hardware configuration.
        /// </summary>
        /// <param name="serialNum">Serial number of the system.</param>
        private void Decode(string serialNum)
        {
            // Set the Base Electronic Hardware architecture
            SetBaseHardware(serialNum);

            // Set the sub-systems
            SetSubSystem(serialNum);

            // Set the spare
            SetSpare(serialNum);

            // Set the serial number
            SetSerialNumber(serialNum);
        }

        /// <summary>
        /// Set the serial number back to a 
        /// byte array.
        /// </summary>
        /// <returns>Byte array of the system serial number.</returns>
        public byte[] Encode()
        {
            return System.Text.Encoding.ASCII.GetBytes(SerialNumberString);
        }

        /// <summary>
        /// Set the serial number for the ADCP.
        /// </summary>
        /// <param name="serialNum">String of the serial number.</param>
        private void SetSerialNumber(string serialNum)
        {
            try
            {
                SystemSerialNumber = Convert.ToUInt32(serialNum.Substring(SERIAL_START, SERIAL_NUM_BYTES));
            }
            catch (Exception)
            {
                SystemSerialNumber = EMPTY_SERIAL_NUM;
            }
        }

        /// <summary>
        /// Set the serial number for the ADCP.
        /// </summary>
        /// <param name="serialNum">String of the serial number.</param>
        private void SetSpare(string serialNum)
        {
            try
            {
                Spare = serialNum.Substring(SPARE_START, SPARE_NUM_BYTES);
            }
            catch (Exception)
            {
                Spare = "";
            }
        }

        /// <summary>
        /// Set the Sub-Systems.  This will create a string
        /// and then decode the string into the sub-systems.
        /// </summary>
        /// <param name="serialNum">String of the serial number.</param>
        private void SetSubSystem(string serialNum)
        {
            try
            {
                SubSystems = serialNum.Substring(SUBSYSTEM_START, SUBSYSTEM_NUM_BYTES);

                // Decode the subsystem string
                DecodeSubsystem(SubSystems);
            }
            catch (Exception)
            {
                SubSystems = "";
                SubSystemsDict = new Dictionary<byte, Subsystem>();
            }
        }

        /// <summary>
        /// Decode the subsystem string.
        /// </summary>
        /// <param name="subsystem">Subsystem string.</param>
        private void DecodeSubsystem(string subsystem)
        {
            // Decode the subsystem string into a list
            // of subsystems
            SubSystemsDict = GetSubsystemList(subsystem);
        }

        /// <summary>
        /// If the Subsystem Dictionary has changed,
        /// reset the Subsystem string.
        /// </summary>
        private void UpdateSubsystemString()
        {
            string ssStr = "";
            foreach (Subsystem ss in SubSystemsDict.Values)
            {
                ssStr += ss.CodeToString();
            }

            _subsytems = ssStr.PadRight(SUBSYSTEM_NUM_BYTES, '0');
        }

        /// <summary>
        /// Set the Base hardware.  This will create a string
        /// and then decode the string in the base hardware electronics architecture.
        /// </summary>
        /// <param name="serialNum">String of the serial number.</param>
        private void SetBaseHardware(string serialNum)
        {
            try
            {
                BaseHardware = serialNum.Substring(BASE_HDWR_START, BASE_HDWR_NUM_BYTES);

                // Decode the base hardware
                DecodeBaseHardware(BaseHardware);
            }
            catch (Exception)
            {
                BaseHardware = "";
            }
        }

        /// <summary>
        /// Decode the base hardware string into the 
        /// base hardware type.
        /// </summary>
        /// <param name="baseHardware">Base Hardware string.</param>
        private void DecodeBaseHardware(string baseHardware)
        {

        }

        /// <summary>
        /// Create a Dictionary of all the subsystems.
        /// </summary>
        /// <returns>List of all the subsystems.</returns>
        private Dictionary<byte, Subsystem> GetSubsystemList(string subsystems)
        {
            //List<Subsystem> list = new List<Subsystem>();
            Dictionary<byte, Subsystem> ssDict = new Dictionary<byte,Subsystem>();

            // Get each character from the subsystem string
            // If the character is not an empty subsystem,
            // add it to the list.
            for (int x = 0; x < subsystems.Length; x++)
            {
                string ss = subsystems.Substring(x, 1);
                if (ss != Subsystem.EMPTY)
                {
                    //list.Add(new Subsystem(ss, (UInt16)x));
                    //list.AddLast(new Subsystem(Convert.ToUInt16(x), ss));
                    ssDict.Add(Convert.ToByte(x), new Subsystem(ss, (UInt16)x));
                }
            }

            return ssDict;
        }

        #endregion

        #region Set/Get Strings

        /// <summary>
        /// Set the serial number based off the string given.
        /// </summary>
        /// <param name="serialNum">String of the serial number.</param>
        private void SetSerialNumberString(string serialNum)
        {
            if (!string.IsNullOrEmpty(serialNum))
            {
                // Set the string
                //SerialNumberString = serialNum;

                // Decode the serial number
                Decode(serialNum);
            }
            else
            {
                //SerialNumberString = EMPTY_SERIAL_NUM_STRING;
                SystemSerialNumber = EMPTY_SERIAL_NUM;
                Spare = "";
                BaseHardware = "";
                SubSystems = "";
                SubSystemsDict = new Dictionary<byte, Subsystem>();
            }
        }

        /// <summary>
        /// Generate the serial number string based off the serial number
        /// values.
        /// </summary>
        /// <returns>String of the serial number.</returns>
        private string GetSerialNumberString()
        {
            StringBuilder result = new StringBuilder();

            // Base Electronics
            if (string.IsNullOrEmpty(BaseHardware))
            {
                result.Append("00");
            }
            else
            {
                result.Append(BaseHardware.PadLeft(BASE_HDWR_NUM_BYTES, '0'));
            }

            // Subsystems
            if (string.IsNullOrEmpty(SubSystems))
            {
                result.Append("000000000000000");
            }
            else
            {
                // Set the subsystems, padding the end with 0's
                result.Append(SubSystems.PadRight(SUBSYSTEM_NUM_BYTES, '0'));
            }

            // Spare
            if (string.IsNullOrEmpty(Spare))
            {
                result.Append("000000000");
            }
            else
            {
                result.Append(Spare.PadRight(SPARE_NUM_BYTES, '0'));
            }

            // Serial number
            if (SystemSerialNumber == EMPTY_SERIAL_NUM)
            {
                result.Append("000000");
            }
            else
            {
                string serial = SystemSerialNumber.ToString();
                result.Append(serial.PadLeft(SERIAL_NUM_BYTES, '0'));
            }


            return result.ToString();
        }

        #endregion

        /// <summary>
        /// Return a string representing only the subsystems
        /// listed in the SubSystem list.  This will drop all the
        /// 0's from the list.
        /// </summary>
        /// <returns>String of the subsystems.</returns>
        public string SubsystemsString()
        {
            string result = "";
            for (int x = 0; x < SubSystems.Length; x++)
            {
                if (SubSystems.Substring(x, 1) != "0")
                {
                    result += SubSystems.Substring(x, 1);
                }
            }

            return result;
        }

        /// <summary>
        /// This will list all the subsystems full name
        /// for the serial number.  It will also list
        /// the serial number.
        /// </summary>
        /// <returns></returns>
        public string GetSerialNumberDescString()
        {
            StringBuilder sb = new StringBuilder();

            // Set the subsystems
            foreach (Subsystem ss in SubSystemsDict.Values)
            {
                sb.Append(string.Format("{0}: {1}", ss.Index, ss.DescString()));
                sb.Append("; ");
            }

            // Set the serial number
            sb.Append(string.Format("ADCP: {0}", SystemSerialNumber));


            return sb.ToString();
        }


        /// <summary>
        /// Return the _serialNumberString as the
        /// string representation of this object.
        /// </summary>
        /// <returns>String of this object.</returns>
        public override string ToString()
        {
            return SerialNumberString;
        }

        /// <summary>
        /// Validate if the serial number string is 
        /// a valid serial number.  This will test
        /// the length and if an object can be created.
        /// If any fail, it will return false.
        /// </summary>
        /// <param name="serialNum">Serial Number string to create a SerialNumber object.</param>
        /// <returns>TRUE = valid serial number string.</returns>
        public static bool Validate(string serialNum)
        {
            bool result = true;

            // Verify length
            if (serialNum.Length != SerialNumber.NUM_BYTES)
            {
                return false;
            }

            // Try to create the object
            try
            {
                // Try to convert the serial number to an int
                Convert.ToUInt32(serialNum.Substring(SERIAL_START, SERIAL_NUM_BYTES));

                // Try to create a serial number
                SerialNumber serial = new SerialNumber(serialNum);
            }
            catch (Exception)
            {
                // If any exceptions thrown, return false
                // One exception is trying to convert the serial number to an int
                return false;
            }
            
            return result;
        }

        /// <summary>
        /// Hashcode for the object.
        /// This will return the hashcode for the
        /// serial number string.
        /// </summary>
        /// <returns>Hashcode for the object.</returns>
        public override int GetHashCode()
        {
            return SerialNumberString.GetHashCode();
        }

        /// <summary>
        /// Determine if the given object is equal to this
        /// object.  This will check the serial number strings
        /// to see if they match.
        /// </summary>
        /// <param name="obj">Object to compare with this object.</param>
        /// <returns>TRUE = serial number strings matched.</returns>
        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            SerialNumber p = (SerialNumber)obj;

            return ( SerialNumberString == p.ToString());
        }

        /// <summary>
        /// Determine if the two serial numbers given are the equal.
        /// </summary>
        /// <param name="sn1">First SerialNumber to check.</param>
        /// <param name="sn2">SerialNumber to check against.</param>
        /// <returns>True if there strings match.</returns>
        public static bool operator ==(SerialNumber sn1, SerialNumber sn2)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(sn1, sn2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)sn1 == null) || ((object)sn2 == null))
            {
                return false;
            }

            // Return true if the fields match:
            return (sn1.ToString() == sn2.ToString());
        }

        /// <summary>
        /// Return the opposite of ==.
        /// </summary>
        /// <param name="sn1">First SerialNumber to check.</param>
        /// <param name="sn2">SerialNumber to check against.</param>
        /// <returns>Return the opposite of ==.</returns>
        public static bool operator !=(SerialNumber sn1, SerialNumber sn2)
        {
            return !(sn1 == sn2);
        }
    }
}