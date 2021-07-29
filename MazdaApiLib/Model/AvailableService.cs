// 
// AvailableService.cs
// 
// Copyright 2021 Wingandprayer Software
// 
// This file is part of MazdaApiLib.
// 
// MazdaApiLib is free software: you can redistribute it and/or modify it under the terms of the
// GNU General Public License as published by the Free Software Foundation, either version 2
// of the License, or (at your option) any later version.
// 
// CDRipper is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
// PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with MazdaApiLib.
// If not, see http://www.gnu.org/licenses/.

using System;

using Newtonsoft.Json;

// ReSharper disable UnusedMember.Global

namespace WingandPrayer.MazdaApi.Model
{
    public class BoolConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue((bool)value ? 1 : 0);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value!.ToString() == "1";
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }
    }

    public class AvailableService
    {
        [JsonConverter(typeof(BoolConverter))]
        public bool VehicleStatus { get; set; }
        [JsonConverter(typeof(BoolConverter))]
        public bool RemoteControl { get; set; }
        [JsonConverter(typeof(BoolConverter))]
        public bool VehicleFinder { get; set; }
        [JsonConverter(typeof(BoolConverter))]
        public bool PoiSendToCar { get; set; }
        [JsonConverter(typeof(BoolConverter))]
        public bool HealthReports { get; set; }
        [JsonConverter(typeof(BoolConverter))]
        public bool VehicleStatusAlert { get; set; }
    }
}
