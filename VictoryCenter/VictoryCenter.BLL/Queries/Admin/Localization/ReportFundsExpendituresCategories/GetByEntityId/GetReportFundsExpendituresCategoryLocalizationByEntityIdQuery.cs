using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresCategories;

namespace VictoryCenter.BLL.Queries.Admin.Localization.ReportFundsExpendituresCategories.GetByEntityId;

public record GetReportFundsExpendituresCategoryLocalizationByEntityIdQuery(long Id)
    : IRequest<Result<List<ReportFundsExpendituresCategoryLocalizationDto>>>;
