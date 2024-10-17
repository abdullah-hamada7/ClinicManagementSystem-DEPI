using ClinicManagementSystem.DTOs;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Controllers
{
    public class MedicalRecordsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public MedicalRecordsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var medicalRecords = await _unitOfWork.MedicalRecords.GetAll();
            return View(medicalRecords);
        }

        public async Task<IActionResult> Details(int id)
        {
            var medicalRecord = await _unitOfWork.MedicalRecords.GetById(id);
            if (medicalRecord == null)
            {
                return NotFound("The medical record you're looking for does not exist.");
            }
            return View(medicalRecord);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicalRecordDTO medicalRecordDto)
        {
            if (ModelState.IsValid)
            {
                var medicalRecord = new MedicalRecord
                {
                    PatientID = medicalRecordDto.PatientID,
                    DoctorID = medicalRecordDto.DoctorID,
                    VisitDate = medicalRecordDto.VisitDate,
                    Diagnosis = medicalRecordDto.Diagnosis,
                    Prescription = medicalRecordDto.Prescription,
                    Notes = medicalRecordDto.Notes,
                    Height = medicalRecordDto.Height,
                    Weight = medicalRecordDto.Weight,
                    BloodPressure = medicalRecordDto.BloodPressure,
                    Temperature = medicalRecordDto.Temperature
                };

                await _unitOfWork.MedicalRecords.Add(medicalRecord);
                TempData["SuccessMessage"] = "Medical record created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(medicalRecordDto);
        }

        public async Task<IActionResult> Edit(int RecordID)
        {
            var medicalRecord = await _unitOfWork.MedicalRecords.GetById(RecordID);
            if (medicalRecord == null)
            {
                return NotFound("The medical record you're trying to edit does not exist.");
            }

            var medicalRecordDto = new MedicalRecordDTO
            {
                RecordID = medicalRecord.ID,
                PatientID = medicalRecord.PatientID,
                DoctorID = medicalRecord.DoctorID,
                VisitDate = medicalRecord.VisitDate,
                Diagnosis = medicalRecord.Diagnosis,
                Prescription = medicalRecord.Prescription,
                Notes = medicalRecord.Notes,
                Height = medicalRecord.Height,
                Weight = medicalRecord.Weight,
                BloodPressure = medicalRecord.BloodPressure,
                Temperature = medicalRecord.Temperature
            };

            return View(medicalRecordDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MedicalRecordDTO medicalRecordDto)
        {
            if (id != medicalRecordDto.RecordID)
            {
                return NotFound("Record ID mismatch.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var medicalRecord = await _unitOfWork.MedicalRecords.GetById(id);
                    if (medicalRecord == null)
                    {
                        return NotFound("The medical record you're trying to update does not exist.");
                    }

                    medicalRecord.PatientID = medicalRecordDto.PatientID;
                    medicalRecord.DoctorID = medicalRecordDto.DoctorID;
                    medicalRecord.VisitDate = medicalRecordDto.VisitDate;
                    medicalRecord.Diagnosis = medicalRecordDto.Diagnosis;
                    medicalRecord.Prescription = medicalRecordDto.Prescription;
                    medicalRecord.Notes = medicalRecordDto.Notes;
                    medicalRecord.Height = medicalRecordDto.Height;
                    medicalRecord.Weight = medicalRecordDto.Weight;
                    medicalRecord.BloodPressure = medicalRecordDto.BloodPressure;
                    medicalRecord.Temperature = medicalRecordDto.Temperature;

                    await _unitOfWork.MedicalRecords.Update(medicalRecord);
                    TempData["SuccessMessage"] = "Medical record updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await MedicalRecordExists(medicalRecordDto.RecordID))
                    {
                        return NotFound("The medical record no longer exists.");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(medicalRecordDto);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var medicalRecord = await _unitOfWork.MedicalRecords.GetById(id);
            if (medicalRecord == null)
            {
                return NotFound("The medical record you're trying to Delete does not exist.");
            }

            ViewBag.ConfirmationMessage = "Are you sure you want to Delete this medical record?";
            return View(medicalRecord);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicalRecord = await _unitOfWork.MedicalRecords.GetById(id);
            if (medicalRecord == null)
            {
                return NotFound("The medical record you're trying to Delete no longer exists.");
            }

            try
            {
                await _unitOfWork.MedicalRecords.Delete(id);
                TempData["SuccessMessage"] = $"Medical record for patient ID {medicalRecord.PatientID} Deleted successfully!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while trying to Delete the medical record. Please try again.";
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> MedicalRecordExists(int id)
        {
            var medicalRecord = await _unitOfWork.MedicalRecords.GetById(id);
            return medicalRecord != null;
        }
    }
}
