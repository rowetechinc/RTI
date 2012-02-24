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
 *
 */

using System;
namespace RTI
{
    /// <summary>
    /// Codes are ASCII codes.
    /// Codes are stored as ASCII codes.
    /// </summary>
    public class Subsystem
    {
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

        #endregion

        #region Properties

        /// <summary>
        /// There can be duplicate subsystem
        /// ID within a serial number.  This will
        /// differeniate the Subsystem, by its location
        /// within the serial number.
        /// </summary>
        public UInt16 Index { get; private set; }

        /// <summary>
        /// Sub-System code.
        /// This code represents the system configuration
        /// for the ensemble given.  The Subsystem ID
        /// can be found in the RTI ADCP User Guide.
        /// </summary>
        public byte Code { get; private set; }

        #endregion

        /// <summary>
        /// Represents an empty or blank subsystem.
        /// 
        /// All SETs are set private to ensure this
        /// value does not change.
        /// </summary>
        public static readonly Subsystem Empty = new Subsystem();

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
        public Subsystem(byte code, UInt16 index)
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
                Code = (byte)System.Convert.ToUInt32(code[0]);
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
        /// Create a string for this object.
        /// The string will contain the code and index.
        /// INDEX_CODE
        /// </summary>
        /// <returns>Code and index of this object as a string.</returns>
        public override string ToString()
        {
            return string.Format("{0}_{1}", Index.ToString(), CodeToString());
        }

        /// <summary>
        /// Return the string version of the code.
        /// This will conver the code from a hex byte value to 
        /// a string character.
        /// </summary>
        /// <returns></returns>
        public string CodeToString()
        {
            return Convert.ToString(Convert.ToChar(Code));
        }

        /// <summary>
        /// Convert the code into a string.
        /// </summary>
        /// <returns>String representing the code.</returns>
        public string DescString()
        {
            switch(Code)
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
            //return (code1.Code == code2.Code);
            return (code1.Code == code2.Code) && (code1.Index == code2.Index);
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

            //return (Code == p.Code);
            return (Code == p.Code) && (Index == p.Index);
        }
    }
}