﻿using System.Security.Claims;
using RecipeCatalog.Application.Contracts.Models;

namespace RecipeCatalog.Application.Contracts.Services;

public interface ICuisineService
{
    public Task<IEnumerable<CuisineDto>> GetListAsync(
        CancellationToken cancellationToken = default);

    public Task<CuisineDto?> GetAsync(
        int id,
        CancellationToken cancellationToken = default);

    public Task<CuisineDto> CreateAsync(
        CreateUpdateCuisineDto dto,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default);

    public Task<CuisineDto?> UpdateAsync(
        int id,
        CreateUpdateCuisineDto dto,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default);

    public Task<bool> DeleteAsync(
        int id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default);
}
