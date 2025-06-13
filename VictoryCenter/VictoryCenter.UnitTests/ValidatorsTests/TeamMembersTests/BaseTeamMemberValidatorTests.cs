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
    public void BaseTeamMembersValidator_ShouldHaveError_WhenFirstNameIsEmpty()
    {
        var model = new CreateTeamMemberDto { FirstName = "", LastName = "Test", CategoryId = 1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("This field is required");
    }

    [Fact]
    public void BaseTeamMembersValidator_ShouldHaveError_WhenFirstNameIsShort()
    {
        var model = new CreateTeamMemberDto { FirstName = "A", LastName = "Test", CategoryId = 1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name must be at least 2 characters long");
    }
    
    [Fact]
    public void BaseTeamMembersValidator_ShouldHaveError_WhenFirstNameIsTooLong()
    {
        var model = new CreateTeamMemberDto { FirstName = new string('A', 51), LastName = "Test", CategoryId = 1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage($"First name must be no longer than 50 characters");
    }

    [Fact]
    public void BaseTeamMembersValidator_ShouldHaveError_WhenLastNameIsEmpty()
    {
        var model = new CreateTeamMemberDto { FirstName = "John", LastName = "", CategoryId = 1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("This field is required");
    }
    
    [Fact]
    public void BaseTeamMembersValidator_ShouldHaveError_WhenLastNameIsShort()
    {
        var model = new CreateTeamMemberDto { FirstName = "Test", LastName = "T", CategoryId = 1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name must be at least 2 characters long");
    }
    
    [Fact]
    public void BaseTeamMembersValidator_ShouldHaveError_WhenLastNameIsTooLong()
    {
        var model = new CreateTeamMemberDto { FirstName = "Test", LastName = new string('A', 51), CategoryId = 1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage($"Last name must be no longer than 50 characters");
    }

    [Fact]
    public void BaseTeamMembersValidator_ShouldHaveError_WhenCategoryIdIsZero()
    {
        var model = new CreateTeamMemberDto { FirstName = "John", LastName = "Doe", CategoryId = 0 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.CategoryId)
            .WithErrorMessage("CategoryId must be positive value");
    }

    [Fact]
    public void BaseTeamMembersValidator_ShouldHaveError_WhenDescriptionIsTooLong()
    {
        var model = new CreateTeamMemberDto
        {
            FirstName = "John",
            LastName = "Doe",
            CategoryId = 1,
            Description = new string('A', 201)
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("the description length cannot exceed 200 characters");
    }

    [Fact]
    public void BaseTeamMembersValidator_ShouldHaveError_WhenDescriptionEmptyForPublished()
    {
        var model = new CreateTeamMemberDto
        {
            FirstName = "John",
            LastName = "Doe",
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
            FirstName = "Anna",
            LastName = "Smith",
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
            FirstName = "Anna",
            LastName = "Smith",
            CategoryId = 1,
            Status = Status.Published,
            Description = "Desc",
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

}

