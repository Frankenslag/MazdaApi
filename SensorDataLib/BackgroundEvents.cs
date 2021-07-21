using System;
using System.Collections.Generic;
using System.Linq;

namespace WingandPrayer.MazdaApi.SensorData
{
    internal class BackgroundEvent
    {
        public int EventType { get; private set; }
        public long EventTime { get; private set; }

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