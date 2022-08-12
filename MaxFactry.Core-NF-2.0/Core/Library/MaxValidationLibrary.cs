// <copyright file="MaxValidationLibrary.cs" company="Lakstins Family, LLC">
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
// <change date="6/5/2015" author="Brian A. Lakstins" description="Initial creation">
// </changelog>
#endregion

namespace MaxFactry.Core
{
	using System;

	/// <summary>
	/// Library to provide validation functionality
	/// </summary>
	public class MaxValidationLibrary : MaxByMethodFactory
	{
        /// <summary>
        /// Internal storage of single object
        /// </summary>
        private static MaxValidationLibrary _oInstance = null;

        /// <summary>
        /// Lock object for multi-threaded access.
        /// </summary>
        private static object _oLock = new object();

        /// <summary>
        /// Gets the single instance of this class.
        /// </summary>
        public static MaxValidationLibrary Instance
        {
            get
            {
                if (null == _oInstance)
                {
                    lock (_oLock)
                    {
                        if (null == _oInstance)
                        {
                            _oInstance = new MaxValidationLibrary();
                        }
                    }
                }

                return _oInstance;
            }
        }

        /// <summary>
        /// General validation check.
        /// </summary>
        /// <param name="loType">Type to use to determine provider.</param>
        /// <param name="lsValidation">Validation check to run.</param>
        /// <param name="loValue">Data to be validated.</param>
        /// <returns>Error message from validation check.</returns>
        public static string GetValidationError(Type loType, string lsValidation, object loValue)
        {
            IMaxProvider loBaseProvider = Instance.GetProvider(loType);
            IMaxValidationLibraryProvider loProvider = loBaseProvider as IMaxValidationLibraryProvider;          
            if (null == loProvider)
            {
                throw new MaxException("Error casting [" + loBaseProvider.GetType() + "] for Provider");
            }

            return loProvider.GetValidationError(lsValidation, loValue);
        }
	}
}