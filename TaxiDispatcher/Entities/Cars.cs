using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiDispatcher.Entities
{
    public class Cars
    {
        public virtual int CarID { get; protected set; }
        public virtual CarTypes CarType { get; set; }
        public virtual string CarColor { get; set; }
        public virtual int CarNumber { get; set; }
    }
}
