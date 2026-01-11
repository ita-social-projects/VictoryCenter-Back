using AutoMapper;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Slugify;
using VictoryCenter.BLL.Commands.Admin.HippotherapyPrograms.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HippotherapyPrograms;

public class UpdateHippotherapyProgramTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IValidator<UpdateHippotherapyProgramCommand>> _validatorMock;
    private readonly Mock<ISlugHelper> _slugHelperMock;

    private readonly UpdateHippotherapyProgramDto _updateProgramDto = new()
    {
        Name = "NewProgramName",
        Description = "NewProgramDescription",
        Status = Status.Published,
        BackgroundImageId = 1,
        PreviewImageId = 2,
        CategoryIds = [1, 2],
        Sections = []
    };

    private readonly HippotherapyProgram _programEntity = new()
    {
        Id = 1,
        Name = "OldProgramName",
        Description = "OldProgramDescription",
        Status = Status.Published,
        BackgroundImageId = 1,
        PreviewImageId = 2,
        Categories = [],
        Sections = []
    };

    private readonly HippotherapyProgramDto _programDto = new()
    {
        Id = 1,
        Name = "MappedProgramName",
        Description = "MappedProgramDescription",
        Status = Status.Published,
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

    public UpdateHippotherapyProgramTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _validatorMock = new Mock<IValidator<UpdateHippotherapyProgramCommand>>();
        _slugHelperMock = new Mock<ISlugHelper>();

        SetUpValidatorAlwaysSuccess();
        SetUpAutomapper();
        SetUpRepositoryWrapper(program: _programEntity, saveResult: 1);
        SetUpSlugHelper();
    }

    [Fact]
    public async Task Handle_ShouldUpdateProgram()
    {
        var result = await ExecuteAsync(id: 1);

        Assert.True(result.IsSuccess);
        Assert.Equal(_programDto.Name, result.Value.Name);

        Assert.Equal(2, _programEntity.Categories.Count);
        _repositoryWrapperMock.Verify(r => r.HippotherapyProgramsRepository.Update(It.IsAny<HippotherapyProgram>()), Times.Once);
        _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFailUpdate_SaveFail()
    {
        SetUpRepositoryWrapper(program: _programEntity, saveResult: -1);

        var result = await ExecuteAsync(id: 1);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyProgram)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFailUpdate_NotFoundProgram()
    {
        SetUpRepositoryWrapper(program: null, saveResult: 1);

        var result = await ExecuteAsync(id: 1);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(1, typeof(HippotherapyProgram)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldUpdateProgram_WhenImagesAreNull()
    {
        _updateProgramDto.Status = Status.Draft;

        _updateProgramDto.BackgroundImageId = null;
        _updateProgramDto.PreviewImageId = null;

        var result = await ExecuteAsync(id: 1);

        Assert.True(result.IsSuccess);
        Assert.Equal(_programDto.Name, result.Value.Name);
    }

    [Fact]
    public async Task Handle_ShouldFailUpdate_WhenSomeCategoriesDoNotExist()
    {
        var onlyOneCategory = new List<HippotherapyProgramCategory>
        {
            new() { Id = 1, Name = "OnlyExistingCategory" }
        };

        _repositoryWrapperMock
            .Setup(r => r.HippotherapyProgramCategoriesRepository.GetAllAsync(It.IsAny<QueryOptions<HippotherapyProgramCategory>>()))
            .ReturnsAsync(onlyOneCategory);

        var result = await ExecuteAsync(id: 1);

        Assert.True(result.IsFailed);
        Assert.Contains(nameof(HippotherapyProgramCategory), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFailUpdate_WhenBackgroundImageNotFound()
    {
        _updateProgramDto.BackgroundImageId = 999;

        var result = await ExecuteAsync(id: 1);

        Assert.False(result.IsSuccess);
        Assert.Contains("Image", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFailUpdate_WhenPreviewImageNotFound()
    {
        _updateProgramDto.PreviewImageId = 999;

        var result = await ExecuteAsync(id: 1);

        Assert.False(result.IsSuccess);
        Assert.Contains("Image", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFailUpdate_WhenSomeSectionImagesDoNotExist()
    {
        _updateProgramDto.Status = Status.Draft;
        _updateProgramDto.BackgroundImageId = null;
        _updateProgramDto.PreviewImageId = null;

        _updateProgramDto.Sections =
        [
            new CreateHippotherapyProgramSectionDto
            {
                Template = default,
                Order = 0,
                ImageIds = [1, 2]
            }

        ];

        _repositoryWrapperMock
            .Setup(r => r.ImageRepository.GetAllAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync([_images[0]]);

        var result = await ExecuteAsync(id: 1);

        Assert.False(result.IsSuccess);
        Assert.Contains("Image", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFailUpdate_WhenSlugAlreadyExists()
    {
        _updateProgramDto.Name = "New Program Name";

        var existingProgramWithSlug = new HippotherapyProgram
        {
            Id = 999,
            Name = "Existing Program",
            Slug = "new-program-name"
        };

        _repositoryWrapperMock
            .Setup(r => r.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(
                It.Is<QueryOptions<HippotherapyProgram>>(opt => opt.AsNoTracking == false)))
            .ReturnsAsync(_programEntity);

        _repositoryWrapperMock
            .Setup(r => r.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(
                It.Is<QueryOptions<HippotherapyProgram>>(opt => opt.AsNoTracking == true)))
            .ReturnsAsync(existingProgramWithSlug);

        var result = await ExecuteAsync(id: 1);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.PropertyMustBeUnique(nameof(HippotherapyProgram.Slug)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldUpdateProgram_WhenSlugDoesNotExist()
    {
        _updateProgramDto.Name = "Unique New Program Name";

        _repositoryWrapperMock
            .Setup(r => r.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(
                It.Is<QueryOptions<HippotherapyProgram>>(opt => opt.AsNoTracking == true)))
            .ReturnsAsync((HippotherapyProgram?)null);

        var result = await ExecuteAsync(id: 1);

        Assert.True(result.IsSuccess);
        Assert.Equal(_programDto.Name, result.Value.Name);
    }

    private Task<Result<HippotherapyProgramDto>> ExecuteAsync(long id)
    {
        var handler = CreateHandler();
        return handler.Handle(CreateCommand(id), CancellationToken.None);
    }

    private UpdateHippotherapyProgramCommand CreateCommand(long id)
        => new(_updateProgramDto, id);

    private UpdateHippotherapyProgramHandler CreateHandler()
        => new(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            _validatorMock.Object,
            _slugHelperMock.Object);

    private void SetUpAutomapper()
    {
        _mapperMock
            .Setup(m => m.Map(It.IsAny<UpdateHippotherapyProgramDto>(), It.IsAny<HippotherapyProgram>()))
            .Returns((UpdateHippotherapyProgramDto src, HippotherapyProgram dest) =>
            {
                dest.Name = src.Name;
                dest.Description = src.Description;
                dest.Status = src.Status;
                dest.BackgroundImageId = src.BackgroundImageId;
                dest.PreviewImageId = src.PreviewImageId;
                return dest;
            });

        _mapperMock
            .Setup(m => m.Map<HippotherapyProgramDto>(It.IsAny<HippotherapyProgram>()))
            .Returns(_programDto);
    }

    private void SetUpValidatorAlwaysSuccess()
    {
        _validatorMock.Reset();

        var ok = new ValidationResult();

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateHippotherapyProgramCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ok);

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<UpdateHippotherapyProgramCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ok);
    }

    private void SetUpRepositoryWrapper(HippotherapyProgram? program, int saveResult)
    {
        _repositoryWrapperMock
            .Setup(r => r.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(
                It.Is<QueryOptions<HippotherapyProgram>>(opt => opt.AsNoTracking == false)))
            .ReturnsAsync(program);

        _repositoryWrapperMock
            .Setup(r => r.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(
                It.Is<QueryOptions<HippotherapyProgram>>(opt => opt.AsNoTracking == true)))
            .ReturnsAsync((HippotherapyProgram?)null);

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
            .Setup(r => r.HippotherapyProgramsRepository.Update(It.IsAny<HippotherapyProgram>()));

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
