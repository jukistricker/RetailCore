using System;

namespace RetailCore.Shared.Entities;

public class Customer: BaseEntity
{
    public Guid? UserId { get; set; } // AspNetUsers (Week 3)
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public bool IsActive { get; set; } = true;
}