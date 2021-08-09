// 
// PerformanceTestResults.cs
// 
// MIT License
// 
// Copyright (c) 2021 Andrew Gould
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// 

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
            Random rnd = new Random();

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