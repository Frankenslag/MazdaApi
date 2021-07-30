// 
// SensorDataEncryptor.cs
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
using System.IO;
using System.Linq;
using System.Security.Cryptography;

using WingandPrayer.MazdaApi.Crypto;

namespace WingandPrayer.MazdaApi.SensorData
{
    internal class SensorDataEncryptor
    {
        private const string RsaPublicKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC4sA7vA7N/t1SRBS8tugM2X4bByl0jaCZLqxPOql+qZ3sP4UFayqJTvXjd7eTjMwg1T70PnmPWyh1hfQr4s12oSVphTKAjPiWmEBvcpnPPMjr5fGgv0w6+KM9DLTxcktThPZAGoVcoyM/cTO/YsAMIxlmTzpXBaxddHRwi8S2NvwIDAQAB";

        private readonly byte[] _aesKey;
        private readonly byte[] _aesIv;
        private readonly byte[] _hmacSha256Key;
        private readonly byte[] _encryptedAesKey;
        private readonly byte[] _encryptedHmacSha256Key;

        public SensorDataEncryptor()
        {
            Random rnd = new();
            _aesKey = new byte[16];
            _aesIv = new byte[16];
            _hmacSha256Key = new byte[32];

            rnd.NextBytes(_aesKey);
            rnd.NextBytes(_aesIv);
            rnd.NextBytes(_hmacSha256Key);

            using RSA rsa = CryptoUtils.CreateRsaFromDerData(Convert.FromBase64String(RsaPublicKey));
            _encryptedAesKey = rsa.Encrypt(_aesKey, RSAEncryptionPadding.Pkcs1);
            _encryptedHmacSha256Key = rsa.Encrypt(_hmacSha256Key, RSAEncryptionPadding.Pkcs1);
        }

        public string EncryptSensorData(string sensorData)
        {
            Random rnd = new();

            byte[] encrIvAndSensorData;
            byte[] encrIvAndSensorDataAndHmac;

            using (Aes aes = Aes.Create())
            {
                aes.Key = _aesKey;
                aes.IV = _aesIv;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                aes.Mode = CipherMode.CBC;

                // Create the streams used for encryption.
                using MemoryStream msEncrypt = new();
                using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
                using (StreamWriter swEncrypt = new(csEncrypt))
                {
                    //Write all data to the stream.
                    swEncrypt.Write(sensorData);
                }
                encrIvAndSensorData = _aesIv.Concat(msEncrypt.ToArray()).ToArray();
            }

            using (HMACSHA256 hmac = new(_hmacSha256Key))
            {
                encrIvAndSensorDataAndHmac = encrIvAndSensorData.Concat(hmac.ComputeHash(encrIvAndSensorData)).ToArray();
            }

            return $"1,a,{Convert.ToBase64String(_encryptedAesKey)},{Convert.ToBase64String(_encryptedHmacSha256Key)}${Convert.ToBase64String(encrIvAndSensorDataAndHmac)}${rnd.Next(0, 3) * 1000},{rnd.Next(0, 3) * 1000},{rnd.Next(0, 3) * 1000}";
        }
    }
}

