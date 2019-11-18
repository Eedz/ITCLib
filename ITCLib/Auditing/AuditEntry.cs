using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public enum AuditEntryType { Undefined, Insert, Update, Delete }

    public class AuditEntry
    {
        public int AuditID { get; set; }
        public AuditEntryType Type { get; set; }
        public string TableName { get; set; }
        public string PrimaryKeyField { get; set; }
        public string PrimaryKeyValue { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UserName { get; set; }
        public string Notes { get; set; }
    }
}
