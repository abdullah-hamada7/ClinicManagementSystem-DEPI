using ClinicManagementSystem.DTOs;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Controllers
{
    [Authorize(Roles = "Doctor,Admin")]
    public class DoctorsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DoctorsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Dashboard()
        {
            var model = new DashboardViewModel
            {
                DoctorsCount = await _unitOfWork.GetDoctorsCountAsync(),
                PatientsCount = await _unitOfWork.GetPatientsCountAsync(),
                AppointmentsCount = await _unitOfWork.GetAppointmentsCountAsync()
            };
            return View(model);
        }

        public async Task<IActionResult> Index()
        {
            var doctors = await _unitOfWork.Doctors.GetAll();
            return View(doctors);
        }

        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _unitOfWork.Doctors.GetById(id);
            if (doctor == null)
            {
                return NotFound();
            }
            return View(doctor);
        }

        public async Task<IActionResult> Create()
        {
            var departments = await _unitOfWork.Departments.GetAll();
            ViewBag.Departments = new SelectList(departments, "DepartmentID", "DepartmentName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorDTO doctorDto)
        {
            if (ModelState.IsValid)
            {
                var doctor = new Doctor
                {
                    FirstName = doctorDto.FirstName,
                    LastName = doctorDto.LastName,
                    Specialization = doctorDto.Specialization,
                    PhoneNumber = doctorDto.PhoneNumber,
                    Email = doctorDto.Email,
                    LicenseNumber = doctorDto.LicenseNumber,
                    HireDate = doctorDto.HireDate,
                    DepartmentID = doctorDto.DepartmentID
                };

                await _unitOfWork.Doctors.Add(doctor);
                TempData["SuccessMessage"] = "Doctor created successfully!";
                return RedirectToAction(nameof(Index));
            }

            // Re-fetch departments to populate the select list again if the model state is invalid
            var departments = await _unitOfWork.Departments.GetAll();
            ViewBag.Departments = new SelectList(departments, "DepartmentID", "Name");

            return View(doctorDto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _unitOfWork.Doctors.GetById(id);
            if (doctor == null)
            {
                return NotFound();
            }

            // Fetch departments for the select list
            var departments = await _unitOfWork.Departments.GetAll();
            ViewBag.DepartmentID = new SelectList(departments, "DepartmentID", "Name");

            return View(doctor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Doctor doctor)
        {
            if (id != doctor.DoctorID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingDoctor = await _unitOfWork.Doctors.GetById(id);
                    if (existingDoctor == null)
                    {
                        return NotFound();
                    }

                    existingDoctor.FirstName = doctor.FirstName;
                    existingDoctor.LastName = doctor.LastName;
                    existingDoctor.Specialization = doctor.Specialization;
                    existingDoctor.PhoneNumber = doctor.PhoneNumber;
                    existingDoctor.Email = doctor.Email;
                    existingDoctor.LicenseNumber = doctor.LicenseNumber;
                    existingDoctor.HireDate = doctor.HireDate;
                    existingDoctor.DepartmentID = doctor.DepartmentID;

                    await _unitOfWork.Doctors.Update(existingDoctor);
                    TempData["SuccessMessage"] = "Doctor updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await DoctorExists(doctor.DoctorID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Re-fetch departments to populate the select list again if the model state is invalid
            var departments = await _unitOfWork.Departments.GetAll();
            ViewBag.DepartmentID = new SelectList(departments, "DepartmentID", "Name");

            return View(doctor);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _unitOfWork.Doctors.GetById(id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedForDoctor(int id)
        {
            await _unitOfWork.Doctors.Delete(id);
            TempData["SuccessMessage"] = "Doctor deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> DoctorExists(int id)
        {
            var doctor = await _unitOfWork.Doctors.GetById(id);
            return doctor != null;
        }
    }
}
