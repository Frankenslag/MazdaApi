// 
// MazdaApiLocks.cs
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
using System.Collections.Generic;
using System.Threading.Tasks;

// ReSharper disable UnusedMember.Global

namespace WingandPrayer.MazdaApi
{
    internal class CachedLockState
    {
        public bool? ApiLockState { get; set; }

        public bool? RequestedLockState { get; set; }

        public DateTime ApiLockTimestamp { get; set; } = DateTime.MinValue;

        public DateTime RequestedLockTimestamp { get; set; } = DateTime.MinValue;

        public bool? AssumedLockState
        {
            get
            {
                if (ApiLockState != null || RequestedLockState != null)
                {
                    // only have the requested lock state
                    if (ApiLockState != null && RequestedLockState is null) return ApiLockState;

                    // only have the api lock state
                    if (ApiLockState is null) return RequestedLockState;

                    // we have both lock states available chose the most appropriate
                    return RequestedLockTimestamp > ApiLockTimestamp && (DateTime.UtcNow - RequestedLockTimestamp).TotalSeconds < 600 ? RequestedLockState : ApiLockState;
                }

                return null;
            }
        }
    }

    public partial class MazdaApiClient
    {
        private readonly Dictionary<string, CachedLockState> _lockStates = new Dictionary<string, CachedLockState>();

        private CachedLockState GetCachedLockState(string internalVin)
        {
            if (!_lockStates.ContainsKey(internalVin)) _lockStates.Add(internalVin, new CachedLockState());

            return _lockStates[internalVin];
        }

        /// <summary>
        /// Get the assumed lock state of a vehicle.
        /// </summary>
        /// <remarks>This method derives the lock state from both the actual lock state that is read from the vehicle and the attempts by this to lock and unlock the vehicle by this api
        /// It is required because there is often a delay between locking the vehical and the status changing with the api</remarks>
        /// <param name="internalVin">The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles</param>
        /// <returns>A bool to indicate the assumed lock state or null if the lock state cannot be determined</returns>
        public bool? GetAssumedLockState(string internalVin) => GetCachedLockState(internalVin).AssumedLockState;

        /// <summary>
        /// Locks the doors of the vehicle.
        /// </summary>
        /// <param name="internalVin">The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles</param>
        public void LockDoor(string internalVin) => LockDoorAsync(internalVin).Wait();

        /// <summary>
        /// Unlocks the doors of the vehicle.
        /// </summary>
        /// <param name="internalVin">The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles</param>
        public void UnlockDoor(string internalVin) => UnlockDoorAsync(internalVin).Wait();

        /// <summary>
        /// Locks the doors of the vehicle asynchronously.
        /// </summary>
        /// <param name="internalVin">The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles</param>
        public async Task LockDoorAsync(string internalVin)
        {
            CachedLockState lockState = GetCachedLockState(internalVin);

            lockState.RequestedLockState = true;
            lockState.RequestedLockTimestamp = DateTime.UtcNow;

            await _controller.SetDoorLockAsync(internalVin, true);
        }

        /// <summary>
        /// Unlocks the doors of the vehicle asynchronously.
        /// </summary>
        /// <param name="internalVin">The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles</param>
        public async Task UnlockDoorAsync(string internalVin)
        {
            CachedLockState lockState = GetCachedLockState(internalVin);

            lockState.RequestedLockState = false;
            lockState.RequestedLockTimestamp = DateTime.UtcNow;

            await _controller.SetDoorLockAsync(internalVin, false);
        }
    }
}