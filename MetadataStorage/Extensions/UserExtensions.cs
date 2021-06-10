using System;
using System.Linq;
using System.Security.Claims;
using MetadataStorage.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MetadataStorage.Extensions
{
    public static class UserExtensions
    {
        /// <summary>
        /// Determines whether [is super admin user].
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns><c>true</c> if [is super admin user] [the specified user]; otherwise, <c>false</c>.</returns>
        public static bool IsSuperAdmin(this ClaimsPrincipal user)
        {
            return user.HasClaim(t =>
            t.Type.Equals(Constants.ClaimType.Role, StringComparison.OrdinalIgnoreCase)
            && t.Value.Equals(Constants.ClaimValue.SuperAdmin, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether [is admin user].
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns><c>true</c> if [is admin user] [the specified user]; otherwise, <c>false</c>.</returns>
        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user.HasClaim(t =>
            t.Type.Equals(Constants.ClaimType.Role, StringComparison.OrdinalIgnoreCase)
            && t.Value.Equals(Constants.ClaimValue.Admin, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <param name="modelState">State of the model.</param>
        /// <returns></returns>
        public static string GetErrorMessage(this ModelStateDictionary modelState)
        {
            var errorList = modelState.Values
                .SelectMany(m => m.Errors)
                .Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.GetBaseException().Message : e.ErrorMessage)
                .ToList();

            return string.Join(',', errorList);
        }
    }
}