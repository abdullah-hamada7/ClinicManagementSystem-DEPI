using ClinicManagementSystem.DTOs;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DoctorsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize(Roles = "Doctor")]
        public IActionResult Dashboard()
        {
            return View();
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

        public IActionResult Create()
        {
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

                if (doctorDto.ImageFile != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await doctorDto.ImageFile.CopyToAsync(memoryStream);
                        doctor.ImageData = memoryStream.ToArray();
                    }
                }

                await _unitOfWork.Doctors.Add(doctor);
                TempData["SuccessMessage"] = "Doctor created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(doctorDto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _unitOfWork.Doctors.GetById(id);
            if (doctor == null)
            {
                return NotFound();
            }

            var doctorDto = new DoctorDTO
            {
                DoctorID = doctor.DoctorID,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                Specialization = doctor.Specialization,
                PhoneNumber = doctor.PhoneNumber,
                Email = doctor.Email,
                LicenseNumber = doctor.LicenseNumber,
                HireDate = doctor.HireDate ?? DateTime.Now,
                DepartmentID = doctor.DepartmentID
            };

            return View(doctorDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DoctorDTO doctorDto)
        {
            if (id != doctorDto.DoctorID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var doctor = await _unitOfWork.Doctors.GetById(id);
                    if (doctor == null)
                    {
                        return NotFound();
                    }

                    doctor.FirstName = doctorDto.FirstName;
                    doctor.LastName = doctorDto.LastName;
                    doctor.Specialization = doctorDto.Specialization;
                    doctor.PhoneNumber = doctorDto.PhoneNumber;
                    doctor.Email = doctorDto.Email;
                    doctor.LicenseNumber = doctorDto.LicenseNumber;
                    doctor.HireDate = doctorDto.HireDate;
                    doctor.DepartmentID = doctorDto.DepartmentID;

                    if (doctorDto.ImageFile != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await doctorDto.ImageFile.CopyToAsync(memoryStream);
                            doctor.ImageData = memoryStream.ToArray();
                        }
                    }

                    await _unitOfWork.Doctors.Update(doctor);
                    TempData["SuccessMessage"] = "doctor updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await DoctorExists(doctorDto.DoctorID))
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
            return View(doctorDto);
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _unitOfWork.Doctors.Delete(id);
            TempData["SuccessMessage"] = "doctor deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> DoctorExists(int id)
        {
            var doctor = await _unitOfWork.Doctors.GetById(id);
            return doctor != null;
        }
    }
}
