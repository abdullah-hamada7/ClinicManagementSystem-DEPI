namespace ClinicManagementSystem.DTOs
{
    public class MedicalRecordDTO
    { 
            public int RecordID { get; set; }
            public int PatientID { get; set; }
            public int DoctorID { get; set; }
            public DateTime VisitDate { get; set; }
            public string Diagnosis { get; set; }
            public string Prescription { get; set; }
            public string Notes { get; set; }
            public decimal Height { get; set; }
            public decimal Weight { get; set; }
            public string BloodPressure { get; set; }
            public decimal Temperature { get; set; }
        }
}
