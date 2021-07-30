// 
// MazdaApiClient.cs
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

using System.Threading.Tasks;
using Newtonsoft.Json;
using WingandPrayer.MazdaApi.Exceptions;
using WingandPrayer.MazdaApi.Model;

// ReSharper disable UnusedMember.Global

namespace WingandPrayer.MazdaApi
{
    public partial class MazdaApiClient
    {
        private readonly MazdaApiController _controller;

        public AvailableService GetAvailableService(string internalVin) => GetAvailableServiceAsync(internalVin).GetAwaiter().GetResult();

        public async Task<AvailableService> GetAvailableServiceAsync(string internalVin)
        {
            return JsonConvert.DeserializeObject<AvailableService>(await _controller.GetAvailableServiceAsync(internalVin));
        }

        public void TurnOnHazzardLights(string internalVin) => TurnOnHazzardLightsAsync(internalVin).Wait();

        public void TurnOffHazzardLights(string internalVin) => TurnOffHazzardLightsAsync(internalVin).Wait();

        public async Task TurnOnHazzardLightsAsync(string internalVin) => await _controller.SetHazzardLightAsync(internalVin, true);

        public async Task TurnOffHazzardLightsAsync(string internalVin) => await _controller.SetHazzardLightAsync(internalVin, false);

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