using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ITCLib
{
    public class VarNameLabel : ObservableObject 
    {
        private int _id;
        private string _label;
        private int _uses;
        
        public int ID { get => _id; set => SetProperty(ref _id, value); }
        public string Label { get => _label; set => SetProperty(ref _label, value); }
        public int Uses { get => _uses; set => SetProperty(ref _uses, value); }

        public VarNameLabel() 
        {
            Label = string.Empty;
        }

        public VarNameLabel (int id, string label)
        {
            ID = id;
            Label = label;
        }

        public override string ToString()
        {
            return Label;
        }

        public override bool Equals(object obj)
        {
            var label = obj as VarNameLabel;
            return label != null &&
                   ID == label.ID &&
                   Label == label.Label;
        }

        public override int GetHashCode()
        {
            var hashCode = -244446586;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Label);
            return hashCode;
        }
    }
}
