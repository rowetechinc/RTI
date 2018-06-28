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
 * Date            Initials    Version     Comments
 * -----------------------------------------------------------------
 * 09/01/2011      RC                      Initial coding
 * 11/28/2011      RC                      Changed NumBins to NumElements.
 * 12/08/2011      RC          1.09        Added EMPTY_VELOCITY.  
 * 12/12/2011      RC          1.09        Removed BYTES_PER_FLOAT AND BYTES_PER_INT.
 *                                          Moved BAD_VELOCITY and EMPTY_VELOCITY to AdcpDataSet.
 * 01/19/2012      RC          1.14        Added NUM_DATASET_HEADER_ELEMENTS. Used variable in GetBaseDataSize().
 *                                          Fix bug in GetDataSetSize(), wrong case statement variable for float.
 *                                          Fix bug in GenerateHeader(), ValueType hard coded to BYTE.
 * 03/30/2012      RC          2.07       Moved Converters.cs methods to MathHelper.cs.
 * 02/20/2013      RC          2.18       Made all public properties' Setters public to convert to and from JSON.
 * 02/22/2013      RC          2.18       Added ToJsonBaseStub() to create a JSON stub for the base dataset properties.
 * 02/25/2013      RC          2.18       Removed Orientation.  Replaced with SubsystemConfiguration.
 * 07/26/2013      RC          2.19.3     Added CEPO index JSON string and renamed SubsystemConfigurationNumber to SubsystemConfigurationIndex.  
 * 01/09/2014      RC          2.21.3     Added SystemSetupDataSet.
 * 02/06/2014      RC          2.21.3     Added Q value to Water Mass DataSet.
 * 07/28/2014      RC          2.23.0     Changed the name of numBins and numBeams to NumElements and ElementMutiplier.
 * 10/31/2014      RC          3.0.2      Added Range Tracking values.
 * 03/09/2015      RC          3.0.3      Added Gage Height.
 * 07/22/2016      RC          3.3.2      Added additional Range Tracking values.
 * 
 */

using System;
using System.Text;
using System.IO;
using Newtonsoft.Json;
namespace RTI
{
    namespace DataSet
    {
        /// <summary>
        /// Base data set.  All data sets will extend from
        /// this data set.  It contains the data set information.
        /// </summary>
        public class BaseDataSet
        {
            #region Variables

            /// <summary>
            /// This is number of elements in the header of a dataset.
            /// Each element is a byte except the NAME.  Its size varies
            /// and is given by NameLength.
            /// </summary>
            public const int NUM_DATASET_HEADER_ELEMENTS = 6;

            #region JSON Strings

            #region Base Data

            /// <summary>
            /// String for NumElements.
            /// </summary>
            public const string JSON_STR_NUMELEMENTS = "NumElements";

            /// <summary>
            /// String for ElementsMultiplier.
            /// </summary>
            public const string JSON_STR_ELEMENTSMULTIPLIER = "ElementsMultiplier";

            #endregion

            #region Data Sets with per Beam Data

            /// <summary>
            /// String for AmplitudeData.
            /// </summary>
            public const string JSON_STR_AMPLITUDEDATA = "AmplitudeData";

            /// <summary>
            /// String for Correlation Data.
            /// </summary>
            public const string JSON_STR_CORRELATIONDATA = "CorrelationData";

            /// <summary>
            /// String for Beam Velocity Data.
            /// </summary>
            public const string JSON_STR_BEAMVELOCITYDATA = "BeamVelocityData";

            /// <summary>
            /// String for Instrument Velocity Data.
            /// </summary>
            public const string JSON_STR_INSTRUMENTVELOCITYDATA = "InstrumentVelocityData";

            /// <summary>
            /// String for Earth Velocity Data.
            /// </summary>
            public const string JSON_STR_EARTHVELOCITYDATA = "EarthVelocityData";

            /// <summary>
            /// String for Good Earth Data.
            /// </summary>
            public const string JSON_STR_GOODEARTHDATA = "GoodEarthData";

            /// <summary>
            /// String for Good Beam Data.
            /// </summary>
            public const string JSON_STR_GOODBEAMDATA = "GoodBeamData";

            #endregion

            #region VeclocityVector Properties

            /// <summary>
            /// String for IsVelocityVectorAvail property.
            /// </summary>
            public const string JSON_STR_ISVELOCITYVECTORAVAIL = "IsVelocityVectorAvail";

            /// <summary>
            /// String for VelocityVectors property.
            /// </summary>
            public const string JSON_STR_VELOCITYVECTORS = "VelocityVectors";

            /// <summary>
            /// String for VelocityVector Magnitude property.
            /// </summary>
            public const string JSON_STR_VV_MAG = "Magnitude";

            /// <summary>
            /// String for VelocityVector DirectionXNorth property.
            /// </summary>
            public const string JSON_STR_VV_XNORTH = "DirectionXNorth";

            /// <summary>
            /// String for VelocityVector DirectionYNorth property.
            /// </summary>
            public const string JSON_STR_VV_YNORTH = "DirectionYNorth";

            #endregion

            #region Ensemble Properties

            /// <summary>
            /// String for EnsembleNumber.
            /// </summary>
            public const string JSON_STR_ENSEMBLENUMBER = "EnsembleNumber";

            #region SerialNumber Properties

            ///// <summary>
            ///// String for SerialNumber SystemSerialNumber.
            ///// </summary>
            //public const string STR_JSON_SERIALNUMBER_SYSTEMSERIALNUMBER = "SystemSerialNumber";

            ///// <summary>
            ///// String for SerialNumber Spare.
            ///// </summary>
            //public const string STR_JSON_SERIALNUMBER_SPARE = "Spare";

            ///// <summary>
            ///// String for SerialNumber BaseHardware.
            ///// </summary>
            //public const string STR_JSON_SERIALNUMBER_BASEHARDWARE = "BaseHardware";

            ///// <summary>
            ///// String for SerialNumber SubSystems.
            ///// </summary>
            //public const string STR_JSON_SERIALNUMBER_SUBSYSTEMS = "SubSystems";

            ///// <summary>
            ///// String for SerialNumber SubSystemsDict.
            ///// </summary>
            //public const string STR_JSON_SERIALNUMBER_SUBSYSTEMDICT = "SubSystemsDict";

            /// <summary>
            /// String for SerialNumber SerialNumberString.
            /// </summary>
            public const string STR_JSON_SERIALNUMBER_SERIALNUMBERSTRING = "SerialNumberString";

            ///// <summary>
            ///// String for SerialNumber Subsystem Dictionary Index.
            ///// </summary>
            //public const string STR_JSON_SERIALNUMBER_DICT_INDEX = "Index";

            ///// <summary>
            ///// String for SerialNumber Subsystem Dictionary Code.
            ///// </summary>
            //public const string STR_JSON_SERIALNUMBER_DICT_CODE = "Code";

            #endregion

            #region Firmware Properties

            /// <summary>
            /// String for FirmwareMajor.
            /// </summary>
            public const string JSON_STR_FIRMWAREMAJOR = "FirmwareMajor";

            /// <summary>
            /// String for FirmwareMinor.
            /// </summary>
            public const string JSON_STR_FIRMWAREMINOR = "FirmwareMinor";

            /// <summary>
            /// String for FirmwareRevision.
            /// </summary>
            public const string JSON_STR_FIRMWAREREVISION = "FirmwareRevision";

            /// <summary>
            /// String for SubsystemCode.
            /// </summary>
            public const string JSON_STR_SUBSYSTEMCODE = "SubsystemCode";

            #endregion

            #region SubsystemConfig Properties

            /// <summary>
            /// String for SubSystem.
            /// </summary>
            public const string JSON_STR_SUBSYSTEM = "SubSystem";

            /// <summary>
            /// String for Subsystem Index.
            /// </summary>
            public const string JSON_STR_SUBSYSTEM_INDEX = "Index";

            /// <summary>
            /// String for Subsystem Code.
            /// </summary>
            public const string JSON_STR_SUBSYSTEM_CODE = "Code";

            /// <summary>
            /// String for CEPO Index.
            /// </summary>
            public const string JSON_STR_CEPOINDEX = "CepoIndex";

            /// <summary>
            /// String for ConfigNumber.
            /// </summary>
            public const string JSON_STR_SSCONFIG_INDEX = "ConfigNumber";

            #endregion

            /// <summary>
            /// String for NumBins.
            /// </summary>
            public const string JSON_STR_NUMBINS = "NumBins";

            /// <summary>
            /// String for NumBeams.
            /// </summary>
            public const string JSON_STR_NUMBEAMS = "NumBeams";

            /// <summary>
            /// String for DesiredPingCount.
            /// </summary>
            public const string JSON_STR_DESIREDPINGCOUNT = "DesiredPingCount";

            /// <summary>
            /// String for ActualPingCount.
            /// </summary>
            public const string JSON_STR_ACTUALPINGCOUNT = "ActualPingCount";

            /// <summary>
            /// String for SysSerialNumber.
            /// </summary>
            public const string JSON_STR_SYSSERIALNUMBER = "SysSerialNumber";

            /// <summary>
            /// String for SysFirmware.
            /// </summary>
            public const string JSON_STR_SYSFIRMWARE = "SysFirmware";

            /// <summary>
            /// String for SubsystemConfig.
            /// </summary>
            public const string JSON_STR_SUBSYSTEMCONFIG = "SubsystemConfig";

            /// <summary>
            /// String for Status.
            /// </summary>
            public const string JSON_STR_STATUS = "Status";

            /// <summary>
            /// String for Year.
            /// </summary>
            public const string JSON_STR_YEAR = "Year";

            /// <summary>
            /// String for Month.
            /// </summary>
            public const string JSON_STR_MONTH = "Month";

            /// <summary>
            /// String for Day.
            /// </summary>
            public const string JSON_STR_DAY = "Day";

            /// <summary>
            /// String for Hour.
            /// </summary>
            public const string JSON_STR_HOUR = "Hour";

            /// <summary>
            /// String for Minute.
            /// </summary>
            public const string JSON_STR_MINUTE = "Minute";

            /// <summary>
            /// String for Second.
            /// </summary>
            public const string JSON_STR_SECOND = "Second";

            /// <summary>
            /// String for HSec.
            /// </summary>
            public const string JSON_STR_HSEC = "HSec";

            /// <summary>
            /// String for EnsDateTime.
            /// </summary>
            public const string JSON_STR_ENSDATETIME = "EnsDateTime";

            #endregion

            #region Ancillary Properties

            /// <summary>
            /// String for FirstBinRange.
            /// </summary>
            public const string JSON_STR_FIRSTBINRANGE = "FirstBinRange";

            /// <summary>
            /// String for BinSize.
            /// </summary>
            public const string JSON_STR_BINSIZE = "BinSize";

            /// <summary>
            /// String for FirstPingTime.
            /// </summary>
            public const string JSON_STR_FIRSTPINGTIME = "FirstPingTime";

            /// <summary>
            /// String for LastPingTime.
            /// </summary>
            public const string JSON_STR_LASTPINGTIME = "LastPingTime";

            /// <summary>
            /// String for Heading.
            /// </summary>
            public const string JSON_STR_HEADING = "Heading";

            /// <summary>
            /// String for Pitch.
            /// </summary>
            public const string JSON_STR_PITCH = "Pitch";

            /// <summary>
            /// String for Roll.
            /// </summary>
            public const string JSON_STR_ROLL = "Roll";

            /// <summary>
            /// String for WaterTemp.
            /// </summary>
            public const string JSON_STR_WATERTEMP = "WaterTemp";

            /// <summary>
            /// String for SystemTemp.
            /// </summary>
            public const string JSON_STR_SYSTEMP = "SystemTemp";

            /// <summary>
            /// String for Salinity.
            /// </summary>
            public const string JSON_STR_SALINITY = "Salinity";

            /// <summary>
            /// String for Pressure.
            /// </summary>
            public const string JSON_STR_PRESSURE = "Pressure";

            /// <summary>
            /// String for TransducerDepth.
            /// </summary>
            public const string JSON_STR_TRANSDUCERDEPTH = "TransducerDepth";

            /// <summary>
            /// String for SpeedOfSound.
            /// </summary>
            public const string JSON_STR_SPEEDOFSOUND = "SpeedOfSound";

            #endregion

            #region Bottom Track Properties

            /// <summary>
            /// String for FirstPingTime.
            /// </summary>
            public const string JSON_STR_BT_FIRSTPINGTIME = "FirstPingTime";

            /// <summary>
            /// String for LastPingTime.
            /// </summary>
            public const string JSON_STR_BT_LASTPINGTIME = "LastPingTime";

            /// <summary>
            /// String for Heading.
            /// </summary>
            public const string JSON_STR_BT_HEADING = "Heading";

            /// <summary>
            /// String for Pitch.
            /// </summary>
            public const string JSON_STR_BT_PITCH = "Pitch";

            /// <summary>
            /// String for Roll.
            /// </summary>
            public const string JSON_STR_BT_ROLL = "Roll";

            /// <summary>
            /// String for WaterTemp.
            /// </summary>
            public const string JSON_STR_BT_WATERTEMP = "WaterTemp";

            /// <summary>
            /// String for SystemTemp.
            /// </summary>
            public const string JSON_STR_BT_SYSTEMTEMP = "SystemTemp";

            /// <summary>
            /// String for Salinity.
            /// </summary>
            public const string JSON_STR_BT_SALINITY = "Salinity";

            /// <summary>
            /// String for Pressure.
            /// </summary>
            public const string JSON_STR_BT_PRESSURE = "Pressure";

            /// <summary>
            /// String for TransducerDepth.
            /// </summary>
            public const string JSON_STR_BT_TRANSDUCERDEPTH = "TransducerDepth";

            /// <summary>
            /// String for SpeedOfSound.
            /// </summary>
            public const string JSON_STR_BT_SPEEDOFSOUND = "SpeedOfSound";

            /// <summary>
            /// String for Status.
            /// </summary>
            public const string JSON_STR_BT_STATUS = "Status";

            /// <summary>
            /// String for NumBeams.
            /// </summary>
            public const string JSON_STR_BT_NUMBEAMS = "NumBeams";

            /// <summary>
            /// String for ActualPingCount.
            /// </summary>
            public const string JSON_STR_BT_ACTUALPINGCOUNT = "ActualPingCount";

            /// <summary>
            /// String for Range.
            /// </summary>
            public const string JSON_STR_BT_RANGE = "Range";

            /// <summary>
            /// String for SNR.
            /// </summary>
            public const string JSON_STR_BT_SNR = "SNR";

            /// <summary>
            /// String for Amplitude.
            /// </summary>
            public const string JSON_STR_BT_AMPLITUDE = "Amplitude";

            /// <summary>
            /// String for Correlation.
            /// </summary>
            public const string JSON_STR_BT_CORRELATION = "Correlation";

            /// <summary>
            /// String for BeamVelocity.
            /// </summary>
            public const string JSON_STR_BT_BEAMVELOCITY = "BeamVelocity";

            /// <summary>
            /// String for BeamGood.
            /// </summary>
            public const string JSON_STR_BT_BEAMGOOD = "BeamGood";

            /// <summary>
            /// String for InstrumentVelocity.
            /// </summary>
            public const string JSON_STR_BT_INSTRUMENTVELOCITY = "InstrumentVelocity";

            /// <summary>
            /// String for InstrumentGood.
            /// </summary>
            public const string JSON_STR_BT_INSTRUMENTGOOD = "InstrumentGood";

            /// <summary>
            /// String for EarthVelocity.
            /// </summary>
            public const string JSON_STR_BT_EARTHVELOCITY = "EarthVelocity";

            /// <summary>
            /// String for EarthGood.
            /// </summary>
            public const string JSON_STR_BT_EARTHGOOD = "EarthGood";

            #endregion

            #region NMEA Properties

            /// <summary>
            /// String for NmeaStrings.
            /// </summary>
            public const string JSON_STR_NMEASTRINGS = "NmeaStrings";

            #endregion

            #region Water Mass Properties

            /// <summary>
            /// String for Water Mass Velocity East property.
            /// </summary>
            public const string JSON_STR_VELEAST = "VelocityEast";

            /// <summary>
            /// String for Water Mass Velocity North property.
            /// </summary>
            public const string JSON_STR_VELNORTH = "VelocityNorth";

            /// <summary>
            /// String for Water Mass Velocity Vertical property.
            /// </summary>
            public const string JSON_STR_VELVERTICAL = "VelocityVertical";

            /// <summary>
            /// String for Water Mass Velocity X property.
            /// </summary>
            public const string JSON_STR_VELX = "VelocityX";

            /// <summary>
            /// String for Water Mass Instrument Velocity Y property.
            /// </summary>
            public const string JSON_STR_VELY = "VelocityY";

            /// <summary>
            /// String for Water Mass Instrument Velocity Z property.
            /// </summary>
            public const string JSON_STR_VELZ = "VelocityZ";

            /// <summary>
            /// String for Water Mass Instrument Velocity Q property.
            /// </summary>
            public const string JSON_STR_VELQ = "VelocityQ";

            /// <summary>
            /// String for Water Mass Ship Velocity Transverse property.
            /// </summary>
            public const string JSON_STR_VEL_TRANS = "VelocityTransverse";

            /// <summary>
            /// String for Water Mass Ship Velocity Longitudinal property.
            /// </summary>
            public const string JSON_STR_VEL_LONG = "VelocityLongitudinal";

            /// <summary>
            /// String for Water Mass Ship Velocity Normal property.
            /// </summary>
            public const string JSON_STR_VEL_NORM = "VelocityNormal";

            /// <summary>
            /// String for Water Mass Depth Layer property.
            /// </summary>
            public const string JSON_STR_WATERMASSDEPTHLAYER = "WaterMassDepthLayer";

            #endregion

            #region Profile Engineering

            /// <summary>
            /// String for PrePingVel.
            /// </summary>
            public const string JSON_STR_PE_PREPINGVEL = "PrePingVel";

            /// <summary>
            /// String for PrePingCor.
            /// </summary>
            public const string JSON_STR_PE_PREPINGCOR = "PrePingCor";

            /// <summary>
            /// String for PrePingAmp.
            /// </summary>
            public const string JSON_STR_PE_PREPINGAMP = "PrePingAmp";

            /// <summary>
            /// String for SamplesPerSecond.
            /// </summary>
            public const string JSON_STR_PE_SAMPLESPERSECOND = "SamplesPerSecond";

            /// <summary>
            /// String for SystemFreqHz.
            /// </summary>
            public const string JSON_STR_PE_SYSTEMFREQHZ = "SystemFreqHz";

            /// <summary>
            /// String for LagSamples.
            /// </summary>
            public const string JSON_STR_PE_LAGSAMPLES = "LagSamples";

            /// <summary>
            /// String for CPCE.
            /// </summary>
            public const string JSON_STR_PE_CPCE = "CPCE";

            /// <summary>
            /// String for NCE.
            /// </summary>
            public const string JSON_STR_PE_NCE = "NCE";

            /// <summary>
            /// String for RepeatN.
            /// </summary>
            public const string JSON_STR_PE_REPEATN = "RepeatN";

            /// <summary>
            /// String for PrePingGap.
            /// </summary>
            public const string JSON_STR_PE_PREPINGGAP = "PrePingGap";

            /// <summary>
            /// String for PrePingNCE.
            /// </summary>
            public const string JSON_STR_PE_PREPINGNCE = "PrePingNCE";

            /// <summary>
            /// String for PrePingRepeatN.
            /// </summary>
            public const string JSON_STR_PE_PREPINGREPEATN = "PrePingRepeatN";

            /// <summary>
            /// String for PrePingLagSamples.
            /// </summary>
            public const string JSON_STR_PE_PREPINGLAGSAMPLES = "PrePingLagSamples";

            /// <summary>
            /// String for TRHighGain.
            /// </summary>
            public const string JSON_STR_PE_PREPINGTRHIGHGAIN = "TRHighGain";

            #endregion

            #region Bottom Track Engineering

            /// <summary>
            /// String for SamplesPerSecond.
            /// </summary>
            public const string JSON_STR_BTE_SAMPLESPERSECOND = "SamplesPerSecond";

            /// <summary>
            /// String for SystemFreqHz.
            /// </summary>
            public const string JSON_STR_BTE_SYSTEMFREQHZ = "SystemFreqHz";

            /// <summary>
            /// String for LagSamples.
            /// </summary>
            public const string JSON_STR_BTE_LAGSAMPLES = "LagSamples";

            /// <summary>
            /// String for CPCE.
            /// </summary>
            public const string JSON_STR_BTE_CPCE = "CPCE";

            /// <summary>
            /// String for NCE.
            /// </summary>
            public const string JSON_STR_BTE_NCE = "NCE";

            /// <summary>
            /// String for RepeatN.
            /// </summary>
            public const string JSON_STR_BTE_REPEATN = "RepeatN";

            /// <summary>
            /// String for AmbHz.
            /// </summary>
            public const string JSON_STR_BTE_AMBHZ = "AmbHz";

            /// <summary>
            /// String for AmbVel.
            /// </summary>
            public const string JSON_STR_BTE_AMBVEL = "AmbVel";

            /// <summary>
            /// String for AmbAmp.
            /// </summary>
            public const string JSON_STR_BTE_AMBAMP = "AmbAmp";

            /// <summary>
            /// String for AmbCor.
            /// </summary>
            public const string JSON_STR_BTE_AMBCOR = "AmbCor";

            /// <summary>
            /// String for AmbSNR.
            /// </summary>
            public const string JSON_STR_BTE_AMBSNR = "AmbSNR";

            /// <summary>
            /// String for LagUsed.
            /// </summary>
            public const string JSON_STR_BTE_LAGUSED = "LagUsed";

            #endregion

            #region System Setup

            /// <summary>
            /// String for BT SamplesPerSecond.
            /// </summary>
            public const string JSON_STR_SS_BT_SAMPLESPERSECOND = "BtSamplesPerSecond";

            /// <summary>
            /// String for BT SystemFreqHz.
            /// </summary>
            public const string JSON_STR_SS_BT_SYSTEMFREQHZ = "BtSystemFreqHz";

            /// <summary>
            /// String for BT CPCE.
            /// </summary>
            public const string JSON_STR_SS_BT_CPCE = "BtCPCE";

            /// <summary>
            /// String for BT NCE.
            /// </summary>
            public const string JSON_STR_SS_BT_NCE = "BtNCE";

            /// <summary>
            /// String for BT RepeatN.
            /// </summary>
            public const string JSON_STR_SS_BT_REPEATN = "BtRepeatN";

            /// <summary>
            /// String for WP SamplesPerSecond.
            /// </summary>
            public const string JSON_STR_SS_WP_SAMPLESPERSECOND = "WpSamplesPerSecond";

            /// <summary>
            /// String for WP SystemFreqHz.
            /// </summary>
            public const string JSON_STR_SS_WP_SYSTEMFREQHZ = "WpSystemFreqHz";

            /// <summary>
            /// String for WP CPCE.
            /// </summary>
            public const string JSON_STR_SS_WP_CPCE = "WpCPCE";

            /// <summary>
            /// String for WP NCE.
            /// </summary>
            public const string JSON_STR_SS_WP_NCE = "WpNCE";

            /// <summary>
            /// String for WP RepeatN.
            /// </summary>
            public const string JSON_STR_SS_WP_REPEATN = "WpRepeatN";

            /// <summary>
            /// String for WP LagSamples.
            /// </summary>
            public const string JSON_STR_SS_WP_LAGSAMPLES = "WpLagSamples";

            /// <summary>
            /// String for Voltage.
            /// </summary>
            public const string JSON_STR_SS_VOLTAGE = "Voltage";

            /// <summary>
            /// String for Voltage.
            /// </summary>
            public const string JSON_STR_SS_XMTVOLTAGE = "XmtVoltage";

            /// <summary>
            /// String for Voltage.
            /// </summary>
            public const string JSON_STR_SS_BTBROADBAND = "BtBroadband";

            /// <summary>
            /// String for Voltage.
            /// </summary>
            public const string JSON_STR_SS_BTLAGLENGTH = "BtLagLength";

            /// <summary>
            /// String for Voltage.
            /// </summary>
            public const string JSON_STR_SS_BTNARROWBAND = "BtNarrowband";

            /// <summary>
            /// String for Voltage.
            /// </summary>
            public const string JSON_STR_SS_BTBEAMMUX = "BtBeamMux";

            /// <summary>
            /// String for Voltage.
            /// </summary>
            public const string JSON_STR_SS_WPBROADBAND = "WpBroadband";

            /// <summary>
            /// String for Voltage.
            /// </summary>
            public const string JSON_STR_SS_WPLAGLENGTH = "WpLagLength";

            /// <summary>
            /// String for Voltage.
            /// </summary>
            public const string JSON_STR_SS_WPBANDWIDTH = "WpBandWidth";

            /// <summary>
            /// String for Voltage.
            /// </summary>
            public const string JSON_STR_SS_WPBANDWIDTH1 = "WpBandWidth1";

            #endregion

            #region Gage Height

            /// <summary>
            /// String for Status.
            /// </summary>
            public const string JSON_STR_GH_STATUS = "Status";

            /// <summary>
            /// String for AvgRange.
            /// </summary>
            public const string JSON_STR_GH_AVGRANGE = "AvgRange";

            /// <summary>
            /// String for Standard Deviation.
            /// </summary>
            public const string JSON_STR_GH_SD = "SD";

            /// <summary>
            /// String for AvgSN.
            /// </summary>
            public const string JSON_STR_GH_AVGSN = "AvgSN";

            /// <summary>
            /// String for N.
            /// </summary>
            public const string JSON_STR_GH_N = "N";

            /// <summary>
            /// String for Salinity.
            /// </summary>
            public const string JSON_STR_GH_SALINITY = "Salinity";

            /// <summary>
            /// String for Pressure.
            /// </summary>
            public const string JSON_STR_GH_PRESSURE = "Pressure";

            /// <summary>
            /// String for Depth.
            /// </summary>
            public const string JSON_STR_GH_DEPTH = "Depth";

            /// <summary>
            /// String for Water Temperatrue.
            /// </summary>
            public const string JSON_STR_GH_WATERTEMP = "WaterTemp";

            /// <summary>
            /// String for System Temperature.
            /// </summary>
            public const string JSON_STR_GH_SYSTEMTEMP = "SystemTemp";

            /// <summary>
            /// String for Speed of Sound.
            /// </summary>
            public const string JSON_STR_GH_SOS = "SoS";

            /// <summary>
            /// String for Heading.
            /// </summary>
            public const string JSON_STR_GH_HEADING = "Heading";

            /// <summary>
            /// String for Pitch.
            /// </summary>
            public const string JSON_STR_GH_PITCH = "Pitch";

            /// <summary>
            /// String for Roll.
            /// </summary>
            public const string JSON_STR_GH_ROLL = "Roll";

            #endregion

            #region DVL

            /// <summary>
            /// String for DVL.
            /// </summary>
            public const string JSON_STR_DVLDATA = "DvlData";

            /// <summary>
            /// String for DVL Sample Number.
            /// </summary>
            public const string JSON_STR_DVL_SAMPLENUMBER = "SampleNumber";

            /// <summary>
            /// String for DVL Pitch.
            /// </summary>
            public const string JSON_STR_DVL_PITCH = "Pitch";

            /// <summary>
            /// String for DVL Roll.
            /// </summary>
            public const string JSON_STR_DVL_ROLL = "Roll";

            /// <summary>
            /// String for DVL Heading.
            /// </summary>
            public const string JSON_STR_DVL_HEADING = "Heading";

            /// <summary>
            /// String for DVL temperature.
            /// </summary>
            public const string JSON_STR_DVL_TEMPERATURE = "Temperature";

            /// <summary>
            /// String for DVL BIT.
            /// </summary>
            public const string JSON_STR_DVL_BIT = "BIT";

            /// <summary>
            /// Reference layer minimum.
            /// </summary>
            public const string JSON_STR_DVL_REF_LAYER_MIN = "RefLayerMin";

            /// <summary>
            /// Reference layer maximum.
            /// </summary>
            public const string JSON_STR_DVL_REF_LAYER_MAX = "RefLayerMax";

            #region Water Mass Instrument Velocity

            /// <summary>
            /// Water Mass X Velocity.
            /// </summary>
            public const string JSON_STR_DVL_WM_X_VEL = "WmXVelocity";

            /// <summary>
            /// Water Mass Y Velocity.
            /// </summary>
            public const string JSON_STR_DVL_WM_Y_VEL = "WmYVelocity";

            /// <summary>
            /// Water Mass Z Velocity.
            /// </summary>
            public const string JSON_STR_DVL_WM_Z_VEL = "WmZVelocity";

            /// <summary>
            /// Water Mass Error Velocity.
            /// </summary>
            public const string JSON_STR_DVL_WM_ERROR_VEL = "WmErrorVelocity";

            /// <summary>
            /// Water Mass Instrument Is Good Velocity.
            /// </summary>
            public const string JSON_STR_DVL_WM_INSTRUMENT_IS_GOOD_VEL = "WmInstrumentIsGoodVelocity";

            #endregion

            #region Water Mass Ship Instrument Velocity

            /// <summary>
            /// Water Mass Transverse Velocity.
            /// </summary>
            public const string JSON_STR_DVL_WM_TRANSVERSE_VEL = "WmTransverseVelocity";

            /// <summary>
            /// Water Mass Longitudinal Velocity.
            /// </summary>
            public const string JSON_STR_DVL_WM_LONGITUDINAL_VEL = "WmLongitudinalVelocity";

            /// <summary>
            /// Water Mass Normal Velocity.
            /// </summary>
            public const string JSON_STR_DVL_WM_NORMAL_VEL = "WmNormalVelocity";

            /// <summary>
            /// Water Mass Ship Is Good Velocity.
            /// </summary>
            public const string JSON_STR_DVL_WM_SHIP_IS_GOOD_VEL = "WmShipIsGoodVelocity";

            #endregion

            #region Water Mass Earth Velocity

            /// <summary>
            /// Water Mass East Velocity.
            /// </summary>
            public const string JSON_STR_DVL_WM_EAST_VEL = "WmEastVelocity";

            /// <summary>
            /// Water Mass North Velocity.
            /// </summary>
            public const string JSON_STR_DVL_WM_NORTH_VEL = "WmNorthVelocity";

            /// <summary>
            /// Water Mass Upward Velocity.
            /// </summary>
            public const string JSON_STR_DVL_WM_UPWARD_VEL = "WmUpwardVelocity";

            /// <summary>
            /// Water Mass Earth Is Good Velocity.
            /// </summary>
            public const string JSON_STR_DVL_WM_EARTH_IS_GOOD_VEL = "WmEarthIsGoodVelocity";

            #endregion

            #region Water Mass Earth Distance

            /// <summary>
            /// Water Mass Earth East Distance.
            /// </summary>
            public const string JSON_STR_DVL_WM_EAST_DMG = "WmEastDistance";

            /// <summary>
            /// Water Mass Earth North Distance.
            /// </summary>
            public const string JSON_STR_DVL_WM_NORTH_DMG = "WmNorthDistance";

            /// <summary>
            /// Water Mass Earth East Distance.
            /// </summary>
            public const string JSON_STR_DVL_WM_UPWARD_DMG = "WmUpwardDistance";

            /// <summary>
            /// Water Mass Earth Range to Water Mass Center.
            /// </summary>
            public const string JSON_STR_DVL_WM_EARTH_RANGE_TO_WM_CENTER = "WmEarthRangeToWaterMassCenter";

            /// <summary>
            /// Water Mass Earth Time of last good velocity.
            /// </summary>
            public const string JSON_STR_DVL_WM_EARTH_TIME_LAST_GOOD_VEL = "WmEarthTimeLastGoodVel";

            #endregion

            #region Bottom Track Instrument Velocity

            /// <summary>
            /// Bottom Track X Velocity.
            /// </summary>
            public const string JSON_STR_DVL_BT_X_VEL = "BtXVelocity";

            /// <summary>
            /// Bottom Track Y Velocity.
            /// </summary>
            public const string JSON_STR_DVL_BT_Y_VEL = "BtYVelocity";

            /// <summary>
            /// Bottom Track Z Velocity.
            /// </summary>
            public const string JSON_STR_DVL_BT_Z_VEL = "BtZVelocity";

            /// <summary>
            /// Bottom Track Error Velocity.
            /// </summary>
            public const string JSON_STR_DVL_BT_ERROR_VEL = "BtErrorVelocity";

            /// <summary>
            /// Bottom Track Instrument Is Good Velocity.
            /// </summary>
            public const string JSON_STR_DVL_BT_INSTRUMENT_IS_GOOD_VEL = "BtInstrumentIsGoodVelocity";

            #endregion

            #region Bottom Track Ship Instrument Velocity

            /// <summary>
            /// Bottom Track Transverse Velocity.
            /// </summary>
            public const string JSON_STR_DVL_BT_TRANSVERSE_VEL = "BtTransverseVelocity";

            /// <summary>
            /// Bottom Track Longitudinal Velocity.
            /// </summary>
            public const string JSON_STR_DVL_BT_LONGITUDINAL_VEL = "BtLongitudinalVelocity";

            /// <summary>
            /// Bottom Track Normal Velocity.
            /// </summary>
            public const string JSON_STR_DVL_BT_NORMAL_VEL = "BtNormalVelocity";

            /// <summary>
            /// Bottom Track Ship Error Velocity.
            /// </summary>
            public const string JSON_STR_DVL_BT_SHIP_ERR_VEL = "BtShipErrorVelocity";

            /// <summary>
            /// Bottom Track Ship Is Good Velocity.
            /// </summary>
            public const string JSON_STR_DVL_BT_SHIP_IS_GOOD_VEL = "BtShipIsGoodVelocity";

            #endregion

            #region Bottom Track Earth Velocity

            /// <summary>
            /// Bottom Track East Velocity.
            /// </summary>
            public const string JSON_STR_DVL_BT_EAST_VEL = "BtEastVelocity";

            /// <summary>
            /// Bottom Track North Velocity.
            /// </summary>
            public const string JSON_STR_DVL_BT_NORTH_VEL = "BtNorthVelocity";

            /// <summary>
            /// Bottom Track Upward Velocity.
            /// </summary>
            public const string JSON_STR_DVL_BT_UPWARD_VEL = "BtUpwardVelocity";

            /// <summary>
            /// Bottom Track Earth Is Good Velocity.
            /// </summary>
            public const string JSON_STR_DVL_BT_EARTH_IS_GOOD_VEL = "BtEarthIsGoodVelocity";

            #endregion

            #region Bottom Track Earth Distance

            /// <summary>
            /// Bottom Track Earth East Distance.
            /// </summary>
            public const string JSON_STR_DVL_BT_EAST_DMG = "BtEastDistance";

            /// <summary>
            /// Bottom Track Earth North Distance.
            /// </summary>
            public const string JSON_STR_DVL_BT_NORTH_DMG = "BtNorthDistance";

            /// <summary>
            /// Bottom Track Earth East Distance.
            /// </summary>
            public const string JSON_STR_DVL_BT_UPWARD_DMG = "BtUpwardDistance";

            /// <summary>
            /// Bottom Track Earth Range to Water Mass Center.
            /// </summary>
            public const string JSON_STR_DVL_BT_EARTH_RANGE_TO_WM_CENTER = "BtEarthRangeToWaterMassCenter";

            /// <summary>
            /// Bottom Track Earth Time of last good velocity.
            /// </summary>
            public const string JSON_STR_DVL_BT_EARTH_TIME_LAST_GOOD_VEL = "BtEarthTimeLastGoodVel";

            #endregion

            #region Pressure and Range

            /// <summary>
            /// Bottom Track Pressure 
            /// </summary>
            public const string JSON_STR_DVL_PRESSURE = "Pressure";

            /// <summary>
            /// Bottom Track Range Beam 0.
            /// </summary>
            public const string JSON_STR_DVL_RANGE_B0 = "RangeBeam0";

            /// <summary>
            /// Bottom Track Range Beam 1.
            /// </summary>
            public const string JSON_STR_DVL_RANGE_B1 = "RangeBeam1";

            /// <summary>
            /// Bottom Track Range Beam 2.
            /// </summary>
            public const string JSON_STR_DVL_RANGE_B2 = "RangeBeam2";

            /// <summary>
            /// Bottom Track Range Beam 3.
            /// </summary>
            public const string JSON_STR_DVL_RANGE_B3 = "RangeBeam3";

            /// <summary>
            /// Bottom Track Range Beam 3.
            /// </summary>
            public const string JSON_STR_DVL_AVG_RANGE = "AverageRange";

            #endregion

            #region Distance Made Good

            /// <summary>
            /// Distance Made Good East.
            /// </summary>
            public const string JSON_STR_DVL_DMG_EAST = "DmgEast";

            /// <summary>
            /// Distance Made Good North.
            /// </summary>
            public const string JSON_STR_DVL_DMG_NORTH = "DmgNorth";

            /// <summary>
            /// Distance Made Good Upward.
            /// </summary>
            public const string JSON_STR_DVL_DMG_UPWARD = "DmgUpward";

            /// <summary>
            /// Distance Made Good Error.
            /// </summary>
            public const string JSON_STR_DVL_DMG_ERROR = "DmgError";

            #endregion

            #region Distance Made Good Reference Layer

            /// <summary>
            /// Distance Made Good East.
            /// </summary>
            public const string JSON_STR_DVL_DMG_REF_EAST = "DmgRefEast";

            /// <summary>
            /// Distance Made Good North.
            /// </summary>
            public const string JSON_STR_DVL_DMG_REF_NORTH = "DmgRefNorth";

            /// <summary>
            /// Distance Made Good Upward.
            /// </summary>
            public const string JSON_STR_DVL_DMG_REF_UPWARD = "DmgRefUpward";

            /// <summary>
            /// Distance Made Good Error.
            /// </summary>
            public const string JSON_STR_DVL_DMG_REF_ERROR = "DmgRefError";

            #endregion

            #region Leak Detection

            /// <summary>
            /// Leak Detection.
            /// </summary>
            public const string JSON_STR_DVL_LEAKDETECTION = "LeakDetection";

            #endregion

            #endregion

            #region Range Tracking

            /// <summary>
            /// String for Range Tracking SNR.
            /// </summary>
            public const string JSON_STR_RT_SNR = "SNR";

            /// <summary>
            /// String for Range Tracking Range.
            /// </summary>
            public const string JSON_STR_RT_RANGE = "Range";

            /// <summary>
            /// String for Range Tracking Pings.
            /// </summary>
            public const string JSON_STR_RT_PINGS = "Pings";

            /// <summary>
            /// String for Range Tracking Pings.
            /// </summary>
            public const string JSON_STR_RT_AMPLITUDE = "Amplitude";

            /// <summary>
            /// String for Range Tracking Pings.
            /// </summary>
            public const string JSON_STR_RT_CORRELATION = "Correlation";

            /// <summary>
            /// String for Range Tracking Pings.
            /// </summary>
            public const string JSON_STR_RT_BEAMVEL = "BeamVelocity";

            /// <summary>
            /// String for Range Tracking Pings.
            /// </summary>
            public const string JSON_STR_RT_INSTRVEL = "InstrumentVelocity";

            /// <summary>
            /// String for Range Tracking Pings.
            /// </summary>
            public const string JSON_STR_RT_EARTHVEL = "EarthVelocity";

            #endregion

            #region System Setup

            /// <summary>
            /// String for BT SamplesPerSecond.
            /// </summary>
            public const string JSON_STR_ADCP2INFO_VIPWR = "ViPwr";

            /// <summary>
            /// String for BT SystemFreqHz.
            /// </summary>
            public const string JSON_STR_ADCP2INFO_VINF= "ViNf";

            /// <summary>
            /// String for BT CPCE.
            /// </summary>
            public const string JSON_STR_ADCP2INFO_VINFL = "ViNfl";

            /// <summary>
            /// String for BT NCE.
            /// </summary>
            public const string JSON_STR_ADCP2INFO_VISLEEP = "ViSleep";

            #endregion

            #endregion

            #endregion

            #region Properties

            /// <summary>
            /// Type of ranges for this data type.
            /// </summary>
            public int ValueType { get; set; }

            /// <summary>
            /// Number of DATA in this data type.
            /// 
            /// DO NOT USE THIS TO GET THE NUMBER OF
            /// BINS IN THE ENSEMBLE.  USE
            /// EnsembleData.NumBins.  THIS NUMBER IS THE
            /// NUMBER OF DATA VALUES IN THE DATATYPE.
            /// </summary>
            public int NumElements { get; set; }

            /// <summary>
            /// Multiply this value to the NumElements to 
            /// determine how many items are in the dataset.
            /// 
            /// DO NOT USE THIS TO GET THE NUMBER OF 
            /// BEAMS IN THE ENSEMBLE.  USE
            /// EnsembleData.NumBeams.  THIS NUMBER IS 
            /// THE NUMBER OF DATAS PER ELEMENT.
            /// Number of beams in this data type.
            /// </summary>
            public int ElementsMultiplier { get; set; }

            /// <summary>
            /// Dataset Image value.
            /// </summary>
            public int Imag { get; set; }

            /// <summary>
            /// Length of name for this data type.
            /// </summary>
            public int NameLength { get; set; }

            /// <summary>
            /// Name of this data type.
            /// </summary>
            public string Name { get; set; }

            #endregion

            /// <summary>
            /// Set all the initial ranges for the base data set.
            /// </summary>
            /// <param name="valueType">Whether it contains 32 bit Integers or Single precision floating point </param>
            /// <param name="numElements">Number of elements</param>
            /// <param name="elementMultiplier">Element Multiplier.</param>
            /// <param name="imag"></param>
            /// <param name="nameLength">Length of name</param>
            /// <param name="name">Name of data type</param>
            public BaseDataSet(int valueType, int numElements, int elementMultiplier, int imag, int nameLength, string name)
            {
                ValueType = valueType;
                NumElements = numElements;
                ElementsMultiplier = elementMultiplier;
                Imag = imag;
                NameLength = nameLength;
                Name = name;
            }

            /// <summary>
            /// Return the size of the Base data set.
            /// </summary>
            /// <returns>Size of base data set.</returns>
            public static int GetBaseDataSize(int nameLength)
            {
                // Name length gives length of name
                // The rest are 4 bytes for each item
                // 1 is subtracted because Name's size is given with NameLength.
                return nameLength + (Ensemble.BYTES_IN_INT32 * (NUM_DATASET_HEADER_ELEMENTS - 1));
            }

            /// <summary>
            /// Generate the size of the data set based off the number of
            /// elements, element multipler and the base data.  Each element size is based
            /// on the type of dataset.  Determine how many element
            /// exist and multiply by elementMultipler.  Then determine the size of the
            /// BaseData area and add that in.
            /// </summary>
            /// <param name="type">DatatSet Type (eg: float, int, byte...)</param>
            /// <param name="nameLength">Length of the name in bytes.</param>
            /// <param name="numElements">Number of Elements in the dataset</param>
            /// <param name="elementMultipler">Element Multipiler.  Usually number of beams.</param>
            /// <returns>Number of bytes in the data set.</returns>
            public static int GetDataSetSize(int type, int nameLength, int numElements, int elementMultipler)
            {
                // Find the number of bytes per element
                int dataType = Ensemble.BYTES_IN_FLOAT;
                switch(type)
                {
                    case Ensemble.DATATYPE_BYTE:
                        dataType = Ensemble.BYTES_IN_BYTES;
                        break;
                    case Ensemble.DATATYPE_INT:
                        dataType = Ensemble.BYTES_IN_INT32;
                        break;
                    case Ensemble.DATATYPE_FLOAT:
                        dataType = Ensemble.BYTES_IN_FLOAT;
                        break;
                    default:
                        dataType = Ensemble.BYTES_IN_FLOAT;
                        break;
                }

                return ((numElements * elementMultipler) * dataType) + BaseDataSet.GetBaseDataSize(nameLength);
            }

            /// <summary>
            /// For all the datasets that store beam Bin Bin ranges, this will
            /// generate the index for the value.
            /// 
            /// Value = 4 Bytes = [0|1|2|3]
            /// The data is stored in Beam Bin Bin.  The first 4 X NumElements are all Beam 0s ranges.
            /// Beam0 Values | Beam1 Values | Beam2 Values | Beam3 Values
            /// 
            /// Within BeamX Values are Y Number of Bin.
            /// 
            /// Beam0 Values = Beam0Bin0 | Beam0Bin1 | ... Beam0Bin(NumElements)
            /// </summary>
            /// <param name="nameLength">Length of the dataset name.</param>
            /// <param name="numBins">Number of bins.</param>
            /// <param name="beam">Number of beams.</param>
            /// <param name="bin">Bin number.</param>
            /// <returns>Index within the dataset.</returns>
            protected int GetBinBeamIndex( int nameLength, int numBins, int beam, int bin)
            {
                return GetBaseDataSize(nameLength) + (beam * numBins * Ensemble.BYTES_IN_FLOAT) + (bin * Ensemble.BYTES_IN_FLOAT);
            }

            /// <summary>
            /// Static version of BetBinBeamIndex.
            /// </summary>
            /// <param name="nameLength"></param>
            /// <param name="numBins"></param>
            /// <param name="beam"></param>
            /// <param name="bin"></param>
            /// <returns></returns>
            public static int GetBinBeamIndexStatic(int nameLength, int numBins, int beam, int bin)
            {
                return GetBaseDataSize(nameLength) + (beam * numBins * Ensemble.BYTES_IN_FLOAT) + (bin * Ensemble.BYTES_IN_FLOAT);
            }

            /// <summary>
            /// Generate the header for this data set.  This is used to
            /// encode the dataset back to binary form.
            /// [HEADER][PAYLOAD]
            /// Header = 28 bytes.  
            /// </summary>
            /// <param name="numElments">Number of elements in the dataset.  For NMEA its the payload size.</param>
            /// <returns>Byte array of the header for the the dataset.</returns>
            public byte[] GenerateHeader(int numElments)
            {
                byte[] result = new byte[DataSet.Ensemble.PAYLOAD_HEADER_LEN];

                // Add Data Type (Byte)
                byte[] dataType = MathHelper.Int32ToByteArray(ValueType);
                System.Buffer.BlockCopy(dataType, 0, result, 0, 4);

                // Add Bins or data length
                byte[] len = MathHelper.Int32ToByteArray(numElments);
                System.Buffer.BlockCopy(len, 0, result, 4, 4);

                // Add Beams (1 for Ensemble, Ancillary and Nmea, 4 for all other data sets.  )
                byte[] beams = MathHelper.Int32ToByteArray(ElementsMultiplier);
                System.Buffer.BlockCopy(beams, 0, result, 8, 4);

                // Add Image (default 0)
                byte[] image = MathHelper.Int32ToByteArray(Imag);
                System.Buffer.BlockCopy(image, 0, result, 12, 4);

                // Add NameLen (default 8)
                byte[] nameLen = MathHelper.Int32ToByteArray(NameLength);
                System.Buffer.BlockCopy(nameLen, 0, result, 16, 4);
 
                // Add Name (E0000XX\0)
                byte[] name = System.Text.Encoding.ASCII.GetBytes(Name);
                System.Buffer.BlockCopy(name, 0, result, 20, NameLength);

                return result;
            }

            /// <summary>
            /// Generate the header for this data set.  This is used to
            /// encode the dataset back to binary form.
            /// [HEADER][PAYLOAD]
            /// Header = 28 bytes.  
            /// </summary>
            /// <param name="numElments">Number of elements in the dataset.  For NMEA its the payload size.</param>
            /// <param name="ElementsMultiplier">Element Multipler.</param>
            /// <param name="ValueType">Type of data.  String, Float, Int or String</param>
            /// <param name="Imag">Null</param>
            /// <param name="Name">Name of the data.</param>
            /// <param name="NameLength">Name length.</param>
            /// <returns>Byte array of the header for the the dataset.</returns>
            public static byte[] GenerateHeader(int ValueType, int ElementsMultiplier, int Imag, int NameLength, string Name, int numElments)
            {
                int payloadHeaderLen = 20 + NameLength;         // Each element is = 4. 20 = ValueType + ElementsMultiplier + Imag + NameLength + numElements

                byte[] result = new byte[payloadHeaderLen];

                // Add Data Type (Byte)
                byte[] dataType = MathHelper.Int32ToByteArray(ValueType);
                System.Buffer.BlockCopy(dataType, 0, result, 0, 4);

                // Add Bins or data length
                byte[] len = MathHelper.Int32ToByteArray(numElments);
                System.Buffer.BlockCopy(len, 0, result, 4, 4);

                // Add Beams (1 for Ensemble, Ancillary and Nmea, 4 for all other data sets.  )
                byte[] beams = MathHelper.Int32ToByteArray(ElementsMultiplier);
                System.Buffer.BlockCopy(beams, 0, result, 8, 4);

                // Add Image (default 0)
                byte[] image = MathHelper.Int32ToByteArray(Imag);
                System.Buffer.BlockCopy(image, 0, result, 12, 4);

                // Add NameLen (default 8)
                byte[] nameLen = MathHelper.Int32ToByteArray(NameLength);
                System.Buffer.BlockCopy(nameLen, 0, result, 16, 4);

                // Add Name (E0000XX\0)
                byte[] name = System.Text.Encoding.ASCII.GetBytes(Name);
                System.Buffer.BlockCopy(name, 0, result, 20, NameLength);

                return result;
            }

            /// <summary>
            /// Give the base JSON information.  This will be used to combine with a dataset to 
            /// give the complete dataset information.
            /// 
            /// To use:
            /// ... 
            /// Write the base values
            /// writer.WriteRaw(base.ToJsonBaseStub());
            /// writer.WriteRaw(",");
            /// ...
            /// 
            /// </summary>
            /// <returns>Return a stub JSON string for the Base data.</returns>
            public string ToJsonBaseStub()
            {
                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    // Start the object
                    writer.Formatting = Formatting.None;            // Make the text not indented, so not as human readable.  This will save disk writing space

                    #region Base Values

                    // NumElements
                    writer.WritePropertyName(JSON_STR_NUMELEMENTS);
                    writer.WriteValue(NumElements);

                    // ElementsMultiplier
                    writer.WritePropertyName(JSON_STR_ELEMENTSMULTIPLIER);
                    writer.WriteValue(ElementsMultiplier);

                    #endregion

                }

                return sb.ToString();
            }

        }
    }
}