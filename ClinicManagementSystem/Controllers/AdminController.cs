using ClinicManagementSystem.DTOs;
using ClinicManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

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
                DoctorsCount = _context.Doctors.Count(),
                PatientsCount = _context.Patients.Count()
            };
            return View(model);
        }


        public IActionResult Doctors()
        {
            var doctors = _context.Doctors.ToList();
            return View(doctors);
        }

        public IActionResult Patients()
        {
            var patients = _context.Patients.ToList();
            return View(patients);
        }
    }
}
