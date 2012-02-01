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
using Polarsoft.Utilities;

namespace Polarsoft.Diplomacy
{
	/// <summary>Represents a route that a unit may take.
	/// </summary>
	public class Route
	{
        private ProvinceCollection route;

		private Route()
		{
            route = new ProvinceCollection();
		}

		/// <summary>Creates a new <see cref="Route"/> instance.
		/// </summary>
		/// <param name="province">Province.</param>
		public Route(Province province)
		{
            route = new ProvinceCollection();
			Add(province);
		}

		/// <summary>Creates a new <see cref="Route"/> instance, based on another <see cref="Route"/>.
		/// </summary>
		/// <param name="route">Route.</param>
		public Route(Route route)
		{
            Robustness.ValidateArgumentNotNull("route", route);
            this.route = new ProvinceCollection(route.route);
		}

		/// <summary>Creates a new <see cref="Route"/> instance, based on another <see cref="Route"/>.
		/// The <see cref="Province"/> is added on to the end of the new <see cref="Route"/>.
		/// </summary>
		/// <param name="route">Route.</param>
		/// <param name="province">Province.</param>
		public Route(Route route, Province province)
			: this(route)
		{
			this.route.Add(province);
		}


		/// <summary>Gets the provinces that this <see cref="Route"/> passes through.
		/// </summary>
		/// <value></value>
        public ProvinceCollection Provinces
		{
			get
			{
				return route;
			}
		}

		/// <summary>Returns the representation of this instance as a <see cref="string"/>
		/// </summary>
		/// <returns>The representation of this instance as a <see cref="string"/></returns>
		public override string ToString()
		{
			if (route.Count == 0)
			{
				return "";
			}
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("From ");
			sb.Append(Start.ToString());
			sb.Append(" To ");
			sb.Append(End.ToString());
			sb.Append(" Via: ");
			foreach(Province p in Via.Provinces)
			{
				sb.Append(p.ToString());
				sb.Append(" ");
			}
			return sb.ToString();
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
	
			Route route = obj as Route;
			if(this.route.Count != route.route.Count)
			{
				return false;
			}
			for(int ii = 0; ii < this.route.Count; ++ii)
			{
				if (!this.route[ii].Equals(route.route[ii]))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>Serves as a hash function for a particular type, suitable for use in hashing algorithms and data structures like a hash table.
		/// </summary>
		/// <returns>A hash code for the current <see cref="Route"/>.</returns>
		/// <remarks>See <see cref="object.GetHashCode"/> for more information.</remarks>
		public override int GetHashCode()
		{
			int iHash = 0;
			foreach(Province p in route)
			{
				iHash = iHash ^ p.GetHashCode();
			}
			return iHash;
		}

		/// <summary>Compares two Route objects for equality.
		/// </summary>
		/// <param name="o1">The first Route.</param>
		/// <param name="o2">The second Route</param>
		/// <returns><c>True</c> if the objects are equal, <c>false</c> otherwise.</returns>
		public static bool operator ==(Route o1, Route o2)
		{
			return object.Equals(o1, o2);
		}

		/// <summary>Compares two Route objects for inequality.
		/// </summary>
		/// <param name="o1">The first Route.</param>
		/// <param name="o2">The second Route</param>
		/// <returns><c>False</c> if the objects are equal, <c>true</c> otherwise.</returns>
		public static bool operator !=(Route o1, Route o2)
		{
			return !object.Equals(o1, o2);
		}

		/// <summary>Adds a province to the end of the route.
		/// </summary>
		/// <param name="province">Province.</param>
		public void Add(Province province)
		{
			route.Add(province);
		}

		/// <summary>Gets the starting <see cref="Province"/>.
		/// </summary>
		/// <value></value>
		public Province Start
		{
			get
			{
				if (route.Count == 0)
				{
					return null;
				}
				return route[0];
			}
		}

		/// <summary>Gets the final <see cref="Province"/>.
		/// </summary>
		/// <value></value>
		public Province End
		{
			get
			{
				if (route.Count == 0)
				{
					return null;
				}
				return route[route.Count - 1];
			}
		}

		/// <summary>Gets the <see cref="Province">Provinces</see> that the route passes through.
		/// This does not include the <see cref="Route.Start"/> or the <see cref="Route.End"/>.
		/// </summary>
		/// <value></value>
		public Route Via
		{
			get
			{
				if (route.Count < 2)
				{
					return new Route();
				}
				else
				{
					Route ret = new Route(this);
					ret.route.RemoveAt(ret.route.Count - 1);
					ret.route.RemoveAt(0);
					return ret;
				}
			}
		}
	}
}
