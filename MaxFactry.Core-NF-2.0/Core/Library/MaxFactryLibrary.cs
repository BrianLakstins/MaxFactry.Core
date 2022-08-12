// <copyright file="MaxFactryLibrary.cs" company="Lakstins Family, LLC">
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
// <change date="1/23/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="1/23/2014" author="Brian A. Lakstins" description="Reviewed and updated documentation.">
// <change date="1/23/2014" author="Brian A. Lakstins" description="Wrapped all provider access in a lock to prevent multiple threads accessing it at the same time.">
// <change date="2/22/2014" author="Brian A. Lakstins" description="Updated GetValue and SetValue to access Provider instead of _oProvider.  Provider can be replaced using SetSetting.">
// <change date="2/25/2014" author="Brian A. Lakstins" description="Updated some documentation.">
// <change date="6/26/2014" author="Brian A. Lakstins" description="Added values by scope.">
// <change date="6/27/2014" author="Brian A. Lakstins" description="Fix scope values.">
// <change date="8/8/2014" author="Brian A. Lakstins" description="Added logging ability and split up providers.">
// <change date="8/13/2014" author="Brian A. Lakstins" description="Add getting a list of keys.  Updated to only call log provider when logsetting allows loglevel to be logged.">
// <change date="8/24/2014" author="Brian A. Lakstins" description="Updated to only manage dynamic object creation.">
// <change date="12/4/2014" author="Brian A. Lakstins" description="Moved Singleton methods from provider.">
// <change date="3/24/2015" author="Brian A. Lakstins" description="Removed inheriting from MaxBaseFactorySingle because it depends on MaxFactryLibrary.">
// <change date="1/28/2015" author="Brian A. Lakstins" description="Add methods to remove singleton providers.">
// <change date="5/25/2017" author="Brian A. Lakstins" description="Added static string for default instead of creating each time to improve memory usage.  Updated to use new FindValue method of MaxIndex to reduce memory usage when creating singleton providers.">
// <change date="7/18/2019" author="Brian A. Lakstins" description="Add ability to configure environment at core level.">
// <change date="6/4/2020" author="Brian A. Lakstins" description="No longer reset settings and values so that config information can be set before provider information to provider initialization can use the config.">
// <change date="6/5/2020" author="Brian A. Lakstins" description="Updated reset to do a full reset.  Only call it if configuration will be set right afterwards.">
// </changelog>
#endregion

namespace MaxFactry.Core
{
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Core class used to create dynamic objects.  This class is used instead of instantiating with a "new" indicator.
    /// The dynamic object created is defined in the MaxFactryLibrary._oSettingIndex collection.
    /// If there is not a setting in the MaxFactryLibrary._oSettingIndex for the type, then the type passed is created.
    /// Most of the methods in this class can be overridden by specifying a different provider the implements the IMaxFactoryProvider interface.
    /// </summary>
    public class MaxFactryLibrary
    {
        /// <summary>
        /// Storage Key Name
        /// </summary>
        public static readonly string MaxStorageKeyName = "MaxStorageKey";

        /// <summary>
        /// Internal storage of single object
        /// </summary>
        private static MaxFactryLibrary _oInstance = null;

        /// <summary>
        /// Lock object for multi-threaded access.
        /// </summary>
        private static object _oLock = new object();

        /// <summary>
        /// Internal storage of the current deployment environment.
        /// </summary>
        private static MaxEnumGroup _oEnvironment = MaxEnumGroup.EnvironmentUnknown;

        /// <summary>
        /// Internal storage of an index of settings used to create dynamic objects.
        /// </summary>
        private static MaxIndex _oSettingIndex = new MaxIndex();

        /// <summary>
        /// Internal storage of an index of values associated with the current deployment environment.
        /// </summary>
        private static MaxIndex _oValueIndex = new MaxIndex();

        /// <summary>
        /// Collection where single instances of objects are stored and retrieved
        /// </summary>
        private static MaxIndex _oSingletonIndex = new MaxIndex();

        /// <summary>
        /// Collection where single instances of objects are stored and retrieved
        /// </summary>
        private static MaxIndex _oSingletonProviderIndex = new MaxIndex();

        /// <summary>
        /// Internal default prefix for provider names
        /// </summary>
        private static string _sDefaultPrefix = "Default:";

        /// <summary>
        /// Internal storage of a Provider that implements the IMaxFactryLibraryProvider interface.
        /// </summary>
        private IMaxFactryLibraryProvider _oProvider = null;

        /// <summary>
        /// Gets or sets the environment.  The environment is normally only set once at startup.
        /// It can be reset by setting it back to Unknown.
        /// </summary>
        public static MaxEnumGroup Environment
        {
            get
            {
                if (_oEnvironment == MaxEnumGroup.EnvironmentUnknown)
                {
                    lock (_oLock)
                    {
                        if (_oEnvironment == MaxEnumGroup.EnvironmentUnknown)
                        {
                            //// Set the environment. 
                            object loEnvironment = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "MaxEnvironment");
                            if (null != loEnvironment && loEnvironment is string)
                            {
                                if (loEnvironment.ToString().Equals("production", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    _oEnvironment = MaxEnumGroup.EnvironmentProduction;
                                }
                                else if (loEnvironment.ToString().Equals("qa", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    _oEnvironment = MaxEnumGroup.EnvironmentQA;
                                }
                                else if (loEnvironment.ToString().Equals("dev", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    _oEnvironment = MaxEnumGroup.EnvironmentDevelopment;
                                }
                                else if (loEnvironment.ToString().Equals("testing", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    _oEnvironment = MaxEnumGroup.EnvironmentTesting;
                                }
                            }
                            else if (System.Diagnostics.Debugger.IsAttached)
                            {
                                _oEnvironment = MaxEnumGroup.EnvironmentDevelopment;
                            }
                            else
                            {
                                _oEnvironment = MaxEnumGroup.EnvironmentProduction;
                            }
                        }
                    }
                }

                return _oEnvironment;
            }

            set
            {
                if (_oEnvironment == MaxEnumGroup.EnvironmentUnknown || value == MaxEnumGroup.EnvironmentUnknown)
                {
                    lock (_oLock)
                    {
                        if (_oEnvironment == MaxEnumGroup.EnvironmentUnknown || value == MaxEnumGroup.EnvironmentUnknown)
                        {
                            _oEnvironment = value;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the Provider used for most MaxFactory methods
        /// </summary>
        public static IMaxFactryLibraryProvider Provider
        {
            get
            {
                if (null == Instance._oProvider)
                {
                    MaxFactryLibrary.SetProvider();
                }

                return (IMaxFactryLibraryProvider)Instance._oProvider;
            }
        }

        /// <summary>
        /// Gets the single instance of this class.
        /// </summary>
        public static MaxFactryLibrary Instance
        {
            get
            {
                if (null == _oInstance)
                {
                    lock (_oLock)
                    {
                        if (null == _oInstance)
                        {
                            _oInstance = new MaxFactryLibrary();
                        }
                    }
                }

                return _oInstance;
            }
        }

        /// <summary>
        /// Gets the current setting based on the key.
        /// </summary>
        /// <param name="lsKey">The key to use to get the setting.</param>
        /// <returns>A copy of the stored MaxSettingsStructure, or null if one is not found.</returns>
        public static MaxSettingsStructure GetSetting(string lsKey)
        {
            MaxSettingsStructure loSetting = null;
            if (_oSettingIndex.Contains(lsKey))
            {
                lock (_oLock)
                {
                    object loObject = _oSettingIndex[lsKey];
                    if (null != loObject)
                    {
                        loSetting = ((MaxSettingsStructure)loObject).Clone();
                    }
                }
            }

            return loSetting;
        }

        /// <summary>
        /// Sets the current setting based on the key.  Settings are used to override provider creation.
        /// </summary>
        /// <param name="lsKey">The key to use to set the setting.</param>
        /// <param name="loSetting">Setting to store.</param>
        public static void SetSetting(string lsKey, MaxSettingsStructure loSetting)
        {
            lock (_oLock)
            {
                _oSettingIndex.Add(lsKey, loSetting);
                //// Clear any singletons for this key
                _oSingletonIndex.Remove(lsKey);
                _oSingletonProviderIndex.Remove(lsKey);
                //// Clear any singletons for this setting
                string lsSingletonSettingKey = loSetting.Name + ":" + loSetting.Type.ToString();
                _oSingletonIndex.Remove(lsSingletonSettingKey);
                _oSingletonProviderIndex.Remove(lsSingletonSettingKey);
                //// Clear any singletons for this type
                _oSingletonIndex.Remove(loSetting.Type.ToString());
                _oSingletonProviderIndex.Remove(loSetting.Type.ToString());
            }
        }

        /// <summary>
        /// Gets the current value based on the key.
        /// </summary>
        /// <param name="lsKey">The key to use to get the value.</param>
        /// <returns>The value associated with the key.</returns>
        public static object GetValue(string lsKey)
        {
            object loValue = _oValueIndex[lsKey];
            return loValue;
        }

        /// <summary>
        /// Gets the value as a string using the passed value as the default.
        /// </summary>
        /// <param name="lsKey">Key to look up.</param>
        /// <param name="lsDefault">Default to return.</param>
        /// <returns>Value of the key or default if the key does not exist.</returns>
        public static string GetValueString(string lsKey, string lsDefault)
        {
            if (!_oValueIndex.Contains(lsKey))
            {
                return lsDefault;
            }

            return GetValue(lsKey).ToString();
        }

        /// <summary>
        /// Sets the current value based on the key.
        /// </summary>
        /// <param name="lsKey">The key to use to set the Value.</param>
        /// <param name="loValue">Value to store.</param>
        public static void SetValue(string lsKey, object loValue)
        {
            lock (_oLock)
            {
                _oValueIndex.Add(lsKey, loValue);
            }
        }

        /// <summary>
        /// Clears all stored information in the MaxFactory class so that a new ones can be created
        /// Clears out the Provider
        /// Clears out the index of Settings
        /// Clears out the index of Values
        /// Clears out the Environment
        /// </summary>
        public static void Reset()
        {
            lock (_oLock)
            {
                Instance._oProvider = null;
                _oEnvironment = MaxEnumGroup.EnvironmentUnknown;
                _oSettingIndex = new MaxIndex();
                _oValueIndex = new MaxIndex();
                _oSingletonIndex = new MaxIndex();
                _oSingletonProviderIndex = new MaxIndex();
            }
        }

        /// <summary>
        /// Dynamic creation of an object based on the type and constructor parameters.
        /// </summary>
        /// <param name="loType">Type of object.</param>
        /// <param name="loParameterTypeList">List of parameter types for the constructor.</param>
        /// <param name="loParameterValueList">List of parameter values for the constructor.  Must match parameter types.</param>
        /// <returns>Dynamically created object of the type specified.</returns>
        public static object Create(Type loType, Type[] loParameterTypeList, object[] loParameterValueList)
        {
            object loR = Provider.Create(loType, loParameterTypeList, loParameterValueList);
            return loR;
        }

        /// <summary>
        /// Dynamic creation of an object based constructor parameters (types are inferred based on the value types).
        /// </summary>
        /// <param name="loType">Type of object.</param>
        /// <param name="laParamList">List of parameter values for the constructor.</param>
        /// <returns>Dynamically created object of the type specified.</returns>
        public static object Create(Type loType, params object[] laParamList)
        {
            object loR = Provider.Create(loType, laParamList);
            return loR;
        }

        /// <summary>
        /// Dynamic creation of an object based on settings.
        /// </summary>
        /// <param name="loSettings">The settings used to construct the object.</param>
        /// <returns>Dynamically created object of the type specified or an object that inherits from the type specified.</returns>
        public static object Create(MaxSettingsStructure loSettings)
        {
            object loR = Provider.Create(loSettings);
            return loR;
        }

        /// <summary>
        /// Dynamic creation of an object based on configuration associated with the type.
        /// </summary>
        /// <param name="loType">Base type of the object</param>
        /// <returns>Dynamically created object of the type specified or an object that inherits from the type specified.</returns>
        public static object Create(Type loType)
        {
            object loR = Provider.Create(loType);
            return loR;
        }

        /// <summary>
        /// Dynamic creation of an object based on settings.
        /// </summary>
        /// <param name="lsNamespace">Namespace of the dynamic object.</param>
        /// <param name="lsType">Type of the dynamic object.</param>
        /// <param name="lsDll">Dynamic Link Library where the object is stored.</param>
        /// <returns>Dynamically created object of the type specified or an object that inherits from the type specified.</returns>
        public static object Create(string lsNamespace, string lsType, string lsDll)
        {
            object loR = Provider.Create(new MaxSettingsStructure(string.Empty, lsNamespace, lsType, lsDll));
            return loR;
        }

        /// <summary>
        /// Dynamic creation of a single instance of an object based on settings.
        /// </summary>
        /// <param name="loSettings">The settings used to create the object</param>
        /// <returns>Dynamically created object of the type specified or an object that inherits from the type specified.</returns>	
        public static object CreateSingleton(MaxSettingsStructure loSettings)
        {
            if (null == loSettings)
            {
                throw new MaxException("Error creating a singleton instance.  The settings are null.");
            }

            string lsKey = loSettings.Name + ":" + loSettings.Type.ToString();
            object loR = null;
            if (_oSingletonIndex.Contains(lsKey))
            {
                loR = _oSingletonIndex[lsKey];
            }
            else
            {
                object loObject = Provider.Create(loSettings);
                if (null != loObject)
                {
                    lock (_oLock)
                    {
                        _oSingletonIndex.Add(lsKey, loObject);
                    }

                    loR = loObject;
                }
            }

            return loR;
        }

        /// <summary>
        /// Dynamic creation of a single instance of an object based on settings related to the type.
        /// </summary>
        /// <param name="loType">Base type of the object.</param>
        /// <returns>Dynamically created object of the type specified or an object that inherits from the type specified.</returns>
        public static object CreateSingleton(Type loType)
        {
            string lsKey = loType.ToString();
            object loR = null;
            if (_oSingletonIndex.Contains(lsKey))
            {
                loR = _oSingletonIndex[lsKey];
            }
            else
            {
                object loObject = Provider.Create(loType);
                if (null != loObject)
                {
                    lock (_oLock)
                    {
                        _oSingletonIndex.Add(lsKey, loObject);
                    }

                    loR = loObject;
                }
            }

            return loR;
        }

        /// <summary>
        /// Dynamic creation and initialization of a provider based on the settings and configuration.
        /// </summary>
        /// <param name="loSettings">The settings used to create the provider.</param>
        /// <param name="loConfig">Configuration values to initialize the provider.</param>
        /// <returns>Dynamically created and initialized provider.</returns>
        public static IMaxProvider CreateProvider(MaxSettingsStructure loSettings, MaxIndex loConfig)
        {
            IMaxProvider loR = Provider.CreateProvider(loSettings.Name, loSettings, loConfig);
            return loR;
        }

        /// <summary>
        /// Dynamic creation and initialization of a provider based on the settings.
        /// </summary>
        /// <param name="loSettings">The settings used to create the provider.</param>
        /// <returns>Dynamically created and initialized provider.</returns>
        public static IMaxProvider CreateProvider(MaxSettingsStructure loSettings)
        {
            IMaxProvider loR = Provider.CreateProvider(loSettings.Name, loSettings, null);
            return loR;
        }

        /// <summary>
        /// Dynamic creation and initialization of a provider based on configuration associated with the type.
        /// </summary>
        /// <param name="loType">Base type of the provider.</param>
        /// <param name="loConfig">Initialization configuration information for the provider.</param>
        /// <returns>Dynamically created and initialized provider.</returns>
        public static IMaxProvider CreateProvider(Type loType, MaxIndex loConfig)
        {
            IMaxProvider loR = Provider.CreateProvider("Default", loType, loConfig);
            return loR;
        }

        /// <summary>
        /// Dynamic creation and initialization of a provider based on configuration associated with the type.
        /// </summary>
        /// <param name="loType">Base type of the provider.</param>
        /// <returns>Dynamically created and initialized provider.</returns>
        public static IMaxProvider CreateProvider(Type loType)
        {
            IMaxProvider loR = Provider.CreateProvider("Default", loType, null);
            return loR;
        }

        /// <summary>
        /// Dynamic creation and initialization of a named provider based on configuration associated with the type.
        /// </summary>
        /// <param name="lsName">The name of the provider.</param>
        /// <param name="loType">Base type of the provider.</param>
        /// <returns>Dynamically created and initialized provider.</returns>
        public static IMaxProvider CreateProvider(string lsName, Type loType)
        {
            IMaxProvider loR = Provider.CreateProvider(lsName, loType, null);
            return loR;
        }

        /// <summary>
        /// Dynamic creation and initialization of a single provider based on settings and configuration.
        /// </summary>
        /// <param name="loSettings">The settings used to create the provider.</param>
        /// <returns>Single dynamically created and initialized provider.</returns>
        public static IMaxProvider CreateSingletonProvider(MaxSettingsStructure loSettings)
        {
            return CreateSingletonProvider(loSettings, null);
        }

        /// <summary>
        /// Dynamic creation and initialization of a single provider based on settings and configuration.
        /// </summary>
        /// <param name="loSettings">The settings used to create the provider.</param>
        /// <param name="loConfig">Configuration values to initialize the provider.</param>
        /// <returns>Single dynamically created and initialized provider.</returns>
        public static IMaxProvider CreateSingletonProvider(MaxSettingsStructure loSettings, MaxIndex loConfig)
        {
            if (null == loSettings)
            {
                throw new MaxException("Error creating a singleton instance.  The settings are null.");
            }

            object loR = _oSingletonProviderIndex.FindValue(loSettings.Name, ":", loSettings.Type.ToString());
            if (loR is Guid && _oSingletonProviderIndex.NotFoundId.Equals((Guid)loR))
            {
                object loObject = Provider.CreateProvider(loSettings.Name, loSettings, loConfig);
                lock (_oLock)
                {
                    if (null != loObject)
                    {
                        _oSingletonProviderIndex.Add(loSettings.Name + ":" + loSettings.Type, loObject);
                        loR = loObject;
                    }
                    else
                    {
                        _oSingletonProviderIndex.Add(loSettings.Name + ":" + loSettings.Type, new object());
                    }
                }
            }

            if (loR is IMaxProvider)
            {
                return (IMaxProvider)loR;
            }

            return null;
        }

        /// <summary>
        /// Dynamic creation and initialization of a single provider based on settings related to the type and the configuration.
        /// </summary>
        /// <param name="loType">Base type of the provider.</param>
        /// <param name="loConfig">Configuration information for initializing the provider.</param>
        /// <returns>Single dynamically created and initialized provider.</returns>
        public static IMaxProvider CreateSingletonProvider(Type loType, MaxIndex loConfig)
        {
            string lsName = loConfig.GetValueString(loType.ToString() + "-Name", loConfig.GetValueString("Name", "Default"));
            object loR = _oSingletonProviderIndex.FindValue(lsName, ":", loType.ToString());
            if (loR is Guid && _oSingletonProviderIndex.NotFoundId.Equals((Guid)loR))
            {
                object loObject = Provider.CreateProvider(lsName, loType, loConfig);
                lock (_oLock)
                {
                    if (null != loObject)
                    {
                        _oSingletonProviderIndex.Add(lsName + ":" + loType.ToString(), loObject);
                        loR = loObject;
                    }
                    else
                    {
                        _oSingletonProviderIndex.Add(lsName + ":" + loType.ToString(), new object());
                    }
                }
            }

            if (loR is IMaxProvider)
            {
                return (IMaxProvider)loR;
            }

            return null;
        }

        /// <summary>
        /// Dynamic creation and initialization of a single provider based on settings related to the type.
        /// </summary>
        /// <param name="loType">Base type of the provider.</param>
        /// <returns>Single dynamically created and initialized provider.</returns>
        public static IMaxProvider CreateSingletonProvider(Type loType)
        {
            object loR = _oSingletonProviderIndex.FindValue(_sDefaultPrefix, loType.ToString());
            if (loR is Guid && _oSingletonProviderIndex.NotFoundId.Equals((Guid)loR))
            {
                object loObject = Provider.CreateProvider("Default", loType, null);
                lock (_oLock)
                {
                    if (null != loObject)
                    {
                        _oSingletonProviderIndex.Add(_sDefaultPrefix + loType.ToString(), loObject);
                        loR = loObject;
                    }
                    else
                    {
                        _oSingletonProviderIndex.Add(_sDefaultPrefix + loType.ToString(), new object());
                    }
                }
            }

            if (loR is IMaxProvider)
            {
                return (IMaxProvider)loR;
            }

            return null;
        }

        /// <summary>
        /// Dynamic creation and initialization of a single named provider based on settings related to the type.
        /// </summary>
        /// <param name="lsName">The name of the provider.</param>
        /// <param name="loType">Base type of the provider.</param>
        /// <returns>Single dynamically created and initialized provider.</returns>
        public static IMaxProvider CreateSingletonProvider(string lsName, Type loType)
        {
            object loR = _oSingletonProviderIndex.FindValue(lsName, ":",  loType.ToString());
            if (loR is Guid && _oSingletonProviderIndex.NotFoundId.Equals((Guid)loR))
            {
                object loObject = Provider.CreateProvider(lsName, loType, null);
                lock (_oLock)
                {
                    if (null != loObject)
                    {
                        _oSingletonProviderIndex.Add(lsName + ":" + loType.ToString(), loObject);
                        loR = loObject;
                    }
                    else
                    {
                        _oSingletonProviderIndex.Add(lsName + ":" + loType.ToString(), new object());
                    }
                }
            }

            if (loR is IMaxProvider)
            {
                return (IMaxProvider)loR;
            }

            return null;
        }

        /// <summary>
        /// Removes a singleton provider
        /// </summary>
        /// <param name="loType">Type of the provider</param>
        public static void RemoveSingletonProvider(Type loType)
        {
            lock (_oLock)
            {
                string[] laKey = _oSingletonProviderIndex.GetSortedKeyList();
                foreach (string lsKey in laKey)
                {
                    if (loType.ToString().Length <= lsKey.Length)
                    {
                        if (lsKey.Substring(0, loType.ToString().Length) == loType.ToString())
                        {
                            _oSingletonProviderIndex.Remove(lsKey);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes a singleton provider
        /// </summary>
        /// <param name="loType">Type of the provider</param>
        /// <param name="lsName">Name of the provider</param>
        public static void RemoveSingletonProvider(Type loType, string lsName)
        {
            string lsKey = lsName + ":" + loType.ToString();
            if (_oSingletonProviderIndex.Contains(lsKey))
            {
                lock (_oLock)
                {
                    if (_oSingletonProviderIndex.Contains(lsKey))
                    {
                        _oSingletonProviderIndex.Remove(lsKey);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the internal provider based on the settings
        /// </summary>
        private static void SetProvider()
        {
            if (null == Instance._oProvider)
            {
                lock (_oLock)
                {
                    if (null == Instance._oProvider)
                    {
                        MaxSettingsStructure loSettings = MaxFactryLibrary.GetSetting(typeof(MaxFactry.Core.Provider.MaxFactryLibraryDefaultProvider).ToString());
                        if (null == loSettings)
                        {
                            loSettings = new MaxSettingsStructure("Default", typeof(MaxFactry.Core.Provider.MaxFactryLibraryDefaultProvider));
                        }

                        try
                        {
                            Type[] laParameterTypeList = new Type[] { loSettings.Name.GetType() };
                            object[] laParameterList = new object[] { loSettings.Name };
                            ConstructorInfo loConstructor = GetConstructor(loSettings.Type, laParameterTypeList);
                            Instance._oProvider = (IMaxFactryLibraryProvider)loConstructor.Invoke(laParameterList);
                            Instance._oProvider.Initialize(loSettings.Name, loSettings.Config);
                        }
                        catch (Exception loE)
                        {
                            throw new MaxException("Error creating instance of IMaxFactoryProvider.", loE);
                        }

                        if (!(Instance._oProvider is IMaxFactryLibraryProvider))
                        {
                            throw new MaxException("Provider for MaxFactryLibrary does not implement IMaxFactryLibraryProvider.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the constructor for a type
        /// </summary>
        /// <param name="loType">Type to get</param>
        /// <param name="laParameterTypeList">Parameter types to match</param>
        /// <returns>ConstructorInfo for the type</returns>
        public static ConstructorInfo GetConstructor(Type loType, Type[] laParameterTypeList)
        {
#if net4_52 || netcore1 
            return loType.GetTypeInfo().GetConstructor(laParameterTypeList);
#elif net2 || netstandard1_2
            return loType.GetConstructor(laParameterTypeList);
#endif
        }

        /// <summary>
        /// Gets the base type for the type provided
        /// </summary>
        /// <param name="loType">Type to use to get the base type</param>
        /// <returns>Base type for the Type</returns>
        public static Type GetBaseType(Type loType)
        {
#if net4_52 || netcore1 || netstandard1_4
            return loType.GetTypeInfo().BaseType;
#elif net2 || netstandard1_2
            return loType.BaseType;
#endif
        }

        /// <summary>
        /// Gets the base type for the type provided
        /// </summary>
        /// <param name="loType">Type to use to get the base type</param>
        /// <returns>Base type for the Type</returns>
        public static MethodInfo[] GetMethods(Type loType, BindingFlags loFlags)
        {
#if net4_52 || netcore1 
            return loType.GetTypeInfo().GetMethods(loFlags);
#elif net2 || netstandard1_2
            return loType.GetMethods(loFlags);
#endif
        }

        /// <summary>
        /// Gets the base type for the type provided
        /// </summary>
        /// <param name="loType">Type to use to get the base type</param>
        /// <returns>Base type for the Type</returns>
        public static Assembly GetAssembly(Type loType)
        {
#if net4_52 || netcore1 || netstandard1_4
            return loType.GetTypeInfo().Assembly;
#elif net2 || netstandard1_2
            return loType.Assembly;
#endif
        }

        public static string GetStringResource(Type loType, string lsKey)
        {
            string lsR = string.Empty;
            Assembly loAssembly = GetAssembly(loType);
            Type[] laType = loAssembly.GetTypes();
            for (int lnT = 0; lnT < laType.Length; lnT++)
            {
                if (laType[lnT].Name == "MaxResource")
                {
                    MethodInfo[] laMethod = GetMethods(laType[lnT], BindingFlags.Static | BindingFlags.Public);
                    for (int lnM = 0; lnM < laMethod.Length; lnM++)
                    {
                        if (laMethod[lnM].Name == "GetString")
                        {
                            lsR = laMethod[lnM].Invoke(null, new object[] { lsKey }) as string;
                        }

                    }
                }
            }

            return lsR;
        }

        public static MaxIndex GetPropertyListValue(object loObject)
        {
            return GetPropertyListValueConditional(loObject);
        }

        public static MaxIndex GetPropertyList(object loObject)
        {
            return GetPropertyListConditional(loObject);
        }

#if net2 || netcore2 || netstandard1_2
        protected static MaxIndex GetPropertyListValueConditional(object loObject)
        {
            MaxIndex loR = new MaxIndex();
            PropertyInfo[] loPropertyList = loObject.GetType().GetProperties();
            foreach (PropertyInfo loProperty in loPropertyList)
            {
                loR.Add(loProperty.Name, loProperty.GetValue(loObject, null));
            }

            return loR;
        }
#elif netcore1
        protected static MaxIndex GetPropertyListValueConditional(object loObject)
        {
            MaxIndex loR = new MaxIndex();
            PropertyInfo[] loPropertyList = loObject.GetType().GetTypeInfo().GetProperties();
            foreach (PropertyInfo loProperty in loPropertyList)
            {
                loR.Add(loProperty.Name, loProperty.GetValue(loObject, null));
            }

            return loR;
        }
#endif

#if net2 || netcore2 || netstandard1_2
        protected static MaxIndex GetPropertyListConditional(object loObject)
        {
            MaxIndex loR = new MaxIndex();
            PropertyInfo[] laInfo = loObject.GetType().GetProperties();
            for (int lnP = 0; lnP < laInfo.Length; lnP++)
            {
                loR.Add(laInfo[lnP].Name, laInfo[lnP]);
            }

            return loR;
        }
#elif netcore1
        protected static MaxIndex GetPropertyListConditional(object loObject)
        {
            MaxIndex loR = new MaxIndex();
            PropertyInfo[] laInfo = loObject.GetType().GetTypeInfo().GetProperties();
            for (int lnP = 0; lnP < laInfo.Length; lnP++)
            {
                loR.Add(laInfo[lnP].Name, laInfo[lnP]);
            }

            return loR;
        }
#endif
    }
}
