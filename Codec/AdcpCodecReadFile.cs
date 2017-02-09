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
 * 02/08/2017      RC          3.4.0      Initial coding
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI
{
    class AdcpCodecReadFile
    {
        #region Variables

        /// <summary>
        /// Binary Codec to read files.
        /// </summary>
        private AdcpBinaryCodecReadFile _binaryCodec;

        /// <summary>
        /// DVL Codec to read files.
        /// </summary>
        private AdcpDvlCodecReadFile _dvlCodec;

        #endregion

        /// <summary>
        /// Initialize the codecs.
        /// </summary>
        public AdcpCodecReadFile()
        {
            _binaryCodec = new AdcpBinaryCodecReadFile();
            _dvlCodec = new AdcpDvlCodecReadFile();
        }

        public List<DataSet.EnsemblePackage> GetEnsembles(string files)
        {
            // Check Binary Codec
            var binList = _binaryCodec.GetEnsembles(files);
            if (binList.Count > 0)
                return binList;

            // Check DVL Codec
            var dvlList = _dvlCodec.GetEnsembles(files);
            if (dvlList.Count > 0)
                return dvlList;


            return new List<DataSet.EnsemblePackage>();
        }

    }
}
