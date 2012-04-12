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
 * 10/14/2011      RC                     Initial coding
 * 10/18/2011      RC                     Properly shutdown object
 * 11/14/2011      RC                     Ensure a message was received in DecodeMessage().
 *                                         Add method to clear the buffer.
 *                                         Check if buffer size is larger then MIN to clear data if lenght not matching.
 * 11/16/2011      RC                     Fixed bug where a negative number could be returned on cal.
 * 11/18/2011      RC                     Removed debug output.
 * 11/30/2011      RC                     Added ICodec.
 * 12/10/2012      RC           2.02      Made calls to PropertyChanged use OnPropertyChanged to check for null.
 * 03/23/2012      RC           2.07      Changed how the incoming data is added to the buffer.
 *                                         Added classes to store results from the commands.
 * 03/26/2012      RC           2.07      Added GetUInt16() to convert array value to UInt16.
 *                                         Added DecodekSaveDone() to decode the kSaveDone message.
 *                                         Added GetDefaultCompassCalMagCommand() to get the command for the Factory defaults for Mag.
 * 03/29/2012      RC           2.07      Moved conversion to MathHelper.
 *                                         Added IS_BIG_ENDIAN to state all data is in Big Endian form.
 * 03/30/2012      RC           2.07      Surround processing thread in try/catch loop.  
 *                                         Check size of buffer before trying to remove the data.
 * 04/06/2012      RC           2.08      Changed serial port read thread, so make the codec decode in a while loop again.
 * 
 */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using log4net;
namespace RTI
{
    /// <summary>
    /// Codec to decode a PNI Prime compass.  This 
    /// is used to do compass calibration.
    /// </summary>
    public class PniPrimeCompassBinaryCodec : ICodec
    {

        /// <summary>
        /// Setup logger to report errors.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region DEFAULTS
        /// <summary>
        /// Default Declination.
        /// </summary>
        public const float DEFAULT_DECLINATION = 0.0f;

        /// <summary>
        /// Minimum Declination.
        /// </summary>
        public const float MIN_DECLINATION = -180.0f;

        /// <summary>
        /// Maximum Declination.
        /// </summary>
        public const float MAX_DECLINATION = 180.0f;

        /// <summary>
        /// Default True North
        /// </summary>
        public const bool DEFAULT_TRUE_NORTH = false;

        /// <summary>
        /// Default Big Endian.
        /// </summary>
        public const bool DEFAULT_BIG_ENDIAN = true;

        /// <summary>
        /// Default Mounting Reference.
        /// </summary>
        public const byte DEFAULT_MOUNTING_REF = 1;

        /// <summary>
        /// Minimum Mounting Reference.
        /// </summary>
        public const byte MIN_MOUNTING_REF = 1;

        /// <summary>
        /// Maximum Mounting Reference.
        /// </summary>
        public const byte MAX_MOUNTING_REF = 24;

        /// <summary>
        /// Default Calibration Stable Check.
        /// </summary>
        public const bool DEFAULT_USER_CAL_STABLE_CHECK = true;

        /// <summary>
        /// Default Number of points in calibration.
        /// </summary>
        public const UInt32 DEFAULT_USER_CAL_NUM_POINTS = 12;

        /// <summary>
        /// Minimum number of Calibration points.
        /// </summary>
        public const UInt32 MIN_USER_CAL_NUM_POINTS = 12;

        /// <summary>
        /// Maximum number of Calibration points.
        /// </summary>
        public const UInt32 MAX_USER_CAL_NUM_POINTS = 32;

        /// <summary>
        /// Default Calibration Auto sampling.
        /// </summary>
        public const bool DEFAULT_USER_CAL_AUTO_SAMPLING = true;

        /// <summary>
        /// Default baudrate.
        /// </summary>
        public const byte DEFAULT_BAUD_RATE = 12;

        /// <summary>
        /// Minimum Baudrate.
        /// </summary>
        public const byte MIN_BAUD_RATE = 0;

        /// <summary>
        /// Maximum Baudrate.
        /// </summary>
        public const byte MAX_BAUD_RATE = 14;

        #endregion

        #region Variables

        /// <summary>
        /// Set flag that all the data from the compass
        /// is in Big Endian form.
        /// </summary>
        private const bool IS_BIG_ENDIAN = true;

        /// <summary>
        /// Size of the checksum value in bytes.
        /// </summary>
        private const int CHECKSUM_SIZE = 2;

        /// <summary>
        /// Minimum size of a serial packet.
        /// </summary>
        private const UInt16 PACKET_MIN_SIZE = 5;

        /// <summary>
        /// Maximum size from documentation (PACKET_MIN_SIZE + (4091 * 8)).
        /// </summary>
        private const int MAX_LENGTH = 32733;

        /// <summary>
        /// The number of tries to wait for more incoming
        /// data before removing the first element and trying again.
        /// </summary>
        private const int TIMEOUT = 10;

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
        /// Lock the buffer of data.
        /// </summary>
        private readonly object _bufferLock = new object();

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
        private volatile bool _continue;

        /// <summary>
        /// Wait queue flag.
        /// </summary>
        private EventWaitHandle _eventWaitData;

        #endregion

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
        /// Response when sending the command kGetData.
        /// </summary>
        public class DataResponse
        {
            /// <summary>
            /// Compass Heading output.
            /// Format: Float32
            /// Units: Degrees
            /// Range: 0.0 to 359.9.
            /// </summary>
            public float Heading {get; set;}

            /// <summary>
            /// Distortion.
            /// True = at least on magnetometer axis is reading beyond +/- 100 uT
            /// Format: Boolean
            /// Units: True / False
            /// Range: False (Default) = No Distortion
            /// </summary>
            public bool Distortion { get; set; }

            /// <summary>
            /// Read only flag that indicates user calibration status.
            /// False (Default) = Not calibrated.
            /// Format: Boolean
            /// Units: True / False
            /// Range: False (Default) = Not calibrated.
            /// </summary>
            public bool CalStatus { get; set; }

            /// <summary>
            /// Pitch angle output.  
            /// Format: Float32
            /// Units: Degrees
            /// Range: -90.0 to 90.0
            /// </summary>
            public float Pitch { get; set; }

            /// <summary>
            /// Roll anglue output.
            /// Format: Float32
            /// Units: Degrees
            /// Range: -180.0 to 180.0
            /// </summary>
            public float Roll { get; set; }

            /// <summary>
            /// User calibrated Earth's acceleration Pitch vector component output.
            /// Format: Float32
            /// Units: G
            /// Range: -1.0 to 1.0
            /// </summary>
            public float pAligned { get; set; }

            /// <summary>
            /// User calibrated Earth's acceleration Roll vector component output.
            /// Format: Float32
            /// Units: G
            /// Range: -1.0 to 1.0
            /// </summary>
            public float rAligned { get; set; }

            /// <summary>
            /// User calibrated Earth's acceleration Z vector component output.
            /// Format: Float32
            /// Units: G
            /// Range: -1.0 to 1.0
            /// </summary>
            public float izAligned { get; set; }

            /// <summary>
            /// User calibrated Earth's magnetic field X vector component output.
            /// Format: Float32
            /// Units: uT
            /// Range:
            /// </summary>
            public float xAligned { get; set; }

            /// <summary>
            /// User calibrated Earth's magnetic field Y vector component output.
            /// Format: Float32
            /// Units: uT
            /// Range:
            /// </summary>
            public float yAligned { get; set; }

            /// <summary>
            /// User calibrated Earth's magnetic field Z vector component output.
            /// Format: Float32
            /// Units: uT
            /// Range:
            /// </summary>
            public float zAligned { get; set; }

            /// <summary>
            /// Set all default values.
            /// </summary>
            public DataResponse()
            {
                Heading = 0.0f;
                Distortion = false;
                CalStatus = false;
                Pitch = 0.0f;
                Roll = 0.0f;
                pAligned = 0.0f;
                rAligned = 0.0f;
                izAligned = 0.0f;
                xAligned = 0.0f;
                xAligned = 0.0f;
                xAligned = 0.0f;
            }
        }

        /// <summary>
        /// Response received from sending the command kGetConfig.
        /// </summary>
        public class Configuration
        {
            /// <summary>
            /// This sets the declination angle to determine True
            /// North heading.  Positive declination is easterly declination
            /// and negative is westerly declination.  THis is not applied until
            /// kTrueNorth is set to true.
            /// Format: Float32
            /// Units: Degrees
            /// Range: -180.0 to 180.0
            /// Default Value: 0
            /// </summary>
            public float Declination {get; set;}

            /// <summary>
            /// Flag to set compass heading output to true north
            /// heading by adding the declination angle to the magnetic 
            /// north heading.
            /// Format: Boolean
            /// Units: True / False
            /// Range: 
            /// Default Value: False
            /// </summary>
            public bool TrueNorth {get; set;}

            /// <summary>
            /// Flag to set the Endianness of packets.
            /// Format: Boolean
            /// Units: True / False
            /// Range: 
            /// Default Value: True
            /// </summary>
            public bool BigEndian { get; set; }

            /// <summary>
            /// This sets the reference orientation for the module.
            /// Standard: When selected the unit is to be mounted with the main board in a
            ///   horizontal position (the Z axis magnetic sensor is vertical).
            /// X Sensor Up: When selected the unit is to be mounted with the main board in a
            ///   vertical position: the X axis magnetic sensor is vertical and points up.
            /// Y Sensor Up: When selected the unit is to be mounted with the main board in a
            ///   vertical position: the Y axis magnetic sensor is vertical and points up.
            /// X Sensor Down: When selected the unit is to be mounted with the main board
            ///   in a vertical position: the X axis magnetic sensor is vertical and points down.
            /// Y Sensor Down: When selected the unit is to be mounted with the main board
            ///   in a vertical position: the Y axis magnetic sensor is vertical and points down.
            /// Standard 90 Degrees: When selected the unit is to be mounted with the main
            ///   board in a horizontal position but rotated so the arrow is pointed 90 degrees
            ///   counterclockwise to the front of the host system.
            /// Standard 180 Degrees: When selected the unit is to be mounted with the main
            ///   board in a horizontal position but rotated so the arrow is pointed 180 degrees
            ///   counterclockwise to the front of the host system.
            /// Standard 270 Degrees: When selected the unit is to be mounted with the main
            ///   board in a horizontal position but rotated so the arrow is pointed 270 degrees
            ///   counterclockwise to the front of the host system.
            /// Format: UInt8
            /// Units: Code
            /// Range: 1 - 24
            /// Default Value: 1
            /// </summary>
            public int MountingRef { get; set; }

            /// <summary>
            /// This flag is used during user calibration.  If set to FALSE, 
            /// the module will take a point if the magnetic field has changed 
            /// more than 23 uT in either axis.  If set to TRUE the unit will
            /// take a point if the magnetic field has a stability of 30 uT in
            /// each direction and the previous point changed more than 5 uT
            /// and acceleration vector delta within 2 mg.
            /// Format: Boolean
            /// Units: True / False
            /// Range: 
            /// Default Value: True
            /// </summary>
            public bool UserCalStableCheck { get; set; }

            /// <summary>
            /// The maximum number of samples taken during user
            /// calibration.
            /// Format: UInt32
            /// Units: Number of Points.
            /// Range: 12 - 32.
            /// Default Value: 12
            /// </summary>
            public UInt32 UserCalNumPoints { get; set; }

            /// <summary>
            /// This flag is used during user calibration.  If set to TRUE,
            /// the module will continuously take calibration sample points
            /// until the set number of calibration samples.  If set to
            /// FALSE, the module waits for kTakeUserCalSample frame to take
            /// a sample with the condition that a magnetic field vector 
            /// componenet delta is greater than 5 micro Telsa from the
            /// last sample point.
            /// Format: Boolean
            /// Units: True / False
            /// Range: 
            /// Default Value: True
            /// </summary>
            public bool UserCalAutoSampling { get; set; }

            /// <summary>
            /// Baudrate index value.  A power-down power-up cycle is
            /// required when changing the baudrate.
            /// Format: UInt8
            /// Units: Baudrate code.
            /// Range: 0 - 14.
            /// Default Value: 12 (38400)
            /// </summary>
            public int BaudRate { get; set; }

            /// <summary>
            /// Set the default values.
            /// </summary>
            public Configuration()
            {
                Declination = DEFAULT_DECLINATION;
                TrueNorth = DEFAULT_TRUE_NORTH;
                BigEndian = DEFAULT_BIG_ENDIAN;
                MountingRef = DEFAULT_MOUNTING_REF;
                UserCalStableCheck = DEFAULT_USER_CAL_STABLE_CHECK;
                UserCalNumPoints = DEFAULT_USER_CAL_NUM_POINTS;
                UserCalAutoSampling = DEFAULT_USER_CAL_AUTO_SAMPLING;
                BaudRate = DEFAULT_BAUD_RATE;
            }

            /// <summary>
            /// Return the string for the Mounting Reference code.
            /// </summary>
            /// <param name="mntRef">Mounting Reference code.</param>
            /// <returns>String for the Mounting Reference code.</returns>
            public static string MountingRefToString(int mntRef)
            {
                switch(mntRef)
                {
                    case 1:
                        return "Standard";
                    case 2:
                        return "X axis up";
                    case 3:
                        return "Y axis up";
                    case 4:
                        return "-90° heading offset";
                    case 5:
                        return "-180° heading offset";
                    case 6:
                        return "-270° heading offset";
                    case 7:
                        return "Z down";
                    case 8:
                        return "X + 90°";
                    case 9:
                        return "X + 180°";
                    case 10:
                        return "X + 270°";
                    case 11:
                        return "Y + 90°";
                    case 12:
                        return "Y + 180°";
                    case 13:
                        return "Y + 270°";
                    case 14:
                        return "Z down + 90°";
                    case 15:
                        return "Z down + 180°";
                    case 16:
                        return "Z down + 270°";
                    case 17:
                        return "X down";
                    case 18:
                        return "X down + 90°";
                    case 19:
                        return "X down + 180°";
                    case 20:
                        return "X down + 270°";
                    case 21:
                        return "Y down";
                    case 22:
                        return "Y down + 90°";
                    case 23:
                        return "Y down + 180°";
                    case 24:
                        return "Y down + 270°";
                    default:
                        return "INVALID";
                }
            }

            /// <summary>
            /// Return the string of the baudrate for
            /// the given baudrate code.
            /// </summary>
            /// <param name="baudrate">Baudrate code.</param>
            /// <returns>String for the baudrate code.</returns>
            public static string BaudRateToString(int baudrate)
            {
                switch(baudrate)
                {
                    case 0:
                        return "300";
                    case 1:
                        return "600";
                    case 2:
                        return "1200";
                    case 3:
                        return "1800";
                    case 4:
                        return "2400";
                    case 5:
                        return "3600";
                    case 6:
                        return "4800";
                    case 7:
                        return "7200";
                    case 8:
                        return "9600";
                    case 9:
                        return "14400";
                    case 10:
                        return "19200";
                    case 11:
                        return "28800";
                    case 12:
                        return "38400";
                    case 14:
                        return "57600";
                    case 15:
                        return "115200";
                    default:
                        return "INVALID";
                }
            }
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
        public enum ID
        {
            // Frame IDs (Commands)

            /// <summary>
            /// 1 Queries the modules type and firmware revision number
            /// </summary>
            kGetModInfo = 1,
            
            /// <summary>
            /// 2 Response to kGetModInfo
            /// </summary>
            kModInfoResp = 2,

            /// <summary>
            /// 3 Sets the data components to be output
            /// </summary>
            kSetDataComponents = 3,

            /// <summary>
            /// 4 Queries the module for data
            /// </summary>
            kGetData = 4,

            /// <summary>
            /// 5 Response to kGetData
            /// </summary>
            kDataResp = 5,

            /// <summary>
            /// 6 Sets internal configuration in the module
            /// </summary>
            kSetConfig = 6,

            /// <summary>
            /// 7 Queries the module for the current internal configuration value
            /// </summary>
            kGetConfig = 7,

            /// <summary>
            /// 8 Response to kGetConfig
            /// </summary>
            kConfigResp = 8,

            /// <summary>
            /// 9 Commands the module to save internal and user calibration
            /// </summary>
            kSave = 9,

            /// <summary>
            /// 10 Commands module to start user calibration
            /// </summary>
            kStartCal = 10,

            /// <summary>
            /// 11 Commands the module to stop user calibration
            /// </summary>
            kStopCal = 11,

            /// <summary>
            /// 12 Sets the FIR filter settings for the magnetometer and accelerometer sensors
            /// </summary>
            kSetParam = 12,

            /// <summary>
            /// 13 Queries for the FIR filter settings for the magnetometer and accelerometer sensors
            /// </summary>
            kGetParam = 13,

            /// <summary>
            /// 14 Contains the FIR filter setting for the magnetometer and accelerometer sensors
            /// </summary>
            kParamResp = 14,

            /// <summary>
            /// 15 Used to completely power-down the module
            /// </summary>
            kPowerDown = 15,

            /// <summary>
            /// 16 Response to kSave
            /// </summary>
            kSaveDone = 16,

            /// <summary>
            /// 17 Sent from the module after taking a calibration sample point
            /// </summary>
            kUserCalSampCount = 17,

            /// <summary>
            /// 18 Contains the calibration score
            /// </summary>
            kUserCalScore = 18,

            /// <summary>
            /// 19 Response to kSetConfig
            /// </summary>
            kSetConfigDone = 19,

            /// <summary>
            /// 20 Response to kSetParam
            /// </summary>
            kSetParamDone = 20,

            /// <summary>
            /// 21 Commands the module to output data at a fixed interval
            /// </summary>
            kStartIntervalMode = 21,

            /// <summary>
            /// 22 Commands the module to stop data output at a fixed interval
            /// </summary>
            kStopIntervalMode = 22,

            /// <summary>
            /// 23 Sent after wake up from power down mode
            /// </summary>
            kPowerUp = 23,

            /// <summary>
            /// 24 Sets the sensor acquistion parameters
            /// </summary>
            kSetAcqParams = 24,

            /// <summary>
            /// 25 Queries for the sensor acquisition parameters 
            /// </summary>
            kGetAcqParams = 25,

            /// <summary>
            /// 26 Response to kSetAcqParams
            /// </summary>
            kAcqParamsDone = 26,

            /// <summary>
            /// 27 Response to kGetAcqParams
            /// </summary>
            kAcqParamsResp = 27,

            /// <summary>
            /// 28 Response to kPowerDown
            /// </summary>
            kPowerDoneDown = 28,

            /// <summary>
            /// 29 Clears user magnetometer calibration coefficients
            /// </summary>
            kFactoryUserCal = 29,

            /// <summary>
            /// 30 Response to kFactoryUserCal
            /// </summary>
            kFactoryUserCalDone = 30,

            /// <summary>
            /// 31 Commands the unit to take a sample during user calibration
            /// </summary>
            kTakeUserCalSample = 31,

            /// <summary>
            /// 36 Clears user accelerometer calibration coefficients
            /// </summary>
            kFactoryInclCal = 36,

            /// <summary>
            /// 37 Response to kFactoryInclCal
            /// </summary>
            kFactoryInclCalDone = 37,
            
            // Param IDs
            /// <summary>
            /// 3-AxisID(UInt8)+ Count(UInt8)+Value(Float64)+...
            /// </summary>
            kFIRConfig = 1,
            
            // Data Component IDs
            /// <summary>
            /// 5 - type Float32
            /// </summary>
            kHeading = 5,
 
            /// <summary>
            /// 8 - type boolean
            /// </summary>
            kDistortion = 8,

            /// <summary>
            /// 9 - type boolean
            /// </summary>
            kCalStatus = 9,

            /// <summary>
            /// 21 - type Float32
            /// </summary>
            kPAligned = 21,

            /// <summary>
            /// 22 - type Float32
            /// </summary>
            kRAligned = 22,

            /// <summary>
            /// 23 - type Float32
            /// </summary>
            kIZAligned = 23,

            /// <summary>
            /// 24 - type Float32
            /// </summary>
            kPAngle = 24,

            /// <summary>
            /// 25 - type Float32
            /// </summary>
            kRAngle = 25,

            /// <summary>
            /// 27 - type Float32
            /// </summary>
            kXAligned = 27,

            /// <summary>
            /// 28 - type Float32
            /// </summary>
            kYAligned = 28,

            /// <summary>
            /// 29 - type Float32
            /// </summary>
            kZAligned = 29,
            
            // Configuration Parameter IDs
            /// <summary>
            /// 1 - type Float32
            /// </summary>
            kDeclination = 1,

            /// <summary>
            /// 2 - type boolean
            /// </summary>
            kTrueNorth = 2,

            /// <summary>
            /// 6 - type boolean
            /// </summary>
            kBigEndian = 6,

            /// <summary>
            /// 10 - type UInt8
            /// </summary>
            kMountingRef = 10,

            /// <summary>
            /// 11 - type boolean
            /// </summary>
            kUserCalStableCheck = 11,
            
            /// <summary>
            /// 12 - type UInt32
            /// </summary>
            kUserCalNumPoints = 12,

            /// <summary>
            /// 13 – type boolean
            /// </summary>
            kUserCalAutoSampling = 13,

            /// <summary>
            /// 14 – UInt8
            /// </summary>
            kBaudRate = 14,
            
            // Mounting Reference IDs

            /// <summary>
            /// 1
            /// </summary>
            kMountedStandard = 1,

            /// <summary>
            /// 
            /// </summary>
            kMountedXUp = 2,

            /// <summary>
            /// 
            /// </summary>
            kMountedYUp = 3,

            /// <summary>
            /// 
            /// </summary>
            kMountedStdPlus90 = 4,

            /// <summary>
            /// 
            /// </summary>
            kMountedStdPlus180 = 5,

            /// <summary>
            /// 
            /// </summary>
            kMountedStdPlus270 = 6,

            // Result IDs
            /// <summary>
            /// 0 No Error
            /// </summary>
            kErrNone = 0,

            /// <summary>
            /// 1 Error Saving
            /// </summary>
            kErrSave = 1,
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
        private static byte[] CreateMsg(byte frameType, byte[] data)
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
        /// Create a message based off the frame type, Config ID
        /// and data given.  This will first calculate
        /// the size of the entire message.  Then it
        /// will start to construct the message.
        /// [byteCount (2 bytes)] [[Frame ID (1 byte)][Payload (0 - N bytes)]] [CRC (2 bytes)]
        /// [Payload] = [[Config ID(1 byte)][Value (0 - N bytes)]]
        /// 2 bytes for Byte count
        /// Packet
        ///    1 byte for Frame ID
        ///    N bytes for Playload
        ///        1 Byte for Config ID
        ///        N bytes for Value
        /// 2 bytes for Checksum
        /// </summary>
        /// <param name="frameType">Frame ID type</param>
        /// <param name="configId">Config ID to update.</param>
        /// <param name="data">Data for message.</param>
        /// <returns>Byte array containing the message.</returns>
        private static byte[] CreateConfigMsg(ID frameType, ID configId, byte[] data)
        {
            UInt32 index = 0;           // Our location in the frame we are putting together
            UInt16 crc = 0;             // The CRC to add to the end of the packer
            UInt16 count = 0;           // The total length the packet will be

            // Check if there is any data to add to the command
            int payload = 0;
            if (data != null)
            {
                payload = data.Length;      // Value size
                payload += 1;               // Config ID
            }

            // Get the length of the command
            count = (UInt16)(Convert.ToUInt16(payload) + PACKET_MIN_SIZE);
            byte[] buffer = new byte[count];

            // Store the total len of the packet including the len
            // byteCount (2), the Frame ID(1), the data (data.length), and the crc (2)
            // If no data is sent, the min len is 5
            buffer[index++] = (byte)(count >> 8);
            buffer[index++] = (byte)(count & 0xff);

            // Store the frame ID
            buffer[index++] = (byte)frameType;

            // Copy the data to be sent
            if (data != null)
            {
                // Set the Config ID
                buffer[index++] = (byte)configId;

                // Set the value
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
        public static byte[] GetModInfoCommand( )
        {
            return CreateMsg((int)ID.kGetModInfo, null);
        }

        /// <summary>
        /// Create the Start Calibration command.
        /// </summary>
        /// <returns>Byte array with command.</returns>
        public static byte[] StartCalibrationCommand(CalMode mode)
        {
            byte[] data = MathHelper.UInt32ToByteArray((UInt32)mode, IS_BIG_ENDIAN);

            return CreateMsg((int)ID.kStartCal, data);
        }

        /// <summary>
        /// Create the Stop Calibration command.
        /// </summary>
        /// <returns>Byte array with command.</returns>
        public static byte[] StopCalibrationCommand()
        {
            return CreateMsg((int)ID.kStopCal, null);
        }

        /// <summary>
        /// Create the Start Interval Mode command.
        /// </summary>
        /// <returns>Byte array with command.</returns>
        public static byte[] StartIntervalModeCommand()
        {
            return CreateMsg((int)ID.kStartIntervalMode, null); 
        }

        /// <summary>
        /// Create the Stop Interval Mode command.
        /// </summary>
        /// <returns>Byte array with command.</returns>
        public static byte[] StopIntervalModeCommand()
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
        /// <returns>Byte array with command.</returns>
        public static byte[] SaveCompassCalCommand()
        {
            return CreateMsg((int)ID.kSave, null);
        }

        /// <summary>
        /// Get the Data from the compass.  This will get
        /// data like the heading, pitch, roll and
        /// distortion.  Results will be stored in DataResponse.
        /// 
        /// The output for this is set by kSetDataComponents.  The
        /// kSetDataComonents states which IDs will be given in 
        /// kGetData.
        /// </summary>
        /// <returns>Byte array with command.</returns>
        public static byte[] GetDataCommand()
        {
            return CreateMsg((int)ID.kGetData, null);
        }

        /// <summary>
        /// This frame cleas the user magnetometer calibration coefficents.  The frame
        /// has no payload.  This frame must be followed by the kSave frame to change in non-volatile
        /// memory.
        /// </summary>
        /// <returns>Byte array with command.</returns>
        public static byte[] GetDefaultCompassCalMagCommand()
        {
            return CreateMsg((int)ID.kFactoryUserCal, null);
        }

        /// <summary>
        /// This frame commands the unit to take a sample during user caliberation.  The frame has no payload.
        /// </summary>
        /// <returns>Byte array with command.</returns>
        public static byte[] GetTakeUserCalSampleCommand()
        {
            return CreateMsg((int)ID.kTakeUserCalSample, null);
        }

        /// <summary>
        /// This frame cleas the user accelerometer calibration coefficents.  The frame
        /// has no payload.  This frame must be followed by the kSave frame to change in non-volatile
        /// memory.
        /// </summary>
        /// <returns>Byte array with command.</returns>
        public static byte[] GetDefaultCompassCalAccelCommand()
        {
            return CreateMsg((int)ID.kFactoryInclCal, null);
        }

        /// <summary>
        /// Set the Data output to all the components.
        /// This will set kGetData to output all the components.
        /// </summary>
        /// <returns>Byte array of the command.</returns>
        public static byte[] SetAllDataComponentsCommand()
        {
            int numComponenets = 11;

            int count = numComponenets + 1;     // Add one to include the ID count
            byte[] payload = new byte[count];

            payload[0] = (byte)count;
            payload[1] = (byte)ID.kHeading;
            payload[2] = (byte)ID.kPAngle;
            payload[3] = (byte)ID.kRAngle;
            payload[4] = (byte)ID.kDistortion;
            payload[5] = (byte)ID.kCalStatus;
            payload[6] = (byte)ID.kPAligned;
            payload[7] = (byte)ID.kRAligned;
            payload[8] = (byte)ID.kZAligned;
            payload[9] = (byte)ID.kXAligned;
            payload[10] = (byte)ID.kYAligned;
            payload[11] = (byte)ID.kZAligned;

            return CreateMsg((int)ID.kSetDataComponents, payload);
        }

        /// <summary>
        /// Return the Data output back to the default
        /// heading, pitch and roll.
        /// </summary>
        /// <returns>Byte array of Data components.</returns>
        public static byte[] SetHPRDataComponentsCommands()
        {
            int numComponenets = 3;

            int count = numComponenets + 1;     // Add one to include the ID count
            byte[] payload = new byte[count];

            payload[0] = (byte)count;
            payload[1] = (byte)ID.kHeading;
            payload[2] = (byte)ID.kPAngle;
            payload[3] = (byte)ID.kRAngle;

            return CreateMsg((int)ID.kSetDataComponents, payload);
        }

        /// <summary>
        /// The frame queries the module for the current internal configuration value.  THe
        /// payload contains the configuration ID request.
        /// </summary>
        /// <returns>Byte array with command.</returns>
        public static byte[] GetConfigCommand(ID configId)
        {
            byte[] id = new byte[1];
            id[0] = (byte)configId;

            return CreateMsg((int)ID.kGetConfig, id);
        }

        /// <summary>
        /// Create the Configuration command.  The configuration command takes
        /// only 1 command at a time.  So create the message with the payload
        /// being the Config ID and Value.
        /// If any value is given that is bad, return NULL.
        /// </summary>
        /// <param name="id">Config ID.</param>
        /// <param name="value">New Value.</param>
        /// <returns>Byte array with the command.  Return NULL if any values are bad.</returns>
        public static byte[] SetConfigCommand(ID id, object value)
        {
            try
            {
                // Set all the id ranges
                switch (id)
                {
                    case ID.kDeclination:
                        if ((float)value < MIN_DECLINATION || (float)value > MAX_DECLINATION)
                        {
                            return null;
                        }
                        return CreateConfigMsg(ID.kSetConfig, ID.kDeclination, MathHelper.FloatToByteArray((float)value, IS_BIG_ENDIAN));
                    case ID.kTrueNorth:
                        return CreateConfigMsg(ID.kSetConfig, ID.kTrueNorth, MathHelper.BooleanToByteArray((bool)value));
                    case ID.kBigEndian:
                        return CreateConfigMsg(ID.kSetConfig, ID.kBigEndian, MathHelper.BooleanToByteArray((bool)value));
                    case ID.kMountingRef:
                        if ((byte)value < MIN_MOUNTING_REF || (byte)value > MAX_MOUNTING_REF)
                        {
                            return null;
                        }
                        return CreateConfigMsg(ID.kSetConfig, ID.kMountingRef, MathHelper.UInt8ToByteArray((byte)value));
                    case ID.kUserCalStableCheck:
                        return CreateConfigMsg(ID.kSetConfig, ID.kUserCalStableCheck, MathHelper.BooleanToByteArray((bool)value));
                    case ID.kUserCalNumPoints:
                        if ((UInt32)value < MIN_USER_CAL_NUM_POINTS || (UInt32)value > MAX_USER_CAL_NUM_POINTS)
                        {
                            return null;
                        }
                        return CreateConfigMsg(ID.kSetConfig, ID.kUserCalNumPoints, MathHelper.UInt32ToByteArray((UInt32)value, IS_BIG_ENDIAN));
                    case ID.kUserCalAutoSampling:
                        return CreateConfigMsg(ID.kSetConfig, ID.kUserCalAutoSampling, MathHelper.BooleanToByteArray((bool)value));
                    case ID.kBaudRate:
                        if ((byte)value < MIN_BAUD_RATE || (byte)value > MAX_BAUD_RATE)
                        {
                            return null;
                        }
                        return CreateConfigMsg(ID.kSetConfig, ID.kBaudRate, MathHelper.UInt8ToByteArray((byte)value));
                    default:
                        return null;
                }
            }
            catch (InvalidCastException)
            {
                // Bad value given
                return null;
            }
        }

        #endregion

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
            lock (_bufferLock)
            {
                //Debug.WriteLine("Buffer Size: {0}  Incoming data size: {1}", _incomingDataBuffer.Count, data.Length);

                // Add new data to the buffer
                _incomingDataBuffer.AddRange(data);
            }

            // Wake up the thread to process data
            _eventWaitData.Set();
        }

        /// <summary>
        /// Clear the buffer.
        /// </summary>
        public void ClearIncomingData()
        {
            lock (_bufferLock)
            {
                _incomingDataBuffer.Clear();
            }
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
            try
            {
                while (_incomingDataBuffer.Count >= PACKET_MIN_SIZE)
                {
                    // Get the length of the message
                    int expectedLength = GetMsgLength();

                    //Debug.WriteLine("Incoming Data: Buffer Size: {0} length: {1}", _incomingDataBuffer.Count, expectedLength);

                    // Check if the entire message has been received
                    // If not, continue to wait for the message
                    if (expectedLength > 0 && expectedLength <= _incomingDataBuffer.Count)
                    {
                        // This will check the first 2 bytes
                        // and last 2 bytes to see if checksum
                        // passes
                        // If passes, decode the message
                        if (VerifyMsg(expectedLength))
                        {
                            byte[] message = new byte[_currentMsgSize];
                            lock (_bufferLock)
                            {
                                // Due to multithreading, the buffer may have been cleared
                                // before this lock is set, ensure the data still exist
                                if (_incomingDataBuffer.Count >= _currentMsgSize)
                                {
                                    // Copy the message to a byte array
                                    // The size of the message was set when the message was found
                                    _incomingDataBuffer.CopyTo(0, message, 0, _currentMsgSize);

                                    // Remove message from buffer
                                    _incomingDataBuffer.RemoveRange(0, _currentMsgSize);
                                }
                            }

                            // Decode the message
                            DecodeMessage(message);
                        }
                        // If the current data does not create a complete
                        // message, remove the first byte and try again
                        else
                        {
                            lock (_bufferLock)
                            {
                                _incomingDataBuffer.RemoveAt(0);
                            }
                        }
                    }
                    else
                    {
                        // Length is too large to be correct
                        if (expectedLength > MAX_LENGTH || _timeout > TIMEOUT)
                        {
                            lock (_bufferLock)
                            {
                                if (_incomingDataBuffer.Count > 0)
                                {
                                    _incomingDataBuffer.RemoveAt(0);
                                }
                            }
                            _timeout = 0;
                            //return;

                            // Try to decode the data now
                            //DecodeIncomingData();
                        }

                        // Try to prevent expectedLength 
                        // from stalling the state
                        _timeout++;
                    }
                }
            }
            catch (Exception e)
            {
                // Some threading issues can cause the buffer
                // to be cleared while still trying to use between
                // checking the size and getting the data out of the
                // buffer
                log.Error("Error decoding PNI Prime messages", e);
            }
        }

        /// <summary>
        /// Determine what type of message was
        /// received.  This will get the 3 byte
        /// in the message and determine which
        /// type the message is.  It will then call
        /// the apporiate decode method for message 
        /// received.
        /// </summary>
        /// <param name="msg">Message to decode.</param>
        private void DecodeMessage(byte[] msg)
        {
            // Ensure a good message was recieved.
            if (msg.Length >= PACKET_MIN_SIZE)
            {

                int frameType = msg[2];

                //Debug.WriteLine("Found good Compass ID: {0}  msg length: {1}", frameType, msg.Length);

                switch (frameType)
                {
                    case (int)ID.kDataResp:
                        DecodekDataResp(msg);
                        //Debug.WriteLine("Compass Data Response");
                        break;
                    case (int)ID.kConfigResp:
                        DecodekConfigResp(msg);
                        //Debug.WriteLine("Compass Config Response");
                        break;
                    case (int)ID.kUserCalSampCount:
                        DecodekUserCalSampCount(msg);
                        //Debug.WriteLine("Compass Cal Sample Count");
                        break;
                    case (int)ID.kUserCalScore:
                        DecodekUserCalScore(msg);
                        //Debug.WriteLine("Compass Cal Score");
                        break;
                    case (int)ID.kFactoryUserCalDone:
                        PublishEvent(new CompassEventArgs(ID.kFactoryUserCalDone, null));
                        //Debug.WriteLine("Compass Mag Factory Cal Done");
                        break;
                    case (int)ID.kFactoryInclCalDone:
                        PublishEvent(new CompassEventArgs(ID.kFactoryInclCalDone, null));
                        //Debug.WriteLine("Compass Accel Factory Cal Done");
                        break;
                    case (int)ID.kSaveDone:
                        DecodekSaveDone(msg);
                        //Debug.WriteLine("Compass Save Done");
                        break;
                    default:
                        break;
                }
            }

        }

        /// <summary>
        /// ID 16
        /// This frame is the response to kSave frame.  The payload contains a UInt16 error
        /// code.  0000h indicates no error, 0001h indicates error when attempting to save data into
        /// non-volatile memory.
        /// </summary>
        /// <param name="msg">Data to decode.</param>
        private void DecodekSaveDone(byte[] msg)
        {
            // Results, 0 = no error
            UInt16 result = MathHelper.ByteArrayToUInt16(msg, FIRST_PACKET_INDEX, IS_BIG_ENDIAN);

            // Call all subscribers with the error value
            PublishEvent(new CompassEventArgs(ID.kSaveDone, result));
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
            // Start with index 3
            // This is the start of a 4 byte UInt32
            UInt32 sampleCount = MathHelper.ByteArrayToUInt32(msg, FIRST_PACKET_INDEX, IS_BIG_ENDIAN);

            // Call all subscribers with new sample count
            PublishEvent(new CompassEventArgs(ID.kUserCalSampCount, sampleCount));
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
            score.stdDevErr = MathHelper.ByteArrayToFloat(msg, index, IS_BIG_ENDIAN); ;
            index += SIZE_OF_FLOAT;

            // X Coverage
            score.xCoverage = MathHelper.ByteArrayToFloat(msg, index, IS_BIG_ENDIAN); ;
            index += SIZE_OF_FLOAT;

            // Y Coverage
            score.yCoverage = MathHelper.ByteArrayToFloat(msg, index, IS_BIG_ENDIAN); ;
            index += SIZE_OF_FLOAT;

            // Z Coverage
            score.zCoverage = MathHelper.ByteArrayToFloat(msg, index, IS_BIG_ENDIAN); ;
            index += SIZE_OF_FLOAT;

            // XYZ Accel Coverage
            float xyzAccelCoverage = MathHelper.ByteArrayToFloat(msg, index, IS_BIG_ENDIAN); ;
            index += SIZE_OF_FLOAT;

            // Accel StdDev Err
            score.accelStdDevErr = MathHelper.ByteArrayToFloat(msg, index, IS_BIG_ENDIAN); ;

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
            PublishEvent(new CompassEventArgs(ID.kUserCalScore, score));
        }

        /// <summary>
        /// ID 5
        /// Decode the Data Response.
        /// This is a message sent by the compass
        /// at a fixed interval time.  It contains
        /// the Heading, pitch, roll.
        /// Pass the data to all subscribers.
        /// </summary>
        /// <param name="msg">Data to decode</param>
        private void DecodekDataResp(byte[] msg)
        {
            int index = FIRST_PACKET_INDEX;
            int idCount = msg[index++];

            DataResponse dataResponse = new DataResponse();

            // Go through each ID in the response
            for (int count = 0; count < idCount; count++)
            {
                int id = msg[index++];

                // Set all the id ranges
                switch (id)
                {
                    case (int)ID.kHeading:
                        dataResponse.Heading = MathHelper.ByteArrayToFloat(msg, index, IS_BIG_ENDIAN);
                        index += SIZE_OF_FLOAT;
                        break;
                    case (int)ID.kPAngle:
                        dataResponse.Pitch = MathHelper.ByteArrayToFloat(msg, index, IS_BIG_ENDIAN);
                        index += SIZE_OF_FLOAT;
                        break;
                    case (int)ID.kRAngle:
                        dataResponse.Roll = MathHelper.ByteArrayToFloat(msg, index, IS_BIG_ENDIAN);
                        index += SIZE_OF_FLOAT;
                        break;
                    case (int)ID.kDistortion:
                        dataResponse.Distortion = MathHelper.ByteArrayToBoolean(msg, index);
                        index += SIZE_OF_BOOLEAN;
                        break;
                    case (int)ID.kCalStatus:
                        dataResponse.CalStatus = MathHelper.ByteArrayToBoolean(msg, index);
                        index += SIZE_OF_BOOLEAN;
                        break;
                    case (int)ID.kPAligned:
                        dataResponse.pAligned = MathHelper.ByteArrayToFloat(msg, index, IS_BIG_ENDIAN);
                        index += SIZE_OF_FLOAT;
                        break;
                    case (int)ID.kRAligned:
                        dataResponse.rAligned = MathHelper.ByteArrayToFloat(msg, index, IS_BIG_ENDIAN);
                        index += SIZE_OF_FLOAT;
                        break;
                    case (int)ID.kIZAligned:
                        dataResponse.izAligned = MathHelper.ByteArrayToFloat(msg, index, IS_BIG_ENDIAN);
                        index += SIZE_OF_FLOAT;
                        break;
                    case (int)ID.kXAligned:
                        dataResponse.xAligned = MathHelper.ByteArrayToFloat(msg, index, IS_BIG_ENDIAN);
                        index += SIZE_OF_FLOAT;
                        break;
                    case (int)ID.kYAligned:
                        dataResponse.yAligned = MathHelper.ByteArrayToFloat(msg, index, IS_BIG_ENDIAN);
                        index += SIZE_OF_FLOAT;
                        break;
                    case (int)ID.kZAligned:
                        dataResponse.zAligned = MathHelper.ByteArrayToFloat(msg, index, IS_BIG_ENDIAN);
                        index += SIZE_OF_FLOAT;
                        break;
                    default:
                        break;
                }
            }

            // Call all subscribers with new sample count
            PublishEvent(new CompassEventArgs(ID.kDataResp, dataResponse));
        }

        /// <summary>
        /// ID 8
        /// The frame queries is the response to kGetConfig frame.  The
        /// payload contains the configuration ID and value.
        /// </summary>
        private void DecodekConfigResp(byte[] msg)
        {
            int index = FIRST_PACKET_INDEX;

            Configuration config = new Configuration();

            int id = msg[index++];

            // Set all the id ranges
            switch (id)
            {
                case (int)ID.kDeclination:
                    config.Declination = MathHelper.ByteArrayToFloat(msg, index, IS_BIG_ENDIAN);;

                    // Call all subscribers with new sample count
                    PublishEvent(new CompassEventArgs(ID.kDeclination, config.Declination));
                    break;
                case (int)ID.kTrueNorth:
                    config.TrueNorth = MathHelper.ByteArrayToBoolean(msg, index);

                    // Call all subscribers with new values
                    PublishEvent(new CompassEventArgs(ID.kTrueNorth, config.TrueNorth));
                    break;
                case (int)ID.kBigEndian:
                    config.BigEndian = MathHelper.ByteArrayToBoolean(msg, index);

                    // Call all subscribers with new values
                    PublishEvent(new CompassEventArgs(ID.kBigEndian, config.BigEndian));
                    break;
                case (int)ID.kMountingRef:
                    config.MountingRef = MathHelper.ByteArrayToInt8(msg, index);

                    // Call all subscribers with new values
                    PublishEvent(new CompassEventArgs(ID.kMountingRef, config.MountingRef));
                    break;
                case (int)ID.kUserCalStableCheck:
                    config.UserCalStableCheck = MathHelper.ByteArrayToBoolean(msg, index);

                    // Call all subscribers with new values
                    PublishEvent(new CompassEventArgs(ID.kUserCalStableCheck, config.UserCalStableCheck));
                    break;
                case (int)ID.kUserCalNumPoints:
                    config.UserCalNumPoints = MathHelper.ByteArrayToUInt32(msg, index, IS_BIG_ENDIAN);

                    // Call all subscribers with new values
                    PublishEvent(new CompassEventArgs(ID.kUserCalNumPoints, config.UserCalNumPoints));
                    break;
                case (int)ID.kUserCalAutoSampling:
                    config.UserCalAutoSampling = MathHelper.ByteArrayToBoolean(msg, index);

                    // Call all subscribers with new values
                    PublishEvent(new CompassEventArgs(ID.kUserCalAutoSampling, config.UserCalAutoSampling));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Give the calibration positions for
        /// the next sample taken.
        /// </summary>
        /// <param name="sample">Position Sample.</param>
        /// <returns>String of next position for each sample given.</returns>
        public static string MagCalibrationPosition(UInt32 sample)
        {
            switch(sample)
            {
                case 0:
                    return "0°";
                case 1:
                    return "90°";
                case 2:
                    return "180°";
                case 3:
                    return "270°";
                case 4:
                    return "30° Roll: 20°";
                case 5:
                    return "120° Roll: 20°";
                case 6:
                    return "210° Roll: 20°";
                case 7:
                    return "300° Roll: 20°";
                case 8:
                    return "60° Roll: -20°";
                case 9:
                    return "150° Roll -20°";
                case 10:
                    return "240° Roll -20°";
                case 11:
                    return "330° Roll -20°";
                default:
                    return "-";
            }
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
        /// size and get the last 2 values.  It will combine
        /// them to form the checksum.
        /// </summary>
        /// <param name="size">Size of the message.</param>
        /// <returns>Checksum at the end of the message.</returns>
        private UInt16 GetMsgChecksum(int size)
        {
            if (size >= PACKET_MIN_SIZE)
            {
                // Get the last 2 values in the buffer
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

        #region Utilities

        /// <summary>
        /// Calculate the checksum for the given data.
        /// This will use CRC-16 to calculate the checksum.
        /// Do not include the checksum in the calculation.
        /// </summary>
        /// <param name="data">Byte array containing the data to get the checksum.</param>
        /// <returns>Checksum value.</returns>
        private static UInt16 CRC(byte[] data)
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

        #region Events

        /// <summary>
        /// Class for an event argument to send and receive
        /// data through events.
        /// </summary>
        public class CompassEventArgs : EventArgs
        {
            /// <summary>
            /// Property name that has changed.
            /// </summary>
            public ID EventType { get; internal set; }

            /// <summary>
            /// Value to hold for property changes.
            /// </summary>
            public object Value { get; internal set; }

            /// <summary>
            /// Object to store the event type and value.
            /// </summary>
            /// <param name="eventType">Event type given the ID enum.</param>
            /// <param name="data">Value to store.</param>
            public CompassEventArgs(ID eventType, object data)
            {
                this.EventType = eventType;
                this.Value = data;
            }
        }                                    

        /// <summary>
        /// Method to call when subscribing to an event.
        /// </summary>
        /// <param name="data"></param>
        public delegate void CompassEventHandler(CompassEventArgs data);

        /// <summary>
        /// Event to subscribe to
        /// </summary>
        public event CompassEventHandler CompassEvent;

        /// <summary>
        /// An event has been fired.  This method
        /// will call all subscribers with the event.
        /// </summary>
        /// <param name="data">The event to send.</param>
        public void PublishEvent(CompassEventArgs data)
        {
            // Check if there are any Subscribers
            if (CompassEvent != null)
            {
                // Call the Event
                CompassEvent(data);
            }
        }

        #endregion
    }
}