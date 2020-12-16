using System;

namespace BaggageSortingH2
{
    internal class BaggageStamp
    {
        private DateTime checkIn;

        public DateTime CheckIn
        {
            get { return checkIn; }
            set { checkIn = value; }
        }

        private DateTime sortedOut;

        public DateTime SortedOut
        {
            get { return sortedOut; }
            set { sortedOut = value; }
        }


    }
}