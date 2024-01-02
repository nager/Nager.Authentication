using System;

namespace Nager.Authentication.Abstraction.Models
{
    public class AuthenticationInfo
    {
        public DateTime LastValid { get; set; }

        public DateTime LastInvalid { get; set; }

        public int InvalidCount { get; set; }
    }
}
