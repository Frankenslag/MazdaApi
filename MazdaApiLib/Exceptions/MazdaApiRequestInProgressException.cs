namespace WingandPrayer.MazdaApi.Exceptions
{
    /// <summary>
    /// Raised when a request fails because another request is already in progress
    /// </summary>
    public class MazdaApiRequestInProgressException : MazdaApiException
    {
        public MazdaApiRequestInProgressException(string strStatus) : base(strStatus) { }
    }
}