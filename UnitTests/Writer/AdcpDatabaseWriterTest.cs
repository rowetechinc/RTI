///*
// * Copyright © 2011 
// * Rowe Technology Inc.
// * All rights reserved.
// * http://www.rowetechinc.com
// * 
// * Redistribution and use in source and binary forms, with or without
// * modification is NOT permitted.
// * 
// * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
// * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
// * COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
// * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
// * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
// * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
// * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
// * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// * POSSIBILITY OF SUCH DAMAGE.
// * 
// * HISTORY
// * -----------------------------------------------------------------
// * Date            Initials    Version    Comments
// * -----------------------------------------------------------------
// * 01/07/2013      RC          2.17       Initial coding
// * 09/11/2013      RC          2.19.5     Updated test to 2.19.5
// * 
// */

//namespace RTI
//{
//    using System;
//    using NUnit.Framework;
//    using System.Diagnostics;
//    using System.IO;

//    /// <summary>
//    /// Unit test of the Adcp Database Writer.
//    /// </summary>
//    [TestFixture]
//    public class AdcpDatabaseWriterTest
//    {

//        /// <summary>
//        /// Test writing data to the database
//        /// </summary>
//        [Test]
//        public void TestConstructor()
//        {
//            string projectSerial = "01300000000000000000000000000001";
//            string projectName = "Test Project3";
//            string projectDir = @"D:\UnitTestResult";  // Choose a FOLDER where database files can be created

//            // Remove a previous test result
//            if (Directory.Exists(projectDir + @"\" + projectName))
//            {
//                File.Delete(projectDir + @"\" + projectName + @"\" + projectName + ".db");
//                Directory.Delete(projectDir + @"\" + projectName);
//            }

//            Project p = new Project(projectName, projectDir, projectSerial);

//            AdcpDatabaseWriter writer = new AdcpDatabaseWriter();
//            writer.SelectedProject = p;

//            // Generate an Ensemble
//            DataSet.Ensemble ensemble = EnsembleHelper.GenerateEnsemble(30);

//            #region Ensemble

//            // Modify the data
//            ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
//            ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
//            ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
//            ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

//            ensemble.AmplitudeData.AmplitudeData[3, DataSet.Ensemble.BEAM_0_INDEX] = 2.2f;
//            ensemble.AmplitudeData.AmplitudeData[3, DataSet.Ensemble.BEAM_1_INDEX] = 3.3f;
//            ensemble.AmplitudeData.AmplitudeData[3, DataSet.Ensemble.BEAM_2_INDEX] = 4.4f;
//            ensemble.AmplitudeData.AmplitudeData[3, DataSet.Ensemble.BEAM_3_INDEX] = 5.5f;

//            ensemble.AmplitudeData.AmplitudeData[5, DataSet.Ensemble.BEAM_0_INDEX] = 3.2f;
//            ensemble.AmplitudeData.AmplitudeData[5, DataSet.Ensemble.BEAM_1_INDEX] = 4.3f;
//            ensemble.AmplitudeData.AmplitudeData[5, DataSet.Ensemble.BEAM_2_INDEX] = 5.4f;
//            ensemble.AmplitudeData.AmplitudeData[5, DataSet.Ensemble.BEAM_3_INDEX] = 6.5f;

//            #endregion

//            writer.AddIncomingData(ensemble);
//            writer.Flush();
//        }

//        /// <summary>
//        /// Test the timing of the two ways to write ensemble data and determine
//        /// which is faster.
//        /// </summary>
//        [Test]
//        public void Timing()
//        {
//            string projectSerial = "01300000000000000000000000000001";
//            string projectName = "Test Project4";
//            string projectName1 = "Test Project5";
//            string projectName2 = "Test Project6";
//            string projectDir = @"D:\UnitTestResult";  // Choose a FOLDER where database files can be created

//            // Remove a previous test result
//            if (Directory.Exists(projectDir + @"\" + projectName))
//            {
//                File.Delete(projectDir + @"\" + projectName + @"\" + projectName + ".db");
//                Directory.Delete(projectDir + @"\" + projectName);
//            }

//            // Remove a previous test result
//            if (Directory.Exists(projectDir + @"\" + projectName1))
//            {
//                File.Delete(projectDir + @"\" + projectName1 + @"\" + projectName1 + ".db");
//                Directory.Delete(projectDir + @"\" + projectName1);
//            }

//            // Remove a previous test result
//            if (Directory.Exists(projectDir + @"\" + projectName2))
//            {
//                File.Delete(projectDir + @"\" + projectName2 + @"\" + projectName2 + ".db");
//                Directory.Delete(projectDir + @"\" + projectName2);
//            }

//            Project p = new Project(projectName, projectDir, projectSerial);
//            AdcpDatabaseWriter writer = new AdcpDatabaseWriter();
//            writer.SelectedProject = p;

//            // Generate an Ensemble
//            DataSet.Ensemble ensemble = EnsembleHelper.GenerateEnsemble(30);

//            #region Ensemble

//            // Modify the data
//            ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
//            ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
//            ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
//            ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

//            ensemble.AmplitudeData.AmplitudeData[3, DataSet.Ensemble.BEAM_0_INDEX] = 2.2f;
//            ensemble.AmplitudeData.AmplitudeData[3, DataSet.Ensemble.BEAM_1_INDEX] = 3.3f;
//            ensemble.AmplitudeData.AmplitudeData[3, DataSet.Ensemble.BEAM_2_INDEX] = 4.4f;
//            ensemble.AmplitudeData.AmplitudeData[3, DataSet.Ensemble.BEAM_3_INDEX] = 5.5f;

//            ensemble.AmplitudeData.AmplitudeData[5, DataSet.Ensemble.BEAM_0_INDEX] = 3.2f;
//            ensemble.AmplitudeData.AmplitudeData[5, DataSet.Ensemble.BEAM_1_INDEX] = 4.3f;
//            ensemble.AmplitudeData.AmplitudeData[5, DataSet.Ensemble.BEAM_2_INDEX] = 5.4f;
//            ensemble.AmplitudeData.AmplitudeData[5, DataSet.Ensemble.BEAM_3_INDEX] = 6.5f;

//            #endregion

//            Stopwatch watch = new Stopwatch();
//            int TEST = 1000;

//            watch.Start();
//            for (int x = 0; x < TEST; x++)
//            {
//                writer.AddIncomingData(ensemble);
//            }
//            writer.Flush();
//            watch.Stop();
//            long result = watch.ElapsedMilliseconds;
//            Debug.WriteLine("Full Write:{0}", result);

//            Debug.WriteLine("Full Write:{0}", result);

//            writer.Dispose();
//        }

//        /// <summary>
//        /// Test how long it takes to write out 1000 entries to 
//        /// the database.  This will first create a database and then write
//        /// 1000 entries. 
//        /// </summary>
//        [Test]
//        public void Write1000Ensembles()
//        {

//            string projectSerial = "01300000000000000000000000000001";
//            string projectName = "Test AdcpDatabaseWriter_Write1000Ensembles";
//            string projectDir = @"D:\UnitTestResult";  // Choose a FOLDER where database files can be created

//            // Remove a previous test result
//            if (Directory.Exists(projectDir + @"\" + projectName))
//            {
//                File.Delete(projectDir + @"\" + projectName + @"\" + projectName + ".db");
//                Directory.Delete(projectDir + @"\" + projectName);
//            }

//            Project p = new Project(projectName, projectDir, projectSerial);

//            #region Write Ensembles
//            AdcpDatabaseWriter writer = new AdcpDatabaseWriter();
//            writer.SelectedProject = p;

//            for (int x = 0; x < 1000; x++)
//            {
//                // Generate an Ensemble
//                DataSet.Ensemble ensemble = EnsembleHelper.GenerateEnsemble(30);

//                #region Ensemble

//                // Modify the data
//                ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_0_INDEX] = 1.2f;
//                ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_1_INDEX] = 2.3f;
//                ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_2_INDEX] = 3.4f;
//                ensemble.AmplitudeData.AmplitudeData[0, DataSet.Ensemble.BEAM_3_INDEX] = 4.5f;

//                ensemble.AmplitudeData.AmplitudeData[3, DataSet.Ensemble.BEAM_0_INDEX] = 2.2f;
//                ensemble.AmplitudeData.AmplitudeData[3, DataSet.Ensemble.BEAM_1_INDEX] = 3.3f;
//                ensemble.AmplitudeData.AmplitudeData[3, DataSet.Ensemble.BEAM_2_INDEX] = 4.4f;
//                ensemble.AmplitudeData.AmplitudeData[3, DataSet.Ensemble.BEAM_3_INDEX] = 5.5f;

//                ensemble.AmplitudeData.AmplitudeData[5, DataSet.Ensemble.BEAM_0_INDEX] = 3.2f;
//                ensemble.AmplitudeData.AmplitudeData[5, DataSet.Ensemble.BEAM_1_INDEX] = 4.3f;
//                ensemble.AmplitudeData.AmplitudeData[5, DataSet.Ensemble.BEAM_2_INDEX] = 5.4f;
//                ensemble.AmplitudeData.AmplitudeData[5, DataSet.Ensemble.BEAM_3_INDEX] = 6.5f;

//                #endregion
//                ensemble.EnsembleData.EnsembleNumber = x;
//                ensemble.EnsembleData.EnsDateTime = DateTime.Now;
//                writer.AddIncomingData(ensemble);
//            }
//            writer.Flush();
//            #endregion

//            writer.Dispose();
//        }

//    }
//}
