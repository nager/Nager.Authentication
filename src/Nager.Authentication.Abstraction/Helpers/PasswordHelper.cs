using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;

namespace Nager.Authentication.Abstraction.Helpers
{
    public static class PasswordHelper
    {
        const int iterationCount = 10_000;

        public static byte[] HashPasword(string password, byte[] salt)
        {
            //byte[] salt = new byte[128 / 8];
            //using (var rng = RandomNumberGenerator.Create())
            //{
            //    rng.GetBytes(salt);
            //}
            //Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");



            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            var passwordData = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: iterationCount,
                numBytesRequested: 256 / 8);

            //string hashed = Convert.ToBase64String(passwordData);



            return passwordData;
        }
    }
}
