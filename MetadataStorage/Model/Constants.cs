namespace MetadataStorage.Model
{
    public static class  Constants
    {
        /// <summary>
        /// Claim type for tenantId claim
        /// </summary>
        public const string TenantId = "http://schemas.microsoft.com/identity/claims/tenantid";

        /// <summary>
        /// Claim type for tenant domain claim
        /// </summary>

        public const string UniqueHomeEnvironmentIdentifier = "utid";


        /// <summary>
        /// </summary>
        public static class ClaimType
        {
            /// <summary>
            /// The role
            /// </summary>
            public const string Role = "Application.Identity.Role"; // Prepending Application.Identity to ignore contradiction with ADFS claims.
        }

        /// <summary>
        /// </summary>
        public static class ClaimValue
        {
            /// <summary>
            /// The admin
            /// </summary>
            public const string Admin = "Administrator";

            /// <summary>
            /// The super admin
            /// </summary>
            public const string SuperAdmin = "Super Administrator";
        }
    }
}