using System;

namespace BeerDispenser.Shared.Dto
{
    public class UserCredentialsDto
    {
        public string UserName { get; set; }
        public UserRolesDto Role { get; set; }
        public string Password { get; set; }
    }
}

