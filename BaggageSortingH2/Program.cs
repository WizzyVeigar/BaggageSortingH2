using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaggageSortingH2
{
    class Program
    {
        static void Main(string[] args)
        {
            //List of planes
            List<Plane> planes = new List<Plane>()
            {
                //new Plane(2, "PsPlane"),
                //new Plane(2, "NintendoWiiFly"),
                new Plane(2, "XboxPlane")
            };

            //List of terminals
            List<Terminal> terminals = new List<Terminal>()
            {
                //new Terminal(Destination.Australia, 5, "Terminal 1", planes[0]),
                //new Terminal(Destination.England, 3, "Terminal 2", planes[1]),
                new Terminal(Destination.Japan, 10, "Terminal 3", planes[0])
            };

            BaggageSorter sorter = new BaggageSorter(40, terminals);

            List<Counter> counters = new List<Counter>()
            {
                //new Counter(2, "Counter 1", Destination.Australia, sorter),
                //new Counter(2, "Counter 2", Destination.England, sorter),
                new Counter(3, "Counter 3", Destination.Japan, sorter)
            };

            BaggageProducer baggageProducer = new BaggageProducer(counters);

            PlaneAssigner assigner = new PlaneAssigner(planes, terminals);

            //Threads
            Thread planeAssignerThread = new Thread(assigner.AssignPlanes);
            planeAssignerThread.Start();

            Thread baggageProdThread = new Thread(baggageProducer.ProduceBaggage);
            baggageProdThread.Start();

            for (int i = 0; i < counters.Count; i++)
            {
                Thread counterThread = new Thread(counters[i].CounterWork);
                counterThread.Start();
            }
            //counters[1].IsOpen = false;

            Thread sortingThread = new Thread(sorter.SortBaggage);
            sortingThread.Start();

            for (int i = 0; i < terminals.Count; i++)
            {
                Thread terminalThread = new Thread(terminals[i].PlaceBaggageInPlane);
                Thread planeThread = new Thread(terminals[i].PlaneAtTerminal.Start);

                terminalThread.Start();
                planeThread.Start();
            }

            
            //terminals[1].IsOpen = false;

            //All console.Writelines could be replaced with a logger class instance
        }
    }
}
