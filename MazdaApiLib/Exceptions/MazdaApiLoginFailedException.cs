namespace WingandPrayer.MazdaApi.Exceptions
{
    /// <summary>
    /// Raised when login fails for an unknown reason
    /// </summary>
    public class MazdaApiLoginFailedException : MazdaApiException
    {
        public MazdaApiLoginFailedException(string strStatus) : base(strStatus) { }
    }
}