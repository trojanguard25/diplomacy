#region Copyright 2006 Fredrik Blom, Licenced under the MIT Licence
/*
 * Copyright (c) 2006 Fredrik Blom
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy,
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software
 * is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
 * ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 * The original author of this work can be reached at:
 * fredrik.blom@polarsoft.se
*/
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace Polarsoft.Diplomacy
{
    /// <summary>A collection of <see cref="Location"/>.
    /// </summary>
    public class LocationCollection : IList<Location>
    {
        private List<Location> items = new List<Location>();

        #region ICollection<Location> Members

        /// <summary>Adds an item to the collection.
        /// </summary>
        /// <param name="item">The object to add to the collection</param>
        public void Add(Location item)
        {
            items.Add(item);
        }

        /// <summary>Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            items.Clear();
        }

        /// <summary>Determines whether the collection contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns><c>true</c> if item is found in the collection; otherwise, <c>false</c>.</returns>
        public bool Contains(Location item)
        {
            return items.Contains(item);
        }

        /// <summary>Copies the elements of the collection to an <c>Array</c>, starting at a particular <c>Array</c> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see>Array</see> that is the destination of the elements copied from collection.
        /// The <c>Array</c> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(Location[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        /// <summary>Gets the number of elements contained in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return items.Count;
            }
        }

        /// <summary>Gets a value indicating whether the collection is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>Removes the first occurrence of a specific object from the collection.
        /// </summary>
        /// <param name="item">The object to remove from the collection.</param>
        /// <returns><c>true</c> if item was successfully removed from the collection; otherwise, <c>false</c>.
        /// This method also returns <c>false</c> if item is not found in the original collection.</returns>
        public bool Remove(Location item)
        {
            return items.Remove(item);
        }

        #endregion

        #region IEnumerable<Location> Members

        /// <summary>Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A IEnumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<Location> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="System.Collections.IEnumerator"/> that can be used to iterate through the collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)items).GetEnumerator();
        }

        #endregion

        #region IList<Location> Members

        /// <summary>Determines the index of a specific item in the collection.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns>The index of item if found in the list; otherwise, -1.</returns>
        public int IndexOf(Location item)
        {
            return items.IndexOf(item);
        }

        /// <summary>Inserts an item to the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert into the IList.</param>
        public void Insert(int index, Location item)
        {
            items.Insert(index, item);
        }

        /// <summary>Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            items.RemoveAt(index);
        }

        /// <summary>Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public Location this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                items[index] = value;
            }
        }

        #endregion
    }
}
