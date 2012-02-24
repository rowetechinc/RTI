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
 * 01/05/2012      RC          1.11       Initial coding
 * 01/16/2012      RC          1.14       Added Calculate Magnitude and direction.
 * 02/14/2012      RC          2.03       Fixed standard deviation equation.
 * 02/24/2012      RC          2.03       Added DegreeToRadian.
 * 
 */

using System;


namespace RTI
{
    /// <summary>
    /// Include additional math equations.
    /// </summary>
    public class MathHelper
    {
        /// <summary>
        /// Calculate the population standard deviation for the 4 values given.
        /// </summary>
        /// <param name="v1">Value 1.</param>
        /// <param name="v2">Value 2.</param>
        /// <param name="v3">Value 3.</param>
        /// <param name="v4">Value 4.</param>
        /// <param name="avg">Output the calculate average.</param>
        /// <returns>Standard deviation of 4 values.</returns>
        public static double StandardDev(double v1, double v2, double v3, double v4, out double avg)
        {
            avg = 0;
            int count = 0;

            // Omit any bad values (0 Bad)
            if (v1 != 0)
            {
                avg += v1;
                count++;
            }
            if (v2 != 0)
            {
                avg += v2;
                count++;
            }
            if (v3 != 0)
            {
                avg += v3;
                count++;
            }
            if (v4 != 0)
            {
                avg += v4;
                count++;
            }

            // Calculate the average
            if (count > 0)
            {
                avg /= count;
            }

            double variance = 0;
            if (v1 != 0)
            {
                variance += Math.Pow(v1 - avg, 2);
            }
            if (v2 != 0)
            {
                variance += Math.Pow(v2 - avg, 2);
            }
            if (v3 != 0)
            {
                variance += Math.Pow(v3 - avg, 2);
            }
            if (v4 != 0)
            {
                variance += Math.Pow(v4 - avg, 2);
            }

            // Calculate the variance
            variance /= 4;

            // Return the standard deviation
            return Math.Sqrt(variance);
        }

        /// <summary>
        /// Calculate the Magnitude given the North, East and Vertical velocity.
        /// </summary>
        /// <param name="east">East Velocity.</param>
        /// <param name="north">North Velocity.</param>
        /// <param name="vertical">Vertical Velocity.</param>
        /// <returns>Magnitude of the velocities given.</returns>
        public static double CalculateMagnitude(double east, double north, double vertical)
        {
            return Math.Sqrt((east * east) + (north * north) + (vertical * vertical));
        }

        /// <summary>
        /// Calculate the Direction of the velocities given.
        /// Value will be returned in degrees.
        /// </summary>
        /// <param name="y">Y axis velocity value.</param>
        /// <param name="x">X axis velocity value.</param>
        /// <returns>Direction of the velocity return in degrees.</returns>
        public static double CalculateDirection(double y, double x)
        {
            return (Math.Atan2(y, x)) * (180 / Math.PI);
        }

        /// <summary>
        /// Convert the given angle to radians.
        /// </summary>
        /// <param name="angle">Angle in degrees.</param>
        /// <returns>Angle in radians.</returns>
        public static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

    }
}
