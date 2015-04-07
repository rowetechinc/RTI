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
 * 09/02/2014      RC          3.0.1      Initial coding
 * 09/12/2014      RC          3.0.1      Allow multiple files to be selected to import.
 * 11/04/2014      RC          3.0.2      Changed FindEnsembles() to load the file in one block.
 * 01/08/2015      RC          3.0.2      Fixed loading the files to wait for all the data to be decoded.
 * 01/16/2015      RC          3.0.2      Close the ProcessDataCompleteEvent in Dispose.
 * 01/29/2015      RC          3.0.2      Fixed waiting for the file to be loaded into the codec before moving on.
 * 02/09/2015      RC          3.0.2      Add constructor that takes only a single file.
 *       
 * 
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RTI
{
    /// <summary>
    /// Playback the given file.  This file can be in any ADCP format.
    /// The file will be decoded then passed as an ensemble.
    /// </summary>
    public class FilePlayback: IPlayback
    {
        #region Variables

        /// <summary>
        /// Store the raw binary data and the ensemble data.
        /// </summary>
        public class EnsembleData
        {
            /// <summary>
            /// Raw binary data for the ensemble.
            /// </summary>
            public byte[] RawData { get; set; }

            /// <summary>
            /// Ensemble data.
            /// </summary>
            public DataSet.Ensemble Ensemble { get; set; }

            /// <summary>
            /// Initialize the value.
            /// </summary>
            /// <param name="rawData">Raw binary data.</param>
            /// <param name="ensemble">Ensemble data.</param>
            public EnsembleData(byte[] rawData, DataSet.Ensemble ensemble)
            {
                RawData = rawData;
                Ensemble = ensemble;
            }
        }

        /// <summary>
        /// Default number of bytes for an ensemble.
        /// This value will be the start value and then 
        /// determined based off the number found from the next 
        /// ensemble found.
        /// </summary>
        private const int DEFAULT_BYTES_PER_ENSEMBLE = 1028 * 10; //4096 1048576

        /// <summary>
        /// File stream to read in the file.
        /// </summary>
        private FileStream _fileStream;

        /// <summary>
        /// Size of the file.
        /// </summary>
        private long _fileSize;

        /// <summary>
        /// Offset within the file.
        /// </summary>
        private int _fileOffset;

        /// <summary>
        /// Binary Codec for binary files.
        /// </summary>
        private AdcpCodec _adcpCodec;

        /// <summary>
        /// Lock for the _ensembleFound lock.
        /// </summary>
        private object _ensembleFoundLock = new object();

        /// <summary>
        /// List of ensembles found.
        /// </summary>
        private List<EnsembleData> _ensembleList;

        /// <summary>
        /// Event to cause the thread
        /// to go to sleep or wakeup.
        /// </summary>
        private EventWaitHandle _eventWaitDecode;

        #endregion

        #region Properties

        /// <summary>
        /// The number of bytes per ensemble.
        /// </summary>
        public int BytesPerEnsemble { get; set; }

        /// <summary>
        /// Total number of ensembles that can be played back.
        /// </summary>
        public int TotalEnsembles { get; set; }

        /// <summary>
        /// Playback Index.
        /// </summary>
        public long PlaybackIndex { get; set; }

        /// <summary>
        /// Set flag is playing back in a loop.
        /// </summary>
        public bool IsLooping { get; set; }

        #endregion

        /// <summary>
        /// Playback the given file.
        /// </summary>
        /// <param name="files">Files to playback.</param>
        public FilePlayback(string[] files)
        {
            // Initialize values
            BytesPerEnsemble = DEFAULT_BYTES_PER_ENSEMBLE;
            _fileOffset = 0;
            TotalEnsembles = 0;
            _ensembleList = new List<EnsembleData>();

            // Codecs
            _adcpCodec = new AdcpCodec();
            _adcpCodec.ProcessDataEvent += new AdcpCodec.ProcessDataEventHandler(_adcpCodec_ProcessDataEvent);
            _adcpCodec.ProcessDataCompleteEvent += _adcpCodec_ProcessDataCompleteEvent;

            // Wait for decoding to be complete
            _eventWaitDecode = new EventWaitHandle(false, EventResetMode.AutoReset);

            // Add all the files
            foreach (var filePath in files)
            {
                // Open the file
                if (File.Exists(filePath))
                {
                    using (_fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        _fileSize = _fileStream.Length;

                        // Find the ensembles in the file
                        FindEnsembles();
                    }
                }
            }

            // Block until awoken when all the data is decoded
            // Have a timeout of 60 seconds
            _eventWaitDecode.WaitOne(60000);
        }

        /// <summary>
        /// Playback the given file.
        /// </summary>
        /// <param name="file">File to playback.</param>
        public FilePlayback(string file)
        {
            // Initialize values
            BytesPerEnsemble = DEFAULT_BYTES_PER_ENSEMBLE;
            _fileOffset = 0;
            TotalEnsembles = 0;
            _ensembleList = new List<EnsembleData>();

            // Codecs
            _adcpCodec = new AdcpCodec();
            _adcpCodec.ProcessDataEvent += new AdcpCodec.ProcessDataEventHandler(_adcpCodec_ProcessDataEvent);
            _adcpCodec.ProcessDataCompleteEvent += _adcpCodec_ProcessDataCompleteEvent;

            // Wait for decoding to be complete
            _eventWaitDecode = new EventWaitHandle(false, EventResetMode.AutoReset);

                // Open the file
            if (File.Exists(file))
            {
                using (_fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    _fileSize = _fileStream.Length;

                    // Find the ensembles in the file
                    FindEnsembles();
                }
            }

            // Block until awoken when all the data is decoded
            // Have a timeout of 60 seconds
            _eventWaitDecode.WaitOne(60000);
        }

        /// <summary>
        /// Dispose of the object.
        /// </summary>
        public void Dispose()
        {
            if (_fileStream != null)
            {
                _fileStream.Close();
                _fileStream.Dispose();
            }

            if (_adcpCodec != null)
            {
                _adcpCodec.ProcessDataCompleteEvent -= _adcpCodec_ProcessDataCompleteEvent;
                _adcpCodec.ProcessDataEvent -= _adcpCodec_ProcessDataEvent;
                _adcpCodec.Dispose();
            }

            if (_eventWaitDecode != null)
            {
                _eventWaitDecode.Set();
                _eventWaitDecode.Dispose();
            }
        }

        /// <summary>
        /// Set forward in the file.
        /// </summary>
        /// <returns>Next ensemble in the file.</returns>
        public PlaybackArgs StepForward()
        {
            // Create the args and set the total number of ensembles
            PlaybackArgs args = new PlaybackArgs();

            // Increment the playback index
            PlaybackIndex++;

            if (PlaybackIndex < _ensembleList.Count)
            {
                // Set the values
                args.Index = PlaybackIndex;
                args.Ensemble = _ensembleList[(int)PlaybackIndex].Ensemble;
                args.TotalEnsembles = TotalEnsembles;
            }
            else
            {
                return null;
            }

            return args;
        }

        /// <summary>
        /// Step backwards in the file.
        /// </summary>
        /// <returns>Previous ensemble in the file.</returns>
        public PlaybackArgs StepBackward()
        {
            // Verify the playback index
            if(PlaybackIndex - 1 < 0)
            {
                return null;
            }

            // Create the args and set the total number of ensembles
            PlaybackArgs args = new PlaybackArgs();

            if ( PlaybackIndex < _ensembleList.Count)
            {
                // Set the values
                args.Index = PlaybackIndex--;
                args.Ensemble = _ensembleList[(int)PlaybackIndex].Ensemble;
                args.TotalEnsembles = TotalEnsembles;
            }

            return args;
        }

        /// <summary>
        /// Jump to a specific location in the file to get the indexed ensemble.
        /// If the index is outside the range, then null will be returned.
        /// </summary>
        /// <param name="index">Index to jump to in the file.</param>
        /// <returns>Ensemble based off the index given.</returns>
        public PlaybackArgs Jump(long index)
        {
            // Create the args and set the total number of ensembles
            PlaybackArgs args = new PlaybackArgs();

            if (index < _ensembleList.Count)
            {
                // Set new index
                PlaybackIndex = index;

                // Set the values
                args.Index = index;
                args.Ensemble = _ensembleList[(int)PlaybackIndex].Ensemble;
                args.TotalEnsembles = TotalEnsembles;
            }

            return args;
        }

        /// <summary>
        /// Return a list of all the ensembles.
        /// </summary>
        /// <returns>List of all the ensembles in the file.</returns>
        public Cache<long, DataSet.Ensemble> GetAllEnsembles()
        {
            Cache<long, DataSet.Ensemble> list = new Cache<long, DataSet.Ensemble>((uint)_ensembleList.Count);

            // Populate the cache with all the ensembles
            for (int x = 0; x < _ensembleList.Count; x++ )
            {
                list.Add(x, _ensembleList[x].Ensemble);
            }

            return list;
        }

        /// <summary>
        /// Return a list of all the ensembles.
        /// </summary>
        /// <returns>List of all the ensembles in the file.</returns>
        public List<EnsembleData> GetEnsembleDataList()
        {
            return _ensembleList;
        }

        /// <summary>
        /// Get the number of ensembles.
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfEnsembles()
        {
            return TotalEnsembles;
        }

        /// <summary>
        /// Find the ensembles in the file.
        /// This will move through the file and
        /// all the ensembles from the file.
        /// </summary>
        private async void FindEnsembles()
        {
            if (_fileStream != null)
            {
                // Read in the data from file
                byte[] buffer = new byte[_fileStream.Length];
                int count = _fileStream.Read(buffer, 0, (int)_fileStream.Length);
                
                // Add the data to the codec
                await Task.Run(() => AddDataToCodecs(buffer));

                // Set the offset
                _fileOffset = count;
            }
        }

        #region Codecs

        /// <summary>
        /// Add data to the codecs.
        /// </summary>
        /// <param name="data">Data to add to the codec.</param>
        private void AddDataToCodecs(byte[] data)
        {
            // Binary Codec
            _adcpCodec.AddIncomingData(data);
        }

        #endregion


        #region EventHandler

        /// <summary>
        /// Receive the ensemble from the codec.
        /// Then set the ensemble size in bytes 
        /// so that the next ensemble can be found quicker.
        /// Set the flag that the ensemble was found.
        /// Then store the ensemble for playback.
        /// </summary>
        /// <param name="ensembleRaw">Ensemble binary data.</param>
        /// <param name="ensemble">Ensemble object.</param>
        private void _adcpCodec_ProcessDataEvent(byte[] ensembleRaw, DataSet.Ensemble ensemble)
        {
            // Set the length of an ensemble to find the next ensemble
            // quicker
            BytesPerEnsemble = ensembleRaw.Length;

            // Create the velocity vectors for the ensemble
            DataSet.VelocityVectorHelper.CreateVelocityVector(ref ensemble);

            // Store the found ensemble
            _ensembleList.Add(new EnsembleData(ensembleRaw, ensemble));

            // Set total number of ensembles
            // Subtract because 0 based
            TotalEnsembles = _ensembleList.Count() - 1;
        }

        /// <summary>
        /// Move forward when all the data has been decoded.
        /// </summary>
        void _adcpCodec_ProcessDataCompleteEvent()
        {
            _eventWaitDecode.Set();
        }

        #endregion
    }
}
