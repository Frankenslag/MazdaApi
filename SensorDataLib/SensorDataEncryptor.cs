// //
// // SensorDataEncryptor.cs
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
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WingandPrayer.MazdaApi.Crypto;

namespace WingandPrayer.MazdaApi.SensorData
{
    internal class SensorDataEncryptor
    {
        private const string RsaPublicKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC4sA7vA7N/t1SRBS8tugM2X4bByl0jaCZLqxPOql+qZ3sP4UFayqJTvXjd7eTjMwg1T70PnmPWyh1hfQr4s12oSVphTKAjPiWmEBvcpnPPMjr5fGgv0w6+KM9DLTxcktThPZAGoVcoyM/cTO/YsAMIxlmTzpXBaxddHRwi8S2NvwIDAQAB";

        private readonly byte[] aesKey;
        private readonly byte[] aesIv;
        private readonly byte[] hmacSha256Key;
        private readonly byte[] encryptedAesKey;
        private readonly byte[] encryptedHmacSha256Key;

        public SensorDataEncryptor()
        {
            Random rnd = new();
            aesKey = new byte[16];
            aesIv = new byte[16];
            hmacSha256Key = new byte[32];

            rnd.NextBytes(aesKey);
            rnd.NextBytes(aesIv);
            rnd.NextBytes(hmacSha256Key);

            using RSA rsa = CryptoUtils.CreateRsaFromDerData(Convert.FromBase64String(RsaPublicKey));
            encryptedAesKey = rsa.Encrypt(aesKey, RSAEncryptionPadding.Pkcs1);
            encryptedHmacSha256Key = rsa.Encrypt(hmacSha256Key, RSAEncryptionPadding.Pkcs1);
        }

        public string EncryptSensorData(string sensorData)
        {
            Random rnd = new();

            byte[] encrIvAndSensorData;
            byte[] encrIvAndSensorDataAndHmac;

            using (Aes aes = Aes.Create())
            {
                aes.Key = aesKey;
                aes.IV = aesIv;

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
                encrIvAndSensorData = aesIv.Concat(msEncrypt.ToArray()).ToArray();
            }

            using (HMACSHA256 hmac = new(hmacSha256Key))
            {
                encrIvAndSensorDataAndHmac = encrIvAndSensorData.Concat(hmac.ComputeHash(encrIvAndSensorData)).ToArray();
            }

            return $"1,a,{Convert.ToBase64String(encryptedAesKey)},{Convert.ToBase64String(encryptedHmacSha256Key)}${Convert.ToString(encrIvAndSensorDataAndHmac)}${rnd.Next(0, 3) * 1000},{rnd.Next(0, 3) * 1000},{rnd.Next(0, 3) * 1000}";
        }
    }
}

