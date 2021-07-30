// 
// VehicleStatus.cs
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

namespace WingandPrayer.MazdaApi.Model
{
    public class TirePressure
    {
        public double FrontLeftTirePressurePsi { get; set; }
        public double FrontRightTirePressurePsi { get; set; }
        public double RearLeftTirePressurePsi { get; set; }
        public double RearRightTirePressurePsi { get; set; }
    }

    public class Windows
    {
        public bool DriverWindowOpen { get; set; }
        public bool PassengerWindowOpen { get; set; }
        public bool RearLeftWindowOpen { get; set; }
        public bool RearRightWindowOpen { get; set; }
    }

    public class DoorLocks
    {
        public bool DriverDoorUnlocked { get; set; }
        public bool PassengerDoorUnlocked { get; set; }
        public bool RearLeftDoorUnlocked { get; set; }
        public bool RearRightDoorUnlocked { get; set; }
    }

    public class Doors
    {
        public bool DriverDoorOpen { get; set; }
        public bool PassengerDoorOpen { get; set; }
        public bool RearLeftDoorOpen { get; set; }
        public bool RearRightDoorOpen { get; set; }
        public bool TrunkOpen { get; set; }
        public bool HoodOpen { get; set; }
        public bool FuelLidOpen { get; set; }
    }

    public class VehicleStatus
    {
        public DateTime LastUpdatedTimestamp { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime PositionTimestamp { get; set; }
        public double FuelRemainingPercent { get; set; }
        public double FuelDistanceRemainingKm { get; set; }
        public double OdometerKm { get; set; }
        public Doors Doors { get; set; }
        public DoorLocks DoorLocks { get; set; }
        public Windows Windows { get; set; }
        public bool HazardLightsOn { get; set; }
        public TirePressure TirePressure { get; set; }
    }
}
