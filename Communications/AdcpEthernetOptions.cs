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
 * 03/14/2013      RC          2.19       Initial coding
 * 07/22/2014      RC          2.23.0     Added Port property.
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// Ethernet options.
    /// </summary>
    public class AdcpEthernetOptions
    {
        #region Properties

        /// <summary>
        /// IP Address Part A.
        /// A.B.C.D
        /// </summary>
        public int IpAddrA { get; set; }

        /// <summary>
        /// IP Address Part B.
        /// A.B.C.D
        /// </summary>
        public int IpAddrB { get; set; }

        /// <summary>
        /// IP Address Part C.
        /// A.B.C.D
        /// </summary>
        public int IpAddrC { get; set; }

        /// <summary>
        /// IP Address Part D.
        /// A.B.C.D
        /// </summary>
        public int IpAddrD { get; set; }

        /// <summary>
        /// Port number.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Return the address as a string.
        /// A.B.C.D
        /// </summary>
        [JsonIgnore]
        public string IpAddr
        {
            get
            {
                return IpAddrA + "." +
                      IpAddrB + "." +
                      IpAddrC + "." +
                      IpAddrD;
            }
        }

        /// <summary>
        /// Options for the ethernet port.
        /// </summary>
        public AdcpEthernetOptions()
        {
            IpAddrA = 191;
            IpAddrB = 168;
            IpAddrC = 1;
            IpAddrD = 130;
            Port = 100;
        }

        #endregion

    }
}
