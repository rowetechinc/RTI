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
 * 09/01/2011      RC                     Initial coding
 * 12/08/2011      RC          1.09       Set Receive Buffer
 * 12/12/2011      RC          1.09       Added RecorderManager. 
 *                                         Record incoming data to RecorderManager.
 * 12/13/2011      RC          1.09       Added IsEnabled property.
 * 12/29/2011      RC          1.11       Added event for GPS serial data.
 *       
 * 
 */


using System.Diagnostics;

namespace RTI
{
    /// <summary>
    /// Serial port connected to a GPS.
    /// This is created to handle GPS specific
    /// serial ports.
    /// </summary>
    public class GpsSerialPort : SerialConnection
    {
        /// <summary>
        /// Set if the GPS is enabled or disabled.
        /// If the GPS is disabled, it will not
        /// send data.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Create a GPS with the given options.
        /// </summary>
        /// <param name="gpsSerialOptions">Serial port options.</param>
        public GpsSerialPort(SerialOptions gpsSerialOptions) :
            base(gpsSerialOptions)
        {
            IsEnabled = false;
        }

        /// <summary>
        /// Set the receive text buffer with the latest data
        /// from the GPS.  Then pass the data to the RecorderManager
        /// to be parsed and recorded.
        /// </summary>
        /// <param name="rcvData">Data received from the Serial port</param>
        protected override void ReceiveDataHandler(byte[] rcvData)
        {
            if (rcvData.Length > 0 && IsEnabled)
            {
                string nmeaStrings = System.Text.ASCIIEncoding.ASCII.GetString(rcvData);

                // Add data to the display
                SetReceiveBuffer(nmeaStrings);

                // Send data to RecorderManager to add to dataset
                //_recorderMgr.ParseNmeaData(nmeaStrings);
                if (this.ReceiveGpsSerialDataEvent != null)
                {
                    this.ReceiveGpsSerialDataEvent(nmeaStrings);
                }
            }
        }

        /// <summary>
        /// Event To subscribe to.  This gives the paramater
        /// that will be passed when subscribing to the event.
        /// </summary>
        /// <param data="ensemble">String of data received from the port.</param>
        public delegate void ReceiveGpsSerialDataEventHandler(string data);

        /// <summary>
        /// Subscribe to receive event when data has been successfully
        /// processed.  This can be used to tell if data is in this format
        /// and is being processed or is not in this format.
        /// Subscribe to this event.  This will hold all subscribers.
        /// 
        /// To subscribe:
        /// _gpsSerialPort.ReceiveGpsSerialDataEvent += new serialConnection.ReceiveGpsSerialDataEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// _gpsSerialPort.ReceiveGpsSerialDataEvent -= (method to call)
        /// </summary>
        public event ReceiveGpsSerialDataEventHandler ReceiveGpsSerialDataEvent;
    }
}