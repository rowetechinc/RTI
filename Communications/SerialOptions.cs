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
 * 02/02/2012      RC          2.0        Added additional baudrate options. 
 * 07/03/2012      RC          2.12       Made the object smaller.  Make the options static.
 */

using System.Collections.Generic;


namespace RTI
{
    /// <summary>
    /// All the options for a Serial Port.
    /// Set the options required for a serial port.
    /// Default ranges will be set when constructed.
    /// There is also a toString() method that will
    /// include all the settings set.
    /// </summary>
    public class SerialOptions
    {

        #region Static Properties

        /// <summary>
        /// Store a list of Serial port COMM port options.
        /// This will first determine which ports are available
        /// then populate the list.
        /// </summary>
        public static List<string> PortOptions
        {
            get
            {
                // Get all the available COM ports
                string[] ports = System.IO.Ports.SerialPort.GetPortNames();

                // Populate the list with all available COM ports
                List<string> list = new List<string>();
                foreach (string port in ports)
                {
                    list.Add(port);
                }

                return list;
            }
        }

        /// <summary>
        /// Store a list of possible serial port baud rates.
        /// The list will contain ints.
        /// </summary>
        public static List<int> BaudRateOptions
        {
            get
            {
                List<int> list = new List<int>();
                list.Add(921600);
                list.Add(460800);
                list.Add(230400);
                list.Add(115200);
                list.Add(38400);
                list.Add(19200);
                list.Add(9600);
                list.Add(4800);
                list.Add(2400);
                return list;
            }
        }

        /// <summary>
        /// Store a list of possible data bits for
        /// the serial port.  This will contain a list
        /// of ints.
        /// </summary>
        public static List<int> DataBitsOptions
        {
            get
            {
                List<int> list = new List<int>();
                //list.Add(5);
                //list.Add(6);
                list.Add(7);
                list.Add(8);
                return list;
            }
        }

        /// <summary>
        /// Store a list of possible _parity options.  This will
        /// contain a list of System.IO.Ports.Parity.
        /// </summary>
        public static List<System.IO.Ports.Parity> ParityOptions
        {
            get
            {
                List<System.IO.Ports.Parity> list = new List<System.IO.Ports.Parity>();
                list.Add(System.IO.Ports.Parity.None);
                list.Add(System.IO.Ports.Parity.Even);
                list.Add(System.IO.Ports.Parity.Odd);
                list.Add(System.IO.Ports.Parity.Mark);
                list.Add(System.IO.Ports.Parity.Space);
                return list;
            }
        }

        /// <summary>
        /// Store a list of possible Stop bits. This will
        /// contain a list of System.IO.Ports.StopBits.
        /// </summary>
        public static List<System.IO.Ports.StopBits> StopBitsOptions
        {
            get
            {
                List<System.IO.Ports.StopBits> list = new List<System.IO.Ports.StopBits>();
                list.Add(System.IO.Ports.StopBits.None);
                list.Add(System.IO.Ports.StopBits.One);
                list.Add(System.IO.Ports.StopBits.OnePointFive);
                list.Add(System.IO.Ports.StopBits.Two);
                return list;
            }
        }

        #endregion

        #region Properties
        /// <summary>
        /// Store the COMM Port value as a string.
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// Store the serial port baud rate as an int.
        /// </summary>
        public int BaudRate { get; set; }

        /// <summary>
        /// Store the serial port data bits as an int.
        /// </summary>
        public int DataBits { get; set; }

        /// <summary>
        /// Store the serial port parity as a System.IO.Ports.Parity.
        /// </summary>
        public System.IO.Ports.Parity Parity { get; set; }

        /// <summary>
        /// Store the serial port Stop bits as a System.IO.Ports.StopBits.
        /// </summary>
        public System.IO.Ports.StopBits StopBits { get; set; }        

        #endregion

        /// <summary>
        /// Constructor.
        /// Initialize the values.
        /// </summary>
        public SerialOptions()
        {
            // Set default ranges
            SetDefaultValues();
        }

        #region Methods

        /// <summary>
        /// Set default ranges for all the parameters.
        /// </summary>
        private void SetDefaultValues()
        {
            // Get the port options
            List<string> portOptions = SerialOptions.PortOptions;

            // Set a COMM port option if one is available
            if (portOptions.Count <= 0)
            {
                Port = "";
            }
            else
            {
                Port = portOptions[0];
            }
            BaudRate = 115200;
            DataBits = 8;
            Parity = System.IO.Ports.Parity.None;
            StopBits = System.IO.Ports.StopBits.One;
        }

        /// <summary>
        /// Give a string representation for all the settings.
        /// </summary>
        /// <returns>A string representings all the settings.</returns>
        public override string ToString()
        {
            return Port + ": " + BaudRate + " " + DataBits + " " + Parity.ToString() + " " + StopBits.ToString();
        }

        #endregion
    }
}