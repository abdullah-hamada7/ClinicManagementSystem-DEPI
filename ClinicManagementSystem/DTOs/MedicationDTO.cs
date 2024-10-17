namespace ClinicManagementSystem.DTOs
{
    public class MedicationDTO
    {
        public int MedicationID { get; set; }
        public string MedicationName { get; set; }
        public string Description { get; set; }
        public string DosageForm { get; set; }
        public string Manufacturer { get; set; }
        public IFormFile ImageFile { get; set; }

    }
}
