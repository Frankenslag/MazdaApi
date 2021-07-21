using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace WingandPrayer.MazdaApi
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

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return JsonConvert.DeserializeObject<MazdaApiVehicleInformation>((string)(reader.Value) ?? string.Empty);
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
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
        [Newtonsoft.Json.JsonConverter(typeof(VehicleInfoConverter))]
        public MazdaApiVehicleInformation VehicleInformation { get; set; }
        public MazdaApiVehicleCvInformation CvInformation { get; set; }
    }

    public class MazdaApiVehicleBaseInfo
    {
        public MazdaApiVehicle Vehicle { get; set; }
        public string VehicleGeneration { get; set; }
        public string EconnectType { get; set; }
        public string Vin { get; set; }
        public string VehicleType { get; set; }
    }

    public class MazdaApiVehicles
    {
        public string ResultCode { get; set; }
        //[JsonPropertyName("vecBaseInfos")]
        public List<MazdaApiVehicleBaseInfo> VecBaseInfos { get; set; }
    }
    public partial class MazdaApiClient
    {
        private readonly bool _useCachedVehicleList;
        private MazdaApiVehicles _vehicleCache;

        public async ValueTask<MazdaApiVehicles> GetVehicles()
        {
            if (_vehicleCache == null || !_useCachedVehicleList)
            {
                _vehicleCache = JsonConvert.DeserializeObject<MazdaApiVehicles>(await _controller.GetVehicleBaseInformation());
            }

            return _vehicleCache;
        }
    }
}