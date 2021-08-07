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

namespace WingandPrayer.MazdaApi.Model
{
    public class EConnectRemoteHvac
    {
        public int AirConditioningOn { get; set; }
        public int AirConditioningOff { get; set; }
    }

    public class Charge
    {
        public int ImmediatelyCharge { get; set; }
        public int StopPowerSupply { get; set; }
    }

    public class DoorLock
    {
        public int LockBaguraSetOff { get; set; }
        public int LockBaguraSetOn { get; set; }
        public int UnLock { get; set; }
        public int RemoteLockCancel { get; set; }
    }

    public class Hazard
    {
        public int HazardOffCancel { get; set; }
        public int CarFinder { get; set; }
        public int HazardOff { get; set; }
    }

    public class RemoteHvac
    {
        public int EngineOff { get; set; }
        public int EngineOn { get; set; }
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