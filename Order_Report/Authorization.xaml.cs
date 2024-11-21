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
using System.Windows.Shapes;

namespace Order_Report
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        private SalaamEntities db;
        private int failedLogin = 0;
        private string captchaText;
        public Authorization()
        {
            InitializeComponent();
            db = new SalaamEntities();
        }

        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {
            string login = tbLogin.Text;
            string password = pbPassword.Password;

            var user = db.Users.FirstOrDefault(u => u.Login == login);
            if (user != null)
            {
                if (string.IsNullOrEmpty(user.Salt))
                {
                    MessageBox.Show("соль не найдена.");
                    return;
                }

                if (PasswordHelper.VerifyPassword(password, user.Password, user.Salt))
                {
                    failedLogin = 0;
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    failedLogin++;
                    MessageBox.Show("ха лох неверно!");

                    if (failedLogin >= 3)
                    {
                        using (var captchaImageStream = Captcha.GenerateCaptchaImage(out captchaText))
                        {
                            ShowCaptchaWindow(captchaImageStream);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("пользователя нет!");
            }
        }

        private void ShowCaptchaWindow(MemoryStream captchaImageStream)
        {
            CaptchaWindow captchaWindow = new CaptchaWindow(captchaImageStream, captchaText);
            bool? result = captchaWindow.ShowDialog();

            if (result == true)
            {
                failedLogin = 0;
            }
        }

        private void Button_Register_Click(object sender, RoutedEventArgs e)
        {
            Register newUserWindow = new Register();
            newUserWindow.Show();
            this.Close();
        }
    }
}
