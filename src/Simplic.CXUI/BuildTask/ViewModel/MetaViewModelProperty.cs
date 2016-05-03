using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CXUI.BuildTask.ViewModel
{
    public class MetaViewModelProperty
    {
        public string Name
        {
            get;
            set;
        }

        public string Type
        {
            get;
            set;
        }

        public bool NotifyPropertyChanged
        {
            get;
            set;
        }

        public bool SetIsDirty
        {
            get;
            set;
        }

        public bool SetForceSave
        {
            get;
            set;
        }

        public object DefaultValue
        {
            get;
            set;
        }
    }
}
