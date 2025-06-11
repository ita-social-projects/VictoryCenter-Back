using FluentValidation.TestHelper;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.BLL.Validators.TeamMembers;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.TeamMembersTests;

public class BaseTeamMembersValidatorTests
{
    private readonly BaseTeamMembersValidator _validator;

    public BaseTeamMembersValidatorTests()
    {
        _validator = new BaseTeamMembersValidator();
    }

    [Fact]
    public void Should_Have_Error_When_FirstName_Is_Empty()
    {
        var model = new CreateTeamMemberDto { FirstName = "", LastName = "Test", CategoryId = 1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Should_Have_Error_When_FirstName_Is_Short()
    {
        var model = new CreateTeamMemberDto { FirstName = "A", LastName = " test", CategoryId = 1};
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

   
    [Fact]
    public void Should_Have_Error_When_CategoryId_Is_Zero()
    {
        var model = new CreateTeamMemberDto { CategoryId = 0, LastName = "Test", FirstName = "Test"};
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }
    

    [Fact]
    public void Should_Have_Error_When_Description_Is_Too_Long()
    {
        var model = new CreateTeamMemberDto { Description = new string('A', 201), FirstName = " test", LastName = " test", CategoryId = 1};
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Not_Have_Errors_For_Valid_Model()
    {
        var model = new CreateTeamMemberDto
        {
            FirstName = "John",
            LastName = "Black",
            CategoryId = 1,
            Status = Status.Draft,
            Description = "Team member responsible for testing"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}