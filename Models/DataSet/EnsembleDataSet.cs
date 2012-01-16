/*
 * Copyright © 2011 
 * Rowe Technology Inc.
 * All rights reserved.
 * http://www.rowetechinc.com
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification is NOT permitted.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
 * COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
 * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
 * POSSIBILITY OF SUCH DAMAGE.
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
 *       
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
            /// <summary>
            /// NUmber of elements as of User Guide Rev C.
            /// </summary>
            public const int NUM_DATA_ELEMENTS_REV_C = 13;


            /// <summary>
            /// NUmber of elements as of User Guide Rev D.
            /// </summary>
            public const int NUM_DATA_ELEMENTS_REV_D = 22;

            /// <summary>
            /// Number of elements within this data set
            /// </summary>
            public const int NUM_DATA_ELEMENTS = NUM_DATA_ELEMENTS_REV_D;

            /// <summary>
            /// Status value for Hardware timeout.
            /// </summary>
            public const int HDWR_TIMEOUT = 0x8000;

            /// <summary>
            /// A unique ID for the dataset.
            /// </summary>
            public UniqueID UniqueId { get; set; }

            /// <summary>
            /// Ensemble number.  Unique number for each ensemble during 
            /// a deployment.
            /// </summary>
            public int EnsembleNumber { get; private set; }

            /// <summary>
            /// Number of Bin within the ensemble.
            /// </summary>
            public int NumBins { get; private set; }

            /// <summary>
            /// Number of beams on the system.
            /// </summary>
            public int NumBeams { get; private set; }

            /// <summary>
            /// Desired number of pings within a single ensemble.
            /// </summary>
            public int DesiredPingCount { get; private set; }

            /// <summary>
            /// Actual number of pings within a single ensemble.
            /// </summary>
            public int ActualPingCount { get; private set; }

            /// <summary>
            /// System Serial number.
            /// </summary>
            public string SysSerialNumber { get; private set; }

            /// <summary>
            /// System Firmware number.
            /// </summary>
            public string Firmware { get; private set; }

            /// <summary>
            /// Status of the system.
            /// Final value is a logical OR of each status bit.
            /// Good Status = 0x0000
            /// Hardware Timeout = 08000
            ///   The hardware did not respond to the ping request
            /// </summary>
            private int _status;
            /// <summary>
            /// Status of the system.
            /// Final value is a logical OR of each status bit.
            /// Good Status = 0x0000
            /// Hardware Timeout = 08000
            ///   The hardware did not respond to the ping request
            /// </summary>
            public int Status 
            {
                get { return _status; } 
                private set
                {
                    _status = value;
                }
            }

            /// <summary>
            /// Create a string representing the 
            /// status of the system.  Currently
            /// there is only 1 issue that can be
            /// recorded, Hardware Timeout.
            /// </summary>
            public string StatusString
            {
                get
                {
                    if (IsHardwareTimeout)
                    {
                        return "Hardware Timeout";
                    }

                    return "Good";
                }
            }

            /// <summary>
            /// Set flag if there is a hardware timeout.
            /// The hardware did not respond to the ping request.
            /// This value is set based off the Status.
            /// </summary>
            public bool IsHardwareTimeout
            {
                get
                {
                    // Check which flags were set
                    return (_status & HDWR_TIMEOUT) > 0;
                }
            }

            /// <summary>
            /// Year for ensemble.
            /// </summary>
            public int Year { get; private set; }

            /// <summary>
            /// Month for ensemble.
            /// </summary>
            public int Month { get; private set; }

            /// <summary>
            /// Day for ensemble.
            /// </summary>
            public int Day { get; private set; }

            /// <summary>
            /// Hour for ensemble.
            /// </summary>
            public int Hour { get; private set; }

            /// <summary>
            /// Minute for ensemble.
            /// </summary>
            public int Minute { get; private set; }

            /// <summary>
            /// Seconds for ensemble.
            /// </summary>
            public int Second { get; private set; }

            /// <summary>
            /// Hundreth of a Second for ensemble.
            /// </summary>
            public int HSec { get; private set; }

            /// <summary>
            /// Date and time of the ensemble.
            /// If the Year, Month or Day are not set, then this will
            /// return the current date and time.
            /// </summary>
            public DateTime EnsDateTime { get; private set; }

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
                DecodeEnsembleData(ensembleData);
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
                DecodeEnsembleData(ensembleData);
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

                // No bin data
                NumBins = 0;
            }

            /// <summary>
            /// Get all the information about the Ensemble.
            /// </summary>
            /// <param name="data">Byte array containing the Ensemble data type.</param>
            private void DecodeEnsembleData(byte[] data)
            {
                EnsembleNumber = Converters.ByteArrayToInt(data, GenerateIndex(0));
                NumBins = Converters.ByteArrayToInt(data, GenerateIndex(1));
                NumBeams = Converters.ByteArrayToInt(data, GenerateIndex(2));
                DesiredPingCount = Converters.ByteArrayToInt(data, GenerateIndex(3));
                ActualPingCount = Converters.ByteArrayToInt(data, GenerateIndex(4));
                Status = Converters.ByteArrayToInt(data, GenerateIndex(5));
                Year = Converters.ByteArrayToInt(data, GenerateIndex(6));
                Month = Converters.ByteArrayToInt(data, GenerateIndex(7));
                Day = Converters.ByteArrayToInt(data, GenerateIndex(8));
                Hour = Converters.ByteArrayToInt(data, GenerateIndex(9));
                Minute = Converters.ByteArrayToInt(data, GenerateIndex(10));
                Second = Converters.ByteArrayToInt(data, GenerateIndex(11));
                HSec = Converters.ByteArrayToInt(data, GenerateIndex(12));

                // Revision D additions
                if (NumElements == NUM_DATA_ELEMENTS_REV_D && data.Length >= NUM_DATA_ELEMENTS_REV_D)
                {
                    // Get the System Serial Number
                    // Start at byte 13
                    // The serial number has 8 elements
                    // BitConvert adds a - between each byte so remove
                    SysSerialNumber = BitConverter.ToString(data, 13, 8);
                    SysSerialNumber = SysSerialNumber.Replace("-", "");

                    // Get the firmware number 
                    Firmware = BitConverter.ToString(data, 21, 1);
                }
                else
                {
                    SysSerialNumber = "";
                    Firmware = "";
                }

                // Set the time and date
                ValidateDateTime(Year, Month, Day, Hour, Minute, Second, HSec * 10);

                // Create UniqueId
                UniqueId = new UniqueID(EnsembleNumber, EnsDateTime);
            }

            /// <summary>
            /// Move pass the Base data of the ensemble.  Then based
            /// off the index, move to the correct location.
            /// </summary>
            /// <param name="index">PlaybackIndex of the order of the data.</param>
            /// <returns>PlaybackIndex for the given xAxis.  Start of the data</returns>
            private int GenerateIndex(int index)
            {
                return GetBaseDataSize(NameLength) + (index * Ensemble.BYTES_IN_INT);
            }


            /// <summary>
            /// Get all the information about the Ensemble.
            /// </summary>
            /// <param name="data">DataRow containing the Ensemble data type.</param>
            private void DecodeEnsembleData(DataRow data)
            {
                // Go through the result settings
                EnsembleNumber = Convert.ToInt32(data[DbCommon.COL_ENS_ENS_NUM].ToString());
                NumBins = Convert.ToInt32(data[DbCommon.COL_ENS_NUM_BIN].ToString());
                NumBeams = Convert.ToInt32(data[DbCommon.COL_ENS_NUM_BEAM].ToString());
                DesiredPingCount = Convert.ToInt32(data[DbCommon.COL_ENS_DES_PING_COUNT].ToString());
                ActualPingCount = Convert.ToInt32(data[DbCommon.COL_ENS_ACT_PING_COUNT].ToString());
                Status = Convert.ToInt32(data[DbCommon.COL_ENS_STATUS].ToString());
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
                    SysSerialNumber = (data[DbCommon.COL_ENS_SYS_SERIAL].ToString());
                    Firmware = (data[DbCommon.COL_ENS_FIRMWARE].ToString());

                    // Set NumElements
                    NumElements = NUM_DATA_ELEMENTS_REV_D;
                }
                else
                {
                    SysSerialNumber = "";
                    Firmware = "";

                    // Set NumElements
                    NumElements = NUM_DATA_ELEMENTS_REV_C;
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
                    Debug.WriteLine(string.Format("Y{0} M{1} D{2} : H{3} M{4} S{5} MS{6}", year, month, day, hour, min, second, millisec));
                }
                // The specified parameters evaluate to less than DateTime.MinValue or more than DateTime.MaxValue
                catch (ArgumentException)
                {
                    EnsDateTime = new DateTime();
                    EnsDateTime = DateTime.MinValue;
                }
            }

            /// <summary>
            /// Override the ToString to return all the Ensemble data as a string.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                string s = "";
                s += "Ensemble: "  + EnsembleNumber + "\n";
                s += "Serial: " + SysSerialNumber + "\n";
                s += "Firmware: " + Firmware + "\n";
                s += EnsDateTime.ToString() + "\n";
                s += "Bins: " + NumBins + " Beams: " + NumBeams + "\n";
                s += "Pings Desired: " + DesiredPingCount + " actual: " + ActualPingCount + "\n";
                s += "Status: " + Status + "\n";
                s += "Status Errors:" + "\n";
                if (IsHardwareTimeout)
                    s += "Hardware Timeout" + "\n";

                return s;
            }
        }
    }
}