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
 * 02/03/2012      RC          2.00       Added equal, hashcode and ToString method.
 * 05/10/2012      RC          2.11       Added PRAGMA to database creation.
 * 09/14/2012      RC          2.15       Added AdcpCommands, AdcpSubsystemCommand list and Deployment Options property.
 *                                         Get the commands and options from the database if it exist.
 * 09/18/2012      RC          2.15       Added SubsystemConfig column to tblEnsemble for the subsystem configuration.
 * 10/01/2012      RC          2.15       Removed AdcpCommands, DeploymentOptions and AdcpSubsystemCommadns and replaced with AdcpConfiguration.
 * 10/15/2012      RC          2.15       In tblEnsemble changed column SubsystemIndex to SubsystemCode.
 * 12/28/2012      RC          2.17       Moved AdcpSubsystemConfig.Subsystem into AdcpSubsystemConfig.SubsystemConfig.Subsystem.
 * 
 */

using System;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using log4net;
using RTI.Commands;
using System.Collections.Generic;
using System.Data;

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
        /// Revision for the Project database.
        /// Revision B includes the Adcp commands and options.
        /// </summary>
        public const string REV = "B1";

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
        private SerialNumber _serialNumber;
        /// <summary>
        /// Serial for the system the project is associated with.
        /// This will determine the subsystems for the project.
        /// </summary>
        public SerialNumber SerialNumber 
        {
            get { return _serialNumber; } 
            set
            {
                _serialNumber = value;

                // Set the AdcpConfiguration serial number also
                // Check to verify configuration has been initialized
                // it may not have been if the project was created.
                if (Configuration != null && Configuration.SerialNumber != value)
                {
                    Configuration.SerialNumber = value;
                }
            }
        }

        /// <summary>
        /// Adcp Configuration.  This is all the command options and
        /// deployment options set for the ADCP.
        /// </summary>
        public AdcpConfiguration Configuration { get; set; }

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
            CreateSerialNumber(serialNum);

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
            CreateSerialNumber(serialNum);

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
            CreateSerialNumber(serialNum);

            // Create the directory if it does not exist
            Directory.CreateDirectory(ProjectFolderPath);

            // Create the database file
            CreateProjectDatabase();
        }

        /// <summary>
        /// Create a project with a given project database file.
        /// This will copy the given project database file instead
        /// of creating an new empty database file.  The database file
        /// will be renamed to the new project name, but the content within
        /// the database file will not be modified.
        /// </summary>
        /// <param name="name">Name of the project.</param>
        /// <param name="dir">Directory of the projects.</param>
        /// <param name="serialNum">Serial number of the project.</param>
        /// <param name="dbFilePath">File path to the database file.</param>
        public Project(string name, string dir, string serialNum, string dbFilePath)
        {
            // Set the initial settings
            ProjectID = Project.EmptyID;
            SetFolders(name, dir);
            DateCreated = DateTime.Now;
            LastDateModified = DateTime.Now;
            CreateSerialNumber(serialNum);

            // Create the directory if it does not exist
            Directory.CreateDirectory(ProjectFolderPath);

            // Copy the database file
            CopyProjectDatabase(dbFilePath);
        }

        /// <summary>
        /// Create the serial number based off the
        /// serial number string.  If the serial number
        /// is empty, then create an empty serial number.
        /// </summary>
        /// <param name="serialNumStr">Serial Number string.</param>
        private void CreateSerialNumber(string serialNumStr)
        {
            if (string.IsNullOrEmpty(serialNumStr))
            {
                SerialNumber = new SerialNumber();
            }
            else
            {
                SerialNumber = new SerialNumber(serialNumStr);
            }
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

        #region Database

        /// <summary>
        /// Create a new Project database that will
        /// store ensemble data.
        /// </summary>
        private void CreateProjectDatabase( )
        {
            // Check if the database exist
            // If it does not, create it
            if (!File.Exists(GetProjectFullPath()))
            {
                // Create the database
                CreateProjectTables();

                // Create all the commands and options with default values
                CreateAdcpConfiguration();
            }
            else
            {
                // Get the command and options from the database
                GetAdcpConfiguration();
            }
        }

        /// <summary>
        /// Copy the Project database file.  If a project is being
        /// imported, then the project database file will need to be
        /// copied to the new project location.  Copy the file to the new
        /// project folder.  Rename the new file to the new project name.
        /// Then get the configuration from the project database.
        /// </summary>
        /// <param name="dbFilePath">File path to the project DB to copy.</param>
        private void CopyProjectDatabase(string dbFilePath)
        {
            // Check if the database exist
            // If it does not, create it
            if (!File.Exists(GetProjectFullPath()))
            {
                // Copy the database file to the project folder
                // This will rename the database file given to the
                // the new project name
                FileInfo file = new FileInfo(dbFilePath);
                file.CopyTo(GetProjectFullPath());

                // Get the command and options from the database
                GetAdcpConfiguration();
            }
        }

        /// <summary>
        /// Create the project table.  This will be the database
        /// and all the tables needed to recorded.
        /// 
        /// With synchronous OFF (0), SQLite continues without syncing as soon as it 
        /// has handed data off to the operating system. If the application running 
        /// SQLite crashes, the data will be safe, but the database might become 
        /// corrupted if the operating system crashes or the computer loses power 
        /// before that data has been written to the disk surface. On the other hand, 
        /// some operations are as much as 50 or more times faster with synchronous OFF.
        /// 
        /// The MEMORY journaling mode stores the rollback journal in volatile RAM. This 
        /// saves disk I/O but at the expense of database safety and integrity. If the 
        /// application using SQLite crashes in the middle of a transaction when the 
        /// MEMORY journaling mode is set, then the database file will very likely go 
        /// corrupt.
        /// 
        /// </summary>
        private void CreateProjectTables( )
        {
            // All the possible tables
            var commands = new[]
            {
                "PRAGMA synchronous = OFF",
                "PRAGMA journal_mode = MEMORY",
                "CREATE TABLE tblEnsemble (ID INTEGER PRIMARY KEY AUTOINCREMENT, EnsembleNum INTEGER, NumBins INTEGER NOT NULL, NumBeams INTEGER NOT NULL, DesiredPingCount INTEGER NOT NULL, ActualPingCount INTEGER NOT NULL, Status INTEGER NOT NULL, Year INTEGER NOT NULL, Month INTEGER NOT NULL, Day INTEGER NOT NULL, Hour INTEGER NOT NULL, Minute INTEGER NOT NULL, Second INTEGER NOT NULL, HundSec INTEGER NOT NULL, DateTime DATETIME NOT NULL, FirstBinRange FLOAT NOT NULL, BinSize FLOAT NOT NULL, ProfileFirstPingTime FLOAT NOT NULL, ProfileLastPingTime FLOAT NOT NULL, Heading FLOAT NOT NULL, Pitch FLOAT NOT NULL, Roll FLOAT NOT NULL, WaterTemp FLOAT NOT NULL, SysTemp FLOAT NOT NULL, Salinity FLOAT NOT NULL, Pressure FLOAT NOT NULL, TransducerDepth FLOAT NOT NULL, SpeedOfSound FLOAT NOT NULL, DbTime DATETIME NOT NULL, IsBeamVelAvail BOOLEAN, IsInstrVelAvail BOOLEAN, IsEarthVelAvail BOOLEAN, IsAmpAvail BOOLEAN, IsCorrAvail BOOLEAN, IsGoodBeamAvail BOOLEAN, IsGoodEarthAvail BOOLEAN, IsAncillaryAvail BOOLEAN, IsBottomTrackAvail BOOLEAN, IsNmeaAvail BOOLEAN, NmeaData TEXT, SysSerialNum TEXT, FirmwareMajor TINYINT, FirmwareMinor TINYINT, FirmwareRevision TINYINT, SubsystemCode TINYINT, SubsystemConfig TINYINT, BTFirstPingTime FLOAT, BTLastPingTime FLOAT, BTHeading FLOAT, BTPitch FLOAT, BTRoll FLOAT, BTWaterTemp FLOAT, BTSysTemp FLOAT, BTSalinity FLOAT, BTPressure FLOAT, BTSpeedOfSound FLOAT, BTStatus INTEGER, BTActualPingCount FLOAT, BTRangeBeam0 FLOAT, BTRangeBeam1 FLOAT, BTRangeBeam2 FLOAT, BTRangeBeam3 FLOAT, BTSNRBeam0 FLOAT, BTSNRBeam1 FLOAT, BTSNRBeam2 FLOAT, BTSNRBEAM3 FLOAT, BTAmpBeam0 FLOAT, BTAmpBeam1 FLOAT, BTAmpBeam2 FLOAT, BTAmpBeam3 FLOAT, BTCorrBeam0 FLOAT, BTCorrBeam1 FLOAT, BTCorrBeam2 FLOAT, BTCorrBeam3 FLOAT, BTBeamVelBeam0 FLOAT, BTBeamVelBeam1 FLOAT, BTBeamVelBeam2 FLOAT, BTBeamVelBeam3 FLOAT, BTBeamGoodBeam0 FLOAT, BTBeamGoodBeam1 FLOAT, BTBeamGoodBeam2 FLOAT, BTBeamGoodBeam3 FLOAT, BTInstrVelBeam0 FLOAT, BTInstrVelBeam1 FLOAT, BTInstrVelBeam2 FLOAT, BTInstrVelBeam3 FLOAT,  BTInstrGoodBeam0 FLOAT, BTInstrGoodBeam1 FLOAT, BTInstrGoodBeam2 FLOAT, BTInstrGoodBeam3 FLOAT, BTEarthVelBeam0 FLOAT, BTEarthVelBeam1 FLOAT, BTEarthVelBeam2 FLOAT, BTEarthVelBeam3 FLOAT, BTEarthGoodBeam0 FLOAT, BTEarthGoodBeam1 FLOAT, BTEarthGoodBeam2 FLOAT, BTEarthGoodBeam3 FLOAT)",
                "CREATE TABLE tblBeam(ID INTEGER PRIMARY KEY AUTOINCREMENT, EnsembleId INTEGER, BinNum INTEGER NOT NULL, BeamNum SAMLLINT NOT NULL, BeamVel FLOAT, EarthVel FLOAT, InstrVel FLOAT, Amplitude FLOAT, Correlation FLOAT, GoodBeam SMALLINT, GoodEarth SMALLINT, Orientation TINYINT NOT NULL, FOREIGN KEY(EnsembleId) REFERENCES tblEnsemble(ID))",
                "CREATE TABLE tblOptions(ID INTEGER PRIMARY KEY AUTOINCREMENT, AdcpConfiguration TEXT, Revision TEXT, Misc TEXT)",
                "CREATE INDEX idxBeam ON tblBeam(EnsembleId, BinNum)",
                string.Format("INSERT INTO {0} ({1}, {2}) VALUES ({3}, {4});", DbCommon.TBL_ENS_CMDS, DbCommon.COL_CMD_ADCP_CONFIGURATION, DbCommon.COL_CMD_REV, "''", "'B1'"),   // Put at least 1 entry so an insert does not have to be done later
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

        #endregion

        #region Commands and Options

        /// <summary>
        /// Create the commands for the project.
        /// These are all the commands for the project and they are set to default.
        /// </summary>
        private void CreateAdcpConfiguration()
        {
            // Create the Adcp Configuration
            Configuration = new AdcpConfiguration(SerialNumber);
        }

        /// <summary>
        /// Get the Commands and deployment options from the database.
        /// If they could not be found in the database, then it will 
        /// give default values.
        /// </summary>
        public void GetAdcpConfiguration()
        {
            //SubsystemCommandList = GetAllAdcpSubsystemCommands();

            // Get the configuration from the database
            Configuration = GetAdcpConfigurationFromDb();
        }

        /// <summary>
        /// Get the AdcpSubsystemCommands from the list of commands stored in the project.
        /// This will assume the list of AdcpSubsystemCommands has already been populated.
        /// It will then search for the AdcpSubsystemCommands for the given subsystem.
        /// If the AdcpSubsystemCommands cannot be found, a default set of commands will
        /// be returned.
        /// </summary>
        /// <param name="asConfig">AdcpSubsystemConfig for the commands that are requested.</param>
        /// <returns>Adcp Subsystem Commands from the list of AdcpSubsystemCommands.</returns>
        public AdcpSubsystemCommands GetAdcpSubsystemCommands(AdcpSubsystemConfig asConfig)
        {
            foreach (AdcpSubsystemConfig config in Configuration.SubsystemConfigDict.Values)
            {
                if (config == asConfig)
                {
                    return config.Commands;
                }
            }

            return new AdcpSubsystemCommands(asConfig.SubsystemConfig.SubSystem, asConfig.CepoIndex);
        }

        #endregion

        #region AdcpConfiguration

        /// <summary>
        /// Get the Adcp Configuration from the database.  This will read the database
        /// table for the configuration.  If one exist, it will set the configuration.
        /// If one does not exist, it will return the default values.
        /// </summary>
        /// <returns>Adcp Configurations found in the project DB file.</returns>
        private AdcpConfiguration GetAdcpConfigurationFromDb()
        {
            AdcpConfiguration config = new AdcpConfiguration(SerialNumber);

            string query = String.Format("SELECT * FROM {0} WHERE ID=1;", DbCommon.TBL_ENS_CMDS);
            try
            {
                // Query the database for the ADCP settings
                DataTable dt = DbCommon.GetDataTableFromProjectDb(this, query);

                // Go through the result settings the settings
                // If more than 1 result is found, return the first one found
                foreach (DataRow r in dt.Rows)
                {
                    // Check if there is data
                    if (r[DbCommon.COL_CMD_ADCP_CONFIGURATION] == DBNull.Value)
                    {
                        break;
                    }

                    // This will call the default constructor or pass to the constructor parameter a null
                    // The constructor parameter must be set after creating the object.
                    string json = Convert.ToString(r[DbCommon.COL_CMD_ADCP_CONFIGURATION]);
                    if (!String.IsNullOrEmpty(json))
                    {
                        config = Newtonsoft.Json.JsonConvert.DeserializeObject<AdcpConfiguration>(json);
                    }


                    // Only read the first row
                    break;
                }
            }
            catch (SQLiteException e)
            {
                log.Error("SQL Error getting ADCP Configuration from the project.", e);
            }
            catch (Exception ex)
            {
                log.Error("Error getting ADCP Configuration from the project.", ex);
            }

            return config;
        }

        #endregion

        #region Folder Paths

        /// <summary>
        /// Create the full path of the project.  Use
        /// the project information to generate the 
        /// project database file name including directory.
        /// </summary>
        /// <returns>String of the full path of the project.</returns>
        public string GetProjectFullPath()
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

        #endregion

        #region Override Methods

        /// <summary>
        /// Check if two projects are equal by checking if
        /// there project name and project directory are the same.
        /// </summary>
        /// <param name="obj">Object to compare against.</param>
        /// <returns>True if they are equal.</returns>
        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            Project p = (Project)obj;

            //return (Code == p.Code);
            return (ProjectDir == p.ProjectDir) && (ProjectName == p.ProjectName);
        }

        /// <summary>
        /// Return the string representation of this object.
        /// This will return the full folder path.
        /// </summary>
        /// <returns>String of this project.</returns>
        public override string ToString()
        {
            return ProjectFolderPath;
        }

        /// <summary>
        /// Check if two projects are equal by checking if
        /// there project name and project directory are the same.
        /// </summary>
        /// <param name="project1">First project to check.</param>
        /// <param name="project2">Project to check against.</param>
        /// <returns>True if there codes match.</returns>
        public static bool operator ==(Project project1, Project project2)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(project1, project2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)project1 == null) || ((object)project2 == null))
            {
                return false;
            }

            // Return true if the fields match:
            return (project1.ProjectName == project2.ProjectName) && (project1.ProjectDir == project2.ProjectDir);
        }


        /// <summary>
        /// Return the opposite of ==.
        /// </summary>
        /// <param name="project1">First project to check.</param>
        /// <param name="project2">Project to check against.</param>
        /// <returns>Return the opposite of ==.</returns>
        public static bool operator !=(Project project1, Project project2)
        {
            return !(project1 == project2);
        }

        /// <summary>
        /// Hash code for the object.
        /// </summary>
        /// <returns>Returns the base hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

    }
}