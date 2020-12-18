using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaggageSortingH2
{
    public class Plane : IHaveBaggageBuffer
    {
        private Baggage[] baggageBuffer;
        public Baggage[] BaggageBuffer
        {
            get { return baggageBuffer; }
            set { baggageBuffer = value; }
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

        private Destination destination;
        public Destination Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        private bool isAvailable;

        public bool IsAvailable
        {
            get { return isAvailable; }
            set { isAvailable = value; }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Plane(int maxSize, string name)
        {
            BaggageBuffer = new Baggage[maxSize];
            Name = name;
            IsAvailable = true;
        }
        public Plane(int maxSize, string name, Destination destination) : this(maxSize, name)
        {
            Destination = destination;
        }

        /// <summary>
        /// Thread method for a plane
        /// </summary>
        public void Start()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                //I don't know what goes wrong in this area, but if this is fixed, planes should work as intended
                if (Monitor.TryEnter(BaggageBuffer))
                {
                    while (GetCurrentBufferAmount() != BaggageBuffer.Length)
                    {
                        if (!Monitor.Wait(BaggageBuffer, 200))
                        {
                            Monitor.Exit(BaggageBuffer);
                        }
                        Console.WriteLine("Why");
                    }

                    Console.WriteLine(Name + " that is going to " + Enum.GetName(typeof(Destination), Destination) + " is leaving the airport \n\n");

                    switch (Destination)
                    {
                        case Destination.England:
                            Thread.Sleep(TimeSpan.FromSeconds(4));
                            break;
                        case Destination.Japan:
                            Thread.Sleep(TimeSpan.FromSeconds(7));
                            break;
                        case Destination.Australia:
                            Thread.Sleep(TimeSpan.FromSeconds(8));
                            break;
                        default:
                            Thread.Sleep(TimeSpan.FromSeconds(5));
                            break;
                    }

                    for (int i = 0; i < BaggageBuffer.Length; i++)
                    {
                        Console.WriteLine("STUFF DID STUFFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
                        BaggageBuffer[i] = null;
                    }

                    IsAvailable = true; //you ran out of pixie dust
                    Console.WriteLine(Name +" that went from " + Enum.GetName(typeof(Destination), Destination) + " has returned.\n");

                    Monitor.Pulse(this);
                    Monitor.Exit(this);
                }
            }
        }
    }
}
