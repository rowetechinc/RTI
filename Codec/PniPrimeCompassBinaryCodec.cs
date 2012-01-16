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
 * Date            Initials    Comments
 * -----------------------------------------------------------------
 * 10/14/2011      RC          Initial coding
 * 10/18/2011      RC          Properly shutdown object
 * 11/14/2011      RC          Ensure a message was received in DecodeMessage().
 *                              Add method to clear the buffer.
 *                              Check if buffer size is larger then MIN to clear data if lenght not matching.
 * 11/16/2011      RC          Fixed bug where a negative number could be returned on cal.
 * 11/18/2011      RC          Removed debug output.
 * 11/30/2011      RC          Added ICodec.
 * 
 */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
namespace RTI
{
    /// <summary>
    /// Codec to decode a PNI Prime compass.  This 
    /// is used to do compass calibration.
    /// </summary>
    public class PniPrimeCompassBinaryCodec : ICodec
    {

        /// <summary>
        /// Size of the checksum value in bytes.
        /// </summary>
        private int CHECKSUM_SIZE = 2;

        /// <summary>
        /// Minimum size of a serial packet.
        /// </summary>
        private UInt16 PACKET_MIN_SIZE = 5;

        /// <summary>
        /// Maximum size from documentation (PACKET_MIN_SIZE + (4091 * 8)).
        /// </summary>
        private int MAX_LENGTH = 32733;

        /// <summary>
        /// First value to start with to look for data.
        /// </summary>
        private int FIRST_PACKET_INDEX = 3;

        /// <summary>
        /// Number of bytes for a float.
        /// </summary>
        private int SIZE_OF_FLOAT = 4;

        /// <summary>
        /// Number of bytes for a boolean.
        /// </summary>
        private int SIZE_OF_BOOLEAN = 1;

        // This value is used if we get stuck
        // on a very large expectedLength value
        private int _timeout = 0;

        /// <summary>
        /// Buffer for incoming data.
        /// </summary>
        private List<Byte> _incomingDataBuffer;

        /// <summary>
        /// Current size of the message within the buffer.
        /// </summary>
        protected int _currentMsgSize;

        /// <summary>
        /// Thread to process data when received.
        /// </summary>
        private Thread _processDataThread;

        /// <summary>
        /// Flag to continue loop.
        /// </summary>
        private bool _continue;

        /// <summary>
        /// Wait queue flag.
        /// </summary>
        private EventWaitHandle _eventWaitData;

        #region Enum and Structs

        /// <summary>
        /// Used to hold the Calibration score.
        /// </summary>
        public struct CalScore
        {
            /// <summary>
            /// Standard Deviation Error.
            /// </summary>
            public float stdDevErr { get; set; }

            /// <summary>
            /// X coverage.
            /// </summary>
            public float xCoverage { get; set; }
            
            /// <summary>
            /// Y Coverage.
            /// </summary>
            public float yCoverage { get; set; }
            
            /// <summary>
            /// Z Coverage.
            /// </summary>
            public float zCoverage { get; set; }
            
            /// <summary>
            /// X Acceleration Coverage.
            /// </summary>
            public UInt16 xAccelCoverage { get; set; }
            
            /// <summary>
            /// Y Acceleration Coverage.
            /// </summary>
            public UInt16 yAccelCoverage { get; set; }
            
            /// <summary>
            /// Z Acceleration Coverage.
            /// </summary>
            public UInt16 zAccelCoverage { get; set; }
            
            /// <summary>
            /// Acceleration Standard Deviation Error.
            /// </summary>
            public float accelStdDevErr { get; set; }
        }

        /// <summary>
        /// Enum to decide which 
        /// calibration mode to use.
        /// </summary>
        public enum CalMode
        {
            /// <summary>
            /// Magnetic calibration only.
            /// </summary>
            CAL_MAG_ONLY = 0,

            /// <summary>
            /// Acceleration calibartion only.
            /// </summary>
            CAL_ACCEL_ONLY = 100,

            /// <summary>
            /// Magnetic and Acceleration calibration.
            /// </summary>
            CAL_ACCEL_AND_MAG = 110
        }

        /// <summary>
        /// Enum to define all the IDs for the
        /// decoding.
        /// </summary>
        enum ID
        {
            // Frame IDs (Commands)
            kGetModInfo = 1,             // 1
            kModInfoResp = 2,            // 2
            kSetDataComponents = 3,      // 3
            kGetData = 4,                // 4
            kDataResp = 5,               // 5
            kSetConfig = 6,              // 6
            kGetConfig = 7,              // 7
            kConfigResp = 8,             // 8
            kSave = 9,                   // 9
            kStartCal = 10,              // 10
            kStopCal = 11,               // 11
            kSetParam = 12,              // 12
            kGetParam = 13,              // 13
            kParamResp = 14,             // 14
            kPowerDown = 15,             // 15
            kSaveDone = 16,              // 16
            kUserCalSampCount = 17,      // 17
            kUserCalScore = 18,          // 18
            kSetConfigDone = 19,         // 19
            kSetParamDone = 20,          // 20
            kStartIntervalMode = 21,     // 21
            kStopIntervalMode = 22,      // 22
            kPowerUp = 23,               // 23
            kSetAcqParams = 24,          // 24
            kGetAcqParams = 25,          // 25
            kAcqParamsDone = 26,         // 26
            kAcqParamsResp = 27,         // 27
            kPowerDoneDown = 28,         // 28
            kFactoryUserCal = 29,        // 29
            kFactoryUserCalDone = 30,    // 30
            kTakeUserCalSample = 31,     // 31
            kFactoryInclCal = 36,        // 36
            kFactoryInclCalDone = 37,    // 37
            
            // Param IDs
            kFIRConfig = 1,         //3-AxisID(UInt8)+ Count(UInt8)+Value(Float64)+...
            
            // Data Component IDs
            kHeading = 5,                // 5 - type Float32
            kDistortion = 8,             // 8 - type boolean
            kCalStatus = 9,              // 9 - type boolean
            kPAligned = 21,              // 21 - type Float32
            kRAligned = 22,              // 22 - type Float32
            kIZAligned = 23,             // 23 - type Float32
            kPAngle = 24,                // 24 - type Float32
            kRAngle = 25,                // 25 - type Float32
            kXAligned = 27,              // 27 - type Float32
            kYAligned = 28,              // 28 - type Float32
            kZAligned = 29,              // 29 - type Float32
            
            // Configuration Parameter IDs
            kDeclination = 1,               // 1 - type Float32
            kTrueNorth = 2,                 // 2 - type boolean
            kMountingRef = 10,              // 10 - type UInt8
            kUserCalStableCheck = 11,       // 11 - type boolean
            kUserCalNumPoints = 12,         // 12 - type UInt32
            kUserCalAutoSampling = 13,      // 13 – type boolean
            kBaudRate = 14,                 // 14 – UInt8
            
            // Mounting Reference IDs
            kMountedStandard = 1,   // 1
            kMountedXUp = 2,        // 2
            kMountedYUp = 3,        // 3
            kMountedStdPlus90 = 4,  // 4
            kMountedStdPlus180 = 5, // 5
            kMountedStdPlus270 = 6, // 6
            // Result IDs
            kErrNone = 0,           // 0
            kErrSave = 1,           // 1
        };

        #endregion

        /// <summary>
        /// Constructor
        /// 
        /// Initialize ranges
        /// </summary>
        public PniPrimeCompassBinaryCodec()
        {
            // Initialize ranges
            _incomingDataBuffer = new List<byte>();
            _currentMsgSize = 0;


            // Initialize the thread
            _continue = true;
            _eventWaitData = new EventWaitHandle(false, EventResetMode.AutoReset);
            _processDataThread = new Thread(ProcessDataThread);
            _processDataThread.Start();
        }

        // Methods
        #region Methods

        /// <summary>
        /// Stop the thread.
        /// </summary>
        private void StopThread()
        {
            _continue = false;

            // Wake up the thread to stop thread
            _eventWaitData.Set();
        }

        /// <summary>
        /// Shutdown this object.
        /// </summary>
        public void Shutdown()
        {
            StopThread();
        }

        #endregion

        // Methods to Create messages
        #region Create Messages

        /// <summary>
        /// Create a message based off the frame type
        /// and data given.  This will first calculate
        /// the size of the entire message.  Then it
        /// will start to construct the message.
        /// [byteCount (2 bytes)] [[Frame ID (1 byte)][data (0 - N bytes)]] [CRC (2 bytes)]
        /// 2 bytes for Byte count
        /// Packet
        ///    1 byte for Frame ID
        ///    N bytes for data
        /// 2 bytes for Checksum
        /// </summary>
        /// <param name="frameType">Frame ID type</param>
        /// <param name="data">Data for message.</param>
        /// <returns>Byte array containing the message.</returns>
        private byte[] CreateMsg(byte frameType, byte[] data)
        {
            UInt32 index = 0;           // Our location in the frame we are putting together
            UInt16 crc = 0;             // The CRC to add to the end of the packer
            UInt16 count = 0;           // The total length the packet will be

            // Check if there is any data to add to the command
            int dataLength = 0;
            if (data != null)
            {
                dataLength = data.Length;
            }

            // Get the length of the command
            count = (UInt16)(Convert.ToUInt16(dataLength) + PACKET_MIN_SIZE);
            byte[] buffer = new byte[count];

            //// Exit without sending if there is too much data to fit inside our packet
            //if (data.Length > kBufferSize - PACKET_MIN_SIZE)
            //{
            //    return;
            //}

            // Store the total len of the packet including the len
            // byteCount (2), the Frame ID(1), the data (data.length), and the crc (2)
            // If no data is sent, the min len is 5
            buffer[index++] = (byte)(count >> 8);
            buffer[index++] = (byte)(count & 0xff);

            // Store the frame ID
            buffer[index++] = frameType;

            // Copy the data to be sent
            if (data != null)
            {
                for (int x = 0; x < data.Length; x++)
                {
                    buffer[index++] = data[x];
                }
            }

            // Compute and add the CRC
            crc = CRC(buffer);
            buffer[index++] = (byte)(crc >> 8);
            buffer[index++] = (byte)(crc & 0xFF);

            return buffer;
        }

        /// <summary>
        /// Create the Get Mod Info command.
        /// </summary>
        /// <returns>Byte array with command.</returns>
        public byte[] GetModInfoCommand( )
        {
            return CreateMsg((int)ID.kGetModInfo, null);
        }

        /// <summary>
        /// Create the Start Calibration command.
        /// </summary>
        /// <returns>Byte array with command.</returns>
        public byte[] StartCalibrationCommand(CalMode mode)
        {
            byte[] data = UInt32ToByteArray((UInt32)mode);

            return CreateMsg((int)ID.kStartCal, data);
        }

        /// <summary>
        /// Create the Stop Calibration command.
        /// </summary>
        /// <returns>Byte array with command.</returns>
        public byte[] StopCalibrationCommand()
        {
            return CreateMsg((int)ID.kStopCal, null);
        }

        /// <summary>
        /// Create the Start Interval Mode command.
        /// </summary>
        /// <returns></returns>
        public byte[] StartIntervalModeCommand()
        {
            return CreateMsg((int)ID.kStartIntervalMode, null); 
        }

        /// <summary>
        /// Create the Stop Interval Mode command.
        /// </summary>
        /// <returns></returns>
        public byte[] StopIntervalModeCommand()
        {
            return CreateMsg((int)ID.kStopIntervalMode, null);
        }

        /// <summary>
        /// Save Compass Calibration.
        /// This frame commands the module to save internal
        /// configurations and user calibration to non-volatile memory.
        /// Internal configurations and user calibration is restored on
        /// power up.  
        /// </summary>
        /// <returns></returns>
        public byte[] SaveCompassCalCommand()
        {
            return CreateMsg((int)ID.kSave, null);
        }

        #endregion

        // Methods to Decode messages
        #region Decode Messages

        /// <summary>
        /// Take incoming data and add it to the
        /// buffer to be decoded.
        /// 
        /// Then start the thread to start decoding data
        /// </summary>
        /// <param name="data">Data to add to incoming buffer.</param>
        public void AddIncomingData(byte[] data)
        {
            for (int x = 0; x < data.Length; x++)
            {
                _incomingDataBuffer.Add(data[x]);
            }

            // Wake up the thread to process data
            _eventWaitData.Set();
        }

        /// <summary>
        /// Clear the buffer.
        /// </summary>
        public void ClearIncomingData()
        {
            _incomingDataBuffer.Clear();
        }

        /// <summary>
        /// Decode the ADCP data received using a seperate
        /// thread.  This thread will also pass data to all
        /// _observers.
        /// </summary>
        private void ProcessDataThread()
        {
            while (_continue)
            {
                // Block until awoken when data is received
                _eventWaitData.WaitOne();

                // If wakeup was called to kill thread
                if (!_continue)
                {
                    return;
                }

                // Decode the data sent to the codec
                DecodeIncomingData();
            }
        }

        /// <summary>
        /// Decode the incoming data buffer.
        /// This will look for messages in the
        /// buffer.  If a message is found, it will
        /// decode the message.
        /// </summary>
        private void DecodeIncomingData()
        {
            if( _incomingDataBuffer.Count >= PACKET_MIN_SIZE )
            {
                // Get the length of the message
                int expectedLength = GetMsgLength();

                // Check if the entire message has been received
                // If not, continue to wait for the message
                if (expectedLength > 0 && _incomingDataBuffer.Count >= expectedLength)
                {
                    // This will check the first 2 bytes
                    // and last 2 bytes to see if checksum
                    // passes
                    // If passes, decode the message
                    if (VerifyMsg(expectedLength))
                    {
                        // Copy the message to a byte array
                        // The size of the message was set when the message was found
                        byte[] message = new byte[_currentMsgSize];
                        _incomingDataBuffer.CopyTo(0, message, 0, _currentMsgSize);

                        // Remove message from buffer
                        _incomingDataBuffer.RemoveRange(0, _currentMsgSize);

                        // Decode the message
                        DecodeMessage(message);
                    }
                    // If the current data does not create a complete
                    // message, remove the first byte and try again
                    else
                    {
                        _incomingDataBuffer.RemoveAt(0);
                    }
                }
                else 
                {
                    // Length is too large to be correct
                    if (expectedLength > MAX_LENGTH || _timeout > 10)
                    {
                        _incomingDataBuffer.RemoveAt(0);
                        _timeout = 0;

                        // Try to decode the data now
                        DecodeIncomingData();
                    }

                    // Try to prevent expectedLength 
                    // from stalling the state
                    _timeout++;
                }
            }
        }

        private void DecodeMessage(byte[] msg)
        {
            // Ensure a good message was recieved.
            if (msg.Length >= 2)
            {
                //Debug.WriteLine("Found good Compass msg: " + msg);
                int frameType = msg[2];

                switch (frameType)
                {
                    case (int)ID.kDataResp:
                        DecodekDataResp(msg);
                        break;
                    case (int)ID.kUserCalSampCount:
                        DecodekUserCalSampCount(msg);
                        break;
                    case (int)ID.kUserCalScore:
                        DecodekUserCalScore(msg);
                        break;
                    default:
                        break;
                }
            }

        }

        /// <summary>
        /// ID 17
        /// Calibration Sample point received.
        /// This frame is sent from the module after taking a calibration sample point.  The 
        /// payload contains the sameple count with the range of 1 to 32.
        /// </summary>
        /// <param name="msg">Data to decode.</param>
        private void DecodekUserCalSampCount(byte[] msg)
        {
            // Start with xAxis 3
            // This is the start of a 4 byte UInt32
            UInt32 sampleCount = GetUInt32(ref msg, FIRST_PACKET_INDEX);

            // Call all subscribers with new sample count
            PropertyChange(this, new PropertyChangeEventArgs(EVENT_CAL_SAMPLE, sampleCount));
        }

        /// <summary>
        /// ID 18
        /// User Calibration Score.
        /// This contains the results of the calibration.
        /// This will be passed when the calibration is 
        /// complete and enough samples were taken.
        /// </summary>
        /// <param name="msg"></param>
        private void DecodekUserCalScore(byte[] msg)
        {
            CalScore score = new CalScore();
            int index = FIRST_PACKET_INDEX;
            
            // Get Std Dev Err
            score.stdDevErr = GetFloat(ref msg, index);
            index += SIZE_OF_FLOAT;

            // X Coverage
            score.xCoverage = GetFloat(ref msg, index);
            index += SIZE_OF_FLOAT;

            // Y Coverage
            score.yCoverage = GetFloat(ref msg, index);
            index += SIZE_OF_FLOAT;

            // Z Coverage
            score.zCoverage = GetFloat(ref msg, index);
            index += SIZE_OF_FLOAT;

            // XYZ Accel Coverage
            float xyzAccelCoverage = GetFloat(ref msg, index);
            index += SIZE_OF_FLOAT;

            // Accel StdDev Err
            score.accelStdDevErr = GetFloat(ref msg, index);

            // xyzAccelCoverage is in the XXYY.ZZ
            // Break up the ranges into there componenets
            string xyzAccelCoverageStr = xyzAccelCoverage.ToString();
            if (xyzAccelCoverageStr.Length >= 7)
            {
                try
                {
                    score.xAccelCoverage = Convert.ToUInt16(xyzAccelCoverageStr.Substring(0, 2));
                    score.yAccelCoverage = Convert.ToUInt16(xyzAccelCoverageStr.Substring(2, 2));
                    score.zAccelCoverage = Convert.ToUInt16(xyzAccelCoverageStr.Substring(5, 2));
                }
                catch (Exception)
                {
                    score.xAccelCoverage = 0;
                    score.yAccelCoverage = 0;
                    score.zAccelCoverage = 0;
                }
            }
            else
            {
                score.xAccelCoverage = 0;
                score.yAccelCoverage = 0;
                score.zAccelCoverage = 0;
            }

            // Call all subscribers with new sample count
            PropertyChange(this, new PropertyChangeEventArgs(EVENT_CAL_SCORE, score));
        }

        /// <summary>
        /// ID 5
        /// Decode the Data Response.
        /// This is a message sent by the compass
        /// at a fixed interval time.  It contains
        /// the Heading, pitch, roll
        /// </summary>
        /// <param name="msg">Data to decode</param>
        private void DecodekDataResp(byte[] msg)
        {
            int index = FIRST_PACKET_INDEX;
            int idCount = msg[index++];
            

            float heading = 0.0f;
            bool distortion = false;    // True = at least on magnetometer axis is reading beyond +/- 100 uT
            bool calStatus = false;     // False = not calibrated
            float pitch = 0.0f;
            float roll = 0.0f;
            float pAligned = 0.0f;      // User calibrated Earth's acceleration vector component output
            float rAligned = 0.0f;      // User calibrated Earth's acceleration vector component output 
            float izAligned = 0.0f;     // User calibrated Earth's acceleration vector component output 
            float xAligned = 0.0f;      // User calibrated Earth's magnetic field vector component output
            float yAligned = 0.0f;      // User calibrated Earth's magnetic field vector component output
            float zAligned = 0.0f;      // User calibrated Earth's magnetic field vector component output

            string str = String.Format("Compass Data: Num Types: {0} ", idCount);

            for (int count = 0; count < idCount; count++)
            {
                int id = msg[index++];

                // Set all the id ranges
                switch (id)
                {
                    case (int)ID.kHeading:
                        //heading = BitConverter.ToSingle(msg, xAxis);
                        heading = GetFloat(ref msg, index);
                        index += SIZE_OF_FLOAT;
                        str += String.Format("Heading: {0} ", heading);
                        break;
                    case (int)ID.kPAngle:
                        //pitch = BitConverter.ToSingle(msg, xAxis);
                        pitch = GetFloat(ref msg, index);
                        index += SIZE_OF_FLOAT;
                        str += String.Format("Pitch: {0} ", pitch);
                        break;
                    case (int)ID.kRAngle:
                        //roll = BitConverter.ToSingle(msg, xAxis);
                        roll = GetFloat(ref msg, index);
                        index += SIZE_OF_FLOAT;
                        str += String.Format("Roll: {0} ", roll);
                        break;
                    case (int)ID.kDistortion:
                        distortion = BitConverter.ToBoolean(msg, index);
                        index += SIZE_OF_BOOLEAN;
                        str += String.Format("Distortion: {0} ", distortion);
                        break;
                    case (int)ID.kCalStatus:
                        calStatus = BitConverter.ToBoolean(msg, index);
                        index += SIZE_OF_BOOLEAN;
                        str += String.Format("CalStatus: {0} ", calStatus);
                        break;
                    case (int)ID.kPAligned:
                        pAligned = GetFloat(ref msg, index);
                        index += SIZE_OF_FLOAT;
                        str += String.Format("PAligned: {0} ", pAligned);
                        break;
                    case (int)ID.kRAligned:
                        rAligned = GetFloat(ref msg, index);
                        index += SIZE_OF_FLOAT;
                        str += String.Format("RAligned: {0} ", rAligned);
                        break;
                    case (int)ID.kIZAligned:
                        izAligned = GetFloat(ref msg, index);
                        index += SIZE_OF_FLOAT;
                        str += String.Format("IZAligned: {0} ", izAligned);
                        break;
                    case (int)ID.kXAligned:
                        xAligned = GetFloat(ref msg, index);
                        index += SIZE_OF_FLOAT;
                        str += String.Format("XAligned: {0} ", xAligned);
                        break;
                    case (int)ID.kYAligned:
                        yAligned = GetFloat(ref msg, index);
                        index += SIZE_OF_FLOAT;
                        str += String.Format("YAligned: {0} ", yAligned);
                        break;
                    case (int)ID.kZAligned:
                        zAligned = GetFloat(ref msg, index);
                        index += SIZE_OF_FLOAT;
                        str += String.Format("ZAligned: {0} ", zAligned);
                        break;
                    default:
                        break;
                }
            }

            //Debug.WriteLine(str);
        }
        
        /// <summary>
        /// This will check the first 2 byte for a message
        /// length.  It will then go to the last 2 bytes to
        /// get a checksum value.  It will then calculate
        /// the checksum for the message, if they match,
        /// we have a complete message and can begin
        /// decoding that message.
        /// </summary>
        /// <returns></returns>
        private bool VerifyMsg( int expectedLength )
        {
            // Get and calculate the checksum of the message
            UInt16 crc, crcCalculated;
            crc = GetMsgChecksum(expectedLength);
            crcCalculated = CalculateMsgChecksum(expectedLength);

            // Verify they are the same
            if (crc == crcCalculated)
            {
                // Set the current message size and return true
                _currentMsgSize = expectedLength;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get the first 2 bytes in the message.
        /// Combine them with Big Endian to get the
        /// size of the message.
        /// </summary>
        /// <returns>Size of the message.</returns>
        private int GetMsgLength()
        {
            if (_incomingDataBuffer.Count > 2)
            {
                return (_incomingDataBuffer[0] << 8) | _incomingDataBuffer[1];
            }

            return -1;
        }

        /// <summary>
        /// This will go the end of the message based off the
        /// size and get the last 2 ranges.  It will combine
        /// them to form the checksum.
        /// </summary>
        /// <param name="size">Size of the message.</param>
        /// <returns>Checksum at the end of the message.</returns>
        private UInt16 GetMsgChecksum(int size)
        {
            if (size >= PACKET_MIN_SIZE)
            {
                // Get the last 2 ranges in the buffer
                // and combine to make a checksum value
                return (UInt16)((_incomingDataBuffer[size - 2] << 8) | _incomingDataBuffer[size - 1]);
            }

            return 0;
        }

        /// <summary>
        /// This will calculate the checksum for the
        /// incoming buffer based off a size given.
        /// This size will include all ranges including
        /// the checksum.  
        /// </summary>
        /// <param name="size">Size including bytecount, data and checksum.</param>
        /// <returns>Checksum value.</returns>
        private UInt16 CalculateMsgChecksum(int size)
        {
            return CRC(_incomingDataBuffer, 0, size-2);
        }

        #endregion

        // Utilties for the codec
        #region Utilities

        /// <summary>
        /// All data is received in Big Endian.
        /// If the system is in Little Endian, then
        /// the byte converter will not convert properly
        /// and the bytes will have to be reversed.
        /// </summary>
        /// <param name="data">Data to get the value.</param>
        /// <param name="index">Index in the array to start.</param>
        /// <returns>Float value from the byte array.</returns>
        private float GetFloat(ref byte[] data, int index)
        {
            // Get the 4 bytes and store to an array
            byte[] temp = new byte[4];
            temp[0] = data[index++];
            temp[1] = data[index++];
            temp[2] = data[index++];
            temp[3] = data[index];

            // Check if we need to reverse the data
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(temp);
            }

            return BitConverter.ToSingle(temp, 0);
        }

        /// <summary>
        /// All data is received in Big Endian.
        /// If the system is in Little Endian, then
        /// the byte converter will not convert properly
        /// and the bytes will have to be reversed.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private UInt32 GetUInt32(ref byte[] data, int index)
        {
            // Get the 4 bytes and store to an array
            byte[] temp = new byte[4];
            temp[0] = data[index++];
            temp[1] = data[index++];
            temp[2] = data[index++];
            temp[3] = data[index];

            // Check if we need to reverse the data
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(temp);
            }

            return BitConverter.ToUInt32(temp, 0);
        }

        /// <summary>
        /// Convert an UInt32 to byte array.
        /// The byte array must then be in Big Endian,
        /// so the byte array may need to be reversed.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private byte[] UInt32ToByteArray(UInt32 value)
        {
            byte[] temp = BitConverter.GetBytes(value);

            // If little endian, the bytes are in the
            // wrong order and reverse
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(temp);
            }

            return temp;
        }

        /// <summary>
        /// Calculate the checksum for the given data.
        /// This will use CRC-16 to calculate the checksum.
        /// Do not include the checksum in the calculation.
        /// </summary>
        /// <param name="data">Byte array containing the data to get the checksum.</param>
        /// <returns>Checksum value.</returns>
        private UInt16 CRC(byte[] data)
        {
            ushort crc = 0;

            // Do not include the checksum to calculate the checksum
            for (int i = 0; i < data.Length - CHECKSUM_SIZE; i++)
            {
                crc = (ushort)((byte)(crc >> 8) | (crc << 8));
                crc ^= data[i];
                crc ^= (byte)((crc & 0xff) >> 4);
                crc ^= (ushort)((crc << 8) << 4);
                crc ^= (ushort)(((crc & 0xff) << 4) << 1);
            }

            return crc;
        }

        /// <summary>
        /// Calculate the checksum for the given list.
        /// This will move to the index. given, then move
        /// through each element in the list for the given size.
        /// This will use CRC-16 to calculate the checksum.
        /// Do not include the checksum in the calculation.
        /// </summary>
        /// <param name="buffer">List of byte arrays.</param>
        /// <param name="index">Index in the buffer.</param>
        /// <param name="size">Size of the buffer.</param>
        /// <returns>Checksum value.</returns>
        private UInt16 CRC(List<byte> buffer, int index, int size)
        {
            ushort crc = 0;

            // Do not include the checksum to calculate the checksum
            for (int i = 0; i < size; i++)
            {
                crc = (ushort)((byte)(crc >> 8) | (crc << 8));
                crc ^= buffer[index + i];
                crc ^= (byte)((crc & 0xff) >> 4);
                crc ^= (ushort)((crc << 8) << 4);
                crc ^= (ushort)(((crc & 0xff) << 4) << 1);
            }

            return crc;
        }

        #endregion

        // Call to subscribers on property change
        #region Property Change Event Handling

        /// <summary>
        /// Class for an event argument to send and receive
        /// data through events.
        /// </summary>
        public class PropertyChangeEventArgs : EventArgs
        {
            /// <summary>
            /// Property name that has changed.
            /// </summary>
            public string PropertyName { get; internal set; }

            /// <summary>
            /// Value to hold for property changes.
            /// </summary>
            public object Value { get; internal set; }

            /// <summary>
            /// Property change event argument.
            /// </summary>
            /// <param name="propertyName">Property name.</param>
            /// <param name="data">Value to store.</param>
            public PropertyChangeEventArgs(string propertyName, object data)
            {
                this.PropertyName = propertyName;
                this.Value = data;
            }
        }

        /// <summary>
        /// Event when Cal Sample received.
        /// </summary>
        public const string EVENT_CAL_SAMPLE = "SAMPLE";

        /// <summary>
        /// Event when Cal Score received.
        /// </summary>
        public const string EVENT_CAL_SCORE = "CAL_SCORE";                                      

        /// <summary>
        /// Method to call when subscribing to an event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public delegate void PropertyChangeHandler(object sender, PropertyChangeEventArgs data);

        /// <summary>
        /// Event to subscribe to
        /// </summary>
        public event PropertyChangeHandler PropertyChange;

        /// <summary>
        /// An event has been fired.  This method
        /// will call all subscribers with the event.
        /// </summary>
        /// <param name="sender">This</param>
        /// <param name="data">The event to send.</param>
        public void OnPropertyChange(object sender, PropertyChangeEventArgs data)
        {
            // Check if there are any Subscribers
            if (PropertyChange != null)
            {
                // Call the Event
                PropertyChange(this, data);
            }
        }

        #endregion
    }
}