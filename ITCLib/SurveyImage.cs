using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class SurveyImage : ITCImage
    {
        public int ID { get; set; }
        public int QID { get; set; }
        public string ImagePath { get; set; }
        public string ImageName { get; set; }
        public string Survey { get; set; }
        public string VarName { get; set; }
        public string Language { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }

        public string Encoded { get => VarName + Country + Language; }

        public SurveyImage()
        {

        }

        public SurveyImage(string filename)
        {
            SetParts(filename);
        }

        public void SetParts(string filename)
        {
            string[] parts = filename.Split('_');

            if (parts.Length == 3)
            {
                Language = parts[0];
                Country = parts[1];
                Description = parts[2];
            }
            else
            {
                if (filename.IndexOf('_') == -1)
                    return;

                int first_ = filename.IndexOf('_') + 1;
                int second_ = filename.IndexOf('_', first_);

                Language = filename.Substring(0, first_);

                if (second_ == -1 || first_ == -1)
                {
                    Country = string.Empty;
                    Description = filename.Substring(filename.LastIndexOf(@"\") + 1);
                }
                else
                {
                    Country = filename.Substring(first_, second_ - first_);
                    Description = filename.Substring(second_ + 1);
                }
            }
        }

        public void SetParts()
        {
            SetParts(ImageName);
        }

        public override string ToString()
        {
            return "Image name: " + ImageName;
        }

    }
}
