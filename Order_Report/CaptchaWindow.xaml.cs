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
    /// Логика взаимодействия для CaptchaWindow.xaml
    /// </summary>
    public partial class CaptchaWindow : Window
    {
        private string captchaText;

        public CaptchaWindow(MemoryStream captchaImageStream, string captchaText)
        {
            InitializeComponent();
            this.captchaText = captchaText;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = captchaImageStream;
            bitmapImage.EndInit();
            CaptchaImage.Source = bitmapImage;
        }

        private void Button_CaptchaOK_Click(object sender, RoutedEventArgs e)
        {
            if (CaptchaInput.Text.Equals(captchaText, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Капча норм!");
                this.DialogResult = true; 
                this.Close();
            }
            else
            {
                MessageBox.Show("введи заново");
                CaptchaInput.Clear();
            }
        }
    }
}
