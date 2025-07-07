using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.BLL.Interfaces.Search;
using VictoryCenter.BLL.Queries.TeamMembers.Search;
using VictoryCenter.BLL.Services.Search;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.TeamMembers;

public class SearchTeamMember
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IValidator<SearchTeamMemberQuery>> _validator;
    private readonly ISearchService _searchService;

    private readonly List<TeamMember> _teamMembers = [
        new TeamMember
        {
            Id = 1,
            FullName = "TestName",
            Priority = 1,
            CategoryId = 1,
            Status = Status.Draft,
            Description = "Long description",
            Email = "Test@gmail.com",
            CreatedAt = DateTime.UtcNow.AddMinutes(-10)
        },
    ];

    private readonly List<TeamMemberDto> _teamMemberDtos = [
        new TeamMemberDto
        {
            Id = 1,
            FullName = "TestName",
            Priority = 1,
            CategoryName = "Name",
            Status = Status.Draft,
            Description = "Long description",
            Email = "Test@gmail.com"
        },
    ];

    public SearchTeamMember()
    {
        _validator = new Mock<IValidator<SearchTeamMemberQuery>>();
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _searchService = new SearchService(_repositoryWrapperMock.Object);
    }

    private void SetupMapper(TeamMemberDto teamMemberDto)
    {
        _mapperMock.Setup(mapper => mapper.Map<TeamMemberDto>(It.IsAny<TeamMember>())).Returns(teamMemberDto);
    }

    private void SetupValidator()
    {
        _validator.Setup(validator => validator.ValidateAsync(It.IsAny<SearchTeamMemberQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private void SetupRepositoryWrapper()
    {
        _repositoryWrapperMock.Setup(repositoryWrapper => repositoryWrapper.AsNoTracking<TeamMember>())
            .Returns(_teamMembers.AsQueryable());
    }
}
