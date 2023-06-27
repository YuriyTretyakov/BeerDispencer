using Beerdispancer.Domain.Implementations;
using BeerDispancer.Application.Abstractions;
using BeerDispancer.Application.DTO;
using BeerDispancer.Application.Implementation.Commands;
using BeerDispancer.Application.Implementation.Handlers;
using BeerDispencer.Application.Abstractions;
using BeerDispencer.Application.DTO;
using BeerDispencer.Domain.Abstractions;
using BeerDispencer.Domain.Implementations;
using BeerDispencer.Infrastructure.Persistence;
using BeerDispencer.Infrastructure.Persistence.Abstractions;
using BeerDispencer.Infrastructure.Persistence.Entities;
using FluentAssertions;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;

namespace BeerDispencer.UnitTests;

[TestFixture]
public class DispencerUnitTests
{
   
    [Test]
    public async Task CreateDispencer_ReturnsValid_DispencerObject_WithStateAndVolume()
    {
        //Arrange
        var mockSet = new Mock<IMongoCollection<Dispencer>>();
        var mockContext = new Mock<IBeerDispencerDbContext>();
        mockContext.Setup(x => x.Dispencers).Returns(mockSet.Object);

        var dispencerRepo = new DispencerRepository(mockContext.Object);

        var uof = new BeerDispencerUof(mockContext.Object,
            new Mock<IUsageRepository>().Object,
            dispencerRepo);

        var _sut = new CreateDispencerHandler(uof);
        var dispencerCommand = new DispencerCreateCommand { FlowVolume = 50 };

        //Act
        var dto = await _sut.Handle(dispencerCommand, CancellationToken.None);

        //Assert
        dto.Should().NotBeNull();
        dto.Volume.Should().Be(50);
        dto.Status.Should().Be(DispencerStatusDto.Close);

        mockSet.Verify(_ => _.InsertOneAsync(It.IsAny<Dispencer>(), null, CancellationToken.None), Times.Once);
    }

    [Test]
    public async Task UpdateDispencerCommand_SetProperState_When_DispencerOpenCommandReceived()
    {
        //Arrange
        var dispencerId = Guid.NewGuid().ToString();

        var dispencer = new DispencerDto
        {
            Id = dispencerId,
            Status = DispencerStatusDto.Close,
            Volume = 30
        };

        var dispencerRepoMock = new Mock<IDispencerRepository>();
        dispencerRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()).Result).Returns(dispencer);
        dispencerRepoMock.Setup(x => x.UpdateAsync(It.IsAny<DispencerDto>()));

        var usagesMock = new Mock<IUsageRepository>();

        var mockUof = new Mock<IDispencerUof>();
        mockUof.Setup(x => x.DispencerRepo).Returns(dispencerRepoMock.Object);
        mockUof.Setup(x => x.UsageRepo).Returns(usagesMock.Object);

        var _sut = new DispencerUpdateHandler(mockUof.Object, new Mock<IBeerFlowCalculator>().Object);
        var dispencerUpdateCommand = new DispencerUpdateCommand { Id = dispencerId, Status = DispencerStatusDto.Open, UpdatedAt = DateTime.UtcNow };

        //Act
        var dto = await _sut.Handle(dispencerUpdateCommand, CancellationToken.None);

        //Assert
        dto.Result.Should().BeTrue();

        dispencerRepoMock
            .Verify(x => x.GetByIdAsync(
                It.Is<string>(x => x == dispencerId)).Result,
            Times.Once);

        dispencerRepoMock
            .Verify(x => x
            .UpdateAsync(It.Is<DispencerUpdateDto>(
                x => x.Id == dispencerId &&
            x.Status == DispencerStatusDto.Open)),
            Times.Once);

        usagesMock
            .Verify(x => x.AddAsync(
                It.Is<UsageDto>(
                    x => x.DispencerId == dispencerId &&
                    x.OpenAt == dispencerUpdateCommand.UpdatedAt)));

    }

    [Test]
    public async Task UpdateDispencerCommand_SetProperState_When_DispencerCloseCommandReceived()
    {
        //Arrange
        var dispencerId = Guid.NewGuid().ToString();

        var dispencer = new DispencerDto
        {
            Id = dispencerId,
            Status = DispencerStatusDto.Open,
            Volume = 30
        };

        var dispencerRepoMock = new Mock<IDispencerRepository>();
        dispencerRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()).Result).Returns(dispencer);
        dispencerRepoMock.Setup(x => x.UpdateAsync(It.IsAny<DispencerDto>()));

        var usagesMock = new Mock<IUsageRepository>();
        var openedAt = DateTime.UtcNow.AddMinutes(-5);
        usagesMock
            .Setup(x =>
            x.GetByDispencerIdAsync(
                It.Is<string>(x => x == dispencerId)))
            .ReturnsAsync(new[] {new UsageDto {
                DispencerId = dispencerId,
                FlowVolume=dispencer.Volume,
                OpenAt = openedAt} });


        var mockUof = new Mock<IDispencerUof>();
        mockUof.Setup(x => x.DispencerRepo).Returns(dispencerRepoMock.Object);
        mockUof.Setup(x => x.UsageRepo).Returns(usagesMock.Object);


        var calculator = new Calculator(new BeerFlowSettings { LitersPerSecond = 0.1, PricePerLiter = 6 });

        var _sut = new DispencerUpdateHandler(mockUof.Object, calculator);
        var dispencerUpdateCommand = new DispencerUpdateCommand
        {
            Id = dispencerId,
            Status = DispencerStatusDto.Close,
            UpdatedAt = DateTime.UtcNow
        };

        //Act
        var dto = await _sut.Handle(dispencerUpdateCommand, CancellationToken.None);

        //Assert
        dto.Result.Should().BeTrue();

      
        dispencerRepoMock
            .Verify(x => x.GetByIdAsync(
                It.Is<string>(x => x == dispencerId)).Result,
            Times.Once);


        dispencerRepoMock
            .Verify(x => x
            .UpdateAsync(It.Is<DispencerUpdateDto>(
                x => x.Id == dispencerId &&
            x.Status == DispencerStatusDto.Close)),
            Times.Once);

        usagesMock
            .Verify(x => x.GetByDispencerIdAsync(It.Is<string>(x => x == dispencerId)).Result);


        usagesMock.Verify(x => x
        .UpdateAsync(It.Is<UsageDto>(
            x => x.DispencerId == dispencerId &&
            x.ClosedAt == dispencerUpdateCommand.UpdatedAt &&
            x.OpenAt == openedAt &&
            x.FlowVolume == x.ClosedAt.Value.Subtract(x.OpenAt.Value).TotalSeconds * 0.1 &&
            x.TotalSpent == x.FlowVolume * 6)));
    }


    [TestCase(DispencerStatusDto.Open, DispencerStatusDto.Open)]
    [TestCase(DispencerStatusDto.Close, DispencerStatusDto.Close)]
    public async Task UpdateDispencerCommand_Status_Is_False_If_Operation_CannotBePErformed(DispencerStatusDto initialstate, DispencerStatusDto updateTo)
    {
        //Arrange
        var dispencerId = Guid.NewGuid().ToString();

        var dispencer = new DispencerDto
        {
            Id = dispencerId,
            Status = initialstate,
            Volume = 30
        };


        var dispencerRepoMock = new Mock<IDispencerRepository>();
        dispencerRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()).Result).Returns(dispencer);
        dispencerRepoMock.Setup(x => x.UpdateAsync(It.IsAny<DispencerDto>()));

        var usagesMock = new Mock<IUsageRepository>();
        var openedAt = DateTime.UtcNow.AddMinutes(-5);
        usagesMock
            .Setup(x =>
            x.GetByDispencerIdAsync(
                It.Is<string>(x => x == dispencerId)))
            .ReturnsAsync(new[] {new UsageDto {
                DispencerId = dispencerId,
                FlowVolume=dispencer.Volume,
                OpenAt = openedAt} });


        var mockUof = new Mock<IDispencerUof>();
        mockUof.Setup(x => x.DispencerRepo).Returns(dispencerRepoMock.Object);
        mockUof.Setup(x => x.UsageRepo).Returns(usagesMock.Object);

        var calculator = new Calculator(new BeerFlowSettings { LitersPerSecond = 0.1, PricePerLiter = 6 });
        var _sut = new DispencerUpdateHandler(mockUof.Object, calculator);

        //Act
        var dispencerUpdateCommand = new DispencerUpdateCommand
        {
            Id = dispencerId,
            Status = updateTo,
            UpdatedAt = DateTime.UtcNow
        };

        var dto = await _sut.Handle(dispencerUpdateCommand, CancellationToken.None);
        //Assert
        dto.Result.Should().BeFalse();
    }

    [Test]
    public async Task UpdateDispencerCommand_ProvidingInvalidId_ReturnsFalse()
    {
        //Arrange
        var dispencerRepoMock = new Mock<IDispencerRepository>();
        dispencerRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()).Result).Returns(default(DispencerDto));
      
        var mockUof = new Mock<IDispencerUof>();
        mockUof.Setup(x => x.DispencerRepo).Returns(dispencerRepoMock.Object);
       
        var _sut = new DispencerUpdateHandler(mockUof.Object, new Mock<IBeerFlowCalculator>().Object);
        //Act
        var dispencerUpdateCommand = new DispencerUpdateCommand
        {
            Id = Guid.NewGuid().ToString(),
            Status = DispencerStatusDto.Open,
            UpdatedAt = DateTime.UtcNow
        };
        var dto = await _sut.Handle(dispencerUpdateCommand, CancellationToken.None);
        //Assert
        dto.Result.Should().BeFalse();
    }
}