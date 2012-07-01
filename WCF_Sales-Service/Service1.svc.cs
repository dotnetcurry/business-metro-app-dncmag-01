using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCF_Sales_Service
{
    public class Service : IService
    {
        SalesInfoEntities objContext;
        public Service()
        {
            objContext = new SalesInfoEntities();
        }
        public ProductMaster[] GetProducts()
        {
            var Products = objContext.ProductMasters.ToArray();
            return Products; 
        }

        public DealerMaster[] GetDealers()
        {
            var Dealers = objContext.DealerMasters.ToArray();
            return Dealers; 
        }

        /// <summary>
        /// Method to Save the Orders
        /// </summary>
        /// <param name="objBill"></param>
        /// <param name="orders"></param>
        /// <returns></returns>
        public int GenerateInvoice(BillMaster objBill, RowOrderMaster[] orders)
        {
            foreach (var Order in orders)
            {
                objContext.AddToRowOrderMasters(Order);  
            }
            objContext.AddToBillMasters(objBill);
           int Res =  objContext.SaveChanges();
           if (Res > 0)
           {
               Res = 1;
           }
           return Res;
        }


        public BillMaster [] GetInvoices()
        {
            var Invoices = objContext.BillMasters.ToArray();
            return Invoices;
        }
    }
}
