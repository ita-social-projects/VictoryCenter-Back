using FluentValidation.TestHelper;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.BLL.Validators.TeamMembers;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.TeamMembers;

public class BaseTeamMembersValidatorTests
{
    private readonly BaseTeamMembersValidator _validator;

    public BaseTeamMembersValidatorTests()
    {
        _validator = new BaseTeamMembersValidator();
    }

    [Fact]
    public void BaseTeamMembersValidator_ShouldHaveError_WhenFullNameIsEmpty()
    {
        var model = new CreateTeamMemberDto { FullName = "", CategoryId = 1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage("FullName field is required");
    }

    [Fact]
    public void BaseTeamMembersValidator_ShouldHaveError_WhenFullNameIsShort()
    {
        var model = new CreateTeamMemberDto { FullName = "A", CategoryId = 1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage("Full name must be at least 2 characters long");
    }

    [Fact]
    public void BaseTeamMembersValidator_ShouldHaveError_WhenFullNameIsTooLong()
    {
        var model = new CreateTeamMemberDto { FullName = new string('A', 101), CategoryId = 1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage("Full name must be no longer than 100 characters");
    }

    [Fact]
    public void BaseTeamMembersValidator_ShouldHaveError_WhenCategoryIdIsZero()
    {
        var model = new CreateTeamMemberDto { FullName = "John Doe", CategoryId = 0 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.CategoryId)
            .WithErrorMessage("CategoryId must be positive value");
    }

    [Fact]
    public void BaseTeamMembersValidator_ShouldHaveError_WhenDescriptionIsTooLong()
    {
        var model = new CreateTeamMemberDto
        {
            FullName = "John Doe",
            CategoryId = 1,
            Description = new string('A', 201)
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("The description length cannot exceed 200 characters");
    }

    [Fact]
    public void BaseTeamMembersValidator_ShouldHaveError_WhenDescriptionEmptyForPublished()
    {
        var model = new CreateTeamMemberDto
        {
            FullName = "John Doe",
            CategoryId = 1,
            Status = Status.Published,
            Description = ""
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description is required for publishing");
    }

    [Fact]
    public void BaseTeamMembersValidator_ShouldNotHaveErrors_ForValidDraftModel()
    {
        var model = new CreateTeamMemberDto
        {
            FullName = "Anna",
            CategoryId = 1,
            Status = Status.Draft,
            Description = "",
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void BaseTeamMembersValidator_ShouldNotHaveErrors_ForValidPublishedModel()
    {
        var model = new CreateTeamMemberDto
        {
            FullName = "Anna",
            CategoryId = 1,
            Status = Status.Published,
            Description = "Desc",
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
