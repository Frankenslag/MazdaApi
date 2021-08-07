// 
// MazdaApiVehicleStatus.cs
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

using System.Threading.Tasks;
using Newtonsoft.Json;
using WingandPrayer.MazdaApi.Model;
using WingandPrayer.MazdaApi.RawModel;

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

            VehicleStatus retval = new()
            {
                LastUpdatedTimestamp = alertInfo.OccurrenceDate,
                Latitude = remoteInfo.PositionInfo.Latitude * (remoteInfo.PositionInfo.LatitudeFlag == 1 ? -1 : 1),
                Longitude = remoteInfo.PositionInfo.Longitude * (remoteInfo.PositionInfo.LongitudeFlag == 1 ? 1 : -1),
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

            CachedLockState lockState = GetCachedLockState(internalVin);

            lockState.ApiLockTimestamp = retval.LastUpdatedTimestamp;
            lockState.ApiLockState = !(retval.DoorLocks.DriverDoorUnlocked || retval.DoorLocks.PassengerDoorUnlocked || retval.DoorLocks.RearLeftDoorUnlocked || retval.DoorLocks.RearRightDoorUnlocked);

            return retval;
        }
    }
}