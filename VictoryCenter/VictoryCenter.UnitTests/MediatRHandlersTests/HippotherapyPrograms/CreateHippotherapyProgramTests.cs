using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.HippotherapyPrograms.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;
using VictoryCenter.BLL.Interfaces.SlugService;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HippotherapyPrograms;

public class CreateHippotherapyProgramTests
{
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IRepositoryWrapper> _repo = new();
    private readonly Mock<IValidator<CreateHippotherapyProgramCommand>> _validator = new();
    private readonly Mock<ISlugService> _slugService = new();

    private static readonly List<HippotherapyProgramCategory> Categories =
    [
        new() { Id = 1, Name = "C1" },
        new() { Id = 2, Name = "C2" }
    ];

    private static readonly List<Image> Images =
    [
        new() { Id = 1, BlobName = "B1", MimeType = "image/png" },
        new() { Id = 2, BlobName = "B2", MimeType = "image/png" }
    ];

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var (sut, _) = CreateSut(saveChanges: 1);

        var result = await sut.Handle(Command(Dto()), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ValidRequest_CallsSlugService()
    {
        var (sut, _) = CreateSut(saveChanges: 1);

        await sut.Handle(Command(Dto(name: "Test Name")), CancellationToken.None);

        _slugService.Verify(
            s => s.GenerateUniqueHippotherapyProgramSlugAsync(
                It.IsAny<long>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_SaveChangesReturnsZero_ReturnsFailedToCreateEntity()
    {
        var (sut, _) = CreateSut(saveChanges: 0);

        var result = await sut.Handle(Command(Dto()), CancellationToken.None);

        Assert.Contains(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyProgram)),
            string.Join(" | ", result.Errors.Select(e => e.Message)));
    }

    [Fact]
    public async Task Handle_MissingCategory_ReturnsNotFoundError()
    {
        var (sut, _) = CreateSut(saveChanges: 1, categories: [Categories[0]]);

        var result = await sut.Handle(Command(Dto(categoryIds: [1, 2])), CancellationToken.None);

        Assert.Contains(
            nameof(HippotherapyProgramCategory),
            string.Join(" | ", result.Errors.Select(e => e.Message)));
    }

    [Fact]
    public async Task Handle_BackgroundImageNotFound_ReturnsNotFoundError()
    {
        var (sut, _) = CreateSut(saveChanges: 1);

        var result = await sut.Handle(Command(Dto(backgroundImageId: 999)), CancellationToken.None);

        Assert.Contains(
            nameof(Image),
            string.Join(" | ", result.Errors.Select(e => e.Message)));
    }

    [Fact]
    public async Task Handle_PreviewImageNotFound_ReturnsNotFoundError()
    {
        var (sut, _) = CreateSut(saveChanges: 1);

        var result = await sut.Handle(Command(Dto(previewImageId: 999)), CancellationToken.None);

        Assert.Contains(
            nameof(Image),
            string.Join(" | ", result.Errors.Select(e => e.Message)));
    }

    [Fact]
    public async Task Handle_SectionImageNotFound_ReturnsNotFoundError()
    {
        var (sut, _) = CreateSut(saveChanges: 1, images: [Images[0]]);

        var dto = Dto(
            backgroundImageId: null,
            previewImageId: null,
            sections:
            [
                Section(
                    0,
                    ImageContent(0, 1),
                    ImageContent(1, 2))
            ]);

        var result = await sut.Handle(Command(dto), CancellationToken.None);

        Assert.Contains(
            nameof(Image),
            string.Join(" | ", result.Errors.Select(e => e.Message)));
    }

    [Fact]
    public async Task Handle_ImagesAreNull_ReturnsSuccess()
    {
        var (sut, _) = CreateSut(saveChanges: 1);

        var result = await sut.Handle(Command(Dto(backgroundImageId: null, previewImageId: null)), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_SaveChangesThrowsDbUpdateException_ReturnsDatabaseError()
    {
        var (sut, _) = CreateSut(saveChanges: 1, throwOnSave: true);

        var result = await sut.Handle(Command(Dto()), CancellationToken.None);

        Assert.Contains(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(HippotherapyProgram)),
            string.Join(" | ", result.Errors.Select(e => e.Message)));
    }

    [Fact]
    public async Task Handle_NameIsDuplicate_SetsUniqueSlug()
    {
        var (sut, program) = CreateSut(saveChanges: 1, slug: "testname-1");

        await sut.Handle(Command(Dto(name: "TestName")), CancellationToken.None);

        Assert.Equal("testname-1", program.Slug);
    }

    private (CreateHippotherapyProgramHandler sut, HippotherapyProgram program) CreateSut(
        int saveChanges,
        List<HippotherapyProgramCategory>? categories = null,
        List<Image>? images = null,
        bool throwOnSave = false,
        string? slug = null)
    {
        var program = new HippotherapyProgram
        {
            Id = 1,
            Categories = [],
            Sections = []
        };

        SetUpValidatorSuccess();
        SetUpMapper(program);
        SetUpRepositories(saveChanges, categories ?? Categories, images ?? Images, throwOnSave);
        SetUpSlugService(slug);

        return (new CreateHippotherapyProgramHandler(_mapper.Object, _repo.Object, _validator.Object, _slugService.Object), program);
    }

    private void SetUpValidatorSuccess()
    {
        _validator.Reset();

        var ok = new ValidationResult();

        _validator
            .Setup(v => v.ValidateAsync(It.IsAny<CreateHippotherapyProgramCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ok);

        _validator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateHippotherapyProgramCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ok);
    }

    private void SetUpMapper(HippotherapyProgram program)
    {
        _mapper.Reset();

        _mapper
            .Setup(m => m.Map<HippotherapyProgram>(It.IsAny<CreateHippotherapyProgramDto>()))
            .Returns((CreateHippotherapyProgramDto dto) =>
            {
                program.Name = dto.Name;
                program.Description = dto.Description;
                program.Status = dto.Status;
                program.Location = dto.Location;
                program.ParticipantsCount = dto.ParticipantsCount;
                program.MeetingsCount = dto.MeetingsCount;
                program.BackgroundImageId = dto.BackgroundImageId;
                program.PreviewImageId = dto.PreviewImageId;
                program.Categories = [];
                program.Sections = [];
                return program;
            });

        _mapper
            .Setup(m => m.Map<HippotherapyProgramDto>(It.IsAny<HippotherapyProgram>()))
            .Returns((HippotherapyProgram p) => new HippotherapyProgramDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Status = p.Status,
                Location = p.Location,
                ParticipantsCount = p.ParticipantsCount,
                MeetingsCount = p.MeetingsCount,
                BackgroundImage = null,
                PreviewImage = null,
                Categories = [],
                Sections = []
            });
    }

    private void SetUpRepositories(
        int saveChanges,
        List<HippotherapyProgramCategory> categories,
        List<Image> images,
        bool throwOnSave)
    {
        _repo.Reset();

        _repo
            .Setup(r => r.HippotherapyProgramCategoriesRepository.GetAllAsync(It.IsAny<QueryOptions<HippotherapyProgramCategory>>()))
            .ReturnsAsync((QueryOptions<HippotherapyProgramCategory> options) =>
            {
                var predicate = options.Filter?.Compile();
                return predicate is null ? categories : [.. categories.Where(predicate)];
            });

        _repo
            .Setup(r => r.ImageRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync((QueryOptions<Image> options) =>
            {
                var predicate = options.Filter?.Compile();
                return predicate is null ? images.FirstOrDefault() : images.FirstOrDefault(predicate);
            });

        _repo
            .Setup(r => r.ImageRepository.GetAllAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync((QueryOptions<Image> options) =>
            {
                var predicate = options.Filter?.Compile();
                return predicate is null ? images : [.. images.Where(predicate)];
            });

        _repo
            .Setup(r => r.HippotherapyProgramsRepository.CreateAsync(It.IsAny<HippotherapyProgram>()));

        if (throwOnSave)
        {
            _repo.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new DbUpdateException());
            return;
        }

        _repo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(saveChanges);
    }

    private void SetUpSlugService(string? uniqueSlug)
    {
        _slugService.Reset();

        _slugService
            .Setup(s => s.GenerateUniqueHippotherapyProgramSlugAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((long _, string name, CancellationToken __) =>
                uniqueSlug ?? name.ToLowerInvariant().Replace(" ", "-"));

        _slugService
            .Setup(s => s.GenerateSlug(It.IsAny<string>()))
            .Returns((string input) => input.ToLowerInvariant().Replace(" ", "-"));
    }

    private static CreateHippotherapyProgramCommand Command(CreateHippotherapyProgramDto dto) => new(dto);

    private static CreateHippotherapyProgramDto Dto(
        string name = "N",
        string description = "D",
        Status status = Status.Draft,
        long? backgroundImageId = 1,
        long? previewImageId = 2,
        List<long>? categoryIds = null,
        List<CreateHippotherapyProgramSectionDto>? sections = null)
    {
        return new CreateHippotherapyProgramDto
        {
            Name = name,
            Description = description,
            Status = status,
            BackgroundImageId = backgroundImageId,
            PreviewImageId = previewImageId,
            CategoryIds = categoryIds ?? [1, 2],
            Sections = sections ?? []
        };
    }

    private static CreateHippotherapyProgramSectionDto Section(int order, params CreateProgramSectionContentDto[] contents)
    {
        return new CreateHippotherapyProgramSectionDto
        {
            Template = default,
            Order = order,
            Contents = [.. contents]
        };
    }

    private static CreateProgramSectionContentDto ImageContent(int order, long imageId)
    {
        return new CreateProgramSectionContentDto
        {
            ContentType = ContentType.Image,
            Order = order,
            ImageId = imageId
        };
    }
}
