// <copyright file="MaxCompressionLibrary.cs" company="Lakstins Family, LLC">
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
	/// Library to provide encryption functionality
	/// </summary>
	public class MaxCompressionLibrary : MaxByMethodFactory
	{
        /// <summary>
        /// Internal storage of single object
        /// </summary>
        private static MaxCompressionLibrary _oInstance = null;

        /// <summary>
        /// Lock object for multi-threaded access.
        /// </summary>
        private static object _oLock = new object();

        /// <summary>
        /// Gets the single instance of this class.
        /// </summary>
        public static MaxCompressionLibrary Instance
        {
            get
            {
                if (null == _oInstance)
                {
                    lock (_oLock)
                    {
                        if (null == _oInstance)
                        {
                            _oInstance = new MaxCompressionLibrary();
                        }
                    }
                }

                return _oInstance;
            }
        }

        /// <summary>
        /// Basic Compression using DeflateStream
        /// </summary>
        /// <param name="loType">Type of object to compress, or type of provider to use to compress</param>
        /// <param name="loValue">Data to be compressed.</param>
        /// <returns>Compressed version of the data.</returns>
        public static byte[] Compress(Type loType, byte[] loValue)
        {
            IMaxProvider loBaseProvider = Instance.GetProvider(loType);
            IMaxCompressionLibraryProvider loProvider = loBaseProvider as IMaxCompressionLibraryProvider;          
            if (null == loProvider)
            {
                throw new MaxException("Error casting [" + loBaseProvider.GetType() + "] for Provider");
            }

            return loProvider.Compress(loValue);
        }

        /// <summary>
        /// Decompression using DeflateStream
        /// </summary>
        /// <param name="loType">Type of object to decompress, or type of provider to use to decompress</param>
        /// <param name="loValue">Data to be decompressed.</param>
        /// <returns>Decompressed version of the data.</returns>
        public static byte[] Decompress(Type loType, byte[] loValue)
        {
            IMaxProvider loBaseProvider = Instance.GetProvider(loType);
            IMaxCompressionLibraryProvider loProvider = loBaseProvider as IMaxCompressionLibraryProvider;
            if (null == loProvider)
            {
                throw new MaxException("Error casting [" + loBaseProvider.GetType() + "] for Provider");
            }

            return loProvider.Decompress(loValue);
        }
	}
}