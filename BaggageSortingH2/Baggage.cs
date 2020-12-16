using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaggageSortingH2
{
    class Baggage
    {
        private int baggageId;

        public int BaggageId
        {
            get { return baggageId; }
            set { baggageId = value; }
        }

        private BaggageStamp stamp;

        public BaggageStamp Stamp
        {
            get { return stamp; }
            set { stamp = value; }
        }

    }
}
