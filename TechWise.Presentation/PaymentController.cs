using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechWise.Services.Abstractions;

namespace TechWise.Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController(IServiceManager _serviceManager) : ControllerBase
    {
        [HttpPost("Stripe/webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> StripeWebhook()
        {
            // FIX: We must read the raw body exactly as Stripe sent it.

            // Because Stripe performs an HMAC signature on the raw bytes.

            // If we read it with a regular string, the encoding might change and the signature validation will fail.

            string json;
            using (var reader = new StreamReader(HttpContext.Request.Body))
            {
                json = await reader.ReadToEndAsync();
            }

            var stripeSignature = Request.Headers["Stripe-Signature"].ToString();

            if (string.IsNullOrEmpty(stripeSignature))
                return BadRequest("Missing Stripe-Signature header");

            try
            {
                await _serviceManager.OrderService
                    .HandleStripeWebhookAsync(json, stripeSignature);

                return Ok();
            }
            catch (Stripe.StripeException ex)
            {
                // Stripe will refund 400 if the signature is wrong
                return BadRequest($"Webhook error: {ex.Message}");
            }
        }
    }
}