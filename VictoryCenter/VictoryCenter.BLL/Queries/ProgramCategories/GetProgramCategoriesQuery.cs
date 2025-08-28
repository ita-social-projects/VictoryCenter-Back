using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.ProgramCategories;

namespace VictoryCenter.BLL.Queries.ProgramCategories;

public record GetProgramCategoriesQuery() : IRequest<Result<List<ProgramCategoryDto>>>;
