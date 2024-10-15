using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class Bills
    {
        public int Id { get; set; }

        [ForeignKey("Patient")]
        public int PatientID { get; set; }

        [ForeignKey("Appointment")]
        public int? AppointmentID { get; set; }

        [Required]
        public DateTime BillDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? PaidAmount { get; set; }

        [MaxLength(20)]
        public string PaymentStatus { get; set; }

        public Patient Patient { get; set; }
        public Appointment Appointment { get; set; }
    }
}
