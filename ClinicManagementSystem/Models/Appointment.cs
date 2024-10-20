using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagementSystem.Models
{
    public class Appointment
    {
        public int AppointmentID { get; set; }

        [ForeignKey("Patient")]
        public int PatientID { get; set; }
        public Patient? Patient { get; set; }

        [ForeignKey("Doctor")]
        public int DoctorID { get; set; }
        public Doctor? Doctor { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }
        [MaxLength(255)]
        public string Reason { get; set; }
    }


}
