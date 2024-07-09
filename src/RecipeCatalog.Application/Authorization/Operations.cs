using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace RecipeCatalog.Application.Authorization;

public static class Operations
{
    public static readonly OperationAuthorizationRequirement Create = new() { Name = nameof(Create) };
    public static readonly OperationAuthorizationRequirement Read = new() { Name = nameof(Read) };
    public static readonly OperationAuthorizationRequirement Update = new() { Name = nameof(Update) };
    public static readonly OperationAuthorizationRequirement Delete = new() { Name = nameof(Delete) };
}
