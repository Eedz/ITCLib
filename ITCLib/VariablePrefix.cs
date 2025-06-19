using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
namespace ITCLib
{
    public class VariablePrefix : ObservableObject
    {
        private int id;
        public int ID
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }
        private string prefix;
        public string Prefix { get { return prefix; } set { SetProperty(ref prefix, value); } }
        private string prefixName;
        public string PrefixName { get { return prefixName; } set { SetProperty(ref prefixName, value); } }
        private string productType;
        public string ProductType { get { return productType; } set { SetProperty(ref productType, value); } }
        // possible not needed
        private string relatedPrefixes;
        public string RelatedPrefixes { get { return relatedPrefixes; } set { SetProperty(ref relatedPrefixes, value); } }
        private string description;
        public string Description { get { return description; } set { SetProperty(ref description, value); } }
        private string comments;
        public string Comments { get { return comments; } set { SetProperty(ref comments, value); } }
        private bool inactive;
        public bool Inactive { get { return inactive; } set { SetProperty(ref inactive, value); } }

        public List<ParallelPrefix> ParallelPrefixes { get; set; }
        public List<VariableRange> Ranges { get; set; }

        public VariablePrefix()
        {
            ParallelPrefixes = new List<ParallelPrefix>();
            Ranges = new List<VariableRange>();
        }


        public override string ToString()
        {
            return Prefix + "(" + PrefixName + ")";
        }

        public override bool Equals(object obj)
        {
            var type = obj as VariablePrefix;
            return type != null &&
                   ID == type.ID &&
                   Prefix == type.Prefix;
        }

        public override int GetHashCode()
        {
            var hashCode = 1479869798;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Prefix);
            return hashCode;
        }
    }

    public class ParallelPrefix
    {
        public int ID { get; set; }
        public int PrefixID { get; set; }
        public int RelatedPrefixID { get; set; }
        public string RelatedPrefix { get; set; }

        public ParallelPrefix()
        {
            RelatedPrefix = string.Empty;
        }

        public override string ToString()
        {
            return RelatedPrefix;
        }

        public override bool Equals(object obj)
        {
            var type = obj as ParallelPrefix;
            return type != null &&
                   ID == type.ID &&
                   RelatedPrefix == type.RelatedPrefix;
        }

        public override int GetHashCode()
        {
            var hashCode = 1479869798;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(RelatedPrefix);
            return hashCode;
        }
    }

    public class VariableRange
    {
        public int ID { get; set; }
        public int PrefixID { get; set; }
        public string Lower { get; set; }
        public string Upper { get; set; }
        public string Description { get; set; }

        public VariableRange()
        {

        }

        public int LowerInt()
        {
            if (string.IsNullOrEmpty(Lower))
                return 0;
            return Int32.Parse(Lower);
        }

        public int UpperInt()
        {
            if (string.IsNullOrEmpty(Upper))
                return 0;
            return Int32.Parse(Upper);
        }
    }
}
