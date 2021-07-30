// 
// TouchEvents.cs
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
    internal class TouchEvent
    {
        public int EventType { get; }
        public long EventTime { get; }
        public int PointerCount { get; }
        public int ToolType { get; }

        public TouchEvent(int eventType, long eventTime, int pointerCount, int toolType)
        {
            EventType = eventType;
            EventTime = eventTime;
            PointerCount = pointerCount;
            ToolType = toolType;
        }

        public override string ToString()
        {
            return $"{EventType},{EventTime},0,0,{PointerCount},1,{ToolType},-1;";
        }
    }

    internal class TouchEvents
    {
        private readonly List<TouchEvent> _touchEvents;

        public TouchEvents()
        {
            _touchEvents = new List<TouchEvent>();
        }

        public void Randomize(DateTime sensorCollectionStartTimestamp)
        {
            Random rnd = new();
            _touchEvents.Clear();

            long msSinceSensorCollectionStarted = (long)(DateTime.UtcNow - sensorCollectionStartTimestamp).TotalMilliseconds;

            switch (msSinceSensorCollectionStarted)
            {
                case >= 3000 and < 5000:
                {
                    // down event
                    _touchEvents.Add(new TouchEvent(2, msSinceSensorCollectionStarted - rnd.Next(1000, 2000), 1, 1));

                    // move events
                    for (int i = 0; i < rnd.Next(2, 9); i++)
                    {
                        _touchEvents.Add(new TouchEvent(1, rnd.Next(3, 50), 1, 1));
                    }

                    // up event
                    _touchEvents.Add(new TouchEvent(3, rnd.Next(3, 100), 1, 1));
                    break;
                }
                case >= 5000 and < 10000:
                {
                    for (int i = 0; i < 2; i++)
                    {
                        // down event
                        _touchEvents.Add(new TouchEvent(2, rnd.Next(100, 1000) + (i == 0 ? 0 : 5000), 1, 1));
                    }

                    for (int i = 0; i < rnd.Next(2, 9); i++)
                    {
                        // move events
                        _touchEvents.Add(new TouchEvent(1, rnd.Next(3, 50), 1, 1));
                    }

                    // up event
                    _touchEvents.Add(new TouchEvent(3, rnd.Next(3, 100), 1, 1));
                    break;
                }
                case >= 10000:
                {
                    for (int j = 0; j < 3; j++)
                    {
                        long tso = j == 0 ? msSinceSensorCollectionStarted - 9000 : rnd.Next(2000, 3000);

                        // down event
                        _touchEvents.Add(new TouchEvent(2, rnd.Next(100, 1000) + tso, 1, 1));

                        for (int i = 0; i < rnd.Next(2, 9); i++)
                        {
                            // move events
                            _touchEvents.Add(new TouchEvent(1, rnd.Next(3, 50), 1, 1));
                        }

                        // up event
                        _touchEvents.Add(new TouchEvent(3, rnd.Next(3, 100), 1, 1));
                    }

                    break;
                }
            }
        }
        public int Length => _touchEvents.Count; 

        public long GetSum()
        {
            long retval = 0;

            foreach(TouchEvent touchEvent in _touchEvents)
            {
                retval += touchEvent.EventType;
                retval += touchEvent.EventTime;
            }

            return retval;
        }

        public override string ToString()
        {
            return _touchEvents.Select(e => e.ToString()).Aggregate(string.Empty, (first, second) => $"{first}{second}");
        }
    }
}
