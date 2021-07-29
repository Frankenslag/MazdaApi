﻿// //
// // CryptoUtils.cs
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
using System.Formats.Asn1;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace WingandPrayer.MazdaApi.Crypto
{
    public class CryptoUtils
    {
        public static string GenerateUuidFromSeed(string seed)
        {
            using SHA256 encoder256 = SHA256.Create();
            string strHash = BitConverter.ToString(encoder256.ComputeHash(Encoding.UTF8.GetBytes(seed))).Replace("-", "").ToUpper();
            return strHash.Substring(0, 8) + "-" + strHash.Substring(8, 4) + "-" + strHash.Substring(12, 4) + "-" +
                   strHash.Substring(16, 4) + "-" + strHash.Substring(20, 12);
        }

        public static string GenerateUsherDeviceIdFromSeed(string seed)
        {
            using SHA256 encoder256 = SHA256.Create();
            string strHash = BitConverter.ToString(encoder256.ComputeHash(Encoding.UTF8.GetBytes(seed))).Replace("-", "").ToUpper();
            return $"ACCT{int.Parse(strHash.Substring(0, 8), NumberStyles.HexNumber)}";
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

            AsnReader topLevelReader = new(data, AsnEncodingRules.DER);
            AsnReader spkiReader = topLevelReader.ReadSequence();
            topLevelReader.ThrowIfNotEmpty();

            AsnReader algorithmReader = spkiReader.ReadSequence();

            if (algorithmReader.ReadObjectIdentifier() != "1.2.840.113549.1.1.1")
            {
                throw new InvalidOperationException();
            }

            algorithmReader.ReadNull();
            algorithmReader.ThrowIfNotEmpty();

            AsnReader bitStringReader = new(spkiReader.ReadBitString(out _), AsnEncodingRules.DER);

            AsnReader publicKeyReader = bitStringReader.ReadSequence();

            RSAParameters rsaParameters = new()
            {
                Modulus = ReadNormalizedInteger(publicKeyReader),
                Exponent = ReadNormalizedInteger(publicKeyReader),
            };

            publicKeyReader.ThrowIfNotEmpty();

            RSA rsa = RSA.Create();
            rsa.ImportParameters(rsaParameters);
            return rsa;
        }
    }
}
