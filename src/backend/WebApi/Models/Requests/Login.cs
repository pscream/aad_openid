using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Requests
{

    public class Login
    {

        [Required]
        [StringLength(250)]
        public string UserName { get; set; }

        [Required]
        [StringLength(250)]
        public string Password { get; set; }

    }

}
