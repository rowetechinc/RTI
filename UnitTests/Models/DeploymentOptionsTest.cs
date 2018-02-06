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
 * 10/01/2012      RC          2.15       Initial coding
 * 
 * 
 */

namespace RTI
{
    using NUnit.Framework;

    /// <summary>
    /// Test the DeploymentOptions object.
    /// </summary>
    [TestFixture]
    public class DeploymentOptionsTest
    {

        /// <summary>
        /// Test Default constructor.
        /// </summary>
        [Test]
        public void TestDefaultConstructor()
        {
            DeploymentOptions options = new DeploymentOptions();

            Assert.AreEqual(DeploymentOptions.DEFAULT_BATTERY_TYPE, options.BatteryType, "Battery Type is incorrect.");
            Assert.AreEqual(DeploymentOptions.DEFAULT_DEPTH_TO_BOTTOM, options.DepthToBottom, "Depth to Bottom is incorrect.");
            Assert.AreEqual(DeploymentOptions.DEFAULT_DURATION, options.Duration, "Duration is incorrect.");
            Assert.AreEqual(DeploymentOptions.DEFAULT_NUM_BATTERIES, options.NumBatteries, "Number of Batteries is incorrect.");
        }

        /// <summary>
        /// Test constructor.
        /// </summary>
        [Test]
        public void TestConstructor()
        {
            DeploymentOptions options = new DeploymentOptions(5, 6, DeploymentOptions.AdcpBatteryType.Lithium_7DD, 77, DeploymentOptions.AdcpDeploymentMode.DirectReading, 22, 33);

            Assert.AreEqual(5, options.Duration, "Duration is incorrect.");
            Assert.AreEqual(6, options.NumBatteries, "Number of Batteries is incorrect.");
            Assert.AreEqual(DeploymentOptions.AdcpBatteryType.Lithium_7DD, options.BatteryType, "Battery Type is incorrect.");
            Assert.AreEqual(77, options.DepthToBottom, "Depth to Bottom is incorrect.");
            Assert.AreEqual(DeploymentOptions.AdcpDeploymentMode.DirectReading, options.DeploymentMode, "Deployment mode is incorrect");
            Assert.AreEqual(22, options.InternalMemoryCardUsed, "Used Internal memory is incorrect.");
            Assert.AreEqual(33, options.InternalMemoryCardTotalSize, "Internal Memory total size is incorrect.");
        }

        /// <summary>
        /// Test Setting values.
        /// </summary>
        [Test]
        public void TestSetting()
        {
            DeploymentOptions options = new DeploymentOptions();
            options.Duration = 5;
            options.NumBatteries = 6;
            options.BatteryType = DeploymentOptions.AdcpBatteryType.Lithium_7DD;
            options.DepthToBottom = 77;

            Assert.AreEqual(5, options.Duration, "Duration is incorrect.");
            Assert.AreEqual(6, options.NumBatteries, "Number of Batteries is incorrect.");
            Assert.AreEqual(DeploymentOptions.AdcpBatteryType.Lithium_7DD, options.BatteryType, "Battery Type is incorrect.");
            Assert.AreEqual(77, options.DepthToBottom, "Depth to Bottom is incorrect.");
        }
    }
}
