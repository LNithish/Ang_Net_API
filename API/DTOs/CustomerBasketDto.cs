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
    }
}
