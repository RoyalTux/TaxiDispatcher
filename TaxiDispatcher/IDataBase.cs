using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiDispatcher.Entities;

namespace TaxiDispatcher
{
    interface IDataBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool IsAdminPassInfoValid(string userName, string password);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientName"></param>
        /// <param name="destinationFrom"></param>
        /// <param name="destinationTo"></param>
        /// <param name="carType"></param>
        /// <param name="onlyThisCarType"></param>
        /// <returns></returns>
        int CreateOrder(string clientName, string destinationFrom, string destinationTo, string carType,
            bool onlyThisCarType);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<string> GetAllAvailableCarTypes();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trackingNumber"></param>
        /// <returns></returns>
        OrdersView TryGetOrderDataByValidTrackingNumber(int trackingNumber);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<OrdersView> GetAllOrdersData();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyNumber"></param>
        /// <returns></returns>
        List<DriversView> GetDriversByCompanyNumber(int companyNumber);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <param name="driversCompanyNumb"></param>
        /// <returns></returns>
        bool UpdateOrderInformation(OrdersView order, int driversCompanyNumb);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="companyNumber"></param>
        void AddNewDriver(DriversView driver, int companyNumber);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driverId"></param>
        /// <param name="companyNumber"></param>
        /// <returns></returns>
        bool DeleteDriver(int driverId, int companyNumber);
    }
}
