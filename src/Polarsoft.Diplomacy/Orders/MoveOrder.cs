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
using System.Diagnostics;
using System.Globalization;

namespace Polarsoft.Diplomacy.Orders
{
	/// <summary>Represents an order to move a unit.
	/// </summary>
	public class MoveOrder : UnitOrder
	{
		private Location targetLocation;
		/// <summary>Gets the target <see cref="Location"/>.
		/// </summary>
		/// <value>The <see cref="Location"/> that the move is targeting.</value>
		public Location TargetLocation
		{
			get
			{
				return this.targetLocation;
			}
		}

		/// <summary>Creates a new <see cref="MoveOrder"/> instance.
		/// </summary>
		/// <param name="unit">The <see cref="Unit"/> that is moving.</param>
		/// <param name="location">The <see cref="Location"/> the unit is moving to.</param>
		public MoveOrder(Unit unit, Location location)
			: base (OrderType.Move, unit)
		{
			this.targetLocation = location;
		}

        /// <summary>Returns <c>true</c> if this order is valid.
        /// </summary>
        public override bool IsValid
        {
            get
            {
                return Unit.Location.AdjacentLocations.Contains(targetLocation);
            }
        }

		/// <summary>Returns the representation of this instance as a <see cref="string"/>
		/// </summary>
		/// <returns>The representation of this instance as a <see cref="string"/></returns>
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"Move Order: Unit: {0}, Target: {1}",
				Unit.ToString(), this.targetLocation.ToString());
		}
	}
}
