using System;
using BeerDispancer.Application.DTO;
using MediatR;

namespace BeerDispancer.Application.Implementation.Commands
{
	public class GetAllDispencersQuery : IRequest<DispencerDto[]>
    {
	}
}

