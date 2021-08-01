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
            IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            IConfigurationProvider secretProvider = config.Providers.First();

            if (secretProvider.TryGet("TestCredentials:Email", out string email) && secretProvider.TryGet("TestCredentials:Secret", out string secret) & secretProvider.TryGet("TestCredentials:Region", out string region))
            {
                MazdaApiClient client = new(email, secret, region);

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (VehicleModel i in client.GetVehicles())
                {
                    DumpObj(i);
                    Console.WriteLine();
                    if (i.Id != null)
                    {
                        client.GetAvailableService(i.Id);

                        VehicleStatus vs = client.GetVehicleStatus(i.Id);
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
