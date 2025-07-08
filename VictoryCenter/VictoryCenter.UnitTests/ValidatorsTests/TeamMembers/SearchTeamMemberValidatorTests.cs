using FluentValidation.TestHelper;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.BLL.Queries.TeamMembers.Search;
using VictoryCenter.BLL.Validators.TeamMembers;

namespace VictoryCenter.UnitTests.ValidatorsTests.TeamMembers;

public class SearchTeamMemberValidatorTests
{
    private readonly SearchTeamMemberValidator _validator = new();

    [Fact]
    public void Validate_ValidQuery_ShouldNotHaveErrors()
    {
        var dto = new SearchTeamMemberDto
        {
            FullName = "Test",
        };
        var command = new SearchTeamMemberQuery(dto);

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("A")]
    public void Validate_InvalidQuery_ShouldHaveError(string? fullName)
    {
        var dto = new SearchTeamMemberDto
        {
            FullName = fullName!,
        };
        var command = new SearchTeamMemberQuery(dto);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.SearchTeamMemberDto.FullName);
    }
}
