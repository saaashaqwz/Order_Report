using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Order_Report
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SalaamEntities db;

        public MainWindow()
        {
            InitializeComponent();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            db = new SalaamEntities();
            OrderDataGrid.ItemsSource = db.Orders.ToList();
            LoadClientDataGrid();
            LoadClientsComboBox();
            combobind();
        }
        private void LoadClientDataGrid()
        {
            var ordersQuery = from o in db.Orders
                              join c in db.Clients on o.Client_ID equals c.Client_ID
                              join s in db.Staffs on o.Staff_ID equals s.Staff_ID
                              join p in db.Posts on s.Post_ID equals p.Post_ID
                              join op in db.Order_Product on o.Order_ID equals op.Order_ID
                              join pr in db.Products on op.Product_ID equals pr.Product_ID
                              select new
                              {
                                  o.Order_ID,
                                  ClientName = c.Surname_Client + " " + c.Name_Client + " " + c.Lastname_Client,
                                  StaffName = s.Surname_Staff + " " + s.Name_Staff + " " + s.Lastname_Staff,
                                  PostTitle = p.Title,
                                  ProductTitle = pr.Title,
                                  ProductAmount = op.Amount,
                                  o.Address,
                                  o.Payment,
                                  o.Status
                              };

            ClientDataGrid.ItemsSource = ordersQuery.ToList();
        }
        private void LoadClientsComboBox()
        {
            var clientsQuery = from c in db.Clients
                               select new
                               {
                                   c.Client_ID,
                                   FullName = c.Surname_Client + " " + c.Name_Client + " " + c.Lastname_Client
                               };

            ClientsComboBox.ItemsSource = clientsQuery.ToList();
            ClientsComboBox.DisplayMemberPath = "FullName"; 
            ClientsComboBox.SelectedValuePath = "Client_ID"; 
        }

        private void ClientsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Проверяем, выбран ли клиент
            if (ClientsComboBox.SelectedValue != null)
            {
                int selectedClientId = (int)ClientsComboBox.SelectedValue;
                FilterDataGridByClient(selectedClientId);
            }
        }

        private void FilterDataGridByClient(int clientId)
        {
            var filteredOrdersQuery = from o in db.Orders
                                      join c in db.Clients on o.Client_ID equals c.Client_ID
                                      join s in db.Staffs on o.Staff_ID equals s.Staff_ID
                                      join p in db.Posts on s.Post_ID equals p.Post_ID
                                      join op in db.Order_Product on o.Order_ID equals op.Order_ID
                                      join pr in db.Products on op.Product_ID equals pr.Product_ID
                                      where o.Client_ID == clientId  // Фильтруем по выбранному клиенту
                                      select new
                                      {
                                          o.Order_ID,
                                          ClientName = c.Surname_Client + " " + c.Name_Client + " " + c.Lastname_Client,
                                          StaffName = s.Surname_Staff + " " + s.Name_Staff + " " + s.Lastname_Staff,
                                          PostTitle = p.Title,
                                          ProductTitle = pr.Title,
                                          ProductAmount = op.Amount,
                                          o.Address,
                                          o.Payment,
                                          o.Status
                                      };

            // Обновляем источник данных для DataGrid
            ClientDataGrid.ItemsSource = filteredOrdersQuery.ToList();
        }

        private void RefreshDatagrid()
        {
            OrderDataGrid.ItemsSource = db.Orders.ToList();
        }
        // свойство для хранения списка продуктов
        public List<Orders> Ord { get; set; }


        //фильтрация продуктов на основе combobox
        private void cbP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            OrderDataGrid.ItemsSource = db.Orders.Where(x => x.Order_ID == cbFiltration.SelectedIndex + 1).ToList();

        }
        //фильтрация продуктов на основе textbox
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbFiltration.Text))
            {
                OrderDataGrid.ItemsSource = db.Orders.Where(x => x.Status.Contains(tbFiltration.Text)).ToList();
            }
            else
            {
                RefreshDatagrid(); //обновление, чтобы показать все продукты, если ввод очищен
            }
        }

        private void combobind()
        {
            SalaamEntities db = new SalaamEntities();
            var item = db.Orders.ToList();
            Ord = item;
            DataContext = Ord;
        }

        private void Button_PDF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false; //отключение UI во время печати
                PrintDialog pD = new PrintDialog();
                if (pD.ShowDialog() == true)
                {
                    pD.PrintVisual(OrderDataGrid, "хеви метал2 пдф");
                }
            }
            finally
            {
                this.IsEnabled = true; // включение UI обратно
            }
        }
        private void dgOrder_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (OrderDataGrid.SelectedItem is var selectedOrder && selectedOrder != null)
            {
                int orderId = ((dynamic)selectedOrder).Order_ID;
                var orderToEdit = db.Orders.Find(orderId);

                OrderEditWindow editWindow = new OrderEditWindow(orderToEdit, db);
                editWindow.DataUpdated += RefreshDatagrid; // подписка на событие
                editWindow.ShowDialog();
            }
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            Orders order = new Orders();

            order.Order_ID = Convert.ToInt32(tbOrderId.Text);
            order.Address = tbAddess.Text;
            order.Payment = tbPayment.Text;
            order.Status = tbStatus.Text;

            db.Orders.Add(order);
            db.SaveChanges();
            RefreshDatagrid();
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            int selectId = Convert.ToInt32(tbOrderId.Text);
            var selectDeleteOrder = db.Orders.Where(o => o.Order_ID == selectId).FirstOrDefault();

            db.Orders.Remove(selectDeleteOrder);
            db.SaveChanges();
            RefreshDatagrid();
        }

        private void Button_Update_Click(object sender, RoutedEventArgs e)
        {
            int selectId = Convert.ToInt32(tbOrderId.Text);
            var selectUpdateOrder = db.Orders.Where(o => o.Order_ID == selectId).FirstOrDefault();

            selectUpdateOrder.Order_ID = Convert.ToInt32(tbOrderId.Text);
            selectUpdateOrder.Address = tbAddess.Text;
            selectUpdateOrder.Payment = tbPayment.Text;
            selectUpdateOrder.Status = tbStatus.Text;

            db.SaveChanges();
            RefreshDatagrid();
        }

        private void ExportToCsv(string filePath)
        {
            var orders = db.Orders.ToList();
            StringBuilder csvContent = new StringBuilder();

            csvContent.AppendLine("Order_ID,Address,Payment,Status");

            foreach (var order in orders)
            {
                csvContent.AppendLine($"{order.Order_ID},{order.Address},{order.Payment},{order.Status}");
            }

            System.IO.File.WriteAllText(filePath, csvContent.ToString(), Encoding.UTF8);
        }
        private void ExportToExcel(string filePath)
        {
            var orders = db.Orders.ToList();
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Orders");
                worksheet.Cells[1, 1].Value = "Order_ID";
                worksheet.Cells[1, 2].Value = "Address";
                worksheet.Cells[1, 3].Value = "Payment";
                worksheet.Cells[1, 4].Value = "Status";

                for (int i = 0; i < orders.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = orders[i].Order_ID;
                    worksheet.Cells[i + 2, 2].Value = orders[i].Address;
                    worksheet.Cells[i + 2, 3].Value = orders[i].Payment;
                    worksheet.Cells[i + 2, 4].Value = orders[i].Status;
                }

                package.SaveAs(new FileInfo(filePath));
            }
        }

        private void Export_Csv_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "заказы.csv",
                Filter = "CSV files (*.csv)|*.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                ExportToCsv(saveFileDialog.FileName);
                MessageBox.Show("экспорт в csv прошел успешно");
            }
        }

        private void Export_Excel_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "заказы.xlsx",
                Filter = "Excel files (*.xlsx)|*.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                ExportToExcel(saveFileDialog.FileName);
                MessageBox.Show("экспорт в excel прошел успешно");
            }
        }

        private void ImportFromCsv(string filePath)
        {
            var lines = System.IO.File.ReadAllLines(filePath);
            for (int i = 1; i < lines.Length; i++)
            {
                var values = lines[i].Split(',');
                var order = new Orders
                {
                    Order_ID = int.Parse(values[0]),
                    Address = values[1],
                    Payment = values[2],
                    Status = values[3]
                };
                db.Orders.Add(order);
            }
            db.SaveChanges();
            RefreshDatagrid();
            MessageBox.Show("Импорт из CSV прошел успешно");
        }

        private void ImportFromExcel(string filePath)
        {
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rows = worksheet.Dimension.Rows;

                for (int i = 2; i <= rows; i++)
                {
                    var order = new Orders();

                    if (worksheet.Cells[i, 1].Value != null && int.TryParse(worksheet.Cells[i, 1].Text, out int orderId))
                    {
                        order.Order_ID = orderId;
                    }

                    order.Address = worksheet.Cells[i, 2].Text ?? string.Empty;
                    order.Payment = worksheet.Cells[i, 3].Text ?? string.Empty;
                    order.Status = worksheet.Cells[i, 4].Text ?? string.Empty;

                    db.Orders.Add(order);
                }
                db.SaveChanges();
                RefreshDatagrid();
                MessageBox.Show("Импорт из Excel прошел успешно");
            }
        }

        private void Import_Csv_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ImportFromCsv(openFileDialog.FileName);
            }
        }

        private void Import_Excel_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ImportFromExcel(openFileDialog.FileName);
            }
        }

        private void OpenAddImage(object sender, RoutedEventArgs e)
        {
            AddImage addImageWindow = new AddImage();
            addImageWindow.ShowDialog(); 
        }
    }
}
