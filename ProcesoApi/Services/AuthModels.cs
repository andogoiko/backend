using System.ComponentModel.DataAnnotations;
using BDD;

namespace ProcesoApi.Auth
{
    public class AuthRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class AuthResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public System.DateTime ValidTo { get; set; }



        // public AuthenticateResponse(User user, string token, System.DateTime validTo)
        // {
        //     Id = user.Id;
        //     FirstName = user.FirstName;
        //     LastName = user.LastName;
        //     Username = user.Username;
        //     Token = token;
        //     ValidTo = validTo;
        // }
    }
}