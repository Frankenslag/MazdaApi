using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using WingandPrayer.MazdaApi;
using WingandPrayer.MazdaApi.Model;
using WingandPrayer.MazdaApi.RawModel;

namespace Wingandprayer.MazdaApi
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            IConfigurationProvider secretProvider = config.Providers.First();

            if (secretProvider.TryGet("TestCredentials:Email", out string email) && secretProvider.TryGet("TestCredentials:Secret", out string secret) & secretProvider.TryGet("TestCredentials:Region", out string region))
            {
                MazdaApiClient client = new(email, secret, region);

                foreach (VehicleModel i in client.GetVehicles().Result)
                {
                    var vs = client.GetVehicleStatus(i.Id).Result;
                }
            }

            Console.ReadKey();
        }
    }
}
