using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;

namespace Nager.Authentication.Abstraction.Helpers
{
    public static class PasswordHelper
    {
        private const int IterationCount = 10_000;

        /// <summary>
        /// Generate a 128-bit salt using a secure PRNG
        /// </summary>
        /// <returns></returns>
        public static byte[] CreateSalt()
        {
            var salt = new byte[128 / 8];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);            

            return salt;
        }

        /// <summary>
        /// Derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static byte[] HashPasword(string password, byte[] salt)
        {
            var passwordData = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: IterationCount,
                numBytesRequested: 256 / 8);

            return passwordData;
        }

        /// <summary>
        /// Create a random password
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string CreateRandomPassword(int length)
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var tokenBuffer = new byte[length];
            rngCryptoServiceProvider.GetBytes(tokenBuffer);
            return Convert.ToBase64String(tokenBuffer);
        }
    }
}
