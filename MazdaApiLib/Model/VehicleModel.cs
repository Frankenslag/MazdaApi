// //
// // VehicleModel.cs
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

namespace WingandPrayer.MazdaApi.Model
{
    public class VehicleModel
    {
        public string Vin { get; set; }
        public string Id { get; set; }
        public string Nickname { get; set; }
        public string CarlineCode { get; set; }
        public string CarlineName { get; set; }
        public string ModelYear { get; set; }
        public string ModelCode { get; set; }
        public string ModelName { get; set; }
        public bool AutomaticTransmission { get; set; }
        public string InteriorColorCode { get; set; }
        public string InteriorColorName { get; set; }
        public string ExteriorColorCode { get; set; }
        public string ExteriorColorName { get; set; }
    }
}
