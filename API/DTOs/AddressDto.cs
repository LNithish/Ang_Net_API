﻿using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class AddressDto
    {
        //To Validate user sending data DTO classes are better than entity classes
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Zipcode { get; set; }
    }
}
