using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiDispatcher.Entities
{
    public class DriversView
    {
        public virtual int DriverID { get; set; }
        public virtual string DriverName { get; set; }
        public virtual string CarTypeName { get; set; }
        public virtual string CarColor { get; set; }
        public virtual int CarNumber { get; set; }
        public virtual bool Available { get; set; }
    }
}
