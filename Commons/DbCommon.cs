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
 * 11/04/2011      RC                     Check if a connection is made in RunQueryOnProjectDb().   
 * 11/11/2011      RC                     Added COL_ADCP_C232B and COL_ADCP_C485B.
 * 11/15/2011      RC                     Removed IsDirectReading from Project table.
 *                                         Removed COM Setting variables.
 * 12/09/2011      RC          1.08       Added COL_ENS_NMEA_DATA.
 * 12/19/2011      RC          1.10       Adding System Serial Number and Firmware to database.
 * 12/28/2011      RC          1.10       Use Ensemble's row id instead of number for foreign key for tblBottomTrack and tblBeam.
 * 12/29/2011      RC          1.11       Adding log and removing RecorderManager.
 * 12/30/2011      RC          1.11       Removed getting project db full path.  Now in project.
 * 01/11/2012      RC          1.12       Removed Pulse specific code.  
 *                                         Used Using arounnd connections to dispose.
 * 01/20/2012      RC          1.14       Added column names for Firmware versions and Subsystem.
 * 09/14/2012      RC          2.15       Added column names for Commands and Options for a project.
 * 09/18/2012      RC          2.15       Added COL_ENS_SUBSYS_CONFIG column to the Project database in tblEnsemble.
 * 10/01/2012      RC          2.15       Added COL_CMD_REV column to the Project database in tblOptions.
 * 02/06/2013      RC          2.18       Changed TBL_ENS_CMDS to TBL_ENS_OPTIONS.
 * 02/07/2013      RC          2.18       Added CheckIfColumnExist() and CheckIfTableIsEmpty().
 * 03/06/2013      RC          2.18       Changed the format of the database.
 * 03/08/2013      RC          2.18       For each method call, created a new one with the SQLiteConnection to prevent opening and closing connection for each call.
 *                                         Added RunQueryOnProjectDbObj() to return an object instead of an int.
 * 08/09/2013      RC          2.19.4     Added COL_CMD_APP_CONFIGURATION.
 * 10/28/2013      RC          2.21.0     Added COL_CMD_PROJECT_OPTIONS.
 * 12/31/2013      RC          2.21.2     Added COL_PROFILEENGINEERING_DS and COL_BOTTOMTRACKENGINEERING_DS.
 * 01/09/2014      RC          2.21.3     Added COL_SYSTEMSETUP_DS.
 * 02/07/2014      RC          2.21.3     Added COL_ADCPGPS.
 * 02/13/2014      RC          2.21.3     Added COL_ENS_POSITION.
 * 06/19/2014      RC          2.22.1     Added COL_DVL.
 * 
 */


using System.Data;
using System.Data.SQLite;
using System;
using System.Data.Common;
using log4net;

namespace RTI
{
    /// <summary>
    /// Utilties commonly used in the database applications.
    /// Resolving names to the database and opening the databases.
    /// Converting booleans.
    /// With System.Data.SQLite version 1.0.77 the error message "SQLite error (21): misuse at line 110832 of [a499ae3835]" is occuring.
    /// System.Data.SQLite admins are claiming its harmless.  http://system.data.sqlite.org/index.html/info/e30b820248e1ecdd839e462646f5c9fe5965d6df
    /// It is most likely cause when DbCommand is garbage collected before the conneciton is closed.
    /// 
    /// Query for specific data within specific bins.  This gives ensemble and beam data.
    /// SELECT * FROM (SELECT * FROM tblEnsemble INNER JOIN tblBeam ON tblEnsemble.ID = tblBeam.EnsembleId) WHERE BinNum greater or equal 2;
    /// </summary>
    public class DbCommon
    {
        #region Variables


        /// <summary>
        /// Logger for logging error messages.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Tables

        /// <summary>
        /// Project Database:
        /// Ensemble table name.  Table to hold ensemble data.
        /// </summary>
        public const string TBL_ENS_ENSEMBLE = "tblEnsemble";

        /// <summary>
        /// Project Database:
        /// Beam table name.  Table to hold all data per a beam in an ensemble.
        /// </summary>
        public const string TBL_ENS_BEAM = "tblBeam";

        /// <summary>
        /// Project Database:
        /// Table to hold all the options for the project.
        /// </summary>
        public const string TBL_ENS_OPTIONS = "tblOptions";

        #endregion

        #region Columns

        /// <summary>
        /// Ensemble Table:
        /// Ensemble ID
        /// (INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT)
        /// </summary>
        public const string COL_ENS_ID = "ID";

        /// <summary>
        /// Ensemble Table:
        /// Ensemble Number.
        /// (INTEGER NOT NULL)
        /// </summary>
        public const string COL_ENS_ENS_NUM = "EnsembleNum";

        /// <summary>
        /// Ensemble Table:
        /// DateTime.
        /// (INTEGER NOT NULL)
        /// </summary>
        public const string COL_ENS_DATETIME = "DateTime";

        /// <summary>
        /// Ensemble Table:
        /// Position.
        /// TEXT
        /// </summary>
        public const string COL_ENS_POSITION = "Position";


        #region DataSet Columns

        /// <summary>
        /// Ensemble Table:
        /// Ensemble DataSet
        /// TEXT
        /// </summary>
        public const string COL_ENSEMBLE_DS = "EnsembleDS";

        /// <summary>
        /// Ensemble Table:
        /// Ancillary DataSet
        /// TEXT
        /// </summary>
        public const string COL_ANCILLARY_DS = "AncillaryDS";

        /// <summary>
        /// Ensemble Table:
        /// Amplitude DataSet
        /// TEXT
        /// </summary>
        public const string COL_AMPLITUDE_DS = "AmplitudeDS";

        /// <summary>
        /// Ensemble Table:
        /// Correlation DataSet
        /// TEXT
        /// </summary>
        public const string COL_CORRELATION_DS = "CorrelationDS";

        /// <summary>
        /// Ensemble Table:
        /// Beam Velocity DataSet
        /// TEXT
        /// </summary>
        public const string COL_BEAMVELOCITY_DS = "BeamVelocityDS";

        /// <summary>
        /// Ensemble Table:
        /// Earth Velocity DataSet
        /// TEXT
        /// </summary>
        public const string COL_EARTHVELOCITY_DS = "EarthVelocityDS";

        /// <summary>
        /// Ensemble Table:
        /// Instrument Velocity DataSet
        /// TEXT
        /// </summary>
        public const string COL_INSTRUMENTVELOCITY_DS = "InstrumentVelocityDS";

        /// <summary>
        /// Ensemble Table:
        /// Bottom Track DataSet
        /// TEXT
        /// </summary>
        public const string COL_BOTTOMTRACK_DS = "BottomTrackDS";

        /// <summary>
        /// Ensemble Table:
        /// Good Beam DataSet
        /// TEXT
        /// </summary>
        public const string COL_GOODBEAM_DS = "GoodBeamDS";

        /// <summary>
        /// Ensemble Table:
        /// Good Earth DataSet
        /// TEXT
        /// </summary>
        public const string COL_GOODEARTH_DS = "GoodEarthDS";

        /// <summary>
        /// Ensemble Table:
        /// NMEA DataSet
        /// TEXT
        /// </summary>
        public const string COL_NMEA_DS = "NmeaDS";

        /// <summary>
        /// Ensemble Table:
        /// Earth Water Mass DataSet
        /// TEXT
        /// </summary>
        public const string COL_EARTHWATERMASS_DS = "EarthWaterMassDS";

        /// <summary>
        /// Ensemble Table:
        /// Instrument Water Mass DataSet
        /// TEXT
        /// </summary>
        public const string COL_INSTRUMENTWATERMASS_DS = "InstrumentWaterMassDS";

        /// <summary>
        /// Ensemble Table:
        /// Profile Engineering DataSet
        /// TEXT
        /// </summary>
        public const string COL_PROFILEENGINEERING_DS = "ProfileEngineeringDS";

        /// <summary>
        /// Ensemble Table:
        /// Bottom Track Engineering DataSet
        /// TEXT
        /// </summary>
        public const string COL_BOTTOMTRACKENGINEERING_DS = "BottomTrackEngineeringDS";

        /// <summary>
        /// Ensemble Table:
        /// System Setup DataSet
        /// TEXT
        /// </summary>
        public const string COL_SYSTEMSETUP_DS = "SystemSetupDS";

        /// <summary>
        /// Ensemble Table:
        /// GPS 1.
        /// TEXT
        /// </summary>
        public const string COL_ADCPGPS = "AdcpGpsData";

        /// <summary>
        /// Ensemble Table:
        /// GPS 1.
        /// TEXT
        /// </summary>
        public const string COL_GPS1 = "Gps1Data";

        /// <summary>
        /// Ensemble Table:
        /// GPS 2.
        /// TEXT
        /// </summary>
        public const string COL_GPS2 = "Gps2Data";

        /// <summary>
        /// Ensemble Table:
        /// NMEA 1.
        /// TEXT
        /// </summary>
        public const string COL_NMEA1 = "Nmea1Data";

        /// <summary>
        /// Ensemble Table:
        /// NMEA 2.
        /// TEXT
        /// </summary>
        public const string COL_NMEA2 = "Nmea2Data";

        /// <summary>
        /// Ensemble Table:
        /// DVL.
        /// TEXT
        /// </summary>
        public const string COL_DVL_DS = "DVL";

        #endregion

        #region Commands and Options

        /// <summary>
        /// Comands and Options Table:
        /// Adcp Commands.
        /// (TEXT)
        /// </summary>
        public const string COL_CMD_ADCP_CONFIGURATION = "AdcpConfiguration";

        /// <summary>
        /// Command and Options Table:
        /// Project Options.
        /// (TEXT)
        /// </summary>
        public const string COL_CMD_PROJECT_OPTIONS = "ProjectOptions";

        /// <summary>
        /// Comands and Options Table:
        /// Application Configuration.
        /// (TEXT)
        /// </summary>
        public const string COL_CMD_APP_CONFIGURATION = "AppConfiguration";

        /// <summary>
        /// Comands and Options Table:
        /// Revision of the database.
        /// (TEXT)
        /// </summary>
        public const string COL_CMD_REV = "Revision";

        /// <summary>
        /// Comands and Options Table:
        /// Adcp Commands.
        /// (TEXT)
        /// </summary>
        public const string COL_CMD_MISC = "Misc";

        #endregion

        #endregion

        #endregion

        /// <summary>
        /// Empty Constructor.
        /// </summary>
        public DbCommon()
        {

        }

        #region Open Database

        /// <summary>
        /// Open a database connection to the project.
        /// Given the Project, get the name and directory
        /// of the project.  Then open a connection to the
        /// database.
        /// </summary>
        /// <param name="project">Project that the database will be opened.</param>
        /// <returns>Database connection to Project database.</returns>
        public static SQLiteConnection OpenProjectDB(Project project)
        {
            try
            {
                // Create full path to the project database
                //string projectFullPath = project.ProjectFolderPath + @"\" + project.ProjectName + ".db";
                string projectFullPath = project.GetProjectFullPath();
                string projectConnection = "Data Source=" + projectFullPath;
                var conn = new SQLiteConnection(projectConnection);
                conn.Open();
                return conn;
            }
            catch (SQLiteException e)
            {
                log.Error("Error opening Project database: " + project.ProjectName, e);
                return null;
            }
            catch (Exception e)
            {
                log.Error("Error opening Project database: " + project.ProjectName, e);
                return null;
            }
        }

        #endregion

        #region Query

        /// <summary>
        /// Run a query that will return an Integer.  Give the
        /// project and the query string.  The result will be returned.
        /// If an error, 0 will be returned.
        /// </summary>
        /// <param name="project">Project to query.</param>
        /// <param name="query">Query string.</param>
        /// <returns>Result of query, if error, it will return 0.</returns>
        public static int RunQueryOnProjectDb(Project project, string query)
        {
            int result = 0;

            try
            {
                // Open a connection to the database
                using (SQLiteConnection cnn = DbCommon.OpenProjectDB(project))
                {
                    // Ensure a connection can be made
                    if (cnn == null)
                    {
                        return -1;
                    }

                    using (DbTransaction dbTrans = cnn.BeginTransaction())
                    {
                        using (DbCommand cmd = cnn.CreateCommand())
                        {
                            cmd.CommandText = query;

                            // Get Result
                            object resultValue = cmd.ExecuteScalar();
                            result = Convert.ToInt32(resultValue.ToString());

                        }
                        // Add all the data
                        dbTrans.Commit();
                    }
                    // Close the connection to the database
                    cnn.Close();
                }
            }
            catch (SQLiteException e)
            {
                log.Error(string.Format("Error running query on database: {0} \n{1}", project.ProjectName, query), e);
                return 0;
            }
            catch (Exception e)
            {
                log.Error(string.Format("Unknown Error running query on database: {0} \n{1}", project.ProjectName, query), e);
                return 0;
            }


            return result;
        }

        /// <summary>
        /// Run a query that will return an Integer.  Give the
        /// project and the query string.  The result will be returned.
        /// If an error, 0 will be returned.
        /// </summary>
        /// <param name="cnn">Sqlite Database Connection.</param>
        /// <param name="project">Project to query.</param>
        /// <param name="query">Query string.</param>
        /// <returns>Result of query, if error, it will return 0.</returns>
        public static int RunQueryOnProjectDb(SQLiteConnection cnn, Project project, string query)
        {
            int result = 0;

            try
            {
                // Open a connection to the database
                //using (SQLiteConnection cnn = DbCommon.OpenProjectDB(project))
                //{
                    // Ensure a connection can be made
                    if (cnn == null)
                    {
                        return -1;
                    }

                    using (DbTransaction dbTrans = cnn.BeginTransaction())
                    {
                        using (DbCommand cmd = cnn.CreateCommand())
                        {
                            cmd.CommandText = query;

                            // Get Result
                            object resultValue = cmd.ExecuteScalar();
                            result = Convert.ToInt32(resultValue.ToString());

                        }
                        // Add all the data
                        dbTrans.Commit();
                    }
                    // Close the connection to the database
                    //cnn.Close();
                //}
            }
            catch (SQLiteException e)
            {
                log.Error(string.Format("Error running query on database: {0} \n{1}", project.ProjectName, query), e);
                return 0;
            }
            catch (Exception e)
            {
                log.Error(string.Format("Unknown Error running query on database: {0} \n{1}", project.ProjectName, query), e);
                return 0;
            }


            return result;
        }


        /// <summary>
        /// Run a query that will return an object.  Give the
        /// project and the query string.  The result will be returned.
        /// If an error, null will be returned.
        /// </summary>
        /// <param name="project">Project to query.</param>
        /// <param name="query">Query string.</param>
        /// <returns>Result of query, if error, it will return 0.</returns>
        public static object RunQueryOnProjectDbObj(Project project, string query)
        {
            object result = 0;

            try
            {
                // Open a connection to the database
                using (SQLiteConnection cnn = DbCommon.OpenProjectDB(project))
                {
                    // Ensure a connection can be made
                    if (cnn == null)
                    {
                        return -1;
                    }

                    using (DbTransaction dbTrans = cnn.BeginTransaction())
                    {
                        using (DbCommand cmd = cnn.CreateCommand())
                        {
                            cmd.CommandText = query;

                            // Get Result
                            result = cmd.ExecuteScalar();

                        }
                        // Add all the data
                        dbTrans.Commit();
                    }
                    // Close the connection to the database
                    cnn.Close();
                }
            }
            catch (SQLiteException e)
            {
                log.Error(string.Format("Error running query on database: {0} \n{1}", project.ProjectName, query), e);
                return 0;
            }
            catch (Exception e)
            {
                log.Error(string.Format("Unknown Error running query on database: {0} \n{1}", project.ProjectName, query), e);
                return 0;
            }


            return result;
        }

        /// <summary>
        /// Allows the programmer to run a query against the project database
        /// and return a table with the result.
        /// </summary>
        /// <param name="project">Project to query.</param>
        /// <param name="query">The SQL Query to run</param>
        /// <returns>A DataTable containing the result set.</returns>
        public static DataTable GetDataTableFromProjectDb(Project project, string query)
        {
            DataTable dt = new DataTable();
            try
            {
                // Open a connection to the database
                using (SQLiteConnection cnn = DbCommon.OpenProjectDB(project))
                {
                    // Ensure a connection can be made
                    if (cnn == null)
                    {
                        return dt;
                    }

                    using (DbTransaction dbTrans = cnn.BeginTransaction())
                    {
                        using (DbCommand cmd = cnn.CreateCommand())
                        {
                            cmd.CommandText = query;
                            DbDataReader reader = cmd.ExecuteReader();

                            // Load the datatable with query result
                            dt.Load(reader);

                            // Close the connection
                            reader.Close();
                            cnn.Close();
                        }
                    }
                }

            }
            catch (Exception e)
            {
                if (project != null)
                {
                    log.Error(string.Format("Error populating datatable from {0} database. \n{1}", project.ProjectName, query), e);
                }
                else
                {
                    log.Error(string.Format("Error populating datatable. \n{0}", query), e);
                }
            }
            return dt;
        }

        /// <summary>
        /// Allows the programmer to run a query against the project database
        /// and return a table with the result.
        /// </summary>
        /// <param name="cnn">Sqlite Database Connection.</param>
        /// <param name="project">Project to query.</param>
        /// <param name="query">The SQL Query to run</param>
        /// <returns>A DataTable containing the result set.</returns>
        public static DataTable GetDataTableFromProjectDb(SQLiteConnection cnn, Project project, string query)
        {
            DataTable dt = new DataTable();
            try
            {
                // Open a connection to the database
                //using (SQLiteConnection cnn = DbCommon.OpenProjectDB(project))
                //{
                    // Ensure a connection can be made
                    if (cnn == null)
                    {
                        return dt;
                    }

                    using (DbTransaction dbTrans = cnn.BeginTransaction())
                    {
                        using (DbCommand cmd = cnn.CreateCommand())
                        {
                            cmd.CommandText = query;
                            DbDataReader reader = cmd.ExecuteReader();

                            // Load the datatable with query result
                            dt.Load(reader);

                            // Close the connection
                            reader.Close();
                            //cnn.Close();
                        }
                    }
                //}

            }
            catch (Exception e)
            {
                if (project != null)
                {
                    log.Error(string.Format("Error populating datatable from {0} database. \n{1}", project.ProjectName, query), e);
                }
                else
                {
                    log.Error(string.Format("Error populating datatable. \n{0}", query), e);
                }
            }
            return dt;
        }

        /// <summary>
        /// Query to check if the table contains any rows.  This will check
        /// the table given in the given project for any rows.  If at least
        /// 1 row exist in the table, it will return false.
        /// </summary>
        /// <param name="project">Project to check.</param>
        /// <param name="table">Table to check.</param>
        /// <returns>TRUE = Table is empty.</returns>
        public static bool CheckIfTableIsEmpty(Project project, string table)
        {
            // Create a query to check if the table is empty
            string query = string.Format("SELECT exists(SELECT 1 FROM {0});", table);

            // Check if any rows exist in the table
            if (RunQueryOnProjectDb(project, query) > 0)
            {
                return false;
            }

            // Table is empty
            return true;
        }

        /// <summary>
        /// Query to check if the table contains any rows.  This will check
        /// the table given in the given project for any rows.  If at least
        /// 1 row exist in the table, it will return false.
        /// </summary>
        /// <param name="cnn">Sqlite Database Connection.</param>
        /// <param name="project">Project to check.</param>
        /// <param name="table">Table to check.</param>
        /// <returns>TRUE = Table is empty.</returns>
        public static bool CheckIfTableIsEmpty(SQLiteConnection cnn, Project project, string table)
        {
            // Create a query to check if the table is empty
            string query = string.Format("SELECT exists(SELECT 1 FROM {0});", table);

            // Check if any rows exist in the table
            if (RunQueryOnProjectDb(cnn, project, query) > 0)
            {
                return false;
            }

            // Table is empty
            return true;
        }

        /// <summary>
        /// Check if the given column exist in the given table for the given project.
        /// If the column does not exist, it will return false.  
        /// 
        /// To check if the colun exist, it will run a query to select the columnn given.
        /// If the column does not exist, an exception will be given.  If an exception is
        /// found, it will return false.
        /// </summary>
        /// <param name="project">Project to check.</param>
        /// <param name="column">Column to check.</param>
        /// <param name="table">Table to check.</param>
        /// <returns>TRUE = Column exist in the table.</returns>
        public static bool CheckIfColumnExist(Project project, string column, string table)
        {
            // Create a query to check if the column exist in the table
            // This will try to select the column, if it does not exist, an error will be thrown
            string query = string.Format("SELECT {0} FROM {1};", column, table);

            // Default is true
            bool result = true;

            try
            {
                // Open a connection to the database
                using (SQLiteConnection cnn = DbCommon.OpenProjectDB(project))
                {
                    // Ensure a connection can be made
                    if (cnn == null)
                    {
                        return false;
                    }

                    using (DbTransaction dbTrans = cnn.BeginTransaction())
                    {
                        using (DbCommand cmd = cnn.CreateCommand())
                        {
                            cmd.CommandText = query;

                            // Run the query
                            cmd.ExecuteNonQuery();

                        }
                        // Add all the data
                        dbTrans.Commit();
                    }
                    // Close the connection to the database
                    cnn.Close();
                }
            }
            catch (SQLiteException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }

            return result;
        }

        /// <summary>
        /// Check if the given column exist in the given table for the given project.
        /// If the column does not exist, it will return false.  
        /// 
        /// To check if the colun exist, it will run a query to select the columnn given.
        /// If the column does not exist, an exception will be given.  If an exception is
        /// found, it will return false.
        /// </summary>
        /// <param name="cnn">Sqlite Database Connection.</param>
        /// <param name="project">Project to check.</param>
        /// <param name="column">Column to check.</param>
        /// <param name="table">Table to check.</param>
        /// <returns>TRUE = Column exist in the table.</returns>
        public static bool CheckIfColumnExist(SQLiteConnection cnn, Project project, string column, string table)
        {
            // Create a query to check if the column exist in the table
            // This will try to select the column, if it does not exist, an error will be thrown
            string query = string.Format("SELECT {0} FROM {1};", column, table);

            // Default is true
            bool result = true;

            try
            {
                // Open a connection to the database
                //using (SQLiteConnection cnn = DbCommon.OpenProjectDB(project))
                //{
                    // Ensure a connection can be made
                    if (cnn == null)
                    {
                        return false;
                    }

                    using (DbTransaction dbTrans = cnn.BeginTransaction())
                    {
                        using (DbCommand cmd = cnn.CreateCommand())
                        {
                            cmd.CommandText = query;

                            // Run the query
                            cmd.ExecuteNonQuery();

                        }
                        // Add all the data
                        dbTrans.Commit();
                    }
                    // Close the connection to the database
                    //cnn.Close();
                //}
            }
            catch (SQLiteException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }

            return result;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// SQLite is not converting boolean ranges correctly.
        /// It is storing the text value for true or false when
        /// giving a boolean value.  It should store 1 or 0.
        /// This will convert a boolean value to 1 or 0.
        /// </summary>
        /// <param name="value">Boolean to convert.</param>
        /// <returns>True = 1 / False = 0</returns>
        public static int ConvertBool(bool value)
        {
            if (value)
            {
                return 1;
            }

            return 0;
        }

        #endregion
    }
}