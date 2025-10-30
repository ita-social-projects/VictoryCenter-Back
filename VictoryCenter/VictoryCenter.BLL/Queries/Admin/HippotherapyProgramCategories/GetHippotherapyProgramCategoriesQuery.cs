using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramCategories;

namespace VictoryCenter.BLL.Queries.Admin.HippotherapyProgramCategories;

public record GetHippotherapyProgramCategoriesQuery : IRequest<Result<List<HippotherapyProgramCategoryDto>>>;
