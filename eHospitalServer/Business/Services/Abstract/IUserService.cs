using CTS.Result;
using Entities.Dtos;

namespace Business.Services.Abstract;

public interface IUserService
{
    Task<Result<string>> CreateUserAsync(CreateUserDto request, CancellationToken cancellationToken);
    Task<Result<Guid>>CreatePatientAsync(CreatePatientDto request, CancellationToken cancellationToken);
}