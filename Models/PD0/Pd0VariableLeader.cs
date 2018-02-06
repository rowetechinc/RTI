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
 * 03/03/2014      RC          2.21.4     Initial coding
 * 07/16/2014      RC          2.23.0     Check if the values are given in DecodeRtiEnsemble().
 * 07/24/2014      RC          2.23.0     Fixed ensemble number to start with 1 in DecodeRtiEnsemble().
 * 
 * 
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// PD0 Variable Leader
    /// </summary>
    public class Pd0VariableLeader : Pd0DataType
    {
        #region Variable

        /// <summary>
        /// Number of bytes required for the Data Type.
        /// </summary>
        public const int DATATYPE_SIZE = 65;

        /// <summary>
        /// LSB for the ID for the PD0 Variable Leader data type.
        /// </summary>
        public const byte ID_LSB = 0x80;

        /// <summary>
        /// MSB for the ID for the PD0 Variable Leader data type.
        /// </summary>
        public const byte ID_MSB = 0x00;

        #endregion

        #region Properties

        /// <summary>
        /// Ensemble number.
        /// 
        /// This field contains the sequential number of the ensemble to
        /// which the data in the output buffer apply.
        /// Scaling: LSD = 1 ensemble; Range = 1 to 65,535 ensembles
        /// NOTE: The first ensemble collected is #1. At “rollover,” we
        /// have the following sequence:
        /// 1 = ENSEMBLE NUMBER 1
        ///   ↓
        /// 65535 = ENSEMBLE NUMBER 65,535 | ENSEMBLE
        ///     0 = ENSEMBLE NUMBER 65,536 | #MSB FIELD
        ///     1 = ENSEMBLE NUMBER 65,537 | (BYTE 12)
        /// INCR.
        /// </summary>
        public ushort EnsembleNumber { get; set; }

        /// <summary>
        /// Real Time Clock Year.
        /// 
        /// These fields contain the time from the Workhorse’s real-time
        /// clock (RTC) that the current data ensemble began. The TScommand
        /// (Set Real-Time Clock) initially sets the clock. The
        /// Workhorse does account for leap years.
        /// </summary>
        public byte RtcYear { get; set; }

        /// <summary>
        /// Real Time Clock Month.
        /// 
        /// These fields contain the time from the Workhorse’s real-time
        /// clock (RTC) that the current data ensemble began. The TScommand
        /// (Set Real-Time Clock) initially sets the clock. The
        /// Workhorse does account for leap years.
        /// </summary>
        public byte RtcMonth { get; set; }

        /// <summary>
        /// Real Time Clock Day.
        /// 
        /// These fields contain the time from the Workhorse’s real-time
        /// clock (RTC) that the current data ensemble began. The TScommand
        /// (Set Real-Time Clock) initially sets the clock. The
        /// Workhorse does account for leap years.
        /// </summary>
        public byte RtcDay { get; set; }

        /// <summary>
        /// Real Time Clock Hour.
        /// 
        /// These fields contain the time from the Workhorse’s real-time
        /// clock (RTC) that the current data ensemble began. The TScommand
        /// (Set Real-Time Clock) initially sets the clock. The
        /// Workhorse does account for leap years.
        /// </summary>
        public byte RtcHour { get; set; }

        /// <summary>
        /// Real Time Clock Minute.
        /// 
        /// These fields contain the time from the Workhorse’s real-time
        /// clock (RTC) that the current data ensemble began. The TScommand
        /// (Set Real-Time Clock) initially sets the clock. The
        /// Workhorse does account for leap years.
        /// </summary>
        public byte RtcMinute { get; set; }

        /// <summary>
        /// Real Time Clock Second.
        /// </summary>
        public byte RtcSecond { get; set; }

        /// <summary>
        /// Real Time Clock Hundredths.
        /// 
        /// These fields contain the time from the Workhorse’s real-time
        /// clock (RTC) that the current data ensemble began. The TScommand
        /// (Set Real-Time Clock) initially sets the clock. The
        /// Workhorse does account for leap years.
        /// </summary>
        public byte RtcHundredths { get; set; }

        /// <summary>
        /// Ensemble Number Rollerover.
        /// 
        /// This field increments each time the Ensemble Number field
        /// (bytes 3,4) “rolls over.” This allows ensembles up to
        /// 16,777,215. See Ensemble Number field above.
        /// </summary>
        public byte EnsembleNumRollover { get; set; }

        /// <summary>
        /// Bit Result.
        /// 
        /// This field contains the results of the Workhorse’s Built-in Test
        /// function. A zero code indicates a successful BIT result.
        /// BYTE 13  BYTE 14 (BYTE 14 RESERVED FOR FUTURE USE)
        /// 1xxxxxxx xxxxxxxx = RESERVED
        /// x1xxxxxx xxxxxxxx = RESERVED
        /// xx1xxxxx xxxxxxxx = RESERVED
        /// xxx1xxxx xxxxxxxx = DEMOD 1 ERROR
        /// xxxx1xxx xxxxxxxx = DEMOD 0 ERROR
        /// xxxxx1xx xxxxxxxx = RESERVED
        /// xxxxxx1x xxxxxxxx = TIMING CARD ERROR
        /// xxxxxxx1 xxxxxxxx = RESERVED
        /// </summary>
        public short BitResult { get; set; }

        /// <summary>
        /// Speed of Sound.
        /// 
        /// Contains either manual or calculated speed of sound information
        /// (EC-command).
        /// Scaling: LSD = 1 meter per second; Range = 1400 to 1600m/s
        /// </summary>
        public short SpeedOfSound { get; set; }

        /// <summary>
        /// Depth of Transducer.
        /// 
        /// Contains the depth of the transducer below the water surface
        /// (ED-command). This value may be a manual setting or a
        /// reading from a depth sensor.
        /// Scaling: LSD = 1 decimeter; Range = 1 to 9999 decimeters
        /// </summary>
        public short DepthOfTransducer { get; set; }

        /// <summary>
        /// Heading.
        /// 
        /// Contains the Workhorse heading angle (EH-command). This
        /// value may be a manual setting or a reading from a heading
        /// sensor.
        /// Scaling: LSD = 0.01 degree; Range = 000.00 to 359.99 degrees
        /// </summary>
        public float Heading { get; set; }

        /// <summary>
        /// Pitch.
        /// 
        /// Contains the Workhorse pitch angle (EP-command). This
        /// value may be a manual setting or a reading from a tilt sensor.
        /// Positive values mean that Beam #3 is spatially higher than
        /// Beam #4.
        /// Scaling: LSD = 0.01 degree; Range = -20.00 to +20.00 degrees
        /// </summary>
        public float Pitch { get; set; }

        /// <summary>
        /// Roll.
        /// 
        /// Contains the Workhorse roll angle (ER-command). This
        /// value may be a manual setting or a reading from a tilt sensor.
        /// For up-facing Workhorses, positive values mean that Beam
        /// #2 is spatially higher than Beam #1. For down-facing Workhorses,
        /// positive values mean that Beam #1 is spatially higher
        /// than Beam #2.
        /// Scaling: LSD = 0.01 degree; Range = -20.00 to +20.00 degrees
        /// </summary>
        public float Roll { get; set; }

        /// <summary>
        /// Salinity.
        /// 
        /// Contains the salinity value of the water at the transducer
        /// head (ES-command). This value may be a manual setting or
        /// a reading from a conductivity sensor.
        /// Scaling: LSD = 1 part per thousand; Range = 0 to 40 ppt
        /// </summary>
        public short Salinity { get; set; }

        /// <summary>
        /// Temperature.
        /// 
        /// Contains the temperature of the water at the transducer head
        /// (ET-command). This value may be a manual setting or a
        /// reading from a temperature sensor.
        /// Scaling: LSD = 0.01 degree; Range = -5.00 to +40.00 degrees
        /// </summary>
        public float Temperature { get; set; }

        /// <summary>
        /// Minimum Pre-Ping Wait Time between ping groups in the ensemble.
        /// Minutes.
        /// 
        /// This field contains the Minimum Pre-Ping Wait Time between
        /// ping groups in the ensemble.
        /// </summary>
        public byte MinPrePingWaitTimeMinutes { get; set; }

        /// <summary>
        /// Minimum Pre-Ping Wait Time between ping groups in the ensemble.
        /// Seconds.
        /// 
        /// This field contains the Minimum Pre-Ping Wait Time between
        /// ping groups in the ensemble.
        /// </summary>
        public byte MinPrePingWaitTimeSeconds { get; set; }

        /// <summary>
        /// Minimum Pre-Ping Wait Time between ping groups in the ensemble.
        /// Hundredths of a Second.
        /// 
        /// This field contains the Minimum Pre-Ping Wait Time between
        /// ping groups in the ensemble.
        /// </summary>
        public byte MinPrePingWaitTimeHundredths { get; set; }

        /// <summary>
        /// Heading Standard Deviation.
        /// 
        /// These fields contain the standard deviation (accuracy) of the
        /// heading and tilt angles from the gyrocompass/pendulums.
        /// Scaling (Heading): LSD = 1°; Range = 0 to 180° Scaling
        /// (Tilts): LSD = 0.1°; Range = 0.0 to 20.0°
        /// </summary>
        public byte HeadingStdDev { get; set; }

        /// <summary>
        /// Pitch Standard Deviation.
        /// 
        /// These fields contain the standard deviation (accuracy) of the
        /// heading and tilt angles from the gyrocompass/pendulums.
        /// Scaling (Heading): LSD = 1°; Range = 0 to 180° Scaling
        /// (Tilts): LSD = 0.1°; Range = 0.0 to 20.0°
        /// </summary>
        public float PitchStdDev { get; set; }

        /// <summary>
        /// Roll Standard Deviation.
        /// 
        /// These fields contain the standard deviation (accuracy) of the
        /// heading and tilt angles from the gyrocompass/pendulums.
        /// Scaling (Heading): LSD = 1°; Range = 0 to 180° Scaling
        /// (Tilts): LSD = 0.1°; Range = 0.0 to 20.0°
        /// </summary>
        public float RollStdDev { get; set; }

        /// <summary>
        /// ADCP Channel 0.
        /// 
        /// These fields contain the outputs of the Analog-to-Digital Converter
        /// (ADC) located on the DSP board. The ADC sequentially
        /// samples one of the eight channels per ping group (the
        /// number of ping groups per ensemble is the maximum of the
        /// WP). These fields are zeroed at the beginning of the deployment
        /// and updated each ensemble at the rate of one
        /// channel per ping group. For example, if the ping group size
        /// is 5, then:
        /// END OF ENSEMBLE No.            CHANNELS UPDATED
        ///      Start                     All channels = 0
        ///       1                          0, 1, 2, 3, 4
        ///       2                          5, 6, 7, 0, 1
        ///       3                          2, 3, 4, 5, 6
        ///       4                          7, 0, 8, 2, 3
        ///       ↓                                ↓
        /// Here is the description for each channel:
        /// CHANNEL      DESCRIPTION
        ///   0          XMIT CURRENT
        ///   1          XMIT VOLTAGE
        ///   2          AMBIENT TEMP
        ///   3          PRESSURE (+)
        ///   4          PRESSURE (-)
        ///   5          ATTITUDE TEMP
        ///   6          ATTITUDE
        ///   7          CONTAMINATION SENSOR
        /// Note that the ADC values may be “noisy” from sample-tosample,
        /// but are useful for detecting long-term trends.
        /// </summary>
        public byte Adc0 { get; set; }

        /// <summary>
        /// ADCP Channel 1.
        /// 
        /// These fields contain the outputs of the Analog-to-Digital Converter
        /// (ADC) located on the DSP board. The ADC sequentially
        /// samples one of the eight channels per ping group (the
        /// number of ping groups per ensemble is the maximum of the
        /// WP). These fields are zeroed at the beginning of the deployment
        /// and updated each ensemble at the rate of one
        /// channel per ping group. For example, if the ping group size
        /// is 5, then:
        /// END OF ENSEMBLE No.            CHANNELS UPDATED
        ///      Start                     All channels = 0
        ///       1                          0, 1, 2, 3, 4
        ///       2                          5, 6, 7, 0, 1
        ///       3                          2, 3, 4, 5, 6
        ///       4                          7, 0, 8, 2, 3
        ///       ↓                                ↓
        /// Here is the description for each channel:
        /// CHANNEL      DESCRIPTION
        ///   0          XMIT CURRENT
        ///   1          XMIT VOLTAGE
        ///   2          AMBIENT TEMP
        ///   3          PRESSURE (+)
        ///   4          PRESSURE (-)
        ///   5          ATTITUDE TEMP
        ///   6          ATTITUDE
        ///   7          CONTAMINATION SENSOR
        /// Note that the ADC values may be “noisy” from sample-tosample,
        /// but are useful for detecting long-term trends.
        /// </summary>
        public byte Adc1 { get; set; }

        /// <summary>
        /// ADCP Channel 2.
        /// 
        /// These fields contain the outputs of the Analog-to-Digital Converter
        /// (ADC) located on the DSP board. The ADC sequentially
        /// samples one of the eight channels per ping group (the
        /// number of ping groups per ensemble is the maximum of the
        /// WP). These fields are zeroed at the beginning of the deployment
        /// and updated each ensemble at the rate of one
        /// channel per ping group. For example, if the ping group size
        /// is 5, then:
        /// END OF ENSEMBLE No.            CHANNELS UPDATED
        ///      Start                     All channels = 0
        ///       1                          0, 1, 2, 3, 4
        ///       2                          5, 6, 7, 0, 1
        ///       3                          2, 3, 4, 5, 6
        ///       4                          7, 0, 8, 2, 3
        ///       ↓                                ↓
        /// Here is the description for each channel:
        /// CHANNEL      DESCRIPTION
        ///   0          XMIT CURRENT
        ///   1          XMIT VOLTAGE
        ///   2          AMBIENT TEMP
        ///   3          PRESSURE (+)
        ///   4          PRESSURE (-)
        ///   5          ATTITUDE TEMP
        ///   6          ATTITUDE
        ///   7          CONTAMINATION SENSOR
        /// Note that the ADC values may be “noisy” from sample-tosample,
        /// but are useful for detecting long-term trends.
        /// </summary>
        public byte Adc2 { get; set; }

        /// <summary>
        /// ADCP Channel 3.
        /// 
        /// These fields contain the outputs of the Analog-to-Digital Converter
        /// (ADC) located on the DSP board. The ADC sequentially
        /// samples one of the eight channels per ping group (the
        /// number of ping groups per ensemble is the maximum of the
        /// WP). These fields are zeroed at the beginning of the deployment
        /// and updated each ensemble at the rate of one
        /// channel per ping group. For example, if the ping group size
        /// is 5, then:
        /// END OF ENSEMBLE No.            CHANNELS UPDATED
        ///      Start                     All channels = 0
        ///       1                          0, 1, 2, 3, 4
        ///       2                          5, 6, 7, 0, 1
        ///       3                          2, 3, 4, 5, 6
        ///       4                          7, 0, 8, 2, 3
        ///       ↓                                ↓
        /// Here is the description for each channel:
        /// CHANNEL      DESCRIPTION
        ///   0          XMIT CURRENT
        ///   1          XMIT VOLTAGE
        ///   2          AMBIENT TEMP
        ///   3          PRESSURE (+)
        ///   4          PRESSURE (-)
        ///   5          ATTITUDE TEMP
        ///   6          ATTITUDE
        ///   7          CONTAMINATION SENSOR
        /// Note that the ADC values may be “noisy” from sample-tosample,
        /// but are useful for detecting long-term trends.
        /// </summary>
        public byte Adc3 { get; set; }

        /// <summary>
        /// ADCP Channel 4.
        /// 
        /// These fields contain the outputs of the Analog-to-Digital Converter
        /// (ADC) located on the DSP board. The ADC sequentially
        /// samples one of the eight channels per ping group (the
        /// number of ping groups per ensemble is the maximum of the
        /// WP). These fields are zeroed at the beginning of the deployment
        /// and updated each ensemble at the rate of one
        /// channel per ping group. For example, if the ping group size
        /// is 5, then:
        /// END OF ENSEMBLE No.            CHANNELS UPDATED
        ///      Start                     All channels = 0
        ///       1                          0, 1, 2, 3, 4
        ///       2                          5, 6, 7, 0, 1
        ///       3                          2, 3, 4, 5, 6
        ///       4                          7, 0, 8, 2, 3
        ///       ↓                                ↓
        /// Here is the description for each channel:
        /// CHANNEL      DESCRIPTION
        ///   0          XMIT CURRENT
        ///   1          XMIT VOLTAGE
        ///   2          AMBIENT TEMP
        ///   3          PRESSURE (+)
        ///   4          PRESSURE (-)
        ///   5          ATTITUDE TEMP
        ///   6          ATTITUDE
        ///   7          CONTAMINATION SENSOR
        /// Note that the ADC values may be “noisy” from sample-tosample,
        /// but are useful for detecting long-term trends.
        /// </summary>
        public byte Adc4 { get; set; }


        /// <summary>
        /// ADCP Channel 5.
        /// 
        /// These fields contain the outputs of the Analog-to-Digital Converter
        /// (ADC) located on the DSP board. The ADC sequentially
        /// samples one of the eight channels per ping group (the
        /// number of ping groups per ensemble is the maximum of the
        /// WP). These fields are zeroed at the beginning of the deployment
        /// and updated each ensemble at the rate of one
        /// channel per ping group. For example, if the ping group size
        /// is 5, then:
        /// END OF ENSEMBLE No.            CHANNELS UPDATED
        ///      Start                     All channels = 0
        ///       1                          0, 1, 2, 3, 4
        ///       2                          5, 6, 7, 0, 1
        ///       3                          2, 3, 4, 5, 6
        ///       4                          7, 0, 8, 2, 3
        ///       ↓                                ↓
        /// Here is the description for each channel:
        /// CHANNEL      DESCRIPTION
        ///   0          XMIT CURRENT
        ///   1          XMIT VOLTAGE
        ///   2          AMBIENT TEMP
        ///   3          PRESSURE (+)
        ///   4          PRESSURE (-)
        ///   5          ATTITUDE TEMP
        ///   6          ATTITUDE
        ///   7          CONTAMINATION SENSOR
        /// Note that the ADC values may be “noisy” from sample-tosample,
        /// but are useful for detecting long-term trends.
        /// </summary>
        public byte Adc5 { get; set; }

        /// <summary>
        /// ADCP Channel 6.
        /// 
        /// These fields contain the outputs of the Analog-to-Digital Converter
        /// (ADC) located on the DSP board. The ADC sequentially
        /// samples one of the eight channels per ping group (the
        /// number of ping groups per ensemble is the maximum of the
        /// WP). These fields are zeroed at the beginning of the deployment
        /// and updated each ensemble at the rate of one
        /// channel per ping group. For example, if the ping group size
        /// is 5, then:
        /// END OF ENSEMBLE No.            CHANNELS UPDATED
        ///      Start                     All channels = 0
        ///       1                          0, 1, 2, 3, 4
        ///       2                          5, 6, 7, 0, 1
        ///       3                          2, 3, 4, 5, 6
        ///       4                          7, 0, 8, 2, 3
        ///       ↓                                ↓
        /// Here is the description for each channel:
        /// CHANNEL      DESCRIPTION
        ///   0          XMIT CURRENT
        ///   1          XMIT VOLTAGE
        ///   2          AMBIENT TEMP
        ///   3          PRESSURE (+)
        ///   4          PRESSURE (-)
        ///   5          ATTITUDE TEMP
        ///   6          ATTITUDE
        ///   7          CONTAMINATION SENSOR
        /// Note that the ADC values may be “noisy” from sample-tosample,
        /// but are useful for detecting long-term trends.
        /// </summary>
        public byte Adc6 { get; set; }

        /// <summary>
        /// ADCP Channel 7.
        /// 
        /// These fields contain the outputs of the Analog-to-Digital Converter
        /// (ADC) located on the DSP board. The ADC sequentially
        /// samples one of the eight channels per ping group (the
        /// number of ping groups per ensemble is the maximum of the
        /// WP). These fields are zeroed at the beginning of the deployment
        /// and updated each ensemble at the rate of one
        /// channel per ping group. For example, if the ping group size
        /// is 5, then:
        /// END OF ENSEMBLE No.            CHANNELS UPDATED
        ///      Start                     All channels = 0
        ///       1                          0, 1, 2, 3, 4
        ///       2                          5, 6, 7, 0, 1
        ///       3                          2, 3, 4, 5, 6
        ///       4                          7, 0, 8, 2, 3
        ///       ↓                                ↓
        /// Here is the description for each channel:
        /// CHANNEL      DESCRIPTION
        ///   0          XMIT CURRENT
        ///   1          XMIT VOLTAGE
        ///   2          AMBIENT TEMP
        ///   3          PRESSURE (+)
        ///   4          PRESSURE (-)
        ///   5          ATTITUDE TEMP
        ///   6          ATTITUDE
        ///   7          CONTAMINATION SENSOR
        /// Note that the ADC values may be “noisy” from sample-tosample,
        /// but are useful for detecting long-term trends.
        /// </summary>
        public byte Adc7 { get; set; }

        /// <summary>
        /// Error Status Word.
        /// 
        /// Contains the long word containing the bit flags for the CY?
        /// Command. The ESW is cleared (set to zero) between each
        /// ensemble.
        /// Note that each number above represents one bit set – they
        /// may occur in combinations. For example, if the long word
        /// value is 0000C000 (hexadecimal), then it indicates that both
        /// a cold wake-up (0004000) and an unknown wake-up
        /// (00008000) occurred.
        /// Low 16 BITS
        /// LSB
        /// BITS 07 06 05 04 03 02 01 00
        ///      x  x  x  x  x  x  x  1    Bus Error exception
        ///      x  x  x  x  x  x  1  x    Address Error exception
        ///      x  x  x  x  x  1  x  x    Illegal Instruction exception
        ///      x  x  x  x  1  x  x  x    Zero Divide exception
        ///      x  x  x  1  x  x  x  x    Emulator exception
        ///      x  x  1  x  x  x  x  x    Unassigned exception
        ///      x  1  x  x  x  x  x  x    Watchdog restart occurred
        ///      1  x  x  x  x  x  x  x    Battery Saver power
        /// </summary>
        public byte Esw0 { get; set; }

        /// <summary>
        /// Error Status Word
        /// 
        /// MSB
        /// BITS 15 14 13 12 11 10 09 08
        ///      x  x  x  x  x  x  x  1    Pinging
        ///      x  x  x  x  x  x  1  x    Not Used
        ///      x  x  x  x  x  1  x  x    Not Used
        ///      x  x  x  x  1  x  x  x    Not Used
        ///      x  x  x  1  x  x  x  x    Not Used
        ///      x  x  1  x  x  x  x  x    Not Used
        ///      x  1  x  x  x  x  x  x    Cold Wakeup occurred
        ///      1  x  x  x  x  x  x  x    Unknown Wakeup occurred
        /// High 16 BITS
        /// 
        /// </summary>
        public byte Esw1 { get; set; }

        /// <summary>
        /// Error Status Word
        /// 
        /// LSB
        /// BITS 24 23 22 21 20 19 18 17
        ///      x  x  x  x  x  x  x  1    Clock Read error occurred
        ///      x  x  x  x  x  x  1  x    Unexpected alarm
        ///      x  x  x  x  x  1  x  x    Clock jump forward
        ///      x  x  x  x  1  x  x  x    Clock jump backward
        ///      x  x  x  1  x  x  x  x    Not Used
        ///      x  x  1  x  x  x  x  x    Not Used
        ///      x  1  x  x  x  x  x  x    Not Used
        ///      1  x  x  x  x  x  x  x    Not Used
        /// 
        /// </summary>
        public byte Esw2 { get; set; }

        /// <summary>
        /// Error Status Word.
        /// 
        /// High 16 BITS
        /// MSB
        /// BITS 32 31 30 29 28 27 26 25
        ///      x  x  x  x  x  x  x  1    Not Used
        ///      x  x  x  x  x  x  1  x    Not Used
        ///      x  x  x  x  x  1  x  x    Not Used
        ///      x  x  x  x  1  x  x  x    Power Fail (Unrecorded)
        ///      x  x  x  1  x  x  x  x    Spurious level 4 intr (DSP)
        ///      x  x  1  x  x  x  x  x    Spurious level 5 intr (UART)
        ///      x  1  x  x  x  x  x  x    Spurious level 6 intr (CLOCK)
        ///      1  x  x  x  x  x  x  x    Level 7 interrupt occurred
        /// 
        /// </summary>
        public byte Esw3 { get; set; }

        /// <summary>
        /// Reserved Bytes 47-48.
        /// </summary>
        public short Reserved { get; set; }

        /// <summary>
        /// Pressrue.
        /// 
        /// Contains the pressure of the water at the transducer head
        /// relative to one atmosphere (sea level). Output is in decapascals
        /// (see “How Does the WorkHorse Sample Depth and
        /// Pressure?,” page 137).
        /// Scaling: LSD=1 deca-pascal; Range=0 to 4,294,967,295
        /// deca-pascals
        /// 
        /// </summary>
        public int Pressure { get; set; }

        /// <summary>
        /// Pressure Variance.
        /// 
        /// Contains the variance (deviation about the mean) of the
        /// pressure sensor data. Output is in deca-pascals.
        /// Scaling: LSD=1 deca-pascal; Range=0 to 4,294,967,295
        /// deca-pascals
        /// </summary>
        public int PressureVariance { get; set; }

        /// <summary>
        /// Spare.
        /// </summary>
        public byte Spare { get; set; }

        /// <summary>
        /// RTC Y2K Century.
        /// 
        /// These fields contain the time from the Workhorse’s Y2K
        /// compliant real-time clock (RTC) that the current data ensemble
        /// began. The TT-command (Set Real-Time Clock) initially
        /// sets the clock. The Workhorse does account for leap years.
        /// </summary>
        public byte RtcY2kCentury { get; set; }

        /// <summary>
        /// RTC Y2K Year.
        /// 
        /// These fields contain the time from the Workhorse’s Y2K
        /// compliant real-time clock (RTC) that the current data ensemble
        /// began. The TT-command (Set Real-Time Clock) initially
        /// sets the clock. The Workhorse does account for leap years.
        /// </summary>
        public byte RtcY2kYear { get; set; }

        /// <summary>
        /// RTC Y2K Month.
        /// 
        /// These fields contain the time from the Workhorse’s Y2K
        /// compliant real-time clock (RTC) that the current data ensemble
        /// began. The TT-command (Set Real-Time Clock) initially
        /// sets the clock. The Workhorse does account for leap years.
        /// </summary>
        public byte RtcY2kMonth { get; set; }

        /// <summary>
        /// RTC Y2K Day.
        /// 
        /// These fields contain the time from the Workhorse’s Y2K
        /// compliant real-time clock (RTC) that the current data ensemble
        /// began. The TT-command (Set Real-Time Clock) initially
        /// sets the clock. The Workhorse does account for leap years.
        /// </summary>
        public byte RtcY2kDay { get; set; }

        /// <summary>
        /// RTC Y2K Hour.
        /// 
        /// These fields contain the time from the Workhorse’s Y2K
        /// compliant real-time clock (RTC) that the current data ensemble
        /// began. The TT-command (Set Real-Time Clock) initially
        /// sets the clock. The Workhorse does account for leap years.
        /// </summary>
        public byte RtcY2kHour { get; set; }

        /// <summary>
        /// RTC Y2K Minute.
        /// 
        /// These fields contain the time from the Workhorse’s Y2K
        /// compliant real-time clock (RTC) that the current data ensemble
        /// began. The TT-command (Set Real-Time Clock) initially
        /// sets the clock. The Workhorse does account for leap years.
        /// </summary>
        public byte RtcY2kMinute { get; set; }

        /// <summary>
        /// RTC Y2K Seconds.
        /// 
        /// These fields contain the time from the Workhorse’s Y2K
        /// compliant real-time clock (RTC) that the current data ensemble
        /// began. The TT-command (Set Real-Time Clock) initially
        /// sets the clock. The Workhorse does account for leap years.
        /// </summary>
        public byte RtcY2kSecond { get; set; }

        /// <summary>
        /// RTC Y2K Hundredths of Second.
        /// 
        /// These fields contain the time from the Workhorse’s Y2K
        /// compliant real-time clock (RTC) that the current data ensemble
        /// began. The TT-command (Set Real-Time Clock) initially
        /// sets the clock. The Workhorse does account for leap years.
        /// </summary>
        public byte RtcY2kHundredth { get; set; }

        #endregion

        /// <summary>
        /// Initialize the data type.
        /// </summary>
        public Pd0VariableLeader()
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.VariableLeader)
        {
            Initialize();
        }

        /// <summary>
        /// Initialize the data type.
        /// </summary>
        /// <param name="data">Binary data for data type.</param>
        public Pd0VariableLeader(byte[] data)
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.VariableLeader)
        {
            Initialize();
            Decode(data);
        }

        /// <summary>
        /// Initialize the data type.
        /// </summary>
        /// <param name="data">Binary data for data type.</param>
        /// <param name="offset">Offset in the ensemble.</param>
        public Pd0VariableLeader(byte[] data, ushort offset)
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.VariableLeader)
        {
            this.Offset = offset;
            Initialize();
            Decode(data);
        }

        /// <summary>
        /// Initialize the data type.
        /// Take the RTI data sets to 
        /// initialize the object.
        /// </summary>
        /// <param name="ens">RTI Ensemble aata set.</param>
        /// <param name="anc">RTI Ancillary data set.</param>
        public Pd0VariableLeader(DataSet.EnsembleDataSet ens, DataSet.AncillaryDataSet anc)
            : base(ID_LSB, ID_MSB, Pd0ID.Pd0Types.VariableLeader)
        {
            Initialize();
            DecodeRtiEnsemble(ens, anc);
        }

        #region Initialize

        /// <summary>
        /// Initialize the object.
        /// </summary>
        public void Initialize()
        {
            EnsembleNumber = 0;                                                     // Ensemble Number
            RtcYear = (byte)(DateTime.Now.Year-2000);                               // RTC Year
            RtcMonth = (byte)DateTime.Now.Month;                                    // RTC Month
            RtcDay = (byte)DateTime.Now.Day;                                        // RTC Day
            RtcHour = (byte)DateTime.Now.Hour;                                      // RTC Hour
            RtcMinute = (byte)DateTime.Now.Minute;                                  // RTC Minute
            RtcSecond = (byte)DateTime.Now.Second;                                  // RTC Second
            RtcHundredths = (byte)(DateTime.Now.Millisecond * 100);                 // RTC Hundredths of Second
            EnsembleNumRollover = 0;                                                // Ensemble Rollerover
            BitResult = 0;                                                          // BIT Result
            SpeedOfSound = 0;                                                       // Speed Of Sound
            DepthOfTransducer = 0;                                                  // Depth of Transducer
            Heading = 0.0f;                                                         // Heading
            Pitch = 0.0f;                                                           // Pitch
            Roll = 0.0f;                                                            // Roll
            Salinity = 0;                                                           // Salinity
            Temperature = 0.0f;                                                     // Temperature
            MinPrePingWaitTimeMinutes = 0;                                          // Minimum Pre-Ping Wait Time Minutes
            MinPrePingWaitTimeSeconds = 0;                                          // Minimum Pre-Ping Wait Time Second;
            MinPrePingWaitTimeHundredths = 0;                                       // Minimum Pre-Ping Wait Time Hundredths of Second
            HeadingStdDev = 0;                                                      // Heading Standard Deviation
            PitchStdDev = 0.0f;                                                     // Pitch Standard Deviation
            RollStdDev = 0.0f;                                                      // Roll Standard Deviation
            Adc0 = 0;                                                               // ADC Channel 0
            Adc1 = 0;                                                               // ADC Channel 1
            Adc2 = 0;                                                               // ADC Channel 2
            Adc3 = 0;                                                               // ADC Channel 3
            Adc4 = 0;                                                               // ADC Channel 4
            Adc5 = 0;                                                               // ADC Channel 5
            Adc6 = 0;                                                               // ADC Channel 6
            Adc7 = 0;                                                               // ADC Channel 7
            Esw0 = 0;                                                               // Error Status Word 43
            Esw1 = 0;                                                               // Error Status Word 44
            Esw2 = 0;                                                               // Error Status Word 45
            Esw3 = 0;                                                               // Error Status Word 46
            Reserved = 0;                                                           // Reserved
            Pressure = 0;                                                           // Pressure
            PressureVariance = 0;                                                   // Pressure Variance
            Spare = 0;                                                              // Spare
            RtcY2kCentury = 0;                                                      // RTC Y2K Century
            RtcY2kYear = (byte)(DateTime.Now.Year - 2000);                          // RTC Y2K Year
            RtcY2kMonth = (byte)DateTime.Now.Month;                                 // RTC Y2K Month
            RtcY2kDay = (byte)DateTime.Now.Day;                                     // RTC Y2K Day
            RtcY2kHour = (byte)DateTime.Now.Hour;                                   // RTC Y2K Hour
            RtcY2kMinute = (byte)DateTime.Now.Minute;                               // RTC Y2K Minute
            RtcY2kSecond = (byte)DateTime.Now.Second;                               // RTC Y2K Second
            RtcY2kHundredth = (byte)(DateTime.Now.Millisecond * 100);               // RTC Y2K Hundredth of Second
        }

        #endregion

        #region Encode

        /// <summary>
        /// Encode the data to binary PD0.
        /// </summary>
        /// <returns>Binary format of the data.</returns>
        public override byte[] Encode()
        {
            // Ensemble Number
            byte ensNumLsb;
            byte ensNumMsb;
            MathHelper.LsbMsbUShort(EnsembleNumber, out ensNumLsb, out ensNumMsb);

            // Bit Result
            byte bitResultLsb;
            byte bitResultMsb;
            MathHelper.LsbMsbShort(BitResult, out bitResultLsb, out bitResultMsb);

            // Speed of Sound
            byte sosLsb;
            byte sosMsb;
            MathHelper.LsbMsbShort(SpeedOfSound, out sosLsb, out sosMsb);

            // Depth of Transducer
            byte dotLsb;
            byte dotMsb;
            MathHelper.LsbMsbShort(DepthOfTransducer, out dotLsb, out dotMsb);

            // Heading
            byte headingLsb;
            byte headingMsb;
            ushort headingVal = (ushort)Math.Round(Heading / 0.01);                                          // Value stored with LSD = 0.01
            MathHelper.LsbMsbUShort(headingVal, out headingLsb, out headingMsb);

            // Pitch
            byte pitchLsb;
            byte pitchMsb;
            short pitchVal = (short)Math.Round(Pitch / 0.01);                                               // Value stored with LSD = 0.01
            MathHelper.LsbMsbShort(pitchVal, out pitchLsb, out pitchMsb);

            // Roll
            byte rollLsb;
            byte rollMsb;
            short rollVal = (short)Math.Round(Roll / 0.01);                                                 // Value stored with LSD = 0.01
            MathHelper.LsbMsbShort(rollVal, out rollLsb, out rollMsb);

            // Salinity
            byte salinityLsb;
            byte salinityMsb;
            MathHelper.LsbMsbShort(Salinity, out salinityLsb, out salinityMsb);

            // Temperature
            byte temperatureLsb;
            byte temperatureMsb;
            short temperatureVal = (short)Math.Round(Temperature / 0.01);                                               // Value stored with LSD = 0.01
            MathHelper.LsbMsbShort(temperatureVal, out temperatureLsb, out temperatureMsb);

            // Reserved
            byte reservedLsb;
            byte reservedMsb;
            MathHelper.LsbMsbShort(Reserved, out reservedLsb, out reservedMsb);

            // Pressure
            byte[] pressureBA = BitConverter.GetBytes(Pressure);
            if (pressureBA.Length < 4)
            {
                pressureBA = new byte[4];
            }

            // Pressure Variance
            byte[] pressureVarianceBA = BitConverter.GetBytes(PressureVariance);
            if (pressureVarianceBA.Length < 4)
            {
                pressureVarianceBA = new byte[4];
            }

            byte[] data = new byte[DATATYPE_SIZE];

            data[0] = ID_LSB;                                   // Fixed Leader ID LSB 0x00
            data[1] = ID_MSB;                                   // Fixed Leader ID MSB 0x80
            data[2] = ensNumLsb;                                // Ensemble Number LSB
            data[3] = ensNumMsb;                                // Ensemble Number MSB
            data[4] = RtcYear;                                  // RTC Year
            data[5] = RtcMonth;                                 // RTC Month
            data[6] = RtcDay;                                   // RTC Day
            data[7] = RtcHour;                                  // RTC Hour
            data[8] = RtcMinute;                                // RTC Minute
            data[9] = RtcSecond;                                // RTC Second
            data[10] = RtcHundredths;                           // RTC Hundredth
            data[11] = EnsembleNumRollover;                     // Ensemble Rollover
            data[12] = bitResultLsb;                            // BIT Result LSB
            data[13] = bitResultMsb;                            // BIT Result MSB
            data[14] = sosLsb;                                  // Speed of Sound LSB
            data[15] = sosMsb;                                  // Speed of Sound MSB
            data[16] = dotLsb;                                  // Depth of Transducer LSB
            data[17] = dotMsb;                                  // Depth of Transducer MSB
            data[18] = headingLsb;                              // Heading LSB
            data[19] = headingMsb;                              // Heading MSB
            data[20] = pitchLsb;                                // Pitch LSB
            data[21] = pitchMsb;                                // Pitch MSB
            data[22] = rollLsb;                                 // Roll LSB
            data[23] = rollMsb;                                 // Roll MSB
            data[24] = salinityLsb;                             // Salinity LSB
            data[25] = salinityMsb;                             // Salinity MSB
            data[26] = temperatureLsb;                          // Temperature LSB
            data[27] = temperatureMsb;                          // Temperature MSB
            data[28] = MinPrePingWaitTimeMinutes;               // MPT Minutes
            data[29] = MinPrePingWaitTimeSeconds;               // MPT Seconds
            data[30] = MinPrePingWaitTimeHundredths;            // MPT Hundredths of Second
            data[31] = HeadingStdDev;                           // Heading Standard Deviation
            data[32] = (byte)Math.Round(PitchStdDev / 0.1f);    // Pitch Standard Deviation
            data[33] = (byte)Math.Round(RollStdDev / 0.1f);     // Roll Standard Deviation
            data[34] = Adc0;                                    // ADC Channel 0
            data[35] = Adc1;                                    // ADC Channel 1
            data[36] = Adc2;                                    // ADC Channel 2
            data[37] = Adc3;                                    // ADC Channel 3
            data[38] = Adc4;                                    // ADC Channel 4
            data[39] = Adc5;                                    // ADC Channel 5
            data[40] = Adc6;                                    // ADC Channel 6
            data[41] = Adc7;                                    // ADC Channel 7
            data[42] = Esw0;                                    // Error Status Word 43
            data[43] = Esw1;                                    // Error Status Word 44
            data[44] = Esw2;                                    // Error Status Word 45
            data[45] = Esw3;                                    // Error Status Word 46
            data[46] = reservedLsb;                             // Reserved LSB
            data[47] = reservedMsb;                             // Reserved MSB
            data[48] = pressureBA[0];                           // Pressure LSB
            data[49] = pressureBA[1];                           // Pressure 
            data[50] = pressureBA[2];                           // Pressure
            data[51] = pressureBA[3];                           // Pressure MSB
            data[52] = pressureVarianceBA[0];                   // Pressure Variance LSB
            data[53] = pressureVarianceBA[1];                   // Pressure Variance 
            data[54] = pressureVarianceBA[2];                   // Pressure Variance 
            data[55] = pressureVarianceBA[3];                   // Pressure Variance MSB
            data[56] = Spare;                                   // Spare
            data[57] = RtcY2kCentury;                           // RTC Y2K Century
            data[58] = RtcY2kYear;                              // RTC Y2K Year
            data[59] = RtcY2kMonth;                             // RTC Y2K Month
            data[60] = RtcY2kDay;                               // RTC Y2K Day
            data[61] = RtcY2kHour;                              // RTC Y2K Hour
            data[62] = RtcY2kMinute;                            // RTC Y2K Minute
            data[63] = RtcY2kSecond;                            // RTC Y2K Second
            data[64] = RtcY2kHundredth;                         // RTC Y2K Hundredth of Second

            return data;
        }

        #endregion

        #region Decode

        /// <summary>
        /// Decode the binary data to the a Variable Leader.
        /// </summary>
        /// <param name="data">Data to decode.</param>
        public override void Decode(byte[] data)
        {
            EnsembleNumber = MathHelper.LsbMsbUShort(data[2], data[3]);              // Ensemble Number
            RtcYear = data[4];                                                      // RTC Year
            RtcMonth = data[5];                                                     // RTC Month
            RtcDay = data[6];                                                       // RTC Day
            RtcHour = data[7];                                                      // RTC Hour
            RtcMinute = data[8];                                                    // RTC Minute
            RtcSecond = data[9];                                                    // RTC Second
            RtcHundredths = data[10];                                               // RTC Hundredths of Second
            EnsembleNumRollover = data[11];                                         // Ensemble Rollerover
            BitResult = MathHelper.LsbMsbShort(data[12], data[13]);                 // BIT Result
            SpeedOfSound = MathHelper.LsbMsbShort(data[14], data[15]);              // Speed Of Sound
            DepthOfTransducer = MathHelper.LsbMsbShort(data[16], data[17]);         // Depth of Transducer
            Heading = MathHelper.LsbMsbUShort(data[18], data[19]) * 0.01f;           // Heading
            Pitch = MathHelper.LsbMsbShort(data[20], data[21]) * 0.01f;             // Pitch
            Roll = MathHelper.LsbMsbShort(data[22], data[23]) * 0.01f;              // Roll
            Salinity = MathHelper.LsbMsbShort(data[24], data[25]);                  // Salinity
            Temperature = MathHelper.LsbMsbShort(data[26], data[27]) * 0.01f;       // Temperature
            MinPrePingWaitTimeMinutes = data[28];                                   // Minimum Pre-Ping Wait Time Minutes
            MinPrePingWaitTimeSeconds = data[29];                                   // Minimum Pre-Ping Wait Time Second;
            MinPrePingWaitTimeHundredths = data[30];                                // Minimum Pre-Ping Wait Time Hundredths of Second
            HeadingStdDev = data[31];                                               // Heading Standard Deviation
            PitchStdDev = data[32] * 0.1f;                                          // Pitch Standard Deviation
            RollStdDev = data[33] * 0.1f;                                           // Roll Standard Deviation
            Adc0 = data[34];                                                        // ADC Channel 0
            Adc1 = data[35];                                                        // ADC Channel 1
            Adc2 = data[36];                                                        // ADC Channel 2
            Adc3 = data[37];                                                        // ADC Channel 3
            Adc4 = data[38];                                                        // ADC Channel 4
            Adc5 = data[39];                                                        // ADC Channel 5
            Adc6 = data[40];                                                        // ADC Channel 6
            Adc7 = data[41];                                                        // ADC Channel 7
            Esw0 = data[42];                                                        // Error Status Word 43
            Esw1 = data[43];                                                        // Error Status Word 44
            Esw2 = data[44];                                                        // Error Status Word 45
            Esw3 = data[45];                                                        // Error Status Word 46
            Reserved = MathHelper.LsbMsbShort(data[46], data[47]);                  // Reserved
            Pressure = BitConverter.ToInt32(data, 48);                             // Pressure
            //Pressure = MathHelper.LsbMsbUInt(data, 48);                             // Pressure
            PressureVariance = BitConverter.ToInt32(data, 52);                     // Pressure Variance
            Spare = data[56];                                                       // Spare
            RtcY2kCentury = data[57];                                               // RTC Y2K Century
            RtcY2kYear = data[58];                                                  // RTC Y2K Year
            RtcY2kMonth = data[59];                                                 // RTC Y2K Month
            RtcY2kDay = data[60];                                                   // RTC Y2K Day
            RtcY2kHour = data[61];                                                  // RTC Y2K Hour
            RtcY2kMinute = data[62];                                                // RTC Y2K Minute
            RtcY2kSecond = data[63];                                                // RTC Y2K Second
            RtcY2kHundredth = data[64];                                             // RTC Y2K Hundredth of Second
        }

        #endregion

        #region Ensemble Number

        /// <summary>
        /// Get the Ensemble number.  This will take into account the 
        /// rollover value.
        /// 
        /// NOTE: The first ensemble collected is #1. At “rollover,” we
        /// have the following sequence:
        /// 1 = ENSEMBLE NUMBER 1
        ///   ↓
        /// 65535 = ENSEMBLE NUMBER 65,535 | ENSEMBLE
        ///     0 = ENSEMBLE NUMBER 65,536 | #MSB FIELD
        ///     1 = ENSEMBLE NUMBER 65,537 | (BYTE 12)
        /// INCR.
        /// </summary>
        /// <returns>Ensemble number.</returns>
        public int GetEnsembleNumber()
        {
            if (EnsembleNumRollover <= 0)
            {
                return EnsembleNumber;
            }


            int baseEnsNum = EnsembleNumRollover * ushort.MaxValue;

            return baseEnsNum + EnsembleNumber + 1;
        }

        #endregion

        #region BIT

        #region Set

        /// <summary>
        /// Reserved.
        /// Byte 13, Bit Position 0.
        /// </summary>
        public void SetReserved0()
        {
            BitResult = MathHelper.SetBitShort(BitResult, 0);
        }

        /// <summary>
        /// Timing Card Error.
        /// Byte 13, Bit Position 1.
        /// </summary>
        public void SetTimingCardError()
        {
            BitResult = MathHelper.SetBitShort(BitResult, 1);
        }

        /// <summary>
        /// Reserved
        /// Byte 13, Bit Position 2.
        /// </summary>
        public void SetReserved2()
        {
            BitResult = MathHelper.SetBitShort(BitResult, 2);
        }

        /// <summary>
        /// Demod 0 Error.
        /// Byte 13, Bit Position 3.
        /// </summary>
        public void SetDemod0Error()
        {
            BitResult = MathHelper.SetBitShort(BitResult, 3);
        }

        /// <summary>
        /// Demod 1 Error.
        /// Byte 13, Bit Position 4.
        /// </summary>
        public void SetDemod1Error()
        {
            BitResult = MathHelper.SetBitShort(BitResult, 4);
        }

        /// <summary>
        /// Reserved.
        /// Byte 13, Bit Position 5.
        /// </summary>
        public void SetReserved5()
        {
            BitResult = MathHelper.SetBitShort(BitResult, 5);
        }

        /// <summary>
        /// Reserved.
        /// Byte 13, Bit Position 6.
        /// </summary>
        public void SetReserved6()
        {
            BitResult = MathHelper.SetBitShort(BitResult, 6);
        }

        /// <summary>
        /// Reserved.
        /// Byte 13, Bit Position 7.
        /// </summary>
        public void SetReserved7()
        {
            BitResult = MathHelper.SetBitShort(BitResult, 7);
        }

        #endregion

        #region UnSet

        /// <summary>
        /// Reserved.
        /// Byte 13, Bit Position 0.
        /// </summary>
        public void UnSetReserved0()
        {
            BitResult = MathHelper.ZeroBitShort(BitResult, 0);
        }

        /// <summary>
        /// Timing Card Error.
        /// Byte 13, Bit Position 1.
        /// </summary>
        public void UnSetTimingCardError()
        {
            BitResult = MathHelper.ZeroBitShort(BitResult, 1);
        }

        /// <summary>
        /// Reserved
        /// Byte 13, Bit Position 2.
        /// </summary>
        public void UnSetReserved2()
        {
            BitResult = MathHelper.ZeroBitShort(BitResult, 2);
        }

        /// <summary>
        /// Demod 0 Error.
        /// Byte 13, Bit Position 3.
        /// </summary>
        public void UnSetDemod0Error()
        {
            BitResult = MathHelper.ZeroBitShort(BitResult, 3);
        }

        /// <summary>
        /// Demod 1 Error.
        /// Byte 13, Bit Position 4.
        /// </summary>
        public void UnSetDemod1Error()
        {
            BitResult = MathHelper.ZeroBitShort(BitResult, 4);
        }

        /// <summary>
        /// Reserved.
        /// Byte 13, Bit Position 5.
        /// </summary>
        public void UnSetReserved5()
        {
            BitResult = MathHelper.ZeroBitShort(BitResult, 5);
        }

        /// <summary>
        /// Reserved.
        /// Byte 13, Bit Position 6.
        /// </summary>
        public void UnSetReserved6()
        {
            BitResult = MathHelper.ZeroBitShort(BitResult, 6);
        }

        /// <summary>
        /// Reserved.
        /// Byte 13, Bit Position 7.
        /// </summary>
        public void UnSetReserved7()
        {
            BitResult = MathHelper.ZeroBitShort(BitResult, 7);
        }

        #endregion

        #region Get

        /// <summary>
        /// Check if Reserved 0 Bit is set.
        /// </summary>
        /// <returns>TRUE = Bit Set / False = Bit not set.</returns>
        public bool IsReserved0Set()
        {
            return MathHelper.IsBitSet(BitResult, 0);
        }

        /// <summary>
        /// Check if Timing Card Error Bit is set.
        /// </summary>
        /// <returns>TRUE = Bit Set / False = Bit not set.</returns>
        public bool IsTimingCardErrorSet()
        {
            return MathHelper.IsBitSet(BitResult, 1);
        }

        /// <summary>
        /// Check if Reserved 2 Bit is set.
        /// </summary>
        /// <returns>TRUE = Bit Set / False = Bit not set.</returns>
        public bool IsReserved2Set()
        {
            return MathHelper.IsBitSet(BitResult, 2);
        }

        /// <summary>
        /// Check if Demod 0 Error Bit is set.
        /// </summary>
        /// <returns>TRUE = Bit Set / False = Bit not set.</returns>
        public bool IsDemod0ErrorSet()
        {
            return MathHelper.IsBitSet(BitResult, 3);
        }

        /// <summary>
        /// Check if Demod 1 Error Bit is set.
        /// </summary>
        /// <returns>TRUE = Bit Set / False = Bit not set.</returns>
        public bool IsDemod1ErrorSet()
        {
            return MathHelper.IsBitSet(BitResult, 4);
        }

        /// <summary>
        /// Check if Reserved 5 Bit is set.
        /// </summary>
        /// <returns>TRUE = Bit Set / False = Bit not set.</returns>
        public bool IsReserved5Set()
        {
            return MathHelper.IsBitSet(BitResult, 5);
        }

        /// <summary>
        /// Check if Reserved 6 Bit is set.
        /// </summary>
        /// <returns>TRUE = Bit Set / False = Bit not set.</returns>
        public bool IsReserved6Set()
        {
            return MathHelper.IsBitSet(BitResult, 6);
        }

        /// <summary>
        /// Check if Reserved 7 Bit is set.
        /// </summary>
        /// <returns>TRUE = Bit Set / False = Bit not set.</returns>
        public bool IsReserved7Set()
        {
            return MathHelper.IsBitSet(BitResult, 7);
        }

        #endregion

        #endregion

        #region Error Status Word

        #region ESW0

        #region Get

        /// <summary>
        /// Check if the Bus Error Exception Bit is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsBusErrorExceptionSet()
        {
            return MathHelper.IsBitSet(Esw0, 0);
        }

        /// <summary>
        /// Check if the Address Error Exception Bit is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsAddressErrorExceptionSet()
        {
            return MathHelper.IsBitSet(Esw0, 1);
        }

        /// <summary>
        /// Check if the Illegal Instruction Exception Bit is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsIllegalInstructionExceptionSet()
        {
            return MathHelper.IsBitSet(Esw0, 2);
        }

        /// <summary>
        /// Check if the Zero Divide Exception Bit is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsZeroDivideExceptionSet()
        {
            return MathHelper.IsBitSet(Esw0, 3);
        }

        /// <summary>
        /// Check if the Emulator Exception Bit is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsEmulatorExceptionSet()
        {
            return MathHelper.IsBitSet(Esw0, 4);
        }

        /// <summary>
        /// Check if the Unassigned Exception Bit is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsUnassignedExceptionSet()
        {
            return MathHelper.IsBitSet(Esw0, 5);
        }

        /// <summary>
        /// Check if the Watchdog restart Exception Bit is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsWatchdogRestartExceptionSet()
        {
            return MathHelper.IsBitSet(Esw0, 6);
        }
        /// <summary>
        /// Check if the Battery Saver Power Bit is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsBatterySaverPowerSet()
        {
            return MathHelper.IsBitSet(Esw0, 7);
        }

        #endregion

        #region Set

        /// <summary>
        /// Set the Bus Error Exception Bit.
        /// </summary>
        public void SetBusErrorException()
        {
            Esw0 = MathHelper.SetBitByte(Esw0, 0);
        }

        /// <summary>
        /// Set the Address Error Exception Bit.
        /// </summary>
        public void SetAddressErrorException()
        {
            Esw0 = MathHelper.SetBitByte(Esw0, 1);
        }

        /// <summary>
        /// Set the Illegal Instruction Exception bit.
        /// </summary>
        public void SetIllegalInstructionException()
        {
            Esw0 = MathHelper.SetBitByte(Esw0, 2);
        }

        /// <summary>
        /// Set the Zero Divide Exception bit.
        /// </summary>
        public void SetZeroDivideException()
        {
            Esw0 = MathHelper.SetBitByte(Esw0, 3);
        }

        /// <summary>
        /// Set the Emulator Exception bit.
        /// </summary>
        public void SetEmulatorException()
        {
            Esw0 = MathHelper.SetBitByte(Esw0, 4);
        }

        /// <summary>
        /// SEt Unassigned Exception bit.
        /// </summary>
        public void SetUnassignedException()
        {
            Esw0 = MathHelper.SetBitByte(Esw0, 5);
        }

        /// <summary>
        /// Set the Watchdog Restart Occured bit.
        /// </summary>
        public void SetWatchdogRestartOccured()
        {
            Esw0 = MathHelper.SetBitByte(Esw0, 6);
        }

        /// <summary>
        /// Set the Battery Saver Power bit.
        /// </summary>
        public void SetBatterySaverPower()
        {
            Esw0 = MathHelper.SetBitByte(Esw0, 7);
        }

        #endregion

        #region UnSet

        /// <summary>
        /// UnSet the Bus Error Exception Bit.
        /// </summary>
        public void UnSetBusErrorException()
        {
            Esw0 = MathHelper.ZeroBitByte(Esw0, 0);
        }

        /// <summary>
        /// UnSet the Address Error Exception Bit.
        /// </summary>
        public void UnSetAddressErrorException()
        {
            Esw0 = MathHelper.ZeroBitByte(Esw0, 1);
        }

        /// <summary>
        /// UnSet the Illegal Instruction Exception bit.
        /// </summary>
        public void UnSetIllegalInstructionException()
        {
            Esw0 = MathHelper.ZeroBitByte(Esw0, 2);
        }

        /// <summary>
        /// UnSet the Zero Divide Exception bit.
        /// </summary>
        public void UnSetZeroDivideException()
        {
            Esw0 = MathHelper.ZeroBitByte(Esw0, 3);
        }

        /// <summary>
        /// UnSet the Emulator Exception bit.
        /// </summary>
        public void UnSetEmulatorException()
        {
            Esw0 = MathHelper.ZeroBitByte(Esw0, 4);
        }

        /// <summary>
        /// UnSet Unassigned Exception bit.
        /// </summary>
        public void UnSetUnassignedException()
        {
            Esw0 = MathHelper.ZeroBitByte(Esw0, 5);
        }

        /// <summary>
        /// UnSet the Watchdog Restart Occured bit.
        /// </summary>
        public void UnSetWatchdogRestartOccured()
        {
            Esw0 = MathHelper.ZeroBitByte(Esw0, 6);
        }

        /// <summary>
        /// UnSet the Battery Saver Power bit.
        /// </summary>
        public void UnSetBatterySaverPower()
        {
            Esw0 = MathHelper.ZeroBitByte(Esw0, 7);
        }

        #endregion

        #endregion

        #region ESW1

        #region Get

        /// <summary>
        /// Check if the Pinging bit is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsPingingSet()
        {
            return MathHelper.IsBitSet(Esw1, 0);
        }

        /// <summary>
        /// Check if the Not Used Byte 44, bit 01 bit is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsNotUsed44_01Set()
        {
            return MathHelper.IsBitSet(Esw1, 1);
        }

        /// <summary>
        /// Check if the Not Used Byte 44, bit 02 bit is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsNotUsed44_02Set()
        {
            return MathHelper.IsBitSet(Esw1, 2);
        }

        /// <summary>
        /// Check if the Not Used Byte 44, bit 03 bit is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsNotUsed44_03Set()
        {
            return MathHelper.IsBitSet(Esw1, 3);
        }

        /// <summary>
        /// Check if the Not Used Byte 44, bit 04 bit is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsNotUsed44_04Set()
        {
            return MathHelper.IsBitSet(Esw1, 4);
        }

        /// <summary>
        /// Check if the Not Used Byte 44, bit 05 bit is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsNotUsed44_05Set()
        {
            return MathHelper.IsBitSet(Esw1, 5);
        }

        /// <summary>
        /// Check if the Cold Wakeup Occurred bit is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsColdWakeupOccuredSet()
        {
            return MathHelper.IsBitSet(Esw1, 6);
        }

        /// <summary>
        /// Check if the Unknown Wakeup Occurred bit is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsUnknownWakeupOccurredSet()
        {
            return MathHelper.IsBitSet(Esw1, 7);
        }

        #endregion

        #region Set

        /// <summary>
        /// Set the Pinging Bit.
        /// </summary>
        public void SetPinging()
        {
            Esw1 = MathHelper.SetBitByte(Esw1, 0);
        }

        /// <summary>
        /// Set the Not Used Byte 44, Bit 1 bit.
        /// </summary>
        public void SetNotUsed44_01()
        {
            Esw1 = MathHelper.SetBitByte(Esw1, 1);
        }

        /// <summary>
        /// Set the Not Used Byte 44, Bit 2 bit.
        /// </summary>
        public void SetNotUsed44_02()
        {
            Esw1 = MathHelper.SetBitByte(Esw1, 2);
        }

        /// <summary>
        /// Set the Not Used Byte 44, Bit 3 bit.
        /// </summary>
        public void SetNotUsed44_03()
        {
            Esw1 = MathHelper.SetBitByte(Esw1, 3);
        }

        /// <summary>
        /// Set the Not Used Byte 44, Bit 4 bit.
        /// </summary>
        public void SetNotUsed44_04()
        {
            Esw1 = MathHelper.SetBitByte(Esw1, 4);
        }

        /// <summary>
        /// Set the Not Used Byte 44, Bit 5 bit.
        /// </summary>
        public void SetNotUsed44_05()
        {
            Esw1 = MathHelper.SetBitByte(Esw1, 5);
        }

        /// <summary>
        /// Set the Cold Wakeup Occurred bit.
        /// </summary>
        public void SetColdWakeupOccurred()
        {
            Esw1 = MathHelper.SetBitByte(Esw1, 6);
        }

        /// <summary>
        /// Set the Unknown Wakeup Occurred bit.
        /// </summary>
        public void SetUnknownWakeupOccurred()
        {
            Esw1 = MathHelper.SetBitByte(Esw1, 7);
        }

        #endregion

        #region UnSet

        /// <summary>
        /// UnSet the Pinging Bit.
        /// </summary>
        public void UnSetPinging()
        {
            Esw1 = MathHelper.ZeroBitByte(Esw1, 0);
        }

        /// <summary>
        /// UnSet the Not Used Byte 44, Bit 1 bit.
        /// </summary>
        public void UnSetNotUsed44_01()
        {
            Esw1 = MathHelper.ZeroBitByte(Esw1, 1);
        }

        /// <summary>
        /// UnSet the Not Used Byte 44, Bit 2 bit.
        /// </summary>
        public void UnSetNotUsed44_02()
        {
            Esw1 = MathHelper.ZeroBitByte(Esw1, 2);
        }

        /// <summary>
        /// UnSet the Not Used Byte 44, Bit 3 bit.
        /// </summary>
        public void UnSetNotUsed44_03()
        {
            Esw1 = MathHelper.ZeroBitByte(Esw1, 3);
        }

        /// <summary>
        /// UnSet the Not Used Byte 44, Bit 4 bit.
        /// </summary>
        public void UnSetNotUsed44_04()
        {
            Esw1 = MathHelper.ZeroBitByte(Esw1, 4);
        }

        /// <summary>
        /// UnSet the Not Used Byte 44, Bit 5 bit.
        /// </summary>
        public void UnSetNotUsed44_05()
        {
            Esw1 = MathHelper.ZeroBitByte(Esw1, 5);
        }

        /// <summary>
        /// UnSet the Cold Wakeup Occurred bit.
        /// </summary>
        public void UnSetColdWakeupOccurred()
        {
            Esw1 = MathHelper.ZeroBitByte(Esw1, 6);
        }

        /// <summary>
        /// UnSet the Unknown Wakeup Occurred bit.
        /// </summary>
        public void UnSetUnknownWakeupOccurred()
        {
            Esw1 = MathHelper.ZeroBitByte(Esw1, 7);
        }

        #endregion

        #endregion

        #region ESW2

        #region Get

        /// <summary>
        /// Check if the Clock Read Error is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsClockReadErrorSet()
        {
            return MathHelper.IsBitSet(Esw2, 0);
        }

        /// <summary>
        /// Check if the Unexpected Alarm is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsUnexpectedAlarmSet()
        {
            return MathHelper.IsBitSet(Esw2, 1);
        }

        /// <summary>
        /// Check if the Clock Jump Forward is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsClockJumpForwardSet()
        {
            return MathHelper.IsBitSet(Esw2, 2);
        }

        /// <summary>
        /// Check if the Clock Jump Backward is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsClockJumpBackwardSet()
        {
            return MathHelper.IsBitSet(Esw2, 3);
        }

        /// <summary>
        /// Check if the Not Used Byte 45, bit 04 is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsNotUsed45_04Set()
        {
            return MathHelper.IsBitSet(Esw2, 4);
        }

        /// <summary>
        /// Check if the Not Used Byte 45, bit 05 is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsNotUsed45_05Set()
        {
            return MathHelper.IsBitSet(Esw2, 5);
        }

        /// <summary>
        /// Check if the Not Used Byte 45, bit 06 is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsNotUsed45_06Set()
        {
            return MathHelper.IsBitSet(Esw2, 6);
        }

        /// <summary>
        /// Check if the Not Used Byte 45, bit 07 is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsNotUsed45_07Set()
        {
            return MathHelper.IsBitSet(Esw2, 7);
        }

        #endregion

        #region Set

        /// <summary>
        /// Set the Clock Read Error Occurred bit.
        /// </summary>
        public void SetClockReadErrorOccurred()
        {
            Esw2 = MathHelper.SetBitByte(Esw2, 0);
        }

        /// <summary>
        /// Set the Unexpected Alarm bit.
        /// </summary>
        public void SetUnexpectedAlarm()
        {
            Esw2 = MathHelper.SetBitByte(Esw2, 1);
        }

        /// <summary>
        /// Set the Clock Jump Forward bit.
        /// </summary>
        public void SetClockJumpForward()
        {
            Esw2 = MathHelper.SetBitByte(Esw2, 2);
        }

        /// <summary>
        /// Set the Clock Jump Backward bit.
        /// </summary>
        public void SetClockJumpBackward()
        {
            Esw2 = MathHelper.SetBitByte(Esw2, 3);
        }

        /// <summary>
        /// Set the Not Used Byte 45, Bit 04 bit.
        /// </summary>
        public void SetNotUsed45_04()
        {
            Esw2 = MathHelper.SetBitByte(Esw2, 4);
        }

        /// <summary>
        /// Set the Not Used Byte 45, Bit 05 bit.
        /// </summary>
        public void SetNotUsed45_05()
        {
            Esw2 = MathHelper.SetBitByte(Esw2, 5);
        }

        /// <summary>
        /// Set the Not Used Byte 45, Bit 06 bit.
        /// </summary>
        public void SetNotUsed45_06()
        {
            Esw2 = MathHelper.SetBitByte(Esw2, 6);
        }

        /// <summary>
        /// Set the Not Used Byte 45, Bit 07 bit.
        /// </summary>
        public void SetNotUsed45_07()
        {
            Esw2 = MathHelper.SetBitByte(Esw2, 7);
        }

        #endregion

        #region UnSet

        /// <summary>
        /// UnSet the Clock Read Error Occurred bit.
        /// </summary>
        public void UnSetClockReadErrorOccurred()
        {
            Esw2 = MathHelper.ZeroBitByte(Esw2, 0);
        }

        /// <summary>
        /// UnSet the Unexpected Alarm bit.
        /// </summary>
        public void UnSetUnexpectedAlarm()
        {
            Esw2 = MathHelper.ZeroBitByte(Esw2, 1);
        }

        /// <summary>
        /// UnSet the Clock Jump Forward bit.
        /// </summary>
        public void UnSetClockJumpForward()
        {
            Esw2 = MathHelper.ZeroBitByte(Esw2, 2);
        }

        /// <summary>
        /// UnSet the Clock Jump Backward bit.
        /// </summary>
        public void UnSetClockJumpBackward()
        {
            Esw2 = MathHelper.ZeroBitByte(Esw2, 3);
        }

        /// <summary>
        /// UnSet the Not Used Byte 45, Bit 04 bit.
        /// </summary>
        public void UnSetNotUsed45_04()
        {
            Esw2 = MathHelper.ZeroBitByte(Esw2, 4);
        }

        /// <summary>
        /// UnSet the Not Used Byte 45, Bit 05 bit.
        /// </summary>
        public void UnSetNotUsed45_05()
        {
            Esw2 = MathHelper.ZeroBitByte(Esw2, 5);
        }

        /// <summary>
        /// UnSet the Not Used Byte 45, Bit 06 bit.
        /// </summary>
        public void UnSetNotUsed45_06()
        {
            Esw2 = MathHelper.ZeroBitByte(Esw2, 6);
        }

        /// <summary>
        /// UnSet the Not Used Byte 45, Bit 07 bit.
        /// </summary>
        public void UnSetNotUsed45_07()
        {
            Esw2 = MathHelper.ZeroBitByte(Esw2, 7);
        }

        #endregion

        #endregion

        #region ESW3

        #region Get

        /// <summary>
        /// Check if the Not Used Byte 46, Bit 00 is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsNotUsed46_00Set()
        {
            return MathHelper.IsBitSet(Esw3, 0);
        }

        /// <summary>
        /// Check if the Not Used Byte 46, Bit 01 is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsNotUsed46_01Set()
        {
            return MathHelper.IsBitSet(Esw3, 1);
        }

        /// <summary>
        /// Check if the Not Used Byte 46, Bit 02 is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsNotUsed46_02Set()
        {
            return MathHelper.IsBitSet(Esw3, 2);
        }

        /// <summary>
        /// Check if the Power Fail bit is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsPowerFailSet()
        {
            return MathHelper.IsBitSet(Esw3, 3);
        }

        /// <summary>
        /// Check if the Spurious Level 4 Interrupt (DSP) bit is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsSpuriousLevel4InterruptSet()
        {
            return MathHelper.IsBitSet(Esw3, 4);
        }

        /// <summary>
        /// Check if the Spurious Level 5 Interrupt (UART) bit is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsSpuriousLevel5InterruptSet()
        {
            return MathHelper.IsBitSet(Esw3, 5);
        }

        /// <summary>
        /// Check if the Spurious Level 6 Interrupt (CLOCK) bit is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsSpuriousLevel6InterruptSet()
        {
            return MathHelper.IsBitSet(Esw3, 6);
        }

        /// <summary>
        /// Check if the Level 7 Interrupt Occurred bit is set.
        /// </summary>
        /// <returns>True = Bit Set / False = Bit is not set.</returns>
        public bool IsLevel7InterruptOccurredSet()
        {
            return MathHelper.IsBitSet(Esw3, 7);
        }

        #endregion

        #region Set

        /// <summary>
        /// Set the Not Used Byte 46, Bit 00 bit.
        /// </summary>
        public void SetNotUsed46_00()
        {
            Esw3 = MathHelper.SetBitByte(Esw3, 0);
        }

        /// <summary>
        /// Set the Not Used Byte 46, Bit 01 bit.
        /// </summary>
        public void SetNotUsed46_01()
        {
            Esw3 = MathHelper.SetBitByte(Esw3, 1);
        }

        /// <summary>
        /// Set the Not Used Byte 46, Bit 02 bit.
        /// </summary>
        public void SetNotUsed46_02()
        {
            Esw3 = MathHelper.SetBitByte(Esw3, 2);
        }

        /// <summary>
        /// Set the Power Fail bit.
        /// </summary>
        public void SetPowerFail()
        {
            Esw3 = MathHelper.SetBitByte(Esw3, 3);
        }

        /// <summary>
        /// Set the Spurious Level 4 Interrupt (DSP) bit.
        /// </summary>
        public void SetSpuriousLevel4Interrupt()
        {
            Esw3 = MathHelper.SetBitByte(Esw3, 4);
        }

        /// <summary>
        /// Set the Spurious Level 5 Interrupt (UART) bit.
        /// </summary>
        public void SetSpuriousLevel5Interrupt()
        {
            Esw3 = MathHelper.SetBitByte(Esw3, 5);
        }

        /// <summary>
        /// Set the Spurious Level 6 Interrupt (CLOCK) bit.
        /// </summary>
        public void SetSpuriousLevel6Interrupt()
        {
            Esw3 = MathHelper.SetBitByte(Esw3, 6);
        }

        /// <summary>
        /// Set the Spurious Level 7 Interrupt Occurred bit.
        /// </summary>
        public void SetLevel7InterruptOccurred()
        {
            Esw3 = MathHelper.SetBitByte(Esw3, 7);
        }

        #endregion

        #region UnSet

        /// <summary>
        /// UnSet the Not Used Byte 46, Bit 00 bit.
        /// </summary>
        public void UnSetNotUsed46_00()
        {
            Esw3 = MathHelper.ZeroBitByte(Esw3, 0);
        }

        /// <summary>
        /// UnSet the Not Used Byte 46, Bit 01 bit.
        /// </summary>
        public void UnSetNotUsed46_01()
        {
            Esw3 = MathHelper.ZeroBitByte(Esw3, 1);
        }

        /// <summary>
        /// UnSet the Not Used Byte 46, Bit 02 bit.
        /// </summary>
        public void UnSetNotUsed46_02()
        {
            Esw3 = MathHelper.ZeroBitByte(Esw3, 2);
        }

        /// <summary>
        /// UnSet the Power Fail bit.
        /// </summary>
        public void UnSetPowerFail()
        {
            Esw3 = MathHelper.ZeroBitByte(Esw3, 3);
        }

        /// <summary>
        /// UnSet the Spurious Level 4 Interrupt (DSP) bit.
        /// </summary>
        public void UnSetSpuriousLevel4Interrupt()
        {
            Esw3 = MathHelper.ZeroBitByte(Esw3, 4);
        }

        /// <summary>
        /// UnSet the Spurious Level 5 Interrupt (UART) bit.
        /// </summary>
        public void UnSetSpuriousLevel5Interrupt()
        {
            Esw3 = MathHelper.ZeroBitByte(Esw3, 5);
        }

        /// <summary>
        /// UnSet the Spurious Level 6 Interrupt (CLOCK) bit.
        /// </summary>
        public void UnSetSpuriousLevel6Interrupt()
        {
            Esw3 = MathHelper.ZeroBitByte(Esw3, 6);
        }

        /// <summary>
        /// UnSet the Spurious Level 7 Interrupt Occurred bit.
        /// </summary>
        public void UnSetLevel7InterruptOccurred()
        {
            Esw3 = MathHelper.ZeroBitByte(Esw3, 7);
        }

        #endregion

        #endregion

        #endregion

        #region Data Type Size

        /// <summary>
        /// Get the number of bytes in the data type.
        /// </summary>
        /// <returns>Number of bytes for the data type.</returns>
        public override int GetDataTypeSize()
        {
            return DATATYPE_SIZE;
        }

        #endregion

        #region RTI Ensemble

        /// <summary>
        /// Convert the RTI Ancillary data set to the PD0 Variable Leader data type.
        /// </summary>
        /// <param name="ens">RTI Ensemble data set.</param>
        /// <param name="anc">RTI Ancillary data set.</param>
        public void DecodeRtiEnsemble(DataSet.EnsembleDataSet ens, DataSet.AncillaryDataSet anc)
        {
            // Ensure the values were given, or create default values
            if (ens == null)
            {
                ens = new DataSet.EnsembleDataSet();
            }
            if (anc == null)
            {
                anc = new DataSet.AncillaryDataSet();
            }

            int rolloverEnsNum = ens.EnsembleNumber / ushort.MaxValue;
            int ensNum = ens.EnsembleNumber - (rolloverEnsNum * ushort.MaxValue);

            EnsembleNumber = (ushort)ensNum;
            RtcYear = (byte)(ens.Year - 2000);
            RtcMonth = (byte)ens.Month;
            RtcDay = (byte)ens.Day;
            RtcHour = (byte)ens.Hour;
            RtcMinute = (byte)ens.Minute;
            RtcSecond = (byte)ens.Second;
            RtcHundredths = (byte)ens.HSec;
            EnsembleNumRollover = (byte)rolloverEnsNum;
            BitResult = (short)ens.Status.Value;
            SpeedOfSound = (short)Math.Round(anc.SpeedOfSound);
            DepthOfTransducer = (short)Math.Round(10 * anc.TransducerDepth);
            Heading = anc.Heading;
            Pitch = anc.Pitch;

            float roll;
            if (anc.Roll > 90)
            {
                roll = -(180.0f - anc.Roll);
            }
            else if (anc.Roll < -90)
            {
                roll = (180.0f + anc.Roll);
            }
            else
            {
                roll = anc.Roll;
            }
            Roll = roll;

            Salinity = (short)Math.Round(anc.Salinity);
            Temperature = anc.WaterTemp;
            MinPrePingWaitTimeMinutes = 0;
            MinPrePingWaitTimeSeconds = 0;
            MinPrePingWaitTimeHundredths = 0;
            HeadingStdDev = 0;                                  // Do not get all the data from binary data
            PitchStdDev = 0;                                    // Do not get all the data from binary data
            RollStdDev = 0;                                     // Do not get all the data from binary data
            Adc0 = 0;
            Adc1 = 0;
            Adc2 = 0;
            Adc3 = 0;
            Adc4 = 0;
            Adc5 = 0;
            Adc6 = 0;
            Adc7 = 0;
            Esw0 = 0;
            Esw1 = 0;
            Esw2 = 0;
            Esw3 = 0;
            Reserved = 0;
            Pressure = (int)Math.Round(0.0001 * anc.Pressure);
            PressureVariance = 0;                               // Do not get all the data from the binary data
            Spare = 0;
            RtcY2kCentury = 20;
            RtcY2kYear = (byte)(ens.Year - 2000);
            RtcY2kMonth = (byte)ens.Month;
            RtcY2kDay = (byte)ens.Day;
            RtcY2kHour = (byte)ens.Hour;
            RtcY2kMinute = (byte)ens.Minute;
            RtcY2kSecond = (byte)ens.Second;
            RtcY2kHundredth = (byte)ens.HSec;
        }

        #endregion

    }
}
