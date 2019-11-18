using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OpenXMLExtensions;
using System.Data;
using System.Text.RegularExpressions;

namespace ITCLib
{
    // TODO create Table, Cell, Row wrapper classes with methods for changing text, align, size etc.
    
    public class WordDocumentMaker
    {
        WordprocessingDocument doc;
        public Body body;

        public WordDocumentMaker(string filePath)
        {
            doc = WordprocessingDocument.Open(filePath, true);
            body = new Body();
            doc.MainDocumentPart.Document.Append(body);
            
        }

        public void Close()
        {
            doc.Close();
        }

        /// <summary>
        /// Create an XML table using the content found in the provided sourceTable.
        /// </summary>
        /// <returns></returns>
        public Table AddTable(DataTable sourceTable)
        {
            Table ResultTable = new Table();
            TableGrid grid = new TableGrid();
            TableProperties tblBorders = new TableProperties(GreySingleTableBorders());

            TableProperties tblLayout = new TableProperties(new TableLayout() { Type = TableLayoutValues.Fixed });
            
            ResultTable.AppendChild<TableProperties>(tblBorders);
            ResultTable.Append(tblLayout);

            // create columns for the table
            foreach (DataColumn c in sourceTable.Columns)
            {
                grid.Append(new GridColumn());
            }

            ResultTable.Append(grid);

            // header row
            CreateHeaderRow(ResultTable, sourceTable);

            // body rows
            FillRows(ResultTable, sourceTable);

            body.Append(ResultTable);

            return ResultTable;
        }

        

        /// <summary>
        /// Insert each row in the source data table into the XML table.
        /// </summary>
        private void FillRows(Table sourceTable, DataTable dt)
        {
            string[] newLineArray = { Environment.NewLine };
            int rowCount = dt.Rows.Count;
            int columnCount = dt.Columns.Count;
            // fill the rest of the rows
            for (int r = 0; r < rowCount; r++)
            {
                TableRow currentRow = new TableRow();
                for (int c = 0; c < columnCount; c++)
                {
                    TableCell currentCell = new TableCell();

                    currentCell.SetCellText(dt.Rows[r][c].ToString());

                    currentRow.Append(currentCell);


                }

                sourceTable.Append(currentRow);
            }


        }

        /// <summary>
        /// Create a single row in the XML table that contains the names of the columns in the source data table.
        /// </summary>
        private void CreateHeaderRow(Table ResultTable, DataTable dt)
        {
            // header row
            string heading;
            TableRow header = new TableRow();
            int columnCount = dt.Columns.Count;
            string[] newLineArray = { Environment.NewLine };

            for (int c = 0; c < columnCount; c++)
            {
                TableCell headerCell = new TableCell();
                heading = dt.Columns[c].Caption;
                headerCell.SetCellText(heading);
                header.Append(headerCell);
            }

            ResultTable.Append(header);
        }

        public void AddTitleParagraph(string title)
        {
            // format paragraph
            string[] lines = title.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string l in lines) { 
                Paragraph reportTitle = new Paragraph();
            
                ParagraphProperties titleProps = new ParagraphProperties();
                titleProps.Append(new Justification() { Val = JustificationValues.Center });
                titleProps.Append(new SpacingBetweenLines() { Before = "0", After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto, AfterAutoSpacing = false, BeforeAutoSpacing = false });

                reportTitle.Append(titleProps);

                // format run
                Run titleRun = new Run();
                RunProperties titleRunProps = new RunProperties();
                titleRunProps.Append(new RunFonts() { Ascii = "Arial" });
                titleRunProps.Append(new FontSize() { Val = "24" });
                titleRun.Append(titleRunProps);

                // add text
                Text titleText = new Text();

                titleText.Text = l;

                titleRun.Append(titleText);

                reportTitle.Append(titleRun);

                body.Append(reportTitle);
            }
           
            body.Append(new Paragraph());

        }


        private TableBorders GreySingleTableBorders() {
            return new TableBorders(
                new TopBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1,
                    Color = "a1a1a1"
                },
                new BottomBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1,
                    Color = "a1a1a1"
                },
                new LeftBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1,
                    Color = "a1a1a1"
                },
                new RightBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1,
                    Color = "a1a1a1"
                },
                new InsideHorizontalBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1,
                    Color = "a1a1a1"
                },
                new InsideVerticalBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1,
                    Color = "a1a1a1"
                });
                
        }

        private TableCellBorders BlackSingleCellBorder()
        {
            return new TableCellBorders(
                 new TopBorder
                 {
                     Val = new EnumValue<BorderValues>(BorderValues.Single),
                     Size = 1,
                     Color = "000000"
                 },
                 new BottomBorder
                 {
                     Val = new EnumValue<BorderValues>(BorderValues.Single),
                     Size = 1,
                     Color = "000000"
                 },
                 new LeftBorder
                 {
                     Val = new EnumValue<BorderValues>(BorderValues.Single),
                     Size = 1,
                     Color = "000000"
                 },
                 new RightBorder
                 {
                     Val = new EnumValue<BorderValues>(BorderValues.Single),
                     Size = 1,
                     Color = "000000"
                 },
                 new InsideHorizontalBorder
                 {
                     Val = new EnumValue<BorderValues>(BorderValues.Single),
                     Size = 1,
                     Color = "000000"
                 },
                 new InsideVerticalBorder
                 {
                     Val = new EnumValue<BorderValues>(BorderValues.Single),
                     Size = 1,
                     Color = "000000"
                 });
        }

        private Shading RoseShading()
        {
            return new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "ffa8d4" };
        }

        private Shading SkyBlueShading()
        {
            return new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "6de2fc" };
        }
    }
}
