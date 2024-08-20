using Business.Services.Abstract;
using Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Absractions;

namespace WebApi.Controllers;

public class UsersController(
    IUserService userService) : ApiController
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create(CreateUserDto request, CancellationToken cancellationToken)
    {
        var response = await userService.CreateUserAsync(request, cancellationToken);

        return StatusCode(response.StatusCode, response);
    }
}
