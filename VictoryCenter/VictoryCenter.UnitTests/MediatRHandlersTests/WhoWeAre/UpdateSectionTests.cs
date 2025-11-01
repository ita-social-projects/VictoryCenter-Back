using AutoMapper;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.Admin.WhoWeAre.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreContent;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreSection;
using VictoryCenter.BLL.Interfaces.WhoWeAreContentFactory;
using VictoryCenter.BLL.Validators.WhoWeAreSections;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.WhoWeAreContents;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.WhoWeAre;

public class UpdateWhoWeAreContentTests
{
    private readonly Mock<IWhoWeAreContentFactory> _mockFactory;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly IValidator<UpdateWhoWeAreContentCommand> _validator;

    private readonly WhoWeAreSection _testSection = new()
    {
        Id = 1,
        SectionType = SectionType.Main,
        Title = "Основне",
        Contents = null!
    };

    private readonly DescriptionContent _testDescriptionContent = new()
    {
        Id = 1,
        SectionId = 1,
        ContentType = ContentType.Description,
        Description = "Description"
    };

    private readonly CardContent _testCardContent = new()
    {
        Id = 1,
        SectionId = 1,
        ImageId = 1,
        ContentType = ContentType.Card,
        Description = "Description 1"
    };

    private readonly Image _testImage = new()
    {
        Id = 1,
        CreatedAt = DateTimeOffset.Now,
        Url = "test.png",
        BlobName = "testBlobName",
        MimeType = "image/png"
    };

    private readonly WhoWeAreSectionDto _testSectionDto = new()
    {
        Title = "Основне",
        SectionType = SectionType.Main,
    };

    public UpdateWhoWeAreContentTests()
    {
        _mockFactory = new Mock<IWhoWeAreContentFactory>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _validator = new UpdateWhoWeAreContentValidator();
    }

    [Theory]
    [InlineData("New description")]
    [InlineData("Another text")]
    public async Task Handle_ShouldUpdateEntity(string updatedText)
    {
        // Arrange
        var command = new UpdateWhoWeAreContentCommand(
            SectionType.Main,
            new List<UpdateWhoWeAreContentDto>
            {
                new() { Id = 1, ContentType = ContentType.Description, Description = updatedText }
            });

        SetupRepositoryWrapper(_testSection, new List<WhoWeAreContent> { _testDescriptionContent });
        _mockMapper.Setup(m => m.Map<WhoWeAreSectionDto>(_testSection))
            .Returns(_testSectionDto);

        // Act
        var handler = new UpdateWhoWeAreContentHandler(_mockFactory.Object, _mockRepositoryWrapper.Object, _mockMapper.Object, _validator);

        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockFactory.Verify(f => f.UpdateDescription(It.IsAny<UpdateWhoWeAreContentDto>(), _testDescriptionContent), Times.Once);
        _mockRepositoryWrapper.Verify(r => r.WhoWeAreContentsRepository.Update(_testDescriptionContent), Times.Once);
        _mockRepositoryWrapper.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldUpdateEntityAndDeleteOldImage()
    {
        // Arrange
        var command = new UpdateWhoWeAreContentCommand(
            SectionType.WhoWeSupport,
            new List<UpdateWhoWeAreContentDto>
            {
                new() { Id = 1, ContentType = ContentType.Card, ImageId = 3, Description = "Description 1" }
            });
        SetupRepositoryWrapper(_testSection, new List<WhoWeAreContent> { _testCardContent });

        // Act
        var handler = new UpdateWhoWeAreContentHandler(_mockFactory.Object, _mockRepositoryWrapper.Object, _mockMapper.Object, _validator);
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockFactory.Verify(f => f.UpdateCard(It.IsAny<UpdateWhoWeAreContentDto>(), _testCardContent), Times.Once);
        _mockRepositoryWrapper.Verify(r => r.WhoWeAreContentsRepository.Update(_testCardContent), Times.Once);
        _mockRepositoryWrapper.Verify(x => x.ImageRepository.DeleteRange(new List<Image> { _testImage }), Times.Once);
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_ShouldUpdateEntityAndNotDeleteOldImage()
    {
        // Arrange
        var command = new UpdateWhoWeAreContentCommand(
            SectionType.WhoWeSupport,
            new List<UpdateWhoWeAreContentDto>
            {
                new() { Id = 1, ContentType = ContentType.Card, ImageId = 1, Description = "Description 1" }
            });

        SetupRepositoryWrapper(_testSection, new List<WhoWeAreContent> { _testCardContent });

        // Act
        var handler = new UpdateWhoWeAreContentHandler(_mockFactory.Object, _mockRepositoryWrapper.Object, _mockMapper.Object, _validator);
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockFactory.Verify(f => f.UpdateCard(It.IsAny<UpdateWhoWeAreContentDto>(), _testCardContent), Times.Once);
        _mockRepositoryWrapper.Verify(r => r.WhoWeAreContentsRepository.Update(_testCardContent), Times.Once);
        _mockRepositoryWrapper.Verify(x => x.ImageRepository.DeleteRange(It.IsAny<IEnumerable<Image>>()), Times.Never);
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ContentNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateWhoWeAreContentCommand(
            SectionType.Main,
            new List<UpdateWhoWeAreContentDto> { new() { Id = 99, ContentType = ContentType.Description } });

        SetupRepositoryWrapper(_testSection, new List<WhoWeAreContent>());

        var handler = new UpdateWhoWeAreContentHandler(_mockFactory.Object, _mockRepositoryWrapper.Object, _mockMapper.Object, _validator);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.NotFound(command.Contents.First().Id, typeof(WhoWeAreContent)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ContentBelongsToOtherSection_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateWhoWeAreContentCommand(
            SectionType.Main,
            new List<UpdateWhoWeAreContentDto> { new() { Id = 1, ContentType = ContentType.Description } });

        var foreignContent = new DescriptionContent { Id = 1, SectionId = 999, ContentType = ContentType.Description };

        SetupRepositoryWrapper(_testSection, new List<WhoWeAreContent> { foreignContent });

        var handler = new UpdateWhoWeAreContentHandler(_mockFactory.Object, _mockRepositoryWrapper.Object, _mockMapper.Object, _validator);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            WhoWeAreConstants.EntityDoesNotBelongToTheSection(typeof(WhoWeAreContent), command.SectionType),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ContentTypeMismatch_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateWhoWeAreContentCommand(
            SectionType.Main,
            new List<UpdateWhoWeAreContentDto> { new() { Id = 1, ContentType = ContentType.Image } });

        SetupRepositoryWrapper(_testSection, new List<WhoWeAreContent> { _testDescriptionContent });

        var handler = new UpdateWhoWeAreContentHandler(_mockFactory.Object, _mockRepositoryWrapper.Object, _mockMapper.Object, _validator);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        var dto = command.Contents.First();
        Assert.Contains(
            WhoWeAreConstants.DtoHasWrongContentType(dto.Id, ContentType.Description, dto.ContentType),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_InvalidSection_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateWhoWeAreContentCommand(SectionType.Main, new List<UpdateWhoWeAreContentDto>());

        SetupRepositoryWrapper();

        var handler = new UpdateWhoWeAreContentHandler(_mockFactory.Object, _mockRepositoryWrapper.Object, _mockMapper.Object, _validator);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.PropertyMustBeValidEnum(nameof(command.SectionType)),
            result.Errors[0].Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Short")]
    [InlineData("ninechars")]
    public async Task Handle_ValidationFails_ShouldReturnValidationError(string? description)
    {
        // Arrange
        var command = new UpdateWhoWeAreContentCommand(
            SectionType.Main,
            new List<UpdateWhoWeAreContentDto>
                { new() { Id = 1, ContentType = ContentType.Description, Description = description } });
        var handler = new UpdateWhoWeAreContentHandler(_mockFactory.Object, _mockRepositoryWrapper.Object, _mockMapper.Object, _validator);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateWhoWeAreContentDto.Description), 10),
            result.Errors[0].Message);
    }

    private void SetupRepositoryWrapper(
        WhoWeAreSection? sectionToReturn = null,
        List<WhoWeAreContent>? contentsToReturn = null)
    {
        _mockRepositoryWrapper.Setup(r =>
                r.WhoWeAreSectionsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<WhoWeAreSection>>()))
            .ReturnsAsync(sectionToReturn);

        _mockRepositoryWrapper.Setup(r =>
                r.WhoWeAreContentsRepository.GetAllAsync(It.IsAny<QueryOptions<WhoWeAreContent>>()))
            .ReturnsAsync(contentsToReturn ?? new List<WhoWeAreContent>());

        _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        _mockRepositoryWrapper.Setup(r =>
            r.ImageRepository
                .GetAllAsync(It.IsAny<QueryOptions<Image>>())).ReturnsAsync(new List<Image> { _testImage });
    }
}
