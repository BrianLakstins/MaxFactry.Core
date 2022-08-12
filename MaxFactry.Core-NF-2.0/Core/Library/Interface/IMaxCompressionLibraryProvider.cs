// <copyright file="IMaxCompressionLibraryProvider.cs" company="Lakstins Family, LLC">
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
// <change date="1/3/2015" author="Brian A. Lakstins" description="Initial Release">
// </changelog>
#endregion

namespace MaxFactry.Core
{
	using System;
	using System.IO;
	using System.Text;

	/// <summary>
	/// Defines the base interface for compressing data.
	/// </summary>
    public interface IMaxCompressionLibraryProvider : IMaxProvider
    {
        /// <summary>
        /// Basic Compression using DeflateStream
        /// </summary>
        /// <param name="loValue">Data to be compressed.</param>
        /// <returns>Compressed version of the data.</returns>
        byte[] Compress(byte[] loValue);

        /// <summary>
        /// Decompression using DeflateStream
        /// </summary>
        /// <param name="loValue">Data to be decompressed.</param>
        /// <returns>Decompressed version of the data.</returns>
        byte[] Decompress(byte[] loValue);
    }
}
