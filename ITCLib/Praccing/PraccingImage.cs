using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class PraccingImage : ITCImage
    {
        public int ID { get; set; }
        public int PraccID { get; set; }
        public string Path { get; set; }

        public PraccingImage()
        {
            ID = 0;
            Path = string.Empty;
        }

        public PraccingImage(int id, string path)
        {
            ID = id;
            Path = path;
        }

        public override bool Equals(object obj)
        {
            var image = obj as PraccingImage;
            return image != null &&
                   ID == image.ID;
        }
    }
}
