using System;
using BeerDispancer.Application.DTO;
using BeerDispencer.Application.Implementation.Response;
using MediatR;
using Newtonsoft.Json;

namespace BeerDispancer.Application.Implementation.Commands.Authorization
{
	public class UserLoginCommand: IRequest<AuthResponseDto>
    {
		[JsonProperty("userName")]
		public string UserName { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
	}
}

