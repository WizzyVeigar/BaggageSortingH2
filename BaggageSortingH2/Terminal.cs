using System.Threading;
using System;

namespace BaggageSortingH2
{
    public class Terminal : IHaveBaggageBuffer, IOpenClose
    {
        public Destination Destination { get; set; }

        private Baggage[] baggageBuffer;
        public Baggage[] BaggageBuffer
        {
            get { return baggageBuffer; }
            set { baggageBuffer = value; }
        }

        private string terminalName;
        public string TerminalName
        {
            get { return terminalName; }
            set { terminalName = value; }
        }

        private bool isOpen;

        public bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; }
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
        public Terminal(Destination terminalFor, int maxSize, string terminalName)
        {
            Destination = terminalFor;
            BaggageBuffer = new Baggage[maxSize];
            TerminalName = terminalName;
            IsOpen = true;
        }

        public void PlaceBaggageInPlane()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                if (Monitor.TryEnter(BaggageBuffer))
                {
                    if (GetCurrentBufferAmount() == 0)
                    {
                        Console.WriteLine("Waiting for baggage to arrive to the terminal");
                        Monitor.Wait(BaggageBuffer, 1000);
                    }

                    //Simulate it goes on an airplane
                    if (GetCurrentBufferAmount() >= BaggageBuffer.Length)
                    {
                        //Clear the Array
                        for (int i = 0; i < BaggageBuffer.Length; i++)
                        {
                            Console.WriteLine(BaggageBuffer[i].BaggageId + " has been put on the plane");
                            Console.WriteLine("     CheckIn:   " + BaggageBuffer[i].Stamp.CheckIn);
                            Console.WriteLine("\n   CheckOut:   " + BaggageBuffer[i].Stamp.SortedOut);
                            BaggageBuffer[i] = null;
                        }
                        Console.WriteLine("Plane to " + Enum.GetName(typeof(Destination), Destination) + " is leaving the airport \n\n\n");
                        IsOpen = false;

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
                        IsOpen = true;
                        Console.WriteLine("The plane from " + Enum.GetName(typeof(Destination), Destination) + " has returned.\n");
                    }
                    Monitor.Pulse(BaggageBuffer);
                    Monitor.Exit(BaggageBuffer);
                }
            }
        }
    }
}