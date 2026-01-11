using AutoMapper;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using Slugify;
using VictoryCenter.BLL.Commands.Admin.HippotherapyPrograms.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HippotherapyPrograms;

public class CreateHippotherapyProgramTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IValidator<CreateHippotherapyProgramCommand>> _validatorMock;
    private readonly Mock<ISlugHelper> _slugHelperMock;

    private readonly CreateHippotherapyProgramDto _createProgramDto = new()
    {
        Name = "TestName",
        Description = "TestDescription",
        Status = Status.Draft,
        BackgroundImageId = 1,
        PreviewImageId = 2,
        CategoryIds = [1, 2],
        Sections = []
    };

    private readonly HippotherapyProgram _programEntity = new()
    {
        Name = "TestName",
        Description = "TestDescription",
        Status = Status.Draft,
        BackgroundImageId = 1,
        PreviewImageId = 2,
        Categories = [],
        Sections = []
    };

    private readonly HippotherapyProgramDto _programDto = new()
    {
        Id = 1,
        Name = "TestName",
        Description = "TestDescription",
        Status = Status.Draft,
        Location = null,
        ParticipantsCount = null,
        MeetingsCount = null,
        BackgroundImage = new ImageDto { BlobName = "BlobName", MimeType = "image/png" },
        PreviewImage = new ImageDto { BlobName = "BlobName", MimeType = "image/png" },
        Categories = []
    };

    private readonly List<HippotherapyProgramCategory> _programCategories =
    [
        new() { Id = 1, Name = "TestCategoryName1" },
        new() { Id = 2, Name = "TestCategoryName2" }
    ];

    private readonly List<Image> _images =
    [
        new() { Id = 1, BlobName = "BlobName1", MimeType = "image/png" },
        new() { Id = 2, BlobName = "BlobName2", MimeType = "image/png" }
    ];

    public CreateHippotherapyProgramTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _validatorMock = new Mock<IValidator<CreateHippotherapyProgramCommand>>();
        _slugHelperMock = new Mock<ISlugHelper>();

        SetUpValidatorAlwaysSuccess();
        SetUpAutomapper();
        SetUpRepositoryWrapper(saveResult: 1);
        SetUpSlugHelper();
    }

    [Fact]
    public async Task Handle_ShouldCreateProgram()
    {
        var result = await ExecuteAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(_programEntity.Name, result.Value.Name);
    }

    [Fact]
    public async Task Handle_ShouldFail_SaveChangesFail()
    {
        SetUpRepositoryWrapper(saveResult: -1);

        var result = await ExecuteAsync();

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyProgram)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSomeCategoriesDoNotExist()
    {
        var onlyOneCategory = new List<HippotherapyProgramCategory>
        {
            new() { Id = 1, Name = "OnlyExistingCategory" }
        };

        _repositoryWrapperMock
            .Setup(r => r.HippotherapyProgramCategoriesRepository.GetAllAsync(It.IsAny<QueryOptions<HippotherapyProgramCategory>>()))
            .ReturnsAsync(onlyOneCategory);

        var result = await ExecuteAsync();

        Assert.False(result.IsSuccess);
        Assert.Contains("HippotherapyProgramCategory", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldCreateProgram_WhenImagesAreNull()
    {
        _createProgramDto.BackgroundImageId = null;
        _createProgramDto.PreviewImageId = null;

        _programEntity.BackgroundImageId = null;
        _programEntity.PreviewImageId = null;

        var result = await ExecuteAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(_programDto.Name, result.Value.Name);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenBackgroundImageNotFound()
    {
        _createProgramDto.BackgroundImageId = 999;
        _programEntity.BackgroundImageId = 999;

        var result = await ExecuteAsync();

        Assert.False(result.IsSuccess);
        Assert.Contains("Image", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenPreviewImageNotFound()
    {
        _createProgramDto.PreviewImageId = 999;
        _programEntity.PreviewImageId = 999;

        var result = await ExecuteAsync();

        Assert.False(result.IsSuccess);
        Assert.Contains("Image", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSomeSectionImagesDoNotExist()
    {
        _createProgramDto.BackgroundImageId = null;
        _createProgramDto.PreviewImageId = null;

        _programEntity.BackgroundImageId = null;
        _programEntity.PreviewImageId = null;

        _createProgramDto.Sections =
        [
            new()
            {
                Template = default,
                Order = 0,
                ImageIds = [1, 2]
            }

        ];

        _repositoryWrapperMock
            .Setup(r => r.ImageRepository.GetAllAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync([_images[0]]);

        var result = await ExecuteAsync();

        Assert.False(result.IsSuccess);
        Assert.Contains("Image", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_DbUpdateException()
    {
        _repositoryWrapperMock
            .Setup(r => r.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateException());

        var result = await ExecuteAsync();

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(HippotherapyProgram)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSlugAlreadyExists()
    {
        var existingProgramWithSlug = new HippotherapyProgram
        {
            Id = 999,
            Name = "Existing Program",
            Slug = "testname"
        };

        _repositoryWrapperMock
            .Setup(r => r.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(
                It.Is<QueryOptions<HippotherapyProgram>>(opt =>
                    opt.Filter != null &&
                    opt.AsNoTracking == true)))
            .ReturnsAsync((QueryOptions<HippotherapyProgram> options) =>
            {
                var predicate = options.Filter?.Compile();
                if (predicate is null)
                {
                    return null;
                }

                return predicate(existingProgramWithSlug) ? existingProgramWithSlug : null;
            });

        var result = await ExecuteAsync();

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.PropertyMustBeInAValidFormat(nameof(HippotherapyProgram.Slug)), result.Errors[0].Message);
    }

    private Task<Result<HippotherapyProgramDto>> ExecuteAsync()
    {
        var handler = CreateHandler();
        return handler.Handle(CreateCommand(), CancellationToken.None);
    }

    private CreateHippotherapyProgramCommand CreateCommand()
        => new(_createProgramDto);

    private CreateHippotherapyProgramHandler CreateHandler()
        => new(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            _validatorMock.Object,
            _slugHelperMock.Object);

    private void SetUpAutomapper()
    {
        _mapperMock
            .Setup(m => m.Map<HippotherapyProgram>(It.IsAny<CreateHippotherapyProgramDto>()))
            .Returns(_programEntity);

        _mapperMock
            .Setup(m => m.Map<HippotherapyProgramDto>(It.IsAny<HippotherapyProgram>()))
            .Returns(_programDto);
    }

    private void SetUpValidatorAlwaysSuccess()
    {
        _validatorMock.Reset();

        var ok = new ValidationResult();

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateHippotherapyProgramCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ok);

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateHippotherapyProgramCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ok);
    }

    private void SetUpRepositoryWrapper(int saveResult)
    {
        _repositoryWrapperMock
            .Setup(r => r.HippotherapyProgramCategoriesRepository.GetAllAsync(It.IsAny<QueryOptions<HippotherapyProgramCategory>>()))
            .ReturnsAsync((QueryOptions<HippotherapyProgramCategory> options) =>
            {
                var predicate = options.Filter?.Compile();
                return predicate is null
                    ? _programCategories
                    : [.. _programCategories.Where(predicate)];
            });

        _repositoryWrapperMock
            .Setup(r => r.ImageRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync((QueryOptions<Image> options) =>
            {
                var predicate = options.Filter?.Compile();
                return predicate is null
                    ? _images.FirstOrDefault()
                    : _images.FirstOrDefault(predicate);
            });

        _repositoryWrapperMock
            .Setup(r => r.ImageRepository.GetAllAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync((QueryOptions<Image> options) =>
            {
                var predicate = options.Filter?.Compile();
                return predicate is null
                    ? _images
                    : [.. _images.Where(predicate)];
            });

        _repositoryWrapperMock
            .Setup(r => r.HippotherapyProgramsRepository.CreateAsync(It.IsAny<HippotherapyProgram>()));

        _repositoryWrapperMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(saveResult);
    }

    private void SetUpSlugHelper()
    {
        _slugHelperMock
            .Setup(s => s.GenerateSlug(It.IsAny<string>()))
            .Returns((string input) => input.ToLowerInvariant().Replace(" ", "-"));
    }
}
