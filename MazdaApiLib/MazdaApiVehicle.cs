// 
// MazdaApiVehicle.cs
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

using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WingandPrayer.MazdaApi.Model;
using WingandPrayer.MazdaApi.RawModel;

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
            if (_vehicleCache == null || !_useCachedVehicleList) _vehicleCache = JsonConvert.DeserializeObject<MazdaApiVehicles>(await _controller.GetVehicleBaseInformationAsync());

            return _vehicleCache;
        }

        public List<VehicleModel> GetVehicles() => GetVehiclesAsync().GetAwaiter().GetResult();

        public async Task<List<VehicleModel>> GetVehiclesAsync()
        {
            List<VehicleModel> retval = new List<VehicleModel>();

            MazdaApiVehicles vehicles = await GetRawVehiclesAsync();

            for (int i = 0; i < vehicles.VecBaseInfos.Count; i++)
            {
                MazdaApiRawVehicleBaseInfo baseInfo = vehicles.VecBaseInfos[i];
                MazdaApiVehicleOtherInformation otherInformation = baseInfo.Vehicle.VehicleInformation.OtherInformation;

                if (vehicles.VehicleFlags[i].VinRegistStatus == 1 || vehicles.VehicleFlags[i].VinRegistStatus == 3)
                    retval.Add(new VehicleModel
                    {
                        Vin = baseInfo.Vin,
                        Id = baseInfo.Vehicle.CvInformation.InternalVin,
                        IsEvVehicle = (baseInfo.VehicleType ?? string.Empty) == "1" && (baseInfo.EconnectType ?? string.Empty) == "1",
                        VinRegistStatus = vehicles.VehicleFlags[i].VinRegistStatus,
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