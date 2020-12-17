using System;

namespace BaggageSortingH2
{
    public class BaggageStamp
    {
        private static int stampId;

        public static int StampId
        {
            get { return stampId; }
            set { stampId = value; }
        }


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

        public BaggageStamp()
        {
            StampId = StampId++;
        }
    }
}