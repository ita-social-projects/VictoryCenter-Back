using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.History.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HistorySection;
using VictoryCenter.BLL.Validators.HistorySections;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.HistorySections;

public class UpdateHistorySectionsCommandValidatorTests
{
    private readonly UpdateHistorySectionsCommandValidator _validator =
        new(new UpdateHistorySectionValidator());

    [Fact]
    public void Validate_UpdateSectionsIsNull_ShouldHaveError()
    {
        var command = new UpdateHistorySectionsCommand(null!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UpdateSections)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateHistorySectionsCommand.UpdateSections)));
    }

    [Fact]
    public void Validate_InvalidSection_ShouldHaveNestedError()
    {
        var command = new UpdateHistorySectionsCommand(
        [
            new UpdateHistorySectionDto
            {
                Id = 1,
                Template = HistorySectionTemplate.TextOnly,
                Order = -1,
                Contents =
                [
                    new CreateHistorySectionContentDto { ContentType = ContentType.Title, Order = 0, Title = "Valid title" },
                    new CreateHistorySectionContentDto { ContentType = ContentType.Description, Order = 1, Description = "Valid description" }
                ]
            }

        ]);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("UpdateSections[0].Order")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeGreaterThan(nameof(UpdateHistorySectionDto.Order), -1));
    }

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveErrors()
    {
        var command = new UpdateHistorySectionsCommand(
        [
            new UpdateHistorySectionDto
            {
                Id = 1,
                Template = HistorySectionTemplate.TextOnly,
                Order = 0,
                Contents =
                [
                    new CreateHistorySectionContentDto { ContentType = ContentType.Title, Order = 0, Title = "Valid title" },
                    new CreateHistorySectionContentDto { ContentType = ContentType.Description, Order = 1, Description = "Valid description" }
                ]
            }

        ]);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}