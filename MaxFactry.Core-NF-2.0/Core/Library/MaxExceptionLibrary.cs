// <copyright file="MaxExceptionLibrary.cs" company="Lakstins Family, LLC">
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
// <change date="11/11/2014" author="Brian A. Lakstins" description="Updated to log only instead of log and throw.">
// <change date="3/24/2015" author="Brian A. Lakstins" description="Removed inheriting from MaxBaseFactorySingle because this class is not using a provider.">
// <change date="8/24/2015" author="Brian A. Lakstins" description="Added interface and provider">
// </changelog>
#endregion

namespace MaxFactry.Core
{
    using System;

    /// <summary>
    /// Library to provide exception functionality
    /// </summary>
    public class MaxExceptionLibrary : MaxSingleFactory
    {
        /// <summary>
        /// Internal storage of single object
        /// </summary>
        private static MaxExceptionLibrary _oInstance = null;

        /// <summary>
        /// Lock object for multi-threaded access.
        /// </summary>
        private static object _oLock = new object();

        /// <summary>
        /// Gets the Provider used for most MaxFactory methods
        /// </summary>
        public static IMaxExceptionLibraryProvider Provider
        {
            get
            {
                if (null == Instance.BaseProvider)
                {
                    Instance.SetProvider(typeof(MaxFactry.Core.Provider.MaxExceptionLibraryDefaultProvider));
                }

                return (IMaxExceptionLibraryProvider)Instance.BaseProvider;
            }
        }

        /// <summary>
        /// Gets the single instance of this class.
        /// </summary>
        public static MaxExceptionLibrary Instance
        {
            get
            {
                if (null == _oInstance)
                {
                    lock (_oLock)
                    {
                        if (null == _oInstance)
                        {
                            _oInstance = new MaxExceptionLibrary();
                        }
                    }
                }

                return _oInstance;
            }
        }

        /// <summary>
        /// Logs an exception.
        /// TODO: Deprecate this and use MaxLogLibrary for exception logging
        /// </summary>
        /// <param name="lsMessage">Message to include when logging the exception.</param>
        /// <param name="loException">The exception to log.</param>
        /// <returns>Id of the log entry.</returns>
        public static string LogException(string lsMessage, Exception loException)
        {
            return Provider.LogException(lsMessage, loException);
        }

        /// <summary>
        /// Gets information about the current environment
        /// </summary>
        /// <returns>Text based message about the current environment</returns>
        public static string GetEnvironmentInformation()
        {
            return Provider.GetEnvironmentInformation();
        }

        /// <summary>
        /// Gets detail about an exception
        /// </summary>
        /// <param name="loException">The exception to get details</param>
        /// <returns>text information about the exception</returns>
        public static string GetExceptionDetail(Exception loException)
        {
            return Provider.GetExceptionDetail(loException);
        }

        /// <summary>
        /// Sets the internal provider based on the settings
        /// </summary>
        /// <param name="loType">Type to use to determine the provider.</param>
        protected override void SetProvider(Type loType)
        {
            base.SetProvider(loType);
            if (!(Instance.BaseProvider is IMaxExceptionLibraryProvider))
            {
                throw new MaxGeneralException("Provider for MaxExceptionLibrary does not implement IMaxExceptionLibraryProvider.");
            }
        }
    }
}