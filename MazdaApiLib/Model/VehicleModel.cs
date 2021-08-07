// 
// VehicleModel.cs
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

namespace WingandPrayer.MazdaApi.Model
{
    public class VehicleModel
    {
        public string Vin { get; set; }
        public string Id { get; set; }
        public int VinRegistStatus { get; set; }
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