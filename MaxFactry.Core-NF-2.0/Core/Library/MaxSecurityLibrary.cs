// <copyright file="MaxSecurityLibrary.cs" company="Lakstins Family, LLC">
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
// <change date="3/3/2022" author="Brian A. Lakstins" description="Initial creation">
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
	public class MaxSecurityLibrary : MaxByMethodFactory
	{
        /// <summary>
        /// Internal storage of single object
        /// </summary>
        private static MaxSecurityLibrary _oInstance = null;

        /// <summary>
        /// Lock object for multi-threaded access.
        /// </summary>
        private static object _oLock = new object();

        /// <summary>
        /// Gets the single instance of this class.
        /// </summary>
        public static MaxSecurityLibrary Instance
        {
            get
            {
                if (null == _oInstance)
                {
                    lock (_oLock)
                    {
                        if (null == _oInstance)
                        {
                            _oInstance = new MaxSecurityLibrary();
                        }
                    }
                }

                return _oInstance;
            }
        }

        /// <summary>
        /// Gets and access token
        /// </summary>
        /// <param name="loType">type of object making the call</param>
        /// <param name="lsClientId">client_id</param>
        /// <param name="lsClientSecret">client_secrey</param>
        /// <param name="loTokenUrl">URL to get token</param>
        /// <returns></returns>
        public static string GetAccessToken(Type loType, Uri loTokenUrl, string lsClientId, string lsClientSecret, string lsScope)
        {
            IMaxProvider loBaseProvider = Instance.GetProvider(typeof(MaxFactry.Core.Provider.MaxSecurityLibraryDefaultProvider));
            IMaxSecurityLibraryProvider loProvider = loBaseProvider as IMaxSecurityLibraryProvider;          
            if (null == loProvider)
            {
                throw new MaxException("Error casting [" + loBaseProvider.GetType() + "] for Provider");
            }

            return loProvider.GetAccessToken(loTokenUrl, lsClientId, lsClientSecret, lsScope);
        }

        /// <summary>
        /// Gets and access token
        /// </summary>
        /// <param name="loType">type of object making the call</param>
        /// <param name="lsClientId">client_id</param>
        /// <param name="lsClientSecret">client_secrey</param>
        /// <param name="loTokenUrl">URL to get token</param>
        /// <returns></returns>
        public static string GetAccessToken(Type loType, Uri loTokenUrl, MaxIndex loCredentialIndex)
        {
            IMaxProvider loBaseProvider = Instance.GetProvider(typeof(MaxFactry.Core.Provider.MaxSecurityLibraryDefaultProvider));
            IMaxSecurityLibraryProvider loProvider = loBaseProvider as IMaxSecurityLibraryProvider;
            if (null == loProvider)
            {
                throw new MaxException("Error casting [" + loBaseProvider.GetType() + "] for Provider");
            }

            return loProvider.GetAccessToken(loTokenUrl, loCredentialIndex);
        }
    }
}