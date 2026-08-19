using EduTrack.Application.Features.Feedbacks.Commands.SubmitFeedback;
using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EduTrack.Application.UnitTests.Features.Feedbacks.Commands;

public class SubmitFeedbackCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IFeedbackRepository> _mockFeedbackRepository;
    private readonly Mock<ILogger<SubmitFeedbackCommandHandler>> _mockLogger;
    private readonly SubmitFeedbackCommandHandler _handler;

    public SubmitFeedbackCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockFeedbackRepository = new Mock<IFeedbackRepository>();
        _mockLogger = new Mock<ILogger<SubmitFeedbackCommandHandler>>();
        _mockUnitOfWork.Setup(x => x.Feedbacks).Returns(_mockFeedbackRepository.Object);
        _handler = new SubmitFeedbackCommandHandler(_mockUnitOfWork.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ValidCommandWithName_ShouldCreateFeedbackAndReturnId()
    {
        // Arrange
        var command = new SubmitFeedbackCommand("Great application!", "John Doe");
        _mockFeedbackRepository
            .Setup(x => x.AddAsync(It.IsAny<Feedback>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _mockFeedbackRepository.Verify(x => x.AddAsync(It.IsAny<Feedback>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_AnonymousCommand_ShouldCreateFeedbackWithNullName()
    {
        // Arrange
        var command = new SubmitFeedbackCommand("Anonymous feedback");
        Feedback? capturedFeedback = null;
        _mockFeedbackRepository
            .Setup(x => x.AddAsync(It.IsAny<Feedback>(), It.IsAny<CancellationToken>()))
            .Callback<Feedback, CancellationToken>((f, _) => capturedFeedback = f)
            .Returns(Task.CompletedTask);
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedFeedback);
        Assert.Null(capturedFeedback!.Name);
        Assert.Equal("Anonymous feedback", capturedFeedback.Message);
        Assert.False(capturedFeedback.IsRead);
    }

    [Fact]
    public async Task Handle_ValidCommandWithName_ShouldSetCorrectProperties()
    {
        // Arrange
        var command = new SubmitFeedbackCommand("Very helpful!", "Jane Smith");
        Feedback? capturedFeedback = null;
        _mockFeedbackRepository
            .Setup(x => x.AddAsync(It.IsAny<Feedback>(), It.IsAny<CancellationToken>()))
            .Callback<Feedback, CancellationToken>((f, _) => capturedFeedback = f)
            .Returns(Task.CompletedTask);
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedFeedback);
        Assert.Equal("Very helpful!", capturedFeedback!.Message);
        Assert.Equal("Jane Smith", capturedFeedback.Name);
        Assert.False(capturedFeedback.IsRead);
        Assert.Null(capturedFeedback.ReadAt);
    }
}
