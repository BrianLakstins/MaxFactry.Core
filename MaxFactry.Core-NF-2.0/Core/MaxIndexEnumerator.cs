// <copyright file="MaxIndexEnumerator.cs" company="Lakstins Family, LLC">
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
// <change date="8/29/2016" author="Brian A. Lakstins" description="Initial creation">
// <change date="9/13/2016" author="Brian A. Lakstins" description="Fix problem with loop and return of current entry">
// <change date="2/15/2021" author="Brian A. Lakstins" description="Fix enumerator issue">
// </changelog>
#endregion

namespace MaxFactry.Core
{
	using System;
	using System.Collections;

    /// <summary>
    /// Enumerator for the MaxIndex class
    /// </summary>
    public class MaxIndexEnumerator : IEnumerator, IDictionaryEnumerator
	{
        /// <summary>
        /// The index being enumerated
        /// </summary>
        private MaxIndex _oIndex = null;

        /// <summary>
        /// The number of the current element
        /// </summary>
        private int _nCurrent = -1;

        /// <summary>
        /// The list of keys of the index being enumerated
        /// </summary>
        private string[] _aKeyList = null;

        /// <summary>
        /// The last changed date for the index being enumerated when the enumerator is created
        /// </summary>
        private DateTime _dLastChanged = DateTime.UtcNow;

        /// <summary>
        /// The current entry
        /// </summary>
        private DictionaryEntry _oCurrent = new DictionaryEntry(null, null);

        /// <summary>
        /// Initializes a new instance of the MaxIndexEnumerator class
        /// </summary>
        /// <param name="loIndex">The index to enumerate</param>
        public MaxIndexEnumerator(MaxIndex loIndex)
        {
            this._oIndex = loIndex;
            this._dLastChanged = loIndex.LastChanged;
        }

        /// <summary>
        /// Gets the current element in the collection.
        /// If the collection is modified between MoveNext and Current, Current returns the element that it is set to, even if the enumerator is already invalidated.
        /// </summary>
        public object Current 
        { 
            get 
            {
                this.CheckRange();
                return this._oCurrent;
            } 
        }

        /// <summary>
        /// Gets the key of the current dictionary entry.
        /// </summary>
        public object Key 
        { 
            get 
            {
                return ((DictionaryEntry)this.Current).Key;
            } 
        }

        /// <summary>
        /// Gets the value of the current dictionary entry.
        /// </summary>
        public object Value 
        { 
            get 
            {
                return ((DictionaryEntry)this.Current).Value;
            } 
        }

        /// <summary>
        /// Gets both the key and the value of the current dictionary entry.
        /// </summary>
        public DictionaryEntry Entry 
        { 
            get 
            {
                this.CheckRange();
                return this._oCurrent; 
            } 
        }

        /// <summary>
        /// Gets a list of keys for the index
        /// </summary>
        protected string[] KeyList
        {
            get
            {
                if (null == this._aKeyList)
                {
                    this._aKeyList = this._oIndex.GetSortedKeyList();
                }

                return this._aKeyList;
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// If changes are made to the collection, such as adding, modifying, or deleting elements, the enumerator is irrecoverably invalidated and the next call to MoveNext or Reset throws an InvalidOperationException.
        /// </summary>
        /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
        public bool MoveNext()
        {
            if (this._dLastChanged != this._oIndex.LastChanged)
            {
                throw new InvalidOperationException("The underlying index has change and the enumerator is no longer valid.");
            }

            if (this._nCurrent < this.KeyList.Length - 1)
            {
                this._nCurrent++;
                this._oCurrent = new DictionaryEntry(this.KeyList[this._nCurrent], this._oIndex[this.KeyList[this._nCurrent]]);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// If changes are made to the collection, such as adding, modifying, or deleting elements, the enumerator is irrecoverably invalidated and the next call to MoveNext or Reset throws an InvalidOperationException.
        /// </summary>
        public void Reset()
        {
            if (this._dLastChanged != this._oIndex.LastChanged)
            {
                throw new InvalidOperationException("The underlying index has change and the enumerator is no longer valid.");
            }

            this._nCurrent = -1;
        }

        /// <summary>
        /// Checks to make sure the current element is within the range of the index
        /// </summary>
        private void CheckRange()
        {
            if (this._nCurrent < 0)
            {
                throw new InvalidOperationException("The current entry is before the start of the collection. MoveNext must be called.");
            }
            else if (this._nCurrent >= this._oIndex.Count)
            {
                throw new InvalidOperationException("The current entry is past the end of the collection.");
            }
        }
	}
}
