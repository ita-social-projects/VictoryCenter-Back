using System.Transactions;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.WhoWeAreContents.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.WhoWeAreContents;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.WhoWeAreContents;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Entities.WhoWeAreContents;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.WhoWeAreContents;
using VictoryCenter.DAL.Repositories.Interfaces.WhoWeAreSections;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.WhoWeAreContents;

public class CreateWhoWeAreContentLocalizationTests
{
    private readonly Mock<ILocalizationService<WhoWeAreContent, WhoWeAreContentLocalization>> _mockLocalizationService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IWhoWeAreContentsRepository> _mockContentsRepository;
    private readonly Mock<IWhoWeAreSectionsRepository> _mockSectionsRepository;
    private readonly IValidator<CreateWhoWeAreContentLocalizationCommand> _validator;

    private readonly WhoWeAreSection _testSection = new()
    {
        Id = 10,
        SectionType = SectionType.Main,
        Title = "Main Section",
        Contents = null!
    };

    private readonly WhoWeAreContentLocalization _testEntity = new()
    {
        EntityId = 1,
        LanguageId = 1,
        Title = "Valid title here"
    };

    private readonly WhoWeAreContentLocalizationDto _testDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 1, Code = "en" },
        Title = "Valid title here"
    };

    private readonly CreateWhoWeAreContentLocalizationDto _testCreateDto = new()
    {
        EntityId = 1,
        LanguageId = 1,
        Title = "Valid title here"
    };

    public CreateWhoWeAreContentLocalizationTests()
    {
        _mockLocalizationService = new Mock<ILocalizationService<WhoWeAreContent, WhoWeAreContentLocalization>>();
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockContentsRepository = new Mock<IWhoWeAreContentsRepository>();
        _mockSectionsRepository = new Mock<IWhoWeAreSectionsRepository>();
        _validator = new CreateWhoWeAreContentLocalizationValidator();

        _mockRepositoryWrapper.Setup(x => x.WhoWeAreContentsRepository).Returns(_mockContentsRepository.Object);
        _mockRepositoryWrapper.Setup(x => x.WhoWeAreSectionsRepository).Returns(_mockSectionsRepository.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateWhoWeAreContentLocalization_Successfully()
    {
        // Arrange
        var titleContent = CreateContentByType(ContentType.Title, id: 1, sectionId: _testSection.Id);
        var command = new CreateWhoWeAreContentLocalizationCommand(
            SectionType.Main, new List<CreateWhoWeAreContentLocalizationDto> { _testCreateDto });

        SetupRepositoryWrapper(_testSection, new List<WhoWeAreContent> { titleContent });
        SetupMapperAndService();

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);
        _mockMapper.Verify(m => m.Map<WhoWeAreContentLocalization>(It.IsAny<CreateWhoWeAreContentLocalizationDto>()), Times.Once);
        _mockLocalizationService.Verify(s => s.CreateEntityLocalizationAsync(It.IsAny<WhoWeAreContentLocalization>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSectionNotFound()
    {
        // Arrange
        var command = new CreateWhoWeAreContentLocalizationCommand(
            SectionType.Main, new List<CreateWhoWeAreContentLocalizationDto> { _testCreateDto });

        SetupRepositoryWrapper(sectionToReturn: null, new List<WhoWeAreContent>());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.PropertyMustBeValidEnum(nameof(command.SectionType)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenContentNotFound()
    {
        // Arrange
        var command = new CreateWhoWeAreContentLocalizationCommand(
            SectionType.Main, new List<CreateWhoWeAreContentLocalizationDto> { _testCreateDto });

        SetupRepositoryWrapper(_testSection, new List<WhoWeAreContent>());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.NotFound(_testCreateDto.EntityId, typeof(WhoWeAreContent)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenContentDoesNotBelongToSection()
    {
        // Arrange
        var foreignContent = CreateContentByType(ContentType.Title, id: 1, sectionId: 999);
        var command = new CreateWhoWeAreContentLocalizationCommand(
            SectionType.Main, new List<CreateWhoWeAreContentLocalizationDto> { _testCreateDto });

        SetupRepositoryWrapper(_testSection, new List<WhoWeAreContent> { foreignContent });

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            WhoWeAreConstants.EntityDoesNotBelongToTheSection(typeof(WhoWeAreContent), SectionType.Main),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenImageContentLocalizationRequested()
    {
        // Arrange
        var imageContent = CreateContentByType(ContentType.Image, id: 1, sectionId: _testSection.Id);
        var command = new CreateWhoWeAreContentLocalizationCommand(
            SectionType.Main, new List<CreateWhoWeAreContentLocalizationDto> { _testCreateDto });

        SetupRepositoryWrapper(_testSection, new List<WhoWeAreContent> { imageContent });

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            WhoWeAreContentLocalizationConstants.CannotCreateLocalizationForContentType(typeof(ImageContent), _testCreateDto.EntityId),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenRequiredContentFieldIsMissing()
    {
        // Arrange
        var titleContent = CreateContentByType(ContentType.Title, id: 1, sectionId: _testSection.Id);
        var dto = new CreateWhoWeAreContentLocalizationDto { EntityId = 1, LanguageId = 1, Title = null };
        var command = new CreateWhoWeAreContentLocalizationCommand(
            SectionType.Main, new List<CreateWhoWeAreContentLocalizationDto> { dto });

        SetupRepositoryWrapper(_testSection, new List<WhoWeAreContent> { titleContent });

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            WhoWeAreContentLocalizationConstants.FieldIsRequiredForContentType(nameof(dto.Title), typeof(TitleContent), dto.EntityId),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        // Arrange
        var titleContent = CreateContentByType(ContentType.Title, id: 1, sectionId: _testSection.Id);
        var command = new CreateWhoWeAreContentLocalizationCommand(
            SectionType.Main, new List<CreateWhoWeAreContentLocalizationDto> { _testCreateDto });

        SetupRepositoryWrapper(_testSection, new List<WhoWeAreContent> { titleContent });
        _mockMapper.Setup(m => m.Map<WhoWeAreContentLocalization>(It.IsAny<CreateWhoWeAreContentLocalizationDto>()))
            .Returns(_testEntity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<WhoWeAreContentLocalization>()))
            .ThrowsAsync(new DbUpdateException());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(WhoWeAreContentLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        // Arrange
        var notFoundMessage = "Language or entity not found";
        var titleContent = CreateContentByType(ContentType.Title, id: 1, sectionId: _testSection.Id);
        var command = new CreateWhoWeAreContentLocalizationCommand(
            SectionType.Main, new List<CreateWhoWeAreContentLocalizationDto> { _testCreateDto });

        SetupRepositoryWrapper(_testSection, new List<WhoWeAreContent> { titleContent });
        _mockMapper.Setup(m => m.Map<WhoWeAreContentLocalization>(It.IsAny<CreateWhoWeAreContentLocalizationDto>()))
            .Returns(_testEntity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<WhoWeAreContentLocalization>()))
            .ThrowsAsync(new KeyNotFoundException(notFoundMessage));

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Equal(notFoundMessage, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        // Arrange
        var titleContent = CreateContentByType(ContentType.Title, id: 1, sectionId: _testSection.Id);
        var command = new CreateWhoWeAreContentLocalizationCommand(
            SectionType.Main, new List<CreateWhoWeAreContentLocalizationDto> { _testCreateDto });

        SetupRepositoryWrapper(_testSection, new List<WhoWeAreContent> { titleContent });
        _mockMapper.Setup(m => m.Map<WhoWeAreContentLocalization>(It.IsAny<CreateWhoWeAreContentLocalizationDto>()))
            .Returns(_testEntity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<WhoWeAreContentLocalization>()))
            .ThrowsAsync(new InvalidOperationException());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(WhoWeAreContentLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        // Arrange
        var command = new CreateWhoWeAreContentLocalizationCommand(SectionType.Main, null!);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.PropertyIsRequired(nameof(CreateWhoWeAreContentLocalizationCommand.ContentLocalizationDtos)),
            result.Errors[0].Message);
    }

    private CreateWhoWeAreContentLocalizationHandler CreateHandler() =>
        new(_mockMapper.Object, _validator, _mockLocalizationService.Object, _mockRepositoryWrapper.Object);

    private void SetupMapperAndService()
    {
        _mockMapper
            .Setup(m => m.Map<WhoWeAreContentLocalization>(It.IsAny<CreateWhoWeAreContentLocalizationDto>()))
            .Returns(_testEntity);

        _mockMapper
            .Setup(m => m.Map<WhoWeAreContentLocalizationDto>(It.IsAny<WhoWeAreContentLocalization>()))
            .Returns(_testDto);

        _mockLocalizationService
            .Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<WhoWeAreContentLocalization>()))
            .ReturnsAsync(_testEntity);
    }

    private void SetupRepositoryWrapper(
        WhoWeAreSection? sectionToReturn = null,
        List<WhoWeAreContent>? contentsToReturn = null)
    {
        _mockSectionsRepository
            .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<WhoWeAreSection>>()))
            .ReturnsAsync(sectionToReturn);

        _mockContentsRepository
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<WhoWeAreContent>>()))
            .ReturnsAsync(contentsToReturn ?? new List<WhoWeAreContent>());

        _mockRepositoryWrapper
            .Setup(r => r.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));
    }

    private static WhoWeAreContent CreateContentByType(ContentType contentType, long id, long sectionId) =>
        contentType switch
        {
            ContentType.Title => new TitleContent { Id = id, ContentType = ContentType.Title, SectionId = sectionId, Title = "Default Title" },
            ContentType.Description => new DescriptionContent { Id = id, ContentType = ContentType.Description, SectionId = sectionId, Description = "Default Description" },
            ContentType.Card => new CardContent { Id = id, ContentType = ContentType.Card, SectionId = sectionId, Description = "Default Card Description" },
            ContentType.Image => new ImageContent { Id = id, ContentType = ContentType.Image, SectionId = sectionId },
            _ => throw new ArgumentException($"Unsupported content type: {contentType}")
        };
}
