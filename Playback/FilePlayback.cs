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
 * 06/12/2015      RC          3.0.5      Removed the file from the constructor and moved it to FindEnsembles().
 * 07/27/2015      RC          3.0.5      Set the file name playing back. 
 * 08/13/2015      RC          3.0.5      Read the file with a smaller buffer so large files will not take all the RAM in FindEnsembles().
 * 05/11/2016      RC          3.3.2      Changed from list to dictionary to prevent playback being out of order.
 * 03/28/2017      RC          3.4.2      Fixed bug in FindRtbEnsembles() when ensemble numbers are duplicated.
 * 09/29/2017      RC          3.4.4      Pass the data format the data was recorded.
 * 04/15/2019      RC          3.4.11     If the ensemble number is the same, then use datetime ticks for the key for the ensemble.
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
        ///  Setup logger
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
            /// Original data format.
            /// </summary>
            public AdcpCodec.CodecEnum OrigDataFormat { get; set; }

            /// <summary>
            /// Initialize the value.
            /// </summary>
            /// <param name="rawData">Raw binary data.</param>
            /// <param name="ensemble">Ensemble data.</param>
            /// <param name="origDataFormat">Original Data format.</param>
            public EnsembleData(byte[] rawData, DataSet.Ensemble ensemble, AdcpCodec.CodecEnum origDataFormat)
            {
                RawData = rawData;
                Ensemble = ensemble;
                OrigDataFormat = origDataFormat;
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
        /// Dictionary to hold all the ensemble data.
        /// </summary>
        private Dictionary<int, EnsembleData> _ensembleDict;

        /// <summary>
        /// Current ensemble to display.
        /// </summary>
        private int _currEnsNum;

        /// <summary>
        /// First ensemble number.  Used to jump around.
        /// </summary>
        private int _firstEnsNum;

        /// <summary>
        /// Last ensemble number.  Used to handle duplicate ensemble numbers.
        /// </summary>
        private int _lastEnsNum;

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

        /// <summary>
        /// File name.
        /// </summary>
        public string Name { get; set; }

        #endregion

        /// <summary>
        /// Playback the given file.
        /// </summary>
        public FilePlayback()
        {
            // Initialize values
            BytesPerEnsemble = DEFAULT_BYTES_PER_ENSEMBLE;
            _fileOffset = 0;
            TotalEnsembles = 0;
            //_ensembleList = new List<EnsembleData>();
            Name = "";
            _ensembleDict = new Dictionary<int, EnsembleData>();
            _currEnsNum = 0;
            _firstEnsNum = 0;
            _lastEnsNum = 0;

            // Codecs
            _adcpCodec = new AdcpCodec();
            _adcpCodec.ProcessDataEvent += new AdcpCodec.ProcessDataEventHandler(_adcpCodec_ProcessDataEvent);
            _adcpCodec.ProcessDataCompleteEvent += _adcpCodec_ProcessDataCompleteEvent;
            _adcpCodec.GoodEnsembleEvent += ReceivedGoodEnsembleEvent;
            _adcpCodec.BadEnsembleEvent += ReceivedBadEnsembleEvent;

            // Wait for decoding to be complete
            _eventWaitDecode = new EventWaitHandle(false, EventResetMode.AutoReset);
        }

        /// <summary>
        /// Dispose of the object.
        /// </summary>
        public void Dispose()
        {
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
        /// Find the next ensemble based off the first ensemble given.
        /// This will max out at int max value.  If it reaches this, it will give
        /// the first ensemble in the dictionary.
        /// </summary>
        /// <param name="ensNum"></param>
        private void FindEns(int ensNum)
        {
            while (!_ensembleDict.ContainsKey(_currEnsNum))
            {
                _currEnsNum++;

                // If we hit the max, then give the first ensemble
                if(_currEnsNum == Int16.MaxValue)
                {
                    _currEnsNum = _ensembleDict.Keys.First();
                }
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

            if (PlaybackIndex < _ensembleDict.Count)
            {
                // Find the first ensemble
                if (_currEnsNum == 0)
                {
                    while (!_ensembleDict.ContainsKey(_currEnsNum))
                    {
                        _currEnsNum++;
                    }
                }

                // Find the next good ensemble
                FindEns(_currEnsNum);

                // Set the values
                args.Index = PlaybackIndex;
                args.Ensemble = _ensembleDict[_currEnsNum].Ensemble;
                args.TotalEnsembles = TotalEnsembles;
                args.OrigDataFormat = _ensembleDict[_currEnsNum].OrigDataFormat;

                _currEnsNum++;
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
            if(_currEnsNum -1 <= 0)
            {
                PlaybackIndex = 0;
                _currEnsNum = 0;
                return null;
            }

            // Create the args and set the total number of ensembles
            PlaybackArgs args = new PlaybackArgs();

            if ( PlaybackIndex < _ensembleDict.Count)
            {

                PlaybackIndex--;
                if(PlaybackIndex < 0)
                {
                    PlaybackIndex = 0;
                }

                _currEnsNum--;

                // Find the next good ensemble
                FindEns(_currEnsNum);

                // Set the values
                args.Index = PlaybackIndex;
                args.Ensemble = _ensembleDict[_currEnsNum].Ensemble;
                args.TotalEnsembles = TotalEnsembles;
                args.OrigDataFormat = _ensembleDict[_currEnsNum].OrigDataFormat;
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

            // Get the first ensemble number
            // Determine how far forward to jump based off index
            // Then add the jump to the first ensemble number and find the next 
            // ensemble number near that jump
            if (index < _ensembleDict.Count)
            {
                // Set new index
                PlaybackIndex = index;

                // Set new current ensemble
                // Zero based, so subtract 1
                _currEnsNum = _firstEnsNum + (int)index;

                // Find the next good ensemble
                FindEns(_currEnsNum);

                // Set the values
                args.Index = PlaybackIndex;
                args.Ensemble = _ensembleDict[_currEnsNum].Ensemble;
                args.TotalEnsembles = TotalEnsembles;
                args.OrigDataFormat = _ensembleDict[_currEnsNum].OrigDataFormat;
            }

            return args;
        }

        /// <summary>
        /// Return a list of all the ensembles.
        /// </summary>
        /// <returns>List of all the ensembles in the file.</returns>
        public Cache<long, DataSet.Ensemble> GetAllEnsembles()
        {
            Cache<long, DataSet.Ensemble> list = new Cache<long, DataSet.Ensemble>((uint)_ensembleDict.Count);

            // Populate the cache with all the ensembles
            int x = 0;
            foreach(var ens in _ensembleDict.Values)
            {
                list.Add(x++, ens.Ensemble);
            }

            return list;
        }

        /// <summary>
        /// Get the original data format.  This is the format
        /// that the data was recorded in.
        /// </summary>
        /// <returns>Original data format the data was recorded.</returns>
        public AdcpCodec.CodecEnum GetOrigDataFormat()
        {
            if(_ensembleDict.Count > 0)
            {
                return _ensembleDict.First().Value.OrigDataFormat;
            }

            // Default to binary
            return AdcpCodec.CodecEnum.Binary;
        }

        /// <summary>
        /// Return a list of all the ensembles.
        /// </summary>
        /// <returns>List of all the ensembles in the file.</returns>
        public List<EnsembleData> GetEnsembleDataList()
        {
            return _ensembleDict.Values.ToList();
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
        /// Find all the ensebles in a RoweTech Binary file.
        /// This is optimized to work with RoweTech Binary files (RTB).
        /// </summary>
        /// <param name="files">List of files.</param>
        public void FindRtbEnsembles(string[] files)
        {
            for(int x = 0; x < files.Length; x++)
            {
                // Find the ensembles in the file
                FindRtbEnsembles(files[x]);

                // Publish events that file is complete
                if(ProcessFileEvent != null)
                {
                    ProcessFileEvent(files.Length, x+1, files[x]);
                }
            }
        }

        /// <summary>
        /// Find all the ensembles in the given file.
        /// Add them to the dictionary.
        /// </summary>
        /// <param name="file">File to search for the ensembles.</param>
        public void FindRtbEnsembles(string file)
        {
            AdcpCodecReadFile readFile = new AdcpCodecReadFile();
            readFile.GoodEnsembleEvent += ReceivedGoodEnsembleEvent;
            readFile.BadEnsembleEvent += ReceivedBadEnsembleEvent;
            var list = readFile.GetEnsembles(file);

            // Add the ensembles to the dictionary
            foreach (var ens in list)
            {
                try
                {
                    AddEnsemble(ens.RawEnsemble, ens.Ensemble, ens.OrigDataFormat);
                }
                catch(Exception e)
                {
                    string num = "";
                    if(ens.Ensemble.IsEnsembleAvail)
                    {
                        num = ens.Ensemble.EnsembleData.EnsembleNumber.ToString();
                    }

                    log.Error("Error adding the ensemble to the list.  " + num + " " + _ensembleDict.Count, e);
                }
            }
        }

        /// <summary>
        /// Find the ensembles in the file.
        /// </summary>
        /// <param name="files">Files to look for the ensembles.</param>
        public void FindEnsembles(string[] files)
        {
            for (int x = 0; x < files.Length; x++)
            {
                FindEnsembles(files[x]);
            }
        }

        /// <summary>
        /// Find the ensembles in the file.
        /// This will move through the file and
        /// all the ensembles from the file.
        /// </summary>
        /// <param name="file">File to get the ensembles.</param>
        public void FindEnsembles(string file)
        {
            try
            { 

                // Open the file
                if (File.Exists(file))
                {
                    //Set the file name
                    FileInfo finfo = new FileInfo(file);
                    Name = finfo.Name;

                    using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        _fileSize = fileStream.Length;

                        // Read in the data from file
                        byte[] buffer = new byte[1024 * 100];
                        int count = 0;
                        while ((count = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            // Add the data to the codec
                            _adcpCodec.AddIncomingData(buffer);

                            // Set the offset
                            _fileOffset += count;
                        }

                        fileStream.Close();
                        fileStream.Dispose();
                    }


                    // Block until awoken when all the data is decoded
                    // Or wait 30 seconds for a timeout.
                    _eventWaitDecode.WaitOne(30000);
                }
            }
            catch(Exception e)
            {
                log.Error("Error reading the file.", e);

                // Set the block to not wait any longer for data
                _eventWaitDecode.Set();
            }
        }

        /// <summary>
        /// Add the ensemble to the dictionary.  The ensemble will then
        /// be available to playback.
        /// </summary>
        /// <param name="ensembleRaw"></param>
        /// <param name="ensemble"></param>
        /// <param name="origDataFormat">Originl Data format.</param>
        public void AddEnsemble(byte[] ensembleRaw, DataSet.Ensemble ensemble, AdcpCodec.CodecEnum origDataFormat)
        {
            // **********
            // Moved this to eventhandler
            // Not needed when reading in the entire file
            // **********
            // Copy the data
            //var ens = ensemble.Clone();
            //byte[] raw = new byte[ensembleRaw.Length];
            //Buffer.BlockCopy(ensembleRaw, 0, raw, 0, ensembleRaw.Length);

            // Create the velocity vectors for the ensemble
            DataSet.VelocityVectorHelper.CreateVelocityVector(ref ensemble);

            if (ensemble.IsEnsembleAvail)
            {
                // Store the found ensemble to the dictionary
                if (!_ensembleDict.ContainsKey(ensemble.EnsembleData.EnsembleNumber))
                {
                    _ensembleDict.Add(ensemble.EnsembleData.EnsembleNumber, new EnsembleData(ensembleRaw, ensemble, origDataFormat));

                    // Find the first ensemble number
                    if (_firstEnsNum == 0 || _firstEnsNum > ensemble.EnsembleData.EnsembleNumber)
                    {
                        _firstEnsNum = ensemble.EnsembleData.EnsembleNumber;
                    }

                    // Find the last ensemble number
                    if (_lastEnsNum < ensemble.EnsembleData.EnsembleNumber)
                    {
                        _lastEnsNum = ensemble.EnsembleData.EnsembleNumber;
                    }
                }
                else
                {
                    if (!_ensembleDict.ContainsKey(_lastEnsNum + ensemble.EnsembleData.EnsembleNumber))
                    {
                        // Create a new ensemble number key based off the last ensemble and add 1
                        _ensembleDict.Add(_lastEnsNum + ensemble.EnsembleData.EnsembleNumber, new EnsembleData(ensembleRaw, ensemble, origDataFormat));
                    }
                    else
                    {
                        // Generate the key from the datetime
                        _ensembleDict.Add((int)DateTime.Now.Ticks, new EnsembleData(ensembleRaw, ensemble, origDataFormat));
                    }
                }
            }

            // Set total number of ensembles
            // Subtract because 0 based
            TotalEnsembles = _ensembleDict.Count() - 1;
        }

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
        /// <param name="origDataFormat">Original Data format.</param>
        private void _adcpCodec_ProcessDataEvent(byte[] ensembleRaw, DataSet.Ensemble ensemble, AdcpCodec.CodecEnum origDataFormat)
        {
            // Set the length of an ensemble to find the next ensemble
            // quicker
            BytesPerEnsemble = ensembleRaw.Length;

            // Copy the data
            var ens = ensemble.Clone();
            byte[] raw = new byte[ensembleRaw.Length];
            Buffer.BlockCopy(ensembleRaw, 0, raw, 0, ensembleRaw.Length);

            AddEnsemble(raw, ens, origDataFormat);

        }

        /// <summary>
        /// Move forward when all the data has been decoded.
        /// </summary>
        void _adcpCodec_ProcessDataCompleteEvent()
        {
            _eventWaitDecode.Set();
        }

        /// <summary>
        /// Pass the event along.
        /// </summary>
        private void ReceivedBadEnsembleEvent()
        {
            if(GoodEnsembleEvent != null)
            {
                GoodEnsembleEvent();
            }
        }

        /// <summary>
        /// Pass the event along.
        /// </summary>
        private void ReceivedGoodEnsembleEvent()
        {
            if(BadEnsembleEvent != null)
            {
                BadEnsembleEvent();
            }
        }


        #endregion

        /// <summary>
        /// Processing the File event.
        /// </summary>
        /// <param name="fileCount">Total number of files.</param>
        /// <param name="fileIndex">Index of process.</param>
        /// <param name="fileName">File name.</param>
        public delegate void ProcessFileEventHandler(int fileCount, int fileIndex, string fileName);

        /// <summary>
        /// Subscribe to receive event when a file has completed being processed.
        /// 
        /// To subscribe:
        /// fpb.ProcessFileEvent += new fpb.ProcessFileEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// fpb.ProcessFileEvent -= (method to call)
        /// </summary>
        public event ProcessFileEventHandler ProcessFileEvent;

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
    }
}
