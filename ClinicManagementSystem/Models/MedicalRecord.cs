using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class MedicalRecord
    {
        public int ID { get; set; }
        [Display(Name ="Patient")]
        public int PatientID { get; set; }
        public Patient Patient { get; set; }
        [Display(Name = "Doctor")]

        public int DoctorID { get; set; }
        public Doctor Doctor { get; set; }

        [Required]
        public DateTime VisitDate { get; set; }
        [MaxLength(255)]
        public string Diagnosis { get; set; }
        [MaxLength(255)]
        public string Prescription { get; set; }
        public string Notes { get; set; }

        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        [MaxLength(20)]
        public string BloodPressure { get; set; }
        public decimal Temperature { get; set; }
    }


}
