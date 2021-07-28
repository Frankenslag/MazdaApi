using System.Collections.Generic;
using System.Threading.Tasks;

using WingandPrayer.MazdaApi.Model;
using WingandPrayer.MazdaApi.RawModel;

using Newtonsoft.Json;

namespace WingandPrayer.MazdaApi
{
    public partial class MazdaApiClient
    {
        private readonly bool _useCachedVehicleList;
        private MazdaApiVehicles _vehicleCache;

        public async ValueTask<MazdaApiVehicles> GetRawVehicles()
        {
            if (_vehicleCache == null || !_useCachedVehicleList)
            {
                _vehicleCache = JsonConvert.DeserializeObject<MazdaApiVehicles>(await _controller.GetVehicleBaseInformation());
            }

            return _vehicleCache;
        }
        public async ValueTask<List<VehicleModel>> GetVehicles()
        {
            List<VehicleModel> retval = new();

            foreach (MazdaApiRawVehicleBaseInfo baseInfo in (await GetRawVehicles()).VecBaseInfos)
            {
                MazdaApiVehicleOtherInformation otherInformation = baseInfo.Vehicle.VehicleInformation.OtherInformation;

                retval.Add(new VehicleModel()
                {
                    Vin = baseInfo.Vin,
                    Id = baseInfo.Vehicle.CvInformation.InternalVin,
                    Nickname = await _controller.GetNickname(baseInfo.Vin),
                    CarlineCode = otherInformation.CarlineCode,
                    CarlineName = otherInformation.CarlineName,
                    ModelYear = otherInformation.ModelYear,
                    ModelCode = otherInformation.ModelCode,
                    ModelName = otherInformation.ModelName,
                    AutomaticTransmission = otherInformation.TransmissionType == "A",
                    InteriorColorCode = otherInformation.InteriorColorCode,
                    InteriorColorName = otherInformation.InteriorColorName,
                    ExteriorColorCode = otherInformation.ExteriorColorCode,
                    ExteriorColorName = otherInformation.ExteriorColorName
                });
            }

            return retval;
        }
    }
}