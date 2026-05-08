using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Entities.HistoryContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Entities.WhoWeAreContents;

namespace VictoryCenter.DAL.Data;

public class VictoryCenterDbContext : IdentityDbContext<AdminUser, IdentityRole<int>, int>
{
    public VictoryCenterDbContext(DbContextOptions<VictoryCenterDbContext> options)
        : base(options)
    {
    }

    public DbSet<TeamCategory> TeamCategories { get; set; }

    public DbSet<VisitorPage> VisitorPages { get; set; }

    public DbSet<FaqPlacement> FaqPlacements { get; set; }

    public DbSet<FaqQuestion> FaqQuestions { get; set; }

    public DbSet<TeamMember> TeamMembers { get; set; }

    public DbSet<Image> Images { get; set; }

    public DbSet<PdfSection> PdfSections { get; set; }

    public DbSet<PdfSectionLocalization> PdfSectionLocalizations { get; set; }

    public DbSet<PdfReport> PdfReports { get; set; }

    public DbSet<HippotherapyProgramCategory> HippotherapyProgramCategories { get; set; }

    public DbSet<HippotherapyProgram> HippotherapyPrograms { get; set; }

    public DbSet<HippotherapyProgramSection> HippotherapyProgramSections { get; set; }

    public DbSet<ProgramSectionContent> ProgramSectionContents { get; set; }

    public DbSet<WhoWeAreSection> WhoWeAreSections { get; set; }

    public DbSet<WhoWeAreContent> WhoWeAreContents { get; set; }

    public DbSet<LocalizationLanguage> LocalizationLanguages { get; set; }

    public DbSet<TeamMemberLocalization> TeamMemberLocalizations { get; set; }

    public DbSet<TeamCategoryLocalization> TeamCategoryLocalizations { get; set; }

    public DbSet<HippotherapyProgramLocalization> HippotherapyProgramLocalizations { get; set; }

    public DbSet<FaqQuestionLocalization> FaqLocalizations { get; set; }

    public DbSet<ProgramSectionContentLocalization> ProgramSectionContentLocalizations { get; set; }

    public DbSet<WhoWeAreContentLocalization> WhoWeAreContentLocalizations { get; set; }

    public DbSet<UahBankDetails> UahBankDetails { get; set; }

    public DbSet<ForeignBankDetails> ForeignBankDetails { get; set; }

    public DbSet<CorrespondentBankDetails> CorrespondentBankDetails { get; set; }

    public DbSet<SupportOptions> SupportOptions { get; set; }

    public DbSet<PartnerSection> PartnersSections { get; set; }

    public DbSet<Partner> Partners { get; set; }

    public DbSet<PartnersPageBanner> PartnersPageBanners { get; set; }

    public DbSet<ChangedLivesBlock> ChangedLivesBlocks { get; set; }

    public DbSet<CollectedFundsBlock> CollectedFundsBlocks { get; set; }

    public DbSet<CompanyProfile> CompanyProfiles { get; set; }

    public DbSet<CompanyProfileContact> CompanyProfileContacts { get; set; }

    public DbSet<CompanyProfileContactLocalization> CompanyProfileContactLocalizations { get; set; }

    public DbSet<CompanyProfileRequisite> CompanyProfileRequisites { get; set; }

    public DbSet<CompanyProfileRequisiteLocalization> CompanyProfileRequisiteLocalizations { get; set; }

    public DbSet<CompanyProfileSocialLink> CompanyProfileSocialLinks { get; set; }

    public DbSet<ReportFundsExpendituresSettings> ReportFundsExpendituresSettings { get; set; }

    public DbSet<ReportFundsExpendituresCategory> ReportFundsExpendituresCategories { get; set; }

    public DbSet<ReportFundsExpendituresRecord> ReportFundsExpendituresRecords { get; set; }

    public DbSet<ReportProgramExpendituresRecord> ReportProgramExpendituresRecords { get; set; }

    public DbSet<MainPage> MainPages { get; set; }

    public DbSet<MainAboutUs> MainAboutUs { get; set; }

    public DbSet<MainPartners> MainPartners { get; set; }

    public DbSet<ImpactStatistics> ImpactStatistics { get; set; }

    public DbSet<MainPageLocalization> MainPageLocalizations { get; set; }

    public DbSet<MainAboutUsLocalization> MainAboutUsLocalizations { get; set; }

    public DbSet<MainPartnersLocalization> MainPartnersLocalizations { get; set; }

    public DbSet<ImpactStatisticsLocalization> ImpactStatisticsLocalizations { get; set; }

    public DbSet<Metric> Metrics { get; set; }

    public DbSet<MetricLocalization> MetricLocalizations { get; set; }

    public DbSet<HistorySection> HistorySections { get; set; }

    public DbSet<HistorySectionContent> HistorySectionContents { get; set; }

    public DbSet<HistorySectionContentLocalization> HistorySectionContentLocalizations { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(VictoryCenterDbContext).Assembly);
    }
}
