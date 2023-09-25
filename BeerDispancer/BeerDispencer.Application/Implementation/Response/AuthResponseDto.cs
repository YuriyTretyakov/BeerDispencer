using System;
namespace BeerDispenser.Application.Implementation.Response
{
	public class AuthResponseDto
	{
		public bool IsSuccess { get; set; }
		public object Data { get; set; }
        public AuthDetails[] ProblemDetails { get; set; }
    }

	public class AuthDetails
    {
		public string Code { get; set; }
        public string Description { get; set; }
	}
}

