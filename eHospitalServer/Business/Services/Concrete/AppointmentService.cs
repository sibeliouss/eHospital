using AutoMapper;
using Business.Services.Abstract;
using CTS.Result;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using Entities.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Business.Services.Concrete;

public class AppointmentService : IAppointmentService
    {
        private readonly UserManager<User> _userManager;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public AppointmentService(
            UserManager<User> userManager,
            IAppointmentRepository appointmentRepository,
            IUnitOfWork unitOfWork,
            IUserService userService,
            IMapper mapper)
        {
            _userManager = userManager;
            _appointmentRepository = appointmentRepository;
            _unitOfWork = unitOfWork;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<Result<string>> CreateAsync(CreateAppointmentDto request, CancellationToken cancellationToken)
        {
            // Check if the doctor exists
            User? doctor = await _userManager.Users.Include(p => p.DoctorDetail)
                .FirstOrDefaultAsync(p => p.Id == request.DoctorId, cancellationToken);

            if (doctor is null || doctor.UserType != UserType.Doctor)
            {
                return Result<string>.Failure(500, "Doctor not found.");
            }

            // Check doctor working days
            string day = request.StartDate.ToString("dddd");
            if (!doctor.DoctorDetail!.WorkingDays.Contains(day))
            {
                return Result<string>.Failure(500, "Doctor does not work on the requested day.");
            }

            // Check doctor working hours
            DateTime startDate = DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc);
            DateTime endDate = DateTime.SpecifyKind(request.EndDate, DateTimeKind.Utc);

            bool isDoctorAvailable = !await _appointmentRepository
                .GetWhere(p => p.DoctorId == request.DoctorId)
                .AnyAsync(p => 
                    (p.StartDate < endDate && p.StartDate >= startDate) ||
                    (p.EndDate > startDate && p.EndDate <= endDate) ||
                    (p.StartDate >= startDate && p.EndDate <= endDate) ||
                    (p.StartDate <= startDate && p.EndDate >= endDate), 
                    cancellationToken);

            if (!isDoctorAvailable)
            {
                return Result<string>.Failure(500, "Doctor is not available on the requested date.");
            }

            // Create appointment and save to database
            Appointment appointment = _mapper.Map<Appointment>(request);

            if (request.PatientId is null)
            {
                CreatePatientDto createPatientDto = new(
                    request.FirstName,
                    request.LastName,
                    request.IdentityNumber,
                    request.FullAddress,
                    request.Email,
                    request.PhoneNumber,
                    request.DateOfBirth,
                    request.BloodType);

                var createPatientResponse = await _userService.CreatePatientAsync(createPatientDto, cancellationToken);
                appointment.PatientId = createPatientResponse.Data;
            }

            await _appointmentRepository.AddAsync(appointment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Succeed("Appointment created successfully.");
        }

        public async Task<Result<string>> CompleteAppointmentAsync(CompleteAppointmentDto request, CancellationToken cancellationToken)
        {
            Appointment? appointment = await _appointmentRepository
                .GetByExpressionWithTrackingAsync(p => p.Id == request.AppointmentId, cancellationToken);

            if (appointment is null)
            {
                return Result<string>.Failure("Appointment not found.");
            }

            if (appointment.IsItFinished)
            {
                return Result<string>.Failure("Appointment is already finished. You cannot close it again.");
            }

            appointment.EpicrisisReport = request.EpicrisisReport;
            appointment.IsItFinished = true;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Succeed("Appointment completed successfully.");
        }

        public Task<Result<List<Appointment>>> GetAllAppointmentByDoktorIdAsync(Guid doctorId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<List<Appointment>>> GetAllAppointmentByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken)
        {
            List<Appointment> appointments = await _appointmentRepository
                .GetWhere(p => p.DoctorId == doctorId)
                .Include(p => p.Doctor)
                .Include(p => p.Patient)
                .OrderBy(p => p.StartDate)
                .ToListAsync(cancellationToken);

            return Result<List<Appointment>>.Succeed(appointments);
        }

        public async Task<Result<User?>> FindPatientByIdentityNumberAsync(FindPatientDto request, CancellationToken cancellationToken)
        {
            User? user = await _userManager.Users
                .FirstOrDefaultAsync(p => p.IdentityNumber == request.IdentityNumber, cancellationToken);

            return Result<User?>.Succeed(user);
        }

        public async Task<Result<List<User>>> GetAllDoctorsAsync(CancellationToken cancellationToken)
        {
            var doctors = await _userManager.Users
                .Where(p => p.UserType == UserType.Doctor)
                .Include(p => p.DoctorDetail)
                .OrderBy(p => p.FirstName)
                .ToListAsync(cancellationToken);

            return Result<List<User>>.Succeed(doctors);
        }

        public async Task<Result<string>> DeleteByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            Appointment? appointment = await _appointmentRepository
                .GetByExpressionAsync(p => p.Id == id, cancellationToken);

            if (appointment is null)
            {
                return Result<string>.Failure("Appointment not found.");
            }

            _appointmentRepository.Remove(appointment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Succeed("Appointment deleted successfully.");
        }

        public async Task<Result<List<AppointmentDetailsDto>>> GetAllAppointmentByPatientIdAsync(Guid patientId, CancellationToken cancellationToken)
        {
            var appointments = await _appointmentRepository
                .GetWhere(p => p.PatientId == patientId)
                .Include(p => p.Doctor)
                .ThenInclude(d => d!.DoctorDetail)
                .ToListAsync(cancellationToken);

            var patientAppointments = appointments
                .Select(p => new AppointmentDetailsDto(
                    p.Id,
                    p.Doctor!.FullName,
                    p.Doctor!.DoctorDetail!.SpecialtyName,
                    p.StartDate,
                    p.EndDate,
                    p.IsItFinished))
                .OrderBy(dto => dto.StartDate)
                .ToList();

            return Result<List<AppointmentDetailsDto>>.Succeed(patientAppointments);
        }
    }
