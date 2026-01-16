using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using VictoryCenter.BLL.Commands.Admin.HippotherapyPrograms.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HippotherapyPrograms;

public class UpdateHippotherapyProgramTests
{
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IRepositoryWrapper> _repo = new();
    private readonly Mock<IValidator<UpdateHippotherapyProgramCommand>> _validator = new();

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
    public async Task Handle_ValidRequest_UpdatesProgramName()
    {
        var program = Program();
        var sut = CreateSut(program: program, saveChanges: 1);

        await sut.Handle(Command(id: 1, dto: Dto(name: "NewName")), CancellationToken.None);

        Assert.Equal("NewName", program.Name);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReplacesCategories()
    {
        var program = Program(categories: [new HippotherapyProgramCategory { Id = 99, Name = "Old" }]);
        var sut = CreateSut(program: program, saveChanges: 1);

        await sut.Handle(Command(id: 1, dto: Dto(categoryIds: [1, 2])), CancellationToken.None);

        Assert.Equal(2, program.Categories.Count);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReplacesSections()
    {
        var program = Program(sections: [new HippotherapyProgramSection { Template = default, Order = 99, CreatedAt = DateTimeOffset.UtcNow, Contents = [] }]);
        var sut = CreateSut(program: program, saveChanges: 1);

        await sut.Handle(
            Command(id: 1, dto: Dto(sections:
            [
                CreateSection(0),
                CreateSection(1)
            ])), CancellationToken.None);

        Assert.Equal(2, program.Sections.Count);
    }

    [Fact]
    public async Task Handle_SaveChangesReturnsZero_ReturnsFailedToUpdateEntity()
    {
        var sut = CreateSut(program: Program(), saveChanges: 0);

        var result = await sut.Handle(Command(id: 1, dto: Dto()), CancellationToken.None);

        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyProgram)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ProgramNotFound_ReturnsNotFound()
    {
        var sut = CreateSut(program: null, saveChanges: 1);

        var result = await sut.Handle(Command(id: 1, dto: Dto()), CancellationToken.None);

        Assert.Equal(ErrorMessagesConstants.NotFound(1, typeof(HippotherapyProgram)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_MissingCategory_ReturnsNotFoundError()
    {
        var sut = CreateSut(program: Program(), saveChanges: 1, categories: [Categories[0]]);

        var result = await sut.Handle(Command(id: 1, dto: Dto(categoryIds: [1, 2])), CancellationToken.None);

        Assert.Contains(nameof(HippotherapyProgramCategory), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_BackgroundImageNotFound_ReturnsNotFoundError()
    {
        var sut = CreateSut(program: Program(), saveChanges: 1);

        var result = await sut.Handle(Command(id: 1, dto: Dto(backgroundImageId: 999)), CancellationToken.None);

        Assert.Contains(nameof(Image), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_PreviewImageNotFound_ReturnsNotFoundError()
    {
        var sut = CreateSut(program: Program(), saveChanges: 1);

        var result = await sut.Handle(Command(id: 1, dto: Dto(previewImageId: 999)), CancellationToken.None);

        Assert.Contains(nameof(Image), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_SectionImageNotFound_ReturnsNotFoundError()
    {
        var sut = CreateSut(program: Program(), saveChanges: 1, images: [Images[0]]);

        var dto = Dto(
            status: Status.Draft,
            backgroundImageId: null,
            previewImageId: null,
            sections:
            [
                CreateSection(
                    0,
                    CreateImageContent(0, 1),
                    CreateImageContent(1, 2))
            ]);

        var result = await sut.Handle(Command(id: 1, dto: dto), CancellationToken.None);

        Assert.Contains(nameof(Image), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ImagesAreNull_ReturnsSuccess()
    {
        var sut = CreateSut(program: Program(), saveChanges: 1);

        var result = await sut.Handle(Command(id: 1, dto: Dto(backgroundImageId: null, previewImageId: null)), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ValidatorThrows_ReturnsValidationError()
    {
        var sut = CreateSut(program: Program(), saveChanges: 1);
        SetUpValidatorToThrow("Name required");

        var result = await sut.Handle(Command(id: 1, dto: Dto()), CancellationToken.None);

        Assert.Equal("Name required", result.Errors[0].Message);
    }

    private UpdateHippotherapyProgramHandler CreateSut(
        HippotherapyProgram? program,
        int saveChanges,
        List<HippotherapyProgramCategory>? categories = null,
        List<Image>? images = null,
        List<ValidationFailure>? validatorErrors = null)
    {
        SetUpValidator(validatorErrors);
        SetUpMapper();
        SetUpRepositories(program, saveChanges, categories ?? Categories, images ?? Images);

        return new UpdateHippotherapyProgramHandler(_mapper.Object, _repo.Object, _validator.Object);
    }

    private void SetUpValidator(List<ValidationFailure>? failures)
    {
        _validator.Reset();

        var result = failures is null
            ? new ValidationResult()
            : new ValidationResult(failures);

        _validator
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateHippotherapyProgramCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        _validator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<UpdateHippotherapyProgramCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);
    }

    private void SetUpMapper()
    {
        _mapper.Reset();

        _mapper
            .Setup(m => m.Map(It.IsAny<UpdateHippotherapyProgramDto>(), It.IsAny<HippotherapyProgram>()))
            .Returns((UpdateHippotherapyProgramDto src, HippotherapyProgram dest) =>
            {
                dest.Name = src.Name;
                dest.Description = src.Description;
                dest.Status = src.Status;
                dest.Location = src.Location;
                dest.ParticipantsCount = src.ParticipantsCount;
                dest.MeetingsCount = src.MeetingsCount;
                dest.BackgroundImageId = src.BackgroundImageId;
                dest.PreviewImageId = src.PreviewImageId;
                return dest;
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
        HippotherapyProgram? program,
        int saveChanges,
        List<HippotherapyProgramCategory> categories,
        List<Image> images)
    {
        _repo.Reset();

        _repo
            .Setup(r => r.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgram>>()))
            .ReturnsAsync(program);

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
            .Setup(r => r.HippotherapyProgramsRepository.Update(It.IsAny<HippotherapyProgram>()));

        _repo
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(saveChanges);
    }

    private void SetUpValidatorToThrow(string message)
    {
        _validator.Reset();

        var failures = new List<ValidationFailure>
        {
            new("UpdateProgramDto.Name", message)
        };

        var ex = new ValidationException(failures);

        _validator
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateHippotherapyProgramCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(ex);

        _validator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<UpdateHippotherapyProgramCommand>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(ex);
    }

    private static UpdateHippotherapyProgramCommand Command(long id, UpdateHippotherapyProgramDto dto) => new(dto, id);

    private static UpdateHippotherapyProgramDto Dto(
        string name = "Name",
        string description = "Description",
        Status status = Status.Published,
        long? backgroundImageId = 1,
        long? previewImageId = 2,
        List<long>? categoryIds = null,
        List<CreateHippotherapyProgramSectionDto>? sections = null)
    {
        return new UpdateHippotherapyProgramDto
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

    private static HippotherapyProgram Program(
        ICollection<HippotherapyProgramCategory>? categories = null,
        ICollection<HippotherapyProgramSection>? sections = null)
    {
        return new HippotherapyProgram
        {
            Id = 1,
            Name = "Old",
            Description = "OldDesc",
            Status = Status.Published,
            BackgroundImageId = 1,
            PreviewImageId = 2,
            Categories = categories ?? [],
            Sections = sections ?? []
        };
    }

    private static CreateHippotherapyProgramSectionDto CreateSection(int order, params CreateProgramSectionContentDto[] contents)
    {
        return new CreateHippotherapyProgramSectionDto
        {
            Template = default,
            Order = order,
            Contents = [.. contents]
        };
    }

    private static CreateProgramSectionContentDto CreateImageContent(int order, long imageId)
    {
        return new CreateProgramSectionContentDto
        {
            ContentType = ContentType.Image,
            Order = order,
            ImageId = imageId
        };
    }
}
