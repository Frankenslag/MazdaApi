namespace WingandPrayer.MazdaApi.Exceptions
{
    /// <summary>
    /// Raised when email address or password are invalid during authentication
    /// </summary>
    public class MazdaApiAuthenticationException : MazdaApiException
    {
        public MazdaApiAuthenticationException(string strStatus) : base(strStatus) { }
    }
}