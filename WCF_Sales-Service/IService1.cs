using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCF_Sales_Service
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        ProductMaster[] GetProducts();
        [OperationContract]
        DealerMaster[] GetDealers(); 
        [OperationContract]
        int GenerateInvoice(BillMaster objBill,RowOrderMaster [] orders);
        [OperationContract]
        BillMaster[] GetInvoices();
    }
}
