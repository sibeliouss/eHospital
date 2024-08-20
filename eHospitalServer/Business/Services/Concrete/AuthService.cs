using Business.Services.Abstract;
using CTS.Result;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Business.Services.Concrete;

 public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly JwtService _jwtProvider;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            JwtService jwtProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtProvider = jwtProvider;
        }

        public async Task<Result<LoginResponseDto>> GetTokenByRefreshTokenAsync(string refreshToken,
            CancellationToken cancellationToken)
        {
            User? user = await _userManager.Users.Where(p => p.RefreshToken == refreshToken)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
            {
                return (500, "Refresh Token unavailable.");
            }

            var loginResponse = await _jwtProvider.CreateToken(user, false);

            return loginResponse;
        }

        public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
        {
            string emailOrUserName = request.EmailOrUserName.ToUpper();

            User? user = await _userManager.Users
                .FirstOrDefaultAsync(p =>
                        p.NormalizedUserName == emailOrUserName ||
                        p.NormalizedEmail == emailOrUserName,
                    cancellationToken);

            if (user is null)
            {
                return (500, "User not found");
            }

            SignInResult signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);

            if (signInResult.IsLockedOut)
            {
                TimeSpan? timeSpan = user.LockoutEnd - DateTime.UtcNow;
                if (timeSpan is not null)
                    return (500,
                        $"Your user has been locked for {Math.Ceiling(timeSpan.Value.TotalMinutes)} minutes due to entering the wrong password 3 times.");
                else
                    return (500, "Your user has been locked out for 5 minutes due to entering the wrong password 3 times.");
            }

            if (signInResult.IsNotAllowed)
            {
                return (500, "Your e-mail address is not confirmed");
            }

            if (!signInResult.Succeeded)
            {
                return (500, "Your password is wrong");
            }

            var loginResponse = await _jwtProvider.CreateToken(user, request.RememberMe);

            return loginResponse;
        }
    }