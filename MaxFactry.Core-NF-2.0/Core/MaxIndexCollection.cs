// <copyright file="MaxIndex.cs" company="Lakstins Family, LLC">
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
// <change date="6/12/2017" author="Brian A. Lakstins" description="Initial creation">
// <change date="6/26/2017" author="Brian A. Lakstins" description="Fix: Error when iterating because MoveToStart was not working as intended.">
// </changelog>
#endregion

namespace MaxFactry.Core
{
    using System;
    using System.Collections;
    using System.Diagnostics;

    /// <summary>
    /// Collection type class that can create an index of other objects.
    /// Created to provide a common List and Index type of class to use instead of List, Dictionary, Hash, NameValueCollection, etc.
    /// Keys are always string type.  Values are always object type.
    /// If the "Add" method is used without a key, then the key is automatically generated as an integer.
    /// </summary>
    public class MaxIndexCollection : ICollection
    {
        public const int CollectionTypeKey = 0;

        public const int CollectionTypeValue = 1;

        /// <summary>
        /// Array to hold just the most recent name/value items.
        /// </summary>
        private MaxIndexItemStructure[] _aItemList = null;

        /// <summary>
        /// Synchronization lock
        /// </summary>
        private object _oLock = new object();

        /// <summary>
        /// Last time this index was changed
        /// </summary>
        private DateTime _dLastChanged = DateTime.UtcNow;

        /// <summary>
        /// Internal index for keeping track of order added
        /// </summary>
        private int _nIndexCurrent = 0;

        private int _nCountCurrent = -1;

        private int _nCollectionType = 0;

        private bool _bMovedToStart = false;

        /// <summary>
        /// Initializes a new instance of the MaxIndex class.
        /// </summary>
        public MaxIndexCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxIndexCollection class.
        /// </summary>
        /// <param name="lnIndexMax">Max Index for collection</param>
        public MaxIndexCollection(int lnIndexMax)
        {
            this._nIndexCurrent = lnIndexMax;
            this._aItemList = new MaxIndexItemStructure[this._nIndexCurrent];
        }

        /// <summary>
        /// Initializes a new instance of the MaxIndexCollection class.
        /// </summary>
        /// <param name="lnIndexMax">Max Index for collection</param>
        public MaxIndexCollection(int lnIndexMax, int lnCollectionType)
        {
            this._nIndexCurrent = lnIndexMax;
            this._aItemList = new MaxIndexItemStructure[this._nIndexCurrent];
            this._nCollectionType = lnCollectionType;
        }

        /// <summary>
        /// Gets the count of items in the list.
        /// Gets the number of elements contained in the ICollection.
        /// Only count those that are not null
        /// </summary>
        public int Count
        {
            get
            {
                if (this._nCountCurrent < 0)
                {
                    this._nCountCurrent = 0;
                    for (int lnI = 0; lnI < this._aItemList.Length; lnI++)
                    {
                        if (null != this._aItemList[lnI])
                        {
                            this._nCountCurrent++;
                        }
                    }
                }

                return this._nCountCurrent;
            }
        }

        protected MaxIndexItemStructure[] Items
        {
            get
            {
                return this._aItemList;
            }
        }

        /// <summary>
        /// Gets a value indicating whether access to the ICollection is synchronized (thread safe). (Inherited from ICollection.)
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the ICollection. (Inherited from ICollection.)
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return this._oLock;
            }
        }

        /// <summary>
        /// Gets the last time this index was changed
        /// </summary>
        public DateTime LastChanged
        {
            get
            {
                return this._dLastChanged;
            }
        }

        /// <summary>
        /// Gets or sets the value of each item based on an integer key.
        /// This only works if items have all been added with incrementing integer keys.
        /// </summary>
        /// <param name="lnKey">Integer to match if possible.</param>
        /// <returns>Value for the item.</returns>
        public object this[int lnKey]
        {
            get
            {
                this.MoveToStart();
                if (this._nCollectionType == CollectionTypeKey)
                {
                    return this._aItemList[lnKey].Key;
                }

                return this._aItemList[lnKey].Value;
            }
        }

        /// <summary>
        /// Adds an item to the index generating an incrementing key.
        /// </summary>
        /// <param name="loValue">value to add.</param>
        public void Add(MaxIndexItemStructure loValue)
        {
            this._aItemList[loValue.Index] = loValue;
        }

        /// <summary>
        /// Copies the elements of the ICollection to an Array, starting at a particular Array index. (Inherited from ICollection.)
        /// </summary>
        /// <param name="loArray">The one-dimensional Array that is the destination of the elements copied from ICollection. The Array must have zero-based indexing.</param>
        /// <param name="lnCount">The zero-based index in array at which copying begins.</param>
        public void CopyTo(System.Array loArray, int lnCount)
        {
            this._aItemList.CopyTo(loArray, lnCount);
        }

        /// <summary>
        /// Returns an IEnumerator object for the IEnumerable object.
        /// </summary>
        /// <returns>An IEnumerator object for the IEnumerable object.</returns>
        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new MaxIndexCollectionEnumerator(this);
        }

        protected void MoveToStart()
        {
            if (!_bMovedToStart)
            {
                int lnMovedMin = -1;
                for (int lnI = 0; lnI < this._aItemList.Length; lnI++)
                {
                    if (null == this._aItemList[lnI] && lnI + 1 < this._aItemList.Length)
                    {
                        this._aItemList[lnI] = this._aItemList[lnI + 1];
                        this._aItemList[lnI + 1] = null;
                        if (lnMovedMin < 0)
                        {
                            lnMovedMin = lnI;
                        }
                    }
                }

                if (lnMovedMin >= 0 && lnMovedMin < this.Count)
                {
                    this.MoveToStart();
                }

                this._bMovedToStart = true;
            }
        }
    }
}
