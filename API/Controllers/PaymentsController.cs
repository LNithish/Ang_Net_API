using API.Errors;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace API.Controllers
{
    
    public class PaymentsController : BaseApiController
    {
        //below secret is from stripe CLI
        private const string WebhookSecret = "whsec_c910f11d9ced0ebc14a4bfd6e85162867b1e4674bbf911eea7361fe7d5ac7697";
        private readonly IPaymentService paymentService;
        private readonly ILogger<PaymentsController> logger;

        public PaymentsController(IPaymentService paymentService,ILogger<PaymentsController> logger)
        {
            this.paymentService = paymentService;
            this.logger = logger;
        }
        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket= await paymentService.CreateOrUpdatePaymentIntent(basketId);
            if (basket == null)
            {
                return BadRequest(new ApiResponse(400, "Problem with your basket"));
            }
            return basket;
        }

        //Webhook from stripe to update payment status for customer
        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebhook()
        {
            //stripe will send a request of event in a request body
            var json=await new StreamReader(Request.Body).ReadToEndAsync();
            //getting access to stripe event
            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"],WebhookSecret);
            PaymentIntent intent;
            Order order;

            switch(stripeEvent.Type) 
            {
                case "payment_intent.succeeded":
                    intent=(PaymentIntent)stripeEvent.Data.Object;
                    logger.LogInformation("Payment succeeded: ", intent.Id);
                    //Updating order with status
                    order = await paymentService.UpdateOrderPaymentSucceeded(intent.Id);
                    logger.LogInformation("Order updated to payment received: ", order.Id);
                    break;
                case "payment_intent.payment_failed":
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    logger.LogInformation("Payment failed: ", intent.Id);
                    //Updating order with status
                    order = await paymentService.UpdateOrderPaymentFailed(intent.Id);
                    logger.LogInformation("Order updated to payment failed: ", order.Id);
                    break;
            }
            //return acknowledgment to stripe
            return new EmptyResult();
        }
    }
}
