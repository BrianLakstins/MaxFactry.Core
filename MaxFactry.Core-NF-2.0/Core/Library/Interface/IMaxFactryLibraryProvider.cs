// <copyright file="IMaxFactryLibraryProvider.cs" company="Lakstins Family, LLC">
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
// <change date="6/26/2014" author="Brian A. Lakstins" description="Added values by scope.">
// <change date="12/4/2014" author="Brian A. Lakstins" description="Removed singleton methods.">
// </changelog>
#endregion

namespace MaxFactry.Core
{
	using System;

	/// <summary>
	/// Interface used by MaxFactory class for providers
	/// </summary>
	public interface IMaxFactryLibraryProvider : IMaxProvider
	{
		/// <summary>
		/// Dynamic creation of an object based on the type and constructor parameters.
		/// </summary>
		/// <param name="loType">Type of object.</param>
		/// <param name="loParameterTypeList">List of parameter types for the constructor.</param>
		/// <param name="loParameterValueList">List of parameter values for the constructor.  Must match parameter types.</param>
		/// <returns>Dynamically created object of the type specified.</returns>
		object Create(Type loType, Type[] loParameterTypeList, object[] loParameterValueList);

		/// <summary>
		/// Dynamic creation of an object based constructor parameters (types are inferred based on the value types).
		/// </summary>
		/// <param name="loType">Type of object.</param>
		/// <param name="laParamList">List of parameter values for the constructor.</param>
		/// <returns>Dynamically created object of the type specified.</returns>		
		object Create(Type loType, params object[] laParamList);
	
		/// <summary>
		/// Dynamic creation of an object based on settings.
		/// </summary>
		/// <param name="loSettings">The settings used to construct the object.</param>
		/// <returns>Dynamically created object of the type specified or an object that inherits from the type specified.</returns>
		object Create(MaxSettingsStructure loSettings);

		/// <summary>
		/// Dynamic creation of an object based on configuration associated with the type.
		/// </summary>
		/// <param name="loType">Base type of the object</param>
		/// <returns>Dynamically created object of the type specified or an object that inherits from the type specified.</returns>
		object Create(Type loType);

		/// <summary>
		/// Dynamic creation and initialization of a provider based on the settings and configuration.
		/// </summary>
        /// <param name="lsName">The name of the provider.</param>
        /// <param name="loSettings">The settings used to create the provider.</param>
		/// <param name="loConfig">Configuration values to initialize the provider.</param>
		/// <returns>Dynamically created and initialized provider.</returns>
		IMaxProvider CreateProvider(string lsName, MaxSettingsStructure loSettings, MaxIndex loConfig);

		/// <summary>
		/// Dynamic creation and initialization of a provider based on configuration associated with the type.
		/// </summary>
        /// <param name="lsName">The name of the provider.</param>
        /// <param name="loType">Base type of the provider.</param>
		/// <param name="loConfig">Initialization configuration information for the provider.</param>
		/// <returns>Dynamically created and initialized provider.</returns>
		IMaxProvider CreateProvider(string lsName, Type loType, MaxIndex loConfig);    
	}
}
