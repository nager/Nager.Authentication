using System;
using System.Linq;

namespace Nager.Authentication.Helpers
{
    public static class RoleHelper
    {
        public static char RoleSeperator = ',';

        public static string[] GetRoles(string? roleData)
        {
            if (string.IsNullOrEmpty(roleData))
            {
                return Array.Empty<string>();
            }

            var roles = roleData.Split(RoleSeperator, StringSplitOptions.RemoveEmptyEntries);
            if (roles.Length == 0)
            {
                return Array.Empty<string>();
            }

            return roles;
        }

        public static string GetRolesData(string[] roles)
        {
            if (roles == null)
            {
                return string.Empty;
            }

            if (roles.Length == 0)
            {
                return string.Empty;
            }

            return string.Join(RoleSeperator, roles);
        }

        public static string RemoveRoleFromRoleData(string? roleData, string roleName)
        {
            var roles = GetRoles(roleData);
            if (roles.Length == 0)
            {
                return string.Empty;
            }

            var tempRoles = roles.Where(o => !o.Equals(roleName, StringComparison.OrdinalIgnoreCase));
            return string.Join(RoleSeperator, tempRoles);
        }

        public static string AddRoleToRoleData(string? roleData, string roleName)
        {
            var roles = GetRoles(roleData);
            var newRoleName = roleName.Trim(RoleSeperator).Trim();

            if (roles.Contains(newRoleName, StringComparer.OrdinalIgnoreCase))
            {
                return roleData;
            }

            var tempRoles = roles.Append(newRoleName);
            return string.Join(RoleSeperator, tempRoles);
        }
    }
}
