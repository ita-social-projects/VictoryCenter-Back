using AutoMapper;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.TeamMembers.CreateTeamMember;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.TeamMembers;


public class CreateTeamMemberTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IValidator<CreateTeamMemberCommand>> _validator;

    private readonly CreateTeamMemberDto _createTeamMemberDto = new CreateTeamMemberDto()
    {
        FirstName = "TestName",
        LastName = "TestLastName",
        MiddleName = "TestMiddleName",
        CategoryId = 1,
        Status = Status.Draft,
        Description = "Long description",
        Email = "Test@gmail.com",
    };
    
    private readonly TeamMemberDto _teamMemberDto = new TeamMemberDto()
    {
        Id = 1,
        FirstName = "TestName",
        LastName = "TestLastName",
        MiddleName = "TestMiddleName",
        Priority = 1,
        CategoryId = 1,
        Status = Status.Draft,
        Description = "Long description",
        Email = "Test@gmail.com",
        CreatedAt = new DateTime(2025, 10, 10)
    };

    private readonly TeamMember _teamMember = new TeamMember()
    {
        Id = 1,
        FirstName = "TestName",
        LastName = "TestLastName",
        MiddleName = "TestMiddleName",
        Priority = 1,
        CategoryId = 1,
        Status = Status.Draft,
        Description = "Long description",
        Email = "Test@gmail.com",
        CreatedAt = new DateTime(2025, 10, 10)
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
        
        SetupDependencies(_createTeamMemberDto, _teamMemberDto, _teamMember, 1 );
        var handler = new CreateTeamMemberHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _validator.Object);

        var result = await handler.Handle(new CreateTeamMemberCommand(_createTeamMemberDto), CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(result.Value, _teamMemberDto);
        
    }

    [Fact]
    public async Task CreateTeamMemberHandle_ShouldReturnFailure_WhenSaveChangeFails()
    {
        string failMessage = "Failed to create new TeamMember";
        SetupDependencies(_createTeamMemberDto, _teamMemberDto, _teamMember, -1);

        var handler = new CreateTeamMemberHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _validator.Object);

        var result = await handler.Handle(new CreateTeamMemberCommand(_createTeamMemberDto), CancellationToken.None);
        
        Assert.True(result.IsFailed);
        Assert.Null(result.ValueOrDefault);
        Assert.Equal( failMessage, result.Errors[0].Message );

    }

    [Fact]
    public async Task CreateTeamMemberHandle_ShouldReturnFailure_WhenExceptionThrown()
    {
        string testMessage = "test message";
        SetupMapper(_createTeamMemberDto, _teamMemberDto, _teamMember);
        _repositoryWrapperMock
            .Setup(repositoryWrapperMock =>
                repositoryWrapperMock.TeamMembersRepository.CreateAsync(It.IsAny<TeamMember>()))
            .ThrowsAsync(new Exception(testMessage));

        var handler = new CreateTeamMemberHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _validator.Object);

        var result = await handler.Handle(new CreateTeamMemberCommand(_createTeamMemberDto), CancellationToken.None);
        
        Assert.True(result.IsFailed);
        Assert.Null(result.ValueOrDefault);
        Assert.Equal( testMessage, result.Errors[0].Message); 
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
            .ReturnsAsync(new FluentValidation.Results.ValidationResult()); 

    }

    private void SetupRepositoryWrapper(TeamMember teamMember, int isSuccess)
    {
        _repositoryWrapperMock.Setup(repositoryWrapper => repositoryWrapper.TeamMembersRepository
                .CreateAsync(It.IsAny<TeamMember>()))
            .ReturnsAsync(teamMember);
        
        _repositoryWrapperMock.Setup(repositoryWrapper => repositoryWrapper.SaveChangesAsync())
            .ReturnsAsync(isSuccess);
    }
}
    