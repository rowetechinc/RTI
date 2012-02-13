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
 * 01/17/2012      RC          1.14       Initial coding
 * 
 */


using System.Collections.Generic;
namespace RTI
{

    namespace Screen
    {
        /// <summary>
        /// Reference Layer Averaging.
        /// Choose a reference layer.  A reference layer is a layer 
        /// in the ensemble made up of 1 or more bins where you believe 
        /// the value measured can be trusted.  There should not be any 
        /// none issues within the layer.  Issues can include wind variation or
        /// depth changing or ...
        /// 
        /// We will then subtract out the reference layer speed from every bin
        /// in the ensemble.  
        /// (Ens1 - RefLayer1)
        /// 
        /// We then accumulate ensembles with the reference layer subtracted out and
        /// the reference layer speed.
        /// (Ens1 - RefLayer1) --> accumEns
        /// RefLayer1          --> accumRefLayer
        /// 
        /// After X number of ensembles have been accummulated, we take an average of
        /// the ensembles.  To calculate the average we will take the average of the 
        /// accumulated reference layer speed and added to the average of the ensembles.
        /// 
        /// To calculate the average ensemble, we will average the speed of each bin together.
        /// (AccumRFLSpeed/X) + (AccumEns/X)
        /// 
        /// Reference layer averaging will not only average ensembles together it will also
        /// fill in data where it is missing between ensembles.  If one ensemble only got good
        /// data from bins 1 through 12 and the second ensemble got good data from bins 1 through
        /// 23, the average of the two ensembles would fill in some data for the missing data in 
        /// ensemble one bin's 13 - 23.  With more samples, the data would look cleaner for the
        /// missing data.
        /// </summary>
        public class ReferenceLayerAverage
        {
            /// <summary>
            /// List to hold the accumulating ensembles to average
            /// together.  These ensembles will have the reference
            /// layer average removed already.
            /// </summary>
            private LinkedList<DataSet.Ensemble> _accumEns;

            /// <summary>
            /// List to hold the accumulating reference layer averages.
            /// </summary>
            private LinkedList<double> _accumRefLayerAvg;

            /// <summary>
            /// Number of samples to accumulate before
            /// averaging the data.
            /// </summary>
            private int _numSamples;

            /// <summary>
            /// Constructor:
            /// Average the ensembles using reference layer.
            /// Give the number of samples before averaging.
            /// Set the reference layer later.
            /// </summary>
            /// <param name="numSamples">Number of samples to average.</param>
            public ReferenceLayerAverage(int numSamples)
            {
                // Initialize values
                _numSamples = numSamples;
                _accumEns = new LinkedList<DataSet.Ensemble>();
                _accumRefLayerAvg = new LinkedList<double>();
            }

            /// <summary>
            /// Add an ensemble to the accumulator.
            /// Calculate the reference layer and add it
            /// to the accumulator.
            /// </summary>
            /// <param name="ensemble">Ensemble to add to average.</param>
            public void AddEnsemble(DataSet.Ensemble ensemble)
            {
                // Get the reference layer average

                // Remove the reference layer average from all the bins

                // Add the ensemble to the accum
 
                // Check the accum size to see if time to average data
            }

            /// <summary>
            /// Clear the accumulators.
            /// If the averaging needs to be restarted,
            /// then clear the accumulators of data.
            /// </summary>
            public void Clear()
            {
                // Clear the accumulators
                _accumEns.Clear();
                _accumRefLayerAvg.Clear();
            }

            private void CalculateRefLayer( )
            {

            }

        }

    }

}
