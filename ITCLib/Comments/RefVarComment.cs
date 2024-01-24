using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class RefVarComment : Comment
    {
        public string RefVarName { get => _refVarName; set => SetProperty(ref _refVarName, value); }

        public RefVarComment() : base()
        {
            RefVarName = string.Empty;
        }

        private string _refVarName;
    }
}
