// 
// MazdaApiConnection.cs
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
using System.Collections.Specialized;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WingandPrayer.MazdaApi.Exceptions;
using WingandPrayer.MazdaApi.SensorData;
using JsonSerializer = System.Text.Json.JsonSerializer;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable ClassNeverInstantiated.Local

namespace WingandPrayer.MazdaApi
{
    internal class MazdaApiConnection
    {
        private readonly ILogger<MazdaApiClient> _logger;
        private readonly HttpClient _httpClient;

        private const string AppOs = "Android";
        private const string Iv = "0102030405060708";
        private const string SignatureMd5 = "C383D8C4D279B78130AD52DC71D95CAA";
        private const string AppVersion = "8.1.1";
        private const string AppPackageId = "com.interrait.mymazda";
        private const string UsherSdkVersion = "11.2.0000.002";
        private const string UserAgentBaseApi = "MyMazda-Android/8.1.1";
        private const string UserAgentUsherApi = "MyMazda/8.1.1 (Google Pixel 3a; Android 11)";
        private const int MaxRetries = 4;

        private static readonly Dictionary<string, RegionConfig> RegionsConfigs = new Dictionary<string, RegionConfig>
        {
            { "MNAO", new RegionConfig { ApplicationCode = "202007270941270111799", BaseUrl = new Uri("https://0cxo7m58.mazda.com/prod"), UsherUrl = new Uri("https://ptznwbh8.mazda.com/appapi/v1") } },
            { "MME", new RegionConfig { ApplicationCode = "202008100250281064816", BaseUrl = new Uri("https://e9stj7g7.mazda.com/prod"), UsherUrl = new Uri("https://rz97suam.mazda.com/appapi/v1") } },
            { "MJO", new RegionConfig { ApplicationCode = "202009170613074283422", BaseUrl = new Uri("https://wcs9p6wj.mazda.com/prod"), UsherUrl = new Uri("https://c5ulfwxr.mazda.com/appapi/v1") } }
        };


        private readonly RegionConfig _regionConfig;
        private readonly string _emailAddress;
        private readonly string _usherApiPassword;

        private readonly SensorDataBuilder _sensorDataBuilder;
        private readonly string _baseApiDeviceId;
        private readonly string _usherApiDeviceId;
        private string _accessToken;
        private DateTime _accessTokenExpirationTs;
        private string _encKey;
        private string _signKey;

        public MazdaApiConnection(string emailAddress, string password, string region, HttpClient httpClient, ILogger<MazdaApiClient> logger)
        {
            _logger = logger;

            if (RegionsConfigs.ContainsKey(region))
            {
                _httpClient = httpClient;
                _emailAddress = emailAddress;
                _usherApiPassword = password;
                _accessToken = null;
                _accessTokenExpirationTs = DateTime.MinValue;
                _regionConfig = RegionsConfigs[region];
                _baseApiDeviceId = CryptoUtils.GenerateUuidFromSeed(emailAddress);
                _usherApiDeviceId = CryptoUtils.GenerateUsherDeviceIdFromSeed(emailAddress);
                _sensorDataBuilder = new SensorDataBuilder();
            }
            else
            {
                throw new MazdaApiConfigException("Invalid region");
            }
        }

        private string GetSignFromTimestamp(long timestamp)
        {
            string strTimestamp = timestamp.ToString();
            string strTimestampExtended = strTimestamp + strTimestamp[6..] + strTimestamp[3..];
            string val2;

            using (MD5 md5 = MD5.Create())
            {
                val2 = CryptoUtils.HexDigest(md5.ComputeHash(Encoding.UTF8.GetBytes(CryptoUtils.HexDigest(md5.ComputeHash(Encoding.UTF8.GetBytes(_regionConfig.ApplicationCode + AppPackageId))).ToUpper() + SignatureMd5))).ToLower();
            }

            return CryptoUtils.GetPayloadSign(strTimestampExtended, val2.Substring(20, 12) + val2[..10] + val2.Substring(4, 2));
        }

        private string GetSignFromPayloadAndTimestamp(string payload, long timestamp)
        {
            string strTimestamp = timestamp.ToString();

            if (!string.IsNullOrWhiteSpace(payload)) return CryptoUtils.GetPayloadSign(CryptoUtils.EncryptPayloadUsingKey(payload, _encKey, Iv) + strTimestamp + strTimestamp[6..] + strTimestamp[3..], _signKey);

            throw new MazdaApiException("Missing sign key");
        }

        public async Task<string> ApiRequestAsync(HttpMethod method, string uri, IDictionary<string, string> body = null, bool needsKeys = true, bool needsAuth = false) => await SendApiRequestAsync(method, uri, JsonConvert.SerializeObject(body, Formatting.None), needsKeys, needsAuth);

        public async Task<string> ApiRequestAsync(HttpMethod method, string uri, string body = "", bool needsKeys = true, bool needsAuth = false) => await SendApiRequestAsync(method, uri, body, needsKeys, needsAuth);

        private async Task<string> SendApiRequestAsync(HttpMethod method, string uri, string body, bool needsKeys, bool needsAuth, int numRetries = 0)
        {
            if (numRetries <= MaxRetries)
            {
                if (needsKeys && (string.IsNullOrEmpty(_encKey) || string.IsNullOrEmpty(_signKey)))
                {
                    _logger?.LogDebug("Retrieving encryption keys");
                    CheckVersionResponse checkVersionResponse = JsonSerializer.Deserialize<CheckVersionResponse>(await SendApiRequestAsync(HttpMethod.Post, "/service/checkVersion", null, false, false), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    _logger?.LogDebug("Successfully retrieved encryption keys");
                    _encKey = checkVersionResponse?.EncKey;
                    _signKey = checkVersionResponse?.SignKey;
                }

                if (needsAuth)
                {
                    if (!string.IsNullOrWhiteSpace(_accessToken) && DateTime.Now - new TimeSpan(0, 0, 1, 0) > _accessTokenExpirationTs)
                    {
                        _logger?.LogDebug("Access token is expired.Fetching a new one.");
                       _accessToken = null;
                        _accessTokenExpirationTs = DateTime.MinValue;
                    }

                    if (string.IsNullOrWhiteSpace(_accessToken))
                    {
                        _logger?.LogDebug("No access token present. Logging in.");
                        await LoginAsync();
                    }
                }

                _logger?.LogTrace($"Sending {method} request to {uri}{(numRetries == 0 ? string.Empty : $" - attempt {numRetries + 1}")}{(string.IsNullOrWhiteSpace(body) ? string.Empty : $"\n\rBody: {body}")}");

                try
                {
                    return await SendRawApiRequestAsync(method, uri, body, needsAuth);
                }
                catch (MazdaApiEncryptionException)
                {
                    _logger?.LogWarning("Server reports request was not encrypted properly. Retrieving new encryption keys.");
                    CheckVersionResponse checkVersionResponse = JsonSerializer.Deserialize<CheckVersionResponse>(await SendApiRequestAsync(HttpMethod.Post, "/service/checkVersion", null, false, false), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    _encKey = checkVersionResponse?.EncKey;
                    _signKey = checkVersionResponse?.SignKey;
                    return await SendApiRequestAsync(method, uri, body, needsKeys, needsAuth, numRetries + 1);
                }
                catch (MazdaApiTokenExpiredException)
                {
                    _logger?.LogDebug("Server reports access token was expired. Retrieving new access token.");
                    await LoginAsync();
                    return await SendApiRequestAsync(method, uri, body, needsKeys, needsAuth, numRetries + 1);
                }
                catch (MazdaApiLoginFailedException)
                {
                    _logger?.LogDebug("Login failed for an unknown reason. Trying again.");
                    await LoginAsync();
                    return await SendApiRequestAsync(method, uri, body, needsKeys, needsAuth, numRetries + 1);
                }
                catch (MazdaApiRequestInProgressException)
                {
                    _logger?.LogDebug("Request failed because another request was already in progress. Waiting 30 seconds and trying again.");
                    await Task.Delay(30 * 1000);
                    return await SendApiRequestAsync(method, uri, body, needsKeys, needsAuth, numRetries + 1);
                }
            }

            _logger?.LogDebug("Request exceeded max number of retries");
            throw new MazdaApiException("Request exceeded max number of retries");
        }

        private async Task<string> SendRawApiRequestAsync(HttpMethod method, string uri, string body, bool needsAuth)
        {
            long timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            ApiResponse apiResponse;
            string encryptedBody = string.Empty;

            if (!string.IsNullOrWhiteSpace(body)) encryptedBody = CryptoUtils.EncryptPayloadUsingKey(body, _encKey, Iv);

            _httpClient.DefaultRequestHeaders.Clear();

            using (HttpRequestMessage request = new HttpRequestMessage { RequestUri = new Uri(_regionConfig.BaseUrl + uri), Method = method, Content = new StringContent(encryptedBody) })
            {
                request.Headers.Add("device-id", _baseApiDeviceId);
                request.Headers.Add("app-code", _regionConfig.ApplicationCode);
                request.Headers.Add("app-os", AppOs);
                request.Headers.Add("user-agent", UserAgentBaseApi);
                request.Headers.Add("app-version", AppVersion);
                request.Headers.Add("app-unique-id", AppPackageId);
                request.Headers.Add("access-token", needsAuth ? _accessToken : string.Empty);
                request.Headers.Add("X-acf-sensor-data", _sensorDataBuilder.GenerateSensorData());
                request.Headers.Add("req-id", "req_" + timestamp);
                request.Headers.Add("timestamp", timestamp.ToString());

                if (uri.Contains("checkVersion"))
                    request.Headers.Add("sign", GetSignFromTimestamp(timestamp));
                else if (method == HttpMethod.Get)
                    request.Headers.Add("sign", GetSignFromPayloadAndTimestamp("", timestamp));
                else if (method == HttpMethod.Post) request.Headers.Add("sign", GetSignFromPayloadAndTimestamp(body, timestamp));

                HttpResponseMessage apiResponseMessage = await _httpClient.SendAsync(request);

                apiResponse = JsonSerializer.Deserialize(await apiResponseMessage.Content.ReadAsByteArrayAsync(), typeof(ApiResponse), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) as ApiResponse;
            }

            if (apiResponse?.State == "S")
            {
                string key;

                if (uri.Contains("checkVersion"))
                {
                    using MD5 md5 = MD5.Create();
                    key = CryptoUtils.HexDigest(md5.ComputeHash(Encoding.UTF8.GetBytes(CryptoUtils.HexDigest(md5.ComputeHash(Encoding.UTF8.GetBytes(_regionConfig.ApplicationCode + AppPackageId))).ToUpper() + SignatureMd5))).ToLower().Substring(4, 16);
                }
                else
                {
                    key = _encKey;
                }

                string payload = CryptoUtils.DecryptPayloadUsingKey(apiResponse.Payload, key, Iv);

                _logger?.LogTrace($"Payload received: {payload}");

                return payload;
            }

            // got to here so must be an error.
            switch (apiResponse?.ErrorCode)
            {
                case 600001:
                    throw new MazdaApiEncryptionException("Server rejected encrypted request");
                case 600002:
                    throw new MazdaApiTokenExpiredException("Token expired");
                case 92000 when apiResponse.ExtraCode == "400S01":
                    throw new MazdaApiRequestInProgressException("Request already in progress, please wait and try again");
                case 92000 when apiResponse.ExtraCode == "400S11":
                    throw new MazdaApiException("The engine can only be remotely started 2 consecutive times. Please drive the vehicle to reset the counter.");
                default:
                {
                    if (!string.IsNullOrWhiteSpace(apiResponse?.Error)) throw new MazdaApiException($"Request failed: {apiResponse.Error}");

                    throw new MazdaApiException("Request failed for an unknown reason");
                }
            }
        }

        private async Task LoginAsync()
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgentUsherApi);
            UriBuilder builder = new UriBuilder(_regionConfig.UsherUrl);
            builder.Path += @"/system/encryptionKey";
            NameValueCollection query = HttpUtility.ParseQueryString(builder.Query);
            query["appId"] = "MazdaApp";
            query["locale"] = "en-US";
            query["deviceId"] = _usherApiDeviceId;
            query["sdkVersion"] = UsherSdkVersion;
            builder.Query = query.ToString()!;
            HttpResponseMessage response = await _httpClient.GetAsync(builder.ToString());
            EncryptionKeyResponse encryptionKey = JsonSerializer.Deserialize(await response.Content.ReadAsByteArrayAsync(), typeof(EncryptionKeyResponse), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) as EncryptionKeyResponse;

            _logger?.LogDebug($"Logging in as {_emailAddress}");
            _logger?.LogDebug("Retrieving public key to encrypt password");

            using (RSA rsa = CryptoUtils.CreateRsaFromDerData(Convert.FromBase64String(encryptionKey!.Data.PublicKey)))
            {
                builder = new UriBuilder(_regionConfig.UsherUrl);
                builder.Path += @"/user/login";
                using StringContent content = new StringContent(JsonSerializer.Serialize(new LoginRequest
                {
                    AppId = "MazdaApp",
                    Locale = "en-US",
                    DeviceId = _usherApiDeviceId,
                    SdkVersion = UsherSdkVersion,
                    Password = $"{encryptionKey.Data.VersionPrefix}{Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes($"{_usherApiPassword}:{DateTimeOffset.Now.ToUnixTimeSeconds()}"), RSAEncryptionPadding.Pkcs1))}",
                    UserId = _emailAddress,
                    UserIdType = "email"
                }), Encoding.UTF8, "application/json");
                response = await _httpClient.PostAsync(builder.ToString(), content);
            }

            _logger?.LogDebug("Sending login request");

            LoginResponse loginResponse = JsonSerializer.Deserialize(await response.Content.ReadAsByteArrayAsync(), typeof(LoginResponse), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) as LoginResponse;

            switch (loginResponse?.Status ?? string.Empty)
            {
                case "OK":
                    _logger?.LogDebug($"Successfully logged in as {_emailAddress}");
                    _accessToken = loginResponse!.Data.AccessToken;
                    _accessTokenExpirationTs = DateTimeOffset.FromUnixTimeSeconds(loginResponse!.Data.AccessTokenExpirationTs).DateTime;
                    break;

                case "INVALID_CREDENTIAL":
                    _logger?.LogDebug("Login failed due to invalid email or password");
                    throw new MazdaApiAuthenticationException("Invalid email or password");

                case "USER_LOCKED":
                    _logger?.LogDebug("Login failed to account being locked");
                    throw new MazdaApiAccountLockedException("Login failed to account being locked");

                default:
                    _logger?.LogDebug($"Login failed{(string.IsNullOrWhiteSpace(loginResponse?.Status) ? string.Empty : $":{loginResponse.Status}")}");
                    throw new MazdaApiLoginFailedException("Login failed");
            }
        }

        private class EncryptionKeyResponseData
        {
            public string PublicKey { get; set; }

            public string VersionPrefix { get; set; }
        }

        private class EncryptionKeyResponse
        {
            public EncryptionKeyResponseData Data { get; set; }
        }

        private class LoginRequest
        {
            [JsonPropertyName("appId")] public string AppId { get; set; }

            [JsonPropertyName("deviceId")] public string DeviceId { get; set; }

            [JsonPropertyName("locale")] public string Locale { get; set; }

            [JsonPropertyName("password")] public string Password { get; set; }

            [JsonPropertyName("sdkVersion")] public string SdkVersion { get; set; }

            [JsonPropertyName("userId")] public string UserId { get; set; }

            [JsonPropertyName("userIdType")] public string UserIdType { get; set; }
        }

        private class LoginResponseData
        {
            public string AccessToken { get; set; }
            public long AccessTokenExpirationTs { get; set; }
        }


        private class LoginResponse
        {
            public string Status { get; set; }
            public LoginResponseData Data { get; set; }
        }

        private class ApiResponse
        {
            public string State { get; set; }
            public string Error { get; set; }
            public int ErrorCode { get; set; }
            public string ExtraCode { get; set; }
            public string Payload { get; set; }
        }

        private class CheckVersionResponse
        {
            public string EncKey { get; set; }
            public string SignKey { get; set; }
        }


        private class RegionConfig
        {
            public string ApplicationCode { get; set; }
            public Uri BaseUrl { get; set; }
            public Uri UsherUrl { get; set; }
        }
    }
}