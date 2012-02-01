using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Polarsoft.Diplomacy;
using Polarsoft.Diplomacy.Orders;
using Polarsoft.Diplomacy.Daide;

namespace Polarsoft.Diplomacy.AI
{
    class HeadlessHoldBot : BaseBot
    {
        public HeadlessHoldBot(string host, int port)
            : base()
        {
            base.Connect(host, port, "Polarsoft.se C# Demo HeadlessHoldBot", "1.1");
        }

        protected override void ProcessNOW(TokenMessage msg)
        {
            switch( this.Game.Turn.Phase )
            {
                case Phase.Spring:
                    OrderAllHold();
                    break;
                case Phase.Summer:
                    OrderDisbandAllRetreats();
                    break;
                case Phase.Fall:
                    OrderAllHold();
                    break;
                case Phase.Autumn:
                    OrderDisbandAllRetreats();
                    break;
                case Phase.Winter:
                    OrderDisbandsAndWaiveAllBuilds();
                    break;
            }
            SubmitOrders();
        }

        private void OrderAllHold()
        {
            foreach( Unit unit in this.Power.Units )
            {
                CurrentOrders.Add(new HoldOrder(unit));
            }
        }

        private void OrderDisbandAllRetreats()
        {
            foreach( Unit unit in this.Power.Units )
            {
                if( unit.MustRetreat )
                {
                    CurrentOrders.Add(new DisbandOrder(unit));
                }
            }
        }

        private void OrderDisbandsAndWaiveAllBuilds()
        {
            int iBuilds = this.Power.OwnedSupplyProvinces.Count - this.Power.Units.Count;
            if( iBuilds > 0 )
            {
                for( int iCounter = 0; iCounter < iBuilds; ++iCounter )
                {
                    CurrentOrders.Add(new WaiveBuildOrder(this.Power));
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

        protected override void ProcessOFF(TokenMessage msg)
        {
            base.ProcessOFF(msg);
            Application.Exit();
        }
    }
}
