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
    public void Validate_ContentsIsNull_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.TextOnly) with
        {
            Contents = null!
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(CreateHippotherapyProgramSectionDto.Contents)));
    }

    [Fact]
    public void Validate_TitlesCountIsInvalid_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.TextOnly) with
        {
            Contents = []
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(HippotherapyProgramSectionConstants.GetTitlesCountErrorMessage(model));
    }

    [Fact]
    public void Validate_TitleItemIsEmpty_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.TextOnly);
        model.Contents.RemoveAll(c => c.ContentType == ContentType.Title);
        model.Contents.Add(new CreateProgramSectionContentDto
        {
            ContentType = ContentType.Title,
            Order = 0,
            Title = ""
        });

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateProgramSectionContentDto.Title)));
    }

    [Fact]
    public void Validate_TitleItemIsTooShort_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.TextOnly);
        var min = HippotherapyProgramSectionConstants.GetRequirements(model.Template).TitleLength.Min;

        model.Contents.RemoveAll(c => c.ContentType == ContentType.Title);
        model.Contents.Add(new CreateProgramSectionContentDto
        {
            ContentType = ContentType.Title,
            Order = 0,
            Title = new string('T', min - 1)
        });

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(HippotherapyProgramSectionConstants.GetTitleLengthErrorMessage(model));
    }

    [Fact]
    public void Validate_DescriptionsCountIsInvalid_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.TextOnly);
        model.Contents.RemoveAll(c => c.ContentType == ContentType.Description);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(HippotherapyProgramSectionConstants.GetDescriptionsCountErrorMessage(model));
    }

    [Fact]
    public void Validate_DescriptionItemIsEmpty_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.TextOnly);
        model.Contents.RemoveAll(c => c.ContentType == ContentType.Description);
        model.Contents.Add(new CreateProgramSectionContentDto
        {
            ContentType = ContentType.Description,
            Order = 1,
            Description = " "
        });

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateProgramSectionContentDto.Description)));
    }

    [Fact]
    public void Validate_DescriptionItemIsTooShort_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.TextOnly);
        var min = HippotherapyProgramSectionConstants.GetRequirements(model.Template).DescriptionLength.Min;

        model.Contents.RemoveAll(c => c.ContentType == ContentType.Description);
        model.Contents.Add(new CreateProgramSectionContentDto
        {
            ContentType = ContentType.Description,
            Order = 1,
            Description = new string('D', min - 1)
        });

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(HippotherapyProgramSectionConstants.GetDescriptionLengthErrorMessage(model));
    }

    [Fact]
    public void Validate_ImagesCountIsInvalid_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.TextOnly);
        model.Contents.Add(new CreateProgramSectionContentDto
        {
            ContentType = ContentType.Image,
            Order = 2,
            ImageId = 1
        });

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(HippotherapyProgramSectionConstants.GetImagesCountErrorMessage(model));
    }

    [Fact]
    public void Validate_ImageIdsContainDuplicates_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.DualImagesBottom);
        model.Contents.RemoveAll(c => c.ContentType == ContentType.Image);

        model.Contents.Add(new CreateProgramSectionContentDto
        {
            ContentType = ContentType.Image,
            Order = 2,
            ImageId = 1
        });

        model.Contents.Add(new CreateProgramSectionContentDto
        {
            ContentType = ContentType.Image,
            Order = 3,
            ImageId = 1
        });

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(CreateProgramSectionContentDto.ImageId)));
    }

    [Fact]
    public void Validate_ImageIdIsNotPositive_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.DualImagesBottom);
        model.Contents.RemoveAll(c => c.ContentType == ContentType.Image);

        model.Contents.Add(new CreateProgramSectionContentDto
        {
            ContentType = ContentType.Image,
            Order = 2,
            ImageId = 0
        });

        model.Contents.Add(new CreateProgramSectionContentDto
        {
            ContentType = ContentType.Image,
            Order = 3,
            ImageId = 2
        });

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateProgramSectionContentDto.ImageId)));
    }

    [Fact]
    public void Validate_DualTitleDescriptionPairs_GroupIndexIsMissing_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.DualTitleDescriptionPairs);

        model = model with
        {
            Contents = [.. model.Contents.Select(c => c with { GroupIndex = null })]
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(HippotherapyProgramSectionConstants.GetGroupIndexRequiredErrorMessage(model));
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
        var req = HippotherapyProgramSectionConstants.GetRequirements(template);
        var contents = new List<CreateProgramSectionContentDto>();
        var order = 0;

        for (var i = 0; i < req.TitleCount.Min; i++)
        {
            contents.Add(new CreateProgramSectionContentDto
            {
                ContentType = ContentType.Title,
                Order = order++,
                Title = new string('T', req.TitleLength.Min)
            });
        }

        for (var i = 0; i < req.DescriptionCount.Min; i++)
        {
            contents.Add(new CreateProgramSectionContentDto
            {
                ContentType = ContentType.Description,
                Order = order++,
                Description = new string('D', req.DescriptionLength.Min)
            });
        }

        for (var i = 0; i < req.ImageCount.Min; i++)
        {
            contents.Add(new CreateProgramSectionContentDto
            {
                ContentType = ContentType.Image,
                Order = order++,
                ImageId = i + 1
            });
        }

        if (req.Grouping is not null)
        {
            contents.Clear();
            order = 0;

            for (var g = 0; g < req.Grouping.GroupCount.Min; g++)
            {
                contents.Add(new CreateProgramSectionContentDto
                {
                    ContentType = ContentType.Title,
                    Order = order++,
                    GroupIndex = g,
                    Title = new string('T', req.TitleLength.Min)
                });

                contents.Add(new CreateProgramSectionContentDto
                {
                    ContentType = ContentType.Description,
                    Order = order++,
                    GroupIndex = g,
                    Description = new string('D', req.DescriptionLength.Min)
                });
            }
        }

        return new CreateHippotherapyProgramSectionDto
        {
            Template = template,
            Order = 0,
            Contents = contents
        };
    }
}
