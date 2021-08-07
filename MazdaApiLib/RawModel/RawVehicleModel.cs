// 
// RawVehicleModel.cs
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
using Newtonsoft.Json;

// ReSharper disable UnusedMember.Global

namespace WingandPrayer.MazdaApi.RawModel
{
    public class MazdaApiVehicleCvServiceInformation
    {
        public string AutomobileIdentificationCode { get; set; }
        public string TransmissionType { get; set; }
        public string CarName { get; set; }
        public string CarModelType { get; set; }
        public string CarBodyColor { get; set; }
        public string DriveType { get; set; }
        public string FuelType { get; set; }
        public string FuelName { get; set; }
        public string EngineStartSec { get; set; }
        public string EngineStartLimitSec { get; set; }
    }

    public class MazdaApiVehicleOtherInformation
    {
        public string ModelYear { get; set; }
        public string ModelCode { get; set; }
        public string ModelName { get; set; }
        public string TransmissionName { get; set; }
        public string TransmissionType { get; set; }
        public string TrimName { get; set; }
        public string EngineInformation { get; set; }
        public string InteriorColorCode { get; set; }
        public string InteriorColorName { get; set; }
        public string ExteriorColorCode { get; set; }
        public string ExteriorColorName { get; set; }
        public string CarlineCode { get; set; }
        public string CarlineName { get; set; }
        public string ImageFileUrl { get; set; }
        public string ImageFileUrl2 { get; set; }
        public string ModelCode4 { get; set; }
        public string JellybeansFileName { get; set; }
    }

    public class MazdaApiVehicleCvInformation
    {
        public string IccId { get; set; }
        public string TcuDestination { get; set; }
        public string TcuVersion { get; set; }
        public string TcuModelYear { get; set; }
        public string InternalVin { get; set; }
        public string ModelSpecificationCode { get; set; }
        public string Imei { get; set; }
        public string CmuVersion { get; set; }
    }

    public class MazdaApiVehicleInformation
    {
        public MazdaApiVehicleOtherInformation OtherInformation { get; set; }
        public MazdaApiVehicleCvServiceInformation CvServiceInformation { get; set; }
    }

    internal class VehicleInfoConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) => JsonConvert.DeserializeObject<MazdaApiVehicleInformation>((string)reader.Value ?? string.Empty);

        public override bool CanConvert(Type objectType) => true;
    }

    public class MazdaApiVehicle
    {
        public string Id => CvInformation.InternalVin;
        public string HandlePosition { get; set; }
        public DateTime PrimaryUserRegisterDateTime { get; set; }
        public DateTime RetailDate { get; set; }
        public DateTime StartCompletionDateTime { get; set; }
        public DateTime StartDateTime { get; set; }
        public string RsaPhoneNumberExternal { get; set; }
        public string RsaPhoneNumberDomestic { get; set; }
        public string RsaStatus { get; set; }
        public string CountryCode { get; set; }

        [JsonConverter(typeof(VehicleInfoConverter))]
        public MazdaApiVehicleInformation VehicleInformation { get; set; }

        public MazdaApiVehicleCvInformation CvInformation { get; set; }
    }

    public class MazdaApiRawVehicleBaseInfo
    {
        public MazdaApiVehicle Vehicle { get; set; }
        public string VehicleGeneration { get; set; }
        public string EconnectType { get; set; }
        public string Vin { get; set; }
        public string VehicleType { get; set; }
    }

    public class VehicleFlags
    {
        public int VinRegistStatus { get; set; }
        public int PrimaryFlag { get; set; }
    }

    public class MazdaApiVehicles
    {
        public string ResultCode { get; set; }
        public List<MazdaApiRawVehicleBaseInfo> VecBaseInfos { get; set; }
        public List<VehicleFlags> VehicleFlags { get; set; }
    }
}