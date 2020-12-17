using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaggageSortingH2
{
    public enum Destination
    {
        England,
        Japan,
        //Germany,
        //India,
        Australia
    }


    public class Baggage
    {
        private int baggageId;
        private static int incrementer = 3000;

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

        private Destination destination;

        public Destination Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        public Baggage(Destination destination)
        {
            Destination = destination;
            BaggageId = incrementer++;
        }
    }
}
