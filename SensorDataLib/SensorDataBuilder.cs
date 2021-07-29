// //
// // SensorDataBuilder.cs
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
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;


namespace WingandPrayer.MazdaApi.SensorData
{
    public class SensorDataBuilder
    {
        private const string SdkVersion = "2.2.3";

        private int _deviceInfoTime;
        private DateTime _sensorCollectionStartTimestamp;
        private SystemInfo _systemInfo;
        private TouchEvents _touchEvents;
        private KeyEvents _keyEvents;
        private BackgroundEvents _backgroundEvents;
        private PerformanceTestResults _performanceTestResults;
        private SensorDataEncryptor _sensorDataEncryptor;

        public SensorDataBuilder()
        {
            _sensorCollectionStartTimestamp = DateTime.UtcNow;
            _deviceInfoTime = new Random().Next(3, 8) * 1000;
            _systemInfo = new SystemInfo();
            _touchEvents = new TouchEvents();
            _keyEvents = new KeyEvents();
            _backgroundEvents = new BackgroundEvents();
            _performanceTestResults = new PerformanceTestResults();
            _sensorDataEncryptor = new SensorDataEncryptor();
        }

        public string GenerateSensorData()
        {
            Random rnd = new();
            _touchEvents.Randomize(_sensorCollectionStartTimestamp);
            _keyEvents.Randomize(_sensorCollectionStartTimestamp);
            _backgroundEvents.Randomize(_sensorCollectionStartTimestamp);

            StringBuilder sensorData = new();

            string orientationEvent = GenerateOrientationDataAa();
            string motionEvent = GenerateMotionDataAa();


            sensorData.Append(SdkVersion);
            sensorData.Append("-1,2,-94,-100,");
            sensorData.Append(_systemInfo.ToString());
            sensorData.Append(",");
            sensorData.Append(_systemInfo.GetSum().ToString(CultureInfo.InvariantCulture));
            sensorData.Append(",");
            sensorData.Append(rnd.Next(int.MinValue, int.MaxValue).ToString());
            sensorData.Append(",");
            sensorData.Append((new DateTimeOffset(_sensorCollectionStartTimestamp).ToUnixTimeMilliseconds() / 2).ToString());
            sensorData.Append("-1,2,-94,-101,");
            sensorData.Append("do_en");
            sensorData.Append(",");
            sensorData.Append("dm_en");
            sensorData.Append(",");
            sensorData.Append("t_en");
            sensorData.Append("-1,2,-94,-102,");
            sensorData.Append(GenerateEditedText());
            sensorData.Append("-1,2,-94,-108,");
            sensorData.Append(_keyEvents.ToString());
            sensorData.Append("-1,2,-94,-117,");
            sensorData.Append(_touchEvents.ToString());
            sensorData.Append("-1,2,-94,-111,");
            sensorData.Append(orientationEvent);
            sensorData.Append("-1,2,-94,-109,");
            sensorData.Append(motionEvent);
            sensorData.Append("-1,2,-94,-144,");
            sensorData.Append(GenerateOrientationDataAc());
            sensorData.Append("-1,2,-94,-142,");
            sensorData.Append(GenerateOrientationDataAb());
            sensorData.Append("-1,2,-94,-145,");
            sensorData.Append(GenerateMotionDataAc());
            sensorData.Append("-1,2,-94,-143,");
            sensorData.Append(GenerateMotionEvent());
            sensorData.Append("-1,2,-94,-115,");
            sensorData.Append(GenerateMiscStat(orientationEvent.Count(f => f == ';'), motionEvent.Count(f => f == ';')));
            sensorData.Append("-1,2,-94,-106,");
            sensorData.Append(GenerateStoredValuesF());
            sensorData.Append(",");
            sensorData.Append(GenerateStoredValuesG());
            sensorData.Append("-1,2,-94,-120,");
            sensorData.Append(GenerateStoredStackTraces());
            sensorData.Append("-1,2,-94,-112,");
            sensorData.Append(_performanceTestResults.ToString());
            sensorData.Append("-1,2,-94,-103,");
            sensorData.Append(_backgroundEvents.ToString());

            return _sensorDataEncryptor.EncryptSensorData(sensorData.ToString());
        }

        private static long FeistelCipher(uint upper32, uint lower32, long key)
        {
            int iterate(int arg1, int arg2, int arg3)
            {
                return arg1 ^ (arg2 >> (32 - arg3) | (int)(arg2 << arg3));
            }

            int lower = (int)lower32;
            int upper = (int)upper32;

            long data = lower + ((long)upper << 32);

            int lower2 = (int)(data & 0xFFFFFFFF);
            int upper2 = (int)((data >> 32) & 0xFFFFFFFF);

            for (int i = 0; i < 16; i++)
            {
                int v21 = upper2 ^ iterate(lower2, (int)key, i);
                int v8 = lower2;
                lower2 = v21;
                upper2 = v8;
            }

            return ((long)upper2 << 32) | (lower2 & 0xFFFFFFFF);
        }

        private string GenerateMiscStat(int orientationDataCount, int motionDataCount)
        {
            long sumTextEventValues = _keyEvents.GetSum();
            long sumTouchEventTimestampsAndTypes = _touchEvents.GetSum();
            int orientationDataB = 0;
            int motionDataB = 0;
            long overallSum = sumTextEventValues + sumTouchEventTimestampsAndTypes + orientationDataB + motionDataB;
            long startTime = new DateTimeOffset(_sensorCollectionStartTimestamp).ToUnixTimeMilliseconds();

            return string.Join(",", new string[]
            {
                sumTextEventValues.ToString(CultureInfo.InvariantCulture),
                sumTouchEventTimestampsAndTypes.ToString(CultureInfo.InvariantCulture),
                orientationDataB.ToString(),
                motionDataB.ToString(),
                overallSum.ToString(CultureInfo.InvariantCulture),
                ((long)(DateTime.UtcNow - _sensorCollectionStartTimestamp).TotalMilliseconds).ToString(CultureInfo.InvariantCulture),
                _keyEvents.Length.ToString(),
                _touchEvents.Length.ToString(),
                orientationDataCount.ToString(),
                motionDataCount.ToString(),
                _deviceInfoTime.ToString(),
                (new Random().Next(5, 15) * 1000).ToString(),
                "0",
                FeistelCipher( (uint)overallSum, (uint)(_keyEvents.Length + _touchEvents.Length + orientationDataCount + motionDataCount), startTime).ToString(),
                startTime.ToString(),
                "0"
            });
        }

        private string GenerateEditedText() => string.Empty;
        private string GenerateOrientationDataAa() => string.Empty;
        private string GenerateOrientationDataAb() => string.Empty;
        private string GenerateOrientationDataAc() => string.Empty;
        private string GenerateMotionDataAa() => string.Empty;
        private string GenerateMotionDataAc() => string.Empty;
        private string GenerateMotionEvent() => string.Empty;
        private string GenerateStoredValuesF() => "-1";
        private string GenerateStoredValuesG() => "0";
        private string GenerateStoredStackTraces() => string.Empty;
    }
}

