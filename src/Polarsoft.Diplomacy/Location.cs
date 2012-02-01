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
using System.Diagnostics;
using System.Collections.Generic;
using Polarsoft.Utilities;

namespace Polarsoft.Diplomacy
{

	/// <summary>Represents a coast for a coastal province.
	/// </summary>
	public enum Coast
	{
		/// <summary>No coast.
        /// </summary>
		NoCoast,

		/// <summary>The north coast.
        /// </summary>
		North,

		/// <summary>The northeast coast.
        /// </summary>
		Northeast,

		/// <summary>The east coast.
        /// </summary>
		East,

		/// <summary>The southeast coast.
        /// </summary>
		Southeast,

		/// <summary>The south coast.
        /// </summary>
		South,

		/// <summary>The southwast coast.
        /// </summary>
		Southwest,

		/// <summary>The west coast.
        /// </summary>
		West,

		/// <summary>The northwest coast.
        /// </summary>
		Northwest
	}

	/// <summary>Represents a location in a province.
	/// </summary>
	/// <remarks>
	/// A location is a place that an army or a fleet can move to.
	/// A location contains information such as what coast a fleet is on,
	/// and possible locations that can be reached from the current location.
	/// </remarks>
	public class Location
	{
		private UnitType unitType = UnitType.Army;
		private Province province;
		private Coast coast = Coast.NoCoast;
		private LocationCollection adjacentLocations;
		private object tag;

		/// <summary>Creates a new <see cref="Location"/> instance.
		/// </summary>
		/// <param name="province">Province.</param>
		/// <param name="unitType">Unit type.</param>
		/// <param name="coast">Coast.</param>
		public Location(Province province, UnitType unitType, Coast coast)
		{
            Robustness.ValidateArgumentNotNull("province", province);

			//Make sure that we do not enter a coast for an Army
			Debug.Assert(unitType == UnitType.Army ? coast == Coast.NoCoast : true);
			//Make sure that the province is a coastal province if there is a coast specified
			Debug.Assert(coast != Coast.NoCoast ? province.IsCoastal : true);

            this.province = province;
            this.unitType = unitType;
            this.coast = coast;
            this.adjacentLocations = new LocationCollection();
		}

		/// <summary>Creates a new <see cref="Location"/> instance.
		/// </summary>
		/// <param name="province">Province.</param>
		/// <param name="unitType">Unit type.</param>
		public Location(Province province, UnitType unitType)
		{
            Robustness.ValidateArgumentNotNull("province", province);
            
            //Make sure that a bicoastal province has a coast for a fleet
			Debug.Assert(!(province.IsBicoastal && unitType == UnitType.Fleet));

            this.province = province;
            this.unitType = unitType;
            this.coast = Coast.NoCoast;
            this.adjacentLocations = new LocationCollection();
		}

		
		/// <summary>Gets the unit type that this location is for.
		/// </summary>
		/// <value>The <see cref="UnitType"/> that this location is for.</value>
		public UnitType UnitType
		{
			get
			{
				return unitType;
			}
		}

		/// <summary>Gets the province that this location is associated with.
		/// </summary>
		/// <value>The <see cref="Province"/> that this location is situated in.</value>
		public Province Province
		{
			get
			{
				return province;
			}
		}

		/// <summary>Gets the coast that this location is associated with.
		/// </summary>
		/// <value>The coast that this location is associated with.</value>
		public Coast Coast
		{
			get
			{
				return coast;
			}
		}

		/// <summary>Gets the adjacent locations of this location.
		/// </summary>
        /// <value>A <see cref="IList"/> of the locations that are adjacent to this location.</value>
		public LocationCollection AdjacentLocations
		{
			get
			{
				return adjacentLocations;
			}
		}

		/// <summary>Gets the adjacent provinces of this location.
		/// </summary>
        /// <value>A <see cref="IList"/> of the provinces that are adjacent to this location.</value>
		public ProvinceCollection AdjacentProvinces
		{
			get
			{
				return province.AdjacentProvinces;
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
			string s = province.ToString();
			if (coast != Coast.NoCoast)
			{
				s += " (" + coast.ToString() + ")";
			}
			return s;
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
	
			Location location = obj as Location;
			return (this.province == location.province 
				&& this.coast == location.coast
				&& this.unitType == location.unitType);
		}

		/// <summary>Serves as a hash function for a particular type, suitable for use in hashing algorithms and data structures like a hash table.
		/// </summary>
		/// <returns>A hash code for the current <see cref="Location"/>.</returns>
		/// <remarks>See <see cref="object.GetHashCode"/> for more information.</remarks>
		public override int GetHashCode()
		{
			return province.GetHashCode() ^ coast.GetHashCode() ^ unitType.GetHashCode();
		}

		/// <summary>Compares two Location objects for equality.
		/// </summary>
		/// <param name="o1">The first Location.</param>
		/// <param name="o2">The second Location</param>
		/// <returns><c>True</c> if the objects are equal, <c>false</c> otherwise.</returns>
		public static bool operator ==(Location o1, Location o2)
		{
			return object.Equals(o1, o2);
		}

		/// <summary>Compares two Location objects for inequality.
		/// </summary>
		/// <param name="o1">The first Location.</param>
		/// <param name="o2">The second Location</param>
		/// <returns><c>False</c> if the objects are equal, <c>true</c> otherwise.</returns>
		public static bool operator !=(Location o1, Location o2)
		{
			return !object.Equals(o1, o2);
		}
	}
}
