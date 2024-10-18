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

        public async Task<IActionResult> MedicalRecords()
        {
            var medicalRecords = await _unitOfWork.MedicalRecords.GetAll();
            return View(medicalRecords);
        }

        public async Task<IActionResult> MedicalRecordDetails(int id)
        {
            var medicalRecord = await _unitOfWork.MedicalRecords.GetById(id);
            if (medicalRecord == null)
            {
                return NotFound("The medical record you're looking for does not exist.");
            }
            return View(medicalRecord);
        }

        public IActionResult CreateMedicalRecord()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMedicalRecord(MedicalRecordDTO medicalRecordDto)
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

        public async Task<IActionResult> EditMedicalRecord(int RecordID)
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
        public async Task<IActionResult> EditMedicalRecord(int id, MedicalRecordDTO medicalRecordDto)
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

        public async Task<IActionResult> DeleteMedicalRecord(int id)
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
        public async Task<IActionResult> DeleteConfirmedForMedicalRecord(int id)
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

        public async Task<IActionResult> Appointments()
        {
            var appointments = await _unitOfWork.Appointments.GetAll();
            return View(appointments);
        }

        // GET: Doctor/CreateAppointment
        public IActionResult CreateAppointment()
        {
            ViewBag.Patients = _unitOfWork.Patients.GetAll().Result;
            return View();
        }

        // POST: Doctor/CreateAppointment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAppointment(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Appointments.Add(appointment);
                await _unitOfWork.Complete();

                TempData["SuccessMessage"] = "Appointment created successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Patients = _unitOfWork.Patients.GetAll().Result;
            return View(appointment);
        }

        // GET: Doctor/EditAppointment/5
        public async Task<IActionResult> EditAppointment(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetById(id);
            if (appointment == null) return NotFound();

            ViewBag.Patients = _unitOfWork.Patients.GetAll().Result;
            return View(appointment);
        }

        // POST: Doctor/EditAppointment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAppointment(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Appointments.Update(appointment);
                await _unitOfWork.Complete();
                TempData["SuccessMessage"] = "Appointment updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Patients = _unitOfWork.Patients.GetAll().Result;
            return View(appointment);
        }

        // GET: Doctor/DeleteAppointment/5
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetById(id);
            if (appointment == null) return NotFound();
            return View(appointment);
        }

        // POST: Doctor/DeleteAppointment/5
        [HttpPost, ActionName("DeleteAppointment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetById(id);
            if (appointment != null)
            {
                _unitOfWork.Appointments.Delete(id);
                await _unitOfWork.Complete();
                TempData["SuccessMessage"] = "Appointment deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Medications()
        {
            var medications = await _unitOfWork.Medications.GetAll();
            return View(medications);
        }

        // Create Medication
        public IActionResult CreateMedication()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMedication(Medication medication)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Medications.Add(medication);
                await _unitOfWork.Complete();

                TempData["SuccessMessage"] = "Medication created successfully!";
                return RedirectToAction(nameof(Medications));
            }
            return View(medication);
        }

        // Edit Medication
        public async Task<IActionResult> EditMedication(int id)
        {
            var medication = await _unitOfWork.Medications.GetById(id);
            if (medication == null) return NotFound();

            return View(medication);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMedication(Medication medication)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Medications.Update(medication);
                await _unitOfWork.Complete();
                TempData["SuccessMessage"] = "Medication updated successfully!";
                return RedirectToAction(nameof(Medications));
            }
            return View(medication);
        }

        // Delete Medication
        public async Task<IActionResult> DeleteMedication(int id)
        {
            var medication = await _unitOfWork.Medications.GetById(id);
            if (medication == null) return NotFound();
            return View(medication);
        }

        [HttpPost, ActionName("DeleteMedication")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedForMedication(int id)
        {
            var medication = await _unitOfWork.Medications.GetById(id);
            if (medication != null)
            {
                _unitOfWork.Medications.Delete(id);
                await _unitOfWork.Complete();
                TempData["SuccessMessage"] = "Medication deleted successfully!";
            }
            return RedirectToAction(nameof(Medications));
        }

        #region Index 
        public async Task<IActionResult> Prescriptions()
        {
            var prescription = await _unitOfWork.Prescriptions.GetAll();

            return View(prescription);
        }

        #endregion

        #region Detalis
        public async Task<IActionResult> DetailsPrescription(int id)
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
        public IActionResult CreatePrescription()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePrescription(PrescriptionDTO prescriptiondto)
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
        public async Task<IActionResult> EditPrescription(int id)
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
        public async Task<IActionResult> EditPrescription(int id, PrescriptionDTO prescriptiondto)
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
        public async Task<IActionResult> DeletePrescription(int id)
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
        public async Task<IActionResult> DeleteConfirmedForPrescription(int id)
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

            // Fetch departments for the select list
            var departments = await _unitOfWork.Departments.GetAll();
            ViewBag.Departments = new SelectList(departments, "DepartmentID", "Name");

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
                    TempData["SuccessMessage"] = "Doctor updated successfully!";
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

            // Re-fetch departments to populate the select list again if the model state is invalid
            var departments = await _unitOfWork.Departments.GetAll();
            ViewBag.Departments = new SelectList(departments, "DepartmentID", "Name");

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

        [HttpPost, ActionName("DeleteDoctor")]
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
