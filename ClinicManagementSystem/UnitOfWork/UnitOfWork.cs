using ClinicManagementSystem.Models;
using ClinicManagementSystem.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace ClinicManagementSystem.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Patients = new Repository<Patient>(_context);
            Doctors = new Repository<Doctor>(_context);
            Appointments = new Repository<Appointment>(_context);
            MedicalRecords = new Repository<MedicalRecord>(_context);
            Medications = new Repository<Medication>(_context);
            Prescriptions = new Repository<Prescription>(_context);
            Departments = new Repository<Department>(_context);
        }

        public IRepository<Patient> Patients { get; private set; }
        public IRepository<Doctor> Doctors { get; private set; }
        public IRepository<Appointment> Appointments { get; private set; }
        public IRepository<MedicalRecord> MedicalRecords { get; private set; }
        public IRepository<Medication> Medications { get; private set; }
        public IRepository<Prescription> Prescriptions { get; private set; }
        public IRepository<Department> Departments { get; private set; }

        public async Task Complete()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
        public async Task<int> GetDoctorsCountAsync()
        {
            return await _context.Set<Doctor>().CountAsync();
        }

        public async Task<int> GetPatientsCountAsync()
        {
            return await _context.Set<Patient>().CountAsync();
        }
        public async Task<int> GetAppointmentsCountAsync()
        {
            return await _context.Set<Appointment>().CountAsync();
        }
    }
}