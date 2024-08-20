namespace Entities.Dtos;

public record AppointmentDetailsDto( Guid AppointmentId,
    string DoctorName,
    string DoctorSpecialty,
    DateTime StartDate,
    DateTime EndDate,
    bool IsItFinished);