using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Test.DeleteTestData;

public record DeleteTestDataCommand(int Id)
    : IRequest<Result<int>>;
