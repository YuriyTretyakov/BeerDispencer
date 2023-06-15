using BeerDispancer.Application.Abstractions;
using BeerDispancer.Application.Implementation.Commands;
using BeerDispancer.Application.Implementation.Handlers;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace BeerDispencer.UnitTests;

[TestFixture]
public class DispencerUnitTests
{
    private readonly CreateDispencerHandler _sut;
    private readonly IDispencerUof _dispencerUof = Substitute.For<IDispencerUof>();

    public DispencerUnitTests()
    {
        _sut = new CreateDispencerHandler(_dispencerUof);
    }


    [Test]
    public async Task CreateDispencer_ReturnsValid_DispencerObject()
    {
        //Arrange
        var dispencerCommand = new DispencerCreateCommand { FlowVolume = 13 };

        //Act
        var dto = await _sut.Handle(dispencerCommand, CancellationToken.None);

        //Assert
        dto.Should().NotBeNull();
        dto.Volume.Should().Be(13);
        dto.Status.Should().Be(BeerDispancer.Application.DTO.DispencerStatusDto.Close);
        dto.Id.Should().Be(Arg.Any<Guid>());
    }
}

