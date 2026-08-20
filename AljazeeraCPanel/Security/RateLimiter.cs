using System;
using System.Web;
using System.Web.Caching;

namespace AljazeeraCPanel.Security
{
    /// <summary>
    /// WAPT07 — lightweight in-memory rate limiter (A07:2025 Authentication Failures).
    ///
    /// Backed by the ASP.NET application cache (HttpRuntime.Cache), so it needs no extra
    /// dependency and no database. Counters expire automatically after the window.
    ///
    /// NOTE: this is per-web-server (in-process). For a multi-server/load-balanced farm,
    /// move the counter store to a shared cache (Redis) or the database; the call sites
    /// stay the same. Threshold/window are passed by the caller so login and SMS flows
    /// can use different limits.
    /// </summary>
    public static class RateLimiter
    {
        /// <summary>True if <paramref name="key"/> has reached <paramref name="maxAttempts"/> within the window.</summary>
        public static bool IsBlocked(string key, int maxAttempts)
        {
            var o = HttpRuntime.Cache.Get(CacheKey(key));
            int count = o == null ? 0 : (int)o;
            return count >= maxAttempts;
        }

        /// <summary>Records one attempt against <paramref name="key"/>, (re)setting the expiry window.</summary>
        public static void RegisterAttempt(string key, int windowMinutes)
        {
            string k = CacheKey(key);
            var o = HttpRuntime.Cache.Get(k);
            int count = o == null ? 0 : (int)o;
            count++;
            HttpRuntime.Cache.Insert(
                k, count, null,
                DateTime.UtcNow.AddMinutes(windowMinutes),
                Cache.NoSlidingExpiration);
        }

        /// <summary>Clears the counter for <paramref name="key"/> (call on a successful, legitimate action).</summary>
        public static void Reset(string key)
        {
            HttpRuntime.Cache.Remove(CacheKey(key));
        }

        private static string CacheKey(string key)
        {
            return "wapt07_rl::" + key;
        }
    }
}
