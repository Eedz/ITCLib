using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ITCLib
{
    public class ITCImage
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public string FilePath { get; set; }

        private BitmapImage imageSource;
        public BitmapImage ImageSource
        {
            get
            {
                if (imageSource == null && File.Exists(FilePath))
                {
                    imageSource = LoadImageWithoutLocking(FilePath);
                }
                return imageSource;
            }
        }

        public void SetSize(string filepath)
        {
            int width = 0;
            int height = 0;

            try
            {
                byte[] data = File.ReadAllBytes(filepath); // read without locking


                using (var ms = new MemoryStream(data))
                using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(ms))
                {
                    width = bmp.Width;
                    height = bmp.Height;
                    width = (int)Math.Round((decimal)width * 9525);
                    height = (int)Math.Round((decimal)height * 9525);
                }

                Height = height;
                Width = width;
            }
            catch
            {

            }
        }

        private BitmapImage LoadImageWithoutLocking(string path)
        {
            byte[] data = File.ReadAllBytes(path);
            using (var ms = new MemoryStream(data))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                image.Freeze(); // Optional but recommended
                return image;
            }
        }
    }
}

