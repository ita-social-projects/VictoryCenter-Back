using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.TeamMembers.Reorder;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.BLL.Validators.TeamMembers;

namespace VictoryCenter.UnitTests.ValidatorsTests.TeamMembers;

public class ReorderTeamMembersValidatorTests
{
    private readonly ReorderTeamMembersValidator _validator = new();

    [Fact]
    public void Validate_ShouldNotHaveError_When_InputIsValid()
    {
        // Arrange
        var dto = new ReorderTeamMembersDto
        {
            CategoryId = 1,
            OrderedIds = [1, 2, 3]
        };
        var command = new ReorderTeamMembersCommand(dto);

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Validate_ShouldHaveError_When_CategoryId_IsNotValid(long categoryId)
    {
        var dto = new ReorderTeamMembersDto
        {
            CategoryId = categoryId,
            OrderedIds = [1, 2, 3]
        };
        var command = new ReorderTeamMembersCommand(dto);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ReorderTeamMembersDto.CategoryId);
    }

    [Fact]
    public void Validate_ShouldHaveError_When_OrderedIds_IsNull()
    {
        var dto = new ReorderTeamMembersDto
        {
            CategoryId = 1,
            OrderedIds = null!
        };
        var command = new ReorderTeamMembersCommand(dto);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ReorderTeamMembersDto.OrderedIds);
    }

    [Fact]
    public void Validate_ShouldHaveError_When_OrderedIds_IsEmpty()
    {
        var dto = new ReorderTeamMembersDto
        {
            CategoryId = 1,
            OrderedIds = []
        };
        var command = new ReorderTeamMembersCommand(dto);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ReorderTeamMembersDto.OrderedIds);
    }

    [Fact]
    public void Validate_ShouldHaveError_When_OrderedIds_ContainsDuplicates()
    {
        var dto = new ReorderTeamMembersDto
        {
            CategoryId = 1,
            OrderedIds = [1, 2, 3, 2]
        };
        var command = new ReorderTeamMembersCommand(dto);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ReorderTeamMembersDto.OrderedIds);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Validate_ShouldHaveError_When_OrderedIds_ContainsInvalidId(long memberId)
    {
        var dto = new ReorderTeamMembersDto
        {
            CategoryId = 1,
            OrderedIds = [1, memberId, 3]
        };
        var command = new ReorderTeamMembersCommand(dto);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor("ReorderTeamMembersDto.OrderedIds[1]");
    }
}
