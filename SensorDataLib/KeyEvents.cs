using System;
using System.Collections.Generic;
using System.Linq;

namespace WingandPrayer.MazdaApi.SensorData
{
    internal class KeyEvent
    {
        public long EventTime { get; private set; }
        public int CharCodeSum { get; private set; }
        public bool LongerThenBefore { get; private set; }

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