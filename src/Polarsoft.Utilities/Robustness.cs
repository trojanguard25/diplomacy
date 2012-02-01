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
using System.Text;

namespace Polarsoft.Utilities
{
    /// <summary>Contains utility methods for implementation of robust code.
    /// </summary>
    public static class Robustness
    {
        /// <summary>Throws an <see cref="ArgumentNullException"/> if <paramref name="value"/> is <c>null</c>.
        /// </summary>
        /// <param name="argumentName">The name of the argument to specify in the <see cref="ArgumentNullException"/> that may be thrown.</param>
        /// <param name="value">The value to check for null.</param>
        /// <exception cref="System.ArgumentNullException">Throws ArgumentNullException if value is null.</exception>
        public static void ValidateArgumentNotNull(string argumentName, object value)
        {
            if (value == null)
            {
                // Omit the argument name since it is already included in the message
                throw new ArgumentNullException(null,
                    Localization.Format("Null value not allowed for argument '{0}'.", argumentName));
            }
        }
    }
}
