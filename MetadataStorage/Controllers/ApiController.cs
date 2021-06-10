using System;
using MetadataStorage.Ancilliary.Contracts;
using MetadataStorage.Extensions;
using MetadataStorage.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetadataStorage.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]

    public class ApiController : ControllerBase
    {
        public ApiController(ITenantOrganizationService tenantOrganizationService)
        {
            _tenantService = tenantOrganizationService;
        }
        private int _tenantId;

        private readonly ITenantOrganizationService _tenantService;
        public int LoggedInTenantId
        {
            get
            {
                if (_tenantId == 0)
                {
                    var tenantDomain = User.FindFirst(Constants.UniqueHomeEnvironmentIdentifier);
                    _tenantId = _tenantService.IdentifyTenant(tenantDomain.Value);
                }
                return _tenantId;
            }
        }
        public bool IsUserExternalApplicationTenantAdmin
        {
            get
            {
                // this ensures that the logged in user is tenant admin and not organization admin user
                var value = User.IsAdmin() && LoggedInTenantId > 0;
                return value;
            }
        }

        protected bool CheckEntityAccessToLoggedInTenant(int tenantId, Guid? organizationId = null)
        {
            //if org context is not passed - do not do org filtering
            if (organizationId == null || organizationId.Value == Guid.Empty)
                return (User.IsSuperAdmin() || (IsUserExternalApplicationTenantAdmin && tenantId == LoggedInTenantId));

            else
                return (User.IsSuperAdmin() ||
                       (IsUserExternalApplicationTenantAdmin && tenantId == LoggedInTenantId));
            // || (IsUserExternalApplicationOrganizationAdmin && tenantId == LoggedInUserTenant.Id && organizationId == LoggedInUser.TenantOrganizationId));
        }
    }
}