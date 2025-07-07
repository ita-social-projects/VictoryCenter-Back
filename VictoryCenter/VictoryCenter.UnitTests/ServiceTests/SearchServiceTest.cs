using VictoryCenter.BLL.Interfaces.Search;
using VictoryCenter.BLL.Services.Search;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ServiceTests;

public class SearchServiceTest
{
    private readonly ISearchService<TeamMember> _searchService;

    private readonly List<TeamMember> _teamMembers = [
        new TeamMember
        {
            Id = 1,
            FullName = "TestName1",
            Priority = 1,
            CategoryId = 1,
            Status = Status.Draft,
            Description = "Long description",
            Email = "Test1@gmail.com",
            CreatedAt = DateTime.UtcNow.AddMinutes(-10)
        },
        new TeamMember
        {
            Id = 2,
            FullName = "TestName2",
            Priority = 2,
            CategoryId = 1,
            Status = Status.Draft,
            Description = "Long description",
            Email = "Test2@gmail.com",
            CreatedAt = DateTime.UtcNow.AddMinutes(-10)
        },
    ];

    public SearchServiceTest()
    {
        _searchService = new SearchService<TeamMember>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("T")]
    [InlineData("Test")]
    [InlineData("TestName")]
    [InlineData("TestName1")]
    public async Task SearchOneTeamMemberByPrefixFullName_ShouldReturnTeamMember_WhenDataIsValid(string term)
    {
        // Arrange

        // Act
        // Assert
    }
}
