// //
// // BackgroundEvents.cs
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