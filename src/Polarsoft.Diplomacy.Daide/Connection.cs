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
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using ErrorMessages = Polarsoft.Diplomacy.Daide.Properties.ErrorMessages;

namespace Polarsoft.Diplomacy.Daide
{
	/// <summary>Responsible for establishing and keeping a connection to a server.
	/// </summary>
	public class Connection
    {
        #region Instrumentation

        const string traceCategory = "DAIDE::Connector";
		static BooleanSwitch connectorSwitch =
			new BooleanSwitch(traceCategory, "Trace messages from the DAIDE Connector");

        #endregion

        #region Fields

        private NetworkStream stream;
		private Game game;

        #endregion

        #region Events

        /// <summary>Occurs when a new Diplomatic Message has been received.
		/// </summary>
		public event EventHandler<DiplomaticMessageEventArgs> DiplomaticMessageReceived;

        #endregion

        #region Construction

        /// <summary>Creates a new <see cref="Connection"/> instance.
		/// </summary>
		/// <param name="game">The <see cref="Game"/> instance.</param>
		public Connection(Game game)
		{
            this.game = game;
        }

        #endregion

        #region Public methods

        /// <summary>Connects this instance to a DAIDE server.
		/// </summary>
        /// <param name="hostName">HostName.</param>
		/// <param name="port">Port.</param>
		public void Connect(string hostName, int port)
		{
			TcpClient socket;
			try
			{
                socket = new TcpClient(hostName, port);
			}
			catch (SocketException)
			{
				throw;
			}
            this.stream = socket.GetStream();
            StartStreamListening(this.stream);

			//Send the Initial Message
			Send(createInitialMessage());
        }

        /// <summary>Sends a message to the server.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void Send(TokenMessage message)
        {
            if( message == null )
            {
                throw new ArgumentNullException("message");
            }

            Trace.WriteLineIf(connectorSwitch.Enabled,
                string.Format(CultureInfo.InvariantCulture,
                ErrorMessages.Connection_Trace_SendMessage,
                message.ToString()), traceCategory);
            Send(createDiplomaticMessage(message));
        }

        /// <summary>Disconnects this instance from the DAIDE server.
        /// </summary>
        public void Disconnect()
        {
            Send(createFinalMessage());
        }

        #endregion

        #region Implementation

        private void StartStreamListening(NetworkStream stream)
		{
			byte[] myReadBuffer = new byte[4];
			StreamReaderState state = new StreamReaderState(myReadBuffer, stream);
			stream.BeginRead(myReadBuffer, 0, myReadBuffer.Length,
				new System.AsyncCallback(this.MessageHeaderReader), state);
		}

		private void MessageHeaderReader(IAsyncResult result)
		{
			StreamReaderState state = (StreamReaderState)result.AsyncState;
			byte[] msgHeader = state.Buffer;
			int length = msgHeader[2] & 0xFF;
			length = length << 8;
			length += msgHeader[3] & 0xFF;
			byte[] msg = new byte[length];
			StreamReaderState stateWithMsgType =
				new StreamReaderState(msg, state.Stream, msgHeader[0]);
			state.Stream.BeginRead(msg, 0, msg.Length,
				new AsyncCallback(this.MessageReader), stateWithMsgType);
		}

		private void MessageReader(IAsyncResult result)
		{
			StreamReaderState state = (StreamReaderState)result.AsyncState;
			HandleMessage(state.MsgType, state.Buffer);
			StartStreamListening(state.Stream);
		}



		private void Send(byte[] message)
		{
			try
			{
                this.stream.Write(message, 0, message.Length);
			}
			catch (System.IO.IOException)
			{
			}
		}

		private static byte[] createInitialMessage()
		{
			return new byte[]
			{
				(byte)MessageType.IM,	// Msg type
				(byte)0x00,				// pad
				(byte)0x00, (byte)0x04,	// Remaining length
				(byte)0x00, (byte)0x01,	// Version
				(byte)0xDA, (byte)0x10	// Magic number
			};
		}

		private static byte[] createDiplomaticMessage(TokenMessage message)
		{
			byte[] data = message.ToBytes();
			byte[] bytes = new byte[data.Length + 4];
			bytes[0] = (byte)MessageType.DM;			// Msg type
			bytes[1] = 0x00;							// pad
			bytes[2] = (byte)(data.Length >> 8);		// Remaining length
			bytes[3] = (byte)(data.Length & 0xFF);		// Remaining length
			Array.Copy(data, 0, bytes, 4, data.Length);	// Language Message
			return bytes;
		}

		private static byte[] createFinalMessage()
		{
			return new byte[]
			{
				(byte)MessageType.FM,	// Msg type
				(byte)0x00,				// pad
				(byte)0x00, (byte)0x00,	// Remaining length
			};
		}

		private void HandleMessage(byte type, byte[] message)
		{
			MessageType mt = (MessageType)type;
			switch (mt)
			{
				case MessageType.RM :
					HandleRepresentationMessage(message);
					break;
				case MessageType.DM :
					HandleDiplomaticMessage(message);
					break;
				case MessageType.FM :
					HandleFinalMessage(message);
					break;
				case MessageType.EM :
					HandleErrorMessage(message);
					break;
			}
		}

		private void HandleRepresentationMessage(byte[] message)
		{
			//This message, if it has a length, will contain
			//new powers and provinces. All the default
			//powers and provinces are not valid if this
			//message has a length of > 0!
			if (message.Length > 0)
			{
				Trace.WriteLineIf(connectorSwitch.Enabled,
					string.Format(CultureInfo.InvariantCulture,
					ErrorMessages.Connection_Trace_PowersFromRM), traceCategory);

				Trace.Indent();
				for(int ii = 0; ii < message.Length; ii+=6)
				{
					//The first two bytes are the message identifier
					byte[] token = new byte[] {	message[ii+2],
												message[ii+3],
												message[ii+4]
											  };
					//Convert the token array to a string
					char[] tokens = new char[token.Length];
					System.Text.Decoder decoder = Encoding.ASCII.GetDecoder();
					decoder.GetChars(token, 0, token.Length, tokens, 0);

					string mnemonic = new string(tokens);
					AddNewMnemonic(mnemonic, message[ii], message[ii+1]);
				}
				Trace.Unindent();
			}
			else
			{
				//Add all the default powers and the default
				//provinces.
				Trace.WriteLineIf(connectorSwitch.Enabled,
					string.Format(CultureInfo.InvariantCulture,
					ErrorMessages.Connection_Trace_DefaultPowers), traceCategory);
				Trace.Indent();

				Trace.WriteLineIf(connectorSwitch.Enabled,
					string.Format(CultureInfo.InvariantCulture,
					ErrorMessages.Connection_Trace_Powers
					), traceCategory);
				Trace.Indent();
				AddNewMnemonic("AUS", 0x41, 0x00); 
				AddNewMnemonic("ENG", 0x41, 0x01); 
				AddNewMnemonic("FRA", 0x41, 0x02); 
				AddNewMnemonic("GER", 0x41, 0x03); 
				AddNewMnemonic("ITA", 0x41, 0x04); 
				AddNewMnemonic("RUS", 0x41, 0x05); 
				AddNewMnemonic("TUR", 0x41, 0x06); 
				Trace.Unindent();

				//Provinces
				Trace.WriteLineIf(connectorSwitch.Enabled,
					string.Format(CultureInfo.InvariantCulture,
					ErrorMessages.Connection_Trace_Provinces
					), traceCategory);
				Trace.Indent();
				// Inland non-SC provinces (0x50)
				AddNewMnemonic("BOH", 0x50, 0x00);
				AddNewMnemonic("BUR", 0x50, 0x01);
				AddNewMnemonic("GAL", 0x50, 0x02);
				AddNewMnemonic("RUH", 0x50, 0x03);
				AddNewMnemonic("SIL", 0x50, 0x04);
				AddNewMnemonic("TYR", 0x50, 0x05);
				AddNewMnemonic("UKR", 0x50, 0x06);
				// Inland SC provinces (0x51)
				AddNewMnemonic("BUD", 0x51, 0x07);
				AddNewMnemonic("MOS", 0x51, 0x08);
				AddNewMnemonic("MUN", 0x51, 0x09);
				AddNewMnemonic("PAR", 0x51, 0x0A);
				AddNewMnemonic("SER", 0x51, 0x0B);
				AddNewMnemonic("VIE", 0x51, 0x0C);
				AddNewMnemonic("WAR", 0x51, 0x0D);
				// Sea non-SC provinces (0x52)
				AddNewMnemonic("ADR", 0x52, 0x0E);
				AddNewMnemonic("AEG", 0x52, 0x0F);
				AddNewMnemonic("BAL", 0x52, 0x10);
				AddNewMnemonic("BAR", 0x52, 0x11);
				AddNewMnemonic("BLA", 0x52, 0x12);
				AddNewMnemonic("EAS", 0x52, 0x13);
				AddNewMnemonic("ECH", 0x52, 0x14);
				AddNewMnemonic("GOB", 0x52, 0x15);
				AddNewMnemonic("GOL", 0x52, 0x16);
				AddNewMnemonic("HEL", 0x52, 0x17);
				AddNewMnemonic("ION", 0x52, 0x18);
				AddNewMnemonic("IRI", 0x52, 0x19);
				AddNewMnemonic("MAO", 0x52, 0x1A);
				AddNewMnemonic("NAO", 0x52, 0x1B);
				AddNewMnemonic("NTH", 0x52, 0x1C);
				AddNewMnemonic("NWG", 0x52, 0x1D);
				AddNewMnemonic("SKA", 0x52, 0x1E);
				AddNewMnemonic("TYS", 0x52, 0x1F);
				AddNewMnemonic("WES", 0x52, 0x20);
				// Coastal non-SC provinces (0x54)
				AddNewMnemonic("ALB", 0x54, 0x21);
				AddNewMnemonic("APU", 0x54, 0x22);
				AddNewMnemonic("ARM", 0x54, 0x23);
				AddNewMnemonic("CLY", 0x54, 0x24);
				AddNewMnemonic("FIN", 0x54, 0x25);
				AddNewMnemonic("GAS", 0x54, 0x26);
				AddNewMnemonic("LVN", 0x54, 0x27);
				AddNewMnemonic("NAF", 0x54, 0x28);
				AddNewMnemonic("PIC", 0x54, 0x29);
				AddNewMnemonic("PIE", 0x54, 0x2A);
				AddNewMnemonic("PRU", 0x54, 0x2B);
				AddNewMnemonic("SYR", 0x54, 0x2C);
				AddNewMnemonic("TUS", 0x54, 0x2D);
				AddNewMnemonic("WAL", 0x54, 0x2E);
				AddNewMnemonic("YOR", 0x54, 0x2F);
				// Coastal SC provinces (0x55)
				AddNewMnemonic("ANK", 0x55, 0x30);
				AddNewMnemonic("BEL", 0x55, 0x31);
				AddNewMnemonic("BER", 0x55, 0x32);
				AddNewMnemonic("BRE", 0x55, 0x33);
				AddNewMnemonic("CON", 0x55, 0x34);
				AddNewMnemonic("DEN", 0x55, 0x35);
				AddNewMnemonic("EDI", 0x55, 0x36);
				AddNewMnemonic("GRE", 0x55, 0x37);
				AddNewMnemonic("HOL", 0x55, 0x38);
				AddNewMnemonic("KIE", 0x55, 0x39);
				AddNewMnemonic("LON", 0x55, 0x3A);
				AddNewMnemonic("LVP", 0x55, 0x3B);
				AddNewMnemonic("MAR", 0x55, 0x3C);
				AddNewMnemonic("NAP", 0x55, 0x3D);
				AddNewMnemonic("NWY", 0x55, 0x3E);
				AddNewMnemonic("POR", 0x55, 0x3F);
				AddNewMnemonic("ROM", 0x55, 0x40);
				AddNewMnemonic("RUM", 0x55, 0x41);
				AddNewMnemonic("SEV", 0x55, 0x42);
				AddNewMnemonic("SMY", 0x55, 0x43);
				AddNewMnemonic("SWE", 0x55, 0x44);
				AddNewMnemonic("TRI", 0x55, 0x45);
				AddNewMnemonic("TUN", 0x55, 0x46);
				AddNewMnemonic("VEN", 0x55, 0x47);
				// Bi-coastal SC provinces (0x57)
				AddNewMnemonic("BUL", 0x57, 0x48);
				AddNewMnemonic("SPA", 0x57, 0x49);
				AddNewMnemonic("STP", 0x57, 0x4A);
				Trace.Unindent();

				Trace.Unindent();
			}
		}

		private void AddNewMnemonic(string mnemonic, byte first, byte second)
		{
			TokenFactory.AddNewMnemonic(mnemonic, first, second);
			TokenType tokenType = (TokenType)first;
			if (tokenType == TokenType.Power)
			{
                this.game.Powers.Add(mnemonic, new Power(mnemonic));
				Trace.WriteLineIf(connectorSwitch.Enabled,
					string.Format(CultureInfo.InvariantCulture,
					ErrorMessages.Connection_Trace_Power,
					mnemonic), traceCategory);
			}
			else
			{
				ProvinceType provinceType;
				switch (tokenType)
				{
					case TokenType.ProvinceInlandNonSupplyCenter :
						provinceType = ProvinceType.InlandNonSupplyCenter;
						break;
					case TokenType.ProvinceInlandSupplyCenter :
						provinceType = ProvinceType.InlandSupplyCenter;
						break;
					case TokenType.ProvinceSeaNonSupplyCenter :
						provinceType = ProvinceType.SeaNonSupplyCenter;
						break;
					case TokenType.ProvinceSeaSupplyCenter :
						provinceType = ProvinceType.SeaSupplyCenter;
						break;
					case TokenType.ProvinceCoastalNonSupplyCenter :
						provinceType = ProvinceType.CoastalNonSupplyCenter;
						break;
					case TokenType.ProvinceCoastalSupplyCenter :
						provinceType = ProvinceType.CoastalSupplyCenter;
						break;
					case TokenType.ProvinceBicoastalNonSupplyCenter :
						provinceType = ProvinceType.BicostalNonSupplyCenter;
						break;
					case TokenType.ProvinceBicoastalSupplyCenter :
						provinceType = ProvinceType.BicostalSupplyCenter;
						break;
					default :
						throw new ArgumentException(
                            string.Format(CultureInfo.InvariantCulture,
							ErrorMessages.Connection_Exception_UnknownRMToken,
							tokenType.ToString()) );
				}
				Trace.WriteLineIf(connectorSwitch.Enabled,
					string.Format(CultureInfo.InvariantCulture,
					ErrorMessages.Connection_Trace_Province,
					mnemonic, provinceType.ToString()), traceCategory);
                this.game.Map.Provinces.Add(mnemonic, new Province(mnemonic, provinceType));
			}
		}

		private void HandleDiplomaticMessage(byte[] bytes)
		{
			TokenMessage message = new TokenMessage(TokenFactory.FromByteArray(bytes));
			Trace.WriteLineIf(connectorSwitch.Enabled,
				string.Format(CultureInfo.InvariantCulture,
				ErrorMessages.Connection_Trace_MessageReceived,
				message.ToString()), traceCategory);
			if (DiplomaticMessageReceived != null)
			{
				DiplomaticMessageReceived(this, new DiplomaticMessageEventArgs(message));
			}
		}

		private static void HandleFinalMessage(byte[] message)
		{
            Trace.WriteLineIf(connectorSwitch.Enabled,
                string.Format(CultureInfo.InvariantCulture,
                ErrorMessages.Connection_Trace_FinalMessage,
                message[0]), traceCategory);
        }

		private static void HandleErrorMessage(byte[] message)
		{
			string detail;
			if (message[1] <= 0x01 && message[1] <= 0x0E)
			{
                detail = ErrorMessages.ResourceManager.GetString(
                    string.Format(CultureInfo.InvariantCulture,
                        "Connection_ErrorMessage_{0}",
                        message[1].ToString("X2", CultureInfo.InvariantCulture))
                    , null);
                if( detail == null )
                {
                    detail = string.Format(CultureInfo.InvariantCulture,
                        ErrorMessages.Connection_ErrorMessage_Unknown,
                        message[0].ToString(CultureInfo.InvariantCulture),
                        message[1].ToString(CultureInfo.InvariantCulture));
                }
			}
			else
			{
				detail = string.Format(CultureInfo.InvariantCulture,
					ErrorMessages.Connection_ErrorMessage_Unknown,
					message[0].ToString(CultureInfo.InvariantCulture),
					message[1].ToString(CultureInfo.InvariantCulture));
			}
			Trace.WriteLineIf(connectorSwitch.Enabled,
				string.Format(CultureInfo.InvariantCulture,
				ErrorMessages.Connection_Trace_ErrorMessage,
				detail), traceCategory);
        }

        #endregion

        #region Nested types

        /// <summary>The type of message
        /// </summary>
        private enum MessageType : byte
        {
            IM = 0x00,
            RM = 0x01,
            DM = 0x02,
            FM = 0x03,
            EM = 0x04,
        }


        private class StreamReaderState
        {
            private byte[] buffer;
            public byte[] Buffer
            {
                get
                {
                    return this.buffer;
                }
            }

            private NetworkStream stream;
            public NetworkStream Stream
            {
                get
                {
                    return this.stream;
                }
            }

            private byte msgType;
            public byte MsgType
            {
                get
                {
                    return this.msgType;
                }
            }

            public StreamReaderState(byte[] buffer, NetworkStream stream)
            {
                this.buffer = buffer;
                this.stream = stream;
            }
            public StreamReaderState(byte[] buffer, NetworkStream stream, byte msgType)
            {
                this.buffer = buffer;
                this.stream = stream;
                this.msgType = msgType;

            }
        }

        #endregion
    }
}
