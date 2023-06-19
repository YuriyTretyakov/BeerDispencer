using System;
using BeerDispencer.Application;
using BeerDispencer.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace BeerDispencer.Infrastructure.Authorization
{
    public class TokenManager : ITokenManager
    {
        private readonly ILoginDbContext _loginDbContext;
        IHttpContextAccessor _httpContextAccessor;



        public TokenManager(ILoginDbContext loginDbContext,
                IHttpContextAccessor httpContextAccessor,
                IOptions<JWTSettings> jwtOptions
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _loginDbContext = loginDbContext;
        }

        public async Task<bool> IsCurrentActiveToken()
            => await IsActiveAsync(GetCurrentAsync());

        public async Task DeactivateCurrentAsync()
            => await DeactivateAsync(GetCurrentAsync());

        public async Task<bool> IsActiveAsync(string token)
        {
            var dbToken = await _loginDbContext
                .Tokens
                .SingleOrDefaultAsync(x => x.TokenData == token);

            return dbToken?.IsActive is null ? true : dbToken.IsActive;
        }
          

        public async Task DeactivateAsync(string token)
        {
            var dbToken = await _loginDbContext
                .Tokens
                .SingleOrDefaultAsync(x => x.TokenData == token);

            if (dbToken is null)
            {
                await _loginDbContext.Tokens.AddAsync(new Token { TokenData = token, IsActive = false, UpdatedAt = DateTime.UtcNow });
                await _loginDbContext.SaveChangesAsync(CancellationToken.None);
             }
        }
           

        private string GetCurrentAsync()
        {
            var authorizationHeader = _httpContextAccessor
                .HttpContext.Request.Headers["authorization"];

            return authorizationHeader == StringValues.Empty
                ? string.Empty
                : authorizationHeader.Single().Split(" ").Last();
        }
    }
}


