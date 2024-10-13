using ClinicManagementSystem.DTOs;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Controllers
{
    public class PrescriptionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public PrescriptionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Index 
        public async Task<IActionResult> Index()
        {
            var prescription = await _unitOfWork.Prescriptions.GetAll();

            return View(prescription);
        }

        #endregion

        #region Detalis
        public async Task<IActionResult> Details(int id)
        {
            var prescription = await _unitOfWork.Prescriptions.GetById(id);
            if (prescription == null)
            {
                return NotFound();
            }
            return View(prescription);
        }
        #endregion

        #region Create 
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PrescriptionDTO prescriptiondto)
        {
            if (ModelState.IsValid)
            {
                var prescription = new Prescription
                {
                    PrescriptionID = prescriptiondto.PrescriptionID,
                    PatientID = prescriptiondto.PatientID,
                    DoctorID = prescriptiondto.DoctorID,
                    MedicationID = prescriptiondto.MedicationID,
                    Dosage = prescriptiondto.Dosage,
                    Frequency = prescriptiondto.Frequency,
                    StartDate = prescriptiondto.StartDate,
                    EndDate = prescriptiondto.EndDate
                };

                if (prescriptiondto.ImageFile != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await prescriptiondto.ImageFile.CopyToAsync(memoryStream);
                        prescription.ImageData = memoryStream.ToArray();
                    }
                }

                await _unitOfWork.Prescriptions.Add(prescription);
                TempData["SuccessMessage"] = "Doctor created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(prescriptiondto);
        }
        #endregion

        #region Edit 
        public async Task<IActionResult> Edit(int id)
        {
            var prescription = await _unitOfWork.Prescriptions.GetById(id);
            if (prescription == null)
            {
                return NotFound();
            }

            var prescriptiondto = new PrescriptionDTO
            {
                PrescriptionID = prescription.PrescriptionID,
                PatientID = prescription.PatientID,
                DoctorID = prescription.DoctorID,
                MedicationID = prescription.MedicationID,
                Dosage = prescription.Dosage,
                Frequency = prescription.Frequency,
                StartDate = prescription.StartDate ?? DateTime.Now,
                EndDate = prescription.EndDate ?? DateTime.Now,
            };

            return View(prescriptiondto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PrescriptionDTO prescriptiondto)
        {
            if (id != prescriptiondto.PrescriptionID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var prescription = await _unitOfWork.Prescriptions.GetById(id);
                    if (prescription == null)
                    {
                        return NotFound();
                    }
                    prescription.PrescriptionID = prescriptiondto.PrescriptionID;
                    prescription.PatientID = prescriptiondto.PatientID;
                    prescription.DoctorID = prescriptiondto.DoctorID;
                    prescription.MedicationID = prescriptiondto.MedicationID;
                    prescription.Dosage = prescriptiondto.Dosage;
                    prescription.Frequency = prescriptiondto.Frequency;
                    prescription.StartDate = prescriptiondto.StartDate;
                    prescription.EndDate = prescriptiondto.EndDate;



                    if (prescriptiondto.ImageFile != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await prescriptiondto.ImageFile.CopyToAsync(memoryStream);
                            prescription.ImageData = memoryStream.ToArray();
                        }
                    }

                    await _unitOfWork.Prescriptions.Update(prescription);
                    TempData["SuccessMessage"] = "Prescription updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await PrescriptionsExists(prescriptiondto.PrescriptionID))
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
            return View(prescriptiondto);
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int id)
        {
            var prescription = await _unitOfWork.Prescriptions.GetById(id);
            if (prescription == null)
            {
                return NotFound();
            }

            return View(prescription);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _unitOfWork.Prescriptions.Delete(id);
            TempData["SuccessMessage"] = "Prescription deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        #endregion  
        private async Task<bool> PrescriptionsExists(int id)
        {
            var prescription = await _unitOfWork.Prescriptions.GetById(id);
            return prescription != null;
        }
    }
}
