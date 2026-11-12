using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Entities.HistoryContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Entities.WhoWeAreContents;

namespace VictoryCenter.DAL.Data;

public class VictoryCenterDbContext : IdentityDbContext<AdminUser, IdentityRole<int>, int>
{
    private readonly TimeProvider _clock;

    public VictoryCenterDbContext(DbContextOptions<VictoryCenterDbContext> options, TimeProvider? clock = null)
        : base(options)
    {
        _clock = clock ?? TimeProvider.System;
    }

    public DbSet<TeamCategory> TeamCategories { get; set; }

    public DbSet<VisitorPage> VisitorPages { get; set; }

    public DbSet<FaqPlacement> FaqPlacements { get; set; }

    public DbSet<FaqQuestion> FaqQuestions { get; set; }

    public DbSet<TeamMember> TeamMembers { get; set; }

    public DbSet<Image> Images { get; set; }

    public DbSet<StorageOperation> StorageOperations { get; set; }

    public DbSet<StorageReconciliationFinding> StorageReconciliationFindings { get; set; }

    public DbSet<PdfSection> PdfSections { get; set; }

    public DbSet<PdfSectionLocalization> PdfSectionLocalizations { get; set; }

    public DbSet<PdfReport> PdfReports { get; set; }

    public DbSet<HippotherapyProgramCategory> HippotherapyProgramCategories { get; set; }

    public DbSet<HippotherapyProgramCategoryLocalization> HippotherapyProgramCategoryLocalizations { get; set; }

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

    public DbSet<PartnersPageBannerLocalization> PartnersPageBannerLocalizations { get; set; }

    public DbSet<PartnerSectionLocalization> PartnerSectionLocalizations { get; set; }

    public DbSet<PartnerLocalization> PartnerLocalizations { get; set; }

    public DbSet<ChangedLivesBlock> ChangedLivesBlocks { get; set; }

    public DbSet<CollectedFundsBlock> CollectedFundsBlocks { get; set; }

    public DbSet<CompanyProfile> CompanyProfiles { get; set; }

    public DbSet<CompanyProfileContact> CompanyProfileContacts { get; set; }

    public DbSet<CompanyProfileContactLocalization> CompanyProfileContactLocalizations { get; set; }

    public DbSet<CompanyProfileRequisite> CompanyProfileRequisites { get; set; }

    public DbSet<CompanyProfileRequisiteLocalization> CompanyProfileRequisiteLocalizations { get; set; }

    public DbSet<CompanyProfileSocialLink> CompanyProfileSocialLinks { get; set; }

    public DbSet<ReportFundsExpendituresSettings> ReportFundsExpendituresSettings { get; set; }

    public DbSet<ReportFundsExpendituresSettingsLocalization> ReportFundsExpendituresSettingsLocalizations { get; set; }

    public DbSet<ReportFundsExpendituresCategory> ReportFundsExpendituresCategories { get; set; }

    public DbSet<ReportFundsExpendituresCategoryLocalization> ReportFundsExpendituresCategoryLocalizations { get; set; }

    public DbSet<ReportFundsExpendituresRecord> ReportFundsExpendituresRecords { get; set; }

    public DbSet<ReportProgramExpendituresRecord> ReportProgramExpendituresRecords { get; set; }

    public DbSet<PublishedReportFundsExpendituresRecord> PublishedReportFundsExpendituresRecords { get; set; }

    public DbSet<PublishedReportProgramExpendituresRecord> PublishedReportProgramExpendituresRecords { get; set; }

    public DbSet<PublishedReportFundsExpendituresSnapshot> PublishedReportFundsExpendituresSnapshots { get; set; }

    public DbSet<BackupReportFundsExpendituresSettings> BackupReportFundsExpendituresSettings { get; set; }

    public DbSet<BackupReportFundsExpendituresSettingsLocalization> BackupReportFundsExpendituresSettingsLocalizations { get; set; }

    public DbSet<BackupReportFundsExpendituresRecord> BackupReportFundsExpendituresRecords { get; set; }

    public DbSet<BackupReportProgramExpendituresRecord> BackupReportProgramExpendituresRecords { get; set; }

    public DbSet<MainPage> MainPages { get; set; }

    public DbSet<MainAboutUs> MainAboutUs { get; set; }

    public DbSet<MainPartners> MainPartners { get; set; }

    public DbSet<MainDonations> MainDonations { get; set; }

    public DbSet<ImpactStatistics> ImpactStatistics { get; set; }

    public DbSet<MainPageLocalization> MainPageLocalizations { get; set; }

    public DbSet<MainAboutUsLocalization> MainAboutUsLocalizations { get; set; }

    public DbSet<MainPartnersLocalization> MainPartnersLocalizations { get; set; }

    public DbSet<MainDonationsLocalization> MainDonationsLocalizations { get; set; }

    public DbSet<ImpactStatisticsLocalization> ImpactStatisticsLocalizations { get; set; }

    public DbSet<Metric> Metrics { get; set; }

    public DbSet<MetricLocalization> MetricLocalizations { get; set; }

    public DbSet<HistorySection> HistorySections { get; set; }

    public DbSet<HistorySectionContent> HistorySectionContents { get; set; }

    public DbSet<HistorySectionContentLocalization> HistorySectionContentLocalizations { get; set; }

    public DbSet<EventNewsCategory> EventNewsCategories { get; set; }

    public DbSet<EventNewsCategoryLocalization> EventNewsCategoryLocalizations { get; set; }

    public DbSet<EventNews> EventNews { get; set; }

    public DbSet<EventNewsLocalization> EventNewsLocalizations { get; set; }

    public DbSet<EventsIntroSection> EventsIntroSections { get; set; }

    public DbSet<EventsIntroSectionLocalization> EventsIntroSectionLocalizations { get; set; }

    public DbSet<HippotherapyLandingPage> HippotherapyLandingPages { get; set; }

    public DbSet<HippotherapyLandingPageIntroSection> HippotherapyLandingPageIntroSections { get; set; }

    public DbSet<HippotherapyLandingPageIntroSectionLocalization> HippotherapyLandingPageIntroSectionLocalizations { get; set; }

    public DbSet<HippotherapyLandingPageDescriptionSectionLocalization> HippotherapyLandingPageDescriptionSectionLocalizations { get; set; }

    public DbSet<HippotherapyLandingPageHippoventionSectionLocalization> HippotherapyLandingPageHippoventionSectionLocalizations { get; set; }

    public DbSet<HippotherapyLandingPageAnalysisSectionLocalization> HippotherapyLandingPageAnalysisSectionLocalizations { get; set; }

    public DbSet<HippotherapyLandingPageScientificReferencesSectionLocalization> HippotherapyLandingPageScientificReferencesSectionLocalizations { get; set; }

    public DbSet<HippotherapyLandingPageScientificReferenceLocalization> HippotherapyLandingPageScientificReferenceLocalizations { get; set; }

    public DbSet<HippotherapyLandingPageEthicsSectionLocalization> HippotherapyLandingPageEthicsSectionLocalizations { get; set; }

    public DbSet<HippotherapyLandingPageEthicsPrincipleLocalization> HippotherapyLandingPageEthicsPrincipleLocalizations { get; set; }

    public DbSet<HippotherapyLandingPageQuoteSectionLocalization> HippotherapyLandingPageQuoteSectionLocalizations { get; set; }

    public DbSet<HippotherapyLandingPageAnotherQuoteSectionLocalization> HippotherapyLandingPageAnotherQuoteSectionLocalizations { get; set; }

    public DbSet<HippotherapyLandingPageParticipantsSectionLocalization> HippotherapyLandingPageParticipantsSectionLocalizations { get; set; }

    public DbSet<HippotherapyLandingPageParticipantCardLocalization> HippotherapyLandingPageParticipantCardLocalizations { get; set; }

    public DbSet<HippotherapyLandingPageHippoventionCenterSectionLocalization> HippotherapyLandingPageHippoventionCenterSectionLocalizations { get; set; }

    public DbSet<HippotherapyLandingPageAdvantagesSectionLocalization> HippotherapyLandingPageAdvantagesSectionLocalizations { get; set; }

    public DbSet<HippotherapyLandingPageAdvantageCardLocalization> HippotherapyLandingPageAdvantageCardLocalizations { get; set; }

    public DbSet<HippotherapyLandingPageDescriptionSection> HippotherapyLandingPageDescriptionSections { get; set; }

    public DbSet<HippotherapyLandingPageQuoteSection> HippotherapyLandingPageQuoteSections { get; set; }

    public DbSet<HippotherapyLandingPageHippoventionSection> HippotherapyLandingPageHippoventionSections { get; set; }

    public DbSet<HippotherapyLandingPageHippoventionCenterSection> HippotherapyLandingPageHippoventionCenterSections { get; set; }

    public DbSet<HippotherapyLandingPageAdvantagesSection> HippotherapyLandingPageAdvantagesSections { get; set; }

    public DbSet<HippotherapyLandingPageAnalysisSection> HippotherapyLandingPageAnalysisSections { get; set; }

    public DbSet<HippotherapyLandingPageScientificReferencesSection> HippotherapyLandingPageScientificReferencesSections { get; set; }

    public DbSet<HippotherapyLandingPageAnotherQuoteSection> HippotherapyLandingPageAnotherQuoteSections { get; set; }

    public DbSet<HippotherapyLandingPageParticipantsSection> HippotherapyLandingPageParticipantsSections { get; set; }

    public DbSet<HippotherapyLandingPageEthicsSection> HippotherapyLandingPageEthicsSections { get; set; }

    public DbSet<HippotherapyLandingPageEthicsPrinciple> HippotherapyLandingPageEthicsPrinciples { get; set; }

    public DbSet<HippotherapyLandingPageAdvantageCard> HippotherapyLandingPageAdvantageCards { get; set; }

    public DbSet<HippotherapyLandingPageParticipantCard> HippotherapyLandingPageParticipantCards { get; set; }

    public DbSet<HippotherapyLandingPageScientificReference> HippotherapyLandingPageScientificReferences { get; set; }
    public DbSet<FeedbackHistory> FeedbackHistories { get; set; }

    public DbSet<FeedbackHistoryLocalization> FeedbackHistoryLocalizations { get; set; }

    public DbSet<FeedbackReview> FeedbackReviews { get; set; }

    public DbSet<FeedbackReviewLocalization> FeedbackReviewLocalizations { get; set; }

    public DbSet<VideoReview> VideoReviews { get; set; }

    public DbSet<VideoReviewLocalization> VideoReviewLocalizations { get; set; }

    public DbSet<AuditLog> AuditLogs { get; set; }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        foreach (var (kind, name) in PrepareStorageChanges())
        {
            CompleteUploadIntent(StorageOperations.SingleOrDefault(operation => operation.FileKind == kind
                && operation.FileName == name && operation.Kind == StorageOperationKind.UploadCleanup));
        }

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        foreach (var (kind, name) in PrepareStorageChanges())
        {
            var operation = await StorageOperations.SingleOrDefaultAsync(
                operation => operation.FileKind == kind && operation.FileName == name
                    && operation.Kind == StorageOperationKind.UploadCleanup, cancellationToken);
            CompleteUploadIntent(operation);
        }

        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(VictoryCenterDbContext).Assembly);
    }

    private List<(StorageFileKind Kind, string Name)> PrepareStorageChanges()
    {
        List<(StorageFileKind Kind, string Name)> uploads = [];
        ChangeTracker.DetectChanges();
        foreach (var entry in ChangeTracker.Entries<Image>().ToArray())
        {
            var upload = PrepareImageChange(entry);
            if (upload != null)
            {
                uploads.Add((StorageFileKind.Image, upload));
            }
        }

        foreach (var entry in ChangeTracker.Entries<PdfReport>().ToArray())
        {
            if (entry.State == EntityState.Deleted)
            {
                QueueStorageDeletion(StorageFileKind.Pdf, entry.Entity.BlobName);
            }

            if (entry.State == EntityState.Added)
            {
                uploads.Add((StorageFileKind.Pdf, entry.Entity.BlobName));
            }
        }

        foreach (var entry in ChangeTracker.Entries<StorageOperation>().Where(entry => entry.State == EntityState.Added
            && entry.Entity.CreatedAt == default))
        {
            entry.Entity.CreatedAt = _clock.GetUtcNow();
        }

        return [.. uploads.Distinct()];
    }

    private string? PrepareImageChange(EntityEntry<Image> entry)
    {
        if (entry.State == EntityState.Added)
        {
            return StorageFileNames.Image(entry.Entity.BlobName, entry.Entity.MimeType);
        }

        if (entry.State is EntityState.Unchanged or EntityState.Detached)
        {
            return null;
        }

        var previous = StorageFileNames.Image(
            entry.OriginalValues.GetValue<string>(nameof(Image.BlobName)),
            entry.OriginalValues.GetValue<string>(nameof(Image.MimeType)));
        if (entry.State == EntityState.Deleted)
        {
            QueueStorageDeletion(StorageFileKind.Image, previous);
            return null;
        }

        entry.Entity.StorageVersion = Guid.NewGuid();
        var current = StorageFileNames.Image(entry.Entity.BlobName, entry.Entity.MimeType);
        if (previous == current)
        {
            return null;
        }

        QueueStorageDeletion(StorageFileKind.Image, previous);
        return current;
    }

    private void QueueStorageDeletion(StorageFileKind fileKind, string fileName)
    {
        if (!StorageOperations.Local.Any(operation => operation.FileKind == fileKind
            && operation.FileName == fileName && operation.Kind == StorageOperationKind.Delete
            && operation.Status == StorageOperationStatus.Pending))
        {
            StorageOperations.Add(new StorageOperation
            {
                FileKind = fileKind,
                FileName = fileName,
                Kind = StorageOperationKind.Delete,
                NextAttemptAt = _clock.GetUtcNow()
            });
        }
    }

    private void CompleteUploadIntent(StorageOperation? operation)
    {
        if (operation == null)
        {
            // Existing files and database-only seed data predate upload intents.
            return;
        }

        if (operation.Status != StorageOperationStatus.Pending)
        {
            throw new DbUpdateConcurrencyException("The upload reservation is no longer available.");
        }

        operation.Status = StorageOperationStatus.Completed;
        operation.CompletedAt = _clock.GetUtcNow();
        operation.Version = Guid.NewGuid();
    }
}
