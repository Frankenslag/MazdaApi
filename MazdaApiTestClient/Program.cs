﻿using System;
using System.Linq;
using System.Xml.Xsl;
using Microsoft.Extensions.Configuration;
using WingandPrayer.MazdaApi;
using WingandPrayer.MazdaApi.SensorData;

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

                MazdaApiVehicles v = client.GetVehicles().Result;

                foreach (var i in v.VecBaseInfos)
                {
                    Console.WriteLine(i.Vehicle.Id);
                }
            }

            Console.ReadKey();
        }
    }
}
