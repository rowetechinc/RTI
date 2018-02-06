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
 * 09/07/2012      RC          2.15       Initial coding
 * 09/14/2012      RC          2.15       Added BatteryType.
 * 10/08/2012      RC          2.15       Added new Battery Types.
 * 10/10/2012      RC          2.15       Changed Ints to UInt32.  Add Min values.
 *                                         Added GetBatteryList().
 * 10/09/2013      RC          2.21.0     Added DeploymentMode.
 * 10/14/2013      RC          2.21.0     Added InternalMemoryCardUsed and InternalMemoryCardTotal.
 * 11/13/2014      RC          2.21.0     Added DVL and VM deployment modes.
 * 10/19/2016      RC          3.3.2      Changed default battery to 38C.
 * 
 */

using System;
using System.Collections.Generic;
namespace RTI
{

    /// <summary>
    /// Deployment options.  These are the options set by the user
    /// for determining the deployment.
    /// </summary>
    public class DeploymentOptions
    {
        #region Struts and Enum

        /// <summary>
        /// Power based off battery option.
        /// Update GetBatteryTypes() with the latest battery types.
        /// </summary>
        public enum AdcpBatteryType
        {
            /// <summary>
            /// 19*2 C Alkaline: 440 Wh-hr.
            /// </summary>
            Alkaline_38C = 440,

            /// <summary>
            /// 14D Alkaline: 360 Wh-hr.
            /// </summary>
            Alkaline_14D = 360,

            /// <summary>
            /// 21D cell Alkaline: 540 Wh-hr.
            /// </summary>
            Alkaline_21D = 540,

            /// <summary>
            /// 7 DD cell Lithium: 800 Wh-hr.
            /// </summary>
            Lithium_7DD = 800
        }


        /// <summary>
        /// Type of deployment the ADCP will be put in.
        /// </summary>
        public enum AdcpDeploymentMode
        {
            /// <summary>
            /// Recording live data from the ADCP through a cable.
            /// SeaPROFILER.
            /// </summary>
            DirectReading,

            /// <summary>
            /// Data recorded internally to the ADCP and deployed for a period of time alone.
            /// SeaWATCH.
            /// </summary>
            SelfContained,

            /// <summary>
            /// Measure waves.
            /// SeaWAVE.
            /// </summary>
            Waves,

            /// <summary>
            /// Measure River discharge.
            /// RiverPROFILER.
            /// </summary>
            River,

            /// <summary>
            /// DVL system.
            /// SeaPILOT.
            /// </summary>
            Dvl,

            /// <summary>
            /// Vessel Mounted system.
            /// SeaTRAK.
            /// </summary>
            VM,
        }

        #endregion

        #region Min Max Values

        /// <summary>
        /// Minimum Duration Value.
        /// </summary>
        public const int MIN_DURATION = 1;

        /// <summary>
        /// Miniumum Number of batteries.
        /// </summary>
        public const int MIN_NUM_BATTERIES = 0;

        /// <summary>
        /// Mininum Depth to Bottom.
        /// </summary>
        public const int MIN_DEPTH_TO_BOTTOM = 0;

        #endregion

        #region Defaults

        /// <summary>
        /// Default Duration value.
        /// </summary>
        public const UInt32 DEFAULT_DURATION = 1;

        /// <summary>
        /// Default number of batteries.
        /// </summary>
        public const UInt32 DEFAULT_NUM_BATTERIES = 1;

        /// <summary>
        /// Default Battery Type.
        /// </summary>
        public const AdcpBatteryType DEFAULT_BATTERY_TYPE = AdcpBatteryType.Alkaline_38C;

        /// <summary>
        /// Default Depth to bottom in meters.
        /// </summary>
        public const UInt32 DEFAULT_DEPTH_TO_BOTTOM = 100;

        /// <summary>
        /// Default deployment is Direct reading.
        /// </summary>
        public const AdcpDeploymentMode DEFAULT_DEPLOYMENT_MODE = AdcpDeploymentMode.DirectReading;

        /// <summary>
        /// Default total memory card used.
        /// </summary>
        public const long DEFAULT_MEMORY_CARD_USED = 0;

        /// <summary>
        /// Default total size of the memory card.
        /// </summary>
        public const long DEFAULT_MEMORY_CARD_TOTAL = 0;

        #endregion

        #region Properties

        /// <summary>
        /// Number of days for the deployment.
        /// </summary>
        public UInt32 Duration { get; set; }

        /// <summary>
        /// Number of batteries in the system.
        /// </summary>
        public UInt32 NumBatteries { get; set; }

        /// <summary>
        /// Type of battery used.
        /// </summary>
        public AdcpBatteryType BatteryType { get; set; }

        /// <summary>
        /// Depth to the bottom of the ocean in meters.
        /// </summary>
        public UInt32 DepthToBottom { get; set; }

        /// <summary>
        /// ADCP deployment mode.
        /// </summary>
        public AdcpDeploymentMode DeploymentMode { get; set; }

        /// <summary>
        /// Total used space in bytes on the internal memory card of the ADCP.
        /// </summary>
        public long InternalMemoryCardUsed { get; set; }

        /// <summary>
        /// Total size of the internal memory card of the ADCP in bytes.
        /// </summary>
        public long InternalMemoryCardTotalSize { get; set; }

        #endregion

        /// <summary>
        /// Default Constructor.
        /// Set the default values.
        /// </summary>
        public DeploymentOptions()
        {
            // Set the defaults
            SetDefaults();
        }

        /// <summary>
        /// Set the Deployment options.
        /// </summary>
        /// <param name="duration">Deployment durations in days.</param>
        /// <param name="numBatt">Number of batteries.</param>
        /// <param name="battType">Battery Type.</param>
        /// <param name="depthBottom">Depth to the bottom in meters.</param>
        /// <param name="mode">Deployment mode.</param>
        /// <param name="memoryCardUsed">Total bytes used on the memory card.</param>
        /// <param name="memoryCardTotal">Total size in bytes of the memory card.</param>
        public DeploymentOptions(UInt32 duration, UInt32 numBatt, AdcpBatteryType battType, UInt32 depthBottom, AdcpDeploymentMode mode, long memoryCardUsed, long memoryCardTotal)
        {
            Duration = duration;
            NumBatteries = numBatt;
            BatteryType = battType;
            DepthToBottom = depthBottom;
            DeploymentMode = mode;
            InternalMemoryCardUsed = memoryCardUsed;
            InternalMemoryCardTotalSize = memoryCardTotal;
        }

        /// <summary>
        /// Set the default values.
        /// </summary>
        public void SetDefaults()
        {
            Duration = DEFAULT_DURATION;                                    // Default Day
            NumBatteries = DEFAULT_NUM_BATTERIES;                           // Default Num Battery
            BatteryType = DEFAULT_BATTERY_TYPE;                             // Default Battery Type
            DepthToBottom = DEFAULT_DEPTH_TO_BOTTOM;                        // Default Depth to Bottom
            DeploymentMode = DEFAULT_DEPLOYMENT_MODE;                       // Default Deployment mode
            InternalMemoryCardUsed = DEFAULT_MEMORY_CARD_USED;              // Default used
            InternalMemoryCardTotalSize = DEFAULT_MEMORY_CARD_TOTAL;        // Default total size
        }

        /// <summary>
        /// Get a list of all the battery types.
        /// </summary>
        /// <returns>List of all the battery types.</returns>
        public static List<DeploymentOptions.AdcpBatteryType> GetBatteryList()
        {
            List<AdcpBatteryType> list = new List<AdcpBatteryType>();
            list.Add(AdcpBatteryType.Alkaline_38C);
            list.Add(AdcpBatteryType.Alkaline_14D);
            list.Add(AdcpBatteryType.Alkaline_21D);
            list.Add(AdcpBatteryType.Lithium_7DD);

            return list;
        }
    }
}
