﻿using BeerDispancer.Application.Abstractions;
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
using BeerDispencer.Shared;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Stripe.Tax;

namespace BeerDispencer.UnitTests;

[TestFixture]
public class DispencerUnitTests
{
    public DispencerUnitTests()
    {
    }


    [Test]
    public async Task CreateDispencer_ReturnsValid_DispencerObject_WithStatisAndVolume()
    {
        //Arrange
        var mockSet = new Mock<DbSet<Dispenser>>();


        var mockContext = new Mock<IBeerDispencerDbContext>();

        mockContext.Setup(x => x.Dispencers).Returns(mockSet.Object);


        var dispencerRepo = new DispenserRepository(mockContext.Object);


        var uof = new BeerDispencerUof(mockContext.Object,
            new Mock<IUsageRepository>().Object,
            dispencerRepo);

        var beerSettings = new BeerFlowSettings { LitersPerSecond = 0.1M, PricePerLiter = 6 };

        var _sut = new CreateDispenserHandler(uof, beerSettings);

        var dispencerCommand = new DispenserCreateCommand { FlowVolume = 50 };

        //Act
        var dto = await _sut.Handle(dispencerCommand, CancellationToken.None);

        //Assert
        dto.Should().NotBeNull();
        dto.Volume.Should().Be(50);
        dto.Status.Should().Be(DispenserStatus.Closed);


        mockSet.Verify(_ => _.AddAsync(It.IsAny<Dispenser>(), CancellationToken.None), Times.Once);
        mockContext.Verify(x => x.SaveChangesAsync(CancellationToken.None).Result, Times.Once);
    }

    [Test]
    public async Task UpdateDispencerCommand_SetProperState_When_DispencerOpenCommandReceived()
    {
        //Arrange
        var dispencerId = Guid.NewGuid();

        var dispencer = new DispenserDto
        {
            Id = dispencerId,
            Status = DispenserStatus.Closed,
            Volume = 30
        };


        var dispencerRepoMock = new Mock<IDispencerRepository>();
        dispencerRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()).Result).Returns(dispencer);
        dispencerRepoMock.Setup(x => x.UpdateAsync(It.IsAny<DispenserDto>()));

        var usagesMock = new Mock<IUsageRepository>();

        var mockUof = new Mock<IDispencerUof>();
        mockUof.Setup(x => x.DispencerRepo).Returns(dispencerRepoMock.Object);
        mockUof.Setup(x => x.UsageRepo).Returns(usagesMock.Object);

        var beerSettings = new BeerFlowSettings { LitersPerSecond = 0.1M, PricePerLiter = 6 };


        var _sut = new DispenserUpdateHandler(mockUof.Object,
            beerSettings);

        var dispenserUpdateCommand = new DispenserUpdateCommand { Id = dispencerId, Status = DispenserStatus.Opened, UpdatedAt = DateTime.UtcNow };

        //Act
        var dto = await _sut.Handle(dispenserUpdateCommand, CancellationToken.None);

        //Assert
        dto.Result.Should().BeTrue();

        mockUof.Verify(x => x.StartTransaction(), Times.Once);
        dispencerRepoMock
            .Verify(x => x.GetByIdAsync(
                It.Is<Guid>(x => x == dispencerId)).Result,
            Times.Once);

        dispencerRepoMock
            .Verify(x => x
            .UpdateAsync(It.Is<DispenserUpdateDto>(
                x => x.Id == dispencerId &&
            x.Status == DispenserStatus.Opened)),
            Times.Once);

        usagesMock
            .Verify(x => x.AddAsync(
                It.Is<UsageDto>(
                    x => x.DispencerId == dispencerId &&
                    x.OpenAt == dispenserUpdateCommand.UpdatedAt)));

        mockUof.Verify(x => x.Complete(), Times.Once);
        mockUof.Verify(x => x.CommitTransaction(), Times.Once);

    }

    [Test]
    public async Task UpdateDispencerCommand_SetProperState_When_DispencerCloseCommandReceived()
    {
        //Arrange
        var dispencerId = Guid.NewGuid();

        var dispencer = new DispenserDto
        {
            Id = dispencerId,
            Status = DispenserStatus.Opened,
            Volume = 30
        };


        var dispencerRepoMock = new Mock<IDispencerRepository>();
        dispencerRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()).Result).Returns(dispencer);
        dispencerRepoMock.Setup(x => x.UpdateAsync(It.IsAny<DispenserDto>()));

        var usagesMock = new Mock<IUsageRepository>();
        var openedAt = DateTime.UtcNow.AddMinutes(-5);
        usagesMock
            .Setup(x =>
            x.GetByDispencerIdAsync(
                It.Is<Guid>(x => x == dispencerId)))
            .ReturnsAsync(new[] {new UsageDto {
                DispencerId = dispencerId,
                FlowVolume=dispencer.Volume,
                OpenAt = openedAt} });


        var mockUof = new Mock<IDispencerUof>();
        mockUof.Setup(x => x.DispencerRepo).Returns(dispencerRepoMock.Object);
        mockUof.Setup(x => x.UsageRepo).Returns(usagesMock.Object);


        var beerSettings = new BeerFlowSettings { LitersPerSecond = 0.1M, PricePerLiter = 6 };

        var _sut = new DispenserUpdateHandler(
            mockUof.Object,
            beerSettings);

        var dispenserUpdateCommand = new DispenserUpdateCommand
        {
            Id = dispencerId,
            Status = DispenserStatus.Closed,
            UpdatedAt = DateTime.UtcNow
        };

        //Act
        var dto = await _sut.Handle(dispenserUpdateCommand, CancellationToken.None);

        //Assert
        dto.Result.Should().BeTrue();

        mockUof.Verify(x => x.StartTransaction(), Times.Once);
        dispencerRepoMock
            .Verify(x => x.GetByIdAsync(
                It.Is<Guid>(x => x == dispencerId)).Result,
            Times.Once);


        dispencerRepoMock
            .Verify(x => x
            .UpdateAsync(It.Is<DispenserUpdateDto>(
                x => x.Id == dispencerId &&
            x.Status == DispenserStatus.Closed)),
            Times.Once);

        usagesMock
            .Verify(x => x.GetByDispencerIdAsync(It.Is<Guid>(x => x == dispencerId)).Result);


        usagesMock.Verify(x => x
        .UpdateAsync(It.Is<UsageDto>(
            x => x.DispencerId == dispencerId &&
            x.ClosedAt == dispenserUpdateCommand.UpdatedAt &&
            x.OpenAt == openedAt &&
            x.FlowVolume == (decimal)x.ClosedAt.Value.Subtract(x.OpenAt).TotalSeconds * beerSettings.LitersPerSecond &&
            x.TotalSpent == x.FlowVolume * beerSettings.PricePerLiter)));


        mockUof.Verify(x => x.Complete(), Times.Once);
        mockUof.Verify(x => x.CommitTransaction(), Times.Once);

    }


    [TestCase(DispenserStatus.Opened, DispenserStatus.Opened)]
    [TestCase(DispenserStatus.Closed, DispenserStatus.Closed)]
    public async Task UpdateDispencerCommand_Status_Is_False_If_Operation_CannotBePErformed(DispenserStatus initialstate, DispenserStatus updateTo)
    {
        //Arrange
        var dispencerId = Guid.NewGuid();

        var dispencer = new DispenserDto
        {
            Id = dispencerId,
            Status = initialstate,
            Volume = 30
        };


        var dispencerRepoMock = new Mock<IDispencerRepository>();
        dispencerRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()).Result).Returns(dispencer);
        dispencerRepoMock.Setup(x => x.UpdateAsync(It.IsAny<DispenserDto>()));

        var usagesMock = new Mock<IUsageRepository>();
        var openedAt = DateTime.UtcNow.AddMinutes(-5);
        usagesMock
            .Setup(x =>
            x.GetByDispencerIdAsync(
                It.Is<Guid>(x => x == dispencerId)))
            .ReturnsAsync(new[] {new UsageDto {
                DispencerId = dispencerId,
                FlowVolume=dispencer.Volume,
                OpenAt = openedAt} });


        var mockUof = new Mock<IDispencerUof>();
        mockUof.Setup(x => x.DispencerRepo).Returns(dispencerRepoMock.Object);
        mockUof.Setup(x => x.UsageRepo).Returns(usagesMock.Object);

        var beerSettings = new BeerFlowSettings { LitersPerSecond = 0.1M, PricePerLiter = 6 };

        var _sut = new DispenserUpdateHandler(mockUof.Object,
            beerSettings);

        //Act
        var dispenserUpdateCommand = new DispenserUpdateCommand
        {
            Id = dispencerId,
            Status = updateTo,
            UpdatedAt = DateTime.UtcNow
        };

        var dto = await _sut.Handle(dispenserUpdateCommand, CancellationToken.None);
        //Assert
        dto.Result.Should().BeFalse();
    }

    [Test]
    public async Task UpdateDispencerCommand_ProvidingInvalidId_ReturnsFalse()
    {
        //Arrange
        var dispencerRepoMock = new Mock<IDispencerRepository>();
        dispencerRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()).Result).Returns(default(DispenserDto));

        var mockUof = new Mock<IDispencerUof>();
        mockUof.Setup(x => x.DispencerRepo).Returns(dispencerRepoMock.Object);

        var _sut = new DispenserUpdateHandler(mockUof.Object,
            new Mock<IBeerFlowSettings>().Object);
        //Act
        var dispenserUpdateCommand = new DispenserUpdateCommand
        {
            Id = Guid.NewGuid(),
            Status = DispenserStatus.Opened,
            UpdatedAt = DateTime.UtcNow
        };
        var dto = await _sut.Handle(dispenserUpdateCommand, CancellationToken.None);
        //Assert
        dto.Result.Should().BeFalse();
    }
}