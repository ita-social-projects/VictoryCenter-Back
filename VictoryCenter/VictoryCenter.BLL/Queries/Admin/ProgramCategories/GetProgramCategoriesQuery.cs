using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.ProgramCategories;

namespace VictoryCenter.BLL.Queries.Admin.ProgramCategories;

public record GetProgramCategoriesQuery : IRequest<Result<List<ProgramCategoryDto>>>;
