namespace WingandPrayer.MazdaApi.Exceptions
{
    /// <summary>
    /// Raised when account is locked from too many login attempts
    /// </summary>
    public class MazdaApiAccountLockedException : MazdaApiException
    {
        public MazdaApiAccountLockedException(string strStatus) : base(strStatus) { }
    }
}