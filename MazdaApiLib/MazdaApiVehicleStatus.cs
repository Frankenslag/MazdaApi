// //
// // MazdaApiVehicleStatus.cs
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

using System.Threading.Tasks;

using WingandPrayer.MazdaApi.Model;
using WingandPrayer.MazdaApi.RawModel;

using Newtonsoft.Json;

// ReSharper disable UnusedMember.Global

namespace WingandPrayer.MazdaApi
{
    public partial class MazdaApiClient
    {

        public MazdaApiRawVehicleStatus GetRawVehicleStatus(string internalVin) => GetRawVehicleStatusAsync(internalVin).GetAwaiter().GetResult();

        public async Task<MazdaApiRawVehicleStatus> GetRawVehicleStatusAsync(string internalVin) => JsonConvert.DeserializeObject<MazdaApiRawVehicleStatus>(await _controller.GetVehicleStatusAsync(internalVin));

        public VehicleStatus GetVehicleStatus(string internalVin) => GetVehicleStatusAsync(internalVin).GetAwaiter().GetResult();

        public async Task<VehicleStatus> GetVehicleStatusAsync(string internalVin)
        {
            MazdaApiRawVehicleStatus rawStatus = await GetRawVehicleStatusAsync(internalVin);

            AlertInfo alertInfo = rawStatus.AlertInfos[0];
            RemoteInfo remoteInfo = rawStatus.RemoteInfos[0];

            return new VehicleStatus
            {
                LastUpdatedTimestamp = alertInfo.OccurrenceDate,
                Latitude = remoteInfo.PositionInfo.Latitude * (remoteInfo.PositionInfo.LatitudeFlag == 1 ? -1 : 1),
                Longitude = remoteInfo.PositionInfo.Longitude * (remoteInfo.PositionInfo.LongitudeFlag == 1 ? -1 : 1),
                PositionTimestamp = remoteInfo.PositionInfo.AcquisitionDatetime,
                FuelRemainingPercent = remoteInfo.ResidualFuel.FuelSegementDActl,
                FuelDistanceRemainingKm = remoteInfo.ResidualFuel.RemDrvDistDActlKm,
                OdometerKm = remoteInfo.DriveInformation.OdoDispValue, 
                Doors = new Doors
                {
                    DriverDoorOpen = alertInfo.Door.DrStatDrv == 1,
                    PassengerDoorOpen = alertInfo.Door.DrStatPsngr == 1,
                    RearLeftDoorOpen = alertInfo.Door.DrStatRl == 1,
                    RearRightDoorOpen = alertInfo.Door.DrStatRr == 1,
                    TrunkOpen = alertInfo.Door.DrStatTrnkLg == 1,
                    HoodOpen = alertInfo.Door.DrStatHood == 1,
                    FuelLidOpen = alertInfo.Door.FuelLidOpenStatus == 1
                },
                DoorLocks = new DoorLocks
                {
                    DriverDoorUnlocked = alertInfo.Door.LockLinkSwDrv == 1,
                    PassengerDoorUnlocked = alertInfo.Door.LockLinkSwPsngr == 1,
                    RearLeftDoorUnlocked = alertInfo.Door.LockLinkSwRl == 1,
                    RearRightDoorUnlocked = alertInfo.Door.LockLinkSwRr == 1
                },
                Windows = new Windows
                {
                    DriverWindowOpen = alertInfo.Pw.PwPosDrv == 1,
                    PassengerWindowOpen = alertInfo.Pw.PwPosPsngr == 1,
                    RearLeftWindowOpen = alertInfo.Pw.PwPosRl == 1,
                    RearRightWindowOpen = alertInfo.Pw.PwPosRr == 1
                },
                HazardLightsOn = alertInfo.HazardLamp.HazardSw == 1,
                TirePressure = new TirePressure
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