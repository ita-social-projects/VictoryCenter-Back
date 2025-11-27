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
    public void BaseTeamMembersValidator_ShouldHaveError_WhenFullNameHasInvalidFormat()
    {
        var model = new CreateTeamMemberDto { FullName = "ha-ha here is unex#p32324ected string -(X_X)-", CategoryId = 1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(
                nameof(CreateTeamMemberDto.FullName)));
    }

    [Fact]
    public void BaseTeamMembersValidator_ShouldHaveError_WhenFullNameIsShort()
    {
        var model = new CreateTeamMemberDto { FullName = "A", CategoryId = 1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateTeamMemberDto.FullName),
                TeamMemberConstants.FullNameMinLength));
    }

    [Fact]
    public void BaseTeamMembersValidator_ShouldHaveError_WhenFullNameIsTooLong()
    {
        var model = new CreateTeamMemberDto { FullName = new string('A', TeamMemberConstants.FullNameMaxLength + 1), CategoryId = 1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateTeamMemberDto.FullName),
                TeamMemberConstants.FullNameMaxLength));
    }

    [Fact]
    public void BaseTeamMembersValidator_ShouldHaveError_WhenFullNameConsistSymbols()
    {
        var model = new CreateTeamMemberDto
        {
            FullName = "@#$",
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage(
                ErrorMessagesConstants.PropertyMustBeInAValidFormat(nameof(CreateTeamMemberDto.FullName)));
    }

    [Fact]
    public void BaseTeamMembersValidator_ShouldHaveError_WhenFullNameConsistNumbers()
    {
        var model = new CreateTeamMemberDto
        {
            FullName = "123",
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage(
                ErrorMessagesConstants.PropertyMustBeInAValidFormat(nameof(CreateTeamMemberDto.FullName)));
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
            Description = new string('A', TeamMemberConstants.DescriptionNameMaxLength + 1),
            Status = Status.Published
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateTeamMemberDto.Description),
                TeamMemberConstants.DescriptionNameMaxLength));
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
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(CreateTeamMemberDto.Description)));
    }

    [Fact]
    public void BaseTeamMembersValidator_ShouldNotHaveErrors_ForValidDraftModel()
    {
        var model = new CreateTeamMemberDto
        {
            FullName = "Anna",
            CategoryId = 1,
            Status = Status.Draft,
            Description = new string('A', TeamMemberConstants.DescriptionNameMinLength + 1),
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
            ImageId = 123
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateTeamMemberDto.Description), TeamMemberConstants.DescriptionNameMinLength));
    }

    [Fact]
    public void BaseTeamMembersValidator_ShouldHaveError_WhenImageIdIsNullForPublished()
    {
        var model = new CreateTeamMemberDto
        {
            FullName = "John Doe",
            CategoryId = 1,
            Status = Status.Published,
            Description = "Valid description",
            ImageId = null
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ImageId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(CreateTeamMemberDto.ImageId)));
    }

    [Fact]
    public void BaseTeamMembersValidator_ShouldNotHaveError_WhenImageIdIsNullForDraft()
    {
        var model = new CreateTeamMemberDto
        {
            FullName = "John Doe",
            CategoryId = 1,
            Status = Status.Draft,
            Description = "",
            ImageId = null
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.ImageId);
    }
}
