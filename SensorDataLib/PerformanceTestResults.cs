// 
// PerformanceTestResults.cs
// 
// Copyright 2021 Wingandprayer Software
// 
// This file is part of MazdaApiLib.
// 
// MazdaApiLib is free software: you can redistribute it and/or modify it under the terms of the
// GNU General Public License as published by the Free Software Foundation, either version 2
// of the License, or (at your option) any later version.
// 
// CDRipper is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
// PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with MazdaApiLib.
// If not, see http://www.gnu.org/licenses/.

using System;
using System.Collections.Generic;
using System.Linq;

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
            Random rnd = new();

            _testResults.Add(16);
            _testResults.Add((rnd.Next(350, 600) * 100 - 1) / 100);
            _testResults.Add(59);
            _testResults.Add((rnd.Next(563, 2000) * 100 - 1) / 100);

            int numIterations = rnd.Next(500, 2000) * 100 - 1;

            _testResults.Add(numIterations - 899);
            _testResults.Add(numIterations / 100);

            numIterations = rnd.Next(500, 1500) * 100 - 1;

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