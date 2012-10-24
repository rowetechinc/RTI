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
 * 09/01/2011      RC                     Initial coding
 * 10/05/2011      RC                     Set time to now if not set.
 *                                         Added a Date and Time string.
 * 10/18/2011      RC                     Added UniqueId.
 * 10/24/2011      RC                     Changed from receiving a DataTable to a DataRow.
 * 11/03/2011      RC                     Added ==, Equals and Hashcode for UniqueID class.
 * 11/28/2011      RC                     Added decoding Prti01Sentence.  
 *                                         Changed how IsHardwareTimeout is set.
 * 11/29/2011      RC                     Added decoding Prti02Sentence.
 * 12/06/2011      RC          1.08       Added ToString() for UniqueId.  
 * 12/09/2011      RC          1.09       Do not make NumBins/NumBeams and NumElements and ElementsMultiplier match.
 * 12/19/2011      RC          1.10       Added SysSerialNumber and Firmware.  Check revision when setting values.
 * 01/17/2012      RC          1.14       Removed the "private set" from all the properties.
 * 01/24/2012      RC          1.14       Created a blank Serial number and Firmware for PRTI creation.
 * 01/26/2012      RC          1.14       Convert subsystem to a byte instead of a string.
 * 01/30/2012      RC          1.14       Changed decoding Firmware::Subsystem to an UInt16 subsystem index.
 *                                         Added a method to get the subsystem for this ensemble.
 * 02/16/2012      RC          2.03       Added UpdateAverageEnsemble() to update averaged ensembles.
 * 02/23/2012      RC          2.03       Changed Status to status object.
 * 02/29/2012      RC          2.04       Set the Status for the constructor that take PRTI01 or PRTI02 sentences.
 *                                         Created a special serial number for the DVL constructors.
 * 03/30/2012      RC          2.07       Moved Converters.cs methods to MathHelper.cs.
 * 04/10/2012      RC          2.08       Fixed bug decoding a 32bit int as an 8bit int.
 * 09/18/2012      RC          2.15       Updated data to ADCP User Guide Rev H.  Added Subsystem Configuration.
 * 10/09/2012      RC          2.15       In Decode(), changed SubsystemIndex to SubsystemCode and changed from an UInt16 to a byte.      
 * 
 */

using System;
using System.Data;
using System.Diagnostics;

namespace RTI
{
    namespace DataSet
    {
        /// <summary>
        /// Data set containing all the Ensemble data.
        /// </summary>
        public class EnsembleDataSet : BaseDataSet
        {
            #region Variables

            /// <summary>
            /// Number of elements as of User Guide Rev C.
            /// </summary>
            public const int NUM_DATA_ELEMENTS_REV_C = 13;

            /// <summary>
            /// Number of elements as of User Guide Rev D.
            /// </summary>
            public const int NUM_DATA_ELEMENTS_REV_D = 22;

            /// <summary>
            /// Number of elements as of User Guide Rev H.
            /// </summary>
            public const int NUM_DATA_ELEMENTS_REV_H = 23;

            /// <summary>
            /// Number of elements within this data set
            /// </summary>
            public const int NUM_DATA_ELEMENTS = NUM_DATA_ELEMENTS_REV_H;

            /// <summary>
            /// Number of Int32 in the serial number for 
            /// revision D.
            /// </summary>
            public const int SERIAL_NUM_INT_REV_D = 8;

            /// <summary>
            /// Number of bytes in the serial number.
            /// </summary>
            public const int SERIAL_NUM_INT = SERIAL_NUM_INT_REV_D;

            /// <summary>
            /// Number of Int32 in the firmware.
            /// </summary>
            public const int FIRMWARE_NUM_INT = 1;

            /// <summary>
            /// Number of Int32 in the Subsystem Configuration.
            /// </summary>
            public const int SUBSYSTEM_CONFIG_NUM_INT = 1;

            /// <summary>
            /// Status value for Hardware timeout.
            /// </summary>
            public const int HDWR_TIMEOUT = 0x8000;

            #endregion

            #region Properties

            /// <summary>
            /// A unique ID for the dataset.
            /// </summary>
            public UniqueID UniqueId { get; set; }

            /// <summary>
            /// Ensemble number.  Unique number for each ensemble during 
            /// a deployment.
            /// </summary>
            public int EnsembleNumber { get; set; }

            /// <summary>
            /// Number of Bin within the ensemble.
            /// </summary>
            public int NumBins { get; set; }

            /// <summary>
            /// Number of beams on the system.
            /// </summary>
            public int NumBeams { get; set; }

            /// <summary>
            /// Desired number of pings within a single ensemble.
            /// </summary>
            public int DesiredPingCount { get; set; }

            /// <summary>
            /// Actual number of pings within a single ensemble.
            /// </summary>
            public int ActualPingCount { get; set; }

            /// <summary>
            /// System Serial number.
            /// </summary>
            public SerialNumber SysSerialNumber { get; set; }

            /// <summary>
            /// System Firmware number.
            /// </summary>
            public Firmware SysFirmware { get; set; }

            /// <summary>
            /// Subsystem configuration.
            /// </summary>
            public SubsystemConfiguration SubsystemConfig { get; set; }

            /// <summary>
            /// Status of the system.
            /// </summary>
            public Status Status { get; set; }

            /// <summary>
            /// Year for ensemble.
            /// </summary>
            public int Year { get; set; }

            /// <summary>
            /// Month for ensemble.
            /// </summary>
            public int Month { get; set; }

            /// <summary>
            /// Day for ensemble.
            /// </summary>
            public int Day { get; set; }

            /// <summary>
            /// Hour for ensemble.
            /// </summary>
            public int Hour { get; set; }

            /// <summary>
            /// Minute for ensemble.
            /// </summary>
            public int Minute { get; set; }

            /// <summary>
            /// Seconds for ensemble.
            /// </summary>
            public int Second { get; set; }

            /// <summary>
            /// Hundreth of a Second for ensemble.
            /// </summary>
            public int HSec { get; set; }

            /// <summary>
            /// Date and time of the ensemble.
            /// If the Year, Month or Day are not set, then this will
            /// return the current date and time.
            /// </summary>
            public DateTime EnsDateTime { get; set; }

            /// <summary>
            /// Return the Date as a string.
            /// </summary>
            public string EnsDateString
            {
                get
                {
                    return EnsDateTime.ToShortDateString();
                }
            }

            /// <summary>
            /// Return the Time as a string.
            /// </summary>
            public string EnsTimeString
            {
                get
                {
                    return EnsDateTime.ToLongTimeString();
                }
            }

            #endregion

            /// <summary>
            /// Create a Ensemble data set.  Includes all the information
            /// about the current Ensemble.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="ensembleData">Byte array containing Ensemble data</param>
            public EnsembleDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] ensembleData) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize ranges

                // Decode the information
                Decode(ensembleData);
            }

            /// <summary>
            /// Create a Ensemble data set.  Includes all the information
            /// about the current Ensemble.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="ensembleData">DataTable containing Ensemble data</param>
            public EnsembleDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, DataRow ensembleData) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize ranges

                // Decode the information
                Decode(ensembleData);
            }

            /// <summary>
            /// Create an Ensemble data set.  Include all the information
            /// about the current ensemble from the sentence.  This will include
            /// the ensemble number and status.
            /// </summary>
            /// <param name="sentence">Sentence containing data.</param>
            public EnsembleDataSet(Prti01Sentence sentence) :
                base(DataSet.Ensemble.DATATYPE_INT, NUM_DATA_ELEMENTS, DataSet.Ensemble.DEFAULT_NUM_BEAMS_NONBEAM, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.EnsembleDataID)
            {
                // Set the ensemble number
                EnsembleNumber = sentence.SampleNumber;

                // Set time to number of seconds since startup
                EnsDateTime = new DateTime();
                TimeSpan span = new TimeSpan(0, 0, 0, 0, sentence.StartTime * 10);      // Multiply by 10 because value is 1/100 of a second
                EnsDateTime = EnsDateTime + span;

                // Create UniqueId
                UniqueId = new UniqueID(EnsembleNumber, EnsDateTime);

                // Use default value for beams
                NumBeams = DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM;

                // Use the special serial number for a DVL
                SysSerialNumber = SerialNumber.DVL;

                // Create blank firmware
                SysFirmware = new Firmware();

                // Create Blank Subsystem configuration
                SubsystemConfig = new SubsystemConfiguration();

                // Get the status from the sentence
                Status = sentence.SystemStatus;

                // No bin data
                NumBins = 0;
            }

            /// <summary>
            /// Create an Ensemble data set.  Include all the information
            /// about the current ensemble from the sentence.  This will include
            /// the ensemble number and status.
            /// </summary>
            /// <param name="sentence">Sentence containing data.</param>
            public EnsembleDataSet(Prti02Sentence sentence) :
                base(DataSet.Ensemble.DATATYPE_INT, NUM_DATA_ELEMENTS, DataSet.Ensemble.DEFAULT_NUM_BEAMS_NONBEAM, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.EnsembleDataID)
            {
                // Set the ensemble number
                EnsembleNumber = sentence.SampleNumber;

                // Set time to number of seconds since startup
                EnsDateTime = new DateTime();
                TimeSpan span = new TimeSpan(0, 0, 0, 0, sentence.StartTime * 10);      // Multiply by 10 because value is 1/100 of a second
                EnsDateTime = EnsDateTime + span;

                // Create UniqueId
                UniqueId = new UniqueID(EnsembleNumber, EnsDateTime);

                // Use default value for beams
                NumBeams = DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM;

                // Use the special serial number for a DVL
                SysSerialNumber = SerialNumber.DVL;

                // Create blank firmware
                SysFirmware = new Firmware();

                // Create blank Subsystem Configuration
                SubsystemConfig = new SubsystemConfiguration();

                // Get the status from the sentence
                Status = sentence.SystemStatus;

                // No bin data
                NumBins = 0;
            }

            /// <summary>
            /// When an ensemble is averaged, the Ping count
            /// and date and time need to be updated for the ensemble.
            /// This will update the necessary values for an averaged
            /// ensemble.
            /// </summary>
            /// <param name="numSamples">Number of samples in the averaged ensemble.</param>
            public void UpdateAverageEnsemble(int numSamples)
            {
                // Set the time as now
                EnsDateTime = DateTime.Now;
                Year = EnsDateTime.Year;
                Month = EnsDateTime.Month;
                Day = EnsDateTime.Day;
                Hour = EnsDateTime.Hour;
                Minute = EnsDateTime.Minute;
                Second = EnsDateTime.Second;
                HSec = EnsDateTime.Millisecond * 10;

                // Create UniqueId
                UniqueId = new UniqueID(EnsembleNumber, EnsDateTime);

                // Set the number of samples in the averaged ensemble
                DesiredPingCount = numSamples;
                ActualPingCount = numSamples;
            }

            /// <summary>
            /// Get all the information about the Ensemble.
            /// </summary>
            /// <param name="data">Byte array containing the Ensemble data type.</param>
            private void Decode(byte[] data)
            {
                EnsembleNumber = MathHelper.ByteArrayToInt32(data, GenerateIndex(0));
                NumBins = MathHelper.ByteArrayToInt32(data, GenerateIndex(1));
                NumBeams = MathHelper.ByteArrayToInt32(data, GenerateIndex(2));
                DesiredPingCount = MathHelper.ByteArrayToInt32(data, GenerateIndex(3));
                ActualPingCount = MathHelper.ByteArrayToInt32(data, GenerateIndex(4));
                Status = new Status(MathHelper.ByteArrayToInt32(data, GenerateIndex(5)));
                Year = MathHelper.ByteArrayToInt32(data, GenerateIndex(6));
                Month = MathHelper.ByteArrayToInt32(data, GenerateIndex(7));
                Day = MathHelper.ByteArrayToInt32(data, GenerateIndex(8));
                Hour = MathHelper.ByteArrayToInt32(data, GenerateIndex(9));
                Minute = MathHelper.ByteArrayToInt32(data, GenerateIndex(10));
                Second = MathHelper.ByteArrayToInt32(data, GenerateIndex(11));
                HSec = MathHelper.ByteArrayToInt32(data, GenerateIndex(12));

                // Revision D additions
                if (NumElements >= NUM_DATA_ELEMENTS_REV_D && data.Length >= NUM_DATA_ELEMENTS_REV_D * Ensemble.BYTES_IN_INT32)
                {
                    // Get the System Serial Num
                    // Start at index 13
                    byte[] serial = new byte[SERIAL_NUM_INT * Ensemble.BYTES_IN_INT32];
                    System.Buffer.BlockCopy(data, GenerateIndex(13), serial, 0, SERIAL_NUM_INT * Ensemble.BYTES_IN_INT32);
                    SysSerialNumber = new SerialNumber(serial);

                    // Get the firmware number 
                    // Start at index 21
                    byte[] firmware = new byte[FIRMWARE_NUM_INT * Ensemble.BYTES_IN_INT32];
                    System.Buffer.BlockCopy(data, GenerateIndex(21), firmware, 0, FIRMWARE_NUM_INT * Ensemble.BYTES_IN_INT32);
                    SysFirmware = new Firmware(firmware);
                }
                else
                {
                    SysSerialNumber = new SerialNumber();
                    SysFirmware = new Firmware();
                    
                }

                // Revision H additions
                if (NumElements >= NUM_DATA_ELEMENTS_REV_H && data.Length >= NUM_DATA_ELEMENTS_REV_H * Ensemble.BYTES_IN_INT32)
                {
                    // Get the Subsystem Configuration
                    // Start at index 22
                    byte[] subConfig = new byte[SUBSYSTEM_CONFIG_NUM_INT * Ensemble.BYTES_IN_INT32];
                    System.Buffer.BlockCopy(data, GenerateIndex(22), subConfig, 0, SUBSYSTEM_CONFIG_NUM_INT * Ensemble.BYTES_IN_INT32);
                    SubsystemConfig = new SubsystemConfiguration(subConfig);
                }
                else
                {
                    SubsystemConfig = new SubsystemConfiguration();
                }

                // Set the time and date
                ValidateDateTime(Year, Month, Day, Hour, Minute, Second, HSec * 10);

                // Create UniqueId
                UniqueId = new UniqueID(EnsembleNumber, EnsDateTime);
            }

            /// <summary>
            /// Generate a byte array representing the
            /// dataset.  The byte array is in the binary format.
            /// The format can be found in the RTI ADCP User Guide.
            /// It contains a header and payload.  This byte array 
            /// will be combined with the other dataset byte arrays
            /// to form an ensemble.
            /// </summary>
            /// <returns>Byte array of the ensemble.</returns>
            public byte[] Encode()
            {
                int index = 0;
                
                // Get the length
                byte[] payload = new byte[NUM_DATA_ELEMENTS * Ensemble.BYTES_IN_INT32];
                System.Buffer.BlockCopy(MathHelper.Int32ToByteArray(EnsembleNumber), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_INT32);
                System.Buffer.BlockCopy(MathHelper.Int32ToByteArray(NumBins), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_INT32);
                System.Buffer.BlockCopy(MathHelper.Int32ToByteArray(NumBeams), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_INT32);
                System.Buffer.BlockCopy(MathHelper.Int32ToByteArray(DesiredPingCount), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_INT32);
                System.Buffer.BlockCopy(MathHelper.Int32ToByteArray(ActualPingCount), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_INT32);
                System.Buffer.BlockCopy(MathHelper.Int32ToByteArray(Status.Value), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_INT32);
                System.Buffer.BlockCopy(MathHelper.Int32ToByteArray(Year), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_INT32);
                System.Buffer.BlockCopy(MathHelper.Int32ToByteArray(Month), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_INT32);
                System.Buffer.BlockCopy(MathHelper.Int32ToByteArray(Day), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_INT32);
                System.Buffer.BlockCopy(MathHelper.Int32ToByteArray(Hour), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_INT32);
                System.Buffer.BlockCopy(MathHelper.Int32ToByteArray(Minute), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_INT32);
                System.Buffer.BlockCopy(MathHelper.Int32ToByteArray(Second), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_INT32);
                System.Buffer.BlockCopy(MathHelper.Int32ToByteArray(HSec), 0, payload, GeneratePayloadIndex(index++), Ensemble.BYTES_IN_INT32);
                System.Buffer.BlockCopy(SysSerialNumber.Encode(), 0, payload, GeneratePayloadIndex(index), SerialNumber.NUM_BYTES);

                // Generate new index
                int newIndex = GeneratePayloadIndex(index) + (SERIAL_NUM_INT * Ensemble.BYTES_IN_INT32);                                                // Last index plus the size of the serial number in bytes
                System.Buffer.BlockCopy(SysFirmware.Encode(), 0, payload, newIndex, Firmware.NUM_BYTES);

                newIndex = GeneratePayloadIndex(index) + (SERIAL_NUM_INT * Ensemble.BYTES_IN_INT32) + (FIRMWARE_NUM_INT * Ensemble.BYTES_IN_INT32);     // Last index plus the size of the serial number and firmware
                System.Buffer.BlockCopy(SubsystemConfig.Encode(), 0, payload, newIndex, SubsystemConfiguration.NUM_BYTES);

                // Generate header for the dataset
                byte[] header = this.GenerateHeader(NumElements);

                // Create the array to hold the dataset
                byte[] result = new byte[payload.Length + header.Length];

                // Copy the header to the array
                System.Buffer.BlockCopy(header, 0, result, 0, header.Length);

                // Copy the Nmea data to the array
                System.Buffer.BlockCopy(payload, 0, result, header.Length, payload.Length);

                return result;
            }

            /// <summary>
            /// Move pass the Base data of the ensemble.  Then based
            /// off the index, move to the correct location.
            /// </summary>
            /// <param name="index">PlaybackIndex of the order of the data.</param>
            /// <returns>PlaybackIndex for the given xAxis.  Start of the data</returns>
            private int GenerateIndex(int index)
            {
                return GetBaseDataSize(NameLength) + (index * Ensemble.BYTES_IN_INT32);
            }

            /// <summary>
            /// Generate a index within a payload byte array.
            /// </summary>
            /// <param name="index">Element number within the payload.</param>
            /// <returns>Start location within the payload.</returns>
            private int GeneratePayloadIndex(int index)
            {
                return index * Ensemble.BYTES_IN_INT32;
            }

            /// <summary>
            /// Get all the information about the Ensemble.
            /// </summary>
            /// <param name="data">DataRow containing the Ensemble data type.</param>
            private void Decode(DataRow data)
            {
                // Go through the result settings
                EnsembleNumber = Convert.ToInt32(data[DbCommon.COL_ENS_ENS_NUM].ToString());
                NumBins = Convert.ToInt32(data[DbCommon.COL_ENS_NUM_BIN].ToString());
                NumBeams = Convert.ToInt32(data[DbCommon.COL_ENS_NUM_BEAM].ToString());
                DesiredPingCount = Convert.ToInt32(data[DbCommon.COL_ENS_DES_PING_COUNT].ToString());
                ActualPingCount = Convert.ToInt32(data[DbCommon.COL_ENS_ACT_PING_COUNT].ToString());
                Status = new Status(Convert.ToInt32(data[DbCommon.COL_ENS_STATUS]));
                Year = Convert.ToInt32(data[DbCommon.COL_ENS_YEAR].ToString());
                Month = Convert.ToInt32(data[DbCommon.COL_ENS_MONTH].ToString());
                Day = Convert.ToInt32(data[DbCommon.COL_ENS_DAY].ToString());
                Hour = Convert.ToInt32(data[DbCommon.COL_ENS_HOUR].ToString());
                Minute = Convert.ToInt32(data[DbCommon.COL_ENS_MINUTE].ToString());
                Second = Convert.ToInt32(data[DbCommon.COL_ENS_SECOND].ToString());
                HSec = Convert.ToInt32(data[DbCommon.COL_ENS_HUN_SECOND].ToString());
                
                // Check if Rev D columns exist
                if (data.Table.Columns.Contains(DbCommon.COL_ENS_SYS_SERIAL))
                {

                    SysSerialNumber = new SerialNumber(data[DbCommon.COL_ENS_SYS_SERIAL].ToString());

                    UInt16 firmMjr = 0;
                    UInt16 firmMnr = 0;
                    UInt16 firmRev = 0;
                    byte subSysCode = 0; 
                    firmMjr = (Convert.ToUInt16(data[DbCommon.COL_ENS_FIRMWARE_MAJOR]));
                    firmMnr = (Convert.ToUInt16(data[DbCommon.COL_ENS_FIRMWARE_MINOR]));
                    firmRev = (Convert.ToUInt16(data[DbCommon.COL_ENS_FIRMWARE_REVISION]));
                    subSysCode = (Convert.ToByte(data[DbCommon.COL_ENS_SUBSYSTEM_CODE]));
                    SysFirmware = new Firmware(subSysCode, firmMjr, firmMnr, firmRev);

                    // Set NumElements
                    NumElements = NUM_DATA_ELEMENTS_REV_D;
                }
                else
                {
                    SysSerialNumber = new SerialNumber();
                    SysFirmware = new Firmware();

                    // Set NumElements
                    NumElements = NUM_DATA_ELEMENTS_REV_C;
                }

                // Check if Rev H columns exist
                if (data.Table.Columns.Contains(DbCommon.COL_ENS_SUBSYS_CONFIG))
                {
                    // Verify data exist in the column
                    if (data[DbCommon.COL_ENS_SUBSYS_CONFIG] != DBNull.Value)
                    {
                        // Set Subsystem Configuration
                        byte subConfig = Convert.ToByte(data[DbCommon.COL_ENS_SUBSYS_CONFIG]);
                        SubsystemConfig = new SubsystemConfiguration(subConfig);
                    }
                    else
                    {
                        SubsystemConfig = new SubsystemConfiguration();
                    }

                    // Set NumElements
                    NumElements = NUM_DATA_ELEMENTS_REV_H;
                }
                else
                {
                    SubsystemConfig = new SubsystemConfiguration();
                }

                // Set the time and date
                ValidateDateTime(Year, Month, Day, Hour, Minute, Second, HSec * 10);

                // Create UniqueId
                UniqueId = new UniqueID(EnsembleNumber, EnsDateTime);


            }

            /// <summary>
            /// Try to set the date and time.  If any values are out of range, an exception
            /// will be thrown and the date and time DateTime.MinValue will be set.
            /// </summary>
            /// <param name="year">Year</param>
            /// <param name="month">Month</param>
            /// <param name="day">Day</param>
            /// <param name="hour">Hour</param>
            /// <param name="min">Minute</param>
            /// <param name="second">Second</param>
            /// <param name="millisec">Milliseconds</param>
            private void ValidateDateTime(int year, int month, int day, int hour, int min, int second, int millisec)
            {
                try
                {
                    EnsDateTime = new DateTime(year, month, day, hour, min, second, millisec);
                }
                // Paramater out of range
                catch (ArgumentOutOfRangeException)
                {
                    EnsDateTime = new DateTime();
                    EnsDateTime = DateTime.MinValue;
                    //Debug.WriteLine(string.Format("Y{0} M{1} D{2} : H{3} M{4} S{5} MS{6}", year, month, day, hour, min, second, millisec));
                }
                // The specified parameters evaluate to less than DateTime.MinValue or more than DateTime.MaxValue
                catch (ArgumentException)
                {
                    EnsDateTime = new DateTime();
                    EnsDateTime = DateTime.MinValue;
                }
            }

            /// <summary>
            /// This is used to quickly get the subsystem.
            /// The subsystem is obtained by a combination 
            /// of the serial number and firmware number.
            /// </summary>
            /// <returns>Subsystem for this ensemble.</returns>
            public Subsystem GetSubSystem( )
            {
                return SysSerialNumber.GetSubsystem(SysFirmware.SubsystemCode);
            }

            /// <summary>
            /// Override the ToString to return all the Ensemble data as a string.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                string s = "";
                s += "Ensemble: "  + EnsembleNumber + "\n";
                s += "Serial: " + SysSerialNumber.ToString() + "\n";
                s += "Firmware: " + SysFirmware.ToString() + "\n";
                s += "SubSystem: " + SysFirmware.SubsystemCode.ToString() + "\n";
                s += "Subsystem Config: " + SubsystemConfig.ToString() + "\n";
                s += EnsDateTime.ToString() + "\n";
                s += "Bins: " + NumBins + " Beams: " + NumBeams + "\n";
                s += "Pings Desired: " + DesiredPingCount + " actual: " + ActualPingCount + "\n";
                s += "Status: " + Status.ToString() + "\n";

                return s;
            }
        }
    }
}