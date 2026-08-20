using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AljazeeraCPanel.Validators
{
    /// <summary>
    /// Enforces password policy validation to prevent weak/default credentials.
    /// Complies with WAPT02-02 requirement to block known weak passwords at login.
    /// </summary>
    public class PasswordPolicyValidator
    {
        /// <summary>
        /// List of known weak/default credentials that should be blocked.
        /// </summary>
        private static readonly HashSet<string> WeakPasswords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // Common default credentials
            "password",
            "password123",
            "admin",
            "admin123",
            "12345678",
            "123456789",
            "1234567890",
            "pass",
            "pass123",
            "password1",
            "password12",
            "qwerty",
            "qwerty123",
            "abc123",
            "letmein",
            "welcome",
            "welcome123",
            "sunshine",
            "prince",
            "monkey",
            "dragon",
            "master",
            "master123",
            "superman",
            "batman",
            "iloveyou",
            "123123",
            "111111",
            "000000",
            "root",
            "toor",
            "test",
            "test123",
            "guest",
            "oracle",
            "oracle123",
            "database",
            "db",
            "sql",
            "admin999",
            "system",
            "system123",
            "cisco",
            "cisco123",
            "default",
            "login",
            "user",
            "pass123456",
            "summer2020",
            "manager",
            "manager123",
            "administrator",
            "1q2w3e4r",
            "1q2w3e4r5t",
            "password@123",
            "admin@123"
        };

        /// <summary>
        /// Validates a password against the policy.
        /// Returns a tuple: (isValid, errorMessage).
        /// </summary>
        public static (bool isValid, string errorMessage) ValidatePassword(string password)
        {
            // Null or empty check
            if (string.IsNullOrWhiteSpace(password))
            {
                return (false, "Password is empty or null.");
            }

            // Check against weak/default password list
            if (IsWeakPassword(password))
            {
                return (false, "This password is not allowed. Please use a stronger password that is not commonly used.");
            }

            // Check minimum length (8 characters recommended for banking)
            if (password.Length < 8)
            {
                return (false, "Password must be at least 8 characters long.");
            }

            // Check for at least one uppercase letter
            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                return (false, "Password must contain at least one uppercase letter.");
            }

            // Check for at least one lowercase letter
            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                return (false, "Password must contain at least one lowercase letter.");
            }

            // Check for at least one digit
            if (!Regex.IsMatch(password, @"[0-9]"))
            {
                return (false, "Password must contain at least one digit.");
            }

            // Check for at least one special character
            if (!Regex.IsMatch(password, @"[!@#$%^&*()_+=\-\[\]{};':"",.<>?/\\|`~]"))
            {
                return (false, "Password must contain at least one special character (!@#$%^&* etc.).");
            }

            // Check for sequential characters (e.g., "abc", "123")
            if (ContainsSequentialCharacters(password))
            {
                return (false, "Password contains sequential characters. Please use a different password.");
            }

            // Check for repeated characters (e.g., "aaaa", "1111")
            if (ContainsRepeatedCharacters(password))
            {
                return (false, "Password contains too many repeated characters. Please use a different password.");
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Checks if a password is in the weak password blacklist.
        /// </summary>
        private static bool IsWeakPassword(string password)
        {
            return WeakPasswords.Contains(password);
        }

        /// <summary>
        /// Detects sequential characters (abc, 123, etc.)
        /// </summary>
        private static bool ContainsSequentialCharacters(string password)
        {
            for (int i = 0; i < password.Length - 2; i++)
            {
                char c1 = password[i];
                char c2 = password[i + 1];
                char c3 = password[i + 2];

                // Check if characters are sequential (ascending or descending)
                if ((c2 == c1 + 1 && c3 == c2 + 1) || (c2 == c1 - 1 && c3 == c2 - 1))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Detects repeated characters (3+ in a row)
        /// </summary>
        private static bool ContainsRepeatedCharacters(string password)
        {
            for (int i = 0; i < password.Length - 2; i++)
            {
                if (password[i] == password[i + 1] && password[i + 1] == password[i + 2])
                {
                    return true;
                }
            }
            return false;
        }
    }
}
