using System.ComponentModel.DataAnnotations;

namespace StaffingService.Employees;

public record Employee
{
    public string? Id { get; init; }

    [Required]
    public string? FirstName { get; init; }

    [Required]
    public string? LastName { get; init; }

    public DateTime? JoinedOn { get; init; }

    public string? Picture { get; init; }

    public string? ProcessedBy { get; init; }
}