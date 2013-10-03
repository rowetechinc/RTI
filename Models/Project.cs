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
 * 02/06/2013      RC          2.18       Changed the database revision to C1 because we added a new table, tblOptions.
 * 02/07/2013      RC          2.18       Added VerifyDatabase() to verify a database when copying a new project database.
 * 03/08/2013      RC          2.18       Added ValidateVersion() to validate which version of the Project file.
 * 04/09/2013      RC          2.19       Fixed bug where GetAdcpConfigurationFromDb() was commented out.
 * 06/17/2013      RC          2.19       Added BinaryWriter to the write binary data to the project.
 * 06/27/2013      RC          2.19       Added Database reader and writer to the project.
 * 06/28/2013      RC          2.19       Added IDisposable to properly shutdown and replace Shutdown.
 * 07/25/2013      RC          2.19.2     Added Flush() to flush the files.
 * 07/26/2013      RC          2.19.3     Added ProjectEnsembleWriteEvent to send event when an ensemble has been written to the project.
 *                                         Subscribe to dbWriter to know when an ensemble has been written to the project.
 *                                         Subscribe to binaryWriter to know when an ensemble has been written to the binary file.
 * 08/09/2013      RC          2.19.4     Added the column AppConfiguration to tblOptions to hold application specific options.
 *                                         Added WriteAppConfiguration().  Changed Project DB rev to D2.
 */

using System;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using log4net;
using RTI.Commands;
using System.Collections.Generic;
using System.Data;
using System.Text;

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
    public class Project: IDisposable
    {
        #region Variables

        /// <summary>
        /// Logger for logging error messages.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Revision for the Project database.
        /// Revision B includes the Adcp commands and options.
        /// Revision C changed the options table.
        /// Revision D made all the columns JSON data.
        /// Revision D2 added AppConfiguration column.
        /// </summary>
        public const string REV = "D2";

        /// <summary>
        /// ID for project if no project 
        /// ID is given.
        /// </summary>
        public const int EmptyID = -1;

        /// <summary>
        /// Binary writer to buffer writing the
        /// ADCP data to binary file.
        /// </summary>
        private AdcpBinaryWriter _binaryWriter;

        /// <summary>
        /// ADCP database writer.  This will write
        /// data to the database file for the
        /// project.
        /// </summary>
        private AdcpDatabaseWriter _dbWriter;

        /// <summary>
        /// ADCP database reader.
        /// This will read the data from the
        /// database file for the project.
        /// </summary>
        private AdcpDatabaseReader _dbReader;

        #endregion

        #region Properties

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

                // This will reset the file name with the new serial number
                //_binaryWriter.SelectedProject = this;
                if (_binaryWriter != null)
                {
                    _binaryWriter.ResetFileName();
                }
            }
        }

        /// <summary>
        /// Adcp Configuration.  This is all the command options and
        /// deployment options set for the ADCP.
        /// </summary>
        public AdcpConfiguration Configuration { get; set; }

        #endregion

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

            // Initialize the object
            Initialize();
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

            // Initialize the object
            Initialize();
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

            // Initialize the object
            Initialize();
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

            // Initialize the object
            Initialize();
        }

        /// <summary>
        /// Destructor.
        /// Close the writers.
        /// </summary>
        public void Dispose()
        {
            // Unsubscribe
            _dbWriter.EnsembleWriteEvent -= _dbWriter_EnsembleWriteEvent;
            _binaryWriter.EnsembleWriteEvent -= _binaryWriter_EnsembleWriteEvent;

            _dbWriter.Dispose();
            _binaryWriter.Dispose();
            _dbReader.Dispose();
        }

        /// <summary>
        /// Initialize the object
        /// </summary>
        private void Initialize()
        {
            // Create reader and writers
            _binaryWriter = new AdcpBinaryWriter(this);
            _dbWriter = new AdcpDatabaseWriter() { SelectedProject = this };
            _dbReader = new AdcpDatabaseReader();

            // Subscribe to the events of the writer
            _dbWriter.EnsembleWriteEvent += new AdcpDatabaseWriter.EnsembleWriteEventHandler(_dbWriter_EnsembleWriteEvent);
            _binaryWriter.EnsembleWriteEvent += new AdcpBinaryWriter.EnsembleWriteEventHandler(_binaryWriter_EnsembleWriteEvent);

            // Check the if the serial number can
            // be set from the first ensemble if it is empty
            CheckSerialNumber();
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
        /// Check if the serial number is empty.
        /// If it is empty, try to get the serial number
        /// from the first ensemble.
        /// </summary>
        private void CheckSerialNumber()
        {
            if (SerialNumber.IsEmpty())
            {
                // Get the serialnumber from the first ensemble
                DataSet.Ensemble ens = GetFirstEnsemble();
                if (ens != null && ens.IsEnsembleAvail)
                {
                    SerialNumber = ens.EnsembleData.SysSerialNumber;
                }
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
        private void CreateProjectDatabase()
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

                // Verify the database is the latest revision
                VerifyDatabase(GetProjectFullPath());

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
        private void CreateProjectTables()
        {
            // All the possible tables
            var commands = new[]
            {
                "PRAGMA synchronous = OFF",
                "PRAGMA journal_mode = MEMORY",
                //"PRAGMA main.page_size = 4096",
                //"PRAGMA main.cache_size=10000",
                "PRAGMA main.locking_mode=EXCLUSIVE",
                //"PRAGMA main.synchronous=NORMAL",
                //"PRAGMA main.journal_mode=WAL",
                //"PRAGMA main.cache_size=5000",
                "CREATE TABLE tblEnsemble (ID INTEGER PRIMARY KEY AUTOINCREMENT, EnsembleNum INTEGER NOT NULL, DateTime DATETIME NOT NULL, EnsembleDS TEXT, AncillaryDS TEXT, AmplitudeDS TEXT, CorrelationDS TEXT, BeamVelocityDS TEXT, EarthVelocityDS TEXT, InstrumentVelocityDS TEXT, BottomTrackDS TEXT, GoodBeamDS TEXT, GoodEarthDS TEXT, NmeaDS TEXT, EarthWaterMassDS TEXT, InstrumentWaterMassDS TEXT)",
                "CREATE TABLE tblOptions(ID INTEGER PRIMARY KEY AUTOINCREMENT, AdcpConfiguration TEXT, AppConfiguration TEXT, Revision TEXT, Misc TEXT)",
                string.Format("INSERT INTO {0} ({1}, {2}) VALUES ({3}, {4});", DbCommon.TBL_ENS_OPTIONS, DbCommon.COL_CMD_ADCP_CONFIGURATION, DbCommon.COL_CMD_REV, "''", "'D1'"),   // Put at least 1 entry so an insert does not have to be done later
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

        #region Verify Database

        /// <summary>
        /// Verify the database is up to date.
        /// 
        /// Check if tblOptions exist and if not, create the table and set the revision
        /// 
        /// Check if tblOptions is the latest version and if not, replace the table with
        /// the latest version.
        /// 
        /// </summary>
        /// <param name="filepath">Filepath to the database.</param>
        private void VerifyDatabase(string filepath)
        {
            //// Create the table tblOptions if it does not exist and set the revision
            //string queryCreateTableOptions = string.Format("CREATE TABLE IF NOT EXISTS tblOptions(ID INTEGER PRIMARY KEY AUTOINCREMENT, AdcpConfiguration TEXT, Revision TEXT, Misc TEXT);");
            //string queryPopulateTableOptions = string.Format("INSERT INTO {0} ({1}, {2}) VALUES ('{3}', '{4}');", DbCommon.TBL_ENS_OPTIONS, DbCommon.COL_CMD_ADCP_CONFIGURATION, DbCommon.COL_CMD_REV, "", REV);
            //string queryDropTableOptions = string.Format("DROP TABLE IF EXISTS {0};", DbCommon.TBL_ENS_OPTIONS);

            //// Determine which querys need to be run
            //StringBuilder query = new StringBuilder();

            //query.Append(queryCreateTableOptions);          // Always verify tblOptions exist

            //// If the table tblOption is empty, then create a row in the table
            //if (DbCommon.CheckIfTableIsEmpty(this, DbCommon.TBL_ENS_OPTIONS))
            //{
            //    query.Append(queryPopulateTableOptions);
            //}

            //// Check if tblOptions is an older revision table
            //// It the table is older, replace it with a new revision table
            //if (!DbCommon.CheckIfColumnExist(this, DbCommon.COL_CMD_REV, DbCommon.TBL_ENS_OPTIONS))
            //{
            //    query.Append(queryDropTableOptions);
            //    query.Append(queryCreateTableOptions);
            //    query.Append(queryPopulateTableOptions);
            //}

            //// run the queries
            ////DbCommon.RunQueryOnProjectDb(this, query.ToString());

            //try
            //{
            //    // Open a connection to the database
            //    string projectConnection = "Data Source=" + GetProjectFullPath();
            //    using (SQLiteConnection cnn = new SQLiteConnection(projectConnection))
            //    {
            //        // Ensure a connection can be made
            //        if (cnn == null)
            //        {
            //            return;
            //        }

            //        cnn.Open();

            //        using (DbTransaction dbTrans = cnn.BeginTransaction())
            //        {
            //            using (DbCommand cmd = cnn.CreateCommand())
            //            {
            //                cmd.CommandText = query.ToString();

            //                // Run query
            //                cmd.ExecuteNonQuery();

            //            }
            //            // Add all the data
            //            dbTrans.Commit();
            //        }
            //        // Close the connection to the database
            //        cnn.Close();
            //    }
            //}
            //catch (SQLiteException e)
            //{
            //    log.Error("Error validating project database: " + ProjectName, e);
            //}
            //catch (Exception e)
            //{
            //    log.Error("Unknown Error validating project database: " + ProjectName, e);
            //}
        }

        /// <summary>
        /// Validate the version of the Project will work with this version of the software.
        /// If the version is correct, return true.
        /// 
        /// Revision B includes the Adcp commands and options.
        /// Revision C changed the options table.
        /// Revision D made all the columns JSON data.
        /// 
        /// </summary>
        /// <param name="project">Project to check.</param>
        /// <returns>TRUE = version is valid.</returns>
        public static bool ValidateVersion(Project project)
        {
            if(project != null)
            {
                // Get the version
                string version = AdcpDatabaseCodec.GetProjectVersion(project);


                // Return the latest version of the project
                if (version.Contains("D"))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Adcp Commands

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

            return new AdcpSubsystemCommands(asConfig.SubsystemConfig);
        }

        #endregion

        #region AdcpConfiguration

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
        public AdcpConfiguration GetAdcpConfiguration()
        {
            //SubsystemCommandList = GetAllAdcpSubsystemCommands();

            // Get the configuration from the database
            return Configuration = GetAdcpConfigurationFromDb();
        }

        /// <summary>
        /// Get the Adcp Configuration from the database.  This will read the database
        /// table for the configuration.  If one exist, it will set the configuration.
        /// If one does not exist, it will return the default values.
        /// </summary>
        /// <returns>Adcp Configurations found in the project DB file.</returns>
        private AdcpConfiguration GetAdcpConfigurationFromDb()
        {
            AdcpConfiguration config = new AdcpConfiguration(SerialNumber);

            string query = String.Format("SELECT * FROM {0} WHERE ID=1;", DbCommon.TBL_ENS_OPTIONS);
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

        /// <summary>
        /// Write the AdcpConfiguration to the project database file.
        /// </summary>
        public void WriteAdcpConfiguration()
        {
            _dbWriter.UpdateAdcpConfiguration(Configuration);
        }

        #endregion

        #region App Configuration

        /// <summary>
        /// Get the Application Configuration from the database.  This will read the database
        /// table for the configuration.  If one exist, it will set the configuration.
        /// If one does not exist, it will return the default values.
        /// </summary>
        /// <returns>Application Configurations found in the project DB file.</returns>
        public string GetAppConfigurationFromDb()
        {
            string config = "";

            string query = String.Format("SELECT * FROM {0} WHERE ID=1;", DbCommon.TBL_ENS_OPTIONS);
            try
            {
                // Query the database for the ADCP settings
                DataTable dt = DbCommon.GetDataTableFromProjectDb(this, query);

                // Go through the result settings the settings
                // If more than 1 result is found, return the first one found
                foreach (DataRow r in dt.Rows)
                {
                    // Check if there is data
                    if (r[DbCommon.COL_CMD_APP_CONFIGURATION] == DBNull.Value)
                    {
                        break;
                    }

                    // Get the JSON string from the project
                    config = Convert.ToString(r[DbCommon.COL_CMD_APP_CONFIGURATION]);


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

        /// <summary>
        /// Write the JSON string to the project database file.
        /// The parameter is a string so any JSON string can be written
        /// to the project file.  This will support other applications (Non-RTI)
        /// saving settings to the project.
        /// </summary>
        /// <param name="config">JSON string of the config.</param>
        public void WriteAppConfiguration(string config)
        {
            _dbWriter.UpdateAppConfiguration(config);
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

        #region Record

        #region Record Ensembles

        /// <summary>
        /// Record the binary data to the project.
        /// This will take the binary data and add it
        /// to the projects buffer to written to the file.
        /// </summary>
        /// <param name="data">Data to write.</param>
        /// <returns>TRUE = Data written to binary file.</returns>
        public bool RecordBinary(byte[] data)
        {
            if (_binaryWriter != null)
            {
                _binaryWriter.AddIncomingData(data);
            }

            return true;
        }

        /// <summary>
        /// Record to the database project file.
        /// </summary>
        /// <param name="ensemble">Ensemble to record.</param>
        /// <returns>True if ensemble could be recorded.</returns>
        public bool RecordDb(DataSet.Ensemble ensemble)
        {
            if (_dbWriter != null)
            {
                _dbWriter.AddIncomingData(ensemble);
            }

            return true;
        }

        /// <summary>
        /// Flush the writers.  This will flush
        /// the binary and database writer.
        /// </summary>
        public void Flush()
        {
            if (_binaryWriter != null)
            {
                _binaryWriter.Flush();
            }

            if (_dbWriter != null)
            {
                _dbWriter.Flush();
            }
        }

        #endregion

        #region Update GPS Data

        /// <summary>
        /// Update the ensemble with the given Nmea DataSet.  The
        /// ensemble will be based off the ensemble number given.
        /// </summary>
        /// <param name="nmea">Nmea Dataset.</param>
        /// <param name="ensNum">Ensemble number.</param>
        public void UpdateNmeaDataSet(DataSet.NmeaDataSet nmea, int ensNum)
        {
            _dbWriter.UpdateNmeaDataSet(nmea, ensNum);
        }

        #endregion

        #endregion

        #region Get Ensemble

        /// <summary>
        /// Playback the given ensemble.
        /// </summary>
        /// <param name="index">Index for the ensemble.</param>
        /// <returns>Ensemble read from the database.</returns>
        public DataSet.Ensemble GetEnsemble(long index)
        {
            return _dbReader.GetEnsemble(this, index);
        }

        /// <summary>
        /// Get a fixed number of ensembles starting at index from the project.
        /// This will open the database for the given project.   It will then read
        /// the ensemble starting at index.  
        /// 
        /// The index starts with 1 in the database.
        /// </summary>
        /// <param name="index">Start location.</param>
        /// <param name="size">Number of ensembles to read.</param>
        /// <returns>Cache with the read ensembles.</returns>
        public Cache<long, DataSet.Ensemble> GetEnsembles(long index, uint size)
        {
            return _dbReader.GetEnsembles(this, index, size);
        }

        /// <summary>
        /// Return the first ensemble found.  This will get the total number
        /// of ensembles to have a timeout.  It will then start to look for
        /// first ensemble.  The first ensemble found will be returned.
        /// </summary>
        /// <returns>First ensemble in the project.</returns>
        public DataSet.Ensemble GetFirstEnsemble()
        {
            return _dbReader.GetFirstEnsemble(this);
        }

        /// <summary>
        /// Get all the ensembles for the given project.  This will
        /// get ensemble from the database and store to a cache.
        /// </summary>
        /// <returns>Cache of all the ensembles.</returns>
        public Cache<long, DataSet.Ensemble> GetAllEnsembles()
        {
            return _dbReader.GetAllEnsembles(this);
        }

        /// <summary>
        /// Get the number of ensembles in the given
        /// project.
        /// </summary>
        public int GetNumberOfEnsembles()
        {
            return _dbReader.GetNumberOfEnsembles(this);
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

        #region EventHandlers

        /// <summary>
        /// Event recieved when an ensemble has been written to the
        /// database.  This will also return the last row ID of the
        /// ensemble.  The row ID is also the number of ensembles in
        /// the project.
        /// </summary>
        /// <param name="count">Number of ensembles in the database.</param>
        void _dbWriter_EnsembleWriteEvent(long count)
        {
            // Publish that the project has been written to
            PublishProjectEnsembleWrite(count);
        }

        /// <summary>
        /// Event recieved when an ensemble has been written to the
        /// binary file.  This will also return the file size of the
        /// binary file.
        /// </summary>
        /// <param name="count">File size of the binary file in bytes.</param>
        void _binaryWriter_EnsembleWriteEvent(long count)
        {
            // Publish that the project has been written to
            PublishBinaryEnsembleWrite(count);
        }

        #endregion

        #region Events

        #region Project Ensemble Write

        /// <summary>
        /// Event To subscribe to. 
        /// </summary>
        /// <param name="count">Number of ensembles in the database.</param>
        public delegate void ProjectEnsembleWriteEventHandler(long count);

        /// <summary>
        /// Subscribe to this event.  This will hold all subscribers.
        /// 
        /// To subscribe:
        /// prj.ProjectEnsembleWriteEvent += new prj.ProjectEnsembleWriteEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// prj.ProjectEnsembleWriteEvent -= (method to call)
        /// </summary>
        public event ProjectEnsembleWriteEventHandler ProjectEnsembleWriteEvent;

        /// <summary>
        /// Publish that an ensemble has been written to the database (Project).
        /// This will give the number of ensembles in the project.
        /// 
        /// Verify there is a subscriber before calling the
        /// subscribers with the new event.
        /// </summary>
        private void PublishProjectEnsembleWrite(long count)
        {
            if (ProjectEnsembleWriteEvent != null)
            {
                ProjectEnsembleWriteEvent(count);
            }
        }

        #endregion

        #region Binary Ensemble Write

        /// <summary>
        /// Event To subscribe to. 
        /// </summary>
        /// <param name="count">Size of the binary file in bytes.</param>
        public delegate void BinaryEnsembleWriteEventHandler(long count);

        /// <summary>
        /// Subscribe to this event.  This will hold all subscribers.
        /// 
        /// To subscribe:
        /// prj.BinaryEnsembleWriteEvent += new prj.BinaryEnsembleWriteEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// prj.BinaryEnsembleWriteEvent -= (method to call)
        /// </summary>
        public event BinaryEnsembleWriteEventHandler BinaryEnsembleWriteEvent;

        /// <summary>
        /// Publish that an ensemble has been written to the database (Project).
        /// This will give the number of ensembles in the project.
        /// 
        /// Verify there is a subscriber before calling the
        /// subscribers with the new event.
        /// </summary>
        private void PublishBinaryEnsembleWrite(long count)
        {
            if (BinaryEnsembleWriteEvent != null)
            {
                BinaryEnsembleWriteEvent(count);
            }
        }

        #endregion

        #endregion
    }
}