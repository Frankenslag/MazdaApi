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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WingandPrayer.MazdaApi.Exceptions;
using WingandPrayer.MazdaApi.Model;

// ReSharper disable UnusedMember.Global

namespace WingandPrayer.MazdaApi
{
    public partial class MazdaApiClient
    {
        private readonly MazdaApiController _controller;

        /// <summary>
        /// Constructs an instance of the MazdaApiClient that is used to access all the public methods.
        /// </summary>
        /// <param name="emailAddress">The email address you use to log into the MyMazda mobile app</param>
        /// <param name="password">The password you use to log into the MyMazda mobile app</param>
        /// <param name="region">The code for the region in which your account was registered (MNAO - North America, MME - Europe, MJO - Japan)</param>
        /// <param name="useCachedVehicleList">A flag that when set to true caches calls to methods that return vehicles. (Optional, defaults to false)</param>
        /// <param name="logger">An ILogger that can be used for debugging and tracing purposes. (Optional, defaults to null)</param>
        public MazdaApiClient(string emailAddress, string password, string region, bool useCachedVehicleList = false, ILogger<MazdaApiClient> logger = null)
        {
            if (!string.IsNullOrWhiteSpace(emailAddress))
            {
                if (!string.IsNullOrWhiteSpace(password))
                {
                    _controller = new MazdaApiController(emailAddress, password, region, logger);
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

        /// <summary>
        /// Get the remote permissions for a given vehicle.
        /// </summary>
        /// <param name="vin">The vehicle identity number fo the vehicle</param>
        /// <returns>A RemoteControlPermissions object containing the remote permissions</returns>
        public RemoteControlPermissions GetRemotePermissions(string vin) => GetRemotePermissionsAsync(vin).GetAwaiter().GetResult();

        /// <summary>
        /// Get the remote permissions for a given vehicle asynchronously.
        /// </summary>
        /// <param name="vin">The vehicle identity number fo the vehicle</param>
        /// <returns>A RemoteControlPermissions object containing the remote permissions</returns>
        public async Task<RemoteControlPermissions> GetRemotePermissionsAsync(string vin) => JsonConvert.DeserializeObject<RemoteControlPermissions>(await _controller.GetRemotePermissionsAsync(vin));

        /// <summary>
        /// Get the available services for a given vehicle.
        /// </summary>
        /// <param name="internalVin">The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles</param>
        /// <returns>An AvailableService object containing the available services</returns>
        public AvailableService GetAvailableService(string internalVin) => GetAvailableServiceAsync(internalVin).GetAwaiter().GetResult();

        /// <summary>
        /// Get the available services for a given vehicle asynchronously.
        /// </summary>
        /// <param name="internalVin">The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles</param>
        /// <returns>An AvailableService object containing the available services</returns>
        public async Task<AvailableService> GetAvailableServiceAsync(string internalVin) => JsonConvert.DeserializeObject<AvailableService>(await _controller.GetAvailableServiceAsync(internalVin));
    }
}