// <copyright file="MaxSingleFactory.cs" company="Lakstins Family, LLC">
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
// <change date="6/5/2020" author="Brian A. Lakstins" description="Lazy load the base provider in reverse order from provider configuration.">
// </changelog>
#endregion

namespace MaxFactry.Core
{
	using System;
	using System.IO;
	using System.Text;

	/// <summary>
	/// Base class when only one provider is used for static methods.
	/// </summary>
	public abstract class MaxSingleFactory
	{
        /// <summary>
        /// Internal storage of a Provider that implements the IMaxDataLibraryProvider interface.
        /// </summary>
        private IMaxProvider _oBaseProvider = null;

        private MaxIndex _oProviderIndex = new MaxIndex();

        /// <summary>
        /// Object used to lock the thread for changes to index
        /// </summary>
        private object _oLock = new object();

        /// <summary>
        /// Gets or sets the provider
        /// </summary>
        protected IMaxProvider BaseProvider
        {
            get
            {
                if (null == this._oBaseProvider)
                {
                    string[] laKey = this._oProviderIndex.GetSortedKeyList();
                    for (int lnK = laKey.Length - 1; lnK >= 0 && null == this._oBaseProvider; lnK--)
                    {
                        string lsKey = laKey[lnK];
                        Type loProviderType = _oProviderIndex[lsKey] as Type;
                        if (null != loProviderType)
                        {
                            object loProvider = MaxFactryLibrary.CreateProvider(loProviderType);
                            if (loProvider is IMaxProvider)
                            {
                                this._oBaseProvider = (IMaxProvider)loProvider;
                            }
                            else
                            {
                                throw new MaxException("Provider {" + loProvider.GetType() + "} for {" + this.GetType() + "} does not implement IMaxProvider interface.");
                            }
                        }
                    }
                }

                return this._oBaseProvider;
            }

            set
            {
                this._oBaseProvider = value;
            }
        }

        /// <summary>
        /// Adds a provider of the specified type to handle all methods.
        /// </summary>
        /// <param name="loProviderType">The provider to use</param>
        public void ProviderSet(Type loProviderType)
        {
            this._oBaseProvider = null;
            this._oProviderIndex.Add(loProviderType);
        }

        /// <summary>
        /// Sets the internal provider based on the settings
        /// </summary>
        /// <param name="loType">Type to use to determine the provider.</param>
        protected virtual void SetProvider(Type loType)
        {
            this._oProviderIndex.Add(loType);
        }
    }
}