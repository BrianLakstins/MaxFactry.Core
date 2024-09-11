// <copyright file="MaxLogLibraryBaseProvider.cs" company="Lakstins Family, LLC">
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
// <change date="7/8/2019" author="Brian A. Lakstins" description="Initial creation">
// <change date="7/30/2019" author="Brian A. Lakstins" description="Create unique folder for each day's log files">
// <change date="4/24/2020" author="Brian A. Lakstins" description="Remove performance counter logging.">
// <change date="7/7/2021" author="Brian A. Lakstins" description="Add static logging.">
// <change date="7/8/2021" author="Brian A. Lakstins" description="Fix issue with log file extension.">
// <change date="10/11/2021" author="Brian A. Lakstins" description="Updated to handle expanded logging levels.">
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
	public abstract class MaxLogLibraryBaseProvider : MaxProvider
    {
        protected virtual string GetLogFile(MaxLogEntryStructure loLogEntry)
        {
            string lsR = this.GetLogFileConditional(loLogEntry);
            return lsR;
        }

        protected virtual string GetLogFileName(MaxLogEntryStructure loLogEntry)
        {
            string lsR = string.Empty;
            if (loLogEntry.Level != MaxEnumGroup.LogStatic)
            {
                lsR = MaxLogLibrary.GetUniqueLogTime().Ticks.ToString() + ".";
            }
            
            if (null != loLogEntry.Name && string.Empty != loLogEntry.Name)
            {
                lsR += loLogEntry.Name + ".";
            }

            lsR += this.GetLevelName(loLogEntry.Level);
            return lsR;
        }

        protected virtual string GetLevelName(MaxEnumGroup loLogLevel)
        {
            string lsR = "txt";
            if (loLogLevel == MaxEnumGroup.LogStatic)
            {
                lsR = "Log.txt";
            }
            else
            {
                lsR = loLogLevel.ToString().Replace("Log", string.Empty) + ".txt";
            }

            return lsR;
        }

        protected virtual string GetLogText(MaxLogEntryStructure loLogEntry)
        {
            string lsSep = "\r\n----------------------------------------------------------------" + Guid.NewGuid().ToString() + "\r\n";
            string lsR = loLogEntry.Timestamp.ToString("HH:mm:ss.fffffff") + "\t" + loLogEntry.Level.ToString() + "\t" + loLogEntry.Name + "\r\n" + loLogEntry.MessageTemplate;
            if (null != loLogEntry.Params && loLogEntry.Params.Length > 0)
            {
                for (int lnL = 0; lnL < loLogEntry.Params.Length; lnL++)
                {
                    object loParam = loLogEntry.Params[lnL];
                    if (null != loParam)
                    {
                        lsR += "Param #" + lnL + lsSep + loParam.GetType().ToString() + lsSep;
                        try
                        {
                            if (loParam is Exception)
                            {
                                lsR += ((Exception)loParam).ToString();
                            }
                            else if (loParam is string)
                            {
                                lsR += (string)loParam + lsSep;
                            }
                            else
                            {
                                lsR += MaxConvertLibrary.SerializeObjectToString(loParam) + lsSep;
                            }
                        }
                        catch (Exception loE)
                        {
                            lsR += "Exception adding param text: " + loE.ToString();
                        }
                    }
                }
            }

            lsR += "\r\n";
            return lsR;
        }

        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="loLogLevel">Level of the message.</param>
        /// <param name="lsMessage">The message to log.</param>
        /// <param name="lsCategory">The category of the message.</param>
        public virtual void Log(MaxEnumGroup loLogLevel, string lsMessage, string lsCategory)
        {
            MaxLogEntryStructure loLogEntry = new MaxLogEntryStructure(loLogLevel, lsMessage);
            this.Log(loLogEntry);
        }

        /// <summary>
        /// Logs a message.  Need and override in child class
        /// </summary>
        /// <param name="loLogEntry">Log entry to process.</param>
        public virtual void Log(MaxLogEntryStructure loLogEntry)
        {
            throw new NotImplementedException();
        }

#if net2 || netcore1 || netstandard1_4
        protected virtual string GetLogFileConditional(MaxLogEntryStructure loLogEntry)
        {
            string lsLogFolder = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "MaxDataDirectory").ToString() + @"\log\" + string.Format("{0:yyyyMMdd}", loLogEntry.Timestamp);
            if (!Directory.Exists(lsLogFolder))
            {
                Directory.CreateDirectory(lsLogFolder);
            }

            string lsR = lsLogFolder + @"\" + this.GetLogFileName(loLogEntry);
            return lsR;
        }
#elif netstandard1_2
        protected virtual string GetLogFileConditional(MaxLogEntryStructure loLogEntry)
        {
            string lsLogFolder = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "MaxDataDirectory").ToString() + @"\log\" + string.Format("{0:yyyyMMdd}", loLogEntry.Timestamp);
            string lsR = lsLogFolder + @"\" + this.GetLogFileName(loLogEntry);
            return lsR;
        }
#endif
    }
}

