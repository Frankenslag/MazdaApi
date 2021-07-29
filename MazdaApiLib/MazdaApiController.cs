// 
// MazdaApiController.cs
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using WingandPrayer.MazdaApi.RawModel;
using WingandPrayer.MazdaApi.Exceptions;

namespace WingandPrayer.MazdaApi
{
    internal class MazdaApiController
    {
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class ApiResult
        {
            public string ResultCode { get; set; }
            public string CarlineDesc { get; set; }
        }

        private readonly MazdaApiConnection _connection;

        private static bool CheckResult(string json)
        {
            ApiResult result = JsonConvert.DeserializeObject<ApiResult>(json);

            return (result?.ResultCode ?? string.Empty) == "200S00";
        }

        public async Task<string>GetVehicleBaseInformationAsync() => await _connection.ApiRequestAsync(HttpMethod.Post, "/remoteServices/getVecBaseInfos/v4", new Dictionary<string, string> { { "internaluserid", "__INTERNAL_ID__" } }, true, true);

        public async Task<string> GetVehicleStatusAsync(string internalVin)
        {
            string json = await _connection.ApiRequestAsync(HttpMethod.Post, "/remoteServices/getVehicleStatus/v4", $"{{\"internaluserid\": \"__INTERNAL_ID__\", \"internalvin\": {internalVin}, \"limit\": 1, \"offset\": 0, \"vecinfotype\": \"0\" }}", true, true);

            if (CheckResult(json))
            {
                return json;
            }

            throw new MazdaApiException("Failed to get vehicle status");
        }

        public async Task<string> GetAvailableServiceAsync(string internalVin)
        {
            RawAvailableService result = JsonConvert.DeserializeObject<RawAvailableService>(await _connection.ApiRequestAsync(HttpMethod.Post, "/remoteServices/getAvailableService/v4", $"{{\"internaluserid\": \"__INTERNAL_ID__\", \"internaluseridget\": \"__INTERNAL_ID__\", \"internalvin\": {internalVin}}}", true, true));

            if ((result?.ResultCode ?? string.Empty) == "200S00")
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(result?.AvailableService ?? string.Empty));
            }

            throw new MazdaApiException("Failed to get available service");
        }

        public async Task SetHazzardLightAsync(string internalVin, bool blnLightOn)
        {
            ApiResult result = JsonConvert.DeserializeObject<ApiResult>(await _connection.ApiRequestAsync(HttpMethod.Post, $"/remoteServices/{(blnLightOn ? "lightOn" : "lightOff")}/v4", $"{{\"internaluserid\": \"__INTERNAL_ID__\", \"vin\": {internalVin}}}", true, true));

            if ((result?.ResultCode ?? string.Empty) != "200S00")
            {
                throw new MazdaApiException($"Failed to turn light {(blnLightOn ? "on" : "off")}");
            }
        }

        public async Task<string> GetNicknameAsync(string vin)
        {
            if (vin.Length == 17)
            {
                ApiResult result = JsonConvert.DeserializeObject<ApiResult>(await _connection.ApiRequestAsync(HttpMethod.Post, "/remoteServices/getNickName/v4", $"{{\"internaluserid\": \"__INTERNAL_ID__\", \"vin\": \"{vin}\"}}", true, true));

                if ((result?.ResultCode ?? string.Empty) == "200S00")
                {
                    return result?.CarlineDesc;
                }

                throw new MazdaApiException("Failed to get vehicle nickname");
            }

            throw new MazdaApiException("Invalid VIN");
        }

        public MazdaApiController(string emailAddress, string password, string region)
        {
            _connection = new MazdaApiConnection(emailAddress, password, region);
        }
    }
}