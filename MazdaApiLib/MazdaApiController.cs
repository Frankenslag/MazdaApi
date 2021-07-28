using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using WingandPrayer.MazdaApi.Exceptions;

namespace WingandPrayer.MazdaApi
{
    internal partial class MazdaApiController
    {
        private class ApiResult
        {
            public string ResultCode { get; set; }
            public string CarlineDesc { get; set; }
        }

        private readonly MazdaApiConnection _connection;

        private bool CheckResult(string json)
        {
            ApiResult result = JsonConvert.DeserializeObject<ApiResult>(json);

            return (result?.ResultCode ?? string.Empty) == "200S00";
        }

        public async ValueTask<string>GetVehicleBaseInformation() => await _connection.ApiRequest(HttpMethod.Post, "/remoteServices/getVecBaseInfos/v4", new Dictionary<string, string>() { { "internaluserid", "__INTERNAL_ID__" } }, true, true);

        public async ValueTask<string> GetVehicleStatus(string internalVin)
        {
            string json = await _connection.ApiRequest(HttpMethod.Post, "/remoteServices/getVehicleStatus/v4", $"{{\"internaluserid\": \"__INTERNAL_ID__\", \"internalvin\": {internalVin}, \"limit\": 1, \"offset\": 0, \"vecinfotype\": \"0\" }}", true, true);

            if (CheckResult(json))
            {
                return json;
            }
            else
            {
                throw new MazdaApiException("Failed to get vehicle status");
            }
        }

        public async ValueTask<string> GetNickname(string vin)
        {
            if (vin.Length == 17)
            {
                ApiResult result = JsonConvert.DeserializeObject<ApiResult>(await _connection.ApiRequest(HttpMethod.Post, "/remoteServices/getNickName/v4", $"{{\"internaluserid\": \"__INTERNAL_ID__\", \"vin\": \"{vin}\"}}", true, true));

                if ((result?.ResultCode ?? string.Empty) == "200S00")
                {
                    return result?.CarlineDesc;
                }
                else
                {
                    throw new MazdaApiException("Failed to get vehicle nickname");
                }
            }
            else
            {
                throw new MazdaApiException("Invalid VIN");
            }
        }

        public MazdaApiController(string emailAddress, string password, string region)
        {
            _connection = new MazdaApiConnection(emailAddress, password, region);
        }
    }
}