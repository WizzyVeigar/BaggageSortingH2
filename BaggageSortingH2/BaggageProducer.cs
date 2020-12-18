using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaggageSortingH2
{
    //Responsible for producing baggage objects to the counters
    class BaggageProducer
    {
        private List<Counter> counters;
        public List<Counter> Counters
        {
            get { return counters; }
            set { counters = value; }
        }

        private Random random = new Random();

        public BaggageProducer(List<Counter> counters)
        {
            Counters = counters;
        }

        /// <summary>
        /// Produces baggage to the counters
        /// </summary>
        public void ProduceBaggage()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                Counter counter = Counters[random.Next(0, Counters.Count)];

                if (Monitor.TryEnter(counter.BaggageBuffer))
                {
                    if (counter.IsOpen)
                    {
                        if (counter.GetCurrentBufferAmount() >= counter.BaggageBuffer.Length)
                        {
                            //Counter is full, need to wait for it to be emptied
                            Console.WriteLine(counter.Name + " is currently full, waiting for available slot");
                            Monitor.Wait(counter, 2000);
                        }

                        for (int i = 0; i < counter.BaggageBuffer.Length; i++)
                        {
                            if (counter.BaggageBuffer[i] == null)
                            {
                                //Add a random baggage to the counter
                                counter.BaggageBuffer[i] = new Baggage(counter.CounterDestination); //Will need to change if flightSchedule is to be implemented
                                Console.WriteLine(counter.BaggageBuffer[i].BaggageId + " has been sent to " + counter.Name);
                                i = counter.BaggageBuffer.Length + 1;
                            }
                        }
                        Monitor.Pulse(counter.BaggageBuffer);
                        Monitor.Exit(counter.BaggageBuffer);
                    }
                    else
                    {
                        Console.WriteLine(counter.Name + " is current closed");
                        Monitor.Exit(counter.BaggageBuffer);
                        //if (!Monitor.Wait(counter, 200))
                        //{
                        //    Console.WriteLine(counter.Name + " was currently unavailable");
                        //    Monitor.Pulse(counter);
                        //    Monitor.Exit(counter);
                        //}
                    }
                }
                //Make not so fast baggage
                Thread.Sleep(1000);
            }
        }

    }
}
