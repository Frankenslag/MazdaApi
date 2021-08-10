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
using System.Collections.Generic;

// ReSharper disable UnusedMember.Global

namespace WingandPrayer.MazdaApi.Model
{
    public class ChargeInfo
    {
        public int SmaphRemDrvDistKm { get; set; }
        public int SmaphSoc { get; set; }
        public int CstmzStatBatHeatAutoSw { get; set; }
        public int SmaphRemDrvDistMile { get; set; }
        public int AcChargeStatus { get; set; }
        public int ChargeStatusSub { get; set; }
        public int ChargerConnectorFitting { get; set; }
        public int MaxChargeMinuteAc { get; set; }
        public int BatteryHeaterOn { get; set; }
        public int DcChargeStatus { get; set; }
        public int MaxChargeMinuteQbc { get; set; }
        public int ChargeScheduleStatus { get; set; }
        public DateTime LastUpdatedTimeForScheduledChargeTime { get; set; }
    }

    public class RemoteHvacInfo
    {
        public int Hvac { get; set; }
        public int RearDefogger { get; set; }
        public double InCarTeDc { get; set; }
        public double InCarTeDf { get; set; }
        public int FrontDefroster { get; set; }
    }

    public class VehicleInfo
    {
        public ChargeInfo ChargeInfo { get; set; }
        public RemoteHvacInfo RemoteHvacInfo { get; set; }
    }

    public class PlusBInformation
    {
        public VehicleInfo VehicleInfo { get; set; }
    }

    public class IgInformation
    {
        public VehicleInfo VehicleInfo { get; set; }
    }

    public class DcmPositionAccuracy
    {
        public int Gradient { get; set; }
        public int MinorAxisError { get; set; }
        public int AcquisitionState { get; set; }
        public int MajorAxisError { get; set; }
    }

    public class PositionInfo
    {
        public DateTime AcquisitionDatetime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DcmPositionAccuracy DcmPositionAccuracy { get; set; }
    }

    public class EvVehicleStatus
    {
        public string NId { get; set; }
        public DateTime InformationDatetime { get; set; }
        public PlusBInformation PlusBInformation { get; set; }
        public int NotificationCategory { get; set; }
        public string SId { get; set; }
        public DateTime OccurrenceTime { get; set; }
        public string DcmNumber { get; set; }
        public string BsId { get; set; }
        public DateTime DcmDormantDatetime { get; set; }
        public IgInformation IgInformation { get; set; }
        public string OccurrenceDate { get; set; }
        public PositionInfo PositionInfo { get; set; }
        public int PositionInfoCategory { get; set; }
        public string TransmissionFactor { get; set; }
    }

    public class EvVehicleStatusResponse
    {
        public List<EvVehicleStatus> ResultData { get; set; }
        public string ResultCode { get; set; }
        public string VisitNo { get; set; }
    }

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