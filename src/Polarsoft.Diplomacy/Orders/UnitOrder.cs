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

namespace Polarsoft.Diplomacy.Orders
{
	/// <summary>An order that comes from a <see cref="Unit"/>. This is an abstract base class
	/// for all orders that occur during the movement phases.
	/// </summary>
	public abstract class UnitOrder : Order
	{
		private Unit unit;
		/// <summary>Gets the unit that this order concerns.
		/// </summary>
		/// <value>The unit that is doing the conveying.</value>
		public Unit Unit
		{
			get
			{
                return this.unit;
			}
		}

		/// <summary>Creates a new <see cref="UnitOrder"/> instance.
		/// </summary>
		/// <param name="orderType">Order type.</param>
		/// <param name="unit">The <see cref="Unit"/> that this order concerns.</param>
		protected UnitOrder(OrderType orderType, Unit unit)
			: base (orderType)
		{
            if( unit == null )
            {
                throw new ArgumentNullException("unit");
            }
            this.unit = unit;
		}
	}
}
