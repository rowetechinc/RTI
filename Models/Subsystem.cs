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
 * 01/24/2012      RC          1.14       Fix bug decoding string to hex byte.
 * 01/25/2012      RC          1.14       Fix bug decoding string to hex byte, it did not handle letters.
 * 01/31/2012      RC          1.14       Updated variables and text, replacing spares with actual values. User Guide Rev F.
 * 02/01/2012      RC          1.14       Added CodeToString().
 * 09/17/2012      RC          2.15       Removed Private Set for Index and Code to allow JSON encode and decode.
 * 09/24/2012      RC          2.15       Added IsEmpty() to check if the Subsystem is an empty subsystem.
 * 11/16/2012      RC          2.16       When checking if subsystems are equal, only check if the codes are equal.
 *                                         Added == and != for SubsystemCodeDesc.
 * 11/19/2012      RC          2.16       Updated the SubsystemList.
 *                                         Set a default value of 0 for the index in the Subsystem Constructor.
 *                                         Add a SubsystemCodeDesc constructor that only takes the code.
 * 11/30/2012      RC          2.17       Added a note about the subsystem code being EMPTY.
 * 12/27/2012      RC          2.17       Removed Subsystem.Empty.  It was not readonly.
 * 01/14/2013      RC          2.17       Convert the subsystem char to a decimal using ConvertSubsystemCode().
 * 05/30/2013      RC          2.19       Added CodedDescString() and Display to display the Subsystem as a string.
 * 06/10/2013      RC          2.19       Added CodeToChar() to convert the code to a char.
 * 07/22/2013      RC          2.19.1     Added SUB_1_2MHZ_4BEAM_20DEG_PISTON_OPPOSITE_FACING_c, SUB_600KHZ_4BEAM_20DEG_PISTON_OPPOSITE_FACING_d and SUB_300KHZ_4BEAM_20DEG_PISTON_OPPOSITE_FACING_e
 * 08/14/2013      RC          2.19.4     Encode and Decode to JSON.
 * 03/17/2017      RC          3.4.2      Added IsVerticalBeam() to check if the code is for a vertical beam subsystem.
 *
 */

using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace RTI
{
    /// <summary>
    /// Hold the subsystem settings.
    /// This includes the subsystem index and system code.
    /// The code designates the type of system.
    /// </summary>
    [JsonConverter(typeof(SubsystemSerializer))]
    public class Subsystem
    {
        #region Enums and Classes

        /// <summary>
        /// System frequency types.
        /// </summary>
        public enum SystemFrequency
        {
            /// <summary>
            /// 20 kHz.
            /// </summary>
            Freq_20kHz,

            /// <summary>
            /// 38 kHz.
            /// </summary>
            Freq_38kHz,

            /// <summary>
            /// 75 kHz.
            /// </summary>
            Freq_75kHz,

            /// <summary>
            /// 150 kHz.
            /// </summary>
            Freq_150kHz,

            /// <summary>
            /// 300 kHz.
            /// </summary>
            Freq_300kHz,

            /// <summary>
            /// 600 kHz.
            /// </summary>
            Freq_600kHz,

            /// <summary>
            /// 1200 kHz.
            /// </summary>
            Freq_1200kHz,

            /// <summary>
            /// 2000 kHz.
            /// </summary>
            Freq_2000kHz

        }

        /// <summary>
        /// Beam angle types.
        /// </summary>
        public enum BeamAngles
        {
            /// <summary>
            /// 15 Degrees Beam Angle.
            /// </summary>
            BeamAngle_15_Degree,

            /// <summary>
            /// 20 Degrees Beam Angle.
            /// </summary>
            BeamAngle_20_Degree,

            /// <summary>
            /// 30 Degrees Beam Angle.
            /// </summary>
            BeamAngle_30_Degree,

            /// <summary>
            /// Vertical Beam Angle.
            /// </summary>
            BeamAngle_Vertical,

            /// <summary>
            /// Other Beam Angle.
            /// </summary>
            BeamAngle_Other
        }

        #endregion

        #region Variables

        /// <summary>
        /// Used to represent an empty sub-system.
        /// </summary>
        public const string EMPTY = "0"; 

        /// <summary>
        /// Empty subsystem.
        /// </summary>
        public const byte EMPTY_CODE = 0x0;

        /// <summary>
        /// Spare with value 0x30.
        /// ASCII: 0
        /// HEX: 0x30
        /// </summary>
        public const byte SUB_SPARE_0 = 0x30;

        /// <summary>
        /// 2 MHz 4 Beam 20 Degree Piston.
        /// ASCII: 1
        /// HEX: 0x31
        /// </summary>
        public const byte SUB_2MHZ_4BEAM_20DEG_PISTON_1 = 0x31;

        /// <summary>
        /// 1.2 MHz 4 Beam 20 Degree Piston.
        /// ASCII: 2
        /// HEX: 0x32
        /// </summary>
        public const byte SUB_1_2MHZ_4BEAM_20DEG_PISTON_2 = 0x32;

        /// <summary>
        /// 600 kHz 4 Beam 20 Degree Piston.
        /// ASCII: 3
        /// HEX: 0x33
        /// </summary>
        public const byte SUB_600KHZ_4BEAM_20DEG_PISTON_3 = 0x33;

        /// <summary>
        /// 300 kHz 4 Beam 20 Degree Piston.
        /// ASCII: 4
        /// HEX: 0x34
        /// </summary>
        public const byte SUB_300KHZ_4BEAM_20DEG_PISTON_4 = 0x34;

        /// <summary>
        /// 2 MHz 4 Beam 20 Degree piston, 45 degree heading offset.
        /// ASCII: 5
        /// HEX: 0x35
        /// </summary>
        public const byte SUB_2MHZ_4BEAM_20DEG_PISTON_45OFFSET_5 = 0x35;

        /// <summary>
        /// 1.2 MHz 4 Beam 20 Degree piston, 45 degree heading offset.
        /// ASCII: 6
        /// HEX: 0x36
        /// </summary>
        public const byte SUB_1_2MHZ_4BEAM_20DEG_PISTON_45OFFSET_6 = 0x36;

        /// <summary>
        /// 600 kHz 4 Beam 20 Degree piston, 45 degree heading offset.
        /// ASCII: 7
        /// HEX: 0x37
        /// </summary>
        public const byte SUB_600KHZ_4BEAM_20DEG_PISTON_45OFFSET_7 = 0x37;

        /// <summary>
        /// 300 kHz 4 Beam 20 Degree piston, 45 degree heading offset.
        /// ASCII: 8
        /// HEX: 0x38
        /// </summary>
        public const byte SUB_300KHZ_4BEAM_20DEG_PISTON_45OFFSET_8 = 0x38;

        /// <summary>
        /// 2 MHz vertical beam piston.
        /// ASCII: 9
        /// HEX: 0x39
        /// </summary>
        public const byte SUB_2MHZ_VERT_PISTON_9 = 0x39;

        /// <summary>
        /// 1.2 MHz vertical beam piston.
        /// ASCII: A
        /// HEX: 0x41
        /// </summary>
        public const byte SUB_1_2MHZ_VERT_PISTON_A = 0x41;

        /// <summary>
        /// 600 kHz vertical beam piston.
        /// ASCII: B
        /// HEX: 0x42
        /// </summary>
        public const byte SUB_600KHZ_VERT_PISTON_B = 0x42;

        /// <summary>
        /// 300 kHz vertical beam piston.
        /// ASCII: C
        /// HEX: 0x43
        /// </summary>
        public const byte SUB_300KHZ_VERT_PISTON_C = 0x43;

        /// <summary>
        /// 150 kHz vertical beam piston.
        /// ASCII: D
        /// HEX: 0x44
        /// </summary>
        public const byte SUB_150KHZ_VERT_PISTON_D = 0x44;

        /// <summary>
        /// 75 kHz vertical beam piston.
        /// ASCII: E
        /// HEX: 0x45
        /// </summary>
        public const byte SUB_75KHZ_VERT_PISTON_E = 0x45;

        /// <summary>
        /// 38 kHz vertical beam piston.
        /// ASCII: F
        /// HEX: 0x46
        /// </summary>
        public const byte SUB_38KHZ_VERT_PISTON_F = 0x46;

        /// <summary>
        /// 20 kHz vertical beam piston.
        /// ASCII: G
        /// HEX: 0x47
        /// </summary>
        public const byte SUB_20KHZ_VERT_PISTON_G = 0x47;

        /// <summary>
        /// Spare.
        /// ASCII: H
        /// HEX: 0x48
        /// </summary>
        public const byte SUB_SPARE_H = 0x48;

        /// <summary>
        /// 600 kHz 4 beam 30 degree array.
        /// ASCII: I
        /// HEX: 0x49
        /// </summary>
        public const byte SUB_600KHZ_4BEAM_30DEG_ARRAY_I = 0x49;

        /// <summary>
        /// 300 kHz 4 beam 30 degree array.
        /// ASCII: J
        /// HEX: 0x4A
        /// </summary>
        public const byte SUB_300KHZ_4BEAM_30DEG_ARRAY_J = 0x4A;

        /// <summary>
        /// 150 kHz 4 beam 30 degree array.
        /// ASCII: K
        /// HEX: 0x4B
        /// </summary>
        public const byte SUB_150KHZ_4BEAM_30DEG_ARRAY_K = 0x4B;

        /// <summary>
        /// 75 kHz 4 beam 30 degree array.
        /// ASCII: L
        /// HEX: 0x4C
        /// </summary>
        public const byte SUB_75KHZ_4BEAM_30DEG_ARRAY_L = 0x4C;

        /// <summary>
        /// 38 kHz 4 beam 30 degree array.
        /// ASCII: M
        /// HEX: 0x4D
        /// </summary>
        public const byte SUB_38KHZ_4BEAM_30DEG_ARRAY_M = 0x4D;

        /// <summary>
        /// 20 kHz 4 beam 30 degree array.
        /// ASCII: N
        /// HEX: 0x4E
        /// </summary>
        public const byte SUB_20KHZ_4BEAM_30DEG_ARRAY_N = 0x4E;

        /// <summary>
        /// 600 kHz 4 beam 15 degree array.
        /// ASCII: O
        /// Hex: 0x4F
        /// </summary>
        public const byte SUB_600KHZ_4BEAM_15DEG_ARRAY_O = 0x4F;

        /// <summary>
        /// 300 kHz 4 beam 15 degree array.
        /// ASCII: P
        /// Hex: 0x50
        /// </summary>
        public const byte SUB_300KHZ_4BEAM_15DEG_ARRAY_P = 0x50;

        /// <summary>
        /// 150 kHz 4 beam 15 degree array.
        /// ASCII: Q
        /// Hex: 0x51
        /// </summary>
        public const byte SUB_150KHZ_4BEAM_15DEG_ARRAY_Q = 0x51;

        /// <summary>
        /// 75 kHz 4 beam 15 degree array.
        /// ASCII: R
        /// Hex: 0x52
        /// </summary>
        public const byte SUB_75KHZ_4BEAM_15DEG_ARRAY_R = 0x52;

        /// <summary>
        /// 38 kHz 4 beam 15 degree array.
        /// ASCII: S
        /// Hex: 0x53
        /// </summary>
        public const byte SUB_38KHZ_4BEAM_15DEG_ARRAY_S = 0x53;

        /// <summary>
        /// 20 kHz 4 beam 15 degree array.
        /// ASCII: T
        /// Hex: 0x54
        /// </summary>
        public const byte SUB_20KHZ_4BEAM_15DEG_ARRAY_T = 0x54;

        /// <summary>
        /// 600 kHz 1 beam 0 degree array.
        /// ASCII: U
        /// Hex: 0x55
        /// </summary>
        public const byte SUB_600KHZ_1BEAM_0DEG_ARRAY_U = 0x55;

        /// <summary>
        /// 300 kHz 1 beam 0 degree array.
        /// ASCII: V
        /// Hex: 0x56
        /// </summary>
        public const byte SUB_300KHZ_1BEAM_0DEG_ARRAY_V = 0x56;

        /// <summary>
        /// 150 kHz 1 beam 0 degree array.
        /// ASCII: W
        /// Hex: 0x57
        /// </summary>
        public const byte SUB_150KHZ_1BEAM_0DEG_ARRAY_W = 0x57;

        /// <summary>
        /// 75 kHz 1 beam 0 degree array.
        /// ASCII: X
        /// Hex: 0x58
        /// </summary>
        public const byte SUB_75KHZ_1BEAM_0DEG_ARRAY_X = 0x58;

        /// <summary>
        /// 38 kHz 1 beam 0 degree array.
        /// ASCII: Y
        /// Hex: 0x59
        /// </summary>
        public const byte SUB_38KHZ_1BEAM_0DEG_ARRAY_Y = 0x59;

        /// <summary>
        /// 20 kHz 1 beam 0 degree array.
        /// ASCII: Z
        /// Hex: 0x5A
        /// </summary>
        public const byte SUB_20KHZ_1BEAM_0DEG_ARRAY_Z = 0x5A;

        /// <summary>
        /// Spare with value 0x61.
        /// </summary>
        public const byte SUB_SPARE_a = 0x61;

        /// <summary>
        /// Spare with value 0x62.
        /// </summary>
        public const byte SUB_SPARE_b = 0x62;

        /// <summary>
        /// 1.2 MHz 4 beam 20 degree piston opposite facing
        /// ASCII: c
        /// Hex: 0x63
        /// </summary>
        public const byte SUB_1_2MHZ_4BEAM_20DEG_PISTON_OPPOSITE_FACING_c = 0x63;

        /// <summary>
        /// 600 KHz 4 beam 20 degree piston opposite facing
        /// ASCII: c
        /// Hex: 0x63
        /// </summary>
        public const byte SUB_600KHZ_4BEAM_20DEG_PISTON_OPPOSITE_FACING_d = 0x64;

        /// <summary>
        /// 300 KHz 4 beam 20 degree piston opposite facing
        /// ASCII: c
        /// Hex: 0x63
        /// </summary>
        public const byte SUB_300KHZ_4BEAM_20DEG_PISTON_OPPOSITE_FACING_e = 0x65;

        #endregion

        #region Properties

        /// <summary>
        /// There can be duplicate subsystem
        /// ID within a serial number.  This will
        /// differeniate the Subsystem, by its location
        /// within the serial number.
        /// </summary>
        public UInt16 Index { get; set; }

        /// <summary>
        /// Sub-System code.
        /// This code represents the system configuration
        /// for the ensemble given.  The Subsystem ID
        /// can be found in the RTI ADCP User Guide.
        /// </summary>
        public byte Code { get; set; }

        /// <summary>
        /// Used to display in a list.
        /// Set the DisplayMember of the combobox.
        /// http://stackoverflow.com/questions/3664956/c-sharp-combobox-overridden-tostring
        /// </summary>
        [JsonIgnore]
        public string Display { get { return CodedDescString(); } }

        #endregion

        /// <summary>
        /// Do not take a code.
        /// </summary>
        public Subsystem()
        {
            Code = EMPTY_CODE;
            Index = 0;
        }

        /// <summary>
        /// Set the code and index for the subsystem.
        /// The Subsystem code is found in the serial number.
        /// </summary>
        /// <param name="code">Subsystem ID.</param>
        /// <param name="index">Index of the subsystem within the serial number.</param>
        public Subsystem(byte code, UInt16 index = 0)
        {
            Index = index;
            Code = code;
        }

        /// <summary>
        /// Set the subsystem code and index.
        /// Convert the ascii character to a 
        /// hex values. 
        /// </summary>
        /// <param name="code">Code for the sub-system.</param>
        /// <param name="index">Index of the subsystem within the serial number.</param>
        public Subsystem(string code, UInt16 index)
        {
            // Ensure a good code is given
            // It should only be 1 character long
            if (code.Length < 0 || code.Length > 1)
            {
                Code = Subsystem.EMPTY_CODE;
                Index = 0;
            }
            else
            {
                Code = Subsystem.ConvertSubsystemCode(code[0]);
                Index = index;
            }
        }

        /// <summary>
        /// Return the code.
        /// </summary>
        /// <returns>The code.</returns>
        public byte Encode()
        {
            return Code;
        }

        /// <summary>
        /// State if this Subsystem is empty.  This will
        /// verify the Code and Index is the empty value.
        /// If they are, return true.
        /// </summary>
        /// <returns>TRUE = Subsystem is Empty. / FALSE = Subsystem is not empty.</returns>
        public bool IsEmpty()
        {
            if (Code == EMPTY_CODE && Index == 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Create a string for this object.
        /// The string will contain the code and index.
        /// 
        /// Ex:
        /// INDEX_CODE
        /// </summary>
        /// <returns>Code and index of this object as a string.</returns>
        public override string ToString()
        {
            return string.Format("{0}_{1}", Index.ToString(), CodeToString());
        }

        /// <summary>
        /// Convert the code to a char.
        /// </summary>
        /// <returns>Code as a char.</returns>
        public char CodeToChar()
        {
            return Convert.ToChar(Code);
        }

        /// <summary>
        /// Return the string version of the code.
        /// This will convert the code from a hex byte value to 
        /// a string character.
        /// 
        /// Ex:
        /// CODE
        /// </summary>
        /// <returns>Code as a string.</returns>
        public string CodeToString()
        {
            return Convert.ToString(CodeToChar());
        }

        /// <summary>
        /// Get the description string based off
        /// the code for this object.
        /// 
        /// Ex:
        /// XXX kHz X Beam XX degree piston
        /// </summary>
        /// <returns>String of the subsystem description.</returns>
        public string DescString()
        {
            return DescString(Code);
        }

        /// <summary>
        /// Create a string for the subsystem that includes the
        /// code and the description.
        /// 
        /// Ex:
        /// CODE - XXX kHz X Beam XX degree piston
        /// </summary>
        /// <returns>String of the subsystem with the code and description.</returns>
        public string CodedDescString()
        {
            return CodeToString() + " - " + DescString(Code);
        }

        /// <summary>
        /// Convert the code into a string.
        ///
        /// Ex:
        /// XXX kHz X Beam XX degree piston
        /// 
        /// NOTE
        /// If the subsystem is an old revision of firmware,
        /// the subsystem code will be an index
        /// instead of the code.  Use the index and the
        /// serial number to determine the string. 
        ///
        /// </summary>
        /// <param name="code">Subsystem code.</param>
        /// <returns>String representing the code.</returns>
        public static string DescString(byte code)
        {
            switch(code)
            {
                case EMPTY_CODE:
                    return EMPTY;
                case SUB_SPARE_0:
                    return "Spare";
                case SUB_2MHZ_4BEAM_20DEG_PISTON_1:
                    return "2 MHz 4 beam 20 degree piston";
                case SUB_1_2MHZ_4BEAM_20DEG_PISTON_2:
                    return "1.2 MHz 4 beam 20 degree piston";
                case SUB_600KHZ_4BEAM_20DEG_PISTON_3:
                    return "600 kHz 4 beam 20 degree piston";
                case SUB_300KHZ_4BEAM_20DEG_PISTON_4:
                    return "300 kHz 4 beam 20 degree piston";
                case SUB_2MHZ_4BEAM_20DEG_PISTON_45OFFSET_5:
                    return "2 MHz 4 beam 20 degree piston, 45 degree heading offset";
                case SUB_1_2MHZ_4BEAM_20DEG_PISTON_45OFFSET_6:
                    return "1.2 MHz 4 beam 20 degree piston, 45 degree heading offset";
                case SUB_600KHZ_4BEAM_20DEG_PISTON_45OFFSET_7:
                    return "600 kHz 4 beam 20 degree piston, 45 degree heading offset";
                case SUB_300KHZ_4BEAM_20DEG_PISTON_45OFFSET_8:
                    return "300 kHz 4 beam 20 degree piston, 45 degree heading offset";
                case SUB_2MHZ_VERT_PISTON_9:
                    return "2 MHz vertical beam piston";
                case SUB_1_2MHZ_VERT_PISTON_A:
                    return "1.2 MHz vertical beam piston";
                case SUB_600KHZ_VERT_PISTON_B:
                    return "600 kHz vertical beam piston";
                case SUB_300KHZ_VERT_PISTON_C:
                    return "300 kHz vertical beam piston";
                case SUB_150KHZ_VERT_PISTON_D:
                    return "150 kHz vertical beam piston";
                case SUB_75KHZ_VERT_PISTON_E:
                    return "75 kHz vertical beam piston";
                case SUB_38KHZ_VERT_PISTON_F:
                    return "38 kHz vertical beam piston";
                case SUB_20KHZ_VERT_PISTON_G:
                    return "20 kHz vertical beam piston";
                case SUB_SPARE_H:
                    return "Spare";
                case SUB_600KHZ_4BEAM_30DEG_ARRAY_I:
                    return "600 kHz 4 beam 30 degree array";
                case SUB_300KHZ_4BEAM_30DEG_ARRAY_J:
                    return "300 kHz 4 beam 30 degree array";
                case SUB_150KHZ_4BEAM_30DEG_ARRAY_K:
                    return "150 kHz 4 beam 30 degree array";
                case SUB_75KHZ_4BEAM_30DEG_ARRAY_L:
                    return "75 kHz 4 beam 30 degree array";
                case SUB_38KHZ_4BEAM_30DEG_ARRAY_M:
                    return "38 kHz 4 beam 30 degree array";
                case SUB_20KHZ_4BEAM_30DEG_ARRAY_N:
                    return "20 kHz 4 beam 30 degree array";
                case SUB_600KHZ_4BEAM_15DEG_ARRAY_O:
                    return "600 kHz 4 beam 15 degree array";
                case SUB_300KHZ_4BEAM_15DEG_ARRAY_P:
                    return "300 kHz 4 beam 15 degree array";
                case SUB_150KHZ_4BEAM_15DEG_ARRAY_Q:
                    return "150 kHz 4 beam 15 degree array";
                case SUB_75KHZ_4BEAM_15DEG_ARRAY_R:
                    return "75 kHz 4 beam 15 degree array";
                case SUB_38KHZ_4BEAM_15DEG_ARRAY_S:
                    return "38 kHz 4 beam 15 degree array";
                case SUB_20KHZ_4BEAM_15DEG_ARRAY_T:
                    return "20 kHz 4 beam 15 degree array";
                case SUB_600KHZ_1BEAM_0DEG_ARRAY_U:
                    return "600 kHz 1 beam 0 degree array";
                case SUB_300KHZ_1BEAM_0DEG_ARRAY_V:
                    return "300 kHz 1 beam 0 degree array";
                case SUB_150KHZ_1BEAM_0DEG_ARRAY_W:
                    return "150 Hz 1 beam 0 degree array";
                case SUB_75KHZ_1BEAM_0DEG_ARRAY_X:
                    return "75 kHz 1 beam 0 degree array";
                case SUB_38KHZ_1BEAM_0DEG_ARRAY_Y:
                    return "38 kHz 1 beam 0 degree array";
                case SUB_20KHZ_1BEAM_0DEG_ARRAY_Z:
                    return "20 kHz 1 beam 0 degree array";
                case SUB_SPARE_a:
                    return "Spare";
                case SUB_SPARE_b:
                    return "Spare";
                default:
                    return EMPTY;
            }
        }

        /// <summary>
        /// Get the system frequency.
        /// </summary>
        /// <returns>System frequency.</returns>
        public SystemFrequency GetSystemFrequency()
        {
            return Subsystem.GetSystemFrequency(Code);
        }

        /// <summary>
        /// Get the system frequency value.
        /// </summary>
        /// <returns>System frequency value.</returns>
        public float GetSystemFrequencyValue()
        {
            return Subsystem.GetSystemFrequencyValue(Code);
        }

        /// <summary>
        /// Get the system configuration based off the code given.
        /// </summary>
        /// <param name="code">Subsystem code.</param>
        /// <returns>System frequency.</returns>
        public static SystemFrequency GetSystemFrequency(byte code)
        {
            switch (code)
            {
                case SUB_2MHZ_4BEAM_20DEG_PISTON_1:
                    return SystemFrequency.Freq_2000kHz;
                case SUB_1_2MHZ_4BEAM_20DEG_PISTON_2:
                    return SystemFrequency.Freq_1200kHz;
                case SUB_600KHZ_4BEAM_20DEG_PISTON_3:
                    return SystemFrequency.Freq_600kHz;
                case SUB_300KHZ_4BEAM_20DEG_PISTON_4:
                    return SystemFrequency.Freq_300kHz;
                case SUB_2MHZ_4BEAM_20DEG_PISTON_45OFFSET_5:
                    return SystemFrequency.Freq_2000kHz;
                case SUB_1_2MHZ_4BEAM_20DEG_PISTON_45OFFSET_6:
                    return SystemFrequency.Freq_1200kHz;
                case SUB_600KHZ_4BEAM_20DEG_PISTON_45OFFSET_7:
                    return SystemFrequency.Freq_600kHz;
                case SUB_300KHZ_4BEAM_20DEG_PISTON_45OFFSET_8:
                    return SystemFrequency.Freq_300kHz;
                case SUB_2MHZ_VERT_PISTON_9:
                    return SystemFrequency.Freq_2000kHz;
                case SUB_1_2MHZ_VERT_PISTON_A:
                    return SystemFrequency.Freq_1200kHz;
                case SUB_600KHZ_VERT_PISTON_B:
                    return SystemFrequency.Freq_600kHz;
                case SUB_300KHZ_VERT_PISTON_C:
                    return SystemFrequency.Freq_300kHz;
                case SUB_150KHZ_VERT_PISTON_D:
                    return SystemFrequency.Freq_150kHz;
                case SUB_75KHZ_VERT_PISTON_E:
                    return SystemFrequency.Freq_75kHz;
                case SUB_38KHZ_VERT_PISTON_F:
                    return SystemFrequency.Freq_38kHz;
                case SUB_20KHZ_VERT_PISTON_G:
                    return SystemFrequency.Freq_20kHz;
                case SUB_600KHZ_4BEAM_30DEG_ARRAY_I:
                    return SystemFrequency.Freq_600kHz;
                case SUB_300KHZ_4BEAM_30DEG_ARRAY_J:
                    return SystemFrequency.Freq_300kHz;
                case SUB_150KHZ_4BEAM_30DEG_ARRAY_K:
                    return SystemFrequency.Freq_150kHz;
                case SUB_75KHZ_4BEAM_30DEG_ARRAY_L:
                    return SystemFrequency.Freq_75kHz;
                case SUB_38KHZ_4BEAM_30DEG_ARRAY_M:
                    return SystemFrequency.Freq_38kHz;
                case SUB_20KHZ_4BEAM_30DEG_ARRAY_N:
                    return SystemFrequency.Freq_20kHz;
                case SUB_600KHZ_4BEAM_15DEG_ARRAY_O:
                    return SystemFrequency.Freq_600kHz;
                case SUB_300KHZ_4BEAM_15DEG_ARRAY_P:
                    return SystemFrequency.Freq_300kHz;
                case SUB_150KHZ_4BEAM_15DEG_ARRAY_Q:
                    return SystemFrequency.Freq_150kHz;
                case SUB_75KHZ_4BEAM_15DEG_ARRAY_R:
                    return SystemFrequency.Freq_75kHz;
                case SUB_38KHZ_4BEAM_15DEG_ARRAY_S:
                    return SystemFrequency.Freq_38kHz;
                case SUB_20KHZ_4BEAM_15DEG_ARRAY_T:
                    return SystemFrequency.Freq_20kHz;
                case SUB_600KHZ_1BEAM_0DEG_ARRAY_U:
                    return SystemFrequency.Freq_600kHz;
                case SUB_300KHZ_1BEAM_0DEG_ARRAY_V:
                    return SystemFrequency.Freq_300kHz;
                case SUB_150KHZ_1BEAM_0DEG_ARRAY_W:
                    return SystemFrequency.Freq_150kHz;
                case SUB_75KHZ_1BEAM_0DEG_ARRAY_X:
                    return SystemFrequency.Freq_75kHz;
                case SUB_38KHZ_1BEAM_0DEG_ARRAY_Y:
                    return SystemFrequency.Freq_38kHz;
                case SUB_20KHZ_1BEAM_0DEG_ARRAY_Z:
                    return SystemFrequency.Freq_20kHz;
                case SUB_SPARE_0:
                case SUB_SPARE_H:
                case SUB_SPARE_a:
                case SUB_SPARE_b:
                case EMPTY_CODE:
                default:
                    return SystemFrequency.Freq_300kHz;
            }
        }

        /// <summary>
        /// Get the system frequency value based off the code given.
        /// </summary>
        /// <param name="code">Subsystem code.</param>
        /// <returns>System frequency value.</returns>
        public static float GetSystemFrequencyValue(byte code)
        {
            SystemFrequency freq = GetSystemFrequency(code);

            switch (freq)
            {
                case SystemFrequency.Freq_20kHz:
                    return RTI.Core.Commons.FREQ_BASE / RTI.Core.Commons.FREQ_DIV_20;
                case SystemFrequency.Freq_38kHz:
                    return RTI.Core.Commons.FREQ_BASE / RTI.Core.Commons.FREQ_DIV_38;
                case SystemFrequency.Freq_75kHz:
                    return RTI.Core.Commons.FREQ_BASE / RTI.Core.Commons.FREQ_DIV_75;
                case SystemFrequency.Freq_150kHz:
                    return RTI.Core.Commons.FREQ_BASE / RTI.Core.Commons.FREQ_DIV_150;
                case SystemFrequency.Freq_300kHz:
                    return RTI.Core.Commons.FREQ_BASE / RTI.Core.Commons.FREQ_DIV_300;
                case SystemFrequency.Freq_600kHz:
                    return RTI.Core.Commons.FREQ_BASE / RTI.Core.Commons.FREQ_DIV_600;
                case SystemFrequency.Freq_1200kHz:
                    return RTI.Core.Commons.FREQ_BASE / RTI.Core.Commons.FREQ_DIV_1200;
                case SystemFrequency.Freq_2000kHz:
                    return RTI.Core.Commons.FREQ_BASE * 2;
            }

            // Default to 300 kHz
            return RTI.Core.Commons.FREQ_BASE / RTI.Core.Commons.FREQ_DIV_300; 
        }

        /// <summary>
        /// Check if the given subsystem code is a vertical beam.
        /// </summary>
        /// <param name="code">Subsystem code.</param>
        /// <returns>True if subsystem code is for a vertical beam.</returns>
        public static bool IsVerticalBeam(byte code)
        {
            switch (code)
            {
                case SUB_2MHZ_VERT_PISTON_9:
                case SUB_1_2MHZ_VERT_PISTON_A:
                case SUB_600KHZ_VERT_PISTON_B:
                case SUB_300KHZ_VERT_PISTON_C:
                case SUB_150KHZ_VERT_PISTON_D:
                case SUB_75KHZ_VERT_PISTON_E:
                case SUB_38KHZ_VERT_PISTON_F:
                case SUB_20KHZ_VERT_PISTON_G:
                    return true;
                default:
                    return false;
            }

        }

        /// <summary>
        /// Determine if the 2 ids given are the equal.
        /// </summary>
        /// <param name="code1">First subsystem to check.</param>
        /// <param name="code2">SubSystem to check against.</param>
        /// <returns>True if there codes match.</returns>
        public static bool operator ==(Subsystem code1, Subsystem code2)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(code1, code2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)code1 == null) || ((object)code2 == null))
            {
                return false;
            }

            // Return true if the fields match:
            // Only 1 code is possible per system
            // It is not possible to have the same subsystem 1 ADCP
            return (code1.Code == code2.Code);
            //return (code1.Code == code2.Code) && (code1.Index == code2.Index);
        }

        /// <summary>
        /// Return the opposite of ==.
        /// </summary>
        /// <param name="code1">First sub-system to check.</param>
        /// <param name="code2">Sub-system to check against.</param>
        /// <returns>Return the opposite of ==.</returns>
        public static bool operator !=(Subsystem code1, Subsystem code2)
        {
            return !(code1 == code2);
        }

        /// <summary>
        /// Create a hashcode based off the Code stored.
        /// </summary>
        /// <returns>Hash the Code.</returns>
        public override int GetHashCode()
        {
            return Code.GetHashCode();
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
            
            Subsystem p = (Subsystem)obj;

            return (Code == p.Code);
            //return (Code == p.Code) && (Index == p.Index);
        }

        #region Convert String or Char to Decimal for SubsystemCode

        /// <summary>
        /// Convert the given string index to a subsystem code decimal value.
        /// Usually the serial number is given as a string.  This will convert
        /// the serial number subsystem string to its decimal value which
        /// is stored in the ensemble as a byte.
        /// </summary>
        /// <param name="value">String containing the subsystem code.</param>
        /// <param name="index">Index within the value to convert.</param>
        /// <returns>Char converted to its decimal byte value.</returns>
        public static byte ConvertSubsystemCode(string value, int index)
        {
            decimal subsysCode = Convert.ToChar(value.Substring(index, 1));
            return (byte)subsysCode;
        }

        /// <summary>
        /// Convert the given char to a subsystem code decimal value.
        /// Usually the serial number is given as a string.  This will
        /// convert the serial number subsystem string char to its 
        /// decimal value which is stored in the ensemble as a byte.
        /// </summary>
        /// <param name="value">Char which is the subsystem code.</param>
        /// <returns>Decimal value for the char value given.</returns>
        public static byte ConvertSubsystemCode(char value)
        {
            return (byte)System.Convert.ToUInt32(value);
        }

        #endregion
    }

            /// <summary>
        /// Information about the subsystem.
        /// </summary>
        public class SubsystemInfo
        {
            /// <summary>
            /// Subsystem.
            /// </summary>
            public Subsystem Subsystem { get; set; }

            /// <summary>
            /// Beam Angle.
            /// </summary>
            public RTI.Subsystem.BeamAngles BeamAngle { get; set; }

            /// <summary>
            /// Frequency.
            /// </summary>
            public RTI.Subsystem.SystemFrequency Frequency { get; set; }

            /// <summary>
            /// Number of beams.
            /// </summary>
            public int NumBeams { get; set; }

            /// <summary>
            /// Flag if the subsystem is a vertical beam.
            /// </summary>
            public bool IsVerticalBeam { get; set; }

            /// <summary>
            /// TRUE if this is the Primary set of beams.  The primary beams are the first set of beams in the configuration.
            /// The Offset beams are the secondary set of beams.  Offset by 45 degrees typically.
            /// </summary>
            public bool IsPrimaryBeams { get; set; }

            /// <summary>
            /// Offset angle of the beams.
            /// </summary>
            public double OffsetAngle { get; set; }

            /// <summary>
            /// Flag if the system is an array.
            /// </summary>
            public bool Array { get; set; }

            /// <summary>
            /// Set if the Head is an opposite facing head.
            /// </summary>
            public bool IsOppositeFacing { get; set; }

            /// <summary>
            /// Default setup.
            /// </summary>
            public SubsystemInfo()
            {
                BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_20_Degree;
                Frequency = RTI.Subsystem.SystemFrequency.Freq_300kHz;
                NumBeams = 4;
                IsVerticalBeam = false;
                IsPrimaryBeams = true;
                OffsetAngle = 0;
                Array = false;
                IsOppositeFacing = false;
            }

            /// <summary>
            /// Initialize the values based off the Subsystems.
            /// </summary>
            /// <param name="ss">Subsystem.</param>
            public SubsystemInfo(Subsystem ss)
            {
                var ssInfo = GetSubystemInfo(ss);

                this.Subsystem = ssInfo.Subsystem;
                this.BeamAngle = ssInfo.BeamAngle;
                this.Frequency = ssInfo.Frequency;
                this.NumBeams = ssInfo.NumBeams;
                this.IsVerticalBeam = ssInfo.IsVerticalBeam;
                this.IsPrimaryBeams = ssInfo.IsPrimaryBeams;
                this.OffsetAngle = ssInfo.OffsetAngle;
                this.Array = ssInfo.Array;
                this.IsOppositeFacing = false;
            }

            /// <summary>
            /// Get the Subsystem info.  This will tell about the system type.
            /// The number of beams, the offset, the beam angle and if its a vertical beam.
            /// </summary>
            /// <param name="ss">Subsystem.</param>
            /// <returns>Subsystem information.</returns>
            public SubsystemInfo GetSubystemInfo(Subsystem ss)
            {
                switch(ss.Code)
                {
                    case RTI.Subsystem.SUB_2MHZ_4BEAM_20DEG_PISTON_1:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_20_Degree,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_2000kHz,
                            NumBeams = 4,
                            IsVerticalBeam = false,
                            IsPrimaryBeams = true,
                            OffsetAngle = 0,
                            Array = false,
                        };
                    case RTI.Subsystem.SUB_1_2MHZ_4BEAM_20DEG_PISTON_2:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_20_Degree,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_1200kHz,
                            NumBeams = 4,
                            IsVerticalBeam = false,
                            IsPrimaryBeams = true,
                            OffsetAngle = 0,
                            Array = false,
                        };
                    case RTI.Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_20_Degree,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_600kHz,
                            NumBeams = 4,
                            IsVerticalBeam = false,
                            IsPrimaryBeams = true,
                            OffsetAngle = 0,
                            Array = false,
                        };
                    case RTI.Subsystem.SUB_300KHZ_4BEAM_20DEG_PISTON_4:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_20_Degree,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_300kHz,
                            NumBeams = 4,
                            IsVerticalBeam = false,
                            IsPrimaryBeams = true,
                            OffsetAngle = 0,
                            Array = false,
                        };
                    case RTI.Subsystem.SUB_2MHZ_4BEAM_20DEG_PISTON_45OFFSET_5:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_20_Degree,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_2000kHz,
                            NumBeams = 4,
                            IsVerticalBeam = false,
                            IsPrimaryBeams = false,
                            OffsetAngle = 45.0,
                            Array = false,
                        };
                    case RTI.Subsystem.SUB_1_2MHZ_4BEAM_20DEG_PISTON_45OFFSET_6:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_20_Degree,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_1200kHz,
                            NumBeams = 4,
                            IsVerticalBeam = false,
                            IsPrimaryBeams = false,
                            OffsetAngle = 45.0,
                            Array = false,
                        };
                    case RTI.Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_45OFFSET_7:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_20_Degree,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_600kHz,
                            NumBeams = 4,
                            IsVerticalBeam = false,
                            IsPrimaryBeams = false,
                            OffsetAngle = 45.0,
                            Array = false,
                        };
                    case RTI.Subsystem.SUB_300KHZ_4BEAM_20DEG_PISTON_45OFFSET_8:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_20_Degree,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_300kHz,
                            NumBeams = 4,
                            IsVerticalBeam = false,
                            IsPrimaryBeams = false,
                            OffsetAngle = 45.0,
                            Array = false,
                        };
                    case RTI.Subsystem.SUB_2MHZ_VERT_PISTON_9:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_Vertical,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_2000kHz,
                            NumBeams = 1,
                            IsVerticalBeam = true,
                            IsPrimaryBeams = false,
                            OffsetAngle = 0.0,
                            Array = false,
                        };
                    case RTI.Subsystem.SUB_1_2MHZ_VERT_PISTON_A:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_Vertical,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_1200kHz,
                            NumBeams = 1,
                            IsVerticalBeam = true,
                            IsPrimaryBeams = false,
                            OffsetAngle = 0.0,
                            Array = false,
                        };
                    case RTI.Subsystem.SUB_600KHZ_VERT_PISTON_B:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_Vertical,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_600kHz,
                            NumBeams = 1,
                            IsVerticalBeam = true,
                            IsPrimaryBeams = false,
                            OffsetAngle = 0.0,
                            Array = false,
                        };
                    case RTI.Subsystem.SUB_300KHZ_VERT_PISTON_C:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_Vertical,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_300kHz,
                            NumBeams = 1,
                            IsVerticalBeam = true,
                            IsPrimaryBeams = false,
                            OffsetAngle = 0.0,
                            Array = false,
                        };
                    case RTI.Subsystem.SUB_150KHZ_VERT_PISTON_D:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_Vertical,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_150kHz,
                            NumBeams = 1,
                            IsVerticalBeam = true,
                            IsPrimaryBeams = false,
                            OffsetAngle = 0.0,
                            Array = false,
                        };
                    case RTI.Subsystem.SUB_75KHZ_VERT_PISTON_E:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_Vertical,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_75kHz,
                            NumBeams = 1,
                            IsVerticalBeam = true,
                            IsPrimaryBeams = false,
                            OffsetAngle = 0.0,
                            Array = false,
                        };
                    case RTI.Subsystem.SUB_38KHZ_VERT_PISTON_F:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_Vertical,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_38kHz,
                            NumBeams = 1,
                            IsVerticalBeam = true,
                            IsPrimaryBeams = false,
                            OffsetAngle = 0.0,
                            Array = false,
                        };
                    case RTI.Subsystem.SUB_20KHZ_VERT_PISTON_G:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_Vertical,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_20kHz,
                            NumBeams = 1,
                            IsVerticalBeam = true,
                            IsPrimaryBeams = false,
                            OffsetAngle = 0.0,
                            Array = false,
                        };
                    case RTI.Subsystem.SUB_600KHZ_4BEAM_30DEG_ARRAY_I:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_30_Degree,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_600kHz,
                            NumBeams = 4,
                            IsVerticalBeam = false,
                            IsPrimaryBeams = true,
                            OffsetAngle = 0.0,
                            Array = true,
                        };
                    case RTI.Subsystem.SUB_300KHZ_4BEAM_30DEG_ARRAY_J:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_30_Degree,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_300kHz,
                            NumBeams = 4,
                            IsVerticalBeam = false,
                            IsPrimaryBeams = true,
                            OffsetAngle = 0.0,
                            Array = true,
                        };
                    case RTI.Subsystem.SUB_150KHZ_4BEAM_30DEG_ARRAY_K:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_30_Degree,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_150kHz,
                            NumBeams = 4,
                            IsVerticalBeam = false,
                            IsPrimaryBeams = true,
                            OffsetAngle = 0.0,
                            Array = true,
                        };
                    case RTI.Subsystem.SUB_75KHZ_4BEAM_30DEG_ARRAY_L:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_30_Degree,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_75kHz,
                            NumBeams = 4,
                            IsVerticalBeam = false,
                            IsPrimaryBeams = true,
                            OffsetAngle = 0.0,
                            Array = true,
                        };
                    case RTI.Subsystem.SUB_38KHZ_4BEAM_30DEG_ARRAY_M:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_30_Degree,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_38kHz,
                            NumBeams = 4,
                            IsVerticalBeam = false,
                            IsPrimaryBeams = true,
                            OffsetAngle = 0.0,
                            Array = true,
                        };
                    case RTI.Subsystem.SUB_20KHZ_4BEAM_30DEG_ARRAY_N:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_30_Degree,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_20kHz,
                            NumBeams = 4,
                            IsVerticalBeam = false,
                            IsPrimaryBeams = true,
                            OffsetAngle = 0.0,
                            Array = true,
                        };
                    case RTI.Subsystem.SUB_600KHZ_4BEAM_15DEG_ARRAY_O:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_15_Degree,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_600kHz,
                            NumBeams = 4,
                            IsVerticalBeam = false,
                            IsPrimaryBeams = true,
                            OffsetAngle = 0.0,
                            Array = true,
                        };
                    case RTI.Subsystem.SUB_300KHZ_4BEAM_15DEG_ARRAY_P:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_15_Degree,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_300kHz,
                            NumBeams = 4,
                            IsVerticalBeam = false,
                            IsPrimaryBeams = true,
                            OffsetAngle = 0.0,
                            Array = true,
                        };
                    case RTI.Subsystem.SUB_150KHZ_4BEAM_15DEG_ARRAY_Q:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_15_Degree,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_150kHz,
                            NumBeams = 4,
                            IsVerticalBeam = false,
                            IsPrimaryBeams = true,
                            OffsetAngle = 0.0,
                            Array = true,
                        };
                    case RTI.Subsystem.SUB_75KHZ_4BEAM_15DEG_ARRAY_R:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_15_Degree,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_75kHz,
                            NumBeams = 4,
                            IsVerticalBeam = false,
                            IsPrimaryBeams = true,
                            OffsetAngle = 0.0,
                            Array = true,
                        };
                    case RTI.Subsystem.SUB_38KHZ_4BEAM_15DEG_ARRAY_S:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_15_Degree,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_38kHz,
                            NumBeams = 4,
                            IsVerticalBeam = false,
                            IsPrimaryBeams = true,
                            OffsetAngle = 0.0,
                            Array = true,
                        };
                    case RTI.Subsystem.SUB_20KHZ_4BEAM_15DEG_ARRAY_T:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_15_Degree,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_20kHz,
                            NumBeams = 4,
                            IsVerticalBeam = false,
                            IsPrimaryBeams = true,
                            OffsetAngle = 0.0,
                            Array = true,
                        };
                    case RTI.Subsystem.SUB_600KHZ_1BEAM_0DEG_ARRAY_U:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_Vertical,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_600kHz,
                            NumBeams = 1,
                            IsVerticalBeam = false,
                            IsPrimaryBeams = false,
                            OffsetAngle = 0.0,
                            Array = true,
                        };
                    case RTI.Subsystem.SUB_300KHZ_1BEAM_0DEG_ARRAY_V:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_Vertical,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_300kHz,
                            NumBeams = 1,
                            IsVerticalBeam = false,
                            IsPrimaryBeams = false,
                            OffsetAngle = 0.0,
                            Array = true,
                        };
                    case RTI.Subsystem.SUB_150KHZ_1BEAM_0DEG_ARRAY_W:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_Vertical,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_150kHz,
                            NumBeams = 1,
                            IsVerticalBeam = false,
                            IsPrimaryBeams = false,
                            OffsetAngle = 0.0,
                            Array = true,
                        };
                    case RTI.Subsystem.SUB_75KHZ_1BEAM_0DEG_ARRAY_X:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_Vertical,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_75kHz,
                            NumBeams = 1,
                            IsVerticalBeam = false,
                            IsPrimaryBeams = false,
                            OffsetAngle = 0.0,
                            Array = true,
                        };
                    case RTI.Subsystem.SUB_38KHZ_1BEAM_0DEG_ARRAY_Y:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_Vertical,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_38kHz,
                            NumBeams = 1,
                            IsVerticalBeam = false,
                            IsPrimaryBeams = false,
                            OffsetAngle = 0.0,
                            Array = true,
                        };
                    case RTI.Subsystem.SUB_20KHZ_1BEAM_0DEG_ARRAY_Z:
                        return new SubsystemInfo()
                        {
                            Subsystem = ss,
                            BeamAngle = RTI.Subsystem.BeamAngles.BeamAngle_Vertical,
                            Frequency = RTI.Subsystem.SystemFrequency.Freq_20kHz,
                            NumBeams = 1,
                            IsVerticalBeam = false,
                            IsPrimaryBeams = false,
                            OffsetAngle = 0.0,
                            Array = true,
                        };
                    default:
                        return new SubsystemInfo();
                }
            }

        }


    /// <summary>
    /// Create a list of subsystem with there description.
    /// </summary>
    public class SubsystemList : ObservableCollection<RTI.SubsystemList.SubsystemCodeDesc>
    {
        /// <summary>
        /// Add all the subsystems and descriptions to this object.
        /// </summary>
        public SubsystemList()
            : base()
        {
            Add(new SubsystemCodeDesc(Subsystem.SUB_2MHZ_4BEAM_20DEG_PISTON_1));
            Add(new SubsystemCodeDesc(Subsystem.SUB_1_2MHZ_4BEAM_20DEG_PISTON_2));
            Add(new SubsystemCodeDesc(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3));
            Add(new SubsystemCodeDesc(Subsystem.SUB_300KHZ_4BEAM_20DEG_PISTON_4));
            Add(new SubsystemCodeDesc(Subsystem.SUB_2MHZ_4BEAM_20DEG_PISTON_45OFFSET_5));
            Add(new SubsystemCodeDesc(Subsystem.SUB_1_2MHZ_4BEAM_20DEG_PISTON_45OFFSET_6));
            Add(new SubsystemCodeDesc(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_45OFFSET_7));
            Add(new SubsystemCodeDesc(Subsystem.SUB_300KHZ_4BEAM_20DEG_PISTON_45OFFSET_8));
            Add(new SubsystemCodeDesc(Subsystem.SUB_2MHZ_VERT_PISTON_9));
            Add(new SubsystemCodeDesc(Subsystem.SUB_1_2MHZ_VERT_PISTON_A));
            Add(new SubsystemCodeDesc(Subsystem.SUB_600KHZ_VERT_PISTON_B));
            Add(new SubsystemCodeDesc(Subsystem.SUB_300KHZ_VERT_PISTON_C));
            Add(new SubsystemCodeDesc(Subsystem.SUB_150KHZ_VERT_PISTON_D));
            Add(new SubsystemCodeDesc(Subsystem.SUB_75KHZ_VERT_PISTON_E));
            Add(new SubsystemCodeDesc(Subsystem.SUB_38KHZ_VERT_PISTON_F));
            Add(new SubsystemCodeDesc(Subsystem.SUB_20KHZ_VERT_PISTON_G));
            Add(new SubsystemCodeDesc(Subsystem.SUB_600KHZ_4BEAM_30DEG_ARRAY_I));
            Add(new SubsystemCodeDesc(Subsystem.SUB_300KHZ_4BEAM_30DEG_ARRAY_J));
            Add(new SubsystemCodeDesc(Subsystem.SUB_150KHZ_4BEAM_30DEG_ARRAY_K));
            Add(new SubsystemCodeDesc(Subsystem.SUB_75KHZ_4BEAM_30DEG_ARRAY_L));
            Add(new SubsystemCodeDesc(Subsystem.SUB_38KHZ_4BEAM_30DEG_ARRAY_M));
            Add(new SubsystemCodeDesc(Subsystem.SUB_20KHZ_4BEAM_30DEG_ARRAY_N));
            Add(new SubsystemCodeDesc(Subsystem.SUB_600KHZ_4BEAM_15DEG_ARRAY_O));
            Add(new SubsystemCodeDesc(Subsystem.SUB_300KHZ_4BEAM_15DEG_ARRAY_P));
            Add(new SubsystemCodeDesc(Subsystem.SUB_150KHZ_4BEAM_15DEG_ARRAY_Q));
            Add(new SubsystemCodeDesc(Subsystem.SUB_75KHZ_4BEAM_15DEG_ARRAY_R));
            Add(new SubsystemCodeDesc(Subsystem.SUB_38KHZ_4BEAM_15DEG_ARRAY_S));
            Add(new SubsystemCodeDesc(Subsystem.SUB_20KHZ_4BEAM_15DEG_ARRAY_T));
            Add(new SubsystemCodeDesc(Subsystem.SUB_600KHZ_1BEAM_0DEG_ARRAY_U));
            Add(new SubsystemCodeDesc(Subsystem.SUB_300KHZ_1BEAM_0DEG_ARRAY_V));
            Add(new SubsystemCodeDesc(Subsystem.SUB_150KHZ_1BEAM_0DEG_ARRAY_W));
            Add(new SubsystemCodeDesc(Subsystem.SUB_75KHZ_1BEAM_0DEG_ARRAY_X));
            Add(new SubsystemCodeDesc(Subsystem.SUB_38KHZ_1BEAM_0DEG_ARRAY_Y));
            Add(new SubsystemCodeDesc(Subsystem.SUB_20KHZ_1BEAM_0DEG_ARRAY_Z));
        }

        /// <summary>
        /// Object to hold the subsystem code and description.
        /// </summary>
        public class SubsystemCodeDesc
        {
            /// <summary>
            /// Add the Code and description for the subsystem.
            /// </summary>
            /// <param name="code">Subsystem code.</param>
            /// <param name="desc">Subsystem description.</param>
            public SubsystemCodeDesc(byte code, string desc)
            {
                Code = code;
                Desc = desc;
            }

            /// <summary>
            /// Set the code and get the desciption based off
            /// the code.
            /// </summary>
            /// <param name="code">Subsystem code.</param>
            public SubsystemCodeDesc(byte code)
            {
                Code = code;
                Desc = Subsystem.DescString(code);
            }

            /// <summary>
            /// Subsystem Code.
            /// </summary>
            public byte Code { get; set; }

            /// <summary>
            /// Subsystem Description.
            /// </summary>
            public string Desc { get; set; }

            /// <summary>
            /// Return the description as the string for this object.
            /// </summary>
            /// <returns>Return the description as the string for this object.</returns>
            public override string ToString()
            {
                return string.Format("{0}\t{1}", Convert.ToString(Convert.ToChar(Code)), Desc);
            }

            /// <summary>
            /// Determine if the 2 ids given are the equal.
            /// </summary>
            /// <param name="code1">First subsystem to check.</param>
            /// <param name="code2">SubSystem to check against.</param>
            /// <returns>True if there codes match.</returns>
            public static bool operator ==(SubsystemCodeDesc code1, SubsystemCodeDesc code2)
            {
                // If both are null, or both are same instance, return true.
                if (System.Object.ReferenceEquals(code1, code2))
                {
                    return true;
                }

                // If one is null, but not both, return false.
                if (((object)code1 == null) || ((object)code2 == null))
                {
                    return false;
                }

                // Return true if the fields match:
                // Only 1 code is possible per system
                // It is not possible to have the same subsystem 1 ADCP
                return (code1.Code == code2.Code);
                //return (code1.Code == code2.Code) && (code1.Index == code2.Index);
            }

            /// <summary>
            /// Return the opposite of ==.
            /// </summary>
            /// <param name="code1">First sub-system to check.</param>
            /// <param name="code2">Sub-system to check against.</param>
            /// <returns>Return the opposite of ==.</returns>
            public static bool operator !=(SubsystemCodeDesc code1, SubsystemCodeDesc code2)
            {
                return !(code1 == code2);
            }

            /// <summary>
            /// Create a hashcode based off the Code stored.
            /// </summary>
            /// <returns>Hash the Code.</returns>
            public override int GetHashCode()
            {
                return Code.GetHashCode();
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

                SubsystemCodeDesc p = (SubsystemCodeDesc)obj;

                return (Code == p.Code);
                //return (Code == p.Code) && (Index == p.Index);
            }

        }

    }

#region JSON

    /// <summary>
    /// Convert this object to a JSON object.
    /// Calling this method is twice as fast as calling the default serializer:
    /// Newtonsoft.Json.JsonConvert.SerializeObject(subsystem).
    /// 
    /// 50ms for this method.
    /// 100ms for calling SerializeObject default.
    /// 
    /// Use this method whenever possible to convert to JSON.
    /// 
    /// http://james.newtonking.com/projects/json/help/
    /// http://james.newtonking.com/projects/json/help/index.html?topic=html/ReadingWritingJSON.htm
    /// http://blog.maskalik.com/asp-net/json-net-implement-custom-serialization
    /// </summary>
    public class SubsystemSerializer : JsonConverter
    {
        #region Variables

        /// <summary>
        /// JSON object name for Subsystem Index.
        /// </summary>
        public const string JSON_STR_SUBSYSTEM_INDEX = "SubsystemIndex";

        /// <summary>
        /// JSON object name for Subsystem Code.
        /// </summary>
        public const string JSON_STR_SUBSYSTEM_CODE = "SubsystemCode";

        #endregion

        /// <summary>
        /// Write the JSON string.  This will convert all the properties to a JSON string.
        /// This is done manaully to improve conversion time.  The default serializer will check
        /// each property if it can convert.  This will convert the properties automatically.  This
        /// will double the speed.
        /// 
        /// Newtonsoft.Json.JsonConvert.SerializeObject(subSystem).
        /// 
        /// </summary>
        /// <param name="writer">JSON Writer.</param>
        /// <param name="value">Object to write to JSON.</param>
        /// <param name="serializer">Serializer to convert the object.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Cast the object
            var data = value as Subsystem;

            // Start the object
            writer.Formatting = Formatting.None;                    // Make the text not indented, so not as human readable.  This will save disk space
            writer.WriteStartObject();                              // Start the JSON object

            // Subsystem Index
            writer.WritePropertyName(JSON_STR_SUBSYSTEM_INDEX);     // Subsystem Index name
            writer.WriteValue(data.Index);                // Subsystem Index value

            // Subsystem Index
            writer.WritePropertyName(JSON_STR_SUBSYSTEM_CODE);      // Subsystem Index name
            writer.WriteValue(data.Code);                 // Subsystem Index value

            // End the object
            writer.WriteEndObject();
        }

        /// <summary>
        /// Read the JSON object and convert to the object.  This will allow the serializer to
        /// automatically convert the object.  No special instructions need to be done and all
        /// the properties found in the JSON string need to be used.
        /// 
        /// Subsystem ss = Newtonsoft.Json.JsonConvert.DeserializeObject{Subsystem}(encodedEns)
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

                // Subsystem Index
                ushort subsystem_index = (ushort)jsonObject[JSON_STR_SUBSYSTEM_INDEX];

                // Subsystem Code
                byte subsystem_code = jsonObject[JSON_STR_SUBSYSTEM_CODE].ToObject<byte>();

                return new Subsystem(subsystem_code, subsystem_index);
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
            return typeof(Subsystem).IsAssignableFrom(objectType);
        }
    }

#endregion
}