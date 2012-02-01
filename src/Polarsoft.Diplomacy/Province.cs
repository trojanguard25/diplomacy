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

namespace Polarsoft.Diplomacy
{
	/// <summary>Represents the type of <see cref="Province"/>.
	/// </summary>
	public enum ProvinceType
	{
		/// <summary>An inland province that is not a supply center.
        /// </summary>
		InlandNonSupplyCenter,

		/// <summary>An inland province that is a supply center.
        /// </summary>
		InlandSupplyCenter,

		/// <summary>An sea province that is not a supply center.
        /// </summary>
		SeaNonSupplyCenter,

		/// <summary>An inland province that is a supply center.
        /// </summary>
		SeaSupplyCenter,

		/// <summary>An coastal province that is not a supply center.
        /// </summary>
		CoastalNonSupplyCenter,

		/// <summary>An coastal province that is a supply center.
        /// </summary>
		CoastalSupplyCenter,

		/// <summary>An bicoastal province that is not a supply center.
        /// </summary>
		BicostalNonSupplyCenter,

		/// <summary>An bicoastal province that is a supply center.
        /// </summary>
		BicostalSupplyCenter
	}

	/// <summary>Represents a province.
	/// </summary>
	public class Province
	{
		private string name;
		private ProvinceType type;
		private LocationCollection locations;
		private Unit unit;
		private Power owningPower;
		private object tag;
        private ProvinceCollection adjacentProvinces;

		/// <summary>Creates a new <see cref="Province"/> instance.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="type">Type.</param>
		public Province(string name, ProvinceType type)
		{
			this.name = name;
            this.type = type;
            this.locations = new LocationCollection();
            this.adjacentProvinces = new ProvinceCollection();
		}

		/// <summary>Gets the name of the <see cref="Province"/>.
		/// </summary>
		/// <value>The name of the <see cref="Province"/>.</value>
		public string Name
		{
			get
			{
				return name;
			}
		}

		/// <summary>Gets the <see cref="ProvinceType"/> of the <see cref="Province"/>.
		/// </summary>
		/// <value>The <see cref="ProvinceType"/> of the <see cref="Province"/>.</value>
		public ProvinceType Type
		{
			get
			{
				return type;
			}
		}

		/// <summary>Gets the locations that are available in the <see cref="Province"/>.
		/// </summary>
        /// <value>A list of the locations in the <see cref="Province"/>.</value>
		public LocationCollection Locations
		{
			get
			{
				return locations;
			}
		}

		/// <summary>Gets or sets the <see cref="Power"/> that owns this <see cref="Province"/>.
		/// </summary>
		/// <value>The <see cref="Power"/> that owns this <see cref="Province"/>.</value>
		public Power OwningPower
		{
			get
			{
				return owningPower;
			}
			set
			{
				owningPower = value;
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

		/// <summary>Gets the adjacent provinces of this <see cref="Province"/>.
		/// </summary>
		/// <value>The <see cref="Province">Provinces</see> that are adjacent to this province.</value>
		public ProvinceCollection AdjacentProvinces
		{
			get
			{
				return adjacentProvinces;
			}
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
	
			Province province = obj as Province;
			return this.name == province.name;
		}

		/// <summary>Serves as a hash function for a particular type, suitable for use in hashing algorithms and data structures like a hash table.
		/// </summary>
		/// <returns>A hash code for the current <see cref="Province"/>.</returns>
		/// <remarks>See <see cref="object.GetHashCode"/> for more information.</remarks>
		public override int GetHashCode()
		{
			return name.GetHashCode();
		}

		/// <summary>Compares two Province objects for equality.
		/// </summary>
		/// <param name="o1">The first Province.</param>
		/// <param name="o2">The second Province</param>
		/// <returns><c>True</c> if the objects are equal, <c>false</c> otherwise.</returns>
		public static bool operator ==(Province o1, Province o2)
		{
			return object.Equals(o1, o2);
		}

		/// <summary>Compares two Province objects for inequality.
		/// </summary>
		/// <param name="o1">The first Province.</param>
		/// <param name="o2">The second Province</param>
		/// <returns><c>False</c> if the objects are equal, <c>true</c> otherwise.</returns>
		public static bool operator !=(Province o1, Province o2)
		{
			return !object.Equals(o1, o2);
		}

		/// <summary>Returns the representation of this instance as a <see cref="string"/>
		/// </summary>
		/// <returns>The representation of this instance as a <see cref="string"/></returns>
		public override string ToString()
		{
			return name;
		}
		
		/// <summary>Gets a value indicating whether this <see cref="Province"/> is a supply center.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this <see cref="Province"/> is a supply center; otherwise, <c>false</c>.
		/// </value>
		public bool IsSupplyCenter
		{
			get
			{
				return type == ProvinceType.InlandSupplyCenter
					|| type == ProvinceType.SeaSupplyCenter
					|| type == ProvinceType.CoastalSupplyCenter
					|| type == ProvinceType.BicostalSupplyCenter;
			}
		}

		/// <summary>Gets a value indicating if a fleet can move to this <see cref="Province"/>.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if if a fleet can move to this <see cref="Province"/>; otherwise, <c>false</c>.
		/// </value>
		public bool FleetMoveable
		{
			get
			{
				return type == ProvinceType.SeaNonSupplyCenter
					|| type == ProvinceType.SeaSupplyCenter
					|| type == ProvinceType.CoastalNonSupplyCenter
					|| type == ProvinceType.CoastalSupplyCenter
					|| type == ProvinceType.BicostalNonSupplyCenter
					|| type == ProvinceType.BicostalSupplyCenter;
			}
		}

		/// <summary>Gets a value indicating if an army can move to this <see cref="Province"/>.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if if an army can move to this <see cref="Province"/>; otherwise, <c>false</c>.
		/// </value>
		public bool ArmyMoveable
		{
			get
			{
				return type == ProvinceType.InlandNonSupplyCenter
					|| type == ProvinceType.InlandSupplyCenter
					|| type == ProvinceType.CoastalNonSupplyCenter
					|| type == ProvinceType.CoastalSupplyCenter
					|| type == ProvinceType.BicostalNonSupplyCenter
					|| type == ProvinceType.BicostalSupplyCenter;
			}
		}

		/// <summary>Gets a value indicating whether this <see cref="Province"/> is coastal.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this <see cref="Province"/> is coastal; otherwise, <c>false</c>.
		/// </value>
		public bool IsCoastal
		{
			get
			{
				return type == ProvinceType.CoastalNonSupplyCenter
					|| type == ProvinceType.CoastalSupplyCenter
					|| type == ProvinceType.BicostalNonSupplyCenter
					|| type == ProvinceType.BicostalSupplyCenter;
			}
		}

		/// <summary>Gets a value indicating whether this <see cref="Province"/> is bicoastal.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this <see cref="Province"/> is bicoastal; otherwise, <c>false</c>.
		/// </value>
		public bool IsBicoastal
		{
			get
			{
				return type == ProvinceType.BicostalNonSupplyCenter
					|| type == ProvinceType.BicostalSupplyCenter;
			}
		}

		/// <summary>Gets a value indicating whether this <see cref="Province"/> is inland.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this <see cref="Province"/> is inland; otherwise, <c>false</c>.
		/// </value>
		public bool IsInland
		{
			get
			{
				return type == ProvinceType.InlandNonSupplyCenter
					|| type == ProvinceType.InlandSupplyCenter;
			}
		}

		/// <summary>Gets a value indicating whether this <see cref="Province"/> is sea.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this <see cref="Province"/> is sea; otherwise, <c>false</c>.
		/// </value>
		public bool IsSea
		{
			get
			{
				return type == ProvinceType.SeaNonSupplyCenter
					|| type == ProvinceType.SeaSupplyCenter;
			}
		}

		/// <summary>Gets a value indicating whether someone can build on this <see cref="Province"/>.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if someone can build on this <see cref="Province"/>; otherwise, <c>false</c>.
		/// </value>
		public bool IsOpenForBuilding
		{
			get
			{
				return IsSupplyCenter && unit == null;
			}
		}

        /// <summary>Gets a value indicating whether a specific power can build on this <see cref="Province"/>.
        /// </summary>
        /// <param name="power">Power.</param>
        /// <returns><c>true</c> if the power can build on this <see cref="Province"/>; otherwise, <c>false</c>.</returns>
        public bool CanBuild(Power power)
        {
            return IsOpenForBuilding && ( this.owningPower == power );
        }

        /// <summary>Gets a value indicating whether a specific <see cref="Power"/> can build a specific <see cref="UnitType"/> on this <see cref="Province"/>
        /// </summary>
        /// <param name="power">Power</param>
        /// <param name="unitType">Unit type.</param>
        /// <returns><c>true</c> if the specified <see cref="Power"/> can build the specified <see cref="UnitType"/> on this <see cref="Province"/>; otherwise, <c>false</c>.</returns>
        public bool CanBuildUnitType(Power power, UnitType unitType)
		{
			switch(unitType)
			{
				case UnitType.Army :
					return CanBuild(power) && ArmyMoveable;
				case UnitType.Fleet :
					return CanBuild(power) && FleetMoveable;
				default :
					throw new ArgumentException(Properties.ErrorMessages.Province_UnknownUnitType, "unitType");
			}
		}

        /// <summary>Gets a value indicating whether a specific <see cref="Power"/> can build a fleet on this <see cref="Province"/>
        /// </summary>
        /// <param name="power">Power</param>
        /// <returns><c>true</c> if the specified <see cref="Power"/> can build a fleet on this <see cref="Province"/>; otherwise, <c>false</c>.</returns>
		public bool CanBuildFleet(Power power)
		{
			return CanBuildUnitType(power, UnitType.Fleet);
		}

        /// <summary>Gets a value indicating whether a specific <see cref="Power"/> can build an army on this <see cref="Province"/>
        /// </summary>
        /// <param name="power">Power</param>
        /// <returns><c>true</c> if the specified <see cref="Power"/> can build an army on this <see cref="Province"/>; otherwise, <c>false</c>.</returns>
        public bool CanBuildArmy(Power power)
		{
			return CanBuildUnitType(power, UnitType.Army);
		}

		/// <summary>Gets the unit that is in this <see cref="Province"/>.
		/// </summary>
		/// <value>
		///		<c>null</c> if there isn't a unit in this <see cref="Province"/>; otherwise a <see cref="Unit"/>
		/// </value>
		public Unit Unit
		{
			get
			{
				return unit;
			}
			set
			{
				unit = value;
			}
		}

		/// <summary>Gets the <see cref="Location"/> in the <see cref="Province"/>.
		/// </summary>
		/// <param name="unitType">Unit type.</param>
		/// <param name="coast">Coast.</param>
		/// <returns><c>null</c> if the <see cref="Location"/> doesn't exist.</returns>
		public Location GetLocation(UnitType unitType, Coast coast)
		{
			Location loc = new Location(this, unitType, coast);
			if (!locations.Contains(loc))
			{
				return null;
			}
			else
			{
                return locations[locations.IndexOf(loc)];
			}
		}

		internal void UpdateAdjacentProvinces()
		{
			foreach (Location loc in locations)
			{
				foreach (Location adjLoc in loc.AdjacentLocations)
				{
					if (!adjacentProvinces.Contains(adjLoc.Province))
					{
						adjacentProvinces.Add(adjLoc.Province);
					}
				}
			}
		}
	}
}
