using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.HippotherapyLandingPage.Update;
using VictoryCenter.BLL.Validators.HippotherapyLandingPage.Dto;

namespace VictoryCenter.BLL.Validators.HippotherapyLandingPage.Commands;

public class UpdateHippotherapyLandingPageCommandValidator : AbstractValidator<UpdateHippotherapyLandingPageCommand>
{
    public UpdateHippotherapyLandingPageCommandValidator(
        UpdateIntroSectionDtoValidator introValidator,
        UpdateTextSectionDtoValidator textValidator,
        UpdateQuoteSectionDtoValidator quoteValidator,
        UpdateHippoventionCenterSectionDtoValidator hippoventionCenterValidator,
        UpdateGallerySectionDtoValidator galleryValidator,
        UpdateScientificReferencesSectionDtoValidator scientificReferencesValidator,
        UpdateEthicsSectionDtoValidator ethicsValidator)
    {
        RuleFor(x => x.Dto.IntroSection).NotNull().SetValidator(introValidator);
        RuleFor(x => x.Dto.DescriptionSection).NotNull().SetValidator(textValidator);
        RuleFor(x => x.Dto.QuoteSection).NotNull().SetValidator(quoteValidator);
        RuleFor(x => x.Dto.HippoventionSection).NotNull().SetValidator(textValidator);
        RuleFor(x => x.Dto.HippoventionCenterSection).NotNull().SetValidator(hippoventionCenterValidator);
        RuleFor(x => x.Dto.AdvantagesSection).NotNull().SetValidator(galleryValidator);
        RuleFor(x => x.Dto.AnalysisSection).NotNull().SetValidator(textValidator);
        RuleFor(x => x.Dto.ScientificReferencesSection).NotNull().SetValidator(scientificReferencesValidator);
        RuleFor(x => x.Dto.AnotherQuoteSection).NotNull().SetValidator(quoteValidator);
        RuleFor(x => x.Dto.ParticipantsSection).NotNull().SetValidator(galleryValidator);
        RuleFor(x => x.Dto.EthicsSection).NotNull().SetValidator(ethicsValidator);
    }
}
