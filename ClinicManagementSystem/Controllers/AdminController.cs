using ClinicManagementSystem.DTOs;
using ClinicManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new DashboardViewModel
            {
                AppointmentsCount = _context.Appointments.Count(),
                DoctorsCount = _context.Doctors.Count(),
                PatientsCount = _context.Patients.Count()
            };
            return View(model);
        }


        // GET: Admin/Appointments
        public IActionResult Appointments()
        {
            var appointments = _context.Appointments.ToList();
            return View(appointments);
        }

        // GET: Admin/Doctors
        public IActionResult Doctors()
        {
            var doctors = _context.Doctors.ToList();
            return View(doctors);
        }

        // GET: Admin/Patients
        public IActionResult Patients()
        {
            var patients = _context.Patients.ToList();
            return View(patients);
        }
    }
}
