using System.ComponentModel.DataAnnotations;

namespace webapp.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "password must be 4 - 8")]
        public string Password { get; set; }
    }
}