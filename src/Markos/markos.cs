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
    }
}
