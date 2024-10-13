using ClinicManagementSystem.DTOs;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Controllers
{
    public class Medications_Controller : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public Medications_Controller(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Index 
        public async Task<IActionResult> Index()
        {
            var Medication = await _unitOfWork.Medications.GetAll();

            return View(Medication);
        }

        #endregion

        #region Detalis
        public async Task<IActionResult> Details(int id)
        {
            var Medication = await _unitOfWork.Medications.GetById(id);
            if (Medication == null)
            {
                return NotFound();
            }
            return View(Medication);
        }
        #endregion

        #region Create 
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicationDTO medicationdto)
        {
            if (ModelState.IsValid)
            {
                var medication = new Medication
                {
                    MedicationID = medicationdto.MedicationID,
                    MedicationName = medicationdto.MedicationName,
                    Description = medicationdto.Description,
                    DosageForm = medicationdto.DosageForm,
                    Manufacturer = medicationdto.Manufacturer,
                };

                if (medicationdto.ImageFile != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await medicationdto.ImageFile.CopyToAsync(memoryStream);
                        medication.ImageData = memoryStream.ToArray();
                    }
                }

                await _unitOfWork.Medications.Add(medication);
                TempData["SuccessMessage"] = "Medication created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(medicationdto);
        }
        #endregion

        #region Edit 
        public async Task<IActionResult> Edit(int id)
        {
            var medicaton = await _unitOfWork.Medications.GetById(id);
            if (medicaton == null)
            {
                return NotFound();
            }

            var medicationdto = new MedicationDTO
            {
                MedicationID = medicaton.MedicationID,
                MedicationName = medicaton.MedicationName,
                Description = medicaton.Description,
                DosageForm = medicaton.DosageForm,
                Manufacturer = medicaton.Manufacturer,
            };

            return View(medicationdto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MedicationDTO medicationdto)
        {
            if (id != medicationdto.MedicationID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var medication = await _unitOfWork.Medications.GetById(id);
                    if (medication == null)
                    {
                        return NotFound();
                    }
                    medication.MedicationID = medicationdto.MedicationID;
                    medication.MedicationName = medicationdto.MedicationName;
                    medication.Description = medicationdto.Description;
                    medication.DosageForm = medicationdto.DosageForm;
                    medication.Manufacturer = medicationdto.Manufacturer;
                   

                    if (medicationdto.ImageFile != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await medicationdto.ImageFile.CopyToAsync(memoryStream);
                            medication.ImageData = memoryStream.ToArray();
                        }
                    }

                    await _unitOfWork.Medications.Update(medication);
                    TempData["SuccessMessage"] = "Medication updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await MedicationExists(medicationdto.MedicationID))
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
            return View(medicationdto);
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int id)
        {
            var medication = await _unitOfWork.Medications.GetById(id);
            if (medication == null)
            {
                return NotFound();
            }

            return View(medication);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _unitOfWork.Medications.Delete(id);
            TempData["SuccessMessage"] = "Medication deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        #endregion

        private async Task<bool> MedicationExists(int id)
        {
            var medication = await _unitOfWork.Medications.GetById(id);
            return medication != null;
        }
    }
}
