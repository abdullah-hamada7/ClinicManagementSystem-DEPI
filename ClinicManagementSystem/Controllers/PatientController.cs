using ClinicManagementSystem.DTOs;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Controllers
{
    public class PatientController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PatientController> _logger;

        public PatientController(IUnitOfWork unitOfWork, ILogger<PatientController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        [Authorize(Roles = "Doctor,Admin")]

        public async Task<IActionResult> Index()
        {
            var patients = await _unitOfWork.Patients.GetAll();
            return View(patients);
        }

        [Authorize(Roles = "Doctor,Admin,Patient")]
        public async Task<IActionResult> Details(int id)
        {
            var patient = await _unitOfWork.Patients.GetById(id);

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        [Authorize(Roles = "Doctor,Admin,Patient")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Doctor,Admin,Patient")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PatientDTO patientDto)
        {
            if (ModelState.IsValid)
            {
                var patient = new Patient
                {
                    FirstName = patientDto.FirstName,
                    LastName = patientDto.LastName,
                    DateOfBirth = patientDto.DateOfBirth,
                    Gender = patientDto.Gender,
                    PhoneNumber = patientDto.PhoneNumber,
                    Email = patientDto.Email,
                    BloodType = patientDto.BloodType,
                    EmergencyContactName = patientDto.EmergencyContactName,
                    EmergencyContactPhone = patientDto.EmergencyContactPhone,
                    Address=patientDto.Address
                };

                // Handle image upload
                if (patientDto.ImageFile != null && patientDto.ImageFile.Length > 0)
                {
                    var acceptedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                    if (acceptedTypes.Contains(patientDto.ImageFile.ContentType))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await patientDto.ImageFile.CopyToAsync(memoryStream);
                            patient.ImageData = memoryStream.ToArray(); // Save image data
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("ImageFile", "Please upload a valid image file (JPEG, PNG, GIF).");
                        return View(patientDto);
                    }
                }

                await _unitOfWork.Patients.Add(patient);
                await _unitOfWork.Complete();
                TempData["SuccessMessage"] = "Patient created successfully!";
                return RedirectToAction(nameof(Index));
            }

            LogModelErrors();
            return View(patientDto);
        }
        [Authorize(Roles = "Doctor,Admin,Patient")]

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var patient = await _unitOfWork.Patients.GetById(id);
            if (patient == null)
            {
                return NotFound();
            }

            var patientDto = new PatientDTO
            {
                PatientID = patient.PatientID,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                PhoneNumber = patient.PhoneNumber,
                Email = patient.Email,
                BloodType = patient.BloodType,
                EmergencyContactName = patient.EmergencyContactName,
                EmergencyContactPhone = patient.EmergencyContactPhone
            };

            return View(patientDto);
        }

        [Authorize(Roles = "Doctor,Admin,Patient")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PatientDTO patientDto)
        {
            if (id != patientDto.PatientID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var patient = await _unitOfWork.Patients.GetById(id);
                    if (patient == null)
                    {
                        return NotFound();
                    }

                    patient.FirstName = patientDto.FirstName;
                    patient.LastName = patientDto.LastName;
                    patient.DateOfBirth = patientDto.DateOfBirth;
                    patient.Gender = patientDto.Gender;
                    patient.PhoneNumber = patientDto.PhoneNumber;
                    patient.Email = patientDto.Email;
                    patient.BloodType = patientDto.BloodType;
                    patient.EmergencyContactName = patientDto.EmergencyContactName;
                    patient.EmergencyContactPhone = patientDto.EmergencyContactPhone;

                    if (patientDto.ImageFile != null && patientDto.ImageFile.Length > 0)
                    {
                        var acceptedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                        if (acceptedTypes.Contains(patientDto.ImageFile.ContentType))
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await patientDto.ImageFile.CopyToAsync(memoryStream);
                                patient.ImageData = memoryStream.ToArray();
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("ImageFile", "Please upload a valid image file (JPEG, PNG, GIF).");
                            return View(patientDto);
                        }
                    }

                    await _unitOfWork.Patients.Update(patient);
                    await _unitOfWork.Complete();
                    TempData["SuccessMessage"] = "Patient updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await PatientExists(patientDto.PatientID))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            LogModelErrors();
            return View(patientDto);
        }

        [Authorize(Roles = "Doctor,Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var patient = await _unitOfWork.Patients.GetById(id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        [Authorize(Roles = "Doctor,Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _unitOfWork.Patients.GetById(id);
            if (patient == null)
            {
                return NotFound();
            }

            await _unitOfWork.Patients.Delete(id);
            await _unitOfWork.Complete();
            TempData["SuccessMessage"] = "Patient deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> PatientExists(int id)
        {
            return await _unitOfWork.Patients.GetById(id) != null;
        }

        private void LogModelErrors()
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                _logger.LogError(error.ErrorMessage);
            }
        }
    }
}
