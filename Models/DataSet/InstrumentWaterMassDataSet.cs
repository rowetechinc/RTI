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
 * 12/08/2011      RC          1.09       Initial coding
 * 
 */

using System.Data;
using System;
using System.ComponentModel;
using System.Text;
namespace RTI
{
    namespace DataSet
    {
        /// <summary>
        /// Data set containing all the Water Mass Instrument Velocity data.
        /// </summary>
        public class InstrumentWaterMassDataSet : BaseDataSet
        {
            /// <summary>
            /// Number of elements within this data set
            /// </summary>
            public const int NUM_DATA_ELEMENTS = 4;

            /// <summary>
            /// Water Mass X velocity in meters per second.
            /// </summary>
            public float VelocityX { get; private set; }

            /// <summary>
            /// Water Mass Y velocity in meters per second.
            /// </summary>
            public float VelocityY { get; private set; }

            /// <summary>
            /// Water Mass Z velocity in meters per second.
            /// </summary>
            public float VelocityZ { get; private set; }

            /// <summary>
            /// Depth layer the Water Mass Velocity was taken in meters.
            /// </summary>
            public float WaterMassDepthLayer { get; private set; }

            /// <summary>
            /// This value will be used when there will be multiple transducers
            /// giving ranges.  There will be duplicate beam Bin Bin ranges and the
            /// orientation will determine which beam Bin Bin ranges to group together.
            /// 
            /// This may not be necessary and additional datatypes may be created instead
            /// for each transducer orientation.
            /// </summary>
            public BaseDataSet.BeamOrientation Orientation { get; private set; }

            /// <summary>
            /// Create an Instrument Water Mass Velocity data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="orientation">Orientation of the beams.  Default Down.</param>
            public InstrumentWaterMassDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, BeamOrientation orientation = BeamOrientation.DOWN) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize data
                VelocityX = Ensemble.EMPTY_VELOCITY;
                VelocityY = Ensemble.EMPTY_VELOCITY;
                VelocityZ = Ensemble.EMPTY_VELOCITY;
                WaterMassDepthLayer = 0;

                // Default orientation value is DOWN
                Orientation = orientation;
            }

            /// <summary>
            /// Create an Instrument Water Mass Velocity data set.  Include all the information to
            /// create the data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numBins">Number of Bin</param>
            /// <param name="numBeams">Number of beams</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            /// <param name="x">Water Mass East Velocity in m/s.</param>
            /// <param name="y">Water Mass North Velocity in m/s.</param>
            /// <param name="z">Water Mass Vertical Velocity in m/s.</param>
            /// <param name="depthLayer">Depth layer of the Water Mass measurement in meters.</param>
            /// <param name="orientation">Orientation of the beams.  Default Down.</param>
            public InstrumentWaterMassDataSet(int valueType, int numBins, int numBeams, int imag, int nameLength, string name, float x, float y, float z, float depthLayer, BeamOrientation orientation = BeamOrientation.DOWN) :
                base(valueType, numBins, numBeams, imag, nameLength, name)
            {
                // Initialize data
                VelocityX = x;
                VelocityY = y;
                VelocityZ = z;
                WaterMassDepthLayer = depthLayer;

                // Default orientation value is DOWN
                Orientation = orientation;
            }

            /// <summary>
            /// Create an Insturment Water Mass Velocity data set.  Include all the information
            /// to create the data set from the NMEA sentence.
            /// </summary>
            /// <param name="sentence">NMEA sentence containing the data.</param>
            /// <param name="orientation">Orientation of the beams.  Default Down.</param>
            public InstrumentWaterMassDataSet(Prti01Sentence sentence, BeamOrientation orientation = BeamOrientation.DOWN) :
                base(DataSet.Ensemble.DATATYPE_FLOAT, NUM_DATA_ELEMENTS, DataSet.Ensemble.DEFAULT_NUM_BEAMS_BEAM, DataSet.Ensemble.DEFAULT_IMAG, DataSet.Ensemble.DEFAULT_NAME_LENGTH, DataSet.Ensemble.WaterMassInstrumentID)
            {
                // Initialize data
                VelocityX = Convert.ToSingle(sentence.WaterMassVelX.ToMetersPerSecond().Value);
                VelocityY = Convert.ToSingle(sentence.WaterMassVelY.ToMetersPerSecond().Value);
                VelocityZ = Convert.ToSingle(sentence.WaterMassVelZ.ToMetersPerSecond().Value);
                WaterMassDepthLayer = Convert.ToSingle(sentence.WaterMassDepth.ToMeters().Value);

                // Default orientation value is DOWN
                Orientation = orientation;
            }

            /// <summary>
            /// Override the ToString to return all the velocity data as a string.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("X : " + VelocityX.ToString() + "\n");
                builder.Append("Y : " + VelocityY.ToString() + "\n");
                builder.Append("Z : " + VelocityZ.ToString() + "\n");
                builder.Append("Depth Layer : " + WaterMassDepthLayer.ToString());


                return builder.ToString();
            }
        }
    }
}