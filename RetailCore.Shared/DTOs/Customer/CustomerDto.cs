using System;

namespace RetailCore.Shared.DTOs.Customer;

public class CustomerDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}