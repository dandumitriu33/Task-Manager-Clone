using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Concurrent;

namespace ProcessNote
{
    public class CustomProcess
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public long CPU { get; set; }
        public long Memory { get; set; }
        public string Started { get; set; }
        public long Thread { get; set; }
        public static List<CustomProcess> Stats { get; set; }
        public static Dictionary<int, long> History = new Dictionary<int, long>();
        public static Dictionary<int, string> Notes = new Dictionary<int, string>();

        /// <summary>
        /// Populates the list of Custom Processes 'Stats' used by xaml for display 
        /// </summary>
        /// <returns></returns>
        public static void PopulateStats()
        {
            List<CustomProcess> result = new List<CustomProcess>();
            ConcurrentBag<CustomProcess> tempBag = new ConcurrentBag<CustomProcess>();
            Parallel.ForEach(Process.GetProcesses(),
                item => 
                {
                    // Some data is not accessible due to security, simulations in place
                    long cpu = 0;
                    if (History.Count() <= 0)
                    {
                        Random randomPercent = new Random();
                        cpu = randomPercent.Next(5, 17);
                    }
                    else
                    {
                        Random randomPositiveNegative = new Random();
                        var values = new[] { 2, -2, 1, -1, 1, 1, 1, -1, -1, -1 };
                        int randomPercent = values[randomPositiveNegative.Next(values.Length)];
                        cpu = findPreviousCPUValue(item.Id) + randomPercent;
                    }
                    string startTime = "00";
                    try
                    {
                        startTime = Convert.ToString(item.StartTime);
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine(e.Message);
                        startTime = "6/15/2020 8:45:61 PM";
                    }
                    tempBag.Add(new CustomProcess()
                    {
                        ID = item.Id,
                        Name = item.ProcessName,
                        Note = verifyNote(item.Id),
                        CPU = cpu,
                        Memory = Convert.ToInt64(item.WorkingSet64),
                        Started = startTime,
                        Thread = Convert.ToInt64(item.Threads.Count)
                    });
                });
            foreach (var item in tempBag)
            {
                result.Add(item);
            }
            Stats = result;
            populateHistory(result);



        }
        //    List<CustomProcess> result = new List<CustomProcess>();
        //    Process[] remoteAll = Process.GetProcesses());
        //    foreach (var item in remoteAll)
        //    {
        //        // Some data is not accessible due to security, simulations in place
        //        long cpu = 0;
        //        if (History.Count() <= 0)
        //        {
        //            Random randomPercent = new Random();
        //            cpu = randomPercent.Next(5, 17);
        //        }
        //        else
        //        {
        //            Random randomPositiveNegative = new Random();
        //            var values = new[] { 2, -2, 1, -1, 1, 1, 1, -1, -1, -1 };
        //            int randomPercent = values[randomPositiveNegative.Next(values.Length)];
        //            cpu = findPreviousCPUValue(item.Id) + randomPercent;
        //        }
        //        string startTime = "00";
        //        try
        //        {
        //            startTime = Convert.ToString(item.StartTime);
        //        }
        //        catch (Exception e)
        //        {
        //            //Console.WriteLine(e.Message);
        //            startTime = "6/15/2020 8:45:61 PM";
        //        }
        //        result.Add(new CustomProcess()
        //        {
        //            ID = item.Id,
        //            Name = item.ProcessName,
        //            Note = verifyNote(item.Id),
        //            CPU = cpu,
        //            Memory = Convert.ToInt64(item.WorkingSet64),
        //            Started = startTime,
        //            Thread = Convert.ToInt64(item.Threads.Count)
        //        });
        //    }
        //    Stats = result;
        //    populateHistory(result);
        //}

        /// <summary>
        /// Checks if there is a note for the process ID in the Notes and returns the text
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static string verifyNote(int id)
        {
            string note = "...";
            if (Notes.ContainsKey(id))
            {
                note = Notes[id];
            }
            return note;
        }

        /// <summary>
        /// Populates the 'History' dictionary. It is a Dictionary that holds
        /// the previous tick CPU data so it can be incremented by the simulation
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private static void populateHistory(List<CustomProcess> result)
        {
            foreach (var item in result)
            {
                if (History.ContainsKey(item.ID))
                {
                    History[item.ID] = item.CPU;
                }
                else
                {
                    History.Add(item.ID, item.CPU);
                }
            }
            Console.WriteLine("history populated");
        }

        /// <summary>
        /// Retrieves from the 'History' dictionary the previous tick CPU data by 
        /// process ID. Used by the CPU simulation mechanism.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static long findPreviousCPUValue(int id)
        {
            long tempResult = 0;
            if (History.ContainsKey(id))
            {
                tempResult = History[id];
            }
            return tempResult;
        }
    }
}