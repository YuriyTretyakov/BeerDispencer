using BeerDispenser.Shared;
using MediatR;
using Newtonsoft.Json;

namespace BeerDispenser.Application.Implementation.Commands.Authorization
{
    public class UserLoginCommand: IRequest<AuthResponseDto>
    {
		[JsonProperty("userName")]
		public string UserName { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
	}
}

