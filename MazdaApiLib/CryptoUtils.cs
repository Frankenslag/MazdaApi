// 
// CryptoUtils.cs
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
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using WingandPrayer.MazdaApi.Exceptions;

namespace WingandPrayer.MazdaApi
{
    internal class CryptoUtils
    {
        public static string GenerateUuidFromSeed(string seed)
        {
            using SHA256 encoder256 = SHA256.Create();
            string strHash = BitConverter.ToString(encoder256.ComputeHash(Encoding.UTF8.GetBytes(seed))).Replace("-", "").ToUpper();
            return strHash[..8] + "-" + strHash.Substring(8, 4) + "-" + strHash.Substring(12, 4) + "-" +
                   strHash.Substring(16, 4) + "-" + strHash.Substring(20, 12);
        }

        public static string GenerateUsherDeviceIdFromSeed(string seed)
        {
            using SHA256 encoder256 = SHA256.Create();
            string strHash = BitConverter.ToString(encoder256.ComputeHash(Encoding.UTF8.GetBytes(seed))).Replace("-", "").ToUpper();
            return $"ACCT{int.Parse(strHash[..8], NumberStyles.HexNumber)}";
        }

        /// <summary>
        ///
        ///  Extract the public key from the the data like in the format example below and create an RSA class.
        /// 
        ///     0 30  159: SEQUENCE {
        ///     3 30   13:   SEQUENCE {
        ///     5 06    9:     OBJECT IDENTIFIER '1 2 840 113549 1 1 1'
        ///    16 05    0:     NULL
        ///              :     }
        ///    18 03  141:   BIT STRING 0 unused bits, encapsulates {
        ///    22 30  137:       SEQUENCE {
        ///    25 02  129:         INTEGER
        ///              :           00 EB 11 E7 B4 46 2E 09 BB 3F 90 7E 25 98 BA 2F
        ///              :           C4 F5 41 92 5D AB BF D8 FF 0B 8E 74 C3 F1 5E 14
        ///              :           9E 7F B6 14 06 55 18 4D E4 2F 6D DB CD EA 14 2D
        ///              :           8B F8 3D E9 5E 07 78 1F 98 98 83 24 E2 94 DC DB
        ///              :           39 2F 82 89 01 45 07 8C 5C 03 79 BB 74 34 FF AC
        ///              :           04 AD 15 29 E4 C0 4C BD 98 AF F4 B7 6D 3F F1 87
        ///              :           2F B5 C6 D8 F8 46 47 55 ED F5 71 4E 7E 7A 2D BE
        ///              :           2E 75 49 F0 BB 12 B8 57 96 F9 3D D3 8A 8F FF 97
        ///              :           73
        ///   157 02    3:         INTEGER 65537
        ///              :         }
        ///              :       }
        ///              :   }
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static RSA CreateRsaFromDerData(byte[] data)
        {
            static byte[] ReadNormalizedInteger(AsnReader asnReader)
            {
                ReadOnlyMemory<byte> memory = asnReader.ReadIntegerBytes();
                ReadOnlySpan<byte> span = memory.Span;

                if (span[0] == 0)
                {
                    span = span[1..];
                }

                return span.ToArray();
            }

            AsnReader topLevelReader = new AsnReader(data, AsnEncodingRules.DER);
            AsnReader spkiReader = topLevelReader.ReadSequence();
            topLevelReader.ThrowIfNotEmpty();

            AsnReader algorithmReader = spkiReader.ReadSequence();

            if (algorithmReader.ReadObjectIdentifier() != "1.2.840.113549.1.1.1")
            {
                throw new InvalidOperationException();
            }

            algorithmReader.ReadNull();
            algorithmReader.ThrowIfNotEmpty();

            AsnReader bitStringReader = new AsnReader(spkiReader.ReadBitString(out _), AsnEncodingRules.DER);

            AsnReader publicKeyReader = bitStringReader.ReadSequence();

            RSAParameters rsaParameters = new RSAParameters
            {
                Modulus = ReadNormalizedInteger(publicKeyReader),
                Exponent = ReadNormalizedInteger(publicKeyReader)
            };

            publicKeyReader.ThrowIfNotEmpty();

            RSA rsa = RSA.Create();
            rsa.ImportParameters(rsaParameters);
            return rsa;
        }

        public static string EncryptPayloadUsingKey(string payload, string key, string iv)
        {
            string retval = string.Empty;

            if (!string.IsNullOrWhiteSpace(payload))
            {
                using AesManaged aes = new AesManaged
                {
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7,
                    BlockSize = 128,
                    Key = Encoding.ASCII.GetBytes(key),
                    IV = Encoding.ASCII.GetBytes(iv)
                };

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                // Create the streams used for encryption.
                using MemoryStream msEncrypt = new MemoryStream();
                using CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

                byte[] plainText = Encoding.UTF8.GetBytes(payload);

                byte[] buffer = encryptor.TransformFinalBlock(plainText, 0, plainText.Length);

                return Convert.ToBase64String(buffer);
            }

            return retval;
        }

        public static string DecryptPayloadUsingKey(string payload, string key, string iv)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                using Aes aes = Aes.Create();
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(iv);

                // Create an decryptor to perform the stream transform.
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                aes.Mode = CipherMode.CBC;

                // Create the streams used for encryption.
                using MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(payload));
                using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using StreamReader sRDecrypt = new StreamReader(csDecrypt);
                return sRDecrypt.ReadToEnd();
            }

            throw new MazdaApiException("Missing encryption key");
        }

        public static string HexDigest(IEnumerable<byte> ary) => ary.Aggregate("", (current, next) => $"{current}{next:X2}");

        public static string GetPayloadSign(string payload, string signKey)
        {
            using SHA256 encoder256 = SHA256.Create();
            return HexDigest(encoder256.ComputeHash(Encoding.UTF8.GetBytes(payload + signKey))).ToUpper();
        }
    }
}
