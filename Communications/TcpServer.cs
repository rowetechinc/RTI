using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RTI
{
    /// <summary>
    /// Class to make a TCP server.
    /// </summary>
    class TcpServer
    {
        /// <summary>
        /// Server connection.
        /// </summary>
        private TcpListener _server;

        /// <summary>
        /// Flag if alive.
        /// </summary>
        private bool _isAlive = true;

        /// <summary>
        /// List of all the connected clients.
        /// </summary>
        private List<TcpClient> _tcpClients;

        /// <summary>
        /// Make a TCP server for the given port.
        /// </summary>
        /// <param name="port">Port to connect.</param>
        public TcpServer(int port)
        {
            // Create the list of clients
            _tcpClients = new List<TcpClient>();

            // Set the IP address and Port
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            _server = new TcpListener(localAddr, RTI.Core.Commons.TCP_ENS);

            // Start listening
            _server.Start();

            // Wait for connections by clients
            Task.Run(() => WaitForConnections());
        }

        /// <summary>
        /// Close the socket server.
        /// </summary>
        public void Close()
        {
            // Stop taking connections
            _isAlive = false;

            // Disconnect all the clients
            foreach (var client in _tcpClients)
            {
                client.Close();
            }
        }

        /// <summary>
        /// Wait for incoming connections and add them to the
        /// the list of clients.
        /// </summary>
        /// <returns></returns>
        private Task WaitForConnections()
        {
            while (_isAlive)
            {
                // Block until a client attempts a connect
                var client = _server.AcceptTcpClient();

                // Add the client to the list
                _tcpClients.Add(client);
            }

            return null;
        }

        /// <summary>
        /// Write the message to all the clients connected to the server.
        /// </summary>
        /// <param name="message">Message to write.</param>
        public void Write(string message)
        {
            List<TcpClient> clientRemoveList = new List<TcpClient>();

            // Go through all the clients and write the data.
            foreach (var client in _tcpClients)
            {
                if (IsClientConnected(client))
                {

                    // Convert the string to a byte array
                    // Add a new line to the end of the message
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(message);

                    try
                    {
                        // Get a stream object for reading and writing
                        NetworkStream stream = client.GetStream();

                        // Write the data to the client
                        stream.Write(msg, 0, msg.Length);
                    }
                    catch(Exception e)
                    {
                        Debug.WriteLine("Error writing the TCP port", e);
                    }
                }
                else
                {
                    // Remove the client, not connected anymore
                    clientRemoveList.Add(client);
                }
            }

            // Remove all bad connections
            foreach(var client in clientRemoveList)
            {
                _tcpClients.Remove(client);
            }
        }

        /// <summary>
        /// Check if the given client is still connected.
        /// </summary>
        /// <param name="client">TCP Client to check.</param>
        /// <returns>TRUE if connected still.</returns>
        private bool IsClientConnected(TcpClient client)
        {
            try
            {
                if (client != null && client.Client != null && client.Client.Connected)
                {
                    /* pear to the documentation on Poll:
                     * When passing SelectMode.SelectRead as a parameter to the Poll method it will return 
                     * -either- true if Socket.Listen(Int32) has been called and a connection is pending;
                     * -or- true if data is available for reading; 
                     * -or- true if the connection has been closed, reset, or terminated; 
                     * otherwise, returns false
                     */

                    // Detect if client disconnected
                    if (client.Client.Poll(0, SelectMode.SelectRead))
                    {
                        byte[] buff = new byte[1];
                        if (client.Client.Receive(buff, SocketFlags.Peek) == 0)
                        {
                            // Client disconnected
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
