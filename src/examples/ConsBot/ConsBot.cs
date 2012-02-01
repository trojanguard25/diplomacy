using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Polarsoft.Diplomacy;
using Polarsoft.Diplomacy.Orders;
using Polarsoft.Diplomacy.Daide;

namespace Polarsoft.Diplomacy.AI
{
    class ConsBot : BaseBot
    {
        Form parent;

        public ConsBot(string host, int port, Form parent )
            : base()
        {
            this.parent = parent;
            base.Connect(host, port, "Polarsoft.se C# Demo ConsBot", "1.1");
        }

        protected override void ProcessNOW(TokenMessage msg)
        {
            switch( this.Game.Turn.Phase )
            {
                case Phase.Spring:
                    OrderMoves();
                    break;
                case Phase.Summer:
                    OrderRetreats();
                    break;
                case Phase.Fall:
                    OrderMoves();
                    break;
                case Phase.Autumn:
                    OrderRetreats();
                    break;
                case Phase.Winter:
                    OrderBuildsAndDisbands();
                    break;
            }
            SubmitOrders();
        }

        private void OrderMoves()
        {
            Dictionary<Unit, List<UnitOrderMetaData>> allOrders = GetAllPossibleMoves();

            List<Unit> orderedUnits = new List<Unit>();
            //Find moves for all my units!
            Random rnd = new Random();
            foreach( Unit unit in Power.Units )
            {
                List<UnitOrderMetaData> possibleOrders = allOrders[unit];
                while( !orderedUnits.Contains(unit) )
                {
                    UnitOrderMetaData orderMeta = possibleOrders[rnd.Next(possibleOrders.Count)];
                    bool bValidOrder = true;
                    foreach( UnitOrderMetaData requiredMeta in orderMeta.RequiredOrders )
                    {
                        //Only select orders that involve friendly units!
                        if( requiredMeta.Order.Unit.Power != Power )
                        {
                            bValidOrder = false;
                            break;
                        }

                        //Only select orders that involve units that haven't been ordered!
                        if( orderedUnits.Contains(requiredMeta.Order.Unit) )
                        {
                            bValidOrder = false;
                            break;
                        }
                    }


                    if( !bValidOrder )
                    {
                        //Try the next one!
                        continue;
                    }
                    else
                    {
                        //Add this order and all dependent orders!
                        CurrentOrders.Add(orderMeta.Order);
                        orderedUnits.Add(orderMeta.Order.Unit);
                        foreach( UnitOrderMetaData requiredMeta in orderMeta.RequiredOrders )
                        {
                            CurrentOrders.Add(requiredMeta.Order);
                            orderedUnits.Add(requiredMeta.Order.Unit);
                        }
                    }
                }
            }
        }

        private Dictionary<Unit, List<UnitOrderMetaData>> GetAllPossibleMoves()
        {
            Dictionary<Unit, List<UnitOrderMetaData>> allOrders = new Dictionary<Unit, List<UnitOrderMetaData>>();
            //Create an orderlist for each unit.
            foreach( Power power in Game.Powers.Values )
            {
                foreach( Unit unit in power.Units )
                {
                    allOrders.Add(unit, new List<UnitOrderMetaData>());
                }
            }

            //Iterate over all the units and add their orders.
            foreach( Unit unit in allOrders.Keys )
            {
                GetPossibleMovesForUnit(unit, allOrders);
            }
            return allOrders;
        }

        private void GetPossibleMovesForUnit(Unit unit, Dictionary<Unit, List<UnitOrderMetaData>> allOrders)
        {
            List<UnitOrderMetaData> orders = allOrders[unit];

            //The hold order!
            orders.Add(new UnitOrderMetaData(new HoldOrder(unit)));

            //All possible move orders
            foreach( Location location in unit.Location.AdjacentLocations )
            {
                orders.Add(new UnitOrderMetaData(new MoveOrder(unit, location)));
            }

            //Support orders
            PopulateSupportOrders(unit, allOrders);

            //Convoy orders
            PopulateMoveByConvoyOrders(unit, allOrders);
        }

        private void PopulateSupportOrders(Unit unit, Dictionary<Unit, List<UnitOrderMetaData>> allOrders)
        {
            List<UnitOrderMetaData> orders = allOrders[unit];

            foreach( Location adjacentLocation in unit.Location.AdjacentLocations )
            {
                //Support of a unit one step away
                Unit adjUnit = adjacentLocation.Province.Unit;
                if( adjUnit != null && adjUnit.Power == this.Power )
                {
                    //Support for the adjacent unit's Hold order
                    {
                        UnitOrderMetaData orderMeta = new UnitOrderMetaData(
                            new SupportHoldOrder(unit, adjUnit));

                        orderMeta.RequiredOrders.Add(new UnitOrderMetaData(
                            new HoldOrder(adjUnit)));
                        orders.Add(orderMeta);
                    }

                    //Support for the adjacent unit's Move orders
                    foreach( Location targetLocation in adjUnit.Location.AdjacentLocations )
                    {
                        if( targetLocation.Province != unit.Province
                            && unit.CanMoveTo(targetLocation.Province) )
                        {
                            UnitOrderMetaData orderMeta = new UnitOrderMetaData(
                                new SupportMoveOrder(unit, adjUnit, targetLocation.Province));
                            orderMeta.RequiredOrders.Add(new UnitOrderMetaData(
                                new MoveOrder(adjUnit, targetLocation)));
                            orders.Add(orderMeta);
                        }
                    }
                }

                //Support of a unit two steps away
                foreach( Province farProvince in adjacentLocation.AdjacentProvinces )
                {
                    if( unit.Location.AdjacentProvinces.Contains(farProvince)	//It is an adjacent province.
                        || farProvince == unit.Province )	//It is this province
                    {
                        continue;
                    }

                    Unit farUnit = farProvince.Unit;
                    if( farUnit != null	//There is a unit there
                        && farUnit.CanMoveTo(adjacentLocation.Province) )
                    {
                        //Now, it might be possible for the farUnit to be able to move
                        //to the adjacentProvince in more than one way!
                        foreach( Location closeLocation in farUnit.Location.AdjacentLocations )
                        {
                            if( closeLocation.Province == adjacentLocation.Province )
                            {
                                UnitOrderMetaData orderMeta = new UnitOrderMetaData(
                                    new SupportMoveOrder(unit, farUnit, adjacentLocation.Province));
                                orderMeta.RequiredOrders.Add(new UnitOrderMetaData(
                                    new MoveOrder(farUnit, closeLocation)));
                                orders.Add(orderMeta);
                            }
                        }
                    }
                }
            }
        }

        private void PopulateMoveByConvoyOrders(Unit unit, Dictionary<Unit, List<UnitOrderMetaData>> allOrders)
        {
            List<UnitOrderMetaData> orders = allOrders[unit];

            if( unit.UnitType != UnitType.Army
                || !unit.Province.IsCoastal )
            {
                return;
            }

            //We've found an army that is placed in a coastal territory!

            //Find all sea-provinces that have fleets in them
            Route route = new Route(unit.Province);
            Queue<Route> routesToSearch = new Queue<Route>();
            foreach( Province province in unit.Location.AdjacentProvinces )
            {
                routesToSearch.Enqueue(new Route(route, province));
            }

            while( routesToSearch.Count > 0 )
            {
                Route aRoute = (Route)routesToSearch.Dequeue();
                if( aRoute.End.IsSea	//It's a sea province
                    && aRoute.End.Unit != null )	//There's a fleet there
                {
                    foreach( Province adjacentProvince in aRoute.End.AdjacentProvinces )
                    {
                        //Make sure that we do not end up in a never-ending loop.
                        if( !aRoute.Provinces.Contains(adjacentProvince) )
                        {
                            routesToSearch.Enqueue(new Route(aRoute, adjacentProvince));
                        }
                    }
                }
                else if( aRoute.End.IsCoastal	//A coastal territory
                    && aRoute.Provinces.Count >= 3 )	//We've gone at least one step on water!
                {
                    UnitOrderMetaData orderMeta = new UnitOrderMetaData(
                        new MoveByConvoyOrder(unit, aRoute));
                    Route via = aRoute.Via;

                    List<UnitOrderMetaData> conveyOrders = new List<UnitOrderMetaData>();
                    for( int index = 0; index < via.Provinces.Count; ++index )
                    {
                        Unit convoyingUnit = via.Provinces[index].Unit;

                        UnitOrderMetaData conveyMeta = new UnitOrderMetaData(
                            new ConveyOrder(convoyingUnit, unit, aRoute.End));

                        //Schedule the convoy order for updates regarding it's dependent orders.
                        conveyOrders.Add(conveyMeta);

                        //Add the convoy order to the conveying unit.
                        //This means that we do not have to look at fleet to add their
                        //convoy orders, since they've been added for all convoyed armies!
                        List<UnitOrderMetaData> convoyUnitOrders = allOrders[convoyingUnit];
                        convoyUnitOrders.Add(conveyMeta);

                        orderMeta.RequiredOrders.Add(conveyMeta);
                    }

                    //Finalize the convey orders.
                    foreach( UnitOrderMetaData conveyOrder in conveyOrders )
                    {
                        conveyOrder.RequiredOrders.Add(orderMeta);
                        conveyOrder.RequiredOrders.AddRange(conveyOrders);
                        conveyOrder.RequiredOrders.Remove(conveyOrder);	//Remove yourself from the list!
                    }

                    orders.Add(orderMeta);
                }
            }
        }

        private void OrderRetreats()
        {
            foreach( Unit unit in this.Power.Units )
            {
                if( unit.MustRetreat )
                {
                    if( unit.RetreatLocations.Count > 0 )
                    {
                        Random rnd = new Random();
                        CurrentOrders.Add(new RetreatOrder(unit,
                            unit.RetreatLocations[rnd.Next(unit.RetreatLocations.Count)]));
                    }
                    else
                    {
                        CurrentOrders.Add(new DisbandOrder(unit));
                    }
                }
            }
        }

        private void OrderBuildsAndDisbands()
        {
            int iBuilds = this.Power.OwnedSupplyProvinces.Count - this.Power.Units.Count;
            if( iBuilds > 0 )
            {
                List<Province> availableBuildSites = new List<Province>();
                foreach( Province supplyProvince in Power.HomeProvinces )
                {
                    if( supplyProvince.CanBuild(this.Power))
                    {
                        availableBuildSites.Add(supplyProvince);
                    }
                }
                //If we can't build - order waives
                for( ; iBuilds > availableBuildSites.Count; --iBuilds )
                {
                    CurrentOrders.Add(new WaiveBuildOrder(this.Power));
                }
                //For all others, order a build order
                for( int iCounter = 0; iCounter < iBuilds; ++iCounter )
                {
                    Random rnd = new Random();
                    Province buildProvince = availableBuildSites[rnd.Next(availableBuildSites.Count)];
                    while( availableBuildSites.Contains(buildProvince) )
                    {
                        UnitType unitType;
                        if( rnd.Next(2) == 1 )
                        {
                            unitType = UnitType.Army;
                        }
                        else
                        {
                            unitType = UnitType.Fleet;
                        }

                        if( buildProvince.CanBuildUnitType(this.Power, unitType) )
                        {
                            //Find a location to build the specified unitType
                            while( true )
                            {
                                Location buildLocation = buildProvince.Locations[rnd.Next(buildProvince.Locations.Count)];
                                if( buildLocation.UnitType == unitType )
                                {
                                    CurrentOrders.Add(new BuildOrder(
                                        new Unit(unitType, Power, buildLocation)));
                                    availableBuildSites.Remove(buildProvince);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else if( iBuilds < 0 )
            {
                for( int index = 0; index < Math.Abs(iBuilds); ++index )
                {
                    CurrentOrders.Add(new RemoveOrder(this.Power.Units[index]));
                }
            }
            else
            {
                //Same number - no builds or disbands need to be done!
            }
        }

        delegate void VoidDelegate();
        protected override void ProcessOFF(TokenMessage msg)
        {
            base.ProcessOFF(msg);
            this.parent.Invoke( new VoidDelegate( delegate
            {
                this.parent.Close();
            }));
        }
    }
}
