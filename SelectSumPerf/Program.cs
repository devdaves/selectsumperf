using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectSumPerf
{
    class Program
    {
        private static Random random = new Random();

        static void Main(string[] args)
        {
            var iterations = 1000;
            var numberOfRows = 100000;
            Console.WriteLine("Generating {0:N0} rows of data", numberOfRows);
            var data = GenerateRows(numberOfRows);

            Console.WriteLine("Calculating time based on {0:N0} iterations", iterations);
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");

            var percentChanges = new List<double>();

            for (int i = 0; i < 10; i++)
            {
                var selectThenSumMs = CalculateAverageTime(iterations, SelectThenSum, data);
                var sumWithSelectMs = CalculateAverageTime(iterations, SumWithoutSelect, data);
                var change = ((selectThenSumMs - sumWithSelectMs) / Math.Abs(sumWithSelectMs));// * 100;

                percentChanges.Add(change);

                Console.WriteLine("Select Then Sum = {0}ms", selectThenSumMs);
                Console.WriteLine("Sum Without Select = {0}ms", sumWithSelectMs);
                Console.WriteLine("% change = {0:P1}", change);
                Console.WriteLine("======================"); 
            }
            
            Console.WriteLine("Average Percent Change {0:P1}", percentChanges.Average());

            Console.WriteLine("Done, press enter");
            Console.ReadLine();
        }

        private static List<DataRow> GenerateRows(int howMany)
        {
            return Enumerable.Range(0, howMany).Select(CreateRow).ToList();
        }

        private static DataRow CreateRow(int id)
        {
            return new DataRow
            {
                Id = id,
                Number = random.Next(1, 100),
                SomeThing1 = Guid.NewGuid().ToString(),
                SomeThing2 = Guid.NewGuid().ToString()
            };
        }

        private static int SelectThenSum(List<DataRow> data)
        {
            return data.Select(x => x.Number).Sum();
        }

        private static int SumWithoutSelect(List<DataRow> data)
        {
            return data.Sum(x => x.Number);
        }

        private static double CalculateAverageTime(int iterations, Func<List<DataRow>, int> sumMethod, List<DataRow> data)
        {
            var elapsedTimes = new List<TimeSpan>();

            Enumerable.Range(0, iterations).ToList().ForEach(x => elapsedTimes.Add(TimedExecution(sumMethod, data)));

            return elapsedTimes.Average(x => x.TotalMilliseconds);
        }

        private static TimeSpan TimedExecution(Func<List<DataRow>, int> sumMethod, List<DataRow> data)
        {
            var sw = Stopwatch.StartNew();

            sumMethod.Invoke(data);

            sw.Stop();
            return sw.Elapsed;
        }
    }
}
