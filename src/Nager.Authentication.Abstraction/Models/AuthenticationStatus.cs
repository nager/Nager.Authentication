namespace Nager.Authentication.Abstraction.Models
{
    public enum AuthenticationStatus
    {
        Invalid,
        Valid,
        MfaCodeRequired,
        TemporaryBlocked
    }
}
