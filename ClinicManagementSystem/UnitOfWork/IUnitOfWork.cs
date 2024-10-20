using ClinicManagementSystem.Models;
using ClinicManagementSystem.Repositories;

namespace ClinicManagementSystem.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Patient> Patients { get; }
        IRepository<Doctor> Doctors { get; }
        IRepository<Appointment> Appointments { get; }
        IRepository<MedicalRecord> MedicalRecords { get; }
        IRepository<Medication> Medications { get; }
        IRepository<Prescription> Prescriptions { get; }
        IRepository<Department> Departments { get; }
        Task Complete();
        Task<int> GetDoctorsCountAsync();
        Task<int> GetPatientsCountAsync();
        Task<int> GetAppointmentsCountAsync();
        Task<IEnumerable<Appointment>> GetAppointmentsWithDetails();
        Task<Appointment> GetAppointmentsWithDetailsByID(int id);

    }
}
