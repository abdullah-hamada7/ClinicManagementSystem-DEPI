using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class Appointment
    {
        public int AppointmentID { get; set; }
        public int PatientID { get; set; }
        public Patient Patient { get; set; }

        public int DoctorID { get; set; }
        public Doctor Doctor { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }
        [MaxLength(255)]
        public string Reason { get; set; }
    }


}
