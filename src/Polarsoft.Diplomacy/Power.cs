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
using System.Collections.Generic;

namespace Polarsoft.Diplomacy
{
	/// <summary>Represents a power (player) of the game.
	/// </summary>
	public class Power
	{
		private string name;
		private UnitCollection units;
		private ProvinceCollection ownedSupplyProvinces;
        private ProvinceCollection homeProvinces;
		private object tag;

		/// <summary>Creates a new <see cref="Power"/> instance.
		/// </summary>
		/// <param name="name">Name.</param>
		public Power(string name)
		{
            this.name = name;
            this.units = new UnitCollection();
            this.ownedSupplyProvinces = new ProvinceCollection();
            this.homeProvinces = new ProvinceCollection();
		}

		/// <summary>Gets the name.
		/// </summary>
		/// <value>The name of the <see cref="Power"/>.</value>
		public string Name
		{
			get
			{
				return name;
			}
		}

		/// <summary>Gets the units this power has.
		/// </summary>
		/// <value>The <see cref="Unit">Units</see> that this <see cref="Power"/> currently controls.</value>
		public UnitCollection Units
		{
			get
			{
				return units;
			}
		}

		/// <summary>Gets the owned supply provinces.
		/// </summary>
		/// <value>The <see cref="Province">Provinces</see> that this <see cref="Power"/> currently controls.</value>
		public ProvinceCollection OwnedSupplyProvinces
		{
			get
			{
				return ownedSupplyProvinces;
			}
		}

		/// <summary>Gets the home provinces (the provinces that this power can build in.
		/// </summary>
		/// <value>A <see cref="IList"/> of the provinces that are considered "Home"
		/// <see cref="Province">Provinces</see> of this <see cref="Power"/>.
		/// </value>
		/// <remarks>A "Home" <see cref="Province">Provinces</see> is a <see cref="Province"/>
		/// where the <see cref="Power"/> can build new units.
		/// </remarks>
        public ProvinceCollection HomeProvinces
		{
			get
			{
				return homeProvinces;
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
				return tag;
			}
			set
			{
				tag = value;
			}
		}

		/// <summary>Returns the representation of this instance as a <see cref="string"/>
		/// </summary>
		/// <returns>The representation of this instance as a <see cref="string"/></returns>
		public override string ToString()
		{
			return name;
		}

		/// <summary>Compares this instance with another object.
		/// </summary>
		/// <param name="obj">The object to compare with.</param>
		/// <returns>True if the objects are equal, false otherwise.</returns>
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
	
			Power power = obj as Power;
			return name == power.name;
		}

		/// <summary>Serves as a hash function for a particular type, suitable for use in hashing algorithms and data structures like a hash table.
		/// </summary>
		/// <returns>A hash code for the current <see cref="Power"/>.</returns>
		/// <remarks>See <see cref="object.GetHashCode"/> for more information.</remarks>
		public override int GetHashCode()
		{
			return name.GetHashCode();
		}

		/// <summary>Compares two Power objects for equality.
		/// </summary>
		/// <param name="o1">The first Power.</param>
		/// <param name="o2">The second Power</param>
		/// <returns><c>True</c> if the objects are equal, <c>false</c> otherwise.</returns>
		public static bool operator ==(Power o1, Power o2)
		{
			return object.Equals(o1, o2);
		}

		/// <summary>Compares two Power objects for inequality.
		/// </summary>
		/// <param name="o1">The first Power.</param>
		/// <param name="o2">The second Power</param>
		/// <returns><c>False</c> if the objects are equal, <c>true</c> otherwise.</returns>
		public static bool operator !=(Power o1, Power o2)
		{
			return !object.Equals(o1, o2);
		}
	}
}
