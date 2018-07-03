using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace TaxiDispatcher
{
    class NhibernateHelper
    {
        private static readonly ISessionFactory TaxiOneSessionFactory;
        private static readonly ISessionFactory TaxiTwoSessionFactory;

        static NhibernateHelper()
        {
            TaxiOneSessionFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012
                .ConnectionString(c => c.FromConnectionStringWithKey("TaxiOneConnectionString"))
                .ShowSql())
                .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()))
                .BuildSessionFactory();

            TaxiTwoSessionFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012
                .ConnectionString(c => c.FromConnectionStringWithKey("TaxiTwoConnectionString"))
                .ShowSql())
                .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()))
                .BuildSessionFactory();
        }

        public static ISession OpenTaxiOneSession()
        {
            return TaxiOneSessionFactory.OpenSession();
        }

        public static ISession OpenTaxiTwoSession()
        {
            return TaxiTwoSessionFactory.OpenSession();
        }
    }
}
