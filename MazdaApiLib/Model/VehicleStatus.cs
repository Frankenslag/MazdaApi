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
