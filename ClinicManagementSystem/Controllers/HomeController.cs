using ClinicManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using Stripe;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly StripeSettings _stripeSettings;
        private readonly IEmailService _emailService;

        public HomeController(IEmailService emailService, IOptions<StripeSettings> stripeSettings)
        {
            _emailService = emailService;
            _stripeSettings = stripeSettings.Value;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult SendEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmail(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _emailService.SendEmailAsync(model.To, model.Subject, model.Body);
                    TempData["SuccessMessage"] = "Email sent successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error sending email: {ex.Message}");
                }
            }

            return View(model);
        }
    public IActionResult CreateCheckoutSession(string amount)
        {
            if (string.IsNullOrEmpty(amount) || !int.TryParse(amount, out int parsedAmount) || parsedAmount <= 0)
            {
                return BadRequest("Invalid amount specified.");
            }

            var currency = "egp";
            var successUrl = Url.Action("Success", "Home", null, Request.Scheme);
            var cancelUrl = Url.Action("Cancel", "Home", null, Request.Scheme);
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
