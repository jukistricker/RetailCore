using System;

namespace RetailCore.Shared.DTOs;

public class BaseDto
{
    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
}