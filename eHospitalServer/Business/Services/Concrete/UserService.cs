using AutoMapper;
using Business.Services.Abstract;
using CTS.Result;
using Entities.Concrete;
using Entities.Dtos;
using Entities.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Business.Services.Concrete;

public sealed class UserService(
    UserManager<User> userManager,
    IMapper mapper) : IUserService
{
    public async Task<Result<string>> CreateUserAsync(CreateUserDto request, CancellationToken cancellationToken)
    {
        // Check if the email already exists
        if (request.Email is not null)
        {
            bool isEmailExist = await userManager.Users.AnyAsync(p => p.Email == request.Email, cancellationToken);
            if (isEmailExist)
            {
                return Result<string>.Failure("Email already exists.");
            }
        }

        // Check if the identity number is not the default and already exists
        if (request.IdentityNumber != "11111111111")
        {
            bool isIdentityNumberExist = await userManager.Users.AnyAsync(p => p.IdentityNumber == request.IdentityNumber, cancellationToken);
            if (isIdentityNumberExist)
            {
                return Result<string>.Failure("Identity number already exists.");
            }
        }

        // Create user
        User user = mapper.Map<User>(request);

        // Check if the username already exists
        bool isUserNameExist = await userManager.Users.AnyAsync(p => p.UserName == user.UserName, cancellationToken);
        if (isUserNameExist)
        {
            return Result<string>.Failure("Username already exists.");
        }

        if (request.Specialty is not null)
        {
            user.DoctorDetail = new DoctorDetail()
            {
                Specialty = (Specialty)request.Specialty,
                WorkingDays = request.WorkingDays ?? new List<string>()
            };
        }

        IdentityResult result;

        // Create user with or without password
        if (request.Password is not null)
        {
            result = await userManager.CreateAsync(user, request.Password);
        }
        else
        {
            result = await userManager.CreateAsync(user);
        }

        if (!result.Succeeded)
        {
            return Result<string>.Failure(500, result.Errors.Select(s => s.Description).ToList());
        }

        return Result<string>.Succeed("User creation is successful.");
    }

    public async Task<Result<Guid>> CreatePatientAsync(CreatePatientDto request, CancellationToken cancellationToken)
    {
        // Check if the email already exists
        if (request.Email is not null)
        {
            bool isEmailExist = await userManager.Users.AnyAsync(p => p.Email == request.Email, cancellationToken);
            if (isEmailExist)
            {
                return Result<Guid>.Failure("Email already exists.");
            }
        }

        // Check if the identity number is not the default and already exists
        if (request.IdentityNumber != "11111111111")
        {
            bool isIdentityNumberExist = await userManager.Users.AnyAsync(p => p.IdentityNumber == request.IdentityNumber, cancellationToken);
            if (isIdentityNumberExist)
            {
                return Result<Guid>.Failure("Identity number already exists.");
            }
        }

        User user = mapper.Map<User>(request);
        user.UserType = UserType.Patient;

        // Ensure unique username
        int number = 0;
        while (await userManager.Users.AnyAsync(p => p.UserName == user.UserName, cancellationToken))
        {
            number++;
            user.UserName += number;
        }

        IdentityResult result = await userManager.CreateAsync(user);

        if (!result.Succeeded)
        {
            return Result<Guid>.Failure(500, result.Errors.Select(s => s.Description).ToList());
        }

        return Result<Guid>.Succeed(user.Id);
    }
}
