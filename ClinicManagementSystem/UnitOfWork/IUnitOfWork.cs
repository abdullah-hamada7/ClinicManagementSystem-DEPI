using ClinicManagementSystem.Models;
using ClinicManagementSystem.Repositories;
using System.Numerics;

namespace ClinicManagementSystem.UnitOfWork
{
    public interface IUnitOfWork:IDisposable
    {
        IRepository<Patient> Patients { get; }
        IRepository<Doctor> Doctors { get; }
        IRepository<Appointment> Appointments { get; }
        IRepository<MedicalRecord> MedicalRecords { get; }
        IRepository<Medication> Medications { get; }
        IRepository<Prescription> Prescriptions { get; }
        IRepository<Department> Departments { get; }
        Task Complete();
    }
}
