using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class PraccingCategory
    {
        public int ID { get; set; }
        public string Category { get; set; }

        public PraccingCategory()
        {
            Category = string.Empty;
        }

        public PraccingCategory(int id, string category)
        {
            ID = id;
            Category = category;
        }

        public override bool Equals(object obj)
        {
            var category = obj as PraccingCategory;
            return category != null &&
                   ID == category.ID;
        }

        public override int GetHashCode()
        {
            var hashCode = 1479869798;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return Category;
        }
    }
}
