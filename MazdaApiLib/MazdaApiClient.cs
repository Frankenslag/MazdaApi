using WingandPrayer.MazdaApi.Exceptions;

namespace WingandPrayer.MazdaApi
{
    public partial class MazdaApiClient
    {
        private readonly MazdaApiController _controller;

        public MazdaApiClient(string emailAddress, string password, string region, bool useCachedVehicleList = false)
        {
            if (!string.IsNullOrWhiteSpace(emailAddress))
            {
                if (!string.IsNullOrWhiteSpace(password))
                {
                    _controller = new MazdaApiController(emailAddress, password, region);
                    _useCachedVehicleList = useCachedVehicleList;
                }
                else
                {
                    throw new MazdaApiConfigException("Invalid or missing password");
                }
            }
            else
            {
                throw new MazdaApiConfigException("Invalid or missing email address");
            }
        }
    }
}
