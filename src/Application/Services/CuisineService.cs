using System.Security;
using System.Security.Claims;
using FluentValidation;
using JonathanPotts.RecipeCatalog.Application.Authorization;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.Application.Contracts.Services;
using JonathanPotts.RecipeCatalog.Application.Mapping;
using JonathanPotts.RecipeCatalog.Application.Validation;
using JonathanPotts.RecipeCatalog.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace JonathanPotts.RecipeCatalog.Application.Services;

public class CuisineService(
    RecipeCatalogDbContext context,
    IAuthorizationService authorizationService) : ICuisineService
{
    public async Task<IEnumerable<CuisineDto>> GetListAsync(
        CancellationToken cancellationToken = default)
    {
        var cuisines = await context.Cuisines
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);

        return cuisines.Select(x => x.ToCuisineDto());
    }

    public async Task<CuisineDto?> GetAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var cuisine = await context.Cuisines
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return cuisine?.ToCuisineDto();
    }

    public async Task<CuisineDto> CreateAsync(
        CreateUpdateCuisineDto dto,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        new CreateUpdateCuisineDtoValidator().ValidateAndThrow(dto);

        var cuisine = dto.ToCuisine();

        var authResult = await authorizationService.AuthorizeAsync(
            user,
            cuisine,
            Operations.Create);

        if (!authResult.Succeeded)
        {
            throw new SecurityException(
                $"User is unauthorized to perform {nameof(Operations.Create)} operation on resource.");
        }

        await context.Cuisines.AddAsync(cuisine, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return cuisine.ToCuisineDto();
    }

    public async Task<CuisineDto> UpdateAsync(
        int id,
        CreateUpdateCuisineDto dto,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        new CreateUpdateCuisineDtoValidator().ValidateAndThrow(dto);

        var cuisine = await context.Cuisines.FindAsync([id], cancellationToken)
            ?? throw new KeyNotFoundException();

        var authResult = await authorizationService.AuthorizeAsync(
            user,
            cuisine,
            Operations.Update);

        if (!authResult.Succeeded)
        {
            throw new SecurityException(
                $"User is unauthorized to perform {nameof(Operations.Update)} operation on resource.");
        }

        context.Entry(cuisine).CurrentValues.SetValues(dto);

        await context.SaveChangesAsync(cancellationToken);

        return cuisine.ToCuisineDto();
    }

    public async Task DeleteAsync(
        int id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        var cuisine = await context.Cuisines.FindAsync([id], cancellationToken)
            ?? throw new KeyNotFoundException();

        var authResult = await authorizationService.AuthorizeAsync(
            user,
            cuisine,
            Operations.Delete);

        if (!authResult.Succeeded)
        {
            throw new SecurityException(
                $"User is unauthorized to perform {nameof(Operations.Delete)} operation on resource.");
        }

        context.Cuisines.Remove(cuisine);
        await context.SaveChangesAsync(cancellationToken);
    }
}
