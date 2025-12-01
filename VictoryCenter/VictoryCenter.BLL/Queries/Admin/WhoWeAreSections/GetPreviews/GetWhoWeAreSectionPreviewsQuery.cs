using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreSection;

namespace VictoryCenter.BLL.Queries.Admin.WhoWeAreSections.GetPreviews;

public record GetWhoWeAreSectionPreviewsQuery : IRequest<Result<List<WhoWeAreSectionInfoDto>>>;
