#region Copyright 2006 Fredrik Blom, Licenced under the MIT Licence
/*
 * Copyright (c) 2006 Fredrik Blom
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
using System.Globalization;
using System.Text;

namespace Polarsoft.Utilities
{
	/// <summary>A utility class that can be used for localization of resources.
	/// </summary>
    public static class Localization
    {
        /// <summary>Gets the current culture.</summary>
        /// <value>The current culture.</value>
        public static CultureInfo CurrentCulture
        {
            get
            {
                return System.Threading.Thread.CurrentThread.CurrentCulture;
            }
        }

        /// <summary>A utility method that formats a string using the current thread's <see cref="System.Globalization.CultureInfo">CurrentCulture</see>.
        /// </summary>
        /// <param name="format">A <see cref="System.String">String</see> containing zero or more format items.</param>
        /// <param name="args">An <see cref="System.Object">Object</see>array containing zero or more objects to format.</param>
        /// <returns>A copy of the format argument in which the format items 
        /// have been replaced by the <b>String</b> equivalent of the 
        /// corresponding instances of <b>Object</b> in args.</returns>
        public static string Format(string format, params Object[] args)
        {
            return string.Format(CurrentCulture, format, args);
        }
    }
}
