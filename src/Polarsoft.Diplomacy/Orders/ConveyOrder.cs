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
	/// <summary>Represents an order to convey a unit.
	/// </summary>
	public class ConveyOrder : UnitOrder
	{
		private Province targetProvince;
		/// <summary>Gets the target province of the conveying order.
		/// </summary>
		/// <value>The <see cref="Province"/> that the convey order is targeting.</value>
		public Province TargetProvince
		{
			get
			{
				return targetProvince;
			}
		}

		private Unit conveyedUnit;
		/// <summary>Gets the conveyed <see cref="Unit"/>.
		/// </summary>
		/// <value>The conveyed <see cref="Unit"/>.</value>
		public Unit ConveyedUnit
		{
			get
			{
				return conveyedUnit;
			}
		}

		/// <summary>Creates a new <see cref="ConveyOrder"/> instance.
		/// </summary>
		/// <param name="unit">The <see cref="Unit"/> that this order concerns.</param>
		/// <param name="conveyedUnit">Conveyed unit.</param>
		/// <param name="targetProvince">Target province.</param>
		public ConveyOrder(Unit unit, Unit conveyedUnit, Province targetProvince)
			:base (OrderType.Convey, unit)
		{
			this.conveyedUnit = conveyedUnit;
            this.targetProvince = targetProvince;
		}

        /// <summary>Returns <c>true</c> if this order is valid.
        /// </summary>
        public override bool IsValid
        {
            get
            {
                return Unit.UnitType == UnitType.Fleet &&
                    conveyedUnit.UnitType == UnitType.Army &&
                    Unit.Province.IsSea &&
                    conveyedUnit.Province.IsCoastal;
            }
        }
        
        /// <summary>Returns the representation of this instance as a <see cref="string"/>
		/// </summary>
		/// <returns>The representation of this instance as a <see cref="string"/></returns>
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"Convey Order: Unit: {0}, ConveyedUnit: {1}, Target: {2}",
                Unit.ToString(), this.conveyedUnit.ToString(), this.targetProvince.ToString());
		}
	}
}
