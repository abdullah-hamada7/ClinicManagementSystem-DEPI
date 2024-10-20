using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.Controllers
{
    public class MedicalRecordsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicalRecordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MedicalRecords
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.MedicalRecords.Include(m => m.Doctor).Include(m => m.Patient);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: MedicalRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalRecord = await _context.MedicalRecords
                .Include(m => m.Doctor)
                .Include(m => m.Patient)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (medicalRecord == null)
            {
                return NotFound();
            }

            return View(medicalRecord);
        }

        // GET: MedicalRecords/Create
        public IActionResult Create()
        {
            // Make sure you fetch the list of doctors and patients properly
            var doctors = _context.Doctors.ToList();
            var patients = _context.Patients.ToList();

            // Use "FullName" instead of "FirstName" if you want to display the full name, or another appropriate property
            ViewData["DoctorID"] = new SelectList(doctors, "DoctorID", "FirstName");
            ViewData["PatientID"] = new SelectList(patients, "PatientID", "FirstName");

            return View();
        }

        // POST: MedicalRecords/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,PatientID,DoctorID,VisitDate,Diagnosis,Prescription,Notes,Height,Weight,BloodPressure,Temperature")] MedicalRecord medicalRecord)
        {
            // Re-populate the select lists if the ModelState is invalid
            if (!ModelState.IsValid)
            {
                var doctors = _context.Doctors.ToList();
                var patients = _context.Patients.ToList();

                ViewData["DoctorID"] = new SelectList(doctors, "DoctorID", "FirstName", medicalRecord.DoctorID);
                ViewData["PatientID"] = new SelectList(patients, "PatientID", "FirstName", medicalRecord.PatientID);
                return View(medicalRecord);
            }

            // Add the medical record to the context and save changes if the model state is valid
            try
            {
                _context.Add(medicalRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                ModelState.AddModelError(string.Empty, "An error occurred while saving the medical record. Please try again.");

                var doctors = _context.Doctors.ToList();
                var patients = _context.Patients.ToList();

                // Ensure that the select lists are repopulated if there's an exception
                ViewData["DoctorID"] = new SelectList(doctors, "DoctorID", "FirstName", medicalRecord.DoctorID);
                ViewData["PatientID"] = new SelectList(patients, "PatientID", "FirstName", medicalRecord.PatientID);
                return View(medicalRecord);
            }
        }


        // GET: MedicalRecords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalRecord = await _context.MedicalRecords.FindAsync(id);
            if (medicalRecord == null)
            {
                return NotFound();
            }
            ViewData["DoctorID"] = new SelectList(_context.Doctors, "DoctorID", "FirstName", medicalRecord.DoctorID);
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "FirstName", medicalRecord.PatientID);
            return View(medicalRecord);
        }

        // POST: MedicalRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,PatientID,DoctorID,VisitDate,Diagnosis,Prescription,Notes,Height,Weight,BloodPressure,Temperature")] MedicalRecord medicalRecord)
        {
            if (id != medicalRecord.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicalRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicalRecordExists(medicalRecord.ID))
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
            ViewData["DoctorID"] = new SelectList(_context.Doctors, "DoctorID", "FirstName", medicalRecord.DoctorID);
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "FirstName", medicalRecord.PatientID);
            return View(medicalRecord);
        }

        // GET: MedicalRecords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalRecord = await _context.MedicalRecords
                .Include(m => m.Doctor)
                .Include(m => m.Patient)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (medicalRecord == null)
            {
                return NotFound();
            }

            return View(medicalRecord);
        }

        // POST: MedicalRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicalRecord = await _context.MedicalRecords.FindAsync(id);
            if (medicalRecord != null)
            {
                _context.MedicalRecords.Remove(medicalRecord);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicalRecordExists(int id)
        {
            return _context.MedicalRecords.Any(e => e.ID == id);
        }
    }
}
