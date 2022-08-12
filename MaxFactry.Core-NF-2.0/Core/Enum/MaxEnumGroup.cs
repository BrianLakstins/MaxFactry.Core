// <copyright file="MaxEnumGroup.cs" company="Lakstins Family, LLC">
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
// <change date="6/26/2014" author="Brian A. Lakstins" description="Copied from MaxEnvironment and added scopes.">
// <change date="7/9/2014" author="Brian A. Lakstins" description="Added User scope.">
// <change date="8/8/2014" author="Brian A. Lakstins" description="Added LogGroup.">
// <change date="11/30/2018" author="Brian A. Lakstins" description="Updated to make extended configuration scopes have generic names.">
// <change date="6/19/2019" author="Brian A. Lakstins" description="Added persistent configuration.">
// <change date="7/7/2021" author="Brian A. Lakstins" description="Add static logging.">
// <change date="10/11/2021" author="Brian A. Lakstins" description="Update logging levels.">
// </changelog>
#endregion

namespace MaxFactry.Core
{
	using System;
	
	/// <summary>
	/// List of integers related to types of environments the application can be deployed to
	/// To be used at application startup to set MaxFactryLibrary.Environment so it can be used for special code based on environment.
	/// </summary>
	public enum MaxEnumGroup
	{
        /// <summary>
        /// Group for the environment values
        /// All contain this bit 2^0
        /// </summary>
        EnvironmentGroup = 1,

		/// <summary>
        /// Production environment - 2^30 + 2^0
		/// </summary>
        EnvironmentProduction = 1073741825,

		/// <summary>
        /// Quality assurance environment - 2^29 + 2^0
		/// </summary>
        EnvironmentQA = 536870913,

		/// <summary>
        /// Development environment - 2^28  + 2^0
		/// </summary>
        EnvironmentDevelopment = 268435457,

		/// <summary>
        /// Testing environment - 2^27 + 2^0
		/// </summary>
        EnvironmentTesting = 134217729,

		/// <summary>
        /// Environment has not yet been set - 2^26 + 2^0
		/// </summary>
        EnvironmentUnknown = 67108865,

        /// <summary>
        /// Group for the scope values
        /// All contain this bit 2^1
        /// </summary>
        ScopeGroup = 2,

        /// <summary>
        /// Entire application scope - 2^30 + 2^1
        /// </summary>
        ScopeApplication = 1073741826,

        /// <summary>
        /// Current session scope - 2^29 + 2^1
        /// </summary>
        ScopeSession = 536870914,

        /// <summary>
        /// Current profile scope - 2^28 + 2^1
        /// </summary>
        ScopeProfile = 268435458,

        /// <summary>
        /// Current process scope - 2^27 + 2^1
        /// </summary>
        ScopeProcess = 134217730,

        /// <summary>
        /// Current user scope - 2^26 + 2^1
        /// </summary>
        ScopeUser = 67108866,

        /// <summary>
        /// Any scope - 2^25 + 2^1
        /// </summary>
        ScopeAny = 33554434,

        /// <summary>
        /// Current 24 scope - 2^24 + 2^1
        /// </summary>
        Scope24 = 16777218,

        /// <summary>
        /// Current 23 scope - 2^23 + 2^1
        /// </summary>
        Scope23 = 8388610,

        /// <summary>
        /// Current 22 scope - 2^22 + 2^1
        /// </summary>
        Scope22 = 4194306,

        /// <summary>
        /// Current 21 scope - 2^21 + 2^1
        /// </summary>
        Scope21 = 2097154,

        /// <summary>
        /// Current 20 scope - 2^20 + 2^1
        /// </summary>
        Scope20 = 1048578,

        /// <summary>
        /// Current Persistent scope - 2^19 + 2^1
        /// </summary>
        ScopePersistent = 524290,

        /// <summary>
        /// Group for the log values
        /// https://www.php-fig.org/psr/psr-3/
        /// https://dev.to/he110/investigating-an-incident-how-to-log-effectively-php-105o
        /// All contain this bit 2^2
        /// </summary>
        LogGroup = 4,

        /// <summary>
        /// Debug level logging.
        /// Just about anything.
        /// Detailed debug information.
        /// Debugging information that reveals the details of the event in detail
        /// Events only a developer would be interesed in.
        /// 2^30 + 2^2
        /// </summary>
        LogDebug = 1073741828,

        /// <summary>
        /// Info level logging.
        /// Interesting events.
        /// Things that are nice to know.  Any interesting events. For instance: user has signed in.
        /// Events that can be convey directly to an administrator.
        /// 2^29 + 2^2
        /// </summary>
        LogInfo = 536870916,

        /// <summary>
        /// Notice level logging.
        /// Normal but significant events.
        /// Important events within the expected behavior.
        /// Events that can be convey directly to the end user.
        /// 2^28 + 2^2
        /// </summary>
        LogNotice = 268435460,

        /// <summary>
        /// Warning level logging.
        /// Exceptional cases which are still not errors. For example usage of a deprecated method or wrong API request.
        /// 2^27 + 2^2
        /// </summary>
        LogWarning = 134217732,

        /// <summary>
        /// Error level logging.
        /// Runtime errors that do not require immediate action but should typically be logged and monitored.
        /// Errors to be monitored, but which don't require an urgent fixing.
        /// 2^26 + 2^2
        /// </summary>
        LogError = 67108868,

        /// <summary>
        /// Critical level logging.  
        /// Critical conditions.
        /// Example: Application component unavailable, unexpected exception.
        /// Critical state or an event. For instance: unavailability of a component or an unhandled exception.
        /// 2^25 + 2^2
        /// </summary>
        LogCritical = 33554436,

        /// <summary>
        /// Alert level logging.
        /// Action must be taken immediately.
        /// Example: Entire website down, database unavailable, etc. This should trigger the SMS alerts and wake you up.
        /// Error and an event to be solved in the shortest time. For example, the database is unavailable.
        /// 2^24 + 2^2
        /// </summary>
        LogAlert = 16777220,

        /// <summary>
        /// Emergency level logging.  System is unusable.  Whole app/system is completely out of order.
        /// 2^23 + 2^2
        /// </summary>
        LogEmergency = 8388612,

        /// <summary>
        /// Static level logging.  Static File. - 2^22 + 2^2
        /// </summary>
        LogStatic = 4194308
    }
}
