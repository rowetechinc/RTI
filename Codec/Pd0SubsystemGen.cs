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
 * 05/21/2019      RC          3.4.11     Initial Coding.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI
{
    /// <summary>
    /// Generate a unique PD0 subsystem configuration.  All the values used
    /// to create a unique subsystem configuration.
    /// </summary>
    public class Pd0Subsystem
    {
        /// <summary>
        /// Subsystem configuration.
        /// </summary>
        public SubsystemConfiguration SsConfig { get; set; }

        /// <summary>
        /// Number of beams.
        /// </summary>
        public int NumBeams { get; set; }

        /// <summary>
        /// Number of bins.
        /// </summary>
        public int NumBins { get; set; }

        /// <summary>
        /// Bin Size.
        /// </summary>
        public float BinSize { get; set; }

        /// <summary>
        /// Blank.
        /// </summary>
        public float Blank { get; set; }

        /// <summary>
        /// Number of pings.
        /// </summary>
        public int NumPings { get; set; }

        /// <summary>
        /// PD0 Subsystem.  This will include a little more details because
        /// the PD0 dataset does not include a separate value for the configuration index.
        /// </summary>
        /// <param name="ssConfig"></param>
        /// <param name="numBeams"></param>
        /// <param name="numBins"></param>
        /// <param name="binSize"></param>
        /// <param name="blank"></param>
        /// <param name="numPings">Number of pings.</param>
        /// 
        public Pd0Subsystem(SubsystemConfiguration ssConfig, 
            int numBeams,
            int numBins,
            float binSize,
            float blank,
            int numPings)
        {
            this.SsConfig = ssConfig;
            this.NumBeams = numBeams;
            this.NumBins = numBins;
            this.BinSize = binSize;
            this.Blank = blank;
            this.NumPings = numPings;
        }

        /// <summary>
        /// Check if the given Pd0Subsystem is equal to this object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            Pd0Subsystem p = (Pd0Subsystem)obj;

            return (this.SsConfig.SubSystem.Code == p.SsConfig.SubSystem.Code &
                this.NumBeams == p.NumBeams &
                this.NumBins == p.NumBins &
                this.BinSize == p.BinSize &
                this.Blank == p.Blank &
                this.NumPings == p.NumPings);
        }

        /// <summary>
        /// Determine if the 2 ids given are the equal.
        /// </summary>
        /// <param name="code1">First subsystem to check.</param>
        /// <param name="code2">SubSystem to check against.</param>
        /// <returns>True if there codes match.</returns>
        public static bool operator ==(Pd0Subsystem code1, Pd0Subsystem code2)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(code1, code2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)code1 == null) || ((object)code2 == null))
            {
                return false;
            }

            // Return true if the fields match:
            // Only 1 code is possible per system
            // It is not possible to have the same subsystem 1 ADCP
            return (code1.SsConfig.SubSystem.Code == code2.SsConfig.SubSystem.Code &
               code1.NumBeams == code2.NumBeams &
               code1.NumBins == code2.NumBins &
               code1.BinSize == code2.BinSize &
               code1.Blank == code2.Blank &
               code1.NumPings == code2.NumPings);
        }

        /// <summary>
        /// Return the opposite of ==.
        /// </summary>
        /// <param name="code1">First sub-system to check.</param>
        /// <param name="code2">Sub-system to check against.</param>
        /// <returns>Return the opposite of ==.</returns>
        public static bool operator !=(Pd0Subsystem code1, Pd0Subsystem code2)
        {
            return !(code1 == code2);
        }

        /// <summary>
        /// Hash code for the object.
        /// </summary>
        /// <returns>Returns the base hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// Generate a RTB subsystem from PD0 data.
    /// PD0 does not contain all the information for a subsystem configuration.
    /// So this will generate a unique subsystem for all the configuration types.
    /// </summary>
    public class Pd0SubsystemGen
    {
        /// <summary>
        /// List to hold all the subsystems found.
        /// </summary>
        private Dictionary<Pd0Subsystem, SubsystemConfiguration> _ssList;

        /// <summary>
        /// Initiailize the list.
        /// </summary>
        public Pd0SubsystemGen()
        {
            _ssList = new Dictionary<Pd0Subsystem, SubsystemConfiguration>();
        }

        /// <summary>
        /// Generate a subsystem based off the information in the ensemble.
        /// </summary>
        /// <param name="ens">Ensemble.</param>
        /// <returns>Subsystem Configuraiton based off the ensemble.</returns>
        public SubsystemConfiguration GenSubsystem(DataSet.Ensemble ens)
        {
            // Create a default subsystem config
            SubsystemConfiguration ss = new SubsystemConfiguration();

            int beams = 4;
            int bins = 30;
            float blank = 0.0f;
            float binSize = 1.0f;
            int numPings = 1;

            // If the ensemble contains a configuration, use it as default
            if(ens.IsEnsembleAvail)
            {
                ss = ens.EnsembleData.SubsystemConfig;
                beams = ens.EnsembleData.NumBeams;
                bins = ens.EnsembleData.NumBins;
                numPings = ens.EnsembleData.DesiredPingCount;
            }

            if(ens.IsAncillaryAvail)
            {
                blank = ens.AncillaryData.FirstBinRange;
                binSize = ens.AncillaryData.BinSize;
            }

            // Generate a PD0 Subsystem
            Pd0Subsystem pd0Ss = new Pd0Subsystem(ss, beams, bins, binSize, blank, numPings);

            // If no entry yet, then add the first entry
            if(_ssList.Count == 0)
            {
                // Add the new entry in the dict
                SubsystemConfiguration newSsConfig = pd0Ss.SsConfig;
                newSsConfig.SubsystemConfigIndex = (byte)0;
                newSsConfig.CepoIndex = (byte)0;
                newSsConfig.SubSystem.Index = (ushort)0;

                _ssList.Add(pd0Ss, newSsConfig);

                return newSsConfig;
            }

            // Check if it already exist in the dict
            foreach(var pd0SsFound in _ssList.Keys)
            {
                if(pd0SsFound == pd0Ss)
                {
                    return _ssList[pd0SsFound];
                }
            }

            // Add the new entry in the dict
            SubsystemConfiguration newSubsystemConfig = pd0Ss.SsConfig;
            newSubsystemConfig.SubsystemConfigIndex = (byte)_ssList.Count;
            newSubsystemConfig.CepoIndex = (byte)_ssList.Count;
            newSubsystemConfig.SubSystem.Index = (ushort)_ssList.Count;

            _ssList.Add(pd0Ss, newSubsystemConfig);

            return newSubsystemConfig;
        }



    }
}

