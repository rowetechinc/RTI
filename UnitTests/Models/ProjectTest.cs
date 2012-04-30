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
 * 11/21/2011      RC                     Initial coding
 * 01/03/2012      RC          1.11       Update test with folder being created.
 * 
 */

namespace RTI
{
    using System;
    using NUnit.Framework;
    using System.IO;

    /// <summary>
    /// Unit test of the Project object.
    /// </summary>
    [TestFixture]
    public class ProjectTest
    {
        /// <summary>
        /// Test the constructor.
        /// </summary>
        [Test]
        public void TestConstructor()
        {
            int projectId = 0;
            string projectSerial = "01300000000000000000000000000001";
            string projectName = "Test Project";
            string projectDir = @"D:\UnitTestResult";  // Choose a FOLDER where database files can be created

            // Remove a previous test result
            if (Directory.Exists(projectDir + @"\" + projectName))
            {
                File.Delete(projectDir + @"\" + projectName + @"\" + projectName + ".db");
                Directory.Delete(projectDir + @"\" + projectName);
            }

            Project p = new Project(projectName, projectDir, projectSerial);

            p.ProjectID = projectId;

            Assert.AreEqual(projectId, p.ProjectID);                                        // Verify Project ID
            Assert.AreEqual(projectName, p.ProjectName);                                    // Verify Project Name
            Assert.AreEqual(projectDir, p.ProjectDir);                                      // Verify Project Dir
            Assert.AreEqual(projectDir + @"\" + projectName, p.ProjectFolderPath);          // Verify Project folder path
            //Assert.AreEqual(projectSerial, p.SysSerialNumber.ToString(), "Project Serial numbers do not match");

        }

        /// <summary>
        /// Test the constructor.
        /// </summary>
        [Test]
        public void TestConstructorId()
        {
            int projectId = 0;
            string projectSerial = "01300000000000000000000000000001";
            string projectName = "Test Project";
            string projectDir = @"D:\UnitTestResult";  // Choose a FOLDER where database files can be created

            // Remove a previous test result
            if (Directory.Exists(projectDir + @"\" + projectName))
            {
                File.Delete(projectDir + @"\" + projectName + @"\" + projectName + ".db");
                Directory.Delete(projectDir + @"\" + projectName);
            }

            Project p = new Project(projectId, projectName, projectDir, projectSerial);

            Assert.AreEqual(projectId, p.ProjectID);                                        // Verify Project ID
            Assert.AreEqual(projectName, p.ProjectName);                                    // Verify Project Name
            Assert.AreEqual(projectDir, p.ProjectDir);                                      // Verify Project Dir
            Assert.AreEqual(projectDir + @"\" + projectName, p.ProjectFolderPath);          // Verify Project folder path
            //Assert.AreEqual(projectSerial, p.SysSerialNumber.ToString(), "Project Serial numbers do not match");

        }

        /// <summary>
        /// Test the constructor.
        /// </summary>
        [Test]
        public void TestConstructorIdTime()
        {
            int projectId = 0;
            string projectSerial = "01300000000000000000000000000001";
            string projectName = "Test Project";
            string projectDir = @"D:\UnitTestResult";  // Choose a FOLDER where database files can be created
            DateTime dt = DateTime.Now;
            DateTime dtMod = DateTime.Now;

            // Remove a previous test result
            if (Directory.Exists(projectDir + @"\" + projectName))
            {
                File.Delete(projectDir + @"\" + projectName + @"\" + projectName + ".db");
                Directory.Delete(projectDir + @"\" + projectName);
            }

            Project p = new Project(projectId, projectName, projectDir, dt, dtMod, projectSerial);

            Assert.AreEqual(projectId, p.ProjectID);                                        // Verify Project ID
            Assert.AreEqual(projectName, p.ProjectName);                                    // Verify Project Name
            Assert.AreEqual(projectDir, p.ProjectDir);                                      // Verify Project Dir
            Assert.AreEqual(projectDir + @"\" + projectName, p.ProjectFolderPath);          // Verify Project folder path
            Assert.AreEqual(dt, p.DateCreated);                                             // Verify Date created
            Assert.AreEqual(dtMod, p.LastDateModified);                                     // Verify Date Modified
            //Assert.AreEqual(projectSerial, p.SysSerialNumber.ToString(), "Project Serial numbers do not match");
        }

    }
}
