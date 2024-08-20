namespace Entities.Dtos;

public record CompleteAppointmentDto(
    Guid AppointmentId,
    string EpicrisisReport);