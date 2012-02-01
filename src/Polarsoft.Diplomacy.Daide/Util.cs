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
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Reflection;
using ErrorMessages = Polarsoft.Diplomacy.Daide.Properties.ErrorMessages;

namespace Polarsoft.Diplomacy.Daide
{
	internal static class Util
	{
        #region Instrumentation

        const string traceCategory = "DAIDE::Connector::Util";
		static BooleanSwitch mySwitch =
			new BooleanSwitch(traceCategory, "Trace messages from the DAIDE Connector Utilities");

        #endregion

        #region Public methods

        public static void CreateMapFromMDF(TokenMessage msg, Game game)
		{
			TokenMessage msgProvinces = msg.SubMessages[2];
			TokenMessage msgSupplyCenterProvinces = msgProvinces.SubMessages[0];
			foreach(TokenMessage message in msgSupplyCenterProvinces.SubMessages)
			{
				for (int index = 1; index < message.SubMessages.Count; ++index)
				{
					Province province = GetProvince(message.SubMessages[index].Tokens[0], game);
					foreach(Token powerToken in message.SubMessages[0].Tokens)
					{
						Power power = GetPower(powerToken, game);
						if (power != null)
						{
							power.HomeProvinces.Add(province);
						}
					}
				}
			}
			//Adjacencies
			TokenMessage msgAdjacencies = msg.SubMessages[3];
			foreach(TokenMessage msgAdjacency in msgAdjacencies.SubMessages)
			{
				Province fromProvince = game.Map.Provinces[msgAdjacency.Tokens[0].ToString()];
				for (int index = 1; index < msgAdjacency.SubMessages.Count; ++index)
				{
					TokenMessage msgUnitAndAdj = msgAdjacency.SubMessages[index];
                    Location fromLocation;
                    UnitType unitType;
                    if (msgUnitAndAdj.SubMessages.Count == 0)
                    {
                        fromLocation = GetOrCreateLocation(fromProvince, msgUnitAndAdj);
                        unitType = GetUnitType(msgUnitAndAdj.Tokens[0]);
                    }
                    else
                    {
                        fromLocation = GetOrCreateLocation(fromProvince, msgUnitAndAdj.SubMessages[0]);
                        unitType = GetUnitType(msgUnitAndAdj.SubMessages[0].Tokens[0]);
                    }
					for (int ii = 1; ii < msgUnitAndAdj.SubMessages.Count; ++ii)
					{
						Location toLocation = GetOrCreateLocation(unitType, msgUnitAndAdj.SubMessages[ii], game);
						Trace.WriteLineIf(mySwitch.Enabled,
							string.Format(CultureInfo.InvariantCulture,
                            ErrorMessages.Util_Trace_NewAdjacency,
							fromLocation.ToString(), toLocation.ToString()),
								traceCategory);
						fromLocation.AdjacentLocations.Add(toLocation);
					}
				}
			}
			game.Map.FinalSetup();
        }

        public static void UpdateGameFromNOW(TokenMessage msg, Game game)
        {
            Debug.Assert(msg.Tokens[0] == Token.NOW);
            //Turn
            TokenMessage msgTurn = msg.SubMessages[1];
            Debug.Assert(msgTurn.Tokens[1].Type == TokenType.Number);
            game.Turn.SetCurrentTurn(GetPhase(msgTurn.Tokens[0]), msgTurn.Tokens[1].ToNumber());
            //Units

            //Clear out previous units from the map and the powers
            foreach( Power power in game.Powers.Values )
            {
                power.Units.Clear();
            }

            foreach( Province province in game.Map.Provinces.Values )
            {
                province.Unit = null;
            }

            //Set new units
            for( int index = 2; index < msg.SubMessages.Count; ++index )
            {
                TokenMessage msgUnit = msg.SubMessages[index];
                Power power = GetPower(msgUnit.Tokens[0], game);
                UnitType unitType = GetUnitType(msgUnit.Tokens[1]);
                Location location = GetLocation(unitType, msgUnit.SubMessages[2], game);
                Unit unit = new Unit(unitType, power, location);
                if( msgUnit.SubMessages.Count > 3 )
                {
                    Debug.Assert(msgUnit.SubMessages.Count == 5);
                    Debug.Assert(msgUnit.SubMessages[3].Tokens[0] == Token.MRT);
                    foreach( TokenMessage msgRetreatLocation in msgUnit.SubMessages[4].SubMessages )
                    {
                        unit.RetreatLocations.Add(GetLocation(unitType, msgRetreatLocation, game));
                    }
                    unit.MustRetreat = true;
                }
                else
                {
                    unit.Province.Unit = unit;
                }
                power.Units.Add(unit);
            }
        }

        //public static void UpdateGameFromORD(TokenMessage msg, Game game)
        //{
        //    //TODO
        //}

        public static void UpdateGameFromSCO(TokenMessage msg, Game game)
        {
            // Make sure that all powers' Owned Supply Provinces are cleared.
            foreach( Power power in game.Powers.Values )
            {
                foreach( Province province in power.OwnedSupplyProvinces )
                {
                    province.OwningPower = null;
                }
                power.OwnedSupplyProvinces.Clear();
            }

            Debug.Assert(msg.Tokens[0] == Token.SCO);
            //Debug.Assert(msg.SubMessages.Count == game.Powers.Count + 2);
            for( int index = 1; index < msg.SubMessages.Count; ++index )
            {
                TokenMessage msgPower = msg.SubMessages[index];
                Power power = GetPower(msgPower.Tokens[0], game);
                if( power != null )
                {
                    for( int provinceIndex = 1; provinceIndex < msgPower.Tokens.Count; ++provinceIndex )
                    {
                        Province province = GetProvince(msgPower.Tokens[provinceIndex], game);
                        province.OwningPower = power;
                        power.OwnedSupplyProvinces.Add(province);
                    }
                }
            }
        }

        public static TokenMessage ToOrderFormat(Power power)
        {
            Token token = TokenFactory.FromMnemonic(power.Name);
            Debug.Assert(token.Type == TokenType.Power);
            return new TokenMessage(token);

        }

        public static TokenMessage ToOrderFormat(Province province)
        {
            Token token = TokenFactory.FromMnemonic(province.Name);
            Debug.Assert(IsProvinceToken(token));
            return new TokenMessage(token);
        }

        public static TokenMessage ToOrderFormat(Location location)
        {
            TokenMessage msg = ToOrderFormat(location.Province);
            if( location.Coast != Coast.NoCoast )
            {
                Token coast = ToOrderFormat(location.Coast);
                Debug.Assert(coast.Type == TokenType.Coast);
                msg.Add(coast);
                return msg.MessageEnclosed;
            }
            else
            {
                return msg;
            }
        }

        public static Token ToOrderFormat(Coast coast)
        {
            switch( coast )
            {
                case Coast.NoCoast:
                    return null;
                case Coast.North:
                    return Token.NCS;
                case Coast.Northeast:
                    return Token.NEC;
                case Coast.East:
                    return Token.ECS;
                case Coast.Southeast:
                    return Token.SEC;
                case Coast.South:
                    return Token.SCS;
                case Coast.Southwest:
                    return Token.SWS;
                case Coast.West:
                    return Token.WCS;
                case Coast.Northwest:
                    return Token.NWC;
                default:
                    Trace.WriteLineIf(mySwitch.Enabled,
                        string.Format(CultureInfo.InvariantCulture,
                        ErrorMessages.Util_UnknownCoast),
                        traceCategory);
                    Debug.Assert(false);
                    return null;
            }
        }

        public static TokenMessage ToOrderFormat(Unit unit)
        {
            TokenMessage msg = ToOrderFormat(unit.Power);
            switch( unit.UnitType )
            {
                case UnitType.Army:
                    msg.Add(Token.ARMY);
                    break;
                case UnitType.Fleet:
                    msg.Add(Token.FLEET);
                    break;
            }
            msg.Add(ToOrderFormat(unit.Location));
            return msg;
        }

        #endregion

        #region Implementation

        private static Location GetOrCreateLocation(Province province, TokenMessage message)
		{
			UnitType unitType = GetUnitType(message.Tokens[0]);
			Coast coast = Coast.NoCoast;
			if (message.Tokens.Count > 1)
			{
				coast = GetCoast(message.Tokens[1]);
			}
			Location location = province.GetLocation(unitType, coast);
			if (location == null)
			{
				location = new Location(province, unitType, coast);
				province.Locations.Add(location);
			}
			return location;
		}

		private static Location GetOrCreateLocation(UnitType unitType, TokenMessage message, Game game)
		{
			Province province = GetProvince(message.Tokens[0], game);
			Coast coast = Coast.NoCoast;
			if (message.Tokens.Count > 1)
			{
				coast = GetCoast(message.Tokens[1]);
			}
			Location location = province.GetLocation(unitType, coast);
			if (location == null)
			{
				location = new Location(province, unitType, coast);
				province.Locations.Add(location);
			}
			return location;
		}

		private static Location GetLocation(UnitType unitType, TokenMessage message, Game game)
		{
			Province province = GetProvince(message.Tokens[0], game);
			if (message.Tokens.Count > 1)
			{
				return province.GetLocation(unitType, GetCoast(message.Tokens[1]));
			}
			else
			{
				return province.GetLocation(unitType, Coast.NoCoast);
			}
		}

		private static bool IsProvinceToken(Token token)
		{
			return token.Type == TokenType.ProvinceBicoastalNonSupplyCenter
				|| token.Type == TokenType.ProvinceBicoastalSupplyCenter
				|| token.Type == TokenType.ProvinceCoastalNonSupplyCenter
				|| token.Type == TokenType.ProvinceCoastalSupplyCenter
				|| token.Type == TokenType.ProvinceInlandNonSupplyCenter
				|| token.Type == TokenType.ProvinceInlandSupplyCenter
				|| token.Type == TokenType.ProvinceSeaNonSupplyCenter
				|| token.Type == TokenType.ProvinceSeaSupplyCenter;
		}

		private static Province GetProvince(Token token, Game game)
		{
			Debug.Assert(IsProvinceToken(token));
			return game.Map.Provinces[token.ToString()];
		}

		private static Coast GetCoast(Token token)
		{
			Debug.Assert(token.Type == TokenType.Coast);
			if (token == Token.NCS)
			{
				return Coast.North;
			}
			else if (token == Token.NEC)
			{
				return Coast.Northeast;
			}
			else if (token == Token.ECS)
			{
				return Coast.East;
			}
			else if (token == Token.SEC)
			{
				return Coast.Southeast;
			}
			else if (token == Token.SCS)
			{
				return Coast.South;
			}
			else if (token == Token.SWS)
			{
				return Coast.Southwest;
			}
			else if (token == Token.WCS)
			{
				return Coast.West;
			}
			else //if (token == Token.NWC)
			{
				return Coast.Northwest;
			}
		}

		private static Power GetPower(Token token, Game game)
		{
			Debug.Assert( token.Type == TokenType.Power || token == Token.UNO);
			if (token == Token.UNO)
			{
				return null;
			}
			else
			{
				return game.Powers[token.ToString()];
			}
		}

		private static UnitType GetUnitType(Token token)
		{
			Debug.Assert(token.Type == TokenType.UnitType);
			if (token == Token.ARMY)
			{
				return UnitType.Army;
			}
			else
			{
				return UnitType.Fleet;
			}
		}

		private static Phase GetPhase(Token token)
		{
			Debug.Assert(token.Type == TokenType.Phase);
			if (token == Token.SPR)
			{
				return Phase.Spring;
			}
			else if (token == Token.SUM)
			{
				return Phase.Summer;
			}
			else if (token == Token.FAL)
			{
				return Phase.Fall;
			}
			else if (token == Token.AUT)
			{
				return Phase.Autumn;
			}
			else if (token == Token.WIN)
			{
				return Phase.Winter;
			}
			else
			{
				throw new ArgumentException
					(
					string.Format(CultureInfo.InvariantCulture,
					ErrorMessages.Util_Exception_UnknownPhase, token.ToString()),
					"token");
			}
        }

        #endregion
	}
}
