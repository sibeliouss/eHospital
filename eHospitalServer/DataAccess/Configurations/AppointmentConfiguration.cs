using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

internal class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.Property(p=>p.Price).HasColumnType("money");
        builder.HasQueryFilter(filter => (!filter.Doctor!.IsDeleted || !filter.Patient!.IsDeleted) && !filter.IsItFinished);
        builder.HasOne(a => a.Doctor).WithMany()
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.NoAction); // or Restrict

        builder.HasOne(a => a.Patient).WithMany()
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}