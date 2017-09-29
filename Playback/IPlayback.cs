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
 * 09/03/2014      RC          3.0.1      Initial coding
 * 07/27/2015      RC          3.0.5      Added Name of file playing back.
 * 09/29/2017      RC          3.4.4      Added GetOrigDataFormat() to know the what format the data was recorded.
 *       
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI
{
    /// <summary>
    /// Interface to playback.  Use this interface
    /// to know what functions need to be called.
    /// </summary>
    public interface IPlayback: IDisposable
    {

        /// <summary>
        /// Playback Index.
        /// </summary>
        long PlaybackIndex { get; set; }

        /// <summary>
        /// Set flag is playing back in a loop.
        /// </summary>
        bool IsLooping { get; set; }

        /// <summary>
        /// Total number of ensembles that can be played back.
        /// </summary>
        int TotalEnsembles { get; set; }

        /// <summary>
        /// File or project name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Step forward when playing the next ensemble.
        /// </summary>
        /// <returns>Playback arguments including the ensemble and index.</returns>
        PlaybackArgs StepForward();

        /// <summary>
        /// Step backward when playing the next ensemble.
        /// </summary>
        /// <returns>Playback arguments including the ensemble and index.</returns>
        PlaybackArgs StepBackward();

        /// <summary>
        /// Jump the given index.  This will get the specific
        /// ensemble from the file.
        /// </summary>
        /// <param name="index">Index location of the ensemble.</param>
        /// <returns>Playback arguments including the ensemble and index.</returns>
        PlaybackArgs Jump(long index);


        /// <summary>
        /// Get all the ensembles within the file.
        /// </summary>
        /// <returns>Playback arguments including the ensemble and index.</returns>
        Cache<long, DataSet.Ensemble> GetAllEnsembles();

        /// <summary>
        /// Get the nubmer of ensembles in the file.
        /// </summary>
        /// <returns>Number of ensembles in the file.</returns>
        int GetNumberOfEnsembles();

        /// <summary>
        /// Get the original data format of the data recorded.
        /// </summary>
        /// <returns>Original data format the data was recorded.</returns>
        AdcpCodec.CodecEnum GetOrigDataFormat();
    }
}
