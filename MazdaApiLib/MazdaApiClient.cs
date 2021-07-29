// //
// // MazdaApiClient.cs
// //
// // Copyright 2021 Wingandprayer Software
// //
// // This file is part of MazdaApiLib.
// //
// // MazdaApiLib is free software: you can redistribute it and/or modify it under the terms of the
// // GNU General Public License as published by the Free Software Foundation, either version 2
// // of the License, or (at your option) any later version.
// //
// // CDRipper is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// // without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
// // PURPOSE. See the GNU General Public License for more details.
// //
// // You should have received a copy of the GNU General Public License along with MazdaApiLib.
// // If not, see http://www.gnu.org/licenses/.
// //

using WingandPrayer.MazdaApi.Exceptions;

namespace WingandPrayer.MazdaApi
{
    public partial class MazdaApiClient
    {
        private readonly MazdaApiController _controller;

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
