using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
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
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateTeamMemberDto.FullName)));
    }

    [Fact]
    public void BaseTeamMembersValidator_ShouldHaveError_WhenFullNameIsShort()
    {
        var model = new CreateTeamMemberDto { FullName = "A", CategoryId = 1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateTeamMemberDto.FullName),
                BaseTeamMembersValidator.FullNameMinLength));
    }

    [Fact]
    public void BaseTeamMembersValidator_ShouldHaveError_WhenFullNameIsTooLong()
    {
        var model = new CreateTeamMemberDto { FullName = new string('A', BaseTeamMembersValidator.FullNameMaxLength + 1), CategoryId = 1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateTeamMemberDto.FullName),
                BaseTeamMembersValidator.FullNameMaxLength));
    }

    [Fact]
    public void BaseTeamMembersValidator_ShouldHaveError_WhenCategoryIdIsZero()
    {
        var model = new CreateTeamMemberDto { FullName = "John Doe", CategoryId = 0 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.CategoryId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateTeamMemberDto.CategoryId)));
    }

    [Fact]
    public void BaseTeamMembersValidator_ShouldHaveError_WhenDescriptionIsTooLong()
    {
        var model = new CreateTeamMemberDto
        {
            FullName = "John Doe",
            CategoryId = 1,
            Description = new string('A', BaseTeamMembersValidator.DescriptionNameMaxLength + 1)
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateTeamMemberDto.Description),
                BaseTeamMembersValidator.DescriptionNameMaxLength));
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
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateTeamMemberDto.Description)));
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
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(CreateTeamMemberDto.Description), 10));
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
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(CreateTeamMemberDto.Description), 10));
    }
}
