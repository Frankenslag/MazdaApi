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