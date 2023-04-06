using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        //Validations for client sending data
        [Required]
        public string DisplayName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression("(?=^.{6,10}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt" +
            ";.&lt;,])(?!.*\\s).*$",ErrorMessage ="Password must have one uppercase,lowercase,number and " +
            "non alphanumeric and atlase 6 characters")]
        public string Password { get; set; }
    }
}
