using System;
using System.Linq;
using System.Security.Cryptography;

namespace GetAnswer.Helpers.Cryptography
{
    public static class PasswordHashHelper
    {
        private const int SALT_LENGTH = 16;
        private const int HASH_LENGTH = 20;
        private const int ITERATIONS_COUNT = 10_000;


        public static string GetSaltedHash(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Value cannot be null or empty.", nameof(password));

            byte[] salt = GetRandomSalt();
            byte[] hash = GetHash(password, salt);
            byte[] saltedHash = CombineSaltAndHash(salt, hash);

            return Convert.ToBase64String(saltedHash);
        }

        private static byte[] GetRandomSalt()
        {
            var salt = new byte[SALT_LENGTH];
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                rngCryptoServiceProvider.GetBytes(salt);
            }

            return salt;
        }

        private static byte[] GetHash(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, ITERATIONS_COUNT))
            {
                return pbkdf2.GetBytes(HASH_LENGTH);
            }
        }

        private static byte[] CombineSaltAndHash(byte[] salt, byte[] hash)
        {
            var saltedHash = new byte[SALT_LENGTH + HASH_LENGTH];
            Array.Copy(salt, 0, saltedHash, 0, SALT_LENGTH);
            Array.Copy(hash, 0, saltedHash, SALT_LENGTH, HASH_LENGTH);

            return saltedHash;
        }

        public static bool CheckPasswordHash(string password, string saltedHash)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Value cannot be null or empty.", nameof(password));
            if (string.IsNullOrEmpty(saltedHash))
                throw new ArgumentException("Value cannot be null or empty.", nameof(saltedHash));

            (byte[] salt, byte[] validHash) = GetSaltedHashParts(saltedHash);
            
            byte[] actualHash = GetHash(password, salt);

            return actualHash.SequenceEqual(validHash);
        }

        private static (byte[] salt, byte[] hash) GetSaltedHashParts(string saltedHash)
        {
            byte[] saltedHashBytes = Convert.FromBase64String(saltedHash);

            if (saltedHashBytes.Length != (SALT_LENGTH + HASH_LENGTH))
                throw new ArgumentException($"{nameof(saltedHash)} is invalid.");
            
            byte[] salt = new byte[SALT_LENGTH];
            Array.Copy(saltedHashBytes, 0, salt, 0, SALT_LENGTH);
            byte[] hash = new byte[HASH_LENGTH];
            Array.Copy(saltedHashBytes, SALT_LENGTH, hash, 0, HASH_LENGTH);

            return (salt, hash);
        }
    }
}