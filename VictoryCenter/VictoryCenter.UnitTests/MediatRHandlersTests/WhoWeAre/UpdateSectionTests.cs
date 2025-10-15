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

    private readonly DescriptionContent _testContent = new()
    {
        Id = 1,
        SectionId = 1,
        ContentType = ContentType.Description,
        Description = "Description"
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
            new List<CreateWhoWeAreContentDto>
            {
                new() { Id = 1, ContentType = ContentType.Description, Description = updatedText }
            });

        SetupRepositiryWrapper(_testSection, new List<WhoWeAreContent> { _testContent });
        _mockMapper.Setup(m => m.Map<WhoWeAreSectionDto>(_testSection))
            .Returns(_testSectionDto);

        // Act
        var handler = new UpdateWhoWeAreContentHandler(_mockFactory.Object, _mockRepositoryWrapper.Object, _mockMapper.Object, _validator);

        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockFactory.Verify(f => f.UpdateDescription(It.IsAny<CreateWhoWeAreContentDto>(), _testContent), Times.Once);
        _mockRepositoryWrapper.Verify(r => r.WhoWeAreContentsRepository.Update(_testContent), Times.Once);
        _mockRepositoryWrapper.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ContentNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateWhoWeAreContentCommand(
            SectionType.Main,
            new List<CreateWhoWeAreContentDto> { new() { Id = 99, ContentType = ContentType.Description } });

        SetupRepositiryWrapper(_testSection, new List<WhoWeAreContent>());

        var handler = new UpdateWhoWeAreContentHandler(_mockFactory.Object, _mockRepositoryWrapper.Object, _mockMapper.Object, _validator);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(ErrorMessagesConstants.NotFound(command.Contents.First().Id, typeof(WhoWeAreContent)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ContentBelongsToOtherSection_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateWhoWeAreContentCommand(
            SectionType.Main,
            new List<CreateWhoWeAreContentDto> { new() { Id = 1, ContentType = ContentType.Description } });

        var foreignContent = new DescriptionContent { Id = 1, SectionId = 999, ContentType = ContentType.Description };

        SetupRepositiryWrapper(_testSection, new List<WhoWeAreContent> { foreignContent });

        var handler = new UpdateWhoWeAreContentHandler(_mockFactory.Object, _mockRepositoryWrapper.Object, _mockMapper.Object, _validator);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(WhoWeAreConstants.EntityDoesNotBelongToTheSection(typeof(WhoWeAreContent), command.Contents.First().Id), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ContentTypeMismatch_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateWhoWeAreContentCommand(
            SectionType.Main,
            new List<CreateWhoWeAreContentDto> { new() { Id = 1, ContentType = ContentType.Image } });

        SetupRepositiryWrapper(_testSection, new List<WhoWeAreContent> { _testContent });

        var handler = new UpdateWhoWeAreContentHandler(_mockFactory.Object, _mockRepositoryWrapper.Object, _mockMapper.Object, _validator);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(WhoWeAreConstants.WrongContentType, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_InvalidSection_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateWhoWeAreContentCommand(SectionType.Main, new List<CreateWhoWeAreContentDto>());

        SetupRepositiryWrapper();

        var handler = new UpdateWhoWeAreContentHandler(_mockFactory.Object, _mockRepositoryWrapper.Object, _mockMapper.Object, _validator);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(ErrorMessagesConstants.PropertyMustBeValidEnum(nameof(command.SectionType)), result.Errors[0].Message);
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
            new List<CreateWhoWeAreContentDto> { new() { Id = 1, ContentType = ContentType.Description, Description = description } });
        var handler = new UpdateWhoWeAreContentHandler(_mockFactory.Object, _mockRepositoryWrapper.Object, _mockMapper.Object, _validator);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateWhoWeAreContentDto.Description), 10),
            result.Errors[0].Message);
    }

    private void SetupRepositiryWrapper(WhoWeAreSection? sectionToReturn = null, List<WhoWeAreContent>? contentsToReturn = null)
    {
        _mockRepositoryWrapper.Setup(r => r.WhoWeAreSectionsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<WhoWeAreSection>>()))
            .ReturnsAsync(sectionToReturn);

        _mockRepositoryWrapper.Setup(r => r.WhoWeAreContentsRepository.GetAllAsync(It.IsAny<QueryOptions<WhoWeAreContent>>()))
            .ReturnsAsync(contentsToReturn ?? new List<WhoWeAreContent>());

        _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
    }
}
