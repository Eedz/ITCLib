using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;

namespace ITCLib
{
    
    
    public class Export
    {
        public String templatePath = "\\\\psychfile\\psych$\\psych-lab-gfong\\SMG\\Access\\Reports\\Templates\\";  // location of report templates
        Word.Application appWord;
        Word.Document doc;
        String path;
        String title;
        String footer;
        PaperSize paperSize;
        Word.WdAutoFitBehavior autoFit;
        private bool prompt;

        // default constructor
        public Export()
        {
            Path = "";
            title = "ISIS Export";
            footer = "ISIS Export";
            paperSize = PaperSize.Letter;
            autoFit = Word.WdAutoFitBehavior.wdAutoFitContent;
            appWord = new Word.Application();

        }

        public string Path { get => path; set => path = value; }
        public bool Prompt { get => prompt; set => prompt = value; }

        public Word.Application ExportDataTable (DataTable dt, bool header = true, int pos = 0)
        {

            int cols, rows;
            Word.Table t;
            cols = dt.Columns.Count;

            if (cols ==0)
            {
                throw new Exception("DataTable has no columns.");
                
            }

            rows = dt.Rows.Count;
            if (rows ==0) { rows = 1; }

            // open a new document based on the appropriate template
            switch (paperSize)
            {
                case PaperSize.Letter:
                    doc = appWord.Documents.Add(templatePath + "SMGLandLet.dotx", false, Word.WdNewDocumentType.wdNewBlankDocument, false);
                    break;
                case PaperSize.Legal:
                    doc = appWord.Documents.Add(templatePath + "SMGLandLeg.dotx", false, Word.WdNewDocumentType.wdNewBlankDocument, false);
                    break;
                case PaperSize.Eleven17:
                    doc = appWord.Documents.Add(templatePath + "SMGLand11.dotx", false, Word.WdNewDocumentType.wdNewBlankDocument, false);
                    break;
                case PaperSize.A4:
                    doc = appWord.Documents.Add(templatePath + "SMGLandA4.dotx", false, Word.WdNewDocumentType.wdNewBlankDocument, false);
                    break;
            }

            t = doc.Tables.Add(doc.Range(pos, pos), rows, cols);

            // format borders
            t.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            t.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            t.Borders.OutsideColor = Word.WdColor.wdColorGray25;
            t.Borders.InsideColor = Word.WdColor.wdColorGray25;
            // header row has black outline
            t.Rows[1].Borders.OutsideColor = Word.WdColor.wdColorBlack;
            t.Rows[1].Borders.InsideColor = Word.WdColor.wdColorBlack;
            
            // row 1 is the header row, insert field names
            for (int i = 1; i <= cols; i++)
            {
                t.Cell(1, i).Range.Text = dt.Columns[i - 1].Caption;
                t.Cell(1, i).Range.Font.Bold = 1;
                t.Cell(1, i).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            }
            // shade it pink
            t.Rows[1].Shading.ForegroundPatternColor = Word.WdColor.wdColorRose;

            // first row of data is row 2
            
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    t.Cell(r+2, c+1).Range.Text = dt.Rows[r][c].ToString();
                }
                    
            }

            t.AutoFitBehavior(autoFit);
            t.Rows[1].HeadingFormat = -1;

            doc.Range(0, 0).Select();
            appWord.Selection.SplitTable();
            appWord.Selection.TypeParagraph();
            appWord.Selection.Font.Bold = 1;
            appWord.Selection.Font.Size = 16;
            appWord.Selection.Font.Name = "Arial";
            appWord.Selection.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            appWord.Selection.Text = title;

            doc.Sections[1].Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.InsertAfter ("\t" + footer + "\t\t" + "Generated on " + DateTime.Today.ToString("d"));

            appWord.Documents[doc].SaveAs2(path, 16);

            return appWord;
        }

        public void showReport()
        {
            DialogResult result;
            bool show = false;
            appWord.Documents[doc].Save();

            if (prompt)
            {
                result = MessageBox.Show("Done. View?", "Done.", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    show = true;
                }
                else
                {
                    show = false;
                }

            }
            else { show = true; }

            if (show)
            {
                appWord.Visible = true;
                appWord.Activate();
                appWord.Documents[doc].Range(0, 0).Select();
            }
            else
            {
                appWord.Quit();
            }

            doc = null;
            appWord = null;
            GC.Collect();
        }


    }
}
