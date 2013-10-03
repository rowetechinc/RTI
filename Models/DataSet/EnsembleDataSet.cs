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
 * 12/27/2012      RC          2.17       In Decode(), check for Firmware revision 0.2.13.  This will fix the SubsystemIndex to SubsystemCode change.
 * 12/28/2012      RC          2.17       Made SubsystemConfiguration take a Subsystem in its constructor.
 * 01/04/2013      RC          2.17       Created a constructor that take no data.
 *                                         Fixed BACKWARDS COMPATITBILITY in Decode where it did not check the Major number and it did not convert the subsystem to a byte correctly.
 * 01/09/2013      RC          2.17       Moved the backwards compatibility to the Firmware object.
 * 01/14/2013      RC          2.17       In UpdateAverageEnsemble(), set the ping count based off the number of samples and the number of pings per ensemble.
 * 02/06/2013      RC          2.18       In Decode(), set SubsystemConfig with the subsystem if the revision is old and no SubsystemConfig is given.  Before it would not give the subsystem.\
 * 02/20/2013      RC          2.18       Fixed setting the time for PRTI sentences with SetPRTITime().
 * 02/21/2013      RC          2.18       In constructors for PRTI messages, set the serial number to SubsystemConfiguration().  This will use the DVL serial number to create the SubsystemConfiguration.
 *                                         Make the time the current time for PRTI messages.
 *                                         Made the time for PRTI be the current time.  Set the start time to AncillaryDataSet.LastPingTime.
 * 02/25/2013      RC          2.18       Added JSON encoding and Decoding.
 * 07/26/2013      RC          2.19.3     Added SubsystemConfigurationIndex to SubsystemConfiguration.  Output for JSON.
 * 10/02/2013      RC          2.20.2     Fixed bug where i was not consistent in Encode() in setting the number of elements.
 * 
 */

using System;
using System.Data;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;

namespace RTI
{
    namespace DataSet
    {
        /// <summary>
        /// Data set containing all the Ensemble data.
        /// </summary>
        [JsonConverter(typeof(EnsembleDataSetSerializer))]
        public class EnsembleDataSet : BaseDataSet
        {
            #region Variables

            /// <summary>
            /// Default ensemble number if one is not given.
            /// </summary>
            public const int DEFAULT_ENS_NUM = 0;

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
            [JsonIgnore]
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
            [JsonIgnore]
            public DateTime EnsDateTime { get; set; }

            /// <summary>
            /// Return the Date as a string.
            /// </summary>
            [JsonIgnore]
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
            [JsonIgnore]
            public string EnsTimeString
            {
                get
                {
                    return EnsDateTime.ToLongTimeString();
                }
            }

            #endregion

            /// <summary>
            /// Create a Ensemble data set.  This will create a blank dataset.  The user must fill in the data for all
            /// the important values.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public EnsembleDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Set the ensemble number to the default ensemble number
                EnsembleNumber = DEFAULT_ENS_NUM;

                // Set time to the current time
                SetTime();

                // Create UniqueId
                UniqueId = new UniqueID(EnsembleNumber, EnsDateTime);

                // Set the number of beams
                NumBeams = numBeams;

                // Set the number of bins
                NumBins = numBins;

                // Use a blank serial number
                SysSerialNumber = new SerialNumber();

                // Create blank firmware
                SysFirmware = new Firmware();

                // Create Blank Subsystem configuration
                SubsystemConfig = new SubsystemConfiguration();

                // Create a blank status
                Status = new Status(0);
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
            /// <param name="ensembleData">Byte array containing Ensemble data</param>
            public EnsembleDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, byte[] ensembleData) :
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

                // Set time to now
                SetTime();

                // Create UniqueId
                UniqueId = new UniqueID(EnsembleNumber, EnsDateTime);

                // Use default value for beams
                NumBeams = DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM;

                // Use the special serial number for a DVL
                SysSerialNumber = SerialNumber.DVL;

                // Create blank firmware
                SysFirmware = new Firmware();

                // Create Subsystem Configuration based off Firmware and Serialnumber
                SubsystemConfig = new SubsystemConfiguration(SysFirmware.GetSubsystem(SysSerialNumber), 0, 0);

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

                // Set time to now
                SetTime();

                // Create UniqueId
                UniqueId = new UniqueID(EnsembleNumber, EnsDateTime);

                // Use default value for beams
                NumBeams = DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM;

                // Use the special serial number for a DVL
                SysSerialNumber = SerialNumber.DVL;

                // Create blank firmware
                SysFirmware = new Firmware();

                // Create Subsystem Configuration based off Firmware and Serialnumber
                SubsystemConfig = new SubsystemConfiguration(SysFirmware.GetSubsystem(SysSerialNumber), 0, 0);

                // Get the status from the sentence
                Status = sentence.SystemStatus;

                // No bin data
                NumBins = 0;
            }

            /// <summary>
            /// Create an Ensemble data set.  Intended for JSON  deserialize.  This method
            /// is called when Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.EnsembleDataSet}(json) is
            /// called.
            /// 
            /// DeserializeObject is slightly faster then passing the string to the constructor.
            /// 162ms for this method.
            /// 181ms for JSON string constructor.
            /// 
            /// Alternative to decoding manually is to use the command:
            /// DataSet.EnsembleDataSet decoded = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.EnsembleDataSet}(json); 
            /// 
            /// To use this method for JSON you must have all the parameters match all the properties in this object.
            /// 
            /// </summary>
            /// <param name="ValueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="NumElements">Number of Bin</param>
            /// <param name="ElementsMultiplier">Number of beams</param>
            /// <param name="Imag"></param>
            /// <param name="NameLength">Length of name</param>
            /// <param name="Name">Name of data type</param>
            /// <param name="EnsembleNumber">Ensemble number.</param>
            /// <param name="NumBins">Number of bins.</param>
            /// <param name="NumBeams">Number of beams.</param>
            /// <param name="DesiredPingCount">Pings Desired.</param>
            /// <param name="ActualPingCount">Ping Count.</param>
            /// <param name="SysSerialNumber">Serial Number of the ADCP.</param>
            /// <param name="SysFirmware">Firmware version and configuration.</param>
            /// <param name="SubsystemConfig">Configuration for the ensemble.</param>
            /// <param name="Status">Status of the ADCP.</param>
            /// <param name="Year">Year of the ensemble.</param>
            /// <param name="Month">Month of the ensemble.</param>
            /// <param name="Day">Day of the ensemble.</param>
            /// <param name="Hour">Hour of the ensemble.</param>
            /// <param name="Minute">Minute of the ensemble.</param>
            /// <param name="Second">Second of the ensemble.</param>
            /// <param name="HSec">Hundredth of second of the ensemble.</param>
            [JsonConstructor]
            public EnsembleDataSet(int ValueType, int NumElements, int ElementsMultiplier, int Imag, int NameLength, string Name,
                                    int EnsembleNumber, int NumBins, int NumBeams, int DesiredPingCount, int ActualPingCount,
                                    SerialNumber SysSerialNumber, Firmware SysFirmware, SubsystemConfiguration SubsystemConfig, Status Status,
                                    int Year, int Month, int Day, int Hour, int Minute, int Second, int HSec) :
                base(ValueType, NumElements, ElementsMultiplier, Imag, NameLength, Name)
            {
                //this.UniqueId = UniqueId;
                this.EnsembleNumber = EnsembleNumber;
                this.NumBins = NumBins;
                this.NumBeams = NumBeams;
                this.DesiredPingCount = DesiredPingCount;
                this.ActualPingCount = ActualPingCount;
                this.SysSerialNumber = SysSerialNumber;
                this.SysFirmware = SysFirmware;
                this.SubsystemConfig = SubsystemConfig;
                this.Status = Status;
                this.Year = Year;
                this.Month = Month;
                this.Day = Day;
                this.Hour = Hour;
                this.Minute = Minute;
                this.Second = Second;
                this.HSec = HSec;
                //this.EnsDateTime = EnsDateTime;

                // Set the time and date
                ValidateDateTime(Year, Month, Day, Hour, Minute, Second, HSec / 10);

                // Create UniqueId
                UniqueId = new UniqueID(EnsembleNumber, EnsDateTime);
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
                // To get the ping count, take the number of samples times the number pf pings per sample
                int pingCount = ActualPingCount* numSamples;
                DesiredPingCount = pingCount;
                ActualPingCount = pingCount;
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


                    // FOR BACKWARDS COMPATITBILITY
                    // Old subsystems in the ensemble were set by the Subsystem Index in Firmware.
                    // This means the that a subsystem code of 0 could be passed because
                    // the index was 0 to designate the first subsystem index.  Firmware revision 0.2.13 changed
                    // SubsystemIndex to SubsystemCode.  This will check which Firmware version this ensemble is
                    // and convert to the new type using SubsystemCode.
                    //
                    // Get the correct subsystem by getting the index found in the firmware and getting
                    // subsystem code from the serial number.
                    //if (SysFirmware.FirmwareMajor <= 0 && SysFirmware.FirmwareMinor <= 2 && SysFirmware.FirmwareRevision <= 13 && GetSubSystem().IsEmpty())
                    //{
                    //    // Set the correct subsystem based off the serial number
                    //    // Get the index for the subsystem
                    //    byte index = SysFirmware.SubsystemCode;

                    //    // Ensure the index is not out of range of the subsystem string
                    //    if (SysSerialNumber.SubSystems.Length > index)
                    //    {
                    //        // Get the Subsystem code from the serialnumber based off the index found
                    //        string code = SysSerialNumber.SubSystems.Substring(index, 1);

                    //        // Create a subsystem with the code and index
                    //        Subsystem ss = new Subsystem(Convert.ToByte(code), index);

                    //        // Set the new subsystem code to the firmware
                    //        //SysFirmware.SubsystemCode = Convert.ToByte(code);

                    //        // Remove the old subsystem and add the new one to the dictionary
                    //        SysSerialNumber.SubSystemsDict.Remove(Convert.ToByte(index));
                    //        SysSerialNumber.SubSystemsDict.Add(Convert.ToByte(index), ss);

                    //    }
                    //}

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
                    SubsystemConfig = new SubsystemConfiguration(SysFirmware.GetSubsystem(SysSerialNumber), subConfig);
                }
                else
                {
                    // Create a default SubsystemConfig with a configuration of 0
                    SubsystemConfig = new SubsystemConfiguration(SysFirmware.GetSubsystem(SysSerialNumber), 0, 0);
                }

                // Set the time and date
                ValidateDateTime(Year, Month, Day, Hour, Minute, Second, HSec / 10);

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
                // Encode using the maximum number of elements available even if the original data
                // did not contain the latest revision of values (NumElements vs NUM_DATA_ELEMENTS)
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
                byte[] header = this.GenerateHeader(NUM_DATA_ELEMENTS);

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
            /// Set the time to now.  This will set the
            /// DateTime and all the individual
            /// values.
            /// </summary>
            private void SetTime()
            {
                // Set time to now
                EnsDateTime = DateTime.Now;
                Year = EnsDateTime.Year;
                Month = EnsDateTime.Month;
                Day = EnsDateTime.Day;
                Hour = EnsDateTime.Hour;
                Minute = EnsDateTime.Minute;
                Second = EnsDateTime.Second;
                HSec = EnsDateTime.Millisecond * 10;
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
                return SysSerialNumber.GetSubsystem(SysFirmware.GetSubsystemCode(SysSerialNumber));
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
                s += "SubSystem: " + SysFirmware.GetSubsystemCode(SysSerialNumber).ToString() + "\n";
                s += "Subsystem Config: " + SubsystemConfig.ToString() + "\n";
                s += EnsDateTime.ToString() + "\n";
                s += "Bins: " + NumBins + " Beams: " + NumBeams + "\n";
                s += "Pings Desired: " + DesiredPingCount + " actual: " + ActualPingCount + "\n";
                s += "Status: " + Status.ToString() + "\n";

                return s;
            }
        }

        /// <summary>
        /// Convert this object to a JSON object.
        /// Calling this method is twice as fast as calling the default serializer:
        /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.EnsembleData).
        /// 
        /// 19ms for this method.
        /// 100ms for calling SerializeObject default.
        /// 
        /// Use this method whenever possible to convert to JSON.
        /// 
        /// http://james.newtonking.com/projects/json/help/
        /// http://james.newtonking.com/projects/json/help/index.html?topic=html/ReadingWritingJSON.htm
        /// http://blog.maskalik.com/asp-net/json-net-implement-custom-serialization
        /// </summary>
        public class EnsembleDataSetSerializer : JsonConverter
        {
            /// <summary>
            /// Write the JSON string.  This will convert all the properties to a JSON string.
            /// This is done manaully to improve conversion time.  The default serializer will check
            /// each property if it can convert.  This will convert the properties automatically.  This
            /// will double the speed.
            /// 
            /// Newtonsoft.Json.JsonConvert.SerializeObject(ensemble.EnsembleData).
            /// 
            /// </summary>
            /// <param name="writer">JSON Writer.</param>
            /// <param name="value">Object to write to JSON.</param>
            /// <param name="serializer">Serializer to convert the object.</param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // Cast the object
                var data = value as EnsembleDataSet;

                // Start the object
                writer.Formatting = Formatting.None;            // Make the text not indented, so not as human readable.  This will save disk space
                writer.WriteStartObject();                      // Start the JSON object

                // Write the base values
                writer.WriteRaw(data.ToJsonBaseStub());
                writer.WriteRaw(",");

                // EnsembleNumber
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_ENSEMBLENUMBER);
                writer.WriteValue(data.EnsembleNumber);

                // NumBins
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_NUMBINS);
                writer.WriteValue(data.NumBins);

                // NumBeams
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_NUMBEAMS);
                writer.WriteValue(data.NumBeams);

                // DesiredPingCount
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DESIREDPINGCOUNT);
                writer.WriteValue(data.DesiredPingCount);

                // ActualPingCount
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_ACTUALPINGCOUNT);
                writer.WriteValue(data.ActualPingCount);

                // SysSerialNumber
                writer.WritePropertyName(DataSet.BaseDataSet.STR_JSON_SERIALNUMBER_SERIALNUMBERSTRING);
                writer.WriteValue(data.SysSerialNumber.SerialNumberString);

                // Firmware Major 
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_FIRMWAREMAJOR);
                writer.WriteValue(data.SysFirmware.FirmwareMajor);

                // Firmware Minor
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_FIRMWAREMINOR);
                writer.WriteValue(data.SysFirmware.FirmwareMinor);

                // Firmware Revision
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_FIRMWAREREVISION);
                writer.WriteValue(data.SysFirmware.FirmwareRevision);

                // Firmware SubsystemCode
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SUBSYSTEMCODE);
                writer.WriteValue(data.SysFirmware.SubsystemCode);

                // Subsystem Index
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SUBSYSTEM_INDEX);
                writer.WriteValue(data.SubsystemConfig.SubSystem.Index);

                // Subsystem Code
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SUBSYSTEM_CODE);
                writer.WriteValue(data.SubsystemConfig.SubSystem.Code);

                // CEPO Index
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_CEPOINDEX);
                writer.WriteValue(data.SubsystemConfig.CepoIndex);

                // SubsystemConfiguration Index
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SSCONFIG_INDEX);
                writer.WriteValue(data.SubsystemConfig.SubsystemConfigIndex);

                // Status Value
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_STATUS);
                writer.WriteValue(data.Status.Value);

                // Year
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_YEAR);
                writer.WriteValue(data.Year);

                // Month
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_MONTH);
                writer.WriteValue(data.Month);

                // Day
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_DAY);
                writer.WriteValue(data.Day);

                // Hour
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_HOUR);
                writer.WriteValue(data.Hour);

                // Minute
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_MINUTE);
                writer.WriteValue(data.Minute);

                // Second
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_SECOND);
                writer.WriteValue(data.Second);

                // HSec
                writer.WritePropertyName(DataSet.BaseDataSet.JSON_STR_HSEC);
                writer.WriteValue(data.HSec);

                // End the object
                writer.WriteEndObject();
            }

            /// <summary>
            /// Read the JSON object and convert to the object.  This will allow the serializer to
            /// automatically convert the object.  No special instructions need to be done and all
            /// the properties found in the JSON string need to be used.
            /// 
            /// DataSet.EnsembleDataSet decodedEns = Newtonsoft.Json.JsonConvert.DeserializeObject{DataSet.EnsembleDataSet}(encodedEns)
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

                    // Decode the data
                    int NumElements = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_NUMELEMENTS];
                    int ElementsMultiplier = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_ELEMENTSMULTIPLIER];

                    // EnsembleNumber
                    int EnsembleNumber = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_ENSEMBLENUMBER];

                    // NumBins
                    int NumBins = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_NUMBINS];

                    // NumBeams
                    int NumBeams = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_NUMBEAMS];

                    // DesiredPingCount
                    int DesiredPingCount = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_DESIREDPINGCOUNT];

                    // ActualPingCount
                    int ActualPingCount = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_ACTUALPINGCOUNT];

                    // SysSerialNumber
                    SerialNumber SysSerialNumber = new SerialNumber((string)jsonObject[DataSet.BaseDataSet.STR_JSON_SERIALNUMBER_SERIALNUMBERSTRING]);

                    // SysFirmware
                    ushort major = (ushort)jsonObject[DataSet.BaseDataSet.JSON_STR_FIRMWAREMAJOR];
                    ushort minor = (ushort)jsonObject[DataSet.BaseDataSet.JSON_STR_FIRMWAREMINOR];
                    ushort rev = (ushort)jsonObject[DataSet.BaseDataSet.JSON_STR_FIRMWAREREVISION];
                    byte code = (byte)jsonObject[DataSet.BaseDataSet.JSON_STR_SUBSYSTEMCODE].ToObject<byte>();
                    Firmware SysFirmware = new Firmware(code, major, minor, rev);

                    // SubsystemConfig
                    ushort index = (ushort)jsonObject[DataSet.BaseDataSet.JSON_STR_SUBSYSTEM_INDEX];
                    byte ssCode = (byte)jsonObject[DataSet.BaseDataSet.JSON_STR_SUBSYSTEM_CODE].ToObject<byte>();
                    byte cepoIndex = (byte)jsonObject[DataSet.BaseDataSet.JSON_STR_CEPOINDEX].ToObject<byte>();
                    byte configNum = (byte)jsonObject[DataSet.BaseDataSet.JSON_STR_SSCONFIG_INDEX].ToObject<byte>();
                    SubsystemConfiguration SubsystemConfig = new SubsystemConfiguration(new Subsystem(ssCode, index), cepoIndex, configNum);

                    // Status
                    Status status = new Status((int)jsonObject[DataSet.BaseDataSet.JSON_STR_STATUS]);

                    // Year
                    int Year = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_YEAR];

                    // Month
                    int Month = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_MONTH];

                    // Day
                    int Day = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_DAY];

                    // Hour
                    int Hour = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_HOUR];

                    // Minute
                    int Minute = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_MINUTE];

                    // Second
                    int Second = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_SECOND];

                    // HSec
                    int HSec = (int)jsonObject[DataSet.BaseDataSet.JSON_STR_HSEC];

                    // Create the object
                    // Need to call the constructor to create
                    // the correct EnsDateTime and UniqueID
                    var data = new EnsembleDataSet(DataSet.Ensemble.DATATYPE_INT, NumElements, ElementsMultiplier, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.EnsembleDataID,
                                                    EnsembleNumber, NumBins, NumBeams, DesiredPingCount, ActualPingCount,
                                                    SysSerialNumber, SysFirmware, SubsystemConfig, status,
                                                    Year, Month, Day, Hour, Minute, Second, HSec);

                    return data;
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
                return typeof(EnsembleDataSet).IsAssignableFrom(objectType);
            }
        }
    }
}