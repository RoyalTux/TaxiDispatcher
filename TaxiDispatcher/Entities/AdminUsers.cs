using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiDispatcher.Entities
{
    public class AdminUsers
    {
        public virtual int id { get; protected set; }
        public virtual string userName { get; set; }
        public virtual string userPassword { get; set; }
    }
}
