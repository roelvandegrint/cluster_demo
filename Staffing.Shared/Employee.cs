using System.ComponentModel.DataAnnotations;

namespace Staffing.Shared;

public record Employee
{
    private string? picture;

    public string? Id { get; init; }

    public Employee()
    {
        Id = Guid.NewGuid().ToString();
        ProcessedBy = Environment.GetEnvironmentVariable("RVDG_SERVICE_NAME");
    }

    [Required]
    public string? FirstName { get; init; }

    [Required]
    public string? LastName { get; init; }

    [Required]
    public DateTime JoinedOn { get; init; }

    public string? Picture
    {
        get => picture;
        set
        {
            switch (FirstName?.ToUpperInvariant())
            {
                case "ROEL":
                    picture = "Roel.jpg";
                    return;
                case "DONALD":
                    picture = "Donald.jpg";
                    return;
                default:
                    picture = value;
                    break;
            }
        }
    }

    public string? ProcessedBy { get; init;}
}