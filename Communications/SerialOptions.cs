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
 * Date            Initials               Comments
 * -----------------------------------------------------------------
 * 09/01/2011      RC                     Initial coding
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
        /// <summary>
        /// Constructor.
        /// Initialize the values.
        /// </summary>
        public SerialOptions()
        {
            // Initialize lists
            PortOptions = new List<string>();
            BaudRateOptions = new List<int>();
            DataBitsOptions = new List<int>();
            ParityOptions = new List<System.IO.Ports.Parity>();
            StopBitsOptions = new List<System.IO.Ports.StopBits>();

            // Set the ranges for all the option lists
            SetPortOptions();
            SetBaudRateOptions();
            SetDataBitsOptions();
            SetParityOptions();
            SetStopBitsOptions();

            // Set default ranges
            SetDefaultValues();
        }

        // This region contains all the properties for this model
        #region properties
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

        /// <summary>
        /// Store a list of Serial port COMM port options.
        /// This will first determine which ports are available
        /// then populate the list.
        /// </summary>
        public List<string> PortOptions { get; set; }


        /// <summary>
        /// Store a list of possible serial port baud rates.
        /// The list will contain ints.
        /// </summary>
        public List<int> BaudRateOptions { get; set; }

        /// <summary>
        /// Store a list of possible data bits for
        /// the serial port.  This will contain a list
        /// of ints.
        /// </summary>
        public List<int> DataBitsOptions { get; set; }

        /// <summary>
        /// Store a list of possible _parity options.  This will
        /// contain a list of System.IO.Ports.Parity.
        /// </summary>
        public List<System.IO.Ports.Parity> ParityOptions { get; set; }

        /// <summary>
        /// Store a list of possible Stop bits. This will
        /// contain a list of System.IO.Ports.StopBits.
        /// </summary>
        public List<System.IO.Ports.StopBits> StopBitsOptions { get; set; }

        #endregion

        // This region contains all the methods for this model
        #region methods
        /// <summary>
        /// Give a string representation for all the settings.
        /// </summary>
        /// <returns>A string representings all the settings.</returns>
        public override string ToString()
        {
            return Port + ": " + BaudRate + " " + DataBits + " " + Parity.ToString() + " " + StopBits.ToString();
        }

        /// <summary>
        /// Set default ranges for all the parameters.
        /// </summary>
        private void SetDefaultValues()
        {
            // Set a COMM port option if one is available
            if (PortOptions.Count <= 0)
            {
                Port = "";
            }
            else
            {
                Port = PortOptions[0];
            }
            BaudRate = 115200;
            DataBits = 8;
            Parity = System.IO.Ports.Parity.None;
            StopBits = System.IO.Ports.StopBits.One;
        }

        /// <summary>
        /// Create a list of COM ports options.
        /// </summary>
        public void SetPortOptions()
        {
            // Get all the available COM ports
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();

            if (PortOptions.Count > 0 )
            {
                PortOptions.Clear();
            }

            // Populate the list with all available COM ports
            foreach (string port in ports)
            {
                PortOptions.Add(port);
            }
        }

        /// <summary>
        /// Create a list of baud rates options.
        /// </summary>
        private void SetBaudRateOptions()
        {
            BaudRateOptions.Add(2400);
            BaudRateOptions.Add(4800);
            BaudRateOptions.Add(9600);
            BaudRateOptions.Add(19200);
            BaudRateOptions.Add(38400);
            BaudRateOptions.Add(115200);
        }

        /// <summary>
        /// Create a list of Data bit options.
        /// </summary>
        private void SetDataBitsOptions()
        {
            //_dataBitsOptions.Add(5);
            //_dataBitsOptions.Add(6);
            DataBitsOptions.Add(7);
            DataBitsOptions.Add(8);
        }

        /// <summary>
        /// Create a list of Parity options.
        /// </summary>
        private void SetParityOptions()
        {
            ParityOptions.Add(System.IO.Ports.Parity.None);
            ParityOptions.Add(System.IO.Ports.Parity.Even);
            ParityOptions.Add(System.IO.Ports.Parity.Odd);
            ParityOptions.Add(System.IO.Ports.Parity.Mark);
            ParityOptions.Add(System.IO.Ports.Parity.Space);
        }

        /// <summary>
        /// Create a list of Stop Bit options.
        /// </summary>
        private void SetStopBitsOptions()
        {
            StopBitsOptions.Add(System.IO.Ports.StopBits.None);
            StopBitsOptions.Add(System.IO.Ports.StopBits.One);
            StopBitsOptions.Add(System.IO.Ports.StopBits.OnePointFive);
            StopBitsOptions.Add(System.IO.Ports.StopBits.Two);
        }
        #endregion
    }
}