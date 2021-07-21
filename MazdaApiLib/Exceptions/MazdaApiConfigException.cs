namespace WingandPrayer.MazdaApi.Exceptions
{
    /// <summary>
    /// Raised when Mazda API client is configured incorrectly
    /// </summary>
    public class MazdaApiConfigException : MazdaApiException
    {
        public MazdaApiConfigException(string strStatus) : base(strStatus) { }
    }
}