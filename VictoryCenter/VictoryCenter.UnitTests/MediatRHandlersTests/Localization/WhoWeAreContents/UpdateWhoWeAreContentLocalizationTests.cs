using System.Transactions;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.WhoWeAreContents.Update;
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

public class UpdateWhoWeAreContentLocalizationTests
{
    private readonly Mock<ILocalizationService<WhoWeAreContent, WhoWeAreContentLocalization>> _mockLocalizationService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IWhoWeAreContentsRepository> _mockContentsRepository;
    private readonly Mock<IWhoWeAreSectionsRepository> _mockSectionsRepository;
    private readonly Mock<IRepositoryBase<WhoWeAreContentLocalization>> _mockGenericLocalizationRepository;
    private readonly IValidator<UpdateWhoWeAreContentLocalizationCommand> _validator;

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
        Title = "Updated title"
    };

    private readonly WhoWeAreContentLocalizationDto _testDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 1, Code = "en" },
        Title = "Updated title"
    };

    private readonly UpdateWhoWeAreContentLocalizationDto _testUpdateDto = new()
    {
        EntityId = 1,
        LanguageId = 1,
        Title = "Updated title",
        Description = "This value should be ignored for title content"
    };

    public UpdateWhoWeAreContentLocalizationTests()
    {
        _mockLocalizationService = new Mock<ILocalizationService<WhoWeAreContent, WhoWeAreContentLocalization>>();
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockContentsRepository = new Mock<IWhoWeAreContentsRepository>();
        _mockSectionsRepository = new Mock<IWhoWeAreSectionsRepository>();
        _mockGenericLocalizationRepository = new Mock<IRepositoryBase<WhoWeAreContentLocalization>>();
        _validator = new UpdateWhoWeAreContentLocalizationValidator();

        _mockRepositoryWrapper.Setup(x => x.WhoWeAreContentsRepository).Returns(_mockContentsRepository.Object);
        _mockRepositoryWrapper.Setup(x => x.WhoWeAreSectionsRepository).Returns(_mockSectionsRepository.Object);
        _mockRepositoryWrapper.Setup(x => x.GetRepository<WhoWeAreContentLocalization>()).Returns(_mockGenericLocalizationRepository.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateWhoWeAreContentLocalization_Successfully()
    {
        // Arrange
        var titleContent = CreateContentByType(ContentType.Title, id: 1, sectionId: _testSection.Id);
        var command = new UpdateWhoWeAreContentLocalizationCommand(
            SectionType.Main,
            new List<UpdateWhoWeAreContentLocalizationDto> { _testUpdateDto });

        SetupRepositoryWrapper(_testSection, new List<WhoWeAreContent> { titleContent });
        SetupMapperAndService();

        UpdateWhoWeAreContentLocalizationDto? mappedDto = null;
        _mockMapper
            .Setup(m => m.Map<WhoWeAreContentLocalization>(It.IsAny<UpdateWhoWeAreContentLocalizationDto>()))
            .Callback<object>(dto => mappedDto = dto as UpdateWhoWeAreContentLocalizationDto)
            .Returns(_testEntity);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);
        Assert.NotNull(mappedDto);
        Assert.Null(mappedDto!.Description);
        _mockLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(It.Is<IEnumerable<WhoWeAreContentLocalization>>(l => l.Count() == 1), true),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSectionNotFound()
    {
        // Arrange
        var command = new UpdateWhoWeAreContentLocalizationCommand(
            SectionType.Main,
            new List<UpdateWhoWeAreContentLocalizationDto> { _testUpdateDto });

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
        var command = new UpdateWhoWeAreContentLocalizationCommand(
            SectionType.Main,
            new List<UpdateWhoWeAreContentLocalizationDto> { _testUpdateDto });

        SetupRepositoryWrapper(_testSection, new List<WhoWeAreContent>());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.NotFound(_testUpdateDto.EntityId, typeof(WhoWeAreContent)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenContentDoesNotBelongToSection()
    {
        // Arrange
        var foreignContent = CreateContentByType(ContentType.Title, id: 1, sectionId: 999);
        var command = new UpdateWhoWeAreContentLocalizationCommand(
            SectionType.Main,
            new List<UpdateWhoWeAreContentLocalizationDto> { _testUpdateDto });

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
        var command = new UpdateWhoWeAreContentLocalizationCommand(
            SectionType.Main,
            new List<UpdateWhoWeAreContentLocalizationDto> { _testUpdateDto });

        SetupRepositoryWrapper(_testSection, new List<WhoWeAreContent> { imageContent });

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            WhoWeAreContentLocalizationConstants.CannotCreateLocalizationForContentType(typeof(ImageContent), _testUpdateDto.EntityId),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenRequiredContentFieldIsMissing()
    {
        // Arrange
        var titleContent = CreateContentByType(ContentType.Title, id: 1, sectionId: _testSection.Id);
        var dto = new UpdateWhoWeAreContentLocalizationDto
        {
            EntityId = 1,
            LanguageId = 1,
            Title = null
        };

        var command = new UpdateWhoWeAreContentLocalizationCommand(
            SectionType.Main,
            new List<UpdateWhoWeAreContentLocalizationDto> { dto });

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
    public async Task Handle_ShouldFail_WhenSaveChangesReturnsZero()
    {
        // Arrange
        var titleContent = CreateContentByType(ContentType.Title, id: 1, sectionId: _testSection.Id);
        var command = new UpdateWhoWeAreContentLocalizationCommand(
            SectionType.Main,
            new List<UpdateWhoWeAreContentLocalizationDto> { _testUpdateDto });

        SetupRepositoryWrapper(_testSection, new List<WhoWeAreContent> { titleContent });
        SetupMapperAndService(saveChangesResult: 0);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(WhoWeAreContentLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        // Arrange
        var titleContent = CreateContentByType(ContentType.Title, id: 1, sectionId: _testSection.Id);
        var command = new UpdateWhoWeAreContentLocalizationCommand(
            SectionType.Main,
            new List<UpdateWhoWeAreContentLocalizationDto> { _testUpdateDto });

        SetupRepositoryWrapper(_testSection, new List<WhoWeAreContent> { titleContent });
        SetupMapperAndService();
        _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new DbUpdateException());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(WhoWeAreContentLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        // Arrange
        var titleContent = CreateContentByType(ContentType.Title, id: 1, sectionId: _testSection.Id);
        var notFoundMessage = "Language or entity not found";
        var command = new UpdateWhoWeAreContentLocalizationCommand(
            SectionType.Main,
            new List<UpdateWhoWeAreContentLocalizationDto> { _testUpdateDto });

        SetupRepositoryWrapper(_testSection, new List<WhoWeAreContent> { titleContent });
        SetupMapperAndService();
        _mockLocalizationService
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<WhoWeAreContentLocalization>>(), true))
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
        var command = new UpdateWhoWeAreContentLocalizationCommand(
            SectionType.Main,
            new List<UpdateWhoWeAreContentLocalizationDto> { _testUpdateDto });

        SetupRepositoryWrapper(_testSection, new List<WhoWeAreContent> { titleContent });
        SetupMapperAndService();
        _mockLocalizationService
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<WhoWeAreContentLocalization>>(), true))
            .ThrowsAsync(new InvalidOperationException());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntity(typeof(WhoWeAreContentLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        // Arrange
        var command = new UpdateWhoWeAreContentLocalizationCommand(SectionType.Main, null!);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateWhoWeAreContentLocalizationCommand.ContentLocalizationDtos)),
            result.Errors[0].Message);
    }

    private UpdateWhoWeAreContentLocalizationHandler CreateHandler() =>
        new(_mockMapper.Object, _validator, _mockLocalizationService.Object, _mockRepositoryWrapper.Object);

    private void SetupMapperAndService(int saveChangesResult = 1)
    {
        _mockMapper
            .Setup(m => m.Map<WhoWeAreContentLocalization>(It.IsAny<UpdateWhoWeAreContentLocalizationDto>()))
            .Returns(_testEntity);

        _mockLocalizationService
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<WhoWeAreContentLocalization>>(), true))
            .Returns(Task.CompletedTask);

        _mockRepositoryWrapper
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(saveChangesResult);

        _mockGenericLocalizationRepository
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<WhoWeAreContentLocalization>>()))
            .ReturnsAsync(new List<WhoWeAreContentLocalization> { _testEntity });

        _mockMapper
            .Setup(m => m.Map<List<WhoWeAreContentLocalizationDto>>(It.IsAny<IEnumerable<WhoWeAreContentLocalization>>()))
            .Returns(new List<WhoWeAreContentLocalizationDto> { _testDto });
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
