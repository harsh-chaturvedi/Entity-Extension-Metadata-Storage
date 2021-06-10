namespace MetadataStorage.Ancilliary.Contracts
{
    public interface ITenantOrganizationService
    {
        int IdentifyTenant(string tenantDomain);
    }
}