// <copyright file="MaxIndexItemStructure.cs" company="Lakstins Family, LLC">
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
// <change date="7/18/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="5/8/2017" author="Brian A. Lakstins" description="Add more handling of nulls when doing compare.">
// <change date="6/12/2017" author="Brian A. Lakstins" description="Add Index property.">
// </changelog>
#endregion

namespace MaxFactry.Core
{
    using System;
    using System.Collections;

    /// <summary>
    /// Class used for key/value mapping in a MaxIndex.
    /// </summary>
    public class MaxIndexItemStructure : IComparable
    {
        /// <summary>
        /// Key for the item used for looking up the item.
        /// </summary>
        private string _sKey;

        /// <summary>
        /// Value of the item.
        /// </summary>
        private object _oValue;

        /// <summary>
        /// Index of the item.
        /// </summary>
        private int _nIndex;

        /// <summary>
        /// Initializes a new instance of the MaxIndexItemStructure class.
        /// </summary>
        public MaxIndexItemStructure()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxIndexItemStructure class.
        /// </summary>
        /// <param name="lsKey">Key for the item.</param>
        /// <param name="loValue">Value for the item.</param>
        public MaxIndexItemStructure(int lnIndex, string lsKey, object loValue)
        {
            this._nIndex = lnIndex;
            this._sKey = lsKey;
            this._oValue = loValue;
        }

        /// <summary>
        /// Gets or sets the key for this item.
        /// </summary>
        public string Key
        {
            get
            {
                return this._sKey;
            }

            set
            {
                this._sKey = value;
            }
        }

        /// <summary>
        /// Gets or sets the value for this item.
        /// </summary>
        public object Value
        {
            get
            {
                return this._oValue;
            }

            set
            {
                this._oValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the value for this item.
        /// </summary>
        public int Index
        {
            get
            {
                return this._nIndex;
            }
        }

        /// <summary>
        /// Compares to items based on the key to allow sorting.
        /// </summary>
        /// <param name="loObject">IndexItem to compare to.</param>
        /// <returns>0 if match, -1 if less, 1 if more.</returns>
        public int CompareTo(object loObject)
        {
            if (null == loObject)
            {
                return -1;
            }
            else if (null == this._sKey && null == ((MaxIndexItemStructure)loObject).Key)
            {
                return 0;
            }
            else if (null == ((MaxIndexItemStructure)loObject).Key)
            {
                return -1;
            }
            else if (null == this._sKey)
            {
                return 1;
            }

            return this._sKey.CompareTo(((MaxIndexItemStructure)loObject).Key);
        }

        /// <summary>
        /// Show the key value pairs when in debug
        /// </summary>
        /// <returns>Key then equals then text of value</returns>
        public override string ToString()
        {
            string lsR = string.Empty;
            if (null != this.Key)
            {
                lsR = this.Key + "=";
            }

            if (null != this.Value)
            {
                lsR += this.Value.ToString();
            }

            return lsR;
        }
    }
}
