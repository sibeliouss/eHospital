using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Absractions;

[Route("api/[controller]/[action]")]
[ApiController]
public abstract class ApiController : ControllerBase
{
}