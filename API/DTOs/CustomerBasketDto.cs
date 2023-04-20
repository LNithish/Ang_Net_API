using Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    //Created CustomerBasketDto to apply validations for client passed data
    public class CustomerBasketDto
    {
        [Required]
        public string Id { get; set; }

        public List<BasketItemDto> Items { get; set; }

        //additional properties for payment module
        //making it optional using ? so while adding these item in basket user don't need to supply these properties
        public int DeliveryMethodId { get; set; }
        public string ClientSecret { get; set; }//used by Stripe so the user can confirm payment itent
        public string PaymentIntentId { get; set; } //to update payment intent created by client

        public decimal ShippingPrice { get; set; }

    }
}
