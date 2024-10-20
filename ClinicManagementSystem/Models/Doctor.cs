using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class Doctor
    {
        public int DoctorID { get; set; }
        [Required, MaxLength(50)]
        public string FirstName { get; set; }
        [Required, MaxLength(50)]
        public string LastName { get; set; }
        [Required, MaxLength(100)]
        public string Specialization { get; set; }
        [MaxLength(15)]
        public string PhoneNumber { get; set; }
        [MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(20)]
        public string LicenseNumber { get; set; }
        public DateTime? HireDate { get; set; }

        public int DepartmentID { get; set; }
        public Department? Department { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<MedicalRecord> MedicalRecords { get; set; }
        public ICollection<Prescription> Prescriptions { get; set; }

    }
}
