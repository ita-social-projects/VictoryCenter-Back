using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.EventNewsCategories.Create;
using VictoryCenter.BLL.Commands.Admin.EventNewsCategories.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventNewsCategories;
using VictoryCenter.BLL.Validators.EventNewsCategories;

namespace VictoryCenter.UnitTests.ValidatorsTests.EventNewsCategories;

public class EventNewsCategoryValidatorsTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void CreateCategory_ShouldRejectEmptyName(string? name)
    {
        var command = new CreateEventNewsCategoryCommand(
            new CreateEventNewsCategoryDto { Name = name! });

        var result = new CreateEventNewsCategoryValidator().TestValidate(command);

        result.ShouldHaveValidationErrorFor(item => item.Category.Name);
    }

    [Fact]
    public void CreateCategory_ShouldRejectNameBelowMinimumLengthAfterTrimming()
    {
        var command = new CreateEventNewsCategoryCommand(
            new CreateEventNewsCategoryDto { Name = "  a  " });

        var result = new CreateEventNewsCategoryValidator().TestValidate(command);

        result.ShouldHaveValidationErrorFor(item => item.Category.Name);
    }

    [Fact]
    public void UpdateCategory_ShouldRejectNameOverMaximumLengthAfterTrimming()
    {
        var command = new UpdateEventNewsCategoryCommand(
            1,
            new UpdateEventNewsCategoryDto
            {
                Name = $"  {new string('a', EventNewsCategoryConstants.MaxNameLength + 1)}  "
            });

        var result = new UpdateEventNewsCategoryValidator().TestValidate(command);

        result.ShouldHaveValidationErrorFor(item => item.Category.Name);
    }

    [Theory]
    [InlineData("  ab  ")]
    [InlineData("  abcdefghijklmnopqrst  ")]
    public void CreateCategory_ShouldAcceptBoundaryLengthAfterTrimming(string name)
    {
        var command = new CreateEventNewsCategoryCommand(
            new CreateEventNewsCategoryDto { Name = name });

        var result = new CreateEventNewsCategoryValidator().TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(item => item.Category.Name);
    }
}
