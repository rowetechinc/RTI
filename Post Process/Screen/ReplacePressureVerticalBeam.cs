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
 * 12/06/2019      RC          3.4.15     Initial coding
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI
{
    namespace ScreenData
    {
        /// <summary>
        /// Replace the Pressure (Transducer Depth) value with the vertical beam data.
        /// Leave the pressure value as the original value.
        /// </summary>
        public class ReplacePressureVerticalBeam
        {
            /// <summary>
            /// Replace the Pressure (Transducer Depth) value with the vertical beam data.
            /// Leave the pressure value as the original value.
            /// 
            /// If there is no EnsembleDataSet, AncillaryDataSet and RangeTrackingDataSet, return false.
            /// If this is not a vertical beam subsystem, return True, but do nothing.
            /// </summary>
            /// <param name="ensemble">Ensemble to modify.</param>
            /// <returns>TRUE if the data was modified.</returns>
            public static bool Replace(ref DataSet.Ensemble ensemble)
            {
                // Verify the data is available
                if (ensemble.IsEnsembleAvail && ensemble.IsAncillaryAvail && ensemble.IsRangeTrackingAvail)
                {
                    // Only process vertical beam subsystems
                    byte ssCode = ensemble.EnsembleData.SubsystemConfig.SubSystem.Code;
                    if (ssCode == Subsystem.SUB_300KHZ_VERT_PISTON_C ||
                        ssCode == Subsystem.SUB_600KHZ_VERT_PISTON_B ||
                        ssCode == Subsystem.SUB_1_2MHZ_VERT_PISTON_A)
                    {
                        // Check if there is pressure data
                        if (ensemble.AncillaryData.TransducerDepth == 0.0f)
                        {
                            // No pressure data
                            return false;
                        }

                        // Get the pressure data and replace with vertical beam data
                        // This will only replace the TransducerDepth and not Pressure so you have a way to go back
                        if(ensemble.RangeTrackingData.NumBeams > 0)
                        {
                            ensemble.AncillaryData.TransducerDepth = ensemble.RangeTrackingData.Range[RTI.DataSet.Ensemble.BEAM_0_INDEX];
                            return true;
                        }

                        // There was no range tracking data
                        return false;
                    }
                    else
                    {
                        // This is not bad, but not a vertical beam subsystem
                        return true;
                    }
                }
                else
                {
                    // No pressure data or ensemble data
                    return false;
                }
            }
        }
    }
}
