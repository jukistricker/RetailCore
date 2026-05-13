using System.Linq;
using RetailCore.Shared.DTOs;

namespace RetailCore.Shared.ViewModels;

public class CartPageViewModel
{
    public PagingResponse<CartItemResponse> CartData { get; set; }
    public decimal GrandTotal => CartData.Items.Sum(x => x.Total);
}