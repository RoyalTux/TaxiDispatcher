using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiDispatcher.Entities
{
    public class Drivers
    {
        public virtual int DriverID { get; protected set; }
        public virtual string DriverName { get; set; }
        public virtual Cars Car { get; set; }
        public virtual bool Available { get; set; }
    }
}
