using BeerDispenser.Application.Implementation.Queries;
using BeerDispenser.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BeerDispenser.Application.Implementation.Handlers.Authorization
{
    public class GetAllUserHandler : IRequestHandler<GetAllUsersQuery, UserCredentials[]>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public GetAllUserHandler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserCredentials[]> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userManager.Users.ToListAsync();

            List<UserCredentials> userWithRoles = new List<UserCredentials>();

            foreach (var user in users)
            {
                var roleStr = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

                userWithRoles.Add(new UserCredentials
                {

                    UserName = user.UserName,
                    Role = Enum.Parse<UserRoles>(roleStr)
                }); ;

                
            }
            return userWithRoles.ToArray();
        }
    }
}

