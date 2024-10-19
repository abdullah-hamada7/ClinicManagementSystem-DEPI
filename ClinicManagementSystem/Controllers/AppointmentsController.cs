using ClinicManagementSystem.DTOs;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.Services;
using ClinicManagementSystem.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using Stripe;

namespace ClinicManagementSystem.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly StripeSettings _stripeSettings;


        public AppointmentsController(IUnitOfWork unitOfWork, IEmailService emailService,IOptions<StripeSettings> stripeSettings)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _stripeSettings = stripeSettings.Value;
        }
        [Authorize(Roles = "Admin,Doctor")]


        public async Task<IActionResult> Index()
        {
            var appointments = await _unitOfWork.Appointments.GetAll();
            return View(appointments);
        }
        [Authorize(Roles = "Doctor")]

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
        [Authorize(Roles = "Doctor,Patient")]

        public async Task<IActionResult> Create()
        {
            var patients = await _unitOfWork.Patients.GetAll();
            var doctors = await _unitOfWork.Doctors.GetAll();

            ViewBag.Patients = new SelectList(patients, "PatientID", "FirstName");
            ViewBag.Doctors = new SelectList(doctors, "DoctorID", "FirstName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Doctor,Patient")]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                try
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
                catch (Exception)
                {
                    ModelState.AddModelError("", "An error occurred while creating the appointment. Please try again.");
                }
            }

            var patients = await _unitOfWork.Patients.GetAll();
            var doctors = await _unitOfWork.Doctors.GetAll();

            ViewBag.Patients = new SelectList(patients, "PatientID", "FirstName");
            ViewBag.Doctors = new SelectList(doctors, "DoctorID", "FirstName");

            return View(appointment);
        }

        [Authorize(Roles = "Doctor")]

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
        [Authorize(Roles = "Doctor")]
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
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetById(id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }
        [Authorize(Roles = "Doctor")]
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
        public IActionResult CreateCheckoutSession(string amount)
        {
            if (string.IsNullOrEmpty(amount) || !int.TryParse(amount, out int parsedAmount) || parsedAmount <= 0)
            {
                return BadRequest("Invalid amount specified.");
            }

            var currency = "egp";
            var successUrl = Url.Action("Success", "Appointments", null, Request.Scheme);
            var cancelUrl = Url.Action("Cancel", "Appointments", null, Request.Scheme);
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = currency,
                            UnitAmount = parsedAmount * 100,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Bills",
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl
            };

            var service = new SessionService();
            var session = service.Create(options);

            return Redirect(session.Url);
        }

        public IActionResult Success()
        {
            return View("Index");
        }

        public IActionResult Cancel()
        {
            return View("Index");
        }
    }
}