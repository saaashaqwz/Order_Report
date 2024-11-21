using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Order_Report
{
    /// <summary>
    /// Логика взаимодействия для OrderEditWindow.xaml
    /// </summary>
    public partial class OrderEditWindow : Window
    {
       
            private Orders order;
            private SalaamEntities db;

            // объявление события
            public event Action DataUpdated;

            public OrderEditWindow(Orders order, SalaamEntities db)
            {
                InitializeComponent();
                this.order = order;
                this.db = db;

                tbAddress.Text = order.Address;
                tbPayment.Text = order.Payment.ToString();
                tbStatus.Text = order.Status;
        }

            private void Button_Save_Click(object sender, RoutedEventArgs e)
            {
                order.Address = tbAddress.Text;
                order.Payment = tbPayment.Text;
                order.Status = tbStatus.Text;

                db.SaveChanges();

                DataUpdated?.Invoke();

                this.DialogResult = true;
                this.Close();
            }
        }
    
}
