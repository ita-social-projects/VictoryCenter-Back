using System.Transactions;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using VictoryCenter.BLL.Commands.Admin.History.Update;
using VictoryCenter.BLL.DTOs.Admin.HistorySection;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HistoryContents;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.HistorySections;
using VictoryCenter.DAL.Repositories.Interfaces.Media;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.BLL.Validators.HistorySections;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HistorySections;

public class UpdateHistorySectionsTests
{
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IRepositoryWrapper> _repositoryWrapper = new();
    private readonly Mock<IHistorySectionsRepository> _historySectionsRepository = new();
    private readonly Mock<IImageRepository> _imageRepository = new();
    private readonly Mock<IValidator<UpdateHistorySectionsCommand>> _validator = new();

    [Fact]
    public async Task Handle_SameStructure_ReplacesSections()
    {
        var image = MakeImage(10);
        var existing = ExistingSection(
            id: 1,
            order: 0,
            template: HistorySectionTemplate.SingleImageBottom,
            TitleContent(order: 0, title: "Old title"),
            DescriptionContent(order: 1, description: "Old description"),
            ImageContent(order: 2, imageId: 10, image: image));

        var dto = SectionDto(
            order: 0,
            template: HistorySectionTemplate.SingleImageBottom,
            TitleDto(order: 0, title: "  New title  "),
            DescriptionDto(order: 1, description: " New description "),
            ImageDto(order: 2, imageId: 10));

        var sut = CreateSut(existingSections: [existing], images: [image], saveChanges: 1);

        await sut.Handle(new UpdateHistorySectionsCommand([dto]), CancellationToken.None);

        _historySectionsRepository.Verify(r => r.DeleteRange(It.IsAny<IEnumerable<HistorySection>>()), Times.Once);
        _historySectionsRepository.Verify(r => r.CreateRangeAsync(It.IsAny<IEnumerable<HistorySection>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DifferentStructure_ReplacesSections()
    {
        var oldSection = ExistingSection(
            id: 1,
            order: 0,
            template: HistorySectionTemplate.TextOnly,
            TitleContent(order: 0, title: "Old title"),
            DescriptionContent(order: 1, description: "Old description"));

        var image = MakeImage(20);

        var replacement = SectionDto(
            order: 0,
            template: HistorySectionTemplate.SingleImageBottom,
            TitleDto(order: 0, title: "Title"),
            DescriptionDto(order: 1, description: "Description value"),
            ImageDto(order: 2, imageId: 20));

        var sut = CreateSut(existingSections: [oldSection], images: [image], saveChanges: 1);

        await sut.Handle(new UpdateHistorySectionsCommand([replacement]), CancellationToken.None);

        _historySectionsRepository.Verify(r => r.DeleteRange(It.IsAny<IEnumerable<HistorySection>>()), Times.Once);
        _historySectionsRepository.Verify(r => r.CreateRangeAsync(It.IsAny<IEnumerable<HistorySection>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ReplacesSectionsWithoutSectionIds()
    {
        var existing = ExistingSection(
            id: 1,
            order: 0,
            template: HistorySectionTemplate.TextOnly,
            TitleContent(order: 0, title: "Title"),
            DescriptionContent(order: 1, description: "Description"));

        var dto = SectionDto(
            order: 0,
            template: HistorySectionTemplate.TextOnly,
            TitleDto(order: 0, title: "Title"),
            DescriptionDto(order: 1, description: "Description"));

        var sut = CreateSut(existingSections: [existing], saveChanges: 1);

        var result = await sut.Handle(new UpdateHistorySectionsCommand([dto]), CancellationToken.None);

        Assert.True(result.IsSuccess);
        _historySectionsRepository.Verify(r => r.DeleteRange(It.IsAny<IEnumerable<HistorySection>>()), Times.Once);
        _historySectionsRepository.Verify(r => r.CreateRangeAsync(It.IsAny<IEnumerable<HistorySection>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_SaveChangesZeroAfterReplace_ReturnsFailedToUpdateEntity()
    {
        var oldSection = ExistingSection(
            id: 1,
            order: 0,
            template: HistorySectionTemplate.TextOnly,
            TitleContent(order: 0, title: "Old title"),
            DescriptionContent(order: 1, description: "Old description"));

        var replacement = SectionDto(
            order: 1,
            template: HistorySectionTemplate.TextOnly,
            TitleDto(order: 0, title: "Title"),
            DescriptionDto(order: 1, description: "Description"));

        var sut = CreateSut(existingSections: [oldSection], saveChanges: 0);

        var result = await sut.Handle(new UpdateHistorySectionsCommand([replacement]), CancellationToken.None);

        Assert.Contains(result.Errors.Select(e => e.Message), m => m.Contains("Failed") && m.Contains("HistorySection"));
    }

    [Fact]
    public async Task Handle_ImageNotFound_ReturnsNotFoundError()
    {
        var existing = ExistingSection(
            id: 1,
            order: 0,
            template: HistorySectionTemplate.SingleImageBottom,
            TitleContent(order: 0, title: "Title"),
            DescriptionContent(order: 1, description: "Description"),
            ImageContent(order: 2, imageId: 1, image: MakeImage(1)));

        var dto = SectionDto(
            order: 0,
            template: HistorySectionTemplate.SingleImageBottom,
            TitleDto(order: 0, title: "Title"),
            DescriptionDto(order: 1, description: "Description"),
            ImageDto(order: 2, imageId: 2));

        var sut = CreateSut(existingSections: [existing], images: [], saveChanges: 1);

        var result = await sut.Handle(new UpdateHistorySectionsCommand([dto]), CancellationToken.None);

        Assert.Contains(result.Errors.Select(e => e.Message), m => m.Contains("Image") && m.Contains("2"));
    }

    [Fact]
    public async Task Handle_ValidatorThrows_ReturnsValidationErrors()
    {
        var sut = new UpdateHistorySectionsHandler(
            _mapper.Object,
            _repositoryWrapper.Object,
            new UpdateHistorySectionsCommandValidator(new UpdateHistorySectionValidator()));

        var result = await sut.Handle(new UpdateHistorySectionsCommand(null!), CancellationToken.None);

        Assert.Contains(result.Errors.Select(e => e.Message), m => m.Contains(nameof(UpdateHistorySectionsCommand.UpdateSections)));
    }

    private UpdateHistorySectionsHandler CreateSut(
        IEnumerable<HistorySection>? existingSections = null,
        IEnumerable<Image>? images = null,
        int saveChanges = 1)
    {
        _repositoryWrapper.Setup(r => r.HistorySectionsRepository).Returns(_historySectionsRepository.Object);
        _repositoryWrapper.Setup(r => r.ImageRepository).Returns(_imageRepository.Object);
        _repositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(saveChanges);
        _repositoryWrapper.Setup(r => r.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));

        _historySectionsRepository
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(existingSections ?? []);

        _imageRepository
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync(images ?? []);

        _mapper
            .Setup(m => m.Map<List<HistorySectionDto>>(It.IsAny<List<HistorySection>>()))
            .Returns(new List<HistorySectionDto>());

        _validator
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateHistorySectionsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        return new UpdateHistorySectionsHandler(_mapper.Object, _repositoryWrapper.Object, _validator.Object);
    }

    private static UpdateHistorySectionDto SectionDto(
        int order,
        HistorySectionTemplate template,
        params CreateHistorySectionContentDto[] contents)
    {
        return new UpdateHistorySectionDto
        {
            Order = order,
            Template = template,
            Contents = [.. contents]
        };
    }

    private static CreateHistorySectionContentDto TitleDto(int order, string title)
        => new()
        {
            ContentType = ContentType.Title,
            Order = order,
            Title = title
        };

    private static CreateHistorySectionContentDto DescriptionDto(int order, string description)
        => new()
        {
            ContentType = ContentType.Description,
            Order = order,
            Description = description
        };

    private static CreateHistorySectionContentDto ImageDto(int order, long imageId)
        => new()
        {
            ContentType = ContentType.Image,
            Order = order,
            ImageId = imageId
        };

    private static HistorySection ExistingSection(
        long id,
        int order,
        HistorySectionTemplate template,
        params HistorySectionContent[] contents)
    {
        return new HistorySection
        {
            Id = id,
            Order = order,
            Template = template,
            CreatedAt = DateTimeOffset.UtcNow,
            Contents = [.. contents]
        };
    }

    private static TitleHistoryContent TitleContent(int order, string title)
        => new()
        {
            ContentType = ContentType.Title,
            Order = order,
            Title = title
        };

    private static DescriptionHistoryContent DescriptionContent(int order, string description)
        => new()
        {
            ContentType = ContentType.Description,
            Order = order,
            Description = description
        };

    private static ImageHistoryContent ImageContent(int order, long imageId, Image image)
        => new()
        {
            ContentType = ContentType.Image,
            Order = order,
            ImageId = imageId,
            Image = image
        };

    private static Image MakeImage(long id)
        => new()
        {
            Id = id,
            CreatedAt = DateTimeOffset.UtcNow,
            BlobName = $"blob-{id}",
            MimeType = "image/png",
            Url = "https://example.com/image.png"
        };
}
