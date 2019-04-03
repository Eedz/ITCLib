using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    /// <summary>
    /// Represents a region of the Earth.
    /// </summary>
    public class Region
    {
        public int RegionID { get; set; }
        public string RegionName { get; set; }
        public List<Study> Studies { get; set; }

        public Region()
        {
            Studies = new List<Study>();
        }
    }
}
