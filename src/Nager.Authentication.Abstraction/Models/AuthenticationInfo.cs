using System;

namespace Nager.Authentication.Abstraction.Models
{
    internal class AuthenticationInfo
    {
        public DateTime LastValid { get; set; }
        public int InvalidCount { get; set; }
        public DateTime LastInvalid { get; set; }
    }
}
