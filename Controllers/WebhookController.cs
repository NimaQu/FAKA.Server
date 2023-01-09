using Microsoft.AspNetCore.Mvc;
using faka.Filters;
using faka.Hubs;
using Microsoft.AspNetCore.SignalR;
using Stripe;

namespace faka.Controllers;

[Route("/api/[controller]")]
[ApiController]
[CustomResultFilter(Enabled = false)]
public class WebhookController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly IHubContext<PaymentHub> _hubContext;

    public WebhookController(IConfiguration configuration, IHubContext<PaymentHub> hubContext)
    {
        _hubContext = hubContext;
        _configuration = configuration;
    }
    
    [HttpPost("stripe")]
    public async Task<IActionResult> Stripe()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var webhookSecret = _configuration["PaymentGateways:Stripe:WebhookSecret"];
        if (webhookSecret is null)
        {
            throw new Exception("Stripe Webhook Secret is not set");
        }

        try
        {
            var stripeEvent = EventUtility.ParseEvent(json);
            var signatureHeader = Request.Headers["Stripe-Signature"];

            stripeEvent = EventUtility.ConstructEvent(json,
                signatureHeader, webhookSecret);

            switch (stripeEvent.Type)
            {
                case Events.PaymentIntentSucceeded:
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    Console.WriteLine("A successful payment for {0} was made.", paymentIntent.Amount);
                    // Then define and call a method to handle the successful payment intent.
                    // handlePaymentIntentSucceeded(paymentIntent);
                    break;
                }
                case Events.PaymentMethodAttached:
                {
                    var paymentMethod = stripeEvent.Data.Object as PaymentMethod;
                    // Then define and call a method to handle the successful attachment of a PaymentMethod.
                    // handlePaymentMethodAttached(paymentMethod);
                    break;
                }
                default:
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                    break;
            }
            return Ok();
        }
        catch (StripeException e)
        {
            Console.WriteLine("Error: {0}", e.Message);
            return BadRequest();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e.Message);
            return StatusCode(500);
        }
    }
}