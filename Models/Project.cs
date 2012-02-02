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
 * 09/01/2011      RC                     Initial coding
 * 11/15/2011      RC                     Removed IsDirectReading.
 * 11/21/2011      RC                     Added comments.
 * 12/30/2011      RC          1.11       Create database file in constructor.
 *                                         Added getting project database full path.
 * 01/03/2012      RC          1.11       Create Project directory in constructor.
 * 01/04/2012      RC          1.11       Added method for Project Image path.
 * 01/13/2012      RC          1.12       Added indexes to database to improve search.  (1600ms vs 16ms-32ms)
 * 01/20/2012      RC          1.14       Added Firmware version and subsystem to ensemble table.
 * 01/26/2012      RC          1.14       Store subsystem as a byte instead of a string.
 * 01/27/2012      RC          1.14       Added Serial number to the project properties.
 */

using System;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using log4net;

namespace RTI
{

    /// <summary>
    /// Hold the project information.
    /// The ID must be unique and will match
    /// the ID in the database.  The directory
    /// is the location where all the recorded data
    /// will be stored.  The date is to keep track of
    /// the project.
    /// </summary>
    public class Project
    {
        /// <summary>
        /// Logger for logging error messages.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// ID for project if no project 
        /// ID is given.
        /// </summary>
        public const int EmptyID = -1;

        /// <summary>
        /// Name of the project.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Directory the project is located.
        /// </summary>
        public string ProjectDir { get; set; }

        /// <summary>
        /// Folder path to the project.
        /// This will include the directory and
        /// the directory created with the project name.
        /// ProjectDir/ProjectName
        /// It will not end with a '/'.
        /// </summary>
        public string ProjectFolderPath { get; set; }

        /// <summary>
        /// ID within the Project database.
        /// </summary>
        public int ProjectID { get; set; }

        /// <summary>
        /// The date the project was created.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// The last data the project was modified.
        /// </summary>
        public DateTime LastDateModified { get; set; }

        /// <summary>
        /// Serial for the system the project is associated with.
        /// This will determine the subsystems for the project.
        /// </summary>
        public SerialNumber SysSerialNumber { get; set; }

        /// <summary>
        /// Constructor
        /// Set the Database ID, name and directory.
        /// </summary>
        /// <param name="name">Project name.</param>
        /// <param name="dir">Project directory.</param>
        /// <param name="serialNum">System serial number.</param>
        public Project(string name, string dir, string serialNum)
        {
            // Set the initial settings
            ProjectID = Project.EmptyID;
            SetFolders(name, dir);
            DateCreated = DateTime.Now;
            LastDateModified = DateTime.Now;
            SysSerialNumber = new SerialNumber(serialNum);

            // Create the directory if it does not exist
            Directory.CreateDirectory(ProjectFolderPath);

            // Create the database file
            CreateProjectDatabase();
        }

        /// <summary>
        /// Constructor
        /// Set the Database ID, name and directory.
        /// </summary>
        /// <param name="id">Database ID.</param>
        /// <param name="name">Project name.</param>
        /// <param name="dir">Project directory.</param>
        /// <param name="serialNum">System serial number.</param>
        public Project(int id, string name, string dir, string serialNum)
        {
            // Set the initial settings
            ProjectID = id;
            SetFolders(name, dir);
            DateCreated = DateTime.Now;
            LastDateModified = DateTime.Now;
            SysSerialNumber = new SerialNumber(serialNum);

            // Create the directory if it does not exist
            Directory.CreateDirectory(ProjectFolderPath);

            // Create the database file
            CreateProjectDatabase();
        }

        /// <summary>
        /// Constructor
        /// Set the Database ID, name and directory.
        /// </summary>
        /// <param name="id">Database ID.</param>
        /// <param name="name">Project name.</param>
        /// <param name="dir">Project directory.</param>
        /// <param name="dateCreated">Date the project is created.</param>
        /// <param name="lastDateModified">Last date the project was modified.</param>
        /// <param name="serialNum">System serial number.</param>
        public Project(int id, string name, string dir, DateTime dateCreated, DateTime lastDateModified, string serialNum)
        {
            // Set the initial settings
            SetFolders(name, dir);
            ProjectID = id;
            DateCreated = dateCreated;
            LastDateModified = lastDateModified;
            SysSerialNumber = new SerialNumber(serialNum);

            // Create the directory if it does not exist
            Directory.CreateDirectory(ProjectFolderPath);

            // Create the database file
            CreateProjectDatabase();
        }

        /// <summary>
        /// Set the project name,
        /// project folder and project
        /// folder path.
        /// </summary>
        /// <param name="name">Name of the project.</param>
        /// <param name="dir">Project folder.</param>
        private void SetFolders(string name, string dir)
        {
            ProjectName = name;
            ProjectDir = dir;
            ProjectFolderPath = dir + @"\" + name;
        }

        /// <summary>
        /// Create a new Project database that will
        /// store ensemble data.
        /// </summary>
        private void CreateProjectDatabase( )
        {
            // Check if the database exist
            // If it does not, create it
            if (!File.Exists(GetProjectFullPath( )))
            {
                CreateProjectTables( );
            }
        }

        /// <summary>
        /// Create the project table.  This will be the database
        /// and all the tables needed to recorded.
        /// </summary>
        private void CreateProjectTables( )
        {
            // All the possible tables
            var commands = new[]
            {
                "CREATE TABLE tblEnsemble (ID INTEGER PRIMARY KEY AUTOINCREMENT, EnsembleNum INTEGER, NumBins INTEGER NOT NULL, NumBeams INTEGER NOT NULL, DesiredPingCount INTEGER NOT NULL, ActualPingCount INTEGER NOT NULL, Status INTEGER NOT NULL, Year INTEGER NOT NULL, Month INTEGER NOT NULL, Day INTEGER NOT NULL, Hour INTEGER NOT NULL, Minute INTEGER NOT NULL, Second INTEGER NOT NULL, HundSec INTEGER NOT NULL, DateTime DATETIME NOT NULL, FirstBinRange FLOAT NOT NULL, BinSize FLOAT NOT NULL, ProfileFirstPingTime FLOAT NOT NULL, ProfileLastPingTime FLOAT NOT NULL, Heading FLOAT NOT NULL, Pitch FLOAT NOT NULL, Roll FLOAT NOT NULL, WaterTemp FLOAT NOT NULL, SysTemp FLOAT NOT NULL, Salinity FLOAT NOT NULL, Pressure FLOAT NOT NULL, TransducerDepth FLOAT NOT NULL, SpeedOfSound FLOAT NOT NULL, DbTime DATETIME NOT NULL, IsBeamVelAvail BOOLEAN, IsInstrVelAvail BOOLEAN, IsEarthVelAvail BOOLEAN, IsAmpAvail BOOLEAN, IsCorrAvail BOOLEAN, IsGoodBeamAvail BOOLEAN, IsGoodEarthAvail BOOLEAN, IsAncillaryAvail BOOLEAN, IsBottomTrackAvail BOOLEAN, IsNmeaAvail BOOLEAN, NmeaData TEXT, SysSerialNum TEXT, FirmwareMajor TINYINT, FirmwareMinor TINYINT, FirmwareRevision TINYINT, SubsystemIndex TINYINT, BTFirstPingTime FLOAT, BTLastPingTime FLOAT, BTHeading FLOAT, BTPitch FLOAT, BTRoll FLOAT, BTWaterTemp FLOAT, BTSysTemp FLOAT, BTSalinity FLOAT, BTPressure FLOAT, BTSpeedOfSound FLOAT, BTStatus INTEGER, BTActualPingCount FLOAT, BTRangeBeam0 FLOAT, BTRangeBeam1 FLOAT, BTRangeBeam2 FLOAT, BTRangeBeam3 FLOAT, BTSNRBeam0 FLOAT, BTSNRBeam1 FLOAT, BTSNRBeam2 FLOAT, BTSNRBEAM3 FLOAT, BTAmpBeam0 FLOAT, BTAmpBeam1 FLOAT, BTAmpBeam2 FLOAT, BTAmpBeam3 FLOAT, BTCorrBeam0 FLOAT, BTCorrBeam1 FLOAT, BTCorrBeam2 FLOAT, BTCorrBeam3 FLOAT, BTBeamVelBeam0 FLOAT, BTBeamVelBeam1 FLOAT, BTBeamVelBeam2 FLOAT, BTBeamVelBeam3 FLOAT, BTBeamGoodBeam0 FLOAT, BTBeamGoodBeam1 FLOAT, BTBeamGoodBeam2 FLOAT, BTBeamGoodBeam3 FLOAT, BTInstrVelBeam0 FLOAT, BTInstrVelBeam1 FLOAT, BTInstrVelBeam2 FLOAT, BTInstrVelBeam3 FLOAT,  BTInstrGoodBeam0 FLOAT, BTInstrGoodBeam1 FLOAT, BTInstrGoodBeam2 FLOAT, BTInstrGoodBeam3 FLOAT, BTEarthVelBeam0 FLOAT, BTEarthVelBeam1 FLOAT, BTEarthVelBeam2 FLOAT, BTEarthVelBeam3 FLOAT, BTEarthGoodBeam0 FLOAT, BTEarthGoodBeam1 FLOAT, BTEarthGoodBeam2 FLOAT, BTEarthGoodBeam3 FLOAT)",
                "CREATE TABLE tblBeam(ID INTEGER PRIMARY KEY AUTOINCREMENT, EnsembleId INTEGER, BinNum INTEGER NOT NULL, BeamNum SAMLLINT NOT NULL, BeamVel FLOAT, EarthVel FLOAT, InstrVel FLOAT, Amplitude FLOAT, Correlation FLOAT, GoodBeam SMALLINT, GoodEarth SMALLINT, Orientation TINYINT NOT NULL, FOREIGN KEY(EnsembleId) REFERENCES tblEnsemble(ID))",
                "CREATE INDEX idxBeam ON tblBeam(EnsembleId, BinNum)",
            };

            //SQLiteConnection cnn = DbCommon.OpenProjectDB(project);

            string projectConnection = "Data Source=" + GetProjectFullPath();
            SQLiteConnection cnn = new SQLiteConnection(projectConnection);
            cnn.Open();


            // Ensure a connection can be made
            if (cnn == null)
            {
                return;
            }

            // Create the tables using array above
            foreach (var cmd in commands)
            {
                using (DbCommand c = cnn.CreateCommand())
                {
                    c.CommandText = cmd;
                    c.CommandType = System.Data.CommandType.Text;

                    try
                    {
                        c.ExecuteNonQuery();
                    }
                    catch (SQLiteException e)
                    {
                        log.Error("Error creating tables for project: " + ProjectName, e);
                    }
                }
            }
            // Close the database connection
            cnn.Close();
        }

        /// <summary>
        /// Create the full path of the project.  Use
        /// the project information to generate the 
        /// project database file name including directory.
        /// </summary>
        /// <returns>String of the full path of the project.</returns>
        public string GetProjectFullPath( )
        {
            return ProjectFolderPath + @"\" + ProjectName + ".db";
        }

        /// <summary>
        /// Return a string of the full path to the Project image.
        /// The project image is a image representing the project.
        /// </summary>
        /// <returns>Path to project image.</returns>
        public string GetProjectImagePath()
        {
            return ProjectFolderPath + @"\" + ProjectName + ".png";
        }

    }
}