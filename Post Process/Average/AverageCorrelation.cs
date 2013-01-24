﻿/*
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
 * 01/02/2013      RC          2.17       Initial coding
 * 01/07/2013      RC          2.17       Put more average logic into AverageBase to reduce redundant code.
 * 
 */

namespace RTI
{

    namespace Average
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Text;

        /// <summary>
        /// Average the Correlation data.  This will take the correlation data
        /// and continuously average the data.  
        /// </summary>
        public class AverageCorrelation : AverageBase
        {

            /// <summary>
            /// Initialize the values.
            /// </summary>
            public AverageCorrelation() :
                base()
            {

            }

            /// <summary>
            /// Add the ensemble data to the accumulator.  This will accumulate all the
            /// correlation data into a list.  If it is a running average, it will remove
            /// the first item in the list as needed.
            /// </summary>
            /// <param name="ensemble">Ensemble to accumulate.</param>
            public override void AddEnsemble(DataSet.Ensemble ensemble)
            {
                // Remove the first ensemble if NumSamples has been met
                RemoveEnsemble();

                // Accumulate the data
                if (ensemble.IsCorrelationAvail)
                {
                    _accumData.Add(ensemble.CorrelationData.CorrelationData);
                }
            }

            /// <summary>
            /// Set the average correlation data to the correlation data set array.
            /// This will replace the array with an averaged array for the accumulated data.
            /// If this is not a running average, it will clear the accumulator.
            /// </summary>
            /// <param name="ensemble">Set the average data to this ensemble.</param>
            public override void SetAverage(ref DataSet.Ensemble ensemble)
            {
                if (ensemble.IsCorrelationAvail)
                {
                    ensemble.CorrelationData.CorrelationData = GetAverage();
                }

                // Clear the accum
                RemoveEnsemble();
            }

        }
    }
}
