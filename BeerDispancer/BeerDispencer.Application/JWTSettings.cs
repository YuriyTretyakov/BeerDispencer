using System;
using BeerDispencer.Application.Abstractions;

namespace BeerDispencer.Application
{
    public class JWTSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }
    }
}

