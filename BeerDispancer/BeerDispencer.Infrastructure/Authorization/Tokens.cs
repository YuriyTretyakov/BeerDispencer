using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeerDispencer.Infrastructure.Authorization
{
    [Table("Tokens")]
    public class Token
	{
		[Key]
		public string TokenData { get; set; }
		public bool IsActive { get; set; }
		public DateTime UpdatedAt { get; set; }
	}
}

