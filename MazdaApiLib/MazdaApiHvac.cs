// 
// MazdaApiHvac.cs
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
using WingandPrayer.MazdaApi.Model;

// ReSharper disable UnusedMember.Global

namespace WingandPrayer.MazdaApi
{
    public partial class MazdaApiClient
    {
        /// <summary>
        /// Turn on the Hvac for a given vehicle.
        /// </summary>
        /// <param name="internalVin">The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles</param>
        public void TurnOnHvac(string internalVin) => TurnOnHvacAsync(internalVin).Wait();

        /// <summary>
        /// Turn off the Hvac for a given vehicle.
        /// </summary>
        /// <param name="internalVin">The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles</param>
        public void TurnOffHvac(string internalVin) => TurnOffHvacAsync(internalVin).Wait();

        /// <summary>
        /// Turn on the Hvac for a given vehicle asynchronously.
        /// </summary>
        /// <param name="internalVin">The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles</param>
        public Task TurnOnHvacAsync(string internalVin) => _controller.SetHvacAsync(internalVin, true);

        /// <summary>
        /// Turn off the Hvac for a given vehicle asynchronously.
        /// </summary>
        /// <param name="internalVin">The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles</param>
        public Task TurnOffHvacAsync(string internalVin) => _controller.SetHvacAsync(internalVin, false);

        /// <summary>
        /// Update the HVAC settings for a given vehicle.
        /// </summary>
        /// <param name="internalVin">The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles</param>
        /// <param name="hvacSettings">An HvacSetting object containing the values to be updated</param>
        public void SetHvacSettings(string internalVin, HvacSettings hvacSettings) => SetHvacSettingsAsync(internalVin, hvacSettings).Wait();

        /// <summary>
        /// Update the HVAC settings for a given vehicle asynchronously.
        /// </summary>
        /// <param name="internalVin">The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles</param>
        /// <param name="hvacSettings">An HvacSetting object containing the values to be updated</param>
        public Task SetHvacSettingsAsync(string internalVin, HvacSettings hvacSettings) => _controller.SetHvacSettingsAsync(internalVin, hvacSettings);

        /// <summary>
        /// Get the HVAC settings for a given vehicle asynchronously.
        /// </summary>
        /// <param name="internalVin">The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles</param>
        /// <returns>An HvacSettings object containing the HVAC settings</returns>
        public HvacSettings GetHvacSettings(string internalVin) => GetHvacSettingsAsync(internalVin).GetAwaiter().GetResult();

        /// <summary>
        /// Get the HVAC settings for a given vehicle asynchronously.
        /// </summary>
        /// <param name="internalVin">The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles</param>
        /// <returns>An HvacSettings object containing the HVAC settings</returns>
        public Task<HvacSettings> GetHvacSettingsAsync(string internalVin) => _controller.GetHvacSettingsAsync(internalVin);
    }
}