using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class Medication
    {
        public int MedicationID { get; set; }
        [Required, MaxLength(100)]
        public string MedicationName { get; set; }
        [MaxLength(255)]
        public string Description { get; set; }
        [MaxLength(50)]
        public string DosageForm { get; set; }
        [MaxLength(100)]
        public string Manufacturer { get; set; }

        public ICollection<Prescription> Prescriptions { get; set; }
        public byte[] ImageData { get; set; }
    }


}
