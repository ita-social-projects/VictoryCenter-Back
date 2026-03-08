using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
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

    public DbSet<ReportFundsExpendituresSettings> ReportFundsExpendituresSettings { get; set; }

    public DbSet<ReportFundsExpendituresCategory> ReportFundsExpendituresCategories { get; set; }

    public DbSet<ReportFundsExpendituresRecord> ReportFundsExpendituresRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(VictoryCenterDbContext).Assembly);
    }
}
