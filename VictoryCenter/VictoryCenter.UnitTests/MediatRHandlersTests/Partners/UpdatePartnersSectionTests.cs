using System.Linq.Expressions;
using System.Transactions;
using AutoMapper;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Partners.UpdateSection;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.BLL.Validators.Partners.Commands;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Partners;

public class UpdatePartnersSectionHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IReorderService> _mockReorderService;
    private readonly IValidator<UpdatePartnersSectionCommand> _validator;

    private static readonly List<Image> _availableImages =
    [
        new() { Id = 100, BlobName = "image1.jpg", MimeType = "image/jpeg" },
        new() { Id = 200, BlobName = "image2.png", MimeType = "image/png" },
        new() { Id = 300, BlobName = "image3.gif", MimeType = "image/gif" }
    ];

    public UpdatePartnersSectionHandlerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockReorderService = new Mock<IReorderService>();
        _validator = new UpdatePartnersSectionCommandValidator();
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldUpdateSectionAndPartners()
    {
        // Arrange
        var existingSection = GetExistingSection();
        var updateDto = new UpdatePartnersSectionDto
        {
            Title = "New Valid Title",
            Description = "New Valid Description",
            PartnerIdsToDelete = [10],
            PartnersToUpdate = [new() { Id = 20, Description = "Updated Partner Desc", ImageId = 200 }],
            PartnersToCreate = [new() { Description = "New Partner Desc", ImageId = 300 }]
        };

        SetUpDependencies(existingSection);

        var handler = new UpdatePartnersSectionHandler(
            _mockRepositoryWrapper.Object, _mockMapper.Object, _validator, _mockReorderService.Object);
        var command = new UpdatePartnersSectionCommand(updateDto, existingSection.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockRepositoryWrapper.Verify(r => r.PartnerRepository.DeleteRange(It.Is<List<Partner>>(list => list.Any(p => p.Id == 10))), Times.Once);
        _mockMapper.Verify(m => m.Map(It.Is<UpdatePartnerDto>(dto => dto.Id == 20), It.Is<Partner>(p => p.Id == 20)), Times.Once);
        _mockMapper.Verify(m => m.Map<Partner>(It.Is<CreatePartnerDto>(dto => dto.Description == "New Partner Desc")), Times.Once);
        _mockReorderService.Verify(s => s.RenumberPriorityAsync<Partner>(It.IsAny<Expression<Func<Partner, bool>>>()), Times.Once);
        _mockRepositoryWrapper.Verify(r => r.SaveChangesAsync(), Times.Exactly(1));

        var createdPartner = existingSection.Partners
            .FirstOrDefault(p => p.Description == "New Partner Desc");

        Assert.NotNull(createdPartner);
        Assert.Equal(3, createdPartner.Priority);
    }

    [Fact]
    public async Task Handle_WhenSectionNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        SetUpDependencies(null);
        var updateDto = new UpdatePartnersSectionDto
        {
            Title = "New Valid Title",
            Description = "New Valid Description",
        };

        var handler = new UpdatePartnersSectionHandler(
            _mockRepositoryWrapper.Object, _mockMapper.Object, _validator, _mockReorderService.Object);
        var command = new UpdatePartnersSectionCommand(updateDto, 999);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(ErrorMessagesConstants.NotFound(999, typeof(PartnerSection)), result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_WhenImageNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        var existingSection = GetExistingSection();
        var updateDto = new UpdatePartnersSectionDto
        {
            Title = "New Valid Title",
            Description = "New Valid Description",
            PartnersToCreate = [
                new()
                {
                    Description = "Partner Description",
                    ImageId = 999
                }

            ]
        };

        SetUpDependencies(existingSection);
        var handler = new UpdatePartnersSectionHandler(
            _mockRepositoryWrapper.Object, _mockMapper.Object, _validator, _mockReorderService.Object);
        var command = new UpdatePartnersSectionCommand(updateDto, existingSection.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(ErrorMessagesConstants.NotFound([999L], typeof(Image)), result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_WhenPartnerToUpdateNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        var existingSection = GetExistingSection();
        var updateDto = new UpdatePartnersSectionDto
        {
            Title = "New Valid Title",
            Description = "New Valid Description",
            PartnersToUpdate = [
                new()
                {
                    Id = 999,
                    Description = "Fake Partner Desc",
                    ImageId = 100
                }

            ]
        };

        SetUpDependencies(existingSection);
        var handler = new UpdatePartnersSectionHandler(
            _mockRepositoryWrapper.Object, _mockMapper.Object, _validator, _mockReorderService.Object);
        var command = new UpdatePartnersSectionCommand(updateDto, existingSection.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(ErrorMessagesConstants.NotFound([999L], typeof(Partner)), result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_WhenDbUpdateExceptionOccurs_ShouldReturnFailResult()
    {
        // Arrange
        var existingSection = GetExistingSection();
        var updateDto = new UpdatePartnersSectionDto
        {
            Title = "New Valid Title",
            Description = "New Valid Description"
        };

        SetUpDependencies(existingSection);
        _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new Microsoft.EntityFrameworkCore.DbUpdateException());

        var handler = new UpdatePartnersSectionHandler(
            _mockRepositoryWrapper.Object, _mockMapper.Object, _validator, _mockReorderService.Object);
        var command = new UpdatePartnersSectionCommand(updateDto, existingSection.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(PartnerSection)), result.Errors.First().Message);
    }

    private static PartnerSection GetExistingSection() => new()
    {
        Id = 1,
        Title = "Old Valid Title",
        Description = "Old Valid Description",
        Partners =
        [
            new Partner { Id = 10, Description = "Partner To Delete", ImageId = 100, PartnersSectionId = 1, Priority = 1 },
            new Partner { Id = 20, Description = "Partner To Update", ImageId = 200, PartnersSectionId = 1, Priority = 2 }
        ]
    };

    private void SetUpDependencies(PartnerSection? sectionToReturn)
    {
        SetUpRepositoryWrapper(sectionToReturn);
        SetUpMapper();
        SetUpReorderService();
    }

    private void SetUpRepositoryWrapper(PartnerSection? sectionToReturn)
    {
        _mockRepositoryWrapper.Setup(r => r.PartnerSectionsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<PartnerSection>>()))
            .ReturnsAsync(sectionToReturn);

        _mockRepositoryWrapper.Setup(r => r.ImageRepository.GetAllAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync((QueryOptions<Image> options) =>
            {
                var query = _availableImages.AsQueryable();
                if (options.Filter != null)
                {
                    query = query.Where(options.Filter);
                }

                return query.ToList();
            });

        _mockRepositoryWrapper.Setup(r => r.PartnerRepository.DeleteRange(It.IsAny<IEnumerable<Partner>>()));
        _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _mockRepositoryWrapper.Setup(x => x.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));
    }

    private void SetUpMapper()
    {
        _mockMapper.Setup(m => m.Map(It.IsAny<UpdatePartnersSectionDto>(), It.IsAny<PartnerSection>()));
        _mockMapper.Setup(m => m.Map(It.IsAny<UpdatePartnerDto>(), It.IsAny<Partner>()));
        _mockMapper.Setup(m => m.Map<Partner>(It.IsAny<CreatePartnerDto>()))
            .Returns((CreatePartnerDto src) => new Partner { Description = src.Description, ImageId = src.ImageId });
        _mockMapper.Setup(m => m.Map<PartnersSectionDto>(It.IsAny<PartnerSection>()))
            .Returns(new PartnersSectionDto());
    }

    private void SetUpReorderService()
    {
        _mockReorderService.Setup(s => s.RenumberPriorityAsync<Partner>(It.IsAny<Expression<Func<Partner, bool>>>()))
            .Returns(Task.CompletedTask);
    }
}
