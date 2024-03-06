using System;
namespace BeerDispenser.Shared
{
    public class AuthResponseDto
    {
        public bool IsSuccess { get; set; }
        public object Data { get; set; }
        public AuthDetails[] ProblemDetails { get; set; }

        public static AuthResponseDto CreateProblemDetails(params string[] messages)
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                ProblemDetails = messages.Select(x =>
                    {
                        return new AuthDetails
                        {
                            Code = "unknown",
                            Description = x
                        };
                    }).ToArray()
            };
        }
    }

    public class AuthDetails
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
}

