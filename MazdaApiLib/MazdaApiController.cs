﻿// 
// MazdaApiController.cs
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WingandPrayer.MazdaApi.Exceptions;
using WingandPrayer.MazdaApi.Model;
using WingandPrayer.MazdaApi.RawModel;

namespace WingandPrayer.MazdaApi
{
    internal class MazdaApiController
    {
        private readonly MazdaApiConnection _connection;

        public MazdaApiController(string emailAddress, string password, string region, HttpClient httpClient, ILogger<MazdaApiClient> logger) => _connection = new MazdaApiConnection(emailAddress, password, region, httpClient, logger);

        private static bool CheckResult(string json)
        {
            ApiResult result = JsonConvert.DeserializeObject<ApiResult>(json);

            return (result?.ResultCode ?? string.Empty) == "200S00";
        }

        public async Task<FindVehicleLocationResponse> FindVehicleLocationAsync(string internalVin)
        {
            FindVehicleLocationResponse result = JsonConvert.DeserializeObject<FindVehicleLocationResponse>(await _connection.ApiRequestAsync(HttpMethod.Post, "/remoteServices/findVehicleLocation/v4", $"{{\"internaluserid\": \"__INTERNAL_ID__\", \"internalvin\": {internalVin}}}", true, true));

            if ((result?.ResultCode ?? string.Empty) != "200S00")
            {
                throw new MazdaApiException("Failed to get Vehicle Location");
            }

            return result;
        }

        public async Task ActivateRealTimeVehicleStatusAsync(string internalVin)
        {
            ApiResult result = JsonConvert.DeserializeObject<ApiResult>(await _connection.ApiRequestAsync(HttpMethod.Post, "/remoteServices/activeRealTimeVehicleStatus/v4", $"{{\"internaluserid\": \"__INTERNAL_ID__\", \"internalvin\": {internalVin}}}", true, true));

            if ((result?.ResultCode ?? string.Empty) != "200S00")
            {
                throw new MazdaApiException("Failed to activate realtime vehicle status");
            }
        }

        public async Task<string> GetVehicleBaseInformationAsync() => await _connection.ApiRequestAsync(HttpMethod.Post, "/remoteServices/getVecBaseInfos/v4", new Dictionary<string, string> { { "internaluserid", "__INTERNAL_ID__" } }, true, true);

        public async Task<EvVehicleStatus> GetEvVehicleStatusAsync(string internalVin)
        {
            EvVehicleStatusResponse result = JsonConvert.DeserializeObject<EvVehicleStatusResponse>(await _connection.ApiRequestAsync(HttpMethod.Post, "/remoteServices/getEVVehicleStatus/v4", $"{{\"internaluserid\": \"__INTERNAL_ID__\", \"internalvin\": {internalVin}, \"limit\": 1, \"offset\": 0, \"vecinfotype\": \"0\"}}", true, true));

            if ((result?.ResultCode ?? string.Empty) == "200S00")
            {
                return result?.ResultData?.FirstOrDefault();
            }

            throw new MazdaApiException("Failed to get EV vehicle status");
        }

        public async Task<string> GetVehicleStatusAsync(string internalVin)
        {
            string json = await _connection.ApiRequestAsync(HttpMethod.Post, "/remoteServices/getVehicleStatus/v4", $"{{\"internaluserid\": \"__INTERNAL_ID__\", \"internalvin\": {internalVin}, \"limit\": 1, \"offset\": 0, \"vecinfotype\": \"0\"}}", true, true);

            if (CheckResult(json)) return json;

            throw new MazdaApiException("Failed to get vehicle status");
        }

        public async Task SetHvacSettingsAsync(string internalVin, HvacSettings hvacSettings)
        {
            ApiResult result = JsonConvert.DeserializeObject<ApiResult>(await _connection.ApiRequestAsync(HttpMethod.Post, "/remoteServices/updateHVACSetting/v4", $"{{\"internaluserid\": \"__INTERNAL_ID__\", \"internalvin\": {internalVin}, \"hvacsettings\": {JsonConvert.SerializeObject(hvacSettings)}}}", true, true));

            if ((result?.ResultCode ?? string.Empty) != "200S00")
            {
                throw new MazdaApiException("Failed to update hvac settings");
            }
        }

        public async Task<HvacSettings> GetHvacSettingsAsync(string vin)
        {
            ApiResult result = JsonConvert.DeserializeObject<ApiResult>(await _connection.ApiRequestAsync(HttpMethod.Post, "/remoteServices/getHVACSetting/v4", $"{{\"internaluserid\": \"__INTERNAL_ID__\", \"vin\": \"{vin}\"}}", true, true));

            if ((result?.ResultCode ?? string.Empty) == "200S00") return result?.HvacSettings;

            throw new MazdaApiException("Failed to get hvac settings");
        }

        public async Task<string> GetRemotePermissionsAsync(string vin)
        {
            ApiResult result = JsonConvert.DeserializeObject<ApiResult>(await _connection.ApiRequestAsync(HttpMethod.Post, "/remoteServices/getRemoteControlPermission/v4", $"{{\"internaluserid\": \"__INTERNAL_ID__\", \"vin\": \"{vin}\"}}", true, true));

            if ((result?.ResultCode ?? string.Empty) == "200S00") return result?.RemoteControl.ToString() ?? string.Empty;

            throw new MazdaApiException("Failed to get remote permissions");
        }

        public async Task<string> GetAvailableServiceAsync(string internalVin)
        {
            ApiResult result = JsonConvert.DeserializeObject<ApiResult>(await _connection.ApiRequestAsync(HttpMethod.Post, "/remoteServices/getAvailableService/v4", $"{{\"internaluserid\": \"__INTERNAL_ID__\", \"internaluseridget\": \"__INTERNAL_ID__\", \"internalvin\": {internalVin}}}", true, true));

            if ((result?.ResultCode ?? string.Empty) == "200S00") return Encoding.UTF8.GetString(Convert.FromBase64String(result?.AvailableService ?? string.Empty));

            throw new MazdaApiException("Failed to get available service");
        }

        public async Task SetHazzardLightAsync(string internalVin, bool blnLightOn)
        {
            ApiResult result = JsonConvert.DeserializeObject<ApiResult>(await _connection.ApiRequestAsync(HttpMethod.Post, $"/remoteServices/{(blnLightOn ? "lightOn" : "lightOff")}/v4", $"{{\"internaluserid\": \"__INTERNAL_ID__\", \"internalvin\": {internalVin}}}", true, true));

            if ((result?.ResultCode ?? string.Empty) != "200S00") throw new MazdaApiException($"Failed to turn light {(blnLightOn ? "on" : "off")}");
        }

        public async Task SetHvacAsync(string internalVin, bool blnOn)
        {
            ApiResult result = JsonConvert.DeserializeObject<ApiResult>(await _connection.ApiRequestAsync(HttpMethod.Post, $"/remoteServices/{(blnOn ? "hvacOn" : "hvacOff")}/v4", $"{{\"internaluserid\": \"__INTERNAL_ID__\", \"internalvin\": {internalVin}}}", true, true));

            if ((result?.ResultCode ?? string.Empty) != "200S00") throw new MazdaApiException($"Failed to set Hvac status to {(blnOn ? "On" : "Off")}");
        }

        public async Task SetDoorLockAsync(string internalVin, bool blnLock)
        {
            ApiResult result = JsonConvert.DeserializeObject<ApiResult>(await _connection.ApiRequestAsync(HttpMethod.Post, $"/remoteServices/{(blnLock ? "doorLock" : "doorUnlock")}/v4", $"{{\"internaluserid\": \"__INTERNAL_ID__\", \"internalvin\": {internalVin}}}", true, true));

            if ((result?.ResultCode ?? string.Empty) != "200S00") throw new MazdaApiException($"Failed to {(blnLock ? "lock" : "unlock")} door");
        }

        public async Task<string> GetNicknameAsync(string vin)
        {
            if (vin.Length == 17)
            {
                ApiResult result = JsonConvert.DeserializeObject<ApiResult>(await _connection.ApiRequestAsync(HttpMethod.Post, "/remoteServices/getNickName/v4", $"{{\"internaluserid\": \"__INTERNAL_ID__\", \"vin\": \"{vin}\"}}", true, true));

                if ((result?.ResultCode ?? string.Empty) == "200S00") return result?.CarlineDesc;

                throw new MazdaApiException("Failed to get vehicle nickname");
            }

            throw new MazdaApiException("Invalid VIN");
        }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class ApiResult
        {
            public string ResultCode { get; set; }
            public HvacSettings HvacSettings { get; set; }
            public string AvailableService { get; set; }
            public string CarlineDesc { get; set; }
            public object RemoteControl { get; set; }
        }
    }
}