using System;
using System.Collections.Generic;
using System.Linq;
//using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NHibernate;
using TaxiDispatcher.Entities;


namespace TaxiDispatcher
{
    class DataBaseServer : IDataBase
    {
        /// <summary>
        /// Проверяет логин и пароль админа
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool IsAdminPassInfoValid(string userName, string password)
        {
            AdminUsers user;
            using (ISession session = NhibernateHelper.OpenTaxiOneSession())
            {
                user =
                    session.Query<AdminUsers>()
                        .FirstOrDefault(x => x.userName == userName && x.userPassword == password);
            }
            if (user != null) return true;
            return false;
        }

        /// <summary>
        /// Создать новый заказ
        /// </summary>
        /// <param name="clientName"></param>
        /// <param name="destinationFrom"></param>
        /// <param name="destinationTo"></param>
        /// <param name="carType"></param>
        /// <param name="onlyThisCarType"></param>
        /// <returns>Номер для отслеживания</returns>
        public int CreateOrder(string clientName, string destinationFrom, string destinationTo, string carType, bool onlyThisCarType)
        {
            int trackingNumber = 0;
            using (ISession session = NhibernateHelper.OpenTaxiOneSession())
            {
                using (var tr = session.BeginTransaction())
                {
                    try
                    {
                        var tempTrackingNumber = new Random(Guid.NewGuid().GetHashCode()).Next();
                        var newOrder = new Orders
                        {
                            OrderTrackingNumber = tempTrackingNumber,
                            ClientName = clientName,
                            Destination_From = destinationFrom,
                            Destination_To = destinationTo,
                            PreferredCarTypeName = carType,
                            OnlyThisCarType = onlyThisCarType,
                            OrderStatus = 0
                        };
                        session.Save(newOrder);
                        tr.Commit();
                        trackingNumber = tempTrackingNumber;
                    }
                    catch (Exception e)
                    {
                        tr.Rollback();
                    }
                }
            }
            return trackingNumber;
        }

        /// <summary>
        /// Возвращает все доступные марки машин
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllAvailableCarTypes()
        {
            List<string> firstCompanyCarTypes;
            using (ISession session1 = NhibernateHelper.OpenTaxiOneSession())
            {
                firstCompanyCarTypes = session1.Query<CarTypes>().Select(x => x.CarTypeName).ToList();
            }

            List<string> secondCompanyCarTypes;
            using (ISession session2 = NhibernateHelper.OpenTaxiTwoSession())
            {
                secondCompanyCarTypes = session2.Query<CarTypes>().Select(x => x.CarTypeName).ToList();
            }

            var carTypes = firstCompanyCarTypes.Union(secondCompanyCarTypes).ToList();
            return carTypes;
        }

        /// <summary>
        /// Проверить наличие заказа с данным номером для отслеживания
        /// </summary>
        /// <param name="trackingNumber"></param>
        /// <returns></returns>
        public OrdersView TryGetOrderDataByValidTrackingNumber(int trackingNumber)
        {
            List<OrdersView> order = new List<OrdersView>();
            try
            {            
                using (ISession session = NhibernateHelper.OpenTaxiOneSession())
                {
                    //order = session.Query<Orders>().Where(x => x.OrderTrackingNumber == trackingNumber).ToList();
                    order = session.Query<OrdersView>().Where(x => x.OrderTrackingNumber == trackingNumber).ToList();
                }

                //if (order.Count > 0) return order[0];
                //return null;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Неправильный номер");
            }
            if (order.Count > 0) return order[0];
            return null;
        }

        /// <summary>
        /// Возвращает все заказы из 2ух баз
        /// </summary>
        /// <returns></returns>
        public List<OrdersView> GetAllOrdersData()
        {
            List<OrdersView> orders = new List<OrdersView>();
            using (ISession session = NhibernateHelper.OpenTaxiOneSession())
            {
                //order = session.Query<Orders>().Where(x => x.OrderTrackingNumber == trackingNumber).ToList();
                orders = session.Query<OrdersView>().ToList();
            }
            return orders;
        }

        /// <summary>
        /// Возввращает список водителей по заданной базе данных
        /// </summary>
        /// <param name="companyNumber"></param>
        /// <returns></returns>
        public List<DriversView> GetDriversByCompanyNumber(int companyNumber)
        {
            var drivers = new List<DriversView>();

            switch (companyNumber)
            {
                case 1:
                    using (ISession session = NhibernateHelper.OpenTaxiOneSession())
                    {
                        drivers = session.Query<DriversView>().ToList();
                    }
                    break;
                case 2:
                    using (ISession session = NhibernateHelper.OpenTaxiTwoSession())
                    {
                        drivers = session.Query<DriversView>().ToList();
                    }
                    break;
            }

            return drivers;
        }

        /// <summary>
        /// Обновить данные заказа
        /// </summary>
        /// <param name="order"></param>
        /// <param name="driversCompanyNumb"></param>
        /// <returns>true - если успешно</returns>
        public bool UpdateOrderInformation(OrdersView order, int driversCompanyNumb)
        {
            bool isOk = true;
            if (order.CompanyID == driversCompanyNumb)
            {
                ISession session = order.CompanyID == 2 ? NhibernateHelper.OpenTaxiTwoSession() : NhibernateHelper.OpenTaxiOneSession();

                using (var tr = session.BeginTransaction())
                {
                    try
                    {
                        var driver = session.Query<Drivers>().FirstOrDefault(x => x.DriverID == order.DriverID);
                        var orderFromDb = session.Query<Orders>()
                            .FirstOrDefault(x => x.OrderID == order.OrderID);
                        orderFromDb.AssignedDriver = driver;
                        orderFromDb.OrderStatus = order.OrderStatus;
                        session.SaveOrUpdate(orderFromDb);
                        tr.Commit();
                    }
                    catch (Exception e)
                    {
                        tr.Rollback();
                        isOk = false;
                    }
                }

                session.Dispose();
            }
            else
            {
                if (driversCompanyNumb > 0)
                {
                    ISession session = order.CompanyID == 2 ? NhibernateHelper.OpenTaxiTwoSession() : NhibernateHelper.OpenTaxiOneSession();

                    Orders changingDbOrder = new Orders { OrderStatus = -1 };

                    using (var tr = session.BeginTransaction())
                    {
                        try
                        {
                            changingDbOrder = session.Query<Orders>().FirstOrDefault(x => x.OrderID == order.OrderID);
                            if (changingDbOrder != null)
                            {
                                session.Delete(changingDbOrder);
                                tr.Commit();
                                changingDbOrder.AssignedDriver = null;
                            }
                            else
                            {
                                throw new Exception("Не найден заказ в базе");
                            }

                        }
                        catch (Exception)
                        {
                            tr.Rollback();
                            isOk = false;
                        }
                    }

                    session.Dispose();

                    if (changingDbOrder != null && changingDbOrder.OrderStatus != -1)
                    {
                        ISession sessionNumber2 = driversCompanyNumb == 2 ? NhibernateHelper.OpenTaxiTwoSession() : NhibernateHelper.OpenTaxiOneSession();

                        using (var tr = sessionNumber2.BeginTransaction())
                        {
                            try
                            {
                                var driver =
                                    sessionNumber2.Query<Drivers>().FirstOrDefault(x => x.DriverID == order.DriverID);
                                var newOrder = new Orders
                                {
                                    OrderTrackingNumber = changingDbOrder.OrderTrackingNumber,
                                    ClientName = changingDbOrder.ClientName,
                                    Destination_From = changingDbOrder.Destination_From,
                                    Destination_To = changingDbOrder.Destination_To,
                                    PreferredCarTypeName = changingDbOrder.PreferredCarTypeName,
                                    OnlyThisCarType = changingDbOrder.OnlyThisCarType,
                                    AssignedDriver = driver,
                                    OrderStatus = order.OrderStatus
                                };
                                sessionNumber2.Save(newOrder);
                                tr.Commit();
                            }
                            catch (Exception e)
                            {
                                tr.Rollback();
                                isOk = false;
                            }
                        }

                        sessionNumber2.Dispose();
                    }
                    else
                    {
                        isOk = false;
                    }
                }
            }
            return isOk;
        }

        /// <summary>
        /// Добавляем нового водителя в заданную бд
        /// </summary>
        /// <param name="companyNumber"></param>
        public void AddNewDriver(DriversView driver, int companyNumber)
        {
            ISession session = (companyNumber == 2)
                ? NhibernateHelper.OpenTaxiTwoSession()
                : NhibernateHelper.OpenTaxiOneSession();

            using (var tr = session.BeginTransaction())
            {
                try
                {
                    //session.CreateQuery(string.Format("EXEC AddNewDriver N'{0}', N'{1}', N'{2}', {3}",
                    //    driver.DriverName, driver.CarTypeName, driver.CarColor, driver.CarNumber));

                    var list = session.CreateSQLQuery("exec AddNewDriver ?, ?, ?, ?")
                        .AddEntity(typeof(Drivers))
                        .SetString(0, driver.DriverName)
                        .SetString(1, driver.CarTypeName)
                        .SetString(2, driver.CarColor)
                        .SetInt32(3, driver.CarNumber)
                        .List<Drivers>();

                    tr.Commit();
                }
                catch (Exception e)
                {
                    tr.Rollback();
                }
            }

            session.Dispose();
        }

        /// <summary>
        /// Удаляем водителя из базы
        /// </summary>
        /// <param name="driverId"></param>
        /// <returns></returns>
        public bool DeleteDriver(int driverId, int companyNumber)
        {
            bool isOk = true;

            ISession session = (companyNumber == 2)
                ? NhibernateHelper.OpenTaxiTwoSession()
                : NhibernateHelper.OpenTaxiOneSession();

            using (var tr = session.BeginTransaction())
            {
                try
                {
                    //session.CreateQuery(string.Format("EXEC AddNewDriver N'{0}', N'{1}', N'{2}', {3}",
                    //    driver.DriverName, driver.CarTypeName, driver.CarColor, driver.CarNumber));

                    var list = session.CreateSQLQuery("exec DeleteDriver ?")
                        .AddEntity(typeof(Drivers))
                        .SetInt32(0, driverId)
                        .List<Drivers>();

                    tr.Commit();
                }
                catch (Exception e)
                {
                    tr.Rollback();
                    isOk = false;
                }
            }

            session.Dispose();

            return isOk;
        }
    }
}
