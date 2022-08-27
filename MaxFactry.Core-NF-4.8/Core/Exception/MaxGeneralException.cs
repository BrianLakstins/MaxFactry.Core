// <copyright file="MaxGeneralException.cs" company="Lakstins Family, LLC">
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
// <change date="2/23/2014" author="Brian A. Lakstins" description="Added HandleException for later use.">
// <change date="7/17/2014" author="Brian A. Lakstins" description="Implemented HandleException to throw exception being handled.">
// <change date="8/8/2014" author="Brian A. Lakstins" description="Added logging.">
// <change date="8/13/2014" author="Brian A. Lakstins" description="Updated logging.">
// <change date="8/24/2014" author="Brian A. Lakstins" description="Moved handling of an exception to the MaxExceptionLibrary.">
// <change date="11/11/2014" author="Brian A. Lakstins" description="Updated to log exception using MaxExceptionLibrary.">
// </changelog>
#endregion

namespace MaxFactry.Core
{
	using System;
	
	/// <summary>
	/// Base exception to use for all errors in the MaxFactry framework
	/// </summary>
    public class MaxGeneralException : MaxException
	{
		/// <summary>
        /// Initializes a new instance of the MaxGeneralException class.
		/// </summary>
		public MaxGeneralException() : base()
		{
            MaxExceptionLibrary.LogException(string.Empty, this);
        }

		/// <summary>
        /// Initializes a new instance of the MaxGeneralException class.
		/// </summary>
		/// <param name="lsMessage">Message for the exception.</param>
		public MaxGeneralException(string lsMessage)
			: base(lsMessage)
		{
            MaxExceptionLibrary.LogException(lsMessage, this);
        }

		/// <summary>
        /// Initializes a new instance of the MaxGeneralException class.
		/// </summary>
		/// <param name="lsMessage">Message for the exception.</param>
		/// <param name="loInnerException">Exception that caused this exception to be thrown.</param>
		public MaxGeneralException(string lsMessage, Exception loInnerException)
			: base(lsMessage, loInnerException)
		{
            MaxExceptionLibrary.LogException(lsMessage, this);
        }
	}
}
