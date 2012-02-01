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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Collections.ObjectModel;

namespace Polarsoft.Diplomacy.Daide
{
	/// <summary>A class that helps in constructing tokens.
	/// </summary>
	public static class TokenFactory
    {
        #region Fields

        private static Dictionary<string, ByteArrayEntry> s_mnemonicToBits = new Dictionary<string, ByteArrayEntry>();
		private static Dictionary<ByteArrayEntry, string> s_bitsToMnemonic = new Dictionary<ByteArrayEntry, string>();

        #endregion

        #region Constructor

        static TokenFactory()
        {
            CreateInitialMaps();
        }

        #endregion

        #region Token creation (Factory methods)

        /// <summary>Creates a <see cref="Token"/> from a string mnemonic.
		/// </summary>
		/// <param name="mnemonic">The mnemonic (three letter representation of the <see cref="Token"/>.</param>
		/// <returns>A <see cref="Token"/> that represents this mnemonic.</returns>
		public static Token FromMnemonic(string mnemonic)
		{
			return new Token(mnemonic, s_mnemonicToBits[mnemonic].Bytes);
		}

		/// <summary>Creates a <see cref="Token"/> from a string.
		/// </summary>
		/// <param name="value">The <see cref="string"/> that is to be converted into a <see cref="Token"/>.</param>
		/// <remarks>This function will return a <see cref="Token"/> that has a <see cref="TokenType"/> of <see cref="TokenType.Text"/>.</remarks>
		/// <returns>A <see cref="Token"/> that represents this string.</returns>
		public static Token FromString(string value)
		{
			return new Token(value, ConvertText(value));
		}

        /// <summary>Creates a <see cref="Token"/> from two bytes.
        /// This is only to be used if the Daide Connection doesn't contain
        /// a predefined token for the bytes (in case the Daide protocol
        /// is updated without this framework being updated).
        /// </summary>
        /// <param name="high">The high byte.</param>
        /// <param name="low">The low byte.</param>
        /// <returns>A token that corresponds to the two bytes.</returns>
        public static Token FromBytes(byte high, byte low)
        {
            ByteArrayEntry bytes = new ByteArrayEntry(high, low);
            if( s_bitsToMnemonic.ContainsKey(bytes) )
            {
                return new Token(s_bitsToMnemonic[bytes], bytes.Bytes);
            }
            else
            {
                return new Token(bytes.Bytes);
            }
        }

		/// <summary>Creates a list of <see cref="Token">Tokens</see> from an array of <see cref="byte">bytes</see>.
		/// </summary>
		/// <param name="bytes">Bytes.</param>
        /// <returns>A list of <see cref="Token">Tokens</see> created from the bytes.</returns>
        public static ReadOnlyCollection<Token> FromByteArray(byte[] bytes)
		{
            List<Token> tokens = new List<Token>();
			int index = 0;
			while(index < bytes.Length)
			{
				if (IsNumber(bytes[index]))
				{
					int iVal = GetNumber(bytes, index);
					tokens.Add(new Token(iVal.ToString(CultureInfo.InvariantCulture), bytes, index, 2));
					index += 2;
				}
				else if ((TokenType)bytes[index] == TokenType.Text)
				{
					int startIndex = index;
					ArrayList charBytes = new ArrayList();
					do
					{
						charBytes.Add(bytes[index+1]);
						index += 2;
					}
					while((TokenType)bytes[index] == TokenType.Text);

					Decoder decoder = Encoding.ASCII.GetDecoder();
					byte[] bytesToConvert = (byte[])charBytes.ToArray(typeof(byte));
					
					char[] chars = new char[decoder.GetCharCount(bytesToConvert, 0, bytesToConvert.Length)];
					decoder.GetChars(bytesToConvert, 0, bytesToConvert.Length, chars, 0);

					tokens.Add(new Token(new string(chars), bytes, startIndex, index - startIndex));
				}
				else
				{
                    ByteArrayEntry byteArrayEntry = new ByteArrayEntry(bytes[index], bytes[index+1]);
                    if( s_bitsToMnemonic.ContainsKey(byteArrayEntry) )
                    {
                        tokens.Add(new Token(s_bitsToMnemonic[new ByteArrayEntry(bytes[index], bytes[index + 1])], bytes, index, 2));
                    }
                    else
                    {
                        tokens.Add(new Token(bytes[index], bytes[index + 1]));
                    }
					index += 2;
				}
			}
            return tokens.AsReadOnly();
		}

		/// <summary>Creates a <see cref="Token"/> from an <see cref="int"/>.
		/// </summary>
		/// <param name="number">The <see cref="int"/> that is to be converted into a <see cref="Token"/>.</param>
		/// <remarks>
		/// This function will return a <see cref="Token"/> that has a <see cref="TokenType"/> of <see cref="TokenType.Number"/>.
		/// This function will only accept numbers in the range of -8192 - 8191.
		/// </remarks>
		/// <returns>A <see cref="Token"/> that represents this number.</returns>
		public static Token FromInt(int number)
		{
			if (number > 8191 ||
				number < -8192)
			{
				throw new ArgumentOutOfRangeException("number");
			}
			string representation = number.ToString(CultureInfo.InvariantCulture);
			byte[] bytes = new byte[2];
			bytes[0] = (byte)number;
			number = number >> 8;
			bytes[1] = (byte)number;
			return new Token(representation, bytes);
        }

        #endregion

        #region Internal methods

        internal static bool IsNumber(byte high)
        {
            //If the two topmost bits are zero, we have a number.
            return ( high & 0xC0 ) == 0;
        }

        internal static int GetNumber(byte[] bytes, int index)
        {
            int iVal = bytes[index];
            iVal = iVal << 8;
            iVal += bytes[index + 1];
            return iVal;
        }

        internal static void AddNewMnemonic(string mnemonic, byte high, byte low)
        {
            ByteArrayEntry entry = new ByteArrayEntry(high, low);
            s_mnemonicToBits.Add(mnemonic, entry);
            s_bitsToMnemonic.Add(entry, mnemonic);
        }

        #endregion

        #region Implementation
        
        private static byte[] ConvertText(string text)
		{
			ASCIIEncoding ascii = new ASCIIEncoding();
			byte[] bytes = ascii.GetBytes(text);
			byte[] result = new byte[bytes.Length * 2];
			for(int ii = 0; ii < result.Length; ii += 2)
			{
				result[ii]   = 0x4B;
				result[ii+1] = bytes[ii/2];
			}
			return result;
		}
        
		private static void CreateInitialMaps()
		{
			// Misc (0x40)
			AddNewMnemonic("(", 0x40, 0x00);
			AddNewMnemonic(")", 0x40, 0x01);

            // Powers (0x41)
            // These are added with the RM message.

			// Unit types (0x42)
			AddNewMnemonic("AMY", 0x42, 0x00);
			AddNewMnemonic("FLT", 0x42, 0x01);

			// Orders (0x43)
			AddNewMnemonic("CTO", 0x43, 0x20);
			AddNewMnemonic("CVY", 0x43, 0x21);
			AddNewMnemonic("HLD", 0x43, 0x22);
			AddNewMnemonic("MTO", 0x43, 0x23);
			AddNewMnemonic("SUP", 0x43, 0x24);
			AddNewMnemonic("VIA", 0x43, 0x25);
			AddNewMnemonic("DSB", 0x43, 0x40);
			AddNewMnemonic("RTO", 0x43, 0x41);
			AddNewMnemonic("BLD", 0x43, 0x80);
			AddNewMnemonic("REM", 0x43, 0x81);
			AddNewMnemonic("WVE", 0x43, 0x82);

			// Order notes (0x44)
			AddNewMnemonic("MBV", 0x44, 0x00);
			AddNewMnemonic("BPR", 0x44, 0x01);
			AddNewMnemonic("CST", 0x44, 0x02);
			AddNewMnemonic("ESC", 0x44, 0x03);
			AddNewMnemonic("FAR", 0x44, 0x04);
			AddNewMnemonic("HSC", 0x44, 0x05);
			AddNewMnemonic("NAS", 0x44, 0x06);
			AddNewMnemonic("NMB", 0x44, 0x07);
			AddNewMnemonic("NMR", 0x44, 0x08);
			AddNewMnemonic("NRN", 0x44, 0x09);
			AddNewMnemonic("NRS", 0x44, 0x0A);
			AddNewMnemonic("NSA", 0x44, 0x0B);
			AddNewMnemonic("NSC", 0x44, 0x0C);
			AddNewMnemonic("NSF", 0x44, 0x0D);
			AddNewMnemonic("NSP", 0x44, 0x0E);
			AddNewMnemonic("NST", 0x44, 0x0F);
			AddNewMnemonic("NSU", 0x44, 0x10);
			AddNewMnemonic("NVR", 0x44, 0x11);
			AddNewMnemonic("NYU", 0x44, 0x12);
			AddNewMnemonic("YSC", 0x44, 0x13);

			// Results (0x45)
			AddNewMnemonic("SUC", 0x45, 0x00);
			AddNewMnemonic("BNC", 0x45, 0x01);
			AddNewMnemonic("CUT", 0x45, 0x02);
			AddNewMnemonic("DSR", 0x45, 0x03);
			AddNewMnemonic("FLD", 0x45, 0x04);
			AddNewMnemonic("NSO", 0x45, 0x05);
			AddNewMnemonic("RET", 0x45, 0x06);

			// Coasts (0x46)
			AddNewMnemonic("NCS", 0x46, 0x00);
			AddNewMnemonic("NEC", 0x46, 0x02);
			AddNewMnemonic("ECS", 0x46, 0x04);
			AddNewMnemonic("SEC", 0x46, 0x06);
			AddNewMnemonic("SCS", 0x46, 0x08);
			AddNewMnemonic("SWC", 0x46, 0x0A);
			AddNewMnemonic("WCS", 0x46, 0x0C);
			AddNewMnemonic("NWC", 0x46, 0x0E);

			// Phases (0x47)
			AddNewMnemonic("SPR", 0x47, 0x00);
			AddNewMnemonic("SUM", 0x47, 0x01);
			AddNewMnemonic("FAL", 0x47, 0x02);
			AddNewMnemonic("AUT", 0x47, 0x03);
			AddNewMnemonic("WIN", 0x47, 0x04);

			// Commands (0x48)
			AddNewMnemonic("CCD", 0x48, 0x00);
			AddNewMnemonic("DRW", 0x48, 0x01);
			AddNewMnemonic("FRM", 0x48, 0x02);
			AddNewMnemonic("GOF", 0x48, 0x03);
			AddNewMnemonic("HLO", 0x48, 0x04);
			AddNewMnemonic("HST", 0x48, 0x05);
			AddNewMnemonic("HUH", 0x48, 0x06);
			AddNewMnemonic("IAM", 0x48, 0x07);
			AddNewMnemonic("LOD", 0x48, 0x08);
			AddNewMnemonic("MAP", 0x48, 0x09);
			AddNewMnemonic("MDF", 0x48, 0x0A);
			AddNewMnemonic("MIS", 0x48, 0x0B);
			AddNewMnemonic("NME", 0x48, 0x0C);
			AddNewMnemonic("NOT", 0x48, 0x0D);
			AddNewMnemonic("NOW", 0x48, 0x0E);
			AddNewMnemonic("OBS", 0x48, 0x0F);
			AddNewMnemonic("OFF", 0x48, 0x10);
			AddNewMnemonic("ORD", 0x48, 0x11);
			AddNewMnemonic("OUT", 0x48, 0x12);
			AddNewMnemonic("PRN", 0x48, 0x13);
			AddNewMnemonic("REJ", 0x48, 0x14);
			AddNewMnemonic("SCO", 0x48, 0x15);
			AddNewMnemonic("SLO", 0x48, 0x16);
			AddNewMnemonic("SND", 0x48, 0x17);
			AddNewMnemonic("SUB", 0x48, 0x18);
			AddNewMnemonic("SVE", 0x48, 0x19);
			AddNewMnemonic("THX", 0x48, 0x1A);
			AddNewMnemonic("TME", 0x48, 0x1B);
			AddNewMnemonic("YES", 0x48, 0x1C);
			AddNewMnemonic("ADM", 0x48, 0x1D);
			AddNewMnemonic("SMR", 0x48, 0x1E);

			// Parameters (0x49)
			AddNewMnemonic("AOA", 0x49, 0x00);
			AddNewMnemonic("BTL", 0x49, 0x01);
			AddNewMnemonic("ERR", 0x49, 0x02);
			AddNewMnemonic("LVL", 0x49, 0x03);
			AddNewMnemonic("MRT", 0x49, 0x04);
			AddNewMnemonic("MTL", 0x49, 0x05);
			AddNewMnemonic("NPB", 0x49, 0x06);
			AddNewMnemonic("NPR", 0x49, 0x07);
			AddNewMnemonic("PDA", 0x49, 0x08);
			AddNewMnemonic("PTL", 0x49, 0x09);
			AddNewMnemonic("RTL", 0x49, 0x0A);
			AddNewMnemonic("UNO", 0x49, 0x0B);
			// EPP was 490C now obsolete
			AddNewMnemonic("DSD", 0x49, 0x0D);

			// Press (0x4A)
			AddNewMnemonic("ALY", 0x4A, 0x00);
			AddNewMnemonic("AND", 0x4A, 0x01);
			AddNewMnemonic("BWX", 0x4A, 0x02);
			AddNewMnemonic("DMZ", 0x4A, 0x03);
			AddNewMnemonic("ELS", 0x4A, 0x04);
			AddNewMnemonic("EXP", 0x4A, 0x05);
			AddNewMnemonic("FCT", 0x4A, 0x06);
			AddNewMnemonic("FOR", 0x4A, 0x07);
			AddNewMnemonic("FWD", 0x4A, 0x08);
			AddNewMnemonic("HOW", 0x4A, 0x09);
			AddNewMnemonic("IDK", 0x4A, 0x0A);
			AddNewMnemonic("IFF", 0x4A, 0x0B);
			AddNewMnemonic("INS", 0x4A, 0x0C);
			//AddNewMnemonic("IOU", 0x4A, 0x0D);    // Removed in Rev 14.
			AddNewMnemonic("OCC", 0x4A, 0x0E);
			AddNewMnemonic("ORR", 0x4A, 0x0F);
			AddNewMnemonic("PCE", 0x4A, 0x10);
			AddNewMnemonic("POB", 0x4A, 0x11);
			//AddNewMnemonic("PPT", 0x4A, 0x12);    // Removed in Rev 14.
			AddNewMnemonic("PRP", 0x4A, 0x13);
			AddNewMnemonic("QRY", 0x4A, 0x14);
			AddNewMnemonic("SCD", 0x4A, 0x15);
			AddNewMnemonic("SRY", 0x4A, 0x16);
			AddNewMnemonic("SUG", 0x4A, 0x17);
			AddNewMnemonic("THK", 0x4A, 0x18);
			AddNewMnemonic("THN", 0x4A, 0x19);
			AddNewMnemonic("TRY", 0x4A, 0x1A);
			//AddNewMnemonic("UOM", 0x4A, 0x1B);    // Removed in Rev 14.
			AddNewMnemonic("VSS", 0x4A, 0x1C);
			AddNewMnemonic("WHT", 0x4A, 0x1D);
			AddNewMnemonic("WHY", 0x4A, 0x1E);
			AddNewMnemonic("XDO", 0x4A, 0x1F);
			AddNewMnemonic("XOY", 0x4A, 0x20);
			AddNewMnemonic("YDO", 0x4A, 0x21);
            AddNewMnemonic("CHO", 0x4A, 0x22);
            AddNewMnemonic("BCC", 0x4A, 0x23);
            AddNewMnemonic("UNT", 0x4A, 0x24);
        }

        #endregion

        #region Nested classes

        private class ByteArrayEntry
		{
			byte high;
			byte low;

			public ByteArrayEntry(byte high, byte low)
			{
                this.high = high;
                this.low = low;
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
	
				ByteArrayEntry byteArrayEntry = obj as ByteArrayEntry;
                return ( this.high == byteArrayEntry.high )
                    && ( this.low == byteArrayEntry.low );
			}

			/// <summary>Serves as a hash function for a particular type, suitable for use in hashing algorithms and data structures like a hash table.
			/// </summary>
			/// <returns>A hash code for the current <see cref="ByteArrayEntry"/>.</returns>
			/// <remarks>See <see cref="object.GetHashCode"/> for more information.</remarks>
			public override int GetHashCode()
			{
                return this.high ^ this.low;
			}

			public byte[] Bytes
			{
				get
				{
                    return new byte[] { this.high, this.low };
				}
			}
        }

        #endregion
    }
}
