using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.BLL.Queries.Admin.TeamMembers.Search;
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
    public void Validate_InvalidQuery_FullNameEmptyShouldHaveError(string? fullName)
    {
        var dto = new SearchTeamMemberDto
        {
            FullName = fullName!,
        };
        var command = new SearchTeamMemberQuery(dto);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.SearchTeamMemberDto.FullName)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(SearchTeamMemberDto.FullName)));
    }

    [Fact]
    public void Validate_InvalidQuery_FullNameTooShortShouldHaveError()
    {
        var dto = new SearchTeamMemberDto
        {
            FullName = "A",
        };
        var command = new SearchTeamMemberQuery(dto);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.SearchTeamMemberDto.FullName)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(SearchTeamMemberDto.FullName),
                GlobalSearchConstants.DefaultSearchQueryMinLength));
    }

    [Fact]
    public void Validate_InvalidQuery_FullNameTooLongShouldHaveError()
    {
        var fullName = new string('A', GlobalSearchConstants.DefaultSearchQueryMaxLength + 1);
        var dto = new SearchTeamMemberDto
        {
            FullName = fullName!,
        };
        var command = new SearchTeamMemberQuery(dto);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.SearchTeamMemberDto.FullName)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(SearchTeamMemberDto.FullName),
                GlobalSearchConstants.DefaultSearchQueryMaxLength));
    }
}
