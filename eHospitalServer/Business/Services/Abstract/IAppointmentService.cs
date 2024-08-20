using CTS.Result;
using Entities.Concrete;
using Entities.Dtos;

namespace Business.Services.Abstract;

public interface IAppointmentService
{
    Task<Result<string>> CreateAsync(CreateAppointmentDto request, CancellationToken cancellationToken);
    Task<Result<string>> CompleteAppointmentAsync(CompleteAppointmentDto request, CancellationToken cancellationToken);
    Task<Result<List<Appointment>>> GetAllAppointmentByDoktorIdAsync(Guid doctorId, CancellationToken cancellationToken);
    Task<Result<User?>> FindPatientByIdentityNumberAsync(FindPatientDto request, CancellationToken cancellationToken);
    Task<Result<List<User>>> GetAllDoctorsAsync(CancellationToken cancellationToken);
    Task<Result<string>> DeleteByIdAsync (Guid id, CancellationToken cancellationToken);
    Task<Result<List<AppointmentDetailsDto>>> GetAllAppointmentByPatientIdAsync(Guid patientId, CancellationToken cancellationToken);

}