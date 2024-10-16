using ClinicManagementSystem.DTOs;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.Services;
using ClinicManagementSystem.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Controllers
{
    [Authorize(Roles = "Patient")]
    public class AppointmentsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public AppointmentsController(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            var appointments = await _unitOfWork.Appointments.GetAll();
            return View(appointments);
        }

        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetById(id);
            if (appointment == null)
            {
                return NotFound();
            }

            var appointmentDto = new AppointmentDTO
            {
                AppointmentID = appointment.AppointmentID,
                PatientID = appointment.PatientID,
                DoctorID = appointment.DoctorID,
                AppointmentDate = appointment.AppointmentDate,
                Reason = appointment.Reason
            };

            return View(appointmentDto); // Ensure the view expects AppointmentDTO
        }

        public IActionResult Create()
        {
            ViewBag.Patients = _unitOfWork.Patients.GetAll().Result; // Adjust as necessary
            ViewBag.Doctors = _unitOfWork.Doctors.GetAll().Result; // Adjust as necessary
            return View(); // Returns the view to create an appointment
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Appointments.Add(appointment);
                await _unitOfWork.Complete();

                // Get patient email from the PatientID
                var patient = await _unitOfWork.Patients.GetById(appointment.PatientID);
                if (patient != null)
                {
                    var emailModel = new EmailModel
                    {
                        To = patient.Email, // Assuming the Patient model has an Email property
                        Subject = "Appointment Confirmation",
                        Body = $"Dear {patient.FirstName},\n\nYour appointment has been scheduled for {appointment.AppointmentDate:MMMM dd, yyyy h:mm tt}.\n\nBest Regards,\nClinic Management System"
                    };

                    // Send confirmation email
                    await _emailService.SendEmailAsync(emailModel.To, emailModel.Subject, emailModel.Body);
                }

                TempData["SuccessMessage"] = "Appointment created successfully and email sent!";
                return RedirectToAction(nameof(Index));
            }

            return View(appointment); // Return the model back to the view in case of validation failure
        }

        public async Task<IActionResult> Edit(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetById(id);
            if (appointment == null)
            {
                return NotFound();
            }

            // Populate ViewBag with Patients and Doctors
            ViewBag.Patients = await _unitOfWork.Patients.GetAll(); // Make sure to await this
            ViewBag.Doctors = await _unitOfWork.Doctors.GetAll(); // Make sure to await this

            var appointmentDto = new AppointmentDTO
            {
                AppointmentID = appointment.AppointmentID,
                PatientID = appointment.PatientID,
                DoctorID = appointment.DoctorID,
                AppointmentDate = appointment.AppointmentDate,
                Reason = appointment.Reason
            };

            return View(appointmentDto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppointmentDTO appointmentDto)
        {
            if (id != appointmentDto.AppointmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var appointment = await _unitOfWork.Appointments.GetById(id);
                    if (appointment == null)
                    {
                        return NotFound();
                    }

                    appointment.PatientID = appointmentDto.PatientID;
                    appointment.DoctorID = appointmentDto.DoctorID;
                    appointment.AppointmentDate = appointmentDto.AppointmentDate;
                    appointment.Reason = appointmentDto.Reason;

                    await _unitOfWork.Appointments.Update(appointment);
                    await _unitOfWork.Complete(); // Ensure you save changes after updating
                    TempData["SuccessMessage"] = "Appointment updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await AppointmentExists(appointmentDto.AppointmentID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw; // Re-throw the exception for unexpected errors
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(appointmentDto); // Return the DTO back to the view in case of validation failure
        }

        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetById(id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment); // Ensure the view expects Appointment model
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _unitOfWork.Appointments.Delete(id);
            await _unitOfWork.Complete(); // Ensure you save changes after deletion
            TempData["SuccessMessage"] = "Appointment deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> AppointmentExists(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetById(id);
            return appointment != null; // Check for appointment existence
        }
    }
}
