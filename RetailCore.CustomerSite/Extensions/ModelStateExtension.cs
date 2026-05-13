using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RetailCore.CustomerSite.Extensions;

public static class ModelStateExtensions
{
    public static void AddResultErrors(this ModelStateDictionary modelState, List<ErrorDetail>? errors)
    {
        if (errors == null || !errors.Any()) return;

        foreach (var error in errors) modelState.AddModelError(error.Key ?? string.Empty, error.Message);
    }
}