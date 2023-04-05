using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Identity
{
    public class Address
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }

        //Configuring 1 to 1 relationship with AppUser and Address
        //AppUserId will be foreignkey, it should not be nullable
        [Required]
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}