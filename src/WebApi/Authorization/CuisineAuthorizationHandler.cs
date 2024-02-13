using JonathanPotts.RecipeCatalog.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace JonathanPotts.RecipeCatalog.WebApi.Authorization;

public class CuisineAuthorizationHandler(UserManager<ApplicationUser> userManager)
    : AuthorizationHandler<OperationAuthorizationRequirement, Cuisine>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, Cuisine resource)
    {
        if (requirement.Name == Operations.Read.Name)
        {
            context.Succeed(requirement);
            return;
        }

        if (!(context.User.Identity?.IsAuthenticated ?? false))
        {
            return;
        }

        if (requirement.Name == Operations.Create.Name)
        {
            context.Succeed(requirement);
            return;
        }

        if (requirement.Name != Operations.Update.Name && requirement.Name != Operations.Delete.Name)
        {
            return;
        }

        var user = await userManager.GetUserAsync(context.User);

        if (user == null)
        {
            return;
        }

        var isAdmin = await userManager.IsInRoleAsync(user, "Administrator");

        if (isAdmin)
        {
            context.Succeed(requirement);
        }
    }
}
