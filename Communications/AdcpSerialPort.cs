/*
 * Copyright © 2011 
 * Rowe Technology Inc.
 * All rights reserved.
 * http://www.rowetechinc.com
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification is NOT permitted.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
 * COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
 * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
 * POSSIBILITY OF SUCH DAMAGE.
 * 
 * HISTORY
 * -----------------------------------------------------------------
 * Date            Initials    Version    Comments
 * -----------------------------------------------------------------
 * 09/01/2011      RC                     Initial coding
 * 10/04/2011      RC                     Removed Codec and put in RecorderManager.
 * 12/29/2011      RC          1.11       Added event for ADCP serial data.
 *       
 * 
 */

using System.IO;

namespace RTI
{
    /// <summary>
    /// Extending the SerialConnection class, open a serial connection
    /// to the ADCP.  Then receive the data by overriding the ReceiveDataHandler
    /// method.  Pass the binary data from the serial port to the codec to be parsed
    /// and to the RecorderManager to be recorded to file.
    /// </summary>
    public class AdcpSerialPort : SerialConnection
    {

        /// <summary>
        /// The ADCP can be put into Compass mode
        /// to send and receive data to the internal
        /// compass.  When in Compass mode, the ADCP
        /// data does not have to decoded, but instead
        /// Compass data has to be decoded.
        /// </summary>
        public enum AdcpSerialModes
        {
            /// <summary>
            /// Data received in ADCP mode.
            /// </summary>
            ADCP,

            /// <summary>
            /// Data received in Compass mode.
            /// </summary>
            COMPASS
        }

        /// <summary>
        /// Mode the serial port is in.
        /// Either ADCP or COMPASS.
        /// </summary>
        public AdcpSerialModes Mode { get; set; }


        /// <summary>
        /// Constructor
        /// 
        /// </summary>
        /// <param name="adcpSerialOptions"></param>
        public AdcpSerialPort(SerialOptions adcpSerialOptions) :
            base(adcpSerialOptions)
        {
            // Set the default mode to ADCP mode
            Mode = AdcpSerialModes.ADCP;
        }

        /// <summary>
        /// Receive data from the serial port and pass the data to all
        /// who need the data.
        /// </summary>
        /// <param name="rcvData">Data received from the Serial port on seperate thread from UI.</param>
        protected override void ReceiveDataHandler(byte[] rcvData)
        {
            if (rcvData.Length > 0)
            {
                // Add data to the display
                SetReceiveBuffer(System.Text.ASCIIEncoding.ASCII.GetString(rcvData));

                if (Mode == AdcpSerialModes.ADCP)
                {
                    // Parse the ADCP data
                    if (this.ReceiveAdcpSerialDataEvent != null)
                    {
                        this.ReceiveAdcpSerialDataEvent(rcvData);
                    }
                }
                else if (Mode == AdcpSerialModes.COMPASS)
                {
                    // Parse the Compass data
                    if (this.ReceiveCompassSerialDataEvent != null)
                    {
                        this.ReceiveCompassSerialDataEvent(rcvData);
                    }
                }
            }
        }

        #region Events

        /// <summary>
        /// Event To subscribe to.  This gives the paramater
        /// that will be passed when subscribing to the event.
        /// </summary>
        /// <param data="ensemble">Byte array of data received from the port.</param>
        public delegate void ReceiveAdcpSerialDataEventHandler(byte[] data);

        /// <summary>
        /// Subscribe to receive event when data has been successfully
        /// processed.  This can be used to tell if data is in this format
        /// and is being processed or is not in this format.
        /// Subscribe to this event.  This will hold all subscribers.
        /// 
        /// To subscribe:
        /// _adcpSerialPort.ReceiveAdcpSerialDataEvent += new serialConnection.ReceiveAdcpSerialDataEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// _adcpSerialPort.ReceiveAdcpSerialDataEvent -= (method to call)
        /// </summary>
        public event ReceiveAdcpSerialDataEventHandler ReceiveAdcpSerialDataEvent;


        /// <summary>
        /// Event To subscribe to.  This gives the paramater
        /// that will be passed when subscribing to the event.
        /// </summary>
        /// <param data="ensemble">Byte array of data received from the port.</param>
        public delegate void ReceiveCompassSerialDataEventHandler(byte[] data);

        /// <summary>
        /// Subscribe to receive event when data has been successfully
        /// processed.  This can be used to tell if data is in this format
        /// and is being processed or is not in this format.
        /// Subscribe to this event.  This will hold all subscribers.
        /// 
        /// To subscribe:
        /// _adcpSerialPort.ReceiveCompassSerialDataEvent += new serialConnection.ReceiveCompassSerialDataEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// _adcpSerialPort.ReceiveCompassSerialDataEvent -= (method to call)
        /// </summary>
        public event ReceiveCompassSerialDataEventHandler ReceiveCompassSerialDataEvent;

        #endregion

    }
}