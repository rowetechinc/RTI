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
 * 01/05/2012      RC          1.11       Initial coding
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
        /// Calculate the standard deviation for the 4 values given.
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

            //double variance = Math.Pow(v1 - avg, 2) + Math.Pow(v2 - avg, 2) + Math.Pow(v3 - avg, 2) + Math.Pow(v4 - avg, 2);
            return Math.Sqrt(variance);
        }
    }
}
