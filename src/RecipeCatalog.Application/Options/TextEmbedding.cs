﻿using System.ComponentModel.DataAnnotations;

namespace RecipeCatalog.Application.Options;

public class TextEmbedding
{
    [Required]
    public required string Model { get; set; }

    [Required]
    public required string Key { get; set; }

    public string? Endpoint { get; set; }
}
