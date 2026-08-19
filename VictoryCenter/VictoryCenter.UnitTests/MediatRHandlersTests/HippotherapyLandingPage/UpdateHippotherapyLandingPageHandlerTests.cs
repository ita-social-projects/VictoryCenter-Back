using System.Linq.Expressions;
using System.Transactions;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.HippotherapyLandingPage.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

using EntityHippotherapyLandingPage = VictoryCenter.DAL.Entities.HippotherapyLandingPage;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HippotherapyLandingPage;

public class UpdateHippotherapyLandingPageHandlerTests
{
    private const long ExistingImageId1 = 101;
    private const long ExistingImageId2 = 102;
    private const long NewImageId = 201;

    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IValidator<UpdateHippotherapyLandingPageCommand>> _validatorMock = new();
    private readonly Mock<IReorderService> _reorderServiceMock = new();

    public UpdateHippotherapyLandingPageHandlerTests()
    {
        _repositoryWrapperMock
            .Setup(x => x.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));
        _repositoryWrapperMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        _reorderServiceMock
            .Setup(s => s.RenumberPriorityAsync<HippotherapyLandingPageScientificReference>(
                It.IsAny<Expression<Func<HippotherapyLandingPageScientificReference, bool>>>()))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(x => x.Map<HippotherapyLandingPageDto>(It.IsAny<EntityHippotherapyLandingPage>()))
            .Returns(new HippotherapyLandingPageDto
            {
                IntroSection = null!,
                DescriptionSection = null!,
                QuoteSection = null!,
                HippoventionSection = null!,
                HippoventionCenterSection = null!,
                AdvantagesSection = null!,
                AnalysisSection = null!,
                ScientificReferencesSection = null!,
                AnotherQuoteSection = null!,
                ParticipantsSection = null!,
                EthicsSection = null!,
            });

        SetupValidationSuccess();
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailResultAndSkipTransaction()
    {
        // Arrange
        SetupValidationFailure("Validation failed");
        var command = new UpdateHippotherapyLandingPageCommand(GetValidUpdateDto(GetExistingEntity()));
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("Validation failed", result.Errors[0].Message);
        _repositoryWrapperMock.Verify(x => x.BeginTransaction(), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRequestedImageDoesNotExist_ShouldReturnNotFoundAndSkipTransaction()
    {
        // Arrange
        var existing = GetExistingEntity();
        var dto = GetValidUpdateDto(existing) with
        {
            IntroSection = GetValidUpdateDto(existing).IntroSection with { ImageId = 999 },
        };
        SetUpRepositoryWrapper(existing);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new UpdateHippotherapyLandingPageCommand(dto), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(ErrorMessagesConstants.NotFound([999L], typeof(Image)), result.Errors.Select(e => e.Message));
        _repositoryWrapperMock.Verify(x => x.BeginTransaction(), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenPageDoesNotExist_ShouldCreateNewPage()
    {
        // Arrange
        SetUpRepositoryWrapper(entityToReturn: null);
        SetUpMapperForCreate();
        var dto = GetValidUpdateDto(GetExistingEntity());
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new UpdateHippotherapyLandingPageCommand(dto), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _repositoryWrapperMock.Verify(x => x.HippotherapyLandingPagesRepository.CreateAsync(It.IsAny<EntityHippotherapyLandingPage>()), Times.Once);
        _repositoryWrapperMock.Verify(x => x.HippotherapyLandingPagesRepository.Update(It.IsAny<EntityHippotherapyLandingPage>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenPageExists_ShouldUpdateExistingPage()
    {
        // Arrange
        var existing = GetExistingEntity();
        var dto = GetValidUpdateDto(existing);
        SetUpRepositoryWrapper(existing);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new UpdateHippotherapyLandingPageCommand(dto), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _repositoryWrapperMock.Verify(x => x.HippotherapyLandingPagesRepository.Update(existing), Times.Once);
        _repositoryWrapperMock.Verify(x => x.HippotherapyLandingPagesRepository.CreateAsync(It.IsAny<EntityHippotherapyLandingPage>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenScientificReferenceIdNotFound_ShouldReturnFailAndSkipSave()
    {
        // Arrange
        var existing = GetExistingEntity();
        var validDto = GetValidUpdateDto(existing);
        var dto = validDto with
        {
            ScientificReferencesSection = validDto.ScientificReferencesSection with
            {
                ScientificReferences = [new UpdateScientificReferenceDto { Id = 9999, Name = "Unknown reference name", Url = "https://example.com/unknown" }],
            },
        };
        SetUpRepositoryWrapper(existing);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new UpdateHippotherapyLandingPageCommand(dto), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(
            ErrorMessagesConstants.NotFound(9999L, typeof(HippotherapyLandingPageScientificReference)),
            result.Errors.Select(e => e.Message));
        _repositoryWrapperMock.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenNewScientificReferenceHasNoId_ShouldAddItWithNextPriority()
    {
        // Arrange
        var existing = GetExistingEntity();
        var validDto = GetValidUpdateDto(existing);
        var dto = validDto with
        {
            ScientificReferencesSection = validDto.ScientificReferencesSection with
            {
                ScientificReferences =
                [
                    .. validDto.ScientificReferencesSection.ScientificReferences,
                    new UpdateScientificReferenceDto { Id = null, Name = "Brand new reference", Url = "https://example.com/new" },
                ],
            },
        };
        SetUpRepositoryWrapper(existing);
        _mapperMock
            .Setup(x => x.Map<HippotherapyLandingPageScientificReference>(It.IsAny<UpdateScientificReferenceDto>()))
            .Returns((UpdateScientificReferenceDto src) => new HippotherapyLandingPageScientificReference
            {
                Name = src.Name,
                Url = src.Url,
            });
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new UpdateHippotherapyLandingPageCommand(dto), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(3, existing.ScientificReferencesSection!.ScientificReferences.Count);
        var added = existing.ScientificReferencesSection.ScientificReferences.Single(r => r.Name == "Brand new reference");
        Assert.Equal(3, added.Priority);
    }

    [Fact]
    public async Task Handle_WhenExistingScientificReferenceOmittedFromRequest_ShouldDeleteIt()
    {
        // Arrange
        var existing = GetExistingEntity();
        var remaining = existing.ScientificReferencesSection!.ScientificReferences.First();
        var dto = GetValidUpdateDto(existing) with
        {
            ScientificReferencesSection = new UpdateScientificReferencesSectionDto
            {
                Title = "New references title",
                Description = "New references description",
                ScientificReferences = [new UpdateScientificReferenceDto { Id = remaining.Id, Name = remaining.Name, Url = remaining.Url }],
            },
        };
        SetUpRepositoryWrapper(existing);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new UpdateHippotherapyLandingPageCommand(dto), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _repositoryWrapperMock.Verify(
            x => x.HippotherapyLandingPageScientificReferencesRepository.DeleteRange(
                It.Is<List<HippotherapyLandingPageScientificReference>>(list => list.Count == 1 && list[0].Id != remaining.Id)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenFixedListCountsMatch_ShouldUpdateCardsAndPrinciplesInPlaceByPosition()
    {
        // Arrange
        var existing = GetExistingEntity();
        var originalCardIds = existing.AdvantagesSection!.AdvantageCards.OrderBy(c => c.Priority).Select(c => c.Id).ToList();
        var originalPrincipleIds = existing.EthicsSection!.EthicsPrinciples.OrderBy(p => p.Priority).Select(p => p.Id).ToList();
        var dto = GetValidUpdateDto(existing);
        SetUpRepositoryWrapper(existing);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new UpdateHippotherapyLandingPageCommand(dto), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        var updatedCards = existing.AdvantagesSection.AdvantageCards.OrderBy(c => c.Priority).ToList();
        Assert.Equal(originalCardIds, updatedCards.Select(c => c.Id).ToList());
        Assert.Equal(
            dto.AdvantagesSection.Cards.Select(c => c.Description).ToList(),
            updatedCards.Select(c => c.Description).ToList());

        var updatedPrinciples = existing.EthicsSection.EthicsPrinciples.OrderBy(p => p.Priority).ToList();
        Assert.Equal(originalPrincipleIds, updatedPrinciples.Select(p => p.Id).ToList());
        Assert.Equal(dto.EthicsSection.Principles, updatedPrinciples.Select(p => p.Text).ToList());
    }

    [Fact]
    public async Task Handle_WhenImageIsReplacedAndNoLongerReferenced_ShouldDeleteOldImage()
    {
        // Arrange
        var existing = GetExistingEntity();
        existing.IntroSection!.ImageId = ExistingImageId1;
        var baseDto = GetValidUpdateDto(existing);
        var dto = baseDto with
        {
            IntroSection = baseDto.IntroSection with { ImageId = NewImageId },
            HippoventionCenterSection = baseDto.HippoventionCenterSection with { ImageId = null },
        };
        SetUpRepositoryWrapper(existing);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new UpdateHippotherapyLandingPageCommand(dto), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _repositoryWrapperMock.Verify(
            x => x.ImageRepository.DeleteRange(It.Is<IEnumerable<Image>>(images => images.Any(i => i.Id == ExistingImageId1))),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenReplacedImageIsStillReferencedElsewhereInRequest_ShouldNotDeleteIt()
    {
        // Arrange
        var existing = GetExistingEntity();
        existing.IntroSection!.ImageId = ExistingImageId1;
        existing.QuoteSection!.ImageId = ExistingImageId2;

        var baseDto = GetValidUpdateDto(existing);
        var dto = baseDto with
        {
            IntroSection = baseDto.IntroSection with { ImageId = NewImageId },
            QuoteSection = baseDto.QuoteSection with { ImageId = ExistingImageId1 },
        };
        SetUpRepositoryWrapper(existing);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new UpdateHippotherapyLandingPageCommand(dto), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _repositoryWrapperMock.Verify(
            x => x.ImageRepository.DeleteRange(It.Is<IEnumerable<Image>>(images => !images.Any(i => i.Id == ExistingImageId1))),
            Times.Once);
        _repositoryWrapperMock.Verify(
            x => x.ImageRepository.DeleteRange(It.Is<IEnumerable<Image>>(images => images.Any(i => i.Id == ExistingImageId2))),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenReorderExceptionOccurs_ShouldReturnReorderFailResult()
    {
        // Arrange
        var existing = GetExistingEntity();
        var dto = GetValidUpdateDto(existing);
        SetUpRepositoryWrapper(existing);
        _reorderServiceMock
            .Setup(s => s.RenumberPriorityAsync<HippotherapyLandingPageScientificReference>(
                It.IsAny<Expression<Func<HippotherapyLandingPageScientificReference, bool>>>()))
            .ThrowsAsync(new ReorderException("reorder boom"));
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new UpdateHippotherapyLandingPageCommand(dto), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(ReorderConstants.ErrorWithReordering("reorder boom"), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_WhenDbUpdateExceptionOccurs_ShouldReturnFailResult()
    {
        // Arrange
        var existing = GetExistingEntity();
        var dto = GetValidUpdateDto(existing);
        SetUpRepositoryWrapper(existing);
        _repositoryWrapperMock.Setup(x => x.SaveChangesAsync()).ThrowsAsync(new DbUpdateException());
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new UpdateHippotherapyLandingPageCommand(dto), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(EntityHippotherapyLandingPage)),
            result.Errors[0].Message);
    }

    private UpdateHippotherapyLandingPageHandler CreateHandler() => new(
        _repositoryWrapperMock.Object,
        _mapperMock.Object,
        _validatorMock.Object,
        _reorderServiceMock.Object);

    private void SetUpRepositoryWrapper(EntityHippotherapyLandingPage? entityToReturn, List<Image>? availableImages = null)
    {
        _repositoryWrapperMock
            .Setup(x => x.HippotherapyLandingPagesRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<EntityHippotherapyLandingPage>>()))
            .ReturnsAsync(entityToReturn);

        _repositoryWrapperMock
            .Setup(x => x.HippotherapyLandingPagesRepository.CreateAsync(It.IsAny<EntityHippotherapyLandingPage>()))
            .ReturnsAsync((EntityHippotherapyLandingPage e) => e);

        _repositoryWrapperMock
            .Setup(x => x.HippotherapyLandingPageScientificReferencesRepository.DeleteRange(
                It.IsAny<IEnumerable<HippotherapyLandingPageScientificReference>>()));

        _repositoryWrapperMock
            .Setup(x => x.ImageRepository.GetAllAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync((QueryOptions<Image> options) =>
            {
                var images = (availableImages ?? DefaultAvailableImages()).AsQueryable();
                return options.Filter != null ? images.Where(options.Filter).ToList() : images.ToList();
            });
    }

    private void SetUpMapperForCreate()
    {
        _mapperMock.Setup(x => x.Map<HippotherapyLandingPageIntroSection>(It.IsAny<UpdateIntroSectionDto>()))
            .Returns(new HippotherapyLandingPageIntroSection());
        _mapperMock.Setup(x => x.Map<HippotherapyLandingPageDescriptionSection>(It.IsAny<UpdateTextSectionDto>()))
            .Returns(new HippotherapyLandingPageDescriptionSection());
        _mapperMock.Setup(x => x.Map<HippotherapyLandingPageQuoteSection>(It.IsAny<UpdateQuoteSectionDto>()))
            .Returns(new HippotherapyLandingPageQuoteSection());
        _mapperMock.Setup(x => x.Map<HippotherapyLandingPageHippoventionSection>(It.IsAny<UpdateTextSectionDto>()))
            .Returns(new HippotherapyLandingPageHippoventionSection());
        _mapperMock.Setup(x => x.Map<HippotherapyLandingPageHippoventionCenterSection>(It.IsAny<UpdateHippoventionCenterSectionDto>()))
            .Returns(new HippotherapyLandingPageHippoventionCenterSection());
        _mapperMock.Setup(x => x.Map<HippotherapyLandingPageAdvantagesSection>(It.IsAny<UpdateGallerySectionDto>()))
            .Returns(new HippotherapyLandingPageAdvantagesSection());
        _mapperMock.Setup(x => x.Map<HippotherapyLandingPageAnalysisSection>(It.IsAny<UpdateTextSectionDto>()))
            .Returns(new HippotherapyLandingPageAnalysisSection());
        _mapperMock.Setup(x => x.Map<HippotherapyLandingPageScientificReferencesSection>(It.IsAny<UpdateScientificReferencesSectionDto>()))
            .Returns(new HippotherapyLandingPageScientificReferencesSection());
        _mapperMock.Setup(x => x.Map<HippotherapyLandingPageAnotherQuoteSection>(It.IsAny<UpdateQuoteSectionDto>()))
            .Returns(new HippotherapyLandingPageAnotherQuoteSection());
        _mapperMock.Setup(x => x.Map<HippotherapyLandingPageParticipantsSection>(It.IsAny<UpdateGallerySectionDto>()))
            .Returns(new HippotherapyLandingPageParticipantsSection());
        _mapperMock.Setup(x => x.Map<HippotherapyLandingPageEthicsSection>(It.IsAny<UpdateEthicsSectionDto>()))
            .Returns(new HippotherapyLandingPageEthicsSection());
    }

    private void SetupValidationSuccess()
    {
        _validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<UpdateHippotherapyLandingPageCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private void SetupValidationFailure(string errorMessage)
    {
        _validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<UpdateHippotherapyLandingPageCommand>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException([new ValidationFailure("Dto", errorMessage)]));
    }

    private static List<Image> DefaultAvailableImages() =>
    [
        new() { Id = ExistingImageId1 },
        new() { Id = ExistingImageId2 },
        new() { Id = NewImageId },
    ];

    private static EntityHippotherapyLandingPage GetExistingEntity()
    {
        const long pageId = 1;

        return new EntityHippotherapyLandingPage
        {
            Id = pageId,
            IntroSection = new HippotherapyLandingPageIntroSection
            {
                Id = 11, HippotherapyLandingPageId = pageId, Title = "Old intro title", Description = "Old intro description", ImageId = ExistingImageId1,
            },
            DescriptionSection = new HippotherapyLandingPageDescriptionSection
            {
                Id = 12, HippotherapyLandingPageId = pageId, Title = "Old description title", Description = "Old description text",
            },
            QuoteSection = new HippotherapyLandingPageQuoteSection
            {
                Id = 13, HippotherapyLandingPageId = pageId, QuoteText = "Old quote text", AuthorName = "Old author", ImageId = ExistingImageId2,
            },
            HippoventionSection = new HippotherapyLandingPageHippoventionSection
            {
                Id = 14, HippotherapyLandingPageId = pageId, Title = "Old hippovention title", Description = "Old hippovention description",
            },
            HippoventionCenterSection = new HippotherapyLandingPageHippoventionCenterSection
            {
                Id = 15, HippotherapyLandingPageId = pageId, Title = "Old center title", Description = "Old center description", Pros = "Old pros text", ImageId = ExistingImageId1,
            },
            AdvantagesSection = new HippotherapyLandingPageAdvantagesSection
            {
                Id = 16,
                HippotherapyLandingPageId = pageId,
                Title = "Old advantages title",
                AdvantageCards =
                [
                    new HippotherapyLandingPageAdvantageCard { Id = 161, AdvantagesSectionId = 16, Description = "Old card 1", Priority = 1 },
                    new HippotherapyLandingPageAdvantageCard { Id = 162, AdvantagesSectionId = 16, Description = "Old card 2", Priority = 2 },
                    new HippotherapyLandingPageAdvantageCard { Id = 163, AdvantagesSectionId = 16, Description = "Old card 3", Priority = 3 },
                    new HippotherapyLandingPageAdvantageCard { Id = 164, AdvantagesSectionId = 16, Description = "Old card 4", Priority = 4 },
                ],
            },
            AnalysisSection = new HippotherapyLandingPageAnalysisSection
            {
                Id = 17, HippotherapyLandingPageId = pageId, Title = "Old analysis title", Description = "Old analysis description",
            },
            ScientificReferencesSection = new HippotherapyLandingPageScientificReferencesSection
            {
                Id = 18,
                HippotherapyLandingPageId = pageId,
                Title = "Old references title",
                Description = "Old references description",
                ScientificReferences =
                [
                    new HippotherapyLandingPageScientificReference { Id = 181, ScientificReferencesSectionId = 18, Name = "Reference one", Url = "https://example.com/1", Priority = 1 },
                    new HippotherapyLandingPageScientificReference { Id = 182, ScientificReferencesSectionId = 18, Name = "Reference two", Url = "https://example.com/2", Priority = 2 },
                ],
            },
            AnotherQuoteSection = new HippotherapyLandingPageAnotherQuoteSection
            {
                Id = 19, HippotherapyLandingPageId = pageId, QuoteText = "Old another quote", AuthorName = "Old author two",
            },
            ParticipantsSection = new HippotherapyLandingPageParticipantsSection
            {
                Id = 20,
                HippotherapyLandingPageId = pageId,
                Title = "Old participants title",
                ParticipantCards =
                [
                    new HippotherapyLandingPageParticipantCard { Id = 201, ParticipantsSectionId = 20, Description = "Old participant 1", Priority = 1 },
                    new HippotherapyLandingPageParticipantCard { Id = 202, ParticipantsSectionId = 20, Description = "Old participant 2", Priority = 2 },
                    new HippotherapyLandingPageParticipantCard { Id = 203, ParticipantsSectionId = 20, Description = "Old participant 3", Priority = 3 },
                    new HippotherapyLandingPageParticipantCard { Id = 204, ParticipantsSectionId = 20, Description = "Old participant 4", Priority = 4 },
                ],
            },
            EthicsSection = new HippotherapyLandingPageEthicsSection
            {
                Id = 21,
                HippotherapyLandingPageId = pageId,
                Title = "Old ethics title",
                Description = "Old ethics description",
                EthicsPrinciples =
                [
                    new HippotherapyLandingPageEthicsPrinciple { Id = 211, EthicsSectionId = 21, Text = "Old principle 1", Priority = 1 },
                    new HippotherapyLandingPageEthicsPrinciple { Id = 212, EthicsSectionId = 21, Text = "Old principle 2", Priority = 2 },
                    new HippotherapyLandingPageEthicsPrinciple { Id = 213, EthicsSectionId = 21, Text = "Old principle 3", Priority = 3 },
                    new HippotherapyLandingPageEthicsPrinciple { Id = 214, EthicsSectionId = 21, Text = "Old principle 4", Priority = 4 },
                ],
            },
        };
    }

    private static UpdateHippotherapyLandingPageDto GetValidUpdateDto(EntityHippotherapyLandingPage existing) => new()
    {
        IntroSection = new UpdateIntroSectionDto { Title = "New intro title", Description = "New intro description", ImageId = ExistingImageId1 },
        DescriptionSection = new UpdateTextSectionDto { Title = "New description title", Description = "New description text" },
        QuoteSection = new UpdateQuoteSectionDto { QuoteText = "New quote text", AuthorName = "New author", ImageId = ExistingImageId2 },
        HippoventionSection = new UpdateTextSectionDto { Title = "New hippovention title", Description = "New hippovention description" },
        HippoventionCenterSection = new UpdateHippoventionCenterSectionDto
        {
            Title = "New center title", Description = "New center description", Pros = "New pros text", ImageId = ExistingImageId1,
        },
        AdvantagesSection = new UpdateGallerySectionDto
        {
            Title = "New advantages title",
            Cards =
            [
                new UpdateGalleryCardDto { Description = "New card 1", ImageId = null },
                new UpdateGalleryCardDto { Description = "New card 2", ImageId = null },
                new UpdateGalleryCardDto { Description = "New card 3", ImageId = null },
                new UpdateGalleryCardDto { Description = "New card 4", ImageId = null },
            ],
        },
        AnalysisSection = new UpdateTextSectionDto { Title = "New analysis title", Description = "New analysis description" },
        ScientificReferencesSection = new UpdateScientificReferencesSectionDto
        {
            Title = "New references title",
            Description = "New references description",
            ScientificReferences = existing.ScientificReferencesSection!.ScientificReferences
                .OrderBy(r => r.Priority)
                .Select((r, i) => new UpdateScientificReferenceDto { Id = r.Id, Name = $"Updated reference {i + 1}", Url = $"https://example.com/updated-{i + 1}" })
                .ToList(),
        },
        AnotherQuoteSection = new UpdateQuoteSectionDto { QuoteText = "New another quote", AuthorName = "New author two", ImageId = null },
        ParticipantsSection = new UpdateGallerySectionDto
        {
            Title = "New participants title",
            Cards =
            [
                new UpdateGalleryCardDto { Description = "New participant 1", ImageId = null },
                new UpdateGalleryCardDto { Description = "New participant 2", ImageId = null },
                new UpdateGalleryCardDto { Description = "New participant 3", ImageId = null },
                new UpdateGalleryCardDto { Description = "New participant 4", ImageId = null },
            ],
        },
        EthicsSection = new UpdateEthicsSectionDto
        {
            Title = "New ethics title",
            Description = "New ethics description",
            Principles = ["New principle 1", "New principle 2", "New principle 3", "New principle 4"],
            ImageId = null,
        },
    };
}
