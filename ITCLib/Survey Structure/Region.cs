using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ITCLib
{
    /// <summary>
    /// Represents a region of the Earth.
    /// </summary>
    public class Region : ObservableObject
    {
        public int ID 
        { 
            get => _id;  
            set =>SetProperty(ref _id, value); 
        }
        
        public string RegionName
        {
            get => _regionName;
            set => SetProperty(ref _regionName, value);
        }

        public string TempVarPrefix
        {
            get => _tempvarprefix;
            set => SetProperty(ref _tempvarprefix, value);
        }

        public List<Study> Studies { get; set; }

        public Region()
        {
            RegionName = string.Empty;
            TempVarPrefix = string.Empty;
            Studies = new List<Study>();
        }

        private int _id;
        private string _regionName;
        private string _tempvarprefix;
        
    }
}
