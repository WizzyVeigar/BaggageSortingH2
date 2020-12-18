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
        /// <summary>
        /// Get the current amount of not null values
        /// </summary>
        /// <returns>Returns the amount of not null values</returns>
        int GetCurrentBufferAmount();
    }
}
