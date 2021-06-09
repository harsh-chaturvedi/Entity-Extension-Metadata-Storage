using System;

namespace MetadataStorage.Infrastructure.Model
{
    public class SearchInputContext
    {
        public int TenantId { get; set; }

        public Guid? OrganizationId { get; set; }

        public bool IsUserExternalApplicationTenantAdmin { get; set; }

        public bool IsUserExternalApplicationOrganizationAdmin { get; set; }

        public bool IsUserSuperAdmin { get; set; }
    }
}