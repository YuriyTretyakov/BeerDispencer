using System;

namespace BeerDispenser.Shared
{
    public class UserCredentials
    {
        public string UserName { get; set; }
        public UserRoles Role { get; set; }
        public string Password { get; set; }
    }
}

