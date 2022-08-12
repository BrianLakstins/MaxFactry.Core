// <copyright file="IMaxMetaLibraryProvider.cs" company="Lakstins Family, LLC">
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
// <change date="6/8/2016" author="Brian A. Lakstins" description="Initial creation">
// </changelog>
#endregion

namespace MaxFactry.Core
{
	using System;
	using System.IO;
	using System.Text;

	/// <summary>
	/// Defines the base interface for encrypting data
	/// </summary>
	public interface IMaxMetaLibraryProvider : IMaxProvider
	{
        /// <summary>
        /// Gets the key for the property
        /// </summary>
        /// <param name="loObject">Object that contains the property</param>
        /// <param name="lsProperty">Name of the property</param>
        /// <returns>Key to use for storing meta data related to the property</returns>
        string GetKey(object loObject, string lsProperty);

        /// <summary>
        /// Gets some text related to the property
        /// </summary>
        /// <param name="loObject">Object that contains the property</param>
        /// <param name="lsProperty">Name of the property</param>
        /// <param name="lsMetaName">Name of the text to get for the property</param>
        /// <returns>String of text related to the property</returns>
        string GetMetaText(object loObject, string lsProperty, string lsMetaName);

        /// <summary>
        /// Gets a list of properties that have some kind of meta display information associated with them
        /// </summary>
        /// <param name="loObject">Object that contains the properties</param>
        /// <returns>List of property names</returns>
        MaxIndex GetPropertyDisplayList(object loObject);
	}
}
