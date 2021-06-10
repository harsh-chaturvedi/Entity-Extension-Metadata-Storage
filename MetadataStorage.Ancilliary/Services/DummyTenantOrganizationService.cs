using MetadataStorage.Ancilliary.Contracts;

namespace MetadataStorage.Ancilliary.Services
{
    public class TenantOrganizationService : ITenantOrganizationService
    {
        public int IdentifyTenant(string tenantDomain)
        {
            if (string.IsNullOrEmpty(tenantDomain))
                return 0;
            if (int.TryParse(tenantDomain, out int value))
            {
                return value;
            }
            else
            {
                //implement application specific logic to identify tenant from its domain
                return 0;
            }
        }
    }
}