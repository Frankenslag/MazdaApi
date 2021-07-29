// //
// // MazdaApiEncryptionException.cs
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

namespace WingandPrayer.MazdaApi.Exceptions
{
    /// <summary>
    /// Raised when server reports that the request is not encrypted properly
    /// </summary>
    public class MazdaApiEncryptionException : MazdaApiException
    {
        public MazdaApiEncryptionException(string strStatus) : base(strStatus) { }
    }
}