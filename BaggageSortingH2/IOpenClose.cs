﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaggageSortingH2
{
    interface IOpenClose
    {
        bool IsOpen { get; set; }
        string Name { get;  }
    }
}
