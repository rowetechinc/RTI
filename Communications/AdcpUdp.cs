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
 * 09/13/2017      RC          3.4.3     Initial coding
 * 05/11/2020      RC          3.4.17    Fixed bug in AdcpUdp when UDP is not initialized.     
 * 
 * 
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RTI
{
    /// <summary>
    /// Receive and send data on the UDP port.
    /// </summary>
    public class AdcpUdp : IDisposable
    {

        /// <summary>
        /// Object to hold the received data.
        /// </summary>
        public struct Datagram
        {
            /// <summary>
            /// Sender of the data.
            /// </summary>
            public IPEndPoint Sender;

            /// <summary>
            /// JSON message.
            /// </summary>
            public string Message;

            /// <summary>
            /// Raw Data received.
            /// </summary>
            public byte[] RawMessage;
        }

        #region Variable

        /// <summary>
        ///  Setup logger
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Flag to stop the read thread.
        /// </summary>
        public bool _stopUdpReadThread;

        /// <summary>
        /// UDP connection.
        /// </summary>
        private UdpClient _udpClient;

        /// <summary>
        /// Endpoint for the connection.
        /// </summary>
        private IPEndPoint _endPoint;

        #endregion

        #region Properties

        /// <summary>
        /// Options for the ethernet connection.
        /// </summary>
        public AdcpEthernetOptions Options { get; set; }

        /// <summary>
        /// Buffer to hold all the data received from the ethernet port.
        /// </summary>
        private string _receiveBufferString;
        /// <summary>
        /// Buffer to hold all the data received from the ethernet port.
        /// </summary>
        public string ReceiveBufferString
        {
            get { return _receiveBufferString; }
            set
            {
                _receiveBufferString = value;

                // Limit the size of the data in the buffer
                if (_receiveBufferString.Length > 16000)
                {
                    _receiveBufferString = _receiveBufferString.Remove(10000, 6000);
                }
            }
        }

        #endregion

        /// <summary>
        /// Send and receive data from any IP address on this port.
        /// </summary>
        /// <param name="port">Port to listen for data.</param>
        public AdcpUdp(int port)
        {
            // Create the endpoint
            _endPoint = new IPEndPoint(IPAddress.Any, port);

            try
            {
                // Make connection
                _udpClient = new UdpClient(port);
            }
            catch(System.Net.Sockets.SocketException)
            {
                Debug.WriteLine("UDP Port already in use on this computer.");
                log.Warn("UDP Port already in use on this computer.");
                ReceiveBufferString = "UDP Port " + _endPoint.Port + " is  already in use on this computer.";
                return;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error making a UDP connection.", ex);
                log.Warn("Error making a UDP connection.", ex);
                ReceiveBufferString = "Error making a UDP connection to port " + _endPoint.Port;
                return;
            }

            // Initialize
            Init();
        }

        /// <summary>
        /// Send and receive data to the given IP and port.
        /// </summary>
        /// <param name="endpoint">IP and port.</param>
        public AdcpUdp(IPEndPoint endpoint)
        {
            // Create the endpoint
            _endPoint = endpoint;

            try
            {
                // Make connection
                _udpClient = new UdpClient(endpoint.Address.ToString(), endpoint.Port);
            }
            catch(System.Net.Sockets.SocketException e)
            {
                Debug.WriteLine("UDP Port already in use on this computer.", e);
                log.Warn("UDP Port already in use on this computer.", e);
                ReceiveBufferString = "UDP Port " + _endPoint.Port + " is  already in use on this computer.";
                return;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Error making a UDP connection.", ex);
                log.Warn("Error making a UDP connection.", ex);
                ReceiveBufferString = "Error making a UDP connection to port " + _endPoint.Port;
                return;
            }

            // Initialize
            Init();
        }

        /// <summary>
        /// Initialize the object.  Start the thread.
        /// </summary>
        private void Init()
        {
            // Begin reading data
            _udpClient.BeginReceive(new AsyncCallback(recv), null);
        }

        /// <summary>
        /// Flag too tell if the UDP is connected.
        /// </summary>
        public bool IsOpen() 
        { 
            return _udpClient != null; 
        }

        /// <summary>
        /// Shutdown the object.
        /// </summary>
        public void Dispose()
        {
            _stopUdpReadThread = true;

            if (_udpClient != null)
            {
                _udpClient.Close();
                _udpClient = null;
            }
        }

        #region Send Data

        /// <summary>
        /// Send a BREAK to the TCP port.
        /// </summary>
        public void SendBreak()
        {
            // Send a soft break
            SendData(Commands.AdcpCommands.CMD_BREAK);
        }

        /// <summary>
        /// Send the given string to through the
        /// UDP connection.
        /// </summary>
        /// <param name="data">Data to send.</param>
        /// <param name="useCR">Flag to include carrage return.</param>
        /// <returns>Flag if data written.</returns>
        public bool SendData(string data, bool useCR = true)
        {
            // Not connected
            if(_udpClient ==  null)
            {
                return false;
            }

            try
            {
                // Format the command
                if (useCR)
                {
                    data += '\r';
                }

                //byte[] buffer = MathHelper.GetBytes(data);
                byte[] buffer = MathHelper.GetBytesUtf8(data);
                _udpClient.Send(buffer, buffer.Length);
            }
            catch (Exception e)
            {
                log.Error(string.Format("Error sending data to UDP port. Sent data: {0} length: {1}", data, data.Length), e);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Write the data to the UDP Port.
        /// </summary>
        /// <param name="data">Data to write.</param>
        /// <param name="offset">Offset to start the write.</param>
        /// <param name="size">Number of bytes to write from the given data.</param>
        /// <returns>Flag if data was written.</returns>
        public bool SendData(byte[] data, int offset, int size)
        {
            // Not connected
            if (_udpClient == null)
            {
                return false;
            }

            try
            {
                // Send data
                _udpClient.Send(data, size);
            }
            catch (ArgumentNullException e)
            {
                log.Error("ArgumentNullException UDP Write.", e);
                return false;
            }
            catch (SocketException e)
            {
                log.Error("SocketException UDP Write.", e);
                return false;
            }
            catch (Exception e)
            {
                log.Error("Exception UDP Write.", e);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Send a list of commands.
        /// </summary>
        /// <param name="commands">List of commands.</param>
        /// <returns>TRUE = All commands were sent successfully.</returns>
        public bool SendCommands(List<string> commands)
        {
            bool result = true;
            byte[] reply = new byte[100];

            foreach (string cmd in commands)
            {
                if (cmd.CompareTo(RTI.Commands.AdcpCommands.CMD_BREAK) == 0)
                {
                    // Send a break
                    SendBreak();
                }
                else
                {
                    // Send the command
                    bool currResult = SendData(cmd);

                    // Keep track if any were false
                    // If any were false, this will stay false
                    result &= currResult;
                }

                // Write the line out
                //Debug.WriteLine(cmd);
            }

            return result;
        }

        #endregion

        #region Read data

        /// <summary>
        /// Read data from the UDP port.
        /// </summary>
        /// <param name="res">Async response from read.</param>
        private void recv(IAsyncResult res)
        {
            try
            {
                if (res != null && _udpClient != null)
                {
                    byte[] received = _udpClient.EndReceive(res, ref _endPoint);

                    if (received != null && _endPoint != null)
                    {
                        // Convert received message to a string
                        string message = "";
                        if (received[0] == 0xFF)
                        {
                            message = _endPoint.Address.ToString() + ":" + _endPoint.Port.ToString() + "\r\n";
                            for (int i = 1; i < received.Length; i++)
                                message += received[i].ToString("X2");

                            message += "\r\n";
                        }
                        else
                        {
                            message = System.Text.ASCIIEncoding.ASCII.GetString(received, 0, received.Length);
                        }

                        // Pass the data to all subscribers
                        AdcpUdp.Datagram datagram = new AdcpUdp.Datagram()
                        {
                            RawMessage = received,
                            Message = message,
                            Sender = _endPoint
                        };

                        // Publish the data
                        PublishRawUdpData(datagram);

                        // Set the Receive Buffer
                        ReceiveBufferString += message;

                        // Keep the Buffer to a set limit
                        if (ReceiveBufferString.Length > 5000)
                        {
                            ReceiveBufferString = ReceiveBufferString.Substring(ReceiveBufferString.Length - 5000);
                        }

                        // Check for the next message
                        _udpClient.BeginReceive(new AsyncCallback(recv), null);
                    }
                }
            }
            catch(Exception e) 
            {
                log.Error("Error reading data from UDP Port.", e);
            }
        }


        #endregion

        #region Events

        /// <summary>
        /// Event To subscribe to.  This gives the paramater
        /// that will be passed when subscribing to the event.
        /// </summary>
        /// <param data="ensemble">Byte array of data received from the port.</param>
        public delegate void ReceiveRawUdpDataEventHandler(byte[] data);

        /// <summary>
        /// Subscribe to receive event when data has been successfully
        /// processed.  This can be used to tell if data is in this format
        /// and is being processed or is not in this format.
        /// Subscribe to this event.  This will hold all subscribers.
        /// 
        /// To subscribe:
        /// _adcpUdp.ReceiveRawUdpDataEvent += new AdcpTcp.ReceiveRawUdpDataEventHandler(method to call);
        /// 
        /// To Unsubscribe:
        /// _adcpUdp.ReceiveRawUdpDataEvent -= (method to call)
        /// </summary>
        public event ReceiveRawUdpDataEventHandler ReceiveRawUdpDataEvent;

        /// <summary>
        /// Send the data to all the subscribers.
        /// </summary>
        /// <param name="datagram">Data to send.</param>
        private void PublishRawUdpData(AdcpUdp.Datagram datagram)
        {
            // If there are subscribers, send the data
            if (ReceiveRawUdpDataEvent != null)
            {
                ReceiveRawUdpDataEvent(datagram.RawMessage);
            }
        }

        #endregion
    }
}
