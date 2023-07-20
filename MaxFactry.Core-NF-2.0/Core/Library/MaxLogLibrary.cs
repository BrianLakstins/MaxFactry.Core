// <copyright file="MaxLogLibrary.cs" company="Lakstins Family, LLC">
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
// <change date="5/22/2019" author="Brian A. Lakstins" description="Added ability to set current log level.">
// <change date="5/22/2019" author="Brian A. Lakstins" description="Added logging a structure instead of just a message">
// <change date="5/23/2019" author="Brian A. Lakstins" description="Added starting and stopping performance logging.">
// <change date="7/3/2019" author="Brian A. Lakstins" description="Set default logging level to Warning.">
// <change date="7/18/2019" author="Brian A. Lakstins" description="Add ability to configure log level in core.">
// <change date="4/24/2020" author="Brian A. Lakstins" description="Remove performance counter logging.">
// <change date="10/11/2021" author="Brian A. Lakstins" description="Updated to handle expanded logging levels.  Combined GetRecent into a single method">
// <change date="7/20/2023" author="Brian A. Lakstins" description="Add constant for configuration name.">
// </changelog>
#endregion

namespace MaxFactry.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Library to provide log functionality
    /// </summary>
    public class MaxLogLibrary : MaxMultipleFactory
    {
        /// <summary>
        /// Internal storage of single object
        /// </summary>
        private static MaxLogLibrary _oInstance = null;

        /// <summary>
        /// Lock object for multi-threaded access.
        /// </summary>
        private static object _oLock = new object();

        /// <summary>
        /// Last unique time used
        /// </summary>
        private static long _nUniqueLogTimeLast = DateTime.UtcNow.Ticks;

        private static Dictionary<int, List<MaxLogEntryStructure>> _oRecentLogByThreadId = new Dictionary<int, List<MaxLogEntryStructure>>();

        private static DateTime _oLastRecentClear = DateTime.UtcNow;

        /// <summary>
        /// Gets the single instance of this class.
        /// </summary>
        public static MaxLogLibrary Instance
        {
            get
            {
                if (null == _oInstance)
                {
                    lock (_oLock)
                    {
                        if (null == _oInstance)
                        {
                            _oInstance = new MaxLogLibrary();
                        }
                    }
                }

                return _oInstance;
            }
        }

        private static MaxEnumGroup _nLevel = MaxEnumGroup.LogGroup;

        public static readonly string LogSettingName = "MaxFactoryLogSetting";

        /// <summary>
        /// Gets or sets the level to use for logging
        /// </summary>
        public static MaxEnumGroup Level
        {
            get
            {
                if (MaxEnumGroup.LogGroup == _nLevel)
                {
                    object loLogSetting = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeProcess, LogSettingName);
                    if (null == loLogSetting)
                    {
                        loLogSetting = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeSession, LogSettingName);
                        if (null == loLogSetting)
                        {
                            loLogSetting = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, LogSettingName);
                        }
                    }

                    if (loLogSetting is MaxEnumGroup)
                    {
                        _nLevel = (MaxEnumGroup)loLogSetting;
                    }
                    else if (loLogSetting is string)
                    {
                        string lsMaxFactoryLogSetting = loLogSetting.ToString();
                        if (lsMaxFactoryLogSetting.Equals("debug", StringComparison.CurrentCultureIgnoreCase))
                        {
                            _nLevel = MaxEnumGroup.LogDebug;
                        }
                        else if (lsMaxFactoryLogSetting.Equals("info", StringComparison.CurrentCultureIgnoreCase))
                        {
                            _nLevel = MaxEnumGroup.LogInfo;
                        }
                        else if (lsMaxFactoryLogSetting.Equals("notice", StringComparison.CurrentCultureIgnoreCase))
                        {
                            _nLevel = MaxEnumGroup.LogNotice;
                        }
                        else if (lsMaxFactoryLogSetting.Equals("warning", StringComparison.CurrentCultureIgnoreCase))
                        {
                            _nLevel = MaxEnumGroup.LogWarning;
                        }
                        else if (lsMaxFactoryLogSetting.Equals("error", StringComparison.CurrentCultureIgnoreCase))
                        {
                            _nLevel = MaxEnumGroup.LogError;
                        }
                        else if (lsMaxFactoryLogSetting.Equals("critical", StringComparison.CurrentCultureIgnoreCase))
                        {
                            _nLevel = MaxEnumGroup.LogCritical;
                        }
                        else if (lsMaxFactoryLogSetting.Equals("alert", StringComparison.CurrentCultureIgnoreCase))
                        {
                            _nLevel = MaxEnumGroup.LogAlert;
                        }
                        else if (lsMaxFactoryLogSetting.Equals("emergency", StringComparison.CurrentCultureIgnoreCase))
                        {
                            _nLevel = MaxEnumGroup.LogEmergency;
                        }
                    }
                    else
                    {
#if DEBUG
                        if (Debugger.IsAttached)
                        {
                            _nLevel = MaxEnumGroup.LogDebug;
                        }
#endif
                    }

                    if (_nLevel == MaxEnumGroup.LogGroup)
                    {
                        _nLevel = MaxEnumGroup.LogWarning;
                    }
                }

                return _nLevel;
            }

            set
            {
                _nLevel = value;
            }
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="loLogLevel">Level of the message.</param>
        /// <param name="lsMessage">The message to log.</param>
        public static void Log(MaxEnumGroup loLogLevel, string lsMessage)
        {
            Log(loLogLevel, lsMessage, "No Category");
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="loLogLevel">Level of the message.</param>
        /// <param name="lsMessage">The message to log.</param>
        /// <param name="lsCategory">The category of the message.</param>
        public static void Log(MaxEnumGroup loLogLevel, string lsMessage, string lsCategory)
        {
            int lnLogSetting = (int)Level;
            int lnLogLevel = (int)loLogLevel;
            if (lnLogSetting >= lnLogLevel)
            {
                IMaxProvider[] loList = Instance.GetProviderList();
                Exception loFirstException = null;
                for (int lnP = 0; lnP < loList.Length; lnP++)
                {
                    if (loList[lnP] is IMaxLogLibraryProvider)
                    {
                        try
                        {
                            ((IMaxLogLibraryProvider)loList[lnP]).Log(loLogLevel, lsMessage, lsCategory);
                        }
                        catch (Exception loE)
                        {
                            loFirstException = loE;
                        }
                    }
                }

                if (null != loFirstException)
                {
                    throw loFirstException;
                }
            }
        }

        /// <summary>
        /// Logs information
        /// </summary>
        /// <param name="loLogEntry">Log entry information.</param>
        public static void Log(MaxLogEntryStructure loLogEntry)
        {
            int lnLogSetting = (int)Level;
            int lnLogLevel = (int)loLogEntry.Level;
            int lnThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;

            lock (_oLock)
            {
                if (!_oRecentLogByThreadId.ContainsKey(lnThreadId))
                {
                    _oRecentLogByThreadId.Add(lnThreadId, new List<MaxLogEntryStructure>());
                }

                _oRecentLogByThreadId[lnThreadId].Add(loLogEntry);
            }

            CleanUpRecent();
            if (lnLogSetting >= lnLogLevel)
            {
                if (loLogEntry.Timestamp.Ticks <= _nUniqueLogTimeLast)
                {
                    loLogEntry.Timestamp = GetUniqueLogTime();
                }
                else
                {
                    _nUniqueLogTimeLast = loLogEntry.Timestamp.Ticks;
                }

                IMaxProvider[] loList = Instance.GetProviderList();
                for (int lnP = 0; lnP < loList.Length; lnP++)
                {
                    if (loList[lnP] is IMaxLogLibraryProvider)
                    {
                        ((IMaxLogLibraryProvider)loList[lnP]).Log(loLogEntry);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a unique time to use for a log entry so that no two events are logged at exactly the same time.
        /// </summary>
        /// <returns></returns>
        public static DateTime GetUniqueLogTime()
        {
            DateTime ldR = DateTime.MinValue;
            lock (_oLock)
            {
                ldR = DateTime.UtcNow;
                if (ldR.Ticks <= _nUniqueLogTimeLast)
                {
                    ldR = new DateTime(_nUniqueLogTimeLast + 1, DateTimeKind.Utc);
                }

                _nUniqueLogTimeLast = ldR.Ticks;
            }

            return ldR;
        }

        /// <summary>
        /// Gets information about the current environment
        /// </summary>
        /// <returns>Text based message about the current environment</returns>
        public static string GetEnvironmentInformation()
        {
            string lsR = string.Empty;
            IMaxProvider[] loList = Instance.GetProviderList();
            for (int lnP = 0; lnP < loList.Length; lnP++)
            {
                if (loList[lnP] is IMaxLogLibraryProvider)
                {
                    string lsInfo = ((IMaxLogLibraryProvider)loList[lnP]).GetEnvironmentInformation();
                    if (!string.IsNullOrEmpty(lsInfo))
                    {
                        if (lsR.Length > 0)
                        {
                            lsR += "\r\n";
                        }

                        lsR += lsInfo;
                    }
                }
            }

            return lsR;
        }

        /// <summary>
        /// Gets detail about an exception
        /// </summary>
        /// <param name="loException">The exception to get details</param>
        /// <returns>text information about the exception</returns>
        public static string GetExceptionDetail(Exception loException)
        {
            string lsR = string.Empty;
            IMaxProvider[] loList = Instance.GetProviderList();
            for (int lnP = 0; lnP < loList.Length; lnP++)
            {
                if (loList[lnP] is IMaxLogLibraryProvider)
                {
                    string lsInfo = ((IMaxLogLibraryProvider)loList[lnP]).GetExceptionDetail(loException);
                    if (!string.IsNullOrEmpty(lsInfo))
                    {
                        if (lsR.Length > 0)
                        {
                            lsR += "\r\n";
                        }

                        lsR += lsInfo;
                    }
                }
            }

            return lsR;
        }

        public static MaxLogEntryStructure[] GetRecent(Type loType, string lsMethod, params MaxEnumGroup[] laLevel)
        {
            MaxLogEntryStructure[] laR = new MaxLogEntryStructure[0];
            List<MaxLogEntryStructure> loList = null;
            int lnThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            if (_oRecentLogByThreadId.ContainsKey(lnThreadId))
            {
                loList = new List<MaxLogEntryStructure>(_oRecentLogByThreadId[lnThreadId].ToArray());
            }

            if (null != loList)
            {
                if (null != loType && null != lsMethod)
                {
                    List<MaxLogEntryStructure> loCheckList = loList;
                    //// Check current threads for a match to the type and method
                    bool lbFound = false;
                    if (null != loCheckList)
                    {
                        for (int lnL = loCheckList.Count - 1; lnL >= 0 && !lbFound; lnL--)
                        {
                            if (loCheckList[lnL].Name.StartsWith(loType.ToString()) && loCheckList[lnL].Name.EndsWith("." + lsMethod))
                            {
                                lbFound = true;
                            }
                        }
                    }

                    if (!lbFound)
                    {
                        //// Check all threads for a match to the type and method
                        loList = null;
                        int[] laKey = new int[_oRecentLogByThreadId.Keys.Count];
                        _oRecentLogByThreadId.Keys.CopyTo(laKey, 0);
                        for (int lnK = 0; lnK < laKey.Length && null == loList; lnK++)
                        {
                            loCheckList = _oRecentLogByThreadId[laKey[lnK]];
                            for (int lnL = loCheckList.Count - 1; lnL >= 0 && null == loList; lnL--)
                            {
                                if (loCheckList[lnL].Name.StartsWith(loType.ToString()) && loCheckList[lnL].Name.EndsWith("." + lsMethod))
                                {
                                    loList = new List<MaxLogEntryStructure>(loCheckList.ToArray());
                                }
                            }
                        }
                    }
                }

                string lsType = string.Empty;
                if (null != loType)
                {
                    lsType = loType.ToString();
                }

                List<int> loLevelList = new List<int>();
                if (null != laLevel)
                {
                    foreach (MaxEnumGroup loLevel in laLevel)
                    {
                        loLevelList.Add((int)loLevel);
                    }
                }

                for (int lnL = loList.Count - 1; lnL >= 0; lnL--)
                {
                    if (!string.IsNullOrEmpty(lsType) && !loList[lnL].Name.StartsWith(lsType))
                    {
                        loList.RemoveAt(lnL);
                    }
                    else if (!string.IsNullOrEmpty(lsMethod) && !loList[lnL].Name.EndsWith("." + lsMethod))
                    {
                        loList.RemoveAt(lnL);
                    }
                    else if (!string.IsNullOrEmpty(lsType) && !string.IsNullOrEmpty(lsMethod) && loList[lnL].Name != lsType + "." + lsMethod)
                    {
                        loList.RemoveAt(lnL);
                    }
                    else if (loLevelList.Count > 0)
                    {
                        int lnLogLevel = (int)loList[lnL].Level;
                        if (!loLevelList.Contains(lnLogLevel))
                        {
                            loList.RemoveAt(lnL);
                        }
                    }
                }

                laR = loList.ToArray();
            }

            return laR;
        }

        public static void ClearRecent(int lnThreadId)
        {
            if (_oRecentLogByThreadId.ContainsKey(lnThreadId))
            {
                lock (_oLock)
                {
                    if (_oRecentLogByThreadId.ContainsKey(lnThreadId))
                    {
                        _oRecentLogByThreadId.Remove(lnThreadId);
                    }
                }
            }
        }

        public static void ClearRecent()
        {
            int lnThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            ClearRecent(lnThreadId);
        }

        public static void ClearRecent(Type loType, string lsMethod)
        {
            List<MaxLogEntryStructure> loList = null;
            int lnThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            if (_oRecentLogByThreadId.ContainsKey(lnThreadId))
            {
                loList = new List<MaxLogEntryStructure>(_oRecentLogByThreadId[lnThreadId].ToArray());
            }

            if (null != loType && null != lsMethod && null != loList)
            {
                List<MaxLogEntryStructure> loCheckList = loList;
                //// Check current threads for a match to the type and method
                bool lbFound = false;
                for (int lnL = loCheckList.Count - 1; lnL >= 0 && !lbFound; lnL--)
                {
                    if (loCheckList[lnL].Name.StartsWith(loType.ToString()) && loCheckList[lnL].Name.EndsWith("." + lsMethod))
                    {
                        lbFound = true;
                    }
                }

                if (!lbFound)
                {
                    //// Check all threads for a match to the type and method
                    lnThreadId = int.MinValue;
                    int[] laKey = new int[_oRecentLogByThreadId.Keys.Count];
                    _oRecentLogByThreadId.Keys.CopyTo(laKey, 0);
                    for (int lnK = 0; lnK < laKey.Length && lnThreadId == int.MinValue; lnK++)
                    {
                        loCheckList = _oRecentLogByThreadId[laKey[lnK]];
                        for (int lnL = loCheckList.Count - 1; lnL >= 0 && lnThreadId == int.MinValue; lnL--)
                        {
                            if (loCheckList[lnL].Name.StartsWith(loType.ToString()) && loCheckList[lnL].Name.EndsWith("." + lsMethod))
                            {
                                lnThreadId = laKey[lnK];
                            }
                        }
                    }
                }
            }

            if (lnThreadId != int.MinValue)
            {
                ClearRecent(lnThreadId);
            }
        }

        protected static void CleanUpRecent()
        {
            //// Remove any recent logs that have not been updated recently
            DateTime ldRecent = DateTime.UtcNow.AddMinutes(-5);
            if (_oLastRecentClear < ldRecent)
            {
                DateTime ldStart = DateTime.UtcNow;
                lock (_oLock)
                {
                    int[] laKey = new int[_oRecentLogByThreadId.Count];
                    _oRecentLogByThreadId.Keys.CopyTo(laKey, 0);
                    foreach (int lnKey in laKey)
                    {
                        if (_oRecentLogByThreadId.ContainsKey(lnKey) && _oRecentLogByThreadId.Count > 0)
                        {
                            //// No updates recently
                            if (_oRecentLogByThreadId[lnKey][_oRecentLogByThreadId[lnKey].Count - 1].Timestamp < ldRecent)
                            {
                                _oRecentLogByThreadId.Remove(lnKey);
                            } 
                            else if (_oRecentLogByThreadId[lnKey].Count > 100) //// recent entries very long, so trim down to 100
                            {
                                for (int lnLog = _oRecentLogByThreadId[lnKey].Count - 1; lnLog < 100; lnLog--)
                                {
                                    _oRecentLogByThreadId[lnKey].RemoveAt(lnLog);
                                }
                            }
                        }
                    }

                    _oLastRecentClear = DateTime.UtcNow;
                }

                MaxLogLibrary.Log(new MaxLogEntryStructure(typeof(MaxLogLibrary), "CleanUpRecent", MaxEnumGroup.LogStatic, "Cleaned Up Recent started at {ldStart} and took {TotalMilliseconds}", ldStart, (DateTime.UtcNow - ldStart).TotalMilliseconds));
            }
        }
    }
}