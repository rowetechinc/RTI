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
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
using RTI.Average;

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
        /// Default number of samples if a bad number
        /// of samples is given.
        /// </summary>
        private const int DEFAULT_NUM_SAMPLES = 2;

        /// <summary>
        /// Default scale.
        /// </summary>
        private const float DEFAULT_SCALE = 1.0f;

        /// <summary>
        /// Default minimum reference layer.
        /// </summary>
        private const int DEFAULT_MIN_REF_LAYER = 1;

        /// <summary>
        /// Default maximum reference layer.
        /// </summary>
        private const int DEFAULT_MAX_REF_LAYER = 3;

        /// <summary>
        /// Default flag for reference layer averaging.
        /// </summary>
        private const bool DEFAULT_IS_REF_LAYER_AVG = false;

        /// <summary>
        /// Default flag for Correlation Averaging.
        /// </summary>
        private const bool DEFAULT_IS_CORR_AVG = false;

        /// <summary>
        /// Default flag for Amplitude averaging.
        /// </summary>
        private const bool DEFAULT_IS_AMP_AVG = false;

        /// <summary>
        /// Default value for _ensCount.
        /// </summary>
        private const int DEFAULT_ENS_COUNT = 0;

        /// <summary>
        /// Default value for Is Running Average.
        /// </summary>
        private const bool DEFAULT_IS_RUNNING_AVG = false;

        /// <summary>
        /// Default first ping time.  Negative because the time
        /// can never be negative.
        /// </summary>
        private const float DEFAULT_FIRST_PING_TIME = -1.0f;

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

        #endregion

        #endregion

        #region Properties

        #region Average Parameters

        /// <summary>
        /// Number of samples to accumulate before
        /// averaging the data.  The value cannot be less than or equal to 0.
        /// If a bad value is given, the default value will be used.
        /// </summary>
        private int _numSamples;
        /// <summary>
        /// Number of samples to accumulate before
        /// averaging the data.  The value cannot be less than or equal to 0.
        /// If a bad value is given, the default value will be used.
        /// </summary>
        public int NumSamples
        {
            get { return _numSamples; }
            set
            {
                if (value <= 0)
                {
                    _numSamples = DEFAULT_NUM_SAMPLES;
                }
                else
                {
                    _numSamples = value;
                }

                // Set the averagers
                _correlationAverager.NumSamples = _numSamples;
                _amplitudeAverager.NumSamples = _numSamples;
                _refLayerAverager.NumSamples = _numSamples;
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
        private bool _isRunningAverage;
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
            get { return _isRunningAverage; }
            set
            {
                _isRunningAverage = value;

                // Set the averagers
                _correlationAverager.IsRunningAverage = _isRunningAverage;
                _amplitudeAverager.IsRunningAverage = _isRunningAverage;
                _refLayerAverager.IsRunningAverage = _isRunningAverage;
            }
        }


        /// <summary>
        /// Get or set the scale for the average value.  This value should be
        /// mulitplied to the averaged value.
        /// </summary>
        private float _scale;
        /// <summary>
        /// Get or set the scale for the average value.  This value should be
        /// mulitplied to the averaged value.
        /// </summary>
        public float Scale 
        {
            get { return _scale; }
            set
            {
                _scale = value;

                // Set the averagers
                _correlationAverager.Scale = _scale;
                _amplitudeAverager.Scale = _scale;
                
            }
        }

        #endregion

        #region Average Types

        #region Reference Layer Averaging

        /// <summary>
        /// Set a flag if we are going to reference layer average.
        /// </summary>
        public bool IsReferenceLayerAveraging { get; set; }

        /// <summary>
        /// Minimum Reference layer for Reference layer averaging.
        /// </summary>
        private int _minRefLayer;
        /// <summary>
        /// Minimum Reference layer for Reference layer averaging.
        /// </summary>
        public int MinRefLayer
        {
            get { return _minRefLayer; }
            set
            {
                _minRefLayer = value;

                _refLayerAverager.SetMinMaxReferenceLayer(_minRefLayer, _maxRefLayer);
            }
        }

        /// <summary>
        /// Maximum Reference layer for Reference layer averaging.
        /// </summary>
        private int _maxRefLayer;
        /// <summary>
        /// Maximum Reference layer for Reference layer averaging.
        /// </summary>
        public int MaxRefLayer
        {
            get { return _maxRefLayer; }
            set
            {
                _maxRefLayer = value;

                _refLayerAverager.SetMinMaxReferenceLayer(_minRefLayer, _maxRefLayer);
            }
        }

        #endregion

        #region Correlation Averaging

        /// <summary>
        /// Set flag if wer are going to average correlation data.
        /// </summary>
        public bool IsCorrelationAveraging { get; set; }
        

        #endregion

        #region Amplitude Averaging

        /// <summary>
        /// Set flag if wer are going to average Amplitude data.
        /// </summary>
        public bool IsAmplitudeAveraging { get; set; }


        #endregion

        #endregion

        #endregion

        /// <summary>
        /// Initialize the values
        /// </summary>
        public AverageManager()
        {
            // Initialize values
            _refLayerAverager = new ReferenceLayerAverage(DEFAULT_NUM_SAMPLES, DEFAULT_MIN_REF_LAYER, DEFAULT_MAX_REF_LAYER, DEFAULT_IS_RUNNING_AVG);
            _correlationAverager = new AverageCorrelation();
            _amplitudeAverager = new AverageAmplitude();

            // Set Defaults
            SetDefaults();

        }

        /// <summary>
        ///  Set the default values.
        /// </summary>
        public void SetDefaults()
        {
            // These must be set after the averagers are created
            _ensCount = DEFAULT_ENS_COUNT;
            _firstPingTime = DEFAULT_FIRST_PING_TIME;
            IsReferenceLayerAveraging = DEFAULT_IS_REF_LAYER_AVG;
            IsCorrelationAveraging = DEFAULT_IS_CORR_AVG;
            IsAmplitudeAveraging = DEFAULT_IS_AMP_AVG;
            _isRunningAverage = DEFAULT_IS_RUNNING_AVG;
            _scale = DEFAULT_SCALE;
            _numSamples = DEFAULT_NUM_SAMPLES;
            _minRefLayer = DEFAULT_MIN_REF_LAYER;
            _maxRefLayer = DEFAULT_MAX_REF_LAYER;
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

            // Reference layer averaging
            if (IsReferenceLayerAveraging)
            {
                _refLayerAverager.AddEnsemble(ensemble);
            }

            // Set the first ping time if it has not been set
            if (ensemble.IsAncillaryAvail && _firstPingTime == DEFAULT_FIRST_PING_TIME)
            {
                _firstPingTime = ensemble.AncillaryData.FirstPingTime;
            }

            // Increment the ensemble count
            _ensCount++;

            // If we have met the number of samples
            // Publish the number of samples
            if (_ensCount >= NumSamples)
            {
                // Publish the averaged data
                PublishAverage(ensemble);

                // If we are not doing a running average
                // Then clear the ensemble count so we 
                // can start over counting
                if (!_isRunningAverage)
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
                }
            }
        }

        /// <summary>
        /// This will reset the number of ensembles that have been
        /// accumulated.
        /// </summary>
        public void ClearCount()
        {
            _ensCount = 0;
            _firstPingTime = DEFAULT_FIRST_PING_TIME;
        }

        #region Methods

        #region Average

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
                _correlationAverager.SetAverage(ref avgEnsemble);
            }

            // Amplitude averaging
            if (IsAmplitudeAveraging)
            {
                _amplitudeAverager.SetAverage(ref avgEnsemble);
            }

            // Reference Layer Averaging
            if (IsReferenceLayerAveraging)
            {
                _refLayerAverager.SetAverage(ref avgEnsemble);
            }

            // Publish the ensemble to all the subscribers
            PublishAveragedEnsemble(avgEnsemble);
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
                ensemble.EnsembleData.UpdateAverageEnsemble(_numSamples);
            }

            if (ensemble.IsAncillaryAvail)
            {
                // Update the first ping time with the first ensemble in the accumulators first ping time
                ensemble.AncillaryData.FirstPingTime = _firstPingTime;
            }
        }

        #endregion

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
