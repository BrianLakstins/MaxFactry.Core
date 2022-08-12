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
// <change date="1/23/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="1/23/2014" author="Brian A. Lakstins" description="Reviewed and updated documentation.">
// <change date="2/5/2014" author="Brian A. Lakstins" description="Fix null _aItemSortedList error.">
// <change date="7/18/2014" author="Brian A. Lakstins" description="Make the data portable so it can be extracted and loaded into another MaxIndex.">
// <change date="9/16/2014" author="Brian A. Lakstins" description="Update PadKey to consume less memory (string.concat)">
// <change date="9/16/2014" author="Brian A. Lakstins" description="Fix Remove function to make last sorted element null.">
// <change date="9/16/2014" author="Brian A. Lakstins" description="Update GetDiff to consume less cpu">
// <change date="9/16/2014" author="Brian A. Lakstins" description="Update IsInteger to consume less memory (string.ToCharArray())">
// <change date="10/17/2014" author="Brian A. Lakstins" description="Update to add items faster and sort less often.">
// <change date="12/22/2014" author="Brian A. Lakstins" description="Update to lock threads, give better out of bound errors, and combine lists faster and more robust.">
// <change date="5/27/2015" author="Brian A. Lakstins" description="Partially implement the IDictionary interface in attempt to work as an MVC Model property automatically.  Didn't work out, but implemeting IDictionary would be a nice addition.">
// <change date="1/28/2016" author="Brian A. Lakstins" description="Fix problem with removing item.">
// <change date="4/20/2016" author="Brian A. Lakstins" description="Fix problem with removing multiple items.">
// <change date="5/27/2016" author="Brian A. Lakstins" description="Fix bubble sort for when both values are null.">
// <change date="8/25/2016" author="Brian A. Lakstins" description="Fix issue with null exception being caused after an item is deleted.">
// <change date="8/29/2016" author="Brian A. Lakstins" description="Add handling of enumeration for IDictionary implementation.">
// <change date="5/25/2017" author="Brian A. Lakstins" description="Add Findvalue method to improve memory usage for calling functions.  No need to check existance, then call for value.  No need to concatenate multiple strings into keys.">
// <change date="6/12/2017" author="Brian A. Lakstins" description="Fix issue with serializing with null value.  Add better handling of Key and Value ICollections.">
// <change date="6/13/2017" author="Brian A. Lakstins" description="Have MaxIndex[int] return items in the order they were added.">
// <change date="6/13/2017" author="Brian A. Lakstins" description="Fix issue with settting _nIndexCurrent.">
// <change date="3/11/2019" author="Brian A. Lakstins" description="Make integer checking smarter to prevent System.OverflowException when the key is all numbers.">
// <change date="12/15/2020" author="Brian A. Lakstins" description="Add GetValueString using blank default">
// </changelog>
#endregion

namespace MaxFactry.Core
{
    using System;
    using System.Collections;

    /// <summary>
    /// Collection type class that can create an index of other objects.
    /// Created to provide a common List and Index type of class to use instead of List, Dictionary, Hash, NameValueCollection, etc.
    /// Keys are always string type.  Values are always object type.
    /// If the "Add" method is used without a key, then the key is automatically generated as an integer.
    /// </summary>
    public class MaxIndex : IDictionary
    {
        /// <summary>
        /// List of zeros to use to pad key.
        /// </summary>
        private static string _sZero = "0000000000";

        /// <summary>
        /// Array to hold all name/value items.
        /// </summary>
        private MaxIndexItemStructure[] _aItemSortedList = null;

        /// <summary>
        /// Current count of items in the sorted list.
        /// </summary>
        private int _nCountSorted = 0;

        /// <summary>
        /// Array to hold just the most recent name/value items.
        /// </summary>
        private MaxIndexItemStructure[] _aItemRecentList = null;

        /// <summary>
        /// Count of items in the recent list.
        /// </summary>
        private int _nCountRecent = 0;

        /// <summary>
        /// Default Max recent items to use.
        /// </summary>
        private int _nRecentIndexMax = 500;

        /// <summary>
        /// Next index to use in the recent array.
        /// </summary>
        private int _nRecentIndexNext = 0;

        /// <summary>
        /// If there are more than this items in the recent list and an item is requested, then combine this lists.
        /// </summary>
        private int _nRecentForceCombine = 50;

        /// <summary>
        /// Synchronization lock
        /// </summary>
        private object _oLock = new object();

        /// <summary>
        /// Last time this index was changed
        /// </summary>
        private DateTime _dLastChanged = DateTime.UtcNow;

        /// <summary>
        /// Internal Id used if element is not found in Index
        /// </summary>
        private Guid _oNotFoundId = Guid.Empty;

        /// <summary>
        /// Internal index for keeping track of order added
        /// </summary>
        private int _nIndexCurrent = 0;

        private MaxIndexCollection _oKeys = null;

        private MaxIndexCollection _oValues = null;

        /// <summary>
        /// Initializes a new instance of the MaxIndex class.
        /// </summary>
        public MaxIndex()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxIndex class.
        /// </summary>
        /// <param name="lnRecentMax">Maximum count of recent list.</param>
        public MaxIndex(int lnRecentMax)
        {
            this._nRecentIndexMax = lnRecentMax;
        }

        /// <summary>
        /// Initializes a new instance of the MaxIndex class.
        /// </summary>
        /// <param name="laItemList">List of items to be in the Index.</param>
        public MaxIndex(MaxIndexItemStructure[] laItemList)
        {
            this._aItemSortedList = laItemList;
            this._nCountSorted = laItemList.Length;
            this._nIndexCurrent = laItemList.Length;
            this.ReSort();
        }

        /// <summary>
        /// Gets the count of items in the list.
        /// Gets the number of elements contained in the ICollection.
        /// </summary>
        public int Count
        {
            get
            {
                return this._nCountSorted + this._nCountRecent;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the IDictionary object has a fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the IDictionary object is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
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
        /// Gets an ICollection object containing the keys of the IDictionary object.
        /// </summary>
        public ICollection Keys
        {
            get
            {
                if (null == this._oKeys)
                {
                    lock (this._oLock)
                    {
                        if (null == this._oKeys)
                        {
                            this._oKeys = new MaxIndexCollection(this._nIndexCurrent);
                            if (null != this._aItemRecentList)
                            {
                                for (int lnK = 0; lnK < this._aItemRecentList.Length; lnK++)
                                {
                                    if (null != this._aItemRecentList[lnK] && null != this._aItemRecentList[lnK].Key)
                                    {
                                        this._oKeys.Add(this._aItemRecentList[lnK]);
                                    }
                                }
                            }

                            if (null != this._aItemSortedList)
                            {
                                for (int lnK = 0; lnK < this._aItemSortedList.Length; lnK++)
                                {
                                    if (null != this._aItemSortedList[lnK] && null != this._aItemSortedList[lnK].Key)
                                    {
                                        this._oKeys.Add(this._aItemSortedList[lnK]);
                                    }
                                }
                            }
                        }
                    }
                }

                return this._oKeys;
            }
        }

        /// <summary>
        /// Gets an ICollection object containing the values in the IDictionary object.
        /// </summary>
        public ICollection Values
        {
            get
            {
                if (null == this._oValues)
                {
                    lock (this._oLock)
                    {
                        if (null == this._oValues)
                        {
                            this._oValues = new MaxIndexCollection(this._nIndexCurrent, MaxIndexCollection.CollectionTypeValue);
                            if (null != this._aItemRecentList)
                            {
                                for (int lnK = 0; lnK < this._aItemRecentList.Length; lnK++)
                                {
                                    if (null != this._aItemRecentList[lnK] && null != this._aItemRecentList[lnK].Key)
                                    {
                                        this._oValues.Add(this._aItemRecentList[lnK]);
                                    }
                                }
                            }

                            if (null != this._aItemSortedList)
                            {
                                for (int lnK = 0; lnK < this._aItemSortedList.Length; lnK++)
                                {
                                    if (null != this._aItemSortedList[lnK] && null != this._aItemSortedList[lnK].Key)
                                    {
                                        this._oValues.Add(this._aItemSortedList[lnK]);
                                    }
                                }
                            }
                        }
                    }
                }

                return this._oValues;
            }
        }

        /// <summary>
        /// Gets an Id used to show that the element was not found in the index
        /// </summary>
        public Guid NotFoundId
        {
            get
            {
                if (this._oNotFoundId.Equals(Guid.Empty))
                {
                    this._oNotFoundId = Guid.NewGuid();
                }

                return this._oNotFoundId;
            }
        }

        /// <summary>
        /// Gets or sets the value of each item based on the key.
        /// </summary>
        /// <param name="lsKey">Unique key for the item.</param>
        /// <returns>Value for the item.</returns>
        public object this[string lsKey]
        {
            get
            {
                MaxIndexItemStructure loItem = this.GetMatch(lsKey);
                if (null != loItem)
                {
                    return loItem.Value;
                }

                return null;
            }

            set
            {
                this.Add(lsKey, value);
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
                return this[((MaxIndexCollection)this.Keys)[lnKey]];
            }
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="loKey">Unique key for the item.</param>
        /// <returns>Value for the item.</returns>
        public object this[object loKey]
        {
            get
            {
                string lsKey = MaxConvertLibrary.ConvertToString(typeof(object), loKey);
                MaxIndexItemStructure loItem = this.GetMatch(lsKey);
                if (null != loItem)
                {
                    return loItem.Value;
                }

                return null;
            }

            set
            {
                string lsKey = MaxConvertLibrary.ConvertToString(typeof(object), loKey);
                this.Add(lsKey, value);
            }
        }

        /// <summary>
        /// Checks the index to see if the requested key is in the list.
        /// </summary>
        /// <param name="lsKey">Unique key used to look up item.</param>
        /// <returns>true if the item is in the list, false if it is not.</returns>
        public bool Contains(string lsKey)
        {
            int lnMatchIndex = this.GetMatchIndex(lsKey);
            if (int.MinValue == lnMatchIndex)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Uses the key part to find the matching value if one exists
        /// </summary>
        /// <param name="laKeyPartList">Array of strings used to make up the key</param>
        /// <returns>Value of key if found, Guid matching a NotFoundId if not found.</returns>
        public object FindValue(params string[] laKeyPartList)
        {
            if (laKeyPartList.Length == 1 && this.Contains(laKeyPartList[0]))
            {
                return this[laKeyPartList[0]];
            }
            else if (laKeyPartList.Length > 1)
            {
                int lnKeyLength = 0;
                foreach (string lsKeyPart in laKeyPartList)
                {
                    lnKeyLength += lsKeyPart.Length;
                }

                if (null != this._aItemRecentList)
                {
                    foreach (MaxIndexItemStructure loItem in this._aItemRecentList)
                    {
                        if (null != loItem && null != loItem.Key)
                        {
                            if (lnKeyLength == loItem.Key.Length)
                            {
                                int lnCurrentStart = 0;
                                bool lbIsMatch = true;
                                foreach (string lsKeyPart in laKeyPartList)
                                {
                                    if (loItem.Key.IndexOf(lsKeyPart, lnCurrentStart) != lnCurrentStart)
                                    {
                                        lbIsMatch = false;
                                    }

                                    lnCurrentStart += lsKeyPart.Length;
                                }

                                if (lbIsMatch)
                                {
                                    return loItem.Value;
                                }
                            }
                        }
                    }
                }

                if (null != this._aItemSortedList)
                {
                    foreach (MaxIndexItemStructure loItem in this._aItemSortedList)
                    {
                        if (null != loItem && null != loItem.Key)
                        {
                            if (lnKeyLength == loItem.Key.Length)
                            {
                                int lnCurrentStart = 0;
                                bool lbIsMatch = true;
                                foreach (string lsParam in laKeyPartList)
                                {
                                    if (loItem.Key.IndexOf(lsParam, lnCurrentStart) != lnCurrentStart)
                                    {
                                        lbIsMatch = false;
                                    }

                                    lnCurrentStart += lsParam.Length;
                                }

                                if (lbIsMatch)
                                {
                                    return loItem.Value;
                                }
                            }
                        }
                    }
                }
            }

            return this.NotFoundId;
        }

        /// <summary>
        /// Adds an item to the index.
        /// </summary>
        /// <param name="lsKey">Unique key used to look up the item.</param>
        /// <param name="loValue">Value stored based on the key.</param>
        public void Add(string lsKey, object loValue)
        {
            lock (this._oLock)
            {
                //// Get the existing item matching the key and update it
                MaxIndexItemStructure loCurrent = this.GetMatch(lsKey);
                if (null != loCurrent)
                {
                    if (loCurrent.Value != loValue)
                    {
                        loCurrent.Value = loValue;
                        this._dLastChanged = DateTime.UtcNow;
                    }
                }
                else
                {
                    //// Create a new item for the index
                    MaxIndexItemStructure loItem = new MaxIndexItemStructure(_nIndexCurrent++, lsKey, loValue);
                    //// Create the recent list if it does not exist
                    if (null == this._aItemRecentList)
                    {
                        this._aItemRecentList = new MaxIndexItemStructure[this._nRecentIndexMax];
                    }

                    //// Add to the recent list if it is not full.  Reserve one at the end for the Combine Lists method.
                    if (this._nRecentIndexNext < this._aItemRecentList.Length - 1)
                    {
                        this._aItemRecentList[this._nRecentIndexNext] = loItem;
                        this._nCountRecent++;
                        this._nRecentIndexNext++;
                    }
                    else
                    {
                        this.CombineLists(loItem);
                    }

                    this._dLastChanged = DateTime.UtcNow;
                }

                this._oKeys = null;
                this._oValues = null;
            }
        }

        /// <summary>
        /// Adds an item to the index.
        /// </summary>
        /// <param name="lsKey">Unique key used to look up the item.</param>
        /// <param name="loValue">Value stored based on the key.</param>
        public void AddWithoutKeyCheck(string lsKey, object loValue)
        {
            lock (this._oLock)
            {
                //// Create a new item for the index
                MaxIndexItemStructure loItem = new MaxIndexItemStructure(_nIndexCurrent++, lsKey, loValue);
                //// Create the recent list if it does not exist
                if (null == this._aItemRecentList)
                {
                    this._aItemRecentList = new MaxIndexItemStructure[this._nRecentIndexMax];
                }

                //// Add to the recent list if it is not full.  Reserve one at the end for the Combine Lists method.
                if (this._nRecentIndexNext < this._aItemRecentList.Length - 1)
                {
                    this._aItemRecentList[this._nRecentIndexNext] = loItem;
                    this._nCountRecent++;
                    this._nRecentIndexNext++;
                }
                else
                {
                    this.CombineLists(loItem);
                }

                this._dLastChanged = DateTime.UtcNow;
                this._oKeys = null;
                this._oValues = null;
            }
        }

        /// <summary>
        /// Adds an item to the index generating an incrementing key.
        /// </summary>
        /// <param name="loValue">value to add.</param>
        public void Add(object loValue)
        {
            string lsKey = PadKey(this.Count);
            this.Add(lsKey, loValue);
        }

        /// <summary>
        /// Removes the item with the matching key.
        /// </summary>
        /// <param name="lsKey">Unique key for the item.</param>
        public void Remove(string lsKey)
        {
            lock (this._oLock)
            {
                //// Get the index of the item that matches this key
                int lnMatchIndex = this.GetMatchIndex(lsKey);
                if (lnMatchIndex != int.MinValue)
                {
                    if (lnMatchIndex >= 0)
                    {
                        //// Clear the matching item in the sorted array
                        this._aItemSortedList[lnMatchIndex] = null;
                        //// Move all other items down one to keep it filled in
                        for (int lnIndex = lnMatchIndex; lnIndex < this._aItemSortedList.Length - 1; lnIndex++)
                        {
                            this._aItemSortedList[lnIndex] = this._aItemSortedList[lnIndex + 1];
                        }

                        //// Make the last one null
                        this._aItemSortedList[this._nCountSorted - 1] = null;
                        this._nCountSorted--;
                    }
                    else
                    {
                        //// Clear the matching item in the recent array
                        int lnRecentIndex = -1 * (lnMatchIndex + 1);
                        this._aItemRecentList[lnRecentIndex] = null;
                        this._nCountRecent--;
                    }

                    this._dLastChanged = DateTime.UtcNow;
                }

                this._oKeys = null;
                this._oValues = null;
            }
        }

        /// <summary>
        /// Clears all values stored in the Index.
        /// </summary>
        public void Clear()
        {
            lock (this._oLock)
            {
                this._aItemSortedList = null;
                this._nCountSorted = 0;
                this._aItemRecentList = null;
                this._nCountRecent = 0;
                this._nRecentIndexMax = 5;
                this._nRecentIndexNext = 0;
                this._dLastChanged = DateTime.UtcNow;
                this._oKeys = null;
                this._oValues = null;
            }
        }

        /// <summary>
        /// Gets a copy of the current sorted item index.
        /// </summary>
        /// <returns>List items in the index.</returns>
        public MaxIndexItemStructure[] GetSortedList()
        {
            lock (this._oLock)
            {
                if (this._nCountRecent > 0)
                {
                    this.CombineLists(null);
                }

                MaxIndexItemStructure[] laList = new MaxIndexItemStructure[this._nCountSorted];
                if (this._nCountSorted > 0 && null != this._aItemSortedList)
                {
                    int lnNext = 0;
                    for (int lnK = 0; lnK < this._aItemSortedList.Length; lnK++)
                    {
                        if (null != this._aItemSortedList[lnK])
                        {
                            laList[lnNext] = new MaxIndexItemStructure(lnNext, this._aItemSortedList[lnK].Key, this._aItemSortedList[lnK].Value);
                            lnNext++;
                        }
                    }
                }

                return laList;
            }
        }

        /// <summary>
        /// Gets a list of the current keys for the index sorted.
        /// </summary>
        /// <returns>List of sorted keys.</returns>
        public string[] GetSortedKeyList()
        {
            lock (this._oLock)
            {
                if (this._nCountRecent > 0)
                {
                    this.CombineLists(null);
                }

                string[] laR = new string[0];
                string[] laKeyList = new string[this._nCountSorted];
                if (this._nCountSorted > 0 && null != this._aItemSortedList)
                {
                    int lnNext = 0;
                    for (int lnK = 0; lnK < this._aItemSortedList.Length; lnK++)
                    {
                        if (null != this._aItemSortedList[lnK] && null != this._aItemSortedList[lnK].Key)
                        {
                            laKeyList[lnNext] = this._aItemSortedList[lnK].Key;
                            lnNext++;
                        }
                    }

                    if (lnNext < laKeyList.Length)
                    {
                        laR = new string[lnNext];
                        for (int lnN = 0; lnN < laR.Length; lnN++)
                        {
                            laR[lnN] = laKeyList[lnN];
                        }
                    }
                    else
                    {
                        laR = laKeyList;
                    }
                }

                return laR;
            }
        }

        /// <summary>
        /// Gets a list of all values sorted by key.
        /// </summary>
        /// <returns>List of values sorted by key.</returns>
        public object[] GetSortedValueList()
        {
            lock (this._oLock)
            {
                string[] laKeyList = this.GetSortedKeyList();
                object[] laValueList = new object[laKeyList.Length];
                for (int lnK = 0; lnK < laKeyList.Length; lnK++)
                {
                    laValueList[lnK] = this[laKeyList[lnK]];
                }

                return laValueList;
            }
        }

        /// <summary>
        /// Gets a value as a string.  Uses the default if it does not exist.
        /// </summary>
        /// <param name="lsKey">the key to look up</param>
        /// <param name="lsDefault">the default string to use</param>
        /// <returns>The value as a string, or the default if not found</returns>
        public string GetValueString(string lsKey, string lsDefault)
        {
            lock (this._oLock)
            {
                if (this.Contains(lsKey))
                {
                    return MaxConvertLibrary.ConvertToString(typeof(object), this[lsKey]);
                }
            }

            return lsDefault;
        }

        /// <summary>
        /// Gets a value as a string.  Uses the default if it does not exist.
        /// </summary>
        /// <param name="lsKey">the key to look up</param>
        /// <returns>The value as a string, or the string.empty if not found</returns>
        public string GetValueString(string lsKey)
        {
            return this.GetValueString(lsKey, string.Empty);
        }

        /// <summary>
        /// Adds an element with the provided key and value to the IDictionary object.
        /// </summary>
        /// <param name="loKey">The Object to use as the key of the element to add.</param>
        /// <param name="loValue">The Object to use as the value of the element to add.</param>
        public void Add(object loKey, object loValue)
        {
            string lsKey = loKey.ToString();
            this.Add(lsKey, loValue);
        }

        /// <summary>
        /// Determines whether the IDictionary object contains an element with the specified key.
        /// </summary>
        /// <param name="loKey">The key to locate in the IDictionary object.</param>
        /// <returns>true if the IDictionary contains an element with the key; otherwise, false.</returns>
        public bool Contains(object loKey)
        {
            string lsKey = loKey.ToString();
            return this.Contains(lsKey);
        }

        /// <summary>
        /// Copies the elements of the ICollection to an Array, starting at a particular Array index. (Inherited from ICollection.)
        /// </summary>
        /// <param name="loArray">The one-dimensional Array that is the destination of the elements copied from ICollection. The Array must have zero-based indexing.</param>
        /// <param name="lnCount">The zero-based index in array at which copying begins.</param>
        public void CopyTo(System.Array loArray, int lnCount)
        {
            this.CombineLists(null);
            this._aItemSortedList.CopyTo(loArray, lnCount);
        }

        /// <summary>
        /// Removes the element with the specified key from the IDictionary object.
        /// </summary>
        /// <param name="loKey">The key of the element to remove.</param>
        public void Remove(object loKey)
        {
            string lsKey = loKey.ToString();
            this.Remove(lsKey);
        }

        /// <summary>
        /// Returns an IEnumerator object for the IEnumerable object.
        /// </summary>
        /// <returns>An IEnumerator object for the IEnumerable object.</returns>
        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new MaxIndexEnumerator(this);
        }

        /// <summary>
        /// Returns an IDictionaryEnumerator object for the IDictionary object.
        /// </summary>
        /// <returns>An IDictionaryEnumerator object for the IDictionary object.</returns>
        public IDictionaryEnumerator GetEnumerator()
        {
            return new MaxIndexEnumerator(this);
        }

        protected MaxIndexItemStructure[] Items
        {
            get
            {
                int lnLengthSorted = 0;
                int lnLengthRecent = 0;
                if (null != this._aItemRecentList)
                {
                    lnLengthRecent += this._aItemRecentList.Length;
                }

                if (null != this._aItemSortedList)
                {
                    lnLengthSorted += this._aItemSortedList.Length;
                }

                MaxIndexItemStructure[] laR = new MaxIndexItemStructure[lnLengthRecent + lnLengthSorted];
                if (null != this._aItemRecentList)
                {
                    this._aItemRecentList.CopyTo(laR, 0);
                }

                if (null != this._aItemSortedList)
                {
                    this._aItemSortedList.CopyTo(laR, lnLengthRecent);
                }

                return laR;
            }
        }

        /// <summary>
        /// Gets half the difference between 2 integers.
        /// </summary>
        /// <param name="lnStart">Start integer.</param>
        /// <param name="lnEnd">End Integer.</param>
        /// <returns>Half the difference.</returns>
        private static int GetDiff(int lnStart, int lnEnd)
        {
            return (int)Math.Floor((double)(lnEnd - lnStart) / 2);
        }

        /// <summary>
        /// Finds the matching array index of a sorted index list.
        /// </summary>
        /// <param name="loIndex">Sorted index list.</param>
        /// <param name="lsKey">Unique key for the item.</param>
        /// <param name="lnCheck">Index to check.</param>
        /// <param name="lnCheckMin">Minimum index to check.</param>
        /// <param name="lnCheckMax">Maximum index to check.</param>
        /// <returns>Matching array index or MinValue if no match found.</returns>
        private static int GetMatchIndexFromSortedArray(MaxIndexItemStructure[] loIndex, string lsKey, int lnCheck, int lnCheckMin, int lnCheckMax)
        {
            if (null == loIndex || lnCheck < 0 || lnCheck >= loIndex.Length)
            {
                //// declare no match
                return int.MinValue;
            }

            //// Get the current item to see if it matches the key
            MaxIndexItemStructure loCheck = loIndex[lnCheck];
            if (null != loCheck)
            {
                int lnCompare = loCheck.Key.CompareTo(lsKey);
                if (lnCompare == 0)
                {
                    //// Current item matches the key, so return the index to it
                    return lnCheck;
                }
                else if (lnCheck == lnCheckMax || lnCheck == lnCheckMin)
                {
                    //// Declare no match if we are already checking either end of the list
                    return int.MinValue;
                }
                else if (lnCompare < 0)
                {
                    //// Current item is in front of the key item
                    //// select next item to check as half way between the current item and the max item checked
                    int lnDiff = GetDiff(lnCheck, lnCheckMax);
                    //// Always change the current index by at least 1
                    if (lnDiff == 0)
                    {
                        lnDiff = 1;
                    }

                    int lnCheckNext = lnCheck + lnDiff;
                    if (lnCheckNext > lnCheckMax)
                    {
                        //// Declare no match
                        return int.MinValue;
                    }

                    //// Set the minimum to the current, since we know the key has to be after the current
                    if (lnCheck > lnCheckMin)
                    {
                        lnCheckMin = lnCheck;
                    }

                    return GetMatchIndexFromSortedArray(loIndex, lsKey, lnCheckNext, lnCheckMin, lnCheckMax);
                }
                else if (lnCompare > 0)
                {
                    //// Current item is in after the key item
                    //// select next item to check as half way between the current item and the min item checked
                    int lnDiff = GetDiff(lnCheckMin, lnCheck);

                    //// Always change the current index by at least 1
                    if (lnDiff == 0)
                    {
                        lnDiff = 1;
                    }

                    int lnCheckNext = lnCheck - lnDiff;
                    if (lnCheckNext < lnCheckMin)
                    {
                        //// Declare no match
                        return int.MinValue;
                    }

                    //// Set the maximum to the current, since we know the key has to be after the current
                    if (lnCheck < lnCheckMax)
                    {
                        lnCheckMax = lnCheck;
                    }

                    return GetMatchIndexFromSortedArray(loIndex, lsKey, lnCheckNext, lnCheckMin, lnCheckMax);
                }
            }
            else
            {
                return GetMatchIndexFromSortedArray(loIndex, lsKey, lnCheck - 1, lnCheckMin, lnCheckMax);
            }

            return int.MinValue;
        }

        /// <summary>
        /// Checks text to make sure is all numbers.
        /// </summary>
        /// <param name="lsKey">Text to check.</param>
        /// <returns>True if all characters are numbers.</returns>
        private static bool IsInteger(string lsKey)
        {
            bool lbR = true;
            if (null == lsKey || lsKey.Length.Equals(0) || lsKey.Length > 10)
            {
                lbR = false;
            }

            if (lbR)
            {
                string lsNumber = "0123456789";
                for (int lnC = 0; lnC < lsKey.Length && lbR; lnC++)
                {
                    if (lsNumber.IndexOf(lsKey[lnC]) < 0)
                    {
                        lbR = false;
                    }
                }

                if (lbR && lsKey.Length == 10)
                {
                    lbR = false;
                    if (int.Parse(lsKey[0].ToString()) < 2)
                    {
                        lbR = true;
                    }
                    else if (int.Parse(lsKey[0].ToString()) == 2)
                    {
                        if (int.Parse(lsKey[1].ToString()) < 1)
                        {
                            lbR = true;
                        }
                        else if (int.Parse(lsKey[1].ToString()) == 1)
                        {
                            if (int.Parse(lsKey[2].ToString()) < 4)
                            {
                                lbR = true;
                            }
                            else if (int.Parse(lsKey[2].ToString()) == 4)
                            {
                                if (int.Parse(lsKey[3].ToString()) < 7)
                                {
                                    lbR = true;
                                }
                                else if (int.Parse(lsKey[3].ToString()) == 7)
                                {
                                    if (int.Parse(lsKey[4].ToString()) < 4)
                                    {
                                        lbR = true;
                                    }
                                    else if (int.Parse(lsKey[4].ToString()) == 4)
                                    {
                                        if (int.Parse(lsKey[5].ToString()) < 8)
                                        {
                                            lbR = true;
                                        }
                                        else if (int.Parse(lsKey[5].ToString()) == 8)
                                        {
                                            if (int.Parse(lsKey[6].ToString()) < 3)
                                            {
                                                lbR = true;
                                            }
                                            else if (int.Parse(lsKey[6].ToString()) == 3)
                                            {
                                                if (int.Parse(lsKey[7].ToString()) < 6)
                                                {
                                                    lbR = true;
                                                }
                                                else if (int.Parse(lsKey[7].ToString()) == 6)
                                                {
                                                    if (int.Parse(lsKey[8].ToString()) < 4)
                                                    {
                                                        lbR = true;
                                                    }
                                                    else if (int.Parse(lsKey[8].ToString()) == 4)
                                                    {
                                                        if (int.Parse(lsKey[9].ToString()) <= 7)
                                                        {
                                                            lbR = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return lbR;
        }

        /// <summary>
        /// Pads an integer key with zeros to make sorting by string work for keys.
        /// </summary>
        /// <param name="lnKey">Integer to pad.</param>
        /// <returns>Integer with 9 characters padded with 0 to the left.</returns>
        private static string PadKey(int lnKey)
        {
            string lsKey = lnKey.ToString();
            if (lsKey.Length < 10)
            {
                lsKey = _sZero.Substring(0, 10 - lsKey.Length) + lsKey;
            }

            return lsKey;
        }

        /// <summary>
        /// Combines the sorted list and the recent lists.
        /// </summary>
        /// <param name="loItem">Item to add.  Can be null if nothing is going to be added.</param>
        private void CombineLists(MaxIndexItemStructure loItem)
        {
            lock (this._oLock)
            {
                //// Get count of items to be held in new list
                int lnTotalNew = 0;

                if (null != this._aItemSortedList)
                {
                    for (int lnIndex = 0; lnIndex < this._aItemSortedList.Length; lnIndex++)
                    {
                        if (null != this._aItemSortedList[lnIndex])
                        {
                            lnTotalNew++;
                        }
                    }
                }

                if (null == this._aItemRecentList || this._aItemRecentList.Length.Equals(0))
                {
                    this._aItemRecentList = new MaxIndexItemStructure[1];
                }
                else
                {
                    for (int lnIndex = 0; lnIndex < this._aItemRecentList.Length; lnIndex++)
                    {
                        if (null != this._aItemRecentList[lnIndex])
                        {
                            lnTotalNew++;
                        }
                    }
                }

                if (null != loItem)
                {
                    //// Add the newest item to the end of the recent list.
                    lnTotalNew++;
                    this._aItemRecentList[this._aItemRecentList.Length - 1] = loItem;
                }

                //// Create a new array to hold as many items as should be in the sorted index
                MaxIndexItemStructure[] laIndexList = new MaxIndexItemStructure[lnTotalNew];

                //// Move the currently sorted items to the start of the new list
                int lnNext = 0;
                if (null != this._aItemSortedList)
                {
                    try
                    {
                        for (int lnIndex = 0; lnIndex < this._aItemSortedList.Length; lnIndex++)
                        {
                            MaxIndexItemStructure loItemSorted = this._aItemSortedList[lnIndex];
                            //// Skip any that were removed
                            if (null != loItemSorted)
                            {
                                laIndexList[lnNext] = loItemSorted;
                                lnNext++;
                            }
                        }
                    }
                    catch (Exception loE)
                    {
                        throw new MaxException("Error attempting to combine lists during sorted list.  Sorted Count is [" + this._nCountSorted.ToString() + "].  Sorted Length is [" + this._aItemSortedList.Length + "].", loE);
                    }
                }

                bool lbIsSorted = true;
                //// Add the recent items to the end of the list
                if (null != this._aItemRecentList)
                {
                    try
                    {
                        for (int lnIndex = 0; lnIndex < this._aItemRecentList.Length; lnIndex++)
                        {
                            MaxIndexItemStructure loItemRecent = this._aItemRecentList[lnIndex];
                            //// Skip any that were removed
                            if (null != loItemRecent)
                            {
                                laIndexList[lnNext] = loItemRecent;
                                if (lnNext > 0)
                                {
                                    //// Check to see if the newest item comes after the previous item
                                    if (this.CompareToEvenIfNull(laIndexList[lnNext - 1], loItemRecent) > 0)
                                    {
                                        //// Add the latest item before the previous item to speed up sorting later.
                                        laIndexList[lnNext] = laIndexList[lnNext - 1];
                                        laIndexList[lnNext - 1] = loItemRecent;
                                        lbIsSorted = false;
                                    }
                                }

                                lnNext++;
                            }
                        }
                    }
                    catch (Exception loE)
                    {
                        throw new MaxException("Error attempting to combine lists during recent list.  Sorted Count is [" + this._nCountSorted.ToString() + "].  Sorted Length is [" + this._aItemSortedList.Length + "]. Recent Count is [" + this._nCountRecent.ToString() + "].  Recent Length is [" + this._aItemRecentList.Length + "].", loE);
                    }
                }

                ////Update the sorted count
                this._nCountSorted = laIndexList.Length;
                //// Determine a new max length for the recent index if the current one is large
                if (this._nCountSorted > 100)
                {
                    double lnNewMaxDbl = Math.Floor((double)this._nCountSorted / 10);
                    this._nRecentIndexMax = (int)lnNewMaxDbl;
                    if (this._nRecentIndexMax > 100)
                    {
                        this._nRecentIndexMax = 100;
                    }
                }

                //// Reset the recent list
                this._aItemRecentList = null;
                this._nCountRecent = 0;
                this._nRecentIndexNext = 0;
                this._aItemSortedList = laIndexList;

                if (!lbIsSorted)
                {
                    this.ReSort();
                }
            }
        }

        /// <summary>
        /// Gets the matching item for the key.
        /// </summary>
        /// <param name="lsKey">Unique key for the item.</param>
        /// <returns>IndexItem that matches the key.</returns>
        private MaxIndexItemStructure GetMatch(string lsKey)
        {
            lock (this._oLock)
            {
                int lnMatchIndex = this.GetMatchIndex(lsKey);
                if (lnMatchIndex == int.MinValue)
                {
                    return null;
                }
                else if (lnMatchIndex >= 0)
                {
                    return this._aItemSortedList[lnMatchIndex];
                }
                else
                {
                    int lnRecentIndex = (-1 * lnMatchIndex) - 1;
                    return this._aItemRecentList[lnRecentIndex];
                }
            }
        }

        /// <summary>
        /// Gets the index that matches the key.  Returns MinValue if no match.
        /// </summary>
        /// <param name="lsKey">Unique key for the item.</param>
        /// <returns>Index where the key matches.</returns>
        private int GetMatchIndex(string lsKey)
        {
            return this.GetMatchIndexConditional(lsKey);
        }

#if net2  || netcore1 || netstandard1_2
        /// <summary>
        /// Gets the index that matches the key.  Returns MinValue if no match.
        /// </summary>
        /// <param name="lsKey">Unique key for the item.</param>
        /// <returns>Index where the key matches.</returns>
        private int GetMatchIndexConditional(string lsKey)
        {
            lock (this._oLock)
            {
                if (this._nRecentIndexNext > this._nRecentForceCombine)
                {
                    this.CombineLists(null);
                }

                int lnR = this.RecentGetMatchIndex(lsKey);

                if (lnR == int.MinValue)
                {
                    int lnCheck = 0;
                    if (null != this._aItemSortedList)
                    {
                        //// Check to see if the index is an incremental set of integers that can be found more quickly
                        if (IsInteger(lsKey))
                        {
                            int lnOut = int.MinValue;
                            if (int.TryParse(lsKey, out lnOut))
                            {
                                lnCheck = lnOut;
                                if (lnCheck >= 0 && lnCheck < this._aItemSortedList.Length)
                                {
                                    if (null != this._aItemSortedList[lnCheck] && null != this._aItemSortedList[lnCheck].Key)
                                    {
                                        string lsKeyCheck = this._aItemSortedList[lnCheck].Key;
                                        if (IsInteger(lsKeyCheck))
                                        {
                                            int lnKey = int.Parse(lsKeyCheck);
                                            if (lnKey == lnCheck)
                                            {
                                                lnR = lnCheck;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (lnR == int.MinValue)
                        {
                            lnCheck = 0;
                            int lnMin = -1;
                            int lnMax = this._nCountSorted;
                            if (this._nCountSorted > 3)
                            {
                                //// Select an index in the middle of the list to check first
                                lnCheck = GetDiff(0, this._nCountSorted);
                            }

                            lnR = GetMatchIndexFromSortedArray(this._aItemSortedList, lsKey, lnCheck, lnMin, lnMax);
                        }
                    }
                }
                else
                {
                    //// Return a negative number to indicate that this item is in the recent list
                    lnR = -1 * (lnR + 1);
                }

                return lnR;
            }
        }
#endif
        /// <summary>
        /// Gets the index from recently added items.
        /// </summary>
        /// <param name="lsKey">Unique key for the item.</param>
        /// <returns>index in recent array that matches the key.</returns>
        private int RecentGetMatchIndex(string lsKey)
        {
            lock (this._oLock)
            {
                if (null != this._aItemRecentList)
                {
                    for (int lnK = 0; lnK < this._nRecentIndexNext; lnK++)
                    {
                        if (null != this._aItemRecentList[lnK] && this._aItemRecentList[lnK].Key == lsKey)
                        {
                            return lnK;
                        }
                    }
                }
            }

            return int.MinValue;
        }

        /// <summary>
        /// Resort the list that should always be sorted by key.
        /// </summary>
        private void ReSort()
        {
            lock (this._oLock)
            {
                if (this._aItemSortedList.Length > 80)
                {
                    int lnPivotStart = GetDiff(0, this._aItemSortedList.Length - 1);
                    this.QuickSort(0, this._aItemSortedList.Length - 1, lnPivotStart);
                }
                else
                {
                    this.BubbleSort();
                }
            }
        }

        /// <summary>
        /// Sort the array using a bubble sort.
        /// </summary>
        private void BubbleSort()
        {
            lock (this._oLock)
            {
                bool lbChanged = false;
                for (int lnIndex = 0; lnIndex < this._aItemSortedList.Length - 1; lnIndex++)
                {
                    if (null != this._aItemSortedList[lnIndex] || null != this._aItemSortedList[lnIndex + 1])
                    {
                        if (this.CompareToEvenIfNull(this._aItemSortedList[lnIndex], this._aItemSortedList[lnIndex + 1]) > 0)
                        {
                            MaxIndexItemStructure loTemp = this._aItemSortedList[lnIndex + 1];
                            this._aItemSortedList[lnIndex + 1] = this._aItemSortedList[lnIndex];
                            this._aItemSortedList[lnIndex] = loTemp;
                            lbChanged = true;
                        }
                    }
                }

                if (lbChanged)
                {
                    this.BubbleSort();
                }
            }
        }

        /// <summary>
        /// Sorted the array using a quicksort.
        /// </summary>
        /// <param name="lnLow">Index of low element for sorting.</param>
        /// <param name="lnHigh">Index of high element for sorting.</param>
        /// <param name="lnPivot">Index of Pivot element for sorting.</param>
        private void QuickSort(int lnLow, int lnHigh, int lnPivot)
        {
            lock (this._oLock)
            {
                int lnLowCurrent = lnLow;
                int lnHighCurrent = lnHigh;
                MaxIndexItemStructure loPivot = this._aItemSortedList[lnPivot];

                //// Loop until the current low meets the current high
                while (lnLowCurrent <= lnHighCurrent)
                {
                    //// Increase the low until the value is larger than the pivot
                    while (this.CompareToEvenIfNull(this._aItemSortedList[lnLowCurrent], loPivot) < 0)
                    {
                        lnLowCurrent++;
                    }

                    //// Decrease the high until the value is smaller than the pivot
                    while (this.CompareToEvenIfNull(this._aItemSortedList[lnHighCurrent], loPivot) > 0)
                    {
                        lnHighCurrent--;
                    }

                    //// The current low is higher than the pivot or the same as the pivot
                    //// The current high is lower than the pivot or the same as the pivot
                    if (lnLowCurrent <= lnHighCurrent)
                    {
                        //// Swap the current low and high item so they will be on the correct sides of each other
                        MaxIndexItemStructure loTemp = this._aItemSortedList[lnLowCurrent];
                        this._aItemSortedList[lnLowCurrent] = this._aItemSortedList[lnHighCurrent];
                        this._aItemSortedList[lnHighCurrent] = loTemp;

                        //// Move on to the next set of items
                        lnLowCurrent++;
                        lnHighCurrent--;
                    }
                }

                //// Do the same thing on the values lower than the pivot
                if (lnLow < lnHighCurrent)
                {
                    //// Select a pivot in the middle of the low and high element
                    int lnPivotLow = lnLow + GetDiff(lnLow, lnHighCurrent);
                    this.QuickSort(lnLow, lnHighCurrent, lnPivotLow);
                }

                //// Do the same thing on the values higher than the pivot
                if (lnLowCurrent < lnHigh)
                {
                    //// Select a pivot in the middle of the low and high element
                    int lnPivotHigh = lnLowCurrent + GetDiff(lnLowCurrent, lnHigh);
                    this.QuickSort(lnLowCurrent, lnHigh, lnPivotHigh);
                }
            }
        }

        /// <summary>
        /// Compares two MaxIndexItemStructure objects even if one or both is null
        /// </summary>
        /// <param name="loLeft">Obect to use compare</param>
        /// <param name="loRight">Object to target compare</param>
        /// <returns>0 if match, -1 if less, 1 if more.</returns>
        private int CompareToEvenIfNull(MaxIndexItemStructure loLeft, MaxIndexItemStructure loRight)
        {
            if (null == loLeft && null == loRight)
            {
                return 0;
            }
            else if (null == loRight)
            {
                return -1;
            }
            else if (null == loLeft)
            {
                return 1;
            }

            return loLeft.CompareTo(loRight);
        }
    }
}
