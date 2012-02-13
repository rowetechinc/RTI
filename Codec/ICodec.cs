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
 * 
 * HISTORY
 * -----------------------------------------------------------------
 * Date            Initials    Comments
 * -----------------------------------------------------------------
 * 11/30/2011      RC          Initial coding
 */

namespace RTI
{
    /// <summary>
    /// Interface for all codecs.  This is used
    /// to standardize how data will be passed to the
    /// codec and how the codec will shutdown.
    /// </summary>
    public interface ICodec
    {
        /// <summary>
        /// Receive new data.  Data will be received
        /// as bytes.  The data can then be manipulated.
        /// This method should return immediately and 
        /// either use a background worker or start a 
        /// thread to process the incoming data.
        /// </summary>
        /// <param name="data">Data to process in the codec.</param>
        void AddIncomingData(byte[] data);

        /// <summary>
        /// This is used to reset the codec.  If settings have
        /// changed, and the codec needs to be reset, this
        /// method should be used.  It should clear any buffers
        /// and reset any files.
        /// </summary>
        void ClearIncomingData();

        /// <summary>
        /// This method is used when the codec needs to be shutdown.
        /// This should handle closing any files and stopping any
        /// threads.
        /// </summary>
        void Shutdown();
    }
}