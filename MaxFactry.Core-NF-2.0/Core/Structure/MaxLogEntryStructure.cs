// <copyright file="MaxLogEntryStructure.cs" company="Lakstins Family, LLC">
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
// <change date="5/22/2019" author="Brian A. Lakstins" description="Initial creation">
// <change date="10/23/2019" author="Brian A. Lakstins" description="Add method to convert to string">
// <change date="7/21/2021" author="Brian A. Lakstins" description="Fix null exception error">
// </changelog>
#endregion

namespace MaxFactry.Core
{
    using System;
    using System.Collections;

    /// <summary>
    /// Class used for logging information details
    /// </summary>
    public class MaxLogEntryStructure
    {
        /// <summary>
        /// Date and time that the entry was created
        /// </summary>
        private DateTime _dTimestamp = DateTime.UtcNow;

        /// <summary>
        /// Level of logging for this entry
        /// </summary>
        private MaxEnumGroup _oLevel;

        /// <summary>
        /// Template for the message
        /// </summary>
        private string _sMessageTemplate;

        /// <summary>
        /// Name for the log entry
        /// </summary>
        private string _sName = string.Empty;

        /// <summary>
        /// Parameters to use for the message template or other information about the event
        /// </summary>
        private object[] _aParams;

        /// <summary>
        /// Initializes a new instance of the MaxIndexItemStructure class.
        /// </summary>
        public MaxLogEntryStructure()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxLogEntryStructure class.
        /// </summary>
        /// <param name="loLevel">Level of the log entry.</param>
        /// <param name="lsMessageTemplate">Template to use for the log message.</param>
        /// <param name="laParam">Data to use for the log message.</param>
        public MaxLogEntryStructure(MaxEnumGroup loLevel, string lsMessageTemplate, params object[] laParam)
        {
            this._oLevel = loLevel;
            this._sMessageTemplate = lsMessageTemplate;
            this._aParams = laParam;
        }

        /// <summary>
        /// Initializes a new instance of the MaxLogEntryStructure class.
        /// </summary>
        /// <param name="lsName">Name to use for the log entry.</param>
        /// <param name="loLevel">Level of the log entry.</param>
        /// <param name="lsMessageTemplate">Template to use for the log message.</param>
        /// <param name="laParam">Data to use for the log message.</param>
        public MaxLogEntryStructure(string lsName, MaxEnumGroup loLevel, string lsMessageTemplate, params object[] laParam)
        {
            this._sName = lsName;
            this._oLevel = loLevel;
            this._sMessageTemplate = lsMessageTemplate;
            this._aParams = laParam;
        }

        /// <summary>
        /// Initializes a new instance of the MaxLogEntryStructure class.
        /// </summary>
        /// <param name="lsName">Name to use for the log entry.</param>
        /// <param name="loLevel">Level of the log entry.</param>
        /// <param name="lsMessageTemplate">Template to use for the log message.</param>
        /// <param name="laParam">Data to use for the log message.</param>
        public MaxLogEntryStructure(Type loType, string lsMethod, MaxEnumGroup loLevel, string lsMessageTemplate, params object[] laParam)
        {
            this._sName = loType.ToString() + "." + lsMethod;
            this._oLevel = loLevel;
            this._sMessageTemplate = lsMessageTemplate;
            this._aParams = laParam;
        }

        /// <summary>
        /// Gets or sets the DateTime for this Log entry
        /// </summary>
        public DateTime Timestamp
        {
            get
            {
                return this._dTimestamp;
            }

            set
            {
                this._dTimestamp = value;
            }
        }

        /// <summary>
        /// Gets and sets the level of logging
        /// </summary>
        public MaxEnumGroup Level
        {
            get
            {
                return this._oLevel;
            }

            set
            {
                this._oLevel = value;
            }

        }

        /// <summary>
        /// Gets and sets the name of the log entry
        /// </summary>
        public string Name
        {
            get
            {
                return this._sName;
            }

            set
            {
                this._sName = value;
            }
        }

        /// <summary>
        /// Gets and sets the template for the message
        /// </summary>
        public string MessageTemplate
        {
            get
            {
                return this._sMessageTemplate;
            }

            set
            {
                this._sMessageTemplate = value;
            }
        }

        /// <summary>
        /// Gets and sets the parameters for this log entry
        /// </summary>
        public object[] Params
        {
            get
            {
                return this._aParams;
            }

            set
            {
                this._aParams = value;
            }
        }

        public string Message
        {
            get
            {
                string lsR = this.MessageTemplate;
                if (null != this.Params && this.Params.Length > 0)
                {
                    int lnStart = 0;
                    if (this.Params[0] is Exception)
                    {
                        lnStart = 1;
                    }

                    for (int lnP = lnStart; lnP < this.Params.Length; lnP++)
                    {
                        if (null != this.Params[lnP])
                        {
                            if (lsR.Contains("{") && lsR.Contains("}"))
                            {
                                lsR = lsR.Substring(0, lsR.IndexOf("{")) + this.Params[lnP].ToString() + lsR.Substring(lsR.IndexOf("}") + 1);
                            }
                        }
                    }
                }

                return lsR;
            }
        }

        public override string ToString()
        {
            string lsR = MaxConvertLibrary.ConvertToDateTimeFromUtc(typeof(object), this.Timestamp).ToString() + "\t" + this.Level.ToString().Replace("Log", string.Empty) + "\t" + this.Name + "\t" + this.Message;
            return lsR;
        }
    }
}
