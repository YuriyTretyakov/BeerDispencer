using System;
using BeerDispencer.Application.Abstractions;

namespace BeerDispancer.Application.Implementation
{
    public class JWTSettings : IJWTSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }
    }
}

