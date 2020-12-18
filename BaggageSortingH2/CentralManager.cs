using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaggageSortingH2
{
    //Class for opening and closing counter/terminals with User input
    class CentralManager
    {
        private List<IOpenClose> checksNTerminals;
        public List<IOpenClose> ChecksNTerminals
        {
            get { return checksNTerminals; }
            set { checksNTerminals = value; }
        }
        public CentralManager(List<IOpenClose> checksNTerminals)
        {
            ChecksNTerminals = checksNTerminals;
        }

        //public void LogMessage(string message)
        //{
        //    Console.WriteLine(message);
        //}

        /// <summary>
        /// Thread method for listening to user input and closing the appropriate thing
        /// </summary>
        public void StartUserControls()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                string command = Console.ReadLine();
                try
                {
                    for (int i = 0; i < ChecksNTerminals.Count; i++)
                    {
                        if (ChecksNTerminals[i].Name.ToLower() == command)
                        {
                            if (ChecksNTerminals[i].IsOpen)
                            {
                                Console.WriteLine("Changing " + ChecksNTerminals[i].Name + "'s status to closed");
                                ChecksNTerminals[i].IsOpen = false;
                            }
                            else
                            {
                                Console.WriteLine("Changing " + ChecksNTerminals[i].Name + "'s status to open");
                                ChecksNTerminals[i].IsOpen = true;
                            }
                        }
                    }
                }
                catch (ArgumentException e)
                {
                    throw e;
                }
            }
        }


    }
}
