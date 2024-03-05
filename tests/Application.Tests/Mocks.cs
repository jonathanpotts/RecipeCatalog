using System.Security.Claims;
using IdGen;
using JonathanPotts.RecipeCatalog.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace JonathanPotts.RecipeCatalog.Application.Tests;

public static class Mocks
{
    public const long GeneratedId = 23430727760609280;

    public static Mock<UserManager<User>> CreateUserManagerMock()
    {
        Mock<UserManager<User>> userManagerMock = new(
            Mock.Of<IUserStore<User>>(),
            Mock.Of<IOptions<IdentityOptions>>(),
            Mock.Of<IPasswordHasher<User>>(),
            Array.Empty<IUserValidator<User>>(),
            Array.Empty<IPasswordValidator<User>>(),
            Mock.Of<ILookupNormalizer>(),
            Mock.Of<IdentityErrorDescriber>(),
            Mock.Of<IServiceProvider>(),
            Mock.Of<ILogger<UserManager<User>>>());

        var adminRoleId = TestData.Roles.First(x => x.NormalizedName == "ADMINISTRATOR").Id;
        var isAdmin = (string userId) => TestData.UserRoles.Any(x => x.UserId == userId && x.RoleId == adminRoleId);
        var isAdminRole = (string role) => role.Equals("ADMINISTRATOR", StringComparison.OrdinalIgnoreCase);

        userManagerMock
            .Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>()))
            .CallBase();
        userManagerMock
            .Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()).Result)
            .Returns((ClaimsPrincipal x) => new User { Id = x.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty });
        userManagerMock
            .Setup(x => x.IsInRoleAsync(It.Is<User>(x => isAdmin(x.Id)), It.Is<string>(x => isAdminRole(x))).Result)
            .Returns(true);
        userManagerMock
            .Setup(x => x.IsInRoleAsync(It.Is<User>(x => !isAdmin(x.Id)), It.Is<string>(x => isAdminRole(x))).Result)
            .Returns(false);

        return userManagerMock;
    }

    public static Mock<IIdGenerator<long>> CreateIdGeneratorMock()
    {
        Mock<IIdGenerator<long>> idGeneratorMock = new();

        idGeneratorMock.Setup(x => x.CreateId()).Returns(GeneratedId);

        return idGeneratorMock;
    }
}
