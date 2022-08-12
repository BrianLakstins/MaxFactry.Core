// <copyright file="MaxFactryLibraryDefaultProvider.cs" company="Lakstins Family, LLC">
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
// <change date="2/22/2014" author="Brian A. Lakstins" description="Updated key used to reference configuration for a provider.">
// <change date="2/25/2014" author="Brian A. Lakstins" description="Added checking for a provider specific configuration.">
// <change date="3/1/2014" author="Brian A. Lakstins" description="Update to make name of provider match settings name.">
// <change date="4/2/2014" author="Brian A. Lakstins" description="Update creation to provide better error reporting.">
// <change date="5/21/2014" author="Brian A. Lakstins" description="Update to return nulls when a class cannot be created based on configuration information.">
// <change date="5/27/2014" author="Brian A. Lakstins" description="Update to return nulls when a singleton provider cannot be created based on configuration information.">
// <change date="6/26/2014" author="Brian A. Lakstins" description="Added values by scope.  Fixed accidentally changed method GetSetting.">
// <change date="6/27/2014" author="Brian A. Lakstins" description="Update methods to virtual so they can be overridden.">
// <change date="7/19/2014" author="Brian A. Lakstins" description="Add an index especially for providers only.">
// <change date="8/13/2014" author="Brian A. Lakstins" description="Add getting a list of keys.">
// <change date="9/16/2014" author="Brian A. Lakstins" description="Update CreateSingletonProvider to not keep creating objects that are not providers.">
// <change date="12/4/2014" author="Brian A. Lakstins" description="Removed singleton creation methods. Centralized provider configuration.">
// <change date="1/28/2015" author="Brian A. Lakstins" description="Fix issue with changing configuration.">
// <change date="7/2/2016" author="Brian A. Lakstins" description="Updated to no longer seek out provider configuration. Allowing the provider to seek it out instead.">
// </changelog>
#endregion

namespace MaxFactry.Core.Provider
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Default provider for the MaxFactryLibrary. class to create dynamic objects.
	/// The creating of this object, or an override of this object is defined in the MaxFactryLibrary..SettingsIndex collection.
	/// </summary>
	public class MaxFactryLibraryDefaultProvider : MaxProvider, IMaxFactryLibraryProvider
	{
        /// <summary>
        /// Initializes a new instance of the MaxFactryLibraryDefaultProvider class.
        /// This constructor is needed for MaxFactryLibrary to create a new instance of this type of provider.
        /// </summary>
        /// <param name="lsName">Name of the provider.</param>
        public MaxFactryLibraryDefaultProvider(string lsName)
        {
            this.Name = lsName;
        }

		/// <summary>
		/// Dynamic creation of an object based on the type and constructor parameters
		/// </summary>
		/// <param name="loType">Type of object</param>
		/// <param name="laParameterTypeList">List of parameter types for the constructor</param>
		/// <param name="laParameterValueList">List of parameter values for the constructor.  Must match parameter types.</param>
		/// <returns>Dynamically created object of the type specified</returns>
        public virtual object Create(Type loType, Type[] laParameterTypeList, object[] laParameterValueList)
		{
            ConstructorInfo loConstructor = MaxFactryLibrary.GetConstructor(loType, laParameterTypeList);
            if (null != loConstructor)
            {
                return loConstructor.Invoke(laParameterValueList);
            }

            return null;
		}

		/// <summary>
		/// Dynamic creation of an object based constructor parameters (types are inferred based on the value types).
		/// </summary>
		/// <param name="loType">Type of object.</param>
		/// <param name="laParamList">List of parameter values for the constructor.</param>
		/// <returns>Dynamically created object of the type specified.</returns>	
        public virtual object Create(Type loType, params object[] laParamList)
		{
			Type[] laType = new Type[laParamList.Length];
			for (int lnT = 0; lnT < laType.Length; lnT++)
			{
				laType[lnT] = laParamList[lnT].GetType();
			}

            ConstructorInfo loConstructor = MaxFactryLibrary.GetConstructor(loType, laType);
            if (null != loConstructor)
            {
                return loConstructor.Invoke(laParamList);
            }

            return null;
		}

		/// <summary>
		/// Dynamic creation of an object based on settings.
		/// </summary>
		/// <param name="loSettings">The settings used to construct the object.</param>
		/// <returns>Dynamically created object of the type specified or an object that inherits from the type specified.</returns>
        public virtual object Create(MaxSettingsStructure loSettings)
		{
			if (null == loSettings)
			{
                throw new MaxException("Error creating object based on settings.  The settings are null.");
			}

            if (null == loSettings.Type)
            {
                throw new MaxException("Error creating object [" + loSettings.Name + "] of class [" + loSettings.Namespace + "." + loSettings.Class + "] in DLL [" + loSettings.Dll + "].  Type is null, so probably not included in the project.");
            }
            
            try
			{
				object loObject = this.Create(loSettings.Type, loSettings.ConstuctorParameterTypeList, loSettings.ConstuctorParameterValueList);
				if (null == loObject)
				{
                    throw new MaxException("Error creating object [" + loSettings.Name + "] of type [" + loSettings.Type + "].  Created object is null.");
				}

				return loObject;
			}
			catch (Exception loE)
			{
                throw new MaxException("Error creating object [" + loSettings.Name + "] of type [" + loSettings.Type.ToString() + "].  Check inner exception.", loE);
			}
		}

		/// <summary>
		/// Dynamic creation of an object based on configuration associated with the type.
		/// </summary>
		/// <param name="loType">Base type of the object</param>
		/// <returns>Dynamically created object of the type specified or an object that inherits from the type specified.</returns>
        public virtual object Create(Type loType)
		{
			MaxSettingsStructure loSettings = MaxFactryLibrary.GetSetting(loType.ToString());
			if (null != loSettings)
			{
				return this.Create(loSettings);
			}

			try
			{
				object loObject = this.Create(loType, new Type[] { }, new object[] { });
				return loObject;
			}
			catch (Exception loE)
			{
                throw new MaxException("Error creating instance of [" + loType.ToString() + "]", loE);
			}
		}

		/// <summary>
		/// Dynamic creation of an object based on the type, namespace, and dynamic link library
		/// </summary>
		/// <param name="lsNamespace">Namespace where the object is located</param>
		/// <param name="lsType">Type of the object</param>
		/// <param name="lsDll">Name of the Dynamic Link Library that the object is in</param>
		/// <returns>Dynamically created object defined by the Namespace, Type, and dynamic link library.</returns>
        public virtual object Create(string lsNamespace, string lsType, string lsDll)
		{
			return this.Create(new MaxSettingsStructure(string.Empty, lsNamespace, lsType, lsDll));
		}

		/// <summary>
		/// Dynamic creation and initialization of a provider based on the settings and configuration.
		/// </summary>
        /// <param name="lsName">The name of the provider.</param>
        /// <param name="loSettings">The settings used to create the provider.</param>
		/// <param name="loConfig">Configuration values to initialize the provider.</param>
		/// <returns>Dynamically created and initialized provider.</returns>
        public virtual IMaxProvider CreateProvider(string lsName, MaxSettingsStructure loSettings, MaxIndex loConfig)
		{
			object loProvider = this.Create(loSettings);
            if (loProvider is IMaxProvider)
            {
                return this.InitializeProvider((IMaxProvider)loProvider, lsName, loConfig, loSettings.Type);
            }

            return null;
		}

		/// <summary>
		/// Dynamic creation and initialization of a provider based on configuration associated with the type.
		/// </summary>
        /// <param name="lsName">The name of the provider.</param>
        /// <param name="loType">Base type of the provider.</param>
		/// <param name="loConfig">Initialization configuration information for the provider.</param>
		/// <returns>Dynamically created and initialized provider.</returns>
        public virtual IMaxProvider CreateProvider(string lsName, Type loType, MaxIndex loConfig)
		{
			object loProvider = this.Create(loType);
            if (loProvider is IMaxProvider)
            {
                return this.InitializeProvider((IMaxProvider)loProvider, lsName, loConfig, loType);
            }

            return null;
		}

		/// <summary>
		/// Initializes a provider.
		/// </summary>
		/// <param name="loProvider">The provider object.</param>
		/// <param name="lsName">The name of the provider.</param>
		/// <param name="loConfig">The Name Value pairs of configuration parameters.</param>
        /// <param name="loOriginalType">Original type that was requested to be created.</param>
		/// <returns>The initialized provider.</returns>
        protected virtual IMaxProvider InitializeProvider(IMaxProvider loProvider, string lsName, MaxIndex loConfig, Type loOriginalType)
		{
            loProvider.Initialize(lsName, loConfig);
            return loProvider;
		}
	}
}
