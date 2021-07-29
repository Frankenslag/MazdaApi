// 
// MazdaApiVehicle.cs
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

using System.Collections.Generic;
using System.Threading.Tasks;

using WingandPrayer.MazdaApi.Model;
using WingandPrayer.MazdaApi.RawModel;

using Newtonsoft.Json;

// ReSharper disable UnusedMember.Global

namespace WingandPrayer.MazdaApi
{
    public partial class MazdaApiClient
    {
        private readonly bool _useCachedVehicleList;
        private MazdaApiVehicles _vehicleCache;

        public MazdaApiVehicles GetRawVehicles() => GetRawVehiclesAsync().GetAwaiter().GetResult();

        public async Task<MazdaApiVehicles> GetRawVehiclesAsync()
        {
            if (_vehicleCache == null || !_useCachedVehicleList)
            {
                _vehicleCache = JsonConvert.DeserializeObject<MazdaApiVehicles>(await _controller.GetVehicleBaseInformationAsync());
            }

            return _vehicleCache;
        }

        public List<VehicleModel> GetVehicles() => GetVehiclesAsync().GetAwaiter().GetResult();

        public async Task<List<VehicleModel>> GetVehiclesAsync()
        {
            List<VehicleModel> retval = new();

            foreach (MazdaApiRawVehicleBaseInfo baseInfo in (await GetRawVehiclesAsync()).VecBaseInfos)
            {
                MazdaApiVehicleOtherInformation otherInformation = baseInfo.Vehicle.VehicleInformation.OtherInformation;

                retval.Add(new VehicleModel
                {
                    Vin = baseInfo.Vin,
                    Id = baseInfo.Vehicle.CvInformation.InternalVin,
                    Nickname = await _controller.GetNicknameAsync(baseInfo.Vin),
                    CarlineCode = otherInformation.CarlineCode,
                    CarlineName = otherInformation.CarlineName,
                    ModelYear = otherInformation.ModelYear,
                    ModelCode = otherInformation.ModelCode,
                    ModelName = otherInformation.ModelName,
                    AutomaticTransmission = otherInformation.TransmissionType == "A",
                    InteriorColorCode = otherInformation.InteriorColorCode,
                    InteriorColorName = otherInformation.InteriorColorName,
                    ExteriorColorCode = otherInformation.ExteriorColorCode,
                    ExteriorColorName = otherInformation.ExteriorColorName
                });
            }

            return retval;
        }
    }
}