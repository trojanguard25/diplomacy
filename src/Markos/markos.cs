using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Polarsoft.Diplomacy;
using Polarsoft.Diplomacy.Orders;
using Polarsoft.Diplomacy.Daide;

namespace Polarsoft.Diplomacy.AI
{
    class markos : BaseBot
    {
        Form parent;

        public markos(string host, int port, Form parent)
            : base()
        {
            this.parent = parent;
            base.Connect(host, port, "Markos", "0.1");
        }

        protected override void ProcessNOW(TokenMessage msg)
        {
            switch (this.Game.Turn.Phase)
            {
                case Phase.Spring:
                    randomOrders();
                    break;
                case Phase.Summer:
                    randomRetreat();
                    break;
                case Phase.Fall:
                    randomOrders();
                    break;
                case Phase.Autumn:
                    randomRetreat();
                    break;
                case Phase.Winter:
                    randomBuild();
                    break;
                default:
                    break;
            }
            SubmitOrders();
        }

        public void randomOrders()
        {
            foreach (Unit unit in this.Power.Units)
            {
                LocationCollection adjacentLocations = unit.Location.AdjacentLocations;
                LocationCollection unitLocations = new LocationCollection();

                foreach (Location loc in adjacentLocations)
                {
                    if (loc.Province.FleetMoveable && unit.UnitType == UnitType.Fleet)
                    {
                        unitLocations.Add(loc);
                    }
                    else if (loc.Province.ArmyMoveable && unit.UnitType == UnitType.Army)
                    {
                        unitLocations.Add(loc);
                    }
                }

                if (unitLocations.Count > 0)
                {
                    Random rand = new Random();
                    int index = rand.Next(unitLocations.Count);
                    CurrentOrders.Add(new MoveOrder(unit, unitLocations[index]));
                }
                else
                {
                    CurrentOrders.Add(new HoldOrder(unit));
                }
            }
        }

        public void randomRetreat()
        {
            foreach (Unit unit in this.Power.Units)
            {
                if (unit.MustRetreat && unit.RetreatLocations.Count > 0)
                {

                    CurrentOrders.Add(new RetreatOrder(unit, unit.RetreatLocations.ElementAt(0)));
                }
                else
                {
                    CurrentOrders.Add(new DisbandOrder(unit));
                }
            }
        }

        public void randomBuild()
        {
            int numBuilds = this.Power.OwnedSupplyProvinces.Count - this.Power.Units.Count;
            if (numBuilds > 0)
            {
                int built = 0;
                for (int i = 0; i < this.Power.HomeProvinces.Count; i++)
                {
                    Province prov = this.Power.HomeProvinces[i];
                    if (prov.CanBuildFleet(this.Power) && prov.IsOpenForBuilding)
                    {
                        foreach (Location loc in prov.Locations)
                        {
                            if (loc.UnitType == UnitType.Fleet)
                            {
                                CurrentOrders.Add(new BuildOrder(new Unit(UnitType.Fleet, this.Power, loc)));
                                built++;
                                break;
                            }
                        }
                    }
                    else if (prov.CanBuildArmy(this.Power) && prov.IsOpenForBuilding)
                    {
                        foreach (Location loc in prov.Locations)
                        {
                            if (loc.UnitType == UnitType.Army)
                            {
                                CurrentOrders.Add(new BuildOrder(new Unit(UnitType.Army, this.Power, loc)));
                                built++;
                                break;
                            }
                        }
                    }
                    if (built == numBuilds)
                        break;
                }
            }
            else if (numBuilds < 0)
            {
                for (int i = 0; i < Math.Abs(numBuilds); i++)
                {
                    CurrentOrders.Add(new RemoveOrder(this.Power.Units[i]));
                }
            }
        }

        delegate void VoidDelegate();

        protected override void ProcessOFF(TokenMessage msg)
        {
            base.ProcessOFF(msg);
            this.parent.Invoke(new VoidDelegate(delegate
            {
                this.parent.Close();
            }));

        }
    }
}
