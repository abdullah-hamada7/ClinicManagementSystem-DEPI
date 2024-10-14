using ClinicManagementSystem.DTOs;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Numerics;

namespace ClinicManagementSystem.Controllers
{
    public class PatientControllerr : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PatientControllerr(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            var patients = await _unitOfWork.Patients.GetAll();
            return View(patients);
        }
        public async Task<IActionResult> Details(int id)
        {
            var patient = await _unitOfWork.Patients.GetById(id); 

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient); 
        }
        
        public ActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient patient)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Patients.Add(patient);
                 
                return RedirectToAction("Index"); 
            }

            return View(patient); 
        }
        
        public async Task<IActionResult> Edit(int id)
        {
            var patient = await _unitOfWork.Patients.GetById(id); 

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient); 
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Patient patient)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Patients.Update(patient);

                return RedirectToAction("Index"); 
            }

            return View(patient); 
        }
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

                    if (patientDto.ImageFile != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await patientDto.ImageFile.CopyToAsync(memoryStream);
                            patient.ImageData = memoryStream.ToArray();
                        }
                    }

                    await _unitOfWork.Doctors.Update(patient);
                    TempData["SuccessMessage"] = "doctor updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await PatientExists(patientDto.PatientID))
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
            return View(patientDto);
        }
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

                if (patientDto.ImageFile != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await patientDto.ImageFile.CopyToAsync(memoryStream);
                        patient.ImageData = memoryStream.ToArray();
                    }
                }

                await _unitOfWork.Patients.Add(patient);
                TempData["SuccessMessage"] = "Doctor created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(patientDto);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var patient = await _unitOfWork.Patients.GetById(id);
            await _unitOfWork.Patients.Delete(id);

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient); 
        }

    }
}
