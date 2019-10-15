using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;

namespace consoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            HashPassword();
            VerifyHashedPassword();
        }

        static void HashPassword()
        {
            Console.Write("Enter a password: ");
            string password = Console.ReadLine();

            // generate a 128-bit salt using a secure PRNG
            var salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");

            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            Console.WriteLine($"Hashed: {hashed}");
        }

        static string GetBase64Hash(string base64Salt, string password)
        {
            var salt = Convert.FromBase64String(base64Salt);
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            Console.WriteLine($"Hashed: {hashed}");
            return hashed;


        }

        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {

            if (a == null && b == null)
            {
                return true;
            }

            if (a == null || b == null || a.Length != b.Length)

            {

                return false;

            }

            var areSame = true;

            for (var i = 0; i < a.Length; i++)

            {

                areSame &= (a[i] == b[i]);

            }

            return areSame;

        }

        static void VerifyHashedPassword()
        {
            string base64Salt = "m/dRzpc4r0s/gKGua54Qkw==";
            string password = "p@ssw0rd";
            string base64PwdHash = "G4iNmkagZ/1P6A5RRiW7fkc70CEw3fwCH3WrMG9+V9I=";
            var computePwdHash = GetBase64Hash(base64Salt, password);
            Console.WriteLine($"save Hash: {base64PwdHash}, base64PwdHash:{base64PwdHash}, {computePwdHash.Equals(base64PwdHash, StringComparison.OrdinalIgnoreCase)}");
        }
    }
}
