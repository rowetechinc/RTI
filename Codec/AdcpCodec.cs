﻿/*
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
 * 
 * HISTORY
 * -----------------------------------------------------------------
 * Date            Initials    Version    Comments
 * -----------------------------------------------------------------
 * 09/12/2014      RC          3.0.1      Initial coding
 * 06/22/2015      RC          3.0.5      Made all the incoming data run in a task to seperate threads.
 * 07/09/2015      RC          3.0.5      Mode the codecs there own thread.
 * 07/27/2015      RC          3.0.5      Check when to clear the codecs based on how many ensembles found.
 * 08/13/2015      RC          3.0.5      Check for complete event for all the codec.
 * 10/10/2016      RC          3.3.2      Changed binary codec to BinaryCodecNew.
 * 09/28/2017      RC          3.4.4      Pass the original data format when the data is sent based off the codec used.
 * 03/12/2018      RC          3.4.5      Fixed Data Complete event handler settings.
 * 06/15/2018      RC          3.4.7      Add VelocityVector to ensemble.
 * 05/21/2019      RC          3.4.11     In FindCompleteEnsembles(), generate a Subsystem Config so the subsystems configurations are seperate.
 * 
 */

using System;
using System.Diagnostics;
using System.Threading.Tasks;


namespace RTI
{
    /// <summary>
    /// Use this codec will determine which codec to use.
    /// This will use all the available codecs to determine which codec
    /// is in use and decode the data.
    /// </summary>
    public class AdcpCodec: IDisposable
    {
        #region Enum

        /// <summary>
        /// Enum of all the codecs.
        /// </summary>
        public enum CodecEnum
        {
            /// <summary>
            /// Binary codec.
            /// </summary>
            Binary,

            /// <summary>
            /// DVL codec.
            /// </summary>
            DVL,

            /// <summary>
            /// PD0 codec.
            /// </summary>
            PD0,

            /// <summary>
            /// PD6 PD13 codec.
            /// </summary>
            PD6_13,

            /// <summary>
            /// PD4 PD5 codec.
            /// </summary>
            PD4_5
        }

        #endregion

        #region Variables

        /// <summary>
        ///  Setup logger
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Binary Codec for binary files.
        /// </summary>
        private AdcpBinaryCodecNew _binaryCodec;

        /// <summary>
        /// Decode the DVL ADCP data into
        /// an ensemble.
        /// </summary>
        private AdcpDvlCodec _dvlCodec;

        /// <summary>
        /// Decode the PD0 data into an
        /// ensemble.
        /// </summary>
        private Pd0Codec _pd0Codec;

        /// <summary>
        /// Decode the PD6 and PD13 data into an 
        /// ensemble.
        /// </summary>
        private Pd6_13Codec _pd6_13Codec;

        /// <summary>
        /// Decode the PD4 and PD5 data into an 
        /// ensemble.
        /// </summary>
        private PD4_5Codec _pd4_5Codec;

        /// <summary>
        /// PD0 Subsystem generator.
        /// </summary>
        private Pd0SubsystemGen _pd0SubsystemGen;

        #region Decode Counters

        /// <summary>
        /// Count the number of Binary ensembles found.
        /// </summary>
        private int _binaryCounter = 0;

        /// <summary>
        /// Count the number of Binary ensembles found.
        /// </summary>
        private int _dvlCounter = 0;

        /// <summary>
        /// Count the number of Binary ensembles found.
        /// </summary>
        private int _pd0Counter = 0;

        /// <summary>
        /// Count the number of Binary ensembles found.
        /// </summary>
        private int _pd6_13Counter = 0;

        /// <summary>
        /// Count the number of Binary ensembles found.
        /// </summary>
        private int _pd4_5Counter = 0; 

        #endregion

        #endregion

        /// <summary>
        /// Initialize all the codecs.
        /// </summary>
        public AdcpCodec()
        {
            // Counters
            _binaryCounter = 0;
            _dvlCounter = 0;
            _pd0Counter = 0;
            _pd6_13Counter = 0;
            _pd4_5Counter = 0;

            //PD0 Subsystem Generator
            _pd0SubsystemGen = new Pd0SubsystemGen();

            // Binary Codecs
            _binaryCodec = new AdcpBinaryCodecNew();
            _binaryCodec.ProcessDataEvent += new AdcpBinaryCodecNew.ProcessDataEventHandler(_binaryCodec_ProcessDataEvent);
            _binaryCodec.ProcessDataCompleteEvent += binaryCodec_ProcessDataCompleteEvent;
            _binaryCodec.GoodEnsembleEvent += ReceivedGoodEnsembleEvent;
            _binaryCodec.BadEnsembleEvent += ReceivedBadEnsembleEvent;

            // DVL Codec
            _dvlCodec = new AdcpDvlCodec();
            _dvlCodec.ProcessDataEvent += new AdcpDvlCodec.ProcessDataEventHandler(_dvlCodec_ProcessDataEvent);
            _dvlCodec.ProcessDataCompleteEvent += dvlCodec_ProcessDataCompleteEvent;

            // PD0 Codec
            _pd0Codec = new Pd0Codec();
            _pd0Codec.ProcessDataEvent += new Pd0Codec.ProcessDataEventHandler(_pd0Codec_ProcessDataEvent);
            _pd0Codec.ProcessDataCompleteEvent += pd0Codec_ProcessDataCompleteEvent;
            _pd0Codec.GoodEnsembleEvent += ReceivedGoodEnsembleEvent;
            _pd0Codec.BadEnsembleEvent += ReceivedBadEnsembleEvent;

            // PD6 and PD13 Codec
            _pd6_13Codec = new Pd6_13Codec();
            _pd6_13Codec.ProcessDataEvent += new Pd6_13Codec.ProcessDataEventHandler(_pd6_13Codec_ProcessDataEvent);
            _pd6_13Codec.ProcessDataCompleteEvent += pd6_13Codec_ProcessDataCompleteEvent;

            // PD4 and PD5 Codec
            _pd4_5Codec = new PD4_5Codec();
            _pd4_5Codec.ProcessDataEvent += new PD4_5Codec.ProcessDataEventHandler(_pd4_5Codec_ProcessDataEvent);
            _pd4_5Codec.ProcessDataCompleteEvent += pd4_5Codec_ProcessDataCompleteEvent;
        }

        /// <summary>
        /// Dispose of the object.
        /// </summary>
        public void Dispose()
        {
            // Clear all the data
            //ClearIncomingData();

            // Shutdown binary codec
            if (_binaryCodec != null)
            {
                _binaryCodec.ProcessDataEvent -= _binaryCodec_ProcessDataEvent;
                _binaryCodec.ProcessDataCompleteEvent -= binaryCodec_ProcessDataCompleteEvent;
                _binaryCodec.Dispose();
            }

            // Shutdown DVL codec
            if (_dvlCodec != null)
            {
                _dvlCodec.ProcessDataEvent -= _dvlCodec_ProcessDataEvent;
                _dvlCodec.ProcessDataCompleteEvent -= dvlCodec_ProcessDataCompleteEvent;
                _dvlCodec.Dispose();
            }

            // Shutdown PD0 codec
            if (_pd0Codec != null)
            {
                _pd0Codec.ProcessDataEvent -= _pd0Codec_ProcessDataEvent;
                _pd0Codec.ProcessDataCompleteEvent -= pd0Codec_ProcessDataCompleteEvent;
                _pd0Codec.Dispose();
            }

            // Shutdown PD6 and PD13 codec
            if (_pd6_13Codec != null)
            {
                _pd6_13Codec.ProcessDataEvent -= _pd6_13Codec_ProcessDataEvent;
                _pd6_13Codec.ProcessDataCompleteEvent -= pd6_13Codec_ProcessDataCompleteEvent;
                _pd6_13Codec.Dispose();
            }

            // Shutdown PD4 and PD5 codec
            if (_pd4_5Codec != null)
            {
                _pd4_5Codec.ProcessDataEvent -= _pd4_5Codec_ProcessDataEvent;
                _pd4_5Codec.ProcessDataCompleteEvent -= pd4_5Codec_ProcessDataCompleteEvent;
                _pd4_5Codec.Dispose();
            }
        }

        #region Data

        /// <summary>
        /// Add the incoming data.
        /// </summary>
        /// <param name="data">Data to add.</param>
        /// <param name="isBinaryData">Flag if Binary data should be decoded.</param>
        /// <param name="isDvlData">Flag if DVL data should be decoded.</param>
        /// <param name="isPd0Data">Flag if PD0 data should be decoded.</param>
        /// <param name="isPd6_13Data">Flag if PD6/PD13 data should be decoded.</param>
        /// <param name="isPd4_5Data">Flag if PD4/PD5 data should be decoded.</param>
        public void AddIncomingData(byte[] data, bool isBinaryData = true, bool isDvlData = true, bool isPd0Data = true, bool isPd6_13Data = true, bool isPd4_5Data = true)
        {
            // Binary data
            if (isBinaryData)
            {
                try
                {
                    _binaryCodec.AddIncomingData(data);
                }
                catch (Exception e)
                {
                    log.Error("Error decoding Binary data.", e);
                }
            }
            else
            {
                // Clear buffer, format not needed
                _binaryCodec.ClearIncomingData();
            }

            // DVL data
            if (isDvlData)
            {
                try
                {
                    _dvlCodec.AddIncomingData(data);
                }
                catch (Exception e)
                {
                    log.Error("Error decoding the DVL data.", e);
                }
            }
            else
            {
                // Clear buffer, format not needed
                _dvlCodec.ClearIncomingData();
            }

            // PD0 data
            if (isPd0Data)
            {
                try
                {
                    _pd0Codec.AddIncomingData(data);
                }
                catch (Exception e)
                {
                    log.Error("Error decoding the PD0 data.", e);
                }
            }
            else
            {
                // Clear buffer, format not needed
                _pd0Codec.ClearIncomingData();
            }

            // PD6 / PD13 data
            if (isPd6_13Data)
            {
                try
                {
                    _pd6_13Codec.AddIncomingData(data);
                }
                catch (Exception e)
                {
                    log.Error("Error decoding PD6/13 data.", e);
                }
            }
            else
            {
                // Clear buffer, format not needed
                _pd6_13Codec.ClearIncomingData();
            }

            // PD4 / PD5 data
            if (isPd4_5Data)
            {
                try
                {
                    _pd4_5Codec.AddIncomingData(data);
                }
                catch (Exception e)
                {
                    log.Error("Error decoding PD4/5 data.", e);
                }
            }
            else
            {
                // Clear buffer, format not needed
                _pd4_5Codec.ClearIncomingData();
            }
        }

        /// <summary>
        /// Store the latest NMEA data to a buffer.
        /// As new ensembles are record, the buffer will
        /// be flushed.
        /// </summary>
        /// <param name="nmeaData">String of NMEA data.  Can be multiple lines.</param>
        public void AddNmeaData(string nmeaData)
        {
            _binaryCodec.AddNmeaData(nmeaData);
            _dvlCodec.AddNmeaData(nmeaData);
        }

        /// <summary>
        /// Clear the incoming data.
        /// </summary>
        public void ClearIncomingData()
        {
            // Clear counter
            _binaryCounter = 0;
            _dvlCounter = 0;
            _pd0Counter = 0;
            _pd6_13Counter = 0;
            _pd4_5Counter = 0;

            // Clear the codec buffers
            _binaryCodec.ClearIncomingData();
            _dvlCodec.ClearIncomingData();
            _pd0Codec.ClearIncomingData();
            _pd6_13Codec.ClearIncomingData();
            _pd4_5Codec.ClearIncomingData();
        }

        #endregion

        #region Check Codec Buffers

        /// <summary>
        /// Check if we should clear the codec based off the
        /// number of ensembles found for each codec.
        /// </summary>
        /// <param name="codec">Codec that received an ensemble.</param>
        private void CheckCodecBuffers(CodecEnum codec)
        {
            //switch(codec)
            //{
            //    case CodecEnum.Binary:
            //        break;
            //    case CodecEnum.DVL:
            //        break;
            //    case CodecEnum.PD0:
            //        break;
            //    case CodecEnum.PD4_5:
            //        break;
            //    case CodecEnum.PD6_13:
            //        break;
            //    default:
            //        break;
            //}

            // Clear the counters to not have a buffer overflow
            //if(_binaryCounter <= 0)
            //{
            //    _binaryCodec.ClearIncomingData();
            //}

            if(_dvlCounter <= 0)
            {
                _dvlCodec.ClearIncomingData();
            }

            if (_pd0Counter <= 0)
            {
                _pd0Codec.ClearIncomingData();
            }

            if (_pd6_13Counter <= 0)
            {
                _pd6_13Codec.ClearIncomingData();
            }

            if (_pd4_5Counter <= 0)
            {
                _pd4_5Codec.ClearIncomingData();
            }

            // Look to find if we have a dominate codec.
            // If we find a dominate codec, clear the other
            // codecs.  
            int MAX_ENS_COUNT = 20;

            if(_binaryCounter > MAX_ENS_COUNT)
            {
                _dvlCodec.ClearIncomingData();
                _pd0Codec.ClearIncomingData();
                _pd6_13Codec.ClearIncomingData();
                _pd4_5Codec.ClearIncomingData();

                // Clear counter
                _binaryCounter = 1;
                _dvlCounter = 0;
                _pd0Counter = 0;
                _pd6_13Counter = 0;
                _pd4_5Counter = 0;
            }

            if(_dvlCounter > MAX_ENS_COUNT)
            {
                _binaryCodec.ClearIncomingData();
                _pd0Codec.ClearIncomingData();
                _pd6_13Codec.ClearIncomingData();
                _pd4_5Codec.ClearIncomingData();

                // Clear counter
                _binaryCounter = 0;
                _dvlCounter = 1;
                _pd0Counter = 0;
                _pd6_13Counter = 0;
                _pd4_5Counter = 0;
            }

            if (_pd0Counter > MAX_ENS_COUNT)
            {
                _binaryCodec.ClearIncomingData();
                _dvlCodec.ClearIncomingData();
                _pd6_13Codec.ClearIncomingData();
                _pd4_5Codec.ClearIncomingData();

                // Clear counter
                _binaryCounter = 0;
                _dvlCounter = 0;
                _pd0Counter = 1;
                _pd6_13Counter = 0;
                _pd4_5Counter = 0;
            }

            if (_pd6_13Counter > MAX_ENS_COUNT)
            {
                _binaryCodec.ClearIncomingData();
                _dvlCodec.ClearIncomingData();
                _pd0Codec.ClearIncomingData();
                _pd4_5Codec.ClearIncomingData();

                // Clear counter
                _binaryCounter = 0;
                _dvlCounter = 0;
                _pd0Counter = 0;
                _pd6_13Counter = 1;
                _pd4_5Counter = 0;
            }

            if(_pd4_5Counter > MAX_ENS_COUNT)
            {
                _binaryCodec.ClearIncomingData();
                _dvlCodec.ClearIncomingData();
                _pd0Codec.ClearIncomingData();
                _pd6_13Codec.ClearIncomingData();

                // Clear counter
                _binaryCounter = 0;
                _dvlCounter = 0;
                _pd0Counter = 0;
                _pd6_13Counter = 0;
                _pd4_5Counter = 1;
            }

        }

        #endregion

        #region EventHandler

        /// <summary>
        /// Receive decoded data from the Binary codec.  This will be 
        /// the latest data decoded.  It will include the complete
        /// binary array of the data and the ensemble object.
        /// </summary>
        /// <param name="binaryEnsemble">Binary data of the ensemble.</param>
        /// <param name="ensemble">Ensemble object.</param>
        void _binaryCodec_ProcessDataEvent(byte[] binaryEnsemble, DataSet.Ensemble ensemble)
        {
            // Create the velocity vector
            DataSet.VelocityVectorHelper.CreateVelocityVector(ref ensemble);

            if (ProcessDataEvent != null)
            {
                // Pass the data to the subscribers
                ProcessDataEvent(binaryEnsemble, ensemble, CodecEnum.Binary);
            }

            _binaryCounter++;

            // Check which buffers to clear
            //CheckCodecBuffers(CodecEnum.Binary);
        }

        /// <summary>
        /// Binary codec received a complete event so pass to all subscribers.
        /// </summary>
        void binaryCodec_ProcessDataCompleteEvent()
        {
            if (_binaryCounter >= 1)
            {
                if (ProcessDataCompleteEvent != null)
                {
                    // Pass the data to the subscribers
                    ProcessDataCompleteEvent();
                }
            }
        }

        /// <summary>
        /// Receive decoded data from the DVL codec.  This will be 
        /// the latest data decoded.  It will include the complete
        /// binary array of the data and the ensemble object.
        /// </summary>
        /// <param name="binaryEnsemble">Binary data of the ensemble.</param>
        /// <param name="ensemble">Ensemble object.</param>
        void _dvlCodec_ProcessDataEvent(byte[] binaryEnsemble, DataSet.Ensemble ensemble)
        {
            // Create the velocity vector
            DataSet.VelocityVectorHelper.CreateVelocityVector(ref ensemble);

            if (ProcessDataEvent != null)
            {
                // Pass the data to the subscribers
                ProcessDataEvent(binaryEnsemble, ensemble, CodecEnum.DVL);
            }

            _dvlCounter++;

            // Check which buffers to clear
            //CheckCodecBuffers(CodecEnum.DVL);
        }

        /// <summary>
        /// DVL codec received a complete event so pass to all subscribers.
        /// </summary>
        void dvlCodec_ProcessDataCompleteEvent()
        {
            if (_dvlCounter > 1)
            {
                if (ProcessDataCompleteEvent != null)
                {
                    // Pass the data to the subscribers
                    ProcessDataCompleteEvent();
                }
            }
        }

        /// <summary>
        /// Receive decoded data from the PD0 codec.  This will be 
        /// the latest data decoded.  It will include the complete
        /// binary array of the data and the ensemble object.
        /// </summary>
        /// <param name="binaryEnsemble">Binary data of the ensemble.</param>
        /// <param name="ensemble">Ensemble object.</param>
        void _pd0Codec_ProcessDataEvent(byte[] binaryEnsemble, PD0 ensemble)
        {
            // Convert to a RTI ensemble
            DataSet.Ensemble rtiEns = new DataSet.Ensemble(ensemble);
            DataSet.VelocityVectorHelper.CreateVelocityVector(ref rtiEns);

            // Generate a subsystem so that multiple configurations can be seprated
            // PD0 does not contain the CEPO index or CEPO Configuraiton Index
            if(rtiEns.IsEnsembleAvail)
            {
                rtiEns.EnsembleData.SubsystemConfig = _pd0SubsystemGen.GenSubsystem(rtiEns);
            }

            if (ProcessDataEvent != null)
            {
                // Pass the data to the subscribers
                ProcessDataEvent(binaryEnsemble, rtiEns, CodecEnum.PD0);
            }

            _pd0Counter++;

            // Check which buffers to clear
            //CheckCodecBuffers(CodecEnum.PD0);
        }

        /// <summary>
        /// PD0 codec received a complete event so pass to all subscribers.
        /// </summary>
        void pd0Codec_ProcessDataCompleteEvent()
        {
            if (_pd0Counter > 1)
            {
                if (ProcessDataCompleteEvent != null)
                {
                    // Pass the data to the subscribers
                    ProcessDataCompleteEvent();
                }
            }
        }

        /// <summary>
        /// Receive decoded data from the PD6 and PD13 codec.  This will be 
        /// the latest data decoded.  It will include the complete
        /// binary array of the data and the ensemble object.
        /// </summary>
        /// <param name="binaryEnsemble">PD6 and PD13 data of the ensemble.</param>
        /// <param name="ensemble">Ensemble object.</param>
        void _pd6_13Codec_ProcessDataEvent(byte[] binaryEnsemble, DataSet.Ensemble ensemble)
        {
            // Create the velocity vector
            DataSet.VelocityVectorHelper.CreateVelocityVector(ref ensemble);

            if (ProcessDataEvent != null)
            {
                // Pass the data to the subscribers
                ProcessDataEvent(binaryEnsemble, ensemble, CodecEnum.PD6_13);
            }

            _pd6_13Counter++;

            // Check which buffers to clear
            //CheckCodecBuffers(CodecEnum.PD6_13);
        }

        /// <summary>
        /// PD6/13 codec received a complete event so pass to all subscribers.
        /// </summary>
        void pd6_13Codec_ProcessDataCompleteEvent()
        {
            if (_pd6_13Counter > 1)
            {
                if (ProcessDataCompleteEvent != null)
                {
                    // Pass the data to the subscribers
                    ProcessDataCompleteEvent();
                }
            }
        }

        /// <summary>
        /// Receive decoded data from the PD4 and PD5 codec.  This will be 
        /// the latest data decoded.  It will include the complete
        /// binary array of the data and the ensemble object.
        /// </summary>
        /// <param name="binaryEnsemble">Binary PD4 and PD5 data of the ensemble.</param>
        /// <param name="ensemble">Ensemble object.</param>
        void _pd4_5Codec_ProcessDataEvent(byte[] binaryEnsemble, DataSet.Ensemble ensemble)
        {
            // Create the velocity vector
            DataSet.VelocityVectorHelper.CreateVelocityVector(ref ensemble);

            if (ProcessDataEvent != null)
            {
                // Pass the data to the subscribers
                ProcessDataEvent(binaryEnsemble, ensemble, CodecEnum.PD4_5);
            }

            _pd4_5Counter++;

            // Check which buffers to clear
            //CheckCodecBuffers(CodecEnum.PD4_5);
        }

        /// <summary>
        /// PD4/PD5 codec received a complete event so pass to all subscribers.
        /// </summary>
        void pd4_5Codec_ProcessDataCompleteEvent()
        {
            if (_pd4_5Counter > 1)
            {
                if (ProcessDataCompleteEvent != null)
                {
                    // Pass the data to the subscribers
                    ProcessDataCompleteEvent();
                }
            }
        }

        /// <summary>
        /// Good Ensemble found.  This event is called before the
        /// ensemble is parsed for all its data.  This means that 
        /// a header and good checksum were found.
        /// </summary>
        private void ReceivedGoodEnsembleEvent()
        {
            if(GoodEnsembleEvent != null)
            {
                GoodEnsembleEvent();
            }
        }

        /// <summary>
        /// Received a Bad Ensemble event.
        /// This typically means the header is found, but the
        /// checksum failed.
        /// </summary>
        private void ReceivedBadEnsembleEvent()
        {
            if (BadEnsembleEvent != null)
            {
                BadEnsembleEvent();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event To subscribe to.  This gives the paramater
        /// that will be passed when subscribing to the event.
        /// </summary>
        /// <param name="binaryEnsemble">Byte array of raw ensemble data.</param>
        /// <param name="ensemble">Ensemble data as an object.</param>
        /// <param name="dataFormat">Format of the data that was received.</param>
        public delegate void ProcessDataEventHandler(byte[] binaryEnsemble, DataSet.Ensemble ensemble, CodecEnum dataFormat);

        /// <summary>
        /// Subscribe to receive event when data has been successfully
        /// processed.  This can be used to tell if data is in this format
        /// and is being processed or is not in this format.
        /// Subscribe to this event.  This will hold all subscribers.
        /// 
        /// To subscribe:
        /// adcpCodec.ProcessDataEvent += new adcpCodec.ProcessDataEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// adcpCodec.ProcessDataEvent -= (method to call)
        /// </summary>
        public event ProcessDataEventHandler ProcessDataEvent;

        /// <summary>
        /// Event To subscribe to.  This gives the paramater
        /// that will be passed when subscribing to the event.
        /// </summary>
        public delegate void ProcessDataCompleteEventHandler();

        /// <summary>
        /// Subscribe to know when the entire file has been processed.
        /// This event will be fired when there is no more data in the 
        /// buffer to decode.
        /// 
        /// To subscribe:
        /// adcpBinaryCodec.ProcessDataCompleteEvent += new adcpBinaryCodec.ProcessDataCompleteEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// adcpBinaryCodec.ProcessDataCompleteEvent -= (method to call)
        /// </summary>
        public event ProcessDataCompleteEventHandler ProcessDataCompleteEvent;

        /// <summary>
        /// Event To subscribe to.  This gives the paramater
        /// that will be passed when subscribing to the event.
        /// </summary>
        public delegate void GoodEnsembleEventHandler();

        /// <summary>
        /// Subscribe to know when a good ensemble has been found.
        /// 
        /// To subscribe:
        /// codec.GoodEnsembleEvent += new codec.GoodEnsembleEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// codec.GoodEnsembleEvent -= (method to call)
        /// </summary>
        public event GoodEnsembleEventHandler GoodEnsembleEvent;

        /// <summary>
        /// Event To subscribe to.  This gives the paramater
        /// that will be passed when subscribing to the event.
        /// </summary>
        public delegate void BadEnsembleEventHandler();

        /// <summary>
        /// Subscribe to know when a bad ensemble has been found
        /// 
        /// To subscribe:
        /// codec.BadEnsembleEvent += new codec.BadEnsembleEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// codec.BadEnsembleEvent -= (method to call)
        /// </summary>
        public event BadEnsembleEventHandler BadEnsembleEvent;

        #endregion
    }
}
