using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using VictoryCenter.BLL.DTOs.Public.PdfSection;
using VictoryCenter.BLL.Hubs;
using VictoryCenter.DAL.Entities.Localization;
using PdfSectionEntity = VictoryCenter.DAL.Entities.PdfSection;

namespace VictoryCenter.BLL.Helpers;

internal static class PdfSectionNotificationHelper
{
    internal static async Task BroadcastUpdatedAsync(
        IHubContext<PdfReportsHub> hubContext,
        IMapper mapper,
        PdfSectionEntity section,
        PdfSectionLocalization? updatedLocalization,
        CancellationToken cancellationToken)
    {
        var mergedLocalizations = (section.Localizations ?? [])
            .Where(loc => updatedLocalization is null || loc.LanguageId != updatedLocalization.LanguageId)
            .ToList();

        if (updatedLocalization is not null)
        {
            mergedLocalizations.Add(updatedLocalization);
        }

        var snapshot = new PdfSectionEntity
        {
            Id = section.Id,
            Title = section.Title,
            Description = section.Description,
            CreatedAt = section.CreatedAt,
            Localizations = mergedLocalizations,
        };

        var publicDto = mapper.Map<PublicPdfSectionDto>(snapshot);

        await hubContext.Clients.All.SendAsync(
            "PdfSectionUpdated",
            publicDto,
            cancellationToken: cancellationToken);
    }
}
