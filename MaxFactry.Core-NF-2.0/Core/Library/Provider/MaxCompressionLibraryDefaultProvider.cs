// <copyright file="MaxCompressionLibraryDefaultProvider.cs" company="Lakstins Family, LLC">
// Copyright (c) Brian A. Lakstins (http://www.lakstins.com/brian/)
// </copyright>

#region License
// <license>
// This software is provided 'as-is', without any express or implied warranty. In no 
// event will the author be held liable for any damages arising from the use of this 
// software.
//  
// Permission is granted to anyone to use this software for any purpose, including 
// commercial applications, and to alter it and redistribute it freely, subject to the 
// following restrictions:
// 
// 1. The origin of this software must not be misrepresented; you must not claim that 
// you wrote the original software. If you use this software in a product, an 
// acknowledgment (see the following) in the product documentation is required.
// 
// Portions Copyright (c) Brian A. Lakstins (http://www.lakstins.com/brian/)
// 
// 2. Altered source versions must be plainly marked as such, and must not be 
// misrepresented as being the original software.
// 
// 3. This notice may not be removed or altered from any source distribution.
// </license>
#endregion

#region Change Log
// <changelog>
// <change date="1/17/2017" author="Brian A. Lakstins" description="Initial creation">
// </changelog>
#endregion

namespace MaxFactry.Core.Provider
{
	using System;
    using System.IO;

	/// <summary>
    /// Defines the default class for implementing the MaxCompressionLibrary functionality
	/// </summary>
    public class MaxCompressionLibraryDefaultProvider : MaxProvider, IMaxCompressionLibraryProvider
	{
        /// <summary>
        /// Basic Compression using DeflateStream
        /// </summary>
        /// <param name="loValue">Data to be compressed.</param>
        /// <returns>Compressed version of the data.</returns>
        public virtual byte[] Compress(byte[] loValue)
        {
            if (null == loValue)
            {
                return null;
            }

            return this.CompressConditional(loValue);
        }

        /// <summary>
        /// Decompression using DeflateStream
        /// </summary>
        /// <param name="loValue">Data to be decompressed.</param>
        /// <returns>Decompressed version of the data.</returns>
        public virtual byte[] Decompress(byte[] loValue)
        {
            if (null == loValue)
            {
                return null;
            }

            return this.DecompressConditional(loValue);
        }

#if net2  || netcore1 || netstandard1_4
        /// <summary>
        /// Basic Compression using DeflateStream
        /// </summary>
        /// <param name="loValue">Data to be compressed.</param>
        /// <returns>Compressed version of the data.</returns>
        protected virtual byte[] CompressConditional(byte[] loValue)
        {
            MemoryStream loMemory = new MemoryStream();
            using (Stream loCompress = new System.IO.Compression.DeflateStream(loMemory, System.IO.Compression.CompressionMode.Compress))
            {
                loCompress.Write(loValue, 0, loValue.Length);
            }

            return loMemory.ToArray();
        }

        /// <summary>
        /// Decompression using DeflateStream
        /// </summary>
        /// <param name="loValue">Data to be decompressed.</param>
        /// <returns>Decompressed version of the data.</returns>
        protected virtual byte[] DecompressConditional(byte[] loValue)
        {
            MemoryStream loMemoryCompressed = new MemoryStream(loValue);
            Stream loDecompress = new System.IO.Compression.DeflateStream(loMemoryCompressed, System.IO.Compression.CompressionMode.Decompress);
            byte[] loBuffer = new byte[1024];
            MemoryStream loMemory = new MemoryStream();
            while (true)
            {
                int lnRead = loDecompress.Read(loBuffer, 0, loBuffer.Length);
                if (lnRead == 0)
                {
                    break;
                }

                loMemory.Write(loBuffer, 0, lnRead);
            }

            return loMemory.ToArray();
        }
#elif netstandard1_2
        /// <summary>
        /// Basic Compression using DeflateStream
        /// </summary>
        /// <param name="loValue">Data to be compressed.</param>
        /// <returns>Compressed version of the data.</returns>
        protected virtual byte[] CompressConditional(byte[] loValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Decompression using DeflateStream
        /// </summary>
        /// <param name="loValue">Data to be decompressed.</param>
        /// <returns>Decompressed version of the data.</returns>
        protected virtual byte[] DecompressConditional(byte[] loValue)
        {
            throw new NotImplementedException();
        }
#endif
    }
}
