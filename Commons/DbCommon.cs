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
    /// </summary>
    public class DbCommon
    {
        #region Variables


        /// <summary>
        /// Logger for logging error messages.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Columns

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

        #endregion

        #region Ensemble Columns

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
        /// Number of bins.
        /// (INTEGER NOT NULL)
        /// </summary>
        public const string COL_ENS_NUM_BIN = "NumBins";

        /// <summary>
        /// Ensemble Table:
        /// Number of beams.
        /// (INTEGER NOT NULL)
        /// </summary>
        public const string COL_ENS_NUM_BEAM = "NumBeams";

        /// <summary>
        /// Ensemble Table:
        /// Desired Number of Pings.
        /// (INTEGER NOT NULL)
        /// </summary>
        public const string COL_ENS_DES_PING_COUNT = "DesiredPingCount";

        /// <summary>
        /// Ensemble Table:
        /// Actual Number of Pings.
        /// (INTEGER NOT NULL)
        /// </summary>
        public const string COL_ENS_ACT_PING_COUNT = "ActualPingCount";

        /// <summary>
        /// Ensemble Table:
        /// Status of the system.
        /// (INTEGER NOT NULL)
        /// </summary>
        public const string COL_ENS_STATUS = "Status";

        /// <summary>
        /// Ensemble Table:
        /// Year.
        /// (INTEGER NOT NULL)
        /// </summary>
        public const string COL_ENS_YEAR = "Year";

        /// <summary>
        /// Ensemble Table:
        /// Month.
        /// (INTEGER NOT NULL)
        /// </summary>
        public const string COL_ENS_MONTH = "Month";

        /// <summary>
        /// Ensemble Table:
        /// Day.
        /// (INTEGER NOT NULL)
        /// </summary>
        public const string COL_ENS_DAY = "Day";

        /// <summary>
        /// Ensemble Table:
        /// Hour.
        /// (INTEGER NOT NULL)
        /// </summary>
        public const string COL_ENS_HOUR = "Hour";

        /// <summary>
        /// Ensemble Table:
        /// Minute.
        /// (INTEGER NOT NULL)
        /// </summary>
        public const string COL_ENS_MINUTE = "Minute";

        /// <summary>
        /// Ensemble Table:
        /// Second.
        /// (INTEGER NOT NULL)
        /// </summary>
        public const string COL_ENS_SECOND = "Second";

        /// <summary>
        /// Ensemble Table:
        /// Hundredth of a second.
        /// (INTEGER NOT NULL)
        /// </summary>
        public const string COL_ENS_HUN_SECOND = "HundSec";

        /// <summary>
        /// Ensemble Table:
        /// DateTime.
        /// (INTEGER NOT NULL)
        /// </summary>
        public const string COL_ENS_DATETIME = "DateTime";

        /// <summary>
        /// Ensemble Table:
        /// Range to first bin.
        /// (INTEGER NOT NULL)
        /// </summary>
        public const string COL_ENS_FIRST_BIN_RANGE = "FirstBinRange";

        /// <summary>
        /// Ensemble Table:
        /// Bin Size.
        /// (INTEGER NOT NULL)
        /// </summary>
        public const string COL_ENS_BIN_SIZE = "BinSize";

        /// <summary>
        /// Ensemble Table:
        /// First Ping Time.
        /// (REAL NOT NULL)
        /// </summary>
        public const string COL_ENS_FIRST_PING_TIME = "ProfileFirstPingTime";

        /// <summary>
        /// Ensemble Table:
        /// Last Ping Time.
        /// (REAL NOT NULL)
        /// </summary>
        public const string COL_ENS_LAST_PING_TIME = "ProfileLastPingTime";

        /// <summary>
        /// Ensemble Table:
        /// Heading.
        /// (REAL NOT NULL)
        /// </summary>
        public const string COL_ENS_HEADING = "Heading";

        /// <summary>
        /// Ensemble Table:
        /// Pitch.
        /// (REAL NOT NULL)
        /// </summary>
        public const string COL_ENS_PITCH = "Pitch";

        /// <summary>
        /// Ensemble Table:
        /// Roll.
        /// (REAL NOT NULL)
        /// </summary>
        public const string COL_ENS_ROLL = "Roll";

        /// <summary>
        /// Ensemble Table:
        /// Water Temperature.
        /// (REAL NOT NULL)
        /// </summary>
        public const string COL_ENS_TEMP_WATER = "WaterTemp";


        /// <summary>
        /// Ensemble Table:
        /// System Temperature.
        /// (REAL NOT NULL)
        /// </summary>
        public const string COL_ENS_TEMP_SYS = "SysTemp";

        /// <summary>
        /// Ensemble Table:
        /// Salinity.
        /// (REAL NOT NULL)
        /// </summary>
        public const string COL_ENS_SALINITY = "Salinity";

        /// <summary>
        /// Ensemble Table:
        /// Pressure.
        /// (REAL NOT NULL)
        /// </summary>
        public const string COL_ENS_PRESSURE = "Pressure";

        /// <summary>
        /// Ensemble Table:
        /// Transducer Depth.
        /// (REAL NOT NULL)
        /// </summary>
        public const string COL_ENS_XDCR_DEPTH = "TransducerDepth";

        /// <summary>
        /// Ensemble Table:
        /// Speed of Sound.
        /// (REAL NOT NULL)
        /// </summary>
        public const string COL_ENS_SOS = "SpeedOfSound";

        /// <summary>
        /// Ensemble Table:
        /// DateTime ensemble was added to the database.
        /// (DATETIME NOT NULL)
        /// </summary>
        public const string COL_ENS_DB_TIME = "DbTime";

        /// <summary>
        /// Ensemble Table:
        /// Is Beam Velocity Available.
        /// (BOOLEAN)
        /// </summary>
        public const string COL_ENS_IS_BEAM_VEL_AVAIL = "IsBeamVelAvail";

        /// <summary>
        /// Ensemble Table:
        /// Is Instrument Velocity Available.
        /// (BOOLEAN)
        /// </summary>
        public const string COL_ENS_IS_INSTR_VEL_AVAIL = "IsInstrVelAvail";

        /// <summary>
        /// Ensemble Table:
        /// Is Earth Velocity Available.
        /// (BOOLEAN)
        /// </summary>
        public const string COL_ENS_IS_EARTH_VEL_AVAIL = "IsEarthVelAvail";

        /// <summary>
        /// Ensemble Table:
        /// Is Amplitude Available.
        /// (BOOLEAN)
        /// </summary>
        public const string COL_ENS_IS_AMP_AVAIL = "IsAmpAvail";

        /// <summary>
        /// Ensemble Table:
        /// Is Correlation Available.
        /// (BOOLEAN)
        /// </summary>
        public const string COL_ENS_IS_CORR_AVAIL = "IsCorrAvail";

        /// <summary>
        /// Ensemble Table:
        /// Is Good Beam Available.
        /// (BOOLEAN)
        /// </summary>
        public const string COL_ENS_IS_GB_AVAIL = "IsGoodBeamAvail";

        /// <summary>
        /// Ensemble Table:
        /// Is Good Earth Available.
        /// (BOOLEAN)
        /// </summary>
        public const string COL_ENS_IS_GE_AVAIL = "IsGoodEarthAvail";

        /// <summary>
        /// Ensemble Table:
        /// Is Ancillary Available.
        /// (BOOLEAN)
        /// </summary>
        public const string COL_ENS_IS_ANCIL_AVAIL = "IsAncillaryAvail";

        /// <summary>
        /// Ensemble Table:
        /// Is Bottom Track Available.
        /// (BOOLEAN)
        /// </summary>
        public const string COL_ENS_IS_BT_AVAIL = "IsBottomTrackAvail";

        /// <summary>
        /// Ensemble Table:
        /// Is NMEA Available.
        /// (BOOLEAN)
        /// </summary>
        public const string COL_ENS_IS_NMEA_AVAIL = "IsNmeaAvail";

        /// <summary>
        /// Ensemble Table:
        /// NMEA strings.
        /// (TEXT)
        /// </summary>
        public const string COL_ENS_NMEA_DATA = "NmeaData";

        /// <summary>
        /// Ensemble Table:
        /// System Serial Number.
        /// (TEXT)
        /// </summary>
        public const string COL_ENS_SYS_SERIAL = "SysSerialNum";

        /// <summary>
        /// Ensemble Table:
        /// Firmware version.
        /// (TEXT)
        /// </summary>
        public const string COL_ENS_FIRMWARE = "Firmware";

        #endregion

        #region Beam Columns

        /// <summary>
        /// Beam Table:
        /// Row ID.
        /// (INTEGER PRIMARY KEY AUTOINCREMENT) 
        /// </summary>
        public const string COL_BV_ID = "ID";

        /// <summary>
        /// Beam Table:
        /// Ensemble Id.
        /// (INTEGER FOREIGN KEY)
        /// </summary>
        public const string COL_BV_ENS_ID = "EnsembleId";

        /// <summary>
        /// Beam Table:
        /// Bin Number.
        /// (INTEGER NOT NULL) 
        /// </summary>
        public const string COL_BV_BIN_NUM = "BinNum";

        /// <summary>
        /// Beam Table:
        /// Beam Number.
        /// (SAMLLINT NOT NULL)
        /// </summary>
        public const string COL_BV_BEAM_NUM = "BeamNum";

        /// <summary>
        /// Beam Table:
        /// Beam Orientation.
        /// (TINYINT NOT NULL)
        /// </summary>
        public const string COL_BV_ORIENT = "Orientation";

        /// <summary>
        /// Beam Table:
        /// Beam Velocity data.
        /// (REAL) 
        /// </summary>
        public const string COL_BV_VEL_BEAM = "BeamVel";

        /// <summary>
        /// Beam Table:
        /// Earth Velocity data.
        /// (REAL)
        /// </summary>
        public const string COL_BV_VEL_EARTH = "EarthVel";

        /// <summary>
        /// Beam Table:
        /// Instrument Velocity data.
        /// (REAL)
        /// </summary>
        public const string COL_BV_VEL_INSTR = "InstrVel";

        /// <summary>
        /// Beam Table:
        /// Amplitude data.
        /// (REAL)
        /// </summary>
        public const string COL_BV_AMP = "Amplitude";

        /// <summary>
        /// Beam Table:
        /// Correlation data.
        /// (REAL)
        /// </summary>
        public const string COL_BV_CORR = "Correlation";

        /// <summary>
        /// Beam Table:
        /// Good Beam data.
        /// (SMALLINT)
        /// </summary>
        public const string COL_BV_GOOD_BEAM = "GoodBeam";

        /// <summary>
        /// Beam Table:
        /// Good Earth data.
        /// (SMALLINT)
        /// </summary>
        public const string COL_BV_GOOD_EARTH = "GoodEarth";

        #endregion

        #region Bottom Track Columns

        /// <summary>
        /// Bottom Track Table:
        /// First Ping Time.
        /// (REAL)
        /// </summary>
        public const string COL_BT_FIRST_PING_TIME = "BTFirstPingTime";

        /// <summary>
        /// Bottom Track Table:
        /// Last Ping Time.
        /// (REAL)
        /// </summary>
        public const string COL_BT_LAST_PING_TIME = "BTLastPingTime";

        /// <summary>
        /// Bottom Track Table:
        /// Heading.
        /// (REAL)
        /// </summary>
        public const string COL_BT_HEADING = "BTHeading";

        /// <summary>
        /// Bottom Track Table:
        /// Pitch.
        /// (REAL)
        /// </summary>
        public const string COL_BT_PITCH = "BTPitch";

        /// <summary>
        /// Bottom Track Table:
        /// Roll.
        /// (REAL)
        /// </summary>
        public const string COL_BT_ROLL = "BTRoll";

        /// <summary>
        /// Bottom Track Table:
        /// Water Temperature.
        /// (REAL)
        /// </summary>
        public const string COL_BT_TEMP_WATER = "BTWaterTemp";

        /// <summary>
        /// Bottom Track Table:
        /// System Temperature.
        /// (REAL)
        /// </summary>
        public const string COL_BT_TEMP_SYS = "BTSysTemp";

        /// <summary>
        /// Bottom Track Table:
        /// Salinity.
        /// (REAL)
        /// </summary>
        public const string COL_BT_SALINITY = "BTSalinity";

        /// <summary>
        /// Bottom Track Table:
        /// Pressure.
        /// (REAL)
        /// </summary>
        public const string COL_BT_PRESSURE = "BTPressure";

        /// <summary>
        /// Bottom Track Table:
        /// Speed of Sound.
        /// (REAL)
        /// </summary>
        public const string COL_BT_SOS = "BTSpeedOfSound";

        /// <summary>
        /// Bottom Track Table:
        /// Status.
        /// (REAL)
        /// </summary>
        public const string COL_BT_STATUS = "BTStatus";

        /// <summary>
        /// Bottom Track Table:
        /// Actual Ping Count.
        /// (REAL)
        /// </summary>
        public const string COL_BT_ACTUAL_PING_COUNT = "BTActualPingCount";

        /// <summary>
        /// Bottom Track Table:
        /// Range Beam 0.
        /// (REAL)
        /// </summary>
        public const string COL_BT_RANGE_B0 = "BTRangeBeam0";

        /// <summary>
        /// Bottom Track Table:
        /// Range Beam 1.
        /// (REAL)
        /// </summary>
        public const string COL_BT_RANGE_B1 = "BTRangeBeam1";

        /// <summary>
        /// Bottom Track Table:
        /// Range Beam 2.
        /// (REAL)
        /// </summary>
        public const string COL_BT_RANGE_B2 = "BTRangeBeam2";

        /// <summary>
        /// Bottom Track Table:
        /// Range Beam 3.
        /// (REAL)
        /// </summary>
        public const string COL_BT_RANGE_B3 = "BTRangeBeam3";

        /// <summary>
        /// Bottom Track Table:
        /// Signal to Noise Ratio Beam 0.
        /// (REAL)
        /// </summary>
        public const string COL_BT_SNR_B0 = "BTSNRBeam0";

        /// <summary>
        /// Bottom Track Table:
        /// Signal to Noise Ratio Beam 1.
        /// (REAL)
        /// </summary>
        public const string COL_BT_SNR_B1 = "BTSNRBeam1";

        /// <summary>
        /// Bottom Track Table:
        /// Signal to Noise Ratio Beam 2.
        /// (REAL)
        /// </summary>
        public const string COL_BT_SNR_B2 = "BTSNRBeam2";

        /// <summary>
        /// Bottom Track Table:
        /// Signal to Noise Ratio Beam 3.
        /// (REAL)
        /// </summary>
        public const string COL_BT_SNR_B3 = "BTSNRBEAM3";

        /// <summary>
        /// Bottom Track Table:
        /// Amplitude Beam 0.
        /// (REAL)
        /// </summary>
        public const string COL_BT_AMP_B0 = "BTAmpBeam0";

        /// <summary>
        /// Bottom Track Table:
        /// Amplitude Beam 1.
        /// (REAL)
        /// </summary>
        public const string COL_BT_AMP_B1 = "BTAmpBeam1";

        /// <summary>
        /// Bottom Track Table:
        /// Amplitude Beam 2.
        /// (REAL)
        /// </summary>
        public const string COL_BT_AMP_B2 = "BTAmpBeam2";

        /// <summary>
        /// Bottom Track Table:
        /// Amplitude Beam 3.
        /// (REAL)
        /// </summary>
        public const string COL_BT_AMP_B3 = "BTAmpBeam3";

        /// <summary>
        /// Bottom Track Table:
        /// Correlation Beam 0.
        /// (REAL)
        /// </summary>
        public const string COL_BT_CORR_B0 = "BTCorrBeam0";

        /// <summary>
        /// Bottom Track Table:
        /// Correlation Beam 1.
        /// (REAL)
        /// </summary>
        public const string COL_BT_CORR_B1 = "BTCorrBeam1";

        /// <summary>
        /// Bottom Track Table:
        /// Correlation Beam 2.
        /// (REAL)
        /// </summary>
        public const string COL_BT_CORR_B2 = "BTCorrBeam2";

        /// <summary>
        /// Bottom Track Table:
        /// Correlation Beam 3.
        /// (REAL)
        /// </summary>
        public const string COL_BT_CORR_B3 = "BTCorrBeam3";

        /// <summary>
        /// Bottom Track Table:
        /// Beam Velocity Beam 0.
        /// (REAL)
        /// </summary>
        public const string COL_BT_BEAM_VEL_B0 = "BTBeamVelBeam0";

        /// <summary>
        /// Bottom Track Table:
        /// Beam Velocity Beam 1.
        /// (REAL)
        /// </summary>
        public const string COL_BT_BEAM_VEL_B1 = "BTBeamVelBeam1";

        /// <summary>
        /// Bottom Track Table:
        /// Beam Velocity Beam 2.
        /// (REAL)
        /// </summary>
        public const string COL_BT_BEAM_VEL_B2 = "BTBeamVelBeam2";

        /// <summary>
        /// Bottom Track Table:
        /// Beam Velocity Beam 3.
        /// (REAL)
        /// </summary>
        public const string COL_BT_BEAM_VEL_B3 = "BTBeamVelBeam3";

        /// <summary>
        /// Bottom Track Table:
        /// Beam Velocity Good Beam Beam 0.
        /// (REAL)
        /// </summary>
        public const string COL_BT_BEAM_GOOD_B0 = "BTBeamGoodBeam0";

        /// <summary>
        /// Bottom Track Table:
        /// Beam Velocity Good Beam Beam 1.
        /// (REAL)
        /// </summary>
        public const string COL_BT_BEAM_GOOD_B1 = "BTBeamGoodBeam1";

        /// <summary>
        /// Bottom Track Table:
        /// Beam Velocity Good Beam Beam 2.
        /// (REAL)
        /// </summary>
        public const string COL_BT_BEAM_GOOD_B2 = "BTBeamGoodBeam2";

        /// <summary>
        /// Bottom Track Table:
        /// Beam Velocity Good Beam 3.
        /// (REAL)
        /// </summary>
        public const string COL_BT_BEAM_GOOD_B3 = "BTBeamGoodBeam3";

        /// <summary>
        /// Bottom Track Table:
        /// Instrument Velocity Beam 0.
        /// (REAL)
        /// </summary>
        public const string COL_BT_INSTR_VEL_B0 = "BTInstrVelBeam0";

        /// <summary>
        /// Bottom Track Table:
        /// Instrument Velocity Beam 1.
        /// (REAL)
        /// </summary>
        public const string COL_BT_INSTR_VEL_B1 = "BTInstrVelBeam1";

        /// <summary>
        /// Bottom Track Table:
        /// Instrument Velocity Beam 2.
        /// (REAL)
        /// </summary>
        public const string COL_BT_INSTR_VEL_B2 = "BTInstrVelBeam2";

        /// <summary>
        /// Bottom Track Table:
        /// Instrument Velocity Beam 3.
        /// (REAL)
        /// </summary>
        public const string COL_BT_INSTR_VEL_B3 = "BTInstrVelBeam3";

        /// <summary>
        /// Bottom Track Table:
        /// Instrument Velocity Good Beam 0.
        /// (REAL)
        /// </summary>
        public const string COL_BT_INSTR_GOOD_B0 = "BTInstrGoodBeam0";

        /// <summary>
        /// Bottom Track Table:
        /// Instrument Velocity Good Beam 1.
        /// (REAL)
        /// </summary>
        public const string COL_BT_INSTR_GOOD_B1 = "BTInstrGoodBeam1";

        /// <summary>
        /// Bottom Track Table:
        /// Instrument Velocity Good Beam 2.
        /// (REAL)
        /// </summary>
        public const string COL_BT_INSTR_GOOD_B2 = "BTInstrGoodBeam2";

        /// <summary>
        /// Bottom Track Table:
        /// Instrument Velocity Good Beam 3.
        /// (REAL)
        /// </summary>
        public const string COL_BT_INSTR_GOOD_B3 = "BTInstrGoodBeam3";

        /// <summary>
        /// Bottom Track Table:
        /// Earth Velocity Beam 0.
        /// (REAL)
        /// </summary>
        public const string COL_BT_EARTH_VEL_B0 = "BTEarthVelBeam0";

        /// <summary>
        /// Bottom Track Table:
        /// Earth Velocity Beam 1.
        /// (REAL)
        /// </summary>
        public const string COL_BT_EARTH_VEL_B1 = "BTEarthVelBeam1";

        /// <summary>
        /// Bottom Track Table:
        /// Earth Velocity Beam 2.
        /// (REAL)
        /// </summary>
        public const string COL_BT_EARTH_VEL_B2 = "BTEarthVelBeam2";

        /// <summary>
        /// Bottom Track Table:
        /// Earth Velocity Beam 3.
        /// (REAL)
        /// </summary>
        public const string COL_BT_EARTH_VEL_B3 = "BTEarthVelBeam3";

        /// <summary>
        /// Bottom Track Table:
        /// Earth Velocity Good Beam 0.
        /// (REAL)
        /// </summary>
        public const string COL_BT_EARTH_GOOD_B0 = "BTEarthGoodBeam0";

        /// <summary>
        /// Bottom Track Table:
        /// Earth Velocity Good Beam 1.
        /// (REAL)
        /// </summary>
        public const string COL_BT_EARTH_GOOD_B1 = "BTEarthGoodBeam1";

        /// <summary>
        /// Bottom Track Table:
        /// Earth Velocity Good Beam 2.
        /// (REAL)
        /// </summary>
        public const string COL_BT_EARTH_GOOD_B2 = "BTEarthGoodBeam2";

        /// <summary>
        /// Bottom Track Table:
        /// Earth Velocity Good Beam 3.
        /// (REAL)
        /// </summary>
        public const string COL_BT_EARTH_GOOD_B3 = "BTEarthGoodBeam3";

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
                log.Error("Error running query on database: " + project.ProjectName, e);
                return 0;
            }
            catch (Exception e)
            {
                log.Error("Unknown Error running query on database: " + project.ProjectName, e);
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
                    log.Error("Error populating datatable from " + project.ProjectName + " database.", e);
                }
                else
                {
                    log.Error("Error populating datatable.", e);
                }
            }
            return dt;
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