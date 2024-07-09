using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using RecipeCatalog.Domain.Entities;

namespace RecipeCatalog.Application.Authorization;

public class CuisineAuthorizationHandler(UserManager<User> userManager)
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
            context.Fail();
            return;
        }

        if (requirement.Name == Operations.Create.Name)
        {
            context.Succeed(requirement);
            return;
        }

        var user = await userManager.GetUserAsync(context.User);

        if (user != null)
        {
            var isAdmin = await userManager.IsInRoleAsync(user, "Administrator");

            if (isAdmin)
            {
                context.Succeed(requirement);
                return;
            }
        }

        context.Fail();
    }
}
