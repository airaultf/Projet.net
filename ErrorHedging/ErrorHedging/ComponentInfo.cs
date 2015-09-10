using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ErrorHedging
{
    public class ComponentInfo : BindableBase
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}
