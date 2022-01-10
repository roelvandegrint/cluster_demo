using System.ComponentModel.DataAnnotations;

namespace Staffing.Shared;

public record Employee
{    
    public string? Id { get; set; }
    
    [Required]
    public string? FirstName { get; init; }
    
    [Required]    
    public string? LastName { get; init; }
    
    [Required]
    public DateTime JoinedOn { get; init; }
    
    public string? Picture { get; init; }
}