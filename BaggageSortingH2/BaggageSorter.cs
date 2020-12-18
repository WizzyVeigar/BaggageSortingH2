using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaggageSortingH2
{
    //TODO CHECK IF TERMINAL IS OPEN BEFORE PUTTING STUFF ON IT

    class BaggageSorter : IHaveBaggageBuffer
    {
        private Baggage[] baggageBuffer;
        public Baggage[] BaggageBuffer
        {
            get { return baggageBuffer; }
            set { baggageBuffer = value; }
        }

        private List<Terminal> terminals;

        public List<Terminal> Terminals
        {
            get { return terminals; }
            set { terminals = value; }
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
        public BaggageSorter(int maxSize, List<Terminal> terminals)
        {
            BaggageBuffer = new Baggage[maxSize];
            Terminals = terminals;
        }

        //Stamp the baggage here too
        public void SortBaggage()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                if (Monitor.TryEnter(BaggageBuffer))
                {
                    while (GetCurrentBufferAmount() == 0)
                    {
                        //output BaggageBuffer is empty
                        Console.WriteLine("Sorter buffer is empty");
                        Monitor.Wait(BaggageBuffer);
                    }

                    for (int i = 0; i < BaggageBuffer.Length; i++)
                    {
                        //Check null baggage
                        if (BaggageBuffer[i] != null)
                        {
                            if (baggageBuffer[i].Stamp.CheckIn == DateTime.MinValue)
                            {
                                Console.WriteLine(baggageBuffer[i].BaggageId + " has been checkIn stamped, going to " + baggageBuffer[i].Destination);
                                StampBaggage(baggageBuffer[i], true);
                            }

                            for (int j = 0; j < Terminals.Count; j++)
                            {
                                //Find the terminal with the same destination as the baggage
                                if (BaggageBuffer[i].Destination == Terminals[j].Destination)
                                {
                                    if (Monitor.TryEnter(terminals[j], 3000))
                                    {
                                        //Wait for terminal to be fillable
                                        while (Terminals[j].GetCurrentBufferAmount() >= Terminals[j].BaggageBuffer.Length)
                                        {
                                            Console.WriteLine("Waiting for terminal buffer to de-fill");
                                            Monitor.Wait(Terminals[j]);
                                        }

                                        //Loop to find a not null spot
                                        for (int k = 0; k < Terminals[j].BaggageBuffer.Length; k++)
                                        {
                                            //Put into terminal baggageBuffer
                                            if (Terminals[j].BaggageBuffer[k] == null)
                                            {

                                                Console.WriteLine(BaggageBuffer[i].BaggageId + " has been stamped out");
                                                StampBaggage(baggageBuffer[i], false);

                                                //Putting baggage from the buffer to the corret terminal
                                                Terminals[j].BaggageBuffer[k] = BaggageBuffer[i];
                                                BaggageBuffer[i] = null;

                                                k = (Terminals[j].BaggageBuffer.Length) + 1;
                                            }
                                        }

                                        Monitor.Pulse(Terminals[j]);
                                        Monitor.Exit(Terminals[j]);
                                        j = Terminals.Count + 1;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Terminal Unavailable, trying again soon..");
                                    }
                                }
                                else
                                {
                                    //Make check if the Terminal is closed
                                }
                            }
                        }
                        i = BaggageBuffer.Length + 1;
                    }
                    Monitor.Pulse(BaggageBuffer);
                    Monitor.Exit(BaggageBuffer);
                }
            }
        }

        /// <summary>
        /// Stamps the baggage with Datetimes of it's arrival and departure from the sorter
        /// </summary>
        /// <param name="baggage">The baggage which has the <see cref="Baggage.Stamp"/></param>
        /// <param name="isCheckIn">Stamps <see cref="BaggageStamp.CheckIn"/> if true, otherwise stamps <seealso cref="BaggageStamp.SortedOut"/></param>
        public void StampBaggage(Baggage baggage, bool isCheckIn)
        {
            if (isCheckIn)
            {
                baggage.Stamp.CheckIn = DateTime.Now;
            }
            else
            {
                baggage.Stamp.SortedOut = DateTime.Now;
            }
        }
    }
}
