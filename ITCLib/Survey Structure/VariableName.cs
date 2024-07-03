using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ITCLib
{
    public class VariableName : RefVariableName
    {
        //public int new ID { get; set; } // TODO implement in database
        
        public string CountryCode { get; set; } // TODO unused

        public string VarName
        {
            get => _varname; 
            set
            {
                SetProperty(ref _varname, value);
                RefVarName = Utilities.ChangeCC(VarName);                
            }
        }

        public string RefVarLabel { get { return RefVarName + " - " + VarLabel; } }
     
        public DomainLabel Domain
        {
            get => _domain; 
            set => SetProperty(ref _domain, value);                
        }

        public TopicLabel Topic
        {
            get => _topic;
            set => SetProperty(ref _topic, value);
        }

        public ContentLabel Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        public ProductLabel Product
        {
            get => _product;
            set => SetProperty(ref _product, value);
        }
        
        public string VarLabel
        {
            get => _varlabel;
            set => SetProperty(ref _varlabel, value);
        }

        public VariableName()
        {
            VarName = string.Empty;

            RefVarName = string.Empty;

            VarLabel = "[blank]";
            Domain = new DomainLabel(0, "No Domain");
            Topic = new TopicLabel(0, "No Topic");
            Content = new ContentLabel(0, "No Content");
            Product = new ProductLabel(0, "Unassigned");
        }

        public VariableName(string varname)
        {
            VarName = varname;

            RefVarName = Utilities.ChangeCC(varname);

            VarLabel = "[blank]";
            Domain = new DomainLabel(0, "No Domain");
            Topic = new TopicLabel(0, "No Topic");
            Content = new ContentLabel(0, "No Content");
            Product = new ProductLabel(0, "Unassigned");
        }

        public VariableName(VariableName varname)
        {
            VarName = varname.VarName;

            RefVarName = Utilities.ChangeCC(varname.VarName);

            VarLabel = varname.VarLabel;
            Domain = new DomainLabel(varname.Domain.ID, varname.Domain.LabelText);
            Topic = new TopicLabel(varname.Topic.ID, varname.Topic.LabelText);
            Content = new ContentLabel(varname.Content.ID, varname.Content.LabelText);
            Product = new ProductLabel(varname.Product.ID, varname.Product.LabelText);
        }


        public override string ToString()
        {
            return VarName;
        }

        public override bool Equals(object obj)
        {
            var name = obj as VariableName;
            return name != null &&
                   VarName == name.VarName;
        }

        public override int GetHashCode()
        {
            return -1632883202 + EqualityComparer<string>.Default.GetHashCode(VarName);
        }

        private string _varname;
        private DomainLabel _domain;
        private TopicLabel _topic;
        private ContentLabel _content;
        private ProductLabel _product;
        private string _varlabel;
    }

    public class RefVariableName : ObservableObject 
    {
        private int _id;
        private string _refvarname;
        private string _prefix;
        private string _number;
        private string _suffix;
        private bool _standardform;

        public int ID { get => _id; set => SetProperty(ref _id, value); } // TODO to implement in database
        public string RefVarName { get => _refvarname; 
            set
            {
                if (SetProperty(ref _refvarname, value))   
                    SetParts();
            }
        }

        // TODO use these properties to get the full refVarName
        public string Prefix { get; set; }
        public string Number { get; set; }
        public string Suffix { get; set; }
        public bool StandardForm { get; set; }

        public RefVariableName()
        {
            RefVarName = string.Empty;
            Prefix = string.Empty;
            Number = string.Empty;
            Suffix = string.Empty;
        }

        public RefVariableName(string refvarname)
        {
            RefVarName = refvarname;
        }

        private void SetParts()
        {
            if (_refvarname.Length < 5)
            {
                StandardForm = false;
                Prefix = string.Empty;
                Number = string.Empty;
                Suffix = string.Empty;
                return;
            }

            Prefix = _refvarname.Substring(0, 2);
            if (!(char.IsLetter(Prefix[0]) && char.IsLetter(Prefix[1])))
            {
                Prefix = string.Empty;
                Number = string.Empty;
                Suffix = string.Empty;
                StandardForm = false;
                return;
            }

            if (Int32.TryParse(_refvarname.Substring(2, 3), out int n))
                Number = n.ToString();
            else
            {
                Prefix = string.Empty;
                Number = string.Empty;
                Suffix = string.Empty;
                StandardForm = false;
                return;
            }

            if (_refvarname.Length >= 6)
            {
                Suffix = _refvarname.Substring(5);
                for (int i = 0; i < Suffix.Length; i++)
                {
                    if (char.IsDigit(Suffix[i]))
                    {
                        Prefix = string.Empty;
                        Number = string.Empty;
                        Suffix = string.Empty;
                        StandardForm = false;
                        return;
                    }
                }
            }

            StandardForm = true;
        }

        public int NumberInt()
        {
            if (string.IsNullOrEmpty(Number))
                return 0;
            return Int32.Parse(Number);
        }

        public override string ToString()
        {
            return RefVarName;
        }

        public override bool Equals(object obj)
        {
            var name = obj as RefVariableName;
            return name != null &&
                   RefVarName == name.RefVarName;
        }

        public override int GetHashCode()
        {
            return -1632883202 + EqualityComparer<string>.Default.GetHashCode(RefVarName);
        }
    }

    public class VarNameKeyword
    {
        public int ID { get; set; }
        public string RefVarName { get; set; }
        public Keyword Key { get; set; }

        public VarNameKeyword()
        {
            RefVarName = string.Empty;
            Key = new Keyword(0, string.Empty);
        }
    }

    public class VariableNameSurveys : VariableName
    { 
        public string SurveyList { get; set; }

        public VariableNameSurveys() :base()
        {
            SurveyList = string.Empty;
        }
    }

    

}
