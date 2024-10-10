using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }
        [Required, MaxLength(100)]
        public string DepartmentName { get; set; }
        [MaxLength(100)]
        public string Location { get; set; }

        public ICollection<Doctor> Doctors { get; set; }
    }


}
