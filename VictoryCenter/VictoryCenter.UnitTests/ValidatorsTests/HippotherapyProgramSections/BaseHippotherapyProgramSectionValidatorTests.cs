using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;
using VictoryCenter.BLL.Validators.HippotherapyProgramSections;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.HippotherapyProgramSections;

public class BaseHippotherapyProgramSectionValidatorTests
{
    private const ProgramSectionTemplate FaqTemplate = ProgramSectionTemplate.SingleTitleQuestionAnswerPairs;

    private readonly BaseHippotherapyProgramSectionValidator _validator = new();

    [Fact]
    public void Validate_TemplateIsInvalid_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.TextOnly) with
        {
            Template = (ProgramSectionTemplate)999
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Template)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeValidEnum(nameof(CreateHippotherapyProgramSectionDto.Template)));
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
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeGreaterThan(nameof(CreateHippotherapyProgramSectionDto.Order), -1));
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
    public void Validate_OrdersContainDuplicates_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.TextOnly);
        var description = model.Contents.First(c => c.ContentType == ContentType.Description);

        model.Contents.Remove(description);
        model.Contents.Add(description with { Order = 0 });

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(CreateProgramSectionContentDto.Order)));
    }

    [Fact]
    public void Validate_DualTitleDescriptionPairs_GroupIndexIsMissing_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.DualTitleDescriptionPairs);
        model.Contents = [.. model.Contents.Select(c => c with { GroupIndex = null })];

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(HippotherapyProgramSectionConstants.GetGroupIndexRequiredErrorMessage(model));
    }

    [Fact]
    public void Validate_TextOnly_WithAuthor_ShouldHaveAuthorsCountError()
    {
        var model = GetValidModel(ProgramSectionTemplate.TextOnly);
        var maxOrder = model.Contents.Max(c => c.Order);

        model.Contents.Add(new CreateProgramSectionContentDto
        {
            ContentType = ContentType.Author,
            Order = maxOrder + 1,
            Author = "Author"
        });

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(HippotherapyProgramSectionConstants.GetAuthorsCountErrorMessage(model));
    }

    [Fact]
    public void Validate_Template12_ValidModel_ShouldNotHaveErrors()
    {
        var model = GetValidModel(ProgramSectionTemplate.SingleTitleDescriptionAuthorPairs);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_Template12_AuthorIsEmpty_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.SingleTitleDescriptionAuthorPairs);
        var author = model.Contents.First(c => c.ContentType == ContentType.Author);

        model.Contents.Remove(author);
        model.Contents.Add(author with { Author = " " });

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateProgramSectionContentDto.Author)));
    }

    [Fact]
    public void Validate_Template12_AuthorIsTooShort_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.SingleTitleDescriptionAuthorPairs);
        var min = HippotherapyProgramSectionConstants.GetRequirements(model.Template).AuthorLength.Min;
        var author = model.Contents.First(c => c.ContentType == ContentType.Author);

        model.Contents.Remove(author);
        model.Contents.Add(author with { Author = new string('A', min - 1) });

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(HippotherapyProgramSectionConstants.GetAuthorLengthErrorMessage(model));
    }

    [Fact]
    public void Validate_Template12_GroupIndexIsMissing_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.SingleTitleDescriptionAuthorPairs);
        model.Contents = [.. model.Contents.Select(c => c.ContentType == ContentType.Title ? c : c with { GroupIndex = null })];

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(HippotherapyProgramSectionConstants.GetGroupIndexRequiredErrorMessage(model));
    }

    [Fact]
    public void Validate_Template12_GroupCompositionIsInvalid_ShouldHaveError()
    {
        var model = GetValidModel(ProgramSectionTemplate.SingleTitleDescriptionAuthorPairs);
        var maxOrder = model.Contents.Max(c => c.Order);

        model.Contents.Add(new CreateProgramSectionContentDto
        {
            ContentType = ContentType.Author,
            Order = maxOrder + 1,
            GroupIndex = 0,
            Author = "Extra author"
        });

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(HippotherapyProgramSectionConstants.GetGroupCompositionErrorMessage(model));
    }

    [Fact]
    public void Validate_ValidModel_ShouldNotHaveErrors()
    {
        var model = GetValidModel(ProgramSectionTemplate.QuadImagesBottom);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_FaqTemplate_ValidNewFaqQuestion_ShouldNotHaveErrors()
    {
        var model = GetValidFaqModel();

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_FaqTemplate_FaqQuestionObjectIsNull_ShouldHaveError()
    {
        var model = GetValidFaqModel();
        model.Contents.RemoveAll(c => c.ContentType == ContentType.FaqQuestion);
        model.Contents.Add(new CreateProgramSectionContentDto
        {
            ContentType = ContentType.FaqQuestion,
            Order = 1,
            FaqQuestion = null
        });

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateProgramSectionContentDto.FaqQuestion)));
    }

    [Fact]
    public void Validate_FaqTemplate_NewFaqQuestion_QuestionTextIsEmpty_ShouldHaveError()
    {
        var model = GetValidFaqModel();
        model.Contents.RemoveAll(c => c.ContentType == ContentType.FaqQuestion);
        model.Contents.Add(FaqContent(order: 1, questionText: " ", answerText: ValidAnswerText()));

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateFaqQuestionDto.QuestionText)));
    }

    [Fact]
    public void Validate_FaqTemplate_NewFaqQuestion_QuestionTextInvalidLength_ShouldHaveError()
    {
        var model = GetValidFaqModel();
        model.Contents.RemoveAll(c => c.ContentType == ContentType.FaqQuestion);
        model.Contents.Add(FaqContent(order: 1, questionText: "X", answerText: ValidAnswerText()));

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(HippotherapyProgramSectionConstants.GetQuestionTextLengthErrorMessage(model));
    }

    [Fact]
    public void Validate_FaqTemplate_NewFaqQuestion_AnswerTextIsEmpty_ShouldHaveError()
    {
        var model = GetValidFaqModel();
        model.Contents.RemoveAll(c => c.ContentType == ContentType.FaqQuestion);
        model.Contents.Add(FaqContent(order: 1, questionText: ValidQuestionText(), answerText: " "));

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateFaqQuestionDto.AnswerText)));
    }

    [Fact]
    public void Validate_FaqTemplate_NewFaqQuestion_AnswerTextInvalidLength_ShouldHaveError()
    {
        var model = GetValidFaqModel();
        model.Contents.RemoveAll(c => c.ContentType == ContentType.FaqQuestion);
        model.Contents.Add(FaqContent(order: 1, questionText: ValidQuestionText(), answerText: "X"));

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(HippotherapyProgramSectionConstants.GetAnswerTextLengthErrorMessage(model));
    }

    [Fact]
    public void Validate_FaqTemplate_ExistingFaqQuestion_SkipsTextValidation_ShouldNotHaveErrors()
    {
        var model = GetValidFaqModel();
        model.Contents.RemoveAll(c => c.ContentType == ContentType.FaqQuestion);
        model.Contents.Add(new CreateProgramSectionContentDto
        {
            ContentType = ContentType.FaqQuestion,
            Order = 1,
            FaqQuestion = new CreateFaqQuestionDto { Id = 5, QuestionText = "", AnswerText = "" }
        });

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Contents);
    }

    [Fact]
    public void Validate_FaqTemplate_FaqQuestionsCountIsZero_ShouldHaveError()
    {
        var model = GetValidFaqModel();
        model.Contents.RemoveAll(c => c.ContentType == ContentType.FaqQuestion);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Contents)
            .WithErrorMessage(HippotherapyProgramSectionConstants.GetFaqQuestionsCountErrorMessage(model));
    }

    private static CreateHippotherapyProgramSectionDto GetValidModel(ProgramSectionTemplate template)
    {
        var req = HippotherapyProgramSectionConstants.GetRequirements(template);
        var contents = new List<CreateProgramSectionContentDto>();
        var order = 0;

        if (req.Grouping is null)
        {
            AddUngroupedContents(req, contents, ref order);
            return BuildSectionDto(template, contents);
        }

        AddUngroupedNonGroupedTypes(req, contents, ref order);
        AddGroupedContents(req, contents, ref order);

        return BuildSectionDto(template, contents);
    }

    private static CreateHippotherapyProgramSectionDto BuildSectionDto(
        ProgramSectionTemplate template,
        List<CreateProgramSectionContentDto> contents)
    {
        return new CreateHippotherapyProgramSectionDto
        {
            Template = template,
            Order = 0,
            Contents = contents
        };
    }

    private static void AddUngroupedContents(
        HippotherapyProgramSectionConstants.TemplateRequirementsConfig req,
        List<CreateProgramSectionContentDto> contents,
        ref int order)
    {
        AddTitles(req, contents, ref order);
        AddDescriptions(req, contents, ref order);
        AddImages(req, contents, ref order);
        AddAuthors(req, contents, ref order);
    }

    private static void AddUngroupedNonGroupedTypes(
        HippotherapyProgramSectionConstants.TemplateRequirementsConfig req,
        List<CreateProgramSectionContentDto> contents,
        ref int order)
    {
        var groupedTypes = GetGroupedTypes(req);

        if (!groupedTypes.Contains(ContentType.Title))
        {
            AddTitles(req, contents, ref order);
        }

        if (!groupedTypes.Contains(ContentType.Description))
        {
            AddDescriptions(req, contents, ref order);
        }

        if (!groupedTypes.Contains(ContentType.Image))
        {
            AddImages(req, contents, ref order);
        }

        if (!groupedTypes.Contains(ContentType.Author))
        {
            AddAuthors(req, contents, ref order);
        }
    }

    private static HashSet<ContentType> GetGroupedTypes(HippotherapyProgramSectionConstants.TemplateRequirementsConfig req)
    {
        return [.. req.Grouping!.PerGroupCounts.Keys];
    }

    private static void AddGroupedContents(
        HippotherapyProgramSectionConstants.TemplateRequirementsConfig req,
        List<CreateProgramSectionContentDto> contents,
        ref int order)
    {
        for (var g = 0; g < req.Grouping!.GroupCount.Min; g++)
        {
            AddGroupedByType(req, contents, ref order, ContentType.Title, g);
            AddGroupedByType(req, contents, ref order, ContentType.Description, g);
            AddGroupedByType(req, contents, ref order, ContentType.Image, g);
            AddGroupedByType(req, contents, ref order, ContentType.Author, g);
        }
    }

    private static void AddGroupedByType(
        HippotherapyProgramSectionConstants.TemplateRequirementsConfig req,
        List<CreateProgramSectionContentDto> contents,
        ref int order,
        ContentType type,
        int groupIndex)
    {
        if (!req.Grouping!.PerGroupCounts.TryGetValue(type, out var count))
        {
            return;
        }

        for (var i = 0; i < count.Min; i++)
        {
            contents.Add(CreateGroupedContent(req, type, groupIndex, ref order));
        }
    }

    private static CreateProgramSectionContentDto CreateGroupedContent(
        HippotherapyProgramSectionConstants.TemplateRequirementsConfig req,
        ContentType type,
        int groupIndex,
        ref int order)
    {
        if (type == ContentType.Title)
        {
            return new CreateProgramSectionContentDto
            {
                ContentType = ContentType.Title,
                Order = order++,
                GroupIndex = groupIndex,
                Title = new string('T', req.TitleLength.Min)
            };
        }

        if (type == ContentType.Description)
        {
            return new CreateProgramSectionContentDto
            {
                ContentType = ContentType.Description,
                Order = order++,
                GroupIndex = groupIndex,
                Description = new string('D', req.DescriptionLength.Min)
            };
        }

        if (type == ContentType.Image)
        {
            return new CreateProgramSectionContentDto
            {
                ContentType = ContentType.Image,
                Order = order++,
                GroupIndex = groupIndex,
                ImageId = 1
            };
        }

        if (type == ContentType.Author)
        {
            return new CreateProgramSectionContentDto
            {
                ContentType = ContentType.Author,
                Order = order++,
                GroupIndex = groupIndex,
                Author = new string('A', Math.Max(req.AuthorLength.Min, 1))
            };
        }

        throw new ArgumentOutOfRangeException(nameof(type), type, null);
    }

    private static void AddTitles(
        HippotherapyProgramSectionConstants.TemplateRequirementsConfig req,
        List<CreateProgramSectionContentDto> contents,
        ref int order)
    {
        for (var i = 0; i < req.TitleCount.Min; i++)
        {
            contents.Add(new CreateProgramSectionContentDto
            {
                ContentType = ContentType.Title,
                Order = order++,
                Title = new string('T', req.TitleLength.Min)
            });
        }
    }

    private static void AddDescriptions(
        HippotherapyProgramSectionConstants.TemplateRequirementsConfig req,
        List<CreateProgramSectionContentDto> contents,
        ref int order)
    {
        for (var i = 0; i < req.DescriptionCount.Min; i++)
        {
            contents.Add(new CreateProgramSectionContentDto
            {
                ContentType = ContentType.Description,
                Order = order++,
                Description = new string('D', req.DescriptionLength.Min)
            });
        }
    }

    private static void AddImages(
        HippotherapyProgramSectionConstants.TemplateRequirementsConfig req,
        List<CreateProgramSectionContentDto> contents,
        ref int order)
    {
        for (var i = 0; i < req.ImageCount.Min; i++)
        {
            contents.Add(new CreateProgramSectionContentDto
            {
                ContentType = ContentType.Image,
                Order = order++,
                ImageId = i + 1
            });
        }
    }

    private static void AddAuthors(
        HippotherapyProgramSectionConstants.TemplateRequirementsConfig req,
        List<CreateProgramSectionContentDto> contents,
        ref int order)
    {
        for (var i = 0; i < req.AuthorCount.Min; i++)
        {
            contents.Add(new CreateProgramSectionContentDto
            {
                ContentType = ContentType.Author,
                Order = order++,
                Author = new string('A', Math.Max(req.AuthorLength.Min, 1))
            });
        }
    }

    private static CreateHippotherapyProgramSectionDto GetValidFaqModel()
    {
        var req = HippotherapyProgramSectionConstants.GetRequirements(FaqTemplate);
        var order = 0;
        var contents = new List<CreateProgramSectionContentDto>();

        AddTitles(req, contents, ref order);
        contents.Add(FaqContent(order: order++, questionText: ValidQuestionText(), answerText: ValidAnswerText()));

        return BuildSectionDto(FaqTemplate, contents);
    }

    private static CreateProgramSectionContentDto FaqContent(int order, string questionText, string answerText)
    {
        return new CreateProgramSectionContentDto
        {
            ContentType = ContentType.FaqQuestion,
            Order = order,
            FaqQuestion = new CreateFaqQuestionDto { QuestionText = questionText, AnswerText = answerText }
        };
    }

    private static string ValidQuestionText()
    {
        var min = HippotherapyProgramSectionConstants.GetRequirements(FaqTemplate).QuestionTextLength.Min;
        return new string('Q', min);
    }

    private static string ValidAnswerText()
    {
        var min = HippotherapyProgramSectionConstants.GetRequirements(FaqTemplate).AnswerTextLength.Min;
        return new string('A', min);
    }
}
