using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RTI
{
    /// <summary>
    /// Object to hold the received data.
    /// </summary>
    public struct Received
    {
        /// <summary>
        /// Sender of the data.
        /// </summary>
        public IPEndPoint Sender;
        
        /// <summary>
        /// JSON message.
        /// </summary>
        public string Message;

        // <summary>
        // Data received.
        // </summary>
        //public byte[] RawMessage;
    }

    /// <summary>
    /// Base class for a UDP port.
    /// </summary>
    abstract class UdpBase
    {
        /// <summary>
        /// UDP connection.
        /// </summary>
        protected UdpClient Client;

        /// <summary>
        /// Create a connection to a UDP port.
        /// </summary>
        protected UdpBase()
        {
            Client = new UdpClient();
        }

        /// <summary>
        /// Receive data from the UDP port async.
        /// This will generate a Receive object with the data.
        /// </summary>
        /// <returns>Received message.</returns>
        public async Task<Received> Receive()
        {
            var result = await Client.ReceiveAsync();
            return new Received()
            {
                //RawMessage = result.Buffer,
                Message = Encoding.ASCII.GetString(result.Buffer, 0, result.Buffer.Length),
                Sender = result.RemoteEndPoint
            };
        }
    }

    /// <summary>
    /// UDP Server to listen for messages.
    /// </summary>
    class UdpListener : UdpBase
    {
        /// <summary>
        /// IP address and port to listen for data.
        /// </summary>
        private IPEndPoint _listenOn;

        /// <summary>
        /// Create a UDP server to listen for data on the given port.
        /// </summary>
        /// <param name="port">Port to listen for data.</param>
        public UdpListener(int port)
            : this(new IPEndPoint(IPAddress.Any, port))
        {
        }

        /// <summary>
        /// Create a UDP server to listen for data on the given endpoint.
        /// </summary>
        /// <param name="endpoint">Ip address and port.</param>
        public UdpListener(IPEndPoint endpoint)
        {
            _listenOn = endpoint;
            Client = new UdpClient(_listenOn);
        }

        /// <summary>
        /// Send a message to the UDP port.
        /// </summary>
        /// <param name="message">Message to send.</param>
        /// <param name="endpoint">IP address and port to send the data.</param>
        public void Reply(string message, IPEndPoint endpoint)
        {
            var datagram = Encoding.ASCII.GetBytes(message);
            Client.Send(datagram, datagram.Length, endpoint);
        }

    }

    /// <summary>
    /// UDP Client to send messages and receive messages.
    /// </summary>
    class UdpUser : UdpBase
    {
        /// <summary>
        /// Constructor. Set private so the ConnectTo function must be called.
        /// </summary>
        private UdpUser() 
        { 
        
        }

        /// <summary>
        /// Connect to the UDP port with the given hostname and port.
        /// Typically the hostname is localhost.
        /// </summary>
        /// <param name="hostname">Hostname or IP address.</param>
        /// <param name="port">UDP port.</param>
        /// <returns>Connection made.</returns>
        public static UdpUser ConnectTo(string hostname, int port)
        {
            var connection = new UdpUser();
            connection.Client.Connect(hostname, port);
            return connection;
        }

        /// <summary>
        /// Connect to the UDP port with the given hostname and port.
        /// Typically the hostname is localhost.
        /// </summary>
        /// <param name="port">UDP port.</param>
        /// <returns>Connection made.</returns>
        public static UdpUser ConnectTo(int port)
        {
            var connection = new UdpUser();
            connection.Client.Connect("127.0.0.1", port);
            return connection;
        }

        /// <summary>
        /// Send the data to the UDP port.
        /// </summary>
        /// <param name="message">Message to send.</param>
        public void Send(string message)
        {
            var datagram = Encoding.ASCII.GetBytes(message);
            Client.Send(datagram, datagram.Length);
        }

        /// <summary>
        /// Send the data to the UDP port.
        /// </summary>
        /// <param name="message">Message to send.</param>
        public void Send(byte[] message)
        {
            var datagram = message;
            Client.Send(datagram, datagram.Length);
        }

    }
}


//
//class Program
//{
//    static void Main(string[] args)
//    {
//        //create a new server
//        var server = new UdpListener();

//        //start listening for messages and copy the messages back to the client
//        Task.Factory.StartNew(async () =>
//        {
//            while (true)
//            {
//                var received = await server.Receive();
//                server.Reply("copy " + received.Message, received.Sender);
//                if (received.Message == "quit")
//                    break;
//            }
//        });

//        //create a new client
//        var client = UdpUser.ConnectTo("127.0.0.1", 32123);

//        //wait for reply messages from server and send them to console 
//        Task.Factory.StartNew(async () =>
//        {
//            while (true)
//            {
//                try
//                {
//                    var received = await client.Receive();
//                    Console.WriteLine(received.Message);
//                    if (received.Message.Contains("quit"))
//                        break;
//                }
//                catch (Exception ex)
//                {
//                    Debug.Write(ex);
//                }
//            }
//        });

//        //type ahead :-)
//        string read;
//        do
//        {
//            read = Console.ReadLine();
//            client.Send(read);
//        } while (read != "quit");
//    }
//}
