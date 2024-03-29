﻿using System;
using BeerDispenser.Application.Implementation.Commands.Authorization;
using BeerDispenser.Application.Implementation.Response;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BeerDispenser.Application.Implementation.Handlers.Authorization
{
	public class CreateUserHandler:IRequestHandler<CreateUserCommand, AuthResponseDto>
	{
        private readonly UserManager<IdentityUser> _userManager;

        public CreateUserHandler(UserManager<IdentityUser> userManager)
		{
            _userManager = userManager;
        }

        public async Task<AuthResponseDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var response = new AuthResponseDto();
            var user = new IdentityUser { UserName = request.UserName };

            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                response = result.Errors.ToAuthResponseDto();
                return response;

            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, request.Password);


            if (!addPasswordResult.Succeeded)
            {
                response = addPasswordResult.Errors.ToAuthResponseDto();
                return response;
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(user, request.Role.ToString());

            if (!addToRoleResult.Succeeded)
            {
                response = addToRoleResult.Errors.ToAuthResponseDto();
                return response;
            }

            response.IsSuccess = true;
            response.Data = user.Id;
            return response;
        }
    }
}

