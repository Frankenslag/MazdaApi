// 
// Program.cs
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
using System.ComponentModel;
using System.Linq;
using Microsoft.Extensions.Configuration;

using WingandPrayer.MazdaApi;
using WingandPrayer.MazdaApi.Model;

// ReSharper disable UnusedVariable
// ReSharper disable UnusedParameter.Local

namespace Wingandprayer.MazdaApi
{
    internal class Program
    {
        private static void DumpObj(object obj)
        {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
            {
                Console.WriteLine($"{descriptor.Name} = {descriptor.GetValue(obj)}");
            }
        }

        private static void Main(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            IConfigurationProvider secretProvider = config.Providers.First();

            if (secretProvider.TryGet("TestCredentials:Email", out string email) && secretProvider.TryGet("TestCredentials:Secret", out string secret) & secretProvider.TryGet("TestCredentials:Region", out string region))
            {
                MazdaApiClient client = new(email, secret, region);

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (VehicleModel i in client.GetVehicles())
                {
                    DumpObj(i);
                    VehicleStatus vs = client.GetVehicleStatus(i.Id);
                    DumpObj(vs);
                }
            }

            Console.WriteLine("Press any key to finish");
            Console.ReadKey();
        }
    }
}
