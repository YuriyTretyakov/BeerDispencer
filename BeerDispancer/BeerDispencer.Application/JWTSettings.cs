using System;
using BeerDispenser.Application.Abstractions;

namespace BeerDispenser.Application
{
    public class JWTSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }
    }
}

