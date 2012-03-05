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
 * 10/04/2011      RC                     Removed Codec and put in RecorderManager.
 * 12/29/2011      RC          1.11       Added event for ADCP serial data.
 * 02/10/2012      RC          2.02       Added common commands to the ADCP.
 * 02/29/2012      RC          2.04       Added a try/catch block in ReceiveDataHandler() to catch any issues so the serial port will not disconnect on issues.
 *       
 * 
 */

using System.IO;
using System.Threading;
using System;

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
        /// Time to wait to see if a command
        /// was sent.  If a timeout occurs
        /// it means a command did not get
        /// a response from the ADCP for sending
        /// a command.  Value in milliseconds.
        /// </summary>
        private const int TIMEOUT = 750; 

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
                try
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
                catch (Exception)
                {
                    // Do nothing on exception
                }
            }
        }

        #region Ping Serial Commands

        /// <summary>
        /// Send the commands to the ADCP to start pinging.
        /// To start pinging, send the command START.  This
        /// will also set the date and time to the system
        /// based off the computers current date and time.
        /// If any command cannot be sent, set the flag and
        /// return that a command could not be set.
        /// </summary>
        /// <returns>TRUE = All commands sent. FALSE = 1 or more commands could not be sent.</returns>
        public bool StartPinging()
        {
            bool timeResult = false;
            bool pingResult = false;

            // Try to send the command, if it fails try again
            timeResult = SendDataWaitReply(RTI.Commands.AdcpCommands.GetTimeCommand(), TIMEOUT);
            if (!timeResult)
            {
                timeResult = SendDataWaitReply(RTI.Commands.AdcpCommands.GetTimeCommand(), TIMEOUT);
            }

            // Allow some time for the ADCP to set the time
            System.Threading.Thread.Sleep(500);

            // Try to send the command, if it fails try again
            pingResult = SendDataWaitReply(RTI.Commands.AdcpCommands.CMD_START_PINGING, TIMEOUT);
            if (!pingResult)
            {
                pingResult = SendDataWaitReply(RTI.Commands.AdcpCommands.CMD_START_PINGING, TIMEOUT);
            }

            // Return if either failed
            return timeResult & pingResult;
        }

        /// <summary>
        /// Send the commands to the ADCP to stop pinging.
        /// To stop pinging, send a BREAK then the STOP command.
        /// The command will wait for an echo back off the command.
        /// If thhe command is not echoed back, then the command was
        /// not receive or the command was not correct.
        /// If any command cannot be sent, set the flag and
        /// return that a command could not be sent.
        /// </summary>
        /// <returns>TRUE = All commands sent. FALSE = 1 or more commands could not be sent.</returns>
        public bool StopPinging()
        {
            bool stopResult = false;

            // Send a break
            SendBreak();

            // Try to send the command, if it fails try again
            stopResult = SendDataWaitReply(RTI.Commands.AdcpCommands.CMD_STOP_PINGING, TIMEOUT);
            if (!stopResult)
            {
                stopResult = SendDataWaitReply(RTI.Commands.AdcpCommands.CMD_STOP_PINGING, TIMEOUT);
            }

            // Return if either failed
            return stopResult;
        }

        #endregion

        #region Compass Serial Commands

        /// <summary>
        /// Start Compass mode.  This make the ADCP output
        /// compass data instead of ADCP data.
        /// </summary>
        /// <returns>TRUE if command sent properly.</returns>
        public bool StartCompassMode()
        {
            bool startResult = false;

            // Try to send the command, if it fails try again
            startResult = SendDataWaitReply(RTI.Commands.AdcpCommands.CMD_DIAGCPT, TIMEOUT);
            if (!startResult)
            {
                startResult = SendDataWaitReply(RTI.Commands.AdcpCommands.CMD_DIAGCPT, TIMEOUT);

                // Delay for 485 response
                Thread.Sleep(100);
            }

            // If was capable of putting 
            // into compass mode, change the mode
            if (startResult)
            {
                // Set the serial port to COMPASS mode to decode compass data
                Mode = AdcpSerialPort.AdcpSerialModes.COMPASS;
            }

            // Return if either failed
            return startResult;
        }

        /// <summary>
        /// Stop the ADCP from being in Compass mode.
        /// This will set the compass to interval mode, 
        /// then set the ADCP back to ADCP mode.
        /// </summary>
        public void StopCompassMode()
        {
            // Turn on compass interval outputing
            byte[] startIntervalCmd = PniPrimeCompassBinaryCodec.StartIntervalModeCommand();
            SendData(startIntervalCmd, 0, startIntervalCmd.Length);

            // Delay for 485 response
            Thread.Sleep(100);

            // Stop ADCP from compass mode
            SendData(RTI.Commands.AdcpCommands.CMD_DIAGCPT_DISCONNECT);

            // Set the serial port to ADCP mode to decode ADCP data
            Mode = AdcpSerialPort.AdcpSerialModes.ADCP;
        }

        /// <summary>
        /// Start compass calibration.  Set flag whether to do 
        /// magnitude and acceleration calibration or just magnitude
        /// calibration.
        /// </summary>
        /// <param name="IsMagAndAccelCalibration"></param>
        public void StartCompassCal(bool IsMagAndAccelCalibration)
        {
            // If not in compass mode, then put in compass mode
            if (Mode != AdcpSerialPort.AdcpSerialModes.COMPASS)
            {
                StartCompassMode();
            }

            // Stop compass from outputing data
            byte[] stopIntervalCmd = PniPrimeCompassBinaryCodec.StopIntervalModeCommand();
            SendData(stopIntervalCmd, 0, stopIntervalCmd.Length);
            
            Thread.Sleep(100);                  // Delay for 485 response

            // Start compass in calibration mode
            byte[] startCalCmd;
            if (IsMagAndAccelCalibration)
            {
                startCalCmd = PniPrimeCompassBinaryCodec.StartCalibrationCommand(PniPrimeCompassBinaryCodec.CalMode.CAL_ACCEL_AND_MAG);
            }
            else
            {
                startCalCmd = PniPrimeCompassBinaryCodec.StartCalibrationCommand(PniPrimeCompassBinaryCodec.CalMode.CAL_MAG_ONLY);
            }
            SendData(startCalCmd, 0, startCalCmd.Length);
        }

        /// <summary>
        /// Stop a calibration process.  This is used
        /// if the user would like to restart a compass calibration.
        /// </summary>
        public void StopCompassCal()
        {
            // Stop Calibration mode
            byte[] stopCalCmd = PniPrimeCompassBinaryCodec.StopCalibrationCommand();
            SendData(stopCalCmd, 0, stopCalCmd.Length);

            // Delay for 485 response
            Thread.Sleep(100);
        }

        /// <summary>
        /// Save the compass calibration to the compass.
        /// The ADCP must be in compass mode to accomplish this.
        /// </summary>
        public void SaveCompassCal()
        {
            if (Mode == AdcpSerialModes.COMPASS)
            {
                // Stop Calibration mode
                byte[] SaveCalDataCmd = PniPrimeCompassBinaryCodec.SaveCompassCalCommand();
                SendData(SaveCalDataCmd, 0, SaveCalDataCmd.Length);
                Thread.Sleep(100);
            }
        }

        #endregion

        #region Configuration Serial Commands

        /// <summary>
        /// Send the command to set the default values to the ADCP.
        /// </summary>
        /// <returns>TRUE = All commands sent. FALSE = 1 or more commands could not be sent.</returns>
        public bool SetDefaults()
        {
            bool defaultResult = false;

            // Send the command within the buffer
            // Try sending the command.  If it fails try one more time
            defaultResult = SendDataWaitReply(RTI.Commands.AdcpCommands.CMD_CDEFAULT, TIMEOUT);
            if (!defaultResult)
            {
                // Try again if failed first time
                defaultResult = SendDataWaitReply(RTI.Commands.AdcpCommands.CMD_CDEFAULT, TIMEOUT);
            }

            return defaultResult;
        }

        /// <summary>
        /// Save the configuration to the SD card.
        /// </summary>
        /// <returns>TRUE = All commands sent. FALSE = 1 or more commands could not be sent.</returns>
        public bool SaveConfigurations()
        {

            bool saveResult = false;

            // Send the command within the buffer
            // Try sending the command.  If it fails try one more time
            saveResult = SendDataWaitReply(RTI.Commands.AdcpCommands.CMD_CSAVE, TIMEOUT);
            if (!saveResult)
            {
                // Try again if failed first time
                saveResult = SendDataWaitReply(RTI.Commands.AdcpCommands.CMD_CSAVE, TIMEOUT);
            }

            return saveResult;
        }

        #endregion

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