using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class Prescription
    {
        public int PrescriptionID { get; set; }
        public int PatientID { get; set; }
        public int DoctorID { get; set; }
        public int MedicationID { get; set; }

        [MaxLength(50)]
        public string Dosage { get; set; }
      

        [MaxLength(50)]
        public string Frequency { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public byte[] ImageData { get; set; }


        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
        public Medication Medication { get; set; }
    }

}
