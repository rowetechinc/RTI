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
 * 05/11/2016      RC          3.3.2       Initial coding
 * 
 */


using RTI.DataSet;
using System;
namespace RTI
{
    namespace ScreenData
    {
        /// <summary>
        /// Use the given previous heading value if a bad heading value is found.
        /// 
        /// </summary>
        public class ScreenBadHeading
        {
            /// <summary>
            /// Apply the previous heading if the current heading is bad.
            /// </summary>
            /// <param name="ensemble">Ensemble to screen.</param>
            /// <param name="prevHeading">Previous good heading value.</param>
            /// <returns>TRUE if screening occured.</returns>
            public static bool Screen(ref DataSet.Ensemble ensemble, float prevHeading)
            {
                // Make sure the previous value is worth using
                if(prevHeading != 0.0f)
                {
                    // Ancillary
                    if(ensemble.IsAncillaryAvail)
                    {
                        if(ensemble.AncillaryData.Heading == 0.0f)
                        {
                            ensemble.AncillaryData.Heading = prevHeading;
                        }
                    }
                    // Bottom Track
                    if (ensemble.IsBottomTrackAvail)
                    {
                        if (ensemble.BottomTrackData.Heading == 0.0f)
                        {
                            ensemble.BottomTrackData.Heading = prevHeading;
                        }
                    }
                }

                return true;
            }
        }
    }
}
