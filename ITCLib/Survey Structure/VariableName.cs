﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace ITCLib
{
    public class VariableName : RefVariableName, INotifyPropertyChanged
    {
        //public int new ID { get; set; } // TODO implement in database
        
        public string CountryCode { get; set; } // TODO unused

        public string VarName
        {
            get { return _varname; }
            set
            {
                if (value != _varname)
                {
                    _varname = value; 
                    RefVarName = Utilities.ChangeCC(VarName);
                }
            }
        }

        private string _varname;

        public string RefVarLabel { get { return RefVarName + " - " + VarLabel; } }
     
        public DomainLabel Domain
        {
            get { return _domain; }
            set
            {
                if (_domain == null || !_domain.Equals(value))
                {
                    _domain = value ?? new DomainLabel(0, "No Domain");
                    NotifyPropertyChanged();
                }
            }
        }

        public TopicLabel Topic
        {
            get { return _topic; }
            set
            {
                if (_topic==null || !_topic.Equals(value))
                {
                    _topic = value ?? new TopicLabel(0, "No Topic");
                    NotifyPropertyChanged();
                }
            }
        }

        public ContentLabel Content
        {
            get { return _content; }
            set
            {
                if (_content == null  || !_content.Equals(value))
                {
                    _content = value ?? new ContentLabel(0, "No Content");
                    NotifyPropertyChanged();
                }
            }
        }

        public ProductLabel Product
        {
            get { return _product; }
            set
            {
                if (_product == null || !_product.Equals(value))
                {
                    _product = value ?? new ProductLabel(0, "Unassigned");
                    NotifyPropertyChanged();
                }
            }
        }
        
        public string VarLabel
        {
            get { return _varlabel; }
            set
            {
                if (value != _varlabel)
                {
                    _varlabel = value;
                    NotifyPropertyChanged();
                }
            }
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

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion


        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        private DomainLabel _domain;
        private TopicLabel _topic;
        private ContentLabel _content;
        private ProductLabel _product;
        private string _varlabel;
    }

    public class RefVariableName
    {
        public int ID { get; set; } // TODO to implement in database
        public string RefVarName { get { return _refVarName; }
            set
            {
                if (_refVarName !=value)
                {
                    _refVarName = value;
                    SetParts();
                }
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
            if (_refVarName.Length < 5)
            {
                StandardForm = false;
                Prefix = string.Empty;
                Number = string.Empty;
                Suffix = string.Empty;
                return;
            }

            Prefix = _refVarName.Substring(0, 2);
            if (!(char.IsLetter(Prefix[0]) && char.IsLetter(Prefix[1])))
            {
                Prefix = string.Empty;
                Number = string.Empty;
                Suffix = string.Empty;
                StandardForm = false;
                return;
            }

            if (Int32.TryParse(_refVarName.Substring(2, 3), out int n))
                Number = n.ToString();
            else
            {
                Prefix = string.Empty;
                Number = string.Empty;
                Suffix = string.Empty;
                StandardForm = false;
                return;
            }

            if (_refVarName.Length >= 6)
            {
                Suffix = _refVarName.Substring(5);
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

        public string _refVarName;

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
