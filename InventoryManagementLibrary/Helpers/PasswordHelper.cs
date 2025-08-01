using System.Security.Cryptography;

namespace InventoryManagementLibrary.Helpers
{
    public static class PasswordHelper
    {
        public static void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using (var hmac = new Rfc2898DeriveBytes(password, 16, 10000, HashAlgorithmName.SHA256))
            {
                salt = hmac.Salt;
                hash = hmac.GetBytes(32);
            }
        }

        public static bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new Rfc2898DeriveBytes(password, storedSalt, 10000, HashAlgorithmName.SHA256))
            {
                var computedHash = hmac.GetBytes(32);
                return AreHashesEqual(computedHash, storedHash);
            }
        }

       
        private static bool AreHashesEqual(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            int result = 0;
            for (int i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }
            return result == 0;
        }
    }
}
