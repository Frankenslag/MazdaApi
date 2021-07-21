using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingandPrayer.MazdaApi.SensorData
{
    internal class PerformanceTestResults
    {
        private readonly List<int> _testResults;

        public PerformanceTestResults()
        {
            _testResults = new List<int>();
            Randomize();
        }

        private void Randomize()
        {
            int numIterations;
            Random rnd = new();

            _testResults.Add(16);
            _testResults.Add(((rnd.Next(350, 600) * 100) - 1) / 100);
            _testResults.Add(59);
            _testResults.Add(((rnd.Next(563, 2000) * 100) - 1) / 100);

            numIterations = (rnd.Next(500, 2000) * 100) - 1;

            _testResults.Add(numIterations - 899);
            _testResults.Add(numIterations / 100);

            numIterations = (rnd.Next(500, 1500) * 100) - 1;

            _testResults.Add(numIterations);
            _testResults.Add(numIterations / 100);

            _testResults.Add(rnd.Next(8500, 16000));
        }

        public override string ToString()
        {
            return _testResults.Select(e => e.ToString()).Aggregate((first, second) => $"{first},{second}");
        }
    }
}