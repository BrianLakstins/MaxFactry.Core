// <copyright file="MaxConfigurationLibraryDefaultProvider.cs" company="Lakstins Family, LLC">
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
// <change date="8/8/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="8/13/2014" author="Brian A. Lakstins" description="Add getting a list of keys.">
// <change date="7/2/2016" author="Brian A. Lakstins" description="Update to not run base initialize since it uses MaxConfigurationLibrary.">
// <change date="7/22/2016" author="Brian A. Lakstins" description="Remove getting assembly information.  Add to provider for application scope.">
// <change date="5/25/2017" author="Brian A. Lakstins" description="Updated to use new FindValue method of MaxIndex to reduce memory usage when creating singleton providers.">
// <change date="11/30/2018" author="Brian A. Lakstins" description="Updated for more configuration scopes.">
// <change date="2/19/2019" author="Brian A. Lakstins" description="Updated to create and secure data folders.">
// <change date="6/4/2020" author="Brian A. Lakstins" description="Updated for change to base class.  Updated to allow using provider config to set MaxConfigurationLibrary config.">
// <change date="6/5/2020" author="Brian A. Lakstins" description="Updated how global and passed configuration can be used used to populate the values for this configuration provider.">
// <change date="6/5/2020" author="Brian A. Lakstins" description="Fix setting value during initialization to use the method.">
// <change date="7/20/2023" author="Brian A. Lakstins" description="Remove creation of error folder.">
// </changelog>
#endregion

namespace MaxFactry.Core.Provider
{
	using System;
    using System.IO;    
    using System.Security.Principal;
    using System.Reflection;
#if net2  || netcore2
    using System.Security.AccessControl;
#endif
    /// <summary>
    /// Default provider for the MaxFactory class to manage configuration.
    /// </summary>
    public class MaxConfigurationLibraryDefaultProvider : MaxProvider, IMaxConfigurationLibraryProvider
	{
        /// <summary>
        /// Internal storage of values
        /// </summary>
        private MaxIndex _oValueIndex = new MaxIndex();

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="lsName">Name of the provider.</param>
        /// <param name="loConfig">Configuration information.</param>
        public override void Initialize(string lsName, MaxIndex loConfig)
        {
            this.Name = lsName;
            string lsNameConfig = this.GetConfigValue(loConfig, "MaxProviderName") as string;
            if (null != lsNameConfig && lsNameConfig.Length > 0)
            {
                this.Name = lsNameConfig;
            }

            MaxIndex loIndex = this.GetConfigValue(loConfig, "MaxConfigurationLibraryDefaultProviderIndex") as MaxIndex;
            if (null != loIndex)
            {
                string[] laIndexKey = loIndex.GetSortedKeyList();
                MaxEnumGroup[] laScope = new MaxEnumGroup[] { MaxEnumGroup.ScopeApplication, MaxEnumGroup.ScopeProcess };
                foreach (MaxEnumGroup loScope in laScope)
                {
                    string lsScopeStart = loScope.ToString() + "-";
                    foreach (string lsIndexKey in laIndexKey)
                    {
                        if (lsIndexKey.StartsWith(lsScopeStart) && lsIndexKey.Length > lsScopeStart.Length)
                        {
                            string lsKey = lsIndexKey.Substring(lsScopeStart.Length);
                            object loValue = loIndex[lsIndexKey];
                            if (null != loValue)
                            {
                                this.SetValue(loScope, lsKey, loValue);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the current value based on the key.
        /// </summary>
        /// <param name="loScope">Scope for the value.</param>
        /// <param name="lsKey">The key to use to get the value.</param>
        /// <returns>The value associated with the key.</returns>
        public virtual object GetValue(MaxEnumGroup loScope, string lsKey)
        {
            //// Updated to search through index without requiring concatenating a string to the key to save memory usage
            string lsScope = this.GetScopeKey(loScope);
            object loValue = _oValueIndex.FindValue(lsScope, lsKey);
            if (loValue is Guid && _oValueIndex.NotFoundId.Equals((Guid)loValue))
            {
                loValue = this.GetValueConditional(loScope, lsKey);
            }

            if (null == loValue && (loScope == MaxEnumGroup.Scope24 || loScope == MaxEnumGroup.Scope23 || loScope == MaxEnumGroup.Scope22 || loScope == MaxEnumGroup.Scope21 || loScope == MaxEnumGroup.Scope20))
            {
                loValue = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, lsKey);
            }

            return loValue;
        }

        /// <summary>
        /// Sets the current value based on the key.
        /// </summary>
        /// <param name="loScope">Scope for the value.</param>
        /// <param name="lsKey">The key to use to set the Value.</param>
        /// <param name="loValue">Value to store.</param>
        public virtual void SetValue(MaxEnumGroup loScope, string lsKey, object loValue)
        {
            string lsScope = this.GetScopeKey(loScope);
            lsScope += lsKey;
            this._oValueIndex.Add(lsScope, loValue);
        }

        /// <summary>
        /// Gets the list of keys.
        /// </summary>
        /// <param name="loScope">Scope for the key list.</param>
        /// <returns>List of keys.</returns>
        public virtual string[] GetKeyList(MaxEnumGroup loScope)
        {
            string[] laKeyList = this._oValueIndex.GetSortedKeyList();
            MaxIndex loIndex = new MaxIndex();
            foreach (string lsKey in laKeyList)
            {
                if (lsKey.Substring(0, loScope.ToString().Length).Equals(loScope.ToString()))
                {
                    loIndex.Add(lsKey, true);
                }
            }

            return loIndex.GetSortedKeyList();
        }

        /// <summary>
        /// Gets a type in the executing assembly
        /// </summary>
        /// <returns>Type in executing assembly</returns>
        public virtual Type GetExecutingType()
        {
            return this.GetExecutingTypeConditional();
        }

        /// <summary>
        /// Gets the current Profile Id
        /// </summary>
        /// <returns>Id of the Profile</returns>
        protected virtual Guid GetCurrentProfileId()
        {
            return this.GetCurrentProfileIdConditional();
        }

        protected virtual string GetScopeKey(MaxEnumGroup loScope)
        {
            return this.GetScopeKeyConditional(loScope);
        }

#if net2 
        /// <summary>
        /// Gets the current value based on the key.
        /// </summary>
        /// <param name="loScope">Scope for the value.</param>
        /// <param name="lsKey">The key to use to get the value.</param>
        /// <returns>The value associated with the key.</returns>
        protected virtual object GetValueConditional(MaxEnumGroup loScope, string lsKey)
        {
            object loValue = null;
            if (loScope == MaxEnumGroup.ScopeApplication)
            {
                loValue = System.Configuration.ConfigurationManager.AppSettings[lsKey];
                if (null == loValue && (lsKey.EndsWith("-ConnectionString") || lsKey.EndsWith("-ProviderName")))
                {
                    string lsName = lsKey.Replace("-ConnectionString", string.Empty).Replace("-ProviderName", string.Empty);
                    System.Configuration.ConnectionStringSettings loSettings = System.Configuration.ConfigurationManager.ConnectionStrings[lsName];
                    if (null == loSettings)
                    {
                        loSettings = System.Configuration.ConfigurationManager.ConnectionStrings[System.Configuration.ConfigurationManager.AppSettings[lsName]];
                    }

                    if (null != loSettings)
                    {
                        if (lsKey.EndsWith("-ConnectionString"))
                        {
                            loValue = loSettings.ConnectionString;
                        }
                        else if (lsKey.EndsWith("-ProviderName"))
                        {
                            loValue = loSettings.ProviderName;
                        }
                    }
                }

                if (null == loValue && lsKey.Equals("MaxAssemblyCompany", StringComparison.InvariantCultureIgnoreCase))
                {
                    AssemblyCompanyAttribute loAttribute = Attribute.GetCustomAttribute(this.GetExecutingType().Assembly, typeof(AssemblyCompanyAttribute)) as AssemblyCompanyAttribute;
                    if (null != loAttribute)
                    {
                        loValue = loAttribute.Company;
                    }
                }

                if (null == loValue && lsKey.Equals("MaxAssemblyProduct", StringComparison.InvariantCultureIgnoreCase))
                {
                    AssemblyProductAttribute loAttribute = Attribute.GetCustomAttribute(this.GetExecutingType().Assembly, typeof(AssemblyProductAttribute)) as AssemblyProductAttribute;
                    if (null != loAttribute)
                    {
                        loValue = loAttribute.Product;
                    }
                }

                if (null == loValue && lsKey.Equals("MaxAssemblyTrademark", StringComparison.InvariantCultureIgnoreCase))
                {
                    AssemblyTrademarkAttribute loAttribute = Attribute.GetCustomAttribute(this.GetExecutingType().Assembly, typeof(AssemblyTrademarkAttribute)) as AssemblyTrademarkAttribute;
                    if (null != loAttribute)
                    {
                        loValue = loAttribute.Trademark;
                    }
                }

                if (null == loValue && lsKey.Equals("MaxAssemblyCopyright", StringComparison.InvariantCultureIgnoreCase))
                {
                    AssemblyCopyrightAttribute loAttribute = Attribute.GetCustomAttribute(this.GetExecutingType().Assembly, typeof(AssemblyCopyrightAttribute)) as AssemblyCopyrightAttribute;
                    if (null != loAttribute)
                    {
                        loValue = loAttribute.Copyright;
                    }
                }

                if (null == loValue && lsKey.Equals("MaxAssemblyConfiguration", StringComparison.InvariantCultureIgnoreCase))
                {
                    AssemblyConfigurationAttribute loAttribute = Attribute.GetCustomAttribute(this.GetExecutingType().Assembly, typeof(AssemblyConfigurationAttribute)) as AssemblyConfigurationAttribute;
                    if (null != loAttribute)
                    {
                        loValue = loAttribute.Configuration;
                    }
                }

                if (null == loValue && lsKey.Equals("MaxAssemblyTitle", StringComparison.InvariantCultureIgnoreCase))
                {
                    AssemblyTitleAttribute loAttribute = Attribute.GetCustomAttribute(this.GetExecutingType().Assembly, typeof(AssemblyTitleAttribute)) as AssemblyTitleAttribute;
                    if (null != loAttribute)
                    {
                        loValue = loAttribute.Title;
                    }
                }

                if (null == loValue && lsKey.Equals("MaxAssemblyInformationalVersion", StringComparison.InvariantCultureIgnoreCase))
                {
                    AssemblyInformationalVersionAttribute loAttribute = Attribute.GetCustomAttribute(this.GetExecutingType().Assembly, typeof(AssemblyInformationalVersionAttribute)) as AssemblyInformationalVersionAttribute;
                    if (null != loAttribute)
                    {
                        loValue = loAttribute.InformationalVersion;
                    }
                }

                if (null == loValue && lsKey.Equals("MaxAssemblyFileVersion", StringComparison.InvariantCultureIgnoreCase))
                {
                    AssemblyFileVersionAttribute loAttribute = Attribute.GetCustomAttribute(this.GetExecutingType().Assembly, typeof(AssemblyFileVersionAttribute)) as AssemblyFileVersionAttribute;
                    if (null != loAttribute)
                    {
                        loValue = loAttribute.Version;
                    }
                }

                if (null == loValue && lsKey.Equals("MaxAssemblyVersion", StringComparison.InvariantCultureIgnoreCase))
                {
                    loValue = this.GetExecutingType().Assembly.GetName().Version;
                }

                if (null == loValue && lsKey.Equals("MaxDataDirectory", StringComparison.InvariantCultureIgnoreCase))
                {
                    string lsCompany = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "MaxAssemblyCompany") as string;
                    string lsProduct = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "MaxAssemblyProduct") as string;
                    string lsCompanyFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), lsCompany);
                    string lsProductFolder = System.IO.Path.Combine(lsCompanyFolder, lsProduct);
                    CreateApplicationDataFolders(lsCompanyFolder, lsProductFolder);
                    loValue = lsProductFolder;
                }

                if (null != loValue)
                {
                    this.SetValue(loScope, lsKey, loValue);
                }
            }

            return loValue;
        }
#elif netcore1 || netstandard1_2
        /// <summary>
        /// Gets the current value based on the key.
        /// </summary>
        /// <param name="loScope">Scope for the value.</param>
        /// <param name="lsKey">The key to use to get the value.</param>
        /// <returns>The value associated with the key.</returns>
        protected virtual object GetValueConditional(MaxEnumGroup loScope, string lsKey)
        {
            return null;
        }
#endif

#if net2

        protected virtual bool CreateApplicationDataFolders(string lsCompanyFolder, string lsProductFolder)
        {
            bool lbR = false;
            SecurityIdentifier loUsersSeccurityIdentifier = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            if (!Directory.Exists(lsCompanyFolder))
            {
                DirectoryInfo loCompanyFolder = Directory.CreateDirectory(lsCompanyFolder);
                bool lbCompanyModified;
                DirectorySecurity loCompanySecurity = loCompanyFolder.GetAccessControl();
                AccessRule loCompanyAccessRule = new FileSystemAccessRule(
                        loUsersSeccurityIdentifier,
                        FileSystemRights.Write |
                        FileSystemRights.ReadAndExecute |
                        FileSystemRights.Modify,
                        AccessControlType.Allow);
                loCompanySecurity.ModifyAccessRule(AccessControlModification.Add, loCompanyAccessRule, out lbCompanyModified);
                loCompanyFolder.SetAccessControl(loCompanySecurity);
            }

            if (!Directory.Exists(lsProductFolder))
            {
                string[] laFolderList = new string[] { string.Empty, "data", "log" };
                foreach (string lsFolderExt in laFolderList)
                {
                    string lsFolder = Path.Combine(lsProductFolder, lsFolderExt);
                    DirectoryInfo loProductFolder = Directory.CreateDirectory(lsFolder);
                    bool lbProductModified;
                    DirectorySecurity loProductSecurity = loProductFolder.GetAccessControl();
                    AccessRule loProductAccessRule = new FileSystemAccessRule(
                        loUsersSeccurityIdentifier,
                        FileSystemRights.Write |
                        FileSystemRights.ReadAndExecute |
                        FileSystemRights.Modify,
                        InheritanceFlags.ContainerInherit |
                        InheritanceFlags.ObjectInherit,
                        PropagationFlags.InheritOnly,
                        AccessControlType.Allow);
                    loProductSecurity.ModifyAccessRule(AccessControlModification.Add, loProductAccessRule, out lbProductModified);
                    loProductFolder.SetAccessControl(loProductSecurity);
                }

                lbR = true;
            }

            return lbR;
        }
#endif 

#if net2 || netcore1 || netstandard1_4
        /// <summary>
        /// Gets the current Profile Id
        /// </summary>
        /// <returns>Id of the Profile</returns>
        protected virtual Guid GetCurrentProfileIdConditional()
        {
            Guid loR = Guid.Empty;
            //// Check the process for the current profile Id
            object loValue = this.GetValue(MaxEnumGroup.ScopeProcess, "MaxProfileIdCurrent");
            if (null != loValue)
            {
                loR = MaxConvertLibrary.ConvertToGuid(typeof(object), loValue);
            }

            if (Guid.Empty.Equals(loR))
            {
                //// Check the current session for the profile Id
                loValue = this.GetValue(MaxEnumGroup.ScopeSession, "MaxProfileIdCurrent");
                if (null != loValue)
                {
                    loR = MaxConvertLibrary.ConvertToGuid(typeof(object), loValue);
                }
            }

            if (Guid.Empty.Equals(loR))
            {
                //// Use an application configuration file.
                string lsDataFolder = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "MaxDataDirectory") as string;
                if (!Directory.Exists(lsDataFolder))
                {
                    Directory.CreateDirectory(lsDataFolder);
                }

                MaxIndex loFileConfig = new MaxIndex();
                string lsConfigFile = System.IO.Path.Combine(lsDataFolder, "Config.json");
                if (File.Exists(lsConfigFile))
                {
                    string lsConfigFileContent = File.ReadAllText(lsConfigFile);
                    loFileConfig = MaxConvertLibrary.DeserializeObject(lsConfigFileContent, typeof(MaxIndex)) as MaxIndex;
                    if (loFileConfig.Contains("MaxProfileId"))
                    {
                        loR = MaxConvertLibrary.ConvertToGuid(typeof(object), loFileConfig["MaxProfileId"]);
                    }
                }

                if (Guid.Empty.Equals(loR))
                {
                    loR = Guid.NewGuid();
                    loFileConfig.Add("MaxProfileId", loR);
                    File.WriteAllText(lsConfigFile, MaxConvertLibrary.SerializeObjectToString(loFileConfig));
                }

                this.SetValue(MaxEnumGroup.ScopeProcess, "MaxProfileIdCurrent", loR);
            }

            return loR;
        }
#elif netstandard1_2
        /// <summary>
        /// Gets the current Profile Id
        /// </summary>
        /// <returns>Id of the Profile</returns>
        protected virtual Guid GetCurrentProfileIdConditional()
        {
            throw new NotImplementedException();
        }
#endif

#if net2 || netcore2 
        protected virtual string GetScopeKeyConditional(MaxEnumGroup loScope)
        {
            string lsR = loScope.ToString();
            if (loScope == MaxEnumGroup.ScopeProcess)
            {
                lsR += System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
            }

            return lsR;
        }
#elif netcore1 || netstandard1_2
        protected virtual string GetScopeKeyConditional(MaxEnumGroup loScope)
        {
            string lsR = loScope.ToString();
            return lsR;
        }
#endif

#if net2 || netcore2 
        /// <summary>
        /// Gets a type in the executing assembly
        /// </summary>
        /// <returns>Type in executing assembly</returns>
        protected virtual Type GetExecutingTypeConditional()
        {
            Assembly loAssembly = Assembly.GetEntryAssembly();
            if (null == loAssembly)
            {
                loAssembly = Assembly.GetExecutingAssembly();
            }

            if (null != loAssembly && loAssembly.GetTypes().Length > 0)
            {
                return loAssembly.GetTypes()[0];
            }

            return null;
        }
#elif netstandard1_4 || netcore1
        /// <summary>
        /// Gets a type in the executing assembly
        /// </summary>
        /// <returns>Type in executing assembly</returns>
        protected virtual Type GetExecutingTypeConditional()
        {
            throw new NotImplementedException();
        }
#elif netstandard1_2
        /// <summary>
        /// Gets a type in the executing assembly
        /// </summary>
        /// <returns>Type in executing assembly</returns>
        protected virtual Type GetExecutingTypeConditional()
        {
            Assembly loAssembly = Assembly.GetExecutingAssembly();
            if (null != loAssembly && loAssembly.GetTypes().Length > 0)
            {
                return loAssembly.GetTypes()[0];
            }

            return null;
        }
#endif



    }
}
