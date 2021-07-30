// 
// KeyEvents.cs
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
    internal class KeyEvent
    {
        public long EventTime { get; }
        public int CharCodeSum { get; }
        public bool LongerThenBefore { get; }

        public KeyEvent(long eventTime, int charCodeSum, bool longerThenBefore)
        {
            EventTime = eventTime;
            CharCodeSum = charCodeSum;
            LongerThenBefore = longerThenBefore;
        }

        public override string ToString()
        {
            return $"2,{EventTime},{CharCodeSum}{(LongerThenBefore ? ",1" : string.Empty)};";
        }
    }

    internal class KeyEvents
    {
        private readonly List<KeyEvent> _keyEvents;

        public KeyEvents()
        {
            _keyEvents = new List<KeyEvent>();
        }

        public void Randomize(DateTime sensorCollectionStartTimestamp)
        {
            Random rnd = new();
            _keyEvents.Clear();

            if (rnd.Next(0, 20) == 0)
            {
                double msSinceSensorCollectionStarted = (DateTime.UtcNow - sensorCollectionStartTimestamp).TotalMilliseconds;

                if (msSinceSensorCollectionStarted >= 10000)
                {
                    int charCodeSum = rnd.Next(517, 519);

                    for (int i = 0; i < rnd.Next(2, 5); i++)
                    {
                        _keyEvents.Add(new KeyEvent(i == 0 ? rnd.Next(5000, 8000) : rnd.Next(10, 50), charCodeSum, rnd.Next(0, 2) == 0));
                    }
                }
            }
        }

        public int Length => _keyEvents.Count;

        public long GetSum()
        {
            long retval = 0;

            foreach (KeyEvent touchEvent in _keyEvents)
            {
                retval += touchEvent.CharCodeSum;
                retval += touchEvent.EventTime;
                retval += 2;
            }

            return retval;
        }

        public override string ToString()
        {
            return _keyEvents.Select(e => e.ToString()).Aggregate(string.Empty, (first, second) => $"{first}{second}");
        }

    }
}