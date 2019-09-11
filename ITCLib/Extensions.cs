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

        /// <summary>
        /// Sets the text of a TableCell. Paragraph and run properties are copied from the first paragraph and run if they are already present.
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="text"></param>
        public static void SetCellText (this TableCell cell, string text)
        {
            string[] newLineArray = { Environment.NewLine };
            string[] textArray = text.Split(newLineArray, StringSplitOptions.None);

            Paragraph firstParagraph = cell.Elements<Paragraph>().FirstOrDefault();
            Run firstRun;
            ParagraphProperties pPr;
            RunProperties rPr;

            if (firstParagraph != null)
            {
                pPr = cell.Elements<Paragraph>().First().Elements<ParagraphProperties>().FirstOrDefault();
                if (pPr == null) firstParagraph.Append(new ParagraphProperties());
                firstRun = firstParagraph.Elements<Run>().FirstOrDefault();
            }
            else
            {
                pPr = new ParagraphProperties();
                rPr = new RunProperties();
                firstRun = null;
            }

            if (firstRun != null)
            {
                rPr = firstRun.Elements<RunProperties>().FirstOrDefault();
                if (rPr == null) firstRun.Append(new RunProperties());
            }
            else
            {
                rPr = new RunProperties();
            }

            cell.RemoveAllChildren<Paragraph>();

            foreach (string s in textArray)
            {
                Paragraph p = new Paragraph();
                p.Append(new ParagraphProperties(pPr.OuterXml));
                Run r = new Run();
                r.Append(new RunProperties(rPr.OuterXml));
                Text t = new Text(s);
                r.Append(t);
                p.Append(r);
                cell.Append(p);
            }
        }

        /// <summary>
        /// Returns the text from each Text element in a TableCell. Line breaks are inserted between each Text run.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static string GetCellText(this TableCell cell)
        {
            string cellText = "";
            foreach (Paragraph p in cell.Descendants<Paragraph>())
            {

                foreach (Text t in p.Descendants<Text>())
                {
                    cellText += t.Text;
                }
                cellText += "\r\n";
            }
            return cellText.TrimEnd(new char[] { '\r', '\n' });
        }
    }

    
}
