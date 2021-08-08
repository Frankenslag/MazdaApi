// 
// RemotePermissions.cs
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

// ReSharper disable UnusedMember.Global

using Newtonsoft.Json;

namespace WingandPrayer.MazdaApi.Model
{
    public class EConnectRemoteHvac
    {
        [JsonConverter(typeof(BoolConverter))]
        public bool AirConditioningOn { get; set; }
        [JsonConverter(typeof(BoolConverter))]
        public bool AirConditioningOff { get; set; }
    }

    public class Charge
    {
        [JsonConverter(typeof(BoolConverter))]
        public bool ImmediatelyCharge { get; set; }
        [JsonConverter(typeof(BoolConverter))]
        public bool StopPowerSupply { get; set; }
    }

    public class DoorLock
    {
        [JsonConverter(typeof(BoolConverter))]
        public bool LockBaguraSetOff { get; set; }
        [JsonConverter(typeof(BoolConverter))]
        public bool LockBaguraSetOn { get; set; }
        [JsonConverter(typeof(BoolConverter))]
        public bool UnLock { get; set; }
        [JsonConverter(typeof(BoolConverter))]
        public bool RemoteLockCancel { get; set; }
    }

    public class Hazard
    {
        [JsonConverter(typeof(BoolConverter))]
        public bool HazardOffCancel { get; set; }
        [JsonConverter(typeof(BoolConverter))]
        public bool CarFinder { get; set; }
        [JsonConverter(typeof(BoolConverter))]
        public bool HazardOff { get; set; }
    }

    public class RemoteHvac
    {
        [JsonConverter(typeof(BoolConverter))]
        public bool EngineOff { get; set; }
        [JsonConverter(typeof(BoolConverter))]
        public bool EngineOn { get; set; }
    }

    public class RemoteControlPermissions
    {
        public EConnectRemoteHvac EConnectRemoteHvac { get; set; }
        public Charge Charge { get; set; }
        public DoorLock DoorLock { get; set; }
        public Hazard Hazard { get; set; }
        public RemoteHvac RemoteHvac { get; set; }
    }
}