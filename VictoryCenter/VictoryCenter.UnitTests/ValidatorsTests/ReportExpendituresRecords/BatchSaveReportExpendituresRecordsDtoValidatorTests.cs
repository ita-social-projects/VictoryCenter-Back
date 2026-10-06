using FluentValidation;
using FluentValidation.TestHelper;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.ReportExpendituresRecords;
using VictoryCenter.BLL.Validators.ReportExpendituresRecords;

namespace VictoryCenter.UnitTests.ValidatorsTests.ReportExpendituresRecords;

public class BatchSaveReportExpendituresRecordsDtoValidatorTests
{
    private readonly int _maxRecordsLimit = 10;
    private readonly BatchSaveReportExpendituresRecordsDtoValidator<TestCreateDto, TestUpdateDto> _validator;

    public BatchSaveReportExpendituresRecordsDtoValidatorTests()
    {
        var createValidatorMock = new Mock<IValidator<TestCreateDto>>();
        var updateValidatorMock = new Mock<IValidator<TestUpdateDto>>();

        _validator = new BatchSaveReportExpendituresRecordsDtoValidator<TestCreateDto, TestUpdateDto>(new()
        {
            CreateDtoValidator = createValidatorMock.Object,
            UpdateDtoValidator = updateValidatorMock.Object,
            MaxNumberOfRecordsPerBatchOperation = _maxRecordsLimit,
            UpdateIdSelector = u => u.Id,
            CreateCategoryIdSelector = c => c.CategoryId,
            UpdateCategoryIdSelector = u => u.CategoryId,
            CategoryIdPropertyName = "CategoryId",
            EntityType = typeof(TestEntity)
        });
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAllCollectionsAreEmpty()
    {
        // Arrange
        var dto = new TestBatchSaveDto
        {
            RecordsToCreate = [],
            RecordsToUpdate = [],
            RecordIdsToDelete = []
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage(ErrorMessagesConstants.BatchOperationMustContainAtLeastOneRecord);
    }

    [Theory]
    [InlineData(nameof(TestBatchSaveDto.RecordsToCreate))]
    [InlineData(nameof(TestBatchSaveDto.RecordsToUpdate))]
    [InlineData(nameof(TestBatchSaveDto.RecordIdsToDelete))]
    public void Validate_ShouldHaveError_WhenAnyBatchCollectionIsNull(string nullCollectionName)
    {
        // Arrange
        var dto = new TestBatchSaveDto
        {
            RecordsToCreate = nullCollectionName == nameof(TestBatchSaveDto.RecordsToCreate) ? null! : [],
            RecordsToUpdate = nullCollectionName == nameof(TestBatchSaveDto.RecordsToUpdate) ? null! : [],
            RecordIdsToDelete = nullCollectionName == nameof(TestBatchSaveDto.RecordIdsToDelete) ? null! : []
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(nullCollectionName)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nullCollectionName));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenCombinedRecordsTotalExceedsLimit()
    {
        // Arrange
        var dto = new TestBatchSaveDto
        {
            RecordsToCreate = [.. Enumerable.Range(1, _maxRecordsLimit).Select(_ => new TestCreateDto())],
            RecordsToUpdate = [new TestUpdateDto()],
            RecordIdsToDelete = []
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                ErrorMessagesConstants.BatchOperationTotalRecordsName, _maxRecordsLimit));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenRecordsToUpdateAndRecordIdsToDeleteContainIntersectingIds()
    {
        // Arrange
        const long intersectingId = 2;

        var dto = new TestBatchSaveDto
        {
            RecordsToUpdate = [new TestUpdateDto { Id = 1 }, new TestUpdateDto { Id = intersectingId }],
            RecordIdsToDelete = [intersectingId, 3]
        };

        var conflictingIds = new List<long> { intersectingId };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage(ErrorMessagesConstants.CannotUpdateAndDeleteSameEntity(
                conflictingIds, typeof(TestEntity)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenRecordsToUpdateHaveDuplicates()
    {
        // Arrange
        var dto = new TestBatchSaveDto
        {
            RecordsToUpdate = [new TestUpdateDto { Id = 10, CategoryId = 1 }, new TestUpdateDto { Id = 10, CategoryId = 2 }],
            RecordIdsToDelete = [1, 2, 3]
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RecordsToUpdate.Select(u => u.Id))
            .WithErrorMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(
                nameof(TestBatchSaveDto.RecordsToUpdate)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenRecordIdsToDeleteHaveDuplicates()
    {
        // Arrange
        var dto = new TestBatchSaveDto
        {
            RecordsToUpdate = [new TestUpdateDto { Id = 10 }],
            RecordIdsToDelete = [1, 2, 2, 3]
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RecordIdsToDelete)
            .WithErrorMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(
                nameof(TestBatchSaveDto.RecordIdsToDelete)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenRecordIdsToDeleteContainNonPositiveId(int invalidId)
    {
        // Arrange
        var dto = new TestBatchSaveDto
        {
            RecordsToUpdate = [new TestUpdateDto { Id = 10 }],
            RecordIdsToDelete = [1, invalidId, 3]
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RecordIdsToDelete)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive("Id"));
    }

    public class TestEntity
    {
        public int Id { get; set; }
    }

    public class TestCreateDto
    {
        public long CategoryId { get; set; }
    }

    public class TestUpdateDto
    {
        public long Id { get; set; }
        public long CategoryId { get; set; }
    }

    public class TestBatchSaveDto : IBatchSaveReportExpendituresRecordsDto<TestCreateDto, TestUpdateDto>
    {
        public List<TestCreateDto> RecordsToCreate { get; init; } = [];
        public List<TestUpdateDto> RecordsToUpdate { get; init; } = [];
        public List<long> RecordIdsToDelete { get; init; } = [];
    }
}
