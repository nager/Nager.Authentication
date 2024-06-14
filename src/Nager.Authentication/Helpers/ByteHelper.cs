using System.Security.Cryptography;

namespace Nager.Authentication.Helpers
{
    /// <summary>
    /// Byte Helper
    /// </summary>
    public static class ByteHelper
    {
        /// <summary>
        /// Generate a secure Pseudo-Random Number Generator
        /// </summary>
        /// <returns></returns>
        public static byte[] CreatePseudoRandomNumber(int bit = 128)
        {
            var temp = new byte[bit / 8];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(temp);

            return temp;
        }
    }
}
