using Auth.RolesToPermission;
using Data.EF;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.API
{
    public class AddPermissionsClaim : IClaimsTransformation
    {
        private readonly DiallogDbContext dbContext;

        public AddPermissionsClaim(DiallogDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (!principal.HasClaim(c => c.Type == PermissionConstants.PackedPermissionClaimType))
            {
                var usersRoles = principal.Claims.Where(x => x.Type == ClaimTypes.Role)
                                .Select(x => x.Value)
                                .ToList();
                var permissionsForUser = await dbContext.RolesToPermissions
                                    .Where(x => usersRoles.Contains(x.RoleName))
                                    .SelectMany(x => x.PermissionsInRole)
                                    .Distinct()
                                    .ToListAsync();

                var id = new ClaimsIdentity();
                id.AddClaim(new Claim(PermissionConstants.PackedPermissionClaimType, permissionsForUser.PackPermissionsIntoString()));
                principal.AddIdentity(id);
            }

            return principal;
        }
    }
}
