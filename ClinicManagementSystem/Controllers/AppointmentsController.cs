using ClinicManagementSystem.DTOs;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            return View(appointment);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentDTO appointmentDto)
        {
            if (ModelState.IsValid)
            {
                var appointment = new Appointment
                {
                    PatientID = appointmentDto.PatientID,
                    DoctorID = appointmentDto.DoctorID,
                    AppointmentDate = appointmentDto.AppointmentDate,
                    Reason = appointmentDto.Reason
                };

                await _unitOfWork.Appointments.Add(appointment);
                TempData["SuccessMessage"] = "Appointment created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(appointmentDto);
        }

        public async Task<IActionResult> Edit(int id)
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
