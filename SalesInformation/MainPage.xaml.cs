using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SalesInformation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //WCF Proxy class object
        MyRef.ServiceClient Proxy;
        //Collection for Products
        ObservableCollection<MyRef.ProductMaster> _Products;
        public ObservableCollection<MyRef.ProductMaster> Products
        {
            get { return _Products; }
            set { _Products = value; }
        }

        #region Local UI Element Declaratrion used for Generatine the Order Row in ListBox
        ComboBox cmbProducts;
        TextBox txtUnitPrice;
        TextBox txtQuantity;
        TextBox txtProductwiseBill;
        StackPanel stkPnlContainer;
        #endregion

        static int TotalBill = 0;

       static ObservableCollection<MyRef.RowOrderMaster> Orders;


        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
            Products = new ObservableCollection<MyRef.ProductMaster>();
            Orders = new ObservableCollection<MyRef.RowOrderMaster>(); 
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Proxy = new MyRef.ServiceClient();
            LoadDealers();
            LoadProducts();

            CreateBillUI();
        }
        /// <summary>
        /// Helper Method to Read All Dealers
        /// </summary>
        void  LoadDealers()
        {
           Task<ObservableCollection<MyRef.DealerMaster>> taskDealers =  Proxy.GetDealersAsync();
           cmbDealerName.ItemsSource = taskDealers.Result;
           cmbDealerName.DisplayMemberPath = "DealerName";
           cmbDealerName.SelectedValuePath = "DealerID";  
        }

        /// <summary> 
        /// Helper Method to Load all Products
        /// </summary>
        void LoadProducts()
        {
            Task<ObservableCollection<MyRef.ProductMaster>> taskProduct = Proxy.GetProductsAsync();
            Products = taskProduct.Result;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }

        /// <summary>
        /// Method Which will Query to the Products collection and read the Product Data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox ctrl = sender as ComboBox;
            int ProductId = Convert.ToInt32(ctrl.SelectedValue);
            var Product = (from p in Products
                          where p.ProductID == ProductId
                          select p).First();

            txtUnitPrice.Text = Product.UnitPrice.ToString(); 
        }

        /// <summary>
        /// Helper Method to Create UI inside the ListBox for adding Productwise order details
        /// like Quantity
        /// </summary>
        void CreateBillUI()
        {
            stkPnlContainer = new StackPanel();
            stkPnlContainer.Orientation = Orientation.Horizontal;
            stkPnlContainer.Width = lstContainer.Width;
            stkPnlContainer.Height = 50;
            
            cmbProducts = new ComboBox();
            cmbProducts.ItemsSource = Products;
            cmbProducts.Width = 150;
            cmbProducts.Height = 50; 
            cmbProducts.DisplayMemberPath = "ProductName";
            cmbProducts.SelectedValuePath = "ProductID";
            //Register to the event to generate Product details
            cmbProducts.SelectionChanged += cmbProducts_SelectionChanged;  
            stkPnlContainer.Children.Add(cmbProducts);

            txtUnitPrice = new TextBox();
            txtUnitPrice.Width = 150;
            txtUnitPrice.Height = 50;
            txtUnitPrice.IsEnabled = false;
            txtUnitPrice.Text = "000";
            stkPnlContainer.Children.Add(txtUnitPrice);

            txtQuantity = new TextBox();
            txtQuantity.Width = 150;
            txtQuantity.Height = 50;
            txtQuantity.Text = "000";
            //Register to the event to generate bill for a Product 
            txtQuantity.LostFocus += txtQuantity_LostFocus; 
            stkPnlContainer.Children.Add(txtQuantity);

           
            txtProductwiseBill = new TextBox();
            txtProductwiseBill.Width = 150;
            txtProductwiseBill.Height = 50;
            txtProductwiseBill.Text = "000";
            txtProductwiseBill.IsEnabled = false;
            stkPnlContainer.Children.Add(txtProductwiseBill);

            ListBoxItem lst = new ListBoxItem();
            lst.Content = stkPnlContainer;
            lstContainer.Items.Add(lst); 
        }
        /// <summary>
        /// Method used to Create Rowwise Order Bill
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void txtQuantity_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                txtProductwiseBill.Text = Convert.ToString(Convert.ToInt32(txtQuantity.Text) * Convert.ToInt32(txtUnitPrice.Text));
                TotalBill += Convert.ToInt32(txtProductwiseBill.Text);
                txtTotalBill.Text = TotalBill.ToString();
                //Store the added bill in Orders collection
                Orders.Add(new MyRef.RowOrderMaster() 
                {
                     DealerID = Convert.ToInt32(cmbDealerName.SelectedValue),
                      ProductID = Convert.ToInt32(cmbProducts.SelectedValue),
                     Quantity = Convert.ToInt32(txtQuantity.Text),
                     OrderPrice = Convert.ToInt32(txtProductwiseBill.Text)
                });

            }
            catch (Exception ex)
            {
                txtError.Text = ex.Message;
            }
        }

        /// <summary>
        /// Method to Add a new Item Row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            CreateBillUI();
        }

        /// <summary>
        /// Method to Save Invoice 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MyRef.BillMaster objBill = new MyRef.BillMaster();
                objBill.DealerID = Convert.ToInt32(cmbDealerName.SelectedValue);
                objBill.TotalBill = Convert.ToInt32(txtTotalBill.Text);

                Task<int> Res =  Proxy.GenerateInvoiceAsync(objBill, Orders);

                if (Res.Result == 1)
                {
                    txtError.Text = "Invoice Genetared Successfully";
                }

                var Invoices = Proxy.GetInvoicesAsync();
                var InvoiceData = Invoices.Result;

                //Get the Newly generated Invoice from Table
                int LatestInvoice = (from inv in InvoiceData
                                     select inv.InvoiceNo).Max();

                txtInvoiceno.Text = LatestInvoice.ToString();
 
            }
            catch (Exception ex)
            {
                txtError.Text = ex.Message; 
            }

        }
       
    }
}
