// 
// Program.cs
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
using System.Collections;
using System.ComponentModel;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WingandPrayer.MazdaApi;
using WingandPrayer.MazdaApi.Model;

// ReSharper disable UnusedVariable
// ReSharper disable UnusedParameter.Local

namespace Wingandprayer.MazdaApi
{
    internal class Program
    {
        private static void DumpObj(object obj, int indent = 0)
        {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
            {
                if (descriptor.PropertyType.GetInterfaces().Contains(typeof(ICollection)))
                {
                    Console.WriteLine($"{new string(' ', indent + 1)}{descriptor.Name} =");
                    // ReSharper disable once PossibleNullReferenceException
                    foreach (object childObj in (IEnumerable) descriptor.GetValue(obj)!)
                    {
                        DumpObj(childObj, indent + 2);
                    }
                }
                else if (descriptor.PropertyType.IsClass && descriptor.PropertyType.Name != "String")
                {
                    Console.WriteLine($"{new string(' ', indent + 1)}{descriptor.Name} =");
                    DumpObj(descriptor.GetValue(obj), indent + 2);
                }
                else
                {
                    Console.WriteLine($"{new string(' ', indent)}{descriptor.Name} = {descriptor.GetValue(obj)}");
                }
            }
        }

        private static void Main(string[] args)
        {
            // create a service host. Done so that logging and user secrets are included as services.
            IHost serviceHost = Host.CreateDefaultBuilder().ConfigureServices((hostContext, services) =>
            {
                services.AddTransient(s =>
                {
                    MazdaApiClient retval = null;
                    
                    ILogger<IHost> logger = s.GetRequiredService<ILogger<IHost>>();

                    IConfigurationRoot configRoot = s.GetRequiredService<IConfiguration>() as IConfigurationRoot;
                    IConfigurationProvider secretProvider = configRoot?.Providers.FirstOrDefault(provider => ((provider as JsonConfigurationProvider)?.Source.Path ?? string.Empty) == @"secrets.json");

                    if (secretProvider != null)
                    {
                        if (secretProvider.TryGet("TestCredentials:Email", out string email) && secretProvider.TryGet("TestCredentials:Secret", out string secret) & secretProvider.TryGet("TestCredentials:Region", out string region))
                        {
                            retval = new MazdaApiClient(email, secret, region, logger: s.GetRequiredService<ILogger<MazdaApiClient>>());
                        }
                        else
                        {
                            logger.LogError("Cannot get email, secret or region from the user secrets file");
                        }
                    }
                    else
                    {
                        logger.LogError("Cannot get the user secrets file");
                    }

                    return retval;
                });
            }).ConfigureAppConfiguration((hostContext, services) =>
            {
                // done to force the loading of user secrets even in release mode. Should be ok for a test harness.
                services.AddUserSecrets<Program>();
            }).Build();

            MazdaApiClient client = serviceHost.Services.GetService<MazdaApiClient>();

            if (client is not null)
            {
                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (VehicleModel vehicle in client.GetVehicles())
                {
                    DumpObj(vehicle);
                    Console.WriteLine();

                    if (vehicle.VinRegistStatus == 3)
                    {
                        client.GetAvailableService(vehicle.Id);

                        VehicleStatus vs = client.GetVehicleStatus(vehicle.Id);
                        DumpObj(vs);

                        Console.WriteLine();
                    }
                }
            }

            Console.WriteLine("Press any key to finish");
            Console.ReadKey();
        }
    }
}
