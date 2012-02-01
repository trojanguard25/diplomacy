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
	/// <summary>Represents a game of Diplomacy
	/// </summary>
	public class Game
	{
		private Map map;
        private Dictionary<string, Power> powers;
		private Turn turn;

		/// <summary>Creates a new <see cref="Game"/> instance.
		/// </summary>
		public Game()
		{
			this.map = new Map();
            this.powers = new Dictionary<string, Power>();
            this.turn = new Turn(Phase.Spring, 1901);
		}

		/// <summary>Gets the map.
		/// </summary>
		/// <value>The current map the game is played on.</value>
		public Map Map
		{
			get
			{
                return this.map;
			}
		}

		/// <summary>Gets the powers.
		/// </summary>
		/// <value>The powers that are in the game.</value>
		public Dictionary<string, Power> Powers
		{
			get
			{
                return this.powers;
			}
		}

		/// <summary>Gets the current turn.
		/// </summary>
		/// <value>The current turn of the game.</value>
		public Turn Turn
		{
			get
			{
                return this.turn;
			}
		}
	}
}
