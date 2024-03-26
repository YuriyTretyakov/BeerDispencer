using System;
namespace BeerDispenser.Infrastructure.Settings
{
	public class DBSettings
	{
		public string ConnectionString { get; set; }
        public bool UseInMemory { get; set; }
    }
}

