using System.ComponentModel.DataAnnotations;

namespace Frontend.Models;

public record Employee
{
    public string? Id { get; init; }

    [Required]
    public string? FirstName { get; set; }

    [Required]
    public string? LastName { get; set; }

    [Required]
    public DateTime JoinedOn { get; init; }

    public string? Picture { get; init; }

    public string? ProcessedBy { get; init; }
}