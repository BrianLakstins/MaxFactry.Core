// <copyright file="MaxSecurityLibraryDefaultProvider.cs" company="Lakstins Family, LLC">
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
// <change date="3/1/2022" author="Brian A. Lakstins" description="Initial creation">
// </changelog>
#endregion

namespace MaxFactry.Core.Provider
{
	using System;
    using System.IO;

	/// <summary>
    /// Defines the default class for implementing the MaxSecurityLibrary functionality
	/// </summary>
    public class MaxSecurityLibraryDefaultProvider : MaxProvider, IMaxSecurityLibraryProvider
	{
        /// <summary>
        /// Gets and access token
        /// </summary>
        /// <param name="lsClientId">client_id</param>
        /// <param name="lsClientSecret">client_secrey</param>
        /// <param name="loTokenUrl">URL to get token</param>
        /// <returns></returns>
		public virtual string GetAccessToken(Uri loTokenUrl, string lsClientId, string lsClientSecret, string lsScope)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets and access token
        /// </summary>
        /// <param name="loTokenUrl">URL to get token</param>
        /// <param name="loCredentialIndex">Content to send</param>
        /// <returns></returns>
		public virtual string GetAccessToken(Uri loTokenUrl, MaxIndex loCredentialIndex)
        {
            throw new NotImplementedException();
        }
    }
}
