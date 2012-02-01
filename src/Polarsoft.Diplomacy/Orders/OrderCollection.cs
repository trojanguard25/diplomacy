#region Copyright 2004-2006 Fredrik Blom, Licenced under the MIT Licence
/*
 * Copyright (c) 2004-2006 Fredrik Blom
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

namespace Polarsoft.Diplomacy.Orders
{
    /// <summary>A collection of <see cref="Order"/>.
    /// </summary>
    public class OrderCollection : ICollection<Order>
    {
        private List<Order> orders = new List<Order>();

        #region ICollection<Order> Members

        /// <summary>Adds an item to the collection.
        /// </summary>
        /// <param name="item">The object to add to the collection</param>
        public void Add(Order item)
        {
            orders.Add(item);
        }

        /// <summary>Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            orders.Clear();
        }

        /// <summary>Determines whether the collection contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns><c>true</c> if item is found in the collection; otherwise, <c>false</c>.</returns>
        public bool Contains(Order item)
        {
            return orders.Contains(item);
        }

        /// <summary>Copies the elements of the collection to an <c>Array</c>, starting at a particular <c>Array</c> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see>Array</see> that is the destination of the elements copied from collection.
        /// The <c>Array</c> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(Order[] array, int arrayIndex)
        {
            orders.CopyTo(array, arrayIndex);
        }

        /// <summary>Gets the number of elements contained in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return orders.Count;
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
        public bool Remove(Order item)
        {
            return orders.Remove(item);
        }

        #endregion

        #region IEnumerable<Order> Members

        /// <summary>Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A IEnumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<Order> GetEnumerator()
        {
            return orders.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="System.Collections.IEnumerator"/> that can be used to iterate through the collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)orders).GetEnumerator();
        }

        #endregion
    }
}
