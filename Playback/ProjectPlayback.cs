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
 * 08/29/2014      RC          3.0.1      Initial coding
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
    /// Playback the ensembles from a project.
    /// </summary>
    public class ProjectPlayback: IPlayback
    {
        #region Variables

        /// <summary>
        /// Smallest index and default value.
        /// </summary>
        private const int MIN_INDEX = 0;

        /// <summary>
        /// Project to playback the data.
        /// </summary>
        private Project _project;

        #endregion

        #region Properties

        /// <summary>
        /// Total number of ensembles that can be played back.
        /// </summary>
        public int TotalEnsembles { get; set; }

        /// <summary>
        /// Playback Index.
        /// </summary>
        public long PlaybackIndex { get; set; }

        /// <summary>
        /// Set flag is playing back in a loop.
        /// </summary>
        public bool IsLooping { get; set; }

        #endregion

        /// <summary>
        /// Initialize the object.
        /// </summary>
        /// <param name="project">Set the project.</param>
        public ProjectPlayback(Project project)
        {
            _project = project;
            PlaybackIndex = MIN_INDEX;
            IsLooping = false;

            // Set the number of ensembles from the project.
            GetNumberOfEnsembles();
        }

        /// <summary>
        /// Dispose of the object.
        /// </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// Set forward in the project.
        /// </summary>
        /// <returns>Next ensemble in the project.</returns>
        public PlaybackArgs StepForward()
        {
            // Create the args and set the total number of ensembles
            PlaybackArgs args = new PlaybackArgs();
            args.TotalEnsembles = TotalEnsembles;

            // If looping, start over
            if (PlaybackIndex + 1 <= TotalEnsembles && IsLooping)
            {
                PlaybackIndex = MIN_INDEX;
            }
            // If a good index
            else if (PlaybackIndex + 1 <= TotalEnsembles)
            {
                args.Ensemble = GetEnsemble(++PlaybackIndex);
            }

            // Get the next ensemble
            args.Ensemble = GetEnsemble(PlaybackIndex);

            // Set the index
            args.Index = PlaybackIndex;

            return args;
        }

        /// <summary>
        /// Step backwards in the project.
        /// </summary>
        /// <returns>Previous ensemble in the project.</returns>
        public PlaybackArgs StepBackward()
        {
            // Create the args and set the total number of ensembles
            PlaybackArgs args = new PlaybackArgs();
            args.TotalEnsembles = TotalEnsembles;

            // Check if can step
            if (PlaybackIndex - 1 >= MIN_INDEX)
            {
                args.Ensemble = GetEnsemble(--PlaybackIndex);
            }
            else
            {
                // Return the previous value
                args.Ensemble = GetEnsemble(PlaybackIndex);
            }

            // Set the index
            args.Index = PlaybackIndex;

            return args;
        }

        /// <summary>
        /// Jump to a specific location in the project.
        /// If the index is outside the range, then null will be returned.
        /// </summary>
        /// <param name="index">Index to jump to in the project.</param>
        /// <returns>Ensemble based off the index given.</returns>
        public PlaybackArgs Jump(long index)
        {
            // Create the args and set the total number of ensembles
            PlaybackArgs args = new PlaybackArgs();
            args.TotalEnsembles = TotalEnsembles;
            args.Index = index;
            args.Ensemble = GetEnsemble(index);

            // Set the new playback index
            PlaybackIndex = index;

            return args;
        }

        /// <summary>
        /// Return a list of all the ensembles.
        /// </summary>
        /// <returns>List of all the ensembles in the file.</returns>
        public Cache<long, DataSet.Ensemble> GetAllEnsembles()
        {
            return _project.GetAllEnsembles();
        }

        /// <summary>
        /// Return the number of ensembles in the project.
        /// This will also reset the max number of ensembles in the project.
        /// </summary>
        /// <returns>Number of ensembles in the project.</returns>
        public int GetNumberOfEnsembles()
        {
            return TotalEnsembles = _project.GetNumberOfEnsembles();
        }

        /// <summary>
        /// Get a specific ensemble from the project, based off
        /// the index.  If the index is outside the range, then
        /// return null.
        /// </summary>
        /// <param name="index">Index to go to.</param>
        /// <returns>Ensemble at the specific location.</returns>
        private DataSet.Ensemble GetEnsemble(long index)
        {
            // Check if less than 0 or greater than max number
            if (index < 0 || index > TotalEnsembles)
            {
                return null;
            }

            return _project.GetEnsemble(index);
        }

    }
}
