using System;
namespace BeerDispenser.Infrastructure.Authorization
{
	public class LoginDBSettings
	{
		public string ConnectionString { get; set; }
		public string DbName  { get; set; }
        public string SpecialConnectionString { get; set; }
    }
}

