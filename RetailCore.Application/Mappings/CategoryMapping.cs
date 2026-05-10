namespace RetailCore.Application.Mappings;

public class CategoryMapping
{
    public static Category ToEntityCreate(CreateCategoryRequest categoryRequest)
    {
        return new Category
        {
            Id = Guid.CreateVersion7(),
            Name = categoryRequest.Name,
            Description = categoryRequest.Description,
            SortOrder =  categoryRequest.SortOrder,
        };
    }

    public static Category ToEntityUpdate(Guid id, UpdateCategoryRequest categoryRequest)
    {
        return new Category
        {
            Id = id,
            Name = categoryRequest.Name,
            Description = categoryRequest.Description,
            SortOrder = categoryRequest.SortOrder,
            // UpdatedBy = categoryRequest.,
            UpdatedDate = DateTime.Now,
        };
    }

    public static CategoryResponse ToResponse(Category category)
    {
        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            SortOrder = category.SortOrder,
            CreatedBy = category.CreatedBy,
            CreatedDate = category.CreatedDate,
            UpdatedBy = category.UpdatedBy,
            UpdatedDate = category.UpdatedDate,
        };
    }

    public static IEnumerable<CategoryResponse> ToResponses(IEnumerable<Category> categories)
    {
        List<CategoryResponse> categoryResponses = new List<CategoryResponse>();
        foreach (var category in categories)
        {
            categoryResponses.Add(ToResponse(category));
        }
        return categoryResponses;
    }
}