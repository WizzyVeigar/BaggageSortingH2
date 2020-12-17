using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaggageSortingH2
{
    interface IHaveBaggageBuffer
    {
        Baggage[] BaggageBuffer { get; }
        int GetCurrentBufferAmount();
    }
}
