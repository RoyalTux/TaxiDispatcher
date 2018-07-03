using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiDispatcher.Entities
{
    public class CarTypes
    {
        public virtual int CarTypeID { get; protected set; }
        public virtual string CarTypeName { get; set; }
    }
}
