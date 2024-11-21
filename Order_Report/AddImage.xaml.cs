using System;
using System.IO;
using System.Data.SqlClient;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Media.Imaging;

namespace Order_Report
{
    public partial class AddImage : Window
    {
        public AddImage()
        {
            InitializeComponent();
        }

        private void SelectImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif"; 

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                FilePathTextBox.Text = filePath; 

                BitmapImage bitmapImage = new BitmapImage(new Uri(filePath));
                image.Source = bitmapImage;
            }
        }

        private void UploadImage(object sender, RoutedEventArgs e)
        {
            string filePath = FilePathTextBox.Text;

            if (string.IsNullOrWhiteSpace(filePath))
            {
                MessageBox.Show("Выберите изображение");
                return;
            }

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Файл не найден");
                return;
            }

            byte[] imageBytes = File.ReadAllBytes(filePath);

            try
            {
                string connectionString = @"Server=WIN-N1U0GT2AIVJ\SQL2023;Database=Salaam;Integrated Security=True;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO ImageTable (ImageData) VALUES (@ImageData)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ImageData", imageBytes);
                        command.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Изображение успешно загружено");
                this.Close(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}");
            }
        }
    }
}