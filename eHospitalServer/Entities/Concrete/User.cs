using Microsoft.AspNetCore.Identity;

namespace Entities;

public class User : IdentityUser<Guid>
{
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string GetName()
  {
    return string.Join(" ", FirstName, LastName);
  }
  
}