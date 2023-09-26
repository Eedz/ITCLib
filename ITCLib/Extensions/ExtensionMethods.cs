using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ITCLib
{
    public static class ExtensionMethods
    {
        public static string ShortDate(this DateTime date)
        {
            return date.ToString("ddMMMyyyy");
        }

        public static string ShortDateDash(this DateTime date)
        {
            return date.ToString("dd-MMM-yyyy");
        }

        public static string DateTimeForFile(this DateTime date)
        {
            return date.ToString("ddMMMyyyy") + " (" + DateTime.Now.ToString("hh.mm.ss tt") + ")";
        }

        
        
        
    }
}
