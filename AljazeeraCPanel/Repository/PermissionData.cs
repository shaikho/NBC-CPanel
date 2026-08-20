using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;

namespace AljazeeraCPanel.Repository
{
    /// <summary>
    /// WAPT03-02: Server-side authorization data source.
    ///
    /// Loads the set of "Controller/Action" URLs a given role is permitted to reach,
    /// straight from the same tables that build the navigation menu
    /// (JSB_MENU_MASTER + JSB_ROLE_MENU_MAPPING). This is the authoritative
    /// permission model already used by usp_GetMenuData2 for the sidebar; enforcing
    /// against it on every request closes the "access control enforced only at the
    /// UI level" finding (WAPT03 / A01:2025 Broken Access Control).
    ///
    /// Role IDs are NOT hardcoded: roles are created at runtime from the CPanel
    /// Profiles Management screen (max(role_id)+1), so authorization must be data-
    /// driven off the mapping table rather than compiled-in role numbers.
    /// </summary>
    public static class PermissionData
    {
        /// <summary>
        /// Returns the normalized set of "controller/action" strings (lower-case)
        /// that the given role is allowed to access. Menu rows whose URL is "#"
        /// (parent/section headers) are ignored.
        /// </summary>
        public static HashSet<string> GetPermittedUrls(string roleId)
        {
            var permitted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(roleId))
                return permitted;

            string conString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (OracleConnection con = new OracleConnection(conString))
            {
                // Parameterized (WAPT01): role id is bound, never concatenated.
                const string query =
                    "select m.menu_url " +
                    "from jsb_role_menu_mapping map " +
                    "join jsb_menu_master m on m.menu_id = map.mapping_menu_id " +
                    "where map.mapping_role_id = :roleid " +
                    "and map.mapping_status = 'A' " +
                    "and m.menu_status = 'A' " +
                    "and m.menu_url is not null " +
                    "and m.menu_url <> '#'";

                using (OracleCommand cmd = new OracleCommand(query, con))
                {
                    cmd.Parameters.Add(":roleid", OracleType.VarChar).Value = roleId;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            string url = sdr["menu_url"] == null ? null : sdr["menu_url"].ToString();
                            string norm = NormalizeUrl(url);
                            if (norm != null)
                                permitted.Add(norm);
                        }
                    }
                }
            }

            return permitted;
        }

        /// <summary>
        /// Normalizes a stored menu URL or a routed controller/action pair into a
        /// consistent "controller/action" key for comparison. Trims a leading slash,
        /// drops query strings, and lower-cases.
        /// </summary>
        public static string NormalizeUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            string u = url.Trim();

            // Drop any query string / fragment
            int q = u.IndexOfAny(new[] { '?', '#' });
            if (q >= 0)
                u = u.Substring(0, q);

            u = u.Trim('/');

            if (u.Length == 0)
                return null;

            return u.ToLowerInvariant();
        }

        /// <summary>
        /// Builds the "controller/action" key from route values.
        /// </summary>
        public static string BuildKey(string controller, string action)
        {
            return NormalizeUrl((controller ?? string.Empty) + "/" + (action ?? string.Empty));
        }

        /// <summary>
        /// Extracts the controller portion (lower-case) from a normalized
        /// "controller/action" key, or null if it cannot be determined.
        /// </summary>
        public static string ControllerOf(string normalizedUrl)
        {
            if (string.IsNullOrEmpty(normalizedUrl))
                return null;
            int slash = normalizedUrl.IndexOf('/');
            string c = slash > 0 ? normalizedUrl.Substring(0, slash) : normalizedUrl;
            return c.Length == 0 ? null : c;
        }

        /// <summary>
        /// Returns the distinct set of controllers a role may reach, derived from
        /// that role's permitted menu URLs. Sub-actions of a permitted controller
        /// (e.g. User/Reject reached from User/Users) are allowed by controller
        /// membership; the menu only maps landing actions, not every action.
        /// </summary>
        public static HashSet<string> GetPermittedControllers(string roleId)
        {
            var controllers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var url in GetPermittedUrls(roleId))
            {
                string c = ControllerOf(url);
                if (c != null)
                    controllers.Add(c);
            }
            return controllers;
        }

        /// <summary>
        /// Returns every controller that appears in the menu model at all (any role).
        /// These are the "role-scoped" controllers the authorization filter guards.
        /// A controller NOT in this set is a supporting page (dashboard, own-profile,
        /// config sub-page) that is governed by session auth only, not by menu scope.
        /// Cached at application level and refreshed lazily.
        /// </summary>
        public static HashSet<string> GetAllMenuControllers()
        {
            lock (_menuControllersLock)
            {
                if (_menuControllersCache != null &&
                    (DateTime.UtcNow - _menuControllersLoadedUtc) < _menuControllersTtl)
                {
                    return _menuControllersCache;
                }

                var controllers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                string conString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

                using (OracleConnection con = new OracleConnection(conString))
                {
                    const string query =
                        "select menu_url from jsb_menu_master " +
                        "where menu_status = 'A' and menu_url is not null and menu_url <> '#'";
                    using (OracleCommand cmd = new OracleCommand(query, con))
                    {
                        con.Open();
                        using (OracleDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                string c = ControllerOf(NormalizeUrl(sdr[0] == null ? null : sdr[0].ToString()));
                                if (c != null)
                                    controllers.Add(c);
                            }
                        }
                    }
                }

                _menuControllersCache = controllers;
                _menuControllersLoadedUtc = DateTime.UtcNow;
                return controllers;
            }
        }

        private static readonly object _menuControllersLock = new object();
        private static HashSet<string> _menuControllersCache;
        private static DateTime _menuControllersLoadedUtc = DateTime.MinValue;
        private static readonly TimeSpan _menuControllersTtl = TimeSpan.FromMinutes(15);
    }
}
