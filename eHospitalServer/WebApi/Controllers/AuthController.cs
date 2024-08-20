using Business.Services.Abstract;
using Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Absractions;

namespace WebApi.Controllers;

public class AuthController(IAuthService authService) :ApiController
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequestDto request, CancellationToken cancellationToken)
    {
        var response= await authService.LoginAsync(request, cancellationToken);

        return StatusCode(response.StatusCode, response);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetTokenByRefreshToken(string refreshToken, CancellationToken cancellationToken)
    {
        var response = await authService.GetTokenByRefreshTokenAsync(refreshToken, cancellationToken);

        return StatusCode(response.StatusCode, response);
    }
    

   
}