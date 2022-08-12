// <copyright file="MaxLogLibraryDefaultProvider.cs" company="Lakstins Family, LLC">
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
// <change date="8/8/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="5/22/2019" author="Brian A. Lakstins" description="Removed unused method.">
// <change date="5/22/2019" author="Brian A. Lakstins" description="Added logging a structure instead of just a message">
// <change date="5/23/2019" author="Brian A. Lakstins" description="Updated with required performance logging methods.">
// <change date="6/19/2019" author="Brian A. Lakstins" description="Consolidated logging code.">
// <change date="4/24/2020" author="Brian A. Lakstins" description="Remove performance counter logging.">
// </changelog>
#endregion

namespace MaxFactry.Core.Provider
{
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Default provider for the MaxFactory class to manage logging.
    /// </summary>
	public class MaxLogLibraryDefaultProvider : MaxLogLibraryBaseProvider, IMaxLogLibraryProvider
    {
        /// <summary>
        /// Adds the first 10000 messages to the MaxFactry.GetValue list.
        /// </summary>
        /// <param name="loLogEntry">Log entry to process.</param>
        public override void Log(MaxLogEntryStructure loLogEntry)
        {
            this.LogConditional(loLogEntry);
        }

#if net2  || netcore1 
        protected virtual void LogConditional(MaxLogEntryStructure loLogEntry)
        {
            try
            {
                string lsFile = this.GetLogFile(loLogEntry);
                string lsContent = this.GetLogText(loLogEntry);
                File.AppendAllText(lsFile, lsContent);
            }
            catch (Exception loE)
            {
                Console.Write(loE.ToString());
            }
        }
#elif netstandard1_2
        protected virtual void LogConditional(MaxLogEntryStructure loLogEntry)
        {
            try
            {
                string lsFile = this.GetLogFile(loLogEntry);
                string lsContent = this.GetLogText(loLogEntry);
            }
            catch (Exception loE)
            {
            }
        }
#endif

        /// <summary>
        /// Gets detail about an exception
        /// </summary>
        /// <param name="loException">The exception to get details</param>
        /// <returns>text information about the exception</returns>
        public virtual string GetExceptionDetail(Exception loException)
        {
            return GetExceptionDetailConditional(loException);
        }

#if net2  || netcore2 
        /// <summary>
        /// Gets detail about an exception
        /// </summary>
        /// <param name="loException">The exception to get details</param>
        /// <returns>text information about the exception</returns>
        public virtual string GetExceptionDetailConditional(Exception loException)
        {
            string lsR = string.Empty;
            try
            {
                int lnLimit = 0;
                while (null != loException && lnLimit < 10)
                {
                    lsR += "\r\n\r\nError Information " + lnLimit.ToString();
                    
                    if (!string.IsNullOrEmpty(loException.Source))
                    {
                        lsR += "\r\nSource=" + loException.Source;
                    }

                    if (null != loException.TargetSite)
                    {
                        lsR += "\r\nTargetSite=" + loException.TargetSite.ToString();
                    }

                    if (null != loException.Message)
                    {
                        lsR += "\r\nMessage=" + loException.Message;
                    }

                    if (null != loException.StackTrace)
                    {
                        lsR += "\r\nStackTrace=" + loException.StackTrace;
                    }

                    if (null != loException.Data)
                    {
                        foreach (object loKey in loException.Data.Keys)
                        {
                            if (null != loKey && null != loException.Data[loKey])
                            {
                                lsR += "\r\nData-" + loKey.ToString() + "=" + loException.Data[loKey].ToString();
                            }
                        }
                    }

                    lsR += "\r\nDetails=" + loException.ToString() + "\r\n";
                    loException = loException.InnerException;
                    lnLimit++;
                }
            }
            catch (Exception loEInnerCheck)
            {
                lsR += "\r\n\r\nError getting inner exception\r\n" + loEInnerCheck.ToString() + "\r\n";
            }

            return lsR;
        }
#elif netcore1 || netstandard1_4
        /// <summary>
        /// Gets detail about an exception
        /// </summary>
        /// <param name="loException">The exception to get details</param>
        /// <returns>text information about the exception</returns>
        public virtual string GetExceptionDetailConditional(Exception loException)
        {
            string lsR = string.Empty;
            try
            {
                int lnLimit = 0;
                while (null != loException && lnLimit < 10)
                {
                    lsR += "\r\n\r\nError Information " + lnLimit.ToString();
                    
                    if (!string.IsNullOrEmpty(loException.Source))
                    {
                        lsR += "\r\nSource=" + loException.Source;
                    }

                    if (null != loException.Message)
                    {
                        lsR += "\r\nMessage=" + loException.Message;
                    }

                    if (null != loException.StackTrace)
                    {
                        lsR += "\r\nStackTrace=" + loException.StackTrace;
                    }

                    if (null != loException.Data)
                    {
                        foreach (object loKey in loException.Data.Keys)
                        {
                            if (null != loKey && null != loException.Data[loKey])
                            {
                                lsR += "\r\nData-" + loKey.ToString() + "=" + loException.Data[loKey].ToString();
                            }
                        }
                    }

                    lsR += "\r\nDetails=" + loException.ToString() + "\r\n";
                    loException = loException.InnerException;
                    lnLimit++;
                }
            }
            catch (Exception loEInnerCheck)
            {
                lsR += "\r\n\r\nError getting inner exception\r\n" + loEInnerCheck.ToString() + "\r\n";
            }

            return lsR;
        }
#elif netstandard1_2
        /// <summary>
        /// Gets detail about an exception
        /// </summary>
        /// <param name="loException">The exception to get details</param>
        /// <returns>text information about the exception</returns>
        public virtual string GetExceptionDetailConditional(Exception loException)
        {
            string lsR = string.Empty;
            try
            {
                int lnLimit = 0;
                while (null != loException && lnLimit < 10)
                {
                    lsR += "\r\n\r\nError Information " + lnLimit.ToString();

                    if (null != loException.Message)
                    {
                        lsR += "\r\nMessage=" + loException.Message;
                    }

                    if (null != loException.StackTrace)
                    {
                        lsR += "\r\nStackTrace=" + loException.StackTrace;
                    }

                    if (null != loException.Data)
                    {
                        foreach (object loKey in loException.Data.Keys)
                        {
                            if (null != loKey && null != loException.Data[loKey])
                            {
                                lsR += "\r\nData-" + loKey.ToString() + "=" + loException.Data[loKey].ToString();
                            }
                        }
                    }

                    lsR += "\r\nDetails=" + loException.ToString() + "\r\n";
                    loException = loException.InnerException;
                    lnLimit++;
                }
            }
            catch (Exception loEInnerCheck)
            {
                lsR += "\r\n\r\nError getting inner exception\r\n" + loEInnerCheck.ToString() + "\r\n";
            }

            return lsR;
        }
#endif

        /// <summary>
        /// Gets information about the current environment
        /// </summary>
        /// <returns>Text based message about the current environment</returns>
        public virtual string GetEnvironmentInformation()
        {
            return this.GetEnvironmentInformationConditional();
        }

#if net2 || netcore2
        protected virtual string GetEnvironmentInformationConditional()
        {
            string lsR = "UTC Time: " + DateTime.UtcNow.ToString() + "\r\n";
            lsR += "\r\n\r\nSystem Data\r\n";

            try
            {
                string lsVersion = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "MaxAssemblyFileVersion") as string;
                if (null == lsVersion)
                {
                    lsVersion = "Version not found";
                }

                lsR += "Type: " + MaxConfigurationLibrary.ExecutingType.ToString() + "\r\n\r\n";
                lsR += "Version: " + lsVersion + "\r\n\r\n";
            }
            catch (Exception loEVersion)
            {
                lsR += "\r\n\r\nError Getting Version\r\n";
                lsR += string.Format("{0}\r\n", loEVersion.ToString());
            }

            try
            {
                lsR += string.Format("{0} = {1}\r\n", "CommandLine", System.Environment.CommandLine);
                lsR += string.Format("{0} = {1}\r\n", "CurrentDirectory", System.Environment.CurrentDirectory);
                lsR += string.Format("{0} = {1}\r\n", "MachineName", System.Environment.MachineName);
                lsR += string.Format("{0} = {1}\r\n", "UserName", System.Environment.UserName);
                lsR += "\r\n\r\nEnvironment Variables\r\n";
                System.Collections.IDictionary loEnvironmentIndex = System.Environment.GetEnvironmentVariables();
                foreach (string lsKey in loEnvironmentIndex.Keys)
                {
                    lsR += string.Format("{0} = {1}\r\n", lsKey, System.Environment.GetEnvironmentVariable(lsKey));
                }
            }
            catch (Exception loESys)
            {
                lsR += "\r\n\r\nError Adding System Variables\r\n";
                lsR += string.Format("{0}\r\n", loESys.ToString());
            }

            return lsR;
        }
#elif netcore1 
        protected virtual string GetEnvironmentInformationConditional()
        {
            string lsR = "UTC Time: " + DateTime.UtcNow.ToString() + "\r\n";
            lsR += "\r\n\r\nSystem Data\r\n";

            try
            {
                string lsVersion = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "MaxAssemblyFileVersion") as string;
                if (null == lsVersion)
                {
                    lsVersion = "Version not found";
                }

                lsR += "Type: " + MaxConfigurationLibrary.ExecutingType.ToString() + "\r\n\r\n";
                lsR += "Version: " + lsVersion + "\r\n\r\n";
            }
            catch (Exception loEVersion)
            {
                lsR += "\r\n\r\nError Getting Version\r\n";
                lsR += string.Format("{0}\r\n", loEVersion.ToString());
            }

            try
            {
                lsR += string.Format("{0} = {1}\r\n", "MachineName", System.Environment.MachineName);
                lsR += "\r\n\r\nEnvironment Variables\r\n";
                System.Collections.IDictionary loEnvironmentIndex = System.Environment.GetEnvironmentVariables();
                foreach (string lsKey in loEnvironmentIndex.Keys)
                {
                    lsR += string.Format("{0} = {1}\r\n", lsKey, System.Environment.GetEnvironmentVariable(lsKey));
                }
            }
            catch (Exception loESys)
            {
                lsR += "\r\n\r\nError Adding System Variables\r\n";
                lsR += string.Format("{0}\r\n", loESys.ToString());
            }

            return lsR;
        }
#elif netstandard1_4
        protected virtual string GetEnvironmentInformationConditional()
        {
            string lsR = "UTC Time: " + DateTime.UtcNow.ToString() + "\r\n";
            lsR += "\r\n\r\nSystem Data\r\n";

            try
            {
                string lsVersion = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "MaxAssemblyFileVersion") as string;
                if (null == lsVersion)
                {
                    lsVersion = "Version not found";
                }

                lsR += "Type: " + MaxConfigurationLibrary.ExecutingType.ToString() + "\r\n\r\n";
                lsR += "Version: " + lsVersion + "\r\n\r\n";
            }
            catch (Exception loEVersion)
            {
                lsR += "\r\n\r\nError Getting Version\r\n";
                lsR += string.Format("{0}\r\n", loEVersion.ToString());
            }

            try
            {
                lsR += "\r\n\r\nEnvironment Variables\r\n";
                System.Collections.IDictionary loEnvironmentIndex = System.Environment.GetEnvironmentVariables();
                foreach (string lsKey in loEnvironmentIndex.Keys)
                {
                    lsR += string.Format("{0} = {1}\r\n", lsKey, System.Environment.GetEnvironmentVariable(lsKey));
                }
            }
            catch (Exception loESys)
            {
                lsR += "\r\n\r\nError Adding System Variables\r\n";
                lsR += string.Format("{0}\r\n", loESys.ToString());
            }

            return lsR;
        }
#elif netstandard1_2
        protected virtual string GetEnvironmentInformationConditional()
        {
            string lsR = "UTC Time: " + DateTime.UtcNow.ToString() + "\r\n";
            lsR += "\r\n\r\nSystem Data\r\n";

            try
            {
                string lsVersion = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "MaxAssemblyFileVersion") as string;
                if (null == lsVersion)
                {
                    lsVersion = "Version not found";
                }

                lsR += "Type: " + MaxConfigurationLibrary.ExecutingType.ToString() + "\r\n\r\n";
                lsR += "Version: " + lsVersion + "\r\n\r\n";
            }
            catch (Exception loEVersion)
            {
                lsR += "\r\n\r\nError Getting Version\r\n";
                lsR += string.Format("{0}\r\n", loEVersion.ToString());
            }

            return lsR;
        }
#endif
    }
}
