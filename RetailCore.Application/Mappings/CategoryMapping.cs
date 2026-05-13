namespace RetailCore.Application.Mappings;

public static class CategoryMapping
{
    public static Category ToEntityCreate(CreateCategoryRequest categoryRequest, Guid userId)
    {
        return new Category
        {
            Id = Guid.CreateVersion7(),
            Name = categoryRequest.Name,
            Description = categoryRequest.Description,
            SortOrder = categoryRequest.SortOrder,
            CreatedBy = userId,
            CreatedDate = DateTime.Now
        };
    }

    public static void ToEntityUpdate(Category category, UpdateCategoryRequest categoryRequest, Guid userId)
    {
        category.Name = categoryRequest.Name;
        category.Description = categoryRequest.Description;
        category.SortOrder = categoryRequest.SortOrder;
        category.UpdatedDate = DateTime.Now;
        category.UpdatedBy = userId;
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
            UpdatedDate = category.UpdatedDate
        };
    }

    public static PagingResponse<CategoryResponse> ToPagingResponse(PagingResponse<Category> pagingResponse)
    {
        return new PagingResponse<CategoryResponse>
        {
            Items = pagingResponse.Items.Select(x => ToResponse(x)).ToList(),
            TotalCount = pagingResponse.TotalCount,
            PageNumber = pagingResponse.PageNumber,
            PageSize = pagingResponse.PageSize
        };
    }
}