using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BeerDispancer.DomainLayer;

namespace BeerDispancer.DataLayer.Models
{
	[Table("Dispencer")]
	public class Dispencer
	{
		[Key]
		public Guid Id { get; set; }
		public double Volume { get; set; }
        public DispencerStatusDto Status { get; set; }
    }
}

