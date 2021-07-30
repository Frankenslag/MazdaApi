// 
// BackgroundEvents.cs
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
    internal class BackgroundEvent
    {
        public int EventType { get; }
        public long EventTime { get; }

        public BackgroundEvent(int eventType, long eventTime)
        {
            EventType = eventType;
            EventTime = eventTime;
        }

        public override string ToString()
        {
            return $"{EventType},{EventTime}";
        }
    }

    internal class BackgroundEvents
    {
        private readonly List<BackgroundEvent> _backgroundEvents;

        public BackgroundEvents()
        {
            _backgroundEvents = new List<BackgroundEvent>();
        }

        public void Randomize(DateTime sensorCollectionStartTimestamp)
        {
            Random rnd = new();
            _backgroundEvents.Clear();

            if (rnd.Next(0, 10) == 0)
            {
                long msSinceSensorCollectionStarted = (long)(DateTime.UtcNow - sensorCollectionStartTimestamp).TotalMilliseconds;

                if (msSinceSensorCollectionStarted >= 10000)
                {
                    long pausedTimestamp = new DateTimeOffset(sensorCollectionStartTimestamp).ToUnixTimeMilliseconds() + rnd.Next(800, 4500);
                    _backgroundEvents.Add(new BackgroundEvent(2, pausedTimestamp));
                    _backgroundEvents.Add(new BackgroundEvent(3, pausedTimestamp + rnd.Next(2000, 5000)));
                }
            }
        }

        public override string ToString()
        {
            return _backgroundEvents.Select(e => e.ToString()).Aggregate(string.Empty, (first, second) => $"{first}{second}");
        }

    }
}