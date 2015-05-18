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
 * 07/03/2014      RC          2.23.0       Initial coding
 * 07/28/2014      RC          2.23.0       Added the IsRecording property.
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Average Manager options.
    /// </summary>
    public class AverageManagerOptions
    {
        #region Variables

        #region Defaults

        /// <summary>
        /// Default value if recording.
        /// </summary>
        public const bool DEFAULT_IS_RECORDING = false;

        /// <summary>
        /// Default value if averaging by number of samples.
        /// </summary>
        public const bool DEFAULT_IS_AVG_BY_NUM_SAMPLES = false;

        /// <summary>
        /// Default value if averaging by timer.
        /// </summary>
        public const bool DEFAULT_IS_AVG_BY_TIME = false;

        /// <summary>
        /// Default value if running averaging.
        /// </summary>
        public const bool DEFAULT_IS_AVG_RUNNING = true;

        /// <summary>
        /// Default number of samples if a bad number
        /// of samples is given.
        /// </summary>
        public const int DEFAULT_NUM_SAMPLES = 2;

        /// <summary>
        /// Default number of milliseconds for averaging the data if using the timer.
        /// </summary>
        public const uint DEFAULT_TIMER_MILLISEC = 2000;

        /// <summary>
        /// Default minimum reference layer.
        /// </summary>
        public const int DEFAULT_MIN_REF_LAYER = 1;

        /// <summary>
        /// Default maximum reference layer.
        /// </summary>
        public const int DEFAULT_MAX_REF_LAYER = 3;

        /// <summary>
        /// Default flag for reference layer averaging.
        /// </summary>
        public const bool DEFAULT_IS_REF_LAYER_AVG = false;

        /// <summary>
        /// Default flag for Correlation Averaging.
        /// </summary>
        public const bool DEFAULT_IS_CORR_AVG = false;

        /// <summary>
        /// Default Correlation Scale value.
        /// </summary>
        public const float DEFAULT_CORR_SCALE = 1.0f;

        /// <summary>
        /// Default flag for Amplitude averaging.
        /// </summary>
        public const bool DEFAULT_IS_AMP_AVG = false;

        /// <summary>
        /// Default Amplitude Scale value.
        /// </summary>
        public const float DEFAULT_AMP_SCALE = 1.0f;

        /// <summary>
        /// Default flag for Beam Velocity averaging.
        /// </summary>
        public const bool DEFAULT_IS_BEAM_VEL_AVG = false;

        /// <summary>
        /// Default Beam Velocity Scale value.
        /// </summary>
        public const float DEFAULT_BEAM_VEL_SCALE = 1.0f;

        /// <summary>
        /// Default flag for Instrument Velocity averaging.
        /// </summary>
        public const bool DEFAULT_IS_INSTRUMENT_VEL_AVG = false;

        /// <summary>
        /// Default Instrument Velocity Scale value.
        /// </summary>
        public const float DEFAULT_INSTRUMENT_VEL_SCALE = 1.0f;

        /// <summary>
        /// Default flag for Earth Velocity averaging.
        /// </summary>
        public const bool DEFAULT_IS_EARTH_VEL_AVG = false;

        /// <summary>
        /// Default Earth Velocity Scale value.
        /// </summary>
        public const float DEFAULT_EARTH_VEL_SCALE = 1.0f;

        /// <summary>
        /// Default flag for Bottom Track averaging.
        /// </summary>
        public const bool DEFAULT_IS_BT_AVG = false;

        /// <summary>
        /// Default Bottom Track Range Scale value.
        /// </summary>
        public const float DEFAULT_BT_RANGE_SCALE = 1.0f;

        /// <summary>
        /// Default Bottom Track SNR Scale value.
        /// </summary>
        public const float DEFAULT_BT_SNR_SCALE = 1.0f;

        /// <summary>
        /// Default Bottom Track Amplitude Scale value.
        /// </summary>
        public const float DEFAULT_BT_AMP_SCALE = 1.0f;

        /// <summary>
        /// Default Bottom Track Correlation Scale value.
        /// </summary>
        public const float DEFAULT_BT_CORR_SCALE = 1.0f;

        /// <summary>
        /// Default Bottom Track Beam Velocity Scale value.
        /// </summary>
        public const float DEFAULT_BT_BEAM_VEL_SCALE = 1.0f;

        /// <summary>
        /// Default Bottom Track INstrument Velocity Scale value.
        /// </summary>
        public const float DEFAULT_BT_INSTRUMENT_VEL_SCALE = 1.0f;

        /// <summary>
        /// Default Bottom Track Earth Velocity Scale value.
        /// </summary>
        public const float DEFAULT_BT_EARTH_VEL_SCALE = 1.0f;

        /// <summary>
        /// Default value for _ensCount.
        /// </summary>
        public const int DEFAULT_ENS_COUNT = 0;

        /// <summary>
        /// Default value for Is Running Average.
        /// </summary>
        public const bool DEFAULT_IS_RUNNING_AVG = false;

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Record the incoming data to a binary file.
        /// </summary>
        public bool IsRecording { get; set; }

        /// <summary>
        /// If this is set to true, then it will average based off the number of
        /// samples collected.  If this is false, it will averaged based off
        /// a timed event.
        /// </summary>
        public bool IsAvgByNumSamples { get; set; }

        /// <summary>
        /// If this is set to true, then it will average based off timed event.  
        /// </summary>
        public bool IsAvgByTimer { get; set; }

        /// <summary>
        /// Flag if a running average.
        /// </summary>
        public bool IsAvgRunning { get; set; }

        /// <summary>
        /// Number of samples averaged together.
        /// </summary>
        public int NumSamples { get; set; }

        /// <summary>
        /// The number of seconds to accumulate data before
        /// averaging the data.  
        /// </summary>
        public uint TimerMilliseconds { get; set; }

        /// <summary>
        /// Flag if the sample averaging is running average.
        /// </summary>
        public bool IsSampleRunningAverage { get; set; }

        /// <summary>
        /// Set flag if reference layer averaging.
        /// </summary>
        public bool IsReferenceLayerAveraging { get; set; }

        /// <summary>
        /// Minimum reference layer.
        /// </summary>
        public uint MinRefLayer { get; set; }

        /// <summary>
        /// Maximum reference layer.
        /// </summary>
        public uint MaxRefLayer { get; set; }

        /// <summary>
        /// Set flag if Correlation averaging.
        /// </summary>
        public bool IsCorrelationAveraging { get; set; }

        /// <summary>
        /// Scale value to multiple to the Correlation averaged value.
        /// </summary>
        public float CorrelationScale { get; set; }

        /// <summary>
        /// Set flag if Amplitude averaging.
        /// </summary>
        public bool IsAmplitudeAveraging { get; set; }

        /// <summary>
        /// Scale value to multiple to the Amplitude averaged value.
        /// </summary>
        public float AmplitudeScale { get; set; }

        /// <summary>
        /// Set flag if Beam Velocity averaging.
        /// </summary>
        public bool IsBeamVelocityAveraging { get; set; }

        /// <summary>
        /// Scale value to multiple to the Beam Velocity averaged value.
        /// </summary>
        public float BeamVelocityScale { get; set; }

        /// <summary>
        /// Set flag if Earth Velocity averaging.
        /// </summary>
        public bool IsEarthVelocityAveraging { get; set; }

        /// <summary>
        /// Scale value to multiple to the Earth Velocity averaged value.
        /// </summary>
        public float EarthVelocityScale { get; set; }

        /// <summary>
        /// Set flag if Instrument Velocity averaging.
        /// </summary>
        public bool IsInstrumentVelocityAveraging { get; set; }

        /// <summary>
        /// Scale value to multiple to the Instrument Velocity averaged value.
        /// </summary>
        public float InstrumentVelocityScale { get; set; }

        /// <summary>
        /// Set flag if Bottom Track averaging.
        /// </summary>
        public bool IsBottomTrackAveraging { get; set; }

        /// <summary>
        /// Scale value to multiple to the Bottom Track Range averaged value.
        /// </summary>
        public float BottomTrackRangeScale { get; set; }

        /// <summary>
        /// Scale value to multiple to the Bottom Track SNR averaged value.
        /// </summary>
        public float BottomTrackSnrScale { get; set; }

        /// <summary>
        /// Scale value to multiple to the Bottom Track Amplitude averaged value.
        /// </summary>
        public float BottomTrackAmplitudeScale { get; set; }

        /// <summary>
        /// Scale value to multiple to the Bottom Track Correlation averaged value.
        /// </summary>
        public float BottomTrackCorrelationScale { get; set; }

        /// <summary>
        /// Scale value to multiple to the Bottom Track Beam Velocity averaged value.
        /// </summary>
        public float BottomTrackBeamVelocityScale { get; set; }

        /// <summary>
        /// Scale value to multiple to the Bottom Track Instrument Velocity averaged value.
        /// </summary>
        public float BottomTrackInstrumentVelocityScale { get; set; }

        /// <summary>
        /// Scale value to multiple to the Bottom Track Earth Velocity averaged value.
        /// </summary>
        public float BottomTrackEarthVelocityScale { get; set; }

        #endregion

        /// <summary>
        /// Initialize the values.
        /// </summary>
        public AverageManagerOptions()
        {
            // Initialize
            Init();
        }

        /// <summary>
        /// Initialize the values.
        /// </summary>
        private void Init()
        {
            // These must be set after the averagers are created
            IsRecording = DEFAULT_IS_RECORDING;
            IsAvgRunning = DEFAULT_IS_AVG_RUNNING;
            IsAvgByNumSamples = DEFAULT_IS_AVG_BY_NUM_SAMPLES;
            IsAvgByTimer = DEFAULT_IS_AVG_BY_TIME;
            IsSampleRunningAverage = DEFAULT_IS_RUNNING_AVG;
            NumSamples = DEFAULT_NUM_SAMPLES;
            MinRefLayer = DEFAULT_MIN_REF_LAYER;
            MaxRefLayer = DEFAULT_MAX_REF_LAYER;
            TimerMilliseconds = DEFAULT_TIMER_MILLISEC;
            IsReferenceLayerAveraging = DEFAULT_IS_REF_LAYER_AVG;
            IsCorrelationAveraging = DEFAULT_IS_CORR_AVG;
            CorrelationScale = DEFAULT_CORR_SCALE;
            IsAmplitudeAveraging = DEFAULT_IS_AMP_AVG;
            AmplitudeScale = DEFAULT_AMP_SCALE;
            IsBeamVelocityAveraging = DEFAULT_IS_BEAM_VEL_AVG;
            BeamVelocityScale = DEFAULT_BEAM_VEL_SCALE;
            IsInstrumentVelocityAveraging = DEFAULT_IS_INSTRUMENT_VEL_AVG;
            InstrumentVelocityScale = DEFAULT_INSTRUMENT_VEL_SCALE;
            IsEarthVelocityAveraging = DEFAULT_IS_EARTH_VEL_AVG;
            EarthVelocityScale = DEFAULT_EARTH_VEL_SCALE;
            IsBottomTrackAveraging = DEFAULT_IS_BT_AVG;
            BottomTrackRangeScale = DEFAULT_BT_RANGE_SCALE;
            BottomTrackSnrScale = DEFAULT_BT_SNR_SCALE;
            BottomTrackAmplitudeScale = DEFAULT_BT_AMP_SCALE;
            BottomTrackCorrelationScale = DEFAULT_BT_CORR_SCALE;
            BottomTrackBeamVelocityScale = DEFAULT_BT_BEAM_VEL_SCALE;
            BottomTrackInstrumentVelocityScale = DEFAULT_BT_INSTRUMENT_VEL_SCALE;
            BottomTrackEarthVelocityScale = DEFAULT_BT_EARTH_VEL_SCALE;
        }

    }
}
