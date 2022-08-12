// <copyright file="MaxByMethodFactory.cs" company="Lakstins Family, LLC">
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
// <change date="1/28/2016" author="Brian A. Lakstins" description="Update adding a provider so a null provider removes the provider.">
// </changelog>
#endregion

namespace MaxFactry.Core
{
	using System;
	using System.IO;
	using System.Text;

	/// <summary>
    /// Base class when multiple providers are used for static methods depending on an object passed to the method.
    /// Each provider is created as a singleton provider, so they need to be thread safe.
	/// </summary>
	public abstract class MaxByMethodFactory
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
        /// Default provider type to use.
        /// </summary>
        private Type _oDefaultProviderType = null;

        /// <summary>
        /// Gets or sets the default provider type for the factory.
        /// </summary>
        protected Type DefaultProviderType
        {
            get
            {
                return this._oDefaultProviderType;
            }

            set
            {
                this._oDefaultProviderType = value;
            }
        }

        /// <summary>
        /// Handles throwing an error when an Interface is not implemented in a provider.
        /// </summary>
        /// <param name="loType">Type used to determine the provider.</param>
        /// <param name="loProvider">The provider created.</param>
        /// <param name="lsClass">The class creating the provider.</param>
        /// <param name="lsInterface">The interface the provider should implement.</param>
        public static void HandleInterfaceNotImplemented(Type loType, IMaxProvider loProvider, string lsClass, string lsInterface)
        {
            if (null == loProvider)
            {
                throw new MaxException("Provider for " + lsClass + " for [" + loType.ToString() + "] is null");
            }

            throw new MaxException("Provider for " + lsClass + " [ " + loProvider.GetType().ToString() + "] for [" + loType.ToString() + "] needs to implement " + lsInterface);
        }

        /// <summary>
        /// Gets the provider based on a type
        /// </summary>
        /// <param name="loDefault">Default type of provider to use.</param>
        /// <returns>A provider for an object of that specified type</returns>
        public virtual IMaxProvider GetProvider(Type loDefault)
        {
            return this.GetProvider(null, loDefault);
        }

        /// <summary>
        /// Gets the provider based on a key
        /// </summary>
        /// <param name="lsKey">Key used to identify the provider.</param>
        /// <param name="loDefault">Default type of provider to use.</param>
        /// <returns>A provider for an object of that specified type</returns>
        public virtual IMaxProvider GetProvider(string lsKey, Type loDefault)
        {
            Type loProviderType = loDefault;
            if (null != lsKey && this._oProviderMapIndex.Contains(lsKey))
            {
                loProviderType = this._oProviderMapIndex[lsKey] as Type;
            }
            else if (null != loDefault && this._oProviderMapIndex.Contains(loDefault.ToString()))
            {
                loProviderType = this._oProviderMapIndex[loDefault.ToString()] as Type;
            }

            object loProvider = null;
            if (null != loProviderType)
            {
                loProvider = MaxFactryLibrary.CreateSingletonProvider(loProviderType);
            }

            if (null == loProvider)
            {
                if (this._oProviderMapIndex.Contains(typeof(object).ToString()))
                {
                    loProviderType = this._oProviderMapIndex[typeof(object).ToString()] as Type;
                    if (null != loProviderType)
                    {
                        loProvider = MaxFactryLibrary.CreateSingletonProvider(loProviderType);
                    }
                }
            }

            if (null == loProvider && null != this.DefaultProviderType)
            {
                loProvider = MaxFactryLibrary.CreateSingletonProvider(this.DefaultProviderType);
            }

            return loProvider as IMaxProvider;
        }

        /// <summary>
        /// Gets the provider based on a key
        /// </summary>
        /// <param name="lsName">Name of the provider to use.</param>
        /// <param name="loDefault">Default type of provider to use.</param>
        /// <returns>A provider for an object of that specified type</returns>
        public virtual IMaxProvider GetProviderByName(string lsName, Type loDefault)
        {
            Type loProviderType = loDefault;
            string lsKey = lsName + ":" + loProviderType.ToString();
            if (null != lsKey && this._oProviderMapIndex.Contains(lsKey))
            {
                loProviderType = this._oProviderMapIndex[lsKey] as Type;
            }
            else if (null != lsName && this._oProviderMapIndex.Contains(lsName))
            {
                loProviderType = this._oProviderMapIndex[lsName] as Type;
            }
            else if (null != loDefault && this._oProviderMapIndex.Contains(loDefault.ToString()))
            {
                loProviderType = this._oProviderMapIndex[loDefault.ToString()] as Type;
            }

            object loProvider = null;
            if (null != loProviderType)
            {
                loProvider = MaxFactryLibrary.CreateSingletonProvider(lsName, loProviderType);
            }

            if (null == loProvider)
            {
                if (this._oProviderMapIndex.Contains(typeof(object).ToString()))
                {
                    loProviderType = this._oProviderMapIndex[typeof(object).ToString()] as Type;
                    if (null != loProviderType)
                    {
                        loProvider = MaxFactryLibrary.CreateSingletonProvider(lsName, loProviderType);
                    }
                }
            }

            if (null == loProvider && null != this.DefaultProviderType)
            {
                loProvider = MaxFactryLibrary.CreateSingletonProvider(this.DefaultProviderType);
            }

            return loProvider as IMaxProvider;
        }

		/// <summary>
		/// Maps a class that uses a specific provider to that provider
		/// </summary>
        /// <param name="lsKey">Key used to identify the provider</param>
		/// <param name="loProviderType">The provider to use</param>
		public void ProviderAdd(string lsKey, Type loProviderType)
		{
			lock (this._oLock)
			{
                if (null != loProviderType)
                {
                    this._oProviderMapIndex.Add(lsKey, loProviderType);
                }
                else
                {
                    this._oProviderMapIndex.Remove(lsKey);
                }
			}
		}

        /// <summary>
        /// Maps a class that uses a specific provider to that provider
        /// </summary>
        /// <param name="loProviderType">The provider to use</param>
        public void ProviderAdd(Type loProviderType)
        {
            lock (this._oLock)
            {
                this._oProviderMapIndex.Add(typeof(object).ToString(), loProviderType);
            }
        }
	}
}