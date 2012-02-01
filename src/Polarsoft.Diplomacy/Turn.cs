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
using System.Globalization;
using Polarsoft.Utilities;

namespace Polarsoft.Diplomacy
{
	/// <summary>Represents a phase of a <see cref="Turn"/>.
	/// </summary>
	public enum Phase
	{
		/// <summary>Spring movement phase.
        /// </summary>
		Spring,
		/// <summary>Summer retreat phase.
        /// </summary>
		Summer,
		/// <summary>Fall movement phase.
        /// </summary>
		Fall,
		/// <summary>Autumn retreat phase.
        /// </summary>
		Autumn,
		/// <summary>Winter build phase.
        /// </summary>
		Winter
	}

	/// <summary>Represents a <see cref="Turn"/>.
	/// </summary>
	public class Turn
	{
		private Phase phase;
		private int year;

		/// <summary>Creates a new <see cref="Turn"/> instance.
		/// </summary>
		/// <param name="phase">Phase.</param>
		/// <param name="year">Year.</param>
		public Turn(Phase phase, int year)
		{
			this.phase = phase;
            this.year = year;
		}

		/// <summary>Gets the phase.
		/// </summary>
		/// <value>The <see cref="Phase"/> of the year.</value>
		public Phase Phase
		{
			get
			{
                return this.phase;
			}
		}

		/// <summary>Gets the year.
		/// </summary>
		/// <value>The current year.</value>
		public int Year
		{
			get
			{
                return this.year;
			}
		}

		/// <summary>Sets the current turn.
		/// </summary>
		/// <param name="year">Year.</param>
		/// <param name="phase">Phase.</param>
		public void SetCurrentTurn(Phase phase, int year)
		{
            this.phase = phase;
            this.year = year;
		}

		/// <summary>Returns the representation of this instance as a <see cref="string"/>
		/// </summary>
		/// <returns>The representation of this instance as a <see cref="string"/></returns>
		public override string ToString()
		{
            return this.phase.ToString() + " " + this.year.ToString(CultureInfo.InvariantCulture);
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
	
			Turn turn = obj as Turn;
			return this.phase == turn.phase
				&& this.year == turn.year;
		}

		/// <summary>Serves as a hash function for a particular type, suitable for use in hashing algorithms and data structures like a hash table.
		/// </summary>
		/// <returns>A hash code for the current <see cref="Turn"/>.</returns>
		/// <remarks>See <see cref="object.GetHashCode"/> for more information.</remarks>
		public override int GetHashCode()
		{
            return this.phase.GetHashCode() ^ this.year.GetHashCode();
		}

		/// <summary>Compares two Turn objects for equality.
		/// </summary>
		/// <param name="o1">The first Turn.</param>
		/// <param name="o2">The second Turn</param>
		/// <returns><c>True</c> if the objects are equal, <c>false</c> otherwise.</returns>
		public static bool operator ==(Turn o1, Turn o2)
		{
			return object.Equals(o1, o2);
		}

		/// <summary>Compares two Turn objects for inequality.
		/// </summary>
		/// <param name="o1">The first Turn.</param>
		/// <param name="o2">The second Turn</param>
		/// <returns><c>False</c> if the objects are equal, <c>true</c> otherwise.</returns>
		public static bool operator !=(Turn o1, Turn o2)
		{
			return !object.Equals(o1, o2);
		}

		/// <summary>Compares this instance with another <see cref="Turn"/>.
		/// </summary>
		/// <param name="left">The left <see cref="Turn"/></param>
		/// <param name="right">The right <see cref="Turn"/></param>
		/// <returns><c>true</c> if the the left instance is less than the right instance; <c>false</c> otherwise.</returns>
		public static bool operator < (Turn left, Turn right)
		{
            Robustness.ValidateArgumentNotNull("left", left);
            Robustness.ValidateArgumentNotNull("right", right);
            if (left.year == right.year)
			{
				return left.phase < right.phase;
			}
			else if (left.year < right.year)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

        /// <summary>Compares this instance with another <see cref="Turn"/>.
		/// </summary>
		/// <param name="left">The left <see cref="Turn"/></param>
		/// <param name="right  ">The right <see cref="Turn"/></param>
		/// <returns><c>true</c> if the the left instance is greater than the right instance; <c>false</c> otherwise.</returns>
		public static bool operator > (Turn left, Turn right)
		{
            Robustness.ValidateArgumentNotNull("a", left);
            Robustness.ValidateArgumentNotNull("b", right);
            if (left.year == right.year)
			{
				return left.phase > right.phase;
			}
			else if (left.year > right.year)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>Performs a comparison of two <see cref="Turn"/> objects
		/// and returns a value indicating whether one is less than, equal to or greater than the other.
		/// </summary>
		/// <param name="t1">The first <see cref="Turn"/>.</param>
		/// <param name="t2">The second <see cref="Turn"/>.</param>
		/// <returns>
		/// <para>A signed number indicating the relative values of <paramref>t1</paramref> and <paramref>t2</paramref></para>.
		/// <para>If <paramref>t1</paramref> is less than <paramref>t2</paramref>, the return value will be less than zero.</para>
		/// <para>If <paramref>t1</paramref> is equal to <paramref>t2</paramref>, the return value will be zero.</para>
		/// <para>If <paramref>t1</paramref> is greater than <paramref>t2</paramref>, the return value will greater than zero.</para>
		/// </returns>
		public static int Compare(Turn t1, Turn t2)
		{
			if (t1 == t2)
			{
				return 0;
			}
			if (t1 < t2)
			{
				return -1;
			}
			return 1;
		}
	}
}
