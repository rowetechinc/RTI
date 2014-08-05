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
 * 01/03/2013      RC          2.17       Initial coding
 * 01/08/2013      RC          2.17       Added Amplitude Averaging and finished Reference Layer averaging.
 *                                         Set the parameters of the averaging to the averaged ensemble (num samples, first/last ping time)
 * 01/09/2012      RC          2.17       Fixed setting the first and last ping time.
 * 06/27/2014      RC          2.23.0     Added Bottom Track, Earth Vel, Instrument Vel and Beam vel to average.
 * 07/14/2014      RC          2.23.0     Moved the logic for removing accumulated data from the averager to the manager.
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
using RTI.Average;
    using System.Timers;

    /// <summary>
    /// This will take in ensembles.  Pass them to the different accumulators and wait for averaged data.
    /// This will also keep all the properties for the averaging process.  This includes if it is a 
    /// running average and the number of samples for an average.  When the number of samples has been accummulated,
    /// an event will be set off with the average ensembled.  
    /// </summary>
    public class AverageManager
    {

        #region Variables

        #region Defaults

        /// <summary>
        /// Default first ping time.  Negative because the time
        /// can never be negative.
        /// </summary>
        public const float DEFAULT_FIRST_PING_TIME = -1.0f;

        #endregion

        /// <summary>
        /// Keep track of the number of ensembles received.
        /// If we are not doing a running average, we need
        /// to ensure when the count matches the NumSamples,
        /// that we publish the ensemble.  If it is a running average,
        /// we need to ensure that when count hits NumSamples, that
        /// we begin to publish ensembles.
        /// </summary>
        private int _ensCount;

        /// <summary>
        /// This is the first ping time in the accumulated data.  By default this will be
        /// negative so that when an ensemble is received and it is negative, then we know
        /// it needs to be set.
        /// </summary>
        private float _firstPingTime;

        /// <summary>
        /// The last ensemble.
        /// </summary>
        private DataSet.Ensemble _lastEnsemble;

        /// <summary>
        /// Average Manager options.
        /// </summary>
        private AverageManagerOptions _options;

        /// <summary>
        /// If timing based off time, enable this timer.
        /// </summary>
        private Timer _avgTimer;

        #region Averagers

        /// <summary>
        /// Average velocity data using Reference layer averaging.
        /// </summary>
        private ReferenceLayerAverage _refLayerAverager;

        /// <summary>
        /// Average correlation data.
        /// </summary>
        private AverageCorrelation _correlationAverager;

        /// <summary>
        /// Average amplitude data.
        /// </summary>
        private AverageAmplitude _amplitudeAverager;

        /// <summary>
        /// Average Beam Velocity data.
        /// </summary>
        private AverageBeamVelocity _beamVelAverager;

        /// <summary>
        /// Average Instrument Velocity data.
        /// </summary>
        private AverageInstrumentVelocity _instrumentVelAverager;

        /// <summary>
        /// Average Earth Velocity data.
        /// </summary>
        private AverageEarthVelocity _earthVelAverager;

        /// <summary>
        /// Average Bottom Track data.
        /// </summary>
        private AverageBottomTrack _bottomTrackAverager;

        #endregion

        #endregion

        #region Properties

        #region Average Parameters

        /// <summary>
        /// Set flag if recording the data to a file.
        /// </summary>
        public bool IsRecording
        {
            get { return _options.IsRecording; }
            set
            {
                _options.IsRecording = value;
            }
        }

        /// <summary>
        /// Set flag if the averaging is done by number of samples
        /// or timer.
        /// </summary>
        public bool IsAvgByNumSamples
        {
            get { return _options.IsAvgByNumSamples; }
            set
            {
                _options.IsAvgByNumSamples = value;
                
                // This will turn the timer on or off
                _avgTimer.Enabled = !value;
            }
        }

        /// <summary>
        /// Number of samples to accumulate before
        /// averaging the data.  The value cannot be less than or equal to 0.
        /// If a bad value is given, the default value will be used.
        /// </summary>
        public int NumSamples
        {
            get { return _options.NumSamples; }
            set
            {
                if (value <= 0)
                {
                    _options.NumSamples = AverageManagerOptions.DEFAULT_NUM_SAMPLES;
                }
                else
                {
                    _options.NumSamples = value;
                }

                // Set the averagers
                _correlationAverager.NumSamples = _options.NumSamples;
                _amplitudeAverager.NumSamples = _options.NumSamples;
                _refLayerAverager.NumSamples = _options.NumSamples;
                _beamVelAverager.NumSamples = _options.NumSamples;
                _instrumentVelAverager.NumSamples = _options.NumSamples;
                _earthVelAverager.NumSamples = _options.NumSamples;
                _bottomTrackAverager.NumSamples = _options.NumSamples;
            }
        }

        /// <summary>
        /// Number of milliseconds for the timer.
        /// </summary>
        public uint TimerMilliseconds
        {
            get { return _options.TimerMilliseconds; }
            set
            {
                _options.TimerMilliseconds = value;
            }
        }

        /// <summary>
        /// Set wheter the average will be a running average or an
        /// average with X number of samples have been received.
        /// 
        /// A running average will average together NumSamples together.
        /// As a new ensemble is added, the last one is removed and the 
        /// average is taken.  This will publish an averaged ensemble
        /// for every ensemble received.
        /// 
        /// A non-running (block) average will wait for NumSamples to be received.
        /// It will then take the average and clear the list of ensembles.
        /// It will then wait again for NumSamples.  This will publish an
        /// averaged ensemble after NumSamples have been received.
        /// </summary>
        public bool IsRunningAverage 
        {
            get { return _options.IsRunningAverage; }
            set
            {
                _options.IsRunningAverage = value;

                // Set the averagers
                _correlationAverager.IsRunningAverage = _options.IsRunningAverage;
                _amplitudeAverager.IsRunningAverage = _options.IsRunningAverage;
                _refLayerAverager.IsRunningAverage = _options.IsRunningAverage;
                _beamVelAverager.IsRunningAverage = _options.IsRunningAverage;
                _instrumentVelAverager.IsRunningAverage = _options.IsRunningAverage;
                _earthVelAverager.IsRunningAverage = _options.IsRunningAverage;
                _bottomTrackAverager.IsRunningAverage = _options.IsRunningAverage;
            }
        }

        #endregion

        #region Average Types

        #region Reference Layer Averaging

        /// <summary>
        /// Set a flag if we are going to reference layer average.
        /// </summary>
        public bool IsReferenceLayerAveraging 
        { 
            get { return _options.IsReferenceLayerAveraging; }
            set
            {
                _options.IsReferenceLayerAveraging = value;
            }
        }

        /// <summary>
        /// Minimum Reference layer for Reference layer averaging.
        /// </summary>
        public uint MinRefLayer
        {
            get { return _options.MinRefLayer; }
            set
            {
                _options.MinRefLayer = value;

                _refLayerAverager.SetMinMaxReferenceLayer(_options.MinRefLayer, _options.MaxRefLayer);
            }
        }

        /// <summary>
        /// Maximum Reference layer for Reference layer averaging.
        /// </summary>
        public uint MaxRefLayer
        {
            get { return _options.MaxRefLayer; }
            set
            {
                _options.MaxRefLayer = value;

                _refLayerAverager.SetMinMaxReferenceLayer(_options.MinRefLayer, _options.MaxRefLayer);
            }
        }

        #endregion

        #region Correlation Averaging

        /// <summary>
        /// Set flag if wer are going to average correlation data.
        /// </summary>
        public bool IsCorrelationAveraging 
        {
            get { return _options.IsCorrelationAveraging; } 
            set
            {
                _options.IsCorrelationAveraging = value;
            }
        }

        /// <summary>
        /// Scale value for the Correlation averaging.
        /// </summary>
        public float CorrelationScale
        {
            get { return _options.CorrelationScale; }
            set
            {
                _options.CorrelationScale = value;
            }
        }

        #endregion

        #region Amplitude Averaging

        /// <summary>
        /// Set flag if wer are going to average Amplitude data.
        /// </summary>
        public bool IsAmplitudeAveraging
        {
            get { return _options.IsAmplitudeAveraging; }
            set
            {
                _options.IsAmplitudeAveraging = value;
            }
        }

        /// <summary>
        /// Scale value for the Amplitude averaging.
        /// </summary>
        public float AmplitudeScale
        {
            get { return _options.AmplitudeScale; }
            set
            {
                _options.AmplitudeScale = value;
            }
        }

        #endregion

        #region Beam Velocity Averaging

        /// <summary>
        /// Set flag if wer are going to average Beam Velocity data.
        /// </summary>
        public bool IsBeamVelocityAveraging
        {
            get { return _options.IsBeamVelocityAveraging; }
            set
            {
                _options.IsBeamVelocityAveraging = value;
            }
        }

        /// <summary>
        /// Scale value for the Beam Velocity averaging.
        /// </summary>
        public float BeamVelocityScale
        {
            get { return _options.BeamVelocityScale; }
            set
            {
                _options.BeamVelocityScale = value;
            }
        }

        #endregion

        #region Earth Velocity Averaging

        /// <summary>
        /// Set flag if wer are going to average Earth Velocity data.
        /// </summary>
        public bool IsEarthVelocityAveraging
        {
            get { return _options.IsEarthVelocityAveraging; }
            set
            {
                _options.IsEarthVelocityAveraging = value;
            }
        }

        /// <summary>
        /// Scale value for the Earth Velocity averaging.
        /// </summary>
        public float EarthVelocityScale
        {
            get { return _options.EarthVelocityScale; }
            set
            {
                _options.EarthVelocityScale = value;
            }
        }

        #endregion

        #region Instrument Velocity Averaging

        /// <summary>
        /// Set flag if wer are going to average Instrument Velocity data.
        /// </summary>
        public bool IsInstrumentVelocityAveraging
        {
            get { return _options.IsInstrumentVelocityAveraging; }
            set
            {
                _options.IsInstrumentVelocityAveraging = value;
            }
        }

        /// <summary>
        /// Scale value for the Instrument Velocity averaging.
        /// </summary>
        public float InstrumentVelocityScale
        {
            get { return _options.InstrumentVelocityScale; }
            set
            {
                _options.InstrumentVelocityScale = value;
            }
        }

        #endregion

        #region Bottom Track Averaging

        /// <summary>
        /// Set flag if we are going to average Bottom Track data.
        /// </summary>
        public bool IsBottomTrackAveraging
        {
            get { return _options.IsBottomTrackAveraging; }
            set
            {
                _options.IsBottomTrackAveraging = value;
            }
        }

        /// <summary>
        /// Scale value for the Bottom Track Range averaging.
        /// </summary>
        public float BottomTrackRangeScale
        {
            get { return _options.BottomTrackRangeScale; }
            set
            {
                _options.BottomTrackRangeScale = value;
            }
        }

        /// <summary>
        /// Scale value for the Bottom Track SNR averaging.
        /// </summary>
        public float BottomTrackSnrScale
        {
            get { return _options.BottomTrackSnrScale; }
            set
            {
                _options.BottomTrackSnrScale = value;
            }
        }

        /// <summary>
        /// Scale value for the Bottom Track Correlation averaging.
        /// </summary>
        public float BottomTrackCorrelationScale
        {
            get { return _options.BottomTrackCorrelationScale; }
            set
            {
                _options.BottomTrackCorrelationScale = value;
            }
        }

        /// <summary>
        /// Scale value for the Bottom Track Amplitude averaging.
        /// </summary>
        public float BottomTrackAmplitudeScale
        {
            get { return _options.BottomTrackAmplitudeScale; }
            set
            {
                _options.BottomTrackAmplitudeScale = value;
            }
        }

        /// <summary>
        /// Scale value for the Bottom Track Beam Velocity averaging.
        /// </summary>
        public float BottomTrackBeamVelocityScale
        {
            get { return _options.BottomTrackBeamVelocityScale; }
            set
            {
                _options.BottomTrackBeamVelocityScale = value;
            }
        }

        /// <summary>
        /// Scale value for the Bottom Track Instrument Velocity averaging.
        /// </summary>
        public float BottomTrackInstrumentVelocityScale
        {
            get { return _options.BottomTrackInstrumentVelocityScale; }
            set
            {
                _options.BottomTrackInstrumentVelocityScale = value;
            }
        }

        /// <summary>
        /// Scale value for the Bottom Track Earth Velocity averaging.
        /// </summary>
        public float BottomTrackEarthVelocityScale
        {
            get { return _options.BottomTrackEarthVelocityScale; }
            set
            {
                _options.BottomTrackEarthVelocityScale = value;
            }
        }

        #endregion

        #endregion

        #endregion

        /// <summary>
        /// Initialize the values
        /// </summary>
        public AverageManager(AverageManagerOptions options)
        {
            // Set the options
            _options = options;

            _avgTimer = new Timer(_options.TimerMilliseconds);
            _avgTimer.Elapsed += new ElapsedEventHandler(_avgTimer_Elapsed);

            // Initialize values
            _refLayerAverager = new ReferenceLayerAverage(_options.NumSamples, _options.MinRefLayer, _options.MaxRefLayer, _options.IsRunningAverage);
            _correlationAverager = new AverageCorrelation();
            _amplitudeAverager = new AverageAmplitude();
            _beamVelAverager = new AverageBeamVelocity();
            _instrumentVelAverager = new AverageInstrumentVelocity();
            _earthVelAverager = new AverageEarthVelocity();
            _bottomTrackAverager = new AverageBottomTrack();
        }

        /// <summary>
        /// Accumulate ensembles to average.  This will pass the ensemble to all the
        /// different averagers to accumulate the data.  When the NumSamples has been met,
        /// an event will be sent with the averaged ensemble.
        /// </summary>
        /// <param name="ensemble"></param>
        public void AddEnsemble(DataSet.Ensemble ensemble)
        {
            // Correlation averager
            if (IsCorrelationAveraging)
            {
                _correlationAverager.AddEnsemble(ensemble);
            }

            // Amplitude averager
            if (IsAmplitudeAveraging)
            {
                _amplitudeAverager.AddEnsemble(ensemble);
            }

            // Beam Velocity Averager
            if (IsBeamVelocityAveraging)
            {
                _beamVelAverager.AddEnsemble(ensemble);
            }

            // Instrument Velocity Averager
            if (IsInstrumentVelocityAveraging)
            {
                _instrumentVelAverager.AddEnsemble(ensemble);
            }

            // Earth Velocity Averager
            if (IsEarthVelocityAveraging)
            {
                _earthVelAverager.AddEnsemble(ensemble);
            }

            // Reference layer averaging
            if (IsReferenceLayerAveraging)
            {
                _refLayerAverager.AddEnsemble(ensemble);
            }

            // Bottom Track averaging
            if (IsBottomTrackAveraging)
            {
                _bottomTrackAverager.AddEnsemble(ensemble);
            }

            // Set the first ping time if it has not been set
            if (ensemble.IsAncillaryAvail && _firstPingTime == DEFAULT_FIRST_PING_TIME)
            {
                _firstPingTime = ensemble.AncillaryData.FirstPingTime;
            }

            // Set the previous ensemble
            _lastEnsemble = ensemble;

            // Increment the ensemble count
            _ensCount++;

            // If we have met the number of samples
            // Publish the number of samples
            if (IsAvgByNumSamples && _ensCount >= NumSamples)
            {
                // Publish the averaged data
                PublishAverage(ensemble);

                // If we are not doing a running average
                // Then clear the ensemble count so we 
                // can start over counting
                if (!IsRunningAverage)
                {
                    ClearCount();
                }
                else
                {
                    // Set the new first ping time for running average
                    _firstPingTime = ensemble.AncillaryData.FirstPingTime;

                    // Keep the _ensCount the same as the NumSamples so the
                    // number does not overflow
                    _ensCount = NumSamples;

                    // Remove the first ensemble
                    RemoveFirstEnsemble();
                }
            }
        }


        #region Average

        /// <summary>
        /// If the timer is enabled and it goes off, this method will be called.
        /// Publish the accumulated averaged data.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Not used.</param>
        void _avgTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_lastEnsemble != null)
            {
                // Publish the averaged data
                PublishAverage(_lastEnsemble);
            }
        }

        /// <summary>
        /// Take the last ensemble as the parameter.  Fill in 
        /// the averaged data to the ensemble.  Then publish
        /// it to all the subscribers.
        /// </summary>
        /// <param name="ensemble">Last ensemble that was accumulated.</param>
        private void PublishAverage(DataSet.Ensemble ensemble)
        {
            // Clone the ensemble
            DataSet.Ensemble avgEnsemble = ensemble.Clone();

            // Set the num of samples and the first ping time to the ensemble
            SetAveragedEnsembleParameters(ref avgEnsemble);

            // Correlation Averaging
            if (IsCorrelationAveraging)
            {
                _correlationAverager.SetAverage(ref avgEnsemble, _options.CorrelationScale);
            }

            // Amplitude averaging
            if (IsAmplitudeAveraging)
            {
                _amplitudeAverager.SetAverage(ref avgEnsemble, _options.AmplitudeScale);
            }

            // Beam Velocity Averging
            if (IsBeamVelocityAveraging)
            {
                _beamVelAverager.SetAverage(ref avgEnsemble, _options.BeamVelocityScale);
            }

            // Instrument Velocity Averging
            if (IsInstrumentVelocityAveraging)
            {
                _instrumentVelAverager.SetAverage(ref avgEnsemble, _options.InstrumentVelocityScale);
            }

            // Earth Velocity Averging
            if (IsEarthVelocityAveraging)
            {
                _earthVelAverager.SetAverage(ref avgEnsemble, _options.EarthVelocityScale);
            }

            // Bottom Track Averging
            if (IsBottomTrackAveraging)
            {
                _bottomTrackAverager.SetAverage(ref avgEnsemble, _options.BottomTrackRangeScale, _options.BottomTrackSnrScale, 
                    _options.BottomTrackAmplitudeScale, _options.BottomTrackCorrelationScale, 
                    _options.BottomTrackBeamVelocityScale, _options.BottomTrackInstrumentVelocityScale, _options.BottomTrackEarthVelocityScale);
            }

            // Reference Layer Averaging
            if (IsReferenceLayerAveraging)
            {
                _refLayerAverager.SetAverage(ref avgEnsemble, 1.0f);
            }

            // Publish the ensemble to all the subscribers
            PublishAveragedEnsemble(avgEnsemble);

            // Clear the accumulated data if not a running average
            if (!IsRunningAverage)
            {
                // Clear the accumulated data
                ClearAccumulatedData();
            }
        }

        /// <summary>
        /// Set the paramters for the average.  This includes the number of samples,
        /// the first ping time and the current time.
        /// </summary>
        /// <param name="ensemble">Ensemble to set the parameters.</param>
        private void SetAveragedEnsembleParameters(ref DataSet.Ensemble ensemble)
        {
            // Set the first and last date time for the average
            // Set the number of ensembles in the average
            if (ensemble.IsEnsembleAvail)
            {
                // Update the ensemble with the number of samples
                ensemble.EnsembleData.UpdateAverageEnsemble(NumSamples);
            }

            if (ensemble.IsAncillaryAvail)
            {
                // Update the first ping time with the first ensemble in the accumulators first ping time
                ensemble.AncillaryData.FirstPingTime = _firstPingTime;
            }
        }

        #endregion

        #region Clear

        /// <summary>
        /// This will reset the number of ensembles that have been
        /// accumulated.
        /// </summary>
        private void ClearCount()
        {
            _ensCount = 0;
            _firstPingTime = DEFAULT_FIRST_PING_TIME;
        }

        /// <summary>
        /// Clear the accumulated data from all the
        /// averagers.
        /// </summary>
        private void ClearAccumulatedData()
        {
            _correlationAverager.ClearAllEnsembles();
            _amplitudeAverager.ClearAllEnsembles();
            _beamVelAverager.ClearAllEnsembles();
            _instrumentVelAverager.ClearAllEnsembles();
            _earthVelAverager.ClearAllEnsembles();
            _refLayerAverager.ClearAllEnsembles();
            _bottomTrackAverager.ClearAllEnsembles();
        }

        /// <summary>
        /// Remove the first ensemble accumulated from all
        /// the averagers.
        /// </summary>
        private void RemoveFirstEnsemble()
        {
            _correlationAverager.RemoveFirstEnsemble();
            _amplitudeAverager.RemoveFirstEnsemble();
            _beamVelAverager.RemoveFirstEnsemble();
            _instrumentVelAverager.RemoveFirstEnsemble();
            _earthVelAverager.RemoveFirstEnsemble();
            _refLayerAverager.RemoveFirstEnsemble();
            _bottomTrackAverager.RemoveFirstEnsemble();
        }

        #endregion

        #region Event

        #region Averaged Ensemble

        /// <summary>
        /// Event To subscribe to.  This gives the paramater
        /// that will be passed when subscribing to the event.
        /// </summary>
        /// <param name="ensemble">Data to pass to subscriber.</param>
        public delegate void AveragedEnsembleEventHandler(DataSet.Ensemble ensemble);

        /// <summary>
        /// Subscribe to this event.  This will hold all subscribers.
        /// 
        /// To subscribe:
        /// refLayerAvger.AveragedEnsemble += new refLayerAvger.AveragedEnsembleEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// refLayerAvger.AveragedEnsemble -= (method to call)
        /// </summary>
        public event AveragedEnsembleEventHandler AveragedEnsemble;

        /// <summary>
        /// Publish the averaged ensemble to all subscribers.
        /// This will verify there are subscribers, if there are
        /// subscribers, it will pass the ensemble to all subscribers.
        /// </summary>
        /// <param name="ensemble"></param>
        private void PublishAveragedEnsemble(DataSet.Ensemble ensemble)
        {
            if (AveragedEnsemble != null)
            {
                AveragedEnsemble(ensemble);
            }
        }

        #endregion

        #endregion



    }
}
