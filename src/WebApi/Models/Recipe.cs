﻿using System.ComponentModel.DataAnnotations;

namespace JonathanPotts.RecipeCatalog.WebApi.Models;

public class Recipe
{
    public long Id { get; set; }

    [Required]
    public string? Name { get; set; }

    public string? CoverImage { get; set; }

    public string? CoverImagePrompt { get; set; }

    public int CuisineId { get; set; }

    [Required]
    public Cuisine? Cuisine { get; set; }

    public string? Description { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Modified { get; set; }

    [Required]
    public string[]? Ingredients { get; set; }

    [Required]
    public MarkdownData? Instructions { get; set; }
}