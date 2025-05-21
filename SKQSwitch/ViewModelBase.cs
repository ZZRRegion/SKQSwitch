using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKQSwitch
{
    public class ViewModelBase : ObservableObject
    {
        public bool IsInDesignMode { get; }
        protected ViewModelBase()
        {
            IsInDesignMode = DesignerProperties.GetIsInDesignMode(new());
        }
    }
}
