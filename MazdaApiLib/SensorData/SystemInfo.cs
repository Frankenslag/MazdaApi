﻿// 
// SystemInfo.cs
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
using System.Text;

namespace WingandPrayer.MazdaApi.SensorData
{
    internal class SystemInfo
    {
        private readonly int[,] _screenSizes = {{ 1280, 720 }, { 1920, 1080 }, { 2560, 1440 } };

        private readonly Dictionary<string, int> _androidToSdkVersion = new Dictionary<string, int> { { "11", 30 }, { "10", 29 }, { "9", 28 }, { "8.1.0", 27 }, { "8.0.0", 26 }, { "7.1", 25 }, { "7.0", 24 } };

        private string _deviceName;
        private AndroidModel _deviceModel;
        private AndroidBuild _deviceBuild;
        private int _screenSizeIdx;
        private int _buildVersionIncremental;
        private bool _batteryCharging;
        private int _batteryLevel;
        private bool _rotationLock;
        private string _buildBootloader;
        private string _androidId;
        private string _buildHost;

        public SystemInfo()
        {
            Randomize();
        }

        private void Randomize()
        {
            Random rnd = new Random();
            _deviceName = AndroidModels.Models.Keys.ToList()[rnd.Next(0, AndroidModels.Models.Count - 1)];
            _deviceModel = AndroidModels.Models[_deviceName];
            _deviceBuild = _deviceModel.Builds[rnd.Next(0, _deviceModel.Builds.Count - 1)];
            _screenSizeIdx = rnd.Next(0,  _screenSizes.GetLength(0) - 1);
            _buildVersionIncremental = rnd.Next(1000000, 9999999);
            _batteryCharging = rnd.Next(0, 10) <= 1;
            _batteryLevel = rnd.Next(10, 90);
            _rotationLock = rnd.Next(0, 10) > 1;
            _buildBootloader = rnd.Next(1000000, 9999999).ToString();
            byte[] buffer = new byte[8];
            rnd.NextBytes(buffer);
            foreach (byte b in buffer)
            {
                _androidId += $"{b:X2}";
            }
            _buildHost = $"abfarm-{rnd.Next(10000, 99999)}";
        }

        public override string ToString()
        {
            // ReSharper disable once RedundantExplicitParamsArrayCreation
            return string.Join(",", new[]
            {
                "-1", "uaend", "-1", _screenSizes[_screenSizeIdx, 0].ToString(), _screenSizes[_screenSizeIdx, 1].ToString(),
                _batteryCharging ? "1" : "0", _batteryLevel.ToString(), "1", PercentEncode("en"), PercentEncode(_deviceBuild.Version), _rotationLock ? "1" : "0",
                PercentEncode(_deviceName), PercentEncode(_buildBootloader), PercentEncode(_deviceModel.Codename), "-1", "com.interrait.mymazda", "-1", "-1", _androidId, "-1",
                "0", "0", PercentEncode("REL"), PercentEncode(_buildVersionIncremental.ToString()), _androidToSdkVersion[_deviceBuild.Version].ToString(), PercentEncode("Google"),
                PercentEncode(_deviceModel.Codename), PercentEncode("release-keys"), PercentEncode("user"), PercentEncode("android-build"),
                PercentEncode(_deviceBuild.BuildId), PercentEncode(_deviceModel.Codename), PercentEncode("google"), PercentEncode(_deviceModel.Codename),
                PercentEncode($"google/{_deviceModel.Codename}/{_deviceModel.Codename}:{_deviceBuild.Version}/{_deviceBuild.BuildId}/{_buildVersionIncremental}:user/release-keys"),
                PercentEncode(_buildHost), PercentEncode(_deviceBuild.BuildId)
            });
        }

        public long GetSum() => Encoding.UTF8.GetBytes(ToString()).Where(b => b < 0x80).Aggregate<byte, long>(0, (current, b) => current + b);

        private static string PercentEncode(string str)
        {
            StringBuilder sb = new StringBuilder();

            foreach (byte b in Encoding.UTF8.GetBytes(str))
            {
                sb.Append(b >= 33 && b <= 0x7E && b != 34 && b != 37 & b != 39 && b != 44 && b != 92 ? ((char)b).ToString() : $"%{b:X2}");
            }

            return sb.ToString();
        }
    }
}