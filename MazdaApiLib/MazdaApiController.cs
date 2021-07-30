// 
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