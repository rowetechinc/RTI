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
 * 09/01/2011      RC          Initial coding
 *
 *       
 * 
 */

namespace RTI
{
    namespace DataSet
    {
        /// <summary>
        /// Struct used to create a list of Bin entries.
        /// This object will contain all the data for each
        /// Bin.  It is assumed there is 4 beams for each Bin
        /// with this struct.
        /// </summary>
        /// <typeparam name="T">Type is usually Float or Integer</typeparam>
        public class BinEntry<T>
        {
            /// <summary>
            /// Bin number.
            /// </summary>
            public int Bin { get; set; }

            /// <summary>
            /// Value for the given Beam 1 Bin.
            /// </summary>
            public T Beam1 { get; set; }

            /// <summary>
            /// Value for the given Beam 2 Bin.
            /// </summary>
            public T Beam2 { get; set; }

            /// <summary>
            /// Value for the given Beam 3 Bin.
            /// </summary>
            public T Beam3 { get; set; }

            /// <summary>
            /// Value for the given Beam 4 Bin
            /// </summary>
            public T Beam4 { get; set; }

            /// <summary>
            /// Constructor
            /// 
            /// Set the Bin value entries.
            /// </summary>
            /// <param name="bin">Bin number</param>
            /// <param name="beam1">Beam 1 value.</param>
            /// <param name="beam2">Beam 1 value.</param>
            /// <param name="beam3">Beam 1 value.</param>
            /// <param name="beam4">Beam 1 value.</param>
            public BinEntry(int bin, T beam1, T beam2, T beam3, T beam4)
            {
                Bin = bin;
                Beam1 = beam1;
                Beam2 = beam2;
                Beam3 = beam3;
                Beam4 = beam4;
            }
        }
    }
}