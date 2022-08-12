// <copyright file="MaxSettingsStructure.cs" company="Lakstins Family, LLC">
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
// <change date="6/26/2014" author="Brian A. Lakstins" description="Added configuration index for initializing providers.">
// </changelog>
#endregion

namespace MaxFactry.Core
{
	using System;
	using System.Collections;

	/// <summary>
	/// Structure to hold setting information used to create an object dynamically.
	/// </summary>
	public class MaxSettingsStructure
	{
		/// <summary>
		/// Name of the setting.
		/// </summary>
		private string _sName = string.Empty;

		/// <summary>
		/// Full type name of the class.
		/// </summary>
		private string _sType = string.Empty;

		/// <summary>
		/// Class used to create the object.
		/// </summary>
		private string _sClass = string.Empty;

		/// <summary>
		/// Namespace of the class.
		/// </summary>
		private string _sNamespace = string.Empty;

		/// <summary>
		/// Dynamic Link Library where the type is located.
		/// </summary>
		private string _sDll = string.Empty;

		/// <summary>
		/// Array of parameter types that can be used for instantiation of the object.
		/// </summary>
		private Type[] _aConstuctorParameterTypeList = new Type[] { };

		/// <summary>
		/// Array of parameter values that can be used for instantiation of the object.
		/// </summary>
		private object[] _aConstuctorParameterValueList = new object[] { };

		/// <summary>
		/// Type of object to create.
		/// </summary>
		private Type _oType = null;

        /// <summary>
        /// Private storage of configuration information
        /// </summary>
        private MaxIndex _oConfig = new MaxIndex();

		/// <summary>
		/// Initializes a new instance of the MaxSettingsStructure class.
		/// </summary>
		public MaxSettingsStructure()
		{
		}

		/// <summary>
		/// Initializes a new instance of the MaxSettingsStructure class.
		/// </summary>
		/// <param name="lsName">Name of the setting.</param>
		/// <param name="lsNamespace">Namespace of the class.</param>
		/// <param name="lsClass">Name of the class.</param>
		/// <param name="lsDll">Dynamic Link Library where the class is located.</param>
		public MaxSettingsStructure(string lsName, string lsNamespace, string lsClass, string lsDll)
		{
			this._sName = lsName;
			this._sNamespace = lsNamespace;
			this._sClass = lsClass;
			this._sDll = lsDll;
		}

		/// <summary>
		/// Initializes a new instance of the MaxSettingsStructure class.
		/// </summary>
		/// <param name="lsName">Name of the setting.</param>
		/// <param name="loType">Type of the class.</param>
		public MaxSettingsStructure(string lsName, Type loType)
		{
			this._sName = lsName;
			this._oType = loType;
		}

		/// <summary>
		/// Gets or sets the name of the setting.
		/// </summary>
		public string Name
		{
			get { return this._sName; }
			set { this._sName = value; }
		}

		/// <summary>
		/// Gets or sets the namespace for the class.
		/// </summary>
		public string Namespace
		{
			get { return this._sNamespace; }
			set { this._sNamespace = value; }
		}

		/// <summary>
		/// Gets or sets the name of the class.
		/// </summary>
		public string Class
		{
			get { return this._sClass; }
			set { this._sClass = value; }
		}

		/// <summary>
		/// Gets or sets the Dynamic Link Library that holds the class.
		/// </summary>
		public string Dll
		{
			get { return this._sDll; }
			set { this._sDll = value; }
		}

		/// <summary>
		/// Gets or sets the type that the setting describes.
		/// </summary>
		public Type Type
		{
			get
			{
				if (null == this._oType)
				{
					return Type.GetType(this._sNamespace + "." + this._sClass + "," + this._sDll);
				}

				return this._oType;
			}

			set
			{
				this._oType = value;
			}
		}

		/// <summary>
		/// Gets or sets the Parameter Type list for creating the class.
		/// </summary>
		public Type[] ConstuctorParameterTypeList
		{
			get { return this._aConstuctorParameterTypeList; }
			set { this._aConstuctorParameterTypeList = value; }
		}

		/// <summary>
		/// Gets or sets the Parameter Value list for creating the class.
		/// </summary>
		public object[] ConstuctorParameterValueList
		{
			get { return this._aConstuctorParameterValueList; }
			set { this._aConstuctorParameterValueList = value; }
		}

        /// <summary>
        /// Gets or sets the configuration information for initialization of a provider.
        /// </summary>
        public MaxIndex Config
        {
            get { return this._oConfig; }
            set { this._oConfig = value; }
        }

		/// <summary>
		/// Makes a copy of the current object.
		/// </summary>
		/// <returns>A copy of the current object.</returns>
		public MaxSettingsStructure Clone()
		{
			MaxSettingsStructure loClone = new MaxSettingsStructure(this.Name, this.Namespace, this.Class, this.Dll);
			if (null != this._oType)
			{
				loClone.Type = this._oType;
			}

			return loClone;
		}
	}
}
