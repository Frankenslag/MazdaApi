using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using WingandPrayer.MazdaApi.Model;
using WingandPrayer.MazdaApi.RawModel;

using Newtonsoft.Json;

namespace WingandPrayer.MazdaApi
{
    public partial class MazdaApiClient
    {
        public async ValueTask<MazdaApiRawVehicleStatus> GetRawVehicleStatus(string internalVin) => JsonConvert.DeserializeObject<MazdaApiRawVehicleStatus>(await _controller.GetVehicleStatus(internalVin));

        public async ValueTask<VehicleStatus> GetVehicleStatus(string internalVin)
        {
            MazdaApiRawVehicleStatus rawStatus = await GetRawVehicleStatus(internalVin);

            AlertInfo alertInfo = rawStatus.AlertInfos[0];
            RemoteInfo remoteInfo = rawStatus.RemoteInfos[0];

            return new VehicleStatus()
            {
                LastUpdatedTimestamp = alertInfo.OccurrenceDate,
                Latitude = remoteInfo.PositionInfo.Latitude * (remoteInfo.PositionInfo.LatitudeFlag == 1 ? -1 : 1),
                Longitude = remoteInfo.PositionInfo.Longitude * (remoteInfo.PositionInfo.LongitudeFlag == 1 ? -1 : 1),
                PositionTimestamp = remoteInfo.PositionInfo.AcquisitionDatetime,
                FuelRemainingPercent = remoteInfo.ResidualFuel.FuelSegementDActl,
                FuelDistanceRemainingKm = remoteInfo.ResidualFuel.RemDrvDistDActlKm,
                OdometerKm = remoteInfo.DriveInformation.OdoDispValue, 
                Doors = new Doors()
                {
                    DriverDoorOpen = alertInfo.Door.DrStatDrv == 1,
                    PassengerDoorOpen = alertInfo.Door.DrStatPsngr == 1,
                    RearLeftDoorOpen = alertInfo.Door.DrStatRl == 1,
                    RearRightDoorOpen = alertInfo.Door.DrStatRr == 1,
                    TrunkOpen = alertInfo.Door.DrStatTrnkLg == 1,
                    HoodOpen = alertInfo.Door.DrStatHood == 1,
                    FuelLidOpen = alertInfo.Door.FuelLidOpenStatus == 1
                },
                DoorLocks = new DoorLocks()
                {
                    DriverDoorUnlocked = alertInfo.Door.LockLinkSwDrv == 1,
                    PassengerDoorUnlocked = alertInfo.Door.LockLinkSwPsngr == 1,
                    RearLeftDoorUnlocked = alertInfo.Door.LockLinkSwRl == 1,
                    RearRightDoorUnlocked = alertInfo.Door.LockLinkSwRr == 1
                },
                Windows = new Windows()
                {
                    DriverWindowOpen = alertInfo.Pw.PwPosDrv == 1,
                    PassengerWindowOpen = alertInfo.Pw.PwPosPsngr == 1,
                    RearLeftWindowOpen = alertInfo.Pw.PwPosRl == 1,
                    RearRightWindowOpen = alertInfo.Pw.PwPosRr == 1,
                },
                HazardLightsOn = alertInfo.HazardLamp.HazardSw == 1,
                TirePressure = new TirePressure()
                {
                    FrontLeftTirePressurePsi = remoteInfo.TpmsInformation.FltPrsDispPsi,
                    FrontRightTirePressurePsi = remoteInfo.TpmsInformation.FrtPrsDispPsi,
                    RearLeftTirePressurePsi = remoteInfo.TpmsInformation.RltPrsDispPsi,
                    RearRightTirePressurePsi = remoteInfo.TpmsInformation.RrtPrsDispPsi
                }
            };
        }
    }
}