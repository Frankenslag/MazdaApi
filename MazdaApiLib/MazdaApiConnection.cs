// //
// // MazdaApiConnection.cs
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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using WingandPrayer.MazdaApi.Crypto;
using WingandPrayer.MazdaApi.Exceptions;
using WingandPrayer.MazdaApi.SensorData;
using JsonSerializer = System.Text.Json.JsonSerializer;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable ClassNeverInstantiated.Local

namespace WingandPrayer.MazdaApi
{
    internal class MazdaApiConnection
    {

        private class EncryptionKeyResponseData
        {
            public string PublicKey { get; init; }

            public string VersionPrefix { get; init; }
        }

        private class EncryptionKeyResponse
        {
            public EncryptionKeyResponseData  Data { get; init; }
        }

        private class LoginRequest
        {
            [JsonPropertyName("appId")]
            public string AppId { get; init; }
            [JsonPropertyName("deviceId")]
            public string DeviceId { get; init; }
            [JsonPropertyName("locale")]
            public string Locale { get; init; }
            [JsonPropertyName("password")]
            public string Password { get; init; }
            [JsonPropertyName("sdkVersion")]
            public string SdkVersion { get; init; }
            [JsonPropertyName("userId")]
            public string UserId { get; init; }
            [JsonPropertyName("userIdType")]
            public string UserIdType { get; init; }
        }

        private class LoginResponseData
        {
            public string AccessToken { get; init; }
            public long AccessTokenExpirationTs { get; init; }
        }


        private class LoginResponse
        {
            public string Status { get; init; }
            public LoginResponseData Data { get; init; }
        }

        private class ApiResponse
        {
            public string State { get; init; }
            public string Error { get; init; }
            public int ErrorCode { get; init; }
            public string ExtraCode { get; init; }
            public string Payload { get; init; }
        }

        private class CheckVersionResponse
        {
            public string EncKey { get; init; }
            public string SignKey { get; init; }
        }



        private class RegionConfig
        {
            public string ApplicationCode { get; init; }
            public Uri BaseUrl { get; init; }
            public Uri UsherUrl { get; init; }
        }

        private static readonly Dictionary<string, RegionConfig> RegionsConfigs = new()
        {
            {"MNAO", new RegionConfig {ApplicationCode = "202007270941270111799", BaseUrl = new Uri("https://0cxo7m58.mazda.com/prod"), UsherUrl = new Uri("https://ptznwbh8.mazda.com/appapi/v1")}},
            {"MME", new RegionConfig {ApplicationCode = "202008100250281064816", BaseUrl = new Uri("https://e9stj7g7.mazda.com/prod"), UsherUrl = new Uri("https://rz97suam.mazda.com/appapi/v1")}},
            {"MJO", new RegionConfig {ApplicationCode = "202009170613074283422", BaseUrl = new Uri("https://wcs9p6wj.mazda.com/prod"), UsherUrl = new Uri("https://c5ulfwxr.mazda.com/appapi/v1")}}
        };

        private const string AppOs = "Android";
        private const string Iv = "0102030405060708";
        private const string SignatureMd5 = "C383D8C4D279B78130AD52DC71D95CAA";
        private const string AppVersion = "7.3.0";
        private const string AppPackageId = "com.interrait.mymazda";
        private const string UsherSdkVersion = "11.2.0000.002";
        private const string UserAgentBaseApi = "MyMazda-Android/7.3.0";
        private const string UserAgentUsherApi = "MyMazda/7.3.0 (Google Pixel 3a; Android 11)";
        private const int MaxRetries = 4;

        private readonly RegionConfig _regionConfig;
        private readonly string _baseApiDeviceId;
        private readonly string _usherApiDeviceId;
        private readonly string _emailAddress;
        private readonly string _usherApiPassword;
        private string _accessToken;
        private DateTime _accessTokenExpirationTs;
        private string _encKey;
        private string _signKey;
        private readonly SensorDataBuilder _sensorDataBuilder;

        public MazdaApiConnection(string emailAddress, string password, string region)
        {
            if (RegionsConfigs.ContainsKey(region))
            {
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

        private static string HexDigest(IEnumerable<byte> ary) => ary.Aggregate("", (current, next) => $"{current}{next:X2}");

        private static string GetPayloadSign(string payload, string signKey)
        {
            using SHA256 encoder256 = SHA256.Create();
            return HexDigest(encoder256.ComputeHash(Encoding.UTF8.GetBytes(payload + signKey))).ToUpper();
        }

        private string GetSignFromTimestamp(long timestamp)
        {
            string strTimestamp = timestamp.ToString();
            string strTimestampExtended = strTimestamp + strTimestamp[6..] + strTimestamp[3..];
            string val2;

            using (MD5 md5 = MD5.Create())
            {
                val2 = HexDigest(md5.ComputeHash(Encoding.UTF8.GetBytes(HexDigest(md5.ComputeHash(Encoding.UTF8.GetBytes(_regionConfig.ApplicationCode + AppPackageId))).ToUpper() +  SignatureMd5))).ToLower();
            }

            return GetPayloadSign(strTimestampExtended, val2.Substring(20, 12) + val2[..10] + val2.Substring(4, 2));
        }

        private string DecryptPayloadUsingAppCode(string payload)
        {
            string decryptionKey;

            using (MD5 md5 = MD5.Create())
            {
                decryptionKey = HexDigest(md5.ComputeHash(Encoding.UTF8.GetBytes(HexDigest(md5.ComputeHash(Encoding.UTF8.GetBytes(_regionConfig.ApplicationCode + AppPackageId))).ToUpper() + SignatureMd5))).ToLower().Substring(4, 16);
            }

            using (Aes aes = Aes.Create())
            {
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = Encoding.UTF8.GetBytes(decryptionKey);
                aes.IV = Encoding.UTF8.GetBytes(Iv);

                // Create an decryptor to perform the stream transform.
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                aes.Mode = CipherMode.CBC;

                // Create the streams used for encryption.
                using MemoryStream msDecrypt = new(Convert.FromBase64String(payload));
                using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
                using (StreamReader sRDecrypt = new(csDecrypt))
                {

                    return sRDecrypt.ReadToEnd();
                }
            }
        }

        private string DecryptPayloadUsingKey(string payload)
        {
            if (!string.IsNullOrWhiteSpace(_encKey))
            {
                using Aes aes = Aes.Create();
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = Encoding.UTF8.GetBytes(_encKey);
                aes.IV = Encoding.UTF8.GetBytes(Iv);

                // Create an decryptor to perform the stream transform.
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                aes.Mode = CipherMode.CBC;

                // Create the streams used for encryption.
                using MemoryStream msDecrypt = new(Convert.FromBase64String(payload));
                using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
                using StreamReader sRDecrypt = new(csDecrypt);
                return sRDecrypt.ReadToEnd();
            }

            throw new MazdaApiException("Missing encryption key");
        }

        private string EncryptPayloadUsingKey(string payload)
        {
            string retval = string.Empty;

            if (!string.IsNullOrWhiteSpace(payload))
            {
                using AesManaged aes = new()
                {
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7,
                    BlockSize = 128,
                    Key = Encoding.ASCII.GetBytes(_encKey),
                    IV = Encoding.ASCII.GetBytes(Iv)
                };

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                // Create the streams used for encryption.
                using MemoryStream msEncrypt = new();
                using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);

                byte[] plainText = Encoding.UTF8.GetBytes(payload);

                byte[] buffer = encryptor.TransformFinalBlock(plainText, 0, plainText.Length);

                // csEncrypt.Write(plainText, 0, plainText.Length);
                // csEncrypt.FlushFinalBlock();

                //return Convert.ToBase64String(msEncrypt.ToArray());

                return Convert.ToBase64String(buffer);
            }

            return retval;
        }

        private string GetSignFromPayloadAndTimestamp(string payload, long timestamp)
        {
            string strTimestamp = timestamp.ToString();

            if (!string.IsNullOrWhiteSpace(payload))
            {
                return GetPayloadSign(EncryptPayloadUsingKey(payload) + strTimestamp + strTimestamp[6..] + strTimestamp[3..], _signKey);
            }

            throw new MazdaApiException("Missing sign key");
        }

        public async Task<string> ApiRequestAsync(HttpMethod method, string uri, IDictionary<string, string> body = null, bool needsKeys = true, bool needsAuth = false)
        {
            return await SendApiRequestAsync(method, uri, JsonConvert.SerializeObject(body, Formatting.None), needsKeys, needsAuth);
        }

        public async Task<string> ApiRequestAsync(HttpMethod method, string uri, string body = "", bool needsKeys = true, bool needsAuth = false)
        {
            return await SendApiRequestAsync(method, uri, body, needsKeys, needsAuth);
        }

        private async Task<string> SendApiRequestAsync(HttpMethod method, string uri, string body, bool needsKeys, bool needsAuth, int numRetries = 0)
        {
            if (numRetries <= MaxRetries)
            {
                if (needsKeys && (string.IsNullOrEmpty(_encKey) || string.IsNullOrEmpty(_signKey)))
                {
                    CheckVersionResponse checkVersionResponse = JsonSerializer.Deserialize<CheckVersionResponse>(await SendApiRequestAsync(HttpMethod.Post, "/service/checkVersion", null, false, false), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    _encKey = checkVersionResponse?.EncKey;
                    _signKey = checkVersionResponse?.SignKey;
                }

                if (needsAuth)
                {
                    if (DateTime.Now - new TimeSpan(0, 0, 1, 0) > _accessTokenExpirationTs)
                    {
                        _accessToken = null;
                        _accessTokenExpirationTs = DateTime.MinValue;
                    }

                    if (string.IsNullOrWhiteSpace(_accessToken))
                    {
                        await LoginAsync();
                    }
                }

                try
                {
                    return await SendRawApiRequestAsync(method, uri, body, needsAuth);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            throw new MazdaApiException("Request exceeded max number of retries");
        }

        private async Task<string> SendRawApiRequestAsync(HttpMethod method, string uri, string body, bool needsAuth)
        {
            long timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            ApiResponse apiResponse;
            string encryptedBody = string.Empty;

            if (!string.IsNullOrWhiteSpace(body))
            {
                encryptedBody = EncryptPayloadUsingKey(body);
            }

            using HttpClient httpClient = new();

            httpClient.DefaultRequestHeaders.Clear();
            using (HttpRequestMessage request = new() {RequestUri = new Uri(_regionConfig.BaseUrl + uri), Method = method, Content = new StringContent(encryptedBody)})
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
                {
                    request.Headers.Add("sign", GetSignFromTimestamp(timestamp));
                }
                else if (method == HttpMethod.Get)
                {
                    request.Headers.Add("sign", GetSignFromPayloadAndTimestamp("", timestamp));
                }
                else if (method == HttpMethod.Post)
                {
                    request.Headers.Add("sign", GetSignFromPayloadAndTimestamp(body, timestamp));
                }

                HttpResponseMessage apiResponseMessage = await httpClient.SendAsync(request);

                apiResponse = await apiResponseMessage.Content.ReadFromJsonAsync<ApiResponse>(new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
            }

            if (apiResponse?.State == "S")
            {
                return uri.Contains("checkVersion") ? DecryptPayloadUsingAppCode(apiResponse.Payload) : DecryptPayloadUsingKey(apiResponse.Payload);
            }

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
                    if (!string.IsNullOrWhiteSpace(apiResponse?.Error))
                    {
                        throw new MazdaApiException($"Request failed: {apiResponse.Error}");
                    }

                    throw new MazdaApiException("Request failed for an unknown reason");
                }
            }
        }

        private async Task LoginAsync()
        {
            try
            {
                using HttpClient httpClient = new();
                httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgentUsherApi);
                UriBuilder builder = new(_regionConfig.UsherUrl);
                builder.Path += @"/system/encryptionKey";
                NameValueCollection query = HttpUtility.ParseQueryString(builder.Query);
                query["appId"] = "MazdaApp";
                query["locale"] = "en-US";
                query["deviceId"] = _usherApiDeviceId;
                query["sdkVersion"] = UsherSdkVersion;
                builder.Query = query.ToString()!;
                HttpResponseMessage response = await httpClient.GetAsync(builder.ToString());
                EncryptionKeyResponse encryptionKey = await response.Content.ReadFromJsonAsync<EncryptionKeyResponse>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                using (RSA rsa = CryptoUtils.CreateRsaFromDerData(Convert.FromBase64String(encryptionKey!.Data.PublicKey)))
                {
                    builder = new UriBuilder(_regionConfig.UsherUrl);
                    builder.Path += @"/user/login";
                    using (StringContent content = new(JsonSerializer.Serialize(new LoginRequest
                    {
                        AppId = "MazdaApp",
                        Locale = "en-US",
                        DeviceId = _usherApiDeviceId,
                        SdkVersion = UsherSdkVersion,
                        Password = $"{encryptionKey.Data.VersionPrefix}{Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes($"{_usherApiPassword}:{DateTimeOffset.Now.ToUnixTimeSeconds()}"), RSAEncryptionPadding.Pkcs1))}",
                        UserId = _emailAddress,
                        UserIdType = "email"
                    }), Encoding.UTF8, "application/json"))
                    {
                        response = await httpClient.PostAsync(builder.ToString(), content);
                    }
                }

                LoginResponse loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                switch (loginResponse?.Status ?? string.Empty)
                {
                    case "OK":
                        _accessToken = loginResponse!.Data.AccessToken;
                        _accessTokenExpirationTs = DateTimeOffset.FromUnixTimeSeconds(loginResponse!.Data.AccessTokenExpirationTs).DateTime;
                        break;

                    case "INVALID_CREDENTIAL":
                        throw new MazdaApiAuthenticationException("Invalid email or password");

                    case "USER_LOCKED":
                        throw new MazdaApiAccountLockedException("Login failed to account being locked");

                    default:
                        throw new MazdaApiLoginFailedException("Login failed");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}