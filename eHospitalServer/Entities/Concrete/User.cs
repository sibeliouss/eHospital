using Entities.Enums;
using Microsoft.AspNetCore.Identity;

namespace Entities.Concrete;

public class User : IdentityUser<Guid>
{
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string FullName => string.Join(" ", FirstName, LastName);
  public string Address { get; set; } = string.Empty;
  public bool IsActive { get; set; } = true;
  public bool IsDeleted { get; set; } = false;
  public DateOnly? DateOfBirth { get; set; }
  public string? BloodType { get; set; }
  public UserType UserType { get; set; } = UserType.Patient;
  public string? RefreshToken { get; set; }
  public DateTime? RefreshTokenExpires{ get; set; }
  
}