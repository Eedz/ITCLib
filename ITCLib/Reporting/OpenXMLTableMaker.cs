using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Data;
using OpenXMLExtensions;

namespace ITCLib
{

    // wrapper for creating OpenXML tables
    public class OpenXMLTableMaker
    {
       
        Table ResultTable { get; set; }
        DataTable sourceTable;
        int rowCount;          // number of rows in the survey table
        int columnCount;    // number of columns in the survey table

        public OpenXMLTableMaker(DataTable dt)
        {
            sourceTable = dt;
            rowCount = dt.Rows.Count;
            columnCount = dt.Columns.Count;
            ResultTable = new Table();
        }

        /// <summary>
        /// Create an XML table using the content found in this object's sourceTable.
        /// </summary>
        /// <returns></returns>
        public Table CreateTable()
        {

            TableGrid grid = new TableGrid();
            TableProperties tblBorders = new TableProperties(new TableBorders(
                    new TopBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 1
                    },
                    new BottomBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 1
                    },
                    new LeftBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 1
                    },
                    new RightBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 1
                    },
                    new InsideHorizontalBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 1
                    },
                    new InsideVerticalBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 1
                    }));

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
            CreateHeaderRow();

            // body rows
            FillRows();

            return ResultTable;
        }

        /// <summary>
        /// Insert each row in the source data table into the XML table.
        /// </summary>
        private void FillRows()
        {
            string[] newLineArray = { Environment.NewLine };

            // fill the rest of the rows
            for (int r = 0; r < rowCount; r++)
            {
                TableRow currentRow = new TableRow();
                for (int c = 0; c < columnCount; c++)
                {
                    TableCell currentCell = new TableCell();

                    currentCell.SetCellText(sourceTable.Rows[r][c].ToString());

                    currentRow.Append(currentCell);
                }

                ResultTable.Append(currentRow);
            }
        }

        /// <summary>
        /// Create a single row in the XML table that contains the names of the columns in the source data table.
        /// </summary>
        private void CreateHeaderRow()
        {
            // header row
            string heading;
            TableRow header = new TableRow();
            string[] newLineArray = { Environment.NewLine };

            for (int c = 0; c < columnCount; c++)
            {
                TableCell headerCell = new TableCell();
                heading = sourceTable.Columns[c].Caption;
                headerCell.SetCellText(heading);
                header.Append(headerCell);
            }

            ResultTable.Append(header);
        }
    }
}
