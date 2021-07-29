// //
// // TouchEvents.cs
// //
// // Copyright 2021 Wingandprayer Software
// //
// // This file is part of MazdaApiLib.
// //
// // MazdaApiLib is free software: you can redistribute it and/or modify it under the terms of the
// // GNU General Public License as published by the Free Software Foundation, either version 2
// // of the License, or (at your option) any later version.
// //
// // CDRipper is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// // without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
// // PURPOSE. See the GNU General Public License for more details.
// //
// // You should have received a copy of the GNU General Public License along with MazdaApiLib.
// // If not, see http://www.gnu.org/licenses/.
// //

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WingandPrayer.MazdaApi.SensorData
{
    internal class TouchEvent
    {
        public int EventType { get; private set; }
        public long EventTime { get; private set; }
        public int PointerCount { get; private set; }
        public int ToolType { get; private set; }

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

            if (msSinceSensorCollectionStarted >= 3000 && msSinceSensorCollectionStarted < 5000)
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
            }
            else if (msSinceSensorCollectionStarted >= 5000 && msSinceSensorCollectionStarted < 10000)
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
            }
            else if (msSinceSensorCollectionStarted >= 10000)
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
