using VictoryCenter.BLL.Interfaces.Search;
using VictoryCenter.BLL.Services.Search;
using VictoryCenter.BLL.Services.Search.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ServiceTests;

public class SearchServiceTests
{
    private readonly ISearchService<TeamMember> _searchService;

    private readonly List<TeamMember> _teamMembers = [
        new TeamMember
        {
            Id = 1,
            FullName = "TestName1 TestSuname1",
            Priority = 1,
            CategoryId = 1,
            Status = Status.Draft,
            Description = "Long test1 description",
            Email = "Test1@gmail.com",
            CreatedAt = DateTime.UtcNow.AddMinutes(-10)
        },
        new TeamMember
        {
            Id = 2,
            FullName = "NotTestName2 NotTestSuname2",
            Priority = 2,
            CategoryId = 1,
            Status = Status.Draft,
            Description = "Long not test2 description",
            Email = "NotTest2@gmail.com",
            CreatedAt = DateTime.UtcNow.AddMinutes(-10)
        },
    ];

    public SearchServiceTests()
    {
        _searchService = new SearchService<TeamMember>();
    }

    [Theory]
    [InlineData("T")]
    [InlineData("Test")]
    [InlineData("TestName")]
    [InlineData("TestName1")]
    public void SearchOneTermByPrefix_ShouldReturnNonEmpty_WhenExists(string term)
    {
        // Arrange
        List<TeamMember> expectedResult = [_teamMembers[0]];
        var searchTerm = new SearchTerm<TeamMember>
        {
            SearchLogic = SearchLogic.Prefix,
            TermSelector = tm => tm.FullName,
            TermValue = term,
        };

        // Act
        var expression = _searchService.CreateSearchExpression(searchTerm);
        var actualResult = _teamMembers.AsQueryable().Where(expression).ToList();

        // Assert
        Assert.Equal(expectedResult, actualResult);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("Tests")]
    [InlineData("TestName12")]
    public void SearchOneTermByPrefix_ShouldReturnEmpty_WhenNotExists(string term)
    {
        // Arrange
        var searchTerm = new SearchTerm<TeamMember>
        {
            SearchLogic = SearchLogic.Prefix,
            TermSelector = tm => tm.FullName,
            TermValue = term,
        };

        // Act
        var expression = _searchService.CreateSearchExpression(searchTerm);
        var actualResult = _teamMembers.AsQueryable().Where(expression).ToList();

        // Assert
        Assert.Empty(actualResult);
    }

    [Theory]
    [InlineData("T", "Long")]
    [InlineData("Test", "Long test")]
    [InlineData("TestName1", "Long test1 description")]
    public void SearchMultipleTermsByPrefix_ShouldReturnNonEmpty_WhenExists(string term1, string term2)
    {
        // Arrange
        List<TeamMember> expectedResult = [_teamMembers[0]];
        var searchTerm1 = new SearchTerm<TeamMember>
        {
            SearchLogic = SearchLogic.Prefix,
            TermSelector = tm => tm.FullName,
            TermValue = term1,
        };
        var searchTerm2 = new SearchTerm<TeamMember>
        {
            SearchLogic = SearchLogic.Prefix,
            TermSelector = tm => tm.Description!,
            TermValue = term2,
        };

        // Act
        var expression = _searchService.CreateSearchExpression(searchTerm1, searchTerm2);
        var actualResult = _teamMembers.AsQueryable().Where(expression).ToList();

        // Assert
        Assert.Equal(expectedResult, actualResult);
    }

    [Theory]
    [InlineData("A", "Long")]
    [InlineData("T", "B")]
    [InlineData("A", "B")]
    public void SearchMultipleTermsByPrefix_ShouldReturnEmpty_WhenNotExists(string term1, string term2)
    {
        // Arrange
        var searchTerm1 = new SearchTerm<TeamMember>
        {
            SearchLogic = SearchLogic.Prefix,
            TermSelector = tm => tm.FullName,
            TermValue = term1,
        };
        var searchTerm2 = new SearchTerm<TeamMember>
        {
            SearchLogic = SearchLogic.Prefix,
            TermSelector = tm => tm.Description!,
            TermValue = term2,
        };

        // Act
        var expression = _searchService.CreateSearchExpression(searchTerm1, searchTerm2);
        var actualResult = _teamMembers.AsQueryable().Where(expression).ToList();

        // Assert
        Assert.Empty(actualResult);
    }

    [Theory]
    [InlineData("TestName1 TestSuname1")]
    public void SearchOneTermByExact_ShouldReturnNonEmpty_WhenExists(string term)
    {
        // Arrange
        List<TeamMember> expectedResult = [_teamMembers[0]];
        var searchTerm = new SearchTerm<TeamMember>
        {
            SearchLogic = SearchLogic.Exact,
            TermSelector = tm => tm.FullName,
            TermValue = term,
        };

        // Act
        var expression = _searchService.CreateSearchExpression(searchTerm);
        var actualResult = _teamMembers.AsQueryable().Where(expression).ToList();

        // Assert
        Assert.Equal(expectedResult, actualResult);
    }

    [Theory]
    [InlineData("TestName1")]
    [InlineData("TestName1 TestSuname")]
    public void SearchOneTermByExact_ShouldReturnEmpty_WhenNotExists(string term)
    {
        // Arrange
        var searchTerm = new SearchTerm<TeamMember>
        {
            SearchLogic = SearchLogic.Exact,
            TermSelector = tm => tm.FullName,
            TermValue = term,
        };

        // Act
        var expression = _searchService.CreateSearchExpression(searchTerm);
        var actualResult = _teamMembers.AsQueryable().Where(expression).ToList();

        // Assert
        Assert.Empty(actualResult);
    }
}
