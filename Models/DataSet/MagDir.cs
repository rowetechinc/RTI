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
 * 12/16/2011      RC          1.10       Initial coding.
 *       
 */


using System;
namespace RTI
{
    /// <summary>
    /// These values will represent the magnitude and
    /// direction values for a given bin within an ensemble.
    /// The East, North and Vertical velocities will come
    /// from the Earth velocities.  The BT velocities
    /// will come from the Bottom Track Earth velocities.
    /// </summary>
    public class MagDir
    {
        /// <summary>
        /// Represents a bad value for the
        /// magnitude or direction value.
        /// </summary>
        public const double BAD = DataSet.Ensemble.BAD_VELOCITY;

        /// <summary>
        /// Magnitude value.
        /// </summary>
        public double Magnitude { get; private set; }

        /// <summary>
        /// Direction value with East up.
        /// </summary>
        public double Direction { get; private set; }

        /// <summary>
        /// Direction value with North up.
        /// </summary>
        public double DirectionYNorth { get; private set; }

        /// <summary>
        /// Set the properites from the given parameters.  This
        /// will validate all the all values.  If any Profile velocities
        /// are bad, it will set all values to bad.  It will remove
        /// the bottom track velocity from the water profile velocity
        /// then calculate the magnitude and direction for the values.
        /// It will set the Magntiude and Direction with North Up and
        /// East up.
        /// </summary>
        /// <param name="btEast">Bottom Track East velocity.</param>
        /// <param name="btNorth">Bottom Track North Velocity.</param>
        /// <param name="btVertical">Bottom Track Vertical Velocity.</param>
        /// <param name="east">Water Profile East velocity.</param>
        /// <param name="north">Water Profile North Velocity.</param>
        /// <param name="vertical">Water Profile Vertical Velocity.</param>
        public MagDir(float btEast, float btNorth, float btVertical, float east, float north, float vertical)
        {
            float eastVal = 0f;
            float northVal = 0f;
            float verticalVal = 0f;

            // Validate values
            // and subtract bottom track velocity from velocities
            // Ensure all the bottom track velocities are good
            // when removing bottom track speed so the data is consistent
            if (east != DataSet.Ensemble.BAD_VELOCITY)
            {
                if (btEast != DataSet.Ensemble.BAD_VELOCITY && btNorth != DataSet.Ensemble.BAD_VELOCITY && btVertical != DataSet.Ensemble.BAD_VELOCITY)
                {
                    // Bottom Track speed is inverted
                    // So add bottom track speed to remove speed
                    eastVal = btEast + east;
                }
                else
                {
                    // Bottom Track east was bad
                    eastVal = east;
                }
            }
            else
            {
                // East is bad so set bad values
                SetBadValues();
                return;
            }

            if (north != DataSet.Ensemble.BAD_VELOCITY)
            {
                if (btEast != DataSet.Ensemble.BAD_VELOCITY && btNorth != DataSet.Ensemble.BAD_VELOCITY && btVertical != DataSet.Ensemble.BAD_VELOCITY)
                {
                    // Bottom Track speed is inverted
                    // So add bottom track speed to remove speed
                    northVal = btNorth + north;
                }
                else
                {
                    // Bottom Track north was bad
                    northVal = north;
                }
            }
            else
            {
                // North is bad so set bad values
                SetBadValues();
                return;
            }

            if (vertical != DataSet.Ensemble.BAD_VELOCITY)
            {
                if (btEast != DataSet.Ensemble.BAD_VELOCITY && btNorth != DataSet.Ensemble.BAD_VELOCITY && btVertical != DataSet.Ensemble.BAD_VELOCITY)
                {
                    // Bottom Track speed is inverted
                    // So add bottom track speed to remove speed
                    verticalVal = btVertical + vertical;
                }
                else
                {
                    // Bottom Track Vertical is bad
                    verticalVal = vertical;
                }
            }
            else
            {
                // Vertical is bad so set bad values
                SetBadValues();
                return;
            }

            // Generate values
            CalculateMag(eastVal, northVal, verticalVal);
            CalculateDir(eastVal, northVal);
        }

        /// <summary>
        /// Return whether the object is bad.  If any of the data
        /// is bad, the object is bad.
        /// </summary>
        /// <returns>TRUE if all the data is good.</returns>
        public bool IsBad()
        {
            return (Direction == MagDir.BAD) || (DirectionYNorth == MagDir.BAD) || (Magnitude == MagDir.BAD);
        }

        /// <summary>
        /// Calculate the Magnitude for the given
        /// velocity values.  It is assumed the Bottom
        /// Track velocities have already been removed.
        /// </summary>
        /// <param name="east">East velocity.</param>
        /// <param name="north">North Velocity.</param>
        /// <param name="vertical">Vertical velocity.</param>
        private void CalculateMag(float east, float north, float vertical)
        {
            Magnitude = Math.Sqrt((east * east) + (north * north) + (vertical * vertical));
        }

        /// <summary>
        /// Calculate the direction with North up and East up
        /// given the velocity values. It is assumed the
        /// Bottom Track velocities have already been
        /// removed.
        /// </summary>
        /// <param name="east">East Velocity.</param>
        /// <param name="north">North Velocity.</param>
        private void CalculateDir(float east, float north)
        {
            Direction = (Math.Atan2(east, north)) * (180 / Math.PI);
            DirectionYNorth = (Math.Atan2(north, east)) * (180 / Math.PI);
        }

        /// <summary>
        /// Set bad value for all the properties.
        /// </summary>
        private void SetBadValues()
        {
            Magnitude = MagDir.BAD;
            Direction = MagDir.BAD;
            DirectionYNorth = MagDir.BAD;
        }
    }
}