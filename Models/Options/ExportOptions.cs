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
 * 07/23/2017      RC          1.0.0      Initial coding
 * 10/06/2017      RC          1.1.0      Added screening options.
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Options to export an ensemble to a different
    /// format.
    /// </summary>
    public class ExportOptions
    {
        #region Properties

        /// <summary>
        /// Minimum ensemble number.
        /// </summary>
        public UInt32 MinEnsembleNumber { get; set; }

        /// <summary>
        /// Maximum ensemble number.
        /// </summary>
        public UInt32 MaxEnsembleNumber { get; set; }

        /// <summary>
        /// Number of bytes for the maximum file size.
        /// </summary>
        public int MaxFileSize { get; set; }

        /// <summary>
        /// If exporting to PD0, a coordinate transform
        /// needs to be set.  
        /// </summary>
        public PD0.CoordinateTransforms CoordinateTransform { get; set; }

        #region Turn on DataSet

        /// <summary>
        /// Turn on or off the Amplitude dataset.
        /// </summary>
        public bool IsAmplitudeDataSetOn { get; set; }

        /// <summary>
        /// Turn on or off the Ancillary dataset.
        /// </summary>
        public bool IsAncillaryDataSetOn { get; set; }

        /// <summary>
        /// Turn on or off the Beam Velocity dataset.
        /// </summary>
        public bool IsBeamVelocityDataSetOn { get; set; }

        /// <summary>
        /// Turn on or off the Bottom Track dataset.
        /// </summary>
        public bool IsBottomTrackDataSetOn { get; set; }

        /// <summary>
        /// Turn on or off the Bottom Track Engineering dataset.
        /// </summary>
        public bool IsBottomTrackEngineeringDataSetOn { get; set; }

        /// <summary>
        /// Turn on or off the Correlation dataset.
        /// </summary>
        public bool IsCorrelationDataSetOn { get; set; }

        /// <summary>
        /// Turn on or off the Earth Velocity dataset.
        /// </summary>
        public bool IsEarthVelocityDataSetOn { get; set; }

        /// <summary>
        /// Turn on or off the Velocity Vector dataset.
        /// </summary>
        public bool IsVelocityVectorDataSetOn { get; set; }

        /// <summary>
        /// Turn on or off the Earth Water Mass dataset.
        /// </summary>
        public bool IsEarthWaterMassDataSetOn { get; set; }

        /// <summary>
        /// Turn on or off the Ensemble dataset.
        /// </summary>
        public bool IsEnsembleDataSetOn { get; set; }

        /// <summary>
        /// Turn on or off the Good Beam dataset.
        /// </summary>
        public bool IsGoodBeamDataSetOn { get; set; }

        /// <summary>
        /// Turn on or off the Good Earth dataset.
        /// </summary>
        public bool IsGoodEarthDataSetOn { get; set; }

        /// <summary>
        /// Turn on or off the Instrument Velocity dataset.
        /// </summary>
        public bool IsInstrumentVelocityDataSetOn { get; set; }

        /// <summary>
        /// Turn on or off the Instrument Water Mass dataset.
        /// </summary>
        public bool IsInstrumentWaterMassDataSetOn { get; set; }

        /// <summary>
        /// Turn on or off the Nmea dataset.
        /// </summary>
        public bool IsNmeaDataSetOn { get; set; }

        /// <summary>
        /// Turn on or off the Profile Engineering dataset.
        /// </summary>
        public bool IsProfileEngineeringDataSetOn { get; set; }

        /// <summary>
        /// Turn on or off the System Setup dataset.
        /// </summary>
        public bool IsSystemSetupDataSetOn { get; set; }

        /// <summary>
        /// Turn on or off the Range Tracking dataset.
        /// </summary>
        public bool IsRangeTrackingDataSetOn { get; set; }

        /// <summary>
        /// Turn on or off the Gage Height dataset.
        /// </summary>
        public bool IsGageHeightDataSetOn { get; set; }

        /// <summary>
        /// Turn on or off the ADCP GPS dataset.
        /// </summary>
        public bool IsAdcpGpsDataSetOn { get; set; }

        /// <summary>
        /// Turn on or off the GPS 1 dataset.
        /// </summary>
        public bool IsGps1DataSetOn { get; set; }

        /// <summary>
        /// Turn on or off the GPS 2 dataset.
        /// </summary>
        public bool IsGps2DataSetOn { get; set; }

        /// <summary>
        /// Turn on or off the NMEA 1 dataset.
        /// </summary>
        public bool IsNmea1DataSetOn { get; set; }

        /// <summary>
        /// Turn on or off the NMEA 2 dataset.
        /// </summary>
        public bool IsNmea2DataSetOn { get; set; }

        /// <summary>
        /// Turn on or off retransforming the data.
        /// </summary>
        public bool IsRetransformData { get; set; }

        /// <summary>
        /// Turn on or off marking bad below bottom.
        /// </summary>
        public bool IsMarkBadBelowBottom { get; set; }

        /// <summary>
        /// Turn on or off removing the ship speed.
        /// </summary>
        public bool IsRemoveShipSpeed { get; set; }

        #region Waves

        /// <summary>
        /// Correlaton threshold.
        /// </summary>
        public float CorrelationThreshold { get; set; }

        /// <summary>
        /// Pressure offset.
        /// </summary>
        public float PressureOffset { get; set; }

        #endregion

        #endregion

        #region Bin Index

        /// <summary>
        /// Beam Velocity Bin Minimum index.
        /// </summary>
        public int BeamMinBin { get; set; }

        /// <summary>
        /// Beam Velcoity Bin Maximum index.
        /// </summary>
        public int BeamMaxBin { get; set; }

        /// <summary>
        /// Instrument Velocity Bin Minimum index.
        /// </summary>
        public int InstrumentMinBin { get; set; }

        /// <summary>
        /// Instrument Velcoity Bin Maximum index.
        /// </summary>
        public int InstrumentMaxBin { get; set; }

        /// <summary>
        /// Earth Velocity Bin Minimum index.
        /// </summary>
        public int EarthMinBin { get; set; }

        /// <summary>
        /// Earth Velcoity Bin Maximum index.
        /// </summary>
        public int EarthMaxBin { get; set; }

        /// <summary>
        /// Velocity Velocity Bin Minimum index.
        /// </summary>
        public int VelVectorMinBin { get; set; }

        /// <summary>
        /// Velcoity Vector Bin Maximum index.
        /// </summary>
        public int VelVectorMaxBin { get; set; }

        /// <summary>
        /// Amplitude Minimum bin index.
        /// </summary>
        public int AmplitudeMinBin { get; set; }

        /// <summary>
        /// Amplitude Maximum bin index.
        /// </summary>
        public int AmplitudeMaxBin { get; set; }

        /// <summary>
        /// Correlation Minimum bin index.
        /// </summary>
        public int CorrelationMinBin { get; set; }

        /// <summary>
        /// Correlation Maximum bin index.
        /// </summary>
        public int CorrelationMaxBin { get; set; }

        /// <summary>
        /// Good Beam Minimum bin index.
        /// </summary>
        public int GoodBeamMinBin { get; set; }

        /// <summary>
        /// Good Beam Maximum bin index.
        /// </summary>
        public int GoodBeamMaxBin { get; set; }

        /// <summary>
        /// Good Beam Minimum bin index.
        /// </summary>
        public int GoodEarthMinBin { get; set; }

        /// <summary>
        /// Good Beam Maximum bin index.
        /// </summary>
        public int GoodEarthMaxBin { get; set; }

        #endregion

        #endregion

        /// <summary>
        /// Initialize the options.
        /// </summary>
        public ExportOptions()
        {
            Initialzie();
        }

        /// <summary>
        /// Initialize the values.
        /// </summary>
        public void Initialzie()
        {
            MinEnsembleNumber = 0;
            MaxEnsembleNumber = 0;
            MaxFileSize = 1048576 * 50; // 50 MegaBytes
            IsAmplitudeDataSetOn = true;
            IsAncillaryDataSetOn = true;
            IsBeamVelocityDataSetOn = true;
            IsBottomTrackDataSetOn = true;
            IsCorrelationDataSetOn = true;
            IsEarthVelocityDataSetOn = true;
            IsVelocityVectorDataSetOn = true;
            IsEarthWaterMassDataSetOn = true;
            IsEnsembleDataSetOn = true;
            IsGoodBeamDataSetOn = true;
            IsGoodEarthDataSetOn = true;
            IsInstrumentVelocityDataSetOn = true;
            IsInstrumentWaterMassDataSetOn = true;
            IsNmeaDataSetOn = true;
            IsProfileEngineeringDataSetOn = true;
            IsBottomTrackEngineeringDataSetOn = true;
            IsSystemSetupDataSetOn = true;
            IsRangeTrackingDataSetOn = true;
            IsGageHeightDataSetOn = true;
            IsAdcpGpsDataSetOn = true;
            IsGps1DataSetOn = true;
            IsGps2DataSetOn = true;
            IsNmea1DataSetOn = true;
            IsNmea2DataSetOn = true;
            CoordinateTransform = PD0.CoordinateTransforms.Coord_Earth;
            BeamMinBin = 0;
            BeamMaxBin = 0;
            InstrumentMinBin = 0;
            InstrumentMaxBin = 0;
            EarthMinBin = 0;
            EarthMaxBin = 0;
            VelVectorMinBin = 0;
            VelVectorMaxBin = 0;
            CorrelationMinBin = 0;
            CorrelationMaxBin = 0;
            AmplitudeMinBin = 0;
            AmplitudeMaxBin = 0;
            GoodBeamMinBin = 0;
            GoodBeamMaxBin = 0;
            GoodEarthMinBin = 0;
            GoodEarthMaxBin = 0;
            CorrelationThreshold = 0.0f;
            PressureOffset = 0.0f;
            IsRetransformData = false;
            IsMarkBadBelowBottom = true;
            IsRemoveShipSpeed = true;
        }

        /// <summary>
        /// Set the max bin for all the options.
        /// </summary>
        /// <param name="maxBin">Maximum bin to display.</param>
        public void SetMaxBin(int maxBin)
        {
            BeamMaxBin = maxBin;
            InstrumentMaxBin = maxBin;
            EarthMaxBin = maxBin;
            VelVectorMaxBin = maxBin;
            CorrelationMaxBin = maxBin;
            AmplitudeMaxBin = maxBin;
            GoodBeamMaxBin = maxBin;
            GoodEarthMaxBin = maxBin;
        }

    }
}
