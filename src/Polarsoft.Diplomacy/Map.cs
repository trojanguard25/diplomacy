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

namespace Polarsoft.Diplomacy
{
	/// <summary>Represents a map for Diplomacy.
	/// </summary>
	public class Map
	{
		private string mapName = "";
        private Dictionary<string, Province> provinces;
		private object tag;
		private ProvinceCollection supplyCenters;

		/// <summary>Creates a new <see cref="Map"/> instance.
		/// </summary>
		public Map()
		{
            this.provinces = new Dictionary<string, Province>();
            this.supplyCenters = new ProvinceCollection();
		}

		/// <summary>Gets or sets the name of the map.
		/// </summary>
		/// <value>The name of the map.</value>
		public string Name
		{
			get
			{
                return this.mapName;
			}
			set
			{
                this.mapName = value;
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

		/// <summary>Gets the provinces.
		/// </summary>
		/// <value>The provinces.</value>
        public Dictionary<string, Province> Provinces
		{
			get
			{
                return this.provinces;
			}
		}

		/// <summary>Gets the supply center provinces.
		/// </summary>
		/// <value>The supply center provinces.</value>
        public ProvinceCollection SupplyCenterProvinces
		{
			get
			{
                return this.supplyCenters;
			}
		}

		/// <summary>Returns the representation of this instance as a <see cref="string"/>
		/// </summary>
		/// <returns>The representation of this instance as a <see cref="string"/></returns>
		public override string ToString()
		{
            return this.mapName;
		}

		/// <summary>Finalizes this instance.
		/// </summary>
		/// <remarks>
		/// This call should be made after all provinces and locations have been
		/// updated in the map. This will update all provinces and locations
		/// with their adjacent provinces and locations, as well as update
		/// the map with the supply center provinces.
		/// </remarks>
		public void FinalSetup()
		{
			foreach(Province p in provinces.Values)
			{
				p.UpdateAdjacentProvinces();
				if (p.IsSupplyCenter)
				{
                    this.supplyCenters.Add(p);
				}
			}
		}
	}
}
