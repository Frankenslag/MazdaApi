namespace WingandPrayer.MazdaApi.Exceptions
{
    /// <summary>
    /// Raised when server reports that the access token has expired
    /// </summary>
    public class MazdaApiTokenExpiredException : MazdaApiException
    {
        public MazdaApiTokenExpiredException(string strStatus) : base(strStatus) { }
    }
}