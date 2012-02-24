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
 *
 */

using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Complete serial number stored as a string.
        /// </summary>
        private string _serialNumberString;

        #endregion

        #region Properties

        /// <summary>
        /// The serial number for the system.
        /// </summary>
        public UInt32 SystemSerialNumber { get; private set; }

        /// <summary>
        /// Spare in the serial number.
        /// </summary>
        public string Spare { get; private set; }

        /// <summary>
        /// The Base Electronics hardware architecture.
        /// </summary>
        public string BaseHardware { get; private set; }

        /// <summary>
        /// The Sub-systems set for the system.
        /// </summary>
        public string SubSystems { get; private set; }

        /// <summary>
        /// Dictionary of all subsystems.  This Dictionary is based off
        /// the subsystems given in the serial number.
        /// </summary>
        public Dictionary<UInt16, Subsystem> SubSystemsDict { get; private set; }

        #endregion

        /// <summary>
        /// Static object to represent and empty 
        /// serial number.
        /// All SETs are set private to ensure this
        /// value does not change.
        /// </summary>
        public static readonly SerialNumber Empty = new SerialNumber();

        /// <summary>
        /// This constructor creates a blank serial number with nothing set.
        /// </summary>
        public SerialNumber()
        {
            _serialNumberString = "";
            SystemSerialNumber = 0;
            Spare = "";
            BaseHardware = "";
            SubSystems = "";
            SubSystemsDict = new Dictionary<UInt16, Subsystem>();
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
        /// <param name="serialNum">String of the serial number.</param>
        public SerialNumber(string serialNum)
        {
            if (!string.IsNullOrEmpty(serialNum))
            {
                // Set the string
                _serialNumberString = serialNum;

                // Decode the serial number
                Decode(serialNum);
            }
            else
            {
                _serialNumberString = "";
                SystemSerialNumber = 0;
                Spare = "";
                BaseHardware = "";
                SubSystems = "";
                SubSystemsDict = new Dictionary<UInt16, Subsystem>();
            }
        }

        #region Methods

        /// <summary>
        /// Get the subsystem from the dictionary.  If the
        /// key does not exist in the list, a static empty
        /// subsystem will be returned.
        /// </summary>
        /// <param name="index">Index of the subsystem.</param>
        /// <returns>Subsystem within the dictionary or an empty subsystem.</returns>
        public Subsystem GetSubsystem(UInt16 index)
        {
            // Try to get the value, if it fails
            // return empty.
            Subsystem ss = Subsystem.Empty;
            if (SubSystemsDict.TryGetValue(index, out ss))
            {
                return ss;
            }

            return Subsystem.Empty;
        }

        /// <summary>
        /// Decode the serial number.
        /// </summary>
        /// <param name="serialNum"></param>
        private void Decode(byte[] serialNum)
        {
            // Convert the byte array to a string
            _serialNumberString = System.Text.Encoding.ASCII.GetString(serialNum);

            // Decode the serial number
            Decode(_serialNumberString);
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
            return System.Text.Encoding.ASCII.GetBytes(_serialNumberString);
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
                SystemSerialNumber = 0;
            }
        }

        /// <summary>
        /// Set the serial number for the ADCP.
        /// </summary>
        /// <param name="serialNum">String of the serial number.</param>
        private void SetSpare(string serialNum)
        {
            Spare = serialNum.Substring(SPARE_START, SPARE_NUM_BYTES);
        }

        /// <summary>
        /// Set the Sub-Systems.  This will create a string
        /// and then decode the string into the sub-systems.
        /// </summary>
        /// <param name="serialNum">String of the serial number.</param>
        private void SetSubSystem(string serialNum)
        {
            SubSystems = serialNum.Substring(SUBSYSTEM_START, SUBSYSTEM_NUM_BYTES);

            // Decode the subsystem string into a list
            // of subsystems
            SubSystemsDict = GetSubsystemList(SubSystems);
        }

        /// <summary>
        /// Set the Base hardware.  This will create a string
        /// and then decode the string in the base hardware electronics architecture.
        /// </summary>
        /// <param name="serialNum">String of the serial number.</param>
        private void SetBaseHardware(string serialNum)
        {
            BaseHardware = serialNum.Substring(BASE_HDWR_START, BASE_HDWR_NUM_BYTES);

            // Decode the base hardware
            DecodeBaseHardware(BaseHardware);
        }

        /// <summary>
        /// Decode the base hardware string into the 
        /// base hardware type.
        /// </summary>
        /// <param name="baseHardware"></param>
        private void DecodeBaseHardware(string baseHardware)
        {

        }

        /// <summary>
        /// Create a Dictionary of all the subsystems.
        /// </summary>
        /// <returns>List of all the subsystems.</returns>
        private Dictionary<UInt16, Subsystem> GetSubsystemList(string subsystems)
        {
            //List<Subsystem> list = new List<Subsystem>();
            Dictionary<UInt16, Subsystem> ssDict = new Dictionary<UInt16,Subsystem>();

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
                    ssDict.Add((UInt16)x, new Subsystem(ss, (UInt16)x));
                }
            }

            return ssDict;
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
        /// Return the _serialNumberString as the
        /// string representation of this object.
        /// </summary>
        /// <returns>String of this object.</returns>
        public override string ToString()
        {
            return _serialNumberString;
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
    }
}