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
 * 02/12/2014      RC          2.21.3     Initial coding
 * 12/04/2019      RC          3.4.15     Add option to retransform the data after applying the offset.
 * 
 */

namespace RTI
{
    namespace VesselMount
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Text;

        /// <summary>
        /// Set the heading values based off the options.
        /// </summary>
        public class VmTiltOffset
        {

            /// <summary>
            /// Add the Pitch and Roll offset to the Pitch and Roll.  This will take the  offset and add it to the
            /// Pitch and Roll value in the ensemble.
            /// </summary>
            /// <param name="ensemble">Ensemble to change the value.</param>
            /// <param name="options">Options to know how the change the value.</param>
            /// <param name="retransformData">When the heading, pitch or roll is modified, the data should be retransfomed so the velocity matches</param>
            public static void TiltOffset(ref DataSet.Ensemble ensemble, VesselMountOptions options, bool retransformData = true)
            {
                // Add the offset to Ancillary Pitch and Roll
                AddAncillaryTiltOffset(ref ensemble, options.PitchOffset, options.RollOffset);

                // Add the offset to the Bottom Track Pitch and Roll
                AddBottomTrackTiltOffset(ref ensemble, options.PitchOffset, options.RollOffset);

                // Retransform the data so the velocties match the pitch and roll
                if(retransformData)
                {
                    Transform.ProfileTransform(ref ensemble, AdcpCodec.CodecEnum.Binary);
                    Transform.BottomTrackTransform(ref ensemble, AdcpCodec.CodecEnum.Binary);
                }
            }

            /// <summary>
            /// Add the given offset to the Ancillary Pitch and Roll.
            /// </summary>
            /// <param name="ensemble">Ensemble to modify the heading.</param>
            /// <param name="pitchOffset">Pitch offset value to add.</param>
            /// <param name="rollOffset">Roll offset value to add.</param>
            public static void AddAncillaryTiltOffset(ref DataSet.Ensemble ensemble, float pitchOffset, float rollOffset)
            {
                if (ensemble.IsAncillaryAvail)
                {
                    ensemble.AncillaryData.Pitch += pitchOffset;
                    ensemble.AncillaryData.Roll += rollOffset;
                }
            }

            /// <summary>
            /// Add the given offset to the Bottom Track Pitch and Roll.
            /// </summary>
            /// <param name="ensemble">Ensemble to modify the heading.</param>
            /// <param name="pitchOffset">Pitch offset value to add.</param>
            /// <param name="rollOffset">Roll offset value to add.</param>
            public static void AddBottomTrackTiltOffset(ref DataSet.Ensemble ensemble, float pitchOffset, float rollOffset)
            {
                if (ensemble.IsBottomTrackAvail)
                {
                    ensemble.BottomTrackData.Pitch += pitchOffset;
                    ensemble.BottomTrackData.Roll += rollOffset;
                }
            }
        }

    }
}
