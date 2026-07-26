using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.EventNewsCategories;

namespace VictoryCenter.BLL.Queries.Admin.EventNewsCategories.GetAll;

public record GetAllEventNewsCategoriesQuery : IRequest<Result<List<AdminEventNewsCategoryDto>>>;
