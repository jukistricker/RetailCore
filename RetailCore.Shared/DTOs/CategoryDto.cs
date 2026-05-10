using System;

namespace RetailCore.Shared.DTOs;

public class CategoryResponse: BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    // public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    
}

public class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; } = 0;
}

public class UpdateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
}