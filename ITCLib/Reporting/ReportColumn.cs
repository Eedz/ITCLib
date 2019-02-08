using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class ReportColumn
    {
        int ordinal;
        string columnName;

        public ReportColumn (string name, int ord)
        {
            columnName = name;
            ordinal = ord;
        }

        public int Ordinal { get => ordinal; set => ordinal = value; }
        public string ColumnName { get => columnName; set => columnName = value; }
    }
}
