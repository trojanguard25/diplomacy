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

namespace Polarsoft.Diplomacy.Daide
{
	/// <summary>
	/// Represents the type of <see cref="Token"/>.
	/// </summary>
	public enum TokenType : byte
	{
		/// <summary>A miscellaneous token.
        /// </summary>
		Misc = 0x40,

		/// <summary>A power token.
        /// </summary>
		Power = 0x41,

		/// <summary>A unit token.
        /// </summary>
		UnitType = 0x42,

		/// <summary>An order token.
        /// </summary>
		Order = 0x43,

		/// <summary>An ordernote.
        /// </summary>
		OrderNote = 0x44,

		/// <summary>An orderresult.
        /// </summary>
		OrderResult = 0x45,

		/// <summary>A coast.
        /// </summary>
		Coast = 0x46,

		/// <summary>A phase.
        /// </summary>
		Phase = 0x47,

		/// <summary>A command token.
        /// </summary>
		Command = 0x48,

		/// <summary>A parameter token.
        /// </summary>
		Parameter = 0x49,

		/// <summary>A press token.
        /// </summary>
		Press = 0x4A,

		/// <summary>A token representing text.
        /// </summary>
		Text = 0x4B,

		/// <summary>An inland province that is not a supply center.
        /// </summary>
		ProvinceInlandNonSupplyCenter = 0x50,

		/// <summary>An inland province that is a supply center.
        /// </summary>
		ProvinceInlandSupplyCenter = 0x51,

		/// <summary>An sea province that is not a supply center.
        /// </summary>
		ProvinceSeaNonSupplyCenter = 0x52,

		/// <summary>An sea province that is a supply center.
        /// </summary>
		ProvinceSeaSupplyCenter = 0x53,

		/// <summary>An coastal province that is not a supply center.
        /// </summary>
		ProvinceCoastalNonSupplyCenter = 0x54,

		/// <summary>An coastal province that is a supply center.
        /// </summary>
		ProvinceCoastalSupplyCenter = 0x55,

		/// <summary>An bicoastal province that is not a supply center.
        /// </summary>
		ProvinceBicoastalNonSupplyCenter = 0x56,

		/// <summary>An bicoastal province that is a supply center.
        /// </summary>
		ProvinceBicoastalSupplyCenter = 0x57,

        #region Local codes (0x58 - 0x5f)

		/// <summary>A number token.
        /// </summary>
		Number = 0x58

        #endregion
    }
}
