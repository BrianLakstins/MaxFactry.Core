// <copyright file="IMaxLogLibraryProvider.cs" company="Lakstins Family, LLC">
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
// <change date="5/22/2019" author="Brian A. Lakstins" description="Added logging a structure instead of just a message">
// <change date="5/22/2019" author="Brian A. Lakstins" description="Added performance logging.">
// <change date="4/24/2020" author="Brian A. Lakstins" description="Remove performance counter logging.">
// </changelog>
#endregion

namespace MaxFactry.Core
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Defines the base interface for interacting with the MaxLogLibrary
    /// </summary>
    public interface IMaxLogLibraryProvider : IMaxProvider
    {
        /// <summary>
        /// Logs messages.
        /// </summary>
        /// <param name="loLogLevel">Level of the message.</param>
        /// <param name="lsMessage">The message to log.</param>
        /// <param name="lsCategory">The category of the message.</param>
        void Log(MaxEnumGroup loLogLevel, string lsMessage, string lsCategory);

        /// <summary>
        /// Logs information.
        /// </summary>
        /// <param name="loLogEntry">Log entry to process.</param>
        void Log(MaxLogEntryStructure loLogEntry);

        /// <summary>
        /// Gets information about the current environment
        /// </summary>
        /// <returns>Text based message about the current environment</returns>
        string GetEnvironmentInformation();

        /// <summary>
        /// Gets detail about an exception
        /// </summary>
        /// <param name="loException">The exception to get details</param>
        /// <returns>text information about the exception</returns>
        string GetExceptionDetail(Exception loException);
    }
}
