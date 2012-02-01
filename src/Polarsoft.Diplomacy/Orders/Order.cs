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
using System.Collections;
using System.Globalization;

namespace Polarsoft.Diplomacy.Orders
{
	/// <summary>Represents the type of <see cref="Order"/>.
	/// </summary>
	public enum OrderType
	{
		/// <summary>A hold order.
        /// </summary>
		Hold,

		/// <summary>A move order.
        /// </summary>
		Move,

		/// <summary>A move by convoy order.
        /// </summary>
		MoveByConvoy,

		/// <summary>A support hold order.
        /// </summary>
		SupportHold,

		/// <summary>A support move order.
        /// </summary>
		SupportMove,

		/// <summary>A convey order.
        /// </summary>
		Convey,

		/// <summary>A retreat order.
        /// </summary>
		Retreat,

		/// <summary>A disband order.
        /// </summary>
		Disband,

		/// <summary>A build order.
        /// </summary>
		Build,

		/// <summary>A remove order.
        /// </summary>
		Remove,

		/// <summary>A waive build order.
        /// </summary>
		WaiveBuild
	}

	/// <summary>Represents a single order.
	/// </summary>
	public abstract class Order
	{
		private OrderType orderType;
		private object tag;

		/// <summary>Creates a new <see cref="Order"/> instance.
		/// </summary>
		/// <param name="orderType">Order type.</param>
		protected Order (OrderType orderType)
		{
			this.orderType = orderType;
		}

		/// <summary>Gets the order type.
		/// </summary>
		/// <value>The <see cref="OrderType"/> of the order.</value>
		public OrderType OrderType
		{
			get
			{
                return this.orderType;
			}
		}

		/// <summary>Gets or sets the tag.
		/// </summary>
		/// <remarks>
		/// The tag can be used by the user of the instance.
		/// </remarks>
		/// <value>The object that has been associated with this instance.</value>
		public object Tag
		{
			get
			{
                return this.tag;
			}
			set
			{
                this.tag = value;
			}
		}

        /// <summary>Returns <c>true</c> if this order is valid.
        /// </summary>
        public abstract bool IsValid
        {
            get;
        }
	}
}
