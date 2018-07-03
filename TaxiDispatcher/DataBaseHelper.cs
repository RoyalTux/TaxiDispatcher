using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiDispatcher
{
    class DataBaseHelper
    {
        private static readonly IDataBase CurrentDataBase = new DataBaseServer();

        public static IDataBase GetCurrentDataBase()
        {
            return CurrentDataBase;
        }
    }
}
