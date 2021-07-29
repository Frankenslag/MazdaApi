// 
// VehicleStatus.cs
// 
// Copyright 2021 Wingandprayer Software
// 
// This file is part of MazdaApiLib.
// 
// MazdaApiLib is free software: you can redistribute it and/or modify it under the terms of the
// GNU General Public License as published by the Free Software Foundation, either version 2
// of the License, or (at your option) any later version.
// 
// CDRipper is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
// PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with MazdaApiLib.
// If not, see http://www.gnu.org/licenses/.

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
