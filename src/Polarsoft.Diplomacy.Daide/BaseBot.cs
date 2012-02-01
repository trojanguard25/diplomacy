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
using Polarsoft.Diplomacy.Daide;
using System.Diagnostics;
using System.Globalization;
using Polarsoft.Diplomacy.Orders;
using System.Collections.Generic;
using ErrorMessages = Polarsoft.Diplomacy.Daide.Properties.ErrorMessages;
using System.Collections.ObjectModel;

namespace Polarsoft.Diplomacy.Daide
{
	/// <summary>The cause for the end of the game.
	/// </summary>
	public enum FinishingCause
	{
		///	<summary>The game hasn't ended yet.</summary>
		None,
		///	<summary>An OFF command was sent.</summary>
		Off,
		///	<summary>Someone won solo.</summary>
		Solo,
		///	<summary>The game was a draw.</summary>
		Draw
	}

	/// <summary>A class that can be used to derive AIs from.
	/// </summary>
	public class BaseBot
    {
        #region Instrumentation

        const string traceCategory = "DAIDE::BaseBot";
		private static BooleanSwitch baseBotSwitch =
			new BooleanSwitch(traceCategory, "Trace messages from the Basebot");

        #endregion

        #region Fields

        private Connection connection;
        private Game game;
        private Power power;
        private List<Order> orders;
        private Hashtable options;
        private int passcode;
        private List<Power> powersInCivilDisorder;
        private FinishingCause finishingCause = FinishingCause.None;
        private TokenMessage latestReceivedMessage;
        private bool mapHasBeenFetched;

        #endregion

        #region Protected properties

        /// <summary>The current <see cref="Game"/>.
		/// </summary>
		/// <value>The current <see cref="Game"/>.</value>
		protected Game Game
		{
			get
			{
				return this.game;
			}
		}

		/// <summary>Gets the <see cref="Power"/> of this Bot.
		/// </summary>
		/// <value>The <see cref="Power"/> of this Bot.</value>
		protected Power Power
		{
			get
			{
				return this.power;
			}
		}

		/// <summary>Gets the orders for the current turn.
		/// </summary>
		/// <value>The orders for the current turn.</value>
        protected IList<Order> CurrentOrders
		{
			get
			{
                return this.orders;
			}
		}

		/// <summary>Gets the options that were specified when the game was started.
		/// </summary>
		/// <value>The options that were specified when the game was started.</value>
		protected Hashtable Options
		{
			get
			{
                return this.options;
			}
		}

		/// <summary>The passcode that was sent from the server when the game was started.
		/// </summary>
		/// <value>The passcode that was sent from the server when the game was started</value>
		protected int Passcode
		{
			get
			{
                return this.passcode;
			}
		}

		/// <summary>Gets the <see cref="Power">Powers</see> that are in civil disorder.
		/// </summary>
        /// <value>A list of <see cref="Power">Powers</see> that are in civil disorder.</value>
        protected IList<Power> PowersInCivilDisorder
		{
			get
			{
                return this.powersInCivilDisorder;
			}
		}

		/// <summary>The cause for the end of the game.
		/// </summary>
		/// <value>The cause for the end of the game.</value>
		protected FinishingCause EndOfGameCause
		{
			get
			{
                return this.finishingCause;
			}
		}

		/// <summary>Gets the last received message.
		/// </summary>
		/// <value>The last received message.</value>
		protected TokenMessage LatestReceivedMessage
		{
			get
			{
                return this.latestReceivedMessage;
			}
        }

        #endregion

        #region Construction

        /// <summary>Creates a new <see cref="BaseBot"/> instance.
		/// </summary>
		public BaseBot()
		{
            this.game = new Game();
            this.orders = new List<Order>();

            this.options = new Hashtable(StringComparer.OrdinalIgnoreCase);
            this.powersInCivilDisorder = new List<Power>();

            this.connection = new Daide.Connection(game);
            this.connection.DiplomaticMessageReceived += new EventHandler<DiplomaticMessageEventArgs>(connection_DiplomaticMessageReceived);
        }

        #endregion

        #region Implementation

		private void connection_DiplomaticMessageReceived(object sender, Daide.DiplomaticMessageEventArgs e)
		{
#if DEBUG
			Trace.WriteLine("Message received: " + e.Message.ToString());
#endif
            this.latestReceivedMessage = e.Message;
			TokenMessage msg = e.Message;
			Token firstToken = msg.Tokens[0];
			if (firstToken.ToString() == null)
			{

				Trace.WriteLineIf( baseBotSwitch.Enabled,
					string.Format(CultureInfo.InvariantCulture,
						ErrorMessages.BaseBot_Trace_UnknownFirstToken,
						firstToken.ToBytes()[0].ToString("x", CultureInfo.InvariantCulture),
						firstToken.ToBytes()[1].ToString("x", CultureInfo.InvariantCulture)),
                    traceCategory);
				Debug.Assert(false);
				return;
			}
			switch (firstToken.ToString().ToUpper(CultureInfo.InvariantCulture))
			{
				case "ADM" :
					ProcessADM(msg);
					break;
				case "CCD" :
					PreProcessCCD(msg);
					break;
				case "DRW" :
					ProcessDRW(msg);
					break;
				case "FRM" :
					ProcessFRM(msg);
					break;
				case "HLO" :
					PreProcessHLO(msg);
					break;
				case "HUH" :
					ProcessHUH(msg);
					break;
				case "LOD" :
					ProcessLOD(msg);
					break;
				case "MAP" :
					PreProcessMAP(msg);
					break;
				case "MDF" :
					PreProcessMDF(msg);
					break;
				case "MIS" :
					ProcessMIS(msg);
					break;
				case "NOT" :
					PreProcessNOT(msg);
					break;
				case "NOW" :
					PreProcessNOW(msg);
					break;
				case "OFF" :
					ProcessOFF(msg);
					break;
				case "ORD" :
					PreProcessORD(msg);
					break;
				case "OUT" :
					PreProcessOUT(msg);
					break;
				case "PRN" :
					ProcessPRN(msg);
					break;
				case "REJ" :
					PreProcessREJ(msg);
					break;
				case "SCO" :
					PreProcessSCO(msg);
					break;
				case "SLO" :
					PreProcessSLO(msg);
					break;
				case "SMR" :
					ProcessSMR(msg);
					break;
				case "SVE" :
					ProcessSVE(msg);
					break;
				case "THX" :
					ProcessTHX(msg);
					break;
				case "TME" :
					ProcessTME(msg);
					break;
				case "YES" :
					PreProcessYES(msg);
					break;
				default :
					Trace.WriteLineIf( baseBotSwitch.Enabled,
						string.Format(CultureInfo.InvariantCulture,
						ErrorMessages.Basebot_Trace_UnexpectedFirstToken,
                        msg.ToString()), traceCategory);
					Debug.Assert(false);
					break;
			}
        }

        private void PreProcessCCD(TokenMessage msg)
        {
            Token powerToken = msg.SubMessages[1].Tokens[0];
            Power power = this.game.Powers[powerToken.ToString()];
            if( !this.powersInCivilDisorder.Contains(power) )
            {
                this.powersInCivilDisorder.Add(power);
                ProcessCCD(msg, true);
            }
            else
            {
                ProcessCCD(msg, false);
            }
        }

        private void PreProcessHLO(TokenMessage msg)
        {
            this.power = this.game.Powers[msg.SubMessages[1].Tokens[0].ToString()];
            this.passcode = msg.SubMessages[2].Tokens[0].ToNumber();
            //Process variants
            TokenMessage variants = msg.SubMessages[3];
            this.options.Clear();
            foreach( TokenMessage message in variants.SubMessages )
            {
                string optName = message.Tokens[0].ToString();
                switch( optName.ToUpper(CultureInfo.InvariantCulture) )
                {
                    case "AOA":
                        this.options.Add(optName, true);
                        break;
                    case "BTL":
                        this.options.Add(optName, message.Tokens[1].ToNumber());
                        break;
                    case "DSD":
                        this.options.Add(optName, true);
                        break;
                    case "LVL":
                        this.options.Add(optName, message.Tokens[1].ToNumber());
                        break;
                    case "MTL":
                        this.options.Add(optName, message.Tokens[1].ToNumber());
                        break;
                    case "NPB":
                        this.options.Add(optName, true);
                        break;
                    case "NPR":
                        this.options.Add(optName, true);
                        break;
                    case "PDA":
                        this.options.Add(optName, true);
                        break;
                    case "PTL":
                        this.options.Add(optName, message.Tokens[1].ToNumber());
                        break;
                    case "RTL":
                        this.options.Add(optName, message.Tokens[1].ToNumber());
                        break;
                }
            }

            ProcessHLO(msg);
        }

        private void PreProcessMAP(TokenMessage msg)
        {
            if( !this.mapHasBeenFetched )
            {
                this.connection.Send(new TokenMessage(Token.MDF));
            }
            this.game.Map.Name = msg.SubMessages[1].Tokens[0].ToString();
            ProcessMAP(msg);
        }

        private void PreProcessMDF(TokenMessage msg)
        {
            if( !this.mapHasBeenFetched )
            {
                Util.CreateMapFromMDF(msg, this.game);
            }

            ProcessMDF(msg);

            if( !this.mapHasBeenFetched )
            {
                this.connection.Send(Token.YES & ( Token.MAP & TokenFactory.FromString(this.game.Map.Name) ));
                this.mapHasBeenFetched = true;
            }
        }

        private void PreProcessNOT(TokenMessage msg)
        {
            TokenMessage notMessage = msg.SubMessages[1];
            switch( notMessage.Tokens[0].ToString().ToUpper(CultureInfo.InvariantCulture) )
            {
                case "CCD":
                    PreProcessNOT_CCD(msg, notMessage.SubMessages[1]);
                    break;
                case "TME":
                    ProcessNOT_TME(msg, notMessage.SubMessages[1]);
                    break;
                default:
                    ProcessNOT_Unexpected(msg);
                    break;
            }
        }

        private void PreProcessNOT_CCD(TokenMessage msg, TokenMessage messageParameters)
        {
            Power power = this.game.Powers[messageParameters.Tokens[0].ToString()];
            if( this.powersInCivilDisorder.Contains(power) )
            {
                this.powersInCivilDisorder.Remove(power);
                ProcessNOT_CCD(msg, messageParameters, true);
            }
            else
            {
                ProcessNOT_CCD(msg, messageParameters, false);
            }
        }

        private void PreProcessNOW(TokenMessage msg)
        {
            BeforeNewTurn();
            Util.UpdateGameFromNOW(msg, this.game);
            this.orders.Clear();
            ProcessNOW(msg);
        }

        private void PreProcessORD(TokenMessage msg)
        {
            //Util.UpdateGameFromORD(msg, this.game);
            ProcessORD(msg);
        }

        private void PreProcessOUT(TokenMessage msg)
        {
            //TODO: Press check.
            ProcessOUT(msg);
        }

        private void PreProcessREJ(TokenMessage msg)
        {
            TokenMessage rejectMessage = msg.SubMessages[1];
            switch( rejectMessage.Tokens[0].ToString().ToUpper(CultureInfo.InvariantCulture) )
            {
                case "ADM":
                    ProcessREJ_ADM(msg, rejectMessage);
                    break;
                case "DRW":
                    ProcessREJ_DRW(msg, rejectMessage);
                    break;
                case "GOF":
                    ProcessREJ_GOF(msg, rejectMessage);
                    break;
                case "HLO":
                    ProcessREJ_HLO(msg, rejectMessage);
                    break;
                case "HST":
                    ProcessREJ_HST(msg, rejectMessage);
                    break;
                case "IAM":
                    ProcessREJ_IAM(msg, rejectMessage);
                    break;
                case "NME":
                    ProcessREJ_NME(msg, rejectMessage);
                    break;
                case "NOT":
                    PreProcessREJ_NOT(msg, rejectMessage);
                    break;
                case "NOW":
                    ProcessREJ_NOW(msg, rejectMessage);
                    break;
                case "ORD":
                    ProcessREJ_ORD(msg, rejectMessage);
                    break;
                case "SCO":
                    ProcessREJ_SCO(msg, rejectMessage);
                    break;
                case "SND":
                    ProcessREJ_SND(msg, rejectMessage);
                    break;
                case "SUB":
                    ProcessREJ_SUB(msg, rejectMessage);
                    break;
                case "TME":
                    ProcessREJ_TME(msg, rejectMessage);
                    break;
                default:
                    ProcessREJ_Unexpected(msg, rejectMessage);
                    break;
            }
        }

        private void PreProcessREJ_NOT(TokenMessage msg, TokenMessage rejectMessage)
        {
            Token command = rejectMessage.SubMessages[1].Tokens[0];
            switch( command.ToString().ToUpper(CultureInfo.InvariantCulture) )
            {
                case "DRW":
                    ProcessREJ_NOT_DRW(msg, rejectMessage);
                    break;
                case "GOF":
                    ProcessREJ_NOT_GOF(msg, rejectMessage);
                    break;
                default:
                    ProcessREJ_NOT_Unexpected(msg, rejectMessage);
                    break;
            }
        }

        private void PreProcessSCO(TokenMessage msg)
        {
            Util.UpdateGameFromSCO(msg, this.game);
            ProcessSCO(msg);
        }

        private void PreProcessSLO(TokenMessage msg)
        {
            this.finishingCause = FinishingCause.Solo;
            ProcessSLO(msg);
        }

        private void PreProcessYES(TokenMessage msg)
        {
            TokenMessage confirmedMessage = msg.SubMessages[1];
            switch( confirmedMessage.Tokens[0].ToString().ToUpper(CultureInfo.InvariantCulture) )
            {
                case "DRW":
                    ProcessYES_DRW(msg, confirmedMessage);
                    break;
                case "GOF":
                    ProcessYES_GOF(msg, confirmedMessage);
                    break;
                case "IAM":
                    ProcessYES_IAM(msg, confirmedMessage);
                    break;
                case "NME":
                    ProcessYES_NME(msg, confirmedMessage);
                    break;
                case "NOT":
                    ProcessYES_NOT(msg, confirmedMessage);
                    break;
                case "OBS":
                    ProcessYES_OBS(msg, confirmedMessage);
                    break;
                case "SND":
                    ProcessYES_SND(msg, confirmedMessage);
                    break;
                case "TME":
                    ProcessYES_TME(msg, confirmedMessage);
                    break;
                default:
                    ProcessYES_Unexpected(msg, confirmedMessage);
                    break;
            }
        }

        #endregion

        #region Protected methods

		/// <summary>Connects to a DAIDE server running at the specified location.
		/// This overloaded version will get the version parameter from the current assembly version.
		/// </summary>
		/// <param name="hostName">The name of the server. This can either be an IP number or a server name.</param>
		/// <param name="port">The port to connect to.</param>
		/// <param name="name">The name of the bot.</param>
		protected void Connect(string hostName, int port, string name)
		{
			Version v = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            Connect(hostName, port, name, v.ToString());
		}

		/// <summary>Connects to a DAIDE server running at the specified location.
		/// </summary>
		/// <param name="hostName">The name of the server. This can either be an IP number or a server name.</param>
		/// <param name="port">The port to connect to.</param>
		/// <param name="name">The name of the bot.</param>
		/// <param name="version">The version of the bot.</param>
		protected void Connect(string hostName, int port, string name, string version)
		{
            this.connection.Connect(hostName, port);
			TokenMessage msg = new TokenMessage(Token.NME);
			msg += new TokenMessage(TokenFactory.FromString(name)).MessageEnclosed;
			msg += new TokenMessage(TokenFactory.FromString(version)).MessageEnclosed;
			Send(msg);
		}

		/// <summary>Sends the <see cref="TokenMessage"/> to the server.
		/// </summary>
		/// <param name="msg">The message to send.</param>
		protected void Send(TokenMessage msg)
		{
			//TODO: Preprocess some orders, so that we will not react
			// to some client orders, such as HST, etc.
            this.connection.Send(msg);
		}

		/// <summary>Submits the <see cref="CurrentOrders"/> to the server.
		/// </summary>
		protected void SubmitOrders()
		{
            if( this.orders.Count == 0 )
			{
				return;
			}
			TokenMessage msg = new TokenMessage(Token.SUB);
            foreach( Order order in this.orders )
			{
				TokenMessage msgOrder = new TokenMessage();
				//object[] parameters = order.GetOrderParameters();
				switch (order.OrderType)
				{
					case OrderType.Build :
					{
						BuildOrder o = order as BuildOrder; 
						//Debug.Assert(parameters.Length == 0);
						msgOrder.Add( Util.ToOrderFormat(o.Unit).MessageEnclosed );
						msgOrder.Add( Token.BLD );
						break;
					}

					case OrderType.Convey :
					{
						ConveyOrder o = order as ConveyOrder; 
						//Debug.Assert(parameters.Length == 2);
						//Debug.Assert(parameters[0] is Unit);
						//Debug.Assert(parameters[1] is Location);
						msgOrder.Add( Util.ToOrderFormat(o.Unit).MessageEnclosed );
						msgOrder.Add( Token.CVY );
						msgOrder.Add( Util.ToOrderFormat(o.ConveyedUnit).MessageEnclosed );
						msgOrder.Add( Token.CTO );
						msgOrder.Add( Util.ToOrderFormat(o.TargetProvince) );
						break;
					}
					case OrderType.Disband :
					{
						DisbandOrder o = order as DisbandOrder;
						//Debug.Assert(parameters.Length == 0);
						msgOrder.Add( Util.ToOrderFormat(o.Unit).MessageEnclosed );
						msgOrder.Add( Token.DSB );
						break;
					}

					case OrderType.Hold :
					{
						HoldOrder o = order as HoldOrder;
						//Debug.Assert(parameters.Length == 0);
						msgOrder.Add( Util.ToOrderFormat(o.Unit).MessageEnclosed );
						msgOrder.Add( Token.HLD );
						break;
					}

					case OrderType.Move :
					{
						MoveOrder o = order as MoveOrder;
						//Debug.Assert(parameters.Length == 1);
						//Debug.Assert(parameters[0] is Location);
						msgOrder.Add( Util.ToOrderFormat(o.Unit).MessageEnclosed );
						msgOrder.Add( Token.MTO );
						msgOrder.Add( Util.ToOrderFormat(o.TargetLocation) );
						break;
					}

					case OrderType.MoveByConvoy :
					{
						MoveByConvoyOrder o = order as MoveByConvoyOrder;
						//Debug.Assert(parameters.Length == 1);
						//Debug.Assert(parameters[0] is Route);
						//Route route = parameters[0] as Route;
						msgOrder.Add( Util.ToOrderFormat(o.Unit).MessageEnclosed );
						msgOrder.Add( Token.CTO );
						msgOrder.Add( Util.ToOrderFormat(o.Route.End) );
						msgOrder.Add( Token.VIA );
						TokenMessage via = new TokenMessage();
						foreach (Province province in o.Route.Via.Provinces)
						{
							via.Add( Util.ToOrderFormat(province) );
						}
						msgOrder.Add( via.MessageEnclosed );
						break;
					}

					case OrderType.Remove :
					{
						RemoveOrder o = order as RemoveOrder;
						//Debug.Assert(parameters.Length == 0);
						msgOrder.Add( Util.ToOrderFormat(o.Unit).MessageEnclosed );
						msgOrder.Add( Token.REM );
						break;
					}

					case OrderType.Retreat :
					{
						RetreatOrder o = order as RetreatOrder;
						//Debug.Assert(parameters.Length == 1);
						//Debug.Assert(parameters[0] is Location);
						msgOrder.Add( Util.ToOrderFormat(o.Unit).MessageEnclosed );
						msgOrder.Add( Token.RTO );
						msgOrder.Add( Util.ToOrderFormat(o.RetreatLocation) );
						break;
					}

					case OrderType.SupportHold :
					{
						SupportHoldOrder o = order as SupportHoldOrder;
						//Debug.Assert(parameters.Length == 1);
						//Debug.Assert(parameters[0] is Unit);
						msgOrder.Add( Util.ToOrderFormat(o.Unit).MessageEnclosed );
						msgOrder.Add( Token.SUP );
						msgOrder.Add( Util.ToOrderFormat(o.SupportedUnit).MessageEnclosed );
						break;
					}

					case OrderType.SupportMove :
					{
						SupportMoveOrder o = order as SupportMoveOrder;
						//Debug.Assert(parameters.Length == 2);
						//Debug.Assert(parameters[0] is Unit);
						//Debug.Assert(parameters[1] is Province);
						msgOrder.Add( Util.ToOrderFormat(o.Unit).MessageEnclosed );
						msgOrder.Add( Token.SUP );
						msgOrder.Add( Util.ToOrderFormat(o.SupportedUnit).MessageEnclosed );
						msgOrder.Add( Token.MTO );
						msgOrder.Add( Util.ToOrderFormat(o.TargetProvince) );
						break;
					}

					case OrderType.WaiveBuild :
					{
						WaiveBuildOrder o = order as WaiveBuildOrder;
						//Debug.Assert(parameters.Length == 1);
						//Debug.Assert(parameters[0] is Power);
						msgOrder.Add( Util.ToOrderFormat(o.Power) );
						msgOrder.Add( Token.WVE );
						break;
					}

					default :
						throw new InvalidOperationException(
							string.Format(CultureInfo.InvariantCulture,
							ErrorMessages.Basebot_Exception_UnknownOrderType,
							order.OrderType.ToString()));
				}
				msg.Add(msgOrder.MessageEnclosed);
			}
            this.connection.Send(msg);
		}

		/// <summary>Occurs before any processing starts for a new turn.
		/// </summary>
		virtual protected void BeforeNewTurn()
		{
        }

        #endregion

        #region Overridable Message processors

        /// <summary>Processes a CCD message (County in Civil Disorder).
		/// The default version does nothing.
		/// </summary>
		/// <param name="msg">The CCD message</param>
		/// <param name="isNewDisconnection">Set to true if the notification is a new country in civil disorder.
		/// <see cref="powersInCivilDisorder"/></param>
		protected virtual void ProcessCCD(TokenMessage msg, bool isNewDisconnection)
		{
		}

		/// <summary>Processes a DRW message (a Draw has been declared by the server).
		/// The default version sets <see cref="finishingCause"/> to <see cref="FinishingCause.Draw"/>.
		/// </summary>
		/// <param name="msg">The DRW message.</param>
		protected virtual void ProcessDRW(TokenMessage msg)
		{
            this.finishingCause = FinishingCause.Draw;
		}

		/// <summary>Processes a FRM message (press from another <see cref="Power"/>).
		/// The default version responds with a HUH message, followed with a TRY message that is empty.
		/// </summary>
		/// <param name="msg">The FRM message.</param>
		protected virtual void ProcessFRM(TokenMessage msg)
		{
			// The BaseBot does not understand press.
			// Reply with a "HUH" and and "TRY" that is
			// empty.
			if (msg.SubMessages[3].Tokens[0] != Token.HUH
				&& msg.SubMessages[3].Tokens[0] != Token.TRY)
			{
                TokenMessage huh = Token.SND
                    & msg.SubMessages[1].Tokens[0]
                    & ( Token.HUH
                    & ( Token.ERR + msg.SubMessages[3] ) );
				TokenMessage tryMsg = Token.SND
					& msg.SubMessages[1].Tokens[0]
					& (Token.TRY
					& new TokenMessage());


                this.connection.Send(huh);
                this.connection.Send(tryMsg);
			}
		}

		/// <summary>Processes a HUH message (a previously sent message has a syntax error).
		/// The default version sends the message to the log.
		/// </summary>
		/// <param name="msg">The HUH message.</param>
		protected virtual void ProcessHUH(TokenMessage msg)
		{
			Trace.WriteLineIf(baseBotSwitch.Enabled,
				string.Format(CultureInfo.InvariantCulture,
				ErrorMessages.Basebot_Trace_HUHMessage,
				msg.ToString()),
                traceCategory);
		}

		/// <summary>Processes a LOD message (Load a previously saved game).
		/// The default version responds by rejecting the message.
		/// </summary>
		/// <param name="msg">The LOD message.</param>
		protected virtual void ProcessLOD(TokenMessage msg)
		{
            this.connection.Send(Token.REJ & msg);
		}

		/// <summary>Processes a MIS message (The server is missing some orders).
		/// The default version does nothing.
		/// </summary>
		/// <param name="msg">The MIS message.</param>
		protected virtual void ProcessMIS(TokenMessage msg)
		{
		}

		/// <summary>Processes an OFF message (Turn off).
		/// The default version sets the <see cref="EndOfGameCause"/> to <see cref="FinishingCause.Off"/>.
		/// </summary>
		/// <param name="msg">The OFF message.</param>
		protected virtual void ProcessOFF(TokenMessage msg)
		{
            this.finishingCause = FinishingCause.Off;
		}

		/// <summary>Processes an OUT message (The indicated country has been eliminated from the game).
		/// The default version does nothing.
		/// </summary>
		/// <param name="msg">The OUT message.</param>
		protected virtual void ProcessOUT(TokenMessage msg)
		{
		}

		/// <summary>Processes a PRN message (The message does not have a correct set of parentheses).
		/// The default version sends the message to the log.
		/// </summary>
		/// <param name="msg">The PRN message.</param>
		protected virtual void ProcessPRN(TokenMessage msg)
		{
			Trace.WriteLineIf(baseBotSwitch.Enabled,
				string.Format(CultureInfo.InvariantCulture,
				ErrorMessages.Basebot_Trace_PRNMessage,
                msg.ToString()), traceCategory); 
		}

		/// <summary>Processes a SLO message (Someone has won solo).
		/// The default version set the <see cref="finishingCause"/> to <see cref="FinishingCause.Solo"/>.
		/// </summary>
		/// <param name="msg">The SLO message.</param>
		protected virtual void ProcessSLO(TokenMessage msg)
		{
            this.finishingCause = FinishingCause.Solo;
		}

		/// <summary>Processes a SMR message (Participants in the game).
		/// The default version does nothing.
		/// </summary>
		/// <param name="msg">The SMR message.</param>
		protected virtual void ProcessSMR(TokenMessage msg)
		{
		}

		/// <summary>Processes a SVE message (Save the game).
		/// The default version does responds with a YES message (but does not save the game).
		/// </summary>
		/// <param name="msg">The SVE message.</param>
		protected virtual void ProcessSVE(TokenMessage msg)
		{
            this.connection.Send(Token.YES & msg);
		}

		/// <summary>Processes a THX message (Thanks for an order).
		/// The default version sends the message to the log if the message indicated an error.
		/// If possible, it also sends a replacement order.
		/// </summary>
		/// <param name="msg">The THX message.</param>
		/// <remarks>
		/// The action that is taken depends on the note that is returned from the server, as follows:
		/// MBV - no action taken, the order is correct as it stands.
		/// NYU - message is logged.
		/// NRS - message is logged.
		/// HLD - message is logged.
		/// NRN - message is logged.
		/// NMB - message is logged.
		/// NMR - message is logged.
		/// FAR - message is logged, and a replacement Hold order is sent.
		/// NSP - message is logged, and a replacement Hold order is sent.
		/// NSU - message is logged, and a replacement Hold order is sent.
		/// NAS - message is logged, and a replacement Hold order is sent.
		/// NSF - message is logged, and a replacement Hold order is sent.
		/// NSA - message is logged, and a replacement Hold order is sent.
		/// NVR - message is logged, and a replacement Disband order is sent.
		/// YSC - message is logged, and a replacement Waive order is sent.
		/// ESC - message is logged, and a replacement Waive order is sent.
		/// HSC - message is logged, and a replacement Waive order is sent.
		/// NSC - message is logged, and a replacement Waive order is sent.
		/// CST - message is logged, and a replacement Waive order is sent.
		/// </remarks>
		protected virtual void ProcessTHX(TokenMessage msg)
		{
			TokenMessage order = msg.SubMessages[1];
			Token note = msg.SubMessages[2].Tokens[0];

			TokenMessage replacementOrder = null;

			if (note == Token.MBV)
			{
				//Valid order
				return;
			}

			//All invalid orders are here!
			if (   note == Token.NYU
				|| note == Token.NRS )
			{
				// Not the right season / not your unit
				// There's not much to do.
			}

			TokenMessage unit = order.SubMessages[0].MessageEnclosed;;
			if (order.SubMessages[1].Tokens[0] == Token.HLD)
			{
				// A unit was ordered to hold, but the order failed.
				// There's not much we can do!
			}

			if (   note == Token.NRN
				|| note == Token.NMB
				|| note == Token.NMR )
			{
				// Order wasn't needed in the first place!
			}

			if (   note == Token.FAR
				|| note == Token.NSP
				|| note == Token.NSU
				|| note == Token.NAS
				|| note == Token.NSF
				|| note == Token.NSA )
			{
				// Illegal movement order. Replace with a hold order.
				replacementOrder = unit + Token.HLD;
			}

			if (note == Token.NVR)
			{
				// Illegal retreat order. Replace with a disband order
				replacementOrder = unit + Token.DSB;
			}

			if (   note == Token.YSC
				|| note == Token.ESC
				|| note == Token.HSC
				|| note == Token.NSC
				|| note == Token.CST )
			{
				// Illegal build order. Replace with a waive order
				replacementOrder = unit.SubMessages[0] + Token.WVE;
			}


			if (replacementOrder != null)
			{
				Trace.WriteLineIf(baseBotSwitch.Enabled,
					string.Format(CultureInfo.InvariantCulture,
					ErrorMessages.Basebot_Trace_InvalidOrderReplaced,
                    order.ToString(), replacementOrder.ToString()), traceCategory);

				replacementOrder = Token.SUB & replacementOrder;
                this.connection.Send(replacementOrder);
			}
			else
			{
				Trace.WriteLineIf( baseBotSwitch.Enabled,
					string.Format(CultureInfo.InvariantCulture,
					ErrorMessages.Basebot_Trace_InvalidOrderNotReplaced,
                    order.ToString()), traceCategory);
			}
		}

		/// <summary>Processes a TME message (Indicates the number of seconds until the next deadline).
		/// The default version does nothing.
		/// </summary>
		/// <param name="msg">The TME message.</param>
		protected virtual void ProcessTME(TokenMessage msg)
		{
		}

		/// <summary>Processes an ADM message (an administrative message).
		/// The default version does nothing.
		/// </summary>
		/// <param name="msg">The ADM message.</param>
		protected virtual void ProcessADM(TokenMessage msg)
		{
		}

		/// <summary>Processes a HLO message (the start of the game message).
		/// The default version does nothing.
		/// </summary>
		/// <param name="msg">The HLO message.</param>
		protected virtual void ProcessHLO(TokenMessage msg)
		{
		}

		/// <summary>Processes a MAP message (the name of the map).
		/// The default version does nothing.
		/// </summary>
		/// <param name="msg">The MAP message.</param>
		protected virtual void ProcessMAP(TokenMessage msg)
		{
		}

		/// <summary>Processes a MDF message (map definition).
		/// The default version does nothing.
		/// </summary>
		/// <param name="msg">The MDF message.</param>
		protected virtual void ProcessMDF(TokenMessage msg)
		{
		}

		/// <summary>Processes a NOT CCD message (the specified country is not in civil disorder).
		/// The default version does nothing.
		/// </summary>
		/// <param name="msg">The NOT message.</param>
		/// <param name="messageParameters">The CCD message.</param>
		/// <param name="newReconnection">Set to true if the indicated power is newly reconnected.</param>
		protected virtual void ProcessNOT_CCD(TokenMessage msg, TokenMessage messageParameters, bool newReconnection)
		{
		}

		/// <summary>Processes a NOT TME message (Specifies that the deadline timer has stopped).
		/// The default version does nothing.
		/// </summary>
		/// <param name="msg">The NOT message.</param>
		/// <param name="messageParameters">The TME message.</param>
		protected virtual void ProcessNOT_TME(TokenMessage msg, TokenMessage messageParameters)
		{
		}

		/// <summary>Processes an unexpected NOT message.
		/// The default version sends the message to the log.
		/// </summary>
		/// <param name="msg">The NOT message.</param>
		protected virtual void ProcessNOT_Unexpected(TokenMessage msg)
		{
			Trace.WriteLineIf(baseBotSwitch.Enabled,
				string.Format(CultureInfo.InvariantCulture,
				ErrorMessages.Basebot_Trace_UnexpectedMessage,
                "NOT", msg.ToString()), traceCategory);
			Debug.Assert(true);
		}

		/// <summary>Processes a NOW message (Indicates that a new turn is beginning).
		/// The default version does nothing.
		/// </summary>
		/// <param name="msg">The NOW message.</param>
		protected virtual void ProcessNOW(TokenMessage msg)
		{
		}

		/// <summary>Processes an ORD message (a single order and its results for the indicated turn).
		/// The default version does nothing.
		/// </summary>
		/// <param name="msg">The ORD message.</param>
		protected virtual void ProcessORD(TokenMessage msg)
		{
		}

		/// <summary>Processes a REJ ADM message (a rejected ADM message).
		/// The default version sends the message to the log.
		/// </summary>
		/// <param name="msg">The REJ message.</param>
		/// <param name="rejectMessage">The ADM message.</param>
		protected virtual void ProcessREJ_ADM(TokenMessage msg, TokenMessage rejectMessage)
		{
			Trace.WriteLineIf(baseBotSwitch.Enabled,
				string.Format(CultureInfo.InvariantCulture,
				ErrorMessages.Basebot_Trace_MessageRejected,
                msg.ToString()), traceCategory);
		}

		/// <summary>Processes a REJ DRW message (a rejected DRW message).
		/// The default version sends the message to the log.
		/// </summary>
		/// <param name="msg">The REJ message.</param>
		/// <param name="rejectMessage">The DRW message.</param>
        protected virtual void ProcessREJ_DRW(TokenMessage msg, TokenMessage rejectMessage)
		{
			Trace.WriteLineIf( baseBotSwitch.Enabled,
				string.Format(CultureInfo.InvariantCulture,
				ErrorMessages.Basebot_Trace_MessageRejected,
                msg.ToString()), traceCategory);
		}

		/// <summary>Processes a REJ GOF message (a rejected GOF message).
		/// The default version sends the message to the log.
		/// </summary>
		/// <param name="msg">The REJ message.</param>
		/// <param name="rejectMessage">The GOF message.</param>
        protected virtual void ProcessREJ_GOF(TokenMessage msg, TokenMessage rejectMessage)
		{
			Trace.WriteLineIf( baseBotSwitch.Enabled,
				string.Format(CultureInfo.InvariantCulture,
				ErrorMessages.Basebot_Trace_MessageRejected,
                msg.ToString()), traceCategory);
		}

		/// <summary>Processes a REJ HLO message (a rejected HLO message).
		/// The default version sends the message to the log.
		/// </summary>
		/// <param name="msg">The REJ message.</param>
		/// <param name="rejectMessage">The HLO message.</param>
        protected virtual void ProcessREJ_HLO(TokenMessage msg, TokenMessage rejectMessage)
		{
			Trace.WriteLineIf( baseBotSwitch.Enabled,
				string.Format(CultureInfo.InvariantCulture,
				ErrorMessages.Basebot_Trace_MessageRejected,
                msg.ToString()), traceCategory);
		}

		/// <summary>Processes a REJ HST message (a rejected HST message).
		/// The default version sends the message to the log.
		/// </summary>
		/// <param name="msg">The REJ message.</param>
		/// <param name="rejectMessage">The HST message.</param>
        protected virtual void ProcessREJ_HST(TokenMessage msg, TokenMessage rejectMessage)
		{
			Trace.WriteLineIf( baseBotSwitch.Enabled,
				string.Format(CultureInfo.InvariantCulture,
				ErrorMessages.Basebot_Trace_MessageRejected,
                msg.ToString()), traceCategory);
		}

		/// <summary>Processes a REJ IAM message (a rejected IAM message).
		/// The default version sends the message to the log.
		/// </summary>
		/// <param name="msg">The REJ message.</param>
		/// <param name="rejectMessage">The IAM message.</param>
        protected virtual void ProcessREJ_IAM(TokenMessage msg, TokenMessage rejectMessage)
		{
			Trace.WriteLineIf( baseBotSwitch.Enabled,
				string.Format(CultureInfo.InvariantCulture,
				ErrorMessages.Basebot_Trace_MessageRejected,
                msg.ToString()), traceCategory);
		}

		/// <summary>Processes a REJ NME message (a rejected NME message).
		/// The default version sends the message to the log.
		/// </summary>
		/// <param name="msg">The REJ message.</param>
		/// <param name="rejectMessage">The NME message.</param>
        protected virtual void ProcessREJ_NME(TokenMessage msg, TokenMessage rejectMessage)
		{
			Trace.WriteLineIf( baseBotSwitch.Enabled,
				string.Format(CultureInfo.InvariantCulture,
				ErrorMessages.Basebot_Trace_MessageRejected,
                msg.ToString()), traceCategory);
		}

		/// <summary>Processes a REJ NOT DRW message (a rejected NOT DRW message).
		/// The default version sends the message to the log.
		/// </summary>
		/// <param name="msg">The REJ message.</param>
		/// <param name="rejectMessage">The NOT message.</param>
        protected virtual void ProcessREJ_NOT_DRW(TokenMessage msg, TokenMessage rejectMessage)
		{
			Trace.WriteLineIf( baseBotSwitch.Enabled,
				string.Format(CultureInfo.InvariantCulture,
				ErrorMessages.Basebot_Trace_MessageRejected,
                msg.ToString()), traceCategory);
		}

		/// <summary>Processes a REJ NOT GOF message (a rejected NOT GOF message).
		/// The default version sends the message to the log.
		/// </summary>
		/// <param name="msg">The REJ message.</param>
		/// <param name="rejectMessage">The NOT message.</param>
        protected virtual void ProcessREJ_NOT_GOF(TokenMessage msg, TokenMessage rejectMessage)
		{
			Trace.WriteLineIf( baseBotSwitch.Enabled,
				string.Format(CultureInfo.InvariantCulture,
				ErrorMessages.Basebot_Trace_MessageRejected,
                msg.ToString()), traceCategory);
		}

		/// <summary>Processes an unexpected REJ NOT message.
		/// The default version sends the message to the log.
		/// </summary>
		/// <param name="msg">The REJ message.</param>
		/// <param name="rejectMessage">The NOT message.</param>
        protected virtual void ProcessREJ_NOT_Unexpected(TokenMessage msg, TokenMessage rejectMessage)
		{
			Trace.WriteLineIf( baseBotSwitch.Enabled,
				string.Format(CultureInfo.InvariantCulture,
				ErrorMessages.Basebot_Trace_MessageRejected,
                msg.ToString()), traceCategory);
		}

		/// <summary>Processes a REJ NOW message (a rejected NOW message).
		/// The default version sends the message to the log.
		/// </summary>
		/// <param name="msg">The REJ message.</param>
		/// <param name="rejectMessage">The NOW message.</param>
        protected virtual void ProcessREJ_NOW(TokenMessage msg, TokenMessage rejectMessage)
		{
			Trace.WriteLineIf( baseBotSwitch.Enabled,
				string.Format(CultureInfo.InvariantCulture,
				ErrorMessages.Basebot_Trace_MessageRejected,
                msg.ToString()), traceCategory);
		}

		/// <summary>Processes a REJ ORD message (a rejected ORD message).
		/// The default version sends the message to the log.
		/// </summary>
		/// <param name="msg">The REJ message.</param>
		/// <param name="rejectMessage">The ORD message.</param>
        protected virtual void ProcessREJ_ORD(TokenMessage msg, TokenMessage rejectMessage)
		{
			Trace.WriteLineIf( baseBotSwitch.Enabled,
				string.Format(CultureInfo.InvariantCulture,
				ErrorMessages.Basebot_Trace_MessageRejected,
                msg.ToString()), traceCategory);
		}

		/// <summary>Processes a REJ SCO message (a rejected SCO message).
		/// The default version sends the message to the log.
		/// </summary>
		/// <param name="msg">The REJ message.</param>
		/// <param name="rejectMessage">The SCO message.</param>
        protected virtual void ProcessREJ_SCO(TokenMessage msg, TokenMessage rejectMessage)
		{
			Trace.WriteLineIf( baseBotSwitch.Enabled,
				string.Format(CultureInfo.InvariantCulture,
				ErrorMessages.Basebot_Trace_MessageRejected,
                msg.ToString()), traceCategory);
		}

		/// <summary>Processes a REJ SND message (a rejected SND message).
		/// The default version sends the message to the log.
		/// </summary>
		/// <param name="msg">The REJ message.</param>
		/// <param name="rejectMessage">The SND message.</param>
		protected virtual void ProcessREJ_SND(TokenMessage msg, TokenMessage rejectMessage)
		{
			Trace.WriteLineIf( baseBotSwitch.Enabled,
				string.Format(CultureInfo.InvariantCulture,
				ErrorMessages.Basebot_Trace_MessageRejected,
                msg.ToString()), traceCategory);
		}

		/// <summary>Processes a REJ SUB message (a rejected SUB message).
		/// The default version sends the message to the log.
		/// </summary>
		/// <param name="msg">The REJ message.</param>
		/// <param name="rejectMessage">The SUB message.</param>
        protected virtual void ProcessREJ_SUB(TokenMessage msg, TokenMessage rejectMessage)
		{
			Trace.WriteLineIf( baseBotSwitch.Enabled,
				string.Format(CultureInfo.InvariantCulture,
				ErrorMessages.Basebot_Trace_MessageRejected,
                msg.ToString()), traceCategory);
		}

		/// <summary>Processes a REJ TME message (a rejected TME message).
		/// The default version sends the message to the log.
		/// </summary>
		/// <param name="msg">The REJ message.</param>
		/// <param name="rejectMessage">The TME message.</param>
        protected virtual void ProcessREJ_TME(TokenMessage msg, TokenMessage rejectMessage)
		{
			Trace.WriteLineIf( baseBotSwitch.Enabled,
				string.Format(CultureInfo.InvariantCulture,
				ErrorMessages.Basebot_Trace_MessageRejected,
                msg.ToString()), traceCategory);
		}
	
		/// <summary>Processes an unexpected REJ message.
		/// The default version sends the message to the log.
		/// </summary>
		/// <param name="msg">The REJ message.</param>
		/// <param name="rejectMessage">The message that was unexpected.</param>
        protected virtual void ProcessREJ_Unexpected(TokenMessage msg, TokenMessage rejectMessage)
		{
			Trace.WriteLineIf( baseBotSwitch.Enabled,
				string.Format(CultureInfo.InvariantCulture,
				ErrorMessages.Basebot_Trace_UnexpectedMessage,
                "REJ", msg.ToString()), traceCategory);
			Debug.Assert(false);
		}

		/// <summary>Processes a SCO message (Supply center ownership).
		/// The default version does nothing.
		/// </summary>
		/// <param name="msg">The SCO message.</param>
		protected virtual void ProcessSCO(TokenMessage msg)
		{
		}

		/// <summary>Processes a YES DRW message (a confirmation of a DRW message).
		/// The default version does nothing.
		/// </summary>
		/// <param name="msg">The YES message.</param>
		/// <param name="confirmedMessage">The DRW message.</param>
        protected virtual void ProcessYES_DRW(TokenMessage msg, TokenMessage confirmedMessage)
        {
		}

		/// <summary>Processes a YES GOF message (a confirmation of a GOF message).
		/// The default version does nothing.
		/// </summary>
		/// <param name="msg">The YES message.</param>
		/// <param name="confirmedMessage">The GOF message.</param>
		protected virtual void ProcessYES_GOF(TokenMessage msg, TokenMessage confirmedMessage)
		{
		}

		/// <summary>Processes a YES IAM message (a confirmation of a IAM message).
		/// The default version does nothing.
		/// </summary>
		/// <param name="msg">The YES message.</param>
		/// <param name="confirmedMessage">The IAM message.</param>
		protected virtual void ProcessYES_IAM(TokenMessage msg, TokenMessage confirmedMessage)
		{
		}

        /// <summary>Processes a YES NME message (a confirmation of a NME message).
		/// The default version does nothing.
		/// </summary>
		/// <param name="msg">The YES message.</param>
		/// <param name="confirmedMessage">The NME message.</param>
		protected virtual void ProcessYES_NME(TokenMessage msg, TokenMessage confirmedMessage)
		{
		}

		/// <summary>Processes a YES NOT message (a confirmation of a NOT message).
		/// The default version does nothing.
		/// </summary>
		/// <param name="msg">The YES message.</param>
		/// <param name="confirmedMessage">The NOT message.</param>
		protected virtual void ProcessYES_NOT(TokenMessage msg, TokenMessage confirmedMessage)
		{
		}

		/// <summary>Processes a YES OBS message (a confirmation of a OBS message).
		/// The default version does nothing.
		/// </summary>
		/// <param name="msg">The YES message.</param>
		/// <param name="confirmedMessage">The OBS message.</param>
		protected virtual void ProcessYES_OBS(TokenMessage msg, TokenMessage confirmedMessage)
		{
		}

		/// <summary>Processes a YES SND message (a confirmation of a SND message).
		/// The default version does nothing.
		/// </summary>
		/// <param name="msg">The YES message.</param>
		/// <param name="confirmedMessage">The SND message.</param>
		protected virtual void ProcessYES_SND(TokenMessage msg, TokenMessage confirmedMessage)
		{
		}

		/// <summary>Processes a YES TME message (a confirmation of a TME message).
		/// The default version does nothing.
		/// </summary>
		/// <param name="msg">The YES message.</param>
		/// <param name="confirmedMessage">The TME message.</param>
		protected virtual void ProcessYES_TME(TokenMessage msg, TokenMessage confirmedMessage)
		{
		}

		/// <summary>Processes an unexpected YES message.
		/// The default version sends the message to the log.
		/// </summary>
		/// <param name="msg">The YES message.</param>
		/// <param name="confirmedMessage">The message that was unexpected.</param>
		protected virtual void ProcessYES_Unexpected(TokenMessage msg, TokenMessage confirmedMessage)
		{
			Trace.WriteLineIf(baseBotSwitch.Enabled,
				string.Format(CultureInfo.InvariantCulture,
				ErrorMessages.Basebot_Trace_UnexpectedMessage,
                "YES", msg.ToString()), traceCategory);
        }

        #endregion
    }
}