// <copyright file="MaxMultipleFactory.cs" company="Lakstins Family, LLC">
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
// <change date="8/24/2014" author="Brian A. Lakstins" description="Initial Release">
// </changelog>
#endregion

namespace MaxFactry.Core
{
	using System;
	using System.IO;
	using System.Text;

	/// <summary>
	/// Base class when multiple providers can execute methods.
	/// </summary>
	public abstract class MaxMultipleFactory
	{
		/// <summary>
		/// Maps classes that use a provider to the class that is the provider
		/// </summary>
		private MaxIndex _oProviderMapIndex = new MaxIndex();

		/// <summary>
		/// Object used to lock the thread for changes to index
		/// </summary>
		private object _oLock = new object();

		/// <summary>
		/// Gets the list of providers that have been registered.
		/// </summary>
        /// <returns>List of registered providers.</returns>
		public IMaxProvider[] GetProviderList()
		{
            IMaxProvider[] loList = new IMaxProvider[this._oProviderMapIndex.Count];
            string[] laKey = this._oProviderMapIndex.GetSortedKeyList();
            for (int lnK = 0; lnK < laKey.Length; lnK++)
            {
                object loType = this._oProviderMapIndex[laKey[lnK]];
                object loProvider = MaxFactryLibrary.CreateSingletonProvider((Type)loType);
                loList[lnK] = (IMaxProvider)loProvider;
            }

            return loList;
		}

        /// <summary>
        /// Adds a provider of the specified type to handle all methods.
        /// </summary>
        /// <param name="loProviderType">The provider to use</param>
        public void ProviderAdd(Type loProviderType)
        {
            if (!this._oProviderMapIndex.Contains(loProviderType.ToString()))
            {
                lock (this._oLock)
                {
                    if (!this._oProviderMapIndex.Contains(loProviderType.ToString()))
                    {
                        this._oProviderMapIndex.Add(loProviderType.ToString(), loProviderType);
                    }
                }
            }
        }

        /// <summary>
        /// Clears all providers.
        /// </summary>
        public void ProviderClear()
        {
            lock (this._oLock)
            {
                this._oProviderMapIndex.Clear();
            }
        }
	}
}