using ClinicManagementSystem.DTOs;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.Services;
using ClinicManagementSystem.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Controllers
{
    [Authorize(Roles = "Patient,Admin")]
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

            return View(appointmentDto); 
        }

        public IActionResult Create()
        {
            ViewBag.Patients = _unitOfWork.Patients.GetAll().Result; 
            ViewBag.Doctors = _unitOfWork.Doctors.GetAll().Result; 
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Appointments.Add(appointment);
                await _unitOfWork.Complete();

                var patient = await _unitOfWork.Patients.GetById(appointment.PatientID);
                if (patient != null)
                {
                    var emailModel = new EmailModel
                    {
                        To = patient.Email, 
                        Subject = "Appointment Confirmation",
                        Body = $"Dear {patient.FirstName},\n\nYour appointment has been scheduled for {appointment.AppointmentDate:MMMM dd, yyyy h:mm tt}.\n\nBest Regards,\nClinic Management System"
                    };

                    await _emailService.SendEmailAsync(emailModel.To, emailModel.Subject, emailModel.Body);
                }

                TempData["SuccessMessage"] = "Appointment created successfully and email sent!";
                return RedirectToAction(nameof(Index));
            }

            return View(appointment); 
        }

        public async Task<IActionResult> Edit(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetById(id);
            if (appointment == null)
            {
                return NotFound();
            }

            ViewBag.Patients = await _unitOfWork.Patients.GetAll(); 
            ViewBag.Doctors = await _unitOfWork.Doctors.GetAll(); 
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
                    await _unitOfWork.Complete();
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
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(appointmentDto); 
        }

        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetById(id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _unitOfWork.Appointments.Delete(id);
            await _unitOfWork.Complete(); 
            TempData["SuccessMessage"] = "Appointment deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> AppointmentExists(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetById(id);
            return appointment != null;
        }
    }
}
