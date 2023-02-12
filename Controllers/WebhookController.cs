using FAKA.Server.Data;
using FAKA.Server.Filters;
using FAKA.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace FAKA.Server.Controllers;


[CustomResultFilter(Enabled = false)]
[Route("/api/[controller]")]
[ApiController]
public class WebhookController : Controller
{
    private readonly OrderService _orderService;
    private readonly FakaContext _context;
    private readonly IConfiguration _configuration;

    public WebhookController(IConfiguration configuration, FakaContext context, OrderService orderService)
    {
        _configuration = configuration;
        _context = context;
        _orderService = orderService;
    }

    [HttpPost("stripe")]
    public async Task<IActionResult> Stripe()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var webhookSecret = _configuration["PaymentGateways:Stripe:WebhookSecret"];
        if (webhookSecret is null) throw new Exception("Stripe Webhook Secret is not set");

        try
        {
            var stripeEvent = EventUtility.ParseEvent(json);
            var signatureHeader = Request.Headers["Stripe-Signature"];

            stripeEvent = EventUtility.ConstructEvent(json,
                signatureHeader, webhookSecret);

            switch (stripeEvent.Type)
            {
                case Events.CheckoutSessionCompleted:
                {
                    if (stripeEvent.Data.Object is not Session session) throw new Exception("Stripe callback Session is null");
                    await _orderService.FulfillOrderAsync(session.Id);
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
    
    [HttpPost("alipay")]
    public async Task<IActionResult> Alipay()
    {
        // todo alipay webhook
        return Ok();
    }
}