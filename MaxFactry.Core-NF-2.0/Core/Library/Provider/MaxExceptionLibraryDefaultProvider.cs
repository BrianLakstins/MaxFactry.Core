// <copyright file="MaxExceptionLibraryDefaultProvider.cs" company="Lakstins Family, LLC">
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
// <change date="8/24/2015" author="Brian A. Lakstins" description="Initial creation">
// </changelog>
#endregion

namespace MaxFactry.Core.Provider
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Default provider for MaxExceptionLibrary
    /// </summary>
    public class MaxExceptionLibraryDefaultProvider : MaxProvider, IMaxExceptionLibraryProvider
    {
        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="lsMessage">Message to include when logging the exception.</param>
        /// <param name="loException">The exception to log.</param>
        /// <returns>Message logged.  Id is in last line of message.</returns>
        public virtual string LogException(string lsMessage, Exception loException)
        {
            Guid loId = Guid.NewGuid();
            string lsError = lsMessage;
            lsError += this.GetExceptionDetail(loException);
            lsError += this.GetEnvironmentInformation();
            lsError += "\r\n" + loId.ToString();

            this.LogErrorMessage(loId, lsError);
            return lsError;
        }

        /// <summary>
        /// Gets detail about an exception
        /// </summary>
        /// <param name="loException">The exception to get details</param>
        /// <returns>text information about the exception</returns>
        public virtual string GetExceptionDetail(Exception loException)
        {
            string lsR = string.Empty;
            try
            {
                int lnLimit = 0;
                while (null != loException && lnLimit < 10)
                {
                    lsR += "\r\n\r\nError Information " + lnLimit.ToString();
                    /*
                    if (!string.IsNullOrEmpty(loException.Source))
                    {
                        lsR += "\r\nSource=" + loException.Source;
                    }
                    */
                    /*
                    if (null != loException.TargetSite)
                    {
                        lsR += "\r\nTargetSite=" + loException.TargetSite.ToString();
                    }
                    */

                    if (null != loException.Message)
                    {
                        lsR += "\r\nMessage=" + loException.Message;
                    }

                    if (null != loException.StackTrace)
                    {
                        lsR += "\r\nStackTrace=" + loException.StackTrace;
                    }

                    /*
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
                    */

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

        /// <summary>
        /// Gets information about the current environment
        /// </summary>
        /// <returns>Text based message about the current environment</returns>
        public virtual string GetEnvironmentInformation()
        {
            return this.GetEnvironmentInformationConditional();
        }

#if net
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
#else
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
                lsR += loEVersion.ToString() + "\r\n";
            }

            return lsR;
        }
#endif

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="loId">Id of the error</param>
        /// <param name="lsMessage">Message to include when logging the exception.</param>
        /// <returns>Id of the log entry.</returns>
        protected virtual string LogErrorMessage(Guid loId, string lsMessage)
        {
            try
            {
                if (null != lsMessage && string.Empty != lsMessage)
                {
                    MaxLogLibrary.Log(MaxEnumGroup.LogError, lsMessage, "MaxException");
                }
            }
            catch (Exception loELog)
            {
                int lnException = 0;
                while (null != MaxFactryLibrary.GetValue("MaxException" + lnException.ToString()))
                {
                    lnException++;
                }

                lnException--;
                if (lnException < 10000)
                {
                    if (null != lsMessage && string.Empty != lsMessage)
                    {
                        lnException++;
                        MaxFactryLibrary.SetValue("MaxException" + lnException.ToString(), lsMessage);
                    }

                    lnException++;
                    MaxFactryLibrary.SetValue("MaxException" + lnException.ToString(), loELog.ToString());

                    return lnException.ToString();
                }
            }

            return string.Empty;
        }
    }
}
