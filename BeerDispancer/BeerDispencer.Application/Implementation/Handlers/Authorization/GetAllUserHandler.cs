using BeerDispenser.Application.Implementation.Queries;
using BeerDispenser.Shared.Dto;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BeerDispenser.Application.Implementation.Handlers.Authorization
{
    public class GetAllUserHandler : IRequestHandler<GetAllUsersQuery, UserCredentialsDto[]>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public GetAllUserHandler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserCredentialsDto[]> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userManager.Users.ToListAsync();

            List<UserCredentialsDto> userWithRoles = new List<UserCredentialsDto>();

            foreach (var user in users)
            {
                var roleStr = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

                userWithRoles.Add(new UserCredentialsDto
                {

                    UserName = user.UserName,
                    Role = Enum.Parse<UserRolesDto>(roleStr)
                }); ;

                
            }
            return userWithRoles.ToArray();
        }
    }
}

