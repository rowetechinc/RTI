using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI
{
    class MemoryStreamExtensions : MemoryStream
    {
        /// <summary>
        /// Contructor that takes a capacity.
        /// </summary>
        /// <param name="cap">Capcity.</param>
        public MemoryStreamExtensions(int cap)
            :base(cap)
        {

        }

        /// <summary>
        /// Contructor that adds data.
        /// </summary>
        /// <param name="data">Byte array of data.</param>
        public MemoryStreamExtensions(byte[] data)
            : base()
        {
            // Write the data to the stream
            this.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Clear the stream.
        /// </summary>
        public void Clear()
        {
            this.SetLength(0);
        }

        /// <summary>
        /// Remaing values in the stream.
        /// </summary>
        /// <returns></returns>
        public long Remaining()
        {
            return this.Length - this.Position;
        }

        /// <summary>
        /// Read the data from the stream.
        /// </summary>
        /// <returns></returns>
        public byte ReadDataByte()
        {
            return Convert.ToByte(this.ReadByte());
        }

    }
}
