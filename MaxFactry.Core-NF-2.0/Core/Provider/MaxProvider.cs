// <copyright file="MaxProvider.cs" company="Lakstins Family, LLC">
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
// <change date="1/28/2015" author="Brian A. Lakstins" description="Add another level to set the name of the provider.">
// <change date="7/2/2016" author="Brian A. Lakstins" description="Restructured how providers access configuration information.">
// <change date="5/25/2017" author="Brian A. Lakstins" description="Updated to use new FindValue method of MaxIndex to reduce memory usage when creating singleton providers.">
// <change date="6/4/2020" author="Brian A. Lakstins" description="Change method for getting configuration to GetConfigValue.  Prevent using MaxConfigurationLibrary for configuration if the provider implements IMaxConfigurationLibrary or IMaxFactryLibrary.">
// </changelog>
#endregion

namespace MaxFactry.Core
{
	using System;
	using System.IO;
    using System.Reflection;
    using System.Text;

	/// <summary>
	/// Base class for any provider.
	/// </summary>
    public abstract class MaxProvider : IMaxProvider
	{
        /// <summary>
        /// List of base class types
        /// </summary>
        private MaxIndex _oBaseList = null;

        /// <summary>
        /// Configuration related to base class types
        /// </summary>
        private MaxIndex _oBaseConfig = null;

        /// <summary>
        /// Gets or sets the Name of the provider.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the list of base names for configuration from the this type down to system.object.
        /// </summary>
        protected MaxIndex BaseList
        {
            get
            {
                if (null == this._oBaseList)
                {
                    this._oBaseList = new MaxIndex();
                    Type loType = this.GetType();
                    this._oBaseList.Add(loType.ToString());
                    while (null != MaxFactryLibrary.GetBaseType(loType))
                    {
                        this._oBaseList.Add(MaxFactryLibrary.GetBaseType(loType).ToString());
                        loType = MaxFactryLibrary.GetBaseType(loType);
                    }
                }

                return this._oBaseList;
            }
        }

        /// <summary>
        /// Gets the configuration for the base class types
        /// </summary>
        protected MaxIndex BaseConfig
        {
            get
            {
                if (null == this._oBaseConfig)
                {
                    this._oBaseConfig = new MaxIndex();
                    string[] laBaseKey = this.BaseList.GetSortedKeyList();
                    foreach (string lsBaseKey in laBaseKey)
                    {
                        string lsBase = this.BaseList[lsBaseKey] as string;
                        //// Check the named base configuration
                        MaxIndex loBaseNamedConfig = MaxFactryLibrary.GetValue(this.Name + ":" + lsBase + "-Config") as MaxIndex;
                        if (null != loBaseNamedConfig)
                        {
                            string[] laKey = loBaseNamedConfig.GetSortedKeyList();
                            foreach (string lsKey in laKey)
                            {
                                if (!this._oBaseConfig.Contains(lsKey))
                                {
                                    this._oBaseConfig.Add(lsKey, loBaseNamedConfig[lsKey]);
                                }
                            }
                        }

                        MaxIndex loBaseConfig = MaxFactryLibrary.GetValue(lsBase + "-Config") as MaxIndex;
                        if (null != loBaseConfig)
                        {
                            string[] laKey = loBaseConfig.GetSortedKeyList();
                            foreach (string lsKey in laKey)
                            {
                                if (!this._oBaseConfig.Contains(lsKey))
                                {
                                    this._oBaseConfig.Add(lsKey, loBaseConfig[lsKey]);
                                }
                            }
                        }
                    }
                }

                return this._oBaseConfig;
            }
        }

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="lsName">Name of the provider.</param>
        /// <param name="loConfig">Configuration information.</param>
        public virtual void Initialize(string lsName, MaxIndex loConfig)
        {
            this.Name = lsName;
            if (null == this.Name || this.Name.Length == 0)
            {
                this.Name = "Default";
            }

            string lsConfigName = this.GetConfigValue(loConfig, "MaxProviderName") as string;
            if (null != lsConfigName)
            {
                this.Name = lsConfigName;
            }
        }

        /// <summary>
        /// Gets the value of the configuration.  Can come from passed configuration, base configuration, global configuration, or Application Configuration
        /// </summary>
        /// <param name="loConfig">Passed configuration</param>
        /// <param name="lsKey">Key used to look up value</param>
        /// <returns>Some value or null if not found.</returns>
        protected object GetConfigValue(MaxIndex loConfig, string lsKey)
        {
            object loR = null;
            if (null != loConfig)
            {
                //// Check the passed configuration using the name of the provider
                loR = loConfig.FindValue(this.Name, ":", lsKey);
                if (loR is Guid && loConfig.NotFoundId.Equals((Guid)loR))
                {
                    //// Check the passed configuration without the name
                    loR = loConfig.FindValue(lsKey);
                    if (loR is Guid && loConfig.NotFoundId.Equals((Guid)loR))
                    {
                        loR = null;
                    }
                }

                if (null != loR)
                {
                    return loR;
                }
            }

            string lsNameKey = this.Name + ":" + lsKey;
            //// Don't use configuration when getting configuration for these
            if (!(this is IMaxConfigurationLibraryProvider) &&
                !(this is IMaxFactryLibraryProvider))
            {
                //// Check the application configuration with this provider name
                loR = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, lsNameKey);
                if (null != loR)
                {
                    return loR;
                }

                //// Check the application configuration
                loR = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, lsKey);
                if (null != loR)
                {
                    return loR;
                }
            }

            //// Check the base config with this provider name
            loR = this.BaseConfig[lsNameKey];
            if (null != loR)
            {
                return loR;
            }

            //// Check the base config just using the base key (object type)
            loR = this.BaseConfig[lsKey];
            if (null != loR)
            {
                return loR;
            }

            //// Check each base names of the configuration
            string[] laBaseListKey = this.BaseList.GetSortedKeyList();
            foreach (string lsBaseListKey in laBaseListKey)
            {
                string lsBase = this.BaseList[lsBaseListKey] as string;
                if (null != loConfig)
                {
                    //// Check the passed in config with this provider name
                    loR = loConfig.FindValue(this.Name, ":", lsBase, "-", lsKey);
                    if (loR is Guid && loConfig.NotFoundId.Equals((Guid)loR))
                    {
                        //// Check the passed in config just using the base key (object type)
                        loR = loConfig.FindValue(lsBase, "-", lsKey);
                        if (loR is Guid && loConfig.NotFoundId.Equals((Guid)loR))
                        {
                            loR = null;
                        }
                    }

                    if (null != loR)
                    {
                        return loR;
                    }
                }

                string lsBaseNameKey = this.Name + ":" + lsBase + "-" + lsKey;
                string lsBaseKey = lsBase + "-" + lsKey;
                //// Don't use configuration when getting configuration for these
                if (!(this is IMaxConfigurationLibraryProvider) &&
                    !(this is IMaxFactryLibraryProvider))
                {
                    //// Check the application configuration for the base with this provider name
                    loR = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, lsBaseNameKey);
                    if (null != loR)
                    {
                        return loR;
                    }

                    //// Check the application configuration using just the base key(object type)
                    loR = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, lsBaseKey);
                    if (null != loR)
                    {
                        return loR;
                    }
                }

                //// Check the base config with this provider name
                loR = this.BaseConfig[lsBaseNameKey];
                if (null != loR)
                {
                    return loR;
                }

                //// Check the base config just using the base key (object type)
                loR = this.BaseConfig[lsBaseKey];
                if (null != loR)
                {
                    return loR;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a base list of names to use for configuration
        /// </summary>
        /// <param name="lsName">Name of the provider</param>
        /// <returns>List of base names.</returns>
        protected MaxIndex GetBaseList(string lsName)
        {
            MaxIndex loR = new MaxIndex();
            Type loType = this.GetType();
            MaxIndex loBaseList = new MaxIndex();
            while (null != MaxFactryLibrary.GetBaseType(loType))
            {
                loBaseList.Add(MaxFactryLibrary.GetBaseType(loType).ToString());
                loType = MaxFactryLibrary.GetBaseType(loType);
            }

            string[] laBaseListKey = loBaseList.GetSortedKeyList();
            for (int lnK = laBaseListKey.Length - 1; lnK >= 0; lnK--)
            {
                string lsBase = loBaseList[laBaseListKey[lnK]] as string;
                loR.Add(lsBase);
                if (null != lsName && lsName.Length > 0)
                {
                    loR.Add(lsBase + "-" + lsName);
                }
            }

            loR.Add(this.GetType().ToString());
            if (null != lsName && lsName.Length > 0)
            {
                loR.Add(this.GetType().ToString() + "-" + lsName);
            }

            return loR;
        }
    }
}