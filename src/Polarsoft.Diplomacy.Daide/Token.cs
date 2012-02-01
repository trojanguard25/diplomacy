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
using System.Collections.Generic;
using Polarsoft.Utilities;
using ErrorMessages = Polarsoft.Diplomacy.Daide.Properties.ErrorMessages;

namespace Polarsoft.Diplomacy.Daide
{
	/// <summary>Represents a single Token that is sent to and received from the DAIDE server.
	/// </summary>
	public sealed class Token
    {
        #region Fields

        private string representation;
		private byte[] bytes;

        #endregion

        #region Construction

        internal Token(string representation, byte[] bytes)
		{
            this.representation = representation;
            this.bytes = bytes;
		}

        internal Token(string representation, byte[] bytes, int index, int length)
        {
            this.representation = representation;
            this.bytes = new byte[length];
            Array.Copy(bytes, index, this.bytes, 0, length);
        }

		private Token(string representation, byte high, byte low)
		{
            this.representation = representation;
            this.bytes = new byte[2];
            this.bytes[0] = high;
            this.bytes[1] = low;
		}

        internal Token(byte high, byte low)
            : this(Localization.Format("0x{0}{1}",
                high.ToString("X", Localization.CurrentCulture), low.ToString("X", Localization.CurrentCulture)),
            high, low)
        {
        }

        internal Token(byte[] bytes)
            : this(bytes[0], bytes[1])
        {
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
	
			Token token = obj as Token;
            if( !( this.representation == token.representation
                && this.bytes.Length == token.bytes.Length ) )
			{
				return false;
			}
            for( int idx = 0; idx < this.bytes.Length; ++idx )
			{
                if( this.bytes[idx] != token.bytes[idx] )
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>Serves as a hash function for a particular type, suitable for use in hashing algorithms and data structures like a hash table.
		/// </summary>
		/// <returns>A hash code for the current <see cref="Token"/>.</returns>
		/// <remarks>See <see cref="object.GetHashCode"/> for more information.</remarks>
		public override int GetHashCode()
		{
            int hash = this.representation.GetHashCode();
            for( int ii = 0; ii < this.bytes.Length; ++ii )
			{
                hash = hash ^ this.bytes[ii];
			}
			return hash;
		}

		/// <summary>Compares two Token objects for equality.
		/// </summary>
		/// <param name="o1">The first Token.</param>
		/// <param name="o2">The second Token</param>
		/// <returns><c>True</c> if the objects are equal, <c>false</c> otherwise.</returns>
		public static bool operator ==(Token o1, Token o2)
		{
			return object.Equals(o1, o2);
		}

		/// <summary>Compares two Token objects for inequality.
		/// </summary>
		/// <param name="o1">The first Token.</param>
		/// <param name="o2">The second Token</param>
		/// <returns><c>False</c> if the objects are equal, <c>true</c> otherwise.</returns>
		public static bool operator !=(Token o1, Token o2)
		{
			return !object.Equals(o1, o2);
        }

        #endregion

        #region Public Methods

        /// <summary>Returns the representation of this <see cref="Token"/> as a <see cref="string"/>
		/// </summary>
		/// <returns>The representation of this <see cref="Token"/> as a <see cref="string"/></returns>
		public override string ToString()
		{
            return this.representation;
		}

		/// <summary>Returns the representation of this <see cref="Token"/> as an array of <see cref="byte">bytes</see>.
		/// </summary>
		/// <returns>The representation of this <see cref="Token"/> as an array of <see cref="byte">bytes</see>.</returns>
		public byte[] ToBytes()
		{
            return this.bytes;
		}

		/// <summary>Returns the representation of this <see cref="Token"/> as a <see cref="int"/>.
		/// </summary>
		/// <returns>The representation of this <see cref="Token"/> as a <see cref="int"/>.</returns>
		public int ToNumber()
		{
			if (Type != TokenType.Number)
			{
				throw new InvalidOperationException(
					string.Format(CultureInfo.InvariantCulture,
					ErrorMessages.Token_Exception_NotANumber));
			}
            return TokenFactory.GetNumber(this.bytes, 0);
		}

		/// <summary>Gets the type of this <see cref="Token"/>.
		/// </summary>
		/// <value>The <see cref="TokenType"/> of this <see cref="Token"/>.</value>
		public TokenType Type
		{
			get
			{
                if( TokenFactory.IsNumber(this.bytes[0]) )
				{
					return TokenType.Number;
				}
                return (TokenType)this.bytes[0];
			}
		}

		/// <summary>Determines if this token is a bracket token.
		/// </summary>
		/// <returns><c>True</c> if this token is a bracket, <c>false</c> otherwise.</returns>
		/// <remarks>See <see cref="Token.OpenBracket"/> or
		/// <see cref="Token.CloseBracket"/> for more information.</remarks>
		public bool IsBracket
		{
            get
            {
                return this == Token.OpenBracket || this == Token.CloseBracket;
            }
        }

        #endregion

        #region Addition operators

        /// <summary>Creates a new <see cref="TokenMessage"/> from two <see cref="Token">Tokens</see>.
		/// </summary>
		/// <param name="t1">The first <see cref="Token"/>.</param>
		/// <param name="t2">The second <see cref="Token"/>.</param>
		/// <returns>A <see cref="TokenMessage"/> containing the two <see cref="Token">Tokens</see></returns>
        public static TokenMessage Add(Token t1, Token t2)
		{
			return t1 + t2;
		}

		/// <summary>Creates a new <see cref="TokenMessage"/> from two <see cref="Token">Tokens</see>.
		/// </summary>
		/// <param name="left">The first <see cref="Token"/>.</param>
		/// <param name="right">The first <see cref="Token"/>.</param>
		/// <returns>A <see cref="TokenMessage"/> containing the two <see cref="Token">Tokens</see></returns>
        public static TokenMessage operator +(Token left, Token right)
		{
			if (left.IsBracket || right.IsBracket)
			{
				throw new ArgumentException(
					string.Format(CultureInfo.InvariantCulture,
					ErrorMessages.Token_Exception_CannotAddBrackets));
			}
            List<Token> tokens = new List<Token>(2);
			tokens.Add(left);
			tokens.Add(right);
			return new TokenMessage(tokens);
		}

		/// <summary>Creates a new <see cref="TokenMessage"/> from two <see cref="Token">Tokens</see>,
		/// with the second <see cref="Token"/> enclosed in parentheses.
		/// </summary>
		/// <param name="left">The first <see cref="Token"/>.</param>
		/// <param name="right">The first <see cref="Token"/>.</param>
		/// <returns>A <see cref="TokenMessage"/> containing the two <see cref="Token">Tokens</see></returns>
        public static TokenMessage operator &(Token left, Token right)
		{
			if (left.IsBracket || right.IsBracket)
			{
				throw new ArgumentException(
					string.Format(CultureInfo.InvariantCulture,
					ErrorMessages.Token_Exception_CannotAddBrackets));
			}
			return new TokenMessage(left) & right;
        }

        #endregion

        #region Predefined tokens - Misc category (0x40)

        /// <summary>A <see cref="Token"/> representing the open bracket '('.
		/// </summary>
        public readonly static Token OpenBracket = new Token("(", 0x40, 0x00);

		/// <summary>A <see cref="Token"/> representing the close bracket ')'.
		/// </summary>
        public readonly static Token CloseBracket = new Token(")", 0x40, 0x01);

        #endregion

        #region Predefined tokens - Unit types category (0x42)

		/// <summary>A <see cref="Token"/> representing an army (AMY).
		/// </summary>
        public readonly static Token ARMY = new Token("AMY", 0x42, 0x00);

		/// <summary>A <see cref="Token"/> representing a fleet (FLT).
		/// </summary>
        public readonly static Token FLEET = new Token("FLT", 0x42, 0x01);

        #endregion

        #region Predefined tokens - Orders category (0x43)
        
		/// <summary>A <see cref="Token"/> representing the order 'Convey to'.
		/// </summary>
		public readonly static Token CTO = new Token("CTO", 0x43, 0x20);

		/// <summary>A <see cref="Token"/> representing the order 'Convoy'.
		/// </summary>
        public readonly static Token CVY = new Token("CVY", 0x43, 0x21);

		/// <summary>A <see cref="Token"/> representing the order 'Hold'.
		/// </summary>
        public readonly static Token HLD = new Token("HLD", 0x43, 0x22);

		/// <summary>A <see cref="Token"/> representing the order 'Move To'.
		/// </summary>
        public readonly static Token MTO = new Token("MTO", 0x43, 0x23);

		/// <summary>A <see cref="Token"/> representing the order 'Support'.
		/// </summary>
        public readonly static Token SUP = new Token("SUP", 0x43, 0x24);

		/// <summary>A <see cref="Token"/> representing 'Via' (used in orders).
		/// </summary>
        public readonly static Token VIA = new Token("VIA", 0x43, 0x25);

		/// <summary>A <see cref="Token"/> representing the order 'Disband'.
		/// </summary>
        public readonly static Token DSB = new Token("DSB", 0x43, 0x40);

		/// <summary>A <see cref="Token"/> representing the order 'Retreat to'.
		/// </summary>
        public readonly static Token RTO = new Token("RTO", 0x43, 0x41);

		/// <summary>A <see cref="Token"/> representing the order 'Build'.
		/// </summary>
        public readonly static Token BLD = new Token("BLD", 0x43, 0x80);

		/// <summary>A <see cref="Token"/> representing the order 'Remove'.
		/// </summary>
		public readonly static Token REM = new Token("REM", 0x43, 0x81);

		/// <summary>A <see cref="Token"/> representing the order 'Waive build'.
		/// </summary>
		public readonly static Token WVE = new Token("WVE", 0x43, 0x82);

        #endregion

        #region Predefined tokens - Order Notes category (0x44)

		/// <summary>A <see cref="Token"/> representing the order note 'OK'.
		/// </summary>
        public readonly static Token MBV = new Token("MBV", 0x44, 0x00);

		//public static Token BPR = new Token("BPR", 0x44, 0x01);	//REMOVED
		/// <summary>A <see cref="Token"/> representing the order note 'No coast specified for fleet build in Bicoastal province (<see cref="TokenType"/>, or an attempt to build a fleet inland, or an army at sea'.
		/// </summary>
        public readonly static Token CST = new Token("CST", 0x44, 0x02);

		/// <summary>A <see cref="Token"/> representing the order note 'Not an empty supply center'.
		/// </summary>
        public readonly static Token ESC = new Token("ESC", 0x44, 0x03);

		/// <summary>A <see cref="Token"/> representing the order note 'Not adjacent'.
		/// </summary>
        public readonly static Token FAR = new Token("FAR", 0x44, 0x04);

		/// <summary>A <see cref="Token"/> representing the order note 'Not a home supply center'.
		/// </summary>
        public readonly static Token HSC = new Token("HSC", 0x44, 0x05);

		/// <summary>A <see cref="Token"/> representing the order note 'Not at sea (for a convoying fleet)'.
		/// </summary>
        public readonly static Token NAS = new Token("NAS", 0x44, 0x06);

		/// <summary>A <see cref="Token"/> representing the order note 'No more builds allowed'.
		/// </summary>
        public readonly static Token NMB = new Token("NMB", 0x44, 0x07);

		/// <summary>A <see cref="Token"/> representing the order note 'No more removals allowed'.
		/// </summary>
        public readonly static Token NMR = new Token("NMR", 0x44, 0x08);

		/// <summary>A <see cref="Token"/> representing the order note 'No retreat needed for this unit'.
		/// </summary>
        public readonly static Token NRN = new Token("NRN", 0x44, 0x09);

		/// <summary>A <see cref="Token"/> representing the order note 'Not the right season'.
		/// </summary>
        public readonly static Token NRS = new Token("NRS", 0x44, 0x0A);

		/// <summary>A <see cref="Token"/> representing the order note 'No such army (for unit being ordered to <see cref="Token.CTO"/> or for unit being <see cref="Token.CVY"/>'.
		/// </summary>
        public readonly static Token NSA = new Token("NSA", 0x44, 0x0B);

		/// <summary>A <see cref="Token"/> representing the order note 'Not a supply center'.
		/// </summary>
        public readonly static Token NSC = new Token("NSC", 0x44, 0x0C);

		/// <summary>A <see cref="Token"/> representing the order note 'No such fleet (in the <see cref="Token.VIA"/> section of <see cref="Token.CTO"/> or the unit performing a <see cref="Token.CVY"/>'.
		/// </summary>
        public readonly static Token NSF = new Token("NSF", 0x44, 0x0D);

		/// <summary>A <see cref="Token"/> representing the order note 'No such province'.
		/// </summary>
        public readonly static Token NSP = new Token("NSP", 0x44, 0x0E);
        
        /// <summary>A <see cref="Token"/> representing the order note 'No such unit'.
		/// </summary>
        public readonly static Token NSU = new Token("NSU", 0x44, 0x10);

		/// <summary>A <see cref="Token"/> representing the order note 'Not a valid retreat space'.
		/// </summary>
        public readonly static Token NVR = new Token("NVR", 0x44, 0x11);

		/// <summary>A <see cref="Token"/> representing the order note 'Not your unit'.
		/// </summary>
        public readonly static Token NYU = new Token("NYU", 0x44, 0x12);

		/// <summary>A <see cref="Token"/> representing the order note 'Not your supply center'.
		/// </summary>
        public readonly static Token YSC = new Token("YSC", 0x44, 0x13);

        #endregion

        #region Predefined tokens - Results category (0x45)

		/// <summary>A <see cref="Token"/> representing the order result 'Success'.
		/// </summary>
        public readonly static Token SUC = new Token("SUC", 0x45, 0x00);

		/// <summary>A <see cref="Token"/> representing the order result 'Move bounced'.
		/// </summary>
        public readonly static Token BNC = new Token("BNC", 0x45, 0x01);

		/// <summary>A <see cref="Token"/> representing the order result 'Support cut'.
		/// </summary>
        public readonly static Token CUT = new Token("CUT", 0x45, 0x02);

		/// <summary>A <see cref="Token"/> representing the order result 'Move via convoy failed due to dislodged convoying fleet'.
		/// </summary>
        public readonly static Token DSR = new Token("DSR", 0x45, 0x03);

		//public static Token FLD = new Token("FLD", 0x45, 0x04);	//REMOVED
		/// <summary>A <see cref="Token"/> representing the order result 'No such order (for a support, convoying fleet, or convoyed army)'.
		/// </summary>
        public readonly static Token NSO = new Token("NSO", 0x45, 0x05);

		/// <summary>A <see cref="Token"/> indicating that the unit was disloged and must retreat.
		/// </summary>
        public readonly static Token RET = new Token("RET", 0x45, 0x06);

        #endregion

        #region Predefined tokens - Coasts category (0x46)

		/// <summary>A <see cref="Token"/> representing the coast 'North coast'.
		/// </summary>
        public readonly static Token NCS = new Token("NCS", 0x46, 0x00);

		/// <summary>A <see cref="Token"/> representing the coast 'North-East coast'.
		/// </summary>
        public readonly static Token NEC = new Token("NEC", 0x46, 0x02);

		/// <summary>A <see cref="Token"/> representing the coast 'East coast'.
		/// </summary>
        public readonly static Token ECS = new Token("ECS", 0x46, 0x04);

		/// <summary>A <see cref="Token"/> representing the coast 'South-East coast'.
		/// </summary>
        public readonly static Token SEC = new Token("SEC", 0x46, 0x06);

		/// <summary>A <see cref="Token"/> representing the coast 'South coast'.
		/// </summary>
        public readonly static Token SCS = new Token("SCS", 0x46, 0x08);

		/// <summary>A <see cref="Token"/> representing the coast 'South-West coast'.
		/// </summary>
        public readonly static Token SWS = new Token("SWC", 0x46, 0x0A);

		/// <summary>A <see cref="Token"/> representing the coast 'West coast'.
		/// </summary>
        public readonly static Token WCS = new Token("WCS", 0x46, 0x0C);

		/// <summary>A <see cref="Token"/> representing the coast 'North-West coast'.
		/// </summary>
        public readonly static Token NWC = new Token("NWC", 0x46, 0x0E);

        #endregion

        #region Predefined tokens - Phases category (0x47)

		/// <summary>A <see cref="Token"/> representing the phase 'Spring (Movement phase).
		/// </summary>
        public readonly static Token SPR = new Token("SPR", 0x47, 0x00);

		/// <summary>A <see cref="Token"/> representing the phase 'Summer (Retreat phase).
		/// </summary>
        public readonly static Token SUM = new Token("SUM", 0x47, 0x01);

		/// <summary>A <see cref="Token"/> representing the phase 'Fall (Movement phase).
		/// </summary>
        public readonly static Token FAL = new Token("FAL", 0x47, 0x02);

		/// <summary>A <see cref="Token"/> representing the phase 'Autumn (Retreat phase).
		/// </summary>
        public readonly static Token AUT = new Token("AUT", 0x47, 0x03);

		/// <summary>A <see cref="Token"/> representing the phase 'Winter (Build phase).
		/// </summary>
        public readonly static Token WIN = new Token("WIN", 0x47, 0x04);

        #endregion

        #region Predefined tokens - Commands category (0x48)

		/// <summary>A <see cref="Token"/> representing the command 'Country in civil disorder'.
		/// </summary>
        public readonly static Token CCD = new Token("CCD", 0x48, 0x00);

		/// <summary>A <see cref="Token"/> representing the command 'Draw'.
		/// </summary>
        public readonly static Token DRW = new Token("DRW", 0x48, 0x01);

		/// <summary>A <see cref="Token"/> representing the command 'Message from'.
		/// </summary>
        public readonly static Token FRM = new Token("FRM", 0x48, 0x02);

		/// <summary>A <see cref="Token"/> representing the command 'Go flag'.
		/// </summary>
        public readonly static Token GOF = new Token("GOF", 0x48, 0x03);

		/// <summary>A <see cref="Token"/> representing the command 'Hello (start of game)'.
		/// </summary>
        public readonly static Token HLO = new Token("HLO", 0x48, 0x04);

		/// <summary>A <see cref="Token"/> representing the command 'History'.
		/// </summary>
        public readonly static Token HST = new Token("HST", 0x48, 0x05);

		/// <summary>A <see cref="Token"/> representing the command 'Syntax error / not understood'.
		/// </summary>
        public readonly static Token HUH = new Token("HUH", 0x48, 0x06);

		/// <summary>A <see cref="Token"/> representing the command 'I am'.
		/// </summary>
        public readonly static Token IAM = new Token("IAM", 0x48, 0x07);

		/// <summary>A <see cref="Token"/> representing the command 'Load game'.
		/// </summary>
        public readonly static Token LOD = new Token("LOD", 0x48, 0x08);

		/// <summary>A <see cref="Token"/> representing the command 'Map to be used for this game'.
		/// </summary>
        public readonly static Token MAP = new Token("MAP", 0x48, 0x09);

		/// <summary>A <see cref="Token"/> representing the command 'Map definition'.
		/// </summary>
        public readonly static Token MDF = new Token("MDF", 0x48, 0x0A);

		/// <summary>A <see cref="Token"/> representing the command 'Missing orders'.
		/// </summary>
        public readonly static Token MIS = new Token("MIS", 0x48, 0x0B);

		/// <summary>A <see cref="Token"/> representing the command 'Name'.
		/// </summary>
        public readonly static Token NME = new Token("NME", 0x48, 0x0C);

		/// <summary>A <see cref="Token"/> representing the command 'Logical not'.
		/// </summary>
        public readonly static Token NOT = new Token("NOT", 0x48, 0x0D);

		/// <summary>A <see cref="Token"/> representing the command 'Current position'.
		/// </summary>
        public readonly static Token NOW = new Token("NOW", 0x48, 0x0E);


		/// <summary>A <see cref="Token"/> representing the command 'Observer'.
		/// </summary>
        public readonly static Token OBS = new Token("OBS", 0x48, 0x0F);

		/// <summary>A <see cref="Token"/> representing the command 'Turn off (Exit)'.
		/// </summary>
        public readonly static Token OFF = new Token("OFF", 0x48, 0x10);

		/// <summary>A <see cref="Token"/> representing the command 'Order results'.
		/// </summary>
        public readonly static Token ORD = new Token("ORD", 0x48, 0x11);

		/// <summary>A <see cref="Token"/> representing the command 'Country is out of the game'.
		/// </summary>
        public readonly static Token OUT = new Token("OUT", 0x48, 0x12);

		/// <summary>A <see cref="Token"/> representing the command 'Parenthesis error'.
		/// </summary>
        public readonly static Token PRN = new Token("PRN", 0x48, 0x13);

		/// <summary>A <see cref="Token"/> representing the command 'Reject'.
		/// </summary>
        public readonly static Token REJ = new Token("REJ", 0x48, 0x14);

		/// <summary>A <see cref="Token"/> representing the command 'Supply center ownership'.
		/// </summary>
        public readonly static Token SCO = new Token("SCO", 0x48, 0x15);

		/// <summary>A <see cref="Token"/> representing the command 'Solo'.
		/// </summary>
        public readonly static Token SLO = new Token("SLO", 0x48, 0x16);

		/// <summary>A <see cref="Token"/> representing the command 'Send message'.
		/// </summary>
        public readonly static Token SND = new Token("SND", 0x48, 0x17);

		/// <summary>A <see cref="Token"/> representing the command 'Submit order'.
		/// </summary>
        public readonly static Token SUB = new Token("SUB", 0x48, 0x18);

		/// <summary>A <see cref="Token"/> representing the command 'Save'.
		/// </summary>
        public readonly static Token SVE = new Token("SVE", 0x48, 0x19);

		/// <summary>A <see cref="Token"/> representing the command 'Thanks for the order'.
		/// </summary>
        public readonly static Token THX = new Token("THX", 0x48, 0x1A);

		/// <summary>A <see cref="Token"/> representing the command 'Time to deadline'.
		/// </summary>
        public readonly static Token TME = new Token("TME", 0x48, 0x1B);

		/// <summary>A <see cref="Token"/> representing the command 'Accept'.
		/// </summary>
        public readonly static Token YES = new Token("YES", 0x48, 0x1C);

		/// <summary>A <see cref="Token"/> representing the command 'Administrative message'.
		/// </summary>
        public readonly static Token ADM = new Token("ADM", 0x48, 0x1D);

		/// <summary>A <see cref="Token"/> representing the command 'Participants in the game'.
		/// </summary>
        public readonly static Token SMR = new Token("SMR", 0x48, 0x1E);

        #endregion

        #region Predefined tokens - Parameters category (0x49)

        /// <summary>A <see cref="Token"/> representing the <see cref="Token.HLO"/> parameter 'Any orders allowed'.
		/// </summary>
        public readonly static Token AOA = new Token("AOA", 0x49, 0x00);

		/// <summary>A <see cref="Token"/> representing the <see cref="Token.HLO"/> parameter 'Build time limit'.
		/// </summary>
        public readonly static Token BTL = new Token("BTL", 0x49, 0x01);

		/// <summary>A <see cref="Token"/> representing the <see cref="Token.HUH"/> information 'Error location'.
		/// </summary>
        public readonly static Token ERR = new Token("ERR", 0x49, 0x02);

		/// <summary>A <see cref="Token"/> representing the <see cref="Token.HLO"/> parameter 'Language level'.
		/// </summary>
        public readonly static Token LVL = new Token("LVL", 0x49, 0x03);

		/// <summary>A <see cref="Token"/> representing the <see cref="Token.NOW"/> parameter 'Must retreat to'.
		/// </summary>
        public readonly static Token MRT = new Token("MRT", 0x49, 0x04);

		/// <summary>A <see cref="Token"/> representing the <see cref="Token.HLO"/> parameter 'Movement time limit'.
		/// </summary>
        public readonly static Token MTL = new Token("MTL", 0x49, 0x05);

		/// <summary>A <see cref="Token"/> representing the <see cref="Token.HLO"/> parameter 'No press during builds'.
		/// </summary>
        public readonly static Token NPB = new Token("NPB", 0x49, 0x06);

		/// <summary>A <see cref="Token"/> representing the <see cref="Token.HLO"/> parameter 'No press during retreats'.
		/// </summary>
        public readonly static Token NPR = new Token("NPR", 0x49, 0x07);

		/// <summary>A <see cref="Token"/> representing the <see cref="Token.HLO"/> parameter 'Partial draws allowed'.
		/// </summary>
        public readonly static Token PDA = new Token("PDA", 0x49, 0x08);

		/// <summary>A <see cref="Token"/> representing the <see cref="Token.HLO"/> parameter 'Press time limit'.
		/// </summary>
        public readonly static Token PTL = new Token("PTL", 0x49, 0x09);

		/// <summary>A <see cref="Token"/> representing the <see cref="Token.HLO"/> parameter 'Retreat time limit'.
		/// </summary>
        public readonly static Token RTL = new Token("RTL", 0x49, 0x0A);

		/// <summary>A <see cref="Token"/> representing the <see cref="Token.SCO"/> parameter 'Unowned'.
		/// </summary>
        public readonly static Token UNO = new Token("UNO", 0x49, 0x0B);

        /// <summary>A <see cref="Token"/> representing the <see cref="Token.HLO"/> parameter 'Deadline stops on disconnection'.
		/// </summary>
        public readonly static Token DSD = new Token("DSD", 0x49, 0x0D);

        #endregion

        #region Predefined tokens - Press category (0x4A)

        /// <summary>A press <see cref="Token"/> representing 'An offer of an alliance'.
		/// </summary>
        public readonly static Token ALY = new Token("ALY", 0x4A, 0x00);

		/// <summary>A press <see cref="Token"/> representing 'Logical AND'.
		/// </summary>
        public readonly static Token AND = new Token("AND", 0x4A, 0x01);

		/// <summary>A press <see cref="Token"/> representing 'None of your business'.
		/// </summary>
        public readonly static Token BWX = new Token("BWX", 0x4A, 0x02);

		/// <summary>A press <see cref="Token"/> representing 'Demilitarised zone'.
		/// </summary>
        public readonly static Token DMZ = new Token("DMZ", 0x4A, 0x03);

		/// <summary>A press <see cref="Token"/> representing 'Else'.
		/// </summary>
        public readonly static Token ELS = new Token("ELS", 0x4A, 0x04);

		/// <summary>A press <see cref="Token"/> representing 'Explain'.
		/// </summary>
        public readonly static Token EXP = new Token("EXP", 0x4A, 0x05);

		/// <summary>A press <see cref="Token"/> representing 'Forward press'.
		/// </summary>
        public readonly static Token FWD = new Token("FWD", 0x4A, 0x06);

		/// <summary>A press <see cref="Token"/> representing 'Fact'.
		/// </summary>
        public readonly static Token FCT = new Token("FCT", 0x4A, 0x07);

		/// <summary>A press <see cref="Token"/> representing 'For specified turn'.
		/// </summary>
        public readonly static Token FOR = new Token("FOR", 0x4A, 0x08);

		/// <summary>A press <see cref="Token"/> representing 'How to attack'.
		/// </summary>
        public readonly static Token HOW = new Token("HOW", 0x4A, 0x09);

		/// <summary>A press <see cref="Token"/> representing 'I don't know'.
		/// </summary>
        public readonly static Token IDK = new Token("IDK", 0x4A, 0x0A);

		/// <summary>A press <see cref="Token"/> representing 'If'.
		/// </summary>
        public readonly static Token IFF = new Token("IFF", 0x4A, 0x0B);

		/// <summary>A press <see cref="Token"/> representing 'Insist'.
		/// </summary>
        public readonly static Token INS = new Token("INS", 0x4A, 0x0C);

		/// <summary>A press <see cref="Token"/> representing 'Occupy'.
		/// </summary> 
        public readonly static Token OCC = new Token("OCC", 0x4A, 0x0E);

        /// <summary>A press <see cref="Token"/> representing 'Logical OR'.
		/// </summary>
        public readonly static Token ORR = new Token("ORR", 0x4A, 0x0F);

        /// <summary>A press <see cref="Token"/> representing 'Peace'.
		/// </summary>
        public readonly static Token PCE = new Token("PCE", 0x4A, 0x10);

        /// <summary>A press <see cref="Token"/> representing 'Position on board'.
		/// </summary>
        public readonly static Token POB = new Token("POB", 0x4A, 0x11);

        /// <summary>A press <see cref="Token"/> representing 'Propose'.
		/// </summary>
        public readonly static Token PRP = new Token("PRP", 0x4A, 0x13);

		/// <summary>A press <see cref="Token"/> representing 'Query'.
		/// </summary>
        public readonly static Token QRY = new Token("QRY", 0x4A, 0x14);

		/// <summary>A press <see cref="Token"/> representing 'Supply center distribution'.
		/// </summary>
        public readonly static Token SCD = new Token("SCD", 0x4A, 0x15);

		/// <summary>A press <see cref="Token"/> representing 'Sorry'.
		/// </summary>
        public readonly static Token SRY = new Token("SRY", 0x4A, 0x16);

		/// <summary>A press <see cref="Token"/> representing 'Suggest'.
		/// </summary>
        public readonly static Token SUG = new Token("SUG", 0x4A, 0x17);

		/// <summary>A press <see cref="Token"/> representing 'Think'.
		/// </summary>
        public readonly static Token THK = new Token("THK", 0x4A, 0x18);

		/// <summary>A press <see cref="Token"/> representing 'Then'.
		/// </summary>
        public readonly static Token THN = new Token("THN", 0x4A, 0x19);

		/// <summary>A press <see cref="Token"/> representing 'Try the following tokens'.
		/// </summary>
        public readonly static Token TRY = new Token("TRY", 0x4A, 0x1A);

		/// <summary>A press <see cref="Token"/> representing 'Versus'.
		/// </summary>
        public readonly static Token VSS = new Token("VSS", 0x4A, 0x1C);

		/// <summary>A press <see cref="Token"/> representing 'What to do with'.
		/// </summary>
        public readonly static Token WHT = new Token("WHT", 0x4A, 0x1D);

		/// <summary>A press <see cref="Token"/> representing 'Why'.
		/// </summary>
        public readonly static Token WHY = new Token("WHY", 0x4A, 0x1E);

		/// <summary>A press <see cref="Token"/> representing 'Moves to do'.
		/// </summary>
        public readonly static Token XDO = new Token("XDO", 0x4A, 0x1F);

		/// <summary>A press <see cref="Token"/> representing 'X owes Y'.
		/// </summary>
        public readonly static Token XOY = new Token("XOY", 0x4A, 0x20);

		/// <summary>A press <see cref="Token"/> representing 'You provide the orders for these units'.
		/// </summary>
        public readonly static Token YDO = new Token("YDO", 0x4A, 0x21);

		/// <summary>A press <see cref="Token"/> representing 'Choose out of the following...'.
		/// </summary>
        public readonly static Token CHO = new Token("CHO", 0x4A, 0x22);

        /// <summary>A press <see cref="Token"/> representing 'Forward all press sent...'.
        /// </summary>
        public readonly static Token BCC = new Token("BCC", 0x4A, 0x23);

        /// <summary>A press <see cref="Token"/> representing 'Any unit'.
        /// </summary>
        public readonly static Token UNT = new Token("UNT", 0x4A, 0x24);

        #endregion
    }
}
