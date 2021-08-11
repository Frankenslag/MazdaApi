// 
// MazdaApiHazzards.cs
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

// ReSharper disable UnusedMember.Global

namespace WingandPrayer.MazdaApi
{
    public partial class MazdaApiClient
    {
        /// <summary>
        /// Turns on the vehicle hazard lights.
        /// </summary>
        /// <param name="internalVin">The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles</param>
        public void TurnOnHazzardLights(string internalVin) => TurnOnHazzardLightsAsync(internalVin).Wait();

        /// <summary>
        /// Turns off the vehicle hazard lights.
        /// </summary>
        /// <param name="internalVin">The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles</param>
        public void TurnOffHazzardLights(string internalVin) => TurnOffHazzardLightsAsync(internalVin).Wait();

        /// <summary>
        /// Turns on the vehicle hazard lights asynchronously.
        /// </summary>
        /// <param name="internalVin">The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles</param>
        public Task TurnOnHazzardLightsAsync(string internalVin) => _controller.SetHazzardLightAsync(internalVin, true);

        /// <summary>
        /// Turns off the vehicle hazard lights asynchronously.
        /// </summary>
        /// <param name="internalVin">The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles</param>
        public Task TurnOffHazzardLightsAsync(string internalVin) => _controller.SetHazzardLightAsync(internalVin, false);
    }
}