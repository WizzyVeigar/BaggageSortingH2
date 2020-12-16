using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaggageSortingH2
{
    //Klasse for check in, Sætter et BaggageStamp på baggagen, som den kommer ind
    class Counter
    {
        private bool isOpen;
        public bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; }
        }

    }
}
