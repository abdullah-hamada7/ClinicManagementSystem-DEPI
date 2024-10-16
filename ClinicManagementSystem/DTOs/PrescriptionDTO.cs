namespace ClinicManagementSystem.DTOs
{
    public class PrescriptionDTO
    {
            public int PrescriptionID { get; set; }
            public int PatientID { get; set; }
            public int DoctorID { get; set; }
            public int MedicationID { get; set; }
            public string Dosage { get; set; }
            public string Frequency { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        public IFormFile ImageFile { get; set; }

    }
}
