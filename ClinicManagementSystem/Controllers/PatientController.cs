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
    [Authorize(Roles ="Patient,Admin")]
    public class PatientController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PatientController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Patient
        public async Task<IActionResult> Index()
        {
            var patients = await _unitOfWork.Patients.GetAll();
            return View(patients);
        }

        // GET: Patient/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var patient = await _unitOfWork.Patients.GetById(id);

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: Patient/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Patient/Create
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
                    EmergencyContactPhone = patientDto.EmergencyContactPhone
                };

                // Handle image upload
                if (patientDto.ImageFile != null && patientDto.ImageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await patientDto.ImageFile.CopyToAsync(memoryStream);
                        patient.ImageData = memoryStream.ToArray(); // Save image data
                    }
                }

                await _unitOfWork.Patients.Add(patient);
                await _unitOfWork.Complete(); // Save changes to the database
                TempData["SuccessMessage"] = "Patient created successfully!";
                return RedirectToAction(nameof(Index));
            }

            // Log errors if the model state is invalid
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage); // Log or inspect
            }

            return View(patientDto); // Return the form with validation errors
        }

        // GET: Patient/Edit/5
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

        // POST: Patient/Edit/5
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

                    // Update patient properties
                    patient.FirstName = patientDto.FirstName;
                    patient.LastName = patientDto.LastName;
                    patient.DateOfBirth = patientDto.DateOfBirth;
                    patient.Gender = patientDto.Gender;
                    patient.PhoneNumber = patientDto.PhoneNumber;
                    patient.Email = patientDto.Email;
                    patient.BloodType = patientDto.BloodType;
                    patient.EmergencyContactName = patientDto.EmergencyContactName;
                    patient.EmergencyContactPhone = patientDto.EmergencyContactPhone;

                    // Handle image upload
                    if (patientDto.ImageFile != null && patientDto.ImageFile.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await patientDto.ImageFile.CopyToAsync(memoryStream);
                            patient.ImageData = memoryStream.ToArray(); // Save new image data
                        }
                    }

                    await _unitOfWork.Patients.Update(patient);
                    await _unitOfWork.Complete(); // Save changes to the database
                    TempData["SuccessMessage"] = "Patient updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await PatientExists(patientDto.PatientID))
                    {
                        return NotFound();
                    }
                    throw; // Re-throw the exception to handle it in middleware
                }

                return RedirectToAction(nameof(Index));
            }

            // Log errors if the model state is invalid
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage); // Log or inspect
            }

            return View(patientDto); // Return the form with validation errors
        }

        // GET: Patient/Delete/5
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

        // POST: Patient/Delete/5
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
            await _unitOfWork.Complete(); // Save changes to the database
            TempData["SuccessMessage"] = "Patient deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> PatientExists(int id)
        {
            var patient = await _unitOfWork.Patients.GetById(id);
            return patient != null;
        }
    }
}
