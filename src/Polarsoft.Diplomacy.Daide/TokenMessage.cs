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
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using ErrorMessages = Polarsoft.Diplomacy.Daide.Properties.ErrorMessages;
using System.Collections.ObjectModel;
using Polarsoft.Utilities;

namespace Polarsoft.Diplomacy.Daide
{
	/// <summary>A class that represents a message that is sent to and from the DAIDE server.
	/// </summary>
	public sealed class TokenMessage
    {
        #region Fields

        private List<Token> tokens;
		private List<TokenMessage> subMessages;
		private bool subMessagesParsed;

        #endregion

        #region Construction

        /// <summary>Creates a new empty <see cref="TokenMessage"/> instance.
		/// </summary>
		public TokenMessage()
		{
            this.subMessages = new List<TokenMessage>();
            this.tokens = new List<Token>();
		}

		/// <summary>/// Creates a new <see cref="TokenMessage"/> instance, containing a single <see cref="Token"/>.
		/// </summary>
		/// <param name="token">Token.</param>
		public TokenMessage(Token token)
			:this()
		{
            this.tokens.Add(token);
		}

		/// <summary>Creates a new <see cref="TokenMessage"/> instance.
		/// </summary>
		/// <param name="bytes">Bytes.</param>
		public TokenMessage(byte[] bytes)
			: this(TokenFactory.FromByteArray(bytes))
		{
		}

        internal TokenMessage(IList<Token> tokens)
			:this()
		{
            this.tokens = new List<Token>(tokens);
        }

        #endregion

        #region Implementation

        private void ParseSubmessages()
		{
            if( this.tokens.Count <= 1 )
			{
				//A single token. There are no submessages
				return;
			}
			int index = 0;
            while( index < this.tokens.Count )
			{
                Token token = this.tokens[index];
				if (token == Token.OpenBracket)
				{
					++index;
					int parenthesisCount = 1;
					int start = index;
					//Start of a submessage
                    while( index < this.tokens.Count )
					{
                        if( this.tokens[index] == Token.OpenBracket )
						{
							parenthesisCount++;
						}
                        else if( this.tokens[index] == Token.CloseBracket )
						{
							parenthesisCount--;
							if (parenthesisCount == 0)
							{
								break;
							}
						}
						++index;
					}
                    if( index >= this.tokens.Count )
					{
						throw new ArgumentException(
							string.Format(CultureInfo.InvariantCulture,
							ErrorMessages.TokenMessage_Exception_ParenthesisMismatch));
					}
                    List<Token> subTokens = new List<Token>(this.tokens);
                    subTokens.RemoveRange(index, this.tokens.Count - index);
					subTokens.RemoveRange(0, start);
                    this.subMessages.Add(new TokenMessage(subTokens));
				}
				else	
				{
					//A single token.
                    this.subMessages.Add(new TokenMessage(this.tokens[index]));
				}
				++index;
			}
        }

        #endregion

        #region Representation methods

        /// <summary>Returns the representation of this <see cref="TokenMessage"/> as a <see cref="string"/>
		/// </summary>
		/// <returns>The representation of this <see cref="TokenMessage"/> as a <see cref="string"/></returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
            foreach( Token token in this.tokens )
			{
				sb.Append(token.ToString() + " ");
			}
			return sb.ToString();
		}

		/// <summary>Returns the representation of this <see cref="TokenMessage"/> as an array of <see cref="byte">bytes</see>
		/// </summary>
		/// <returns>The representation of this <see cref="TokenMessage"/> as an array of <see cref="byte">bytes</see></returns>
		public byte[] ToBytes()
		{
            List<byte> bytes = new List<byte>();
            foreach( Token token in this.tokens )
			{
				bytes.AddRange(token.ToBytes());
			}
			return bytes.ToArray();
        }

        #endregion

        #region Comparison methods

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
	
			TokenMessage tokenMessage = obj as TokenMessage;
            if( this.tokens.Count != tokenMessage.tokens.Count )
			{
				return false;
			}
            for( int index = 0; index < this.tokens.Count; ++index )
			{
                if( this.tokens[index] != tokenMessage.tokens[index] )
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>Serves as a hash function for a particular type, suitable for use in hashing algorithms and data structures like a hash table.
		/// </summary>
		/// <returns>A hash code for the current <see cref="TokenMessage"/>.</returns>
		/// <remarks>See <see cref="object.GetHashCode"/> for more information.</remarks>
		public override int GetHashCode()
		{
			int hash = 0;
            foreach( Token token in this.tokens )
			{
				hash = hash ^ token.GetHashCode();
			}
			return hash;
		}

		/// <summary>Compares two TokenMessage objects for equality.
		/// </summary>
		/// <param name="o1">The first TokenMessage.</param>
		/// <param name="o2">The second TokenMessage</param>
		/// <returns><c>True</c> if the objects are equal, <c>false</c> otherwise.</returns>
		public static bool operator ==(TokenMessage o1, TokenMessage o2)
		{
			return object.Equals(o1, o2);
		}

		/// <summary>Compares two TokenMessage objects for inequality.
		/// </summary>
		/// <param name="o1">The first TokenMessage.</param>
		/// <param name="o2">The second TokenMessage</param>
		/// <returns><c>False</c> if the objects are equal, <c>true</c> otherwise.</returns>
		public static bool operator !=(TokenMessage o1, TokenMessage o2)
		{
			return !object.Equals(o1, o2);
        }

        #endregion

        #region Addition methods

        /// <summary>Adds a <see cref="TokenMessage"/> to the end of this <see cref="TokenMessage"/>.
		/// </summary>
        /// <param name="message">The <see cref="TokenMessage"/> containing the tokens to be added to this message.</param>
		public void Add(TokenMessage message)
		{
            Robustness.ValidateArgumentNotNull("message", message);
            this.subMessagesParsed = false;
            this.tokens.AddRange(message.tokens);
		}

		/// <summary>Adds a <see cref="Token"/> to the end of this <see cref="TokenMessage"/>.
		/// </summary>
		/// <param name="token">The <see cref="Token"/> to be added to this message.</param>
		public void Add(Token token)
		{
            Robustness.ValidateArgumentNotNull("token", token);
            this.subMessagesParsed = false;
            this.tokens.Add(token);
		}

		/// <summary>Adds two <see cref="TokenMessage">TokenMessages</see>.
		/// </summary>
		/// <param name="t1">The first <see cref="TokenMessage"/>.</param>
		/// <param name="t2">The first <see cref="TokenMessage"/>.</param>
		/// <returns>A new <see cref="TokenMessage"/> containing the two <see cref="TokenMessage">TokenMessages</see>.</returns>
		public static TokenMessage Add(TokenMessage t1, TokenMessage t2)
		{
            Robustness.ValidateArgumentNotNull("t1", t1);
            Robustness.ValidateArgumentNotNull("t2", t2);
            return t1 + t2;
		}

		/// <summary>Creates a new <see cref="TokenMessage"/> from two <see cref="TokenMessage">TokenMessages</see>.
		/// </summary>
		/// <param name="left">The first <see cref="TokenMessage"/>.</param>
		/// <param name="right">The second <see cref="TokenMessage"/>.</param>
		/// <returns>A new <see cref="TokenMessage"/> containing the two <see cref="TokenMessage">TokenMessages</see>.</returns>
		public static TokenMessage operator +(TokenMessage left, TokenMessage right)
		{
            Robustness.ValidateArgumentNotNull("left", left);
            Robustness.ValidateArgumentNotNull("right", right);
            List<Token> tokens = new List<Token>(left.tokens.Count + right.tokens.Count);
			tokens.AddRange(left.Tokens);
			tokens.AddRange(right.Tokens);
			return new TokenMessage(tokens);
		}

		/// <summary>Creates a new <see cref="TokenMessage"/> from two <see cref="TokenMessage">TokenMessages</see>,
		/// with the second <see cref="TokenMessage"/> enclosed in parentheses.
		/// </summary>
		/// <param name="left">The first <see cref="TokenMessage"/>.</param>
		/// <param name="right">The second <see cref="TokenMessage"/>.</param>
		/// <returns>A <see cref="TokenMessage"/> containing the two <see cref="TokenMessage">TokenMessages</see>.</returns>
		public static TokenMessage operator &(TokenMessage left, TokenMessage right)
		{
            Robustness.ValidateArgumentNotNull("left", left);
            Robustness.ValidateArgumentNotNull("right", right);
            return left + right.MessageEnclosed;
		}

		/// <summary>Creates a new <see cref="TokenMessage"/> from a <see cref="Token"/> and a <see cref="TokenMessage"/>.
		/// </summary>
		/// <param name="left">The <see cref="Token"/>.</param>
		/// <param name="right">The <see cref="TokenMessage"/>.</param>
		/// <returns>A <see cref="TokenMessage"/> containing the <see cref="Token"/> and the <see cref="TokenMessage"/>.</returns>
		public static TokenMessage operator +(Token left, TokenMessage right)
		{
            Robustness.ValidateArgumentNotNull("left", left);
            Robustness.ValidateArgumentNotNull("right", right);
            if (left.IsBracket)
			{
				throw new ArgumentException(
					ErrorMessages.TokenMessage_Exception_CannotAddBrackets,
					"left");
			}
			return (new TokenMessage(left)) + right;
		}

		/// <summary>Creates a new <see cref="TokenMessage"/> from a <see cref="TokenMessage"/> and a <see cref="Token"/>.
		/// </summary>
		/// <param name="left">The <see cref="TokenMessage"/>.</param>
		/// <param name="right">The <see cref="Token"/>.</param>
		/// <returns>A <see cref="TokenMessage"/> containing the <see cref="TokenMessage"/> and the <see cref="Token"/>.</returns>
		public static TokenMessage operator +(TokenMessage left, Token right)
		{
            Robustness.ValidateArgumentNotNull("left", left);
            Robustness.ValidateArgumentNotNull("right", right);
            if (right.IsBracket)
			{
				throw new ArgumentException(
					ErrorMessages.TokenMessage_Exception_CannotAddBrackets,
					"right");
			}
			return (left + new TokenMessage(right));
		}

		/// <summary>Creates a new <see cref="TokenMessage"/> from a <see cref="Token"/> and a <see cref="TokenMessage"/>,
		/// with the <see cref="TokenMessage"/> enclosed in parentheses.
		/// </summary>
		/// <param name="left">The <see cref="Token"/>.</param>
		/// <param name="right">The <see cref="TokenMessage"/>.</param>
		/// <returns>A <see cref="TokenMessage"/> containing the the <see cref="Token"/> and the <see cref="TokenMessage"/>.</returns>
		public static TokenMessage operator &(Token left, TokenMessage right)
		{
            Robustness.ValidateArgumentNotNull("left", left);
            Robustness.ValidateArgumentNotNull("right", right);
            if (left.IsBracket)
			{
				throw new ArgumentException(
					ErrorMessages.TokenMessage_Exception_CannotAddBrackets,
					"left");
			}
			return (new TokenMessage(left)) & right;
		}

		/// <summary>Creates a new <see cref="TokenMessage"/> from a <see cref="TokenMessage"/> and a <see cref="Token"/>,
		/// with the <see cref="Token"/> enclosed in parentheses.
		/// </summary>
		/// <param name="left">The <see cref="TokenMessage"/>.</param>
		/// <param name="right">The <see cref="Token"/>.</param>
		/// <returns>A <see cref="TokenMessage"/> containing the the <see cref="TokenMessage"/> and the <see cref="Token"/>.</returns>
		public static TokenMessage operator &(TokenMessage left, Token right)
		{
            Robustness.ValidateArgumentNotNull("left", left);
            Robustness.ValidateArgumentNotNull("right", right);
            if (right.IsBracket)
			{
				throw new ArgumentException(
					ErrorMessages.TokenMessage_Exception_CannotAddBrackets,
					"right");
			}
			return (left & new TokenMessage(right));
		}

		/// <summary>Creates a new <see cref="TokenMessage"/> containing this message enclosed in parentheses.
		/// </summary>
		/// <value>Creates a new <see cref="TokenMessage"/> containing this message enclosed in parentheses.</value>
		public TokenMessage MessageEnclosed
		{
			get
			{
                List<Token> tokens = new List<Token>(this.tokens.Count + 2);
				tokens.Add(Token.OpenBracket);
                tokens.AddRange(this.tokens);
				tokens.Add(Token.CloseBracket);
				return new TokenMessage(tokens);
			}
        }

        #endregion

        #region Token & Submessage access

        /// <summary>Returns the Submessages of this <see cref="TokenMessage"/>.
		/// </summary>
		/// <remarks>
		/// A submessage is a collection of <see cref="Token">Tokens</see> that are
		/// enclosed in parenthesis, or a single <see cref="Token"/>.
		/// </remarks>
		/// <value>The SubMessages of this <see cref="TokenMessage"/>.</value>
        public ReadOnlyCollection<TokenMessage> SubMessages
		{
			get
			{
                if( !this.subMessagesParsed )
				{
					// Use a lazy evaluation
					// This will most probably cause all incoming
					// messages to be parsed, while all outgoing
					// message will not be parsed.
					ParseSubmessages();
                    this.subMessagesParsed = true;
				}
                return new ReadOnlyCollection<TokenMessage>(this.subMessages);
			}
		}
		
		/// <summary>The tokens that make up this <see cref="TokenMessage"/>.
		/// </summary>
		/// <value>The tokens that make up this <see cref="TokenMessage"/>.</value>
        public ReadOnlyCollection<Token> Tokens
		{
			get
			{
                return new ReadOnlyCollection<Token>(this.tokens);
			}
        }

        #endregion

    }
}
