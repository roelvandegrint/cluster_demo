using System.ComponentModel.DataAnnotations;

namespace Staffing.Shared;

public record Employee
{    
    public string? Id { get; set; }
    
    [Required]
    public string? FirstName { get; set; }
    
    [Required]    
    public string? LastName { get; set; }
    
    [Required]
    public DateTime JoinedOn { get; set; }
    
    public string? Picture { get; set; }
}