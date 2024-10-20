namespace ClinicManagementSystem.DTOs
{
    public class AppointmentDTO
    {
        public int AppointmentID { get; set; }
        public int PatientID { get; set; }
        public string? PatientName { get; set; }
        public int DoctorID { get; set; }
        public string? DoctorName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Reason { get; set; }
    }


    public class AppointmentEditDTO
    {
        public int AppointmentID { get; set; }
        public int PatientID { get; set; }

        public int DoctorID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Reason { get; set; }
    }


}
