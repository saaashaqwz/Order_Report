using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Order_Report
{
    /// <summary>
    /// Логика взаимодействия для Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        private SalaamEntities db;
        public Register()
        {
            InitializeComponent();
            db = new SalaamEntities();
        }
        private void Button_Register_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (db.Users.Any(u => u.Login == tbLogin.Text))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует.");
                    return;
                }
                else
                {
                    string salt = PasswordHelper.GenerateSalt();
                    Users newUser = new Users
                    {
                        Login = tbLogin.Text,
                        Salt = salt,
                        Password = PasswordHelper.HashPassword(pbPassword.Password, salt)
                    };

                    db.Users.Add(newUser);
                    db.SaveChanges();
                    MessageBox.Show("Регистрация прошла успешно!");

                    Authorization authWindow = new Authorization();
                    authWindow.Show();
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}
