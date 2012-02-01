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
using System.Collections.Generic;

namespace Polarsoft.Diplomacy
{
	/// <summary>Represents the type of a <see cref="Unit"/>.
	/// </summary>
	public enum UnitType
	{
		/// <summary>An army.
        /// </summary>
		Army,
		/// <summary>A fleet.
        /// </summary>
		Fleet
	}

	/// <summary>Represents a unit.
	/// </summary>
	public class Unit
	{
		private UnitType type;
		private Power power;
		private Location location;
		private LocationCollection retreatLocations;
		private object tag;
		private bool mustRetreat;

		/// <summary>Creates a new <see cref="Unit"/> instance.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="power">Power.</param>
		/// <param name="location">Location.</param>
		public Unit(UnitType type, Power power, Location location)
		{
            this.type = type;
            this.power = power;
            this.location = location;
			this.retreatLocations = new LocationCollection();
		}

		/// <summary>Gets the <see cref="UnitType"/> of this <see cref="Unit"/>.
		/// </summary>
		/// <value>The <see cref="UnitType"/> of this <see cref="Unit"/>.</value>
		public UnitType UnitType
		{
			get
			{
				return type;
			}
		}

		/// <summary>Gets the <see cref="Power"/> this <see cref="Unit"/> belongs to.
		/// </summary>
		/// <value>The <see cref="Power"/> this <see cref="Unit"/> belongs to.</value>
		public Power Power
		{
			get
			{
				return power;
			}
		}

		/// <summary>Gets the location this unit is at.
		/// </summary>
		/// <value>The <see cref="Location"/> this <see cref="Unit"/> is at.</value>
		public Location Location
		{
			get
			{
				return location;
			}
		}

		/// <summary>Gets a value indicating whether the <see cref="Unit"/> must retreat.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the <see cref="Unit"/> must retreat; otherwise, <c>false</c>.
		/// </value>
		public bool MustRetreat
		{
			get
			{
				return mustRetreat || retreatLocations.Count > 0;
			}
			set
			{
				mustRetreat = value;
			}
		}

		/// <summary>Gets the retreat <see cref="Location">locations</see> that are available for this <see cref="Unit"/>.
		/// </summary>
        /// <value>A list of the possible retreat
		/// <see cref="Location">locations</see> of this <see cref="Unit"/>
		/// </value>
		public LocationCollection RetreatLocations
		{
			get
			{
				return retreatLocations;
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
			return "(" + power.ToString() + " " + type.ToString() + " " + location.ToString() + ")";
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
	
			Unit unit = obj as Unit;
			return power == unit.power
				&& type == unit.type
				&& location == unit.location;
		}

		/// <summary>Serves as a hash function for a particular type, suitable for use in hashing algorithms and data structures like a hash table.
		/// </summary>
		/// <returns>A hash code for the current <see cref="Unit"/>.</returns>
		/// <remarks>See <see cref="object.GetHashCode"/> for more information.</remarks>
		public override int GetHashCode()
		{
			return power.GetHashCode() ^ type.GetHashCode() ^ location.GetHashCode();
		}

		/// <summary>Compares two Unit objects for equality.
		/// </summary>
		/// <param name="o1">The first Unit.</param>
		/// <param name="o2">The second Unit</param>
		/// <returns><c>True</c> if the objects are equal, <c>false</c> otherwise.</returns>
		public static bool operator ==(Unit o1, Unit o2)
		{
			return object.Equals(o1, o2);
		}

		/// <summary>Compares two Unit objects for inequality.
		/// </summary>
		/// <param name="o1">The first Unit.</param>
		/// <param name="o2">The second Unit</param>
		/// <returns><c>False</c> if the objects are equal, <c>true</c> otherwise.</returns>
		public static bool operator !=(Unit o1, Unit o2)
		{
			return !object.Equals(o1, o2);
		}

		/// <summary>Determines if the <see cref="Unit"/> can move to the indicated <see cref="Province"/>.
		/// </summary>
		/// <param name="province">Province.</param>
		/// <returns><c>True</c> if the <see cref="Unit"/> can move to the <see cref="Province"/>; false otherwise.</returns>
		/// <remarks>This function will not check if the unit can be conveyed anywhere.</remarks>
		public bool CanMoveTo(Province province)
		{
			foreach(Location loc in location.AdjacentLocations)
			{
				if (loc.Province == province)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>Gets the <see cref="Province"/> this <see cref="Unit"/> currently occupies.
		/// </summary>
		/// <value>The <see cref="Province"/> this <see cref="Unit"/> currently occupies.</value>
		public Province Province
		{
			get
			{
				return location.Province;
			}
		}
	}
}
