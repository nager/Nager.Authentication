namespace Nager.Authentication.Abstraction.Models
{
    public class MfaInformation
    {
        public bool IsActive { get; set; }
        public string ActivationQrCode { get; set; }
    }
}
