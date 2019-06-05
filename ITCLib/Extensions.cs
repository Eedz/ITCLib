using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace OpenXMLExtensions
{
    public static class OpenXMLExtensions
    {
        public static void SetCellText (this TableCell cell, string text)
        {
            string[] newLineArray = { Environment.NewLine };
            string[] textArray = text.Split(newLineArray, StringSplitOptions.None);
            cell.RemoveAllChildren<Paragraph>();
            foreach (string s in textArray)
            {
                cell.Append(new Paragraph(new Run(new Text(s))));
            }
        }

        public static string GetCellText(this TableCell cell)
        {
            string cellText = "";
            foreach (Paragraph p in cell.Elements<Paragraph>())
            {

                foreach (Text t in p.Descendants<Text>())
                {
                    cellText += t.Text + "\r\n";
                }

            }
            return cellText.TrimEnd(new char[] { '\r', '\n' });
        }
    }

    
}
