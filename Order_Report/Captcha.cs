using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Order_Report
{
    public class Captcha
    {
        private static Random random = new Random();
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public static MemoryStream GenerateCaptchaImage(out string captchaText)
        {
            int length = random.Next(4, 8); 
            captchaText = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            Bitmap bitmap = new Bitmap(400, 100);
            Graphics g = Graphics.FromImage(bitmap);

            g.Clear(Color.FromArgb(255, 255, 255)); 
            for (int i = 0; i < 500; i++)
            {
                int x = random.Next(0, bitmap.Width);
                int y = random.Next(0, bitmap.Height);
                bitmap.SetPixel(x, y, Color.FromArgb(random.Next(200), random.Next(200), random.Next(200))); 
            }

            int X = 20; 
            int Y = 25; 

            for (int i = 0; i < captchaText.Length; i++)
            {
                float angle = random.Next(-15, 15);
                Font font = new Font("Arial", random.Next(30, 40), FontStyle.Bold); 
                Brush brush = new SolidBrush(Color.FromArgb(random.Next(0, 150), random.Next(0, 150), random.Next(0, 150)));

                X += random.Next(40, 50); 
                g.TranslateTransform(X, Y);
                g.RotateTransform(angle);
                g.DrawString(captchaText[i].ToString(), font, brush, new PointF(0, 0));
                g.ResetTransform();
            }

            for (int i = 0; i < 5; i++)
            {
                g.DrawLine(new Pen(Color.FromArgb(random.Next(100, 150), random.Next(100, 150), random.Next(100, 150)), 2),
                    random.Next(0, bitmap.Width), random.Next(0, bitmap.Height),
                    random.Next(0, bitmap.Width), random.Next(0, bitmap.Height));
            }

            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            g.Dispose();
            bitmap.Dispose();

            ms.Position = 0;
            return ms;
        }
    }
}
