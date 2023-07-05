using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ITCLib
{
    /// <summary>
    /// Represents a region of the Earth.
    /// </summary>
    public class Region : INotifyPropertyChanged
    {
        public int ID { get; set; }
        public string RegionName { get { return _regionName; } set { if (_regionName != value) { _regionName = value; NotifyPropertyChanged(); } } }
        public string TempVarPrefix { get { return _tempvarprefix; } set { if (_tempvarprefix != value) { _tempvarprefix = value; NotifyPropertyChanged(); } } }
        public List<Study> Studies { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Region()
        {
            RegionName = string.Empty;
            TempVarPrefix = string.Empty;
            Studies = new List<Study>();
        }

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

        private string _regionName;
        private string _tempvarprefix;
        
    }
}
