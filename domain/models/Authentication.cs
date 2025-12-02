using System.Security.Claims;

namespace domain.models
{
    public class Authentication
    {
        public string User { get; set; }

        public string Password { get; set; }
    }
}