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
 * 
 * HISTORY
 * -----------------------------------------------------------------
 * Date            Initials    Version    Comments
 * -----------------------------------------------------------------
 * 09/12/2014      RC          3.0.1      Initial coding
 * 
 */ 

using System;

namespace RTI
{
    /// <summary>
    /// Use this codec will determine which codec to use.
    /// This will use all the available codecs to determine which codec
    /// is in use and decode the data.
    /// </summary>
    public class AdcpCodec: ICodec, IDisposable
    {
        #region Variables

        /// <summary>
        /// Binary Codec for binary files.
        /// </summary>
        private AdcpBinaryCodec _binaryCodec;

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

        #endregion

        /// <summary>
        /// Initialize all the codecs.
        /// </summary>
        public AdcpCodec()
        {
            // Codecs
            _binaryCodec = new AdcpBinaryCodec();
            _binaryCodec.ProcessDataEvent += new AdcpBinaryCodec.ProcessDataEventHandler(_binaryCodec_ProcessDataEvent);
            _binaryCodec.ProcessDataCompleteEvent += _binaryCodec_ProcessDataCompleteEvent;

            // DVL Codec
            _dvlCodec = new AdcpDvlCodec();
            _dvlCodec.ProcessDataEvent += new AdcpDvlCodec.ProcessDataEventHandler(_dvlCodec_ProcessDataEvent);

            // PD0 Codec
            _pd0Codec = new Pd0Codec();
            _pd0Codec.ProcessDataEvent += new Pd0Codec.ProcessDataEventHandler(_pd0Codec_ProcessDataEvent);

            // PD6 and PD13 Codec
            _pd6_13Codec = new Pd6_13Codec();
            _pd6_13Codec.ProcessDataEvent += new Pd6_13Codec.ProcessDataEventHandler(_pd6_13Codec_ProcessDataEvent);

            // PD4 and PD5 Codec
            _pd4_5Codec = new PD4_5Codec();
            _pd4_5Codec.ProcessDataEvent += new PD4_5Codec.ProcessDataEventHandler(_pd4_5Codec_ProcessDataEvent);
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
                _binaryCodec.ProcessDataCompleteEvent -= _binaryCodec_ProcessDataCompleteEvent;
                _binaryCodec.Dispose();
            }

            // Shutdown DVL codec
            if (_dvlCodec != null)
            {
                _dvlCodec.ProcessDataEvent -= _dvlCodec_ProcessDataEvent;
                _dvlCodec.Dispose();
            }

            // Shutdown PD0 codec
            if (_pd0Codec != null)
            {
                _pd0Codec.ProcessDataEvent -= _pd0Codec_ProcessDataEvent;
                _pd0Codec.Dispose();
            }

            // Shutdown PD6 and PD13 codec
            if (_pd6_13Codec != null)
            {
                _pd6_13Codec.ProcessDataEvent -= _pd6_13Codec_ProcessDataEvent;
                _pd6_13Codec.Dispose();
            }

            // Shutdown PD4 and PD5 codec
            if (_pd4_5Codec != null)
            {
                _pd4_5Codec.ProcessDataEvent -= _pd4_5Codec_ProcessDataEvent;
                _pd4_5Codec.Dispose();
            }
        }

        #region Data

        /// <summary>
        /// Add the incoming data.
        /// </summary>
        /// <param name="data">Data to add.</param>
        public void AddIncomingData(byte[] data)
        {
            _binaryCodec.AddIncomingData(data);
            _dvlCodec.AddIncomingData(data);
            _pd0Codec.AddIncomingData(data);
            _pd6_13Codec.AddIncomingData(data);
            _pd4_5Codec.AddIncomingData(data);
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
            _binaryCodec.ClearIncomingData();
            _dvlCodec.ClearIncomingData();
            _pd0Codec.ClearIncomingData();
            _pd6_13Codec.ClearIncomingData();
            _pd4_5Codec.ClearIncomingData();
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
            if (ProcessDataEvent != null)
            {
                // Pass the data to the subscribers
                ProcessDataEvent(binaryEnsemble, ensemble);
            }

            // Since the data is in binary format
            // clear the other buffers so they don't overflow
            _dvlCodec.ClearIncomingData();
            _pd0Codec.ClearIncomingData();
            _pd6_13Codec.ClearIncomingData();
            _pd4_5Codec.ClearIncomingData();
        }

        /// <summary>
        /// Binary codec received a complete event so pass to all subscribers.
        /// </summary>
        void _binaryCodec_ProcessDataCompleteEvent()
        {
            if (ProcessDataCompleteEvent != null)
            {
                // Pass the data to the subscribers
                ProcessDataCompleteEvent();
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
            if (ProcessDataEvent != null)
            {
                // Pass the data to the subscribers
                ProcessDataEvent(binaryEnsemble, ensemble);
            }

            // Since the data is in binary format
            // clear the other buffers so they don't overflow
            _binaryCodec.ClearIncomingData();
            _pd0Codec.ClearIncomingData();
            _pd6_13Codec.ClearIncomingData();
            _pd4_5Codec.ClearIncomingData();
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

            if (ProcessDataEvent != null)
            {
                // Pass the data to the subscribers
                ProcessDataEvent(binaryEnsemble, rtiEns);
            }

            // Since the data is in binary format
            // clear the other buffers so they don't overflow
            _binaryCodec.ClearIncomingData();
            _dvlCodec.ClearIncomingData();
            _pd6_13Codec.ClearIncomingData();
            _pd4_5Codec.ClearIncomingData();
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
            if (ProcessDataEvent != null)
            {
                // Pass the data to the subscribers
                ProcessDataEvent(binaryEnsemble, ensemble);
            }

            // Since the data is in binary format
            // clear the other buffers so they don't overflow
            _binaryCodec.ClearIncomingData();
            _dvlCodec.ClearIncomingData();
            _pd0Codec.ClearIncomingData();
            _pd4_5Codec.ClearIncomingData();
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
            if (ProcessDataEvent != null)
            {
                // Pass the data to the subscribers
                ProcessDataEvent(binaryEnsemble, ensemble);
            }

            // Since the data is in binary format
            // clear the other buffers so they don't overflow
            _binaryCodec.ClearIncomingData();
            _dvlCodec.ClearIncomingData();
            _pd0Codec.ClearIncomingData();
            _pd6_13Codec.ClearIncomingData();
        }

        #endregion

        #region Events

        /// <summary>
        /// Event To subscribe to.  This gives the paramater
        /// that will be passed when subscribing to the event.
        /// </summary>
        /// <param name="binaryEnsemble">Byte array of raw ensemble data.</param>
        /// <param name="ensemble">Ensemble data as an object.</param>
        public delegate void ProcessDataEventHandler(byte[] binaryEnsemble, DataSet.Ensemble ensemble);

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

        #endregion
    }
}
