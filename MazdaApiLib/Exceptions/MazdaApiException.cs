using System;

namespace WingandPrayer.MazdaApi.Exceptions
{
    /// <summary>
    /// Raised when an unknown error occurs during API interaction
    /// </summary>
    public class MazdaApiException : Exception
    {
        public MazdaApiException(string strStatus) : base(strStatus)
        {
        }
    }
}