using System.Linq.Expressions;
using System.Transactions;
using AutoMapper;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.TeamMembers.Create;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.TeamMembers;

public class CreateTeamMemberTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IValidator<CreateTeamMemberCommand>> _validator;

    private readonly CreateTeamMemberDto _createTeamMemberDto = new()
    {
        FullName = "TestName",
        CategoryId = 1,
        Status = Status.Draft,
        Description = "Long description",
        Email = "Test@gmail.com"
    };

    private readonly TeamMember _teamMember = new()
    {
        Id = 1,
        FullName = "TestName",
        Priority = 1,
        CategoryId = 1,
        Status = Status.Draft,
        Description = "Long description",
        Email = "Test@gmail.com",
        CreatedAt = DateTime.UtcNow.AddMinutes(-10)
    };

    private readonly TeamMemberDto _teamMemberDto = new()
    {
        Id = 1,
        FullName = "TestName",
        Priority = 1,
        CategoryId = 1,
        Status = Status.Draft,
        Description = "Long description",
        Email = "Test@gmail.com"
    };

    private readonly Category _category = new()
    {
        Id = 1,
        Name = "Test",
        CreatedAt = DateTime.UtcNow.AddMinutes(-10)
    };

    public CreateTeamMemberTests()
    {
        _validator = new Mock<IValidator<CreateTeamMemberCommand>>();
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task CreateTeamMemberHandle_ShouldReturnTeamMemberDto_WhenCreationIsValid()
    {
        SetupDependencies(_createTeamMemberDto, _teamMemberDto, _teamMember, 1);
        var handler = new CreateTeamMemberHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _validator.Object);

        Result<TeamMemberDto> result =
            await handler.Handle(new CreateTeamMemberCommand(_createTeamMemberDto), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_teamMemberDto, result.Value);
    }

    [Fact]
    public async Task CreateTeamMemberHandle_ShouldReturnFailure_WhenSaveChangeFails()
    {
        var failMessage = "Failed to create new TeamMember";
        SetupDependencies(_createTeamMemberDto, _teamMemberDto, _teamMember, -1);

        var handler = new CreateTeamMemberHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _validator.Object);

        Result<TeamMemberDto> result =
            await handler.Handle(new CreateTeamMemberCommand(_createTeamMemberDto), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Null(result.ValueOrDefault);
        Assert.Equal(failMessage, result.Errors[0].Message);
    }

    [Fact]
    public async Task CreateTeamMemberHandle_WhenCategoryIdIsInvalid_ShouldReturnFailure()
    {
        _repositoryWrapperMock
            .Setup(repositoryWrapper =>
                repositoryWrapper.CategoriesRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Category>>()))
            .ReturnsAsync((Category?)null);
        SetupMapper(_createTeamMemberDto, _teamMemberDto, _teamMember);
        SetupValidator();
        var handler = new CreateTeamMemberHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _validator.Object);

        Result<TeamMemberDto> result =
            await handler.Handle(new CreateTeamMemberCommand(_createTeamMemberDto), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Null(result.ValueOrDefault);
        Assert.Equal("There are no categories with this id", result.Errors[0].Message);
    }

    [Fact]
    public async Task CreateTeamMemberHandle_ShouldReturnFailure_WhenExceptionThrown()
    {
        var testMessage = "test message";
        SetupMapper(_createTeamMemberDto, _teamMemberDto, _teamMember);
        _repositoryWrapperMock
            .Setup(repositoryWrapperMock =>
                repositoryWrapperMock.TeamMembersRepository.CreateAsync(It.IsAny<TeamMember>()))
            .ThrowsAsync(new DbUpdateException(testMessage));
        _repositoryWrapperMock.Setup(r => r.TeamMembersRepository.MaxAsync(It.IsAny<Expression<Func<TeamMember, long>>>(), It.IsAny<Expression<Func<TeamMember, bool>>?>()))
            .ReturnsAsync(0L);

        _repositoryWrapperMock.Setup(repositoryWrapper => repositoryWrapper.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));

        _repositoryWrapperMock
            .Setup(repositoryWrapper =>
                repositoryWrapper.CategoriesRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Category>>()))
            .ReturnsAsync(_category);

        var handler = new CreateTeamMemberHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _validator.Object);

        Result<TeamMemberDto> result =
            await handler.Handle(new CreateTeamMemberCommand(_createTeamMemberDto), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Null(result.ValueOrDefault);
        Assert.Equal("Fail to create new team member in database:" + testMessage, result.Errors[0].Message);
    }

    private void SetupDependencies(CreateTeamMemberDto createMember, TeamMemberDto memberDto, TeamMember member, int isSuccess)
    {
        SetupMapper(createMember, memberDto, member);
        SetupRepositoryWrapper(member, isSuccess);
        SetupValidator();
    }

    private void SetupMapper(CreateTeamMemberDto createTeamMemberDto, TeamMemberDto teamMemberDto, TeamMember teamMember)
    {
        _mapperMock.Setup(mapper => mapper.Map<TeamMember>(It.IsAny<CreateTeamMemberDto>())).Returns(teamMember);
        _mapperMock.Setup(mapper => mapper.Map<TeamMemberDto>(It.IsAny<TeamMember>())).Returns(teamMemberDto);
    }

    private void SetupValidator()
    {
        _validator.Setup(v => v.ValidateAsync(It.IsAny<CreateTeamMemberCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private void SetupRepositoryWrapper(TeamMember teamMember, int isSuccess)
    {
        _repositoryWrapperMock.Setup(repositoryWrapper => repositoryWrapper.TeamMembersRepository
                .CreateAsync(It.IsAny<TeamMember>()))
            .ReturnsAsync(teamMember);

        _repositoryWrapperMock.Setup(r => r.TeamMembersRepository.MaxAsync(It.IsAny<Expression<Func<TeamMember, long>>>(), It.IsAny<Expression<Func<TeamMember, bool>>?>()))
            .ReturnsAsync(0L);

        _repositoryWrapperMock.Setup(repositoryWrapper => repositoryWrapper.SaveChangesAsync())
            .ReturnsAsync(isSuccess);

        _repositoryWrapperMock.Setup(repositoryWrapper => repositoryWrapper.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));

        _repositoryWrapperMock
            .Setup(repositoryWrapper =>
                repositoryWrapper.CategoriesRepository.GetFirstOrDefaultAsync(
                    It.IsAny<QueryOptions<Category>>()))
            .ReturnsAsync(_category);
    }
}
