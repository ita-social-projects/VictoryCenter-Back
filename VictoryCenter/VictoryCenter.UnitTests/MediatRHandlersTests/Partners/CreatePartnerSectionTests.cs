using System.Linq.Expressions;
using System.Transactions;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Partners.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.BLL.Validators.Partners.Commands;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Partners;

public class CreatePartnersSectionTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly IValidator<CreatePartnersSectionCommand> _validator;
    private readonly Mock<IReorderService> _mockReorderService;

    // Sample DTO for the request
    private readonly CreatePartnersSectionDto _createDto = new()
    {
        Title = "New Partner Section",
        Description = "A valid description.", // ✅ Додано валідний опис
        Partners =
        [
            new() { Description = "Partner 1", ImageId = 1 },
            new() { Description = "Partner 2", ImageId = 2 }
        ]
    };

    // Sample entity returned by the mapper
    private readonly PartnerSection _sectionEntity = new()
    {
        Id = 1,
        Title = "New Partner Section",
        Partners =
        [
            new() { Description = "Partner 1", ImageId = 1 },
            new() { Description = "Partner 2", ImageId = 2 }
        ]
    };

    // Sample DTO for the response
    private readonly PartnersSectionDto _resultDto = new()
    {
        Id = 1,
        Title = "New Partner Section",
    };

    // Sample existing images in the DB
    private readonly List<Image> _existingImages =
    [
        new() { Id = 1 },
        new() { Id = 2 }
    ];

    public CreatePartnersSectionTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockReorderService = new Mock<IReorderService>();
        _validator = new CreatePartnersSectionCommandValidator(); // ✅ Створюємо екземпляр реального валідатора
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldCreateSectionAndReturnOk()
    {
        // Arrange
        SetupDependencies(_sectionEntity, _resultDto);
        var command = new CreatePartnersSectionCommand(_createDto);
        var handler = new CreatePartnersSectionHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _validator, _mockReorderService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_resultDto, result.Value);
        _mockRepositoryWrapper.Verify(r => r.PartnerSectionsRepository.CreateAsync(It.IsAny<PartnerSection>()), Times.Once);
        _mockRepositoryWrapper.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingImageId_ShouldReturnFailure()
    {
        // Arrange
        var dtoWithInvalidImage = _createDto with
        {
            Partners = [new CreatePartnerDto { Description = "Partner 3", ImageId = 99 }]
        };

        SetupDependencies(_sectionEntity, _resultDto);
        _mockRepositoryWrapper
            .Setup(r => r.ImageRepository.GetAllAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync([]);

        var command = new CreatePartnersSectionCommand(dtoWithInvalidImage);
        var handler = new CreatePartnersSectionHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _validator, _mockReorderService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.NotFound([99L], typeof(Image)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_InvalidData_ShouldReturnValidationFailure()
    {
        // Arrange
        var invalidDto = new CreatePartnersSectionDto
        {
            Title = "Valid Title",
            Description = new string('A', PartnerConstants.PartnersSectionDescriptionMaxLength + 1),
            Partners = [new() { Description = "Partner 1", ImageId = 1 }]
        };

        var command = new CreatePartnersSectionCommand(invalidDto);
        var handler = new CreatePartnersSectionHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _validator, _mockReorderService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);

        Assert.Contains(
            ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreatePartnersSectionDto.Description), PartnerConstants.PartnersSectionDescriptionMaxLength),
            result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_DbUpdateException_ShouldReturnFailure()
    {
        // Arrange
        SetupDependencies(_sectionEntity, _resultDto);
        _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new DbUpdateException());

        var command = new CreatePartnersSectionCommand(_createDto);
        var handler = new CreatePartnersSectionHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _validator, _mockReorderService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(PartnerSection)), result.Errors[0].Message);
    }

    private void SetupDependencies(PartnerSection sectionEntity, PartnersSectionDto resultDto)
    {
        // ❌ Метод SetupValidator більше не потрібен
        SetupMapper(sectionEntity, resultDto);
        SetupRepositoryWrapper(sectionEntity);
        SetupReorderService();
    }

    private void SetupMapper(PartnerSection entityToReturn, PartnersSectionDto dtoToReturn)
    {
        _mockMapper.Setup(m => m.Map<PartnerSection>(It.IsAny<CreatePartnersSectionDto>()))
            .Returns(entityToReturn);
        _mockMapper.Setup(m => m.Map<PartnersSectionDto>(It.IsAny<PartnerSection>()))
            .Returns(dtoToReturn);
    }

    private void SetupReorderService()
    {
        _mockReorderService.Setup(s => s.GetNextDisplayOrderAsync<PartnerSection>(It.IsAny<Expression<Func<PartnerSection, bool>>>()))
            .ReturnsAsync(1L);
    }

    private void SetupRepositoryWrapper(PartnerSection createdEntity)
    {
        _mockRepositoryWrapper.Setup(r => r.ImageRepository.GetAllAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync(_existingImages);

        _mockRepositoryWrapper.Setup(r => r.PartnerSectionsRepository.CreateAsync(It.IsAny<PartnerSection>()))
            .ReturnsAsync(createdEntity);

        _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        _mockRepositoryWrapper.Setup(r => r.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));

        _mockRepositoryWrapper.Setup(r => r.PartnerSectionsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PartnerSection>>()))
            .ReturnsAsync(createdEntity);
    }
}
