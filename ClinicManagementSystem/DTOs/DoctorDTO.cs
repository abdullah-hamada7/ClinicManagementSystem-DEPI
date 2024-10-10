namespace ClinicManagementSystem.DTOs
{
    public class DoctorDTO
    {
            public int DoctorID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Specialization { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
            public string LicenseNumber { get; set; }
            public DateTime HireDate { get; set; }
            public int DepartmentID { get; set; }
            public IFormFile ImageFile { get; set; }
        }
}
