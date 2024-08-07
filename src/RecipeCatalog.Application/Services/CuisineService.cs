﻿using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RecipeCatalog.Application.Authorization;
using RecipeCatalog.Application.Contracts.Models;
using RecipeCatalog.Application.Contracts.Services;
using RecipeCatalog.Application.Mapping;
using RecipeCatalog.Application.Validation;
using RecipeCatalog.Domain;

namespace RecipeCatalog.Application.Services;

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
            throw new UnauthorizedAccessException(
                $"User is unauthorized to perform {nameof(Operations.Create)} operation on resource.");
        }

        await context.Cuisines.AddAsync(cuisine, cancellationToken);

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            context.Cuisines.Remove(cuisine);
            throw;
        }

        return cuisine.ToCuisineDto();
    }

    public async Task<CuisineDto?> UpdateAsync(
        int id,
        CreateUpdateCuisineDto dto,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        new CreateUpdateCuisineDtoValidator().ValidateAndThrow(dto);

        var cuisine = await context.Cuisines.FindAsync([id], cancellationToken);

        if (cuisine == null)
        {
            return null;
        }

        var authResult = await authorizationService.AuthorizeAsync(
            user,
            cuisine,
            Operations.Update);

        if (!authResult.Succeeded)
        {
            throw new UnauthorizedAccessException(
                $"User is unauthorized to perform {nameof(Operations.Update)} operation on resource.");
        }

        context.Entry(cuisine).CurrentValues.SetValues(dto);

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            await context.Entry(cuisine).ReloadAsync(cancellationToken);
            throw;
        }

        return cuisine.ToCuisineDto();
    }

    public async Task<bool> DeleteAsync(
        int id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        var cuisine = await context.Cuisines.FindAsync([id], cancellationToken);

        if (cuisine == null)
        {
            return false;
        }

        var authResult = await authorizationService.AuthorizeAsync(
            user,
            cuisine,
            Operations.Delete);

        if (!authResult.Succeeded)
        {
            throw new UnauthorizedAccessException(
                $"User is unauthorized to perform {nameof(Operations.Delete)} operation on resource.");
        }

        context.Cuisines.Remove(cuisine);

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            await context.Entry(cuisine).ReloadAsync(cancellationToken);
            throw;
        }

        return true;
    }
}
