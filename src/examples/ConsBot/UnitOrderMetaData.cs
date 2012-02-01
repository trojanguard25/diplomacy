using System;
using System.Collections.Generic;
using Polarsoft.Diplomacy.Orders;

namespace Polarsoft.Diplomacy.AI
{
	public class UnitOrderMetaData
	{
		private UnitOrder order;
        private List<UnitOrderMetaData> requiredOrders;

		public UnitOrderMetaData(UnitOrder order)
		{
            this.requiredOrders = new List<UnitOrderMetaData>();
			this.order = order;
			this.order.Tag = this;
		}

		public List<UnitOrderMetaData> RequiredOrders
		{
			get
			{
				return this.requiredOrders;
			}
		}

		public UnitOrder Order
		{
			get
			{
				return this.order;
			}
		}
	}
}
