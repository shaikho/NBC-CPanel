using System;
using System.Security.Cryptography;
using System.Text;

namespace AljazeeraCPanel.Security
{
    /// <summary>
    /// WAPT11 / WAPT02 — one-way password hashing (A02:2025 Cryptographic Failures).
    ///
    /// Replaces the previous reversible AES storage (hardcoded key + static IV) with
    /// salted PBKDF2-SHA256. Stored format:
    ///
    ///     PBKDF2$&lt;iterations&gt;$&lt;base64(salt)&gt;$&lt;base64(hash)&gt;
    ///
    /// Passwords can no longer be recovered from the database, a backup, or SQLi.
    ///
    /// Transparent migration: <see cref="Verify"/> also accepts a legacy AES-encrypted
    /// value; when one verifies successfully it sets <c>needsUpgrade = true</c> so the
    /// caller can re-store the password as a PBKDF2 hash. Legacy verification exists
    /// ONLY to carry existing users across the transition and should be removed once
    /// all stored values begin with the "PBKDF2$" prefix.
    /// </summary>
    public static class PasswordHasher
    {
        private const string Prefix = "PBKDF2";
        private const int Iterations = 100000;
        private const int SaltSize = 16;   // 128-bit salt
        private const int HashSize = 32;   // 256-bit derived key

        // Legacy AES parameters — must match the retired CryptLib usage so existing
        // stored values can be verified once during the user's next login.
        private const string LegacyKey = "b16920894899c7780b5fc7161560a412";
        private const string LegacyIv = "e77886746a9b416d";

        /// <summary>Produce a new salted PBKDF2 hash for storage.</summary>
        public static string Hash(string password)
        {
            if (password == null) password = string.Empty;

            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            byte[] hash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
                hash = pbkdf2.GetBytes(HashSize);

            return string.Format("{0}${1}${2}${3}",
                Prefix, Iterations,
                Convert.ToBase64String(salt),
                Convert.ToBase64String(hash));
        }

        /// <summary>True if the stored value is already a PBKDF2 hash produced here.</summary>
        public static bool IsHashed(string stored)
        {
            return !string.IsNullOrEmpty(stored) &&
                   stored.StartsWith(Prefix + "$", StringComparison.Ordinal);
        }

        /// <summary>
        /// Verify a password against a stored value. Handles both the new PBKDF2 format
        /// and (during migration) the legacy AES format. On a successful legacy match,
        /// <paramref name="needsUpgrade"/> is set true so the caller can re-hash.
        /// </summary>
        public static bool Verify(string password, string stored, out bool needsUpgrade)
        {
            needsUpgrade = false;
            if (string.IsNullOrEmpty(stored)) return false;
            if (password == null) password = string.Empty;

            if (IsHashed(stored))
                return VerifyPbkdf2(password, stored);

            bool ok = VerifyLegacy(password, stored);
            if (ok) needsUpgrade = true;
            return ok;
        }

        private static bool VerifyPbkdf2(string password, string stored)
        {
            string[] parts = stored.Split('$');
            if (parts.Length != 4) return false;

            int iters;
            if (!int.TryParse(parts[1], out iters) || iters <= 0) return false;

            byte[] salt, expected;
            try
            {
                salt = Convert.FromBase64String(parts[2]);
                expected = Convert.FromBase64String(parts[3]);
            }
            catch { return false; }

            byte[] actual;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iters, HashAlgorithmName.SHA256))
                actual = pbkdf2.GetBytes(expected.Length);

            return FixedTimeEquals(actual, expected);
        }

        private static bool VerifyLegacy(string password, string stored)
        {
            try
            {
                var crypt = new AljazeeraCPanel.CryptLib();
                string plain = crypt.decrypt(stored, LegacyKey, LegacyIv);
                if (plain == null) return false;
                return FixedTimeEquals(
                    Encoding.UTF8.GetBytes(plain),
                    Encoding.UTF8.GetBytes(password));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Length-then-content comparison in constant time for equal-length inputs.</summary>
        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null) return false;
            if (a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}
