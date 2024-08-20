using Core.DataAccess;
using DataAccess.Abstract;
using DataAccess.Concrete.Context;
using Entities.Concrete;

namespace DataAccess.Concrete;

public class AppointmentRepository : Repository<Appointment, AppDbContext>, IAppointmentRepository
{
    public AppointmentRepository(AppDbContext context) : base(context)
    {
    }
}
