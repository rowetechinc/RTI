/*
 * Copyright © 2011 
 * Rowe Technology Inc.
 * All rights reserved.
 * http://www.rowetechinc.com
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification is NOT permitted.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
 * COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
 * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
 * POSSIBILITY OF SUCH DAMAGE.
 * 
 * HISTORY
 * -----------------------------------------------------------------
 * Date            Initials    Version    Comments
 * -----------------------------------------------------------------
 * 12/28/2011      RC          1.10       Initial coding.
 *       
 * 
 */


namespace RTI
{
    namespace DataSet
    {
        /// <summary>
        /// Represent the velocity data for each
        /// bin as a vector.  This will store the
        /// velocity data as magnitude and direction.
        /// Direction is given with X and Y North.
        /// </summary>
        public struct VelocityVector
        {
            /// <summary>
            /// Magnitude for the bin with 
            /// Bottom Track velocity removed.
            /// </summary>
            public double Magnitude;

            /// <summary>
            /// Direction of the bin velocity
            /// with Bottom Track velocity removed.
            /// The positive X direction is North.
            /// </summary>
            public double DirectionXNorth;

            /// <summary>
            /// Direction of the bin velocity
            /// with Bottom Track velocity removed.
            /// The positive Y direction is North.
            /// </summary>
            public double DirectionYNorth;
        }

        /// <summary>
        /// A struct to hold the velocity vectors
        /// for an ensemble.
        /// </summary>
        public struct EnsembleVelocityVectors
        {
            /// <summary>
            /// Unique ID for the ensemble.
            /// </summary>
            public DataSet.UniqueID Id;

            /// <summary>
            /// Array of vectors representing
            /// the velocities for each bin.
            /// </summary>
            public VelocityVector[] Vectors;
        }
    }
}