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

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private bool isOpen;
        public bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; }
        }

        private Plane planeAtTerminal;

        public Plane PlaneAtTerminal
        {
            get { return planeAtTerminal; }
            set { planeAtTerminal = value; }
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
            Name = terminalName;
            IsOpen = true;
        }
        public Terminal(Destination terminalFor, int maxSize, string terminalName, Plane plane) : this(terminalFor, maxSize, terminalName)
        {
            PlaneAtTerminal = plane;
            PlaneAtTerminal.IsAvailable = false;
            PlaneAtTerminal.Destination = terminalFor;
        }

        /// <summary>
        /// Terminal Thread method for placing baggage on <see cref="PlaneAtTerminal"/>
        /// </summary>
        public void PlaceBaggageInPlane()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                if (Monitor.TryEnter(this))
                {
                    do
                    {
                        Monitor.Wait(this);
                    } while (!IsOpen);

                    //Needs a fix
                    while (PlaneAtTerminal == null)
                    {
                        Console.WriteLine("Waiting for plane at " + Name);
                        Monitor.Wait(this, 200);
                    }

                    while (GetCurrentBufferAmount() == 0)
                    {
                        Console.WriteLine("Waiting for baggage to arrive at " + Name);
                        Monitor.Wait(this, 200);
                    }

                    

                    //Lock the plane buffer
                    if (Monitor.TryEnter(PlaneAtTerminal.BaggageBuffer))
                    {

                        //Check if the plane is full
                        if (PlaneAtTerminal.GetCurrentBufferAmount() < PlaneAtTerminal.BaggageBuffer.Length)
                        {
                            //Find not null baggage
                            for (int i = 0; i < BaggageBuffer.Length; i++)
                            {
                                if (BaggageBuffer[i] != null)
                                {
                                    for (int j = 0; j < PlaneAtTerminal.BaggageBuffer.Length; j++)
                                    {
                                        //Find a spot in plane's baggage array
                                        if (PlaneAtTerminal.BaggageBuffer[j] == null)
                                        {
                                            Console.WriteLine(BaggageBuffer[i].BaggageId + " has been put on the plane to " + Destination + "\n" +
                                            "     CheckIn:   " + BaggageBuffer[i].Stamp.CheckIn + "\n" +
                                            "     CheckOut:   " + BaggageBuffer[i].Stamp.SortedOut);

                                            PlaneAtTerminal.BaggageBuffer[j] = BaggageBuffer[i];
                                            j = PlaneAtTerminal.BaggageBuffer.Length + 1; //End the loop

                                            BaggageBuffer[i] = null;

                                            //check if loop needs to continue
                                            if (PlaneAtTerminal.GetCurrentBufferAmount() >= PlaneAtTerminal.BaggageBuffer.Length)
                                            {
                                                i = BaggageBuffer.Length + 1; //End the other loop
                                                Monitor.PulseAll(PlaneAtTerminal.BaggageBuffer);
                                                Monitor.Exit(PlaneAtTerminal.BaggageBuffer);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            PlaneAtTerminal = null;
                        }
                        Monitor.Pulse(this);
                        Monitor.Exit(this);
                        //else
                        //{

                        //    Monitor.Wait(PlaneAtTerminal);
                        //    PlaneAtTerminal = null;
                        //}
                    }
                }
            }
        }
    }
}