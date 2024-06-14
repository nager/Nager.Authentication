namespace Nager.Authentication.AspNet.Dtos
{
    public class MfaRequiredResponseDto
    {
        public string MfaIdentifier { get; set; }
        public string MfaType { get; set; }
    }
}
