using faka.Data;
using faka.Filters;
using faka.Hubs;
using faka.Models;
using faka.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Stripe;
using Stripe.Checkout;

namespace faka.Controllers;


[CustomResultFilter(Enabled = false)]
[Route("/api/[controller]")]
[ApiController]
public class WebhookController : Controller
{
    private readonly OrderService _orderService;
    private readonly fakaContext _context;
    private readonly IConfiguration _configuration;
    private readonly IHubContext<PaymentHub> _hubContext;

    public WebhookController(IConfiguration configuration, IHubContext<PaymentHub> hubContext, fakaContext context, OrderService orderService)
    {
        _configuration = configuration;
        _hubContext = hubContext;
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