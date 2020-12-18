using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaggageSortingH2
{
    //Finds available planes and assigns them to available terminals
    class PlaneAssigner
    {
        private List<Plane> planes;
        public List<Plane> Planes
        {
            get { return planes; }
            set { planes = value; }
        }

        private List<Terminal> terminals;

        public List<Terminal> Terminals
        {
            get { return terminals; }
            set { terminals = value; }
        }
        public PlaneAssigner(List<Plane> planes, List<Terminal> terminals)
        {
            Planes = planes;
            Terminals = terminals;
        }


        /// <summary>
        /// Checks if any planes are available, then tries to assign it to a terminal.
        /// </summary>
        public void AssignPlanes()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                //Loop through terminals
                for (int i = 0; i < Terminals.Count; i++)
                {
                    if (Terminals[i].PlaneAtTerminal == null)
                    {
                        if (Monitor.TryEnter(Terminals[i]))
                        {
                            //Loop through planes
                            for (int j = 0; j < Planes.Count; j++)
                            {
                                if (Planes[j].IsAvailable)
                                {
                                    if (Monitor.TryEnter(Planes[j]))
                                    {
                                        Planes[j].IsAvailable = false; //Make it unavailable for other terminals
                                        Console.WriteLine("Assigned " + Planes[j].Name + " to " + Terminals[i].Name);
                                        Terminals[i].PlaneAtTerminal = Planes[j];

                                        Monitor.Pulse(Terminals[i]);
                                        Monitor.Exit(Terminals[i]);

                                        Monitor.Pulse(Planes[j]);
                                        Monitor.Exit(Planes[j]);

                                        i = Terminals.Count + 1;
                                        j = Planes.Count + 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
