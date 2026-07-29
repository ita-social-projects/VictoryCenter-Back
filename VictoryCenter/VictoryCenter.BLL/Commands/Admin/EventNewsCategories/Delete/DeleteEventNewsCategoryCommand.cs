using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.EventNewsCategories.Delete;

public record DeleteEventNewsCategoryCommand(long Id) : IRequest<Result<long>>;
