using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;
using VictoryCenter.BLL.Validators.HippotherapyProgramSections;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.HippotherapyProgramSections;

public class BaseHippotherapyProgramSectionValidatorTests
{
    private readonly BaseHippotherapyProgramSectionValidator _validator;

    public BaseHippotherapyProgramSectionValidatorTests()
    {
        _validator = new BaseHippotherapyProgramSectionValidator();
    }

    [Fact]
    public void Validate_TemplateIsInvalid_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.TextOnly) with
        {
            Template = (ProgramSectionTemplate)999
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Template)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeValidEnum(
                nameof(CreateHippotherapyProgramSectionDto.Template)));
    }

    [Fact]
    public void Validate_OrderIsNegative_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.TextOnly) with
        {
            Order = -1
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Order)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeGreaterThan(
                nameof(CreateHippotherapyProgramSectionDto.Order), -1));
    }

    [Fact]
    public void Validate_TitlesCountIsInvalid_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.TextOnly) with
        {
            Titles = []
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Titles)
            .WithErrorMessage(ProgramSectionConstants.GetTitlesCountErrorMessage(model));
    }

    [Fact]
    public void Validate_TitleItemIsEmpty_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.TextOnly);
        model.Titles[0] = "";

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor("Titles[0]")
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateHippotherapyProgramSectionDto.Titles)));
    }

    [Fact]
    public void Validate_TitleItemIsTooShort_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.TextOnly);
        model.Titles[0] = new string('T', ProgramSectionConstants.TemplateRequirements[model.Template].TitleLength.Min - 1);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor("Titles[0]")
            .WithErrorMessage(ProgramSectionConstants.GetTitleLengthErrorMessage(model));
    }

    [Fact]
    public void Validate_DescriptionsCountIsInvalid_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.TextOnly) with
        {
            Descriptions = []
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Descriptions)
            .WithErrorMessage(ProgramSectionConstants.GetDescriptionsCountErrorMessage(model));
    }

    [Fact]
    public void Validate_DescriptionItemIsEmpty_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.TextOnly);
        model.Descriptions[0] = " ";

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor("Descriptions[0]")
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateHippotherapyProgramSectionDto.Descriptions)));
    }

    [Fact]
    public void Validate_DescriptionItemIsTooShort_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.TextOnly);
        model.Descriptions[0] = new string('D', ProgramSectionConstants.TemplateRequirements[model.Template].DescriptionLength.Min - 1);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor("Descriptions[0]")
            .WithErrorMessage(ProgramSectionConstants.GetDescriptionLengthErrorMessage(model));
    }

    [Fact]
    public void Validate_ImageIdsCountIsInvalid_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.TextOnly) with
        {
            ImageIds = [1]
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.ImageIds)
            .WithErrorMessage(ProgramSectionConstants.GetImagesCountErrorMessage(model));
    }

    [Fact]
    public void Validate_ImageIdsContainDuplicates_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.DualImagesBottom) with
        {
            ImageIds = [1, 1]
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.ImageIds)
            .WithErrorMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(
                nameof(CreateHippotherapyProgramSectionDto.ImageIds)));
    }

    [Fact]
    public void Validate_ImageIdIsNotPositive_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.DualImagesBottom) with
        {
            ImageIds = [0, 2] // count ок, unique ок => зайдемо в DependentRules і впадемо на GreaterThan(0)
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor("ImageIds[0]")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(
                nameof(CreateHippotherapyProgramSectionDto.ImageIds)));
    }

    [Fact]
    public void Validate_ValidModel_ShouldNotHaveErrors()
    {
        var model = GetValidModel(ProgramSectionTemplate.QuadImagesBottom);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static CreateHippotherapyProgramSectionDto GetValidModel(ProgramSectionTemplate template)
    {
        var req = ProgramSectionConstants.TemplateRequirements[template];

        return new CreateHippotherapyProgramSectionDto
        {
            Template = template,
            Order = 0,
            Titles = Enumerable.Range(1, req.TitleCount.Min)
                .Select(_ => new string('T', req.TitleLength.Min))
                .ToList(),
            Descriptions = Enumerable.Range(1, req.DescriptionCount.Min)
                .Select(_ => new string('D', req.DescriptionLength.Min))
                .ToList(),
            ImageIds = req.ImageCount.Min == 0
                ? []
                : Enumerable.Range(1, req.ImageCount.Min).Select(i => (long)i).ToList()
        };
    }
}
