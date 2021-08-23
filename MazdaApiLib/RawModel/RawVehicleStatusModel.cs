// 
// RawVehicleStatusModel.cs
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
using Newtonsoft.Json;
using WingandPrayer.MazdaApi.Model;

// ReSharper disable UnusedMember.Global

namespace WingandPrayer.MazdaApi.RawModel
{
    public class TnsLight
    {
        public int LghtSwState { get; set; }
        public int LightCombiSwMode { get; set; }
        public int TnsLamp { get; set; }
    }

    public class VehicleCondition
    {
        public int PwSavMode { get; set; }
    }

    public class HazardLamp
    {
        public int HazardSw { get; set; }
    }

    public class Pw
    {
        public int PwPosDrv { get; set; }
        public int PwPosRr { get; set; }
        public int PwPosPsngr { get; set; }
        public int PwPosRl { get; set; }
    }

    public class PositionInfo
    {
        [JsonConverter(typeof(StampDateTimeConverter))]
        public DateTime AcquisitionDatetime { get; set; }

        public double Latitude { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool LatitudeFlag { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool LongitudeFlag { get; set; }

        public double Longitude { get; set; }
    }

    public class Door
    {
        public int SrTiltSignal { get; set; }
        public int FuelLidOpenStatus { get; set; }
        public int DrStatHood { get; set; }
        public int LockLinkSwRr { get; set; }
        public int DrStatRr { get; set; }
        public int DrStatPsngr { get; set; }
        public int LockLinkSwRl { get; set; }
        public int DrStatDrv { get; set; }
        public int DrStatRl { get; set; }
        public int AllDrSwSignal { get; set; }
        public int SrSlideSignal { get; set; }
        public int LockLinkSwDrv { get; set; }
        public int DrOpnWrn { get; set; }
        public int DrStatTrnkLg { get; set; }
        public int LockLinkSwPsngr { get; set; }
    }

    public class DcmPositionAccuracyEntity
    {
        public int MinorAxisError { get; set; }
        public int Gradient { get; set; }
        public int AcquisitionState { get; set; }
        public int MajorAxisError { get; set; }
    }

    public class AlertInfo
    {
        public TnsLight TnsLight { get; set; }
        public VehicleCondition VehicleCondition { get; set; }
        public HazardLamp HazardLamp { get; set; }
        public Pw Pw { get; set; }

        [JsonConverter(typeof(StampDateTimeConverter))]
        public DateTime OccurrenceDate { get; set; }

        public PositionInfo PositionInfo { get; set; }
        public int PositionInfoCategory { get; set; }
        public Door Door { get; set; }
        public DcmPositionAccuracyEntity DcmPositionAccuracyEntity { get; set; }
    }

    public class RegularMntInformation
    {
        public int RemRegDistMile { get; set; }
        public int RemRegDistKm { get; set; }
        public int MntSetDate { get; set; }
        public int MntSetMonth { get; set; }
        public int MntSetDistMile { get; set; }
        public int MntSetDistKm { get; set; }
        public int MntSetYear { get; set; }
    }

    public class ResidualFuel
    {
        public double RemDrvDistDActlMile { get; set; }
        public double RemDrvDistDActlKm { get; set; }
        public double FuelSegementDActl { get; set; }
    }

    public class SeatBeltInformation
    {
        public int FirstRowBuckleDriver { get; set; }
        public int RlocsStatDActl { get; set; }
        public int OcsStatus { get; set; }
        public int RrocsStatDActl { get; set; }
        public int RcocsStatDActl { get; set; }
        public int SeatBeltWrnDRq { get; set; }
        public string SeatBeltStatDActl { get; set; }
        public int FirstRowBucklePsngr { get; set; }
    }

    public class TpmsInformation
    {
        public double FltPrsDispBar { get; set; }
        public int MntTyreAtFlg { get; set; }
        public int FltPrsDispKp { get; set; }
        public double RltPrsDispKgfPcm2 { get; set; }
        public double FrtPrsDispPsi { get; set; }
        public double FltPrsDispPsi { get; set; }
        public double RrtPrsDispBar { get; set; }
        public double FrtPrsDispKgfPcm2 { get; set; }
        public int PrsDispMinute { get; set; }
        public int PrsDispYear { get; set; }
        public int PrsDispMonth { get; set; }
        public double RrtPrsDispPsi { get; set; }
        public int RltPrsDispKp { get; set; }
        public int RrTyrePressWarn { get; set; }
        public int TpmsSystemFlt { get; set; }
        public int RlTyrePressWarn { get; set; }
        public double RltPrsDispBar { get; set; }
        public int FrTyrePressWarn { get; set; }
        public double RrtPrsDispKgfPcm2 { get; set; }
        public double FltPrsDispKgfPcm2 { get; set; }
        public int PrsDispDate { get; set; }
        public int RrtPrsDispKp { get; set; }
        public int FrtPrsDispKp { get; set; }
        public int TpmsStatus { get; set; }
        public double RltPrsDispPsi { get; set; }
        public int FlTyrePressWarn { get; set; }
        public double FrtPrsDispBar { get; set; }
        public int PrsDispHour { get; set; }
    }

    public class DriveInformation
    {
        public double Drv1AvlFuelG { get; set; }
        public double Drv1AvlFuelE { get; set; }
        public double Drv1AmntFuel { get; set; }
        public double OdoDispValue { get; set; }
        public double Drv1Distnc { get; set; }
        public int Drv1DrvTm { get; set; }
        public double OdoDispValueMile { get; set; }
    }

    public class ElectricalInformation
    {
        public int PowerControlStatus { get; set; }
        public int EngineState { get; set; }
    }

    public class BatteryStatus
    {
        public double SocEcmAEst { get; set; }
    }

    public class MntScrInformation
    {
        public double UreaTankLevel { get; set; }
        public int RemainingMileage { get; set; }
        public int MntScrAtFlg { get; set; }
    }

    public class OilMntInformation
    {
        public int DrOilDeteriorateLevel { get; set; }
        public int MntOilAtFlg { get; set; }
        public int OilDeteriorateWarning { get; set; }
        public int OilDtrInitTime { get; set; }
        public int OilDtrInitDistMile { get; set; }
        public int OilLevelWarning { get; set; }
        public int MntOilLvlAtFlg { get; set; }
        public int OilDtrInitDistKm { get; set; }
        public int OilLevelStatusMonitor { get; set; }
        public int OilLevelSensWarnBRq { get; set; }
    }

    public class RemoteInfo
    {
        public RegularMntInformation RegularMntInformation { get; set; }
        public ResidualFuel ResidualFuel { get; set; }
        public SeatBeltInformation SeatBeltInformation { get; set; }
        public TpmsInformation TpmsInformation { get; set; }
        public DriveInformation DriveInformation { get; set; }
        public ElectricalInformation ElectricalInformation { get; set; }
        public BatteryStatus BatteryStatus { get; set; }
        public MntScrInformation MntScrInformation { get; set; }
        public int UsbPositionAccuracy { get; set; }
        public PositionInfo PositionInfo { get; set; }
        public string OccurrenceDate { get; set; }
        public OilMntInformation OilMntInformation { get; set; }
        public int PositionInfoCategory { get; set; }
        public DcmPositionAccuracyEntity DcmPositionAccuracyEntity { get; set; }
    }

    public class MazdaApiRawVehicleStatus
    {
        public List<AlertInfo> AlertInfos { get; set; }
        public List<RemoteInfo> RemoteInfos { get; set; }
        public string VisitNo { get; set; }
    }
}