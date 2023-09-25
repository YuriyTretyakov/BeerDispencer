using System;
namespace BeerDispenser.Infrastructure.Settings
{
	public class DBSettings
	{
		public string ConnectionString { get; set; }
		public string DbName  { get; set; }
        public string SpecialConnectionString { get; set; }
    }
}

