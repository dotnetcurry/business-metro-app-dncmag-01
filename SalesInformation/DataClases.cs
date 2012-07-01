using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesInformation
{
    public class BillInformation
    {
        public ObservableCollection<MyRef.ProductMaster> Products { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
        public int OrderBill { get; set; }
    }
}
