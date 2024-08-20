using Entities.Enums;

namespace Entities.Concrete;

public class DoctorDetail
{
    public DoctorDetail()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }
    public Specialty Specialty { get; set; } = Specialty.GeneralMedicine;
    public List<string> WorkingDays { get; set; } = [];
    public decimal AppointmentPrice { get; set; }
    public string SpecialtyName => Specialty.ToString();
}