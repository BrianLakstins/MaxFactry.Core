// <copyright file="MaxConfigurationLibrary.cs" company="Lakstins Family, LLC">
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
// <change date="6/6/2015" author="Brian A. Lakstins" description="Added support for ScopeAny">
// <change date="7/22/2016" author="Brian A. Lakstins" description="Remove getting assemble information.  Add to provider for application scope.">
// </changelog>
#endregion

namespace MaxFactry.Core
{
    using System;

    /// <summary>
    /// Library to provide configuration functionality
    /// </summary>
    public class MaxConfigurationLibrary : MaxSingleFactory
    {
        /// <summary>
        /// Internal storage of single object
        /// </summary>
        private static MaxConfigurationLibrary _oInstance = null;

        /// <summary>
        /// Executing type
        /// </summary>
        private static Type _oExecutingType = null;

        /// <summary>
        /// Lock object for multi-threaded access.
        /// </summary>
        private static object _oLock = new object();

        /// <summary>
        /// Gets the Provider used for most MaxFactory methods
        /// </summary>
        public static IMaxConfigurationLibraryProvider Provider
        {
            get
            {
                if (null == Instance.BaseProvider)
                {
                    Instance.SetProvider(typeof(MaxFactry.Core.Provider.MaxConfigurationLibraryDefaultProvider));
                }

                return (IMaxConfigurationLibraryProvider)Instance.BaseProvider;
            }
        }

        /// <summary>
        /// Gets the single instance of this class.
        /// </summary>
        public static MaxConfigurationLibrary Instance
        {
            get
            {
                if (null == _oInstance)
                {
                    lock (_oLock)
                    {
                        if (null == _oInstance)
                        {
                            _oInstance = new MaxConfigurationLibrary();
                        }
                    }
                }

                return _oInstance;
            }
        }

        /// <summary>
        /// Gets or sets the type of the executing program
        /// </summary>
        public static Type ExecutingType
        {
            get
            {
                if (null == _oExecutingType)
                {
                    return Provider.GetExecutingType();
                }

                return _oExecutingType;
            }

            set
            {
                _oExecutingType = value;
            }
        }

        /// <summary>
        /// Gets the current value based on the key.
        /// </summary>
        /// <param name="loScope">Scope for the value.</param>
        /// <param name="lsKey">The key to use to get the value.</param>
        /// <returns>The value associated with the key.</returns>
        public static object GetValue(MaxEnumGroup loScope, string lsKey)
        {
            if (loScope == MaxEnumGroup.ScopeAny)
            {
                object loR = GetValue(MaxEnumGroup.ScopeProcess, lsKey);
                if (null == loR)
                {
                    loR = GetValue(MaxEnumGroup.ScopeSession, lsKey);
                    if (null == loR)
                    {
                        loR = GetValue(MaxEnumGroup.ScopeUser, lsKey);
                        if (null == loR)
                        {
                            loR = GetValue(MaxEnumGroup.ScopeProfile, lsKey);
                            if (null == loR)
                            {
                                loR = GetValue(MaxEnumGroup.ScopeApplication, lsKey);
                            }
                        }
                    }
                }

                return loR;
            }

            return Provider.GetValue(loScope, lsKey);
        }

        /// <summary>
        /// Gets the list of keys for the scope.
        /// </summary>
        /// <param name="loScope">Scope for the value.</param>
        /// <returns>The value associated with the key.</returns>
        public static string[] GetKeyList(MaxEnumGroup loScope)
        {
            return Provider.GetKeyList(loScope);
        }

        /// <summary>
        /// Sets the current value based on the key.
        /// </summary>
        /// <param name="loScope">Scope for the value.</param>
        /// <param name="lsKey">The key to use to set the Value.</param>
        /// <param name="loValue">Value to store.</param>
        public static void SetValue(MaxEnumGroup loScope, string lsKey, object loValue)
        {
            Provider.SetValue(loScope, lsKey, loValue);
        }

        /// <summary>
        /// Sets the internal provider based on the settings
        /// </summary>
        /// <param name="loType">Type to use to determine the provider.</param>
        protected override void SetProvider(Type loType)
        {
            base.SetProvider(loType);
            if (!(Instance.BaseProvider is IMaxConfigurationLibraryProvider))
            {
                throw new MaxException("Provider for MaxConfigurationLibrary does not implement IMaxConfigurationLibraryProvider.");
            }
        }
    }
}