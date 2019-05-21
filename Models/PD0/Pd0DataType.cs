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
 * 05/06/2019      RC          3.4.11     Fixed code to handle any number of beams
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
    /// The Base data type for all PD0 data types.
    /// 
    /// This will include the PD0 ID and type.
    /// </summary>
    public abstract class Pd0DataType
    {
        #region Properties

        /// <summary>
        /// PD0 ID.
        /// </summary>
        public Pd0ID ID { get; set; }

        /// <summary>
        /// Used for decoding and encoding to PD0 Binary format.
        /// </summary>
        public ushort Offset { get; set; }

        /// <summary>
        /// Number of beams.
        /// </summary>
        public int NumBeams { get; set; }

        /// <summary>
        /// Number of depth cells.
        /// </summary>
        public int NumDepthCells { get; set; }

        #endregion

        /// <summary>
        /// Initialize the PD0 Data Type.
        /// </summary>
        /// <param name="lsb">LSB ID.</param>
        /// <param name="msb">MSB ID.</param>
        /// <param name="type">PD0 Data Type.</param>
        public Pd0DataType(byte lsb, byte msb, Pd0ID.Pd0Types type)
        {
            // Set the ID
            ID = new Pd0ID(lsb, msb, type);
            Offset = 0;
        }

        /// <summary>
        /// Encode the data to PD0 binary format.
        /// </summary>
        /// <returns>Binary data for the data type.</returns>
        public abstract byte[] Encode();

        /// <summary>
        /// Get the data type size.
        /// </summary>
        /// <returns>Size of the data type.</returns>
        public abstract int GetDataTypeSize();

        /// <summary>
        /// Number of bytes in a depth cell.
        /// n Beams per depth cell.
        /// 1 or 2 Bytes per beam.
        /// </summary>
        /// <returns></returns>
        public int BytesPerDepthCell()
        {
            switch (ID.Type)
            {
                case Pd0ID.Pd0Types.Velocity:
                    return 2 * NumBeams;                    // 2 Bytes per beam
                default:
                    return 1 * NumBeams;                    // 1 Byte per beam
            }
        }
    }
}
