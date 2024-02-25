using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace JonathanPotts.RecipeCatalog.Application.Authorization;

public class RecipeDtoAuthorizationHandler(UserManager<User> userManager)
    : AuthorizationHandler<OperationAuthorizationRequirement, RecipeDto>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, RecipeDto resource)
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

        var userId = userManager.GetUserId(context.User);

        if (resource.OwnerId == userId)
        {
            context.Succeed(requirement);
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
