using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.HippotherapyLandingPage.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;
using VictoryCenter.BLL.Validators.HippotherapyLandingPage.Commands;
using VictoryCenter.BLL.Validators.HippotherapyLandingPage.Dto;

namespace VictoryCenter.UnitTests.ValidatorsTests.HippotherapyLandingPage;

public class UpdateHippotherapyLandingPageCommandValidatorTests
{
    private readonly UpdateHippotherapyLandingPageCommandValidator _validator = new(
        new UpdateIntroSectionDtoValidator(),
        new UpdateTextSectionDtoValidator(),
        new UpdateQuoteSectionDtoValidator(),
        new UpdateHippoventionCenterSectionDtoValidator(),
        new UpdateGallerySectionDtoValidator(new UpdateGalleryCardDtoValidator()),
        new UpdateScientificReferencesSectionDtoValidator(new UpdateScientificReferenceDtoValidator()),
        new UpdateEthicsSectionDtoValidator());

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new UpdateHippotherapyLandingPageCommand(GetValidDto());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_IntroSectionIsNull_ShouldHaveError()
    {
        AssertSectionRequired(dto => dto with { IntroSection = null! }, x => x.Dto.IntroSection);
    }

    [Fact]
    public void Validate_DescriptionSectionIsNull_ShouldHaveError()
    {
        AssertSectionRequired(dto => dto with { DescriptionSection = null! }, x => x.Dto.DescriptionSection);
    }

    [Fact]
    public void Validate_QuoteSectionIsNull_ShouldHaveError()
    {
        AssertSectionRequired(dto => dto with { QuoteSection = null! }, x => x.Dto.QuoteSection);
    }

    [Fact]
    public void Validate_HippoventionSectionIsNull_ShouldHaveError()
    {
        AssertSectionRequired(dto => dto with { HippoventionSection = null! }, x => x.Dto.HippoventionSection);
    }

    [Fact]
    public void Validate_HippoventionCenterSectionIsNull_ShouldHaveError()
    {
        AssertSectionRequired(dto => dto with { HippoventionCenterSection = null! }, x => x.Dto.HippoventionCenterSection);
    }

    [Fact]
    public void Validate_AdvantagesSectionIsNull_ShouldHaveError()
    {
        AssertSectionRequired(dto => dto with { AdvantagesSection = null! }, x => x.Dto.AdvantagesSection);
    }

    [Fact]
    public void Validate_AnalysisSectionIsNull_ShouldHaveError()
    {
        AssertSectionRequired(dto => dto with { AnalysisSection = null! }, x => x.Dto.AnalysisSection);
    }

    [Fact]
    public void Validate_ScientificReferencesSectionIsNull_ShouldHaveError()
    {
        AssertSectionRequired(dto => dto with { ScientificReferencesSection = null! }, x => x.Dto.ScientificReferencesSection);
    }

    [Fact]
    public void Validate_AnotherQuoteSectionIsNull_ShouldHaveError()
    {
        AssertSectionRequired(dto => dto with { AnotherQuoteSection = null! }, x => x.Dto.AnotherQuoteSection);
    }

    [Fact]
    public void Validate_ParticipantsSectionIsNull_ShouldHaveError()
    {
        AssertSectionRequired(dto => dto with { ParticipantsSection = null! }, x => x.Dto.ParticipantsSection);
    }

    [Fact]
    public void Validate_EthicsSectionIsNull_ShouldHaveError()
    {
        AssertSectionRequired(dto => dto with { EthicsSection = null! }, x => x.Dto.EthicsSection);
    }

    [Fact]
    public void Validate_NestedSectionIsInvalid_ShouldPropagateNestedError()
    {
        // Arrange
        var dto = GetValidDto() with { IntroSection = GetValidIntroSection() with { Title = string.Empty } };
        var command = new UpdateHippotherapyLandingPageCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.IntroSection.Title);
    }

    private void AssertSectionRequired<T>(
        Func<UpdateHippotherapyLandingPageDto, UpdateHippotherapyLandingPageDto> mutate,
        System.Linq.Expressions.Expression<Func<UpdateHippotherapyLandingPageCommand, T>> propertyExpression)
    {
        // Arrange
        var dto = mutate(GetValidDto());
        var command = new UpdateHippotherapyLandingPageCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(propertyExpression);
    }

    private static UpdateIntroSectionDto GetValidIntroSection() => new()
    {
        Title = new string('A', HippotherapyLandingPageConstants.TitleMinLength),
        Description = new string('A', HippotherapyLandingPageConstants.TextMinLength),
        ImageId = 1,
    };

    private static UpdateTextSectionDto GetValidTextSection() => new()
    {
        Title = new string('A', HippotherapyLandingPageConstants.TitleMinLength),
        Description = new string('A', HippotherapyLandingPageConstants.TextMinLength),
    };

    private static UpdateQuoteSectionDto GetValidQuoteSection() => new()
    {
        QuoteText = new string('A', HippotherapyLandingPageConstants.TextMinLength),
        AuthorName = null,
        ImageId = 1,
    };

    private static UpdateGallerySectionDto GetValidGallerySection() => new()
    {
        Title = new string('A', HippotherapyLandingPageConstants.TitleMinLength),
        Cards = Enumerable.Range(0, HippotherapyLandingPageConstants.GalleryCardsCount)
            .Select(_ => new UpdateGalleryCardDto
            {
                Description = new string('A', HippotherapyLandingPageConstants.TextMinLength),
                ImageId = 1,
            })
            .ToList(),
    };

    private static UpdateHippotherapyLandingPageDto GetValidDto() => new()
    {
        IntroSection = GetValidIntroSection(),
        DescriptionSection = GetValidTextSection(),
        QuoteSection = GetValidQuoteSection(),
        HippoventionSection = GetValidTextSection(),
        HippoventionCenterSection = new UpdateHippoventionCenterSectionDto
        {
            Title = new string('A', HippotherapyLandingPageConstants.TitleMinLength),
            Description = new string('A', HippotherapyLandingPageConstants.TextMinLength),
            Pros = new string('A', HippotherapyLandingPageConstants.TextMinLength),
            ImageId = 1,
        },
        AdvantagesSection = GetValidGallerySection(),
        AnalysisSection = GetValidTextSection(),
        ScientificReferencesSection = new UpdateScientificReferencesSectionDto
        {
            Title = new string('A', HippotherapyLandingPageConstants.TitleMinLength),
            Description = new string('A', HippotherapyLandingPageConstants.TextMinLength),
            ScientificReferences =
            [
                new UpdateScientificReferenceDto
                {
                    Id = null,
                    Name = new string('A', HippotherapyLandingPageConstants.ScientificReferenceNameMinLength),
                    Url = "https://example.com/reference",
                },
            ],
        },
        AnotherQuoteSection = GetValidQuoteSection(),
        ParticipantsSection = GetValidGallerySection(),
        EthicsSection = new UpdateEthicsSectionDto
        {
            Title = new string('A', HippotherapyLandingPageConstants.TitleMinLength),
            Description = new string('A', HippotherapyLandingPageConstants.TextMinLength),
            Principles = Enumerable.Range(0, HippotherapyLandingPageConstants.EthicsPrinciplesCount)
                .Select(_ => new string('A', HippotherapyLandingPageConstants.TextMinLength))
                .ToList(),
            ImageId = 1,
        },
    };
}
