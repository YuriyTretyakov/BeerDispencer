using System;
namespace BeerDispenser.Infrastructure.Authorization
{
	public class LoginDBSettings
	{
		public string ConnectionString { get; set; }
		public bool UseInMemory { get; set; }
    }
}

