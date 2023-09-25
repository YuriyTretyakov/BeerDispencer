using BeerDispenser.Application.Implementation.Queries;
using BeerDispenser.Application.Implementation.Response;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BeerDispenser.Application.Implementation.Handlers.Authorization
{
    public class GetAllUserHandler : IRequestHandler<GetAllUsersQuery, User[]>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public GetAllUserHandler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<User[]> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userManager.Users.ToListAsync();

            List<User> userWithRoles = new List<User>();

            foreach (var user in users)
            {
                userWithRoles.Add(new User
                {

                    UserName = user.UserName,
                    Role = (await _userManager.GetRolesAsync(user)).ToArray()
                });

                
            }
            return userWithRoles.ToArray();
        }
    }
}

