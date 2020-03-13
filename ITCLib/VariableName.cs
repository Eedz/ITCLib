using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace ITCLib
{
    public class VariableName : INotifyPropertyChanged
    {
        public string FullVarName { get; set; }
        public string RefVarName { get; set; }

        // labels
        #region labels
        
        public DomainLabel Domain
        {
            get { return _domain; }
            set
            {
                if (value != _domain)
                {
                    _domain = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public TopicLabel Topic
        {
            get { return _topic; }
            set
            {
                if (value != _topic)
                {
                    _topic = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ContentLabel Content
        {
            get { return _content; }
            set
            {
                if (value != _content)
                {
                    _content = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ProductLabel Product
        {
            get { return _product; }
            set
            {
                if (value != _product)
                {
                    _product = value;
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

        #endregion

        public VariableName()
        {
            FullVarName = "";

            RefVarName = "";

            VarLabel = "";
            Domain = new DomainLabel(0, "No Domain");
            Topic = new TopicLabel(0, "No Topic");
            Content = new ContentLabel(0, "No Content");
            Product = new ProductLabel(0, "Unassigned");
        }

        public VariableName(string varname)
        {
            FullVarName = varname;

            RefVarName = Utilities.ChangeCC(varname);

            VarLabel = "";
            Domain = new DomainLabel(0, "No Domain");
            Topic = new TopicLabel(0, "No Topic");
            Content = new ContentLabel(0, "No Content");
            Product = new ProductLabel(0, "Unassigned");
        }

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public override string ToString()
        {
            return FullVarName;
        }

        private DomainLabel _domain;
        private TopicLabel _topic;
        private ContentLabel _content;
        private ProductLabel _product;
        private string _varlabel;
    }

    public class RefVariableName
    {

        public string refVarName { get; set; }
        public string VarLabel { get; set; }
        public DomainLabel Domain { get; set; }
        public TopicLabel Topic { get; set; }
        public ContentLabel Content { get; set; }
        public ProductLabel Product { get; set; }


        public RefVariableName()
        {
            refVarName = "";

            VarLabel = "";
            Domain = new DomainLabel(0, "No Domain");
            Topic = new TopicLabel(0, "No Topic");
            Content = new ContentLabel(0, "No Content");
            Product = new ProductLabel(0, "Unassigned");
        }

        public RefVariableName(string refvarname)
        {
            refVarName = refvarname;

            VarLabel = "";
            Domain = new DomainLabel(0, "No Domain");
            Topic = new TopicLabel(0, "No Topic");
            Content = new ContentLabel(0, "No Content");
            Product = new ProductLabel(0, "Unassigned");
        }

        public override string ToString()
        {
            return refVarName;
        }
    }
}
