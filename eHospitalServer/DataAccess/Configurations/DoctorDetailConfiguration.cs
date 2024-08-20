using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

internal class DoctorDetailConfiguration : IEntityTypeConfiguration<DoctorDetail>
{
    public void Configure(EntityTypeBuilder<DoctorDetail> builder)
    {
        builder.HasKey(key=>key.Id);
        builder.Property(p=>p.AppointmentPrice).HasColumnType("money");
    }
}