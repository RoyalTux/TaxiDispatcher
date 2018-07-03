using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiDispatcher.Entities
{
    public class Orders
    {
        public virtual int OrderID { get; protected set; }
        public virtual int OrderTrackingNumber { get; set; }
        public virtual string ClientName { get; set; }
        public virtual string Destination_From { get; set; }
        public virtual string Destination_To { get; set; }
        public virtual string PreferredCarTypeName { get; set; }
        public virtual bool OnlyThisCarType { get; set; }
        public virtual Drivers AssignedDriver { get; set; }
        public virtual int OrderStatus { get; set; }
    }
}
