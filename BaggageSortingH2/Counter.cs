using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaggageSortingH2
{

    class Counter : IHaveBaggageBuffer, IOpenClose
    {
        private bool isOpen;
        public bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private Baggage[] counterBaggage;
        public Baggage[] BaggageBuffer
        {
            get { return counterBaggage; }
            set { counterBaggage = value; }
        }

        private Destination counterDestination;
        public Destination CounterDestination
        {
            get { return counterDestination; }
            set { counterDestination = value; }
        }

        private BaggageSorter sorter;
        public BaggageSorter Sorter
        {
            get { return sorter; }
            set { sorter = value; }
        }

        public Counter(int maxSize, string name, Destination gateFor, BaggageSorter baggageSorter)
        {
            BaggageBuffer = new Baggage[maxSize];
            Name = name;
            CounterDestination = gateFor;
            IsOpen = true;
            Sorter = baggageSorter;
        }

        public int GetCurrentBufferAmount()
        {
            int amount = 0;
            for (int i = 0; i < BaggageBuffer.Length; i++)
            {
                if (BaggageBuffer[i] != null)
                {
                    amount++;
                }
            }
            return amount;
        }

        public void CounterWork()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                while (IsOpen)
                {
                    if (Monitor.TryEnter(BaggageBuffer))
                    {
                        if (IsOpen)
                        {
                            while (GetCurrentBufferAmount() == 0)
                            {
                                Monitor.Wait(BaggageBuffer, 2000);
                            }
                        }

                        if (Monitor.TryEnter(Sorter.BaggageBuffer))
                        {
                            if (Sorter.GetCurrentBufferAmount() >= Sorter.BaggageBuffer.Length)
                            {
                                Console.WriteLine("Sorter is currently full, waiting to insert");
                                Monitor.Wait(Sorter.BaggageBuffer, 1000);
                            }

                            for (int i = 0; i < BaggageBuffer.Length; i++)
                            {
                                if (BaggageBuffer[i] != null)
                                {
                                    //Find spot in the sorter's buffer
                                    for (int j = 0; j < Sorter.BaggageBuffer.Length; j++)
                                    {
                                        if (Sorter.BaggageBuffer[j] == null)
                                        {
                                            Console.WriteLine(BaggageBuffer[i].BaggageId + " taken from counter, to the sorter");
                                            BaggageBuffer[i].Stamp = new BaggageStamp(); //Give the baggage a baggageStamp
                                            Sorter.BaggageBuffer[j] = BaggageBuffer[i];
                                            BaggageBuffer[i] = null;

                                            i = BaggageBuffer.Length + 1;
                                            j = Sorter.BaggageBuffer.Length + 1;

                                            Monitor.Pulse(Sorter.BaggageBuffer);
                                            Monitor.Exit(Sorter.BaggageBuffer);
                                        }
                                    }
                                }

                            }
                            Monitor.Pulse(BaggageBuffer);
                            Monitor.Exit(BaggageBuffer);
                        }
                    }
                }
            }
        }
    }
}
