using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class Patient
    {
        public int PatientID { get; set; }
        [Required, MaxLength(50)]
        public string FirstName { get; set; }
        [Required, MaxLength(50)]
        public string LastName { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public char Gender { get; set; }
        [MaxLength(15)]
        public string PhoneNumber { get; set; }
        [MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(255)]
        public string Address { get; set; }
        [MaxLength(3)]
        public string BloodType { get; set; }
        [MaxLength(100)]
        public string EmergencyContactName { get; set; }
        [MaxLength(15)]
        public string EmergencyContactPhone { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<MedicalRecord> MedicalRecords { get; set; }
        public byte[] ImageData { get; set; }

       
    }

}
